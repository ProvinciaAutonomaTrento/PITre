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
using ProtocollazioneIngresso.Log;
using DocsPAWA;
using DocsPAWA.DocsPaWR;
using System.Configuration;
using DocsPAWA.DocsPaWR;
using System.Collections.Generic;
//Andrea
using DocsPAWA.utils;
//End Andrea


namespace ProtocollazioneIngresso
{
    public enum StatoDocumentoEnum
    {
        NonProtocollato = 0,
        Protocollato = 1,
        ProtocollatoAcquisito = 2,
        ProtocollatoSmistato = 3
    }

    /// <summary>
    /// Summary description for WebForm3.
    /// </summary>
    public class Protocollazione : DocsPAWA.CssPage
    {
        //Andrea
        private ArrayList listaExceptionTrasmissioni = new ArrayList();
        private string messError = "";
        //End Andrea

        protected System.Web.UI.HtmlControls.HtmlInputHidden txtCountDocument;
        protected System.Web.UI.WebControls.TextBox txtNumProtocollo;
        protected DocsPaWebCtrlLibrary.DateMask txtDataProtocollo;
        protected System.Web.UI.WebControls.TextBox txtSegnatura;
        protected System.Web.UI.WebControls.TextBox txtCodMittente;
        protected System.Web.UI.WebControls.TextBox txtDescrMittente;
        protected System.Web.UI.WebControls.TextBox txtCodDest;
        protected System.Web.UI.WebControls.TextBox txtDescrDest;
        protected System.Web.UI.WebControls.TextBox txt_CodFascicolo;
        protected System.Web.UI.WebControls.TextBox txt_DescFascicolo;

        protected DocsPaWebCtrlLibrary.ImageButton btnShowRubrica;
        protected DocsPaWebCtrlLibrary.ImageButton btnShowRubricaDest;
        protected DocsPaWebCtrlLibrary.ImageButton btnShowRubricaMittUsc;
        //protected DocsPaWebCtrlLibrary.DateMask txtDataArrivoProt;
        protected DocsPAWA.UserControls.Calendar txtDataArrivoProt;
        protected System.Web.UI.WebControls.DropDownList cboRegistriDisponibili;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtModAcquisizione;
        //protected System.Web.UI.WebControls.TextBox txtOggetto;
        protected System.Web.UI.WebControls.Label lblRegistriDisponibili;
        protected System.Web.UI.WebControls.Label lblNumProtocollo;
        protected System.Web.UI.WebControls.Label lblDataProtocollo;
        protected System.Web.UI.WebControls.Label lblSegnatura;
        protected System.Web.UI.WebControls.Label lblOggetto;
        protected System.Web.UI.WebControls.Label lblMittente;
        protected System.Web.UI.WebControls.Label lblDataArrivoProt;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtStatoDocumento;
        protected System.Web.UI.HtmlControls.HtmlTable tblContainer;
        protected System.Web.UI.HtmlControls.HtmlTable tblRegistri;
        protected DocsPaWebCtrlLibrary.ImageButton btnNuovoProtocollo;
        protected DocsPaWebCtrlLibrary.ImageButton btnClearData;
        protected System.Web.UI.WebControls.ImageButton btnAcquireDocument;
        protected System.Web.UI.WebControls.ImageButton btnAcquireAttach;
        protected System.Web.UI.WebControls.ImageButton btnClose;
        protected System.Web.UI.HtmlControls.HtmlGenericControl panelUO;
        protected System.Web.UI.HtmlControls.HtmlTable tblButtonsContainer;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtUnloadMode;
        protected System.Web.UI.WebControls.Label lblDataProtocolloMittente;
        protected System.Web.UI.WebControls.Label lblDescrProtMittente;
        protected System.Web.UI.WebControls.TextBox txtDescrProtMittente;
        protected System.Web.UI.WebControls.TextBox txt_num_stampe;
        protected DocsPaWebCtrlLibrary.ImageButton btnPrintSignature;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtDocumentsFolder;
        protected System.Web.UI.WebControls.CompareValidator ctlDataArrivoProtocollo;
        protected DocsPaWebCtrlLibrary.ImageButton btnShowCalDataProtMittente;
        //       protected DocsPaWebCtrlLibrary.ImageButton btnShowCalDataArrivo;
        protected System.Web.UI.WebControls.Label lblStato;
        //      protected DocsPaWebCtrlLibrary.DateMask txtDataProtocolloMittente;
        protected DocsPAWA.UserControls.Calendar txtDataProtocolloMittente;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtDocumentID;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtNewProtocolloPending;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtInsertProtocolloPending;
        protected DocsPaWebCtrlLibrary.ImageButton btnRiproponi;
        protected System.Web.UI.HtmlControls.HtmlTable tblInsertButtonsContainer;
        protected System.Web.UI.WebControls.Label lblDescrizioneDocAcquisiti;
        protected System.Web.UI.WebControls.Label lblDescrizioneAllAcquisiti;
        protected System.Web.UI.WebControls.ImageButton btnProtocolla;
        protected System.Web.UI.WebControls.ImageButton btnProtocollaDisabled;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtPDFConvert;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtPDFConvertEnabled;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtPDFConvertServer;
        protected System.Web.UI.WebControls.CheckBox chkConvertiPDF;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtMessageProtocolloEsistente;
        protected System.Web.UI.WebControls.Panel pnlProtoIngressoSemplificato;
        protected System.Web.UI.WebControls.Panel pnlProtoUscitaSemplificato;
        protected DocsPaWebCtrlLibrary.ImageButton btn_aggiungiDest_P;
        private const string CODICE_MITTENTE_OCCASIONALE = "O";

        private Protocollo.ProtocolloMng _protocolloMng = null;
        private Registro.RegistroMng _registroMng = null;
        private DocsPAWA.DocsPaWR.Registro[] _registriAperti = null;
        protected System.Web.UI.WebControls.CheckBox chkRecognizeText;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtSmartClientActivation;
        protected DocsPaWebCtrlLibrary.IFrameWebControl FrameTest;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtCountAttatchment;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtCountAttachment;
        protected DocsPaWebCtrlLibrary.IFrameWebControl frameTest;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtOcrSupported;
        protected System.Web.UI.WebControls.Image imgStatoRegistro;
        protected System.Web.UI.WebControls.ListBox lbx_dest;
        protected System.Web.UI.WebControls.ListBox lbx_destCC;
        protected DocsPaWebCtrlLibrary.ImageButton btn_insDest;
        protected DocsPaWebCtrlLibrary.ImageButton btn_insDestCC;
        protected DocsPaWebCtrlLibrary.ImageButton btn_cancDest;
        protected DocsPaWebCtrlLibrary.ImageButton btn_cancDestCC;
        protected System.Web.UI.WebControls.TextBox txtCodMittUsc;
        protected System.Web.UI.WebControls.TextBox txtDescMittUsc;
        protected DocsPaWebCtrlLibrary.ImageButton btn_RubrOgget_P;
        protected string codClassifica = "";
        protected DocsPaWebCtrlLibrary.ImageButton imgFasc;
        protected System.Web.UI.WebControls.Label labelFascRapid;
        protected System.Web.UI.HtmlControls.HtmlInputHidden isFascRequired;
        protected DocsPaWebCtrlLibrary.ImageButton btn_Correttore;
        protected DocsPAWA.documento.Oggetto ctrl_oggetto;
        protected System.Web.UI.WebControls.TextBox textOggetto;
        protected System.Web.UI.HtmlControls.HtmlInputHidden isTrasmRapidaRequired;
        protected System.Web.UI.WebControls.CheckBox chkPrivato;
        protected System.Web.UI.WebControls.HiddenField appoIdMod;
        protected System.Web.UI.WebControls.HiddenField appoIdAmm;
        protected DocsPAWA.DocsPaWR.Registro regSelezionato;
        protected DocsPaWebCtrlLibrary.ImageButton imgFascNew;

        protected string tipoProto;
        /// <summary>
        /// Flag, se false l'utente non è autorizzato all'utilizzo della protocollazione in ingresso/uscita
        /// </summary>
        private bool _userAuthorized = false;
        private bool _userAuthorizedUscita = false;
        private bool _oggettoSelezionatoDaOggettario = false;
        private bool isFascRapidaRequired = false;
        private bool isTipologiaDocumentoVisible = false;
        private bool isTipologiaDocumentoRequired = false;
        private bool isFileUploadEnable = false;
        protected System.Web.UI.WebControls.Panel pnl_tipologia_doc;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoAtto;
        protected System.Web.UI.WebControls.ImageButton btn_CampiPersonalizzati;
        protected System.Web.UI.WebControls.Panel pnl_DiagrammiStato;
        protected System.Web.UI.WebControls.Label lbl_statoAttuale;
        protected System.Web.UI.WebControls.DropDownList ddl_statiSuccessivi;
        protected DocsPaWebCtrlLibrary.DateMask txt_dataScadenza;
        protected System.Web.UI.WebControls.Panel pnl_DataScadenza;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtCloseAnteprimaProf;
        protected System.Web.UI.WebControls.Panel pnl_note;
        protected System.Web.UI.WebControls.Button btnNote;
        protected DocsPAWA.Note.DettaglioNota dettaglioNota;
        protected System.Web.UI.WebControls.Label star;
        protected System.Web.UI.WebControls.Label unicaTipoAtto;
        protected System.Web.UI.WebControls.HiddenField unicoCodTipoAtto;
        //protected bool isUniqueTipoAtto;
        protected System.Web.UI.WebControls.HiddenField estendiVisibilita;
        protected System.Web.UI.WebControls.HiddenField abilitaModaleVis;
        protected System.Web.UI.WebControls.HiddenField isInterno;
        protected DocsPaWebCtrlLibrary.ImageButton btn_titolario;
        protected System.Web.UI.WebControls.DropDownList ddl_trasm_rapida;
        protected System.Web.UI.WebControls.Panel pnl_trasm_rapida;

        protected int numeroRuoliDestInTrasmissione = 0;
        protected int numeroUtentiConNotifica = 0;
        protected string idPeopleNewOwner = string.Empty;

        //Mittenti Multipli
        protected DocsPaWebCtrlLibrary.ImageButton btn_RubrMittMultiplo;
        protected bool btn_RubrMittMultiplo_state = true;
        protected System.Web.UI.WebControls.Panel panel_DettaglioMittenti;
        //protected System.Web.UI.WebControls.TextBox txt_codMittMultiplo;
        //protected System.Web.UI.WebControls.TextBox txt_descMittMultiplo;
        protected DocsPaWebCtrlLibrary.ImageButton btn_CancMittMultiplo;
        //protected DocsPaWebCtrlLibrary.ImageButton btn_insMittMultiplo;
        protected System.Web.UI.WebControls.ListBox lbx_mittMultiplo;

        protected System.Web.UI.HtmlControls.HtmlGenericControl checkConverti;
        protected System.Web.UI.HtmlControls.HtmlGenericControl checkRecognize;
        protected StampaEtichetta StampaEtichetta;

        protected DocsPAWA.UserControls.RubricaVeloce rubrica_veloce;
        protected DocsPAWA.UserControls.RubricaVeloce rubrica_veloce_destinatario;
        protected DocsPAWA.UserControls.RubricaVeloce rubrica_veloce_destinatario_mittente_uscita;
        protected DocsPAWA.UserControls.RubricaVeloce rubrica_mitt_multiplo_semplificato;
        protected DocsPaWebCtrlLibrary.ImageButton btn_upMittente;
        protected DocsPaWebCtrlLibrary.ImageButton btn_downMittente;

        protected System.Web.UI.WebControls.Button btn_nascosto_mitt_multipli;
        protected System.Web.UI.WebControls.HiddenField txt_cod_mitt_mult_nascosto;
        protected System.Web.UI.WebControls.HiddenField txt_desc_mitt_mult_nascosto;
        protected bool isRiproposto;

        protected System.Web.UI.WebControls.Panel pnl_new_mittente_semplificato_ingresso_veloce;
        protected AjaxControlToolkit.AutoCompleteExtender new_mitt_veloce_sempl_ing;

        protected System.Web.UI.WebControls.Panel pnl_new_mittente_uscita_semplificato;
        protected AjaxControlToolkit.AutoCompleteExtender new_mitt_veloce_sempl_usc;

