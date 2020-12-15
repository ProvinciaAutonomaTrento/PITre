using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class RicercaRequest : Request
    {
        /// <summary>
        /// Tipi ricerca
        /// </summary>
        public class TipiRicercaDescrizione
        {
            /// <summary>
            /// 
            /// </summary>
            public const string UO = "U";

            /// <summary>
            /// 
            /// </summary>
            public const string RUOLO = "R";

            /// <summary>
            /// 
            /// </summary>
            public const string UTENTE_NOME = "PN";

            /// <summary>
            /// 
            /// </summary>
            public const string UTENTE_COGNOME = "PC";
        }

        /// <summary>
        /// Id amministrazione
        /// </summary>
        public string IdAmministrazione
        {
            get;
            set;
        }

        /// <summary>
        /// Chiave di ricerca
        /// </summary>
        /// <remarks>
        /// Identificata da una delle costanti della classe "TipiRicerca"
        /// </remarks>
        public string TipoRicercaDescrizione
        {
            get;
            set;
        }

        /// <summary>
        /// Chiave di ricerca per descrizione elemento
        /// </summary>
        public string Descrizione
        {
            get;
            set;
        }

        /// <summary>
        /// Chiave di ricerca elemento per codice elemento
        /// </summary>
        public string Codice
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se ricercare un elemento per il codice esatto
        /// </summary>
        public bool RicercaPerCodiceEsatto
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se ricercare gli elementi storicizzati
        /// </summary>
        public bool RicercaStoricizzati
        {
            get;
            set;
        }
    }
}
