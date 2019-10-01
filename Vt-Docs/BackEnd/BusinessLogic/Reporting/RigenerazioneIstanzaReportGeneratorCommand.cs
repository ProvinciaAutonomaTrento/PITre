using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using BusinessLogic.Reporting.Exceptions;
using DocsPaDB.Query_DocsPAWS;
using DocsPaVO.Report;
using log4net;


namespace BusinessLogic.Reporting
{

    [ReportGenerator(Name="Rigenerazione Istanza", ContextName="RigenerazioneIstanza", Key="RigenerazioneIstanza")]
    public class RigenerazioneIstanzaReportGeneratorCommand : ReportGeneratorCommand
    {

        private ILog logger = LogManager.GetLogger(typeof(RigenerazioneIstanzaReportGeneratorCommand));

        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }

        protected override PrintReportResponse GenerateReport(PrintReportRequest request, IReportDataExtractionBehavior dataExtractor, IReportGeneratorBehavior reportGeneration)
        {

            PrintReportResponse response = null;
            try
            {
                //Personalizzazione report
                //le informazioni necessarie sono inserite nella request:
                string nomePolicy = request.SubTitle;
                //string idIstanza = request.SearchFilters.Where(f => f.argomento == "idIstanza").FirstOrDefault().valore;
                string idSupporto = request.SearchFilters.Where(f=> f.argomento=="idSupporto").FirstOrDefault().valore;

                request.Title = string.Format("{0} - Richiesta di Rigenerazione Istanza", Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(request.UserInfo.idAmministrazione).Descrizione);
                //request.SubTitle = string.Format("Stampa del {0}", DateTime.Now.ToShortDateString());

                //request.AdditionalInformation = "Numero istanza: " + idIstanza + "\n";
                //request.SubTitle = string.Format("Si richiede la rigenerazione dell'istanza numero {0} perché risulta danneggiato il supporto numero {1}.", idIstanza, idSupporto);
                request.AdditionalInformation = string.Empty;

                //istanzio la response
                response = new PrintReportResponse();

                //Recupero dataset
                //DataSet dataSet = dataExtractor.ExtractData(request);

                //Generazione dati per report
                List<DocsPaVO.Report.Report> report = new List<DocsPaVO.Report.Report>();

                report.AddRange(this.GetReport(dataExtractor, request));

                //2 - report aggiuntivo con dettaglio policy
                //report.AddRange(this.GetReport(dataExtractor, request, "NotificheRifiutoPolicy"));

                //Creazione file report
                response.Document = reportGeneration.GenerateReport(request, report);

            }
            catch (Exception e)
            {
                throw new ReportGenerationFailedException(e.Message);
            }

            //restituzione del report
            return response;
        }

        private List<DocsPaVO.Report.Report> GetReport(IReportDataExtractionBehavior dataExtractor, PrintReportRequest request)
        {
            //imposto il context name
            //request.ReportKey = contextName;
            DataSet dataSet = dataExtractor.ExtractData(request);

            List<DocsPaVO.Report.Report> report = this.CreateReport(dataSet, request);

            return report;
        }

        protected override List<DocsPaVO.Report.Report> CreateReport(DataSet dataSet, PrintReportRequest request)
        {
            // Report da restiutuire
            DocsPaVO.Report.Report report = new DocsPaVO.Report.Report();

            // Impostazione delle proprietà di base del report
            report.CreationDate = DateTime.Now;
            report.Subtitle = request.SubTitle;
            report.Title = request.Title;
            report.AdditionalInformation = request.AdditionalInformation;
            report.ReportMapRow = new ReportMapRow();
            report.ReportHeader = new HeaderColumnCollection();

            // Generazione delle righe del report
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null)
            {
                // Generazione dell'header del report
                report.ReportHeader = this.GenerateReportHeader(dataSet, request.ColumnsToExport, request.ReportKey);

                report.ReportMapRow.Rows.AddRange(this.GenerateReportRows(dataSet, report.ReportHeader, request.ReportKey));
            }


            // Costruzione del summary
            report.Summary = String.Format("Numero documenti: {0}", report.ReportMapRow.Rows.Count);

            // Restituzione del report generato
            return new List<DocsPaVO.Report.Report>() { report };
        }

        protected HeaderColumnCollection GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport, string contextName)
        {
            HeaderColumnCollection header = new HeaderColumnCollection();

            //genero le colonne di interesse
            header.Add(new HeaderProperty()
            {
                ColumnName = "Tipo Doc.",
                ColumnSize = 40,
                OriginalName = "TIPO_DOC",
                Export = true,
                DataType = HeaderProperty.ContentDataType.Integer
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Oggetto",
                ColumnSize = 250,
                OriginalName = "OGGETTO",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Fasc.",
                ColumnSize = 40,
                OriginalName = "COD_FASC",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Data Ins.",
                ColumnSize = 100,
                OriginalName = "DATA_INS",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Id/Segn. Data",
                ColumnSize = 160,
                OriginalName = "ID_SEGN_DATA",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Size Byte",
                ColumnSize = 70,
                OriginalName = "SIZE_ITEM",
                Export = true,
                DataType = HeaderProperty.ContentDataType.Integer
            });

            return header;
        }

        protected IEnumerable<ReportMapRowProperty> GenerateReportRows(DataSet dataSet, HeaderColumnCollection reportHeader, string contextName)
        {
            List<ReportMapRowProperty> rows = new List<ReportMapRowProperty>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                //creo una nuova riga
                ReportMapRowProperty reportRow = new ReportMapRowProperty();

                //TIPO DOC
                //nel report va sostituito "G" con "NP"
                //vedi richiesta nel metodo exportConservazioneToXML
                string tipoDoc = row["TIPO_DOC"].ToString();
                if (tipoDoc.Equals("G"))
                    tipoDoc = "NP";
                reportRow.Columns.Add(this.GenerateHeaderColumn(tipoDoc, "Tipo Doc.", "TIPO_DOC"));

                //OGGETTO, FASCICOLO, DATA
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["OGGETTO"].ToString(), "Oggetto", "OGGETTO"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["COD_FASC"].ToString(), "Fasc.", "COD_FASC"));
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["DATA_INS"].ToString(), "Data Ins.", "DATA_INS"));

                //ID/SEGN. DATA
                string data_doc = string.Format("{0} \n {1}", row["SEGNATURA"].ToString(), row["DATA_PROT_OR_CREA"].ToString());
                reportRow.Columns.Add(this.GenerateHeaderColumn(data_doc, "Id/Segn. Data", "ID_SEGN_DATA"));

                //SIZE
                reportRow.Columns.Add(this.GenerateHeaderColumn(row["SIZE_ITEM"].ToString(), "Size Byte", "SIZE_ITEM"));

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
