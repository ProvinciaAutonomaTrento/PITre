using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Configuration;
using DocsPAWA.DocsPaWR;
using log4net;
/*
 * Andrea
 */
using DocsPAWA.utils;
/*
 * End Andrea 
 */


namespace DocsPAWA.documento
{
    /// <summary>
    /// Summary description for p_Trasmissioni.
    /// </summary>
    public class p_Trasmissioni : DocsPAWA.CssPage
    {
        /*
         * Andrea
         */
        private ArrayList listaExceptionTrasmissioni = new ArrayList();
        private string messError = "";
        /*
         * End Andrea
         */

        private ILog logger = LogManager.GetLogger(typeof(p_Trasmissioni));
        protected System.Web.UI.WebControls.Button ButtSel;
        protected System.Web.UI.WebControls.Button btn_ElDettDest;
        protected System.Web.UI.WebControls.Button btn_ElCancDest;
        protected System.Web.UI.WebControls.Label lbl_ruolo;
        protected System.Web.UI.WebControls.DropDownList ddl_registri;
        protected System.Web.UI.WebControls.Image img_statoReg;
        protected System.Web.UI.WebControls.DataGrid grdEffettuate;
        protected System.Web.UI.WebControls.DataGrid grdRicevute;
        protected Utilities.MessageBox msg_Trasmetti;
        protected DocsPaWebCtrlLibrary.ImageButton btn_nuovaT;
        protected System.Web.UI.WebControls.RadioButtonList rbl_tipoTrasm;
        protected DocsPaWebCtrlLibrary.ImageButton btn_NuovaTrasm;
        protected DocsPaWebCtrlLibrary.ImageButton btn_ModifTrasm;
        protected DocsPaWebCtrlLibrary.ImageButton butt_Trasm;
        protected System.Web.UI.WebControls.Label lbl_message;
        protected System.Web.UI.WebControls.DropDownList ddl_tmpl;
        protected System.Web.UI.WebControls.Panel pnl_trasm_rapida;
        protected DocsPaWebCtrlLibrary.ImageButton btn_salva_trasm;
        protected System.Web.UI.WebControls.HyperLink stampa;
        protected DocsPaWebCtrlLibrary.ImageButton btn_stampa;
        protected System.Web.UI.HtmlControls.HtmlInputHidden flag_template;
        protected DocsPaWebCtrlLibrary.ImageButton btn_trasmettiDisabled;

        protected DocsPaWebService wws = new DocsPaWebService();
        protected const string separatore = "----------------";

        protected string _idDoc;
        protected DocsPAWA.DocsPaWR.Utente _userHome;
        protected DocsPAWA.DocsPaWR.Ruolo _userRuolo;
        protected DocsPAWA.DocsPaWR.SchedaDocumento _schedaDocumento;

        private const string TRASMISSIONI_EFFETTUATE = "E";
        private const string TRASMISSIONI_RICEVUTE = "R";
        protected bool userAutorizedEditingACL;
        protected System.Web.UI.HtmlControls.HtmlLink idLinkCss;

        protected System.Web.UI.WebControls.HiddenField estendiVisibilita;
        protected System.Web.UI.WebControls.HiddenField abilitaModaleVis;

        /// <summary>
        /// 
        /// </summary>
        private bool _onSelectDetail = false;

        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            logger.Info("BEGIN");
            try
            {
                Utils.startUp(this);

                this._idDoc = (string)Session["idDoc"];
                this._userRuolo = UserManager.getRuolo(this);
                this._userHome = UserManager.getUtente(this);
                this._schedaDocumento = DocumentManager.getDocumentoSelezionato(this);

                Session.Remove("refreshDxPageVisualizzatore");

                if (!IsPostBack)
                {
                    _schedaDocumento = DocumentManager.getDettaglioDocumentoNoDataVista(this, _schedaDocumento.systemId, _schedaDocumento.docNumber);
                    DocumentManager.setDocumentoSelezionato(_schedaDocumento);
                }

                // Inizializzazione controllo verifica acl
                if (this._schedaDocumento != null &&
                    this._schedaDocumento.inCestino != "1" &&
                    this._schedaDocumento.systemId != null)
                {
                    this.InitializeControlAclDocumento();
                }

                //se clicco direttamente
                //su tab doc senza essere passato su profilo o protocollo
                //devo bloccare tutto se no errore
                if (this._schedaDocumento == null ||
                    this._schedaDocumento.systemId == null ||
                    this._schedaDocumento.systemId.Equals(""))
                {
                    this.btn_NuovaTrasm.Enabled = false;
                    this.btn_ModifTrasm.Enabled = false;
                    this.butt_Trasm.Enabled = false;
                    return;
                }
                else if ((this._schedaDocumento.inCestino != null &&
                         this._schedaDocumento.inCestino == "1") || (_schedaDocumento.inArchivio != null && _schedaDocumento.inArchivio == "1"))
                {
                    this.DisableButtons();
                }

                //gestione finestra attendere prego
                //this.ClientScript.RegisterStartupScript(this.GetType(), "chiusuraProgrMsgDocTrasmissioni", "hideWorkingInProgress('docTrasmissioni');", true);
                //this.ClientScript.RegisterStartupScript(this.GetType(), "chiusuraProgrMsgTrasmDatiTrasm", "hideWorkingInProgress('trasmDatiTrasm');", true);

                // gestione cessione diritti            
                this.checkIsAutorizedEditingACL();



                if (!IsPostBack)
                {
                    // Attach usercontrol per gestire la clessidra
                    this.AttatchGridPagingWaitControl();

                    this.butt_Trasm.Attributes.Add("onClick", "butt_trasm_onClick()");

                    this.Session.Remove("Modello");
                    this.FillComboModelliTrasm();
                    this.FillComboTemplate();

                    // Azione di trasmissione in sessione
                    this.DoTrasmInSessione();

                    this.FetchCurrentPage();
                }

                //Andrea
                if (Session["MessError"] != null)
                {
                    messError = Session["MessError"].ToString();
                    //Response.Write("<script language=\"javascript\">alert('Trasmissioni avvenute con successo per tutti i destinatari ad eccezione delle seguenti: \\n" + messError + "');</script>");
                    Response.Write("<script language=\"javascript\">alert('Trasmissioni con esito negativo: \\n" + messError + "\\n');</script>");
                    //this.AlertJS(Session["MessError"].ToString());
                    Session.Remove("MessError");
                }
                //End Andrea
            }
            catch (Exception es)
            {
                ErrorManager.redirect(this, es);
            }
            logger.Info("END");
        }

        /// <summary>
        /// Trasmissione da sessione
        /// </summary>
        private void DoTrasmInSessione()
        {
            if (Session["doTrasm"] != null)
            {
                DocsPaWR.Trasmissione trasmissione = (DocsPAWA.DocsPaWR.Trasmissione)Session["doTrasm"];

                if (trasmissione != null)
                {
                    if (this.ViewState["AclRevocata"] == null)
                        this.GetControlAclDocumento().AclRevocata = false;

                    // Trasmetti il documento
                    this.PerformActionTrasmettiDocumento();
                }

                Session.Remove("doTrasm");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Nessuna trasmissione in sessione!");
            }
        }

        private string GetCssAmministrazione()
        {
            string Tema = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
            {
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                DocsPAWA.UserManager userM = new UserManager();
                Tema = userM.getCssAmministrazione(idAmm);
            }
            else
            {
                if (UserManager.getInfoUtente() != null)
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    DocsPAWA.UserManager userM = new UserManager();
                    Tema = userM.getCssAmministrazione(idAmm);
                }
            }
            return Tema;
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            logger.Info("BEGIN");
            string Tema = GetCssAmministrazione();

            if (Tema != null && !Tema.Equals(""))
            {
                string[] realTema = Tema.Split('^');
                this.idLinkCss.Href = "../App_Themes/" + realTema[0] + "/" + realTema[0] + ".css";
            }
            else
                this.idLinkCss.Href = "../App_Themes/TemaRosso/TemaRosso.css";

