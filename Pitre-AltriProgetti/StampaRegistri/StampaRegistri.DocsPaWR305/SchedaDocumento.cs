using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class SchedaDocumento
	{
		private string systemIdField;

		private string dataCreazioneField;

		private string noteField;

		private string docNumberField;

		private string tipoProtoField;

		private string accessRightsField;

		private string idPeopleField;

		private string userIdField;

		private string typeIdField;

		private string appIdField;

		private string modOggettoField;

		private string assegnatoField;

		private string fascicolatoField;

		private string privatoField;

		private string tipoSessoField;

		private string autoreField;

		private string numOggettoField;

		private string commissioneRefField;

		private int idFasciaEtaField;

		private string evidenzaField;

		private bool daAggiornareNoteField;

		private bool daAggiornareParoleChiaveField;

		private bool predisponiProtocollazioneField;

		private bool daAggiornareTipoAttoField;

		private Protocollo protocolloField;

		private Oggetto oggettoField;

		private TipologiaAtto tipologiaAttoField;

		private Registro registroField;

		private Protocollatore protocollatoreField;

		private Folder[] folderField;

		private Documento[] documentiField;

		private Allegato[] allegatiField;

		private DocumentoParolaChiave[] paroleChiaveField;

		private string[] destinatariModificatiField;

		private string[] destinatariCCModificatiField;

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

		public string dataCreazione
		{
			get
			{
				return this.dataCreazioneField;
			}
			set
			{
				this.dataCreazioneField = value;
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

		public string docNumber
		{
			get
			{
				return this.docNumberField;
			}
			set
			{
				this.docNumberField = value;
			}
		}

		public string tipoProto
		{
			get
			{
				return this.tipoProtoField;
			}
			set
			{
				this.tipoProtoField = value;
			}
		}

		public string accessRights
		{
			get
			{
				return this.accessRightsField;
			}
			set
			{
				this.accessRightsField = value;
			}
		}

		public string idPeople
		{
			get
			{
				return this.idPeopleField;
			}
			set
			{
				this.idPeopleField = value;
			}
		}

		public string userId
		{
			get
			{
				return this.userIdField;
			}
			set
			{
				this.userIdField = value;
			}
		}

		public string typeId
		{
			get
			{
				return this.typeIdField;
			}
			set
			{
				this.typeIdField = value;
			}
		}

		public string appId
		{
			get
			{
				return this.appIdField;
			}
			set
			{
				this.appIdField = value;
			}
		}

		public string modOggetto
		{
			get
			{
				return this.modOggettoField;
			}
			set
			{
				this.modOggettoField = value;
			}
		}

		public string assegnato
		{
			get
			{
				return this.assegnatoField;
			}
			set
			{
				this.assegnatoField = value;
			}
		}

		public string fascicolato
		{
			get
			{
				return this.fascicolatoField;
			}
			set
			{
				this.fascicolatoField = value;
			}
		}

		public string privato
		{
			get
			{
				return this.privatoField;
			}
			set
			{
				this.privatoField = value;
			}
		}

		public string tipoSesso
		{
			get
			{
				return this.tipoSessoField;
			}
			set
			{
				this.tipoSessoField = value;
			}
		}

		public string autore
		{
			get
			{
				return this.autoreField;
			}
			set
			{
				this.autoreField = value;
			}
		}

		public string numOggetto
		{
			get
			{
				return this.numOggettoField;
			}
			set
			{
				this.numOggettoField = value;
			}
		}

		public string commissioneRef
		{
			get
			{
				return this.commissioneRefField;
			}
			set
			{
				this.commissioneRefField = value;
			}
		}

		public int idFasciaEta
		{
			get
			{
				return this.idFasciaEtaField;
			}
			set
			{
				this.idFasciaEtaField = value;
			}
		}

		public string evidenza
		{
			get
			{
				return this.evidenzaField;
			}
			set
			{
				this.evidenzaField = value;
			}
		}

		public bool daAggiornareNote
		{
			get
			{
				return this.daAggiornareNoteField;
			}
			set
			{
				this.daAggiornareNoteField = value;
			}
		}

		public bool daAggiornareParoleChiave
		{
			get
			{
				return this.daAggiornareParoleChiaveField;
			}
			set
			{
				this.daAggiornareParoleChiaveField = value;
			}
		}

		public bool predisponiProtocollazione
		{
			get
			{
				return this.predisponiProtocollazioneField;
			}
			set
			{
				this.predisponiProtocollazioneField = value;
			}
		}

		public bool daAggiornareTipoAtto
		{
			get
			{
				return this.daAggiornareTipoAttoField;
			}
			set
			{
				this.daAggiornareTipoAttoField = value;
			}
		}

		public Protocollo protocollo
		{
			get
			{
				return this.protocolloField;
			}
			set
			{
				this.protocolloField = value;
			}
		}

		public Oggetto oggetto
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

		public TipologiaAtto tipologiaAtto
		{
			get
			{
				return this.tipologiaAttoField;
			}
			set
			{
				this.tipologiaAttoField = value;
			}
		}

		public Registro registro
		{
			get
			{
				return this.registroField;
			}
			set
			{
				this.registroField = value;
			}
		}

		public Protocollatore protocollatore
		{
			get
			{
				return this.protocollatoreField;
			}
			set
			{
				this.protocollatoreField = value;
			}
		}

		public Folder[] folder
		{
			get
			{
				return this.folderField;
			}
			set
			{
				this.folderField = value;
			}
		}

		public Documento[] documenti
		{
			get
			{
				return this.documentiField;
			}
			set
			{
				this.documentiField = value;
			}
		}

		public Allegato[] allegati
		{
			get
			{
				return this.allegatiField;
			}
			set
			{
				this.allegatiField = value;
			}
		}

		public DocumentoParolaChiave[] paroleChiave
		{
			get
			{
				return this.paroleChiaveField;
			}
			set
			{
				this.paroleChiaveField = value;
			}
		}

		public string[] destinatariModificati
		{
			get
			{
				return this.destinatariModificatiField;
			}
			set
			{
				this.destinatariModificatiField = value;
			}
		}

		public string[] destinatariCCModificati
		{
			get
			{
				return this.destinatariCCModificatiField;
			}
			set
			{
				this.destinatariCCModificatiField = value;
			}
		}
	}
}
