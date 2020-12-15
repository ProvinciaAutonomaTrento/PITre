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

namespace DocsPAWA.FascicolazioneCartacea
{
    public partial class DocumentiFascicolazione : System.Web.UI.UserControl
    {
        /// <summary>
        /// Enumerazione per raggruppare le colonne della griglia dei documenti
        /// </summary>
        private enum GridColumnsEnum
        {
            Index,
            Documento,
            Tipo,
            Versione,
            Registro,
            Fascicolo,
            PresenteAltroFascicolo,
            VisualizzaFile,
            Cartaceo,
            InserisciInFascicoloCartaceo
        }

        /// <summary>
        /// // Istanza ws docspa
        /// </summary>
        private DocsPaWR.DocsPaWebService _webServiceInstance = null;

        /// <summary>
        /// Flag, se true è attivata la paginazione dei dati lato server
        /// </summary>
        private bool _serverPagingEnabled = true;

        /// <summary>
        /// Documenti correntemetne in sessione
        /// </summary>
        private List<DocsPaWR.DocumentoFascicolazione> _documenti = null;

        /// <summary>
        /// 
        /// </summary>
        private DocsPaWR.InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        private int _lastRowIdCreated = 0;

        /// <summary>
        /// 
        /// </summary>
        private System.Drawing.Color _rowColor = System.Drawing.Color.Gray;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;

            if (!this.IsPostBack)
            {
                // Attach usercontrol per gestire la clessidra e i messaggi di attesa
                this.AttatchGridPagingWaitControl();

                this.btnPrint.OnClientClick = "return PrintList();";
                this.btnSave.OnClientClick = "SaveDocuments();";
            }
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

        /// <summary>
        /// Istanza ws docspa
        /// </summary>
        private DocsPaWR.DocsPaWebService WebServiceInstance
        {
            get
            {
                if (this._webServiceInstance == null)
                    this._webServiceInstance = new DocsPaWR.DocsPaWebService();
                return this._webServiceInstance;
            }
        }

        /// <summary>
        /// Ricerca documenti
        /// </summary>
        /// <param name="idSnapshot"></param>
        public void Search(int idSnapshot)
        {
            this.IdSnapshot = idSnapshot;

            this.InitializeData();

            this.Fetch();
        }

        #region Gestione visualizzazione documenti

        /// <summary>
        /// Verifica se è abilitata la paginazione dati lato server
        /// </summary>
        private bool ServerPagingEnabled
        {
            get
            {
                return this._serverPagingEnabled;
            }
        }

        /// <summary>
        /// Caricamento dati
        /// </summary>
        private void Fetch()
        {
            int totalRecordCount = this.TotalRecordCount;

            // Caricamento e impostazione documenti in sessione
            List<DocsPaWR.DocumentoFascicolazione> documenti = this.GetDocumentiFascicolazione(ref totalRecordCount);

            // Impostazione numero record totali
            this.TotalRecordCount = totalRecordCount;

            this.Documenti = documenti;

            // Aggirnamento dati
            this.Refresh();
        }

        /// <summary>
        /// Numero di record totali estratti
        /// </summary>
        private int TotalRecordCount
        {
            get
            {
                if (this.ViewState["TotalRecordCount"] != null)
                    return Convert.ToInt32(this.ViewState["TotalRecordCount"]);
                else
                    return 0;
            }
            set
            {
                this.ViewState["TotalRecordCount"] = value;
            }
        }

        /// <summary>
        /// Impostazione visibilità campi UI
        /// </summary>
        private void SetFieldsVisibility()
        {
            bool hasRows = (this.grdDocumentiFascicolazione.Items.Count > 0);

            this.btnPrint.Visible = hasRows;
            this.btnSave.Visible = hasRows;
            this.grdDocumentiFascicolazione.Visible = hasRows;
        }

        /// <summary>
        /// Aggiornamento dati
        /// </summary>
        private void Refresh()
        {
            List<DocsPaWR.DocumentoFascicolazione> documenti = this.Documenti;

            DataSet ds = this.DocumentiFascicolazioneToDataSet(documenti);

            this.grdDocumentiFascicolazione.VirtualItemCount = this.TotalRecordCount;
            this.grdDocumentiFascicolazione.DataSource = ds;
            this.grdDocumentiFascicolazione.DataBind();

            this.SetCountMessage();

            // Impostazione visibilità campi UI
            this.SetFieldsVisibility();
        }

