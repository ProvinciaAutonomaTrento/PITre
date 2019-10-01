using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;


namespace NttDataWA.Popup
{
    public partial class RecipientsDetails : System.Web.UI.Page
    {

        public List<Corrispondente> ListRecipients
        {
            get
            {
                List<Corrispondente> result = new List<Corrispondente>();
                if (HttpContext.Current.Session["listRecipients"] != null)
                {
                    result = HttpContext.Current.Session["listRecipients"] as List<Corrispondente>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listRecipients"] = value;
            }
        }

        public List<Corrispondente> ListRecipientsCC
        {
            get
            {
                List<Corrispondente> result = new List<Corrispondente>();
                if (HttpContext.Current.Session["listRecipientsCC"] != null)
                {
                    result = HttpContext.Current.Session["listRecipientsCC"] as List<Corrispondente>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listRecipientsCC"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitLanguage();

                    switch (Request.QueryString["type"]) {
                        case "cc":
                            this.DrawListRecipientsCC();
                            break;
                        default:
                            this.DrawListRecipients();
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try {
                this.CloseMask();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void CloseMask()
        {
            switch (Request.QueryString["type"])
            {
                case "cc":
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('RecipientsCCDetails');} else {parent.closeAjaxModal('RecipientsCCDetails');};", true);
                    break;
                default:
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('RecipientsDetails');} else {parent.closeAjaxModal('RecipientsDetails');};", true);
                    break;
            }
        }

        private void DrawListRecipients()
        {
            Table tbl = new Table();
            tbl.CssClass = "tbl";

            for (int i = 0; i < this.ListRecipients.Count; i++)
            {
                TableRow row = new TableRow();
                if (i % 2 == 0) row.CssClass = "AltRow";

                TableCell cell = new TableCell();

                Literal lit = new Literal();
                lit.Text = this.ListRecipients[i].descrizione;

                cell.Controls.Add(lit);
                row.Controls.Add(cell);
                tbl.Controls.Add(row);
                this.plcDetails.Controls.Add(tbl);
            }
        }

        private void DrawListRecipientsCC()
        {
            Table tbl = new Table();
            tbl.CssClass = "tbl";

            for (int i = 0; i < this.ListRecipientsCC.Count; i++)
            {
                TableRow row = new TableRow();
                if (i % 2 == 0) row.CssClass = "AltRow";

                TableCell cell = new TableCell();

                Literal lit = new Literal();
                lit.Text = this.ListRecipientsCC[i].descrizione;

                cell.Controls.Add(lit);
                row.Controls.Add(cell);
                tbl.Controls.Add(row);
                this.plcDetails.Controls.Add(tbl);
            }
        }

    }
}