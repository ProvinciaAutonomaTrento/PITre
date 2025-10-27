// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    /// 

    [Serializable()]
    [DataContract]

	public class InfoUtente 
	{
        [DataMember]
        public string idCorrGlobali { get; set; }
        [DataMember]
        public string idPeople { get; set; }
        [DataMember]
        public string userId { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string dst { get; set; }
        [DataMember]
        public string idGruppo { get; set; }
        [DataMember]
        public string idAmministrazione { get; set; }
        [DataMember]
        public string sede { get; set; }
        [DataMember]
        public string urlWA { get; set; } //url della wa dove � loggato l'utente serve per il link nelle mail delle trasmissioni
        [DataMember]
        public InfoUtente delegato { get; set; }
        [DataMember]
        public ArrayList extApplications { get; set; }

        /// <summary>
        /// arraylist oggetti extApplication che rappresenta le applicazioni per l'utente
        /// </summary>
        //[XmlArray()]
        //[XmlArrayItem(typeof(DocsPaVO.utente.ExtApplication))]
        //public System.Collections.ArrayList extApplications ;


        /// <summary>

        [DataMember]
        public string codWorkingApplication { get; set; }

        /// <summary>
        /// Indica la matricola associata all'utente
        /// </summary>
        [DataMember]
        public string matricola { get; set; }

        /// <summary>
        /// Autenticazione Sistemi Esterni
        /// Per i sistemi esterni e gli altri utenti di sistema
        /// </summary>
        [DataMember]
        public string diSistema { get; set; }

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