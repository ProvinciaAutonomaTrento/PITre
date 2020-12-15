using System;
using System.Configuration;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using log4net;

namespace BusinessLogic.report
{
    /// <summary>
    /// Summary description for ReportLog.
    /// </summary>
    public class ReportLog
    {
        private static ILog logger = LogManager.GetLogger(typeof(ReportLog));
        private string codAmm = "";
        private XmlDocument _xmlDoc = new XmlDocument();
        private string title;
        private string _logXSLUrl = HttpContext.Current.Server.MapPath(@"xml/xslfo_export_log.xsl");

        public ReportLog()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public DataSet GeneraRisultati(string user, string data_a, string type, string data_da, string oggetto, string azione, string esito, int tabelle)
        {
            DataSet dsStampaLog = new DataSet();
            // lettura tabella dei log
            DocsPaDB.Query_DocsPAWS.Log dblog = new DocsPaDB.Query_DocsPAWS.Log();
            //USERID_OPERATORE, DTA_AZIONE, VAR_OGGETTO, VAR_DESC_OGGETTO, VAR_DESC_AZIONE, CHA_ESITO
            dsStampaLog = dblog.GetXmlLogFiltrato(data_da, data_a, user, oggetto, azione, this.codAmm, esito, type, tabelle);

            return dsStampaLog;
        }

        public bool RiversaInStorico(string codAmm, string type)
        {
            this.codAmm = codAmm;
            DataSet dsLog = this.GeneraRisultati(null, null, type, null, null, null, null, 0);
            DocsPaDB.Query_DocsPAWS.Log dblog = new DocsPaDB.Query_DocsPAWS.Log();
            bool resInsert = true;
            bool result = true;

            if (dsLog.Tables["QUERY"].Rows.Count > 0)
            {
                // carica la data table 
                foreach (DataRow riga in dsLog.Tables["QUERY"].Rows)
                {
                    DocsPaDB.Query_DocsPAWS.Log dblogstorico = new DocsPaDB.Query_DocsPAWS.Log();
                    string UserID = riga[0].ToString();
                    string dataAzione = riga[1].ToString();
                    string var_oggetto = riga[2].ToString().Replace("'", "''");
                    string desc_oggetto = riga[3].ToString().Replace("'", "''");
                    string desc_azione = riga[4].ToString().Replace("'", "''");
                    string esito = riga[5].ToString().Replace("'", "''");
                    string idpeople = riga[6].ToString();
                    string idgruppo = riga[7].ToString();
                    string idAmm = riga[8].ToString();
                    string idoggetto = riga[9].ToString();
                    string cod_azione = riga[10].ToString().Replace("'", "''");

                    bool inserted = dblogstorico.InsertLogStorico(UserID, idpeople, idgruppo, idAmm, var_oggetto, idoggetto, desc_oggetto, cod_azione, desc_azione, esito, dataAzione);
                    if (!inserted)
                        resInsert = false;
                }
            }

            if (resInsert)
            {
                if (!dblog.DeleteLogFiltrato(codAmm, type))
                {
                    logger.Debug("Errore nella eliminazione dei record dei log!");
                    result = false;
                }
            }
            else
            {
                logger.Debug("Errore nell'inserimento dei record nella tabella di storico log!");
                result = false;
            }
            return result;
        }

        public DocsPaVO.documento.FileDocumento GeneraFile(string codAmm, string type, string exportType, string titolo, string user, string data_a, string data_da, string oggetto, string azione, string esito, int tabelle)
        {
            if (exportType.ToUpper().Equals("PDF"))
                return this.GeneraFilePDF(codAmm, type, titolo, user, data_a, data_da, oggetto, azione, esito, tabelle);
            else
                return this.GeneraFileXLS(codAmm, type, titolo, user, data_a, data_da, oggetto, azione, esito, tabelle);
        }

        #region Vecchio metodo di generazione del file PDF (COMMENTATO)

