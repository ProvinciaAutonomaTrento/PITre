// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Authenticate.Authenticate
{

    // Richiede libreria MediatR
    public class AuthenticateCommandHandler : IRequestHandler<AuthenticateCommand,AuthenticateCommandResponse>
    {
        #region Public Members

        public AuthenticateCommandHandler(ILogger<AuthenticateCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<AuthenticateCommandResponse> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Authenticate - BEGIN");

            AuthenticateCommandResponse response = new AuthenticateCommandResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                if (request == null || string.IsNullOrEmpty(request.Username))
                {
                    throw new Exception("MISSING_PARAMETER");
                }

                if (string.IsNullOrEmpty(request.CodeAdm.Trim()) || string.IsNullOrEmpty(request.CodeApplication.Trim()))
                {
                    throw new Exception("MISSING_PARAMETER");
                }

                //utente = BusinessLogic.Utenti.UserManager.getUtenteByCodice(request.Username, request.CodeAdm);
                utente = Utils.DBUtils.getUtenteByCodice(request.Username, request.CodeAdm, _pi3DbContext);

                if (utente == null)
                {
                    //Utente non trovato
                    throw new Exception("USER_NO_EXIST");
                }
                else
                {
                    string token = null;
                    _logger.LogDebug("Verifico le chiavi di configurazione");
                    var chiaveSSO = _pi3DbContext.ChiaviConfigurazioneEntities.Where(c => c.VAR_CODICE == "BE_SSOLOGIN" && c.ID_AMM == 0).Select(c => c.VAR_VALORE).FirstOrDefault();
                    //if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SSOLOGIN")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SSOLOGIN").Equals("1"))
                    if(!string.IsNullOrWhiteSpace(chiaveSSO) && chiaveSSO=="1")
                    {
                        DocsPaVO.utente.Ruolo ruolo = null;
                        bool ruoloOK = false;
                        if (!string.IsNullOrEmpty(request.CodeRole))
                        {
                            //ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(request.CodeRole);
                            ruolo = DBUtils.getRuoloByCodice(request.CodeRole, _pi3DbContext);
                            if (ruolo == null)
                            {
                                //Ruolo non trovato
                                throw new Exception("ROLE_NO_EXIST");
                            }
                            else
                            {

                                //List<UserMinimalInfo> utentiinruolo = BusinessLogic.Utenti.UserManager.GetUsersInRoleMinimalInfo(ruolo.idGruppo);
                                List<UserMinimalInfo> utentiinruolo = DBUtils.GetUsersInRoleMinimalInfo(ruolo.idGruppo, _pi3DbContext);
                                    if (utentiinruolo != null && utentiinruolo.Count > 0)
                                {
                                    foreach (UserMinimalInfo umi in utentiinruolo)
                                    {
                                        if (umi.SystemId == utente.idPeople) { ruoloOK = true; break; }
                                    }
                                }
                                if (!ruoloOK) throw new Exception("ROLE_NO_EXIST");
                            }

                        }
                        else
                        {
                            //ruolo = Utils.GetRuoloPreferito(utente.idPeople);
                            ruolo = DBUtils.getRuoloPreferito(utente.idPeople, _pi3DbContext);
                            if (ruolo == null)
                            {
                                //L'utente non ha ruoli
                                throw new Exception("USER_NO_ROLE");
                            }
                        }
                        if (string.IsNullOrEmpty(utente.dst))
                        {
                            //Reperimento token superutente
                            //utente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

                            // applico la sola implementazione ETDOCS, non credo sarà disponibile per altri documentali.
                            utente.dst= Guid.NewGuid().ToString().Replace("-", string.Empty);
                        }
                        _logger.LogDebug("Chiavi di configurazione presenti");
                        token = GetToken(utente, ruolo, request.CodeApplication, _logger);
                        _logger.LogDebug("token generato.");
                    }
                    if (string.IsNullOrEmpty(token))
                    {
                        _logger.LogError("Errore nella generazione del token.");
                        throw new Exception("Errore nella generazione del token.");
                    }
                    else
                    {
                        response.Token = token;
                        response.Code = AuthenticateResponseCode.OK;
                    }


                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.Code = AuthenticateResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = ex.Message;
            }
            _logger.LogInformation("Authenticate - END");
            return response;
        }

        private static string GetToken(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo, string codeApplication, ILogger<AuthenticateCommandHandler> logger)
        {
            //Controllo Correttezza Ruolo
            bool okRuolo = false;
            if (utente != null && utente.ruoli != null && utente.ruoli.Count() > 0)
            {
                foreach (DocsPaVO.utente.Ruolo rl in utente.ruoli)
                {
                    if (rl.idGruppo == ruolo.idGruppo)
                        okRuolo = true;
                }
            }
            else okRuolo = true;

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
                    clearToken += utente.urlWA + "|";
                    if (!string.IsNullOrEmpty(codeApplication))
                        clearToken += codeApplication.ToUpper() + "|";
                    else
                        clearToken += "INTEGRAZIONE_NON_CENSITA|";
                    clearToken += DateTime.Now.ToString("s");

                    tokenDiAutenticazione = Encrypt(clearToken, logger);
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

        private static string Encrypt(string toEncrypt, ILogger<AuthenticateCommandHandler> logger)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
                byte[] resultArray;

                //La chiave deve essere di 24 caratteri
                string key = "ValueTeamDocsPa3Services";

                keyArray = UTF8Encoding.UTF8.GetBytes(key);
                                
                using(Aes aes = Aes.Create())
                {
                    aes.Key = keyArray;
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform cTransform = aes.CreateEncryptor();
                    resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                    aes.Clear();
                }
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,ex.Message);
                throw;
            }
        }
        #endregion

        #region Private Members

        protected readonly ILogger<AuthenticateCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }
        
}
