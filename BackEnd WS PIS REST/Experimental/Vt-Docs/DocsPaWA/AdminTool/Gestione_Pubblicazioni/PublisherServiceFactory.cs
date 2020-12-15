using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPAWA.AdminTool.Gestione_Pubblicazioni
{
    /// <summary>
    /// Factory class del servizio di pubblicazione
    /// </summary>
    public sealed class PublisherServiceFactory
    {
        /// <summary>
        /// Creazione servizio per la pubblicazione
        /// </summary>
        /// <returns></returns>
        public static Publisher.Proxy.PublisherWebService Create()
        {
            Publisher.Proxy.PublisherWebService instance = new Publisher.Proxy.PublisherWebService();
            instance.Url = ServiceUrl;
            return instance;
        }

        /// <summary>
        /// Costruzione dell'url del servizio web di pubblicazione
        /// </summary>
        public static string ServiceUrl
        {
            get
            {
                string serviceUrl = string.Empty;

                string standardWebServiceUrl = DocsPAWA.Properties.Settings.Default.DocsPaWA_DocsPaWR_DocsPaWebService;

                Uri uri = new Uri(DocsPAWA.Properties.Settings.Default.DocsPaWA_DocsPaWR_DocsPaWebService);

                serviceUrl = uri.Scheme + "://" + uri.Host;

                if (!uri.Port.Equals(80))
                    serviceUrl += ":" + uri.Port;

                const string STANDARD_WS_NAME = "DocsPaWS.asmx";
                // Il servizio di pubblicazione risiede sempre nella cartella "Publisher"
                const string PUBLISHER_WS_RELATIVE_PATH = "Publisher/PublisherWebService.asmx";

                // Costruzione automatica dell'url del servizio di pubblicazione
                // sulla base dei componenti dell'url del servizio standard
                foreach (string segment in uri.Segments)
                {
                    if (string.Compare(STANDARD_WS_NAME, segment, true) == 0)
                    {
                        serviceUrl += PUBLISHER_WS_RELATIVE_PATH;
                        break;
                    }
                    else
                    {
                        serviceUrl += segment;
                    }
                }

                return serviceUrl;
            }
        }
    }
}