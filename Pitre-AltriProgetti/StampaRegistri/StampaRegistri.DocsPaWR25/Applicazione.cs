using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Applicazione
	{
		private string systemIdField;

		private string estensioneField;

		private string descrizioneField;

		private string applicationField;

		private string mimeTypeField;

		public string systemId
		{
			get
			{
				return this.systemIdField;
			}
			set
			{
				this.systemIdField = value;
			}
		}

		public string estensione
		{
			get
			{
				return this.estensioneField;
			}
			set
			{
				this.estensioneField = value;
			}
		}

		public string descrizione
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

		public string application
		{
			get
			{
				return this.applicationField;
			}
			set
			{
				this.applicationField = value;
			}
		}

		public string mimeType
		{
			get
			{
				return this.mimeTypeField;
			}
			set
			{
				this.mimeTypeField = value;
			}
		}
	}
}
