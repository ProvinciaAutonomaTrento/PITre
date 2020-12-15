using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlInclude(typeof(UnitaOrganizzativa)), XmlInclude(typeof(Ruolo)), XmlInclude(typeof(Utente)), XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Corrispondente
	{
		private string systemIdField;

		private string descrizioneField;

		private string codiceCorrispondenteField;

		private string codiceRubricaField;

		private string idAmministrazioneField;

		private string tipoCorrispondenteField;

		private string tipoIEField;

		private string idRegistroField;

		private Canale canalePrefField;

		private bool dettagliField;

		private DataSet infoField;

		private ServerPosta serverPostaField;

		private string idOldField;

		private string emailField;

		private bool notificaConAllegatoField;

		private string erroreField;

		private string codiceAOOField;

		private string codiceAmmField;

		public string systemId
		{
			get
			{
				return this.systemIdField;
			}
			set
			{
				this.systemIdField = value;
			}
		}

		public string descrizione
		{
			get
			{
				return this.descrizioneField;
			}
			set
			{
				this.descrizioneField = value;
			}
		}

		public string codiceCorrispondente
		{
			get
			{
				return this.codiceCorrispondenteField;
			}
			set
			{
				this.codiceCorrispondenteField = value;
			}
		}

		public string codiceRubrica
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

		public string tipoCorrispondente
		{
			get
			{
				return this.tipoCorrispondenteField;
			}
			set
			{
				this.tipoCorrispondenteField = value;
			}
		}

		public string tipoIE
		{
			get
			{
				return this.tipoIEField;
			}
			set
			{
				this.tipoIEField = value;
			}
		}

		public string idRegistro
		{
			get
			{
				return this.idRegistroField;
			}
			set
			{
				this.idRegistroField = value;
			}
		}

		public Canale canalePref
		{
			get
			{
				return this.canalePrefField;
			}
			set
			{
				this.canalePrefField = value;
			}
		}

		public bool dettagli
		{
			get
			{
				return this.dettagliField;
			}
			set
			{
				this.dettagliField = value;
			}
		}

		public DataSet info
		{
			get
			{
				return this.infoField;
			}
			set
			{
				this.infoField = value;
			}
		}

		public ServerPosta serverPosta
		{
			get
			{
				return this.serverPostaField;
			}
			set
			{
				this.serverPostaField = value;
			}
		}

		public string idOld
		{
			get
			{
				return this.idOldField;
			}
			set
			{
				this.idOldField = value;
			}
		}

		public string email
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

		public bool notificaConAllegato
		{
			get
			{
				return this.notificaConAllegatoField;
			}
			set
			{
				this.notificaConAllegatoField = value;
			}
		}

		public string errore
		{
			get
			{
				return this.erroreField;
			}
			set
			{
				this.erroreField = value;
			}
		}

		public string codiceAOO
		{
			get
			{
				return this.codiceAOOField;
			}
			set
			{
				this.codiceAOOField = value;
			}
		}

		public string codiceAmm
		{
			get
			{
				return this.codiceAmmField;
			}
			set
			{
				this.codiceAmmField = value;
			}
		}
	}
}