            //abilitazione delle funzioni in base al ruolo
            UserManager.disabilitaFunzNonAutorizzate(this);

            // controllo se il documento è annullato - in tal caso disabilito tutto
            //annullamento 
            if (this._schedaDocumento.protocollo != null)
            {
                DocsPaWR.ProtocolloAnnullato protAnnull = this._schedaDocumento.protocollo.protocolloAnnullato;

                if (protAnnull != null && !string.IsNullOrEmpty(protAnnull.dataAnnullamento))
                {
                    this.DisableButtons();
                }
            }

            // Impostazione visibilità controlli
            this.SetGridVisibility();

            if (!this._onSelectDetail)
                this.ShowDetailTrasmissione();

            if (this.GetTrasmissioneSelezionata() != null
                && (this.GetTrasmissioneSelezionata().dataInvio == null ||
                this.GetTrasmissioneSelezionata().dataInvio == "")
                || ddl_tmpl.SelectedValue.Trim() != ""
                )
                this.EnableButtonTrasmissione(true);
            else
            {
                this.EnableButtonTrasmissione(false);
                this.btn_ModifTrasm.Enabled = false;
            }

            //cestino
            //CESTINO
            if (this._schedaDocumento != null
                && this._schedaDocumento.inCestino != null &&
                this._schedaDocumento.inCestino == "1")
            {
                this.btn_stampa.Enabled = false;
            }

            this.verificaHMdiritti();
            logger.Info("END");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isEnabled"></param>
        private void EnableButtonTrasmissione(bool isEnabled)
        {
            if (isEnabled)
            {
                this.butt_Trasm.Style["display"] = string.Empty;
                this.btn_trasmettiDisabled.Style["display"] = "none";
            }
            else
            {
                this.butt_Trasm.Style["display"] = "none";
                this.btn_trasmettiDisabled.Style["display"] = string.Empty;
            }
        }

        /// <summary>
        /// Impostazione visibilità controlli
        /// </summary>
        private void SetGridVisibility()
        {
            this.grdEffettuate.Visible = (this.CurrentTipoTrasmissione == "E");
            this.grdRicevute.Visible = (this.CurrentTipoTrasmissione == "R");

            // Impostazione visibilità datagrid corrente
            this.CurrentDataGrid.Visible = (this.CurrentDataGrid.Items.Count > 0);
        }

        /// <summary>
        /// Disabilitazione pulsanti
        /// </summary>
        private void DisableButtons()
        {
            this.btn_NuovaTrasm.Enabled = false;
            this.btn_ModifTrasm.Enabled = false;
            this.butt_Trasm.Enabled = false;
            this.ddl_tmpl.Enabled = false;
        }

        /// <summary>
        /// Visualizzazione dettaglio trasmissione correntemente selezionata
        /// </summary>
        private void ShowDetailTrasmissione()
        {
            string script = string.Format("<script language='javascript'>top.principale.iFrame_dx.document.location='{0}';</script>", this.CurrentDetailPageName);

            this.Response.Write(script);
        }

        /// <summary>
        /// Nome pagina per il dettaglio trasmissione
        /// </summary>
        private string CurrentDetailPageName
        {
            get
            {
                if (this.CurrentTipoTrasmissione == TRASMISSIONI_EFFETTUATE)
                    return "tabTrasmissioniEff.aspx";
                else if (this.CurrentTipoTrasmissione == TRASMISSIONI_RICEVUTE)
                    return "tabTrasmissioniRic.aspx";
                else
                    return "blank:page";
            }
        }

        #region Gestione DataGridPagingWait

        /// <summary>
        /// Attach usercontrol per gestire la clessidra e i messaggi di attesa
        /// </summary>
        private void AttatchGridPagingWaitControl()
        {
            DataGridPagingWaitControl.DataGridID = this.CurrentDataGrid.ID;
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

        #endregion

        /// <summary>
        /// Reperimento datagrid corrente in base al tipo di trasmissione selezionata
        /// </summary>
        private DataGrid CurrentDataGrid
        {
            get
            {
                if (CurrentTipoTrasmissione == TRASMISSIONI_EFFETTUATE)
                    return this.grdEffettuate;
                else
                    return this.grdRicevute;
            }
        }

        /// <summary>
        /// Reperimento tipo trasmissione correntemente selezionata: E o R
        /// </summary>
        private string CurrentTipoTrasmissione
        {
            get
            {
                return this.rbl_tipoTrasm.SelectedValue;
            }
        }

        /// <summary>
        /// Reperimento trasmissioni per il documento corrente
        /// </summary>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        private DocsPaWR.InfoTrasmissione[] GetTrasmissioniDocumento(ref DocsPaWR.SearchPagingContext pagingContext)
        {
            if (CurrentTipoTrasmissione == TRASMISSIONI_EFFETTUATE)
                return TrasmManager.GetInfoTrasmissioniEffettuate(this._schedaDocumento.systemId, "D", ref pagingContext);
            else if (CurrentTipoTrasmissione == TRASMISSIONI_RICEVUTE)
                return TrasmManager.GetInfoTrasmissioniRicevute(this._schedaDocumento.systemId, "D", ref pagingContext);
            else
                return null;
        }

        /// <summary>
        /// Caricamento dati pagina corrente
        /// </summary>
        private void FetchCurrentPage()
        {
            DataGrid grid = this.CurrentDataGrid;

            //TrasmManager.removeDocTrasmSel(this);

            // Creazione oggetto paginazione
            DocsPaWR.SearchPagingContext pagingContext = new DocsPaWR.SearchPagingContext();
            pagingContext.Page = grid.CurrentPageIndex + 1;
            pagingContext.PageSize = grid.PageSize;

            // Reperimento trasmissioni per il documento corrente
            DocsPaWR.InfoTrasmissione[] infoTrasmList = this.GetTrasmissioniDocumento(ref pagingContext);

            //gestione in caso di trasmissione per delega
            foreach (DocsPaWR.InfoTrasmissione infoTrasm in infoTrasmList)
            {
                if (!string.IsNullOrEmpty(infoTrasm.UtenteDelegato))

                    infoTrasm.Utente = infoTrasm.UtenteDelegato + "<br>Delegato da " + infoTrasm.Utente;
            }

            grid.VirtualItemCount = pagingContext.RecordCount;
            grid.CurrentPageIndex = pagingContext.Page - 1;
            grid.DataSource = infoTrasmList;
            grid.DataBind();



            this.SetMsgTrasmissioniTrovate(pagingContext.RecordCount);

            this.PerformActionSelectFirstTrasmissione();
        }

        /// <summary>
        /// Handler evento di cambio pagina per il datagrid corrente
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnGridPageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            ((DataGrid)source).CurrentPageIndex = e.NewPageIndex;
            this.FetchCurrentPage();
        }

        /// <summary>
        /// Handler evento prerender datagrid corrente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGridPreRender(object sender, System.EventArgs e)
        {
            foreach (DataGridItem item in ((DataGrid)sender).Items)
            {
                ImageButton imgDettaglio = (ImageButton)item.FindControl("imgDettaglio");

                if (imgDettaglio != null)
                    imgDettaglio.Attributes.Add("onClick", "ShowWaitCursor()");
            }
        }

        private void SetMsgTrasmissioniTrovate(int totalRecordCount)
        {
            if (totalRecordCount > 1)
                this.lbl_message.Text = "Elenco trasmissioni - Trovati " + totalRecordCount + " elementi.";
            else if (totalRecordCount == 1)
                this.lbl_message.Text = "Elenco trasmissioni - Trovato 1 elemento.";
            else
                this.lbl_message.Text = "Trasmissioni non trovate";
        }

