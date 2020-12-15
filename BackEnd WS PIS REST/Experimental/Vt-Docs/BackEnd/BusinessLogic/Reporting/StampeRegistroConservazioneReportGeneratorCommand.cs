using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using BusinessLogic.Reporting.Exceptions;
using DocsPaDB.Query_DocsPAWS;
using DocsPaVO.utente.RegistroConservazione;
using DocsPaVO.filtri;
using DocsPaVO.Report;
using log4net;

namespace BusinessLogic.Reporting
{
    [ReportGenerator(Name="Stampa registro conservazione", ContextName="StampaRegistroConservazione", Key="StampaRegistroConservazione")]
    public class StampeRegistroConservazioneReportGeneratorCommand : ReportGeneratorCommand
    {

        private static ILog logger = LogManager.GetLogger(typeof(RegistroConservazionePrintManager));

        protected override DocsPaVO.Report.HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }

        protected override PrintReportResponse GenerateReport(PrintReportRequest request, Behaviors.DataExtraction.IReportDataExtractionBehavior dataExtractor, Behaviors.ReportGenerator.IReportGeneratorBehavior reportGeneration)
        {

            PrintReportResponse response = null;
            
            try
            {
                logger.Debug("Inizio GenerateReport.");
                //istanzio la response da restituire
                response = new PrintReportResponse();

                //creo la lista di report da esportare
                List<DocsPaVO.Report.Report> reports = new List<DocsPaVO.Report.Report>();

                //creo il titolo
                this.SetTitle(request);

                //ricavo la lista delle istanze

                RegistroConservazionePrintManager manager = new RegistroConservazionePrintManager();
                List<string> listIdIstanze = manager.GetListaIstanze(request);
                logger.Debug(string.Format("{0} istanze trovate.", listIdIstanze.Count));

                //creo due nuovi filtri su istanze e documenti
                request.SearchFilters.Add(new FiltroRicerca() { argomento = "id_istanza", valore = string.Empty });
                request.SearchFilters.Add(new FiltroRicerca() { argomento = "id_oggetto", valore = string.Empty });

                foreach (var ist in listIdIstanze)
                {
                    logger.Debug(string.Format("Istanza {0}: ", ist));
                    //creo il summary per l'istanza
                    string summary = GetSummary(request, "istanza", ist, String.Empty);

                    //inserisco il summary nel campo additional information della request
                    request.AdditionalInformation = summary;

                    //imposto il filtro sull'istanza al valore attuale e inizializzo il filtro sul documento
                    foreach (FiltroRicerca f in request.SearchFilters)
                    {
                        if (f.argomento == "id_istanza")
                            f.valore = ist;
                        if (f.argomento == "id_oggetto")
                            f.valore = string.Empty;
                    }

                    //genero il report per l'istanza
                    reports.AddRange(this.GetReport(dataExtractor, request, "rcIstanze"));

                    //dopo la prima stampa devo cancellare titolo e sottotitolo!
                    request.Title = string.Empty;
                    request.SubTitle = string.Empty;

                    //ricavo la lista dei documenti che appartengono all'istanza
                    List<string> listIdDoc = manager.GetListaDocumentiIstanze(request);
                    logger.Debug(string.Format("{0} documenti trovati nell'istanza.", listIdDoc.Count));

                    //loop sui documenti, eventualmente realizzare un metodo ad hoc
                    
                    foreach (var doc in listIdDoc)
                    {
                        //creo il summary per il documento
                        summary = GetSummary(request, "documento", ist, doc);

                        //inserisco il summary nel campo additional information della request
                        request.AdditionalInformation = summary;

                        //imposto il filtro sul documento al valore attuale
                        foreach (FiltroRicerca f in request.SearchFilters)
                        {
                            if (f.argomento == "id_oggetto")
                                f.valore = doc;
                        }

                        //genero il report per il documento
                        reports.AddRange(this.GetReport(dataExtractor, request, "rcDocumenti"));
                    }

                }

                request.ReportKey = "StampaConservazione";
                logger.Debug("Chiamata servizio di stampa.");
                response.Document = reportGeneration.GenerateReport(request, reports);

            }
            catch (Exception ex)
            {
                logger.Debug("Errore in StampeRegistroConservazioneReportGeneratorCommand: ", ex);
                throw new ReportGenerationFailedException(ex.Message);
                
            }

            return response;


        }

