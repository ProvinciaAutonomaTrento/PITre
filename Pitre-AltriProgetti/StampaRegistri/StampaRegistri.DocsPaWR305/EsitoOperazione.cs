using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class EsitoOperazione
	{
		private int codiceField;

		private string descrizioneField;

		public int Codice
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
	}
}
