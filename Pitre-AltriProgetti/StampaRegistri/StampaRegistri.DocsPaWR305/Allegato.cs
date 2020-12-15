using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Allegato : FileRequest
	{
		private int numeroPagineField;

		public int numeroPagine
		{
			get
			{
				return this.numeroPagineField;
			}
			set
			{
				this.numeroPagineField = value;
			}
		}
	}
}
