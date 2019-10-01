using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.UserControls
{
    public partial class messager : System.Web.UI.UserControl
    {

        private string text = string.Empty;

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!string.IsNullOrEmpty(this.Text))
                {
                    this.plc.Visible = true;
                    this.msg.Text = this.Text;
                }

                if (!IsPostBack)
                {
                    string language = UIManager.UserManager.GetUserLanguage();
                    this.MessangerWarning.Text = Utils.Languages.GetLabelFromCode("MessangerWarning", language);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
    }
}