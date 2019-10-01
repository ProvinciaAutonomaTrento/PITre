using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class InfoFascicolo
	{
		private string idClassificazioneField;

		private string idFascicoloField;

		private string codRegistroField;

		private string idRegistroField;

		private string codiceField;

		private string descrizioneField;

		private string descClassificazioneField;

		private string aperturaField;

		public string idClassificazione
		{
			get
			{
				return this.idClassificazioneField;
			}
			set
			{
				this.idClassificazioneField = value;
			}
		}

		public string idFascicolo
		{
			get
			{
				return this.idFascicoloField;
			}
			set
			{
				this.idFascicoloField = value;
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

		public string descClassificazione
		{
			get
			{
				return this.descClassificazioneField;
			}
			set
			{
				this.descClassificazioneField = value;
			}
		}

		public string apertura
		{
			get
			{
				return this.aperturaField;
			}
			set
			{
				this.aperturaField = value;
			}
		}
	}
}
