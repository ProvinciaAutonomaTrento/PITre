using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlInclude(typeof(ProtocolloInterno)), XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class ProtocolloUscita : Protocollo
	{
		private bool daAggiornareDestinatariField;

		private bool daAggiornareDestinatariConoscenzaField;

		private bool daAggiornareMittenteField;

		private Corrispondente mittenteField;

		private Corrispondente ufficioReferenteField;

		private Corrispondente[] destinatariField;

		private Corrispondente[] destinatariConoscenzaField;

		public bool daAggiornareDestinatari
		{
			get
			{
				return this.daAggiornareDestinatariField;
			}
			set
			{
				this.daAggiornareDestinatariField = value;
			}
		}

		public bool daAggiornareDestinatariConoscenza
		{
			get
			{
				return this.daAggiornareDestinatariConoscenzaField;
			}
			set
			{
				this.daAggiornareDestinatariConoscenzaField = value;
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

		public Corrispondente[] destinatari
		{
			get
			{
				return this.destinatariField;
			}
			set
			{
				this.destinatariField = value;
			}
		}

		public Corrispondente[] destinatariConoscenza
		{
			get
			{
				return this.destinatariConoscenzaField;
			}
			set
			{
				this.destinatariConoscenzaField = value;
			}
		}
	}
}
