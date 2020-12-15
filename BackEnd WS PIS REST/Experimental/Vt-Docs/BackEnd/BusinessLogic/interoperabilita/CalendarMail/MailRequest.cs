using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.interoperabilita.CalendarMail
{
    /// <summary>
    /// Dati di richiesta della mail di pubblicazione
    /// </summary>
    [Serializable()]
    public class MailRequest
    {
        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(MailRequest));

        /// <summary>
        /// 
        /// </summary>
        private string _subject = string.Empty;

        /// <summary>
        /// Dati del mittente 
        /// </summary>
        public MailSender Sender
        {
            get;
            set;
        }

        /// <summary>
        /// Lista dei destinatari principali
        /// </summary>
        public string[] To
        {
            get;
            set;
        }

        /// <summary>
        /// Lista dei destinatari in cononscenza
        /// </summary>
        public string[] CC
        {
            get;
            set;
        }

        /// <summary>
        /// Lista dei destinatari in copia nascosta
        /// </summary>
        public string[] Bcc
        {
            get;
            set;
        }

        /// <summary>
        /// Oggetto della mail
        /// </summary>
        public string Subject
        {
            get
            {
                return this._subject;
            }
            set
            {
                this._subject = this.TruncateValue(value, 255, "...");
            }
        }

        /// <summary>
        /// Corpo della mail
        /// </summary>
        public string Body
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        private iCalendar.AppointmentInfo _appointment = null;

        /// <summary>
        /// Appuntamento iCalendar
        /// </summary>
        public iCalendar.AppointmentInfo Appointment
        {
            get
            {
                return this._appointment;
            }
            set
            {
                this._appointment = value;

                // Creazione del messaggio iCalendar
                this.Method = this._appointment.Method;
            }
        }

        /// <summary>
        /// Appumento in formato testo iCalendar
        /// </summary>
        public string AppointmentAsText
        {
            get
            {
                return iCalendar.Appointment.Make(this._appointment);
            }
            set
            {}
        }

        /// <summary>
        /// Mail method
        /// </summary>
        public string Method
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLenght"></param>
        /// <param name="placeHolder"></param>
        /// <returns></returns>
        protected virtual string TruncateValue(string value, int maxLenght, string placeHolder)
        {
            if (maxLenght > 0)
            {
                if (value.Length > maxLenght)
                {
                    value = string.Format("{0}{1}", value.Substring(0, (value.Length - placeHolder.Length)), placeHolder);
                }
            }

            return value;
        }
    }
}
