using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Xml.Serialization;
using Rubrica.Library.Data;

namespace Rubrica
{
    /// <summary>
    /// 
    /// </summary>
    [WebService(Namespace = "http://valueteam.com/rubrica")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class UtentiServices : System.Web.Services.WebService
    {
        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public SecurityCreadentialsSoapHeader Credentials = new SecurityCreadentialsSoapHeader();

        /// <summary>
        /// Validazione credenziali utente
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="amministratore"></param>
        /// <returns></returns>
        [WebMethod(Description = "Validazione credenziali utente")]
        [SoapHeader("Credentials")]
        public Rubrica.Library.Security.SecurityCredentialsResult ValidateCredentials()
        {
            try
            {
                return Rubrica.Library.UtentiServices.ValidateCredentials
                    (
                        new Rubrica.Library.Security.SecurityCredentials
                        {
                            UserName = this.Credentials.UserName,
                            Password = this.Credentials.Password
                        }
                    );
            }
            catch (Exception ex)
            {
                throw SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Modifica password per l'utente
        /// </summary>
        /// <param name="credentials"></param>
        [WebMethod(Description = "Modifica password per l'utente")]
        [SoapHeader("Credentials")]
        public void ChangePassword(Rubrica.Library.Security.ChangePwdSecurityCredentials credentials)
        {
            try
            {
                SecurityHelper.Login(this.Credentials);

                Rubrica.Library.UtentiServices.ChangePassword(credentials);
            }
            catch (Exception ex)
            {
                throw SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Ricerca di utenti nella rubrica
        /// </summary>
        /// <param name="opzioniRicerca"></param>
        /// <returns></returns>
        [WebMethod(Description = "Ricerca di utenti nella rubrica")]
        [SoapHeader("Credentials")]
        [XmlInclude(typeof(CampiRicercaUtentiEnum))]
        public Utente[] Search(ref OpzioniRicerca opzioniRicerca)
        {
            try
            {
                SecurityHelper.Login(this.Credentials);

                return Rubrica.Library.UtentiServices.Search(opzioniRicerca);
            }
            catch (Exception ex)
            {
                throw SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Reperimento di un elemento utente in rubrica
        /// </summary>
        /// <param name="codice"></param>        
        /// <returns></returns>
        [WebMethod(Description = "Reperimento di un utente esistente in rubrica")]
        [SoapHeader("Credentials")]
        public Utente Get(int id)
        {
            try
            {
                SecurityHelper.Login(this.Credentials);

                return Rubrica.Library.UtentiServices.Get(id);
            }
            catch (Exception ex)
            {
                throw SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Inserimento di un nuovo utente in rubrica
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [WebMethod(Description = "Inserimento di un nuovo utente in rubrica")]
        [SoapHeader("Credentials")]
        public Utente Insert(Utente utente)
        {
            try
            {
                SecurityHelper.Login(this.Credentials);

                return Rubrica.Library.UtentiServices.Insert(utente);
            }
            catch (Exception ex)
            {
                throw SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        [WebMethod(Description = "Aggiornamento di un utente esistente in rubrica")]
        [SoapHeader("Credentials")]
        public Utente Update(Utente utente)
        {
            try
            {
                SecurityHelper.Login(this.Credentials);

                return Rubrica.Library.UtentiServices.Update(utente);
            }
            catch (Exception ex)
            {
                throw SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Rimozione di un utente in rubrica
        /// </summary>
        /// <param name="utente"></param>
        [WebMethod(Description = "Cancellazione di un utente esistente in rubrica")]
        [SoapHeader("Credentials")]
        public void Delete(Utente utente)
        {
            try
            {
                SecurityHelper.Login(this.Credentials);

                Rubrica.Library.UtentiServices.Delete(utente);
            }
            catch (Exception ex)
            {
                throw SoapExceptionFactory.Create(ex);
            }
        }

        #endregion
    }
}
