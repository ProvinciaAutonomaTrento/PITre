using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class Note : System.Web.UI.Page
    {

        private const string DELETE_COMMAND = "Delete";
        private const string SELECT_COMMAND = "Select";

        #region Properties

        protected int MaxLenghtNote
        {
            get
            {
                int result = 2000;
                if (HttpContext.Current.Session["MaxLenghtNote"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["MaxLenghtNote"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["MaxLenghtNote"] = value;
            }
        }

        private SchedaDocumento DocumentInWorking
        {
            get
            {
                SchedaDocumento result = null;
                if (HttpContext.Current.Session["document"] != null)
                {
                    result = HttpContext.Current.Session["document"] as SchedaDocumento;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["document"] = value;
            }
        }

        private DocsPaWR.InfoUtente InfoUser
        {
            get
            {
                DocsPaWR.InfoUtente result = null;
                if (HttpContext.Current.Session["infoUser"] != null)
                {
                    result = HttpContext.Current.Session["infoUser"] as DocsPaWR.InfoUtente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["infoUser"] = value;
            }
        }

        private GridViewRow SelectedItem
        {
            get
            {
                GridViewRow result = null;
                if (HttpContext.Current.Session["SelectedItem"] != null)
                {
                    result = HttpContext.Current.Session["SelectedItem"] as GridViewRow;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SelectedItem"] = value;
            }
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
        /// Note del documento
        /// </summary>
        protected InfoNota[] Notes
        {
            get
            {
                if (this.TypeCaller.Equals("D"))
                {
                    if (this.DocumentInWorking.noteDocumento == null)
                        return new InfoNota[0];
                    else
                        return this.DocumentInWorking.noteDocumento;
                }
                else
                {
                    if (UIManager.ProjectManager.getProjectInSession() == null)
                        return new InfoNota[0];
                    else
                        return UIManager.ProjectManager.getProjectInSession().noteFascicolo;
                }
            }
            set
            {
                if (this.TypeCaller.Equals("D"))
                {
                    this.DocumentInWorking.noteDocumento = value;
                }
                else
                {
                    UIManager.ProjectManager.getProjectInSession().noteFascicolo = value;
                }
            }
        }

        /// <summary>
        /// Verifica se il documento è in stato readonly
        /// </summary>
        public bool IsNoteReadOnly
        {
            get
            {
                if (this.TypeCaller.Equals("D"))
                {
                    return !string.IsNullOrEmpty(this.DocumentInWorking.accessRights) && ( this.DocumentInWorking.accessRights == ((int)HMdiritti.HDdiritti_Waiting).ToString() || Int32.Parse(this.DocumentInWorking.accessRights) < ((int)HMdiritti.HMdiritti_Read));
                }
                else
                {
                    if (UIManager.ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(UIManager.ProjectManager.getProjectInSession().systemID))
                    {
                        return UIManager.ProjectManager.getProjectInSession().stato.Equals("C") || UIManager.ProjectManager.getProjectInSession().accessRights == ((int)HMdiritti.HDdiritti_Waiting).ToString() || Int32.Parse(UIManager.ProjectManager.getProjectInSession().accessRights) < ((int)HMdiritti.HMdiritti_Read);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool Enabled
        {
            get
            {
                bool result = true;
                if (TypeCaller.Equals("D"))
                {
                    //elimino il controllo per documenti consolidati 
                    //if ((this.DocumentInWorking!=null && this.DocumentInWorking.ConsolidationState!=null && this.DocumentInWorking.ConsolidationState.State == DocumentConsolidationStateEnum.Step2) || (!string.IsNullOrEmpty(this.DocumentInWorking.inCestino) && this.DocumentInWorking.inCestino == "1")) result = false;
                    if (this.DocumentInWorking != null && !string.IsNullOrEmpty(this.DocumentInWorking.inCestino) && this.DocumentInWorking.inCestino == "1") result = false;
                }
                return result;
            }
        }

        protected int AutocompleteMinimumPrefixLength
        {
            get
            {
                int result = 3;
                if (HttpContext.Current.Session["AutocompleteMinimumPrefixLength"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["AutocompleteMinimumPrefixLength"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["AutocompleteMinimumPrefixLength"] = value;
            }
        }

        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["type"]))
                    {
                        this.TypeCaller = Request.QueryString["type"].ToString();
                    }

                    this.InitPage();
                    if (this.TypeCaller.Equals("D"))
                    {
                        this.ControlAbortDocument();
                    }

                    this.Bind();

                    if (this.grdNote.Rows.Count > 0)
                    {
                        // Impostazione in modalità di aggiornamento
                        //this.grdNote.SelectedIndex = 0;
                        //this.SelectedItem = this.grdNote.Rows[this.grdNote.SelectedIndex];
                        //this.SelectItemCommand(this.SelectedItem);

                        this.grdNote.SelectedIndex = -1;
                        this.SelectedItem = null;
                        this.SelectItemCommand(this.SelectedItem);
                    }
                    else
                    {
                        // Impostazione modalità di inserimento nuovo elemento
                        this.SetInsertMode();
                    }

                }

                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ControlAbortDocument()
        {
            this.AbortDocument = false;
            if (TypeCaller.Equals("D"))
            {
                if (this.DocumentInWorking != null && !string.IsNullOrEmpty(this.DocumentInWorking.systemId) && (this.DocumentInWorking.tipoProto.ToUpper().Equals("A") || this.DocumentInWorking.tipoProto.ToUpper().Equals("P") || this.DocumentInWorking.tipoProto.ToUpper().Equals("I")))
                {
                    if (this.DocumentInWorking != null && this.DocumentInWorking.tipoProto != null && this.DocumentInWorking.protocollo.protocolloAnnullato != null)
                    {
                        this.BtnNew.Enabled = false;
                        this.BtnSave.Enabled = false;
                        this.rblTipiVisibilita.Enabled = false;
                        this.txtNoteAutoComplete.Enabled = false;
                        this.AbortDocument = true;
                    }

                }
            }
        }

        private void InitPage()
        {
            this.InitLanguage();
            this.LoadKeys();
            if (!UIManager.AdministrationManager.IsEnableRF(UIManager.UserManager.GetUserInSession().idAmministrazione))
            {
                this.rblTipiVisibilita.Items.RemoveAt(2);
            }
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.litTitle.Text = Utils.Languages.GetLabelFromCode("NotesTitle", language);
            this.DocumentLitVisibleNotesChars.Text = Utils.Languages.GetLabelFromCode("DocumentLitVisibleNotesChars", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("NotesBtnClose", language);
            this.BtnNew.Text = Utils.Languages.GetLabelFromCode("NotesBtnNew", language);
            this.BtnSave.Text = Utils.Languages.GetLabelFromCode("NotesBtnSave", language);
            this.optPersonal.Text = Utils.Languages.GetLabelFromCode("NotesOptPersonal", language);
            this.optRole.Text = Utils.Languages.GetLabelFromCode("NotesOptRole", language);
            this.optRF.Text = Utils.Languages.GetLabelFromCode("NotesOptRF", language);
            this.optAll.Text = Utils.Languages.GetLabelFromCode("NotesOptAll", language);
        }

        protected void LoadKeys()
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]))
            {
                this.AutocompleteMinimumPrefixLength = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]);
            }
        }

        public string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        public string GetMessage(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetMessageFromCode(id, language);
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshDate", "DatePicker();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshHour", "FormatHour();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshNoteChars", "charsLeft('TxtNote', " + this.MaxLenghtNote + ", '" + this.DocumentLitVisibleNotesChars.Text.Replace("'", "\'") + "');", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen_deselect", "$('.chzn-select-deselect').chosen({ allow_single_deselect: true, no_results_text: '" + utils.FormatJs(this.GetLabel("GenericNoResults")) + "' });", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen", "$('.chzn-select').chosen({ no_results_text: '" + utils.FormatJs(this.GetLabel("GenericNoResults")) + "' });", true);
            this.TxtNote_chars.Attributes["rel"] = "TxtNote_" + this.MaxLenghtNote + "_" + this.DocumentLitVisibleNotesChars.Text;
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try {
                this.CloseMask();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void CloseMask()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "parent.closeAjaxModal('Note', 'true', parent);", true);
        }

        /// <summary>
        /// Associazione dati griglia
        /// </summary>
        protected void Bind()
        {
            this.grdNote.Columns[1].HeaderText = this.GetLabel("NotesText");
            this.grdNote.Columns[2].HeaderText = this.GetLabel("NotesUser");
            this.grdNote.Columns[3].HeaderText = this.GetLabel("NotesDate");
            this.grdNote.Columns[4].HeaderText = this.GetLabel("NotesVisibility");

            InfoNota[] source = this.GetNote(this.CreateFiltroRicerca());
            this.grdNote.DataSource = source;
            this.grdNote.DataBind();

            this.UpPnlGrid.Update();

            if (this.grdNote.Rows.Count == 0)
                this.SetMessage(this.GetLabel("NotesNoneResult"));
            else if (this.grdNote.Rows.Count == 1)
                this.SetMessage(this.GetLabel("NotesOneResult"));
            else
                this.SetMessage(this.GetLabel("NotesMoreResult").Replace("@@", this.grdNote.Rows.Count.ToString()));


            this.SetControlsEnabled();
        }

        protected void grdNote_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[0].Attributes["onclick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "'); $('#BtnSelect').click();";
                    e.Row.Cells[1].Attributes["onclick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "'); $('#BtnSelect').click();";
                    e.Row.Cells[2].Attributes["onclick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "'); $('#BtnSelect').click();";
                    e.Row.Cells[3].Attributes["onclick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "'); $('#BtnSelect').click();";
                    e.Row.Cells[4].Attributes["onclick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "'); $('#BtnSelect').click();";
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnSelect_Click(object sender, EventArgs e)
        {
            try {
                int index = int.Parse(this.grid_rowindex.Value);

                this.grdNote.SelectedIndex = index;
                this.SelectedItem = this.grdNote.Rows[index];

                this.Bind();
                this.SelectItemCommand(this.SelectedItem);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try {
                int index = int.Parse(this.grid_rowindex.Value);

                this.grdNote.SelectedIndex = index;
                this.SelectedItem = this.grdNote.Rows[index];
                this.DeleteItemCommand(this.SelectedItem);

                this.SetControlsEnabled();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected DocsPaWR.FiltroRicercaNote CreateFiltroRicerca()
        {
            DocsPaWR.FiltroRicercaNote filtroRicerca = new DocsPaWR.FiltroRicercaNote();
            filtroRicerca.Visibilita = this.TipoVisibilita;
            return filtroRicerca;
        }

        /// <summary>
        /// Reperimento della lista delle note associate ad un documento / fascicolo
        /// </summary>
        /// <param name="filtroRicerca"></param>
        /// <returns></returns>
        public InfoNota[] GetNote(FiltroRicercaNote filtroRicerca)
        {
            List<InfoNota> note = new List<InfoNota>();
            foreach (InfoNota item in this.Notes)
                if (!item.DaRimuovere)
                    note.Add(item);
            return note.ToArray();
        }

        /// <summary>
        /// Impostazione messaggio
        /// </summary>
        /// <param name="message"></param>
        protected void SetMessage(string message)
        {
            // Nessuna nota trovata
            this.litMessage.Text = message;
        }

        /// <summary>
        /// Impostazione modalità di inserimento
        /// </summary>
        protected virtual void SetInsertMode()
        {
            // Deselezione elementi griglia
            this.UnselectItemCommand();
            this.SetControlsEnabled();
            this.EnableTipiVisibilita();
            this.rblTipiVisibilita_SelectedIndexChanged(null, null);

            // Se l'oggetto di appartenenza delle note è in stato readonly,
            // per impostazione predefinita la visibilità è personale
            if (this.IsNoteReadOnly)
                this.TipoVisibilita = TipiVisibilitaNotaEnum.Personale;
            else
                this.TipoVisibilita = TipiVisibilitaNotaEnum.Tutti;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "TxtNoteFocus", "$('#TxtNote').focus();", true);
        }

        /// <summary>
        /// Comando di deselezione dati griglia
        /// </summary>
        protected virtual void UnselectItemCommand()
        {
            this.grdNote.SelectedIndex = -1;
            this.SelectedItem = null;
            this.TipoVisibilita = TipiVisibilitaNotaEnum.Tutti;
            this.TxtNote.Text = string.Empty;
        }

        private void EnableTipiVisibilita()
        {
            // Le note con visibilità "Personale" e "Ruolo" si intendo abilitate anche
            // quando il documento è in sola lettura
            this.rblTipiVisibilita.Items.FindByValue("Personale").Enabled = true;
            this.rblTipiVisibilita.Items.FindByValue("Ruolo").Enabled = true;

            //Modifica inserita perchè crasha su inps (fa enabled ma non è presente l'elemento rf)
            if (this.rblTipiVisibilita.Items.FindByValue("RF") != null)
            {
                if (UserManager.IsAuthorizedFunctions("INSERIMENTO_NOTERF"))
                {
                    this.rblTipiVisibilita.Items.FindByValue("RF").Enabled = true;
                }
                else
                {
                    this.rblTipiVisibilita.Items.FindByValue("RF").Enabled = false;
                }
            }

            // La visibilità "Tutti" della nota è abilitata solamente se il documento non è in sola lettura
            this.rblTipiVisibilita.Items.FindByValue("Tutti").Enabled = (!this.IsNoteReadOnly);
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
                    retValue = string.Concat(retValue, "<br />(", nota.UtenteCreatore.DescrizioneRuolo, ")");

                if (!string.IsNullOrEmpty(nota.DescrPeopleDelegato))
                {
                    string temp = nota.DescrPeopleDelegato + "<br />" + Utils.Languages.GetLabelFromCode("DocumentNoteAuthorDelegatedBy", UIManager.UserManager.GetUserLanguage()) + " " + retValue;
                    retValue = temp;
                }

            }

            return retValue;
        }

        /// <summary>
        /// Verifica se è possibile applicare modifiche alla nota richiesta
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        protected virtual bool CanUpdateNota(InfoNota nota)
        {
            bool canUpdate = (this.Enabled && !nota.SolaLettura && !this.AbortDocument);

            if (canUpdate && this.IsNoteReadOnly)
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
            bool canDelete = (this.Enabled && !nota.SolaLettura && !this.AbortDocument);

            if (canDelete && this.IsNoteReadOnly)
                canDelete = (nota.TipoVisibilita != TipiVisibilitaNotaEnum.Tutti);

            return canDelete;
        }

        protected void DeleteItemCommand(GridViewRow item)
        {
            this.SelectedItem = item;
            this.DeleteNota(this.GetId(this.SelectedItem));

            this.grid_rowindex.Value = "-1";
            this.grdNote.SelectedIndex = -1;
            this.SelectedItem = null;

            this.Bind();

            this.UnselectItemCommand();
            this.EnableTipiVisibilita();
        }

        protected void SelectItemCommand(GridViewRow item)
        {
            if (item != null)
            {
                string idNota = this.GetId(item);

                if (!string.IsNullOrEmpty(idNota))
                {
                    // Reperimento nota
                    InfoNota nota = this.GetNota(idNota);
                    if (nota != null)
                    {
                        // Associazione dati
                        this.TipoVisibilita = nota.TipoVisibilita;
                        this.TxtNote.Text = nota.Testo;

                        // Abilitazione radiobutton visibilità
                        this.EnableRadioTipiVisibilita(nota);
                        this.rblTipiVisibilita_SelectedIndexChanged(null, null);
                        if (this.rblTipiVisibilita.SelectedValue == "RF")
                        {
                            this.ddlNoteRF.SelectedIndex = -1;

                            foreach (ListItem lItem in this.ddlNoteRF.Items)
                                if (lItem.Value == nota.IdRfAssociato)
                                    lItem.Selected = true;
                        }
                    }
                }
            }
        }

        protected string GetId(GridViewRow item)
        {
            if (item != null)
                return item.Cells[0].Text;
            else
                return string.Empty;
        }

        /// <summary>
        /// Reperimento di una nota esistente, solo se non è marcata come da rimuovere
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idNota"></param>
        /// <returns></returns>
        public InfoNota GetNota(string idNota)
        {
            foreach (InfoNota nota in this.Notes)
                if (nota.Id.Equals(idNota) && !nota.DaRimuovere)
                    return nota;
            return null;
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
                switch (nota.TipoVisibilita)
                {
                    case TipiVisibilitaNotaEnum.Tutti:
                        this.rblTipiVisibilita.Items.FindByValue("Personale").Enabled = false;
                        this.rblTipiVisibilita.Items.FindByValue("Ruolo").Enabled = false;
                        //MODICA PER CRASH SU INPS NON TROVA RF
                        if (this.rblTipiVisibilita.Items.FindByValue("RF") != null)
                        {
                            this.rblTipiVisibilita.Items.FindByValue("RF").Enabled = false;
                        }
                        break;

                    case TipiVisibilitaNotaEnum.Ruolo:
                        this.rblTipiVisibilita.Items.FindByValue("Personale").Enabled = false;
                        //MODICA PER CRASH SU INPS NON TROVA RF
                        //if (this.rblTipiVisibilita.Items.FindByValue("RF") != null)
                        //{
                        //    this.rblTipiVisibilita.Items.FindByValue("RF").Enabled = false;
                        //}
                        break;

                    case TipiVisibilitaNotaEnum.RF:
                        this.rblTipiVisibilita.Items.FindByValue("Personale").Enabled = false;
                        this.rblTipiVisibilita.Items.FindByValue("Ruolo").Enabled = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Cancellazione di una nota esistente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idNota"></param>
        public void DeleteNota(string idNota)
        {
            foreach (InfoNota nota in this.Notes)
            {
                if (nota.Id.Equals(idNota))
                {
                    // Impostazione della nota come "DaRimuovere"
                    nota.DaRimuovere = true;
                    break;
                }
            }
        }

        protected void BtnNew_Click(object sender, EventArgs e)
        {
            try {
                this.SetInsertMode();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try {
                if (this.TxtNote.Text.Trim().Length == 0)
                {
                    string msg = "ErrorDocumentNoteEmpty";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');}", true);
                }
                else if (this.TxtNote.Text.Length > this.MaxLenghtNote)
                {
                    string msg = "ErrorDocumentNoteMaxLength";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');}", true);
                }
                else if (this.rblTipiVisibilita.SelectedValue == "RF" && string.IsNullOrEmpty(this.ddlNoteRF.SelectedValue))
                {
                    string msg = "ErrorDocumentNoneRF";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');}", true);
                }
                else
                {
                    this.Save();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Save dei dati
        /// </summary>
        protected virtual void Save()
        {
            if (this.SelectedItem == null)
                this.InsertItemCommand();
            else
                this.UpdateItemCommand();
        }

        protected virtual void InsertItemCommand()
        {
            InfoNota nota = new InfoNota();
            nota.Testo = this.TxtNote.Text;
            nota.TipoVisibilita = this.TipoVisibilita;

            // *****************************************************
            // Codice Tappabuchi. non funziona se l'utente ha più di un RF.
            // Se il tipo di visibilità è RF, impostiamo idRF al primo RF disponibile
            if (nota.TipoVisibilita == TipiVisibilitaNotaEnum.RF)
            {
                //Registro[] registri = UserManager.getListaRegistriWithRF(this, "1", String.Empty);
                Registro[] registri = RegistryManager.GetRFListInSession();

                if (registri != null && registri.Length > 0)
                    nota.IdRfAssociato = registri[0].systemId;
                else
                    nota.IdRfAssociato = "0";
            }

            nota = this.InsertNote(nota);

            this.Bind();

            // Selezione nuovo elemento inserito
            if (this.grdNote != null && this.grdNote.Rows!=null && this.grdNote.Rows.Count > 0)
            {
                this.grdNote.SelectedIndex = 0;
                this.SelectedItem = this.grdNote.Rows[this.grdNote.SelectedIndex];
                this.SelectItemCommand(this.SelectedItem);
                this.SetControlsEnabled();
            }
        }

        /// <summary>
        /// Creazione nuova nota a seguito di una modifica dei dati
        /// </summary>
        protected void InsertNote()
        {
            InfoNota nota = new InfoNota();

            if (this.rblTipiVisibilita.SelectedItem != null)
                nota.TipoVisibilita = (TipiVisibilitaNotaEnum)Enum.Parse(typeof(TipiVisibilitaNotaEnum), this.rblTipiVisibilita.SelectedItem.Value, true);
            else
                nota.TipoVisibilita = TipiVisibilitaNotaEnum.Tutti;

            nota.Testo = this.TxtNote.Text;

            if (nota.TipoVisibilita == TipiVisibilitaNotaEnum.RF)
            {
                if (TypeCaller.Equals("D"))
                {
                    if (this.DocumentInWorking != null && !string.IsNullOrEmpty(this.DocumentInWorking.cod_rf_prot) && !string.IsNullOrEmpty(this.DocumentInWorking.id_rf_prot))
                    {
                        nota.IdRfAssociato = this.DocumentInWorking.id_rf_prot;
                    }
                    else
                    {
                        if (Session["RFNote"] != null)
                        {
                            string[] mySplitResult = Session["RFNote"].ToString().Split('^');
                            if (mySplitResult[0] == "OK")
                                nota.IdRfAssociato = mySplitResult[1];
                        }
                    }
                }
                else
                {
                    if (Session["RFNote"] != null)
                    {
                        string[] mySplitResult = Session["RFNote"].ToString().Split('^');
                        if (mySplitResult[0] == "OK")
                            nota.IdRfAssociato = mySplitResult[1];
                    }
                }
            }

            // Se la nota contiene del testo (vengono eliminati anche i ritorni a capo ai lati della stringa)
            if (!String.IsNullOrEmpty(this.TxtNote.Text.Trim()))
                nota = this.InsertNote(nota);
        }

        /// <summary>
        /// Inserimento di una nuova nota da associare ad un documento / fascicolo
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        public InfoNota InsertNote(InfoNota nota)
        {
            nota.DaInserire = true;
            nota.Id = Guid.NewGuid().ToString();
            nota.DataCreazione = DateTime.Now;
            nota.UtenteCreatore = new InfoUtenteCreatoreNota();

            InfoUtente utente = UserManager.GetInfoUser();
            nota.UtenteCreatore.IdUtente = utente.idPeople;
            nota.UtenteCreatore.DescrizioneUtente = utente.userId;
            if (utente.delegato != null)
            {
                nota.IdPeopleDelegato = utente.delegato.idPeople;
                nota.DescrPeopleDelegato = utente.delegato.userId;
            }
            Ruolo ruolo = RoleManager.GetRoleInSession();
            nota.UtenteCreatore.IdRuolo = ruolo.idGruppo;
            nota.UtenteCreatore.DescrizioneRuolo = ruolo.descrizione;

            // Inserimento della nota nella scheda documento (come primo elemento della lista, 
            // solo se il testo della nota da inserire ed il tipo di visibilità sono differenti
            // da quelli dell'ultima nota inserita)
            if (this.TypeCaller.Equals("D"))
            {
                if (!String.IsNullOrEmpty(nota.Testo.Trim()) &&
                    (this.DocumentInWorking.noteDocumento.Length == 0 ||
                    !this.DocumentInWorking.noteDocumento[0].Testo.Trim().Equals(nota.Testo.Trim())
                    || !this.DocumentInWorking.noteDocumento[0].TipoVisibilita.Equals(nota.TipoVisibilita)))
                {
                    List<InfoNota> note = new List<InfoNota>(this.DocumentInWorking.noteDocumento);
                    note.Insert(0, nota);
                    this.DocumentInWorking.noteDocumento = note.ToArray();
                }
            }
            else
            {
                Fascicolo prj = UIManager.ProjectManager.getProjectInSession();
                if (prj == null)
                {
                    prj = new Fascicolo();
                    prj.noteFascicolo = new List<InfoNota>().ToArray();
                }
                if (!String.IsNullOrEmpty(nota.Testo.Trim()) && prj != null &&
                (prj.noteFascicolo == null || prj.noteFascicolo.Length == 0 ||
                !prj.noteFascicolo[0].Testo.Trim().Equals(nota.Testo.Trim())
                || !prj.noteFascicolo[0].TipoVisibilita.Equals(nota.TipoVisibilita)))
                {
                    List<InfoNota> note = null;
                    if (prj.noteFascicolo == null)
                    {
                        note = new List<InfoNota>();
                    }
                    else
                    {
                        note = new List<InfoNota>(prj.noteFascicolo);
                    }

                    note.Insert(0, nota);
                    prj.noteFascicolo = note.ToArray();
                    UIManager.ProjectManager.setProjectInSession(prj);
                }
            }

            return nota;
        }

        protected virtual void UpdateItemCommand()
        {
            string idNota = this.GetId(this.SelectedItem);

            DocsPaWR.InfoNota nota = this.GetNota(idNota);
            nota.TipoVisibilita = this.TipoVisibilita;
            nota.Testo = this.TxtNote.Text;
            if (this.rblTipiVisibilita.SelectedValue == "RF") nota.IdRfAssociato = this.ddlNoteRF.SelectedValue;
            nota = this.UpdateNota(nota);

            this.Bind();

            this.SelectItemCommand(this.SelectedItem);
        }

        /// <summary>
        /// Aggiornamento di una nota esistente
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        public InfoNota UpdateNota(InfoNota nota)
        {
            for (int i = 0; i < this.Notes.Length; i++)
            {
                if (this.Notes[i].Id.Equals(nota.Id))
                {
                    this.Notes[i] = nota;
                    break;
                }
            }

            return nota;
        }

        protected void SetControlsEnabled()
        {

            this.grdNote.Visible = (this.grdNote.Rows.Count > 0);

            if (!this.Enabled || this.IsNoteReadOnly || this.AbortDocument)
            {
                this.TxtNote.ReadOnly = true;
                this.rblTipiVisibilita.Enabled = false;
                this.BtnNew.Enabled = false;
                this.BtnSave.Enabled = false;
            }
            else
            {
                bool insertMode = (this.SelectedItem == null || this.grdNote.Rows.Count == 0);

                this.BtnNew.Enabled = (!insertMode);

                if (!insertMode)
                {
                    this.BtnSave.Text = this.GetLabel("NotesBtnSave");
                    string idNota = this.GetId(this.SelectedItem);
                    if (!string.IsNullOrEmpty(idNota))
                    {
                        InfoNota nota = this.GetNota(idNota);
                        if (nota != null) this.BtnSave.Enabled = this.CanUpdateNota(nota);
                    }
                }
                else
                {
                    this.TxtNote.Text = string.Empty;
                    this.BtnSave.Text = this.GetLabel("NotesBtnInsert");
                    this.BtnSave.Enabled = true;
                }

                this.rblTipiVisibilita.Enabled = this.BtnSave.Enabled;
                this.TxtNote.Enabled = this.BtnSave.Enabled;
            }

            if (this.rblTipiVisibilita.Enabled)
            {
                //Il bottone a selezione esclusiva "RF" è visibile solo se è stato definito almeno un RF
                if (ViewState["rf"] == null)
                {
                    //Ruolo ruoloUtente = RoleManager.GetRoleInSession();
                    //DocsPaWR.Registro[] registriRf = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "1", "");
                    DocsPaWR.Registro[] registriRf = RegistryManager.GetRFListInSession();
                    if (registriRf == null || (registriRf != null && registriRf.Length == 0))
                        this.rblTipiVisibilita.Items.Remove(this.rblTipiVisibilita.Items.FindByValue("RF"));
                    else
                        ViewState.Add("rf", registriRf);
                }
            }

            this.UpPnlGrid.Update();
            this.upPnlButtons.Update();
        }

        protected void ddlNoteRF_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                if ((this.ddlNoteRF.Items.Count > 1 && this.ddlNoteRF.SelectedIndex != 0) || (this.ddlNoteRF.Items.Count == 1))
                {
                    this.EnableNoteAutoComplete();
                    //this.TxtNote.Text = "";
                }
                else
                {
                    this.txtNoteAutoComplete.Text = "";
                    this.txtNoteAutoComplete.Enabled = false;
                    Session.Add("RFNote", "");
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void EnableNoteAutoComplete()
        {
            Session.Add("RFNote", "OK^" + this.ddlNoteRF.SelectedItem.Value + "^" + this.ddlNoteRF.SelectedItem.Text);
            this.txtNoteAutoComplete.Enabled = true;
            this.autoComplete1.ContextKey = ddlNoteRF.SelectedItem.Value;
            //this.autoComplete1.MinimumPrefixLength = this.AutocompleteMinimumPrefixLength;
            this.txtNoteAutoComplete.Text = "";
        }

        protected void rblTipiVisibilita_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                this.ddlNoteRF.Visible = false;
                this.txtNoteAutoComplete.Visible = false;

                ListItem item = this.rblTipiVisibilita.Items.FindByValue("RF");
                //Se è presente il bottone di selezione esclusiva "RF" si verifica quanti sono gli
                //RF associati al ruolo dell'utente
                if (item != null && rblTipiVisibilita.SelectedIndex == 2)
                {
                    //DocsPaWR.Registro[] registriRf = UserManager.getListaRegistriWithRF(RoleManager.GetRoleInSession().systemId, "1", "");
                    DocsPaWR.Registro[] registriRf = RegistryManager.GetRFListInSession();
                    //Se un ruolo appartiene a più di un RF, allora selezionando dal menù il valore RF
                    //l'utente deve selezionare su quale degli RF creare la nota
                    if (registriRf != null && registriRf.Length > 0)
                    {
                        //Se l'inserimento della nota avviene durante la protocollazione 
                        //ed è impostato nella segnatura il codice del RF, la selezione del RF dal quale
                        //prendere il codice sarà mantenuta valida anche per l'eventuale inserimento delle note
                        //in questo caso non si deve presentare la popup di selezione del RF
                        if (this.ddlNoteRF != null)
                            this.LoadNoteRF(registriRf);

                        this.txtNoteAutoComplete.Visible = false;

                        if (UserManager.IsAuthorizedFunctions("RICERCA_NOTE_ELENCO"))
                        {
                            this.txtNoteAutoComplete.Enabled = false;
                            this.txtNoteAutoComplete.Visible = true;
                            this.ddlNoteRF_SelectedIndexChanged(null, null);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadNoteRF(DocsPaWR.Registro[] listaRF)
        {
            this.txtNoteAutoComplete.Text = "";
            this.ddlNoteRF.Items.Clear();
            if (listaRF != null && listaRF.Length > 0)
            {
                this.ddlNoteRF.Visible = true;
                this.txtNoteAutoComplete.Visible = true;

                if (listaRF.Length == 1)
                {
                    ListItem item = new ListItem();
                    item.Value = listaRF[0].systemId;
                    item.Text = listaRF[0].codRegistro;
                    this.ddlNoteRF.Items.Add(item);
                    this.EnableNoteAutoComplete();
                }
                else
                {
                    ListItem itemVuoto = new ListItem();
                    itemVuoto.Value = "";
                    itemVuoto.Text = Utils.Languages.GetLabelFromCode("DocumentNoteSelectAnRF", UIManager.UserManager.GetUserLanguage());
                    this.ddlNoteRF.Items.Add(itemVuoto);
                    foreach (DocsPaWR.Registro regis in listaRF)
                    {
                        ListItem item = new ListItem();
                        item.Value = regis.systemId;
                        item.Text = regis.codRegistro;
                        this.ddlNoteRF.Items.Add(item);
                    }
                }
            }
        }

        private bool AbortDocument
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["abortDocument"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["abortDocument"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["abortDocument"] = value;
            }
        }

        private string TypeCaller
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["typeCaller"] != null)
                {
                    result = HttpContext.Current.Session["typeCaller"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["typeCaller"] = value;
            }
        }

    }
}