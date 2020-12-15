using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class SubjectCommonName
	{
		private string cognomeField;

		private string nomeField;

		private string codiceFiscaleField;

		private string certIdField;

		public string Cognome
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

		public string Nome
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

		public string CodiceFiscale
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

		public string CertId
		{
			get
			{
				return this.certIdField;
			}
			set
			{
				this.certIdField = value;
			}
		}
	}
}
