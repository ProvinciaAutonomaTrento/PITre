using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.LibroFirma.ClosePassoAndGetNext
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "ClosePassoAndGetNextResponse"
    /// </summary>
    [DataContract]
    public class ClosePassoAndGetNextResponse : Response
    {
        /// <summary>
        /// IstanzaPasso successiva
        /// </summary>
        [DataMember]
        public Domain.IstanzaPassoFirma IstanzaPasso
        {
            get;
            set;
        }
    }
}