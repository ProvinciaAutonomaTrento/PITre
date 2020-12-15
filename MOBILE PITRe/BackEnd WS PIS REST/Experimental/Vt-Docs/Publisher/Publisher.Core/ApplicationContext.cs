using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Publisher
{
    /// <summary>
    /// Classe per la gestione del contesto applicativo
    /// </summary>
    public sealed class ApplicationContext
    {
        /// <summary>
        /// 
        /// </summary>
        private ApplicationContext()
        { }

        /// <summary>
        /// Reperimento nome computer
        /// </summary>
        /// <returns></returns>
        public static string GetMachineName()
        {
            return System.Environment.MachineName;
        }

        /// <summary>
        /// Reperimento url del servizio di pubblicazione per l'installazione corrente
        /// </summary>
        /// <returns></returns>
        public static string GetPublishServiceUrl()
        {
            string retVal = "";
            if (System.Configuration.ConfigurationManager.AppSettings["PUBLISHER_STATIC_URL"] != null)
            {
                retVal = System.Configuration.ConfigurationManager.AppSettings["PUBLISHER_STATIC_URL"].ToString();
            }
            else
            {
                Uri uri = System.Web.HttpContext.Current.Request.Url;

                string serviceUrl = uri.Scheme + "://" + uri.Host;

                if (!uri.Port.Equals(80))
                    serviceUrl += ":" + uri.Port;

                // Il servizio di pubblicazione risiede sempre nella cartella "Publisher"
                const string PUBLISHER_WS_RELATIVE_PATH = "/Publisher/PublisherWebService.asmx";

                serviceUrl += System.Web.HttpContext.Current.Request.ApplicationPath + PUBLISHER_WS_RELATIVE_PATH;
                retVal = serviceUrl;
            }
            return retVal;
        }
    }
}
