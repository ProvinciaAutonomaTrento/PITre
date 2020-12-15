using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using NttDataWA.UIManager;


namespace NttDataWA.UserControls
{
    public partial class InstanceTabs : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    this.InitializeLanguage();
                    this.InitializePage();
                    this.ClearProperties();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ClearProperties()
        {

        }

        private void InitializePage()
        {
            this.RefreshTabs();
        }

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

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.LinkInstance.Text = Utils.Languages.GetLabelFromCode("LinkInstance", language);
            this.LinkInstanceStructure.Text = Utils.Languages.GetLabelFromCode("LinkInstanceStructure", language);
        }

        public void RefreshTabs()
        {
            if (!string.IsNullOrEmpty(this.PageCaller))
            {
                DocsPaWR.InstanceAccess instance = InstanceAccessManager.getInstanceAccessInSession();

                switch (this.PageCaller.ToUpper())
                {
                    case "CONTENT":
                        this.LinkInstance.Enabled = true;
                        this.LiInstance.Attributes.Remove("class");
                        this.LiInstance.Attributes.Add("class", "prjIAmProfile");

                        this.LinkInstance.Attributes["onclick"] = "disallowOp('IdMasterBody')";

                        if (instance != null)
                        {
                            this.LinkInstanceStructure.Enabled = true;
                            this.LiInstanceStructure.Attributes.Remove("class");
                            this.LiInstanceStructure.Attributes.Add("class", "prjOther");
                            this.LinkInstance.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiInstanceStructure.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        }
                        else
                        {
                            this.LinkInstance.Enabled = true;
                            this.LinkInstanceStructure.Enabled = false;
                        }

                        break;

                    case "STRUCTURE":
                        this.LinkInstance.Enabled = true;
                        this.LiInstance.Attributes.Remove("class");
                        this.LiInstance.Attributes.Add("class", "prjOther");
                        this.LinkInstance.Attributes["onclick"] = "disallowOp('IdMasterBody')";

                        this.LinkInstanceStructure.Enabled = true;
                        this.LinkInstanceStructure.Attributes.Remove("class");
                        this.LiInstanceStructure.Attributes.Add("class", "prjIAmProfile");
                        this.LinkInstanceStructure.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        break;
                }

                this.UpProjectTabs.Update();
            }
        }

    }
}