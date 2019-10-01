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

namespace BusinessLogic.Reporting
{
    [ReportGenerator(Name="Export lista micro", ContextName="AmmExportMicroFunzioni", Key="AmmExportMicroFunzioni")]
    public class AmmExportMicroFunzioniReportGeneratorCommand : ReportGeneratorCommand
    {

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

                // i due fogli del report sono stati uniti in un singolo report
                dataSet = dataExtractor.ExtractData(request);
                report.AddRange(this.CreateReport(dataSet, request));

                response.Document = reportGeneration.GenerateReport(request, report);

                if (response.Document != null)
                {
                    response.Document.name = String.Format("Report_Funzioni_{0}.xls", DateTime.Now.ToString("dd-MM-yyyy"));
                    response.Document.fullName = response.Document.name;
                }

            }
            catch (Exception ex)
            {
                throw new ReportGenerationFailedException(ex.Message);
            }

            return response;
        }

        protected override List<DocsPaVO.Report.Report> CreateReport(DataSet dataSet, PrintReportRequest request)
        {
            DocsPaVO.Report.Report report = new DocsPaVO.Report.Report();

            // impostazione delle proprietà di base del report
            report.CreationDate = DateTime.Now;
            report.Title = request.Title;
            report.Subtitle = request.SubTitle;
            report.AdditionalInformation = request.AdditionalInformation;
            report.ReportMapRow = new ReportMapRow();
            report.ReportHeader = new HeaderColumnCollection();

            // generazione delle righe del report
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null)
            {
                // Generazione dell'header personalizzato del report
                report.ReportHeader = this.GenerateReportHeader(dataSet, request.ColumnsToExport);

                report.ReportMapRow.Rows.AddRange(this.GenerateReportRows(dataSet, report.ReportHeader));
            }

            // Costruzione del summary
            report.Summary = String.Format("Righe estratte: {0}", report.ReportMapRow.Rows.Count);

            return new List<DocsPaVO.Report.Report>() { report };
        }

        protected HeaderColumnCollection GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport)
        {
            HeaderColumnCollection header = new HeaderColumnCollection();

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
                ColumnName = "Cod. tipo funzione",
                ColumnSize = 100,
                OriginalName = "COD_TIPO_FUNZ",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "Descrizione tipo funzione",
                ColumnSize = 250,
                OriginalName = "DESC_TIPO_FUNZ",    
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
            return header;
        }

        protected IEnumerable<ReportMapRowProperty> GenerateReportRows(DataSet dataSet, HeaderColumnCollection reportHeader)
        {
            List<ReportMapRowProperty> rows = new List<ReportMapRowProperty>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                //creo una nuova riga
                ReportMapRowProperty reportRow = new ReportMapRowProperty();

                reportRow.Columns.Add(this.GenerateHeaderColumn(row["COD_RUOLO"].ToString(), "Codice ruolo", "COD_RUOLO"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["DESC_RUOLO"].ToString(), "Descrizione ruolo", "DESC_RUOLO"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["COD_TIPO_FUNZ"].ToString(), "Cod. tipo funzione", "COD_TIPO_FUNZ"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["DESC_TIPO_FUNZ"].ToString(), "Descrizione tipo funzione", "DESC_TIPO_FUNZ"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["COD_UO"].ToString(), "Cod. UO", "COD_UO"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["DESC_UO"].ToString(), "Descrizione UO", "DESC_UO"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["REG_RF"].ToString(), "Reg. / RF", "REG_RF"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["UTENTI"].ToString(), "Utenti", "UTENTI"));

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
