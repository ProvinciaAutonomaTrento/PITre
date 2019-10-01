using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class ElementoRubrica : MarshalByRefObject
	{
		private string codiceField;

		private string descrizioneField;

		private string tipoField;

		private bool internoField;

		private bool has_childrenField;

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

		public string tipo
		{
			get
			{
				return this.tipoField;
			}
			set
			{
				this.tipoField = value;
			}
		}

		public bool interno
		{
			get
			{
				return this.internoField;
			}
			set
			{
				this.internoField = value;
			}
		}

		public bool has_children
		{
			get
			{
				return this.has_childrenField;
			}
			set
			{
				this.has_childrenField = value;
			}
		}
	}
}
