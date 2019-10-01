using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class InterruptionSignatureProcess : System.Web.UI.Page
    {
        #region Properties

        private IstanzaProcessoDiFirma IstanzaProcessoDiFirma
        {
            get
            {
                if (HttpContext.Current.Session["IstanzaProcessoDiFirma"] != null)
                    return (IstanzaProcessoDiFirma)HttpContext.Current.Session["IstanzaProcessoDiFirma"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["IstanzaProcessoDiFirma"] = value;
            }
        }

        private string MotivoRespingimento
        {
            set
            {
                HttpContext.Current.Session["MotivoRespingimento"] = value;
            }
        }
        #endregion

        public int maxLength = 250;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    this.InitializePage();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializePage()
        {
            this.InitializeLanguage();

            this.TxtTextAbortRecord.Text = string.IsNullOrEmpty(HttpContext.Current.Session["MotivoRespingimento"] as String) ? string.Empty : HttpContext.Current.Session["MotivoRespingimento"].ToString();

            this.TxtTextAbortRecord.MaxLength = 250;
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            this.InterruptionSignatureProcessOk.Text = Utils.Languages.GetLabelFromCode("AbortRecordBtnOk", language);
            this.InterruptionSignatureProcessClose.Text = Utils.Languages.GetLabelFromCode("AbortRecordBtnClose", language);
            this.ltrTextAbortRecord.Text = Utils.Languages.GetLabelFromCode("DocumentLitVisibleNotesChars", language) + " ";
            if (!string.IsNullOrEmpty(Request.QueryString["from"]))
            {
                this.lblNoteInterruption.Text = Utils.Languages.GetLabelFromCode("EsaminaltlMotivoRespingimento", language);
            }
            else
            {
                this.lblNoteInterruption.Text = Utils.Languages.GetLabelFromCode("lblNoteInterruption", language);
            }
        }

        protected void InterruptionSignatureProcessClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('InterruptionSignatureProcess', '');} else {parent.closeAjaxModal('InterruptionSignatureProcess', '', parent);};", true);
        }

        protected void InterruptionSignatureProcessOk_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["from"]))
                {
                    MotivoRespingimento = this.TxtTextAbortRecord.Text;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('InterruptionSignatureProcess', '');} else {parent.closeAjaxModal('InterruptionSignatureProcess', 'up', parent);};", true);
                }
                else
                {
                    string idDocPrinc = HttpContext.Current.Session["idDocPrinc"].ToString();

                    if (LibroFirmaManager.InterruptionSignatureProcessByProponent(IstanzaProcessoDiFirma, this.TxtTextAbortRecord.Text, idDocPrinc))
                    {
                        IstanzaProcessoDiFirma.MotivoRespingimento = this.TxtTextAbortRecord.Text;
                        IstanzaProcessoDiFirma.dataChiusura = DateTime.Now.ToString();
                        IstanzaProcessoDiFirma.statoProcesso = TipoStatoProcesso.STOPPED;
                        IstanzaProcessoDiFirma.ChaInterroDa = 'P';

                        IstanzaProcessoDiFirma.DescUtenteInterruzione = UserManager.GetUserInSession().descrizione;
                        if (UIManager.UserManager.getUtenteDelegato() != null)
                            IstanzaProcessoDiFirma.DescUtenteDelegatoInterruzione = UIManager.UserManager.getUtenteDelegato().descrizione;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('InterruptionSignatureProcess', 'up');} else {parent.closeAjaxModal('InterruptionSignatureProcess', 'up', parent);};", true);
                    }
                    else
                    {
                        string msg = "ErrorInterruptionSignatureProcess";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "ErrorInterruptionSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }
    }
}