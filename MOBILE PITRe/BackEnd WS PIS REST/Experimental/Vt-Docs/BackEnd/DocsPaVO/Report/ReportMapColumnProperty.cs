using System;

namespace DocsPaVO.Report
{
    /// <summary>
    /// Questo oggetto rappresenta il mapping fra un elemento di una sorgente dati
    /// ed una riga del report
    /// </summary>
    [Serializable()]
    public class ReportMapColumnProperty : HeaderProperty
    {
        
        /// <summary>
        /// Valore da esportare
        /// </summary>
        public String Value { get; set; }

    }
}
