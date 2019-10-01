using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class TipoOggetto
	{
		private int sYSTEM_IDField;

		private string dESCRIZIONE_TIPOField;

		public int SYSTEM_ID
		{
			get
			{
				return this.sYSTEM_IDField;
			}
			set
			{
				this.sYSTEM_IDField = value;
			}
		}

		public string DESCRIZIONE_TIPO
		{
			get
			{
				return this.dESCRIZIONE_TIPOField;
			}
			set
			{
				this.dESCRIZIONE_TIPOField = value;
			}
		}
	}
}
