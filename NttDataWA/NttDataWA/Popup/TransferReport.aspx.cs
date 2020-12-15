using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class TransferReport : System.Web.UI.Page
    {
        #region field

        protected Int32 Transfer_ID
        {
            get
            {
                Int32 result = 0;
                if (HttpContext.Current.Session["CurrentTransfer_ID"] != null)
                    result = int.Parse(HttpContext.Current.Session["CurrentTransfer_ID"].ToString());
                return result;
            }
        }

        protected List<ARCHIVE_Transfer> TransferOriginalInContext
        {
            get
            {
                List<ARCHIVE_Transfer> result = null;
                if (HttpContext.Current.Session["TransferOriginalInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferOriginalInContext"] as List<ARCHIVE_Transfer>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferOriginalInContext"] = value;
            }
        }

        protected List<Int32> TransferPolicyListIdInContext
        {
            get
            {
                List<Int32> result = null;
                if (HttpContext.Current.Session["TransferPolicyListId"] != null)
                {
                    result = HttpContext.Current.Session["TransferPolicyListId"] as List<Int32>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferPolicyListId"] = value;
            }
        }

        protected List<ARCHIVE_View_FascReport> gridFascSource
        {
            get
            {
                List<ARCHIVE_View_FascReport> result = null;
                if (HttpContext.Current.Session["gridFascSource"] != null)
                {
                    result = HttpContext.Current.Session["gridFascSource"] as List<ARCHIVE_View_FascReport>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["gridFascSource"] = value;
            }
        }

        protected List<ARCHIVE_View_DocReport> gridDocSource
        {
            get
            {
                List<ARCHIVE_View_DocReport> result = null;
                if (HttpContext.Current.Session["gridDocSource"] != null)
                {
                    result = HttpContext.Current.Session["gridDocSource"] as List<ARCHIVE_View_DocReport>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["gridDocSource"] = value;
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
            ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(this.cImgBtnExportDoc);
            ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(this.cImgBtnExportFasc);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeTableDoc", "resizeTableDoc();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeTableFasc", "resizeTableFasc();", true);
        }

        private void PopulatePageObject()
        {


            if (this.Transfer_ID > 0 && TransferOriginalInContext.Count > 0 && this.TransferOriginalInContext != null)
            {
                this.txtIdVersamento.Text = Transfer_ID.ToString();
                this.TxtDescrVersamento.Text = this.TransferOriginalInContext.FirstOrDefault().Description;
            }

        }

        private void GetData()
        {
            try
            {
                this.txtIdVersamento.Text = this.Transfer_ID.ToString();
                this.TxtDescrVersamento.Text = this.TransferOriginalInContext.FirstOrDefault().Description;

                List<ARCHIVE_View_FascReport> listaFasc = UIManager.ArchiveManager.GetARCHIVE_View_FascReportByTransfer_ID(this.Transfer_ID);


                //li elabero per presentarli meglio
                while (listaFasc.Count < 9 || listaFasc.Count % 9 != 0)
                    listaFasc.Add(new ARCHIVE_View_FascReport());
                //datasourcefinale
                this.gridFascSource = listaFasc;

                List<ARCHIVE_View_DocReport> listaDoc = UIManager.ArchiveManager.GetARCHIVE_View_DocReportByTransfer_ID(this.Transfer_ID);

                //li elabero per presentarli meglio
                var righe = (from d in listaDoc
                             select new ARCHIVE_View_DocReport
                             {
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
                    righe.Add(new ARCHIVE_View_DocReport());
                //datasourcefinale
                this.gridDocSource = righe;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }

        public List<ARCHIVE_View_DocReport> GetDataSourceDummyGrdDoc()
        {
            List<ARCHIVE_View_DocReport> dummy = new List<ARCHIVE_View_DocReport>();
            ARCHIVE_View_DocReport element = new ARCHIVE_View_DocReport();

            dummy.Add(element);
            return dummy;
        }

        public List<ARCHIVE_View_FascReport> GetDataSourceDummyGrdFasc()
        {
            List<ARCHIVE_View_FascReport> dummy = new List<ARCHIVE_View_FascReport>();
            ARCHIVE_View_FascReport element = new ARCHIVE_View_FascReport();

            dummy.Add(element);
            return dummy;
        }

        private void InitializePage()
        {
            this.InitializeLanguage();
            this.GetData();
            this.PopulatePageObject();
            this.LoadDataInGrid();

        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.litIdVersamento.Text = Utils.Languages.GetLabelFromCode("LitIdVersamento", language);
            this.litDescrVersamento.Text = Utils.Languages.GetLabelFromCode("LitDescVers", language);
            this.DocTabLinkList.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblDocument", language);
            this.FascTabLinkList.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchProject", language);
            this.btnClose.Text = Utils.Languages.GetLabelFromCode("AddDocInProjectBtnClose", language);
            this.cImgBtnExportDoc.ToolTip = Utils.Languages.GetLabelFromCode("cImgBtnReportDocFasc", language);
            this.cImgBtnExportFasc.ToolTip = Utils.Languages.GetLabelFromCode("cImgBtnReportDocFasc", language);

        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            try
            {

                this.closePage();
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
                this.liReportDocumenti.Attributes.Remove("class");
                this.liReportDocumenti.Attributes.Add("class", "addressTab");
                this.liReportFascicoli.Attributes.Remove("class");
                this.liReportFascicoli.Attributes.Add("class", "otherAddressTab");
                this.UpTypeResult.Update();

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


                this.liReportDocumenti.Attributes.Remove("class");
                this.liReportDocumenti.Attributes.Add("class", "otherAddressTab");
                this.liReportFascicoli.Attributes.Remove("class");
                this.liReportFascicoli.Attributes.Add("class", "addressTab");
                this.UpTypeResult.Update();

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
            this.UpPnlGridFascResult.Update();

            this.PnlGridDocResult.Visible = !visible;
            this.UpPnlGridDocResult.Update();
        }

        private void LoadDataInGrid()
        {
            if (this.gridDocSource != null && this.gridDocSource.Count > 0)
            {
                this.GrdDocResult.DataSource = this.gridDocSource;
                this.GrdDocResult.DataBind();
                this.UpPnlGridDocResult.Update();
            }
            else
            {
                this.GrdDocResult.DataSource = this.GetDataSourceDummyGrdDoc();
                this.GrdDocResult.DataBind();
                this.UpPnlGridDocResult.Update();
                //this.GrdDocResult.Rows[0].Visible = false;
            }
            //ARCHIVE_View_DocReport doc = new ARCHIVE_View_DocReport();
            //doc.c


            if (this.gridFascSource != null && this.gridFascSource.Count > 0)
            {
                this.GrdFascResult.DataSource = this.gridFascSource;
                this.GrdFascResult.DataBind();
                this.UpPnlGridFascResult.Update();
            }
            else
            {
                this.GrdFascResult.DataSource = this.GetDataSourceDummyGrdFasc();
                this.GrdFascResult.DataBind();
                this.UpPnlGridFascResult.Update();
            }

        }

        protected void GrdFascResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (this.GrdFascResult.DataKeys[e.Row.RowIndex].Value.ToString().Equals("0"))
                {
                    e.Row.Cells[2].Text = string.Empty;
                }
            }
        }

        protected void GrdDocResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (this.GrdDocResult.DataKeys[e.Row.RowIndex].Value.ToString().Equals("0"))
                {
                    e.Row.Cells[2].Text = string.Empty;
                }
            }
        }

        protected void GrdFascResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GrdFascResult.DataSource = this.gridFascSource;
            this.GrdFascResult.PageIndex = e.NewPageIndex;
            this.GrdFascResult.DataBind();
            this.UpPnlGridFascResult.Update();
        }


        protected void GrdDocResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GrdDocResult.DataSource = this.gridDocSource;
            this.GrdDocResult.PageIndex = e.NewPageIndex;
            this.GrdDocResult.DataBind();
            this.UpPnlGridDocResult.Update();
        }

        public void btnExportDoc_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = "ExportDocumentiInVersamentoId=" + this.Transfer_ID.ToString() + ".xls";

                GridView grdForExport = new GridView();
                grdForExport.AllowPaging = false;
                grdForExport.DataSource = (from d in this.gridDocSource
                                           where d.Profile_ID > 0
                                           select new
                                           {
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
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


        public void btnExportFasc_Click(object sender, EventArgs e)
        {
            string filename = "ExportFascicoliInVersamentoId= " + this.Transfer_ID.ToString() + ".xls";

            GridView grdForExport = new GridView();
            grdForExport.AllowPaging = false;
            grdForExport.DataSource = (from d in this.gridFascSource
                                       where d.Project_ID > 0
                                       select new
                                       {
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
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeAJM", "closeAjaxModal('TransferReport','');", true);
        }

        public override void VerifyRenderingInServerForm(Control control)
        {

        }
    }
}