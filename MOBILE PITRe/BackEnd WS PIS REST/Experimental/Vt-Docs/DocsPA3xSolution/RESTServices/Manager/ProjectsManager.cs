using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using System.Threading.Tasks;
using RESTServices.Model.Domain;
using RESTServices.Model.Responses;
using RESTServices.Model.Requests;
using System.Collections;

namespace RESTServices.Manager
{
    public class ProjectsManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(ProjectsManager));

        public static async Task<GetFiltersResponse> getProjectFilters(string token)
        {
            logger.Info("begin getProjectFilters");
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion

                List<Filter> listaFiltri = new List<Filter>();

                listaFiltri.Add(new Filter() { Name = "YEAR", Description = "Filtro per inserire l’anno", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "CREATION_DATE_FROM", Description = "Filtro utilizzato per intervallo su data creazione, questo valore è il limite inferiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "CREATION_DATE_TO", Description = "Filtro utilizzato per intervallo su data creazione, questo valore è il limite superiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "CLOSING_DATE_FROM", Description = "Filtro utilizzato per intervallo su data chiusura, questo valore è il limite inferiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "CLOSING_DATE_TO", Description = "Filtro utilizzato per intervallo su data chiusura, questo valore è il limite superiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "OPENING_DATE_FROM", Description = "Filtro utilizzato per intervallo su data apertura, questo valore è il limite inferiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "OPENING_DATE_TO", Description = "Filtro utilizzato per intervallo su data apertura, questo valore è il limite superiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "STATE", Description = "Filtro utilizzato per cercare il fascicolo in base allo stato di chiusura/apertura. Il valore del filtro è “O” per aperto e “C” per chiuso", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "TYPE_PROJECT", Description = "Filtro utilizzato per cercare il fascicolo in base al tipo procedimentale/generalre. Inserire “P” per procedimentale, “G” per generale", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "PROJECT_NUMBER", Description = "Filtro utilizzato per cercare il fascicolo per numero", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "PROJECT_DESCRIPTION", Description = "Filtro utilizzato per cercare i fascicoli per descrizione", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "TEMPLATE", Description = "Filtro che permette la ricerca per tipologia di fascicolo", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "PROJECT_CODE", Description = "Filtro che permette la ricerca per codice di fascicolo", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "CLASSIFICATION_SCHEME", Description = "Filtro che permette la ricerca per titolario", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "REGISTER", Description = "Filtro per la ricerca dei fascicoli in registro", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "CLASSIFICATION_CODE", Description = "Filtro per cercare per codice di classificazione", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "SUBPROJECT", Description = "Filtro per cercare un fascicolo tramite la descrizione di un suo sottofascicolo", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "TEMPLATE_EXTRACTION", Description = "Filtro utilizzato per l'estrazione dei campi profilati di una tipologia. Necessita della presenza del filtro Template.", Type = FilterTypeEnum.String });

                GetFiltersResponse response = new GetFiltersResponse();
                response.Filters = listaFiltri;
                response.Code = GetFiltersResponseCode.OK;
                logger.Info("END - getProjectFilters");
                return response;

            }
            catch (Exception e)
            {
                logger.Error("eccezione getProjectFilters: " + e);
                GetFiltersResponse errorResp = new GetFiltersResponse();
                errorResp.Code = GetFiltersResponseCode.SYSTEM_ERROR;
                errorResp.ErrorMessage = e.Message;
                return errorResp;
            }
        }

        public static async Task<GetProjectResponse> getProject(string token, string idProject, string codeProject, string classificationSchemeId)
        {
            logger.Info("begin getProject");
            GetProjectResponse response = new GetProjectResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion

                if (string.IsNullOrEmpty(classificationSchemeId) && string.IsNullOrEmpty(codeProject) && string.IsNullOrEmpty(idProject))
                {
                    throw new RestException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(classificationSchemeId) || !string.IsNullOrEmpty(codeProject)) && !string.IsNullOrEmpty(idProject))
                {
                    throw new RestException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(classificationSchemeId) && string.IsNullOrEmpty(codeProject)) || (string.IsNullOrEmpty(classificationSchemeId) && !string.IsNullOrEmpty(codeProject)))
                {
                    throw new RestException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }

                Project responseProject = new Project();

                DocsPaVO.fascicolazione.Fascicolo fascicolo = null;

                try
                {
                    if (!string.IsNullOrEmpty(idProject))
                    {
                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(idProject, infoUtente);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(codeProject) && (!string.IsNullOrEmpty(classificationSchemeId)))
                        {
                            DocsPaVO.fascicolazione.Fascicolo[] fascicoli = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurity(codeProject, infoUtente.idAmministrazione, classificationSchemeId, false);
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
                                throw new RestException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }

                if (fascicolo != null)
                {
                    responseProject = Utils.GetProject(fascicolo, infoUtente);
                    if (responseProject != null)
                    {
                        response.Project = responseProject;
                        response.Code = GetProjectResponseCode.OK;
                    }
                    else
                    {
                        throw new RestException("PROJECT_NOT_FOUND");
                    }
                }
                else
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }

                logger.Info("end getProject");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getProject: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetProjectResponse();
                response.Code = GetProjectResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getProject: " + e);
                response = new GetProjectResponse();
                response.Code = GetProjectResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetProjectResponse> createProject(string token, CreateProjectRequest request)
        {
            logger.Info("begin createProject");
            GetProjectResponse response = new GetProjectResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion
                bool setStatoDiagrammaIniziale = false;
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                DocsPaVO.DiagrammaStato.Stato statoIniziale = null;
                if (request.Project == null)
                {
                    throw new RestException("REQUIRED_PROJECT");
                }

                if (request.Project == null || string.IsNullOrEmpty(request.Project.CodeNodeClassification))
                {
                    throw new RestException("REQUIRED_GENERAL_NODE");
                }

                if (request.Project.ClassificationScheme == null || string.IsNullOrEmpty(request.Project.ClassificationScheme.Id))
                {
                    throw new RestException("ID_CLASSIFICATIONSCHEME_REQUIRED");
                }

                DocsPaVO.amministrazione.OrgTitolario titolario = null;

                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                //Reperimento ruolo utente
                
                try
                {
                    titolario = BusinessLogic.Amministrazione.TitolarioManager.getTitolarioById(request.Project.ClassificationScheme.Id);
                }
                catch
                {
                    //Titolario non trovati
                    throw new RestException("CLASSIFICATION_NOT_FOUND");
                }

                //Reperimento del nodo di titolario
                DocsPaVO.fascicolazione.Classificazione classificazione = Utils.GetClassificazione(infoUtente, request.Project.CodeNodeClassification);

                if (classificazione == null || titolario == null)
                {
                    if (classificazione == null)
                    {
                        throw new RestException("GENERAL_NODE_NOT_FOUND");
                    }
                    else
                    {
                        //Titolario non trovati
                        throw new RestException("CLASSIFICATION_NOT_FOUND");
                    }
                }
                else
                {
                    try
                    {
                        if (classificazione.registro != null)
                        {
                            request.Project.Register = new Register();
                            request.Project.Register.Id = classificazione.registro.systemId;
                        }
                        fascicolo = Utils.GetProjectFromPis(request.Project, infoUtente);

                        if (request.Project.Register != null)
                            fascicolo.codUltimo = BusinessLogic.Fascicoli.FascicoloManager.getFascNumRif(classificazione.systemID, request.Project.Register.Id);
                        else
                            fascicolo.codUltimo = BusinessLogic.Fascicoli.FascicoloManager.getFascNumRif(classificazione.systemID, null);

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
                                throw new RestException("TEMPLATE_NOT_ROLE_EDITABLE");
                            }

                            if (template != null)
                            {
                                // Modifica Lembo 11-01-2013: Utilizzo il metodo che controlla la visibilità dei campi.
                                //fascicolo.template = Utils.GetTemplateFromPis(request.Project.Template, template, false);
                                fascicolo.template = Utils.GetTemplateFromPisVisibility(request.Project.Template, template, false, infoUtente.idGruppo, "P", "");
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
                                throw new RestException("TEMPLATE_NOT_FOUND");
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
                            throw new RestException("ERROR_PROJECT");
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
                        throw new RestException("ERROR_PROJECT");
                    }
                }
                response.Code = GetProjectResponseCode.OK;


                logger.Info("end createProject");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione createProject: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetProjectResponse();
                response.Code = GetProjectResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione createProject: " + e);
                response = new GetProjectResponse();
                response.Code = GetProjectResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetProjectResponse> editProject(string token, CreateProjectRequest request)
        {
            logger.Info("begin editProject");
            GetProjectResponse response = new GetProjectResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion
                if (request.Project == null)
                {
                    throw new RestException("REQUIRED_PROJECT");
                }

                Project responseProject = new Project();

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
                                throw new RestException("TEMPLATE_NOT_ROLE_EDITABLE");
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
                                fascicolo.template = Utils.GetTemplateFromPisVisibility(request.Project.Template, template, false, infoUtente.idGruppo, "P", "");
                                //fascicolo.tipo = template.SYSTEM_ID.ToString();

                            }
                            else
                            {
                                //Template non trovato
                                throw new RestException("TEMPLATE_NOT_FOUND");
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
                        throw new RestException("PROJECT_NOT_FOUND");
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
                        throw new RestException("PROJECT_NOT_FOUND");
                    }
                }
                else
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }


                response.Code = GetProjectResponseCode.OK;
                logger.Info("end editProject");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione editProject: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetProjectResponse();
                response.Code = GetProjectResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione editProject: " + e);
                response = new GetProjectResponse();
                response.Code = GetProjectResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<SearchProjectsResponse> searchProjects(string token, SearchRequest request)
        {
            logger.Info("begin searchProjects");
            SearchProjectsResponse response = new SearchProjectsResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion
                if (request.Filters == null || request.Filters.Length == 0)
                {
                    throw new RestException("REQUIRED_FILTER");
                }

                //Chiamata al metodo CheckFilterType(request.Filters)
                Utils.CheckFilterTypes(request.Filters);

                DocsPaVO.utente.Registro registro = null;
                DocsPaVO.fascicolazione.Classificazione objClassificazione = null;

                //Converti i filtri di ricerca
                DocsPaVO.filtri.FiltroRicerca[][] qV = Utils.GetFiltersProjectsFromPis(request.Filters, infoUtente, out registro, out objClassificazione);

                Project[] responseProjects = null;

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
                        throw new RestException("INVALID_PAGINATION");
                    }
                    else
                    {
                        pageSize = request.ElementsInPage == null ? 20: request.ElementsInPage;
                        numPage = request.PageNumber== null ? 1 : request.PageNumber;
                    }
                }

                List<DocsPaVO.ricerche.SearchResultInfo> idProfileList = null;
                ArrayList objListaInfoFascicoli = null;
                #region Estrazione template da ricerca con filtro TEMPLATE_EXTRACTION
                // Tentativo di estrazione template per ricerca PIS
                bool estrazioneTemplate = false;
                Filter filtroTemplate = (from filtro in request.Filters where (filtro != null && !string.IsNullOrEmpty(filtro.Name) && filtro.Name.ToUpper() == "TEMPLATE") select filtro).FirstOrDefault();
                List<DocsPaVO.Grid.Field> visibilityFields = new List<DocsPaVO.Grid.Field>();
                if (filtroTemplate != null)
                {
                    Filter filtroEstrazione = (from filtro in request.Filters where (filtro != null && !string.IsNullOrEmpty(filtro.Name) && filtro.Name.ToUpper() == "TEMPLATE_EXTRACTION") select filtro).FirstOrDefault();
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
                            throw new RestException("TEMPLATE_NOT_FOUND");
                        }

                    }
                }
                // fine estrazione template da ricerca
                #endregion
                if (!estrazioneTemplate)
                    objListaInfoFascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPagingCustom(infoUtente, objClassificazione, registro, qV[0], false, false, false, out numTotPage, out nRec, numPage, pageSize, true, out idProfileList, null, string.Empty, false, allDocuments, null, null, true);
                else
                    objListaInfoFascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPagingCustom(infoUtente, objClassificazione, registro, qV[0], false, false, false, out numTotPage, out nRec, numPage, pageSize, true, out idProfileList, null, string.Empty, true, allDocuments, visibilityFields.ToArray(), null, true);


                if (objListaInfoFascicoli != null && objListaInfoFascicoli.Count > 0)
                {
                    responseProjects = new Project[objListaInfoFascicoli.Count];
                    int y = 0;
                    foreach (DocsPaVO.Grids.SearchObject obj in objListaInfoFascicoli)
                    {
                        responseProjects[y] = Utils.GetProjectFromSearchObject(obj, estrazioneTemplate, visibilityFields.ToArray());
                        y++;
                    }

                    if (idProfileList != null)
                        response.TotalProjectsNumber = idProfileList.Count;
                }
                else
                {
                    //Documenti non trovati
                    //throw new PisException("PROJECTS_NOT_FOUND");

                   response.TotalProjectsNumber = 0;

                    responseProjects = new Project[0];
                }

                //if (request.GetTotalProjectsNumber && idProfileList != null)
                //    response.TotalProjectsNumber = idProfileList.Count;

                response.Projects = responseProjects;

                response.Code = SearchProjectsResponseCode.OK;
                logger.Info("end searchProjects");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione searchProjects: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new SearchProjectsResponse();
                response.Code = SearchProjectsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione searchProjects: " + e);
                response = new SearchProjectsResponse();
                response.Code = SearchProjectsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetTemplatesResponse> getProjectTemplates(string token)
        {
            logger.Info("begin getProjectTemplates");
            GetTemplatesResponse response = new GetTemplatesResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion
                DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] listaTemplate = null;
                listaTemplate = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getListTemplatesLiteByRole(infoUtente.idAmministrazione, infoUtente.idGruppo);

                if (listaTemplate != null && listaTemplate.Length > 0)
                {
                    Template[] templateResponse = new Template[listaTemplate.Length];

                    for (int y = 0; y < listaTemplate.Length; y++)
                    {
                        Template tempTemp = new Template();
                        tempTemp.Name = listaTemplate[y].name;
                        tempTemp.Id = listaTemplate[y].system_id;
                        templateResponse[y] = tempTemp;
                    }

                    response.Templates = templateResponse;
                }
                else
                {
                    //Nessun template trovato
                    throw new RestException("TEMPLATES_NOT_FOUND");
                }

                response.Code = GetTemplatesResponseCode.OK;

                logger.Info("end getProjectTemplates");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getProjectTemplates: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetTemplatesResponse();
                response.Code = GetTemplatesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getProjectTemplates: " + e);
                response = new GetTemplatesResponse();
                response.Code = GetTemplatesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetTemplateResponse> getTemplateProject(string token, string descriptionTemplate, string idTemplate)
        {
            logger.Info("begin getTemplateProject");
            GetTemplateResponse response = new GetTemplateResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion
                if (string.IsNullOrEmpty(descriptionTemplate) && string.IsNullOrEmpty(idTemplate))
                {
                    throw new RestException("REQUIRED_CODE_OR_ID_TEMPLATE");
                }

                if (!string.IsNullOrEmpty(descriptionTemplate) && !string.IsNullOrEmpty(idTemplate))
                {
                    throw new RestException("REQUIRED_ONLY_CODE_OR_ID_TEMPLATE");
                }

                Template templateResponse = new Template();

                //DocsPaVO.utente.Registro registro = null;
                DocsPaVO.ProfilazioneDinamica.Templates template = null;

                if (!string.IsNullOrEmpty(descriptionTemplate))
                {
                    // Causa errore con più template con stessa descrizione eseguo un fix
                    DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] listaTemplate = null;
                    listaTemplate = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getListTemplatesLiteByRole(infoUtente.idAmministrazione, infoUtente.idGruppo);

                    if (listaTemplate != null && listaTemplate.Length > 0)
                    {
                        foreach (DocsPaVO.ProfilazioneDinamicaLite.TemplateLite TL in listaTemplate)
                        {
                            if (descriptionTemplate.ToUpper().Equals(TL.name.ToUpper()))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(TL.system_id);
                            }
                        }
                    }
                    //template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateByDescrizione(request.DescriptionTemplate);
                }
                else
                {
                    if (!string.IsNullOrEmpty(idTemplate))
                    {
                        try
                        {
                            template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(idTemplate);
                        }
                        catch
                        {
                            //Template non trovato
                            throw new RestException("TEMPLATE_NOT_FOUND");
                        }
                    }
                }

                //Modifica Lembo 11-01-2013: Controllo se l'utente è abilitato all'utilizzo del template.
                // per ora facciamo visualizzare sia quelli con diritti di scrittura che diritti di visualizzazione.
                string controlFilter = string.Format("{0} and id_ruolo={1} and (diritti='1' or diritti='2')", template.SYSTEM_ID.ToString(), infoUtente.idGruppo);
                ArrayList controlloVisibilita = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getRuoliTipoFasc(controlFilter);
                if (controlloVisibilita == null || controlloVisibilita.Count == 0)
                {
                    throw new RestException("TEMPLATE_NOT_ROLE_VISIBLE");
                }

                if (template != null)
                {
                    templateResponse.Name = template.DESCRIZIONE;
                    templateResponse.Id = template.SYSTEM_ID.ToString();

                    DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                    template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom));

                    if (oggettiCustom != null && oggettiCustom.Length > 0)
                    {
                        templateResponse.Fields = new Field[oggettiCustom.Length];
                        int numField = 0;
                        //Modifica Lembo 11-01-2013: Prelevo i diritti per popolare l'attributo Rights
                        DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[] dirittiOggetti =
                            (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[])(BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getDirittiCampiTipologiaFasc(infoUtente.idGruppo, template.SYSTEM_ID.ToString())).ToArray(typeof(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli));

                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in oggettiCustom)
                        {

                            Field fieldTemp = new Field();

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
                            StateDiagram responseDiagram = new StateDiagram();
                            responseDiagram.Description = diagramma.DESCRIZIONE;
                            responseDiagram.Id = diagramma.SYSTEM_ID.ToString();
                            int y = 0;
                            if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                            {
                                responseDiagram.StateOfDiagram = new StateOfDiagram[diagramma.STATI.Count];

                                foreach (DocsPaVO.DiagrammaStato.Stato statoD in diagramma.STATI)
                                {
                                    StateOfDiagram stateDiagram = new StateOfDiagram();
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
                    throw new RestException("TEMPLATE_NOT_FOUND");
                }

                response.Template = templateResponse;
                response.Code = GetTemplateResponseCode.OK;
                logger.Info("end getTemplateProject");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getTemplateProject: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetTemplateResponse();
                response.Code = GetTemplateResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getTemplateProject: " + e);
                response = new GetTemplateResponse();
                response.Code = GetTemplateResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<SearchProjectsResponse> getProjectsByDocument(string token, string idDocument, string signature)
        {
            logger.Info("begin getProjectsByDocument");
            SearchProjectsResponse response = new SearchProjectsResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion

                if (string.IsNullOrEmpty(idDocument) && string.IsNullOrEmpty(signature))
                {
                    throw new RestException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(idDocument) && !string.IsNullOrEmpty(signature))
                {
                    throw new RestException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                Project[] responseProjects = null;

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(idDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idDocument, idDocument);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(signature))
                        {
                            documento = BusinessLogic.Documenti.DocManager.GetDettaglioBySignature(signature, infoUtente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                if (documento != null)
                {
                    ArrayList fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDoc(infoUtente, documento.systemId);
                    if (fascicoli != null && fascicoli.Count > 0)
                    {
                        int y = 0;
                        responseProjects = new Project[fascicoli.Count];
                        foreach (DocsPaVO.fascicolazione.Fascicolo fascTemp in fascicoli)
                        {
                            Project prj = Utils.GetProject(fascTemp, infoUtente);
                            responseProjects[y] = prj;
                            y++;
                        }
                        response.TotalProjectsNumber = fascicoli.Count;
                    }
                    else
                    {
                        throw new RestException("PROJECTS_NOT_FOUND");
                    }
                }
                else
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                response.Projects = responseProjects;
                response.Code = SearchProjectsResponseCode.OK;

                logger.Info("end getProjectsByDocument");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getProjectsByDocument: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new SearchProjectsResponse();
                response.Code = SearchProjectsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getProjectsByDocument: " + e);
                response = new SearchProjectsResponse();
                response.Code = SearchProjectsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<MessageResponse> editPrjStateDiagram(string token, string stateOfDiagram, string idProject, string classificationSchemeId, string codeProject)
        {
            logger.Info("begin editPrjStateDiagram");
            MessageResponse response = new MessageResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion
                if (string.IsNullOrEmpty(classificationSchemeId) && string.IsNullOrEmpty(codeProject) && string.IsNullOrEmpty(idProject))
                {
                    throw new RestException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(classificationSchemeId) || !string.IsNullOrEmpty(codeProject)) && !string.IsNullOrEmpty(idProject))
                {
                    throw new RestException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(classificationSchemeId) && string.IsNullOrEmpty(codeProject)) || (string.IsNullOrEmpty(classificationSchemeId) && !string.IsNullOrEmpty(codeProject)))
                {
                    throw new RestException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }

                if (string.IsNullOrEmpty(stateOfDiagram))
                {
                    throw new RestException("REQUIRED_STATE_OF_DIAGRAM");
                }

                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                try
                {
                    if (!string.IsNullOrEmpty(idProject))
                    {
                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(idProject, infoUtente);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(codeProject) && (!string.IsNullOrEmpty(classificationSchemeId)))
                        {
                            DocsPaVO.fascicolazione.Fascicolo[] fascicoli = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurity(codeProject, infoUtente.idAmministrazione, classificationSchemeId, false);
                            if (fascicoli != null && fascicoli.Length > 0)
                            {
                                fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicoli[0].systemID, infoUtente);
                            }
                            else
                            {
                                throw new RestException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("PROJECT_NOT_FOUND");
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
                                            if (stato.DESCRIZIONE.ToUpper().Equals(stateOfDiagram.ToUpper()))
                                            {
                                                BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStatoFasc(fascicolo.systemID, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                                result = true;
                                                response.ResultMessage = string.Format("Fascicolo {0} passato nello stato {1}",fascicolo.codice, stateOfDiagram);
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
                                                    if (((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).DESCRIZIONE.ToUpper().Equals(stateOfDiagram.ToUpper()))
                                                    {
                                                        BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStatoFasc(fascicolo.systemID, ((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                                        result = true;
                                                        response.ResultMessage = string.Format("Fascicolo {0} passato nello stato {1}", fascicolo.codice, stateOfDiagram);
                                                
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (!result)
                                    {
                                        throw new RestException("STATEOFDIAGRAM_NOT_FOUND");
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new RestException("DIAGRAM_NOT_FOUND");
                        }
                    }
                    else
                    {
                        throw new RestException("TEMPLATE_NOT_FOUND");
                    }
                }
                else
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }

                response.Code = MessageResponseCode.OK;

                logger.Info("end editPrjStateDiagram");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione editPrjStateDiagram: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new MessageResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione editPrjStateDiagram: " + e);
                response = new MessageResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetStateOfDiagramResponse> getProjectStateDiagram(string token, string idProject, string classificationSchemeId, string codeProject)
        {
            logger.Info("begin getProjectStateDiagram");
            GetStateOfDiagramResponse response = new GetStateOfDiagramResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion

                if (string.IsNullOrEmpty(classificationSchemeId) && string.IsNullOrEmpty(codeProject) && string.IsNullOrEmpty(idProject))
                {
                    throw new RestException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(classificationSchemeId) || !string.IsNullOrEmpty(codeProject)) && !string.IsNullOrEmpty(idProject))
                {
                    throw new RestException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(classificationSchemeId) && string.IsNullOrEmpty(codeProject)) || (string.IsNullOrEmpty(classificationSchemeId) && !string.IsNullOrEmpty(codeProject)))
                {
                    throw new RestException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }

                StateOfDiagram stateOfDiagramResponse = new StateOfDiagram();

                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                try
                {
                    if (!string.IsNullOrEmpty(idProject))
                    {
                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(idProject, infoUtente);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(codeProject) && (!string.IsNullOrEmpty(classificationSchemeId)))
                        {
                            DocsPaVO.fascicolazione.Fascicolo[] fascicoli = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurity(codeProject, infoUtente.idAmministrazione, classificationSchemeId, false);
                            if (fascicoli != null && fascicoli.Length > 0)
                            {
                                fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicoli[0].systemID, infoUtente);
                            }
                            else
                            {
                                throw new RestException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("PROJECT_NOT_FOUND");
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
                                    throw new RestException("FASC_STATEOFDIAGRAM_NOT_FOUND");
                                }
                            }
                        }
                        else
                        {
                            throw new RestException("DIAGRAM_NOT_FOUND");
                        }
                    }
                    else
                    {
                        throw new RestException("TEMPLATE_NOT_FOUND");
                    }
                }
                else
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }

                response.StateOfDiagram = stateOfDiagramResponse;
                response.Code = GetStateOfDiagramResponseCode.OK;

                logger.Info("end getProjectStateDiagram");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getProjectStateDiagram: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetStateOfDiagramResponse();
                response.Code = GetStateOfDiagramResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getProjectStateDiagram: " + e);
                response = new GetStateOfDiagramResponse();
                response.Code = GetStateOfDiagramResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetFolderResponse> createFolder(string token, CreateFolderRequest request)
        {
            logger.Info("begin createFolder");
            GetFolderResponse response = new GetFolderResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion


                if (string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) || !string.IsNullOrEmpty(request.CodeProject)) && !string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject)) || (string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject)))
                {
                    throw new RestException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
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
                                throw new RestException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    throw new RestException("PROJECT_NOT_FOUND");
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
                Folder f1 = new Folder();
                f1.Description = cartella.descrizione;
                f1.Id = cartella.systemID;
                f1.IdParent = cartella.idParent;
                f1.IdProject = cartella.idFascicolo;
                f1.CreationDate = cartella.dtaApertura;
                response.Folder = f1;
                response.Code = GetFolderResponseCode.OK;
                logger.Info("end createFolder");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione createFolder: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetFolderResponse();
                response.Code = GetFolderResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione createFolder: " + e);
                response = new GetFolderResponse();
                response.Code = GetFolderResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetProjectFoldersResponse> getProjectFolders(string token, string idProject, string classificationSchemeId, string codeProject)
        {
            logger.Info("begin getProjectFolders");
            GetProjectFoldersResponse response = new GetProjectFoldersResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion
                if (string.IsNullOrEmpty(classificationSchemeId) && string.IsNullOrEmpty(codeProject) && string.IsNullOrEmpty(idProject))
                {
                    throw new RestException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(classificationSchemeId) || !string.IsNullOrEmpty(codeProject)) && !string.IsNullOrEmpty(idProject))
                {
                    throw new RestException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(classificationSchemeId) && string.IsNullOrEmpty(codeProject)) || (string.IsNullOrEmpty(classificationSchemeId) && !string.IsNullOrEmpty(codeProject)))
                {
                    throw new RestException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }
                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                try
                {
                    if (!string.IsNullOrEmpty(idProject))
                    {
                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(idProject, infoUtente);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(codeProject) && (!string.IsNullOrEmpty(classificationSchemeId)))
                        {
                            DocsPaVO.fascicolazione.Fascicolo[] fascicoli = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurity(codeProject, infoUtente.idAmministrazione, classificationSchemeId, false);
                            if (fascicoli != null && fascicoli.Length > 0)
                            {
                                fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicoli[0].systemID, infoUtente);
                            }
                            else
                            {
                                throw new RestException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }

                response.IdProject = fascicolo.systemID;
                response.ProjectDescription = fascicolo.descrizione;
                ArrayList cartelle = BusinessLogic.Fascicoli.FascicoloManager.getListaFolderDaIdFascicolo(infoUtente, fascicolo.systemID, null, false, false);
                Folder[] cartellePIS = new Folder[cartelle.Count];
                int i = 0;
                foreach (DocsPaVO.fascicolazione.Folder f in cartelle)
                {
                    Folder f1 = new Folder();
                    f1.Description = f.descrizione;
                    f1.Id = f.systemID;
                    f1.IdParent = f.idParent;
                    f1.IdProject = f.idFascicolo;
                    f1.CreationDate = f.dtaApertura;
                    cartellePIS[i] = f1;
                    i++;
                    
                }

                if (cartellePIS.Length > 1)
                {
                    response.Folders = cartellePIS;
                }
                else
                {
                    throw new Exception("Errore nel prelievo delle cartelle");
                }
                response.Code = GetProjectFoldersResponseCode.OK;

                logger.Info("end getProjectFolders");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getProjectFolders: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetProjectFoldersResponse();
                response.Code = GetProjectFoldersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getProjectFolders: " + e);
                response = new GetProjectFoldersResponse();
                response.Code = GetProjectFoldersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }
        
        public static async Task<GetProjectResponse> openCloseProject(string token, OpenCloseProjectRequest request)
        {
            logger.Info("begin openCloseProject");
            GetProjectResponse response = new GetProjectResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion
                //Controllo validità della richiesta
                if (request.IdProject == null || (request.CodeProject == null && request.ClassificationSchemeId  == null))
                {
                    throw new RestException("REQUIRED_PROJECT");
                }

                if (String.IsNullOrEmpty(request.Action))
                {
                    throw new RestException("REQUIRED_ACTION");
                }

                Project responseProject = new Project();
                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                //Ricerca del fascicolo
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
                                throw new RestException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }

                if (fascicolo != null)
                {
                    if (!string.IsNullOrEmpty(fascicolo.stato) && fascicolo.stato.Equals("A") && request.Action.ToUpper().Equals("O"))
                    {
                        throw new RestException("PROJECT_ALREADY_OPEN");
                    }

                    if (!string.IsNullOrEmpty(fascicolo.stato) && fascicolo.stato.Equals("C") && request.Action.ToUpper().Equals("C"))
                    {
                        throw new RestException("PROJECT_ALREADY_CLOSE");
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
                        DocsPaVO.utente.Ruolo role = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                        if (role != null)
                        {
                            fascicolo.chiusura = ora.ToString("dd/MM/yyyy");
                            fascicolo.stato = "C";
                            fascicolo.chiudeFascicolo = new DocsPaVO.fascicolazione.ChiudeFascicolo();
                            fascicolo.chiudeFascicolo.idPeople = infoUtente.idPeople;
                            fascicolo.chiudeFascicolo.idCorrGlob_Ruolo = role.systemId;
                            fascicolo.chiudeFascicolo.idCorrGlob_UO = role.uo.systemId;
                        }
                    }
                    BusinessLogic.Fascicoli.FascicoloManager.setFascicolo(infoUtente, fascicolo);
                }
                else
                {
                    throw new RestException("PROJECT_NOT_FOUND");
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
                        throw new RestException("PROJECT_NOT_FOUND");
                    }
                }
                else
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }

                response.Code = GetProjectResponseCode.OK;
                logger.Info("end editProject");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione openCloseProject: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetProjectResponse();
                response.Code = GetProjectResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione openCloseProject: " + e);
                response = new GetProjectResponse();
                response.Code = GetProjectResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

    }
}