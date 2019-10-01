using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Registro
	{
		private string systemIdField;

		private string codRegistroField;

		private string codiceField;

		private string descrizioneField;

		private string emailField;

		private string statoField;

		private string dataAperturaField;

		private string dataChiusuraField;

		private string idAmministrazioneField;

		private string codAmministrazioneField;

		private string dataUltimoProtocolloField;

		private string ultimoNumeroProtocolloField;

		private string ruoloRiferimentoField;

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

		public string codRegistro
		{
			get
			{
				return this.codRegistroField;
			}
			set
			{
				this.codRegistroField = value;
			}
		}

		public string codice
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

		public string stato
		{
			get
			{
				return this.statoField;
			}
			set
			{
				this.statoField = value;
			}
		}

		public string dataApertura
		{
			get
			{
				return this.dataAperturaField;
			}
			set
			{
				this.dataAperturaField = value;
			}
		}

		public string dataChiusura
		{
			get
			{
				return this.dataChiusuraField;
			}
			set
			{
				this.dataChiusuraField = value;
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

		public string codAmministrazione
		{
			get
			{
				return this.codAmministrazioneField;
			}
			set
			{
				this.codAmministrazioneField = value;
			}
		}

		public string dataUltimoProtocollo
		{
			get
			{
				return this.dataUltimoProtocolloField;
			}
			set
			{
				this.dataUltimoProtocolloField = value;
			}
		}

		public string ultimoNumeroProtocollo
		{
			get
			{
				return this.ultimoNumeroProtocolloField;
			}
			set
			{
				this.ultimoNumeroProtocolloField = value;
			}
		}

		public string ruoloRiferimento
		{
			get
			{
				return this.ruoloRiferimentoField;
			}
			set
			{
				this.ruoloRiferimentoField = value;
			}
		}
	}
}
