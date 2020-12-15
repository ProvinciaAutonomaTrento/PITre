using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Report
	{
		private string descrizioneField;

		private string valoreField;

		private object[] subReportsField;

		public string Descrizione
		{
			get
			{
				return this.descrizioneField;
			}
			set
			{
				this.descrizioneField = value;
			}
		}

		public string Valore
		{
			get
			{
				return this.valoreField;
			}
			set
			{
				this.valoreField = value;
			}
		}

		public object[] SubReports
		{
			get
			{
				return this.subReportsField;
			}
			set
			{
				this.subReportsField = value;
			}
		}
	}
}
