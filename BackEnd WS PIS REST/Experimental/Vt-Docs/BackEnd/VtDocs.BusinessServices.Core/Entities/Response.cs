using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;

namespace VtDocs.BusinessServices.Entities
{
    /// <summary>
    /// Classe per l'incapsulamento dei pacchetti di risposta di tutti i BusinessServices
    /// </summary>
    [Serializable()]
    public class Response
    {
        /// <summary>
        /// 
        /// </summary>
        public Response()
        {
        }

        /// <summary>
        /// Indica l'esito dell'invocazione al servizio
        /// </summary>
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// Riporta in formato stringa l'eventuale eccezione avvenuta nell'invocazione al servizio
        /// </summary>
        /// <remarks>
        /// Valorizzata solo se Success è false
        /// </remarks>
        public string Exception
        {
            get;
            set;
        }
    }
}
