using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class DisposalReport : System.Web.UI.Page
    {
        #region field

        protected Int32 Disposal_ID
        {
            get
            {
                Int32 result = 0;
                if (HttpContext.Current.Session["CurrentDisposal_ID"] != null)
                    result = int.Parse(HttpContext.Current.Session["CurrentDisposal_ID"].ToString());
                return result;
            }
            set
            {
                HttpContext.Current.Session["CurrentDisposal_ID"] = value;
            }
        }

        protected List<ARCHIVE_Disposal> DisposalOriginalInContext
        {
            get
            {
                List<ARCHIVE_Disposal> result = null;
                if (HttpContext.Current.Session["DisposalOriginalInContext"] != null)
                {
                    result = HttpContext.Current.Session["DisposalOriginalInContext"] as List<ARCHIVE_Disposal>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["DisposalOriginalInContext"] = value;
            }
        }

        protected List<ARCHIVE_DisposalState> DisposalStateInContext
        {
            get
            {
                List<ARCHIVE_DisposalState> result = null;
                if (HttpContext.Current.Session["DisposalStateInContext"] != null)
                {
                    result = HttpContext.Current.Session["DisposalStateInContext"] as List<ARCHIVE_DisposalState>;
                }
                return result;
            }

        }

        protected List<Int32> DisposalPolicyListIdInContext
        {
            get
            {
                List<Int32> result = null;
                if (HttpContext.Current.Session["DisposalPolicyListId"] != null)
                {
                    result = HttpContext.Current.Session["DisposalPolicyListId"] as List<Int32>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["DisposalPolicyListId"] = value;
            }
        }

        protected List<ARCHIVE_View_FascReportDisposal> gridFascSource
        {
            get
            {
                List<ARCHIVE_View_FascReportDisposal> result = null;
                if (HttpContext.Current.Session["gridFascSource"] != null)
                {
                    result = HttpContext.Current.Session["gridFascSource"] as List<ARCHIVE_View_FascReportDisposal>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["gridFascSource"] = value;
            }
        }

        protected List<ARCHIVE_View_FascReportDisposal> gridFascSourceOriginal
        {
            get
            {
                List<ARCHIVE_View_FascReportDisposal> result = null;
                if (HttpContext.Current.Session["gridFascSourceOriginal"] != null)
                {
                    result = HttpContext.Current.Session["gridFascSourceOriginal"] as List<ARCHIVE_View_FascReportDisposal>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["gridFascSourceOriginal"] = value;
            }
        }

        protected List<ARCHIVE_View_DocReportDisposal> gridDocSource
        {
            get
            {
                List<ARCHIVE_View_DocReportDisposal> result = null;
                if (HttpContext.Current.Session["gridDocSource"] != null)
                {
                    result = HttpContext.Current.Session["gridDocSource"] as List<ARCHIVE_View_DocReportDisposal>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["gridDocSource"] = value;
            }
        }

        protected List<ARCHIVE_View_DocReportDisposal> gridDocSourceOriginal
        {
            get
            {
                List<ARCHIVE_View_DocReportDisposal> result = null;
                if (HttpContext.Current.Session["gridDocSourceOriginal"] != null)
                {
                    result = HttpContext.Current.Session["gridDocSourceOriginal"] as List<ARCHIVE_View_DocReportDisposal>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["gridDocSourceOriginal"] = value;
            }
        }

        protected List<Int32> ListDocChecked
        {
            get
            {
                List<Int32> result = new List<Int32>();
                if (HttpContext.Current.Session["ListDocChecked"] != null)
                {
                    result = HttpContext.Current.Session["ListDocChecked"] as List<Int32>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ListDocChecked"] = value;
            }
        }

        protected bool CheckAllDoc
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["CheckAllDoc"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["CheckAllDoc"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["CheckAllDoc"] = value;
            }
        }

        protected List<Int32> ListFascChecked
        {
            get
            {
                List<Int32> result = new List<Int32>();
                if (HttpContext.Current.Session["ListFascChecked"] != null)
                {
                    result = HttpContext.Current.Session["ListFascChecked"] as List<Int32>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ListFascChecked"] = value;
            }
        }

        protected bool CheckAllFasc
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["CheckAllFasc"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["CheckAllFasc"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["CheckAllFasc"] = value;
            }
        }


        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();


            }

            this.RefreshScript();

        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeTableDocDisposal", "resizeTableDocDisposal();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeTableFascDisposal", "resizeTableFascDisposal();", true);
            ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(this.cImgBtnExportDocDisp);
            ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(this.cImgBtnExportFascDisp);
        }

        private void PopulatePageObject()
        {


            if (this.Disposal_ID > 0 && this.DisposalOriginalInContext.Count > 0 && this.DisposalOriginalInContext != null)
            {
                this.txtIdScarto.Text = Disposal_ID.ToString();
                this.TxtDescrScarto.Text = this.DisposalOriginalInContext.FirstOrDefault().Description;
            }

        }

        private void UpdateData()
        {

        }

        private void GetData()
        {
            try
            {
                this.Disposal_ID = this.DisposalOriginalInContext.FirstOrDefault().System_ID;
                this.txtIdScarto.Text = this.Disposal_ID.ToString();
                this.TxtDescrScarto.Text = this.DisposalOriginalInContext.FirstOrDefault().Description;

                //FASCICOLI
                List<ARCHIVE_View_FascReportDisposal> listaFasc = UIManager.ArchiveManager.GetARCHIVE_View_FascReportByDisposal_ID(this.Disposal_ID);
                //List<ARCHIVE_View_FascReportDisposal> listaFasc = new List<ARCHIVE_View_FascReportDisposal>();
                this.gridFascSourceOriginal = UIManager.ArchiveManager.GetARCHIVE_View_FascReportByDisposal_ID(this.Disposal_ID);

                if (listaFasc == null)
                    listaFasc = new List<ARCHIVE_View_FascReportDisposal>();

                this.ListFascChecked = listaFasc.Where(x => x.ToDisposal == 1).Select(x => x.Project_ID).ToList();
                //li elabero per presentarli meglio
                while (listaFasc.Count < 9 || listaFasc.Count % 9 != 0)
                    listaFasc.Add(new ARCHIVE_View_FascReportDisposal());
                //datasourcefinale
                this.gridFascSource = listaFasc;

                //sono tutti checkati se la il numero dei checkati è uguale al numero totale
                this.CheckAllFasc = this.gridFascSourceOriginal.Count == this.ListFascChecked.Count
                                                                        && this.gridFascSourceOriginal.Count > 0;


                //DOCUMENTI
                List<ARCHIVE_View_DocReportDisposal> listaDoc = UIManager.ArchiveManager.GetARCHIVE_View_DocReportByDisposal_ID(this.Disposal_ID);
                //List<ARCHIVE_View_DocReportDisposal> listaDoc = new List<ARCHIVE_View_DocReportDisposal>();
                this.gridDocSourceOriginal = UIManager.ArchiveManager.GetARCHIVE_View_DocReportByDisposal_ID(this.Disposal_ID);
                if (listaDoc == null)
                    listaDoc = new List<ARCHIVE_View_DocReportDisposal>();

                this.ListDocChecked = listaDoc.Where(x => x.ToDisposal == 1).Select(x => x.Profile_ID).ToList();
                //li elabero per presentarli meglio
                var righe = (from d in listaDoc
                             select new ARCHIVE_View_DocReportDisposal
                             {
                                 ToDisposal = d.ToDisposal,
                                 Registro = d.Registro,
                                 UO = d.UO,
                                 Profile_ID = d.Profile_ID,
                                 Proto = d.Proto,
                                 ProtoDate = d.ProtoDate,
                                 CreateDate = d.CreateDate,
                                 Oggetto = d.Oggetto,
                                 Tipo = d.Tipo,
                                 Tipologia = d.Tipologia,
                                 TipoTransfer = d.TipoTransfer,
                                 MittDest = d.MittDest.Replace(";", "<br />"),
                                 ProjectCode = d.ProjectCode.Replace(";", "<br />")

                             }).ToList();

                //se ho la griglia vuota o con un numero di righe che non è multiplo di 9 aggiungo righe vuote
                while (righe.Count < 9 || righe.Count % 9 != 0)
                    righe.Add(new ARCHIVE_View_DocReportDisposal());
                //datasourcefinale
                this.gridDocSource = righe;

                //sono tutti checkati se la il numero dei checkati è uguale al numero totale
                this.CheckAllDoc = this.gridDocSourceOriginal.Count == this.ListDocChecked.Count
                                                                        && this.gridDocSourceOriginal.Count > 0;

                this.LitNumFasc.Text = gridFascSourceOriginal.Count.ToString();
                this.LitNumDoc.Text = gridDocSourceOriginal.Count.ToString();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }

        private void resetData()
        {
            this.ListFascChecked = this.gridFascSourceOriginal.Where(x => x.ToDisposal == 1).Select(x => x.Project_ID).ToList();
            this.ListDocChecked = this.gridDocSourceOriginal.Where(x => x.ToDisposal == 1).Select(x => x.Profile_ID).ToList();

            //sono tutti checkati se la il numero dei checkati è uguale al numero totale
            this.CheckAllDoc = this.gridDocSourceOriginal.Count == this.ListDocChecked.Count;
            this.CheckAllFasc = this.gridFascSourceOriginal.Count == this.ListFascChecked.Count;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);

            this.LoadDataInGrid();

        }

        private void UpdatePage()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            this.GetData();
            this.LoadDataInGrid();
        }

        public bool GetEnabledByDisposalState()
        {
            if (this.DisposalStateInContext != null)
            {
                switch (this.DisposalStateInContext.Max(p => p.DisposalStateType_ID))
                {
                    //IN DEFINIZIONE
                    case 1:
                        return false;
                    //RICERCA COMPLETATA
                    case 2:
                        return true;
                    //PROPOSTO
                    case 3:
                        return true;
                    //APPROVATO
                    case 4:
                        return false;
                    //IN ESECUZIONE
                    case 5:
                        return false;
                    //EFFETTUATO
                    case 6:
                        return false;
                    //ERRORE
                    case 7:
                        return false;
                }
            }
            return false;
        }


        private string convertListInString(List<Int32> list)
        {
            int brk = 0;
            string sqlIn = string.Empty;

            foreach (Int32 id in list)
            {
                if (brk == 0)
                {
                    sqlIn = "" + id.ToString();
                    brk++;
                }
                else
                    sqlIn += "," + id.ToString();
            }
            sqlIn += "";

            return sqlIn;
        }

        public List<ARCHIVE_View_DocReportDisposal> GetDataSourceDummyGrdDoc()
        {
            List<ARCHIVE_View_DocReportDisposal> dummy = new List<ARCHIVE_View_DocReportDisposal>();
            ARCHIVE_View_DocReportDisposal element = new ARCHIVE_View_DocReportDisposal();

            dummy.Add(element);
            return dummy;
        }

        public List<ARCHIVE_View_FascReportDisposal> GetDataSourceDummyGrdFasc()
        {
            List<ARCHIVE_View_FascReportDisposal> dummy = new List<ARCHIVE_View_FascReportDisposal>();
            ARCHIVE_View_FascReportDisposal element = new ARCHIVE_View_FascReportDisposal();

            dummy.Add(element);
            return dummy;
        }

        private void InitializePage()
        {
            this.InitializeLanguage();
            this.GetData();
            this.VisibilityByDisposalState();
            this.PopulatePageObject();
            this.LoadDataInGrid();

        }

        private void VisibilityByDisposalState()
        {
            this.btnAggiorna.Enabled = this.GetEnabledByDisposalState();
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.litIdScarto.Text = Utils.Languages.GetLabelFromCode("LitScartoId", language);
            this.litDescrScarto.Text = Utils.Languages.GetLabelFromCode("LitScartoDescrizione", language);
            this.DocTabLinkList.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblDocument", language);
            this.FascTabLinkList.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchProject", language);
            this.btnClose.Text = Utils.Languages.GetLabelFromCode("AddDocInProjectBtnClose", language);
            this.cImgBtnExportDocDisp.ToolTip = Utils.Languages.GetLabelFromCode("cImgBtnReportDocFasc", language);
            this.cImgBtnExportFascDisp.ToolTip = Utils.Languages.GetLabelFromCode("cImgBtnReportDocFasc", language);
            this.btnAnnulla.Text = Utils.Languages.GetLabelFromCode("VisibilityBtnCancel", language);
            this.btnAggiorna.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnSaveTemplate", language);


        }

        protected void addAllDoc_Click(object sender, EventArgs e)
        {

            bool value = ((CheckBox)sender).Checked;
            this.ListDocChecked = new List<Int32>();
            //this.idPolicySelected = new List<Int32>();
            foreach (GridViewRow dgItem in GrdDocResultDisposal.Rows)
            {
                CheckBox checkBox = dgItem.FindControl("checkDocumento") as CheckBox;
                checkBox.Checked = value;
            }
            if (value)
                this.ListDocChecked.AddRange(this.gridDocSource.Where(x => x.Profile_ID > 0).Select(x => x.Profile_ID).ToList());
            this.CheckAllDoc = value;
            this.UpPnlGridDocResultDisp.Update();

        }

        protected void addAllFasc_Click(object sender, EventArgs e)
        {

            bool value = ((CheckBox)sender).Checked;
            this.ListFascChecked = new List<Int32>();
            //this.idPolicySelected = new List<Int32>();
            foreach (GridViewRow dgItem in GrdFascResultDisposal.Rows)
            {
                CheckBox checkBox = dgItem.FindControl("checkFascicolo") as CheckBox;
                checkBox.Checked = value;
            }
            if (value)
                this.ListFascChecked.AddRange(this.gridFascSource.Where(x => x.Project_ID > 0).Select(x => x.Project_ID).ToList());
            this.CheckAllFasc = value;
            this.UpPnlGridDocResultDisp.Update();

        }

        protected void checkDocumento_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = ((CheckBox)sender);
            bool value = cb.Checked;
            GridViewRow gridviewRow = (GridViewRow)cb.NamingContainer;

            //Get the rowindex
            int rowindex = gridviewRow.RowIndex;

            int idDoc = int.Parse(this.GrdDocResultDisposal.DataKeys[rowindex].Value.ToString());
            if (value)
            {
                this.ListDocChecked.Add(idDoc);
                this.gridDocSource.Where(x => x.Profile_ID == idDoc).FirstOrDefault().ToDisposal = 1;
            }
            else
            {
                this.ListDocChecked.Remove(idDoc);
                this.gridDocSource.Where(x => x.Profile_ID == idDoc).FirstOrDefault().ToDisposal = 0;
                this.CheckAllDoc = value;
            }
        }

        protected void checkFascicolo_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = ((CheckBox)sender);
            bool value = cb.Checked;
            GridViewRow gridviewRow = (GridViewRow)cb.NamingContainer;

            //Get the rowindex
            int rowindex = gridviewRow.RowIndex;

            int idFasc = int.Parse(this.GrdFascResultDisposal.DataKeys[rowindex].Value.ToString());
            if (value)
            {
                this.ListFascChecked.Add(idFasc);
                this.gridFascSource.Where(x => x.Project_ID == idFasc).FirstOrDefault().ToDisposal = 1;
            }
            else
            {
                this.ListFascChecked.Remove(idFasc);
                this.gridFascSource.Where(x => x.Project_ID == idFasc).FirstOrDefault().ToDisposal = 1;
                this.CheckAllFasc = value;
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.clearSessionProperties();
                this.closePage();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }

        private void clearSessionProperties()
        {
            this.gridFascSourceOriginal = null;
            this.ListFascChecked = null;
            this.gridFascSource = null;
            this.CheckAllFasc = false;
            this.gridDocSourceOriginal = null;
            this.ListDocChecked = null;
            this.gridDocSource = null;
            this.CheckAllDoc = false;
        }

        protected void btnAnnulla_Click(object sender, EventArgs e)
        {
            try
            {
                this.resetData();

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }

        protected void btnAggiorna_Click(object sender, EventArgs e)
        {
            try
            {
                List<Int32> list = (from id in this.gridDocSource.Select(x => x.Profile_ID)
                                    where id > 0 && !this.ListDocChecked.Contains(id)
                                    select id).ToList();
                string slistDoc = this.convertListInString(list);

                bool bDoc = UIManager.ArchiveManager.Update_ARCHIVE_TempProfileDisposal(this.Disposal_ID, slistDoc);

                list = (from id in this.gridFascSource.Select(x => x.Project_ID)
                        where id > 0 && !this.ListFascChecked.Contains(id)
                        select id).ToList();

                string slistFasc = this.convertListInString(list);

                bool bFasc = UIManager.ArchiveManager.Update_ARCHIVE_TempProjectDisposal(this.Disposal_ID, slistFasc);

                this.UpdatePage();

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }



        protected void AddressBookLinkList_Click(object sender, EventArgs e)
        {
            try
            {
                this.liReportDocumentiDisposal.Attributes.Remove("class");
                this.liReportDocumentiDisposal.Attributes.Add("class", "addressTab");
                this.liReportFascicoliDisposal.Attributes.Remove("class");
                this.liReportFascicoliDisposal.Attributes.Add("class", "otherAddressTab");
                this.UpTypeResultDisp.Update();

                this.showFascReport(false);
                this.UpdatePanelTop.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void AddressBookLinkOrg_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);


                this.liReportDocumentiDisposal.Attributes.Remove("class");
                this.liReportDocumentiDisposal.Attributes.Add("class", "otherAddressTab");
                this.liReportFascicoliDisposal.Attributes.Remove("class");
                this.liReportFascicoliDisposal.Attributes.Add("class", "addressTab");
                this.UpTypeResultDisp.Update();

                this.showFascReport(true);
                this.UpdatePanelTop.Update();

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void showFascReport(bool visible)
        {
            this.PnlFascReport.Visible = visible;
            this.UpPnlGridFascResultDisp.Update();

            this.PnlGridDocResult.Visible = !visible;
            this.UpPnlGridDocResultDisp.Update();
        }

        private void LoadDataInGrid()
        {
            if (this.gridDocSource != null && this.gridDocSource.Count > 0)
            {
                this.GrdDocResultDisposal.DataSource = this.gridDocSource;
                this.GrdDocResultDisposal.DataBind();
                this.UpPnlGridDocResultDisp.Update();
            }
            else
            {
                this.GrdDocResultDisposal.DataSource = this.GetDataSourceDummyGrdDoc();
                this.GrdDocResultDisposal.DataBind();
                this.UpPnlGridDocResultDisp.Update();
                //this.GrdDocResultDisposal.Rows[0].Visible = false;
            }


            if (this.gridFascSource != null && this.gridFascSource.Count > 0)
            {
                this.GrdFascResultDisposal.DataSource = this.gridFascSource;
                this.GrdFascResultDisposal.DataBind();
                this.UpPnlGridFascResultDisp.Update();
            }
            else
            {
                this.GrdFascResultDisposal.DataSource = this.GetDataSourceDummyGrdFasc();
                this.GrdFascResultDisposal.DataBind();
                this.UpPnlGridFascResultDisp.Update();
            }

        }

        protected void GrdFascResultDisposal_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                if (this.DisposalStateInContext != null)
                {
                    if (this.GetEnabledByDisposalState())
                        e.Row.Cells[0].Enabled = true;
                    else
                        e.Row.Cells[0].Enabled = false;
                }
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int idFasc = int.Parse(this.GrdFascResultDisposal.DataKeys[e.Row.RowIndex].Value.ToString());
                if (idFasc.ToString().Equals("0"))
                {
                    ((CheckBox)e.Row.FindControl("checkFascicolo")).Visible = false;
                    e.Row.Cells[3].Text = string.Empty;
                }

                if (this.ListFascChecked.Contains(idFasc))
                    ((CheckBox)e.Row.FindControl("checkFascicolo")).Checked = true;

            }
        }

        protected void GrdDocResultDisposal_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                if (this.DisposalStateInContext != null)
                {
                    if (this.GetEnabledByDisposalState())
                        e.Row.Cells[0].Enabled = true;
                    else
                        e.Row.Cells[0].Enabled = false;
                }
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int idDoc = int.Parse(this.GrdDocResultDisposal.DataKeys[e.Row.RowIndex].Value.ToString());
                if (idDoc.ToString().Equals("0"))
                {
                    ((CheckBox)e.Row.FindControl("checkDocumento")).Visible = false;
                    e.Row.Cells[3].Text = string.Empty;
                }
                //else
                //{
                //    bool value = this.gridDocSource.Where(x => x.Profile_ID == idDoc).FirstOrDefault().ToDisposal == 1;
                //    ((CheckBox)e.Row.FindControl("checkDocumento")).Checked = value;
                //}
                if (this.ListDocChecked.Contains(idDoc))
                    ((CheckBox)e.Row.FindControl("checkDocumento")).Checked = true;


            }
        }

        protected void GrdFascResultDisposal_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GrdFascResultDisposal.DataSource = this.gridFascSource;
            this.GrdFascResultDisposal.PageIndex = e.NewPageIndex;
            this.GrdFascResultDisposal.DataBind();
            this.UpPnlGridFascResultDisp.Update();
        }


        protected void GrdDocResultDisposal_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GrdDocResultDisposal.DataSource = this.gridDocSource;
            this.GrdDocResultDisposal.PageIndex = e.NewPageIndex;
            this.GrdDocResultDisposal.DataBind();
            this.UpPnlGridDocResultDisp.Update();
        }

        public void cImgBtnExportDocDisp_Click(object sender, EventArgs e)
        {

            string filename = "ExportDocumentiInScartoId=" + this.Disposal_ID.ToString() + ".xls";

            GridView grdForExport = new GridView();
            grdForExport.AllowPaging = false;
            grdForExport.DataSource = (from d in this.gridDocSource
                                       where d.Profile_ID > 0
                                       select new
                                       {
                                           DaScartare = d.ToDisposal == 1 ? "Si" : "No",
                                           Registro = d.Registro,
                                           UO = d.UO,
                                           CodiceDocumento = d.Profile_ID,
                                           Protocollo = d.Proto,
                                           DataProtocollo = d.ProtoDate,
                                           DataCreazione = d.CreateDate,
                                           Oggetto = d.Oggetto,
                                           Tipo = d.Tipo,
                                           Tipologia = d.Tipologia,
                                           MittenteDestestinatario = d.MittDest.Replace("<br />", ";"),
                                           CodiceFascicolo = d.ProjectCode.Replace("<br />", ";"),
                                           TipoTransfererimento = d.TipoTransfer

                                       }).ToList();
            grdForExport.DataBind();


            this.exportGridToExcel(grdForExport, filename);

        }

        private void exportGridToExcel(GridView gv, string filename)
        {
            try
            {
                if (gv.Rows.Count > 0)
                {

                    System.IO.StringWriter tw = new System.IO.StringWriter();
                    System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

                    //Get the HTML for the control.
                    gv.RenderControl(hw);
                    //Write the HTML back to the browser.
                    //Response.ContentType = application/vnd.ms-excel;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
                    Response.Write(tw.ToString());
                    Response.End();
                }
                else
                {
                    string msgErrorExportExcel = "msgErrorExportExcel";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msgErrorExportExcel.Replace("'", @"\'") + "', 'warning', '');", true);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }


        public void cImgBtnExportFascDisp_Click(object sender, EventArgs e)
        {
            string filename = "ExportFascicoliInScartoId= " + this.Disposal_ID.ToString() + ".xls";

            GridView grdForExport = new GridView();
            grdForExport.AllowPaging = false;
            grdForExport.DataSource = (from d in this.gridFascSource
                                       where d.Project_ID > 0
                                       select new
                                       {
                                           DaScartare = d.ToDisposal == 1 ? "Si" : "No",
                                           Registro = d.Registro,
                                           UO = d.UO,
                                           CodiceFascicolo = d.Project_ID,
                                           Descrizione = d.Descrizione,
                                           DataCreazione = d.StartDate,
                                           DataChiusura = d.CloseDate,
                                           Tipologia = d.Tipologia,
                                           TipoTransferimento = d.TipoTransfer

                                       }).ToList();
            grdForExport.DataBind();


            this.exportGridToExcel(grdForExport, filename);


        }


        private void closePage()
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeAJM", "closeAjaxModal('DisposalReport','');", true);
        }

        public override void VerifyRenderingInServerForm(Control control)
        {

        }
    }
}