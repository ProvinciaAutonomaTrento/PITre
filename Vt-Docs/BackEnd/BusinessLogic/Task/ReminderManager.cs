using BusinessLogic.interoperabilita;
using BusinessLogic.Interoperabilità;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Task
{
    public class ReminderManager
    {
        private static ILog _logger = LogManager.GetLogger(typeof(ReminderManager));

        /// <summary>
        /// Summary description for ChilkatKeys.
        /// </summary>
        public class ChilkatKeys
        {
            public const string SMime = "SETNOTSMIME_zHBJHW4d6Rpk";
            public const string Mail = "SETNOTMAILQ_f6YHFfVZ6R3z";
            public const string Crypt = "SETNOTCrypt_ZX8rgyYKORN6";
            public const string IMAP = "SETNOTIMAPMAIL_DfzE4tbH6R17";
            public const string PFX = "SETNOTPFX_ZBWXKJFC7YsS";
        }

        /// <summary>
        /// 
        /// </summary>
        private const string CALENDAR_TYPE = "text/calendar";

        /// <summary>
        /// Metodo per la creazione di un reminder
        /// </summary>
        /// <param name="reminder"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private bool CreateReminder(DocsPaVO.Task.Task task, string subject, DocsPaVO.utente.InfoUtente infoUtente)
        {
            _logger.Debug("BusinessLogic.Task.ReminderManager.CreateReminder");
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Reminder r = new DocsPaDB.Query_DocsPAWS.Reminder();
                result = r.InsertReminder(task, subject, infoUtente);
            }
            catch (Exception e)
            {
                _logger.Debug("Errore in BusinessLogic.Task.ReminderManager.CreateReminder", e);
                result = false;
            }

            return result;
        }
         
        /// <summary>
        /// Invia un reminder
        /// </summary>
        /// <param name="reminder"></param>
        /// <param name="infoutente"></param>
        /// <returns></returns>
        public bool SendReminder(DocsPaVO.Task.Task task, DocsPaVO.utente.InfoUtente infoutente)
        {
            _logger.Info("BEGIN");
            bool ret = false;

            try
            {
                BusinessLogic.interoperabilita.CalendarMail.MailRequest request = CreateMailRequest(task.UTENTE_MITTENTE.idAmministrazione);
                
                request.Subject = ComputeMailSubject(task);
                request.Appointment = new interoperabilita.CalendarMail.iCalendar.AppointmentInfo()
                {
                    Sequence = 1,
                    UID = BusinessLogic.interoperabilita.CalendarMail.iCalendar.UidGenerator.Create(),
                    OrganizerName = task.UTENTE_MITTENTE.cognome,
                    OrganizerEMail = task.UTENTE_MITTENTE.email,
                    Method = BusinessLogic.interoperabilita.CalendarMail.iCalendar.AppointmentMethodTypes.REQUEST,
                    Status = BusinessLogic.interoperabilita.CalendarMail.iCalendar.AppointmentStatusTypes.CONFIRMED,
                    DtStamp = DateTime.Now,
                    Summary = request.Subject,
                    DtStart = DateTime.Now,
                    DtEnd = DateTime.Now,
                    Alert = new interoperabilita.CalendarMail.iCalendar.AlertInfo()
                    {
                        TriggerDays = GetTriggerDays(task.STATO_TASK.DATA_SCADENZA),
                        Description = request.Subject
                    }
                };

                // Invio messaggio
                DispatchMail(request);

                // Salvo i dati su db
                CreateReminder(task, request.Subject, infoutente);

                ret = true;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }

            return ret;
        }

        private int GetTriggerDays(string scadenza)
        {
            DateTime dtScadenza = DateTime.Parse(scadenza);
            double days = (dtScadenza - DateTime.Now).TotalDays;

            return (int)days;
        }

        private void DispatchMail(interoperabilita.CalendarMail.MailRequest request)
        {
            string err = string.Empty;
            SvrPosta mailserver = null;

            try
            {
                mailserver = new SvrPosta(
                    request.Sender.Host,
                    request.Sender.UserName,
                    request.Sender.Password,
                    request.Sender.Port.ToString(),
                    System.IO.Path.GetTempPath(),
                    CMClientType.SMTP, 
                    "1", 
                    "0");

                mailserver.connect();

                mailserver.sendMail(
                    request.Sender.SenderEMail,
                    request.To[0],
                    request.Subject,
                    request.Body,
                    GetMailAttachment(request)
                    );

                if (!string.IsNullOrEmpty(err))
                    throw new Exception(err);
            }
            finally
            {
                if (mailserver != null)
                    mailserver.disconnect();
            }
        }

        private void DisplatchMailTest(interoperabilita.CalendarMail.MailRequest request)
        {
            string err = string.Empty;
            SvrPosta mailserver = null;

            try
            {
                //mailserver = new SvrPosta(
                //    "sendm.cert.legalmail.it", "KIT00427", "WX0UV8U8", "465",
                //    System.IO.Path.GetTempPath(), CMClientType.SMTP, "1", "0");

                mailserver = new SvrPosta(
                    "freesmtp.valueteam.com", "", "", "25",
                    System.IO.Path.GetTempPath(), CMClientType.SMTP, "0", "0");

                mailserver.connect();
                mailserver.sendMail(
                    "claudio.rinaldi@nttdata.com",
                    "claudio.rinaldi@nttdata.com",
                    null,
                    null,
                    "Reminder",
                    "This is a test body",
                    CMMailFormat.Text,
                    GetMailAttachment(request),
                    out err);

                if (!string.IsNullOrEmpty(err))
                    throw new Exception(err);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (mailserver != null)
                    mailserver.disconnect();
            }
        }

        private void DisplatchMailTest2(interoperabilita.CalendarMail.MailRequest request)
        {
            bool success = false;

            Chilkat.MailMan server = new Chilkat.MailMan()
            {
                SmtpHost = "freesmtp.valueteam.com",
                SmtpUsername = "",
                SmtpPassword = ""
            };

            Chilkat.Email mail = new Chilkat.Email()
        {
                Subject = "This is a iCal test",
                Body = "This is a iCal test",
                From = "Organizer <claudio.rinaldi@nttdata.com>"
            };

            server.UnlockComponent(ChilkatKeys.Mail);

            success = mail.AddTo("Attendee","claudio.rinaldi@nttdata.com");
            if (success != true)
                throw new Exception(mail.LastErrorText);

            success = mail.AddiCalendarAlternativeBody(request.AppointmentAsText,"REQUEST");
            if (success != true)
                throw new Exception(mail.LastErrorText);

            success  = server.SendEmail(mail);
            if (success != true)
                throw new Exception(server.LastErrorText);

            success = server.CloseSmtpConnection();
            if (success != true)
                throw new Exception("Connection to SMTP server not closed cleanly.");
        }

        private CMAttachment[] GetMailAttachment(BusinessLogic.interoperabilita.CalendarMail.MailRequest request)
        {
            return new CMAttachment[]
            {
                new CMAttachment()
                {
                    _data = Encoding.ASCII.GetBytes(request.AppointmentAsText),
                    name = "appointment.ics",
                    contentType = CALENDAR_TYPE,
                    _method = request.Method
                }
            };
        }

        private string ComputeMailSubject(DocsPaVO.Task.Task task)
        {
            return string.Format("Reminder : {0} - {1}",task.DESC_RAGIONE_TRASM,task.ID_TASK);
        }

        private BusinessLogic.interoperabilita.CalendarMail.MailRequest CreateMailRequest(string idamministrazione)
        {
            System.Data.DataSet ds;
            string smtp_user = string.Empty;
            string smtp_pwd = string.Empty;
            BusinessLogic.interoperabilita.CalendarMail.MailRequest request = null;

            try
            {
                DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
                obj.getSmtp(out ds, idamministrazione);

                if (ds.Tables["SERVER"].Rows.Count == 0)
                    return null;

                string server = ds.Tables["SERVER"].Rows[0]["VAR_SMTP"].ToString();
                if (!ds.Tables["SERVER"].Rows[0]["VAR_USER_SMTP"].Equals(System.DBNull.Value))
                    smtp_user = ds.Tables["SERVER"].Rows[0]["VAR_USER_SMTP"].ToString();
                if (!ds.Tables["SERVER"].Rows[0]["VAR_PWD_SMTP"].Equals(System.DBNull.Value))
                    smtp_pwd = DocsPaUtils.Security.Crypter.Decode(ds.Tables["SERVER"].Rows[0]["VAR_PWD_SMTP"].ToString(), smtp_user);

                string port = ds.Tables["SERVER"].Rows[0]["NUM_PORTA_SMTP"].ToString();
                string SmtpSsl = ds.Tables["SERVER"].Rows[0]["CHA_SMTP_SSL"].ToString();
                string SmtpSTA = ds.Tables["SERVER"].Rows[0]["CHA_SMTP_STA"].ToString();

                // Inserimento dati mittente
                request = new BusinessLogic.interoperabilita.CalendarMail.MailRequest()
                {
                    Body = string.Empty,
                    Sender = new BusinessLogic.interoperabilita.CalendarMail.MailSender()
                    {
                        Host = server,
                        Port = int.Parse(port),
                        SSL = SmtpSsl == "0" ? true : false,
                        UserName = smtp_user,
                        Password = smtp_pwd,
                    }
                };
            }
            catch(Exception)
            {
            }

            return request;
        }
    }
}