        /// <summary>
        /// Discrimina il comportamento del generatore di report in base alla tipologia
        /// in ingresso (istanze/documenti).
        /// </summary>
        /// <param name="dataExtractor"></param>
        /// <param name="request"></param>
        /// <param name="contextName">Identifica istanze/documenti</param>
        /// <returns>Report relativo alla singola istanza/documento</returns>
        private List<DocsPaVO.Report.Report> GetReport
            (IReportDataExtractionBehavior dataExtractor, PrintReportRequest request, string contextName)
        {


            //imposto il context al valore attuale ed estraggo i dati
            request.ReportKey = contextName;
            DataSet dataset = dataExtractor.ExtractData(request);
            
            //genero il report
            List<DocsPaVO.Report.Report> report = this.CreateReport(dataset, request);

            return report;


        }

        /// <summary>
        /// Metodo per la generazione di titolo e sottotitolo del report
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private void SetTitle(PrintReportRequest request)
        {
            string title = string.Empty;
            string subTitle = string.Empty;
           
            string ammDescr = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(request.UserInfo.idAmministrazione).Descrizione;
            title = string.Format("{0} - Registro della Conservazione", ammDescr);
            subTitle = request.AdditionalInformation;

            //imposto titolo e sottotitolo nella request e svuoto il campo additional information
            request.Title = title;
            request.SubTitle = subTitle;
            request.AdditionalInformation = string.Empty;

        }

        /// <summary>
        /// Metodo per la generazione del report da esportare
        /// </summary>
        /// <param name="dataSet">Dataset da cui estrarre i dati</param>
        /// <param name="request">Informazioni sulla richiesta</param>
        /// <returns>Report con i dati estratti</returns>
        protected override List<DocsPaVO.Report.Report> CreateReport(DataSet dataSet, PrintReportRequest request)
        {

            logger.Debug("Metodo CreateReport.");

            // Report da restiutuire
            DocsPaVO.Report.Report report = new DocsPaVO.Report.Report();

            //definisco il summary che ho inserito nel campo additional information
            report.Summary = request.AdditionalInformation;

            // Impostazione delle proprietà di base del report
            report.CreationDate = DateTime.Now;
            report.Subtitle = request.SubTitle;
            report.Title = request.Title;
            report.AdditionalInformation = string.Empty;
            report.ReportMapRow = new ReportMapRow();
            report.ReportHeader = new HeaderColumnCollection();

            // Generazione delle righe del report
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null)
            {
                // Generazione dell'header del report
                report.ReportHeader = this.GenerateReportHeader(dataSet, request.ColumnsToExport);

                report.ReportMapRow.Rows.AddRange(this.GenerateReportRows(dataSet, report.ReportHeader));
            }

