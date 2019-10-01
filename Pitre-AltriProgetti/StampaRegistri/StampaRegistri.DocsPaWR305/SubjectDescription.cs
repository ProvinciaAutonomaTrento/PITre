using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class SubjectDescription
	{
		private string cognomeField;

		private string nomeField;

		private DateTime dataDiNascitaField;

		private string ruoloField;

		private string countryField;

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

		[XmlElement(DataType = "date")]
		public DateTime DataDiNascita
		{
			get
			{
				return this.dataDiNascitaField;
			}
			set
			{
				this.dataDiNascitaField = value;
			}
		}

		public string Ruolo
		{
			get
			{
				return this.ruoloField;
			}
			set
			{
				this.ruoloField = value;
			}
		}

		public string Country
		{
			get
			{
				return this.countryField;
			}
			set
			{
				this.countryField = value;
			}
		}
	}
}
