using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class ProtocolloEmergenza
	{
		private string numeroField;

		private string dataProtocollazioneField;

		private string idRegistroField;

		private string tipoProtocolloField;

		private string oggettoField;

		private string codiceClassificaField;

		private string templateTrasmissioneField;

		private string idAutoreField;

		private string dataArrivoField;

		private string numeroProtocolloMittenteField;

		private string dataProtocolloMittenteField;

		private string[] mittentiField;

		private string nomeFirmatarioField;

		private string cognomeFirmatarioField;

		private string[] destinatariField;

		private string[] destinatariCCField;

		private string idUtenteAnnullamentoField;

		private string dataAnnullamentoField;

		private string noteAnnullamentoField;

		public string numero
		{
			get
			{
				return this.numeroField;
			}
			set
			{
				this.numeroField = value;
			}
		}

		public string dataProtocollazione
		{
			get
			{
				return this.dataProtocollazioneField;
			}
			set
			{
				this.dataProtocollazioneField = value;
			}
		}

		public string idRegistro
		{
			get
			{
				return this.idRegistroField;
			}
			set
			{
				this.idRegistroField = value;
			}
		}

		public string tipoProtocollo
		{
			get
			{
				return this.tipoProtocolloField;
			}
			set
			{
				this.tipoProtocolloField = value;
			}
		}

		public string oggetto
		{
			get
			{
				return this.oggettoField;
			}
			set
			{
				this.oggettoField = value;
			}
		}

		public string codiceClassifica
		{
			get
			{
				return this.codiceClassificaField;
			}
			set
			{
				this.codiceClassificaField = value;
			}
		}

		public string templateTrasmissione
		{
			get
			{
				return this.templateTrasmissioneField;
			}
			set
			{
				this.templateTrasmissioneField = value;
			}
		}

		public string idAutore
		{
			get
			{
				return this.idAutoreField;
			}
			set
			{
				this.idAutoreField = value;
			}
		}

		public string dataArrivo
		{
			get
			{
				return this.dataArrivoField;
			}
			set
			{
				this.dataArrivoField = value;
			}
		}

		public string numeroProtocolloMittente
		{
			get
			{
				return this.numeroProtocolloMittenteField;
			}
			set
			{
				this.numeroProtocolloMittenteField = value;
			}
		}

		public string dataProtocolloMittente
		{
			get
			{
				return this.dataProtocolloMittenteField;
			}
			set
			{
				this.dataProtocolloMittenteField = value;
			}
		}

		public string[] mittenti
		{
			get
			{
				return this.mittentiField;
			}
			set
			{
				this.mittentiField = value;
			}
		}

		public string nomeFirmatario
		{
			get
			{
				return this.nomeFirmatarioField;
			}
			set
			{
				this.nomeFirmatarioField = value;
			}
		}

		public string cognomeFirmatario
		{
			get
			{
				return this.cognomeFirmatarioField;
			}
			set
			{
				this.cognomeFirmatarioField = value;
			}
		}

		public string[] destinatari
		{
			get
			{
				return this.destinatariField;
			}
			set
			{
				this.destinatariField = value;
			}
		}

		public string[] destinatariCC
		{
			get
			{
				return this.destinatariCCField;
			}
			set
			{
				this.destinatariCCField = value;
			}
		}

		public string idUtenteAnnullamento
		{
			get
			{
				return this.idUtenteAnnullamentoField;
			}
			set
			{
				this.idUtenteAnnullamentoField = value;
			}
		}

		public string dataAnnullamento
		{
			get
			{
				return this.dataAnnullamentoField;
			}
			set
			{
				this.dataAnnullamentoField = value;
			}
		}

		public string noteAnnullamento
		{
			get
			{
				return this.noteAnnullamentoField;
			}
			set
			{
				this.noteAnnullamentoField = value;
			}
		}
	}
}
