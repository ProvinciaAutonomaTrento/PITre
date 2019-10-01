using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using System.Web.UI.HtmlControls;
using NttDatalLibrary;
using System.Text;
using System.Collections;
using System.Data;



namespace NttDataWA.Popup
{
    public partial class StartProcessSignature : System.Web.UI.Page
    {
        #region Properties

        private RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_PROTO_INT_DEST;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }
        }

        private List<ProcessoFirma> ListaProcessiDiFirma
        {
            get
            {
                if (HttpContext.Current.Session["ListaProcessiDiFirma"] != null)
                    return (List<ProcessoFirma>)HttpContext.Current.Session["ListaProcessiDiFirma"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ListaProcessiDiFirma"] = value;
            }
        }

        private ProcessoFirma SelectedSegnatureProcess
        {
            get
            {
                if (HttpContext.Current.Session["SelectedSegnatureProcess"] != null)
                    return (ProcessoFirma)HttpContext.Current.Session["SelectedSegnatureProcess"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["SelectedSegnatureProcess"] = value;
            }
        }

        private ProcessoFirma SelectedSegnatureProcessOriginale
        {
            get
            {
                if (HttpContext.Current.Session["SelectedSegnatureProcessOriginale"] != null)
                    return (ProcessoFirma)HttpContext.Current.Session["SelectedSegnatureProcessOriginale"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["SelectedSegnatureProcessOriginale"] = value;
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

        private SupportedFileType[] FileTypes
        {
            get
            {
                SupportedFileType[] result = null;
                if (HttpContext.Current.Session["SignatureProcessFileTypes"] != null)
                {
                    result = HttpContext.Current.Session["SignatureProcessFileTypes"] as SupportedFileType[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SignatureProcessFileTypes"] = value;
            }
        }
        
        /// <summary>
        /// Verifica se risulta abilitata la gestione dei tipi file supportati
        /// </summary>
        public bool SupportedFileTypesEnabled
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["SignatureProcessSupportedFileTypesEnabled"] != null)
                {
                    result = (bool)HttpContext.Current.Session["SignatureProcessSupportedFileTypesEnabled"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SignatureProcessSupportedFileTypesEnabled"] = value;
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

        private int MaxDimFileSign
        {
            get
            {
                if (HttpContext.Current.Session["MaxDimFileSign"] != null)
                    return (int)HttpContext.Current.Session["MaxDimFileSign"];
                else
                    return 0;
            }
            set
            {
                HttpContext.Current.Session["MaxDimFileSign"] = value;
            }
        }

        private string IdProcessoDaAvviare
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["IdProcessoDaAvviare"] != null)
                {
                    result = HttpContext.Current.Session["IdProcessoDaAvviare"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["IdProcessoDaAvviare"] = value;
            }
        }

        private bool CambiaStatoDocumento
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["CambiaStatoDocumento"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["CambiaStatoDocumento"].ToString());
                }
                return result;
            }
        }

        private string IdStatoSelezionatoDocumento
        {
            get
            {
                if (HttpContext.Current.Session["IdStatoSelezionatoDocumento"] != null)
                    return HttpContext.Current.Session["IdStatoSelezionatoDocumento"] as String;
                else
                    return null;
            }
        }

        private DiagrammaStato StateDiagram
        {
            get
            {
                DiagrammaStato result = null;
                if (HttpContext.Current.Session["stateDiagram"] != null)
                {
                    result = HttpContext.Current.Session["stateDiagram"] as DiagrammaStato;
                }
                return result;
            }
        }

        public string IdObjectRuoloTitolare
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["IdObjectRuoloTitolare"] != null)
                {
                    result = HttpContext.Current.Session["IdObjectRuoloTitolare"] as string;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["IdObjectRuoloTitolare"] = value;
            }
        }

        #endregion

        #region Constant

        private const string DIGITALE = "SIGN_D";
        private const string SIGN = "F";

        #endregion

        #region Standard method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.InitializeLanguage();
                if (!string.IsNullOrEmpty(Request.QueryString["from"]) && Request.QueryString["from"].Equals("SearchDocument"))
                    this.InitializeList();
                this.InitializePage();
            }
            else
            {
                RefreshScript();

                TreeNode node = this.TreeProcessSignature.SelectedNode;
                if (node != null)
                {
                    ProcessoFirma processo = (from p in this.ListaProcessiDiFirma
                                              where p.idProcesso.Equals(node.Value)
                                              select p).FirstOrDefault();

                    ViewDetailsSteps(processo);
                }
            }
        }

        private void LoadKeys()
        {
            this.SupportedFileTypesEnabled = FileManager.IsEnabledSupportedFileTypes();

            if(!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString())) && 
                !Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString()).Equals("0"))
                this.MaxDimFileSign = Convert.ToInt32(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString()));
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]))
            {
                this.AjaxAddressBookMinPrefixLenght = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]);
            }
        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.StartProcessSignatureAssigns.Text = Utils.Languages.GetLabelFromCode("StartProcessSignatureAssigns", language);
            this.StartProcessSignatureClose.Text = Utils.Languages.GetLabelFromCode("StartProcessSignatureClose", language);
            this.lblStartProcessSignature.Text = Utils.Languages.GetLabelFromCode("lblStartProcessSignature", language);
            this.ltlNote.Text = Utils.Languages.GetLabelFromCode("StartProcessSignatureLtlNote", language);
            this.ltrNotes.Text = Utils.Languages.GetLabelFromCode("DocumentLitObjectChAv", language);
            this.ltlNotificationOption.Text = Utils.Languages.GetLabelFromCode("StartProcessSignatureltlNotificationOption", language);
            this.lblNoVisibleSignatureProcess.Text = Utils.Languages.GetLabelFromCode("StartProcessSignatureLblNoVisibleSignatureProcess", language);
            this.BtnReport.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
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


        private void InitializePage()
        {
            ClearSession();
            this.LoadKeys();
            if (string.IsNullOrEmpty(this.IdProcessoDaAvviare))
            {
                LoadProcessesSignature();
            }
            else
            {
                LoadProcesseSignatureById();
                this.IdProcessoDaAvviare = null;
            }
            TreeviewProcesses_Bind();
            this.BuildOpzioniNotifiche(null);
            EnabledButton();
        }

        private void ClearSession()
        {
            HttpContext.Current.Session.Remove("ListaProcessiDiFirma");
            HttpContext.Current.Session.Remove("SignatureProcessFileTypes");
            HttpContext.Current.Session.Remove("SignatureProcessSupportedFileTypesEnabled");
            HttpContext.Current.Session.Remove("SelectedSegnatureProcessOriginale");
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshprojectTxtDescrizione", "charsLeft('txtNotes', '2000' , '" + this.ltrNotes.Text.Replace("'", "\'") + "');", true);
            this.txtNotes_chars.Attributes["rel"] = "txtNotes_'2000'_" + this.ltrNotes.Text;
        }
      
        /// <summary>
        /// Carico la lista dei processi di cui il ruolo ha visibilità
        /// </summary>
        private void LoadProcessesSignature()
        {
            try
            {
                this.ListaProcessiDiFirma = UIManager.SignatureProcessesManager.GetProcessesSignatureVisibleRole(true, false);
                if (ListaProcessiDiFirma == null || ListaProcessiDiFirma.Count == 0)
                {
                    this.plcNoSignatureProcess.Visible = true;
                    this.StartProcessSignatureAssigns.Visible = false;
                    this.plcSignatureProcesses.Visible = false;
                }
                else
                {
                    this.plcNoSignatureProcess.Visible = false;
                    this.StartProcessSignatureAssigns.Visible = true;
                    this.plcSignatureProcesses.Visible = true;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadProcesseSignatureById()
        {
            try
            {
                this.ListaProcessiDiFirma = new List<ProcessoFirma>() { SignatureProcessesManager.GetProcessoDiFirma(this.IdProcessoDaAvviare) };
                if (ListaProcessiDiFirma == null || ListaProcessiDiFirma.Count == 0)
                {
                    this.plcNoSignatureProcess.Visible = true;
                    this.StartProcessSignatureAssigns.Visible = false;
                    this.plcSignatureProcesses.Visible = false;
                }
                else
                {
                    this.plcNoSignatureProcess.Visible = false;
                    this.StartProcessSignatureAssigns.Visible = true;
                    this.plcSignatureProcesses.Visible = true;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void EnabledButton()
        {
            this.StartProcessSignatureAssigns.Enabled = this.TreeProcessSignature.SelectedNode != null;
            this.UpPnlButtons.Update();
        }

        /// <summary>
        /// Funzione per il caricamento delle informazioni sui documenti
        /// </summary>
        /// <param name="selectedItemSystemIdList">La lista dei system id degli elementi selezionati</param>
        /// <returns>La lista degli id dei documenti selezionati</returns>
        private List<SchedaDocumento> LoadSchedaDocumentsList(List<MassiveOperationTarget> selectedItemSystemIdList)
        {
            // La lista da restituire
            List<SchedaDocumento> toReturn = new List<SchedaDocumento>();

            if (selectedItemSystemIdList != null && selectedItemSystemIdList.Count > 0)
            {
                List<string> idDocumentList = (from temp in selectedItemSystemIdList select temp.Id).ToList<string>();
                toReturn = DocumentManager.GetSchedaDocuments(idDocumentList, this);
            }

            // Restituzione della lista di info documento
            return toReturn;

        }
        #endregion


        #region TreeView

        protected void TreeSignatureProcess_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode node = this.TreeProcessSignature.SelectedNode;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            /*
            ProcessoFirma processo = (from p in this.ListaProcessiDiFirma
                                   where p.idProcesso.Equals(node.Value)
                                   select p).FirstOrDefault();
            */
            ProcessoFirma processo = SignatureProcessesManager.GetProcessoDiFirma(node.Value);
            this.SelectedSegnatureProcessOriginale = new ProcessoFirma() { idProcesso = processo.idProcesso };
            this.SelectedSegnatureProcessOriginale.passi = new PassoFirma[processo.passi.Count()];
            for(int i=0; i<processo.passi.Count(); i++)
            {
                this.SelectedSegnatureProcessOriginale.passi[i] = new PassoFirma()
                {
                    idPasso = processo.passi[i].idPasso,
                    ruoloCoinvolto = processo.passi[i].ruoloCoinvolto,
                    IdAOO = processo.passi[i].IdAOO,
                    IdRF = processo.passi[i].IdRF,
                    IdMailRegistro = processo.passi[i].IdMailRegistro
                };
            }
            ViewDetailsSteps(processo);
            BuildOpzioniNotifiche(processo);
            EnabledButton();
        }

        private void BuildOpzioniNotifiche(ProcessoFirma processo)
        {
            string language = UserManager.GetUserLanguage();
            bool isPresentePassoSpedizione = false;
            bool isPresentePassoAutomatico = false;

            this.cbxNotificationOption.Items.Clear();

            this.cbxNotificationOption.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("StartProcessSignaturecbxNotificationOptionOptCP", language), "cbxNotificationOptionOptCP"));

            ListItem checkInterruzione = new ListItem(Utils.Languages.GetLabelFromCode("StartProcessSignaturecbxNotificationOptionOptIP", language), "cbxNotificationOptionOptIP");
            checkInterruzione.Selected = true;
            this.cbxNotificationOption.Items.Add(checkInterruzione);

            if (processo != null && processo.passi != null && processo.passi.Count() > 0)
            {
                isPresentePassoAutomatico = (from p in processo.passi where p.IsAutomatico select p).FirstOrDefault() != null;
                if (isPresentePassoAutomatico)
                {
                    this.cbxNotificationOption.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("cbxNotificationOptionOptErrorePassoAutomaticoP", language), "cbxNotificationOptionOptErrorePassoAutomaticoP"));
                    this.cbxNotificationOption.Items.FindByValue("cbxNotificationOptionOptErrorePassoAutomaticoP").Selected = true;

                    //Se la chiave di notifica obbligatoria di presenza di destinatari non interoperanti è obbligatoria, non mostro il check di scelta
                    string attiva = Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.NOTIFICA_DEST_NO_INTEROP_OBB.ToString());
                    if (string.IsNullOrEmpty(attiva) || !attiva.Equals("1"))
                    {
                        isPresentePassoSpedizione = (from p in processo.passi where p.Evento.CodiceAzione.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) select p).FirstOrDefault() != null;
                        if (isPresentePassoSpedizione)
                            this.cbxNotificationOption.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("cbxNotificationOptionOptDestNonInterop", language), "cbxNotificationOptionOptDestNonInterop"));
                    }

                    //Se è presente un passo automati la check di notifica di conclusione è attiva
                    this.cbxNotificationOption.Items.FindByValue("cbxNotificationOptionOptCP").Selected = true;
                }
            }
            this.UpPnlNotificationOption.Update();
        }

        private void ViewDetailsSteps(ProcessoFirma processo)
        {
            TreeNode node = this.TreeProcessSignature.SelectedNode;
            if(node != null)
            {
                //Per ogni passo, se di modello o di firma digitale, visualizzo il dettaglio a destra
                bool viewPanel = false;
                this.PnlDettaglioPassi.Controls.Clear();
                foreach(PassoFirma passo in processo.passi)
                {
                    if (passo.IsModello || passo.Evento.Gruppo.Equals(DIGITALE))
                    {
                        AddDetailsStep(passo);
                        if (!viewPanel)
                            viewPanel = true;
                    }
                }
                this.PnlPassi.Visible = viewPanel;
                this.UpPnlDettaglioPassi.Update();
            }
        }

        private void AddDetailsStep(PassoFirma passo)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            Panel divRow = new Panel();
            divRow.CssClass = "row";
            divRow.EnableViewState = false;
            divRow.ID = "divRow_" + passo.idPasso;

            HtmlGenericControl field = new HtmlGenericControl("fieldset");
            field.EnableViewState = false;
            field.ID = "field_" + passo.idPasso;
            HtmlGenericControl numeroPasso = new HtmlGenericControl("legend");
            numeroPasso.EnableViewState = false;
            numeroPasso.ID = "numeroPasso_" + passo.idPasso;
            numeroPasso.InnerHtml = Utils.Languages.GetLabelFromCode("SignatureProcessesLblSectionDocument", language) + passo.numeroSequenza.ToString();

            Image img = new Image();
            img.ImageUrl = LibroFirmaManager.GetIconEventType(passo);
            img.ImageAlign = ImageAlign.Middle;
            numeroPasso.Controls.Add(img);
            numeroPasso.Attributes.Add("class", "legend");

            field.Controls.Add(numeroPasso);
            field.Attributes.Add("class", "azure");

            if (passo.Evento.Gruppo.Equals(DIGITALE))
            {
                Label lblTypeSignature = new Label();
                lblTypeSignature.EnableViewState = false;
                lblTypeSignature.CssClass = "weight";
                lblTypeSignature.ID = "lblTypeSignature_" + passo.idPasso;
                lblTypeSignature.Text = Utils.Languages.GetLabelFromCode("ltlSignatureProcessesTypeSignature", language);

                RadioButtonList rblTypeSignatureD = new RadioButtonList();
                rblTypeSignatureD.EnableViewState = false;
                rblTypeSignatureD.RepeatDirection = RepeatDirection.Horizontal;
                rblTypeSignatureD.ID = "rbl_" + passo.idPasso.ToString();
                rblTypeSignatureD.AutoPostBack = true;
                rblTypeSignatureD.Attributes.Add("onchange", "disallowOp('Content2');");
                rblTypeSignatureD.SelectedIndexChanged += rblTypeSignatureD_SelectedIndexChanged;


                List<AnagraficaEventi> listSignatureEvent = SignatureProcessesManager.GetEventTypes(SIGN);
                #region TIPO FIRMA DIGITALE
                List<AnagraficaEventi> typeDigitalSignature = listSignatureEvent.Where(e => e.gruppo.Equals(DIGITALE)).OrderBy(g => g.idEvento).ToList();
                if (typeDigitalSignature != null && typeDigitalSignature.Count > 0)
                {
                    List<ListItem> listItem = new List<ListItem>();
                    foreach (AnagraficaEventi evento in typeDigitalSignature)
                    {
                        listItem.Add(new ListItem()
                        {
                            Text = evento.descrizione,
                            Value = evento.codiceAzione
                        });
                    }
                    rblTypeSignatureD.Items.AddRange(listItem.ToArray());
                    rblTypeSignatureD.SelectedValue = passo.Evento.CodiceAzione;
                }
                field.Controls.Add(lblTypeSignature);
                field.Controls.Add(rblTypeSignatureD);
                #endregion

            }
            if (passo.IsModello)
            {
                field.Controls.Add(AddRuoloTitolare(passo));
                if (!passo.IsAutomatico)
                    field.Controls.Add(AddUtenteTitolare(passo));
                else
                    field.Controls.Add(AddCampiPassoAutomatico(passo));
            }
            Panel divRow4 = new Panel();
            divRow4.CssClass = "row";
            divRow4.EnableViewState = false;

            divRow.Controls.Add(field);
            divRow.Controls.Add(divRow4);
            this.PnlDettaglioPassi.Controls.Add(divRow);

        }

        private Panel AddRuoloTitolare(PassoFirma passo)
        {
            //Se il passo aveva già un RF non consento di modificarlo all'avvio
            PassoFirma passoOriginale = new PassoFirma();
            if (SelectedSegnatureProcessOriginale != null)
                passoOriginale = (from p in this.SelectedSegnatureProcessOriginale.passi where p.idPasso.Equals(passo.idPasso) select p).FirstOrDefault();
            string language = UIManager.UserManager.GetUserLanguage();
            Panel content = new Panel();
            content.EnableViewState = false;

            ////Label Ruolo coinvolto
            Panel divContentLbl = new Panel();
            divContentLbl.CssClass = "col";
            divContentLbl.EnableViewState = false;
            Label lblRuoloCoinvolto = new Label();
            lblRuoloCoinvolto.ID = "lblRuoloCoinvolto_" + passo.idPasso;
            lblRuoloCoinvolto.EnableViewState = false;
            lblRuoloCoinvolto.CssClass = "weight";
            lblRuoloCoinvolto.Text = Utils.Languages.GetLabelFromCode("AddFilterSignatureProcessRuoloTitolare", language) + " *";
            divContentLbl.Controls.Add(lblRuoloCoinvolto);

            ////Bottone rubrica
            Panel divContentBtn = new Panel();
            divContentBtn.CssClass = "col-right-no-margin";
            divContentBtn.EnableViewState = false;
            CustomImageButton btnRubrica = new CustomImageButton();
            btnRubrica.ID = "btnRubrica_" + passo.idPasso;
            btnRubrica.ImageUrl = "../Images/Icons/address_book.png";
            btnRubrica.OnMouseOutImage = "../Images/Icons/address_book.png";
            btnRubrica.OnMouseOverImage = "../Images/Icons/address_book_hover.png";
            btnRubrica.ImageUrlDisabled = "../Images/Icons/address_book_disabled.png";
            btnRubrica.CssClass = "clickable";
            btnRubrica.Click += BtnAddressBook_Click;
            divContentBtn.Controls.Add(btnRubrica);

            content.Controls.Add(divContentLbl);
            content.Controls.Add(divContentBtn);


            Panel divContentTxt = new Panel();
            divContentTxt.CssClass = "row";
            divContentTxt.EnableViewState = false;

            //Codice rubrica
            Panel divContentCod = new Panel();
            divContentCod.CssClass = "colHalf";
            divContentCod.EnableViewState = false;
            CustomTextArea ctxCodRuolo = new CustomTextArea();
            ctxCodRuolo.CssClass = "txt_addressBookLeft";
            ctxCodRuolo.ID = "ctxCodRuolo_" + passo.idPasso;
            ctxCodRuolo.ClientIDMode = ClientIDMode.Static;
            ctxCodRuolo.EnableViewState = false;
            ctxCodRuolo.AutoPostBack = true;
            ctxCodRuolo.Attributes.Add("onchange", "disallowOp('Content2');");
            ctxCodRuolo.TextChanged += TxtCode_OnTextChanged;
            ctxCodRuolo.CssClassReadOnly = "txt_addressBookLeft_disabled";
            if (passoOriginale != null && passoOriginale.ruoloCoinvolto != null && !string.IsNullOrEmpty(passoOriginale.ruoloCoinvolto.idGruppo))
                ctxCodRuolo.ReadOnly = true;
            divContentCod.Controls.Add(ctxCodRuolo);

                //Descrizione 
            Panel divContentDesc = new Panel();
            divContentDesc.CssClass = "colHalf2";
            divContentDesc.EnableViewState = false;
            Panel divContentDesc1 = new Panel();
            divContentDesc1.CssClass = "colHalf3";
            divContentDesc1.EnableViewState = false;
            CustomTextArea ctxDescRuolo = new CustomTextArea();
            ctxDescRuolo.CssClass = "txt_addressBookRight";
            ctxDescRuolo.ID = "ctxDescRuolo_" + passo.idPasso;
            ctxDescRuolo.ClientIDMode = ClientIDMode.Static;
            ctxDescRuolo.EnableViewState = false;
            ctxDescRuolo.CssClassReadOnly = "txt_addressBookLeft_disabled";
            if (passoOriginale != null && passoOriginale.ruoloCoinvolto != null && !string.IsNullOrEmpty(passoOriginale.ruoloCoinvolto.idGruppo))
                ctxDescRuolo.ReadOnly = true;
            divContentDesc1.Controls.Add(ctxDescRuolo);
            divContentDesc.Controls.Add(divContentDesc1);

            Button btnRecipient = new Button();
            btnRecipient.ID = "btnRecipient_" + passo.idPasso;
            btnRecipient.ClientIDMode = ClientIDMode.Static;
            btnRecipient.Attributes.Add("Style", "display: none;");
            btnRecipient.Attributes.Add("onclick", "disallowOp('Content2');");
            divContentDesc.Controls.Add(btnRecipient);
            divContentDesc.Controls.Add(SetAjaxAddressBook(ctxDescRuolo, ctxCodRuolo, btnRecipient));

            divContentTxt.Controls.Add(divContentCod);
            divContentTxt.Controls.Add(divContentDesc);

            content.Controls.Add(divContentTxt);


            if (passo.ruoloCoinvolto != null && !string.IsNullOrEmpty(passo.ruoloCoinvolto.idGruppo))
            {
                ctxCodRuolo.Text = passo.ruoloCoinvolto.codiceRubrica;
                ctxDescRuolo.Text = passo.ruoloCoinvolto.descrizione;
            }
            else
            {
                ctxCodRuolo.Text = string.Empty;
                ctxDescRuolo.Text = string.Empty;
            }

            return content;
        }

        private Panel AddUtenteTitolare(PassoFirma passo)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            Panel content = new Panel();
            content.CssClass = "row";
            content.EnableViewState = false;

            Label lblUtenteCoinvolto1 = new Label();
            lblUtenteCoinvolto1.ID = "lblUtenteCoinvolto1_" + passo.idPasso;
            lblUtenteCoinvolto1.EnableViewState = false;
            lblUtenteCoinvolto1.CssClass = "weight";
            lblUtenteCoinvolto1.Text = Utils.Languages.GetLabelFromCode("AddFilterSignatureProcessUtenteTitolare", language);
            content.Controls.Add(lblUtenteCoinvolto1);

            DropDownList ddlUtenteTitolare = new DropDownList();
            ddlUtenteTitolare.EnableViewState = false;
            ddlUtenteTitolare.ID = "ddlUtente_" + passo.idPasso.ToString();
            ddlUtenteTitolare.CssClass = "chzn-select-deselect";
            ddlUtenteTitolare.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));
            ddlUtenteTitolare.SelectedIndexChanged += new EventHandler(ddlUtenteTitolare_SelectedIndexChanged);
            ddlUtenteTitolare.AutoPostBack = true;
            ddlUtenteTitolare.Attributes.Add("onchange", "disallowOp('Content2');");
            int menuWidth = 300;
            ddlUtenteTitolare.Width = menuWidth;
            ddlUtenteTitolare.Items.Clear();
            ddlUtenteTitolare.Enabled = true;
            if (passo.ruoloCoinvolto != null && !string.IsNullOrEmpty(passo.ruoloCoinvolto.idGruppo))
            {
                List<Utente> listUserInRole = UIManager.UserManager.getUserInRoleByIdGruppo(passo.ruoloCoinvolto.idGruppo);
                if (listUserInRole != null && listUserInRole.Count > 0)
                {
                    ListItem empty = new ListItem("", "");
                    ddlUtenteTitolare.Items.Add(empty);
                    ddlUtenteTitolare.SelectedIndex = -1;

                    for (int i = 0; i < listUserInRole.Count; i++)
                    {
                        ListItem item = new ListItem(listUserInRole[i].descrizione, listUserInRole[i].systemId);
                        ddlUtenteTitolare.Items.Add(item);
                    }

                    ddlUtenteTitolare.Enabled = true;
                }
                if (passo.utenteCoinvolto != null && !string.IsNullOrEmpty(passo.utenteCoinvolto.idPeople))
                    ddlUtenteTitolare.SelectedValue = passo.utenteCoinvolto.idPeople;
            }
            content.Controls.Add(ddlUtenteTitolare);

            return content;
        }

        private Panel AddCampiPassoAutomatico(PassoFirma passo)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            Panel content = new Panel();
            content.EnableViewState = false;

            Panel registroAOO = AddRegistroAOO(passo);
            registroAOO.EnableViewState = false;
            registroAOO.CssClass = "colHalf15";
            content.Controls.Add(registroAOO);

            Panel registroRF= AddRegistroRF(passo, registroAOO);
            registroRF.EnableViewState = false;
            registroRF.CssClass = "colHalf16";
            content.Controls.Add(registroRF);

            if (passo.Evento.CodiceAzione.Equals(Azione.DOCUMENTOSPEDISCI.ToString()))
            {
                Panel caselle = AddCaselleRegistro(passo);
                caselle.EnableViewState = false;
                caselle.CssClass = "colHalf16";
                content.Controls.Add(caselle);
            }

            return content;
        }

        private Panel AddRegistroAOO(PassoFirma passo)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            Panel content = new Panel();
            content.CssClass = "row";
            content.EnableViewState = false;

            Label lblRegistroAOO = new Label();
            lblRegistroAOO.ID = "lblRegistroAOO_" + passo.idPasso;
            lblRegistroAOO.EnableViewState = false;
            lblRegistroAOO.CssClass = "weight";
            lblRegistroAOO.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLtlRegistroAOO", language) + " *";
            content.Controls.Add(lblRegistroAOO);

            Panel ddl = new Panel();
            ddl.CssClass = "row";
            ddl.EnableViewState = false;
            DropDownList ddlRegistroAOO = new DropDownList();
            ddlRegistroAOO.EnableViewState = false;
            ddlRegistroAOO.ID = "ddlRegistroAOO_" + passo.idPasso.ToString();
            ddlRegistroAOO.CssClass = "chzn-select-deselect";
            ddlRegistroAOO.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SignatureProcessesDdlRegistroAOO", language));
            ddlRegistroAOO.Attributes.Add("onchange", "disallowOp('Content2');");
            ddlRegistroAOO.AutoPostBack = true;
            ddlRegistroAOO.Width = 110;
            ddlRegistroAOO.SelectedIndexChanged += ddlRegistroAOO_OnTextChanged;
            ddlRegistroAOO.Enabled = false;

            if (passo.ruoloCoinvolto != null && !string.IsNullOrEmpty(passo.ruoloCoinvolto.idGruppo))
            {
                ddlRegistroAOO.Enabled = true;
                ddlRegistroAOO.Items.Add(new ListItem("", ""));
                Registro[] registriAOO = UIManager.RegistryManager.GetRegistriesByRole(passo.ruoloCoinvolto.systemId);
                foreach (DocsPaWR.Registro reg in registriAOO)
                {
                    if (!reg.flag_pregresso)
                    {
                        ddlRegistroAOO.Items.Add(new ListItem(reg.codRegistro, reg.systemId));
                    }
                }
                if (!string.IsNullOrEmpty(passo.IdAOO))
                {
                    ddlRegistroAOO.SelectedValue = passo.IdAOO;
                    //Se il passo aveva già un AOO non consento di modificarlo all'avvio
                    if (SelectedSegnatureProcessOriginale != null)
                    {
                        PassoFirma passoOriginale = (from p in this.SelectedSegnatureProcessOriginale.passi where p.idPasso.Equals(passo.idPasso) select p).FirstOrDefault();
                        if (passoOriginale != null && !string.IsNullOrEmpty(passoOriginale.IdAOO))
                            ddlRegistroAOO.Enabled = false;
                    }
                }
                else
                {
                    ddlRegistroAOO.SelectedIndex = 0;
                }
            }
            ddl.Controls.Add(ddlRegistroAOO);
            content.Controls.Add(ddl);
            return content;
        }

        private Panel AddRegistroRF(PassoFirma passo, Panel registroAOO)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            Panel content = new Panel();
            content.CssClass = "row";
            content.EnableViewState = false;

            Label lblRegistroRF = new Label();
            lblRegistroRF.ID = "lblRegistroRF_" + passo.idPasso;
            lblRegistroRF.EnableViewState = false;
            lblRegistroRF.CssClass = "weight";
            lblRegistroRF.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLtlRegistroRFSegnatura", language);
            if (passo.Evento.CodiceAzione.Equals(Azione.DOCUMENTOSPEDISCI.ToString()))
            {
                lblRegistroRF.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLtlRegistroRFSpedizione", language);
                lblRegistroRF.Text += " *";
            }
            content.Controls.Add(lblRegistroRF);

            Panel ddl = new Panel();
            ddl.CssClass = "row";
            ddl.EnableViewState = false;
            DropDownList ddlRegistroRF;
            if (passo.Evento.CodiceAzione.Equals(Azione.DOCUMENTOSPEDISCI.ToString()))
                ddlRegistroRF = LoadRegistriRFSpedizione(passo);
            else
                ddlRegistroRF = LoadRegistriRF(passo);
            ddl.Controls.Add(ddlRegistroRF);
            content.Controls.Add(ddl);

            return content;
        }

        private DropDownList LoadRegistriRF(PassoFirma passo)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            DropDownList ddlRegistroRF = new DropDownList();
            ddlRegistroRF.EnableViewState = false;
            ddlRegistroRF.ID = "ddlRegistroRF_" + passo.idPasso.ToString();
            ddlRegistroRF.CssClass = "chzn-select-deselect";
            ddlRegistroRF.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SignatureProcessesDdlRegistroRF", language));
            ddlRegistroRF.Width = 200;
            ddlRegistroRF.Attributes.Add("onchange", "disallowOp('Content2');");
            ddlRegistroRF.AutoPostBack = true;
            ddlRegistroRF.SelectedIndexChanged += ddlRegistroRF_OnTextChanged;
            ddlRegistroRF.Enabled = false;
            if (!string.IsNullOrEmpty(passo.IdAOO))
            {
                ddlRegistroRF.Enabled = true;
                ddlRegistroRF.Items.Add(new ListItem("", ""));
                Registro[] registriRfVisibili = UIManager.RegistryManager.GetListRegistriesAndRF(passo.ruoloCoinvolto.systemId, "1", passo.IdAOO);
                foreach (NttDataWA.DocsPaWR.Registro registro in registriRfVisibili)
                {
                    ddlRegistroRF.Items.Add(new ListItem(registro.codRegistro + " - " + registro.descrizione, registro.systemId));
                }
                if (!string.IsNullOrEmpty(passo.IdRF))
                {
                    ddlRegistroRF.SelectedValue = passo.IdRF;
                    if (SelectedSegnatureProcessOriginale != null)
                    {
                        //Se il passo aveva già un RF non consento di modificarlo all'avvio
                        PassoFirma passoOriginale = (from p in this.SelectedSegnatureProcessOriginale.passi where p.idPasso.Equals(passo.idPasso) select p).FirstOrDefault();
                        if (passoOriginale != null && !string.IsNullOrEmpty(passoOriginale.IdRF))
                            ddlRegistroRF.Enabled = false;
                    }
                }
                else
                {
                    ddlRegistroRF.SelectedIndex = 0;
                }
            }

            return ddlRegistroRF;
        }

        private DropDownList LoadRegistriRFSpedizione(PassoFirma passo)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            DropDownList ddlRegistroRF = new DropDownList();
            ddlRegistroRF.ID = "ddlRegistroRF_" + passo.idPasso.ToString();
            ddlRegistroRF.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SignatureProcessesDdlRegistroRFMittente", language));
            ddlRegistroRF.CssClass = "chzn-select-deselect";
            ddlRegistroRF.Items.Add(new ListItem("", ""));
            ddlRegistroRF.SelectedIndex = -1;
            ddlRegistroRF.Width = 200;
            ddlRegistroRF.Attributes.Add("onchange", "disallowOp('Content2');");
            ddlRegistroRF.AutoPostBack = true;
            ddlRegistroRF.SelectedIndexChanged += ddlRegistroRF_OnTextChanged;
            ddlRegistroRF.Enabled = false;
            if (!string.IsNullOrEmpty(passo.IdAOO))
            {
                ddlRegistroRF.Enabled = true;
                ddlRegistroRF.Items.Add(new ListItem("", ""));
                Registro reg = RegistryManager.getRegistroBySistemId(passo.IdAOO);
                ddlRegistroRF.Items.Add(new ListItem(reg.codRegistro, passo.IdAOO));
                NttDataWA.DocsPaWR.Registro[] rf = RegistryManager.GetListRegistriesAndRF(passo.ruoloCoinvolto.systemId, "1", passo.IdAOO);
                foreach (NttDataWA.DocsPaWR.Registro registro in rf)
                {
                    DataSet ds = MultiBoxManager.GetRightMailRegistro(registro.systemId, passo.ruoloCoinvolto.systemId);
                    if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                        {
                            if (row["SPEDISCI"].ToString().Equals("1"))
                            {
                                ListItem item = new ListItem(registro.codRegistro + " - " + registro.descrizione, registro.systemId);
                                ddlRegistroRF.Items.Add(item);
                                break;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(passo.IdRF))
                {
                    ddlRegistroRF.SelectedValue = passo.IdRF;
                    //Se il passo aveva già un RF non consento di modificarlo all'avvio
                    if (SelectedSegnatureProcessOriginale != null)
                    {
                        PassoFirma passoOriginale = (from p in this.SelectedSegnatureProcessOriginale.passi where p.idPasso.Equals(passo.idPasso) select p).FirstOrDefault();
                        if (passoOriginale != null && !string.IsNullOrEmpty(passoOriginale.IdRF))
                            ddlRegistroRF.Enabled = false;
                    }
                }
                else
                {
                    ddlRegistroRF.SelectedIndex = 0;
                }

            }
            return ddlRegistroRF;
        }

        private Panel AddCaselleRegistro(PassoFirma passo)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            Panel content = new Panel();
            content.EnableViewState = false;

            Label lblCasellaMittente = new Label();
            lblCasellaMittente.ID = "lblCasellaMittente_" + passo.idPasso;
            lblCasellaMittente.EnableViewState = false;
            lblCasellaMittente.CssClass = "weight";
            lblCasellaMittente.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLtlElencoCaselle", language) + " *";
            content.Controls.Add(lblCasellaMittente);

            DropDownList ddlCasellaMittente = new DropDownList();
            ddlCasellaMittente.EnableViewState = false;
            ddlCasellaMittente.Enabled = false;
            ddlCasellaMittente.ID = "ddlCasellaMittente_" + passo.idPasso.ToString();
            ddlCasellaMittente.CssClass = "chzn-select-deselect";
            ddlCasellaMittente.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SignatureProcessesDdlElencoCaselle", language));
            ddlCasellaMittente.Width = 200;
            ddlCasellaMittente.Attributes.Add("onchange", "disallowOp('Content2');");
            ddlCasellaMittente.AutoPostBack = true;
            ddlCasellaMittente.SelectedIndexChanged += ddlCasellaMittente_OnTextChanged;
            ddlCasellaMittente.Enabled = false;
            if (!string.IsNullOrEmpty(passo.IdRF))
            {
                ddlCasellaMittente.Enabled = true;
                List<DocsPaWR.CasellaRegistro> listCaselle = new List<DocsPaWR.CasellaRegistro>();
                ddlCasellaMittente.Items.Add(new ListItem("", ""));
                listCaselle = GetComboRegisterSend(passo.IdRF, passo.ruoloCoinvolto.systemId);
                if (listCaselle.Count > 0)
                {
                    ddlCasellaMittente.Enabled = true;
                    foreach (DocsPaWR.CasellaRegistro c in listCaselle)
                    {
                        System.Text.StringBuilder formatMail = new System.Text.StringBuilder();
                        if (c.Principale.Equals("1"))
                            formatMail.Append("* ");
                        formatMail.Append(c.EmailRegistro);
                        if (!string.IsNullOrEmpty(c.Note))
                        {
                            formatMail.Append(" - ");
                            formatMail.Append(c.Note);
                        }
                        ddlCasellaMittente.Items.Add(new ListItem(formatMail.ToString(), c.System_id));
                    }

                    //imposto la casella principale come selezionata
                    foreach (ListItem i in ddlCasellaMittente.Items)
                    {
                        if (i.Text.Split(new string[] { "*" }, 2, System.StringSplitOptions.None).Length > 1)
                        {
                            ddlCasellaMittente.SelectedValue = i.Value;
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(passo.IdMailRegistro))
                    {
                        ddlCasellaMittente.SelectedValue = passo.IdMailRegistro;
                    }
                    else
                    {
                        ddlCasellaMittente.SelectedIndex = -1;
                    }
                }
            }
            Panel ddl = new Panel();
            ddl.CssClass = "row";
            ddl.EnableViewState = false;
            ddl.Controls.Add(ddlCasellaMittente);

            content.Controls.Add(ddl);
            return content;
        }

        /// <summary>
        /// Resistuisce l'elenco delle caselle associate al registro/rf per le quali il ruolo è abilitato in spedizione
        /// </summary>
        /// <returns>List CasellaRegistro</returns>
        private List<CasellaRegistro> GetComboRegisterSend(string idRegistro, string idRuolo)
        {
            try
            {
                List<CasellaRegistro> listCaselle = new List<CasellaRegistro>();
                string casellaPrincipale = MultiBoxManager.GetMailPrincipaleRegistro(idRegistro);
                DataSet ds = MultiBoxManager.GetRightMailRegistro(idRegistro, idRuolo);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                        {
                            System.Text.StringBuilder formatMail = new System.Text.StringBuilder();
                            if (row["SPEDISCI"].ToString().Equals("1"))
                            {
                                listCaselle.Add(new CasellaRegistro
                                {
                                    Principale = row["EMAIL_REGISTRO"].ToString().Equals(casellaPrincipale) ? "1" : "0",
                                    EmailRegistro = row["EMAIL_REGISTRO"].ToString(),
                                    Note = row["VAR_NOTE"].ToString(),
                                    System_id = row["ID_MAIL_REGISTRI"].ToString()
                                });
                            }
                        }
                    }
                }
                return listCaselle;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private AjaxControlToolkit.AutoCompleteExtender SetAjaxAddressBook(CustomTextArea ctxDescription, CustomTextArea ctxCode, Button btnRecipient)
        {
            AjaxControlToolkit.AutoCompleteExtender rapidRuoloTitolareCustom = new AjaxControlToolkit.AutoCompleteExtender();
            string idPasso = ctxDescription.ID.Replace("ctxDescRuolo_", "");
            rapidRuoloTitolareCustom.ID = "rapidRuoloTitolareCustom_" + idPasso;
            rapidRuoloTitolareCustom.TargetControlID = ctxDescription.ID;
            rapidRuoloTitolareCustom.CompletionListCssClass = "autocomplete_completionListElement";
            rapidRuoloTitolareCustom.CompletionListItemCssClass = "single_item";
            rapidRuoloTitolareCustom.CompletionListHighlightedItemCssClass = "single_item_hover";
            rapidRuoloTitolareCustom.ServiceMethod = "GetListaCorrispondentiVeloce";
            rapidRuoloTitolareCustom.CompletionInterval = 1000;
            rapidRuoloTitolareCustom.EnableCaching = true;
            rapidRuoloTitolareCustom.CompletionSetCount = 20;
            rapidRuoloTitolareCustom.DelimiterCharacters = ";";
            rapidRuoloTitolareCustom.ServicePath = "~/AjaxProxy.asmx";
            rapidRuoloTitolareCustom.ShowOnlyCurrentWordInCompletionListItem = true;
            rapidRuoloTitolareCustom.UseContextKey = true;
            rapidRuoloTitolareCustom.MinimumPrefixLength = AjaxAddressBookMinPrefixLenght;
            rapidRuoloTitolareCustom.Enabled = true;
            string dataUser = UIManager.RoleManager.GetRoleInSession().systemId;
            dataUser = dataUser + "-" + UIManager.RegistryManager.GetRegistryInSession().systemId;
            rapidRuoloTitolareCustom.ContextKey = dataUser + "-" + UIManager.UserManager.GetInfoUser().idAmministrazione + "-" + RubricaCallType.CALLTYPE_CORR_INT.ToString();
            rapidRuoloTitolareCustom.BehaviorID = "behavior_" + this.ClientID;
            string b = "behavior_" + this.ClientID;
            rapidRuoloTitolareCustom.OnClientPopulated = "acePopulated_" + idPasso + this.ClientID;
            rapidRuoloTitolareCustom.OnClientItemSelected = "aceSelected_" + idPasso + this.ClientID;
            string nomeFunzionePopulated = "acePopulated_" + idPasso + this.ClientID;
            string nomeFunzioneSelected = "aceSelected_"+ idPasso + this.ClientID;
            string unique = this.UniqueID;
            builderJS(b, nomeFunzionePopulated, nomeFunzioneSelected, unique, ctxDescription, ctxCode, btnRecipient);

            return rapidRuoloTitolareCustom;
        }

        private void builderJS(string b, string nomeFunzionePopulated, string nomeFunzioneSelected, string uniqueID, CustomTextArea ctxDescription, CustomTextArea ctxCode, Button btnRecipient)
        {
            //Populated
            StringBuilder sbPopulated = new StringBuilder();
            sbPopulated.AppendLine("function " + nomeFunzionePopulated + "(sender, e) {");
            sbPopulated.AppendLine("var behavior = $find('" + b + "');");
            sbPopulated.AppendLine("var target = behavior.get_completionList();");
            sbPopulated.AppendLine("if (behavior._currentPrefix != null) {");
            sbPopulated.AppendLine("var prefix = behavior._currentPrefix.toLowerCase();");
            sbPopulated.AppendLine("var i;");
            sbPopulated.AppendLine("for (i = 0; i < target.childNodes.length; i++) {");
            sbPopulated.AppendLine("var sValue = target.childNodes[i].innerHTML.toLowerCase();");
            sbPopulated.AppendLine("if (sValue.indexOf(prefix) != -1) {");
            sbPopulated.AppendLine("var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));");
            sbPopulated.AppendLine(
                "var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);");
            sbPopulated.AppendLine(
                "var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);");
            sbPopulated.AppendLine("target.childNodes[i].innerHTML = fstr + '<span class=\"selectedWord\">' + pstr + '</span>' + estr;");
            sbPopulated.AppendLine("try");
            sbPopulated.AppendLine("{");
            sbPopulated.AppendLine("target.childNodes[i].attributes[\"_value\"].value = fstr + pstr + estr;");
            sbPopulated.AppendLine("}");
            sbPopulated.AppendLine("catch (ex)");
            sbPopulated.AppendLine("{");
            sbPopulated.AppendLine("target.childNodes[i].attributes[\"_value\"] = fstr + pstr + estr;");
            sbPopulated.AppendLine("}");
            sbPopulated.AppendLine("}");
            sbPopulated.AppendLine("}");
            sbPopulated.AppendLine("}");
            sbPopulated.AppendLine("}");

            //Response.Write(sbPopulated.ToString());
            ScriptManager.RegisterStartupScript(this, this.GetType(), nomeFunzioneSelected + "1", sbPopulated.ToString(), true);

            StringBuilder sbSelected = new StringBuilder();
            sbSelected.AppendLine("function " + nomeFunzioneSelected + "(sender, e) {");
            sbSelected.AppendLine("var value = e.get_value();");
            sbSelected.AppendLine("if (!value)");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("if (e._item.parentElement && e._item.parentElement.tagName == \"LI\")");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("try");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("value = e._item.parentElement.attributes[\"_value\"].value;");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("catch (ex1)");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("value = e._item.parentElement.attributes[\"_value\"];");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("if (value == undefined || value == null)");
            sbSelected.AppendLine("value = e._item.parentElement.attributes[\"_value\"];");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == \"LI\")");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("try");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("value = e._item.parentElement.attributes[\"_value\"].value;");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("catch (ex1)");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("value = e._item.parentElement.attributes[\"_value\"];");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("if (value == undefined || value == null)");
            sbSelected.AppendLine("value = e._item.parentElement.attributes[\"_value\"];");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("else if (e._item.parentNode && e._item.parentNode.tagName == \"LI\")");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("value = e._item.parentNode._value;");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == \"LI\")");
            sbSelected.AppendLine("{");
            sbSelected.AppendLine("value = e._item.parentNode.parentNode._value;");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("else value = \"\";");
            sbSelected.AppendLine("}");
            sbSelected.AppendLine("var searchText = $get('" + ctxDescription.ClientID + "').value;");
            sbSelected.AppendLine("searchText = searchText.replace('null', '');");
            sbSelected.AppendLine("var testo = value;");
            sbSelected.AppendLine("var indiceFineCodice = testo.lastIndexOf(')');");
            sbSelected.AppendLine("document.getElementById('" + ctxDescription.ClientID + "').focus();");
            sbSelected.AppendLine("document.getElementById('" + ctxDescription.ClientID + "').value = \"\";");
            sbSelected.AppendLine("var indiceDescrizione = testo.lastIndexOf('(');");
            sbSelected.AppendLine("var descrizione = testo.substr(0, indiceDescrizione - 1);");
            sbSelected.AppendLine("var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);");
            sbSelected.AppendLine("document.getElementById('" + ctxCode.ClientID + "').value = codice;");
            sbSelected.AppendLine("document.getElementById('" + ctxDescription.ClientID + "').value = descrizione;");
            sbPopulated.Append("disallowOp('Content2');");
            sbSelected.AppendLine("document.getElementById('" + btnRecipient.ClientID + "').click();");
            //sbSelected.AppendLine("setTimeout(\"__doPostBack('txt_Codice',''), 0\");");
            sbSelected.AppendLine("}");
            ScriptManager.RegisterStartupScript(this, this.GetType(), nomeFunzioneSelected + "2", sbSelected.ToString(), true);

        }

        protected void TreeSignatureProcess_Collapsed(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
        }

        private void TreeviewProcesses_Bind()
        {
            if (ListaProcessiDiFirma != null && ListaProcessiDiFirma.Count > 0)
            {
                foreach (ProcessoFirma p in ListaProcessiDiFirma)
                {
                    this.AddNode(p);
                }
                this.TreeProcessSignature.DataBind();
                this.TreeProcessSignature.CollapseAll();
            }

        }

        #region Treeview Singolo Processo
        
        protected void TreeViewDetailsProcess_Collapsed(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
        }

        #endregion


        private TreeNode AddNode(ProcessoFirma p)
        {
            TreeNode root = new TreeNode();
            string nomeProcesso = p.nome + " " + (p.IsProcessModel ? Utils.Languages.GetLabelFromCode("ManagementSignatureProcessModel", UserManager.GetUserLanguage()) : string.Empty);
            if (p.isInvalidated)
            {
                root.SelectAction = TreeNodeSelectAction.None;
                root.Text = p.isInvalidated ? "<strike>" + nomeProcesso + "</strike>" : nomeProcesso;
            }
            else
            {
                root.Text = nomeProcesso;
            }
            root.Value = p.idProcesso;
            root.ToolTip = nomeProcesso;
            foreach (PassoFirma passo in p.passi)
            {
                this.AddChildrenElements(passo, ref root);
            }
            this.TreeProcessSignature.Nodes.Add(root);
            return root;
        }

        private TreeNode AddChildrenElements(PassoFirma p, ref TreeNode root)
        {
            TreeNode nodeChild = new TreeNode();

            nodeChild.ImageUrl = LibroFirmaManager.GetIconEventType(p);
            nodeChild.Value = p.idPasso;
            nodeChild.Text = LibroFirmaManager.GetHolder(p);
            nodeChild.ToolTip = LibroFirmaManager.GetHolder(p);
            nodeChild.SelectAction = TreeNodeSelectAction.None;
            root.ChildNodes.Add(nodeChild);

            return nodeChild;
        }

        #endregion

        #region Event button

       
        protected void StartProcessSignatureClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["from"]) && Request.QueryString["from"].Equals("SearchDocument"))
                {
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('StartProcessSignature','up');", true);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('StartProcessSignature','');", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void StartProcessSignatureAssigns_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            try
            {
                TreeNode node = this.TreeProcessSignature.SelectedNode;

                if (node != null)
                {
                    ProcessoFirma selectedProcess = (from processo in this.ListaProcessiDiFirma where processo.idProcesso.Equals(node.Value) select processo).FirstOrDefault();

                    if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_TIPO_RUOLO_LIBRO_FIRMA.ToString())) && Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_TIPO_RUOLO_LIBRO_FIRMA.ToString()) != "0")
                    {
                        //Verifico se nel processo sono presenti passi a Tipo ruolo, in caso positivo verifico se esiste un ruolo con il tipo ruolo specificato e rispetto
                        //al ruolo che avvia. Se esiste un tipo ruolo per cui non è possibile determinare il ruolo, interrompo l'avvio del processo
                        List<TipoRuolo> typeRoleInSteps = (from passo in selectedProcess.passi
                                                           where passo.TpoRuoloCoinvolto != null && !string.IsNullOrEmpty(passo.TpoRuoloCoinvolto.systemId)
                                                           select passo.TpoRuoloCoinvolto).ToList();
                        if (typeRoleInSteps != null && typeRoleInSteps.Count > 0)
                        {
                            List<TipoRuolo> listTypeRoleNoMatchRole = SignatureProcessesManager.CheckExistsRoleSupByTypeRoles(typeRoleInSteps);
                            if (listTypeRoleNoMatchRole == null)
                            {
                                string msg = "ErrorStartProcessSignature";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                                return;
                            }
                            else if (listTypeRoleNoMatchRole.Count > 0)
                            {
                                string typeRoleNoMatch = string.Empty;
                                foreach (TipoRuolo t in listTypeRoleNoMatchRole)
                                    typeRoleNoMatch += "<li> " + t.descrizione + "</li>";

                                string msgDesc = "WarningStartProcessSignatureTypeRoleNoMatch";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                return;
                            }
                        }
                    }

                    //Se il processo è un modello, verifico che siano stati inseriti i campi obbligatori.
                    if (selectedProcess.IsProcessModel && !CheckCampiObbligatoriModello(selectedProcess))
                    {
                        string msgDesc = "WarningStartProcessSignatureTypeModelAssignRole";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(msgDesc) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(msgDesc) + "');}; ", true);
                        return;
                    }
                    #region AVVIO PROCESSO DI FIRMA MASSIVO
                    if (!string.IsNullOrEmpty(Request.QueryString["from"]) && Request.QueryString["from"].Equals("SearchDocument"))
                    {
                        List<SchedaDocumento> schedaDocumentList = LoadSchedaDocumentsList(MassiveOperationUtils.GetSelectedItems());
                        FileRequest fileReq;
                        if (schedaDocumentList != null && schedaDocumentList.Count > 0)
                        {
                            MassiveOperationReport.MassiveOperationResultEnum result;
                            MassiveOperationReport report = new MassiveOperationReport();
                            string details = string.Empty;
                            string codice = string.Empty;
                            List<FileRequest> fileRequestList = new List<FileRequest>();
                            foreach (SchedaDocumento schedaDoc in schedaDocumentList)
                            {
                                result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                                details = String.Empty;

                                result = CanStartSignatureProcess(schedaDoc, out details);

                                if (result != MassiveOperationReport.MassiveOperationResultEnum.KO)
                                {
                                    fileReq = schedaDoc.documenti[0];
                                    if (schedaDoc.documentoPrincipale != null && !string.IsNullOrEmpty(schedaDoc.documentoPrincipale.docNumber))
                                    {
                                        Allegato att = new Allegato()
                                        {
                                            docNumber = fileReq.docNumber,
                                            versionId = fileReq.versionId,
                                            versionLabel = fileReq.versionLabel,
                                            descrizione = fileReq.descrizione,
                                            version = fileReq.version,
                                            fileSize = fileReq.fileSize,
                                            firmato = fileReq.firmato,
                                            fileName = fileReq.fileName,
                                            tipoFirma = fileReq.tipoFirma

                                        };
                                        fileRequestList.Add(att);
                                    }
                                    else
                                        fileRequestList.Add(fileReq);
                                }
                                else
                                {
                                    codice = MassiveOperationUtils.getItem(schedaDoc.docNumber).Codice;
                                    report.AddReportRow(
                                        codice,
                                        result,
                                        details);
                                }
                            }

                            //AVVIO IL PROCESSO DI FIRMA PER I DOC SELEZIONATI
                            if (fileRequestList != null && fileRequestList.Count > 0)
                            {
                                OpzioniNotifica opzioniNotifiche = new OpzioniNotifica();
                                opzioniNotifiche.Notifica_concluso = this.cbxNotificationOption.Items.FindByValue("cbxNotificationOptionOptCP").Selected;
                                opzioniNotifiche.Notifica_interrotto = this.cbxNotificationOption.Items.FindByValue("cbxNotificationOptionOptIP").Selected;

                                ListItem cbxErrore = this.cbxNotificationOption.Items.FindByValue("cbxNotificationOptionOptErrorePassoAutomaticoP");
                                opzioniNotifiche.NotificaErrore = cbxErrore != null && cbxErrore.Selected;

                                ListItem cbxDestNonInterop = this.cbxNotificationOption.Items.FindByValue("cbxNotificationOptionOptDestNonInterop");
                                opzioniNotifiche.NotificaPresenzaDestNonInterop = cbxDestNonInterop != null && cbxDestNonInterop.Selected;

                                List<FirmaResult> firmaResult = SignatureProcessesManager.StartProccessSignatureMassive(selectedProcess, fileRequestList, this.txtNotes.Text, opzioniNotifiche);
                                if (firmaResult != null && ((firmaResult.Count > 1) || (firmaResult.Count == 1 && firmaResult[0].fileRequest != null)))
                                {
                                    foreach (FirmaResult r in firmaResult)
                                    {
                                        if (string.IsNullOrEmpty(r.errore))
                                        {
                                            result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                                            details = "Avvio del processo di firma avvenuto correttamente";
                                            codice = MassiveOperationUtils.getItem(r.fileRequest.docNumber).Codice;
                                            report.AddReportRow(
                                                codice,
                                                result,
                                                details);
                                        }
                                        else
                                        {
                                            string msg = string.Empty;
                                            string msgErrore = string.Empty;
                                            string language = UserManager.GetUserLanguage();
                                            ResultProcessoFirma errore = (ResultProcessoFirma)Enum.Parse(typeof(ResultProcessoFirma), r.errore, true);
                                            msg = GetMessageError(errore);
                                            msgErrore = Utils.Languages.GetMessageFromCode(msg, language);
                                            result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                                            codice = MassiveOperationUtils.getItem(r.fileRequest.docNumber).Codice;
                                            details = String.Format(
                                                "Si sono verificati degli errori durante l'avvio del processo di firma. Dettagli: {0}",
                                                msgErrore);
                                            report.AddReportRow(
                                                codice,
                                                result,
                                                details);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (FileRequest fr in fileRequestList)
                                    {
                                        result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                                        codice = MassiveOperationUtils.getItem(fr.docNumber).Codice;
                                        details = "Si sono verificati degli errori durante l'avvio del processo di firma.";
                                        report.AddReportRow(
                                            codice,
                                            result,
                                            details);
                                    }
                                }
                            }

                            // Introduzione della riga di summary
                            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
                            report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);

                            this.generateReport(report, "Avvio del processo massivo");
                        }
                    }
                    #endregion
                    #region AVVIO PROCESSO DI FIRMA DAL TAB PROFILO
                    else
                    {
                        DocsPaWR.FileRequest fileReq = null;

                        if (FileManager.GetSelectedAttachment() == null)
                        {
                            fileReq = UIManager.FileManager.getSelectedFile();
                            //Se stò avviando il processo dal dettaglio dell'allegato converto il fileRequest in allegato
                            if (DocumentManager.getSelectedRecord().documentoPrincipale != null && !string.IsNullOrEmpty(DocumentManager.getSelectedRecord().documentoPrincipale.docNumber))
                            {
                                fileReq = this.ConvertiFileRequestInAllegato(fileReq);
                            }
                        }
                        else
                        {
                            fileReq = FileManager.GetSelectedAttachment();
                        }
                        if (string.IsNullOrEmpty(fileReq.fileSize) || Convert.ToUInt32(fileReq.fileSize) == 0)
                        {
                            string msgDesc = "WarningStartProcessSignatureFileNonAcquisito";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(msgDesc) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(msgDesc) + "');}", true);
                            return;
                        }
                        string extensionFile = FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToLowerInvariant();
                        if (!verifyExtensionForSign(extensionFile))
                        {
                            string msgDesc = "WarningStartProcessSignatureFileNonAmmessoAllaFirma";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(msgDesc) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(msgDesc) + "');}", true);
                            return;
                        }
                        if(fileReq.firmato.Equals("1") && !CheckTipoFirma(fileReq.tipoFirma))
                        {
                            string msgDesc = "WarningStartProcessSignatureFileFirmatoCADES";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(msgDesc) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(msgDesc) + "');}", true);
                            return;
                        }
                        DocsPaWR.ResultProcessoFirma resultAvvioProcesso = ResultProcessoFirma.OK;
                        OpzioniNotifica opzioniNotifiche = new OpzioniNotifica();
                        opzioniNotifiche.Notifica_concluso = this.cbxNotificationOption.Items.FindByValue("cbxNotificationOptionOptCP").Selected;
                        opzioniNotifiche.Notifica_interrotto = this.cbxNotificationOption.Items.FindByValue("cbxNotificationOptionOptIP").Selected;

                        ListItem cbxErrore = this.cbxNotificationOption.Items.FindByValue("cbxNotificationOptionOptErrorePassoAutomaticoP");
                        opzioniNotifiche.NotificaErrore = cbxErrore != null && cbxErrore.Selected;

                        ListItem cbxDestNonInterop = this.cbxNotificationOption.Items.FindByValue("cbxNotificationOptionOptDestNonInterop");
                        opzioniNotifiche.NotificaPresenzaDestNonInterop = cbxDestNonInterop != null && cbxDestNonInterop.Selected;

                        bool resultAvvio = false;
                        if (CambiaStatoDocumento)
                            resultAvvio = SignatureProcessesManager.SalvaModificaStatoStartSignatureProcess(selectedProcess, fileReq, this.txtNotes.Text, opzioniNotifiche,
                                this.IdStatoSelezionatoDocumento, StateDiagram, string.Empty, this.Page,
                                out resultAvvioProcesso);
                        else
                            resultAvvio = SignatureProcessesManager.StartProccessSignature(selectedProcess, fileReq, this.txtNotes.Text, opzioniNotifiche, out resultAvvioProcesso);
                        if (resultAvvio)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('StartProcessSignature','up');", true);
                        }
                        else
                        {
                            string msg = GetMessageError(resultAvvioProcesso);                       
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return;
                        }
                    }
                    #endregion
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private bool CheckCampiObbligatoriModello(ProcessoFirma processo)
        {
            bool result = true;

            foreach(PassoFirma passo in processo.passi)
            {
                if(passo.IsModello)
                {
                    if((passo.ruoloCoinvolto == null || string.IsNullOrEmpty(passo.ruoloCoinvolto.idGruppo))
                        && (passo.TpoRuoloCoinvolto == null || string.IsNullOrEmpty(passo.TpoRuoloCoinvolto.systemId)))
                    {
                        result = false;
                        break;
                    }
                    if(passo.IsAutomatico && (string.IsNullOrEmpty(passo.IdAOO) || string.IsNullOrEmpty(passo.IdAOO)))
                    {
                        result = false;
                        break;
                    }
                    if (passo.IsAutomatico && passo.Evento.CodiceAzione.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) && string.IsNullOrEmpty(passo.IdMailRegistro))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Verifica se al dcoumento che sto avviando è associato un diagramma di stato con avvio automatico di un processo di firma
        /// </summary>
        /// <returns></returns>
        private bool CheckDiagramStateWithStartProcess()
        {
            bool result = false;

            return result;
        }

        private Boolean verifyExtensionForSign(string extension)
        {
            Boolean retVal = true;

            if (SupportedFileTypesEnabled)
            {
                if (this.FileTypes == null)
                    FileTypes = UIManager.FileManager.GetSupportedFileTypes(Convert.ToInt32(UIManager.UserManager.GetInfoUser().idAmministrazione));
                int count = FileTypes.Count(e => e.FileExtension.ToLowerInvariant() == extension.ToLowerInvariant() &&
                                                        e.FileTypeUsed && e.FileTypeSignature);
                retVal = (count > 0);
            }
            else
                retVal = true;

            return retVal;
        }

        private MassiveOperationReport.MassiveOperationResultEnum CanStartSignatureProcess(SchedaDocumento schedaDoc, out string details)
        {
            // Risultato della verifica
            MassiveOperationReport.MassiveOperationResultEnum retValue = MassiveOperationReport.MassiveOperationResultEnum.OK;
            System.Text.StringBuilder detailsBS = new System.Text.StringBuilder();
            bool isPdf = (FileManager.getEstensioneIntoSignedFile(schedaDoc.documenti[0].fileName).ToUpper() == "PDF");

            string msgError = string.Empty;

            #region FILE ACQUISITO
            if (string.IsNullOrEmpty(schedaDoc.documenti[0].fileSize) || Convert.ToInt32(schedaDoc.documenti[0].fileSize) == 0)
            {
                msgError = "File non acquisito.";
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                details = msgError;
                return retValue;
            }
            #endregion
            #region DIMENSIONE FILE
            if (this.MaxDimFileSign > 0 && Convert.ToInt32(schedaDoc.documenti[0].fileSize) > this.MaxDimFileSign)
            {
                string maxSize = Convert.ToString(Math.Round((double)this.MaxDimFileSign / 1048576, 3));
                msgError = "La dimensione del file supera il limite massimo consentito per la firma. Il limite massimo consentito è: " + maxSize + " Mb.";
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                details = msgError;
                return retValue;
            }
            #endregion
            #region CONTROLLO TIPO FIRMA
            if(!CheckTipoFirma(schedaDoc.documenti[0].tipoFirma))
            {
                msgError = "Il file è firmato CADES, non è possibile applicare una firma PADES";
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                details = msgError;
                return retValue;
            }
            #endregion
            #region DOCUMENTO IN LIBRO FIRMA
            if (schedaDoc.documenti[0].inLibroFirma)
            {
                msgError = "File già presente in libro firma.";
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                details = msgError;
                return retValue;
            }
            #endregion

            #region VERFICA L'ESTENSIONE DEL FILE
            string extensionFile = FileManager.getEstensioneIntoSignedFile(schedaDoc.documenti[0].fileName).ToLowerInvariant();
            if (!verifyExtensionForSign(extensionFile))
            {
                msgError = "Non è stato possibile avviare il processo di firma. Il formato del file non è ammesso alla firma.";
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                details = msgError;
                return retValue;
            }
            #endregion

            #region DOCUMENTO IN ATTESA DI ACCETTAZIONE

            if (!string.IsNullOrEmpty(schedaDoc.accessRights) && Convert.ToInt32(schedaDoc.accessRights) == Convert.ToInt32(HMdiritti.HDdiritti_Waiting))
            {
                msgError = "Il documento è in attesa di accetazione.";
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                details = msgError;
                return retValue;
            }

            if (!string.IsNullOrEmpty(schedaDoc.accessRights) && Convert.ToInt32(schedaDoc.accessRights) == Convert.ToInt32(HMdiritti.HMdiritti_Read))
            {
                msgError = "Il documento è in sola lettura.";
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                details = msgError;
                return retValue;
            }
            #endregion

            #region DOCUMENTO CONSOLIDATO

            if (schedaDoc.ConsolidationState != null && schedaDoc.ConsolidationState.State != DocsPaWR.DocumentConsolidationStateEnum.None)
            {
                msgError = "Processo di firma non avviato in quanto il documento è consolidato.";
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                details = msgError;
                return retValue;
            }


            #endregion

            #region DOCUMENTO IN CHECKOUT

            if (schedaDoc.checkOutStatus != null && !string.IsNullOrEmpty(schedaDoc.checkOutStatus.ID))
            {
                msgError = "Processo di firma non avviato in quanto il documento è bloccato.";
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                details = msgError;
                return retValue;
            }

            #endregion


            if (!string.IsNullOrEmpty(msgError))
            {
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                detailsBS.Append(msgError);
            }

            details = detailsBS.ToString();
            return retValue;

        }

        private void generateReport(MassiveOperationReport report, string titolo)
        {
            this.grdReport.DataSource = report.GetDataSet();
            this.grdReport.DataBind();
            this.pnlReport.Visible = true;
            this.plcSignatureProcesses.Visible = false;
            this.PnlPassi.Visible = false;
            this.PnlDettaglioProcessi.Visible = false;
            this.PnlDettaglioPassi.Controls.Clear();
            this.UpDettaglioProcessi.Update();
            this.UpPnlDettaglioPassi.Update();
            this.upPnlSignatureProcesses.Update();
            this.upReport.Update();

            string template = "../xml/massiveOp_formatPdfExport.xml";
            report.GenerateDataSetForExport(Server.MapPath(template), titolo);

            this.StartProcessSignatureAssigns.Enabled = false;
            this.BtnReport.Visible = true;
            this.UpPnlButtons.Update();
        }

        protected void BtnReport_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "reallowOp", "reallowOp();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "visualReport", "parent.ajaxModalPopupMassiveReport();", true);
        }

        private bool IsRoleEnabledSignature(Ruolo role, out string error, PassoFirma passo)
        {
            bool isAuthorizedSign = true;
            bool isPassoTipoFirma = false;
            error = string.Empty;
            if (passo.Evento.CodiceAzione.Equals(LibroFirmaManager.TypeEvent.SIGN_CADES) || passo.Evento.CodiceAzione.Equals(LibroFirmaManager.TypeEvent.SIGN_PADES))
            {
                isPassoTipoFirma = true;
                isAuthorizedSign = ((from function in role.funzioni
                                        where function.codice.ToUpper().Equals("DO_DOC_FIRMA")
                                        select function.systemId).Count() > 0 ||
                                (from function in role.funzioni
                                    where function.codice.ToUpper().Equals("FIRMA_HSM")
                                    select function.systemId).Count() > 0);
            }
            if (passo.Evento.CodiceAzione.Equals(LibroFirmaManager.TypeEvent.VERIFIED))
            {
                isPassoTipoFirma = true;
                isAuthorizedSign = ((from function in role.funzioni
                                        where function.codice.ToUpper().Equals("DO_DOC_FIRMA_ELETTRONICA")
                                        select function.systemId).Count() > 0);
            }
            if (passo.Evento.CodiceAzione.Equals(LibroFirmaManager.TypeEvent.ADVANCEMENT_PROCESS))
            {
                isPassoTipoFirma = true;
                isAuthorizedSign = ((from function in role.funzioni
                                        where function.codice.ToUpper().Equals("DO_DOC_AVANZAMENTO_ITER")
                                        select function.systemId).Count() > 0);
            }
            if(isPassoTipoFirma)
            {
                bool isAuthorizedLf = ((from function in role.funzioni
                                           where function.codice.ToUpper().Equals("DO_LIBRO_FIRMA")
                                           select function.systemId).Count() > 0);
                if (!isAuthorizedLf)
                {
                    error = "WarningNotAuthorizedLibroFirma";
                    isAuthorizedSign = false;
                }
            }
            return isAuthorizedSign;
        }

        public bool IsRoleEnableLibroFirma(Ruolo role, Passo passo)
        {
            bool result = true;



            return result;
        }

        /// <summary>
        /// Se il file è firmato cades e esiste un passo con firma pades non posso avviare il processo
        /// </summary>
        /// <param name="tipoFirma"></param>
        /// <returns></returns>
        public bool CheckTipoFirma(string tipoFirma)
        {
            bool result = true;

            if (!string.IsNullOrEmpty(tipoFirma) &&
                (tipoFirma.Equals(NttDataWA.Utils.TipoFirma.CADES) || tipoFirma.Equals(NttDataWA.Utils.TipoFirma.CADES_ELETTORNICA)))
            {
                TreeNode node = this.TreeProcessSignature.SelectedNode;
                if (node != null)
                {
                    ProcessoFirma selectedProcess = (from processo in this.ListaProcessiDiFirma where processo.idProcesso.Equals(node.Value) select processo).FirstOrDefault();
                    bool existsPades = (from passo in selectedProcess.passi
                                        where passo.Evento.CodiceAzione.Equals(LibroFirmaManager.TypeEvent.SIGN_PADES)
                                        select passo).FirstOrDefault() != null;
                    if (existsPades)
                        result = false;
                }
            }

            return result;
        }

        protected void BtnAddressBook_Click(object sender, EventArgs e)
        {
            this.CallType = RubricaCallType.CALLTYPE_CORR_INT;
            HttpContext.Current.Session["AddressBook.from"] = "START_SIGNATURE_PROCESS";
            HttpContext.Current.Session["AddressBook.EnableOnly"] = "R";
            CustomImageButton btnRubrica = (CustomImageButton)this.UpPnlDettaglioPassi.FindControl((((CustomImageButton)sender).ID));
            this.IdObjectRuoloTitolare = btnRubrica.ID.Replace("btnRubrica_", "");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAddressBook", "parent.ajaxModalPopupAddressBookFromPopup();", true);
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                if (atList != null && atList.Count > 0)
                {
                    Corrispondente corr = null;
                    //Profiler document
                    string idPasso = this.IdObjectRuoloTitolare;
                    //CustomTextArea userCorr = (CustomTextArea)this.UpPnlDettaglioPassi.FindControl(this.IdObjectRuoloTitolare);

                    string idAmm = UIManager.UserManager.GetInfoUser().idAmministrazione;
                    foreach (NttDataWA.Popup.AddressBook.CorrespondentDetail addressBookCorrespondent in atList)
                    {

                        if (!addressBookCorrespondent.isRubricaComune)
                        {
                            corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(addressBookCorrespondent.SystemID);
                        }
                        else
                        {
                            corr = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(addressBookCorrespondent.CodiceRubrica);
                        }

                    }
                    Ruolo ruolo = RoleManager.GetRuolo(corr.systemId);
                    CustomTextArea ctxCodRuolo = (CustomTextArea)this.UpPnlDettaglioPassi.FindControl("ctxCodRuolo_" + this.IdObjectRuoloTitolare);
                    CustomTextArea ctxDescRuolo = (CustomTextArea)this.UpPnlDettaglioPassi.FindControl("ctxDescRuolo_" + this.IdObjectRuoloTitolare);
                    DropDownList ddlUtenteCoinvolto = (DropDownList)this.UpPnlDettaglioPassi.FindControl("ddlUtente_" + idPasso);
                    ddlUtenteCoinvolto.Items.Clear();
                    PassoFirma passo = GetPassoDaAggiornare(idPasso);
                    passo.ruoloCoinvolto = new Ruolo();
                    passo.utenteCoinvolto = new Utente();
                    if (ruolo != null)
                    {

                        string errorMessage = string.Empty;
                        if (IsRoleEnabledSignature(ruolo, out errorMessage, passo) && !corr.disabledTrasm)
                        {
                            ctxCodRuolo.Text = corr.codiceRubrica;
                            ctxDescRuolo.Text = corr.descrizione;

                            //Aggiorno il passo
                            passo.ruoloCoinvolto = ruolo;
                            if (!passo.IsAutomatico)
                                PopolaComboTitolare(idPasso, ruolo.idGruppo);
                            else
                                PopolaRegistroAOO(idPasso);
                        }
                        else
                        {
                            ctxCodRuolo.Text = string.Empty;
                            ctxDescRuolo.Text = string.Empty;
                            string msg = "WarningRoleNotEnabledSign";
                            if (corr.disabledTrasm)
                                msg = "WarningRoleDisabledTrasm";
                            if (!string.IsNullOrEmpty(errorMessage))
                                msg = errorMessage;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        passo.DaAggiornare = true;
                        AggiornaPassoTreeView(passo);
                    }
                    
                    this.UpPnlDettaglioPassi.Update();
                }
                HttpContext.Current.Session["AddressBook.At"] = null;
                HttpContext.Current.Session["AddressBook.Cc"] = null;
                HttpContext.Current.Session["AddressBook.type"] = null;
                HttpContext.Current.Session["AddressBook.from"] = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void rblTypeSignatureD_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            try
            {
                RadioButtonList rblTypeSignatureD = (RadioButtonList)this.UpPnlDettaglioPassi.FindControl((((RadioButtonList)sender).ID));
                string idPasso = rblTypeSignatureD.ID.Replace("rbl_", "");
                PassoFirma passo = GetPassoDaAggiornare(idPasso);
                TreeNode node = this.TreeProcessSignature.SelectedNode;

                ProcessoFirma processo = (from p in this.ListaProcessiDiFirma
                                            where p.idProcesso.Equals(node.Value)
                                            select p).FirstOrDefault();

                //Una passo di firma pades non può essere preceduto da un passo di firma CADES
                bool canInsertStepSign = true;
                if (rblTypeSignatureD.SelectedValue.Equals(LibroFirmaManager.TypeEvent.SIGN_PADES))
                {
                    canInsertStepSign = (from i in processo.passi
                                              where i.numeroSequenza < passo.numeroSequenza && i.Evento.CodiceAzione.Equals(LibroFirmaManager.TypeEvent.SIGN_CADES)
                                              select i).FirstOrDefault() == null;
                    if (!canInsertStepSign)
                    {
                        rblTypeSignatureD.SelectedValue = passo.Evento.CodiceAzione;
                        string msg = "WarningNotStepSignAfterCades";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                }
                if(canInsertStepSign)
                {
                    passo.Evento.CodiceAzione = rblTypeSignatureD.SelectedValue;
                    passo.DaAggiornare = true;
                    AggiornaPassoTreeView(passo);
                    ViewDetailsSteps(processo);
                }
            }
            catch(Exception ex)
            {
                string msg = "ErrorSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            try
            {
                CustomTextArea ctxCodRuolo = (CustomTextArea)this.UpPnlDettaglioPassi.FindControl((((CustomTextArea)sender).ID));
                string idPasso = ctxCodRuolo.ID.Replace("ctxCodRuolo_", "");
                CustomTextArea ctxDescRuolo = (CustomTextArea)this.UpPnlDettaglioPassi.FindControl("ctxDescRuolo_" + idPasso);
                PassoFirma passo = GetPassoDaAggiornare(idPasso);
                passo.ruoloCoinvolto = new Ruolo();
                passo.utenteCoinvolto = new Utente();
                if(!passo.IsAutomatico)
                {
                    DropDownList ddlUtenteCoinvolto = (DropDownList)this.UpPnlDettaglioPassi.FindControl("ddlUtente_" + idPasso);
                    ddlUtenteCoinvolto.Items.Clear();
                }
                if (!string.IsNullOrEmpty(ctxCodRuolo.Text))
                {
                    RubricaCallType calltype = RubricaCallType.CALLTYPE_PROTO_INT_MITT;
                    ElementoRubrica[] listaCorr = null;
                    Corrispondente corr = null;
                    UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
                    listaCorr = UIManager.AddressBookManager.getElementiRubricaMultipli(ctxCodRuolo.Text, calltype, true);
                    if (listaCorr != null && (listaCorr.Count() == 1))
                    {
                        if (listaCorr.Count() == 1)
                        {
                            corr = UIManager.AddressBookManager.getCorrispondenteRubrica(ctxCodRuolo.Text, calltype);
                        }
                        if (corr == null || !corr.tipoCorrispondente.Equals("R"))
                        {
                            ctxCodRuolo.Text = string.Empty;
                            ctxDescRuolo.Text = string.Empty;

                            string msg = "ErrorTransmissionCorrespondentNotFound";
                            if (!corr.tipoCorrispondente.Equals("R"))
                                msg = "WarningCorrespondentAsRole";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            Ruolo ruolo = RoleManager.GetRuolo(corr.systemId);
                            string errorMessage = string.Empty;
                            if (IsRoleEnabledSignature(ruolo, out errorMessage, passo) && !corr.disabledTrasm)
                            {
                                ctxCodRuolo.Text = corr.codiceRubrica;
                                ctxDescRuolo.Text = corr.descrizione;

                                //Aggiorno il passo
                                passo.ruoloCoinvolto = ruolo;
                                if (!passo.IsAutomatico)
                                    PopolaComboTitolare(idPasso, ruolo.idGruppo);
                            }
                            else
                            {
                                ctxCodRuolo.Text = string.Empty;
                                ctxDescRuolo.Text = string.Empty;
                                string msg = "WarningRoleNotEnabledSign";
                                if (corr.disabledTrasm)
                                    msg = "WarningRoleDisabledTrasm";
                                if (!string.IsNullOrEmpty(errorMessage))
                                    msg = errorMessage;
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                        }
                    }
                    else
                    {
                        ctxCodRuolo.Text = string.Empty;
                        ctxDescRuolo.Text = string.Empty;
                        string msg = "ErrorTransmissionCorrespondentNotFound";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                }
                else
                {
                    ctxCodRuolo.Text = string.Empty;
                    ctxDescRuolo.Text = string.Empty;
                }
                passo.DaAggiornare = true;
                if (passo.IsAutomatico)
                    PopolaRegistroAOO(passo.idPasso);
                AggiornaPassoTreeView(passo);
            }
            catch (Exception ex)
            {
                string msg = "ErrorSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        private void PopolaComboTitolare(string idPasso, string idGruppo)
        {
            DropDownList ddlUtenteCoinvolto = (DropDownList)this.UpPnlDettaglioPassi.FindControl("ddlUtente_" + idPasso);
            ddlUtenteCoinvolto.Items.Clear();
            List<Utente> listUserInRole = UIManager.UserManager.getUserInRoleByIdGruppo(idGruppo);
            if (listUserInRole != null && listUserInRole.Count > 0)
            {
                ListItem empty = new ListItem("", "");
                ddlUtenteCoinvolto.Items.Add(empty);
                ddlUtenteCoinvolto.SelectedIndex = -1;

                for (int i = 0; i < listUserInRole.Count; i++)
                {
                    ListItem item = new ListItem(listUserInRole[i].descrizione, listUserInRole[i].systemId);
                    ddlUtenteCoinvolto.Items.Add(item);
                }

                ddlUtenteCoinvolto.Enabled = true;
            }
        }

        private void PopolaRegistroAOO(string idPasso)
        {
            DropDownList ddlRegistroAOO = (DropDownList)this.UpPnlDettaglioPassi.FindControl("ddlRegistroAOO_" + idPasso);
            PassoFirma passo = GetPassoDaAggiornare(idPasso);
            ddlRegistroAOO.Enabled = false;
            ddlRegistroAOO.Items.Clear();
            if (passo.ruoloCoinvolto != null && !string.IsNullOrEmpty(passo.ruoloCoinvolto.idGruppo))
            {
                ddlRegistroAOO.Enabled = true;
                ddlRegistroAOO.Items.Add(new ListItem("", ""));
                Registro[] registriAOO = UIManager.RegistryManager.GetRegistriesByRole(passo.ruoloCoinvolto.systemId);
                foreach (DocsPaWR.Registro reg in registriAOO)
                {
                    if (!reg.flag_pregresso)
                    {
                        ddlRegistroAOO.Items.Add(new ListItem(reg.codRegistro, reg.systemId));
                    }
                }
                ddlRegistroAOO.SelectedIndex = 0;

                if (ddlRegistroAOO.Items != null && ddlRegistroAOO.Items.Count == 2)
                {
                    ddlRegistroAOO.SelectedIndex = 1;
                    passo.IdAOO = registriAOO[0].systemId;
                }
            }
            passo.IdAOO = ddlRegistroAOO.SelectedValue;

            DropDownList ddlRegistroRF = (DropDownList)this.UpPnlDettaglioPassi.FindControl("ddlRegistroRF_" + idPasso);
            ddlRegistroRF.Items.Clear();
            ddlRegistroRF.Enabled = false;
            AggiornaRegistriRF(passo);


            if (passo.Evento.CodiceAzione.Equals(Azione.DOCUMENTOSPEDISCI.ToString()))
                AggiornaCaselleMailRegistri(passo.idPasso);
        }

        private PassoFirma GetPassoDaAggiornare(string idPasso)
        {
            PassoFirma passo = new PassoFirma();
            TreeNode node = this.TreeProcessSignature.SelectedNode;
            if (node != null)
            {
                ProcessoFirma processo = (from p in this.ListaProcessiDiFirma
                                          where p.idProcesso.Equals(node.Value)
                                          select p).FirstOrDefault();
                passo = (from p in processo.passi
                                    where p.idPasso.Equals(idPasso)
                                    select p).FirstOrDefault();
            }
            return passo;
        }


        protected void ddlUtenteTitolare_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            string idPasso = (((DropDownList)sender).ID).Replace("ddlUtente_", "");
            DropDownList ddlUtenteCoinvolto = (DropDownList)this.UpPnlDettaglioPassi.FindControl((((DropDownList)sender).ID));
            PassoFirma passo = GetPassoDaAggiornare(idPasso);
            if (!string.IsNullOrEmpty(ddlUtenteCoinvolto.SelectedValue))
                passo.utenteCoinvolto = new Utente() { idPeople = ddlUtenteCoinvolto.SelectedValue, descrizione = ddlUtenteCoinvolto.SelectedItem.Text };
            else
                passo.utenteCoinvolto = new Utente();
            AggiornaPassoTreeView(passo);
        }

        protected void ddlRegistroAOO_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            string idPasso = (((DropDownList)sender).ID).Replace("ddlRegistroAOO_", "");
            DropDownList ddlRegistroAOO = (DropDownList)this.UpPnlDettaglioPassi.FindControl((((DropDownList)sender).ID));
            PassoFirma passo = GetPassoDaAggiornare(idPasso);
            passo.IdAOO = ddlRegistroAOO.SelectedValue;

            //Aggiorno l'RF
            AggiornaRegistriRF(passo);
            
            passo.DaAggiornare = true;
        }

        private void AggiornaRegistriRF(PassoFirma passo)
        {
            DropDownList ddlRegistroRF = (DropDownList)this.UpPnlDettaglioPassi.FindControl("ddlRegistroRF_" + passo.idPasso);
            ddlRegistroRF.Items.Clear();
            ddlRegistroRF.Enabled = false;
            passo.IdRF = string.Empty;
            ddlRegistroRF.Items.Add(new ListItem("", ""));
            if (passo.Evento.CodiceAzione.Equals(Azione.DOCUMENTOSPEDISCI.ToString()))
            {
                if (!string.IsNullOrEmpty(passo.IdAOO))
                {
                    Registro reg = RegistryManager.getRegistroBySistemId(passo.IdAOO);
                    //prendo il registro corrente
                    DataSet dsReg = MultiBoxManager.GetRightMailRegistro(reg.systemId, passo.ruoloCoinvolto.systemId);
                    if (dsReg.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                    {
                        foreach (DataRow row in dsReg.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                        {
                            if (row["SPEDISCI"].ToString().Equals("1"))
                            {
                                ddlRegistroRF.Items.Add(new ListItem(reg.codRegistro, reg.systemId));
                                break;
                            }
                        }
                    }

                    NttDataWA.DocsPaWR.Registro[] rf = RegistryManager.GetListRegistriesAndRF(passo.ruoloCoinvolto.systemId, "1", passo.IdAOO);
                    if (rf != null & rf.Count() > 0)
                    {
                        foreach (NttDataWA.DocsPaWR.Registro registro in rf)
                        {
                            DataSet ds = MultiBoxManager.GetRightMailRegistro(registro.systemId, passo.ruoloCoinvolto.systemId);
                            if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                            {
                                foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                                {
                                    if (row["SPEDISCI"].ToString().Equals("1"))
                                    {
                                        ListItem item = new ListItem(registro.codRegistro + " - " + registro.descrizione, registro.systemId);
                                        ddlRegistroRF.Items.Add(item);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (ddlRegistroRF.Items.Count == 2)
                        ddlRegistroRF.SelectedIndex = 1;

                    passo.IdRF = ddlRegistroRF.SelectedValue;
                    ddlRegistroRF.Enabled = true;
                }

                //Nel caso di spedizione aggiorno anche la combo delle caselle
                AggiornaCaselleMailRegistri(passo.idPasso);
            }
            else
            {
                if (!string.IsNullOrEmpty(passo.IdAOO))
                {
                    Registro[] registriRfVisibili = UIManager.RegistryManager.GetListRegistriesAndRF(passo.ruoloCoinvolto.systemId, "1", passo.IdAOO);
                    if (registriRfVisibili != null & registriRfVisibili.Count() > 0)
                    {
                        foreach (NttDataWA.DocsPaWR.Registro registro in registriRfVisibili)
                        {
                            ddlRegistroRF.Items.Add(new ListItem(registro.codRegistro + " - " + registro.descrizione, registro.systemId));
                        }

                        if(registriRfVisibili.Count() == 1)
                        {
                            ddlRegistroRF.SelectedIndex = 1;
                        }

                        ddlRegistroRF.Enabled = true;
                        passo.IdRF = ddlRegistroRF.SelectedValue;
                    }
                }
            }
        }


        protected void ddlRegistroRF_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            string idPasso = (((DropDownList)sender).ID).Replace("ddlRegistroRF_", "");
            DropDownList ddlRegistroRF = (DropDownList)this.UpPnlDettaglioPassi.FindControl((((DropDownList)sender).ID));
            PassoFirma passo = GetPassoDaAggiornare(idPasso);
            passo.IdRF = ddlRegistroRF.SelectedValue;

            //Aggiorno se passo di spedizione, modifico la combo della casella
            if (passo.Evento.CodiceAzione.Equals(Azione.DOCUMENTOSPEDISCI.ToString()))
            {
                AggiornaCaselleMailRegistri(passo.idPasso);
            }
            passo.DaAggiornare = true;
        }

        protected void ddlCasellaMittente_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            string idPasso = (((DropDownList)sender).ID).Replace("ddlCasellaMittente_", "");
            DropDownList ddlCasellaMittente = (DropDownList)this.UpPnlDettaglioPassi.FindControl((((DropDownList)sender).ID));
            PassoFirma passo = GetPassoDaAggiornare(idPasso);
            passo.IdMailRegistro = ddlCasellaMittente.SelectedValue;
            passo.DaAggiornare = true;
        }

        private void AggiornaCaselleMailRegistri(string idPasso)
        {
            DropDownList ddlCasellaMittente = (DropDownList)this.UpPnlDettaglioPassi.FindControl("ddlCasellaMittente_" + idPasso);
            ddlCasellaMittente.Items.Clear();
            ddlCasellaMittente.Enabled = false;
            PassoFirma passo = GetPassoDaAggiornare(idPasso);
            passo.IdMailRegistro = string.Empty;
            if (!string.IsNullOrEmpty(passo.IdRF))
            {
                List<DocsPaWR.CasellaRegistro> listCaselle = new List<DocsPaWR.CasellaRegistro>();
                listCaselle = GetComboRegisterSend(passo.IdRF, passo.ruoloCoinvolto.systemId);
                if (listCaselle.Count > 0)
                {
                    ddlCasellaMittente.Enabled = true;
                    foreach (DocsPaWR.CasellaRegistro c in listCaselle)
                    {
                        System.Text.StringBuilder formatMail = new System.Text.StringBuilder();
                        if (c.Principale.Equals("1"))
                            formatMail.Append("* ");
                        formatMail.Append(c.EmailRegistro);
                        if (!string.IsNullOrEmpty(c.Note))
                        {
                            formatMail.Append(" - ");
                            formatMail.Append(c.Note);
                        }
                        ddlCasellaMittente.Items.Add(new ListItem(formatMail.ToString(), c.System_id));
                    }

                    //imposto la casella principale come selezionata
                    foreach (ListItem i in ddlCasellaMittente.Items)
                    {
                        if (i.Text.Split(new string[] { "*" }, 2, System.StringSplitOptions.None).Length > 1)
                        {
                            ddlCasellaMittente.SelectedValue = i.Value;
                            break;
                        }
                    }
                    passo.IdMailRegistro = ddlCasellaMittente.SelectedValue;
                }
            }
        }

        private void AggiornaPassoTreeView(PassoFirma passo)
        {
            TreeNode node = this.TreeProcessSignature.SelectedNode;
            if (node != null)
            {
                foreach (TreeNode nodeChild in node.ChildNodes)
                {
                    if (nodeChild.Value.Equals(passo.idPasso))
                    {
                        nodeChild.ImageUrl = LibroFirmaManager.GetIconEventType(passo);
                        nodeChild.Value = passo.idPasso;
                        nodeChild.Text = LibroFirmaManager.GetHolder(passo);
                        nodeChild.ToolTip = LibroFirmaManager.GetHolder(passo);
                    }
                }
            }
            this.UpdatePanelTreeView.Update();
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

            return file;
        }

        private string GetMessageError(ResultProcessoFirma errore)
        {
            string msg = string.Empty;

            switch (errore)
            {
                case ResultProcessoFirma.DOCUMENTO_GIA_IN_LIBRO_FIRMA:
                    msg = "ErrorStartProcessSignatureDocInLibroFirma";
                    break;
                case ResultProcessoFirma.DOCUMENTO_CONSOLIDATO:
                    msg = "ErrorStartProcessSignatureDocConsolidato";
                    break;
                case ResultProcessoFirma.DOCUMENTO_BLOCCATO:
                    msg = "ErrorStartProcessSignatureDocCheckout";
                    break;
                case ResultProcessoFirma.PASSO_PROTO_DOC_GIA_PROTOCOLLATO:
                    msg = "ErrorStartProcessSignaturePassoProtoSuDocProtocollato";
                    break;
                case ResultProcessoFirma.PASSO_PROTO_DOC_NON_PREDISPOSTO:
                    msg = "ErrorStartProcessSignaturePassoProtoSuDocNonPredisposto";
                    break;
                case ResultProcessoFirma.PASSO_REP_DOC_GIA_REPERTORIATO:
                    msg = "ErrorStartProcessSignaturePassoRepSuDocReperotriato";
                    break;
                case ResultProcessoFirma.PASSO_REP_DOC_NON_TIPIZZATO:
                    msg = "ErrorStartProcessSignaturePassoRepDocNonTipizzato";
                    break;
                case ResultProcessoFirma.PASSO_REP_NESSUN_CONTATORE_TIPO_DOC:
                    msg = "ErrorStartProcessSignaturePassoRepNoContatoreTipoDoc";
                    break;
                case ResultProcessoFirma.PASSO_REP_NO_DIRITTI_SCRITTURA_CONTATORE:
                    msg = "ErrorStartProcessSignaturePassoRepNoDirittiScritturaContatore";
                    break;
                case ResultProcessoFirma.PASSO_REP_RF_MANCANTE:
                    msg = "ErrorStartProcessSignaturePassoRepRFMancante";
                    break;
                case ResultProcessoFirma.PASSO_SPEDIZIONE_PROTO_ARRIVO:
                    msg = "ErrorStartProcessSignaturePassoSpedizioneSuDocArrivo";
                    break;
                case ResultProcessoFirma.PASSO_PADES_SU_FILE_CADES:
                    msg = "WarningStartProcessSignatureFileFirmatoCADES";
                    break;
                case ResultProcessoFirma.PASSO_SPEDIZIONE_DOC_NON_PROTOCOLLATO:
                    msg = "WarningStartProcessSignatureStepSpedizioneAutoDocNonProto";
                    break;
                case ResultProcessoFirma.PASSO_AUTOMATICO_REGISTRO_ERRATO:
                    msg = "WarningStartProcessSignatureSPassoAutomaticoRegistroErrato";
                    break;
                case ResultProcessoFirma.PASSO_PROTO_REG_CHIUSO:
                    msg = "WarningStartProcessSignatureSPassoAutomaticoRegistroChiuso";
                    break;
                default:
                    msg = "ErrorStartProcessSignature";
                    break;
            }
            return msg;
        }
    }
}