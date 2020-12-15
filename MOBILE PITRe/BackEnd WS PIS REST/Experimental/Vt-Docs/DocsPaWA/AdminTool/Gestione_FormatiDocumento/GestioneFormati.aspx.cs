using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.AdminTool.Gestione_FormatiDocumento
{
    /// <summary>
    /// 
    /// </summary>
    public partial class GestioneFormati : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        private DocsPaWR.DocsPaWebService _wsInstance = null;

        /// <summary>
        /// 
        /// </summary>
        protected DocsPaWR.DocsPaWebService WsInstance
        {
            get
            {
                if (this._wsInstance == null)
                    this._wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();
                return this._wsInstance;
            }
        }

        #region Gestione caricamento dati lista

        /// <summary>
        /// Costanti che identificano le colonne del dataset
        /// per il caricamento dati griglia
        /// </summary>
        private const string COL_SYSTEM_ID = "SystemId";
        private const string COL_ID_AMMINISTRAZIONE = "IdAmministrazione";
        private const string COL_DESCRIPTION = "Description";
        //private const string COL_MIME_TYPE = "MimeType";
        private const string COL_FILE_EXTENSION = "FileExtension";
        private const string COL_MAX_FILE_SIZE = "MaxFileSize";
        private const string COL_FILE_TYPE_USED = "FileTypeUsed";
        private const string COL_CONTAINS_FILE_MODEL = "ContainsFileModel";
        private const string COL_FILE_TYPE_SIGNATURE = "FileTypeSignature";
        private const string COL_FILE_TYPE_PRESERVATION = "FileTypePreservation";
        private const string COL_FILE_TYPE_VALIDATION = "FileTypeValidation";

        /// <summary>
        /// Caricamento dati 
        /// </summary>
        protected virtual void FetchData()
        {
            SupportedFileType[] fileTypes = this.GetSupportedFileTypes();

            this.grdTipiFile.DataSource = this.ToDataset(fileTypes);
            this.grdTipiFile.DataBind();

            this.FileTypes = fileTypes;

            //#region MEV CS 1.3
            ////
            //// Se la chiave di DB "PGU_FE_DISABLE_AMM_GEST_CONS" è attiva, le funzionalità di conservazione in amministrazione vengono disabilitate
            //if (this.DisableAmmGestCons()) 
            //{
            //    //
            //    // Colonna N.ro 6: Ammesso conservazione
            //    // Colonna N.ro 7: Validazione conservazione
            //    this.grdTipiFile.Columns[6].Visible = false;
            //    this.grdTipiFile.Columns[7].Visible = false;
            //}
            //#endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supportedFileTypes"></param>
        /// <returns></returns>
        protected DataSet ToDataset(SupportedFileType[] supportedFileTypes)
        {
            DataSet ds = new DataSet("DatasetSupportedFileTypes");
            DataTable dt = new DataTable("TableSupportedFileTypes");

            dt.Columns.Add(COL_SYSTEM_ID, typeof(int));
            dt.Columns.Add(COL_ID_AMMINISTRAZIONE, typeof(int));
            dt.Columns.Add(COL_DESCRIPTION, typeof(string));
            //dt.Columns.Add(COL_MIME_TYPE, typeof(string));
            dt.Columns.Add(COL_FILE_EXTENSION, typeof(string));
            dt.Columns.Add(COL_MAX_FILE_SIZE, typeof(string));
            dt.Columns.Add(COL_FILE_TYPE_USED, typeof(string));
            dt.Columns.Add(COL_CONTAINS_FILE_MODEL, typeof(string));
            dt.Columns.Add(COL_FILE_TYPE_SIGNATURE, typeof(string));
            dt.Columns.Add(COL_FILE_TYPE_PRESERVATION, typeof(string));
            dt.Columns.Add(COL_FILE_TYPE_VALIDATION, typeof(string));

            int idAmministrazione = this.IdAmministrazione;

            foreach (SupportedFileType item in supportedFileTypes)
            {
                DataRow row = dt.NewRow();

                row[COL_SYSTEM_ID] = item.SystemId;
                row[COL_ID_AMMINISTRAZIONE] = item.IdAmministrazione;
                row[COL_DESCRIPTION] = item.Description;
                //row[COL_MIME_TYPE] = item.MimeType;
                row[COL_FILE_EXTENSION] = item.FileExtension;

                if (item.MaxFileSize > 0)
                    row[COL_MAX_FILE_SIZE] = string.Format("{0} kb", item.MaxFileSize.ToString());
                else
                    row[COL_MAX_FILE_SIZE] = "Illimitata"; 

                if (item.FileTypeUsed)
                    row[COL_FILE_TYPE_USED] = "Sì";
                else
                    row[COL_FILE_TYPE_USED] = "No";

                if (item.ContainsFileModel)
                    row[COL_CONTAINS_FILE_MODEL] = "Sì";
                else
                    row[COL_CONTAINS_FILE_MODEL] = "No";
                
                if (item.FileTypeSignature)
                    row[COL_FILE_TYPE_SIGNATURE] = "Sì";
                else
                    row[COL_FILE_TYPE_SIGNATURE] = "No";

                if (item.FileTypePreservation)
                    row[COL_FILE_TYPE_PRESERVATION] = "Sì";
                else
                    row[COL_FILE_TYPE_PRESERVATION] = "No";

                if (item.FileTypeValidation)
                    row[COL_FILE_TYPE_VALIDATION] = "Sì";
                else
                    row[COL_FILE_TYPE_VALIDATION] = "No";

                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);

            return ds;
        }

        /// <summary>
        /// Tipi di file
        /// </summary>
        private SupportedFileType[] FileTypes
        {
            get
            {
                if (this.ViewState["SupportedFileType"] == null)
                    return null;
                else
                    return this.ViewState["SupportedFileType"] as SupportedFileType[];
            }
            set
            {
                if (this.ViewState["SupportedFileType"] == null)
                    this.ViewState.Add("SupportedFileType", value);
                else
                    this.ViewState["SupportedFileType"] = value;
            }
        }

        /// <summary>
        /// Modalità di inserimento nuovo tipo di file
        /// </summary>
        protected bool InsertMode
        {
            get
            {
                if (this.ViewState["InsertMode"] != null)
                    return Convert.ToBoolean(this.ViewState["InsertMode"].ToString());
                else
                    return false;
            }
            set
            {
                if (this.ViewState["InsertMode"] != null)
                    this.ViewState["InsertMode"] = value;
                else
                    this.ViewState.Add("InsertMode", value);
            }
        }

        /// <summary>
        /// Reperimento idamministrazione corrente
        /// </summary>
        protected int IdAmministrazione
        {
            get
            {
                string idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                return Convert.ToInt32(idAmministrazione);
            }
        }

        /// <summary>
        /// Reperimento codice amministrazione
        /// </summary>
        protected string CodiceAmministrazione
        {
            get
            {
                return AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
            }
        }

        /// <summary>
        /// Reperimento tipi di file supportati dall'amministrazione corrente
        /// </summary>
        /// <returns></returns>
        protected SupportedFileType[] GetSupportedFileTypes()
        {
            return this.WsInstance.GetSupportedFileTypes(this.IdAmministrazione);
        }

        #endregion

        #region Gestione caricamento dettaglio

        #endregion

        #region Handler eventi

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            this.SetDescription(string.Format("Presenti {0} formati", this.grdTipiFile.Items.Count.ToString()));

            this.grdTipiFile.Visible = (this.FileTypes.Length > 0);
        }

        /// <summary>
        /// Impostazione descrizione
        /// </summary>
        /// <param name="description"></param>
        protected void SetDescription(string description)
        {
            this.lblDescription.Text = description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
            Session["AdminBookmark"] = "FormatoDocumenti";
            if (Session.IsNewSession)
            {
                Response.Redirect("../Exit.aspx?FROM=EXPIRED");
            }

            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            if (!ws.CheckSession(Session.SessionID))
            {
                Response.Redirect("../Exit.aspx?FROM=ABORT");
            }
            // ---------------------------------------------------------------
            
            this.RegisterScrollKeeper("divGrdTipiFile");			

            if (!this.IsPostBack)
            {
                // Registrazione handler javascript
                this.RegisterJavaScriptEventHandlers();

                // Il dettaglio viene nascosto
                this.ShowDetail(false);

                // Caricamento dati tipi di file
                this.FetchData();

                this.lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
            }

            #region MEV CS 1.3
            //
            // Se la chiave di DB "PGU_FE_DISABLE_AMM_GEST_CONS" è attiva, le funzionalità di conservazione in amministrazione vengono disabilitate
            if (this.DisableAmmGestCons()) 
            {
                this.lblConservazione.Visible = false;
                this.chkUseFileTypePreservation.Visible = false;
                this.lblValidazone.Visible = false;
                this.chkUseFileTypeValidation.Visible = false;
            }

            // INTEGRAZIONE PITRE-PARER
            // Se è attiva la conservazione PARER i campi relativi alla conservazione devono essere nascosti
            if (this.IsConservazionePARER())
            {
                this.lblConservazione.Visible = false;
                this.chkUseFileTypePreservation.Visible = false;
                this.lblValidazone.Visible = false;
                this.chkUseFileTypeValidation.Visible = false;
                foreach (DataGridColumn col in grdTipiFile.Columns)
                {
                    if (col.HeaderText.Equals("Ammesso conservazione") || col.HeaderText.Equals("Validazione conservazione"))
                    {
                        col.Visible = false;
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdTipiFile_ItemCreated(object sender, DataGridItemEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="imageName"></param>
        /// <param name="value"></param>
        private void SetGridImageFlag(DataGridItem item, string imageName, bool value)
        {
            Image image = item.FindControl(imageName) as Image;

            if (image != null)
                image.Visible = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdTipiFile_PreRender(object sender, EventArgs e)
        {
            foreach (DataGridItem item in this.grdTipiFile.Items)
            {
                //ImageButton button = item.Cells[8].FindControl("btnDeleteItem") as ImageButton;
                ImageButton button = item.FindControl("btnDeleteItem") as ImageButton;
                if (button != null)
                    button.Attributes.Add("onClick", "if (ConfirmRemoveItem()) { ShowWaitPanel('Rimozione formato in corso...'); return true; } else { return false; }");

                this.SetGridImageFlag(item, "imgFileTypeUsed", this.FileTypes[item.ItemIndex].FileTypeUsed);
                this.SetGridImageFlag(item, "imgFileTypeUsedFirma", this.FileTypes[item.ItemIndex].FileTypeSignature);
                this.SetGridImageFlag(item, "imgFileTypeUsedConservazione", this.FileTypes[item.ItemIndex].FileTypePreservation);
                this.SetGridImageFlag(item, "imgFileTypeUsedValidazione", this.FileTypes[item.ItemIndex].FileTypeValidation);

                ////Image image = item.Cells[4].FindControl("imgFileTypeUsed") as Image;
                //Image image = item.FindControl("imgFileTypeUsed") as Image;
                //if (image != null)
                //    image.Visible = this.FileTypes[item.ItemIndex].FileTypeUsed;

                ////image = item.Cells[5].FindControl("imgFileTypeUsedFirma") as Image;
                //image = item.FindControl("imgFileTypeUsedFirma") as Image;
                //if (image != null)
                //    image.Visible = this.FileTypes[item.ItemIndex].FileTypeSignature;

                ////image = item.Cells[6].FindControl("imgFileTypeUsedConservazione") as Image;
                //image = item.FindControl("imgFileTypeUsedConservazione") as Image;
                //if (image != null)
                //    image.Visible = this.FileTypes[item.ItemIndex].FileTypePreservation;

                //button = item.Cells[7].FindControl("btnEditItem") as ImageButton;
                button = item.FindControl("btnEditItem") as ImageButton;
                if (button != null)
                {
                    if (!this.FileTypes[item.ItemIndex].ContainsFileModel)
                    {
                        button.ImageUrl = "../../images/proto/dett_lente.gif";
                        button.ToolTip = "Modifica formato documento";
                    }
                    else
                    {
                        button.ImageUrl = "../../images/proto/dett_lente_doc.gif";
                        button.ToolTip = "Modifica formato documento (modello predefinito presente)";
                    }
                }

                //button = item.Cells[8].FindControl("btnDeleteItem") as ImageButton;
                button = item.FindControl("btnDeleteItem") as ImageButton;
                if (button != null)
                    button.Attributes.Add("onclick", "return ConfirmRemoveItem('" + this.FileTypes[item.ItemIndex].ContainsFileModel + "')");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected DocsPaWebCtrlLibrary.ImageButton GetButtonEditItem(DataGridItem item)
        {
            return item.Cells[6].FindControl("btnEditItem") as DocsPaWebCtrlLibrary.ImageButton;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected DocsPaWebCtrlLibrary.ImageButton GetButtonDeleteItem(DataGridItem item)
        {
            return item.Cells[7].FindControl("btnDeleteItem") as DocsPaWebCtrlLibrary.ImageButton;
        }

        /// <summary>
        /// Associazione dati campi UI
        /// </summary>
        /// <param name="supportedFile"></param>
        private void BindData(SupportedFileType supportedFile)
        {
            this.ClearData();

           
            this.txtDescrizione.Text = supportedFile.Description;
            //this.txtMimeType.Text = supportedFile.MimeType;
            this.txtEstensione.Text = supportedFile.FileExtension;
            this.txtDimMaxFile.Text = supportedFile.MaxFileSize.ToString();
            this.chkUseFileType.Checked = supportedFile.FileTypeUsed;
            if (!this.chkUseFileType.Checked)
            {
                this.chkUseFileTypePreservation.Enabled = false;
                this.chkUseFileTypeSignature.Enabled = false;
                this.chkUseFileTypeValidation.Enabled = false;
                this.chkUseFileTypePreservation.Checked = false;
                this.chkUseFileTypeSignature.Checked = false;
                this.chkUseFileTypeValidation.Checked = false;
                this.lblFirma.Enabled = false;
                this.lblConservazione.Enabled = false;
            }
            else
            {
                this.chkUseFileTypeSignature.Checked = supportedFile.FileTypeSignature;
                this.chkUseFileTypePreservation.Checked = supportedFile.FileTypePreservation;
                this.chkUseFileTypeValidation.Checked = supportedFile.FileTypeValidation;
            }       
            // Modalità avviso impostazione dimensione file
            this.cboMaxFileSizeAlertMode.SelectedValue = supportedFile.MaxFileSizeAlertMode.ToString();

            // Tipo documento cui può essere applicato il formato 
            this.cboDocumentTypes.SelectedValue = supportedFile.DocumentType.ToString();
        }

        /// <summary>
        /// Rimozione valori campi UI
        /// </summary>
        private void ClearData()
        {
            this.grdTipiFile.SelectedIndex = -1;
            this.chkUseFileType.Checked = false;
            this.txtDescrizione.Text = string.Empty;
            //this.txtMimeType.Text = string.Empty;
            this.txtEstensione.Text = string.Empty;
            this.txtDimMaxFile.Text = string.Empty;
            this.cboMaxFileSizeAlertMode.SelectedValue = MaxFileSizeAlertModeEnum.Warning.ToString();
            this.cboDocumentTypes.SelectedValue = DocumentTypeEnum.All.ToString();
            this.chkUseFileTypeSignature.Checked = false;
            this.chkUseFileTypePreservation.Checked = false;
            this.chkUseFileTypeValidation.Checked = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RegisterScrollKeeper(string divID)
        {
            AdminTool.UserControl.ScrollKeeper scrollKeeper = new AdminTool.UserControl.ScrollKeeper();
            scrollKeeper.WebControl = divID;
            this.Form.Controls.Add(scrollKeeper);
        }

        /// <summary>
        /// Visualizzazione dettaglio
        /// </summary>
        /// <param name="visible"></param>
        private void ShowDetail(bool visible)
        {
            this.tblDetail.Visible = visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void grdTipiFile_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "EDIT_ITEM")
            {
                this.PerformActionEditFileType(e.Item);
            }
            else if (e.CommandName == "DELETE_ITEM")
            {
                this.PerformActionRemoveFileType(e.Item);
            }
        }

        /// <summary>
        /// Azione di cancellazione file modello
        /// </summary>
        /// <param name="item"></param>
        private void PerformActionDeleteModel()
        {
            SupportedFileType fileType = this.FileTypes[this.grdTipiFile.SelectedIndex];

            ValidationResultInfo result = this.WsInstance.RemoveFileTypeDocumentModel(this.CodiceAmministrazione, fileType.FileExtension);

            if (!result.Value)
            {
                this.ShowValidationMessage(result.BrokenRules);
            }
            else
            {
                fileType.ContainsFileModel = false;
                this.btnDeleteFileModel.Visible = false;

                this.FetchData();

                this.SetFileTypeAsSelected(fileType);
            }
        }

        /// <summary>
        /// Azione di associazione file modello
        /// </summary>
        /// <param name="item"></param>
        private void PerformActionUploadFile(DataGridItem item)
        {
            this.FetchData();

            this.SetFileTypeAsSelected(this.FileTypes[item.ItemIndex]);
        }

        /// <summary>
        /// Inserimento nuovo tipo di file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNuovoTipoFile_Click(object sender, EventArgs e)
        {
            this.PerformActionNew();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkUseFileType_CheckedChanged(object sender, EventArgs e)
        {
            //if (this.grdTipiFile.SelectedIndex >= 0)
            //{
            //    SupportedFileType fileType = this.FileTypes[this.grdTipiFile.SelectedIndex];
            //    if (!this.chkUseFileType.Checked && fileType.ContainsFileModel)
            //        fileType.ContainsFileModel = false;
            //}
            if (!this.chkUseFileType.Checked)
            {
                this.chkUseFileTypePreservation.Enabled = false;
                this.chkUseFileTypeSignature.Enabled = false;
                this.chkUseFileTypeValidation.Enabled = false;
                this.chkUseFileTypePreservation.Checked = false;
                this.chkUseFileTypeSignature.Checked = false;
                this.chkUseFileTypeValidation.Checked = false;
                this.lblFirma.Enabled = false;
                this.lblConservazione.Enabled = false;

            }
            else
            {
                this.chkUseFileTypePreservation.Enabled = true;
                this.chkUseFileTypeSignature.Enabled = true;
                this.chkUseFileTypeValidation.Enabled = true;
                this.chkUseFileTypePreservation.Checked = true;
                this.chkUseFileTypeSignature.Checked = true;
                this.chkUseFileTypeValidation.Checked = true;
                this.lblFirma.Enabled = true;
                this.lblConservazione.Enabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkUseFileTypeSignature_CheckedChanged(object sender, EventArgs e)
        {
            //if (this.grdTipiFile.SelectedIndex >= 0)
            //{
            //    SupportedFileType fileType = this.FileTypes[this.grdTipiFile.SelectedIndex];
            //    if (!this.chkUseFileType.Checked && fileType.ContainsFileModel)
            //        fileType.ContainsFileModel = false;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkUseFileTypePreservation_CheckedChanged(object sender, EventArgs e)
        {
            //if (this.grdTipiFile.SelectedIndex >= 0)
            //{
            //    SupportedFileType fileType = this.FileTypes[this.grdTipiFile.SelectedIndex];
            //    if (!this.chkUseFileType.Checked && fileType.ContainsFileModel)
            //        fileType.ContainsFileModel = false;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkUseFileTypeValidation_CheckedChanged(object sender, EventArgs e)
        {
            //if (this.grdTipiFile.SelectedIndex >= 0)
            //{
            //    SupportedFileType fileType = this.FileTypes[this.grdTipiFile.SelectedIndex];
            //    if (!this.chkUseFileType.Checked && fileType.ContainsFileModel)
            //        fileType.ContainsFileModel = false;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.PerformActionSave();
        }

        /// <summary>
        /// Cancellazione modello file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDeleteFileModel_Click(object sender, ImageClickEventArgs e)
        {
            this.PerformActionDeleteModel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClose_Click(object sender, EventArgs e)
        {
            this.ShowDetail(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClosePanel_Click(object sender, ImageClickEventArgs e)
        {
            this.ShowDetail(false);
        }

        /// <summary>
        /// Azione di predisposizione alla modifica elemento
        /// </summary>
        /// <param name="item"></param>
        protected virtual void PerformActionEditFileType(DataGridItem item)
        {
            // Modalità di aggiornamento dati
            this.InsertMode = false;

            // Azione di modifica elemento
            SupportedFileType supportedFile = this.FileTypes[item.ItemIndex];

            // Visualizzazione dati tipo file
            this.BindData(supportedFile);

            // Elemento selezionato
            this.grdTipiFile.SelectedIndex = item.ItemIndex;

            //  Pulsante di cancellazione modello file predefinito visibile solamente se presente
            this.btnDeleteFileModel.Visible = supportedFile.ContainsFileModel;

            // Visualizzazione dettaglio
            this.ShowDetail(true);

            this.RegisterClientScript("SetFocus", "SetFocus('" + this.txtDescrizione.ID + "');");
        }

        /// <summary>
        /// Azione di rimozione elemento
        /// </summary>
        /// <param name="item"></param>
        protected virtual void PerformActionRemoveFileType(DataGridItem item)
        {
            SupportedFileType fileToDelete = this.FileTypes[item.ItemIndex];

            ValidationResultInfo result = this.WsInstance.RemoveSupportedFileType(ref fileToDelete);

            if (result.Value)
            {
                this.FetchData();

                // Il dettaglio dei dati viene nascosto
                if (this.grdTipiFile.SelectedIndex == item.ItemIndex)
                    this.ShowDetail(false);
            }
            else
            {
                // Save dei dati non andato a buon fine,
                // visualizzazione messaggio di errore
                this.ShowValidationMessage(result.BrokenRules);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected byte[] GetModelFileContent()
        {
            byte[] content = null;

            if (this.uploadFile.PostedFile != null && uploadFile.PostedFile.FileName != string.Empty)
            {
                content = new byte[uploadFile.PostedFile.InputStream.Length];
                uploadFile.PostedFile.InputStream.Read(content, 0, content.Length);
            }

            return content;
        }

        /// <summary>
        /// Validazione dati
        /// </summary>
        /// <param name="firstInvalidControl"></param>
        /// <returns></returns>
        protected virtual ValidationResultInfo ValidateData(out Control firstInvalidControl)
        {
            firstInvalidControl = null;
            ValidationResultInfo validationResult = new ValidationResultInfo();

            List<BrokenRule> brokenRuleList = new List<BrokenRule>();

            // Verifica dati obbligatori
            if (this.txtDescrizione.Text.Trim() == string.Empty)
            {
                if (firstInvalidControl == null)
                    firstInvalidControl = this.txtDescrizione;

                brokenRuleList.Add(this.GetBrokenRule("MISSING_DESCRIPTION", "Descrizione mancante"));
            }

            //if (this.txtMimeType.Text.Trim() == string.Empty)
            //{
            //    if (firstInvalidControl == null)
            //        firstInvalidControl = this.txtMimeType;

            //    brokenRuleList.Add(this.GetBrokenRule("MISSING_MIME_TYPE", "Mime Type mancante"));
            //}

            if (this.txtEstensione.Text.Trim() == string.Empty)
            {
                if (firstInvalidControl == null)
                    firstInvalidControl = this.txtEstensione;

                brokenRuleList.Add(this.GetBrokenRule("MISSING_EXTENSION", "Estensione mancante"));
            }

            if (this.txtDimMaxFile.Text.Trim() != string.Empty)
            {
                int fileSize;
                if (!Int32.TryParse(this.txtDimMaxFile.Text, out fileSize))
                {
                    if (firstInvalidControl == null)
                        firstInvalidControl = this.txtDimMaxFile;

                    brokenRuleList.Add(this.GetBrokenRule("INVALID_FILE_SIZE_FORMAT", "Dimensione file non valida"));
                }
            }

            // Validazione correttezza file selezionato per l'upload
            if (this.txtEstensione.Text.Trim() != string.Empty &&
                this.uploadFile.PostedFile != null &&
                !string.IsNullOrEmpty(this.uploadFile.PostedFile.FileName))
            {
                FileInfo fileInfo = new FileInfo(this.uploadFile.PostedFile.FileName);
                string extension = this.txtEstensione.Text.Trim();
                if (fileInfo.Extension.Replace(".", "").ToUpper() != extension.ToUpper())
                {
                    if (firstInvalidControl == null)
                        firstInvalidControl = this.uploadFile;

                    brokenRuleList.Add(this.GetBrokenRule("MODEL_FILE_INVALID_FORMAT", string.Format("Il file inserito per il modello predefinito non è del formato {0}", extension)));
                }
            }

            validationResult.BrokenRules = brokenRuleList.ToArray();
            validationResult.Value = (validationResult.BrokenRules.Length == 0);

            return validationResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        protected BrokenRule GetBrokenRule(string id, string description)
        {
            BrokenRule brokenRule = new BrokenRule();
            brokenRule.ID = id;
            brokenRule.Description = description;
            brokenRule.Level = BrokenRuleLevelEnum.Error;
            return brokenRule;
        }

        /// <summary>
        /// Azione di salvataggio dati
        /// </summary>
        protected virtual void PerformActionSave()
        {
            Control firstInvalidControl;
            ValidationResultInfo result = this.ValidateData(out firstInvalidControl);

            if (result.Value)
            {
                SupportedFileType fileType = null;

                if (this.InsertMode)
                    fileType = new SupportedFileType();
                else
                    fileType = this.FileTypes[this.grdTipiFile.SelectedItem.ItemIndex];

                if (this.uploadFile.PostedFile != null &&
                    !string.IsNullOrEmpty(this.uploadFile.PostedFile.FileName))
                {
                    // Reperimento contenuto binario del file modello
                    fileType.ModelFileContent = this.GetModelFileContent();
                    fileType.ContainsFileModel = (fileType.ModelFileContent != null);
                    //if (fileType.ContainsFileModel)
                    //    this.txtMimeType.Text = this.uploadFile.PostedFile.ContentType;
                }

                fileType.IdAmministrazione = this.IdAmministrazione;
                fileType.CodiceAmministrazione = this.CodiceAmministrazione;
                fileType.FileTypeUsed = this.chkUseFileType.Checked;
                fileType.Description = this.txtDescrizione.Text;
                //fileType.MimeType = this.txtMimeType.Text;
                fileType.FileExtension = this.txtEstensione.Text;
                fileType.FileTypeSignature = this.chkUseFileTypeSignature.Checked;
                fileType.FileTypePreservation = this.chkUseFileTypePreservation.Checked;
                fileType.FileTypeValidation = this.chkUseFileTypeValidation.Checked;

                int fileSize;
                if (Int32.TryParse(this.txtDimMaxFile.Text, out fileSize))
                    fileType.MaxFileSize = fileSize;

                // Modalità avviso impostazione dimensione file
                fileType.MaxFileSizeAlertMode = (MaxFileSizeAlertModeEnum) 
                    Enum.Parse(typeof(MaxFileSizeAlertModeEnum), this.cboMaxFileSizeAlertMode.SelectedValue, true);

                // Tipo documento
                fileType.DocumentType = (DocumentTypeEnum)
                    Enum.Parse(typeof(DocumentTypeEnum), this.cboDocumentTypes.SelectedValue, true);

                result = this.WsInstance.SaveSupportedFileType(ref fileType);

                if (result.Value)
                {
                    if (this.InsertMode)
                        this.InsertMode = false;

                    // Caricamento dati
                    this.FetchData();

                    // Impostazione del nuovo tipo di file come selezionato
                    this.SetFileTypeAsSelected(fileType);

                    // Impostazione visibilità pulsante cancellazione file modello (solo se presente)
                    this.btnDeleteFileModel.Visible = fileType.ContainsFileModel;

                    this.RegisterClientScript("SetFocus", "SetFocus('" + this.txtDescrizione.ID + "');");
                }
                else
                {
                    // Save dei dati non andato a buon fine,
                    // visualizzazione messaggio di errore
                    this.ShowValidationMessage(result.BrokenRules);
                }
            }
            else
            {
                // Validazione dati non andata a buon fine
                this.ShowValidationMessage(result.BrokenRules);

                // Impostazione focus sul primo controllo non valido
                if (firstInvalidControl != null)
                    this.RegisterClientScript("SetFocus", "SetFocus('" + firstInvalidControl.ID + "');");
            }
        }

        /// <summary>
        /// Impostazione del tipo di file come selezionato
        /// </summary>
        /// <param name="fileType"></param>
        protected void SetFileTypeAsSelected(SupportedFileType fileType)
        {
            for (int i = 0; i < this.FileTypes.Length; i++)
            {
                if (fileType.SystemId.Equals(this.FileTypes[i].SystemId))
                {
                    this.grdTipiFile.SelectedIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// Azione di inserimento
        /// </summary>
        protected virtual void PerformActionNew()
        {
            this.InsertMode = true;

            this.ClearData();

            // In inserimento, il file per default è utilizzato
            this.chkUseFileType.Checked = true;
            this.chkUseFileTypeSignature.Checked = true;
            this.chkUseFileTypePreservation.Checked = true;
            this.chkUseFileTypeValidation.Checked = true;

            this.ShowDetail(true);

            this.RegisterClientScript("SetFocus", "SetFocus('" + this.txtDescrizione.ID + "');");
        }

        /// <summary>
        /// Visualizzazione messaggio di validazione
        /// </summary>
        /// <param name="brokenRules"></param>
        protected void ShowValidationMessage(BrokenRule[] brokenRules)
        {
            if (brokenRules != null && brokenRules.Length > 0)
            {
                string validationMessage = string.Empty;

                foreach (DocsPaWR.BrokenRule item in brokenRules)
                {
                    if (validationMessage != string.Empty)
                        validationMessage += @"\n";

                    validationMessage += " - " + item.Description;
                }

                this.RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RegisterJavaScriptEventHandlers()
        {
            this.btnSave.Attributes.Add("onclick", "SaveData()");
            this.btnDeleteFileModel.Attributes.Add("onclick", "if (ConfirmRemoveFileModel()) { ShowWaitPanel('Rimozione modello predefito in corso...'); return true; } else { return false; }");
        }

        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.ClientScript.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), scriptKey, scriptString);
            }
        }

        #endregion

        //
        // Metodo per controllo valore chiave PGU_FE_DISABLE_AMM_GEST_CONS
        public bool DisableAmmGestCons() 
        {
            //
            // Se la chiave di DB "PGU_FE_DISABLE_AMM_GEST_CONS" è attiva, le funzionalità di conservazione in amministrazione vengono disabilitate
            string PGU_FE_DISABLE_AMM_GEST_CONS_Value = string.Empty;

            PGU_FE_DISABLE_AMM_GEST_CONS_Value = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "PGU_FE_DISABLE_AMM_GEST_CONS");

            bool DisableAmmGestCons = false;
            DisableAmmGestCons = ((PGU_FE_DISABLE_AMM_GEST_CONS_Value.Equals("0") || string.IsNullOrEmpty(PGU_FE_DISABLE_AMM_GEST_CONS_Value)) ? false : true);

            return DisableAmmGestCons;
        }

        /// <summary>
        /// INTEGRAZIONE PITRE-PARER
        /// Determina se è attiva la conservazione PARER
        /// </summary>
        /// <returns></returns>
        public bool IsConservazionePARER()
        {
            bool result = false;

            string IS_CONSERVAZIONE_PARER = string.Empty;
            IS_CONSERVAZIONE_PARER = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_WA_CONSERVAZIONE");
            result = ((string.IsNullOrEmpty(IS_CONSERVAZIONE_PARER) || IS_CONSERVAZIONE_PARER.Equals("0")) ? false : true);

            return result;
        }
    }
}