        /// <summary>
        /// Azione di trasmissione del documento corrente
        /// </summary>
        private void PerformActionTrasmettiDocumento()
        {
            //if (!this.GetControlAclDocumento().AclRevocata)
            //{
            //    if (((this._schedaDocumento.privato == "1") || (this._schedaDocumento.personale == "1")) && (Session["doTrasm"] == null))
            //    {
            //        string messaggio;
            //        if (this._schedaDocumento.privato == "1")
            //        {
            //            messaggio = InitMessageXml.getInstance().getMessage("trasmDocPrivato");
            //        }
            //        else
            //            messaggio = InitMessageXml.getInstance().getMessage("trasmDocPersonale");
            //        msg_Trasmetti.Confirm(messaggio);
            //    }
            //    else
            //    {
            int indexSel = ddl_tmpl.Items.IndexOf(ddl_tmpl.Items.FindByValue(separatore));

            if (this.ddl_tmpl.SelectedIndex > 0 && ddl_tmpl.SelectedIndex < indexSel)
            {
                //MODELLI TRASMISSIONE NUOVI
                if (Session["Modello"] != null)
                {
                    DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

                    if (!this.notificheUtImpostate(modello))
                    {
                        this.ddl_tmpl.SelectedIndex = -1;
                        Response.Write("<script language='javascript'>window.alert('Attenzione,\\nil modello selezionato presenta la seguente anomalia:\\n\\nDESTINATARI PRIVI DI NOTIFICA IMPOSTATA.\\n\\nImpostare le notifiche utenti per questo modello nella sezione Gestione > Modelli tras.');</script>");
                        return;
                    }

                    Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();
                    if (modello != null)
                        trasmissione.NO_NOTIFY = modello.NO_NOTIFY;
                    //Parametri della trasmissione
                    trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

                    if (modello.CHA_TIPO_OGGETTO == "D")
                    {
                        trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
                        trasmissione.infoDocumento = DocumentManager.getInfoDocumento(this._schedaDocumento);
                    }
                    else
                    {
                        trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
                        trasmissione.infoFascicolo = FascicoliManager.getInfoFascicolo(this);
                    }
                    trasmissione.utente = UserManager.getUtente(this);
                    trasmissione.ruolo = UserManager.getRuolo(this);

                    // gestione della cessione diritti
                    if (modello.CEDE_DIRITTI != null && modello.CEDE_DIRITTI.Equals("1"))
                    {
                        DocsPaWR.CessioneDocumento objCessione = new DocsPAWA.DocsPaWR.CessioneDocumento();

                        objCessione.docCeduto = true;

                        //*******************************************************************************************
                        // MODIFICA IACOZZILLI GIORDANO 30/07/2012
                        // Modifica inerente la cessione dei diritti di un doc da parte di un utente non proprietario ma 
                        // nel ruolo del proprietario, in quel caso non posso valorizzare l'IDPEOPLE  con il corrente perchè
                        // il proprietario può essere un altro utente del mio ruolo, quindi andrei a generare un errore nella security,
                        // devo quindi controllare che nell'idpeople venga inserito l'id corretto del proprietario.
                        string valoreChiaveCediDiritti = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_CEDI_DIRITTI_IN_RUOLO");
                        if (!string.IsNullOrEmpty(valoreChiaveCediDiritti) && valoreChiaveCediDiritti.Equals("1"))
                        {
                            //Devo istanziare una classe utente.
                            string idProprietario = string.Empty;
                            idProprietario = GetAnagUtenteProprietario();
                            Utente _utproprietario = UserManager.GetUtenteByIdPeople(idProprietario);

                            objCessione.idPeople = idProprietario;
                            objCessione.idRuolo = UserManager.getInfoUtente(this).idGruppo;
                            objCessione.userId = _utproprietario.cognome + " " + _utproprietario.nome;


                            if (objCessione.idPeople == null || objCessione.idPeople == "")
                                objCessione.idPeople = idProprietario;

                            if (objCessione.idRuolo == null || objCessione.idRuolo == "")
                                objCessione.idRuolo = UserManager.getInfoUtente(this).idGruppo;

                            if (objCessione.userId == null || objCessione.userId == "")
                                objCessione.userId = _utproprietario.cognome + " " + _utproprietario.nome;
                        }
                        else
                        {
                            //OLD CODE:
                            objCessione.idPeople = UserManager.getInfoUtente(this).idPeople;
                            objCessione.idRuolo = UserManager.getInfoUtente(this).idGruppo;
                            objCessione.userId = UserManager.getInfoUtente(this).userId;
                        }
                        //*******************************************************************************************
                        // FINE MODIFICA
                        //********************************************************************************************
                        if (modello.ID_PEOPLE_NEW_OWNER != null && modello.ID_PEOPLE_NEW_OWNER != "")
                            objCessione.idPeopleNewPropr = modello.ID_PEOPLE_NEW_OWNER;
                        if (modello.ID_GROUP_NEW_OWNER != null && modello.ID_GROUP_NEW_OWNER != "")
                            objCessione.idRuoloNewPropr = modello.ID_GROUP_NEW_OWNER;

                        trasmissione.cessione = objCessione;
                        //
                        // Mev Cessione Diritti - mantieni scrittura
                        if (!string.IsNullOrEmpty(modello.MANTIENI_SCRITTURA) && modello.MANTIENI_SCRITTURA == "1")
                            trasmissione.mantieniScrittura = true;
                        else
                            trasmissione.mantieniScrittura = false;
                        // End Mev
                        //
                        
                        // Se per il modello creato è prevista l'opzione di mantenimento dei diritti di lettura
                        if (!string.IsNullOrEmpty(modello.MANTIENI_LETTURA) && modello.MANTIENI_LETTURA == "1")
                            trasmissione.mantieniLettura = true;
                        else
                            trasmissione.mantieniLettura = false;
                    }

                    bool eredita = false;
                    //Parametri delle trasmissioni singole

                    for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                    {
                        DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                        ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                        for (int j = 0; j < destinatari.Count; j++)
                        {
                            DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                            DocsPaWR.Corrispondente corr = new Corrispondente();
                            if (mittDest.CHA_TIPO_MITT_DEST == "D")
                            {
                                corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                            }
                            else
                            {
                                corr = TrasmManager.getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, _schedaDocumento, null, this);
                            }

                            if (corr != null)
                            {
                                DocsPaWR.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());

                                /*
                                 * Andrea try - catch
                                 */

                                try
                                {
                                    trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, mittDest.NASCONDI_VERSIONI_PRECEDENTI);
                                }
                                catch (ExceptionTrasmissioni e)
                                {
                                    //Aggiungo l'errore alla lista
                                    listaExceptionTrasmissioni.Add(e.Messaggio);

                                    //foreach (string s in listaExceptionTrasmissioni) 
                                    //{
                                    //    //messError = messError + s + "\r\n";
                                    //    messError = messError + s + "|";
                                    //}

                                    //if (messError!="") 
                                    //{
                                    //    Session.Add("MessError", messError);

                                    //    //Response.Write("<script language='javascript'>window.alert(" + messError + ");</script>");
                                    //}
                                    //if (messError != "")
                                    //{
                                    //    // Alert
                                    //    //ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "<script>alert(" + messError + ");</script>");
                                    //    //Response.Write("<script language='javascript'>window.alert(" + messError + ");</script>");
                                    //    Session.Add("MessError", messError);
                                    //}

                                }

                                /*
                                 *End Andrea
                                 */

                                if (ragione.eredita == "1")
                                    eredita = true;
                            }

                        }
                    }
                    //Andrea
                    foreach (string s in listaExceptionTrasmissioni)
                    {
                        //messError = messError + s + "\r\n";
                        messError = messError + s + "\\n";
                    }

                    if (messError != "")
                    {
                        Session.Add("MessError", messError);

                    }
                    //End Andrea

