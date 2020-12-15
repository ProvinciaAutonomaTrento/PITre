using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Dispatcher.CalendarMail
{
    /// <summary>
    /// Informazioni sull'account Smtp per la pubblicazione dei contenuti tramite eMail
    /// </summary>
    [Serializable()]
    public class MailSender
    {
        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(MailSender));

        /// <summary>
        /// Server Smtp
        /// </summary>
        public string Host
        {
            get;
            set;
        }

        /// <summary>
        /// Porta server Smtp
        /// </summary>
        public int Port
        {
            get;
            set;
        }

        /// <summary>
        /// Nome utente Smtp
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Password utente Smtp
        /// </summary>
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se è utilizzato il protocollo Ssl per l'invio delle eMail
        /// </summary>
        public bool SSL
        {
            get;
            set;
        }

        /// <summary>
        /// Nome utente
        /// </summary>
        public string SenderName
        {
            get;
            set;
        }

        /// <summary>
        /// Mail utente
        /// </summary>
        public string SenderEMail
        {
            get;
            set;
        }
    }
}
