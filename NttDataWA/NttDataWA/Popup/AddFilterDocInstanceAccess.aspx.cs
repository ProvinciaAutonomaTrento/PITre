using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class AddFilterDocInstanceAccess : System.Web.UI.Page
    {
        #region Properties

        private string ReturnValue
        {
            get
            {
                if ((HttpContext.Current.Session["ReturnValuePopup"]) != null)
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
            }
        }

        private string TypeDocument
        {
            get
            {
                return HttpContext.Current.Session["typeDoc"].ToString();

            }
            set
            {
                if (value != null)
                    HttpContext.Current.Session["typeDoc"] = value;
                else if (!string.IsNullOrEmpty(Request.QueryString["t"]))
                    HttpContext.Current.Session["typeDoc"] = Request.QueryString["t"];
                else
                    HttpContext.Current.Session["typeDoc"] = string.Empty;
            }
        }

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

        public bool OpenAddressBookFromFilter
        {
            get
            {
                if (HttpContext.Current.Session["OpenAddressBookFromFilter"] != null)
                    return (Boolean)HttpContext.Current.Session["OpenAddressBookFromFilter"];
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session["OpenAddressBookFromFilter"] = value;
            }
        }

        private FiltroRicerca[][] SearchFilters
        {
            get
            {
                return (FiltroRicerca[][])HttpContext.Current.Session["filtroDocumentInstanceAccess"];
            }
            set
            {
                HttpContext.Current.Session["filtroDocumentInstanceAccess"] = value;
            }
        }

        #endregion

        #region Const
        private const string TYPE_EXT = "4";
        private const string TYPE_PITRE = "3";
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ClearSession();
                InitializeLanguage();
                InitializePage();
                LoadKeys();
                if (SearchFilters != null)
                {
                    PopulatesFields();
                }
            }

            RefreshScript();
        }

        private void ClearSession()
        {
            this.TypeDocument = "Search";
            UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
            //UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
        }


        private void InitializePage()
        {
            this.txt_initIdProto.ReadOnly = false;
            this.txt_fineIdProto.Visible = false;
            this.LtlAIdProto.Visible = false;
            this.LtlDaIdProto.Visible = false;
            this.txt_initIdDoc.ReadOnly = false;
            this.txt_fineIdDoc.Visible = false;
            this.LtlAIdDoc.Visible = false;
            this.LtlDaIdDoc.Visible = false;
            ListaRegistri();
            LoadRegistriRepertorio();
            SetAjaxDescriptionProject();
            this.cbl_archDoc_E.Attributes.Add("onclick", "enableField();");
        }

        private void LoadKeys()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString())) && !Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString()).Equals("1"))
            {
                this.opInt.Attributes.CssStyle.Add("display", "none");
                this.opInt.Selected = false;
            }
            if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FILTRO_ALLEGATI_ESTERNI.ToString())) ||
               !Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FILTRO_ALLEGATI_ESTERNI.ToString()).Equals("1"))
            {
                ListItem itemExternal = (from item in rblFiltriAllegati.Items.Cast<ListItem>() where item.Value.Equals(TYPE_EXT) select item).FirstOrDefault();
                rblFiltriAllegati.Items.Remove(itemExternal);
            }
            if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.INTEROP_SERVICE_ACTIVE.ToString())) ||
             !Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.INTEROP_SERVICE_ACTIVE.ToString()).Equals("1"))
            {
                ListItem itemPitre = (from item in rblFiltriAllegati.Items.Cast<ListItem>() where item.Value.Equals(TYPE_PITRE) select item).FirstOrDefault();
                rblFiltriAllegati.Items.Remove(itemPitre);
            }

        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.AddFilterBtnConfirm.Text = Utils.Languages.GetLabelFromCode("AddFilterInstanceAccessConfirm", language);
            this.AddFilterBtnCancel.Text = Utils.Languages.GetLabelFromCode("AddFilterInstanceAccessCancel", language);
            this.InstanceAccessTypeDocument.Text = Utils.Languages.GetLabelFromCode("SearchDocumentTypeDocument", language);
            this.DocumentLitObject.Text = Utils.Languages.GetLabelFromCode("DocumentLitObject", language);
            this.opPredisposed.Text = Utils.Languages.GetLabelFromCode("opOredisposed", language);
            this.LtlIdProto.Text = Utils.Languages.GetLabelFromCode("AddFilterInstanceAccessLtIdProto", language);
            this.ddl_idProto.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E0", language);
            this.ddl_idProto.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E1", language);
            this.LtlDaIdProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
            this.LtlAIdProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.ddl_idDoc.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E0", language);
            this.ddl_idDoc.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E1", language);
            this.LtlDaIdDoc.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
            this.LtlAIdDoc.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlIdDoc.Text = Utils.Languages.GetLabelFromCode("LtlIdDoc", language);
            this.litMittDest.Text = Utils.Languages.GetLabelFromCode("LtlMitDest", language);
            this.chkMittDestExtendHistoricized.Text = Utils.Languages.GetLabelFromCode("SearchProjectCreatoreExtendHistoricized", language);
            this.LtlIdReg.Text = Utils.Languages.GetLabelFromCode("SearchProjectRegistry", language);
            this.LtlCodFascGenProc.Text = Utils.Languages.GetLabelFromCode("LtlCodFascGenProc", language);
            this.LtlRepertorio.Text = Utils.Languages.GetLabelFromCode("AddFilterInstanceAccessRepertorio", language);
            this.opConf.Text = Utils.Languages.GetLabelFromCode("AddFilterInstanceAccessOpConf", language);
            this.opAut.Text = Utils.Languages.GetLabelFromCode("AddFilterInstanceAccessOpAut", language);
            this.opEstr.Text = Utils.Languages.GetLabelFromCode("AddFilterInstanceAccessOpEstr", language);
            this.opDup.Text = Utils.Languages.GetLabelFromCode("AddFilterInstanceAccessOpDup", language);
            this.InstanceAccessRequest.Text = Utils.Languages.GetLabelFromCode("AddFilterInstanceAccessRequest", language);
            this.LtlAllegati.Text = Utils.Languages.GetLabelFromCode("LtlAllegati", language);
            this.LtlNumAllegatiDoc.Text = Utils.Languages.GetLabelFromCode("LtlNumAllegatiDoc", language);
            if ((DocumentManager.GetDescriptionLabel("A")).Length > 3)
            {
                this.opArr.Text = ((DocumentManager.GetDescriptionLabel("A")).Substring(0, 3)) + "."; //Valore A
            }
            else
            {
                this.opArr.Text = DocumentManager.GetDescriptionLabel("A");
            }

            if ((DocumentManager.GetDescriptionLabel("P")).Length > 3)
            {
                //CASO PER INFORMATICA TRENTINA PER LASCIARE 4 CARATTERI (Part.)
                if (DocumentManager.GetDescriptionLabel("P").Equals("Partenza"))
                {
                    this.opPart.Text = "Part.";
                }
                else
                {
                    this.opPart.Text = ((DocumentManager.GetDescriptionLabel("P")).Substring(0, 3)) + "."; //Valore P
                }
            }
            else
            {
                this.opPart.Text = DocumentManager.GetDescriptionLabel("P");
            }

            if (DocumentManager.GetDescriptionLabel("I").Length > 3)
            {
                this.opInt.Text = ((DocumentManager.GetDescriptionLabel("I")).Substring(0, 3)) + ".";//Valore I
            }
            else
            {
                this.opInt.Text = DocumentManager.GetDescriptionLabel("I");
            }
            if (DocumentManager.GetDescriptionLabel("G").Length > 3)
            {
                this.opGrigio.Text = (DocumentManager.GetDescriptionLabel("G").Substring(0, 3)) + ".";//Valore G
            }
            else
            {
                this.opGrigio.Text = DocumentManager.GetDescriptionLabel("G");
            }
            if (DocumentManager.GetDescriptionLabel("ALL").Length > 3)
            {
                this.opAll.Text = (DocumentManager.GetDescriptionLabel("ALL").Substring(0, 3)) + ".";//Valore ALL
            }
            else
            {
                this.opAll.Text = DocumentManager.GetDescriptionLabel("ALL");
            }
            this.rbOpIS.Text = SimplifiedInteroperabilityManager.SearchItemDescriprion;
        }

        private void LoadRegistriRepertorio()
        {
            List<String> listRegRep = (from d in InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS
                                       where !string.IsNullOrEmpty(d.INFO_DOCUMENT.COUNTER_REPERTORY) && !string.IsNullOrEmpty(d.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO)
                                       select d.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO).Distinct().ToList();
            this.ddl_RegRep.Items.Add( new ListItem() { Text = string.Empty});
            if (listRegRep != null && listRegRep.Count > 0)
            {
                foreach (string s in listRegRep)
                {
                    this.ddl_RegRep.Items.Add(new ListItem()
                    {
                        Text = s
                    });
                }
            }
            this.ddl_RegRep.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlTypeDocument", UserManager.GetUserLanguage()));
        }

        #region Event

        protected void AddFilterBtnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                SearchDocumentFilters();
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('AddFilterDocInstanceAccess','up');", true);

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void AddFilterBtnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('AddFilterDocInstanceAccess','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void chkMittDestExtendHistoricized_Click(object sender, EventArgs e)
        {
            string dataUser = RoleManager.GetRoleInSession().systemId;
            Registro reg = RegistryManager.GetRegistryInSession();
            if (reg == null)
            {
                reg = RoleManager.GetRoleInSession().registri[0];
            }
            dataUser = dataUser + "-" + reg.systemId;
            string callType = string.Empty;

            if (this.chkMittDestExtendHistoricized.Checked)
            {
                callType = "CALLTYPE_CORR_INT_EST_CON_DISABILITATI";
            }
            else
            {
                callType = "CALLTYPE_CORR_INT_EST";
            }
            this.RapidMittDest.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

            this.upPnlMittDest.Update();
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                if (!string.IsNullOrEmpty(this.txtCodiceMittDest.Text))
                {
                    RubricaCallType calltype = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
                    if (this.chkMittDestExtendHistoricized.Checked)
                    {
                        calltype = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                    }
                    else
                    {
                        calltype = RubricaCallType.CALLTYPE_CORR_INT_EST;
                    }
                    Corrispondente corr = null;
                    corr = UIManager.AddressBookManager.getCorrispondenteRubrica(this.txtCodiceMittDest.Text, calltype);
                    if (corr == null)
                    {
                        this.txtCodiceMittDest.Text = string.Empty;
                        this.txtDescrizioneMittDest.Text = string.Empty;
                        this.idMittDest.Value = string.Empty;
                        this.upPnlMittDest.Update();
                        string msg = "ErrorTransmissionCorrespondentNotFound";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                    else
                    {
                        this.txtCodiceMittDest.Text = corr.codiceRubrica;
                        this.txtDescrizioneMittDest.Text = corr.descrizione;
                        this.idMittDest.Value = corr.systemId;
                        this.upPnlMittDest.Update();
                    }
                }
                else
                {
                    this.txtCodiceMittDest.Text = string.Empty;
                    this.txtDescrizioneMittDest.Text = string.Empty;
                    this.idMittDest.Value = string.Empty;
                    this.upPnlMittDest.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_idProto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (this.ddl_idProto.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initIdProto.ReadOnly = false;
                        this.txt_fineIdProto.Visible = false;
                        this.LtlAIdProto.Visible = false;
                        this.LtlDaIdProto.Visible = false;
                        this.txt_fineIdProto.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initIdProto.ReadOnly = false;
                        this.txt_fineIdProto.ReadOnly = false;
                        this.LtlAIdProto.Visible = true;
                        this.LtlDaIdProto.Visible = true;
                        this.txt_fineIdProto.Visible = true;
                        break;
                }
                this.UpPnlIdProto.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_idDoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (this.ddl_idDoc.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initIdDoc.ReadOnly = false;
                        this.txt_fineIdDoc.Visible = false;
                        this.LtlAIdDoc.Visible = false;
                        this.LtlDaIdDoc.Visible = false;
                        this.txt_fineIdDoc.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initIdDoc.ReadOnly = false;
                        this.txt_fineIdDoc.ReadOnly = false;
                        this.LtlAIdDoc.Visible = true;
                        this.LtlDaIdDoc.Visible = true;
                        this.txt_fineIdDoc.Visible = true;
                        break;
                }
                this.UpPnlIdDoc.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnObjectPostback_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ReturnValue))
            {
                this.TxtObject.Text = this.ReturnValue.Split('#').First();
                if (this.ReturnValue.Split('#').Length > 1)
                    this.TxtCodeObject.Text = this.ReturnValue.Split('#').Last();
                this.UpdPnlObject.Update();
            }
        }

        protected void btnTitolarioPostback_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.txt_CodFascicolo.Text = this.ReturnValue.Split('#').First();
                    this.txt_DescFascicolo.Text = this.ReturnValue.Split('#').Last();
                    this.UpCodFasc.Update();
                    txt_CodFascicolo_OnTextChanged(new object(), new EventArgs());
                }   
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SetAjaxDescriptionProject()
        {
            string dataUser = RoleManager.GetRoleInSession().idGruppo;
            dataUser = dataUser + "-" + this.lb_reg_C.SelectedValue;
            if (UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione) != null)
            {
                this.RapidSenderDescriptionProject.ContextKey = dataUser + "-" + UserManager.GetUserInSession().idAmministrazione + "-" + UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione).ID + "-" + UserManager.GetUserInSession().idPeople + "-" + UIManager.UserManager.GetUserInSession().systemId;
            }
        }

        protected void btnSearchProjectPostback_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.txt_CodFascicolo.Text = this.ReturnValue.Split('#').First();
                    this.txt_DescFascicolo.Text = this.ReturnValue.Split('#').Last();
                    this.UpCodFasc.Update();
                    this.txt_CodFascicolo_OnTextChanged(new object(), new EventArgs());
                }
                else if (this.ReturnValue.Contains("//"))
                {
                    this.txt_CodFascicolo.Text = string.Empty;
                    this.txt_DescFascicolo.Text = string.Empty;
                    this.UpCodFasc.Update();
                    this.txt_CodFascicolo_OnTextChanged(new object(), new EventArgs());
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void txt_CodFascicolo_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ProjectManager.removeFascicoloSelezionatoFascRapida(this);

                if (!string.IsNullOrEmpty(this.txt_CodFascicolo.Text))
                {
                    this.SearchProjectRegistro();
                }
                else
                {
                    this.txt_CodFascicolo.Text = string.Empty;
                    this.txt_DescFascicolo.Text = string.Empty;
                    this.IdProject.Value = string.Empty;
                    //Laura 25 Marzo
                    ProjectManager.setProjectInSessionForRicFasc(null);
                    ProjectManager.setProjectInSessionForRicFasc(String.Empty);
                }

                this.UpCodFasc.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchProjectRegistro()
        {
            Registro registro = RegistryManager.getRegistroBySistemId(this.lb_reg_C.SelectedValue);
            this.txt_DescFascicolo.Text = string.Empty;
            string codClassifica = string.Empty;

            if (string.IsNullOrEmpty(this.txt_CodFascicolo.Text))
            {
                this.txt_DescFascicolo.Text = string.Empty;
                return;
            }

            DocsPaWR.Fascicolo[] listaFasc = getFascicoli(registro);

            if (listaFasc != null)
            {
                if (listaFasc.Length > 0)
                {
                    //caso 1: al codice digitato corrisponde un solo fascicolo
                    if (listaFasc.Length == 1)
                    {
                        this.IdProject.Value = listaFasc[0].systemID;
                        this.txt_DescFascicolo.Text = listaFasc[0].descrizione;
                        if (listaFasc[0].tipo.Equals("G"))
                        {
                            codClassifica = listaFasc[0].codice;
                        }
                        else
                        {
                            //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                            DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                            string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                            codClassifica = codiceGerarchia;
                        }
                    }
                    else
                    {
                        //caso 2: al codice digitato corrispondono piu fascicoli
                        codClassifica = this.txt_CodFascicolo.Text;
                        if (listaFasc[0].tipo.Equals("G"))
                        {
                            //codClassifica = codClassifica;
                        }
                        else
                        {
                            //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                            DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                            string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                            codClassifica = codiceGerarchia;
                        }

                        ////Da Fare
                        //RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codClassifica + "', 'Y')</script>");
                        return;
                    }
                }
                else
                {
                    //caso 0: al codice digitato non corrisponde alcun fascicolo
                    if (listaFasc.Length == 0)
                    {
                        //Provo il caso in cui il fascicolo è chiuso
                        Fascicolo chiusoFasc = ProjectManager.getFascicoloDaCodice(this.Page, this.txt_CodFascicolo.Text);
                        if (chiusoFasc != null && !string.IsNullOrEmpty(chiusoFasc.stato) && chiusoFasc.stato.Equals("C"))
                        {
                            //string msg = @"Attenzione, il fascicolo scelto è chiuso. Pertanto il documento non può essere inserito nel fascicolo selezionato.";
                            string msg = "WarningDocumentFileNoOpen";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            //string msg = @"Attenzione, codice fascicolo non presente.";
                            string msg = "WarningDocumentCodFileNoFound";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        this.txt_DescFascicolo.Text = string.Empty;
                        this.txt_CodFascicolo.Text = string.Empty;
                        this.IdProject.Value = string.Empty;
                    }
                }
            }
        }

        private DocsPaWR.Fascicolo[] getFascicoli(DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                string codiceFascicolo = this.txt_CodFascicolo.Text;
                listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");
            }
            if (listaFasc != null)
            {
                return listaFasc;
            }
            else
            {
                return null;
            }
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> ccList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.Cc"];
                if (atList != null && atList.Count > 0)
                {
                    NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                    Corrispondente tempCorrSingle;
                    if (!corrInSess.isRubricaComune)
                        tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                    else
                        tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                    this.txtCodiceMittDest.Text = tempCorrSingle.codiceRubrica;
                    this.txtDescrizioneMittDest.Text = tempCorrSingle.descrizione;
                    this.idMittDest.Value = tempCorrSingle.systemId;
                    this.upPnlMittDest.Update();
                }
                HttpContext.Current.Session["AddressBook.At"] = null;
                HttpContext.Current.Session["AddressBook.Cc"] = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void ImgMittDestAddressBook_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.chkMittDestExtendHistoricized.Checked)
                {
                    this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                }
                else
                {
                    this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST;
                }
                HttpContext.Current.Session["AddressBook.from"] = "F_X_X_S";
                OpenAddressBookFromFilter = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "AddressBook", "parent.ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        private void ListaRegistri()
        {
            bool filtroAoo = false;
            DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistriNoFiltroAOO(out filtroAoo);

            //DocsPaWR.Registro[] registri = UserManager.getRuolo(this).registri;
            //string[] listaReg = new string[registri.Length];
            if (userRegistri != null && filtroAoo)
            {
                ListItem itemM = new ListItem(this.GetLabel("LbRegistroMine"), "M");
                rbl_Reg_C.Items.Add(itemM);
                itemM = new ListItem(this.GetLabel("LbRegistroAll"), "T");
                rbl_Reg_C.Items.Add(itemM);
                itemM = new ListItem(this.GetLabel("LbRegistroReset"), "R");
                rbl_Reg_C.Items.Add(itemM);
                lb_reg_C.Rows = 5;
            }
            else
            {
                userRegistri = RoleManager.GetRoleInSession().registri;
                ListItem itemM = new ListItem(this.GetLabel("LbRegistroAll"), "T");
                rbl_Reg_C.Items.Add(itemM);
                itemM = new ListItem(this.GetLabel("LbRegistroReset"), "R");
                rbl_Reg_C.Items.Add(itemM);
                //rbl_Reg_E.SelectedIndex = 1;
            }
            rbl_Reg_C.SelectedIndex = 0;
            string[] id = new string[userRegistri.Length];
            for (int i = 0; i < userRegistri.Length; i++)
            {
                lb_reg_C.Items.Add(userRegistri[i].codRegistro);
                lb_reg_C.Items[i].Value = userRegistri[i].systemId;
                string nomeRegCurrente = "UserReg" + i;
                // SELEZIONA TUTTI I REGISTRI PRESENTI per DEFAULT
                if (!filtroAoo)
                {
                    if (!userRegistri[i].flag_pregresso)
                        lb_reg_C.Items[i].Selected = true;
                }
                else
                    if (rbl_Reg_C.SelectedItem.Value == "M")
                        for (int j = 0; j < RoleManager.GetRoleInSession().registri.Length; j++)
                        {
                            if (RoleManager.GetRoleInSession().registri[j].codRegistro == lb_reg_C.Items[i].Text)
                            {
                                if (!userRegistri[i].flag_pregresso)
                                {
                                    lb_reg_C.Items[i].Selected = true;
                                    break;
                                }
                            }
                        }

                id[i] = (string)userRegistri[i].systemId;
            }

            if (this.lb_reg_C.Items.Count == 1)
            {
                this.plcRegistro.Visible = false;
                this.UpPnlRegistro.Update();
            }

            //UserManager.setListaIdRegistri(this, listaReg);
            //rbl_Reg_C.Items[0].Selected = true;
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        private void SearchDocumentFilters()
        {
            DocsPaWR.FiltroRicerca[][] qV;
            DocsPaWR.FiltroRicerca[] fVList;
            DocsPaWR.FiltroRicerca fV1;
            //array contenitore degli array filtro di ricerca
            qV = new DocsPaWR.FiltroRicerca[1][];
            qV[0] = new DocsPaWR.FiltroRicerca[1];
            fVList = new DocsPaWR.FiltroRicerca[0];

            #region filtro tipo documento
            if (this.cbl_archDoc_E.Items.FindByValue("A") != null)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("A").Selected)
                    fV1.valore = "true";
                else
                    fV1.valore = "false";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.cbl_archDoc_E.Items.FindByValue("P") != null)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("P").Selected)
                    fV1.valore = "true";
                else
                    //valore += "0^";
                    fV1.valore = "false";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.cbl_archDoc_E.Items.FindByValue("I") != null)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("I").Selected)
                    fV1.valore = "true";
                else
                    fV1.valore = "false";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
            fV1.valore = this.cbl_archDoc_E.Items.FindByValue("G").Selected.ToString();
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

            if (UIManager.DocumentManager.IsEnabledProfilazioneAllegati() && this.cbl_archDoc_E.Items.FindByValue("ALL").Selected)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ALLEGATO.ToString();
                fV1.valore = this.rblFiltriAllegati.SelectedValue.ToString();
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.cbl_archDoc_E.Items.FindByValue("Pr") != null)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("Pr").Selected)
                    //valore += "1";
                    fV1.valore = "true";
                else
                    //valore += "0";
                    fV1.valore = "false";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }


            #endregion
            #region filtro oggetto
            if (!string.IsNullOrEmpty(this.TxtObject.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.OGGETTO.ToString();
                fV1.valore = utils.DO_AdattaString(this.TxtObject.Text);
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region filtro numero protocollo
            if (this.ddl_idProto.SelectedIndex == 0)
            {//valore singolo carico NUM_PROTOCOLLO

                if (this.txt_initIdProto.Text != null && !this.txt_initIdProto.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();
                    fV1.valore = this.txt_initIdProto.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            else
            {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                if (!this.txt_initIdProto.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                    fV1.valore = this.txt_initIdProto.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.txt_fineIdProto.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                    fV1.valore = this.txt_fineIdProto.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region filtro mitt/dest
            if (!this.txtDescrizioneMittDest.Text.Equals(""))
            {
                if (!string.IsNullOrEmpty(this.txtCodiceMittDest.Text))
                {
                    // Ricerca i documenti per i mittenti / destinatari storicizzati
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString();
                    fV1.valore = this.txtCodiceMittDest.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                else
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
                    fV1.valore = this.txtDescrizioneMittDest.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString();
            fV1.valore = this.chkMittDestExtendHistoricized.Checked.ToString();
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            
            #endregion
            #region filtro registro
            if(this.plcRegistro.Visible)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                string registri = "";
                if (this.lb_reg_C.Items.Count > 0)
                {
                    for (int i = 0; i < this.lb_reg_C.Items.Count; i++)
                    {
                        if (this.lb_reg_C.Items[i].Selected)
                        {
                            if (!string.IsNullOrEmpty(registri)) registri += ",";
                            registri += this.lb_reg_C.Items[i].Text;
                        }

                    }
                }
                if (!registri.Equals(""))
                {
                    fV1.valore = registri;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region filtro docNumber
            if (this.ddl_idDoc.SelectedIndex == 0)
            {
                if (this.txt_initIdDoc.Text != null && !this.txt_initIdDoc.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
                    fV1.valore = this.txt_initIdDoc.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            else
            {
                if (this.txt_initIdDoc.Text != null && !this.txt_initIdDoc.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
                    fV1.valore = this.txt_initIdDoc.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.txt_fineIdDoc.Text != null && !this.txt_fineIdDoc.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
                    fV1.valore = this.txt_fineIdDoc.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region filtro repertorio
            if (this.ddl_RegRep.SelectedIndex != 0)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = "TIPOLOGIA_DOCUMENTO";
                fV1.valore = this.ddl_RegRep.SelectedValue.ToString();
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (!string.IsNullOrEmpty(this.txtRepertorio.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = "NUMERO_CONTATORE";
                fV1.valore = this.txtRepertorio.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region filtro Allegati
            if (!string.IsNullOrEmpty(this.txt_allegati.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.NUMERO_ALLEGATI.ToString();
                fV1.valore = this.ddl_op_allegati.SelectedValue + this.txt_allegati.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region filtro CODICE FASCICOLO
            if (!string.IsNullOrEmpty(this.txt_DescFascicolo.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = "DESCRIZIONE_FASCICOLO";
                fV1.valore = this.txt_DescFascicolo.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.CODICE_FASCICOLO.ToString();
                fV1.valore = txt_CodFascicolo.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

            }
            #endregion
            #region filtro TIPO RICHIESTA
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = InstanceAccessManager.TipoRichiesta.COPIA_CONFORME;
            if (this.cbl_request.Items.FindByValue("C").Selected)
                fV1.valore = "true";
            else
                fV1.valore = "false";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = InstanceAccessManager.TipoRichiesta.COPIA_SEMPLICE;
            if (this.cbl_request.Items.FindByValue("A").Selected)
                fV1.valore = "true";
            else
                fV1.valore = "false";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = InstanceAccessManager.TipoRichiesta.ESTRATTO;
            if (this.cbl_request.Items.FindByValue("E").Selected)
                fV1.valore = "true";
            else
                fV1.valore = "false";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = InstanceAccessManager.TipoRichiesta.DUPLCATO;
            if (this.cbl_request.Items.FindByValue("D").Selected)
                fV1.valore = "true";
            else
                fV1.valore = "false";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion

            qV[0] = fVList;
            this.SearchFilters = qV;
        }

        private void PopulatesFields()
        {
            try
            {
                foreach (DocsPaWR.FiltroRicerca item in SearchFilters[0])
                {
                    #region filtro tipo documento

                    if (item.argomento == DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString())
                    {
                        this.cbl_archDoc_E.Items.FindByValue("A").Selected = Convert.ToBoolean(item.valore);
                    }
                    else if (item.argomento == DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString())
                    {
                        this.cbl_archDoc_E.Items.FindByValue("P").Selected = Convert.ToBoolean(item.valore);
                    }
                     else if (item.argomento == DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString())
                    {
                        this.cbl_archDoc_E.Items.FindByValue("I").Selected = Convert.ToBoolean(item.valore);
                    }
                    else if (item.argomento == DocsPaWR.FiltriDocumento.GRIGIO.ToString())
                    {
                        this.cbl_archDoc_E.Items.FindByValue("G").Selected = Convert.ToBoolean(item.valore);
                    }
                    else if (item.argomento == DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString())
                    {
                        this.cbl_archDoc_E.Items.FindByValue("Pr").Selected = Convert.ToBoolean(item.valore);
                    }
                    else if (UIManager.DocumentManager.IsEnabledProfilazioneAllegati() && item.argomento == DocsPaWR.FiltriDocumento.ALLEGATO.ToString())
                    {
                        this.cbl_archDoc_E.Items.FindByValue("ALL").Selected = true;
                        this.rblFiltriAllegati.Items.FindByValue(item.valore.ToString()).Selected = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "enableRb", "enableField();", true);
                    }

                    #endregion
                    #region filtro oggetto
                    else if (item.argomento == DocsPaWR.FiltriDocumento.OGGETTO.ToString())
                    {
                        this.TxtObject.Text = item.valore;
                    }
                    #endregion
                    #region filtro numero protocollo

                    #region NUM_PROTOCOLLO
                    else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString())
                    {
                        if (this.ddl_idProto.SelectedIndex != 0)
                            this.ddl_idProto.SelectedIndex = 0;
                        this.ddl_idProto_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initIdProto.Text = item.valore;
                    }
                    #endregion NUM_PROTOCOLLO
                    #region NUM_PROTOCOLLO_DAL
                    else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString())
                    {
                        if (this.ddl_idProto.SelectedIndex != 1)
                            this.ddl_idProto.SelectedIndex = 1;
                        this.ddl_idProto_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initIdProto.Text = item.valore;
                    }
                    #endregion NUM_PROTOCOLLO_DAL
                    #region NUM_PROTOCOLLO_AL
                    else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString())
                    {
                        if (this.ddl_idProto.SelectedIndex != 1)
                            this.ddl_idProto.SelectedIndex = 1;
                        this.ddl_idProto_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_fineIdProto.Text = item.valore;
                    }
                    #endregion NUM_PROTOCOLLO_AL

                    #endregion
                    #region filtro id documento

                    #region DOCNUMBER
                    else if (item.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER.ToString())
                    {
                        if (this.ddl_idDoc.SelectedIndex != 0)
                            this.ddl_idDoc.SelectedIndex = 0;
                        this.ddl_idDoc_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initIdDoc.Text = item.valore;
                    }
                    #endregion DOCNUMBER
                    #region DOCNUMBER_DAL
                    else if (item.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString())
                    {
                        if (this.ddl_idDoc.SelectedIndex != 1)
                            this.ddl_idDoc.SelectedIndex = 1;
                        this.ddl_idDoc_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initIdDoc.Text = item.valore;
                    }
                    #endregion DOCNUMBER_DAL
                    #region DOCNUMBER_AL
                    else if (item.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString())
                    {
                        if (this.ddl_idDoc.SelectedIndex != 1)
                            this.ddl_idDoc.SelectedIndex = 1;
                        this.ddl_idDoc_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_fineIdDoc.Text = item.valore;
                    }
                    #endregion DOCNUMBER_AL

                    #endregion
                    #region filtro mitt/dest
                    #region filtro MITT_DEST
                    else if (item.argomento == DocsPaWR.FiltriDocumento.MITT_DEST.ToString())
                    {
                        txtDescrizioneMittDest.Text = item.valore;
                    }
                    #endregion MITT_DEST
                    #region COD_MITT_DEST
                    else if (item.argomento == DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString())
                    {
                        DocsPaWR.Corrispondente corr = AddressBookManager.getCorrispondenteByCodRubrica(item.valore, true);
                        this.txtCodiceMittDest.Text = corr.codiceRubrica;
                        txtDescrizioneMittDest.Text = corr.descrizione;
                        this.idMittDest.Value = corr.systemId;
                    }
                    #endregion
                    #region MITT_DEST_STORICIZZATI
                    else if (item.argomento == DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString())
                    {
                        bool chkValue;
                        bool.TryParse(item.valore, out chkValue);
                        this.chkMittDestExtendHistoricized.Checked = chkValue;
                    }
                    #endregion
                    #endregion
                    #region filtro registro
                    else if (item.argomento == DocsPaWR.FiltriDocumento.REGISTRO.ToString())
                    {
                        char[] sep = { ',' };
                        string[] regs = item.valore.Split(sep);
                        foreach (ListItem li in this.lb_reg_C.Items)
                            li.Selected = false;
                        foreach (string reg in regs)
                        {
                            for (int i = 0; i < this.lb_reg_C.Items.Count; i++)
                            {
                                if (this.lb_reg_C.Items[i].Value == reg)
                                    this.lb_reg_C.Items[i].Selected = true;
                            }
                        }
                    }
                    #endregion REGISTRO
                    #region filtro numero allegati
                    else if (item.argomento == DocsPaWR.FiltriDocumento.NUMERO_ALLEGATI.ToString())
                    {
                        if (!string.IsNullOrEmpty(item.valore))
                        {
                            this.ddl_op_allegati.SelectedValue = item.valore.Substring(0, 1);
                            this.txt_allegati.Text = item.valore.Substring(1);
                        }
                    }
                    #endregion
                    #region filtro codice fascicolo

                    else if (item.argomento == DocsPaWR.FiltriDocumento.CODICE_FASCICOLO.ToString())
                    {
                        this.txt_CodFascicolo.Text = item.valore;
                    }
                    else if (item.argomento.Equals("DESCRIZIONE_FASCICOLO"))
                    {
                        this.txt_DescFascicolo.Text = item.valore;
                    }
                    #endregion
                    #region filtro repertorio

                    else if (item.argomento.Equals("TIPOLOGIA_DOCUMENTO"))
                    {
                        this.ddl_RegRep.Items.FindByValue(item.valore).Selected = true;
                    }
                    else if (item.argomento.Equals("NUMERO_CONTATORE"))
                    {
                        this.txtRepertorio.Text = item.valore.ToString();
                    }

                    #endregion
                    #region filtro tipo richiesta

                    if (item.argomento == InstanceAccessManager.TipoRichiesta.COPIA_CONFORME.ToString())
                    {
                        this.cbl_request.Items.FindByValue("C").Selected = Convert.ToBoolean(item.valore);
                    }
                    if (item.argomento == InstanceAccessManager.TipoRichiesta.COPIA_SEMPLICE.ToString())
                    {
                        this.cbl_request.Items.FindByValue("A").Selected = Convert.ToBoolean(item.valore);
                    }
                    if (item.argomento == InstanceAccessManager.TipoRichiesta.ESTRATTO.ToString())
                    {
                        this.cbl_request.Items.FindByValue("E").Selected = Convert.ToBoolean(item.valore);
                    }
                    if (item.argomento == InstanceAccessManager.TipoRichiesta.DUPLCATO.ToString())
                    {
                        this.cbl_request.Items.FindByValue("D").Selected = Convert.ToBoolean(item.valore);
                    }

                    #endregion
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