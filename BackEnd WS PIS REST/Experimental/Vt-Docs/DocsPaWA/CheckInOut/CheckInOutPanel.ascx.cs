namespace DocsPAWA.CheckInOut
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.IO;
    using DocsPAWA.DocsPaWR;
    using DocsPAWA.SiteNavigation;

    /// <summary>
    ///	Controllo per la gestione dei pulsanti delle funzioni di checkin - checkout
    /// </summary>
    public class CheckInOutPanel : System.Web.UI.UserControl
    {
        protected DocsPaWebCtrlLibrary.ImageButton btnSave;
        protected DocsPaWebCtrlLibrary.ImageButton btnCheckIn;
        protected DocsPaWebCtrlLibrary.ImageButton btnCheckOut;
        protected DocsPaWebCtrlLibrary.ImageButton btnUndoCheckOut;
        protected DocsPaWebCtrlLibrary.ImageButton btnOpenCheckedOutFile;
        protected DocsPaWebCtrlLibrary.ImageButton btnShowCheckOutStatus;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtExtension;

        /// <summary>
        /// Evento relativo al CheckOut di un documento
        /// </summary>
        public event EventHandler OnCheckOut = null;

        /// <summary>
        /// Evento relativo al CheckIn di un documento
        /// </summary>
        public event EventHandler OnCheckIn = null;

        /// <summary>
        /// Evento relativo all'UndoCheckOut di un documento
        /// </summary>
        public event EventHandler OnUndoCheckOut = null;

        protected System.Web.UI.HtmlControls.HtmlInputHidden txtResponse;

        private void Page_Load(object sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// Temporeaneo
        /// </summary>
        protected CheckOutStatus DocumentCheckOutStatus
        {
            get
            {
                if (Session["DocumentCheckOutStatus"] != null)
                    return (CheckOutStatus)Session["DocumentCheckOutStatus"];
                else
                    return null;
            }
            set
            {
                Session["DocumentCheckOutStatus"] = value;
            }
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            // Aggiornamento abilitazione / disabilitazione pulsanti
            this.RefreshButtons();
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCheckOut.Click += new System.Web.UI.ImageClickEventHandler(this.btnCheckOut_Click);
            this.btnCheckIn.Click += new System.Web.UI.ImageClickEventHandler(this.btnCheckIn_Click);
            this.btnUndoCheckOut.Click += new System.Web.UI.ImageClickEventHandler(this.btnUndoCheckOut_Click);
            this.btnOpenCheckedOutFile.Click += new System.Web.UI.ImageClickEventHandler(this.btnOpenCheckedOutFile_Click);
            this.btnShowCheckOutStatus.Click += new System.Web.UI.ImageClickEventHandler(this.btnShowCheckOutStatus_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);

        }
        #endregion

        //		/// <summary>
        //		/// Azione di blocco di un documento
        //		/// </summary>
        //		/// <param name="downloadFile"></param>
        //		public void PerformActionCheckOut(bool downloadFile)
        //		{
        //			this.PerformActionCheckOut(string.Empty,downloadFile);
        //		}
        //
        //		/// <summary>
        //		/// Azione di blocco di un documento
        //		/// </summary>
        //		/// <param name="defaultFilePath"></param>
        //		/// <param name="downloadFile"></param>
        //		public void PerformActionCheckOut(string defaultFilePath,bool downloadFile)
        //		{
        //			this.RegisterClientScript("PerformActionCheckOut","if (CheckOut('" + defaultFilePath + "'," + downloadFile.ToString().ToLower() + ")) { SubmitForm('" + this.FormID + "'); }");
        //		}
        //
        //		/// <summary>
        //		/// 
        //		/// </summary>
        //		public void PerformActionCheckIn()
        //		{
        //			this.RegisterClientScript("PerformActionCheckIn","if (CheckIn('" + this.txtResponse.ClientID + "')) { SubmitForm('" + this.FormID + "'); }");
        //		}
        //
        //		/// <summary>
        //		/// 
        //		/// </summary>
        //		public void PerformActionUndoCheckOut()
        //		{
        //			this.RegisterClientScript("PerformActionUndoCheckOut","if (UndoCheckOut('" + this.txtResponse.ClientID + "')) { SubmitForm('" + this.FormID + "'); }");
        //		}
        //
        //		/// <summary>
        //		/// 
        //		/// </summary>
        //		public void PerformActionOpenFile()
        //		{
        //			this.RegisterClientScript("PerformActionOpenFile","OpenFile();");
        //		}

        /// <summary>
        /// Impostazione e reperimento del path statico del folder checkinout
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public string RelativeFolderPath
        {
            get
            {
                return this.CheckInOutController.RelativeFolderPath;
            }
            set
            {
                this.CheckInOutController.RelativeFolderPath = value;
            }
        }

        /// <summary>
        /// Verifica se l'utente corrente è abilitato alla funzione di checkin-checkout
        /// </summary>
        public bool UserEnabled
        {
            get
            {
                bool retValue = false;

                if (this.ViewState["IsUserEnabled"] == null)
                {
                    retValue = this.CheckInOutController.UserEnabled;

                    this.ViewState["IsUserEnabled"] = retValue;
                }
                else
                {
                    retValue = Convert.ToBoolean(this.ViewState["IsUserEnabled"]);
                }

                ////Se il Documento è in conversione pdf lato server va abilitata la fuznione di checkin-checkout
                //if (this.CurrentSchedaDocumento != null && DocumentManager.isDocInConversionePdf(UserManager.getInfoUtente(), this.CurrentSchedaDocumento.systemId))
                //{
                //    RegisterClientScriptEvents();
                //    retValue = true;
                //}
                return retValue;
            }
        }

        /// <summary>
        /// Verifica se l'ultima versione acquisita del file è firmata
        /// </summary>
        protected bool IsSignedFile
        {
            get
            {
                bool retValue = false;

                // Reperimento ultimo file acquisito dal documento
                FileRequest lastDocument = this.LastDocument;

                // Verifica se è stato acquisito un file
                if (this.IsAcquired(lastDocument))
                    retValue = (lastDocument.fileName.ToLower().EndsWith(".p7m"));

                return retValue;
            }
        }

        /// <summary>
        /// Verifica se l'ultima versione acquisita del file è un timeStamp
        /// </summary>
        protected bool IsTimeStampedFile
        {
            get
            {
                bool retValue = false;

                // Reperimento ultimo file acquisito dal documento
                FileRequest lastDocument = this.LastDocument;

                // Verifica se è stato acquisito un file
                if (this.IsAcquired(lastDocument))
                    retValue = (
                        lastDocument.fileName.ToLower().EndsWith(".tsd")||
                        lastDocument.fileName.ToLower().EndsWith(".m7m")
                        );

                return retValue;
            }
        }
        /// <summary>
        /// Reperimento dell'estensione del file acquisito
        /// relativamente all'ultima versione del documento
        /// </summary>
        /// <returns></returns>
        protected string LastAcquiredDocumentExtension
        {
            get
            {
                string retValue = string.Empty;

                if (this.ViewState["LastAcquiredDocumentExtension"] == null)
                {
                    if (this.UserEnabled)
                    {
                        // Reperimento ultimo file acquisito dal documento
                        FileRequest lastDocument = this.LastDocument;

                        // Verifica se è stato acquisito un file
                        if (this.IsAcquired(lastDocument))
                        {
                            // Reperimento oggetto "FileDocumento" contenente
                            // il nome originale del file, indipendentemente 
                            // dal documentale utilizzato correntemente
                            FileManager fileManager = new FileManager();
                            DocsPaWR.FileDocumento fileDocument = fileManager.getInfoFile(this.Page, lastDocument);

                            string defaultFileName = fileDocument.name;

                            // In caso di file firmato, viene reperita l'estensione originaria del file
                            while (defaultFileName.ToLower().EndsWith(".p7m"))
                                defaultFileName = defaultFileName.Substring(0, defaultFileName.Length - 4);

                            FileInfo fileInfo = new FileInfo(defaultFileName);
                            retValue = fileInfo.Extension.Replace(".", "");
                        }
                    }

                    this.ViewState.Add("LastAcquiredDocumentExtension", retValue);
                }
                else
                {
                    retValue = this.ViewState["LastAcquiredDocumentExtension"].ToString();
                }

                return retValue;
            }
        }

        /// <summary>
        /// Inizializzazione pannello checkinout
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        public void Initialize(string idDocument, string documentNumber)
        {
            if (!this.IsPostBack)
            {
                // Impostazione parametri relativi al documento corrente
                this.IDDocument = idDocument;
                this.DocumentNumber = documentNumber;

                // Registrazione eventi javascript
                this.RegisterClientScriptEvents();

                // Verifica se l'utente è abilitato all'utilizzo della funzione
                if (this.UserEnabled)
                {
                    // Inizializzazione del contesto di checkout per il documento corrente
                    CheckInOutServices.InitializeContext();
                }
            }
        }

        /// <summary>
        /// Verifica se un documento è acquisito
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        private bool IsAcquired(FileRequest fileRequest)
        {
            return (fileRequest != null &&
                    fileRequest.fileName != null &&
                    fileRequest.fileName != string.Empty &&
                    fileRequest.fileSize != null &&
                    fileRequest.fileSize != "0");
        }

        /// <summary>
        /// Reperimento percorso del file per il documento in CheckOut
        /// </summary>
        protected string CheckOutFilePath
        {
            get
            {
                string retValue = string.Empty;

                if (CheckOutContext.Current != null && CheckOutContext.Current.Status != null)
                    retValue = CheckOutContext.Current.Status.DocumentLocation;

                return retValue.Replace(@"\", @"\\");
            }
        }

        /// <summary>
        /// Reperimento usercontrol "CheckInOutController"
        /// </summary>
        /// <returns></returns>
        public CheckInOutController CheckInOutController
        {
            get
            {
                return this.FindControl("checkInOutController") as CheckInOutController;
            }
        }

        /// <summary>
        /// Reperimento ID della form container del controllo
        /// </summary>
        protected string FormID
        {
            get
            {
                Control parentControl = this.Parent;

                while (Parent.GetType() != typeof(HtmlForm))
                    parentControl = parentControl.Parent;

                return parentControl.ClientID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string IDDocument
        {
            get
            {
                if (this.ViewState["IDDocument"] != null)
                    return this.ViewState["IDDocument"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["IDDocument"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string DocumentNumber
        {
            get
            {
                if (this.ViewState["DocumentNumber"] != null)
                    return this.ViewState["DocumentNumber"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["DocumentNumber"] = value;
            }
        }

        /// <summary>
        /// Utente proprietario del blocco sul documento
        /// </summary>
        protected string CheckOutOwnerUser
        {
            get
            {
                if (this.ViewState["CheckOutOwnerUser"] != null)
                    return this.ViewState["CheckOutOwnerUser"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["CheckOutOwnerUser"] = value;
            }
        }

        /// <summary>
        /// Verifica se il documento corrente è in stato checkout
        /// </summary>
        protected bool IsCheckedOutDocument
        {
            get
            {
                bool ret = false;
                if (this.ViewState["IsCheckedOutDocument"] != null)
                    bool.TryParse(this.ViewState["IsCheckedOutDocument"].ToString(), out ret);
                return ret;
            }
            set
            {
                this.ViewState["IsCheckedOutDocument"] = value;
            }
        }

        //		/// <summary>
        //		/// 
        //		/// </summary>
        //		protected string DefaultCheckOutFileName
        //		{
        //			get
        //			{
        //				if (this.ViewState["DefaultCheckOutFileName"]!=null)
        //					return this.ViewState["DefaultCheckOutFileName"].ToString();
        //				else
        //					return string.Empty;
        //			}
        //		}

        /// <summary>
        /// 
        /// </summary>
        protected string ResponseControlID
        {
            get
            {
                return this.txtResponse.ClientID;
            }
        }

        private void btnCheckOut_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Se il checkout del documento è andato a buon fine
            if (this.txtResponse.Value.Equals("true") && this.OnCheckOut != null)
            {
                this.txtResponse.Value = string.Empty;

                this.OnCheckOut(this, new EventArgs());
            }
        }

        private void btnCheckIn_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Se il checkin del documento è andato a buon fine
            if (this.txtResponse.Value.Equals("true") && this.OnCheckIn != null)
            {
                this.txtResponse.Value = string.Empty;
                this.OnCheckIn(this, new EventArgs());
                CallContextStack.CurrentContext.ContextState["isCheckin"] = "isCheckin";
            }
        }

        private void btnUndoCheckOut_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Se l'undocheckout del documento è andato a buon fine
            if (this.txtResponse.Value.Equals("true") && this.OnUndoCheckOut != null)
            {
                this.txtResponse.Value = string.Empty;

                this.OnUndoCheckOut(this, new EventArgs());
            }
        }

        private void btnOpenCheckedOutFile_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        private void btnShowCheckOutStatus_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Visualizzazione pagina relativa alle informazioni sul documento checkedout
            this.CheckInOutController.ShowDialogCheckOutStatus(this.IDDocument, this.DocumentNumber);
        }

        /// <summary>
        /// Disabilitazione di tutti i pulsanti
        /// </summary>
        private void DisableAllButtons()
        {
            this.btnSave.Enabled = false;
            this.btnCheckOut.Enabled = false;
            this.btnCheckIn.Enabled = false;
            this.btnOpenCheckedOutFile.Enabled = false;
            this.btnUndoCheckOut.Enabled = false;
            this.btnShowCheckOutStatus.Enabled = false;
            this.btnShowCheckOutStatus.Visible = false;
        }

        /// <summary>
        /// Aggiornamento abilitazione / disabilitazione funzioni di checkin - checkout
        /// </summary>
        private void RefreshButtons()
        {
            this.DisableAllButtons();

            DocsPaWR.SchedaDocumento currentScheda = this.CurrentSchedaDocumento;

            if (currentScheda != null)
            {
                FileRequest selectedFile = FileManager.getSelectedFile();

                // Verifica stato consolidamento del documento (solo se non consolidato, si può fare checkout)
                if (currentScheda.ConsolidationState == null ||
                    (currentScheda.ConsolidationState != null && currentScheda.ConsolidationState.State == DocsPaWR.DocumentConsolidationStateEnum.None))
                {
                    if (!string.IsNullOrEmpty(currentScheda.systemId) && (currentScheda.tipoProto != "R" && currentScheda.tipoProto != "C"))
                    {
                        // NB: Nei casi in cui l'utente non è abilitato per l'utilizzo della funzione di checkin / checkout,
                        // le uniche funzionalità non disponibili saranno quelle relative al checkin e all'undocheckout.
                        // In ogni caso potrà vedere lo stato del documento e chi l'ha posto in checkout.

                        // Verifica se il documento non è né cestinato né annullato
                        if (!this.IsDocumentoInCestino && !this.IsDocumentoAnnullato)
                        {
                            CheckOutStatus status = CheckInOutServices.GetCheckOutDocumentStatus();

                            if (status != null)
                            {
                                // Il documento risulta bloccato, reperimento del proprietario del blocco
                                string ownerUser = status.UserName.ToUpper();
                                bool isOwnerCheckOut = (ownerUser == UserManager.getInfoUtente().userId.ToUpper());

                                if (this.UserEnabled && isOwnerCheckOut)
                                {
                                    // Documento bloccato dall'utente corrente: funzioni di rilascio abilitate
                                    this.btnCheckIn.Enabled = true;
                                    this.btnOpenCheckedOutFile.Enabled = true;
                                    this.btnUndoCheckOut.Enabled = true;

                                    if (DocsPAWA.UserManager.isFiltroAooEnabled(this.Page))
                                    {
                                        DocsPaWR.Registro[] userRegistri = DocsPAWA.UserManager.getListaRegistri(this.Page);
                                        this.btnCheckIn.Enabled = UserManager.verifyRegNoAOO(currentScheda, userRegistri);
                                        this.btnOpenCheckedOutFile.Enabled = UserManager.verifyRegNoAOO(currentScheda, userRegistri);
                                        this.btnUndoCheckOut.Enabled = UserManager.verifyRegNoAOO(currentScheda, userRegistri);
                                    }
                                }

                                this.btnShowCheckOutStatus.Enabled = true;
                                this.btnShowCheckOutStatus.Visible = true;
                                this.btnShowCheckOutStatus.ToolTip = "Il documento risulta bloccato dall'utente " + ownerUser;

                                //Verifico che il documento è bloccato per una richiesta di conversione pdf lato server
                                //In caso affermativo disabilito l'apertura del file ed il rilascia sen
                                if (status.InConversionePdf)
                                {
                                    this.btnOpenCheckedOutFile.Enabled = false;
                                    this.btnCheckIn.Enabled = false;

                                    // Se il documento è in stato di conversione pdf,
                                    // il pulsante di annullamento è abilitato solo se l'owner
                                    // del blocco è l'utente stesso
                                    this.btnUndoCheckOut.Enabled = isOwnerCheckOut;
                                }
                            }
                            else
                            {
                             //modifica
                            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                            
                            string idAmm = UserManager.getInfoUtente().idAmministrazione;
                            //DocsPAWA.DocsPaWR.CacheConfig info = ws.getConfigurazioneCache(idAmm);
                            if (ws.isActiveCache(idAmm))
                            {
                                bool inCache = ws.inCache(currentScheda.docNumber, currentScheda.documenti[currentScheda.documenti.Length - 1].versionId, idAmm);

                                if (inCache)
                                {
                                    string pathComponents = ws.recuperaPathComponents(currentScheda.docNumber, currentScheda.documenti[currentScheda.documenti.Length - 1].versionId);
                                    if (string.IsNullOrEmpty(pathComponents))
                                        this.btnCheckOut.Enabled = false;
                                    else
                                        this.btnCheckOut.Enabled = true;

                                    if (IsTimeStampedFile)
                                        this.btnCheckOut.Enabled = false;
                                }
                            }
                            else
                            //fine mofidica
                                if (Session["docInRisposta"] == null && Session["docInRisposta2"] == null)
                                {
                                    if (!this.IsEnabledProfilazioneAllegati &&
                                        this.IsSelectedTabAllegati && selectedFile.GetType() == typeof(DocsPaWR.Allegato))
                                    {
                                        // La funzione di checkout esplicita da tab allegati è disabilitata se la profilazione allegati è disabilitata
                                        this.btnCheckOut.Enabled = false;
                                    }
                                    else
                                    {
                                        this.btnCheckOut.Enabled = this.UserEnabled;

                                        if (IsTimeStampedFile)
                                            this.btnCheckOut.Enabled = false;

                                        if (DocsPAWA.UserManager.isFiltroAooEnabled(this.Page))
                                        {
                                            if (btnCheckOut.Enabled)
                                            {
                                                DocsPaWR.Registro[] userRegistri = DocsPAWA.UserManager.getListaRegistri(this.Page);
                                                btnCheckOut.Enabled = UserManager.verifyRegNoAOO(currentScheda, userRegistri);
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    this.btnCheckOut.Enabled = false;
                                    if (Session["docInRisposta"] != null)
                                        Session.Remove("docInRisposta");
                                    else
                                        if (Session["docInRisposta2"] != null)
                                            Session.Remove("docInRisposta2");
                                }
                            }
                        }
                    }
                }

                // Abilitazione / disabilitazione pulsante salva file,
                // che viene comunque abilitato (se file presente) 
                // indipendentemente dalle regole di abilitazione del checkin / checkout

                int size;
                if (selectedFile != null && Int32.TryParse(selectedFile.fileSize, out size))
                    this.btnSave.Enabled = (size > 0);
            }
        }

        /// <summary>
        /// Verifica se il documento corrente è annullato
        /// </summary>
        private bool IsDocumentoAnnullato
        {
            get
            {
                SchedaDocumento currentScheda = this.CurrentSchedaDocumento;

                return (currentScheda != null &&
                        currentScheda.protocollo != null &&
                        currentScheda.protocollo.protocolloAnnullato != null);
            }
        }

        /// <summary>
        /// Verifica se il documento è stato cestinato
        /// </summary>
        private bool IsDocumentoInCestino
        {
            get
            {
                SchedaDocumento currentScheda = this.CurrentSchedaDocumento;

                return (!string.IsNullOrEmpty(currentScheda.inCestino) &&
                                    currentScheda.inCestino == "1");
            }
        }

        /// <summary>
        /// Reperimento ultimo file acquisito nel documento
        /// </summary>
        private FileRequest LastDocument
        {
            get
            {
                FileRequest retValue = null;

                SchedaDocumento currentScheda = this.CurrentSchedaDocumento;

                if (currentScheda != null && currentScheda.documenti != null && currentScheda.documenti.Length > 0)
                    retValue = currentScheda.documenti[0];

                return retValue;
            }
        }

        /// <summary>
        /// Reperimento scheda documento corrente
        /// </summary>
        private SchedaDocumento CurrentSchedaDocumento
        {
            get
            {
                return CheckInOutServices.CurrentSchedaDocumento;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetCurrentFileExtensionList()
        {
            string retValue = string.Empty;

            DocsPaWR.FileDocumento fileDocument = new FileManager().getInfoFile(this.Page);

            if (fileDocument != null)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileDocument.fullName);

                retValue = fileInfo.Extension.Replace(".", string.Empty);
                
                //SKIZZO decommentare per la 3.0 (tsd m7m)
                if (retValue.ToUpper() == "P7M"
                    /*|| retValue.ToUpper() == "TSD"||
                    retValue.ToUpper() == "M7M"*/
                    )
                {
                    // Se il file è firmato, viene reperito il nome del file originale firmato
                    retValue += "|" + FileManager.getEstensioneIntoSignedFile(this.Page, fileDocument.fullName).ToUpper();
                }
            }

            return retValue;
        }

        /// <summary>
        /// Determina se è attiva la gestione della profilazione degli allegati
        /// </summary>
        protected bool IsEnabledProfilazioneAllegati
        {
            get
            {
                const string key = "CheckInOutPanel.IsEnabledProfilazioneAllegati";

                if (this.ViewState[key] == null)
                {
                    DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();
                    this.ViewState.Add(key, ws.IsEnabledProfilazioneAllegati());
                }

                return Convert.ToBoolean(this.ViewState[key]);
            }
        }

        /// <summary>
        /// Verifica, in base al contesto corrente, se il tab corrente della scheda documento è quella degli allegati
        /// </summary>
        /// <returns></returns>
        protected bool IsSelectedTabAllegati
        {
            get
            {
                SiteNavigation.CallContext context = SiteNavigation.CallContextStack.CurrentContext;

                string currentTab = context.QueryStringParameters["tab"] as string;

                return (!string.IsNullOrEmpty(currentTab) && currentTab.ToLower() == "allegati");
            }
        }

        /// <summary>
        /// Registrazione eventi javascript
        /// </summary>
        private void RegisterClientScriptEvents()
        {
            this.btnCheckOut.Attributes.Add("onClick", "return CheckOut('',true);");
            this.btnCheckIn.Attributes.Add("onClick", "return CheckIn();");
            this.btnOpenCheckedOutFile.Attributes.Add("onClick", "return OpenFile();");
            this.btnUndoCheckOut.Attributes.Add("onClick", "return UndoCheckOut();");
            this.btnShowCheckOutStatus.Attributes.Add("onClick", "return ShowCheckOutStatus();");
        }

        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }

        /// <summary>
        /// Path del documento in chackout salvato in locale
        /// </summary>
        protected String CheckOuFilePath
        {
            get
            {
                String retVal = String.Empty;
                if (CheckOutContext.Current != null &&
                    CheckOutContext.Current.Status != null &&
                    !String.IsNullOrEmpty(CheckOutContext.Current.Status.DocumentLocation))
                    retVal = CheckOutContext.Current.Status.DocumentLocation;

                return retVal;
            }
        }
    }
}
