using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace NttDataWA.Guide
{
    public partial class Guide : System.Web.UI.Page
    {
        protected string from;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "loading", "disallowOp('Content2');", true);
                this.InitializeLanguage();
                string nomeBookmark = this.Request.QueryString["from"];
                string sLine = string.Empty;

                string valoreChiave = Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, "FE_ELENCO_BOOKMARKS");
              
                if (!string.IsNullOrEmpty(valoreChiave))
                {
                    if (!string.IsNullOrEmpty(nomeBookmark))
                    {
                        StreamReader objReader = new StreamReader(valoreChiave);
                        while (sLine != null)
                        {
                            sLine = objReader.ReadLine();
                            if (sLine != null && sLine.Contains(nomeBookmark + "="))
                            {
                                this.from = sLine.Substring(sLine.IndexOf("="));
                                this.frame.Attributes["src"] = "../Guide/guide.pdf#page" + from;
                            }
                        }
                        objReader.Close();
                    }
                }

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeFrame", "resizeIframeViewer();", true);
            }
        }

        private void InitializeLanguage()
        {
              string language = UIManager.UserManager.GetUserLanguage();
              this.GuideBtnClose.Text = Utils.Languages.GetLabelFromCode("GuideBtnClose", language);
        }

        protected void GuideBtnClose_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('Guide','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
    }
}