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
    public partial class Timestamp : System.Web.UI.Page
    {
        #region Properties

        private List<TimestampDoc> TimestampsDoc
        {
            get
            {
                if (HttpContext.Current.Session["TimestampsDoc"] != null)
                    return (List<TimestampDoc>)HttpContext.Current.Session["TimestampsDoc"];
                else return null;
            }
            set
            {
                HttpContext.Current.Session["TimestampsDoc"] = value;
            }
        }

        private TimestampDoc TimestampDoc
        {
            set
            {
                HttpContext.Current.Session["TimestampDoc"] = value;
            }
        }

        private FileRequest FileReq
        {
            get
            {
                if (HttpContext.Current.Session["FileRequestTimestamp"] != null)
                    return (FileRequest)HttpContext.Current.Session["FileRequestTimestamp"];
                else return null;
            }
            set
            {
                HttpContext.Current.Session["FileRequestTimestamp"] = value;
            }
        }

        private bool UpdateImageTimestamp
        {
            set
            {
                HttpContext.Current.Session["UpdateImageTimestamp"] = value;
            }
        }

        private bool UpdateNewVersionTSD
        {
            set
            {
                HttpContext.Current.Session["UpdateNewVersionTSD"] = value;
            }
        }

        private void RemoveFileReq()
        {
            HttpContext.Current.Session.Remove("FileRequestTimestamp");
        }
        
        #endregion

        #region Const

        private const string UPDATE_PANEL_GRD_TIMESTAMP = "UpdatePanelGrdTimestamp";
        private const int ANNI_VALIDITA_MARCA = 20;

        #endregion

        #region Standard method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RemoveFileReq();
                InitializeContent();
                InitializeLanguage();
            }
            else
            {
                if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UPDATE_PANEL_GRD_TIMESTAMP))
                {
                    GrdTimestamp_Bind();
                    this.UpdatePanelGrdTimestamp.Update();
                    return;
                }
            }
        }

        private void InitializeContent()
        {

            if (UIManager.DocumentManager.getSelectedAttachId() != null)
            {
                FileReq = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
            }
            else
            {
                FileReq = FileManager.GetFileRequest();
            }

            if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
            {
                FileReq = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();

                if (UIManager.DocumentManager.getSelectedAttachId() != null)
                {
                    FileReq = ConvertiFileRequestInAllegato(FileReq);
                }

            }
            LoadTimestamp();
            if (RoleManager.GetRoleInSession().funzioni.Where(funz => funz.codice.ToUpper().Equals("DO_TIMESTAMP")).FirstOrDefault() != null)
            {
                this.BtnTimestampAssignsTSR.Enabled = true;
                if (TimestampsDoc != null && TimestampsDoc.Count > 0 && Convert.ToDateTime(TimestampsDoc[0].DTA_SCADENZA) > System.DateTime.Now)
                {
                    this.BtnTimestampCreatesTSD.Enabled = true;
                }
            }
            if (TimestampsDoc != null && TimestampsDoc.Count > 0)
            {
                GrdTimestamp_Bind();
                this.containerDetailsTimestamp.Visible = true;
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnTimestampAssignsTSR.Text = Utils.Languages.GetLabelFromCode("TimestampBtnAssignsTSR", language);
            this.BtnTimestampCreatesTSD.Text = Utils.Languages.GetLabelFromCode("TimestampBtnCreatesTSD", language);
            this.BtnTimestampSave.Text = Utils.Languages.GetLabelFromCode("TimestampBtnSave", language);
            this.BtnTimestampCancel.Text = Utils.Languages.GetLabelFromCode("BtnTimestampCancel", language);
            this.BtnTimestampCreatesTSD.ToolTip = Utils.Languages.GetLabelFromCode("BtnTimestampCreatesTSDTooltip", language);
            this.BtnTimestampAssignsTSR.ToolTip = Utils.Languages.GetLabelFromCode("BtnTimestampAssignsTSRTooltip", language);
            this.BtnTimestampSave.ToolTip = Utils.Languages.GetLabelFromCode("BtnTimestampSaveTooltip", language);
            this.TimestampLblSubject.Text = Utils.Languages.GetLabelFromCode("TimestampLblSubject", language);
            this.TimestampLblCountry.Text = Utils.Languages.GetLabelFromCode("TimestampLblCountry", language);
            this.TimestampLblCertified.Text = Utils.Languages.GetLabelFromCode("TimestampLblCertified", language);
            this.TimestampLblAlgorithm.Text = Utils.Languages.GetLabelFromCode("TimestampLblAlgorithm", language);
            this.TimestampLblSeries.Text = Utils.Languages.GetLabelFromCode("TimestampLblSeries", language);
            this.TimestampLblDetails.Text = Utils.Languages.GetLabelFromCode("TimestampLblDetails", language);
            this.TimestampsLbl.Text = Utils.Languages.GetLabelFromCode("TimestampsLbl", language);
            this.TimestampLblTipoValiditaMarca.Text = Utils.Languages.GetLabelFromCode("TimestampLblTipoValiditaMarca", language);
        }

        private void LoadTimestamp()
        {
            try
            {
                TimestampsDoc = DocumentManager.getTimestampsDoc(UserManager.GetInfoUser(), FileReq);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region Event Button


        protected void BtnTimestampAssignsTSR_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                DocsPaWR.SchedaDocumento sch = NttDataWA.UIManager.DocumentManager.getSelectedRecord();
                FileDocumento fileDocumento = FileManager.getInstance(sch.systemId).GetFile(this, FileReq, false, true);

                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

                string stringFile = BitConverter.ToString(fileDocumento.content);
                stringFile = stringFile.Replace("-", "");

                DocsPaWR.InputMarca inputMarca = new DocsPaWR.InputMarca();
                inputMarca.applicazione = infoUtente.urlWA;
                inputMarca.file_p7m = stringFile;
                inputMarca.riferimento = infoUtente.userId;
                OutputResponseMarca outputResponseMarca = DocumentManager.executeAndSaveTSR(infoUtente, inputMarca, FileReq);
                string message = "OK";
                if (outputResponseMarca == null)
                {
                    message = "TimestampErrorServiceNotAvailable";
                }
                if (outputResponseMarca != null && string.IsNullOrEmpty(outputResponseMarca.esito))
                    message = "TimestampErrorServiceNotAvailable";
                if (outputResponseMarca != null && outputResponseMarca.esito == "KO")
                    message = outputResponseMarca.descrizioneErrore;
                if (message == "OK")
                {
                    TimestampsDoc = DocumentManager.getTimestampsDoc(infoUtente, FileReq);
                    if (TimestampsDoc.Count == 1)
                    {
                        this.containerDetailsTimestamp.Visible = true;
                        this.BtnTimestampCreatesTSD.Enabled = true;
                        this.UpPnlButtons.Update();
                        UpdateImageTimestamp = true;
                    }
                    GrdTimestamp_Bind();
                    this.UpdatePanelGrdTimestamp.Update();
                }
                else
                {
                    if (outputResponseMarca != null && outputResponseMarca.esito == "KO")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('TimestampErrorAssignsTSR', 'error', '','" + message + "');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + message + "', 'error', '','');", true);
                    }
                    return;
                }

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnTimestampSave_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            if (this.TimestampsDoc != null && this.TimestampsDoc.Count > 0)
            {
                TimestampDoc = TimestampsDoc[this.GrdTimestamp.SelectedIndex + this.GrdTimestamp.PageIndex * this.GrdTimestamp.PageSize];
                this.frame.Attributes["src"] = "AttachmentTimestamp.aspx";
                this.UpdatePanelFrame.Update();
            }
            return;
        }

        protected void BtnTimestampCreatesTSD_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);

                FileRequest tsdVer = new FileRequest();
                // INC000000598064
                // se allegato devo fare il cast del filerequest
                // in modo da avere il numero di pagine all'inserimento della nuova versione
                var allegato = FileReq as Allegato;
                if (allegato != null)
                {
                    tsdVer = DocumentManager.CreateTSDVersion(UserManager.GetInfoUser(), allegato);
                }
                else
                {
                    tsdVer = DocumentManager.CreateTSDVersion(UserManager.GetInfoUser(), FileReq);
                }

                if (tsdVer == null)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('ErroreTimespampCreatesTSD', 'error', '','');", true);
                else
                {
                    UpdateNewVersionTSD = true;
                    if (DocumentManager.getSelectedAttachId() != null)
                    {
                        DocumentManager.setSelectedAttachId(tsdVer.versionId);
                    }
                    DocumentManager.setSelectedNumberVersion(tsdVer.version);
                    SchedaDocumento sch = DocumentManager.getDocumentDetails(this, DocumentManager.getSelectedRecord().systemId, DocumentManager.getSelectedRecord().systemId);
                    DocumentManager.setSelectedRecord(sch);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('SuccessTimespampCreatesTSD', 'check', '','');", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('ErroreTimespampCreatesTSD', 'error', '','');", true);
                return;
            }
        }

        protected void BtnTimestampCancel_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                RemoveFileReq();
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('Timestamp','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region Grid manager

        private void GrdTimestamp_Bind()
        {
            try
            {
                this.GrdTimestamp.DataSource = TimestampsDoc;
                this.GrdTimestamp.DataBind();
                if (string.IsNullOrEmpty(this.grid_rowindex.Value))
                {
                    this.GrdTimestamp.SelectedIndex = 0;
                }
                else
                {
                    this.GrdTimestamp.SelectedIndex = int.Parse(this.grid_rowindex.Value);
                }
                HighlightSelectedRow();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GrdTimestamp_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                   
                    for (int i = 0; i < e.Row.Cells.Count-1; i++)
                    {
                        e.Row.Cells[i].Attributes["onClick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "');__doPostBack('UpdatePanelGrdTimestamp');return false;";
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void HighlightSelectedRow()
        {
            if (this.GrdTimestamp.Rows.Count > 0 && this.GrdTimestamp.SelectedRow != null)
            {
                GridViewRow gvRow = this.GrdTimestamp.SelectedRow;
                foreach (GridViewRow GVR in this.GrdTimestamp.Rows)
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
                TimestampDoc t = TimestampsDoc[this.GrdTimestamp.SelectedIndex + this.GrdTimestamp.PageIndex * this.GrdTimestamp.PageSize];
                this.TimestampLblTxtSubject.Text = t.SOGGETTO;
                this.TimestampLblTxtCountry.Text = t.PAESE;
                this.TimestampLblTxtCertified.Text = t.S_N_CERTIFICATO;
                this.TimestampLblTxtAlgorithm.Text = t.ALG_HASH;
                this.TimestampLblTxtSeries.Text = t.NUM_SERIE;
                this.imgCheckTimestampTipoValiditaMarca.ImageUrl = TipoValiditaMarca(t);
                //this.PnlExpiredCertificate.Visible = !Utils.utils.verificaIntervalloDate(t.DTA_SCADENZA, DateTime.Now.ToString());
                this.TimestampLblExpiredCertificate.Text = Utils.Languages.GetLabelFromCode("TimestampLblExpiredCertificate", UserManager.GetUserLanguage()).Replace("@@", t.DTA_SCADENZA);
            }
        }

        protected void GrdTimestamp_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.GrdTimestamp.PageIndex = e.NewPageIndex;
                this.grid_rowindex.Value = "0";
                GrdTimestamp_Bind();
                this.UpdatePanelGrdTimestamp.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        private FileRequest ConvertiFileRequestInAllegato(FileRequest fileReq)
        {
            FileRequest file = new DocsPaWR.Allegato();

            file.applicazione = fileReq.applicazione;
            file.autore = fileReq.autore;
            file.autoreFile = fileReq.autoreFile;
            file.cartaceo = fileReq.cartaceo;
            file.daAggiornareFirmatari = fileReq.daAggiornareFirmatari;
            file.dataAcquisizione = fileReq.dataAcquisizione;
            file.dataInserimento = fileReq.dataInserimento;
            file.descrizione = fileReq.descrizione;
            file.docNumber = fileReq.docNumber;
            file.docServerLoc = fileReq.docServerLoc;
            file.fileName = fileReq.fileName;
            file.fileSize = fileReq.fileSize;
            file.firmatari = fileReq.firmatari;
            file.firmato = fileReq.firmato;
            file.fNversionId = fileReq.fNversionId;
            file.idPeople = fileReq.idPeople;
            file.idPeopleDelegato = fileReq.idPeopleDelegato;
            file.inLibroFirma = fileReq.inLibroFirma;
            file.msgErr = fileReq.msgErr;
            file.path = fileReq.path;
            file.repositoryContext = fileReq.repositoryContext;
            file.subVersion = fileReq.subVersion;
            file.version = fileReq.version;
            file.versionId = fileReq.versionId;
            file.versionLabel = fileReq.versionLabel;
            
            (file as Allegato).numeroPagine = (FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId()) as Allegato).numeroPagine;

            return file;
        }

        private string TipoValiditaMarca(TimestampDoc t)
        {
            string url = string.Empty;
            string fineValiditaMarca = Convert.ToDateTime(t.DTA_CREAZIONE).AddYears(ANNI_VALIDITA_MARCA).ToString();

            bool isValid = Utils.utils.verificaIntervalloDate(fineValiditaMarca, DateTime.Now.ToString());
            url = isValid ? "../Images/Icons/identity_valid.png" : "../Images/Icons/identity_not_valid.png";
            return url;
        }


        protected string GetDataFineValiditaMarca(TimestampDoc timeStamp)
        {
            return Convert.ToDateTime(timeStamp.DTA_CREAZIONE).AddYears(ANNI_VALIDITA_MARCA).ToString();
        }
    }
}