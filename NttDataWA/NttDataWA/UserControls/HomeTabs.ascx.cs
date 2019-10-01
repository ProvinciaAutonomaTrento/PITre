using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace NttDataWA.UserControls
{
    public partial class HomeTabs : System.Web.UI.UserControl
    {

        #region Property

        [Browsable(true)]
        public string PageCaller
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["pageCaller"] != null)
                {
                    result = HttpContext.Current.Session["pageCaller"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["pageCaller"] = value;
            }
        }

        #endregion

        #region Const

        private const string NOTIFICATION_CENTER = "NOTIFICATION_CENTER";
        private const string ADL_DOCUMENT = "ADL_DOCUMENT";
        private const string ADL_PROJECT = "ADL_PROJECT";
        private const string LIBRO_FIRMA = "LIBRO_FIRMA";
        private const string TASK = "TASK";

        #endregion

        #region Standard method

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!this.IsPostBack)
                {
                    this.InitializePage();
                    this.InitializeLanguage();
                }
                this.UnderlineTab(this.PageCaller.ToUpper());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitializePage()
        {
            if (!string.IsNullOrEmpty(this.PageCaller))
            {
                this.LinkNotificationCenter.Enabled = true;
                this.LinkAdlDocument.Enabled = true;
                this.LinkAdlProject.Enabled = true;
                this.LinkLibroFirma.Enabled = true;
                this.LinkTask.Enabled = true;

                switch (this.PageCaller.ToUpper())
                {
                    case NOTIFICATION_CENTER:
                        this.LiNotificationCenter.Attributes.Remove("class");
                        this.LiNotificationCenter.Attributes.Add("class", "homeIAmHome");

                        this.LiAdlProject.Attributes.Remove("class");
                        this.LiAdlDocument.Attributes.Remove("class");
                        this.LiLibroFirma.Attributes.Remove("class");
                        this.LiTask.Attributes.Remove("class");

                        this.LiAdlProject.Attributes.Add("class", "homeOther");
                        this.LiAdlDocument.Attributes.Add("class", "homeOther");
                        this.LiLibroFirma.Attributes.Add("class", "homeOther");
                        this.LiTask.Attributes.Add("class", "homeOther");
                        break;

                    case ADL_DOCUMENT:
                        this.LiAdlDocument.Attributes.Remove("class");
                        this.LiAdlDocument.Attributes.Add("class", "homeIAmHome");

                        this.LiNotificationCenter.Attributes.Remove("class");
                        this.LiAdlProject.Attributes.Remove("class");
                        this.LiLibroFirma.Attributes.Remove("class");
                        this.LiTask.Attributes.Remove("class");

                        this.LiNotificationCenter.Attributes.Add("class", "homeOther");
                        this.LiAdlProject.Attributes.Add("class", "homeOther");
                        this.LiLibroFirma.Attributes.Add("class", "homeOther");
                        this.LiTask.Attributes.Add("class", "homeOther");
                        break;

                    case ADL_PROJECT:
                        this.LiAdlProject.Attributes.Remove("class");
                        this.LiAdlProject.Attributes.Add("class", "homeIAmHome");

                        this.LiNotificationCenter.Attributes.Remove("class");
                        this.LiAdlDocument.Attributes.Remove("class");
                        this.LiLibroFirma.Attributes.Remove("class");
                        this.LiTask.Attributes.Remove("class");

                        this.LiNotificationCenter.Attributes.Add("class", "homeOther");
                        this.LiAdlDocument.Attributes.Add("class", "homeOther");
                        this.LiLibroFirma.Attributes.Add("class", "homeOther");
                        this.LiTask.Attributes.Add("class", "homeOther");
                        break;

                    case LIBRO_FIRMA:
                        this.LiLibroFirma.Attributes.Remove("class");
                        this.LiLibroFirma.Attributes.Add("class", "homeIAmHome");
                        
                        this.LiNotificationCenter.Attributes.Remove("class");
                        this.LiAdlDocument.Attributes.Remove("class");
                        this.LiAdlProject.Attributes.Remove("class");
                        this.LiTask.Attributes.Remove("class");

                        this.LiNotificationCenter.Attributes.Add("class", "homeOther");
                        this.LiAdlDocument.Attributes.Add("class", "homeOther");
                        this.LiAdlProject.Attributes.Add("class", "homeOther");
                        this.LiTask.Attributes.Add("class", "homeOther");
                        break;

                    case TASK:
                        this.LiTask.Attributes.Remove("class");
                        this.LiTask.Attributes.Add("class", "homeIAmHome");
                        
                        this.LiNotificationCenter.Attributes.Remove("class");
                        this.LiAdlDocument.Attributes.Remove("class");
                        this.LiAdlProject.Attributes.Remove("class");
                        this.LiLibroFirma.Attributes.Remove("class");

                        this.LiNotificationCenter.Attributes.Add("class", "homeOther");
                        this.LiAdlDocument.Attributes.Add("class", "homeOther");
                        this.LiAdlProject.Attributes.Add("class", "homeOther");
                        this.LiLibroFirma.Attributes.Add("class", "homeOther");
                        break;
                }

                this.LiNotificationCenter.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                this.LiAdlDocument.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                this.LiAdlProject.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                this.LiLibroFirma.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                this.LiTask.Attributes["onclick"] = "disallowOp('IdMasterBody')";

            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_LIBRO_FIRMA.ToString())) && Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_LIBRO_FIRMA.ToString()).Equals("1") && UIManager.UserManager.IsAuthorizedFunctions("DO_LIBRO_FIRMA"))
            {
                this.LiLibroFirma.Visible = true;
            }
            else
            {
                this.LiLibroFirma.Visible = false;
            }
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.ENABLE_TASK.ToString())) && Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.ENABLE_TASK.ToString()).Equals("1") && UIManager.UserManager.IsAuthorizedFunctions("DO_TASK"))
            {
                this.LiTask.Visible = true;
            }
            else
            {
                this.LiTask.Visible = false;
            }
        }

        private void InitializeLanguage()
        { 
            string language = UIManager.UserManager.GetUserLanguage();
            this.LinkNotificationCenter.Text = Utils.Languages.GetLabelFromCode("HomeTabsLinkNotificationCenter", language);
            this.LinkAdlDocument.Text = Utils.Languages.GetLabelFromCode("HomeTabsLinkAdlDocument", language);
            this.LinkAdlProject.Text = Utils.Languages.GetLabelFromCode("HomeTabsLinkAdlProject", language);
            this.LinkLibroFirma.Text = Utils.Languages.GetLabelFromCode("HomeTabsLinkLibroFirma", language);
            this.LinkTask.Text = Utils.Languages.GetLabelFromCode("HomeTabsLinkTask", language);
        }

    #endregion

        private void UnderlineTab(string pageCaller)
        {
            string language = UIManager.UserManager.GetUserLanguage();

            //LIBRO_FIRMA
            string elementiInLibroFirma = Utils.Languages.GetLabelFromCode("HomeTabElementiInLibroFirma", language);
            int countElementiInLibroFirma = UIManager.LibroFirmaManager.CountElementiInLibroFirma();
            this.LinkLibroFirma.ToolTip = countElementiInLibroFirma + " " + elementiInLibroFirma;
            if (countElementiInLibroFirma > 0)
            {
                this.LiLibroFirma.Attributes.Remove("class");
                if (pageCaller.Equals("LIBRO_FIRMA"))
                {
                    this.LiLibroFirma.Attributes.Add("class", "homeIAmHomeUnderline");
                }
                else
                {
                    this.LiLibroFirma.Attributes.Add("class", "homeOtherUnderline");
                }
            }
        }

        public virtual void RefreshLayoutTab()
        {
            switch (this.PageCaller.ToUpper())
            {
                case NOTIFICATION_CENTER:
                    this.LiNotificationCenter.Attributes.Remove("class");
                    this.LiNotificationCenter.Attributes.Add("class", "homeIAmHome");

                    this.LiAdlProject.Attributes.Remove("class");
                    this.LiAdlDocument.Attributes.Remove("class");
                    this.LiLibroFirma.Attributes.Remove("class");
                    this.LiTask.Attributes.Remove("class");

                    this.LiAdlProject.Attributes.Add("class", "homeOther");
                    this.LiAdlDocument.Attributes.Add("class", "homeOther");
                    this.LiLibroFirma.Attributes.Add("class", "homeOther");
                    this.LiTask.Attributes.Add("class", "homeOther");
                    break;

                case ADL_DOCUMENT:
                    this.LiAdlDocument.Attributes.Remove("class");
                    this.LiAdlDocument.Attributes.Add("class", "homeIAmHome");

                    this.LiNotificationCenter.Attributes.Remove("class");
                    this.LiAdlProject.Attributes.Remove("class");
                    this.LiLibroFirma.Attributes.Remove("class");
                    this.LiTask.Attributes.Remove("class");

                    this.LiNotificationCenter.Attributes.Add("class", "homeOther");
                    this.LiAdlProject.Attributes.Add("class", "homeOther");
                    this.LiLibroFirma.Attributes.Add("class", "homeOther");
                    this.LiTask.Attributes.Add("class", "homeOther");
                    break;

                case ADL_PROJECT:
                    this.LiAdlProject.Attributes.Remove("class");
                    this.LiAdlProject.Attributes.Add("class", "homeIAmHome");

                    this.LiNotificationCenter.Attributes.Remove("class");
                    this.LiAdlDocument.Attributes.Remove("class");
                    this.LiLibroFirma.Attributes.Remove("class");
                    this.LiTask.Attributes.Remove("class");

                    this.LiNotificationCenter.Attributes.Add("class", "homeOther");
                    this.LiAdlDocument.Attributes.Add("class", "homeOther");
                    this.LiLibroFirma.Attributes.Add("class", "homeOther");
                    this.LiTask.Attributes.Add("class", "homeOther");
                    break;

                case LIBRO_FIRMA:
                    this.LiLibroFirma.Attributes.Remove("class");
                    this.LiLibroFirma.Attributes.Add("class", "homeIAmHome");

                    this.LiNotificationCenter.Attributes.Remove("class");
                    this.LiAdlDocument.Attributes.Remove("class");
                    this.LiAdlProject.Attributes.Remove("class");
                    this.LiTask.Attributes.Remove("class");

                    this.LiNotificationCenter.Attributes.Add("class", "homeOther");
                    this.LiAdlDocument.Attributes.Add("class", "homeOther");
                    this.LiAdlProject.Attributes.Add("class", "homeOther");
                    this.LiTask.Attributes.Add("class", "homeOther");
                    break;

                case TASK:
                    this.LiTask.Attributes.Remove("class");
                    this.LiTask.Attributes.Add("class", "homeIAmHome");

                    this.LiNotificationCenter.Attributes.Remove("class");
                    this.LiAdlDocument.Attributes.Remove("class");
                    this.LiAdlProject.Attributes.Remove("class");
                    this.LiLibroFirma.Attributes.Remove("class");

                    this.LiNotificationCenter.Attributes.Add("class", "homeOther");
                    this.LiAdlDocument.Attributes.Add("class", "homeOther");
                    this.LiAdlProject.Attributes.Add("class", "homeOther");
                    this.LiLibroFirma.Attributes.Add("class", "homeOther");
                    break;
            }
            UnderlineTab(this.PageCaller.ToUpper());
            this.UpHomeTabs.Update();
        }
    }
}