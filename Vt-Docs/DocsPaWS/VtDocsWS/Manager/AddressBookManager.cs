using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VtDocsWS.WebServices;
using System.Collections;
using log4net;

namespace VtDocsWS.Manager
{
    public class AddressBookManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(AddressBookManager));

        public static Services.AddressBook.AddCorrespondent.AddCorrespondentResponse AddCorrespondent(Services.AddressBook.AddCorrespondent.AddCorrespondentRequest request)
        {
            Services.AddressBook.AddCorrespondent.AddCorrespondentResponse response = new Services.AddressBook.AddCorrespondent.AddCorrespondentResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "AddCorrespondent");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (request.Correspondent == null)
                {
                    //Corrispondente non trovato
                    throw new PisException("REQUIRED_CORRESPONDENT");
                }

                if (string.IsNullOrEmpty(request.Correspondent.CorrespondentType) || (!request.Correspondent.CorrespondentType.Equals("U") && !request.Correspondent.CorrespondentType.Equals("P")))
                {
                    //Inserire il tipo U o P
                    throw new PisException("REQUIRED_CORRESPONDENTTYPE");
                }

                if (request.Correspondent.CorrespondentType.Equals("U"))
                {
                    if (string.IsNullOrEmpty(request.Correspondent.Description))
                    {
                        //richiesta descrizione
                        throw new PisException("REQUIRED_DESCRIPTION_CORREPONDENT");
                    }
                }

                if (request.Correspondent.CorrespondentType.Equals("P"))
                {
                    if (string.IsNullOrEmpty(request.Correspondent.Name) || string.IsNullOrEmpty(request.Correspondent.Surname))
                    {
                        //richiesta descrizione
                        throw new PisException("REQUIRED_N_S_CORREPONDENT");
                    }
                }

                DocsPaVO.utente.Corrispondente corrResult = null;
                Domain.Correspondent corrResponse = null;

                //Posso creare solo esterni
                request.Correspondent.Type = "E";

                DocsPaVO.utente.Corrispondente corr = Utils.GetCorrespondentFromPisNewInsert(request.Correspondent, infoUtente);


                if (corr != null && !string.IsNullOrEmpty(corr.tipoCorrispondente) && !corr.tipoCorrispondente.Equals("O") && !BusinessLogic.Utenti.addressBookManager.VerificaInserimentoCorrispondente(corr, null))
                {
                    //Codice corrispondente già presente
                    throw new PisException("CODE_CORRESPONDENT_EXISTS");
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

                    BusinessLogic.Utenti.addressBookManager.InsertMailCorrispondente(casella, corrResult.systemId);
                    response.Correspondent = corrResponse;
                    response.Success = true;
                }
                else
                {
                    //Corrispondente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}",pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}",ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.AddressBook.DeleteCorrespondent.DeleteCorrespondentResponse DeleteCorrespondent(Services.AddressBook.DeleteCorrespondent.DeleteCorrespondentRequest request)
        {
            Services.AddressBook.DeleteCorrespondent.DeleteCorrespondentResponse response = new Services.AddressBook.DeleteCorrespondent.DeleteCorrespondentResponse();

            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "DeleteCorrespondent");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.CorrespondentId))
                {
                    //Corrispondente non trovato
                    throw new PisException("REQUIRED_IDCORRESPONDENT");
                }

                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(request.CorrespondentId);

                if (corr == null)
                {
                    //Corrispondente non trovato
                    throw new PisException("CORRESPONDENT_NOT_FOUND");
                }

                string message = string.Empty;

                bool result = BusinessLogic.Utenti.UserManager.DeleteCorrispondenteEsterno(request.CorrespondentId, 0, infoUtente, out message);

                if (result)
                {

                    response.Success = true;
                }
                else
                {
                    //Corrispondente non trovato
                    throw new PisException("CORRESPONDENT_NOT_FOUND");
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

        public static Services.AddressBook.EditCorrespondent.EditCorrespondentResponse EditCorrespondent(Services.AddressBook.EditCorrespondent.EditCorrespondentRequest request)
        {
            Services.AddressBook.EditCorrespondent.EditCorrespondentResponse response = new Services.AddressBook.EditCorrespondent.EditCorrespondentResponse();

            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request,"EditCorrespondent");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (request == null)
                {
                    throw new PisException("REQUIRED_CORRESPONDENT");
                }

                if (string.IsNullOrEmpty(request.Correspondent.Id))
                {
                    //Corrispondente non trovato
                    throw new PisException("REQUIRED_IDCORRESPONDENT");
                }

                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(request.Correspondent.Id);

                if (corr == null)
                {
                    //Corrispondente non trovato
                    throw new PisException("CORRESPONDENT_NOT_FOUND");
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

                            BusinessLogic.Utenti.addressBookManager.InsertMailCorrispondente(casella, corrRes.systemId);
                        }
                    }
                    response.Correspondent = Utils.GetCorrespondent(corrRes, infoUtente);
                    response.Success = true;
                }
                else
                {
                    //Corrispondente non trovato
                    throw new PisException("CORRESPONDENT_NOT_FOUND");
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

        public static Services.AddressBook.GetCorrespondent.GetCorrespondentResponse GetCorrespondent(Services.AddressBook.GetCorrespondent.GetCorrespondentRequest request)
        {
            Services.AddressBook.GetCorrespondent.GetCorrespondentResponse response = new Services.AddressBook.GetCorrespondent.GetCorrespondentResponse();

            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetCorrespondent");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdCorrespondent))
                {
                    //Id del corrispondente non presente
                    throw new PisException("REQUIRED_IDCORRESPONDENT");
                }

                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(request.IdCorrespondent);

                if ((corr == null) || (corr!=null && string.IsNullOrEmpty(corr.systemId) && string.IsNullOrEmpty(corr.codiceRubrica) ))
                {
                    bool rubricaComuneAbilitata = BusinessLogic.RubricaComune.Configurazioni.GetConfigurazioni(infoUtente).GestioneAbilitata;
                    if (rubricaComuneAbilitata)
                    {
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubricaRubricaComune(request.IdCorrespondent, infoUtente);
                    }

                    if (corr == null)
                        //Corrispondente non trovato
                        throw new PisException("CORRESPONDENT_NOT_FOUND");
                    else
                        corr.inRubricaComune = true;
                }
                else
                {
                    corr.inRubricaComune = false;
                }

                Domain.Correspondent corrResponse = Utils.GetCorrespondent(corr, infoUtente);
                response.Correspondent = corrResponse;

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

        public static Services.AddressBook.SearchCorrespondents.SearchCorrespondentsResponse SearchCorrespondents(Services.AddressBook.SearchCorrespondents.SearchCorrespondentsRequest request)
        {
            Services.AddressBook.SearchCorrespondents.SearchCorrespondentsResponse response = new Services.AddressBook.SearchCorrespondents.SearchCorrespondentsResponse();

            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "SearchCorrespondents");

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
                bool idFromRC = false;
                Domain.Filter filtroIDRC = (from filtro in request.Filters where (filtro != null && !string.IsNullOrEmpty(filtro.Name) && filtro.Name.ToUpper() == "EXTRACT_ID_COMMONADDRESSBOOK") select filtro).FirstOrDefault();
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
                ArrayList objElementiRubrica = corrSearcher.Search(qco, smistamentoRubrica);
                //ArrayList objElementiRubrica = query.GetElementiRubrica(qco);


                Domain.Correspondent[] correspondentsRespone = null;

                if (objElementiRubrica != null && objElementiRubrica.Count > 0)
                {
                    correspondentsRespone = new Domain.Correspondent[objElementiRubrica.Count];
                    for (int i = 0; i < objElementiRubrica.Count; i++)
                    {
                        Domain.Correspondent userTemp = new Domain.Correspondent();
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
                    correspondentsRespone = new Domain.Correspondent[0];
                    response.Correspondents = correspondentsRespone;
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

        public static Services.AddressBook.SearchUsers.SearchUsersResponse SearchUsers(Services.AddressBook.SearchUsers.SearchUsersRequest request)
        {
            Services.AddressBook.SearchUsers.SearchUsersResponse response = new Services.AddressBook.SearchUsers.SearchUsersResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "SearchUsers");

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

                foreach (VtDocsWS.Domain.Filter fil in request.Filters)
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
                            throw new PisException("FILTER_NOT_FOUND");
                    }
                }
                Domain.User[] userResponse = null;

                ArrayList objElementiRubrica = ccs.Search(qco, smistamentoRubrica);

                if (objElementiRubrica != null && objElementiRubrica.Count > 0)
                {
                    userResponse = new Domain.User[objElementiRubrica.Count];
                    for (int i = 0; i < objElementiRubrica.Count; i++)
                    {
                        Domain.User userTemp = new Domain.User();
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
                    userResponse = new Domain.User[0];
                    response.Users = userResponse;
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

        public static Services.AddressBook.GetCorrespondentFilters.GetCorrespondentFiltersResponse GetCorrespondentFilters(Services.AddressBook.GetCorrespondentFilters.GetCorrespondentFiltersRequest request)
        {
            Services.AddressBook.GetCorrespondentFilters.GetCorrespondentFiltersResponse response = new Services.AddressBook.GetCorrespondentFilters.GetCorrespondentFiltersResponse();

            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetCorrespondentFilters");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                List<Domain.Filter> listaFiltri = new List<Domain.Filter>();

                listaFiltri.Add(new Domain.Filter() { Name = "TYPE", Description = "Filtro utilizzato per reperire il tipo di corrispondente. Inserire il valore “INTERNAL” per gli interni, “EXTERNAL” per gli esterni, “GLOBAL” per tutti i corrispondenti", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "OFFICES", Description = "Filtro utilizzato per reperire i corrispondenti di tipo UO con valore “true”", Type = Domain.FilterTypeEnum.Bool });
                listaFiltri.Add(new Domain.Filter() { Name = "USERS", Description = "Filtro utilizzato per reperire i corrispondenti di tipo Persona con valore “true”", Type = Domain.FilterTypeEnum.Bool });
                listaFiltri.Add(new Domain.Filter() { Name = "ROLES", Description = "Filtro utilizzato per reperire i corrispondenti di tipo Ruoli con valore “true”", Type = Domain.FilterTypeEnum.Bool });
                listaFiltri.Add(new Domain.Filter() { Name = "COMMON_ADDRESSBOOK", Description = "Filtro utilizzato per reperire i corrispondenti anche in rubrica comune con valore “true”", Type = Domain.FilterTypeEnum.Bool });
                listaFiltri.Add(new Domain.Filter() { Name = "RF", Description = "Filtro utilizzato per reperire i corrispondenti anche di RF con valore “true”", Type = Domain.FilterTypeEnum.Bool });
                listaFiltri.Add(new Domain.Filter() { Name = "CODE", Description = "Filtro utilizzato per la ricerca di corrispondenti dato il codice rubrica o parte di esso", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "EXACT_CODE", Description = "Filtro utilizzato per la ricerca di corrispondenti dato il codice rubrica esatto", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "DESCRIPTION", Description = "Filtro utilizzato per la ricerca di corrispondenti data la descrizione", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "CITY", Description = "Filtro utilizzato per la ricerca di corrispondenti data la città", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "LOCALITY", Description = "Filtro utilizzato per la ricerca di corrispondenti indicata al località", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "REGISTRY_OR_RF", Description = "Filtro utilizzato per la ricerca di corrispondenti soltanto in un determinato Registro/RF dato il codice del Registro/RF", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "MAIL", Description = "Filtro utilizzato per la ricerca di corrispondenti indicando l’indirizzo mail", Type = Domain.FilterTypeEnum.String });
                // Modifica 23-01-2013: In seguito alla distinzione dei valori di codice fiscale e partita iva, la ricerca va fatta sui due campi separatamente.
                // Quindi devono essere creati dei filtri distinti.
                //listaFiltri.Add(new Domain.Filter() { Name = "NAT", Description = "Filtro utilizzato per la ricerca di corrispondenti indicando il codice fiscale/p.iva", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "NATIONAL_IDENTIFICATION_NUMBER", Description = "Filtro utilizzato per la ricerca di corrispondenti indicando il codice fiscale", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "VAT_NUMBER", Description = "Filtro utilizzato per la ricerca di corrispondenti indicando la partita iva", Type = Domain.FilterTypeEnum.String });
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

        public static Services.AddressBook.GetUserFilters.GetUserFiltersResponse GetUserFilters(Services.AddressBook.GetUserFilters.GetUserFiltersRequest request)
        {
            Services.AddressBook.GetUserFilters.GetUserFiltersResponse response = new Services.AddressBook.GetUserFilters.GetUserFiltersResponse();

            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetUserFilters");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                List<Domain.Filter> listaFiltri = new List<Domain.Filter>();

                listaFiltri.Add(new Domain.Filter() { Name = "NATIONAL_IDENTIFICATION_NUMBER", Description = "Filtro per codice fiscale", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "USER_MAIL", Description = "Filtro per mail", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "USER_NAME", Description = "Filtro per nome", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "USER_SURNAME", Description = "Filtro per cognome", Type = Domain.FilterTypeEnum.String });

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

        public static Services.AddressBook.GetOpportunity.GetOpportunityResponse GetOpportunityList(Services.AddressBook.GetOpportunity.GetOpportunityRequest request)
        {
            Services.AddressBook.GetOpportunity.GetOpportunityResponse response = new Services.AddressBook.GetOpportunity.GetOpportunityResponse();

            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetOpportunityList");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente
                
                /*
                if (string.IsNullOrEmpty(request.CodiceUtente))
                {
                    //Codice dell'offer manager non presente
                    throw new PisException("REQUIRED_CODUTENTE");
                }
                */

                List<string> oppIdlist = BusinessLogic.Utenti.UserManager.getIdOpportunityList(request.CodiceUtente);

                if (oppIdlist != null && oppIdlist.Count>0)
                {
                    response.IdOpportunityList = oppIdlist.ToArray();
                }
                else
                {
                    throw new PisException("OPPORTUNITY_NOT_FOUND");
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
    }
}