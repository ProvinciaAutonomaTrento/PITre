// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Runtime.Serialization;
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
    [DataContract]
	public class SchedaDocumento 
	{
        /// <summary>
        /// identificativo univoco che identifica la schedaDocumento
        /// </summary>
        [DataMember]
        public string systemId { get; set; }
        /// <summary>
        /// data creazione
        /// </summary>
        [DataMember]
        public string dataCreazione { get; set; }
        [DataMember]
        public string oraCreazione { get; set; }
        [DataMember]
        public string docNumber { get; set; }
        [DataMember]
        public string tipoProto { get; set; }
        [DataMember]
        public string accessRights { get; set; }
        [DataMember]
        public string idPeople { get; set; }
        /// <summary>
        /// userID del utente creatore/protocollatore
        /// </summary>
        [DataMember]
        public string userId { get; set; }
        [DataMember]
        public string typeId { get; set; }
        [DataMember]
        public string appId { get; set; }
        [DataMember]
        public string modOggetto { get; set; }
        [DataMember]
        public string assegnato { get; set; }
        [DataMember]
        public string fascicolato { get; set; }
        [DataMember]
        public string privato { get; set; }
        [DataMember]
        public bool pubblico { get; set; } = false;
        [DataMember]
        public string personale { get; set; }
        [DataMember]
        public string tipoSesso { get; set; }
        [DataMember]
        public string autore { get; set; }
        [DataMember]
        public string numOggetto { get; set; }
        [DataMember]
        public string commissioneRef { get; set; }
        [DataMember]
        public int idFasciaEta { get; set; }
        [DataMember]
        public string evidenza { get; set; }
        [DataMember]
        public bool daAggiornareParoleChiave { get; set; } = false;
        [DataMember]
        public bool daAggiornarePrivato { get; set; } = false;
        [DataMember]
        public bool predisponiProtocollazione { get; set; } = false;
        [DataMember]
        public bool daAggiornareTipoAtto { get; set; } = false;
        [DataMember]
        public Protocollo protocollo { get; set; }
        [DataMember]
        public Oggetto oggetto { get; set; }
        [DataMember]
        public TipologiaAtto tipologiaAtto { get; set; }
        /// <summary>
        /// registro del protocollo
        /// </summary>
        [DataMember]
        public DocsPaVO.utente.Registro registro { get; set; }
        [DataMember]
        public Protocollatore protocollatore { get; set; }
        [DataMember]
        public CreatoreDocumento creatoreDocumento { get; set; }
        /// <summary>
        /// dati del protocollo emergenza
        /// </summary>
        [DataMember]
        public DatiEmergenza datiEmergenza { get; set; }
        [DataMember]
        public string interop { get; set; }
        /// <summary>
        /// data scadenza documento
        /// </summary>
        [DataMember]
        public string dataScadenza { get; set; }
        [DataMember]
        public string inCestino { get; set; }
        [DataMember]
        public string inArchivio { get; set; }
        [DataMember]
        public string mezzoSpedizione { get; set; }
        [DataMember]
        public string descMezzoSpedizione { get; set; }
        [DataMember]
        public DocsPaVO.ProfilazioneDinamica.Templates template { get; set; }
        /// <summary>
        /// Per controllare se la dataArrivo va storicizzata
        /// 1 da storicizzare 
        /// 0 stroricizzato
        /// </summary>
        [DataMember]
        public string dtaArrivoDaStoricizzare { get; set; } = "0";

        [DataMember]
        public bool pregresso = false;

        [DataMember]
        public DocsPaVO.Spedizione.SpedizioneDocumento spedizioneDocumento { get; set; }

        ////Libro firma
        //public bool inLibroFirma = false;

        [XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.fascicolazione.Folder))]
        [DataMember]
        public System.Collections.ArrayList folder { get; set; }

        [XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.documento.Documento))]
        [DataMember]
        public System.Collections.ArrayList documenti { get; set; }

        [XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.documento.Allegato))]
        [DataMember]
        public System.Collections.ArrayList allegati { get; set; }

        [XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.documento.ParolaChiave))]
        [DataMember]
        public ArrayList paroleChiave { get; set; }

        [XmlArray()]
		[XmlArrayItem(typeof(string))]
        [DataMember]
        public ArrayList destinatariModificati { get; set; }

        [XmlArray()]
		[XmlArrayItem(typeof(string))]
        [DataMember]
        public ArrayList destinatariCCModificati { get; set; }

        /// <summary>
        /// Informazioni di stato se il documento � in stato checkout
        /// </summary>
        [DataMember]
        public CheckInOut.CheckOutStatus checkOutStatus { get; set; } = null;

        /// <summary>
        /// Note del documento
        /// </summary>
        [DataMember]
        public System.Collections.Generic.List<DocsPaVO.Note.InfoNota> noteDocumento { get; set; } = new System.Collections.Generic.List<Note.InfoNota>();

        /// <summary>
        /// Se allegato, rappresenta il documento principale che lo contiene
        /// </summary>
        [DataMember]
        public InfoDocumento documentoPrincipale { get; set; } = null;

        [DataMember]
        public bool modificaRispostaDocumento { get; set; } = false;
        [DataMember]
        public InfoDocumento rispostaDocumento { get; set; } = null;

        /// <summary>
        /// Questo campo quando � impostato a "0" inibisce l'estensione gerarchica della visibilit� 
        /// di un documento privato la cui ragione di trasmissione prevede invece tale estensione.
        /// </summary>
        [DataMember]
        public string eredita { get; set; } = string.Empty;

        //per la gestione del campo CODICE RF nella segnatura del documento
        [DataMember]
        public string cod_rf_prot { get; set; } = string.Empty;
        [DataMember]
        public string id_rf_prot { get; set; } = string.Empty;

        /// <summary>
        /// Indica il contesto di esecuzione del repository temporaneo per l'inserimento dei file
        /// riservato al documento finch� i metatati non vengono persistiti
        /// </summary>
        /// <remarks>
        /// L'oggetto � valorizzato solamente in fase di creazione di un nuovo documento
        /// </remarks>
        [DataMember]
        public SessionRepositoryContext repositoryContext { get; set; } = null;
        /// <summary>
        /// Il protocollo titolario del documento
        /// </summary>
        [DataMember]
        public string protocolloTitolario { get; set; } = string.Empty;

        /// <summary>
        /// Il numero progressivo nel fascicolo del protocollo titolario
        /// </summary>
        [DataMember]
        public string numInFasc { get; set; } = string.Empty;

        /// <summary>
        /// System id del fascicolo che ha generato il protocollo titolario
        /// </summary>
        [DataMember]
        public string idFascProtoTit { get; set; } = string.Empty;

        /// <summary>
        /// Numero protocollo nodo titolario
        /// </summary>
        [DataMember]
        public string numProtTit { get; set; } = string.Empty;

        /// <summary>
        /// System Id del titolario sul quale � stato generato il protocollo titolario
        /// </summary>
        [DataMember]
        public string idTitolario { get; set; } = string.Empty;

        /// <summary>
        /// Riferimento mittente usato per il riscontro
        /// </summary>
        [DataMember]
        public string riferimentoMittente { get; set; } = string.Empty;
        [DataMember]
        public string id_rf_invio_ricevuta { get; set; } = string.Empty;
        [DataMember]
        public string codiceFascicolo { get; set; } = string.Empty;

        [DataMember]
        public string documento_da_pec { get; set; } = string.Empty;

        /// <summary>
        /// Stato di consolidamento del documento
        /// </summary>
        [DataMember]
        public DocsPaVO.documento.DocumentConsolidationStateInfo ConsolidationState { get; set; } = new DocumentConsolidationStateInfo();

        /// <summary>
        /// Indica se per l'utente corrente le versioni del documento precedenti a quella corrente siano nascoste o meno.
        /// Per default, non sono nascoste.
        /// </summary>
        [DataMember]
        public bool previousVersionsHidden { get; set; } = false;

        /// <summary>
        /// Codice univoco dell'applicazione di appartenenza
        /// </summary>
        [DataMember]
        public string codiceApplicazione { get; set; } = string.Empty;

        [DataMember]
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
        [DataMember]
        public String LastForward { get; set; }

        /// <summary>
        /// Informazioni sulla atipicita del documento
        /// </summary>
        [DataMember]
        public DocsPaVO.Security.InfoAtipicita InfoAtipicita { get; set; }

        [DataMember]
        public string inConservazione { get; set; } = string.Empty;
        [DataMember]
        public bool isRiprodotto { get; set; } = false;

        [DataMember]
        public DocsPaVO.fascicolazione.Fascicolo fascicolo { get; set; } = null;
    }
}
