using System;
using System.Collections.Generic;
using System.Data;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using BusinessLogic.Reporting.Exceptions;
using DocsPaVO.Report;

namespace BusinessLogic.Reporting
{
    /// <summary>
    /// Report per l'estrazione dei ruoli disabilitati o inibiti
    /// </summary>
    [ReportGeneratorAttribute(Name = "Ruoli disabilitati o inibiti alla ricezione di trasmissioni", ContextName = "ModelliTrasmissione", Key="RuoliDisabilitatiInibiti")]
    [ReportGeneratorAttribute(Name = "Ruoli disabilitati o inibiti alla ricezione di trasmissioni", ContextName = "ModelliTrasmissioneUtente", Key = "RuoliDisabilitatiInibiti")]
    public class RuoliDisabilitatiInibitiReportGeneratorCommand : ReportGeneratorCommand
    {

        /// <summary>
        /// Generazione del report con la lista dei ruoli disabilitati o inibiti. Il report prodotto sarà un worksheet Excel
        /// costituito da 2 fogli con i seguenti sotto report:
        /// <ol>
        ///     <li>Ruoli Disabilitati (codice, descrizione, UO)</li>
        ///     <li>Ruoli inibiti alla ricezione di trasmissioni (codice, descrizione , UO)</li>
        /// </ol>
        /// </summary>
        /// <param name="request">Informazioni sul report da generare</param>
        /// <param name="dataExtractor">Estrattore dei dati</param>
        /// <param name="reportGeneration">Behavior costruttore del report</param>
        /// <returns>Report prodotto</returns>
        protected override PrintReportResponse GenerateReport(PrintReportRequest request, IReportDataExtractionBehavior dataExtractor, IReportGeneratorBehavior reportGeneration)
        {
            PrintReportResponse response = null;

            try
            {
                // Lista dei report da esportare
                List<DocsPaVO.Report.Report> reports = new List<DocsPaVO.Report.Report>();

                // Response da restituire
                response = new PrintReportResponse();

                // Generazione del report relativo ai ruoli disabilitati
                reports.AddRange(this.GetReport(dataExtractor, request, "DisabledRoles",
                    String.Format("{0}\n{1}", request.Title, "Ruoli disabilitati"), "Foglio 1 di 2"));

                // Generazione del report relativo ai ruoli inibiti alla ricezione di trasmissioni
                reports.AddRange(this.GetReport(dataExtractor, request, "InhibitedRoles", "Ruoli inibiti alla ricezione trasmissioni", "Foglio 2 di 2"));

                // Generazione file report
                response.Document = reportGeneration.GenerateReport(request, reports);
            }
            catch (Exception ex)
            {
                throw new ReportGenerationFailedException(ex.Message);
            }

            // Restituzione del report
            return response;

        }

        /// <summary>
        /// Metodo per la generazione di un report
        /// </summary>
        /// <param name="dataExtractor">Estrattore dei dati</param>
        /// <param name="request">Informazioni sul report da generare</param>
        /// <param name="contextName">Nome del contesto di estrazione</param>
        /// <param name="reportTitle">Titolo del report</param>
        /// <param name="reportSubtitle">Sotto titolo da assegnare al report</param>
        /// <returns>Report da esportare</returns>
        private List<DocsPaVO.Report.Report> GetReport(
            IReportDataExtractionBehavior dataExtractor,
            PrintReportRequest request,
            String contextName,
            String reportTitle,
            String reportSubtitle)
        {
            // Impostazione di titolo e sottotitolo del report
            request.Title = reportTitle;
            request.SubTitle = reportSubtitle;

            // Estrazione dati
            DataSet dataSet = this.GetReportData(dataExtractor, request, contextName);

            // Generazione del report
            List<DocsPaVO.Report.Report> report = base.CreateReport(dataSet, request);

            // Restituzione del report creato
            return report;

        }

        /// <summary>
        /// Metodo per l'estrazione dei dati relativi ad un determinato contesto
        /// </summary>
        /// <param name="dataExtractor">Estrattore dei dati</param>
        /// <param name="request">Informazioni sul report da generare</param>
        /// <param name="contextName">Nome del contesto</param>
        /// <returns>DataSet con i dati da esportare</returns>
        private DataSet GetReportData(IReportDataExtractionBehavior dataExtractor, PrintReportRequest request, String contextName)
        {
            // Impostazione del contesto
            request.ReportKey = contextName;

            // Generazione del report
            return dataExtractor.ExtractData(request);
        }

        /// <summary>
        /// Per questo report non è possibile scegliere le colonne da esportare
        /// </summary>
        /// <returns>Null</returns>
        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }
    }
}
