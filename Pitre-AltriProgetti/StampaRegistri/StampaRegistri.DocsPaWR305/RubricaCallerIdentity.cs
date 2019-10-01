using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class RubricaCallerIdentity
	{
		private string idUtenteField;

		private string idRuoloField;

		private string idRegistroField;

		public string IdUtente
		{
			get
			{
				return this.idUtenteField;
			}
			set
			{
				this.idUtenteField = value;
			}
		}

		public string IdRuolo
		{
			get
			{
				return this.idRuoloField;
			}
			set
			{
				this.idRuoloField = value;
			}
		}

		public string IdRegistro
		{
			get
			{
				return this.idRegistroField;
			}
			set
			{
				this.idRegistroField = value;
			}
		}
	}
}
