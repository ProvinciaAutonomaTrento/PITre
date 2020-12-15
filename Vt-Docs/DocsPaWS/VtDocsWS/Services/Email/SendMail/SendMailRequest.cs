using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Email.SendMail
{
    [DataContract]
    public class SendMailRequest : Request
    {
        /// <summary>
        /// Id del Registro
        /// </summary>
        [DataMember]
        public string IdRegister
        {
            get;
            set;
        }

        /// <summary>
        ///Oggetto della mail
        /// </summary>
        [DataMember]
        public string Subject
        {
            get;
            set;
        }


        /// <summary>
        ///Email Mittente
        /// </summary>
        [DataMember]
        public string FromEmailAddress
        {
            get;
            set;
        }

        /// <summary>
        ///Email Destinatorio 
        /// </summary>
        [DataMember]
        public string ToEmailAddress
        {
            get;
            set;
        }

        /// <summary>
        ///Email Destinatorio in CC 
        /// </summary>
        [DataMember]
        public string CcEmailAddress
        {
            get;
            set;
        }

        /// <summary>
        ///Corpo della mail
        /// </summary>
        [DataMember]
        public string EmailBody
        {
            get;
            set;
        }

        /// <summary>
        ///Corpo della mail
        /// </summary>
        [DataMember]
        public Domain.EmailAttachment[] EmailAttachment
        {
            get;
            set;
        }

        /// <summary>
        ///Formato del body della mail
        /// </summary>
        [DataMember]
        public EmailBodyFormat Format
        {
            get;
            set;
        }
        
    }

    /// <summary>
    /// Enumerato bodyFormat
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3", Name = "EmailBodyFormat")]
    public enum EmailBodyFormat
    {
        [EnumMember]
        Text,
        [EnumMember]
        HTML
    }
}