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
    [ReportGenerator(Name = "Registro degli accessi per export", ContextName = "RegistroAccessiExport", Key = "RegistroAccessiExport")]
    public class RegistroAccessiExportReportGeneratorCommand : ReportGeneratorCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(RegistroAccessiExportReportGeneratorCommand));

        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }

        /// <summary>
        /// Metodo per la generazione del report
        /// </summary>
        /// <param name="request"></param>
        /// <param name="dataExtractor"></param>
        /// <param name="reportGeneration"></param>
        /// <returns></returns>
        protected override PrintReportResponse GenerateReport(PrintReportRequest request, IReportDataExtractionBehavior dataExtractor, IReportGeneratorBehavior reportGeneration)
        {
            PrintReportResponse response = null;

            try
            {
                // Lista dei report da esportare
                List<DocsPaVO.Report.Report> reports = new List<DocsPaVO.Report.Report>();

                // Response da restituire
                response = new PrintReportResponse();

                // Dataset per l'estrazione
                DataSet dataSet = new DataSet();

                // Foglio per Accesso documentale
                request.SearchFilters.Where(f => f.argomento == "tipologia").FirstOrDefault().valore = "Accesso Documentale";
                dataSet = dataExtractor.ExtractData(request);
                reports.AddRange(this.CreateReport(dataSet, request));

                // Foglio per Accesso generalizzato e civico
                request.SearchFilters.Where(f => f.argomento == "tipologia").FirstOrDefault().valore = "Accesso Generalizzato e Civico";
                dataSet = dataExtractor.ExtractData(request);
                reports.AddRange(this.CreateReport(dataSet, request));

                // Foglio per Accesso dei consiglieri provinciali
                request.SearchFilters.Where(f => f.argomento == "tipologia").FirstOrDefault().valore = "Accesso dei Consiglieri provinciali";
                dataSet = dataExtractor.ExtractData(request);
                reports.AddRange(this.CreateReport(dataSet, request));

                // Generazione file report
                response.Document = reportGeneration.GenerateReport(request, reports);

                // Formato file
                string ext = string.Empty;
                if(request.ReportType == ReportTypeEnum.ODS)
                {
                    ext = "ods";
                }
                else
                {
                    ext = "xls";
                }

                if(response.Document != null)
                {
                    response.Document.name = String.Format("Estrazione_Registro_Accessi_{0}.{1}", DateTime.Now.ToString("dd-MM-yyyy"), ext);
                    response.Document.fullName = response.Document.name;
                }

            }
            catch(Exception ex)
            {
                throw new ReportGenerationFailedException(ex.Message);
            }

            return response;
        }

        protected override List<DocsPaVO.Report.Report> CreateReport(DataSet dataSet, PrintReportRequest request)
        {
            DocsPaVO.Report.Report report = new DocsPaVO.Report.Report();

            report.CreationDate = DateTime.Now;         
            report.AdditionalInformation = request.AdditionalInformation;
            report.ReportMapRow = new ReportMapRow();
            report.ReportHeader = new HeaderColumnCollection();

            String tipologia = request.SearchFilters.Where(f => f.argomento == "tipologia").FirstOrDefault().valore;
            report.Title = request.Title;
            report.Subtitle = "Tipologia : " + tipologia;
            report.AdditionalInformation = RegistroAccessi.RegistroAccessiManager.GetAdditionalInformation(request.SearchFilters);
            report.SectionName = tipologia;           
            
            // Generazione delle righe del report
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null)
            {
                // Generazione dell'header personalizzato del report
                report.ReportHeader = this.GenerateReportHeader(dataSet, request.ColumnsToExport);

                report.ReportMapRow.Rows.AddRange(this.GenerateReportRows(dataSet, report.ReportHeader));
            }

            // Costruzione del summary
            report.Summary = String.Format("Righe estratte: {0}", report.ReportMapRow.Rows.Count);

            // Restituzione del report generato
            return new List<DocsPaVO.Report.Report>() { report };

        }

        /// <summary>
        /// Metodo per la generazione dell'header e delle colonne del report
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="fieldsToExport"></param>
        /// <returns></returns>
        protected override HeaderColumnCollection GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport)
        {
            logger.Debug("BEGIN");
            HeaderColumnCollection header = new HeaderColumnCollection();

            // Inizializzo l'header a partire dal dataset
            header = base.GenerateReportHeaderFromDataSet(dataSet);

            // Modifico gli header delle colonne
            header["CODICE"].ColumnName = "CODICE FASCICOLO";
            header["DATA_CREAZIONE"].ColumnName = "DATA CREAZIONE";

            // Rimuovo le colonne di servizio e quelle non necessarie per l'export
            header.Remove(header["ID_PROJECT"]);
            header.Remove(header["POSIZIONE"]);
            header.Remove(header["ANNO"]);
            header.Remove(header["STATO_FASC"]);
            header.Remove(header["NOME_CAMPO"]);
            header.Remove(header["VALORE_CAMPO"]);

            // Aggiungo le colonne relative alla tipologia in esame
            this.GenerateAdditionalColumns(dataSet, header);

            // Setto la larghezza delle colonne
            header["CODICE"].ColumnSize = 100;
            header["DESCRIZIONE"].ColumnSize = 120;
            header["UFFICIO"].ColumnSize = 140;
            header["STRUTTURA"].ColumnSize = 140;
            header["DATA_CREAZIONE"].ColumnSize = 70;
            header["TIPOLOGIA"].ColumnSize = 125;

            logger.Debug("END");
            return header;
        }

        /// <summary>
        /// Metodo per la generazione delle righe del report
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="reportHeader"></param>
        /// <returns></returns>
        protected override IEnumerable<ReportMapRowProperty> GenerateReportRows(DataSet dataSet, HeaderColumnCollection reportHeader)
        {
            logger.Debug("BEGIN");

            // Lista delle righe del report
            List<ReportMapRowProperty> rows = new List<ReportMapRowProperty>();

            // Riga in corso di generazione
            ReportMapRowProperty row = null;
            // Id del fascicolo
            String folderId = String.Empty;

            foreach(DataRow dataRow in dataSet.Tables[0].Rows)
            {
                if (!String.IsNullOrEmpty(dataRow["ID_PROJECT"].ToString()))
                {
                    if (String.IsNullOrEmpty(folderId) || !dataRow["ID_PROJECT"].ToString().Equals(folderId))
                    {
                        // Se folderId non è valorizzato sto analizzando la prima riga del dataset
                        // Se il valore del campo ID_PROJECT del dataset non coincide con folderId sto analizzando una nuova riga
                        folderId = dataRow["ID_PROJECT"].ToString();
                        row = this.GenerateNewRow(dataRow, reportHeader);
                        rows.Add(row);
                    }
                    else
                    {
                        // Se il valore del campo ID_PROJECT coincide con folderId devo aggiungere il valore dell'i-esimo campo profilato
                        // alla riga del report
                        this.UpdateRow(row, dataRow, reportHeader);
                    }
                }
            }

            logger.Debug("END");

            return rows;
        }

        /// <summary>
        /// Metodo per l'aggiunta dei nomi dei campi profilati alle colonne del report
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="header"></param>
        private void GenerateAdditionalColumns(DataSet dataSet, HeaderColumnCollection header)
        {
            logger.Debug("BEGIN");

            String firstFolderId = string.Empty;

            foreach(DataRow dataRow in dataSet.Tables[0].Rows)
            {              
                if (string.IsNullOrEmpty(dataRow["ID_PROJECT"].ToString()))
                {
                    // Le prime righe con id_project = null contengono solo i campi profilati della tipologia
                    if(dataRow["NOME_CAMPO"] != null)
                    {
                        header.Add(new HeaderProperty() { ColumnName = dataRow["NOME_CAMPO"].ToString(), OriginalName = dataRow["NOME_CAMPO"].ToString(), ColumnSize = 80 });
                        logger.Debug("Aggiunta colonna " + dataRow["NOME_CAMPO"].ToString());
                    }
                }
                else
                {
                    // Se il valore della riga cambia sono passato all'elemento successivo e devo fermarmi
                    break;
                }
            }

            logger.Debug("END");
        }

        private ReportMapRowProperty GenerateNewRow(DataRow dataRow, HeaderColumnCollection reportHeader)
        {
            logger.Debug("BEGIN");

            ReportMapRowProperty row = new ReportMapRowProperty();

            row.Columns.Add(this.GenerateHeaderColumn(dataRow["CODICE"].ToString(), "CODICE FASCICOLO", "CODICE"));
            row.Columns.Add(this.GenerateHeaderColumn(dataRow["DESCRIZIONE"].ToString(), "DESCRIZIONE", "DESCRIZIONE"));
            row.Columns.Add(this.GenerateHeaderColumn(dataRow["UFFICIO"].ToString(), "UFFICIO", "UFFICIO"));
            row.Columns.Add(this.GenerateHeaderColumn(dataRow["STRUTTURA"].ToString(), "STRUTTURA", "STRUTTURA"));
            row.Columns.Add(this.GenerateHeaderColumn(dataRow["DATA_CREAZIONE"].ToString(), "DATA CREAZIONE", "DATA_CREAZIONE"));
            row.Columns.Add(this.GenerateHeaderColumn(dataRow["TIPOLOGIA"].ToString(), "TIPOLOGIA", "TIPOLOGIA"));

            // Primo valore campo profilato
            row.Columns.Add(this.GenerateHeaderColumn(dataRow["VALORE_CAMPO"].ToString(), dataRow["NOME_CAMPO"].ToString(), dataRow["NOME_CAMPO"].ToString()));

            logger.Debug("END");

            return row;
        }

        private void UpdateRow(ReportMapRowProperty row, DataRow dataRow, HeaderColumnCollection reportHeader)
        {
            logger.Debug("BEGIN");

            // Controllo esistenza nell'header
            if(reportHeader[dataRow["NOME_CAMPO"].ToString()] != null)
            {
                // Controllo esistenza valore nella riga
                if(row[dataRow["NOME_CAMPO"].ToString()] == null)
                {
                    row.Columns.Add(this.GenerateHeaderColumn(dataRow["VALORE_CAMPO"].ToString(), dataRow["NOME_CAMPO"].ToString(), dataRow["NOME_CAMPO"].ToString()));
                }
            }

            logger.Debug("END");
        }

        /// <summary>
        /// Funzione per la generazione di una riga del report
        /// </summary>
        /// <param name="value">Valore da assegnare al campo</param>
        /// <param name="columnName">Nome da assegnare alla colonna</param>
        /// <param name="originalName">Nome originale della colonna</param>
        /// <returns>Colonna</returns>
        private ReportMapColumnProperty GenerateHeaderColumn(string value, string columnName, String originalName)
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
