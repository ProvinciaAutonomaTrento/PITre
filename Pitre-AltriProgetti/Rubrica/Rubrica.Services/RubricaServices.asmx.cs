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
    public class RubricaServices : System.Web.Services.WebService
    {
        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public SecurityCreadentialsSoapHeader Credentials = new SecurityCreadentialsSoapHeader();

        /// <summary>
        /// Ricerca di elementi nella rubrica
        /// </summary>
        /// <param name="criteriRicerca"></param>
        /// <param name="criteriPaginazione"></param>
        /// <returns></returns>
        [WebMethod(Description = "Ricerca di elementi nella rubrica")]
        [SoapHeader("Credentials")]
        public ElementoRubrica[] Search(ref OpzioniRicerca opzioniRicerca)
        {
            try
            {
                SecurityHelper.Login(this.Credentials);
                
                return Rubrica.Library.RubricaServices.Search(opzioniRicerca);
            }
            catch (Exception ex)
            {
                throw SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Reperimento di un elemento esistente in rubrica
        /// </summary>
        /// <param name="codice"></param>        
        /// <returns></returns>
        [WebMethod(Description = "Reperimento di un elemento esistente in rubrica")]
        [SoapHeader("Credentials")]
        public ElementoRubrica Get(int id)
        {
            try
            {
                SecurityHelper.Login(this.Credentials);

                return Rubrica.Library.RubricaServices.Get(id);
            }
            catch (Exception ex)
            {
                throw SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elemento"></param>
        /// <returns></returns>
        [WebMethod(Description = "Inserimento di un nuovo elmemento in rubrica")]
        [SoapHeader("Credentials")]
        public ElementoRubrica Insert(ElementoRubrica elemento)
        {
            try
            {
                SecurityHelper.Login(this.Credentials);

                return Rubrica.Library.RubricaServices.Insert(elemento);
            }
            catch (Exception ex)
            {
                throw SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elemento"></param>
        /// <returns></returns>
        [WebMethod(Description = "Aggiornamento di un elmemento esistente in rubrica")]
        [SoapHeader("Credentials")]
        public ElementoRubrica Update(ElementoRubrica elemento)
        {
            try
            {
                SecurityHelper.Login(this.Credentials);

                // Vecchia gestione, corrispondente monocasella
                return Rubrica.Library.RubricaServices.Update(elemento, false);
            }
            catch (Exception ex)
            {
                throw SoapExceptionFactory.Create(ex);
            }
        }

        /// <summary>
        /// Rimozione di un elemento in rubrica
        /// </summary>
        /// <param name="elemento"></param>
        [WebMethod(Description = "Cancellazione di un elmemento esistente in rubrica")]
        [SoapHeader("Credentials")]
        public void Delete(ElementoRubrica elemento)
        {
            try
            {
                SecurityHelper.Login(this.Credentials);

                Rubrica.Library.RubricaServices.Delete(elemento);
            }
            catch (Exception ex)
            {
                throw SoapExceptionFactory.Create(ex);
            }
        }

        [WebMethod(Description = "Aggiornamento di un elmemento multicasella presente in rubrica")]
        [SoapHeader("Credentials")]
        public ElementoRubrica UpdateMulticasella(ElementoRubrica elemento)
        {
            try
            {
                SecurityHelper.Login(this.Credentials);

                // Nuova gestione, corrispondente multicasella
                return Rubrica.Library.RubricaServices.Update(elemento, true);
            }
            catch (Exception ex)
            {
                throw SoapExceptionFactory.Create(ex);
            }
        }

        #endregion
    }
}