using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Firmatario
	{
		private string systemIdField;

		private string nomeField;

		private string cognomeField;

		private string codiceFiscaleField;

		private string dataNascitaField;

		private string identificativoCAField;

		private int livelloField;

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

		public string nome
		{
			get
			{
				return this.nomeField;
			}
			set
			{
				this.nomeField = value;
			}
		}

		public string cognome
		{
			get
			{
				return this.cognomeField;
			}
			set
			{
				this.cognomeField = value;
			}
		}

		public string codiceFiscale
		{
			get
			{
				return this.codiceFiscaleField;
			}
			set
			{
				this.codiceFiscaleField = value;
			}
		}

		public string dataNascita
		{
			get
			{
				return this.dataNascitaField;
			}
			set
			{
				this.dataNascitaField = value;
			}
		}

		public string identificativoCA
		{
			get
			{
				return this.identificativoCAField;
			}
			set
			{
				this.identificativoCAField = value;
			}
		}

		public int livello
		{
			get
			{
				return this.livelloField;
			}
			set
			{
				this.livelloField = value;
			}
		}
	}
}
