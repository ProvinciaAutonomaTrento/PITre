using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;


namespace NttDataWA.Popup
{
    public partial class MassiveTransmission : System.Web.UI.Page
    {

        #region Properties

        private bool IsFasc
        {
            get
            {
                return Request.QueryString["objType"].Equals("D") ? false : true;
            }
        }

        protected Dictionary<String, String> ListCheck
        {
            get
            {
                Dictionary<String, String> result = null;
                if (HttpContext.Current.Session["listCheck"] != null)
                {
                    result = HttpContext.Current.Session["listCheck"] as Dictionary<String, String>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listCheck"] = value;
            }
        }

        /// <summary>
        /// La collezione con i dettagli sulle trasmissioni da effettuare
        /// </summary>
        private MassiveOperationTrasmissionDetailsCollection TransmissionCollection
        {
            get
            {
                return HttpContext.Current.Session["TransmissionCollection"] as MassiveOperationTrasmissionDetailsCollection;
            }

            set
            {
                HttpContext.Current.Session["TransmissionCollection"] = value;
            }

        }

        /// <summary>
        /// Lista delle informazioni sui documenti da trasmettere
        /// </summary>
        private List<InfoDocumento> InfoDocumentList
        {
            get
            {
                return HttpContext.Current.Session["InfoDocumentList"] as List<InfoDocumento>;
            }
            set
            {
                HttpContext.Current.Session["InfoDocumentList"] = value;
            }
        }

        /// <summary>
        /// Lista delle informazioni sui fascicoli da trasmettere
        /// </summary>
        private List<InfoFascicolo> InfoProjectList
        {
            get
            {
                return HttpContext.Current.Session["InfoProjectList"] as List<InfoFascicolo>;
            }
            set
            {
                HttpContext.Current.Session["InfoProjectList"] = value;
            }
        }

        /// <summary>
        /// Lista dei fascicoli associati al nodo di titolario per la trasmissione extra AOO
        /// </summary>
        /// 
        private List<InfoFascicolo> InfoProjectListExtraAOO
        {
            get
            {
                return HttpContext.Current.Session["InfoProjectListExtraAOO"] as List<InfoFascicolo>;
            }
            set
            {
                HttpContext.Current.Session["InfoProjectListExtraAOO"] = value;
            }
        }

        /// <summary>
        /// Indica se il corrispondente per il quale si crea la trasmissione ha visibilità limitata sui fascicoli 
        /// </summary>
        private bool trasmRestrictFasc = false;

        ///<summary>
        ///Elenco dei corrispondenti per il quale è prevista la trasmissione dei soli fascicoli contenuti in InfoProjectListExtraAOO
        ///
        /// </summary>
        private List<string> ListaCorrProjectOnlyExtraAOO
        {
            get
            {
                return HttpContext.Current.Session["ListaCorrProjectOnlyExtraAOO"] as List<string>;
            }
            set
            {
                HttpContext.Current.Session["ListaCorrProjectOnlyExtraAOO"] = value;
            }
        }

        /// <summary>
        /// Le ragioni di trasmissione
        /// </summary>
        private RagioneTrasmissione[] TransmissionReasons
        {
            get
            {
                return HttpContext.Current.Session["TransmissionReasons"] as RagioneTrasmissione[];
            }
            set
            {
                HttpContext.Current.Session["TransmissionReasons"] = value;
            }
        }

        private bool canSearchInLists
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["canSearchInLists"] != null)
                    result = (bool)HttpContext.Current.Session["canSearchInLists"];
                return result;
            }
            set
            {
                HttpContext.Current.Session["canSearchInLists"] = value;
            }
        }

        public RubricaCallType CallType
        {
            get
            {
                return RubricaCallType.CALLTYPE_TRASM_ALL;
            }
        }

        private int AjaxAddressBookMinPrefixLenght
        {
            get
            {
                int result = 3;
                if (HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"] = value;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            // Lista della trasmissioni singole create attraverso la selezione di corrispondenti
            // dalla finestra della rubrica
            TrasmissioneSingola[] singleTransmissionArray;

            if (!IsPostBack)
            {
                this.InitializePage();
            }
            else
            {
                this.ReadRetValueFromPopup();
            }
            this.RefreshScript();
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.CloseMask(this.cbLeaseRights.Checked);
        }

        protected void BtnConfirm_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            // Prima di procedere viene verificato se almeno un utente per ogni trasmissione singola
            // è checkato. Se così non fosse, viene mostrato un messaggio di avviso all'utente e non 
            // procede con la trasmissione
            // L'oggetto con le informazioni sulle trasmissioni da inviare
            List<TrasmissioneSingola> singleTransmissionArray;

            // Booleano utilizzare per indicare che è possibile continuare
            bool canContinue = true;

            // L'eventuale messaggio da mostrare all'utente
            StringBuilder message = new StringBuilder(); ;

            // Recupero della lista di trasmissioni singole
            singleTransmissionArray = this.TransmissionCollection.GetSingleTransmissionList();

            // Se c'è un item in modifica, non si può continuare
            if (this.dgDestinatari.EditIndex != -1)
            {
                canContinue = false;
                //message = new StringBuilder("Prima di poter effettuare la trasmissione, è necessario uscire dalla modalità di modifica cliccando sul pulsante Annulla o sul pulsante Salva.");
                message = new StringBuilder(Utils.Languages.GetMessageFromCode("WarningMassiveTransmissionModify", UserManager.GetUserLanguage()));
            }

            // Se il tipo di trasmissione prevede cessione dei diritti ma la trasmissione in costruzione non 
            // contiene informazioni sul nuovo proprietario, viene segnalato un errore a meno che la lista degli
            // utenti destinatari non è costituita da un solo utente con un solo ruolo
            if (this.TransmissionCollection.GetNumberOfTransmissionsWithCessionReason() > 0 &&
                this.TransmissionCollection.DocumentLeasing == null && singleTransmissionArray[0].trasmissioneUtente.Length > 1 &&
                singleTransmissionArray[0].trasmissioneUtente[0].utente.ruoli.Length > 1)
            {
                canContinue = false;
                message = new StringBuilder(Utils.Languages.GetMessageFromCode("WarningMassiveTransmissionSelectedOwner", UserManager.GetUserLanguage()));
            }

            if (canContinue)
            {
                message = new StringBuilder(Utils.Languages.GetMessageFromCode("WarningMassiveTransmissionSelectedUser", UserManager.GetUserLanguage()));

                // Verifica delle notifiche
                foreach (TrasmissioneSingola singleTransmission in singleTransmissionArray)
                    if (singleTransmission.trasmissioneUtente.Where(
                        elem => elem.daNotificare == true).Count() == 0 &&
                        !this.TransmissionCollection.NoNotify)
                    {
                        canContinue = false;
                        message.AppendFormat("<br /> - {0}<br />", singleTransmission.corrispondenteInterno.descrizione);
                    }
            }

            // Se si può continuare, si procede, altrimenti si invita l'utente a correggere i problemi e
            // riprovare
            if (canContinue)
            {
                this.ExecuteTransmission();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCustom', 'warning', '', '" + utils.FormatJs(message.ToString()) + "');", true);
            }
        }

        protected void BtnReport_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "reallowOp", "reallowOp();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "visualReport", "parent.ajaxModalPopupMassiveReport();", true);
        }

        /// <summary>
        /// Al cambiamento della selezione dell'indice, si aggiorna la lista di trasmissioni
        /// </summary>
        protected void ddlTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            // I dettagli sul modello di trasmissione selezionato
            ModelloTrasmissione transmissionModel;

            // Riferimento alla dropdown list
            DropDownList ddlModels;

            // Inizializzazione della trasmissione da effettuare
            this.InitializeTransmission(Request.QueryString["objType"]);

            // Viene reso visibile il pulsante per l'nvio della trasmissione
            this.ResetReportAndDisplayTransmissionButton();

            // Casting del sender a DDL
            ddlModels = (DropDownList)sender;

            // Se l'item selezionato è il primo, non si procede,
            // altrimenti si prova a caricare il modello
            if (ddlModels.SelectedIndex > 0)
            {
                // Uscita dall'eventuale modalità di edit
                this.dgDestinatari.EditIndex = -1;

                // Azzeramento delle selezioni effetuate nella sezione delle trasmissioni semplici
                this.ddlReasons.SelectedIndex = 0;
                this.TxtCodeRecipientTransmission.Text = string.Empty;
                this.TxtDescriptionRecipient.Text = string.Empty;
                this.TxtCodeRecipientTransmission.Enabled = false;
                this.TxtDescriptionRecipient.Enabled = false;
                this.dtEntryDocImgAddressBookUser.Enabled = false;
                this.txtNotes.Text = String.Empty;
                this.UpPnlNote.Update();
                this.InitializeTransmission(Request.QueryString["objType"]);
                this.dgDestinatari.DataSource = null;
                this.dgDestinatari.DataBind();
                this.dgDestinatari.Visible = false;

                // La trasmissione viene creata da modello
                this.TransmissionCollection.FromModel = true;

                // Recupero dei dettagli del modello
                transmissionModel = TransmissionModelsManager.GetTemplateById(
                    UserManager.GetInfoUser().idAmministrazione,
                    ddlModels.SelectedValue);

                // Se il modello non è stato caricato con successo, viene mostrato un 
                // messaggio all'utente
                if (transmissionModel == null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorMassiveTransmissionLoadModel', 'warning', '');", true);
                }
                else
                    try
                    {
                        // Caricamento delle informazioni sul modello e creazione di una trasmissione
                        this.LoadTransmissionModelDataAndCreateTransmission(transmissionModel);
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCustom', 'warning', '', '" + utils.FormatJs(ex.Message) + "');", true);
                    }

            }

        }

        /// <summary>
        /// Funzione per la selezione / deselezione di tutti i destiatai della trasmissione
        /// </summary>
        protected void chkSelectDeselectAll_CheckedChanged(object sender, EventArgs e)
        {
            // La checkbox Seleziona / Deseleziona Tutto
            CheckBox checkBox;

            // L'identificativo univoco della trasmissione
            string transmissionId;

            // L'oggetto con le informazioni sulla trasmissione
            MassiveOperationTransmissionDetailsElement transmissionDetails;

            // Casting a checkbox del sender
            checkBox = sender as CheckBox;

            // Prelevamento dell'identificativo dell'elemento cui si riferisce la riga
            transmissionId = ((HiddenField)this.dgDestinatari.Rows[this.dgDestinatari.EditIndex].FindControl("hfId")).Value;

            // Prelevamento delle informazioni sulla trasmissione con identificativo transmissionId
            transmissionDetails = this.TransmissionCollection.GetElement(transmissionId);

            // Selezione del flag di notifica in base al valore della checkbox
            foreach (TrasmissioneUtente user in transmissionDetails.UserTransmission)
                user.daNotificare = checkBox.Checked;

            this.TransmissionCollection.SetElement(transmissionId, transmissionDetails);

            this.dgDestinatari.DataSource = this.TransmissionCollection.GetTransmissionElementArray();
            this.dgDestinatari.DataBind();
        }

        /// <summary>
        /// Al prerender viene selezionato l'item salvato
        /// </summary>
        protected void ddlType_PreRender(object sender, EventArgs e)
        {
            // L'identificativo dell'elemento con i dettagli della trasmissione
            string idTransmission;

            // L'oggetto con le informazioni sulla trasmissione da effettuare
            MassiveOperationTransmissionDetailsElement element;

            // Recupero dell'identificativo della trasmissione
            idTransmission = ((HiddenField)this.dgDestinatari.Rows[this.dgDestinatari.EditIndex].FindControl("hfId")).Value;

            // Recupero delle informazioni sulla trasmissione
            element = this.TransmissionCollection.GetElement(idTransmission);

            // Selezione del tipo di trasmissione
            ((DropDownList)sender).SelectedValue = element.Type;

            // Se la trasmissione coinvolge un solo utente, viene nascosta la drop down list
            if (element.UserTransmission.Count() == 1)
                ((DropDownList)sender).Visible = false;

        }

        /// <summary>
        /// Al prerender della lista vengono selezionati gli item per cui "da notificare" è true
        /// </summary>
        protected void cblUsers_PreRender(object sender, EventArgs e)
        {
            // La checkbox list con la lista degli utenti destinatari della trasmissione
            CheckBoxList list;

            // L'identificativo univoco della trasmissione
            string transmissionId;

            // L'oggetto con le informazioni sulla trasmissione
            MassiveOperationTransmissionDetailsElement transmissionDetails;

            // Casting del sender a check box list
            list = sender as CheckBoxList;

            // Prelevamento dell'identificativo dell'elemento cui si riferisce la riga
            transmissionId = ((HiddenField)this.dgDestinatari.Rows[this.dgDestinatari.EditIndex].FindControl("hfId")).Value;

            // Prelevamento delle informazioni sulla trasmissione con identificativo
            // transmissionId
            transmissionDetails = this.TransmissionCollection.GetElement(transmissionId);

            // Per ogni utente contenuto nella checkbox list degli utenti...
            foreach (ListItem user in list.Items)
                // ...viene prelevato l'oggetto con le informazioni sull'utente cui si
                // riferisce user e viene impostato la stato di checking dell'item stesso
                user.Selected = transmissionDetails.UserTransmission.Where(
                    u => u.utente.descrizione.Equals(user.Text)).FirstOrDefault().daNotificare;

            // Se c'è un solo utente, questo viene notificato e viene impedita la
            // modifica da parte dell'utente
            if (list.Items.Count == 1 ||
                this.TransmissionCollection.NoNotify)
            {
                list.Enabled = false;
                ((CheckBox)this.dgDestinatari.Rows[this.dgDestinatari.EditIndex].FindControl("chkSelectDeselectAll")).Visible = false;
            }
            else
                // Se i destinatari sono tutti selezionati, viene flaggato anche "Seleziona tutti"
                if (transmissionDetails.UserTransmission.Where(
                        u => u.daNotificare == true).Count() == list.Items.Count)
                    ((CheckBox)this.dgDestinatari.Rows[this.dgDestinatari.EditIndex].FindControl("chkSelectDeselectAll")).Checked = true;

            ((Literal)this.dgDestinatari.Rows[this.dgDestinatari.EditIndex].FindControl("ltlNoEditable")).Visible =
                !list.Enabled && TransmissionCollection.NoNotify;

        }

        /// <summary>
        /// Quando viene scatenato questo evento, viene individuato il tipo di operazione da compiere
        /// e viene eseguita
        /// </summary>
        protected void dgDestinatari_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                GridViewRow selectedRow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                int index = selectedRow.RowIndex;

                switch (e.CommandName)
                {
                    case "NewOwner":
                        this.ChooseNewOwner(selectedRow);
                        this.dgDestinatari.EditIndex = -1;
                        break;
                }
            }
            catch (Exception ex)
            {
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCustom', 'warning', '', '" + utils.FormatJs(ex.Message) + "');", true);
            }

        }

        /// <summary>
        /// Al cambio dell'indice di selezione della ddl della ragione di trasissione
        /// bisogna attivate la casella di testo Codice Corrispondente, la freccia,
        /// la rubrica ed il pulsante di risposta e si riempe la casella di descrizione
        /// con la descrizione della ragione
        /// </summary>
        protected void ddlReasons_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            // Conversione del sender a dropdownlist
            DropDownList ddl = sender as DropDownList;

            // Il Value della ddl splittato
            string[] splittedValues;

            // Viene reso visibile il pulsante per inviare la tramsissione
            this.ResetReportAndDisplayTransmissionButton();

            // Se l'item selezionato non è il primo si procede con l'abilitazione
            // dei campi relativi alla ricerca del corrispondente
            if (ddl.SelectedIndex != 0)
            {
                // Se la drop down list dei template ha un elemento selezionato, bisogna
                // inizializzare una nuova trasmissione
                if (this.ddlTemplates.SelectedIndex != 0)
                {
                    // Uscita dalla modalità di edit
                    this.dgDestinatari.EditIndex = -1;

                    this.ddlTemplates.SelectedIndex = 0;
                    this.upTemplates.Update();
                    this.InitializeTransmission(Request.QueryString["objType"]);
                    this.dgDestinatari.DataSource = this.TransmissionCollection.GetTransmissionElementArray();
                    this.dgDestinatari.DataBind();
                }

                this.TxtCodeRecipientTransmission.Enabled = true;
                this.TxtDescriptionRecipient.Enabled = true;
                this.dtEntryDocImgAddressBookUser.Enabled = true;

                // Splitting del Value della ddl
                splittedValues = ddl.SelectedItem.Value.Split(new string[] { "@-@" }, StringSplitOptions.None);

                // Salvataggio della ragione di trasmissione selezionata
                TrasmManager.setRagioneSel(
                    this,
                    this.TransmissionReasons.Where(tr => tr.systemId == splittedValues[0]).FirstOrDefault());

                // Gestione tasto cedi diritti
                this.SetRightLease();

                // Impostazione della descrizione della ragione
                //this.txtDescription.Text = splittedValues[1];

            }
            else
            {
                // Altrimenti si procede alla disabilitazione dei controlli
                // ed alla cancellazione del contenuto della casella di testo descrizione
                this.TxtCodeRecipientTransmission.Enabled = false;
                this.TxtDescriptionRecipient.Enabled = false;
                this.dtEntryDocImgAddressBookUser.Enabled = false;
                this.HideNewOwnerSelectionColumn();
            }

        }

        protected void dgDestinatari_OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            this.dgDestinatari.EditIndex = e.NewEditIndex;
            this.dgDestinatari.DataSource = this.TransmissionCollection.GetTransmissionElementArray();
            this.dgDestinatari.DataBind();
        }

        protected void dgDestinatari_OnRowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            this.dgDestinatari.EditIndex = -1;
            this.dgDestinatari.DataSource = this.TransmissionCollection.GetTransmissionElementArray();
            this.dgDestinatari.DataBind();
        }

        protected void dgDestinatari_OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int index = Convert.ToInt32(e.RowIndex);
            GridViewRow selectedRow = this.dgDestinatari.Rows[index];

            try
            {
                this.UpdateData(selectedRow);
                this.dgDestinatari.EditIndex = -1;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCustom', 'warning', '', '" + utils.FormatJs(ex.Message) + "');", true);
            }

            this.dgDestinatari.DataSource = this.TransmissionCollection.GetTransmissionElementArray();
            this.dgDestinatari.DataBind();
        }

        protected void dgDestinatari_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = Convert.ToInt32(e.RowIndex);
            GridViewRow selectedRow = this.dgDestinatari.Rows[index];

            this.DeleteItem(selectedRow);
            this.dgDestinatari.EditIndex = -1;

            this.dgDestinatari.DataSource = this.TransmissionCollection.GetTransmissionElementArray();
            this.dgDestinatari.DataBind();
        }

        protected void dtEntryDocImgAddressBookUser_Click(object sender, EventArgs e)
        {
            try
            {
                HttpContext.Current.Session["callType"] = this.CallType;
                HttpContext.Current.Session["AddressBook.from"] = "T_N_R_S";
                if (this.IsFasc)
                {
                    HttpContext.Current.Session["AddressBook.type"] = "F";
                }
                else
                {
                    HttpContext.Current.Session["AddressBook.type"] = "D";
                }
                HttpContext.Current.Session["FromMassive"] = true;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpPnlSender", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                // Inizializzazione della trasmissione se c'è un modello di trasmissione
                // rapida selezionato
                if (this.ddlTemplates.SelectedIndex != 0)
                {
                    this.InitializeTransmission(Request["objType"]);
                    this.ddlTemplates.SelectedIndex = -1;
                }

                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                if (atList != null && atList.Count > 0)
                {
                    foreach (NttDataWA.Popup.AddressBook.CorrespondentDetail addressBookCorrespondent in atList)
                    {
                        this.SearchCorrAndAddASetRecivier(
                            addressBookCorrespondent.CodiceRubrica.Trim(),
                            this.CallType, "S", new String[0], true, String.Empty, String.Empty, false);
                    }

                    this.dgDestinatari.DataSource = this.TransmissionCollection.GetTransmissionElementArray();
                    this.dgDestinatari.DataBind();
                }

                HttpContext.Current.Session["AddressBook.At"] = null;
                HttpContext.Current.Session["AddressBook.Cc"] = null;
                this.UpPnlRecipient.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = this.TxtCodeRecipientTransmission.Text;

                if (!string.IsNullOrEmpty(codeAddressBook))
                {
                    // this.SearchCorrespondent(codeAddressBook, caller.ID);

                    if (!this.SearchCorrAndAddASetRecivier(
                        codeAddressBook.Trim(),
                        this.CallType, "S", new String[0], true, String.Empty, String.Empty, false))
                    {
                        string msg = "ErrorTransmissionCorrespondentNotFound";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                    }

                    this.TxtCodeRecipientTransmission.Text = string.Empty;
                    this.TxtDescriptionRecipient.Text = string.Empty;


                    this.dgDestinatari.Visible = true;
                    this.dgDestinatari.DataSource = this.TransmissionCollection.GetTransmissionElementArray();
                    this.dgDestinatari.DataBind();
                }
                else
                {
                    string msg = "ErrorTransmissionCorrespondentNotFound";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region Methods

        protected void SetAjaxAddressBook()
        {
            this.RapidRecipient.MinimumPrefixLength = this.AjaxAddressBookMinPrefixLenght;

            string dataUser = RoleManager.GetRoleInSession().systemId;
            dataUser = dataUser + "-" + RegistryManager.GetRegistryInSession().systemId;
            string callType = this.CallType.ToString();
            this.RapidRecipient.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

            if (this.IsFasc)
                this.RapidRecipient.ServiceMethod = "GetListaCorrispondentiVeloce_trasmF";
        }

        protected void LoadKeys()
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]))
            {
                this.AjaxAddressBookMinPrefixLenght = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]);
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LISTE_DISTRIBUZIONE.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LISTE_DISTRIBUZIONE.ToString()] == "1")
            {
                this.canSearchInLists = true;
            }
        }

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.MassiveReport.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveReport','');", true);
            }
        }

        protected void InitializePage()
        {
            this.InitializeLanguage();
            this.SetAjaxAddressBook();
            this.InitializeList();
            this.Initialize(Request.QueryString["objType"]);

            this.BtnReport.Visible = false;
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnConfirm.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnConfirm", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnClose", language);
            this.BtnReport.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.MassiveReport.Title = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.litRapidTransmission.Text = Utils.Languages.GetLabelFromCode("MassiveTransmissionRapid", language);
            this.litSimpleTransmission.Text = Utils.Languages.GetLabelFromCode("MassiveTransmissionSimple", language);
            this.litModel.Text = Utils.Languages.GetLabelFromCode("MassiveTransmissionModel", language);
            this.litRole.Text = Utils.Languages.GetLabelFromCode("MassiveTransmissionRole", language);
            this.litUser.Text = Utils.Languages.GetLabelFromCode("MassiveTransmissionUser", language);
            this.litReason.Text = Utils.Languages.GetLabelFromCode("MassiveTransmissionReason", language);
            this.litNote.Text = Utils.Languages.GetLabelFromCode("MassiveTransmissionNote", language);
            this.TransmissionLitRecipient.Text = Utils.Languages.GetLabelFromCode("TransmissionLitRecipient", language);
            this.cbLeaseRights.Text = Utils.Languages.GetLabelFromCode("MassiveTransmissionCbLeaseRights", language);
            this.dgDestinatari.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("MassiveTransmissionGridRecipient", language);
            this.dgDestinatari.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("MassiveTransmissionGridReason", language);
            this.dgDestinatari.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("MassiveTransmissionGridType", language);
            this.dgDestinatari.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("MassiveTransmissionGridNote", language);
            this.dgDestinatari.Columns[5].HeaderText = Utils.Languages.GetLabelFromCode("MassiveTransmissionGridExpiry", language);
            this.dgDestinatari.Columns[6].HeaderText = Utils.Languages.GetLabelFromCode("MassiveTransmissionGridHidePrevious", language);
            this.dgDestinatari.Columns[7].HeaderText = Utils.Languages.GetLabelFromCode("MassiveTransmissionGridUsers", language);
            ((CommandField)this.dgDestinatari.Columns[8]).EditText = Utils.Languages.GetLabelFromCode("MassiveTransmissionGridBtnEdit", language);
            ((CommandField)this.dgDestinatari.Columns[8]).UpdateText = Utils.Languages.GetLabelFromCode("MassiveTransmissionGridBtnUpdate", language);
            ((CommandField)this.dgDestinatari.Columns[8]).CancelText = Utils.Languages.GetLabelFromCode("MassiveTransmissionGridBtnCancel", language);
            this.dgDestinatari.Columns[9].HeaderText = Utils.Languages.GetLabelFromCode("MassiveTransmissionGridBtnDelete", language);
            this.dgDestinatari.Columns[10].HeaderText = Utils.Languages.GetLabelFromCode("MassiveTransmissionGridBtnNewOwner", language);
            this.TransmitNewOwner.Title = Utils.Languages.GetLabelFromCode("TransmissionTransmitNewOwner", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("BaseMasterAddressBook", language);
            this.ddlTemplates.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("ddlTemplatesMassiveTrans", language));
            this.ddlReasons.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("ddlReasonsMassiveTrans", language));
        }

        protected string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        public void InitializeList()
        {
            Dictionary<String, MassiveOperationTarget> temp = new Dictionary<string, MassiveOperationTarget>();

            // Inizializzazione della mappa con i system id degli oggetti e lo stato
            // di checking (in fase di inizializzazione tutti gli item sono deselezionati)
            foreach (KeyValuePair<string, string> item in this.ListCheck)
                if (!temp.Keys.Contains(item.Key))
                    temp.Add(item.Key, new MassiveOperationTarget(item.Key, item.Value));

            // Salvataggio del dizionario
            MassiveOperationUtils.ItemsStatus = temp;
        }

        /// <summary>
        /// Funzione per l'inizializzazione della pagina
        /// </summary>
        /// <param name="objType">Il tipo di oggetto</param>
        private void Initialize(string objType)
        {
            // Caricamento dei modelli di trasmissione
            //this.FillTransmissionModelDDLWithUserModel(objType);
            this.FillTransmissionModelDDLWithSharedModel(objType);

            // Se objType è D vengono caricate le informazioni sui documenti,
            // altrimenti si caricano le informazioni sui fascicoli da trasmettere
            if (objType.Equals("D"))
                this.InfoDocumentList = this.LoadDocumentInformation(
                    MassiveOperationUtils.GetSelectedItems());
            else
                this.InfoProjectList = this.LoadProjectInformation(
                    MassiveOperationUtils.GetSelectedItems());
            //se abilitato il controllo della visibilità sui fascicoli da trasmettere (INPS)
            if (UserManager.isFiltroAooEnabled() && this.InfoProjectList != null && this.InfoProjectList.Count > 0)
            {
                InfoProjectListExtraAOO = new List<InfoFascicolo>();
                ListaCorrProjectOnlyExtraAOO = new List<string>();
                InfoProjectListExtraAOO.AddRange(this.InfoProjectList.GetRange(0, this.InfoProjectList.Count));
                //VERIFICO QUALI REGISTRI SI TROVANO SU UN NODO DI TITOLARIO COMUNE(EXTRA AOO)
                foreach (InfoFascicolo f in InfoProjectList)
                {
                    string idRegFasc = ProjectManager.getFascicoloById(this.Page, f.idFascicolo).idRegistroNodoTit;
                    if (!string.IsNullOrEmpty(idRegFasc))// il fascicolo non si trova sul nodo di titolario per la trasmissione di fascicoli extra AOO
                    {
                        InfoProjectListExtraAOO.Remove(f);
                    }
                }
            }

            // Creazione e inizializzazione di una nuova trasmissione
            this.InitializeTransmission(objType);

            // Caricamento delle ragioni di trasmissione, se ci sono documenti / fascicoli
            if (this.InfoDocumentList != null &&
                this.InfoDocumentList.Count > 0)
                this.FillDocumentTransmissionReasonDDL(this.TransmissionCollection);
            if (this.InfoProjectList != null &&
                this.InfoProjectList.Count > 0)
                this.FillProjectTransmissionReasonDDL(this.TransmissionCollection);

            // Compilazione della casella di testo Utente
            this.litUserInSession.Text = UserManager.GetUserInSession().descrizione;

            // Compilazione della casella di testo Ruolo
            this.litRoleInSession.Text = RoleManager.GetRoleInSession().descrizione;

            // Visualizzazione della ceckbox "Cede diritti" se l'utente è abilitato
            // Inizialmente non è flaggato e disabilitato
            this.cbLeaseRights.Visible = this.CheckIsRoleEnabledToRightLease();
            this.cbLeaseRights.Checked = false;
            this.cbLeaseRights.Enabled = false;
        }

        /// <summary>
        /// Funzione per il caricamento delle tipologie di trasmissione disponibili per i fascicoli
        /// </summary>
        /// <param name="transmission">Le informazioni sulla trasmissione da effettuare</param>
        private void FillProjectTransmissionReasonDDL(MassiveOperationTrasmissionDetailsCollection transmission)
        {
            // Le informazioni sul primo fascicolo della lista
            Fascicolo project;

            // Caricamento dei dettagli del primo fascicolo
            project = ProjectManager.getFascicolo(
                this,
                this.InfoProjectList[0].idFascicolo);

            // Impostazione del fascicolo in sessione (per la ricerca da rubrica)
            ProjectManager.setFascicoloSelezionato(project);

            // Booleano utilizzato per indicare se si può procedere con l'inserimento della
            // ragione di trasmissione nella ddl
            bool canContinue;

            // Caricamento della lista delle ragioni di trasmissione
            this.TransmissionReasons = TrasmManager.getListaRagioniFasc(
                this,
                project);

            // Se sono state trovate ragioni di trasmissioni, viene
            // popolata la drop down lista delle ragioni
            if (this.TransmissionReasons != null)
            {
                foreach (RagioneTrasmissione reason in this.TransmissionReasons)
                {
                    // Si può procedere con l'inseriemento
                    canContinue = true;

                    // Se è valorizzato tipoRisposta e la trasmissione contiene almeno
                    // una trasmissione singola la cui ragione ha descrizione RISPOSTA, 
                    // non si può procedere
                    if (!String.IsNullOrEmpty(reason.tipoRisposta) &&
                        transmission.GetTransmissionElementArray() != null &&
                        transmission.GetTransmissionElementArray().Where(e => e.SingleTrasmission.ragione.descrizione.Equals("RISPOSTA")).Count() > 0)
                        canContinue = false;

                    // Se la ragione prevede cessione diritti ma l'utente non è abilitato,
                    // non si può procedere con l'inserimento
                    if (!reason.prevedeCessione.Equals("N") &&
                        !this.CheckIsRoleEnabledToRightLease())
                        canContinue = false;

                    // Se si può procedere con l'inserimento, viene creato
                    // e aggiunto un nuovo item alla ddl
                    if (canContinue)
                        this.ddlReasons.Items.Add(
                            new ListItem(
                                reason.descrizione,
                                reason.systemId + "@-@" +
                                reason.note + "@-@" +
                                reason.tipoDestinatario.ToString().TrimStart()[0]));
                }

            }
        }

        /// <summary>
        /// Funzione per il popolamento della lista delle ragioni di trasmissione
        /// </summary>
        /// <param name="transmission">La trasmissione su cui si sta lavorando</param>
        private void FillDocumentTransmissionReasonDDL(MassiveOperationTrasmissionDetailsCollection transmission)
        {
            // La scheda documento del primo documento della lista
            SchedaDocumento documentScheda;

            // Booleano utilizzato per indicare se si può procedere con l'inserimento della
            // ragione di trasmissione nella ddl
            bool canContinue;

            // Caricamento della scheda documento del primo documento della lista
            string error = string.Empty;
            documentScheda = DocumentManager.getDocumentDetailsNoDataCheck(
                this,
                this.InfoDocumentList[0].idProfile,
                this.InfoDocumentList[0].docNumber,
                out error);

            DocumentManager.setDocumentoSelezionato(documentScheda);

            // Caricamento della lista delle ragioni di trasmissione
            this.TransmissionReasons = TrasmManager.getListaRagioni(
                this,
                documentScheda,
                false);

            // Se sono state trovate ragioni di trasmissioni, viene
            // popolata la drop down lista delle ragioni
            if (this.TransmissionReasons != null)
            {
                foreach (RagioneTrasmissione reason in this.TransmissionReasons)
                {
                    // Si può procedere con l'inseriemento
                    canContinue = true;

                    // Se è valorizzato tipoRisposta e la trasmissione contiene almeno
                    // una trasmissione singola la cui ragione ha descrizione RISPOSTA, 
                    // non si può procedere
                    if (!String.IsNullOrEmpty(reason.tipoRisposta) &&
                        transmission.GetTransmissionElementArray() != null &&
                        transmission.GetTransmissionElementArray().Where(e => e.SingleTrasmission.ragione.descrizione.Equals("RISPOSTA")).Count() > 0)
                        canContinue = false;

                    // Se la ragione prevede cessione diritti ma l'utente non è abilitato,
                    // non si può procedere con l'inserimento
                    if (!reason.prevedeCessione.Equals("N") &&
                        !this.CheckIsRoleEnabledToRightLease())
                        canContinue = false;

                    // Se si può procedere con l'inserimento, viene creato
                    // e aggiunto un nuovo item alla ddl
                    if (canContinue)
                        this.ddlReasons.Items.Add(
                            new ListItem(
                                reason.descrizione,
                                reason.systemId + "@-@" +
                                reason.note + "@-@" +
                                reason.tipoDestinatario.ToString().TrimStart()[0]));
                }

            }

        }

        /// <summary>
        /// Funzione per il caricamento delle informazioni sui fascicoli
        /// </summary>
        /// <param name="selectedItemSystemIdList">La lista dei system id degli elementi selezionati</param>
        /// <returns>La lista degli id dei fascicoli selezionati</returns>
        private List<InfoFascicolo> LoadProjectInformation(List<MassiveOperationTarget> selectedItemList)
        {
            // La lista da restituire
            List<InfoFascicolo> toReturn = new List<InfoFascicolo>();

            // Per ogni system id presente nella lista selectedItemSystemIdList
            foreach (MassiveOperationTarget temp in selectedItemList)
                toReturn.Add(ProjectManager.getInfoFascicoloDaFascicolo(
                    ProjectManager.getFascicoloById(
                        this,
                        temp.Id)));

            // Restituzione della lista di info documento
            return toReturn;

        }

        /// <summary>
        /// Funzione per il caricamento delle informazioni sui documenti
        /// </summary>
        /// <param name="selectedItemSystemIdList">La lista dei system id degli elementi selezionati</param>
        /// <returns>La lista degli id dei documenti selezionati</returns>
        private List<InfoDocumento> LoadDocumentInformation(List<MassiveOperationTarget> selectedItemSystemIdList)
        {
            // La lista da restituire
            List<InfoDocumento> toReturn = new List<InfoDocumento>();

            // Per ogni system id presente nella lista selectedItemSystemIdList
            foreach (MassiveOperationTarget temp in selectedItemSystemIdList)
                toReturn.Add(DocumentManager.GetInfoDocumento(
                    temp.Id,
                    String.Empty,
                    this));

            // Restituzione della lista di info documento
            return toReturn;

        }

        /// <summary>
        /// Funzione per il caricamento della lista di modelli di trasmissione
        /// shared
        /// </summary>
        /// <param name="objType">Il tipo di oggetto per cui effettuare la ricerca</param>
        private void FillTransmissionModelDDLWithSharedModel(string objType)
        {
            // Lista dei template di trasmissione
            ModelloTrasmissione[] transmissionModelTemplates;

            // Informazioni sull'utente
            InfoUtente userInfo;

            // Reperimento informazioni sull'utente
            userInfo = UserManager.GetInfoUser();

            transmissionModelTemplates = (ModelloTrasmissione[])TransmissionModelsManager.getModelliPerTrasmLite(
                userInfo.idAmministrazione,
                ((Ruolo)RoleManager.GetRoleInSession()).registri,
                userInfo.idPeople,
                userInfo.idCorrGlobali,
                String.Empty,
                String.Empty,
                String.Empty,
                objType.Equals("D") ? "D" : "F",
                this,
                null,
                userInfo.idGruppo,
                true,
                null).ToArray(typeof(ModelloTrasmissione));

            // Se ci sono elementi, si procede con il riempimento della
            // dll dei template
            if (transmissionModelTemplates != null)
            {
                // Se la ddl è vuota, viene aggiunto un item vuoto
                if (this.ddlTemplates.Items.Count == 0)
                    this.ddlTemplates.Items.Add(String.Empty);

                foreach (ModelloTrasmissione transmModel in transmissionModelTemplates)
                    this.ddlTemplates.Items.Add(
                        new ListItem(
                            transmModel.NOME,
                            transmModel.SYSTEM_ID.ToString()));
            }
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void CloseMask(bool withReturnValue)
        {
            HttpContext.Current.Session["fromMassiveAct"] = null;

            string retValue = withReturnValue ? "true" : "false";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('MassiveTransmission', '" + retValue + "');", true);
        }

        protected void generateReport(MassiveOperationReport report, string titolo)
        {
            this.generateReport(report, titolo, IsFasc);
        }

        public void generateReport(MassiveOperationReport report, string titolo, bool isFasc)
        {
            this.grdReport.DataSource = report.GetDataSet();
            this.grdReport.DataBind();
            this.pnlReport.Visible = true;
            this.upReport.Update();

            this.upRapidTransmission.Visible = false;
            this.upSimpleTransmission.Visible = false;
            this.upDestinatari.Visible = false;
            this.UpPnlGeneral.Update();

            string template = (isFasc) ? "../xml/massiveOp_formatPdfExport_fasc.xml" : "../xml/massiveOp_formatPdfExport.xml";
            report.GenerateDataSetForExport(Server.MapPath(template), titolo);

            this.BtnConfirm.Enabled = false;
            this.BtnReport.Visible = true;
            this.UpPnlButtons.Update();
        }

        /// <summary>
        /// Funzione per l'impostazione dello stato di abilitazione e flagging della
        /// casella cedi diritti
        /// </summary>
        private void SetRightLease()
        {
            switch (TrasmManager.getRagioneSel(this).prevedeCessione)
            {
                case "N":
                    this.cbLeaseRights.Checked = false;
                    this.cbLeaseRights.Enabled = false;
                    this.HideNewOwnerSelectionColumn();
                    break;
                case "W":
                    this.cbLeaseRights.Checked = false;
                    this.cbLeaseRights.Enabled = true;
                    this.ShowNewOwnerSelectionColumn();
                    break;
                case "R":
                    this.cbLeaseRights.Checked = true;
                    this.cbLeaseRights.Enabled = false;
                    this.ShowNewOwnerSelectionColumn();
                    break;
                default:
                    this.cbLeaseRights.Checked = false;
                    this.cbLeaseRights.Enabled = false;
                    this.HideNewOwnerSelectionColumn();
                    break;
            }
        }

        private void HideNewOwnerSelectionColumn()
        {
            foreach (DataControlField column in this.dgDestinatari.Columns)
                if (column.HeaderText.Equals("Nuovo proprietario"))
                    column.Visible = false;
        }

        private void ShowNewOwnerSelectionColumn()
        {
            foreach (DataControlField column in this.dgDestinatari.Columns)
                if (column.HeaderText.Equals("Nuovo proprietario"))
                    column.Visible = true;
        }

        /// <summary>
        /// Funzione per l'inizializzazione dellla collezione delle trasmissioni
        /// </summary>
        /// <param name="objType">Il tipo di oggetto da trasmettere</param>
        private void InitializeTransmission(string objType)
        {
            this.TransmissionCollection = new MassiveOperationTrasmissionDetailsCollection(objType);

        }

        /// <summary>
        /// Funzione per il caricamento delle informazioni sul modello di trasmissione e successiva creazione
        /// della trasmissione
        /// </summary>
        /// <param name="transmissionModel">Modello di trasmissione</param>
        private void LoadTransmissionModelDataAndCreateTransmission(ModelloTrasmissione transmissionModel)
        {
            // I dettagli della ragione di trasmissione
            RagioneTrasmissione transmissionReason;
            // Le informazioni sul corrispondente
            Corrispondente corr;
            // Lista delle strutture vuote
            List<string> emptyStructures;
            // Istanza del web service
            DocsPaWebService webService;
            // Array degli utenti da notificare
            String[] userToNotify;
            // Data di scadenza
            DateTime expirationDate = DateTime.Now;

            // Inizializzazione della lista delle strutture vuote
            emptyStructures = new List<string>();

            // Instanziazione del web service
            webService = new DocsPaWebService();

            // Impostazione del no_notify
            this.TransmissionCollection.NoNotify = transmissionModel.NO_NOTIFY == "1" ? true : false;

            // Impostazione del flag di cessione diritto a seconda delle autorizzazioni
            // dell'utente e delle impostazioni di cessione del modello
            this.cbLeaseRights.Checked = this.CheckIsRoleEnabledToRightLease() &&
                transmissionModel.CEDE_DIRITTI == "1";

            try
            {
                // Se c'è cessione dei diritti ed il modello è destinato a Documenti, viene
                // creato un oggetto CessioneDocumento
                if (transmissionModel.CEDE_DIRITTI == "1" &&
                    transmissionModel.CHA_TIPO_OGGETTO == "D")
                {
                    CessioneDocumento documentLeasing = new CessioneDocumento();

                    documentLeasing.docCeduto = true;
                    documentLeasing.idPeople = UserManager.GetInfoUser().idPeople;
                    documentLeasing.idRuolo = UserManager.GetInfoUser().idGruppo;
                    documentLeasing.userId = UserManager.GetInfoUser().userId;

                    if (!String.IsNullOrEmpty(transmissionModel.ID_PEOPLE_NEW_OWNER))
                        documentLeasing.idPeopleNewPropr = transmissionModel.ID_PEOPLE_NEW_OWNER;

                    if (!String.IsNullOrEmpty(transmissionModel.ID_GROUP_NEW_OWNER))
                        documentLeasing.idRuoloNewPropr = transmissionModel.ID_GROUP_NEW_OWNER;

                    this.TransmissionCollection.DocumentLeasing = documentLeasing;

                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCustom', 'warning', '', '" + utils.FormatJs(ex.Message) + "');", true);
            }

            // Impostazione delle note generali della trasmissione
            this.txtNotes.Text = transmissionModel.VAR_NOTE_GENERALI;

            // Impostazione delle note generali
            this.TransmissionCollection.GeneralNotes = transmissionModel.VAR_NOTE_GENERALI;

            // Impostazione del flag cede diritti
            this.TransmissionCollection.LeaseRigths = transmissionModel.CEDE_DIRITTI == "0" ? false : true;

            foreach (RagioneDest recivierReason in transmissionModel.RAGIONI_DESTINATARI)
                foreach (MittDest senderRecivier in recivierReason.DESTINATARI)
                {
                    transmissionReason = webService.getRagioneById(
                        senderRecivier.ID_RAGIONE.ToString());

                    TrasmManager.setRagioneSel(this, transmissionReason);

                    corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(
                        this,
                        senderRecivier.VAR_COD_RUBRICA,
                        DocsPaWR.AddressbookTipoUtente.INTERNO);

                    // Calcolo della data di scadenza
                    if (senderRecivier.SCADENZA > 0)
                        expirationDate.AddDays(senderRecivier.SCADENZA);
                    if (corr != null)
                        this.SearchCorrAndAddASetRecivier(
                            corr.codiceRubrica,
                            this.CallType,
                            senderRecivier.CHA_TIPO_TRASM,
                            senderRecivier.UTENTI_NOTIFICA.Where(e => e.FLAG_NOTIFICA == "1").Select(i => i.CODICE_UTENTE).ToArray<string>(),
                            false,
                            senderRecivier.VAR_NOTE_SING,
                            expirationDate.Date > DateTime.Now.Date ?
                                expirationDate.ToString() :
                                String.Empty,
                            senderRecivier.NASCONDI_VERSIONI_PRECEDENTI);
                }
        }

        /// <summary>
        /// Funzione per la ricerca di un corrispondente in rubrica e l'impostazione
        /// dei detinatari della trasmissione
        /// </summary>
        /// <param name="corrCode">Il codice del corrispondente da ricercare</param>
        /// <param name="callType">Il calltype con cui aprire la rubrica</param>
        /// <param name="transmissionType">Tipo di trasmissione Uno / Tutti</param>
        /// <param name="userToNotify">Lista degli utenti da notificare</param>
        /// <param name="simpleTransmission">True se si sta creando una trasmissione semplice, false se si sta creando una trasmissione da modello</param>
        /// <param name="singleNote">Nota per la trasmissione singola</param>
        /// <param name="expirationDate">Data si scadenza della trasmissione</param>
        private bool SearchCorrAndAddASetRecivier(string corrCode, RubricaCallType callType, String transmissionType, String[] userToNotify, bool simpleTransmission, string singleNote, string expirationDate, bool hidePreviousVersions)
        {
            // I parametri di ricerca da utilizzare per la ricerca
            // dei corrispondenti
            ParametriRicercaRubrica searchParameters;

            // I risultati della ricerca
            ElementoRubrica[] result;

            // Creazione dei parametri di ricerca
            searchParameters = new ParametriRicercaRubrica();

            // Salvataggio del chiamante
            UserManager.setQueryRubricaCaller(ref searchParameters);

            // Impostazione del filtro di ricerca
            searchParameters.codice = corrCode;
            searchParameters.tipoIE = AddressbookTipoUtente.INTERNO;
            searchParameters.doListe = this.canSearchInLists;
            searchParameters.doRuoli = true;
            searchParameters.doUtenti = true;
            searchParameters.doUo = true;

            // Se sono abilitati gli rf, viene abilitata anche la ricerca
            // all'interno degli stessi
            if (new DocsPaWebService().IsEnabledRF(String.Empty))
                searchParameters.doRF = true;

            // La ricerca deve essere esatta, non per like
            searchParameters.queryCodiceEsatta = true;

            // Selezione del calltype a seconda della ragione di trasmissione
            searchParameters.calltype = callType;

            // Impostazione del tipo di oggetto
            searchParameters.ObjectType = Request.QueryString["objType"].ToString();

            // Esecuzione della ricerca
            result = UserManager.getElementiRubrica(this, searchParameters);

            // Se sono stati trovati dei corrispondenti, viene impostato il destinatario
            if (result != null && result.Length == 1)
            {
                // Se il destinatario è inibito non si può procedre
                if (result[0].disabledTrasm)
                    return false;
                //throw new Exception(String.Format("Il Ruolo {0} risulta disabilitato alla ricezione delle trasmissioni",
                //    corrCode));
                else
                {
                    bool setReciviers = true;

                    // se è necessario la verifica di visibilità sui fasc da trasmettere
                    if (UserManager.isFiltroAooEnabled() && InfoProjectList != null && InfoProjectList.Count > 0)
                    {
                        Registro reg = null;
                        if (result[0].tipo.Equals("P"))// se il corrispondente è di tipo utente
                        {
                            Ruolo[] ruolo = UserManager.GetRuoliUtenteByIdCorr(result[0].systemId);
                            Registro[] registro = UserManager.GetRegistriByRuolo(this.Page, ruolo[0].systemId);
                            reg = registro[0];
                        }
                        if (result[0].tipo.Equals("R"))//se il corrispondente è un ruolo
                        {
                            Registro[] registro = UserManager.GetRegistriByRuolo(this.Page, result[0].systemId);
                            reg = registro[0];
                        }
                        if (result[0].tipo.Equals("U"))// se il corrispondente è una UO
                        {
                            string[] codiceUO = UserManager.GetUoInterneAoo();
                            if (codiceUO.Contains(result[0].codice))
                                reg = UserManager.getRegistroSelezionato(this.Page);
                            else
                            {
                                reg = new Registro();
                                reg.systemId = "0";
                            }
                        }
                        if (reg == null)
                            setReciviers = false;
                        
                        string idCurrentRegistry = string.Empty;
                        if (UserManager.getRegistroSelezionato(this.Page) != null)
                        {
                            idCurrentRegistry = UserManager.getRegistroSelezionato(this.Page).systemId;
                        }
                        else
                        {
                            idCurrentRegistry = UserManager.GetUserInSession().idRegistro;
                        }

                        //il corrispondente selezionato non ha visibilità sul registro corrente
                        if (reg != null && (!reg.systemId.Equals(idCurrentRegistry))) //il corrispondente non ha visibilità sul registro del mittente
                        {
                            if (this.InfoProjectListExtraAOO.Count < 1)
                            {
                                setReciviers = false;
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorMassiveTransmissionCorrVisibility', 'warning', '');", true);
                            }
                            else  // il corrispondente selezionato ha visibilità solo sui fascicoli contenuti in nodi extra Aoo(Inps)
                            {
                                this.trasmRestrictFasc = true; //trasmissione con restrizione per il corrispondente corrente
                                string msg = "Verranno trasmessi solo i fascicoli contenuti in nodi di titolario per la trasmissione di fascicoli extra AOO:<br />";
                                int count = 0;
                                foreach (InfoFascicolo fasc in InfoProjectListExtraAOO)
                                {
                                    ++count;
                                    if (count > 5)
                                    {
                                        msg += "<p style=\"color:blue;\">...</p>";
                                        break;
                                    }
                                    else
                                        msg += "<p style=\"color:blue;\">- " + fasc.descrizione + "</p>";
                                }
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCustom', 'warning', '', '" + utils.FormatJs(msg) + "');", true);
                            }
                        }
                    }
                    if (setReciviers)
                        this.SetReciviers(result, searchParameters, transmissionType, userToNotify, simpleTransmission, singleNote, expirationDate, hidePreviousVersions);

                    return true;
                }
            }
            else
                //throw new Exception(String.Format("Nessun destinatario presente con il codice inserito ({0})",
                //    corrCode));
                return false;
        }

        /// <summary>
        /// Funzione utilizzata per indicare se un ruolo è autorizzato alla cessione diritti
        /// </summary>
        /// <returns>True se l'utente è autorizzato alla cessione diritti</returns>
        private bool CheckIsRoleEnabledToRightLease()
        {
            bool isAuth = false;
            if (this.InfoDocumentList != null && this.InfoDocumentList.Count > 0)
                isAuth = UserManager.IsAuthorizedFunctions("ABILITA_CEDI_DIRITTI_DOC");

            if (this.InfoProjectList != null && this.InfoProjectList.Count > 0)
                isAuth = UserManager.IsAuthorizedFunctions("ABILITA_CEDI_DIRITTI_FASC");

            return isAuth;
        }

        /// <summary>
        /// Funzione per l'impostazione del destinario di una trasmissione
        /// </summary>
        /// <param name="searchResult">Risultati della ricerca</param>
        /// <param name="searchParameters">Parametri di ricerca</param>
        /// <param name="transmissionType">Tipo di trasmissione Uno / Tutti</param>
        /// <param name="userToNotify">Lista degli utenti da notificare</param>
        /// <param name="simpleTransmission">True se si tratta di trasmissione singola, false se si tratta di trasmissione creata da modello</param>
        /// <param name="singleNote">Nota per la trasmissione</param>
        /// <param name="expirationDate">Data di scadenza della trasmissione</param>
        private void SetReciviers(ElementoRubrica[] searchResult, ParametriRicercaRubrica searchParameters, String transmissionType, String[] userToNotify, bool simpleTransmission, string singleNote, string expirationDate, bool hidePreviousVersions)
        {
            // Eventuale avviso da mostrare all'utente
            string alert = String.Empty;
            // Lista dei corrispondenti appartenenti alla lista
            ArrayList corrList;
            // Lista in cui depositare i corrispondenti appartenti ad un lista
            List<ElementoRubrica> tempList;
            // La query per la costruzione della trasmissione singola
            AddressbookQueryCorrispondente corrQuery;
            // Lista delle struttura vuote
            List<String> empyTransmStructures = new List<string>();

            // A seconda del tipo di corrispondente, si procede in modo differente
            switch (searchResult[0].tipo)
            {
                case "L":
                case "F":
                    // Se il tipo è L viene effettuata una ricerca nelle Liste,
                    // altrimenti negli RF
                    if (searchResult[0].tipo == "L")
                        // Reperimento della lista di corrispondenti appartenenti alla lista
                        corrList = UserManager.getCorrispondentiByCodLista(
                            this,
                            searchParameters.codice,
                            UserManager.GetInfoUser().idAmministrazione);
                    else
                        // Reperimento lista corrispondenti del raggrupamento funzionale
                        corrList = UserManager.getCorrispondentiByCodRF(
                            this,
                            searchResult[0].codice);

                    // Se sono stati trovati dei corrispondenti...
                    if (corrList != null && corrList.Count > 0)
                    {
                        tempList = new List<ElementoRubrica>();

                        // Recupero dei corrispondenti della lista
                        foreach (Corrispondente corr in corrList)
                            tempList.Add(UserManager.getElementoRubrica(this, corr.codiceRubrica));

                        // Filtraggio della lista per la verifica delle autorizzazioni
                        tempList = new List<ElementoRubrica>(
                            UserManager.filtra_trasmissioniPerListe(this,
                                searchParameters,
                                tempList.ToArray()));

                        foreach (Corrispondente corr in corrList)
                            if (tempList.Where(e => e.codice == corr.codiceRubrica && !corr.disabledTrasm).Count() > 0)
                                this.AddSingleTransmission(
                                    this.TransmissionCollection,
                                    corr,
                                    empyTransmStructures,
                                    transmissionType,
                                    userToNotify,
                                    simpleTransmission,
                                    singleNote,
                                    expirationDate,
                                    hidePreviousVersions,
                                    false);
                        // Se la trasmissione non contiene corrispondenti o se ne contiene in
                        // quantità inferiore rispetto al numero di elementi della lista
                        // di corrispondenti, viene visualizzato un messaggio
                        if (this.TransmissionCollection.GetSingleTransmissionNumber() < corrList.Count)
                            alert = "<span style=\"color:Red\">AVVISO</span>: Nella lista ci sono corrispondenti ai quali non è possibile trasmettere!";

                    }

                    break;

                default:
                    // Impostazione dei parametri
                    corrQuery = new DocsPaWR.AddressbookQueryCorrispondente();
                    corrQuery.codiceRubrica = searchParameters.codice;
                    corrQuery.getChildren = false;
                    corrQuery.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                    corrQuery.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    corrQuery.fineValidita = true;

                    // Creazione di una trasmissione singola al corrispondente
                    if (UserManager.getListaCorrispondenti(this, corrQuery).Length > 0)
                    {
                        this.AddSingleTransmission(
                            this.TransmissionCollection,
                            UserManager.getListaCorrispondenti(this, corrQuery)[0],
                            empyTransmStructures,
                            transmissionType,
                            userToNotify,
                            simpleTransmission,
                            singleNote,
                            expirationDate,
                            hidePreviousVersions,
                            false);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorTransmissionUserNoRoles', 'warning', '');", true);
                    }
                    break;
            }

            // Se sono state rilevate delle strutture vuote, viene visualizzato un messaggio
            if (empyTransmStructures.Count > 0)
            {
                // Se non bisogna già visualizzare un messaggio, viene visualizzato un messaggio appropriato
                if (String.IsNullOrEmpty(alert))
                {
                    foreach (string s in empyTransmStructures)
                        alert += (" - " + s + "<br />");

                    alert = String.Format("Impossibile effettuare la trasmissione a questa struttura perchè priva di utenti o ruoli di riferimento:<br />{0}", alert);
                }
            }

            // Se è stato valorizzato il messaggio di alert, viene visualizzato
            if (!String.IsNullOrEmpty(alert))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCustom', 'warning', '', '" + utils.FormatJs(alert) + "');", true);
            }

        }

        /// <summary>
        /// Funzione per la creazione di una trasmissione singola
        /// </summary>
        /// <param name="transmission">La trasmissione in cui creare la trasmissione singola</param>
        /// <param name="corr">Il corrispondente a cui effettuare la trasmissione</param>
        /// <param name="emptyStructures">Lista delle struttura vuote</param>
        /// <param name="transmissionType">Tipo di trasmissione Uno / Tutti</param>
        /// <param name="userToNotify">Array da notificare</param>
        /// <param name="simpleTransmission">Se true imposta la notifica in base alla configurazione altrimenti notifica solo gli utenti contenuti nell'array</param>
        /// <param name="singleNote">Nota per la trasmissione</param>
        /// <param name="expirationDate">Data di scadenza della trasmissione</param>
        /// <param name="notifyAll">True se bisogna notificare a tutti</param>
        /// <returns>La trasmissione aggiornata</returns>
        private void AddSingleTransmission(
            MassiveOperationTrasmissionDetailsCollection transmission,
            Corrispondente corr,
            List<String> emptyStructures,
            String transmissionType,
            String[] userToNotify,
            bool simpleTransmission,
            string singleNote,
            string expirationDate,
            bool hidePreviousVersions,
            bool notifyAll)
        {
            // La trasmissione relativa ad un determinato corrispodente
            TrasmissioneSingola singleTransmission;
            // Booleano utilizzato per indicare la possibilità di continuare
            // con la creazione della trasmissione singola
            bool canContinue = true;
            // Lista dei destinatari della trasmissione
            Corrispondente[] corrs;
            // La trasmissione utente
            TrasmissioneUtente userTransmission;
            // Oggetto utilizzato per il reperimento dei ruoli cui un corrispondente
            // può inviare una tramissione
            AddressbookQueryCorrispondenteAutorizzato corrAuthoryzed;
            // Lista dei ruoli cui effettuare un trasmissione a UO
            Ruolo[] roles;
            // La trasmissione da aggiungere
            MassiveOperationTransmissionDetailsElement element;

            // Se ci sono trasmissioni singole...
            if (transmission.GetSingleTransmissionNumber() > 0)
            {
                // ...viene verificato se esiste già una trasmissione associata al corrispondente
                singleTransmission = transmission.GetSingleTransmissionForUser(corr.systemId);

                // Se è stata trovata una trasmissione relativa al corrispondente,
                // viene verificato se deve essere eliminata ed in tal caso ne
                // viene eliminata la cancellazione
                if (singleTransmission != null)
                {
                    // Non si può continuare
                    canContinue = false;

                    if (singleTransmission.daEliminare)
                        singleTransmission.daEliminare = false;
                }

                // Se esistono tramissioni con ragione cessione, o se la ragione attuale prevede cessione ma
                // le altre trasmissioni singole non la prevedono, non si procede
                if (this.ExistSingleTransmissionWithCessionReason(transmission))
                    canContinue = false;
                //se abilitato il controllo di visibilità per la trasmissione dei fascicoli e il corrispondente corrente ha visibilità limitata sui fascicoli
                if (!canContinue && UserManager.isFiltroAooEnabled() && trasmRestrictFasc)
                    ListaCorrProjectOnlyExtraAOO.Add(singleTransmission.corrispondenteInterno.codiceCorrispondente);
            }

            // Se la ragione di trasmissione selezionata prevede cessione dei diritti ma il corrispondente non è un ruolo, viene
            // segnalato un errore
            if (TrasmManager.getRagioneSel(this).prevedeCessione != "N" && !(corr is Ruolo))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningMassiveTransmissionTransferRightsRole', 'warning');", true);
                canContinue = false;
            }

            // Se non esistono trasmissioni relative al corrispondente,
            // o trasmissioni che prevedano cessione di diritti,
            // si crea una nuova trasmissione singola
            if (canContinue)
            {
                // Creazione e inizializzazione della trasmissione singola
                singleTransmission = new TrasmissioneSingola();
                singleTransmission.hideDocumentPreviousVersions = hidePreviousVersions;
                singleTransmission.tipoTrasm = transmissionType;
                singleTransmission.corrispondenteInterno = corr;
                singleTransmission.ragione = TrasmManager.getRagioneSel(this.Page);
                singleTransmission.noteSingole = singleNote;
                singleTransmission.dataScadenza = expirationDate;
                //se abilitato il controllo di visibilità per la trasmissione dei fascicoli e il corrispondente corrente ha visibilità limitata sui fascicoli
                if (UserManager.isFiltroAooEnabled() && trasmRestrictFasc)
                    ListaCorrProjectOnlyExtraAOO.Add(singleTransmission.corrispondenteInterno.codiceCorrispondente);
                // Se l'utente è abilitato alla cessione dei diritti, viene impostato il
                // flag in base allo stato della casella di cessione
                if (this.CheckIsRoleEnabledToRightLease())
                    singleTransmission.ragione.cessioneImpostata = this.cbLeaseRights.Checked;

                // Impostazione dei dettagli della trasmissione relativi al tipo di corrispondente
                if (corr is Ruolo)
                {
                    // Trasmissione di tipo Ruolo
                    singleTransmission.tipoDest = TrasmissioneTipoDestinatario.RUOLO;

                    // Recupero della lista di utenti
                    corrs = this.GetUsers(corr);

                    // Se non sono stati individuati elementi, viene resettata
                    // la trasmissione singola
                    if (corrs.Length == 0)
                        singleTransmission = null;

                    // Aggiunta dei corrispondenti alla lista dei destinatari della trasmissione
                    // (viene castato il Corrispondente a Utente)
                    foreach (Utente cor in corrs)
                    {
                        // Creazione di una nuova trasmissione utente
                        userTransmission = new TrasmissioneUtente();

                        // Impostazione dell'utente e del flag di notifica relativi al corrispondente cor
                        userTransmission.utente = cor;

                        if (simpleTransmission)
                            userTransmission.daNotificare = TrasmManager.getTxRuoloUtentiChecked() && !this.TransmissionCollection.NoNotify;
                        else
                            if (notifyAll)
                                userTransmission.daNotificare = transmission.NoNotify ? false : true;
                            else
                                // userTransmission.daNotificare = userToNotify.Contains(cor.codiceRubrica);
                                userTransmission.daNotificare = userToNotify.Contains(cor.codiceRubrica.ToUpper());

                        singleTransmission.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(
                            singleTransmission.trasmissioneUtente,
                            userTransmission);
                    }

                }

                if (corr is Utente)
                {
                    // Trasmissione di tipo utente
                    singleTransmission.tipoDest = TrasmissioneTipoDestinatario.UTENTE;

                    // Creazione della trasmissione utente
                    userTransmission = new TrasmissioneUtente();

                    // Impostazione dell'utente e dello stato di cessione diritti
                    userTransmission.utente = (DocsPaWR.Utente)corr;

                    if (simpleTransmission)
                        userTransmission.daNotificare = TrasmManager.getTxRuoloUtentiChecked() && !this.TransmissionCollection.NoNotify;
                    else
                        userTransmission.daNotificare = userToNotify.Contains(corr.codiceRubrica);

                    singleTransmission.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(
                        singleTransmission.trasmissioneUtente,
                        userTransmission);
                }

                if (corr is UnitaOrganizzativa)
                {

                    // Inizializzazione dell'oggetto per la ricerca dei ruoli cui 
                    // effettuare la trasmissione
                    corrAuthoryzed = new AddressbookQueryCorrispondenteAutorizzato();
                    corrAuthoryzed.ragione = singleTransmission.ragione;
                    corrAuthoryzed.ruolo = RoleManager.GetRoleInSession();
                    corrAuthoryzed.queryCorrispondente = new AddressbookQueryCorrispondente();
                    corrAuthoryzed.queryCorrispondente.fineValidita = true;

                    // Reperimento dell'array dei ruoli
                    roles = UserManager.getRuoliRiferimentoAutorizzati(
                        this,
                        corrAuthoryzed,
                        (UnitaOrganizzativa)corr);

                    foreach (Ruolo role in roles)
                        this.AddSingleTransmission(transmission, role, emptyStructures, transmissionType, userToNotify, simpleTransmission, singleNote, expirationDate, hidePreviousVersions, true);

                    canContinue = false;
                }

                // Se la trasmissione singola è stata creata con successo, viene aggiunta una nuova trasmissione singola
                // altrimenti viene aggiunto un elemento alla lista delle strutture vuote.
                if (canContinue && singleTransmission != null)
                    transmission.AddTransmissionDetails(singleTransmission);

                if (canContinue && singleTransmission == null)
                    emptyStructures.Add(String.Format("{0} ({1})", corr.descrizione, corr.codiceRubrica));

            }

        }

        /// <summary>
        /// Funzione per la restituzione degli utenti con un determinato codice
        /// </summary>
        /// <param name="corr">Il corrispondente da cui prelevare il codice per effettuare le ricerche</param>
        /// <returns>La lista dei corrispondenti con codice pari a quello di corr</returns>
        protected Corrispondente[] GetUsers(Corrispondente corr)
        {
            // L'oggetto per effettuare la query del corrispondente
            AddressbookQueryCorrispondente query;

            // Creazione ed inizializzazione della query per il recupero dei corrispondenti
            query = new AddressbookQueryCorrispondente();
            query.codiceRubrica = corr.codiceRubrica;
            query.getChildren = true;
            query.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
            query.fineValidita = true;
            query.tipoUtente = AddressbookTipoUtente.INTERNO;

            // Restituzione della lista dei corrispondenti
            return UserManager.getListaCorrispondenti(
                this,
                query);
        }

        /// <summary>
        /// Restituzione di tutti gli utenti destinatari della trasmissione
        /// </summary>
        /// <param name="element">L'elemento da cui estrarre i dati</param>
        /// <returns>Una stringa con la lista degli utente destinatari della trasmissione. Quelli che devono
        ///     essere notificati avranno un * dopo la descrizione</returns>
        protected string GetUsers(MassiveOperationTransmissionDetailsElement element)
        {
            // La stringa da restituire
            StringBuilder toReturn = new StringBuilder();

            // Per ogni utente destinatario della trasmissione...
            foreach (TrasmissioneUtente user in element.UserTransmission)
                // ...si aggiunge la descrizione dell'utente alla stringa da restituire
                toReturn.AppendFormat("{0}{1}<br />",
                    user.utente.descrizione,
                    user.daNotificare ? " *" : String.Empty);

            // Restituzione della lista degli utenti destinatari della trasmissione
            return toReturn.ToString();

        }

        /// <summary>
        /// Funzione per la verifica di esistenza di trasmissioni singole con
        /// ragione di trasmissione Cessione.
        /// </summary>
        /// <param name="transmission">La trasmissione di cui analizzare le trasmissioni singole</param>
        /// <returns>True se esiste almeno una trasmissione singola con ragione Cessione</returns>
        private bool ExistSingleTransmissionWithCessionReason(
            MassiveOperationTrasmissionDetailsCollection transmission)
        {
            // Il valore da restituire
            bool retValue = false;
            // Il numero di trasmissioni con cessione
            int transWithCession = 0;

            // Calcolo del numero di trasmissione con cessione
            transWithCession = transmission.GetNumberOfTransmissionsWithCessionReason();

            // Si può inserire solo un trasmissione con cessione, quindi se ce n'è più di una
            // viene visualizzato un messaggio per l'utente
            if (transWithCession > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningMassiveTransmissionTransferRightsOtherReasons', 'warning');", true);
                retValue = true;
            }
            else
                // Altrimenti se la ragione della trasmissione che si desidera inserire non prevede 
                if (this.CheckIsRoleEnabledToRightLease() && !TrasmManager.getRagioneSel(this).prevedeCessione.Equals("N") &&
                        this.cbLeaseRights.Checked)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningMassiveTransmissionTransferRightsOtherReasons2', 'warning');", true);
                    retValue = true;
                }

            return retValue;
        }

        /// <summary>
        /// Funzione per l'azeramento del report e la visualizzazione del pulsante di trasmissione
        /// </summary>
        private void ResetReportAndDisplayTransmissionButton()
        {
            this.txtNotes.Text = String.Empty;
            this.TxtCodeRecipientTransmission.Text = string.Empty;
            this.TxtDescriptionRecipient.Text = string.Empty;
        }

        /// <summary>
        /// Funzione per la creazione della sorgente dati da associare alla
        /// drop down list del tipo di notifica
        /// </summary>
        /// <returns>Il datasource per la drop down list dei tipi di notifica</returns>
        protected DataSet GetDataSourceForDDLType(MassiveOperationTransmissionDetailsElement element)
        {
            string language = UIManager.UserManager.GetUserLanguage();

            // Il dataset da restituire
            DataSet toReturn = new DataSet();

            // La tabella con i dati
            DataTable table = new DataTable();
            toReturn.Tables.Add(table);

            // Una riga da aggiungere alla sorgente dati
            DataRow dataRow;

            // Creazione ed inizializzazione della sorgente
            table.Columns.Add(new DataColumn("Text", typeof(string)));
            table.Columns.Add(new DataColumn("Value", typeof(string)));

            #region Uno
            // Inizializzazione di una nuova riga
            dataRow = table.NewRow();

            // Inizializzazione dei valori
            dataRow["Text"] = Utils.Languages.GetLabelFromCode("TransmissionDdlTypeSingle", language);
            dataRow["Value"] = "S";

            // Aggiunta della riga
            table.Rows.Add(dataRow);

            #endregion

            #region Tutti

            // Inizializzazione di una nuova riga
            dataRow = table.NewRow();

            // Inizializzazione dei valori
            dataRow["Text"] = Utils.Languages.GetLabelFromCode("TransmissionDdlTypeMulti", language);
            dataRow["Value"] = "T";

            // Aggiunta della riga
            table.Rows.Add(dataRow);

            #endregion

            // Restituzione sorgente dati
            return toReturn;

        }

        /// <summary>
        /// Funzione per la creazione di un lista con le descrizioni dei corrispondenti destinatari della trasmissione
        /// </summary>
        /// <param name="element">I dettagli sulla trasmissione</param>
        /// <returns>Lista degli utenti destinatari della trasmissione</returns>
        protected List<string> GetUsersForModifyMod(MassiveOperationTransmissionDetailsElement element)
        {
            // La lista da restituire
            List<String> toReturn = new List<string>();

            // Per ogni utente della lista di utenti, viene creato e aggiunto un nuovo elemento alla
            // lista da restituire
            foreach (TrasmissioneUtente user in element.UserTransmission)
                toReturn.Add(user.utente.descrizione);

            // Restituzione della lista
            return toReturn;
        }

        private void UpdateData(GridViewRow dataGridItem)
        {
            // L'identificativo univoco dell'elemento con le informazioni sull'elemento da modificare
            string detailsId;

            // L'oggetto con le informazioni da aggiornare
            MassiveOperationTransmissionDetailsElement elementToModify;

            // La lista degli utenti destinatari della trasmissione
            CheckBoxList list;

            // Valore booleano utilizzato per indicare che almeno un destinatario è notiifcato
            bool oneSelected = false;

            // Prelevamento dell'identificativo univoco dell'oggetto con i dettagli
            // della trasmissione da modificare
            detailsId = ((HiddenField)dataGridItem.FindControl("hfId")).Value;

            // Prelevamento dell'oggetto con le informazioni da modificare
            elementToModify = this.TransmissionCollection.GetElement(detailsId);

            // Prelevanmento della list box con i destinatari della trasmissione
            list = dataGridItem.FindControl("cblUsers") as CheckBoxList;

            // Se non ci sono destinatari da notificare, non si può procedere: almeno uno
            // deve essere flaggato
            foreach (ListItem item in list.Items)
                if (item.Selected)
                    oneSelected = true;

            if (!oneSelected && !this.TransmissionCollection.NoNotify)
                throw new Exception("E' necessario selezionare almeno un utente da notificare.");

            // Salvataggio del tipo di trasmissione,
            // delle note e della data di scadenza
            elementToModify.Type = ((DropDownList)dataGridItem.FindControl("ddlType")).SelectedItem.Value;
            elementToModify.SingleTrasmission.tipoTrasm = elementToModify.Type;

            elementToModify.Note = ((TextBox)dataGridItem.FindControl("txtNote")).Text;
            elementToModify.SingleTrasmission.noteSingole = elementToModify.Note;

            elementToModify.ExpirationDate = ((TextBox)dataGridItem.FindControl("txtDate")).Text;
            elementToModify.SingleTrasmission.dataScadenza = elementToModify.ExpirationDate;

            elementToModify.HidePreviousVersions = ((CheckBox)dataGridItem.FindControl("chkHideVers")).Checked;
            elementToModify.SingleTrasmission.hideDocumentPreviousVersions = elementToModify.HidePreviousVersions;
            // Salvataggio dello stato di notifica per gli utenti destinatari della trasmissione
            // se bisogna notificarne almeno uno
            if (!this.TransmissionCollection.NoNotify)
                foreach (ListItem user in list.Items)
                    elementToModify.UserTransmission.Where(
                        e => e.utente.descrizione.Equals(user.Text)).FirstOrDefault().daNotificare =
                            user.Selected;

        }

        private void DeleteItem(GridViewRow dataGridItem)
        {
            // L'id dell'elemento da eliminare
            string idTransmission;

            // Prelevamento dell'idetificativo dell'elemento da eliminare
            idTransmission = ((HiddenField)dataGridItem.FindControl("hfId")).Value;

            // Eliminazione dell'elemento
            this.TransmissionCollection.RemoveTransmissionDetails(idTransmission);

        }

        private void ChooseNewOwner(GridViewRow dataGridItem)
        {
            // Prelevamento dell'identificativo univoco dell'oggetto con i dettagli
            // della trasmissione da modificare
            String detailsId = ((HiddenField)dataGridItem.FindControl("hfId")).Value;

            // Prelevamento dell'oggetto con le informazioni sulla trasmissione
            MassiveOperationTransmissionDetailsElement elementToModify = this.TransmissionCollection.GetElement(detailsId);

            Trasmissione t = new Trasmissione();
            t.systemId = null;
            t.ruolo = RoleManager.GetRoleInSession();
            t.utente = UserManager.GetUserInSession();
            if (this.IsFasc)
                t.tipoOggetto = TrasmissioneTipoOggetto.FASCICOLO;
            else
                t.tipoOggetto = TrasmissioneTipoOggetto.DOCUMENTO;
            t.trasmissioniSingole = new TrasmissioneSingola[1];
            t.trasmissioniSingole[0] = elementToModify.SingleTrasmission;
            t.cessione = new CessioneDocumento();
            TrasmManager.setGestioneTrasmissione(this, t);

            String type = this.ddlTemplates.SelectedIndex == 0 ? "ST" : "STempl";
            Session["TransmissionTransmitNewOwner_type"] = type;
            Session["SaveButNotTransmit"] = true;
            Session["fromMassiveAct"] = true;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "TransmissionTransmitNewOwner", "ajaxModalPopupTransmitNewOwner();", true);
        }

        protected void SearchCorrespondent(string addressCode, string idControl)
        {
            RubricaCallType calltype = this.CallType;
            DocsPaWR.Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteRubrica(addressCode, calltype);

            if (corr == null)
            {
                this.TxtCodeRecipientTransmission.Text = string.Empty;
                this.TxtDescriptionRecipient.Text = string.Empty;
                this.IdRecipient.Value = string.Empty;

                string msg = "ErrorTransmissionCorrespondentNotFound";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');", true);
            }
            else
            {
                this.TxtCodeRecipientTransmission.Text = corr.codiceRubrica;
                this.TxtDescriptionRecipient.Text = corr.descrizione;
                this.IdRecipient.Value = corr.systemId;
            }

            this.UpPnlRecipient.Update();
        }

        /// <summary>
        /// Funzione per l'effettuazione della trasmissione
        /// </summary>
        /// <param name="objectType">Tipo di oggetto da trasmettere</param>
        private void ExecuteTransmission()
        {
            // Il report dell'esecuzione
            MassiveOperationReport report;
            // Il risultato dell'elaborazione per un documento
            MassiveOperationReport.MassiveOperationResultEnum result;
            // Il dettaglio dell'elaborazione per un documento
            string details;
            // Il path in cui risiede il template per il report da generare
            string templateFilePath;

            // Inizializzazione del report
            report = new MassiveOperationReport();

            // Se non ci sono trasmissioni da effettura non si può procedere
            if (!this.TransmissionCollection.HaveTransmissions())
                report.AddReportRow(
                    "N.A.",
                    MassiveOperationReport.MassiveOperationResultEnum.OK,
                    "Selezionare almeno un destinatario per la trasmissione.");
            else
            {
                // Salvataggio dell'oggetto con le informazioni sulla trasmissione
                TrasmManager.setGestioneTrasmissione(
                    this,
                    this.TransmissionCollection.CompileTransmissionObject(this.txtNotes.Text));

                // Se bisogna trasmettere documenti, si procede con la trasmissione dei documenti
                // altrimenti si procede con la trasmissione dei fascicoli
                if (!this.IsFasc)
                    this.TransmitDocuments(this.InfoDocumentList, report);
                else
                {
                    if (UserManager.isFiltroAooEnabled() && InfoProjectList != null && InfoProjectList.Count > 0)
                        this.TransmitProjectsExtraAoo(report);
                    else
                        this.TransmitProjects(this.InfoProjectList, report);
                }
                // Introduzione della riga di summary
                string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
                if (!this.IsFasc)
                    report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);
                else
                    report.AddSummaryRow("Fascicoli lavorati: {0} - Fascicoli non lavorati: {1}", pars);
            }

            this.generateReport(report, "Trasmissione massiva");
        }

        /// <summary>
        /// Funzione per la verifica della possibilità di effettuare la trasmissione
        /// </summary>
        /// <param name="docInfo">Informazioni sul documento da trasmettere</param>
        /// <param name="details">Dettaglio di un eventuale problema</param>
        /// <returns>Risultato della verifica</returns>
        private MassiveOperationReport.MassiveOperationResultEnum CanTransmitDocument(
            InfoDocumento docInfo,
            out string details)
        {
            // Risultato della verifica
            MassiveOperationReport.MassiveOperationResultEnum retValue = MassiveOperationReport.MassiveOperationResultEnum.OK;
            StringBuilder detailsBS = new StringBuilder();

            // Verifica della possibilità di effettuare una trasmissione per cessione dei diritti (se impostata)
            detailsBS.Append(this.SetRightLeaseOnTransmissionDoc(TrasmManager.getGestioneTrasmissione(this)));
            if (!String.IsNullOrEmpty(detailsBS.ToString()))
            {
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                detailsBS.Append("<br />");
            }

            // Verifica accessRight
            string accessRight = DocumentManager.getAccessRightDocBySystemID(docInfo.idProfile, UserManager.GetInfoUser());
            if (accessRight.Equals("20"))
            {
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                detailsBS.Append("Il documento è in attesa di accettazione, quindi non può essere trasmesso <br />");
            }

            // Verifica dello stato di annullamento del documento
            if (DocumentManager.IsDocAnnullatoByIdProfile(docInfo.idProfile))
            {
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                detailsBS.Append("Il documento risulta annullato, quindi non può essere trasmesso");
            }

            details = detailsBS.ToString();
            return retValue;

        }

        /// <summary>
        /// Funzione per la verifica della possibilità di effettuare la cessione dei diritti per un dato documento
        /// e per l'eventuale impostazione delle informazioni di cessione
        /// </summary>
        /// <param name="trasmissione">La trasmissione da verificare</param>
        /// <returns>Una stringa valorizzata se non è possibile effettuare la cessione dei diritti</returns>
        private String SetRightLeaseOnTransmissionDoc(Trasmissione trasmissione)
        {
            String retVal = String.Empty;

            // Se esiste una trasmissione con cessione diritti impostata, si verifica se l'utente è proprietario del documento
            // a livello utente o gruppo e, se il test è positivo, vengono impostate le informazioni sulla cessione
            if (this.TransmissionCollection.GetNumberOfTransmissionsWithCessionReason() > 0)
                if (!this.IsUserOwnerOfDocument(trasmissione.infoDocumento))
                    retVal = "Attenzione, impossibile cedere i diritti di proprietà se non si è il creatore del documento!";
                else
                {
                    trasmissione.salvataConCessione = true;
                    trasmissione.cessione = new CessioneDocumento();
                    trasmissione.cessione.docCeduto = true;
                    trasmissione.cessione.idPeople = UserManager.GetInfoUser().idPeople;
                    trasmissione.cessione.idRuolo = RoleManager.GetRoleInSession().idGruppo;
                    trasmissione.cessione.userId = UserManager.GetInfoUser().userId;
                    trasmissione.cessione.idPeopleNewPropr = String.IsNullOrEmpty(
                        this.TransmissionCollection.DocumentLeasing.idPeopleNewPropr) ? trasmissione.trasmissioniSingole[0].trasmissioneUtente[0].utente.idPeople :
                        this.TransmissionCollection.DocumentLeasing.idPeopleNewPropr;
                    trasmissione.cessione.idRuoloNewPropr = String.IsNullOrEmpty(this.TransmissionCollection.DocumentLeasing.idRuoloNewPropr) ?
                        RoleManager.getRuoloById(trasmissione.trasmissioniSingole[0].trasmissioneUtente[0].utente.ruoli[0].systemId).idGruppo :
                        this.TransmissionCollection.DocumentLeasing.idRuoloNewPropr;

                }

            return retVal;
        }

        /// <summary>
        /// Funzione per la verifica di proprietà di un documento da parte dell'utente loggato
        /// </summary>
        /// <param name="infoDocumento">Documento di cui verificare la proprietà</param>
        /// <returns>True se l'utente è proprietario</returns>
        private bool IsUserOwnerOfDocument(InfoDocumento infoDocumento)
        {
            // Valore da restituire
            bool retValue = true;
            string accessRights = string.Empty;
            string idGruppoTrasm = string.Empty;
            string tipoDiritto = string.Empty;

            // Utente proprietario a livello di persona
            bool isPersonOwner = false;
            // Utente proprietario a livello di gruppo
            bool isGroupOwner = false;

            try
            {
                // L'utente potrebbe essere proprietario a livello di utente
                TrasmManager.SelectSecurity(infoDocumento.idProfile, UserManager.GetInfoUser().idPeople, "= 0", out accessRights, out idGruppoTrasm, out tipoDiritto);
                isPersonOwner = (accessRights.Equals("0"));

                // caso di doc. personale (non ha il ruolo proprietario)
                if (!(isPersonOwner && (infoDocumento.personale != null && infoDocumento.personale.Equals("1"))))
                {
                    // verifica se è proprietario come RUOLO...
                    TrasmManager.SelectSecurity(infoDocumento.idProfile, UserManager.GetInfoUser().idGruppo, "= 255", out accessRights, out idGruppoTrasm, out tipoDiritto);
                    isGroupOwner = (accessRights.Equals("255"));
                }

                // se non è il proprietario come utente e ruolo, ritorna true e cederà i diritti acquisiti...
                //if (!isPersonOwner && !isGroupOwner)
                //    return true;

                // altrimenti verifica la proprietà solo come ruolo...

                //Modifica Iacozzilli Giordano 09/07/2012
                //Ora posso cedere i diritti sul doc anche se non ne sono il proprietario.
                //
                //OLD CODE:
                //if (!isPersonOwner && isGroupOwner)
                //    retValue = false;
                //
                //NEW CODE:
                if (!isPersonOwner && isGroupOwner)
                {
                    retValue = true;
                }
            }
            catch
            {
                retValue = false;
            }

            return retValue;

        }

        /// <summary>
        /// Funzione per l'effettiva esecuzione della trasmissione per i documenti selezionati
        /// </summary>
        /// <param name="documents">Lista dei documenti da trasmettere</param>
        /// <param name="report">Report dell'esecuzione</param>
        private void TransmitDocuments(List<InfoDocumento> documents, MassiveOperationReport report)
        {
            // Il risultato relativo ad una trasmissione
            MassiveOperationReport.MassiveOperationResultEnum result;

            // Il dettaglio relativo ad una trasmissione
            string details;

            bool isAnnullato;

            // Invio della trasmissione per ogni documento da inviare
            foreach (InfoDocumento docInfo in documents)
            {
                result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                details = String.Empty;
                TrasmManager.getGestioneTrasmissione(this).infoDocumento = docInfo;

                // Verifica della possibilità di trasmettere il documento
                result = this.CanTransmitDocument(docInfo, out details);

                try
                {
                    if (result != MassiveOperationReport.MassiveOperationResultEnum.KO)
                        TrasmManager.saveExecuteTrasmAM(this, TrasmManager.getGestioneTrasmissione(this), UserManager.GetInfoUser());
                }
                catch (Exception ex)
                {
                    result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                    details = String.Format(
                        "Si sono verificati degli errori durante la trasmissione del documento. Dettagli: {0}",
                        ex.Message);
                }

                if (String.IsNullOrEmpty(details))
                {
                    result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                    details = "Trasmissione del documento riuscita correttamente.";
                }

                // Aggiunta di una riga al report
                string codice = MassiveOperationUtils.getItem(docInfo.idProfile).Codice;
                report.AddReportRow(
                    codice,
                    result,
                    details);

            }
        }

        /// <summary>
        /// Funzione per l'effettiva trasmissione dei fascicoli selezionati.
        /// Richiamata nel caso in cui sia attivo il controllo sulla visibilità dei fascicoli da trasmettere
        /// </summary>
        /// <param name="report">Il report di esecuzione</param>
        private void TransmitProjectsExtraAoo(MassiveOperationReport report)
        {
            // Il risultato relativo ad una trasmissione
            MassiveOperationReport.MassiveOperationResultEnum result;

            // Il dettaglio relativo ad una trasmissione
            string details;

            List<TrasmissioneSingola> trasmSingola = new List<TrasmissioneSingola>();
            Trasmissione trasm = TrasmManager.getGestioneTrasmissione(this.Page);
            List<TrasmissioneSingola> ListAllTrasmSing = TransmissionCollection.GetSingleTransmissionList();

            //cerco tutte le trasmissioni singole riguardanti utenti con visibilità su tutti i fascicoli
            foreach (TrasmissioneSingola trasmSing in ListAllTrasmSing)
            {
                if (!ListaCorrProjectOnlyExtraAOO.Contains(trasmSing.corrispondenteInterno.codiceCorrispondente))
                    trasmSingola.Add(trasmSing);
            }
            //trasmetto i fascicoli agli utenti con visibilità su tutti i fascicoli
            if (trasmSingola.Count > 0)
            {
                trasm.trasmissioniSingole = new TrasmissioneSingola[trasmSingola.Count];
                trasm.trasmissioniSingole = trasmSingola.ToArray();
                foreach (InfoFascicolo prjInfo in InfoProjectList)
                {
                    result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                    details = String.Empty;
                    trasm.infoFascicolo = prjInfo;
                    result = this.CanTransmitProject(prjInfo, out details);
                    try
                    {
                        if (result != MassiveOperationReport.MassiveOperationResultEnum.KO)
                            TrasmManager.saveExecuteTrasmAM(this, trasm, UserManager.GetInfoUser());
                    }
                    catch (Exception ex)
                    {
                        result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                        details = String.Format(
                        "Si sono verificati degli errori durante la trasmissione del fascicolo. Dettagli: {0}",
                            ex.Message);
                    }
                    if (String.IsNullOrEmpty(details))
                    {
                        result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                        details = "Trasmissione del fascicolo riuscita correttamente.";
                    }

                    // Aggiunta di una riga al report
                    string codice = MassiveOperationUtils.getItem(prjInfo.idFascicolo).Codice;
                    report.AddReportRow(
                        codice,
                        result,
                        details);
                }
            }
            //clean lista trasm singole
            if (trasmSingola.Count > 0)
                trasmSingola.RemoveRange(0, trasmSingola.Count);

            //cerco tutte le trasmissioni singole riguardanti utenti con visibilità solo su fascicoli di nodo extra Aoo
            foreach (TrasmissioneSingola trasmSing in ListAllTrasmSing)
            {
                if (ListaCorrProjectOnlyExtraAOO.Contains(trasmSing.corrispondenteInterno.codiceCorrispondente))
                    trasmSingola.Add(trasmSing);
            }
            //trasmetto i fascicoli agli utenti con visibilità su fascicoli di nodo extra Aoo
            if (trasmSingola.Count > 0)
            {
                trasm.trasmissioniSingole = new TrasmissioneSingola[trasmSingola.Count];
                trasm.trasmissioniSingole = trasmSingola.ToArray();
                foreach (InfoFascicolo prjInfo in InfoProjectList)
                {
                    //trasmetto solo se il fascicolo appartiene ad un nodo extra Aoo
                    if (InfoProjectListExtraAOO.Contains(prjInfo))
                    {
                        result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                        details = String.Empty;
                        trasm.infoFascicolo = prjInfo;
                        result = this.CanTransmitProject(prjInfo, out details);
                        try
                        {
                            if (result != MassiveOperationReport.MassiveOperationResultEnum.KO)
                                TrasmManager.saveExecuteTrasmAM(this, trasm, UserManager.GetInfoUser());
                        }
                        catch (Exception ex)
                        {
                            result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                            details = String.Format("Si sono verificati degli errori durante la trasmissione del fascicolo. Dettagli: {0}", ex.Message);
                        }
                        if (String.IsNullOrEmpty(details))
                        {
                            result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                            details = "Trasmissione del fascicolo riuscita correttamente.";
                        }

                        // Aggiunta di una riga al report
                        string codice = MassiveOperationUtils.getItem(prjInfo.idFascicolo).Codice;
                        report.AddReportRow(codice, result, details);
                    }
                }
            }
        }

        /// <summary>
        /// Funzione per la verifica della possibilità di effettuare la trasmissione
        /// </summary>
        /// <param name="prjInfo">Informazioni sul fascicolo da trasmettere</param>
        /// <param name="details">Dettaglio di un eventuale problema</param>
        /// <returns>Risultato della verifica</returns>
        private MassiveOperationReport.MassiveOperationResultEnum CanTransmitProject(
            InfoFascicolo prjInfo,
            out string details)
        {
            // Risultato della verifica
            MassiveOperationReport.MassiveOperationResultEnum retValue = MassiveOperationReport.MassiveOperationResultEnum.OK;
            StringBuilder detailsBS = new StringBuilder();

            // Verifica della possibilità di effettuare una trasmissione per cessione dei diritti (se impostata)
            detailsBS.Append(this.SetRightLeaseOnTransmissionPrj(TrasmManager.getGestioneTrasmissione(this)));
            if (!String.IsNullOrEmpty(detailsBS.ToString()))
            {
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                detailsBS.Append("<br />");
            }

            // Verifica accessRight
            string accessRight = ProjectManager.getAccessRightFascBySystemID(prjInfo.idFascicolo, UserManager.GetInfoUser());
            if (accessRight.Equals("20"))
            {
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                detailsBS.Append("Il fascicolo è in attesa di accettazione, quindi non può essere trasmesso <br />");
            }

            // Verifica se il fascicolo è un generale
            try
            {
                bool isGenerale = ProjectManager.IsFascicoloGenerale(UserManager.GetInfoUser(), prjInfo.idFascicolo);

                if (isGenerale)
                {
                    retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                    detailsBS.Append("Non è possibile trasmettere un fascicolo generale");
                }

            }
            catch (Exception e)
            {
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                detailsBS.Append(e.Message);
            }


            details = detailsBS.ToString();
            return retValue;

        }

        /// <summary>
        /// Funzione per la verifica della possibilità di effettuare la cessione dei diritti per un dato fascicolo
        /// e per l'eventuale impostazione delle informazioni di cessione
        /// </summary>
        /// <param name="trasmissione">La trasmissione da verificare</param>
        /// <returns>Una stringa valorizzata se non è possibile effettuare la cessione dei diritti</returns>
        private String SetRightLeaseOnTransmissionPrj(Trasmissione trasmissione)
        {
            String retVal = String.Empty;

            // Se esiste una trasmissione con cessione diritti impostata, si verifica se l'utente è proprietario del fascicolo
            // a livello utente o gruppo e, se il test è positivo, vengono impostate le informazioni sulla cessione
            if (this.TransmissionCollection.GetNumberOfTransmissionsWithCessionReason() > 0)
                if (!this.IsUserOwnerOfPrj(trasmissione.infoFascicolo))
                    retVal = "Attenzione, impossibile cedere i diritti di proprietà se non si è il creatore del fascicolo!";
                else
                {
                    trasmissione.salvataConCessione = true;
                    trasmissione.cessione = new CessioneDocumento();
                    trasmissione.cessione.docCeduto = true;
                    trasmissione.cessione.idPeople = UserManager.GetInfoUser().idPeople;
                    trasmissione.cessione.idRuolo = RoleManager.GetRoleInSession().idGruppo;
                    trasmissione.cessione.userId = UserManager.GetInfoUser().userId;
                    trasmissione.cessione.idPeopleNewPropr = String.IsNullOrEmpty(
                        this.TransmissionCollection.DocumentLeasing.idPeopleNewPropr) ? trasmissione.trasmissioniSingole[0].trasmissioneUtente[0].utente.idPeople :
                        this.TransmissionCollection.DocumentLeasing.idPeopleNewPropr;
                    trasmissione.cessione.idRuoloNewPropr = String.IsNullOrEmpty(this.TransmissionCollection.DocumentLeasing.idRuoloNewPropr) ?
                        RoleManager.getRuoloById(trasmissione.trasmissioniSingole[0].trasmissioneUtente[0].utente.ruoli[0].systemId).idGruppo :
                        this.TransmissionCollection.DocumentLeasing.idRuoloNewPropr;

                }

            return retVal;

        }

        /// <summary>
        /// Funzione per la verifica di proprietà di un fascicolo da parte dell'utente loggato
        /// </summary>
        /// <param name="prjInfo">Fascicolo di cui verificare la proprietà</param>
        /// <returns>True se l'utente è proprietario</returns>
        private bool IsUserOwnerOfPrj(InfoFascicolo prjInfo)
        {
            // Valore da restituire
            bool retValue = true;
            string accessRights = string.Empty;
            string idGruppoTrasm = string.Empty;
            string tipoDiritto = string.Empty;

            // Utente proprietario a livello di persona
            bool isPersonOwner = false;
            // Utente proprietario a livello di gruppo
            bool isGroupOwner = false;

            try
            {
                // L'utente potrebbe essere proprietario a livello di utente
                TrasmManager.SelectSecurity(prjInfo.idFascicolo, UserManager.GetInfoUser().idPeople, "= 0", out accessRights, out idGruppoTrasm, out tipoDiritto);
                isPersonOwner = (accessRights.Equals("0"));

                // verifica se è proprietario come RUOLO...
                TrasmManager.SelectSecurity(prjInfo.idFascicolo, UserManager.GetInfoUser().idGruppo, "= 255", out accessRights, out idGruppoTrasm, out tipoDiritto);
                isGroupOwner = (accessRights.Equals("255"));

                // se non è il proprietario come utente e ruolo, ritorna true e cederà i diritti acquisiti...
                if (!isPersonOwner && !isGroupOwner)
                    return true;

                // altrimenti verifica la proprietà solo come ruolo...
                if (!isPersonOwner && isGroupOwner)
                    retValue = false;
            }
            catch
            {
                retValue = false;
            }

            return retValue;

        }

        /// <summary>
        /// Funzione per l'effettiva trasmissione dei fascicoli selezionati.
        /// </summary>
        /// <param name="projects">Lista dei fascicoli da trasmettere</param>
        /// <param name="report">Il report di esecuzione</param>
        private void TransmitProjects(List<InfoFascicolo> projects, MassiveOperationReport report)
        {
            // Il risultato relativo ad una trasmissione
            MassiveOperationReport.MassiveOperationResultEnum result;

            // Il dettaglio relativo ad una trasmissione
            string details;

            // Invio della trasmissione per ogni fascicolo da inviare
            foreach (InfoFascicolo prjInfo in projects)
            {
                result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                details = String.Empty;
                TrasmManager.getGestioneTrasmissione(this).infoFascicolo = prjInfo;

                result = this.CanTransmitProject(prjInfo, out details);

                try
                {
                    if (result != MassiveOperationReport.MassiveOperationResultEnum.KO)
                        TrasmManager.saveExecuteTrasmAM(this, TrasmManager.getGestioneTrasmissione(this), UserManager.GetInfoUser());
                }
                catch (Exception ex)
                {
                    result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                    details = String.Format(
                        "Si sono verificati degli errori durante la trasmissione del fascicolo. Dettagli: {0}",
                        ex.Message);
                }

                if (String.IsNullOrEmpty(details))
                {
                    result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                    details = "Trasmissione del fascicolo riuscita correttamente.";
                }

                // Aggiunta di una riga al report
                string codice = MassiveOperationUtils.getItem(prjInfo.idFascicolo).Codice;
                report.AddReportRow(
                    codice,
                    result,
                    details);

            }
        }

        #endregion

    }
}