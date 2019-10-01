using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class RagioneTrasmissione
	{
		private string systemIdField;

		private string descrizioneField;

		private string tipoField;

		private TrasmissioneDiritto tipoDirittiField;

		private string rispostaField;

		private TramissioneTipoGerarchia tipoDestinatarioField;

		private string noteField;

		private string ereditaField;

		private string tipoRispostaField;

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

		public TrasmissioneDiritto tipoDiritti
		{
			get
			{
				return this.tipoDirittiField;
			}
			set
			{
				this.tipoDirittiField = value;
			}
		}

		public string risposta
		{
			get
			{
				return this.rispostaField;
			}
			set
			{
				this.rispostaField = value;
			}
		}

		public TramissioneTipoGerarchia tipoDestinatario
		{
			get
			{
				return this.tipoDestinatarioField;
			}
			set
			{
				this.tipoDestinatarioField = value;
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

		public string eredita
		{
			get
			{
				return this.ereditaField;
			}
			set
			{
				this.ereditaField = value;
			}
		}

		public string tipoRisposta
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
	}
}
