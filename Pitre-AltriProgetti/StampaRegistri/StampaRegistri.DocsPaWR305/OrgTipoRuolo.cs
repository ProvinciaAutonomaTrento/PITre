using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class OrgTipoRuolo
	{
		private string iDTipoRuoloField;

		private string codiceField;

		private string descrizioneField;

		private string livelloField;

		private string iDAmministrazioneField;

		public string IDTipoRuolo
		{
			get
			{
				return this.iDTipoRuoloField;
			}
			set
			{
				this.iDTipoRuoloField = value;
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

		public string Descrizione
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

		public string Livello
		{
			get
			{
				return this.livelloField;
			}
			set
			{
				this.livelloField = value;
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
