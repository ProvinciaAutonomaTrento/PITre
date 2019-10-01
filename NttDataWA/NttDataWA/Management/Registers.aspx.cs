using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using System.Collections;
using NttDataWA.UserControls;
using NttDatalLibrary;


namespace NttDataWA.Management
{
    public partial class Registers : System.Web.UI.Page
    {

        #region const
        private const string PANEL_REGISTERS = "panelRegisters";
        private const string UP_PANEL_BUTTONS = "upPnlButtons";
        private const string ALL_MAILBOX = "allMailbox";
        private const string SELECTED_MAILBOX = "selectedMailbox";
        private const string CLOSE_CHECK_MAILBOX_REPORT = "closeCheckMailReport";
        #endregion

        #region global variable
        private static string open;
        private static string close;
        private static string yellow;
        private static string suspended;
        private static string progressBarWait;
        private static string progressBarWaitCloseVerification;
        private static string progressBarWaitInitializeCheck;
        #endregion

        #region Property

        /// <summary>
        /// Registro selezionato nella griglia
        /// </summary>
        private Registro SelectedRegister
        {
            get
            {
                if (HttpContext.Current.Session["selectedRegister"] != null)
                    return (Registro)HttpContext.Current.Session["selectedRegister"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["selectedRegister"] = value;
            }
        }

        /// <summary>
        /// Id corrispondente all'oggetto InfoCheckMailbox 
        /// </summary>
        private string IdCheckMailbox
        {
            get
            {
                if (HttpContext.Current.Session["idCheckMailbox"] != null)
                    return (String)HttpContext.Current.Session["idCheckMailbox"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["idCheckMailbox"] = value;
            }
        }

        private Registro[] ListRegisters
        {
            get
            {
                if (HttpContext.Current.Session["listRegisters"] != null)
                    return (Registro[])HttpContext.Current.Session["listRegisters"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["listRegisters"] = value;
            }
        }


        /// <summary>
        ///  Contiene il file doc per la stampa registri
        /// </summary>
        private FileDocumento FileDocPrintRegister
        {
            get
            {
                return HttpContext.Current.Session["fileDocPrintRegister"] as FileDocumento;
            }
            set
            {
                HttpContext.Current.Session["fileDocPrintRegister"] = value;
            }
        }

        #endregion

        #region Standard Method

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    ClearSession();
                    InitializeLanguage();
                    InitializePage();
                    VisibilityRoleFunction();
                    GrdRegisters_Bind();
                    ButtonsManager(); 
                    UpdateStateMailbox(ALL_MAILBOX);                              
                    this.panelRegisters.Update();
                }
                else
                {
                    this.ReadRetValueFromPopup();
                }
                RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ClearSession()
        {
            HttpContext.Current.Session.Remove("listRegisters");
            HttpContext.Current.Session.Remove("selectedRegister");
        }

        protected void InitializePage()
        {

        }

        protected void DeletePropertyCheckMailboxReport()
        {
            HttpContext.Current.Session.Remove("mailCheckResponse");
            HttpContext.Current.Session.Remove("requestPrintReport");
            HttpContext.Current.Session.Remove("dataSet");
            HttpContext.Current.Session.Remove("idCheckMailbox");
            HttpContext.Current.Session.Remove("readOnlySubtitle");
            HttpContext.Current.Session.Remove("tipoMailRicevutaPec");
            HttpContext.Current.Session.Remove("ReportCreated");
        }

        protected void ReadRetValueFromPopup()
        {

            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(PANEL_REGISTERS))
            {
                this.GrdRegisters.SelectedIndex = int.Parse(this.grid_rowindex.Value);
                HighlightSelectedRow();
                SelectedRegister = ListRegisters[this.GrdRegisters.SelectedIndex + this.GrdRegisters.PageIndex * this.GrdRegisters.PageSize];
                ButtonsManager();
                if ((GrdRegisters.SelectedRow.FindControl("DdlEmailRegister") as DropDownList).Enabled && this.RegistersBtnBox.Visible)
                    UpdateStateMailbox(SELECTED_MAILBOX);
                if ((GrdRegisters.SelectedRow.FindControl("idCheckMailbox") as HiddenField).Value != null)
                    IdCheckMailbox = (GrdRegisters.SelectedRow.FindControl("idCheckMailbox") as HiddenField).Value;
                return;
            }

            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_PANEL_BUTTONS))
            {
                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_CHECK_MAILBOX_REPORT)))
                {
                    CheckMailboxManager.RemoveReportMailbox(IdCheckMailbox);
                    DeletePropertyCheckMailboxReport();
                    ButtonsManager();
                    UpdateStateMailbox(ALL_MAILBOX);
                    this.panelRegisters.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('CheckMailboxReport','')", true);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(HiddenVerifyBox.Value))
            {
                CheckIstitutionalMailbox();
                this.HiddenVerifyBox.Value = string.Empty;
            }

            if (!string.IsNullOrEmpty(this.RegisterModify.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('RegisterModify','')", true);
                return;
            }
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeIframe", "resizeIframe();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
        }

        /// <summary>
        /// Language Manager
        /// </summary>
        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.RegisterModify.Title = Utils.Languages.GetLabelFromCode("RegisterModifyTitle", language);
            this.ViewPrintRegister.Title = Utils.Languages.GetLabelFromCode("ViewPrintRegisterTitle", language);
            this.RegistersBtnChangesState.Text = Utils.Languages.GetLabelFromCode("RegistersBtnChangesState", language);
            this.RegistersBtnBox.Text = Utils.Languages.GetLabelFromCode("RegistersBtnBox", language);
            this.RegistersBtnModify.Text = Utils.Languages.GetLabelFromCode("RegistersBtnModify", language);
            this.RegistersBtnPrint.Text = Utils.Languages.GetLabelFromCode("RegistersBtnPrint", language);
            this.RegistersManagementRegisters.Text = Utils.Languages.GetLabelFromCode("RegistersManagementRegisters", language);
            this.RegistersBtnUpdateStateBox.Text = Utils.Languages.GetLabelFromCode("RegistersBtnUpdateStateBox", language);
            this.CheckMailboxReport.Title = Utils.Languages.GetLabelFromCode("RegistersTitleCheckMailboxReport", language);
            open = Utils.Languages.GetLabelFromCode("RegistersOpen", language);
            close = Utils.Languages.GetLabelFromCode("RegistersClose", language);
            yellow = Utils.Languages.GetLabelFromCode("RegistersYellow", language);
            suspended = Utils.Languages.GetLabelFromCode("RegistersSuspended", language);
            progressBarWait = Utils.Languages.GetLabelFromCode("RegistersLblWait", language);
            progressBarWaitCloseVerification = Utils.Languages.GetLabelFromCode("RegistersLblProgressBarWaitCloseVerification", language);
            progressBarWaitInitializeCheck = Utils.Languages.GetLabelFromCode("RegistersLblProgressBarWaitInitializeCheck", language);
        }

        #endregion

        #region Buttons

        /// <summary>
        /// Verifica micro funzioni attive
        /// </summary>
        private void VisibilityRoleFunction()
        {
            if (UserManager.IsAuthorizedFunctions("GEST_REG_C_STATO"))
            {
                RegistersBtnChangesState.Visible = true;
            }
            else
            {
                RegistersBtnChangesState.Visible = false;
            }
            //if (UserManager.IsAuthorizedFunctions("GEST_CASELLA_IST"))
            //{
            //    RegistersBtnBox.Visible = true;
            //}
            //else
            //{
            //    RegistersBtnBox.Visible = false;
            //}
            if (UserManager.IsAuthorizedFunctions("GEST_REG_MODIFICA"))
            {
                RegistersBtnModify.Visible = true;
            }
            else
            {
                RegistersBtnModify.Visible = false;
            }
            if (UserManager.IsAuthorizedFunctions("GEST_REG_STAMPA"))
            {
                RegistersBtnPrint.Visible = true;
            }
            else
            {
                RegistersBtnPrint.Visible = false;
            }
        }

        private void ButtonsManager()
        {
            this.RegistersBtnModify.Enabled = true;
            this.RegistersBtnPrint.Enabled = false;
            this.RegistersBtnChangesState.Enabled = false;
            this.RegistersBtnBox.Enabled = true;

            if (!SelectedRegister.chaRF.Equals("1"))
            {
                if (SelectedRegister.stato.Equals("C"))
                {
                    this.RegistersBtnPrint.Enabled = true;
                }
                else
                {
                    this.RegistersBtnPrint.Enabled = false;
                }
                if (!SelectedRegister.flag_pregresso == true)
                    this.RegistersBtnChangesState.Enabled = true;
                this.RegistersBtnBox.Enabled = true;

            }
            if (MultiBoxManager.RoleIsAuthorizedConsult(SelectedRegister, UserManager.GetSelectedRole().systemId))
                this.RegistersBtnBox.Enabled = true;
            else
                this.RegistersBtnBox.Enabled = false;
            this.upPnlButtons.Update();
        }

        #endregion

        #region Grid manager

        private void GrdRegisters_Bind()
        {
            if (ListRegisters == null)
            {
                ListRegisters = UserManager.getListaRegistriWithRF(this, "0", "");
                //ListRegisters = RoleManager.GetRoleInSession().registri;
                Registro[] listaRF = null;
                ArrayList allRegisters = new ArrayList();
                for (int i = 0; i < ListRegisters.Length; i++)
                {
                    if (!ListRegisters[i].flag_pregresso)
                    {
                        allRegisters.Add(ListRegisters[i]);
                        //prendo gli RF di ciascun registro
                        listaRF = UserManager.getListaRegistriWithRF(this, "1", (ListRegisters[i]).systemId);
                        for (int j = 0; j < listaRF.Length; j++)
                        {
                            allRegisters.Add(listaRF[j]);
                        }
                    }
                }
                ListRegisters = allRegisters.Cast<Registro>().ToArray();
            }
            this.GrdRegisters.DataSource = ListRegisters;
            this.GrdRegisters.DataBind();
            if (string.IsNullOrEmpty(this.grid_rowindex.Value) && (ListRegisters != null && ListRegisters.Length > 0))
                this.grid_rowindex.Value = "0";
            else if (string.IsNullOrEmpty(this.grid_rowindex.Value) && ListRegisters == null)
                this.grid_rowindex.Value = "-1";
            this.GrdRegisters.SelectedIndex = int.Parse(this.grid_rowindex.Value);
            HighlightSelectedRow();
            SelectedRegister = ListRegisters[this.GrdRegisters.SelectedIndex + this.GrdRegisters.PageIndex * this.GrdRegisters.PageSize];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrdRegisters_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Registro reg = (Registro)e.Row.DataItem;
                    List<string> listMultimailFormat;
                    List<CasellaRegistro> listMultimail = GetEmailsRegister(reg.systemId, out listMultimailFormat);
                    DropDownList DdlEmailRegister = (DropDownList)e.Row.FindControl("DdlEmailRegister");
                    DdlEmailRegister.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", UserManager.GetUserLanguage()));
                    for (int i = 0; i < listMultimail.Count; i++)
                    {
                        DdlEmailRegister.Items.Add(new ListItem()
                        {
                            Value = listMultimail[i].EmailRegistro,
                            Text = listMultimailFormat[i],
                            Selected = listMultimail[i].Principale.Equals("1")
                        });
                    }
                    //PEC 3 gestione visibilità(FLAG CONSULTA)
                    if (MultiBoxManager.RoleIsAuthorizedConsult(reg, UserManager.GetSelectedRole().systemId) && DdlEmailRegister.Items.Count > 0)
                        DdlEmailRegister.Enabled = true;
                    else
                        DdlEmailRegister.Enabled = false;
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (!GrdRegisters.Columns[i].HeaderText.Equals(Utils.Languages.GetLabelFromCode("RegistersGrdEmail", UIManager.UserManager.GetUserLanguage())))
                        {
                            e.Row.Cells[i].Attributes["onClick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "');__doPostBack('panelRegisters');return false;";
                        }
                        else
                            e.Row.Cells[i].Attributes["onClick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "');";
                    }
                    if (reg.chaRF == "1")
                    {
                        Image img = (Image)e.Row.FindControl("btnImageRegister");
                        img.CssClass = "containerCodRegister";
                        e.Row.FindControl("rf").Visible = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void UpdateListRegisters()
        {
            for (int i = 0; i < ListRegisters.Length; i++)
            {
                if (ListRegisters[i].systemId.Equals(SelectedRegister.systemId))
                    ListRegisters[i] = SelectedRegister;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void HighlightSelectedRow()
        {

            if (this.GrdRegisters.Rows.Count > 0 && this.GrdRegisters.SelectedRow != null)
            {
                GridViewRow gvRow = this.GrdRegisters.SelectedRow;
                foreach (GridViewRow GVR in this.GrdRegisters.Rows)
                {
                    if (GVR == gvRow)
                    {
                        GVR.CssClass += " selectedrow";
                        if (ListRegisters[this.GrdRegisters.SelectedIndex + this.GrdRegisters.PageIndex * this.GrdRegisters.PageSize].chaRF != "1")
                            GVR.FindControl("pnlDetails").Visible = true;
                    }
                    else
                    {
                        GVR.CssClass = GVR.CssClass.Replace(" selectedrow", "");
                        GVR.FindControl("pnlDetails").Visible = false;
                    }
                }
            }
        }

        protected void GrdRegisters_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try {
                this.GrdRegisters.PageIndex = e.NewPageIndex;
                this.grid_rowindex.Value = "0";
                GrdRegisters_Bind();
                this.panelRegisters.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Prende l'immagine dipendentemente se si tratta di un registro o un RF
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        protected string GetImage(Registro reg)
        {
            string retValue = string.Empty;
            string spaceIndent = string.Empty;

            switch (reg.chaRF)
            {
                case "0":
                    retValue = "icon_reg.png";
                    break;

                case "1":
                    retValue = "rf_icon.png";
                    spaceIndent = "&nbsp;&nbsp;";
                    break;
            }

            retValue = "../Images/Icons/" + retValue;

            return retValue;
        }

        /// <summary>
        /// Prende l'immagine dello stato del registro dipendentemente se si tratta di un registro o un RF
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        protected string GetImageState(Registro reg)
        {
            string img = string.Empty;
            switch (reg.chaRF)
            {
                case "0":
                    img = setStatoReg(reg);
                    break;
                case "1":
                    img = GetImageStatoRF(reg.rfDisabled);
                    break;
            }
            return img;
        }

        private string setStatoReg(Registro reg)
        {
            string dataApertura = reg.dataApertura;
            string nomeImg;
            string img;
            DateTime dt_cor = DateTime.Now;

            if (UserManager.getStatoRegistro(reg).Equals("G"))
                nomeImg = "giallo.png";
            else if (UserManager.getStatoRegistro(reg).Equals("V"))
                nomeImg = "verde.png";
            else
                nomeImg = "rosso.png";


            img = "../Images/Icons/" + nomeImg;

            return img;
        }

        private string GetImageStatoRF(string chaDisabled)
        {
            string retValue = string.Empty;

            switch (chaDisabled)
            {
                case "1":
                    retValue = "rosso.png"; //"disabled";
                    break;

                default:
                    retValue = "verde.png"; //"abled";
                    break;
            }

            retValue = "../Images/Icons/" + retValue;

            return retValue;
        }

        protected string GetLabelRegistarState(Registro reg)
        {
            if (reg.chaRF == "0")
            {
                string state = string.Empty;
                if (UserManager.getStatoRegistro(reg).Equals("V"))
                    return open;
                else if (UserManager.getStatoRegistro(reg).Equals("G"))
                    return yellow;
                else
                    return close;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Aggiunge spazio nella prima colonna della tabella nel caso in cui il registro è un RF
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        protected string GetSpaceRF(Registro reg)
        {
            if (reg.chaRF == "1")
                return "&nbsp;&nbsp";
            else
                return "";
        }

        protected List<CasellaRegistro> GetEmailsRegister(string systemIdReg, out List<string> listFormatEmails)
        {
            List<CasellaRegistro> listEmails = MultiBoxManager.GetComboRegisterConsult(systemIdReg);
            List<string> listFormatE = new List<string>();
            foreach (CasellaRegistro c in listEmails)
            {
                if (!string.IsNullOrEmpty(c.EmailRegistro) && !c.EmailRegistro.Equals("&nbsp;"))
                {
                    System.Text.StringBuilder formatMail = new System.Text.StringBuilder();
                    if (c.Principale.Equals("1"))
                        formatMail.Append("* ");
                    formatMail.Append(c.EmailRegistro);
                    if (!string.IsNullOrEmpty(c.Note))
                    {
                        formatMail.Append(" - ");
                        formatMail.Append(c.Note);
                    }
                    listFormatE.Add(formatMail.ToString());
                }
            }
            listFormatEmails = listFormatE;
            return listEmails;
        }

        #endregion

        #region Event Handler
        protected void RegistersBtnChangesState_Click(object sender, EventArgs e)
        {
            try {
                Registro reg = null;
                if (SelectedRegister == null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningSelectedRegister', 'warning', '');", true);
                    return;
                }

                if (SelectedRegister != null && SelectedRegister.Sospeso)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningRegisterSuspended', 'warning', '');", true);
                    return;
                }

                reg = GestManager.cambiaStatoRegistro(SelectedRegister);
                if (reg == null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('ErrorChangeStateRegister', 'error', '');", true);
                    return;
                }
                if (!reg.stato.Equals("C"))
                    reg.ultimoNumeroProtocollo = string.Empty;
                SelectedRegister = reg;
                UpdateListRegisters();
                UpdateSelectedRecord();
                GrdRegisters_Bind();
                ButtonsManager();
                UpdateStateMailbox(ALL_MAILBOX);
                this.panelRegisters.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void RegistersBtnBox_Click(object sender, EventArgs e)
        {
            try {
                if (SelectedRegister != null)
                {
                    if (SelectedRegister.Sospeso)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningRegisterSuspendedBox', 'warning', '');} else {parent.ajaxDialogModal('WarningRegisterSuspendedBox', 'warning', '');}", true);
                        return;
                    }
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('ConfirmVerifyBox', 'HiddenVerifyBox', '');} else{parent.fra_main.ajaxConfirmModal('ConfirmVerifyBox', 'HiddenVerifyBox', '');}", true);
                return;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void RegistersBtnUpdateStateBox_Click(object sender, EventArgs e)
        {
            try {
                UpdateStateMailbox(ALL_MAILBOX);
                this.panelRegisters.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void RegistersBtnPrint_Click(object sender, EventArgs e)
        {
            DocsPaWR.InfoUtente infoUser = UserManager.GetInfoUser();
            DocsPaWR.InfoDocumento infoDoc = new InfoDocumento();
            DocsPaWR.Ruolo role = RoleManager.GetRoleInSession();
            try
            {
                DocsPaWR.StampaRegistroResult StpRegRS = GestManager.StampaRegistro(this, infoUser, role, SelectedRegister);
                if (StpRegRS != null && StpRegRS.errore != null && StpRegRS.errore != "")
                {
                    string error = StpRegRS.errore;
                    error = error.Replace("'", "\\'");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningRegisterPrint', 'warning', '','" + error + "');} else {parent.ajaxDialogModa('ErrorRegisterPrint', 'error', '','" + error + "');}", true);
                    return;
                }
                else
                {
                    infoDoc.docNumber = StpRegRS.docNumber;
                    DocsPaWR.SchedaDocumento schedaDoc = new SchedaDocumento();
                    schedaDoc = DocumentManager.getDocumentDetails(this, infoDoc.idProfile, infoDoc.docNumber);
                    FileDocPrintRegister = FileManager.getInstance(schedaDoc.systemId).GetFile(this.Page, schedaDoc.documenti[0], false);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "viewPrintRegister", "ajaxModalPopupViewPrintRegister();", true);
                    return;
                }

            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorRegisterPrint', 'error', '');} else {parent.ajaxDialogModa('ErrorRegisterPrint', 'error', '');}", true);
            }
        }

        protected void RegistersBtnRegisterModify_Click(object sender, EventArgs e)
        {
            try
            {
                string email = (this.GrdRegisters.SelectedRow.FindControl("DdlEmailRegister") as DropDownList).SelectedValue;
                List<DocsPaWR.InfoCheckMailbox> listCheckMailbox = CheckMailboxManager.InfoCheckMailbox(new List<string>() { email });
                if ((from checkMailBox in listCheckMailbox where checkMailBox.Concluded.Equals("0") select checkMailBox).Count() > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningRegisterModifyPass', 'warning', '');} else {parent.ajaxDialogModal('WarningRegisterModifyPass', 'warning', '');}", true);
                    return;
                }
                else
                {
                    SelectedRegister.email = email;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "viewPrintRegister", "ajaxModalPopupRegisterModify();", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        //Aggiorna lo stato del registro nella scheda del documento in sessione
        private void UpdateSelectedRecord()
        {
            SchedaDocumento sch = DocumentManager.getSelectedRecord();
            if (sch != null && sch.registro!=null)
            {
                Registro changedStateReg = SelectedRegister;

                if (sch.registro.codRegistro.Equals(changedStateReg.codRegistro))
                    sch.registro = changedStateReg;
            }
        }


        protected void ddlEmailRegister_SelectedIndexChanged(Object sender, EventArgs e)
        {
            try {
                this.GrdRegisters.SelectedIndex = int.Parse(this.grid_rowindex.Value);
                HighlightSelectedRow();
                SelectedRegister = ListRegisters[this.GrdRegisters.SelectedIndex + this.GrdRegisters.PageIndex * this.GrdRegisters.PageSize];
                ButtonsManager();
                if(this.RegistersBtnBox.Visible)
                    UpdateStateMailbox(SELECTED_MAILBOX);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void Timer_Tick(object sender, EventArgs e)
        {
            UpdateStateMailbox(ALL_MAILBOX);
            this.upPnlButtons.Update();
        }

        #endregion

        #region Check Mailbox Management

        private void CheckIstitutionalMailbox()
        {
            string email = (this.GrdRegisters.SelectedRow.FindControl("DdlEmailRegister") as DropDownList).SelectedValue;
            SelectedRegister.email = email; ;
            CheckMailboxManager.CheckMailBoxDelegate checkMailBox = new CheckMailboxManager.CheckMailBoxDelegate(CheckMailboxManager.CheckMailBox);
            checkMailBox.BeginInvoke(Utils.utils.getHttpFullPath(), SelectedRegister, UserManager.GetUserInSession(), RoleManager.GetRoleInSession(), null, null);
            this.GrdRegisters.SelectedRow.FindControl("containerVerifyEmail").Visible = true;
            this.GrdRegisters.SelectedRow.FindControl("containerProgressBar").Visible = true;
            this.GrdRegisters.SelectedRow.FindControl("progressBarWait").Visible = false;
            this.GrdRegisters.SelectedRow.FindControl("progressBarInfoMailProcessed").Visible = false;
            this.GrdRegisters.SelectedRow.FindControl("viewLinkReport").Visible = false;
            Panel pnlProgressBarInternal = this.GrdRegisters.SelectedRow.FindControl("progressBarInternal") as Panel;
            pnlProgressBarInternal.Width = new Unit(0);
            (this.GrdRegisters.SelectedRow.FindControl("checkMailbox") as HiddenField).Value = "true";
            //if(!TimerUpdateMailbox.Enabled)
            //     this.TimerUpdateMailbox.Enabled = true;
            this.RegistersBtnUpdateStateBox.Enabled = true;
            this.RegistersBtnBox.Enabled = false;
            this.upPnlButtons.Update();
            this.panelRegisters.Update();
        }

        /// <summary>
        /// Aggiorna lo stato di avanzamento per la casella
        /// </summary>
        private void UpdateStateMailbox(string typeUpdate)
        {
            //Tre possibili aggiornamenti:
            //   -da bottone(si aggiornano tutte le caselle, attualmente selezionate, per tutti i registri)
            //   -selezionado il registro(si va ad aggiornare la casella selezionata nel momento in cui si seleziona un registro)
            //   -selezionando una casella nel dropDownList(si va ad aggiornare lo stato per la casella appena selezionata)
            List<string> listEmails = new List<string>();
            List<DocsPaWR.InfoCheckMailbox> listCheckMailbox;
            string mail = string.Empty;
            switch (typeUpdate)
            {
                case "allMailbox":
                    foreach (GridViewRow grd in this.GrdRegisters.Rows)
                    {
                        if ((grd.FindControl("DdlEmailRegister") as DropDownList).Enabled)
                        {
                            grd.FindControl("containerVerifyEmail").Visible = false;
                            mail = (grd.FindControl("DdlEmailRegister") as DropDownList).SelectedValue;
                            listEmails.Add(mail);
                        }
                    }
                    listCheckMailbox = CheckMailboxManager.InfoCheckMailbox(listEmails);
                    if ((from checkMailBox in listCheckMailbox where checkMailBox.UserID.Equals(UserManager.GetUserInSession().systemId) && checkMailBox.RoleID.Equals(RoleManager.GetRoleInSession().systemId) select checkMailBox).Count() > 0)
                    {
                        foreach (GridViewRow grd in this.GrdRegisters.Rows)
                        {
                            
                            if ((grd.FindControl("DdlEmailRegister") as DropDownList).Enabled)
                            {
                                UpdateStateMailBoxManager(grd, listCheckMailbox);
                            }
                        }
                    }

                    break;
                case "selectedMailbox":
                    {
                        mail = (this.GrdRegisters.SelectedRow.FindControl("DdlEmailRegister") as DropDownList).SelectedValue;
                        listEmails.Add(mail);

                        listCheckMailbox = CheckMailboxManager.InfoCheckMailbox(listEmails);

                        if ((from checkMailBox in listCheckMailbox where checkMailBox.UserID.Equals(UserManager.GetUserInSession().systemId) && checkMailBox.RoleID.Equals(RoleManager.GetRoleInSession().systemId) select checkMailBox).Count() > 0)
                        {
                            UpdateStateMailBoxManager(this.GrdRegisters.SelectedRow, listCheckMailbox);
                        }
                        else
                        {
                            this.GrdRegisters.SelectedRow.FindControl("containerVerifyEmail").Visible = false;
                        }
                         
                    }
                    break;
            }
            foreach (GridViewRow grd in this.GrdRegisters.Rows)
            {
                if (string.IsNullOrEmpty((grd.FindControl("checkMailbox") as HiddenField).Value))
                {
                    this.RegistersBtnUpdateStateBox.Enabled = false;
                }
                else
                {
                    this.RegistersBtnUpdateStateBox.Enabled = true;
                    break;
                }
            }

            //if (!this.RegistersBtnUpdateStateBox.Enabled)
            //    this.TimerUpdateMailbox.Enabled = false;
            //else
            //    if(!this.TimerUpdateMailbox.Enabled)
            //        this.TimerUpdateMailbox.Enabled = true;
        }

        /// <summary>
        /// Aggiorna lo stato di avanzamento della casella selezionata corrispondente ad una determinata riga della griglia
        /// </summary>
        private void UpdateStateMailBoxManager(GridViewRow grd, List<DocsPaWR.InfoCheckMailbox> listCheckMailbox)
        {
            string regLabel = (grd.FindControl("lblCodiceRegistro") as Label).Text;
            Registro reg = (from regR in ListRegisters where regR.codRegistro.Equals(regLabel) select regR).FirstOrDefault();

            string mail = (grd.FindControl("DdlEmailRegister") as DropDownList).SelectedValue;

            //Per ogni registro Il risultato sarà un solo record perchè per ogni registro analizzo al più una sola casella(cioè quella selezionata)
            InfoCheckMailbox infoCheckMailbox = (from checkMailBox in listCheckMailbox
                                                 where checkMailBox.UserID.Equals(UserManager.GetUserInSession().systemId)
                                                 && checkMailBox.RoleID.Equals(RoleManager.GetRoleInSession().systemId)
                                                 && checkMailBox.Mail.Equals(mail)
                                                 && checkMailBox.RegisterID.Equals(reg.systemId)
                                                 select checkMailBox).FirstOrDefault();
            //se infoCheckMailbox è nullo
            if (grd == this.GrdRegisters.SelectedRow)
                this.RegistersBtnBox.Enabled = true;
            //se infoCheckMailbox è nullo ne segue che non c'è nessuno scarico per l'utente
            if (infoCheckMailbox != null)
            {
                grd.FindControl("containerVerifyEmail").Visible = true;
                if (grd == this.GrdRegisters.SelectedRow)
                    this.RegistersBtnBox.Enabled = false;
                //se lo scarico è completato
                if (infoCheckMailbox.Concluded == "1")
                {
                    //mostro link report
                    grd.FindControl("viewLinkReport").Visible = true;
                    (grd.FindControl("idCheckMailbox") as HiddenField).Value = infoCheckMailbox.IdCheckMailbox;
                    grd.FindControl("progressBarWait").Visible = false;
                    grd.FindControl("containerProgressBar").Visible = false;
                    grd.FindControl("progressBarInfoMailProcessed").Visible = false;
                    (grd.FindControl("checkMailbox") as HiddenField).Value = "";
                }
                else
                {
                    (grd.FindControl("checkMailbox") as HiddenField).Value = "true";
                     grd.FindControl("viewLinkReport").Visible = false;
                     grd.FindControl("containerProgressBar").Visible = true;
                     Panel pnlProgressBarInternal = grd.FindControl("progressBarInternal") as Panel;
                     Panel pnlProgressBar = grd.FindControl("progressBar") as Panel;
                    //se il totale è diverso da 0, è in atto lo scarico della casella
                    if (infoCheckMailbox.Total != 0)
                    {
                        grd.FindControl("progressBarInfoMailProcessed").Visible = true;
                        grd.FindControl("progressBarWait").Visible = false;
                        Label lblNumerMailProcessed = grd.FindControl("lblNumberMailProcessed") as Label;
                        lblNumerMailProcessed.Text = Utils.Languages.GetLabelFromCode("RegistersLblNumerMailProcessed", UIManager.UserManager.GetUserLanguage()) + " " + infoCheckMailbox.Elaborate + "/" + infoCheckMailbox.Total;
                        Label lblNumberMailProcessedDate = grd.FindControl("lblNumberMailProcessedDate") as Label;
                        lblNumberMailProcessedDate.Text = DateTime.Now.ToString();
                        double widthForMail = Convert.ToDouble(pnlProgressBar.Width.Value) / infoCheckMailbox.Total;
                        pnlProgressBarInternal.Width = new Unit(widthForMail * infoCheckMailbox.Elaborate);
                    }
                    else
                    {
                        grd.FindControl("progressBarInfoMailProcessed").Visible = false;
                        grd.FindControl("progressBarWait").Visible = true;
                        pnlProgressBarInternal.Width = new Unit(0);
                        //mostro il messaggio di attesa
                        //totale potrebbe essere uguale a zero per due motivi:
                        //- se l'utente stà scaricando la stessa casella da un altro registro/rf
                        //- se la casella è attualmente in scarico da un'altro utente

                        if ((from info in listCheckMailbox
                                where info.UserID.Equals(UserManager.GetUserInSession().systemId)
                                && info.RoleID.Equals(RoleManager.GetRoleInSession().systemId)
                                && info.Total != 0 && info.Concluded == "0"
                                select info).Count() > 0)
                        {
                            //Verifico se l'utente sta effettuando lo scarico della stessa casella su di un registro differente
                            foreach( InfoCheckMailbox i in listCheckMailbox)
                            {
                                if ((from info in listCheckMailbox
                                     where info.UserID.Equals(UserManager.GetUserInSession().systemId)
                                     && info.RoleID.Equals(RoleManager.GetRoleInSession().systemId)
                                     && (!info.RegisterID.Equals(i.RegisterID))
                                     && info.Total != 0 && info.Concluded == "0" && i.Mail.Equals(info.Mail)
                                     select info).Count() > 0)
                                {
                                    (grd.FindControl("lblWait") as Label).Text = progressBarWaitCloseVerification;
                                    break;
                                }
                                else
                                    grd.FindControl("progressBarWait").Visible = false;
                            }
                        }
                            
                        else
                        {
                            if ((from info in listCheckMailbox
                                 where (!info.UserID.Equals(UserManager.GetUserInSession().systemId))
                                 || (!info.RoleID.Equals(RoleManager.GetRoleInSession().systemId))
                                 && info.Total != 0 && info.Concluded == "0"
                                 select info).Count() > 0)
                            {
                                (grd.FindControl("lblWait") as Label).Text = progressBarWait;
                            }
                            else
                            {
                                grd.FindControl("progressBarWait").Visible = false;
                            }
                        }
                    }

                }
            }
            else
            {
                grd.FindControl("containerVerifyEmail").Visible = false;
            }
        }

        #endregion
    }
}