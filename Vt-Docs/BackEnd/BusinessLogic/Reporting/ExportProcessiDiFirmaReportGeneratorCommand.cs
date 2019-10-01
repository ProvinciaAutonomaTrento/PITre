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
    [ReportGenerator(Name = "Export processi firma", ContextName = "ExportProcessiDiFirma", Key = "ExportProcessiDiFirma")]
    [ReportGenerator(Name = "Report processi invalidati", ContextName = "ExportProcessiDiFirmaInvalidati", Key = "ExportProcessiDiFirmaInvalidati")]
    public class ExportProcessiDiFirmaReportGeneratorCommand : ReportGeneratorCommand
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
                if (request.ContextName.Equals("ExportProcessiDiFirma"))
                {
                    // 1) report processi di firma
                    request.ReportKey = "ExportProcessiFirma";
                    dataSet = dataExtractor.ExtractData(request);
                    report.AddRange(this.CreateReport(dataSet, request));

                    // 2) report istanza di processi di firma
                    request.ReportKey = "ExportIstanzeProcessiFirma";
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

                        response.Document.name = String.Format("Report_Processi_Firma_{0}." + ext, DateTime.Now.ToString("dd-MM-yyyy"));
                        response.Document.fullName = response.Document.name;
                    }
                }
                else
                {
                    dataSet = dataSet = dataExtractor.ExtractData(request);
                    report.AddRange(this.CreateReport(dataSet, request));

                    response.Document = reportGeneration.GenerateReport(request, report);

                    string ext = string.Empty;
                    switch (request.ReportType)
                    {
                        case ReportTypeEnum.PDF:
                            ext = "pdf";
                            break;
                        case ReportTypeEnum.Excel:
                            ext = "xls";
                            break;
                    }
                    if (response.Document != null)
                    {
                        response.Document.name = String.Format("Report_Processi_Firma_Invalidati_{0}." + ext , DateTime.Now.ToString("dd-MM-yyyy"));
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
                case "ExportProcessiFirma":
                    report.Title = "Dettaglio processi di firma";
                    break;

                case "ExportIstanzeProcessiFirma":
                    report.Title = "Dettaglio istanza processi di firma";
                    break;
                case "ExportProcessiDiFirmaInvalidati":
                    report.Title = "Elenco processi/modelli di firma Invalidati";
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
                case "ExportProcessiFirma":
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Nome processo",
                        ColumnSize = 400,
                        OriginalName = "NOME_PROCESSO",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Utente Disegnatore",
                        ColumnSize = 100,
                        OriginalName = "DESC_UTENTE_CREATORE",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Ruolo Disegnatore",
                        ColumnSize = 300,
                        OriginalName = "DESC_RUOLO_CREATORE",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Diagramma stato",
                        ColumnSize = 400,
                        OriginalName = "DESC_DIAGRAMMA_STATO",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    break;

                case "ExportIstanzeProcessiFirma":
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Nome Processo",
                        ColumnSize = 400,
                        OriginalName = "NOME_PROCESSO",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Utente Proponente",
                        ColumnSize = 100,
                        OriginalName = "DESC_UTENTE_PROPONENTE",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Ruolo Proponente",
                        ColumnSize = 300,
                        OriginalName = "DESC_RUOLO_PROPONENTE",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Data Avvio",
                        ColumnSize = 90,
                        OriginalName = "DATO_AVVIO",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Id. Documento",
                        ColumnSize = 70,
                        OriginalName = "ID_DOCUMENTO",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Tipo documento",
                        ColumnSize = 70,
                        OriginalName = "TIPO_DOCUMENTO",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    break;
                case "ExportProcessiDiFirmaInvalidati":
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Nome",
                        ColumnSize = 400,
                        OriginalName = "NOME_PROCESSO",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Disegnatore",
                        ColumnSize = 400,
                        OriginalName = "CREATORE",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                    header.Add(new HeaderProperty()
                    {
                        ColumnName = "Titolare modificato",
                        ColumnSize = 400,
                        OriginalName = "TITOLARE",
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
                    case "ExportProcessiFirma":
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["NOME_PROCESSO"].ToString(), "Nome processo", "NOME_PROCESSO"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["DESC_UTENTE_CREATORE"].ToString(), "Utente Disegnatore", "DESC_UTENTE_CREATORE"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["DESC_RUOLO_CREATORE"].ToString(), "Ruolo Disegnatore", "DESC_RUOLO_CREATORE"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["DESC_DIAGRAMMA_STATO"].ToString(), "Diagramma stato", "DESC_DIAGRAMMA_STATO"));
                        break;

                    case "ExportIstanzeProcessiFirma":
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["NOME_PROCESSO"].ToString(), "Nome Processo", "NOME_PROCESSO"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["DESC_UTENTE_PROPONENTE"].ToString(), "Utente Proponente", "DESC_UTENTE_PROPONENTE"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["DESC_RUOLO_PROPONENTE"].ToString(), "Ruolo Proponente", "DESC_RUOLO_PROPONENTE"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["DATO_AVVIO"].ToString(), "Data Avvio", "DATO_AVVIO"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["ID_DOCUMENTO"].ToString(), "Id. Documento", "ID_DOCUMENTO"));
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["DOC_ALL"].ToString().Equals("D") ? "Documento" : "Allegato", "Tipo documento", "TIPO_DOCUMENTO"));
                        break;
                    case "ExportProcessiDiFirmaInvalidati":
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["NOME_PROCESSO"].ToString(), "Nome Processo", "NOME_PROCESSO"));

                        string creatore = row["UTENTE_CREATORE"].ToString() + " (" + row["RUOLO_CREATORE"].ToString() + ")";
                        reportRow.Columns.Add(this.GenerateHeaderColumn(creatore, "Disegnatore", "CREATORE"));

                        string titolare = string.Empty;
                        if (!string.IsNullOrEmpty(row["UTENTE_COINVOLTO"].ToString()) && !string.IsNullOrEmpty(row["RUOLO_COINVOLTO"].ToString()))
                            titolare = row["UTENTE_COINVOLTO"].ToString() + " (" + row["RUOLO_COINVOLTO"].ToString() + ")";
                        else if (!string.IsNullOrEmpty(row["UTENTE_COINVOLTO"].ToString()))
                            titolare = row["UTENTE_COINVOLTO"].ToString();
                        else if (!string.IsNullOrEmpty(row["RUOLO_COINVOLTO"].ToString()))
                            titolare = row["RUOLO_COINVOLTO"].ToString();
                        reportRow.Columns.Add(this.GenerateHeaderColumn(titolare, "Titolare modificato", "TITOLARE"));
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
