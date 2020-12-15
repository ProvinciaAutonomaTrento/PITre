using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using log4net;
using System.Security.Cryptography;

namespace ArubaConnector
{
    // common to hsm services session managment
    public class SessionManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(SessionManager));

        public string Session_PutFileToSign(string SessionToken, byte[] FileDafirmare, string FileName)
        {
            SessionToken = SessionToken.ToUpper();
            string cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MultiSignWorkDir");
            string sessionDir = Path.Combine(cacheDir, SessionToken);
            if (Directory.Exists(sessionDir))
            {
                string manifestFile = Path.Combine(sessionDir, "Manifest.xml");
                if (File.Exists(manifestFile))
                {
                    String manifestXML = File.ReadAllText(manifestFile);
                    Manifest.ManifestFile mft = Manifest.ManifestFile.Deserialize(manifestXML);

                    SHA256 mySHA256 = SHA256Managed.Create();
                    string sha256Hash = BitConverter.ToString(mySHA256.ComputeHash(FileDafirmare)).Replace("-", "");

                    foreach (Manifest.MainfestFileInformation FileInformation in mft.FileInformation)
                    {
                        if (FileInformation.hash.ToUpper() == sha256Hash.ToUpper())
                        {
                            //file esiste nel manifest uscire.
                            return null;
                        }
                    }
                    try
                    {
                        string SignFileName = Guid.NewGuid().ToString() + FileName;
                        mft.FileInformation.Add(new Manifest.MainfestFileInformation { hash = sha256Hash.ToUpper(), OriginalFullName = SignFileName });
                        File.WriteAllBytes(Path.Combine(sessionDir, SignFileName), FileDafirmare);
                        File.WriteAllText(manifestFile, mft.Serialize());
                        return sha256Hash;
                    }
                    catch (Exception e)
                    {
                        //errori scrivendo il file o il manifest.. uscire con null
                        //return null;
                        logger.ErrorFormat("Errore in PutFileToSign  {0} stk {1}", e.Message, e.StackTrace);
                        throw e;
                    }
                }
            }
            return null;
        }

        public string Session_GetSessions()
        {
            string retval = "";
            string cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MultiSignWorkDir");
            string[] dirs = Directory.GetDirectories(cacheDir);
            foreach (string dir in dirs)
                retval += dir + ":";
            return retval;
        }

        public string Session_GetManifest(string SessionToken)
        {
            SessionToken = SessionToken.ToUpper();
            string cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MultiSignWorkDir");
            string sessionDir = Path.Combine(cacheDir, SessionToken);
            if (Directory.Exists(sessionDir))
            {
                string manifestFile = Path.Combine(sessionDir, "Manifest.xml");
                if (File.Exists(manifestFile))
                {
                    String manifestXML = File.ReadAllText(manifestFile);
                    return manifestXML;
                }
            }
            return null;
        }

        public byte[] Session_GetSignedFile(string SessionToken, string hashFileDaFirmare)
        {
            SessionToken = SessionToken.ToUpper();
            string cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MultiSignWorkDir");
            string sessionDir = Path.Combine(cacheDir, SessionToken);
            if (Directory.Exists(sessionDir))
            {
                string manifestFile = Path.Combine(sessionDir, "Manifest.xml");
                if (File.Exists(manifestFile))
                {
                    String manifestXML = File.ReadAllText(manifestFile);
                    Manifest.ManifestFile mft = Manifest.ManifestFile.Deserialize(manifestXML);

                    foreach (Manifest.MainfestFileInformation FileInformation in mft.FileInformation)
                    {
                        if (FileInformation.hash.ToUpper() == hashFileDaFirmare.ToUpper())
                        {
                            //file esiste nel manifest leggere e uscire.
                            //per test, poi commentare, se no torna solo e sempre quello inviato (echo)
                            //return File.ReadAllBytes(Path.Combine(sessionDir, FileInformation.OriginalFullName));
                            try
                            {
                                return File.ReadAllBytes(Path.Combine(sessionDir, FileInformation.SignedFullName));
                            }
                            catch
                            {
                                logger.ErrorFormat("Il file {0} | {1} non è leggibile",sessionDir,FileInformation.SignedFullName);
                                return null;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public string OpenMultiSignSession(bool cosign, bool timestamp, int Type)
        {
            string cacheDir = AppDomain.CurrentDomain.BaseDirectory + "MultiSignWorkDir";
            string sessionToken = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            try
            {
                DirectoryInfo di = System.IO.Directory.CreateDirectory(String.Format("{0}\\{1}", cacheDir, sessionToken));
                Manifest.SignType st = (Manifest.SignType)Type;
                Manifest.ManifestFile m = new Manifest.ManifestFile { SignatureType = st, timestamp = timestamp, Token = sessionToken };
                File.WriteAllText(Path.Combine(di.FullName, "Manifest.xml"), m.Serialize());
                return sessionToken;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore in OpenMultiSignSession  {0} stk {1}", e.Message, e.StackTrace);
                throw e;
            }
        }

        public bool Session_CloseMultiSign(string SessionToken)
        {
            string cacheDir = AppDomain.CurrentDomain.BaseDirectory + "MultiSignWorkDir";

            string dir = String.Format("{0}\\{1}", cacheDir, SessionToken);
            if (System.IO.Directory.Exists(dir))
            {
                try
                {
                    System.IO.Directory.Delete(dir, true);
                    return true;
                }
                catch (Exception e)
                {
                    logger.ErrorFormat("Errore in CloseMultiSign {0} stk {1}", e.Message, e.StackTrace);
                    throw e;
                }
            }
            return false;
        }
    }
}