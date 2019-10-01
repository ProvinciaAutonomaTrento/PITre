using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaUtils
{
    public class logWriter
    {
        public static string logSourcePath = string.Empty;
        public static int loglevel = 0;
        public static bool logAttive = false;
        public static string INIT = " INIT ";
        public static string ERRORE = " ERRORE ";
        public static string INFO = " INFO ";
        public static string DEBUG = " DEBUG ";

        public static void addLog(string tipoLog, string messaggio)
        {
            if (!string.IsNullOrEmpty(logSourcePath) && logAttive)
            {
                if (tipoLog == INIT || (tipoLog == ERRORE && loglevel > 0) || (tipoLog == INFO && loglevel > 1) || (tipoLog == DEBUG && loglevel > 2))
                    System.IO.File.AppendAllText(@logSourcePath, "[" + tipoLog + "]:" + "[" + System.DateTime.Now.ToString() + "]" + messaggio + System.Environment.NewLine);
            }
        }
    }
}
