using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Reflection;

namespace Subscriber.Dispatcher.CalendarMail
{
    /// <summary>
    /// Dispatcher per la pubblicazione dei contenuti tramite eMail
    /// </summary>
    public class MailDispatcher : Subscriber.Dispatcher.IDispatcher
    {
        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(MailDispatcher));

        /// <summary>
        /// 
        /// </summary>
        private const string CALENDAR_CONTENT_TYPE = "text/calendar";

        /// <summary>
        /// Pubblicazione dei contenuti
        /// </summary>
        /// <param name="request"></param>
        public virtual void Dispatch(MailRequest request)
        {
            _logger.Info("BEGIN");

            try
            {
                using (MailMessage message = this.MakeMessage(request))
                {
                    _logger.InfoFormat("SmtpHost: {0}; SmtpPort: {1}; SmtpSsl: {2}; SmtpUserName: {3}; SmtpPassword: {4}", 
                                        request.Sender.Host,
                                        request.Sender.Port,
                                        request.Sender.SSL,
                                        request.Sender.UserName,
                                        request.Sender.Password);

                    SmtpClient smtp = new SmtpClient(request.Sender.Host, request.Sender.Port);
                    smtp.UseDefaultCredentials = false;
                    smtp.EnableSsl = request.Sender.SSL;
                    smtp.Credentials = new System.Net.NetworkCredential(request.Sender.UserName, request.Sender.Password);

                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                throw ex;
            }
            finally
            {
                _logger.Info("END");
            }
        
        }

        /// <summary>
        /// Pubblicazione dei contenuti
        /// </summary>
        /// <param name="data"></param>
        public virtual void Dispatch(object data)
        {
            this.Dispatch((MailRequest) data);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Normalizzazione mittente della mail
        /// </summary>
        /// <param name="senderName"></param>
        /// <returns></returns>
        protected virtual string NormalizeSenderName(string senderName)
        {
            senderName = senderName.Replace(",", " ");
            senderName = senderName.Replace(";", " ");
            senderName = senderName.Replace(".", " ");
            senderName = senderName.Replace("-", " ");
            senderName = senderName.Replace("_", " ");
            return senderName;
        }

        /// <summary>
        /// Normalizzazione oggetto della mail
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        protected virtual string NormalizeSubject(string subject)
        {
            return subject.Replace('\r', ' ').Replace('\n', ' ');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appointment"></param>
        /// <param name="attendeeList"></param>
        /// <returns></returns>
        protected MailMessage MakeMessage(MailRequest request)
        {
            _logger.Info("BEGIN");

            try
            {
                //  Set up the different mime types contained in the message    
                ContentType calendarType = new ContentType(CALENDAR_CONTENT_TYPE);

                //  Add parameters to the calendar header   
                calendarType.Parameters.Add("name", "appointment.ics");

                if (!string.IsNullOrEmpty(request.Method))
                    calendarType.Parameters.Add("method", request.Method);

                MailMessage message = new MailMessage();

                _logger.InfoFormat("SenderEMail: {0}; SenderName: {1}", request.Sender.SenderEMail, this.NormalizeSenderName(request.Sender.SenderName));
                _logger.InfoFormat("Subject: {0}", request.Subject);

                message.From = new MailAddress(request.Sender.SenderEMail, request.Sender.SenderName);
                message.Subject = this.NormalizeSubject(request.Subject);
                    
                foreach (string attendee in request.To)
                {
                    _logger.InfoFormat("To: {0}", attendee);

                    message.To.Add(new MailAddress(attendee));
                }

                if (request.CC != null)
                {
                    foreach (string attendee in request.CC)
                    {
                        _logger.InfoFormat("CC: {0}", attendee);

                        message.CC.Add(new MailAddress(attendee));
                    }
                }

                if (request.Bcc != null)
                {
                    foreach (string attendee in request.Bcc)
                    {
                        _logger.InfoFormat("Bcc: {0}", attendee);

                        message.Bcc.Add(new MailAddress(attendee));
                    }
                }

                //if (!string.IsNullOrEmpty(request.Body))
                //{
                //    _logger.InfoFormat("HtmlBody: {0}", request.Body);

                //    string bodyText = string.Format(this.GetMailBodyTemplate(), request.Body);
                //    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(bodyText, new System.Net.Mime.ContentType("text/html"));
                //    htmlView.TransferEncoding = TransferEncoding.QuotedPrintable;
                //    message.AlternateViews.Add(htmlView);
                //}

                if (!string.IsNullOrEmpty(request.AppointmentAsText))
                {
                    _logger.InfoFormat("iCalendarMessage: {0}", request.AppointmentAsText);

                    AlternateView calendarView = AlternateView.CreateAlternateViewFromString(request.AppointmentAsText, calendarType);
                    calendarView.TransferEncoding = TransferEncoding.QuotedPrintable;
                    message.AlternateViews.Add(calendarView);
                }

                return message;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                throw ex;
            }
            finally
            {
                _logger.Info("END");
            }
        }

        /// <summary>
        /// Reperimento template del body della mail
        /// </summary>
        /// <returns></returns>
        protected string GetMailBodyTemplate()
        {
            const string MAIL_BODY_TEMPLATE = "MailBody.htm";

            string content = string.Empty;

            string templateName = typeof(MailDispatcher).Namespace + ".Templates." + MAIL_BODY_TEMPLATE;

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(templateName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }

            return content;
        }
    }
}
