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
    [ReportGenerator(Name="Export Policy PARER", ContextName="AmmExportPolicyPARER", Key="AmmExportPolicyPARER")]
    public class AmmExportPolicyPARERReportGeneratorCommand : ReportGeneratorCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(AmmExportPolicyPARERReportGeneratorCommand));

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

                DataSet dataSet = dataExtractor.ExtractData(request);

                List<DocsPaVO.Report.Report> report = this.CreateReport(dataSet, request);

                response.Document = reportGeneration.GenerateReport(request, report);

                // Devo selezionare il tipo di report (policy documenti/policy stampe)
                string tipo = request.SearchFilters.Where(f => f.argomento.Equals("type")).FirstOrDefault().valore;
            }
            catch (Exception ex)
            {
                throw new ReportGenerationFailedException(ex.Message);
            }

            return response;
        }

        protected override List<DocsPaVO.Report.Report> CreateReport(DataSet dataSet, PrintReportRequest request)
        {
            // Report da restiutuire
            DocsPaVO.Report.Report report = new DocsPaVO.Report.Report();

            // Devo selezionare il tipo di report (policy documenti/policy stampe)
            string tipo = request.SearchFilters.Where(f => f.argomento.Equals("type")).FirstOrDefault().valore;

            if (tipo.Equals("DOC"))
            {
                report.Title = "Dettaglio policy documenti";
            }
            if (tipo.Equals("ST"))
            {
                report.Title = "Dettaglio policy stampe";
            }
            report.CreationDate = DateTime.Now;
            report.Subtitle = "Report generato in data " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            report.AdditionalInformation = request.AdditionalInformation;
            report.ReportMapRow = new ReportMapRow();
            report.ReportHeader = new HeaderColumnCollection();

            // Generazione delle righe del report
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null)
            {
                // Generazione dell'header del report
                report.ReportHeader = this.GenerateReportHeader(dataSet, request.ColumnsToExport, tipo);

                report.ReportMapRow.Rows.AddRange(this.GenerateReportRows(dataSet, report.ReportHeader, tipo));
            }

            return new List<DocsPaVO.Report.Report>() { report };
        }

        protected HeaderColumnCollection GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport, string type)
        {
            HeaderColumnCollection header = new HeaderColumnCollection();

            #region Policy documenti
            if (type.Equals("DOC"))
            {
                header.Add(new HeaderProperty()
                    {
                        ColumnName = "CODICE",
                        ColumnSize = 50,
                        OriginalName = "VAR_CODICE",
                        Export = true,
                        DataType = HeaderProperty.ContentDataType.String
                    });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "DESCRIZIONE",
                    ColumnSize = 300,
                    OriginalName = "VAR_DESCRIZIONE",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "RUOLO RESPONSABILE",
                    ColumnSize = 200,
                    OriginalName = "RUOLO_RESP",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "STATO POLICY",
                    ColumnSize = 50,
                    OriginalName = "STATO_POLICY",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "PERIODICITA'",
                    ColumnSize = 150,
                    OriginalName = "PERIODICITA",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "STATO CONSERVAZIONE",
                    ColumnSize = 100,
                    OriginalName = "STATO_CONS",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "TIPO DOC.",
                    ColumnSize = 150,
                    OriginalName = "TIPO_DOC",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "TIPOLOGIA",
                    ColumnSize = 200,
                    OriginalName = "TIPOLOGIA",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "STATO",
                    ColumnSize = 100,
                    OriginalName = "STATO",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "REGISTRO/AOO",
                    ColumnSize = 100,
                    OriginalName = "REGISTRO",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "RF",
                    ColumnSize = 100,
                    OriginalName = "RF",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "UO CREATRICE",
                    ColumnSize = 100,
                    OriginalName = "UO_CREATRICE",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "CLASSIFICAZIONE",
                    ColumnSize = 200,
                    OriginalName = "CLASSIFICAZIONE",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "TITOLARIO",
                    ColumnSize = 200,
                    OriginalName = "TITOLARIO",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "SOLO DOC. DIGITALI",
                    ColumnSize = 100,
                    OriginalName = "DOC_DIGITALI",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "FORMATI DOCUMENTO",
                    ColumnSize = 200,
                    OriginalName = "FORMATI_DOC",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "FIRMATI",
                    ColumnSize = 100,
                    OriginalName = "FIRMATI",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "MARCATI",
                    ColumnSize = 100,
                    OriginalName = "MARCATI",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "DATA CREAZIONE",
                    ColumnSize = 150,
                    OriginalName = "DATA_CREAZIONE",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "DATA PROTOCOLLAZIONE",
                    ColumnSize = 150,
                    OriginalName = "DATA_PROTO",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "CAMPI PROFILATI",
                    ColumnSize = 300,
                    OriginalName = "CAMPI_PROFILATI",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });

                
            }
            #endregion

            #region Policy stampe
            if (type.Equals("ST"))
            {
                header.Add(new HeaderProperty()
                {
                    ColumnName = "CODICE",
                    ColumnSize = 50,
                    OriginalName = "VAR_CODICE",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "DESCRIZIONE",
                    ColumnSize = 300,
                    OriginalName = "VAR_DESCRIZIONE",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "RUOLO RESPONSABILE",
                    ColumnSize = 200,
                    OriginalName = "RUOLO_RESP",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "STATO POLICY",
                    ColumnSize = 50,
                    OriginalName = "STATO_POLICY",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "PERIODICITA'",
                    ColumnSize = 150,
                    OriginalName = "PERIODICITA",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "STATO CONSERVAZIONE",
                    ColumnSize = 100,
                    OriginalName = "STATO_CONS",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "TIPO REGISTRO STAMPA",
                    ColumnSize = 100,
                    OriginalName = "TIPO_REG_PRINT",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "REPERTORIO",
                    ColumnSize = 200,
                    OriginalName = "REPERTORIO",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "REGISTRO/AOO",
                    ColumnSize = 100,
                    OriginalName = "REGISTRO",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "RF",
                    ColumnSize = 100,
                    OriginalName = "RF",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "ANNO STAMPA",
                    ColumnSize = 50,
                    OriginalName = "ANNO_STAMPA",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                header.Add(new HeaderProperty()
                {
                    ColumnName = "DATA STAMPA",
                    ColumnSize = 150,
                    OriginalName = "DATA_STAMPA",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
                
            }
            #endregion

            return header;

        }

        protected IEnumerable<ReportMapRowProperty> GenerateReportRows(DataSet dataSet, HeaderColumnCollection reportHeader, string type)
        {
            List<ReportMapRowProperty> rows = new List<ReportMapRowProperty>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                //creo una nuova riga
                ReportMapRowProperty reportRow = new ReportMapRowProperty();

                #region Policy documenti
                if (type.Equals("DOC"))
                {
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_CODICE"].ToString(), "CODICE", "VAR_CODICE"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_DESCRIZIONE"].ToString(), "DESCRIZIONE", "VAR_DESCRIZIONE"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["RUOLO_RESP"].ToString(), "RUOLO RESPONSABILE", "RUOLO_RESP"));
                    if (row["CHA_ATTIVA"].ToString().Equals("1"))
                        reportRow.Columns.Add(this.GenerateHeaderColumn("Attiva", "STATO", "STATO"));
                    else
                        reportRow.Columns.Add(this.GenerateHeaderColumn("Non attiva", "STATO POLICY", "STATO_POLICY"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetPeriodicita(row), "PERIODICITA'", "PERIODICITA"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetStatoConservazione(row), "STATO CONSERVAZIONE", "STATO_CONS"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetTipoDoc(row), "TIPO DOC.", "TIPO_DOC"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["TIPOLOGIA"].ToString(), "TIPOLOGIA", "TIPOLOGIA"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetStatoDiagramma(row), "STATO", "STATO"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["REGISTRO"].ToString(), "REGISTRO/AOO", "REGISTRO"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["RF"].ToString(), "RF", "RF"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetUoCreatrice(row), "UO CREATRICE", "UO_CREATRICE"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetClassificazione(row), "CLASSIFICAZIONE", "CLASSIFICAZIONE"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetTitolario(row["ID_TITOLARIO"].ToString()), "TITOLARIO", "TITOLARIO"));
                    if (!string.IsNullOrEmpty(row["CHA_DOC_DIGITALI"].ToString()) && row["CHA_DOC_DIGITALI"].ToString().Equals("1"))
                        reportRow.Columns.Add(this.GenerateHeaderColumn("Sì", "SOLO DOC. DIGITALI", "DOC_DIGITALI"));
                    else
                        reportRow.Columns.Add(this.GenerateHeaderColumn(string.Empty, "DOC_DIGITALI", "DOC_DIGITALI"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetFormatiDocumento(row["SYSTEM_ID"].ToString()), "FORMATI DOCUMENTO", "FORMATI_DOC"));
                    if (!string.IsNullOrEmpty(row["CHA_FIRMATO"].ToString()))
                        reportRow.Columns.Add(this.GenerateHeaderColumn(row["CHA_FIRMATO"].ToString().Equals("1") ? "Firmati" : "Non firmati", "FIRMATI", "FIRMATI"));
                    else
                        reportRow.Columns.Add(this.GenerateHeaderColumn(string.Empty, "FIRMATI", "FIRMATI"));
                    if (!string.IsNullOrEmpty(row["CHA_MARCATO"].ToString()))
                    {
                        if (row["CHA_MARCATO"].ToString().Equals("1"))
                        {
                            if (!string.IsNullOrEmpty(row["CHA_SCADENZA_TIMESTAMP"].ToString()))
                                reportRow.Columns.Add(this.GenerateHeaderColumn("Con timestamp " + this.GetScadenzaMarca(row["CHA_SCADENZA_TIMESTAMP"].ToString()), "MARCATI", "MARCATI"));
                            else
                                reportRow.Columns.Add(this.GenerateHeaderColumn("Con timestamp", "MARCATI", "MARCATI"));
                        }
                        else
                            reportRow.Columns.Add(this.GenerateHeaderColumn("Senza timestamp", "MARCATI", "MARCATI"));
                    }
                    else
                        reportRow.Columns.Add(this.GenerateHeaderColumn(string.Empty, "MARCATI", "MARCATI"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetData(row["CHA_DATA_CREAZIONE_TIPO"].ToString(), row["DATA_CREAZ_FROM"].ToString(), row["DATA_CREAZ_TO"].ToString(), row["NUM_GIORNI_DATA_CREAZIONE"].ToString()), "DATA CREAZIONE", "DATA_CREAZIONE"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetData(row["CHA_DATA_PROTO_TIPO"].ToString(), row["DATA_PROT_FROM"].ToString(), row["DATA_PROT_TO"].ToString(), row["NUM_GIORNI_DATA_PROTO"].ToString()), "DATA PROTOCOLLAZIONE", "DATA_PROTO"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetCampiProfilati(row["SYSTEM_ID"].ToString(), row["ID_TEMPLATE"].ToString()), "CAMPI PROFILATI", "CAMPI_PROFILATI"));

                }
                #endregion

                #region Policy stampe
                if (type.Equals("ST"))
                {
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_CODICE"].ToString(), "CODICE", "VAR_CODICE"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["VAR_DESCRIZIONE"].ToString(), "DESCRIZIONE", "VAR_DESCRIZIONE"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["RUOLO_RESP"].ToString(), "RUOLO RESPONSABILE", "RUOLO_RESP"));
                    if (row["CHA_ATTIVA"].ToString().Equals("1"))
                        reportRow.Columns.Add(this.GenerateHeaderColumn("Attiva", "STATO", "STATO"));
                    else
                        reportRow.Columns.Add(this.GenerateHeaderColumn("Non attiva", "STATO POLICY", "STATO_POLICY"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetPeriodicita(row), "PERIODICITA'", "PERIODICITA"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetStatoConservazione(row), "STATO CONSERVAZIONE", "STATO_CONS"));
                    if (row["CHA_TIPO_REGISTRO_STAMPA"].ToString().Equals("R"))
                        reportRow.Columns.Add(this.GenerateHeaderColumn("Registro di protocollo", "TIPO REGISTRO STAMPA", "TIPO_REG_PRINT"));
                    else if (row["CHA_TIPO_REGISTRO_STAMPA"].ToString().Equals("C"))
                        reportRow.Columns.Add(this.GenerateHeaderColumn("Registro di repertorio", "TIPO REGISTRO STAMPA", "TIPO_REG_PRINT"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["REPERTORIO"].ToString(), "REPERTORIO", "REPERTORIO"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["REGISTRO"].ToString(), "REGISTRO/AOO", "REGISTRO"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["RF"].ToString(), "RF", "RF"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["NUM_ANNO_STAMPA"].ToString(), "ANNO STAMPA", "ANNO_STAMPA"));
                    reportRow.Columns.Add(this.GenerateHeaderColumn(this.GetData(row["CHA_DATA_STAMPA_TIPO"].ToString(), row["DATA_ST_FROM"].ToString(), row["DATA_ST_TO"].ToString(), row["NUM_GIORNI_DATA_STAMPA"].ToString()), "DATA STAMPA", "DATA_STAMPA"));
                        
                }
                #endregion

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

        private string GetPeriodicita(DataRow row)
        {
            string result = string.Empty;
            switch (row["CHA_PERIODICITA"].ToString())
            {
                case "D":
                    result = "Giornaliera";
                    break;
                case "W":
                    string giorno = row["CHA_ESECUZIONE_GIORNO"].ToString();
                    string weekday = string.Empty;
                    if (giorno.Equals("1"))
                        weekday = "lunedì";
                    if (giorno.Equals("2"))
                        weekday = "martedì";
                    if (giorno.Equals("3"))
                        weekday = "mercoledì";
                    if (giorno.Equals("4"))
                        weekday = "giovedì";
                    if (giorno.Equals("5"))
                        weekday = "venerdì";
                    if (giorno.Equals("6"))
                        weekday = "sabato";
                    if (giorno.Equals("7"))
                        weekday = "domenica";
                    result = string.Format("Settimanale ({0})", weekday);
                    break;
                case "M":
                    if (row["CHA_ESECUZIONE_GIORNO"].ToString().Equals("31"))
                        result = "Mensile (fine mese)";
                    else
                        result = "Mensile (il giorno " + row["CHA_ESECUZIONE_GIORNO"].ToString() + ")";
                    break;
                case "Y":
                    result = "Annuale (il giorno " + row["CHA_ESECUZIONE_GIORNO"].ToString() + "/" + row["CHA_ESECUZIONE_MESE"].ToString() + ")";
                    break;
                case "O":
                    result = "Una tantum (il giorno " + row["DATA_EXEC_POLICY"].ToString() + ")";
                    break;
            }

            return result;
        }

        private string GetStatoConservazione(DataRow row)
        {
            string result = string.Empty;
            switch (row["CHA_STATO_VERSAMENTO"].ToString())
            {
                case "R":
                    result = "Rifiutato";
                    break;
                case "F":
                    result = "Versamento fallito";
                    break;
                default:
                    result = "Non conservato";
                    break;
            }

            return result;
        }

        private string GetTipoDoc(DataRow row)
        {
            string result = string.Empty;
            if (row["CHA_TIPO_PROTO_A"].ToString().Equals("1"))
            {
                result = result + "Arrivo, ";
            }
            if (row["CHA_TIPO_PROTO_P"].ToString().Equals("1"))
            {
                result = result + "Partenza, ";
            }
            if (row["CHA_TIPO_PROTO_I"].ToString().Equals("1"))
            {
                result = result + "Interno, ";
            }
            if (row["CHA_TIPO_PROTO_G"].ToString().Equals("1"))
            {
                result = result + "Non Protocollato";
            }

            if(result.Trim().EndsWith(",")) {
                result = result.Trim();
                result = result.Remove(result.Length - 1);
            }

            return result;
        }

        private string GetUoCreatrice(DataRow row)
        {
            string result = row["UO_CREATRICE"].ToString();
            if (row["CHA_UO_SOTTOPOSTE"].Equals("1"))
                result = result + " (includi UO sottoposte)";

            return result;
        }

        private string GetClassificazione(DataRow row)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(row["CLASSIFICA"].ToString()))
            {
                result = row["CLASSIFICA"].ToString();
                if (row["CHA_TIPO_CLASS"].ToString().Equals("C"))
                    result = result + " (solo classificati)";
                else if (row["CHA_TIPO_CLASS"].ToString().Equals("F"))
                    result = result + " (solo fascicolati)";
                else if (row["CHA_TIPO_CLASS"].ToString().Equals("CF"))
                    result = result + " (classificati e fascicolati)";
            }

            return result;
        }

        private string GetData(string tipo, string from, string to, string days)
        {
            string result = string.Empty;
            switch (tipo)
            {
                case "S":
                    if(!string.IsNullOrEmpty(from))
                        result = string.Format("Valore singolo (il giorno {0})", from);
                    break;
                case "R":
                    if(!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
                        result = string.Format("Intervallo (dal giorno {0} al giorno {1})", from, to);
                    else if(!string.IsNullOrEmpty(from))
                        result = string.Format("Intervallo (dal giorno {0})", from);
                    else if(!string.IsNullOrEmpty(to))
                        result = string.Format("Intervallo (fino al giorno {0})", to);
                    break;
                case "T":
                    result = "Oggi";
                    break;
                case "W":
                    result = "Settimana corrente";
                    break;
                case "M":
                    result = "Mese corrente";
                    break;
                case "Y":
                    result = "Anno corrente";
                    break;
                case "B":
                    result = "Ieri";
                    break;
                case "V":
                    result = "Settimana precedente";
                    break;
                case "N":
                    result = "Mese precedente";
                    break;
                case "X":
                    result = "Anno precedente";
                    break;
                case "P":
                    result = days + " giorni prima";
                    break;
            }

            return result;
        }

        private string GetStatoDiagramma(DataRow row)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(row["STATO_DIAGRAMMA"].ToString()))
            {
                if (row["CHA_STATO"].ToString().Equals("1"))
                    result = "Uguale a " + row["STATO_DIAGRAMMA"].ToString();
                else
                    result = "Diverso da " + row["STATO_DIAGRAMMA"].ToString();
            }
            return result;
        }

        private string GetFormatiDocumento(string id)
        {
            string result = string.Empty;
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            List<DocsPaVO.FormatiDocumento.SupportedFileType> lista = new List<DocsPaVO.FormatiDocumento.SupportedFileType>();
            lista = c.GetFormatiDocumentoPolicy(id);

            if (lista != null && lista.Count > 0)
            {
                for (int i = 0; i < lista.Count; i++)
                {
                    DocsPaVO.FormatiDocumento.SupportedFileType file = lista[i];
                    result = result + file.FileExtension.ToUpper();
                    if (i < lista.Count - 1)
                        result = result + ", ";
                }
            }
            else
            {
                result = "Tutti";
            }

            return result;
        }

        private string GetCampiProfilati(string idPolicy, string idTemplate)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(idTemplate) && !idTemplate.Equals("0"))
            {
                DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
                DocsPaVO.ProfilazioneDinamica.Templates template = c.GetTemplateDocumento(idPolicy, idTemplate);

                for (int i = 0; i < template.ELENCO_OGGETTI.Count; i++)
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[i];
                    if (!string.IsNullOrEmpty(ogg.VALORE_DATABASE) || ogg.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione"))
                    {
                        string valore = ogg.VALORE_DATABASE;
                        switch (ogg.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "CasellaDiSelezione":
                                valore = string.Empty;
                                foreach (string v in ogg.VALORI_SELEZIONATI)
                                {
                                    if (!string.IsNullOrEmpty(v))
                                        valore = valore + v + ",";
                                }
                                if (valore.EndsWith(","))
                                    valore = valore.Remove(valore.Length - 1);
                                break;
                            case "Contatore":
                            case "Data":
                                valore = ogg.VALORE_DATABASE.Replace("@", "-");
                                break;
                            case "Corrispondente":
                                if (!string.IsNullOrEmpty(ogg.VALORE_DATABASE))
                                    valore = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(ogg.VALORE_DATABASE).descrizione;
                                break;
                        }
                        if (!string.IsNullOrEmpty(result))
                            result = result + "; ";
                        if(!string.IsNullOrEmpty(valore))
                            result = result + ogg.DESCRIZIONE + "=" + valore;

                    }
                    
                }
            }
            return result;
        }

        private string GetTitolario(string idTitolario)
        {
            string result = string.Empty;

            DocsPaVO.amministrazione.OrgTitolario titolario = BusinessLogic.Amministrazione.TitolarioManager.getTitolarioById(idTitolario);
            if (titolario != null && !string.IsNullOrEmpty(titolario.Descrizione))
                result = titolario.Descrizione;

            return result;
        }

        private string GetScadenzaMarca(string scadenza)
        {
            string result = string.Empty;
            switch (scadenza)
            {
                case "E":
                    result = "(scaduti)";
                    break;
                case "W":
                    result = "(scadenza entro la settimana corrente)";
                    break;
                case "M":
                    result = "(scadenza entro il mese corrente)";
                    break;
                case "Y":
                    result = "(scadenza entro l'anno corrente)";
                    break;
            }

            return result;
        }

    }
}
