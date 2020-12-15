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

    [ReportGenerator(Name="Export Rubrica", ContextName="ExportRubrica", Key="ExportRubrica")]
    public class ExportRubricaReportGeneratorCommand : ReportGeneratorCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(ExportRubricaReportGeneratorCommand));

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

                dataSet = dataExtractor.ExtractData(request);

                report.AddRange(this.CreateReport(dataSet, request));

                response.Document = reportGeneration.GenerateReport(request, report);
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

            report.Title = request.Title;
            report.CreationDate = DateTime.Now;
            report.Subtitle = request.SubTitle;
            report.AdditionalInformation = request.AdditionalInformation;
            report.ReportMapRow = new ReportMapRow();
            report.ReportHeader = new HeaderColumnCollection();
            report.SectionName = "RUBRICA";
            report.ShowHeaderRow = false;
            report.Summary = string.Empty;

            bool store = request.SearchFilters.Where(f => f.argomento == "store").FirstOrDefault().valore.Equals("1") ? true : false;

            // Generazione delle righe del report
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null)
            {
                report.ReportHeader = this.GenerateReportHeader(dataSet, request.ColumnsToExport, store);
                report.ReportMapRow.Rows.AddRange(this.GenerateReportRows(dataSet, report.ReportHeader, store));
            }

            return new List<DocsPaVO.Report.Report>() { report };

        }

        protected HeaderColumnCollection GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport, bool store)
        {

            HeaderColumnCollection header = new HeaderColumnCollection();

            if (store)
            {
                header.Add(new HeaderProperty()
                {
                    ColumnName = "Storicizza",
                    ColumnSize = 60,
                    OriginalName = "STORICIZZA",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
            }

            header.Add(new HeaderProperty()
            {
                ColumnName = "Cod. Registro",
                ColumnSize = 50,
                OriginalName = "VAR_CODICE",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Cod. Rubrica",
                ColumnSize = 80,
                OriginalName = "VAR_COD_RUBRICA",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Cod. Amm.",
                ColumnSize = 60,
                OriginalName = "VAR_CODICE_AMM",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Cod. AOO",
                ColumnSize = 60,
                OriginalName = "VAR_CODICE_AOO",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Tipo",
                ColumnSize = 40,
                OriginalName = "CHA_TIPO_URP",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Descrizione",
                ColumnSize = 180,
                OriginalName = "VAR_DESC_CORR",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Cognome",
                ColumnSize = 70,
                OriginalName = "VAR_COGNOME",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Nome",
                ColumnSize = 70,
                OriginalName = "VAR_NOME",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Indirizzo",
                ColumnSize = 120,
                OriginalName = "VAR_INDIRIZZO",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "CAP",
                ColumnSize = 40,
                OriginalName = "VAR_CAP",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Città",
                ColumnSize = 60,
                OriginalName = "VAR_CITTA",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Provincia",
                ColumnSize = 50,
                OriginalName = "VAR_PROVINCIA",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Nazione",
                ColumnSize = 50,
                OriginalName = "VAR_NAZIONE",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Cod. Fiscale",
                ColumnSize = 80,
                OriginalName = "VAR_COD_FISC",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "P. IVA",
                ColumnSize = 80,
                OriginalName = "VAR_COD_PI",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Tel 1",
                ColumnSize = 60,
                OriginalName = "VAR_TELEFONO",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Tel 2",
                ColumnSize = 60,
                OriginalName = "VAR_TELEFONO2",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Fax",
                ColumnSize = 60,
                OriginalName = "VAR_FAX",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Email",
                ColumnSize = 100,
                OriginalName = "VAR_EMAIL",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Località",
                ColumnSize = 60,
                OriginalName = "VAR_LOCALITA",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Note",
                ColumnSize = 120,
                OriginalName = "VAR_NOTE",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Nuovo registro",
                ColumnSize = 80,
                OriginalName = "NUOVO_REGISTRO",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Canale preferenziale",
                ColumnSize = 100,
                OriginalName = "DESCRIPTION",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            return header;

        }

        protected IEnumerable<ReportMapRowProperty> GenerateReportRows(DataSet dataSet, HeaderColumnCollection reportHeader, bool store)
        {
            List<ReportMapRowProperty> rows = new List<ReportMapRowProperty>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                ReportMapRowProperty reportRow = new ReportMapRowProperty();

                if(store)
                    reportRow.Columns.Add(this.GenerateHeaderColumn("", "Storicizza", "STORICIZZA"));

                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_CODICE"].ToString(), "Cod. Registro", "VAR_CODICE"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_COD_RUBRICA"].ToString(), "Cod. Rubrica", "VAR_COD_RUBRICA"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_CODICE_AMM"].ToString(), "Cod. Amm.", "VAR_CODICE_AMM"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_CODICE_AOO"].ToString(), "Cod. AOO", "VAR_CODICE_AOO"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["CHA_TIPO_URP"].ToString(), "Tipo", "CHA_TIPO_URP"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_DESC_CORR"].ToString(), "Descrizione", "VAR_DESC_CORR"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_COGNOME"].ToString(), "Cognome", "VAR_COGNOME"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_NOME"].ToString(), "Nome", "VAR_NOME"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_INDIRIZZO"].ToString(), "Indirizzo", "VAR_INDIRIZZO"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_CAP"].ToString(), "CAP", "VAR_CAP"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_CITTA"].ToString(), "Città", "VAR_CITTA"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_PROVINCIA"].ToString(), "Provincia", "VAR_PROVINCIA"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_NAZIONE"].ToString(), "Nazione", "VAR_NAZIONE"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_COD_FISC"].ToString(), "Cod. Fiscale", "VAR_COD_FISC"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_COD_PI"].ToString(), "P. IVA", "VAR_COD_PI"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_TELEFONO"].ToString(), "Tel 1", "VAR_TELEFONO"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_TELEFONO2"].ToString(), "Tel 2", "VAR_TELEFONO2"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_FAX"].ToString(), "Fax", "VAR_FAX"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_EMAIL"].ToString(), "Email", "VAR_EMAIL"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_LOCALITA"].ToString(), "Località", "VAR_LOCALITA"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_NOTE"].ToString(), "Note", "VAR_NOTE"));
                reportRow.Columns.Add(this.GenerateHeaderColumn("", "Nuovo Registro", "NUOVO_REGISTRO"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["DESCRIPTION"].ToString(), "Canale preferenziale", "DESCRIPTION"));

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
