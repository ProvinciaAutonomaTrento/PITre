using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Trasmissione : TrasmissioneOggettoTrasm
	{
		private TrasmissioneSingola[] trasmissioniSingoleField;

		private Utente utenteField;

		private Ruolo ruoloField;

		private TrasmissioneTipoOggetto tipoOggettoField;

		private string noteGeneraliField;

		private string dataInvioField;

		private string systemIdField;

		private bool daAggiornareField;

		public TrasmissioneSingola[] trasmissioniSingole
		{
			get
			{
				return this.trasmissioniSingoleField;
			}
			set
			{
				this.trasmissioniSingoleField = value;
			}
		}

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

		public Ruolo ruolo
		{
			get
			{
				return this.ruoloField;
			}
			set
			{
				this.ruoloField = value;
			}
		}

		public TrasmissioneTipoOggetto tipoOggetto
		{
			get
			{
				return this.tipoOggettoField;
			}
			set
			{
				this.tipoOggettoField = value;
			}
		}

		public string noteGenerali
		{
			get
			{
				return this.noteGeneraliField;
			}
			set
			{
				this.noteGeneraliField = value;
			}
		}

		public string dataInvio
		{
			get
			{
				return this.dataInvioField;
			}
			set
			{
				this.dataInvioField = value;
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
