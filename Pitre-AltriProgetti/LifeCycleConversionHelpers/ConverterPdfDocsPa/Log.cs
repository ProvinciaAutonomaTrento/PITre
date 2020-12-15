using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace ConverterPdfDocsPa
{
    public class Log
    {
        private StreamWriter sw = null;
        string logFullFileName = String.Empty;

        public Log() { }

        public Log(string fullFileName)
        {
            try
            {
                //Se la directory di log non esiste la creiamo
                if (!Directory.Exists(Properties.Settings.Default.LogFilePath))
                {
                    Directory.CreateDirectory(Properties.Settings.Default.LogFilePath);
                }

                logFullFileName = Properties.Settings.Default.LogFilePath + "\\" + fullFileName + ".log";
                if (!File.Exists(logFullFileName))
                {
                    /*Se il file non esiste dobbiamo crearlo*/
                    sw = File.CreateText(logFullFileName);
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
                }
            }
            catch (Exception ex)
            {
                sw = null;
                throw ex;
            }
            finally
            {
                Close();
            }

        }

        public string WriteLog(string msg)
        {
            if (sw != null)
            {
                try
                {
                    sw = (StreamWriter)File.AppendText(logFullFileName);
                    sw.WriteLine(msg);
                    Close();
                }
                catch (Exception ex)
                {
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
                    throw ex;
                }
            }
        }

    }
}