        protected System.Web.UI.WebControls.Panel pnl_new_destinatario_uscita_semplificato;
        protected AjaxControlToolkit.AutoCompleteExtender new_dest_veloce_sempl_usc;
        protected Dictionary<string, Corrispondente> dic_Corr;

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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cboRegistriDisponibili.SelectedIndexChanged += new System.EventHandler(this.cboRegistriDisponibili_SelectedIndexChanged);
            this.btnPrintSignature.Click += new System.Web.UI.ImageClickEventHandler(this.btnPrintSignature_Click);
            //this.txtOggetto.TextChanged += new System.EventHandler(this.txtOggetto_TextChanged);
            this.ctrl_oggetto.OggettoChangedEvent += new DocsPAWA.documento.Oggetto.OggettoChangedDelegate(this.ctrl_oggetto_OggChanged);
            this.txtCodMittente.TextChanged += new System.EventHandler(this.txtCodMittente_TextChanged);
            this.txtDescrMittente.TextChanged += new System.EventHandler(this.txtDescrMittente_TextChanged);
            this.btnShowRubrica.Click += new System.Web.UI.ImageClickEventHandler(this.btnShowRubrica_Click);
            this.btnShowRubricaDest.Click += new System.Web.UI.ImageClickEventHandler(this.btnShowRubricaDest_Click);
            this.btnNuovoProtocollo.Click += new System.Web.UI.ImageClickEventHandler(this.btnNuovoProtocollo_Click);
            this.btnRiproponi.Click += new System.Web.UI.ImageClickEventHandler(this.btnRiproponi_Click);
            this.ddl_tipoAtto.SelectedIndexChanged += new System.EventHandler(this.ddl_tipoAtto_SelectedIndexChanged);
            this.btnClearData.Click += new System.Web.UI.ImageClickEventHandler(this.btnClearData_Click);
            this.btnProtocolla.Click += new System.Web.UI.ImageClickEventHandler(this.btnProtocolla_Click);
            this.btnProtocollaDisabled.Click += new System.Web.UI.ImageClickEventHandler(this.btnProtocolla_Click);
            this.btnClose.Click += new System.Web.UI.ImageClickEventHandler(this.btnClose_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);
            this.btn_insDestCC.Click += new System.Web.UI.ImageClickEventHandler(this.btn_insDestCC_Click);
            this.btn_insDest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_insDest_Click);
            this.btn_aggiungiDest_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungiDest_P_Click);
            this.btn_cancDest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_cancDest_Click);
            this.btn_cancDestCC.Click += new System.Web.UI.ImageClickEventHandler(this.btn_cancDestCC_Click);
            this.txtCodDest.TextChanged += new System.EventHandler(this.txtCodDestinatario_TextChanged);
            this.btnShowRubricaMittUsc.Click += new System.Web.UI.ImageClickEventHandler(this.btnShowRubricaMittUsc_Click);
            this.txtDescMittUsc.TextChanged += new System.EventHandler(this.txtDescMittUsc_TextChanged);
            this.txtCodMittUsc.TextChanged += new System.EventHandler(this.txtCodMittUsc_TextChanged);
            this.btn_RubrOgget_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_RubrOgget_P_Click);
            this.txt_CodFascicolo.TextChanged += new System.EventHandler(this.txt_cod_fasc_TextChanged);
            this.imgFasc.Click += new System.Web.UI.ImageClickEventHandler(this.imgFasc_Click);
            this.imgFascNew.Click += new System.Web.UI.ImageClickEventHandler(this.imgFascNew_Click);
            //this.txt_codMittMultiplo.TextChanged += new System.EventHandler(this.txt_codMittMultiplo_TextChanged);
            //this.btn_insMittMultiplo.Click += new ImageClickEventHandler(btn_insMittMultiplo_Click);
            this.btn_CancMittMultiplo.Click += new ImageClickEventHandler(btn_CancMittMultiplo_Click);
            if (System.Configuration.ConfigurationManager.AppSettings["ENABLE_ACQUISIZ_DAFILE_PROT_SEMPL"] != null
                && System.Configuration.ConfigurationManager.AppSettings["ENABLE_ACQUISIZ_DAFILE_PROT_SEMPL"].Equals("true"))
            {
                this.btnAcquireDocument.Click += new System.Web.UI.ImageClickEventHandler(this.btnAcquireDocument_Click);
                this.btnAcquireAttach.Click += new System.Web.UI.ImageClickEventHandler(this.btnAcquireAttach_Click);
                this.isFileUploadEnable = true;
                this.checkConverti.Visible = false;
                this.checkRecognize.Visible = false;
                //this.StampaEtichetta.Visible = false;
            }

            this.btn_upMittente.Click += new ImageClickEventHandler(btn_upMittente_Click);
            this.btn_downMittente.Click += new ImageClickEventHandler(btn_downMittente_Click);
            this.btn_nascosto_mitt_multipli.Click += new EventHandler(btn_nascosto_mitt_multipli_Click);
            this.chkPrivato.CheckedChanged += new System.EventHandler(this.chkPrivato_CheckedChanged);
            this.ddl_trasm_rapida.SelectedIndexChanged += new System.EventHandler(this.ddl_trasm_rapida_SelectedIndexChanged);
        }
        #endregion

        #region Gestione javascript

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
        /// Associazione funzioni agli handler javascript dei controlli
        /// </summary>
        private void RegisterClientEvents(string tipoProto)
        {
            string isAcquisizioneDocumentiObbligatoria = this.IsAcquisizioneDocumentiObbligatoria().ToString().ToLower();

            this.btn_RubrOgget_P.Attributes.Add("onclick", "ApriModalOggettario('protoSempl');");
            
            this.btnProtocolla.Attributes.Add("onClick", "ShowWaitCursor(); EnableButtonProtocollazione(false);");
            this.btnNuovoProtocollo.Attributes.Add("onClick", "ShowWaitCursor();");
            this.btnRiproponi.Attributes.Add("onClick", "ShowWaitCursor();");
            this.btnClearData.Attributes.Add("onClick", "ShowWaitCursor();");


            if (System.Configuration.ConfigurationManager.AppSettings["ENABLE_ACQUISIZ_DAFILE_PROT_SEMPL"] != null
                && System.Configuration.ConfigurationManager.AppSettings["ENABLE_ACQUISIZ_DAFILE_PROT_SEMPL"].Equals("false")
                || System.Configuration.ConfigurationManager.AppSettings["ENABLE_ACQUISIZ_DAFILE_PROT_SEMPL"] == null)
            {
                this.btnAcquireDocument.Attributes.Add("onClick", "ShowWaitCursor(); ScanSingleDocument(false);");
                this.btnAcquireAttach.Attributes.Add("onClick", "ShowWaitCursor(); ScanSingleAttachment(false);");
                this.chkConvertiPDF.Attributes.Add("onClick", "OnCheckConvertPDF();");
                
            }
            /*else {
                if ((System.Configuration.ConfigurationManager.AppSettings["ENABLE_ACQUISIZ_DAFILE_PROT_SEMPL"] == null))
                {
                    this.btnAcquireDocument.Attributes.Add("onClick", "ShowWaitCursor(); ScanSingleDocument(false);");
                    this.btnAcquireAttach.Attributes.Add("onClick", "ShowWaitCursor(); ScanSingleAttachment(false);");
                }
            }*/
            
            this.btnClose.Attributes.Add("onClick", "ShowWaitCursor();");

            

            if (tipoProto.Equals("A"))
            {
                this.txtCodMittente.Attributes.Add("onKeyUp", "RefreshButtonProtocollaEnabled();");
                this.txtDescrMittente.Attributes.Add("onKeyUp", "RefreshButtonProtocollaEnabled();");
                //this.btnShowCalDataProtMittente.Attributes.Add("onClick", "ShowCalendarDataProtMitt();");
                //this.btnShowCalDataArrivo.Attributes.Add("onClick", "ShowCalendarDataArrivoProt();");
                //this.txtOggetto.Attributes.Add("onKeyUp", "RefreshButtonProtocollaEnabled();");
                this.ctrl_oggetto.Ogg_Add_Attribute("onKeyUp", "RefreshButtonProtocollaEnabled();");
                if (isFascRapidaRequired)
                    this.txt_CodFascicolo.Attributes.Add("onKeyUp", "RefreshButtonProtocollaEnabled();");

                this.btnShowRubrica.Attributes.Add("onClick", "ShowWaitCursor(); ShowDialogRubrica();");
                this.btn_RubrMittMultiplo.Attributes.Add("onClick", "ShowWaitCursor(); ShowDialogRubricaMittMultipli();");
            }
            else
            {
                if (tipoProto.Equals("P"))
                {
                    this.btnShowRubricaDest.Attributes.Add("onClick", "ShowWaitCursor(); ShowDialogRubricaDest();");
                    this.btnShowRubricaMittUsc.Attributes.Add("onClick", "ShowWaitCursor(); ShowDialogRubricaMittUsc();");
                    if (isFascRapidaRequired)
                        this.txt_CodFascicolo.Attributes.Add("onKeyUp", "RefreshButtonProtocollaUscitaEnabled();");
                    //this.txtOggetto.Attributes.Add("onKeyUp", "RefreshButtonProtocollaUscitaEnabled();");
                    this.ctrl_oggetto.Ogg_Add_Attribute("onKeyUp", "RefreshButtonProtocollaUscitaEnabled();");
                }
            }
        }

        /// <summary>
        /// Impostazione del focus su un controllo
        /// </summary>
        /// <param name="controlID"></param>
        //private void SetControlFocus(string controlID)
        //{
        //    this.RegisterClientScript("SetFocus", "SetControlFocus('" + controlID + "');");
        //}

        #endregion

        #region Gestione eventi pagina

        private void Page_Load(object sender, System.EventArgs e)
        {
            //Inizializzazione user control Oggetto
            ctrl_oggetto = this.GetControlOggetto();
            //Attivo la ricerca sul codice oggetto
            ctrl_oggetto.cod_oggetto_postback = true;
            //abilito l'auto postback per aggiornare il valore
            ctrl_oggetto.oggetto_postback = true;
            //Imposto l'aspetto del controllo oggetto
            ctrl_oggetto.DimensioneOggetto("estesa", "protoSempl");

            Response.Expires = -1;

            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_TIPOLOGIA_DOC_PROT_SEMPL) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_TIPOLOGIA_DOC_PROT_SEMPL).Equals("1"))
                this.isTipologiaDocumentoVisible = true;
            else
                this.isTipologiaDocumentoVisible = false;

            if (isTipologiaDocumentoVisible)
            {
                this.pnl_tipologia_doc.Visible = true;

                isTipologiaDocumentoRequired = this.IsRequiredTipologiaAtto();
                this.star.Visible = isTipologiaDocumentoRequired;

                //if (Session["daAnteprimaProf"] != null && (bool)Session["daAnteprimaProf"])
                //{
                //    this.ddl_tipoAtto.SelectedIndex = 0;
                //    this.pnl_DataScadenza.Visible = false;
                //    this.pnl_DiagrammiStato.Visible = false;
                //    Session.Remove("daAnteprimaProf");
                //    SchedaDocumento sc = this.GetProtocolloManager().GetDocumentoCorrente();
                //    if (sc != null && sc.tipologiaAtto != null)
                //        sc.tipologiaAtto = null;
                //}
            }

            //abilito il pulsante di creazione diretta dei fascicoli procedimentali
            if (DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NUOVO_FASC_DIRECT") != null && DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NUOVO_FASC_DIRECT").Equals("1"))
            {
                this.imgFascNew.Visible = true;
            }

            if (!IsPostBack)
            {
                Session.Remove("tipoProto");
                //rimuovo il documento creato in precedenza
                this.GetProtocolloManager().ReleaseDocumentoCorrente();
                CaricaComboTipologiaAtto(this.ddl_tipoAtto);
            }

            if (Session["oggettario.retValue"] != null)
            {
                if ((bool)Session["oggettario.retValue"])
                    this._oggettoSelezionatoDaOggettario = true;

                Session.Remove("oggettario.retValue");
            }

            // chiamata al web service per sapere se la fascicolazione rapida è obbligatoria o no
            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            string idAmm = UserManager.getInfoUtente(this).idAmministrazione;
            //this.isFascRapidaRequired = ws.IsFascRapidaRequired(idAmm);
            string valoreChiaveFasc = DocsPAWA.utils.InitConfigurationKeys.GetValue(idAmm, "FE_FASC_RAPIDA_REQUIRED");
            if (valoreChiaveFasc != null && valoreChiaveFasc.ToUpper().Equals("TRUE"))
                this.isFascRapidaRequired = true;
            else
                this.isFascRapidaRequired = false;

            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_NOTE_PROT_SEMPL) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_NOTE_PROT_SEMPL).Equals("1"))
            {
                this.pnl_note.Visible = true;
                SchedaDocumento sc = new SchedaDocumento();
                if (this.GetProtocolloManager().GetDocumentoCorrente() == null)
                    DocumentManager.setDocumentoInLavorazione(sc);
                else
                    DocumentManager.setDocumentoInLavorazione(this.GetProtocolloManager().GetDocumentoCorrente());
            }
            if (Session["docRiproposto"] != null && Session["docRiproposto"].ToString().ToLower().Equals("true"))
                isRiproposto = true;
            if (isFascRapidaRequired)
            {
                this.isFascRequired.Value = "true";
                this.labelFascRapid.Text = "Fasc. Rapida: *";
            }
            else
            {
                this.isFascRequired.Value = "false";
                this.labelFascRapid.Text = "Fasc. Rapida:";
            }

            this.btnProtocolla.Style["display"] = "none";

            tipoProto = Request.QueryString["proto"];

            Session["tipoProto"] = tipoProto;


            if (tipoProto != null)
            {
                switch (tipoProto)
                {
                    case "A": // PROTOCOLLO IN INGRESSO
                        this.pnlProtoIngressoSemplificato.Visible = true;
                        this.pnlProtoUscitaSemplificato.Visible = false;
                        
                        if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                        {
                            this.rubrica_veloce.Visible = true;
                            this.rubrica_veloce_destinatario.Visible = false;
                            this.rubrica_veloce_destinatario_mittente_uscita.Visible = false;
                            if (DocumentManager.isEnableMittentiMultipli())
                            {
                                this.rubrica_mitt_multiplo_semplificato.Visible = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                        {
                            pnl_new_mittente_semplificato_ingresso_veloce.Visible = true;
                            pnl_new_mittente_uscita_semplificato.Visible = false;
                            pnl_new_destinatario_uscita_semplificato.Visible = false;
                            this.txtDescMittUsc.ReadOnly = true;
                        }
                       
                        if (ConfigSettings.getKey(ConfigSettings.KeysENUM.TITLE) != null)
                            this.Page.Title = ConfigSettings.getKey(ConfigSettings.KeysENUM.TITLE) + "> Protocollazione in ingresso";
                        else
                            this.Page.Title = "DOCSPA > Protocollazione in ingresso";
                        int altezza = 380;
                        if (ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_TIPOLOGIA_DOC_PROT_SEMPL) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_TIPOLOGIA_DOC_PROT_SEMPL).Equals("1"))
                            altezza = altezza - 50;

                        if (ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_NOTE_PROT_SEMPL) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_NOTE_PROT_SEMPL).Equals("1"))
                            altezza = altezza - 80;

                        this.panelUO.Style.Add("height", altezza.ToString());

                        // Verifica se l'utente corrente è abilitato alla protocollazione in ingresso
                        this._userAuthorized = this.IsUtenteAutorizzato();
                        break;

                    case "P": // PROTOCOLLO IN USCITA
                        if (ConfigSettings.getKey(ConfigSettings.KeysENUM.TITLE) != null)
                            this.Page.Title = ConfigSettings.getKey(ConfigSettings.KeysENUM.TITLE) + "> Protocollazione in uscita";
                        else
                            this.Page.Title = "DOCSPA > Protocollazione in uscita";
                       // SettaMittenteDefault();
                        this.pnlProtoIngressoSemplificato.Visible = false;
                        this.pnlProtoUscitaSemplificato.Visible = true;

                        if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                        {
                            this.rubrica_veloce.Visible = false;
                            this.rubrica_veloce_destinatario.Visible = true;
                            this.rubrica_veloce_destinatario_mittente_uscita.Visible = true;
                            this.rubrica_mitt_multiplo_semplificato.Visible = false;
                        }

                        if (!string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                        {
                            pnl_new_mittente_semplificato_ingresso_veloce.Visible = false;
                            pnl_new_mittente_uscita_semplificato.Visible = true;
                            pnl_new_destinatario_uscita_semplificato.Visible = true;
                            this.txtDescMittUsc.ReadOnly = false;
                        }

                        //                        this.Title = this.Title + " Protocollazione in uscita";
                        altezza = 300;
                        if (ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_TIPOLOGIA_DOC_PROT_SEMPL) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_TIPOLOGIA_DOC_PROT_SEMPL).Equals("1"))
                            altezza = altezza - 40;

                        if (ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_NOTE_PROT_SEMPL) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_NOTE_PROT_SEMPL).Equals("1"))
                            altezza = altezza - 60;

                        this.panelUO.Style.Add("height", altezza.ToString());

                        // Verifica se l'utente corrente è abilitato alla protocollazione in uscita semplificata
                        this._userAuthorizedUscita = this.IsUtenteAutorizzatoUscita();
                        break;
                }
            }

            // verifica se esistono le ragioni di trasmissione utili allo smistamento
            //if(this.CheckExistRagTrasm())
            RagioneTrasmissione[] listaRagioniSmistamento = null;
            if (this.CheckExistenzaRagioniSmistamento(out listaRagioniSmistamento))
            {
                if (!this._userAuthorized && tipoProto.ToUpper().Equals("A"))
                    this.ShowMessageAccessoNegato("Utente non abilitato alla protocollazione in ingresso");
                else
                {
                    if (!this._userAuthorizedUscita && tipoProto.ToLower().Equals("p"))
                        this.ShowMessageAccessoNegato("Utente non abilitato alla protocollazione in uscita");
                    else
                    {
                        // Verifica della presenza di registri aperti
                        bool containsRegistri = this.ContainsRegistriAperti();

                        if (!containsRegistri)
                            this.ShowMessageAccessoNegato("Nessun registro disponibile su cui protocollare documenti");

                        if (containsRegistri)
                        {
                            // Impostazione delle configurazioni del file web.config
                            if (!isFileUploadEnable)
                                this.SetWebConfigConfigurations();

                            if (!this.IsPostBack)
                            {
                                this.SmartNavigation = true;

                                this.SetBackColorFieldsReadOnly();

                                this.LoadModalitaAcquisizioneDocumenti(idAmm);

                                this.RegisterClientEvents(tipoProto);

                                // Caricamento della combo dei registri
                                this.FillComboRegistri();

                                // Selezione del primo registro disponibile
                                this.SetRegistroCorrente(this.cboRegistriDisponibili.SelectedValue);

                                // Predisposizione per un nuovo documento
                                this.CreateNewDocument(false, tipoProto);

                                if(tipoProto=="P")
                                    SettaMittenteDefault(false);
                            }

                            // Impostazione pulsante di protocollazione come pulsante di default
                            this.SetDefaultButton(tipoProto);

                            // Ripristiono valore check "chkConvertiPDF" in base al valore
                            // del campo nascosto "txtPDFConvert"
                            //this.RestoreValueCheckConvertiPDF();

                            // Abilitazione / disabilitazione check "chkConvertiPDF"
                            if (!isFileUploadEnable)
                                this.RegisterClientScript("SetEnabledCheckConvertiPDF", "SetEnabledCheckConvertiPDF();");

                            // Registrazione controllo per l'acquisizione dei documenti
                            // con i componenti smartclient
                            this.RegisterControlAcquisizioneSmartClient();

                            // Registrazione controllo che verifica le
                            // opzioni correnti relative al convertitore pdf utilizzato
                            if (!isFileUploadEnable)
                                this.RegisterControlPdfCapabilities();

                            // Verifica se è in pending la protocollazione del documento corrente
                            if (this.OnInsertProtocolloPending())
                            {
                                string errorMessage;
                                if (!this.ProtocollaDocumentoCorrente(out errorMessage))
                                {
                                    // Abilitazione pulsante protocollazione che, in caso
                                    // di errore, permette di rieseguire l'operazione
                                    // con gli stessi dati visualizzati
                                    this.RegisterClientScript("EnableButtonProtocollazione", "EnableButtonProtocollazione(true);");

                                    string validationMessage = "Errore nella protocollazione del documento: \\n" + errorMessage;
                                    this.RegisterClientScript("ErrorOnProtocollazione", "alert(\"" + validationMessage + "\");");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                this.SetStatoDocumento(StatoDocumentoEnum.NonProtocollato);
                this.txtModAcquisizione.Value = "true";
                string msg = "Attenzione, non esistono le ragioni di trasmissione COMPETENZA e CONOSCENZA utili allo smistamento.";
                string scriptString = "<SCRIPT>alert('" + msg + "');window.close();</SCRIPT>";
                //this.Page.RegisterStartupScript("AlertModalDialog", scriptString);
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "AlertModalDialog", scriptString);
            }


         /*   if (this.isInterno.Value == "true")
            {
                
                //string idAmm = UserManager.getInfoUtente(this).idAmministrazione;
                if (ws.ereditaVisibilita(idAmm, "null"))
                {
                    abilitaModaleVis.Value = "true";
                    //ClientScript.RegisterStartupScript(this.GetType(), "openAvvisoVisibilita", "AvvisoVisibilita();", true);
                }
                else
                {
                    abilitaModaleVis.Value = "false";
                }
            }
            else
            {
                abilitaModaleVis.Value = "false";
            }
          * */

            if (appoIdAmm.Value == string.Empty)
            {
                if (regSelezionato != null)
                {
                    appoIdAmm.Value = regSelezionato.idAmministrazione;
                }
                else
                {
                    //Se il registro è uguale a null qui schianta perchè si prende i dati lo stesso!!
                    //regSelezionato = UserManager.getRegistroSelezionato(this);
                    //appoIdAmm.Value = regSelezionato.idAmministrazione;
                }

                if (string.IsNullOrEmpty(appoIdMod.Value))
                {
                    appoIdMod.Value = "null";
                }

                if (isInterno.Value == "true")
                {
                    if (ws.ereditaVisibilita(appoIdAmm.Value, appoIdMod.Value))
                    {
                        this.abilitaModaleVis.Value = "true";
                        //ClientScript.RegisterStartupScript(this.GetType(), "openAvvisoVisibilita", "AvvisoVisibilita();", true);
                    }
                    else
                    {
                        this.abilitaModaleVis.Value = "false";
                    }
                }
            }
            //se vengono rimossi tutti i destinatari interni NON devo più visualizzare la modale
            //a meno che non sia stato selezionato un modello!!!
            if (isInterno.Value != "true")
            {
                appoIdAmm.Value = string.Empty;
                if (appoIdMod.Value == "null" || string.IsNullOrEmpty(appoIdMod.Value))
                {
                    this.abilitaModaleVis.Value = "false";
                }
            }

            if (!IsPostBack)
            {
                DocumentManager.removeClassificazioneSelezionata(this);
                FascicoliManager.removeFascicoloSelezionatoFascRapida();
            }

            //Nodo titolario scelto
            DocsPAWA.DocsPaWR.FascicolazioneClassificazione classificazione = DocumentManager.getClassificazioneSelezionata(this);
            if (classificazione != null && !string.IsNullOrEmpty(classificazione.codice))
            {
                this.txt_CodFascicolo.Text = classificazione.codice;
                //this.txt_DescFascicolo.Text = classificazione.descrizione;
                

                //OLD:   Fascicolo Fasc = FascicoliManager.getFascicoloDaCodice(this,);

                DocsPAWA.DocsPaWR.Registro registroCorrente = this.GetRegistroCorrente();

                Fascicolo[] FascS = FascicoliManager.getListaFascicoliDaCodice(this, classificazione.codice, registroCorrente, "I");

                Fascicolo Fasc = null;

                if (FascS != null && FascS.Length > 0 && FascS[0] != null)
                {
                    Fasc = (Fascicolo)FascS[0];

                }
                FascicoliManager.setFascicoloSelezionatoFascRapida(Fasc);
                DocumentManager.setClassificazioneSelezionata(this, null);
            }

            if (!Page.IsPostBack)
            {
                //abilito pannello per la trasmissione rapida
                this.pnl_trasm_rapida.Visible = false;
                this.isTrasmRapidaRequired.Value = "false";
                if (cfg_Smista_Abilita_Trasm_Rapida())
                {
                    this.pnl_trasm_rapida.Visible = true;
                    this.isTrasmRapidaRequired.Value = "true";
                    this.caricaModelliTrasmRapida();
                }

                this.ctrl_oggetto.oggetto_SetControlFocus();
            }
            else
            {
                this.Form.Focus();
            }

            ////Andrea
            //if (Session["MessError"] != null)
            //{
            //    messError = Session["MessError"].ToString();
            //    Response.Write("<script language=\"javascript\">alert('Trasmissioni avvenute con successo per tutti i destinatari ad eccezione delle seguenti: \\n" + messError + "');</script>");
            //    Session.Remove("MessError");
            //}
            ////End Andrea
        }

        private bool CheckExistenzaRagioniSmistamento(out RagioneTrasmissione[] listaRagioniSmistamento)
        {
            listaRagioniSmistamento = null;
            bool retValue = false;
            if (DocsPAWA.smistaDoc.SmistaDocSessionManager.ExistSessionRagTrasm())
            {
                retValue = true;
            }
            else
            {
                DocsPAWA.smistaDoc.SmistaDocManager docManager = new DocsPAWA.smistaDoc.SmistaDocManager();
                listaRagioniSmistamento = docManager.GetListaRagioniSmistamento(UserManager.getInfoUtente(this));
                if (listaRagioniSmistamento != null && listaRagioniSmistamento.Length == 2)
                {
                    retValue = true;
                }
                if (retValue)
                    DocsPAWA.smistaDoc.SmistaDocSessionManager.SetSessionRagTrasm();
            }
            return retValue;
        }

        //elisa
        private void SettaMittenteDefault(bool associaMittaProtocollo)
        {
            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();
            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.MITTENTE_DEFAULT) != null && this.txtDescMittUsc.Text.Equals("") && this.txtCodMittUsc.Text.Equals("") && schedaDocumento != null)
            {
                string mittenteDefault = ConfigSettings.getKey(ConfigSettings.KeysENUM.MITTENTE_DEFAULT).ToLower();
                // luluciani errore in ENAC se impostato mitt default su web.config, anche se lo cerco e cambio da rubrica o da codice quando 
                //protocollo e refresh ritorna il default.
                //OLD	if(mittenteDefault=="1" )
                Corrispondente corrMitt = null;

                if (schedaDocumento != null && schedaDocumento.protocollo != null)
                {
                    if (schedaDocumento.tipoProto == "P")
                        corrMitt = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente;

                }
                string descrizione = txtCodMittUsc.Text.Trim();

                if (mittenteDefault == "1" && !(corrMitt != null && corrMitt.descrizione != null && corrMitt.descrizione != "") && schedaDocumento.tipoProto == "P")
                {
                    if (UserManager.getRuolo(this) != null)
                    {
                        Ruolo ruo = UserManager.getRuolo(this);
                        Corrispondente corr = ruo.uo;
                        if (corr != null)
                        {
                            this.txtCodMittUsc.Text = corr.codiceRubrica;
                            this.txtDescMittUsc.Text = corr.descrizione;                           
                            if (this.txtCodMittUsc.Text.Trim() != string.Empty)
                            {
                                setDescrizioneCorrispondenteUscita("Mit");
                            }
                            else
                            {
                                this.txtDescMittUsc.Text = String.Empty;
                                SetCorrispondenteMittenteInProtocolloUscita(null);
                            }
                        }
                    }
                }
            }
            else if (ConfigSettings.getKey(ConfigSettings.KeysENUM.MITTENTE_DEFAULT) != null && schedaDocumento != null)
            {
                string mittenteDefault = ConfigSettings.getKey(ConfigSettings.KeysENUM.MITTENTE_DEFAULT).ToLower();
                // luluciani errore in ENAC se impostato mitt default su web.config, anche se lo cerco e cambio da rubrica o da codice quando 
                //protocollo e refresh ritorna il default.
                //OLD	if(mittenteDefault=="1" )
                Corrispondente corrMitt = null;

                if (schedaDocumento != null && schedaDocumento.protocollo != null)
                {
                    if (schedaDocumento.tipoProto.Equals("P"))
                        corrMitt = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente;
                    else
                        corrMitt = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente;
                }
                string descrizione = txtCodMittUsc.Text.Trim();                

                if (mittenteDefault == "1" && !(corrMitt != null && corrMitt.descrizione != null && corrMitt.descrizione != ""))
                {
                    if (UserManager.getRuolo(this) != null)
                    {
                        Ruolo ruo = UserManager.getRuolo(this);
                        Corrispondente corr = ruo.uo;
                        if (corr != null)
                        {
                            this.txtCodMittUsc.Text = corr.codiceRubrica;                          
                            this.txtDescMittUsc.Text = corr.descrizione;                            
                            if (associaMittaProtocollo)
                            {
                                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente = corr;
                            }
                        }
                    }
                }

            }
        }

        #region tipologia documento

        protected void btn_CampiPersonalizzati_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (ddl_tipoAtto.SelectedIndex == 0)
                return;

            TipologiaAtto[] listaTipologiaAtto = DocumentManager.getListaTipologiaAtto(this);
           //UNIUQUE TIPO ATTO CODICE COMMENTATO
            /* if (listaTipologiaAtto.Length == 2)
                Session.Add("isUniqueTipoAtto", "true");
            else
                Session.Add("isUniqueTipoAtto", "false");
            */
            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();
            
             string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
             Templates template1 = (Templates)Session["template"];
             if (Session["template"] == null)
             {
                 if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
                 {
                     Templates template = ProfilazioneDocManager.getTemplateDettagli(schedaDocumento.docNumber,this);
                     if (template != null && template.SYSTEM_ID != 0 && template.ELENCO_OGGETTI.Length > 0)
                     {
                         //Se la tipologia documento è privato allora il documento deve essere privato
                         if (template.PRIVATO != null && template.PRIVATO == "1")
                             schedaDocumento.privato = "1";
                         else
                             schedaDocumento.privato = "0";

                        if (Session["tipoProto"].Equals("A"))
                            this.GetProtocolloManager().setDatiProtocolloIngresso(schedaDocumento);
                        else
                            this.GetProtocolloManager().setDatiProtocolloUscita(schedaDocumento);

                        Session.Add("template", template);
                    }
                }
            }
            DocumentManager.setDocumentoInLavorazione(schedaDocumento);
            ClientScript.RegisterStartupScript(this.GetType(), "apriPopupAnteprima", "apriPopupAnteprima();", true);
        }

        private void CaricaComboTipologiaAtto(DropDownList ddl)
        {
            TipologiaAtto[] listaTipologiaAtto;
            string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
            if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
            {
                //listaTipologiaAtto = DocumentManager.getListaTipologiaAtto(this, UserManager.getInfoUtente(this).idAmministrazione);

                //sch sarà diverso da null solo se arrivo da ricerca, o se il documento è stato creato
                SchedaDocumento sch = this.GetProtocolloManager().GetDocumentoCorrente();
                //SchedaDocumento sch = DocumentManager.getDocumentoInLavorazione();
                if (sch != null && sch.systemId != null && sch.systemId != "" && sch.tipologiaAtto != null
                  && sch.tipologiaAtto.systemId != null && sch.tipologiaAtto.systemId != "")
                    listaTipologiaAtto = DocumentManager.getListaTipologiaAtto(this);
                else
                    listaTipologiaAtto = DocumentManager.getTipoAttoPDInsRic(this, UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "2");
            }
            else
            {
                listaTipologiaAtto = DocumentManager.getListaTipologiaAtto(this);
            }
            ddl.Items.Clear();
            //aggiunge una riga vuota alla combo
            ddl.Items.Add("");
            if (listaTipologiaAtto != null)
            {
                //MANCA LA CHIVAE PER UNIQUR TIPO ATTO, SE MAI UN GIORNO SERVIRA LO MODIFICHEREMO ORA E INUTILE :)
              /*  if (listaTipologiaAtto.Length == 1)
                {
                    Session.Add("isUniqueTipoAtto", "true");
                    // this.isUniqueTipoAtto = true;
                    this.ddl_tipoAtto.Visible = false;
                    this.unicaTipoAtto.Visible = true;
                    this.unicaTipoAtto.Text = listaTipologiaAtto[0].descrizione;
                    this.unicoCodTipoAtto.Value = listaTipologiaAtto[0].systemId;
                    
                    
                    Session.Remove("template");
                    SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();
                    if (schedaDocumento == null)
                        schedaDocumento = new SchedaDocumento();

                    TipologiaAtto tipologiaAtto = new DocsPAWA.DocsPaWR.TipologiaAtto();
                    tipologiaAtto.systemId = this.unicoCodTipoAtto.Value;
                    tipologiaAtto.descrizione = this.unicaTipoAtto.Text;
                    schedaDocumento.tipologiaAtto = tipologiaAtto;
                    schedaDocumento.daAggiornareTipoAtto = true;
                    if(isTipologiaDocumentoRequired)
                    {
                        //ClientScript.RegisterStartupScript(this.GetType(), "apriPopupAnteprima", "apriPopupAnteprima();", true);
                        settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
                        if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
                        {
                            Templates template = ProfilazioneDocManager.getTemplateDettagli(schedaDocumento.docNumber,this);
                            if (template != null && template.SYSTEM_ID != 0 && template.ELENCO_OGGETTI.Length > 0)
                            {
                                //Se la tipologia documento è privato allora il documento deve essere privato
                                if (template.PRIVATO != null && template.PRIVATO == "1")
                                    schedaDocumento.privato = "1";
                                else
                                    schedaDocumento.privato = "0";

                                if (Session["tipoProto"].Equals("A"))
                                    this.GetProtocolloManager().setDatiProtocolloIngresso(schedaDocumento);
                                else
                                    this.GetProtocolloManager().setDatiProtocolloUscita(schedaDocumento);

                                DocumentManager.setDocumentoInLavorazione(schedaDocumento);
                                Session.Add("template", template);
                                ClientScript.RegisterStartupScript(this.GetType(), "apriPopupAnteprima", "apriPopupAnteprima();", true);
                            }
                            else
                            {
                                Templates template2 = ProfilazioneDocManager.getTemplateById(tipologiaAtto.systemId,this);
                                //template2.DESCRIZIONE = this.unicaTipoAtto.Text;
                                Session.Add("template", template2);
                                ClientScript.RegisterStartupScript(this.GetType(), "apriPopupAnteprima", "apriPopupAnteprima();", true);
                            }
                        }
                    }
                }
                else
                {
                    Session.Add("isUniqueTipoAtto", "false");
                    this.isUniqueTipoAtto = false;*/
                    for (int i = 0; i < listaTipologiaAtto.Length; i++)
                    {
                        ddl.Items.Add(listaTipologiaAtto[i].descrizione);
                        ddl.Items[i + 1].Value = listaTipologiaAtto[i].systemId;
                    }
               // }
            }
        }

        /// <summary>
        /// Reperimento controllo acldocumento
        /// </summary>
        /// <returns></returns>
        protected DocsPAWA.documento.AclDocumento GetControlAclDocumento()
        {
            return (DocsPAWA.documento.AclDocumento)this.FindControl("aclDocumento");
        }

        private void ddl_tipoAtto_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.btn_CampiPersonalizzati.Enabled = true;
           
            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();
            if (schedaDocumento == null)
                schedaDocumento = new SchedaDocumento();

            Session.Remove("template");
            TipologiaAtto tipologiaAtto = new DocsPAWA.DocsPaWR.TipologiaAtto();
            tipologiaAtto.systemId = this.ddl_tipoAtto.SelectedItem.Value;
            tipologiaAtto.descrizione = this.ddl_tipoAtto.SelectedItem.Text;
            schedaDocumento.tipologiaAtto = tipologiaAtto;
            schedaDocumento.daAggiornareTipoAtto = true;

            string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
            if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
            {
                //Templates template = ProfilazioneDocManager.getTemplateDettagli(schedaDocumento.docNumber,this);
                //if (template != null && template.SYSTEM_ID != 0)
                if(!string.IsNullOrEmpty(schedaDocumento.docNumber))
                {
                    Templates template = ProfilazioneDocManager.getTemplateDettagli(schedaDocumento.docNumber, this);
                    if (template != null && template.SYSTEM_ID != 0)
                    {
                        //Se la tipologia documento è privato allora il documento deve essere privato
                        if (template.PRIVATO != null && template.PRIVATO == "1")
                            schedaDocumento.privato = "1";
                        else
                            schedaDocumento.privato = "0";

                        if (Session["tipoProto"].Equals("A"))
                            this.GetProtocolloManager().setDatiProtocolloIngresso(schedaDocumento);
                        else
                            this.GetProtocolloManager().setDatiProtocolloUscita(schedaDocumento);

                        schedaDocumento.template = template;
                        DocumentManager.setDocumentoSelezionato(schedaDocumento);
                        DocumentManager.setDocumentoInLavorazione(schedaDocumento);
                        Session.Add("template", template);
                        if (template.ELENCO_OGGETTI.Length > 0)
                            ClientScript.RegisterStartupScript(this.GetType(), "apriPopupAnteprima", "apriPopupAnteprima();", true);
                    }
                }
                else
                {
                    Templates template = ProfilazioneDocManager.getTemplateById(this.ddl_tipoAtto.SelectedItem.Value, this);
                    schedaDocumento.template = template;
                    DocumentManager.setDocumentoSelezionato(schedaDocumento);
                    DocumentManager.setDocumentoInLavorazione(schedaDocumento);
                    Session.Add("template", template);
                    if (template != null && template.ELENCO_OGGETTI.Length > 0)
                        ClientScript.RegisterStartupScript(this.GetType(), "apriPopupAnteprima", "apriPopupAnteprima();", true);
                }                
            }
        }

        private bool controllaStatoFinale()
        {
            DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"];
            for (int i = 0; i < dg.STATI.Length; i++)
            {
                Stato st = (DocsPAWA.DocsPaWR.Stato)dg.STATI[i];
                if (st.SYSTEM_ID.ToString() == ddl_statiSuccessivi.SelectedValue && st.STATO_FINALE)
                    return true;
            }
            return false;
        }

        private void effettuaTrasmissione(DocsPAWA.DocsPaWR.ModelloTrasmissione modello, string idStato)
        {
            Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();
            DocsPaWebService wws = new DocsPaWebService();
            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();

            //Parametri della trasmissione
            trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;
            if (modello != null)
                if (string.IsNullOrEmpty(modello.NO_NOTIFY))
                    trasmissione.NO_NOTIFY = "0";
                else
                    trasmissione.NO_NOTIFY = modello.NO_NOTIFY;
            if (modello.CHA_TIPO_OGGETTO == "D")
            {
                trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
                trasmissione.infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
            }
            else
            {
                trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
                trasmissione.infoFascicolo = FascicoliManager.getInfoFascicolo(this);
            }
            trasmissione.utente = UserManager.getUtente(this);
            trasmissione.ruolo = UserManager.getRuolo(this);

            //Parametri delle trasmissioni singole
            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
            {
                RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                    Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this, mittDest.VAR_COD_RUBRICA, AddressbookTipoUtente.INTERNO);
                    RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                    
                    //Andrea - try - catch
                    try
                    {
                        trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA);
                    }
                    catch (ExceptionTrasmissioni e)
                    {
                        listaExceptionTrasmissioni.Add(e.Messaggio);
                    }
                    //End Andrea
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

            //Modifica per cessione diritti

            if (this.cessioneDirittiAbilitato(trasmissione, modello))
            {
                bool aperturaPopUpSceltaNuovoProprietario = false;

                // verifica se esistono ruoli tra i destinatari
                this.verificaRuoliDestInTrasmissione(trasmissione);

                switch (this.numeroRuoliDestInTrasmissione)
                {
                    case 0:
                        // non ci sono ruoli tra i destinatari! avvisa...
                        this.inviaMsgNoRuoli();
                        return;
                        break;

                    case 1:
                        // ce n'è 1, verifica se un solo utente del ruolo ha la notifica...
                        this.utentiConNotifica(trasmissione);
                        if (this.numeroUtentiConNotifica > 1)
                            aperturaPopUpSceltaNuovoProprietario = true;
                        else
                        {
                            // 1 solo utente con notifica, il sistema ha già memorizzato il suo id_people...
                            trasmissione.cessione.idPeopleNewPropr = this.idPeopleNewOwner;
                            trasmissione.cessione.idRuoloNewPropr = ((DocsPAWA.DocsPaWR.Ruolo)trasmissione.trasmissioniSingole[0].corrispondenteInterno).idGruppo;

                            modello.CEDE_DIRITTI = "1";
                            modello.ID_PEOPLE_NEW_OWNER = trasmissione.cessione.idPeopleNewPropr;
                            modello.ID_GROUP_NEW_OWNER = trasmissione.cessione.idRuoloNewPropr;
                        }
                        break;

                    default:
                        // ce ne sono + di 1, quindi ne fa scegliere uno...                                    
                        aperturaPopUpSceltaNuovoProprietario = true;
                        break;
                }
            }

            //Nuovo metodo saveExecuteTrasm
            TrasmManager.saveExecuteTrasm(this, trasmissione, infoUtente);
            //trasmissione = TrasmManager.saveTrasm(this, trasmissione);
            //TrasmManager.executeTrasm(this, trasmissione);

            wws.salvaStoricoTrasmDiagrammi(trasmissione.systemId, schedaDocumento.docNumber, idStato);
        }
        #endregion

        private DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Corrispondente corr, DocsPAWA.DocsPaWR.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza)
        {
            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                {
                    TrasmissioneSingola ts = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
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
            TrasmissioneSingola trasmissioneSingola = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;
            //Imposto la data di scadenza
            if (scadenza > 0)
            {
                string dataScadenza = "";
                System.DateTime data = System.DateTime.Now.AddDays(scadenza);
                dataScadenza = data.Day + "/" + data.Month + "/" + data.Year;
                trasmissioneSingola.dataScadenza = dataScadenza;
            }

            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPAWA.DocsPaWR.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
                Corrispondente[] listaUtenti = queryUtenti(corr);
                
                //Andrea
                if (listaUtenti.Length == 0)
                {
                    trasmissioneSingola = null;
                    throw new ExceptionTrasmissioni("Non è presente alcun utente per la Trasmissione al ruolo: "
                                                    + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + ".");
                }
                //End Andrea
                else
                {
                    //ciclo per utenti se dest è gruppo o ruolo
                    for (int i = 0; i < listaUtenti.Length; i++)
                    {
                        TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                        trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)listaUtenti[i];
                        trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    }
                }
            }

            if (corr is DocsPAWA.DocsPaWR.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
                TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)corr;

                //Andrea
                if (trasmissioneUtente.utente == null)
                {
                    throw new ExceptionTrasmissioni("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + " è inesistente.");
                }
                //End Andrea
                else
                    trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
            }

            if (corr is DocsPAWA.DocsPaWR.UnitaOrganizzativa)
            {
                UnitaOrganizzativa theUo = (DocsPAWA.DocsPaWR.UnitaOrganizzativa)corr;
                AddressbookQueryCorrispondenteAutorizzato qca = new AddressbookQueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                qca.ruolo = UserManager.getRuolo();

                Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati(this, qca, theUo);

                //Andrea
                if (ruoli.Length == 0)
                {
                    throw new ExceptionTrasmissioni("Manca un ruolo di riferimento per l Ufficio: "
                                                        + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                        + ".");
                }
                //End Andrea
                else
                {
                    foreach (DocsPAWA.DocsPaWR.Ruolo r in ruoli)
                        trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm, scadenza);
                }
                return trasmissione;
            }

            if (trasmissioneSingola != null)
                trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);

            return trasmissione;
        }

        private DocsPAWA.DocsPaWR.Corrispondente[] queryUtenti(DocsPAWA.DocsPaWR.Corrispondente corr)
        {
            //costruzione oggetto queryCorrispondente
            AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();
            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;
            qco.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
            qco.fineValidita = true;
            //corrispondenti interni
            qco.tipoUtente = AddressbookTipoUtente.INTERNO;
            return UserManager.getListaCorrispondenti(this, qco);
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            DocsPaWebService wws = new DocsPaWebService();

            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();

            if (Session["dictionaryCorrispondente"] != null)
            {
                dic_Corr = (Dictionary<string, Corrispondente>)Session["dictionaryCorrispondente"];
                if (dic_Corr != null && dic_Corr.ContainsKey("protIngr") && dic_Corr["protIngr"] != null)
                {
                    txtCodMittente.Text = dic_Corr["protIngr"].codiceRubrica;
                    this.txtDescrMittente.Text = dic_Corr["protIngr"].descrizione;
                    //this.hd_systemIdMit_Est.Value = dic_Corr["protIngr"].systemId;
                }
                dic_Corr.Remove("protIngr");
                Session.Add("dictionaryCorrispondente", dic_Corr);
            }

            //Uso una textBox di appoggio (di dimensione zero) per verificare se effettivamente 
            //la textBox oggetto (dello user control) non è vuota!
            textOggetto.Text = ctrl_oggetto.oggetto_text;
            if (schedaDocumento != null && isFileUploadEnable)
            {
                if (schedaDocumento.documenti != null && schedaDocumento.documenti[0].path != "")
                {
                    txtCountDocument.Value = schedaDocumento.documenti.Length.ToString();
                    //this.RegisterClientScript("Protocollato acquisito", " SetStatoProtocollatoAcquisito();");
                    //this.RegisterClientScript("Protocollato acquisito", "alert('Protocollato acquisito? ' + document.frmProtIngresso.txtStatoDocumento.value);");
                    
                }
                if (schedaDocumento.allegati != null)
                    txtCountAttachment.Value = schedaDocumento.allegati.Length.ToString();
            }
            if (this.ContainsRegistriAperti() && (this._userAuthorized || _userAuthorizedUscita))
            // if (this.ContainsRegistriAperti())
            {
                // Aggiornamento pulsanti bottoniera
                this.RefreshButtonsEnabled();

                /// Aggiornamento descrizione pulsanti di acquisizione immagini
                /// con il numero di documenti scannerizzati
                this.RefreshAcquireButtonsDescription();

                // Aggiornamento selezioni delle UO
                this.GetControlSmistaUO().RefreshSelections(this.GetSessionSelectedUO());
            }

            if (!this.IsPostBack)
            {
                // Caricamento dati dettaglio nota
                //this.dettaglioNota.AttatchPulsanteConferma(this.btnProtocolla.ClientID);
                this.dettaglioNota.Fetch();
            }

            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_NOTE_PROT_SEMPL) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_NOTE_PROT_SEMPL).Equals("1"))
            {
                if (schedaDocumento != null)
                {
                    if (schedaDocumento.docNumber != null)
                    {
                        this.dettaglioNota.Fetch();
                    }
                }
            }

            DocsPAWA.DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
            if (fasc != null)
            {
                string codice = FascicoliManager.getCodiceFascRapida(this);
                string descr = FascicoliManager.getDescrizioneFascRapida(this);
                if (!string.IsNullOrEmpty(descr))
                {
                    this.txt_CodFascicolo.Text = codice;
                    this.txt_DescFascicolo.Text = descr;
                }
                else
                {
                    this.txt_CodFascicolo.Text = fasc.codice;
                    this.txt_DescFascicolo.Text = fasc.descrizione;
                }
                //FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
            }

            //Richiamo il metodo verificaCampiPersonalizzati per abilitare o meno il pannello corrispondente
            string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
            if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
            {
                //ProfilazioneDocManager.verificaCampiPersonalizzati(this, this.GetProtocolloManager().GetDocumentoCorrente());
                //DIAGRAMMI DI STATO
                if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                {
                    //Controllo se lo stato è uno stato automatico
                    if (Session["statoAutomatico"] != null)
                    {
                        if ((string)Session["statoAutomatico"] == "SI")
                            ClientScript.RegisterStartupScript(this.GetType(), "confirm", "Confirm('Lo stato selezionato è uno stato automatico.\\nConfermi il salvataggio ?');", true);
                        //Utilities.MessageBox.Message.MessageBox1.Confirm("Lo stato selezionato è uno stato automatico.\\nConfermi il salvataggio ?");
                    }

                    //Controllo se lo stato è uno stato finale
                    if (Session["docSolaLettura"] != null)
                    {
                        if ((string)Session["docSolaLettura"] == "SI")
                            ClientScript.RegisterStartupScript(this.GetType(), "confirm", "Confirm('Si sta portando il documento in uno stato finale.\\nIl documento diventerà di sola lettura.\\nConfermi il salvataggio ?');", true);
                        //MessageBox.Confirm("Si sta portando il documento in uno stato finale.\\nIl documento diventerà di sola lettura.\\nConfermi il salvataggio ?");
                    }

                    //Verifico se esiste un diagramma di stato associato al tipo di documento
                    DiagrammaStato dg = null;
                    if (schedaDocumento != null && schedaDocumento.tipologiaAtto != null)
                        dg = DiagrammiManager.getDgByIdTipoDoc(schedaDocumento.tipologiaAtto.systemId, (UserManager.getInfoUtente(this)).idAmministrazione,this);
                    else
                        dg = DiagrammiManager.getDgByIdTipoDoc(ddl_tipoAtto.SelectedValue, (UserManager.getInfoUtente(this)).idAmministrazione,this);
                    Session.Add("DiagrammaSelezionato", dg);
                    if (ddl_tipoAtto.SelectedValue != "" && dg != null)
                    {
                        pnl_DiagrammiStato.Visible = true;
                        //Controllo se il documento è nuovo o meno
                        if (schedaDocumento != null && schedaDocumento.docNumber != null)
                        {
                            Stato stato = DiagrammiManager.getStatoDoc(schedaDocumento.docNumber,this);
                            //Controllo che il doc non è gia' in un determintato stato del diagramma suddetto "DPA_DIAGRAMMI_DOC"
                            if (stato != null)
                            {
                                lbl_statoAttuale.Text = stato.DESCRIZIONE;
                                lbl_statoAttuale.Visible = true;
                                if (stato.STATO_FINALE)
                                    ddl_statiSuccessivi.Enabled = false;
                                else
                                    popolaComboBoxStatiSuccessivi(stato, dg);
                            }
                            //Controllo che il doc non è in uno stato di un diagramma non piu' attivo "DPA_DIAGRAMMI_STO" (in questo caso sicuro è uno stato finale)
                            else
                            {
                                string st = DiagrammiManager.getStatoDocStorico(schedaDocumento.docNumber,this);
                                lbl_statoAttuale.Text = st;
                                lbl_statoAttuale.Visible = true;
                                //ddl_statiSuccessivi.Enabled = false;
                                popolaComboBoxStatiSuccessivi(null, dg);
                            }

                            //Imposto la data di scadenza per un doc non nuovo
                            if (schedaDocumento.dataScadenza != null && schedaDocumento.dataScadenza != "" && schedaDocumento.dataScadenza != "0" && schedaDocumento.dataScadenza.IndexOf("1900") == -1)
                            {
                                pnl_DataScadenza.Visible = true;
                                txt_dataScadenza.Text = schedaDocumento.dataScadenza;
                                txt_dataScadenza.Enabled = false;
                            }
                        }
                        else
                        {
                            //Documento nuovo popolo la combo box degli stati con gli stati iniziali del diagramma corrispondente alla tipologia di documento
                            //Al seguente metodo se come parametro gli viene passato "null", popola la combo box con gli stati iniziali, altrimenti se gli viene
                            //passato uno stato, popola la combo box con gli stati successivi possibili
                            popolaComboBoxStatiSuccessivi(null, dg);

                            //Imposto la data di scadenza per un doc nuovo
                            if (Session["template"] != null)
                            {
                                Templates template = (Templates)Session["template"];
                                if (template.SCADENZA != null && template.SCADENZA != "" && template.SCADENZA != "0")
                                {
                                    try
                                    {
                                        DateTime dataOdierna = System.DateTime.Now;
                                        int scadenza = Convert.ToInt32(template.SCADENZA);
                                        DateTime dataCalcolata = dataOdierna.AddDays(scadenza);
                                        pnl_DataScadenza.Visible = true;
                                        if (schedaDocumento.dataScadenza == null || schedaDocumento.dataScadenza == "")
                                        {
                                            txt_dataScadenza.Text = Utils.formatDataDocsPa(dataCalcolata);
                                            schedaDocumento.dataScadenza = Utils.formatDataDocsPa(dataCalcolata);
                                        }
                                    }
                                    catch (Exception ex) { }
                                }
                                else
                                {
                                    txt_dataScadenza.Text = "";
                                    pnl_DataScadenza.Visible = false;
                                    //schedaDocumento.dataScadenza = "";
                                }
                            }
                        }
                        pnl_DiagrammiStato.Visible = false;
                    }
                    else
                    {
                        pnl_DiagrammiStato.Visible = false;
                        txt_dataScadenza.Text = "";
                        pnl_DataScadenza.Visible = false;
                    }
                    if (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)) != null && ((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45")
                        ddl_statiSuccessivi.Enabled = false;
                }
                //FINE DIAGRAMMI DI STATO	
            }

            if (CheckDestinatariInterni())
            {
                isInterno.Value = "true";
                if (appoIdAmm.Value == string.Empty)
                {
                    if (regSelezionato != null)
                    {
                        appoIdAmm.Value = regSelezionato.idAmministrazione;
                    }
                    else
                    {
                        regSelezionato = UserManager.getRegistroSelezionato(this);
                        appoIdAmm.Value = regSelezionato.idAmministrazione;
                    }

                    if (string.IsNullOrEmpty(appoIdMod.Value))
                    {
                        appoIdMod.Value = "null";
                    }

                    if (wws.ereditaVisibilita(appoIdAmm.Value, appoIdMod.Value))
                    {
                        this.abilitaModaleVis.Value = "true";
                        //ClientScript.RegisterStartupScript(this.GetType(), "openAvvisoVisibilita", "AvvisoVisibilita();", true);
                    }
                    else
                    {
                        this.abilitaModaleVis.Value = "false";
                    }
                }
            }
            else
            {
                isInterno.Value = "false";
            }

            if (tipoProto == "A" && DocumentManager.isEnableMittentiMultipli())
            {
                panel_DettaglioMittenti.Visible = true;
                setListBoxMittentiMultipli();
            }
            newRubricaVeloce(tipoProto);


            //Andrea
            if (Session["MessError"] != null)
            {
                messError = Session["MessError"].ToString();
                //Response.Write("<script language=\"javascript\">alert('Trasmissioni avvenute con successo per tutti i destinatari ad eccezione delle seguenti: \\n" + messError + "');</script>");
                Response.Write("<script language=\"javascript\">alert('Trasmissioni con esito negativo: \\n" + messError + "\\n');</script>");
                Session.Remove("MessError");
            }
            //End Andrea

        }

        private void popolaComboBoxStatiSuccessivi(DocsPAWA.DocsPaWR.Stato stato, DocsPAWA.DocsPaWR.DiagrammaStato diagramma)
        {
            //Inizializzazione
            ddl_statiSuccessivi.Items.Clear();
            ListItem itemEmpty = new ListItem();
            ddl_statiSuccessivi.Items.Add(itemEmpty);

            //Popola la combo con gli stati iniziali del diagramma
            if (stato == null)
            {
                lbl_statoAttuale.Text = "";
                for (int i = 0; i < diagramma.STATI.Length; i++)
                {
                    Stato st = (DocsPAWA.DocsPaWR.Stato)diagramma.STATI[i];
                    if (st.STATO_INIZIALE)
                    {
                        ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                        ddl_statiSuccessivi.Items.Add(item);
                    }
                }
                if (ddl_statiSuccessivi.Items.Count == 2)
                    ddl_statiSuccessivi.SelectedIndex = 1;
            }
            //Popola la combo con i possibili stati, successivi a quello passato
            else
            {
                for (int i = 0; i < diagramma.PASSI.Length; i++)
                {
                    Passo step = (DocsPAWA.DocsPaWR.Passo)diagramma.PASSI[i];
                    if (step.STATO_PADRE.SYSTEM_ID == stato.SYSTEM_ID)
                    {
                        for (int j = 0; j < step.SUCCESSIVI.Length; j++)
                        {
                            Stato st = (DocsPAWA.DocsPaWR.Stato)step.SUCCESSIVI[j];
                            ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                            ddl_statiSuccessivi.Items.Add(item);
                        }
                    }
                }
            }
        }

        #endregion

        #region Gestione eventi controlli UI

        /// <summary>
        /// Gestione impostazione pulsante di protocollazione come pulsante di default.
        /// E' necessario impostare un handler di evento javascript per ogni controllo 
        /// editabile sul quale ci si può posizionare.
        /// </summary>
        private void SetDefaultButton(string tipoProto)
        {
            DocsPAWA.Utils.DefaultButton(this, ref this.cboRegistriDisponibili, ref this.btnProtocolla);
            ctrl_oggetto.DefButton_Ogg(ref this.btnProtocolla);
            if (isFascRapidaRequired)
                DocsPAWA.Utils.DefaultButton(this, ref this.txt_CodFascicolo, ref this.btnProtocolla);

            if (tipoProto.Equals("A"))
                SetDefaultButtonIngresso();
            else
            {
                if (tipoProto.Equals("P"))
                {
                    SetDefaultButtonUscita();
                }
            }

            this.GetControlSmistaUO().SetDefaultButtonJSHandler(this.btnProtocolla);
        }

        /// <summary>
        /// Setta i default button per i campi della protocollazione in ingresso
        /// </summary>
        private void SetDefaultButtonIngresso()
        {
            DocsPAWA.Utils.DefaultButton(this, ref this.txtCodMittente, ref this.btnProtocolla);
            if (string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) || !(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
            {
                DocsPAWA.Utils.DefaultButton(this, ref this.txtDescrMittente, ref this.btnProtocolla);
            }
            DocsPAWA.Utils.DefaultButton(this, ref this.txtDescrProtMittente, ref this.btnProtocolla);

            this.GetCalendarControl("txtDataArrivoProt").SetDefaultPageButton(this.btnProtocolla);
            this.GetCalendarControl("txtDataProtocolloMittente").SetDefaultPageButton(this.btnProtocolla);

            //            DocsPAWA.Utils.DefaultButton(this, ref this.txtDataProtocolloMittente, ref this.btnProtocolla);
            //            DocsPAWA.Utils.DefaultButton(this, ref this.txtDataArrivoProt, ref this.btnProtocolla);

            if (isFascRapidaRequired)
                DocsPAWA.Utils.DefaultButton(this, ref this.txt_CodFascicolo, ref this.btnProtocolla);

            if (isTipologiaDocumentoRequired)
                DocsPAWA.Utils.DefaultButton(this, ref ddl_tipoAtto, ref this.btnProtocolla);
        }

        /// <summary>
        /// Setta i default button per i campi della protocollazione in uscita
        /// </summary>
        private void SetDefaultButtonUscita()
        {
            DocsPAWA.Utils.DefaultButton(this, ref this.txtCodDest, ref this.btnProtocolla);
            if (string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) || !(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
            {
                DocsPAWA.Utils.DefaultButton(this, ref this.txtDescrDest, ref this.btnProtocolla);
            }
            if (isFascRapidaRequired)
                DocsPAWA.Utils.DefaultButton(this, ref this.txt_CodFascicolo, ref this.btnProtocolla);

            if (isTipologiaDocumentoRequired)
                DocsPAWA.Utils.DefaultButton(this, ref ddl_tipoAtto, ref this.btnProtocolla);
        }


        private void cboRegistriDisponibili_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.SetRegistroCorrente(this.cboRegistriDisponibili.SelectedValue);
        }

        #region Gestione eventi pulsanti

        /// <summary>
        /// Reperimento delle UO selezionate per lo smistamento 
        /// ed impostazione in sessione
        /// </summary>
        private void SetSessionSelectedUO()
        {
            Session["Protocollazione.SelectedUO"] = this.GetControlSmistaUO().GetSelectedUO();
        }

        private DocsPAWA.DocsPaWR.UOSmistamento[] GetSessionSelectedUO()
        {
            return Session["Protocollazione.SelectedUO"] as DocsPAWA.DocsPaWR.UOSmistamento[];
        }

        /// <summary>
        /// Rimozione dalla sessione delle UO selezionate per lo smistamento
        /// </summary>
        private void ReleaseSessionSelectedUO()
        {
            Session.Remove("Protocollazione.SelectedUO");
        }

        private void btnShowRubrica_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Caricamento dati del corrispondente nella UI
            this.FillDatiCorrispondente();

            //this.SetControlFocus("txtCodMittente");
        }

        private void btnShowRubricaMittUsc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Caricamento dati del corrispondente nella UI
            this.FillDatiCorrispondenteMittenteInUscita();

            //this.SetControlFocus("txtCodMittUsc");
        }

        private void btnShowRubricaDest_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Caricamento dati del corrispondente nella UI
            this.FillDatiDestinatari();

            //this.SetControlFocus("txtCodDest");
        }

        private void btn_RubrOgget_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

            // Caricamento dati del corrispondente nella UI
            this.FillDatiOggetto();

            //this.ctrl_oggetto.oggetto_SetControlFocus();
        }


        private void btnShowCalDataProtMittente_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //this.SetControlFocus("txtDataProtocolloMittente");
        }

        private void btnShowCalDataArrivo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //this.SetControlFocus("txtDataArrivoProt");
        }

        private void btnProtocolla_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Esecuzione della protocollazione del documento
            this.PerformActionProtocollaDocumento();
        }

        private void btnNuovoProtocollo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Creazione di un nuovo documento
            this.PerformActionNuovoDocumento(false, tipoProto);
        }

        private void btnRiproponi_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Creazione di un nuovo documento mantenendo
            // i dati inseriti (riproposizione)
            this.PerformActionNuovoDocumento(true, tipoProto);
        }

        private void btnClearData_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Cancellazione dei dati immessi
            tipoProto = Session["tipoProto"].ToString();
            this.PerformActionClearData(tipoProto);
        }


        /// <summary>
        /// Deallocazione risorse in sessione utilizzate
        /// </summary>
        private void UnloadData()
        {
            this.GetProtocolloManager().ReleaseDocumentoCorrente();
            this.GetRegistroManager().ReleaseRegistroCorrente();
            Session.Remove("template");
        }

        private void btnClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.UnloadData();

            this.RegisterClientScript("CloseWindow", "CloseWindow();");
        }

        #endregion

        #endregion

        private bool checkFasc()
        {
            bool retValue = true;
            if (this.isFascRapidaRequired)
            {
                if (!this.txt_CodFascicolo.Text.Equals(""))
                {
                    DocsPAWA.DocsPaWR.Fascicolo fasc = this.getFascicolo();
                    if (fasc == null)
                        retValue = false;
                }
            }
            //controllo sulla lunghezza dell'oggetto (max 2000 car.)
            //if (this.txtOggetto.Text.Length > 2000)
            //{
            //    string msg = "La lunghezza massima del campo oggetto non deve superare i 2000 caratteri.";
            //    string s = "<SCRIPT language='javascript'>document.getElementById('" + txtOggetto.ID + "').focus() </SCRIPT>";
            //    RegisterStartupScript("max_ogg", s);
            //}
            return retValue;
        }

        #region Gestione azioni pulsanti

        /// <summary>
        /// Azione di protocollazione del documento corrente
        /// </summary>
        private bool PerformActionProtocollaDocumento()
        {
            bool retValue = false;
            bool profVerifica = false;
            string errorMessage = null;
            
            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();
            SettaMittenteDefault(true);
            Templates tem = (Templates)Session["template"];

            ArrayList validationItems = null;
            string firstInvalidControlID = string.Empty;
            string validationMessage = string.Empty;
            bool esisteDuplicato = false;

            // Validazione dati immessi
            if (this.IsValidRequiredData(out validationItems, out firstInvalidControlID) && this.checkFasc())
            {
                // Aggiornamento dati immessi nella UI nell'oggetto SchedaDocumento corrente

                if (tipoProto.Equals("A"))
                {
                    this.RefreshSchedaDocumentoFromUI();
                    esisteDuplicato = this.ContainsDocumento();
                }
                else
                    this.RefreshSchedaDocumentoUscitaFromUI();

                schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();
                DocumentManager.setDocumentoInLavorazione(schedaDocumento);

               
                this.dettaglioNota.Save();
                if (tipoProto.Equals("A"))
                    this.GetProtocolloManager().setDatiProtocolloIngresso(schedaDocumento);
                else
                    this.GetProtocolloManager().setDatiProtocolloUscita(schedaDocumento);

                //PROFILAZIONE DINAMICA
                string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
                if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
                {
                    //Salvataggio dei campi della profilazione dinamica
                    if (Session["template"] != null)
                    {
                        if (!ProfilazioneDocManager.verificaCampiObbligatori((DocsPAWA.DocsPaWR.Templates)Session["template"]))
                        {
                            //wws.salvaInserimentoUtenteProfDim(UserManager.getInfoUtente(this), (Templates)Session["template"], schedaDocumento.docNumber, "");
                            schedaDocumento.template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                            // Session["template"] = wws.getTemplate((UserManager.getInfoUtente(this)).idAmministrazione, ddl_tipoAtto.SelectedItem.Text, schedaDocumento.docNumber);
                            //wws.getTemplate((UserManager.getInfoUtente(this)).idAmministrazione, ddl_tipoAtto.SelectedItem.Text, schedaDocumento.docNumber);
                            profVerifica = true;
                        }
                        else
                        {
                            errorMessage = "Ci sono dei campi obbligatori relativi al tipo di documento selezionato !";
                            profVerifica = false;
                        }
                    }
                    else
                    {
                        profVerifica = true;
                    }
                }
                else
                {
                    profVerifica = true;
                }
                //FINE PROFILAZIONE DINAMICA
 
                // rimuovo il documento in lavorazione dalla sessione che ho utilizzato per la tipologia doc
                //DocumentManager.removeDocumentoInLavorazione();

                //memorizzo il codice di classificazione nella scheda documento per recuperarlo nel backend
                //per successive elaborazioni (ad es. trasmissione verso WSPIA)
                schedaDocumento.codiceFascicolo = txt_CodFascicolo.Text;

                if ((!noSubmitProtCorrente() && esisteDuplicato && tipoProto.Equals("A")) || !tipoProto.Equals("A"))
                {
                    if (!profVerifica)
                    {
                        this.RegisterClientScript("ErroreProfilazione", "alert('" + errorMessage + "');");

                    }
                    else
                    {
                        retValue = this.ProtocollaDocumentoCorrente(out errorMessage);

                        if (!retValue)
                        {
                            // Abilitazione pulsante protocollazione che, in caso
                            // di errore, permette di rieseguire l'operazione
                            // con gli stessi dati visualizzati
                            this.RegisterClientScript("EnableButtonProtocollazione", "EnableButtonProtocollazione(true);");

                            validationMessage = "Errore nella protocollazione del documento: \\n" + errorMessage;
                            this.RegisterClientScript("ErrorOnProtocollazione", "alert('" + validationMessage + "');");
                        }
                    }
                }
            }
            else
            {
                // Disabilitazione pulsante protocollazione
                this.RegisterClientScript("EnableButtonProtocollazione", "EnableButtonProtocollazione(false);");

                foreach (string item in validationItems)
                {
                    if (validationMessage != string.Empty)
                        validationMessage += @"\n";

                    validationMessage += " - " + item;
                }

                if (validationMessage != string.Empty)
                    validationMessage = "Sono state rilevate le seguenti incongruenze: " +
                        @"\n" + @"\n" + validationMessage;

                this.RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");

                //if (firstInvalidControlID != string.Empty)
                // impostazione del focus sul primo controllo non valido
                //this.SetControlFocus(firstInvalidControlID);
            } 

            //Aggiunto x acquisizione file
            /*SchedaDocumento dp = this.GetProtocolloManager().GetDocumentoCorrente();
            DocumentManager.setDocumentoSelezionato(dp);
            //Aggiunti per acquisizione Doc
            if (dp.documenti[0].versionId == null) dp.documenti[0].versionId = string.Empty;
            FileManager.setSelectedFile(this, dp.documenti[0]);*/
            
            return retValue;
        }

        
        private void btnAcquireDocument_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            
            SchedaDocumento dp = this.GetProtocolloManager().GetDocumentoCorrente();
            
            DocumentManager.setDocumentoSelezionato(dp);
            
            //if (dp.documenti[0].versionId == null) dp.documenti[0].versionId = string.Empty;
            int l = dp.documenti.Length;
            FileManager.setSelectedFile(this, dp.documenti[l-1]);
            
            
            ClientScript.RegisterStartupScript(this.GetType(), "acquireDocument", "AcquireDocument();", true);
            
        }

        private void btnAcquireAttach_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            SchedaDocumento dp = this.GetProtocolloManager().GetDocumentoCorrente();
            DocumentManager.setDocumentoSelezionato(dp);
            DocsPAWA.DocsPaWR.Allegato[] allegati = DocumentManager.getAllegati(dp, string.Empty);
            //Aggiunti per acquisizione Allegato
            int l= allegati.Length;
            //if (l == 1) FileManager.setSelectedFile(this, allegati[l]);
            FileManager.setSelectedFile(this, allegati[l-1]);

            ClientScript.RegisterStartupScript(this.GetType(), "AcquireAttach", "AcquireAttach();", true);
        }

        /// <summary>
        /// Azione di inserimento di un nuovo documento
        /// </summary>
        /// <param name="leaveCurrentData">se true, deve mantenere i dati correntemente visualizzati (funzione riproponi)</param>
        private void PerformActionNuovoDocumento(bool leaveCurrentData, string tipoProto)
        {
            if (this.GetStatoDocumento() != StatoDocumentoEnum.NonProtocollato)
            {
                //				// Smistamento del documento (solo se protocollato)
                //				DocsPaWR.EsitoSmistamentoDocumento[] retValue=
                //					this.GetControlSmistaUO().SmistaDocumento(this.GetSessionSelectedUO());
                //
                //				// Impostazione dello stato del documento come "ProtocollatoSmistato"
                //				this.SetStatoDocumento(StatoDocumentoEnum.ProtocollatoSmistato);
            }

            // Creazione del nuovo documento
            this.CreateNewDocument(leaveCurrentData, tipoProto);
        }

        private void PerformActionClearData(string tipoProto)
        {
            this.ClearDocumentData(tipoProto);

            //this.ctrl_oggetto.oggetto_SetControlFocus();
        }

        #endregion

        #region Metodi per il refresh della UI

        /// <summary>
        /// Aggiornamento descrizione pulsanti di acquisizione immagini
        /// con il numero di documenti scannerizzati
        /// </summary>
        private void RefreshAcquireButtonsDescription()
        {
            string count = this.txtCountDocument.Value;
            if (count == string.Empty)
                count = "0";
            this.lblDescrizioneDocAcquisiti.Text = "N° documenti: " + count;

            count = this.txtCountAttachment.Value;
            if (count == string.Empty)
                count = "0";
            this.lblDescrizioneAllAcquisiti.Text = "N° allegati: " + count;
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

        private void RefreshButtonsEnabled()
        {
            string Tema = GetCssAmministrazione();
            string pageTheme = string.Empty;
            if (Tema != null && !Tema.Equals(""))
            {
                string[] realTema = Tema.Split('^');
                pageTheme = realTema[0];
            }
            else
                pageTheme = "TemaRosso";
            
            // Gestine abilitazione / disabilitazione pulsante protocollazione
            this.RegisterClientScript("EnableButtonProtocollazione", "EnableButtonProtocollazione(false);");

            this.btnNuovoProtocollo.Enabled = false;
            this.btnRiproponi.Enabled = false;
            this.btnClearData.Enabled = false;

            this.EnableImageButton(this.btnAcquireDocument, false, "../App_Themes/" + pageTheme + "/btn_PS_acquisisci.gif", "../App_Themes/ImgComuni/btn_PS_acquisisci_dis.gif");
            this.EnableImageButton(this.btnAcquireAttach, false, "../App_Themes/" + pageTheme + "/btn_PS_acquisisci_alleg.gif", "../App_Themes/ImgComuni/btn_PS_acquisisci_alleg_dis.gif");
            this.EnableImageButton(this.btnClose, false, "../App_Themes/" + pageTheme + "/btn_PS_chiudi.gif", "../App_Themes/ImgComuni/btn_PS_chiudi_dis.gif");

            StatoDocumentoEnum statoDocumento = this.GetStatoDocumento();

            bool isAcqObbl = this.IsAcquisizioneDocumentiObbligatoria();
            int countDocument = this.GetCountDocument();

            switch (statoDocumento.ToString())
            {
                case "NonProtocollato":
                    if (tipoProto.Equals("A"))
                    {
                        this.RegisterClientScript("RefreshButtonProtocollaEnabled", "RefreshButtonProtocollaEnabled();");
                    }
                    else
                    {
                        this.RegisterClientScript("RefreshButtonProtocollaUscitaEnabled", "RefreshButtonProtocollaUscitaEnabled();");
                    }
                    this.EnableImageButton(this.btnClose, true, "../App_Themes/" + pageTheme + "/btn_PS_chiudi.gif", "../App_Themes/ImgComuni/btn_PS_chiudi_dis.gif");
                    this.btnClearData.Enabled = true;

                    break;

                case "Protocollato":
                    if (isAcqObbl)
                    {
                        this.btnNuovoProtocollo.Enabled = (countDocument > 0);
                        this.btnRiproponi.Enabled = (countDocument > 0);
                        this.EnableImageButton(this.btnClose, (countDocument > 0), "../App_Themes/" + pageTheme + "/btn_PS_chiudi.gif", "../App_Themes/ImgComuni/btn_PS_chiudi_dis.gif");
                    }
                    else
                    {
                        this.btnNuovoProtocollo.Enabled = true;
                        this.btnRiproponi.Enabled = true;
                        this.EnableImageButton(this.btnClose, true, "../App_Themes/" + pageTheme + "/btn_PS_chiudi.gif", "../App_Themes/ImgComuni/btn_PS_chiudi_dis.gif");
                    }

                    this.EnableImageButton(this.btnAcquireDocument, true, "../App_Themes/" + pageTheme + "/btn_PS_acquisisci.gif", "../App_Themes/ImgComuni/btn_PS_acquisisci_dis.gif");
                    this.EnableImageButton(this.btnAcquireAttach, (countDocument > 0), "../App_Themes/" + pageTheme + "/btn_PS_acquisisci_alleg.gif", "../App_Themes/ImgComuni/btn_PS_acquisisci_alleg_dis.gif");

                    break;

                case "ProtocollatoAcquisito":
                    this.btnNuovoProtocollo.Enabled = true;
                    this.btnRiproponi.Enabled = true;

                    this.EnableImageButton(this.btnAcquireDocument, true, "../App_Themes/" + pageTheme + "/btn_PS_acquisisci.gif", "../App_Themes/ImgComuni/btn_PS_acquisisci_dis.gif");
                    this.EnableImageButton(this.btnAcquireAttach, (countDocument > 0), "../App_Themes/" + pageTheme + "/btn_PS_acquisisci_alleg.gif", "../App_Themes/ImgComuni/btn_PS_acquisisci_alleg_dis.gif");
                    this.EnableImageButton(this.btnClose, true, "../App_Themes/" + pageTheme + "/btn_PS_chiudi.gif", "../App_Themes/ImgComuni/btn_PS_chiudi_dis.gif");

                    break;

                case "ProtocollatoSmistato":
                    if (isAcqObbl)
                    {
                        this.btnNuovoProtocollo.Enabled = (countDocument > 0);
                        this.btnRiproponi.Enabled = (countDocument > 0);
                        this.EnableImageButton(this.btnClose, (countDocument > 0), "../App_Themes/" + pageTheme + "/btn_PS_chiudi.gif", "../App_Themes/ImgComuni/btn_PS_chiudi_dis.gif");
                    }
                    else
                    {
                        this.btnNuovoProtocollo.Enabled = true;
                        this.btnRiproponi.Enabled = true;
                        this.EnableImageButton(this.btnClose, true, "../App_Themes/" + pageTheme + "/btn_PS_chiudi.gif", "../App_Themes/ImgComuni/btn_PS_chiudi_dis.gif");
                    }

                    this.EnableImageButton(this.btnAcquireDocument, true, "../App_Themes/" + pageTheme + "/btn_PS_acquisisci.gif", "../App_Themes/ImgComuni/btn_PS_acquisisci_dis.gif");
                    this.EnableImageButton(this.btnAcquireAttach, (countDocument > 0), "../App_Themes/" + pageTheme + "/btn_PS_acquisisci_alleg.gif", "../App_Themes/ImgComuni/btn_PS_acquisisci_alleg_dis.gif");

                    break;
            }
        }

        /// <summary>
        /// Gestione abilitazione/disabilitazione controlli "ImageButton".
        /// La funzione si rende necessaria unicamente per impostare 
        /// l'immagine in stato disabilitato e il cursore del mouse
        /// </summary>
        /// <param name="button"></param>
        /// <param name="enabled"></param>
        /// <param name="imageUrl"></param>
        /// <param name="disabledImageUrl"></param>
        private void EnableImageButton(ImageButton button,
                                       bool enabled,
                                       string imageUrl,
                                       string disabledImageUrl)
        {
            if (enabled)
            {
                button.Style["CURSOR"] = "Hand";
                button.ImageUrl = imageUrl;
            }
            else
            {
                button.Style["CURSOR"] = "Default";
                button.ImageUrl = disabledImageUrl;
            }

            button.Enabled = enabled;
        }

        /// <summary>
        /// Impostazione backcolor controlli disabilitati
        /// </summary>
        private void SetBackColorFieldsReadOnly()
        {
            this.txtNumProtocollo.BackColor = Color.LightGray;
            this.txtDataProtocollo.BackColor = Color.LightGray;
            this.txtSegnatura.BackColor = Color.LightGray;
        }

        /// <summary>
        /// Rimozione valori campi univoci
        /// </summary>
        private void ClearUniqueDocumentData()
        {
            this.txtNumProtocollo.Text = null;
          //IMPOSTA this.txtDataProtocollo
            SetDataProtocollo();
            this.txtSegnatura.Text = string.Empty;
            this.txtDescrProtMittente.Text = string.Empty;
            this.GetCalendarControl("txtDataProtocolloMittente").txt_Data.Text = string.Empty;
            this.GetCalendarControl("txtDataArrivoProt").txt_Data.Text = string.Empty;

            // Pulizia campi nascosti
            this.ResetDocumentID();
            this.ResetDocumentsFolder();
            this.ResetCountDocument();
            this.ResetCountAttachments();
        }

        /// <summary>
        /// Pulizia dei campi relativi alla fascicolazione Rapida
        /// </summary>
        private void ClearFasc()
        {
            this.txt_CodFascicolo.Text = string.Empty;
            this.txt_DescFascicolo.Text = string.Empty;
        }

        /// <summary>
        /// Rimozione valori campi
        /// </summary>
        private void ClearDocumentData(string tipoProto)
        {
            // Rimozione valori campi univoci
              this.ClearUniqueDocumentData();
              this.ClearFasc();

            this.ctrl_oggetto.oggetto_text = string.Empty;
            this.ctrl_oggetto.cod_oggetto_text = string.Empty;

            if (tipoProto.Equals("A"))
            {
                this.ClearDatiCorrispondenteIngresso();
                this.txtDescrProtMittente.Text = string.Empty;
                this.GetCalendarControl("txtDataProtocolloMittente").txt_Data.Text = string.Empty;
                this.GetCalendarControl("txtDataArrivoProt").txt_Data.Text = string.Empty;

                if (DocumentManager.isEnableMittentiMultipli())
                {
                    clearDatiMittentiMultipli();
                }
                
            }
            if (tipoProto.Equals("P"))
            {
                this.ClearDatiCorrispondenteUscita();
            }

            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_TIPOLOGIA_DOC_PROT_SEMPL) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_TIPOLOGIA_DOC_PROT_SEMPL).Equals("1"))
            {
                this.ddl_tipoAtto.SelectedIndex = 0;
                this.pnl_DiagrammiStato.Visible = false;
                this.pnl_DataScadenza.Visible = false;
            }

            ddl_trasm_rapida.SelectedIndex = -1;

            // Rimozione delle selezioni dalla lista delle UO
            this.GetControlSmistaUO().ClearSelections();

            // Rilascio delle UO selezionate dalla sessione
            this.ReleaseSessionSelectedUO();

            // Pulizia campi nascosti
            this.ResetDocumentID();
            this.ResetDocumentsFolder();
            this.ResetCountDocument();
            this.ResetCountAttachments();

            DocumentManager.removeClassificazioneSelezionata(this);
            FascicoliManager.removeFascicoloSelezionatoFascRapida();

            //Rimozione note se abilitate
            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_NOTE_PROT_SEMPL) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_NOTE_PROT_SEMPL).Equals("1"))
            {
                this.dettaglioNota.Clear();
            }
            this.ClearPrivato();
        }

        #endregion

        #region Gestione campi HTML HiddenText

        /// <summary>
        /// Impostazione dell'id del protocollo, composto da:
        //  "USERID_IDAMM_IDREGISTRO_NUMPROTO".
        //  Utilizzato per la creazione della cartella temporanea
        //  necessaria per la memorizzazione delle immagini scansionate.
        /// </summary>
        private void SetDocumentID()
        {
            string documentID = string.Empty;

            ProtocollazioneIngresso.Login.LoginMng loginMng = new ProtocollazioneIngresso.Login.LoginMng(this);
            ProtocollazioneIngresso.Protocollo.ProtocolloMng protocolloMng = this.GetProtocolloManager();

            DocsPAWA.DocsPaWR.InfoUtente infoUtente = loginMng.GetInfoUtente();
            documentID = infoUtente.userId + "_" +
                       infoUtente.idAmministrazione + "_" +
                       this.GetRegistroCorrente().systemId + "_" +
                       protocolloMng.GetDocumentoCorrente().protocollo.numero;

            this.txtDocumentID.Value = documentID;
        }

        private void ResetDocumentID()
        {
            this.txtDocumentID.Value = string.Empty;
        }

        private void ResetDocumentsFolder()
        {
            this.txtDocumentsFolder.Value = string.Empty;
        }

        /// <summary>
        /// Rimozione del numero di documenti principali scansionati
        /// </summary>
        private void ResetCountDocument()
        {
            this.txtCountDocument.Value = "0";
        }

        /// <summary>
        /// Reperimento del numero di documenti principali scansionati
        /// (il valore è memorizzato in un HiddenText della form)
        /// </summary>
        /// <returns></returns>
        private int GetCountDocument()
        {
            int retValue = 0;

            if (this.txtCountDocument.Value != string.Empty)
                retValue = Convert.ToInt32(this.txtCountDocument.Value);

            return retValue;
        }

        /// <summary>
        /// Rimozione del numero di documenti allegati scansionati
        /// </summary>
        private void ResetCountAttachments()
        {
            this.txtCountAttachment.Value = "0";
        }

        /// <summary>
        /// Reperimento del numero di documenti allegati scansionati
        /// (il valore è memorizzato in un HiddenText della form)
        /// </summary>
        /// <returns></returns>
        private int GetCountAttachments()
        {
            int retValue = 0;

            if (this.txtCountAttachment.Value != string.Empty)
                retValue = Convert.ToInt32(this.txtCountAttachment.Value);

            return retValue;
        }

        private void SetStatoDocumento(StatoDocumentoEnum statoProtocollazione)
        {
            this.txtStatoDocumento.Value = statoProtocollazione.ToString();
        }

        private StatoDocumentoEnum GetStatoDocumento()
        {
            string statoProt = this.txtStatoDocumento.Value;
            return (StatoDocumentoEnum)Enum.Parse(typeof(StatoDocumentoEnum), statoProt);
        }

        #endregion

        #region Gestione registro protocollazione

        /// <summary>
        /// Verifica la presenza di almeno un registro aperto
        /// </summary>
        /// <returns></returns>
        private bool ContainsRegistriAperti()
        {
            //return (this.cboRegistriDisponibili.Items.Count>0);
            return (this.GetRegistriAperti().Length > 0);
        }

        /// <summary>
        /// Impostazione del registro corrente
        /// </summary>
        /// <param name="idRegistro"></param>
        private void SetRegistro(string idRegistro)
        {
            // Reperimento dello stato del registro
            Registro.RegistroMng regMng = this.GetRegistroManager();

            regMng.SetRegistroCorrente(idRegistro);

            // Impostazione data di apertura del registro come la data del protocollo
            this.SetDataProtocollo();

            this.RefreshColorStatoRegistro(regMng.GetStatoRegistroCorrente());
        }

        private Registro.RegistroMng GetRegistroManager()
        {
            if (this._registroMng == null)
                this._registroMng = new Registro.RegistroMng(this);

            return this._registroMng;
        }

        /// <summary>
        /// Reperimento registro correntemente selezionato
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Registro GetRegistroCorrente()
        {
            return this.GetRegistroManager().GetRegistroCorrente();
        }

        private void RefreshColorStatoRegistro(string statoRegistro)
        {
            switch (statoRegistro)
            {
                case "R":
                    //pnlStatoRegistro.BackColor=Color.Red;
                    this.imgStatoRegistro.ImageUrl = "Images/StatoRegistroRosso.gif";
                    break;

                case "V":
                    //pnlStatoRegistro.BackColor=Color.Green;
                    this.imgStatoRegistro.ImageUrl = "Images/StatoRegistroVerde.gif";
                    break;

                case "G":
                    //pnlStatoRegistro.BackColor=Color.Yellow;
                    this.imgStatoRegistro.ImageUrl = "Images/StatoRegistroGiallo.gif";
                    break;
            }
        }

        /// <summary>
        /// Impostazione del registro correntemente selezionato
        /// e caricamento delle UO dipendenti per lo smistamento
        /// </summary>
        /// <param name="idRegistro"></param>
        private void SetRegistroCorrente(string idRegistro)
        {
            this.SetRegistro(idRegistro);

            // cancellazione dei campi relativi alla fascicolazione rapida
            this.ClearFasc();

            // Caricamento delle UO cui smistare il documento
            this.FillUO(idRegistro);
        }

        /// <summary>
        /// Reperimento dei registri aperti
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Registro[] GetRegistriAperti()
        {
            if (this._registriAperti == null)
            {
                ProtocollazioneIngresso.Registro.RegistroMng regMng = this.GetRegistroManager();
                this._registriAperti = regMng.GetRegistriAperti();
            }

            return this._registriAperti;
        }

        private void FillComboRegistri()
        {
            DocsPAWA.DocsPaWR.Registro[] registri = this.GetRegistriAperti();

            foreach (DocsPAWA.DocsPaWR.Registro registro in registri)
            {
                this.cboRegistriDisponibili.Items.Add(new ListItem(registro.codRegistro + " - " + registro.descrizione, registro.systemId));
            }
        }

        #endregion

        #region Gestione protocollazione

        private Protocollo.ProtocolloMng GetProtocolloManager()
        {
            if (this._protocolloMng == null)
                this._protocolloMng = new Protocollo.ProtocolloMng(this);

            return this._protocolloMng;
        }

        /// <summary>
        /// Creazione e inizializzazione di un nuovo documento
        /// </summary>
        /// <param name="leaveCurrentData">se true, vengono mantenuti i dati corrementemente visualizzati (funzione Riproponi)</param>
        private void CreateNewDocument(bool leaveCurrentData, string tipoProto)
        {
            // Impostazione dello stato del documento come non protocollato (stato iniziale)
            this.SetStatoDocumento(StatoDocumentoEnum.NonProtocollato);

            // Abilitazione campi protocollazione
            this.EnableCampiProtocollazione(true, tipoProto);

            Protocollo.ProtocolloMng protocolloMng = this.GetProtocolloManager();

            DocsPAWA.DocsPaWR.SchedaDocumento currentDoc = this.GetProtocolloManager().GetDocumentoCorrente();

            if (leaveCurrentData)
            {
                // Rimozione dei soli valori dei campi univoci:
                // - numero protocollo
                // - data protocollo
                // - segnatura
                this.ClearUniqueDocumentData();

                // Riproposizione documento
                protocolloMng.RiproponiDocumento(tipoProto);

                if (currentDoc != null)
                {
                    if (currentDoc.privato != null)
                    {
                        if (currentDoc.privato.Equals("1"))
                        {
                            this.chkPrivato.Checked = true;
                        }
                        else
                        {
                            this.chkPrivato.Checked = false;
                        }
                    }
                }
                //this.dettaglioNota.AttatchPulsanteConferma(this.btnProtocolla.ClientID);
                this.dettaglioNota.Fetch();
                Session.Add("protocolloRiproposto", true);
            }
            else
            {
                // Rimozione di tutti i dati correntemente visualizzati
                this.ClearDocumentData(tipoProto);

                if (Session["isUniqueTipoAtto"] != null && Session["isUniqueTipoAtto"].ToString().ToLower().Equals("false") && Session["template"] != null)
                    Session.Remove("template");

                // Creazione nuovo documento
                protocolloMng.CreaNuovoDocumento(tipoProto);

                if (tipoProto.Equals("A"))
                {
                    // Creazione nuovo oggetto "ProtocolloEntrata"
                    protocolloMng.GetDocumentoCorrente().protocollo = new DocsPAWA.DocsPaWR.ProtocolloEntrata();
                    //this.dettaglioNota.AttatchPulsanteConferma(this.btnProtocolla.ClientID);
                    this.dettaglioNota.Fetch();
                }
                else
                {
                    // Creazione nuovo oggetto "ProtocolloUscita"
                    protocolloMng.GetDocumentoCorrente().protocollo = new DocsPAWA.DocsPaWR.ProtocolloUscita();
                    //this.dettaglioNota.AttatchPulsanteConferma(this.btnProtocolla.ClientID);
                    this.dettaglioNota.Fetch();
                    SettaMittenteDefault(true);
                }
            }

            //setto a 1 il numero di stampe etichette per nuovo protocollo o riproponi
            txt_num_stampe.Text = "1";

            // Impostazione data protocollo
            this.SetDataProtocollo();
            
            this.ddl_trasm_rapida.SelectedIndex = -1;
            // Impostazione del focus sul campo oggetto
            //this.ctrl_oggetto.oggetto_SetControlFocus();
        }


        /// <summary>
        /// Impostazione della data del protocollo (data di apertura del registro corrente)
        /// </summary>
        private void SetDataProtocollo()
        {
            Registro.RegistroMng regMng = this.GetRegistroManager();

            // Impostazione della data di apertura del registro come la data del protocollo
            this.txtDataProtocollo.Text = regMng.GetRegistroCorrente().dataApertura;
        }

        /// <summary>
        /// Creazione nuovo oggetto
        /// </summary>
        private void CreateNewOggetto()
        {
            DocsPAWA.DocsPaWR.SchedaDocumento currentDoc = this.GetProtocolloManager().GetDocumentoCorrente();
            DocsPAWA.DocsPaWR.Oggetto oggetto = new DocsPAWA.DocsPaWR.Oggetto();
            oggetto.descrizione = this.ctrl_oggetto.oggetto_text;
            currentDoc.oggetto = oggetto;

            if (currentDoc.systemId != null)
                currentDoc.oggetto.daAggiornare = true;
        }

        /// <summary>
        /// Validazione dati immessi ai fini della protocollazione
        /// </summary>
        /// <param name="validationItems"></param>
        /// <returns></returns>
        private bool IsValidRequiredData(out ArrayList validationItems, out string firstInvalidControlID)
        {
            bool retValue = true;
            validationItems = new ArrayList();
            firstInvalidControlID = string.Empty;

            if (this.ctrl_oggetto.oggetto_text.Length == 0)
            {
                validationItems.Add("Oggetto mancante");

                if (firstInvalidControlID == string.Empty)
                    firstInvalidControlID = this.ctrl_oggetto.Ogg_ClientID;

                retValue = false;
            }

            bool isValidDataProt = false;

            //this.GetCalendarControl("txtDataArrivoProt").DataValue

            if (this.txtDataProtocollo.Text.Length > 0)
            {
                isValidDataProt = DocsPAWA.Utils.isDate(this.txtDataProtocollo.Text);

                if (!isValidDataProt)
                {
                    validationItems.Add("Data di protocollo non valida");

                    if (firstInvalidControlID == string.Empty)
                        firstInvalidControlID = this.txtDataProtocollo.ClientID;

                    retValue = false;
                }
            }
            if (tipoProto.Equals("A"))
            {
                //if (this.txtCodMittente.Text.Length==0 && this.txtDescrMittente.Text.Length==0)
                if (this.txtDescrMittente.Text.Length == 0)
                {
                    validationItems.Add("Mittente mancante");

                    if (firstInvalidControlID == string.Empty)
                        firstInvalidControlID = this.txtCodMittente.ClientID;

                    retValue = false;
                }
                bool isValidDataProtMittente = false;
                bool isValidDataArrivoProtMittente = false;
                if (this.GetCalendarControl("txtDataProtocolloMittente").txt_Data.Text.Length > 0)
                {
                    isValidDataProtMittente = DocsPAWA.Utils.isDate(this.GetCalendarControl("txtDataProtocolloMittente").txt_Data.Text);

                    if (!isValidDataProtMittente)
                    {
                        validationItems.Add("Data del protocollo mittente non valida");

                        if (firstInvalidControlID == string.Empty)
                            firstInvalidControlID = this.txtDataProtocolloMittente.ClientID;

                        retValue = false;
                    }
                }

                if (this.GetCalendarControl("txtDataArrivoProt").txt_Data.Text.Length > 0)
                {
                    isValidDataArrivoProtMittente = DocsPAWA.Utils.isDate(this.GetCalendarControl("txtDataArrivoProt").txt_Data.Text);

                    if (!isValidDataArrivoProtMittente)
                    {
                        validationItems.Add("Data di arrivo del protocollo mittente non valida");

                        if (firstInvalidControlID == string.Empty)
                            firstInvalidControlID = this.txtDataArrivoProt.ClientID;


                        retValue = false;
                    }
                }

                if (isValidDataProt && isValidDataProtMittente)
                {
                    // La data del protocollo mittente non può essere superiore
                    // alla data del protocollo
                    DateTime dataProtocollo = DateTime.Parse(this.txtDataProtocollo.Text);
                    DateTime dataProtocolloMittente = DateTime.Parse(this.GetCalendarControl("txtDataProtocolloMittente").txt_Data.Text);

                    if (dataProtocolloMittente > dataProtocollo)
                    {
                        validationItems.Add("La data del protocollo mittente non può essere superiore alla data di protocollazione");

                        if (firstInvalidControlID == string.Empty)
                            firstInvalidControlID = this.txtDataProtocolloMittente.ClientID;

                        retValue = false;
                    }
                }

                if (isValidDataProt && isValidDataArrivoProtMittente)
                {
                    // La data di arrivo del protocollo mittente non può essere superiore
                    // alla data del protocollo
                    DateTime dataProtocollo = DateTime.Parse(this.txtDataProtocollo.Text);
                    DateTime dataArrivoProtocollo = DateTime.Parse(this.GetCalendarControl("txtDataArrivoProt").txt_Data.Text);

                    if (dataArrivoProtocollo > dataProtocollo)
                    {
                        validationItems.Add("La data di arrivo del protocollo mittente non può essere superiore alla data di protocollazione");

                        if (firstInvalidControlID == string.Empty)
                            firstInvalidControlID = this.GetCalendarControl("txtDataArrivoProt").txt_Data.ClientID;

                        retValue = false;
                    }
                }
            }
            if (tipoProto.Equals("P"))
            {
                if (this.lbx_dest.Items.Count == 0)
                {
                    validationItems.Add("Destinatari mancanti");

                    if (firstInvalidControlID == string.Empty)
                        firstInvalidControlID = this.txtCodDest.ClientID;

                    retValue = false;
                }

                //
                if (!string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                {
                    ProtocollazioneIngresso.Protocollo.ProtocolloMng protocolloMng = this.GetProtocolloManager();
                    DocsPAWA.DocsPaWR.SchedaDocumento documento = protocolloMng.GetDocumentoCorrente();
                    if (documento != null)
                    {
                        Corrispondente temp = new Corrispondente();
                        if ((((DocsPAWA.DocsPaWR.ProtocolloUscita)documento.protocollo).mittente) != null && ((DocsPAWA.DocsPaWR.ProtocolloUscita)documento.protocollo).mittente.tipoCorrispondente=="O")
                        {
                            retValue = false;
                            validationItems.Add("Non è possibile inserire occasionali come mittenti");
                        }
                    }
                }
                //
            }
            // Verifica se almeno una UO è stata selezionata ai fini dello smistamento
            if (!this.GetControlSmistaUO().AlmostOneUOSelected() && this.ddl_trasm_rapida.SelectedIndex==0 && IsUORequired())
            {
                validationItems.Add("Nessuna UO selezionata per lo smistamento");

                // Impostazione diretta del focus sul controllo
                //if (firstInvalidControlID == string.Empty)
                //  this.GetControlSmistaUO().SetFocusNullRadio();

                retValue = false;
            }
            

            if (this.isFascRapidaRequired && this.txt_CodFascicolo.Text.Equals(""))
            {
                validationItems.Add("Nessun fascicolo selezionato per la fascicolazione rapida");

                if (firstInvalidControlID == string.Empty)
                    //this.txt_CodFascicolo.Focus();
                    firstInvalidControlID = this.txt_CodFascicolo.ClientID;

                retValue = false;
            }

            // controllo su obbligatorietà campo tipologia documento
            if (this.isTipologiaDocumentoRequired && this.ddl_tipoAtto.SelectedIndex == 0)
            {
                validationItems.Add("Campo Tipologia Documento non valorizzato");
                retValue = false;
            }

            return retValue;
        }

        /// <summary>
        /// Verifica esistenza del documento
        /// </summary>
        /// <returns></returns>
        private bool ContainsDocumento()
        {
            bool res = false;
            ProtocollazioneIngresso.Protocollo.ProtocolloMng protocolloMng = this.GetProtocolloManager();

            DocsPAWA.DocsPaWR.InfoProtocolloDuplicato[] infoProtocolloDuplicato;

            DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum containsDocumento = this.GetProtocolloManager().ContainsDocumento(out infoProtocolloDuplicato);

            if (containsDocumento != DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum.NessunDuplicato)
            {
                // Disabilitazione pulsante protocollazione
                this.RegisterClientScript("EnableButtonProtocollazione", "EnableButtonProtocollazione(false);");

                if (infoProtocolloDuplicato.Length > 0)
                {
                    #region old_code
                    //string msg=string.Empty;

                    //msg += Environment.NewLine + Environment.NewLine + "Data: " + infoProtocolloDuplicato[0].dataProtocollo;
                    //msg += Environment.NewLine + "Segnatura: " + infoProtocolloDuplicato[0].segnaturaProtocollo;

                    //if(infoProtocolloDuplicato[0].uoProtocollatore!=null && 
                    //    infoProtocolloDuplicato[0].uoProtocollatore!=string.Empty)
                    //    msg += Environment.NewLine + "Ufficio: " + infoProtocolloDuplicato[0].uoProtocollatore;

                    //msg="Dati di protocollazione già presenti:" + msg + Environment.NewLine + Environment.NewLine + "Continuare?";

                    //this.txtMessageProtocolloEsistente.Value=msg;
                    //this.RegisterClientScript("ConfirmInsertProtocollo","ConfirmInsertProtocollo();");
                    #endregion

                    int modTrasm = -1;

                    if (cfg_Smista_Abilita_Trasm_Rapida())
                        modTrasm = ddl_trasm_rapida.SelectedIndex;

                    string querySt = this.Server.UrlEncode("dtaProto=" + infoProtocolloDuplicato[0].dataProtocollo
                    + "&Segn=" + infoProtocolloDuplicato[0].segnaturaProtocollo + "&Uff=" + infoProtocolloDuplicato[0].uoProtocollatore
                    + "&IdDoc=" + infoProtocolloDuplicato[0].idProfile + "&NumProto=" + infoProtocolloDuplicato[0].numProto
                    + "&DoProto=SI&result=" + containsDocumento + "&modelloTrasm=" + modTrasm + "&docAcquisito=" + infoProtocolloDuplicato[0].docAcquisito);

                    string script = "<script>var retValue=window.showModalDialog('../popup/avvisoProtocolloEsistente.aspx?" + querySt
                                    + "','','dialogWidth:520px;dialogHeight:240px;fullscreen:no;toolbar:no;status:no;resizable:no;scroll:auto;"
                                    + "center:yes;help:no;close:no'); if (retValue=='protocolla')InsertProtocollo(); if(retValue=='chiudi')CloseProtocollo();</script>";
                    this.ClientScript.RegisterStartupScript(this.GetType(), "popupDuplicati", script);
                }
            }
            else
                res = true;

            return res;
        }

        /// <summary>
        /// Smistamento documento corrente, solamente se lo stato è protocollato
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.EsitoSmistamentoDocumento[] SmistaDocumentoCorrente()
        {
            DocsPAWA.DocsPaWR.EsitoSmistamentoDocumento[] retValue = null;

            if (this.GetStatoDocumento() == StatoDocumentoEnum.Protocollato)
            {
                // Smistamento del documento (solo se protocollato)
                retValue = this.GetControlSmistaUO().SmistaDocumento(this.GetSessionSelectedUO());

                // Impostazione dello stato del documento come "ProtocollatoSmistato"
                this.SetStatoDocumento(StatoDocumentoEnum.ProtocollatoSmistato);
            }

            return retValue;
        }

        /// <summary>
        /// Visualizzazione messaggio smistamento non andato a buon fine
        /// </summary>
        /// <param name="esito"></param>
        private void ShowMessageDocumentiNonSmistati(DocsPAWA.DocsPaWR.EsitoSmistamentoDocumento[] esito)
        {
            string message = string.Empty;

            foreach (DocsPAWA.DocsPaWR.EsitoSmistamentoDocumento item in esito)
            {
                // Se smistamento non andato a buon fine,
                // reperimento di tutti i destinatari del documento
                // e visualizzazione di un'alert javascript
                if (item.CodiceEsitoSmistamento > 0)
                {
                    if (message != string.Empty)
                        message += "\\n";

                    message += "-" + item.DenominazioneDestinatario;
                }
            }

            if (message != string.Empty)
            {
                message = "Non è stato possibile smistare il documento ai seguenti ruoli destinatari: \\n\\n" + message;

                this.RegisterClientScript("MessageDocumentiNonSmistati", "alert('" + message + "')");
            }
        }

        /// <summary>
        /// Protocollazione del documento
        /// </summary>
        /// <returns></returns>
        private bool ProtocollaDocumentoCorrente(out string errorMessage)
        {
            ProtocollazioneIngresso.Protocollo.ProtocolloMng protocolloMng = this.GetProtocolloManager();

            // Protocollazione del documento
            bool retValue = protocolloMng.ProtocollaDocumentoCorrente(out errorMessage);

            if (retValue)
            {
                DocsPAWA.DocsPaWR.SchedaDocumento documento = protocolloMng.GetDocumentoCorrente();
               
                //PROFILAZIONE DINAMICA
                string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
                if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
                {
                    //Salvataggio dei campi della profilazione dinamica
                    if (Session["template"] != null)
                    {
                        if (!ProfilazioneDocManager.verificaCampiObbligatori((DocsPAWA.DocsPaWR.Templates)Session["template"]))
                            documento.template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                    }

                    //DIAGRAMMI DI STATO
                    if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                    {
                        if (Session["DiagrammaSelezionato"] != null && ddl_statiSuccessivi.SelectedItem.Value != "")
                        {
                            bool statoFinale = false;
                            bool statoAutomatico = false;
                            Session.Add("statoSelezionato", ddl_statiSuccessivi.SelectedItem.Value);

                            //Controllo se lo stato selezionato è finale
                            if (controllaStatoFinale())
                            {
                                if (DocsPAWA.CheckInOut.CheckInOutServices.IsCheckedOutDocument())
                                {
                                    RegisterStartupScript("docInCheckOut", "<script>alert('Attenzione non è possibile passare in uno stato finale un documento bloccato !');</script>");
                                    return false;
                                }
                                Session.Add("statoFinale", ddl_statiSuccessivi.SelectedValue);
                                Session.Add("docSolaLettura", "SI");
                                statoFinale = true;
                            }
                            //Controllo se lo stato selezionato è uno stato automatico
                           
                            if (DiagrammiManager.isStatoAuto(ddl_statiSuccessivi.SelectedItem.Value, Convert.ToString(((DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"]).SYSTEM_ID),this))
                            {
                                Session.Add("statoAutomatico", "SI");
                                statoAutomatico = true;
                            }

                            if (statoAutomatico || statoFinale)
                            {
                                statoAutomatico = false;
                                statoFinale = false;
                            }
                            else
                            {
                                DiagrammiManager.salvaModificaStato(documento.docNumber, ddl_statiSuccessivi.SelectedItem.Value, (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"], UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), txt_dataScadenza.Text,this);
                                string idTemplate = ProfilazioneDocManager.getIdTemplate(documento.docNumber,this);

                                if (idTemplate != "")
                                {
                                    ArrayList modelli = new ArrayList(DiagrammiManager.isStatoTrasmAuto(UserManager.getInfoUtente(this).idAmministrazione, ddl_statiSuccessivi.SelectedItem.Value, idTemplate,this));
                                    for (int i = 0; i < modelli.Count; i++)
                                    {
                                        ModelloTrasmissione mod = (ModelloTrasmissione)modelli[i];
                                        for (int k = 0; k < mod.MITTENTE.Length; k++)
                                        {
                                            if (mod.SINGLE == "1" || mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                            {
                                                
                                                if (documento != null
                                                    && documento.protocollo != null
                                                    && documento.protocollo.numero != null && documento.protocollo.numero != ""
                                                    && UserManager.getRegistroSelezionato(this) != null
                                                    && mod.ID_REGISTRO == UserManager.getRegistroSelezionato(this).systemId)
                                                {
                                                    effettuaTrasmissione(mod, ddl_statiSuccessivi.SelectedItem.Value);
                                                }
                                                else
                                                    effettuaTrasmissione(mod, ddl_statiSuccessivi.SelectedItem.Value);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //FINE DIAGRAMMI DI STATO 
                }
                //FINE PROFILAZIONE DINAMINCA

                // fascicolazione rapida (se esistente)
                if (!this.txt_CodFascicolo.Text.Equals(""))
                {
                    DocsPAWA.DocsPaWR.Fascicolo fasc = this.getFascicolo();
                    DocsPAWA.DocsPaWR.Folder selectedFolder = FascicoliManager.getFolder(this, fasc);

                    string idDoc = protocolloMng.GetDocumentoCorrente().systemId;

                    bool isInFolder = false;
                    string msg = string.Empty;
                    DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = protocolloMng.GetDocumentoCorrente();
                    //bool docInserted = DocumentManager.addDocumentoInFolder(this, idDoc, selectedFolder.systemID, false, out isInFolder);
                    bool docInserted = DocumentManager.fascicolaRapida(this, currentDocument.systemId, currentDocument.docNumber, currentDocument.protocollo.segnatura, fasc, out msg);
                    if (!docInserted)
                    {
                        // documento non classificato nel folder selezionato
                        string Msg = "Errore durante la classificazione del documento nel fascicolo selezionato!";
                        this.RegisterClientScript("DocumentInFolder", "alert('" + Msg + "');");
                    }
                }

                if (protocolloMng.GetDocumentoCorrente() != null && protocolloMng.GetDocumentoCorrente().tipoProto == "P")
                    completaProtocollazione(protocolloMng.GetDocumentoCorrente());




                // Disabilitazione pulsante protocollazione
                this.RegisterClientScript("EnableButtonProtocollazione", "EnableButtonProtocollazione(false);");

                // Impostazione dello stato del documento corrente come protocollato
                this.SetStatoDocumento(ProtocollazioneIngresso.StatoDocumentoEnum.Protocollato);

                /// Impostazione dell'id del protocollo, composto da:
                //  "USERID_IDAMM_IDREGISTRO_NUMPROTO".
                //  Utilizzato per la creazione della cartella temporanea
                //  necessaria per la memorizzazione delle immagini scansionate.
                this.SetDocumentID();

                // Aggiornamento dati relativamente al documento appena protocollato
                this.RefreshDatiNuovoProtocollo(documento);

                // In seguito alla protocollazione, vengono disabilitati tutti i campi di acquisizione)
                this.EnableCampiProtocollazione(false, tipoProto);

                // Impostazione delle UO selezionate per lo smistamento in sessione.
                // Gli oggetti relativi verranno poi utilizzati nella fase dello smistamento.
                this.SetSessionSelectedUO();

                // Trasmissione rapida del documento


                this.execTrasmRapidaDaModello();

                // Smistamento documento corrente
                DocsPAWA.DocsPaWR.EsitoSmistamentoDocumento[] esitoSmistamento = this.SmistaDocumentoCorrente();

                if (esitoSmistamento.Length > 0)
                    // Gestione visualizzazione messaggio in caso di documenti non smistati
                    this.ShowMessageDocumentiNonSmistati(esitoSmistamento);

                // Stampa automatica dell'etichetta, solo se abilitata
                if (this.AutoPrintSignature())
                    this.PrintSignature();
                if (!isFileUploadEnable) 
                    this.RegisterClientScript("ScanDocuments", "ScanDocuments();");
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se è abilitata da web.config la stampa automatica
        /// dell'etichetta all'atto della protocollazione
        /// </summary>
        /// <returns></returns>
        private bool AutoPrintSignature()
        {
            bool retValue = true;

            try
            {
                string configValue = DocsPAWA.ConfigSettings.getKey(DocsPAWA.ConfigSettings.KeysENUM.PROTOINGRESSO_STAMPA_ETICHETTA_AUTO);

                if (configValue != null && configValue != string.Empty)
                    retValue = Convert.ToBoolean(configValue);
            }
            catch
            {
            }

            return retValue;
        }

        /// <summary>
        /// Indica se è richiesta obbligatoriamente la tipologia atto
        /// </summary>
        /// <returns></returns>
        protected bool IsRequiredTipologiaAtto()
        {
            //return (System.Configuration.ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"] != null
            //       && System.Configuration.ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"] == "1");
            string idAmm = UserManager.getInfoUtente().idAmministrazione;
            if (DocumentManager.GetTipoDocObbl(idAmm).Equals("1"))
                return true;
            else
                return false;

        }

        protected bool IsUORequired()
        {
            string valoreChiaveDB = DocsPAWA.utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_ENABLE_PROT_RAPIDA_NO_UO");
            // Se il valore è diverso da null oppure è "1" allora ritorna true, altrimenti torna false;
            return (valoreChiaveDB != null && valoreChiaveDB.ToUpper().Equals("TRUE")) ? false : true;

        }

        /// <summary>
        /// Indica se la tipologia atto è valorizzata o meno
        /// </summary>
        /// <returns></returns>
        protected bool IsTipologiaDocOk()
        {
            if (Session["isUniqueTipoAtto"] != null && Session["isUniqueTipoAtto"].ToString().ToLower().Equals("true"))
            {
                return true;
            }
            else
            {
                if (isTipologiaDocumentoRequired)
                    if (this.ddl_tipoAtto.SelectedIndex != 0)
                        return true;
                    else
                        return false;
                else
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Gestione abilitazione / disabilitazione campi UI relativi alla protocollazione
        /// (a seconda se il documento è stato protocollato o meno)
        /// </summary>
        /// <param name="isEnabled"></param>
        private void EnableCampiProtocollazione(bool isEnabled, string tipoProto)
        {
            //elementi UI comuni
            this.cboRegistriDisponibili.Enabled = isEnabled;
            this.btnPrintSignature.Enabled = !isEnabled;
            this.ctrl_oggetto.oggetto_isEnable = isEnabled;
            this.GetControlSmistaUO().EnableControls(isEnabled);
            this.btn_RubrOgget_P.Enabled = isEnabled;
            this.txt_CodFascicolo.Enabled = isEnabled;
            this.txt_DescFascicolo.Enabled = isEnabled;
            this.ddl_trasm_rapida.Enabled = isEnabled;
            this.chkPrivato.Enabled = isEnabled;
            if (DocumentManager.isEnableMittentiMultipli())
            {
                //this.txt_codMittMultiplo.Enabled = isEnabled;
                //this.txt_descMittMultiplo.Enabled = isEnabled;
                this.txtDescrProtMittente.Enabled = isEnabled;
            }
            //this.dettaglioNota.ReadOnly = !isEnabled;
            if (isEnabled)
                this.dettaglioNota.Testo = "";
            else
                this.dettaglioNota.Testo = "";


            //Attivazione del controllo ortografico di default è disabilitato
            bool useSpell = false;
            if (ConfigurationManager.AppSettings["USE_SPELL_ACTIVEX"] != null)
            {
                bool.TryParse(ConfigurationManager.AppSettings["USE_SPELL_ACTIVEX"], out useSpell);
            }
            if (useSpell)
            {
                this.btn_Correttore.Enabled = true;
                this.btn_Correttore.Visible = true;
            }

            //verifico se ho l'abilitaione alla creazione di un nuovo fascicolo
            if (UserManager.ruoloIsAutorized(this, "FASC_NUOVO"))
            {
                this.imgFascNew.Enabled = true;
            }
            else
            {
                this.imgFascNew.Enabled = false;
            }
            //*************************************

            if (tipoProto.Equals("A"))
            {
                this.txtCodMittente.Enabled = isEnabled;
                this.txtDescrMittente.Enabled = isEnabled;
                this.btnShowRubrica.Enabled = isEnabled;
                this.txtDescrProtMittente.Enabled = isEnabled;

                this.btn_upMittente.Enabled = isEnabled;
                this.btn_downMittente.Enabled = isEnabled;
                this.btn_RubrMittMultiplo.Enabled = isEnabled;
                this.btn_CancMittMultiplo.Enabled = isEnabled;
                this.lbx_mittMultiplo.Enabled = isEnabled;
                this.txtDataProtocolloMittente.EnableBtnCal = isEnabled;
                this.txtDataArrivoProt.EnableBtnCal = isEnabled;


                if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                {
                    if (isEnabled)
                    {
                        this.rubrica_veloce.Visible = true;
                        if (DocumentManager.isEnableMittentiMultipli())
                        {
                            this.rubrica_mitt_multiplo_semplificato.Visible = true;
                        }
                    }
                    else
                    {
                        this.rubrica_veloce.Visible = false;
                        if (DocumentManager.isEnableMittentiMultipli())
                        {
                            this.rubrica_mitt_multiplo_semplificato.Visible = false;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                {
                    if (isEnabled)
                    {
                        pnl_new_mittente_semplificato_ingresso_veloce.Visible = true;
                    }
                }
                // calend
                //this.txtDataProtocolloMittente.Enabled = isEnabled;
                //this.btnShowCalDataProtMittente.Enabled = isEnabled;

                // this.txtDataArrivoProt.Enabled = isEnabled;
                // this.btnShowCalDataArrivo.Enabled = isEnabled;
            }


            if (tipoProto.Equals("P"))
            {
                //per il protocollo in uscita
                this.btn_aggiungiDest_P.Enabled = isEnabled;
                this.btnShowRubricaDest.Enabled = isEnabled;
                this.btn_insDestCC.Enabled = isEnabled;
                this.btn_insDest.Enabled = isEnabled;
                this.btn_cancDestCC.Enabled = isEnabled;
                this.btn_cancDest.Enabled = isEnabled;
                this.txtDescrDest.Enabled = isEnabled;
                this.txtCodDest.Enabled = isEnabled;
                this.txtCodMittUsc.Enabled = isEnabled;
                this.txtDescMittUsc.Enabled = isEnabled;
                this.btnShowRubricaMittUsc.Enabled = isEnabled;
            }

            if (isTipologiaDocumentoVisible)
                this.ddl_tipoAtto.Enabled = isEnabled;

            if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
            {
                if (isEnabled)
                {
                    this.rubrica_veloce_destinatario.Visible = true;
                    this.rubrica_veloce_destinatario_mittente_uscita.Visible = true;
                }
                else
                {
                    this.rubrica_veloce_destinatario.Visible = false;
                    this.rubrica_veloce_destinatario_mittente_uscita.Visible = false;
                }     
            }

            if (!string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
            {
                if (isEnabled)
                {
                    pnl_new_mittente_uscita_semplificato.Visible = true;
                    pnl_new_destinatario_uscita_semplificato.Visible = true;
                    this.txtDescMittUsc.ReadOnly = false;
                }
                else
                {
                    pnl_new_mittente_uscita_semplificato.Visible = false;
                    pnl_new_destinatario_uscita_semplificato.Visible = true;
                    this.txtDescMittUsc.ReadOnly = true;
                }
            }

            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_NOTE_PROT_SEMPL) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUAL_NOTE_PROT_SEMPL).Equals("1"))
            {
                this.dettaglioNota.Enabled = isEnabled;
            }
        }

        /// <summary>
        /// Verifica del valore del campo nascosto "txtInsertProtocolloPending":
        /// se true, è in pending la protocollazione del documento corrente
        /// </summary>
        /// <returns></returns>
        private bool OnInsertProtocolloPending()
        {
            bool retValue = false;

            if (this.txtInsertProtocolloPending.Value == "true")
            {
                this.txtInsertProtocolloPending.Value = "false";
                retValue = true;
            }

            return retValue;
        }

        /// <summary>
        /// Verifica del valore del campo nascosto "txtCloseAnteprimaProf":
        /// se true, è richiesta la non protocollazione del documento corrente
        /// in quanto copia di un protocollo già esistente
        /// </summary>
        /// <returns></returns>
        private bool noSubmitProtCorrente()
        {
            bool retValue = false;

            if (this.txtCloseAnteprimaProf.Value == "true")
            {
                this.txtCloseAnteprimaProf.Value = "false";
                retValue = true;
            }

            return retValue;
        }


        /// <summary>
        /// Aggiornamento dati relativamente al documento appena protocollato
        /// </summary>
        /// <param name="schedaDocumento"></param>
        private void RefreshDatiNuovoProtocollo(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento)
        {
            this.txtNumProtocollo.Text = schedaDocumento.protocollo.numero;
            this.txtSegnatura.Text = schedaDocumento.protocollo.segnatura;

            if (schedaDocumento.tipoProto.Equals("A"))
            {
                DocsPAWA.DocsPaWR.ProtocolloEntrata protocolloEntrata = schedaDocumento.protocollo as DocsPAWA.DocsPaWR.ProtocolloEntrata;
                if (protocolloEntrata != null &&
                    protocolloEntrata.mittente != null &&
                    protocolloEntrata.mittente.tipoCorrispondente == CODICE_MITTENTE_OCCASIONALE)
                {
                    // Impostazione del codice mittente occasionale, generato dopo la protocollazione
                    this.txtCodMittente.Text = protocolloEntrata.mittente.codiceRubrica;
                }
            }
            else
            {
                DocsPAWA.DocsPaWR.ProtocolloUscita protocolloUscita = schedaDocumento.protocollo as DocsPAWA.DocsPaWR.ProtocolloUscita;
                if (protocolloUscita != null &&
                    protocolloUscita.mittente != null &&
                    protocolloUscita.mittente.tipoCorrispondente == CODICE_MITTENTE_OCCASIONALE)
                {
                    // Impostazione del codice mittente occasionale, generato dopo la protocollazione
                    this.txtCodMittUsc.Text = protocolloUscita.mittente.codiceRubrica;
                }
            }
        }

        /// <summary>
        /// Aggiornamento dati immessi nella UI nell'oggetto SchedaDocumento corrente
        /// </summary>
        private void RefreshSchedaDocumentoFromUI()
        {
            Protocollo.ProtocolloMng protocolloMng = this.GetProtocolloManager();

            DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = protocolloMng.GetDocumentoCorrente();

            // Creazione oggetto protocollo per la scheda documento
            DocsPAWA.DocsPaWR.ProtocolloEntrata protocolloEntrata = (DocsPAWA.DocsPaWR.ProtocolloEntrata)currentDocument.protocollo;

            // Impostazione della data di protocollazione,
            // che è la data di apertura del registro
            protocolloEntrata.dataProtocollazione = this.txtDataProtocollo.Text;

            currentDocument.registro = this.GetRegistroCorrente();

            if (protocolloEntrata.mittente == null)
                // Impostazione dell'eventuale mittente occasionale,
                // nel caso in cui l'oggetto mittente non fosse stato
                // ancora valorizzato
                this.SetMittenteOccasionale();

            // Impostazione dati del protocollo mittente
            protocolloEntrata.descrizioneProtocolloMittente = this.txtDescrProtMittente.Text;
            protocolloEntrata.dataProtocolloMittente = this.GetCalendarControl("txtDataProtocolloMittente").txt_Data.Text;

            if (currentDocument.documenti == null || currentDocument.documenti.Length == 0)
            {
                DocsPAWA.DocsPaWR.Documento[] docs = new DocsPAWA.DocsPaWR.Documento[1];
                docs[0] = new DocsPAWA.DocsPaWR.Documento();
                currentDocument.documenti = docs;
            }

            DocsPAWA.DocsPaWR.Documento doc = ((DocsPAWA.DocsPaWR.Documento)currentDocument.documenti[0]);

            doc.dataArrivo = this.GetCalendarControl("txtDataArrivoProt").txt_Data.Text;
            doc = null;
        }


        /// <summary>
        /// Aggiornamento dati immessi nella UI nell'oggetto SchedaDocumento corrente
        /// nel caso di protocollo in uscita
        /// </summary>
        private void RefreshSchedaDocumentoUscitaFromUI()
        {
            Protocollo.ProtocolloMng protocolloMng = this.GetProtocolloManager();

            DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = protocolloMng.GetDocumentoCorrente();

            // Creazione oggetto protocollo per la scheda documento
            DocsPAWA.DocsPaWR.ProtocolloUscita protocolloUscita = (DocsPAWA.DocsPaWR.ProtocolloUscita)currentDocument.protocollo;

            if (protocolloUscita.mittente == null)
            {
                // Impostazione dell'eventuale mittente occasionale,
                // nel caso in cui l'oggetto mittente non fosse stato
                // ancora valorizzato
                this.SetMittenteOccasionaleInProtoUscita();
            }

            // Impostazione della data di protocollazione,
            // che è la data di apertura del registro
            protocolloUscita.dataProtocollazione = this.txtDataProtocollo.Text;

            currentDocument.registro = this.GetRegistroCorrente();
        }

        #endregion

        #region Gestione smistamento documento

        private bool cfg_Smista_Abilita_Trasm_Rapida()
        {
            string valoreChiaveDB = DocsPAWA.utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_SMISTA_ABILITA_TRASM_RAPIDA");
            // Se il valore è diverso da null oppure è "1" allora ritorna true, altrimenti torna false;
            return (valoreChiaveDB != null && valoreChiaveDB.Equals("1")) ? true : false;
        }

        /// <summary>
        /// Caricamento delle UO cui smistare il documento
        /// </summary>
        /// <param name="registro"></param>
        private void FillUO(string idRegistro)
        {
            //abilito pannello per la trasmissione rapida
            this.pnl_trasm_rapida.Visible = false;
            this.isTrasmRapidaRequired.Value = "false";

            if (cfg_Smista_Abilita_Trasm_Rapida())
            {
                this.pnl_trasm_rapida.Visible = true;
                this.isTrasmRapidaRequired.Value = "true";
                this.caricaModelliTrasmRapida();
            }

            ProtocollazioneIngresso.Smistamento.SmistaUO smistaUO = this.GetControlSmistaUO();
            smistaUO.LoadData(idRegistro);

            //Se il valore ritornato è FALSE restituisce false (l'inversione è fatta nel metodo IsUORequired
            if (!IsUORequired())
                this.isTrasmRapidaRequired.Value = "false";

            // if (System.Configuration.ConfigurationManager.AppSettings["ENABLE_PROT_RAPIDA_NO_UO"] != null
            //    && System.Configuration.ConfigurationManager.AppSettings["ENABLE_PROT_RAPIDA_NO_UO"].Equals("true"))
            //     this.isTrasmRapidaRequired.Value = "false";

        }

        private ProtocollazioneIngresso.Smistamento.SmistaUO GetControlSmistaUO()
        {
            return (ProtocollazioneIngresso.Smistamento.SmistaUO)this.panelUO.FindControl("SmistaUO");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
        }

        #endregion

        #region Gestione rubrica mittenti

        /// <summary>
        /// Verifica se, in base ad un flag del web.config, 
        /// sia abilitata la gestione della rubrica nuova
        /// </summary>
        /// <returns></returns>
        private bool IsEnabledNewRubrica()
        {
            bool retValue = false;

            string configValue = DocsPAWA.ConfigSettings.getKey(DocsPAWA.ConfigSettings.KeysENUM.RUBRICA_V2);

            if (configValue != null && configValue != string.Empty)
                retValue = (configValue == "1");

            return retValue;
        }

        private DocsPAWA.DocsPaWR.Corrispondente GetCorrispondenteDaCodice(string codCorrispondente)
        {
            DocsPAWA.DocsPaWR.Corrispondente retValue = null;

            if (codCorrispondente != null)
            {
                retValue = DocsPAWA.UserManager.getCorrispondente(this, codCorrispondente, true);
            }

            return retValue;
        }

        /// <summary>
        /// Impostazione oggetto corrisponente nel protocollo corrente
        /// </summary>
        /// <param name="corrispondente"></param>
        private void SetCorrispondenteInProtocollo(DocsPAWA.DocsPaWR.Corrispondente corrispondente)
        {
            DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc = this.GetProtocolloManager().GetDocumentoCorrente();
            if (schedaDoc != null)
                ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittente = corrispondente;
        }
        /// <summary>
        /// Impostazione oggetto corrisponente mittente nel protocollo in uscita corrente        
        /// </summary>
        /// <param name="corrispondente"></param>
        private void SetCorrispondenteMittenteInProtocolloUscita(DocsPAWA.DocsPaWR.Corrispondente corrispondente)
        {
            DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc = this.GetProtocolloManager().GetDocumentoCorrente();
            if (schedaDoc != null)
                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).mittente = corrispondente;
        }




        private void SetCorrispondentiUscitaInProtocollo(DocsPAWA.DocsPaWR.Corrispondente[] corrispondenti)
        {
            DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc = this.GetProtocolloManager().GetDocumentoCorrente();
            if (schedaDoc != null)
            {
                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari = corrispondenti;

            }
        }

        private void SetCorrispondentiConoscenzaUscitaInProtocollo(DocsPAWA.DocsPaWR.Corrispondente[] corrispondenti)
        {
            DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc = this.GetProtocolloManager().GetDocumentoCorrente();
            if (schedaDoc != null)
            {
                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza = corrispondenti;

            }
        }

        /// <summary>
        /// Impostazione del mittente occasionale
        /// in base alla descrizione immessa dall'utente
        /// </summary>
        private void SetMittenteOccasionale()
        {
            DocsPAWA.DocsPaWR.SchedaDocumento currentDoc = this.GetProtocolloManager().GetDocumentoCorrente();

            DocsPAWA.DocsPaWR.ProtocolloEntrata protocolloEntrata = currentDoc.protocollo as DocsPAWA.DocsPaWR.ProtocolloEntrata;

            if (protocolloEntrata != null)
            {
                DocsPAWA.DocsPaWR.Corrispondente mittente = null;

                if (this.txtDescrMittente.Text != string.Empty)
                {
                    // Creazione oggetto mittente occasionale
                    mittente = new DocsPAWA.DocsPaWR.Corrispondente();
                    mittente.tipoCorrispondente = CODICE_MITTENTE_OCCASIONALE;
                    mittente.descrizione = this.txtDescrMittente.Text;

                    //aggiunto per la modifica del popolamento del campo VAR_COD_RUBRICA dei corrisp occasionali
                    ProtocollazioneIngresso.Login.LoginMng loginMng = new ProtocollazioneIngresso.Login.LoginMng(this);
                    mittente.idAmministrazione = (loginMng.GetInfoUtente()).idAmministrazione;
                    loginMng = null;

                    if (currentDoc.systemId != null)
                        protocolloEntrata.daAggiornareMittente = true;
                }

                // Impostazione oggetto corrispondente nel protocollo corrente
                this.SetCorrispondenteInProtocollo(mittente);
            }
        }

        /// <summary>
        /// Impostazione del mittente occasionale
        /// in base alla descrizione immessa dall'utente nel protocollo in uscita
        /// </summary>
        private void SetMittenteOccasionaleInProtoUscita()
        {
            DocsPAWA.DocsPaWR.SchedaDocumento currentDoc = this.GetProtocolloManager().GetDocumentoCorrente();

            DocsPAWA.DocsPaWR.ProtocolloUscita protocolloUscita = currentDoc.protocollo as DocsPAWA.DocsPaWR.ProtocolloUscita;

            if (protocolloUscita != null)
            {
                DocsPAWA.DocsPaWR.Corrispondente mittente = null;

                if (this.txtDescMittUsc.Text != string.Empty)
                {
                    // Creazione oggetto mittente occasionale
                    mittente = new DocsPAWA.DocsPaWR.Corrispondente();
                    mittente.tipoCorrispondente = CODICE_MITTENTE_OCCASIONALE;
                    mittente.descrizione = this.txtDescMittUsc.Text;

                    ////aggiunto per la modifica del popolamento del campo VAR_COD_RUBRICA dei corrisp occasionali
                    //ProtocollazioneIngresso.Login.LoginMng loginMng = new ProtocollazioneIngresso.Login.LoginMng(this);
                    //mittente.idAmministrazione = (loginMng.GetInfoUtente()).idAmministrazione;
                    //loginMng = null;

                    if (currentDoc.systemId != null)
                        protocolloUscita.daAggiornareMittente = true;
                }

                // Impostazione oggetto corrispondente nel protocollo corrente
                this.SetCorrispondenteMittenteInProtocolloUscita(mittente);
            }
        }

        /// <summary>
        /// Rimozione dell'oggetto corrispondente corrente
        /// e pulizia campi UI
        /// </summary>
        private void ClearDatiCorrispondenteIngresso()
        {
            this.SetCorrispondenteInProtocollo(null);

            this.txtCodMittente.Text = string.Empty;
            this.txtDescrMittente.Text = string.Empty;
        }

        /// <summary>
        /// Rimozione dei destinatari e del mittent corrente e pulizia dei campi UI
        /// </summary>
        private void ClearDatiCorrispondenteUscita()
        {
            //eliminazione degli items relativi ai corrispondenti principali
            this.SetCorrispondentiUscitaInProtocollo(null);

            this.SetCorrispondenteMittenteInProtocolloUscita(null);
            if (lbx_dest != null && this.lbx_dest.Items != null && this.lbx_dest.Items.Count > 0)
            {
                this.lbx_dest.Items.Clear();
            }
            //pulizia campi relativi al destinatario
            this.txtDescrDest.Text = string.Empty;
            this.txtCodDest.Text = string.Empty;

            //pulizia campi relativi al mittente
            if (!(ConfigSettings.getKey(ConfigSettings.KeysENUM.MITTENTE_DEFAULT) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.MITTENTE_DEFAULT).ToUpper().Equals("1")))
            {
                this.txtCodMittUsc.Text = string.Empty;
                this.txtDescMittUsc.Text = string.Empty;
            }
            else
            {
                this.SettaMittenteDefault(true);
            }
            //eliminazione degli items relativi ai corrispondenti in conoscenza
            this.ClearDatiCorrispondenteUscitaConoscenza();

            
        }

        private void ClearDatiCorrispondenteUscitaConoscenza()
        {
            this.SetCorrispondentiConoscenzaUscitaInProtocollo(null);
            if (this.lbx_destCC != null && this.lbx_destCC.Items != null && this.lbx_destCC.Items.Count > 0)
            {
                this.lbx_destCC.Items.Clear();
            }
        }

        /// <summary>
        /// Caricamento dati del corrispondente nei campi della UI
        /// </summary>
        private void FillDatiCorrispondente()
        {
            // Reperimento del documento corrente
            DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = this.GetProtocolloManager().GetDocumentoCorrente();

            if (currentDocument != null)
            {
                DocsPAWA.DocsPaWR.ProtocolloEntrata protocolloEntrata = currentDocument.protocollo as DocsPAWA.DocsPaWR.ProtocolloEntrata;

                if (protocolloEntrata != null && protocolloEntrata.mittente != null)
                {
                    this.txtCodMittente.Text = protocolloEntrata.mittente.codiceRubrica;
                    this.txtDescrMittente.Text = protocolloEntrata.mittente.descrizione;
                }
            }
        }

        /// <summary>
        /// Caricamento dati del corrispondente mittente nel protocollo in uscita semplificato
        /// </summary>
        private void FillDatiCorrispondenteMittenteInUscita()
        {
            // Reperimento del documento corrente
            DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = this.GetProtocolloManager().GetDocumentoCorrente();

            if (currentDocument != null)
            {
                DocsPAWA.DocsPaWR.ProtocolloUscita protocolloUscita = currentDocument.protocollo as DocsPAWA.DocsPaWR.ProtocolloUscita;

                if (protocolloUscita != null && protocolloUscita.mittente != null)
                {
                    this.txtCodMittUsc.Text = protocolloUscita.mittente.codiceRubrica;
                    this.txtDescMittUsc.Text = protocolloUscita.mittente.descrizione;
                }
            }
        }


        /// <summary>
        /// Caricamento dati del corrispondente mittente nel protocollo in uscita semplificato
        /// </summary>
        private void FillDatiOggetto()
        {
            // Reperimento del documento corrente
            DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = this.GetProtocolloManager().GetDocumentoCorrente();

            if (currentDocument != null)
            {
                if (currentDocument.oggetto != null && currentDocument.oggetto.descrizione != null && currentDocument.oggetto.descrizione != string.Empty)
                {
                    this.ctrl_oggetto.oggetto_text = currentDocument.oggetto.descrizione.ToString();
                    //solo se il campo codice oggetto è attivo!!!
                    if (currentDocument.oggetto.codOggetto != null)
                    {
                        this.ctrl_oggetto.cod_oggetto_text = currentDocument.oggetto.codOggetto;
                    }
                }
                else
                {
                    this.ctrl_oggetto.oggetto_text = string.Empty;
                }
            }
        }

        /// <summary>
        /// Caricamento dati dei destinatari nei campi della UI
        /// </summary>
        private void FillDatiDestinatari()
        {
            // Reperimento del documento corrente
            DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = this.GetProtocolloManager().GetDocumentoCorrente();

            if (currentDocument != null)
            {
                DocsPAWA.DocsPaWR.ProtocolloUscita protocolloUscita = currentDocument.protocollo as DocsPAWA.DocsPaWR.ProtocolloUscita;

                if (protocolloUscita != null)
                {
                    setListBoxDestinatari(protocolloUscita);
                }
            }
        }

        private void txtCodMittente_TextChanged(object sender, System.EventArgs e)
        {
            string codiceMittente = this.txtCodMittente.Text;
            DocsPAWA.DocsPaWR.Corrispondente corrispondente = null;
            ElementoRubrica[] listaCorr = null;
            RubricaCallType calltype = new DocsPAWA.DocsPaWR.RubricaCallType();
            DocsPAWA.DocsPaWR.SchedaDocumento schDoc= this.GetProtocolloManager().GetDocumentoCorrente();
            if (Session["dictionaryCorrispondente"] != null)
            {
                dic_Corr = (Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>)Session["dictionaryCorrispondente"];
            }

            if (schDoc.tipoProto != "A")
            {
                calltype = RubricaCallType.CALLTYPE_PROTO_OUT;
            }
            else
            {
                calltype = RubricaCallType.CALLTYPE_PROTO_IN;
            }

            if (codiceMittente != string.Empty)
            {
                // Reperimento oggetto corrispondente dal codice immesso dall'utente
                //corrispondente = this.GetCorrispondenteDaCodice(codiceMittente);

                // corr = UserManager.GetCorrispondenteInterno(this, codiceRubrica, fineValidita, true);
               // calltype = RubricaCallType.CALLTYPE_PROTO_IN;
                //corr = UserManager.getCorrispondenteRubrica(this, codiceRubrica, calltype);
                listaCorr = UserManager.getElementiRubricaMultipli(this, codiceMittente, calltype, true);
                if (listaCorr == null || (listaCorr != null && listaCorr.Length == 0))
                {
                    this.txtCodMittente.Text = codiceMittente;
                    this.txtDescrMittente.Text = "";
                    this.RegisterClientScript("EnableButtonProtocollazione", "EnableButtonProtocollazione(true);");
                    this.RegisterClientScript("CodiceMittenteNonTrovato", "alert('Codice rubrica non trovato');");
                }
                else
                {
                    if (listaCorr != null && listaCorr.Length > 1)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "multiCorr", "ApriFinestraMultiCorrispondenti();", true);
                        Session.Add("multiCorr", listaCorr);
                        //Session["noRicercaDesc"] = true;
                        //Session["idCorrMulti"] = Convert.ToInt32(this.ID);
                        return;
                    }
                    else
                    {
                        if (listaCorr != null && listaCorr.Length == 1) // && !this.cbx_storicizzato.Checked)
                        {
                            DocsPAWA.DocsPaWR.ElementoRubrica er = listaCorr[0];
                            dic_Corr[this.ID] = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this.Page, er.codice, AddressbookTipoUtente.GLOBALE);

                            if (dic_Corr[this.ID] != null)
                            {
                                //this.ID_CORRISPONDENTE = er.systemId;
                                txtDescrMittente.Text = er.descrizione;
                                txtCodMittente.Text = er.codice;
                            }
                            else
                            {
                                //this.ID_CORRISPONDENTE = string.Empty;
                                txtDescrMittente.Text = string.Empty;
                                txtCodMittente.Text = string.Empty;
                            }
                            Session.Add("dictionaryCorrispondente", dic_Corr);
                            //Session["noRicercaDesc"] = true;
                        }
                    }

                    //    if (listaCorr.Length == 1)
                        //     {
                        //if (!string.IsNullOrEmpty(listaCorr[0].systemId))
                        //{
                        //    corrispondente = UserManager.getCorrispondenteBySystemID(this.Page, listaCorr[0].systemId);
                        //}
                        //else
                        //{
                        //    corrispondente = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, listaCorr[0].codice);
                        //}
                        //     }
                        //     else
                        //      {
                        //Da implementare
                        //  ClientScript.RegisterStartupScript(this.GetType(), "multiCorr", "ApriFinestraMultiCorrispondentiMittenti();", true);
                        // Session.Add("multiCorr", listaCorr);
                        //         return;
                        //     }
                    //}
                }

                //if (corrispondente == null)
                //{
                //    // Abilitazione pulsante protocollazione che, in caso
                //    // di errore, permette di rieseguire l'operazione
                //    // con gli stessi dati visualizzati
                //    this.RegisterClientScript("EnableButtonProtocollazione", "EnableButtonProtocollazione(true);");

                //    this.RegisterClientScript("CodiceMittenteNonTrovato", "alert('Codice rubrica non trovato');");
                //}
            }
            else
            {
                this.txtDescrMittente.Text = String.Empty;
            }
            // Impostazione oggetto corrispondente nel protocollo corrente
            this.SetCorrispondenteInProtocollo(corrispondente);

            // Caricamento dati del corrispondente nella UI
            this.FillDatiCorrispondente();

            //this.SetControlFocus("txtCodMittente");

            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();
            if (schedaDocumento.tipoProto == "A" && DocumentManager.isEnableMittentiMultipli())
            {
                if (((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente != null && !checkDuplicatiMittMultipli(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente, lbx_mittMultiplo))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "MittPresenteInMittMultipli", "alert('Il mittente è già presente nella lista mittenti multipli.');", true);
                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = false;
                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente = null;
                    this.txtCodMittente.Text = string.Empty;
                    this.txtDescrMittente.Text = string.Empty;
                }
            }
        }


        private void txtCodMittUsc_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                string codiceMittenteUscita = this.txtCodMittUsc.Text;
                if (codiceMittenteUscita.Trim() != string.Empty)
                {
                    setDescrizioneCorrispondenteUscita("Mit");
                }
                else
                {
                    this.txtDescMittUsc.Text = String.Empty;
                    SetCorrispondenteMittenteInProtocolloUscita(null);

                    //this.SetControlFocus("txtCodMittUsc");
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }

        }


        private void txtCodDestinatario_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                setDescrizioneCorrispondenteUscita("Dest");
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }
        }

        private void setDescrizioneCorrispondenteUscita(string tipoCorr)
        {
            string codiceRubrica = "";
            DocsPAWA.DocsPaWR.Corrispondente corr = null;
            string msg = "Codice rubrica non esistente";

            DocsPAWA.DocsPaWR.ElementoRubrica er;
            DocsPAWA.DocsPaWR.RubricaCallType calltype;

            if (tipoCorr.Equals("Dest"))
            {
                //caso del destinatario in uscita
                calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO;
                codiceRubrica = this.txtCodDest.Text;
            }
            else
            {
                //caso del mittente in uscita
                calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO;
                codiceRubrica = this.txtCodMittUsc.Text;
            }

            if (codiceRubrica.Trim() != string.Empty)
            {
                // Reperimento del documento corrente
                DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = this.GetProtocolloManager().GetDocumentoCorrente();

                DocsPAWA.DocsPaWR.ProtocolloUscita protocolloUscita = currentDocument.protocollo as DocsPAWA.DocsPaWR.ProtocolloUscita;

                if (tipoCorr == "Mit")
                {
                    string[] registri = new string[1];
                    registri[0] = GetRegistroCorrente().systemId;

                    UserManager.setListaIdRegistri(this, registri);

                    //finevalidità = true
                    corr = UserManager.GetCorrispondenteInterno(this, codiceRubrica, true, false);
                    UserManager.removeListaIdRegistri(this);

                    if (corr != null)
                    {
                        // Impostazione oggetto corrispondente nel protocollo corrente
                        this.SetCorrispondenteMittenteInProtocolloUscita(corr);

                        // Caricamento dati del corrispondente nella UI
                        this.FillDatiCorrispondenteMittenteInUscita();

                        //this.SetControlFocus("txtCodMittUsc");
                    }
                    else
                    {
                        codice_non_trovato(msg, ref txtCodMittUsc, ref txtDescMittUsc);
                        this.txtCodMittUsc.Text = String.Empty;

                    }
                }
                else
                {
                    if (tipoCorr.Equals("Dest"))
                    {
                        //se lo smistamento è abilitato
                        if (Utils.getAbilitazioneSmistamento().Equals("1"))
                        {
                            DocsPAWA.DocsPaWR.SmistamentoRubrica smistaRubrica = creaSmistamentoRubrica(calltype);
                            if ((er = UserManager.getElementoRubrica(this, codiceRubrica, smistaRubrica, "")) != null)
                            {
                                if (!er.isVisibile)
                                {
                                    msg = "Codice rubrica non utilizzabile in questo contesto";
                                    codice_non_trovato(msg, ref txtCodDest, ref txtDescrDest);

                                    //btn_aggiungiDest_P.Enabled = false;
                                    return;
                                }
                            }
                        }


                        //Si verifica se il codice appartiene a una lista di distribuzione
                        ArrayList lsCorr = new ArrayList();
                        if (System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null && System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "1")
                        {
                            string idAmm = UserManager.getInfoUtente().idAmministrazione;
                            lsCorr = UserManager.getCorrispondentiByCodLista(this, codiceRubrica,idAmm);
                            if (lsCorr.Count != 0)
                            {
                                corr = new DocsPAWA.DocsPaWR.Corrispondente();
                                corr.codiceRubrica = codiceRubrica;
                                corr.descrizione = UserManager.getNomeLista(this, codiceRubrica,idAmm);
                                corr.tipoCorrispondente = "L";
                            }
                            else
                            {
                                corr = UserManager.getCorrispondenteRubrica(this, codiceRubrica, calltype);
                            }
                        }
                        else
                        {
                            corr = UserManager.getCorrispondenteRubrica(this, codiceRubrica, calltype);
                        }


                        //aggiunge il corrispondente tra i destinatari
                        if (corr != null)
                        {

                            inserisciDestinatarioAutomatico(corr, currentDocument);
                            this.txtCodDest.Text = "";
                        }
                        else
                        {
                            codice_non_trovato(msg, ref txtCodDest, ref txtDescrDest);
                        }
                    }
                }
            }
        }

        #region metodi per la digitazione del codice rubrica

        private DocsPAWA.DocsPaWR.SmistamentoRubrica creaSmistamentoRubrica(DocsPAWA.DocsPaWR.RubricaCallType calltype)
        {
            //popolamento oggetto per RUBRICA_USA_PROTO_SMISTAMENTO
            DocsPAWA.DocsPaWR.SmistamentoRubrica smistamentoRubrica = new DocsPAWA.DocsPaWR.SmistamentoRubrica();
            //smistamentoRubrica.smistamento: indica se è abilitata o meno lo smistamento
            smistamentoRubrica.smistamento = DocsPAWA.Utils.getAbilitazioneSmistamento();

            //callType
            smistamentoRubrica.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO;

            //InfoUtente
            DocsPAWA.DocsPaWR.InfoUtente infoUt = new DocsPAWA.DocsPaWR.InfoUtente();
            infoUt = UserManager.getInfoUtente(this);
            smistamentoRubrica.infoUt = infoUt;

            //Ruolo Protocollatore
            if (Session["userRuolo"] != null)
            {
                smistamentoRubrica.ruoloProt = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
            }

            if (this.GetRegistroCorrente() != null)
            {
                //Registro corrente
                smistamentoRubrica.idRegistro = this.GetRegistroCorrente().systemId;
            }
            return smistamentoRubrica;
        }
        #endregion

        void codice_non_trovato(string msg, ref TextBox tbxCod, ref TextBox tbxDesc)
        {
            Response.Write("<script>alert(\"" + msg + "\");</script>");
            //string s = "<SCRIPT language='javascript'>document.getElementById('" + tbxCod.ID + "').focus() </SCRIPT>";
            //RegisterStartupScript("focus", s);

            tbxCod.Text = "";
            tbxDesc.Text = "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDescrMittente_TextChanged(object sender, System.EventArgs e)
        {
            try
            {

                DocsPAWA.DocsPaWR.Corrispondente corr = null;
                DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = this.GetProtocolloManager().GetDocumentoCorrente();
                DocsPAWA.DocsPaWR.ProtocolloEntrata protocolloEntrata = currentDocument.protocollo as DocsPAWA.DocsPaWR.ProtocolloEntrata;

                corr = protocolloEntrata.mittente;

                string descrizione = this.txtDescrMittente.Text;

                if (corr == null || (descrizione.Trim() != "" && !descrizione.Trim().Equals(corr.descrizione)))
                {
                    corr = new DocsPAWA.DocsPaWR.Corrispondente();

                    corr.descrizione = descrizione;

                    corr.tipoCorrispondente = "O";

                    corr.idAmministrazione = (UserManager.getInfoUtente(this)).idAmministrazione;



                    /* vuol dire che sto creando un occasionale dalla popUp dettagliCorrisponenti.aspx */

                    if (Session["dettagliCorr.corrInfo"] != null)
                    {
                        corr.info = Session["dettagliCorr.corrInfo"] as DocsPaVO.addressbook.DettagliCorrispondente;
                        Session.Remove("dettagliCorr.corrInfo");
                    }

                    protocolloEntrata.mittente = corr;

                    if (currentDocument.systemId != null)
                    {
                        protocolloEntrata.daAggiornareMittente = true;
                    }

                }

            }

            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }

        }

        private void txtDescMittUsc_TextChanged(object sender, System.EventArgs e)
        {
            try
            {

                DocsPAWA.DocsPaWR.Corrispondente corr = null;
                DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = this.GetProtocolloManager().GetDocumentoCorrente();
                DocsPAWA.DocsPaWR.ProtocolloUscita protocolloUscita = currentDocument.protocollo as DocsPAWA.DocsPaWR.ProtocolloUscita;

                corr = protocolloUscita.mittente;

                string descrizione = this.txtDescMittUsc.Text;

                if (corr == null || (descrizione.Trim() != "" && !descrizione.Trim().Equals(corr.descrizione)))
                {
                    corr = new DocsPAWA.DocsPaWR.Corrispondente();

                    corr.descrizione = descrizione;

                    corr.tipoCorrispondente = "O";

                    corr.idAmministrazione = (UserManager.getInfoUtente(this)).idAmministrazione;



                    /* vuol dire che sto creando un occasionale dalla popUp dettagliCorrisponenti.aspx */

                    if (Session["dettagliCorr.corrInfo"] != null)
                    {
                        corr.info = Session["dettagliCorr.corrInfo"] as DocsPaVO.addressbook.DettagliCorrispondente;
                        Session.Remove("dettagliCorr.corrInfo");
                    }

                    protocolloUscita.mittente = corr;

                    if (currentDocument.systemId != null)
                    {
                        protocolloUscita.daAggiornareMittente = true;
                    }

                }

            }

            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }

        }

        private void ctrl_oggetto_OggChanged(object sender, DocsPAWA.documento.Oggetto.OggTextEvent e)
        {
            // Creazione nuovo oggetto
            if (!_oggettoSelezionatoDaOggettario)
            {
                this.CreateNewOggetto();
            }

        }

        #endregion

        #region Gestione stampa etichetta

        private void btnPrintSignature_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (this.GetStatoDocumento() == StatoDocumentoEnum.NonProtocollato)
                this.RegisterClientScript("AlertPrintSignature", "alert('Documento non ancora protocollato');");
            else
                this.PrintSignature();
        }

        private void PrintSignature()
        {
            ProtocollazioneIngresso.StampaEtichetta stampaEtichetta = this.GetControlStampaEtichetta();
            stampaEtichetta.Stampa(this.GetProtocolloManager().GetDocumentoCorrente(), false,this.txt_num_stampe.Text);
        }

        private ProtocollazioneIngresso.StampaEtichetta GetControlStampaEtichetta()
        {
            return (ProtocollazioneIngresso.StampaEtichetta)this.FindControl("StampaEtichetta");
        }

        #endregion

        #region Gestione Fascicolatura Rapida
        /// <summary>
        /// Metodo per inserire la descrizione del fascicolo nel campo descrizione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_cod_fasc_TextChanged(object sender, System.EventArgs e)
        {
            //inizialmente svuoto il campo e pulisco la sessione
            FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
            FascicoliManager.removeCodiceFascRapida(this);
            FascicoliManager.removeDescrizioneFascRapida(this);

            this.txt_DescFascicolo.Text = "";
            Session["validCodeFasc"] = "true";

            if (this.txt_CodFascicolo.Text.Equals(""))
            {
                txt_DescFascicolo.Text = "";
                return;
            }


            //FASCICOLAZIONE IN SOTTOFASCICOLI

            if (this.txt_CodFascicolo.Text.IndexOf("//") > -1)
            {
                #region FASCICOLAZIONE IN SOTTOFASCICOLI
                string codice = string.Empty;
                string descrizione = string.Empty;
                
                DocsPAWA.DocsPaWR.Fascicolo SottoFascicolo = getFolder(UserManager.getRegistroSelezionato(this), ref codice, ref descrizione);
                if (SottoFascicolo != null)
                {

                    if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                    {
                        txt_DescFascicolo.Text = descrizione;
                        txt_CodFascicolo.Text = codice;
                        FascicoliManager.setCodiceFascRapida(this, codice);
                        FascicoliManager.setDescrizioneFascRapida(this, descrizione);
                        FascicoliManager.setFascicoloSelezionatoFascRapida(this, SottoFascicolo);
                    }
                    else
                    {
                        //caso 0: al codice digitato non corrisponde alcun fascicolo

                        Session["validCodeFasc"] = "false";
                        Page.RegisterStartupScript("", "<script>alert('Attenzione, sottofascicolo non presente')</script>");
                        this.txt_DescFascicolo.Text = "";
                        this.txt_CodFascicolo.Text = "";
                    }
                }
                else
                {
                    Session["validCodeFasc"] = "false";
                    Page.RegisterStartupScript("", "<script>alert('Attenzione, sottofascicolo non presente')</script>");
                    this.txt_DescFascicolo.Text = "";
                    this.txt_CodFascicolo.Text = "";
                }

                #endregion
            }
            else
            {
                DocsPAWA.DocsPaWR.Fascicolo[] listaFasc = getFascicoli(UserManager.getRegistroSelezionato(this));

                if (listaFasc != null)
                {
                    if (listaFasc.Length > 0)
                    {
                        //caso 1: al codice digitato corrisponde un solo fascicolo
                        if (listaFasc.Length == 1)
                        {
                            txt_DescFascicolo.Text = listaFasc[0].descrizione;
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                codClassifica = listaFasc[0].codice;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPAWA.DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            FascicoliManager.setFascicoloSelezionatoFascRapida(this, listaFasc[0]);
                        }
                        else
                        {
                            //caso 2: al codice digitato corrispondono piu fascicoli
                            Session.Add("listaFascFascRapida", listaFasc);
                            codClassifica = this.txt_CodFascicolo.Text;
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                codClassifica = codClassifica;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPAWA.DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codClassifica + "', 'Y')</script>");
                            return;
                        }
                    }
                    else
                    {
                        //caso 0: al codice digitato non corrisponde alcun fascicolo
                        if (listaFasc.Length == 0)
                        {
                            Session["validCodeFasc"] = "false";
                            Page.RegisterStartupScript("", "<script>alert('Attenzione, codice fascicolo non presente')</script>");
                            this.txt_DescFascicolo.Text = "";
                            this.txt_CodFascicolo.Text = "";
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Metodo per il recupero del sottofascicolo da codice fascicolo e descrizione sottofascicolo
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo getFolder(DocsPAWA.DocsPaWR.Registro registro, ref string codice, ref string descrizione)
        {
            DocsPAWA.DocsPaWR.Folder[] listaFolder = null;
            DocsPAWA.DocsPaWR.Fascicolo fasc = null;
            string separatore = "//";
            int posSep = this.txt_CodFascicolo.Text.IndexOf("//");
            if (this.txt_CodFascicolo.Text != string.Empty && posSep > -1)
            {

                string codiceFascicolo = txt_CodFascicolo.Text.Substring(0, posSep);
                string descrFolder = txt_CodFascicolo.Text.Substring(posSep + separatore.Length);

                listaFolder = FascicoliManager.getListaFolderDaCodiceFascicolo(this, codiceFascicolo, descrFolder, registro);
                if (listaFolder != null && listaFolder.Length > 0)
                {

                    //calcolo fascicolazionerapida
                    fasc = FascicoliManager.getFascicoloById(this, listaFolder[0].idFascicolo);

                    if (fasc != null)
                    {
                        //folder selezionato è l'ultimo
                        fasc.folderSelezionato = listaFolder[listaFolder.Length - 1];
                    }
                    codice = fasc.codice + separatore;
                    descrizione = fasc.descrizione + separatore;
                    for (int i = 0; i < listaFolder.Length; i++)
                    {
                        codice += listaFolder[i].descrizione + "/";
                        descrizione += listaFolder[i].descrizione + "/";
                    }
                    codice = codice.Substring(0, codice.Length - 1);
                    descrizione = descrizione.Substring(0, descrizione.Length - 1);
                }
            }
            if (fasc != null)
            {
                return fasc;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Metodo per il recupero dei fascicoli da codice
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo[] getFascicoli(DocsPAWA.DocsPaWR.Registro registro)
        {
            DocsPAWA.DocsPaWR.Fascicolo[] listaFasc = null;
            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                string codiceFascicolo = txt_CodFascicolo.Text;
                listaFasc = FascicoliManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");
            }
            if (listaFasc != null)
            {
                return listaFasc;
            }
            else
            {
                return null;
            }
        }

        private void imgFasc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            /*DocsPAWA.DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
            if (fasc != null)
            {
                if (this.txt_CodFascicolo.Text != "" && this.txt_DescFascicolo.Text != "")
                {
                    if (fasc.tipo.Equals("G"))
                    {
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + fasc.codice + "')</script>");
                    }
                    else
                    {
                        ///se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                        DocsPAWA.DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, fasc.idClassificazione, UserManager.getUtente(this).idAmministrazione);
                        string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "')</script>");
                    }
                }
            }
            else
            {
  */
            if (this.txt_CodFascicolo.Text != "")
            {
                DocsPAWA.DocsPaWR.Fascicolo fascSel = getFascicolo();
                if (fascSel != null)
                {
                    FascicoliManager.setFascicoloSelezionatoFascRapida(this, fascSel);
                    if (fascSel.tipo.Equals("G"))
                    {
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + fascSel.codice + "')</script>");
                    }
                    else
                    {
                        ///se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                        DocsPAWA.DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, fascSel.idClassificazione, UserManager.getUtente(this).idAmministrazione);
                        string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "')</script>");
                    }
                }
            }
            else
            {
                // if (!(Session["validCodeFasc"] != null && Session["validCodeFasc"].ToString() == "false"))
                RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + txt_CodFascicolo.Text + "')</script>");
            }
            //            }
        }

        //controllo per la creazione di un nuovo fascicolo procedimentale e la sua successiva selezione 
        //auomatica all'interno della form di fascicolazione rapida
        private void imgFascNew_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            regSelezionato = UserManager.getRegistroSelezionato(this);
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPaWebService();
            //DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = this.GetProtocolloManager().GetDocumentoCorrente();
            //Session["protoSempl"] = currentDocument;

            ArrayList listaTitolari = new ArrayList(wws.getTitolariUtilizzabili(UserManager.getUtente(this).idAmministrazione));
            
            if(regSelezionato.Sospeso)
            {
                RegisterClientScriptBlock("alertRegistroSospeso", "<SCRIPT language='javascript'>alert('Il registro selezionato è sospeso!');</SCRIPT>");
                return;
            }


            string profilazione = System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"];
            if (this.txt_CodFascicolo.Text.Trim().Equals(""))
            {
                RegisterClientScriptBlock("alertCodiceInesistente", "<SCRIPT language='javascript'>alert('Inserire un nodo di titolario!');</SCRIPT>");
            }


            if (!this.txt_CodFascicolo.Text.Trim().Equals(""))
            {

                //ClientScript.RegisterStartupScript(this.GetType(), "apreGestNew", "ApriFinestraNewFascNewTit('" + Server.UrlEncode(this.txt_codClass.Text) + "','ricercaFascicoli','fascNewFascicolo.aspx','" + profilazione + "','"+ddl_titolari.SelectedValue+"');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "apreGestNew", "ApriFinestraNewFascNewTit('" + Server.UrlEncode(this.txt_CodFascicolo.Text) + "','protoSempl','fascNewFascicolo.aspx','" + profilazione + "','" + getIdTitolario(this.txt_CodFascicolo.Text, listaTitolari) + "');", true);


            }
        }

        /// <summary>
        /// Metodo per il recupero del fascicolo da codice
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo getFascicolo()
        {

            DocsPAWA.DocsPaWR.Fascicolo fascicoloSelezionato = null;
            string codiceFascicolo = string.Empty;
            string descrizione = string.Empty;
            if (Request.QueryString["protocolla"] != null && Request.QueryString["protocolla"].Equals("1"))
                // this.txt_CodFascicolo.Text = FascicoliManager.getFascicoloSelezionatoFascRapida(this.Page).codice;
                setFascicolazioneRapida();

            string codFasc = Request.QueryString["codFasc"];
            if (!string.IsNullOrEmpty(codFasc))
                this.txt_CodFascicolo.Text = codFasc;

            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                if (this.txt_CodFascicolo.Text.IndexOf("//") > -1)
                    fascicoloSelezionato = getFolder(UserManager.getRegistroSelezionato(this), ref codiceFascicolo, ref descrizione);
                else
                {
                    codiceFascicolo = txt_CodFascicolo.Text;

                    //OLD:   Fascicolo Fasc = FascicoliManager.getFascicoloDaCodice(this,);
                    Fascicolo[] FascS = FascicoliManager.getListaFascicoliDaCodice(this, codiceFascicolo, null, "I");

                    if (FascS != null && FascS.Length > 0 && FascS[0] != null)
                    {
                        fascicoloSelezionato = (Fascicolo)FascS[0];
                    }

                    //fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice(this, codiceFascicolo);

                }
            }
            if (fascicoloSelezionato != null)
            {
                return fascicoloSelezionato;
            }
            else
            {
                return null;
            }
            //DocsPAWA.DocsPaWR.Fascicolo fascicoloSelezionato = null;
            //if (!this.txt_CodFascicolo.Text.Equals(""))
            //{
            //    string codiceFascicolo = txt_CodFascicolo.Text;
            //    fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice(this, codiceFascicolo);
            //}
            //if (fascicoloSelezionato != null)
            //{
            //    return fascicoloSelezionato;
            //}
            //else
            //{
            //    return null;
            //}
        }

        #endregion


      

        public void setFascicolazioneRapida()
        {
            DocsPAWA.DocsPaWR.Fascicolo fascRap = new Fascicolo();
            if (isRiproposto)
            {
                //if (Session["FascicoloRipr"] != null)
                if (FascicoliManager.GetDoFascRapida(this) != null)
                {
                    //fascRap = (DocsPaWR.Fascicolo)Session["FascicoloRipr"];
                    fascRap = FascicoliManager.GetDoFascRapida(this);
                    if (fascRap != null)
                    {
                        FascicoliManager.setFascicoloSelezionatoFascRapida(fascRap);
                        FascicoliManager.setCodiceFascRapida(Page, fascRap.codice);
                        FascicoliManager.setDescrizioneFascRapida(Page, fascRap.descrizione);
                    }
                }
            }
            else
                fascRap = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
            string codiceFascRapida = FascicoliManager.getCodiceFascRapida(this);
            string descrFascRapida = FascicoliManager.getDescrizioneFascRapida(this);

            if (fascRap != null)
            {
                if (fascRap.folderSelezionato != null && codiceFascRapida != string.Empty && descrFascRapida != string.Empty)
                {
                    //this.txt_CodFascicolo.Text = fascRap.codice + "//" + fascRap.folderSelezionato.descrizione;
                    //this.txt_DescFascicolo.Text = fascRap.folderSelezionato.descrizione;
                    this.txt_CodFascicolo.Text = codiceFascRapida;
                }
                else
                {
                    this.txt_CodFascicolo.Text = fascRap.codice;
                    this.txt_DescFascicolo.Text = fascRap.descrizione;
                }
            }
            else
            {
                //Vero
                this.txt_CodFascicolo.Text = "";
                this.txt_DescFascicolo.Text = "";
                //this.riproponi();
            }

            //setto la tooltip del fascicolo
            this.txt_DescFascicolo.ToolTip = txt_DescFascicolo.Text;
        }
        /// <summary>
        /// Visualizzazione usercontrol accesso negato
        /// </summary>
        /// <param name="message"></param>
        private void ShowMessageAccessoNegato(string message)
        {
            this.tblContainer.Visible = false;

            AccessoNegato accessoNegato = (AccessoNegato)
                this.Page.FindControl("frmProtIngresso").FindControl("AccessoNegato");
            accessoNegato.Visible = true;
            accessoNegato.SetMessage(message);
        }

        /// <summary>
        /// Verifica se l'utente corrente è autorizzato
        /// alla protocollazione in ingresso
        /// </summary>
        /// <returns></returns>
        private bool IsUtenteAutorizzato()
        {
            ProtocollazioneIngresso.Login.LoginMng loginMng = new ProtocollazioneIngresso.Login.LoginMng(this);
            return loginMng.IsUtenteAutorizzato();
        }


        /// <summary>
        /// Verifica se l'utente corrente è autorizzato
        /// alla protocollazione in uscita semplificata
        /// </summary>
        /// <returns></returns>
        private bool IsUtenteAutorizzatoUscita()
        {
            ProtocollazioneIngresso.Login.LoginMng loginMng = new ProtocollazioneIngresso.Login.LoginMng(this);
            return loginMng.IsUtenteAutorizzatoUscita();
        }

        #region Gestione acquisizione e upload di documenti

        /// <summary>
        /// Caricamento dal db del flag relativo
        /// alla modalità di acquisizione documenti
        /// </summary>
        private void LoadModalitaAcquisizioneDocumenti(string idAmm)
        {
            string valoreChiaveDB = DocsPAWA.utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_PROTOIN_ACQ_DOC_OBBLIGATORIA");

            //Il default è TRUE, incaso di stringa nulla o mancante, altrimenti è il valore imposto
            this.txtModAcquisizione.Value = (valoreChiaveDB != null && valoreChiaveDB != string.Empty) ? valoreChiaveDB : "true";
        }
        /// <summary>
        /// Reperimento ed impostazione delle configurazioni del file web.config
        /// </summary>
        private void SetWebConfigConfigurations()
        {
            /// (se standard o in pdf con l'integrazione di acrobat 7).
            /// - Il flag "ADOBE_ACROBAT_INTEGRATION" determina se 
            ///   è attiva la conversione in PDF mediante l'SDK di Acrobat.
            ///   Se false, i 2 flag successivi non vengono presi in considerazione
            ///   e la scansione documenti è standard.
            /// - Il flag "DOCUMENT_PDF_CONVERT" rappresenta il valore 
            ///   di default del check "chkConvertiPDF".
            /// - Il flag "DOCUMENT_PDF_CONVERT_ENABLED" stabilisce 
            ///   se deve essere abilitato o meno il check "chkConvertiPDF".
            bool documentPDFConvert = false;
            bool documentPDFConvertEnabled = false;
            bool documentoPDFConverterServer = false;

            if (!this.IsPostBack)
            {
                try
                {
                    // Reperimento valore flag "DOCUMENT_PDF_CONVERT"
                    documentPDFConvert = Convert.ToBoolean(DocsPAWA.ConfigSettings.getKey(DocsPAWA.ConfigSettings.KeysENUM.DOCUMENT_PDF_CONVERT));
                    documentPDFConvertEnabled = Convert.ToBoolean(DocsPAWA.ConfigSettings.getKey(DocsPAWA.ConfigSettings.KeysENUM.DOCUMENT_PDF_CONVERT_ENABLED));
                    documentoPDFConverterServer = Convert.ToBoolean(DocsPAWA.Utils.isEnableConversionePdfLatoServer());
                }
                catch
                {
                }

                this.txtPDFConvert.Value = documentPDFConvert.ToString().ToLower();
                this.txtPDFConvertEnabled.Value = documentPDFConvertEnabled.ToString().ToLower();
                this.txtPDFConvertServer.Value = documentoPDFConverterServer.ToString().ToLower();

                //Cambio la descrizione della label se è attiva la conversione PDF lato server
                if (documentoPDFConverterServer)
                    chkConvertiPDF.Text = "Converti in PDF lato server";                       
            }
            else
            {

            }

            this.RegisterClientScript("SetValueCheckConvertiPDF", "SetValueCheckConvertiPDF();");

            // Attivazione componenti smartclient
            this.txtSmartClientActivation.Value = DocsPAWA.SmartClient.Configurations.IsActive().ToString().ToLower();
        }

        /// <summary>
        /// Gestione registrazione usercontrol per la scansione documenti
        /// mediante i componenti smartclient
        /// </summary>
        private void RegisterControlAcquisizioneSmartClient()
        {
            if (this.txtSmartClientActivation.Value == "true")
            {
                Scansione.AcquisizioneSmartClient smartClientAcquireCtl = (Scansione.AcquisizioneSmartClient)this.LoadControl("Scansione/AcquisizioneSmartClient.ascx");
                smartClientAcquireCtl.ID = "smartClientAcquireCtl";

                HtmlForm form = (HtmlForm)this.FindControl("frmProtIngresso");
                form.Controls.Add(smartClientAcquireCtl);
            }
        }

        /// <summary>
        /// Gestione registrazione usercontrol per il reperimento
        /// delle impostazioni riguardanti la conversione in pdf
        /// </summary>
        private void RegisterControlPdfCapabilities()
        {
            HtmlForm form = (HtmlForm)this.FindControl("frmProtIngresso");
            Control control = null;

            //if (this.txtSmartClientActivation.Value == "true")
            //{
            //    control = this.LoadControl("PdfCapabilitiesSmartClient.ascx");
            //    control.ID = "smartClientPdfCapabilities";
            //}
            //else
            //{
                control = this.LoadControl("PdfCapabilities.ascx");
                control.ID = "pdfCapabilities";
            //}

            form.Controls.Add(control);
        }

        /// <summary>
        /// Ripristiono valore check "chkConvertiPDF" in base al valore
        /// del campo nascosto "txtPDFConvert"
        /// </summary>
        private void RestoreValueCheckConvertiPDF()
        {
            this.chkConvertiPDF.Checked = Convert.ToBoolean(this.txtPDFConvert.Value);
        }

        /// <summary>
        /// Verifica se è obbligatorio acquisire documenti
        /// </summary>
        /// <returns></returns>
        private bool IsAcquisizioneDocumentiObbligatoria()
        {
            return Convert.ToBoolean(this.txtModAcquisizione.Value);
        }

        /// <summary>
        /// Reperimento URL dell'applicazione web, necessario per l'upload dei documenti
        /// (vi si fa riferimento nel codice HTML della pagina)
        /// </summary>
        /// <returns></returns>
        public string GetHttpFullPath()
        {
            return DocsPAWA.Utils.getHttpFullPath(this);
        }

        #endregion

        /// <summary>
        /// Inserisce i corrispondenti selezionati dalla Rubrica nella corrispondente ListBox
        /// </summary>
        /// <param name="protocolloUscita"></param>
        private void setListBoxDestinatari(DocsPAWA.DocsPaWR.ProtocolloUscita protocolloUscita)
        {
            //Valido per i documenti in Partenza
            DocsPAWA.DocsPaWR.Corrispondente destinatario;
            this.lbx_dest.Items.Clear();
            this.lbx_destCC.Items.Clear();
            if ((protocolloUscita).destinatari != null)
            {
                for (int i = 0; i < (protocolloUscita).destinatari.Length; i++)
                {

                    destinatario = ((protocolloUscita).destinatari[i]);

                    this.lbx_dest.Items.Add(destinatario.descrizione);

                    this.lbx_dest.Items[i].Value = destinatario.codiceRubrica;

                }
            }

            if ((protocolloUscita).destinatariConoscenza != null)
            {
                for (int i = 0; i < (protocolloUscita).destinatariConoscenza.Length; i++)
                {
                    destinatario = ((protocolloUscita).destinatariConoscenza[i]);

                    this.lbx_destCC.Items.Add(destinatario.descrizione);

                    this.lbx_destCC.Items[i].Value = destinatario.codiceRubrica;
                }

            }

        }

        /// <summary>
        /// Inserisce un destinatario tra i destinatari per conoscenza e lo rimuove tra i destinatari
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_insDest_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

            try
            {

                if (this.lbx_destCC.SelectedIndex >= 0)
                {
                    //cerca il corrispondente e lo aggiunge tra i destinatari principali

                    addDestinatari(this.lbx_destCC.SelectedIndex, "P");

                    // protocolloMng.setDatiProtocolloUscita();
                }
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }
        }

        /// <summary>
        /// Inserisce un destinatario tra i destinatari per conoscenza e lo rimuove tra i destinatari principali
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_insDestCC_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

            try
            {

                if (this.lbx_dest.SelectedIndex >= 0)
                {
                    addDestinatari(this.lbx_dest.SelectedIndex, "C");
                }
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }
        }

        private void addDestinatari(int index, string tipoDest)
        {
            //controlo se esiste già il corrispondente selezionato

            DocsPAWA.DocsPaWR.Corrispondente[] listaDest;
            DocsPAWA.DocsPaWR.Corrispondente[] listaDestCC;
            DocsPAWA.DocsPaWR.Corrispondente corr;

            DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = this.GetProtocolloManager().GetDocumentoCorrente();
            DocsPAWA.DocsPaWR.ProtocolloUscita protocolloUscita = currentDocument.protocollo as DocsPAWA.DocsPaWR.ProtocolloUscita;
            ProtocollazioneIngresso.Protocollo.ProtocolloMng protocolloMng = this.GetProtocolloManager();


            listaDest = (protocolloUscita).destinatari;
            listaDestCC = (protocolloUscita).destinatariConoscenza;

            string itemDaRimuovere = "";
            //aggiungo il corrispondente
            if (tipoDest.Equals("P"))
            {
                corr = listaDestCC[index];
                (protocolloUscita).destinatari = UserManager.addCorrispondente(listaDest, corr);

                removeDestinatari(index, "C", currentDocument);

            }
            else
            {
                if (tipoDest.Equals("C"))
                {
                    corr = listaDest[index];
                    (protocolloUscita).destinatariConoscenza = UserManager.addCorrispondente(listaDestCC, corr);

                    //rimuovo il destinatario in CC che è stato aggiunto tra i destinatari in TO
                    removeDestinatari(index, "P", currentDocument);

                }
            }
            lbx_dest.Items.Clear();
            lbx_destCC.Items.Clear();
            setListBoxDestinatari(protocolloUscita);


            //Setto la nuova scheda in sessione
            protocolloMng.setDatiProtocolloUscita(currentDocument);

        }

        private void removeDestinatari(int index, string tipoDest, DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento)
        {
            DocsPAWA.DocsPaWR.Corrispondente[] listaDest;

            if (tipoDest.Equals("P"))
            {
                listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari;
                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari = UserManager.removeCorrispondente(listaDest, index);

            }
            else
                if (tipoDest.Equals("C"))
                {
                    listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza;
                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza = UserManager.removeCorrispondente(listaDest, index);
                }

        }

        #region btn_aggiungiDest_P_Click
        /// <summary>
        /// Aggiunge un destinatario occasionale nella listBox dei destinatari
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btn_aggiungiDest_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            aggiungiDestinatariDaCodice();
        }

        private void aggiungiDestinatariDaCodice()
        {
            //cerca il corrispondente e lo aggiunge tra i destinatari principali
            DocsPAWA.DocsPaWR.Corrispondente corr = null;

            DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = this.GetProtocolloManager().GetDocumentoCorrente();
            DocsPAWA.DocsPaWR.ProtocolloUscita protocolloUscita = currentDocument.protocollo as DocsPAWA.DocsPaWR.ProtocolloUscita;
            ProtocollazioneIngresso.Protocollo.ProtocolloMng protocolloMng = this.GetProtocolloManager();

            if (this.txtDescrDest.Text != null && !this.txtDescrDest.Text.Trim().Equals(""))
            {
                //se ho digitato un corrispondente occasionale
                corr = new DocsPAWA.DocsPaWR.Corrispondente();
                corr.descrizione = this.txtDescrDest.Text;
                corr.tipoCorrispondente = "O";
                corr.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
            }
            if (corr != null)
            {
                DocsPAWA.DocsPaWR.Corrispondente[] listaDestCC = (protocolloUscita).destinatariConoscenza;
                if (!UserManager.esisteCorrispondente(listaDestCC, corr))
                {
                    DocsPAWA.DocsPaWR.Corrispondente[] listaDest = (protocolloUscita).destinatari;
                    if (!UserManager.esisteCorrispondente(listaDest, corr))
                    {
                        (protocolloUscita).destinatari = UserManager.addCorrispondenteModificato(listaDest, listaDestCC, corr);

                        this.lbx_dest.Items.Add(corr.descrizione);
                        protocolloMng.setDatiProtocolloUscita(currentDocument);

                    }
                }

            }

            this.txtCodDest.Text = "";
            this.txtDescrDest.Text = "";

            //string s = "<SCRIPT language='javascript'>document.getElementById('" + txtCodDest.ID + "').focus() </SCRIPT>";
            //RegisterStartupScript("focus", s);
        }

        private void inserisciDestinatarioAutomatico(DocsPAWA.DocsPaWR.Corrispondente corr, DocsPAWA.DocsPaWR.SchedaDocumento currentDocument)
        {
            if (corr != null)
            {
                DocsPAWA.DocsPaWR.ProtocolloUscita protocolloUscita = (DocsPAWA.DocsPaWR.ProtocolloUscita)(currentDocument.protocollo);
                DocsPAWA.DocsPaWR.Corrispondente[] listaDestCC = (protocolloUscita).destinatariConoscenza;
                ProtocollazioneIngresso.Protocollo.ProtocolloMng protocolloMng = this.GetProtocolloManager();
                if (!UserManager.esisteCorrispondente(listaDestCC, corr))
                {
                    DocsPAWA.DocsPaWR.Corrispondente[] listaDest = (protocolloUscita).destinatari;
                    if (!UserManager.esisteCorrispondente(listaDest, corr))
                    {
                        (protocolloUscita).destinatari = UserManager.addCorrispondenteModificato(listaDest, listaDestCC, corr);
                    }
                    setListBoxDestinatari(protocolloUscita);
                }

            }
        }

        /// <summary>
        /// Effettua la trasmissione ai destinatari del protocollo in uscita
        /// </summary>
        /// <param name="isEnableUffRef"></param>
        private void completaProtocollazione(DocsPAWA.DocsPaWR.SchedaDocumento currentDocument)
        {
            string message = "";

            if (currentDocument.systemId != null)
            {
                if (currentDocument.tipoProto == "P")
                {
                    string serverName = Utils.getHttpFullPath(this);
                    bool verificaRagioni;

                    if (estendiVisibilita.Value == "false")
                    {
                        currentDocument.eredita = "0";
                    }

                    if (DocumentManager.TrasmettiProtocolloInterno(this, serverName, currentDocument, false, out verificaRagioni, out message))
                    {
                        if (!verificaRagioni)
                        {
                            // Notifica utente che la trasmissione non e' stata effettuata
                            string theAlert = "<script>alert('Attenzione! le trasmissioni non sono state effettuate poiché non sono presenti\\nle ragioni di trasmissione per: " + message + "');</script>";
                            Response.Write(theAlert);
                        }
                    }
                    else
                    {
                        Exception exception = new Exception("Errore durante la trasmissione del protocollo.");
                        ErrorManager.redirect(this, exception, "protocollazione");
                    }
                }
            }
        }

        /// <summary>
        /// Rimuove un destinatario in TO dalla lista dei destinatari 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_cancDest_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

            if (this.lbx_dest.SelectedIndex >= 0)
            {
                DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = this.GetProtocolloManager().GetDocumentoCorrente();
                DocsPAWA.DocsPaWR.ProtocolloUscita protocolloUscita = currentDocument.protocollo as DocsPAWA.DocsPaWR.ProtocolloUscita;

                removeDestinatari(this.lbx_dest.SelectedIndex, "P", currentDocument);
                //lbx_dest.Items.Clear();
                setListBoxDestinatari(protocolloUscita);

            }
            else
            {
                string msg = "Destinatario non selezionato";
                Response.Write("<script>alert('" + msg + "')</script>");
            }
        }

        /// <summary>
        /// Rimuove un destinatario in TO dalla lista dei destinatari 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_cancDestCC_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

            if (this.lbx_destCC.SelectedIndex >= 0)
            {
                DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = this.GetProtocolloManager().GetDocumentoCorrente();
                DocsPAWA.DocsPaWR.ProtocolloUscita protocolloUscita = currentDocument.protocollo as DocsPAWA.DocsPaWR.ProtocolloUscita;

                removeDestinatari(this.lbx_destCC.SelectedIndex, "C", currentDocument);

                setListBoxDestinatari(protocolloUscita);

            }
            else
            {
                string msg = "Destinatario non selezionato";
                Response.Write("<script>alert('" + msg + "')</script>");
            }
        }
        #endregion

        protected void btn_Correttore_Click(object sender, ImageClickEventArgs e)
        {
            this.ctrl_oggetto.SpellCheck();
        }

        //Ricerca dello user control Oggetto lato server
        protected DocsPAWA.documento.Oggetto GetControlOggetto()
        {
            return (DocsPAWA.documento.Oggetto)this.FindControl("ctrl_oggetto");
        }


        /// <summary>
        /// Verifica se ci sono corrispondenti interni che non siano utenti dunque soggetti ad ereditarietà
        /// </summary>
        /// <returns></returns>
        private bool CheckDestinatariInterni()
        {
            Corrispondente[] listaDest = null;
            Corrispondente[] listaDestCC = null;
            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();

            ////se è abilitato l'ufficio referente invio trasmissioni con la ragione dell'amministrazione
            //if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
            //{
            //    return true;
            //}

            DocsPAWA.DocsPaWR.Registro regSelezionato = UserManager.getRegistroSelezionato(this);
            //if (regSelezionato == null)
            //{
            //    regSelezionato = UserManager.getRegistroSelezionato(this);
            //}
            if(schedaDocumento!=null){
                if (schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloInterno))
                {
                    listaDest = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatari;
                    listaDestCC = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatariConoscenza;
                }

                if (schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloUscita))
                {
                    listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari;
                    listaDestCC = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza;
                }

                if (listaDest != null && listaDest.Length > 0)
                {
                    for (int i = 0; i < listaDest.Length; i++)
                    {
                        if (listaDest[i].GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
                        {
                            DocsPAWA.DocsPaWR.UnitaOrganizzativa UO = (DocsPAWA.DocsPaWR.UnitaOrganizzativa)listaDest[i];
                            //Controlla se almeno uno dei registri del ruolo è uguale a quello corrente
                            //foreach (DocsPAWA.DocsPaWR.Registro regUO in UO.registri)
                            //{
                            //    if (regUO.systemId == regSelezionato.systemId)
                            if (UO.tipoIE == "I")
                            {
                                return true;
                            }
                            //}
                        }

                        if (listaDest[i].GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
                        {
                            DocsPAWA.DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)listaDest[i];
                            //Controlla se almeno uno dei registri del ruolo è uguale a quello corrente
                            //foreach(DocsPAWA.DocsPaWR.Registro reg in ruolo.registri)
                            //{
                            //    if (reg.systemId == regSelezionato.systemId)
                            if (ruolo.tipoIE == "I")
                            {
                                return true;
                            }
                            //}
                        }
                    }
                }

                if (listaDestCC != null && listaDestCC.Length > 0)
                {
                    for (int i = 0; i < listaDestCC.Length; i++)
                    {
                        if (listaDestCC[i].GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
                        {
                            DocsPAWA.DocsPaWR.UnitaOrganizzativa UOcc = (DocsPAWA.DocsPaWR.UnitaOrganizzativa)listaDestCC[i];
                            //Controlla se almeno uno dei registri del ruolo è uguale a quello corrente
                            //foreach (DocsPAWA.DocsPaWR.Registro regUOcc in UOcc.registri)
                            //{
                            //    if (regUOcc.systemId == regSelezionato.systemId)
                            if (UOcc.tipoIE == "I")
                            {
                                return true;
                            }
                            //}
                        }

                        if (listaDestCC[i].GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
                        {
                            DocsPAWA.DocsPaWR.Ruolo ruoloCC = (DocsPAWA.DocsPaWR.Ruolo)listaDestCC[i];
                            //Controlla se almeno uno dei registri del ruolo è uguale a quello corrente
                            //foreach (DocsPAWA.DocsPaWR.Registro regCC in ruoloCC.registri)
                            //{
                            //    if (regCC.systemId == regSelezionato.systemId)
                            if (ruoloCC.tipoIE == "I")
                            {
                                return true;
                            }
                            //}
                        }
                    }
                }
            }
            return false;
        }
        protected void btn_titolario_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ////E' necessario che sia selezionato un titolario e non la voce "tutti i titolari"
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPaWebService();
            ArrayList listaTitolari = new ArrayList(wws.getTitolariUtilizzabili(UserManager.getUtente(this).idAmministrazione));
            string idTitolario = this.getIdTitolario("", listaTitolari);
            FascicoliManager.removeFascicoloSelezionatoFascRapida();
            if (!string.IsNullOrEmpty(idTitolario))
            {
                if (!this.IsStartupScriptRegistered("apriModalDialog"))
                {
                    //string scriptString = "<SCRIPT>ApriTitolario('codClass=" + txt_codClass.Text + "&idTit=" + ddl_titolari.SelectedValue + "','gestClass')</SCRIPT>";
                    string scriptString = "<SCRIPT>ApriTitolario2('codClass=" + string.Empty + "&idTit=" + idTitolario + "','gestProtInSempl')</SCRIPT>";
                    this.RegisterStartupScript("apriModalDialog", scriptString);
                }
            }
            else
            {
                string script = "<SCRIPT>alert('Non è presente nessun titolario attivo');</SCRIPT>";
                this.RegisterStartupScript("alert", script);
            }
        }

        private string getIdTitolario(string codClassificazione, ArrayList titolari)
        {
            string result = string.Empty;
            foreach (DocsPAWA.DocsPaWR.OrgTitolario titolario in titolari)
            {
                if (titolario.Stato == DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Attivo)
                {
                    result = titolario.ID;
                }
            }
            return result;
        }

        private void caricaModelliTrasmRapida()
        {
            this.ddl_trasm_rapida.Items.Clear();
            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPAWA.DocsPaWR.Registro[] registri = new DocsPAWA.DocsPaWR.Registro[1];
            registri[0] = UserManager.getRegistroSelezionato(this);
            string idAmm = UserManager.getInfoUtente(this).idAmministrazione;
            string idPeople = UserManager.getInfoUtente(this).idPeople;
            string idCorrGlobali = UserManager.getInfoUtente(this).idCorrGlobali;
            string idRuoloUtente = UserManager.getInfoUtente(this).idGruppo;
            //ArrayList idModelli = new ArrayList(ws.getModelliPerTrasm(idAmm, registri, idPeople, idCorrGlobali, "", "", "", "D", "", idRuoloUtente, false, ""));
            ArrayList idModelli = new ArrayList(ws.getModelliPerTrasmLite(idAmm, registri, idPeople, idCorrGlobali, "", "", "", "D", "", idRuoloUtente, false, ""));
               
            if (this.ddl_trasm_rapida.Items.Count == 0)
            {
                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                li.Text = " ";
                this.ddl_trasm_rapida.Items.Add(li);
            }

            for (int i = 0; i < idModelli.Count; i++)
            {
                DocsPAWA.DocsPaWR.ModelloTrasmissione mod = (DocsPAWA.DocsPaWR.ModelloTrasmissione)idModelli[i];
                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                li.Value = mod.SYSTEM_ID.ToString();
                li.Text = mod.NOME;
                if (System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_CODICE_MODELLI_TRASM"] != null && System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_CODICE_MODELLI_TRASM"] == "1")
                    li.Text += " (" + mod.CODICE + ")";
                this.ddl_trasm_rapida.Items.Add(li);
            }
        }

      
        private void getDatiAggiuntiviUtenteSmistamento(ref UtenteSmistamento utenteSmistamento, string idUtente)
        {
            DocsPaWebService ws = new DocsPaWebService();
            ws.getDatiAggiuntiviUtenteSmistamento(ref utenteSmistamento, idUtente);
        }



        private void execTrasmRapidaDaModello()
        {
            ModelloTrasmissione modello = new ModelloTrasmissione();
            int indexSel = ddl_trasm_rapida.SelectedIndex;
            DocsPaWebService ws = new DocsPaWebService();            
            if (indexSel > 0)
            {
                modello = ws.getModelloByID(UserManager.getInfoUtente(this).idAmministrazione, ddl_trasm_rapida.SelectedValue);
                SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();
                Session["Modello"] = modello;

                        Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();
                        if (modello != null)
                        {
                            if (string.IsNullOrEmpty(modello.NO_NOTIFY))
                                trasmissione.NO_NOTIFY = "0";
                            else
                                trasmissione.NO_NOTIFY = modello.NO_NOTIFY;
                        }
                        //Parametri della trasmissione
                        trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;
                        trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
                        trasmissione.infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
                        trasmissione.utente = UserManager.getUtente(this);
                        trasmissione.ruolo = UserManager.getRuolo(this);

                        //Parametri delle trasmissioni singole
                        for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                        {
                            DocsPAWA.DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                            ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                            for (int j = 0; j < destinatari.Count; j++)
                            {
                                DocsPAWA.DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                                DocsPAWA.DocsPaWR.Corrispondente corr = new Corrispondente();
                                //old: ritoranva anche i corr diasbilitati DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                                if (mittDest.CHA_TIPO_MITT_DEST == "D")
                                {
                                    corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this, mittDest.VAR_COD_RUBRICA, DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO);
                                }
                                else
                                {
                                    corr = TrasmManager.getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, schedaDocumento, null, this);
                                }
                                if (corr != null)
                                {   //il corr è null se non esiste o se è stato disabiltato.    
                                    DocsPAWA.DocsPaWR.RagioneTrasmissione ragione =ws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                                    //Andrea - try - catch
                                    try
                                    {
                                        trasmissione = TrasmManager.addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, this);
                                    }
                                    catch(ExceptionTrasmissioni e) 
                                    {
                                        listaExceptionTrasmissioni.Add(e.Messaggio);
                                    }
                                    //End Andrea
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
                        
                        DocsPAWA.DocsPaWR.Trasmissione t_rs = null;
                        if (trasmissione.trasmissioniSingole != null && trasmissione.trasmissioniSingole.Length > 0)
                        {

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

                            trasmissione = this.impostaNotificheUtentiDaModello(trasmissione);
                            DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
                            if (infoUtente.delegato != null)
                                trasmissione.delegato = ((DocsPAWA.DocsPaWR.InfoUtente)(infoUtente.delegato)).idPeople;

                            if (this.cessioneDirittiAbilitato(trasmissione, modello))
                            {
                                bool aperturaPopUpSceltaNuovoProprietario = false;

                                // verifica se esistono ruoli tra i destinatari
                                this.verificaRuoliDestInTrasmissione(trasmissione);

                                switch (this.numeroRuoliDestInTrasmissione)
                                {
                                    case 0:
                                        // non ci sono ruoli tra i destinatari! avvisa...
                                        this.inviaMsgNoRuoli();
                                        return;
                                        break;

                                    case 1:
                                        // ce n'è 1, verifica se un solo utente del ruolo ha la notifica...
                                        this.utentiConNotifica(trasmissione);
                                        if (this.numeroUtentiConNotifica > 1)
                                            aperturaPopUpSceltaNuovoProprietario = true;
                                        else
                                        {
                                            // 1 solo utente con notifica, il sistema ha già memorizzato il suo id_people...
                                            trasmissione.cessione.idPeopleNewPropr = this.idPeopleNewOwner;
                                            trasmissione.cessione.idRuoloNewPropr = ((DocsPAWA.DocsPaWR.Ruolo)trasmissione.trasmissioniSingole[0].corrispondenteInterno).idGruppo;

                                            modello.CEDE_DIRITTI = "1";
                                            modello.ID_PEOPLE_NEW_OWNER = trasmissione.cessione.idPeopleNewPropr;
                                            modello.ID_GROUP_NEW_OWNER = trasmissione.cessione.idRuoloNewPropr;
                                        }
                                        break;

                                    default:
                                        // ce ne sono + di 1, quindi ne fa scegliere uno...                                    
                                        aperturaPopUpSceltaNuovoProprietario = true;
                                        break;
                                }
                            }


                          t_rs=TrasmManager.saveExecuteTrasm(this, trasmissione, infoUtente);
                          
                            //trasmissione = TrasmManager.saveTrasm(this, trasmissione);
                            //t_rs = TrasmManager.executeTrasm(this, trasmissione);
                        }
                        if (t_rs != null && t_rs.ErrorSendingEmails)
                            Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");

                        //Salvataggio del system_id della trasm per il cambio di stato automatico
                        if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                        {
                            DocsPAWA.DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber, this);
                            bool trasmWF = false;
                            if (trasmissione != null && trasmissione.trasmissioniSingole != null)
                                for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                                {
                                    DocsPAWA.DocsPaWR.TrasmissioneSingola trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                                    if (trasmSing.ragione.tipo == "W")
                                        trasmWF = true;
                                }
                            if (stato != null && trasmWF)
                                DocsPAWA.DiagrammiManager.salvaStoricoTrasmDiagrammi(trasmissione.systemId, schedaDocumento.docNumber, Convert.ToString(stato.SYSTEM_ID), this);
                        }
                #region modelli vecchi
                //FINE MODELLI TRASMISSIONE NUOVI
                //        DocsPAWA.DocsPaWR.Trasmissione trasmEff = null;
                //try
                //{

                //        trasmEff = creaTrasmissione();

                //        if (trasmEff == null)
                //            Response.Write("<script language='javascript'>alert('Si è verificato un errore nella creazione della trasmissione da template');</script>");
                //        else
                //        {

                //            if (estendiVisibilita.Value == "false")
                //            {
                //                TrasmissioneSingola[] appoTrasmSingole = new TrasmissioneSingola[trasmEff.trasmissioniSingole.Length];
                //                for (int i = 0; i < trasmEff.trasmissioniSingole.Length; i++)
                //                {
                //                    TrasmissioneSingola trasmSing = new TrasmissioneSingola();
                //                    trasmSing = trasmEff.trasmissioniSingole[i];
                //                    trasmSing.ragione.eredita = "0";
                //                    appoTrasmSingole[i] = trasmSing;
                //                }
                //                trasmEff.trasmissioniSingole = appoTrasmSingole;
                //            }

                //            DocsPaWR.Trasmissione t_rs = TrasmManager.executeTrasm(this, trasmEff);

                //            if (t_rs != null && t_rs.ErrorSendingEmails)
                //                Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");



                //            //	Response.Write("<script>parent.IframeTabs.document.location='docTrasmissioni.aspx';</script>");	
                //            //Response.Write("<script language='javascript'>parent.IframeTabs.document.location='docTrasmissioni.aspx';</script>");	
                //            //resetto il template della trasmissione

                //            this.ddl_tmpl.SelectedIndex = 0;

                //            //Salvataggio del system_id della trasm per il cambio di stato automatico
                //            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                //            {
                //                DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber, this);
                //                bool trasmWF = false;
                //                for (int i = 0; i < trasmEff.trasmissioniSingole.Length; i++)
                //                {
                //                    DocsPaWR.TrasmissioneSingola trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmEff.trasmissioniSingole[i];
                //                    if (trasmSing.ragione.tipo == "W")
                //                        trasmWF = true;
                //                }
                //                if (stato != null && trasmWF)
                //                    DocsPAWA.DiagrammiManager.salvaStoricoTrasmDiagrammi(trasmEff.systemId, schedaDocumento.docNumber, Convert.ToString(stato.SYSTEM_ID), this);
                //            }
                //        }
                //}
                //catch (System.Web.Services.Protocols.SoapException es)
                //{
                //    ErrorManager.redirect(this, es);
                //}

                #endregion
            }
        }

        private DocsPAWA.DocsPaWR.Trasmissione impostaNotificheUtentiDaModello(DocsPAWA.DocsPaWR.Trasmissione objTrasm)
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

            DocsPAWA.DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
            {
                DocsPAWA.DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPAWA.DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
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

        /// <summary>
        /// Verifica se è una trasmissione con cessione dei diritti sull'oggetto.
        /// Ritorna True se:
        /// 
        ///     1 - la trasmissione è generata da un modello di trasmissione con cessione diritti (anche se l'utente non è abilitato all'editing ACL)
        ///     2 - la trasmissione prevede già cessione dei diritti poichè l'utente (abilitato all'editing ACL) ha selezionato la checkbox "Cedi diritti"
        ///     
        /// </summary>
        /// <returns>True, False</returns>
        private bool cessioneDirittiAbilitato(DocsPAWA.DocsPaWR.Trasmissione trasm, DocsPAWA.DocsPaWR.ModelloTrasmissione modello)
        {
            bool retValue = false;
            if (modello != null)
                trasm.NO_NOTIFY = modello.NO_NOTIFY;
            if (modello != null && modello.CEDE_DIRITTI.Equals("1"))
            {

                if (trasm.cessione == null)
                {
                    DocsPAWA.DocsPaWR.CessioneDocumento cessione = new DocsPAWA.DocsPaWR.CessioneDocumento();
                    cessione.docCeduto = true;
                    //*******************************************************************************************
                    // MODIFICA IACOZZILLI GIORDANO 19/07/2012
                    //
                    // NOTA!!!!!! E' la 5 volta che inserisco questo codice, in origine è stato replicato scriptato
                    // ogni santa volta senza farne un unico metodo!!!! Io DEVO seguire questa linea.
                    //
                    // Modifica inerente la cessione dei diritti di un doc da parte di un utente non proprietario ma 
                    // nel ruolo del proprietario, in quel caso non posso valorizzare l'IDPEOPLE  con il corrente perchè
                    // il proprietario può essere un altro utente del mio ruolo, quindi andrei a generare un errore nella security,
                    // devo quindi controllare che nell'idpeople venga inserito l'id corretto del proprietario.
                    string valoreChiaveCediDiritti = DocsPAWA.utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_CEDI_DIRITTI_IN_RUOLO");
                    if (!string.IsNullOrEmpty(valoreChiaveCediDiritti) && valoreChiaveCediDiritti.Equals("1"))
                    {
                        //Devo istanziare una classe utente.
                        string idProprietario = string.Empty;
                        idProprietario = GetAnagUtenteProprietario();
                        Utente _utproprietario = UserManager.GetUtenteByIdPeople(idProprietario);

                        cessione.idPeople = idProprietario;
                        cessione.idRuolo = UserManager.getInfoUtente(this).idGruppo;
                        cessione.userId = _utproprietario.cognome + " " + _utproprietario.nome;


                        if (cessione.idPeople == null || cessione.idPeople == "")
                            cessione.idPeople = idProprietario;

                        if (cessione.idRuolo == null || cessione.idRuolo == "")
                            cessione.idRuolo = UserManager.getInfoUtente(this).idGruppo;

                        if (cessione.userId == null || cessione.userId == "")
                            cessione.userId = _utproprietario.cognome + " " + _utproprietario.nome;
                    }
                    else
                    {
                        //OLD CODE:
                        cessione.idPeople = UserManager.getInfoUtente(this).idPeople;
                        cessione.idRuolo = UserManager.getInfoUtente(this).idGruppo;
                        cessione.userId = UserManager.getInfoUtente(this).userId;
                    }
                    //*******************************************************************************************
                    // FINE MODIFICA
                    //********************************************************************************************
                    cessione.idPeopleNewPropr = modello.ID_PEOPLE_NEW_OWNER;
                    cessione.idRuoloNewPropr = modello.ID_GROUP_NEW_OWNER;
                    trasm.cessione = cessione;
                }
            }
            else
                retValue = (trasm.cessione != null && trasm.cessione.docCeduto);

            return retValue;
        }



        /// <summary>
        /// Iacozzilli: Faccio la Get dell'idProprietario del doc!
        /// </summary>
        /// <returns></returns>
        private string GetAnagUtenteProprietario()
        {
            DocumentoDiritto[] listaVisibilita = null;
            DocsPAWA.DocsPaWR.SchedaDocumento sd = DocumentManager.getDocumentoSelezionato(this);
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

        /// <summary>
        /// Verifica se esistono RUOLI tra i destinatari della trasmissione 
        /// ed imposta quanti sono
        /// </summary>
        private void verificaRuoliDestInTrasmissione(DocsPAWA.DocsPaWR.Trasmissione trasm)
        {
            foreach (DocsPAWA.DocsPaWR.TrasmissioneSingola trasmy in trasm.trasmissioniSingole)
            {
                if (trasmy.tipoDest.ToString().ToUpper().Equals("RUOLO"))
                    this.numeroRuoliDestInTrasmissione++;
            }
        }

        /// <summary>
        /// Invia un messaggio a video che avvisa l'utente che tra i destinatari della trasmissioni
        /// non ci sono ruoli
        /// </summary>
        private void inviaMsgNoRuoli()
        {
            DocsPAWA.utils.CessioneDiritti obj = new DocsPAWA.utils.CessioneDiritti();
            if (!ClientScript.IsStartupScriptRegistered("OpenMsg"))
                ClientScript.RegisterStartupScript(this.GetType(), "OpenMsg", obj.InviaMsgNoRuoli());
        }

        /// <summary>
        /// Verifica se ci sono utenti con notifica, quanti sono e, nel caso di 1, ne memorizza l'ID
        /// </summary>
        private void utentiConNotifica(DocsPAWA.DocsPaWR.Trasmissione trasm)
        {
            foreach (DocsPAWA.DocsPaWR.TrasmissioneSingola trasmy in trasm.trasmissioniSingole)
                foreach (DocsPAWA.DocsPaWR.TrasmissioneUtente trasmUt in trasmy.trasmissioneUtente)
                    if (trasmUt.daNotificare)
                    {
                        this.numeroUtentiConNotifica++;
                        this.idPeopleNewOwner = trasmUt.utente.idPeople; // memorizza l'id people... da utilizzare nel caso di un solo utente con notifica                        
                    }
        }

        private void setListBoxMittentiMultipli()
        {
            //Valido per i protocolli in Arrivo
            DocsPAWA.DocsPaWR.Corrispondente mittente;
            lbx_mittMultiplo.Items.Clear();
            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();
            if (schedaDocumento != null && ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenti != null)
            {
                for (int i = 0; i < ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenti.Length; i++)
                {
                    mittente = (((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenti[i]);
                    ListItem item = new ListItem(UserManager.getDecrizioneCorrispondenteSemplice(mittente), mittente.codiceRubrica);
                    lbx_mittMultiplo.Items.Add(item);
                }
            }            
        }

        private void txt_codMittMultiplo_TextChanged(object sender, System.EventArgs e)
        {
            /*
            //Valido per i protocolli in Arrivo
            try
            {
                if (!string.IsNullOrEmpty(this.txt_codMittMultiplo.Text))
                {
                    Corrispondente corr = UserManager.getCorrispondenteByCodRubrica(this, this.txt_codMittMultiplo.Text);

                    if (corr == null)
                        codice_non_trovato("Nessun corrispondente trovato!", ref txt_codMittMultiplo, ref txt_descMittMultiplo);
                    if (corr != null)
                        txt_descMittMultiplo.Text = corr.descrizione;
                }

                if (string.IsNullOrEmpty(this.txt_codMittMultiplo.Text))
                {
                    txt_codMittMultiplo.Text = string.Empty;
                    txt_descMittMultiplo.Text = string.Empty;
                }
                
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }
            */
        }

        private void btn_insMittMultiplo_Click(object sender, System.EventArgs e)
        {
            /*
            //Valido per i protocolli in Arrivo
            try
            {
                if (!string.IsNullOrEmpty(this.txt_descMittMultiplo.Text))
                {
                    Corrispondente corr = null;
                    SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();
                    if (!string.IsNullOrEmpty(this.txt_codMittMultiplo.Text))
                        corr = UserManager.getCorrispondenteByCodRubrica(this, this.txt_codMittMultiplo.Text);

                    if (corr != null)
                    {
                        ((ProtocolloEntrata)schedaDocumento.protocollo).mittenti = UserManager.addCorrispondente(((ProtocolloEntrata)schedaDocumento.protocollo).mittenti, corr);
                    }
                    else
                    {
                        corr = new DocsPAWA.DocsPaWR.Corrispondente();
                        corr.descrizione = this.txt_descMittMultiplo.Text;
                        corr.tipoCorrispondente = "O";
                        corr.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
                        ((ProtocolloEntrata)schedaDocumento.protocollo).mittenti = UserManager.addCorrispondente(((ProtocolloEntrata)schedaDocumento.protocollo).mittenti, corr);
                    }

                    ((ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittentiMultipli = true;
                    DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                    DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                    this.setListBoxMittentiMultipli();
                    txt_codMittMultiplo.Text = string.Empty;
                    txt_descMittMultiplo.Text = string.Empty;
                }                
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }
            */
        }

        private void btn_CancMittMultiplo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //Valido per i protocolli in Arrivo
            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();
                
            if (this.lbx_mittMultiplo.SelectedIndex >= 0)
            {
                ((ProtocolloEntrata)schedaDocumento.protocollo).mittenti = UserManager.removeCorrispondente(((ProtocolloEntrata)schedaDocumento.protocollo).mittenti, this.lbx_mittMultiplo.SelectedIndex);
                ((ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittentiMultipli = true;
                DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                this.setListBoxMittentiMultipli();
            }
            else
            {
                if (((ProtocolloEntrata)schedaDocumento.protocollo).mittenti != null && ((ProtocolloEntrata)schedaDocumento.protocollo).mittenti.Length > 0)
                {
                    string msg = "Mittente non selezionato";
                    Response.Write("<script>alert('" + msg + "')</script>");
                }
            }

        }

        private void btn_upMittente_Click(object sender, System.EventArgs e)
        {
            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();

            if (lbx_mittMultiplo.SelectedIndex != -1 && ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenti != null)
            {
                txtCodMittente.Text = lbx_mittMultiplo.SelectedItem.Value;
                txtDescrMittente.Text = lbx_mittMultiplo.SelectedItem.Text;

                Corrispondente corr = null;
                corr = UserManager.getCorrispondenteByCodRubrica(this, txtCodMittente.Text);

                if (corr != null)
                {
                    SetCorrispondenteInProtocollo(corr);
                    //setDescCorrispondente(this.txtCodMittente.Text, "Mit", true);
                }
                else
                {
                    txtCodMittente.Text = string.Empty;
                    corr = new DocsPAWA.DocsPaWR.Corrispondente();
                    corr.descrizione = this.txtDescrMittente.Text;
                    corr.tipoCorrispondente = "O";
                    corr.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
                    ((ProtocolloEntrata)schedaDocumento.protocollo).mittente = corr;
                }
                ((ProtocolloEntrata)schedaDocumento.protocollo).mittenti = UserManager.removeCorrispondente(((ProtocolloEntrata)schedaDocumento.protocollo).mittenti, this.lbx_mittMultiplo.SelectedIndex);
                ((ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittentiMultipli = true;
                ((ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = true;
                DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                this.setListBoxMittentiMultipli();
            }
        }

        private void btn_downMittente_Click(object sender, System.EventArgs e)
        {
            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();

            if (!string.IsNullOrEmpty(this.txtDescrMittente.Text))
            {
                Corrispondente corr = null;
                if (!string.IsNullOrEmpty(this.txtCodMittente.Text))
                    corr = UserManager.getCorrispondenteByCodRubrica(this, this.txtCodMittente.Text);

                if (corr != null && !string.IsNullOrEmpty(corr.codiceRubrica) && !string.IsNullOrEmpty(corr.descrizione))
                {
                    if (checkDuplicatiMittMultipli(corr, lbx_mittMultiplo))
                        ((ProtocolloEntrata)schedaDocumento.protocollo).mittenti = UserManager.addCorrispondente(((ProtocolloEntrata)schedaDocumento.protocollo).mittenti, corr);
                }
                else
                {
                    corr = new DocsPAWA.DocsPaWR.Corrispondente();
                    corr.descrizione = this.txtDescrMittente.Text;
                    corr.tipoCorrispondente = "O";
                    corr.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
                    if (checkDuplicatiMittMultipli(corr, lbx_mittMultiplo))
                        ((ProtocolloEntrata)schedaDocumento.protocollo).mittenti = UserManager.addCorrispondente(((ProtocolloEntrata)schedaDocumento.protocollo).mittenti, corr);
                }

                ((ProtocolloEntrata)schedaDocumento.protocollo).mittente = null;
                ((ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = true;
                ((ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittentiMultipli = true;
                DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                this.setListBoxMittentiMultipli();
                txtCodMittente.Text = string.Empty;
                txtDescrMittente.Text = string.Empty;
            }
        }

        private bool checkDuplicatiMittMultipli(Corrispondente corrispondente, ListBox listBoxMittMultipli)
        {
            bool result = true;
            if (corrispondente != null && lbx_mittMultiplo != null && lbx_mittMultiplo.Items.Count > 0)
            {
                foreach (ListItem item in lbx_mittMultiplo.Items)
                {
                    if (item.Value == corrispondente.codiceRubrica || item.Text.ToUpper() == corrispondente.descrizione.ToUpper())
                        result = false;
                }
            }
            return result;
        }

        private void btn_nascosto_mitt_multipli_Click(object sender, System.EventArgs e)
        {
            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();

            if (!string.IsNullOrEmpty(this.txt_desc_mitt_mult_nascosto.Value))
            {
                Corrispondente corr = null;
                if (!string.IsNullOrEmpty(this.txt_cod_mitt_mult_nascosto.Value))
                    corr = UserManager.getCorrispondenteByCodRubrica(this, this.txt_cod_mitt_mult_nascosto.Value);

                if (corr != null && !string.IsNullOrEmpty(corr.codiceRubrica) && !string.IsNullOrEmpty(corr.descrizione))
                {
                    if (checkDuplicatiMittMultipli(corr, lbx_mittMultiplo))
                    {
                        if (((ProtocolloEntrata)schedaDocumento.protocollo).mittente == null ||
                            (((ProtocolloEntrata)schedaDocumento.protocollo).mittente != null &&
                            ((ProtocolloEntrata)schedaDocumento.protocollo).mittente.codiceRubrica != corr.codiceRubrica &&
                            ((ProtocolloEntrata)schedaDocumento.protocollo).mittente.descrizione.ToUpper() != corr.descrizione.ToUpper())
                        )
                        {
                            ((ProtocolloEntrata)schedaDocumento.protocollo).mittenti = UserManager.addCorrispondente(((ProtocolloEntrata)schedaDocumento.protocollo).mittenti, corr);
                            ((ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittentiMultipli = true;
                            DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                            DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                            this.setListBoxMittentiMultipli();
                            txt_cod_mitt_mult_nascosto.Value = string.Empty;
                            txt_desc_mitt_mult_nascosto.Value = string.Empty;
                        }
                    }
                }

            }
        }

        private void clearDatiMittentiMultipli()
        {
            //Valido per i protocolli in Arrivo
            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();

            if (this.lbx_mittMultiplo.Items.Count >= 0)
            {
                for (int i = 0; i < lbx_mittMultiplo.Items.Count; i++)
                {
                    ((ProtocolloEntrata)schedaDocumento.protocollo).mittenti = UserManager.removeCorrispondente(((ProtocolloEntrata)schedaDocumento.protocollo).mittenti, 0);
                    ((ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittentiMultipli = true;
                    DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                    DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                }
                this.setListBoxMittentiMultipli();
            }
        }

        private void chkPrivato_CheckedChanged(object sender, System.EventArgs e)
        {
            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();
            if (this.chkPrivato.Checked)
                schedaDocumento.privato = "1";
            else
            {
                schedaDocumento.privato = "0";
            }
        }

        private void ddl_trasm_rapida_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            DocsPaWebService ws = new DocsPaWebService();
            if (ddl_trasm_rapida.SelectedValue != null && ddl_trasm_rapida.SelectedValue.Trim() != string.Empty)
            {
                //MODELLI TRASMISSIONE NUOVI
                DocsPAWA.DocsPaWR.ModelloTrasmissione modello = new DocsPAWA.DocsPaWR.ModelloTrasmissione();
                //int indexSel = ddl_tmpl.Items.IndexOf(ddl_tmpl.Items.FindByValue(separatore));
                int indexSel = ddl_trasm_rapida.SelectedIndex;

                //se l'item selezionato è il separatore oppure è un template vecchio..esco dal metodo
                if (String.IsNullOrEmpty(ddl_trasm_rapida.SelectedItem.Text) || indexSel <= 0)
                {
                    return;
                }
                modello = ModelliTrasmManager.getModelloByID(UserManager.getRegistroSelezionato(this).idAmministrazione, ddl_trasm_rapida.SelectedValue, this);
                if (modello != null && modello.SYSTEM_ID != 0)
                {
                    appoIdMod.Value = modello.SYSTEM_ID.ToString();

                    if (string.IsNullOrEmpty(appoIdAmm.Value))
                    {
                        appoIdAmm.Value = "null";
                    }
                    else
                    {
                        //nel caso in cui non vi fossero destinatari interni devo usare la FUNCTION
                        //SOLO passandogli IdMod mentre IdAmm lo metto a null!
                        if (isInterno.Value != "true")
                        {
                            appoIdAmm.Value = "null";
                        }
                    }

                    if (ws.ereditaVisibilita(appoIdAmm.Value, appoIdMod.Value))
                    {
                        this.abilitaModaleVis.Value = "true";
                    }
                    else
                    {
                        this.abilitaModaleVis.Value = "false";
                    }

                }
                else
                {
                    Session.Remove("Modello");
                    appoIdMod.Value = "null";
                    if (appoIdAmm.Value == "null" || string.IsNullOrEmpty(appoIdAmm.Value))
                    {
                        this.abilitaModaleVis.Value = "false";
                    }
                }
                //FINE MODELLI TRASMISSIONE NUOVI
            }
        }

        private void ClearPrivato()
        {
            SchedaDocumento schedaDocumento = this.GetProtocolloManager().GetDocumentoCorrente();
            if (schedaDocumento != null)
            {
               schedaDocumento.privato = "0";
               this.chkPrivato.Checked = false;
            }
        }

        protected void newRubricaVeloce(string tipo)
        {
            //RUBRICA VELOCE BETA
            if (System.Configuration.ConfigurationManager.AppSettings["RUBRICAVELOCE_MINIMUMPREFIXLENGTH"] != null)
            {
                new_mitt_veloce_sempl_ing.MinimumPrefixLength = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RUBRICAVELOCE_MINIMUMPREFIXLENGTH"].ToString());
            }

            string dataUser = null;
            DocsPAWA.DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
            System.Web.HttpContext ctx = System.Web.HttpContext.Current;
            if (ctx.Session["userRuolo"] != null)
            {
                dataUser = ((DocsPAWA.DocsPaWR.Ruolo)ctx.Session["userRuolo"]).systemId;
            }

            if (ctx.Session["userRegistro"] != null)
            {
                dataUser = dataUser + "-" + ((DocsPAWA.DocsPaWR.Registro)ctx.Session["userRegistro"]).systemId;
            }

            string idAmm = cr.idAmministrazione;
            string callType = null;
            string javascript = null;

            if (tipo.Equals("A"))
            {
                // Mittente su protocollo in ingresso
                callType = "CALLTYPE_PROTO_INGRESSO";
                new_mitt_veloce_sempl_ing.ContextKey = dataUser + "-" + idAmm + "-" + callType;
            }

            if (tipo.Equals("P"))
            {
                // Mittente su protocollo in uscita
                callType = "CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO";
                new_mitt_veloce_sempl_usc.ContextKey = dataUser + "-" + idAmm + "-" + callType;

                // Destinatario su protocollo in uscita
                callType = "CALLTYPE_PROTO_USCITA_SEMPLIFICATO";
                new_dest_veloce_sempl_usc.ContextKey = dataUser + "-" + idAmm + "-" + callType;
            }


        }
    }
}
