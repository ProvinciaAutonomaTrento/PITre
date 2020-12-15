using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlInclude(typeof(DocumentoStoricoOggetto)), XmlInclude(typeof(DocumentoStoricoMittente)), XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Storico
	{
		private string systemIdField;

		private string dataModificaField;

		private Utente utenteField;

		private Ruolo ruoloField;

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

		public string dataModifica
		{
			get
			{
				return this.dataModificaField;
			}
			set
			{
				this.dataModificaField = value;
			}
		}

		public Utente utente
		{
			get
			{
				return this.utenteField;
			}
			set
			{
				this.utenteField = value;
			}
		}

		public Ruolo ruolo
		{
			get
			{
				return this.ruoloField;
			}
			set
			{
				this.ruoloField = value;
			}
		}
	}
}
