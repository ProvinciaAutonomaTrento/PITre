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
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.Note
{
    public partial class ListaNote : DocsPAWA.CssPage
    {
        protected int caratteriDisponibili = 2000;
       
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.Response.Expires = -1;

                // Impostazione pulsante di default
                //Utils.DefaultButton(this.Page, ref this.txtNote, ref this.btnSave);

                this.RegisterScrollKeeper("divGrdNote");
                
                if (!this.IsPostBack)
                {

                    DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();
                    info = UserManager.getInfoUtente(this.Page);


                    string valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_NOTE");
                    if (!string.IsNullOrEmpty(valoreChiave))
                        caratteriDisponibili = int.Parse(valoreChiave);

                    txtNote.MaxLength = caratteriDisponibili;
                    clTesto.Value = caratteriDisponibili.ToString();
                    txtNote.Attributes.Add("onKeyUp", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','Descrizione Nota'," + clTesto.ClientID + ")");
                    txtNote.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','Descrizione Nota'," + clTesto.ClientID + ")");
                    this.btnChiudi.OnClientClick = "CloseDialog();";

                    this.Bind();

                    if (this.grdNote.Items.Count > 0)
                    {
                        // Impostazione in modalità di aggiornamento
                        this.grdNote.SelectedIndex = 0;
                        this.SelectItemCommand(this.SelectedItem);
                    }
                    else
                    {
                        // Impostazione modalità di inserimento nuovo elemento
                        this.SetInsertMode();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "Page_Load");
            }
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

                this.clTesto.Value = (caratteriDisponibili - txtNote.Text.Length).ToString();
                this.SetControlsEnabled();

                if (this.grdNote.Items.Count == 0)
                    this.SetMessage("Nessuna nota trovata");
                else if (this.grdNote.Items.Count == 1)
                    this.SetMessage("Trovata 1 nota");
                else
                    this.SetMessage(string.Format("Trovate {0} note", this.grdNote.Items.Count.ToString()));
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "Page_PreRender");
            }
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

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        private const string DELETE_COMMAND = "Delete";
        private const string SELECT_COMMAND = "Select";

        /// <summary>
        /// 
        /// </summary>
        private const string INSERT_MODE_KEY = "INSERT_MODE";
        private const string IS_DIRTY_KEY = "IS_DIRTY";

        /// <summary>
        /// Indica se è possibile modificare la nota corrente
        /// </summary>
        private bool _canUpdateSelectedNota = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctl"></param>
        protected void SetControlFocus(Control ctl)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "setfocus", string.Format("<script>SetFocus('{0}');</script>", ctl.ClientID));
        }

        /// <summary>
        /// Verifica se è possibile applicare modifiche alla nota richiesta
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        protected virtual bool CanUpdateNota(InfoNota nota)
        {
            bool canUpdate = (this.Enabled && this.NoteManager.CanUpdateNota(nota));

            if (canUpdate && this.ReadOnly)
                canUpdate = (nota.TipoVisibilita != TipiVisibilitaNotaEnum.Tutti);

            return canUpdate;
        }

        /// <summary>
        /// Verifica se è possibile rimuovere la nota richiesta
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        protected bool CanDeleteNota(DocsPaWR.InfoNota nota)
        {
            bool canDelete = (this.Enabled && this.NoteManager.CanDeleteNota(nota));
            
            if (canDelete && this.ReadOnly)
                canDelete = (nota.TipoVisibilita != TipiVisibilitaNotaEnum.Tutti);

            return canDelete;
        }

        /// <summary>
        /// Reperimento descrizione dell'utente creatore della nota
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        protected string GetDescrizioneUtenteCreatore(DocsPaWR.InfoNota nota)
        {
            string retValue = string.Empty;

            if (nota != null && nota.UtenteCreatore != null)
            {
                if (!string.IsNullOrEmpty(nota.UtenteCreatore.DescrizioneUtente))
                    retValue = nota.UtenteCreatore.DescrizioneUtente;

                if (retValue != string.Empty && !string.IsNullOrEmpty(nota.UtenteCreatore.DescrizioneRuolo))
                    retValue = string.Concat(retValue, "<BR />(", nota.UtenteCreatore.DescrizioneRuolo, ")");
              
                if (!string.IsNullOrEmpty(nota.DescrPeopleDelegato))
                {
                    string temp = nota.DescrPeopleDelegato + "<br>Delegato da " + retValue;
                    retValue = temp;
                }
                
            }
            
            return retValue;
        }

        /// <summary>
        /// Tipo visibilità della nota
        /// </summary>
        public TipiVisibilitaNotaEnum TipoVisibilita
        {
            get
            {
                if (this.rblTipiVisibilita.SelectedItem != null)
                    return (TipiVisibilitaNotaEnum)Enum.Parse(typeof(TipiVisibilitaNotaEnum), this.rblTipiVisibilita.SelectedItem.Value, true);
                else
                    return TipiVisibilitaNotaEnum.Tutti;
            }
            set
            {
                this.rblTipiVisibilita.SelectedValue = value.ToString();
            }
        }

        /// <summary>
        /// Teso della nota
        /// </summary>
        public string Testo
        {
            get
            {
                return this.txtNote.Text;
            }
            set
            {
                this.txtNote.Text = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected DocsPaWR.OggettiAssociazioniNotaEnum TipoOggetto
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Request.QueryString["Tipo"]))
                    return (DocsPaWR.OggettiAssociazioniNotaEnum)Enum.Parse(typeof(DocsPaWR.OggettiAssociazioniNotaEnum), this.Request.QueryString["Tipo"], true);
                else
                    return DocsPaWR.OggettiAssociazioniNotaEnum.Documento;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ContainerSessionKey
        {
            get
            {
                return this.Request.QueryString["ContainerSessionKey"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected DocsPaWR.AssociazioneNota CreateAssociazioneNota()
        {
            DocsPaWR.AssociazioneNota associazione = new DocsPaWR.AssociazioneNota();
            associazione.TipoOggetto = this.TipoOggetto;
            return associazione;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected DocsPaWR.FiltroRicercaNote CreateFiltroRicerca()
        {
            DocsPaWR.FiltroRicercaNote filtroRicerca = new DocsPaWR.FiltroRicercaNote();
            filtroRicerca.Visibilita = this.TipoVisibilita;
            return filtroRicerca;
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool ReadOnly
        {
            get
            {
                return this.NoteManager.IsNoteReadOnly;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool Enabled
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Request.QueryString["Enabled"]))
                    return Convert.ToBoolean(this.Request.QueryString["Enabled"]);
                else
                    return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SetControlsEnabled()
        {
            this.grdNote.Visible = (this.grdNote.Items.Count > 0);

            if (!this.Enabled)
            {
                this.txtNote.ReadOnly = true;
                this.rblTipiVisibilita.Enabled = false;
                this.btnNuovo.Enabled = false;
                this.btnSave.Enabled = false;
            }
            else
            {
                bool insertMode = this.InsertMode;

                this.btnNuovo.Enabled = (!insertMode);

                if (!insertMode)
                    this.btnSave.Enabled = this._canUpdateSelectedNota;
                else
                    this.btnSave.Enabled = (insertMode || this.SelectedItem != null);

                this.rblTipiVisibilita.Enabled = this.btnSave.Enabled;

                this.txtNote.Enabled = this.btnSave.Enabled;
            }

            if (this.rblTipiVisibilita.Enabled)
            {
                //Il bottone a selezione esclusiva "RF" è visibile solo se è stato definito almeno un RF
                if (ViewState["rf"] == null)
                {
                    Ruolo ruoloUtente = UserManager.getRuolo();
                    DocsPaWR.Registro[] registriRf = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "1", "");
                    if (registriRf == null || (registriRf != null && registriRf.Length == 0))
                        this.rblTipiVisibilita.Items.Remove(this.rblTipiVisibilita.Items.FindByValue("RF"));
                    else
                        ViewState.Add("rf", registriRf);
                }
            }
        }

        /// <summary>
        /// Reperimento elemento griglia selezionato
        /// </summary>
        /// <returns></returns>
        protected DataGridItem SelectedItem
        {
            get
            {
                if (this.grdNote.Items.Count > 0 && this.grdNote.SelectedIndex > -1)
                    return this.grdNote.Items[this.grdNote.SelectedIndex];
                else
                    return null;
            }
        }

        /// <summary>
        /// Impostazione messaggio
        /// </summary>
        /// <param name="message"></param>
        protected void SetMessage(string message)
        {
            // Nessuna nota trovata
            this.lblMessaggi.Text = message;
        }

        #region Event Handler

        /// <summary>
        /// Inserimento nuova nota
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNuovo_Click(object sender, EventArgs e)
        {
            try
            {
                this.SetInsertMode();
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "btnNuovo_Click");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSalva_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNote.Text.Length > 2000)
                    throw new Exception("Il numero di caratteri inseriti per il campo note e superiore al limite massimo");
                else
                    this.Save();
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "btnSalva_Click");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void grdNote_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                switch (e.CommandName)
                {
                    case DELETE_COMMAND:
                        this.DeleteItemCommand(e.Item);
                        break;

                    case SELECT_COMMAND:
                        this.SelectItemCommand(e.Item);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "grdNote_ItemCommand");
            }
        }

        #endregion

        #region DataCommand

        /// <summary>
        /// 
        /// </summary>
        protected bool IsDirty
        {
            get
            {
                if (string.IsNullOrEmpty(this.txtReturnValue.Value))
                    return false;
                else
                    return Convert.ToBoolean(this.txtReturnValue.Value);
            }
            set
            {
                this.txtReturnValue.Value = value.ToString().ToLower();
            }
        }

        /// <summary>
        /// Associazione dati griglia
        /// </summary>
        protected void Bind()
        {
            this.grdNote.DataSource = this.NoteManager.GetNote(this.CreateFiltroRicerca());
            this.grdNote.DataBind();
        }

        /// <summary>
        /// Impostazione modalità di inserimento
        /// </summary>
        protected virtual void SetInsertMode()
        {
            this.InsertMode = true;

            // Deselezione elementi griglia
            this.UnselectItemCommand();

            this.EnableTipiVisibilita();

            // Se l'oggetto di appartenenza delle note è in stato readonly,
            // per impostazione predefinita la visibilità è personale
            if (this.ReadOnly)
                this.TipoVisibilita = TipiVisibilitaNotaEnum.Personale;
            else
                this.TipoVisibilita = TipiVisibilitaNotaEnum.Tutti;


            this.SetControlFocus(this.txtNote);
        }

        /// <summary>
        /// Indica se si è in modalità di inserimento
        /// </summary>
        protected bool InsertMode
        {
            get
            {
                if (this.ViewState[INSERT_MODE_KEY] != null)
                    return Convert.ToBoolean(this.ViewState[INSERT_MODE_KEY]);
                else
                    return false;
            }
            set
            {
                this.ViewState[INSERT_MODE_KEY] = value;
            }
        }

        /// <summary>
        /// Save dei dati
        /// </summary>
        protected virtual void Save()
        {
            if (this.InsertMode)
                this.InsertItemCommand();
            else
                this.UpdateItemCommand();

            this.IsDirty = true;
        }

        /// <summary>
        /// Reperimento istanza INoteManager corrente
        /// </summary>
        protected virtual INoteManager NoteManager
        {
            get
            {
                return NoteManagerFactory.CreateInstance(this.TipoOggetto, this.ContainerSessionKey);
            }
        }

        #region Commands

        /// <summary>
        /// 
        /// </summary>
        protected virtual void InsertItemCommand()
        {
            InfoNota nota = new InfoNota();
            nota.Testo = this.Testo;
            nota.TipoVisibilita = this.TipoVisibilita;

            // *****************************************************
            // Codice Tappabuchi. non funziona se l'utente ha più di un RF.
            // Se il tipo di visibilità è RF, impostiamo idRF al primo RF disponibile
            if (nota.TipoVisibilita == TipiVisibilitaNotaEnum.RF)
            {
                Registro[] registri = UserManager.getListaRegistriWithRF(this, "1", String.Empty);

                if (registri != null && registri.Length > 0)
                    nota.IdRfAssociato = registri[0].systemId;
                else
                    nota.IdRfAssociato = "0";
            }

            nota = NoteManager.InsertNota(nota);

            this.Bind();

            // Selezione nuovo elemento inserito
            this.grdNote.SelectedIndex = 0;
            this.SelectItemCommand(this.SelectedItem);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void UpdateItemCommand()
        {
            string idNota = this.GetId(this.SelectedItem);

            DocsPaWR.InfoNota nota = NoteManager.GetNota(idNota);
            nota.TipoVisibilita = this.TipoVisibilita;
            nota.Testo = this.Testo;
            nota = NoteManager.UpdateNota(nota);

            this.Bind();

            this.SelectItemCommand(this.SelectedItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        protected virtual void DeleteItemCommand(DataGridItem item)
        {
            NoteManager.DeleteNota(this.GetId(item));

            this.Bind();

            // Deselezione elemento
            this.UnselectItemCommand();

            this.EnableTipiVisibilita();

            this.IsDirty = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        protected virtual void SelectItemCommand(DataGridItem item)
        {
            if (item != null)
            {
                string idNota = this.GetId(item);

                if (!string.IsNullOrEmpty(idNota))
                {
                    // Reperimento nota
                    InfoNota nota = NoteManager.GetNota(idNota);

                    // Associazione dati
                    this.TipoVisibilita = nota.TipoVisibilita;
                    this.Testo = nota.Testo;

                    // Abilitazione radiobutton visibilità
                    this.EnableRadioTipiVisibilita(nota);

                    // Abilitazione del pulsante save, solo se l'utente può modificare la nota
                    this._canUpdateSelectedNota = this.CanUpdateNota(nota);
                    
                    // Modalità di inserimento a false
                    this.InsertMode = false;

                    this.SetControlFocus(this.txtNote);
                }
            }
        }

        /// <summary>
        /// Comando di deselezione dati griglia
        /// </summary>
        protected virtual void UnselectItemCommand()
        {
            this.grdNote.SelectedIndex = -1;
            this.TipoVisibilita = TipiVisibilitaNotaEnum.Tutti;
            this.Testo = string.Empty;
        }

        #endregion        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual string GetId(DataGridItem item)
        {
            return item.Cells[0].Text;
        }

        #endregion

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        private void EnableTipiVisibilita()
        {
            // Le note con visibilità "Personale" e "Ruolo" si intendo abilitate anche
            // quando il documento è in sola lettura
            this.rblTipiVisibilita.Items.FindByValue("Personale").Enabled = true;
            this.rblTipiVisibilita.Items.FindByValue("Ruolo").Enabled = true;

            //Modifica inserita perchè crasha su inps (fa enabled ma non è presente l'elemento rf)
            if (this.rblTipiVisibilita.Items.FindByValue("RF") != null)
            {
                if (UserManager.ruoloIsAutorized(this, "INSERIMENTO_NOTERF"))
                {
                    this.rblTipiVisibilita.Items.FindByValue("RF").Enabled = true;
                }
                else
                {
                    this.rblTipiVisibilita.Items.FindByValue("RF").Enabled = false;
                }
            }

            // La visibilità "Tutti" della nota è abilitata solamente se il documento non è in sola lettura
            this.rblTipiVisibilita.Items.FindByValue("Tutti").Enabled = (!this.ReadOnly);
        }

        /// <summary>
        /// Gestione abilitazione radio button visibilità
        /// </summary>
        /// <param name="nota"></param>
        private void EnableRadioTipiVisibilita(InfoNota nota)
        {
            this.EnableTipiVisibilita();

            if (nota != null)
            {
                if (nota.TipoVisibilita == TipiVisibilitaNotaEnum.Tutti)
                {
                    this.rblTipiVisibilita.Items.FindByValue("Personale").Enabled = false;
                    this.rblTipiVisibilita.Items.FindByValue("Ruolo").Enabled = false;
                    //MODICA PER CRASH SU INPS NON TROVA RF
                    if (this.rblTipiVisibilita.Items.FindByValue("RF") != null)
                    {
                        this.rblTipiVisibilita.Items.FindByValue("RF").Enabled = false;
                    }
                }
                else if (nota.TipoVisibilita == TipiVisibilitaNotaEnum.Ruolo)
                {
                    this.rblTipiVisibilita.Items.FindByValue("Personale").Enabled = false;
                    //MODICA PER CRASH SU INPS NON TROVA RF
                    if (this.rblTipiVisibilita.Items.FindByValue("RF") != null)
                    {
                        this.rblTipiVisibilita.Items.FindByValue("RF").Enabled = false;
                    }
                }
            }
        }

        #endregion
    }
}
