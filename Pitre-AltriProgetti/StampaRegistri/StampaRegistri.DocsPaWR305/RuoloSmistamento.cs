using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class RuoloSmistamento
	{
		private string idField;

		private string codiceField;

		private string descrizioneField;

		private bool ruoloRiferimentoField;

		private UtenteSmistamento[] utentiField;

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

		public string Codice
		{
			get
			{
				return this.codiceField;
			}
			set
			{
				this.codiceField = value;
			}
		}

		public string Descrizione
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

		public bool RuoloRiferimento
		{
			get
			{
				return this.ruoloRiferimentoField;
			}
			set
			{
				this.ruoloRiferimentoField = value;
			}
		}

		public UtenteSmistamento[] Utenti
		{
			get
			{
				return this.utentiField;
			}
			set
			{
				this.utentiField = value;
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
