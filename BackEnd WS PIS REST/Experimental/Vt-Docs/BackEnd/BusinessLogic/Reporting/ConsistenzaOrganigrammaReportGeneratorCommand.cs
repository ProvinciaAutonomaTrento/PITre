
using DocsPaVO.Report;
using System.Data;
using System;
using BusinessLogic.Reporting.Exceptions;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using System.Collections.Generic;
namespace BusinessLogic.Reporting
{
    /// <summary>
    /// Classe per la generazione del report sulla consistenza dell'organigramma
    /// </summary>
    [ReportGeneratorAttribute(Name = "Report consistenza organigramma", ContextName = "ConsistenzaOrganigramma", Key = "ConsistenzaOrganigramma")]
    public class ConsistenzaOrganigrammaReportGeneratorCommand : ReportGeneratorCommand
    {

        /// <summary>
        /// Metodo per la generazione del report sulla consistenza dell'organigramma. Il report prodotto sarà un worksheet Excel
        /// costituito da 7 fogli con i seguenti sotto report:
        /// <ol>
        ///     <li>Ruoli Disabilitati (codice, descrizione, UO)</li>
        ///     <li>Ruoli inibiti alla ricezione di trasmissioni (codice, descrizione , UO)</li>
        ///     <li>Ruoli senza utenti(codice, descrizione, UO)</li>
        ///     <li>Ruoli senza registri o RF (codice, descrizione, UO)</li>
        ///     <li>Ruoli senza funzioni (codice, descrizione, UO)</li>
        ///     <li>UO senza ruoli (codice, descrizione, UO Padre)</li>
        ///     <li>UO senza ruoli di riferimento (codice descrizione, UO Padre)</li>
        /// </ol>
        /// </summary>
        /// <param name="request">Informazioni sul report da generare</param>
        /// <param name="dataExtractor">Estrattore dei dati</param>
        /// <param name="reportGeneration">Behavior responsabile della generazione del report</param>
        /// <returns>Report</returns>
        protected override PrintReportResponse GenerateReport(PrintReportRequest request, IReportDataExtractionBehavior dataExtractor, IReportGeneratorBehavior reportGeneration)
        {
            PrintReportResponse response = null;

            try
            {
                // Lista dei report da esportare
                List<DocsPaVO.Report.Report> reports = new List<DocsPaVO.Report.Report>();

                // Response da restituire
                response = new PrintReportResponse();

                // Generazione del report relativo ai ruoli disabilitati (essengo il primo report, viene scritto anche
                // il titolo
                reports.AddRange(this.GetReport(dataExtractor, request, "DisabledRoles", 
                    String.Format("{0}\n{1}", request.Title, "Ruoli disabilitati"), "Foglio 1 di 7"));

                // Generazione del report relativo ai ruoli inibiti alla ricezione di trasmissioni
                reports.AddRange(this.GetReport(dataExtractor, request, "InhibitedRoles", "Ruoli inibiti alla ricezione trasmissioni", "Foglio 2 di 7"));

                // Generazione del report relativo ai ruoli senza utenti
                reports.AddRange(this.GetReport(dataExtractor, request, "RolesWOutUsers", "Ruoli senza utenti", "Foglio 3 di 7"));

                // Generazione del report relativo ai ruoli senza registri o rf
                reports.AddRange(this.GetReport(dataExtractor, request, "RolesWOutRegsOrRF", "Ruoli senza registri o rf", "Foglio 4 di 7"));

                // Generazione del report relativo ai ruoli senza funzioni
                reports.AddRange(this.GetReport(dataExtractor, request, "RolesWOutFunctions", "Ruoli senza funzioni", "Foglio 5 di 7"));

                // Generazione del report relativo alle UO senza ruoli
                reports.AddRange(this.GetReport(dataExtractor, request, "UOWOutRoles", "UO senza ruoli", "Foglio 6 di 7"));

                // Generazione del report relativo alle UO senza ruoli di riferimento
                reports.AddRange(this.GetReport(dataExtractor, request, "UOWOutRF", "UO senza ruoli di riferimento", "Foglio 7 di 7"));

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
        /// Per questa tipologia di report, non è possibile decidere i campi da esportare
        /// </summary>
        /// <returns>Null</returns>
        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }
    }
}