        /// <summary>
        /// Impostazione messaggio numero documenti trovati
        /// </summary>
        private void SetCountMessage()
        {
            int count = this.grdDocumentiFascicolazione.VirtualItemCount;

            string message = string.Empty;

            if (count == 0)
            {
                message = "Nessun documento trovato";
            }
            else
            {
                if (count == 1)
                    message = "Trovato 1 documento";
                else
                    message = "Trovati " + count.ToString() + " documenti";
            }

            this.SetMessage(message);
        }

        /// <summary>
        /// Validazione dati correnti
        /// </summary>
        /// <param name="firstInvalidControl"></param>
        /// <returns></returns>
        private DocsPaWR.ValidationResultInfo ValidateData(out Control firstInvalidControl)
        {
            List<DocsPaWR.BrokenRule> brokenRuleList = new List<DocsPaWR.BrokenRule>();

            DocsPaWR.ValidationResultInfo retValue = new DocsPaWR.ValidationResultInfo();

            firstInvalidControl = null;

            bool almostOneChecked = false;

            foreach (DataGridItem item in this.grdDocumentiFascicolazione.Items)
            {
                HtmlInputCheckBox chkDocumentoCartaceo = this.GetCheckDocumentoCartaceo(item);
                HtmlInputCheckBox chkInsFascCartaceo = this.GetCheckInsFascCartaceo(item);

                almostOneChecked = (!chkDocumentoCartaceo.Disabled && 
                                    chkDocumentoCartaceo.Checked ||
                                    chkInsFascCartaceo.Checked);

                if (almostOneChecked)
                {
                    firstInvalidControl = null;
                    break;
                }
                else if (firstInvalidControl == null)
                {
                    firstInvalidControl = chkDocumentoCartaceo;
                }
            }

            if (!almostOneChecked)
            {
                DocsPaWR.BrokenRule brokenRule = new DocsPaWR.BrokenRule();
                brokenRule.ID = "NESSUN_DOCUMENTO_SELEZIONATO";
                brokenRule.Description = "Nessun documento selezionato";
                brokenRule.Level = DocsPaWR.BrokenRuleLevelEnum.Error;
                brokenRuleList.Add(brokenRule);
            }

            retValue.BrokenRules = brokenRuleList.ToArray();

            retValue.Value = (retValue.BrokenRules.Length == 0);

            return retValue;
        }

        /// <summary>
        /// Save dei documenti selezionati
        /// </summary>
        public void Save()
        {
            DocsPaWR.ValidationResultInfo result = null;

            Control firstInvalidControl;

            result = this.ValidateData(out firstInvalidControl);
            
            if (result.Value)
            {
                List<DocsPaWR.DocumentoFascicolazione> dirtyDocuments = new List<DocsPaWR.DocumentoFascicolazione>();

                foreach (DataGridItem item in this.grdDocumentiFascicolazione.Items)
                {
                    HtmlInputCheckBox chkDocumentoCartaceo = this.GetCheckDocumentoCartaceo(item);
                    HtmlInputCheckBox chkInsFascCartaceo = this.GetCheckInsFascCartaceo(item);

                    DocsPaWR.DocumentoFascicolazione documento = this.GetDocumento(item);

                    if (chkInsFascCartaceo.Checked && !documento.InsertInFascicoloCartaceo)
                    {
                        // Il documento deve essere inserito in fascicolo cartaceo
                        documento.InsertInFascicoloCartaceo = true;
                        documento.IsDirty = true;
                    }
                    else if (chkDocumentoCartaceo.Checked && !documento.Cartaceo)
                    {
                        // Il documento ha un corrispondente documento cartaceo
                        documento.Cartaceo = true;
                        documento.IsDirty = true;
                    }

                    if (documento.IsDirty)
                    {
                        // Inserimento del documento tra quelli modificati
                        dirtyDocuments.Add(documento);
                    }
                }

                if (dirtyDocuments.Count > 0)
                {
                    DocsPaWR.DocumentoFascicolazione[] documenti = dirtyDocuments.ToArray();

                    result = this.WebServiceInstance.FascCartaceaSaveDocumentiFascicolazione(this.InfoUtente, this.IdSnapshot, ref documenti);

                    // Verifica del numero di documenti inseriti in fascicolo cartaceo
                    int countInserted = 0;
                    foreach (DocsPaWR.DocumentoFascicolazione documento in documenti)
                        if (documento.IdFascicolazione > 0)
                            countInserted++;

                    // Visualizzazione messaggi di errore
                    if (result.BrokenRules.Length > 0)
                    {
                        this.ShowValidationMessage(result.BrokenRules);
                    }

                    if (countInserted > 0)
                    {
                        // Se il numero di documenti inseriti in fascicolo cartaceo è > 0,
                        // è necessario ricaricare la lista dei documenti per la pagina dal server
                        this.TotalRecordCount -= countInserted;

                        // Se almeno un documento è stato inserito in fascicolo cartaceo,
                        // vengono ricaricati i documenti per la pagina corrente
                        int page = this.grdDocumentiFascicolazione.CurrentPageIndex + 1;

                        if (page > 1)
                        {
                            int newPageCount = (this.TotalRecordCount / this.grdDocumentiFascicolazione.PageSize);
                            if ((this.TotalRecordCount % this.grdDocumentiFascicolazione.PageSize) > 0)
                                newPageCount++;

                            if (page > newPageCount)
                                this.grdDocumentiFascicolazione.CurrentPageIndex = newPageCount - 1;
                        }
                    }

                    this.Fetch();
                }
            }
            else
            {
                this.ShowValidationMessage(result.BrokenRules);

                // impostazione del focus sul primo controllo non valido
                if (firstInvalidControl != null)
                    firstInvalidControl.Focus();
            }
        }

