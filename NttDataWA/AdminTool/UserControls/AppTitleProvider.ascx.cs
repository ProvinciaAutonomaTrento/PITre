using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Reflection;

namespace SAAdminTool.UserControls
{
    /// <summary>
    /// Usercontrol di utilità per l'impostazione del nome dell'applicazione a ciascuna pagina
    /// </summary>
    public partial class AppTitleProvider : System.Web.UI.UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        private const string CONFIG_KEY = "TITLE";
        private const string DEFAULT_APP_NAME = "DOCSPA";
        private const string DEFAULT_COPYRIGHT_NAME = "© Copyright VALUE TEAM  2002-2006";
        private const string TITLE_SEPARATOR = " > ";
        private const string PAGE_TITLE_VIEW_STATE = "PAGE_TITLE";
        private const string COPYRIGHT_INFORMATION = "COPYRIGHT";

        /// <summary>
        /// 
        /// </summary>
        private static string _staticApplicationName = string.Empty;
        private static string _copyright = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // if (!this.IsPostBack)
            // {
                // Assegnazione del titolo
                this.Page.Title = this.PageTitle;
            // }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Description("Nome della pagina")]
        public string PageName
        {
            get
            {
                if (this.ViewState[PAGE_TITLE_VIEW_STATE] != null)
                    return this.ViewState[PAGE_TITLE_VIEW_STATE].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState[PAGE_TITLE_VIEW_STATE] = value;
            }
        }

        /// <summary>
        /// Reperimento del titolo dell'applicazione
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Description("Nome dell'applicazione")]
        public string ApplicationName
        {
            get
            {
                if (string.IsNullOrEmpty(_staticApplicationName))
                {
                    //_staticApplicationName = System.Configuration.ConfigurationManager.AppSettings[CONFIG_KEY];
                    if (Session["ApplicationName"] == null)
                    {
                        _staticApplicationName = ApplicationManager.getApplicationName(this.Page);
                        Session.Add("ApplicationName", _staticApplicationName);
                    }
                    else
                        _staticApplicationName = Session["ApplicationName"].ToString();
                }

                if (!string.IsNullOrEmpty(_staticApplicationName))
                    return _staticApplicationName;
                else
                    return DEFAULT_APP_NAME;
            }
        }

        /// <summary>
        /// Reperimento copyright
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Description("Copyright")]
        public string CopyrightName
        {
            get
            {
                if (!string.IsNullOrEmpty(_staticApplicationName))
                    _copyright = System.Configuration.ConfigurationManager.AppSettings[COPYRIGHT_INFORMATION];

                if (_copyright != string.Empty)
                    return _copyright;
                else
                    return DEFAULT_COPYRIGHT_NAME;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Description("Nome dell'applicazione insieme ai metadati della versione")]
        public string ApplicationNameVersion
        {
            get
            {
                string version = string.Empty;

                if (this.ApplicationName.ToUpper().Equals("DOCSPA"))
                {
                    Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

                    string major = assemblyVersion.Major.ToString();
                    string minor = assemblyVersion.Minor.ToString();
                    string build = assemblyVersion.Build.ToString();
                    string patch = assemblyVersion.Revision.ToString();

                    if (build != null && build.Equals("0"))
                        version = string.Format("{0} {1}.{2}", this.ApplicationName, major, minor);
                    else
                        version = string.Format("{0} {1}.{2} SP {3}", this.ApplicationName, major, minor, build);
#if BETA
    version = string.Format("{0} {1}.{2} Beta", this.ApplicationName, major, minor);
#endif
                }
                else
                    version = this.ApplicationName;

                return version;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Description("Informazioni sul copyright")]
        public string CopyrightInformation
        {
            get
            {
                return CopyrightName;
            }
        }


        /// <summary>
        /// Creazione del titolo da assegnare alla pagina
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Description("Titolo della maschera")]
        public string PageTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(this.PageName))
                    return string.Format("{0}{1}{2}", this.ApplicationName, TITLE_SEPARATOR, this.PageName);
                else
                    return this.ApplicationName;
            }
        }
    }
}