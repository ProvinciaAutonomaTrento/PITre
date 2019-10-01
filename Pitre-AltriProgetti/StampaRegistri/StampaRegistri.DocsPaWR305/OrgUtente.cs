using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class OrgUtente
	{
		private string iDCorrGlobaleField;

		private string iDPeopleField;

		private string userIdField;

		private string codiceField;

		private string codiceRubricaField;

		private string nomeField;

		private string cognomeField;

		private string emailField;

		private string sedeField;

		private string passwordField;

		private string abilitatoField;

		private string dominioField;

		private string amministratoreField;

		private string notificaTrasmField;

		private string iDAmministrazioneField;

		public string IDCorrGlobale
		{
			get
			{
				return this.iDCorrGlobaleField;
			}
			set
			{
				this.iDCorrGlobaleField = value;
			}
		}

		public string IDPeople
		{
			get
			{
				return this.iDPeopleField;
			}
			set
			{
				this.iDPeopleField = value;
			}
		}

		public string UserId
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

		public string Codice
		{
			get
			{
				return this.codiceField;
			}
			set
			{
				this.codiceField = value;
			}
		}

		public string CodiceRubrica
		{
			get
			{
				return this.codiceRubricaField;
			}
			set
			{
				this.codiceRubricaField = value;
			}
		}

		public string Nome
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

		public string Cognome
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

		public string Email
		{
			get
			{
				return this.emailField;
			}
			set
			{
				this.emailField = value;
			}
		}

		public string Sede
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

		public string Password
		{
			get
			{
				return this.passwordField;
			}
			set
			{
				this.passwordField = value;
			}
		}

		public string Abilitato
		{
			get
			{
				return this.abilitatoField;
			}
			set
			{
				this.abilitatoField = value;
			}
		}

		public string Dominio
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

		public string Amministratore
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

		public string NotificaTrasm
		{
			get
			{
				return this.notificaTrasmField;
			}
			set
			{
				this.notificaTrasmField = value;
			}
		}

		public string IDAmministrazione
		{
			get
			{
				return this.iDAmministrazioneField;
			}
			set
			{
				this.iDAmministrazioneField = value;
			}
		}
	}
}
