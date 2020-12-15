using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class ProtocolloEntrata : Protocollo
	{
		private Corrispondente mittenteField;

		private Corrispondente mittenteIntermedioField;

		private Corrispondente ufficioReferenteField;

		private string descrizioneProtocolloMittenteField;

		private string dataProtocolloMittenteField;

		private bool daAggiornareMittenteField;

		private bool daAggiornareMittenteIntermedioField;

		public Corrispondente mittente
		{
			get
			{
				return this.mittenteField;
			}
			set
			{
				this.mittenteField = value;
			}
		}

		public Corrispondente mittenteIntermedio
		{
			get
			{
				return this.mittenteIntermedioField;
			}
			set
			{
				this.mittenteIntermedioField = value;
			}
		}

		public Corrispondente ufficioReferente
		{
			get
			{
				return this.ufficioReferenteField;
			}
			set
			{
				this.ufficioReferenteField = value;
			}
		}

		public string descrizioneProtocolloMittente
		{
			get
			{
				return this.descrizioneProtocolloMittenteField;
			}
			set
			{
				this.descrizioneProtocolloMittenteField = value;
			}
		}

		public string dataProtocolloMittente
		{
			get
			{
				return this.dataProtocolloMittenteField;
			}
			set
			{
				this.dataProtocolloMittenteField = value;
			}
		}

		public bool daAggiornareMittente
		{
			get
			{
				return this.daAggiornareMittenteField;
			}
			set
			{
				this.daAggiornareMittenteField = value;
			}
		}

		public bool daAggiornareMittenteIntermedio
		{
			get
			{
				return this.daAggiornareMittenteIntermedioField;
			}
			set
			{
				this.daAggiornareMittenteIntermedioField = value;
			}
		}
	}
}
