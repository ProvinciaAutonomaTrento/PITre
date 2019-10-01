using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class TipoRuolo
	{
		private string systemIdField;

		private string id_AmmField;

		private string codiceField;

		private string descrizioneField;

		private string livelloField;

		private bool abilitatoField;

		private TipoRuolo parentField;

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

		public string id_Amm
		{
			get
			{
				return this.id_AmmField;
			}
			set
			{
				this.id_AmmField = value;
			}
		}

		public string codice
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

		public string livello
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

		public bool abilitato
		{
			get
			{
				return this.abilitatoField;
			}
			set
			{
				this.abilitatoField = value;
			}
		}

		public TipoRuolo Parent
		{
			get
			{
				return this.parentField;
			}
			set
			{
				this.parentField = value;
			}
		}
	}
}
