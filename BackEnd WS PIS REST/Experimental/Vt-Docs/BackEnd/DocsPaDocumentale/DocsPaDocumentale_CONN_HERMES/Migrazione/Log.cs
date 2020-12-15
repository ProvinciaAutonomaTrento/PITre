using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;

namespace DocsPaDocumentale_HERMES.Migrazione
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class AppDataFolder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        public static string GetFolderAmministrazione(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            const string APP_FOLDER = "MigrazionePITRE";

            string folder =
                    string.Format(@"{0}\{1}\{2}\", 
                            System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                            APP_FOLDER, 
                            amministrazione.Codice);
            
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return folder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        public static string GetLogFilePath(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            string fileName = string.Format("logmigrazione_{0}.txt", DateTime.Now.ToString("yyyyMMddhhmmss"));;

            return Path.Combine(GetFolderAmministrazione(amministrazione), fileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        public static string GetStatoMigrazioneFilePath(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            const string FILE = "statomigrazione.xml";

            return Path.Combine(GetFolderAmministrazione(amministrazione), FILE);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amministrazione"></param>
        public static void ClearFolder(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(GetFolderAmministrazione(amministrazione));

            dirInfo.Delete(true);
        }
    }

    /// <summary>
    /// Classe per la gestione del log della migrazione
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<string, Log> _instance = new Dictionary<string, Log>();

        /// <summary>
        /// 
        /// </summary>
        private DocsPaVO.amministrazione.InfoAmministrazione _amministrazione = null;

        /// <summary>
        /// 
        /// </summary>
        private int _logItems = 0;

        /// <summary>
        /// 
        /// </summary>
        private StringBuilder _values = null;

        /// <summary>
        /// 
        /// </summary>
        private string _currentValue = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amministrazione"></param>
        private Log(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            this._amministrazione = amministrazione;
            this._values = new StringBuilder();
        }

        /// <summary>
        /// Reperimento log operazione corrente in amministrazione
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        public static Log GetInstance(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            if (!_instance.ContainsKey(amministrazione.Codice))
                _instance.Add(amministrazione.Codice, new Log(amministrazione));
            return _instance[amministrazione.Codice];
        }

        /// <summary>
        /// Rimozione log delle operazioni per l'amministrazione
        /// </summary>
        /// <param name="amministrazione"></param>
        public static void Delete(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            if (_instance.ContainsKey(amministrazione.Codice))
                _instance.Remove(amministrazione.Codice);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="exception"></param>
        public void Write(string value, bool exception)
        {
            if (!exception)
                _logItems++;

            string logValue = GetLogValue(value, exception);

            _values.AppendLine(logValue);

            this._currentValue = logValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ReadCurrent()
        {
            return this._currentValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ReadAll()
        {
            return _values.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Flush()
        {
            string filePath = AppDataFolder.GetLogFilePath(this._amministrazione);
            
            File.WriteAllText(filePath, ReadAll());
        }

        /// <summary>
        /// Format del valore da loggare
        /// </summary>
        /// <param name="value"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private string GetLogValue(string value, bool exception)
        {
            if (exception)
                return string.Format("({0}) ERROR {1}{2}", DateTime.Now.ToString(), Environment.NewLine, value);
            else
                return string.Format("({0}) TASK {1}.{2}{3}", DateTime.Now.ToString(), _logItems.ToString(), Environment.NewLine, value);
        }
    }
}