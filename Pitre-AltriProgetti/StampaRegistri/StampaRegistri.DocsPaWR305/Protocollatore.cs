using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Protocollatore
	{
		private string utente_idPeopleField;

		private string ruolo_idCorrGlobaliField;

		private string uo_idCorrGlobaliField;

		private string uo_codiceCorrGlobaliField;

		public string utente_idPeople
		{
			get
			{
				return this.utente_idPeopleField;
			}
			set
			{
				this.utente_idPeopleField = value;
			}
		}

		public string ruolo_idCorrGlobali
		{
			get
			{
				return this.ruolo_idCorrGlobaliField;
			}
			set
			{
				this.ruolo_idCorrGlobaliField = value;
			}
		}

		public string uo_idCorrGlobali
		{
			get
			{
				return this.uo_idCorrGlobaliField;
			}
			set
			{
				this.uo_idCorrGlobaliField = value;
			}
		}

		public string uo_codiceCorrGlobali
		{
			get
			{
				return this.uo_codiceCorrGlobaliField;
			}
			set
			{
				this.uo_codiceCorrGlobaliField = value;
			}
		}
	}
}
