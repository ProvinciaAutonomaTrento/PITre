using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Rubrica.ClientProxy.RubricaServices;

namespace Rubrica.ClientProxy
{
    /// <summary>
    /// Proxy per la gestione dei servizi della rubrica
    /// </summary>
    public class RubricaServiceProxy
    {
        /// <summary>
        /// 
        /// </summary>
        private SecurityCredentials _credentials = null;

        /// <summary>
        /// 
        /// </summary>
        private string _remoteAddress = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="serviceRoot"></param>
        public RubricaServiceProxy(SecurityCredentials credentials, string serviceRoot)
        {
            this._credentials = credentials;
            this._remoteAddress = string.Format("{0}RubricaServices.asmx", serviceRoot);
        }

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteriRicerca"></param>
        /// <param name="criteriPaginazione"></param>
        /// <returns></returns>
        public ElementoRubrica[] Search(OpzioniRicerca opzioniRicerca)
        {
            return GetService().Search(new SearchRequest(this.GetCredentials(), opzioniRicerca)).SearchResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ElementoRubrica Get(int id)
        {
            return GetService().Get(new GetRequest(this.GetCredentials(), id)).GetResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ElementoRubrica Insert(ElementoRubrica data)
        {
            return GetService().Insert(new InsertRequest(GetCredentials(), data)).InsertResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ElementoRubrica Update(ElementoRubrica data)
        {
            return GetService().Update(new UpdateRequest(this.GetCredentials(), data)).UpdateResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            GetService().Delete(new DeleteRequest(this.GetCredentials(), id));
        }

        #endregion

        #region Factory methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private RubricaServicesSoap GetService()
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            EndpointAddress endpoint = new EndpointAddress(this._remoteAddress);

            return new RubricaServicesSoapClient(binding, endpoint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private SecurityCreadentialsSoapHeader GetCredentials()
        {
            return new SecurityCreadentialsSoapHeader
            {
                UserName = this._credentials.UserName, 
                Password = this._credentials.Password
            };
        }

        #endregion
    }
}