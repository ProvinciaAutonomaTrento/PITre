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
    public class RolesManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(RolesManager));

        public static Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsResponse GetRolesForEnabledActions(Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsRequest request)
        {
            Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsResponse response = new Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsResponse();

            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetRolesForEnabledActions");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.CodiceTipoFunzione))
                {
                    throw new PisException("REQUIRED_CODTIPOFUNC");
                }

                if (string.IsNullOrEmpty(request.UserID))
                {
                    throw new PisException("REQUIRED_USERID");
                }

                Domain.Role[] roleResponse = null;

                DocsPaVO.utente.Utente utenteDaCercare = null;
                ArrayList arrayRuoli = new ArrayList();

                if (!string.IsNullOrEmpty(request.UserID))
                {
                    utenteDaCercare = BusinessLogic.Utenti.UserManager.getUtente(request.UserID, Utils.GetIdAmministrazione(request.UserID));
                }
                else
                {
                    throw new PisException("REQUIRED_USERID");
                }

                if (utenteDaCercare != null)
                {
                    DocsPaVO.utente.InfoUtente infoUtenteDaCercare = new DocsPaVO.utente.InfoUtente(utenteDaCercare, Utils.GetRuoloPreferito(utenteDaCercare.idPeople));
                    arrayRuoli = BusinessLogic.Utenti.UserManager.getRuoliUtenteForEnabledActions(infoUtenteDaCercare.idPeople, request.CodiceTipoFunzione, Utils.GetIdAmministrazione(request.UserID));
                    if (arrayRuoli != null && arrayRuoli.Count > 0)
                    {
                        roleResponse = new Domain.Role[arrayRuoli.Count];
                        int i = 0;
                        foreach (DocsPaVO.utente.Ruolo rol in arrayRuoli)
                        {
                            Domain.Role roleTemp = new Domain.Role();
                            roleTemp.Code = rol.codiceRubrica;
                            roleTemp.Description = rol.descrizione;
                            roleTemp.Id = rol.idGruppo;
                            roleResponse[i] = roleTemp;
                            i++;
                        }
                    }
                    else
                    {
                        throw new PisException("USER_NO_ROLES");
                    }
                }
                else
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }

                response.Roles = roleResponse;

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

        public static Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsResponse GetRolesForEnabledSingleFunction(Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsRequest request)
        {
            Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsResponse response = new Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsResponse();

            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetRolesForEnabledSingleFunction");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.CodiceTipoFunzione))
                {
                    throw new PisException("REQUIRED_CODTIPOFUNC");
                }

                if (string.IsNullOrEmpty(request.UserID))
                {
                    throw new PisException("REQUIRED_USERID");
                }

                Domain.Role[] roleResponse = null;

                DocsPaVO.utente.Utente utenteDaCercare = null;
                ArrayList arrayRuoli = new ArrayList();

                if (!string.IsNullOrEmpty(request.UserID))
                {
                    utenteDaCercare = BusinessLogic.Utenti.UserManager.getUtente(request.UserID, Utils.GetIdAmministrazione(request.UserID));
                }
                else
                {
                    throw new PisException("REQUIRED_USERID");
                }

                if (utenteDaCercare != null)
                {
                    DocsPaVO.utente.InfoUtente infoUtenteDaCercare = new DocsPaVO.utente.InfoUtente(utenteDaCercare, Utils.GetRuoloPreferito(utenteDaCercare.idPeople));
                    arrayRuoli = BusinessLogic.Utenti.UserManager.getRolesForEnabledSingleFunction(infoUtenteDaCercare.idPeople, request.CodiceTipoFunzione, Utils.GetIdAmministrazione(request.UserID));
                    if (arrayRuoli != null && arrayRuoli.Count > 0)
                    {
                        roleResponse = new Domain.Role[arrayRuoli.Count];
                        int i = 0;
                        foreach (DocsPaVO.utente.Ruolo rol in arrayRuoli)
                        {
                            Domain.Role roleTemp = new Domain.Role();
                            roleTemp.Code = rol.codiceRubrica;
                            roleTemp.Description = rol.descrizione;
                            roleTemp.Id = rol.idGruppo;
                            roleResponse[i] = roleTemp;
                            i++;
                        }
                    }
                    else
                    {
                        throw new PisException("USER_NO_ROLES");
                    }
                }
                else
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }

                response.Roles = roleResponse;

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

        public static Services.Roles.GetRole.GetRoleResponse GetRole(Services.Roles.GetRole.GetRoleRequest request)
        {
            Services.Roles.GetRole.GetRoleResponse response = new Services.Roles.GetRole.GetRoleResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetRole");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.CodeRole) && string.IsNullOrEmpty(request.IdRole))
                {
                    throw new PisException("REQUIRED_CODE_OR_ID_ROLE");
                }

                if (!string.IsNullOrEmpty(request.CodeRole) && !string.IsNullOrEmpty(request.IdRole))
                {
                    throw new PisException("REQUIRED_ONLY_CODE_OR_ID_ROLE");
                }

                Domain.Role roleResponse = new Domain.Role();
                Domain.Register[] registerResponse = null;

                DocsPaVO.utente.Ruolo ruolo = null;
                ArrayList registri = new ArrayList();
                if (!string.IsNullOrEmpty(request.CodeRole))
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(request.CodeRole);
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.IdRole))
                    {
                        //Id gruppo
                        ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(request.IdRole);
                    }
                }

                if (ruolo != null)
                {
                    registri = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(ruolo.systemId, string.Empty, string.Empty);
                    roleResponse.Code = ruolo.codiceRubrica;
                    roleResponse.Description = ruolo.descrizione;
                    roleResponse.Id = ruolo.idGruppo;
                    if (registri != null && registri.Count > 0)
                    {
                        registerResponse = new Domain.Register[registri.Count];
                        int i = 0;
                        foreach (DocsPaVO.utente.Registro reg in registri)
                        {
                            Domain.Register regTemp = new Domain.Register();
                            regTemp.Code = reg.codRegistro;
                            regTemp.Description = reg.descrizione;
                            regTemp.Id = reg.systemId;
                            if (!string.IsNullOrEmpty(reg.chaRF) && (reg.chaRF).Equals("1"))
                            {
                                regTemp.IsRF = true;
                            }
                            else
                            {
                                regTemp.IsRF = false;
                            }
                            if (!string.IsNullOrEmpty(reg.stato))
                            {
                                if (reg.stato.Equals("A"))
                                {
                                    regTemp.State = "Open";
                                }
                                else
                                {
                                    regTemp.State = "Closed";
                                }
                            }
                            registerResponse[i] = regTemp;
                            i++;
                        }
                    }
                }
                else
                {
                    //Ruolo non trovato
                    throw new PisException("ROLE_NO_EXIST");
                }

                response.Role = roleResponse;
                response.Role.Registers = registerResponse;

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

        public static Services.Roles.GetRoles.GetRolesResponse GetRoles(Services.Roles.GetRoles.GetRolesRequest request)
        {
            Services.Roles.GetRoles.GetRolesResponse response = new Services.Roles.GetRoles.GetRolesResponse();

            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetRoles");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.UserID))
                {
                    throw new PisException("REQUIRED_USERID");
                }

                Domain.Role[] roleResponse = null;

                DocsPaVO.utente.Utente utenteDaCercare = null;
                ArrayList arrayRuoli = new ArrayList();

                if (!string.IsNullOrEmpty(request.UserID))
                {
                    utenteDaCercare = BusinessLogic.Utenti.UserManager.getUtente(request.UserID, Utils.GetIdAmministrazione(request.UserID));
                }
                else
                {
                    throw new PisException("REQUIRED_USERID");
                }

                if (utenteDaCercare != null)
                {
                    DocsPaVO.utente.InfoUtente infoUtenteDaCercare = new DocsPaVO.utente.InfoUtente(utenteDaCercare, Utils.GetRuoloPreferito(utenteDaCercare.idPeople));
                    arrayRuoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(infoUtenteDaCercare.idPeople);
                    if (arrayRuoli != null && arrayRuoli.Count > 0)
                    {
                        roleResponse = new Domain.Role[arrayRuoli.Count];
                        int i = 0;
                        foreach (DocsPaVO.utente.Ruolo rol in arrayRuoli)
                        {
                            Domain.Role roleTemp = new Domain.Role();
                            roleTemp.Code = rol.codiceRubrica;
                            roleTemp.Description = rol.descrizione;
                            roleTemp.Id = rol.idGruppo;
                            roleResponse[i] = roleTemp;
                            i++;
                        }
                    }
                    else
                    {
                        throw new PisException("USER_NO_ROLES");
                    }
                }
                else
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }

                response.Roles = roleResponse;

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

        public static Services.Roles.GetUsersInRole.GetUsersInRoleResponse GetUsersInRole(Services.Roles.GetUsersInRole.GetUsersInRoleRequest request)
        {
            Services.Roles.GetUsersInRole.GetUsersInRoleResponse response = new Services.Roles.GetUsersInRole.GetUsersInRoleResponse();

            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetUsersInRole");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.CodeRole))
                {
                    throw new PisException("REQUIRED_CODE_ROLE");
                }

                Domain.User[] userResponse = null;

                DocsPaVO.utente.Ruolo ruolo = null;

                if (!string.IsNullOrEmpty(request.CodeRole))
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(request.CodeRole);
                    if (ruolo != null)
                    {
                        List<DocsPaVO.utente.UserMinimalInfo> userList = BusinessLogic.Utenti.UserManager.GetUsersInRoleMinimalInfo(ruolo.idGruppo);
                        if (userList != null && userList.Count > 0)
                        {
                            int i = 0;
                            userResponse = new Domain.User[userList.Count];
                            foreach (DocsPaVO.utente.UserMinimalInfo minimalUser in userList)
                            {
                                Domain.User tempUser = new Domain.User();
                                DocsPaVO.utente.Utente utenteU = BusinessLogic.Utenti.UserManager.getUtenteById(minimalUser.SystemId);
                                tempUser.Description = utenteU.descrizione;
                                tempUser.Name = utenteU.nome;
                                tempUser.Surname = utenteU.cognome;
                                tempUser.UserId = utenteU.userId;
                                tempUser.Id = utenteU.idPeople;
                                userResponse[i] = tempUser;
                                i++;
                            }
                        }
                    }
                    else
                    {
                        //Ruolo non trovato
                        throw new PisException("ROLE_NO_EXIST");
                    }
                }
                else
                {
                    throw new PisException("REQUIRED_CODE_ROLE");
                }

                response.Users = userResponse;

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