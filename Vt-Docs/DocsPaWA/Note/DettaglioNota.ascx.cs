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
    /// <summary>
    /// User control per la gestione del dettaglio della nota
    /// </summary>
    public partial class DettaglioNota : System.Web.UI.UserControl
    {
        protected  int caratteriDisponibili=2000 ;
        protected bool newNota = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                // Impotazione abilitazione controllo
                this.Enabled = (this.Enabled && !this.ReadOnly);
                txtAutoComplete.Enabled = false;

                 DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();
                info = UserManager.getInfoUtente(this.Page);


                string valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_NOTE");
                if (!string.IsNullOrEmpty(valoreChiave))
                    caratteriDisponibili = int.Parse(valoreChiave);

                txtNote.MaxLength = caratteriDisponibili;
                clTesto.Value = caratteriDisponibili.ToString();
                txtNote.Attributes.Add("onKeyUp", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','Descrizione Nota'," + clTesto.ClientID + ")");
                txtNote.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','Descrizione Nota'," + clTesto.ClientID + ")");

                if (this.PAGINA_CHIAMANTE.Equals("fascNewFascicolo"))
                {
                    this.txtNote.Width= 655;
                    this.txtAutoComplete.Width = 655;
                }

                if (this.PAGINA_CHIAMANTE.Equals("Protocollazione"))
                {
                    this.txtNote.Width = 655;
                }
            }
        }

        /// <summary>
        /// Flag, se true indica che i controlli grafici devono essere disabilitati
        /// indipendentemente dal valore della properietà Enabled
        /// </summary>
        private bool _mustDisableControls = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {

            this.clTesto.Value=(caratteriDisponibili-txtNote.Text.Length).ToString();
            // Gestione abilitazione / disabilitazione controlli
            this.SetControlsEnabled(!this._mustDisableControls && this.Enabled);
        }

        #region Public methods

        /// <summary>
        /// Caricamento dati
        /// </summary>
        public virtual void Fetch()
        {
            // Reperimento ultima nota
            InfoNota nota = this.NoteManager.GetUltimaNota();

            if (nota != null)
            {
                // Impostazione dell'autore dell'ultima nota
                this.AutoreNota = this.GetAutoreUltimaNota(nota);
                this.TipoVisibilita = nota.TipoVisibilita;
                this.Testo = nota.Testo;
                
                // Impostazione numero note visibili dall'utente corrente
                this.SetNoteVisibili(this.NoteManager.CountNote);
            }
            else
            {
                this.Clear();
            }

            this.IsDirtyControl = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Clear()
        {
            this.NoteManager.ClearNote();
            this.SetNoteVisibili(0);
            this.AutoreNota = string.Empty;
            this.TipoVisibilita = TipiVisibilitaNotaEnum.Tutti;
            this.Testo = string.Empty;
            this.ddl_regRF.Visible = false;
            this.txtAutoComplete.Visible = false;
        }

        /// <summary>
        /// Salvataggio dei dati
        /// </summary>
        public virtual void Save()
        {

            if (Testo.Length > 2000)
                throw new Exception("Il numero di caratteri inseriti per il campo note e superiore al limite massimo");
            else
            {
                if (this.IsDirtyControl)
                {
                    // Se la nota non contiene testo, viene ripristinato il vecchio valore
                    // e viene mostrato un messaggio di errore
                    if (String.IsNullOrEmpty(Testo.Trim()) && this.NoteManager.GetUltimaNota() != null)
                    {
                        this.txtNote.Text = this.NoteManager.GetUltimaNota().Testo;
                        throw new Exception("Attenzione. Non è possibile impostare un testo vuoto per la nota.");
                    }

                    Ruolo ruoloUtente = UserManager.getRuolo();
                    DocsPaWR.Registro[] registriRf = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "1", "");
                    // verifico se è stata selezionata una nota di RF e se si sia selezionato un RF corretto nel caso di utenti con 2 RF almeno
                     if (registriRf != null && registriRf.Length > 1 && rblTipiVisibilita.SelectedValue.Equals("RF"))
                    {
                        if (this.ddl_regRF.SelectedIndex == 0)
                            throw new Exception("Attenzione. Selezionare un RF per la Nota");
                    }


                    // Se i dati risultano modificati, viene creata una nuova nota
                    this.InsertNota();

                    this.newNota = true;

                    this.Fetch();
         
                }
            }
        }

        /// <summary>
        /// Indica se i dati risultano modificati
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public virtual bool IsDirty
        {
            get
            {
                return (this.IsDirtyControl || this.IsDirtyList);
            }
        }

        /// <summary>
        /// Indica se è stata appena inserita una nuova nota
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public bool NewNote
        {
            get
            {
                return this.newNota;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public string Width
        {
            get
            {
                return this.GetViewStateProperty<string>("WIDTH", string.Empty);
            }
            set
            {
                this.SetViewStateProperty("WIDTH", value);
            }
        }

        /// <summary>
        /// Indica che l'oggetto contenente le note è in sola lettura o meno
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public bool ReadOnly
        {
            get
            {
                return this.NoteManager.IsNoteReadOnly;
            }
        }

        /// <summary>
        /// Abilitazione / disabilitazione controllo
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public bool Enabled
        {
            get
            {
                return this.GetViewStateProperty<bool>("Enabled", true);
            }
            set
            {
                this.SetViewStateProperty("Enabled", value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public TextBoxMode TextMode
        {
            get
            {
                return this.txtNote.TextMode;
            }
            set
            {
                this.txtNote.TextMode = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public int Rows
        {
            get
            {
                return this.txtNote.Rows;
            }
            set
            {
                this.txtNote.Rows = value;
            }
        }

        /// <summary>
        /// Tipo di oggetto gestito dallo usercontrol
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public OggettiAssociazioniNotaEnum TipoOggetto
        {
            get
            {
                return this.GetViewStateProperty<OggettiAssociazioniNotaEnum>("TipoOggetto", OggettiAssociazioniNotaEnum.Documento);
            }
            set
            {
                this.SetViewStateProperty("TipoOggetto", value);
            }
        }

        /// <summary>
        /// Chiave di sessione dell'oggetto contenente le note
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public string ContainerSessionKey
        {
            get
            {
                return this.GetViewStateProperty<string>("ContainerSessionKey", string.Empty);
            }
            set
            {
                this.SetViewStateProperty("ContainerSessionKey", value);
            }
        }

        /// <summary>
        /// Tipo visibilità della nota
        /// </summary>
        [System.ComponentModel.Browsable(true)]
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
        [System.ComponentModel.Browsable(true)]
        public string Testo
        {
            get
            {
                return this.txtNote.Text;
            }
            set
            {
                this.txtNote.Text = value.Replace("\n", "\r\n");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPulsanteConferma"></param>
        public virtual void AttatchPulsanteConferma(string idPulsanteConferma)
        {
            if (!string.IsNullOrEmpty(idPulsanteConferma))
            {
                WebControl control = this.Page.Form.FindControl(idPulsanteConferma) as WebControl;

                if (control is DocsPaWebCtrlLibrary.ImageButton)
                {
                    DocsPaWebCtrlLibrary.ImageButton button = (DocsPaWebCtrlLibrary.ImageButton)control;
                    Utils.DefaultButton(this.Page, ref this.txtNote, ref button);
                }
                else if (control is Button)
                {
                    Button button = (Button)control;
                    Utils.DefaultButton(this.Page, ref this.txtNote, ref button);
                }
                else if (control is ImageButton)
                {
                    ImageButton button = (ImageButton)control;
                    Utils.DefaultButton(this.Page, ref this.txtNote, ref button);
                }
            }
        }


        private void CaricaComboRegistri(DropDownList ddl, DocsPaWR.Registro[] listaRF)
        {
            this.txtAutoComplete.Text = "";
            this.ddl_regRF.Items.Clear();
            if (listaRF != null && listaRF.Length > 0)
            {
                this.ddl_regRF.Visible = true;
                this.txtAutoComplete.Visible = true;

                if (listaRF.Length == 1)
                {
                    ListItem item = new ListItem();
                    item.Value = listaRF[0].systemId;
                    item.Text = listaRF[0].codRegistro;// +" - " + listaRF[0].descrizione.Substring(0, 14) + "...";
                    this.ddl_regRF.Items.Add(item);
                    EnableAutoComplete();
                    
                    
                }
                else
                {
                    ListItem itemVuoto = new ListItem();
                    itemVuoto.Value = "";
                    itemVuoto.Text = "Selezionare un rf ";
                    this.ddl_regRF.Items.Add(itemVuoto);
                    foreach (DocsPaWR.Registro regis in listaRF)
                    {
                        ListItem item = new ListItem();
                        item.Value = regis.systemId;
                        item.Text = regis.codRegistro;// +" - " + regis.descrizione.Substring(0, 14) + "...";
                        this.ddl_regRF.Items.Add(item);
                    }
                }
            }

           
                    
        }

        private void EnableAutoComplete()
        {
            Session.Add("RFNote", "OK^" + this.ddl_regRF.SelectedItem.Value + "^" + this.ddl_regRF.SelectedItem.Text);
            txtAutoComplete.Enabled = true;
            autoComplete1.ContextKey = ddl_regRF.SelectedItem.Value;

            if (System.Configuration.ConfigurationManager.AppSettings["AUTOCOMPLETE_MINIMUMPREFIXLENGTH"] != null)
                autoComplete1.MinimumPrefixLength = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["AUTOCOMPLETE_MINIMUMPREFIXLENGTH"].ToString());
            txtAutoComplete.Text = "";
            //txtNote.Text = "";
        }
        #endregion

        #region Protected methods

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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected virtual T GetViewStateProperty<T>(string name, T defaultValue)
        {
            if (this.ViewState[name] != null)
                return (T)this.ViewState[name];
            else
                return defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected virtual void SetViewStateProperty(string name, object value)
        {
            this.ViewState[name] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool IsDirtyControl
        {
            get
            {
                return this.GetViewStateProperty<bool>("IsDirty", false);
            }
            set
            {
                this.SetViewStateProperty("IsDirty", value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool IsDirtyList
        {
            get
            {
                return this.GetViewStateProperty<bool>("IsDirtyList", false);
            }
            set
            {
                this.SetViewStateProperty("IsDirtyList", value);
            }
        }

        /// <summary>
        /// Impostazione messaggio di visibilità delle note
        /// </summary>
        /// <param name="countNote"></param>
        protected virtual void SetNoteVisibili(int countNote)
        {
            if (countNote == 0)
                this.lblNoteDisponibili.Text = "Nessuna nota visibile";
            else
            {
                string format = string.Empty;

                if (countNote == 1)
                    format = "{0} nota visibile";
                else
                    format = "{0} note visibili";

                this.lblNoteDisponibili.Text = string.Format(format, countNote.ToString());
            }
        }

        /// <summary>
        /// Gestione abilitazione / disabilitazione controlli
        /// </summary>
        /// <param name="enabled"></param>
        protected virtual void SetControlsEnabled(bool enabled)
        {
            DocsPaWR.Registro[] registriRf = null;
            this.btnSbloccaNote.Visible = this.ReadOnly && !enabled;
            this.rblTipiVisibilita.Enabled = enabled;

            //se l'utilizzo di note RF è settato come non visibile da WebConfig il radioBtn corrispondente non sarà visibile
            if (!UserManager.isRFEnabled())
            {
                this.rblTipiVisibilita.Items.Remove(this.rblTipiVisibilita.Items.FindByValue("RF"));
            }


            if (this.rblTipiVisibilita.Enabled)
            {
                // La visibilità tutti non è consentita se l'oggetto contenitore è in readonly
                this.rblTipiVisibilita.Items.FindByValue("Tutti").Enabled = !this.ReadOnly;

                //Il bottone a selezione esclusiva "RF" è visibile solo se è stato definito almeno un RF per il ruolo dell'utente
                if (ViewState["rf"] == null)
                {
                    Ruolo ruoloUtente = UserManager.getRuolo();
                    registriRf = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "1", "");
                    if (registriRf == null || (registriRf != null && registriRf.Length == 0))
                        this.rblTipiVisibilita.Items.Remove(this.rblTipiVisibilita.Items.FindByValue("RF"));
                    else
                        ViewState.Add("rf", registriRf);
                }
                if (this.rblTipiVisibilita.Items.FindByValue("RF") != null)
                {
                    if (!UserManager.ruoloIsAutorized(this, "INSERIMENTO_NOTERF"))
                        this.rblTipiVisibilita.Items.FindByValue("RF").Enabled = false;
                    else
                    {
                        if (this.rblTipiVisibilita.Items.FindByValue("RF").Selected && !string.IsNullOrEmpty(this.txtNote.Text))
                        {
                            this.rblTipiVisibilita.Items.FindByValue("RF").Enabled = true;
                            this.ddl_regRF.Visible = true;
                            this.txtAutoComplete.Visible = true;
                            //this.pnl_noteRf.Visible = true;
                            if (ddl_regRF.Items.Count == 0)
                                CaricaComboRegistri(ddl_regRF, registriRf);
                        }
                    }
                }
            }

            this.txtNote.Enabled = enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        protected string AutoreNota
        {
            get
            {
                return this.lblAutoreNote.Text;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    this.lblAutoreNote.Text = string.Empty;
                else
                    this.lblAutoreNote.Text = string.Format("Nota di {0}", value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DataChanged(object sender, EventArgs e)
        {
            this.IsDirtyControl = true;
            this.ddl_regRF.Visible = false;
            this.txtAutoComplete.Visible = false;
            this.newNota = true;
            //pnl_noteRf.Visible = false;
            ListItem item = this.rblTipiVisibilita.Items.FindByValue("RF");
            //Se è presente il bottone di selezione esclusiva "RF" si verifica quanti sono gli
            //RF associati al ruolo dell'utente
            if (item != null && rblTipiVisibilita.SelectedIndex == 2)
            {
                DocsPaWR.Registro[] registriRf;
                if (ViewState["rf"] == null)
                {
                    Ruolo ruoloUtente = UserManager.getRuolo();
                    registriRf = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "1", "");
                }
                else
                    registriRf = (DocsPaWR.Registro[])ViewState["rf"];
                //Se un ruolo appartiene a più di un RF, allora selezionando dal menù il valore RF
                //l'utente deve selezionare su quale degli RF creare la nota
                if (registriRf != null && registriRf.Length > 0)
                {
                    //Se l'inserimento della nota avviene durante la protocollazione 
                    //ed è impostato nella segnatura il codice del RF, la selezione del RF dal quale
                    //prendere il codice sarà mantenuta valida anche per l'eventuale inserimento delle note
                    //in questo caso non si deve presentare la popup di selezione del RF
                    if (this.PAGINA_CHIAMANTE.Equals("docProtocollo"))
                    {
                        DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoInLavorazione();
                        //if (schedaDoc.protocollo == null || (schedaDoc.protocollo != null && string.IsNullOrEmpty(schedaDoc.protocollo.segnatura)))
                        //{
                        if(ddl_regRF != null && ddl_regRF.SelectedIndex == -1)
                            CaricaComboRegistri(ddl_regRF, registriRf);
                        //}
                    }
                    else
                    //if (this.PAGINA_CHIAMANTE.Equals("docProfilo") || this.PAGINA_CHIAMANTE.Equals("fascNewFascicolo") )
                    {
                        if(ddl_regRF != null && ddl_regRF.SelectedIndex == -1)
                        	CaricaComboRegistri(ddl_regRF, registriRf);
                        if (!this.PAGINA_CHIAMANTE.Equals("docProfilo"))
                        {
                            //Protocollo semplificato
                           // txtAutoComplete.Width = Unit.Pixel(620);
                            DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoInLavorazione();
                        }
                    }
                    this.txtAutoComplete.Visible = false;
                    //if (this.PAGINA_CHIAMANTE.Equals("docProtocollo") || this.PAGINA_CHIAMANTE.Equals("docProfilo"))
                    //{
                        if (UserManager.ruoloIsAutorized(this, "RICERCA_NOTE_ELENCO"))
                        {
                            this.txtAutoComplete.Visible = true;
                        }
                    //}

                }
            }


            clTesto.Value = (caratteriDisponibili - txtNote.Text.Length).ToString();
        }

        protected void RFChanged(object sender, EventArgs e)
        {
            if ((ddl_regRF.Items.Count > 1 && ddl_regRF.SelectedIndex != 0) || (ddl_regRF.Items.Count == 1))
            {
                EnableAutoComplete();
                txtNote.Text = "";
            }
            else
                Session.Add("RFNote", "");
        }


        protected void txtAutoComplete_Changed(object sender, EventArgs e)
        {
            txtNote.Text = txtAutoComplete.Text;
        }

        /// <summary>
        /// Creazione nuova nota a seguito di una modifica dei dati
        /// </summary>
        protected void InsertNota()
        {
            InfoNota nota = new InfoNota();
            nota.TipoVisibilita = this.TipoVisibilita;
            nota.Testo = this.Testo;

            //if (this.isTutti.Value.Equals("1")) { nota.TipoVisibilita = TipiVisibilitaNotaEnum.Tutti; }

            if (nota.TipoVisibilita == TipiVisibilitaNotaEnum.RF)
            {
                //if (PAGINA_CHIAMANTE.Equals("docProtocollo"))
                //{
                DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoInLavorazione();
                if (schedaDoc != null && !string.IsNullOrEmpty(schedaDoc.cod_rf_prot) && !string.IsNullOrEmpty(schedaDoc.id_rf_prot))
                    nota.IdRfAssociato = schedaDoc.id_rf_prot;
                else
                    if (Session["RFNote"] != null)
                    {
                        string[] mySplitResult = Session["RFNote"].ToString().Split('^');
                        if (mySplitResult[0] == "OK")
                            nota.IdRfAssociato = mySplitResult[1];
                    }
                //}
            }

            // Se la nota contiene del testo (vengono eliminati anche i ritorni a capo ai lati della stringa)
            if(!String.IsNullOrEmpty(this.Testo.Trim()))
                nota = this.NoteManager.InsertNota(nota);
        }

        /// <summary>
        /// Creazione url per l'apertura della finestra modale per la gestione delle note
        /// </summary>
        protected string ListaNoteModalDialogUrl
        {
            get
            {
                return string.Format("{0}/Note/ListaNote.aspx?Tipo={1}&Enabled={2}&ContainerSessionKey={3}",
                    Utils.getHttpFullPath(), this.TipoOggetto.ToString(), this.Enabled, this.ContainerSessionKey);
            }
        }

        /// <summary>
        /// Caricamento dati
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnListaNote_Click(object sender, ImageClickEventArgs e)
        {
            this.Fetch();

            this.IsDirtyList = true;

            if (this.TipoVisibilita == TipiVisibilitaNotaEnum.Tutti && this.ReadOnly)
            {
                // Indica di disabilitare solamente i controlli grafici
                this._mustDisableControls = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSbloccaNote_Click(object sender, ImageClickEventArgs e)
        {
            this.Enabled = true;

            this.SetInsertMode();
        }

        /// <summary>
        /// Reperimento dell'autore dell'ultima nota
        /// </summary>
        /// <param name="ultimaNota"></param>
        /// <returns></returns>
        protected string GetAutoreUltimaNota(InfoNota ultimaNota)
        {
            string autore = string.Empty;

            if (ultimaNota.UtenteCreatore != null)
            {

                if (!string.IsNullOrEmpty(ultimaNota.UtenteCreatore.DescrizioneUtente))
                    autore = ultimaNota.UtenteCreatore.DescrizioneUtente;

                if (autore != string.Empty && !string.IsNullOrEmpty(ultimaNota.UtenteCreatore.DescrizioneRuolo))
                    autore = string.Concat(autore, " (", ultimaNota.UtenteCreatore.DescrizioneRuolo, ")");

                if (!string.IsNullOrEmpty(ultimaNota.DescrPeopleDelegato))
                {
                    string temp = ultimaNota.DescrPeopleDelegato + "<br>Delegato da " + autore;
                    autore = temp;
                }
            }

            return autore;
        }

        /// <summary>
        /// Predisposizione alla modalità di inserimento
        /// </summary>
        protected virtual void SetInsertMode()
        {
            this.TipoVisibilita = TipiVisibilitaNotaEnum.Personale;
            this.Testo = string.Empty;
            this.SetControlFocus(this.txtNote);

            this.IsDirtyControl = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctl"></param>
        protected void SetControlFocus(Control ctl)
        {
            this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "dettaglionota_setfocus", string.Format("<script>dettaglionota_setfocus('{0}');</script>", ctl.ClientID));
        }


        public string PAGINA_CHIAMANTE
        {
            get
            {
                return this.GetStateValue("PAGINA_CHIAMANTE");
            }
            set
            {
                this.SetStateValue("PAGINA_CHIAMANTE", value);
            }
        }

        protected string GetStateValue(string key)
        {
            if (this.ViewState[key] != null)
                return this.ViewState[key].ToString();
            else
                return string.Empty;
        }

        protected void SetStateValue(string key, string obj)
        {
            this.ViewState[key] = obj;
        }



        #endregion


    }
}
