// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Report;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using BusinessLogic.Reporting.Exceptions;
using System.Data;

namespace BusinessLogic.Reporting
{
    /// <summary>
    /// Report relativo alla modifica di un ruolo
    /// </summary>
    [ReportGeneratorAttribute(Name = "Modifica ruolo", ContextName = "ModificaRuolo", Key = "ModificaRuolo")]
    class ModificaRuoloReportGeneratorCommand : ReportGeneratorCommand
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

