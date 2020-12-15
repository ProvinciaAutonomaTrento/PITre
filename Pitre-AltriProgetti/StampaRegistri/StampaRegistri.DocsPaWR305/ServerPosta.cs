using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class ServerPosta
	{
		private string systemIdField;

		private string descrizioneField;

		private string serverPOPField;

		private string portaPOPField;

		private string serverSMTPField;

		private string portaSMTPField;

		private string dominioField;

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

		public string serverPOP
		{
			get
			{
				return this.serverPOPField;
			}
			set
			{
				this.serverPOPField = value;
			}
		}

		public string portaPOP
		{
			get
			{
				return this.portaPOPField;
			}
			set
			{
				this.portaPOPField = value;
			}
		}

		public string serverSMTP
		{
			get
			{
				return this.serverSMTPField;
			}
			set
			{
				this.serverSMTPField = value;
			}
		}

		public string portaSMTP
		{
			get
			{
				return this.portaSMTPField;
			}
			set
			{
				this.portaSMTPField = value;
			}
		}

		public string dominio
		{
			get
			{
				return this.dominioField;
			}
			set
			{
				this.dominioField = value;
			}
		}
	}
}
