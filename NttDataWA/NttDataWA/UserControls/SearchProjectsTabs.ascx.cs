using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace NttDataWA.UserControls
{
    public partial class SearchProjectTabs : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //try {
                if (!this.IsPostBack)
                {
                    this.InitializePage();
                    this.InitializeLanguage();
                    this.VisibiltyRoleFunctions();
                }
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        private void VisibiltyRoleFunctions()
        {
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_TAB_ARCHIVIO"))
            {
                this.LiSearchArchive.Visible = false;
            }
        }

        public void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.LinkSearchProject.Text = Utils.Languages.GetLabelFromCode("LinkSearchProject", language);
            this.LinkSearchArchive.Text = Utils.Languages.GetLabelFromCode("LinkSearchArchive", language);
        }

        public void InitializePage()
        {
            if (!string.IsNullOrEmpty(this.PageCaller))
            {
                this.LinkSearchProject.Enabled = true;
                this.LinkSearchArchive.Enabled = true;

                switch (this.PageCaller.ToUpper())
                {
                    case "PROJECT":
                        this.LiSearchProject.Attributes.Remove("class");
                        this.LiSearchProject.Attributes.Add("class", "searchIAmSearch");

                        this.LiSearchArchive.Attributes.Remove("class");

                        this.LiSearchArchive.Attributes.Add("class", "searchOther");
                        break;

                    case "ARCHIVE":
                        this.LiSearchArchive.Attributes.Remove("class");
                        this.LiSearchArchive.Attributes.Add("class", "searchIAmSearch");

                        this.LiSearchProject.Attributes.Remove("class");
                        this.LiSearchProject.Attributes.Add("class", "searchOther");

                        break;

                }

                this.LiSearchArchive.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                this.LiSearchProject.Attributes["onclick"] = "disallowOp('IdMasterBody')";

                if (this.IsAdl)
                {
                    this.LiSearchArchive.Visible = false;
                    this.LinkSearchProject.NavigateUrl = ResolveUrl("~/Search/SearchProject.aspx?IsAdl=true");
                }
            }

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

        private bool IsAdl
        {
            get
            {
                return Request.QueryString["IsAdl"] != null ? true : false;
            }
        }
    }
}