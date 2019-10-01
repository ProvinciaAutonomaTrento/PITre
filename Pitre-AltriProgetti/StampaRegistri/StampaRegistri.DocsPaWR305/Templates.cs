using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Templates
	{
		private int sYSTEM_IDField;

		private string dESCRIZIONEField;

		private object[] eLENCO_OGGETTIField;

		private string dOC_NUMBERField;

		private string aBILITATO_SI_NOField;

		private string iN_ESERCIZIOField;

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

		public string DESCRIZIONE
		{
			get
			{
				return this.dESCRIZIONEField;
			}
			set
			{
				this.dESCRIZIONEField = value;
			}
		}

		public object[] ELENCO_OGGETTI
		{
			get
			{
				return this.eLENCO_OGGETTIField;
			}
			set
			{
				this.eLENCO_OGGETTIField = value;
			}
		}

		public string DOC_NUMBER
		{
			get
			{
				return this.dOC_NUMBERField;
			}
			set
			{
				this.dOC_NUMBERField = value;
			}
		}

		public string ABILITATO_SI_NO
		{
			get
			{
				return this.aBILITATO_SI_NOField;
			}
			set
			{
				this.aBILITATO_SI_NOField = value;
			}
		}

		public string IN_ESERCIZIO
		{
			get
			{
				return this.iN_ESERCIZIOField;
			}
			set
			{
				this.iN_ESERCIZIOField = value;
			}
		}
	}
}
