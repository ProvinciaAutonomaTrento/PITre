using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class TrasmissioneUtente
	{
		private Utente utenteField;

		private string dataVistaField;

		private string dataAccettataField;

		private string dataRifiutataField;

		private string noteRifiutoField;

		private string noteAccettazioneField;

		private string systemIdField;

		private TrasmissioneTipoRisposta tipoRispostaField;

		private string dataRispostaField;

		private string idTrasmRispSingField;

		private string validaField;

		private bool daAggiornareField;

		public Utente utente
		{
			get
			{
				return this.utenteField;
			}
			set
			{
				this.utenteField = value;
			}
		}

		public string dataVista
		{
			get
			{
				return this.dataVistaField;
			}
			set
			{
				this.dataVistaField = value;
			}
		}

		public string dataAccettata
		{
			get
			{
				return this.dataAccettataField;
			}
			set
			{
				this.dataAccettataField = value;
			}
		}

		public string dataRifiutata
		{
			get
			{
				return this.dataRifiutataField;
			}
			set
			{
				this.dataRifiutataField = value;
			}
		}

		public string noteRifiuto
		{
			get
			{
				return this.noteRifiutoField;
			}
			set
			{
				this.noteRifiutoField = value;
			}
		}

		public string noteAccettazione
		{
			get
			{
				return this.noteAccettazioneField;
			}
			set
			{
				this.noteAccettazioneField = value;
			}
		}

		public string systemId
		{
			get
			{
				return this.systemIdField;
			}
			set
			{
				this.systemIdField = value;
			}
		}

		public TrasmissioneTipoRisposta tipoRisposta
		{
			get
			{
				return this.tipoRispostaField;
			}
			set
			{
				this.tipoRispostaField = value;
			}
		}

		public string dataRisposta
		{
			get
			{
				return this.dataRispostaField;
			}
			set
			{
				this.dataRispostaField = value;
			}
		}

		public string idTrasmRispSing
		{
			get
			{
				return this.idTrasmRispSingField;
			}
			set
			{
				this.idTrasmRispSingField = value;
			}
		}

		public string valida
		{
			get
			{
				return this.validaField;
			}
			set
			{
				this.validaField = value;
			}
		}

		public bool daAggiornare
		{
			get
			{
				return this.daAggiornareField;
			}
			set
			{
				this.daAggiornareField = value;
			}
		}
	}
}
