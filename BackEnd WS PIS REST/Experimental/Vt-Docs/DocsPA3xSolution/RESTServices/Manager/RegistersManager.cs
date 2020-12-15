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
    public class RegistersManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(RegistersManager));

        public static async Task<GetRegisterOrRFResponse> getRegisterOrRF(string token, string codeRegister, string idRegister)
        {
            logger.Info("begin getRegisterOrRF");
            GetRegisterOrRFResponse response = new GetRegisterOrRFResponse();
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
                if (string.IsNullOrEmpty(codeRegister) && string.IsNullOrEmpty(idRegister))
                {
                    throw new RestException("REQUIRED_CODE_OR_ID_REGISTER");
                }

                if (!string.IsNullOrEmpty(codeRegister) && !string.IsNullOrEmpty(idRegister))
                {
                    throw new RestException("REQUIRED_ONLY_CODE_OR_ID_REGISTER");
                }


                Register registerResponse = new Register();

                DocsPaVO.utente.Registro registro = null;

                if (!string.IsNullOrEmpty(codeRegister))
                {
                    registro = BusinessLogic.Utenti.RegistriManager.getRegistroByCodAOO(codeRegister, infoUtente.idAmministrazione);
                }
                else
                {
                    if (!string.IsNullOrEmpty(idRegister))
                    {
                        try
                        {
                            registro = BusinessLogic.Utenti.RegistriManager.getRegistro(idRegister);
                        }
                        catch
                        {
                            //Registro non trovato
                            throw new RestException("REGISTER_NOT_FOUND");
                        }
                    }
                }

                if (registro != null)
                {
                    registerResponse = Utils.GetRegister(registro);
                }
                else
                {
                    //Registro non trovato
                    throw new RestException("REGISTER_NOT_FOUND");
                }

                response.Register = registerResponse;

                response.Code = GetRegisterOrRFResponseCode.OK;

                logger.Info("end getRegisterOrRF");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getRegisterOrRF: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetRegisterOrRFResponse();
                response.Code = GetRegisterOrRFResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getRegisterOrRF: " + e);
                response = new GetRegisterOrRFResponse();
                response.Code = GetRegisterOrRFResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetRegistersOrRFResponse> getRegistersOrRF(string token, string codeRole, string idRole, string RegOrRF)
        {
            logger.Info("begin getRegistersOrRF");
            GetRegistersOrRFResponse response = new GetRegistersOrRFResponse();
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

                Register[] registerResponse = null;

                DocsPaVO.utente.Registro[] registri = null;

                DocsPaVO.utente.Ruolo ruoloX = null;
                ArrayList registriLista = new ArrayList();

                if (!string.IsNullOrEmpty(codeRole))
                {
                    ruoloX = BusinessLogic.Utenti.UserManager.getRuoloByCodice(codeRole);
                }
                else
                {
                    if (!string.IsNullOrEmpty(idRole))
                    {
                        try
                        {
                            //Id gruppo
                            ruoloX = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(idRole);
                        }
                        catch
                        {
                            //Ruolo non trovato
                            throw new RestException("ROLE_NO_EXIST");
                        }
                    }
                    else ruoloX = ruolo;
                }

                if (ruoloX != null)
                {
                    //Prendi i registri e gli rf
                    if (string.IsNullOrWhiteSpace(RegOrRF) || (RegOrRF.ToUpper() != "RF" && RegOrRF.ToUpper() != "REG"))
                    {
                        registriLista = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(ruolo.systemId, string.Empty, string.Empty);
                    }
                    else
                    {
                        //Prendi solo i registri
                        if (RegOrRF.ToUpper() == "REG")
                        {
                            registriLista = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(ruolo.systemId, "0", string.Empty);
                        }
                        else
                        {
                            //Prendi solo gli RF
                            if (RegOrRF.ToUpper() == "RF")
                            {
                                registriLista = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(ruolo.systemId, "1", string.Empty);
                            }
                        }
                    }
                    if (registriLista != null && registriLista.Count > 0)
                    {
                        registerResponse = new Register[registriLista.Count];
                        int i = 0;
                        foreach (DocsPaVO.utente.Registro reg in registriLista)
                        {
                            Register regTemp = new Register();
                            regTemp = Utils.GetRegister(reg);
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

                response.Registers = registerResponse;
                response.Code = GetRegistersOrRFResponseCode.OK;
                
                logger.Info("end getRegistersOrRF");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getRegistersOrRF: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetRegistersOrRFResponse();
                response.Code = GetRegistersOrRFResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getRegistersOrRF: " + e);
                response = new GetRegistersOrRFResponse();
                response.Code = GetRegistersOrRFResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }
    }
}