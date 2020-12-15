using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DocsPaVO.RegistroAccessi;
using DocsPaVO.RegistroAccessi.FOIA;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using log4net;


namespace BusinessLogic.RegistroAccessi
{
    public class RegistroAccessiManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(RegistroAccessiManager));

        public static RegistroAccessiReportResponse PubblicaRegistroAccessi(RegistroAccessiReportRequest request)
        {
            logger.Debug("BEGIN");

            RegistroAccessiReportResponse response = new RegistroAccessiReportResponse();

            try
            {
                // Lista dei report da produrre
                List<DocsPaVO.documento.FileDocumento> reportList = new List<DocsPaVO.documento.FileDocumento>();

                // Accesso documentale
                reportList.AddRange(RegistroAccessiGetReport(request.filters, "Accesso Documentale"));

                // Accesso generalizzato e civico
                reportList.AddRange(RegistroAccessiGetReport(request.filters, "Accesso Generalizzato e Civico"));

                // Archivio da restituire
                DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
                fileDoc.contentType = "application/octet-stream";
                fileDoc.name = String.Format("Registro_Accessi_{0}.zip", DateTime.Now.ToString("yyyyMMdd"));
                fileDoc.fullName = String.Format("Registro_Accessi_{0}.zip", DateTime.Now.ToString("yyyyMMdd"));
                fileDoc.estensioneFile = "zip";

                fileDoc.content = RegistroAccessiCreateZipArchive(reportList);
                fileDoc.length = fileDoc.content.Length;

                response.document = fileDoc;
                response.success = true;

            }
            catch(Exception ex)
            {
                logger.DebugFormat("Errore in RegistroAccessiManager>PubblicaRegistroAccessi.\r\n{0}\r\n{1}", ex.Message, ex.StackTrace);
                response.success = false;
                response.document = null;
            }

            logger.Debug("END");

            return response;
        }

        public static String GetAdditionalInformation(List<DocsPaVO.filtri.FiltroRicerca> searchFilters)
        {
            String additionalInformation = String.Empty;

            String creationDateInterval = searchFilters.Where(f => f.argomento == "data_creazione").FirstOrDefault().valore;
            String creationDateFrom = searchFilters.Where(f => f.argomento == "data_creazione_da").FirstOrDefault().valore;
            String creationDateTo = searchFilters.Where(f => f.argomento == "data_creazione_a").FirstOrDefault().valore;

            if (creationDateInterval == "0")
            {
                if (!String.IsNullOrEmpty(creationDateFrom))
                {
                    additionalInformation = String.Format("Accessi del giorno {0}", creationDateFrom);
                }
            }
            else if (creationDateInterval == "1")
            {
                if (!String.IsNullOrEmpty(creationDateFrom) && !String.IsNullOrEmpty(creationDateTo))
                {
                    additionalInformation = String.Format("Accessi nel periodo dal {0} al {1}", creationDateFrom, creationDateTo);
                }
                else if (!String.IsNullOrEmpty(creationDateFrom) && String.IsNullOrEmpty(creationDateTo))
                {
                    additionalInformation = String.Format("Accessi successivi al giorno {0}", creationDateFrom);
                }
                else if (!String.IsNullOrEmpty(creationDateTo) && String.IsNullOrEmpty(creationDateFrom))
                {
                    additionalInformation = String.Format("Accessi precedenti al giorno {0}", creationDateTo);
                }
            }

            return additionalInformation;
        }

        private static List<DocsPaVO.documento.FileDocumento> RegistroAccessiGetReport(List<DocsPaVO.filtri.FiltroRicerca> searchFilters, String typology)
        {
            logger.Debug("BEGIN");

            List<DocsPaVO.documento.FileDocumento> reports = new List<DocsPaVO.documento.FileDocumento>();

            searchFilters.Where(f => f.argomento == "tipologia").FirstOrDefault().valore = typology;

            string descrizioneAmm = string.Empty;
            try
            {
                string idAmm = searchFilters.Where(f => f.argomento == "id_amm").FirstOrDefault().valore;
                if (!string.IsNullOrEmpty(idAmm))
                {
                    descrizioneAmm = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(idAmm).Descrizione;
                }
            }
            catch (Exception ex)
            {
                descrizioneAmm = string.Empty;
            }

            // Estrazione dataset da fornire ai servizi di reportistica
            DataSet dataSet = RegistroAccessiGetDataset(searchFilters);

            if (dataSet == null)
            {
                throw new Exception("Errore nell'estrazione del dataset");
            }

            // Costruisco la request da passare al servizio di reportistica
            // per generare PDF e EXCEL/ODS

            DocsPaVO.Report.PrintReportRequestDataset printRequest = new DocsPaVO.Report.PrintReportRequestDataset();
            DocsPaVO.Report.PrintReportResponse printResponse = new DocsPaVO.Report.PrintReportResponse();

            printRequest.ReportKey = "RegistroAccessiPublish";
            printRequest.ContextName = "RegistroAccessiPublish";
            printRequest.Title = string.IsNullOrEmpty(descrizioneAmm) ? "Registro degli accessi" : "Registro degli accessi - " + descrizioneAmm;
            printRequest.SubTitle = typology;
            printRequest.AdditionalInformation = GetAdditionalInformation(searchFilters);
            printRequest.InputDataset = dataSet;

            String ext = String.Empty;
            String typologyFilename = typology.Replace(" ", "_");

            // 1 - Report Foglio di calcolo
            printRequest.ReportType = RegistroAccessiGetReportType(searchFilters, out ext);
            printResponse = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(printRequest);
            printResponse.Document.name = String.Format("{0}_{1}.{2}", typologyFilename, DateTime.Now.ToString("yyyyMMdd"), ext);
            printResponse.Document.fullName = String.Format("{0}_{1}.{2}", typologyFilename, DateTime.Now.ToString("yyyyMMdd"), ext);
            reports.Add(printResponse.Document);

            // 2 - Report PDF
            printRequest.ReportType = DocsPaVO.Report.ReportTypeEnum.PDF;
            printResponse = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(printRequest);
            printResponse.Document.name = String.Format("{0}_{1}.pdf", typologyFilename, DateTime.Now.ToString("yyyyMMdd"));
            printResponse.Document.fullName = String.Format("{0}_{1}.pdf", typologyFilename, DateTime.Now.ToString("yyyyMMdd"));
            reports.Add(printResponse.Document);

            // 3 - Report XML FOIA
            DocsPaVO.documento.FileDocumento fileDocXml = new DocsPaVO.documento.FileDocumento();
            fileDocXml.content = RegistroAccessiGetXML(dataSet, typology);
            if(fileDocXml.content == null)
            {
                throw new Exception("Errore nella generazione dell'XML FOIA");
            }
            fileDocXml.length = fileDocXml.content.Length;
            fileDocXml.contentType = "text/xml";
            fileDocXml.estensioneFile = "xml";
            fileDocXml.name = String.Format("{0}_{1}.xml", typologyFilename, DateTime.Now.ToString("yyyyMMdd"));
            fileDocXml.fullName = String.Format("{0}_{1}.xml", typologyFilename, DateTime.Now.ToString("yyyyMMdd"));
            reports.Add(fileDocXml);

            return reports;
        }

        private static DataSet RegistroAccessiGetDataset(List<DocsPaVO.filtri.FiltroRicerca> filters)
        {
            DocsPaDB.RegistroAccessi reg = new DocsPaDB.RegistroAccessi();
            return reg.GetDataSetRegistroAccessi(filters);
        }

        private static DocsPaVO.Report.ReportTypeEnum RegistroAccessiGetReportType(List<DocsPaVO.filtri.FiltroRicerca> filters, out String extString)
        {
            String reportType = filters.Where(f => f.argomento == "formato_export").FirstOrDefault().valore;
            DocsPaVO.Report.ReportTypeEnum reportTypeEnum;

            if(!String.IsNullOrEmpty(reportType) && reportType.ToUpper() == "ODS")
            {
                reportTypeEnum = DocsPaVO.Report.ReportTypeEnum.ODS;
                extString = "ods";
            }
            else
            {
                reportTypeEnum = DocsPaVO.Report.ReportTypeEnum.Excel;
                extString = "xls";
            }

            return reportTypeEnum;
        }

        private static Byte[] RegistroAccessiGetXML(DataSet dataSet, String typology)
        {
            try
            {
                logger.Debug("BEGIN");

                logger.Debug("Costruzione XML secondo standard FOIA-EXT");

                List<RichiestaDiAccesso> list = new List<RichiestaDiAccesso>();

                String folderId = String.Empty;

                foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                {
                    if (!String.IsNullOrEmpty(dataRow["ID_PROJECT"].ToString()))
                    {
                        if (String.IsNullOrEmpty(folderId) || !dataRow["ID_PROJECT"].ToString().Equals(folderId))
                        {
                            // E' una nuova riga
                            folderId = dataRow["ID_PROJECT"].ToString();

                            RichiestaDiAccesso rda = new RichiestaDiAccesso();
                            rda.StatoRichiesta = dataRow["STATO_FASC"].ToString() == "C" ? StatoType.Chiusa : StatoType.InCorso;
                            rda.Oggetto = new Oggetto() { Sintesi = dataRow["DESCRIZIONE"].ToString() };

                            SetValoreCampoProfilato(rda, dataRow["NOME_CAMPO"].ToString(), dataRow["VALORE_CAMPO"].ToString());
                            
                            list.Add(rda);
                        }
                        else
                        {
                            // E' una riga esistente
                            RichiestaDiAccesso rda = list.Last();
                            if(rda != null)
                            {
                                SetValoreCampoProfilato(rda, dataRow["NOME_CAMPO"].ToString(), dataRow["VALORE_CAMPO"].ToString());
                            }
                        }
                    }
                }

                logger.Debug("Analisi richieste di accesso...");

                foreach(RichiestaDiAccesso rda in list)
                {
                    SetStatoRichiesta(rda, typology);
                    CheckValues(rda);
                }

                logger.Debug("Serializzazione...");

                Byte[] serializedXML = GetSerializedXML(list);

                logger.Debug("END");

                return serializedXML;
            }
            catch(Exception ex)
            {
                logger.DebugFormat("Errore nella produzione dell'XML FOIA.\r\n{0}\r\n{1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        private static void SetValoreCampoProfilato(RichiestaDiAccesso rda, String name, String value)
        {
            switch(name.ToUpper().Trim())
            {
                case "DATA DI ARRIVO DELLA DOMANDA":
                    rda.DataCreazione = FormatDate(value);
                    break;
                case "PRESENZA CONTRO INTERESSATI":
                case "PRESENZA CONTROINTERESSATI":
                    if (value.ToUpper() == "SI")
                        rda.PresenzaControinteressati = true;
                    else
                        rda.PresenzaControinteressati = false;
                    break;
                case "ESITO":                        
                    if (!String.IsNullOrEmpty(value))
                    {
                        if(rda.Esito == null)
                        {
                            rda.Esito = new Esito();
                        }
                        SetEsito(rda.Esito, value);
                    }
                    else
                    {
                        rda.Esito = null;
                    }
                    break;
                case "DATA RISPOSTA":
                    if(!String.IsNullOrEmpty(value))
                    {
                        if(rda.Esito == null)
                        {
                            rda.Esito = new Esito();
                        }
                        rda.Esito.DataEsito = FormatDate(value);
                    }
                    break;
                case "MOTIVI DEL RIFIUTO PARZIALE, DEL RIFIUTO TOTALE O DEL DIFFERIMENTO":
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (rda.Esito == null)
                            rda.Esito = new Esito();

                        SetMotivoRifiuto(rda.Esito, value);
                    }
                    break;
                case "ALTRO - MOTIVI DEL RIFIUTO PARZIALE, DEL RIFIUTO TOTALE O DEL DIFFERIMENTO":
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (rda.Esito == null)
                            rda.Esito = new Esito();
                        rda.Esito.SintesiMotivazione = value;
                    }
                    break;
                case "RIESAME - ESITO":
                    if (!String.IsNullOrEmpty(value) && value.ToUpper() != "DIFFERIMENTO")
                    {
                        if (rda.EsitoRiesame == null)
                        {
                            rda.EsitoRiesame = new Esito();
                        }
                        SetEsito(rda.EsitoRiesame, value);
                    }
                    else
                    {
                        rda.EsitoRiesame = null;
                    }
                    break;
                case "RIESAME - MOTIVI DEL RIFIUTO PARZIALE, DEL RIFIUTO TOTALE O DEL DIFFERIMENTO":
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (rda.EsitoRiesame == null)
                            rda.EsitoRiesame = new Esito();

                        SetMotivoRifiuto(rda.EsitoRiesame, value);
                    }
                    break;
                case "RIESAME - DATA RISPOSTA":
                    if(!String.IsNullOrEmpty(value))
                    {
                        if (rda.EsitoRiesame == null)
                            rda.EsitoRiesame = new Esito();

                        rda.EsitoRiesame.DataEsito = FormatDate(value);
                    }
                    break;
                case "RICORSO - ESITO":
                    if (!String.IsNullOrEmpty(value) && value.ToUpper() != "DIFFERIMENTO")
                    {
                        if (rda.EsitoRicorso == null)
                        {
                            rda.EsitoRicorso = new Esito();
                        }
                        SetEsito(rda.EsitoRicorso, value);
                    }
                    else
                    {
                        rda.EsitoRicorso = null;
                    }
                    break;
                case "RIESAME - DATA DI PRESENTAZIONE DELLA DOMANDA":
                    if(!String.IsNullOrEmpty(value))
                    {
                        rda.DataNotificaRiesame = value;
                    }
                    break;
                case "RICORSO – DATA DI NOTIFICAZIONE DEL RICORSO GIURISDIZIONALE DELL'AMMINISTRAZIONE":
                    if(!String.IsNullOrEmpty(value))
                    {
                        rda.DataNotificaRicorso = value;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void CheckValues(RichiestaDiAccesso rda)
        {
            // Controllo su differimento
            if(rda.Esito != null && rda.Esito.TipoEsito != null && rda.Esito.TipoEsito == EsitoType.Differimento)
            {
                rda.Esito = null;
            }
            if(rda.EsitoRiesame != null && rda.EsitoRiesame.TipoEsito != null && rda.EsitoRiesame.TipoEsito == EsitoType.Differimento)
            {
                rda.EsitoRiesame = null;
            }
        }

        private static void SetStatoRichiesta(RichiestaDiAccesso rda, String typology)
        {
            // Implementazione attuale:
            // La richiesta è CHIUSA se è definito un esito e non è definita la data notifica del blocco esito successivo
            // La richiesta è SOSPESA se sono presenti controinteressati ma non è stato ancora definito un esito
            // La richiesta è IN CORSO negli altri casi

            rda.StatoRichiesta = StatoType.InCorso;

            if(rda.Esito == null)
            {
                if(rda.PresenzaControinteressati)
                    rda.StatoRichiesta = StatoType.Sospesa;
            }
            else
            {
                if(rda.EsitoRiesame == null && typology == "Accesso Generalizzato e Civico")
                {
                    if(!String.IsNullOrEmpty(rda.DataNotificaRiesame))
                        rda.StatoRichiesta = StatoType.Chiusa;
                }
                else
                {
                    if(rda.EsitoRicorso == null)
                    {
                        if (!String.IsNullOrEmpty(rda.DataNotificaRicorso))
                            rda.StatoRichiesta = StatoType.Chiusa;
                    }
                    else
                    {
                        rda.StatoRichiesta = StatoType.Chiusa;
                    }
                }
            }
            
        }

        private static Byte[] RegistroAccessiCreateZipArchive(List<DocsPaVO.documento.FileDocumento> reportList)
        {
            Byte[] result;

            using (MemoryStream outputStream = new MemoryStream())
            {
                using (ZipOutputStream zipStream = new ZipOutputStream(outputStream))
                {
                    foreach(DocsPaVO.documento.FileDocumento report in reportList)
                    {
                        ZipEntry entry = new ZipEntry(report.fullName);
                        entry.DateTime = DateTime.Now;

                        zipStream.PutNextEntry(entry);

                        MemoryStream input = new MemoryStream(report.content);
                        StreamUtils.Copy(input, zipStream, new byte[4096]);
                        input.Close();

                        zipStream.CloseEntry();
                    }
                }

                result = outputStream.ToArray();
            }

            return result;
        }

        private static Byte[] GetSerializedXML(List<RichiestaDiAccesso> reg)
        {
            logger.Debug("BEGIN");

            XmlSerializer serializer = new XmlSerializer(typeof(List<RichiestaDiAccesso>), new XmlRootAttribute("RegistroAccessi"));

            MemoryStream ms = new MemoryStream();

            serializer.Serialize(ms, reg);

            Byte[] serializedXML = ms.ToArray();

            logger.Debug("END");

            return serializedXML;
        }

        private static void SetEsito(Esito esitoElement, String esitoValue)
        {
            switch(esitoValue.ToUpper())
            {
                case "ACCOGLIMENTO":
                    esitoElement.TipoEsito = EsitoType.Accoglimento;
                    break;
                case "DIFFERIMENTO":
                    // al momento non è gestito
                    // il campo verrà pulito in seguito
                    esitoElement.TipoEsito = EsitoType.Differimento;
                    break;
                case "ACCOGLIMENTO PARZIALE":
                case "RIFIUTO PARZIALE":
                    esitoElement.TipoEsito = EsitoType.AccoglimentoParziale;
                    break;
                case "RIFIUTO TOTALE":
                case "NON ACCOGLIMENTO":
                    esitoElement.TipoEsito = EsitoType.Rifiuto;
                    break;
                default:
                    break;
            }
        }

        private static void SetMotivoRifiuto(Esito esitoElement, string rifiuto)
        {
            esitoElement.MotiviRifiuto = new MotiviRifiuto();
            
            switch(rifiuto)
            {
                case "sicurezza pubblica e ordine pubblico":
                    esitoElement.MotiviRifiuto.SicurezzaPubblica = true;
                    break;
                case "sicurezza nazionale":
                    esitoElement.MotiviRifiuto.SicurezzaNazionale = true;
                    break;
                case "difesa e questioni militari":
                    esitoElement.MotiviRifiuto.Difesa = true;
                    break;
                case "relazioni internazionali":
                    esitoElement.MotiviRifiuto.RelazioniInternazionali = true;
                    break;
                case "politica e stabilità finanziaria ed economica dello Stato":
                    esitoElement.MotiviRifiuto.Politica = true;
                    break;
                case "conduzione di indagini sui reati e loro perseguimento":
                    esitoElement.MotiviRifiuto.ConduzioneIndaginiReati = true;
                    break;
                case "segreto di Stato":
                    esitoElement.MotiviRifiuto.SegretoDiStato = true;
                    break;
                case "a protezione di interessi economici e commerciali":
                    esitoElement.MotiviRifiuto.InteressiEconomiciCommerciali = true;
                    break;
                case "a protezione di dati personali":
                    esitoElement.MotiviRifiuto.ProtezioneDatiPersonali = true;
                    break;
                case "divieto di divulgazione per norma di legge":
                    esitoElement.MotiviRifiuto.LibertaSegretezzaCorrispondenza = true;
                    break;
                case "dati richiesti non sono detenuti dall'Amministrazione provinciale":
                    esitoElement.MotiviRifiuto.InformazioneNonEsistente = true;
                    break;
                case "dati richiesti richiedono un'elaborazione a cui l'Amministrazione non è tenuta":
                case "domanda manifestamente irragionevole cd. richiesta massiva":
                    esitoElement.MotiviRifiuto.RichiestaOnerosa = true;
                    break;
                case "tutela del regolare svolgimento di attività ispettive":
                    esitoElement.MotiviRifiuto.AttivitaIspettive = true;
                    break;
                case "altri motivi (descriverli nel campo successivo)":
                    esitoElement.MotiviRifiuto.AltriMotivi = true;
                    break;
            }
        }

        private static String FormatDate(String dateString)
        {
            try
            {
                String result = getDate(dateString);
                String d = result.Split('/')[0];
                String m = result.Split('/')[1];
                String y = result.Split('/')[2];

                result = y + "-" + m + "-" + d;
                return result;
            }
            catch(Exception ex)
            {
                logger.Error("Errore nella formattazione della data " + dateString, ex);
                return dateString;
            }
        }

        private static String getDate(String date)
        {
            String retVal = String.Empty;

            if (date.Length < 10)
            {
                retVal = date;
            }
            else
            {
                retVal = date.Substring(0, 10);
            }

            return retVal;
        }
    }
}
