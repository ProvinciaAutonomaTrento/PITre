using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using BusinessLogic.Reporting.Exceptions;
using DocsPaVO.Report;
using log4net;

namespace BusinessLogic.Reporting
{
    [ReportGenerator(Name = "Export trasmissioni pendenti", ContextName = "ExportTrasmissioniPendenti", Key = "ExportTrasmissioniPendenti")]
    public class ExportTrasmissioniPendentiUtenteReportGeneratorCommand : ReportGeneratorCommand
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

                DataSet dataSet = new DataSet();

                List<DocsPaVO.Report.Report> report = new List<DocsPaVO.Report.Report>();
                if (request.ContextName.Equals("ExportTrasmissioniPendenti"))
                {

                    request.ReportKey = "ExportTrasmissioniPendentiDoc";
                    dataSet = dataExtractor.ExtractData(request);
                    report.AddRange(this.CreateReport(dataSet, request));

                    request.ReportKey = "ExportTrasmissioniPendentiFasc";
                    dataSet = dataExtractor.ExtractData(request);
                    report.AddRange(this.CreateReport(dataSet, request));

                    response.Document = reportGeneration.GenerateReport(request, report);

                    if (response.Document != null)
                    {
                        string ext = string.Empty;
                        switch (request.ReportType)
                        {
                            case ReportTypeEnum.PDF:
                                ext = "pdf";
                                break;
                            case ReportTypeEnum.Excel:
                                ext = "xls";
                                break;
                            case ReportTypeEnum.ODS:
                                ext = "ods";
                                break;
                        }

                        response.Document.name = String.Format("Report_Trasmissioni_Pendenti{0}." + ext, DateTime.Now.ToString("dd-MM-yyyy"));
                        response.Document.fullName = response.Document.name;
                    }
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
                case "ExportTrasmissioniPendentiDoc":
                    report.Title = "Dettaglio trasmissioni pendenti documenti";
                    report.SectionName = "Documenti";
                    break;
                case "ExportTrasmissioniPendentiFasc":
                    report.Title = "Dettaglio trasmissioni pendenti fascicoli";
                    report.SectionName = "Fascicoli";
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
                case "ExportTrasmissioniPendentiDoc":
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Trasm. il",
                        ColumnSize = 100,
                        OriginalName = "DTA_INVIO",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Mittente Trasm. (Ruolo)",
                        ColumnSize = 300,
                        OriginalName = "MITTENTE_TRASM",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Ragione",
                        ColumnSize = 200,
                        OriginalName = "VAR_DESC_RAGIONE",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Scadenza",
                        ColumnSize = 100,
                        OriginalName = "DTA_SCADENZA",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Doc.",
                        ColumnSize = 100,
                        OriginalName = "ID_DOCUMENTO",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Segnatura",
                        ColumnSize = 200,
                        OriginalName = "VAR_SEGNATURA",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Oggetto",
                        ColumnSize = 400,
                        OriginalName = "OGGETTO",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Mittente",
                        ColumnSize = 400,
                        OriginalName = "MITTENTE",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    break;
                case "ExportTrasmissioniPendentiFasc":
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Trasm. il",
                        ColumnSize = 100,
                        OriginalName = "DTA_INVIO",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Mittente Trasm. (Ruolo)",
                        ColumnSize = 300,
                        OriginalName = "MITTENTE_TRASM",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Ragione",
                        ColumnSize = 200,
                        OriginalName = "VAR_DESC_RAGIONE",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Scadenza",
                        ColumnSize = 100,
                        OriginalName = "DTA_SCADENZA",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Descrizione",
                        ColumnSize = 400,
                        OriginalName = "DESCRIPTION",
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
                    case "ExportTrasmissioniPendentiDoc":
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["DTA_INVIO"].ToString(), "Trasm. il", "DTA_INVIO"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["MITTENTE_TRASM"].ToString(), "Mittente Trasm. (Ruolo)", "MITTENTE_TRASM"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_DESC_RAGIONE"].ToString(), "Ragione", "VAR_DESC_RAGIONE"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["DTA_SCADENZA"].ToString(), "Scadenza", "DTA_SCADENZA"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["ID_DOCUMENTO"].ToString(), "Doc.", "ID_DOCUMENTO"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_SEGNATURA"].ToString(), "Segnatura", "VAR_SEGNATURA"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["OGGETTO"].ToString(), "Oggetto", "OGGETTO"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["MITTENTE"].ToString(), "Mittente", "MITTENTE"));
                        break;
                    case "ExportTrasmissioniPendentiFasc":
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["DTA_INVIO"].ToString(), "Trasm. il", "DTA_INVIO"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["MITTENTE_TRASM"].ToString(), "Mittente Trasm. (Ruolo)", "MITTENTE_TRASM"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_DESC_RAGIONE"].ToString(), "Ragione", "VAR_DESC_RAGIONE"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["DTA_SCADENZA"].ToString(), "Scadenza", "DTA_SCADENZA"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["DESCRIPTION"].ToString(), "Descrizione", "DESCRIPTION"));
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
