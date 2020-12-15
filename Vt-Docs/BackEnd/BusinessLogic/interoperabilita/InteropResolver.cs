using System;
using System.Data;
using System.Globalization;
using log4net;
using System.Net;
using System.Linq;
using System.Configuration;
using System.Web;

namespace BusinessLogic.Interoperabilità
{
    /// <summary>
    /// </summary>
    public class InteropResolver : System.Xml.XmlUrlResolver
    {
        private static ILog logger = LogManager.GetLogger(typeof(InteropResolver));
        /// <summary>
        /// </summary>
        /// <param name="absoluteUri"></param>
        /// <param name="role"></param>
        /// <param name="ofObjectToReturn"></param>
        /// <returns></returns>
        //override public object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        //{
        //    if (absoluteUri.AbsoluteUri.EndsWith("daticert.xml"))
        //    {
        //        //String filename = DocsPaWebService.Path + "\\interoperabilita\\dtd\\Segnatura.dtd";
        //        String filename = AppDomain.CurrentDomain.BaseDirectory;
        //        filename += System.Configuration.ConfigurationManager.AppSettings.Get("DTD_DATICERT_PATH");
        //        System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
        //        return fs;
        //    }
        //    else
        //        //The filename that the cached stream will be stored to.
        //        //OLD if(absoluteUri.AbsoluteUri.EndsWith("Segnatura.dtd"))
        //        if (absoluteUri.AbsoluteUri.ToLower().EndsWith("segnatura.dtd") || (absoluteUri.AbsoluteUri.ToLower().StartsWith("http")))
        //        {
        //            //String filename = DocsPaWebService.Path + "\\interoperabilita\\dtd\\Segnatura.dtd";
        //            String filename = AppDomain.CurrentDomain.BaseDirectory;
        //            filename += System.Configuration.ConfigurationManager.AppSettings.Get("DTD_SEGNATURA_PATH");
        //            System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
        //            return fs;
        //        }
        //        else
        //            return new System.IO.FileStream(absoluteUri.AbsolutePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
        //}

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            Uri retVal;

            String keyVal = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CHECK_INTEROP_DTD");
            if (relativeUri.ToLower().EndsWith("dtd"))
            {
                String fileName = baseUri.Segments.Last();
                retVal = String.IsNullOrEmpty(keyVal) || keyVal == "0" ? this.GetDTDPath(baseUri.AbsoluteUri) : base.ResolveUri(baseUri, relativeUri);
            }
            else
                retVal = base.ResolveUri(baseUri, relativeUri);

            return retVal;

        }

        public bool IsReachableUri(string uriInput)
        {
            // Variable to Return             
            bool testStatus;

            try
            {
                // Create a request for the URL.
                WebRequest request = WebRequest.Create(uriInput);
                request.Timeout = 1000; // 15 Sec
                WebResponse response;

                response = request.GetResponse();
                testStatus = true;
                // Uri does exist                                  
                response.Close();
            }
            catch (Exception)
            {
                testStatus = false; // Uri does not exist            
            }
            // Result            
            return testStatus;
        }

        public Uri GetDTDPath(String dtdName)
        {

            String keyName = String.Empty;
            if (dtdName.ToLower().EndsWith("daticert.xml"))
                keyName = "DTD_DATICERT_PATH";
            else
                keyName = "DTD_SEGNATURA_PATH";

            String value = String.Empty;
            try
            {
                value = ConfigurationManager.AppSettings.Get(keyName);
            }
            catch (Exception e)
            {

            }

            if (String.IsNullOrEmpty(value))
                throw new NotSupportedException();
            return new Uri(AppDomain.CurrentDomain.BaseDirectory + value);

        }

    }

    public class InteropXsdResolver : System.Xml.XmlUrlResolver
    {
        private static ILog logger = LogManager.GetLogger(typeof(InteropResolver));
        /// <summary>
        /// </summary>
        /// <param name="absoluteUri"></param>
        /// <param name="role"></param>
        /// <param name="ofObjectToReturn"></param>
        /// <returns></returns>
        //override public object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        //{
        //    if (absoluteUri.AbsoluteUri.EndsWith("daticert.xml"))
        //    {
        //        //String filename = DocsPaWebService.Path + "\\interoperabilita\\dtd\\Segnatura.dtd";
        //        String filename = AppDomain.CurrentDomain.BaseDirectory;
        //        filename += System.Configuration.ConfigurationManager.AppSettings.Get("DTD_DATICERT_PATH");
        //        System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
        //        return fs;
        //    }
        //    else
        //        //The filename that the cached stream will be stored to.
        //        //OLD if(absoluteUri.AbsoluteUri.EndsWith("Segnatura.dtd"))
        //        if (absoluteUri.AbsoluteUri.ToLower().EndsWith("segnatura.dtd") || (absoluteUri.AbsoluteUri.ToLower().StartsWith("http")))
        //        {
        //            //String filename = DocsPaWebService.Path + "\\interoperabilita\\dtd\\Segnatura.dtd";
        //            String filename = AppDomain.CurrentDomain.BaseDirectory;
        //            filename += System.Configuration.ConfigurationManager.AppSettings.Get("DTD_SEGNATURA_PATH");
        //            System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
        //            return fs;
        //        }
        //        else
        //            return new System.IO.FileStream(absoluteUri.AbsolutePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
        //}

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            return GetXsdPath();

        }

        public bool IsReachableUri(string uriInput)
        {
            // Variable to Return             
            bool testStatus;

            try
            {
                // Create a request for the URL.
                WebRequest request = WebRequest.Create(uriInput);
                request.Timeout = 1000; // 15 Sec
                WebResponse response;

                response = request.GetResponse();
                testStatus = true;
                // Uri does exist                                  
                response.Close();
            }
            catch (Exception)
            {
                testStatus = false; // Uri does not exist            
            }
            // Result            
            return testStatus;
        }

        public Uri GetXsdPath()
        {

            String keyName = "XSD_SEGNATURA_PATH";

            String value = String.Empty;
            try
            {
                value = ConfigurationManager.AppSettings.Get(keyName);
            }
            catch (Exception e)
            {

            }

            if (String.IsNullOrEmpty(value))
                throw new NotSupportedException();
            return new Uri(AppDomain.CurrentDomain.BaseDirectory + value);

        }

    }
}