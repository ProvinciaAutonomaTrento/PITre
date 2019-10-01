using System;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Web.Services.Protocols;
using System.Xml;

namespace DocsPAWA
{
	/// <summary>
	/// Classe per il debug
	/// </summary>
	/*public class Logger
	{

		public static string getLogPath()
		{ 
            string path = "";
            path = utils.InitConfigurationKeys.GetValue("0", "FE_LOG_PATH");
            if (path == null)
                path = ConfigSettings.getKey(ConfigSettings.KeysENUM.LOG_PATH);

			path = path.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
			return path;
		}

        public static string getLogLevel()
        {
            string level ="0";
            level = utils.InitConfigurationKeys.GetValue("0", "FE_LOG_LEVEL");
            if (level == null)
                level = ConfigSettings.getKey(ConfigSettings.KeysENUM.LOG_LEVEL);
            return level;
        }

		public static void log(string message) 
        {
			string level = "0";
			string path ="";
			level = getLogLevel();
			path = getLogPath();
            log(message, level, path);
		}

        public static void systemLog(string message)
        {
            string level = "0";
            string path = "";
            level = ConfigSettings.getKey(ConfigSettings.KeysENUM.LOG_LEVEL);
            path  = ConfigSettings.getKey(ConfigSettings.KeysENUM.LOG_PATH);
            log(message, level, path);
        }


        private static void log(string message, string level, string path)
        {
            try
            {
                if (!level.Equals("0"))
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string dateString = System.DateTime.Now.ToString("dd/MM/yyyy hh:mm");
                    string fileName = path + "\\DocsPaWA_" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    StreamWriter sw = new StreamWriter(fileName, true);
                    sw.WriteLine(dateString + " - " + message + "\n\n");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception) { }
        }

		public static void logException(Exception e) {
			string level = "0";
			try {
				level = getLogLevel();
			} catch (Exception) {}

			if (!level.Equals("0")) {
				
				string msg = e.ToString();
				//string debug = "";

				SoapException es;
				if (e.GetType().Equals(typeof(SoapException))) {
					es = (SoapException) e;
					//msg = ((XmlElement)es.Detail).GetElementsByTagName("messaggio")[0].FirstChild.Value;
					//debug = ((XmlElement)es.Detail).GetElementsByTagName("debug")[0].FirstChild.Value;
					
				}
				if(level.Equals("2")) 
					msg += "\n" ;
				log(msg);
			}
		}

	}*/
}
