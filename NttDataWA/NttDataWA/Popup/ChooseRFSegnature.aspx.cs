using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class ChooseRFSegnature : System.Web.UI.Page
    {

        #region Properties

        private SchedaDocumento DocumentInWorking
        {
            get
            {
                SchedaDocumento result = null;
                if (HttpContext.Current.Session["document"] != null)
                {
                    result = HttpContext.Current.Session["document"] as SchedaDocumento;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["document"] = value;
            }
        }

        private string Code
        {
            get
            {
                string result = null;
                if (HttpContext.Current.Session["ChooseRFSegnature_code"] != null)
                {
                    result = HttpContext.Current.Session["ChooseRFSegnature_code"] as string;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ChooseRFSegnature_code"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!this.Page.IsPostBack)
                {
                    this.InitLanguage();

                    // l'utente può selezionare registro o RF da utilizzare
                    this.lbl_doc_rf.Visible = true;

                    if (!string.IsNullOrEmpty(this.Code))
                    {
                        switch (this.Code.ToUpper())
                        {
                            case "SEGNATURA":
                                this.lbl_doc_rf.Text = this.GetLabel("ChooseRFSegnatureToView");
                                this.litTitle.Text = this.GetLabel("ChooseRFSegnatureToViewTitle");
                                this.divCaselle.Visible = false;
                                break;

                            case "RICEV":
                            case "RICEVUTA":
                                this.lbl_doc_rf.Text = this.GetLabel("ChooseRFSegnatureToSend");
                                this.litTitle.Text = this.GetLabel("ChooseRFSegnatureToSendTitle");
                                this.divCaselle.Visible = true;
                                break;
                        }
                    }
                    this.CaricaComboRegistri(this.ddl_regRF, this.Code);
                }

                this.ReApplyScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ReApplyScripts()
        {
            this.ReApplyChosenScript();
        }

        private void ReApplyChosenScript()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen_deselect", "$('.chzn-select-deselect').chosen({ allow_single_deselect: true, no_results_text: '"+utils.FormatJs(this.GetLabel("GenericChosenSelectNone"))+"' });", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen", "$('.chzn-select').chosen({ no_results_text: '" + utils.FormatJs(this.GetLabel("GenericChosenSelectNone")) + "' });", true);
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnOk.Text = Utils.Languages.GetLabelFromCode("GenericBtnOk", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.lblMailAddress.Text = Utils.Languages.GetLabelFromCode("ChooseRFSegnatureMailAddress", language);
            this.ddl_regRF.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language);
            this.ddlCaselle.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language);
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try {
                this.CloseMask(string.Empty);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnOk_Click(object sender, EventArgs e)
        {
            try {
                if (this.ddl_regRF.SelectedValue.Equals(""))
                {
                    string msg = "WarningSendReceiptSelectRF";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'check');", true);
                    return;
                }
                else
                {
                    //salvo in dpa_ass_doc_mail_interop le informazioni sull'rf e la casella da utilizzare per l'invio della ricevuta ed eventualmente della notifica di annullamento
                    DataSet ds = MultiCasellaManager.GetAssDocAddress(DocumentManager.getSelectedRecord().docNumber);
                    if (ds != null && ds.Tables["ass_doc_rf"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["ass_doc_rf"].Rows)
                        {
                            if (!this.Code.Equals("RICEVUTA") && (!ddl_regRF.SelectedValue.Equals(row["registro"])))
                            {
                                string mailAddress = MultiCasellaManager.GetMailPrincipaleRegistro(ddl_regRF.SelectedValue);
                                MultiCasellaManager.UpdateAssDocAddress(DocumentManager.getSelectedRecord().docNumber,
                                    ddl_regRF.SelectedValue, mailAddress);
                            }
                            else
                            {
                                MultiCasellaManager.UpdateAssDocAddress(DocumentManager.getSelectedRecord().docNumber,
                                    ddl_regRF.SelectedValue, ddlCaselle.SelectedValue);
                            }

                        }
                    }

                    this.CloseMask(this.ddl_regRF.SelectedItem.Value);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void CloseMask(string returnValue)
        {
            string popupId = "ChooseRFSegnature";
            if (!string.IsNullOrEmpty(Request.QueryString["fromRecord"])) popupId = "ChooseRFSegnatureFromRecord";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('" + utils.FormatJs(popupId) + "', '" + utils.FormatJs(returnValue) + "');} else {parent.closeAjaxModal('" + utils.FormatJs(popupId) + "', '" + utils.FormatJs(returnValue) + "');};", true);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        private void CaricaComboRegistri(DropDownList ddl, string codice)
        {
            DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getSelectedRecord();
            //verifico se il registro ha RF associati
            DocsPaWR.Registro[] listaRF = UserManager.getListaRegistriWithRF(this, "1", schedaDocumento.registro.systemId);
            if (listaRF != null && listaRF.Length > 0)
            {
                if (listaRF.Length == 1)
                {
                    ListItem li = new ListItem();
                    li.Value = listaRF[0].systemId;
                    li.Text = listaRF[0].codRegistro + " - " + listaRF[0].descrizione;
                    this.ddl_regRF.Items.Add(li);
                    ListItem lit2 = new ListItem();
                    lit2.Value = schedaDocumento.registro.systemId;
                    lit2.Text = schedaDocumento.registro.codRegistro + " - " + schedaDocumento.registro.descrizione;
                    this.ddl_regRF.Items.Add(lit2);
                }
                else
                {
                    ListItem lit = new ListItem();
                    lit.Value = "";
                    lit.Text = " ";
                    this.ddl_regRF.Items.Add(lit);
                    ListItem lit2 = new ListItem();
                    lit2.Value = schedaDocumento.registro.systemId;
                    lit2.Text = schedaDocumento.registro.codRegistro + " - " + schedaDocumento.registro.descrizione;
                    this.ddl_regRF.Items.Add(lit2);

                    foreach (DocsPaWR.Registro regis in listaRF)
                    {
                        if (codice.ToUpper().Equals("RICEVUTA"))
                        {
                            if (!regis.invioRicevutaManuale.ToUpper().Equals("1"))
                            {
                                ListItem li = new ListItem();
                                li.Value = regis.systemId;
                                li.Text = regis.codRegistro + " - " + regis.descrizione;
                                this.ddl_regRF.Items.Add(li);
                            }
                        }
                        else
                        {
                            ListItem li = new ListItem();
                            li.Value = regis.systemId;
                            li.Text = regis.codRegistro + " - " + regis.descrizione;
                            this.ddl_regRF.Items.Add(li);
                        }
                    }
                }
                if (codice != null && codice.Equals("RICEVUTA") && (!string.IsNullOrEmpty(ddl_regRF.SelectedValue)))
                {
                    this.CaricaComboCaselle(ddl_regRF.SelectedValue);
                }
            }
            else
            {
                this.ddl_regRF.Visible = false;
                this.ddlCaselle.Visible = false;
            }
        }

        private void CaricaComboCaselle(string idRegistro)
        {
            DocsPaWR.CasellaRegistro[] caselle = MultiCasellaManager.GetMailRegistro(idRegistro);
            if (caselle != null && caselle.Length > 0)
            {
                foreach (DocsPaWR.CasellaRegistro c in caselle)
                {
                    System.Text.StringBuilder formatString = new System.Text.StringBuilder();
                    if (c.Principale.Equals("1"))
                        formatString.Append("* ");
                    formatString.Append(c.EmailRegistro);
                    if (!string.IsNullOrEmpty(c.Note))
                        formatString.Append(" - " + c.Note);
                    ddlCaselle.Items.Add(new ListItem { Text = formatString.ToString(), Value = c.EmailRegistro });
                    if (c.Principale.Equals("1"))
                        ddlCaselle.SelectedValue = c.EmailRegistro;
                }
            }

            this.ddlCaselle_IndexChanged(null, null);
        }

        protected void ddl_regRF_IndexChanged(object sender, EventArgs e)
        {
            try {
                if (string.IsNullOrEmpty(this.ddl_regRF.SelectedValue))
                {
                    this.ddlCaselle.Items.Clear();
                    this.BtnOk.Enabled = false;
                    this.UpPnlButtons.Update();
                }
                else
                {
                    this.ddlCaselle.Items.Clear();
                    this.CaricaComboCaselle(ddl_regRF.SelectedValue);
                    this.UpPnlButtons.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddlCaselle_IndexChanged(object sender, EventArgs e)
        {
            try {
                if (string.IsNullOrEmpty(this.ddlCaselle.SelectedValue))
                {
                    this.BtnOk.Enabled = false;
                    this.UpPnlButtons.Update();
                }
                else
                {
                    this.BtnOk.Enabled = true;
                    this.UpPnlButtons.Update();
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