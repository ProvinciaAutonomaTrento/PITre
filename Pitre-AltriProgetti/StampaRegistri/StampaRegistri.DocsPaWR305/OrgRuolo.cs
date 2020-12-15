using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class OrgRuolo
	{
		private string iDCorrGlobaleField;

		private string iDGruppoField;

		private string iDTipoRuoloField;

		private string iDUoField;

		private string codiceField;

		private string codiceRubricaField;

		private string descrizioneField;

		private string diRiferimentoField;

		private string iDAmministrazioneField;

		private OrgUtente[] utentiField;

		public string IDCorrGlobale
		{
			get
			{
				return this.iDCorrGlobaleField;
			}
			set
			{
				this.iDCorrGlobaleField = value;
			}
		}

		public string IDGruppo
		{
			get
			{
				return this.iDGruppoField;
			}
			set
			{
				this.iDGruppoField = value;
			}
		}

		public string IDTipoRuolo
		{
			get
			{
				return this.iDTipoRuoloField;
			}
			set
			{
				this.iDTipoRuoloField = value;
			}
		}

		public string IDUo
		{
			get
			{
				return this.iDUoField;
			}
			set
			{
				this.iDUoField = value;
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

		public string CodiceRubrica
		{
			get
			{
				return this.codiceRubricaField;
			}
			set
			{
				this.codiceRubricaField = value;
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

		public string DiRiferimento
		{
			get
			{
				return this.diRiferimentoField;
			}
			set
			{
				this.diRiferimentoField = value;
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

		public OrgUtente[] Utenti
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
	}
}
