using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaDocumentale_OCS.OCSServices
{
    /// <summary>
    /// Class factory per la crezione di istanze dei servizi OCS
    /// </summary>
    public sealed class OCSServiceFactory
    {
        #region Private members

        /// <summary>
        /// 
        /// </summary>
        private OCSServiceFactory() { }

        #endregion

        #region Factory methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetDocumentServiceInstance<T>() 
            where T : System.Web.Services.Protocols.SoapHttpClientProtocol 
        {
            T instance = Activator.CreateInstance<T>();
            SetServiceInstanceUrl("DocumentServices", instance);
            return instance;
        }

        /// <summary>
        /// Impostazione dell'url del webservice
        /// </summary>
        /// <param name="module"></param>
        /// <param name="serviceInstance"></param>
        private static void SetServiceInstanceUrl(string module, System.Web.Services.Protocols.SoapHttpClientProtocol serviceInstance)
        {
            string httpBindingClassName = serviceInstance.GetType().Name;
            string serviceName = httpBindingClassName.Remove(httpBindingClassName.IndexOf("SOAPHTTPBinding"));

            serviceInstance.Url = string.Concat(OCSConfigurations.GetServiceUrl(), string.Format("{0}/{1}SOAPHTTPPort", module, serviceName));
        }

        #endregion
    }
}
