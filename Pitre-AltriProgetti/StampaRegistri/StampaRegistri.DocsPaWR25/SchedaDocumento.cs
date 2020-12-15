using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class SchedaDocumento
	{
		private string systemIdField;

		private string dataCreazioneField;

		private string noteField;

		private string docNumberField;

		private Oggetto oggettoField;

		private DocumentoStatoDocumento statoField;

		private TipologiaAtto tipologiaAttoField;

		private Atto attoField;

		private Folder[] folderField;

		private string tipoProtoField;

		private Protocollo protocolloField;

		private Documento[] documentiField;

		private Allegato[] allegatiField;

		private string accessRightsField;

		private string idPeopleField;

		private string userIdField;

		private string typeIdField;

		private string appIdField;

		private Registro registroField;

		private DocumentoParolaChiave[] paroleChiaveField;

		private bool daAggiornareNoteField;

		private bool daAggiornareParoleChiaveField;

		private bool predisponiProtocollazioneField;

		private string modOggettoField;

		private string assegnatoField;

		private string fascicolatoField;

		private string privatoField;

		private string numOggettoField;

		private string commissioneRefField;

		private string evidenzaField;

		private string tiposessoField;

		private int idfasciaEtaField;

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

		public DocumentoStatoDocumento stato
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

		public Atto atto
		{
			get
			{
				return this.attoField;
			}
			set
			{
				this.attoField = value;
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

		public string tiposesso
		{
			get
			{
				return this.tiposessoField;
			}
			set
			{
				this.tiposessoField = value;
			}
		}

		public int idfasciaEta
		{
			get
			{
				return this.idfasciaEtaField;
			}
			set
			{
				this.idfasciaEtaField = value;
			}
		}
	}
}
