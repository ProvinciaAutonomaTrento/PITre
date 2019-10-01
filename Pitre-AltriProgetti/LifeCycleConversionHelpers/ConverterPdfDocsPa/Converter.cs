using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Threading;

namespace ConverterPdfDocsPa
{
    class Converter
    {
        //Timer Clock;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            Thread t = new Thread(new ThreadStart(startControl));
            t.Name = "Converter PDF DocsPa";
            t.Start();
            int intervalTimer = Convert.ToInt32(Properties.Settings.Default.intervalTimer);
            while(true)
            {
                Thread.Sleep(intervalTimer);
                startControl();
            }
        }

        public static void startControl()
        {
            try
            {
                controlInputPdf();

                controlOutputPdf(Properties.Settings.Default.AdobeOutputFolder);
                controlFailurePdf(Properties.Settings.Default.AdobeFailureFolder);

                controlOutputPdf(Properties.Settings.Default.AdobeOutputFolderHtml);
                controlFailurePdf(Properties.Settings.Default.AdobeFailureFolderHtml);
            }
            catch (Exception ex)
            {
                Log logFileApplication = new Log("logApplication");
                logFileApplication.WriteLog(System.DateTime.Now.ToString("gg/mm/yyyy") + " - ERRORE start application : " + ex.Message);
            }
        }

