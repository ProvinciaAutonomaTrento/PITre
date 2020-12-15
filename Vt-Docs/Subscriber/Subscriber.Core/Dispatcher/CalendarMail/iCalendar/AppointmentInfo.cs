using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Dispatcher.CalendarMail.iCalendar
{
    /// <summary>
    /// Classe per la gestione delle informazioni relative ad un appuntamento
    /// </summary>
    [Serializable()]
    public class AppointmentInfo
    {
        /// <summary>
        /// 
        /// </summary>
        private string _organizerName = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private string _summary = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private string _location = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private string _description = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(AppointmentInfo));

        /// <summary>
        /// Identificativo univoco del messaggio iCalendar
        /// </summary>
        [AppointmentParamAttribute(Name = "{uid}", CalName = "UID")]
        public string UID
        {
            get;
            set;
        }

        /// <summary>
        /// Method del messaggio iCalendar
        /// </summary>
        /// <remarks>
        /// Valori ammessi:
        /// - REQUEST per un appuntamento di conferma
        /// - CANCEL per un appuntamento di cancellazione
        /// </remarks>
        [AppointmentParamAttribute(Name = "{method}", CalName = "METHOD")]
        public string Method
        {
            get;
            set;
        }

        /// <summary>
        /// Nome dell'organizzatore dell'appuntamento
        /// </summary>
        [AppointmentParamAttribute(Name = "{organizerName}", CalName = "ORGANIZER")]
        public string OrganizerName
        {
            get
            {
                return this._organizerName;
            }
            set
            {
                this._organizerName = this.NormalizeValue(value);
            }
        }

        /// <summary>
        /// eMail dell'organizzatore dell'appuntamento
        /// </summary>
        [AppointmentParamAttribute(Name = "{organizerMail}", CalName = "MAILTO")]
        public string OrganizerEMail
        {
            get;
            set;
        }

        /// <summary>
        /// Data di creazione dell'appuntamento
        /// </summary>
        [AppointmentParamAttribute(Name = "{dtstamp}", CalName = "DTSTAMP")]
        public DateTime DtStamp
        {
            get;
            set;
        }

        /// <summary>
        /// Data di inizio dell'appuntamento
        /// </summary>
        [AppointmentParamAttribute(Name = "{dtstart}", CalName = "DTSTART")]
        public DateTime DtStart
        {
            get;
            set;
        }

        /// <summary>
        /// Data finale dell'appuntamento
        /// </summary>
        [AppointmentParamAttribute(Name = "{dtend}", CalName = "DTEND")]
        public DateTime DtEnd
        {
            get;
            set;
        }

        /// <summary>
        /// Stato dell'appuntamento
        /// </summary>
        /// <remarks>
        /// Valori ammessi:
        /// - CONFIRMED per un appuntamento confermato
        /// - CANCELLED per un appuntamento revocato
        /// </remarks>
        [AppointmentParamAttribute(Name = "{status}", CalName = "STATUS")]
        public string Status
        {
            get;
            set;
        }

        /// <summary>
        /// Contatore di sequenza dell'appuntamento
        /// </summary>
        /// <remarks>
        /// Lo stesso appuntamento, ovvero contrassegnato dallo stesso UID, può essere inviato più volte incrementando il contatore di sequenza
        /// </remarks>
        [AppointmentParamAttribute(Name = "{sequence}", CalName = "SEQUENCE")]
        public int Sequence
        {
            get;
            set;
        }

        /// <summary>
        /// Oggetto dell'appuntamento
        /// </summary>
        /// <remarks>
        /// Al massimo a 255 caratteri consentiti
        /// </remarks>
        [AppointmentParamAttribute(Name = "{summary}", CalName = "SUMMARY")]
        public string Summary
        {
            get
            {
                return this._summary;
            }
            set
            {
                this._summary = this.TruncateValue(value, 255, "...");
            }
        }

        /// <summary>
        /// Luogo dell'appuntamento
        /// </summary>
        /// <remarks>
        /// Al massimo a 255 caratteri consentiti
        /// </remarks>
        [AppointmentParamAttribute(Name = "{location}", CalName = "LOCATION")]
        public string Location
        {
            get
            {
                return this._location;
            }
            set
            {
                this._location = this.TruncateValue(value, 255, "...");
            }
        }

        
        /// <summary>
        /// Descrizione dell'appuntamento
        /// </summary>
        [AppointmentParamAttribute(Name = "{description}", CalName = "DESCRIPTION")]
        public string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value;
            }
        }

        /// <summary>
        /// Informazioni relative all'alert
        /// </summary>
        public AlertInfo Alert
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual string NormalizeValue(string value)
        {
            value = value.Replace(",", " ");
            value = value.Replace(";", " ");
            value = value.Replace(".", " ");
            value = value.Replace("-", " ");
            value = value.Replace("_", " ");
            return value;
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
