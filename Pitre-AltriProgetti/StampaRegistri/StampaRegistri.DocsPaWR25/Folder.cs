using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Folder : MarshalByRefObject
	{
		private Folder[] childsField;

		private string systemIDField;

		private string idFascicoloField;

		private string idParentField;

		private string descrizioneField;

		public Folder[] childs
		{
			get
			{
				return this.childsField;
			}
			set
			{
				this.childsField = value;
			}
		}

		public string systemID
		{
			get
			{
				return this.systemIDField;
			}
			set
			{
				this.systemIDField = value;
			}
		}

		public string idFascicolo
		{
			get
			{
				return this.idFascicoloField;
			}
			set
			{
				this.idFascicoloField = value;
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
	}
}
