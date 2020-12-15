using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.LibroFirma.ClosePassoAndGetNext
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "AddElementoInLF"
    /// </summary>
    [DataContract]
    public class ClosePassoAndGetNextRequest : Request
    {
        /// <summary>
        /// Id dell'istanza del passo di firma
        /// </summary>
        [DataMember]
        public string IdIstanzaPasso
        {
            get;
            set;
        }

        /// <summary>
        /// Id dell'istanza del processo di firma
        /// </summary>
        [DataMember]
        public string IdIstanzaProcesso
        {
            get;
            set;
        }

        /// <summary>
        /// Ordine del Passo
        /// </summary>
        [DataMember]
        public int OrdinePasso
        {
            get;
            set;
        }

        /// <summary>
        /// System id del documento
        /// </summary>
        [DataMember]
        public string IdDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// Version id del documento
        /// </summary>
        [DataMember]
        public string IdVersione
        {
            get;
            set;
        }

        /// <summary>
        /// Data evento
        /// </summary>
        [DataMember]
        public string DataEsecuzione
        {
            get;
            set;
        }

        /// <summary>
        /// Determina se Documento o Allegato
        /// </summary>
        [DataMember]
        public string DocAll
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