using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Collections;
using DocsPaVO.utente;
using DocsPaVO.documento;
using DocsPaVO.filtri;
using DocsPaVO.amministrazione;
using DocsPaVO.ricerche;
using VtDocsWS.WebServices;
using log4net;


namespace VtDocsWS.Manager
{
    public class ProjectsManager
    {

        private static ILog logger = LogManager.GetLogger(typeof(ProjectsManager));

        public static Services.Projects.CreateProject.CreateProjectResponse CreateProject(Services.Projects.CreateProject.CreateProjectRequest request)
        {
            Services.Projects.CreateProject.CreateProjectResponse response = new Services.Projects.CreateProject.CreateProjectResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "CreateProject");
                bool setStatoDiagrammaIniziale = false;
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                DocsPaVO.DiagrammaStato.Stato statoIniziale = null;
                                

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (request.Project == null)
                {
                    throw new PisException("REQUIRED_PROJECT");
                }

                if (request.Project == null || string.IsNullOrEmpty(request.Project.CodeNodeClassification))
                {
                    throw new PisException("REQUIRED_GENERAL_NODE");
                }

                if (request.Project.ClassificationScheme == null || string.IsNullOrEmpty(request.Project.ClassificationScheme.Id))
                {
                    throw new PisException("ID_CLASSIFICATIONSCHEME_REQUIRED");
                }

                DocsPaVO.amministrazione.OrgTitolario titolario = null;

                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                //Reperimento ruolo utente
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                try
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                    if (ruolo == null)
                    {
                        //Ruolo non trovato
                        throw new PisException("ROLE_NO_EXIST");
                    }
                }
                catch
                {
                    //Ruolo non trovato
                    throw new PisException("ROLE_NO_EXIST");
                }

                try
                {
                    titolario = BusinessLogic.Amministrazione.TitolarioManager.getTitolarioById(request.Project.ClassificationScheme.Id);
                }
                catch
                {
                    //Titolario non trovati
                    throw new PisException("CLASSIFICATION_NOT_FOUND");
                }

                //Reperimento del nodo di titolario
                DocsPaVO.fascicolazione.Classificazione classificazione = Utils.GetClassificazione(infoUtente, request.Project.CodeNodeClassification);

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
                else
                {
                    try
                    {
                        if (classificazione.registro != null)
                        {
                            request.Project.Register = new Domain.Register();
                            request.Project.Register.Id = classificazione.registro.systemId;
                        }
                        fascicolo = Utils.GetProjectFromPis(request.Project, infoUtente);

                        if(request.Project.Register != null)
                        fascicolo.codUltimo = BusinessLogic.Fascicoli.FascicoloManager.getFascNumRif(classificazione.systemID, request.Project.Register.Id);
                        else
                            fascicolo.codUltimo = BusinessLogic.Fascicoli.FascicoloManager.getFascNumRif(classificazione.systemID,null);
                        
                        ///Imposto il template
                        if (request != null && request.Project != null && request.Project.Template != null && (!string.IsNullOrEmpty(request.Project.Template.Id) || (!string.IsNullOrEmpty(request.Project.Template.Name))))
                        {
                            DocsPaVO.ProfilazioneDinamica.Templates template = null;
                            if (!string.IsNullOrEmpty(request.Project.Template.Id))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(request.Project.Template.Id);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(request.Project.Template.Name))
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateByDescrizione(request.Project.Template.Name);
                                }
                            }

                            //Modifica Lembo 11-01-2013: Controllo se l'utente è abilitato all'utilizzo del template.
                            //Solo ruoli con diritti di scrittura ammessi.
                            string controlFilter = string.Format("{0} and id_ruolo={1} and (diritti='2')", template.SYSTEM_ID.ToString(), infoUtente.idGruppo);
                            ArrayList controlloVisibilita = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getRuoliTipoFasc(controlFilter);
                            if (controlloVisibilita == null || controlloVisibilita.Count == 0)
                            {
                                throw new PisException("TEMPLATE_NOT_ROLE_EDITABLE");
                            }

