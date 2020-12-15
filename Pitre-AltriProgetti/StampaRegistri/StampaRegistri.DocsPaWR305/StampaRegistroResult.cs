using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class StampaRegistroResult
	{
		private string erroreField;

		private string docNumberField;

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

		public string docNumber
		{
			get
			{
				return this.docNumberField;
			}
			set
			{
				this.docNumberField = value;
			}
		}
	}
}
