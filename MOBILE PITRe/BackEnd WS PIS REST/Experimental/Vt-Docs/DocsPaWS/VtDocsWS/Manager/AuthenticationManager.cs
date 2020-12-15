using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.utente;
using VtDocsWS.WebServices;
using log4net;

namespace VtDocsWS.Manager
{
    public class AuthenticationManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(AuthenticationManager));

        public static Services.Authentication.LogIn.LogInResponse LogIn(Services.Authentication.LogIn.LogInRequest request)
        {
            Services.Authentication.LogIn.LogInResponse response = new Services.Authentication.LogIn.LogInResponse();

            try
            {
                DocsPaVO.utente.Utente utente = null;
                string idWebSession = string.Empty;
                string ipAddress = string.Empty;

                if (request != null && (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password)))
                {
                    throw new PisException("MISSING_PARAMETER");
                }

                DocsPaVO.utente.UserLogin.LoginResult loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;

                DocsPaVO.utente.UserLogin login = new DocsPaVO.utente.UserLogin
                {
                    UserName = request.UserName,
                    Password = request.Password,
                };

                utente = BusinessLogic.Utenti.Login.loginMethod(login, out loginResult, true, idWebSession, out ipAddress);

                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }

                // Giovanni Olivari - 06/11/2012
                // Se l'utente non ha Ruoli viene generata un'eccezione prima di questo blocco di codice,
                // quindi arrivati qui posso essere sicuro di avere almeno un ruolo, prendo il primo della lista
                //
                DocsPaVO.utente.Ruolo ruolo = null;
                ruolo = (DocsPaVO.utente.Ruolo)utente.ruoli[0];

                if (loginResult == DocsPaVO.utente.UserLogin.LoginResult.OK)
                {
                    // Generazione del token di autenticazione
                    response.AuthenticationToken = CreateAuthToken(utente, ruolo);

                    BusinessLogic.Utenti.Login.logoff(utente.userId, utente.idAmministrazione, utente.dst);
                }
                else
                {
                    throw new PisException("MISSING_PARAMETER");
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

        public static Services.Authentication.LogOut.LogOutResponse LogOut(Services.Authentication.LogOut.LogOutRequest request)
        {
            Services.Authentication.LogOut.LogOutResponse response = new Services.Authentication.LogOut.LogOutResponse();

            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "LogOut");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                BusinessLogic.Utenti.Login.logoff(utente.userId, utente.idAmministrazione, utente.dst);

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

        public static Services.Authentication.Authenticate.AuthenticateResponse Authenticate(Services.Authentication.Authenticate.AuthenticateRequest request)
        {
            Services.Authentication.Authenticate.AuthenticateResponse response = new Services.Authentication.Authenticate.AuthenticateResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                if (request == null || string.IsNullOrEmpty(request.UserName))
                {
                    throw new PisException("MISSING_PARAMETER");
                }

                if (string.IsNullOrEmpty(request.Password))
                {
                    throw new PisException("MISSING_PARAMETER");
                }
                
                DocsPaVO.utente.UserLogin userlog = new UserLogin(request.UserName,request.Password,null);
                System.Collections.ArrayList listaAmmin = BusinessLogic.Utenti.UserManager.getListaIdAmmUtente(userlog);

                if (listaAmmin != null && listaAmmin.Count > 0)
                {
                    if (listaAmmin.Count == 1)
                        userlog.IdAmministrazione = listaAmmin[0].ToString();
                    else
                    {
                        throw new PisException("APPLICATION_ERROR");
                    }
                }
                if (listaAmmin == null) throw new PisException("APPLICATION_ERROR");
                utente = BusinessLogic.Utenti.UserManager.getUtente(request.UserName, userlog.IdAmministrazione);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("APPLICATION_ERROR");
                }
                string encPass = BusinessLogic.Amministrazione.AmministraManager.GetPasswordUtenteMultiAmm(request.UserName.ToUpper());
                encPass = encPass.Split('^')[0];
                if (DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(System.Text.Encoding.Unicode.GetBytes(request.Password)) != encPass)
                {
                    throw new PisException("APPLICATION_ERROR");
                }

                
                DocsPaVO.utente.Ruolo ruolo = Utils.GetRuoloPreferito(utente.idPeople);
                if (ruolo == null)
                {
                    //L'utente non ha ruoli
                    throw new PisException("APPLICATION_ERROR");
                }

                DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);


                utente.ruoli = new System.Collections.ArrayList();
                utente.ruoli.Add(ruolo);

                response.AuthenticationToken = CreateAuthToken(utente, ruolo);
                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(utente.systemId);
                response.User = Utils.GetCorrespondent(corr, infoUtente);

                //response.User = new Domain.Correspondent();
                //response.User.Name = utente.nome;
                //response.User.Surname = utente.cognome;
                //response.User.Location = utente.sede;
                //response.User.Email = utente.email;
                //response.User.Id = utente.systemId;
                //response.User.CorrespondentType = utente.tipoCorrispondente;
                //response.User.Type = "I";
                //response.User.Description = utente.descrizione;
                //response.User.Code = utente.userId;
                

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

        private static string CreateAuthToken(Utente utente, Ruolo ruolo)
        {
            string token = DocsPaUtils.Security.SSOAuthTokenHelper.Generate(utente.userId);

            string ss_token = GetToken(utente, ruolo);
            return ss_token;
        }

        private static string GetToken(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
        {
            //Controllo Correttezza Ruolo
            bool okRuolo = false;
            foreach (DocsPaVO.utente.Ruolo rl in utente.ruoli)
            {
                if (rl.idGruppo == ruolo.idGruppo)
                    okRuolo = true;
            }

            if (okRuolo)
            {
                string tokenDiAutenticazione = null;
                try
                {
                    string clearToken = string.Empty;
                    clearToken += ruolo.systemId + "|";
                    clearToken += utente.idPeople + "|";
                    clearToken += ruolo.idGruppo + "|";
                    clearToken += utente.dst + "|";
                    clearToken += utente.idAmministrazione + "|";
                    clearToken += utente.userId + "|";
                    clearToken += utente.sede + "|";
                    clearToken += utente.urlWA;

                    tokenDiAutenticazione = Utils.Encrypt(clearToken);
                }
                catch (Exception e)
                {
                    //  logger.Debug("Errore durante il GetInfoUtente.", e);
                }

                tokenDiAutenticazione = "SSO=" + tokenDiAutenticazione;
                return tokenDiAutenticazione;
            }
            else
            {
                //logger.Debug("L'utente : " + utente.descrizione + " non appartiene al ruolo : " + ruolo.descrizione);
                return null;
            }
        }
    }
}