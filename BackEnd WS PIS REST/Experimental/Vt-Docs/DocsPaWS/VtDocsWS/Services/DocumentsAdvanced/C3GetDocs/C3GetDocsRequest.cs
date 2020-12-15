using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.DocumentsAdvanced.C3GetDocs
{
    [DataContract]
    public class C3GetDocsRequest : Request
    {
        /// <summary>
        /// Opzione per il prelievo dei doc modificati, da valorizzare a TRUE nel caso
        /// </summary>
        [DataMember]
        public string Modified
        {
            get;
            set;
        }

        /// <summary>
        /// Limite inferiore della data, formato dd/mm/yyyy hh:mi:ss
        /// </summary>
        [DataMember]
        public string FromDateTime
        {
            get;
            set;
        }

        /// <summary>
        /// Limite superiore della data, formato dd/mm/yyyy hh:mi:ss
        /// </summary>
        [DataMember]
        public string ToDateTime
        {
            get;
            set;
        }

        /// <summary>
        /// Opzioni di scelta rapida per l'intervallo date. Esempio: D -> Daily 
        /// </summary>
        [DataMember]
        public string DateLimitsOptions { get; set; }

    }
}