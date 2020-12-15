using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using System.Threading.Tasks;
using RESTServices.Model.Domain;
using RESTServices.Model.Responses;
using RESTServices.Model.Requests;

namespace RESTServices.Manager
{
    public class AddressBookManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(AddressBookManager));

        public static async Task<GetCorrespondentResponse> getCorrespondent(string token, string idCorr)
        {
            logger.Info("begin getCorrespondent");
            GetCorrespondentResponse response = new GetCorrespondentResponse();
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

                if (string.IsNullOrEmpty(idCorr))
                {
                    //Id del corrispondente non presente
                    throw new RestException("REQUIRED_IDCORRESPONDENT");
                }

                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(idCorr);

                if ((corr == null) || (corr != null && string.IsNullOrEmpty(corr.systemId) && string.IsNullOrEmpty(corr.codiceRubrica)))
                {
                    bool rubricaComuneAbilitata = BusinessLogic.RubricaComune.Configurazioni.GetConfigurazioni(infoUtente).GestioneAbilitata;
                    if (rubricaComuneAbilitata)
                    {
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubricaRubricaComune(idCorr, infoUtente);
                    }

                    if (corr == null)
                        //Corrispondente non trovato
                        throw new RestException("CORRESPONDENT_NOT_FOUND");
                    else
                        corr.inRubricaComune = true;
                }
                else
                {
                    corr.inRubricaComune = false;
                }

                Correspondent corrResponse = Utils.GetCorrespondent(corr, infoUtente);
                response.Correspondent = corrResponse;
                response.Code = GetCorrespondentResponseCode.OK;

                logger.Info("end getCorrespondent");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getCorrespondent: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetCorrespondentResponse();
                response.Code = GetCorrespondentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getCorrespondent: " + e);
                response = new GetCorrespondentResponse();
                response.Code = GetCorrespondentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetCorrespondentResponse> addCorrespondent(string token, AddCorrespondentRequest request)
        {
            logger.Info("begin uploadFileToDocument");
            GetCorrespondentResponse response = new GetCorrespondentResponse();
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
                if (request.Correspondent == null)
                {
                    //Corrispondente non trovato
                    throw new RestException("REQUIRED_CORRESPONDENT");
                }

                if (string.IsNullOrEmpty(request.Correspondent.CorrespondentType) || (!request.Correspondent.CorrespondentType.Equals("U") && !request.Correspondent.CorrespondentType.Equals("P")))
                {
                    //Inserire il tipo U o P
                    throw new RestException("REQUIRED_CORRESPONDENTTYPE");
                }

                if (request.Correspondent.CorrespondentType.Equals("U"))
                {
                    if (string.IsNullOrEmpty(request.Correspondent.Description))
                    {
                        //richiesta descrizione
                        throw new RestException("REQUIRED_DESCRIPTION_CORREPONDENT");
                    }
                }

                if (request.Correspondent.CorrespondentType.Equals("P"))
                {
                    if (string.IsNullOrEmpty(request.Correspondent.Name) || string.IsNullOrEmpty(request.Correspondent.Surname))
                    {
                        //richiesta descrizione
                        throw new RestException("REQUIRED_N_S_CORREPONDENT");
                    }
                }

                DocsPaVO.utente.Corrispondente corrResult = null;
                Correspondent corrResponse = null;

                //Posso creare solo esterni
                request.Correspondent.Type = "E";

                DocsPaVO.utente.Corrispondente corr = Utils.GetCorrespondentFromPisNewInsert(request.Correspondent, infoUtente);


                if (corr != null && !string.IsNullOrEmpty(corr.tipoCorrispondente) && !corr.tipoCorrispondente.Equals("O") && !BusinessLogic.Utenti.addressBookManager.VerificaInserimentoCorrispondente(corr, null))
                {
                    //Codice corrispondente già presente
                    throw new RestException("CODE_CORRESPONDENT_EXISTS");
                }

                corrResult = BusinessLogic.Utenti.addressBookManager.insertCorrispondente(corr, null);

                corrResponse = Utils.GetCorrespondent(corrResult, infoUtente);

                if (corrResponse != null)
                {
                    List<DocsPaVO.utente.MailCorrispondente> casella = new List<DocsPaVO.utente.MailCorrispondente>();
                    casella.Add(new DocsPaVO.utente.MailCorrispondente()
                    {
                        Email = corrResult.email,
                        Note = "",
                        Principale = "1"
                    });

                    //Altre email
                    if(request.Correspondent.OtherEmails != null && request.Correspondent.OtherEmails.Count > 0)
                    {
                        foreach(string s in request.Correspondent.OtherEmails)
                        {
                            casella.Add(new DocsPaVO.utente.MailCorrispondente()
                            {
                                Email = s,
                                Note = "",
                                Principale = "0"
                            });
                        }
                    }

                    BusinessLogic.Utenti.addressBookManager.InsertMailCorrispondente(casella, corrResult.systemId);
                    response.Correspondent = corrResponse;
                    response.Code = GetCorrespondentResponseCode.OK;
                }
                else
                {
                    //Corrispondente non trovato
                    throw new RestException("USER_NO_EXIST");
                }


                logger.Info("end uploadFileToDocument");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetCorrespondentResponse();
                response.Code = GetCorrespondentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione uploadFileToDocument: " + e);
                response = new GetCorrespondentResponse();
                response.Code = GetCorrespondentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetCorrespondentResponse> editCorrespondent(string token, AddCorrespondentRequest request)
        {
            logger.Info("begin editCorrespondent");
            GetCorrespondentResponse response = new GetCorrespondentResponse();
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

                if (request == null)
                {
                    throw new RestException("REQUIRED_CORRESPONDENT");
                }

                if (string.IsNullOrEmpty(request.Correspondent.Id))
                {
                    //Corrispondente non trovato
                    throw new RestException("REQUIRED_IDCORRESPONDENT");
                }

                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(request.Correspondent.Id);

                if (corr == null)
                {
                    //Corrispondente non trovato
                    throw new RestException("CORRESPONDENT_NOT_FOUND");
                }

                string message = string.Empty;

                DocsPaVO.utente.DatiModificaCorr mod = new DocsPaVO.utente.DatiModificaCorr();
                mod = Utils.GetModificaCorrespondentFromPis(request.Correspondent, corr);

                bool result = BusinessLogic.Utenti.UserManager.ModifyCorrispondenteEsterno(mod, infoUtente, out message);

                if (result)
                {
                    //DocsPaVO.utente.Corrispondente corrRes = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(request.Correspondent.Id);
                    //DocsPaVO.utente.Corrispondente corrRes = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubricaNotDisabled(request.Correspondent.Code, request.Correspondent.Type == "E" ? DocsPaVO.addressbook.TipoUtente.ESTERNO : DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);

                    DocsPaVO.utente.Corrispondente corrRes = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubricaNotDisabled(request.Correspondent.Code, DocsPaVO.addressbook.TipoUtente.GLOBALE, infoUtente);
                    if (corrRes == null)
                        corrRes = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubricaNotDisabled(request.Correspondent.Code, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                    if (corrRes != null && corrRes.tipoIE == "E" && !string.IsNullOrEmpty(corrRes.email))
                    {
                        if (!string.IsNullOrEmpty(corrRes.email))
                        {
                            List<DocsPaVO.utente.MailCorrispondente> casella = new List<DocsPaVO.utente.MailCorrispondente>();
                            casella.Add(new DocsPaVO.utente.MailCorrispondente()
                            {
                                Email = corrRes.email,
                                Note = "",
                                Principale = "1"
                            });

                            //Altre email
                            if (request.Correspondent.OtherEmails != null && request.Correspondent.OtherEmails.Count > 0)
                            {
                                foreach (string s in request.Correspondent.OtherEmails)
                                {
                                    casella.Add(new DocsPaVO.utente.MailCorrispondente()
                                    {
                                        Email = s,
                                        Note = "",
                                        Principale = "0"
                                    });
                                }
                            }

                            BusinessLogic.Utenti.addressBookManager.InsertMailCorrispondente(casella, corrRes.systemId);
                        }
                    }
                    response.Correspondent = Utils.GetCorrespondent(corrRes, infoUtente);
                    response.Code = GetCorrespondentResponseCode.OK;
                }
                else
                {
                    //Corrispondente non trovato
                    throw new RestException("CORRESPONDENT_NOT_FOUND");
                }

                logger.Info("end editCorrespondent");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione editCorrespondent: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetCorrespondentResponse();
                response.Code = GetCorrespondentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione editCorrespondent: " + e);
                response = new GetCorrespondentResponse();
                response.Code = GetCorrespondentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetFiltersResponse> getCorrespondentFilters(string token)
        {
            logger.Info("begin getCorrespondentFilters");
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

                listaFiltri.Add(new Filter() { Name = "TYPE", Description = "Filtro utilizzato per reperire il tipo di corrispondente. Inserire il valore “INTERNAL” per gli interni, “EXTERNAL” per gli esterni, “GLOBAL” per tutti i corrispondenti", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "OFFICES", Description = "Filtro utilizzato per reperire i corrispondenti di tipo UO con valore “true”", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "USERS", Description = "Filtro utilizzato per reperire i corrispondenti di tipo Persona con valore “true”", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "ROLES", Description = "Filtro utilizzato per reperire i corrispondenti di tipo Ruoli con valore “true”", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "COMMON_ADDRESSBOOK", Description = "Filtro utilizzato per reperire i corrispondenti anche in rubrica comune con valore “true”", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "RF", Description = "Filtro utilizzato per reperire i corrispondenti anche di RF con valore “true”", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "CODE", Description = "Filtro utilizzato per la ricerca di corrispondenti dato il codice rubrica o parte di esso", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "EXACT_CODE", Description = "Filtro utilizzato per la ricerca di corrispondenti dato il codice rubrica esatto", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "DESCRIPTION", Description = "Filtro utilizzato per la ricerca di corrispondenti data la descrizione", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "CITY", Description = "Filtro utilizzato per la ricerca di corrispondenti data la città", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "LOCALITY", Description = "Filtro utilizzato per la ricerca di corrispondenti indicata al località", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "REGISTRY_OR_RF", Description = "Filtro utilizzato per la ricerca di corrispondenti soltanto in un determinato Registro/RF dato il codice del Registro/RF", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "MAIL", Description = "Filtro utilizzato per la ricerca di corrispondenti indicando l’indirizzo mail", Type = FilterTypeEnum.String });
                // Modifica 23-01-2013: In seguito alla distinzione dei valori di codice fiscale e partita iva, la ricerca va fatta sui due campi separatamente.
                // Quindi devono essere creati dei filtri distinti.
                //listaFiltri.Add(new Filter() { Name = "NAT", Description = "Filtro utilizzato per la ricerca di corrispondenti indicando il codice fiscale/p.iva", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "NATIONAL_IDENTIFICATION_NUMBER", Description = "Filtro utilizzato per la ricerca di corrispondenti indicando il codice fiscale", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "VAT_NUMBER", Description = "Filtro utilizzato per la ricerca di corrispondenti indicando la partita iva", Type = FilterTypeEnum.String });
                
                GetFiltersResponse response = new GetFiltersResponse();
                response.Filters = listaFiltri;
                response.Code = GetFiltersResponseCode.OK;
                logger.Info("END - getCorrespondentFilters");
                return response;

            }
            catch (Exception e)
            {
                logger.Error("eccezione getCorrespondentFilters: " + e);
                GetFiltersResponse errorResp = new GetFiltersResponse();
                errorResp.Code = GetFiltersResponseCode.SYSTEM_ERROR;
                errorResp.ErrorMessage = e.Message;
                return errorResp;
            }
        }

        public static async Task<GetFiltersResponse> getUserFilters(string token)
        {
            logger.Info("begin getUserFilters");
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

                listaFiltri.Add(new Filter() { Name = "NATIONAL_IDENTIFICATION_NUMBER", Description = "Filtro per codice fiscale", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "USER_MAIL", Description = "Filtro per mail", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "USER_NAME", Description = "Filtro per nome", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "USER_SURNAME", Description = "Filtro per cognome", Type = FilterTypeEnum.String });

                GetFiltersResponse response = new GetFiltersResponse();
                response.Filters = listaFiltri;
                response.Code = GetFiltersResponseCode.OK;
                logger.Info("END - getUserFilters");
                return response;

            }
            catch (Exception e)
            {
                logger.Error("eccezione getUserFilters: " + e);
                GetFiltersResponse errorResp = new GetFiltersResponse();
                errorResp.Code = GetFiltersResponseCode.SYSTEM_ERROR;
                errorResp.ErrorMessage = e.Message;
                return errorResp;
            }
        }

        public static async Task<SearchCorrespondentsResponse> searchCorrespondents(string token, SearchFiltersOnlyRequest request)
        {
            logger.Info("begin searchCorrespondents");
            SearchCorrespondentsResponse response = new SearchCorrespondentsResponse();
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
                bool idFromRC = false;
                Filter filtroIDRC = (from filtro in request.Filters where (filtro != null && !string.IsNullOrEmpty(filtro.Name) && filtro.Name.ToUpper() == "EXTRACT_ID_COMMONADDRESSBOOK") select filtro).FirstOrDefault();
                if (filtroIDRC != null && filtroIDRC.Value.ToUpper() == "TRUE")
                {
                    idFromRC = true;
                }
                //Chiamata al metodo CheckFilterType(request.Filters)
                Utils.CheckFilterTypes(request.Filters);

                DocsPaVO.rubrica.ParametriRicercaRubrica qco = Utils.GetParametriRicercaRubricaFromPis(request.Filters, infoUtente);

                BusinessLogic.Rubrica.DPA3_RubricaSearchAgent corrSearcher = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(infoUtente);

                // DocsPaDB.Query_DocsPAWS.Rubrica query = new DocsPaDB.Query_DocsPAWS.Rubrica(infoUtente);
                DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica = new DocsPaVO.rubrica.SmistamentoRubrica();
                System.Collections.ArrayList objElementiRubrica = corrSearcher.Search(qco, smistamentoRubrica);
                //ArrayList objElementiRubrica = query.GetElementiRubrica(qco);


                Correspondent[] correspondentsRespone = null;

                if (objElementiRubrica != null && objElementiRubrica.Count > 0)
                {
                    correspondentsRespone = new Correspondent[objElementiRubrica.Count];
                    for (int i = 0; i < objElementiRubrica.Count; i++)
                    {
                        Correspondent userTemp = new Correspondent();
                        DocsPaVO.rubrica.ElementoRubrica elementoRubrica = ((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[i]);

                        userTemp.Id = elementoRubrica.systemId;
                        userTemp.Description = elementoRubrica.descrizione;
                        userTemp.Code = elementoRubrica.codice;
                        userTemp.Name = elementoRubrica.nome;
                        userTemp.Surname = elementoRubrica.cognome;
                        userTemp.NationalIdentificationNumber = elementoRubrica.cf_piva;
                        userTemp.IsCommonAddress = elementoRubrica.isRubricaComune;
                        userTemp.Type = (elementoRubrica.interno ? "I" : "E");
                        if (!String.IsNullOrEmpty(elementoRubrica.tipo))
                            userTemp.CorrespondentType = elementoRubrica.tipo;

                        if (idFromRC && string.IsNullOrEmpty(elementoRubrica.systemId))
                        {
                            DocsPaVO.utente.Corrispondente userTemp2 = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubricaRubricaComune(elementoRubrica.codice, infoUtente);
                            userTemp.Id = userTemp2.systemId;
                        }


                        correspondentsRespone[i] = userTemp;
                    }
                    response.Correspondents = correspondentsRespone;
                }
                else
                {
                    //throw new PisException("CORRESPONDENTS_NOT_FOUND");
                    correspondentsRespone = new Correspondent[0];
                    response.Correspondents = correspondentsRespone;
                }
                response.Code = SearchCorrespondentsResponseCode.OK;


                logger.Info("end searchCorrespondents");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione searchCorrespondents: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new SearchCorrespondentsResponse();
                response.Code = SearchCorrespondentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione searchCorrespondents: " + e);
                response = new SearchCorrespondentsResponse();
                response.Code = SearchCorrespondentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetUsersResponse> searchUsers(string token, SearchFiltersOnlyRequest request)
        {
            logger.Info("begin searchUsers");
            GetUsersResponse response = new GetUsersResponse();
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

                //Filtri ricerca rubrica
                DocsPaVO.rubrica.ParametriRicercaRubrica qco = new DocsPaVO.rubrica.ParametriRicercaRubrica();
                BusinessLogic.Rubrica.DPA3_RubricaSearchAgent ccs = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(infoUtente);
                DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica = new DocsPaVO.rubrica.SmistamentoRubrica();
                qco.caller = new DocsPaVO.rubrica.ParametriRicercaRubrica.CallerIdentity();
                qco.caller.IdUtente = infoUtente.idPeople;
                qco.caller.IdRuolo = infoUtente.idGruppo;
                qco.caller.filtroRegistroPerRicerca = null;
                qco.doUtenti = true;
                qco.calltype = DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN;
                //

                bool filterFound = false;

                foreach (Filter fil in request.Filters)
                {
                    if (fil != null && !string.IsNullOrEmpty(fil.Value))
                    {
                        if (fil.Name.ToUpper().Equals("NATIONAL_IDENTIFICATION_NUMBER"))
                        {
                            filterFound = true;
                            qco.cf_piva = fil.Value;
                        }
                        if (fil.Name.ToUpper().Equals("USER_MAIL"))
                        {
                            filterFound = true;
                            qco.email = fil.Value;
                        }
                        if (fil.Name.ToUpper().Equals("USER_NAME"))
                        {
                            filterFound = true;
                            qco.descrizione = fil.Value;
                        }
                        if (fil.Name.ToUpper().Equals("USER_SURNAME"))
                        {
                            filterFound = true;
                            qco.descrizione = fil.Value;
                        }

                        if (!filterFound)
                            throw new RestException("FILTER_NOT_FOUND");
                    }
                }
                User[] userResponse = null;

                System.Collections.ArrayList objElementiRubrica = ccs.Search(qco, smistamentoRubrica);

                if (objElementiRubrica != null && objElementiRubrica.Count > 0)
                {
                    userResponse = new User[objElementiRubrica.Count];
                    for (int i = 0; i < objElementiRubrica.Count; i++)
                    {
                        User userTemp = new User();
                        userTemp.Id = ((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[i]).idPeople;
                        userTemp.Description = ((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[i]).descrizione;
                        userTemp.UserId = ((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[i]).codice;
                        userTemp.Name = ((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[i]).nome;
                        userTemp.Surname = ((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[i]).cognome;
                        userTemp.NationalIdentificationNumber = ((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[i]).cf_piva;
                        userResponse[i] = userTemp;
                    }
                    response.Users = userResponse;
                }
                else
                {
                    //throw new PisException("USERS_NOT_FOUND");
                    userResponse = new User[0];
                    response.Users = userResponse;
                }
                response.Code = GetUsersResponseCode.OK;
                logger.Info("end searchUsers");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione searchUsers: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetUsersResponse();
                response.Code = GetUsersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione searchUsers: " + e);
                response = new GetUsersResponse();
                response.Code = GetUsersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }
    }
}