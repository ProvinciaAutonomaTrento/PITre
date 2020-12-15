using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.CheckWS.CheckConnection
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "CheckConnectionResponse"
    /// </summary>
    [DataContract]
    public class CheckConnectionResponse : Response
    {
        /// <summary>
        /// EsitoConnection restituito
        /// </summary>
        [DataMember]
        public bool EsitoConnection
        {
            get;
            set;
        }
    }
}