using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Fascicolo : MarshalByRefObject
	{
		private string idClassificazioneField;

		private string descrizioneField;

		private string codiceField;

		private string systemIDField;

		private string aperturaField;

		private string chiusuraField;

		private string statoField;

		private string tipoField;

		private string noteField;

		private string codUltimoField;

		private string dirittoUtenteField;

		private string codLegislaturaField;

		public string idClassificazione
		{
			get
			{
				return this.idClassificazioneField;
			}
			set
			{
				this.idClassificazioneField = value;
			}
		}

		public string descrizione
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

		public string codice
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

		public string systemID
		{
			get
			{
				return this.systemIDField;
			}
			set
			{
				this.systemIDField = value;
			}
		}

		public string apertura
		{
			get
			{
				return this.aperturaField;
			}
			set
			{
				this.aperturaField = value;
			}
		}

		public string chiusura
		{
			get
			{
				return this.chiusuraField;
			}
			set
			{
				this.chiusuraField = value;
			}
		}

		public string stato
		{
			get
			{
				return this.statoField;
			}
			set
			{
				this.statoField = value;
			}
		}

		public string tipo
		{
			get
			{
				return this.tipoField;
			}
			set
			{
				this.tipoField = value;
			}
		}

		public string note
		{
			get
			{
				return this.noteField;
			}
			set
			{
				this.noteField = value;
			}
		}

		public string codUltimo
		{
			get
			{
				return this.codUltimoField;
			}
			set
			{
				this.codUltimoField = value;
			}
		}

		public string dirittoUtente
		{
			get
			{
				return this.dirittoUtenteField;
			}
			set
			{
				this.dirittoUtenteField = value;
			}
		}

		public string codLegislatura
		{
			get
			{
				return this.codLegislaturaField;
			}
			set
			{
				this.codLegislaturaField = value;
			}
		}
	}
}
