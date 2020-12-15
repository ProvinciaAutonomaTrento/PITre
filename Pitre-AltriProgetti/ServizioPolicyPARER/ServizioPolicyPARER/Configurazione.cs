using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ServizioPolicyPARER
{
    class Configurazione
    {

        public static string pathLog
        {
            get
            {
                string pathlog = string.Empty;
                const string PATHLOG = "pathLog";

                if (ConfigurationSettings.AppSettings[PATHLOG] != null &&
                    ConfigurationSettings.AppSettings[PATHLOG].ToString() != string.Empty)
                    pathlog = ConfigurationSettings.AppSettings[PATHLOG].ToString().ToUpper();

                return pathlog;
            }
        }

        public static int logLevel
        {
            get
            {
                string loglevel = "0";
                const string LOGLEVEL = "loglevel";


                if (ConfigurationSettings.AppSettings[LOGLEVEL] != null &&
                    ConfigurationSettings.AppSettings[LOGLEVEL].ToString() != string.Empty)
                    loglevel = ConfigurationSettings.AppSettings[LOGLEVEL].ToString().ToUpper();


                int loglevel_i = 0;
                int.TryParse(loglevel, out loglevel_i);
                return loglevel_i;
            }
        }

    }
}
