using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class ProtocolloUscita : Protocollo
	{
		private Corrispondente[] destinatariField;

		private bool daAggiornareDestinatariField;

		private bool daAggiornareDestinatariConoscenzaField;

		private Corrispondente[] destinatariConoscenzaField;

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
