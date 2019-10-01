using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class AddressbookQueryCorrispondente
	{
		private string systemIdField;

		private string codiceRubricaField;

		private string codiceGruppoField;

		private string descrizioneGruppoField;

		private string codiceUOField;

		private string descrizioneUOField;

		private string codiceRuoloField;

		private string descrizioneRuoloField;

		private string nomeUtenteField;

		private string cognomeUtenteField;

		private string idAmministrazioneField;

		private bool getChildrenField;

		private bool fineValiditaField;

		private AddressbookTipoUtente tipoUtenteField;

		private string[] idRegistriField;

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

		public string codiceGruppo
		{
			get
			{
				return this.codiceGruppoField;
			}
			set
			{
				this.codiceGruppoField = value;
			}
		}

		public string descrizioneGruppo
		{
			get
			{
				return this.descrizioneGruppoField;
			}
			set
			{
				this.descrizioneGruppoField = value;
			}
		}

		public string codiceUO
		{
			get
			{
				return this.codiceUOField;
			}
			set
			{
				this.codiceUOField = value;
			}
		}

		public string descrizioneUO
		{
			get
			{
				return this.descrizioneUOField;
			}
			set
			{
				this.descrizioneUOField = value;
			}
		}

		public string codiceRuolo
		{
			get
			{
				return this.codiceRuoloField;
			}
			set
			{
				this.codiceRuoloField = value;
			}
		}

		public string descrizioneRuolo
		{
			get
			{
				return this.descrizioneRuoloField;
			}
			set
			{
				this.descrizioneRuoloField = value;
			}
		}

		public string nomeUtente
		{
			get
			{
				return this.nomeUtenteField;
			}
			set
			{
				this.nomeUtenteField = value;
			}
		}

		public string cognomeUtente
		{
			get
			{
				return this.cognomeUtenteField;
			}
			set
			{
				this.cognomeUtenteField = value;
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

		public bool getChildren
		{
			get
			{
				return this.getChildrenField;
			}
			set
			{
				this.getChildrenField = value;
			}
		}

		public bool fineValidita
		{
			get
			{
				return this.fineValiditaField;
			}
			set
			{
				this.fineValiditaField = value;
			}
		}

		public AddressbookTipoUtente tipoUtente
		{
			get
			{
				return this.tipoUtenteField;
			}
			set
			{
				this.tipoUtenteField = value;
			}
		}

		public string[] idRegistri
		{
			get
			{
				return this.idRegistriField;
			}
			set
			{
				this.idRegistriField = value;
			}
		}
	}
}
