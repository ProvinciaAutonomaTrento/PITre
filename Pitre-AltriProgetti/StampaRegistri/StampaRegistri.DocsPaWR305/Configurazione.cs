using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Configurazione
	{
		private string parametroField;

		private string valoreField;

		private string idAmministrazioneField;

		public string parametro
		{
			get
			{
				return this.parametroField;
			}
			set
			{
				this.parametroField = value;
			}
		}

		public string valore
		{
			get
			{
				return this.valoreField;
			}
			set
			{
				this.valoreField = value;
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
	}
}
