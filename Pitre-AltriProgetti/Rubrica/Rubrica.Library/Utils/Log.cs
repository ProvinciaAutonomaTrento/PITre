using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace Rubrica.Library.Utils
{
    /// <summary>
    /// Classe di utilità per la gestione dei log
    /// </summary>
    public sealed class Log
    {
        /// <summary>
        /// 
        /// </summary>
        private Log()
        { }

        /// <summary>
        /// Log di un valore
        /// </summary>
        /// <param name="value"></param>
        public static void Write(string value)
        {
            using (StreamWriter writer = File.AppendText(LogFullPath))
                writer.WriteLine(string.Concat(GetLogTitle("TASK"), Environment.NewLine, value));
        }

        /// <summary>
        /// Log di un command
        /// </summary>
        /// <param name="command"></param>
        public static void Write(IDbCommand command)
        {
            try
            {
                string args = string.Empty;

                foreach (IDataParameter par in command.Parameters)
                {
                    if (args != string.Empty)
                        args = string.Concat(args, ", ");

                    args = string.Concat(args, string.Format("{0}:'{1}'", par.ParameterName, par.Value));
                }

                string value = string.Format("{0} [{1}]", command.CommandText, args);

                using (StreamWriter writer = File.AppendText(LogFullPath))
                    writer.WriteLine(string.Concat(GetLogTitle("COMMAND"), Environment.NewLine, value));
            }
            catch (Exception ex)
            {
                //alcune volte , se le chiamate arrivano troppo ravvicinate , il log non riesce a tracciarle e si blocca 
            }
        }

        /// <summary>
        /// Log di un'eccezione
        /// </summary>
        /// <param name="exception"></param>
        public static void Write(Exception exception)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(LogFullPath))
                    writer.Write(string.Concat(GetLogTitle("ERROR"),
                                            Environment.NewLine,
                                            exception.GetBaseException().Message,
                                            Environment.NewLine,
                                            exception.GetBaseException().StackTrace));
            }
            catch (Exception ex)
            {
                //alcune volte , se le chiamate arrivano troppo ravvicinate , il log non riesce a tracciarle e si blocca 
            }
      }

        /// <summary>
        /// Reperimento percorso completo del file di log
        /// </summary>
        private static string LogFullPath
        {
            get
            {
                string machineName = System.Environment.MachineName;
                string logFolder = string.Format(@"{0}\RubricaComune\{4}\{1}\{2}\{3}\", 
                                                        LogRootPath,
                                                        DateTime.Now.Year, 
                                                        DateTime.Now.Month, 
                                                        DateTime.Now.Day,
                                                        machineName);

                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                const string FILE_NAME = "Log.txt";
                return Path.Combine(logFolder, FILE_NAME);
            }
        }

        /// <summary>
        /// Reperimento root path per la scrittura dei file di log della rubrica
        /// </summary>
        private static string LogRootPath
        {
            get
            {
                string rootPath = System.Configuration.ConfigurationManager.AppSettings["LogRootPath"];

                if (string.IsNullOrEmpty(rootPath))
                    rootPath = Path.GetTempPath();

                return rootPath;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private static string GetLogTitle(string title)
        {
            if (Security.SecurityHelper.AuthenticatedPrincipal != null &&
                Security.SecurityHelper.AuthenticatedPrincipal.Identity != null)
                return string.Concat(Environment.NewLine, string.Format("{0} - {1} - Utente: {2}", DateTime.Now.ToString(), title, Security.SecurityHelper.AuthenticatedPrincipal.Identity.Name));
            else
                return string.Concat(Environment.NewLine, string.Format("{0} - {1}", DateTime.Now.ToString(), title));
        }
    }
}
