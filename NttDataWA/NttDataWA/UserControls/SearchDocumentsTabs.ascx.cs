using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using NttDataWA.Utils;

namespace NttDataWA.UserControls
{
    public partial class SearchDocumentsTabs : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!this.IsPostBack)
                {
                    this.InitializePage();
                    this.InitializeLanguage();
                    this.LoadKeys();
                    this.VisibiltyRoleFunctions();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadKeys()
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
            {
                this.LiSearchDocumentSimple.Visible = false;
                this.LiSearchDocumentPrint.Visible = false;
            }
        }

        private void VisibiltyRoleFunctions()
        {
            //Microfunzione che disabilita il protocollo e altri filtri nella ricerca 
            //documenti semplice e avanzata
            if (UIManager.UserManager.IsAuthorizedFunctions("DIS_PROTO_SEARCH_DOC"))
            {
                this.LiSearchDocumentPrint.Visible = false;
            }
        }

        public void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.LinkSearchDocumentSimple.Text = Utils.Languages.GetLabelFromCode("LinkSearchDocumentSimple", language);
            if (this.IsAdl)
            {
                this.LinkSearchAdvanced.Text = Utils.Languages.GetLabelFromCode("LinkSearchAdvancedAdl", language);
            }
            else
            {
                this.LinkSearchAdvanced.Text = Utils.Languages.GetLabelFromCode("LinkSearchAdvanced", language);
            }
            this.LinkSearchDocumentPrint.Text = Utils.Languages.GetLabelFromCode("LinkSearchDocumentPrint", language);
        }

        public void InitializePage()
        {
            if (!string.IsNullOrEmpty(this.PageCaller))
            {
                this.LinkSearchDocumentSimple.Enabled = true;
                this.LinkSearchDocumentPrint.Enabled = true;
                this.LinkSearchAdvanced.Enabled = true;

                switch (this.PageCaller.ToUpper())
                {
                    case "SIMPLE":
                        this.LiSearchDocumentSimple.Attributes.Remove("class");
                        this.LiSearchDocumentSimple.Attributes.Add("class", "searchIAmSearch");

                        this.LiSearchDocumentAdvanced.Attributes.Remove("class");
                        this.LiSearchDocumentPrint.Attributes.Remove("class");

                        this.LiSearchDocumentAdvanced.Attributes.Add("class", "searchOther");
                        this.LiSearchDocumentPrint.Attributes.Add("class", "searchOther");
                        break;

                    case "ADVANCED":
                        this.LiSearchDocumentAdvanced.Attributes.Remove("class");
                        this.LiSearchDocumentAdvanced.Attributes.Add("class", "searchIAmSearch");

                        this.LiSearchDocumentSimple.Attributes.Remove("class");
                        this.LiSearchDocumentPrint.Attributes.Remove("class");

                        this.LiSearchDocumentSimple.Attributes.Add("class", "searchOther");
                        this.LiSearchDocumentPrint.Attributes.Add("class", "searchOther");
                        break;

                    case "PRINTS":
                        this.LiSearchDocumentPrint.Attributes.Remove("class");
                        this.LiSearchDocumentPrint.Attributes.Add("class", "searchIAmSearch");

                        this.LiSearchDocumentSimple.Attributes.Remove("class");
                        this.LiSearchDocumentAdvanced.Attributes.Remove("class");

                        this.LiSearchDocumentSimple.Attributes.Add("class", "searchOther");
                        this.LiSearchDocumentAdvanced.Attributes.Add("class", "searchOther");
                        break;
                }

                this.LiSearchDocumentSimple.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                this.LiSearchDocumentAdvanced.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                this.LiSearchDocumentPrint.Attributes["onclick"] = "disallowOp('IdMasterBody')";

                if (this.IsAdl)
                {
                    this.LiSearchDocumentSimple.Visible = false;
                    this.LiSearchDocumentPrint.Visible = false;
                    this.LinkSearchAdvanced.NavigateUrl = ResolveUrl("~/Search/SearchDocumentAdvanced.aspx?IsAdl=true");
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