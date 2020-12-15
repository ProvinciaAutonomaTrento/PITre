using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class Formazione : System.Web.UI.Page
    {

        #region Properties

        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_PROTO_INT_DEST;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }

        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                this.boxShowHide.Visible = false; 
                // this.boxCleanDocuments.Visible = false;
                // this.txtDocumentiEliminati.Visible = false;
                System.Web.HttpContext.Current.Session["Formazione_idUO"] = null;
                System.Web.HttpContext.Current.Session["Formazione_enableUpload"] = null;
                System.Web.HttpContext.Current.Session["TemplateCaricato"] = null;

                InitializedLanguages();
                SetAjaxAddressBook();


                // this.panelUploadFileFormazione.Visible = false;
                //this.test.Visible = false;
                //this.prova.Visible = false;
            }
            else
            {
                //this.test.Visible = true;
                //this.prova.Visible = true;


                //bool uploadEnable = false;
                //var test = System.Web.HttpContext.Current.Session["Formazione_enableUpload"];
                //bool.TryParse(System.Web.HttpContext.Current.Session["Formazione_enableUpload"] != null ? System.Web.HttpContext.Current.Session["Formazione_enableUpload"].ToString() : "false", out uploadEnable);
                //bool idUOhasValue = System.Web.HttpContext.Current.Session["Formazione_idUO"] != null;
                //// this.panelUploadFileFormazione.Visible = uploadEnable && idUOhasValue;
                //if(uploadEnable && idUOhasValue)
                //{
                //    // this.boxUploadFileFormazione.Attributes.Add("style", "visibility:Visible");
                //}
                
            }
            
            

        }

        protected void SetAjaxAddressBook()
        {
            string dataUser = UIManager.RoleManager.GetRoleInSession().systemId;
            Registro reg = UIManager.RegistryManager.GetRegistryInSession();
            if (reg == null)
            {
                reg = UIManager.RoleManager.GetRoleInSession().registri[0];
            }
            dataUser = dataUser + "-" + reg.systemId;

            string callType = "CALLTYPE_IN_ONLY_UO";
            this.RapidUO.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
        }

        private void InitializedLanguages()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.FormazioneBtnConfirm.Text = Utils.Languages.GetLabelFromCode("FormazioneBtnConfirm", language);
            this.FormazioneBtnCancel.Text = Utils.Languages.GetLabelFromCode("FormazioneBtnCancel", language);
            this.ltlUO.Text = Utils.Languages.GetLabelFromCode("FormazioneLtlUO", language);
            this.LnkPulisciUO.Text = Utils.Languages.GetLabelFromCode("FormazioneLnkPulisciUO", language);
            this.LnkPopolaUO.Text = Utils.Languages.GetLabelFromCode("FormazioneLnkPopolaUO", language);
            this.LblMessage.Text = Utils.Languages.GetLabelFromCode("FormazioneLblMessagePulisciUO", language);
        }

        protected void FormazioneBtnConfirm_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);

            bool uploadEnable = false;
            bool.TryParse(System.Web.HttpContext.Current.Session["Formazione_enableUpload"] != null ? System.Web.HttpContext.Current.Session["Formazione_enableUpload"].ToString() : "false", out uploadEnable);

            // TemplateCaricato
            if (System.Web.HttpContext.Current.Session["TemplateCaricato"] == null && uploadEnable)
            {
                //WarningTemplateFormazioneNonCaricato
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningTemplateFormazioneNonCaricato', 'warning', '');} else {parent.ajaxDialogModal('WarningTemplateFormazioneNonCaricato', 'warning', '');};", true);

                return;
            }


            
            bool pulisciUO = this.liPulisciUO.Attributes["class"].Equals("addressTab");
            
            if (string.IsNullOrEmpty(this.idUO.Value))
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningFormazioneUoMancante', 'warning', '');} else {parent.ajaxDialogModal('WarningFormazioneUoMancante', 'warning', '');};", true);
       
            bool result = true;

            if (pulisciUO)
                result = UIManager.FormazioneManager.PulisciUnitaOrganizzativa(this.idUO.Value);
            else
                result = UIManager.FormazioneManager.PopolaUnitaOrganizzativa(this.idUO.Value);

            if (result)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('SuccessFormazioneConfirm', 'check', '');} else {parent.ajaxDialogModal('SuccessFormazioneConfirm', 'check', '');};", true);
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorFormazioneConfirm', 'error', '');} else {parent.ajaxDialogModal('ErrorFormazioneConfirm', 'error', '');};", true);
        }

        protected void FormazioneBtnCancel_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);      
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('Formazione','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ImgAddressBook_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_CORR_INT;
                HttpContext.Current.Session["AddressBook.from"] = "FORMAZIONE";
                HttpContext.Current.Session["AddressBook.EnableOnly"] = "U";
                //OpenAddressBookFromFilter = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "AddressBook", "parent.ajaxModalPopupFormazioneAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> ccList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.Cc"];
                string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

                if (atList != null && atList.Count > 0)
                {
                    NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                    Corrispondente tempCorrSingle;
                    if (!corrInSess.isRubricaComune)
                        tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                    else
                        tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                    this.txtCodiceUO.Text = tempCorrSingle.codiceRubrica;
                    this.txtDescrizioneUO.Text = tempCorrSingle.descrizione;
                    this.idUO.Value = tempCorrSingle.systemId;
                    this.UpnlUO.Update();
                }

                System.Web.HttpContext.Current.Session["Formazione_idUO"] = this.idUO.Value;
                this.showHideUploadBox();

                HttpContext.Current.Session["AddressBook.At"] = null;
                HttpContext.Current.Session["AddressBook.Cc"] = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCodiceUO_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = caller.Text;

                if (!string.IsNullOrEmpty(this.txtCodiceUO.Text))
                {
                    RubricaCallType calltype = RubricaCallType.CALLTYPE_PROTO_INT_MITT;
                    Corrispondente corr = null;
                    ElementoRubrica[] listaCorr = null;
                    UIManager.RegistryManager.SetRegistryInSession(UIManager.RoleManager.GetRoleInSession().registri[0]);
                    listaCorr = UIManager.AddressBookManager.getElementiRubricaMultipli(this.txtCodiceUO.Text, calltype, true);
                    if (listaCorr != null && (listaCorr.Count() == 1))
                    {
                        corr = UIManager.AddressBookManager.getCorrispondenteRubrica(this.txtCodiceUO.Text, calltype);
                        if (corr == null || !corr.tipoCorrispondente.Equals("U"))
                        {
                            this.txtCodiceUO.Text = string.Empty;
                            this.txtDescrizioneUO.Text = string.Empty;
                            this.idUO.Value = string.Empty;
                            this.UpnlUO.Update();
                            string msg = "ErrorTransmissionCorrespondentNotFound";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            this.txtCodiceUO.Text = corr.codiceRubrica;
                            this.txtDescrizioneUO.Text = corr.descrizione;
                            this.idUO.Value = corr.systemId;
                            this.UpnlUO.Update();

                            
                        }
                    }
                }
                else
                {
                    this.txtCodiceUO.Text = string.Empty;
                    this.txtDescrizioneUO.Text = string.Empty;
                    this.idUO.Value = string.Empty;
                    this.UpnlUO.Update();
                }

                System.Web.HttpContext.Current.Session["Formazione_idUO"] = this.idUO.Value;
                this.showHideUploadBox();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
        
        protected void LnkPulisciUO_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            string language = UIManager.UserManager.GetUserLanguage();
            try
            {
                System.Web.HttpContext.Current.Session["Formazione_enableUpload"] = false;
                this.liPulisciUO.Attributes.Remove("class");
                this.liPulisciUO.Attributes.Add("class", "addressTab");
                this.liPopolaUO.Attributes.Remove("class");
                this.liPopolaUO.Attributes.Add("class", "otherAddressTab");

                System.Web.HttpContext.Current.Session["Formazione_enableUpload"] = false;
                this.showHideUploadBox();

                this.LblMessage.Text = Utils.Languages.GetLabelFromCode("FormazioneLblMessagePulisciUO", language);
                this.UpnlMessagge.Update();
                this.UpnlFormazioneTab.Update();
            }
            catch (Exception ex)
            {

            }
        }

        protected void LnkPopolaUO_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            string language = UIManager.UserManager.GetUserLanguage();
            try
            {
                System.Web.HttpContext.Current.Session["Formazione_enableUpload"] = true;
                this.liPopolaUO.Attributes.Remove("class");
                this.liPopolaUO.Attributes.Add("class", "addressTab");
                this.liPulisciUO.Attributes.Remove("class");
                this.liPulisciUO.Attributes.Add("class", "otherAddressTab");

                this.LblMessage.Text = Utils.Languages.GetLabelFromCode("FormazioneLblMessagePopolaUO", language);
                this.UpnlMessagge.Update();
                this.UpnlFormazioneTab.Update();

                System.Web.HttpContext.Current.Session["Formazione_enableUpload"] = true;
                this.showHideUploadBox();
            }
            catch(Exception ex)
            {

            }
        }


        private void showHideUploadBox()
        {
            bool uploadEnable = false;
            bool.TryParse(System.Web.HttpContext.Current.Session["Formazione_enableUpload"] != null ? System.Web.HttpContext.Current.Session["Formazione_enableUpload"].ToString() : "false", out uploadEnable);
            string idUO = System.Web.HttpContext.Current.Session["Formazione_idUO"] != null ? System.Web.HttpContext.Current.Session["Formazione_idUO"] .ToString() : String.Empty ;
            // this.panelUploadFileFormazione.Visible = uploadEnable && idUOhasValue;

            if (uploadEnable && !String.IsNullOrEmpty(idUO))
            {
                //this.boxCleanDocuments.Visible = false;
                //this.txtDocumentiEliminati.Visible = false;
                this.boxShowHide.Visible = true;
                this.updateTest.Update();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "jquerizeUpload", "jquerizeUpload()", true);
            } 
            else if (!uploadEnable && !String.IsNullOrEmpty(idUO))
            {
                this.boxShowHide.Visible = false;
                // this.boxCleanDocuments.Visible = true;
                //this.txtDocumentiEliminati.Visible = true;
                this.updateTest.Update();
                // ScriptManager.RegisterStartupScript(this, this.GetType(), "createDeleteButton", "createDeleteButton()", true);
            }
            else
            {
                // this.boxShowHide.Attributes.Add("class", "hidden");
                this.boxShowHide.Visible = false;
                // this.boxCleanDocuments.Visible = false;
                //this.txtDocumentiEliminati.Visible = true;
                this.updateTest.Update();
            }
        }

        //private void _setUI(string idUO)
        //{
        //    if(String.IsNullOrWhiteSpace(idUO))
        //    {
                
        //        System.Web.HttpContext.Current.Session["Formazione_idUO"] = null;
        //    } else
        //    {
        //        System.Web.HttpContext.Current.Session["Formazione_idUO"] = idUO;
        //    }
        //}


    }
}