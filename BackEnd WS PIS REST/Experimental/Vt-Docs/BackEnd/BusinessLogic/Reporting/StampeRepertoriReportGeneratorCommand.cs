using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Report;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using System.Data;
using BusinessLogic.Reporting.Exceptions;
using BusinessLogic.Documenti;
using DocsPaVO.utente;

namespace BusinessLogic.Reporting
{
    [ReportGenerator(Name = "Stampa repertori", ContextName = "StampaRegistriRepertori", Key = "StampaRegistriRepertori")]
    public class StampeRepertoriReportGeneratorCommand : ReportGeneratorCommand
    {
        /// <summary>
        /// Per questo report non è possibile scegliere i campi da esportare
        /// </summary>
        /// <returns>Null</returns>
        protected override DocsPaVO.Report.HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }

        protected override PrintReportResponse GenerateReport(PrintReportRequest request, IReportDataExtractionBehavior dataExtractor, IReportGeneratorBehavior reportGeneration)
        {
            PrintReportResponse response = null;

            try
            {
                // Lista dei report da esportare
                List<DocsPaVO.Report.Report> reports = new List<DocsPaVO.Report.Report>();

                // Response da restituire
                response = new PrintReportResponse();

                // Costruzione della prima parte del titolo
                String title = GetTitle(request);
                request.Title = title;

                // Repertori creati dopo l'ultima stampa
                reports.AddRange(this.GetReport(dataExtractor, request, "StampaRepertoriNuovi", "Nuovi documenti"));

                request.Title = title;

                // Repertori creati dopo l'ultima stampa
                reports.AddRange(this.GetReport(dataExtractor, request, "StampaRepertoriAggiornati", "Documenti modificati"));
                //String.Format("{0}\n{1}", "Documenti repertoriati nuovi", "Ruoli disabilitati"), "Foglio 1 di 7"));


                // Generazione file report
                response.Document = reportGeneration.GenerateReport(request, reports);

            }
            catch (Exception ex)
            {
                throw new ReportGenerationFailedException(ex.Message);
            }

            // Restituzione del report
            return response;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private String GetTitle(PrintReportRequest request)
        {
            String title = String.Empty;
            // Creazione del titolo del report
            String ammDescr = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(request.UserInfo.idAmministrazione).Descrizione;
            String rfDesc = String.Empty;

            // Recupero dell'id dell'RF
            String idRf = request.SearchFilters.Where(f => f.argomento == "idRf").FirstOrDefault().valore;

            if (!String.IsNullOrEmpty(idRf))
            {
                Registro rf = Utenti.RegistriManager.getRegistro(idRf);
                rfDesc = String.Format("[{0}] {1}", rf.codRegistro, rf.descrizione);
            }

            title = String.Format("{0}\n{1}\n", ammDescr, String.IsNullOrEmpty(rfDesc) ? request.AdditionalInformation : rfDesc);

            if (!String.IsNullOrEmpty(rfDesc))
                title += request.AdditionalInformation + "\n";
            request.AdditionalInformation = String.Empty;
            return title;
        }

        /// <summary>
        /// Funzione per la generazione di un report
        /// </summary>
        /// <param name="dataExtractor">Estrattore dei dati</param>
        /// <param name="request">Informazioni sul report da generare</param>
        /// <param name="contextName">Nome del contesto di estrazione</param>
        /// <param name="reportSubtitle">Sotto titolo da assegnare al report</param>
        /// <returns>Report da esportare</returns>
        private List<DocsPaVO.Report.Report> GetReport(
            IReportDataExtractionBehavior dataExtractor,
            PrintReportRequest request,
            String contextName,
            String reportSubtitle)
        {
            // Impostazione di titolo e sottotitolo del report
            request.SubTitle = reportSubtitle;

            // Estrazione dati
            DataSet dataSet = this.GetReportData(dataExtractor, request, contextName);

            String title = request.Title;
            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                title += BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.GetTipologyDescriptionByDocNumber(dataSet.Tables[0].Rows[0]["DocNumber"].ToString());

                title += String.Format(" - Stampa generata {0}", DateTime.Now.ToString("dddd dd MMMM yyyy"));
            }

            request.Title = title;

            // Generazione del report
            List<DocsPaVO.Report.Report> report = base.CreateReport(dataSet, request);

            // Restituzione del report creato
            return report;

        }

