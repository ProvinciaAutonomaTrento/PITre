using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
//using System.Threading.Tasks;

namespace VtDocsWS.Services.Administration.GetAdministrations
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetAdministrations"
    /// </summary>
    [DataContract]
    public class GetAdministrationsRequest
    {
        /// <summary>
        /// Nome utente per l'autenticazione
        /// </summary>
        [DataMember]
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Password per l'autenticazione
        /// </summary>
        [DataMember]
        public string Password
        {
            get;
            set;
        }
    }
}
