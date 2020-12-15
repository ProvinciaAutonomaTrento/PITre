using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.LibroFirma.AddElementoInLF
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "AddElementoInLF"
    /// </summary>
    [DataContract]
    public class AddElementoInLFRequest : Request
    {
        /// <summary>
        /// Id del passo
        /// </summary>
        [DataMember]
        public string IdPasso
        {
            get;
            set;
        }

        /// <summary>
        /// Modalità (Automatica/Manuale)
        /// </summary>
        [DataMember]
        public string Modalita
        {
            get;
            set;
        }

        /// <summary>
        /// Id passo precedente
        /// </summary>
        [DataMember]
        public string IdPassoPrecedente
        {
            get;
            set;
        }

        /// <summary>
        /// People Id del Delegato
        /// </summary>
        [DataMember]
        public string Delegato
        {
            get;
            set;
        }
    }
}