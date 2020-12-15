using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class AddFilterSignatureProcesses : System.Web.UI.Page
    {
        #region Properties

        private List<FiltroProcessoFirma> FiltersProcesses
        {
            get
            {
                return (List<FiltroProcessoFirma>)HttpContext.Current.Session["FiltroProcessoFirma"];
            }
            set
            {
                HttpContext.Current.Session["FiltroProcessoFirma"] = value;
            }
        }

        private RubricaCallType CallType
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

        #region Standard method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
            }
        }

        private void InitializePage()
        {
            InitializeLanguage();
            PopolaDddlOrdinaPer();
            if (this.FiltersProcesses != null)
            {
                BindFilterValues();
            }
            this.SetAjaxAddressbookRuoloTitolare();
            this.SetAjaxAddressbookUtenteTitolare();
        }

        private void SetAjaxAddressbookRuoloTitolare()
        {
            string dataUser = RoleManager.GetRoleInSession().systemId;

            Registro reg = RegistryManager.GetRegistryInSession();
            if (reg == null)
            {
                reg = RoleManager.GetRoleInSession().registri[0];
            }
            dataUser = dataUser + "-" + reg.systemId;
            this.RapidRuoloTitolare.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + "CALLTYPE_IN_ONLY_ROLE";
        }

        private void SetAjaxAddressbookUtenteTitolare()
        {
            string dataUser = RoleManager.GetRoleInSession().systemId;

            Registro reg = RegistryManager.GetRegistryInSession();
            if (reg == null)
            {
                reg = RoleManager.GetRoleInSession().registri[0];
            }
            dataUser = dataUser + "-" + reg.systemId;
            this.RapidUtenteTitolare.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + "CALLTYPE_IN_ONLY_USER";
        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.AddFilterBtnConfirm.Text = Utils.Languages.GetLabelFromCode("AddFilterLbroFirmaConfirm", language);
            this.AddFilterBtnCancel.Text = Utils.Languages.GetLabelFromCode("AddFilterLibroFirmaCancel", language);
            this.ltlRuoloTitolare.Text = Utils.Languages.GetLabelFromCode("AddFilterSignatureProcessRuoloTitolare", language);
            this.ltlUtenteTitolare.Text = Utils.Languages.GetLabelFromCode("AddFilterSignatureProcessUtenteTitolare", language);
            this.ltlTipo.Text = Utils.Languages.GetLabelFromCode("AddFilterSignatureProcessTipo", language);
            this.opModello.Text = Utils.Languages.GetLabelFromCode("AddFilterSignatureProcessModello", language);
            this.opProcesso.Text = Utils.Languages.GetLabelFromCode("AddFilterSignatureProcessProcesso", language);
            this.ltlNome.Text = Utils.Languages.GetLabelFromCode("AddFilterSignatureProcessNome", language);
            this.ltlStato.Text = Utils.Languages.GetLabelFromCode("AddFilterSignatureProcessStato", language);
            this.opValido.Text = Utils.Languages.GetLabelFromCode("AddFilterSignatureProcessValido", language);
            this.opInvalido.Text = Utils.Languages.GetLabelFromCode("AddFilterSignatureProcessInvalido", language);
            this.LtlOrdinaPer.Text = Utils.Languages.GetLabelFromCode("lblOrdina", language);
            this.li_asc.Text = Utils.Languages.GetLabelFromCode("AddFilterSignatureProcessAsc", language);
            this.li_desc.Text = Utils.Languages.GetLabelFromCode("AddFilterSignatureProcessDesc", language);
        }

        #endregion

        #region Event Button

        protected void AddFilterBtnConfirm_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                if (this.BindFilters())
                {
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('AddFilterSignatureProcesses','up');", true);
                }

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
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('AddFilterSignatureProcesses','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        private bool BindFilters()
        {
            bool result = true;

            List<FiltroProcessoFirma> processFilter = new List<FiltroProcessoFirma>();
            FiltroProcessoFirma filter;

            #region ORDINAMENTO

            filter = new DocsPaWR.FiltroProcessoFirma();
            filter.Argomento = DocsPaWR.FiltriProcessoFirma.ORDER_FIELD.ToString();
            filter.Valore = this.ddlOrder.SelectedValue;
            processFilter.Add(filter);

            filter = new DocsPaWR.FiltroProcessoFirma();
            filter.Argomento = DocsPaWR.FiltriProcessoFirma.ORDER_DIRECTION.ToString();
            filter.Valore = this.ddlAscDesc.SelectedValue;
            processFilter.Add(filter);

            #endregion

            #region RUOLO COINVOLTO

            if (!string.IsNullOrEmpty(this.txtCodiceRuoloTitolare.Text))
            {
                filter = new DocsPaWR.FiltroProcessoFirma();
                filter.Argomento = DocsPaWR.FiltriProcessoFirma.RUOLO_COINVOLTO.ToString();
                filter.Valore = this.idRuoloTitolare.Value;
                processFilter.Add(filter);
            }

            #endregion

            #region UTENTE COINVOLTO

            if (!string.IsNullOrEmpty(this.txtCodiceUtenteTitolare.Text))
            {
                filter = new DocsPaWR.FiltroProcessoFirma();
                filter.Argomento = DocsPaWR.FiltriProcessoFirma.UTENTE_COINVOLTO.ToString();
                filter.Valore = this.idUtenteTitolare.Value;
                processFilter.Add(filter);
            }

            #endregion

            #region TIPO
            if (this.cbl_TipoProcesso.Items.FindByValue("P") != null)
            {
                filter = new DocsPaWR.FiltroProcessoFirma();
                filter.Argomento = DocsPaWR.FiltriProcessoFirma.TIPO_PROCESSO.ToString();
                if (this.cbl_TipoProcesso.Items.FindByValue("P").Selected)
                    filter.Valore = "true";
                else
                    filter.Valore = "false";
                processFilter.Add(filter);
            }
            if (this.cbl_TipoProcesso.Items.FindByValue("M") != null)
            {
                filter = new DocsPaWR.FiltroProcessoFirma();
                filter.Argomento = DocsPaWR.FiltriProcessoFirma.TIPO_MODELLO.ToString();
                if (this.cbl_TipoProcesso.Items.FindByValue("M").Selected)
                    filter.Valore = "true";
                else
                    filter.Valore = "false";
                processFilter.Add(filter);
            }
            #endregion

            #region NOME
            if (!string.IsNullOrEmpty(this.txtNome.Text.Trim()))
            {
                filter = new DocsPaWR.FiltroProcessoFirma();
                filter.Argomento = DocsPaWR.FiltriProcessoFirma.NOME.ToString();
                filter.Valore = this.txtNome.Text;
                processFilter.Add(filter);
            }
            #endregion

            #region STATO

            if (this.cbl_StatoProcesso.Items.FindByValue("V") != null)
            {
                filter = new DocsPaWR.FiltroProcessoFirma();
                filter.Argomento = DocsPaWR.FiltriProcessoFirma.VALIDO.ToString();
                if (this.cbl_StatoProcesso.Items.FindByValue("V").Selected)
                    filter.Valore = "true";
                else
                    filter.Valore = "false";
                processFilter.Add(filter);
            }
            if (this.cbl_StatoProcesso.Items.FindByValue("I") != null)
            {
                filter = new DocsPaWR.FiltroProcessoFirma();
                filter.Argomento = DocsPaWR.FiltriProcessoFirma.INVALIDO.ToString();
                if (this.cbl_StatoProcesso.Items.FindByValue("I").Selected)
                    filter.Valore = "true";
                else
                    filter.Valore = "false";
                processFilter.Add(filter);
            }

            #endregion

            this.FiltersProcesses = processFilter;

            return result;
        }

        private void BindFilterValues()
        {
            try
            {
                foreach (DocsPaWR.FiltroProcessoFirma item in this.FiltersProcesses)
                {
                    #region ORDINAMENTO
                    if (item.Argomento == DocsPaWR.FiltriProcessoFirma.ORDER_FIELD.ToString())
                    {
                        this.ddlOrder.SelectedValue = item.Valore;
                    }

                    if (item.Argomento == DocsPaWR.FiltriProcessoFirma.ORDER_DIRECTION.ToString())
                    {
                        this.ddlAscDesc.SelectedValue = item.Valore;
                    }
                    #endregion

                    #region RUOLO COINVOLTO
                    if (item.Argomento == DocsPaWR.FiltriProcessoFirma.RUOLO_COINVOLTO.ToString())
                    {
                        DocsPaWR.Corrispondente corr = AddressBookManager.GetCorrespondentBySystemId(item.Valore);
                        if (corr != null)
                        {
                            this.txtCodiceRuoloTitolare.Text = corr.codiceRubrica;
                            this.txtDescrizioneRuoloTitolare.Text = corr.descrizione;
                        }
                        this.idRuoloTitolare.Value = item.Valore;
                    }
                    #endregion

                    #region UTENTE COINVOLTO
                    else if (item.Argomento == DocsPaWR.FiltriProcessoFirma.UTENTE_COINVOLTO.ToString())
                    {
                        DocsPaWR.Corrispondente corr = AddressBookManager.GetCorrespondentBySystemId(item.Valore);
                        if (corr != null)
                        {
                            this.txtCodiceUtenteTitolare.Text = corr.codiceRubrica;
                            this.txtDescrizioneUtenteTitolare.Text = corr.descrizione;
                        }
                        this.idUtenteTitolare.Value = item.Valore;
                    }
                    #endregion

                    #region TIPO

                    else if (item.Argomento == DocsPaWR.FiltriProcessoFirma.TIPO_MODELLO.ToString())
                    {
                        this.cbl_TipoProcesso.Items.FindByValue("M").Selected = Convert.ToBoolean(item.Valore);
                    }
                    else if (item.Argomento == DocsPaWR.FiltriProcessoFirma.TIPO_PROCESSO.ToString())
                    {
                        this.cbl_TipoProcesso.Items.FindByValue("P").Selected = Convert.ToBoolean(item.Valore);
                    }

                    #endregion

                    #region NOME
                    else if (item.Argomento == DocsPaWR.FiltriProcessoFirma.NOME.ToString())
                    {
                        this.txtNome.Text = item.Valore;
                    }
                    #endregion

                    #region STATO

                    else if (item.Argomento == DocsPaWR.FiltriProcessoFirma.VALIDO.ToString())
                    {
                        this.cbl_StatoProcesso.Items.FindByValue("V").Selected = Convert.ToBoolean(item.Valore);
                    }
                    else if (item.Argomento == DocsPaWR.FiltriProcessoFirma.INVALIDO.ToString())
                    {
                        this.cbl_StatoProcesso.Items.FindByValue("I").Selected = Convert.ToBoolean(item.Valore);
                    }

                    #endregion


                }
            }
            catch (Exception e)
            {

            }
        }

        private void PopolaDddlOrdinaPer()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            // Cancellazione degli item delle due drop down list
            ddlOrder.Items.Clear();


            ddlOrder.Items.Add(new ListItem() { Value = OrderBy.DTA_CREAZIONE.ToString(), Text = Utils.Languages.GetLabelFromCode("AddFilterSignatureProcessDataCreazione", language) });
            ddlOrder.Items.Add(new ListItem() { Value = OrderBy.NOME.ToString(), Text = Utils.Languages.GetLabelFromCode("AddFilterSignatureProcessNome", language) });
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = string.Empty;
                if (caller.ID == "txtCodiceRuoloTitolare")
                {
                    codeAddressBook = this.txtCodiceRuoloTitolare.Text;
                }
                if (caller.ID == "txtCodiceUtenteTitolare")
                {
                    codeAddressBook = this.txtCodiceUtenteTitolare.Text;
                }

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
                            if (caller.ID == "txtCodiceRuoloTitolare")
                            {
                                ClearRuoloCoinvolto();
                                this.UpdPnlRuoloTitolare.Update();
                            }
                            if (caller.ID == "txtCodiceUtenteTitolare")
                            {
                                ClearUtenteConvolto();
                                this.UpdPnlUtenteTitolare.Update();
                            }
                            string msg = "ErrorTransmissionCorrespondentNotFound";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return;
                        }
                        if (caller.ID == "txtCodiceRuoloTitolare")
                         {
                             ClearRuoloCoinvolto();
                            if (!corr.tipoCorrispondente.Equals("R"))
                            {
                                string msg = "WarningCorrespondentAsRole";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            else
                            {
                                this.txtCodiceRuoloTitolare.Text = corr.codiceRubrica;
                                this.txtDescrizioneRuoloTitolare.Text = corr.descrizione;
                                this.idRuoloTitolare.Value = corr.systemId;
                            }
                            this.UpdPnlRuoloTitolare.Update();
                        }
                        if (caller.ID == "txtCodiceUtenteTitolare")
                        {
                            ClearUtenteConvolto();
                            if (!corr.tipoCorrispondente.Equals("P"))
                            {
                                string msg = "WarningCorrespondentAsRole";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            else
                            {
                                this.txtCodiceUtenteTitolare.Text = corr.codiceRubrica;
                                this.txtDescrizioneUtenteTitolare.Text = corr.descrizione;
                                this.idUtenteTitolare.Value = corr.systemId;
                            }
                            this.UpdPnlUtenteTitolare.Update();
                        }
                    }
                    else
                    {
                        string msg = "ErrorTransmissionCorrespondentNotFound";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                }
                else
                {
                    if (caller.ID == "txtCodiceRuoloTitolare")
                    {
                        ClearRuoloCoinvolto();
                        this.UpdPnlRuoloTitolare.Update();
                     }
                    if (caller.ID == "txtCodiceUtenteTitolare")
                    {
                        ClearUtenteConvolto();
                        this.UpdPnlUtenteTitolare.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "ErrorSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        private void ClearUtenteConvolto()
        {
            this.txtCodiceUtenteTitolare.Text = string.Empty;
            this.txtDescrizioneUtenteTitolare.Text = string.Empty;
            this.idUtenteTitolare.Value = string.Empty;
        }

        private void ClearRuoloCoinvolto()
        {
            this.txtCodiceRuoloTitolare.Text = string.Empty;
            this.txtDescrizioneRuoloTitolare.Text = string.Empty;
            this.idRuoloTitolare.Value = string.Empty;
        }

        protected void BtnAddressBookRuoloTitolare_Click(object sender, EventArgs e)
        {
            this.CallType = RubricaCallType.CALLTYPE_CORR_INT;
            HttpContext.Current.Session["AddressBook.from"] = "FILTER_SIGNATURE_PROCESS_ROLE";
            HttpContext.Current.Session["AddressBook.EnableOnly"] = "R";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAddressBook", "parent.ajaxModalPopupAddressBook();", true);
        }

        protected void BtnAddressBookUtenteTitolare_Click(object sender, EventArgs e)
        {
            this.CallType = RubricaCallType.CALLTYPE_CORR_INT;
            HttpContext.Current.Session["AddressBook.from"] = "FILTER_SIGNATURE_PROCESS_USER";
            HttpContext.Current.Session["AddressBook.EnableOnly"] = "P";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAddressBook", "parent.ajaxModalPopupAddressBook();", true);
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
                    string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();
                    if (corr != null)
                    {
                        this.txtCodiceRuoloTitolare.Text = corr.codiceRubrica;
                        this.txtDescrizioneRuoloTitolare.Text = corr.descrizione;
                        this.idRuoloTitolare.Value = corr.systemId;
                        this.UpdPnlRuoloTitolare.Update();
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
    }
}