using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using BusinessLogic.Reporting.Exceptions;
using DocsPaDB.Query_DocsPAWS;
using DocsPaVO.filtri;
using DocsPaVO.Report;
using log4net;

namespace BusinessLogic.Reporting
{
    [ReportGenerator(Name="Export lista micro", ContextName="AmmExportMacroFunzioni", Key="AmmExportMacroFunzioni")]
    public class AmmExportMacroFunzioniReportGeneratorCommand : ReportGeneratorCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(AmmExportMacroFunzioniReportGeneratorCommand));

        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }

        protected override PrintReportResponse GenerateReport(PrintReportRequest request, IReportDataExtractionBehavior dataExtractor, IReportGeneratorBehavior reportGeneration)
        {

            PrintReportResponse response = null;
            try
            {

                response = new PrintReportResponse();

                // selezione tipo report (micro associate o ruoli/utenti)
                //string key = request.AdditionalInformation;
                //request.AdditionalInformation = string.Empty;
                //request.ReportKey = key;

                DataSet dataSet = new DataSet();

                List<DocsPaVO.Report.Report> report = new List<DocsPaVO.Report.Report>();

                // 1) report micro associate
                request.ReportKey = "AmmExportMacroFunzioni_Micro";
                dataSet = dataExtractor.ExtractData(request);
                report.AddRange(this.CreateReport(dataSet, request));

                // 2) report ruoli/utenti associati
                request.ReportKey = "AmmExportMacroFunzioni_Ruoli";
                dataSet = dataExtractor.ExtractData(request);
                report.AddRange(this.CreateReport(dataSet, request));

                response.Document = reportGeneration.GenerateReport(request, report);

                if (response.Document != null)
                {
                    response.Document.name = String.Format("Report_Tipi_Funzione_{0}.xls", DateTime.Now.ToString("dd-MM-yyyy"));
                    response.Document.fullName = response.Document.name;
                }

            }
            catch (Exception ex)
            {
                logger.Debug("Errore nella generazione del report - ", ex);
                throw new ReportGenerationFailedException(ex.Message);
            }

            return response;
        }

        protected override List<DocsPaVO.Report.Report> CreateReport(DataSet dataSet, PrintReportRequest request)
        {
            DocsPaVO.Report.Report report = new DocsPaVO.Report.Report();

            // Impostazione delle proprietà di base del report
            switch (request.ReportKey)
            {
                case "AmmExportMacroFunzioni_Micro":
                    report.Title = "Dettaglio funzioni associate al tipo funzione";
                    break;

                case "AmmExportMacroFunzioni_Ruoli":
                    report.Title = "Dettaglio ruoli e utenti associati al tipo funzione";
                    break;
            }
            report.CreationDate = DateTime.Now;
            report.Subtitle = request.SubTitle;
            report.AdditionalInformation = request.AdditionalInformation;
            report.ReportMapRow = new ReportMapRow();
            report.ReportHeader = new HeaderColumnCollection();

            // Generazione delle righe del report
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null)
            {
                // Generazione dell'header personalizzato del report
                report.ReportHeader = this.GenerateReportHeader(dataSet, request.ColumnsToExport, request.ReportKey);

                report.ReportMapRow.Rows.AddRange(this.GenerateReportRows(dataSet, report.ReportHeader, request.ReportKey));
            }

            // Costruzione del summary
            report.Summary = String.Format("Righe estratte: {0}", report.ReportMapRow.Rows.Count);

            // Restituzione del report generato
            return new List<DocsPaVO.Report.Report>() { report };
        }

        protected HeaderColumnCollection GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport, string reportType)
        {

            HeaderColumnCollection header = new HeaderColumnCollection();

            switch (reportType)
            {
                case "AmmExportMacroFunzioni_Micro":
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Codice",
                        ColumnSize = 100,
                        OriginalName = "COD_FUNZIONE",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Descrizione",
                        ColumnSize = 500,
                        OriginalName = "VAR_DESC_FUNZIONE",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    break;

                case "AmmExportMacroFunzioni_Ruoli":
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Codice ruolo",
                        ColumnSize = 80,
                        OriginalName = "COD_RUOLO",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Descrizione ruolo",
                        ColumnSize = 200,
                        OriginalName = "DESC_RUOLO",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Cod. UO",
                        ColumnSize = 50,
                        OriginalName = "COD_UO",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Descrizione UO",
                        ColumnSize = 200,
                        OriginalName = "DESC_RUOLO",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Reg. / RF",
                        ColumnSize = 50,
                        OriginalName = "REG_RF",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Utenti",
                        ColumnSize = 400,
                        OriginalName = "UTENTI",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    break;
            }

            return header;
        }

        protected IEnumerable<ReportMapRowProperty> GenerateReportRows(DataSet dataSet, HeaderColumnCollection reportHeader, string reportType)
        {
            List<ReportMapRowProperty> rows = new List<ReportMapRowProperty>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                //creo una nuova riga
                ReportMapRowProperty reportRow = new ReportMapRowProperty();

                switch (reportType)
                {
                    case "AmmExportMacroFunzioni_Micro":
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["COD_FUNZIONE"].ToString(), "Codice", "COD_FUNZIONE"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_DESC_FUNZIONE"].ToString(), "Descrizione", "VAR_DESC_FUNZIONE"));
                        break;

                    case "AmmExportMacroFunzioni_Ruoli":
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["COD_RUOLO"].ToString(), "Codice ruolo", "COD_RUOLO"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["DESC_RUOLO"].ToString(), "Descrizione ruolo", "DESC_RUOLO"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["COD_UO"].ToString(), "Cod. UO", "COD_UO"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["DESC_UO"].ToString(), "Descrizione UO", "DESC_UO"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["REG_RF"].ToString(), "Reg. / RF", "REG_RF"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["UTENTI"].ToString(), "Utenti", "UTENTI"));
                        break;
                }

                rows.Add(reportRow);

            }

            return rows;

        }

        private ReportMapColumnProperty GenerateHeaderColumn(string value, string columnName, string originalName)
        {
            return new ReportMapColumnProperty()
            {
                OriginalName = originalName,
                ColumnName = columnName,
                Value = value
            };
        }
    }
}
