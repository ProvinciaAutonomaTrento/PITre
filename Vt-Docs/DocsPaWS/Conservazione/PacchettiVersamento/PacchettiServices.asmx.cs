using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using DocsPaWS.Conservazione.PacchettiVersamento.Dominio;
using System.Collections;
using DocsPaVO.utente;

namespace DocsPaWS.Conservazione.PacchettiVersamento
{
    /// <summary>
    /// 
    /// </summary>
    [WebService(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class PacchettiServices : System.Web.Services.WebService, DocsPaWS.Conservazione.PacchettiVersamento.Services.IServices
    {
        #region Servizi per l'autenticazione

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public Services.LogIn.LogInResponse LogIn(Services.LogIn.LogInRequest request)
        {
            Services.LogIn.LogInResponse response = new Services.LogIn.LogInResponse();

            try
            {
                DocsPaVO.utente.Utente utente = null;
                string idWebSession = string.Empty;
                string ipAddress = string.Empty;

                DocsPaVO.utente.UserLogin.LoginResult loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;

                DocsPaVO.utente.UserLogin login = new DocsPaVO.utente.UserLogin
                {
                    UserName = request.UserName,
                    Password = request.Password
                };

                utente = BusinessLogic.Utenti.Login.loginMethod(login, out loginResult, true, idWebSession, out ipAddress);

                if (loginResult == DocsPaVO.utente.UserLogin.LoginResult.OK)
                {
                    // Generazione del token di autenticazione
                    response.AuthenticationToken = this.CreateAuthToken(utente.userId);

                    BusinessLogic.Utenti.Login.logoff(utente.userId, utente.idAmministrazione, utente.dst);
                }
                else
                {
                    throw new PacchettiException(ErrorCodes.AUTENTICAZIONE_FALLITA, string.Format(ErrorDescriptions.AUTENTICAZIONE_FALLITA, loginResult));
                }

                response.Success = true;
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[WebMethod()]
        //public Services.LogOut.LogOutResponse LogOut(Services.LogOut.LogOutRequest request)
        //{
        //    Services.LogOut.LogOutResponse response = new Services.LogOut.LogOutResponse();

        //    try
        //    {
        //        // Validazione token di autenticazione
        //        this.CheckAuthenticationToken(request);


        //        response.Success = true;
        //    }
        //    catch (PacchettiException pacchettiEx)
        //    {
        //        response.Error = new Services.ResponseError
        //        {
        //            Code = pacchettiEx.ErrorCode,
        //            Description = pacchettiEx.Message
        //        };

        //        response.Success = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Error = new Services.ResponseError
        //        {
        //            Code = ErrorCodes.ERRORE_NON_GESTITO,
        //            Description = ex.Message
        //        };

        //        response.Success = false;
        //    }

        //    return response;
        //}

        /// <summary>
        /// Creazione del token di autenticazione
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        protected virtual string CreateAuthToken(string userId)
        {
            return DocsPaUtils.Security.SSOAuthTokenHelper.Generate(userId.ToLowerInvariant());

            //Security.AuthenticationTokenManager.SSO.SSOAuthTokenManager tokenMng = new Security.AuthenticationTokenManager.SSO.SSOAuthTokenManager();

            //return tokenMng.Generate(userId.Trim().ToUpper(),
            //                    Security.AuthenticationTokenManager.AuthTokenManager.TokenTypeEnum.Explicit,
            //                    0,
            //                    Security.AuthenticationTokenManager.AuthTokenManager.KeyLengthEnum.Sixteen,
            //                    Security.AuthenticationTokenManager.AuthTokenManager.KeyLengthEnum.Sixteen);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        protected virtual void RestoreAuthToken(string userId, string token)
        {
            DocsPaUtils.Security.SSOAuthTokenHelper.Restore(userId.ToLowerInvariant(), token);

            //Security.AuthenticationTokenManager.SSO.SSOAuthTokenManager tokenMng = new Security.AuthenticationTokenManager.SSO.SSOAuthTokenManager();

            //if (tokenMng.IsAuthToken(token))
            //{
            //    string clearToken = tokenMng.Restore(userId.Trim().ToUpper(), token);

            //}
        }

        /// <summary>
        /// Validazione del token di autenticazione
        /// </summary>
        /// <param name="request"></param>
        protected virtual void CheckAuthenticationToken(Services.Request request)
        {
            if (string.IsNullOrEmpty(request.UserName))
                throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "UserName"));

            if (string.IsNullOrEmpty(request.AuthenticationToken))
                throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "AuthenticationToken"));

            if (!DocsPaUtils.Security.SSOAuthTokenHelper.IsAuthToken(request.AuthenticationToken))
                throw new PacchettiException(ErrorCodes.TOKEN_NON_VALIDO, ErrorDescriptions.TOKEN_NON_VALIDO);

            this.RestoreAuthToken(request.UserName, request.AuthenticationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private DocsPaVO.utente.InfoUtente Impersonate(string userName)
        {
            // Reperimento oggetto utente richiedente
            DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(userName, GetIdAmministrazione(userName));

            if (utente == null)
                throw new ApplicationException(string.Format("Utente {0} non trovato", userName));

            DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, GetRuoloPreferito(utente.idPeople));

            // Reperimento token superutente
            infoUtente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

            return infoUtente;
        }

        /// <summary>
        /// Reperimento dell'id dell'amministrazione di appartenenza dell'utente
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GetIdAmministrazione(string userId)
        {
            string idAmministrazione = BusinessLogic.Utenti.UserManager.getIdAmmUtente(userId);

            if (string.IsNullOrEmpty(idAmministrazione))
                throw new ApplicationException(string.Format("Nessuna amministrazione trovata per l'utente {0}", userId));

            return idAmministrazione;
        }

        /// <summary>
        /// Reperimento del primo ruolo di appartenenza dell'utente
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Ruolo GetRuoloPreferito(string idPeople)
        {
            DocsPaVO.utente.Ruolo[] ruoli = (DocsPaVO.utente.Ruolo[])BusinessLogic.Utenti.UserManager.getRuoliUtente(idPeople).ToArray(typeof(DocsPaVO.utente.Ruolo));

            if (ruoli != null && ruoli.Length > 0)
                return ruoli[0];
            else
                throw new ApplicationException("L'utente non non risulta associato ad alcun ruolo");
        }

        #endregion

        #region Servizi per la gestione dell'istanza di conservazione

        ///// <summary>
        ///// Servizio per la creazione di una nuova istanza di conservazione
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        [WebMethod()]
        public Services.CreateIstanza.CreateIstanzaResponse CreateIstanza(Services.CreateIstanza.CreateIstanzaRequest request)
        {
            Services.CreateIstanza.CreateIstanzaResponse response = new Services.CreateIstanza.CreateIstanzaResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                // 2. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    // 3. Creazione istanza di conservazione
                    string tipoIstanza = string.Empty;
                    bool consolidaDocumenti = false;

                    if (request.DatiIstanza.Tipo == Dominio.TipiIstanzaEnum.Consolidata)
                    {
                        tipoIstanza = DocsPaConservazione.TipoIstanzaConservazione.CONSERVAZIONE_CONSOLIDATA;
                        consolidaDocumenti = true;
                    }
                    else if (request.DatiIstanza.Tipo == Dominio.TipiIstanzaEnum.NonConsolidata)
                    {
                        tipoIstanza = DocsPaConservazione.TipoIstanzaConservazione.CONSERVAZIONE_NON_CONSOLIDATA;
                        consolidaDocumenti = false;
                    }
                    else if (request.DatiIstanza.Tipo == Dominio.TipiIstanzaEnum.Esibizione)
                    {
                        tipoIstanza = DocsPaConservazione.TipoIstanzaConservazione.ESIBIZIONE;
                        consolidaDocumenti = false;
                    }
                    else if (request.DatiIstanza.Tipo == Dominio.TipiIstanzaEnum.Interna)
                    {
                        tipoIstanza = DocsPaConservazione.TipoIstanzaConservazione.CONSERVAZIONE_INTERNA;
                        consolidaDocumenti = request.DatiIstanza.ConsolidaDocumentiSeInterna;
                    }

                    response.DatiIstanza = request.DatiIstanza;
                    response.DatiIstanza.Id = BusinessLogic.Documenti.areaConservazioneManager.CreateIstanzaConservazione(
                                                            infoUtente,
                                                            response.DatiIstanza.Descrizione,
                                                            response.DatiIstanza.NoteDiInvio,
                                                            "NULL",
                                                            "N",
                                                            consolidaDocumenti,
                                                            tipoIstanza);

                    transactionContext.Complete();
                }

                response.Success = true;
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public Services.DeleteIstanza.DeleteIstanzaResponse DeleteIstanza(Services.DeleteIstanza.DeleteIstanzaRequest request)
        {
            Services.DeleteIstanza.DeleteIstanzaResponse response = new Services.DeleteIstanza.DeleteIstanzaResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                // 2. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                // 3. Verifica se l'istanza di conservazione è in stato "DaInviare"
                Services.GetIstanza.GetIstanzaResponse getIstanza = this.GetIstanza(new Services.GetIstanza.GetIstanzaRequest { UserName = request.UserName, AuthenticationToken = request.AuthenticationToken, IdIstanza = request.IdIstanza });

                if (getIstanza.Success)
                {
                    if (getIstanza.Istanza.Stato == Dominio.StatiIstanzaEnum.DaInviare)
                    {
                        // Possono essere rimosse solamente le istanze di conservazione in stato "DaInviare"
                        BusinessLogic.Documenti.areaConservazioneManager.DeleteIstanzaConservazione(infoUtente, request.IdIstanza);
                    }
                    else
                    {
                        // E' possibile rimuovere l'istanza solo se non è stata inviata a centro servizi
                        throw new PacchettiException(ErrorCodes.ISTANZA_NON_CANCELLABILE,
                            string.Format(ErrorDescriptions.ISTANZA_NON_CANCELLABILE, "istanza già inviata al Centro Servizi"));
                    }
                }
                else
                {
                    // Istanza di conservazione non trovata
                    throw new PacchettiException(ErrorCodes.ISTANZA_NON_TROVATA, ErrorDescriptions.ISTANZA_NON_TROVATA);
                }

                response.Success = true;
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public Services.GetIstanza.GetIstanzaResponse GetIstanza(Services.GetIstanza.GetIstanzaRequest request)
        {
            Services.GetIstanza.GetIstanzaResponse response = new Services.GetIstanza.GetIstanzaResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                // 2. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConservazione.DocsPaConsManager();

                DocsPaVO.areaConservazione.InfoConservazione[] list = consManager.RicercaInfoConservazione(string.Format("WHERE SYSTEM_ID = {0}", request.IdIstanza));

                if (list != null && list.Length == 1)
                {
                    response.Success = true;

                    response.Istanza = this.MapIstanzaConservazione(list[0]);
                }
                else
                {
                    // Istanza di conservazione non trovata
                    throw new PacchettiException(ErrorCodes.ISTANZA_NON_TROVATA, ErrorDescriptions.ISTANZA_NON_TROVATA);
                }

                response.Success = true;
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento dei dati dell'istanza di conservazione da inviare al Centro Servizi
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito    
        /// 
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        ///     
        /// - ISTANZA_NON_TROVATA
        ///     Istanza di conservazione inesistente
        ///     
        /// - ERRORE_NON_GESTITO: 
        ///     Si è verificato un errore non previsto tra quelli censiti
        /// </returns>
        [WebMethod()]
        public Services.GetIstanzaDaInviare.GetIstanzaDaInviareResponse GetIstanzaDaInviare(Services.GetIstanzaDaInviare.GetIstanzaDaInviareRequest request)
        {
            Services.GetIstanzaDaInviare.GetIstanzaDaInviareResponse response = new Services.GetIstanzaDaInviare.GetIstanzaDaInviareResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                // 2. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                Services.GetIstanze.GetIstanzeResponse getIstanzeResponse = this.GetIstanze(new Services.GetIstanze.GetIstanzeRequest
                {
                    UserName = request.UserName,
                    AuthenticationToken = request.AuthenticationToken,
                    GetDaInviare = true,
                    GetChiuse = false,
                    GetFirmate = false,
                    GetInLavorazione = false,
                    GetInviate = false,
                    GetRifiutate = false,
                    GetConservate = false,
                    GetErrore = false
                });

                if (getIstanzeResponse.Success)
                {
                    if (getIstanzeResponse.Istanze.Length == 0)
                    {
                        // Istanza di conservazione non trovata
                        throw new PacchettiException(ErrorCodes.ISTANZA_NON_TROVATA, ErrorDescriptions.ISTANZA_NON_TROVATA);
                    }

                    response.Istanza = getIstanzeResponse.Istanze[0];
                }
                else
                    throw new PacchettiException(getIstanzeResponse.Error.Code, getIstanzeResponse.Error.Description);

                response.Success = true;
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public Services.GetIstanze.GetIstanzeResponse GetIstanze(Services.GetIstanze.GetIstanzeRequest request)
        {
            Services.GetIstanze.GetIstanzeResponse response = new Services.GetIstanze.GetIstanzeResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                // 2. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConservazione.DocsPaConsManager();

                string filterString = string.Empty;

                List<string> filters = new List<string>();

                if (request.GetDaInviare) filters.Add("CHA_STATO = 'N'");
                if (request.GetInviate) filters.Add("CHA_STATO = 'I'");
                if (request.GetRifiutate) filters.Add("CHA_STATO = 'R'");
                if (request.GetInLavorazione) filters.Add("CHA_STATO = 'L'");
                if (request.GetFirmate) filters.Add("CHA_STATO = 'F'");
                if (request.GetChiuse) filters.Add("CHA_STATO = 'C'");
                if (request.GetConservate) filters.Add("CHA_STATO = 'V'");
                if (request.GetErrore) filters.Add("CHA_STATO = 'E'");

                // Filtro obbligatorio su idPeople e idGroup
                filterString = string.Format(" WHERE ID_PEOPLE = {0} AND ID_GRUPPO = {1}", infoUtente.idPeople, infoUtente.idGruppo);

                string additionalFilterString = string.Empty;

                foreach (string itm in filters)
                {
                    if (!string.IsNullOrEmpty(additionalFilterString))
                        additionalFilterString += " OR ";

                    additionalFilterString += itm;
                }

                if (!string.IsNullOrEmpty(additionalFilterString))
                    filterString += string.Format(" AND ({0})", additionalFilterString);

                DocsPaVO.areaConservazione.InfoConservazione[] list = consManager.RicercaInfoConservazione(filterString);

                if (list != null)
                {
                    List<Dominio.Istanza> istanze = new List<Dominio.Istanza>();

                    foreach (DocsPaVO.areaConservazione.InfoConservazione info in list)
                        istanze.Add(this.MapIstanzaConservazione(info));

                    response.Istanze = istanze.ToArray();
                }

                response.Success = true;
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public Services.AddDocumentoInIstanza.AddDocumentoInIstanzaResponse AddDocumentoInIstanza(Services.AddDocumentoInIstanza.AddDocumentoInIstanzaRequest request)
        {
            Services.AddDocumentoInIstanza.AddDocumentoInIstanzaResponse response = new Services.AddDocumentoInIstanza.AddDocumentoInIstanzaResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                if (string.IsNullOrEmpty(request.IdDocumento))
                    throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "IdDocumento"));

                // 2. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                // 3. Reperimento del documento
                DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, request.IdDocumento);

                if (schedaDocumento == null)
                    throw new PacchettiVersamento.PacchettiException(ErrorCodes.DOCUMENTO_NON_TROVATO, ErrorDescriptions.DOCUMENTO_NON_TROVATO);

                // 1. Inserimento del documento in conservazione
                string ret = BusinessLogic.Documenti.areaConservazioneManager.CreateAndAddDocInAreaConservazione
                                                (infoUtente,
                                                schedaDocumento.systemId,
                                                string.Empty,
                                                schedaDocumento.docNumber,
                                                "D",
                                                string.Empty);

                if (!string.IsNullOrEmpty(ret) && (Convert.ToInt32(ret) > 0))
                {
                    DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];

                    int size;
                    Int32.TryParse(fileRequest.fileSize, out size);

                    BusinessLogic.Documenti.areaConservazioneManager.updateItemsCons(System.IO.Path.GetExtension(fileRequest.fileName), "0", ret);
                    BusinessLogic.Documenti.areaConservazioneManager.updateSizeItemCons(ret, size);
                }
                else
                {
                    throw new PacchettiVersamento.PacchettiException(ErrorCodes.INSERIMENTO_IN_ISTANZA_FALLITO, ErrorDescriptions.INSERIMENTO_IN_ISTANZA_FALLITO);
                }

                response.Success = true;
                response.IdIstanza = ret;
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public Services.AddFascicoloInIstanza.AddFascicoloInIstanzaResponse AddFascicoloInIstanza(Services.AddFascicoloInIstanza.AddFascicoloInIstanzaRequest request)
        {
            Services.AddFascicoloInIstanza.AddFascicoloInIstanzaResponse response = new Services.AddFascicoloInIstanza.AddFascicoloInIstanzaResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                if (string.IsNullOrEmpty(request.IdFascicoloProcedimentale))
                    throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "IdFascicoloProcedimentale"));

                // 2. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                // 3. Reperimento dei documenti contenuti nel fascicolo
                List<DocsPaVO.ricerche.SearchResultInfo> result = BusinessLogic.Documenti.areaConservazioneManager.getListaDocumentiByIdProject(request.IdFascicoloProcedimentale);

                foreach (DocsPaVO.ricerche.SearchResultInfo doc in result)
                {
                    DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, doc.Id);

                    if (schedaDocumento == null)
                        throw new PacchettiVersamento.PacchettiException(ErrorCodes.DOCUMENTO_NON_TROVATO, ErrorDescriptions.DOCUMENTO_NON_TROVATO);

                    // 7. Inserimento del documento in conservazione
                    string ret = BusinessLogic.Documenti.areaConservazioneManager.CreateAndAddDocInAreaConservazione
                                                    (infoUtente,
                                                    schedaDocumento.systemId,
                                                    request.IdFascicoloProcedimentale,
                                                    schedaDocumento.docNumber,
                                                    "F",
                                                    string.Empty);

                    if (!string.IsNullOrEmpty(ret) && (Convert.ToInt32(ret) > 0))
                    {
                        DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];

                        int size;
                        Int32.TryParse(fileRequest.fileSize, out size);

                        BusinessLogic.Documenti.areaConservazioneManager.updateItemsCons(System.IO.Path.GetExtension(fileRequest.fileName), "0", ret);
                        BusinessLogic.Documenti.areaConservazioneManager.updateSizeItemCons(ret, size);
                    }
                    else
                    {
                        throw new PacchettiVersamento.PacchettiException(ErrorCodes.INSERIMENTO_IN_ISTANZA_FALLITO, ErrorDescriptions.INSERIMENTO_IN_ISTANZA_FALLITO);
                    }
                }