        public static void controlInputPdf()
        {
            try
            {
                //Recupero il path della coda di conversione
                string pathQueueFolder = Properties.Settings.Default.AdobeQueueFolder;

                //Controllo i files nella coda
                foreach (string genericFile in Directory.GetFiles(pathQueueFolder))
                {
                    //Controllo se è un file di metadati
                    if (Path.GetExtension(genericFile) == "")
                    {
                        string fileNameDaConvertire = Path.GetFileName(genericFile);

                        //Cerco il file da convertire associato
                        foreach (string convertFile in Directory.GetFiles(pathQueueFolder))
                        {
                            if (Path.GetExtension(convertFile) != "" && fileNameDaConvertire == Path.GetFileNameWithoutExtension(convertFile))
                            {
                                try
                                {
                                    //Se il file è un html lo sposto nel WF di conversione specifico per html,
                                    //altrimenti lo sposto nel WF di conversione generico
                                    if (Path.GetExtension(convertFile).ToUpper() == ".html".ToUpper() || Path.GetExtension(convertFile).ToUpper() == ".htm".ToUpper())
                                    {
                                        File.Move(convertFile, Path.Combine(Properties.Settings.Default.AdobeInputFolderHtml, Path.GetFileName(convertFile)));
                                    }
                                    else
                                    {
                                        File.Move(convertFile, Path.Combine(Properties.Settings.Default.AdobeInputFolder, Path.GetFileName(convertFile)));
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log logFileInput = new Log("logInputConverter");
                                    logFileInput.WriteLog(System.DateTime.Now.ToString("gg/mm/yyyy") + " - ERRORE non è stato possibile spostare in input il file : " + convertFile + " - " + e.Message);    
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log logFileInput = new Log("logInputConverter");
                logFileInput.WriteLog(System.DateTime.Now.ToString("gg/mm/yyyy") + " - ERRORE controllo coda di conversione : " + ex.Message);    
            }
        }

        static string removeAllExtensions(string file)
        {
            string FileName = Path.GetFileName(file);
            while (FileName.Contains("."))
            {
                FileName = Path.GetFileNameWithoutExtension(FileName);
            };

            return FileName;
        }
        static void deleteAllFiles(string path)
        {
            string pathdir = Path.GetDirectoryName (path);
            string filename = removeAllExtensions(path);
             foreach (string genericFile in Directory.GetFiles(pathdir,filename+ "*"))
             {
                 File.Delete (genericFile);
             }
        }



        public static void controlOutputPdf(string pathOutputWF)
        {
            try
            {
                //Recupero il path della cartella di output di conversione e dichiaro le varibili di utilità
                //string pathOutputFolder = Properties.Settings.Default.AdobeOutputFolder;
                string pathOutputFolder = pathOutputWF;
                DocsPa.DocsPaWebService ws = new DocsPa.DocsPaWebService();
                string urlBackend = string.Empty;
                string nameFileXml = string.Empty;
                string nameFileConvertito = string.Empty;
                byte[] xmlByte;
                byte[] docConvertitoByte;
                
                //Controllo i files convertiti nella cartella di output
                foreach (string genericFile in Directory.GetFiles(pathOutputFolder, "*", SearchOption.AllDirectories))
                {
                    //nameFileXml = Path.GetFileNameWithoutExtension(genericFile);
                    nameFileXml = removeAllExtensions(genericFile);

                    //Controllo che il file convertito sia effettivamente un PDF che esiste il suo xml a corredo
                    if (Path.GetExtension(genericFile).ToUpper() == ".pdf".ToUpper() && File.Exists(Properties.Settings.Default.AdobeQueueFolder + "\\" + nameFileXml))
                    {
                        //Recupero e leggo le informazioni utili dal file xml a corredo di quello convertito
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(Properties.Settings.Default.AdobeQueueFolder + "\\" + nameFileXml);
                        if (xmlDoc != null)
                        {
                            XmlNode node = xmlDoc.DocumentElement;
                            nameFileConvertito = node.SelectSingleNode("FILE_NAME").InnerText.ToString();
                            urlBackend = node.SelectSingleNode("URL_WS").InnerText.ToString();

                        }
            
                        //Imposto l'url del backend da chiamare
                        if (urlBackend != "")
                        {
                            ws.Url = urlBackend;
                        }

                        //Recupero il file xml
                        StringWriter sw = new StringWriter();
                        XmlTextWriter xw = new XmlTextWriter(sw);
                        xmlDoc.WriteTo(xw);
                        xmlByte = System.Text.Encoding.Default.GetBytes(sw.ToString());
                        
                        //Recupero il file converito e il suo nome
                        nameFileConvertito = Path.GetFileName(genericFile);
                        docConvertitoByte = File.ReadAllBytes(genericFile);
                        
                        //Chiamo webMethod per inviare il file convertito e il suo xml a corredo
                        if (!string.IsNullOrEmpty(nameFileConvertito) &&
                            !string.IsNullOrEmpty(nameFileXml) &&
                            docConvertitoByte != null && docConvertitoByte.Length != 0 &&
                            xmlByte != null && xmlByte.Length != 0)
                        {
                            try
                            {
                                ws.Timeout = System.Threading.Timeout.Infinite;
                                ws.DequeueServerPdfConversion(nameFileConvertito, nameFileXml, docConvertitoByte, xmlByte);
                            }
                            catch (Exception e)
                            {
                                Log logFileOutput = new Log("logOutputConverter");
                                logFileOutput.WriteLog(System.DateTime.Now.ToString("gg/mm/yyyy") + " - ERRORE chiamata WS DocsPa : " + e.Message);    
                            }
                        }
                        
                        //Elimino i files
                        //File.Delete(genericFile);
                        deleteAllFiles(genericFile);
                        File.Delete(Properties.Settings.Default.AdobeQueueFolder + "\\" + nameFileXml);
                    }                    
                }
            }
            catch (Exception ex)
            {
                Log logFileOutput = new Log("logOutputConverter");
                logFileOutput.WriteLog(System.DateTime.Now.ToString("gg/mm/yyyy") + " - ERRORE controllo output di conversione : " + ex.Message);    
            }
        }

        public static void controlFailurePdf(string pathFailureWF)
        {
            try
            {
                //Recupero il path della cartella di failure di conversione e dichiaro le varibili di utilità
                //string pathFailureFolder = Properties.Settings.Default.AdobeFailureFolder;
                string pathFailureFolder = pathFailureWF;
                DocsPa.DocsPaWebService ws = new DocsPa.DocsPaWebService();
                string urlBackend = string.Empty;
                string nameFileXml = string.Empty;
                string nameFileConvertito = string.Empty;
                byte[] xmlByte;
                
                //Controllo i files per cui è fallita la conversione nella cartella di failure
                foreach (string genericFile in Directory.GetFiles(pathFailureFolder, "*", SearchOption.AllDirectories))
                {
                    nameFileXml = Path.GetFileNameWithoutExtension(genericFile);

                    //Controllo che esiste l'xml a corredo del file per cui è fallita la conversione
                    if (File.Exists(Properties.Settings.Default.AdobeQueueFolder + "\\" + nameFileXml))
                    {
                        //Recupero e leggo le informazioni utili dal file xml a corredo di quello per cui è fallita la conversione
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(Properties.Settings.Default.AdobeQueueFolder + "\\" + nameFileXml);
                        if (xmlDoc != null)
                        {
                            XmlNode node = xmlDoc.DocumentElement;
                            nameFileConvertito = node.SelectSingleNode("FILE_NAME").InnerText.ToString();
                            urlBackend = node.SelectSingleNode("URL_WS").InnerText.ToString();

                        }
                        
                        //Imposto l'url del backend da chiamare
                        if (urlBackend != "")
                        {
                            ws.Url = urlBackend;
                        }

                        //Recupero il file xml
                        StringWriter sw = new StringWriter();
                        XmlTextWriter xw = new XmlTextWriter(sw);
                        xmlDoc.WriteTo(xw);
                        xmlByte = System.Text.Encoding.Default.GetBytes(sw.ToString());
                        
                        //Recupero il nome del file per cui è fallita la conversione
                        nameFileConvertito = Path.GetFileName(genericFile);
                        //docConvertitoByte = File.ReadAllBytes(genericFile);
                        
                        //Chiamo webMethod per inviare il file convertito e il suo xml a corredo
                        if (!string.IsNullOrEmpty(nameFileConvertito) &&
                            !string.IsNullOrEmpty(nameFileXml) &&
                            xmlByte != null && xmlByte.Length != 0)
                        {
                            try
                            {
                                ws.Timeout = System.Threading.Timeout.Infinite;
                                ws.DequeueServerPdfConversion(nameFileConvertito, nameFileXml, null, xmlByte);
                            }
                            catch (Exception e)
                            {
                                Log logFileOutput = new Log("logOutputConverter");
                                logFileOutput.WriteLog(System.DateTime.Now.ToString("gg/mm/yyyy") + " - ERRORE chiamata WS DocsPa : " + e.Message);
                            }
                        }
                        
                        //Elimino i files
                        File.Delete(genericFile);
                        File.Delete(Properties.Settings.Default.AdobeQueueFolder + "\\" + nameFileXml);
                    }
                }
            }
            catch (Exception ex)
            {
                Log logFileOutput = new Log("logOutputConverter");
                logFileOutput.WriteLog(System.DateTime.Now.ToString("gg/mm/yyyy") + " - ERRORE controllo output di conversione : " + ex.Message);
            }
        }        
    }
}
