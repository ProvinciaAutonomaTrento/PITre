using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
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

		private bool amministratoreField;

		private bool assegnanteField;

		private bool assegnatarioField;

		private bool docElettrField;

		private bool docCartField;

		private Ruolo[] ruoliField;

		private string dominioField;

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

		public bool docElettr
		{
			get
			{
				return this.docElettrField;
			}
			set
			{
				this.docElettrField = value;
			}
		}

		public bool docCart
		{
			get
			{
				return this.docCartField;
			}
			set
			{
				this.docCartField = value;
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
	}
}
