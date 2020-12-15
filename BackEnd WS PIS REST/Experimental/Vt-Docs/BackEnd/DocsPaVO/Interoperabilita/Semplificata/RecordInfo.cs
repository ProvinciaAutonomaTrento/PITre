using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Interoperabilita.Semplificata
{
    /// <summary>
    /// Informazioni su un protocollo
    /// </summary>
    public class RecordInfo
    {
        /// <summary>
        /// Codice dell'amministrazione
        /// </summary>
        public String AdministrationCode { get; set; }

        /// <summary>
        /// Codice della AOO
        /// </summary>
        public String AOOCode { get; set; }

        /// <summary>
        /// Numero di protocollo
        /// </summary>
        public String RecordNumber { get; set; }

        /// <summary>
        /// Data di protocollo
        /// </summary>
        public DateTime RecordDate { get; set; }

    }
}