        //public DocsPaVO.documento.FileDocumento GeneraFilePDF(string codAmm, string type, string titolo, string user, string data_a, string data_da, string oggetto, string azione, string esito, int tabelle)
        //{
        //    this.codAmm = codAmm;
        //    bool result = true;
        //    DocsPaVO.documento.FileDocumento file = new DocsPaVO.documento.FileDocumento();
        //    FileStream fs = null;

        //    //DocsPaDB.Query_DocsPAWS.Amministrazione Amm = DocsPaDB.Query_DocsPAWS.Amministrazione();
        //    //string logPath = Amm.GetPathLogAmministrazione(codAmm);

        //    try
        //    {
        //        logger.Debug("START : ReportLog -> GeneraFilePDF");

        //        string rootPath			= AppDomain.CurrentDomain.BaseDirectory + "report/TemplateXML/";            
        //        //string rootPath = "C:\\PI3Report\\";
        //        string templateFilePath	= rootPath + "XMLRepLog.xml";
        //        string schemaFilePath	= rootPath + "XMLReport.xsd";
        //        string outputFileName = "";

        //        // Composizione del nome del file PDF di output es: CodiceAmm.ne_Log_AnnoMeseGiorno_OraMinutiSecondi
        //        //string outputFileName = codAmm + "_Logs_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".pdf";
        //        if (titolo.Equals("") || titolo == null)
        //            outputFileName = codAmm + "_Logs_del_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + "__Ore" + DateTime.Now.Hour.ToString() + "_Min" + DateTime.Now.Minute.ToString() + "_Sec" + DateTime.Now.Second.ToString();
        //        else
        //            outputFileName = titolo;

        //        outputFileName += ".pdf";

        //        DataSet dsStampaLog = new DataSet();

        //        dsStampaLog.Tables.Add("QUERY");

        //        DataColumn dc = new DataColumn("utente");
        //        dsStampaLog.Tables["QUERY"].Columns.Add(dc);

        //        dc = new DataColumn("data");
        //        dsStampaLog.Tables["QUERY"].Columns.Add(dc);

        //        dc = new DataColumn("oggetto");
        //        dsStampaLog.Tables["QUERY"].Columns.Add(dc);

        //        dc = new DataColumn("descrizione");
        //        dsStampaLog.Tables["QUERY"].Columns.Add(dc);

        //        dc = new DataColumn("operazione");
        //        dsStampaLog.Tables["QUERY"].Columns.Add(dc);

        //        dc = new DataColumn("esito");
        //        dsStampaLog.Tables["QUERY"].Columns.Add(dc);


        //        // Dataset contenente i dati della query
        //        // data table
        //        DataTable tableLog = new DataTable("LOG");
        //        DataRow dr;

        //        tableLog.Columns.Add(new DataColumn("utente", typeof(string)));
        //        tableLog.Columns.Add(new DataColumn("data", typeof(string)));
        //        tableLog.Columns.Add(new DataColumn("oggetto", typeof(string)));
        //        tableLog.Columns.Add(new DataColumn("descrizione", typeof(string)));
        //        tableLog.Columns.Add(new DataColumn("operazione", typeof(string)));
        //        tableLog.Columns.Add(new DataColumn("esito", typeof(string)));

        //        dsStampaLog = this.GeneraRisultati(user, data_a, type, data_da, oggetto, azione, esito, tabelle);

        //        if(dsStampaLog.Tables["QUERY"].Rows.Count>0)
        //        {		
        //            // carica la data table 
        //            foreach (DataRow riga in dsStampaLog.Tables["QUERY"].Rows)
        //            {
        //                dr = tableLog.NewRow();
        //                dr[0] = riga[0].ToString();		// campo utente
        //                dr[1] = riga[1].ToString();		// campo data
        //                dr[2] = riga[2].ToString();		// campo oggetto
        //                dr[3] = riga[3].ToString();		// campo descrizione
        //                dr[4] = riga[4].ToString();		// campo operazione
        //                if (riga[5].ToString() == "1")	// campo esito
        //                {
        //                    dr[5] = "OK";
        //                }
        //                else
        //                {
        //                    dr[5] = "KO";
        //                }
        //                tableLog.Rows.Add(dr);
        //            }

