using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Funzione : MarshalByRefObject
	{
		private string systemIdField;

		private string codiceField;

		private string descrizioneField;

		private string idTipoFunzioneField;

		private string codTipoFunzioneField;

		private string descTipoFunzioneField;

		private string idParentField;

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

		public string idTipoFunzione
		{
			get
			{
				return this.idTipoFunzioneField;
			}
			set
			{
				this.idTipoFunzioneField = value;
			}
		}

		public string codTipoFunzione
		{
			get
			{
				return this.codTipoFunzioneField;
			}
			set
			{
				this.codTipoFunzioneField = value;
			}
		}

		public string descTipoFunzione
		{
			get
			{
				return this.descTipoFunzioneField;
			}
			set
			{
				this.descTipoFunzioneField = value;
			}
		}

		public string idParent
		{
			get
			{
				return this.idParentField;
			}
			set
			{
				this.idParentField = value;
			}
		}
	}
}
