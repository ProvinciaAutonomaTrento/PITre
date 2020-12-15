using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Subscriber
{
    /// <summary>
    /// Metadati di un canale di pubblicazione
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public class ChannelInfo
    {
        /// <summary>
        /// Identificativo univoco del canale pubblicazione
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del canale di pubblicazione
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del canale di pubblicazione
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Attributi contenenti informazioni di contesto del modulo Publisher che è utile fornire al Subscriber
        /// </summary>
        public NameValuePair[] PublisherContextInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Host Smtp per l'invio delle eMail di notifica della pubblicazione
        /// </summary>
        public string SmtpHost
        {
            get;
            set;
        }

        /// <summary>
        /// Porta Smtp
        /// </summary>
        public int SmtpPort
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il server Smtp utilizza il protocollo Ssl per la comunicazione sicura
        /// </summary>
        public bool SmtpSsl
        {
            get;
            set;
        }

        /// <summary>
        /// Nome utente Smtp
        /// </summary>
        public string SmtpUserName
        {
            get;
            set;
        }

        /// <summary>
        /// Password utente Smtp
        /// </summary>
        public string SmtpPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Mail Smtp
        /// </summary>
        public string SmtpMail
        {
            get;
            set;
        }
    }
}