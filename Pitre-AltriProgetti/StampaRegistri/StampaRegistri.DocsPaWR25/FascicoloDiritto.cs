using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class FascicoloDiritto
	{
		private string idObjField;

		private FascicoloTipoDiritto tipoDirittoField;

		private Corrispondente soggettoField;

		public string idObj
		{
			get
			{
				return this.idObjField;
			}
			set
			{
				this.idObjField = value;
			}
		}

		public FascicoloTipoDiritto tipoDiritto
		{
			get
			{
				return this.tipoDirittoField;
			}
			set
			{
				this.tipoDirittoField = value;
			}
		}

		public Corrispondente soggetto
		{
			get
			{
				return this.soggettoField;
			}
			set
			{
				this.soggettoField = value;
			}
		}
	}
}
