using System;
using System.Text;
using System.Collections.Generic;
using DocsPaDB.Query_DocsPAWS;
using System.Net;
using System.IO;
using System.Linq;
using log4net;

namespace Subscriber.AlboTelematico.Rules
{
    /// <summary>
    /// 
    /// </summary>
    public class PubblicazioneAlboRule : Subscriber.Rules.BaseRule
    {
        #region Public Members

        private static ILog logger = LogManager.GetLogger(typeof(PubblicazioneAlboRule));
        /// <summary>
        /// 
        /// </summary>
        public override string RuleName
        {
            get
            {
                return RULE_NAME;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string[] GetSubRules()
        {
            return new string[0];
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Nome della regola
        /// </summary>
        private const string RULE_NAME = "ALBO_TELEMATICO_RULE";

        /// <summary>
        /// 
        /// </summary>
        private const string DOCUMENT_OBJECT_TYPE = "Documento";

        /// <summary>
        /// 
        /// </summary>
        protected override void InternalExecute()
        {
            PublishedObject pubObj = ListenerRequest.EventInfo.PublishedObject;
            string IdDocument = pubObj.IdObject;
            string UserName = FindProperty("UserName").Value.ToString();
            string CodeRole = FindProperty("RoleCode").Value.ToString();
            string StateDiagram = FindProperty("Stato").Value.ToString().Trim().ToUpper();
            string CodeAdm = FindProperty("CodeAdm").Value.ToString();
            logger.DebugFormat("Dati: IdDoc {0}, User {1}, ruolo {2}, stateDiagram {3}, CodeAdm {4}", IdDocument, UserName, CodeRole, StateDiagram, CodeAdm);
            bool computed = false;
            try
            {
                //valido ed eventualmente notifico ad ALT che il documento è pronto per la PUBBLICAZIONE/REVOCA/ANNULLAMENTO
                computed =  this.IsValidForNotify(IdDocument, UserName, CodeRole, StateDiagram, CodeAdm);
            }
            catch (Exception ex)
            {
                computed = false;
                this.Response.Rule.Error = new ErrorInfo
                {
                    Id = Subscriber.ErrorCodes.UNHANDLED_ERROR,
                    Message = ex.Message,
                    Stack = ex.ToString()
                };
            }
            finally
            {
                this.Response.Rule.Computed = computed;
                this.Response.Rule.ComputeDate = DateTime.Now;
                this.WriteRuleHistory(this.Response.Rule);
            }
        }

        /// <summary>
        /// Controlla che il documeno sia valido per la pubblicazione 
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsValidForNotify(string IdDocument, string UserName, string CodeRole, string StateDiagram, string CodeAdm)
        {
            //accesso ai servizi WPF
            sr.Documents services = new AlboTelematico.sr.Documents();
            // In seguito all'esposizione dei due endpoint da parte dei servizi (uno http uno https), Documents non esiste più
            // bisogna usare questo oggetto
            //sr.BasicHttpBinding_IDocuments services = new AlboTelematico.sr.BasicHttpBinding_IDocuments();
            int valtmp;
            logger.Debug("Prelevo il token");
            string authToken = getAuthenticationToken(CodeAdm, UserName);
            string durata = this.FindProperty(FieldTipology.DURATA).Value.ToString();
            //verifico se il documento si trova in uno stato che genera notifica ad ALT
            if (this.AreEquals(StateDiagram, DocumentState.PUBBLICARE) ||
                this.AreEquals(StateDiagram, DocumentState.ANNULLARE) ||
                this.AreEquals(StateDiagram, DocumentState.REVOCARE))
            {
                logger.Debug("Stato genera notifica:" + StateDiagram);
                if (string.IsNullOrEmpty(durata))
                    durata = "0";
                if (!Int32.TryParse(durata, out valtmp) )
                {
                   
                    //imposto l'opportuno stato di errore e termino l'esecuzione
                    sr.EditDocStateDiagramRequest requestState = new sr.EditDocStateDiagramRequest();
                    requestState.IdDocument = IdDocument;
                    requestState.UserName = UserName;
                    requestState.CodeRoleLogin = CodeRole;
                    //Per evitare che l'utente venga sloggato con l'utilizzo del token, bisogna evitare di mandare in request il codeAdm
                    requestState.CodeAdm = CodeAdm;
                    requestState.AuthenticationToken = authToken;
                    switch (StateDiagram.ToUpper())
                    {
                        case DocumentState.PUBBLICARE:
                            requestState.StateOfDiagram = DocumentState.PUBBLICARE_ERRORE;
                            break;
                        case DocumentState.ANNULLARE:
                            requestState.StateOfDiagram = DocumentState.ANNULLARE_ERRORE;
                            break;
                        case DocumentState.REVOCARE:
                            requestState.StateOfDiagram = DocumentState.REVOCARE_ERRORE;
                            break;
                    }
                    try
                    {
                        services.EditDocStateDiagram(requestState); //servizio da invocare per il cambio stato
                    }
                    catch (System.ServiceModel.FaultException e)
                    {
                        this.Response.Rule.Error = new ErrorInfo
                        {
                            Id = ErrorCodes.WS_MODIFY_DIAGRAM_STATE_ERROR,
                            Message = string.Format(ErrorDescriptions.WS_MODIFY_DIAGRAM_STATE_ERROR, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                        };
                        return false;
                    }
                    this.Response.Rule.Error = new ErrorInfo
                    {
                        Id = ErrorCodes.FORMAT_NUMBER_ERROR,
                        Message = string.Format(ErrorDescriptions.FORMAT_NUMBER_ERROR, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                    };
                    return false;
                }
                logger.Debug("GetDocument");
                sr.GetDocumentRequest requestGetDocument = new sr.GetDocumentRequest();
                sr.GetDocumentResponse responseGetDocument = new sr.GetDocumentResponse();
                requestGetDocument.IdDocument = IdDocument;
                requestGetDocument.UserName = UserName;
                requestGetDocument.CodeRoleLogin = CodeRole;
                requestGetDocument.CodeApplication = "ALBO_TELEMATICO";
                requestGetDocument.CodeAdm = CodeAdm;
                requestGetDocument.AuthenticationToken = authToken;
                requestGetDocument.GetFile = true;
                requestGetDocument.GetFileSpecified = true;
                try
                {
                    logger.Debug("Immediatamente prima getDocument");
                    responseGetDocument = services.GetDocument(requestGetDocument); // servizio invocato per il reperimento delle informazioni del documento
                    logger.Debug("GetDocument eseguito");
                }
                catch (System.ServiceModel.FaultException e)
                {
                    this.Response.Rule.Error = new ErrorInfo
                    {
                        Id = ErrorCodes.WS_GETDOCUMENT_ERROR,
                        Message = string.Format(ErrorDescriptions.WS_GETDOCUMENT_ERROR, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                    };
                    logger.Error("Errore nella getDocument, Messaggio: " + e.Message + ", Stack: " + e.StackTrace);
                    return false;
                }
                catch (Exception ex)
                {
                    this.Response.Rule.Error = new ErrorInfo
                    {
                        Id = ErrorCodes.WS_GETDOCUMENT_ERROR,
                        Message = string.Format(ErrorDescriptions.WS_GETDOCUMENT_ERROR, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                    };
                    logger.Error("Errore nella getDocument, Messaggio: " + ex.Message + ", Stack: " + ex.StackTrace);
                    
                    return false;
                }

                //se il documento è da pubblicare
                //if (StateDiagram.Equals(DocumentState.PUBBLICARE))
                if (this.AreEquals(StateDiagram, DocumentState.PUBBLICARE))
                {
                    try
                    {
                        logger.Debug("Pubblicare");
                        //controllo se l'estensione del documento da pubblicare e dei suoi allegati è pdf o p7m
                        if(responseGetDocument.Document.MainDocument != null)
                        {
                            //if (this.FindProperty("FileName") != null && !string.IsNullOrEmpty(this.FindProperty("FileName").Value.ToString()))
                            if(!string.IsNullOrEmpty(responseGetDocument.Document.MainDocument.Name))
                            {
                                logger.Debug(responseGetDocument.Document.MainDocument.Name);
                                //if (!IsTypePdfP7m(this.FindProperty("FileName").Value.ToString()))
                                if(!IsTypePdfP7m(responseGetDocument.Document.MainDocument.Name))
                                {
                                    logger.Debug("Formato non valido");
                                    //logger.Debug(this.FindProperty("FileName").Value.ToString());
                                    
                                    this.Response.Rule.Error = new ErrorInfo
                                    {
                                        Id = ErrorCodes.EXTENSION_FILE_ERROR,
                                        Message = string.Format(ErrorDescriptions.EXTENSION_FILE_ERROR, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                                    };
                                    // imposto lo stato DA PUBBLICARE ERRORE
                                    sr.EditDocStateDiagramRequest requestState = new sr.EditDocStateDiagramRequest();
                                    requestState.IdDocument = IdDocument;
                                    requestState.UserName = UserName;
                                    requestState.CodeAdm = CodeAdm;
                                    requestState.CodeRoleLogin = CodeRole;
                                    requestState.StateOfDiagram = DocumentState.PUBBLICATO_ERRORE;
                                    requestState.AuthenticationToken = authToken;
                                    services.EditDocStateDiagram(requestState);
                                    return false;
                                }
                            }
                        }

                        //lo stesso controllo sull'estensione del file ripetuto per gli allegati
                        if(responseGetDocument.Document.Attachments != null && responseGetDocument.Document.Attachments.Length > 0)
                        {
                            int count = responseGetDocument.Document.Attachments.Length;
                            for (int i = 0; i < count; i++)
                            {
                                //Property fileExtension = this.FindProperty(String.Format("AttachmentName_{0}", i));
                                //if(!IsTypePdfP7m(fileExtension.Value.ToString()))
                                    if (responseGetDocument.Document.Attachments[i] != null && !string.IsNullOrEmpty(responseGetDocument.Document.Attachments[i].Name))
                                    {
                                        if(!IsTypePdfP7m(responseGetDocument.Document.Attachments[i].Name))
                                        {
                                            logger.Debug("Formato allegato non valido");
                                            //logger.Debug(fileExtension.Value);
                                            logger.Debug(responseGetDocument.Document.Attachments[i].Name);
                                            this.Response.Rule.Error = new ErrorInfo
                                            {
                                                Id = ErrorCodes.EXTENSION_FILE_ERROR,
                                                Message = string.Format(ErrorDescriptions.EXTENSION_FILE_ERROR, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                                            };
                                            // imposto lo stato DA PUBBLICARE ERRORE
                                            sr.EditDocStateDiagramRequest requestState = new sr.EditDocStateDiagramRequest();
                                            requestState.IdDocument = IdDocument;
                                            requestState.UserName = UserName;
                                            requestState.CodeAdm = CodeAdm;
                                            requestState.CodeRoleLogin = CodeRole;
                                            requestState.StateOfDiagram = DocumentState.PUBBLICATO_ERRORE;
                                            requestState.AuthenticationToken = authToken;
                                            services.EditDocStateDiagram(requestState);

                                            return false;
                                        }
                                    }
                            }
                        }

                        //invoco il servizio di ALT
                        if (!NotifyALT(IdDocument, UserName, CodeRole, CodeAdm))
                        {
                           this.Response.Rule.Error = new ErrorInfo
                            {
                                Id = ErrorCodes.ALBO_TELEMATICO_SERVICES_ERROR,
                                Message = string.Format(ErrorDescriptions.ALBO_TELEMATICO_SERVICES_ERROR, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                            };
                            //il servizio di ALT ha fallito, quindi mettiamo lo stato del documento a DA PUBBLICARE ERRORE
                            sr.EditDocStateDiagramRequest requestState = new sr.EditDocStateDiagramRequest();
                            requestState.IdDocument = IdDocument;
                            requestState.UserName = UserName;
                            requestState.CodeAdm = CodeAdm;
                            requestState.CodeRoleLogin = CodeRole;
                            requestState.StateOfDiagram = DocumentState.PUBBLICARE_ERRORE;
                            requestState.AuthenticationToken = authToken;
                            try
                            {
                                services.EditDocStateDiagram(requestState);
                            }
                            catch(System.ServiceModel.FaultException e)
                            {
                                this.Response.Rule.Error = new ErrorInfo
                                {
                                    Id = ErrorCodes.WS_MODIFY_TEMPLATE_ERROR,
                                    Message = string.Format(ErrorDescriptions.WS_MODIFY_TEMPLATE_ERROR, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                                };
                                return false;
                            }
                            return false;
                        }
                        else
                        {
                            /* se ho notificato correttamente ad ALT la presenza del documento da pubblicare allora imposta tramite i nuovi WS di PITRE 
                            * il responsabile della pubblicazione. 
                            * Lato ALT, andrà configurata la data di pubblicazione del documento tramite l'opportuna chiamata ai nuovi PIS.
                            */
                            try
                            {
                                //devo impostare il Nome dell'ente pubblicante uguale al codice amministrazione
                                foreach (sr.Field f in responseGetDocument.Document.Template.Fields)
                                {
                                    if (f.Name.Equals("Nome dell'ente pubblicante"))
                                    {
                                        f.Value = CodeAdm;
                                        break;
                                    }
                                }
                                sr.EditDocumentRequest requestEditDocument = new sr.EditDocumentRequest();
                                requestEditDocument.UserName = UserName;
                                requestEditDocument.CodeAdm = CodeAdm;
                                requestEditDocument.CodeRoleLogin = CodeRole;
                                requestEditDocument.Document = responseGetDocument.Document;
                                requestEditDocument.AuthenticationToken = authToken;
                                requestEditDocument.CodeApplication = "ALBO_TELEMATICO";
                                try
                                {
                                    services.EditDocument(requestEditDocument);
                                }
                                catch (System.ServiceModel.FaultException e)
                                {
                                    this.Response.Rule.Error = new ErrorInfo
                                    {
                                        Id = ErrorCodes.WS_MODIFY_TEMPLATE_ERROR,
                                        Message = string.Format(ErrorDescriptions.WS_MODIFY_TEMPLATE_ERROR, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                                    };
                                    return false;
                                }
                                catch (Exception e)
                                {
                                    this.Response.Rule.Error = new ErrorInfo
                                    {
                                        Id = ErrorCodes.WS_MODIFY_TEMPLATE_ERROR,
                                        Message = string.Format(e.StackTrace, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                                    };
                                    return false;
                                }
                            }
                            catch (Exception e)
                            {
                                this.Response.Rule.Error = new ErrorInfo
                                {
                                    Id = ErrorCodes.WS_MODIFY_TEMPLATE_ERROR,
                                    Message = string.Format(e.StackTrace, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                                };
                                return false;
                            }
                            return true;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }

                //se il documento è nello stato DA ANNULLARE
                //if (StateDiagram.Equals(DocumentState.ANNULLARE))
                if (this.AreEquals(StateDiagram, DocumentState.ANNULLARE))
                {
                    logger.Debug("Da Annullare");
                    DateTime dataPubb = new DateTime();
                    logger.Debug("Provo a caricare il giorno odierno");
                    DateTime now =  Convert.ToDateTime(DateTime.Now.ToString("d", new System.Globalization.CultureInfo("it-IT")),
                        new System.Globalization.CultureInfo("it-IT"));
                    logger.Debug(now.ToString());
                    foreach (sr.Field f in responseGetDocument.Document.Template.Fields)
                    {
                        if (f.Name.Equals("Data di pubblicazione"))
                        {
                            logger.Debug("Data di pubblicazione :"+f.Value);
                            dataPubb = Convert.ToDateTime(f.Value, new System.Globalization.CultureInfo("it-IT"));
                            dataPubb = dataPubb.Add(TimeSpan.FromDays(1));
                            logger.Debug(dataPubb.ToString());
                            break;
                        }
                    }
                    TimeSpan ts = now - dataPubb;
                    int differenceInDays = ts.Days;
                    if (differenceInDays > 5) // non posso annullare il documento
                    {
                        logger.Debug("Non annullabile. Vado in PUBBLICATO - IMPOSSIBILE ANNULLARE - SUPERATA LA DATA CONSENTITA");
                        this.Response.Rule.Error = new ErrorInfo
                        {
                            Id = ErrorCodes.VOID_DOCUMENT_ERROR,
                            Message = string.Format(ErrorDescriptions.VOID_DOCUMENT_ERROR, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                        };
                        //imposto lo stato del documento a IMPOSSIBILE ANNULLARE - SUPERATA LA DATA CONSENTITA
                        sr.EditDocStateDiagramRequest requestState = new sr.EditDocStateDiagramRequest();
                        requestState.IdDocument = IdDocument;
                        requestState.UserName = UserName;
                        requestState.CodeAdm = CodeAdm;
                        requestState.CodeRoleLogin = CodeRole;
                        requestState.StateOfDiagram = DocumentState.IMPOSSIBILE_ANNULLARE;
                        requestState.AuthenticationToken = authToken;
                        services.EditDocStateDiagram(requestState);
                        return false;
                    }
                    else
                    {
                        //invoco il servizio di ALT
                        if (!NotifyALT(IdDocument, UserName, CodeRole, CodeAdm))
                        {
                            this.Response.Rule.Error = new ErrorInfo
                            {
                                Id = ErrorCodes.ALBO_TELEMATICO_SERVICES_ERROR,
                                Message = string.Format(ErrorDescriptions.ALBO_TELEMATICO_SERVICES_ERROR, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                            };
                            return false;
                        }
                    }
                }

                //se il documento è da revocare
                //if (StateDiagram.Equals(DocumentState.REVOCARE))
                if (this.AreEquals(StateDiagram, DocumentState.REVOCARE))
                {
                    logger.Debug("Da revocare");
                    if (Int32.TryParse(durata, out valtmp) && valtmp > 0) //il documento si trova nello stato da revocare, ma in realtà non prevede revoca
                    {
                        logger.Debug("Non revocabile. Vado in PUBBLICATO - IMPOSSIBILE REVOCARE DOC SENZA REVOCA");
                        
                        // imposto lo stato IMPOSSIBILE REVOCARE DOC SENZA REVOCA
                        sr.EditDocStateDiagramRequest requestState = new sr.EditDocStateDiagramRequest();
                        requestState.IdDocument = IdDocument;
                        requestState.UserName = UserName;
                        requestState.CodeAdm = CodeAdm;
                        requestState.CodeRoleLogin = CodeRole;
                        requestState.StateOfDiagram = DocumentState.IMPOSSIBILE_REVOCARE;
                        requestState.AuthenticationToken = authToken;
                        services.EditDocStateDiagram(requestState);

                        //scrivo nell'history 
                        this.Response.Rule.Error = new ErrorInfo
                        {
                            Id = ErrorCodes.REVOCATION_DOCUMENT_ERROR,
                            Message = string.Format(ErrorDescriptions.REVOCATION_DOCUMENT_ERROR, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                        };
                        return false;
                    }
                    else
                    {
                        //invoco il servizio di ALT
                        if (!NotifyALT(IdDocument, UserName, CodeRole, CodeAdm))
                        {
                            this.Response.Rule.Error = new ErrorInfo
                            {
                                Id = ErrorCodes.ALBO_TELEMATICO_SERVICES_ERROR,
                                Message = string.Format(ErrorDescriptions.ALBO_TELEMATICO_SERVICES_ERROR, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                            };
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                this.Response.Rule.Error = new ErrorInfo
                {
                    Id = ErrorCodes.STATE_DISCARDS_ERROR,
                    Message = string.Format(ErrorDescriptions.STATE_DISCARDS_ERROR, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                };
                return false;
            }
        }

        /// <summary>
        /// Invoco il servizio di ALT per notificare la presenza di un documento da PUBBLICARE/ANNULLARE/REVOCARE
        /// </summary>
        protected virtual bool NotifyALT(string IdDocument, string UserName, string CodeRole, string CodeAdmin)
        {
            try
            {
                logger.Debug("start");
                String date = DateTime.Now.ToString("yyyyMMdd");
                logger.DebugFormat("Date: {0}", date);
                String input =  date + IdDocument + UserName + CodeAdmin + CodeRole;
                input.Replace(" ", "");
                logger.DebugFormat("Input {0}", input);
                System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
                bs = x.ComputeHash(bs);
                System.Text.StringBuilder s = new System.Text.StringBuilder();
                foreach (byte b in bs)
                {
                    s.Append(b.ToString("x2"));
                }
                string hash = s.ToString();
                logger.DebugFormat("Hash: {0}", hash);
                // Per test
                //string sURL = "http://194.105.52.153/gate_pi3/?iddoc=" + IdDocument + "&username=" + UserName + "&codamm=" +
                //    CodeAdmin + "&codrole=" + CodeRole + "&cod=" + hash;

                // Per produzione
                string sURL = "https://www.albotelematico.tn.it/gate_pi3p/?iddoc=" + IdDocument + "&username=" + UserName + "&codamm=" +
                    CodeAdmin + "&codrole=" + CodeRole + "&cod=" + hash;
                logger.DebugFormat("SUrl: {0}", sURL);
                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(sURL);
                Stream objStream;
                objStream = wrGETURL.GetResponse().GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);
                return Convert.ToBoolean(objReader.ReadLine());
            }
            catch (Exception ex)
            {
                this.Response.Rule.Error = new ErrorInfo
                {
                    Id = ErrorCodes.ALBO_TELEMATICO_SERVICES_ERROR,
                    Message = ex.Message
                };
                logger.Error("Errore nella notifica ad Albo Telematico. " + ex.Message);
                logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Scrittura esito dell'esecuzione della regola
        /// </summary>
        /// <param name="rule"></param>
        protected virtual void WriteRuleHistory(BaseRuleInfo rule)
        {
            // Scrittura elemento di pubblicazione nell'history
            RuleHistoryInfo historyInfo = RuleHistoryInfo.CreateInstance(rule);
            historyInfo.Author = this.ListenerRequest.EventInfo.Author;
            historyInfo.ObjectSnapshot = this.ListenerRequest.EventInfo.PublishedObject;
            historyInfo = DataAccess.RuleHistoryDataAdapter.SaveHistoryItem(historyInfo);
        }

        //verifica se il file (documento principale o allegato è di tipo pdf/p7m)
        private bool IsTypePdfP7m(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;
            string[] splitExtArray = fileName.Split('.');
            string fileExtension = string.Empty;
            if (splitExtArray.Length > 1)
                fileExtension = splitExtArray[splitExtArray.Length - 1].ToLower();
            else return false;
            List<string> extensions = new List<string>();
            extensions.AddRange(new string[] {"p7m","pdf"});
            if (fileExtension.Equals(extensions.First()) || fileExtension.Equals(extensions.Last()))
                return true;
            else return false;
        }

        /// <summary>
        /// Preleva 
        /// </summary>
        /// <param name="codeAdm"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        private string getAuthenticationToken(string codeAdm, string Username)
        {
            logger.Debug("Start");
            string retval = string.Empty;
            // in seguito all'esposizione di due endpoint, non si può usare l'oggetto token ma bisogna utilizzare il binding.
            tkn.Token service = new AlboTelematico.tkn.Token();
            //tkn.BasicHttpBinding_IToken service= new AlboTelematico.tkn.BasicHttpBinding_IToken();
            tkn.GetAuthenticationTokenRequest tokenRequest = new tkn.GetAuthenticationTokenRequest();
            tokenRequest.CodeAdm = codeAdm;
            tokenRequest.UserName = Username;
            logger.DebugFormat("Request: CodeAdm {0}, User {1}", codeAdm, Username);
            logger.DebugFormat("Endpoint: "+ service.Url);
            tkn.GetAuthenticationTokenResponse tokenResponse = new tkn.GetAuthenticationTokenResponse();
            try
            {
                logger.Debug("Prelievo");
                tokenResponse = service.GetAuthenticationToken(tokenRequest);
                retval = tokenResponse.AuthenticationToken;
                logger.Debug("prelevato");
            }
            catch (System.ServiceModel.FaultException e)
            {
                logger.ErrorFormat("Errore: {0}, stack {1}", e.Message, e.StackTrace);
                this.Response.Rule.Error = new ErrorInfo
                {
                    Id = ErrorCodes.WS_GET_AUTHENTICATION_TOKEN_ERROR,
                    Message = string.Format(ErrorDescriptions.WS_GET_AUTHENTICATION_TOKEN_ERROR, this.ListenerRequest.EventInfo.PublishedObject.IdObject)
                };

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore: {0}, stack {1}", e.Message, e.StackTrace);
            }
            return retval;
        }
        #endregion
    }   
}
