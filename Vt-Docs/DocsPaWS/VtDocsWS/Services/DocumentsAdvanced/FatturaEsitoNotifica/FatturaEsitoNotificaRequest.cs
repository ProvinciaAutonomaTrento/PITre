using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.DocumentsAdvanced.FatturaEsitoNotifica
{
    [DataContract]
    public class FatturaEsitoNotificaRequest : Request
    {
        /// <summary>
        /// DocNumber del documento a cui associare la notifica
        /// </summary>
        [DataMember]
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// File della notifica
        /// </summary>
        [DataMember]
        public Domain.File File
        {
            get;
            set;
        }

        /// <summary>
        /// Esito da inserire nel campo profilato.
        /// </summary>
        [DataMember]
        public string EsitoNotifica
        {
            get;
            set;
        }
    }
}