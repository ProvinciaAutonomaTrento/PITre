using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class InfoUtente : MarshalByRefObject
	{
		private string idCorrGlobaliField;

		private string idPeopleField;

		private string userIdField;

		private string dstField;

		private string idGruppoField;

		private string idAmministrazioneField;

		public string idCorrGlobali
		{
			get
			{
				return this.idCorrGlobaliField;
			}
			set
			{
				this.idCorrGlobaliField = value;
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

		public string userId
		{
			get
			{
				return this.userIdField;
			}
			set
			{
				this.userIdField = value;
			}
		}

		public string dst
		{
			get
			{
				return this.dstField;
			}
			set
			{
				this.dstField = value;
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
