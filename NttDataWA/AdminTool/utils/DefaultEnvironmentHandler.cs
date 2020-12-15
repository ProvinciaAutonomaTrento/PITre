using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaUtils.LogsManagement;
using DocsPaUtils.Interfaces.DbManagement;
using DocsPaUtils.Configuration;
using System.Configuration;

namespace SAAdminTool.utils
{
    public class DefaultEnvironmentHandler : ILogEnvironmentHandler 
    {
        public string FileName
        {
            get {
                return "fe.log";
            }
        }

        public string FilePath
        {
            get
            {
                string debugPath = string.Empty;
                // Accesso ai dati e reperimento del path in cui saranno scritti i log per l'amministrazione
                debugPath = InitConfigurationKeys.GetValue("0", "FE_LOG_PATH");
                if (debugPath == null)
                    debugPath = ConfigurationManager.AppSettings["DEBUG_PATH"];

                return debugPath;
            }
        }

        public bool Enabled
        {
            get {
                int logLevel = 0;
                // Accesso ai dati e reperimento del path in cui saranno scritti i log per l'amministrazione             
                string kValue = InitConfigurationKeys.GetValue("0", "FE_LOG_LEVEL");
                if (kValue == null)
                    kValue = ConfigurationManager.AppSettings["LOG_LEVEL"];
                Int32.TryParse(kValue, out logLevel);
                return (logLevel>0);
            }
        }
    }
}