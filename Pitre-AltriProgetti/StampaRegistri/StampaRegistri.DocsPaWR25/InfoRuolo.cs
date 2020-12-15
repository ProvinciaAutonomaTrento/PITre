using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class InfoRuolo : MarshalByRefObject
	{
		private string idRuoloField;

		private string idGruppoField;

		private string descRuoloField;

		private string idUoField;

		private string descUoRuoloField;

		private string codRubUoRuoloField;

		private string codUoRuoloField;

		private string codAOOUoRuoloField;

		private string idPeopleField;

		private string idAmministrazioneField;

		public string idRuolo
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

		public string idGruppo
		{
			get
			{
				return this.idGruppoField;
			}
			set
			{
				this.idGruppoField = value;
			}
		}

		public string descRuolo
		{
			get
			{
				return this.descRuoloField;
			}
			set
			{
				this.descRuoloField = value;
			}
		}

		public string idUo
		{
			get
			{
				return this.idUoField;
			}
			set
			{
				this.idUoField = value;
			}
		}

		public string descUoRuolo
		{
			get
			{
				return this.descUoRuoloField;
			}
			set
			{
				this.descUoRuoloField = value;
			}
		}

		public string codRubUoRuolo
		{
			get
			{
				return this.codRubUoRuoloField;
			}
			set
			{
				this.codRubUoRuoloField = value;
			}
		}

		public string codUoRuolo
		{
			get
			{
				return this.codUoRuoloField;
			}
			set
			{
				this.codUoRuoloField = value;
			}
		}

		public string codAOOUoRuolo
		{
			get
			{
				return this.codAOOUoRuoloField;
			}
			set
			{
				this.codAOOUoRuoloField = value;
			}
		}

		public string idPeople
		{
			get
			{
				return this.idPeopleField;
			}
			set
			{
				this.idPeopleField = value;
			}
		}

		public string idAmministrazione
		{
			get
			{
				return this.idAmministrazioneField;
			}
			set
			{
				this.idAmministrazioneField = value;
			}
		}
	}
}
