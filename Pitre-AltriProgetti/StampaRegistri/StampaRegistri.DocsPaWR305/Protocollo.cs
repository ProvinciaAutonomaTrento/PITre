using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlInclude(typeof(ProtocolloEntrata)), XmlInclude(typeof(ProtocolloUscita)), XmlInclude(typeof(ProtocolloInterno)), XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Protocollo
	{
		private string numeroField;

		private string dataProtocollazioneField;

		private string annoField;

		private string segnaturaField;

		private string daProtocollareField;

		private string invioConfermaField;

		private string modMittDestField;

		private string modMittIntField;

		private bool modUffRefField;

		private DocumentoDatiEmergenza datiEmergenzaField;

		private ProtocolloAnnullato protocolloAnnullatoField;

		private InfoDocumento rispostaProtocolloField;

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

		public string anno
		{
			get
			{
				return this.annoField;
			}
			set
			{
				this.annoField = value;
			}
		}

		public string segnatura
		{
			get
			{
				return this.segnaturaField;
			}
			set
			{
				this.segnaturaField = value;
			}
		}

		public string daProtocollare
		{
			get
			{
				return this.daProtocollareField;
			}
			set
			{
				this.daProtocollareField = value;
			}
		}

		public string invioConferma
		{
			get
			{
				return this.invioConfermaField;
			}
			set
			{
				this.invioConfermaField = value;
			}
		}

		public string modMittDest
		{
			get
			{
				return this.modMittDestField;
			}
			set
			{
				this.modMittDestField = value;
			}
		}

		public string modMittInt
		{
			get
			{
				return this.modMittIntField;
			}
			set
			{
				this.modMittIntField = value;
			}
		}

		public bool ModUffRef
		{
			get
			{
				return this.modUffRefField;
			}
			set
			{
				this.modUffRefField = value;
			}
		}

		public DocumentoDatiEmergenza datiEmergenza
		{
			get
			{
				return this.datiEmergenzaField;
			}
			set
			{
				this.datiEmergenzaField = value;
			}
		}

		public ProtocolloAnnullato protocolloAnnullato
		{
			get
			{
				return this.protocolloAnnullatoField;
			}
			set
			{
				this.protocolloAnnullatoField = value;
			}
		}

		public InfoDocumento rispostaProtocollo
		{
			get
			{
				return this.rispostaProtocolloField;
			}
			set
			{
				this.rispostaProtocolloField = value;
			}
		}
	}
}
