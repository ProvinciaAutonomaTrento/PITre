using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class TrasmissioneSingola
	{
		private RagioneTrasmissione ragioneField;

		private Corrispondente corrispondenteInternoField;

		private TrasmissioneUtente[] trasmissioneUtenteField;

		private string noteSingoleField;

		private string tipoTrasmField;

		private TrasmissioneTipoDestinatario tipoDestField;

		private string dataScadenzaField;

		private string systemIdField;

		private string idTrasmUtenteField;

		private bool daAggiornareField;

		private bool daEliminareField;

		public RagioneTrasmissione ragione
		{
			get
			{
				return this.ragioneField;
			}
			set
			{
				this.ragioneField = value;
			}
		}

		public Corrispondente corrispondenteInterno
		{
			get
			{
				return this.corrispondenteInternoField;
			}
			set
			{
				this.corrispondenteInternoField = value;
			}
		}

		public TrasmissioneUtente[] trasmissioneUtente
		{
			get
			{
				return this.trasmissioneUtenteField;
			}
			set
			{
				this.trasmissioneUtenteField = value;
			}
		}

		public string noteSingole
		{
			get
			{
				return this.noteSingoleField;
			}
			set
			{
				this.noteSingoleField = value;
			}
		}

		public string tipoTrasm
		{
			get
			{
				return this.tipoTrasmField;
			}
			set
			{
				this.tipoTrasmField = value;
			}
		}

		public TrasmissioneTipoDestinatario tipoDest
		{
			get
			{
				return this.tipoDestField;
			}
			set
			{
				this.tipoDestField = value;
			}
		}

		public string dataScadenza
		{
			get
			{
				return this.dataScadenzaField;
			}
			set
			{
				this.dataScadenzaField = value;
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

		public string idTrasmUtente
		{
			get
			{
				return this.idTrasmUtenteField;
			}
			set
			{
				this.idTrasmUtenteField = value;
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

		public bool daEliminare
		{
			get
			{
				return this.daEliminareField;
			}
			set
			{
				this.daEliminareField = value;
			}
		}
	}
}
