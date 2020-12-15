using System;
using System.Net;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Security.Permissions;
using System.Web;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using log4net;


namespace RetreiveCRL
{

    public class GetCRL
    {
        private static ILog logger = LogManager.GetLogger(typeof(GetCRL));

        byte[] crl = null;
        bool noCache = false;
        string cachePath = null;
        int timeOut = 10;

        public int TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }


        public string CachePath
        {
            get { return cachePath; }
            set { cachePath = value; }
        }

        public bool NoCache
        {
            set { noCache = value; }
        }

        public byte[] CertificationRevocationListBinary
        {
            get { return crl; }
        }
       

        public void GetCRLFromDistributionList(string crlUrl)
        {
            logger.DebugFormat("Got distribution point: {0}", crlUrl);
            try
            {
                Uri u = new Uri(crlUrl);

                string urlHash = CalculateMD5Hash(crlUrl);
                if (!noCache)
                {
                    crl = getCrlFile(urlHash);
                    logger.Debug("Got CRL");
                }
                else
                {
                    logger.Debug("Forcing CRL download");
                    crl = null;
                }

                if (crl == null)
                {
                    if (u.Scheme == "ldap")
                        crl = GetLdapCRL(crlUrl);
                    else if (u.Scheme == "http")
                        crl = getHttpCrl(crlUrl);
                    else if (u.Scheme == "https")
                        crl = getHttpCrl(crlUrl);
                    else if (u.Scheme == "ftp")
                        crl = getHttpCrl(crlUrl);
                    else
                    {
                        logger.DebugFormat("Unknow scheme {0} (only ldap/http|s/ftp allowed)", u.Scheme);
                    }

                    if (crl != null)
                        writeCrlFile(crlUrl, crl);
                }
            }
            catch (Exception e)
            {
                logger.DebugFormat("Bad URI {0} {1} {2}", crlUrl, e.Message,e.StackTrace );
                throw e;
            }
        }

        public byte[] getHttpCrl(string httpUrl)
        {
            byte[] retval = null;
            logger.DebugFormat("Getting from http {0}", httpUrl);
            ServicePointManager.ServerCertificateValidationCallback +=
                delegate(object sender, X509Certificate certificate, X509Chain chain,
                SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };

            MyWebClient wc = new MyWebClient(timeOut);
            try
            {
                retval = wc.DownloadData(httpUrl);
            }
            catch (Exception e)
            {
                logger.DebugFormat("Error Downloading HTTP/S crl from {0}, Msg {1}, stack {2}", httpUrl, e.Message, e.StackTrace);
            }
            return retval;
        }

        public byte[] GetLdapCRL(string ldapUrl)
        {
            Uri u = new Uri(ldapUrl);
            string server = String.Format("{0}:{1}", u.Host, u.Port);
            LdapConnection conn = new LdapConnection(new LdapDirectoryIdentifier(server));
            conn.AuthType = AuthType.Anonymous;
            conn.Timeout = TimeSpan.FromMinutes(timeOut); //due minuti
            logger.DebugFormat("Ldap BIND....{0}", server);
            conn.Bind();
            logger.DebugFormat ("Bind OK!");
            string path = u.LocalPath;
            string query = u.Query;
            if (path.StartsWith("/"))
                path = path.Substring(1);

            if (query.StartsWith("?"))
                query = query.Substring(1);

            SearchRequest request = new SearchRequest(path, "(objectClass=*)", System.DirectoryServices.Protocols.SearchScope.Subtree);//"(objectClass=group)"
            SearchResponse response = (SearchResponse)conn.SendRequest(request);
            foreach (SearchResultEntry cl in response.Entries)
            {
                foreach (DirectoryAttribute da in cl.Attributes.Values)
                {
                    if (da.Name.ToLowerInvariant().Contains(query.ToLowerInvariant()))
                    {
                        byte[] fileContents = (byte[])(da.GetValues(typeof(byte[]))[0]);
                        //System.IO.File.WriteAllBytes(query + ".crl", fileContents);
                        logger.DebugFormat ("Got {0} bytes of CRL file",fileContents.Length);
                        return fileContents;
                    }
                }
            }

            //byte[] fileContents = (byte[])(resultEntry.Attributes["userCertificate"].GetValues(typeof(byte[]))[0]);    
            //Console.WriteLine("done {0}", response.Entries.Count);

            return null;
        }

        public byte[] getCrlFile(string path)
        {
            logger.DebugFormat("Start");
            string fileName = null;

            if (!System.IO.Directory.Exists(cachePath))
                System.IO.Directory.CreateDirectory(cachePath);

            
            string[] files = System.IO.Directory.GetFiles(cachePath, path + ".*.crl");
            //Non ho file, o ne ho più di uno, per non saper ne leggere ne scrivere lo cancello e rifaccio la retreive
            if ((files.Length > 1) || (files.Length == 0))
            {
                if (files.Length > 1)
                    logger.DebugFormat("more than one {0}.*.crl found , will delete", path);

                if (files.Length == 0)
                    logger.DebugFormat("No {0}.*.crl found , will Download", path);

                foreach (string f in files)
                {
                    logger.DebugFormat("Deleting {0}", f);
                    System.IO.File.Delete(System.IO.Path.Combine(cachePath, f));
                }
                return null;
            }
            fileName = files[0].ToUpper();
            string[] parts = System.IO.Path.GetFileNameWithoutExtension(fileName).Split('.');

            //scaduto da piu di 24 h
            if (System.IO.File.GetLastWriteTime(fileName) < DateTime.Now.AddDays(-1))
            {
                logger.DebugFormat("Crl File older than retension time, Deleting {0}", System.IO.Path.Combine(cachePath, fileName));
                System.IO.File.Delete(System.IO.Path.Combine(cachePath, fileName));
                return null;
            }

            //leggo il file
            byte[] crlFile = System.IO.File.ReadAllBytes(System.IO.Path.Combine(cachePath, fileName));

            if (parts.Length == 2)
            {
                string hash = parts[1];
                string fileHash = CalculateMD5Hash(crlFile);
                if (hash != fileHash) //l'hash non corrisponde ... cancello il file
                {
                    logger.DebugFormat("Crl file corrupted , hash {0} != {1} , deleting", hash, fileHash);
                    System.IO.File.Delete(System.IO.Path.Combine(cachePath, fileName));
                    return null;
                }
            }
            logger.DebugFormat("Retreived {0} bytes from cache", crlFile.Length);
            return crlFile;
        }

        public void writeCrlFile(string distributionPoint, byte[] crlFile)
        {
            string fileName = String.Format("{0}.{1}.crl", CalculateMD5Hash(distributionPoint), CalculateMD5Hash(crlFile));
            logger.DebugFormat("Writing {0} of {1} bytes on cache", fileName, crlFile.Length);
            if (!System.IO.Directory.Exists(cachePath))
                System.IO.Directory.CreateDirectory(cachePath);

            System.IO.File.WriteAllBytes(System.IO.Path.Combine(cachePath, fileName), crlFile);
        }

        public string CalculateMD5Hash(byte[] inbyte)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] hash = md5.ComputeHash(inbyte);
            return BitConverter.ToString(hash).Replace("-", "");
        }

        public string CalculateMD5Hash(string input)
        {
            return CalculateMD5Hash(System.Text.Encoding.ASCII.GetBytes(input));
        }

    }

    class MyWebClient : WebClient
    {
        int timeout;
        public MyWebClient(int t )
        {
            timeout = t;
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = timeout * 60 * 1000;
            return w;
        }
    }
}
