using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPAWA.AdminTool.Gestione_Pubblicazioni
{
    /// <summary>
    /// Factory class del servizio sottoscrittore
    /// </summary>
    public sealed class SubscriberServiceFactory
    {
        /// <summary>
        /// Creazione servizio per il sottoscrittore
        /// </summary>
        /// <param name="serviceUrl">
        /// Url del servizio sottoscrittore
        /// </param>
        /// <returns></returns>
        public static Subscriber.Proxy.SubscriberWebService Create(string serviceUrl)
        {
            Subscriber.Proxy.SubscriberWebService instance = new Subscriber.Proxy.SubscriberWebService();
            instance.Url = serviceUrl;
            return instance;
        }
    }
}