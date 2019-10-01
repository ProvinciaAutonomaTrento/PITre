using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class OrgRegistro
	{
		private string iDRegistroField;

		private string codiceField;

		private string descrizioneField;

		private string iDAmministrazioneField;

		private string associatoField;

		public string IDRegistro
		{
			get
			{
				return this.iDRegistroField;
			}
			set
			{
				this.iDRegistroField = value;
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

		public string Associato
		{
			get
			{
				return this.associatoField;
			}
			set
			{
				this.associatoField = value;
			}
		}
	}
}
