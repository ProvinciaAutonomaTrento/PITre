using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class ValoreOggetto
	{
		private int sYSTEM_IDField;

		private string dESCRIZIONE_VALOREField;

		private string vALOREField;

		private string vALORE_DI_DEFAULTField;

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

		public string DESCRIZIONE_VALORE
		{
			get
			{
				return this.dESCRIZIONE_VALOREField;
			}
			set
			{
				this.dESCRIZIONE_VALOREField = value;
			}
		}

		public string VALORE
		{
			get
			{
				return this.vALOREField;
			}
			set
			{
				this.vALOREField = value;
			}
		}

		public string VALORE_DI_DEFAULT
		{
			get
			{
				return this.vALORE_DI_DEFAULTField;
			}
			set
			{
				this.vALORE_DI_DEFAULTField = value;
			}
		}
	}
}