                response.Success = true;
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        ///// <summary>
        ///// Verifica se il documento è già stato inserito nell'istanza di conservazione
        ///// </summary>
        ///// <param name="idDocumento"></param>
        ///// <param name="idIstanza"></param>
        ///// <returns></returns>
        //protected bool IsDocumentoInAreaConservazione(string idDocumento, string idIstanza)
        //{
        //    bool retValue = false;

        //    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
        //    {
        //        string commandText = string.Format("SELECT COUNT(*) FROM DPA_ITEMS_CONSERVAZIONE WHERE DOCNUMBER = {0} AND ID_CONSERVAZIONE = {1}", idDocumento, idIstanza);

        //        string field;
        //        if (!dbProvider.ExecuteScalar(out field, commandText))
        //            throw new ApplicationException(dbProvider.LastExceptionMessage);

        //        int count;
        //        Int32.TryParse(field, out count);
        //        retValue = (count > 0);
        //    }

        //    return retValue;
        //}

        /// <summary>
        /// Mapping dell'oggetto Istanza di conservazione
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected virtual Dominio.Istanza MapIstanzaConservazione(DocsPaVO.areaConservazione.InfoConservazione info)
        {
            Dominio.Istanza istanza = new Dominio.Istanza();

            istanza.Id = info.SystemID;
            istanza.Descrizione = info.Descrizione;
            istanza.NoteDiInvio = info.Note;
            istanza.DataApertura = info.Data_Apertura;
            istanza.DataInvio = info.Data_Invio;
            istanza.DataChiusura = info.Data_Conservazione;

            // Mapping dello stato dell'istanza
            istanza.Stato = this.MapStatoIstanza(info.StatoConservazione);

            // Mapping del tipo di istanza
            istanza.Tipo = this.MapTipoIstanza(info.TipoConservazione);

            // Mapping del richiedente della conservazione
            istanza.Richiedente = string.Format("{0} ({1})", info.userID, info.IdGruppo);

            // TODO: AGGIUNGERE CAMPO
            // istanza.ModoCreazione = Dominio.ModiCreazioneIstanzaEnum.Manuale;

            return istanza;
        }