        //            //string temporaryPDFFilePath = string.Empty;
        //            //temporaryPDFFilePath = DocsPaUtils.Functions.Functions.GetArchivioLogPath() + @"\" + outputFileName;

        //            // Apertura stream su file template per la stampa
        //            fs = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read);

        //            StampaPDF.Report stampaLog = new StampaPDF.Report(fs, schemaFilePath);

        //            // Sostituzione parametri dinamici della stampa
        //            //Hashtable ht = new Hashtable();
        //            //ht["@param1"] = DateTime.Now.ToString();
        //            //stampaLog.replace(null, ht);

        //            // stampa file pdf
        //            stampaLog.printData(tableLog);
        //            stampaLog.appendTable("LOG", tableLog, false);

        //            // Chiusura stream file template
        //            fs.Close();

        //            MemoryStream memoryStream = stampaLog.getStream();

        //            stampaLog.close();
        //            // Gestione della path della direcory dei file PDF
        //            string pathDir = DocsPaUtils.Functions.Functions.GetArchivioLogPath();
        //            if (pathDir.Length > 0)
        //            {
        //                if (!Directory.Exists(pathDir))
        //                {
        //                    Directory.CreateDirectory(pathDir);
        //                }

        //                // stream
        //                Stream outputStream = new FileStream(
        //                pathDir + @"\" + outputFileName,
        //                FileMode.CreateNew,
        //                FileAccess.Write);

        //                Byte[] data = memoryStream.GetBuffer();
        //                outputStream.Write(data, 0, data.Length);

        //                memoryStream.Close();
        //                memoryStream = null;

        //                outputStream.Flush();
        //                outputStream.Close();
        //                outputStream = null;
        //            }
        //            else
        //            {
        //                logger.Debug("Directory dell'archivio dei log inesistente!");
        //                result = false;
        //            }
        //        }
        //        else
        //        {
        //            logger.Debug("Nessun record nella tabella dei log!");
        //            result = false;
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        logger.Debug("Errore nella stampa dei Report dei log! ",e);
        //        result = false;
        //    }
        //    finally
        //    {
        //        if (fs != null)
        //            fs.Close();
        //    }

        //    fs = null;

        //    logger.Debug("END : ReportLog -> GeneraFilePDF");

        //    return file;
        //}
        #endregion


