using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace LCDocsPaService
{
    class Configurazione
    {


        public static string RemoteFolderUser
        {
            get
            {
                string remotefolderuser = string.Empty;
                const string REMOTEFOLDERUSER = "RemoteFolderUser";

                if (ConfigurationSettings.AppSettings[REMOTEFOLDERUSER] != null &&
                    ConfigurationSettings.AppSettings[REMOTEFOLDERUSER].ToString() != string.Empty)
                    remotefolderuser = ConfigurationSettings.AppSettings[REMOTEFOLDERUSER].ToString().ToUpper();

                return remotefolderuser;
            }
        }

        public static string RemoteFolderPass
        {
            get
            {
                string remotefolderpass = string.Empty;
                const string REMOTEFOLDERPASS = "RemoteFolderPass";

                if (ConfigurationSettings.AppSettings[REMOTEFOLDERPASS] != null &&
                    ConfigurationSettings.AppSettings[REMOTEFOLDERPASS].ToString() != string.Empty)
                    remotefolderpass = ConfigurationSettings.AppSettings[REMOTEFOLDERPASS].ToString();

                return remotefolderpass;
            }
        }

        public static string RemoteFolderDomain
        {
            get
            {
                string remotefolderdomain = string.Empty;
                const string REMOTEFOLDERDOMAIN = "RemoteFolderDomain";

                if (ConfigurationSettings.AppSettings[REMOTEFOLDERDOMAIN] != null &&
                    ConfigurationSettings.AppSettings[REMOTEFOLDERDOMAIN].ToString() != string.Empty)
                    remotefolderdomain = ConfigurationSettings.AppSettings[REMOTEFOLDERDOMAIN].ToString().ToUpper();

                return remotefolderdomain;
            }
        }

        public static string RemoteFolderRoot
        {
            get
            {
                string remotefolderroot = string.Empty;
                const string REMOTEFOLDERROOT = "RemoteFolderRoot";

                if (ConfigurationSettings.AppSettings[REMOTEFOLDERROOT] != null &&
                    ConfigurationSettings.AppSettings[REMOTEFOLDERROOT].ToString() != string.Empty)
                    remotefolderroot = ConfigurationSettings.AppSettings[REMOTEFOLDERROOT].ToString().ToUpper();

                return remotefolderroot;
            }
        }


        public static string AdobeQueueFolder
        {
            get
            {
                string adobequeuefolder = string.Empty;
                const string ADOBEQUEUEFOLDER = "AdobeQueueFolder";

                if (ConfigurationSettings.AppSettings[ADOBEQUEUEFOLDER] != null &&
                    ConfigurationSettings.AppSettings[ADOBEQUEUEFOLDER].ToString() != string.Empty)
                    adobequeuefolder = ConfigurationSettings.AppSettings[ADOBEQUEUEFOLDER].ToString().ToUpper();

                return adobequeuefolder;
            }
        }

        public static string AdobeInputFolder
        {
            get
            {
                string adobeinputfolder = string.Empty;
                const string ADOBEINPUTFOLDER = "AdobeInputFolder";

                if (ConfigurationSettings.AppSettings[ADOBEINPUTFOLDER] != null &&
                    ConfigurationSettings.AppSettings[ADOBEINPUTFOLDER].ToString() != string.Empty)
                    adobeinputfolder = ConfigurationSettings.AppSettings[ADOBEINPUTFOLDER].ToString().ToUpper();

                return adobeinputfolder;
            }
        }

        public static string AdobeInputFolderHtml
        {
            get
            {
                string adobeinputfolderhtml = string.Empty;
                const string ADOBEINPUTFOLDERHTML = "AdobeInputFolderHtml";

                if (ConfigurationSettings.AppSettings[ADOBEINPUTFOLDERHTML] != null &&
                    ConfigurationSettings.AppSettings[ADOBEINPUTFOLDERHTML].ToString() != string.Empty)
                    adobeinputfolderhtml = ConfigurationSettings.AppSettings[ADOBEINPUTFOLDERHTML].ToString().ToUpper();

                return adobeinputfolderhtml;
            }
        }

        public static string AdobeFailureFolder
        {
            get
            {
                string adobefailurefolder = string.Empty;
                const string ADOBEFAILUREFOLDER = "AdobeFailureFolder";

                if (ConfigurationSettings.AppSettings[ADOBEFAILUREFOLDER] != null &&
                    ConfigurationSettings.AppSettings[ADOBEFAILUREFOLDER].ToString() != string.Empty)
                    adobefailurefolder = ConfigurationSettings.AppSettings[ADOBEFAILUREFOLDER].ToString().ToUpper();

                return adobefailurefolder;
            }
        }

        public static string AdobeFailureFolderHtml
        {
            get
            {
                string adobefailurefolderhtml = string.Empty;
                const string ADOBEFAILUREFOLDERHTML = "AdobeFailureFolderHtml";

                if (ConfigurationSettings.AppSettings[ADOBEFAILUREFOLDERHTML] != null &&
                    ConfigurationSettings.AppSettings[ADOBEFAILUREFOLDERHTML].ToString() != string.Empty)
                    adobefailurefolderhtml = ConfigurationSettings.AppSettings[ADOBEFAILUREFOLDERHTML].ToString().ToUpper();

                return adobefailurefolderhtml;
            }
        }

        public static string AdobeOutputFolder
        {
            get
            {
                string adobeoutputfolder = string.Empty;
                const string ADOBEOUTPUTFOLDER = "AdobeOutputFolder";

                if (ConfigurationSettings.AppSettings[ADOBEOUTPUTFOLDER] != null &&
                    ConfigurationSettings.AppSettings[ADOBEOUTPUTFOLDER].ToString() != string.Empty)
                    adobeoutputfolder = ConfigurationSettings.AppSettings[ADOBEOUTPUTFOLDER].ToString().ToUpper();

                return adobeoutputfolder;
            }
        }

        public static string AdobeOutputFolderHtml
        {
            get
            {
                string adobeoutputfolderhtml = string.Empty;
                const string ADOBEOUTPUTFOLDERHTML = "AdobeOutputFolderHtml";

                if (ConfigurationSettings.AppSettings[ADOBEOUTPUTFOLDERHTML] != null &&
                    ConfigurationSettings.AppSettings[ADOBEOUTPUTFOLDERHTML].ToString() != string.Empty)
                    adobeoutputfolderhtml = ConfigurationSettings.AppSettings[ADOBEOUTPUTFOLDERHTML].ToString().ToUpper();

                return adobeoutputfolderhtml;
            }
        }

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

        public static double pollingDelay
        {
            get
            {
                string pollingDelay = "0";
                const string POLLINGDELAY = "pollingDelay";


                if (ConfigurationSettings.AppSettings[POLLINGDELAY] != null &&
                    ConfigurationSettings.AppSettings[POLLINGDELAY].ToString() != string.Empty)
                    pollingDelay = ConfigurationSettings.AppSettings[POLLINGDELAY].ToString().ToUpper();

                double pollingDelay_i = 0;
                double.TryParse(pollingDelay, out pollingDelay_i);
                return pollingDelay_i;
            }
        }

        public static string getServiceName()
        {
            string retval = "Default Service";

            string value = ConfigurationManager.AppSettings["ServiceName"];
            if (!string.IsNullOrEmpty(value))
                retval = value;

            return retval;
        }


        public static string getServiceDescription()
        {
            string retval = "Default Service";

            string value = ConfigurationManager.AppSettings["ServiceDescription"];
            if (!string.IsNullOrEmpty(value))
                retval = value;

            return retval;
        }


        public static string getUserName()
        {
            string retval = null;

            string value = ConfigurationManager.AppSettings["SvcUserName"];
            if (!string.IsNullOrEmpty(value))
                retval = value;

            return retval;
        }

        public static string getUserPass()
        {
            string retval = null;

            string value = ConfigurationManager.AppSettings["SvcPassword"];
            if (!string.IsNullOrEmpty(value))
                retval = value;

            return retval;
        }

        public static System.ServiceProcess.ServiceStartMode getServiceStartUpType()
        {
            System.ServiceProcess.ServiceStartMode retval = System.ServiceProcess.ServiceStartMode.Manual;

            string value = ConfigurationManager.AppSettings["ServiceStartup"];
            if (!string.IsNullOrEmpty(value))
            {
                switch (value.ToLower().Trim())
                {
                    case "automatic":
                        retval = System.ServiceProcess.ServiceStartMode.Automatic;
                        break;
                    case "disabled":
                        retval = System.ServiceProcess.ServiceStartMode.Disabled;
                        break;
                    case "manual":
                        retval = System.ServiceProcess.ServiceStartMode.Manual;
                        break;
                }
            }

            return retval;
        }


        public static System.ServiceProcess.ServiceAccount getAccountType()
        {
            System.ServiceProcess.ServiceAccount retval = System.ServiceProcess.ServiceAccount.LocalSystem;
            string value = ConfigurationManager.AppSettings["ServiceAccountType"];
            if (!string.IsNullOrEmpty(value))
            {
                switch (value.ToLower().Trim())
                {
                    case "LocalSystem":
                        retval = System.ServiceProcess.ServiceAccount.LocalSystem;
                        break;
                    case "LocalService":
                        retval = System.ServiceProcess.ServiceAccount.LocalService;
                        break;
                    case "NetworkService":
                        retval = System.ServiceProcess.ServiceAccount.NetworkService;
                        break;
                    case "User":
                        retval = System.ServiceProcess.ServiceAccount.User;
                        break;
                }
            }

            return retval;

        }
    

    }
}
