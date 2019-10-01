using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Transmissions.GiveUpRights
{
    [DataContract]
    public class GiveUpRightsRequest : Request
    {
        /// <summary>
        /// Diritto da mantenere:
        /// WRITE - Scrittura
        /// READ - Lettura
        /// NONE - Eliminazione di tutti i diritti
        /// </summary>
        [DataMember]
        public string RightToKeep
        {
            get;
            set;
        }

        /// <summary>
        /// ID dell'oggetto sul quale cedere i diritti
        /// </summary>
        [DataMember]
        public string idObject
        {
            get;
            set;
        }
    }
}