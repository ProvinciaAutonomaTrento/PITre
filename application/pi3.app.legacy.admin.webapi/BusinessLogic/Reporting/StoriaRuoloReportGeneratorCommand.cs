// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2

using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Report;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;

namespace BusinessLogic.Reporting
{
    /// <summary>
    /// Command per la realizzazione del report relativo alla storia di un ruolo
    /// </summary>
    [ReportGeneratorAttribute(Name = "Storia modifiche apportate ad un ruolo", ContextName = "GestioneRuolo", Key = "StoriaRuolo")]
    class StoriaRuoloReportGeneratorCommand : ReportGeneratorCommand
    {
        /// <summary>
        /// Non è possibile scegliere le colonne da esportare
        /// </summary>
        /// <returns>null</returns>
        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }

        protected override PrintReportResponse GenerateReport(PrintReportRequest request, ISpreadsheetService spreadsheetService, IFileConverterFactory fileConverterPDF, IReportGeneratorService reportGeneratorService)
        {
            IReportDataExtractionBehavior reportDataExtractor = new ObjectTrasformationDataExtractionBehavior();
            IReportGeneratorBehavior formatGenerator = base.CreateReportGenerator(request);

            // Generazione del report
            return base.GenerateReport(request, reportDataExtractor, formatGenerator, spreadsheetService, fileConverterPDF, reportGeneratorService);
        }
    }
}
