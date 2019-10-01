using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class AddressbookRisultatoInsert : MarshalByRefObject
	{
		private string erroreField;

		private Corrispondente corrispondenteField;

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

		public Corrispondente corrispondente
		{
			get
			{
				return this.corrispondenteField;
			}
			set
			{
				this.corrispondenteField = value;
			}
		}
	}
}
