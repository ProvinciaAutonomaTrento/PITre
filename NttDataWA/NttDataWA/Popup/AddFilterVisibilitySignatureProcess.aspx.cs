using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDatalLibrary;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class AddFilterVisibilitySignatureProcess : System.Web.UI.Page
    {
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

        private List<FiltroProcessoFirma> FiltroRicerca
        {
            get
            {
                return HttpContext.Current.Session["FiltroRicercaVisibilitySignatureProcess"] as List<FiltroProcessoFirma>;
            }
            set
            {
                HttpContext.Current.Session["FiltroRicercaVisibilitySignatureProcess"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                InitializeLanguage();
                InitializePage();
            }
            SetAjaxAddressbookRuolo();
        }

        private void InitializePage()
        {
            if (this.FiltroRicerca != null)
                BindFilterValues();
        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.ltlRuolo.Text = Utils.Languages.GetLabelFromCode("AddFilterVisibilitySignatureProcess", language);
            this.AddFilterBtnConfirm.Text = Utils.Languages.GetLabelFromCode("AddFilterLbroFirmaConfirm", language);
            this.AddFilterBtnCancel.Text = Utils.Languages.GetLabelFromCode("AddFilterLibroFirmaCancel", language);
        }

        private void SetAjaxAddressbookRuolo()
        {
            string dataUser = RoleManager.GetRoleInSession().systemId;

            Registro reg = RegistryManager.GetRegistryInSession();
            if (reg == null)
            {
                reg = RoleManager.GetRoleInSession().registri[0];
            }
            dataUser = dataUser + "-" + reg.systemId;
            this.RapidRuolo.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + "CALLTYPE_IN_ONLY_ROLE";
        }

        protected void AddFilterBtnConfirm_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                this.BindFilter();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "closeAJM", "parent.closeAjaxModal('AddFilterVisibilitySignatureProcess', 'up', parent);", true);

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void AddFilterBtnCancel_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "closeAJM", "parent.closeAjaxModal('AddFilterVisibilitySignatureProcess', '', parent);", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnAddressBookRuolo_Click(object sender, EventArgs e)
        {
            this.CallType = RubricaCallType.CALLTYPE_CORR_INT;
            HttpContext.Current.Session["AddressBook.from"] = "FILTER_VISIBILITY_SIGNATURE_PROCESS";
            HttpContext.Current.Session["AddressBook.EnableOnly"] = "R";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAddressBook", "parent.parent.ajaxModalPopupAddressBook();", true);
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = this.txtCodiceRuolo.Text;
                if (!string.IsNullOrEmpty(codeAddressBook))
                {
                    RubricaCallType calltype = RubricaCallType.CALLTYPE_PROTO_INT_MITT;
                    ElementoRubrica[] listaCorr = null;
                    Corrispondente corr = null;
                    UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
                    listaCorr = UIManager.AddressBookManager.getElementiRubricaMultipli(codeAddressBook, calltype, true);
                    if (listaCorr != null && (listaCorr.Count() == 1))
                    {
                        if (listaCorr.Count() == 1)
                        {
                            corr = UIManager.AddressBookManager.getCorrispondenteRubrica(codeAddressBook, calltype);
                        }
                        if (corr == null)
                        {
                            this.txtCodiceRuolo.Text = string.Empty;
                            this.txtDescrizioneRuolo.Text = string.Empty;
                            this.UpdPnlRuolo.Update();
                            string msg = "ErrorTransmissionCorrespondentNotFound";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return;
                        }
                        this.txtCodiceRuolo.Text = string.Empty;
                        this.txtDescrizioneRuolo.Text = string.Empty;
                        this.UpdPnlRuolo.Update();
                        if (!corr.tipoCorrispondente.Equals("R"))
                        {
                            string msg = "WarningCorrespondentAsRole";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            this.txtCodiceRuolo.Text = corr.codiceRubrica;
                            this.txtDescrizioneRuolo.Text = corr.descrizione;
                            this.idRuolo.Value = corr.systemId;
                        }
                        this.UpdPnlRuolo.Update();
                    }
                    else
                    {
                        string msg = "ErrorTransmissionCorrespondentNotFound";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                }
                else
                {
                    this.txtCodiceRuolo.Text = string.Empty;
                    this.txtDescrizioneRuolo.Text = string.Empty;
                    this.UpdPnlRuolo.Update();
                }
            }
            catch (Exception ex)
            {
                string msg = "ErrorSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                if (atList != null && atList.Count > 0)
                {
                    Corrispondente corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                    if (corr != null)
                    {
                        this.txtCodiceRuolo.Text = corr.codiceRubrica;
                        this.txtDescrizioneRuolo.Text = corr.descrizione;
                        this.idRuolo.Value = corr.systemId;
                        this.UpdPnlRuolo.Update();
                    }
                }
                HttpContext.Current.Session["AddressBook.At"] = null;
                HttpContext.Current.Session["AddressBook.Cc"] = null;
                HttpContext.Current.Session["AddressBook.type"] = null;
                HttpContext.Current.Session["AddressBook.from"] = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void BindFilterValues()
        {
            foreach (DocsPaWR.FiltroProcessoFirma item in this.FiltroRicerca)
            {
                if (item.Argomento == DocsPaWR.FiltriProcessoFirma.ID_RUOLO_VISIBILITA.ToString())
                {
                    DocsPaWR.Corrispondente corr = AddressBookManager.GetCorrespondentBySystemId(item.Valore);
                    if (corr != null)
                    {
                        this.txtCodiceRuolo.Text = corr.codiceRubrica;
                        this.txtDescrizioneRuolo.Text = corr.descrizione;
                    }
                    this.idRuolo.Value = item.Valore;
                }
                if (item.Argomento == DocsPaWR.FiltriProcessoFirma.DESC_RUOLO_VISIBILITA.ToString())
                {
                    this.txtDescrizioneRuolo.Text = item.Valore;
                }
            }
        }

        private void BindFilter()
        {
            List<FiltroProcessoFirma> processFilter = new List<FiltroProcessoFirma>();
            FiltroProcessoFirma filter;

            #region FILTRO RUOLO

            if (!string.IsNullOrEmpty(this.txtCodiceRuolo.Text))
            {
                filter = new DocsPaWR.FiltroProcessoFirma();
                filter.Argomento = DocsPaWR.FiltriProcessoFirma.ID_RUOLO_VISIBILITA.ToString();
                filter.Valore = this.idRuolo.Value;
                processFilter.Add(filter);
            }
            if(string.IsNullOrEmpty(this.txtCodiceRuolo.Text) && !string.IsNullOrEmpty(this.txtDescrizioneRuolo.Text))
            {
                filter = new DocsPaWR.FiltroProcessoFirma();
                filter.Argomento = DocsPaWR.FiltriProcessoFirma.DESC_RUOLO_VISIBILITA.ToString();
                filter.Valore = this.txtDescrizioneRuolo.Text;
                processFilter.Add(filter);
            }

            this.FiltroRicerca = processFilter;

            #endregion
        }
    }
}