        /// <summary>
        /// Visualizzazione messaggio di validazione
        /// </summary>
        /// <param name="brokenRules"></param>
        private void ShowValidationMessage(DocsPaWR.BrokenRule[] brokenRules)
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

        /// <summary>
        /// 
        /// </summary>
        private void InitializeData()
        {
            this.Documenti = null;
            this.grdDocumentiFascicolazione.CurrentPageIndex = 0;
            this.TotalRecordCount = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        protected DocsPaWR.InfoUtente InfoUtente
        {
            get
            {
                if (this._infoUtente == null)
                    this._infoUtente = UserManager.getInfoUtente();
                return this._infoUtente;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected CheckBox CheckBoxSelectAllInsCartaceo
        {
            get
            {
                CheckBox retValue = null;

                if (this.grdDocumentiFascicolazione.Items.Count > 0 &&
                    this.grdDocumentiFascicolazione.Items[0].ItemType == ListItemType.Header)
                {
                    DataGridItem item = this.grdDocumentiFascicolazione.Items[0];

                    retValue = item.Cells[(int)GridColumnsEnum.InserisciInFascicoloCartaceo].FindControl("chkSelectAllInsFascCartaceo") as CheckBox;

                }

                return retValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documenti"></param>
        /// <returns></returns>
        private DataSet DocumentiFascicolazioneToDataSet(List<DocsPaWR.DocumentoFascicolazione> documenti)
        {
            DataSet ds = new DataSet();

            DataTable table = new DataTable();

            table.Columns.Add("Index", typeof(int));
            table.Columns.Add("IdProfile", typeof(int));
            table.Columns.Add("DocNumber", typeof(int));
            table.Columns.Add("VersionId", typeof(int));
            table.Columns.Add("VersionLabel", typeof(string));
            table.Columns.Add("VersionNote", typeof(string));
            table.Columns.Add("DataCreazione", typeof(DateTime));
            table.Columns.Add("IdFascicolo", typeof(int));
            table.Columns.Add("CodiceFascicolo", typeof(string));
            table.Columns.Add("DescrizioneFascicolo", typeof(string));
            table.Columns.Add("FascicoloCartaceo", typeof(bool));
            table.Columns.Add("DocumentoCartaceo", typeof(bool));
            table.Columns.Add("NumeroProtocollo", typeof(int));
            table.Columns.Add("DataProtocollo", typeof(DateTime));
            table.Columns.Add("Documento", typeof(string));
            table.Columns.Add("TipoDocumento", typeof(string));
            table.Columns.Add("Versione", typeof(string));
            table.Columns.Add("Registro", typeof(string));
            table.Columns.Add("Fascicolo", typeof(string));

            ds.Tables.Add(table);

            int pageSize = this.grdDocumentiFascicolazione.PageSize;
            int startRow = 0;

            if (!this.ServerPagingEnabled)
                startRow = (this.grdDocumentiFascicolazione.CurrentPageIndex * pageSize);

            for (int i = startRow; (i < startRow + pageSize) && (i < documenti.Count); i++)
            {
                DocsPaWR.DocumentoFascicolazione item = documenti[i];

                DataRow row = table.NewRow();

                row["Index"] = i;
                row["IdProfile"] = item.IdProfile;
                row["DocNumber"] = item.DocNumber;
                row["VersionId"] = item.VersionId;
                row["VersionLabel"] = item.VersionLabel;
                row["VersionNote"] = item.VersionNote;
                row["DataCreazione"] = item.DataCreazione;
                row["IdFascicolo"] = item.Fascicolo.IdFascicolo;
                row["CodiceFascicolo"] = item.Fascicolo.CodiceFascicolo;
                row["DescrizioneFascicolo"] = item.Fascicolo.DescrizioneFascicolo;
                row["FascicoloCartaceo"] = item.Fascicolo.Cartaceo;
                row["DocumentoCartaceo"] = item.Cartaceo;

                row["NumeroProtocollo"] = item.NumeroProtocollo;
                row["DataProtocollo"] = item.DataProtocollo;
                row["TipoDocumento"] = item.TipoDocumento;

                string documento = string.Empty;

                if (item.NumeroProtocollo > 0)
                {
                    documento = item.NumeroProtocollo + "<br />" +
                                item.DataProtocollo.ToString("dd/MM/yyyy");
                }
                else
                {
                    documento = item.DocNumber + "<br />" +
                                item.DataCreazione.ToString("dd/MM/yyyy");
                }
                row["Documento"] = documento;
                row["Versione"] = item.VersionLabel.ToString();
                row["Registro"] = item.CodiceRegistro;
                row["Fascicolo"] = item.Fascicolo.CodiceFascicolo;
                table.Rows.Add(row);
            }

            return ds;
        }

        /// <summary>
        /// Id dell'immagine della ricerca salvata
        /// </summary>
        private int IdSnapshot
        {
            get
            {
                int retValue = 0;

                if (this.ViewState["IdSnapshot"] != null)
                    Int32.TryParse(this.ViewState["IdSnapshot"].ToString(), out retValue);

                return retValue;
            }
            set
            {
                this.ViewState["IdSnapshot"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalRecordCount"></param>
        /// <returns></returns>
        private List<DocsPaWR.DocumentoFascicolazione> GetDocumentiFascicolazione(ref int totalRecordCount)
        {
            DocsPaWR.DocumentoFascicolazione[] documenti = null;

            if (this.ServerPagingEnabled)
            {
                documenti = this.WebServiceInstance.FascCartaceaGetDocumentiFascicolazionePaging(this.InfoUtente, SessionManager.Filtri, (this.grdDocumentiFascicolazione.CurrentPageIndex + 1), this.grdDocumentiFascicolazione.PageSize, ref totalRecordCount);
            }
            else
            {
                documenti = this.WebServiceInstance.FascCartaceaGetDocumentiFascicolazione(UserManager.getInfoUtente(), SessionManager.Filtri);
                totalRecordCount = documenti.Length;
            }

            return new List<DocsPAWA.DocsPaWR.DocumentoFascicolazione>(documenti);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdDocumentiFascicolazione_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.DataItem != null)
            {
                TableCell cellDocumento = e.Item.Cells[(int)GridColumnsEnum.Documento];

                cellDocumento.Font.Bold = true;

                if (this.GetDocumento(e.Item).NumeroProtocollo > 0)
                    cellDocumento.ForeColor = System.Drawing.Color.Red;
                else
                    cellDocumento.ForeColor = System.Drawing.Color.Black;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdDocumentiFascicolazione_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CheckBox chkSelectAllInsCartaceo = e.Item.Cells[(int)GridColumnsEnum.InserisciInFascicoloCartaceo].FindControl("chkSelectAllInsFascCartaceo") as CheckBox;
                if (chkSelectAllInsCartaceo != null)
                    chkSelectAllInsCartaceo.Attributes.Add("onClick", "SelectAllCheckInserisci('" + chkSelectAllInsCartaceo.ClientID + "')");
            }

            if (e.Item.ItemType == ListItemType.Pager && e.Item.Cells.Count > 0)
                e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdDocumentiFascicolazione_PreRender(object sender, EventArgs e)
        {
            foreach (DataGridItem item in this.grdDocumentiFascicolazione.Items)
            {
                this.SetItemBackColor(item);

                this.SetHandlerCheckFascicoliButton(item);

                this.SetHandlerShowFileButton(item);

                this.BindCheckDocCartaceo(item);

                this.ShowButtonScartaDocumentoNonCartaceo(item);
            }
        }

        /// <summary>
        /// Impostazione visibilità pulsante scarta documento non cartaceo
        /// </summary>
        /// <param name="item"></param>
        private void ShowButtonScartaDocumentoNonCartaceo(DataGridItem item)
        {
            DocsPaWR.DocumentoFascicolazione documento = this.GetDocumento(item);
            ImageButton button = item.FindControl("btnDiscardDocument") as ImageButton;
            if (button != null)
            {
                // Il pulsante è visibile solo se il documento non è cartaceo
                button.Visible = (!documento.Cartaceo);
                button.OnClientClick = "return OnClickScartaDocumento();";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void grdDocumentiFascicolazione_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            this.grdDocumentiFascicolazione.CurrentPageIndex = e.NewPageIndex;

            if (this.ServerPagingEnabled)
                this.Fetch();
            else
                this.Refresh();
        }

        /// <summary>
        /// Restituzione dei documenti in sessione
        /// </summary>
        protected List<DocsPaWR.DocumentoFascicolazione> Documenti
        {
            get
            {
                if (this._documenti == null)
                    this._documenti = SessionManager.Documenti;
                return this._documenti;
            }
            set
            {
                if (this._documenti != null)
                    this._documenti = null;

                this._documenti = value;
                SessionManager.Documenti = this._documenti;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected Label LabelMessageControl
        {
            get
            {
                return this.lblMessage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected int GetItemIndex(DataGridItem item)
        {
            int index;
            if (int.TryParse(item.Cells[(int)GridColumnsEnum.Index].Text, out index))
                return index;
            else
                return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected DocsPaWR.DocumentoFascicolazione GetDocumento(DataGridItem item)
        {
            int index = this.GetItemIndex(item);

            if (index > -1)
                return this.Documenti[index];
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void BindCheckDocCartaceo(DataGridItem item)
        {
            HtmlInputCheckBox chkDocumentoCartaceo = this.GetCheckDocumentoCartaceo(item);

            if (chkDocumentoCartaceo != null)
            {
                DocsPaWR.DocumentoFascicolazione documento = this.GetDocumento(item);

                chkDocumentoCartaceo.Checked = documento.Cartaceo;
                // Il check documento cartaceo è abilitato solo se il documento non è cartaceo,
                // altrimenti è disabilitato e checked
                chkDocumentoCartaceo.Disabled = chkDocumentoCartaceo.Checked;

                HtmlInputCheckBox chkInsFascCartaceo = this.GetCheckInsFascCartaceo(item);

                // Abilitazione / disabilitazione check di inserimento in cartaceo
                chkInsFascCartaceo.Disabled = (!chkDocumentoCartaceo.Checked);
                // Impostazione valore check di inserimento in cartaceo
                chkInsFascCartaceo.Checked = (documento.IdFascicolazione > 0);

                chkDocumentoCartaceo.Attributes.Add("onClick", "OnClickDocumentoCartaceo('" + chkDocumentoCartaceo.ClientID + "', '" + chkInsFascCartaceo.ClientID + "');");
            }
        }

        /// <summary>
        /// Attach usercontrol per gestire la clessidra e i messaggi di attesa
        /// </summary>
        private void AttatchGridPagingWaitControl()
        {
            DataGridPagingWaitControl.DataGridID = this.grdDocumentiFascicolazione.ID;
            DataGridPagingWaitControl.WaitScriptCallback = "WaitDataGridCallback();";
        }

        /// <summary>
        /// 
        /// </summary>
        private waiting.DataGridPagingWait DataGridPagingWaitControl
        {
            get
            {
                return this.FindControl("DataGridPagingWait1") as waiting.DataGridPagingWait;
            }
        }

        /// <summary>
        /// Reperimento checkbox "cartaceo" per la riga del datagrid richiesta
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private HtmlInputCheckBox GetCheckDocumentoCartaceo(DataGridItem item)
        {
            return item.Cells[(int)GridColumnsEnum.Cartaceo].FindControl("chkDocumentoCartaceo") as HtmlInputCheckBox;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private HtmlInputCheckBox GetCheckInsFascCartaceo(DataGridItem item)
        {
            return item.Cells[(int)GridColumnsEnum.InserisciInFascicoloCartaceo].FindControl("chkInsFascCartaceo") as HtmlInputCheckBox;
        }

        /// <summary>
        /// Impostazione handler evento onclick per il pulsante "VerificaFascicoli"
        /// </summary>
        /// <param name="item"></param>
        private void SetHandlerCheckFascicoliButton(DataGridItem item)
        {
            ImageButton btnCheckFascicoli = item.Cells[(int)GridColumnsEnum.PresenteAltroFascicolo].FindControl("btnCheckFascicoli") as ImageButton;

            if (btnCheckFascicoli != null)
            {
                DocsPaWR.DocumentoFascicolazione documento = this.GetDocumento(item);

                btnCheckFascicoli.Attributes.Add("onClick", "CheckFascicoli('" + documento.VersionId.ToString() + "'); return false;");
            }
        }

        /// <summary>
        /// Impostazione handler evento onclick per il pulsante "ShowFile"
        /// </summary>
        /// <param name="item"></param>
        private void SetHandlerShowFileButton(DataGridItem item)
        {
            ImageButton btnShowFile = item.Cells[(int)GridColumnsEnum.VisualizzaFile].FindControl("btnShowFile") as ImageButton;

            if (btnShowFile != null)
            {
                DocsPaWR.DocumentoFascicolazione documento = this.GetDocumento(item);

                btnShowFile.Attributes.Add("onClick", "return ShowFile('" + documento.VersionId.ToString() + "');");
            }
        }

        /// <summary>
        /// Impostazione backcolor per la riga del datagrid:
        /// la logica è che il colore di sfondo cambia non appena
        /// si cambia l'identificativo del documento
        /// </summary>
        /// <param name="item"></param>
        private void SetItemBackColor(DataGridItem item)
        {
            int rowId;

            DocsPaWR.DocumentoFascicolazione documento = this.GetDocumento(item);

            if (this._lastRowIdCreated != documento.IdProfile)
            {
                this._lastRowIdCreated = documento.IdProfile;

                // grigion #d9d9d9  217 217 217
                // grigioa #4b4b4b  242

                if (this._rowColor == System.Drawing.Color.FromArgb(242, 242, 242))
                    this._rowColor = System.Drawing.Color.FromArgb(217, 217, 217);
                else
                    this._rowColor = System.Drawing.Color.FromArgb(242, 242, 242);
            }

            item.BackColor = this._rowColor;
        }

        /// <summary>
        /// Impostazione messaggio
        /// </summary>
        /// <param name="message"></param>
        protected void SetMessage(string message)
        {
            this.lblMessage.Text = message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void grdDocumentiFascicolazione_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "DISCARD_DOCUMENT")
            {
                // Comando di "Scarta documento non cartaceo"
                DocsPaWR.DocumentoFascicolazione documento = this.Documenti[e.Item.ItemIndex];
                this.DiscardDocument(documento);
            }
        }

        /// <summary>
        /// Operazione di scarto di un documento non cartaceo
        /// </summary>
        /// <param name="documento"></param>
        protected void DiscardDocument(DocsPaWR.DocumentoFascicolazione documento)
        {
            if (!documento.Cartaceo)
            {
                int fascicoliScartati;
                DocsPaWR.ValidationResultInfo result = this.WebServiceInstance.FascCartaceaScartaDocumentoNonCartaceo(this.InfoUtente, this.IdSnapshot, documento, out fascicoliScartati);
                
                if (!result.Value)
                {
                    // Operazione non andata a buon fine, visualizzazione del messaggio di errore
                    this.ShowValidationMessage(result.BrokenRules);
                }
                else
                {
                    this.TotalRecordCount -= fascicoliScartati;

                    // Se almeno un documento è stato inserito in fascicolo cartaceo,
                    // vengono ricaricati i documenti per la pagina corrente
                    int page = this.grdDocumentiFascicolazione.CurrentPageIndex + 1;

                    if (page > 1)
                    {
                        int newPageCount = (this.TotalRecordCount / this.grdDocumentiFascicolazione.PageSize);
                        if ((this.TotalRecordCount % this.grdDocumentiFascicolazione.PageSize) > 0)
                            newPageCount++;

                        if (page > newPageCount)
                            this.grdDocumentiFascicolazione.CurrentPageIndex = newPageCount - 1;
                    }

                    this.Fetch(); 
                }
            }
        }

        protected void btnSave_Click(object sender, ImageClickEventArgs e)
        {
            // Save dei dati
            this.Save();
        }

        #endregion

    }
}