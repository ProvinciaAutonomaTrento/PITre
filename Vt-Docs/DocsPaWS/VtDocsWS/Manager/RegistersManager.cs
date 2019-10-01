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
    public class RegistersManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(RegistersManager));

        public static Services.Registers.GetRegisterOrRF.GetRegisterOrRFResponse GetRegisterOrRF(Services.Registers.GetRegisterOrRF.GetRegisterOrRFRequest request)
        {
            Services.Registers.GetRegisterOrRF.GetRegisterOrRFResponse response = new Services.Registers.GetRegisterOrRF.GetRegisterOrRFResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetRegisterOrRF");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.CodeRegister) && string.IsNullOrEmpty(request.IdRegister))
                {
                    throw new PisException("REQUIRED_CODE_OR_ID_REGISTER");
                }

                if (!string.IsNullOrEmpty(request.CodeRegister) && !string.IsNullOrEmpty(request.IdRegister))
                {
                    throw new PisException("REQUIRED_ONLY_CODE_OR_ID_REGISTER");
                }


                Domain.Register registerResponse = new Domain.Register();

                DocsPaVO.utente.Registro registro = null;

                if (!string.IsNullOrEmpty(request.CodeRegister))
                {
                    registro = BusinessLogic.Utenti.RegistriManager.getRegistroByCodAOO(request.CodeRegister, infoUtente.idAmministrazione);
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.IdRegister))
                    {
                        try
                        {
                            registro = BusinessLogic.Utenti.RegistriManager.getRegistro(request.IdRegister);
                        }
                        catch
                        {
                            //Registro non trovato
                            throw new PisException("REGISTER_NOT_FOUND");
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
                    throw new PisException("REGISTER_NOT_FOUND");
                }

                response.Register = registerResponse;

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

        public static Services.Registers.GetRegistersOrRF.GetRegistersOrRFResponse GetRegistersOrRF(Services.Registers.GetRegistersOrRF.GetRegistersOrRFRequest request)
        {
            Services.Registers.GetRegistersOrRF.GetRegistersOrRFResponse response = new Services.Registers.GetRegistersOrRF.GetRegistersOrRFResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetRegistersOrRF");

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

                Domain.Register[] registerResponse = null;

                DocsPaVO.utente.Registro[] registri = null;

                DocsPaVO.utente.Ruolo ruolo = null;
                ArrayList registriLista = new ArrayList();

                if (!string.IsNullOrEmpty(request.CodeRole))
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(request.CodeRole);
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.IdRole))
                    {
                        try
                        {
                            //Id gruppo
                            ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(request.IdRole);
                        }
                        catch
                        {
                            //Ruolo non trovato
                            throw new PisException("ROLE_NO_EXIST");
                        }
                    }
                }

                if (ruolo != null)
                {
                    //Prendi i registri e gli rf
                    if ((!request.OnlyRegisters && !request.OnlyRF) || (request.OnlyRegisters && request.OnlyRF))
                    {
                        registriLista = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(ruolo.systemId, string.Empty, string.Empty);
                    }
                    else
                    {
                        //Prendi solo i registri
                        if (request.OnlyRegisters && !request.OnlyRF)
                        {
                            registriLista = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(ruolo.systemId, "0", string.Empty);
                        }
                        else
                        {
                            //Prendi solo gli RF
                            if (request.OnlyRF && !request.OnlyRegisters)
                            {
                                registriLista = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(ruolo.systemId, "1", string.Empty);
                            }
                        }
                    }
                    if (registriLista != null && registriLista.Count > 0)
                    {
                        registerResponse = new Domain.Register[registriLista.Count];
                        int i = 0;
                        foreach (DocsPaVO.utente.Registro reg in registriLista)
                        {
                            Domain.Register regTemp = new Domain.Register();
                            regTemp = Utils.GetRegister(reg);
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

                response.Registers = registerResponse;

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