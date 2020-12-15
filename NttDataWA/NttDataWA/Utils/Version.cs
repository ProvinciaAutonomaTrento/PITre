using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NttDataWA.Utils
{
    public class Version
    {
        private const string DEFAULT_APP_NAME = "DOCSPA";
        private const string DEFAULT_COPYRIGHT_NAME = "";
        private const string TITLE_SEPARATOR = " > ";

        private static string _staticApplicationName = string.Empty;
        private static string _copyright = string.Empty;

        public string ApplicationName
        {
            get
            {
                try
                {
                    _staticApplicationName = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.APPLICATION_NAME.ToString()];
                    if (!string.IsNullOrEmpty(_staticApplicationName))
                    {
                        return _staticApplicationName;
                    }
                    else
                    {
                        return DEFAULT_APP_NAME;
                    }
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
        }

        public string CopyrightName
        {
            get
            {
                try
                {
                        _copyright = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.COPYRIGHT_INFORMATION.ToString()];

                    if (!string.IsNullOrEmpty(_copyright))
                        return _copyright;
                    else
                        return DEFAULT_COPYRIGHT_NAME;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
        }

        public string ApplicationNameVersion
        {
            get
            {
                try
                {
                    string version = string.Empty;

                    version = _staticApplicationName + " " + UIManager.AdministrationManager.getApplicationName();
                    //System.Version assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                    //string major = assemblyVersion.Major.ToString();
                    //string minor = assemblyVersion.Minor.ToString();
                    //string build = assemblyVersion.Build.ToString();
                    //string patch = assemblyVersion.Revision.ToString();

                    //if (build != null && build.Equals("0"))
                    //    version = string.Format("{0} {1}.{2}", this.ApplicationName, major, minor);
                    //else
                    //    version = string.Format("{0} {1}.{2} SP {3}", this.ApplicationName, major, minor, build);
#if BETA
    version = string.Format("{0} {1}.{2} Beta", this.ApplicationName, major, minor);
#endif


                    return version;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
        }
    }
}