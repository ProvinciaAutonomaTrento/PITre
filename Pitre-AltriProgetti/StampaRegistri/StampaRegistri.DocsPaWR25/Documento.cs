using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Documento : FileRequest
	{
		private string daInviareField;

		private TipologiaCanale tipologiaField;

		private string dataArrivoField;

		public string daInviare
		{
			get
			{
				return this.daInviareField;
			}
			set
			{
				this.daInviareField = value;
			}
		}

		public TipologiaCanale tipologia
		{
			get
			{
				return this.tipologiaField;
			}
			set
			{
				this.tipologiaField = value;
			}
		}

		public string dataArrivo
		{
			get
			{
				return this.dataArrivoField;
			}
			set
			{
				this.dataArrivoField = value;
			}
		}
	}
}
