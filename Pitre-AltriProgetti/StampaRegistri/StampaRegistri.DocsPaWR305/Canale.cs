using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Canale
	{
		private string systemIdField;

		private string descrizioneField;

		private string typeIdField;

		private string tipoCanaleField;

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

		public string typeId
		{
			get
			{
				return this.typeIdField;
			}
			set
			{
				this.typeIdField = value;
			}
		}

		public string tipoCanale
		{
			get
			{
				return this.tipoCanaleField;
			}
			set
			{
				this.tipoCanaleField = value;
			}
		}
	}
}