        /// <summary>
        /// Mapping del tipo di istanza
        /// </summary>
        /// <param name="codiceTipo"></param>
        /// <returns></returns>
        protected Dominio.TipiIstanzaEnum MapTipoIstanza(string codiceTipo)
        {
            if (codiceTipo == DocsPaConservazione.TipoIstanzaConservazione.CONSERVAZIONE_CONSOLIDATA)
                return Dominio.TipiIstanzaEnum.Consolidata;
            else if (codiceTipo == DocsPaConservazione.TipoIstanzaConservazione.CONSERVAZIONE_NON_CONSOLIDATA)
                return Dominio.TipiIstanzaEnum.NonConsolidata;
            else if (codiceTipo == DocsPaConservazione.TipoIstanzaConservazione.ESIBIZIONE)
                return Dominio.TipiIstanzaEnum.Esibizione;
            else if (codiceTipo == DocsPaConservazione.TipoIstanzaConservazione.CONSERVAZIONE_INTERNA)
                return Dominio.TipiIstanzaEnum.Interna;
            else
                return Dominio.TipiIstanzaEnum.NonConsolidata;
        }

        /// <summary>
        /// Mapping dello stato dell'istanza di conservazione
        /// </summary>
        /// <param name="codiceStato"></param>
        /// <returns></returns>
        protected Dominio.StatiIstanzaEnum MapStatoIstanza(string codiceStato)
        {
            if (codiceStato == "N")
                return Dominio.StatiIstanzaEnum.DaInviare;
            else if (codiceStato == "I")
                return Dominio.StatiIstanzaEnum.Inviata;
            else if (codiceStato == "R")
                return Dominio.StatiIstanzaEnum.Rifiutata;
            else if (codiceStato == "L")
                return Dominio.StatiIstanzaEnum.InLavorazione;
            else if (codiceStato == "F")
                return Dominio.StatiIstanzaEnum.Firmata;
            else if (codiceStato == "E")
                return Dominio.StatiIstanzaEnum.Errore;
            else if (codiceStato == "C")
                return Dominio.StatiIstanzaEnum.Chiusa;
            else if (codiceStato == "V")
                return Dominio.StatiIstanzaEnum.Conservata;
            else
                return Dominio.StatiIstanzaEnum.DaInviare;
        }

        /// <summary>
        /// Reperimento dei sottofascicoli dal path
        /// </summary>
        /// <param name="pathSottofascicoli"></param>
        /// <returns></returns>
        protected virtual string[] GetSottofascicoli(string pathSottofascicolo)
        {
            if (!string.IsNullOrEmpty(pathSottofascicolo))
                return pathSottofascicolo.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sottofascicoli"></param>
        /// <param name="parentFolder"></param>
        /// <param name="currentLevel"></param>
        /// <returns></returns>
        protected virtual DocsPaVO.fascicolazione.Folder GetOrCreateFolder(
                        DocsPaVO.utente.InfoUtente infoUtente,
                        string[] sottofascicoli,
                        DocsPaVO.fascicolazione.Folder parentFolder,
                        int currentLevel)
        {
            DocsPaVO.fascicolazione.Folder[] childs = (DocsPaVO.fascicolazione.Folder[])parentFolder.childs.ToArray(typeof(DocsPaVO.fascicolazione.Folder));

            DocsPaVO.fascicolazione.Folder retValue = childs.FirstOrDefault(e => e.descrizione == sottofascicoli[currentLevel]);

            if (retValue == null)
            {
                // Creazione sottofascicolo
                retValue = new DocsPaVO.fascicolazione.Folder
                {
                    descrizione = sottofascicoli[currentLevel],
                    idParent = parentFolder.systemID,
                    idFascicolo = parentFolder.idFascicolo
                };

                DocsPaVO.utente.Ruolo ruoloProprietario = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);

                DocsPaVO.fascicolazione.ResultCreazioneFolder result;
                retValue = BusinessLogic.Fascicoli.FolderManager.newFolder(retValue, infoUtente, ruoloProprietario, out result);

                if (result != DocsPaVO.fascicolazione.ResultCreazioneFolder.OK)
                {
                    throw new PacchettiException(ErrorCodes.CREAZIONE_SOTTOFASCICOLO_FALLITA,
                                    string.Format(ErrorDescriptions.CREAZIONE_SOTTOFASCICOLO_FALLITA, sottofascicoli[currentLevel]));
                }
            }

            if (currentLevel < (sottofascicoli.Length - 1))
            {
                retValue = this.GetOrCreateFolder(infoUtente, sottofascicoli, retValue, (currentLevel + 1));
            }

            return retValue;


            //if (retValue == null)
            //{
            //    if (sottofascicoli.Length == (currentLevel + 1))
            //    {

            //    }
            //    else
            //    {
            //        foreach (DocsPaVO.fascicolazione.Folder c in childs)
            //        {
            //            retValue = this.GetOrCreateFolder(infoUtente, sottofascicoli, c, currentLevel++);

            //            if (retValue != null)
            //                break;
            //        }
            //    }
            //}
            //else
            //{
            //    if ((currentLevel + 1) < sottofascicoli.Length)
            //    {
            //        retValue = this.GetOrCreateFolder(infoUtente, sottofascicoli, retValue, currentLevel++);
            //    }
            //}

            return retValue;
        }

