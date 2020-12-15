using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using ConnectUNCWithCredentials;

namespace LCDocsPaService
{
    class Logica
    {

        public const string errore = "ERROR";
        public const string info = "INFO";
        public const string debug = "DEBUG";

        private DocsPa.DocsPaWebService ws = null;
  
        public Logica()
        {
            ws = new DocsPa.DocsPaWebService();
            ws.Timeout = System.Threading.Timeout.Infinite;
            fileLog(debug, string.Format("Indirizzo webservice {0}", ws.Url), 4);
        }


        public void fileLog(string tipoLog, string messaggio, int loglevel)
        {
            if (Configurazione.logLevel > loglevel)
            {
                if (!string.IsNullOrEmpty(Configurazione.pathLog))
                    File.AppendAllText(@Configurazione.pathLog, "[" + tipoLog + "]:" + "[" + System.DateTime.Now.ToString() + "]" + messaggio + System.Environment.NewLine);
            }
        }


        public bool StartLifeCycleCheck
        {
            
            get
            {
                //logMainProcess.WriteLog("chiamaWebService:Start ");
                bool result = false;
                try
                {
                    if (!String.IsNullOrEmpty(@Configurazione.RemoteFolderRoot))
                    {
                        using (UNCAccessWithCredentials unc = new UNCAccessWithCredentials())
                        {
                            if (unc.NetUseWithCredentials(@Configurazione.RemoteFolderRoot, @Configurazione.RemoteFolderUser, @Configurazione.RemoteFolderDomain, @Configurazione.RemoteFolderPass))
                            {
                                result = checkCycle();
                            }
                            else
                            {
                                fileLog(errore, String.Format (" - ERRORE non reisco a collegarmi con le credenziali di rete :{0}  {1} {2} {3} {4}",unc.LastError , @Configurazione.RemoteFolderRoot, @Configurazione.RemoteFolderUser, @Configurazione.RemoteFolderDomain, @Configurazione.RemoteFolderPass), 0);
                            }
                        }
                    }
                    else //normalFS
                    {
                        result = checkCycle();
                    }

                }
                catch (Exception ex)
                {
                   fileLog(errore,  " - ERRORE start application : " + ex.Message,0);
                }
                return result;
            }
        }

        private bool checkCycle()
        {
            bool result;
            controlInputPdf();

            controlOutputPdf(@Configurazione.AdobeOutputFolder);
            controlFailurePdf(@Configurazione.AdobeFailureFolder);

            controlOutputPdf(@Configurazione.AdobeOutputFolderHtml);
            controlFailurePdf(@Configurazione.AdobeFailureFolderHtml);
            result = true;
            return result;
        }


        public void controlInputPdf()
        {
            try
            {
                //Recupero il path della coda di conversione
                string pathQueueFolder = @Configurazione.AdobeQueueFolder;

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
                                        if (!string.IsNullOrEmpty(@Configurazione.AdobeInputFolderHtml)) //solo se la coda è configurata
                                        {
                                            fileLog(info, " - Muovo il file nella cartella di input ", 5);
                                            File.Move(convertFile, Path.Combine(@Configurazione.AdobeInputFolderHtml, Path.GetFileName(convertFile)));
                                        }
                                        else
                                        {
                                            fileLog(info, " - Muovo il file nella cartella di input standard, visto che la cartella HTML non è configurata: ", 5);
                                            File.Move(convertFile, Path.Combine(@Configurazione.AdobeInputFolder, Path.GetFileName(convertFile)));
                                        }
                                    }
                                    else
                                    {
                                        fileLog(info, " - Muovo il file nella cartella di input ", 5);
                                        File.Move(convertFile, Path.Combine(@Configurazione.AdobeInputFolder, Path.GetFileName(convertFile)));
                                    }
                                }
                                catch (Exception e)
                                {
                                   
                                    fileLog(errore, " - non è stato possibile spostare in input il file : " + convertFile + " - " + e.Message,0);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                fileLog(errore, "- controllo coda di conversione : " + ex.Message,0);
            }
        }



        public  void controlOutputPdf(string pathOutputWF)
        {
            //è vuota, non processare
            if (String.IsNullOrEmpty(pathOutputWF))
                return;
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
                    if (Path.GetExtension(genericFile).ToUpper() == ".pdf".ToUpper() && File.Exists(@Configurazione.AdobeQueueFolder + "\\" + nameFileXml))
                    {
                        //Recupero e leggo le informazioni utili dal file xml a corredo di quello convertito
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(@Configurazione.AdobeQueueFolder + "\\" + nameFileXml);
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
                                fileLog(info, " - Chiamo il WS DequeueServerPdfConversion", 5);
                                ws.Timeout = System.Threading.Timeout.Infinite;
                                ws.DequeueServerPdfConversion(nameFileConvertito, nameFileXml, docConvertitoByte, xmlByte);
                            }
                            catch (Exception e)
                            {
                                fileLog(errore, " - chiamata WS DocsPa : " + e.Message,0);
                            }
                        }

                        //Elimino i files
                        //File.Delete(genericFile);
                        deleteAllFiles(genericFile);
                        File.Delete(@Configurazione.AdobeQueueFolder + "\\" + nameFileXml);
                    }
                }
            }
            catch (Exception ex)
            {
                fileLog(errore, " - ERRORE controllo output di conversione : " + ex.Message,0);
            }
        }

        public void controlFailurePdf(string pathFailureWF)
        {
            //è vuota, non processare
            if (String.IsNullOrEmpty(pathFailureWF))
                return;

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
                    if (File.Exists(@Configurazione.AdobeQueueFolder + "\\" + nameFileXml))
                    {
                        //Recupero e leggo le informazioni utili dal file xml a corredo di quello per cui è fallita la conversione
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(@Configurazione.AdobeQueueFolder + "\\" + nameFileXml);
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
                                fileLog(info, " - Chiamo il WS DequeueServerPdfConversion", 5);
                                ws.Timeout = System.Threading.Timeout.Infinite;
                                ws.DequeueServerPdfConversion(nameFileConvertito, nameFileXml, null, xmlByte);
                            }
                            catch (Exception e)
                            {

                                fileLog(errore,  " - chiamata WS DocsPa : " + e.Message,0);
                            }
                        }

                        //Elimino i files
                        File.Delete(genericFile);
                        File.Delete(@Configurazione.AdobeQueueFolder + "\\" + nameFileXml);
                    }
                }
            }
            catch (Exception ex)
            {
                fileLog (errore, " - controllo output di conversione : " + ex.Message,0);
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
            string pathdir = Path.GetDirectoryName(path);
            string filename = removeAllExtensions(path);
            foreach (string genericFile in Directory.GetFiles(pathdir, filename + "*"))
            {
                File.Delete(genericFile);
            }
        }

    }
}
