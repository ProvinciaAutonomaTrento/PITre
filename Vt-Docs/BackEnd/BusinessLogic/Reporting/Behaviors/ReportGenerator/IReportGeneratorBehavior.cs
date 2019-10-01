using System.Data;
using DocsPaVO.documento;
using DocsPaVO.Report;
using System.Collections.Generic;

namespace BusinessLogic.Reporting.Behaviors.ReportGenerator
{
    /// <summary>
    /// Questa interfaccia definisce il contratto che deve essere implementato da una
    /// classe che desidera realizzare un behavior per la generazione di un file di
    /// report.
    /// </summary>
    public interface IReportGeneratorBehavior
    {
        /// <summary>
        /// Creazione del report nel formato richiesto
        /// </summary>
        /// <param name="request">Informazioni sull'azione da compiere</param>
        /// <param name="reports">Lista dei report da esportare</param>
        /// <returns>Oggetto contenente il file fisico del report</returns>
        FileDocumento GenerateReport(PrintReportRequest request, List<DocsPaVO.Report.Report> reports);
    }
}
