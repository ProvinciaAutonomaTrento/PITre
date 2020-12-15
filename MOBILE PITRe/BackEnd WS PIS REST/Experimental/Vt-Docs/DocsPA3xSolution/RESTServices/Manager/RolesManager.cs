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
    public class RolesManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(RolesManager));

        public static async Task<GetRoleResponse> getRole(string token, string codeRole, string idRole)
        {
            logger.Info("begin getRole");
            GetRoleResponse response = new GetRoleResponse();
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

                if (string.IsNullOrEmpty(codeRole) && string.IsNullOrEmpty(idRole))
                {
                    throw new RestException("REQUIRED_CODE_OR_ID_ROLE");
                }

                if (!string.IsNullOrEmpty(codeRole) && !string.IsNullOrEmpty(idRole))
                {
                    throw new RestException("REQUIRED_ONLY_CODE_OR_ID_ROLE");
                }

                Role roleResponse = new Role();
                Register[] registerResponse = null;

                ruolo = null;
                ArrayList registri = new ArrayList();
                if (!string.IsNullOrEmpty(codeRole))
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(codeRole);
                }
                else
                {
                    if (!string.IsNullOrEmpty(idRole))
                    {
                        //Id gruppo
                        ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(idRole);
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
                        registerResponse = new Register[registri.Count];
                        int i = 0;
                        foreach (DocsPaVO.utente.Registro reg in registri)
                        {
                            Register regTemp = new Register();
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
                    throw new RestException("ROLE_NO_EXIST");
                }

                response.Role = roleResponse;
                response.Role.Registers = registerResponse;
                response.Code = GetRoleResponseCode.OK;


                logger.Info("end getRole");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getRole: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetRoleResponse();
                response.Code = GetRoleResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione uploadFileToDocument getRole: " + e);
                response = new GetRoleResponse();
                response.Code = GetRoleResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetRolesResponse> getRoles(string token, string userid)
        {
            logger.Info("begin getRoles");
            GetRolesResponse response = new GetRolesResponse();
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

                Role[] roleResponse = null;

                DocsPaVO.utente.Utente utenteDaCercare = null;
                ArrayList arrayRuoli = new ArrayList();

                if (!string.IsNullOrEmpty(userid))
                {
                    utenteDaCercare = BusinessLogic.Utenti.UserManager.getUtente(userid, infoUtente.idAmministrazione);
                }
                else
                {
                    throw new RestException("REQUIRED_USERID");
                }

                if (utenteDaCercare != null)
                {
                    DocsPaVO.utente.InfoUtente infoUtenteDaCercare = new DocsPaVO.utente.InfoUtente(utenteDaCercare, Utils.GetRuoloPreferito(utenteDaCercare.idPeople));
                    arrayRuoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(infoUtenteDaCercare.idPeople);
                    if (arrayRuoli != null && arrayRuoli.Count > 0)
                    {
                        roleResponse = new Role[arrayRuoli.Count];
                        int i = 0;
                        foreach (DocsPaVO.utente.Ruolo rol in arrayRuoli)
                        {
                            Role roleTemp = new Role();
                            roleTemp.Code = rol.codiceRubrica;
                            roleTemp.Description = rol.descrizione;
                            roleTemp.Id = rol.idGruppo;
                            roleResponse[i] = roleTemp;
                            i++;
                        }
                    }
                    else
                    {
                        throw new RestException("USER_NO_ROLES");
                    }
                }
                else
                {
                    //Utente non trovato
                    throw new RestException("USER_NO_EXIST");
                }

                response.Roles = roleResponse;

                response.Code = GetRolesResponseCode.OK;

                logger.Info("end getRoles");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getRoles: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetRolesResponse();
                response.Code = GetRolesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getRoles: " + e);
                response = new GetRolesResponse();
                response.Code = GetRolesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetRolesResponse> getRolesForEnabledActions(string token, string userId, string codeFunction)
        {
            logger.Info("begin getRolesForEnabledActions");
            GetRolesResponse response = new GetRolesResponse();
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
                if (string.IsNullOrEmpty(codeFunction))
                {
                    throw new RestException("REQUIRED_CODTIPOFUNC");
                }

                if (string.IsNullOrEmpty(userId))
                {
                    throw new RestException("REQUIRED_USERID");
                }

                Role[] roleResponse = null;

                DocsPaVO.utente.Utente utenteDaCercare = null;
                ArrayList arrayRuoli = new ArrayList();

                if (!string.IsNullOrEmpty(userId))
                {
                    utenteDaCercare = BusinessLogic.Utenti.UserManager.getUtente(userId, infoUtente.idAmministrazione);
                }
                else
                {
                    throw new RestException("REQUIRED_USERID");
                }

                if (utenteDaCercare != null)
                {
                    DocsPaVO.utente.InfoUtente infoUtenteDaCercare = new DocsPaVO.utente.InfoUtente(utenteDaCercare, Utils.GetRuoloPreferito(utenteDaCercare.idPeople));
                    arrayRuoli = BusinessLogic.Utenti.UserManager.getRuoliUtenteForEnabledActions(infoUtenteDaCercare.idPeople, codeFunction, infoUtente.idAmministrazione);
                    if (arrayRuoli != null && arrayRuoli.Count > 0)
                    {
                        roleResponse = new Role[arrayRuoli.Count];
                        int i = 0;
                        foreach (DocsPaVO.utente.Ruolo rol in arrayRuoli)
                        {
                            Role roleTemp = new Role();
                            roleTemp.Code = rol.codiceRubrica;
                            roleTemp.Description = rol.descrizione;
                            roleTemp.Id = rol.idGruppo;
                            roleResponse[i] = roleTemp;
                            i++;
                        }
                    }
                    else
                    {
                        throw new RestException("USER_NO_ROLES");
                    }
                }
                else
                {
                    //Utente non trovato
                    throw new RestException("USER_NO_EXIST");
                }

                response.Roles = roleResponse;
                response.Code = GetRolesResponseCode.OK;


                logger.Info("end getRolesForEnabledActions");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getRolesForEnabledActions: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetRolesResponse();
                response.Code = GetRolesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getRolesForEnabledActions: " + e);
                response = new GetRolesResponse();
                response.Code = GetRolesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetUsersResponse> getUsersInRole(string token, string codeRole)
        {
            logger.Info("begin getUsersInRole");
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

                if (string.IsNullOrEmpty(codeRole))
                {
                    throw new RestException("REQUIRED_CODE_ROLE");
                }

                User[] userResponse = null;

                ruolo = null;

                if (!string.IsNullOrEmpty(codeRole))
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(codeRole);
                    if (ruolo != null)
                    {
                        List<DocsPaVO.utente.UserMinimalInfo> userList = BusinessLogic.Utenti.UserManager.GetUsersInRoleMinimalInfo(ruolo.idGruppo);
                        if (userList != null && userList.Count > 0)
                        {
                            int i = 0;
                            userResponse = new User[userList.Count];
                            foreach (DocsPaVO.utente.UserMinimalInfo minimalUser in userList)
                            {
                                User tempUser = new User();
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
                        throw new RestException("ROLE_NO_EXIST");
                    }
                }
                else
                {
                    throw new RestException("REQUIRED_CODE_ROLE");
                }

                response.Users = userResponse;
                response.Code = GetUsersResponseCode.OK;

                logger.Info("end getUsersInRole");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getUsersInRole: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetUsersResponse();
                response.Code = GetUsersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getUsersInRole: " + e);
                response = new GetUsersResponse();
                response.Code = GetUsersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }
    }
}