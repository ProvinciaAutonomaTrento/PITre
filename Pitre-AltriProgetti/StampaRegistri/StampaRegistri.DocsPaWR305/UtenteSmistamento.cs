using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class UtenteSmistamento
	{
		private string idField;

		private string iDCorrGlobaliField;

		private string userIDField;

		private string denominazioneField;

		private TipoNotificaSmistamento tipoNotificaSmistamentoField;

		private string eMailField;

		private bool flagCompetenzaField;

		private bool flagConoscenzaField;

		public string ID
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
			}
		}

		public string IDCorrGlobali
		{
			get
			{
				return this.iDCorrGlobaliField;
			}
			set
			{
				this.iDCorrGlobaliField = value;
			}
		}

		public string UserID
		{
			get
			{
				return this.userIDField;
			}
			set
			{
				this.userIDField = value;
			}
		}

		public string Denominazione
		{
			get
			{
				return this.denominazioneField;
			}
			set
			{
				this.denominazioneField = value;
			}
		}

		public TipoNotificaSmistamento TipoNotificaSmistamento
		{
			get
			{
				return this.tipoNotificaSmistamentoField;
			}
			set
			{
				this.tipoNotificaSmistamentoField = value;
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

		public bool FlagCompetenza
		{
			get
			{
				return this.flagCompetenzaField;
			}
			set
			{
				this.flagCompetenzaField = value;
			}
		}

		public bool FlagConoscenza
		{
			get
			{
				return this.flagConoscenzaField;
			}
			set
			{
				this.flagConoscenzaField = value;
			}
		}
	}
}
