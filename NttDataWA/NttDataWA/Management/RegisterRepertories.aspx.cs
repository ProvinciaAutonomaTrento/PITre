using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Management
{
    public partial class RegisterRepertories : System.Web.UI.Page
    {
        #region const
        private const string PANEL_REGISTER_REPERTORIES = "panelRegistersRepertories";
        #endregion

        #region Property

        /// <summary>
        /// Memorizza la lista dei reperotri
        /// </summary>
        private RegistroRepertorio[] ListRegRepertories
        { 
            get
            {
                if (HttpContext.Current.Session["listRegRepertories"] != null)
                    return (RegistroRepertorio[])HttpContext.Current.Session["listRegRepertories"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["listRegRepertories"] = value;
            }
        }


        /// <summary>
        /// Memorizza la lista dei reperotri
        /// </summary>
        private ColsRegisterRepertories[] ListRegRepertoriesSingleSetting
        {
            get
            {
                if (HttpContext.Current.Session["listRegRepertoriesSingleSettings"] != null)
                    return (ColsRegisterRepertories[])HttpContext.Current.Session["listRegRepertoriesSingleSettings"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["listRegRepertoriesSingleSettings"] = value;
            }
        }

        #endregion

        #region RemoveProperty

        private void RemoveListRegRepertories()
        {
            HttpContext.Current.Session.Remove("listRegRepertories");
        }

        #endregion

        #region Standard Method
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    InitializeLanguage();
                    InitializePage();
                    GrdRegisterRepertories_Bind();
                }
                else
                {
                    this.ReadRetValueFromPopup();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Language Manager
        /// </summary>
        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.RegisterRepertorieslbl.Text = Utils.Languages.GetLabelFromCode("RegisterRepertorieslbl", language);
            this.RegisterRepertoriesBtnPrint.Text = Utils.Languages.GetLabelFromCode("RegisterRepertoriesBtnPrint", language);
            this.RegisterRepertoriesBtnChangesState.Text = Utils.Languages.GetLabelFromCode("RegisterRepertoriesBtnChangesState", language);
            this.RegisterRepertoriesPrintTxt.InnerText = Utils.Languages.GetLabelFromCode("RegisterRepertoriesPrintTxt", language);

        }

        protected void InitializePage()
        {
            RemoveListRegRepertories();
        }

        protected void ReadRetValueFromPopup()
        {
            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(PANEL_REGISTER_REPERTORIES))
            {
                this.GrdRegisterRepertories.SelectedIndex = int.Parse(this.grid_rowindex.Value);
                HighlightSelectedRow();
                int index = GrdRegisterRepertories.SelectedIndex + this.GrdRegisterRepertories.PageIndex * this.GrdRegisterRepertories.PageSize;
                SetSourceForBullettedList(ListRegRepertoriesSingleSetting[index]);
                this.panelRegistersRepertories.Update();
            }
        }

        #endregion

        #region Event Handler
        protected void RegisterRepertoriesBtnPrint_Click(object sender, EventArgs e)
        {
            try {
                int index = GrdRegisterRepertories.SelectedIndex + this.GrdRegisterRepertories.PageIndex * this.GrdRegisterRepertories.PageSize;
                if (ListRegRepertoriesSingleSetting[index].RegistroRepertorioSingleSettings.CounterState.ToString().Equals("O"))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningPrintRegisterRepertories', 'warning', '');} else {parent.ajaxDialogModal('WarningPrintRegisterRepertories', 'warning', '');}", true);
                    return;
                }
                try
                {
                    SchedaDocumento sch = UIManager.RegisterRepertories.GeneratePrintRepertorio(RoleManager.GetRoleInSession(),
                        UserManager.GetInfoUser(),
                        ListRegRepertoriesSingleSetting[index].RegistroRepertorioSingleSettings.RFId,
                        ListRegRepertoriesSingleSetting[index].RegistroRepertorioSingleSettings.RegistryId,
                        ListRegRepertoriesSingleSetting[index].RegistroRepertorio.CounterId);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('SuccessPrintRegisterRepertories', 'check', '');} else {parent.ajaxDialogModal('SuccessPrintRegisterRepertories', 'check', '');}", true);
                    return;
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorPrintRegisterRepertories', 'error', '', '" + Utils.utils.FormatJs(ex.Message) + "');} else {parent.ajaxDialogModal('ErrorPrintRegisterRepertories', 'error', '', '" + Utils.utils.FormatJs(ex.Message) + "');}", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void RegisterRepertoriesBtnChangesState_Click(object sender, EventArgs e)
        {
            try
            {
                int index = GrdRegisterRepertories.SelectedIndex + this.GrdRegisterRepertories.PageIndex * this.GrdRegisterRepertories.PageSize;
                bool res = UIManager.RegisterRepertories.ChangeRepertorioState(ListRegRepertoriesSingleSetting[index].RegistroRepertorio.CounterId,
                    ListRegRepertoriesSingleSetting[index].RegistroRepertorioSingleSettings.RegistryId,
                    ListRegRepertoriesSingleSetting[index].RegistroRepertorioSingleSettings.RFId,
                    AdministrationManager.AmmGetInfoAmmCorrente(UserManager.GetInfoUser().idAmministrazione).Codice);
                if (res)
                {
                    //Aggiorno lo stato a tutti i registri aventi lo stesso CounterId di quello selezionato
                    RepertorioState newState = ListRegRepertoriesSingleSetting[index].RegistroRepertorioSingleSettings.CounterState.ToString().Equals("O") ? RepertorioState.C : RepertorioState.O;
                    for (int i = 0; i < ListRegRepertoriesSingleSetting.Length; i++ )
                    {
                        if (ListRegRepertoriesSingleSetting[i].RegistroRepertorio.CounterId.Equals(ListRegRepertoriesSingleSetting[index].RegistroRepertorio.CounterId))
                            ListRegRepertoriesSingleSetting[i].RegistroRepertorioSingleSettings.CounterState = newState;
                    }
                    GrdRegisterRepertories_Bind();
                    this.panelRegistersRepertories.Update();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorChangesStateRegisterRepertories', 'error', '');} else {parent.ajaxDialogModal('ErrorChangesStateRegisterRepertories', 'error', '');}", true);
                    return;
                }
            }
            catch (Exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorChangesStateRegisterRepertories', 'error', '');} else {parent.ajaxDialogModal('ErrorChangesStateRegisterRepertories', 'error', '');}", true);
                return;
            }
             
        }
        #endregion

        #region Grid manager
        private void GrdRegisterRepertories_Bind()
        {
            ArrayList list;
            if (ListRegRepertories == null)
            {
                list = new ArrayList();
                ListRegRepertories = NttDataWA.UIManager.RegisterRepertories.GetRegistriesWithAooOrRf(null, UserManager.GetInfoUser().idGruppo);
                if (ListRegRepertories != null && ListRegRepertories.Length > 0)
                {
                    for (int i = 0; i < ListRegRepertories.Length; i++)
                    {
                        foreach (RegistroRepertorioSingleSettings regRepSS in ListRegRepertories[i].SingleSettings)
                        {
                            ColsRegisterRepertories c = new ColsRegisterRepertories(ListRegRepertories[i], regRepSS);
                            list.Add(c);
                        }
                    }
                    ListRegRepertoriesSingleSetting = list.Cast<ColsRegisterRepertories>().ToArray();
                }
            }
            if (ListRegRepertories != null && ListRegRepertories.Length > 0)
            {
                this.GrdRegisterRepertories.DataSource = ListRegRepertoriesSingleSetting;
                this.GrdRegisterRepertories.DataBind();
                if (string.IsNullOrEmpty(this.grid_rowindex.Value))
                    this.grid_rowindex.Value = "0";
                this.GrdRegisterRepertories.SelectedIndex = int.Parse(this.grid_rowindex.Value);
                HighlightSelectedRow();
                int index = GrdRegisterRepertories.SelectedIndex + this.GrdRegisterRepertories.PageIndex * this.GrdRegisterRepertories.PageSize;
                SetSourceForBullettedList(ListRegRepertoriesSingleSetting[index]);
            }
            else
            {
                this.divRegRepertoriesPrint.Visible = false;
                DisableButtons();
            }
        }


        public class ColsRegisterRepertories
        {
            RegistroRepertorio regRep;
            RegistroRepertorioSingleSettings regRepSS;
            public ColsRegisterRepertories(RegistroRepertorio regRep, RegistroRepertorioSingleSettings regRepSS)
            {
                this.regRep = regRep;
                this.regRepSS = regRepSS;
            }

            public RegistroRepertorio RegistroRepertorio { get { return regRep; } }
            public RegistroRepertorioSingleSettings RegistroRepertorioSingleSettings { get { return regRepSS; } }
        }



        /// <summary>
        /// 
        /// </summary>
        protected void HighlightSelectedRow()
        {

            if (this.GrdRegisterRepertories.Rows.Count > 0 && this.GrdRegisterRepertories.SelectedRow != null)
            {
                GridViewRow gvRow = this.GrdRegisterRepertories.SelectedRow;
                foreach (GridViewRow GVR in this.GrdRegisterRepertories.Rows)
                {
                    if (GVR == gvRow)
                    {
                        GVR.CssClass += " selectedrow";
                    }
                    else
                    {
                        GVR.CssClass = GVR.CssClass.Replace(" selectedrow", "");
                    }
                }
            }
        }

        protected void GrdRegisterRepertories_RowDataBound(object sender, GridViewRowEventArgs e)
        {        
            try {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onClick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "');__doPostBack('panelRegistersRepertories');return false;";
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GrdRegisterRepertories_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try {
                this.GrdRegisterRepertories.PageIndex = e.NewPageIndex;
                this.grid_rowindex.Value = "0";
                GrdRegisterRepertories_Bind();
                this.panelRegistersRepertories.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected string GetLabelDateLastPrint(ColsRegisterRepertories reg)
        {
            if (reg.RegistroRepertorioSingleSettings.DateLastPrint.ToShortDateString().Equals("01/01/0001"))
                return "";
            else
                return reg.RegistroRepertorioSingleSettings.DateLastPrint.ToShortDateString().ToString();
        }

        protected string GetLabelState(ColsRegisterRepertories reg)
        {
            return reg.RegistroRepertorioSingleSettings.CounterState.ToString().Equals("O") ? "Aperto" : "Chiuso";
        }

        protected string GetImageState(ColsRegisterRepertories reg)
        {
            string retValue = string.Empty;

            switch (reg.RegistroRepertorioSingleSettings.CounterState.ToString())
            {
                case "O":
                    retValue = "verde.png"; 
                    break;

                case "C":
                    retValue = "rosso.png";
                    break;
            }

            retValue = "../Images/Icons/" + retValue;

            return retValue;
        }

        private void SetSourceForBullettedList(ColsRegisterRepertories colsReg)
        {
             // Se ci sono repertori degli anni passati da stampare, viene visualizzato l'avviso
             RepertorioPrintRange[] ranges = UIManager.RegisterRepertories.GetRepertoriPrintRanges(colsReg.RegistroRepertorio.CounterId, colsReg.RegistroRepertorioSingleSettings.RegistryId, colsReg.RegistroRepertorioSingleSettings.RFId);
             if (ranges != null && ranges.Length > 0)
             {
                 System.Collections.Generic.List<String> docs = new System.Collections.Generic.List<string>();
                 foreach (var r in ranges)
                     docs.Add(String.Format(Utils.Languages.GetLabelFromCode("RegisterRepertoriesPrint", UIManager.UserManager.GetUserLanguage()),
                       r.FirstNumber, r.LastNumber, r.Year));
                 this.blDocList.DataSource = docs;
                 this.blDocList.DataBind();
                 this.divRegRepertoriesPrint.Visible = true;
             }
             else
                 this.divRegRepertoriesPrint.Visible = false;
        }

        #endregion

        #region Buttons
        private void DisableButtons()
        {
            this.RegisterRepertoriesBtnChangesState.Enabled = false;
            this.RegisterRepertoriesBtnPrint.Enabled = false;
        }
        #endregion
    }


}