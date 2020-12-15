using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class FirmaResult
	{
		private FileRequest fileRequestField;

		private string erroreField;

		public FileRequest fileRequest
		{
			get
			{
				return this.fileRequestField;
			}
			set
			{
				this.fileRequestField = value;
			}
		}

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
	}
}
