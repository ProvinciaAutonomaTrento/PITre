using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Utente : Corrispondente
	{
		private string idPeopleField;

		private string dstField;

		private string userIdField;

		private string notificaField;

		private string telefonoField;

		private string nomeField;

		private string cognomeField;

		private string dominioField;

		private bool amministratoreField;

		private bool assegnanteField;

		private bool assegnatarioField;

		private string sedeField;

        //aggiunta 18-11-2016 Luisa Antonelli
        public string urlWA; //url della wa dove è loggato l'utente serve per il link nelle mail delle trasmissioni

        /// <summary>
        /// Indica la matricola associata all'utente
        /// </summary>
        public string matricola;


        /// <summary>
        /// arraylist oggetti extApplication che rappresenta le applicazioni per l'utente
        /// </summary>
        [XmlArray()]
       // [XmlArrayItem(typeof(DocsPaVO.utente.ExtApplication))]
        public System.Collections.ArrayList extApplications;

        /// <summary>
        /// Indica l'appicazione su cui sta lavorando
        /// </summary>
        public string codWorkingApplication;


        //FINE aggiunta 18-11-2016 Luisa Antonelli



		private Ruolo[] ruoliField;

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

		public string notifica
		{
			get
			{
				return this.notificaField;
			}
			set
			{
				this.notificaField = value;
			}
		}

		public string telefono
		{
			get
			{
				return this.telefonoField;
			}
			set
			{
				this.telefonoField = value;
			}
		}

		public string nome
		{
			get
			{
				return this.nomeField;
			}
			set
			{
				this.nomeField = value;
			}
		}

		public string cognome
		{
			get
			{
				return this.cognomeField;
			}
			set
			{
				this.cognomeField = value;
			}
		}

		public string dominio
		{
			get
			{
				return this.dominioField;
			}
			set
			{
				this.dominioField = value;
			}
		}

		public bool amministratore
		{
			get
			{
				return this.amministratoreField;
			}
			set
			{
				this.amministratoreField = value;
			}
		}

		public bool assegnante
		{
			get
			{
				return this.assegnanteField;
			}
			set
			{
				this.assegnanteField = value;
			}
		}

		public bool assegnatario
		{
			get
			{
				return this.assegnatarioField;
			}
			set
			{
				this.assegnatarioField = value;
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

		public Ruolo[] ruoli
		{
			get
			{
				return this.ruoliField;
			}
			set
			{
				this.ruoliField = value;
			}
		}
	}
}
