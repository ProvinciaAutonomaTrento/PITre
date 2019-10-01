using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.utente;
using VtDocsWS.WebServices;
using log4net;

namespace VtDocsWS.Manager
{
    public class TokenManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(TokenManager));

        public static Services.Token.GetAuthenticationToken.GetAuthenticationTokenResponse GetAuthenticationToken(Services.Token.GetAuthenticationToken.GetAuthenticationTokenRequest request)
        {
            Services.Token.GetAuthenticationToken.GetAuthenticationTokenResponse response = new Services.Token.GetAuthenticationToken.GetAuthenticationTokenResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                if (request == null || string.IsNullOrEmpty(request.UserName))
                {
                    throw new PisException("MISSING_PARAMETER");
                }

                if (string.IsNullOrEmpty(request.CodeAdm))
                {
                    throw new PisException("MISSING_PARAMETER");
                }
                utente = BusinessLogic.Utenti.UserManager.getUtenteByCodice(request.UserName, request.CodeAdm);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                else
                {
                    string token = null;
                    logger.Debug("Verifico le chiavi di configurazione");
                    if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SSOLOGIN")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SSOLOGIN").Equals("1"))
                    {
                        logger.Debug("Chiavi di configurazione presenti");
                        token = DocsPaUtils.Security.SSOAuthTokenHelper.Generate(request.UserName.ToUpper());
                        logger.Debug("token generato.");
                    }
                    if (string.IsNullOrEmpty(token))
                    {
                        logger.Error("Errore nella generazione del token.");
                        throw new PisException("APPLICATION_ERROR");
                    }
                    else
                    {
                        response.AuthenticationToken = token;
                    }

                    response.Success = true;
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

        public static Services.Token.GetToken.GetTokenResponse GetToken(Services.Token.GetToken.GetTokenRequest request)
        {
            Services.Token.GetToken.GetTokenResponse response = new Services.Token.GetToken.GetTokenResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                bool sysExt = false;

                if (request == null || string.IsNullOrEmpty(request.CodeAdm))
                {
                    throw new PisException("MISSING_PARAMETER");
                }
                if (string.IsNullOrEmpty(request.UserName) && string.IsNullOrEmpty(request.CodeApplication))
                {
                    throw new PisException("MISSING_PARAMETER");
                }
                if (!string.IsNullOrEmpty(request.CodeApplication) && string.IsNullOrEmpty(request.UserName))
                {
                    sysExt = true;
                }
                string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(request.CodeAdm);
                if (!sysExt)
                {
                    utente = BusinessLogic.Utenti.UserManager.getUtenteByCodice(request.UserName, request.CodeAdm);
                    if (utente == null)
                    {
                        //Utente non trovato
                        throw new PisException("USER_NO_EXIST");
                    }
                    else
                    {
                        string token = null;
                        logger.Debug("Verifico le chiavi di configurazione");
                        if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SSOLOGIN")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SSOLOGIN").Equals("1"))
                        {
                            logger.Debug("Chiavi di configurazione presenti");
                            token = DocsPaUtils.Security.SSOAuthTokenHelper.Generate(request.UserName.ToUpper());
                            logger.Debug("token generato.");
                        }
                        if (string.IsNullOrEmpty(token))
                        {
                            logger.Error("Errore nella generazione del token.");
                            throw new PisException("APPLICATION_ERROR");
                        }
                        else
                        {
                            response.AuthenticationToken = token;
                            // Devo creare la chiave di sistema per la durata standard del token. Per ora 20 minuti
                            string durataToken = "";
                            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_PIS_TOKEN_DURATION")))
                                                            durataToken= DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_PIS_TOKEN_DURATION");
                            else
                                durataToken="20";
                            response.TokenDuration = durataToken;
                        }


                        response.Success = true;
                    }
                }
                else
                {
                    //string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(request.CodeAdm);
                    logger.Debug("Prelievo del token per il sistema esterno "+request.CodeApplication+ " nell'amministrazione "+ request.CodeAdm);
                    DocsPaVO.amministrazione.SistemaEsterno sisEsterno = BusinessLogic.Amministrazione.SistemiEsterni.getSistemaEsterno(idAmm, request.CodeApplication);
                    if (sisEsterno == null)
                    {
                        logger.ErrorFormat("Errore: Sistema esterno {0} non trovato nell'amministrazione {1}", request.CodeApplication, request.CodeAdm);
                        throw new Exception(String.Format("Errore: Sistema esterno {0} non trovato nell'amministrazione {1}", request.CodeApplication, request.CodeAdm));
                    }
                    utente = BusinessLogic.Utenti.UserManager.getUtenteByCodice(sisEsterno.UserIdAssociato, request.CodeAdm);
                    if (utente == null)
                    {
                        //Utente non trovato
                        throw new PisException("USER_NO_EXIST");
                    }
                    else
                    {
                        string token = null;
                        logger.Debug("Verifico le chiavi di configurazione");
                        if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SSOLOGIN")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SSOLOGIN").Equals("1"))
                        {
                            logger.Debug("Chiavi di configurazione presenti");
                            token = DocsPaUtils.Security.SSOAuthTokenHelper.Generate(sisEsterno.UserIdAssociato.ToUpper());
                            logger.Debug("token generato.");
                        }
                        if (string.IsNullOrEmpty(token))
                        {
                            logger.Error("Errore nella generazione del token.");
                            throw new PisException("APPLICATION_ERROR");
                        }
                        else
                        {
                            response.AuthenticationToken = token;
                            response.Username = sisEsterno.UserIdAssociato;
                            response.TokenDuration = sisEsterno.TokenPeriod.ToString();
                        }

                        response.Success = true;
                    }
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
    }
}