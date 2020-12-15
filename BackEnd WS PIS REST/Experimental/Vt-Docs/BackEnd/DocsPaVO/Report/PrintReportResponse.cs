using System;
using DocsPaVO.documento;
using System.Collections.Generic;

namespace DocsPaVO.Report
{
    /// <summary>
    /// Questo oggetto rappresenta la risposta ricevuta dal servizio di generazione report.
    /// Nota per gli sviluppatori: Chi volesse creare una response personalizzata e specifica
    /// per un determinato caso d'uso, può estendere questa classe.
    /// </summary>
    [Serializable()]
    public class PrintReportResponse
    {
        /// <summary>
        /// Informazioni sul documento creato
        /// </summary>
        public FileDocumento Document { get; set; }

        /// <summary>
        /// Metadati relativi ai report registrati nel sistema
        /// </summary>
        public List<ReportMetadata> ReportMetadata { get; set; }

    }
}
