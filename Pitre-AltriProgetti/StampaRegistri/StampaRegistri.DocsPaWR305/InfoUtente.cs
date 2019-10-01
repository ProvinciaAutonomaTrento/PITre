using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Collections;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class InfoUtente
	{
		private string idCorrGlobaliField;

		private string idPeopleField;

		private string userIdField;

		private string dstField;

		private string idGruppoField;

		private string idAmministrazioneField;

		private string sedeField;


        //LA aggiunte da DocsPAWS 18-11-2016
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

        //LA FINE aggiunte 18-11-2016
		public string idCorrGlobali
		{
			get
			{
				return this.idCorrGlobaliField;
			}
			set
			{
				this.idCorrGlobaliField = value;
			}
		}

		public string idPeople
		{
			get
			{
				return this.idPeopleField;
			}
			set
			{
				this.idPeopleField = value;
			}
		}

		public string userId
		{
			get
			{
				return this.userIdField;
			}
			set
			{
				this.userIdField = value;
			}
		}

		public string dst
		{
			get
			{
				return this.dstField;
			}
			set
			{
				this.dstField = value;
			}
		}

		public string idGruppo
		{
			get
			{
				return this.idGruppoField;
			}
			set
			{
				this.idGruppoField = value;
			}
		}

		public string idAmministrazione
		{
			get
			{
				return this.idAmministrazioneField;
			}
			set
			{
				this.idAmministrazioneField = value;
			}
		}

		public string sede
		{
			get
			{
				return this.sedeField;
			}
			set
			{
				this.sedeField = value;
			}
		}

        public InfoUtente()
        {
        }

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
