using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RESTServices.Model;
using System.Threading.Tasks;
using log4net;
using DocsPaVO.utente;

namespace RESTServices.Manager
{
    public class AuthenticateManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(AuthenticateManager));
        
        public static async Task<Model.Responses.AuthenticateResponse> GetAuthenticationToken(Model.Requests.AuthenticateRequest request)
        {
            logger.Info("Authenticate - BEGIN");
            Model.Responses.AuthenticateResponse response = new Model.Responses.AuthenticateResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                if (request == null || string.IsNullOrEmpty(request.Username))
                {
                    throw new Exception("MISSING_PARAMETER");
                }

                if (string.IsNullOrEmpty(request.CodeAdm.Trim())|| string.IsNullOrEmpty(request.CodeApplication.Trim()))
                {
                    throw new Exception("MISSING_PARAMETER");
                }
                utente = BusinessLogic.Utenti.UserManager.getUtenteByCodice(request.Username, request.CodeAdm);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new Exception("USER_NO_EXIST");
                }
                else
                {
                    string token = null;
                    logger.Debug("Verifico le chiavi di configurazione");
                    if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SSOLOGIN")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SSOLOGIN").Equals("1"))
                    {
                        DocsPaVO.utente.Ruolo ruolo = null;
                        bool ruoloOK = false;
                        if (!string.IsNullOrEmpty(request.CodeRole))
                        {
                            ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(request.CodeRole);
                                if (ruolo == null)
                                {
                                    //Ruolo non trovato
                                    throw new Exception("ROLE_NO_EXIST");
                                }
                                else
                                {
                                    //arrayRuoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(utente.idPeople);
                                    //if (arrayRuoli != null && arrayRuoli.Count > 0)
                                    //{
                                    //    foreach (DocsPaVO.utente.Ruolo rol in arrayRuoli)
                                    //    {
                                    //        if (rol.codiceRubrica.ToUpper() == ruolo.codice.ToUpper())
                                    //            ruoloOK = true;
                                    //    }
                                    //    if (!ruoloOK) throw new PisException("ROLE_NO_EXIST");
                                    //}

                                    List<UserMinimalInfo> utentiinruolo = BusinessLogic.Utenti.UserManager.GetUsersInRoleMinimalInfo(ruolo.idGruppo);
                                    if (utentiinruolo != null && utentiinruolo.Count > 0)
                                    {
                                        foreach (UserMinimalInfo umi in utentiinruolo)
                                        {
                                            if (umi.SystemId == utente.idPeople) ruoloOK = true;
                                        }
                                    }
                                    if (!ruoloOK) throw new Exception("ROLE_NO_EXIST");
                                }
                            
                        }
                        else
                        {
                            ruolo = Utils.GetRuoloPreferito(utente.idPeople);
                            if (ruolo == null)
                            {
                                //L'utente non ha ruoli
                                throw new Exception("USER_NO_ROLE");
                            }
                        }
                        if (string.IsNullOrEmpty(utente.dst))
                        {
                            //Reperimento token superutente
                            utente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
                        }
                        logger.Debug("Chiavi di configurazione presenti");
                        token = Utils.GetToken(utente, ruolo, request.CodeApplication);
                        logger.Debug("token generato.");
                    }
                    if (string.IsNullOrEmpty(token))
                    {
                        logger.Error("Errore nella generazione del token.");
                        throw new Exception("Errore nella generazione del token.");
                    }
                    else
                    {
                        response.Token = token;
                        response.Code = Model.Responses.AuthenticateResponseCode.OK;
                    }

                    
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                response.Code = Model.Responses.AuthenticateResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = ex.Message;
            }
            logger.Info("Authenticate - END");
            return response;
        }
    }
}