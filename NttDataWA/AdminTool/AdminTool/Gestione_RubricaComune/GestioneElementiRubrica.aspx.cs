using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using RubricaComune;
using RubricaComune.Proxy.Elementi;
using SAAdminTool.AdminTool.Manager;
using SAAdminTool.utils;

namespace SAAdminTool.AdminTool.Gestione_RubricaComune
{
    /// <summary>
    /// 
    /// </summary>
    public partial class GestioneElementiRubrica : System.Web.UI.Page
    {
        #region Handler eventi

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
                Session["AdminBookmark"] = "RubricaComune";

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

                if (!this.IsPostBack)
                {
                    this.lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");

                    this.grdElementiRubrica.CurrentPageIndex = 0;

                    // Inizializzazione della lista dei tipi corrispondenti
                    this.InitializeCorrType();

                    // Caricamento dati lista
                    this.FetchList();

                }
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// Metodo per l'inizializzazione della DropDown list dei tipi corrispondente
        /// </summary>
        private void InitializeCorrType()
        {
            this.ddlCorrType.Items.Add(new ListItem("Unità Organizzativa", Tipo.UO.ToString()));
                    
            // Se sono attivi gli RF, viene aggiunta anche la possibilità di inserire corrispondenti
            // di tipo RF
            AmministrazioneManager ammoinistrazioneManager = new AmministrazioneManager();
            if (ammoinistrazioneManager.IsEnabledRF(null))
                this.ddlCorrType.Items.Add(new ListItem("Raggruppamento Funzionale", Tipo.RF.ToString()));
                
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                // Gestione abilitazione / disabilitazione controlli
                this.SetControlsEnabled();

                // Impostazione visibilità controlli
                this.SetControlsVisibility();

                // Impostazione della descrizione del numero di record visualizzati
                this.SetRecordCountDescription();
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cboFiltri_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkInteroperante_CheckedChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Chiave del comando di update
        /// </summary>
        private const string EDIT_COMMAND = "EDIT_COMMAND";

        /// <summary>
        /// Chiave del comando di delete
        /// </summary>
        private const string DELETE_COMMAND = "DELETE_COMMAND";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void grdElementiRubrica_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == EDIT_COMMAND)
                {
                    // Azione di selezione dati
                    this.SelectAction(e.Item);
                }
                else if (e.CommandName == DELETE_COMMAND)
                {
                    // Azione di rimozione dati
                    this.DeleteAction(e.Item);
                }
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void grdElementiRubrica_SortCommand(object source, DataGridSortCommandEventArgs e)
        {   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void grdElementiRubrica_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            try
            {
                this.grdElementiRubrica.CurrentPageIndex = e.NewPageIndex;

                this.FetchList();
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdElementiRubrica_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                this.SetInsertMode();
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClosePanel_OnClick(object sender, EventArgs e)
        {
            try
            {
                this.ClearSelection();

                this.ShowDetail(false);
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.SaveAction();
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// Caricamento dati
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnFiltro_Click(object sender, EventArgs e)
        {
            try
            {
                this.grdElementiRubrica.CurrentPageIndex = 0;

                // Caricamento dati lista
                this.FetchList();
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        #region Eventi Multi Casella Corrispondenti esterni

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvCaselle_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && Convert.ToBoolean(ViewState["readOnlyMail"]))
            {
                (e.Row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = true;
                (e.Row.FindControl("txtNoteMailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = true;
                (e.Row.FindControl("rdbPrincipale") as System.Web.UI.WebControls.RadioButton).Enabled = false;
                (e.Row.FindControl("imgEliminaCasella") as System.Web.UI.WebControls.ImageButton).Enabled = false;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow && (!Convert.ToBoolean(ViewState["readOnlyMail"])))
            {
                (e.Row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = false;
                (e.Row.FindControl("txtNoteMailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = false;
                (e.Row.FindControl("rdbPrincipale") as System.Web.UI.WebControls.RadioButton).Enabled = true;
                (e.Row.FindControl("imgEliminaCasella") as System.Web.UI.WebControls.ImageButton).Enabled = true;
            }
        }

        /// <summary>
        /// Evento elimina casella di posta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void imgEliminaCasella_Click(object sender, ImageClickEventArgs e)
        {
            bool isComboMain = (((sender as System.Web.UI.WebControls.ImageButton).Parent.Parent as System.Web.UI.WebControls.GridViewRow).
                        FindControl("rdbPrincipale") as RadioButton).Checked;
            //se presenti più caselle e si tenta di eliminare una casella settata come principale il sistema avvisa l'utente
            if (isComboMain && (ViewState["gvCaselle"] as List<EmailInfo>).Count > 1)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningPrincipale", "<script>alert('Prima di eliminare una casella " +
                    "definita come principale è necessario impostare una nuova casella principale !');</script>", false);
                return;
            }
            int indexRowDelete = ((sender as System.Web.UI.WebControls.ImageButton).Parent.Parent as System.Web.UI.WebControls.GridViewRow).RowIndex;
            (ViewState["gvCaselle"] as List<EmailInfo>).RemoveAt(indexRowDelete);
            gvCaselle.DataSource = (List<EmailInfo>)ViewState["gvCaselle"];
            gvCaselle.DataBind();
        }

        /// <summary>
        /// Aggiunge la mail al datagrid multicasella
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void imgAggiungiCasella_Click(object sender, ImageClickEventArgs e)
        {
            //verifico che l'indirizzo non sia vuoto
            if (string.IsNullOrEmpty(this.txtEmail.Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningInserisciIndirizzo", "<script>alert('Immettere una casella di posta elettronica valida!');</script>", false);
                return;
            }

            //verifico il formato dell'indirizzo mail
            string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
            if (!System.Text.RegularExpressions.Regex.Match(this.txtEmail.Text, pattern).Success)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningInserisciIndirizzo", "<script>alert('Formato casella di posta non valido!');</script>", false);
                return;
            }

            //verifico che la casella non sia già stata associata al corrispondente     
            foreach (EmailInfo  c in (List<EmailInfo>)ViewState["gvCaselle"])
            {
                if (c.Email.Trim().Equals(this.txtEmail.Text.Trim()))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningIndirizzoPresente", "<script>alert('La casella che si sta tentando di inserire è già presente!');</script>", false);
                    return;
                }
            }
            (ViewState["gvCaselle"] as List<EmailInfo>).Add(new EmailInfo()
            {
                Email = this.txtEmail.Text,
                Note = this.txtNote.Text,
                Preferita = gvCaselle.Rows.Count < 1 ? true : false
            });
            gvCaselle.DataSource = (List<EmailInfo>)ViewState["gvCaselle"];
            gvCaselle.DataBind();

            //PULISCO I CAMPI EMAIL/NOTE EMAIL
            this.txtEmail.Text = string.Empty;
            this.txtNote.Text = string.Empty;
        }

        /// <summary>
        /// Evento selezione casella mail
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rdbPrincipale_ChecekdChanged(object sender, EventArgs e)
        {
            string mailSelect = (((sender as RadioButton).Parent.Parent as System.Web.UI.WebControls.GridViewRow).FindControl("txtEmailCorr") as TextBox).Text;
            List<EmailInfo> listCaselle = ViewState["gvCaselle"] as List<EmailInfo>;
            foreach (EmailInfo c in listCaselle)
            {
                if (c.Email.Trim().Equals(mailSelect.Trim()))
                    c.Preferita = true;
                else
                    c.Preferita = false;
            }
            ViewState["gvCaselle"] = listCaselle as List<EmailInfo>;
            gvCaselle.DataSource = ViewState["gvCaselle"];
            gvCaselle.DataBind();
        }

        /// <summary>
        /// Evento change mail
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtEmailCorr_TextChanged(object sender, EventArgs e)
        {
            string newMail = (sender as System.Web.UI.WebControls.TextBox).Text;
            int rowModify = ((sender as System.Web.UI.WebControls.TextBox).Parent.Parent as System.Web.UI.WebControls.GridViewRow).RowIndex;
            (ViewState["gvCaselle"] as List<EmailInfo>)[rowModify].Email = newMail;
        }

        /// <summary>
        /// Evento change note email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtNoteMailCorr_TextChanged(object sender, EventArgs e)
        {
            string newNote = (sender as System.Web.UI.WebControls.TextBox).Text;
            int rowModify = ((sender as System.Web.UI.WebControls.TextBox).Parent.Parent as System.Web.UI.WebControls.GridViewRow).RowIndex;
            (ViewState["gvCaselle"] as List<EmailInfo>)[rowModify].Note = newNote;
        }
        #endregion

        #endregion

        #region Gestione dati

        /// <summary>
        /// Istanza servizi rubrica
        /// </summary>
        private ElementiRubricaServices _rubricaServices = null;

        /// <summary>
        /// Caricamento dati lista
        /// </summary>
        protected virtual void FetchList()
        {
            this.ClearSelection();

            int totaleOggetti;
            this.grdElementiRubrica.DataSource = this.GetElementiRubrica(out totaleOggetti);
            this.grdElementiRubrica.VirtualItemCount = totaleOggetti;
            this.DataBind();
        }

        /// <summary>
        /// Caricamento dati elemento rubrica
        /// </summary>
        /// <param name="elemento"></param>
        protected void FetchDetail(ElementoRubrica elemento)
        {
            EnableInsertMail(elemento);
            this.txtCodice.Text = elemento.Codice;
            this.txtDescrizione.Text = elemento.Descrizione;
            this.txtTelefono.Text = elemento.Telefono;
            this.txtFax.Text = elemento.Fax;
            this.txtCodiceFiscale.Text = elemento.CodiceFiscale;
            this.txtPartitaIva.Text = elemento.PartitaIva;
            this.txtCap.Text = elemento.Cap;
            this.txtIndirizzo.Text = elemento.Indirizzo;
            this.txtCitta.Text = elemento.Citta;
            this.txtNazione.Text = elemento.Nazione;
            this.txtProvincia.Text = elemento.Provincia;
            this.txtAmministrazione.Text = elemento.Amministrazione;
            this.txtAOO.Text = elemento.AOO;
            this.txtUrl.Text = elemento.Urls != null && elemento.Urls.Length > 0 ? elemento.Urls[0].Url : String.Empty;
            this.ddlCorrType.SelectedValue = elemento.TipoCorrispondente.ToString();
            this.txtEmail.Text = string.Empty;
            this.txtNote.Text = string.Empty;
            this.txtCodice.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            this.txtDescrizione.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            this.txtTelefono.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            this.txtFax.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            this.txtCodiceFiscale.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            this.txtPartitaIva.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            this.txtCap.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            this.txtIndirizzo.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            this.txtCitta.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            this.txtNazione.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            this.txtProvincia.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            this.txtAmministrazione.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            this.txtAOO.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            this.txtUrl.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            this.ddlCorrType.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            txtEmail.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            txtNote.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            imgAggiungiCasella.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;
            this.btnSave.Enabled = Convert.ToBoolean(ViewState["readOnlyMail"]) ? false : true;

            BindGridViewCaselle(elemento);
        }

        /// <summary>
        /// Rimozione contesto di selezione corrente
        /// </summary>
        protected void ClearSelection()
        {
            this.grdElementiRubrica.SelectedIndex = -1;

            this.ShowDetail(false);

            this.ClearDetail();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ClearDetail()
        {
            this.txtCodice.Text = string.Empty;
            this.txtDescrizione.Text = string.Empty;
            this.txtTelefono.Text = string.Empty;
            this.txtFax.Text = string.Empty;
            this.txtCodiceFiscale.Text = string.Empty;
            this.txtPartitaIva.Text = string.Empty;
            this.txtCap.Text = string.Empty;
            this.txtIndirizzo.Text = string.Empty;
            this.txtCitta.Text = string.Empty;
            this.txtNazione.Text = string.Empty;
            this.txtProvincia.Text = string.Empty;
            this.txtAmministrazione.Text = string.Empty;
            this.txtAOO.Text = string.Empty;
            this.txtUrl.Text = String.Empty;
            this.txtEmail.Text = string.Empty;
            this.txtNote.Text = string.Empty;
            this.ddlCorrType.SelectedIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elemento"></param>
        protected bool RefreshElementoRubrica(ElementoRubrica elemento)
        {
            bool save = true;
            if (!String.IsNullOrEmpty(this.txtUrl.Text) && !Uri.IsWellFormedUriString(this.txtUrl.Text, UriKind.Absolute))
                throw new ArgumentException("Inserire un indirizzo URL valido.");

            if (this.InsertMode)
                elemento.Codice = this.txtCodice.Text;

            elemento.Descrizione = this.txtDescrizione.Text;
            elemento.Telefono = this.txtTelefono.Text;
            elemento.Fax = this.txtFax.Text;
            elemento.CodiceFiscale = this.txtCodiceFiscale.Text;
            elemento.PartitaIva = this.txtPartitaIva.Text;
            elemento.Cap = this.txtCap.Text;
            elemento.Indirizzo = this.txtIndirizzo.Text;
            elemento.Citta = this.txtCitta.Text;
            elemento.Nazione = this.txtNazione.Text;
            elemento.Provincia = this.txtProvincia.Text;
            elemento.Amministrazione = this.txtAmministrazione.Text;
            elemento.AOO = this.txtAOO.Text;
            elemento.TipoCorrispondente = (Tipo)Enum.Parse(typeof(Tipo), this.ddlCorrType.SelectedValue);
            //se ho inserito una mail valida e la lista delle caselle è vuota, allora inserisco questa mail
            if (((ViewState["gvCaselle"] as List<EmailInfo>) == null || (ViewState["gvCaselle"] as List<EmailInfo>).Count < 1) &&
                !string.IsNullOrEmpty(this.txtEmail.Text))
            {
                //verifico il formato dell'indirizzo mail
                string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                if (!System.Text.RegularExpressions.Regex.Match(this.txtEmail.Text, pattern).Success)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningInserisciIndirizzo", "<script>alert('Formato casella di posta non valido!');</script>", false);
                    return !save;
                }
                (ViewState["gvCaselle"] as List<EmailInfo>).Add(new EmailInfo()
                {
                    Email = this.txtEmail.Text,
                    Note = this.txtNote.Text,
                    Preferita = gvCaselle.Rows.Count < 1 ? true : false
                });
            }
            elemento.Urls = new UrlInfo[] { new UrlInfo() { Url = this.txtUrl.Text } };
            elemento.Emails = (ViewState["gvCaselle"] as List<EmailInfo>).ToArray();
            return save;
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool InsertMode
        {
            get
            {
                if (this.ViewState["InsertMode"] != null)
                    return Convert.ToBoolean(this.ViewState["InsertMode"]);
                else
                    return false;
            }
            set
            {
                this.ViewState["InsertMode"] = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void SetInsertMode()
        {
            this.InsertMode = true;

            this.grdElementiRubrica.SelectedIndex = -1;

            this.ClearDetail();

            this.ShowDetail(true);

            this.SetControlFocus(this.txtCodice.ClientID);
            ViewState["readOnlyMail"] = false;
            ViewState["gvCaselle"] = new List<EmailInfo>();
            gvCaselle.DataSource = ViewState["gvCaselle"];
            gvCaselle.DataBind();
        }

        /// <summary>
        /// Reperimento descrizione per il numero di record correntemente visualizzati
        /// </summary>
        /// <returns></returns>
        protected virtual string GetRecordCountDescription()
        {
            return string.Format("{0} oggetti visualizzati", this.grdElementiRubrica.VirtualItemCount);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SetRecordCountDescription()
        {
            this.lblOggettiVisualizzati.Text = this.GetRecordCountDescription();
        }

        /// <summary>
        /// Gestione abilitazione / disabilitazione controlli
        /// </summary>
        protected virtual void SetControlsEnabled()
        {
            this.txtFiltro.Enabled = (this.cboFiltri.SelectedItem != null &&
                          this.cboFiltri.SelectedItem.Value != string.Empty);

            this.txtCodice.Enabled = this.InsertMode;
        }

        /// <summary>
        /// Impostazione visibilità controlli
        /// </summary>
        protected virtual void SetControlsVisibility()
        {
            this.grdElementiRubrica.Visible = (this.grdElementiRubrica.Items.Count > 0);
        }

        #region Azioni 

        /// <summary>
        /// Azione di selezione di un elemento
        /// </summary>
        /// <param name="item"></param>
        protected virtual void SelectAction(DataGridItem item)
        {
            if (item != null)
            {
                this.InsertMode = false;

                this.grdElementiRubrica.SelectedIndex = item.ItemIndex;

                // Caricamento dati del dettaglio correntemente selezionato
                this.FetchDetail(this.GetElementoRubricaSelezionato());

                this.ShowDetail(true);

                this.SetControlFocus(this.txtDescrizione.ClientID);
            }
        }

        /// <summary>
        /// Azione di rimozione di un elemento
        /// </summary>
        /// <param name="item"></param>
        protected virtual void DeleteAction(DataGridItem item)
        {
            if (item != null)
            {
                this.RubricaServices.Delete(this.RubricaServices.Get(this.GetIdElementoRubrica(item)));

                this.grdElementiRubrica.CurrentPageIndex = 0;

                this.FetchList();
            }
        }

        #region Validazione dati di input

        /// <summary>
        /// Visualizzazione messaggio di validazione
        /// </summary>
        /// <param name="brokenRules"></param>
        protected void ShowValidationMessage(string[] brokenRules)
        {
            if (brokenRules.Length > 0)
            {
                string validationMessage = string.Empty;

                foreach (string item in brokenRules)
                {
                    if (validationMessage != string.Empty)
                        validationMessage += @"\n";
                    validationMessage += " - " + item;
                }

                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ValidationMessage", "<script>alert('" + validationMessage + "');</script>");
            }
        }

        #endregion

        /// <summary>
        /// Azione di save dei dati di un elemento
        /// </summary>
        protected virtual void SaveAction()
        {
            ElementoRubrica elemento = null;

            if (this.InsertMode)
                elemento = new ElementoRubrica();
            else
                elemento = this.GetElementoRubricaSelezionato();

            // Aggiornamento attributi oggetto
            if (!this.RefreshElementoRubrica(elemento))
                return;
            if (this.InsertMode)
            {
                // Inserimento
                elemento = this.RubricaServices.Insert(elemento);

                // Impostazione della prima pagina come pagina corrente
                this.grdElementiRubrica.CurrentPageIndex = 0;

                this.InsertMode = false;
            }
            else
            {
                // Modifica
                elemento = this.RubricaServices.Update(elemento);
            }

            // Caricamento dati pagina corrente
            this.FetchList();
        }

        #endregion

        /// <summary>
        /// Reperimento elmenti rubrica per la pagina corrente
        /// </summary>
        /// <param name="totaleOggetti"></param>
        protected ElementoRubrica[] GetElementiRubrica(out int totaleOggetti)
        {
            OpzioniRicerca opzioniRicerca = this.GetOpzioniRicerca();

            ElementoRubrica[] elementi = this.RubricaServices.Search(ref opzioniRicerca);

            // Reperimento del numero totale di oggetti estratti dalla query
            totaleOggetti = opzioniRicerca.CriteriPaginazione.TotaleOggetti;

            return elementi;
        }

        /// <summary>
        /// Reperimento elemento rubrica selezionato in lista
        /// </summary>
        /// <returns></returns>
        protected virtual ElementoRubrica GetElementoRubricaSelezionato()
        {
            // Reperimento id elemento selezionato
            int idElemento = this.GetIdElementoRubrica(this.grdElementiRubrica.SelectedItem);

            if (idElemento > 0)
                // Reperimento oggetto ElementoRubrica
                return this.RubricaServices.Get(idElemento);
            else
                return null;
        }

        /// <summary>
        /// Reperimento id dell'elemento rubrica selezionato in lista
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected int GetIdElementoRubrica(DataGridItem item)
        {
            int id = 0;
            if (item != null)
                Int32.TryParse(item.Cells[0].Text, out id);
            return id;
        }

        /// <summary>
        /// Reperimento istanza servizi rubrica
        /// </summary>
        protected ElementiRubricaServices RubricaServices
        {
            get
            {
                if (this._rubricaServices == null)
                {
                    AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();

                    this._rubricaServices = RubricaComune.RubricaServices.GetElementiRubricaServiceInstance(sessionManager.getUserAmmSession());
                }

                return this._rubricaServices;
            }
        }

        /// <summary>
        /// Reperimento oggetto OpzioniRicerca
        /// </summary>
        /// <returns></returns>
        protected OpzioniRicerca GetOpzioniRicerca()
        {
            OpzioniRicerca opzioni = new OpzioniRicerca();
            opzioni.CriteriPaginazione = this.GetCriteriPaginazione();
            opzioni.CriteriRicerca = this.GetCriteriRicerca();
            opzioni.CriteriOrdinamento = this.GetCriteriOrdinamento();
            return opzioni;
        }

        /// <summary>
        /// Reperimento oggetto CriteriPaginazione
        /// </summary>
        /// <returns></returns>
        protected CriteriPaginazione GetCriteriPaginazione()
        {
            CriteriPaginazione criteri = new CriteriPaginazione();
            criteri.Pagina = (this.grdElementiRubrica.CurrentPageIndex + 1);
            criteri.OggettiPerPagina = this.grdElementiRubrica.PageSize;
            return criteri;
        }

        /// <summary>
        /// Reperimento criteri di ricerca
        /// </summary>
        /// <returns></returns>
        protected CriteriRicerca GetCriteriRicerca()
        {
            CriteriRicerca criteri = new CriteriRicerca();

            if (this.cboFiltri.SelectedValue != string.Empty && this.txtFiltro.Text != string.Empty)
            {
                CriterioRicerca criterio = new CriterioRicerca();
                criterio.Nome = this.cboFiltri.SelectedValue.ToString();
                criterio.Valore = this.txtFiltro.Text;
                criterio.TipoRicerca = TipiRicercaParolaEnum.ParteDellaParola;
                criteri.Criteri = new CriterioRicerca[1] { criterio };
            }

            return criteri;
        }

        /// <summary>
        /// Reperimento criteri di ordinamento nella ricerca
        /// </summary>
        /// <returns></returns>
        protected CriteriOrdinamento GetCriteriOrdinamento()
        {
            return new CriteriOrdinamento();
        }


        /// <summary>
        /// Impostazione messaggio di errore
        /// </summary>
        /// <param name="errorMessage"></param>
        private void ShowErrorMessage(string errorMessage)
        {
            this.RegisterClientScript("ErrorMessage", "alert('" + errorMessage.Replace("'", "\\'") + "')");
        }

        /// <summary>
        /// Visualizzazione dei dati del dettaglio
        /// </summary>
        /// <param name="visible"></param>
        protected void ShowDetail(bool visible)
        {
            this.tblDetail.Visible = visible;

            // L'URL deve essere visibile solo se è attiva l'interoperabilità semplificata
            bool isEnabledIS = InteroperabilitaSemplificataManager.IsEnabledSimpInterop;
            this.txtUrl.Visible = isEnabledIS;
            this.lblUrl.Visible = isEnabledIS;

        }

        #region Gestione Multi casella

        /// <summary>
        /// Bind caselle email associate al corrispondente
        /// </summary>
        /// <param name="corr"></param>
        protected void BindGridViewCaselle(ElementoRubrica corr)
        {
            if (corr.Emails != null && corr.Emails.Length > 0)
                 ViewState["gvCaselle"] = new List<EmailInfo>(corr.Emails);
            else
                ViewState["gvCaselle"] = new List<EmailInfo>();
            gvCaselle.DataSource = ViewState["gvCaselle"];
            gvCaselle.DataBind();
        }

        /// <summary>
        /// true se consentito accesso writable, false readonly
        /// </summary>
        protected void EnableInsertMail(ElementoRubrica elem)
        {
            if (elem.CHA_Pubblicato.Equals("1"))
                ViewState["readOnlyMail"] = true;
            else
                ViewState["readOnlyMail"] = false;
        }
        #endregion

        #endregion

        #region Gestione JavaScript

        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        protected void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.ClientScript.IsStartupScriptRegistered(this.GetType(), scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, scriptString);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlClientId"></param>
        protected void SetControlFocus(string controlClientId)
        {
            this.RegisterClientScript("SetFocus", string.Format("SetFocus('{0}');", controlClientId));
        }

        #endregion
    }
}
