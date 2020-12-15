using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocsPaDocumentale_OCS.CorteContentServices;
using log4net;

namespace DocsPaDocumentale_OCS.OCSServices
{
    /// <summary>
    /// Helper class per la gestione dei documenti in OCS
    /// </summary>
    public sealed class OCSDocumentHelper
    {
        private static ILog logger = LogManager.GetLogger(typeof(OCSDocumentHelper));
        /// <summary>
        /// 
        /// </summary>
        private OCSDocumentHelper()
        { }

        /// <summary>
        /// Reperimento proprietà ocs contenenti i valori corrispondenti del documento
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static CorteContentServices.CategoryType[] getDocumentProperties(SchedaDocumento schedaDoc, InfoUtente infoUtente)
        {
            // Lista attributi documento ocs
            List<MetadataType> metaDataList = new List<MetadataType>();

            metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.AUTORE, infoUtente.userId));
            metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.DOC_NUMBER, schedaDoc.docNumber));
            metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.OGGETTO, schedaDoc.oggetto.descrizione));
            metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.TIPO_PROTOCOLLO, schedaDoc.tipoProto));

            if (schedaDoc.tipoProto.Equals("A") || schedaDoc.tipoProto.Equals("P") || schedaDoc.tipoProto.Equals("I"))
            {
                if (schedaDoc.protocollo.numero != null)
                    metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.NUMERO_PROTOCOLLO, schedaDoc.protocollo.numero));

                if (schedaDoc.protocollo.dataProtocollazione != null)
                    metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.DATA_PROTOCOLLO, OCSServices.OCSUtils.getOCSDateStringFormat(schedaDoc.protocollo.dataProtocollazione)));

                if (schedaDoc.protocollo.segnatura != null)
                    metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.SEGNATURA, schedaDoc.protocollo.segnatura));

                if (schedaDoc.registro != null)
                    metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.CODICE_REGISTRO, DocsPaServices.DocsPaQueryHelper.getCodiceRegistroFromId(schedaDoc.registro.systemId)));

                fetchMittentiDestinatari(schedaDoc, metaDataList);

                if (schedaDoc.tipoProto.Equals("A"))
                {
                    if (((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).dataProtocolloMittente != null)
                        metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.DATA_PROTOCOLLO_MITTENTE, OCSServices.OCSUtils.getOCSDateStringFormat(((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).dataProtocolloMittente)));

                    //PROTOCOLLO_MITTENTE
                    if (((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).descrizioneProtocolloMittente != null)
                        metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.PROTOCOLLO_MITTENTE, ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).descrizioneProtocolloMittente));

                    //DATA_ARRIVO
                    if (schedaDoc.documenti != null && schedaDoc.documenti.Count > 0)
                    {
                        if (schedaDoc.documenti[0] != null && ((DocsPaVO.documento.Documento)schedaDoc.documenti[0]).dataArrivo != null && !((DocsPaVO.documento.Documento)schedaDoc.documenti[0]).dataArrivo.Equals(""))
                        {
                            metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.DATA_ARRIVO, OCSServices.OCSUtils.getOCSDateStringFormat(((DocsPaVO.documento.Documento)schedaDoc.documenti[0]).dataArrivo)));
                        }
                    }
                }

            }

            //gestione documento collegato ORA RIGUARDA ANCHE I DOCUMENTI GRIGI
            if (schedaDoc.rispostaDocumento != null)
            {
                string risposta = schedaDoc.rispostaDocumento.docNumber;
                if (!String.IsNullOrEmpty(schedaDoc.rispostaDocumento.segnatura))
                    risposta = schedaDoc.rispostaDocumento.segnatura;
                if (!String.IsNullOrEmpty(risposta)) 
                    metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.RISPOSTA_A, risposta));
            }

            //NOTE --  reperimento ultima nota visibile a tutti
            foreach (DocsPaVO.Note.InfoNota item in schedaDoc.noteDocumento)
            {
                if (item.TipoVisibilita.Equals(DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti))
                {
                    if (!string.IsNullOrEmpty(item.Testo))
                        metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.NOTE, item.Testo));
                    break;
                }
            }

            //TIPO_DOCUMENTO 
            if (schedaDoc.tipologiaAtto != null)
                metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.TIPO_DOCUMENTO, schedaDoc.tipologiaAtto.descrizione));

            //PAROLE_CHIAVE
            if (schedaDoc.paroleChiave != null)
            {
                List<string> items = new List<string>();
                foreach (DocsPaVO.documento.ParolaChiave item in schedaDoc.paroleChiave)
                    if (!string.IsNullOrEmpty(item.descrizione))
                        items.Add(item.descrizione);

                if (items.Count > 0)
                    metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.PAROLE_CHIAVE, items.ToArray()));
            }

            //ANNULLAMENTO
            if (schedaDoc.protocollo != null && schedaDoc.protocollo.protocolloAnnullato != null &&
                schedaDoc.protocollo.protocolloAnnullato.dataAnnullamento != null)
            {
                //DATA
                metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.DATA_ANNULLAMENTO_PROTOCOLLO, OCSServices.OCSUtils.getOCSDateStringFormat(schedaDoc.protocollo.protocolloAnnullato.dataAnnullamento)));

                //MOTIVO
                metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.NOTE_ANNULLAMENTO_PROTOCOLLO,
                        schedaDoc.protocollo.protocolloAnnullato.autorizzazione));
            }

            //PROTOCOLLO_EMERGENZA
            if (schedaDoc.protocollo != null && schedaDoc.datiEmergenza != null &&
               schedaDoc.datiEmergenza.dataProtocollazioneEmergenza != null)
            {
                //DATA
                metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.DATA_PROTOCOLLO_EMERGENZA,
                        OCSServices.OCSUtils.getOCSDateStringFormat(schedaDoc.datiEmergenza.dataProtocollazioneEmergenza)));

                //MOTIVO
                metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.PROTOCOLLO_EMERGENZA, schedaDoc.datiEmergenza.protocolloEmergenza));
            }

            //ABBATANGELI GIANLUIGI
            //COD_EXT_APP
            if (!string.IsNullOrEmpty(schedaDoc.codiceApplicazione))
                metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.COD_EXT_APP, schedaDoc.codiceApplicazione));

            //PREDISPOSTO ALLA PROTOCOLLAZIONE
            string predispostoProtoc = "NO";
            if (schedaDoc.predisponiProtocollazione)
                predispostoProtoc = "SI";
            metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.PREDISPOSTO_PROTOCOLLAZIONE, predispostoProtoc));

            //LIVELLO RISERVATEZZA  //privato-personale
            string livello_riservatezza = "";
            if (schedaDoc.privato != null && schedaDoc.privato.Equals("1"))
                livello_riservatezza = "PRIVATO";
            if (schedaDoc.personale != null && schedaDoc.personale.Equals("1"))
                livello_riservatezza = "PERSONALE";               
            metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.LIVELLO_RISERVATEZZA, livello_riservatezza));
            
            List<CategoryType> retValue = new List<CategoryType>();

            CategoryType categoryType = new CategoryType();
            categoryType.name = DocsPaObjectType.ObjectTypes.CATEGOTY_PROTOCOLLO;
            categoryType.metadataList = metaDataList.ToArray();
            retValue.Add(categoryType);

            return retValue.ToArray();
        }

        /// <summary>
        /// Reperimento proprietà allegato
        /// </summary>
        /// <param name="allegato"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static CorteContentServices.CategoryType[] getAttachmentProperties(Allegato allegato, InfoUtente infoUtente)
        {
            List<MetadataType> metaDataList = new List<MetadataType>();

            metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoAllegato.AUTORE, infoUtente.userId));
            metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoAllegato.DOC_NUMBER, DocsPaServices.DocsPaQueryHelper.getDocNumberDocumentoPrincipale(allegato.docNumber)));
            metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoAllegato.ATTACHID, allegato.docNumber));
            metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoAllegato.DESCRIZIONE, allegato.descrizione));
            metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoAllegato.NUM_PAGINE, allegato.numeroPagine.ToString()));

            if (!string.IsNullOrEmpty(allegato.versionLabel))
                metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoAllegato.CODICE, allegato.versionLabel.ToString()));

            List<CategoryType> retValue = new List<CategoryType>();

            CategoryType categoryType = new CategoryType();
            categoryType.name = DocsPaObjectType.ObjectTypes.CATEGOTY_ALLEGATO;
            categoryType.metadataList = metaDataList.ToArray();
            retValue.Add(categoryType);

            return retValue.ToArray();
        }

        /// <summary>
        /// Reperimento proprietà per documento di tipo stampa registro
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static CorteContentServices.CategoryType[] getDocumentStampaRegistroProperties(SchedaDocumento schedaDoc, InfoUtente infoUtente)
        {
            List<MetadataType> metaDataList = new List<MetadataType>();

            metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoStampaRegistro.AUTORE, infoUtente.userId));
            metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoStampaRegistro.DOC_NUMBER, schedaDoc.docNumber));

            if (schedaDoc.oggetto != null && !string.IsNullOrEmpty(schedaDoc.oggetto.descrizione))
                metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoStampaRegistro.OGGETTO, schedaDoc.oggetto.descrizione));

            //DATA_STAMPA
            metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoStampaRegistro.DATA_STAMPA, System.DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss", new CultureInfo("it-IT"))));

            //COD_REGISTRO
            if (schedaDoc.registro != null)
            {
                metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoStampaRegistro.CODICE_REGISTRO,
                        DocsPaServices.DocsPaQueryHelper.getCodiceRegistroFromId(schedaDoc.registro.systemId)));
            }

            List<CategoryType> retValue = new List<CategoryType>();

            CategoryType categoryType = new CategoryType();
            categoryType.name = DocsPaObjectType.ObjectTypes.CATEGOTY_STAMPA_REGISTRO;
            categoryType.metadataList = metaDataList.ToArray();
            retValue.Add(categoryType);

            return retValue.ToArray();
        }

        /// <summary>
        /// Caricamento nella lista delle proprietà ocs
        /// della descrizione dei mittenti / destinatari del documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="metaDataList"></param>        
        private static void fetchMittentiDestinatari(DocsPaVO.documento.SchedaDocumento schedaDocumento, List<MetadataType> metaDataList)
        {
            if (schedaDocumento.tipoProto.Equals("A"))
            {
                // Se il documento è in ingresso, viene reperito solo il mittente
                DocsPaVO.documento.ProtocolloEntrata pe = (DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo;

                if (pe.mittente != null && pe.mittente.descrizione != null)
                    metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.MITTENTE, pe.mittente.ToString()));
            }
            else if (schedaDocumento.tipoProto.Equals("P"))
            {
                // Se il documento è in uscita, vengono reperiti sia il mittente che i destinatari (anche per conoscenza)
                DocsPaVO.documento.ProtocolloUscita pu = (DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo;
                if (pu.mittente != null && pu.mittente.descrizione != null)
                    metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.MITTENTE, pu.mittente.ToString()));

                if (pu.destinatari != null && pu.destinatari.Count > 0)
                {
                    List<string> destList = new List<string>();
                    foreach (DocsPaVO.utente.Corrispondente item in pu.destinatari)
                        destList.Add(item.ToString());
                    metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.DESTINATARI, destList.ToArray()));
                }

                if (pu.destinatariConoscenza != null && pu.destinatariConoscenza.Count > 0)
                {
                    List<string> destList = new List<string>();
                    foreach (DocsPaVO.utente.Corrispondente item in pu.destinatariConoscenza)
                        destList.Add(item.ToString());
                    metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.DESTINATARI_CC, destList.ToArray()));
                }
            }
            else if (schedaDocumento.tipoProto.Equals("I"))
            {
                // Se il documento è interno, vengono reperiti sia il mittente che i destinatari (anche per conoscenza)
                DocsPaVO.documento.ProtocolloInterno pi = (DocsPaVO.documento.ProtocolloInterno)schedaDocumento.protocollo;
                if (pi.mittente != null && pi.mittente.descrizione != null)
                    metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.MITTENTE, pi.mittente.ToString()));

                if (pi.destinatari != null && pi.destinatari.Count > 0)
                {
                    List<string> destList = new List<string>();
                    foreach (DocsPaVO.utente.Corrispondente item in pi.destinatari)
                        destList.Add(item.ToString());
                    metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.DESTINATARI, destList.ToArray()));
                }

                if (pi.destinatariConoscenza != null && pi.destinatariConoscenza.Count > 0)
                {
                    List<string> destList = new List<string>();
                    foreach (DocsPaVO.utente.Corrispondente item in pi.destinatariConoscenza)
                        destList.Add(item.ToString());
                    metaDataList.Add(OCSUtils.getMetadataItem(DocsPaObjectType.TypeDocumentoProtocollo.DESTINATARI_CC, destList.ToArray()));
                }
            }
        }

        /// <summary>
        /// Per reperire la locazione del documento
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="objSicurezza"></param>
        /// <param name="isNewDoc"></param>
        /// <returns></returns>
        public static string getDocumentLocation(SchedaDocumento schedaDoc, DocsPaVO.utente.InfoUtente objSicurezza, bool isNewDoc)
        {
            string result = null;

            try
            {
                if (schedaDoc != null)
                {
                    //legge il record da DPA_CORR_GLOBALI in JOIN con DPA_AMMINISTRA
                    DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();

                    System.Data.DataSet corrispondente;

                    if (!documentale.DOC_GetCorrByIdPeople(objSicurezza.idPeople, out corrispondente))
                    {
                        logger.Debug("Errore nella lettura del corrispondente relativo al documento");
                        throw new Exception();
                    }

                    //legge l'amministrazione
                    string amministrazione = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(corrispondente.Tables[0].Rows[0], "VAR_CODICE_AMM", false);

                    string codiceUO = string.Empty;
                    if (schedaDoc.creatoreDocumento != null && schedaDoc.creatoreDocumento.uo_codiceCorrGlobali != null)
                        codiceUO = schedaDoc.creatoreDocumento.uo_codiceCorrGlobali;
                    else
                    {
                        codiceUO = DocsPaServices.DocsPaQueryHelper.getCodiceUO(schedaDoc.docNumber);
                        if ((codiceUO == null || codiceUO.Equals("")) && isNewDoc)
                        {
                            //legge l'id della uo di appartenenza del gruppo
                            string id = documentale.DOC_GetIdUoBySystemId(objSicurezza.idGruppo);

                            if (string.IsNullOrEmpty(id))
                            {
                                logger.Debug("Errore nella lettura del gruppo relativo al documento");
                                throw new Exception();
                            }
                            //recupera il nome della UO
                            codiceUO = documentale.DOC_GetUoById(id);
                        }
                    }

                    //legge la tabella profile
                    System.Data.DataSet documento;
                    if (!documentale.DOC_GetDocByDocNumber(schedaDoc.docNumber, out documento))
                    {
                        logger.Debug("Errore nella lettura del documento: " + schedaDoc.docNumber);
                        throw new Exception();
                    }
                    //legge l'anno di creazione del documento
                    string anno = System.DateTime.Parse(documento.Tables[0].Rows[0]["CREATION_DATE"].ToString()).Year.ToString();
                    //verifica se il documento è protocollato
                    string tipoProtocollo;
                    tipoProtocollo = documento.Tables[0].Rows[0]["CHA_TIPO_PROTO"].ToString().ToUpper();

                    string registro = "";
                    string arrivoPartenza = "";
                    if (schedaDoc.tipoProto == "A" || schedaDoc.tipoProto == "P" || schedaDoc.tipoProto == "I" || schedaDoc.tipoProto == "R")
                    {
                        //crea il path nel caso di documento protocollato -> AMMINISTRAZIONE + REGISTRO + ANNO + [COD_UO] + [ARRIVO|PARTENZA]

                        //legge il registro della protocollazione
                        //controllo che l'idRegistro esiste
                        string idRegistro = "";
                        if (schedaDoc.registro != null && !String.IsNullOrEmpty(schedaDoc.registro.systemId))
                            idRegistro = schedaDoc.registro.systemId;
                        if (String.IsNullOrEmpty(idRegistro))
                            documento.Tables[0].Rows[0]["ID_REGISTRO"].ToString();
                        registro = documentale.DOC_GetRegistroById(idRegistro);
                        if (registro == null)
                        {
                            logger.Debug("registro non trovato");
                            registro = "";
                        }

                        if (schedaDoc.tipoProto.Equals("A"))
                            arrivoPartenza = "Arrivo";
                        else if (schedaDoc.tipoProto.Equals("P"))
                            arrivoPartenza = "Partenza";
                        else if (schedaDoc.tipoProto.Equals("I"))
                            arrivoPartenza = "Interno";
                        else if (schedaDoc.tipoProto.Equals("R"))
                            arrivoPartenza = "StampaRegistro";
                    }

                    string filePath = OCSConfigurations.GetDocPathPattern();
                    filePath = filePath.Replace("AMMINISTRAZIONE", amministrazione);
                    filePath = filePath.Replace("REGISTRO", registro);
                    filePath = filePath.Replace("ANNO", anno);
                    filePath = filePath.Replace("ARRIVO_PARTENZA", arrivoPartenza);
                    filePath = filePath.Replace("UFFICIO", codiceUO);
                    filePath = filePath.Replace("UTENTE", objSicurezza.userId);

                    filePath = filePath.Replace("//", "/");
                    if (filePath.EndsWith("/"))
                        filePath = filePath.Remove(filePath.Length - 1, 1);

                    //verifica se la directory esiste
                    string pathCompleto = OCSConfigurations.GetDocRootFolder() + "/" + filePath;
                    pathCompleto = pathCompleto.Replace("//", "/");

                    //restituisce la directory
                    result = pathCompleto;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore creazione path documentale per documento: " + schedaDoc.docNumber, e);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public static long getVersionIdOCS(string docNumber, string versionId)
        {
            //NB: con l'implementazione fatta dallo strato CorteContentServices
            //    il version_id di OCS corrisponde al version_id di DocsPa. 

            return Convert.ToInt32(versionId);
        }

        /// <summary>
        /// Impostazione delle ACL per un documento
        /// </summary>
        /// <param name="idDocsPa">ID del documento in docspa</param>
        /// <param name="idOcs">ID del documento in ocs</param>
        /// <param name="infoUtente">Credenziali docspa</param>
        /// <param name="ocsCredentials">Credenziali per effettuare l'operazione in ocs</param>
        /// <returns></returns>
        public static bool setAclDocument(string idDocsPa, long idOcs, InfoUtente infoUtente, UserCredentialsType ocsCredentials)
        {
            CorteContentServices.GrantsType grants;
            CorteContentServices.ItemGrantsRequestType itemGrantsRequest;
            CorteContentServices.ItemGrantsType itemGrantsType;
            CorteContentServices.ResultType resultGrants;

            grants = OCSServices.OCSUtils.getAclSecurity(idDocsPa);

            itemGrantsRequest = new CorteContentServices.ItemGrantsRequestType();
            itemGrantsRequest.userCredentials = ocsCredentials;
            itemGrantsType = new CorteContentServices.ItemGrantsType();
            itemGrantsType.itemId = idOcs;
            itemGrantsType.grants = grants;
            itemGrantsRequest.grants = itemGrantsType;

            DocumentManagementSOAPHTTPBinding wsDocumentInstance = OCSServiceFactory.GetDocumentServiceInstance<DocumentManagementSOAPHTTPBinding>();
            resultGrants = wsDocumentInstance.SetDocumentGrants(itemGrantsRequest);

            OCSUtils.throwExceptionIfInvalidResult(resultGrants);

            return true;
        }

        /// <summary>
        /// Rimozione delle ACL per un documento
        /// </summary>
        /// <param name="idDocsPa">ID del documento in docspa</param>
        /// <param name="idOcs">ID del documento in ocs</param>
        /// <param name="infoUtente">Credenziali docspa</param>
        /// <param name="ocsCredentials">Credenziali per effettuare l'operazione in ocs</param>
        /// <returns></returns>
        public static bool removeAclDocument(string idDocsPa, long idOcs, InfoUtente infoUtente, UserCredentialsType ocsCredentials)
        {
            CorteContentServices.GrantsType grants;
            CorteContentServices.ItemGrantsRequestType itemGrantsRequest;
            CorteContentServices.ItemGrantsType itemGrantsType;
            CorteContentServices.ResultType resultGrants;

            grants = OCSServices.OCSUtils.getAclSecurity(idDocsPa);

            itemGrantsRequest = new CorteContentServices.ItemGrantsRequestType();
            itemGrantsRequest.userCredentials = ocsCredentials;
            itemGrantsType = new CorteContentServices.ItemGrantsType();
            itemGrantsType.itemId = idOcs;
            itemGrantsType.grants = grants;
            itemGrantsRequest.grants = itemGrantsType;

            DocumentManagementSOAPHTTPBinding wsDocumentInstance = OCSServiceFactory.GetDocumentServiceInstance<DocumentManagementSOAPHTTPBinding>();
            resultGrants = wsDocumentInstance.RemoveDocumentGrants(itemGrantsRequest);

            OCSUtils.throwExceptionIfInvalidResult(resultGrants);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="versionId"></param>
        /// <param name="ocsCredentials"></param>
        /// <returns></returns>
        public static long getDocumentIdOCS(string docNumber, string versionId, UserCredentialsType ocsCredentials)
        {
            long idOCS = -1;

            //Reperimento idOCS in base al path e al nome del documento
            string pathDoc = DocsPaServices.DocsPaQueryHelper.getDocumentPath(docNumber);
            string nameDoc = DocsPaServices.DocsPaQueryHelper.getDocumentName(docNumber, versionId);

            if (pathDoc != null)
                idOCS = getIdOCSByPath(pathDoc, nameDoc, ocsCredentials);

            return idOCS;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="docNumber"></param>
        ///// <param name="folderPath"></param>
        ///// <param name="ocsCredentials">Credenziali per effettuare l'operazione in ocs</param>
        ///// <returns></returns>
        //public static long getGeneralDocumentIdOCSBySearch(string docNumber, string folderPath, UserCredentialsType ocsCredentials)
        //{
        //    long idOCS = -1;

        //    //Reperimento idOCS in base ad una ricerca
        //    CorteContentServices.FilterSearchRequestType searchDocumentRequest = new CorteContentServices.FilterSearchRequestType();

        //    searchDocumentRequest.userCredentials = ocsCredentials;
        //    searchDocumentRequest.filter = new CorteContentServices.FilterSearchType();
        //    if (folderPath != null)
        //        searchDocumentRequest.filter.folderPath = folderPath;
        //    searchDocumentRequest.filter.includeSubFolders = true;
        //    searchDocumentRequest.filter.includeSubFoldersSpecified = true;
        //    searchDocumentRequest.filter.count = 0;
        //    searchDocumentRequest.filter.countSpecified = true;
        //    searchDocumentRequest.filter.offset = 0;
        //    searchDocumentRequest.filter.offsetSpecified = true;
        //    searchDocumentRequest.filter.searchExpress = new DocsPaDocumentale_OCS.CorteContentServices.searchExpress(); //TODO: da fare!!!
        //    searchDocumentRequest.filter.searchExpress.SearchExprType = new DocsPaDocumentale_OCS.CorteContentServices.SearchExprType();
        //    string leftOpCr1 = "[" + DocsPaObjectType.ObjectTypes.CATEGOTY_PROTOCOLLO + ":" + DocsPaObjectType.TypeDocumentoProtocollo.DOC_NUMBER + "]";
        //    string operatCr1 = "EQUAL";
        //    string rightOpCr1 = docNumber;
        //    searchDocumentRequest.filter.searchExpress.SearchExprType.leftOperand = leftOpCr1;
        //    searchDocumentRequest.filter.searchExpress.SearchExprType.@operator = operatCr1;
        //    searchDocumentRequest.filter.searchExpress.SearchExprType.rightOperand = rightOpCr1;

        //    searchDocumentRequest.filter.searchExpress.SearchExprType.sameLevelOperator = "OR";

        //    searchDocumentRequest.filter.searchExpress.SearchExprType.SearchExprType1 = new DocsPaDocumentale_OCS.CorteContentServices.SearchExprType();
        //    string leftOpCr2 = "[" + DocsPaObjectType.ObjectTypes.CATEGOTY_STAMPA_REGISTRO + ":" + DocsPaObjectType.TypeDocumentoStampaRegistro.DOC_NUMBER + "]";
        //    string operatCr2 = "EQUAL";
        //    string rightOpCr2 = docNumber;

        //    searchDocumentRequest.filter.searchExpress.SearchExprType.SearchExprType1.@operator = operatCr2;
        //    searchDocumentRequest.filter.searchExpress.SearchExprType.SearchExprType1.rightOperand = rightOpCr2;
        //    searchDocumentRequest.filter.searchExpress.SearchExprType.SearchExprType1.leftOperand = leftOpCr2;

        //    searchDocumentRequest.filter.searchExpress.SearchExprType.SearchExprType1.sameLevelOperator = "OR";

        //    searchDocumentRequest.filter.searchExpress.SearchExprType.SearchExprType1.SearchExprType1 = new DocsPaDocumentale_OCS.CorteContentServices.SearchExprType();
        //    string leftOpCr3 = "[" + DocsPaObjectType.ObjectTypes.CATEGOTY_ALLEGATO + ":" + DocsPaObjectType.TypeDocumentoAllegato.DOC_NUMBER + "]";
        //    string operatCr3 = "EQUAL";
        //    string rightOpCr3 = docNumber;

        //    searchDocumentRequest.filter.searchExpress.SearchExprType.SearchExprType1.SearchExprType1.@operator = operatCr3;
        //    searchDocumentRequest.filter.searchExpress.SearchExprType.SearchExprType1.SearchExprType1.rightOperand = rightOpCr3;
        //    searchDocumentRequest.filter.searchExpress.SearchExprType.SearchExprType1.SearchExprType1.leftOperand = leftOpCr3;

        //    CorteContentServices.ItemSearchListResponseType listResult;

        //    DocumentManagementSOAPHTTPBinding wsDocument = OCSServiceFactory.GetDocumentServiceInstance<DocumentManagementSOAPHTTPBinding>();
        //    listResult = wsDocument.SearchDocuments(searchDocumentRequest);

        //    OCSUtils.throwExceptionIfInvalidResult(listResult.result);

        //    // Reperimento itemId
        //    if (listResult.items != null && listResult.items.Length == 1)
        //        idOCS = listResult.items[0].itemId;

        //    return idOCS;
        //}

        /// <summary>
        /// Ricerca dell'id in ocs del documento in docspa
        /// </summary>
        /// <param name="idDocsPa"></param>
        /// <param name="documentCategoryName"></param>
        /// <param name="categoryFieldName"></param>
        /// <param name="ocsCredentials"></param>
        /// <returns></returns>
        public static long getIdDocument(string idDocsPa, string documentCategoryName, string categoryFieldName, UserCredentialsType ocsCredentials)
        {
            return getIdDocument(idDocsPa, documentCategoryName, categoryFieldName, false, ocsCredentials);
        }

        /// <summary>
        /// Ricerca dell'id in ocs del documento in docspa
        /// </summary>
        /// <param name="idDocsPa">
        /// Id del documento in docspa da cercare in ocs
        /// </param>
        /// <param name="documentCategoryName">
        /// Categoria di appartenenza del documento
        /// </param>
        /// <param name="categoryFieldName">
        /// Campo della categoria in base a cui cercare il documento
        /// </param>
        /// <param name="searchInCestino">
        /// Se true, indica di cercare il documento nel cestino
        /// </param>
        /// <param name="ocsCredentials"></param>
        /// <returns>
        /// Corrispondente id in ocs del documento
        /// </returns>
        public static long getIdDocument(string idDocsPa, string documentCategoryName, string categoryFieldName, bool searchInCestino, UserCredentialsType ocsCredentials)
        {
            long idOCS = -1;

            //Reperimento idOCS in base ad una ricerca
            CorteContentServices.FilterSearchRequestType searchDocumentRequest = new CorteContentServices.FilterSearchRequestType();

            searchDocumentRequest.userCredentials = ocsCredentials;
            searchDocumentRequest.filter = new CorteContentServices.FilterSearchType();

            if (searchInCestino)
                // Ricerca del documento nella cartella del cestino
                searchDocumentRequest.filter.folderPath = OCSConfigurations.GetPathCestino();
            else
                // Ricerca del documento a partire dalla root path della libreria ocs corrente
                searchDocumentRequest.filter.folderPath = OCSConfigurations.GetDocRootFolder();

            searchDocumentRequest.filter.includeSubFolders = true;
            searchDocumentRequest.filter.includeSubFoldersSpecified = true;
            searchDocumentRequest.filter.count = 0;
            searchDocumentRequest.filter.countSpecified = true;
            searchDocumentRequest.filter.offset = 0;
            searchDocumentRequest.filter.offsetSpecified = true;
            searchDocumentRequest.filter.searchExpress = new DocsPaDocumentale_OCS.CorteContentServices.searchExpress();
            searchDocumentRequest.filter.searchExpress.SearchExprType = new DocsPaDocumentale_OCS.CorteContentServices.SearchExprType();
            string leftOpCr1 = string.Format("[{0}:{1}]", documentCategoryName, categoryFieldName);
            string operatCr1 = "EQUAL";
            string rightOpCr1 = idDocsPa;
            searchDocumentRequest.filter.searchExpress.SearchExprType.leftOperand = leftOpCr1;
            searchDocumentRequest.filter.searchExpress.SearchExprType.@operator = operatCr1;
            searchDocumentRequest.filter.searchExpress.SearchExprType.rightOperand = rightOpCr1;

            searchDocumentRequest.filter.searchExpress.SearchExprType.sameLevelOperator = "AND";

            searchDocumentRequest.filter.searchExpress.SearchExprType.SearchExprType1 = new DocsPaDocumentale_OCS.CorteContentServices.SearchExprType();
            string operatCr2 = "HAS_CATEGORY";
            string rightOpCr2 = string.Format("[{0}]", documentCategoryName);

            searchDocumentRequest.filter.searchExpress.SearchExprType.SearchExprType1.@operator = operatCr2;
            searchDocumentRequest.filter.searchExpress.SearchExprType.SearchExprType1.rightOperand = rightOpCr2;

            CorteContentServices.ItemSearchListResponseType listResult;

            DocumentManagementSOAPHTTPBinding wsDocument = OCSServiceFactory.GetDocumentServiceInstance<DocumentManagementSOAPHTTPBinding>();
            listResult = wsDocument.SearchDocuments(searchDocumentRequest);

            OCSUtils.throwExceptionIfInvalidResult(listResult.result);

            // Reperimento itemId
            if (listResult.items != null && listResult.items.Length == 1)
                idOCS = listResult.items[0].itemId;

            return idOCS;
        }

        /// <summary>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="versionId"></param>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="ocsCredentials">Credenziali per effettuare l'operazione in ocs</param>
        /// <returns></returns>
        public static long getDocumentIdOCSByPath(string docNumber, string versionId, string path, string name, UserCredentialsType ocsCredentials)
        {
            long idOCS = -1;

            //Reperimento idOCS in base al path e al nome del documento
            string pathDoc = path;
            if (pathDoc == null || pathDoc.Equals(""))
                pathDoc = DocsPaServices.DocsPaQueryHelper.getDocumentPath(docNumber);

            string nameDoc = name;
            if (nameDoc == null || nameDoc.Equals(""))
                nameDoc = DocsPaServices.DocsPaQueryHelper.getDocumentName(docNumber, versionId);

            if (pathDoc != null)
                idOCS = getIdOCSByPath(pathDoc, nameDoc, ocsCredentials);

            return idOCS;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="versionId"></param>
        /// <param name="ocsCredentials">Credenziali per effettuare l'operazione in ocs</param>
        /// <returns></returns>
        public static long getAttachIdOCS(string docNumber, string versionId, UserCredentialsType ocsCredentials)
        {
            long idOCS = -1;

            //Reperimento idOCS in base al path e al nome del documento
            string pathDoc = DocsPaServices.DocsPaQueryHelper.getDocumentPath(docNumber);
            string nameDoc = DocsPaServices.DocsPaQueryHelper.getDocumentName(docNumber, versionId);

            if (pathDoc != null)
                idOCS = getIdOCSByPath(pathDoc, nameDoc, ocsCredentials);

            return idOCS;
        }

        /// <summary>
        /// Accesso ad OCS usando il metodo GetDocumentByName
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="ocsCredentials">Credenziali per effettuare l'operazione in ocs</param>
        /// <returns></returns>
        private static long getIdOCSByPath(string path, string name, UserCredentialsType ocsCredentials)
        {
            CorteContentServices.DocumentNameSearchRequestType nameReq = new CorteContentServices.DocumentNameSearchRequestType();
            CorteContentServices.ItemIdResponseType itemIdResp;

            nameReq.userCredentials = ocsCredentials;
            nameReq.document = new DocsPaDocumentale_OCS.CorteContentServices.DocumentNameSearchType();
            nameReq.document.info = new DocsPaDocumentale_OCS.CorteContentServices.InfoType();
            nameReq.document.info.location = path;
            nameReq.document.info.name = name;

            DocumentManagementSOAPHTTPBinding wsDocument = OCSServiceFactory.GetDocumentServiceInstance<DocumentManagementSOAPHTTPBinding>();
            itemIdResp = wsDocument.GetDocumentByName(nameReq);

            OCSUtils.throwExceptionIfInvalidResult(itemIdResp.result);

            return itemIdResp.itemId;
        }
    }
}