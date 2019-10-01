using System;
using System.Collections.Generic;

namespace DocsPaVO.Report
{
    /// <summary>
    /// Questa classe viene utilizzata per riportare metainformazioni relative ad un report
    /// </summary>
    public class ReportMetadata
    {
        /// <summary>
        /// Nome del report
        /// </summary>
        public String ReportName { get; set; }

        /// <summary>
        /// Chiave identificativa del report
        /// </summary>
        public String ReportKey { get; set; }

        /// <summary>
        /// Campi del report esportabili
        /// </summary>
        public HeaderColumnCollection ExportableFields { get; set; }

    }
}