        public DocsPaVO.documento.FileDocumento GeneraFilePDF(string codAmm, string type, string titolo, string user, string data_a, string data_da, string oggetto, string azione, string esito, int tabelle)
        {
            this.codAmm = codAmm;
            bool result = true;
            DocsPaVO.documento.FileDocumento file = new DocsPaVO.documento.FileDocumento();
            FileStream fs = null;

            //DocsPaDB.Query_DocsPAWS.Amministrazione Amm = DocsPaDB.Query_DocsPAWS.Amministrazione();
            //string logPath = Amm.GetPathLogAmministrazione(codAmm);

            try
            {
                logger.Debug("START : ReportLog -> GeneraFilePDF");

                //string rootPath = AppDomain.CurrentDomain.BaseDirectory + "report/TemplateXML/";
                //string rootPath = "C:\\PI3Report\\";
                //string templateFilePath = rootPath + "XMLRepLog.xml";
                //string schemaFilePath = rootPath + "XMLReport.xsd";
                string outputFileName = "";

                // Composizione del nome del file PDF di output es: CodiceAmm.ne_Log_AnnoMeseGiorno_OraMinutiSecondi
                //string outputFileName = codAmm + "_Logs_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".pdf";
                if (titolo.Equals("") || titolo == null)
                    outputFileName = codAmm + "_Logs_del_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + "__Ore" + DateTime.Now.Hour.ToString() + "_Min" + DateTime.Now.Minute.ToString() + "_Sec" + DateTime.Now.Second.ToString();
                else
                    outputFileName = titolo;

                outputFileName += ".pdf";

                this.title = outputFileName;

                DataSet dsStampaLog = new DataSet();

                dsStampaLog.Tables.Add("QUERY");

                DataColumn dc = new DataColumn("utente");
                dsStampaLog.Tables["QUERY"].Columns.Add(dc);

                dc = new DataColumn("data");
                dsStampaLog.Tables["QUERY"].Columns.Add(dc);

                dc = new DataColumn("oggetto");
                dsStampaLog.Tables["QUERY"].Columns.Add(dc);

                dc = new DataColumn("descrizione");
                dsStampaLog.Tables["QUERY"].Columns.Add(dc);

                dc = new DataColumn("operazione");
                dsStampaLog.Tables["QUERY"].Columns.Add(dc);

                dc = new DataColumn("esito");
                dsStampaLog.Tables["QUERY"].Columns.Add(dc);

                dsStampaLog = this.GeneraRisultati(user, data_a, type, data_da, oggetto, azione, esito, tabelle);

                if (dsStampaLog.Tables["QUERY"].Rows.Count > 0)
                {
                 
                    // ...il nome del file che contiene la definizione dello schema
                    // per del report
                    string templateFileName = "XMLRepStampaRisRicLog.xml";

                    // ...creazione del data table con i dati sulle righe di log
                    // da inserire nel report
                    DataTable dataTable = this.GetDataTableRisRicLog(dsStampaLog.Tables["QUERY"]);

                    // ...creazione dell'oggetto che si occuperò della creazione del report
                    StampaPDF.StampaRisRicerca report = new StampaPDF.StampaRisRicerca();

                    // ...creazione del report
                    file = report.GetFileDocumento(
                        templateFileName,
                        titolo,
                        codAmm,
                        dsStampaLog.Tables["QUERY"].Rows.Count.ToString(),
                        dataTable);

                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella stampa dei Report dei log! ", e);
                result = false;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }

            fs = null;

            logger.Debug("END : ReportLog -> GeneraFilePDF");

            return file;
        }

        private DataTable GetDataTableRisRicLog(DataTable dataTable)
        {
            // Creazione del dataset con i dati sulle righe di 
            // log da inserire nel report
            DataTable infoLogs = new DataTable();
            DataRow infoLog;

            // Creazione della struttura per infoLogs
            infoLogs.Columns.Add("UTENTE");
            infoLogs.Columns.Add("DATA");
            infoLogs.Columns.Add("OGGETTO");
            infoLogs.Columns.Add("DESCRIZIONE");
            infoLogs.Columns.Add("OPERAZIONE");
            infoLogs.Columns.Add("ESITO");

            foreach (DataRow riga in dataTable.Rows)
            {
                // Creazione di una nuova riga
                infoLog = infoLogs.NewRow();

                // Aggiunta delle informaizoni sull'utente
                infoLog["UTENTE"] = riga[0].ToString();

                // Aggiunta delle informazioni sulla data
                infoLog["DATA"] = riga[1].ToString();

                // Aggiunta delle informazioni sull'oggetto
                infoLog["OGGETTO"] = riga[2].ToString();

                // Aggiunta delle informazioni sulla descrizione
                infoLog["DESCRIZIONE"] = riga[3].ToString();

                // Aggiunta delle informazioni sull'operazione
                infoLog["OPERAZIONE"] = riga[4].ToString();

                // Aggiunta delle informazioni sull'esito
                string esito;
                if (riga[5].ToString().Equals("1"))
                    esito = "OK";
                else
                    esito = "KO";

                infoLog["ESITO"] = esito;

                // Aggiunta della riga compilata
                infoLogs.Rows.Add(infoLog);

            }

            // Restituisce le informazioni sui log
            return infoLogs;
        }

        private void exportToXML(DataSet dsStampaLog)
        {
            // carica il file xml dei record 
            foreach (DataRow riga in dsStampaLog.Tables["QUERY"].Rows)
            {
                XmlElement record = this._xmlDoc.CreateElement("RECORD");

                XmlElement ut = this._xmlDoc.CreateElement("UTENTE");
                ut.InnerText = riga[0].ToString();
                record.AppendChild(ut);

                XmlElement dta = this._xmlDoc.CreateElement("DATA");
                dta.InnerText = riga[1].ToString();
                record.AppendChild(dta);

                XmlElement ogg = this._xmlDoc.CreateElement("OGGETTO");
                ogg.InnerText = riga[2].ToString();
                record.AppendChild(ogg);

                XmlElement desc = this._xmlDoc.CreateElement("DESCRIZIONE");
                desc.InnerText = riga[3].ToString();
                record.AppendChild(desc);

                XmlElement op = this._xmlDoc.CreateElement("OPERAZIONE");
                op.InnerText = riga[4].ToString();
                record.AppendChild(op);

                XmlElement es = this._xmlDoc.CreateElement("ESITO");
                if (riga[5].ToString().Equals("1"))
                    es.InnerText = "OK";
                else
                    es.InnerText = "KO";
                record.AppendChild(es);

                this._xmlDoc.DocumentElement.AppendChild(record);
            }
        }

        private void addAttToRootNode(int numero)
        {
            XmlNode rootNode = this._xmlDoc.AppendChild(this._xmlDoc.CreateElement("EXPORT"));

            XmlAttribute attrRoot = this._xmlDoc.CreateAttribute("admin");
            attrRoot.InnerText = this.codAmm;
            rootNode.Attributes.Append(attrRoot);

            attrRoot = this._xmlDoc.CreateAttribute("title");
            attrRoot.InnerText = this.title;
            rootNode.Attributes.Append(attrRoot);

            attrRoot = this._xmlDoc.CreateAttribute("date");
            attrRoot.InnerText = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString();
            rootNode.Attributes.Append(attrRoot);

            attrRoot = this._xmlDoc.CreateAttribute("rows");
            attrRoot.InnerText = numero.ToString();
            rootNode.Attributes.Append(attrRoot);
        }

        public DocsPaVO.documento.FileDocumento GeneraFileXLS(string codAmm, string type, string titolo, string user, string data_a, string data_da, string oggetto, string azione, string esito, int tabelle)
        {
            this.codAmm = codAmm;
            bool result = true;
            DocsPaVO.documento.FileDocumento file = new DocsPaVO.documento.FileDocumento();
            FileStream fs = null;

            try
            {
                logger.Debug("START : ReportLog -> GeneraFileXLS");

                string outputFileName = "";

                // Composizione del nome del file PDF di output es: CodiceAmm.ne_Log_AnnoMeseGiorno_OraMinutiSecondi
                //string outputFileName = codAmm + "_Logs_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".pdf";
                if (titolo.Equals("") || titolo == null)
                    outputFileName = codAmm + "_Logs_del_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + "__Ore" + DateTime.Now.Hour.ToString() + "_Min" + DateTime.Now.Minute.ToString() + "_Sec" + DateTime.Now.Second.ToString();
                else
                    outputFileName = titolo;

                outputFileName += ".xls";

                DataSet dsStampaLog = new DataSet();

                dsStampaLog.Tables.Add("QUERY");

                DataColumn dc = new DataColumn("utente");
                dsStampaLog.Tables["QUERY"].Columns.Add(dc);

                dc = new DataColumn("data");
                dsStampaLog.Tables["QUERY"].Columns.Add(dc);

                dc = new DataColumn("oggetto");
                dsStampaLog.Tables["QUERY"].Columns.Add(dc);

                dc = new DataColumn("descrizione");
                dsStampaLog.Tables["QUERY"].Columns.Add(dc);

                dc = new DataColumn("operazione");
                dsStampaLog.Tables["QUERY"].Columns.Add(dc);

                dc = new DataColumn("esito");
                dsStampaLog.Tables["QUERY"].Columns.Add(dc);

                dsStampaLog = this.GeneraRisultati(user, data_a, type, data_da, oggetto, azione, esito, tabelle);

                // Dataset contenente i dati della query
                // data table
                DataTable tableLog = new DataTable("LOG");
                DataRow dr;

                tableLog.Columns.Add(new DataColumn("utente", typeof(string)));
                tableLog.Columns.Add(new DataColumn("data", typeof(string)));
                tableLog.Columns.Add(new DataColumn("oggetto", typeof(string)));
                tableLog.Columns.Add(new DataColumn("descrizione", typeof(string)));
                tableLog.Columns.Add(new DataColumn("operazione", typeof(string)));
                tableLog.Columns.Add(new DataColumn("esito", typeof(string)));

                if (dsStampaLog.Tables["QUERY"].Rows.Count > 0)
                {
                    string temporaryXLSFilePath = string.Empty;
                    StreamWriter writer = null;
                    StringBuilder sb = new StringBuilder();

                    //Creazione stringa XML
                    sb = creaXML(dsStampaLog, codAmm);

                    string serverPath = System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"];
                    serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd") + @"\Export\Log\");
                    if (!Directory.Exists(serverPath))
                    {
                        Directory.CreateDirectory(serverPath);
                    }
                    //temporaryXLSFilePath = serverPath.Replace("%DATA", outputFileName);
                    temporaryXLSFilePath = Path.Combine(serverPath, outputFileName);
                    logger.Debug("Cancello se esistente il file " + temporaryXLSFilePath);
                    if (File.Exists(temporaryXLSFilePath))
                    {
                        File.Delete(temporaryXLSFilePath);
                    }
                    //temporaryXLSFilePath += @"\" + outputFileName;
                    // string pathDir = DocsPaUtils.Functions.Functions.GetArchivioLogPath();
                    string pathDir = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                    logger.Debug("Path dir = " + pathDir);
                    if (pathDir.Length > 0)
                    {
                        //Salva e chiudi il file
                        //temporaryXLSFilePath = HttpContext.Current.Server.MapPath("ExportLogAmm.xls");

                        writer = new StreamWriter(temporaryXLSFilePath, true);
                        writer.AutoFlush = true;
                        writer.WriteLine(sb.ToString());
                        writer.Flush();
                        writer.Close();

                        //Crea il file
                        FileStream stream = new FileStream(temporaryXLSFilePath, FileMode.Open, FileAccess.Read);
                        if (stream != null)
                        {
                            byte[] contentExcel = new byte[stream.Length];
                            stream.Read(contentExcel, 0, contentExcel.Length);
                            stream.Flush();
                            stream.Close();
                            stream = null;
                            file.content = contentExcel;
                            file.length = contentExcel.Length;
                            file.estensioneFile = "xls";
                            file.name = "ExportLogAmm";
                            file.contentType = "application/vnd.ms-excel";
                        }

                        // File.Delete(temporaryXLSFilePath);
                    }
                    else
                    {
                        logger.Debug("Directory dell'archivio dei log inesistente!");
                        result = false;
                    }
                }
                else
                {
                    logger.Debug("Nessun record nella tabella dei log!");
                    result = false;
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella stampa dei Report dei log! ", e);
                result = false;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }

            fs = null;

            logger.Debug("END : ReportLog -> GeneraFileXLS");

            return file;
        }


        private StringBuilder creaXML(DataSet dsStampaLog, string codAmm)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            //Fogli Excel
            strXML += sheetLogAmm(dsStampaLog, codAmm);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());
            return sb;
        }

