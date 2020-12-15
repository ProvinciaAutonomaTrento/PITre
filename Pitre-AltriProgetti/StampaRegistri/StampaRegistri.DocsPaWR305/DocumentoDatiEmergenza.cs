using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class DocumentoDatiEmergenza
	{
		private string protocolloEmergenzaField;

		private string dataProtocollazioneEmergenzaField;

		private string nomeProtocollatoreEmergenzaField;

		private string cognomeProtocollatoreEmergenzaField;

		public string protocolloEmergenza
		{
			get
			{
				return this.protocolloEmergenzaField;
			}
			set
			{
				this.protocolloEmergenzaField = value;
			}
		}

		public string dataProtocollazioneEmergenza
		{
			get
			{
				return this.dataProtocollazioneEmergenzaField;
			}
			set
			{
				this.dataProtocollazioneEmergenzaField = value;
			}
		}

		public string nomeProtocollatoreEmergenza
		{
			get
			{
				return this.nomeProtocollatoreEmergenzaField;
			}
			set
			{
				this.nomeProtocollatoreEmergenzaField = value;
			}
		}

		public string cognomeProtocollatoreEmergenza
		{
			get
			{
				return this.cognomeProtocollatoreEmergenzaField;
			}
			set
			{
				this.cognomeProtocollatoreEmergenzaField = value;
			}
		}
	}
}