        #endregion

        #region Servizi per la gestione dei documenti in un'istanza di conservazione

        /// <summary>
        /// Servizio per l'inserimento di un documento in un'istanza di conservazione
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public Services.InsertDocumento.InsertDocumentoResponse InsertDocumento(Services.InsertDocumento.InsertDocumentoRequest request)
        {
            Services.InsertDocumento.InsertDocumentoResponse response = new Services.InsertDocumento.InsertDocumentoResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                // 2. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                if (request.Documento == null)
                    throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "Documento"));

                if (request.Documento.File == null)
                    throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "File"));

                if (request.Documento.File.Nome == null)
                    throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "File.Nome"));

                if (request.Documento.File.Contenuto == null || (request.Documento.File.Contenuto != null && request.Documento.File.Contenuto.Length == 0))
                    throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "File.Contenuto"));

                if (request.Documento.File.TipoMime == null)
                    throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "File.TipoMime"));

                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    if (!string.IsNullOrEmpty(request.Documento.IdDocumentoPrincipale))
                    {
                        // Richiesta di inserimento di un allegato

                        DocsPaVO.documento.FileRequest allegato = new DocsPaVO.documento.Allegato
                        {
                            docNumber = request.Documento.IdDocumentoPrincipale,
                            descrizione = request.Documento.Oggetto
                        };

                        // Acquisizione dell'allegato
                        allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, ((DocsPaVO.documento.Allegato)allegato));

                        // Acquisizione file allegato
                        // 5. Acquisizione del file al documento
                        DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                        {
                            name = request.Documento.File.Nome,
                            fullName = request.Documento.File.Nome,
                            content = request.Documento.File.Contenuto,
                            contentType = request.Documento.File.TipoMime,
                            length = request.Documento.File.Contenuto.Length,
                            bypassFileContentValidation = true
                        };

                        if (request.ValidaContenutoFile)
                        {
                            // Controllo sulla validazione del contenuto
                            if (!BusinessLogic.Documenti.FileManager.IsValidFileContent(fileDocumento))
                                throw new PacchettiException(ErrorCodes.CONTENUTO_FILE_NON_CONFORME, ErrorDescriptions.CONTENUTO_FILE_NON_CONFORME);
                        }

                        string errorMessage;
                        if (!BusinessLogic.Documenti.FileManager.putFile(ref allegato, fileDocumento, infoUtente, out errorMessage))
                            throw new PacchettiException(ErrorCodes.UPLOAD_FILE_FALLITO, string.Format(ErrorDescriptions.UPLOAD_FILE_FALLITO, errorMessage));

                        response.Documento = request.Documento;
                        response.Documento.Id = allegato.docNumber;

                        ////Services.GetIstanze.GetIstanzeResponse getIstanzeResponse = this.GetIstanze(new Services.GetIstanze.GetIstanzeRequest
                        ////{
                        ////    UserName = request.UserName,
                        ////    AuthenticationToken = request.AuthenticationToken,
                        ////    GetDaInviare = true,
                        ////    GetChiuse = false,
                        ////    GetFirmate = false,
                        ////    GetInLavorazione = false,
                        ////    GetInviate = false,
                        ////    GetRifiutate = false
                        ////});

                        ////if (getIstanzeResponse.Success)
                        ////{

                        ////    // Aggiornamento del numero degli allegati nel doc in istanza di conservazione
                        ////    BusinessLogic.Documenti.areaConservazioneManager.updateItemsCons(System.IO.Path.GetExtension(request.Documento.File.Nome), "0", ret);
                        ////}
                    }
                    else
                    {
                        // 4. Creazione del documento grigio
                        DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(infoUtente, false);

                        // 4.1. Valorizzazione oggetto del documento
                        schedaDocumento.oggetto = new DocsPaVO.documento.Oggetto();
                        schedaDocumento.oggetto.descrizione = request.Documento.Oggetto;
                        schedaDocumento.oggetto.daAggiornare = true;

                        // 4.2. Reperimento del template del documento
                        if (request.Documento.Profilo != null)
                        {
                            DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(request.Documento.Profilo.Id);

                            if (template == null)
                                throw new PacchettiException(ErrorCodes.PROFILO_NON_TROVATO, ErrorDescriptions.PROFILO_NON_TROVATO);

                            DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                                                template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom));


                            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in oggettiCustom)
                            {
                                Dominio.CampoProfilo campo = request.Documento.Profilo.Campi.FirstOrDefault(e => e.Nome.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant());

                                if (campo == null)
                                    throw new PacchettiException(ErrorCodes.CAMPO_PROFILO_NON_TROVATO, string.Format(ErrorDescriptions.CAMPO_PROFILO_NON_TROVATO, campo.Nome));

                                // Impostazione del valore
                                if (string.IsNullOrEmpty(campo.Valore))
                                {
                                    if (campo.Obbligatorio)
                                        throw new PacchettiException(ErrorCodes.VALORE_CAMPO_PROFILO_OBBLIGATORIO,
                                                        string.Format(ErrorDescriptions.VALORE_CAMPO_PROFILO_OBBLIGATORIO, campo.Nome));
                                    else
                                        oggettoCustom.VALORE_DATABASE = string.Empty;

                                    if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore") || (oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore"))
                                    {
                                        oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                                    }
                                }
                                else
                                {
                                    oggettoCustom.VALORE_DATABASE = campo.Valore;
                                    //Passo come valore il codice del Registro rf del contatore
                                    string idAOORF = string.Empty;
                                    Registro reg = null;

                                    if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore") || (oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore"))
                                    {
                                        //Aggiunta per contatore
                                        if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                                        {
                                            reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCodAOO(campo.Valore, infoUtente.idAmministrazione);
                                            if (reg != null)
                                            {
                                                oggettoCustom.ID_AOO_RF = reg.systemId;
                                            }

                                        }

                                        oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                                    }


                                    if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                                    {

                                        string[] words = (campo.Valore).Split('|');

                                        for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Count; i++)
                                        {
                                            foreach (string word in words)
                                            {
                                                if (((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE.Equals(word))
                                                {
                                                    oggettoCustom.VALORI_SELEZIONATI[i] = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            schedaDocumento.template = template;
                            schedaDocumento.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto
                            {
                                systemId = template.SYSTEM_ID.ToString(),
                                descrizione = template.DESCRIZIONE
                            };
                        }

                        DocsPaVO.utente.Ruolo ruoloProprietario = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);

                        try
                        {
                            schedaDocumento = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDocumento, infoUtente, ruoloProprietario);

                            response.Documento = new Dominio.Documento
                            {
                                Id = schedaDocumento.systemId,
                                Oggetto = request.Documento.Oggetto
                                // NB. Il file non sarà restituito
                            };
                        }
                        catch (Exception ex)
                        {
                            // Errore nella creazione del documento
                            throw new PacchettiException(ErrorCodes.CREAZIONE_DOCUMENTO_FALLITA, string.Format(ErrorDescriptions.CREAZIONE_DOCUMENTO_FALLITA, ex.Message));
                        }

                        // 5. Acquisizione del file al documento
                        DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];

                        DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                        {
                            name = request.Documento.File.Nome,
                            fullName = request.Documento.File.Nome,
                            content = request.Documento.File.Contenuto,
                            contentType = request.Documento.File.TipoMime,
                            length = request.Documento.File.Contenuto.Length,
                            bypassFileContentValidation = true
                        };

                        if (request.ValidaContenutoFile)
                        {
                            // Controllo sulla validazione del contenuto
                            if (!BusinessLogic.Documenti.FileManager.IsValidFileContent(fileDocumento))
                                throw new PacchettiException(ErrorCodes.CONTENUTO_FILE_NON_CONFORME, ErrorDescriptions.CONTENUTO_FILE_NON_CONFORME);
                        }

                        string errorMessage;
                        if (!BusinessLogic.Documenti.FileManager.putFile(ref versioneCorrente, fileDocumento, infoUtente, out errorMessage))
                            throw new PacchettiException(ErrorCodes.UPLOAD_FILE_FALLITO, string.Format(ErrorDescriptions.UPLOAD_FILE_FALLITO, errorMessage));

                        // 6. Inserimento del documento nel fascicolo
                        if (!string.IsNullOrEmpty(request.Documento.IdFascicolo))
                        {
                            // Reperimento id del folder
                            string idFolder = this.GetIdFolderCType(request.Documento.IdFascicolo);

                            // Reperimento del folder in cui inserire il fascicolo
                            DocsPaVO.fascicolazione.Folder folder = BusinessLogic.Fascicoli.FolderManager.getFolderById(infoUtente.idPeople, infoUtente.idGruppo, idFolder);

                            string msg;
                            if (!BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente,
                                                                                    schedaDocumento.systemId,
                                                                                    folder.systemID,
                                                                                    false,
                                                                                    out msg))
                            {
                                throw new PacchettiException(ErrorCodes.FASCICOLAZIONE_FALLITA,
                                                            ErrorDescriptions.FASCICOLAZIONE_FALLITA);
                            }

                            response.Documento.IdFascicolo = request.Documento.IdFascicolo;
                            response.Documento.CodiceFascicolo = folder.descrizione;
                        }

                        if (request.AddInIstanzaConservazione)
                        {
                            // 7. Inserimento del documento in conservazione
                            string ret = BusinessLogic.Documenti.areaConservazioneManager.CreateAndAddDocInAreaConservazione
                                                            (infoUtente,
                                                            schedaDocumento.systemId,
                                                            request.Documento.IdFascicolo,
                                                            schedaDocumento.docNumber,
                                                            "D",
                                                            string.Empty);

                            if (!string.IsNullOrEmpty(ret) && (Convert.ToInt32(ret) > 0))
                            {
                                BusinessLogic.Documenti.areaConservazioneManager.updateItemsCons(System.IO.Path.GetExtension(request.Documento.File.Nome), "0", ret);
                                BusinessLogic.Documenti.areaConservazioneManager.updateSizeItemCons(ret, fileDocumento.length);
                            }
                            else
                            {
                                throw new PacchettiVersamento.PacchettiException(ErrorCodes.INSERIMENTO_IN_ISTANZA_FALLITO, ErrorDescriptions.INSERIMENTO_IN_ISTANZA_FALLITO);
                            }
                        }

                        response.Documento.Profilo = request.Documento.Profilo;
                        response.Documento.File = request.Documento.File;

                        // Il contenuto del file viene rimosso per evitare traffico di rete
                        response.Documento.File.Contenuto = null;
                    }

                    response.Success = true;


                    transactionContext.Complete();
                }
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Documento = null;
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Documento = null;
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        ///// <summary>
        ///// Servizio per l'upload di un file ad un documento in un'istanza di conservazione
        ///// </summary>
        ///// <param name="request">
        ///// Dati della richiesta
        ///// </param>
        ///// <returns>
        ///// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        ///// - PARAMETRO_MANCANTE
        /////     Parametro obbligatorio non fornito
        ///// 
        ///// - DOCUMENTO_NON_TROVATO
        /////     Documento non trovato
        /////     
        ///// - UPLOAD_FILE
        /////     Errore nell'upload del file
        ///// 
        ///// - CONTENUTO_FILE_NON_CONFORME
        /////     Errore relativo al controllo di validazione effettuato sul contenuto del file rispetto al suo formato
        ///// 
        ///// - TOKEN_NON_VALIDO
        /////     Il token di autenticazione fornito non è valido o scaduto
        ///// </returns>
        //[WebMethod()]
        //public Services.UploadFile.UploadFileResponse UploadFile(Services.UploadFile.UploadFileRequest request)
        //{
        //    Services.UploadFile.UploadFileResponse response = new Services.UploadFile.UploadFileResponse();

        //    try
        //    {
        //        // 1. Validazione token di autenticazione
        //        this.CheckAuthenticationToken(request);

        //        // 2. Ripristino contesto utente
        //        DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

        //        // 3. Reperimento scheda documento
        //        DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, request.IdDocumento);

        //        if (schedaDocumento == null)
        //            throw new PacchettiException(ErrorCodes.DOCUMENTO_NON_TROVATO, ErrorDescriptions.DOCUMENTO_NON_TROVATO);

        //        DocsPaVO.documento.FileRequest version = (DocsPaVO.documento.FileRequest) schedaDocumento.documenti[0];

        //        bool acquired = false;

        //        int size;
        //        if (Int32.TryParse(version.fileSize, out size))
        //            acquired = (size > 0);

        //        if (acquired)
        //        {
        //            // Per la versione selezionata, il file è già acquisito, pertanto il file sarà acquisito su una nuova versione
        //            version = new DocsPaVO.documento.Documento();
        //            version.docNumber = schedaDocumento.docNumber;

        //            // Creazione nuova versione e acquisizione
        //            version = BusinessLogic.Documenti.VersioniManager.addVersion(version, infoUtente, false);
        //        }

        //        // Acquisizione file su ultima verisone
        //        DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
        //        {
        //            name = request.Nome,
        //            fullName = request.Nome,
        //            content = request.Contenuto,
        //            contentType = request.TipoMime,
        //            length = request.Contenuto.Length,
        //            bypassFileContentValidation = true
        //        };

        //        if (request.ValidaContenutoFile)
        //        {
        //            // Controllo sulla validazione del contenuto
        //            if (!BusinessLogic.Documenti.FileManager.IsValidFileContent(fileDocumento))
        //                throw new PacchettiException(ErrorCodes.CONTENUTO_FILE_NON_CONFORME, ErrorDescriptions.CONTENUTO_FILE_NON_CONFORME);
        //        }

        //        string errorMessage;
        //        if (!BusinessLogic.Documenti.FileManager.putFile(ref version, fileDocumento, infoUtente, out errorMessage))
        //            throw new PacchettiException(ErrorCodes.UPLOAD_FILE_FALLITO, string.Format(ErrorDescriptions.UPLOAD_FILE_FALLITO, errorMessage));

        //        response.Success = true;
        //    }
        //    catch (PacchettiException pacchettiEx)
        //    {
        //        response.Error = new Services.ResponseError
        //        {
        //            Code = pacchettiEx.ErrorCode,
        //            Description = pacchettiEx.Message
        //        };

        //        response.Success = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Error = new Services.ResponseError
        //        {
        //            Code = ErrorCodes.ERRORE_NON_GESTITO,
        //            Description = ex.Message
        //        };

        //        response.Success = false;
        //    }

        //    return response;
        //}

        /// <summary>
        /// Servizio per il reperimento di un documento in un'istanza di conservazione
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public Services.GetDocumento.GetDocumentoResponse GetDocumento(Services.GetDocumento.GetDocumentoRequest request)
        {
            Services.GetDocumento.GetDocumentoResponse response = new Services.GetDocumento.GetDocumentoResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                // 2. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                // 3. Reperimento del documento
                DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, request.IdDocumento);

                if (schedaDocumento == null)
                    throw new PacchettiVersamento.PacchettiException(ErrorCodes.DOCUMENTO_NON_TROVATO, ErrorDescriptions.DOCUMENTO_NON_TROVATO);

                DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConservazione.DocsPaConsManager();

                DocsPaVO.areaConservazione.ItemsConservazione[] itemsConservazione = consManager.getItemsConservazioneByIdLite(request.IdIstanza, infoUtente);

                if (itemsConservazione != null)
                {
                    DocsPaVO.areaConservazione.ItemsConservazione itemConservazione = itemsConservazione.FirstOrDefault(e => e.ID_Profile.Trim().ToLowerInvariant() == request.IdDocumento.Trim().ToLowerInvariant());

                    if (itemConservazione == null)
                        throw new PacchettiException(ErrorCodes.DOCUMENTO_NON_TROVATO_IN_ISTANZA, ErrorCodes.DOCUMENTO_NON_TROVATO_IN_ISTANZA);

                    response.Documento = new Dominio.Documento
                    {
                        Id = schedaDocumento.systemId,
                        Oggetto = schedaDocumento.oggetto.descrizione,
                        IdFascicolo = itemConservazione.ID_Project,
                        CodiceFascicolo = itemConservazione.CodFasc,
                        IdDocumentoPrincipale = string.Empty,
                        Profilo = null,
                        File = null
                    };

                    // 4. Reperimento dati del profilo
                    if (schedaDocumento.template != null)
                    {
                        List<Dominio.CampoProfilo> campi = new List<Dominio.CampoProfilo>();

                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom o in schedaDocumento.template.ELENCO_OGGETTI)
                        {
                            campi.Add(new Dominio.CampoProfilo { Nome = o.DESCRIZIONE, Obbligatorio = (o.CAMPO_OBBLIGATORIO == "SI"), Valore = o.VALORE_DATABASE });
                        }

                        response.Documento.Profilo = new Dominio.DettaglioProfilo
                        {
                            Id = schedaDocumento.template.SYSTEM_ID.ToString(),
                            Nome = schedaDocumento.template.DESCRIZIONE,
                            Campi = campi.ToArray()
                        };
                    }

                    // 5. Reperimento del file associato al documento
                    if (request.GetFile)
                    {
                        DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];

                        DocsPaVO.documento.FileDocumento fileDocumento = null;

                        try
                        {
                            fileDocumento = BusinessLogic.Documenti.FileManager.getFile(versioneCorrente, infoUtente);
                        }
                        catch
                        {
                            fileDocumento = null;
                        }

                        if (fileDocumento == null)
                            throw new PacchettiVersamento.PacchettiException(ErrorCodes.DOWNLOAD_FILE_FALLITO, ErrorDescriptions.DOWNLOAD_FILE_FALLITO);

                        response.Documento.File = new Dominio.File
                        {
                            Nome = fileDocumento.name,
                            Contenuto = fileDocumento.content,
                            TipoMime = fileDocumento.contentType
                        };
                    }
                }
                else
                {
                    throw new PacchettiException(ErrorCodes.DOCUMENTO_NON_TROVATO_IN_ISTANZA, ErrorCodes.DOCUMENTO_NON_TROVATO_IN_ISTANZA);
                }

                response.Success = true;
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento dei documenti presenti in un'istanza di conservazione
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns></returns>
        [WebMethod()]
        public Services.GetDocumenti.GetDocumentiResponse GetDocumenti(Services.GetDocumenti.GetDocumentiRequest request)
        {
            Services.GetDocumenti.GetDocumentiResponse response = new Services.GetDocumenti.GetDocumentiResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                // 2. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                // 3. Verifica esistenza istanza di conservazione
                Services.GetIstanza.GetIstanzaResponse getIstanzaResponse = GetIstanza(new Services.GetIstanza.GetIstanzaRequest { UserName = request.UserName, AuthenticationToken = request.AuthenticationToken, IdIstanza = request.IdIstanza });

                if (getIstanzaResponse.Success)
                {
                    // 4. Reperimento dei documenti nell'istanza di conservazione
                    DocsPaConservazione.DocsPaConsManager consMng = new DocsPaConservazione.DocsPaConsManager();

                    List<Dominio.Documento> list = new List<Dominio.Documento>();

                    foreach (DocsPaVO.areaConservazione.ItemsConservazione itm in consMng.getItemsConservazioneById(request.IdIstanza, infoUtente))
                    {
                        File file = new File();
                        DettaglioProfilo profile = new DettaglioProfilo();
                        file.Nome = itm.immagineAcquisita;
                        if (itm.template != null)
                        {
                            profile.Id = itm.template.ID_TIPO_ATTO;
                            profile.Nome = itm.template.DESCRIZIONE;
                        }
                        list.Add(new Dominio.Documento
                        {
                            Id = itm.ID_Profile,
                            Oggetto = itm.desc_oggetto,
                            IdFascicolo = itm.ID_Project,
                            CodiceFascicolo = itm.CodFasc,
                            IdDocumentoPrincipale = string.Empty,
                            File = file,
                            Profilo = profile
                        });
                    }

                    response.Success = true;
                    response.Documenti = list.ToArray();
                }
                else
                {
                    response.Success = false;
                    response.Error = getIstanzaResponse.Error;
                }
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public Services.DeleteDocumento.DeleteDocumentoResponse DeleteDocumento(Services.DeleteDocumento.DeleteDocumentoRequest request)
        {
            Services.DeleteDocumento.DeleteDocumentoResponse response = new Services.DeleteDocumento.DeleteDocumentoResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                // 2. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                // 3. Verifica esistenza istanza di conservazione
                Services.GetIstanza.GetIstanzaResponse getIstanzaResponse = GetIstanza(new Services.GetIstanza.GetIstanzaRequest { UserName = request.UserName, AuthenticationToken = request.AuthenticationToken, IdIstanza = request.IdIstanza });

                if (getIstanzaResponse.Success)
                {
                    if (getIstanzaResponse.Istanza.Stato == Dominio.StatiIstanzaEnum.DaInviare)
                    {
                        try
                        {
                            // 4. Rimozione documento dall'istanza
                            BusinessLogic.Documenti.areaConservazioneManager.DeleteDocumentoFromItemCons(request.IdIstanza, request.IdDocumento);

                            response.Success = true;
                        }
                        catch (Exception ex)
                        {
                            throw new PacchettiVersamento.PacchettiException(ErrorCodes.ERRORE_NON_GESTITO, ex.Message);
                        }
                    }
                    else
                        throw new PacchettiVersamento.PacchettiException(ErrorCodes.DOCUMENTO_NON_CANCELLABILE,
                                                    string.Format(ErrorDescriptions.DOCUMENTO_NON_CANCELLABILE, "istanza già inviata al Centro Servizi"));
                }
                else
                {
                    response.Success = false;
                    response.Error = getIstanzaResponse.Error;
                }
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        #endregion

        #region Servizi per la gestione della profilazione

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public Services.GetProfili.GetProfiliResponse GetProfili(Services.GetProfili.GetProfiliRequest request)
        {
            Services.GetProfili.GetProfiliResponse response = new Services.GetProfili.GetProfiliResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                // 2. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                List<Dominio.Profilo> listaProfili = new List<Dominio.Profilo>();

                DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] listaTemplate = null;

                if (request.TipoProfilo == Dominio.TipiOggettoProfiloEnum.Documento)
                {
                    // Reperimento delle tipologie documento
                    listaTemplate = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getListTemplatesLite(infoUtente.idAmministrazione);
                }
                else if (request.TipoProfilo == Dominio.TipiOggettoProfiloEnum.Fascicolo)
                {
                    // Reperimento delle tipologie fascicolo
                    listaTemplate = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getListTemplatesLite(infoUtente.idAmministrazione);
                }

                if (listaTemplate != null)
                {
                    foreach (DocsPaVO.ProfilazioneDinamicaLite.TemplateLite t in listaTemplate)
                    {
                        if (t.name != "CAMPI_COMUNI")
                            listaProfili.Add(new Dominio.Profilo { Id = t.system_id, Nome = t.name });
                    }
                }

                response.Success = true;
                response.Profili = listaProfili.ToArray();
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento dei dati di un profilo
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito
        ///     
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        ///     
        /// - ERRORE_NON_GESTITO
        ///     Si è verificato un errore non previsto tra quelli censiti        
        /// </returns>
        [WebMethod()]
        public Services.GetProfilo.GetProfiloResponse GetProfilo(Services.GetProfilo.GetProfiloRequest request)
        {
            Services.GetProfilo.GetProfiloResponse response = new Services.GetProfilo.GetProfiloResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                // 2. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                DocsPaVO.ProfilazioneDinamica.Templates template = null;

                if (request.TipoProfilo == Dominio.TipiOggettoProfiloEnum.Documento)
                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(request.IdProfilo);
                else if (request.TipoProfilo == Dominio.TipiOggettoProfiloEnum.Fascicolo)
                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(request.IdProfilo);

                if (template == null)
                    throw new PacchettiException(ErrorCodes.PROFILO_NON_TROVATO, ErrorDescriptions.PROFILO_NON_TROVATO);

                response.Profilo = new Dominio.DettaglioProfilo();
                response.Profilo.Id = template.SYSTEM_ID.ToString();
                response.Profilo.Nome = template.DESCRIZIONE;

                List<Dominio.CampoProfilo> campi = new List<Dominio.CampoProfilo>();

                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom o in template.ELENCO_OGGETTI)
                {
                    campi.Add(new Dominio.CampoProfilo { Nome = o.DESCRIZIONE, Obbligatorio = (o.CAMPO_OBBLIGATORIO == "SI") });
                }

                response.Profilo.Campi = campi.ToArray();
                response.Success = true;
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        #endregion

        #region Servizi per la gestione dei fascicoli

        /// <summary>
        /// Servizio di creazione di un fascicolo
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito
        ///     
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        ///     
        /// - MULTIPLO VALORE CASELLA DI SELEZIONE CAMPO PROFILATO
        ///     Nel caso di più attributi delle caselle di selezione dei campi profilati vanno inseriti con un separatore "|" nella forma attributo1|attributo2
        /// </returns>
        [WebMethod()]
        public Services.CreateFascicolo.CreateFascicoloResponse CreateFascicolo(Services.CreateFascicolo.CreateFascicoloRequest request)
        {

            Services.CreateFascicolo.CreateFascicoloResponse response = new Services.CreateFascicolo.CreateFascicoloResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                // 2. Verifica presenza del nodo di titolario
                if (string.IsNullOrEmpty(request.CodiceNodoTitolario))
                    throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "CodiceNodoTitolario"));

                if (request.Fascicolo == null)
                    throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "Fascicolo"));

                if (request.Fascicolo.Descrizione == null)
                    throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "Fascicolo.Descrizione"));

                // 3. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    // 4. Reperimento del nodo di titolario
                    DocsPaVO.fascicolazione.Classificazione classificazione = this.GetClassificazione(infoUtente, request.CodiceNodoTitolario);

                    classificazione.registro = BusinessLogic.Utenti.RegistriManager.getRegistro(classificazione.idRegistroNodoTit);

                    // La data di apertura è impostata automaticamente dal sistema
                    DateTime dataApertura = DateTime.Now;

                    // Reperimento del ruolo proprietario del fascicolo
                    DocsPaVO.utente.Ruolo ruoloProprietario = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);

                    // 5. Reperimento del template del fascicolo
                    DocsPaVO.ProfilazioneDinamica.Templates template = null;

                    if (request.Fascicolo.Profilo != null)
                    {
                        template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(request.Fascicolo.Profilo.Id);

                        if (template == null)
                            throw new PacchettiException(ErrorCodes.PROFILO_NON_TROVATO, ErrorDescriptions.PROFILO_NON_TROVATO);

                        DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                                            template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom));

                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in oggettiCustom)
                        {
                            Dominio.CampoProfilo campo = request.Fascicolo.Profilo.Campi.FirstOrDefault(e => e.Nome.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant());

                            if (campo == null)
                                throw new PacchettiException(ErrorCodes.CAMPO_PROFILO_NON_TROVATO, string.Format(ErrorDescriptions.CAMPO_PROFILO_NON_TROVATO, campo.Nome));

                            // Impostazione del valore
                            if (string.IsNullOrEmpty(campo.Valore))
                            {
                                if (campo.Obbligatorio)
                                    throw new PacchettiException(ErrorCodes.VALORE_CAMPO_PROFILO_OBBLIGATORIO,
                                                    string.Format(ErrorDescriptions.VALORE_CAMPO_PROFILO_OBBLIGATORIO, campo.Nome));
                                else
                                    oggettoCustom.VALORE_DATABASE = string.Empty;

                                if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore") || (oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore"))
                                {
                                    oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                                }
                            }
                            else
                            {
                                oggettoCustom.VALORE_DATABASE = campo.Valore;
                                //Passo come valore il codice del Registro rf del contatore
                                string idAOORF = string.Empty;
                                Registro reg = null;

                                if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore") || (oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore"))
                                {
                                    //Aggiunta per contatore
                                    if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                                    {
                                        reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCodAOO(campo.Valore, infoUtente.idAmministrazione);
                                        if (reg != null)
                                        {
                                            oggettoCustom.ID_AOO_RF = reg.systemId;
                                        }

                                    }

                                    oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                                }


                                if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                                {

                                    string[] words = (campo.Valore).Split('|');

                                    for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Count; i++)
                                    {
                                        foreach (string word in words)
                                        {
                                            if (((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE.Equals(word))
                                            {
                                                oggettoCustom.VALORI_SELEZIONATI[i] = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo
                    {
                        descrizione = request.Fascicolo.Descrizione,
                        apertura = dataApertura.ToString("dd/MM/yyyy"),
                        codiceGerarchia = request.CodiceNodoTitolario,
                        template = template,
                        tipo = (template == null ? string.Empty : template.SYSTEM_ID.ToString()),
                        codUltimo = BusinessLogic.Fascicoli.FascicoloManager.getFascNumRif(classificazione.systemID, classificazione.idRegistroNodoTit),
                        idRegistro = classificazione.idRegistroNodoTit
                    };

                    DocsPaVO.fascicolazione.ResultCreazioneFascicolo result;
                    fascicolo = BusinessLogic.Fascicoli.FascicoloManager.newFascicolo(
                                    classificazione,
                                    fascicolo,
                                    infoUtente,
                                    ruoloProprietario,
                                    false,
                                    out result);

                    if (result != DocsPaVO.fascicolazione.ResultCreazioneFascicolo.OK)
                        throw new PacchettiException(ErrorCodes.CREAZIONE_FASCICOLO_FALLITA, ErrorDescriptions.CREAZIONE_FASCICOLO_FALLITA);

                    response.Success = true;
                    response.Fascicolo = request.Fascicolo;
                    response.Fascicolo.Id = fascicolo.systemID;
                    response.Fascicolo.Codice = fascicolo.codice;

                    transactionContext.Complete();
                }

                response.Success = true;
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public Services.ClassificaDocumento.ClassificaDocumentoResponse ClassificaDocumento(Services.ClassificaDocumento.ClassificaDocumentoRequest request)
        {
            Services.ClassificaDocumento.ClassificaDocumentoResponse response = new Services.ClassificaDocumento.ClassificaDocumentoResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                // 2. Verifica presenza del nodo di titolario
                if (string.IsNullOrEmpty(request.CodiceNodoTitolario))
                    throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "CodiceNodoTitolario"));

                // 3. Verifica presenza id documento
                if (string.IsNullOrEmpty(request.IdDocumento))
                    throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "IdDocumento"));

                // 3. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                //using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                //{
                    DocsPaVO.fascicolazione.Classificazione classificazione = this.GetClassificazione(infoUtente, request.CodiceNodoTitolario);

                    // Reperimento id del folder
                    string idFolder = this.GetIdFolderCTypeGenerale(classificazione.systemID);

                    // Reperimento del folder in cui inserire il fascicolo
                    DocsPaVO.fascicolazione.Folder folder = BusinessLogic.Fascicoli.FolderManager.getFolderByIdNoSecurity(infoUtente.idPeople, infoUtente.idGruppo, idFolder);

                    string msg;
                    if (!BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente,
                                                                            request.IdDocumento,
                                                                            folder.systemID,
                                                                            false,
                                                                            out msg))
                    {
                        throw new PacchettiException(ErrorCodes.CLASSIFICAZIONE_FALLITA, ErrorDescriptions.CLASSIFICAZIONE_FALLITA);
                    }
                //}

                response.Success = true;
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// Servizio per la fascicolazione di un documento in un fascicolo procedimentale
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito
        ///     
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        /// 
        /// - FASCICOLO_NON_TROVATO
        ///     Nodo di titolario non trovato
        ///     
        /// - FASCICOLAZIONE_FALLITA
        ///     Errore nella classificazione del documento
        ///     
        /// </returns>
        [WebMethod()]
        public Services.FascicolaDocumento.FascicolaDocumentoResponse FascicolaDocumento(Services.FascicolaDocumento.FascicolaDocumentoRequest request)
        {
            Services.FascicolaDocumento.FascicolaDocumentoResponse response = new Services.FascicolaDocumento.FascicolaDocumentoResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                // 2. Verifica presenza del nodo di titolario
                if (string.IsNullOrEmpty(request.IdFascicoloProcedimentale))
                    throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "IdFascicoloProcedimentale"));

                // 3. Verifica presenza id documento
                if (string.IsNullOrEmpty(request.IdDocumento))
                    throw new PacchettiException(ErrorCodes.PARAMETRO_MANCANTE, string.Format(ErrorDescriptions.PARAMETRO_MANCANTE, "IdDocumento"));

                // 3. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                //using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                //{
                    // Reperimento degli eventuali sottofascicoli
                    string[] sottofascicoli = this.GetSottofascicoli(request.PathSottofascicolo);

                    DocsPaVO.fascicolazione.Folder folder = null;

                    if (sottofascicoli != null)
                    {
                        DocsPaVO.fascicolazione.Folder rootFolder = BusinessLogic.Fascicoli.FolderManager.getFolder(infoUtente.idPeople, infoUtente.idGruppo, request.IdFascicoloProcedimentale);

                        if (rootFolder == null)
                            throw new PacchettiException(ErrorCodes.FASCICOLO_NON_TROVATO, ErrorDescriptions.FASCICOLO_NON_TROVATO);

                        folder = this.GetOrCreateFolder(infoUtente, sottofascicoli, rootFolder, 0);
                    }
                    else
                    {
                        // Reperimento id del folder
                        string idFolder = this.GetIdFolderCType(request.IdFascicoloProcedimentale);

                        if (string.IsNullOrEmpty(idFolder))
                            throw new PacchettiException(ErrorCodes.FASCICOLO_NON_TROVATO, ErrorDescriptions.FASCICOLO_NON_TROVATO);

                        folder = new DocsPaVO.fascicolazione.Folder();

                        folder.systemID = idFolder;
                        //// Reperimento del folder in cui inserire il fascicolo
                        //folder = BusinessLogic.Fascicoli.FolderManager.getFolderById(infoUtente.idPeople, infoUtente.idGruppo, idFolder);
                    }

                    string msg;
                    if (!BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente,
                                                                            request.IdDocumento,
                                                                            folder.systemID,
                                                                            false,
                                                                            out msg))
                    {
                        throw new PacchettiException(ErrorCodes.FASCICOLAZIONE_FALLITA, ErrorDescriptions.FASCICOLAZIONE_FALLITA);
                    }
                //}

                response.Success = true;
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// Reperimento oggetto classificazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codiceClassifica"></param>
        /// <returns></returns>
        protected DocsPaVO.fascicolazione.Classificazione GetClassificazione(DocsPaVO.utente.InfoUtente infoUtente, string codiceClassifica)
        {
            System.Collections.ArrayList listClassificazioni = BusinessLogic.Fascicoli.TitolarioManager.getTitolario(
            infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, null, codiceClassifica, false);

            if (listClassificazioni != null && listClassificazioni.Count > 0)
                return (DocsPaVO.fascicolazione.Classificazione)listClassificazioni[0];
            else
                throw new ApplicationException(string.Format(ErrorCodes.NODO_TITOLARIO_NON_TROVATO, ErrorDescriptions.NODO_TITOLARIO_NON_TROVATO));
        }

        /// <summary>
        /// Reperimento ID del record di tipo "C" per il fascicolo o per il nodo di titolario (F o T)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idForT"></param>
        /// <returns></returns>
        protected string GetIdFolderCType(string idForT)
        {
            string recordC = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                // Query per stabilire la tipologia del record fornito in ingresso
                string commandText = string.Format("select cha_tipo_proj from project where system_id = {0}", idForT);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                {
                    if (field.Equals("C"))
                    {
                        recordC = idForT;
                    }
                    else
                    {
                        if (field.Equals("T"))
                        {
                            // Il record è di tipo "T", reperimento record di tipo "C" 
                            commandText = string.Format("select system_id from project where id_parent in (select system_id from project where id_parent = {0} and cha_tipo_proj = 'F')", idForT);
                        }
                        else if (field.Equals("F"))
                        {
                            // Il record è di tipo "F", reperimento record di tipo "C" 
                            commandText = string.Format("select system_id from project where id_parent = {0}", idForT);
                        }

                        if (dbProvider.ExecuteScalar(out field, commandText))
                        {
                            recordC = field;
                        }
                    }
                }
            }

            return recordC;
        }

        /// <summary>
        /// Reperimento ID del record di tipo "C" per il fascicolo o per il nodo di titolario (F o T)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idForT"></param>
        /// <returns></returns>
        protected string GetIdFolderCTypeGenerale(string idForT)
        {
            string recordC = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                // Query per stabilire la tipologia del record fornito in ingresso
                string commandText = string.Format("select cha_tipo_proj from project where system_id = {0}", idForT);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                {
                    if (field.Equals("C"))
                    {
                        recordC = idForT;
                    }
                    else
                    {
                        if (field.Equals("T"))
                        {
                            // Il record è di tipo "T", reperimento record di tipo "C" 
                            commandText = string.Format("select system_id from project where id_parent in (select system_id from project where id_parent = {0} and cha_tipo_proj = 'F' and cha_tipo_fascicolo = 'G')", idForT);
                        }
                        else if (field.Equals("F"))
                        {
                            // Il record è di tipo "F", reperimento record di tipo "C" 
                            commandText = string.Format("select system_id from project where id_parent = {0}", idForT);
                        }

                        if (dbProvider.ExecuteScalar(out field, commandText))
                        {
                            recordC = field;
                        }
                    }
                }
            }

            return recordC;
        }

        #endregion

        #region Servizi per l'invio a Centro Servizi e gestione dei supporti

        /// <summary>
        /// Servizio per l'invio del documento al Centro Servizi
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito
        ///     
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        ///     
        /// 
        /// </returns>
        [WebMethod()]
        public Services.InviaCentroServizi.InviaCentroServiziResponse InviaCentroServizi(Services.InviaCentroServizi.InviaCentroServiziRequest request)
        {
            Services.InviaCentroServizi.InviaCentroServiziResponse response = new Services.InviaCentroServizi.InviaCentroServiziResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                bool consolidata = false;

                // 2. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                // 3. Reperimento istanza di conservazione 
                Services.GetIstanze.GetIstanzeResponse getIstanzeResponse = GetIstanze(new Services.GetIstanze.GetIstanzeRequest
                {
                    UserName = request.UserName,
                    AuthenticationToken = request.AuthenticationToken,
                    GetDaInviare = true,
                    GetInLavorazione = false,
                    GetRifiutate = false,
                    GetFirmate = false,
                    GetChiuse = false,
                    GetConservate = false,
                    GetErrore = false,
                    GetInviate = false

                });

                if (getIstanzeResponse.Success)
                {
                    if (getIstanzeResponse.Istanze.Length == 0)
                        throw new PacchettiException(ErrorCodes.ISTANZA_NON_TROVATA, ErrorDescriptions.ISTANZA_NON_TROVATA);

                    Dominio.Istanza istanza = getIstanzeResponse.Istanze[0];

                    // 4. Invio istanza a Centro Servizi
                    string descrizione = request.Descrizione.Replace("'", "''");
                    string note = request.NoteDiInvio.Replace("'", "''");
                    string tipo_cons = string.Empty;

                    if (request.Tipo == Dominio.TipiIstanzaEnum.Consolidata)
                    {
                        tipo_cons = DocsPaConservazione.TipoIstanzaConservazione.CONSERVAZIONE_CONSOLIDATA;
                        consolidata = true;
                    }
                    else if (request.Tipo == Dominio.TipiIstanzaEnum.NonConsolidata)
                        tipo_cons = DocsPaConservazione.TipoIstanzaConservazione.CONSERVAZIONE_NON_CONSOLIDATA;
                    else if (request.Tipo == Dominio.TipiIstanzaEnum.Esibizione)
                        tipo_cons = DocsPaConservazione.TipoIstanzaConservazione.ESIBIZIONE;
                    else if (request.Tipo == Dominio.TipiIstanzaEnum.Interna)
                        tipo_cons = DocsPaConservazione.TipoIstanzaConservazione.CONSERVAZIONE_INTERNA;

                    try
                    {
                        BusinessLogic.Documenti.areaConservazioneManager.updateAreaConservazione(istanza.Id, tipo_cons, note, descrizione, "", consolidata);

                        response.Success = true;
                    }
                    catch (Exception ex)
                    {
                        throw new PacchettiException(ErrorCodes.INVIO_CENTRO_SERVIZI_FALLITO, ErrorDescriptions.INVIO_CENTRO_SERVIZI_FALLITO);
                    }
                }
                else
                {
                    throw new PacchettiException(response.Error.Code, response.Error.Description);
                }
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public Services.GetSupporti.GetSupportiResponse GetSupporti(Services.GetSupporti.GetSupportiRequest request)
        {
            Services.GetSupporti.GetSupportiResponse response = new Services.GetSupporti.GetSupportiResponse();

            try
            {
                // 1. Validazione token di autenticazione
                this.CheckAuthenticationToken(request);

                // 2. Ripristino contesto utente
                DocsPaVO.utente.InfoUtente infoUtente = this.Impersonate(request.UserName);

                // 3. Reperimento istanza di conservazione
                Services.GetIstanza.GetIstanzaResponse getIstanzaResponse = this.GetIstanza(new Services.GetIstanza.GetIstanzaRequest
                {
                    UserName = request.UserName,
                    AuthenticationToken = request.AuthenticationToken,
                    IdIstanza = request.IdIstanza

                });
                if (getIstanzaResponse.Success)
                {
                    // 4. Reperimento dei supporti dell'istanza
                    DocsPaConservazione.DocsPaConsManager consManager = new DocsPaConservazione.DocsPaConsManager();

                    List<Dominio.Supporto> supporti = new List<Dominio.Supporto>();

                    foreach (DocsPaVO.areaConservazione.InfoSupporto itm in consManager.RicercaInfoSupporto(string.Format(" A.ID_CONSERVAZIONE = {0} and c.SYSTEM_ID= {1}", request.IdIstanza, request.IdIstanza)))
                    {
                        supporti.Add(new Dominio.Supporto
                        {
                            Id = itm.SystemID,
                            Tipo = (itm.numCopia == "0" ? Dominio.TipiSupportoEnum.Storage : Dominio.TipiSupportoEnum.SupportoEsterno),
                            DataProduzione = itm.dataProduzione,
                            NumeroCopia = itm.numCopia,
                            Url = itm.istanzaDownloadUrl
                        });
                    }

                    response.Success = true;
                    response.Supporti = supporti.ToArray();
                }
                else
                {
                    // Istanza non trovata
                    throw new PacchettiException(getIstanzaResponse.Error.Code, getIstanzaResponse.Error.Description);
                }
            }
            catch (PacchettiException pacchettiEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pacchettiEx.ErrorCode,
                    Description = pacchettiEx.Message
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = ErrorCodes.ERRORE_NON_GESTITO,
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        #endregion
    }
}