                    DocsPaWR.Trasmissione t_rs = null;
                    if (trasmissione.trasmissioniSingole != null && trasmissione.trasmissioniSingole.Length > 0)
                    {
                        trasmissione = this.impostaNotificheUtentiDaModello(trasmissione);

                        if (estendiVisibilita.Value == "false")
                        {
                            TrasmissioneSingola[] appoTrasmSingole = new TrasmissioneSingola[trasmissione.trasmissioniSingole.Length];
                            for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                            {
                                TrasmissioneSingola trasmSing = new TrasmissioneSingola();
                                trasmSing = trasmissione.trasmissioniSingole[i];
                                trasmSing.ragione.eredita = "0";
                                appoTrasmSingole[i] = trasmSing;
                            }
                            trasmissione.trasmissioniSingole = appoTrasmSingole;
                        }

                        DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
                        if (infoUtente.delegato != null)
                            trasmissione.delegato = ((DocsPAWA.DocsPaWR.InfoUtente)(infoUtente.delegato)).idPeople;

                        //Nuovo metodo saveExecuteTrasm
                        t_rs = TrasmManager.saveExecuteTrasm(this, trasmissione, infoUtente);
                        //trasmissione = TrasmManager.saveTrasm(this, trasmissione);
                        //t_rs = TrasmManager.executeTrasm(this, trasmissione);
                    }
                    if (t_rs != null && t_rs.ErrorSendingEmails)
                    {
                        Response.Write("<script language='javascript'>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");
                    }
                    if (t_rs != null && !t_rs.dirittiCeduti && modello.CEDE_DIRITTI.Equals("1"))
                    {
                        Response.Write("<script language='javascript'>window.alert('Non è stato possibile cedere i diritti con questo tipo di trasmissione.');</script>");
                    }
                    Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=trasmissioni'; </script>");
                    //Salvataggio del system_id della trasm per il cambio di stato automatico
                    if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                    {
                        DocsPaWR.Stato stato = wws.getStatoDoc(this._schedaDocumento.docNumber);
                        bool trasmWF = false;
                        if (trasmissione != null && trasmissione.trasmissioniSingole != null &&
                            trasmissione.trasmissioniSingole.Length > 0)
                        {
                            for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                            {
                                DocsPaWR.TrasmissioneSingola trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                                if (trasmSing.ragione.tipo == "W")
                                    trasmWF = true;
                            }
                        }
                        if (stato != null && trasmWF)
                            wws.salvaStoricoTrasmDiagrammi(trasmissione.systemId, this._schedaDocumento.docNumber, Convert.ToString(stato.SYSTEM_ID));

                    }

