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
    [ReportGenerator(Name="Log Conservazione", ContextName="LogConservazione", Key="LogConservazione")]
    public class ExportLogConservazioneReportGeneratorCommand : ReportGeneratorCommand
    {
        protected override DocsPaVO.Report.HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }

        protected override PrintReportResponse GenerateReport(PrintReportRequest request, IReportDataExtractionBehavior dataExtractor, IReportGeneratorBehavior reportGeneration)
        {

            PrintReportResponse response = null;

            try
            {
                //personalizzo il report
                request.Title = String.Format("{0} - Log Conservazione", Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(request.UserInfo.idAmministrazione).Descrizione);
                request.SubTitle = string.Format("Data generazione report: {0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                request.AdditionalInformation = this.ReportSummary(request.SearchFilters);
                

                //istanzio la response da restituire
                response = new PrintReportResponse();

                // Recupero dati
                DataSet dataSet = dataExtractor.ExtractData(request);

                // Generazione dati per il report
                List<DocsPaVO.Report.Report> report = this.CreateReport(dataSet, request);

                // Generazione file report
                response.Document = reportGeneration.GenerateReport(request, report);

            }
            catch(Exception ex)
            {
                throw new ReportGenerationFailedException(ex.Message);
            }

            // Restituzione del report
            return response;
        }

        protected override HeaderColumnCollection GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport)
        {

            HeaderColumnCollection header = new HeaderColumnCollection();
            //header = base.GenerateReportHeaderFromDataSet(dataSet);

            //aggiunta colonne per report
            header.Add(new HeaderProperty()
            {
                ColumnName = "ISTANZA",
                ColumnSize = 50,
                OriginalName = "id_oggetto",
                Export = true,
                DataType = HeaderProperty.ContentDataType.Integer
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "UTENTE",
                ColumnSize = 120,
                OriginalName = "userid_operatore",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "DATA AZIONE",
                ColumnSize = 120,
                OriginalName = "dta_azione",
                Export = true,
                DataType = HeaderProperty.ContentDataType.DateTime
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "ESITO",
                ColumnSize = 50,
                OriginalName = "cha_esito",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "AZIONE",
                ColumnSize = 350,
                OriginalName = "var_desc_oggetto",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            

            return header;
        }

        protected override IEnumerable<ReportMapRowProperty> GenerateReportRows(DataSet dataSet, HeaderColumnCollection reportHeader)
        {

            List<ReportMapRowProperty> rows = new List<ReportMapRowProperty>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                //creo una nuova riga
                ReportMapRowProperty reportRow = new ReportMapRowProperty();
                
                //aggiungo le colonne da inserire nel report dei log

                //campi istanza, utente e data azione:
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["id_oggetto"].ToString(), "ISTANZA", "id_oggetto"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["userid_operatore"].ToString(), "UTENTE", "userid_operatore"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["dta_azione"].ToString(), "DATA AZIONE", "dta_azione"));

                //campo esito:
                string esito = string.Empty;
                if (row["cha_esito"].ToString() == "1")
                    esito = "OK";
                else if (row["cha_esito"].ToString() == "0")
                    esito = "KO";
                reportRow.Columns.Add(this.GenerateHeaderColumn(esito, "ESITO", "cha_esito"));
                    
                //campo azione:
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["var_desc_oggetto"].ToString(), "AZIONE", "var_desc_oggetto"));

                rows.Add(reportRow);

            }

            return rows;

        }

        private ReportMapColumnProperty GenerateHeaderColumn(string value, string columnName, String originalName)
        {
            return new ReportMapColumnProperty()
            {
                OriginalName = originalName,
                ColumnName = columnName,
                Value = value
            };
        }

        /// <summary>
        /// Restituisce il summary del report in base ai filtri di ricerca utilizzati
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        private string ReportSummary(List<FiltroRicerca> filters)
        {
            
            string summary = string.Empty;
            bool bDataFrom = false;
            bool bDataTo = false;
            string dataFrom = string.Empty;
            string dataTo = string.Empty;
            

            DocsPaDB.Query_DocsPAWS.Reporting.EsportaLogConservazioneReport manager = new DocsPaDB.Query_DocsPAWS.Reporting.EsportaLogConservazioneReport();

            foreach (FiltroRicerca f in filters)
            {
                switch (f.argomento)
                {
                    case "idIstanza":
                        if (!string.IsNullOrEmpty(f.valore))
                            summary += string.Format("Numero istanza: {0}\n", f.valore);
                        break;

                    case "dataFrom":
                        if (!string.IsNullOrEmpty(f.valore))
                        {
                            bDataFrom = true;
                            dataFrom = f.valore;
                        }
                        break;

                    case "dataTo":
                        if (!string.IsNullOrEmpty(f.valore))
                        {
                            bDataTo = true;
                            dataTo = f.valore;
                        }
                        break;

                    case "utente":
                        if (!string.IsNullOrEmpty(f.valore))
                            summary += string.Format("Utente: {0}\n", f.valore.ToUpper());
                        break;

                    case "azione":
                        if (!string.IsNullOrEmpty(f.valore))
                            summary += string.Format("Azione: {0}\n", manager.GetAzione(f.valore));
                        break;

                    case "esito":
                        if (f.valore == "0")
                        {
                            summary += "Esito: negativo\n";
                        }
                        else if (f.valore == "1")
                        {
                            summary += "Esito: positivo\n";
                        }
                        break;

                }

            }

            if (bDataFrom && bDataTo)
            {
                summary += string.Format("Data: tra {0} e {1}", dataFrom, dataTo);
            }
            else if (bDataFrom && (!bDataTo))
            {
                summary += string.Format("Data: successiva a {0}", dataFrom);
            }
            else if (bDataTo && (!bDataFrom))
            {
                summary += string.Format("Data: precedente a {0}", dataTo);
            }


            return summary;
        }
    }
}
