using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    /// 

    [Serializable()]

	public class InfoUtente 
	{
		public string idCorrGlobali;
		public string idPeople;
		public string userId;
		public string dst;
		public string idGruppo;
		public string idAmministrazione;
		public string sede;
		public string urlWA; //url della wa dove è loggato l'utente serve per il link nelle mail delle trasmissioni
        public InfoUtente delegato;
       public ArrayList extApplications;

        /// <summary>
        /// arraylist oggetti extApplication che rappresenta le applicazioni per l'utente
        /// </summary>
        //[XmlArray()]
        //[XmlArrayItem(typeof(DocsPaVO.utente.ExtApplication))]
        //public System.Collections.ArrayList extApplications ;

     
        /// <summary>

        public string codWorkingApplication;

        /// <summary>
        /// Indica la matricola associata all'utente
        /// </summary>
        public string matricola;

        /// <summary>
        /// Autenticazione Sistemi Esterni
        /// Per i sistemi esterni e gli altri utenti di sistema
        /// </summary>
        public string diSistema;

		/// <summary>
		/// </summary>
		public InfoUtente() 
		{
		}

		/// <summary></summary>
		/// <param name="ut"></param>
		/// <param name="ruo"></param>
		public InfoUtente(Utente ut, Ruolo ruo) 
		{
            if (ut != null)
            {
                this.idPeople = ut.idPeople;
                this.userId = ut.userId;
                this.dst = ut.dst;
                this.idAmministrazione = ut.idAmministrazione;
                this.sede = ut.sede;
                this.urlWA = ut.urlWA;
                this.matricola = ut.matricola;
                this.extApplications = ut.extApplications;
                this.codWorkingApplication = ut.codWorkingApplication;
            }

            //BUG - A Volte utente arriva con idAmm nullo
            if (ruo != null)
            {
                this.idCorrGlobali = ruo.systemId;
                this.idGruppo = ruo.idGruppo;
                if (ut != null && string.IsNullOrEmpty(ut.idAmministrazione))
                {
                    this.idAmministrazione = ruo.idAmministrazione;
                }
            }
		}
		
	}
}