                    return;
                }
                //FINE MODELLI TRASMISSIONE NUOVI
            }

            try
            {
                DocsPaWR.Trasmissione trasmEff;
                if (this.flag_template.Value.Equals("S") && this.ddl_tmpl.SelectedIndex > 0)
                {
                    trasmEff = creaTrasmissione();
                    if (trasmEff == null)
                        Response.Write("<script language='javascript'>alert('Si è verificato un errore nella creazione della trasmissione da template');</script>");
                }
                else
                {
                    trasmEff = TrasmManager.getGestioneTrasmissione(this);
                    if (trasmEff != null && trasmEff.utente != null && string.IsNullOrEmpty(trasmEff.utente.idAmministrazione))
                        trasmEff.utente.idAmministrazione = UserManager.getInfoUtente().idAmministrazione;
                }
                //Nuova gestione save&executeTrasm in trasmDatiTrasm_dx
                if (trasmEff != null)
                {

                    if (estendiVisibilita.Value == "false")
                    {
                        TrasmissioneSingola[] appoTrasmSingole = new TrasmissioneSingola[trasmEff.trasmissioniSingole.Length];
                        for (int i = 0; i < trasmEff.trasmissioniSingole.Length; i++)
                        {
                            TrasmissioneSingola trasmSing = new TrasmissioneSingola();
                            trasmSing = trasmEff.trasmissioniSingole[i];
                            trasmSing.ragione.eredita = "0";
                            appoTrasmSingole[i] = trasmSing;
                        }
                        trasmEff.trasmissioniSingole = appoTrasmSingole;
                    }

                    //DocsPaWR.Trasmissione t_rs = TrasmManager.executeTrasm(this, trasmEff);
                    if (string.IsNullOrEmpty(trasmEff.dataInvio))
                        trasmEff = TrasmManager.executeTrasm(this, trasmEff);


                    if (trasmEff != null && trasmEff.ErrorSendingEmails)
                        Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");
                }


                Response.Write("<script language='javascript'>parent.IframeTabs.document.location='docTrasmissioni.aspx';</script>");
                //resetto il template della trasmissione
                flag_template.Value = "N";
                this.ddl_tmpl.SelectedIndex = -1;

                //Salvataggio del system_id della trasm per il cambio di stato automatico
                if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                {
                    DocsPaWR.Stato stato = wws.getStatoDoc(this._schedaDocumento.docNumber);
                    bool trasmWF = false;
                    for (int i = 0; i < trasmEff.trasmissioniSingole.Length; i++)
                    {
                        DocsPaWR.TrasmissioneSingola trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmEff.trasmissioniSingole[i];
                        if (trasmSing.ragione.tipo == "W")
                            trasmWF = true;
                    }
                    if (stato != null && trasmWF)
                        wws.salvaStoricoTrasmDiagrammi(trasmEff.systemId, this._schedaDocumento.docNumber, Convert.ToString(stato.SYSTEM_ID));
                }
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                Session.Remove("doTrasm");
                ErrorManager.redirect(this, es);
            }
            //    }
            //}
        }

        /// <summary>
        /// Iacozzilli: Faccio la Get dell'idProprietario del doc!
        /// </summary>
        /// <returns></returns>
        private string GetAnagUtenteProprietario()
        {
            DocumentoDiritto[] listaVisibilita = null;
            DocsPaWR.SchedaDocumento sd = DocumentManager.getDocumentoSelezionato(this);
            string idProprietario = string.Empty;
            listaVisibilita = DocumentManager.getListaVisibilitaSemplificata(this, sd.docNumber, true);
            if (listaVisibilita != null && listaVisibilita.Length > 0)
            {
                for (int i = 0; i < listaVisibilita.Length; i++)
                {
                    if (listaVisibilita[i].accessRights == 0)
                    {
                        return idProprietario = listaVisibilita[i].personorgroup;
                    }
                }
            }

            return "";
        }

        private void butt_Trasm_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            logger.Info("BEGIN");
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                if ((this._schedaDocumento.personale == "1") && (Session["doTrasm"] == null))
                {
                    string messaggio = string.Empty;
                    //if (this._schedaDocumento.privato == "1")
                    //{
                    //    messaggio = InitMessageXml.getInstance().getMessage("trasmDocPrivato");
                    //}
                    //else
                    messaggio = InitMessageXml.getInstance().getMessage("trasmDocPersonale");
                    msg_Trasmetti.Confirm(messaggio);
                }
                else
                {

                    //Modifica Iacozzilli Giordano 30/07/2012
                    //Ora posso cedere i diritti sul doc anche se non ne sono il proprietario.
                    //
                    //OLD CODE:
                    // gestione cessione diritti - inizializzazione dati oggetto
                    //this.PerformActionTrasmettiDocumento();

                    //
                    //NEW CODE:
                    // verifica se è proprietario come UTENTE...
                    //Uso una chiave su DB per attivare la modifica da db.
                    try
                    {
                        string valoreChiaveCediDiritti = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_CEDI_DIRITTI_IN_RUOLO");
                        if (!string.IsNullOrEmpty(valoreChiaveCediDiritti) && valoreChiaveCediDiritti.Equals("1"))
                        {
                            string accessRights = string.Empty;
                            string idGruppoTrasm = string.Empty;
                            string tipoDiritto = string.Empty;
                            string IDObject = string.Empty;

                            bool isPersonOwner = false;
                            DocsPaWR.SchedaDocumento sd = DocumentManager.getDocumentoSelezionato(this);
                            IDObject = sd.systemId;
                            DocsPaWebService ws = new DocsPaWebService();
                            ws.SelectSecurity(IDObject, UserManager.getInfoUtente(this).idPeople, "= 0", out accessRights, out idGruppoTrasm, out tipoDiritto);
                            isPersonOwner = (accessRights.Equals("0"));
                            if (!isPersonOwner)
                            {
                                string idProprietario = GetAnagUtenteProprietario();
                                Utente _utproprietario = UserManager.GetUtenteByIdPeople(idProprietario);
                                msg_Trasmetti.Confirm("Sei sicuro di voler cedere i diritti del documento\\n di proprietà dell’utente : " + _utproprietario.cognome + " " + _utproprietario.nome + " ?");
                            }
                            else
                            {
                                this.PerformActionTrasmettiDocumento();
                            }
                        }
                        else
                        {
                            this.PerformActionTrasmettiDocumento();
                        }
                    }
                    catch (Exception ex)
                    {
                        //Messaggio di ritorno.
                    }
                }
            }
            logger.Info("END");
        }

        private void msg_Trasmetti_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                this.PerformActionTrasmettiDocumento();

                //riapre la finestra attendere prego
                //ClientScript.RegisterStartupScript(this.GetType(), "refreshProgressMsgDocTrasmissioni", "showWorkingInProgress(''); ", true);


                //Trasmissione trasmissione = this.GetTrasmissioneSelezionata();
                //if (trasmissione != null)
                //{
                //   DocsPaWR.Trasmissione t_rs = null;
                //   //trasmissione = TrasmManager.saveTrasm(this, trasmissione);
                //   t_rs = TrasmManager.executeTrasm(this, trasmissione);
                //   if (t_rs != null && t_rs.ErrorSendingEmails)
                //   {
                //      Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");
                //      Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=trasmissioni'; </script>");
                //   }

                //   Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=trasmissioni'; </script>");

                //   //Salvataggio del system_id della trasm per il cambio di stato automatico
                //   if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                //   {
                //       DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(this._schedaDocumento.docNumber,this);
                //      bool trasmWF = false;
                //      if (trasmissione != null && trasmissione.trasmissioniSingole != null)
                //         for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                //         {
                //            DocsPaWR.TrasmissioneSingola trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                //            if (trasmSing.ragione.tipo == "W")
                //               trasmWF = true;
                //         }
                //      if (stato != null && trasmWF)
                //          DocsPAWA.DiagrammiManager.salvaStoricoTrasmDiagrammi(trasmissione.systemId, this._schedaDocumento.docNumber, Convert.ToString(stato.SYSTEM_ID),this);
                //   }
                //   return;
                //}

                //DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
                //Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();


                ////Parametri della trasmissione
                //trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

                //if (modello.CHA_TIPO_OGGETTO == "D")
                //{
                //    trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
                //    trasmissione.infoDocumento = DocumentManager.getInfoDocumento(this._schedaDocumento);
                //}
                //else
                //{
                //    trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
                //    trasmissione.infoFascicolo = FascicoliManager.getInfoFascicolo(this);
                //}
                //trasmissione.utente = UserManager.getUtente(this);
                //trasmissione.ruolo = UserManager.getRuolo(this);

                ////Parametri delle trasmissioni singole
                //for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                //{
                //    DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                //    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                //    for (int j = 0; j < destinatari.Count; j++)
                //    {
                //        DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                //        //old: ritoranva anche i corr diasbilitati DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);

                //        DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                //        if (corr != null)
                //        {   //il corr è null se non esiste o se è stato disabiltato.    
                //            DocsPaWR.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                //            trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA);

                //        }
                //    }
                //}
                //DocsPaWR.Trasmissione t_rs = null;
                //trasmissione = TrasmManager.saveTrasm(this, trasmissione);
                //t_rs = TrasmManager.executeTrasm(this, trasmissione);
                //if (t_rs != null && t_rs.ErrorSendingEmails)
                //    Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");


                //Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=trasmissioni'; </script>");

                ////Salvataggio del system_id della trasm per il cambio di stato automatico
                //if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                //{
                //    DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(this._schedaDocumento.docNumber,this);
                //    bool trasmWF = false;
                //    if (trasmissione != null && trasmissione.trasmissioniSingole != null)
                //        for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                //        {
                //            DocsPaWR.TrasmissioneSingola trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                //            if (trasmSing.ragione.tipo == "W")
                //                trasmWF = true;
                //        }
                //    if (stato != null && trasmWF)
                //        DocsPAWA.DiagrammiManager.salvaStoricoTrasmDiagrammi(trasmissione.systemId, this._schedaDocumento.docNumber, Convert.ToString(stato.SYSTEM_ID),this);
                //}
                //return;
            }
            //Modifica Iacozzilli Giordano 12/10/2012
            //Ora posso cedere i diritti sul doc anche se non ne sono il proprietario.
            //Con questo codice forzo il postback e tolgo la splash: Attendere prego.
            else
                Response.Write("<script language='javascript'>top.principale.document.location='../trasmissione/gestioneTrasm.aspx?azione=Modifica';</script>");


            //if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Cancel)
            //{
            //    //chiude la finestra attendere prego
            //    ClientScript.RegisterStartupScript(this.GetType(), "chiusuraProgressMsgDocTrasmissioni", "hideWorkingInProgress('docTrasmissioni');", true);
            //}
        }

        private void btn_ModifTrasm_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                Session["OggettoDellaTrasm"] = "DOC";
                Response.Write("<script language='javascript'>top.principale.document.location='../trasmissione/gestioneTrasm.aspx?azione=Modifica';</script>");
            }
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
        private void InitializeComponent()
        {
            this.grdEffettuate.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.OnGridPageIndexChanged);
            this.grdEffettuate.PreRender += new EventHandler(this.OnGridPreRender);
            this.grdEffettuate.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
            this.grdRicevute.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.OnGridPageIndexChanged);
            this.grdRicevute.PreRender += new EventHandler(this.OnGridPreRender);
            this.grdRicevute.SelectedIndexChanged += new System.EventHandler(this.DataGrid2_SelectedIndexChanged);
            this.ddl_tmpl.SelectedIndexChanged += new System.EventHandler(this.ddl_tmpl_SelectedIndexChanged);
            this.btn_NuovaTrasm.Click += new System.Web.UI.ImageClickEventHandler(this.btn_NuovaTrasm_Click);
            this.btn_ModifTrasm.Click += new System.Web.UI.ImageClickEventHandler(this.btn_ModifTrasm_Click);
            this.butt_Trasm.Click += new System.Web.UI.ImageClickEventHandler(this.butt_Trasm_Click);
            this.btn_stampa.Click += new System.Web.UI.ImageClickEventHandler(this.btn_stampa_Click);
            this.flag_template.ServerChange += new System.EventHandler(this.flag_template_ServerChange);
            this.msg_Trasmetti.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_Trasmetti_GetMessageBoxResponse);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);

        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rbl_tipoTrasm_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.PerformChangeTipoRicercaTrasm();
            ViewState.Add("nuovaPag", true);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void PerformChangeTipoRicercaTrasm()
        {
            this.grdEffettuate.CurrentPageIndex = 0;
            this.grdRicevute.CurrentPageIndex = 0;

            // Attach usercontrol per gestire la clessidra
            this.AttatchGridPagingWaitControl();

            // Caricamento pagina corrente
            this.FetchCurrentPage();
        }

        /// <summary>
        /// Reperimento id trasmissione per l'elemento correntemente selezionato in griglia
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <returns></returns>
        protected string GetSelectedIdTrasm(DataGrid dataGrid)
        {
            string idTrasmissione = string.Empty;

            if (dataGrid.SelectedItem != null)
            {
                Label lblIdTrasm = (Label)dataGrid.SelectedItem.FindControl("lblIdTrasm");

                if (lblIdTrasm != null)
                    idTrasmissione = lblIdTrasm.Text;
            }

            return idTrasmissione;
        }

        /// <summary>
        /// Reperimento trasmissione corrente
        /// </summary>
        /// <param name="idTrasmissione"></param>
        /// <returns></returns>
        private DocsPaWR.Trasmissione GetTrasmissione(string idTrasmissione)
        {
            DocsPaWR.Trasmissione retValue = null;

            if (!string.IsNullOrEmpty(idTrasmissione))
            {
                // Reperimento oggetto trasmissione
                DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPaWR.TrasmissioneOggettoTrasm();
                oggettoTrasm.infoDocumento = DocumentManager.getInfoDocumento(this._schedaDocumento);

                System.Collections.Generic.List<FiltroRicerca> filters = new System.Collections.Generic.List<FiltroRicerca>();
                FiltroRicerca item = new FiltroRicerca();
                item.argomento = "ID_TRASMISSIONE";
                item.valore = idTrasmissione;
                filters.Add(item);

                // Utilizzo del metodo di ricerca trasmissioni fornendo i filtri di ricerca
                int totalPageNumber;
                int recordCount;

                DocsPaWR.Trasmissione[] trasmissioni = null;

                if (this.CurrentTipoTrasmissione == TRASMISSIONI_EFFETTUATE)
                {
                    trasmissioni = TrasmManager.getQueryEffettuateDocumentoPaging(
                            this, oggettoTrasm,
                            this._userHome, this._userRuolo, filters.ToArray(),
                            1, out totalPageNumber, out recordCount);
                }
                else
                {
                    item = new FiltroRicerca();
                    item.argomento = "TAB_TRASMISSIONI";
                    item.valore = "TRUE";
                    filters.Add(item);

                    trasmissioni = TrasmManager.getQueryRicevutePaging(
                            this, oggettoTrasm,
                            this._userHome, this._userRuolo, filters.ToArray(),
                            1, out totalPageNumber, out recordCount);
                }

                if (trasmissioni.Length == 1)
                {
                    // Reperimento prima trasmissione estratta
                    retValue = trasmissioni[0];
                }
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento trasmissione selezionata in griglia
        /// </summary>
        /// <returns></returns>
        protected DocsPaWR.Trasmissione GetTrasmissioneSelezionata()
        {

            // Reperimento id trasmissione selezionata
            string idTrasmissione = this.GetSelectedIdTrasm(this.CurrentDataGrid);

            DocsPAWA.DocsPaWR.Trasmissione trasmissioneSel = TrasmManager.getDocTrasmSel(this);
            if (trasmissioneSel == null || trasmissioneSel.systemId != idTrasmissione || Convert.ToBoolean(ViewState["nuovaPag"]))
            {
                // Reperimento oggetto trasmissione corrente
                if (!string.IsNullOrEmpty(idTrasmissione))
                {
                    trasmissioneSel = this.GetTrasmissione(idTrasmissione);
                    TrasmManager.setDocTrasmSel(this, trasmissioneSel);
                    return trasmissioneSel;

                }
                else
                {
                    TrasmManager.removeDocTrasmSel(this);
                    return null;
                }
                ViewState.Remove("nuovaPag");
            }
            else
            {
                return trasmissioneSel;
            }
        }

        /// <summary>
        /// Azione di selezione prima trasmissione disponibile
        /// </summary>
        protected void PerformActionSelectFirstTrasmissione()
        {
            DataGrid grd = this.CurrentDataGrid;

            if (grd.Items.Count > 0)
            {
                grd.SelectedIndex = 0;

                if (this.CurrentTipoTrasmissione == TRASMISSIONI_EFFETTUATE)
                    this.PerformSelectionTrasmissioneEffettuata();
                else
                    this.PerformSelectionTrasmissioneRicevuta();

                this.btn_stampa.Enabled = true;
            }
            else
                this.btn_stampa.Enabled = false;
        }

        /// <summary>
        /// Azione di selezione trasmissione effettuata
        /// </summary>
        protected void PerformSelectionTrasmissioneEffettuata()
        {
            // Reperimento oggetto trasmissione corrente
            DocsPaWR.Trasmissione trasmissione = this.GetTrasmissioneSelezionata();

            if (trasmissione != null)
            {
                this._onSelectDetail = true;

                bool ruoloProprTrasmSalvata = false;
                if (trasmissione.ruolo != null && trasmissione.ruolo.idGruppo != null)
                    ruoloProprTrasmSalvata = (this._userRuolo.idGruppo.Equals(trasmissione.ruolo.idGruppo));

                this.btn_ModifTrasm.Enabled = (string.IsNullOrEmpty(trasmissione.dataInvio) && ruoloProprTrasmSalvata);
                this.butt_Trasm.Enabled = this.btn_ModifTrasm.Enabled;

                // Impostazione della trasmissione in sessione
                TrasmManager.setDocTrasmSel(this, trasmissione);
                TrasmManager.setGestioneTrasmissione(this, trasmissione);

                // Visualizzazione dettaglio trasmissione
                this.ShowDetailTrasmissione();

                //rimuovo la possibile trasmissione da template
                this.ddl_tmpl.SelectedIndex = -1;
                this.flag_template.Value = "N";
            }
        }

        /// <summary>
        /// Azione di selezione trasmissione ricevuta
        /// </summary>
        protected void PerformSelectionTrasmissioneRicevuta()
        {
            // Reperimento oggetto trasmissione corrente
            DocsPaWR.Trasmissione trasmissione = this.GetTrasmissioneSelezionata();

            if (trasmissione != null)
            {
                this._onSelectDetail = true;

                TrasmManager.setDocTrasmSel(this, trasmissione);

                // Visualizzazione dettaglio trasmissione
                this.ShowDetailTrasmissione();
            }

            //rimuovo la possibile trasmissione da template
            this.ddl_tmpl.SelectedIndex = -1;
            this.flag_template.Value = "N";

            this.btn_ModifTrasm.Enabled = false;
            this.butt_Trasm.Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.PerformSelectionTrasmissioneEffettuata();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid2_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.PerformSelectionTrasmissioneRicevuta();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_NuovaTrasm_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                Session.Remove("Modello");
                TrasmManager.removeGestioneTrasmissione(this);
                Session["OggettoDellaTrasm"] = "DOC";
                Response.Write("<script language='javascript'>top.principale.document.location='../trasmissione/gestioneTrasm.aspx?azione=Nuova';</script>");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_NuovoDaTempl_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void FillComboTemplate()
        {
            Session.Remove("doc_protocollo.tx_tmpl");
            DocsPaWR.TemplateTrasmissione[] listaTmp;
            if (Session["doc_protocollo.tx_tmpl"] != null)
            {
                listaTmp = (DocsPAWA.DocsPaWR.TemplateTrasmissione[])Session["doc_protocollo.tx_tmpl"];
            }
            else
            {
                listaTmp = TrasmManager.getListaTemplate(this, UserManager.getUtente(this), UserManager.getRuolo(this), "D");
                Session["doc_protocollo.tx_tmpl"] = listaTmp;

            }

            if (listaTmp != null && listaTmp.Length > 0)
            {
                if (ddl_tmpl.Items.Count == 0)
                    ddl_tmpl.Items.Add(" "); // valore vuoto;

                for (int i = 0; i < listaTmp.Length; i++)
                {
                    System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                    li.Text = listaTmp[i].descrizione;
                    li.Value = listaTmp[i].systemId;
                    ddl_tmpl.Items.Add(li);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddl_tmpl_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!ddl_tmpl.SelectedIndex.Equals(0))
            {
                this.flag_template.Value = "S";
                this.butt_Trasm.Enabled = true;
                //elimino la selezione della riga del datagrid
                this.grdEffettuate.SelectedIndex = -1;
                this.grdRicevute.SelectedIndex = -1;
                TrasmManager.removeDocTrasmSel(this);
                Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='tabTrasmissioniRic.aspx';</script>");
            }
            else
            {
                this.flag_template.Value = "N";
                this.butt_Trasm.Enabled = false;
            }

            //MODELLI TRASMISSIONE NUOVI
            if (ddl_tmpl.SelectedItem.Text == separatore)
            {
                this.flag_template.Value = "N";
                this.butt_Trasm.Enabled = false;
                Session.Remove("Modello");
                return;
            }
            DocsPaWR.ModelloTrasmissione modello = new DocsPAWA.DocsPaWR.ModelloTrasmissione();
            modello = wws.getModelloByID(UserManager.getRegistroSelezionato(this).idAmministrazione, ddl_tmpl.SelectedValue);
            if (modello != null && modello.SYSTEM_ID != 0)
            {
                Session.Add("Modello", modello);

                if (this._schedaDocumento.privato == "1")
                {
                    if (string.IsNullOrEmpty(abilitaModaleVis.Value))
                    {
                        if (wws.ereditaVisibilita("null", modello.SYSTEM_ID.ToString()))
                        {
                            abilitaModaleVis.Value = "true";
                            //ClientScript.RegisterStartupScript(this.GetType(), "openAvvisoVisibilita", "AvvisoVisibilita();", true);
                        }
                    }
                }

            }
            else
            {
                Session.Remove("Modello");
            }
            //FINE MODELLI TRASMISSIONE NUOVI
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Trasmissione creaTrasmissione()
        {
            logger.Info("BEGIN");
            //crea trasmissione da template
            DocsPaWR.TemplateTrasmissione[] listaTmp;
            DocsPaWR.Trasmissione trasmissione = null;
            try
            {
                DocsPaWR.TemplateTrasmissione template = null;
                DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);
                if (this._schedaDocumento == null)
                    this._schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
                DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(this._schedaDocumento);
                listaTmp = (DocsPAWA.DocsPaWR.TemplateTrasmissione[])(Session["doc_protocollo.tx_tmpl"]);

                //Adesso con i modelli di trasmissione nuovi il calcolo del template vecchio selezionato
                //é necessario farlo cosi'. In ogni caso funziona sia se ci sono sia se non ci sono modelli nuovi.
                int numberOldTemplate = ddl_tmpl.Items.Count - listaTmp.Length;
                if (listaTmp != null && listaTmp.Length > 0)
                    template = (DocsPAWA.DocsPaWR.TemplateTrasmissione)listaTmp[ddl_tmpl.SelectedIndex - numberOldTemplate];

                if (template != null)
                    trasmissione = TrasmManager.addTrasmDaTemplate(this, infoDoc, template, infoUtente);
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
            logger.Info("END");
            return trasmissione;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_salva_trasm_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                if (!ddl_tmpl.SelectedIndex.Equals(0))
                {
                    //crea trasmissione da template
                    DocsPaWR.Trasmissione trasmissione;
                    trasmissione = creaTrasmissione();
                    if (trasmissione == null)
                        Response.Write("<script language='javascript'>alert('Si è verificato un errore nella creazione della trasmissione da template');</script>");
                    else
                        Response.Write("<script language='javascript'>parent.IframeTabs.document.location='docTrasmissioni.aspx';</script>");

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_stampa_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                try
                {
                    DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(this._schedaDocumento);
                    DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm();
                    oggettoTrasm.infoDocumento = infoDoc;
                    DocsPaWR.FileDocumento fileRep = TrasmManager.getReportTrasm(this, oggettoTrasm);
                    if (fileRep == null)
                        return;
                    FileManager.setSelectedFileReport(this, fileRep, "../popup");
                }
                catch (Exception ex)
                {
                    ErrorManager.redirectToErrorPage(this, ex);
                }
            }
        }

        private void flag_template_ServerChange(object sender, System.EventArgs e)
        {

        }

        #region ModelliTrasmissioni

        private DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Corrispondente corr, DocsPAWA.DocsPaWR.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, bool nascondiVersioniPrecedenti)
        {
            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                {
                    DocsPaWR.TrasmissioneSingola ts = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                    if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                    {
                        if (ts.daEliminare)
                        {
                            ((DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                            return trasmissione;
                        }
                        else
                            return trasmissione;
                    }
                }
            }

            // Aggiungo la trasmissione singola
            DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;

            if (trasmissione.tipoOggetto == TrasmissioneTipoOggetto.DOCUMENTO)
                trasmissioneSingola.hideDocumentPreviousVersions = nascondiVersioniPrecedenti;
            else
                trasmissioneSingola.hideDocumentPreviousVersions = false;

            //Imposto la data di scadenza
            if (scadenza > 0)
            {
                //string dataScadenza = "";
                System.DateTime data = System.DateTime.Now.AddDays(scadenza);
                //dataScadenza = data.Day + "/" + data.Month + "/" + data.Year;
                trasmissioneSingola.dataScadenza = DocsPAWA.Utils.formatDataDocsPa(data);
            }

            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPAWA.DocsPaWR.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
                DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr);
                /*
                 * Andrea
                 */
                if (listaUtenti == null || listaUtenti.Length == 0)
                {
                    trasmissioneSingola = null;
                    throw new ExceptionTrasmissioni("Non è presente alcun utente per la Trasmissione al ruolo: "
                                                    + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + ".");
                    //DocsPAWA.utils.AlertPostLoad.GenericMessage(this, "nessun utente per il ruolo.");
                }
                //End Andrea
                else
                {
                    //ciclo per utenti se dest è gruppo o ruolo
                    for (int i = 0; i < listaUtenti.Length; i++)
                    {
                        DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                        trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)listaUtenti[i];
                        trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    }
                }
            }

            if (corr is DocsPAWA.DocsPaWR.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
                DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)corr;
                /*
                 * Andrea
                 */
                if (trasmissioneUtente.utente == null)
                {
                    throw new ExceptionTrasmissioni("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + " è inesistente.");
                    //DocsPAWA.utils.AlertPostLoad.GenericMessage(this, "Utente inesistente.");
                }
                //End Andrea
                else
                    trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
            }

            if (corr is DocsPAWA.DocsPaWR.UnitaOrganizzativa)
            {
                DocsPaWR.UnitaOrganizzativa theUo = (DocsPAWA.DocsPaWR.UnitaOrganizzativa)corr;
                DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = new AddressbookQueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                qca.ruolo = UserManager.getRuolo();
                qca.queryCorrispondente = new AddressbookQueryCorrispondente();
                qca.queryCorrispondente.fineValidita = true;

                //				DocsPaWR.Ruolo[] ruoli = UserManager.getListaRuoliInUO (page, (DocsPAWA.DocsPaWR.UnitaOrganizzativa) corr, UserManager.getInfoUtente(page));
                DocsPaWR.Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati(this, qca, theUo);
                /*
                 * Andrea
                 */
                if (ruoli == null || ruoli.Length == 0)
                {
                    throw new ExceptionTrasmissioni("Manca un ruolo di riferimento per la UO: "
                                                    + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + ".");
                    //DocsPAWA.utils.AlertPostLoad.GenericMessage(this, "Manca il ruolo di riferimento");
                }
                //End Andrea
                else
                {
                    foreach (DocsPAWA.DocsPaWR.Ruolo r in ruoli)
                        trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm, scadenza, nascondiVersioniPrecedenti);
                }
                return trasmissione;
            }

            if (trasmissioneSingola != null)
                trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);
            /*
            else
            {
                // In questo caso questa trasmissione non può avvenire perché la
                // struttura non ha utenti (TICKET #1608)
                trasm_strutture_vuote.Add (String.Format ("{0} ({1})", corr.descrizione, corr.codiceRubrica));

            }
            */
            return trasmissione;

        }

        private DocsPAWA.DocsPaWR.Corrispondente[] queryUtenti(DocsPAWA.DocsPaWR.Corrispondente corr)
        {

            //costruzione oggetto queryCorrispondente
            DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();

            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;

            qco.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
            qco.fineValidita = true;

            //corrispondenti interni
            qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;

            return UserManager.getListaCorrispondenti(this, qco);
        }

        private void FillComboModelliTrasm()
        {
            string idAmm = UserManager.getRegistroSelezionato(this).idAmministrazione;
            string idRegistro = UserManager.getRegistroSelezionato(this).systemId;
            Registro[] registri = new Registro[1];
            registri[0] = UserManager.getRegistroSelezionato(this);
            string idPeople = UserManager.getInfoUtente(this).idPeople;
            string idCorrGlobali = UserManager.getInfoUtente(this).idCorrGlobali;
            string idRuoloUtente = UserManager.getInfoUtente(this).idGruppo;
            string idTipoDoc = "";
            string idDiagramma = "";
            string idStato = "";

            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
            {
                if (this._schedaDocumento.tipologiaAtto != null)
                {
                    //DocsPaWR.Templates template = wws.getTemplate(idAmm, this._schedaDocumento.tipologiaAtto.descrizione, this._schedaDocumento.docNumber);
                    //if (template != null)
                    //{
                    //    idTipoDoc = template.SYSTEM_ID.ToString();
                    if (this._schedaDocumento.template != null && this._schedaDocumento.template.SYSTEM_ID.ToString() != "")
                    {
                        idTipoDoc = this._schedaDocumento.template.SYSTEM_ID.ToString();

                        if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                        {
                            DocsPaWR.DiagrammaStato dg = wws.getDgByIdTipoDoc(this._schedaDocumento.tipologiaAtto.systemId, idAmm);
                            if (dg != null)
                            {
                                idDiagramma = dg.SYSTEM_ID.ToString();
                                DocsPaWR.Stato stato = wws.getStatoDoc(this._schedaDocumento.docNumber);
                                if (stato != null)
                                    idStato = stato.SYSTEM_ID.ToString();
                            }
                        }
                    }
                }
            }

            bool AllReg = false;
            if (this._schedaDocumento != null &&
                this._schedaDocumento.tipoProto != null &&
                this._schedaDocumento.tipoProto == "G")
            {
                DocsPAWA.DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistri(this);
                registri = userRegistri;
                AllReg = true;
            }

            //ArrayList idModelli = new ArrayList(wws.getModelliPerTrasm(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, "D", this._schedaDocumento.systemId, idRuoloUtente, AllReg, this._schedaDocumento.accessRights));
            ArrayList idModelli = new ArrayList(wws.getModelliPerTrasmLite(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, "D", this._schedaDocumento.systemId, idRuoloUtente, AllReg, this._schedaDocumento.accessRights));

            if (ddl_tmpl.Items.Count == 0)
            {
                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                li.Text = " ";
                ddl_tmpl.Items.Add(li);
            }


            for (int i = 0; i < idModelli.Count; i++)
            {
                DocsPaWR.ModelloTrasmissione mod = (DocsPAWA.DocsPaWR.ModelloTrasmissione)idModelli[i];
                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();

                if (mod.CEDE_DIRITTI != null && mod.CEDE_DIRITTI.Equals("1") && !userAutorizedEditingACL)
                {
                    continue;
                }
                else
                {
                    li.Value = mod.SYSTEM_ID.ToString();
                    li.Text = mod.NOME;
                    if (System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_CODICE_MODELLI_TRASM"] != null && System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_CODICE_MODELLI_TRASM"] == "1")
                        li.Text += " (" + mod.CODICE + ")";
                    ddl_tmpl.Items.Add(li);
                }
            }
            if (idModelli.Count > 0)
            {
                ddl_tmpl.Items.Add(separatore);
            }
        }

        #endregion

        protected void DataGrid1_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        protected void Datagrid2_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        #region Gestione controllo acl documento

        /// <summary>
        /// Inizializzazione controllo verifica acl
        /// </summary>
        protected virtual void InitializeControlAclDocumento()
        {
            AclDocumento ctl = this.GetControlAclDocumento();
            ctl.IdDocumento = DocumentManager.getDocumentoSelezionato().systemId;
            ctl.OnAclRevocata += new EventHandler(this.OnAclDocumentoRevocata);
        }

        /// <summary>
        /// Reperimento controllo acldocumento
        /// </summary>
        /// <returns></returns>
        protected AclDocumento GetControlAclDocumento()
        {
            return (AclDocumento)this.FindControl("aclDocumento");
        }

        /// <summary>
        /// Listener evento OnAclDocumentoRevocata
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAclDocumentoRevocata(object sender, EventArgs e)
        {
            // Redirect alla homepage di docspa
            SiteNavigation.CallContextStack.Clear();
            SiteNavigation.NavigationContext.RefreshNavigation();
            string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');</script>";
            Response.Write(script);
        }

        #endregion

        #region Gestione notifica utenti

        private DocsPaWR.Trasmissione impostaNotificheUtentiDaModello(DocsPaWR.Trasmissione objTrasm)
        {
            if (objTrasm.trasmissioniSingole != null && objTrasm.trasmissioniSingole.Length > 0)
            {
                for (int cts = 0; cts < objTrasm.trasmissioniSingole.Length; cts++)
                {
                    if (objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length > 0)
                    {
                        for (int ctu = 0; ctu < objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length; ctu++)
                        {
                            objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].daNotificare = this.daNotificareSuModello(objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].utente.idPeople, objTrasm.trasmissioniSingole[cts].corrispondenteInterno.systemId);
                        }
                    }
                }
            }

            return objTrasm;
        }

        private bool daNotificareSuModello(string currentIDPeople, string currentIDCorrGlobRuolo)
        {
            bool retValue = true;

            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
            {
                DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                    if (mittDest.ID_CORR_GLOBALI.Equals(Convert.ToInt32(currentIDCorrGlobRuolo)))
                    {
                        if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 0)
                        {
                            for (int cut = 0; cut < mittDest.UTENTI_NOTIFICA.Length; cut++)
                            {
                                if (mittDest.UTENTI_NOTIFICA[cut].ID_PEOPLE.Equals(currentIDPeople))
                                {
                                    if (mittDest.UTENTI_NOTIFICA[cut].FLAG_NOTIFICA.Equals("1"))
                                        retValue = true;
                                    else
                                        retValue = false;

                                    return retValue;
                                }
                            }
                        }
                    }
                }
            }
            return retValue;
        }
        #endregion

        private void verificaHMdiritti()
        {
            //disabilitazione dei bottoni in base all'autorizzazione di HM 
            //sul documento
            if (_schedaDocumento != null && _schedaDocumento.accessRights != null && _schedaDocumento.accessRights != "")
            {
                if (UserManager.disabilitaButtHMDiritti(_schedaDocumento.accessRights))
                {
                    //bottoni che devono essere disabilitati in caso
                    //di diritti di sola lettura


                    //bottoni che devono essere disabilitati in caso
                    //di documento trasmesso con "Worflow" e ancora da accettare
                    if (UserManager.disabilitaButtHMDirittiTrasmInAccettazione(_schedaDocumento.accessRights))
                    {
                        this.btn_NuovaTrasm.Enabled = false;
                        this.btn_ModifTrasm.Enabled = false;
                        this.butt_Trasm.Enabled = false;
                        this.ddl_tmpl.Enabled = false;
                    }
                }
            }
        }

        private bool notificheUtImpostate(DocsPaWR.ModelloTrasmissione modello)
        {
            bool retValue = true;
            bool flag = false;

            foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
            {
                foreach (DocsPaWR.MittDest mittDest in ragDest.DESTINATARI)
                {
                    if (!mittDest.CHA_TIPO_URP.Equals("U"))
                    {
                        // ritorna FALSE se anche un solo destinatario del modello non ha UTENTI_NOTIFICA
                        if (mittDest.UTENTI_NOTIFICA == null)
                            return false;

                        if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 0)
                        {
                            flag = false;

                            foreach (DocsPaWR.UtentiConNotificaTrasm utNot in mittDest.UTENTI_NOTIFICA)
                            {
                                if (utNot.FLAG_NOTIFICA.Equals("1"))
                                    flag = true;
                            }

                            // ritorna FALSE se anche un solo destinatario ha tutti gli utenti con le notifiche non impostate
                            if (!flag)
                                return false;
                        }
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// GESTIONE CESSIONE DIRITTI:
        /// verifica se l'utente è abilitato alla funzione ABILITA_CEDI_DIRITTI_DOC
        /// </summary>
        private void checkIsAutorizedEditingACL()
        {
            userAutorizedEditingACL = UserManager.ruoloIsAutorized(this, "ABILITA_CEDI_DIRITTI_DOC");
        }


        //Andrea    
        //private void RegisterClientScript(string scriptKey, string scriptValue)
        //{
        //    if (!this.Page.IsStartupScriptRegistered(scriptKey))
        //    {
        //        string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
        //        this.Page.RegisterStartupScript(scriptKey, scriptString);
        //    }
        //}

        //private void AlertJS(string msg)
        //{

        //    string scriptString = "<SCRIPT>alert('" + msg.Replace("|", "\r\n'") + "');</SCRIPT>";
        //    this.Page.RegisterStartupScript("alertJavaScript", scriptString);

        //}
    }
}
