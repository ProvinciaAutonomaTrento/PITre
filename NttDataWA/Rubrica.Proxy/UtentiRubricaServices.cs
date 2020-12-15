using System;
using System.Data;
using System.Configuration;
using System.Web.Services.Protocols;
using RubricaComune;
using RubricaComune.Proxy.Utenti;

namespace RubricaComune
{
    /// <summary>
    /// 
    /// </summary>
    public class UtentiRubricaServices : IDisposable
    {
        /// <summary>
        /// Istanza del servizio
        /// </summary>
        protected UtentiServices Services { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UtentiRubricaServices(string serviceRoot, string userName, string password)
        {
            this.Services = CreateInstance(serviceRoot, userName, password);
        }

        /// <summary>
        /// Ricerca di un singolo utente a partire dal nome
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public Utente SearchSingle(string nome)
        {
            Utente retValue = null;

            OpzioniRicerca opzioniRicerca = new OpzioniRicerca();

            CriterioRicerca critero = new CriterioRicerca();
            critero.Nome = "Nome";
            critero.Valore = nome;
            CriteriRicerca criteri = new CriteriRicerca();
            criteri.Criteri = new CriterioRicerca[1] { critero };
            opzioniRicerca.CriteriRicerca = criteri;

            Utente[] result = this.Search(ref opzioniRicerca);

            if (result.Length > 0)
                retValue = result[0];

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="opzioniRicerca"></param>
        /// <returns></returns>
        public Utente[] Search(ref OpzioniRicerca opzioniRicerca)
        {
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
        /// <param name="idUtente"></param>
        /// <returns></returns>
        public Utente Get(int idUtente)
        {
            try
            {
                return this.Services.Get(idUtente);
            }
            catch (SoapException ex)
            {
                throw SoapExceptionParser.GetOriginalException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public Utente Insert(Utente utente)
        {
            try
            {
                return this.Services.Insert(utente);
            }
            catch (SoapException ex)
            {
                throw SoapExceptionParser.GetOriginalException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public Utente Update(Utente utente)
        {
            try
            {
                return this.Services.Update(utente);
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
        public void Delete(Utente utente)
        {
            try
            {
                this.Services.Delete(utente);
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

        /// <summary>
        /// Creazione istanza servizio UtentiServices
        /// </summary>
        /// <param name="serviceRoot"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static UtentiServices CreateInstance(string serviceRoot, string userName, string password)
        {
            UtentiServices services = new UtentiServices();

            services.Url = string.Format("{0}UtentiServices.asmx", serviceRoot);
            services.SecurityCreadentialsSoapHeaderValue = new RubricaComune.Proxy.Utenti.SecurityCreadentialsSoapHeader();
            services.SecurityCreadentialsSoapHeaderValue.UserName = userName;
            services.SecurityCreadentialsSoapHeaderValue.Password = password;

            return services;
        }
    }
}