        private string topXML()
        {
            string strXML = string.Empty;

            strXML = "<?xml version=\"1.0\" encoding = \"UTF-16\" ?>";
            strXML += "<?mso-application progid=\"Excel.Sheet\"?>";
            strXML += "<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" xmlns:html=\"http://www.w3.org/TR/REC-html40\">";
            strXML += "<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">";
            strXML += "<Author></Author>";
            strXML += "<LastAuthor></LastAuthor>";
            strXML += "<Created></Created>";
            strXML += "<Company>ETNOTEAM S.p.A.</Company>";
            strXML += "<Version></Version>";
            strXML += "</DocumentProperties>";
            strXML += "<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">";
            strXML += "<WindowHeight>1</WindowHeight>";
            strXML += "<WindowWidth>1</WindowWidth>";
            strXML += "<WindowTopX>1</WindowTopX>";
            strXML += "<WindowTopY>1</WindowTopY>";
            strXML += "<ProtectStructure>False</ProtectStructure>";
            strXML += "<ProtectWindows>False</ProtectWindows>";
            strXML += "</ExcelWorkbook>";
            return strXML;
        }

        private string stiliXML()
        {
            string strXML = string.Empty;

            strXML = "<Styles>";

            strXML += "<Style ss:ID=\"Default\" ss:Name=\"Normal\">";
            strXML += "<Alignment/>";
            strXML += "<Borders/>";
            strXML += "<Font/>";
            strXML += "<Interior/>";
            strXML += "<NumberFormat/>";
            strXML += "<Protection/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s63\">";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"8\"/>";
            strXML += "<NumberFormat ss:Format=\"@\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s68\">";
            strXML += "<Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Bottom\"/>";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"8\"/>";
            strXML += "<NumberFormat ss:Format=\"@\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s66\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"8\" ss:Color=\"#FFFFFF\" ss:Bold=\"1\"/>";
            strXML += "<Interior ss:Color=\"#FF0000\" ss:Pattern=\"Solid\"/>";
            strXML += "<NumberFormat ss:Format=\"@\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s67\">";
            strXML += "<Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Bottom\"/>";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"8\" ss:Color=\"#FFFFFF\" ss:Bold=\"1\"/>";
            strXML += "<Interior ss:Color=\"#993300\" ss:Pattern=\"Solid\"/>";
            strXML += "<NumberFormat ss:Format=\"@\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s62\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"8\" ss:Color=\"#FFFFFF\" ss:Bold=\"1\"/>";
            strXML += "<Interior ss:Color=\"#993300\" ss:Pattern=\"Solid\"/>";
            strXML += "<NumberFormat ss:Format=\"@\"/>";
            strXML += "</Style>";

            strXML += "</Styles>";

            return strXML;
        }

