using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class MittenteSmistamento
	{
		private string iDPeopleField;

		private string iDAmministrazioneField;

		private string[] registriAppartenenzaField;

		private string eMailField;

		private string iDCorrGlobaleRuoloField;

		private string iDGroupField;

		private string livelloRuoloField;

		public string IDPeople
		{
			get
			{
				return this.iDPeopleField;
			}
			set
			{
				this.iDPeopleField = value;
			}
		}

		public string IDAmministrazione
		{
			get
			{
				return this.iDAmministrazioneField;
			}
			set
			{
				this.iDAmministrazioneField = value;
			}
		}

		public string[] RegistriAppartenenza
		{
			get
			{
				return this.registriAppartenenzaField;
			}
			set
			{
				this.registriAppartenenzaField = value;
			}
		}

		public string EMail
		{
			get
			{
				return this.eMailField;
			}
			set
			{
				this.eMailField = value;
			}
		}

		public string IDCorrGlobaleRuolo
		{
			get
			{
				return this.iDCorrGlobaleRuoloField;
			}
			set
			{
				this.iDCorrGlobaleRuoloField = value;
			}
		}

		public string IDGroup
		{
			get
			{
				return this.iDGroupField;
			}
			set
			{
				this.iDGroupField = value;
			}
		}

		public string LivelloRuolo
		{
			get
			{
				return this.livelloRuoloField;
			}
			set
			{
				this.livelloRuoloField = value;
			}
		}
	}
}
