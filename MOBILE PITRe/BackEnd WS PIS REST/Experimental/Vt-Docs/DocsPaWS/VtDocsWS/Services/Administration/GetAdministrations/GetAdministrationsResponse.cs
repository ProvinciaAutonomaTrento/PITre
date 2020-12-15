using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
//using System.Threading.Tasks;

namespace VtDocsWS.Services.Administration.GetAdministrations
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetAdministrations"
    /// </summary>
    [DataContract]
    public class GetAdministrationsResponse : Response
    {
        /// <summary>
        /// Restituzione della lista di amministrazioni 
        /// </summary>
        [DataMember]
        public Domain.Administration[] Administrations
        {
            get;
            set;
        }
    }
}
