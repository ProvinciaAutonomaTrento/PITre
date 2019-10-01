using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using VtDocsWS.Domain;

namespace VtDocsWS.Services.FollowDomainObject
{
    [DataContract]
    public class FollowRequest: Request
    {

        /// <summary>
        /// Id oggetto che si intende monitorare/ non monitorare più
        /// </summary>
        [DataMember]
        public string IdObject
        {
            get;
            set;
        }

        /// <summary>
        /// Specifica il tipo di operazione:
        /// 1. Monitora documento
        /// 2. Non monitorare più il documento
        /// 3. Monitora il fascicolo
        /// 4. Non monitorare più il fascicolo
        /// </summary>
        [DataMember]
        public OperationFollow Operation
        {
            get;
            set;
        }
         /// <summary>
        /// Codice dell'applicazione
        /// </summary>
        [DataMember]
        public override string CodeApplication
        {
            get;
            set;
        }
    }
}