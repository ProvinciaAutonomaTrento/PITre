using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace NttDataWA.UserControls
{
    public partial class ScartiTab : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    this.InitializePage();
                    this.InitializeLanguage();
                    this.VisibiltyRoleFunctions();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void VisibiltyRoleFunctions()
        {

        }

        public void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.LinkGestioneScarto.Text = Utils.Languages.GetLabelFromCode("LinkGestioneScarto", language);
        }

        public void InitializePage()
        {
            if (!string.IsNullOrEmpty(this.PageCaller))
            {
                this.LinkGestioneScarto.Enabled = true;


                switch (this.PageCaller.ToUpper())
                {
                    case "NEW":
                        this.LiSearchDocumentSimple.Attributes.Remove("class");
                        this.LiSearchDocumentSimple.Attributes.Add("class", "searchIAmSearch");
                        break;
                }

                this.LiSearchDocumentSimple.Attributes["onclick"] = "disallowOp('IdMasterBody')";


                if (this.IsAdl)
                {
                    this.LiSearchDocumentSimple.Visible = false;
                    this.LinkGestioneScarto.NavigateUrl = ResolveUrl("~/Deposito/Scarto.aspx?IsAdl=true");
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