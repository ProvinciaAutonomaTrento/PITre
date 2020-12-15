using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class resultProtoEmergenza
	{
		private bool isProtocollatoField;

		private bool isClassificatoField;

		private bool isTrasmessoField;

		private bool isAnnullatoField;

		private string messaggioField;

		public bool isProtocollato
		{
			get
			{
				return this.isProtocollatoField;
			}
			set
			{
				this.isProtocollatoField = value;
			}
		}

		public bool isClassificato
		{
			get
			{
				return this.isClassificatoField;
			}
			set
			{
				this.isClassificatoField = value;
			}
		}

		public bool isTrasmesso
		{
			get
			{
				return this.isTrasmessoField;
			}
			set
			{
				this.isTrasmessoField = value;
			}
		}

		public bool isAnnullato
		{
			get
			{
				return this.isAnnullatoField;
			}
			set
			{
				this.isAnnullatoField = value;
			}
		}

		public string messaggio
		{
			get
			{
				return this.messaggioField;
			}
			set
			{
				this.messaggioField = value;
			}
		}
	}
}
