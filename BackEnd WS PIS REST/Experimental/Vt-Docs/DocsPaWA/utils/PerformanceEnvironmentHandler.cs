using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaUtils.LogsManagement;
using DocsPaUtils.Interfaces.DbManagement;
using DocsPaUtils.Configuration;
using System.Configuration;
namespace DocsPAWA.utils
{
    public class PerformanceEnvironmentHandler : ILogEnvironmentHandler 
    {
        public string FileName
        {
            get
            {
                return "performance.xml";
            }
        }

        public string FilePath
        {
            get
            {
                string debugPath = string.Empty;
                debugPath = InitConfigurationKeys.GetValue("0", "FE_PERFORMANCE_LOG_PATH");
                if (debugPath == null)
                    debugPath = ConfigurationManager.AppSettings["PERFORMANCE_DEBUG_PATH"];
                
                return debugPath;
            }
        }

        public bool Enabled
        {
            get
            {
                int logLevel = 0;
                string kValue = InitConfigurationKeys.GetValue("0", "FE_PERFORMANCE_LOG_LEVEL");
                if (kValue == null)
                    kValue = ConfigurationManager.AppSettings["PERFORMANCE_LOG_LEVEL"];
                Int32.TryParse(kValue, out logLevel);
                return (logLevel > 0);
            }
        }
    }
}