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
using System.Collections.Specialized;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.FascicolazioneCartacea
{
    public partial class GestioneAllineamento : DocsPAWA.CssPage
    {
        /// <summary>
        /// // Istanza ws docspa
        /// </summary>
        private DocsPaWR.DocsPaWebService _webServiceInstance = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;
            Session["Bookmark"] = "ArchCartaceo";

            getLettereProtocolli();

            if (!this.IsPostBack)
            {
                // Impostazione contesto corrente
                this.SetContext();

                this.RegisterClientScriptEvents();
                
                this.Form.DefaultButton = this.btnSearch.ClientID;

                this.chkProtocolliInterno.Visible = this.IsEnabledProtocolloInterno();
                this.chkProtocolliInterno.Checked = this.chkProtocolliInterno.Visible;

                // Inizializzazione dati di filtro
                this.InitializeFilters();

                // Ricerca documenti sui filtri impostati per default
                this.PerformSearch();
            }
        }

        /// <summary>
        /// Impostazione contesto corrente
        /// </summary>
        private void SetContext()
        {
            string url = DocsPAWA.Utils.getHttpFullPath() + "/FascicolazioneCartacea/GestioneAllineamento.aspx";

            CallContext newContext = new CallContext(NavigationKeys.GESTIONE_ARCHIVIO_CARTACEO, url);
            newContext.ContextFrameName = "top.principale";

            if (CallContextStack.SetCurrentContext(newContext))
                NavigationContext.RefreshNavigation();
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

        #region Gestione filtri

        /// <summary>
        /// Inizializzazione dati filtro
        /// </summary>
        private void InitializeFilters()
        {
            this.FetchListSnapshots(this.cboSnapshots.Items);

            DateTime now = DateTime.Now;

            this.GetCalendarControl("txtInitDataCreazione").txt_Data.Text = now.ToString("dd/MM/yyyy"); // now.AddMonths(-1).ToString("dd/MM/yyyy");
            //this.txtEndDataCreazione.Text = now.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        private void FetchListSnapshots(ListItemCollection items)
        {
            items.Clear();

            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPaWR.SnapshotDocumentiFascicolazione[] snapshotList = ws.FascCartaceaGetSnapshotList(UserManager.getInfoUtente());

            items.Add(new ListItem(string.Empty, "-1"));

            foreach (DocsPaWR.SnapshotDocumentiFascicolazione snapshot in snapshotList)
                items.Add(new ListItem(snapshot.Name, snapshot.IdSnapshot.ToString()));
        }

        /// <summary>
        /// Verifica se l'utente è abilitato alla protocollazione interna
        /// </summary>
        /// <returns></returns>
        private bool IsEnabledProtocolloInterno()
        {
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            return ws.IsInternalProtocolEnabled(UserManager.getInfoUtente().idAmministrazione);
        }

        /// <summary>
        /// Reperimento filtri correntemente impostati
        /// </summary>
        /// <returns></returns>
        private DocsPaWR.FiltroRicerca[] GetCurrentFilters()
        {
            List<DocsPaWR.FiltroRicerca> filters = new List<DocsPaWR.FiltroRicerca>();

            DocsPaWR.FiltroRicerca filter = null;

            filter = new DocsPaWR.FiltroRicerca();
            filter.argomento = "idSnapshot";
            filter.valore = this.SnapshotId.ToString();
            filters.Add(filter);

            filter = new DocsPaWR.FiltroRicerca();
            filter.argomento = "dataCreazioneIniziale";
            filter.valore = this.GetCalendarControl("txtInitDataCreazione").txt_Data.Text;
            filters.Add(filter);

            filter = new DocsPaWR.FiltroRicerca();
            filter.argomento = "dataCreazioneFinale";
            if (this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text.Trim() == string.Empty)
                filter.valore = this.GetCalendarControl("txtInitDataCreazione").txt_Data.Text;
            else
                filter.valore = this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text;
            filters.Add(filter);

            filters.Add(this.GetFiltersCheckTipoDocumento("protocolloIngresso", this.chkProtocolliIngresso));
            filters.Add(this.GetFiltersCheckTipoDocumento("protocolloUscita", this.chkProtocolliPartenza));
            if (this.chkProtocolliInterno.Visible)
                filters.Add(this.GetFiltersCheckTipoDocumento("protocolloInterno", this.chkProtocolliInterno));
            filters.Add(this.GetFiltersCheckTipoDocumento("documentoGrigio", this.chkDocumentiGrigi));

            return filters.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterType"></param>
        /// <param name="checkBox"></param>
        /// <returns></returns>
        private DocsPaWR.FiltroRicerca GetFiltersCheckTipoDocumento(string filterType, CheckBox checkBox)
        {
            DocsPaWR.FiltroRicerca filter = new DocsPaWR.FiltroRicerca();
            filter.argomento = filterType;
            filter.valore = checkBox.Checked.ToString();
            return filter;
        }


        #region Gestione eventi

        /// <summary>
        /// 
        /// </summary>
        private void RegisterClientScriptEvents()
        {
            this.btnRemoveSnapshot.Attributes.Add("onClick", "return ConfirmDeleteSnapshot();");
            this.btnSearch.Attributes.Add("onClick", "ShowWaitingPage();");
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, ImageClickEventArgs e)
        {
            if (this.cboSnapshots.Items.Count > 0)
                this.cboSnapshots.SelectedIndex = 0;

            this.PerformSearch();
        }

        /// <summary>
        /// Save della ricerca
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSaveSearch_Click(object sender, ImageClickEventArgs e)
        {
            this.PerformSaveSearch();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cboSnapshots_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SnapshotId > 0)
            {
                // Ripristino dei filtri utilizzati per la snapshot
                this.RestoreSnapshotFilters();

                this.PerformSearch();
            }
        }

        /// <summary>
        /// Handler evento pulsante per la rimozione della snapshot
        /// correntemente selezionata
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRemoveSnapshot_Click(object sender, ImageClickEventArgs e)
        {
            int snapshotId = this.SnapshotId;

            if (snapshotId > 0)
            {
                this.WebServiceInstance.FascCartaceaRemoveSnapshot(UserManager.getInfoUtente(), snapshotId);

                ListItem itemToRemove = this.cboSnapshots.Items.FindByValue(snapshotId.ToString());
                if (itemToRemove != null)
                    this.cboSnapshots.Items.Remove(itemToRemove);
            }
        }

        /// <summary>
        /// Reperimento id snapshot selezionata
        /// </summary>
        private int SnapshotId
        {
            get
            {
                int idSnapshot;
                Int32.TryParse(this.cboSnapshots.SelectedValue, out idSnapshot);
                return idSnapshot;
            }
        }

        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
        }


        /// <summary>
        /// Rimozione valori campi di filtro
        /// </summary>
        private void ClearFilterFields()
        {
            this.GetCalendarControl("txtInitDataCreazione").txt_Data.Text = string.Empty;
            this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text = string.Empty;
            this.chkProtocolliIngresso.Checked = false;
            this.chkProtocolliPartenza.Checked = false;
            if (this.chkProtocolliInterno.Visible)
                this.chkProtocolliInterno.Checked = false;
            this.chkDocumentiGrigi.Checked = false;
        }

        /// <summary>
        /// Ripristino dei filtri utilizzati per la snapshot
        /// </summary>
        private void RestoreSnapshotFilters()
        {
            // Rimozione valori campi di filtro
            this.ClearFilterFields();

            DocsPaWR.FiltroRicerca[] filtriSnapshot = this.WebServiceInstance.FascCartaceaGetSnapshotFilters(UserManager.getInfoUtente(), this.SnapshotId);

            if (filtriSnapshot != null)
            {
                foreach (DocsPaWR.FiltroRicerca filterItem in filtriSnapshot)
                {
                    if (filterItem.argomento.Equals("dataCreazioneIniziale"))
                        this.GetCalendarControl("txtInitDataCreazione").txt_Data.Text = filterItem.valore;

                    else if (filterItem.argomento.Equals("dataCreazioneFinale"))
                        this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text = filterItem.valore;
    
                    else if (filterItem.argomento.Equals("protocolloIngresso"))
                        this.chkProtocolliIngresso.Checked = filterItem.valore.Equals(bool.TrueString);

                    else if (filterItem.argomento.Equals("protocolloUscita"))
                        this.chkProtocolliPartenza.Checked = filterItem.valore.Equals(bool.TrueString);

                    else if (filterItem.argomento.Equals("protocolloInterno") && this.chkProtocolliInterno.Visible)
                        this.chkProtocolliInterno.Checked = filterItem.valore.Equals(bool.TrueString);

                    else if (filterItem.argomento.Equals("documentoGrigio")) 
                        this.chkDocumentiGrigi.Checked = filterItem.valore.Equals(bool.TrueString);
                }
            }
        }

        /// <summary>
        /// Azione di salvataggio della ricerca
        /// </summary>
        private void PerformSaveSearch()
        {
            DocsPaWR.SnapshotDocumentiFascicolazione snapshot = this.WebServiceInstance.FascCartaceaCreateSnapshot(UserManager.getInfoUtente(), this.GetCurrentFilters());
            
            if (snapshot != null)
            {
                this.FetchListSnapshots(this.cboSnapshots.Items);

                // Selezione ricerca salvata
                this.cboSnapshots.SelectedValue = snapshot.IdSnapshot.ToString();

                this.RegisterClientScript("SaveSearchCompleted", "alert('Ricerca salvata. Nome: " + snapshot.Name + "');");
            }
        }

        /// <summary>
        /// Azione di ricerca
        /// </summary>
        private void PerformSearch()
        {
            Control firstInvalidControl;

            DocsPaWR.ValidationResultInfo result = this.ValidateFilters(out firstInvalidControl);

            if (result.Value)
            {
                // Impostazione filtri correnti
                SessionManager.Filtri = this.GetCurrentFilters();

                this.DocumentiFascicolazione1.Search(this.SnapshotId);
            }
            else
            {
                this.ShowValidationMessage(result.BrokenRules);

                if (firstInvalidControl != null)
                    // impostazione del focus sul primo controllo non valido
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
        /// 
        /// </summary>
        /// <param name="validationItems"></param>
        /// <param name="firstInvalidControl"></param>
        /// <returns></returns>
        protected DocsPaWR.ValidationResultInfo ValidateFilters(out Control firstInvalidControl)
        {
            List<DocsPaWR.BrokenRule> brokenRules = new List<DocsPaWR.BrokenRule>();
            DocsPaWR.ValidationResultInfo result = new DocsPaWR.ValidationResultInfo();
            firstInvalidControl = null;

            if (this.SnapshotId == -1)
            {
                // Validazione tipo documento
                if (!this.chkDocumentiGrigi.Checked && !this.chkProtocolliIngresso.Checked &&
                    !this.chkProtocolliInterno.Checked && !this.chkProtocolliPartenza.Checked)
                {
                    brokenRules.Add(this.CreateBrokenRule("TIPO_DOCUMENTO", "Tipo documento non impostato"));
                    firstInvalidControl = this.chkProtocolliIngresso;
                }

                // Validazione data creazione
                if (this.GetCalendarControl("txtInitDataCreazione").txt_Data.Text.Trim() == string.Empty)
                {
                    brokenRules.Add(this.CreateBrokenRule("DATA_CREAZIONE", "Data creazione non impostata"));
                    if (firstInvalidControl == null)
                        firstInvalidControl = this.GetCalendarControl("txtInitDataCreazione").txt_Data;
                }
                else if (!this.IsValidDate(this.GetCalendarControl("txtInitDataCreazione").txt_Data))
                {
                     brokenRules.Add(this.CreateBrokenRule("DATA_CREAZIONE", "Data creazione non valida"));
                    if (firstInvalidControl == null)
                        firstInvalidControl = this.GetCalendarControl("txtInitDataCreazione").txt_Data;
                }

                // this.ValidateDateRange("Data creazione", this.txtInitDataCreazione, this.txtEndDataCreazione, brokenRules, ref firstInvalidControl);
            }

            result.BrokenRules = (brokenRules.ToArray());

            result.Value = (result.BrokenRules.Length == 0);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private DocsPaWR.BrokenRule CreateBrokenRule(string id, string description)
        {
            DocsPaWR.BrokenRule brokenRule = new DocsPaWR.BrokenRule();
            brokenRule.ID = id;
            brokenRule.Description = description;
            brokenRule.Level = DocsPaWR.BrokenRuleLevelEnum.Error;
            return brokenRule;
        }

        /// <summary>
        /// Validazione range di date
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="initDate"></param>
        /// <param name="endDate"></param>
        /// <param name="validationItems"></param>
        /// <param name="firstInvalidControl"></param>
        private void ValidateDateRange(string fieldName,
                                        DocsPaWebCtrlLibrary.DateMask initDate,
                                        DocsPaWebCtrlLibrary.DateMask endDate,
                                        List<DocsPaWR.BrokenRule> brokenRules,
                                        ref Control firstInvalidControl)
        {
            bool isValidInitDate = false;
            bool isValidEndDate = false;

            if (initDate.Text.Length > 0)
            {
                // Validazione data iniziale
                isValidInitDate = this.IsValidDate(initDate);

                if (!isValidInitDate)
                {
                    brokenRules.Add(this.CreateBrokenRule(fieldName, fieldName + " iniziale non valida"));

                    if (firstInvalidControl == null)
                        firstInvalidControl = initDate;
                }
            }

            if (endDate.Visible && endDate.Text.Length > 0)
            {
                // Validazione data finale
                isValidEndDate = this.IsValidDate(endDate);

                if (!isValidEndDate)
                {
                    brokenRules.Add(this.CreateBrokenRule(fieldName, fieldName + " finale non valida"));

                    if (firstInvalidControl == null)
                        firstInvalidControl = endDate;
                }
            }

            // Validazione range di dati
            if (isValidInitDate && isValidEndDate &&
                DateTime.Parse(initDate.Text) > DateTime.Parse(endDate.Text))
            {
                brokenRules.Add(this.CreateBrokenRule(fieldName, fieldName + " iniziale maggiore di quella finale"));

                if (firstInvalidControl == null)
                    firstInvalidControl = endDate;
            }
        }

        /// <summary>
        /// Validazione singola data
        /// </summary>
        /// <param name="dateMask"></param>
        /// <returns></returns>
        private bool IsValidDate(DocsPaWebCtrlLibrary.DateMask dateMask)
        {
            bool retValue = false;

            if (dateMask.Text.Length > 0)
                retValue = DocsPAWA.Utils.isDate(dateMask.Text);

            return retValue;
        }

        #endregion

        //INSERITA DA FABIO PRENDE LE ETICHETTE DEI PROTOCOLLI
        private void getLettereProtocolli()
        {
            DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
            string idAmm = cr.idAmministrazione;
            DocsPAWA.DocsPaWR.EtichettaInfo[] etichette;
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = new DocsPAWA.DocsPaWR.InfoUtente();
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            etichette = wws.getEtichetteDocumenti(infoUtente, idAmm);
            this.chkProtocolliIngresso.Text = "Protocolli in " + etichette[0].Etichetta; //Valore A
            this.chkProtocolliPartenza.Text = "Protocolli in " + etichette[1].Etichetta; //Valore P
            this.chkProtocolliInterno.Text = "Protocolli in " + etichette[2].Etichetta;//Valore I
            // this.chkDocumentiGrigi.Text = "Documenti " + etichette[3].Etichetta;//Valore I
        }
    }
}