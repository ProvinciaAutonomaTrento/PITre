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
    [ReportGenerator(Name = "Notifiche Chiusura", ContextName = "NotificheChiusura", Key = "NotificheChiusura")]
    public class NotificheChiusuraReportGeneratorCommand : ReportGeneratorCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(NotificheChiusuraReportGeneratorCommand));

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
                string idIstanza = request.SearchFilters.Where(f => f.argomento == "idIstanza").FirstOrDefault().valore;

                request.Title = string.Format("{0} - Notifica di chiusura istanza", Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(request.UserInfo.idAmministrazione).Descrizione);
                request.SubTitle = string.Format("Stampa del {0}", DateTime.Now.ToShortDateString());
                request.AdditionalInformation = "Numero istanza: " + idIstanza + "\n";
                if (!(string.IsNullOrEmpty(nomePolicy)))
                    request.AdditionalInformation += "Policy utilizzata: " + nomePolicy + "\n";

                //istanzio la response
                response = new PrintReportResponse();

                //Recupero dataset
                //DataSet dataSet = dataExtractor.ExtractData(request);

                //Generazione dati per report
                List<DocsPaVO.Report.Report> report = new List<DocsPaVO.Report.Report>();

                //1 - report principale con esito verifiche
                //utilizziamo la stessa classe usata per le notifiche di rifiuto
                //le query sono le stesse
                report.AddRange(this.GetReport(dataExtractor, request, "NotificheRifiutoVerifiche"));

                //2 - report aggiuntivo con dettaglio policy
                report.AddRange(this.GetReport(dataExtractor, request, "NotificheRifiutoPolicy"));

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

        private List<DocsPaVO.Report.Report> GetReport(IReportDataExtractionBehavior dataExtractor, PrintReportRequest request, string contextName)
        {
            //imposto il context name
            request.ReportKey = contextName;
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
            report.Summary = String.Format("Righe estratte: {0}", report.ReportMapRow.Rows.Count);

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

            if (contextName == "NotificheRifiutoVerifiche")
            {
                header.Add(new HeaderProperty()
                {
                    ColumnName = "Size Byte",
                    ColumnSize = 70,
                    OriginalName = "SIZE_ITEM",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.Integer
                });

                header.Add(new HeaderProperty()
                {
                    ColumnName = "Firma",
                    ColumnSize = 40,
                    OriginalName = "ESITO_FIRMA",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });

                header.Add(new HeaderProperty()
                {
                    ColumnName = "Marca",
                    ColumnSize = 40,
                    OriginalName = "VALIDAZIONE_MARCA",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });

                header.Add(new HeaderProperty()
                {
                    ColumnName = "Formato",
                    ColumnSize = 40,
                    OriginalName = "VALIDAZIONE_FORMATO",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
            }
            if (contextName == "NotificheRifiutoPolicy")
            {
                header.Add(new HeaderProperty()
                {
                    ColumnName = "Criteri di verifica della Policy",
                    ColumnSize = 1000,
                    OriginalName = "MASK_VALIDAZIONE_POLICY",
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
            }

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

                //CAMPI PRESENTI NELLA PAGINA DELLE VERIFICHE:
                if (contextName == "NotificheRifiutoVerifiche")
                {

                    //SIZE
                    reportRow.Columns.Add(this.GenerateHeaderColumn(row["SIZE_ITEM"].ToString(), "Size Byte", "SIZE_ITEM"));

                    //FIRMA
                    string validazione = row["VALIDAZIONE_FIRMA"].Equals(DBNull.Value) ? string.Empty : row["VALIDAZIONE_FIRMA"].ToString();
                    int checkFirma = row["ESITO_FIRMA"].Equals(DBNull.Value) ? -1 : Convert.ToInt32(row["ESITO_FIRMA"]);
                    string esitoFirma = string.Empty;
                    if (validazione == "1" || validazione == "5")
                        esitoFirma = "Valido";
                    else if (checkFirma == 0 || validazione == "2" || validazione == "6")
                        esitoFirma = "Non Valido";
                    reportRow.Columns.Add(this.GenerateHeaderColumn(esitoFirma, "Firma", "ESITO_FIRMA"));

                    //MARCA
                    int checkMarca = row["VALIDAZIONE_MARCA"].Equals(DBNull.Value) ? -1 : Convert.ToInt32(row["VALIDAZIONE_MARCA"]);
                    string esitoMarca = string.Empty;
                    if (checkMarca == 1 && !(validazione == "3" || validazione == "7"))
                        esitoMarca = "Valido";
                    else if (checkMarca == 0 || validazione == "3" || validazione == "7")
                        esitoMarca = "Non Valido";
                    reportRow.Columns.Add(this.GenerateHeaderColumn(esitoMarca, "Marca", "VALIDAZIONE_MARCA"));

                    //FORMATO
                    int checkFormato = row["VALIDAZIONE_FORMATO"].Equals(DBNull.Value) ? -1 : Convert.ToInt32(row["VALIDAZIONE_FORMATO"]);
                    string esitoFormato = string.Empty;
                    if (checkFormato == 1 && !(validazione == "4" || validazione == "5" || validazione == "6" || validazione == "7"))
                        esitoFormato = "Valido";
                    else if (checkFormato == 0 || validazione == "4" || validazione == "5" || validazione == "6" || validazione == "7")
                        esitoFormato = "Non Valido";
                    reportRow.Columns.Add(this.GenerateHeaderColumn(esitoFormato, "Formato", "VALIDAZIONE_FORMATO"));
                }

                //CAMPI PRESENTI NELLA PAGINA DELLE POLICY:
                if (contextName == "NotificheRifiutoPolicy")
                {
                    //DETTAGLIO POLICY
                    string validationMask = row["MASK_VALIDAZIONE_POLICY"].Equals(DBNull.Value) ? string.Empty : row["MASK_VALIDAZIONE_POLICY"].ToString();
                    //string esitoPolicy = this.FormatDatiVerificaPolicy(validationMask);
                    string esitoPolicy = new ExportDati.ExportDatiManager().FormatDatiVerificaPolicy(validationMask);
                    reportRow.Columns.Add(this.GenerateHeaderColumn(esitoPolicy, "Criteri di Verifica della Policy", "MASK_VALIDAZIONE_POLICY"));
                    logger.Debug("stringa validazione policy: \n" + esitoPolicy);

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

        /// <summary>
        /// Formatta i dati relativi alla verifica della policy
        /// </summary>
        /// <param name="maskPolicyDoc">stringa mask della policy</param>
        /// <returns></returns>
        public string FormatDatiVerificaPolicy(string maskPolicyDoc)
        {
            string result_validazione_policy = string.Empty;
            if (!string.IsNullOrEmpty(maskPolicyDoc))
            {
                // decodifica il mask di validità della policy
                DocsPaVO.areaConservazione.ItemPolicyValidator policyValidator = new DocsPaVO.areaConservazione.ItemPolicyValidator();
                policyValidator = DocsPaVO.areaConservazione.ItemPolicyValidator.getItemPolicyValidator(maskPolicyDoc);

                // recupera la validità di ogni filtro
                // Filtro TIPOLOGIA DOCUMENTO
                if (policyValidator.TipologiaDocumento != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("Tipologia del Documento: {0}\n", policyValidator.TipologiaDocumento == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro STATO DOCUMENTO
                if (policyValidator.StatoDocumento != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("Stato del Documento: {0}\n", policyValidator.StatoDocumento == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro AOO CREATORE
                if (policyValidator.AooCreator != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("AOO Creatore: {0}\n", policyValidator.AooCreator == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro RF Creatore
                if (policyValidator.Rf_Creator != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("RF Creatore: {0}\n", policyValidator.Rf_Creator == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro UO creatore
                if (policyValidator.Uo_Creator != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("Uo Creatore: {0}\n", policyValidator.Uo_Creator == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro Includi anche sottoposti
                if (policyValidator.Uo_Creator != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("Uo Creatore: {0}\n", policyValidator.Uo_Creator == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro Titoloario
                if (policyValidator.Titolario != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("Titolario: {0}\n", policyValidator.Titolario == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro Codice Classificazioni
                if (policyValidator.Classificazione != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("Classificazione: {0}\n", policyValidator.Classificazione == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro tipo documento: arrivo
                if (policyValidator.DocArrivo != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("Tipo Doc. Arrivo: {0}\n", policyValidator.DocArrivo == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro tipo documento: partenza
                if (policyValidator.DocPartenza != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("Tipo Doc. Partenza: {0}\n", policyValidator.DocPartenza == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro tipo documento: Interno
                if (policyValidator.DocInterno != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("Tipo Doc. Interno: {0}\n", policyValidator.DocInterno == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro tipo documento: NP
                if (policyValidator.DocNP != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("Tipo Doc. NP: {0}\n", policyValidator.DocNP == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro includi solo i documenti digitali
                if (policyValidator.DocDigitale != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("Doc. Digitale: {0}\n", policyValidator.DocDigitale == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro DOCUMENTI FIRMATI: Includi solo documenti firmati
                if (policyValidator.DocFirmato != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("Doc. Firmato: {0}\n", policyValidator.DocFirmato == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro DATA CREAZIONE + DATA DA | DATA A
                if (policyValidator.DocDataCreazione != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("Data Creazione: {0}\n", policyValidator.DocDataCreazione == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro DATA DI PROTOCOLLAZIONE
                if (policyValidator.DocDataProtocollazione != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("Data Protocollazione: {0}\n", policyValidator.DocDataProtocollazione == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro FORMATI DOCUMENTI 
                if (policyValidator.DocFormato != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format("Formato: {0}\n", policyValidator.DocFormato == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                return (!string.IsNullOrEmpty(result_validazione_policy)) ? result_validazione_policy.Substring(3) : string.Empty;
            }
            else return string.Empty;
        }

    }
}
