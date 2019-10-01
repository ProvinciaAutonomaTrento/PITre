using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class AnelloDocumentale
	{
		private InfoDocumento infoDocField;

		private AnelloDocumentale[] childrenField;

		public InfoDocumento infoDoc
		{
			get
			{
				return this.infoDocField;
			}
			set
			{
				this.infoDocField = value;
			}
		}

		public AnelloDocumentale[] children
		{
			get
			{
				return this.childrenField;
			}
			set
			{
				this.childrenField = value;
			}
		}
	}
}
