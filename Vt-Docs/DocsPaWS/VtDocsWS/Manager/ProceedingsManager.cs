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

                if (request.Proceeding.Content == null || request.Proceeding.Content.Length == 0)
                {
                    throw new PisException("MISSING_FILE_CONTENT");
                }

                BusinessLogic.Modelli.AsposeModelProcessor.PdfModelProcessor processor = new BusinessLogic.Modelli.AsposeModelProcessor.PdfModelProcessor(request.Proceeding.Content);

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
                    if (corr != null)
                    {
                        List<DocsPaVO.utente.MailCorrispondente> mbox = new List<DocsPaVO.utente.MailCorrispondente>();
                        mbox.Add(new DocsPaVO.utente.MailCorrispondente()
                        {
                            Email = corr.email,
                            Note = string.Empty,
                            Principale = "1"
                        });
                        if (!BusinessLogic.Utenti.addressBookManager.InsertMailCorrispondente(mbox, corr.systemId))
                        {
                            throw new PisException("APPLICATION_ERROR");
                        }
                    }
                    else
                    {
                        throw new PisException("APPLICATION_ERROR");
                    }


                    logger.Debug("Inserito nuovo corrispondente " + corr.codiceRubrica);
                }
                #endregion

                #region CREAZIONE PROCEDIMENTO
                // Controllo esistenza procedimento nel sistema
                DocsPaVO.Procedimento.Procedimento proceeding = BusinessLogic.Procedimenti.ProcedimentiManager.GetProcedimentoByIdEsterno(request.Proceeding.Id);
                if (proceeding != null && !string.IsNullOrEmpty(proceeding.Id) && proceeding.Id != "0")
                {
                    // Il procedimento esiste già nel sistema
                    response.IdProject = proceeding.Id;
                    response.IdDocument = proceeding.Documenti[0].Id;
                    response.Success = true;
                }
                else
                {
                    using (DocsPaDB.TransactionContext trContx = new DocsPaDB.TransactionContext())
                    {
                        try
                        {
                            #region CREAZIONE DOCUMENTO - OLD
                            //DocsPaVO.documento.SchedaDocumento doc = new DocsPaVO.documento.SchedaDocumento();
                            //DocsPaVO.documento.SchedaDocumento docResult = new DocsPaVO.documento.SchedaDocumento();
                            //doc = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(infoUtente, false);

                            //logger.Debug("Registro");
                            //DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeAdm);

                            //logger.Debug("Ricerca tipologia documento");
                            //DocsPaVO.ProfilazioneDinamica.Templates templateDoc = processor.PopolaTemplateDocumento(request.Proceeding.DocumentTypology, infoUtente.idAmministrazione);

                            //logger.Debug("Popolamento SchedaDocumento");
                            //doc.oggetto.descrizione = request.Proceeding.DocumentObject;
                            //doc.oggetto.daAggiornare = true;
                            //doc.tipoProto = "A";
                            //doc.protocollo = new DocsPaVO.documento.ProtocolloEntrata();
                            //string arrivalDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                            //((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).mittente = corr;
                            //((DocsPaVO.documento.Documento)doc.documenti[0]).dataArrivo = arrivalDate;
                            //if (templateDoc != null)
                            //{
                            //    doc.template = templateDoc;
                            //    doc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto() { systemId = templateDoc.SYSTEM_ID.ToString(), descrizione = templateDoc.DESCRIZIONE };
                            //}

                            //// -----------------------------------------------------
                            //logger.Debug("Salvataggio scheda");
                            //docResult = BusinessLogic.Documenti.DocSave.addDocGrigia(doc, infoUtente, role);

                            //if (docResult != null)
                            //{
                            //    bool dummy;
                            //    // fix per demo 29-08 (si spera di risolvere prima o poi)
                            //    ((DocsPaVO.documento.Documento)docResult.documenti[0]).dataArrivo = arrivalDate;
                            //    logger.Debug("Arrival date: " + arrivalDate);

                            //    if (reg != null)
                            //        docResult.registro = reg;
                            //    docResult.tipoProto = "A";
                            //    docResult.mezzoSpedizione = BusinessLogic.Documenti.InfoDocManager.getIdMezzoSpedizioneByDesc("PORTALE");
                            //    docResult.descMezzoSpedizione = "PORTALE";
                            //    docResult.predisponiProtocollazione = true;
                            //    docResult = BusinessLogic.Documenti.DocSave.save(infoUtente, docResult, false, out dummy, role);
                            //}
                            //else
                            //{
                            //    throw new PisException("APPLICATION_ERROR");
                            //}

                            //// -----------------------------------------------------
                            //logger.Debug("Associazione mezzo spedizione PORTALE");
                            //BusinessLogic.Documenti.ProtoManager.collegaMezzoSpedizioneDocumento(infoUtente, docResult.mezzoSpedizione, docResult.docNumber);

                            //// -----------------------------------------------------
                            //logger.Debug("Upload del file");
                            //DocsPaVO.documento.FileRequest fileReq = (DocsPaVO.documento.FileRequest)docResult.documenti[0];
                            //DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
                            ////fileDoc.name = request.Proceeding.DocumentObject + ".pdf";
                            ////fileDoc.fullName = request.Proceeding.DocumentObject + ".pdf";
                            //fileDoc.name = addressBookCode + ".pdf";
                            //fileDoc.fullName = addressBookCode + ".pdf";
                            //fileDoc.content = request.Proceeding.Content;
                            //fileDoc.contentType = "application/pdf";
                            //fileDoc.length = request.Proceeding.Content.Length;
                            //fileDoc.bypassFileContentValidation = true;

                            //// -----------------------------------------------------
                            //logger.Debug("Aggiunta allegati");
                            //if (request.Proceeding.Attachment != null && request.Proceeding.Attachment.Count() > 0)
                            //{
                            //    logger.DebugFormat("{0} allegati", request.Proceeding.Attachment.Count());
                            //    foreach (Domain.File reqAttachment in request.Proceeding.Attachment)
                            //    {
                            //        DocsPaVO.documento.Allegato attachment = new DocsPaVO.documento.Allegato();
                            //        attachment.docNumber = docResult.docNumber;
                            //        attachment.descrizione = reqAttachment.Description;
                            //        attachment.fileName = reqAttachment.Name;
                            //        attachment.version = "0";
                            //        attachment.numeroPagine = 1;

                            //        DocsPaVO.documento.FileDocumento fdAttachment = new DocsPaVO.documento.FileDocumento();
                            //        fdAttachment.name = reqAttachment.Name;
                            //        fdAttachment.fullName = reqAttachment.Name;
                            //        fdAttachment.nomeOriginale = reqAttachment.Name;
                            //        fdAttachment.content = reqAttachment.Content;
                            //        fdAttachment.contentType = string.IsNullOrEmpty(reqAttachment.MimeType) ? "application/pdf" : reqAttachment.MimeType;
                            //        fdAttachment.estensioneFile = reqAttachment.Name.Split('.').Last();

                            //        DocsPaVO.documento.Allegato attResult = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, attachment);
                            //        if (attResult != null)
                            //        {
                            //            BusinessLogic.Documenti.FileManager.putFile(attResult, fdAttachment, infoUtente);
                            //        }

                            //    }
                            //}

                            //string errorMsg = string.Empty;
                            //if (!BusinessLogic.Documenti.FileManager.putFile(ref fileReq, fileDoc, infoUtente, out errorMsg))
                            //{
                            //    logger.DebugFormat("Errore nel salvataggio del documento! {0}", errorMsg);
                            //    throw new PisException("FILE_CREATION_ERROR");
                            //}
                            //response.IdDocument = docResult.docNumber;
                            #endregion

                            #region CREAZIONE DOCUMENTO
                            logger.Debug("Ricerca tipologia documento");
                            DocsPaVO.ProfilazioneDinamica.Templates templateDoc = processor.PopolaTemplateDocumento(request.Proceeding.IdDocumentTypology.ToString());

                            logger.Debug("Registro");
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeAdm);

                            logger.Debug("Creazione documento");
                            string docNumber = Utils.CreateDocProceeding(infoUtente, reg.codRegistro, request.Proceeding.DocumentObject, templateDoc, corr, role, addressBookCode, request.Proceeding.Content, request.Proceeding.Attachment);

                            if (string.IsNullOrEmpty(docNumber))
                            {
                                throw new PisException("APPLICATION_ERROR");
                            }

                            response.IdDocument = docNumber;
                            #endregion

                            #region CREAZIONE FASCICOLO (ISTANZA)
                            logger.Debug("Ricerca tipologia fascicolo");

                            DocsPaVO.ProfilazioneDinamica.Templates template = processor.PopolaTemplateIstanzaProcedimenti(request.Proceeding.IdFolderTypology.ToString());
                            if (template == null)
                            {
                                throw new PisException("TEMPLATE_NOT_FOUND");
                            }
                            logger.DebugFormat("Trovata tipologia fascicolo {0} - ID={1}", template.DESCRIZIONE, template.SYSTEM_ID.ToString());
                            
                            // Popolamento campi
                            BusinessLogic.Procedimenti.ProcedimentiManager.PopolaCampiProcedimento(ref template, infoUtente);

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
                                // Inserimento in DPA_PROCEDIMENTI
                                if (!BusinessLogic.Procedimenti.ProcedimentiManager.InsertDoc(istanza.systemID, docNumber, corr.systemId, true, request.Proceeding.Id, true))
                                {
                                    throw new PisException("APPLICATION_ERROR");
                                }

                                logger.DebugFormat("Creato fascicolo ID={0}", istanza.systemID);
                                response.IdProject = istanza.systemID;
                                // Impostazione diagramma di stato allo stato iniziale
                                if (setDiagram)
                                {
                                    BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStatoFasc(istanza.systemID, initialState.SYSTEM_ID.ToString(), stateDiagram, infoUtente.idPeople, infoUtente, string.Empty);
                                }
                            }

                            logger.Debug("Inserimento documento in fascicolo");
                            string errorMsg = string.Empty;
                            if (!BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, docNumber, istanza.systemID, false, out errorMsg))
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

                            trContx.Complete();
                        }
                        catch (PisException pisEx1)
                        {
                            throw pisEx1;
                        }
                        catch (Exception ex1)
                        {
                            throw ex1;
                        }
                    }
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

                DocsPaVO.Procedimento.Procedimento proceeding = BusinessLogic.Procedimenti.ProcedimentiManager.GetProcedimentoByIdFascicolo(request.IdProject);
                if (proceeding == null)
                {
                    throw new Exception("PROJECT_NOT_FOUND");
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

                logger.Debug("Stato di approvazione");
                response.Status = 0;
                DocsPaVO.Procedimento.EsitoProcedimento status = BusinessLogic.Procedimenti.ProcedimentiManager.GetEsitoProcedimento(request.IdProject);
                if (status != null)
                {
                    if (status == DocsPaVO.Procedimento.EsitoProcedimento.Positivo)
                    {
                        response.Status = 1;
                    }
                    if (status == DocsPaVO.Procedimento.EsitoProcedimento.Negativo)
                    {
                        response.Status = -1;
                    }
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
                List<Domain.DocInProceeding> responseDocuments = new List<DocInProceeding>();

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
                            DocsPaVO.Procedimento.DocumentoProcedimento dip = proceeding.Documenti.Where(d => d.Id == item.SearchObjectID).FirstOrDefault();

                                if (dip != null && dip.Id == item.SearchObjectID)
                                {

                                    Domain.Document doc = Utils.GetDocumentFromSearchObject(item, false, null, request.CodeAdm);

                                    // Estrazione mittente/destinatario
                                    //string senderRecipient = item.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D5")).FirstOrDefault().SearchObjectFieldValue;
                                    //if (senderRecipient.EndsWith("(M)"))
                                    //{
                                    //    string corr = senderRecipient.Substring(0, senderRecipient.Length - 4);
                                    //    doc.Sender = new Correspondent() { Description = corr };
                                    //}
                                    //else if (senderRecipient.EndsWith("(D)"))
                                    //{
                                    //    string corr = senderRecipient.Substring(0, senderRecipient.Length - 4);
                                    //    doc.Sender = new Correspondent() { Description = corr };
                                    //}
                                    // Estrazione mittente
                                    string sender = item.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D6")).FirstOrDefault().SearchObjectFieldValue;
                                    if (sender.EndsWith("(M)"))
                                    {
                                        string corr = sender.Substring(0, sender.Length - 4);
                                        doc.Sender = new Correspondent() { Description = corr };
                                    }

                                    if ((doc.DocumentType == "A" || doc.DocumentType == "P") && !string.IsNullOrEmpty(doc.Signature))
                                    {
                                        doc.ProtocolDate = doc.CreationDate;
                                        doc.CreationDate = string.Empty;
                                    }

                                    List<DocsPaVO.documento.Allegato> attachments = BusinessLogic.Documenti.AllegatiManager.getAllegati(doc.DocNumber, "all").Cast<DocsPaVO.documento.Allegato>().ToList();

                                    if (attachments != null && attachments.Count > 0)
                                    {
                                        List<DocsPaVO.documento.Allegato> filteredAttachments = (from a in attachments where a.TypeAttachment == 1 select a).ToList();

                                        if (filteredAttachments != null && filteredAttachments.Count > 0)
                                        {
                                            doc.Attachments = new File[filteredAttachments.Count];
                                            int i = 0;
                                            foreach (DocsPaVO.documento.FileRequest fr in attachments)
                                            {
                                                doc.Attachments[i] = Utils.GetFile(fr, false, infoUtente, false, false, string.Empty, null, true);
                                                i++;
                                            }
                                        }
                                    }

                                    // Stato visualizzazione
                                    Domain.DocInProceeding docInProc = Utils.GetDocInProceeding(doc);
                                    docInProc.Viewed = BusinessLogic.Procedimenti.ProcedimentiManager.IsDocVisualizzato(request.IdProject, doc.DocNumber);
                                    docInProc.ViewedOn = !string.IsNullOrEmpty(dip.DataVisualizzazione) ? dip.DataVisualizzazione : string.Empty;

                                    if (!responseDocuments.Contains(docInProc))
                                    {
                                        responseDocuments.Add(docInProc);
                                    }
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

        /// <summary>
        /// Aggiunta documento ad un procedimento esistente
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Services.Proceedings.AddDocToProceeding.AddDocToProceedingResponse AddDocToProceeding(Services.Proceedings.AddDocToProceeding.AddDocToProceedingRequest request)
        {
            Services.Proceedings.AddDocToProceeding.AddDocToProceedingResponse response = new Services.Proceedings.AddDocToProceeding.AddDocToProceedingResponse();

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
                if (request.User == null || (string.IsNullOrEmpty(request.User.NationalIdentificationNumber) && string.IsNullOrEmpty(request.User.VatNumber)))
                {
                    throw new PisException("MISSING_USER");
                }

                if (request.Content == null || request.Content.Length == 0)
                {
                    throw new PisException("MISSING_FILE_CONTENT");
                }

                logger.Debug("Estrazione dati fascicolo");
                DocsPaVO.fascicolazione.Fascicolo proj = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdProceeding, infoUtente);
                if (proj == null)
                {
                    throw new PisException("PROJECT_NOT_FOUND");
                }

                DocsPaVO.Procedimento.Procedimento proceeding = BusinessLogic.Procedimenti.ProcedimentiManager.GetProcedimentoByIdFascicolo(request.IdProceeding);
                #endregion

                #region CONTROLLO ESISTENZA CORRISPONDENTE
                logger.Debug("Controllo esistenza utente in rubrica");
                string addressBookCode = !string.IsNullOrEmpty(request.User.VatNumber) ? request.User.VatNumber : request.User.NationalIdentificationNumber;
                logger.Debug("Codice rubrica: " + addressBookCode);

                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(addressBookCode, infoUtente);
                if (corr == null)
                {
                    throw new PisException("CORRESPONDENT_NOT_FOUND");
                }
                #endregion

                using (DocsPaDB.TransactionContext trContext = new DocsPaDB.TransactionContext())
                {
                    try
                    {
                        #region CREAZIONE DOCUMENTO
                        BusinessLogic.Modelli.AsposeModelProcessor.PdfModelProcessor processor = new BusinessLogic.Modelli.AsposeModelProcessor.PdfModelProcessor(request.Content);

                        logger.Debug("Ricerca tipologia documento");
                        DocsPaVO.ProfilazioneDinamica.Templates templateDoc = processor.PopolaTemplateDocumento(request.IdDocumentTypology.ToString());

                        string docNumber = Utils.CreateDocProceeding(infoUtente, request.CodeAdm, request.DocumentObject, templateDoc, corr, role, addressBookCode, request.Content, request.Attachment);

                        if (string.IsNullOrEmpty(docNumber))
                        {
                            throw new PisException("APPLICATION_ERROR");
                        }
                        #endregion

                        #region INSERIMENTO IN FASCICOLO

                        string uploadMsg = string.Empty;
                        bool uploadResult = BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, docNumber, request.IdProceeding, false, out uploadMsg);

                        if (!uploadResult)
                        {
                            throw new Exception(uploadMsg);
                        }

                        // Inserimento in DPA_PROCEDIMENTI
                        if (!BusinessLogic.Procedimenti.ProcedimentiManager.InsertDoc(request.IdProceeding, docNumber, corr.systemId, true, proceeding.IdEsterno, true))
                        {
                            throw new PisException("APPLICATION_ERROR");
                        }

                        #endregion

                        #region TRASMISSIONE DOCUMENTO
                        logger.Debug("Trasmissione documento");
                        DocsPaVO.trasmissione.Trasmissione transmission = new DocsPaVO.trasmissione.Trasmissione();
                        DocsPaVO.trasmissione.RagioneTrasmissione reason = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, "PORTALE"); // CABLATO PER ORA

                        transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                        transmission.utente = user;
                        transmission.ruolo = role;
                        transmission.infoDocumento = new DocsPaVO.documento.InfoDocumento()
                        {
                            idProfile = docNumber,
                            docNumber = docNumber,
                            oggetto = request.DocumentObject,
                            tipoProto = "A",
                            tipoAtto = request.IdDocumentTypology.ToString()
                        };

                        // Destinatari della trasmissione

                        // 1 - ruolo a cui trasmetto il fascicolo (cablato)
                        DocsPaVO.utente.Corrispondente recipient = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica("SABAP-RM-MET_SEG-II", infoUtente); // CABLATO PER ORA
                        transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(transmission, recipient, reason, string.Empty, "S");
                        
                        // 2 - utente responsabile del procedimento (se definito)
                        string assigneeID = string.Empty;
                        DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascDettagli(request.IdProceeding);
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom obj in template.ELENCO_OGGETTI)
                        {
                            if (obj.DESCRIZIONE.ToUpper() == "UTENTE ASSEGNATARIO")
                            {
                                assigneeID = obj.VALORE_DATABASE;
                                break;
                            }
                        }
                        if (!string.IsNullOrEmpty(assigneeID))
                        {
                            DocsPaVO.utente.Corrispondente assignee = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(assigneeID);
                            if (assignee != null)
                            {
                                transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(transmission, assignee, reason, string.Empty, "S");
                            }
                        }
                        

                        logger.Debug("Invio trasmissione");
                        DocsPaVO.trasmissione.Trasmissione resultTrasm = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(string.Empty, transmission);

                        if (resultTrasm != null && resultTrasm.infoDocumento != null && !string.IsNullOrEmpty(resultTrasm.infoDocumento.docNumber))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in resultTrasm.trasmissioniSingole)
                            {
                                string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                string desc = "Trasmesso Documento: " + resultTrasm.infoDocumento.docNumber;
                                BusinessLogic.UserLog.UserLog.WriteLog(resultTrasm.utente.userId, resultTrasm.utente.idPeople, resultTrasm.ruolo.idGruppo, infoUtente.idAmministrazione, method, resultTrasm.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                                    (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", single.systemId);
                            }
                        }
                        #endregion

                        #region CONTROLLO EVENTI PER CAMBIO STATO
                        if (templateDoc != null)
                        {
                            BusinessLogic.Procedimenti.ProcedimentiManager.CambioStatoProcedimento(request.IdProceeding, "RICEZIONE", templateDoc.ID_TIPO_ATTO, infoUtente);
                        }
                        #endregion

                        response.IdDocument = docNumber;
                        trContext.Complete();
                    }
                    catch (PisException pisEx1)
                    {
                        throw pisEx1;
                    }
                    catch (Exception ex1)
                    {
                        throw ex1;
                    }
                }

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
        /// Restituisce la lista dei documenti non visualizzati da un utente
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Services.Proceedings.GetUnreadNotifications.GetUnreadNotificationsResponse GetUnreadNotifications(Services.Proceedings.GetUnreadNotifications.GetUnreadNotificationsRequest request)
        {
            Services.Proceedings.GetUnreadNotifications.GetUnreadNotificationsResponse response = new Services.Proceedings.GetUnreadNotifications.GetUnreadNotificationsResponse();

            try
            {
                #region AUTENTICAZIONE E VALIDAZIONE PARAMETRI
                DocsPaVO.utente.Utente user = null;
                DocsPaVO.utente.Ruolo role = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = null;

                infoUtente = Utils.CheckAuthentication(request, "GetUnreadNotifications");
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

                if (string.IsNullOrEmpty(request.CodeCorrespondent))
                {
                    throw new PisException("MISSING_USER");
                }
                #endregion

                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.CodeCorrespondent, infoUtente);
                if (corr == null)
                {
                    // Gestione caso corrispondente non presente in rubrica
                    // Si tratta di utenti che non hanno mai creato procedimenti
                    response.Proceedings = new Proceeding[0];
                }
                else
                {
                    List<DocsPaVO.Procedimento.Procedimento> list = BusinessLogic.Procedimenti.ProcedimentiManager.GetProcedimentiNonVisualizzati(corr.systemId);
                    if (list == null)
                    {
                        throw new PisException("APPLICATION_ERROR");
                    }
                    else
                    {
                        response.Proceedings = new Proceeding[list.Count];
                        for (int i = 0; i < list.Count; i++)
                        {
                            response.Proceedings[i] = new Domain.Proceeding() { Id = list[i].IdEsterno, Description = list[i].Descrizione,  UnreadDocuments = list[i].Documenti.Count() };
                        }
                    }
                }
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
        /// Aggiorna lo stato di visualizzazione di un documento nel procedimento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Services.Proceedings.SetReadNotifications.SetReadNotificationsResponse SetReadNotifications(Services.Proceedings.SetReadNotifications.SetReadNotificationsRequest request)
        {
            Services.Proceedings.SetReadNotifications.SetReadNotificationsResponse response = new Services.Proceedings.SetReadNotifications.SetReadNotificationsResponse();

            try
            {
                #region AUTENTICAZIONE E VALIDAZIONE PARAMETRI
                DocsPaVO.utente.Utente user = null;
                DocsPaVO.utente.Ruolo role = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = null;

                infoUtente = Utils.CheckAuthentication(request, "SetReadNotifications");
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

                DocsPaVO.Procedimento.Procedimento proceeding = BusinessLogic.Procedimenti.ProcedimentiManager.GetProcedimentoByIdFascicolo(request.IdProceeding);
                if (proceeding == null || string.IsNullOrEmpty(proceeding.Id))
                {
                    throw new PisException("PROJECT_NOT_FOUND");
                }
                if (proceeding.Documenti.Where(p => p.Id == request.IdDoc).FirstOrDefault() == null)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }
                #endregion

                using(DocsPaDB.TransactionContext trContxt = new DocsPaDB.TransactionContext())
                {
                    try
                    {
                        #region AGGIORNAMENTO STATO
                        if (!BusinessLogic.Procedimenti.ProcedimentiManager.UpdateStato(request.IdDoc, request.IdProceeding))
                        {
                            throw new PisException("APPLICATION_ERROR");
                        }
                        #endregion

                        #region CONTROLLO AUTOMATISMI
                        DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplate(request.IdDoc);
                        if (template != null)
                        {
                            BusinessLogic.Procedimenti.ProcedimentiManager.CambioStatoProcedimento(request.IdProceeding, "PRESA_VISIONE", template.SYSTEM_ID.ToString(), infoUtente);
                        }
                        #endregion

                        #region CREAZIONE RICEVUTA DI PRESA VISIONE
                        DocsPaVO.documento.FileDocumento receipt = BusinessLogic.Modelli.StampaRicevutaGenerica.Create(infoUtente, request.IdDoc, BusinessLogic.Modelli.TipoRicevuta.PresaVisione);
                        if (receipt != null)
                        {
                            DocsPaVO.documento.Allegato attachment = new DocsPaVO.documento.Allegato();
                            attachment.docNumber = request.IdDoc;
                            attachment.descrizione = "Ricevuta di presa visione";
                            attachment.fileName = receipt.nomeOriginale;
                            attachment.version = "0";
                            attachment.numeroPagine = 1;

                            DocsPaVO.documento.Allegato attResult = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegatoPEC(infoUtente, attachment);
                            if (attResult != null)
                            {
                                BusinessLogic.Documenti.FileManager.putFile(attResult, receipt, infoUtente);
                            }

                        }
                        else
                        {
                            throw new PisException("APPLICATION_ERROR");
                        }
                        #endregion

                        #region CONSOLIDAMENTO
                        DocsPaVO.documento.DocumentConsolidationStateInfo freezeDocInfo = BusinessLogic.Procedimenti.ProcedimentiManager.ConsolidateNoSecurity(infoUtente, request.IdDoc, DocsPaVO.documento.DocumentConsolidationStateEnum.Step2, true);
                        #endregion

                        trContxt.Complete();
                        response.Success = true;
                    }
                    catch (Exception ex1)
                    {
                        throw ex1;
                    }
                }

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