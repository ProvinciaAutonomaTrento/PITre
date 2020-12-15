using System;
using System.Data;
using System.Configuration;
using System.Web.Services.Protocols;
using RubricaComune;
using RubricaComune.Proxy.Elementi;

namespace RubricaComune
{
    /// <summary>
    /// 
    /// </summary>
    public class ElementiRubricaServices : IDisposable
    {
        /// <summary>
        /// Istanza del servizio
        /// </summary>
        protected RubricaServices Services { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ElementiRubricaServices(string serviceRoot, string userName, string password)
        {
            this.Services = this.CreateInstance(serviceRoot, userName, password);
        }

        /// <summary>
        /// Creazione istanza servizio RubricaServices
        /// </summary>
        /// <param name="serviceRoot"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private RubricaServices CreateInstance(string serviceRoot, string userName, string password)
        {
            RubricaServices services = new RubricaServices();

            services.Url = string.Format("{0}/RubricaServices.asmx", serviceRoot);
            services.SecurityCreadentialsSoapHeaderValue = new RubricaComune.Proxy.Elementi.SecurityCreadentialsSoapHeader();
            services.SecurityCreadentialsSoapHeaderValue.UserName = userName;
            services.SecurityCreadentialsSoapHeaderValue.Password = password;
            services.Timeout = 300000;

            return services;
        }

        /// <summary>
        /// Ricerca di un singolo elemento a partire dal codice
        /// </summary>
        /// <param name="codice"></param>
        /// <returns></returns>
        public ElementoRubrica SearchSingle(string codice)
        {
 //           DocsPaUtils.LogsManagement.Debugger.Write("SearchSingle(string codice) START");
            return SearchSingle(codice, TipiRicercaParolaEnum.ParolaIntera);
        }

        /// <summary>
        /// Ricerca di un singolo elemento a partire dal codice
        /// </summary>
        /// <param name="codice"></param>
        /// <param name="tipoRicerca"></param>
        /// <returns></returns>
        public ElementoRubrica SearchSingle(string codice, TipiRicercaParolaEnum tipoRicerca)
        {

         //   DocsPaUtils.LogsManagement.Debugger.Write("SearchSingle(string codice, TipiRicercaParolaEnum tipoRicerca) START");
            ElementoRubrica retValue = null;
           // DocsPaUtils.LogsManagement.Debugger.Write("istanza opzione ricerca");
            OpzioniRicerca opzioniRicerca = new OpzioniRicerca();
           // DocsPaUtils.LogsManagement.Debugger.Write("istanza criterio");
            CriterioRicerca criterio = new CriterioRicerca();
            criterio.Nome = "Codice";
            criterio.Valore = codice.Replace("'", "''");
            criterio.TipoRicerca = tipoRicerca;
            //if(criterio != null)
            //    DocsPaUtils.LogsManagement.Debugger.Write("ho valorizzato criterio con: "+criterio.Nome+";"+criterio.Valore+";"+criterio.TipoRicerca );
            //else
            //    DocsPaUtils.LogsManagement.Debugger.Write("creiterio è null");
            //DocsPaUtils.LogsManagement.Debugger.Write("istazione criteri");
            CriteriRicerca criteri = new CriteriRicerca();
            criteri.Criteri = new CriterioRicerca[1] { criterio };

            //if(criteri != null)
            //    DocsPaUtils.LogsManagement.Debugger.Write("creiterii non è null");
            //else
            //    DocsPaUtils.LogsManagement.Debugger.Write("creiterio è null non ci sono criteri di ricerca");
            opzioniRicerca.CriteriRicerca = criteri;
            //if(opzioniRicerca.CriteriRicerca != null)
            //    DocsPaUtils.LogsManagement.Debugger.Write("opzioniRicerca.CriteriRicerca non è null");
            //else
            //    DocsPaUtils.LogsManagement.Debugger.Write("opzioniRicerca.CriteriRicerca è null");
            //DocsPaUtils.LogsManagement.Debugger.Write("sto per esguire la ricerca");
            ElementoRubrica[] result = this.Search(ref opzioniRicerca);
            
            //if(result.Length >0)
            //    DocsPaUtils.LogsManagement.Debugger.Write("sono stati trovati dei corrispondenti in rubrica comune");
            //else
            //    DocsPaUtils.LogsManagement.Debugger.Write("non sono stati trovati dei corrispondenti in rubrica comune");
            
            if (result.Length > 0)
                retValue = result[0];

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="opzioniRicerca"></param>
        /// <returns></returns>
        public ElementoRubrica[] Search(ref OpzioniRicerca opzioniRicerca)
        {
            //DocsPaUtils.LogsManagement.Debugger.Write("ElementoRubrica[] Search(ref OpzioniRicerca opzioniRicerca) start");
            try
            {
                return this.Services.Search(ref opzioniRicerca);
            }
            catch (SoapException ex)
            {
                throw SoapExceptionParser.GetOriginalException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idElemento"></param>
        /// <returns></returns>
        public ElementoRubrica Get(int idElemento)
        {
            try
            {
                return this.Services.Get(idElemento);
            }
            catch (SoapException ex)
            {
                throw SoapExceptionParser.GetOriginalException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elemento"></param>
        /// <returns></returns>
        public ElementoRubrica Insert(ElementoRubrica elemento)
        {
            try
            {
                return this.Services.Insert(elemento);
            }
            catch (SoapException ex)
            {
                throw SoapExceptionParser.GetOriginalException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elemento"></param>
        /// <returns></returns>
        public ElementoRubrica Update(ElementoRubrica elemento)
        {
            try
            {
                return this.Services.UpdateMulticasella(elemento);
            }
            catch (SoapException ex)
            {
                throw SoapExceptionParser.GetOriginalException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elemento"></param>
        public void Delete(ElementoRubrica elemento)
        {
            try
            {
                this.Services.Delete(elemento);
            }
            catch (SoapException ex)
            {
                throw SoapExceptionParser.GetOriginalException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Services.Dispose();
            this.Services = null;
        }
    }
}