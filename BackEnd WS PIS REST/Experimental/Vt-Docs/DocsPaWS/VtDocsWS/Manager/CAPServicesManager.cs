using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Collections;
using VtDocsWS.WebServices;
using VtDocsWS.Domain;
using log4net;
using DocsPaVO.utente;

namespace VtDocsWS.Manager
{
    public class CAPServicesManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(CAPServicesManager));

        public static Services.CAPServices.StoreOpportunity.StoreOpportunityResponse StoreOpportunity(Services.CAPServices.StoreOpportunity.StoreOpportunityRequest request)
        {
            Services.CAPServices.StoreOpportunity.StoreOpportunityResponse response = new Services.CAPServices.StoreOpportunity.StoreOpportunityResponse();

            try
            {
                #region autenticazione e prelievo informazioni
                // Procedura di autenticazione. Tramite token o username?
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                bool refactoring = false;
                string idOpportunity = "", oggettoFasc = "", idTemplate = "", nodoClassifica = "SALE.OPP", idTemplateDoc = "";
                string soloModCampi = "";
                string authorizerOppData = "", proposersOppData = "", reviewsOppData = "",
                    authorizerTemplate = "", proposerTemplate = "", reviewsTemplate = "",
                    authorizerNotFound = "", proposerNotFound = "", reviewsNotFound = "",
                    authorizerRagTrasm = "", proposerRagTrasm = "", reviewsRagTrasm = "",
                    idOggOppId = "", valoreOppId = "", idPrjOppRefactor = "", notaRolesNotFound = "", notaRevsNotFound = "", missingRoles = "";
                DocsPaVO.ProfilazioneDinamica.Templates template = null;
                DocsPaVO.ProfilazioneDinamica.Templates templateDoc = null;


                bool setStatoDiagrammaIniziale = false;
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                DocsPaVO.DiagrammaStato.Stato statoIniziale = null;
                logger.Debug("Variabili inizializzate");
                string authInfoString = Utils.Decrypt(request.AuthenticationToken.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                logger.Debug("Prelievo utenza");
                string splitcharstr = "_";

                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(authInfoArray[4], "BE_CAP_TR_REASON_SPLITCHAR")))
                {
                    splitcharstr = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(authInfoArray[4], "BE_CAP_TR_REASON_SPLITCHAR");

                }


                string authorizerKey = "AUTHORIZER";
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(authInfoArray[4], "BE_CAP_AUTHORIZER_FIELD")))
                {
                    authorizerKey = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(authInfoArray[4], "BE_CAP_AUTHORIZER_FIELD");
                }
                string proposersKey = "PROPOSERS";
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(authInfoArray[4], "BE_CAP_PROPOSERS_FIELD")))
                {
                    proposersKey = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(authInfoArray[4], "BE_CAP_PROPOSERS_FIELD");
                }
                string reviewsKey = "REVIEWS";
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(authInfoArray[4], "BE_CAP_REVIEWS_FIELD")))
                {
                    reviewsKey = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(authInfoArray[4], "BE_CAP_REVIEWS_FIELD");
                }
                string missingRolesKey = "MISSING ROLES";
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(authInfoArray[4], "BE_CAP_MISSING_ROLES_FIELD")))
                {
                    missingRolesKey = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(authInfoArray[4], "BE_CAP_MISSING_ROLES_FIELD");
                }

                foreach (OpportunityData opp1 in request.OppDatas)
                {
                    //if (opp1.DataName.ToUpper() == "OPPORTUNITY COUNTRY")
                    //    idTemplate = "Opportunity " + opp1.DataValue;
                    //if (opp1.DataName.ToUpper() == "AUTHORIZER")
                    //    authorizerOppData = opp1.DataValue;
                    //if (opp1.DataName.ToUpper() == "REVIEWS")
                    //    reviewsOppData = opp1.DataValue;
                    //if (opp1.DataName.ToUpper() == "PROPOSERS")
                    //    proposersOppData = opp1.DataValue;
                    switch (opp1.DataName.ToUpper())
                    {
                        case ("OPPORTUNITY COUNTRY"):
                            idTemplate = "Opportunity " + opp1.DataValue;
                            idTemplateDoc = "CAP Presentation " + opp1.DataValue;
                            break;
                        case ("OPPORTUNITY NAME"):
                            oggettoFasc = opp1.DataValue;
                            break;
                        case("NOT_REWORKING"):
                            soloModCampi = opp1.DataValue;
                            break;
                        //case ("AUTHORIZER"):
                        //    authorizerOppData = opp1.DataValue;
                        //    break;
                        //case ("REVIEWS"):
                        //    reviewsOppData = opp1.DataValue;
                        //    break;
                        //case ("PROPOSERS"):
                        //    proposersOppData = opp1.DataValue;
                        //    break;
                    }

                    if (opp1.DataName.ToUpper() == authorizerKey.ToUpper()) authorizerOppData = opp1.DataValue;
                    if (opp1.DataName.ToUpper() == proposersKey.ToUpper()) proposersOppData = opp1.DataValue;
                    if (opp1.DataName.ToUpper() == reviewsKey.ToUpper()) reviewsOppData = opp1.DataValue;

                }

                logger.Debug("Prelievo campi country, name, authorizer, reviews,proposer");
                // Estrazione corrispondenti
                ArrayList authCorrs = null;
                ArrayList propCorrs = null;
                ArrayList revCorrs = null;
                OpportunityData oppDataRes = null;
                ArrayList oppDatasRes = new ArrayList();
                authCorrs = Utils.estraiCorrispondenti(authorizerOppData, infoUtente, out authorizerTemplate, out authorizerNotFound, out authorizerRagTrasm, splitcharstr);
                propCorrs = Utils.estraiCorrispondenti(proposersOppData, infoUtente, out proposerTemplate, out proposerNotFound, out proposerRagTrasm, splitcharstr);
                revCorrs = Utils.estraiCorrispondenti(reviewsOppData, infoUtente, out reviewsTemplate, out reviewsNotFound, out reviewsRagTrasm, splitcharstr);
                if (!string.IsNullOrEmpty(authorizerNotFound))
                {
                    oppDataRes = new OpportunityData();
                    oppDataRes.DataName = "Authorizer";
                    oppDataRes.DataValue = authorizerNotFound;
                    notaRolesNotFound += "Authorizer:" + Environment.NewLine + authorizerNotFound.Replace("§", ", ") + "<br/>";
                    foreach (string zz1 in authorizerNotFound.Split('§'))
                    {
                        missingRoles += zz1 + " (Authorizer)" + Environment.NewLine;
                    }
                    oppDatasRes.Add(oppDataRes);
                }
                if (!string.IsNullOrEmpty(proposerNotFound))
                {
                    oppDataRes = new OpportunityData();
                    oppDataRes.DataName = "Proposers";
                    oppDataRes.DataValue = proposerNotFound;
                    notaRolesNotFound += "Proposers:" + Environment.NewLine + proposerNotFound.Replace("§", ", ") + "<br/>";
                    foreach (string zz1 in proposerNotFound.Split('§'))
                    {
                        missingRoles += zz1 + " (Proposer)" + Environment.NewLine;
                    }
                    oppDatasRes.Add(oppDataRes);
                }
                if (!string.IsNullOrEmpty(reviewsNotFound))
                {
                    oppDataRes = new OpportunityData();
                    oppDataRes.DataName = "Reviews";
                    oppDataRes.DataValue = reviewsNotFound;
                    notaRevsNotFound += "Reviews:" + Environment.NewLine + reviewsNotFound.Replace("§", ", ") + "<br/>";
                    foreach (string zz1 in reviewsNotFound.Split('§'))
                    {
                        if (zz1.Contains(splitcharstr))
                            missingRoles += zz1.Substring(0, zz1.LastIndexOf(splitcharstr)) + " (" + zz1 + ")" + Environment.NewLine;
                        else missingRoles += zz1 + " (" + zz1 + ")" + Environment.NewLine;
                    }
                    oppDatasRes.Add(oppDataRes);
                }
                logger.Debug("Gestione corrispondenti");
                response.OppDatas = (Domain.OpportunityData[])oppDatasRes.ToArray(typeof(Domain.OpportunityData));


                DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] listaTemplate = null;
                listaTemplate = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getListTemplatesLiteByRole(infoUtente.idAmministrazione, infoUtente.idGruppo);
                logger.Debug("Prelievo Template");
                if (listaTemplate != null && listaTemplate.Length > 0)
                {
                    foreach (DocsPaVO.ProfilazioneDinamicaLite.TemplateLite TL in listaTemplate)
                    {
                        if (idTemplate.ToUpper().Equals(TL.name.ToUpper()))
                        {
                            template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(TL.system_id);
                        }
                    }
                }

                if (template == null) { throw new PisException("TEMPLATE_NOT_FOUND"); }

                DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] listaTemplateDoc = null;
                listaTemplateDoc = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getListTemplatesLiteByRole(infoUtente.idAmministrazione, infoUtente.idGruppo);

                if (listaTemplateDoc != null && listaTemplateDoc.Length > 0)
                {
                    foreach (DocsPaVO.ProfilazioneDinamicaLite.TemplateLite TL in listaTemplateDoc)
                    {
                        if (idTemplateDoc.ToUpper().Equals(TL.name.ToUpper()))
                        {
                            templateDoc = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(TL.system_id);
                        }
                    }
                }
                if (templateDoc == null) { throw new PisException("TEMPLATE_NOT_FOUND"); }
                logger.Debug("Prelievo template doc");

                bool campoTrovato = false;
                DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                                           template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom));
                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in oggettiCustom)
                {
                    campoTrovato = false;
                    foreach (VtDocsWS.Domain.OpportunityData oppData in request.OppDatas)
                    {
                        if (oppData.DataName.ToUpper() == ogg.DESCRIZIONE.ToUpper())
                        {
                            switch (oppData.DataName.ToUpper())
                            {
                                //case ("AUTHORIZER"):
                                //    ogg.VALORE_DATABASE = authorizerTemplate;
                                //    break;
                                //case ("PROPOSERS"):
                                //    ogg.VALORE_DATABASE = proposerTemplate;
                                //    break;
                                //case ("REVIEWS"):
                                //    ogg.VALORE_DATABASE = reviewsTemplate;
                                //    break;
                                case ("OPPORTUNITY ID"):
                                    ogg.VALORE_DATABASE = oppData.DataValue;
                                    idOggOppId = ogg.SYSTEM_ID.ToString();
                                    valoreOppId = oppData.DataValue;
                                    break;
                                default:
                                    if (!string.IsNullOrEmpty(oppData.DataValue) && oppData.DataValue.Contains("&amp;"))
                                        oppData.DataValue = oppData.DataValue.Replace("&amp;", "&");
                                    if (!string.IsNullOrEmpty(oppData.DataValue))
                                        ogg.VALORE_DATABASE = oppData.DataValue.Replace("§", Environment.NewLine);
                                    else ogg.VALORE_DATABASE = "";
                                    break;
                            }

                            if (oppData.DataName.ToUpper() == authorizerKey.ToUpper()) ogg.VALORE_DATABASE = authorizerTemplate;
                            if (oppData.DataName.ToUpper() == proposersKey.ToUpper()) ogg.VALORE_DATABASE = proposerTemplate;
                            if (oppData.DataName.ToUpper() == reviewsKey.ToUpper()) ogg.VALORE_DATABASE = reviewsTemplate;


                            campoTrovato = true;
                        }
                        if (campoTrovato) break;
                    }

                    if (ogg.DESCRIZIONE.Trim().ToUpper() == missingRolesKey.ToUpper())
                        ogg.VALORE_DATABASE = missingRoles;

                }
                int idDiagramma = 0;
                idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociatoFasc(template.SYSTEM_ID.ToString());
                if (idDiagramma != 0)
                {
                    setStatoDiagrammaIniziale = true;
                    diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                    if (diagramma != null)
                    {
                        if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                        {
                            foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                            {
                                if (stato.STATO_INIZIALE == true) statoIniziale = stato;
                            }
                        }
                    }
                }
                logger.Debug("Popolamento template");

                DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustomDoc = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                                           templateDoc.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom));
                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in oggettiCustomDoc)
                {
                    campoTrovato = false;
                    foreach (VtDocsWS.Domain.OpportunityData oppData in request.OppDatas)
                    {
                        if (oppData.DataName.ToUpper() == ogg.DESCRIZIONE.ToUpper())
                        {
                            //switch (oppData.DataName.ToUpper())
                            //{
                            //    case ("AUTHORIZER"):
                            //        ogg.VALORE_DATABASE = authorizerTemplate;
                            //        break;
                            //    case ("PROPOSERS"):
                            //        ogg.VALORE_DATABASE = proposerTemplate;
                            //        break;
                            //    case ("REVIEWS"):
                            //        ogg.VALORE_DATABASE = reviewsTemplate;
                            //        break;
                            //    //case("OPPORTUNITY ID"):
                            //    //    ogg.VALORE_DATABASE = oppData.DataValue;
                            //    //    idOggOppId = ogg.SYSTEM_ID.ToString();
                            //    //    valoreOppId = oppData.DataValue;
                            //    //    break;
                            //    default:
                            //        if (!string.IsNullOrEmpty(oppData.DataValue) && oppData.DataValue.Contains("&amp;"))
                            //            oppData.DataValue = oppData.DataValue.Replace("&amp;", "&");
                            //        if (!string.IsNullOrEmpty(oppData.DataValue))
                            //            ogg.VALORE_DATABASE = oppData.DataValue.Replace("§", Environment.NewLine);
                            //        else ogg.VALORE_DATABASE = "";
                            //        break;
                            //}
                            if (!string.IsNullOrEmpty(oppData.DataValue) && oppData.DataValue.Contains("&amp;"))
                                oppData.DataValue = oppData.DataValue.Replace("&amp;", "&");
                            if (!string.IsNullOrEmpty(oppData.DataValue))
                                ogg.VALORE_DATABASE = oppData.DataValue.Replace("§", Environment.NewLine);
                            else ogg.VALORE_DATABASE = "";

                            if (oppData.DataName.ToUpper() == authorizerKey.ToUpper()) ogg.VALORE_DATABASE = authorizerTemplate;
                            if (oppData.DataName.ToUpper() == proposersKey.ToUpper()) ogg.VALORE_DATABASE = proposerTemplate;
                            if (oppData.DataName.ToUpper() == reviewsKey.ToUpper()) ogg.VALORE_DATABASE = reviewsTemplate;


                            campoTrovato = true;
                        }
                        if (campoTrovato) break;
                    }
                    if ((ogg.DESCRIZIONE.ToUpper() == "TRAFFIC LIGHT" || ogg.DESCRIZIONE.ToUpper() == "RISK LEVEL") && string.IsNullOrEmpty(ogg.VALORE_DATABASE))
                    {
                        ogg.VALORE_DATABASE = "Gray (not defined)";
                    }

                }
                logger.Debug("Popolamento template DOC");

                #endregion
                // Controllo se l'opportunità è presente tramite il suo ID - TODO
                ArrayList idsrefactor = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getIdPrjByAssTemplates(idOggOppId, valoreOppId, "DESC");
                if (idsrefactor != null && idsrefactor.Count > 0)
                {
                    refactoring = true;
                    idPrjOppRefactor = (string)idsrefactor[0];
                }

                //Creazione
                if (!refactoring)
                {
                    logger.Debug("Creazione nuovo fascicolo");
                    // Prelievo del titolario attivo
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


                    // Creazione del fascicolo, e assegnazione dei campi profilati
                    DocsPaVO.fascicolazione.Classificazione classificazione = Utils.GetClassificazione(infoUtente, nodoClassifica);
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
                    if (classificazione.registro != null)
                    {
                        fascicolo.idRegistro = classificazione.registro.systemId;
                    }

                    fascicolo.idTitolario = titolario.ID;
                    fascicolo.controllato = "0";
                    fascicolo.privato = "0";
                    fascicolo.apertura = DateTime.Now.ToString("dd/MM/yyyy");
                    fascicolo.codiceGerarchia = nodoClassifica;
                    fascicolo.descrizione = oggettoFasc;
                    fascicolo.codUltimo = BusinessLogic.Fascicoli.FascicoloManager.getFascNumRif(classificazione.systemID, null);

                    //template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(idTemplate);


                    fascicolo.template = template;

                    //throw new Exception("Ferma qua");

                    DocsPaVO.fascicolazione.ResultCreazioneFascicolo result;
                    fascicolo = BusinessLogic.Fascicoli.FascicoloManager.newFascicolo(
                                    classificazione,
                                    fascicolo,
                                    infoUtente,
                                    ruolo,
                                    false,
                                    out result);

                    if (result != DocsPaVO.fascicolazione.ResultCreazioneFascicolo.OK)
                    {
                        throw new PisException("ERROR_PROJECT");
                    }
                    else
                    {
                        if (setStatoDiagrammaIniziale)
                        {
                            BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStatoFasc(fascicolo.systemID, statoIniziale.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                        }
                    }

                }
                //Refactoring
                else
                {
                    if(!string.IsNullOrEmpty(soloModCampi))
                        logger.DebugFormat("Modifica dei campi (no reworking) per il progetto con id {0}, opportunity id {1}", idPrjOppRefactor, valoreOppId);
                    else
                    logger.DebugFormat("Refactoring per il progetto con id {0}, opportunity id {1}", idPrjOppRefactor, valoreOppId);
                   
                    string idFolder = "";
                    DocsPaVO.fascicolazione.Folder folder1 = null;
                    DocsPaVO.fascicolazione.Folder folderFasc = null;
                    bool folderTraovata = false;
                    fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(idPrjOppRefactor, infoUtente);
                    if (fascicolo != null)
                    {
                        // modifica del fascicolo
                        fascicolo.descrizione = oggettoFasc;
                        fascicolo.template = new DocsPaVO.ProfilazioneDinamica.Templates();
                        fascicolo.template = template;
                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.updateFascicolo(fascicolo);

                        if (template != null)
                        {
                            BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.salvaInserimentoUtenteProfDimFasc(fascicolo.template, fascicolo.systemID);
                        }
                        if (string.IsNullOrEmpty(soloModCampi))
                        {
                            ArrayList cartelle = BusinessLogic.Fascicoli.FascicoloManager.getListaFolderDaIdFascicolo(infoUtente, fascicolo.systemID, null, false, false);
                            if (cartelle != null && cartelle.Count > 0)
                            {
                                logger.Debug("Cartelle trovate");
                                foreach (DocsPaVO.fascicolazione.Folder f in cartelle)
                                {
                                    logger.Debug(f.descrizione);
                                    if (f.descrizione.ToUpper() == "OLD")
                                    {
                                        folderTraovata = true;
                                        idFolder = f.systemID;
                                    }
                                }
                            }

                            if (!folderTraovata)
                            {
                                string idParentFolder = "";
                                folder1 = BusinessLogic.Fascicoli.FolderManager.getFolderByIdFascicolo(infoUtente.idPeople, infoUtente.idGruppo, fascicolo);
                                //BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                                idParentFolder = folder1.systemID;

                                DocsPaVO.fascicolazione.ResultCreazioneFolder resCrFldr = new DocsPaVO.fascicolazione.ResultCreazioneFolder();
                                DocsPaVO.fascicolazione.Folder newFolder = new DocsPaVO.fascicolazione.Folder();
                                newFolder.descrizione = "OLD";
                                newFolder.idFascicolo = fascicolo.systemID;
                                newFolder.idParent = idParentFolder;

                                DocsPaVO.fascicolazione.Folder cartella = BusinessLogic.Fascicoli.FolderManager.newFolder(newFolder, infoUtente, BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo), out resCrFldr);

                                switch (resCrFldr)
                                {
                                    case DocsPaVO.fascicolazione.ResultCreazioneFolder.DM_ERROR:
                                        throw new Exception("Errore nella creazione della cartella");
                                        break;
                                    case DocsPaVO.fascicolazione.ResultCreazioneFolder.FOLDER_EXIST:
                                        throw new Exception("Errore nella creazione della cartella: cartella già esistente");
                                        break;
                                    case DocsPaVO.fascicolazione.ResultCreazioneFolder.GENERIC_ERROR:
                                        throw new Exception("Errore nella creazione della cartella: Errore generico");
                                        break;
                                    default:
                                        break;
                                }
                                idFolder = cartella.systemID;
                            }
                            folderFasc = BusinessLogic.Fascicoli.FolderManager.getFolderByIdFascicolo(infoUtente.idPeople, infoUtente.idGruppo, fascicolo);
                            ArrayList docDaSpostare = BusinessLogic.Fascicoli.FolderManager.getDocumenti(infoUtente.idGruppo, infoUtente.idPeople, folderFasc);
                            string msgFolder = "";
                            if (docDaSpostare != null && docDaSpostare.Count > 0)
                            {
                                foreach (DocsPaVO.documento.InfoDocumento documento in docDaSpostare)
                                {
                                    BusinessLogic.Fascicoli.FolderManager.RemoveDocumentFromProject(infoUtente, documento.docNumber, folderFasc, "", out msgFolder);
                                    if (!BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente, documento.docNumber, idFolder, false, out msgFolder))
                                        throw new Exception("Error during document insert in old folder");
                                }
                            }
                            if (setStatoDiagrammaIniziale)
                            {
                                BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStatoFasc(fascicolo.systemID, statoIniziale.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                            }

                            if (!BusinessLogic.Amministrazione.SistemiEsterni.CAPRefactorTrasmissioni(fascicolo.systemID, "REMOVED FOR CAP REWORKING"))
                                throw new Exception("Error during transmission removal");
                        }
                        else
                        {
                            folderFasc = BusinessLogic.Fascicoli.FolderManager.getFolderByIdFascicolo(infoUtente.idPeople, infoUtente.idGruppo, fascicolo);
                            ArrayList docDaSpostare = BusinessLogic.Fascicoli.FolderManager.getDocumenti(infoUtente.idGruppo, infoUtente.idPeople, folderFasc);
                            string msgFolder = "";
                            if (docDaSpostare != null && docDaSpostare.Count > 0)
                            {
                                foreach (DocsPaVO.documento.InfoDocumento documento in docDaSpostare)
                                {
                                    
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new PisException("PROJECT_NOT_FOUND");
                    }
                }
                // upload dei documenti (probabilmente da spostare fuori l'if refactoring)
                DocsPaVO.documento.SchedaDocumento schedaDoc = null;
                DocsPaVO.documento.SchedaDocumento schedaDocResult = null;

                schedaDoc = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(infoUtente, false);
                schedaDoc.oggetto.descrizione = oggettoFasc;
                schedaDoc.oggetto.daAggiornare = true;
                schedaDoc.tipoProto = "G";

                schedaDoc.template = templateDoc;
                schedaDoc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                schedaDoc.tipologiaAtto.systemId = templateDoc.SYSTEM_ID.ToString();
                schedaDoc.tipologiaAtto.descrizione = templateDoc.DESCRIZIONE.ToString();

                schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                string errorMessage;
                string msg;
                if (!BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, schedaDocResult.systemId, fascicolo.systemID, false, out msg))
                    throw new Exception("Error during uploading file in Opportunity");

                if (request.Files != null && request.Files.Length > 0)
                {
                    foreach (VtDocsWS.Domain.File doc in request.Files)
                    {
                        schedaDoc = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(infoUtente, false);
                        if (doc.Description != null)
                            schedaDoc.oggetto.descrizione = doc.Description;
                        else
                            schedaDoc.oggetto.descrizione = doc.Name.Substring(0, doc.Name.LastIndexOf("."));
                        schedaDoc.oggetto.daAggiornare = true;
                        schedaDoc.tipoProto = "G";

                        // Verificare la tipologia per il documento
                        //schedaDoc.template = templateDoc;
                        //schedaDoc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                        //schedaDoc.tipologiaAtto.systemId = templateDoc.SYSTEM_ID.ToString();
                        //schedaDoc.tipologiaAtto.descrizione = templateDoc.DESCRIZIONE.ToString();


                        schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);

                        DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)schedaDocResult.documenti[0];

                        DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                        {
                            name = doc.Name,
                            fullName = doc.Name,
                            content = doc.Content,
                            contentType = doc.MimeType,
                            length = doc.Content.Length,
                            bypassFileContentValidation = true
                        };



                        if (!BusinessLogic.Documenti.FileManager.putFile(ref versioneCorrente, fileDocumento, infoUtente, out errorMessage))
                        {
                            throw new PisException("FILE_CREATION_ERROR");
                        }

                        if (!BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, schedaDocResult.systemId, fascicolo.systemID, false, out msg))
                            throw new Exception("Error during uploading file in Opportunity");
                    }
                }
                //salvaTrasmissioni(authCorrs, "AUTHORIZER", infoUtente, fascicolo);
                //salvaTrasmissioni(propCorrs, "PROPOSER", infoUtente, fascicolo);
                //salvaTrasmissioni(revCorrs, "REVIEWER", infoUtente, fascicolo);

                //salvaTrasmissioni(authCorrs, authorizerRagTrasm, infoUtente, fascicolo);
                //salvaTrasmissioni(propCorrs, proposerRagTrasm, infoUtente, fascicolo);
                //salvaTrasmissioni(revCorrs, reviewsRagTrasm, infoUtente, fascicolo);

                Utils.salvaTrasmUnica(authCorrs, propCorrs, revCorrs, infoUtente, reviewsRagTrasm, fascicolo, notaRolesNotFound, notaRevsNotFound);

                BusinessLogic.Documenti.areaLavoroManager.execAddLavoroMethod(null, null, "", infoUtente, fascicolo);

                string pathFE = "";
                if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                    pathFE = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();

                if (!string.IsNullOrEmpty(pathFE))
                {
                    response.OpportunityLink = string.Format("{0}VisualizzaOggetto.htm?idAmministrazione={1}&tipoOggetto=F&idObj={2}", pathFE, infoUtente.idAmministrazione, fascicolo.codice);
                    //response.OpportunityLink += "&tknVCAP=" + System.Uri.EscapeDataString(request.AuthenticationToken.Substring(4));
                    response.Success = true;
                }
                else
                {
                    throw new Exception("Path del frontend non disponibile: impossibile generare il link.");
                }
                response.Success = true;

            }
            catch (PisException pisEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.CAPServices.Authenticate.AuthenticateResponse Authenticate(Services.CAPServices.Authenticate.AuthenticateRequest request)
        {
            Services.CAPServices.Authenticate.AuthenticateResponse response = new Services.CAPServices.Authenticate.AuthenticateResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                string userid = "";
                if (request == null || (string.IsNullOrEmpty(request.Username) && string.IsNullOrEmpty(request.Email)))
                {
                    throw new PisException("MISSING_PARAMETER");
                }

                if (string.IsNullOrEmpty(request.Password))
                {
                    throw new PisException("MISSING_PARAMETER");
                }

                if (string.IsNullOrEmpty(request.Username))
                {
                    utente = BusinessLogic.Amministrazione.SistemiEsterni.CAPGetUserByEmail(request.Email);
                    if (utente == null) throw new PisException("USER_NO_EXIST");

                    userid = utente.userId;
                }
                else userid = request.Username;
                DocsPaVO.utente.UserLogin userlog = new UserLogin(userid, request.Password, null);
                System.Collections.ArrayList listaAmmin = BusinessLogic.Utenti.UserManager.getListaIdAmmUtente(userlog);

                if (listaAmmin != null && listaAmmin.Count > 0)
                {
                    if (listaAmmin.Count == 1)
                        userlog.IdAmministrazione = listaAmmin[0].ToString();
                    else
                    {
                        throw new PisException("APPLICATION_ERROR");
                    }
                }
                if (listaAmmin == null) throw new PisException("APPLICATION_ERROR");
                utente = BusinessLogic.Utenti.UserManager.getUtente(userid, userlog.IdAmministrazione);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("APPLICATION_ERROR");
                }
                string encPass = BusinessLogic.Amministrazione.AmministraManager.GetPasswordUtenteMultiAmm(userid.ToUpper());
                encPass = encPass.Split('^')[0];
                if (DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(System.Text.Encoding.Unicode.GetBytes(request.Password)) != encPass)
                {
                    throw new PisException("USER_NO_EXIST");
                }


                DocsPaVO.utente.Ruolo ruolo = Utils.GetRuoloPreferito(utente.idPeople);
                if (ruolo == null)
                {
                    //L'utente non ha ruoli
                    throw new PisException("APPLICATION_ERROR");
                }

                DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);


                utente.ruoli = new System.Collections.ArrayList();
                utente.ruoli.Add(ruolo);

                response.AuthenticationToken = Utils.CreateAuthToken(utente, ruolo);
                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(utente.systemId);
                response.User = Utils.GetCorrespondent(corr, infoUtente);

                //response.User = new Domain.Correspondent();
                //response.User.Name = utente.nome;
                //response.User.Surname = utente.cognome;
                //response.User.Location = utente.sede;
                //response.User.Email = utente.email;
                //response.User.Id = utente.systemId;
                //response.User.CorrespondentType = utente.tipoCorrispondente;
                //response.User.Type = "I";
                //response.User.Description = utente.descrizione;
                //response.User.Code = utente.userId;


                response.Success = true;
            }
            catch (PisException pisEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;

        }

        public static Services.CAPServices.GetDocsInOpportunity.GetDocsInOpportunityResponse GetDocsInOpportunity(Services.CAPServices.GetDocsInOpportunity.GetDocsInOpportunityRequest request)
        {
            Services.CAPServices.GetDocsInOpportunity.GetDocsInOpportunityResponse response = new Services.CAPServices.GetDocsInOpportunity.GetDocsInOpportunityResponse();
            try
            {
                if (request == null || (string.IsNullOrEmpty(request.AuthenticationToken) || string.IsNullOrEmpty(request.IdPrjOpportunity)))
                {
                    throw new PisException("MISSING_PARAMETER");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(request.AuthenticationToken.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                InfoUtente infoUtTemp = null;
                Ruolo ruoloTemp = null;
                ArrayList apprInfoArray = BusinessLogic.Amministrazione.SistemiEsterni.CAPGetOpportunitiesPending("OPPORTUNITY", infoUtente.idPeople, "PENDING", request.IdPrjOpportunity);
                if (apprInfoArray != null && apprInfoArray.Count > 0)
                {

                    string apprInfo = apprInfoArray[0].ToString();
                    if (apprInfo.Split('§')[5] == infoUtente.idCorrGlobali)
                    {
                        infoUtTemp = infoUtente;
                        ruoloTemp = ruolo;
                    }
                    else
                    {
                        ruoloTemp = BusinessLogic.Utenti.UserManager.getRuoloById(apprInfo.Split('§')[5]);
                        infoUtTemp = new InfoUtente(utente, ruoloTemp);
                    }
                }

                DocsPaVO.fascicolazione.Folder folder = null;
                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
                List<DocsPaVO.ricerche.SearchResultInfo> idProfileList = null;
                Domain.Document[] responseDocuments = null;
                ArrayList listaIdDocs = null;
                fascicolo.systemID = request.IdPrjOpportunity;
                folder = BusinessLogic.Fascicoli.FolderManager.getFolderByIdFascicolo(infoUtTemp.idPeople, infoUtTemp.idGruppo, fascicolo);
                if (folder != null)
                {
                    // non va bene, prende l'id fascicolo e ritorna TUTTI i doc
                    //listaIdDocs = BusinessLogic.Fascicoli.FolderManager.getIdFolderDoc(folder.systemID);
                    listaIdDocs = BusinessLogic.Amministrazione.SistemiEsterni.CAPGetDocInFolder(folder.systemID);
                    if (listaIdDocs != null && listaIdDocs.Count > 0)
                    {
                        int indice = 0;
                        DocsPaVO.documento.SchedaDocumento documento = null;
                        responseDocuments = new Domain.Document[listaIdDocs.Count];
                        foreach (string idDoc in listaIdDocs)
                        {
                            documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtTemp, idDoc, idDoc);
                            responseDocuments[indice] = Utils.GetDocument(documento, infoUtTemp, true, false);
                            indice++;
                        }
                    }
                }
                else
                {
                    //Fascicolo non trovato
                    throw new PisException("PROJECT_NOT_FOUND");
                }

                if (idProfileList != null)
                    response.TotalDocumentsNumber = idProfileList.Count;

                response.Documents = responseDocuments;
                response.Success = true;
            }
            catch (PisException pisEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }
            return response;

        }

        public static Services.CAPServices.DownloadDoc.DownloadDocResponse DownloadDoc(Services.CAPServices.DownloadDoc.DownloadDocRequest request)
        {
            Services.CAPServices.DownloadDoc.DownloadDocResponse response = new Services.CAPServices.DownloadDoc.DownloadDocResponse();
            try
            {
                if (request == null || (string.IsNullOrEmpty(request.AuthenticationToken) || string.IsNullOrEmpty(request.IdDocument)))
                {
                    throw new PisException("MISSING_PARAMETER");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(request.AuthenticationToken.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();
                try
                {
                    documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                }
                catch (Exception ex2) { documento = null; }
                if (documento != null && documento.documenti != null && documento.documenti.Count > 0)
                {
                    DocsPaVO.documento.FileRequest versione = (DocsPaVO.documento.FileRequest)documento.documenti[0];
                    response.FileDoc = Utils.GetFile(versione, true, infoUtente, false, false, string.Empty, null, false);
                }
                else
                {
                    //throw new PisException("DOCUMENT_NOT_FOUND");
                    DocsPaVO.utente.Ruolo[] ruoli = (DocsPaVO.utente.Ruolo[])BusinessLogic.Utenti.UserManager.getRuoliUtente(utente.idPeople).ToArray(typeof(DocsPaVO.utente.Ruolo));
                    InfoUtente infoUtTemp = null;
                    foreach (Ruolo r1 in ruoli)
                    {
                        infoUtTemp = new InfoUtente(utente, r1);
                        try
                        {
                            documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtTemp, request.IdDocument, request.IdDocument);
                            if (documento != null && documento.documenti != null && documento.documenti.Count > 0)
                            {
                                DocsPaVO.documento.FileRequest versione = (DocsPaVO.documento.FileRequest)documento.documenti[0];
                                response.FileDoc = Utils.GetFile(versione, true, infoUtente, false, false, string.Empty, null, false);
                                break;
                            }
                        }
                        catch (Exception ex3) { }
                    }

                }
                response.Success = true;
            }
            catch (PisException pisEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }
            return response;
        }

        public static Services.CAPServices.GetOpportunities.GetOpportunitiesResponse GetOpportunities(Services.CAPServices.GetOpportunities.GetOpportunitiesRequest request)
        {
            VtDocsWS.Services.CAPServices.GetOpportunities.GetOpportunitiesResponse response = new Services.CAPServices.GetOpportunities.GetOpportunitiesResponse();
            try
            {
                if (request == null || (string.IsNullOrEmpty(request.AuthenticationToken)))
                {
                    throw new PisException("MISSING_PARAMETER");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(request.AuthenticationToken.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                string oppStatus = "PENDING";
                if (!string.IsNullOrEmpty(request.OpportunityStatus) && request.OpportunityStatus == "APPROVED") oppStatus = "APPROVED";
                ArrayList opps = new ArrayList();
                string[] oppSplit;
                opps = BusinessLogic.Amministrazione.SistemiEsterni.CAPGetOpportunitiesPending("OPPORTUNITY", utente.idPeople, oppStatus.ToUpper(), null);
                int indice = 0;
                DocsPaVO.fascicolazione.Fascicolo fascicolo = null;
                Domain.Project prjTemp = null;
                DocsPaVO.utente.Utente autOpp = null;
                if (opps != null && opps.Count > 0)
                {
                    response.Opportunities = new Project[opps.Count];
                    InfoUtente infoUtTemp = null;
                    foreach (string opp in opps)
                    {
                        fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
                        oppSplit = opp.Split('§');
                        prjTemp = new Project();
                        if (infoUtente.idCorrGlobali != oppSplit[3].Trim())
                        {
                            infoUtTemp = new InfoUtente(utente, BusinessLogic.Utenti.UserManager.getRuoloById(oppSplit[3].Trim()));
                        }
                        else { infoUtTemp = infoUtente; }

                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(oppSplit[0].Trim(), infoUtTemp);
                        if (fascicolo != null)
                        {
                            prjTemp = Utils.GetProject(fascicolo, infoUtTemp);

                            prjTemp.PhysicsCollocation = oppSplit[1].Trim();
                            autOpp = BusinessLogic.Utenti.UserManager.getUtente(fascicolo.creatoreFascicolo.idPeople);
                            prjTemp.PhysicsCollocation += "§" + autOpp.nome + "§" + autOpp.cognome + "§" + oppSplit[2];
                            response.Opportunities[indice] = prjTemp;
                            indice++;
                        }

                    }
                }

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static VtDocsWS.Services.CAPServices.ApproveOpportunity.ApproveOpportunityResponse ApproveOpportunity(VtDocsWS.Services.CAPServices.ApproveOpportunity.ApproveOpportunityRequest request)
        {
            VtDocsWS.Services.CAPServices.ApproveOpportunity.ApproveOpportunityResponse response = new Services.CAPServices.ApproveOpportunity.ApproveOpportunityResponse();
            try
            {
                if (request == null || (string.IsNullOrEmpty(request.AuthenticationToken)))
                {
                    throw new PisException("MISSING_PARAMETER");
                }
                if (string.IsNullOrEmpty(request.AcceptReject))
                {
                    throw new PisException("MISSING_PARAMETER");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(request.AuthenticationToken.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                string errore = "", mode = "", idObj = "";
                ArrayList apprInfoArray = BusinessLogic.Amministrazione.SistemiEsterni.CAPGetOpportunitiesPending("OPPORTUNITY", infoUtente.idPeople, "PENDING", request.IdOppDocsPa);
                if (apprInfoArray != null && apprInfoArray.Count > 0)
                {
                    InfoUtente infoUtTemp = null;
                    Ruolo ruoloTemp = null;
                    string apprInfo = apprInfoArray[0].ToString();
                    if (apprInfo.Split('§')[5] == infoUtente.idCorrGlobali)
                    {
                        infoUtTemp = infoUtente;
                        ruoloTemp = ruolo;
                    }
                    else
                    {
                        ruoloTemp = BusinessLogic.Utenti.UserManager.getRuoloById(apprInfo.Split('§')[5]);
                        infoUtTemp = new InfoUtente(utente, ruoloTemp);
                    }
                    DocsPaVO.trasmissione.TrasmissioneUtente[] trasmUtente = BusinessLogic.Trasmissioni.TrasmManager.getTrasmissioneUtenteInRuolo(infoUtTemp, apprInfo.Split('§')[3], utente);
                    if (trasmUtente != null && trasmUtente.Length > 0)
                    {
                        DocsPaVO.trasmissione.TrasmissioneUtente trasmUtenteX = trasmUtente[0];
                        if (request.AcceptReject.ToUpper() == "ACCEPT")
                        {
                            trasmUtenteX.tipoRisposta = DocsPaVO.trasmissione.TipoRisposta.ACCETTAZIONE;
                            if (!BusinessLogic.Amministrazione.SistemiEsterni.CAPCtrlContribution(request.IdOppDocsPa, DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(authInfoArray[4], "BE_CAP_TR_REASON_SPLITCHAR")))
                                throw new Exception("Mandatory Contribution not present, approval not allowed");
                        }
                        else if (request.AcceptReject.ToUpper() == "REJECT") trasmUtenteX.tipoRisposta = DocsPaVO.trasmissione.TipoRisposta.RIFIUTO;

                        if (!BusinessLogic.Trasmissioni.ExecTrasmManager.executeAccRifMethod(trasmUtenteX, apprInfo.Split('§')[4], ruoloTemp, infoUtTemp, out errore, out mode, out idObj))
                            throw new Exception("ERROR DURING APPROVAL");
                    }
                    else throw new Exception("TRANSMISSION NOT FOUND");
                }
                else throw new PisException("APPLICATION_ERROR");


                response.Success = true;
            }
            catch (PisException pisEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static VtDocsWS.Services.CAPServices.GetOppApprovals.GetOppApprovalsResponse GetOppApprovals(VtDocsWS.Services.CAPServices.GetOppApprovals.GetOppApprovalsRequest request)
        {
            VtDocsWS.Services.CAPServices.GetOppApprovals.GetOppApprovalsResponse response = new Services.CAPServices.GetOppApprovals.GetOppApprovalsResponse();
            try
            {
                if (request == null || (string.IsNullOrEmpty(request.AuthenticationToken)))
                {
                    throw new PisException("MISSING_PARAMETER");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(request.AuthenticationToken.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                ArrayList respArray = BusinessLogic.Amministrazione.SistemiEsterni.CAPGetOppApprovals(request.IdOppDocspa);

                response.OppApprovals = (string[])respArray.ToArray(typeof(string));

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
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