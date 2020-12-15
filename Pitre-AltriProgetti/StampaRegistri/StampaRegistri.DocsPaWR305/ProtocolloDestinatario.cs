using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class ProtocolloDestinatario
	{
		private string systemIdField;

		private string codiceAOOField;

		private string protocolloDestinatarioField;

		private string dataProtocolloDestinatarioField;

		private string codiceAmmField;

		private string descrizioneCorrField;

		private string documentTypeField;

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

		public string codiceAOO
		{
			get
			{
				return this.codiceAOOField;
			}
			set
			{
				this.codiceAOOField = value;
			}
		}

		public string protocolloDestinatario
		{
			get
			{
				return this.protocolloDestinatarioField;
			}
			set
			{
				this.protocolloDestinatarioField = value;
			}
		}

		public string dataProtocolloDestinatario
		{
			get
			{
				return this.dataProtocolloDestinatarioField;
			}
			set
			{
				this.dataProtocolloDestinatarioField = value;
			}
		}

		public string codiceAmm
		{
			get
			{
				return this.codiceAmmField;
			}
			set
			{
				this.codiceAmmField = value;
			}
		}

		public string descrizioneCorr
		{
			get
			{
				return this.descrizioneCorrField;
			}
			set
			{
				this.descrizioneCorrField = value;
			}
		}

		public string documentType
		{
			get
			{
				return this.documentTypeField;
			}
			set
			{
				this.documentTypeField = value;
			}
		}
	}
}
