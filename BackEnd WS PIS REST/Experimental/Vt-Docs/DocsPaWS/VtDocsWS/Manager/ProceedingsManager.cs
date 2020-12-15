using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using VtDocsWS.Domain;
using VtDocsWS.WebServices;
using log4net;

namespace VtDocsWS.Manager
{
    public class ProceedingsManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(ProceedingsManager));

        /// <summary>
        /// Avvia un nuovo procedimento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Services.Proceedings.StartProceeding.StartProceedingResponse StartProceeding(Services.Proceedings.StartProceeding.StartProceedingRequest request)
        {
            Services.Proceedings.StartProceeding.StartProceedingResponse response = new Services.Proceedings.StartProceeding.StartProceedingResponse();
            try
            {
                #region AUTENTICAZIONE E VALIDAZIONE PARAMETRI
                DocsPaVO.utente.Utente user = null;
                DocsPaVO.utente.Ruolo role = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = null;

                infoUtente = Utils.CheckAuthentication(request, "StartProceeding");
                user = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (user == null)
                {
                    throw new PisException("USER_NO_EXIST");
                }
                role = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                if (role == null)
                {
                    throw new PisException("ROLE_NO_EXIST");
                }

                // Validazione
                if (request.Proceeding.User == null || (string.IsNullOrEmpty(request.Proceeding.User.NationalIdentificationNumber) && string.IsNullOrEmpty(request.Proceeding.User.VatNumber)))
                {
                    throw new PisException("MISSING_USER");
                }

                if (request.Content == null || request.Content.Length == 0)
                {
                    throw new PisException("MISSING_FILE_CONTENT");
                }

                BusinessLogic.Modelli.AsposeModelProcessor.PdfModelProcessor processor = new BusinessLogic.Modelli.AsposeModelProcessor.PdfModelProcessor(request.Content);

                #endregion

                #region CONTROLLO PRESENZA ED EVENTUALE INSERIMENTO UTENTE IN RUBRICA
                logger.Debug("Controllo esistenza utente in rubrica");
                string addressBookCode = !string.IsNullOrEmpty(request.Proceeding.User.VatNumber) ? request.Proceeding.User.VatNumber : request.Proceeding.User.NationalIdentificationNumber;
                logger.Debug("Codice rubrica: " + addressBookCode);

                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(addressBookCode, infoUtente);
                if (corr != null)
                {
                    logger.Debug("Corrispondente già presente in rubrica");
                }
                else
                {
                    logger.Debug("Corrispondente non presente in rubrica");

                    // Configuro i parametri
                    request.Proceeding.User.Code = addressBookCode;
                    request.Proceeding.User.CorrespondentType = "P";
                    request.Proceeding.User.PreferredChannel = "PORTALE";
                    request.Proceeding.User.Type = "E";

                    corr = Utils.GetCorrespondentFromPisNewInsert(request.Proceeding.User, infoUtente);
                    corr = BusinessLogic.Utenti.addressBookManager.insertCorrispondente(corr, null);
                    logger.Debug("Inserito nuovo corrispondente " + corr.codiceRubrica);
                }
                #endregion

                #region CREAZIONE DOCUMENTO
                DocsPaVO.documento.SchedaDocumento doc = new DocsPaVO.documento.SchedaDocumento();
                DocsPaVO.documento.SchedaDocumento docResult = new DocsPaVO.documento.SchedaDocumento();
                doc = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(infoUtente, false);

                logger.Debug("Ricerca tipologia documento");
                DocsPaVO.ProfilazioneDinamica.Templates templateDoc = processor.PopolaTemplateDocumento(request.Proceeding.DocumentTypology, infoUtente.idAmministrazione);

                logger.Debug("Popolamento SchedaDocumento");
                doc.oggetto.descrizione = request.Proceeding.DocumentObject;
                doc.oggetto.daAggiornare = true;
                doc.tipoProto = "A";
                doc.protocollo = new DocsPaVO.documento.ProtocolloEntrata();
                string arrivalDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                ((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).mittente = corr;
                ((DocsPaVO.documento.Documento)doc.documenti[0]).dataArrivo = arrivalDate;
                if (templateDoc != null)
                {
                    doc.template = templateDoc;
                    doc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto() { systemId = templateDoc.SYSTEM_ID.ToString(), descrizione = templateDoc.DESCRIZIONE };
                }

                // -----------------------------------------------------
                logger.Debug("Salvataggio scheda");
                docResult = BusinessLogic.Documenti.DocSave.addDocGrigia(doc, infoUtente, role);

                if (docResult != null)
                {
                    bool dummy;
                    // fix per demo 29-08 (si spera di risolvere prima o poi)
                    ((DocsPaVO.documento.Documento)docResult.documenti[0]).dataArrivo = arrivalDate;
                    logger.Debug("Arrival date: " + arrivalDate);

                    docResult.tipoProto = "A";
                    docResult.mezzoSpedizione = BusinessLogic.Documenti.InfoDocManager.getIdMezzoSpedizioneByDesc("PORTALE");
                    docResult.descMezzoSpedizione = "PORTALE";
                    docResult.predisponiProtocollazione = true;
                    docResult = BusinessLogic.Documenti.DocSave.save(infoUtente, docResult, false, out dummy, role);
                }
                // -----------------------------------------------------
                logger.Debug("Associazione mezzo spedizione PORTALE");
                BusinessLogic.Documenti.ProtoManager.collegaMezzoSpedizioneDocumento(infoUtente, docResult.mezzoSpedizione, docResult.docNumber);

                // -----------------------------------------------------
                logger.Debug("Upload del file");
                DocsPaVO.documento.FileRequest fileReq = (DocsPaVO.documento.FileRequest)docResult.documenti[0];
                DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
                //fileDoc.name = request.Proceeding.DocumentObject + ".pdf";
                //fileDoc.fullName = request.Proceeding.DocumentObject + ".pdf";
                fileDoc.name = addressBookCode + ".pdf";
                fileDoc.fullName = addressBookCode + ".pdf";
                fileDoc.content = request.Content;
                fileDoc.contentType = "application/pdf";
                fileDoc.length = request.Content.Length;
                fileDoc.bypassFileContentValidation = true;

                string errorMsg = string.Empty;
                if (!BusinessLogic.Documenti.FileManager.putFile(ref fileReq, fileDoc, infoUtente, out errorMsg))
                {
                    logger.DebugFormat("Errore nel salvataggio del documento! {0}", errorMsg);
                    throw new PisException("FILE_CREATION_ERROR");
                }
                response.IdDocument = docResult.docNumber;
                #endregion

                #region CREAZIONE FASCICOLO (ISTANZA)
                // PER SVILUPPO-----------------------------------------------------------------------------------------------------------------------
                //System.IO.FileStream file = new System.IO.FileStream("C:\\temp\\testportale.pdf", System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //byte[] content = new byte[file.Length];
                //file.Read(content, 0, content.Length);
                //file.Close();
                //request.Content = new byte[content.Length];
                //request.Content = content;
                // -----------------------------------------------------------------------------------------------------------------------------------
                

                logger.Debug("Ricerca tipologia fascicolo");

                DocsPaVO.ProfilazioneDinamica.Templates template = processor.PopolaTemplateIstanzaProcedimenti(request.Proceeding.FolderTypology);
                if (template == null)
                {
                    throw new PisException("TEMPLATE_NOT_FOUND");
                }
                logger.DebugFormat("Trovata tipologia fascicolo {0} - ID={1}", template.DESCRIZIONE, template.SYSTEM_ID.ToString());

                // Ricerca diagramma di stato
                DocsPaVO.DiagrammaStato.DiagrammaStato stateDiagram = null;
                DocsPaVO.DiagrammaStato.Stato initialState = null;
                bool setDiagram = false;
                int idDiagram = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociatoFasc(template.ID_TIPO_FASC);
                if (idDiagram != 0)
                {
                    stateDiagram = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagram.ToString());
                    if (stateDiagram != null)
                    {
                        logger.DebugFormat("Trovato diagramma associato {0} - ID={1}", stateDiagram.DESCRIZIONE, stateDiagram.SYSTEM_ID.ToString());
                        if (stateDiagram.STATI != null && stateDiagram.STATI.Count > 0)
                        {
                            foreach (DocsPaVO.DiagrammaStato.Stato s in stateDiagram.STATI)
                            {
                                if (s.STATO_INIZIALE)
                                {
                                    initialState = s;
                                    break;
                                }
                            }
                        }
                        setDiagram = true;
                    }
                }
                else
                {
                    logger.Debug("Nessun diagramma associato alla tipologia!");
                }

                logger.Debug("Creazione fascicolo per istanza procedimento");
                DocsPaVO.fascicolazione.Fascicolo istanza = new DocsPaVO.fascicolazione.Fascicolo();

                // Prelievo del titolario attivo
                logger.Debug("Prelievo del titolario attivo");
                DocsPaVO.amministrazione.OrgTitolario titolario = null;
                ArrayList titolari = new ArrayList();
                try
                {
                    titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(infoUtente.idAmministrazione);
                }
                catch
                {
                    //Titolario attivo non trovato
                    throw new PisException("CLASSIFICATION_NOT_FOUND");
                }
                if (titolari != null && titolari.Count > 0)
                {
                    foreach (DocsPaVO.amministrazione.OrgTitolario tempTit in titolari)
                    {
                        if (tempTit.Stato == DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo)
                        {
                            titolario = tempTit;
                            break;
                        }
                    }
                }
                else
                {
                    //Titolario attivo non trovato
                    throw new PisException("CLASSIFICATION_NOT_FOUND");
                }

                logger.Debug("Ricerca classifica");
                DocsPaVO.fascicolazione.Classificazione classificazione = Utils.GetClassificazione(infoUtente, request.Proceeding.FolderCode);
                if (classificazione == null || titolario == null)
                {
                    if (classificazione == null)
                    {
                        throw new PisException("GENERAL_NODE_NOT_FOUND");
                    }
                    else
                    {
                        //Titolario non trovati
                        throw new PisException("CLASSIFICATION_NOT_FOUND");
                    }
                }

                // Popolamento oggetto fascicolo
                logger.Debug("Creazione oggetto fascicolo");
                istanza.apertura = DateTime.Now.ToString("dd/MM/yyyy");
                istanza.codiceGerarchia = classificazione.codice;                
                istanza.descrizione = request.Proceeding.Description;
                istanza.idRegistro = classificazione.registro != null ? classificazione.registro.systemId : string.Empty;
                istanza.idTitolario = titolario.ID;
                istanza.privato = "0";
                istanza.codUltimo = BusinessLogic.Fascicoli.FascicoloManager.getFascNumRif(classificazione.systemID, istanza.idRegistro);

                // Tipologia
                istanza.template = template;

                // Salvataggio fascicolo
                logger.Debug("Salvataggio fascicolo");
                DocsPaVO.fascicolazione.ResultCreazioneFascicolo result = DocsPaVO.fascicolazione.ResultCreazioneFascicolo.OK;
                istanza = BusinessLogic.Fascicoli.FascicoloManager.newFascicolo(classificazione, istanza, infoUtente, role, false, out result);

                if (result != DocsPaVO.fascicolazione.ResultCreazioneFascicolo.OK)
                {
                    throw new PisException("ERROR_PROJECT");
                }
                else
                {
                    logger.DebugFormat("Creato fascicolo ID={0}", istanza.systemID);
                    response.IdProject = istanza.systemID;
                    // Impostazione diagramma di stato allo stato iniziale
                    if (setDiagram)
                    {
                        BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStatoFasc(istanza.systemID, initialState.SYSTEM_ID.ToString(), stateDiagram, infoUtente.idPeople, infoUtente, string.Empty);
                    }
                }

                logger.Debug("Inserimento documento in fascicolo");
                if (!BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, docResult.systemId, istanza.systemID, false, out errorMsg))
                {
                    logger.DebugFormat("Errore nell'inserimento del documento in fascicolo - {0}", errorMsg);
                    throw new PisException("");
                }

                logger.Debug("Chiusura fascicolo");
                istanza.stato = "C";
                istanza.chiusura = DateTime.Now.ToString("dd/MM/yyyy");
                istanza.chiudeFascicolo = new DocsPaVO.fascicolazione.ChiudeFascicolo() { idPeople = infoUtente.idPeople, idCorrGlob_Ruolo = infoUtente.idCorrGlobali };
                if (role.uo != null)
                    istanza.chiudeFascicolo.idCorrGlob_UO = role.uo.systemId;
                BusinessLogic.Fascicoli.FascicoloManager.setFascicolo(infoUtente, istanza);


                #endregion

                #region TRASMISSIONE FASCICOLO
                logger.Debug("Trasmissione fascicolo");
                DocsPaVO.trasmissione.Trasmissione transmission = new DocsPaVO.trasmissione.Trasmissione();
                DocsPaVO.trasmissione.RagioneTrasmissione reason = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, "PORTALE"); // CABLATO PER ORA

                transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;
                transmission.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(istanza);
                transmission.utente = user;
                transmission.ruolo = role;

                // Destinatari della trasmissione
                DocsPaVO.utente.Corrispondente recipient = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica("SABAP-RM-MET_SEG-II", infoUtente); // CABLATO PER ORA
                transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(transmission, recipient, reason, string.Empty, "S");

                logger.Debug("Invio trasmissione");
                DocsPaVO.trasmissione.Trasmissione resultTrasm = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(string.Empty, transmission);
                if (resultTrasm != null && resultTrasm.infoFascicolo != null && !string.IsNullOrEmpty(resultTrasm.infoFascicolo.idFascicolo))
                {
                    foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in resultTrasm.trasmissioniSingole)
                    {
                        string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                        string desc = "Trasmesso Fascicolo: " + resultTrasm.infoFascicolo.codice;
                        BusinessLogic.UserLog.UserLog.WriteLog(resultTrasm.utente.userId, resultTrasm.utente.idPeople, resultTrasm.ruolo.idGruppo, infoUtente.idAmministrazione, method, resultTrasm.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                            (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", single.systemId);
                    }
                }

                // Inserimento in ADL Ruolo del ruolo destinatario
                try
                {
                    DocsPaVO.utente.InfoUtente roleRecipient = new DocsPaVO.utente.InfoUtente() { idCorrGlobali = recipient.systemId, idPeople = "0" };
                    BusinessLogic.Documenti.areaLavoroManager.execAddLavoroRoleMethod(string.Empty, string.Empty, string.Empty, roleRecipient, istanza);
                }
                catch (Exception e)
                {
                    logger.DebugFormat("Errore nell'inserimento in ADL ruolo: {0}", e.Message);
                }

                #endregion

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }
            return response;
        }

        /// <summary>
        /// Restituisce il dettaglio di un procedimento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Services.Proceedings.GetProceeding.GetProceedingResponse GetProceedings(Services.Proceedings.GetProceeding.GetProceedingRequest request)
        {
            Services.Proceedings.GetProceeding.GetProceedingResponse response = new Services.Proceedings.GetProceeding.GetProceedingResponse();
            try
            {
                DocsPaVO.utente.Utente user = null;
                DocsPaVO.utente.Ruolo role = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = null;

                infoUtente = Utils.CheckAuthentication(request, "GetProceeding");
                user = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);

                if (user == null)
                {
                    throw new PisException("USER_NO_EXIST");
                }
                role = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                if (role == null)
                {
                    throw new PisException("ROLE_NO_EXIST");
                }

                #region FASCICOLO
                logger.Debug("Estrazione dati fascicolo");
                Domain.Project responseProject = new Project();
                DocsPaVO.fascicolazione.Fascicolo project = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdProject, infoUtente);

                if (project == null)
                {
                    throw new PisException("PROJECT_NOT_FOUND");
                }

                responseProject = Utils.GetProject(project, infoUtente);

                if (responseProject.Template != null)
                {
                    foreach (Domain.Field field in responseProject.Template.Fields)
                    {
                        if (field.Type == "Correspondent")
                        {
                            DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(field.Value);
                            if (corr != null)
                            {
                                field.Value = corr.descrizione;
                            }
                        }
                    }
                }

                if (responseProject != null)
                {
                    response.Proceeding = responseProject;
                }
                else
                {
                    throw new PisException("PROJECT_NOT_FOUND");
                }
                #endregion

                #region FASI
                logger.Debug("Estrazione fasi diagramma di stato");
                
                // Estrazione diagramma di stato
                logger.Debug("Estrazione diagramma di stato");
                DocsPaVO.DiagrammaStato.DiagrammaStato diagram = BusinessLogic.DiagrammiStato.DiagrammiStato.getDgByIdTipoFasc(project.template.ID_TIPO_FASC, infoUtente.idAmministrazione);

                // Stato del fascicolo
                logger.Debug("Stato del fascicolo");
                DocsPaVO.DiagrammaStato.Stato state = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoFasc(request.IdProject);

                // Lista associazioni fasi-stati
                if (diagram != null && state != null)
                {
                    logger.Debug("Associazione fasi-stati");
                    List<DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma> assPhasesDiagram = BusinessLogic.DiagrammiStato.DiagrammiStato.GetFasiStatiDiagramma(diagram.SYSTEM_ID.ToString(), infoUtente);

                    if (assPhasesDiagram != null && assPhasesDiagram.Count > 0)
                    {
                        List<Domain.Phase> phases = new List<Phase>();

                        foreach (DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma item in assPhasesDiagram)
                        {
                            Phase ph = new Phase();
                            ph.Description = item.PHASE.DESCRIZIONE;
                            ph.Selected = (item.STATO.SYSTEM_ID == state.SYSTEM_ID);

                            // Se sono presenti delle fasi nella lista devo fare attenzione a non inserire duplicati
                            if (phases.Count > 0)
                            {
                                if (phases.Last().Description != ph.Description)
                                {
                                    phases.Add(ph);
                                }
                                else
                                {
                                    if (ph.Selected)
                                    {
                                        phases.Last().Selected = true;
                                    }
                                }
                            }
                            else
                            {
                                phases.Add(ph);
                            }
                         }

                        response.Phases = phases.ToArray();
                    }
                }
                #endregion

                #region DOCUMENTI
                logger.Debug("Estrazione lista documenti fascicolo");
                List<Domain.Document> responseDocuments = new List<Document>();

                // Folder contenuti nel fascicolo
                logger.Debug("Lista folder");
                ArrayList folders = BusinessLogic.Fascicoli.FascicoloManager.getListaFolderDaIdFascicolo(infoUtente, request.IdProject, null, false, false);
                int numPage = 0;
                int numTotPage = 0;
                int numRec = 0;
                int pageSize = 100;

                // Lista dei fitri da restituire
                    DocsPaVO.filtri.FiltroRicerca[][] orderRicerca = new DocsPaVO.filtri.FiltroRicerca[1][];
                    orderRicerca = new DocsPaVO.filtri.FiltroRicerca[1][];
                    orderRicerca[0] = new DocsPaVO.filtri.FiltroRicerca[1];
                    DocsPaVO.filtri.FiltroRicerca[] fVlist = new DocsPaVO.filtri.FiltroRicerca[3];
                    fVlist[0] = new DocsPaVO.filtri.FiltroRicerca()
                    {
                        argomento = "ORACLE_FIELD_FOR_ORDER",
                        valore = "NVL (a.dta_proto, a.creation_time)",
                        nomeCampo = "D9",
                    };
                    fVlist[1] = new DocsPaVO.filtri.FiltroRicerca()
                    {
                        argomento = "SQL_FIELD_FOR_ORDER",
                        valore = "ISNULL (a.dta_proto, a.creation_time)",
                        nomeCampo = "D9",
                    };
                    fVlist[2] = new DocsPaVO.filtri.FiltroRicerca()
                    {
                        argomento = "ORDER_DIRECTION",
                        valore = "DESC",
                        nomeCampo = "D9",
                    };

                    orderRicerca[0] = fVlist;

                foreach (DocsPaVO.fascicolazione.Folder folder in folders)
                {
                    List<DocsPaVO.ricerche.SearchResultInfo> searchResultInfo = new List<DocsPaVO.ricerche.SearchResultInfo>();
                    ArrayList docInFolder = BusinessLogic.Fascicoli.FolderManager.getDocumentiPagingCustom(infoUtente, folder, null, numPage, out numTotPage, out numRec, true,
                        out searchResultInfo, false, true, null, null, pageSize, orderRicerca);

                    if (docInFolder != null && docInFolder.Count > 0)
                    {
                        foreach (DocsPaVO.Grids.SearchObject item in docInFolder)
                        {
                            Domain.Document doc = Utils.GetDocumentFromSearchObject(item, false, null, request.CodeAdm);

                            // Estrazione mittente/destinatario
                            string senderRecipient = item.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D5")).FirstOrDefault().SearchObjectFieldValue;
                            if (senderRecipient.EndsWith("(M)"))
                            {
                                string corr = senderRecipient.Substring(0, senderRecipient.Length - 4);
                                doc.Sender = new Correspondent() { Description = corr };
                            }
                            else if (senderRecipient.EndsWith("(D)"))
                            {
                                string corr = senderRecipient.Substring(0, senderRecipient.Length - 4);
                                doc.Sender = new Correspondent() { Description = corr };
                            }
                            if ((doc.DocumentType == "A" || doc.DocumentType == "P") && !string.IsNullOrEmpty(doc.Signature))
                            {
                                doc.ProtocolDate = doc.CreationDate;
                                doc.CreationDate = string.Empty;
                            }

                            ArrayList attachments = BusinessLogic.Documenti.AllegatiManager.getAllegati(doc.DocNumber, string.Empty);
                            if (attachments != null && attachments.Count > 0)
                            {
                                doc.Attachments = new File[attachments.Count];
                                int i = 0;
                                foreach (DocsPaVO.documento.FileRequest fr in attachments)
                                {
                                    doc.Attachments[i] = Utils.GetFile(fr, false, infoUtente, false, false, string.Empty, null, true);
                                    i++;
                                }
                            }

                            if (!responseDocuments.Contains(doc))
                            {
                                responseDocuments.Add(doc);
                            }
                        }
                    }
                }
                response.Documents = responseDocuments.ToArray();
                #endregion

                response.Success = true;

            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }
    }
}