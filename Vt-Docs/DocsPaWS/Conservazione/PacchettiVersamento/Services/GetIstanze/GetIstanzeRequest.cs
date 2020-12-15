using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.GetIstanze
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetIstanze"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetIstanzeRequest")]
    public class GetIstanzeRequest : Request
    {
        /// <summary>
        /// Filtro per consentire il reperimento delle istanze di conservazione in stato "DaInviare"
        /// </summary>
        public bool GetDaInviare
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per consentire il reperimento delle istanze di conservazione in stato "Inviate"
        /// </summary>
        public bool GetInviate
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per consentire il reperimento delle istanze di conservazione in stato "Rifiutate"
        /// </summary>
        public bool GetRifiutate
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per consentire il reperimento delle istanze di conservazione in stato "InLavorazione"
        /// </summary>
        public bool GetInLavorazione
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per consentire il reperimento delle istanze di conservazione in stato "Firmate"
        /// </summary>
        public bool GetFirmate
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per consentire il reperimento delle istanze di conservazione in stato "Chiuse"
        /// </summary>
        public bool GetChiuse
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per consentire il reperimento delle istanze di conservazione in stato "Conservate"
        /// </summary>
        public bool GetConservate
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per consentire il reperimento delle istanze di conservazione in stato "Errore"
        /// </summary>
        public bool GetErrore
        {
            get;
            set;
        }
    }
}