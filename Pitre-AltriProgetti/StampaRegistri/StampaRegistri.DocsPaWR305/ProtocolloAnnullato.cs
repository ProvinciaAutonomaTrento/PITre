using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class ProtocolloAnnullato
	{
		private string autorizzazioneField;

		private string dataAnnullamentoField;

		public string autorizzazione
		{
			get
			{
				return this.autorizzazioneField;
			}
			set
			{
				this.autorizzazioneField = value;
			}
		}

		public string dataAnnullamento
		{
			get
			{
				return this.dataAnnullamentoField;
			}
			set
			{
				this.dataAnnullamentoField = value;
			}
		}
	}
}
