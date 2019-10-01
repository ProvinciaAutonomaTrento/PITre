using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;


namespace NttDataWA.Popup
{
    public partial class AddKeyword : System.Web.UI.Page
    {
        private string keyWordAdd
        {
            set
            {
                HttpContext.Current.Session["keyWordAdd"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializeLanguage();
                    this.InitializePage();
                    //Session.Abandon(); //DA CANCELLARE!
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
        private void InitializePage()
        {
           
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.AddKeywordBtnOk.Text = Utils.Languages.GetLabelFromCode("AddKeywordBtnOk", language);            
            this.AddKeywordBtnChiudi.Text = Utils.Languages.GetLabelFromCode("AddKeywordBtnChiudi", language);           
            this.AddKeywordLblText.Text = Utils.Languages.GetLabelFromCode("AddKeywordLblText", language);
        }

        protected void AddKeywordBtnOk_Click(object sender, EventArgs e)
        {
            try {
                string msg;
                DocsPaWR.DocumentoParolaChiave parolaC = new DocsPaWR.DocumentoParolaChiave();
                
                //controllo sull'inserimento della parola chiave
                if (this.NewKeywordText.Text.Equals("") || this.NewKeywordText.Text == null)
                {                   
                    //string msg = "Inserire il valore: parola chiave.";
                    msg = "WarningAddKeywordNotFound";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    
                    return;
                }

                parolaC.descrizione = this.NewKeywordText.Text.ToUpper();
                parolaC = DocumentManager.addParolaChiave(this, parolaC);

                if (parolaC != null)
                {
                    // string msg = "Operazione effettuata con successo.";
                    msg = "ConfermAddKeywordOk";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'check', '');} else {parent.parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'check', '');}", true);
                    keyWordAdd = parolaC.systemId;
                    this.NewKeywordText.Text = "";                    
                }
                else
                {
                    // string msg = "Attenzione, parola chiave già presente.";
                    msg = "WarningAddKeywordFound";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                  
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void AddKeywordBtnChiudi_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "AddKeyword", "parent.closeAjaxModal('AddKeyword', 'up', parent);", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
       
    }
}