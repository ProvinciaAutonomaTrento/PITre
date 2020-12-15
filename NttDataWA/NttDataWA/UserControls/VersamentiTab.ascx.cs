using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace NttDataWA.UserControls
{
    public partial class VersamentiTab : System.Web.UI.UserControl
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
            this.LinkGestioneVersamento.Text = Utils.Languages.GetLabelFromCode("LitAutorizzazioniVersamento", language);
            this.LinkVersamentoImpact.Text = Utils.Languages.GetLabelFromCode("LinkAnalisiImpatto", language);
        }

        public void InitializePage()
        {
            if (!string.IsNullOrEmpty(this.PageCaller))
            {
                this.LinkGestioneVersamento.Enabled = true;
                this.LinkVersamentoImpact.Enabled = true;

                switch (this.PageCaller.ToUpper())
                {
                    case "NEW":
                        this.LiSearchDocumentSimple.Attributes.Remove("class");
                        this.LiSearchDocumentSimple.Attributes.Add("class", "searchIAmSearch");

                        this.LiVersamentoImpact.Attributes.Remove("class");

                        this.LiVersamentoImpact.Attributes.Add("class", "searchOther");
                        break;

                    case "IMPACT":
                        this.LiVersamentoImpact.Attributes.Remove("class");
                        this.LiVersamentoImpact.Attributes.Add("class", "searchIAmSearch");

                        this.LiSearchDocumentSimple.Attributes.Remove("class");

                        this.LiSearchDocumentSimple.Attributes.Add("class", "searchOther");
                        break;
                }

                this.LiSearchDocumentSimple.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                this.LiVersamentoImpact.Attributes["onclick"] = "disallowOp('IdMasterBody')";

                if (this.IsAdl)
                {
                    this.LiVersamentoImpact.Visible = false;
                    this.LiSearchDocumentSimple.Visible = false;
                    this.LinkGestioneVersamento.NavigateUrl = ResolveUrl("~/Deposito/Versamento.aspx?IsAdl=true");
                }
            }

        }

        public void enableImpactTabs(bool value)
        {
            this.LiVersamentoImpact.Visible = value;
            this.LinkVersamentoImpact.Enabled = value;

            string language = UIManager.UserManager.GetUserLanguage();
            this.LinkVersamentoImpact.Text = value ? Utils.Languages.GetLabelFromCode("LinkAnalisiImpatto", language) : String.Empty;

            this.UpAutorizzazioniTabs.Update();
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
