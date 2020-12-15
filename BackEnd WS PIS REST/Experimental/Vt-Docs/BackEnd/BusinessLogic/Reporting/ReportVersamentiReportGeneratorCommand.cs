using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using BusinessLogic.Reporting.Exceptions;
using DocsPaDB.Query_DocsPAWS;
using DocsPaVO.Report;
using log4net;

namespace BusinessLogic.Reporting
{
    [ReportGenerator(Name="Report Versamenti PARER", ContextName="ReportVersamentiPARER", Key="ReportVersamentiPARER")]
    public class ReportVersamentiReportGeneratorCommand : ReportGeneratorCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(ReportVersamentiReportGeneratorCommand));

        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }

        protected override List<DocsPaVO.Report.Report> CreateReport(System.Data.DataSet dataSet, PrintReportRequest request)
        {
            DocsPaVO.Report.Report report = new DocsPaVO.Report.Report();
            string tipo = request.SearchFilters.Where(f => f.argomento.Equals("stato")).FirstOrDefault().valore;
            string idAmm = request.SearchFilters.Where(f => f.argomento.Equals("idAmm")).FirstOrDefault().valore;

            DocsPaVO.amministrazione.InfoAmministrazione amm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(idAmm);
            string ammString = string.Format("AMMINISTRAZIONE - Codice: {0}, Descrizione: {1}\r\n", amm.Codice, amm.Descrizione);

            if (tipo.Equals("R"))
            {
                report.Title = "Report sui documenti in stato \"Rifiutato\" ";
                report.Subtitle = ammString + string.Format("I seguenti documenti versati in conservazione il giorno {0} sono stati rifiutati dal sistema di conservazione", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"));
            }
            else
            {
                report.Title = "Report sui documenti in stato \"Versamento fallito\" ";
                report.Subtitle = ammString + string.Format("I seguenti documenti risultano essere in stato \"Versamento fallito\" alla data del {0}", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"));
            }
            report.AdditionalInformation = request.AdditionalInformation;
            report.ReportMapRow = new ReportMapRow();
            report.ReportHeader = new HeaderColumnCollection();

            // Generazione delle righe del report
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null)
            {
                report.ReportHeader = this.GenerateReportHeader(dataSet, request.ColumnsToExport, tipo);
                report.ReportMapRow.Rows.AddRange(this.GenerateReportRows(dataSet, report.ReportHeader, tipo));
            }

            return new List<DocsPaVO.Report.Report>() { report };
        }

        protected HeaderColumnCollection GenerateReportHeader(System.Data.DataSet dataSet, HeaderColumnCollection fieldsToExport, string tipo)
        {
            HeaderColumnCollection header = new HeaderColumnCollection();
            header.Add(new HeaderProperty()
            {
                ColumnName = "ID Doc. / Num. Protocollo",
                ColumnSize = 50,
                OriginalName = "ID",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "Tipo",
                ColumnSize = 100,
                OriginalName = "TIPO",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "Data creazione / protocollazione",
                ColumnSize = 100,
                OriginalName = "DATA",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "Oggetto",
                ColumnSize = 200,
                OriginalName = "OGGETTO",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            if (tipo.Equals("F"))
            {
                header.Add(new HeaderProperty()
                {
                    ColumnName = "Data ultimo versamento",
                    ColumnSize = 100,
                    OriginalName = "DATA_ULTIMO_VERS",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
            }
            header.Add(new HeaderProperty()
            {
                ColumnName = "Codice Policy",
                ColumnSize = 100,
                OriginalName = "COD_POLICY",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "N. esecuzione Policy",
                ColumnSize = 50,
                OriginalName = "NUM_POLICY",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            if (tipo.Equals("R"))
            {
                header.Add(new HeaderProperty()
                {
                    ColumnName = "Messaggio di rifiuto",
                    ColumnSize = 300,
                    OriginalName = "MESS_RIFIUTO",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
            }
            

            return header;
        }

        protected IEnumerable<ReportMapRowProperty> GenerateReportRows(System.Data.DataSet dataSet, HeaderColumnCollection reportHeader, string tipo)
        {
            List<ReportMapRowProperty> rows = new List<ReportMapRowProperty>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                ReportMapRowProperty reportRow = new ReportMapRowProperty();

                if(!string.IsNullOrEmpty(row["NUM_PROTO"].ToString()))
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["NUM_PROTO"].ToString(), "Id Doc. / Num. Protocollo", "ID"));
                else
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["ID_PROFILE"].ToString(), "Id Doc. / Num. Protocollo", "ID"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetTipoDocumento(row["CHA_TIPO_PROTO"].ToString()), "Tipo", "TIPO"));
                if (!string.IsNullOrEmpty(row["DATA_PROTO"].ToString()))
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["DATA_PROTO"].ToString(), "Data creazione / protocollazione", "DATA"));
                else
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["DATA_CREAZIONE"].ToString(), "Data creazione / protocollazione", "DATA"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_PROF_OGGETTO"].ToString(), "Oggetto", "OGGETTO"));
                if (tipo.Equals("F"))
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["DATA_VERSAMENTO"].ToString(), "Data ultimo versamento", "DATA_ULTIMO_VERS"));
                if (!string.IsNullOrEmpty(row["VAR_CODICE"].ToString()))
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_CODICE"].ToString(), "Codice Policy", "COD_POLICY"));
                else
                    reportRow.Columns.Add(this.GenerateHeaderColumn("-", "Codice Policy", "COD_POLICY"));
                if (!string.IsNullOrEmpty(row["NUM_ESECUZIONE_POLICY"].ToString()))
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["NUM_ESECUZIONE_POLICY"].ToString(), "N. esecuzione Policy", "NUM_POLICY"));
                else
                    reportRow.Columns.Add(this.GenerateHeaderColumn("-", "N. esecuzione Policy", "NUM_POLICY"));
                if (tipo.Equals("R"))
                    reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetMessaggioRifiuto(row["VAR_FILE_RISPOSTA"].ToString()), "Messaggio di rifiuto", "MESS_RIFIUTO"));

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

        private string GetTipoDocumento(string tipo)
        {
            string retVal = string.Empty;

            switch (tipo)
            {
                case "A":
                    retVal = "Arrivo";
                    break;
                case "P":
                    retVal = "Partenza";
                    break;
                case "I":
                    retVal = "Interno";
                    break;
                case "G":
                    retVal = "Non protocollato";
                    break;
                case "R":
                    retVal = "Stampa registro di protocollo";
                    break;
                case "C":
                    retVal = "Stampa registro di repertorio";
                    break;
            }

            return retVal;
        }

        private string GetMessaggioRifiuto(string xmlString)
        {
            string result = string.Empty;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            XmlElement el = (XmlElement)xmlDoc.SelectSingleNode("EsitoVersamento/EsitoGenerale/MessaggioErrore");
            if (el != null)
            {
                result = el.InnerText.Trim();
            }

            return result;
        }
    }
}