        /// <summary>
        /// Funzione per l'estrazione dei dati relativi ad un determinato contesto
        /// </summary>
        /// <param name="dataExtractor">Estrattore dei dati</param>
        /// <param name="request">Informazioni sul report da generare</param>
        /// <param name="contextName">Nome del contesto</param>
        /// <returns>DataSet con i dati da esportare</returns>
        private DataSet GetReportData(IReportDataExtractionBehavior dataExtractor, PrintReportRequest request, String contextName)
        {
            // Impostazione del contesto
            request.ReportKey = contextName;

            // Generazione del report
            return dataExtractor.ExtractData(request);
        }

        /// <summary>
        /// Metodo per la generazione dell'header del report
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="fieldsToExport"></param>
        /// <returns></returns>
        protected override HeaderColumnCollection  GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport)
        {
            HeaderColumnCollection header = new HeaderColumnCollection();

            // Inizializzazione dell'header a partire dal dataset
            header = base.GenerateReportHeaderFromDataSet(dataSet);
            
            // Modifica delle descrizioni di interesse
            header["IdDoc"].ColumnName = "Segn. proto. / Id doc.";
            header["Oggetto_Documento"].ColumnName = "Oggetto";
            header.Add(new HeaderProperty() { ColumnName = "Segn. repertorio", OriginalName = "Segn. repertorio" });
            header["Data_Di_Repertorio"].ColumnName = "Data repertorio";
            header["Data_Annullamento"].ColumnName = "Data annullamento";

            // Rimozione delle colonne con la descrizione del nome del campo e con l'id dell'amministrazione
            header.Remove(header["Descrizione_Campo_Profilato"]); // Ogni colonna un campo
            header.Remove(header["Valore"]);
            header.Remove(header["IdAmm"]);
            header.Remove(header["DocNumber"]);
            header.Remove(header["ObjType"]);
            header.Remove(header["EnabledHistory"]);
            header.Remove(header["ObjectId"]);

            return header;
        }

        /// <summary>
        /// Metodo per la generazione delle righe del report
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="reportHeader"></param>
        /// <returns></returns>
        protected override IEnumerable<ReportMapRowProperty>  GenerateReportRows(DataSet dataSet, HeaderColumnCollection reportHeader)
        {
            // Lista delle righe
            List<ReportMapRowProperty> rows = new List<ReportMapRowProperty>();

            // Riga in generazione
            ReportMapRowProperty row = null;
            // Id del documento in esame
            String documentId = String.Empty;

            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                // Se non è stato ancora recuperato un id di documento o
                // se l'id della riga corrente è diverso da quello memorizzato,
                // significa che è cambiato il documento, quindi viene generata una nuova
                // riga
                if (String.IsNullOrEmpty(documentId) || !dataRow["IdDoc"].ToString().Equals(documentId))
                {
                    // Se la riga attuale è diversa da null, viene aggiunta all'elenco delle righe
                    documentId = dataRow["IdDoc"].ToString();
                    row = this.GenerateNewRow(dataRow, reportHeader);
                    rows.Add(row);

                }
                else
                {
                    // Aggiornamento della riga solo se il campo profilato da esportare ha attivo lo storico)
                    if (dataRow["EnabledHistory"].ToString() == "1")
                        this.UpdateRow(row, dataRow, reportHeader);

                    // Se il campo è un contatore di repertorio, viene impostata la data (tutti gli altri campi
                    // hanno data di repertorio nulla
                    if (!String.IsNullOrEmpty(dataRow["data_di_repertorio"].ToString()))
                        row["Data_Di_Repertorio"].Value = dataRow["data_di_repertorio"].ToString();

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

            // Aggiunta dell'id del documento
            row.Columns.Add(this.GenerateHeaderColumn(dataRow["IdDoc"].ToString(), "Segn. proto. / Id doc.", "IDDOC"));

            // Aggiunta dell'oggetto
            row.Columns.Add(this.GenerateHeaderColumn(dataRow["Oggetto_Documento"].ToString(), "Oggetto", "oggetto_documento"));

            String dataAnnullamento = String.Empty;
            // Aggiunta della segnatura di repertorio
            row.Columns.Add(this.GenerateHeaderColumn(
                DocManager.GetSegnaturaRepertorio(dataRow["DocNumber"].ToString(), dataRow["IdAmm"].ToString(), false, out dataAnnullamento),
                "Segn. repertorio",
                "Segn. repertorio"));

            // Aggiunta della data di repertorio
            row.Columns.Add(this.GenerateHeaderColumn(dataRow["Data_Di_Repertorio"].ToString(), "Data repertorio", "data_di_repertorio"));

            // Aggiunta della data di annullamento
            row.Columns.Add(this.GenerateHeaderColumn(dataAnnullamento, "Data annullamento", "data_annullamento"));

            // Aggiunta dell'impronta
            row.Columns.Add(this.GenerateHeaderColumn(dataRow["Impronta"].ToString(), "Impronta", "Impronta"));

            // Aggiunta del campo profilato con rispettivo valore (solo se il campo ha lo storico attivo)
            if (dataRow["EnabledHistory"].ToString() == "1")
            {
                if (reportHeader[dataRow["Descrizione_Campo_Profilato"].ToString() + dataRow["ObjectId"].ToString()] == null)
                    reportHeader.Add(new HeaderProperty() { ColumnName = dataRow["Descrizione_Campo_Profilato"].ToString(), OriginalName = dataRow["Descrizione_Campo_Profilato"].ToString() + dataRow["ObjectId"].ToString() });

                String value = dataRow["ObjType"].ToString().ToLower() == "corrispondente" && !String.IsNullOrEmpty(dataRow["Valore"].ToString()) ? BusinessLogic.Utenti.UserManager.GetRoleDescriptionByIdCorrGlobali(dataRow["Valore"].ToString()) : dataRow["Valore"].ToString();

                row.Columns.Add(this.GenerateHeaderColumn(value, dataRow["Descrizione_Campo_Profilato"].ToString(), dataRow["Descrizione_Campo_Profilato"].ToString() + dataRow["ObjectId"].ToString()));
            }

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
        /// Funzione per l'aggiornamento di una riga del report
        /// </summary>
        /// <param name="row">Riga da aggiornare</param>
        /// <param name="reportHeader"></param>
        /// <param name="dataRow">DataRow da cui estrarre i dati con cui aggiornare la riga</param>
        private void UpdateRow(ReportMapRowProperty row, DataRow dataRow, HeaderColumnCollection reportHeader)
        {
            // Se non esiste già una colonna per un dato campo profilato, viene aggiunta
            if (reportHeader[dataRow["Descrizione_Campo_Profilato"].ToString() + dataRow["ObjectId"].ToString()] == null)
                reportHeader.Add(new HeaderProperty() { ColumnName = dataRow["Descrizione_Campo_Profilato"].ToString(), OriginalName = dataRow["Descrizione_Campo_Profilato"].ToString() + dataRow["ObjectId"].ToString() });
            
            // Se non esiste già un mapping di colonna, viene aggiunto
            if (row[dataRow["Descrizione_Campo_Profilato"].ToString() + dataRow["ObjectId"].ToString()] == null)
                row.Columns.Add(new ReportMapColumnProperty() { ColumnName = dataRow["Descrizione_Campo_Profilato"].ToString(), OriginalName = dataRow["Descrizione_Campo_Profilato"].ToString() + dataRow["ObjectId"].ToString() });

            // Aggiunta della data di annullamento
            //row[dataRow["Descrizione_Campo_Profilato"].ToString() + dataRow["ObjectId"].ToString()].Value = dataRow["Data_Annullamento"].ToString();

            // Aggiornamento del valore per il dato campo
            if (String.IsNullOrEmpty(row[dataRow["Descrizione_Campo_Profilato"].ToString() + dataRow["ObjectId"].ToString()].Value))
            {
                String value = dataRow["ObjType"].ToString().ToLower() == "corrispondente" && !String.IsNullOrEmpty(dataRow["Valore"].ToString()) ? BusinessLogic.Utenti.UserManager.GetRoleDescriptionByIdCorrGlobali(dataRow["Valore"].ToString()) : dataRow["Valore"].ToString();
                row[dataRow["Descrizione_Campo_Profilato"].ToString() + dataRow["ObjectId"].ToString()].Value = value;
            }
            else
                row[dataRow["Descrizione_Campo_Profilato"].ToString() + dataRow["ObjectId"].ToString()].Value += ", " + dataRow["Valore"].ToString();

            
        }

    }
}
