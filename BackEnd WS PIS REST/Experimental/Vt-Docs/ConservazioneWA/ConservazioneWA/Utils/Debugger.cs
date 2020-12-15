using System;
using System.IO;
using System.Configuration;
using System.Xml;

namespace ConservazioneWA.Utils
{
    public class Debugger
    {
        static System.Text.UnicodeEncoding Utf16;

        static Debugger()
        {
            Utf16 = new System.Text.UnicodeEncoding();
        }

        /// <summary>Genera un messaggio sul file di log.</summary>
        /// <param name="message"></param>
        public static void Write(string message)
        {
            Write(message, null);
        }

        /// <summary>Genera un messaggio di errore sul file di log.</summary>
        /// <param name="sourceException"></param>
        public static void Write(Exception sourceException)
        {
            Write(null, sourceException);
        }

        private static void WriteString(FileStream f, string s)
        {
            //			if (!s.EndsWith ("\n"))
            //				s += "\n";

            byte[] bfr = Utf16.GetBytes(s);
            f.Write(bfr, 0, bfr.Length);
        }

        public static void Write(string message, Exception sourceException)
        {
            byte logLevel = Byte.Parse(ConfigurationManager.AppSettings["LOG_LEVEL"]);
            string time = System.DateTime.Now.ToString();
            if (logLevel > 0)
            {
                try
                {
                    // Genera debug sulla finestra di output

                    System.Diagnostics.Debug.WriteLine("[Message] " + time + " " + message);

                    if (sourceException != null)
                    {
                        System.Diagnostics.Debug.WriteLine("[Exception] " + sourceException.ToString());
                    }

                    if (logLevel == 2)
                    {
                        // Apri il file relativo al debug
                        XmlDocument xmlDocument = new XmlDocument();
                        string DebugLogPath = ConfigurationManager.AppSettings["DEBUG_PATH"];
                        DebugLogPath = DebugLogPath.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                        //DebugLogPath = Path.Combine (DebugLogPath, "debug_log");
                        DebugLogPath = Path.Combine(DebugLogPath, DateTime.Now.Hour.ToString("D2"));

                        if (!Directory.Exists(DebugLogPath))
                        {
                            Directory.CreateDirectory(DebugLogPath);
                        }

                        if (!File.Exists(Path.Combine(DebugLogPath, "logger.xsl")))
                        {
                            // Copia il file XSL nella directory							
                            string sourceFileXsl = AppDomain.CurrentDomain.BaseDirectory + @"\xml\logger.xsl";
                            if (File.Exists(sourceFileXsl))
                            {
                                File.Copy(sourceFileXsl, DebugLogPath + @"\logger.xsl");
                            }


                            //File.Copy(Path.Combine (AppDomain.CurrentDomain.BaseDirectory, Path.Combine ("xml", "logger.xsl")), Path.Combine (DebugLogPath, "logger.xsl"));
                            //							File.Copy(AppDomain.CurrentDomain.BaseDirectory + @"\xml\logger.xsl", DebugLogPath + @"\logger.xsl");
                        }

                        DebugLogPath = Path.Combine(DebugLogPath, "logger.xml");
                        Console.WriteLine(DebugLogPath);

                        //se il file non esiste, viene creato
                        if (!File.Exists(DebugLogPath))
                        {
                            // Crea un nuovo file XML
                            XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-16", "yes");
                            xmlDocument.InnerXml = xmlDeclaration.OuterXml + "<?xml-stylesheet type='text/xsl' href='logger.xsl'?><DEBUG>\r\n</DEBUG>";
                            xmlDocument.Save(DebugLogPath);
                        }

                        // Aggiungi messaggi al file XML
                        FileStream file = new FileStream(DebugLogPath, System.IO.FileMode.Open, System.IO.FileAccess.Write);
                        if (file != null)
                        {

                            file.Seek(-Utf16.GetByteCount("\r\n</DEBUG>"),
                                        System.IO.SeekOrigin.End);

                            string b = "<LOGENTRY date=\"" + DateTime.Now.ToString() + "\">";
                            b += "\r\n<MESSAGE>";
                            if (message != null)
                            {
                                b += "\r\n" + "<![CDATA[" + message + "]]>";
                            }
                            b += "\r\n</MESSAGE>\r\n";
                            b += "<EXCEPTION>";
                            if (sourceException != null)
                            {
                                b += "\r\n" + "<![CDATA[" + sourceException + "]]>";
                            }
                            b += "\r\n</EXCEPTION>\r\n";
                            b += "</LOGENTRY>\r\n</DEBUG>";
                            WriteString(file, b);
                            file.Close();
                        }

                    }
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine("Errore nella scrittura del file di debug (segue descrizione errore):");
                    System.Diagnostics.Debug.WriteLine(exception.ToString());
                }
            }
        }


        public static void WriteAdm(bool organigramma, string message, Exception sourceException)
        {
            try
            {
                // Apri il file relativo al debug
                XmlDocument xmlDocument = new XmlDocument();
                string DebugLogPath = ConfigurationManager.AppSettings["DEBUG_PATH"];
                DebugLogPath = DebugLogPath.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                DebugLogPath = Path.Combine(DebugLogPath, "debug_log");

                if (!Directory.Exists(DebugLogPath))
                {
                    Directory.CreateDirectory(DebugLogPath);
                }

                if (!File.Exists(Path.Combine(DebugLogPath, "logger.xsl")))
                {
                    // Copia il file XSL nella directory
                    File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "/xml/logger.xsl"), Path.Combine(DebugLogPath, "logger.xsl"));
                }

                if (organigramma)
                {
                    DebugLogPath = Path.Combine(DebugLogPath, "AmministrazioneLogger.xml");
                }
                else
                {
                    DebugLogPath = Path.Combine(DebugLogPath, "TitolarioLogger.xml");
                }

                //se il file non esiste, viene creato
                if (!File.Exists(DebugLogPath))
                {
                    // Crea un nuovo file XML
                    XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-16", "yes");
                    xmlDocument.InnerXml = xmlDeclaration.OuterXml + "<?xml-stylesheet type='text/xsl' href='logger.xsl'?><DEBUG>\r\n</DEBUG>";
                    xmlDocument.Save(DebugLogPath);
                }

                // Aggiungi messaggi al file XML
                FileStream file = new FileStream(DebugLogPath, System.IO.FileMode.Open, System.IO.FileAccess.Write);
                if (file != null)
                {
                    file.Seek(-Utf16.GetByteCount("\r\n</DEBUG>"), System.IO.SeekOrigin.End);

                    string b = "<LOGENTRY date=\"" + DateTime.Now.ToString() + "\">";
                    b += "\r\n<MESSAGE>";
                    if (message != null)
                    {
                        b += "\r\n" + "<![CDATA[" + message + "]]>";
                    }
                    b += "\r\n</MESSAGE>\r\n";
                    b += "<EXCEPTION>";
                    if (sourceException != null)
                    {
                        b += "\r\n" + "<![CDATA[" + sourceException + "]]>";
                    }
                    b += "\r\n</EXCEPTION>\r\n";
                    b += "</LOGENTRY>\r\n</DEBUG>";
                    WriteString(file, b);
                    file.Close();
                }

            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("Errore nella scrittura del file di debug (segue descrizione errore):");
                System.Diagnostics.Debug.WriteLine(exception.ToString());
            }
        }
    }
}
