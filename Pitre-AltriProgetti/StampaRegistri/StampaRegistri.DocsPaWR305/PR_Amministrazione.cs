using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class PR_Amministrazione
	{
		private string system_idField;

		private string codiceField;

		private string descrizioneField;

		private string libreriaField;

		public string System_id
		{
			get
			{
				return this.system_idField;
			}
			set
			{
				this.system_idField = value;
			}
		}

		public string Codice
		{
			get
			{
				return this.codiceField;
			}
			set
			{
				this.codiceField = value;
			}
		}

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

		public string Libreria
		{
			get
			{
				return this.libreriaField;
			}
			set
			{
				this.libreriaField = value;
			}
		}
	}
}
