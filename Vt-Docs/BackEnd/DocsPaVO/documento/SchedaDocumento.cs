using System;
using System.Collections;
using System.Xml.Serialization;
using DocsPaVO.areaConservazione;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
	[XmlInclude(typeof(ProtocolloEntrata))]
	[XmlInclude(typeof(ProtocolloUscita))]
	[XmlInclude(typeof(ProtocolloInterno))]
    [XmlInclude(typeof(DocsPaVO.Note.AssociazioneNota))]
    [XmlInclude(typeof(DocsPaVO.Note.FiltroRicercaNote))]
    [XmlInclude(typeof(DocsPaVO.ProfilazioneDinamica.Templates))]
    [XmlInclude(typeof(DocsPaVO.Security.InfoAtipicita))]
    [XmlInclude(typeof(DocsPaVO.Spedizione.SpedizioneDocumento))]
    [Serializable()]
	public class SchedaDocumento 
	{
        /// <summary>
        /// identificativo univoco che identifica la schedaDocumento
        /// </summary>
		public string systemId;
        /// <summary>
        /// data creazione
        /// </summary>
		public string dataCreazione;
		public string oraCreazione;
		public string docNumber;	
		public string tipoProto;
		public string accessRights;
		public string idPeople;
        /// <summary>
        /// userID del utente creatore/protocollatore
        /// </summary>
		public string userId;
		public string typeId;   
		public string appId;
		public string modOggetto;
		public string assegnato;
		public string fascicolato;
		public string privato;
        public bool pubblico = false;
        public string personale;
		public string tipoSesso;
		public string autore;
		public string numOggetto;
		public string commissioneRef;
		public int idFasciaEta;
		public string evidenza;
		public bool daAggiornareParoleChiave = false;
        public bool daAggiornarePrivato = false;
		public bool predisponiProtocollazione = false;
		public bool daAggiornareTipoAtto = false;
		public Protocollo protocollo;
		public Oggetto oggetto;
		public TipologiaAtto tipologiaAtto;
        /// <summary>
        /// registro del protocollo
        /// </summary>
		public DocsPaVO.utente.Registro registro;
		public Protocollatore protocollatore; 
		public CreatoreDocumento creatoreDocumento;
        /// <summary>
        /// dati del protocollo emergenza
        /// </summary>
        public DatiEmergenza datiEmergenza;
		public string interop;
        /// <summary>
        /// data scadenza documento
        /// </summary>
        public string dataScadenza;
        public string inCestino;
        public string inArchivio;
        public string mezzoSpedizione;
        public string descMezzoSpedizione;
        public DocsPaVO.ProfilazioneDinamica.Templates template;
        /// <summary>
        /// Per controllare se la dataArrivo va storicizzata
        /// 1 da storicizzare 
        /// 0 stroricizzato
        /// </summary>
        public string dtaArrivoDaStoricizzare = "0";

        public bool pregresso = false;

        public DocsPaVO.Spedizione.SpedizioneDocumento spedizioneDocumento;
		
        ////Libro firma
        //public bool inLibroFirma = false;

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.fascicolazione.Folder))]
		public System.Collections.ArrayList folder;
		
		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.documento.Documento))]
		public System.Collections.ArrayList documenti;
		
		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.documento.Allegato))]
		public System.Collections.ArrayList allegati;
		
		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.documento.ParolaChiave))]
		public ArrayList paroleChiave;

		[XmlArray()]
		[XmlArrayItem(typeof(string))]
		public ArrayList destinatariModificati;

		[XmlArray()]
		[XmlArrayItem(typeof(string))]
		public ArrayList destinatariCCModificati;

        /// <summary>
        /// Informazioni di stato se il documento è in stato checkout
        /// </summary>
        public CheckInOut.CheckOutStatus checkOutStatus = null;

        /// <summary>
        /// Note del documento
        /// </summary>
        public System.Collections.Generic.List<DocsPaVO.Note.InfoNota> noteDocumento = new System.Collections.Generic.List<Note.InfoNota>();

        /// <summary>
        /// Se allegato, rappresenta il documento principale che lo contiene
        /// </summary>
        public InfoDocumento documentoPrincipale = null;

        public bool modificaRispostaDocumento = false;
        public InfoDocumento rispostaDocumento = null;

        /// <summary>
        /// Questo campo quando è impostato a "0" inibisce l'estensione gerarchica della visibilità 
        /// di un documento privato la cui ragione di trasmissione prevede invece tale estensione.
        /// </summary>
        public string eredita = string.Empty;

         //per la gestione del campo CODICE RF nella segnatura del documento
        public string cod_rf_prot = string.Empty;
        public string id_rf_prot = string.Empty;

       /// <summary>
        /// Indica il contesto di esecuzione del repository temporaneo per l'inserimento dei file
        /// riservato al documento finché i metatati non vengono persistiti
        /// </summary>
        /// <remarks>
        /// L'oggetto è valorizzato solamente in fase di creazione di un nuovo documento
        /// </remarks>
        public SessionRepositoryContext repositoryContext = null;
        /// <summary>
        /// Il protocollo titolario del documento
        /// </summary>
        public string protocolloTitolario = string.Empty;

        /// <summary>
        /// Il numero progressivo nel fascicolo del protocollo titolario
        /// </summary>
        public string numInFasc = string.Empty;

        /// <summary>
        /// System id del fascicolo che ha generato il protocollo titolario
        /// </summary>
        public string idFascProtoTit = string.Empty;

        /// <summary>
        /// Numero protocollo nodo titolario
        /// </summary>
        public string numProtTit = string.Empty;

        /// <summary>
        /// System Id del titolario sul quale è stato generato il protocollo titolario
        /// </summary>
        public string idTitolario = string.Empty;

        /// <summary>
        /// Riferimento mittente usato per il riscontro
        /// </summary>
        public string riferimentoMittente = string.Empty;
        public string id_rf_invio_ricevuta = string.Empty;
        public string codiceFascicolo = string.Empty;

        public string documento_da_pec = string.Empty;

        /// <summary>
        /// Stato di consolidamento del documento
        /// </summary>
        public DocsPaVO.documento.DocumentConsolidationStateInfo ConsolidationState = new DocumentConsolidationStateInfo();

        /// <summary>
        /// Indica se per l'utente corrente le versioni del documento precedenti a quella corrente siano nascoste o meno.
        /// Per default, non sono nascoste.
        /// </summary>
        public bool previousVersionsHidden = false;

        /// <summary>
        /// Codice univoco dell'applicazione di appartenenza
        /// </summary>
        public string codiceApplicazione = string.Empty;

        public InfoDocumento InfoDocumento
        {
            get
            {
                InfoDocumento infoDoc = new InfoDocumento();
                infoDoc.idProfile = this.systemId;
                infoDoc.oggetto = this.oggetto.descrizione;
                infoDoc.docNumber = this.docNumber;
                infoDoc.tipoProto = this.tipoProto;
                infoDoc.evidenza = this.evidenza;

                if (this.registro != null)
                {
                    infoDoc.codRegistro = this.registro.codRegistro;
                    infoDoc.idRegistro = this.registro.systemId;
                }

                if (this.protocollo != null)
                {
                    infoDoc.numProt = this.protocollo.numero;
                    infoDoc.daProtocollare = this.protocollo.daProtocollare;
                    infoDoc.dataApertura = this.protocollo.dataProtocollazione;
                    infoDoc.segnatura = this.protocollo.segnatura;

                    if (this.protocollo.GetType().Equals(typeof(ProtocolloEntrata)))
                    {
                        infoDoc.mittDest = new ArrayList();
                        ProtocolloEntrata pe = (ProtocolloEntrata)this.protocollo;

                        if (pe != null && pe.mittente != null && infoDoc.mittDest != null && infoDoc.mittDest.Count > 0)
                        {
                            infoDoc.mittDest[0] = pe.mittente.descrizione;
                        }
                    }
                    else if (this.protocollo.GetType().Equals(typeof(ProtocolloUscita)))
                    {
                        ProtocolloUscita pu = (ProtocolloUscita)this.protocollo;
                        if (pu.destinatari != null)
                        {
                            infoDoc.mittDest = new ArrayList();
                            for (int i = 0; i < pu.destinatari.Count; i++)
                                infoDoc.mittDest[i] = ((Corrispondente)pu.destinatari[i]).descrizione;
                        }
                    }

                }
                else
                {
                    infoDoc.dataApertura = this.dataCreazione;
                }

                infoDoc.privato = this.privato;
                infoDoc.personale = this.personale;

                infoDoc.codiceApplicazione = this.codiceApplicazione;

                return infoDoc;
            }
        }

        /// <summary>
        /// Identificativo dell'ultimo documento di Inoltro generato a partire da questo documento
        /// </summary>
        public String LastForward { get; set; }

        /// <summary>
        /// Informazioni sulla atipicita del documento
        /// </summary>
        public DocsPaVO.Security.InfoAtipicita InfoAtipicita;

        public string inConservazione = string.Empty;
        public bool isRiprodotto = false;
	}
}
