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
    [ReportGenerator(Name = "Verifica formati conservazione", ContextName = "VerificaFormatiConservazione", Key = "VerificaFormatiConservazione")]
    public class VerificaFormatiConservazioneReportGeneratorCommand : ReportGeneratorCommand
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
                
               // request.Title = string.Format("{0} - Richiesta di Rigenerazione Istanza", Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(request.UserInfo.idAmministrazione).Descrizione);
                //request.SubTitle = string.Format("Stampa del {0}", DateTime.Now.ToShortDateString());

                //request.AdditionalInformation = "Numero istanza: " + idIstanza + "\n";
               // request.SubTitle = string.Empty;//string.Format("Si richiede la rigenerazione dell'istanza numero {0} perché risulta danneggiato il supporto numero {1}.", idIstanza, idSupporto);
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

            DataSet dataSet = dataExtractor.ExtractData(request);
            List<DocsPaVO.Report.Report> report = this.CreateReport(dataSet, request);

            return report;
        }

        /// <summary>
        /// Metodo da richiamare per la generazione di un report. Il comportamento strandard prevede
        /// l'utilizzo di un behavior per l'estrazione dati basato su query list. Sovrascrivere questo
        /// metodo per ottenere un comportamento differente.
        /// </summary>
        /// <param name="request">Informazioni utili alla generazione del report</param>
        /// <returns>Outup del processo di generazione</returns>
        protected override PrintReportResponse GenerateReport(PrintReportRequest request)
        {
            IReportDataExtractionBehavior reportDataExtractor = new DatasetReportDataExtractionBehavior();
            IReportGeneratorBehavior formatGenerator = this.CreateReportGenerator(request);

            // Generazione del report
            return this.GenerateReport(request, reportDataExtractor, formatGenerator);
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
            report.Summary = string.Empty;  //String.Format("Numero documenti: {0}", report.ReportMapRow.Rows.Count);

            // Restituzione del report generato
            return new List<DocsPaVO.Report.Report>() { report };
        }

        protected HeaderColumnCollection GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport, string contextName)
        {
            HeaderColumnCollection header = new HeaderColumnCollection();

            //genero le colonne di interesse
            header.Add(new HeaderProperty()
            {
                ColumnName = "Segnatura/ID Documento",
                ColumnSize = 90,
                OriginalName = "Segnatura",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "ID Doc./Allegato",
                ColumnSize = 80,
                OriginalName = "idDoc",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "Oggetto",
                ColumnSize = 200,
                OriginalName = "Oggetto",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Data Crezione/Proto",
                ColumnSize = 100,
                OriginalName = "DataCrezioneOProto",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Tipo Doc.",
                ColumnSize = 40,
                OriginalName = "TipoDoc",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Tipo File",
                ColumnSize = 40,
                OriginalName = "TipoFile",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Principale/Allegato",
                ColumnSize = 60,
                OriginalName = "PrincipaleAllegato",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Ammesso",
                ColumnSize = 40,
                OriginalName = "Ammesso",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Valido",
                ColumnSize = 40,
                OriginalName = "Valido",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Consolidato",
                ColumnSize = 40,
                OriginalName = "Consolidato",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Firmato",
                ColumnSize = 40,
                OriginalName = "Firmato",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Con Timestamp",
                ColumnSize = 40,
                OriginalName = "ConTimestamp",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Convertibile",
                ColumnSize = 50,
                OriginalName = "Convertibile",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Tipo Diritto",
                ColumnSize = 40,
                OriginalName = "TipoDiritto",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Utente Proprietario",
                ColumnSize = 80,
                OriginalName = "UtenteProprietario",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            header.Add(new HeaderProperty()
            {
                ColumnName = "Ruolo Proprietario",
                ColumnSize = 200,
                OriginalName = "RuoloProprietario",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
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

                // Creazione della riga
                foreach (DataColumn dataColumn in row.Table.Columns)
                {

                    // Se bisogna esportare il campo, viene aggiunta una colonna
                    if (reportHeader[dataColumn.ColumnName].Export)
                    {
                        reportRow.Columns.Add(new ReportMapColumnProperty()
                        {
                            DataType = reportHeader[dataColumn.ColumnName].DataType,
                            Value = row[dataColumn].ToString(),
                            OriginalName = dataColumn.ColumnName
                        });
                    }
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
