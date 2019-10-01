using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class TipologiaAtto
	{
		private string systemIdField;

		private string descrizioneField;

		private CategoriaAtto categoriaAttoField;

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

		public CategoriaAtto categoriaAtto
		{
			get
			{
				return this.categoriaAttoField;
			}
			set
			{
				this.categoriaAttoField = value;
			}
		}
	}
}