            return new List<DocsPaVO.Report.Report>() { report };
        }

        /// <summary>
        /// Metodo per la generazione dell'header del report
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="fieldsToExport"></param>
        /// <returns></returns>
        protected override HeaderColumnCollection GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport)
        {

            HeaderColumnCollection header = new HeaderColumnCollection();

            // Inizializzazione dell'header a partire dal dataset
            header = base.GenerateReportHeaderFromDataSet(dataSet);

            // Modifica delle descrizioni di interesse
            //header["var_descrizione"].ColumnName = "EVENTO";
            header["var_desc_azione"].ColumnName = "DETTAGLI";
            header["userid_operatore"].ColumnName = "OPERATORE";
            header["dta_operazione"].ColumnName = "DATA E ORA";
            header["cha_esito"].ColumnName = "ESITO";

            // Rimozione delle altre colonne
            header.Remove(header["system_id"]);
            header.Remove(header["cha_tipo_oggetto"]);
            header.Remove(header["var_descrizione"]);

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

            // Lista delle righe
            List<ReportMapRowProperty> rows = new List<ReportMapRowProperty>();

            // Riga in generazione
            ReportMapRowProperty row = null;
            // Id dell'istanza/documento in esame
            String currentId = String.Empty;

            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                // Se non è stato ancora recuperato un id di documento o
                // se l'id della riga corrente è diverso da quello memorizzato,
                // significa che è cambiato il documento, quindi viene generata una nuova
                // riga
                if (String.IsNullOrEmpty(currentId) || !dataRow["system_id"].ToString().Equals(currentId))
                {

                    // Se la riga attuale è diversa da null, viene aggiunta all'elenco delle righe
                    currentId = dataRow["system_id"].ToString();
                    row = this.GenerateNewRow(dataRow, reportHeader);
                    rows.Add(row);
                }
            }

            return rows;
        }

        /// <summary>
        /// Funzione per la generazione di una nuova riga del report
        /// </summary>
        /// <param name="dataRow">DataRow da cui estrarre i dati per inizializzare la riga</param>
        /// <param name="reportHeader">Header del report</param>
        /// <returns>Riga inizializzata</returns>
        private ReportMapRowProperty GenerateNewRow(DataRow dataRow, HeaderColumnCollection reportHeader)
        {
            ReportMapRowProperty row = new ReportMapRowProperty();

            // Aggiunta dei campi
            //row.Columns.Add(this.GenerateHeaderColumn(dataRow["var_descrizione"].ToString(), "EVENTO", "var_descrizione"));
            row.Columns.Add(this.GenerateHeaderColumn(dataRow["var_desc_azione"].ToString(), "DETTAGLI", "var_desc_azione"));
            row.Columns.Add(this.GenerateHeaderColumn(dataRow["userid_operatore"].ToString(), "OPERATORE", "userid_operatore"));
            row.Columns.Add(this.GenerateHeaderColumn(dataRow["dta_operazione"].ToString(), "DATA E ORA", "dta_operazione"));

            //campo esito
            string esito = string.Empty;
            if (dataRow["cha_esito"].ToString() == "1")
                esito = "positivo";
            else if (dataRow["cha_esito"].ToString() == "0")
                esito = "negativo";
            row.Columns.Add(this.GenerateHeaderColumn(esito, "ESITO", "cha_esito"));

            return row;

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

        /// <summary>
        /// Costruisce il summary relativo all'istanza o documento in oggetto
        /// </summary>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context">distingue tra summary istanza/documento</param>
        /// <param name="idIstanza">id istanza attuale</param>
        /// <param name="idDoc">id documento (se richiesto)</param>
        /// <returns></returns>
        private string GetSummary(PrintReportRequest request, string context, string idIstanza, string idDoc)
        {

            string summary = string.Empty;
            RegistroConservazionePrintManager manager = new RegistroConservazionePrintManager();


            //costruzione del summary nei due casi
            if (context == "istanza")
            {

                RegistroConservazioneSummary summaryData = manager.GetSummaryDataIst(idIstanza);

                string creationDate = summaryData.creationDate.Equals(DateTime.MinValue) ? string.Empty : summaryData.creationDate.ToString("dd/MM/yyyy");
                string invioDate = summaryData.invioDate.Equals(DateTime.MinValue) ? string.Empty : summaryData.invioDate.ToString("dd/MM/yyyy");

                summary = "ISTANZA - ID istanza: " + idIstanza + "\n\n";

                summary += "Descrizione istanza: " + summaryData.descrizione + "\n";
                summary += "Data di apertura: " + creationDate + "\n";
                summary += "Data di invio al CS: " + invioDate + "\n";
                summary += "Numero documenti: " + summaryData.numDoc + "\n";
                summary += "Dimensioni complessive: " + manager.ConvertDocSize(summaryData.fileDim) + "\n";

                logger.Debug(summary);

            }
            else if (context == "documento")
            {
                RegistroConservazioneSummary summaryData = manager.GetSummaryDataDoc(idDoc);

                string segnatura = new DocsPaDB.Query_DocsPAWS.Conservazione().getSegnatura_Id(idDoc);
                string creationDate = summaryData.creationDate.Equals(DateTime.MinValue) ? string.Empty : summaryData.creationDate.ToString("dd/MM/yyyy");

                summary = "DOCUMENTO - ID/Segnatura documento: " + segnatura + "\n\n";

                summary += "ID istanza: " + idIstanza + "\n";
                summary += "Data creazione/protocollazione: " + creationDate + "\n";
                summary += "Oggetto: " + summaryData.descrizione + "\n";
                summary += "Codice fascicolo: " + summaryData.codiceFascicolo + "\n";
                summary += "N. allegati: " + summaryData.numDoc + "\n";
                summary += "Tipo file: " + summaryData.tipoFile + "\n";
                summary += "Dimensioni file: " + manager.ConvertDocSize(summaryData.fileDim) + "\n";

                logger.Debug(summary);

            }


            return summary;
        }

    }
}