                            if (template != null)
                            {
                                // Modifica Lembo 11-01-2013: Utilizzo il metodo che controlla la visibilità dei campi.
                                //fascicolo.template = Utils.GetTemplateFromPis(request.Project.Template, template, false);
                                fascicolo.template = Utils.GetTemplateFromPisVisibility(request.Project.Template, template, false, infoUtente.idGruppo, "P", request.CodeApplication);
                                fascicolo.tipo = fascicolo.template.SYSTEM_ID.ToString();

                                int idDiagramma = 0;
                                idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociatoFasc(fascicolo.template.SYSTEM_ID.ToString());
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
                            }
                            else
                            {
                                //Template non trovato
                                throw new PisException("TEMPLATE_NOT_FOUND");
                            }
                        }
                        ///

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

                            response.Project = Utils.GetProject(fascicolo, infoUtente);
                            if (setStatoDiagrammaIniziale)
                            {
                                response.Project.Template.StateDiagram.StateOfDiagram[0] = Utils.GetStateOfDiagram(statoIniziale, diagramma.SYSTEM_ID.ToString()); 
                            }

                            if (!string.IsNullOrEmpty(infoUtente.diSistema) && infoUtente.diSistema == "1")
                            {
                                bool pulito = Utils.CleanRightsExtSys(fascicolo.systemID, infoUtente.idPeople, infoUtente.idGruppo);
                            }
                        }
                    }
                    catch
                    {
                        throw new PisException("ERROR_PROJECT");
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

        public static Services.Projects.EditPrjStateDiagram.EditPrjStateDiagramResponse EditPrjStateDiagram(Services.Projects.EditPrjStateDiagram.EditPrjStateDiagramRequest request)
        {
            Services.Projects.EditPrjStateDiagram.EditPrjStateDiagramResponse response = new Services.Projects.EditPrjStateDiagram.EditPrjStateDiagramResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "EditPrjStateDiagram");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) || !string.IsNullOrEmpty(request.CodeProject)) && !string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject)) || (string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject)))
                {
                    throw new PisException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }

                if (string.IsNullOrEmpty(request.StateOfDiagram))
                {
                    throw new PisException("REQUIRED_STATE_OF_DIAGRAM");
                }

                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdProject))
                    {
                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdProject, infoUtente);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.CodeProject) && (!string.IsNullOrEmpty(request.ClassificationSchemeId)))
                        {
                            DocsPaVO.fascicolazione.Fascicolo[] fascicoli = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurity(request.CodeProject, infoUtente.idAmministrazione, request.ClassificationSchemeId, false);
                            if (fascicoli != null && fascicoli.Length > 0)
                            {
                                fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicoli[0].systemID, infoUtente);
                            }
                            else
                            {
                                throw new PisException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PisException("PROJECT_NOT_FOUND");
                }

                if (fascicolo != null)
                {
                    fascicolo.template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFasc(fascicolo.systemID);
                    if (fascicolo.template != null)
                    {
                        int idDiagramma = 0;
                        DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                        DocsPaVO.DiagrammaStato.Stato statoAttuale = null;
                        idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociatoFasc(fascicolo.template.SYSTEM_ID.ToString());
                        if (idDiagramma != 0)
                        {
                            diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                            if (diagramma != null)
                            {
                                if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                                {
                                    statoAttuale = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoFasc(fascicolo.systemID);
                                    bool result = false;


                                    if (statoAttuale == null)
                                    {
                                        foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                        {
                                            if (stato.DESCRIZIONE.ToUpper().Equals(request.StateOfDiagram.ToUpper()))
                                            {
                                                BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStatoFasc(fascicolo.systemID, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                                result = true;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < diagramma.PASSI.Count; i++)
                                        {
                                            DocsPaVO.DiagrammaStato.Passo step = (DocsPaVO.DiagrammaStato.Passo)diagramma.PASSI[i];
                                            if (step.STATO_PADRE.SYSTEM_ID == statoAttuale.SYSTEM_ID)
                                            {
                                                for (int j = 0; j < step.SUCCESSIVI.Count; j++)
                                                {
                                                    if (((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).DESCRIZIONE.ToUpper().Equals(request.StateOfDiagram.ToUpper()))
                                                    {
                                                        BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStatoFasc(fascicolo.systemID, ((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                                        result = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (!result)
                                    {
                                        throw new PisException("STATEOFDIAGRAM_NOT_FOUND");
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new PisException("DIAGRAM_NOT_FOUND");
                        }
                    }
                    else
                    {
                        throw new PisException("TEMPLATE_NOT_FOUND");
                    }
                }
                else
                {
                    throw new PisException("PROJECT_NOT_FOUND");
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

        public static Services.Projects.EditProject.EditProjectResponse EditProject(Services.Projects.EditProject.EditProjectRequest request)
        {
            Services.Projects.EditProject.EditProjectResponse response = new Services.Projects.EditProject.EditProjectResponse();

            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "EditProject");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (request.Project == null)
                {
                    throw new PisException("REQUIRED_PROJECT");
                }

                Domain.Project responseProject = new Domain.Project();

                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                try
                {
                    fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.Project.Id, infoUtente);

                    if (fascicolo != null)
                    {
                        fascicolo.cartaceo = request.Project.Paper;
                        if (request.Project.Controlled)
                        {
                            fascicolo.controllato = "1";
                        }
                        else
                        {
                            fascicolo.controllato = "0";
                        }

                        fascicolo.descrizione = request.Project.Description;
                        fascicolo.dtaLF = request.Project.CollocationDate;
                        fascicolo.idUoLF = request.Project.PhysicsCollocation;
                        if (request.Project.Private)
                        {
                            fascicolo.privato = "1";
                        }
                        else
                        {
                            fascicolo.privato = "0";
                        }

                        ///Imposto il template
                        DocsPaVO.ProfilazioneDinamica.Templates template = null;
                        if (request != null && request.Project != null && request.Project.Template != null && (!string.IsNullOrEmpty(request.Project.Template.Id) || (!string.IsNullOrEmpty(request.Project.Template.Name))))
                        {
                            
                            if (!string.IsNullOrEmpty(request.Project.Template.Id))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(request.Project.Template.Id);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(request.Project.Template.Name))
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateByDescrizione(request.Project.Template.Name);
                                }
                            }
                            //Modifica Lembo 11-01-2013: Controllo se l'utente è abilitato all'utilizzo del template.
                            //Solo ruoli con diritti di scrittura ammessi.
                            string controlFilter = string.Format("{0} and id_ruolo={1} and (diritti='2')", template.SYSTEM_ID.ToString(), infoUtente.idGruppo);
                            ArrayList controlloVisibilita = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getRuoliTipoFasc(controlFilter);
                            if (controlloVisibilita == null || controlloVisibilita.Count == 0)
                            {
                                throw new PisException("TEMPLATE_NOT_ROLE_EDITABLE");
                            }

                            if (template != null)
                            {
                                // Modifica per gestire tipologie modificate in seguito al salvataggio di un documento.
                                fascicolo.template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascDettagli(fascicolo.systemID);
                                if (fascicolo.template != null)
                                {
                                    template = fascicolo.template;
                                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg1 in fascicolo.template.ELENCO_OGGETTI)
                                    {
                                        DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue oldObjText = new DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue();
                                        oldObjText.IDTemplate = fascicolo.template.SYSTEM_ID.ToString();
                                        oldObjText.ID_Doc_Fasc = fascicolo.systemID;
                                        oldObjText.ID_Oggetto = ogg1.SYSTEM_ID.ToString();
                                        oldObjText.Valore = ogg1.VALORE_DATABASE;
                                        oldObjText.Tipo_Ogg_Custom = ogg1.TIPO.DESCRIZIONE_TIPO;
                                        oldObjText.ID_People = infoUtente.idPeople;
                                        oldObjText.ID_Ruolo_In_UO = infoUtente.idCorrGlobali;
                                        template.OLD_OGG_CUSTOM.Add(oldObjText);
                                    }
                                }

                                // Modifica Lembo 11-01-2013: Utilizzo il metodo che controlla la visibilità dei campi.
                                //fascicolo.template = Utils.GetTemplateFromPis(request.Project.Template, template, false);
                                fascicolo.template = Utils.GetTemplateFromPisVisibility(request.Project.Template, template, false, infoUtente.idGruppo, "P",request.CodeApplication);
                                //fascicolo.tipo = template.SYSTEM_ID.ToString();

                            }
                            else
                            {
                                //Template non trovato
                                throw new PisException("TEMPLATE_NOT_FOUND");
                            }
                        }
                        ///

                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.updateFascicolo(fascicolo);

                        if (template != null)
                        {
                            BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.salvaInserimentoUtenteProfDimFasc(fascicolo.template, fascicolo.systemID);
                        }
                    }
                    else
                    {
                        throw new PisException("PROJECT_NOT_FOUND");
                    }
                }
                catch (Exception ex)
                {
                    //throw new PisException("PROJECT_NOT_FOUND");
                    throw new Exception("Errore nell'aggiornamento dei dati del fascicolo");
                }

                if (fascicolo != null)
                {
                    responseProject = Utils.GetProject(fascicolo, infoUtente);
                    if (responseProject != null)
                    {
                        response.Project = responseProject;
                    }
                    else
                    {
                        throw new PisException("PROJECT_NOT_FOUND");
                    }
                }
                else
                {
                    throw new PisException("PROJECT_NOT_FOUND");
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

        public static Services.Projects.GetProject.GetProjectResponse GetProject(Services.Projects.GetProject.GetProjectRequest request)
        {
            Services.Projects.GetProject.GetProjectResponse response = new Services.Projects.GetProject.GetProjectResponse();

            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetProject");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) || !string.IsNullOrEmpty(request.CodeProject)) && !string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject)) || (string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject)))
                {
                    throw new PisException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }

                Domain.Project responseProject = new Domain.Project();

                DocsPaVO.fascicolazione.Fascicolo fascicolo = null;

                try
                {
                    if (!string.IsNullOrEmpty(request.IdProject))
                    {
                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdProject, infoUtente);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.CodeProject) && (!string.IsNullOrEmpty(request.ClassificationSchemeId)))
                        {
                            DocsPaVO.fascicolazione.Fascicolo[] fascicoli = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurity(request.CodeProject, infoUtente.idAmministrazione, request.ClassificationSchemeId, false);
                            if (fascicoli != null && fascicoli.Length > 0)
                            {
                                fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicoli[0].systemID, infoUtente);
                                if (fascicolo == null && fascicoli.Length > 1)
                                {
                                    fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicoli[1].systemID, infoUtente);
                                }
                            }
                            else
                            {
                                throw new PisException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PisException("PROJECT_NOT_FOUND");
                }

                if (fascicolo != null)
                {
                    responseProject = Utils.GetProject(fascicolo, infoUtente);
                    if (responseProject != null)
                    {
                        response.Project = responseProject;
                    }
                    else
                    {
                        throw new PisException("PROJECT_NOT_FOUND");
                    }
                }
                else
                {
                    throw new PisException("PROJECT_NOT_FOUND");
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

        public static Services.Projects.GetProjectsByDocument.GetProjectsByDocumentResponse GetProjectsByDocument(Services.Projects.GetProjectsByDocument.GetProjectsByDocumentRequest request)
        {
            Services.Projects.GetProjectsByDocument.GetProjectsByDocumentResponse response = new Services.Projects.GetProjectsByDocument.GetProjectsByDocumentResponse();

            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetProjectsByDocument");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument) && string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.IdDocument) && !string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                Domain.Project[] responseProjects = null;

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.Signature))
                        {
                            documento = BusinessLogic.Documenti.DocManager.GetDettaglioBySignature(request.Signature, infoUtente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                if (documento != null)
                {
                    ArrayList fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDoc(infoUtente, documento.systemId);
                    if (fascicoli != null && fascicoli.Count > 0)
                    {
                        int y = 0;
                        responseProjects = new Domain.Project[fascicoli.Count];
                        foreach (DocsPaVO.fascicolazione.Fascicolo fascTemp in fascicoli)
                        {
                            Domain.Project prj = Utils.GetProject(fascTemp, infoUtente);
                            responseProjects[y] = prj;
                            y++;
                        }
                    }
                    else
                    {
                        throw new PisException("PROJECTS_NOT_FOUND");
                    }
                }
                else
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                response.Projects = responseProjects;

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

        public static Services.Projects.GetProjectStateDiagram.GetProjectStateDiagramResponse GetProjectStateDiagram(Services.Projects.GetProjectStateDiagram.GetProjectStateDiagramRequest request)
        {
            Services.Projects.GetProjectStateDiagram.GetProjectStateDiagramResponse response = new Services.Projects.GetProjectStateDiagram.GetProjectStateDiagramResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetProjectStateDiagram");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) || !string.IsNullOrEmpty(request.CodeProject)) && !string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject)) || (string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject)))
                {
                    throw new PisException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }

                Domain.StateOfDiagram stateOfDiagramResponse = new Domain.StateOfDiagram();

                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdProject))
                    {
                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdProject, infoUtente);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.CodeProject) && (!string.IsNullOrEmpty(request.ClassificationSchemeId)))
                        {
                            DocsPaVO.fascicolazione.Fascicolo[] fascicoli = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurity(request.CodeProject, infoUtente.idAmministrazione, request.ClassificationSchemeId, false);
                            if (fascicoli != null && fascicoli.Length > 0)
                            {
                                fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicoli[0].systemID, infoUtente);
                            }
                            else
                            {
                                throw new PisException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PisException("PROJECT_NOT_FOUND");
                }

                if (fascicolo != null)
                {
                    fascicolo.template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFasc(fascicolo.systemID);
                    if (fascicolo.template != null)
                    {
                        int idDiagramma = 0;
                        DocsPaVO.DiagrammaStato.Stato stato = null;
                        DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                        idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociatoFasc(fascicolo.template.SYSTEM_ID.ToString());
                        if (idDiagramma != 0)
                        {
                            diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                            if (diagramma != null)
                            {
                                stato = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoFasc(fascicolo.systemID);
                                if (stato != null)
                                {
                                    stateOfDiagramResponse = Utils.GetStateOfDiagram(stato, diagramma.SYSTEM_ID.ToString());
                                }
                                else
                                {
                                    throw new PisException("FASC_STATEOFDIAGRAM_NOT_FOUND");
                                }
                            }
                        }
                        else
                        {
                            throw new PisException("DIAGRAM_NOT_FOUND");
                        }
                    }
                    else
                    {
                        throw new PisException("TEMPLATE_NOT_FOUND");
                    }
                }
                else
                {
                    throw new PisException("PROJECT_NOT_FOUND");
                }

                response.StateOfDiagram = stateOfDiagramResponse;

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

        public static Services.Projects.GetTemplatePrj.GetTemplatePrjResponse GetTemplatePrj(Services.Projects.GetTemplatePrj.GetTemplatePrjRequest request)
        {
            Services.Projects.GetTemplatePrj.GetTemplatePrjResponse response = new Services.Projects.GetTemplatePrj.GetTemplatePrjResponse();

            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetTemplatePrj");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.DescriptionTemplate) && string.IsNullOrEmpty(request.IdTemplate))
                {
                    throw new PisException("REQUIRED_CODE_OR_ID_TEMPLATE");
                }

                if (!string.IsNullOrEmpty(request.DescriptionTemplate) && !string.IsNullOrEmpty(request.IdTemplate))
                {
                    throw new PisException("REQUIRED_ONLY_CODE_OR_ID_TEMPLATE");
                }

                Domain.Template templateResponse = new Domain.Template();

                //DocsPaVO.utente.Registro registro = null;
                DocsPaVO.ProfilazioneDinamica.Templates template = null;

                if (!string.IsNullOrEmpty(request.DescriptionTemplate))
                {
                    // Causa errore con più template con stessa descrizione eseguo un fix
                    DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] listaTemplate = null;
                    listaTemplate = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getListTemplatesLiteByRole(infoUtente.idAmministrazione, infoUtente.idGruppo);

                    if (listaTemplate != null && listaTemplate.Length > 0)
                    {
                        foreach (DocsPaVO.ProfilazioneDinamicaLite.TemplateLite TL in listaTemplate)
                        {
                            if (request.DescriptionTemplate.ToUpper().Equals(TL.name.ToUpper()))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(TL.system_id);
                            }
                        }
                    }
                    //template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateByDescrizione(request.DescriptionTemplate);
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.IdTemplate))
                    {
                        try
                        {
                            template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(request.IdTemplate);
                        }
                        catch
                        {
                            //Template non trovato
                            throw new PisException("TEMPLATE_NOT_FOUND");
                        }
                    }
                }

                //Modifica Lembo 11-01-2013: Controllo se l'utente è abilitato all'utilizzo del template.
                // per ora facciamo visualizzare sia quelli con diritti di scrittura che diritti di visualizzazione.
                string controlFilter = string.Format("{0} and id_ruolo={1} and (diritti='1' or diritti='2')", template.SYSTEM_ID.ToString(), infoUtente.idGruppo);
                ArrayList controlloVisibilita = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getRuoliTipoFasc(controlFilter);
                if (controlloVisibilita == null || controlloVisibilita.Count == 0)
                {
                    throw new PisException("TEMPLATE_NOT_ROLE_VISIBLE");
                }

                if (template != null)
                {
                    templateResponse.Name = template.DESCRIZIONE;
                    templateResponse.Id = template.SYSTEM_ID.ToString();

                    DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                    template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom));

                    if (oggettiCustom != null && oggettiCustom.Length > 0)
                    {
                        templateResponse.Fields = new Domain.Field[oggettiCustom.Length];
                        int numField = 0;
                        //Modifica Lembo 11-01-2013: Prelevo i diritti per popolare l'attributo Rights
                        DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[] dirittiOggetti =
                            (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[])(BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getDirittiCampiTipologiaFasc(infoUtente.idGruppo, template.SYSTEM_ID.ToString())).ToArray(typeof(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli));
                    
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in oggettiCustom)
                        {

                            Domain.Field fieldTemp = new Domain.Field();

                            fieldTemp.Id = oggettoCustom.SYSTEM_ID.ToString();
                            fieldTemp.Name = oggettoCustom.DESCRIZIONE;
                            if (!string.IsNullOrEmpty(oggettoCustom.CAMPO_OBBLIGATORIO) && oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                            {
                                fieldTemp.Required = true;
                            }
                            else
                            {
                                fieldTemp.Required = false;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                            {
                                fieldTemp.Type = "MultipleChoise";
                                if (oggettoCustom.ELENCO_VALORI != null && oggettoCustom.ELENCO_VALORI.Count > 0)
                                {
                                    fieldTemp.MultipleChoice = new string[oggettoCustom.ELENCO_VALORI.Count];
                                    for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Count; i++)
                                    {
                                        fieldTemp.MultipleChoice[i] = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE;
                                    }
                                }
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Corrispondente"))
                            {
                                fieldTemp.Type = "Correspondent";
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CampoDiTesto"))
                            {
                                fieldTemp.Type = "TextField";
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("SelezioneEsclusiva"))
                            {
                                fieldTemp.Type = "ExclusiveSelection";
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("MenuATendina"))
                            {
                                fieldTemp.Type = "DropDown";
                                if (oggettoCustom.ELENCO_VALORI != null && oggettoCustom.ELENCO_VALORI.Count > 0)
                                {
                                    fieldTemp.MultipleChoice = new string[oggettoCustom.ELENCO_VALORI.Count];
                                    for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Count; i++)
                                    {
                                        fieldTemp.MultipleChoice[i] = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE;
                                    }
                                }
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore"))
                            {
                                fieldTemp.Type = "Counter";
                                if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                                {
                                    DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                    if (reg != null)
                                    {
                                        fieldTemp.CodeRegisterOrRF = reg.codRegistro;
                                    }
                                }
                                fieldTemp.CounterToTrigger = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Data"))
                            {
                                fieldTemp.Type = "Date";
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore"))
                            {
                                fieldTemp.Type = "SubCounter";
                                if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                                {
                                    DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                    if (reg != null)
                                    {
                                        fieldTemp.CodeRegisterOrRF = reg.codRegistro;
                                    }
                                }
                                fieldTemp.CounterToTrigger = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            //Modifica Lembo 11-01-2013: popolo l'attributo Rights
                            if (dirittiOggetti.FirstOrDefault(e => e.ID_OGGETTO_CUSTOM.ToUpperInvariant() == oggettoCustom.SYSTEM_ID.ToString().ToUpperInvariant()).INS_MOD_OGG_CUSTOM == "1")
                                fieldTemp.Rights = "INSERT_AND_MODIFY";
                            else if (dirittiOggetti.FirstOrDefault(e => e.ID_OGGETTO_CUSTOM.ToUpperInvariant() == oggettoCustom.SYSTEM_ID.ToString().ToUpperInvariant()).VIS_OGG_CUSTOM == "1")
                                fieldTemp.Rights = "SEARCH_AND_VIEW";
                            else
                                fieldTemp.Rights = "NONE";

                            templateResponse.Fields[numField] = fieldTemp;
                            numField++;
                        }
                    }

                    int idDiagramma = 0;
                    DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                    idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociatoFasc(template.SYSTEM_ID.ToString());
                    if (idDiagramma != 0)
                    {
                        diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                        if (diagramma != null)
                        {
                            Domain.StateDiagram responseDiagram = new Domain.StateDiagram();
                            responseDiagram.Description = diagramma.DESCRIZIONE;
                            responseDiagram.Id = diagramma.SYSTEM_ID.ToString();
                            int y = 0;
                            if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                            {
                                responseDiagram.StateOfDiagram = new Domain.StateOfDiagram[diagramma.STATI.Count];

                                foreach (DocsPaVO.DiagrammaStato.Stato statoD in diagramma.STATI)
                                {
                                    Domain.StateOfDiagram stateDiagram = new Domain.StateOfDiagram();
                                    stateDiagram.Description = statoD.DESCRIZIONE;
                                    stateDiagram.Id = statoD.SYSTEM_ID.ToString();
                                    stateDiagram.InitialState = statoD.STATO_INIZIALE;
                                    stateDiagram.FinaleState = statoD.STATO_FINALE;
                                    stateDiagram.DiagramId = responseDiagram.Id;
                                    responseDiagram.StateOfDiagram[y] = stateDiagram;
                                    y++;
                                }
                                templateResponse.StateDiagram = responseDiagram;
                            }
                        }
                    }
                }
                else
                {
                    //Template non trovato
                    throw new PisException("TEMPLATE_NOT_FOUND");
                }

                response.Template = templateResponse;

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

        public static Services.Projects.GetTemplatesProjects.GetTemplatesProjectsResponse GetTemplatesProjects(Services.Projects.GetTemplatesProjects.GetTemplatesProjectsRequest request)
        {
            Services.Projects.GetTemplatesProjects.GetTemplatesProjectsResponse response = new Services.Projects.GetTemplatesProjects.GetTemplatesProjectsResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetTemplatesProjects");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] listaTemplate = null;
                listaTemplate = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getListTemplatesLiteByRole(infoUtente.idAmministrazione,infoUtente.idGruppo);

                if (listaTemplate != null && listaTemplate.Length > 0)
                {
                    Domain.Template[] templateResponse = new Domain.Template[listaTemplate.Length];

                    for (int y = 0; y < listaTemplate.Length; y++)
                    {
                        Domain.Template tempTemp = new Domain.Template();
                        tempTemp.Name = listaTemplate[y].name;
                        tempTemp.Id = listaTemplate[y].system_id;
                        templateResponse[y] = tempTemp;
                    }

                    response.Templates = templateResponse;
                }
                else
                {
                    //Nessun template trovato
                    throw new PisException("TEMPLATES_NOT_FOUND");
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

        public static Services.Projects.OpenCloseProject.OpenCloseProjectResponse OpenCloseProject(Services.Projects.OpenCloseProject.OpenCloseProjectRequest request)
        {
            Services.Projects.OpenCloseProject.OpenCloseProjectResponse response = new Services.Projects.OpenCloseProject.OpenCloseProjectResponse();

            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "OpenCloseProject");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) || !string.IsNullOrEmpty(request.CodeProject)) && !string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject)) || (string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject)))
                {
                    throw new PisException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }

                if (string.IsNullOrEmpty(request.Action) || (!string.IsNullOrEmpty(request.Action) && (!request.Action.ToUpper().Equals("O") && !request.Action.ToUpper().Equals("C"))))
                {
                    throw new PisException("REQUIRED_OC_PROJECT");
                }

                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdProject))
                    {
                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdProject, infoUtente);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.CodeProject) && (!string.IsNullOrEmpty(request.ClassificationSchemeId)))
                        {
                            DocsPaVO.fascicolazione.Fascicolo[] fascicoli = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurity(request.CodeProject, infoUtente.idAmministrazione, request.ClassificationSchemeId, false);
                            if (fascicoli != null && fascicoli.Length > 0)
                            {
                                fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicoli[0].systemID, infoUtente);
                            }
                            else
                            {
                                throw new PisException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PisException("PROJECT_NOT_FOUND");
                }

                if (fascicolo != null)
                {
                    if (!string.IsNullOrEmpty(fascicolo.stato) && fascicolo.stato.Equals("A") && request.Action.ToUpper().Equals("O"))
                    {
                        throw new PisException("PROJECT_ALREADY_OPEN");
                    }

                    if (!string.IsNullOrEmpty(fascicolo.stato) && fascicolo.stato.Equals("C") && request.Action.ToUpper().Equals("C"))
                    {
                        throw new PisException("PROJECT_ALREADY_CLOSE");
                    }

                    DateTime ora = DateTime.Now;

                    if (request.Action.ToUpper().Equals("O"))
                    {
                        fascicolo.chiusura = "";
                        fascicolo.apertura = ora.ToString("dd/MM/yyyy");
                        fascicolo.stato = "A";
                    }
                    else
                    {
                        DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                        if (ruolo != null)
                        {
                            fascicolo.chiusura = ora.ToString("dd/MM/yyyy");
                            fascicolo.stato = "C";
                            fascicolo.chiudeFascicolo = new DocsPaVO.fascicolazione.ChiudeFascicolo();
                            fascicolo.chiudeFascicolo.idPeople = infoUtente.idPeople;
                            fascicolo.chiudeFascicolo.idCorrGlob_Ruolo = ruolo.systemId;
                            fascicolo.chiudeFascicolo.idCorrGlob_UO = ruolo.uo.systemId;
                        }
                    }
                    BusinessLogic.Fascicoli.FascicoloManager.setFascicolo(infoUtente, fascicolo);
                }
                else
                {
                    throw new PisException("PROJECT_NOT_FOUND");
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

        public static Services.Projects.SearchProjects.SearchProjectsResponse SearchProjects(Services.Projects.SearchProjects.SearchProjectsRequest request)
        {
            Services.Projects.SearchProjects.SearchProjectsResponse response = new Services.Projects.SearchProjects.SearchProjectsResponse();

            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "SearchProjects");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (request.Filters == null || request.Filters.Length == 0)
                {
                    throw new PisException("REQUIRED_FILTER");
                }

                //Chiamata al metodo CheckFilterType(request.Filters)
                Utils.CheckFilterTypes(request.Filters);

                DocsPaVO.utente.Registro registro = null;
                DocsPaVO.fascicolazione.Classificazione objClassificazione = null;

                //Converti i filtri di ricerca
                DocsPaVO.filtri.FiltroRicerca[][] qV = Utils.GetFiltersProjectsFromPis(request.Filters, infoUtente, out registro, out objClassificazione);

                Domain.Project[] responseProjects = null;

                int nRec = 0;
                int numTotPage = 0;
                bool allDocuments = false;
                int pageSize = 20;
                int numPage = 1;

                if (request.ElementsInPage == null && request.PageNumber == null)
                {
                    allDocuments = true;
                }
                else
                {
                    if ((request.ElementsInPage != null && request.PageNumber == null) || (request.ElementsInPage == null && request.PageNumber != null))
                    {
                        //Paginazione non valida
                        throw new PisException("INVALID_PAGINATION");
                    }
                    else
                    {
                        pageSize = request.ElementsInPage ?? 20;
                        numPage = request.PageNumber ?? 1;
                    }
                }

                List<DocsPaVO.ricerche.SearchResultInfo> idProfileList = null;
                ArrayList objListaInfoFascicoli = null;
                #region Estrazione template da ricerca con filtro TEMPLATE_EXTRACTION
                // Tentativo di estrazione template per ricerca PIS
                bool estrazioneTemplate = false;
                Domain.Filter filtroTemplate = (from filtro in request.Filters where (filtro != null && !string.IsNullOrEmpty(filtro.Name) && filtro.Name.ToUpper() == "TEMPLATE") select filtro).FirstOrDefault();
                List<DocsPaVO.Grid.Field> visibilityFields = new List<DocsPaVO.Grid.Field>();
                if (filtroTemplate != null)
                {
                    Domain.Filter filtroEstrazione = (from filtro in request.Filters where (filtro != null && !string.IsNullOrEmpty(filtro.Name) && filtro.Name.ToUpper() == "TEMPLATE_EXTRACTION") select filtro).FirstOrDefault();
                    if (filtroEstrazione != null && filtroEstrazione.Value.ToUpper() == "TRUE")
                    {
                        estrazioneTemplate = true;
                        DocsPaVO.Grid.Field x1 = new DocsPaVO.Grid.Field();
                        DocsPaVO.ProfilazioneDinamica.Templates template = null;
                        if (filtroTemplate != null)
                        {
                            int idTplTemp = 0;
                            if (Int32.TryParse(filtroTemplate.Value, out idTplTemp))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(idTplTemp.ToString());

                            }
                            else if (filtroTemplate.Template != null && !string.IsNullOrEmpty(filtroTemplate.Template.Id))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(filtroTemplate.Template.Id);
                            }
                            else
                            {
                                if (filtroTemplate.Template != null && !string.IsNullOrEmpty(filtroTemplate.Template.Name))
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateByDescrizione(filtroTemplate.Template.Name);
                                }
                            }
                        }
                        if (template != null)
                        {
                            DocsPaVO.Grid.Field campoProf = null;
                            int i = 0;
                            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in template.ELENCO_OGGETTI)
                            {
                                campoProf = new DocsPaVO.Grid.Field();
                                campoProf.AssociatedTemplateId = template.SYSTEM_ID.ToString();
                                campoProf.AssociatedTemplateName = template.DESCRIZIONE;
                                campoProf.CanAssumeMultiValues = false;
                                campoProf.CustomObjectId = ogg.SYSTEM_ID;
                                campoProf.FieldId = "T" + ogg.SYSTEM_ID;
                                campoProf.IsTruncable = true;
                                campoProf.Label = ogg.DESCRIZIONE;
                                campoProf.OriginalLabel = ogg.DESCRIZIONE;
                                campoProf.MaxLength = 1000000;
                                campoProf.OracleDbColumnName = ogg.SYSTEM_ID.ToString();
                                campoProf.SqlServerDbColumnName = ogg.SYSTEM_ID.ToString();
                                campoProf.Position = i;
                                i++;
                                campoProf.Visible = true;
                                campoProf.Width = 100;
                                visibilityFields.Add(campoProf);

                            }
                        }
                        else
                        {
                            throw new PisException("TEMPLATE_NOT_FOUND");
                        }

                    }
                }
                // fine estrazione template da ricerca
                #endregion
                if(!estrazioneTemplate)
                objListaInfoFascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPagingCustom(infoUtente, objClassificazione, registro, qV[0], false, false, false, out numTotPage, out nRec, numPage, pageSize, request.GetTotalProjectsNumber, out idProfileList, null, string.Empty, false, allDocuments, null, null, true);
                else
                    objListaInfoFascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPagingCustom(infoUtente, objClassificazione, registro, qV[0], false, false, false, out numTotPage, out nRec, numPage, pageSize, request.GetTotalProjectsNumber, out idProfileList, null, string.Empty, true, allDocuments, visibilityFields.ToArray(), null, true);
                

                if (objListaInfoFascicoli != null && objListaInfoFascicoli.Count > 0)
                {
                    responseProjects = new Domain.Project[objListaInfoFascicoli.Count];
                    int y = 0;
                    foreach (DocsPaVO.Grids.SearchObject obj in objListaInfoFascicoli)
                    {
                        responseProjects[y] = Utils.GetProjectFromSearchObject(obj,estrazioneTemplate,visibilityFields.ToArray());
                        y++;
                    }

                    if (request.GetTotalProjectsNumber && idProfileList != null)
                        response.TotalProjectsNumber = idProfileList.Count;
                }
                else
                {
                    //Documenti non trovati
                    //throw new PisException("PROJECTS_NOT_FOUND");

                    if (request.GetTotalProjectsNumber)
                        response.TotalProjectsNumber = 0;

                    responseProjects = new Domain.Project[0];
                }

                //if (request.GetTotalProjectsNumber && idProfileList != null)
                //    response.TotalProjectsNumber = idProfileList.Count;

                response.Projects = responseProjects;
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

        public static Services.Projects.GetProjectFilters.GetProjectFiltersResponse GetProjectFilters(Services.Projects.GetProjectFilters.GetProjectFiltersRequest request)
        {
            Services.Projects.GetProjectFilters.GetProjectFiltersResponse response = new Services.Projects.GetProjectFilters.GetProjectFiltersResponse();

            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetProjectFilters");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                List<Domain.Filter> listaFiltri = new List<Domain.Filter>();

                listaFiltri.Add(new Domain.Filter() { Name = "YEAR", Description = "Filtro per inserire l’anno", Type = Domain.FilterTypeEnum.Number });
                listaFiltri.Add(new Domain.Filter() { Name = "CREATION_DATE_FROM", Description = "Filtro utilizzato per intervallo su data creazione, questo valore è il limite inferiore", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "CREATION_DATE_TO", Description = "Filtro utilizzato per intervallo su data creazione, questo valore è il limite superiore", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "CLOSING_DATE_FROM", Description = "Filtro utilizzato per intervallo su data chiusura, questo valore è il limite inferiore", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "CLOSING_DATE_TO", Description = "Filtro utilizzato per intervallo su data chiusura, questo valore è il limite superiore", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "OPENING_DATE_FROM", Description = "Filtro utilizzato per intervallo su data apertura, questo valore è il limite inferiore", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "OPENING_DATE_TO", Description = "Filtro utilizzato per intervallo su data apertura, questo valore è il limite superiore", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "STATE", Description = "Filtro utilizzato per cercare il fascicolo in base allo stato di chiusura/apertura. Il valore del filtro è “O” per aperto e “C” per chiuso", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "TYPE_PROJECT", Description = "Filtro utilizzato per cercare il fascicolo in base al tipo procedimentale/generalre. Inserire “P” per procedimentale, “G” per generale", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "PROJECT_NUMBER", Description = "Filtro utilizzato per cercare il fascicolo per numero", Type = Domain.FilterTypeEnum.Number });
                listaFiltri.Add(new Domain.Filter() { Name = "PROJECT_DESCRIPTION", Description = "Filtro utilizzato per cercare i fascicoli per descrizione", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "TEMPLATE", Description = "Filtro che permette la ricerca per tipologia di fascicolo", Type = Domain.FilterTypeEnum.Number });
                listaFiltri.Add(new Domain.Filter() { Name = "PROJECT_CODE", Description = "Filtro che permette la ricerca per codice di fascicolo", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "CLASSIFICATION_SCHEME", Description = "Filtro che permette la ricerca per titolario", Type = Domain.FilterTypeEnum.Number });
                listaFiltri.Add(new Domain.Filter() { Name = "REGISTER", Description = "Filtro per la ricerca dei fascicoli in registro", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "CLASSIFICATION_CODE", Description = "Filtro per cercare per codice di classificazione", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "SUBPROJECT", Description = "Filtro per cercare un fascicolo tramite la descrizione di un suo sottofascicolo", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "TEMPLATE_EXTRACTION", Description = "Filtro utilizzato per l'estrazione dei campi profilati di una tipologia. Necessita della presenza del filtro Template.", Type = Domain.FilterTypeEnum.String });

                response.Filters = listaFiltri.ToArray();

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
        /// Servizio che permette alla coppia (ruolo,utente) di monitorare un fascicolo presente nel sistema.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Services.FollowDomainObject.FollowResponse FollowProject(Services.FollowDomainObject.FollowRequest request)
        {
            Services.FollowDomainObject.FollowResponse response = new Services.FollowDomainObject.FollowResponse();
            try
            {
                if (request.Operation == null || string.IsNullOrEmpty(request.CodeApplication) || string.IsNullOrEmpty(request.IdObject))
                {
                    throw new Exception("I campi Operation, CodeApplication, IdObject sono obbligatori.");
                }
                //Controllo se l'utente si è correttamente autenticatoInizio controllo autenticazione utente
                DocsPaVO.utente.InfoUtente userInfo = Utils.CheckAuthentication(request, "FollowProject");

                // Costruisco l'oggetto FollowDomainObject
                DocsPaVO.Notification.FollowDomainObject followDomainObject = new DocsPaVO.Notification.FollowDomainObject();
                switch (request.Operation)
                {
                    case Domain.OperationFollow.AddFolder:
                        followDomainObject.Operation = DocsPaVO.Notification.FollowDomainObject.OperationFollow.AddFolder;
                        break;
                    case Domain.OperationFollow.RemoveFolder:
                        followDomainObject.Operation = DocsPaVO.Notification.FollowDomainObject.OperationFollow.RemoveFolder;
                        break;
                    default:
                        throw new Exception("Il campo operazione deve essere AddFolder o RemoveFolder");
                }
                followDomainObject.App = request.CodeApplication;
                followDomainObject.IdAmm = userInfo.idAmministrazione;
                followDomainObject.IdProject = request.IdObject;
                followDomainObject.IdPeople = userInfo.idPeople;
                followDomainObject.IdGroup = userInfo.idGruppo;

                //eseguo l'operazione di monitora/non monitorare più il fascicolo.
                BusinessLogic.ServiceNotifications.Notification.Follow(followDomainObject);
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

        public static Services.Projects.GetLinkPrjByID.GetLinkPrjByIDResponse GetLinkPrjByID(Services.Projects.GetLinkPrjByID.GetLinkPrjByIDRequest request)
        {
            Services.Projects.GetLinkPrjByID.GetLinkPrjByIDResponse response = new Services.Projects.GetLinkPrjByID.GetLinkPrjByIDResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                response.Success = false;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetLinkPrjByID");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) || !string.IsNullOrEmpty(request.CodeProject)) && !string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject)) || (string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject)))
                {
                    throw new PisException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }

                Domain.Project responseProject = new Domain.Project();

                DocsPaVO.fascicolazione.Fascicolo fascicolo = null;

                try
                {
                    if (!string.IsNullOrEmpty(request.IdProject))
                    {
                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdProject, infoUtente);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.CodeProject) && (!string.IsNullOrEmpty(request.ClassificationSchemeId)))
                        {
                            DocsPaVO.fascicolazione.Fascicolo[] fascicoli = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurity(request.CodeProject, infoUtente.idAmministrazione, request.ClassificationSchemeId, false);
                            if (fascicoli != null && fascicoli.Length > 0)
                            {
                                fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicoli[0].systemID, infoUtente);
                                if (fascicolo == null && fascicoli.Length > 1)
                                {
                                    fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicoli[1].systemID, infoUtente);
                                }
                            }
                            else
                            {
                                throw new PisException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PisException("PROJECT_NOT_FOUND");
                }

                if (fascicolo != null)
                {
                    string pathFE = "";
                    if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                        pathFE = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();

                    if (!string.IsNullOrEmpty(pathFE))
                    {
                        response.ProjectLink = string.Format("{0}VisualizzaOggetto.htm?idAmministrazione={1}&tipoOggetto=F&idObj={2}", pathFE, infoUtente.idAmministrazione, fascicolo.codice);
                        response.Success = true;
                    }
                    else
                    {
                        throw new Exception("Path del frontend non disponibile: impossibile generare il link.");
                    }
                }
                else
                {
                    throw new PisException("PROJECT_NOT_FOUND");
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

        public static Services.Projects.RemoveProjectFolders.RemoveProjectFoldersResponse RemoveProjectFolder(Services.Projects.RemoveProjectFolders.RemoveProjectFoldersRequest request)
        {

            Services.Projects.RemoveProjectFolders.RemoveProjectFoldersResponse response = new Services.Projects.RemoveProjectFolders.RemoveProjectFoldersResponse();
            try
            {
                response.Success = true;
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "RemoveProjectFolder");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente


                if (string.IsNullOrEmpty(request.Folder.Id))
                {
                    throw new PisException("REQUIRED_IDPROJECT");
                }


                //In effetti basta il systemID
                DocsPaVO.fascicolazione.Folder folderToDelete = new DocsPaVO.fascicolazione.Folder
                {
                    systemID = request.Folder.Id,
                    idParent = request.Folder.IdParent,
                    idFascicolo = request.Folder.IdProject,
                    descrizione = request.Folder.Description,
                    dtaApertura = request.Folder.CreationDate
                };

                BusinessLogic.Fascicoli.FolderManager.delFolder(folderToDelete, infoUtente);

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

        #region Servizi sviluppati per MIT
        public static Services.Projects.CreateFolder.CreateFolderResponse CreateFolder(Services.Projects.CreateFolder.CreateFolderRequest request)
        {
            Services.Projects.CreateFolder.CreateFolderResponse response = new Services.Projects.CreateFolder.CreateFolderResponse();
            try
            {
                response.Success = true;
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "CreateFolder");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) || !string.IsNullOrEmpty(request.CodeProject)) && !string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject)) || (string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject)))
                {
                    throw new PisException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }

                if (string.IsNullOrEmpty(request.FolderDescription))
                {
                    throw new Exception("Descrizione della cartella obbligatoria");
                }


                if (!string.IsNullOrEmpty(request.IdParentFolder))
                {
                    int temp1 = 0;
                    if (!Int32.TryParse(request.IdParentFolder, out temp1))
                    {
                        throw new Exception("IdParentFolder non è inserito nel formato corretto");
                    }
                }

                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdProject))
                    {
                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdProject, infoUtente);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.CodeProject) && (!string.IsNullOrEmpty(request.ClassificationSchemeId)))
                        {
                            DocsPaVO.fascicolazione.Fascicolo[] fascicoli = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurity(request.CodeProject, infoUtente.idAmministrazione, request.ClassificationSchemeId, false);
                            if (fascicoli != null && fascicoli.Length > 0)
                            {
                                fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicoli[0].systemID, infoUtente);
                            }
                            else
                            {
                                throw new PisException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    throw new PisException("PROJECT_NOT_FOUND");
                }

                string idParentFolder = "";
                if (string.IsNullOrEmpty(request.IdParentFolder))
                {

                    DocsPaVO.fascicolazione.Folder folder1 = BusinessLogic.Fascicoli.FolderManager.getFolderByIdFascicolo(infoUtente.idPeople, infoUtente.idGruppo, fascicolo);
                    //BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                    idParentFolder = folder1.systemID;
                }
                else
                {
                    idParentFolder = request.IdParentFolder;
                }
                DocsPaVO.fascicolazione.ResultCreazioneFolder resCrFldr = new DocsPaVO.fascicolazione.ResultCreazioneFolder();
                DocsPaVO.fascicolazione.Folder newFolder = new DocsPaVO.fascicolazione.Folder();
                newFolder.descrizione = request.FolderDescription;
                newFolder.idFascicolo = fascicolo.systemID;
                newFolder.idParent = idParentFolder;

                DocsPaVO.fascicolazione.Folder cartella = BusinessLogic.Fascicoli.FolderManager.newFolder(newFolder, infoUtente, BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo), out resCrFldr);
                //if (resCrFldr == DocsPaVO.fascicolazione.ResultCreazioneFolder.DM_ERROR)
                //{
                //    throw new Exception("Errore nella creazione della cartella");
                //}

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
                response.Result = "Creazione della cartella effettuata.";
                Domain.Folder f1 = new Domain.Folder();
                f1.Description = cartella.descrizione;
                f1.Id = cartella.systemID;
                f1.IdParent = cartella.idParent;
                f1.IdProject = cartella.idFascicolo;
                f1.CreationDate = cartella.dtaApertura;
                response.Folder = f1;
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

        public static VtDocsWS.Services.Projects.GetProjectFolders.GetProjectFoldersResponse GetProjectFolders(VtDocsWS.Services.Projects.GetProjectFolders.GetProjectFoldersRequest request)
        {
            VtDocsWS.Services.Projects.GetProjectFolders.GetProjectFoldersResponse response = new Services.Projects.GetProjectFolders.GetProjectFoldersResponse();
            try
            {
                response.Success = true;
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetProjectFolders");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) || !string.IsNullOrEmpty(request.CodeProject)) && !string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject)) || (string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject)))
                {
                    throw new PisException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }
                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdProject))
                    {
                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdProject, infoUtente);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.CodeProject) && (!string.IsNullOrEmpty(request.ClassificationSchemeId)))
                        {
                            DocsPaVO.fascicolazione.Fascicolo[] fascicoli = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurity(request.CodeProject, infoUtente.idAmministrazione, request.ClassificationSchemeId, false);
                            if (fascicoli != null && fascicoli.Length > 0)
                            {
                                fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicoli[0].systemID, infoUtente);
                            }
                            else
                            {
                                throw new PisException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    throw new PisException("PROJECT_NOT_FOUND");
                }

                response.IdProject = fascicolo.systemID;
                response.ProjectDescription = fascicolo.descrizione;
                ArrayList cartelle = BusinessLogic.Fascicoli.FascicoloManager.getListaFolderDaIdFascicolo(infoUtente, fascicolo.systemID, null, false, false);
                Domain.Folder[] cartellePIS = new Domain.Folder[cartelle.Count];
                int i = 0;
                foreach (DocsPaVO.fascicolazione.Folder f in cartelle)
                {
                    Domain.Folder f1 = new Domain.Folder();
                    f1.Description = f.descrizione;
                    f1.Id = f.systemID;
                    f1.IdParent = f.idParent;
                    f1.IdProject = f.idFascicolo;
                    f1.CreationDate = f.dtaApertura;
                    cartellePIS[i] = f1;
                    i++;
                    //if (f1.IdProject == f1.IdParent)
                    //{
                    //    response.MainFolder = f1;
                    //}
                }

                //if (response.MainFolder != null)
                //{
                if (cartellePIS.Length > 1)
                {
                    response.Folders = cartellePIS;
                }
                //}
                else
                {
                    throw new Exception("Errore nel prelievo delle cartelle");
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
        #endregion
    }
}