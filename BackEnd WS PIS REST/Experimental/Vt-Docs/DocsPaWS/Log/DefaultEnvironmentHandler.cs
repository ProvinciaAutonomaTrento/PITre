using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaUtils.LogsManagement;
using DocsPaUtils.Interfaces.DbManagement;
using DocsPaUtils.Configuration;
using System.Configuration;

namespace DocsPaWS.Log
{
    public class DefaultEnvironmentHandler : ILogEnvironmentHandler 
    {
        public string FileName
        {
            get {
                return "be.log";
            }
        }

        public string FilePath
        {
            get
            {
                string debugPath = string.Empty;
                if (DatabaseFactory.IsValidDatabase())
                {
                    // Accesso ai dati e reperimento del path in cui saranno scritti i log per l'amministrazione

                    debugPath = InitConfigurationKeys.GetValue("0", "BE_LOG_PATH");
                    if (debugPath == null)
                        debugPath = ConfigurationManager.AppSettings["DEBUG_PATH"];
                }
                else
                {
                    return ConfigurationManager.AppSettings["DEBUG_PATH"];
                }
                return debugPath;
            }
        }

        public bool Enabled
        {
            get {
                string force = ConfigurationManager.AppSettings["FORCE_LOG"];
                if (force != null)
                {
                    bool bforce = false;
                    bool.TryParse(force, out bforce);
                    return bforce;
                }

                int logLevel = 0;
                if (DatabaseFactory.IsValidDatabase())
                {
                    // Accesso ai dati e reperimento del path in cui saranno scritti i log per l'amministrazione             
                    string kValue = InitConfigurationKeys.GetValue("0", "BE_LOG_LEVEL");
                    if (kValue == null)
                        kValue = ConfigurationManager.AppSettings["LOG_LEVEL"];
                    Int32.TryParse(kValue, out logLevel);
                }
                else
                {
                    Int32.TryParse(ConfigurationManager.AppSettings["LOG_LEVEL"], out logLevel);
                }
                return (logLevel>0);
            }
        }
    }
}