        private string sheetLogAmm(DataSet dsStampaLog, string codAmm)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"LOG_AMM\">";
            strXML += "<Table>";
            strXML += creaTabellaLogAmm();
            strXML += datiLogAmmXML(dsStampaLog, codAmm);
            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";
            return strXML;
        }

        private string creaTabellaLogAmm()
        {
            string strXML = string.Empty;

            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"10\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"30\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"40\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"30\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:AutoFitWidth=\"0\" ss:Width=\"300\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"200\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"10\"/>";
            return strXML;
        }

        private string datiLogAmmXML(DataSet dsStampaLog, string codAmm)
        {
            string strXML = string.Empty;
            strXML = creaColonneLogAmm();
            strXML += inserisciDatiLogAmm(dsStampaLog, codAmm);
            return strXML;
        }

        private string creaColonneLogAmm()
        {
            string strXML = string.Empty;
            strXML += "<Row>";

            //Colonna Amministrazione
            strXML += "<Cell ss:StyleID=\"s63\">";
            strXML += "<Data ss:Type=\"String\">AMM";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Registro
            strXML += "<Cell ss:StyleID=\"s63\">";
            strXML += "<Data ss:Type=\"String\">UTENTE";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Livello 1
            strXML += "<Cell ss:StyleID=\"s63\">";
            strXML += "<Data ss:Type=\"String\">DATA";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Livello 2
            strXML += "<Cell ss:StyleID=\"s63\">";
            strXML += "<Data ss:Type=\"String\">OGGETTO";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Livello 3
            strXML += "<Cell ss:StyleID=\"s63\">";
            strXML += "<Data ss:Type=\"String\">OPERAZIONE";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Livello 4
            strXML += "<Cell ss:StyleID=\"s63\">";
            strXML += "<Data ss:Type=\"String\">DESCRIZIONE";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Livello 5
            strXML += "<Cell ss:StyleID=\"s63\">";
            strXML += "<Data ss:Type=\"String\">ESITO";
            strXML += "</Data>";
            strXML += "</Cell>";

            strXML += "</Row>";
            return strXML;

        }

