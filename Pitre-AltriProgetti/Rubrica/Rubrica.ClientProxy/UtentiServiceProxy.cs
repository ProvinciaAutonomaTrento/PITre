using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Rubrica.ClientProxy.UtentiServices;

namespace Rubrica.ClientProxy
{
    /// <summary>
    /// Proxy per la gestione dei servizi della rubrica
    /// </summary>
    public class UtentiServiceProxy
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
        public UtentiServiceProxy(SecurityCredentials credentials, string serviceRoot)
        {
            this._credentials = credentials;
            this._remoteAddress = string.Format("{0}UtentiServices.asmx", serviceRoot);
        }

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public SecurityCredentialsResult ValidateCredentials()
        {
            return this.GetService().ValidateCredentials(new ValidateCredentialsRequest(this.GetCredentials())).ValidateCredentialsResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>
        public void ChangePassword(ChangePwdSecurityCredentials credentials)
        {
            this.GetService().ChangePassword(new ChangePasswordRequest(this.GetCredentials(), credentials));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteriRicerca"></param>
        /// <param name="criteriPaginazione"></param>
        /// <returns></returns>
        public Utente[] Search(OpzioniRicerca opzioniRicerca)
        {   
            return this.GetService().Search(new SearchRequest(this.GetCredentials(), opzioniRicerca)).SearchResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Utente Get(int id)
        {
            return this.GetService().Get(new GetRequest(this.GetCredentials(), id)).GetResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public Utente Insert(Utente utente)
        {
            return this.GetService().Insert(new InsertRequest(this.GetCredentials(), utente)).InsertResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public Utente Update(Utente utente)
        {
            return this.GetService().Update(new UpdateRequest(this.GetCredentials(), utente)).UpdateResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            this.GetService().Delete(new DeleteRequest(this.GetCredentials(), id));
        }

        #endregion

        #region Factory methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private UtentiServicesSoap GetService()
        {
            Binding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            EndpointAddress endpoint = new EndpointAddress(this._remoteAddress);

            return new UtentiServicesSoapClient(binding, endpoint);
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
