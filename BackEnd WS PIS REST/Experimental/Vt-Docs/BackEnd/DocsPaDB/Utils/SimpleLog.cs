using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using log4net;

namespace DocsPaDB.Utils
{
    public class SimpleLog
    {
        private ILog logger = LogManager.GetLogger(typeof(SimpleLog));
        private StreamWriter sw = null;
        string logFullFileName = String.Empty;

        public SimpleLog() { }

        public SimpleLog(string fullFileName)
        {
            try
            {
                
                logFullFileName = fullFileName + ".log";
                logFullFileName = logFullFileName.Replace("%SERVERNAME", System.Environment.MachineName);
                if (!File.Exists(logFullFileName))
                {
                    /*Se il file non esiste dobbiamo crearlo*/
                    sw = File.CreateText(logFullFileName);
                    logger.Debug("BuildXMLReport : file di log " + logFullFileName + " creato correttamente.");
                }
                else
                {
                    /*Se esiste lo cancelliamo e ne ricreiamo uno nuovo*/
                    try
                    {
                        File.Delete(logFullFileName);
                    }
                    catch 
                    { 
                        sw = File.AppendText(logFullFileName); 
                    }
                    if (sw == null)
                    {
                        sw = File.CreateText(logFullFileName);
                    }
                    logger.Debug("BuildXMLReport : file di log " + logFullFileName + " creato correttamente.");                    
                }
            }
            catch (Exception ex)
            {
                sw = null;
                logger.Debug("BuildXMLReport : errore nella creazione del file di log " + logFullFileName + " MESSAGE : " + ex.Message + " STACKTRACE : " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                Close();
            }

        }

        public string Log(string exceptionType, string msg, string error, string function, string stackTrace)
        {
            if (sw != null)
            {
                try
                {
                    sw = (StreamWriter)File.AppendText(logFullFileName);
                    sw.WriteLine("");
                    sw.WriteLine("-----------------------------------------------------------------------------------------------");
                    sw.WriteLine(DateTime.UtcNow + " - Genera Proto ASL XML: ");
                    sw.WriteLine("");
                    sw.WriteLine("ECCEZIONE: " + exceptionType);
                    sw.WriteLine("");
                    sw.WriteLine("ERRORE: " + error);
                    sw.WriteLine("");
                    sw.WriteLine("FUNZIONE: " + function + ": " + msg);
                    sw.WriteLine(stackTrace);
                    sw.WriteLine("-----------------------------------------------------------------------------------------------");
                    sw.WriteLine("");
                    Close();
                }
                catch (Exception ex)
                {
                    logger.Debug("BuildXMLReport : errore nella scritura delle informazioni nel log MESSAGE : " + ex.Message + " STACKTRACE : " + ex.StackTrace);
                    throw ex;
                }
            }
            return logFullFileName;
        }

        public string Log(string exceptionType, string msg, string stackTrace)
        {
            if (sw != null)
            {
                try
                {
                    sw = (StreamWriter)File.AppendText(logFullFileName);
                    sw.WriteLine("");
                    sw.WriteLine("-----------------------------------------------------------------------------------------------");
                    sw.WriteLine(DateTime.UtcNow + " - Genera Proto ASL XML: ");
                    sw.WriteLine("");
                    sw.WriteLine("ECCEZIONE: " + exceptionType);
                    sw.WriteLine("");
                    sw.WriteLine(msg);
                    sw.WriteLine("");
                    sw.WriteLine(stackTrace);
                    sw.WriteLine("-----------------------------------------------------------------------------------------------");
                    sw.WriteLine("");

                    Close();
                }
                catch (Exception ex)
                {
                    logger.Debug("BuildXMLReport : errore nella scritura delle informazioni nel log MESSAGE : " + ex.Message + " STACKTRACE : " + ex.StackTrace);
                    throw ex;
                }
            }
            return logFullFileName;
        }

        public string Log(string msg)
        {
            if (sw != null)
            {
                try
                {
                    sw = (StreamWriter)File.AppendText(logFullFileName);
                    //sw.WriteLine("");
                    //sw.WriteLine("-----------------------------------------------------------------------------------------------");
                    //sw.WriteLine(DateTime.UtcNow + " - Genera Proto ASL XML: ");
                    //sw.WriteLine("");
                    //sw.WriteLine("MESSAGE: " + msg);
                    //sw.WriteLine("");
                    //sw.WriteLine("-----------------------------------------------------------------------------------------------");
                    //sw.WriteLine("");
                    sw.WriteLine(msg);
                    Close();
                }
                catch (Exception ex)
                {
                    logger.Debug("BuildXMLReport : errore nella scritura delle informazioni nel log MESSAGE : " + ex.Message + " STACKTRACE : " + ex.StackTrace);
                    throw ex;
                }
            }
            return logFullFileName;
        }

        public void Close()
        {
            if (sw != null)
            {
                try
                {
                    sw.Flush();
                    sw.Close();
                }
                catch (Exception ex)
                {
                    logger.Debug("BuildXMLReport : errore nella close del file di log MESSAGE : " + ex.Message + " STACKTRACE : " + ex.StackTrace);
                    throw ex;
                }
            }
        }

    }
}
