using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using DocsPaVO.utente;
using System.Threading.Tasks;
using DocsPaVO.Mobile.Responses;
using DocsPaVO.Mobile;
using DocsPaRESTWS.Model;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using DocsPaDB.Query_DocsPAWS;
using DocsPaVO.ricerche;
using System.Collections;
using DocsPaVO.trasmissione;


namespace DocsPaRESTWS.Manager
{
    public class MobileManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(MobileManager));
        #region Mobile
        public static async Task<DocsPaVO.Mobile.Responses.LoginResponse> login(DocsPaRESTWS.Model.AuthenticateRequest loginRequest)
        {
            logger.Info("LoginResponse - begin");
            DocsPaVO.Mobile.Responses.LoginResponse retval = null;
            DocsPaVO.utente.UserLogin.LoginResult loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
            string ipAddress = "";
            try
            {
                logger.Debug("ricerca user con userName " + loginRequest.Username);
                UserLogin usrLog = new UserLogin();
                usrLog.Password = loginRequest.Password;
                usrLog.UserName = loginRequest.Username;
               
                if (!string.IsNullOrEmpty(loginRequest.idAmministrazione))
                    usrLog.IdAmministrazione = loginRequest.idAmministrazione;
                Utente utente = BusinessLogic.Utenti.Login.loginMethod(usrLog, out loginResult, true,
                        null, out ipAddress);


                logger.Debug("user found");
                if (utente != null)
                {
                    utente.codWorkingApplication = "mobileApp";
                    // BusinessLogic.UserLog.UserLog.WriteLog(utente.userId, utente.idPeople, usrLog.IdAmministrazione, ((DocsPaVO.utente.Corrispondente)(utente)).idAmministrazione, "LOGIN", utente.systemId, utente.systemId, DocsPaVO.Logger.CodAzione.Esito.OK, null);
                    BusinessLogic.UserLog.UserLog.WriteLog(new InfoUtente(utente, (Ruolo)utente.ruoli[0]), "LOGIN", utente.systemId, utente.systemId, DocsPaVO.Logger.CodAzione.Esito.OK);

                    retval = new DocsPaVO.Mobile.Responses.LoginResponse(utente, loginResult);

                    Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(retval.UserInfo.Ruoli[0].IdGruppo);
                    bool permessoOTP = false;
                    bool abilitatoCondivisione = false;
                    try
                    {
                        if (ruolo != null)
                        {
                            permessoOTP = (from function in (Funzione[])ruolo.funzioni.ToArray(typeof(Funzione)) where function.codice.ToUpper().Equals("TO_GET_OTP") select function.systemId).Count() > 0 ? true : false;
                            abilitatoCondivisione = (from function in (Funzione[])ruolo.funzioni.ToArray(typeof(Funzione)) where function.codice.ToUpper().Equals("DO_CONDIVIDI_MOBILE") select function.systemId).Count() > 0 ? true : false;

                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("isAllowedOTP - Exception: " + ex);
                    }
                    retval.OTPAllowed = permessoOTP;
                    retval.ShareAllowed = abilitatoCondivisione;
                    InfoUtente infoutente = new InfoUtente(utente, ruolo);
                    Memento infoMemento = new Memento();
                    // recupera le info del memento
                    string[] infoMementoResult = BusinessLogic.Utenti.UserManager.getMementoUtente(infoutente);
                    if (infoMementoResult.Length == 2)
                    {
                        infoMemento.Dominio = infoMementoResult[0].ToUpper();
                        infoMemento.Alias = infoMementoResult[1].ToUpper();
                    }
                    retval.InfoMemento = infoMemento;

                    retval.UserInfo.Token = Utils.CreateAuthToken(utente, ruolo);

                    string set_data_vista_grd = System.Configuration.ConfigurationManager.AppSettings["SET_DATA_VISTA_GRD"];
                    if (!string.IsNullOrWhiteSpace(set_data_vista_grd) && set_data_vista_grd.ToUpper() == "2")
                        retval.TodoListRemoval = "Manual";
                    else if (!string.IsNullOrWhiteSpace(set_data_vista_grd) && set_data_vista_grd.ToUpper() == "TRUE")
                        retval.TodoListRemoval = "Automatic";
                }
                else
                {
                    retval = new LoginResponse(loginResult);
                }
                logger.Info("LoginResponse - END");
                return retval;
            }
            catch (Exception ex)
            {
                logger.Error("Login - eccezione: " + ex);
                return DocsPaVO.Mobile.Responses.LoginResponse.ErrorResponse;
            }
        }

        public static async Task<LoginResponse> cambioPassword(AuthenticateRequest loginRequest)
        {
            logger.Info("cambioPassword - begin");
            DocsPaVO.Mobile.Responses.LoginResponse retval = null;
            DocsPaVO.utente.UserLogin.LoginResult loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
            string ipAddress = "";
            try
            {
                logger.Debug("ricerca user con userName " + loginRequest.Username);
                UserLogin usrLog = new UserLogin();
                usrLog.Password = loginRequest.Password;
                usrLog.UserName = loginRequest.Username;
                DocsPaVO.Validations.ValidationResultInfo resultChange = BusinessLogic.Utenti.Login.ChangePassword(usrLog, loginRequest.OldPassword);
                if (!resultChange.Value)
                {
                    logger.Error("cambioPassword - eccezione: " + ((DocsPaVO.Validations.BrokenRule)resultChange.BrokenRules[0]).Description);
                    return DocsPaVO.Mobile.Responses.LoginResponse.ErrorResponse;
                }
                else
                {
                    Utente utente = BusinessLogic.Utenti.Login.loginMethod(usrLog, out loginResult, true,
                        null, out ipAddress);
                    logger.Debug("user found");
                    retval = new DocsPaVO.Mobile.Responses.LoginResponse(utente, loginResult);
                    if (retval.Code == LoginResponseCode.OK)
                    {
                        Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(retval.UserInfo.Ruoli[0].IdGruppo);
                        retval.UserInfo.Token = Utils.CreateAuthToken(utente, ruolo);
                    }
                    logger.Info("cambioPassword - END");
                    return retval;
                }

            }
            catch (Exception ex)
            {
                logger.Error("cambioPassword - eccezione: " + ex);
                return DocsPaVO.Mobile.Responses.LoginResponse.ErrorResponse;
            }
        }

        public static async Task<LoginResponse> resetPassword(AuthenticateRequest loginRequest )
        {
            logger.Info("resetPassword - begin");
            DocsPaVO.Mobile.Responses.LoginResponse retval = null;
            DocsPaVO.utente.UserLogin.LoginResult loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
            string ipAddress = "";
            string otp = loginRequest.Otp;

            try
            {
                logger.Debug("ricerca user con userName " + loginRequest.Username);
                UserLogin usrLog = new UserLogin();
                usrLog.Password = loginRequest.Password;
                usrLog.UserName =  loginRequest.Username;
                DocsPaVO.Validations.ValidationResultInfo resultChange = BusinessLogic.Utenti.UserManager.ResetPasswordUtente(usrLog, otp);
                if (!resultChange.Value)
                {
                    logger.Error("resetPassword - eccezione: " + ((DocsPaVO.Validations.BrokenRule)resultChange.BrokenRules[0]).Description);
                   
                   //return DocsPaVO.Mobile.Responses.LoginResponse.ErrorResponse;
                    return new DocsPaVO.Mobile.Responses.LoginResponse(getOtpErrorCode(((DocsPaVO.Validations.BrokenRule)resultChange.BrokenRules[0]).ID));
                
                }
                else
                {
                    Utente utente = BusinessLogic.Utenti.Login.loginMethod(usrLog, out loginResult, true,
                        null, out ipAddress);
                    logger.Debug("user found");
                    retval = new DocsPaVO.Mobile.Responses.LoginResponse(utente, loginResult);
                    if (retval.Code == LoginResponseCode.OK)
                    {
                        Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(retval.UserInfo.Ruoli[0].IdGruppo);
                        retval.UserInfo.Token = Utils.CreateAuthToken(utente, ruolo);
                    }
                    logger.Info("cambioPassword - END");
                    return retval;
                }

            }
            catch (Exception ex)
            {
                logger.Error("cambioPassword - eccezione: " + ex);
                return DocsPaVO.Mobile.Responses.LoginResponse.ErrorResponse;
            }
        }

        private static LoginResponseCode getOtpErrorCode(string code)
        {
            switch (code)
            {
                case "INVALID_OTP":
                    return LoginResponseCode.INVALID_OTP;
                case "PASSWORD_EQUALITY":
                    return LoginResponseCode.PASSWORD_EQUALITY;
                case "DOMAIN_AUTH_ENABLED":
                    return LoginResponseCode.DOMAIN_AUTH_ENABLED;
                default:
                return LoginResponseCode.SYSTEM_ERROR;
            }
       
        }

        public static async Task<LoginResponse> resetPasswordInviaOTP(AuthenticateRequest loginRequest)
        {
            logger.Info("resetPassword get OTP- begin");
            DocsPaVO.Mobile.Responses.LoginResponse retval = null;
            DocsPaVO.utente.UserLogin.LoginResult loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
            string ipAddress = "";
            string email = "";
            try
            {
                logger.Debug("ricerca user con userName " + loginRequest.Username);
             /*   UserLogin usrLog = new UserLogin();
               usrLog.Password = loginRequest.Password;
                  usrLog.UserName = loginRequest.Username;
                  */
                UserLogin.ResetPasswordResult resultOTP = BusinessLogic.Utenti.UserManager.ResetPasswordInviaOTP(loginRequest.Username, out email);

              //  DocsPaVO.Validations.ValidationResultInfo resultChange = BusinessLogic.Utenti.UserManager.ResetPasswordInviaOTP(usrLog.UserName, out email);
                if (resultOTP != UserLogin.ResetPasswordResult.OK)
                {
                    logger.Error("resetPassword otp ko  " );
                    return new LoginResponse(resultOTP);  //DocsPaVO.Mobile.Responses.LoginResponse.ErrorResponse;
                }
                else
                {
                //    Utente utente = BusinessLogic.Utenti.Login.loginMethod(usrLog, out loginResult, true, null, out ipAddress);
                    logger.Debug("user found");
                    retval = new DocsPaVO.Mobile.Responses.LoginResponse(loginRequest.Username, email);
                    /*    if (retval.Code == LoginResponseCode.OK)
                        {
                            Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(retval.UserInfo.Ruoli[0].IdGruppo);
                            retval.UserInfo.Token = Utils.CreateAuthToken(utente, ruolo);
                        }
                      */
                    logger.Info("cambioPassword get OTP - END");
                    return retval;
                }

            }
            catch (Exception ex)
            {
                logger.Error("cambioPassword - eccezione: " + ex);
                return DocsPaVO.Mobile.Responses.LoginResponse.ErrorResponse;
            }
        }

        public static async Task<VerifyUpdateResponse> verifyUpdate(VerifyUpdateRequest updateRequest)
        {
            logger.Info("verifica disponibilita aggironaemnto mobile- begin");
            DocsPaVO.Mobile.Responses.VerifyUpdateResponse retval = null;
            DocsPaVO.utente.UserLogin.LoginResult loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
            string url = "";
         
            try
            {
                logger.Debug("verifica aggironamento versione " + updateRequest.Version);
                /*   UserLogin usrLog = new UserLogin();
                  usrLog.Password = loginRequest.Password;
                     usrLog.UserName = loginRequest.Username;
                     */
                //  UserLogin.ResetPasswordResult resultOTP = BusinessLogic.Utenti.UserManager.ResetPasswordInviaOTP(loginRequest.Username, out email);

                //  DocsPaVO.Validations.ValidationResultInfo resultChange = BusinessLogic.Utenti.UserManager.ResetPasswordInviaOTP(usrLog.UserName, out email);

                url = BusinessLogic.Mobile.VerifyUpdate.getUrlUpdate(updateRequest.Version , updateRequest.Model, updateRequest.Brand);

                if(!string.IsNullOrEmpty(url))
                {
                    retval = new DocsPaVO.Mobile.Responses.VerifyUpdateResponse(url);
                    return retval;
                }

                //retval = new DocsPaVO.Mobile.Responses.VerifyUpdateResponse("www.google.com");
                //return retval;



            }
            catch (Exception ex)
            {
                logger.Error("verifica update mobile - eccezione: " + ex);
                
            }
            return DocsPaVO.Mobile.Responses.VerifyUpdateResponse.NoUpdate;
        }

        public static async Task<DocsPaVO.utente.Amministrazione[]> getListaAmmByUser(string userId)
        {
            logger.Info("getListaAmmByUser - begin");
            ArrayList retAList = BusinessLogic.Amministrazione.AmministraManager.GetAmministrazioniByUser(userId, true);
            return (DocsPaVO.utente.Amministrazione[])retAList.ToArray(typeof(DocsPaVO.utente.Amministrazione));

        }

        public static async Task<LogoutResponse> logout(string token, string dst)
        {
            logger.Info("Logout - BEGIN");
            try
            {
                DocsPaVO.utente.Utente utente = null;
                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');
                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                BusinessLogic.Utenti.Login.logoff(utente.systemId, utente.idAmministrazione, dst);
                LogoutResponse res = new LogoutResponse(LogoutResponseCode.OK);
                logger.Info("Logout - END");
                return res;
            }
            catch (Exception e)
            {
                logger.Error("Logout - eccezione: " + e);
                return LogoutResponse.ErrorResponse;
            }
        }

        public static async Task<ToDoListResponse> getTodoList(string token, int requestedPage, int pageSize)
        {
            logger.Info("getTodoList - Begin");
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                int totalRecordCount;
                string tipoRagione = "";
                ToDoListResponse res = new ToDoListResponse();
                res.Elements = new List<ToDoListElement>();
                logger.Debug("Prelievo todolist");
                List<DocsPaVO.Notification.Notification> listNotificatione = BusinessLogic.ServiceNotifications.Notification.ReadNotificationsMobile(infoUtente.idPeople, infoUtente.idGruppo, requestedPage, pageSize, out totalRecordCount);
                logger.Debug("Todolist prelevata. E' null? " + listNotificatione == null ? "SI" : "NO");
                foreach (DocsPaVO.Notification.Notification notification in listNotificatione)
                {
                    string idTrasmissione = string.Empty;
                    if (!string.IsNullOrEmpty(notification.ID_SPECIALIZED_OBJECT))
                    {
                        idTrasmissione = BusinessLogic.Trasmissioni.TrasmManager.GetIdTrasmissioneByIdTrasmSingola(notification.ID_SPECIALIZED_OBJECT);
                        tipoRagione = BusinessLogic.Trasmissioni.TrasmManager.getTipoRagioneDaTramSingola(notification.ID_SPECIALIZED_OBJECT);
                    }
                    logger.Debug("Type_event: " + notification.TYPE_EVENT);
                    res.Elements.Add(ToDoListElement.buildInstance(notification, idTrasmissione, tipoRagione));


                }
                res.TotalRecordCount = totalRecordCount;

                logger.Info("getTodoList - end");
                return res;
            }
            catch (Exception e)
            {
                logger.Error("getTodoList - eccezione: " + e);
                return ToDoListResponse.ErrorResponse;
            }
        }

        public static async Task<DocsPaVO.Mobile.Responses.LoginResponse> cambiaRuolo(string token, string idCorrRuolo)
        {
            logger.Info("cambiaRuolo - BEGIN");
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuolo(idCorrRuolo);
                infoUtente = new InfoUtente(utente, ruolo);
                DocsPaVO.Mobile.Responses.LoginResponse retval = null;
                DocsPaVO.utente.UserLogin.LoginResult loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
                if (utente != null && utente.ruoli == null)
                {
                    utente.ruoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(utente.idPeople);

                }

                retval = new DocsPaVO.Mobile.Responses.LoginResponse(utente, loginResult);

                bool permessoOTP = false;
                bool abilitatoCondivisione = false;
                try
                {
                    if (ruolo != null)
                    {
                        permessoOTP = (from function in (Funzione[])ruolo.funzioni.ToArray(typeof(Funzione)) where function.codice.ToUpper().Equals("TO_GET_OTP") select function.systemId).Count() > 0 ? true : false;
                        abilitatoCondivisione = (from function in (Funzione[])ruolo.funzioni.ToArray(typeof(Funzione)) where function.codice.ToUpper().Equals("DO_CONDIVIDI_MOBILE") select function.systemId).Count() > 0 ? true : false;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("isAllowedOTP - Exception: " + ex);
                }                
                retval.OTPAllowed = permessoOTP;
                retval.ShareAllowed = abilitatoCondivisione;
                InfoUtente infoutente = new InfoUtente(utente, ruolo);
                Memento infoMemento = new Memento();
                // recupera le info del memento
                string[] infoMementoResult = BusinessLogic.Utenti.UserManager.getMementoUtente(infoutente);
                if (infoMementoResult.Length == 2)
                {
                    infoMemento.Dominio = infoMementoResult[0].ToUpper();
                    infoMemento.Alias = infoMementoResult[1].ToUpper();
                }
                string set_data_vista_grd = System.Configuration.ConfigurationManager.AppSettings["SET_DATA_VISTA_GRD"];
                if (!string.IsNullOrWhiteSpace(set_data_vista_grd) && set_data_vista_grd.ToUpper() == "2")
                    retval.TodoListRemoval = "Manual";
                else if (!string.IsNullOrWhiteSpace(set_data_vista_grd) && set_data_vista_grd.ToUpper() == "TRUE")
                    retval.TodoListRemoval = "Automatic";
                retval.InfoMemento = infoMemento;

                retval.UserInfo.Token = Utils.CreateAuthToken(utente, ruolo);

                string tokenRet = Utils.CreateAuthToken(utente, ruolo);
                logger.Info("cambiaRuolo - END");
                return retval;

            }
            catch (Exception e)
            {
                logger.Error("cambiaRuolo - eccezione: " + e);
                return DocsPaVO.Mobile.Responses.LoginResponse.ErrorResponse;
            }
        }

        public static async Task<ArrayList> ListaIstanze()
        {
            ArrayList istanze = new ArrayList();
            try
            {
                string pathListaIstanze = HttpContext.Current.Server.MapPath("xml/listaIstanze.xml");
                logger.Debug(pathListaIstanze);
                if (System.IO.File.Exists(pathListaIstanze))
                {
                    string xml = System.IO.File.ReadAllText(pathListaIstanze);
                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    xmlDoc.LoadXml(xml);
                    DocsPaVO.Mobile.Istanza istanza = null;
                    string nome = "", descrizione = "", tipo = "", url = "";
                    foreach (System.Xml.XmlNode x1 in xmlDoc.ChildNodes[1].ChildNodes)
                    {
                        if (x1.Name == "Istanza")
                        {
                            //Console.WriteLine(x1.Name+" - "+ x1.InnerXml);
                            foreach (System.Xml.XmlNode x2 in x1.ChildNodes)
                            {
                                switch (x2.Name)
                                {
                                    case "Nome":
                                        nome = x2.InnerText;
                                        break;
                                    case "Descrizione":
                                        descrizione = x2.InnerText;
                                        break;
                                    case "Tipo":
                                        tipo = x2.InnerText;
                                        break;
                                    case "URL":
                                        url = x2.InnerText;
                                        break;
                                }
                            }
                            istanza = new Istanza(nome, descrizione, tipo, url);
                            istanze.Add(istanza);
                        }
                    }
                }
                else { return null; }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
            return istanze;
        }
        #endregion
        #region Documenti
        public static async Task<GetDocInfoResponse> getDocInfo(string token, GetDocInfoRequest request)
        {
            logger.Info("getDocInfo - BEGIN");
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                GetDocInfoResponse resp = new GetDocInfoResponse();
                if (!string.IsNullOrEmpty(request.IdTrasm))
                {
                    DocsPaVO.trasmissione.Trasmissione trasm = BusinessLogic.Trasmissioni.TrasmManager.CreateObjTrasmissioneByID(request.IdTrasm);
                    logger.Debug("data invio: " + trasm.dataInvio);
                    Utente delegato = null;
                    // Gabriele Melini 17-02-2014
                    //if (trasm.delegato != null)
                    if (!string.IsNullOrEmpty(trasm.delegato))
                    {
                        try
                        {
                            delegato = BusinessLogic.Utenti.UserManager.getUtenteById(trasm.delegato);
                        }
                        catch (Exception e1)
                        {
                            delegato = null;
                        }
                    }
                    resp.TrasmInfo = Utils.buildInstanceTrasmInfo(trasm, delegato, infoUtente);
                }
                List<BaseInfoDoc> infos = BusinessLogic.Documenti.DocManager.GetBaseInfoForDocument(request.idDoc, request.idDoc, null);
                logger.Debug("Infodoc prelevati? " + infos == null ? "NO" : "si num " + infos.Count);
                if (infos.Count > 0)
                    resp.Allegati = new List<DocInfo>();

                foreach (BaseInfoDoc info in infos)
                    resp.Allegati.Add(generateDocInfo(resp, infoUtente, info));

                resp.DocInfo = resp.Allegati.FirstOrDefault();
                if (string.IsNullOrEmpty(request.IdTrasm) && !string.IsNullOrEmpty(request.IdEvento) && Cfg_SET_DATA_VISTA_GRD != "2")
                {
                    DocsPaDB.Query_DocsPAWS.NotificationDB notification = new NotificationDB();
                    notification.CheckNotification(request.IdEvento);
                }

                //DOCUMENTOGETDETTAGLIODOCUMENTO
                if (resp != null && resp.DocInfo != null)
                {
                    var VarDescOggetto="";
                    if (!string.IsNullOrWhiteSpace(resp.DocInfo.Segnatura))
                        VarDescOggetto = string.Format("{0}{1} / {2}{3}", "N.ro Doc.:", resp.DocInfo.IdDoc, "Segnatura: ", resp.DocInfo.Segnatura);
                    else
                        VarDescOggetto = string.Format("{0}{1}", "N.ro Doc.:", resp.DocInfo.IdDoc + " Creato il " + resp.DocInfo.DataDoc);
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETDETTAGLIODOCUMENTO", resp.DocInfo.IdDoc, VarDescOggetto, DocsPaVO.Logger.CodAzione.Esito.OK);
                }

                logger.Info("getDocInfo - end ");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("getDocInfo - eccezione: " + e);
                return GetDocInfoResponse.ErrorResponse;
            }
        }

        private static string Cfg_SET_DATA_VISTA_GRD
        {
            get
            {
                string eme = System.Configuration.ConfigurationManager.AppSettings["SET_DATA_VISTA_GRD"];
                return (eme != null) ? eme : "0";
            }
        }


        private static DocInfo generateDocInfo(GetDocInfoResponse resp, InfoUtente infoUtente, BaseInfoDoc doc)
        {
            logger.Debug("generateDocInfo: " + doc == null ? "doc è null" : "doc corretto");
            SchedaDocumento schedaDoc = BusinessLogic.Mobile.DocManager.getDettaglioMobile(infoUtente, doc.IdProfile, doc.DocNumber);
            IEnumerable<Fascicolo> fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDoc(infoUtente, doc.IdProfile).Cast<Fascicolo>();
            //logger.Debug(String.Format("SLOG Data1 [{0}]  Data2[{1}]  TipoProto[{2}]  daproto[{3}], isegnatura[{4}] ", schedaDoc.dataCreazione.ToString(), schedaDoc.protocollo.dataProtocollazione.ToString(), schedaDoc.tipoProto, schedaDoc.protocollo.daProtocollare.ToString(), schedaDoc.protocollo.segnatura));
            return DocInfo.buildInstance(schedaDoc, doc.OriginalFileName, fascicoli, doc.HaveFile, doc.HaveFile && doc.FileName.ToUpper().Contains(".PDF"));
        }

        private static DocInfo generateDocInfo(InfoUtente infoUtente, BaseInfoDoc doc)
        {
            logger.Debug("generateDocInfo: " + doc == null ? "doc è null" : "doc corretto");
            SchedaDocumento schedaDoc = BusinessLogic.Mobile.DocManager.getDettaglioMobile(infoUtente, doc.IdProfile, doc.DocNumber);
            IEnumerable<Fascicolo> fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDoc(infoUtente, doc.IdProfile).Cast<Fascicolo>();
            return DocInfo.buildInstance(schedaDoc, doc.OriginalFileName, fascicoli, doc.HaveFile, doc.HaveFile && doc.FileName.ToUpper().Contains(".PDF"));
        }

        private static DocInfo generateDocInfo2(InfoUtente infoUtente, BaseInfoDoc doc)
        {
            logger.Debug("generateDocInfo2: " + doc == null ? "doc è null" : "doc corretto");
            SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, doc.DocNumber);
            //IEnumerable<Fascicolo> fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDoc(infoUtente, doc.IdProfile).Cast<Fascicolo>();
            return DocInfo.buildInstance(schedaDoc, doc.OriginalFileName, null, doc.HaveFile, doc.HaveFile && doc.FileName.ToUpper().Contains(".PDF"));
        }

        public static async Task<bool> rimuoviNotifica(string idEvento)
        {
            logger.Info("rimuoviNotifica - begin");
            try
            {
                DocsPaDB.Query_DocsPAWS.NotificationDB notification = new NotificationDB();
                logger.Info("rimuoviNotifica - end");

                return notification.CheckNotification(idEvento);
            }
            catch (Exception e)
            {
                logger.Error("rimuoviNotifica - eccezione: " + e);
                return false;
            }
        }

        public static async Task<GetFileResponse> getFile(string token, string idDoc, string getSignedFile)
        {
            logger.Info("getFile - BEGIN");
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                GetFileResponse resp = new GetFileResponse();

                List<BaseInfoDoc> infos = BusinessLogic.Documenti.DocManager.GetBaseInfoForDocument(idDoc, null, null);
                BaseInfoDoc doc = infos[0];
                bool sbusta = true;
                //if (doc.FileName.ToUpper().EndsWith(".P7M") ||
                //    doc.FileName.ToUpper().EndsWith(".TSD"))
                //{
                //    sbusta = false;
                //}
                if (getSignedFile == "1") sbusta = true;

                DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, idDoc);
                DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];

                //BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDoc,null);
                DocsPaVO.documento.FileDocumento fileDocumento = null;
                if (sbusta)
                    fileDocumento = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente);
                else
                    fileDocumento = BusinessLogic.Documenti.FileManager.getFileFirmato(fileRequest, infoUtente, false);

                resp.File = DocsPaVO.Mobile.FileInfo.buildInstance(fileDocumento);

                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETFILE", fileRequest.docNumber, string.Format("{0} {1} {2} {3}", "Visualizzazione Doc. nr.", fileRequest.docNumber, "Ver.", fileRequest.version), DocsPaVO.Logger.CodAzione.Esito.OK);
                    
                logger.Info("getFile - END");

                return resp;
            }
            catch (Exception e)
            {
                logger.Error("getFile - eccezione: " + e);
                return GetFileResponse.ErrorResponse;
            }
        }

        public static async Task<ADLActionResponse> ADLAction(string token, AdlActionRequest request)
        {
            logger.Debug("ADLAction - begin");

            DocsPaVO.utente.Utente utente = null;
            DocsPaVO.utente.InfoUtente infoUtente = null;
            DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

            string authInfoString = Utils.Decrypt(token.Substring(4));
            string[] authInfoArray = authInfoString.Split('|');

            utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
            ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
            infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

            ADLActionResponse response = new ADLActionResponse { Code = AddToADLResponseCode.SYSTEM_ERROR };
            string idProfile = request.IdElemento;
            string tipoProto = request.TipoElemento;
            string idRegistro = ruolo.idRegistro;
            Fascicolo fasc = null;
            SchedaDocumento docX = null;

            bool retOperation = false;

            if (!string.IsNullOrWhiteSpace(tipoProto) && tipoProto.Equals("F"))
            {   // si tratta di un fascicolo
                fasc = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(idProfile, infoUtente);
                idProfile = null;
                tipoProto = null;
            }
            else
            {
                if (request.ADLAction.ToUpper() == "ADD")
                {
                    docX = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idProfile, idProfile);
                    tipoProto = docX.tipoProto;
                }
            }

            try
            {
                if (request.ADLAction.ToUpper() == "ADD")
                    BusinessLogic.Documenti.areaLavoroManager.execAddLavoroMethod(idProfile, tipoProto, idRegistro, infoUtente, fasc);
                else if (request.ADLAction.ToUpper() == "REMOVE")
                    BusinessLogic.Documenti.areaLavoroManager.cancellaAreaLavoro(infoUtente.idPeople, infoUtente.idCorrGlobali, idProfile, fasc);

                response.Code = AddToADLResponseCode.OK;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore in addToAdl, {0} {1}", e.Message, e.StackTrace);
                response.Code = AddToADLResponseCode.SYSTEM_ERROR;
            }
            logger.Info("ADLAction - END");
            return response;

        }

        public static async Task<string> getTokenCondivisioneDocumento(string token, string idpeople, string idDocumento)
        {
            logger.Info("getTokenCondivisioneDocumento - BEGIN");
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                //devo cambiare la risposta in un oggetto più utile
                string retval = Utils.getCondivisionToken(idpeople, idDocumento);
                //retval = System.Uri.EscapeDataString(retval);
                logger.Info("getTokenCondivisioneDocumento - END");
                return retval;
            }
            catch (Exception e)
            {
                logger.Error("getTokenCondivisioneDocumento - eccezione: " + e);
                return "ERROR";
            }
        }

        public static async Task<DocCondivisoResponse> ctrlTokenCondivisioneDoc(string token, string tokenDoc)
        {
            logger.Info("ctrlTokenCondivisioneDoc - BEGIN");
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                //devo cambiare la risposta in un oggetto più utile

                DocCondivisoResponse resp = new DocCondivisoResponse();
                string idDocumento = Utils.ctrlCondivisioneToken(infoUtente.idPeople, utente.systemId, tokenDoc);
                if (!string.IsNullOrWhiteSpace(idDocumento))
                {
                    switch (idDocumento)
                    {
                        case "WRONG_USER":
                            resp = new DocCondivisoResponse(DocCondivisoResponseCode.WRONG_USER);
                            break;
                        case "EXPIRED":
                            resp = new DocCondivisoResponse(DocCondivisoResponseCode.EXPIRED);
                            break;
                        case "ERROR_TOKEN":
                            resp = DocCondivisoResponse.ErrorResponse;
                            break;
                        default:
                            List<BaseInfoDoc> infos = BusinessLogic.Documenti.DocManager.GetBaseInfoForDocument(idDocumento, idDocumento, null);
                            logger.Debug("Infodoc prelevati? " + infos == null ? "NO" : "si num " + infos.Count);
                            if (infos.Count > 0)
                                resp.Allegati = new List<DocInfo>();

                            foreach (BaseInfoDoc info in infos)
                                resp.Allegati.Add(generateDocInfo2(infoUtente, info));
                            resp.DocInfo = resp.Allegati.FirstOrDefault();
                            break;
                    }
                }

                return resp;
            }
            catch (Exception e)
            {
                logger.Error("ctrlTokenCondivisioneDoc - eccezione: " + e);
                return DocCondivisoResponse.ErrorResponse;
            }
        }
        #endregion
        #region Fascicoli
        public static async Task<GetFascInfoResponse> getFascInfo(string token, GetFascInfoRequest request)
        {
            logger.Info("getFascInfo - START");
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                GetFascInfoResponse response = new GetFascInfoResponse();
                if (!string.IsNullOrEmpty(request.IdTrasm))
                {
                    DocsPaVO.trasmissione.Trasmissione trasm = BusinessLogic.Trasmissioni.TrasmManager.CreateObjTrasmissioneByID(request.IdTrasm);
                    Utente delegato = null;
                    if (trasm.delegato != null)
                    {
                        try
                        {
                            delegato = BusinessLogic.Utenti.UserManager.getUtenteById(trasm.delegato);
                        }
                        catch (Exception e1)
                        {
                            delegato = null;
                        }
                    }
                    response.TrasmInfo = Utils.buildInstanceTrasmInfo(trasm, delegato, infoUtente);
                }
                Fascicolo fasc = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdFasc, infoUtente);
                Fascicoli fascicoli = new Fascicoli();
                fascicoli.SetDataVistaSP(infoUtente, request.IdFasc, "F");
                response.FascInfo = FascInfo.buildInstance(fasc);
                response.Code = GetFascInfoResponseCode.OK;

                logger.Info("getFascInfo - end");
                return response;
            }
            catch (Exception e)
            {
                logger.Error("getFascInfo - eccezione: " + e);
                return GetFascInfoResponse.ErrorResponse;
            }
        }
        #endregion
        #region Ricerca
        public static async Task<GetRicSalvateResponse> getRicSalvate(string token)
        {
            logger.Info("getRicSalvate - START");
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                GetRicSalvateResponse resp = null;
                resp.Risultati = new List<RicercaSalvata>();
                SearchItem[] ricSalvDoc = BusinessLogic.Documenti.InfoDocManager.GetSearchList(Int32.Parse(infoUtente.idPeople), Int32.Parse(infoUtente.idGruppo), null, false, "D");
                foreach (SearchItem temp in ricSalvDoc)
                {
                    resp.Risultati.Add(new RicercaSalvata("" + temp.system_id, temp.descrizione, RicercaSalvataType.RIC_DOCUMENTO));
                }
                SearchItem[] ricSalvFasc = BusinessLogic.Documenti.InfoDocManager.GetSearchList(Int32.Parse(infoUtente.idPeople), Int32.Parse(infoUtente.idGruppo), null, false, "F");
                foreach (SearchItem temp in ricSalvFasc)
                {
                    resp.Risultati.Add(new RicercaSalvata("" + temp.system_id, temp.descrizione, RicercaSalvataType.RIC_FASCICOLO));
                }
                resp.Code = GetRicSalvateResponseCode.OK;

                logger.Info("getRicSalvate - end");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("getRicSalvate - eccezione: " + e);
                return GetRicSalvateResponse.ErrorResponse;
            }
        }

        public static async Task<RicercaResponse> ricerca(string token, RicercaRequest request)
        {
            try
            {
                logger.Info("ricerca - begin");
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                RicercaResponse resp = new RicercaResponse();
                #region Debug della richiesta
                if (true)
                {
                    System.Xml.Serialization.XmlSerializer seri = new System.Xml.Serialization.XmlSerializer(typeof(RicercaRequest));
                    using (var sww = new System.IO.StringWriter())
                    {
                        using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sww))
                        {
                            seri.Serialize(writer, request);
                            string xmlDoc = sww.ToString(); // Your XML
                            logger.Debug(xmlDoc);
                        }
                    }
                }
                #endregion
                if (!string.IsNullOrEmpty(request.TipoRicercaSalvata))
                {
                    if (request.TipoRicercaSalvata.ToUpper() == "F") request.TypeRicercaSalvata = RicercaSalvataType.RIC_FASCICOLO;
                    else request.TypeRicercaSalvata = RicercaSalvataType.RIC_DOCUMENTO;
                }
                if (!string.IsNullOrEmpty(request.TipoRicerca))
                {
                    switch (request.TipoRicerca.ToUpper())
                    {
                        case "DOC":
                            request.TypeRicerca = RicercaType.RIC_DOCUMENTO;
                            break;
                        case "FASC":
                            request.TypeRicerca = RicercaType.RIC_FASCICOLO;
                            break;
                        case "ALL":
                            request.TypeRicerca = RicercaType.RIC_DOC_FASC;
                            break;
                        case "ADL_DOC":
                            request.TypeRicerca = RicercaType.RIC_DOCUMENTO_ADL;
                            break;
                        case "ADL_FASC":
                            request.TypeRicerca = RicercaType.RIC_FASCICOLO_ADL;
                            break;
                        case "ADL_ALL":
                            request.TypeRicerca = RicercaType.RIC_DOC_FASC_ADL;
                            break;
                    }
                }

                request.EnableProfilazione = false;
                request.EnableUfficioRef = false;
                logger.Debug("tiporicerca: " + request.TipoRicerca);
                DocsPaRESTWS.Model.RicercaStrategies.RicercaStrategy.GetStrategy(request).buildResponse(infoUtente, request, resp);
                resp.Code = RicercaResponseCode.OK;
                logger.Info("ricerca - end");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("ricerca - eccezione: " + e);
                return RicercaResponse.ErrorResponse;
            }
        }

        public static async Task<RicercaFascResponse> getDocsInFasc(string token, string idFasc, int pagesize, int numpage, string text)
        {
            try
            {
                logger.Info("ricerca - begin");
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                RicercaFascResponse response = new RicercaFascResponse();

                RicercaRequest request = new RicercaRequest();
                request.ParentFolderId = idFasc;
                request.FascId = idFasc;
                request.PageSize = pagesize;
                request.RequestedPage = numpage;
                request.Text = text;
                request.EnableProfilazione = false;
                request.EnableUfficioRef = false;
                RicercaResponse resp = new RicercaResponse();
                DocsPaRESTWS.Model.RicercaStrategies.RicercaStrategy.GetStrategy(request).buildResponse(infoUtente, request, resp);
                GetDocInfoResponse respDocInfo = null;
                RicercaFascElement elemFasc = null;
                if (resp != null && resp.Risultati != null && resp.Risultati.Count > 0)
                {
                    response.Risultati = new List<RicercaFascElement>();
                    foreach (RicercaElement elem in resp.Risultati)
                    {
                        elemFasc = new RicercaFascElement();
                        elemFasc.InfoElement = elem;
                        respDocInfo = new GetDocInfoResponse();
                        List<BaseInfoDoc> infos = BusinessLogic.Documenti.DocManager.GetBaseInfoForDocument(elem.Id, elem.Id, null);
                        if (infos.Count > 0)
                            respDocInfo.Allegati = new List<DocInfo>();

                        foreach (BaseInfoDoc info in infos)
                            respDocInfo.Allegati.Add(generateDocInfo(respDocInfo, infoUtente, info));

                        elemFasc.Documenti = respDocInfo.Allegati;

                        response.Risultati.Add(elemFasc);
                    }
                    response.TotalRecordCount = resp.Risultati.Count;
                }
                else
                {
                    response.TotalRecordCount = 0;
                }
                response.TotalRecordCount = resp.Risultati.Count;
                response.Code = RicercaFascResponseCode.OK;
                return response;
            }
            catch (Exception e)
            {
                logger.Error("ricerca - eccezione: " + e);
                return RicercaFascResponse.ErrorResponse;
            }
        }
        #endregion
        #region Deleghe
        public static async Task<CreaDelegaResponse> creaDelega(string token, Delega resDelega)
        {
            logger.Info("creaDelega - begin");
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                CreaDelegaResponse resp = new CreaDelegaResponse();
                Delega input = resDelega;
                DocsPaVO.Deleghe.InfoDelega delega = new DocsPaVO.Deleghe.InfoDelega();
                if (!string.IsNullOrEmpty(input.IdRuoloDelegante) && input.IdRuoloDelegante.ToUpper() == "TUTTI")
                {
                    logger.Debug("crea delega per tutti i ruoli");
                    delega.id_ruolo_delegante = "0";
                    delega.cod_ruolo_delegante = "TUTTI";
                }
                else
                {
                    // modifico il comportamento per fare in modo che venga preso il ruolo di autenticazione.
                    //logger.Debug("crea delega per il ruolo con id " + input.IdRuoloDelegante);
                    //delega.id_ruolo_delegante = input.IdRuoloDelegante;
                    //Ruolo ruoloDelegante = BusinessLogic.Utenti.UserManager.getRuoloById(input.IdRuoloDelegante);
                    //logger.Debug("codice ruolo delegante: " + ruoloDelegante.codice);
                    //delega.cod_ruolo_delegante = ruoloDelegante.codice;

                    logger.Debug("crea delega per il ruolo con id " + ruolo.systemId);
                    Ruolo ruoloDelegante = ruolo;
                    logger.Debug("codice ruolo delegante: " + ruoloDelegante.codice);
                    delega.cod_ruolo_delegante = ruoloDelegante.codice;
                    delega.id_ruolo_delegante = ruolo.systemId;
                }
                delega.id_utente_delegante = infoUtente.idPeople;
                logger.Debug("ricerca delegante...");
                Corrispondente delegante = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(infoUtente.idPeople, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                logger.Debug("delegante: " + delegante.codiceRubrica);
                delega.cod_utente_delegante = delegante.codiceRubrica;
                //ricerca delegato
                logger.Debug("ricerca delegato " + resDelega.IdDelegato);
                Corrispondente delegato = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(resDelega.IdDelegato, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);

                logger.Debug("delegato: " + delegato.codiceRubrica);
                delega.cod_utente_delegato = delegato.codiceRubrica;
                //ricerca ruoli delegato
                ArrayList ruoliDelegato = BusinessLogic.Utenti.UserManager.getRuoliUtente(input.IdDelegato);
                ruoliDelegato.Sort(new RuoliComparer());
                if (string.IsNullOrWhiteSpace(input.IdRuoloDelegato))
                {
                    delega.cod_ruolo_delegato = ((Ruolo)ruoliDelegato[0]).codice;
                    delega.id_ruolo_delegato = ((Ruolo)ruoliDelegato[0]).systemId;
                    delega.id_uo_delegato = ((Ruolo)ruoliDelegato[0]).uo.systemId;
                }
                else
                {
                    foreach (Ruolo rDelegato in ruoliDelegato)
                    {
                        if (input.IdRuoloDelegato.ToUpper() == rDelegato.systemId.ToUpper())
                        {
                            delega.cod_ruolo_delegato = rDelegato.codice;
                            delega.id_ruolo_delegato = rDelegato.systemId;
                            delega.id_uo_delegato = rDelegato.uo.systemId;
                        }
                    }
                }
                delega.id_utente_delegato = input.IdDelegato;
                /*
                delega.dataDecorrenza = Utils.buildDateString(input.DataDecorrenza).Replace('.', ':');
                logger.DebugFormat("Data Decorrenza {0}, Data Scadenza {1}, Data Scad 2 {2}", input.DataDecorrenza.ToShortDateString(), input.DataScadenza.ToShortDateString(), resDelega.DataScadenza.ToShortDateString());
                if (input.DataScadenza >= input.DataDecorrenza) delega.dataScadenza = Utils.buildDateString(input.DataScadenza, true).Replace('.', ':');
                */
                delega.dataDecorrenza = input.DataDecorrenza.ToString("dd/MM/yyyy HH:mm:ss");
                if (input.DataScadenza >= input.DataDecorrenza) delega.dataScadenza = input.DataScadenza.ToString("dd/MM/yyyy HH:mm:ss");

                logger.Debug("dataDecorrenza: " + delega.dataDecorrenza + " dataScadenza: " + delega.dataScadenza);

                if (BusinessLogic.Deleghe.DelegheManager.verificaUnicaDelega(infoUtente, delega))
                {
                    bool result = BusinessLogic.Deleghe.DelegheManager.creaNuovaDelega(infoUtente, delega);
                    if (result == false)
                    {
                        resp.Code = CreaDelegaResponseCode.NOT_CREATED;
                    }
                    else
                    {
                        resp.Code = CreaDelegaResponseCode.OK;
                    }
                }
                else
                {
                    resp.Code = CreaDelegaResponseCode.OVERLAPPING_PERIODS;
                }
                logger.Info("creaDelega - end");

                return resp;

            }
            catch (Exception e)
            {
                logger.Error("creaDelega - eccezione: " + e);
                return CreaDelegaResponse.ErrorResponse;
            }
        }

        public static async Task<DelegheResponse> listaDelegheAssegnate(string token, string statoDel)
        {
            logger.Info("listaDeleghe - begin");
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                DelegheResponse resp = new DelegheResponse();
                string tipoDelega = "assegnate";
                if (string.IsNullOrEmpty(statoDel)) statoDel = "T";
                string statoDelega = statoDel;
                bool attiveImpostate = false;
                switch (statoDel)
                {
                    case "A":
                    case "T":
                    case "S":
                    case "I":
                        statoDelega = statoDel;
                        break;
                    case "N":
                        statoDelega = "T";
                        attiveImpostate = true;
                        break;
                    default:
                        statoDelega = "T";
                        break;
                }

                DocsPaVO.Deleghe.SearchDelegaInfo sdInfo = new DocsPaVO.Deleghe.SearchDelegaInfo();
                sdInfo.StatoDelega = statoDelega;
                sdInfo.TipoDelega = tipoDelega;
                SearchPagingContext pagingContext = new SearchPagingContext();
                pagingContext.Page = 1;
                pagingContext.PageSize = 1000;
                List<DocsPaVO.Deleghe.InfoDelega> deleghe = BusinessLogic.Deleghe.DelegheManager.searchDeleghe(infoUtente, sdInfo, pagingContext);
                resp.Elements = new List<Delega>();
                foreach (DocsPaVO.Deleghe.InfoDelega temp in deleghe)
                {
                    logger.DebugFormat("Delega: decorr: {0} -> scad :{1} codDel {2} - IdRuoloDel: {3}", temp.dataDecorrenza, temp.dataScadenza, temp.codiceDelegante, temp.id_ruolo_delegante);
                    logger.DebugFormat("Delegato {0} - Ruolo {1} - Ruolo id {2}", temp.cod_utente_delegato, temp.cod_ruolo_delegato, temp.id_ruolo_delegato);
                    if (string.IsNullOrWhiteSpace(temp.id_ruolo_delegato) || "0".Equals(temp.id_ruolo_delegato))
                    {
                        if (!attiveImpostate || string.IsNullOrEmpty(temp.dataScadenza) || toDate(temp.dataScadenza).CompareTo(DateTime.Now) > 0)
                        {
                            logger.Debug("if errato");
                            resp.Elements.Add(Delega.buildInstance(temp, null, null));
                        }
                    }
                    else
                    {
                        if (!attiveImpostate || string.IsNullOrEmpty(temp.dataScadenza) || toDate(temp.dataScadenza).CompareTo(DateTime.Now) > 0)
                        {
                            Ruolo ruoloDelegato = BusinessLogic.Utenti.UserManager.getRuoloById(temp.id_ruolo_delegato);
                            Ruolo ruoloDelegante = BusinessLogic.Utenti.UserManager.getRuoloById(temp.id_ruolo_delegante);

                            resp.Elements.Add(Delega.buildInstance(temp, ruoloDelegato, ruoloDelegante));
                        }
                    }
                }
                //resp.TotalRecordCount = deleghe.Count;
                if (resp.Elements != null)
                    resp.TotalRecordCount = resp.Elements.Count;
                else resp.TotalRecordCount = 0;
                logger.Info("listaDeleghe - end");

                return resp;
            }
            catch (Exception e)
            {
                logger.Error("listaDeleghe - eccezione: " + e);
                return DelegheResponse.ErrorResponse;
            }
        }

        public static async Task<CreaDelegaDaModelloResponse> creaDelegaDaModello(string token, CreaDelegaDaModelloRequest request)
        {
            logger.Info("creaDelegaDaModello - begin");
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                CreaDelegaDaModelloResponse resp = new CreaDelegaDaModelloResponse();
                DocsPaVO.Deleghe.InfoDelega delega = BusinessLogic.Deleghe.ModelliDelegaManager.buildDelegaFromModello(infoUtente, request.IdModelloDelega, request.DataInizio, request.DataFine);
                bool result = BusinessLogic.Deleghe.DelegheManager.creaNuovaDelega(infoUtente, delega);
                if (result == false)
                {
                    resp.Code = CreaDelegaDaModelloResponseCode.NOT_CREATED;
                }
                else
                {
                    resp.Code = CreaDelegaDaModelloResponseCode.OK;
                }
                logger.Info("CreaDelegaDaModello - end");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("CreaDelegaDaModello - eccezione: " + e);
                return CreaDelegaDaModelloResponse.ErrorResponse;
            }
        }

        public static async Task<ListaModelliDelegaResponse> getListaModelliDelega(string token)
        {
            logger.Info("getListaModelliDelega - begin");
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                ListaModelliDelegaResponse resp = new ListaModelliDelegaResponse();

                resp.Modelli = new List<ModelloDelegaInfo>();
                List<DocsPaVO.Deleghe.ModelloDelega> tempList = BusinessLogic.Deleghe.ModelliDelegaManager.getModelliDelegaByStato(infoUtente, DocsPaVO.Deleghe.StatoModelloDelega.VALIDO);
                foreach (DocsPaVO.Deleghe.ModelloDelega temp in tempList)
                {
                    resp.Modelli.Add(ModelloDelegaInfo.buildInstance(temp));
                }
                resp.Code = ListaModelliDelegaResponseCode.OK;
                logger.Info("getListaModelliDelega - end");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("getListaModelliDelega - eccezione: " + e);
                return ListaModelliDelegaResponse.ErrorResponse;
            }
        }

        public static async Task<ListaTipiRuoloResponse> getListaRuoli(string token)
        {
            logger.Info("getListaRuoli - begin");
            try
            {
                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');
                ListaTipiRuoloResponse resp = new ListaTipiRuoloResponse();
                ArrayList list = BusinessLogic.Amministrazione.OrganigrammaManager.GetListTipiRuolo(authInfoArray[4]);
                resp.TipiRuolo = new List<TipoRuoloInfo>();
                foreach (DocsPaVO.amministrazione.OrgTipoRuolo temp in list)
                {
                    resp.TipiRuolo.Add(TipoRuoloInfo.buildInstance(temp));
                }
                resp.Code = ListaRuoliResponseCode.OK;
                logger.Info("getListaRuoli - end");

                return resp;
            }
            catch (Exception e)
            {
                logger.Error("getListaRuoli - eccezione: " + e);
                return ListaTipiRuoloResponse.ErrorResponse;
            }
        }

        public static async Task<ListaUtentiResponse> getListaUtenti(string token, string codiceTipoRuolo)
        {
            logger.Info("getListaUtenti - begin");
            try
            {
                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                ListaUtentiResponse resp = new ListaUtentiResponse();
                resp.Utenti = new List<UserInfo>();
                ArrayList ruoli = BusinessLogic.Amministrazione.TipiRuoloManager.ListTipoRuoloUtenti(codiceTipoRuolo, authInfoArray[4]);
                foreach (DocsPaVO.amministrazione.OrgRuolo temp in ruoli)
                {
                    foreach (DocsPaVO.amministrazione.OrgUtente utente in temp.Utenti)
                    {
                        resp.Utenti.Add(UserInfo.buildInstance(utente));
                    }
                }
                resp.Code = ListaUtentiResponseCode.OK;
                logger.Info("getListaUtenti - end");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("getListaUtenti - eccezione: " + e);
                return ListaUtentiResponse.ErrorResponse;
            }

        }

        public static async Task<RicercaUtentiWithRolesResponse> ricercaUtenti(string token, string descrizione, int numMaxResult)
        {
            logger.Info("ricercaUtenti - begin");
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuolo(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                UserInfo tempUt = null;
                ArrayList arrayRuoli = null;
                List<RuoloInfo> ruoliReturn = null;
                //RicercaUtentiResponse res = new RicercaUtentiResponse();
                RicercaUtentiWithRolesResponse res = new RicercaUtentiWithRolesResponse();
                RuoloInfo roleinfo = RuoloInfo.buildInstance(ruolo);
                List<String> idRegistri = (from a in roleinfo.Registri select a.SystemId).ToList();
                String idRegistro = idRegistri.FirstOrDefault();

                ArrayList listaIdPref = new ArrayList();

                List<InfoPreferito> listaPref = BusinessLogic.Mobile.RubricaManager.PrefMobile_getList(infoUtente.idPeople, true, "");
                foreach (InfoPreferito infPref in listaPref)
                {
                    listaIdPref.Add(infPref.IdInternal);
                }

                //res.Risultati = BusinessLogic.Mobile.RubricaManager.GetListaUtentiInterni(descrizione.ToUpper(), infoUtente, numMaxResult, idRegistro);
                List<LightUserInfo> utentiLight = BusinessLogic.Mobile.RubricaManager.GetListaUtentiInterni(descrizione.ToUpper(), infoUtente, numMaxResult, idRegistro);
                List<UserInfo> resList = new List<UserInfo>();
                if (utentiLight != null && utentiLight.Count > 0)
                {
                    foreach (LightUserInfo userL in utentiLight)
                    {
                        tempUt = new UserInfo();
                        tempUt.Descrizione = userL.Descrizione;
                        tempUt.IdPeople = userL.IdPeople;
                        tempUt.UserId = userL.UserId;
                        tempUt.IdAmministrazione = infoUtente.idAmministrazione;
                        if (listaIdPref != null && listaIdPref.Count > 0 && listaIdPref.Contains(userL.IdPeople)) tempUt.Preferito = true;
                        else tempUt.Preferito = false;
                        arrayRuoli = new ArrayList();

                        //DocsPaVO.utente.InfoUtente infoUtenteDaCercare = new DocsPaVO.utente.InfoUtente(utenteDaCercare, Utils.GetRuoloPreferito(utenteDaCercare.idPeople));
                        arrayRuoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(userL.IdPeople);
                        if (arrayRuoli != null && arrayRuoli.Count > 0)
                        {
                            ruoliReturn = new List<RuoloInfo>();

                            foreach (DocsPaVO.utente.Ruolo rol in arrayRuoli)
                            {
                                roleinfo = RuoloInfo.buildInstance(rol);
                                ruoliReturn.Add(roleinfo);
                            }
                            tempUt.Ruoli = ruoliReturn;
                        }

                        resList.Add(tempUt);
                    }
                }
                res.Risultati = resList;
                res.Code = RicercaUtentiWithRolesResponseCode.OK;
                logger.Info("ricercaUtenti - end");
                return res;
            }
            catch (Exception e)
            {
                logger.Error("ricercaUtenti - eccezione: " + e);
                return RicercaUtentiWithRolesResponse.ErrorResponse;
            }
        }

        public static async Task<bool> revocaDelega(string token, Delega delega)
        {
            logger.Info("revocaDelega - begin");
            try
            {
                bool res = false;
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuolo(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                // TODO
                DocsPaVO.Deleghe.InfoDelega infoDelega = delega.InfoDelega;

                DocsPaVO.Deleghe.InfoDelega[] inputArray = { infoDelega };
                string msg = "";
                res = BusinessLogic.Deleghe.DelegheManager.revocaDelega(infoUtente, inputArray, out msg);

                logger.Info("revocaDelega - end");


                return res;
            }
            catch (Exception e)
            {
                logger.Error("revocaDelega - eccezione: " + e);
                //return RicercaUtentiResponse.ErrorResponse;
                return false;
            }
        }
        #endregion
        #region Trasmissioni
        public static async Task<AccettaRifiutaTrasmResponse> setDataVistaSP_TV(string token, TrasmVistaRequest request)
        {
            logger.Info("begin MobileManager.setDataVistaSP_TV");
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                bool result = false;
                if (!string.IsNullOrWhiteSpace(request.IdTrasmissione))
                {
                    DocsPaVO.trasmissione.Trasmissione trasm = BusinessLogic.Trasmissioni.TrasmManager.CreateObjTrasmissioneByID(request.IdTrasmissione);

                    if (trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.docNumber))
                    {
                        result = BusinessLogic.Documenti.DocManager.setDataVistaSP_TV(infoUtente, trasm.infoDocumento.docNumber, "D", trasm.infoDocumento.idRegistro); ;
                    }
                    else
                    {
                        result = BusinessLogic.Documenti.DocManager.setDataVistaSP_TV(infoUtente, trasm.infoFascicolo.idFascicolo, "F", string.Empty);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(request.IdEvento))
                {
                    DocsPaDB.Query_DocsPAWS.NotificationDB notification = new NotificationDB();
                    result = notification.CheckNotification(request.IdEvento);
                }
                AccettaRifiutaTrasmResponse resp = new AccettaRifiutaTrasmResponse();
                if (result)
                {
                    resp.Code = AccettaRifiutaTrasmResponseCode.OK;
                }
                else
                {
                    resp.Code = AccettaRifiutaTrasmResponseCode.BL_ERROR;
                }
                logger.Info("end MobileManager.setDataVistaSP_TV");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("eccezione MobileManager.setDataVistaSP_TV: " + e);
                return AccettaRifiutaTrasmResponse.ErrorResponse;
            }
        }

        public static async Task<AccettaRifiutaTrasmResponse> accettaRifiutaTrasm(string token, AccettaRifiutaTrasmRequest request)
        {
            logger.Info("begin MobileManager.accettaRifiutaTrasm");
            try
            {
                const string ACCETTATADOC = "ACCETTATA_DOCUMENTO";
                const string ACCETTATAFASC = "ACCETTATA_FASCICOLO";
                const string RIFIUTATADOC = "RIFIUTATA_DOCUMENTO";
                const string RIFIUTATAFASC = "RIFIUTATA_FASCICOLO";
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                string msg = "";

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                TrasmissioneUtente tu = new TrasmissioneUtente();
                tu.utente = utente;
                if (!string.IsNullOrEmpty(request.IdTrasmissioneUtente))
                {
                    logger.Debug("trasmissione utente con id " + request.IdTrasmissioneUtente);
                    tu.systemId = request.IdTrasmissioneUtente;
                }
                else
                {
                    logger.Debug("ricerca trasmissione utente, idTrasm: " + request.IdTrasmissione);

                    DocsPaVO.trasmissione.Trasmissione trasm = BusinessLogic.Trasmissioni.TrasmManager.CreateObjTrasmissioneByID(request.IdTrasmissione);
                    foreach (TrasmissioneSingola temp in trasm.trasmissioniSingole)
                    {
                        foreach (TrasmissioneUtente tempUt in temp.trasmissioneUtente)
                        {
                            if (tempUt != null && tempUt.utente != null && infoUtente.userId.Equals(tempUt.utente.userId))
                            {
                                if (
                                    (temp.tipoDest == TipoDestinatario.RUOLO && int.Parse(temp.corrispondenteInterno.systemId) - 1 == int.Parse(infoUtente.idGruppo))
                                    || temp.tipoDest != TipoDestinatario.RUOLO
                                )
                                {
                                    tu.systemId = tempUt.systemId;
                                    logger.Debug("idTrasmUtente: " + tu.systemId);

                                    if (!String.IsNullOrEmpty(tempUt.dataAccettata) || !String.IsNullOrEmpty(tempUt.dataRifiutata))
                                    {   //è Già stata accettata o rifiutata.. 
                                        return new AccettaRifiutaTrasmResponse { Code = AccettaRifiutaTrasmResponseCode.BL_ERROR, Errore = "La trasmissione è già stata accettata o rifiutata" };
                                    }
                                }
                            }
                        }
                    }
                }
                logger.Debug("If tipo richiesta");
                if (request.Action.ToUpper() == "ACCETTA")
                {
                    tu.noteAccettazione = request.Note;
                    tu.tipoRisposta = TipoRisposta.ACCETTAZIONE;
                }
                else
                {
                    tu.noteRifiuto = request.Note;
                    tu.tipoRisposta = TipoRisposta.RIFIUTO;
                }
                tu.dataVista = Convert.ToString(DateTime.Now);
                string errore;
                string mode;
                string idObj;
                logger.Debug("Prima di chiamare il metodo");
                bool result = BusinessLogic.Trasmissioni.ExecTrasmManager.executeAccRifMethod(tu, request.IdTrasmissione, ruolo, infoUtente, out errore, out mode, out idObj);
                AccettaRifiutaTrasmResponse resp = new AccettaRifiutaTrasmResponse();
                if (result)
                {
                    resp.Code = AccettaRifiutaTrasmResponseCode.OK;
                    if (result && !string.IsNullOrEmpty(mode))
                    {
                        DocsPaVO.trasmissione.TrasmissioneSingola sing = BusinessLogic.Trasmissioni.ExecTrasmManager.getTrasmSingola(tu);
                        string idRole = sing.tipoDest == DocsPaVO.trasmissione.TipoDestinatario.RUOLO ? ruolo.idGruppo : "0";

                        switch (mode)
                        {
                            case ACCETTATADOC:
                                msg = "Accettazione della trasmissione. Id documento: " + idObj;
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, idRole, infoUtente.idAmministrazione, "ACCEPTTRASMDOCUMENT", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1", sing.systemId);
                                break;
                            case ACCETTATAFASC:
                                msg = "Accettazione della trasmissione. Id fascicolo: " + idObj;
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, idRole, infoUtente.idAmministrazione, "ACCEPTTRASMFOLDER", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1", sing.systemId);
                                break;
                            case RIFIUTATAFASC:
                                msg = "Rifiuto della trasmissione. Id fascicolo: " + idObj;
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, idRole, infoUtente.idAmministrazione, "REJECTTRASMFOLDER", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1", sing.systemId);
                                break;
                            case RIFIUTATADOC:
                                msg = "Rifiuto della trasmissione. Id documento: " + idObj;
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, idRole, infoUtente.idAmministrazione, "REJECTTRASMDOCUMENT", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1", sing.systemId);
                                break;
                        }
                    }
                }
                else
                {
                    resp.Code = AccettaRifiutaTrasmResponseCode.BL_ERROR;
                    resp.Errore = errore;
                }
                logger.Info("end MobileManager.accettaRifiutaTrasm");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("eccezione MobileManager.accettaRifiutaTrasm: " + e);
                return AccettaRifiutaTrasmResponse.ErrorResponse;
            }
        }

        public static async Task<ListaModelliTrasmResponse> getListaModelliTrasm(string token, bool fasc)
        {
            logger.Info("getListaModelliTrasm - begin");
            try
            {
                DocsPaVO.utente.Utente ut = null;
                DocsPaVO.utente.InfoUtente iu = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                ut = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                iu = new DocsPaVO.utente.InfoUtente(ut, ruolo);

                ListaModelliTrasmResponse resp = new ListaModelliTrasmResponse();
                ArrayList listaModelli = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliUtente(ut, iu, null);
                resp.Code = ListaModelliTrasmResponseCode.OK;
                resp.Modelli = new List<ModelloTrasm>();
                string tipo = "D";
                if (fasc) tipo = "F";
                foreach (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione temp in listaModelli)
                {
                    if (tipo.Equals(temp.CHA_TIPO_OGGETTO)) resp.Modelli.Add(ModelloTrasm.buildInstance(temp));
                }
                logger.Info("getListaModelliTrasm - end");
                //PassTrough
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("getListaModelliTrasm - eccezione: " + e);
                return ListaModelliTrasmResponse.ErrorResponse;
            }
        }

        public static async Task<EseguiTrasmResponse> eseguiTrasm(string token, EseguiTrasmRequest request)
        {
            try
            {
                logger.Info("eseguiTrasm - begin");
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                EseguiTrasmResponse response = new EseguiTrasmResponse();
                response.Code = EseguiTrasmResponseCode.OK;
                DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByID(infoUtente.idAmministrazione, request.IdModelloTrasm);
                string id = (!string.IsNullOrEmpty(request.IdDoc)) ? request.IdDoc : request.IdFasc;
                bool isDoc = !string.IsNullOrEmpty(request.IdDoc);
                TrasmBuilder builder = new TrasmBuilder(modello, id, infoUtente, ruolo, isDoc);
                DocsPaVO.trasmissione.Trasmissione trasm = builder.Trasmissione;
                if (!string.IsNullOrEmpty(request.Note))
                {
                    trasm.noteGenerali = request.Note;
                }

                // il path da inserire nella mail dev'essere quello del frontend, non quello del mobile
                // lo recupero dalla chiave di BE URL_PATH_IS
                string path = string.Empty;

                if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                    path = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();
                else
                    path = request.Path;

                logger.Info("path : " + path);
                trasm = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(path, trasm);

                string notify = "1";
                if (trasm.NO_NOTIFY != null && trasm.NO_NOTIFY.Equals("1"))
                {
                    notify = "0";
                }
                else
                {
                    notify = "1";
                }
                if (trasm != null)
                {
                    string desc = string.Empty;
                    // LOG per documento
                    if (trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.idProfile))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasm.trasmissioniSingole)
                        {
                            string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            if (trasm.infoDocumento.segnatura == null)
                                desc = "Trasmesso Documento : " + trasm.infoDocumento.docNumber.ToString();
                            else
                                desc = "Trasmesso Documento : " + trasm.infoDocumento.segnatura.ToString();

                            BusinessLogic.UserLog.UserLog.WriteLog(trasm.utente.userId, trasm.utente.idPeople, trasm.ruolo.idGruppo, trasm.utente.idAmministrazione, method, trasm.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, notify, single.systemId);
                        }
                    }
                }
                //BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(path, trasm);
                //BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(request.Path, trasm);

                logger.Info("eseguiTrasm - end");
                return response;
            }
            catch (Exception e)
            {
                logger.Error("eseguiTrasm - eccezione: " + e);
                return EseguiTrasmResponse.ErrorResponse;
            }
        }

        public static async Task<EseguiTrasmResponse> eseguiTrasmDiretta(string token, EseguiTrasmDirettaRequest request)
        {
            try
            {
                logger.Info("eseguiTrasmDiretta - begin");
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuolo(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                EseguiTrasmResponse response = new EseguiTrasmResponse();
                response.Code = EseguiTrasmResponseCode.OK;
                if (string.IsNullOrEmpty(request.IdDoc) && string.IsNullOrEmpty(request.IdFasc)) throw new Exception();

                DocsPaVO.utente.Corrispondente corr = null;
                DocsPaVO.documento.SchedaDocumento documento = null;
                DocsPaVO.fascicolazione.Fascicolo fascicolo = null;


                if (!string.IsNullOrEmpty(request.IdDestinatario))
                {
                    corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(request.IdDestinatario);
                }
                else if (!string.IsNullOrEmpty(request.CodiceDestinatario))
                {
                    //corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.Receiver.Code, infoUtente);
                    corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.CodiceDestinatario, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                }
                if (corr == null)
                {
                    throw new Exception();
                }
                DocsPaVO.trasmissione.RagioneTrasmissione ragione = null;
                if (string.IsNullOrEmpty(request.Ragione)) throw new Exception();
                ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, request.Ragione.ToUpper());
                if (ragione == null) throw new Exception();

                if (!string.IsNullOrEmpty(request.IdDoc)) documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDoc, request.IdDoc);
                else fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdFasc, infoUtente);
                string trasmType = "S";
                if (!string.IsNullOrEmpty(request.TipoTrasmissione) && request.TipoTrasmissione == "T")
                {
                    trasmType = "T";
                }
                DocsPaVO.trasmissione.Trasmissione transmission = new DocsPaVO.trasmissione.Trasmissione();
                // Creazione dell'oggetto trasmissione
                transmission = new DocsPaVO.trasmissione.Trasmissione();

                // Impostazione dei parametri della trasmissione

                transmission.noteGenerali = request.Note;

                // Tipo oggetto

                if (!string.IsNullOrEmpty(request.IdDoc))
                {
                    transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                    transmission.infoDocumento = BusinessLogic.Documenti.DocManager.getInfoDocumento(documento);

                }
                else
                {
                    transmission.tipoOggetto = TipoOggetto.FASCICOLO;
                    transmission.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(fascicolo);
                }

                // Utente mittente della trasmissione
                transmission.utente = utente;

                // Ruolo mittente
                // Se inserito in ingresso il CodeRole utilizzo il coderole (WIP)
                transmission.ruolo = ruolo;
                string generaNotifica = "1";
                if (request.Notify != null && !request.Notify)
                {
                    transmission.NO_NOTIFY = "1";
                    generaNotifica = "0";
                }
                else
                {
                    transmission.NO_NOTIFY = "0";
                    generaNotifica = "1";
                }
                string urlfrontend = "";
                // Modifica per invio in mail dell'url del frontend
                if (string.IsNullOrWhiteSpace(request.Path))
                {

                    if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                        urlfrontend = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();
                }
                else urlfrontend = request.Path;

                transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(
                    transmission, corr, ragione, "", trasmType);
                // il tipo trasm è S

                DocsPaVO.trasmissione.Trasmissione result = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(urlfrontend, transmission);
                string desc = string.Empty;
                // LOG per documento
                if (result.infoDocumento != null && !string.IsNullOrEmpty(result.infoDocumento.idProfile))
                {
                    foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in result.trasmissioniSingole)
                    {
                        string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                        if (result.infoDocumento.segnatura == null)
                            desc = "Trasmesso Documento : " + result.infoDocumento.docNumber.ToString();
                        else
                            desc = "Trasmesso Documento : " + result.infoDocumento.segnatura.ToString();

                        BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                    }
                }
                // LOG per fascicolo
                if (result.infoFascicolo != null && !string.IsNullOrEmpty(result.infoFascicolo.idFascicolo))
                {
                    foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in result.trasmissioniSingole)
                    {
                        string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                        desc = "Trasmesso Fascicolo ID: " + result.infoFascicolo.idFascicolo.ToString();
                        BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                    }
                }

                logger.Info("eseguiTrasmDiretta - end");
                return response;
            }
            catch (Exception e)
            {
                logger.Error("eseguiTrasmDiretta - eccezione: " + e);
                return EseguiTrasmResponse.ErrorResponse;
            }
        }

        public static async Task<AccettaRifiutaTrasmResponse> smista(string token, SmistaRequest request)
        {
            try
            {
                logger.Info("smista - begin");
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuolo(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                EseguiTrasmResponse response = new EseguiTrasmResponse();
                response.Code = EseguiTrasmResponseCode.OK;
                bool result = false;
                AccettaRifiutaTrasmResponse resp = new AccettaRifiutaTrasmResponse();
                // TODO

                const string ACCETTATADOC = "ACCETTATA_DOCUMENTO";
                const string ACCETTATAFASC = "ACCETTATA_FASCICOLO";
                const string RIFIUTATADOC = "RIFIUTATA_DOCUMENTO";
                const string RIFIUTATAFASC = "RIFIUTATA_FASCICOLO";
                string msg = "";
                string errore;
                string mode;
                string idObj;

                if (request.HasWorkflow)
                {
                    TrasmissioneUtente tu = new TrasmissioneUtente();
                    tu.utente = utente;
                    if (!string.IsNullOrEmpty(request.IdTrasmissioneUtente))
                    {
                        logger.Debug("trasmissione utente con id " + request.IdTrasmissioneUtente);
                        tu.systemId = request.IdTrasmissioneUtente;
                    }
                    else
                    {
                        logger.Debug("ricerca trasmissione utente, idTrasm: " + request.IdTrasmissione);

                        DocsPaVO.trasmissione.Trasmissione trasm = BusinessLogic.Trasmissioni.TrasmManager.CreateObjTrasmissioneByID(request.IdTrasmissione);
                        foreach (TrasmissioneSingola temp in trasm.trasmissioniSingole)
                        {
                            foreach (TrasmissioneUtente tempUt in temp.trasmissioneUtente)
                            {
                                if (tempUt != null && tempUt.utente != null && infoUtente.userId.Equals(tempUt.utente.userId))
                                {
                                    if (
                                        (temp.tipoDest == TipoDestinatario.RUOLO && int.Parse(temp.corrispondenteInterno.systemId) - 1 == int.Parse(infoUtente.idGruppo))
                                        || temp.tipoDest != TipoDestinatario.RUOLO
                                    )
                                    {
                                        tu.systemId = tempUt.systemId;
                                        logger.Debug("idTrasmUtente: " + tu.systemId);

                                        if (!String.IsNullOrEmpty(tempUt.dataAccettata) || !String.IsNullOrEmpty(tempUt.dataRifiutata))
                                        {   //è Già stata accettata o rifiutata.. 
                                            return new AccettaRifiutaTrasmResponse { Code = AccettaRifiutaTrasmResponseCode.BL_ERROR, Errore = "La trasmissione è già stata accettata o rifiutata" };
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // lo smistamento accetta sempre e non inserisce note di accettazione.
                    //if (request.Action.ToUpper() == "ACCETTA")
                    //{
                    //tu.noteAccettazione = request.NoteAccettazione;
                    tu.tipoRisposta = TipoRisposta.ACCETTAZIONE;
                    //}
                    //else
                    //{
                    //    tu.noteRifiuto = request.NoteAccettazione;
                    //    tu.tipoRisposta = TipoRisposta.RIFIUTO;
                    //}
                    tu.dataVista = Convert.ToString(DateTime.Now);
                    
                    logger.Debug("Prima di chiamare il metodo");
                    result = BusinessLogic.Trasmissioni.ExecTrasmManager.executeAccRifMethod(tu, request.IdTrasmissione, ruolo, infoUtente, out errore, out mode, out idObj);
                    
                
                if (result)
                {
                    resp.Code = AccettaRifiutaTrasmResponseCode.OK;
                    if (result && !string.IsNullOrEmpty(mode))
                    {
                        DocsPaVO.trasmissione.TrasmissioneSingola sing = BusinessLogic.Trasmissioni.ExecTrasmManager.getTrasmSingola(tu);
                        string idRole = sing.tipoDest == DocsPaVO.trasmissione.TipoDestinatario.RUOLO ? ruolo.idGruppo : "0";

                        switch (mode)
                        {
                            case ACCETTATADOC:
                                msg = "Accettazione della trasmissione. Id documento: " + idObj;
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, idRole, infoUtente.idAmministrazione, "ACCEPTTRASMDOCUMENT", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1", sing.systemId);
                                break;
                            case ACCETTATAFASC:
                                msg = "Accettazione della trasmissione. Id fascicolo: " + idObj;
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, idRole, infoUtente.idAmministrazione, "ACCEPTTRASMFOLDER", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1", sing.systemId);
                                break;
                            case RIFIUTATAFASC:
                                msg = "Rifiuto della trasmissione. Id fascicolo: " + idObj;
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, idRole, infoUtente.idAmministrazione, "REJECTTRASMFOLDER", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1", sing.systemId);
                                break;
                            case RIFIUTATADOC:
                                msg = "Rifiuto della trasmissione. Id documento: " + idObj;
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, idRole, infoUtente.idAmministrazione, "REJECTTRASMDOCUMENT", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1", sing.systemId);
                                break;
                        }
                    }
                }
                else
                {
                    resp.Code = AccettaRifiutaTrasmResponseCode.BL_ERROR;
                    resp.Errore = errore;
                }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(request.IdTrasmissione))
                    {
                        DocsPaVO.trasmissione.Trasmissione trasm = BusinessLogic.Trasmissioni.TrasmManager.CreateObjTrasmissioneByID(request.IdTrasmissione);

                        if (trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.docNumber))
                        {
                            result = BusinessLogic.Documenti.DocManager.setDataVistaSP_TV(infoUtente, trasm.infoDocumento.docNumber, "D", trasm.infoDocumento.idRegistro); ;
                        }
                        else
                        {
                            result = BusinessLogic.Documenti.DocManager.setDataVistaSP_TV(infoUtente, trasm.infoFascicolo.idFascicolo, "F", string.Empty);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(request.IdEvento))
                    {
                        DocsPaDB.Query_DocsPAWS.NotificationDB notification = new NotificationDB();
                        result = notification.CheckNotification(request.IdEvento);
                    }
                    if (!result)
                    {
                        
                    resp.Code = AccettaRifiutaTrasmResponseCode.BL_ERROR;
                    resp.Errore = "Errore nella procedura di visto";
                
                    }
                }
                if (result)
                {
                    if (!string.IsNullOrEmpty(request.IdModelloTrasm))
                    {
                        DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByID(infoUtente.idAmministrazione, request.IdModelloTrasm);
                        string id = (!string.IsNullOrEmpty(request.IdDoc)) ? request.IdDoc : request.IdFasc;
                        bool isDoc = !string.IsNullOrEmpty(request.IdDoc);
                        TrasmBuilder builder = new TrasmBuilder(modello, id, infoUtente, ruolo, isDoc);
                        DocsPaVO.trasmissione.Trasmissione trasm = builder.Trasmissione;
                        if (!string.IsNullOrEmpty(request.NoteTrasm))
                        {
                            trasm.noteGenerali = request.NoteTrasm;
                        }

                        // il path da inserire nella mail dev'essere quello del frontend, non quello del mobile
                        // lo recupero dalla chiave di BE URL_PATH_IS
                        string path = string.Empty;

                        if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                            path = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();
                        else
                            path = request.Path;

                        logger.Info("path : " + path);
                        trasm = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(path, trasm);

                        string notify = "1";
                        if (trasm.NO_NOTIFY != null && trasm.NO_NOTIFY.Equals("1"))
                        {
                            notify = "0";
                        }
                        else
                        {
                            notify = "1";
                        }
                        if (trasm != null)
                        {
                            string desc = string.Empty;
                            // LOG per documento
                            if (trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.idProfile))
                            {
                                foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasm.trasmissioniSingole)
                                {
                                    string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                    if (trasm.infoDocumento.segnatura == null)
                                        desc = "Trasmesso Documento : " + trasm.infoDocumento.docNumber.ToString();
                                    else
                                        desc = "Trasmesso Documento : " + trasm.infoDocumento.segnatura.ToString();

                                    BusinessLogic.UserLog.UserLog.WriteLog(trasm.utente.userId, trasm.utente.idPeople, trasm.ruolo.idGruppo, trasm.utente.idAmministrazione, method, trasm.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, notify, single.systemId);
                                }
                            }
                            if (trasm.infoFascicolo != null && !string.IsNullOrEmpty(trasm.infoFascicolo.idFascicolo))
                            {
                                foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasm.trasmissioniSingole)
                                {
                                    string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                    desc = "Trasmesso Fascicolo ID: " + trasm.infoFascicolo.idFascicolo.ToString();
                                    BusinessLogic.UserLog.UserLog.WriteLog(trasm.utente.userId, trasm.utente.idPeople, trasm.ruolo.idGruppo, trasm.utente.idAmministrazione, method, trasm.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, notify, single.systemId);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(request.IdDoc) && string.IsNullOrEmpty(request.IdFasc)) throw new Exception();

                        DocsPaVO.utente.Corrispondente corr = null;
                        DocsPaVO.documento.SchedaDocumento documento = null;
                        DocsPaVO.fascicolazione.Fascicolo fascicolo = null;


                        if (!string.IsNullOrEmpty(request.IdDestinatario))
                        {
                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(request.IdDestinatario);
                        }
                        else if (!string.IsNullOrEmpty(request.CodiceDestinatario))
                        {
                            //corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.Receiver.Code, infoUtente);
                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.CodiceDestinatario, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                        }
                        if (corr == null)
                        {
                            throw new Exception();
                        }
                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = null;
                        if (string.IsNullOrEmpty(request.Ragione)) throw new Exception();
                        ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, request.Ragione.ToUpper());
                        if (ragione == null) throw new Exception();

                        if (!string.IsNullOrEmpty(request.IdDoc)) documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDoc, request.IdDoc);
                        else fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdFasc, infoUtente);
                        string trasmType = "S";
                        if (!string.IsNullOrEmpty(request.TipoTrasmissione) && request.TipoTrasmissione == "T")
                        {
                            trasmType = "T";
                        }
                        DocsPaVO.trasmissione.Trasmissione transmission = new DocsPaVO.trasmissione.Trasmissione();
                        // Creazione dell'oggetto trasmissione
                        transmission = new DocsPaVO.trasmissione.Trasmissione();

                        // Impostazione dei parametri della trasmissione

                        transmission.noteGenerali = request.NoteTrasm;

                        // Tipo oggetto

                        if (!string.IsNullOrEmpty(request.IdDoc))
                        {
                            transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                            transmission.infoDocumento = BusinessLogic.Documenti.DocManager.getInfoDocumento(documento);

                        }
                        else
                        {
                            transmission.tipoOggetto = TipoOggetto.FASCICOLO;
                            transmission.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(fascicolo);
                        }

                        // Utente mittente della trasmissione
                        transmission.utente = utente;

                        // Ruolo mittente
                        // Se inserito in ingresso il CodeRole utilizzo il coderole (WIP)
                        transmission.ruolo = ruolo;
                        string generaNotifica = "1";
                        if (request.Notify != null && !request.Notify)
                        {
                            transmission.NO_NOTIFY = "1";
                            generaNotifica = "0";
                        }
                        else
                        {
                            transmission.NO_NOTIFY = "0";
                            generaNotifica = "1";
                        }
                        string urlfrontend = "";
                        // Modifica per invio in mail dell'url del frontend
                        if (string.IsNullOrWhiteSpace(request.Path))
                        {

                            if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                                urlfrontend = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();
                        }
                        else urlfrontend = request.Path;

                        transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(
                            transmission, corr, ragione, "", trasmType);
                        // il tipo trasm è S

                        DocsPaVO.trasmissione.Trasmissione resultTrasmDiretta = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(urlfrontend, transmission);
                        string desc = string.Empty;
                        // LOG per documento
                        if (resultTrasmDiretta.infoDocumento != null && !string.IsNullOrEmpty(resultTrasmDiretta.infoDocumento.idProfile))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in resultTrasmDiretta.trasmissioniSingole)
                            {
                                string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                if (resultTrasmDiretta.infoDocumento.segnatura == null)
                                    desc = "Trasmesso Documento : " + resultTrasmDiretta.infoDocumento.docNumber.ToString();
                                else
                                    desc = "Trasmesso Documento : " + resultTrasmDiretta.infoDocumento.segnatura.ToString();

                                BusinessLogic.UserLog.UserLog.WriteLog(resultTrasmDiretta.utente.userId, resultTrasmDiretta.utente.idPeople, resultTrasmDiretta.ruolo.idGruppo, resultTrasmDiretta.utente.idAmministrazione, method, resultTrasmDiretta.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                            }
                        }
                        // LOG per fascicolo
                        if (resultTrasmDiretta.infoFascicolo != null && !string.IsNullOrEmpty(resultTrasmDiretta.infoFascicolo.idFascicolo))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in resultTrasmDiretta.trasmissioniSingole)
                            {
                                string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                desc = "Trasmesso Fascicolo ID: " + resultTrasmDiretta.infoFascicolo.idFascicolo.ToString();
                                BusinessLogic.UserLog.UserLog.WriteLog(resultTrasmDiretta.utente.userId, resultTrasmDiretta.utente.idPeople, resultTrasmDiretta.ruolo.idGruppo, resultTrasmDiretta.utente.idAmministrazione, method, resultTrasmDiretta.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                            }
                        }

                    }
                }
                


                logger.Info("smista - end");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("smista - eccezione: " + e);
                return AccettaRifiutaTrasmResponse.ErrorResponse;
            }
        }

        public static async Task<RicercaSmistamentoResponse> GetListaCorrispondentiVeloce(string token, GetListaCorrVeloceRequest request)
        {
            try
            {
                logger.Info("GetListaCorrispondentiVeloce - begin");
                logger.Info("descrizione: " + request.Descrizione);
                logger.Info("ragione: " + request.Ragione);
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuolo(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                RicercaSmistamentoResponse res = new RicercaSmistamentoResponse();


                //RuoloInfo roleinfo = RuoloInfo.buildInstance(ruolo);
                //List<String> idRegistri = (from a in roleinfo.Registri select a.SystemId).ToList();
                //String idRegistro = idRegistri.FirstOrDefault();

                string IdRuolo = ruolo.systemId;
                string idRegistro = ruolo.idRegistro;
                List<RicercaSmistamentoElement> elements = BusinessLogic.Mobile.RubricaManager.GetListaCorrispondentiVeloce(request.Descrizione, infoUtente, idRegistro, IdRuolo, request.Ragione, 10000);
                res.Elements = elements;
                res.Code = RicercaSmistamentoResponseCode.OK;

                return res;
            }
            catch (Exception ex)
            {
                logger.Error("GetListaCorrispondentiVeloce - Exception: " + ex);
                return RicercaSmistamentoResponse.ErrorResponse;
            }

        }

        public static async Task<ListaRuoliByUserResponse> getListaRuoliUser(string token, string idUtenteDaCercare)
        {
            try
            {
                ListaRuoliByUserResponse resp = null;

                logger.Info("getListaRuoliUser - begin");

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                RuoloInfo roleinfo = null;
                RuoloInfo[] ruoliReturn = null;
                //DocsPaVO.utente.Utente utenteDaCercare = null;
                ArrayList arrayRuoli = new ArrayList();

                //DocsPaVO.utente.InfoUtente infoUtenteDaCercare = new DocsPaVO.utente.InfoUtente(utenteDaCercare, Utils.GetRuoloPreferito(utenteDaCercare.idPeople));
                arrayRuoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(idUtenteDaCercare);
                if (arrayRuoli != null && arrayRuoli.Count > 0)
                {
                    ruoliReturn = new RuoloInfo[arrayRuoli.Count];
                    int i = 0;
                    foreach (DocsPaVO.utente.Ruolo rol in arrayRuoli)
                    {
                        roleinfo = RuoloInfo.buildInstance(rol);
                        ruoliReturn[i] = roleinfo;
                        i++;
                    }
                    resp = new ListaRuoliByUserResponse(ListaRuoliByUserResponseCode.OK);

                    resp.ListaRuoli = ruoliReturn;
                }
                else
                    resp = new ListaRuoliByUserResponse(ListaRuoliByUserResponseCode.USER_NO_ROLES);


                logger.Info("getListaRuoliUser - end");
                return resp;
            }
            catch (Exception e)
            {
                logger.Error("getListaRuoliUser - eccezione: " + e);
                return new ListaRuoliByUserResponse(ListaRuoliByUserResponseCode.SYSTEM_ERROR);
            }
        }

        public static async Task<bool> PrefMobile_insert(string token, InfoPreferito infoPref)
        {
            try
            {
                logger.Info("hsmSign - begin");

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                bool res = false;
                infoPref.IdPeopleOwner = infoUtente.idPeople;
                res = BusinessLogic.Mobile.RubricaManager.PrefMobile_Insert(infoPref);

                return res;
            }
            catch (Exception e)
            {
                logger.Error("hsmSign - Exception: " + e);
                return false;
            }
        }

        public static async Task<bool> PrefMobile_delete(string token, InfoPreferito infoPref)
        {
            try
            {
                logger.Info("hsmSign - begin");

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                bool res = false;
                infoPref.IdPeopleOwner = infoUtente.idPeople;
                res = BusinessLogic.Mobile.RubricaManager.PrefMobile_Delete(infoPref);

                return res;
            }
            catch (Exception e)
            {
                logger.Error("hsmSign - Exception: " + e);
                return false;
            }
        }

        public static async Task<ListaPreferitiResponse> PrefMobile_getList(string token, string soloPers, string tipoPref)
        {
            try
            {
                logger.Info("hsmSign - begin");

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                bool soloP = false;
                bool.TryParse(soloPers, out soloP);

                ListaPreferitiResponse response = new ListaPreferitiResponse();
                List<InfoPreferito> preferiti = BusinessLogic.Mobile.RubricaManager.PrefMobile_getList(infoUtente.idPeople, soloP, tipoPref);
                response.Preferiti = preferiti;
                response.Code = ListaPreferitiResponseCode.OK;
                return response;
            }
            catch (Exception e)
            {
                logger.Error("hsmSign - Exception: " + e);
                return ListaPreferitiResponse.ErrorResponse;
            }
        }

        public static async Task<ListaRagioniResponse> getListaRagioni(string token, string idObject, string DoF)
        {
            ListaRagioniResponse response = new ListaRagioniResponse();
            try
            {
                logger.Info("getListaRagioni - begin");

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                DocsPaVO.trasmissione.Diritti diritti = new Diritti();
                if (!string.IsNullOrEmpty(DoF) && DoF.ToUpper() == "F")
                {
                    Fascicolo fasc = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(idObject, infoUtente);
                    diritti.accessRights = fasc.accessRights;
                }
                else
                {
                    DocsPaVO.documento.SchedaDocumento doc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idObject, idObject);
                    diritti.accessRights = doc.accessRights;
                }
                diritti.idAmministrazione = infoUtente.idAmministrazione;
                ArrayList ragioni = BusinessLogic.Trasmissioni.RagioniManager.getListaRagioni(diritti, false);
                List<DocsPaVO.trasmissione.RagioneTrasmissione> ragioniList = ragioni.Cast<DocsPaVO.trasmissione.RagioneTrasmissione>().ToList<DocsPaVO.trasmissione.RagioneTrasmissione>();
                response.Ragioni = ragioniList;
                return response;
            }
            catch (Exception e)
            {
                logger.Error("getListaRagioni - Exception: " + e);
                return ListaRagioniResponse.ErrorResponse;
            }
        }
        #endregion
        #region HSMSign
        public static async Task<HSMSignResponse> hsmSign(string token, HSMSignRequest request)
        {
            try
            {
                logger.Info("hsmSign - begin");

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                HSMSignResponse res = new HSMSignResponse();
                DocsPaVO.documento.SchedaDocumento sd = null;
                FileRequest fr = null;
                // recupera il file request del doc
                sd = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDoc, request.IdDoc);
                fr = (FileRequest)sd.documenti[0];
                res.Code = ((BusinessLogic.Documenti.FileManager.HSM_Sign(infoUtente,
                                                                         fr,
                                                                         request.cofirma,
                                                                         request.timestamp,
                                                                         request.TipoFirma,
                                                                         request.AliasCertificato,
                                                                         request.DominioCertificato,
                                                                         request.OtpFirma,
                                                                         request.PinCertificato,
                                                                         request.ConvertPdf)
                                                                          ) ?
                                                                        HSMSignResponseCode.OK :
                                                                        HSMSignResponseCode.KO);
                logger.Info("hsmSign - end: " + res.Code.ToString());
                return res;
            }
            catch (Exception e)
            {
                logger.Error("hsmSign - Exception: " + e);
                return HSMSignResponse.ErrorResponse;
            }
        }

        public static async Task<HSMSignResponse> hsmRequestOTP(string token, HSMSignRequest request)
        {
            try
            {
                logger.Info("hsmRequestOTP - begin");
                HSMSignResponse res = new HSMSignResponse();

                //aggiorna memento se diverso da quello in db
                //inizio
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                Memento infoMemento = new Memento();
                // recupera le info del memento
                string[] infoMementoResult = BusinessLogic.Utenti.UserManager.getMementoUtente(infoUtente);
                if (infoMementoResult.Length == 2)
                {
                    infoMemento.Dominio = infoMementoResult[0].ToUpper();
                    infoMemento.Alias = infoMementoResult[1].ToUpper();
                    if ((!infoMemento.Dominio.ToUpper().Equals(request.DominioCertificato) ||
                        (!infoMemento.Alias.ToUpper().Equals(request.AliasCertificato))))
                    {
                        BusinessLogic.Utenti.UserManager.setMementoUtente(infoUtente, request.DominioCertificato.ToUpper(), request.AliasCertificato.ToUpper());
                    }
                }
                if (infoMementoResult.Length == 1) //nessun memento definito
                {
                    //update campo memento
                    BusinessLogic.Utenti.UserManager.setMementoUtente(infoUtente, request.DominioCertificato.ToUpper(), request.AliasCertificato.ToUpper());
                }

                //fine

                res.Code = (BusinessLogic.Documenti.FileManager.HSM_RequestOTP(request.AliasCertificato, request.DominioCertificato)) ?
                                                                        HSMSignResponseCode.OK :
                                                                        HSMSignResponseCode.SYSTEM_ERROR;
                logger.Info("hsmRequestOTP - end: " + res.Code.ToString());
                return res;

            }
            catch (Exception e)
            {
                logger.Error("hsmRequestOTP - Exception: " + e);
                return HSMSignResponse.ErrorResponse;
            }

        }

        public static async Task<HSMSignResponse> hsmInfoSign(string token, HSMSignRequest request)
        {
            try
            {
                logger.Info("hsmInfoSign - begin");
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                HSMSignResponse res = new HSMSignResponse();

                InfoDocFirmato rootInfoDoc = new InfoDocFirmato();
                List<NodoFirma> listaNodoFirme = new List<NodoFirma>();

                DocsPaVO.documento.SchedaDocumento sd = null;

                FileRequest fr = null;
                // recupera il file request del doc
                sd = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDoc, request.IdDoc);
                fr = (FileRequest)sd.documenti[0];

                // Info documento 
                DocsPaVO.documento.FileDocumento signedDocument = BusinessLogic.Documenti.FileManager.getFile(fr, infoUtente);
                if (signedDocument != null && signedDocument.signatureResult != null)
                {
                    //Info Data Validità 
                    DateTime dtValiditaDocumento = GetDataRiferimentoValiditaDocumento(fr, infoUtente);
                    string dataValiditaDocumento = dtValiditaDocumento.ToString();
                    //Info verifica CRL 
                    string resultValiditaCRL = VerificaValiditaFirmaAllaData(fr, infoUtente, signedDocument, dtValiditaDocumento).ToString();

                    //info generali documento firmato
                    int documentIndex = 0;
                    bool isFirstItem = true;
                    //aggiunta nodo firme root 
                    NodoFirma rootFirma = new NodoFirma
                    {
                        Value = "",
                        Text = "",
                        DetailFirma = new InfoDetailFirma(),
                        ChildNodiFirma = new List<NodoFirma>()
                    };

                    List<NodoFirma> listPrec = new List<NodoFirma>();
                    foreach (PKCS7Document document in signedDocument.signatureResult.PKCS7Documents)
                    {
                        NodoFirma nodoFirma = GetNodesFirmeDigitali(document, documentIndex, null, signedDocument.signatureResult, dataValiditaDocumento, resultValiditaCRL);
                        nodoFirma.DetailFirma = GetInfoDettaglioFirma(document);
                        //  nodoFirma.ChildNodiFirma = new List<NodoFirma>();
                        if (isFirstItem)
                        {
                            rootFirma.ChildNodiFirma.Add(nodoFirma);
                            isFirstItem = false;
                        }
                        else
                        {
                            listPrec.Add(nodoFirma);
                        }
                        listPrec = nodoFirma.ChildNodiFirma;
                        documentIndex++;
                    }
                    listaNodoFirme.Add(rootFirma);
                    rootInfoDoc.Firme = listaNodoFirme;
                    res.infoFirma = rootInfoDoc;
                    res.Code = HSMSignResponseCode.OK;
                }
                else
                {
                    res.Code = HSMSignResponseCode.KO;
                    throw new Exception("Il Documento non risulta firmato.");
                }
                logger.Info("hsmInfoSign - end: " + res.Code.ToString());
                return res;
            }
            catch (Exception e)
            {
                logger.Error("hsmInfoSign - Exception: " + e);
                return HSMSignResponse.ErrorResponse;
            }
        }
        private static NodoFirma GetNodesFirmeDigitali(PKCS7Document document, int documentIndex, string signerLevel, VerifySignatureResult signatureResult, string dataValiditaDocumento, string resultValiditaCRL)
        {
            NodoFirma nodoFirme = new NodoFirma();
            nodoFirme.Text = "Firme" + " (" + document.SignersInfo.Length + ")";
            nodoFirme.Value = string.Empty;
            List<NodoFirma> lstNodoFirmeChild = new List<NodoFirma>();
            int index = 0;
            foreach (SignerInfo info in document.SignersInfo)
            {
                NodoFirma signNode = new NodoFirma();
                List<NodoFirma> lstSignNodeChild = new List<NodoFirma>();

                signNode.Value = "sign&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;
                signNode.Text = (info.SubjectInfo.Cognome + " " + info.SubjectInfo.Nome + " " + document.SignatureType.ToString()).Trim();
                signNode.DetailFirma = GetInfoDettaglioFirma(info, document, signatureResult, dataValiditaDocumento, resultValiditaCRL);
                if (info.SignatureTimeStampInfo != null)
                {
                    foreach (TSInfo ts in info.SignatureTimeStampInfo)
                    {
                        NodoFirma tsNode = new NodoFirma();
                        tsNode.Value = "timestamp&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;
                        string tsText = string.Empty;
                        tsNode.Text = tsText;
                        tsNode.DetailFirma = GetInfoDettaglioFirma(ts);
                        lstSignNodeChild.Add(tsNode);
                    }
                    signNode.ChildNodiFirma = lstSignNodeChild;
                }

                if (info.counterSignatures != null)
                {
                    int signLevels = 0;
                    foreach (SignerInfo csigner in info.counterSignatures)
                    {
                        List<SignerInfo> tmpLst = new List<SignerInfo>();
                        tmpLst.Add(csigner);
                        signerLevel = ":" + signLevels.ToString();

                        lstSignNodeChild.Add(GetNodesFirmeDigitali(tmpLst.ToArray(), documentIndex, signerLevel, document, signatureResult, dataValiditaDocumento, resultValiditaCRL));
                        signLevels++;
                    }
                    signNode.ChildNodiFirma = lstSignNodeChild;
                }
                lstNodoFirmeChild.Add(signNode);
                index++;
            }
            nodoFirme.ChildNodiFirma = lstNodoFirmeChild;

            return nodoFirme;
        }
        private static NodoFirma GetNodesFirmeDigitali(SignerInfo[] signersInfo, int documentIndex, string signerLevel, PKCS7Document document, VerifySignatureResult signatureResult, string dataValiditaDocumento, string resultValiditaCRL)
        {
            NodoFirma rootNode = new NodoFirma();
            List<NodoFirma> lstNodoFirmeChild = new List<NodoFirma>();
            rootNode.Text = "Firme" + " (" + signersInfo.Length + ")";
            int index = 0;

            foreach (SignerInfo info in signersInfo)
            {
                NodoFirma signNode = new NodoFirma();
                List<NodoFirma> lstSignNodeChild = new List<NodoFirma>();
                signNode.Value = "sign&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;

                string nodeText = info.SubjectInfo.Cognome + " " + info.SubjectInfo.Nome;
                if (nodeText.Trim() == string.Empty) nodeText = "Non Presente";
                signNode.Text = nodeText;
                signNode.DetailFirma = GetInfoDettaglioFirma(info, document, signatureResult, dataValiditaDocumento, resultValiditaCRL);

                if (info.SignatureTimeStampInfo != null)
                {
                    foreach (TSInfo ts in info.SignatureTimeStampInfo)
                    {
                        NodoFirma tsNode = new NodoFirma();
                        tsNode.Value = "timestamp&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;
                        string tsText = "Marca Temporale";
                        tsNode.Text = tsText;
                        tsNode.DetailFirma = GetInfoDettaglioFirma(ts);
                        lstSignNodeChild.Add(tsNode);
                    }
                    signNode.ChildNodiFirma = lstSignNodeChild;
                }

                if (info.counterSignatures != null)
                {
                    int signLevels = 0;
                    foreach (SignerInfo csigner in info.counterSignatures)
                    {
                        List<SignerInfo> tmpLst = new List<SignerInfo>();
                        tmpLst.Add(csigner);
                        signerLevel = ":" + signLevels.ToString();
                        lstSignNodeChild.Add(GetNodesFirmeDigitali(tmpLst.ToArray(), documentIndex, signerLevel, document, signatureResult, dataValiditaDocumento, resultValiditaCRL));
                        signLevels++;
                    }
                    signNode.ChildNodiFirma = lstSignNodeChild;
                }

                lstNodoFirmeChild.Add(signNode);
                signNode = null;

                index++;
            }
            rootNode.ChildNodiFirma = lstNodoFirmeChild;
            return rootNode;
        }
        private static InfoDetailFirma GetInfoDettaglioFirma(PKCS7Document document)
        {
            InfoDetailFirma infoDetailFirma = new InfoDetailFirma();

            //Nome File
            infoDetailFirma.NomeFile = document.DocumentFileName;
            if (!document.SignAlgorithm.Contains("PADES"))
            {
                infoDetailFirma.NomeFile = string.Format("{0}.P7M (Nome file P7M)", infoDetailFirma.NomeFile);
            }
            //Livello
            infoDetailFirma.Livello = document.Level.ToString();
            //Algoritmo di Firma documento
            infoDetailFirma.InfoFirmaAlgoritmo = document.SignAlgorithm;

            return infoDetailFirma;
        }
        private static InfoDetailFirma GetInfoDettaglioFirma(SignerInfo info, PKCS7Document document, VerifySignatureResult signatureResult, string dataValiditaDocumento, string resultValiditaCRL)
        {
            InfoDetailFirma infoDetailFirma = new InfoDetailFirma();

            #region Dati Controllo

            //////Stato della firma
            ////if (signatureResult.StatusCode == 0)
            ////    infoDetailFirma.VerificaStatoFirma = "Valido";
            ////else if (signatureResult.StatusCode == -5)
            ////    infoDetailFirma.VerificaStatoFirma = "Non Conforme CADES (idaasigningcertificateV2 non trovato nella firma)";
            ////else
            ////    infoDetailFirma.VerificaStatoFirma = "Invalido";

            ////if (signatureResult.ErrorMessages != null)
            ////    foreach (string msg in signatureResult.ErrorMessages)
            ////        infoDetailFirma.VerificaStatoFirma += msg + "<br/>";

            string VerifiCRLResult = VerifyCRLReturnValue(info.CertificateInfo, dataValiditaDocumento);
            switch (VerifiCRLResult)
            {
                case "VerifyCRLNotValid":
                    infoDetailFirma.VerificaStatoFirma = "Non Valido";
                    break;
                case "VerifyCRLValid":
                    infoDetailFirma.VerificaStatoFirma = "Valido";
                    break;
                case "VerifyCRLExpired":
                    infoDetailFirma.VerificaStatoFirma = "Verifica firma scaduta";
                    break;
                case "VerifyCRLRevoked":
                    infoDetailFirma.VerificaStatoFirma = "Verifica CRL revocata";
                    break;
            }

            //Stato del certificato
            infoDetailFirma.VerificaStatoCertificato = infoDetailFirma.VerificaStatoFirma; // info.CertificateInfo.RevocationStatusDescription;

            //Data Verifica Validità
            infoDetailFirma.VerificaDataValiditaDocumento = dataValiditaDocumento;
            //Verifica CRL
            infoDetailFirma.VerificaCRL = resultValiditaCRL;

            #endregion

            #region  Dati certificato

            CertificateInfo certInfo = info.CertificateInfo;
            //Ente Certificatore
            infoDetailFirma.CertificatoEnte = getEnteCertificatore(certInfo.IssuerName);
            //SN Certificato
            infoDetailFirma.CertificatoSN = certInfo.SerialNumber;
            //Data Validità
            infoDetailFirma.CertificatoValidoDal = certInfo.ValidFromDate.ToLongDateString();
            infoDetailFirma.CertificatoValidoAl = certInfo.ValidToDate.ToLongDateString();
            //Algoritmo Firma certificato
            infoDetailFirma.CertificatoAlgoritmo = certInfo.SignatureAlgorithm;
            //Firmatario
            infoDetailFirma.CertificatoFirmatario = string.Format("{0} {1}", info.SubjectInfo.Cognome, info.SubjectInfo.Nome);
            // Thumbprint certificato
            infoDetailFirma.CertificatoThumbprint = certInfo.ThumbPrint;

            #endregion

            #region Dati soggetto

            //Nome
            infoDetailFirma.SoggettoNome = info.SubjectInfo.Nome;
            //Cognome
            infoDetailFirma.SoggettoCognome = info.SubjectInfo.Cognome;
            //CodiceFiscale
            infoDetailFirma.SoggettoCodiceFiscale = info.SubjectInfo.CodiceFiscale;
            //Data Di Nascita
            infoDetailFirma.SoggettoDataNascita = info.SubjectInfo.DataDiNascita;
            //Organizzazione
            infoDetailFirma.SoggettoOrganizzazione = info.SubjectInfo.Organizzazione;
            //Ruolo
            infoDetailFirma.SoggettoRuolo = info.SubjectInfo.Ruolo;
            //Paese
            infoDetailFirma.SoggettoPaese = info.SubjectInfo.Country;
            //Id Titolare
            infoDetailFirma.SoggettoIdTitolare = info.SubjectInfo.CertId;

            #endregion

            #region Dati relativi all'algoritmo e firma digitale
            //Algoritmo di firma
            infoDetailFirma.InfoFirmaAlgoritmo = info.SignatureAlgorithm;
            //Impronta
            infoDetailFirma.InfoFirmaImpronta = document.SignHash;
            //Controfirmatario
            infoDetailFirma.InfoFirmaControfirmatario = info.isCountersigner ? "Vero" : "Falso";
            //Data di Firma
            if (info.SigningTime != DateTime.MinValue)
            {
                infoDetailFirma.InfoFirmaData = info.SigningTime.ToLocalTime().ToString();
            }

            #endregion

            return infoDetailFirma;
        }

        private static string VerifyCRLReturnValue(CertificateInfo certInfo, string dateVerificaCRL)
        {
            if ((certInfo.RevocationDate != null) && (certInfo.ValidToDate != null))
            {
                string label = string.Empty;
                DateTime date = Convert.ToDateTime(dateVerificaCRL);
                //SE LA DATA INSERITA è PRECEDENTE ALLA DATA DI INIZIO VALIDITà
                if (!verificaIntervalloDate(date.ToString(), certInfo.ValidFromDate.ToString()))
                {
                    label = "VerifyCRLNotValid";
                }
                else
                {
                    //SE IL CERTIFICATO HA UNA DATA DI REVOCA
                    if (!certInfo.RevocationDate.Equals(DateTime.MinValue))
                    {
                        if (verificaIntervalloDate(certInfo.RevocationDate.ToString(), date.ToString()))
                        {
                            label = verificaIntervalloDate(certInfo.ValidToDate.ToString(), date.ToString()) ? "VerifyCRLValid" : "VerifyCRLExpired";
                        }
                        else
                        {
                            label = verificaIntervalloDate(certInfo.ValidToDate.ToString(), date.ToString()) ? "VerifyCRLRevoked" : "VerifyCRLExpired";
                        }
                    }
                    else
                    {
                        label = verificaIntervalloDate(certInfo.ValidToDate.ToString(), date.ToString()) ? "VerifyCRLValid" : "VerifyCRLExpired";
                    }
                }
                return label;
            }
            else
            {
                return "VerifyCLRko";
            }
        }

        public static bool verificaIntervalloDate(string dataUno, string dataDue)
        {
            if (dataUno != "" && dataDue != "")
            {
                try
                {
                    dataUno = dataUno.Trim();
                    dataDue = dataDue.Trim();
                    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("it-IT");
                    string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy" };
                    DateTime d_Uno = DateTime.ParseExact(dataUno, formati, ci.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
                    DateTime d_Due = DateTime.ParseExact(dataDue, formati, ci.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces);

                    if (d_Uno >= d_Due)
                        return true;
                    else
                        return false;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            else
            {
                return false;
            }
        }
        private static InfoDetailFirma GetInfoDettaglioFirma(TSInfo tsfInfo)
        {
            InfoDetailFirma infoDetailFirma = new InfoDetailFirma();
            infoDetailFirma.TimeStampInfo = tsfInfo;
            return infoDetailFirma;

        }
        private static string getEnteCertificatore(string issuerName)
        {
            string enteCertCN = string.Empty;
            string enteCertO = string.Empty;
            string enteCertC = string.Empty;
            if (string.IsNullOrEmpty(issuerName)) return string.Empty; //no issuesName
            string[] issuerNamePars = issuerName.Split(',');
            // recupera CN
            if (issuerName.Contains("CN="))
                enteCertCN = issuerNamePars.Where(a => a.Contains("CN=")).SingleOrDefault().Split('=')[1];
            // recupera O
            if (issuerName.Contains("O="))
                enteCertO = string.Format("{0}{1}", (string.IsNullOrEmpty(enteCertCN) ? string.Empty : ", "), issuerNamePars.Where(a => a.Contains("O=")).SingleOrDefault().Split('=')[1]);
            if (issuerName.Contains("C="))
                enteCertC = string.Format(", {0}", issuerNamePars.Where(a => a.Contains("C=")).SingleOrDefault().Split('=')[1]);
            return string.Format("{0}{1}{2}", enteCertCN, enteCertO, enteCertC);
        }
        private static DateTime GetDataRiferimentoValiditaDocumento(FileRequest fr, InfoUtente iu)
        {
            return BusinessLogic.Documenti.FileManager.dataRiferimentoValitaDocumento(fr, iu);
        }

        private static DocsPaVO.Mobile.InfoDetailFirma.StatusCode VerificaValiditaFirmaAllaData(FileRequest fr, InfoUtente iu, DocsPaVO.documento.FileDocumento fileDocumento, DateTime dataRif)
        {
            DocsPaVO.Mobile.InfoDetailFirma.StatusCode statusCode = InfoDetailFirma.StatusCode.NOT_DEFINED;
            string risultato = "Non Definito";
            try
            {
                BusinessLogic.Documenti.FileManager.processFileInformationCRLUpdate(fr, iu, fileDocumento, dataRif);
                DocsPaVO.Logger.CodAzione.Esito esito = DocsPaVO.Logger.CodAzione.Esito.OK;
                if (fileDocumento.signatureResult.StatusCode == -100)
                {
                    statusCode = InfoDetailFirma.StatusCode.ERROR_SERVICE_CRL;
                    esito = DocsPaVO.Logger.CodAzione.Esito.KO;
                    risultato = "Errore di connessione al servizio CRL";
                }

                if (fileDocumento.signatureResult.StatusCode != 0)
                {
                    statusCode = InfoDetailFirma.StatusCode.ERROR;
                    risultato = "Verifica con errori "; // +BusinessLogic.Documenti.FileManager.getExtCheckErrors(fileDocumento);
                }
                if (fileDocumento.signatureResult.StatusCode == 0)
                {
                    statusCode = InfoDetailFirma.StatusCode.VERIFY_OK;
                    risultato = "Verifica OK";
                }

                bool revokedPresent = BusinessLogic.Documenti.FileManager.revokedCertArePresent(fileDocumento);
                if (revokedPresent)
                {
                    statusCode = InfoDetailFirma.StatusCode.REVOKED_CERT;
                    risultato = "Sono presenti Certificati Revocati";
                }

                BusinessLogic.UserLog.UserLog.WriteLog(iu, "DOCUMENTOVERIFICACRL", fr.docNumber, string.Format("{0} {1} {2} {3} {4}  Esito:{5}", "Verifica CRL alla data", dataRif.ToLongDateString(), fr.docNumber, "Ver.", fr.version, risultato), esito);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: VerificaValiditaFirma", e);
            }
            return statusCode;
        }

        public static async Task<HSMSignResponse> hsmVerifySign(string token, HSMSignRequest request)
        {
            try
            {
                logger.Info("hsmVerifySign - begin");
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                HSMSignResponse res = new HSMSignResponse();
                DocsPaVO.documento.SchedaDocumento sd = null;
                FileRequest fr = null;
                // recupera il file request del doc
                sd = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDoc, request.IdDoc);
                fr = (FileRequest)sd.documenti[0];
                DocsPaVO.documento.FileDocumento signedDocument = BusinessLogic.Documenti.FileManager.getFile(fr, infoUtente);
                res.Code = (signedDocument != null && signedDocument.signatureResult != null) ? HSMSignResponseCode.OK : HSMSignResponseCode.KO;
                logger.Info("hsmVerifySign - end: " + res.Code.ToString());
                return res;
            }
            catch (Exception e)
            {
                logger.Error("hsmVerifySign - Exception: " + e);
                return HSMSignResponse.ErrorResponse;
            }
        }

        public static async Task<HSMSignResponse> hsmGetMementoForUser(string token)
        {
            try
            {
                logger.Info("hsmGetMementoForUser - begin");
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                HSMSignResponse res = new HSMSignResponse();
                Memento infoMemento = new Memento();
                // recupera le info del memento
                string[] infoMementoResult = BusinessLogic.Utenti.UserManager.getMementoUtente(infoUtente);
                if (infoMementoResult.Length == 2)
                {
                    infoMemento.Dominio = infoMementoResult[0].ToUpper();
                    infoMemento.Alias = infoMementoResult[1].ToUpper();
                }
                res.memento = infoMemento;
                res.Code = HSMSignResponseCode.OK;
                logger.Info("hsmGetMementoForUser - end: " + res.Code.ToString());
                return res;
            }
            catch (Exception e)
            {
                logger.Error("hsmGetMementoForUser - Exception: " + e);
                return HSMSignResponse.ErrorResponse;
            }
        }

        /// <summary>
        /// Verifica se il ruolo è abilitato alla funzione di richiesta OTP
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static async Task<bool> isAllowedOTP(string token)
        {
            bool result = false;
            logger.Info("isAllowedOTP - begin");
            DocsPaVO.utente.Utente utente = null;
            DocsPaVO.utente.InfoUtente infoUtente = null;
            DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

            string authInfoString = Utils.Decrypt(token.Substring(4));
            string[] authInfoArray = authInfoString.Split('|');

            utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
            ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
            infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
            try
            {
                if (ruolo != null)
                {
                    result = (from function in (Funzione[])ruolo.funzioni.ToArray(typeof(Funzione)) where function.codice.ToUpper().Equals("TO_GET_OTP") select function.systemId).Count() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("isAllowedOTP - Exception: " + ex);
            }
            logger.Info("isAllowedOTP - end");
            return result;

        }

        #endregion
        #region LIBRO FIRMA

        public static async Task<LibroFirmaResponse> GetLibroFirmaElements(string token, LibroFirmaRequest request)
        {
            try
            {
                logger.Info("GetLibroFirmaElements - start");
                int totalRecordCount = 0;
                LibroFirmaResponse res = new LibroFirmaResponse();
                res.Elements = new List<LibroFirmaElement>();
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUser = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUser = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                //List<DocsPaVO.LibroFirma.ElementoInLibroFirma> elements = BusinessLogic.LibroFirma.LibroFirmaManager.GetElementiInLibroFirmaIntoPage(infoUser, request.PageSize, request.RequestedPage, request.Testo, request.TipoRicerca, out totalRecordCount);
                // foreach (DocsPaVO.LibroFirma.ElementoInLibroFirma el in elements)
                // {
                //     res.Elements.Add(DocsPaVO.Mobile.LibroFirmaElement.BuildInstance(el));
                // }
                // res.TotalRecordCount = totalRecordCount;
                res = getElemLibroFirma(infoUser, request);
                res.ModalitaFirmaParallela = BusinessLogic.LibroFirma.LibroFirmaManager.IsTipoFirmaParallela();
                logger.Info("GetLibroFirmaElements - end");
                return res;
            }
            catch (Exception e)
            {
                logger.Error("GetLibroFirmaElements - eccezione: " + e);
                return LibroFirmaResponse.ErrorResponse;
            }
        }

        private static LibroFirmaResponse getElemLibroFirma(InfoUtente infoUser, LibroFirmaRequest request)
        {
            int totalRecordCount = 0;
            LibroFirmaResponse res = new LibroFirmaResponse();
            res.Elements = new List<LibroFirmaElement>();
            DocsPaVO.Mobile.RicercaType TipoRicerca;
            if (!string.IsNullOrEmpty(request.TipoRicerca))
            {
                switch (request.TipoRicerca.ToUpper())
                {
                    case "OGGETTO":
                        TipoRicerca = RicercaType.RIC_OGGETTO_LF;
                        break;
                    case "PROPONENTE":
                        TipoRicerca = RicercaType.RIC_PROPONENTE_LF;
                        break;
                    case "NOTE":
                        TipoRicerca = RicercaType.RIC_NOTE_LF;
                        break;
                    case "ID_DOCUMENTO":
                        TipoRicerca = RicercaType.RIC_IDDOC_LF;
                        break;
                    case "NUM_PROTO":
                        TipoRicerca = RicercaType.RIC_NUMPROTO_LF;
                        break;
                    case "DATA":
                        TipoRicerca = RicercaType.RIC_DATA_LF;
                        break;
                    case "NUM_ANNO_PROTO":
                        TipoRicerca = RicercaType.RIC_NUMANNOPROTO_LF;
                        break;
                    case "INTERVALLO_DATE":
                        TipoRicerca = RicercaType.RIC_INTER_DATE_LF;
                        break;
                    default:
                        TipoRicerca = RicercaType.RIC_OGGETTO_LF;
                        break;
                }
            }
            else TipoRicerca = RicercaType.RIC_OGGETTO_LF;

            List<DocsPaVO.filtri.FiltroRicerca> filtriDoc = new List<DocsPaVO.filtri.FiltroRicerca>();
            DocsPaVO.filtri.FiltroRicerca filtroX;

            if (!string.IsNullOrWhiteSpace(request.Oggetto))
            {
                filtroX = new DocsPaVO.filtri.FiltroRicerca();
                filtroX.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.OGGETTO.ToString();
                filtroX.valore = request.Oggetto;
                filtriDoc.Add(filtroX);
            }
            if (!string.IsNullOrWhiteSpace(request.Note))
            {
                filtroX = new DocsPaVO.filtri.FiltroRicerca();
                filtroX.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.NOTE.ToString();
                filtroX.valore = request.Note;
                filtriDoc.Add(filtroX);
            } if (!string.IsNullOrWhiteSpace(request.Proponente))
            {
                filtroX = new DocsPaVO.filtri.FiltroRicerca();
                filtroX.argomento = "PROPONENTE";
                filtroX.valore = request.Proponente;
                filtriDoc.Add(filtroX);
            }
            if (!string.IsNullOrWhiteSpace(request.DataDa))
            {
                filtroX = new DocsPaVO.filtri.FiltroRicerca();
                filtroX.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                filtroX.valore = request.DataDa;
                filtriDoc.Add(filtroX);
            }
            if (!string.IsNullOrWhiteSpace(request.DataA))
            {
                filtroX = new DocsPaVO.filtri.FiltroRicerca();
                filtroX.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                filtroX.valore = request.DataA;
                filtriDoc.Add(filtroX);
            }
            if (!string.IsNullOrWhiteSpace(request.DataProtoDa))
            {
                filtroX = new DocsPaVO.filtri.FiltroRicerca();
                filtroX.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.DATA_PROT_SUCCESSIVA_AL.ToString();
                filtroX.valore = request.DataDa;
                filtriDoc.Add(filtroX);
            }
            if (!string.IsNullOrWhiteSpace(request.DataProtoA))
            {
                filtroX = new DocsPaVO.filtri.FiltroRicerca();
                filtroX.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.DATA_PROT_PRECEDENTE_IL.ToString();
                filtroX.valore = request.DataProtoDa;
                filtriDoc.Add(filtroX);
            }
            if (!string.IsNullOrWhiteSpace(request.IdDocumento))
            {
                filtroX = new DocsPaVO.filtri.FiltroRicerca();
                filtroX.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.DOCNUMBER.ToString();
                filtroX.valore = request.IdDocumento;
                filtriDoc.Add(filtroX);
            }
            if (!string.IsNullOrWhiteSpace(request.NumProto))
            {
                filtroX = new DocsPaVO.filtri.FiltroRicerca();
                filtroX.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.NUM_PROTOCOLLO.ToString();
                filtroX.valore = request.NumProto;
                filtriDoc.Add(filtroX);
            }
            if (!string.IsNullOrWhiteSpace(request.NumAnnoProto))
            {
                filtroX = new DocsPaVO.filtri.FiltroRicerca();
                filtroX.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.ANNO_PROTOCOLLO.ToString();
                filtroX.valore = request.NumAnnoProto;
                filtriDoc.Add(filtroX);
            }



            //List<DocsPaVO.LibroFirma.ElementoInLibroFirma> elements = BusinessLogic.LibroFirma.LibroFirmaManager.GetElementiInLibroFirmaIntoPage(infoUser, request.PageSize, request.RequestedPage, request.Testo, TipoRicerca, out totalRecordCount,
            //    request.Oggetto,request.Proponente,request.Note,request.IdDocumento,request.NumProto,request.DataDa,request.DataA,request.NumAnnoProto);

            List<DocsPaVO.LibroFirma.ElementoInLibroFirma> elements = BusinessLogic.LibroFirma.LibroFirmaManager.GetElementiInLibroFirmaIntoPage(infoUser, request.PageSize, request.RequestedPage, request.Testo, TipoRicerca, out totalRecordCount, filtriDoc);
            DocsPaVO.Mobile.LibroFirmaElement elX1 = null;
            DocsPaVO.documento.InfoDocumento infoDocPrinc = null;
            foreach (DocsPaVO.LibroFirma.ElementoInLibroFirma el in elements)
            {
                elX1 = DocsPaVO.Mobile.LibroFirmaElement.BuildInstance(el);
                if (elX1.InfoDocumento != null)
                    elX1.InfoDocumento.Extension = BusinessLogic.Documenti.FileManager.getOriginalExtension(elX1.Id, string.Format("(select MAX(version_id) from versions where docnumber = {0} )", elX1.Id));
                if (elX1.InfoDocumento != null && !string.IsNullOrWhiteSpace(elX1.InfoDocumento.IdDocPrincipale))
                {
                    infoDocPrinc = BusinessLogic.Documenti.DocManager.GetInfoDocumento(infoUser, elX1.InfoDocumento.IdDocPrincipale, elX1.InfoDocumento.IdDocPrincipale);
                    if (infoDocPrinc != null)
                    {
                        elX1.InfoDocumento.OggettoDocPrincipale = infoDocPrinc.oggetto;
                    }
                }
                res.Elements.Add(elX1);
            }
            res.TotalRecordCount = totalRecordCount;
            return res;
        }

        public static async Task<LibroFirmaResponse> CambiaStatoElementoLibroFirma(string token, LibroFirmaCambiaStatoRequest request)
        {
            LibroFirmaResponse res = new LibroFirmaResponse();
            string message = string.Empty;
            try
            {
                logger.Info("CambiaStatoElementoLibroFirma - begin");
                DocsPaVO.LibroFirma.ElementoInLibroFirma elemento = new DocsPaVO.LibroFirma.ElementoInLibroFirma();
                elemento.IdElemento = request.elemento.IdElemento;
                elemento.DataAccettazione = request.elemento.DataAccettazione;
                elemento.StatoFirma = (DocsPaVO.LibroFirma.TipoStatoElemento)Enum.Parse(typeof(DocsPaVO.LibroFirma.TipoStatoElemento), request.elemento.StatoFirma);
                elemento.IdIstanzaProcesso = request.elemento.IdIstanzaProcesso;
                elemento.MotivoRespingimento = request.elemento.MotivoRespingimento;

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUser = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUser = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaStatoElementoInLibroFirma(elemento, request.NuovoStato, ruolo, infoUser, out message);
            }
            catch (Exception e)
            {
                logger.Error("CambiaStatoElementoLibroFirma - eccezione: " + e);
                return LibroFirmaResponse.ErrorResponse;
            }
            logger.Info("CambiaStatoElementoLibroFirma - end");

            return res;
        }

        public static async Task<LFCambiaStatiResponse> LFCambiaStati(string token, LFCambiaStatiRequest request)
        {
            logger.Info("LFCambiaStati - begin");

            LFCambiaStatiResponse res = new LFCambiaStatiResponse();
            List<ElementResult> elemRes = new List<ElementResult>();
            ElementResult elemResX;
            string message = string.Empty;
            bool retChange = false;
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUser = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUser = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                if (request != null && request.Elementi != null && request.Elementi.Length > 0)
                {
                    logger.Debug("Num elementi in request: " + request.Elementi.Length);
                    foreach (ElementToChange elementX in request.Elementi)
                    {
                        retChange = false;
                        DocsPaVO.LibroFirma.ElementoInLibroFirma elemento = new DocsPaVO.LibroFirma.ElementoInLibroFirma();
                        elemento.IdElemento = elementX.Elemento.IdElemento;
                        elemento.DataAccettazione = elementX.Elemento.DataAccettazione;
                        elemento.StatoFirma = (DocsPaVO.LibroFirma.TipoStatoElemento)Enum.Parse(typeof(DocsPaVO.LibroFirma.TipoStatoElemento), elementX.Elemento.StatoFirma);
                        elemento.IdIstanzaProcesso = elementX.Elemento.IdIstanzaProcesso;
                        elemento.MotivoRespingimento = elementX.Elemento.MotivoRespingimento;

                        elemResX = new ElementResult();
                        elemResX.Elemento = elementX.Elemento;


                        retChange = BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaStatoElementoInLibroFirma(elemento, elementX.NuovoStato, ruolo, infoUser, out message);
                        if (retChange) elemResX.Esito = "OK";
                        else elemResX.Esito = "ERROR";

                        elemRes.Add(elemResX);
                    }

                    res.Elementi = (ElementResult[])elemRes.ToArray();
                }
                else
                {
                    logger.Error("Nessun elemento nella request.");
                }
            }
            catch (Exception e)
            {
                logger.Error("LFCambiaStati - eccezione: " + e);
                return LFCambiaStatiResponse.ErrorResponse;
            }
            logger.Info("LFCambiaStati - end");

            return res;
        }

        public static async Task<LibroFirmaResponse> FirmaSelezionatiElementiLf(string token, LibroFirmaRequest request)
        {
            LibroFirmaResponse res = new LibroFirmaResponse();
            string message = string.Empty;
            try
            {
                logger.Info("FirmaSelezionatiElementiLf - begin");
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUser = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUser = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                //Devo estrarre la lista degli elementi da respingere
                List<DocsPaVO.LibroFirma.ElementoInLibroFirma> elements = BusinessLogic.LibroFirma.LibroFirmaManager.GetElementsInLibroFirmabByState(infoUser, DocsPaVO.LibroFirma.TipoStatoElemento.DA_FIRMARE);
                if (elements != null && elements.Count > 0)
                {
                    //Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(request.IdGruppo);
                    //infoUser.idCorrGlobali = ruolo.systemId;

                    List<FileRequest> fileReqToSign = new List<FileRequest>();
                    Allegato attach;
                    Documento doc;

                    #region AVANZAMENTO ITER
                    List<DocsPaVO.LibroFirma.ElementoInLibroFirma> listElementAdvancementProcess = (from i in elements
                                                                                                    where i.TipoFirma.Equals("DOC_STEP_OVER")
                                                                                                    select i).ToList();
                    if (listElementAdvancementProcess != null && listElementAdvancementProcess.Count > 0)
                    {
                        foreach (DocsPaVO.LibroFirma.ElementoInLibroFirma element in listElementAdvancementProcess)
                        {
                            if (string.IsNullOrEmpty(element.InfoDocumento.IdDocumentoPrincipale))
                            {
                                doc = new Documento()
                                {
                                    descrizione = element.InfoDocumento.Oggetto,
                                    docNumber = element.InfoDocumento.Docnumber,
                                    versionId = element.InfoDocumento.VersionId,
                                    version = element.InfoDocumento.NumVersione.ToString(),
                                    versionLabel = element.InfoDocumento.NumAllegato.ToString(),
                                    inLibroFirma = true

                                };
                                fileReqToSign.Add(doc as FileRequest);
                            }
                            else
                            {
                                attach = new Allegato()
                                {
                                    descrizione = element.InfoDocumento.Oggetto,
                                    docNumber = element.InfoDocumento.Docnumber,
                                    versionId = element.InfoDocumento.VersionId,
                                    version = element.InfoDocumento.NumVersione.ToString(),
                                    versionLabel = element.InfoDocumento.NumAllegato.ToString(),
                                    inLibroFirma = true

                                };
                                fileReqToSign.Add(attach as FileRequest);
                            }
                        }
                        bool isAdvancementProcess = true;
                        PutElectronicSignature(fileReqToSign, infoUser, isAdvancementProcess);
                    }
                    #endregion

                    #region SOTTOSCRIZIONE

                    fileReqToSign.Clear();
                    List<DocsPaVO.LibroFirma.ElementoInLibroFirma> listElementSubscription = (from i in elements
                                                                                              where i.TipoFirma.Equals("DOC_VERIFIED")
                                                                                              select i).ToList();
                    if (listElementSubscription != null && listElementSubscription.Count > 0)
                    {
                        foreach (DocsPaVO.LibroFirma.ElementoInLibroFirma element in listElementSubscription)
                        {
                            if (string.IsNullOrEmpty(element.InfoDocumento.IdDocumentoPrincipale))
                            {
                                doc = new Documento()
                                {
                                    descrizione = element.InfoDocumento.Oggetto,
                                    docNumber = element.InfoDocumento.Docnumber,
                                    versionId = element.InfoDocumento.VersionId,
                                    version = element.InfoDocumento.NumVersione.ToString(),
                                    versionLabel = element.InfoDocumento.NumAllegato.ToString(),
                                    inLibroFirma = true

                                };
                                fileReqToSign.Add(doc as FileRequest);
                            }
                            else
                            {
                                attach = new Allegato()
                                {
                                    descrizione = element.InfoDocumento.Oggetto,
                                    docNumber = element.InfoDocumento.Docnumber,
                                    versionId = element.InfoDocumento.VersionId,
                                    version = element.InfoDocumento.NumVersione.ToString(),
                                    versionLabel = element.InfoDocumento.NumAllegato.ToString(),
                                    inLibroFirma = true

                                };
                                fileReqToSign.Add(attach as FileRequest);
                            }
                        }

                        bool isAdvancementProcess = false;
                        PutElectronicSignature(fileReqToSign, infoUser, isAdvancementProcess);
                    }
                    #endregion

                }

                res = getElemLibroFirma(infoUser, request);
            }
            catch (Exception e)
            {
                logger.Error("FirmaSelezionatiElementiLf - eccezione: " + e);
                return LibroFirmaResponse.ErrorResponse;
            }
            logger.Info("FirmaSelezionatiElementiLf - end");
            return res;
        }

        private static void PutElectronicSignature(List<DocsPaVO.documento.FileRequest> approvingFiles, DocsPaVO.utente.InfoUtente infoUtente, bool isAdvancementProcess)
        {
            string message;
            foreach (DocsPaVO.documento.FileRequest fileReq in approvingFiles)
            {
                message = string.Empty;
                if (BusinessLogic.LibroFirma.LibroFirmaManager.PutElectronicSignature(fileReq, infoUtente, isAdvancementProcess, out message))
                {
                    string method2 = "DOC_VERIFIED";
                    string description2 = "Il documento è stato firmato elettronicamente";
                    if (isAdvancementProcess)
                    {
                        description2 = "Eseguito passo avanzamento iter";
                        method2 = "DOC_STEP_OVER";
                    }
                    if (fileReq.inLibroFirma)
                    {
                        BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaDataEsecuzioneElemento(fileReq.docNumber, DocsPaVO.LibroFirma.TipoStatoElemento.FIRMATO.ToString());
                        BusinessLogic.LibroFirma.LibroFirmaManager.SalvaStoricoIstanzaProcessoFirmaByDocnumber(fileReq.docNumber, description2, infoUtente);
                    }

                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method2, fileReq.docNumber,
                        description2, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "0");
                }
                else if (fileReq.inLibroFirma)
                {
                    BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaErroreEsitoFirma(fileReq.docNumber, message);
                }
            }
        }

        public static async Task<LibroFirmaResponse> RespingiSelezionatiElementiLf(string token, LibroFirmaRequest request)
        {
            logger.Info("RespingiSelezionatiElementiLf - begin");
            LibroFirmaResponse res = new LibroFirmaResponse();
            string message = string.Empty;
            bool result = false;
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUser = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUser = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                //Devo estrarre la lista degli elementi da respingere
                List<DocsPaVO.LibroFirma.ElementoInLibroFirma> elements = BusinessLogic.LibroFirma.LibroFirmaManager.GetElementsInLibroFirmabByState(infoUser, DocsPaVO.LibroFirma.TipoStatoElemento.DA_RESPINGERE);
                if (elements != null && elements.Count > 0)
                {
                    //Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(request.IdGruppo);

                    foreach (DocsPaVO.LibroFirma.ElementoInLibroFirma e in elements)
                    {
                        result = BusinessLogic.LibroFirma.LibroFirmaManager.RejectElementsSignatureProcess(e, ruolo, infoUser);

                        //Se è andato a buon fine ed è un allegato
                        if (result && (!string.IsNullOrEmpty(e.InfoDocumento.IdDocumentoPrincipale)) && (e.InfoDocumento.IdDocumentoPrincipale != e.InfoDocumento.Docnumber))
                        {
                            BusinessLogic.LibroFirma.LibroFirmaManager.StopPassoWait(e.InfoDocumento.IdDocumentoPrincipale, infoUser);
                        }
                    }
                }
                res = getElemLibroFirma(infoUser, request);
            }
            catch (Exception e)
            {
                logger.Error("RespingiSelezionatiElementiLf - eccezione: " + e);
                return LibroFirmaResponse.ErrorResponse;
            }
            logger.Info("RespingiSelezionatiElementiLf - end");
            return res;
        }

        public static async Task<bool> ExistsElementWithSignCades(string token)
        {
            logger.Info("ExistsElementWithSignCades - begin");
            bool result = false;
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                string tipoFirma = "DOC_SIGNATURE";
                result = BusinessLogic.LibroFirma.LibroFirmaManager.ExistsElementWithTypeSign(tipoFirma, infoUtente);
            }
            catch (Exception ex)
            {
                logger.Error("ExistsElementWithSignCades - Exception: " + ex);
            }
            logger.Info("ExistsElementWithSignCades - end");
            return result;

        }

        public static async Task<bool> ExistsElementWithSignPades(string token)
        {
            logger.Info("ExistsElementWithSignPades - begin");

            bool result = false;
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                string tipoFirma = "DOC_SIGNATURE_P";
                result = BusinessLogic.LibroFirma.LibroFirmaManager.ExistsElementWithTypeSign(tipoFirma, infoUtente);
            }
            catch (Exception ex)
            {
                logger.Error("ExistsElementWithSignPades - Exception: " + ex);
            }
            logger.Info("ExistsElementWithSignPades - end");
            return result;

        }

        public static async Task<HSMMultiSignResponse> HsmMultiSign(string token, HSMSignRequest request)
        {
            try
            {
                logger.Info("HsmMultiSign - begin");
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                #region Debug della richiesta
                if (true)
                {
                    System.Xml.Serialization.XmlSerializer seri = new System.Xml.Serialization.XmlSerializer(typeof(HSMSignRequest));
                    using (var sww = new System.IO.StringWriter())
                    {
                        using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sww))
                        {
                            seri.Serialize(writer, request);
                            string xmlDoc = sww.ToString(); // Your XML
                            logger.Debug(xmlDoc);
                        }
                    }
                }
                #endregion
                HSMMultiSignResponse res = new HSMMultiSignResponse();
                List<string> idDocuments = new List<string>();
                DocsPaVO.documento.SchedaDocumento sd = null;
                FileRequest[] frs = null;
                FileRequest fr = null;
                string method = "DOC_SIGNATURE";
                string description = "Il documento è stato firmato digitalmente HSM CADES";
                if (request.TipoFirma.ToString() == "PADES")
                {
                    method = "DOC_SIGNATURE_P";
                    description = "Il documento è stato firmato digitalmente HSM PADES";
                }

                idDocuments = BusinessLogic.LibroFirma.LibroFirmaManager.GetIdDocumentInLibroFirmabBySign(infoUtente, DocsPaVO.LibroFirma.TipoStatoElemento.DA_FIRMARE, method);
                if (idDocuments != null && idDocuments.Count > 0)
                {
                    frs = new FileRequest[idDocuments.Count];
                    int idfr = 0;
                    foreach (string idDoc in idDocuments)
                    {

                        // recupera il file request del doc
                        sd = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idDoc, idDoc);
                        fr = (FileRequest)sd.documenti[0];
                        // cambiamento del servizio interno
                        frs[idfr] = fr;
                        idfr++;
                        /*
                            res.Code = ((BusinessLogic.Documenti.FileManager.HSM_Sign(infoUtente,
                                                                                 fr,
                                                                                 request.cofirma,
                                                                                 request.timestamp,
                                                                                 request.TipoFirma,
                                                                                 request.AliasCertificato,
                                                                                 request.DominioCertificato,
                                                                                 request.OtpFirma,
                                                                                 request.PinCertificato,
                                                                                 request.ConvertPdf)
                                                                                  ) ?
                                                                                HSMSignResponseCode.OK :
                                                                                HSMSignResponseCode.SYSTEM_ERROR);
                        logger.Info("end: " + res.Code.ToString());
                        if (res.Code.ToString().Equals(HSMSignResponseCode.OK.ToString()))
                        {
                            if (fr.inLibroFirma)
                            {
                                BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaDataEsecuzioneElemento(fr.docNumber, DocsPaVO.LibroFirma.TipoStatoElemento.FIRMATO.ToString());
                            }

                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method, fr.docNumber,
                             description, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "0");
                        }
                        else
                        {
                            if (fr.inLibroFirma)
                            {
                                string msg = "Errore durante la procedura di firma";
                                BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaErroreEsitoFirma(fr.docNumber, msg);
                            }
                        }
                        */
                    }
                    string tokenHSM = BusinessLogic.Documenti.FileManager.HSM_OpenMultiSignSession(infoUtente, frs, request.cofirma, request.timestamp, request.TipoFirma);
                    DocsPaVO.documento.FirmaResult[] firme = BusinessLogic.Documenti.FileManager.HSM_SignMultiSignSession(infoUtente, frs, tokenHSM, request.AliasCertificato, request.DominioCertificato, request.OtpFirma, request.PinCertificato, request.cofirma);
                    ArrayList resFirmaHSMMulti = new ArrayList();
                    DocsPaVO.Mobile.Responses.InfoDocFirmaHSM rispX = null;
                    if (firme != null && firme.Length > 0)
                    {
                        InfoDocFirmaHSM[] resultArray = new InfoDocFirmaHSM[firme.Length];
                        int irax = 0;
                        foreach (FirmaResult firmaX in firme)
                        {
                            if (!string.IsNullOrWhiteSpace(firmaX.errore) && firmaX.errore.ToUpper().StartsWith("TRUE"))
                            {
                                rispX = new InfoDocFirmaHSM() { ErrorMessage = "", IdDocumento = firmaX.fileRequest.docNumber, Status = "OK" };
                            }
                            else
                            {
                                rispX = new InfoDocFirmaHSM() { ErrorMessage = firmaX.errore, IdDocumento = firmaX.fileRequest.docNumber, Status = "KO" };
                            }
                            resultArray[irax] = rispX;
                            irax++;
                            //resFirmaHSMMulti.Add(rispX);
                        }
                        //res.infoFirma = (InfoDocFirmaHSM[])resFirmaHSMMulti.ToArray();
                        res.infoFirma = resultArray;
                        res.Code = HSMMultiSignResponseCode.OK;
                    }
                    else
                    {
                        logger.Error("Firme non effettuate.");
                    }
                }
                logger.Info("HsmMultiSign - end");
                return res;
            }
            catch (Exception e)
            {
                logger.Error("HsmMultiSign - Exception: " + e);
                return HSMMultiSignResponse.ErrorResponse;
            }
        }

        /// <summary>
        /// Verifica se il ruolo è abilitato alla funzione di richiesta OTP
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static async Task<bool> LibroFirmaIsAutorized(string token)
        {
            logger.Info("LibroFirmaIsAutorized - begin");
            bool result = false;
            try
            {
                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                Ruolo role = BusinessLogic.Utenti.UserManager.getRuolo(authInfoArray[0]);
                if (role != null)
                {
                    result = (from function in (Funzione[])role.funzioni.ToArray(typeof(Funzione)) where function.codice.ToUpper().Equals("DO_LIBRO_FIRMA") select function.systemId).Count() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("LibroFirmaIsAutorized - Exception: " + ex);
            }
            logger.Info("LibroFirmaIsAutorized - end");
            return result;

        }
        #endregion

        private static DateTime toDate(string date)
        {
            string[] formats = {"dd/MM/yyyy",
                                "dd/MM/yyyy HH:mm:ss",
								"dd/MM/yyyy h:mm:ss",
								"dd/MM/yyyy h.mm.ss",
								"dd/MM/yyyy HH.mm.ss"};
            return DateTime.ParseExact(date, formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AllowWhiteSpaces);
        }
    }
    public class RuoliComparer : IComparer
    {

        public int Compare(object x, object y)
        {
            int xId = Int32.Parse(((Ruolo)x).systemId);
            int yId = Int32.Parse(((Ruolo)y).systemId);
            return xId.CompareTo(yId);
        }
    }
}