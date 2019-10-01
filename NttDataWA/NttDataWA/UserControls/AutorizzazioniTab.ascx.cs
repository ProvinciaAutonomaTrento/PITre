using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace NttDataWA.UserControls
{
    public partial class AutorizzazioniTab : System.Web.UI.UserControl
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
            this.LinkAutorizzazioniVersamento.Text = Utils.Languages.GetLabelFromCode("LitAutorizzazioniVersamento", language);
            if (this.IsAdl)
            {
                this.LinkSearchAutorizzazioniVersamento.Text = Utils.Languages.GetLabelFromCode("LiSearchAutorizzazioniVersamento", language);
            }
            else
            {
                this.LinkSearchAutorizzazioniVersamento.Text = Utils.Languages.GetLabelFromCode("LiSearchAutorizzazioniVersamento", language);
            }
        }

        public void InitializePage()
        {
            if (!string.IsNullOrEmpty(this.PageCaller))
            {
                this.LinkAutorizzazioniVersamento.Enabled = true;
                this.LinkSearchAutorizzazioniVersamento.Enabled = true;

                switch (this.PageCaller.ToUpper())
                {
                    case "NEW":
                        this.LiSearchDocumentSimple.Attributes.Remove("class");
                        this.LiSearchDocumentSimple.Attributes.Add("class", "searchIAmSearch");
                        this.LiSearchDocumentAdvanced.Attributes.Remove("class");
                        this.LiSearchDocumentAdvanced.Attributes.Add("class", "searchOther");
                        break;

                    case "SEARCH":
                        this.LiSearchDocumentAdvanced.Attributes.Remove("class");
                        this.LiSearchDocumentAdvanced.Attributes.Add("class", "searchIAmSearch");
                        this.LiSearchDocumentSimple.Attributes.Remove("class");
                        this.LiSearchDocumentSimple.Attributes.Add("class", "searchOther");
                        break;
                }

                this.LiSearchDocumentSimple.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                this.LiSearchDocumentAdvanced.Attributes["onclick"] = "disallowOp('IdMasterBody')";

                if (this.IsAdl)
                {
                    this.LiSearchDocumentSimple.Visible = false;
                    this.LinkSearchAutorizzazioniVersamento.NavigateUrl = ResolveUrl("~/Search/SearchAutorizzazioniVersamento.aspx?IsAdl=true");
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
