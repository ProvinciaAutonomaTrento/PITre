using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Email.SendMail
{
    [DataContract]
    public class SendMailResponse : Response
    {

        /// <summary>
        /// Esito della mail Messaggio
        /// </summary>
        [DataMember]
        public string StatusMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Esito della mail codice
        /// </summary>
        [DataMember]
        public string StatusCode
        {
            get;
            set;
        }

    }
}