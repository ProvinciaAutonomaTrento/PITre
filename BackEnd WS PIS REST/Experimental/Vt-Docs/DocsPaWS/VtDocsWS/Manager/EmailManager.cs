using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLogic.Interoperabilità;
using System.Configuration;
using VtDocsWS.WebServices;
using log4net;

namespace VtDocsWS.Manager
{
    public class EmailManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(EmailManager));
        private static string errorMessage = string.Empty;
        public static Services.Email.SendMail.SendMailResponse SendMail(Services.Email.SendMail.SendMailRequest request)
        {
            logger.Debug("Start");
            Services.Email.SendMail.SendMailResponse response = new Services.Email.SendMail.SendMailResponse();
            string idRegistro = request.IdRegister;
            string to = request.ToEmailAddress;
            string cc = request.CcEmailAddress;
            string body = request.EmailBody;
            string subject = request.Subject;
            string emailMittente = request.FromEmailAddress;
            bool retval = false; 
            string statusString=string.Empty;
            if (string.IsNullOrEmpty(cc))
                cc = String.Empty;

            //Gestisce il formato del BodyMail: Text / HTML
            CMMailFormat format;
            if (request.Format.Equals(VtDocsWS.Services.Email.SendMail.EmailBodyFormat.HTML))
                format = CMMailFormat.HTML;
            else
                format = CMMailFormat.Text;
            

            DocsPaVO.amministrazione.CasellaRegistro[] caselle = BusinessLogic.Amministrazione.RegistroManager.GetMailRegistro(idRegistro);
            //Nel caso non passo l'emailMittente allora prendo il first or default dell'array caselle, sempre se la count è maggiore di uno.
            if (String.IsNullOrEmpty(emailMittente))
            {
                if (caselle.Length > 0)
                    emailMittente = caselle[0].EmailRegistro;
            }

            //Preparo gli attachments
            List<CMAttachment> cmAttachmentLst = new List<CMAttachment>();
            if (request.EmailAttachment != null)
            {
                foreach (Domain.EmailAttachment datt in request.EmailAttachment)
                {
                    cmAttachmentLst.Add(new CMAttachment
                    {
                        sourceFile = datt.SourceFile,
                        name = datt.Name,
                        contentType = datt.ContentType,
                        _data = datt.AttachmentData
                    });
                }
            }
            try
            {
                foreach (DocsPaVO.amministrazione.CasellaRegistro casellaMittente in caselle)
                {
                    if (casellaMittente.EmailRegistro.ToLower().Equals(emailMittente.ToLower()))
                    {
                        logger.DebugFormat("Trovata casella per {0}", emailMittente);
                        //creazione ed invio mail
                        string porta = null;

                        if (casellaMittente.PortaSMTP != 0)
                            porta = casellaMittente.PortaSMTP.ToString();

                        string smtp_user = (!string.IsNullOrEmpty(casellaMittente.UserSMTP)) ? casellaMittente.UserSMTP : null;
                        string smtp_pwd = (!string.IsNullOrEmpty(casellaMittente.PwdSMTP)) ? casellaMittente.PwdSMTP : null;

                        string ricevutaPec = string.Empty;
                        ricevutaPec = (!string.IsNullOrEmpty(casellaMittente.RicevutaPEC)) ? casellaMittente.RicevutaPEC : null;
                        string X_TipoRicevuta = null;
                        //aggiunta la trim() per gestire la presenza di spazi bianchi nei campi VAR_USER_SMTP e VAR_PWD_SMTP
                        if (smtp_user != null)
                            smtp_user = smtp_user.Trim();
                        if (smtp_pwd != null)
                            smtp_pwd = smtp_pwd.Trim();

                        if (ricevutaPec != null)
                        {

                            if (ricevutaPec != string.Empty)
                            {
                                X_TipoRicevuta = ricevutaPec;
                                switch (ricevutaPec.Length)
                                {
                                    case 1:
                                        X_TipoRicevuta = ricevutaPec.Substring(0, 1);
                                        break;
                                    case 2:
                                        //Se la len è maggiore di uno, vuol dire che ho un valore diverso da quello di default
                                        //Preleverò quindi quello.
                                        X_TipoRicevuta = ricevutaPec.Substring(1, 1);
                                        break;
                                    default:    //non si sa mai
                                        X_TipoRicevuta = string.Empty;
                                        break;
                                }
                                //Qui transcodifico il tipo ricevuta CHA in header 
                                //(sarebbe carino metterlo in un enum per evitare hardcoding nel codice).
                                switch (X_TipoRicevuta)
                                {
                                    case "C":
                                        X_TipoRicevuta = "completa";
                                        break;
                                    case "B":
                                        X_TipoRicevuta = "breve";
                                        break;
                                    case "S":
                                        X_TipoRicevuta = "sintetica";
                                        break;
                                    default:
                                        X_TipoRicevuta = null;
                                        break;
                                }
                            }
                        }
                        errorMessage = string.Empty;

                        retval = creaMail(
                                   subject,
                                   body,
                                   casellaMittente.ServerSMTP,
                                   casellaMittente.EmailRegistro,
                                   smtp_user,
                                   smtp_pwd,
                                   to,
                                   cc,
                                   porta,
                                   casellaMittente.SmtpSSL.ToString(),
                                   casellaMittente.PopSSL.ToString(),
                                   casellaMittente.SmtpSta.ToString(),
                                   X_TipoRicevuta,
                                   cmAttachmentLst.ToArray(),
                                   format,
                                   out statusString
                                   );

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat ("Invio Mail a {2} con oggetto {3} ha dato errore {0} : {1} ", ex.Message,ex.StackTrace,to,subject);
                response.Success = false;
                response.Error = new Services.ResponseError();
                response.Error.Code = "ERR_SEND_MAIL";
                response.Error.Description = "Errore invio della mail";
            }


            if (!retval)
            {
                logger.DebugFormat("Invio Fallito {0} : {1} ", errorMessage, statusString);
                response.StatusCode = "false";
                response.Success = false;
                response.StatusMessage = String.Format("errore :{0} EmailStatus {1}", errorMessage, statusString);
                response.Error = new Services.ResponseError();
                response.Error.Code = "ERR_CREATE_MAIL";
                response.Error.Description = "Errore invio della mail";
            }
            else
            {
                logger.DebugFormat("Email To {0} from {1} with subject {2} sent correctly", to,emailMittente,subject);
                response.StatusCode = "true";
                response.Success = true;
                response.StatusMessage = statusString;
            }

            logger.Debug("End");
            return response;
        }

        private static bool creaMail(string subject,string bodyMail, string server, string mailMitt, string smtp_user, string smtp_pwd, string mailDest, string mailDestCC, string port, string SmtpSsl, string PopSsl, string smtpSTA, string X_TipoRicevuta, CMAttachment[] attachments, CMMailFormat format,out string statusString)
        {
            bool retValue = false;
            logger.Debug("Start");
            SvrPosta svr = new SvrPosta(server,
                smtp_user,
                smtp_pwd,
                port,
               null,
                    CMClientType.SMTP, SmtpSsl, PopSsl, smtpSTA);

            try
            {
                svr.connect();
                List<CMMailHeaders> headers = new List<CMMailHeaders>();
                if ((X_TipoRicevuta != null) && (X_TipoRicevuta != string.Empty))
                    headers.Add(new CMMailHeaders { header = "X-TipoRicevuta", value = X_TipoRicevuta });

                svr.sendMail(mailMitt, mailDest, mailDestCC, String.Empty, subject, bodyMail, format, attachments, out statusString);
                //svr.sendMail(mailMitt, mailDest, mailDestCC, String.Empty, subject, bodyMail, CMMailFormat.Text, attachments,out statusString);
                retValue = true;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Invio Mail traimte creaMail con errore {0} : {1} ", e.Message, e.StackTrace);
                errorMessage = e.Message;
                throw e;
            }
            finally
            {
                
                svr.disconnect();
            }
            logger.Debug("End");
            return retValue;
        }

    }
}