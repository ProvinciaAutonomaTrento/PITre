using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Atto
	{
		private string progressivoCategoriaField;

		private CategoriaAtto categoriaAttoField;

		public string progressivoCategoria
		{
			get
			{
				return this.progressivoCategoriaField;
			}
			set
			{
				this.progressivoCategoriaField = value;
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