        private string inserisciDatiLogAmm(DataSet ds, string codAmm)
        {
            string righe = string.Empty;
            if ((ds.Tables[0] != null) && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                    righe += inserisciRigaLogAmm(row, codAmm);
            }
            return righe;
        }

        private static string inserisciRigaLogAmm(DataRow row, string codAmm)
        {
            string riga = string.Empty;
            riga = "<Row>";

            //DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            //string codiceAmm = string.Empty;
            //string codiceReg = string.Empty;
            //string[] codiceClassifica = null;

            //if (row["ID_AMM"].ToString() != null && row["ID_AMM"].ToString() != "")
            //    codiceAmm = amm.GetVarCodiceAmm(row["ID_AMM"].ToString());

            //Colonna Amministrazione
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\">" + codAmm;
            riga += "</Data>";
            riga += "</Cell>";

            //Colonna Utente
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\">" + row["USERID_OPERATORE"].ToString();
            riga += "</Data>";
            riga += "</Cell>";

            //Colonna Data
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\">" + row["DTA_AZIONE"].ToString();
            riga += "</Data>";
            riga += "</Cell>";

            //Colonna Oggetto
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\">" + row["VAR_OGGETTO"].ToString();
            riga += "</Data>";
            riga += "</Cell>";

            //Colonna Operazione
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\">" + row["VAR_DESC_AZIONE"].ToString();
            riga += "</Data>";
            riga += "</Cell>";

            //Colonna Informazioni
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\">" + row["VAR_DESC_OGGETTO"].ToString();
            riga += "</Data>";
            riga += "</Cell>";

            //Colonna Esito
            if (row["CHA_ESITO"].ToString() == "1")
            {
                riga += "<Cell>";
                riga += "<Data ss:Type=\"String\">OK";
                riga += "</Data>";
                riga += "</Cell>";
            }
            else
            {
                riga += "<Cell>";
                riga += "<Data ss:Type=\"String\">KO";
                riga += "</Data>";
                riga += "</Cell>";
            }

            riga += "</Row>";
            return riga;
        }

        private string workSheetOptionsXML()
        {
            string strXML = string.Empty;

            strXML = "<WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\">";
            strXML += "<Selected/>";
            strXML += "<ProtectObjects>False</ProtectObjects>";
            strXML += "<ProtectScenarios>False</ProtectScenarios>";
            strXML += "<PageSetup>";
            strXML += "<Layout x:Orientation=\"Landscape\"/>";
            strXML += "</PageSetup>";
            strXML += "<Print>";
            strXML += "<ValidPrinterInfo/>";
            strXML += "<HorizontalResolution>600</HorizontalResolution>";
            strXML += "<VerticalResolution>600</VerticalResolution>";
            strXML += "</Print>";
            strXML += "</WorksheetOptions>";

            return strXML;
        }


    }
}
