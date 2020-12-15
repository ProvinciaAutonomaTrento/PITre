using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.IO;
using DocsPAWA.DocsPaWR;
using DocsPAWA.popup.RubricaDocsPA;
using DocsPAWA.utils;
using log4net;
using DocsPAWA.Spedizione;
using System.Text;
using System.Linq;

namespace DocsPAWA.documento
{
    /// <summary>
    /// Questa e' la form centrale nella gestione della protocollazione di un documento.
    /// </summary>
    public class docProtocollo : DocsPAWA.CssPage
    {

        /*
         * Andrea
         */
        private ArrayList listaExceptionTrasmissioni = new ArrayList();
        private string messError = "";
        /*
         * End Andrea
         */

        private ILog logger = LogManager.GetLogger(typeof(docProtocollo));
        #region Controls

        protected System.Web.UI.WebControls.TextBox lbl_segnatura;
        protected DocsPaWebCtrlLibrary.ImageButton btn_DetMit_P;
        protected System.Web.UI.WebControls.TextBox txt_CodMit_P;
        protected System.Web.UI.WebControls.TextBox txt_DescMit_P;
        protected DocsPaWebCtrlLibrary.ImageButton btn_DetMitInt_P;
        protected System.Web.UI.WebControls.TextBox txt_CodMitInt_P;
        protected System.Web.UI.WebControls.TextBox txt_DescMitInt_P;
        protected DocsPaWebCtrlLibrary.ImageButton btn_verificaPrec_P;
        //protected DocsPaWebCtrlLibrary.DateMask txt_DataProtMit_P;
        protected DocsPAWA.UserControls.Calendar txt_DataProtMit_P;
        protected System.Web.UI.WebControls.TextBox txt_NumProtMit_P;
        protected DocsPaWebCtrlLibrary.ImageButton btn_aggiungiDest_P;
        protected System.Web.UI.WebControls.TextBox txt_CodDest_P;
        protected System.Web.UI.WebControls.TextBox txt_DescDest_P;
        protected System.Web.UI.WebControls.Panel panel_Mit;
        protected System.Web.UI.WebControls.Panel panel_Dest;
        protected System.Web.UI.WebControls.Panel panel_Annul;
        protected System.Web.UI.WebControls.TextBox txt_dataAnnul_P;
        protected System.Web.UI.WebControls.TextBox txt_numAnnul_P;
        protected System.Web.UI.WebControls.TextBox lbl_dataCreazione;
        protected System.Web.UI.WebControls.ImageButton btn_oggettario;
        protected System.Web.UI.WebControls.TextBox txt_oggetto;
        protected System.Web.UI.WebControls.ImageButton btn_selezionaParoleChiave;
        protected System.Web.UI.WebControls.ListBox lbx_paroleChiave;
        protected Note.DettaglioNota dettaglioNota;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoAtto;
        protected DocsPaWebCtrlLibrary.ImageButton btn_visibilita;
        protected DocsPaWebCtrlLibrary.ImageButton btn_log;
        protected System.Web.UI.WebControls.ListBox lbx_dest;
        protected System.Web.UI.WebControls.ListBox lbx_destCC;
        protected DocsPaWebCtrlLibrary.ImageButton btn_dettDest;
        protected DocsPaWebCtrlLibrary.ImageButton btn_cancDest;
        protected DocsPaWebCtrlLibrary.ImageButton btn_dettDestCC;
        protected DocsPaWebCtrlLibrary.ImageButton btn_cancDestCC;
        protected DocsPaWebCtrlLibrary.ImageButton btn_insDestCC;
        protected DocsPaWebCtrlLibrary.ImageButton btn_insDest;
        protected System.Web.UI.WebControls.RadioButtonList rbl_InOut_P;
        protected System.Web.UI.WebControls.TextBox txt_dataSegn;
        protected DocsPaWebCtrlLibrary.ImageButton btn_salva_P;
        protected DocsPaWebCtrlLibrary.ImageButton btn_aggiungi_P;
        protected DocsPaWebCtrlLibrary.ImageButton btn_riproponiDati_P;
        protected DocsPaWebCtrlLibrary.ImageButton btn_annulla_P;
        protected DocsPaWebCtrlLibrary.ImageButton btn_annullaPred;
        protected DocsPaWebCtrlLibrary.ImageButton btn_protocolla_P;
        protected DocsPaWebCtrlLibrary.ImageButton btn_protocollaGiallo_P;
        protected System.Web.UI.WebControls.TextBox txt_oggetto_;
        //protected System.Web.UI.WebControls.TextBox txt_oggetto_P;
        protected DocsPaWebCtrlLibrary.ImageButton btn_spedisci_P;
        protected DocsPaWebCtrlLibrary.ImageButton btn_modificaOgget_P;
        protected DocsPaWebCtrlLibrary.ImageButton btn_storiaOgg_P;
        //protected DocsPaWebCtrlLibrary.DateMask txt_DataArrivo_P;
        protected DocsPAWA.UserControls.Calendar txt_DataArrivo_P;
        protected DocsPaWebCtrlLibrary.TimeMask txt_OraPervenuto_P;
        protected DocsPaWebCtrlLibrary.ImageButton btn_StampaBuste_P;
        protected DocsPaWebCtrlLibrary.ImageButton btn_tipo_sped;
        protected DocsPaWebCtrlLibrary.ImageButton btn_tipo_spedCC;
        protected DocsPaWebCtrlLibrary.ImageButton btn_StampaVoidLabel;
        protected System.Web.UI.WebControls.Panel panel_ActiveX;
        protected Utilities.MessageBox msg_TrasmettiRapida;
        protected Utilities.MessageBox msg_TrasmettiProto;
        protected Utilities.MessageBox msg_copiaDoc;
        protected Utilities.MessageBox msg_SpedizioneAutomatica;
        //protected Utilities.MessageBox msg_SalvaDoc;
        protected DocsPaWebCtrlLibrary.ImageButton btn_invio_mail;

        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_descrizioneAmministrazione;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_dataArrivo;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_dataArrivoEstesa;

        private DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento;

        protected System.Web.UI.WebControls.CheckBox chkEvidenza;
        protected System.Web.UI.WebControls.Panel panel_mittInt;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_UrlIniFileDispositivo;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_dispositivo;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_modello_dispositivo;

        private string srcStampaSegnatura = "../blank_page.htm";
        protected System.Web.UI.WebControls.Panel pnl_mit_int_semplice;
        protected System.Web.UI.WebControls.Panel pnl_mit_int;
        protected System.Web.UI.WebControls.Panel pnl_rubr_dest_Semplice;
        protected System.Web.UI.WebControls.Panel pnl_rubr_des;
        protected System.Web.UI.WebControls.Panel pnl_mit_sempl;
        protected System.Web.UI.WebControls.Panel pnl_mit;

        //protected bool daAggiornarePrivato = false;
        protected bool m_modifica = false;
        protected bool btn_RubrOgget_P_state = false;

        protected bool btn_rubrica_p_sempl_state = false;
        protected bool btn_rubrica_p_state = false;
        protected bool btn_RubrMitInt_Sempl_state = false;
        protected bool btn_RubrMitInt_state = false;
        protected bool btn_RubrDest_Sempl_P_state = false;
        protected DocsPaWebCtrlLibrary.ImageButton btn_RubrOgget_P;

        protected System.Web.UI.WebControls.TextBox txt_dta_protoEme;
        protected System.Web.UI.WebControls.TextBox txt_ProtoEme;
        protected System.Web.UI.WebControls.Panel pnl_protoEme;
        protected System.Web.UI.WebControls.TextBox txt_CodFascicolo;
        protected System.Web.UI.WebControls.TextBox txt_DescFascicolo;
        protected System.Web.UI.WebControls.DropDownList ddl_tmpl;
        protected System.Web.UI.WebControls.DropDownList ddl_spedizione;
        protected System.Web.UI.WebControls.Panel pnl_fasc_rapida;
        protected System.Web.UI.WebControls.Panel pnl_trasm_rapida;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_amministrazioneEtichetta;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_classifica;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_fascicolo;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_tipoProtocollazione;
        protected System.Web.UI.HtmlControls.HtmlInputHidden h_proto2;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_ora_creazione;

        protected string codClassifica = "";
        protected bool btn_RubrDest_P_state = false;

        protected System.Web.UI.WebControls.CheckBox chkPrivato;

        // PROTO_IN_OUT
        protected static bool PIN = false;
        protected System.Web.UI.HtmlControls.HtmlInputHidden h_tipoAtto;
        protected static bool POUT = false;
        protected System.Web.UI.WebControls.Panel pnl_star_YES;
        protected System.Web.UI.WebControls.Panel pnl_star_NO;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_num_proto;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_anno_proto;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_data_proto;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_codreg_proto;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_descreg_proto;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_coduo_proto;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_tipo_proto;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_num_doc;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_numero_allegati;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_dataCreazione;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_codiceUoCreatore;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hdn_returnConfermaSped;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hdn_idRegRF;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hdnCodRFSegnatura;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hdnIdRFSegnatura;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_num_stampe;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_num_stampe_effettuate;

        protected System.Web.UI.WebControls.Panel rispProtoPanel;
        protected static bool POWN = false;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// 
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
        protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
        protected DocsPaWebCtrlLibrary.ImageButton btn_risp_sx;
        protected System.Web.UI.WebControls.TextBox txt_RispProtSegn_P;
        protected System.Web.UI.WebControls.Panel rispProtoPanelUscita;
        protected System.Web.UI.WebControls.Panel Panel_TipoAtto;
        protected System.Web.UI.WebControls.Panel pnl_evidenza;
        protected System.Web.UI.WebControls.CheckBox chkEvidenza1;
        protected DocsPaWebCtrlLibrary.ImageButton ImgBtnTrasp;
        protected System.Web.UI.WebControls.TextBox txt_num_stampe;
        protected DocsPaWebCtrlLibrary.ImageButton btn_stampaSegn_P;
        protected System.Web.UI.WebControls.Panel pnl_ufficioRef;
        protected DocsPaWebCtrlLibrary.ImageButton btn_Rubrica_ref;
        protected System.Web.UI.WebControls.TextBox txt_cod_uffRef;
        protected System.Web.UI.WebControls.TextBox txt_desc_uffRef;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
        protected System.Web.UI.WebControls.ImageButton btn_CampiPersonalizzati;
        protected string enableUfficioRef;
        protected System.Web.UI.WebControls.Panel Panel_ProfilazioneDinamica;
        protected DocsPaWebCtrlLibrary.ImageButton btn_addTipoAtto;
        protected bool isEnableUffRef;
        protected bool documentoProtocollato;
        protected System.Web.UI.WebControls.Label lbl_atto;

        protected DocsPaWebService wws = new DocsPaWebService();

        private bool _oggettoSelezionatoDaOggettario = false;
        protected System.Web.UI.HtmlControls.HtmlImage btn_rubrica_p_sempl;
        protected System.Web.UI.HtmlControls.HtmlImage btn_rubrica_p;
        protected System.Web.UI.HtmlControls.HtmlImage btn_RubrMitInt_Sempl;
        protected System.Web.UI.HtmlControls.HtmlImage btn_RubrMitInt;
        protected System.Web.UI.HtmlControls.HtmlImage btn_RubrDest_Sempl_P;
        protected System.Web.UI.HtmlControls.HtmlImage btn_RubrDest_P;
        protected DocsPaWebCtrlLibrary.ImageButton btn_StoriaDest;
        protected DocsPaWebCtrlLibrary.ImageButton btn_StoriaMitt;
        protected DocsPaWebCtrlLibrary.ImageButton btn_storiaData;
        protected DocsPaWebCtrlLibrary.ImageButton imgDescOgg;
        protected DocsPaWebCtrlLibrary.ImageButton imgListaDest;
        protected DocsPaWebCtrlLibrary.ImageButton imgListaDestCC;
        protected DocsPaWebCtrlLibrary.ImageButton btn_storiaCons;
        protected string val_proto_sele;

        protected DocsPaWebCtrlLibrary.ImageButton btn_inoltra;
        protected DocsPaWebCtrlLibrary.ImageButton btn_stampa_ricevuta;

        protected DocsPaWebCtrlLibrary.ImageButton btn_in_rispota_a;
        protected DocsPaWebCtrlLibrary.ImageButton btn_Risp;
        protected DocsPaWebCtrlLibrary.ImageButton btn_risp_dx;
        protected System.Web.UI.WebControls.Panel pnl_text_risposta;
        protected System.Web.UI.HtmlControls.HtmlTable tbl_note;
        protected DocsPaWebCtrlLibrary.ImageButton btn_protocollaDisabled;
        protected DocsPaWebCtrlLibrary.ImageButton btn_spedisciDisabled;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hdConfirmSpedisci;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hdnSpedisciConInterop;
        //  protected System.Web.UI.HtmlControls.HtmlInputHidden hdConfirmSpedisciAcquisito;

        protected static string separatore = "----------------";
        protected DocsPaWebCtrlLibrary.ImageButton imgFasc;
        protected DocsPaWebCtrlLibrary.ImageButton btn_notifica_sped;
        protected DocsPaWebCtrlLibrary.ImageButton btn_notifica_sped_CC;
        protected DocsPaWebCtrlLibrary.ImageButton btn_el_spediz;
        protected DocsPaWebCtrlLibrary.ImageButton btn_notifica;
        protected DocsPaWebCtrlLibrary.ImageButton btn_Correttore;
        protected DocsPAWA.documento.Oggetto ctrl_oggetto;
        protected System.Web.UI.WebControls.Label lbl_mezzoSpedizione;
        protected System.Web.UI.WebControls.Panel pnl_mezzoSpedizione;
        private bool isFascRapidaRequired = false;
        protected System.Web.UI.WebControls.Label labelFascRapid;
        protected System.Web.UI.HtmlControls.HtmlInputHidden isFascRequired;
        private bool isMezzoSpedizioneRequired = false;

        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] ListaFiltri;

        protected string mittType = "mitt";
        protected System.Web.UI.WebControls.TextBox txt_codModello;
        protected bool isRiproposto;

        protected DocsPAWA.ActivexWrappers.ClientModelProcessor clientModelProcessor;

        protected System.Web.UI.WebControls.HiddenField abilitaModaleVis;
        protected System.Web.UI.WebControls.HiddenField estendiVisibilita;
        protected System.Web.UI.WebControls.HiddenField appoIdAmm;
        protected System.Web.UI.WebControls.HiddenField appoIdMod;
        protected System.Web.UI.WebControls.HiddenField isInterno;
        protected System.Web.UI.WebControls.HiddenField fieldsOK;
        protected Registro regSelezionato;

        protected System.Web.UI.WebControls.Panel pnl_protocolloTitolario;
        protected System.Web.UI.WebControls.Label lbl_etProtTitolario;
        protected System.Web.UI.WebControls.Label lbl_txtProtTitolario;
        protected DocsPaWebCtrlLibrary.ImageButton btn_titolario;

        protected System.Web.UI.WebControls.Panel pnl_riferimentoMittente;
        protected System.Web.UI.WebControls.TextBox txt_riferimentoMittente;
        protected System.Web.UI.WebControls.Button btnShowFilters;

        protected System.Web.UI.WebControls.ListItem rbIn;
        protected System.Web.UI.WebControls.ListItem rbOut;
        protected System.Web.UI.WebControls.ListItem rbOwn;
        protected string eti_arrivo;
        protected string eti_partenza;
        protected string eti_interno;
        protected int numeroRuoliDestInTrasmissione = 0;
        protected int numeroUtentiConNotifica = 0;
        protected string idPeopleNewOwner = string.Empty;

        //Mittenti Multipli
        protected System.Web.UI.HtmlControls.HtmlImage btn_RubrMittMultiplo;
        protected bool btn_RubrMittMultiplo_state = true;
        protected System.Web.UI.WebControls.Panel panel_DettaglioMittenti;
        protected System.Web.UI.WebControls.ListBox lbx_mittMultiplo;
        //protected System.Web.UI.WebControls.TextBox txt_codMittMultiplo;
        //protected System.Web.UI.WebControls.TextBox txt_descMittMultiplo;
        protected DocsPaWebCtrlLibrary.ImageButton btn_CancMittMultiplo;
        //protected DocsPaWebCtrlLibrary.ImageButton btn_insMittMultiplo;
        protected DocsPaWebCtrlLibrary.ImageButton btn_StoriaMittMultipli;
        protected DocsPaWebCtrlLibrary.ImageButton btn_dettMittMultipli;
        protected DocsPaWebCtrlLibrary.ImageButton btn_trasmetti;

        protected System.Web.UI.WebControls.Panel OraPervenuto;
        protected UserControls.RubricaVeloce rubrica_veloce;
        protected UserControls.RubricaVeloce rubrica_veloce_destinatario;
        protected UserControls.RubricaVeloce rubrica_veloce_mitt_multi;

        protected System.Web.UI.WebControls.ImageButton img_busta_pec;

        protected DocsPaWebCtrlLibrary.ImageButton btn_upMittente;
        protected DocsPaWebCtrlLibrary.ImageButton btn_downMittente;

        protected DocsPaWebCtrlLibrary.ImageButton btn_risp_grigio;

        protected System.Web.UI.WebControls.Button btn_nascosto_mitt_multipli;
        protected System.Web.UI.WebControls.HiddenField txt_cod_mitt_mult_nascosto;
        protected System.Web.UI.WebControls.HiddenField txt_desc_mitt_mult_nascosto;

        protected System.Web.UI.WebControls.Panel pnl_fasc_Primaria;
        protected System.Web.UI.WebControls.Label lbl_fasc_Primaria;
        protected DocsPaWebCtrlLibrary.ImageButton imgNewFasc;

        protected bool ricevutaPdf;

        protected System.Web.UI.WebControls.Label lblStatoConsolidamento;
        protected DocsPAWA.UserControls.DocumentConsolidation documentConsolidationCtrl;
        protected AjaxControlToolkit.AutoCompleteExtender mittente_veloce;
        protected System.Web.UI.WebControls.Panel pnl_mittente_veloce;
        protected System.Web.UI.WebControls.Panel pnl_destinatario_veloce;
        protected AjaxControlToolkit.AutoCompleteExtender destinatario_veloce;

        protected System.Web.UI.WebControls.HiddenField hiddenIdCodMit_p;
        protected bool daAggiornareDx;
        protected DocsPaWebCtrlLibrary.ImageButton btn_delTipologyDoc;
        protected Utilities.MessageBox msg_rimuoviTipologia;

        protected DocsPAWA.UserControls.AppTitleProvider appTitleProvider;
        protected System.Web.UI.WebControls.HiddenField hiddenDest;
        protected System.Web.UI.WebControls.HiddenField hiddenMitt;

        public string StampaSegnatura
        {
            get
            {
                return srcStampaSegnatura;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private System.Boolean spedisciConInterop()
        {
            System.Boolean retValue = false;
            if (Request["hdnSpedisciConInterop"] != null && Request["hdnSpedisciConInterop"] == "1")
            {
                retValue = true;
            }
            return retValue;
        }

        private System.Boolean spedisciConInterop_new()
        {
            System.Boolean retValue = false;
            if (this.hdnSpedisciConInterop != null && hdnSpedisciConInterop.Value == "1")
            {
                retValue = true;
            }
            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ValidazioneProtocollazioneInterna()
        {
            DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];

            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            bool protoInterno = ws.IsInternalProtocolEnabled(cr.idAmministrazione);

            if (!protoInterno) this.rbl_InOut_P.Items.Remove(this.rbl_InOut_P.Items[2]);

        }

        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            logger.Info("BEGIN");
            this.daAggiornareDx = true;
            DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();
            info = UserManager.getInfoUtente(this.Page);
            string valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_RICEVUTA_PROTOCOLLO_PDF");
            if (valoreChiave != null)
            {
                if (valoreChiave.Equals("0")) ricevutaPdf = false;
                else ricevutaPdf = true;
            }
            if (!IsPostBack)
            {
                Session.Remove("refreshDxPageProf");
                //Session.Remove("rubrica.campoCorrispondente");
                //Session.Remove("CorrSelezionatoDaMulti");
            }
            //inizializzo lo user control
            ctrl_oggetto = this.GetControlOggetto();
            //Attivo la ricerca sul codice oggetto
            ctrl_oggetto.cod_oggetto_postback = true;
            //Imposto l'aspetto del controllo oggetto
            ctrl_oggetto.DimensioneOggetto("default", "proto");

            this.InitializeControlConsolidation();
            this.MaintainScrollPositionOnPostBack = true;
            string valoreNascosto = this.hdn_returnConfermaSped.Value;

            //luluciani 24/11/2005:
            Page.Response.Expires = -1;

            this.RegisterClientScript("nascondi", "nascondi();");
            //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"');");
            Session.Remove("validCodeFasc");
            string defaultError = "protocollazione";
            bool protoInterno = false;
            enableUfficioRef = ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF);

            if (Session["docRiproposto"] != null && Session["docRiproposto"].ToString().ToLower().Equals("true"))
                isRiproposto = true;

            string isNew = Request.QueryString["isNew"];

            string newIdCorr = Request.QueryString["newIdCorr"];
            string editMode = Request.QueryString["editMode"];
            schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);
            if (!string.IsNullOrEmpty(editMode))
            {
                if (Convert.ToBoolean(editMode))
                {
                    schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);
                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatari = true;
                    DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                    DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                }
                this.ViewState["EditMode"] = Convert.ToBoolean(editMode);
            }
            else
                this.ViewState["EditMode"] = false;

            if (isNew == "1" && !IsPostBack && !DocumentManager.onSessionRepositoryContext())
            {
                if (!(Request.QueryString["protocolla"] != null && Request.QueryString["protocolla"] == "1"))
                {
                    DocumentManager.removeDocumentoSelezionato(this);
                    DocumentManager.removeDocumentoInLavorazione(this);
                    FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                    Session.Remove("Modello");
                    Session.Remove("validCodeFasc");
                }
            }

            if (newIdCorr != null)
            {
                Corrispondente newMitt = UserManager.getCorrispondenteBySystemID(this, newIdCorr);
                ((DocsPAWA.DocsPaWR.ProtocolloEntrata) schedaDocumento.protocollo).mittente = newMitt;
                ((DocsPAWA.DocsPaWR.ProtocolloEntrata) schedaDocumento.protocollo).daAggiornareMittente = true;
                
                //TODO Reimpostare il campo VAR_INSERT_INTEROP a zero per questo mitt
                DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
                int rows = ws.ResetCorrVarInsertIterop(newIdCorr, "NULL");

                ////TODO Reimpostare il campo VAR_INSERT_INTEROP a null per questo mitt
                //DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
                //int rows = ws.ResetCorrVarInsertIterop(newIdCorr, "NULL");
            }
            else if (Session["NewIdCorrK2"] != null)
            {
                Corrispondente newMitt = UserManager.getCorrispondenteBySystemID(this, Session["NewIdCorrK2"].ToString());
                ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente = newMitt;
                ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = true;
                Session.Remove("NewIdCorrK2");
            }
            else if (Session["NewIdCorrNoInteropK2"] != null)
            {
                Corrispondente newMitt = UserManager.getCorrispondenteBySystemID(this, Session["NewIdCorrNoInteropK2"].ToString());
                ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente = newMitt;
                ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = true;
                Session.Remove("NewIdCorrNoInteropK2");
            }

            //TODO Reimpostare il campo VAR_INSERT_INTEROP a zero per questo mitt
           
            if (Request.QueryString["protocolla"] != null && Request.QueryString["protocolla"] == "1")
            {
                if (Session["dettaglioNota"] != null)
                {
                    this.dettaglioNota = (DocsPAWA.Note.DettaglioNota)Session["dettaglioNota"];
                    Session.Remove("dettaglioNota");
                }
            }

            string valoreChiaveFasc = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");
            if (valoreChiaveFasc != null && valoreChiaveFasc.ToUpper().Equals("TRUE"))
                this.isFascRapidaRequired = true;
            else
                this.isFascRapidaRequired = false;

            if (isFascRapidaRequired)
            {
                this.isFascRequired.Value = "true";
                this.labelFascRapid.Text = "Fascicolazione Rapida *";
            }
            else
            {
                this.isFascRequired.Value = "false";
                this.labelFascRapid.Text = "Fascicolazione Rapida";
            }

            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.MEZZO_SPEDIZIONE) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.MEZZO_SPEDIZIONE).ToUpper().Equals("1"))
                this.isMezzoSpedizioneRequired = true;
            else
                this.isMezzoSpedizioneRequired = false;

            if (System.Configuration.ConfigurationManager.AppSettings["PROT_DATA_ORA_MODIFICA"] != null
                    && System.Configuration.ConfigurationManager.AppSettings["PROT_DATA_ORA_MODIFICA"] == "1")
            {
                this.OraPervenuto.Visible = true;
            }
            else this.OraPervenuto.Visible = false;

            //abilito il pulsante di creazione diretta dei fascicoli procedimentali
            if (utils.InitConfigurationKeys.GetValue("0", "FE_NUOVO_FASC_DIRECT") != null && utils.InitConfigurationKeys.GetValue("0", "FE_NUOVO_FASC_DIRECT").Equals("1"))
            {
                this.imgNewFasc.Visible = true;
            }

            if (!IsPostBack)
            {
                if (!ricevutaPdf)
                    this.btn_stampa_ricevuta.Attributes.Add("onclick", "return StampaRicevuta();");
                else
                    this.btn_stampa_ricevuta.Attributes.Add("onclick", "return StampaRicevutaPdf();");

                //if(!FascicoliManager.GetFolderViewTracing(this)) this.btn_BackToFolder.Visible = false;
                //this.ValidazioneProtocollazioneInterna();
                DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
                DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                protoInterno = ws.IsInternalProtocolEnabled(cr.idAmministrazione);

                if (!protoInterno)
                    this.rbl_InOut_P.Items.Remove(this.rbl_InOut_P.Items[2]);

                if (Convert.ToBoolean(this.ViewState["EditMode"]))
                    this.btn_salva_P.Enabled = true;

                //ViewState["EditMode"] = false;

                // se PROTO_OWN o PROTO_IN o PROTO_OUT
                if (this.IsRoleInternalEnabled() || this.IsRoleInwardEnabled() || this.IsRoleOutwardEnabled())
                {
                    this.btn_protocolla_P.Enabled = true;
                    this.btn_protocollaGiallo_P.Enabled = true;
                }
                else
                {
                    this.btn_protocolla_P.Enabled = false;
                    this.btn_protocollaGiallo_P.Enabled = false;
                }
                //Fine Celeste

                //Abilito il pulsante di protocollo in risposta grigio
                if (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null
                    && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1")
                {
                    this.btn_risp_grigio.Visible = true;
                }
            }

            if (!this.IsRoleInwardEnabled())
                this.rbl_InOut_P.Items[0].Enabled = false;

            if (!this.IsRoleOutwardEnabled())
                this.rbl_InOut_P.Items[1].Enabled = false;

            // L'oggetto in sessione viene creato su oggettario.aspx.cs
            if (Session["saveButtonEnabled"] != null && (bool)Session["saveButtonEnabled"]) this.m_modifica = true;
            Session.Remove("saveButtonEnabled");

            if (!IsPostBack)
            {
                this.ctrl_oggetto.oggetto_SetControlFocus();
                CaricaComboTipologiaAtto(this.ddl_tipoAtto);
                if (this.isMezzoSpedizioneRequired)
                {
                    this.pnl_mezzoSpedizione.Visible = true;
                    CaricaComboMezzoSpedizione(this.ddl_spedizione);
                }
                else
                    this.pnl_mezzoSpedizione.Visible = false;
            }
            else
                if (this.rbl_InOut_P.SelectedValue.Equals("In") && this.isMezzoSpedizioneRequired)
                    this.pnl_mezzoSpedizione.Visible = true;
                else
                    this.pnl_mezzoSpedizione.Visible = false;

            if (this.Page.IsPostBack && (!this.h_tipoAtto.Value.Equals("") && !this.h_tipoAtto.Value.Equals("N")))
            {
                CaricaComboTipologiaAtto(this.ddl_tipoAtto);
                this.h_tipoAtto.Value = "N";
            }

            try
            {
                // documento in lavorazione
                schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);

                isEnableRubricaVeloce();

                if (schedaDocumento == null)
                {
                    this.btn_log.Visible = false;
                    this.btn_inoltra.Visible = false;
                }
                string confirm = this.hdConfirmSpedisci.Value;
                //string confirmAcquisito = this.hdConfirmSpedisciAcquisito.Value;

                if (spedisciConInterop())
                {
                    try
                    {
                        if (confirm != null && !confirm.Equals("") && confirm.Equals("Yes"))
                        {
                            spedisciDoc();
                        }
                    }
                    catch
                    {
                        string script = "<script>if(window.parent.parent.document.getElementById('please_wait')!=null)";
                        script += "{";
                        script += "		window.parent.parent.document.getElementById('please_wait').style.display = 'none'";
                        script += "} </script>";
                        Response.Write(script);

                        defaultError = "spedizione";
                        throw new Exception("Errore durante la spedizione di un documento. <BR><BR>Ripetere l'operazione di spedizione.");
                    }
                }

                Utils.startUp(this);

                if (this.isMezzoSpedizioneRequired)
                    if (schedaDocumento != null && !this.Page.IsPostBack)
                    {
                        if (schedaDocumento.descMezzoSpedizione != null && !schedaDocumento.descMezzoSpedizione.Equals("Errore"))
                        {
                            if (!schedaDocumento.descMezzoSpedizione.Equals("") && schedaDocumento.tipoProto.Equals("A"))
                            {
                                this.lbl_mezzoSpedizione.Text = schedaDocumento.descMezzoSpedizione.ToUpper();
                                this.ddl_spedizione.Visible = false;
                                this.lbl_mezzoSpedizione.Visible = true;
                            }
                            else
                                this.pnl_mezzoSpedizione.Visible = false;
                        }
                        else
                            if (schedaDocumento.rispostaDocumento != null)
                                this.pnl_mezzoSpedizione.Visible = true;
                            else
                                this.pnl_mezzoSpedizione.Visible = false;
                    }


                if (Session["docRiproposto"] != null)
                    if (this.isMezzoSpedizioneRequired)
                        if ((bool)Session["docRiproposto"] && schedaDocumento.tipoProto.Equals("A"))
                        {
                            this.lbl_mezzoSpedizione.Visible = false;
                            this.ddl_spedizione.Visible = true;
                            if (!string.IsNullOrEmpty(schedaDocumento.mezzoSpedizione))
                                for (int i = 0; i < this.ddl_spedizione.Items.Count; i++)
                                {
                                    if (this.ddl_spedizione.Items[i].Value == schedaDocumento.mezzoSpedizione.ToString())
                                        this.ddl_spedizione.Items[i].Selected = true;
                                }
                            //this.ddl_spedizione.SelectedValue = schedaDocumento.mezzoSpedizione.ToString();
                            //Session.Remove("docRiproposto");
                        }
                        else
                            this.pnl_mezzoSpedizione.Visible = false;

                if (schedaDocumento == null) nuovaSchedaDocumento();
                if (schedaDocumento.registro == null || (schedaDocumento.registro != null && schedaDocumento.registro.systemId == null)) schedaDocumento.registro = UserManager.getRegistroSelezionato(this);

                // Gestione visualizzazione documento protocollato
                if (schedaDocumento.protocollo == null)
                {
                    //							DocsPaWR.Corrispondente cor = (DocsPAWA.DocsPaWR.Corrispondente) this.Session["userData"];
                    //							// Gestione validazione tipo protocollo
                    //							DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();						
                    //							bool protoInterno = ws.IsInternalProtocolEnabled(cor.idAmministrazione);	

                    if (this.IsRoleInwardEnabled())
                    {
                        this.SetAsProtoIn();
                    }
                    else
                    {
                        if (this.IsRoleOutwardEnabled())
                        {
                            this.SetAsProtoOut(); // Default to 'Uscita'
                            SettaMittenteDefault("P");
                        }
                        else if (this.IsRoleInternalEnabled() && protoInterno)
                        {
                            this.SetAsProtoOwn(); // Default to 'Interno'
                        }
                        else
                        {
                            /* Questa eventualita' puo' verificarsi solo a seguito di un eventuale bug
                             * dell'applicazione che permettesse ad un utente non abilitato alla protocollazione
                             * di visualizzare comunque questa form.
                             */

                            throw new Exception("L'utente non e' abilitato alla protocollazione.");
                        }
                    }
                }
                else
                {
                    switch (schedaDocumento.tipoProto)
                    {
                        case "P":
                            schedaDocumento.tipoProto = "P";
                            this.btn_inoltra.Visible = false;
                            if (!IsPostBack && schedaDocumento.protocollo.numero == null)
                                SettaMittenteDefault("P");
                            break;

                        case "I":
                            schedaDocumento.tipoProto = "I";
                            this.btn_inoltra.Visible = false;
                            break;

                        case "A":
                            schedaDocumento.tipoProto = "A";
                            if (System.Configuration.ConfigurationManager.AppSettings["INOLTRA_DOC"] != null && System.Configuration.ConfigurationManager.AppSettings["INOLTRA_DOC"] == "1" && UserManager.ruoloIsAutorized(this, "DO_PROT_PROTOCOLLA"))
                                this.btn_inoltra.Visible = true;
                            else
                                this.btn_inoltra.Visible = false;
                            break;

                        case "G":
                            if (schedaDocumento.protocollo != null && schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloEntrata))
                                schedaDocumento.tipoProto = "A";
                            if (schedaDocumento.protocollo != null && schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloUscita))
                                schedaDocumento.tipoProto = "P";
                            if (System.Configuration.ConfigurationManager.AppSettings["INOLTRA_DOC"] != null && System.Configuration.ConfigurationManager.AppSettings["INOLTRA_DOC"] == "1")
                                this.btn_inoltra.Visible = true;
                            else
                                this.btn_inoltra.Visible = false;
                            break;

                        default: // "A"
                            //if (System.Configuration.ConfigurationManager.AppSettings["INOLTRA_DOC"] != null && System.Configuration.ConfigurationManager.AppSettings["INOLTRA_DOC"] == "1")
                            //    this.btn_inoltra.Visible = true;
                            //else
                            //    this.btn_inoltra.Visible = false;
                            break;
                    }
                }

                setDataProtocollo();

                if (!IsPostBack)
                {
                    ViewState["varFromInteropPecMit"] = null;
                    ViewState["varFromInteropPecProtMitt"] = null;
                    ViewState["varFromInteropPecDataMitt"] = null;
                    ViewState["varFromInteropPecDataArrivo"] = null;
                    //Se ricevuto per interoperabilità con pec questi campi non sono modificabili se valorizzati
                    if (this.FromInteropPecOrSimpInterop(schedaDocumento))
                    {
                        if (string.IsNullOrEmpty(schedaDocumento.protocollo.segnatura))
                        {
                            ViewState["varFromInteropPecMit"] = true;
                        }

                        if (!string.IsNullOrEmpty(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).descrizioneProtocolloMittente))
                        {
                            ViewState["varFromInteropPecProtMitt"] = true;
                        }

                        if (!string.IsNullOrEmpty(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).dataProtocolloMittente))
                        {
                            ViewState["varFromInteropPecDataMitt"] = true;
                        }

                        if (schedaDocumento != null && schedaDocumento.documenti != null && schedaDocumento.documenti.Length > 0 && !string.IsNullOrEmpty(schedaDocumento.documenti[0].dataArrivo))
                        {
                            ViewState["varFromInteropPecDataArrivo"] = true;
                        }
                    }
                }

                enableEditableFields();

                if (!Page.IsPostBack)
                {
                    this.caricaModelliTrasm();
                    this.caricaComboTemplate();

                    //this.CaricaComboRispostaProtocollo(this.ddl_RispProtSegn_P, schedaDocumento.protocollo.rispostaProtocollo);
                    if (Convert.ToBoolean(this.ViewState["EditMode"]))
                    {
                        this.btn_salva_P.Enabled = true;
                    }
                }
                //				}

                if (Session["oggettario.retValue"] != null)
                {
                    if ((bool)Session["oggettario.retValue"])
                    {
                        this._oggettoSelezionatoDaOggettario = true;
                    }

                    Session.Remove("oggettario.retValue");
                }

                //PROFILAZIONE DINAMICA
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null)
                {
                    if (!IsPostBack)
                    {
                        if (!(Request.QueryString["protocolla"] != null && Request.QueryString["protocolla"] == "1"))
                            if (Session["templateRiproposto"] == null)
                                Session.Remove("template");
                    }
                    //Disabilito il pulsante che permette da questa pagina l'inserimento di una nuova tipologia di atto
                    //btn_addTipoAtto.Visible = false;
                }
                else
                {
                    Panel_TipoAtto.Visible = false;
                    Panel_ProfilazioneDinamica.Visible = false;
                }
                //FINE PROFILAZIONE DINAMICA			

                //  Gestione deallocazione risorse utilizzata dalla dialog risposta al protocollo
                this.ClearResourcesRicercaProtocolliUscita();

                tastoInvio();

                // Inizializzazione controllo verifica acl
                if ((schedaDocumento != null) && (schedaDocumento.systemId != null) && (schedaDocumento.inCestino != "1"))
                    this.InitializeControlAclDocumento();

                //Protocollo Titolario
                if (!IsPostBack)
                {
                    string protocolloTitolario = DocumentManager.GetProtocolloTitolario(schedaDocumento);
                    if (!string.IsNullOrEmpty(protocolloTitolario))
                    {
                        pnl_protocolloTitolario.Visible = true;
                        lbl_etProtTitolario.Text = wws.isEnableProtocolloTitolario();
                        lbl_txtProtTitolario.Text = protocolloTitolario;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, defaultError);
            }


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

                if (isInterno.Value == "true")
                {
                    if (this.wws.ereditaVisibilita(appoIdAmm.Value, appoIdMod.Value))
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

            //Nodo titolario scelto
            DocsPAWA.DocsPaWR.FascicolazioneClassificazione classificazione = DocumentManager.getClassificazioneSelezionata(this);
            if (classificazione != null && !string.IsNullOrEmpty(classificazione.codice))
            {
                this.txt_CodFascicolo.Text = classificazione.codice;
                //this.txt_DescFascicolo.Text = classificazione.descrizione;
                //OLD:   Fascicolo Fasc = FascicoliManager.getFascicoloDaCodice(this,);

                Fascicolo[] FascS = FascicoliManager.getListaFascicoliDaCodice(this, classificazione.codice, schedaDocumento.registro, "I");

                Fascicolo Fasc = null;

                if (FascS != null && FascS.Length > 0 && FascS[0] != null)
                {
                    Fasc = (Fascicolo)FascS[0];
                }

                FascicoliManager.setFascicoloSelezionatoFascRapida(Fasc);
                DocumentManager.setClassificazioneSelezionata(this, null);
            }

            /*
            //Per la profilazione dinamica, se di proviene dalla popup di profilazione ed
            //è stato selezionato il pulsante conferma, il bottone salva di questa pagina
            //viene abilitato
            if( Session["modificaProfilazione"] != null && (bool) Session["modificaProfilazione"] )
            {
                btn_salva_P.Enabled = true;
            }
            */

            //CONTROLLO DATA ORA
            if ((Session["ifChooseRf"] == null) && (Session["ProtoExist"] == null))
            {
                if (schedaDocumento != null && (string.IsNullOrEmpty(schedaDocumento.docNumber) || Convert.ToBoolean(this.ViewState["EditMode"])) && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.tipoProto) && schedaDocumento.tipoProto.Equals("A") && schedaDocumento.documenti != null && schedaDocumento.documenti.Length > 0)
                {
                    if (!string.IsNullOrEmpty(this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text))
                    {
                        string oraPerv = Utils.getTime(this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text + " " + this.txt_OraPervenuto_P.Text);
                        schedaDocumento.documenti[0].dataArrivo = this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text + " " + oraPerv;
                    }
                    else
                    {
                        schedaDocumento.documenti[0].dataArrivo = "";
                    }
                }
                Session.Remove("ifChooseRf");
            }

            DocumentManager.setDocumentoInLavorazione(schedaDocumento);
            DocumentManager.setDocumentoSelezionato(schedaDocumento);

            if (!IsPostBack)
            {
                //Riferimento Mittente
                impostaRiferimentoMittente();
            }
            getLettereProtocolli();

            //Gestione bottone freccia destra in risposta
            if (System.Configuration.ConfigurationManager.AppSettings["NON_VISUALIZZA_FRECCIA_RISPOSTA"] != null && System.Configuration.ConfigurationManager.AppSettings["NON_VISUALIZZA_FRECCIA_RISPOSTA"] == "1")
            {
                this.btn_risp_dx.Visible = false;
            }

            string abilita_multi_stampa_etichetta = utils.InitConfigurationKeys.GetValue("0", "FE_MULTI_STAMPA_ETICHETTA");
            //Se la stampa multipla delle etichette è disabilitata, rendo invisibile il campo per l'inserimento del numero di stampe da effettuare
            if (abilita_multi_stampa_etichetta.Equals("0") || string.IsNullOrEmpty(abilita_multi_stampa_etichetta))
                txt_num_stampe.Visible = false;

            // Se si è arrivati sul page load dopo che l'utente abbia risposto alla notifica di documento
            // marcato privato dalla AOO mittente, si procede con l'analisi della risposta dell'utente
            if (!String.IsNullOrEmpty(Request["userChoice"]))
            {
                int signAsPrivate = 0;
                Int32.TryParse(Request["userChoice"], out signAsPrivate);

                schedaDocumento.privato = signAsPrivate == 6 ? "1" : "0";
                protocollaDoc();
            }

            //Prova Andrea Messaggio errore per Trasmissioni
            if (Session["MessError"] != null)
            {
                messError = Session["MessError"].ToString();
                //Response.Write("<script language=\"javascript\">alert('Trasmissioni avvenute con successo per tutti i destinatari ad eccezione delle seguenti: \\n" + messError + "');</script>");
                Response.Write("<script language=\"javascript\">alert('Trasmissioni con esito negativo: \\n" + messError + "\\n');</script>");
                Session.Remove("MessError");
            }
            //End Andrea

            logger.Info("END");
        }

        /// <summary>
        /// Gestione deallocazione risorse utilizzata dalla dialog risposta al protocollo (13/02/2006)
        /// </summary>
        private void ClearResourcesRicercaProtocolliUscita()
        {
            if (DocsPAWA.popup.RicercaProtocolliUscita.RicercaProtocolliUscitaSessionMng.IsLoaded(this))
            {
                DocsPAWA.popup.RicercaProtocolliUscita.RicercaProtocolliUscitaSessionMng.SetAsNotLoaded(this);
                DocsPAWA.popup.RicercaProtocolliUscita.RicercaProtocolliUscitaSessionMng.ClearSessionData(this);
            }
        }

        private void spedisciDoc()
        {
            try
            {
                this.hdConfirmSpedisci.Value = string.Empty;
                this.hdnSpedisciConInterop.Value = string.Empty;

                if (!this.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "ShowDialogSpedizioneDocumento"))
                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ShowDialogSpedizioneDocumento", "<script language='javascript'>ShowDialogSpedizioneDocumento();</script>");

            }
            catch (Exception e)
            {
                throw new Exception("Errore durante la spedizione di un documento. <BR><BR>Ripetere l'operazione di spedizione.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void nuovaSchedaDocumento()
        {
            schedaDocumento = DocumentManager.getDocumentoSelezionato(this);

            if (schedaDocumento == null)
            {
                schedaDocumento = DocumentManager.NewSchedaDocumento(this.Page);

                DocumentManager.setDocumentoInLavorazione(schedaDocumento);
                DocumentManager.setDocumentoSelezionato(schedaDocumento);
                FileManager.setSelectedFile(this, schedaDocumento.documenti[0]);

                ////crea nuovo documento
                //DocsPaWR.Utente utente = UserManager.getUtente(this);
                //DocsPaWR.Ruolo ruolo = UserManager.getRuolo(this);

                //schedaDocumento = new DocsPAWA.DocsPaWR.SchedaDocumento();

                //schedaDocumento.systemId = null;
                //schedaDocumento.oggetto = new DocsPAWA.DocsPaWR.Oggetto();

                //// campi obbligatori per DocsFusion
                //schedaDocumento.idPeople = utente.idPeople;
                //schedaDocumento.userId = utente.userId;
                ////schedaDocumento.typeId = "LETTERA";
                //schedaDocumento.typeId = DocumentManager.getTypeId();
                //schedaDocumento.appId = "ACROBAT";
                //schedaDocumento.privato = "0";  // doc non privato
                //schedaDocumento.personale = "0";  // doc non personale
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        #region per binding pulsanti
        //		this.btn_BackToQuery.Click += new System.Web.UI.ImageClickEventHandler(this.btn_BackToQuery_Click);
        //		this.btn_BackToFolder.Click += new System.Web.UI.ImageClickEventHandler(this.btn_BackToFolder_Click);
        //		this.rbl_InOut_P.SelectedIndexChanged += new System.EventHandler(this.rbl_InOut_P_SelectedIndexChanged);
        //		this.btn_modificaOgget_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_modificaOgget_P_Click);
        //		this.btn_stampaSegn_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_stampaSegn_P_Click);
        //		this.txt_dataSegn.TextChanged += new System.EventHandler(this.txt_dataSegn_TextChanged);
        //		this.btn_RubrOgget_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_RubrOgget_P_Click);
        //		this.btn_storiaOgg_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_storiaOgg_P_Click);
        //		this.txt_oggetto_P.TextChanged += new System.EventHandler(this.txt_oggetto_P_TextChanged);
        //		//this.btn_DetMit_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_DetMit_P_Click);
        //		this.txt_CodMit_P.TextChanged += new System.EventHandler(this.txt_CodMit_P_TextChanged);
        //		this.txt_DescMit_P.TextChanged += new System.EventHandler(this.txt_DescMit_P_TextChanged);
        //		this.txt_CodMitInt_P.TextChanged += new System.EventHandler(this.txt_CodMitInt_P_TextChanged);
        //		this.txt_DescMitInt_P.TextChanged += new System.EventHandler(this.txt_DescMitInt_P_TextChanged);
        //		this.txt_DataProtMit_P.TextChanged += new System.EventHandler(this.txt_DataProtMit_P_TextChanged);
        //		this.txt_NumProtMit_P.TextChanged += new System.EventHandler(this.txt_NumProtMit_P_TextChanged);
        //		this.txt_DataArrivo_P.TextChanged += new System.EventHandler(this.txt_DataArrivo_P_TextChanged);
        //		this.btn_aggiungiDest_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungiDest_P_Click);
        //		this.txt_CodDest_P.TextChanged += new System.EventHandler(this.txt_CodDest_P_TextChanged);
        //		this.txt_DescDest_P.TextChanged += new System.EventHandler(this.txt_DescDest_P_TextChanged);
        //		this.btn_dettDest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_dettDest_Click);
        //		this.btn_cancDest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_cancDest_Click);
        //		this.btn_insDestCC.Click += new System.Web.UI.ImageClickEventHandler(this.btn_insDestCC_Click);
        //		this.btn_insDest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_insDest_Click);
        //		this.btn_cancDestCC.Click += new System.Web.UI.ImageClickEventHandler(this.btn_cancDestCC_Click);
        //		this.chkEvidenza.CheckedChanged += new System.EventHandler(this.chkEvidenza_CheckedChanged);
        //		this.ddl_tipoAtto.SelectedIndexChanged += new System.EventHandler(this.ddl_tipoAtto_SelectedIndexChanged);
        //		this.btn_addTipoAtto.Click += new System.Web.UI.ImageClickEventHandler(this.btn_addTipoAtto_Click);
        //		this.btn_Risp.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Risp_Click);
        //		//this.btn_risp_dx.Click += new System.Web.UI.ImageClickEventHandler(this.btn_risp_dx_Click);
        //		this.btn_risp_sx.Click += new System.Web.UI.ImageClickEventHandler(this.btn_risp_sx_Click);
        //		this.txt_CodFascicolo.TextChanged += new System.EventHandler(this.txt_cod_fasc_TextChanged);
        //		//this.ddl_tmpl.SelectedIndexChanged += new System.EventHandler(this.ddl_tmpl_SelectedIndexChanged);
        //		this.btn_salva_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_salva_P_Click);
        //		this.btn_protocolla_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_protocolla_P_Click);
        //		this.btn_protocollaGiallo_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_protocollaGiallo_P_Click);
        //		this.btn_aggiungi_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungi_P_Click);
        //		this.btn_spedisci_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_spedisci_P_Click);
        //		this.btn_riproponiDati_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_riproponiDati_P_Click);
        //		this.btn_annulla_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_annulla_P_Click);
        //		this.Load += new System.EventHandler(this.Page_Load);
        //		this.txt_cod_uffRef.TextChanged += new System.EventHandler(this.txt_cod_uffRef_TextChanged);
        //		this.PreRender += new System.EventHandler(this.docProtocollo_PreRender);
        //		this.btn_verificaPrec_P.Click +=new ImageClickEventHandler(this.btn_verificaPrec_P_Click);
        #endregion

        private void InitializeComponent()
        {
            this.chkPrivato.CheckedChanged += new System.EventHandler(this.chkPrivato_CheckedChanged);
            this.rbl_InOut_P.SelectedIndexChanged += new System.EventHandler(this.rbl_InOut_P_SelectedIndexChanged);
            this.btn_modificaOgget_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_modificaOgget_P_Click);
            this.btn_stampaSegn_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_stampaSegn_P_Click);
            this.txt_dataSegn.TextChanged += new System.EventHandler(this.txt_dataSegn_TextChanged);
            this.btn_storiaOgg_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_storiaOgg_P_Click);
            //this.txt_oggetto_P.TextChanged += new System.EventHandler(this.txt_oggetto_P_TextChanged);
            this.ctrl_oggetto.OggettoChangedEvent += new Oggetto.OggettoChangedDelegate(this.ctrl_oggetto_OggChanged);
            this.txt_CodMit_P.TextChanged += new System.EventHandler(this.txt_CodMit_P_TextChanged);
            this.txt_CodMitInt_P.TextChanged += new System.EventHandler(this.txt_CodMitInt_P_TextChanged);
            this.txt_DescMit_P.TextChanged += new System.EventHandler(this.txt_DescMit_P_TextChanged);
            this.msg_TrasmettiRapida.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_TrasmettiRapida_GetMessageBoxResponse);
            this.msg_TrasmettiProto.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_TrasmettiProto_GetMessageBoxResponse);
            this.msg_copiaDoc.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_copiaDoc_GetMessageBoxResponse);
            //this.msg_SalvaDoc.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_SalvaDoc_GetMessageBoxResponse);
            this.btn_log.Click += new System.Web.UI.ImageClickEventHandler(this.btn_log_Click);
            this.txt_DescMitInt_P.TextChanged += new System.EventHandler(this.txt_DescMitInt_P_TextChanged);
            this.btn_verificaPrec_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_verificaPrec_P_Click);
            this.txt_NumProtMit_P.TextChanged += new System.EventHandler(this.txt_NumProtMit_P_TextChanged);
            this.GetCalendarControl("txt_DataProtMit_P").txt_Data.TextChanged += new System.EventHandler(this.txt_DataProtMit_P_TextChanged);
            this.GetCalendarControl("txt_DataArrivo_P").txt_Data.TextChanged += new System.EventHandler(this.txt_DataArrivo_P_TextChanged);
            //this.txt_DataArrivo_P.TextChanged += new System.EventHandler(this.txt_DataArrivo_P_TextChanged);
            this.txt_OraPervenuto_P.TextChanged += new System.EventHandler(this.txt_OraPervenuto_P_TextChanged);
            this.btn_StampaBuste_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_StampaBuste_P_Click);
            this.btn_aggiungiDest_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungiDest_P_Click);
            this.txt_CodDest_P.TextChanged += new System.EventHandler(this.txt_CodDest_P_TextChanged);
            this.txt_DescDest_P.TextChanged += new System.EventHandler(this.txt_DescDest_P_TextChanged);
            this.btn_dettDest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_dettDest_Click);
            this.btn_cancDest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_cancDest_Click);
            this.btn_insDestCC.Click += new System.Web.UI.ImageClickEventHandler(this.btn_insDestCC_Click);
            this.btn_insDest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_insDest_Click);
            this.btn_dettDestCC.Click += new System.Web.UI.ImageClickEventHandler(this.btn_dettDestCC_Click);
            this.btn_cancDestCC.Click += new System.Web.UI.ImageClickEventHandler(this.btn_cancDestCC_Click);
            this.chkEvidenza.CheckedChanged += new System.EventHandler(this.chkEvidenza_CheckedChanged);
            this.btn_addTipoAtto.Click += new System.Web.UI.ImageClickEventHandler(this.btn_addTipoAtto_Click);
            this.ddl_tipoAtto.SelectedIndexChanged += new System.EventHandler(this.ddl_tipoAtto_SelectedIndexChanged);
            this.btn_CampiPersonalizzati.Click += new System.Web.UI.ImageClickEventHandler(this.btn_CampiPersonalizzati_Click);
            this.btn_risp_sx.Click += new System.Web.UI.ImageClickEventHandler(this.btn_risp_sx_Click);
            this.btn_risp_dx.Click += new System.Web.UI.ImageClickEventHandler(this.btn_risp_dx_Click);
            this.btn_in_rispota_a.Click += new System.Web.UI.ImageClickEventHandler(this.btn_in_rispota_a_Click);
            this.btn_Risp.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Risp_Click);
            this.txt_cod_uffRef.TextChanged += new System.EventHandler(this.txt_cod_uffRef_TextChanged);
            this.imgFasc.Click += new System.Web.UI.ImageClickEventHandler(this.imgFasc_Click);
            this.txt_CodFascicolo.TextChanged += new System.EventHandler(this.txt_cod_fasc_TextChanged);
            this.ddl_tmpl.SelectedIndexChanged += new System.EventHandler(this.ddl_tmpl_SelectedIndexChanged);
            this.btn_salva_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_salva_P_Click);
            this.btn_protocolla_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_protocolla_P_Click);
            this.btn_protocollaGiallo_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_protocollaGiallo_P_Click);
            this.btn_aggiungi_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungi_P_Click);
            this.btn_spedisci_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_spedisci_P_Click);
            this.btn_trasmetti.Click += new System.Web.UI.ImageClickEventHandler(this.btn_trasmetti_Click);
            this.btn_riproponiDati_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_riproponiDati_P_Click);
            this.btn_inoltra.Click += new System.Web.UI.ImageClickEventHandler(this.btn_inoltra_Click);
            this.btn_annulla_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_annulla_P_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.docProtocollo_PreRender);
            this.btn_annullaPred.Click += new ImageClickEventHandler(this.btn_annullaPred_Click);
            this.txt_codModello.TextChanged += new EventHandler(txt_codModello_TextChanged);
            //this.btn_Risp_ingresso.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Risp_ingresso_Click);
            // this.btn_in_rispostaIngresso_a.Click += new System.Web.UI.ImageClickEventHandler(this.btn_in_rispostaIngresso_a_Click);
            //this.btn_risp_ingresso_sx.Click += new System.Web.UI.ImageClickEventHandler(this.btn_risp_ingresso_sx_Click);
            this.ddl_spedizione.SelectedIndexChanged += new EventHandler(ddl_spedizione_SelectedIndexChanged);
            this.btn_storiaCons.Click += new System.Web.UI.ImageClickEventHandler(this.btn_storiaCons_Click);
            this.btn_tipo_sped.Click += new ImageClickEventHandler(btn_tipo_sped_Click);
            this.btn_tipo_spedCC.Click += new ImageClickEventHandler(btn_tipo_spedCC_Click);

            this.btn_stampa_ricevuta.Click += new ImageClickEventHandler(this.btn_stampa_ricevuta_Click);
            ////this.btn_tipo_sped.Attributes.Add("onclick", "ApriFinestraSceltaTipoSped(document.docProtocollo.lbx_dest.selectedIndex,'D','" + this.ViewState["EditMode"].ToString().ToLower() + "');return false");
            //this.btn_tipo_sped.Attributes.Add("onclick", "ApriFinestraSceltaTipoSped(document.docProtocollo.lbx_dest.selectedIndex,'D','true');return false");
            ////this.btn_tipo_spedCC.Attributes.Add("onclick", "ApriFinestraSceltaTipoSped(document.docProtocollo.lbx_destCC.selectedIndex,'CC','" + this.ViewState["EditMode"].ToString().ToLower() + "');return false");
            //this.btn_tipo_spedCC.Attributes.Add("onclick", "ApriFinestraSceltaTipoSped(document.docProtocollo.lbx_destCC.selectedIndex,'CC','true');return false");

            this.msg_SpedizioneAutomatica.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_SpedizioneAutomatica_GetMessageBoxResponse);
            this.btn_invio_mail.Click += new ImageClickEventHandler(btn_invio_mail_Click);
            this.GetCalendarControl("txt_DataProtMit_P").txt_Data.TextChanged += new System.EventHandler(this.txt_DataProtMit_P_TextChanged);
            //this.GetCorrispondenteControl("CorrDaCodMit").TIPO_PROTO = "A";

            //this.txt_codMittMultiplo.TextChanged += new System.EventHandler(this.txt_codMittMultiplo_TextChanged);
            //this.btn_insMittMultiplo.Click += new ImageClickEventHandler(btn_insMittMultiplo_Click);
            this.btn_CancMittMultiplo.Click += new ImageClickEventHandler(btn_CancMittMultiplo_Click);

            this.btn_upMittente.Click += new ImageClickEventHandler(btn_upMittente_Click);
            this.btn_downMittente.Click += new ImageClickEventHandler(btn_downMittente_Click);
            this.imgNewFasc.Click += new System.Web.UI.ImageClickEventHandler(this.imgNewFasc_Click);

            this.btn_risp_grigio.Click += new System.Web.UI.ImageClickEventHandler(this.btn_risp_grigio_Click);

            this.btn_nascosto_mitt_multipli.Click += new EventHandler(btn_nascosto_mitt_multipli_Click);
            this.msg_rimuoviTipologia.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_rimuoviTipologia_GetMessageBoxResponse);
            this.btn_delTipologyDoc.Click += new System.Web.UI.ImageClickEventHandler(this.btn_delTipologyDoc_Click);
        

        }

        void btn_tipo_spedCC_Click(object sender, ImageClickEventArgs e)
        {
            if (this.schedaDocumento != null)
            {
                if (this.schedaDocumento.protocollo != null && this.schedaDocumento.protocollo.segnatura != null)
                    RegisterStartupScript("apri", "<script>ApriFinestraSceltaTipoSped('" + this.ViewState["EditMode"].ToString().ToLower() + "');</script>");
                else
                    RegisterStartupScript("apri", "<script>ApriFinestraSceltaTipoSped('true');</script>");
            }
        }

        void btn_tipo_sped_Click(object sender, ImageClickEventArgs e)
        {
            if (this.schedaDocumento != null)
            {
                if (this.schedaDocumento.protocollo != null && this.schedaDocumento.protocollo.segnatura != null)
                    RegisterStartupScript("apri", "<script>ApriFinestraSceltaTipoSped('" + this.ViewState["EditMode"].ToString().ToLower() + "');</script>");
                else
                    RegisterStartupScript("apri", "<script>ApriFinestraSceltaTipoSped('true');</script>");
            }
        }

        void ddl_spedizione_SelectedIndexChanged(object sender, EventArgs e)
        {
            //schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
            schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);

            if (schedaDocumento != null)
            {
                if (this.rbl_InOut_P.SelectedValue.Equals("In") && this.isMezzoSpedizioneRequired)
                {
                    if (!this.ddl_spedizione.SelectedItem.Value.Equals(""))
                    {
                        schedaDocumento.mezzoSpedizione = this.ddl_spedizione.SelectedItem.Value;
                        schedaDocumento.descMezzoSpedizione = this.ddl_spedizione.SelectedItem.Text;
                    }
                    else
                    {
                        schedaDocumento.mezzoSpedizione = "0";
                        schedaDocumento.descMezzoSpedizione = "";
                    }
                }
            }
            DocumentManager.setDocumentoInLavorazione(schedaDocumento);
        }

        void txt_codModello_TextChanged(object sender, EventArgs e)
        {
            if (!this.txt_codModello.Text.Equals(""))
            {
                if (this.txt_codModello.Text.Length > 3 && this.txt_codModello.Text.Substring(0, 3).ToUpper().Equals("MT_"))
                {
                    // visto che il codice del modello trasmissione viene creato automaticamente con "MT_" + system_id
                    // utilizzo il già esistente metodo getModelloById che richiede il solo system_id restituendo il
                    // modello ricercato. Il system_id me lo ricavo dalla textbox txt_codModello escludendo le prime 3
                    // lettere "MT_".
                    string idModello = this.txt_codModello.Text.Substring(3);
                    DocsPaWR.ModelloTrasmissione modello = ModelliTrasmManager.getModelloByID(UserManager.getRegistroSelezionato(this).idAmministrazione, idModello, this);
                    if (modello != null)
                    {
                        this.ddl_tmpl.SelectedValue = modello.SYSTEM_ID.ToString();
                        this.txt_codModello.Text = modello.CODICE;
                    }
                    else
                    {
                        this.ddl_tmpl.SelectedIndex = 0;
                        Response.Write("<script>alert('Non esiste un modello trasmissione con questo codice');</script>");
                    }
                }
                else
                {
                    this.ddl_tmpl.SelectedIndex = 0;
                    Response.Write("<script>alert('Non esiste un modello trasmissione con questo codice');</script>");
                }
            }
            else
                this.ddl_tmpl.SelectedIndex = 0;
        }

        private void btn_annullaPred_Click(object sender, ImageClickEventArgs e)
        {
            schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
            schedaDocumento = DocumentManager.annullaPredisposizione(UserManager.getInfoUtente(), schedaDocumento);
            if (schedaDocumento != null)
            {
                DocumentManager.setDocumentoSelezionato(schedaDocumento);
                Response.Write("<script>top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=profilo';</script>");
            }
            else
                Response.Write("<script>alert('Errore nella procedura di annullamento predisposizione');</script>");

        }

        #region InitializeComponent() prima dello storico
        //		private void InitializeComponent() 
        //		{    
        //			this.btn_BackToQuery.Click += new System.Web.UI.ImageClickEventHandler(this.btn_BackToQuery_Click);
        //			this.btn_BackToFolder.Click += new System.Web.UI.ImageClickEventHandler(this.btn_BackToFolder_Click);
        //			this.rbl_InOut_P.SelectedIndexChanged += new System.EventHandler(this.rbl_InOut_P_SelectedIndexChanged);
        //			this.btn_modificaOgget_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_modificaOgget_P_Click);
        //			this.btn_stampaSegn_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_stampaSegn_P_Click);
        //			this.txt_dataSegn.TextChanged += new System.EventHandler(this.txt_dataSegn_TextChanged);
        //			this.btn_RubrOgget_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_RubrOgget_P_Click);
        //			this.btn_storiaOgg_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_storiaOgg_P_Click);
        //			this.txt_oggetto_P.TextChanged += new System.EventHandler(this.txt_oggetto_P_TextChanged);
        //			this.txt_CodMit_P.TextChanged += new System.EventHandler(this.txt_CodMit_P_TextChanged);
        //			this.txt_DescMit_P.TextChanged += new System.EventHandler(this.txt_DescMit_P_TextChanged);
        //			this.txt_CodMitInt_P.TextChanged += new System.EventHandler(this.txt_CodMitInt_P_TextChanged);
        //			this.txt_DescMitInt_P.TextChanged += new System.EventHandler(this.txt_DescMitInt_P_TextChanged);
        //			this.btn_verificaPrec_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_verificaPrec_P_Click);
        //			this.txt_DataProtMit_P.TextChanged += new System.EventHandler(this.txt_DataProtMit_P_TextChanged);
        //			this.txt_NumProtMit_P.TextChanged += new System.EventHandler(this.txt_NumProtMit_P_TextChanged);
        //			this.txt_DataArrivo_P.TextChanged += new System.EventHandler(this.txt_DataArrivo_P_TextChanged);
        //			this.btn_aggiungiDest_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungiDest_P_Click);
        //			this.txt_CodDest_P.TextChanged += new System.EventHandler(this.txt_CodDest_P_TextChanged);
        //			this.txt_DescDest_P.TextChanged += new System.EventHandler(this.txt_DescDest_P_TextChanged);
        //			this.btn_dettDest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_dettDest_Click);
        //			this.btn_cancDest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_cancDest_Click);
        //			this.btn_insDestCC.Click += new System.Web.UI.ImageClickEventHandler(this.btn_insDestCC_Click);
        //			this.btn_insDest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_insDest_Click);
        //			this.btn_cancDestCC.Click += new System.Web.UI.ImageClickEventHandler(this.btn_cancDestCC_Click);
        //			this.chkEvidenza.CheckedChanged += new System.EventHandler(this.chkEvidenza_CheckedChanged);
        //			this.btn_addTipoAtto.Click += new System.Web.UI.ImageClickEventHandler(this.btn_addTipoAtto_Click);
        //			this.btn_Risp.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Risp_Click);
        //			this.ddl_tipoAtto.SelectedIndexChanged += new System.EventHandler(this.ddl_tipoAtto_SelectedIndexChanged);
        //			this.btn_risp_sx.Click += new System.Web.UI.ImageClickEventHandler(this.btn_risp_sx_Click);
        //			this.txt_cod_uffRef.TextChanged += new System.EventHandler(this.txt_cod_uffRef_TextChanged);
        //			this.txt_CodFascicolo.TextChanged += new System.EventHandler(this.txt_cod_fasc_TextChanged);
        //			this.btn_salva_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_salva_P_Click);
        //			this.btn_protocolla_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_protocolla_P_Click);
        //			this.btn_protocollaGiallo_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_protocollaGiallo_P_Click);
        //			this.btn_aggiungi_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungi_P_Click);
        //			this.btn_spedisci_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_spedisci_P_Click);
        //			this.btn_riproponiDati_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_riproponiDati_P_Click);
        //			this.btn_annulla_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_annulla_P_Click);
        //			this.Load += new System.EventHandler(this.Page_Load);
        //			this.PreRender += new System.EventHandler(this.docProtocollo_PreRender);
        //
        //		}
        #endregion

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void docProtocollo_PreRender(object sender, System.EventArgs e)
        {
            logger.Info("BEGIN");
            schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
            string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
            //visualizzazione campo note
            this.NotaDocumentoEnabled = (ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_NOTE_PROTOCOLLO) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_NOTE_PROTOCOLLO).Equals("1"));

            if (Utils.getSpedizioneFax() == true)
                this.btn_spedisci_P.Attributes.Add("onclick", "ApriFinestraSpedisci();return false");

            img_busta_pec.Visible = false;

            //Luluciani
            //  if (UserManager.ruoloIsAutorized(this, "ACL_RIMUOVI"))
            //{
            //utente è autorizzato a rimuovere le acl per il documento 
            //verifica che ci siano ACL rimosse 
            if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.systemId))
            {
                int result = DocumentManager.verificaDeletedACL(this.schedaDocumento.systemId, UserManager.getInfoUtente(this));
                if (result == 1)
                {
                    btn_visibilita.BorderWidth = 1;
                    btn_visibilita.BorderColor = Color.Red;
                }
                else
                    btn_visibilita.BorderWidth = 0;
            }
            //}

            //string conservazione = ConfigurationManager.AppSettings["CONSERVAZIONE"];
            //if (!string.IsNullOrEmpty(conservazione) && conservazione == "1")
            //{
            //    this.btn_storiaCons.Visible = true;
            //}

            if (UserManager.ruoloIsAutorized(this, "DO_CONS"))
            {
                this.btn_storiaCons.Visible = true;
            }
            try
            {
                //Controllo se il ruolo utente è autorizzato a creare documenti privati
                if (!UserManager.ruoloIsAutorized(this, "DO_PROTO_PRIVATO") || UserManager.getInfoUtente().delegato != null)
                    this.chkPrivato.Visible = false;
                else
                    this.chkPrivato.Visible = true;

                //se il documento esiste, riempie i campi con i valori
                if (schedaDocumento != null)
                {
                    if (schedaDocumento.privato != null && schedaDocumento.privato == "1")
                    {
                        this.chkPrivato.Checked = true;
                        //in questo caso in cui privato=1
                        //forzo la visibilità anche se 
                        //il ruolo non è autorizzato, 
                        // altrimenti non è visibile 
                        //l'informazione che il documento è privato
                        this.chkPrivato.Visible = true;
                    }
                    else
                        this.chkPrivato.Checked = false;

                    //oggetto
                    //this.txt_oggetto_P.Text = schedaDocumento.oggetto.descrizione;
                    if (schedaDocumento.oggetto != null && schedaDocumento.oggetto.descrizione != null && schedaDocumento.oggetto.descrizione != "")
                    {
                        this.ctrl_oggetto.oggetto_text = schedaDocumento.oggetto.descrizione;
                        //solo se il campo codice oggetto è attivo!!!
                        if (schedaDocumento.oggetto.codOggetto != null)
                            this.ctrl_oggetto.cod_oggetto_text = schedaDocumento.oggetto.codOggetto;
                    }

                    //Valorizzazione descrizione mittente
                    DocsPaWR.Corrispondente mitt = null;
                    DocsPaWR.Corrispondente ufficioRef = null;
                    string enableUfficioRef = ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF);

                    bool load_scegliUoUtente;
                    string mittDisabled = "";//descrizione dei corrispondenti mittenti che sono stati disabilitati, "" altrimenti

                    switch (schedaDocumento.tipoProto)
                    {

                        case "A":
                            #region PROTOCOLLO IN ARRIVO
                            #region CATENE
                            this.rbl_InOut_P.SelectedIndex = 0;
                            this.val_proto_sele = "proto";
                            this.rispProtoPanelUscita.Visible = true;
                            this.rbl_InOut_P.Items[0].Attributes.Add("class", "radiobutton");


                            GestioneRispostaDocumenti();
                            #endregion

                            #region POPOLAMENTO OGGETTO MITTENTE E VERIFICA MITTENTE ABILITATO
                            mitt = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente;

                            if (schedaDocumento.predisponiProtocollazione == true) //CASO 1: ho cliccato su RIPROPONI
                            {
                                if (mitt != null)
                                {
                                    if (IsPostBack)//nel caso 1 la pagina docProtocollo è già stata caricata
                                    {
                                        mitt = setMittenteAbilitato(mitt); // ritorna il mittente se è abilitato, NULL altrimenti
                                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente = mitt;
                                    }
                                }
                            }
                            #region Codice commentato
                            //							else
                            //							{
                            //								//CASO 2: ho selezionato il dettaglio di un documento PREDISPOSTO alla protocollazione da RICERCA
                            //								if(schedaDocumento.protocollo.daProtocollare!=null && schedaDocumento.protocollo.daProtocollare.Equals("1"))
                            //								{
                            //									
                            //								if(mitt!=null)
                            //									{
                            //								if(!IsPostBack) // nel caso 2 la pagina docProtocollo è la prima volta che viene caricata
                            //										{
                            //								mitt = setMittenteAbilitato(mitt); // ritorna il mittente se è abilitato, NULL altrimenti
                            //								((DocsPAWA.DocsPaWR.ProtocolloEntrata) schedaDocumento.protocollo).mittente = mitt;
                            //										}
                            //									}
                            //								
                            //								}
                            //							
                            //							}		
                            #endregion
                            #endregion

                            #region POPOLAMENTO OGGETTO MITTENTE INTERMEDIO E VERIFICA ABILITATAZIONE

                            if (Session["corr_disabled"] != null) //vuol dire non ho riproposto. se ho riproposto Session["corr_disabled"]=""
                            {
                                mittDisabled = Session["corr_disabled"].ToString();
                            }
                            DocsPaWR.Corrispondente mittInt;
                            mittInt = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenteIntermedio;

                            //leggo la chiave sul web.config
                            string config = ConfigSettings.getKey(ConfigSettings.KeysENUM.VIEW_MITT_INTERMEDI);

                            if (config != null && config.Equals("1"))
                            {
                                if (schedaDocumento.predisponiProtocollazione == true) //CASO 1: ho cliccato su RIPROPONI
                                {
                                    if (mittInt != null)
                                    {
                                        if (IsPostBack)//IsPostBack poichè nel caso 1 la pagina docProtocollo è già stata caricata
                                        {
                                            mittInt = setMittenteIntermedioAbilitato(mittInt, mittDisabled); // ritorna il mittente se è abilitato, NULL altrimenti
                                            ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenteIntermedio = mittInt;
                                        }
                                        else
                                        {
                                            if (Session["corr_disabled"] != null && !Session["corr_disabled"].ToString().Equals(""))
                                            {
                                                RegisterStartupScript("disabled", "<SCRIPT>alert('I seguenti corrispondenti non sono riproposti poichè disabilitati/modificati:" + Session["corr_disabled"].ToString() + "');</SCRIPT>");
                                            }
                                            Session.Remove("corr_disabled");
                                        }

                                    }
                                    else
                                    {
                                        if (!IsPostBack)
                                        {
                                            if (Session["corr_disabled"] != null && !Session["corr_disabled"].ToString().Equals(""))
                                            {
                                                RegisterStartupScript("disabled", "<SCRIPT>alert('I seguenti corrispondenti non sono riproposti poichè disabilitati/modificati:" + Session["corr_disabled"].ToString() + "');</SCRIPT>");
                                            }
                                            Session.Remove("corr_disabled");
                                        }

                                    }
                                    #region Codice commentato
                                    //								else
                                    //								{
                                    //									//CASO 2: ho selezionato il dettaglio di un documento PREDISPOSTO alla protocollazione da RICERCA
                                    //									if(schedaDocumento.protocollo.daProtocollare!=null && schedaDocumento.protocollo.daProtocollare.Equals("1"))
                                    //									{
                                    //										if(mittInt!=null)
                                    //										{
                                    //											if(!IsPostBack) // nel caso 2 la pagina docProtocollo è la prima volta che viene caricata
                                    //											{
                                    //												mittInt = setMittenteIntermedioAbilitato(mittInt,mittDisabled); // ritorna il mittente se è abilitato, NULL altrimenti
                                    //												((DocsPAWA.DocsPaWR.ProtocolloEntrata) schedaDocumento.protocollo).mittenteIntermedio = mittInt;
                                    //												
                                    //												if(Session["corr_disabled"]!=null && !Session["corr_disabled"].ToString().Equals(""))
                                    //												{
                                    //													RegisterStartupScript("disabled","<SCRIPT>alert('I seguenti corrispondenti sono disabilitati:" + Session["corr_disabled"].ToString() + "');</SCRIPT>");
                                    //												}
                                    //												Session.Remove("corr_disabled");
                                    //											}
                                    //											
                                    //										}
                                    //										else
                                    //										{	
                                    //											if(Session["corr_disabled"]!=null && !Session["corr_disabled"].ToString().Equals(""))
                                    //											{
                                    //												RegisterStartupScript("disabled","<SCRIPT>alert('I seguenti corrispondenti sono disabilitati:" + Session["corr_disabled"].ToString() + "');</SCRIPT>");
                                    //											}
                                    //											Session.Remove("corr_disabled");
                                    //										}	
                                    //									}
                                    //								}
                                    #endregion

                                }
                                else
                                {
                                    //se il mittente intermedio è disabilitato
                                    if (schedaDocumento.predisponiProtocollazione == true) //CASO 1: ho cliccato su RIPROPONI
                                    {
                                        if (!IsPostBack)
                                        {
                                            if (Session["corr_disabled"] != null && !Session["corr_disabled"].ToString().Equals(""))
                                            {
                                                RegisterStartupScript("disabled", "<SCRIPT>alert('I seguenti corrispondenti non sono riproposto poichè disabilitati:" + Session["corr_disabled"].ToString() + "');</SCRIPT>");
                                            }
                                            Session.Remove("corr_disabled");
                                        }
                                    }
                                }
                                #region Codice commentato
                                //								else
                                //								{
                                //									if(schedaDocumento.protocollo.daProtocollare!=null && schedaDocumento.protocollo.daProtocollare.Equals("1"))
                                //									{
                                //									if(!IsPostBack)
                                //									{
                                //										if(Session["corr_disabled"]!=null && !Session["corr_disabled"].ToString().Equals(""))
                                //										{
                                //												RegisterStartupScript("disabled","<SCRIPT>alert('I seguenti corrispondenti sono disabilitati:" + Session["corr_disabled"].ToString() + "');</SCRIPT>");
                                //										}
                                //											Session.Remove("corr_disabled");
                                //										
                                //									}
                                //								}
                                #endregion
                            }

                            if (config != null)
                            {
                                // Rendiamo il pannello relativo ai mittenti intermedi invisibile
                                if (config.Equals("0"))
                                {
                                    this.panel_mittInt.Visible = false;
                                }
                                else
                                {
                                    //Rendiamo il pannello relativo ai mittenti intermedi visibile
                                    this.panel_mittInt.Visible = true;
                                    if (mittInt != null)
                                    {
                                        //commentato poiche ora anche gli occasionali hanno codice rubrica
                                        //if (mittInt.tipoCorrispondente == null || !mittInt.tipoCorrispondente.Equals("O"))
                                        //{
                                        if (mittInt.codiceRubrica != null)
                                        {
                                            this.txt_CodMitInt_P.Text = mittInt.codiceRubrica;
                                            //this.GetCorrispondenteControl("CorrDaCodMitInt").CODICE_TEXT = mittInt.codiceRubrica;
                                        }
                                        //}
                                        this.txt_DescMitInt_P.Text = UserManager.getDecrizioneCorrispondenteSemplice(mittInt);
                                        //this.GetCorrispondenteControl("CorrDaCodMitInt").DESCRIZIONE_TEXT = UserManager.getDecrizioneCorrispondenteSemplice(mittInt);
                                        this.txt_DescMitInt_P.ToolTip = this.txt_DescMitInt_P.Text;
                                    }
                                    else
                                    {
                                        this.txt_DescMitInt_P.ToolTip = "";
                                    }
                                }
                            }
                            #endregion

                            #region GESTIONE UFFICIO REFERENTE

                            if (enableUfficioRef != null)
                            {
                                if (enableUfficioRef.Equals("0"))
                                {
                                    this.pnl_ufficioRef.Visible = false;
                                }
                                else
                                {
                                    //prendiamo l'ufficio referente per popolare il campo sul tab protocollo

                                    ufficioRef = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).ufficioReferente;
                                    this.pnl_ufficioRef.Visible = true;
                                    if (ufficioRef != null)
                                    {
                                        this.txt_cod_uffRef.Text = ufficioRef.codiceRubrica;
                                        this.txt_desc_uffRef.Text = UserManager.getDecrizioneCorrispondenteSemplice(ufficioRef);
                                    }
                                    else
                                    {
                                        this.txt_cod_uffRef.Text = "";
                                        this.txt_desc_uffRef.Text = "";
                                    }

                                }
                            }
                            #endregion

                            //nascondo le tabelle con i dati sui destinatari
                            this.panel_Dest.Visible = false;
                            this.panel_Mit.Visible = true;
                            this.pnl_star_YES.Visible = true;
                            this.pnl_star_NO.Visible = false;

                            //cambio la label del pulsante della bottoniera
                            this.btn_aggiungi_P.Visible = true;
                            this.btn_spedisci_P.Visible = false;

                            //per valorizzare la combo box del mezzo di spedizione
                            if (schedaDocumento != null && schedaDocumento.mezzoSpedizione != null && !schedaDocumento.mezzoSpedizione.Equals("0"))
                                for (int i = 0; i < this.ddl_spedizione.Items.Count; i++)
                                {
                                    if (this.ddl_spedizione.Items[i].Value == schedaDocumento.mezzoSpedizione.ToString())
                                        this.ddl_spedizione.Items[i].Selected = true;
                                }
                            //Andrea De Marco - Gestione Eccezioni PEC
                            //In eseguiSenzaSegnatura prima del save della scheda documento, imposto sd.interop = E
                            //Se schedaDoc.interop = E si è verificata un'eccezione nella gestione del file Segnatura.xml, pertanto il tasto di invio Conferma.xml deve essere disabilitato 
                            //Per ripristino, commentare De Marco e decommentare il codice immediatamente sottostante
                            if (!string.IsNullOrEmpty(schedaDocumento.interop) && schedaDocumento.interop.ToUpper().Equals("E"))
                            {
                                this.btn_invio_mail.Enabled = false;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(schedaDocumento.protocollo.segnatura)
                                    && (schedaDocumento.typeId == "MAIL" || schedaDocumento.typeId == "INTEROPERABILITA")
                                    && UserManager.ruoloIsAutorized(this.Page, "DO_INVIO_RICEVUTE")
                                    )
                                {
                                    this.btn_invio_mail.Enabled = true;
                                }
                            }
                            //End Andrea De Marco
                            //if (!string.IsNullOrEmpty(schedaDocumento.protocollo.segnatura)
                            //        && (schedaDocumento.typeId == "MAIL" || schedaDocumento.typeId == "INTEROPERABILITA")
                            //        && UserManager.ruoloIsAutorized(this.Page, "DO_INVIO_RICEVUTE")
                            //        )
                            //{
                            //    this.btn_invio_mail.Enabled = true;
                            //}
                            this.btn_stampa_ricevuta.Visible = true;

                            #region POPOLAMENTO MITTENTI MULTIPLI E VERIFICA ABILITAZIONE
                            if (DocumentManager.isEnableMittentiMultipli())
                            {
                                panel_DettaglioMittenti.Visible = true;
                                setListBoxMittentiMultipli();
                            }
                            #endregion

                            btn_trasmetti.Visible = false;

                            //ABILITA NUOVA RUBRICA VELOCE MITTENTE ARRIVO
                            if (!string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                            {
                                newRubricaVeloce("A");
                            }

                            //Andrea De Marco - Campi editabili - Gestione Eccezioni segnatura.xml PEC
                            //schedaDocumento.interop = E indica che si è verificata una Eccezione non Bloccante in segnatura.xml
                            //Per ripristino, commentare La sezione di codice De Marco
                            //if (!string.IsNullOrEmpty(schedaDocumento.interop) && schedaDocumento.interop.ToUpper().Equals("E") && !string.IsNullOrEmpty(schedaDocumento.descMezzoSpedizione) && schedaDocumento.descMezzoSpedizione.Equals("MAIL")) 
                            //{
                            //        this.txt_NumProtMit_P.ReadOnly = false;
                            //        this.txt_DataProtMit_P.ReadOnly = false;
                            //}
                            //End Andrea De Marco

                            break;
                            #endregion

                        case "P":
                            #region PROTOCOLLO IN USCITA
                            #region CATENE
                            this.pnl_mezzoSpedizione.Visible = false;
                            this.lbx_dest.Items.Clear();
                            this.lbx_destCC.Items.Clear();
                            this.rbl_InOut_P.SelectedIndex = 1;
                            this.rbl_InOut_P.Items[1].Attributes.Add("class", "radiobutton");
                            this.val_proto_sele = "proto";
                            DocsPaWR.Corrispondente[] destUscita;
                            DocsPaWR.Corrispondente[] destCCUscita;
                            //leggo l'elenco delle mail associate ai destinatari del proto in uscita
                            System.Collections.Generic.List<DocsPAWA.DocsPaWR.MailCorrispondente> listMailDestinatari =
                                MultiCasellaManager.GetMailsAllCorrProto(Convert.ToInt32(schedaDocumento.systemId));

                            GestioneRispostaDocumenti();
                            #endregion

                            #region POPOLAMENTO OGGETTO MITTENTE E VERIFICA MITTENTE ABILITATO
                            mitt = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente;

                            if (schedaDocumento.predisponiProtocollazione == true) //CASO 1: ho cliccato su RIPROPONI
                            {
                                if (mitt != null)
                                {
                                    if (IsPostBack)//nel caso 1 la pagina docProtocollo è già stata caricata
                                    {
                                        mitt = setMittenteAbilitato(mitt); // ritorna il mittente se è abilitato, NULL altrimenti  
                                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente = mitt;
                                    }
                                }
                            }
                            #region Codice commentato
                            //							else
                            //							{
                            //								//CASO 2: ho selezionato il dettaglio di un documento PREDISPOSTO alla protocollazione da RICERCA
                            //								if(schedaDocumento.protocollo.daProtocollare!=null && schedaDocumento.protocollo.daProtocollare.Equals("1"))
                            //								{
                            //									
                            //									if(mitt!=null)
                            //									{
                            //										if(!IsPostBack) // nel caso 2 la pagina docProtocollo è la prima volta che viene caricata
                            //										{
                            //											mitt = setMittenteAbilitato(mitt); // ritorna il mittente se è abilitato, NULL altrimenti
                            //											((DocsPAWA.DocsPaWR.ProtocolloUscita) schedaDocumento.protocollo).mittente = mitt;
                            //										}
                            //									}
                            //								
                            //								}
                            //							
                            //							}
                            #endregion
                            #endregion

                            #region POPOLAMENTO UFFICIO REFERENTE
                            //per l'ufficio referente
                            if (enableUfficioRef != null)
                            {
                                if (enableUfficioRef.Equals("0"))
                                {
                                    this.pnl_ufficioRef.Visible = false;
                                }
                                else
                                {
                                    //true: se è stato selezionato un elemento dalla lista delle UO
                                    //false: se ho premuto la X o annulla senza selezionare alcun elemento
                                    load_scegliUoUtente = scegliUOUtente_load();
                                    //prendiamo l'ufficio referente per popolare il campo sul tab protocollo
                                    if (load_scegliUoUtente == true)
                                    {
                                        //DeMarcoA-UffRef
                                        if (Session["tempCorrMultiSelected_P"] != null)
                                        {
                                            //Provengo dalla Popup MultiDestinatari.aspx
                                            Corrispondente tempCorrSelected = (Corrispondente)Session["tempCorrMultiSelected_P"];
                                            if (tempCorrSelected.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
                                            {
                                                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente = ((DocsPAWA.DocsPaWR.Ruolo)tempCorrSelected).uo;
                                            }
                                            if (tempCorrSelected.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
                                            {
                                                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente = tempCorrSelected;
                                            }
                                            Session.Remove("tempCorrMultiSelected_P");
                                        }
                                        //DeMarcoA-UffRef

                                        ufficioRef = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente;
                                        this.pnl_ufficioRef.Visible = true;
                                        if (ufficioRef != null)
                                        {
                                            this.txt_cod_uffRef.Text = ufficioRef.codiceRubrica;
                                            this.txt_desc_uffRef.Text = UserManager.getDecrizioneCorrispondenteSemplice(ufficioRef);
                                        }
                                        else
                                        {
                                            ufficioRef = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente = UserManager.getRuolo(this).uo;
                                            this.txt_cod_uffRef.Text = ufficioRef.codiceRubrica;
                                            this.txt_desc_uffRef.Text = UserManager.getDecrizioneCorrispondenteSemplice(ufficioRef);
                                        }
                                    }
                                    else
                                    {
                                        ufficioRef = null;
                                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente = ufficioRef;
                                        //if(Session["isLoaded_ScegliUoUtente"] != null && (bool)Session["isLoaded_ScegliUoUtente"] == false)
                                        mitt = null;

                                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente = mitt;
                                        if (ufficioRef != null)
                                        {
                                            this.txt_cod_uffRef.Text = ufficioRef.codiceRubrica;
                                            this.txt_desc_uffRef.Text = UserManager.getDecrizioneCorrispondenteSemplice(ufficioRef);
                                        }
                                        else
                                        {
                                            ufficioRef = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente = UserManager.getRuolo(this).uo;
                                            this.txt_cod_uffRef.Text = ufficioRef.codiceRubrica;
                                            this.txt_desc_uffRef.Text = UserManager.getDecrizioneCorrispondenteSemplice(ufficioRef);
                                        }
                                    }
                                }

                                scegliUOUtente_clear();

                            }
                            #endregion

                            #region POPOLAMENTO OGGETTO DESTINATARIO TO/CC E VERIFICA DESTINATARI ABILITATI

                            destUscita = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari;

                                                        if (destUscita != null && destUscita.Length > 0)
                            {                     
                                if (listMailDestinatari != null && listMailDestinatari.Count > 0)
                                {
                                    //itero sui destinatari per cercare le mail associate
                                    foreach (Corrispondente c in destUscita)
                                    {
                                        System.Collections.Generic.List<MailCorrispondente> listMail = new System.Collections.Generic.List<MailCorrispondente>();
                                        var mailCorr = from mc in listMailDestinatari where mc.systemId.Equals(c.systemId) select mc;
                                        foreach (var mc in mailCorr)
                                        {
                                            listMail.Add(new MailCorrispondente()
                                            {
                                                systemId = mc.systemId,
                                                Email = mc.Email,
                                                Note = mc.Note,
                                                Principale = mc.Principale
                                            });
                                        }
                                        if (listMail != null && listMail.Count > 0)
                                            c.Emails = listMail.ToArray();
                                    }
                                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari = destUscita;
                                    DocumentManager.setDocumentoInLavorazione(schedaDocumento);
                                }
                            }
                            
                            destCCUscita = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza;
                            if (destCCUscita != null && destCCUscita.Length > 0)
                            {
                                //leggo l'elenco delle mail associate ai destinatari in CC del proto in uscita
                                if (listMailDestinatari != null && listMailDestinatari.Count > 0)
                                {
                                    //itero sui destinatari per cercare le mail associate
                                    foreach (Corrispondente c in destCCUscita)
                                    {
                                        System.Collections.Generic.List<MailCorrispondente> listMail = new System.Collections.Generic.List<MailCorrispondente>();
                                        var mailCorr = from mc in listMailDestinatari where mc.systemId.Equals(c.systemId) select mc;
                                        foreach (var mc in mailCorr)
                                        {
                                            listMail.Add(new MailCorrispondente()
                                            {
                                                systemId = mc.systemId,
                                                Email = mc.Email,
                                                Note = mc.Note,
                                                Principale = mc.Principale
                                            });
                                        }
                                        if (listMail != null && listMail.Count > 0)
                                            c.Emails = listMail.ToArray();
                                    }
                                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza = destCCUscita;
                                    DocumentManager.setDocumentoInLavorazione(schedaDocumento);
                                }
                            }
                            /* Session["corr_disabled"]:
                             *  variabile di sessione che contiene la stringa relativa alle descrizioni 
                             * 	dei corrispondenti (mitt, destTo, destCC) che sono stati disabilitati */
                            if (Session["corr_disabled"] != null) //vuol dire non ho riproposto. se ho riproposto Session["corr_disabled"]=""
                            {
                                mittDisabled = Session["corr_disabled"].ToString();
                            }

                            //devo costruire la tabella con l'elenco dei destinatari
                            if (schedaDocumento.predisponiProtocollazione == true)
                            {
                                if (IsPostBack)
                                    setListBoxDestinatariAbilitati(mittDisabled);
                                else
                                {
                                    setListBoxDestinatari();
                                    if (Session["corr_disabled"] != null && !Session["corr_disabled"].ToString().Equals(""))
                                    {
                                        RegisterStartupScript("disabled", "<SCRIPT>alert('I seguenti corrispondenti non sono riproposti poichè disabilitati/modificati:" + Session["corr_disabled"].ToString() + "');</SCRIPT>");
                                    }
                                    Session.Remove("corr_disabled");
                                }
                            }
                            else
                            {
                                #region Codice commentato
                                //								if(schedaDocumento.protocollo.daProtocollare!=null && schedaDocumento.protocollo.daProtocollare.Equals("1"))
                                //								{
                                //									if(!IsPostBack)
                                //									{
                                //										setListBoxDestinatariAbilitati(mittDisabled);	
                                //									
                                //										if(Session["corr_disabled"]!=null && !Session["corr_disabled"].ToString().Equals(""))
                                //										{
                                //											RegisterStartupScript("disabled","<SCRIPT>alert('I seguenti corrispondenti sono disabilitati:" + Session["corr_disabled"].ToString() + "');</SCRIPT>");                               
                                //										}
                                //										Session.Remove("corr_disabled");
                                //									}	
                                //									else
                                //									{
                                //										setListBoxDestinatari();
                                //									}
                                //								}
                                //								else
                                //								{
                                #endregion
                                setListBoxDestinatari();
                                //								}
                            }
                            #endregion

                            //nascondo le tabelle con i dati sui mittenti
                            this.panel_Dest.Visible = true;
                            this.panel_Mit.Visible = false;
                            this.pnl_star_NO.Visible = true;
                            this.pnl_star_YES.Visible = false;
                            this.panel_DettaglioMittenti.Visible = false;

                            this.btn_aggiungi_P.Visible = true;

                            //cambio la label del pulsante spedisci
                            
                            // PALUMBO: Intervento per evitare rallentamenti al refresh della pagina sostituito metodo GetSpedizioneDocumento con verificaSpedizioneDocumento. 
                            //DocsPaWR.SpedizioneDocumento infoSpedizione = SpedizioneManager.GetSpedizioneDocumento(DocumentManager.getDocumentoSelezionato());
                            int result = 0; // 0 (il documento non è mai stato spedito)
                            result = DocumentManager.verificaSpedizioneDocumento(this, schedaDocumento.systemId); 

                            this.btn_spedisci_P.Visible = true;
                            string Tema = GetCssAmministrazione();

                            //if (infoSpedizione == null || (infoSpedizione != null && !infoSpedizione.Spedito))
                            if (result == 0)
                            {
                                if (Tema != null && !Tema.Equals(""))
                                {
                                    string[] realTema = Tema.Split('^');
                                    this.btn_spedisci_P.ImageUrl = "../App_Themes/" + realTema[0] + "/btn_spedisci_attivo.gif";
                                    this.btn_spedisci_P.DisabledUrl = "../App_Themes/" + realTema[0] + "/btn_spedisci_nonattivo.gif";
                                }
                                else
                                {
                                    this.btn_spedisci_P.ImageUrl = "../App_Themes/TemaRosso" + "/btn_spedisci_attivo.gif";
                                    this.btn_spedisci_P.DisabledUrl = "../App_Themes/TemaRosso" + "/btn_spedisci_nonattivo.gif";
                                }
                                this.btn_spedisci_P.AlternateText = "Spedisci";
                            }
                            else
                            {
                                if (Tema != null && !Tema.Equals(""))
                                {
                                    string[] realTema = Tema.Split('^');
                                    this.btn_spedisci_P.ImageUrl = "../App_Themes/" + realTema[0] + "/btn_rispedisci.gif";
                                    this.btn_spedisci_P.DisabledUrl = "../App_Themes/" + realTema[0] + "/btn_rispedisci_no_attivo.gif";

                                }
                                else
                                {
                                    this.btn_spedisci_P.ImageUrl = "../App_Themes/TemaRosso" + "/btn_rispedisci.gif";
                                    this.btn_spedisci_P.DisabledUrl = "../App_Themes/TemaRosso" + "/btn_rispedisci_no_attivo.gif";
                                }
                                this.btn_spedisci_P.AlternateText = "Rispedisci";
                            }
                            //Fine cambio label pulsante spedisci

                            this.btn_stampa_ricevuta.Visible = false;
                            btn_trasmetti.Visible = false;

                            //ABILITA NUOVA RUBRICA VELOCE MITTENTE ARRIVO
                            if (!string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                            {
                                newRubricaVeloce("P");
                            }

                            break;

                            #endregion

                        case "I":
                            #region PROTOCOLLO INTERNO
                            this.pnl_mezzoSpedizione.Visible = false;

                            this.lbx_dest.Items.Clear();
                            this.lbx_destCC.Items.Clear();
                            this.rbl_InOut_P.SelectedIndex = 2;
                            this.rbl_InOut_P.Items[2].Attributes.Add("class", "radiobutton");

                            this.val_proto_sele = "protoInt";
                            DocsPaWR.Corrispondente[] destInterno;
                            if (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null
                                && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1")
                            {
                                this.rispProtoPanelUscita.Visible = true;
                                GestioneRispostaDocumenti();
                            }
                            else
                                this.rispProtoPanelUscita.Visible = false;
                            //se il protocollo è annullato disabilito il pulsante relativo alle catene documentali
                            if (schedaDocumento.protocollo != null)
                                if (schedaDocumento.protocollo.protocolloAnnullato != null)
                                    this.btn_annulla_P.Enabled = false;


                            #region POPOLAMENTO OGGETTO MITTENTE E VERIFICA MITTENTE ABILITATO

                            mitt = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente;

                            if (schedaDocumento.predisponiProtocollazione == true) //CASO 1: ho cliccato su RIPROPONI
                            {
                                if (mitt != null)
                                {
                                    if (IsPostBack) // nel caso 1 la pagina docProtocollo è già stata caricata
                                    {
                                        mitt = setMittenteAbilitato(mitt); // ritorna il mittente se è abilitato, NULL altrimenti
                                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente = mitt;
                                    }
                                }
                            }
                            #region Codice commentato
                            //							else
                            //							{
                            //								//CASO 2: ho selezionato il dettaglio di un documento PREDISPOSTO alla protocollazione da RICERCA
                            //								if(schedaDocumento.protocollo.daProtocollare!=null && schedaDocumento.protocollo.daProtocollare.Equals("1"))
                            //								{
                            //									if(mitt!=null)
                            //									{
                            //										if(!IsPostBack) // nel caso 2 la pagina docProtocollo è la prima volta che viene caricata
                            //										{
                            //											mitt = setMittenteAbilitato(mitt); // ritorna il mittente se è abilitato, NULL altrimenti
                            //											((DocsPAWA.DocsPaWR.ProtocolloUscita) schedaDocumento.protocollo).mittente = mitt;
                            //										}
                            //									}
                            //								
                            //								}
                            //							}
                            #endregion
                            #endregion

                            #region POPOLAMENTO UFFICIO REFERENTE

                            //per l'ufficio referente
                            if (enableUfficioRef != null)
                            {
                                if (enableUfficioRef.Equals("0"))
                                {
                                    this.pnl_ufficioRef.Visible = false;
                                }
                                else
                                {
                                    //true: se è stato selezionato un elemento dalla lista delle UO
                                    //false: se ho premuto la X o annulla senza selezionare alcun elemento
                                    load_scegliUoUtente = scegliUOUtente_load();
                                    //prendiamo l'ufficio referente per popolare il campo sul tab protocollo
                                    if (load_scegliUoUtente == true)
                                    {
                                        //prendiamo l'ufficio referente per popolare il campo sul tab protocollo

                                        //DeMarcoA-UffRef
                                        if (Session["tempCorrMultiSelected_I"] != null)
                                        {
                                            //Provengo dalla Popup MultiDestinatari.aspx
                                            Corrispondente tempCorrSelected = (Corrispondente)Session["tempCorrMultiSelected_I"];
                                            if (tempCorrSelected.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
                                            {
                                                ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).ufficioReferente = ((DocsPAWA.DocsPaWR.Ruolo)tempCorrSelected).uo;
                                            }
                                            if (tempCorrSelected.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
                                            {
                                                ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).ufficioReferente = tempCorrSelected;
                                            }
                                            Session.Remove("tempCorrMultiSelected_I");
                                        }
                                        //DeMarcoA-UffRef

                                        ufficioRef = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).ufficioReferente;
                                        this.pnl_ufficioRef.Visible = true;
                                        if (ufficioRef != null)
                                        {
                                            this.txt_cod_uffRef.Text = ufficioRef.codiceRubrica;
                                            this.txt_desc_uffRef.Text = UserManager.getDecrizioneCorrispondenteSemplice(ufficioRef);
                                        }
                                        else
                                        {
                                            this.txt_cod_uffRef.Text = "";
                                            this.txt_desc_uffRef.Text = "";
                                        }
                                    }
                                    else
                                    {
                                        ufficioRef = null;
                                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).ufficioReferente = ufficioRef;
                                        mitt = null;
                                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente = mitt;
                                        if (ufficioRef != null)
                                        {
                                            this.txt_cod_uffRef.Text = ufficioRef.codiceRubrica;
                                            this.txt_desc_uffRef.Text = UserManager.getDecrizioneCorrispondenteSemplice(ufficioRef);
                                        }
                                        else
                                        {
                                            this.txt_cod_uffRef.Text = "";
                                            this.txt_desc_uffRef.Text = "";
                                        }
                                    }

                                }
                                scegliUOUtente_clear();
                            }
                            #endregion

                            #region POPOLAMENTO OGGETTO DESTINATARIO TO/CC E VERIFICA DESTINATARI ABILITATI

                            destInterno = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatari;

                            //devo costruire la tabella con l'elenco dei destinatari			
                            //							setListBoxDestinatari();
                            //new per bug riproponi con destinatari disabilitati
                            //devo costruire la tabella con l'elenco dei destinatari	

                            if (Session["corr_disabled"] != null) //vuol dire non ho riproposto. se ho riproposto Session["corr_disabled"]=""
                            {
                                mittDisabled = Session["corr_disabled"].ToString();
                            }

                            if (schedaDocumento.predisponiProtocollazione == true)
                            {
                                if (IsPostBack)
                                    setListBoxDestinatariAbilitati(mittDisabled);
                                else
                                {
                                    setListBoxDestinatari();
                                    if (Session["corr_disabled"] != null && !Session["corr_disabled"].ToString().Equals(""))
                                    {
                                        RegisterStartupScript("disabled", "<SCRIPT>alert('I seguenti corrispondenti non sono riproposti poichè disabilitati/modificati:" + Session["corr_disabled"].ToString() + "');</SCRIPT>");
                                    }
                                    Session.Remove("corr_disabled");
                                }
                            }
                            else
                            {
                                #region Codice commentato
                                //								if(schedaDocumento.protocollo.daProtocollare!=null && schedaDocumento.protocollo.daProtocollare.Equals("1"))
                                //								{
                                //									if(!IsPostBack)
                                //									{
                                //										setListBoxDestinatariAbilitati(mittDisabled);	
                                //									
                                //										if(Session["corr_disabled"]!=null && !Session["corr_disabled"].ToString().Equals(""))
                                //										{
                                //											RegisterStartupScript("disabled","<SCRIPT>alert('I seguenti corrispondenti sono disabilitati:" + Session["corr_disabled"].ToString() + "');</SCRIPT>");
                                //										}
                                //										Session.Remove("corr_disabled");
                                //									}	
                                //									else
                                //									{
                                //										setListBoxDestinatari();
                                //									}
                                //								}
                                //								else
                                //								{
                                #endregion
                                setListBoxDestinatari();
                                //								}
                            }
                            #endregion

                            //nascondo le tabelle con i dati sui mittenti
                            this.panel_Dest.Visible = true;
                            this.panel_Mit.Visible = false;
                            this.pnl_star_YES.Visible = true;
                            this.pnl_star_NO.Visible = false;
                            this.panel_DettaglioMittenti.Visible = false;

                            //cambio la label del pulsante della bottoniera
                            this.btn_aggiungi_P.Visible = true;
                            //this.btn_spedisci_P.Visible = false;
                            this.btn_trasmetti.Visible = true;

                            //cambio la label del pulsante trasmetti
                            // PALUMBO: intervento per velocizzare il refresh della pagina
                            int documentTab = 0;
                            if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.systemId))
                                documentTab = DocumentManager.GetDocumentTrasmToCCDest(schedaDocumento.systemId, UserManager.getInfoUtente(this.Page));

                            //DocsPaWR.SpedizioneDocumento infoSpedizioneTrasm = SpedizioneManager.GetSpedizioneDocumento(DocumentManager.getDocumentoSelezionato());
                            string TemaTrasm = GetCssAmministrazione();

                            //if (infoSpedizioneTrasm == null || (infoSpedizioneTrasm != null && !infoSpedizioneTrasm.Spedito))
                            //if (documentTab == null)
                            //{
                            //    if (TemaTrasm != null && !TemaTrasm.Equals(""))
                            //    {
                            //        string[] realTema = TemaTrasm.Split('^');
                            //        this.btn_trasmetti.ImageUrl = "../App_Themes/" + realTema[0] + "/btn_trasmetti_attivo.gif";
                            //        this.btn_trasmetti.DisabledUrl = "../App_Themes/ImgComuni/btn_trasmetti_NoAttivo.gif";
                            //    }
                            //    else
                            //    {
                            //        this.btn_trasmetti.ImageUrl = "../App_Themes/TemaRosso" + "/btn_trasmetti_attivo.gif";
                            //        this.btn_trasmetti.DisabledUrl = "../App_Themes/ImgComuni/btn_trasmetti_NoAttivo.gif";
                            //    }
                            //    this.btn_trasmetti.AlternateText = "Trasmetti";
                            //}
                            //else
                            //{
                                //if(documentTab.TransmissionsNumber.Equals("0"))
                                if(documentTab == 0)
                                {
                                    if (TemaTrasm != null && !TemaTrasm.Equals(""))
                                    {
                                        string[] realTema = TemaTrasm.Split('^');
                                        this.btn_trasmetti.ImageUrl = "../App_Themes/" + realTema[0] + "/btn_trasmetti_attivo.gif";
                                        this.btn_trasmetti.DisabledUrl = "../App_Themes/ImgComuni/btn_trasmetti_NoAttivo.gif";
                                    }
                                    else
                                    {
                                        this.btn_trasmetti.ImageUrl = "../App_Themes/TemaRosso" + "/btn_trasmetti_attivo.gif";
                                        this.btn_trasmetti.DisabledUrl = "../App_Themes/ImgComuni/btn_trasmetti_NoAttivo.gif";
                                    }
                                    this.btn_trasmetti.AlternateText = "Trasmetti";
                                }
                                else
                                {
                                    if (TemaTrasm != null && !TemaTrasm.Equals(""))
                                    {
                                        string[] realTema = TemaTrasm.Split('^');
                                        this.btn_trasmetti.ImageUrl = "../App_Themes/" + realTema[0] + "/btn_ritrasmetti.gif";
                                        this.btn_trasmetti.DisabledUrl = "../App_Themes/ImgComuni/btn_ritrasmetti_no_attivo.gif";

                                    }
                                    else
                                    {
                                        this.btn_trasmetti.ImageUrl = "../App_Themes/TemaRosso" + "/btn_ritrasmetti.gif";
                                        this.btn_trasmetti.DisabledUrl = "../App_Themes/ImgComuni/btn_ritrasmetti_no_attivo.gif";
                                    }
                                    this.btn_trasmetti.AlternateText = "Ritrasmetti";
                                }
                            //}
                            //Fine cambio label pulsante trasmetti

                            this.btn_spedisci_P.Visible = false;

                            btn_trasmetti.Enabled = false;
                            if (!String.IsNullOrEmpty(schedaDocumento.protocollo.segnatura))
                                btn_trasmetti.Enabled = true;

                            this.btn_stampa_ricevuta.Visible = false;

                            //ABILITA NUOVA RUBRICA VELOCE MITTENTE ARRIVO
                            if (!string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                            {
                                newRubricaVeloce("I");
                            }

                            break;
                            #endregion
                    }

                    #region POPOLAMENTO CAMPO DESCRIZIONE MITTENTE

                    if (schedaDocumento != null)
                    {
                        if (schedaDocumento.typeId != null &&
                            (schedaDocumento.typeId.ToUpper().Equals("INTEROPERABILITA") ||
                             schedaDocumento.typeId.ToUpper().Equals("MAIL")) &&
                            !string.IsNullOrEmpty(schedaDocumento.documento_da_pec))
                        {
                            img_busta_pec.Visible = true;
                            if (schedaDocumento.documento_da_pec.Equals("1"))
                            {
                                img_busta_pec.AlternateText = "Documento spedito tramite la posta elettronica certificata";
                                img_busta_pec.Enabled = true;
                            }
                            else
                            {
                                img_busta_pec.Enabled = false;
                                img_busta_pec.AlternateText = "Documento spedito tramite la posta elettronica";
                            }
                        }
                    }
                    //valorizzazione campo descrizione mittente
                    if (mitt != null)
                    {
                        //commentato poichè ora anche i corrisp occasionali hanno codice rubrica
                        //if (mitt.tipoCorrispondente == null || !mitt.tipoCorrispondente.Equals("O"))
                        //{
                        if (mitt.codiceRubrica != null)
                        {
                            this.txt_CodMit_P.Text = mitt.codiceRubrica;
                            //this.GetCorrispondenteControl("CorrDaCodMit").CODICE_TEXT = mitt.codiceRubrica;                           

                        }

                        //this.GetCorrispondenteControl("CorrDaCodMit").DESCRIZIONE_TEXT = UserManager.getDecrizioneCorrispondenteSemplice(mitt);                        
                        if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE)) &&
                            bool.Parse(ConfigSettings.getKey(ConfigSettings.KeysENUM.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE)) &&
                             schedaDocumento != null && schedaDocumento.typeId != null &&
                             schedaDocumento.typeId.ToUpper().Equals("INTEROPERABILITA") &&
                             !string.IsNullOrEmpty(mitt.codDescAmministrizazione))
                            this.txt_DescMit_P.Text = mitt.codDescAmministrizazione + UserManager.getDecrizioneCorrispondenteSemplice(mitt);
                        else
                            this.txt_DescMit_P.Text = UserManager.getDecrizioneCorrispondenteSemplice(mitt);


                        this.hiddenIdCodMit_p.Value = mitt.systemId;
                        this.txt_DescMit_P.ToolTip = this.txt_DescMit_P.Text;

                    }
                    else
                    {
                        //this.GetCorrispondenteControl("CorrDaCodMit").clean();
                        this.txt_CodMit_P.Text = "";
                        //luluciani 20061219
                        //OCCASIONALE appena selezionato con scheda non protocollata, se
                        //faccio un postoback si perde, quindi faccio questo controllo.
                        if (!(this.txt_DescMit_P.Text != null
                            && this.txt_DescMit_P.Text != ""))
                            this.txt_DescMit_P.Text = "";

                        this.hiddenIdCodMit_p.Value = "";
                        this.txt_DescMit_P.ToolTip = "";
                    }

                    #endregion

                    #region POPOLAMENTO CAMPO SEGNATURA E DATA PROTOCOLLAZIONE
                    if (schedaDocumento.protocollo != null)
                    {
                        //segnatura e data
                        if (schedaDocumento.protocollo.segnatura != null &&
                            schedaDocumento.protocollo.segnatura != "")
                        {
                            this.lbl_segnatura.Text = schedaDocumento.protocollo.segnatura;

                            string idAmm = string.Empty;
                            if ((string)Session["AMMDATASET"] != null)
                                idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                            else
                                if (UserManager.getInfoUtente() != null && !string.IsNullOrEmpty(UserManager.getInfoUtente().idAmministrazione))
                                    idAmm = UserManager.getInfoUtente().idAmministrazione;

                            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                            string segnAmm = "0";
                            if (!string.IsNullOrEmpty(idAmm))
                                segnAmm = ws.getSegnAmm(idAmm);
                            switch (segnAmm)
                            {
                                case "0":
                                    this.lbl_segnatura.ForeColor = Color.Black;
                                    break;

                                case "1":
                                    this.lbl_segnatura.ForeColor = Color.Blue;
                                    break;

                                case "2":
                                    this.lbl_segnatura.ForeColor = Color.Red;
                                    break;
                            }
                        }
                        if (schedaDocumento.protocollo.dataProtocollazione != null && !schedaDocumento.protocollo.dataProtocollazione.Equals(""))
                        {
                            string tipoReg = UserManager.getStatoRegistro(UserManager.getRegistroSelezionato(this));
                            //	this.txt_dataSegn.Text = Utils.dateLength(schedaDocumento.protocollo.dataProtocollazione);
                            if (schedaDocumento.oraCreazione != null && schedaDocumento.oraCreazione != "")
                                this.txt_dataSegn.Text = Utils.dateLength(schedaDocumento.protocollo.dataProtocollazione) + " " + Utils.timeLength(schedaDocumento.oraCreazione);
                            else
                                this.txt_dataSegn.Text = Utils.dateLength(schedaDocumento.protocollo.dataProtocollazione);

                        }
                    }
                    #endregion

                    #region VISUALIZZAZIONE PULSANTE PROTOCOLLA/PROTOCOLLA IN GIALLO (REGISTRO VERDE/GIALLO)
                    if (schedaDocumento.protocollo != null)
                    {
                        if (!(schedaDocumento.protocollo.segnatura != null && !schedaDocumento.protocollo.segnatura.Equals("")))
                        {
                            //visualizzo uno dei due pulsanti per la protocollazione (normale o in giallo)
                            string tipoReg = UserManager.getStatoRegistro(UserManager.getRegistroSelezionato(this));

                            if (tipoReg.Equals("G"))
                            {
                                this.btn_protocollaGiallo_P.Visible = true;
                                this.btn_protocolla_P.Visible = false;
                            }
                            else if (tipoReg.Equals("V"))
                            {
                                this.btn_protocollaGiallo_P.Visible = false;
                                this.btn_protocolla_P.Visible = true;
                            }
                            else
                            {
                                this.btn_protocollaGiallo_P.Visible = false;
                                this.btn_protocolla_P.Visible = true;
                                this.btn_protocolla_P.Enabled = false;
                                this.btn_protocollaGiallo_P.Enabled = false;

                            }
                        }
                    }
                    #endregion

                    #region POPOLAMENTO CAMPO PROTOCOLLO MITTENTE
                    //protocollo mittente 
                    if (schedaDocumento.protocollo != null)
                    {
                        if (schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloEntrata))
                        {
                            this.txt_NumProtMit_P.Text = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).descrizioneProtocolloMittente;
                            this.GetCalendarControl("txt_DataProtMit_P").txt_Data.Text = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).dataProtocolloMittente;
                        }

                        //data arrivo
                        if (schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloEntrata))
                        {
                            if (schedaDocumento.documenti != null)
                            {
                                if (schedaDocumento.documenti.Length > 0)
                                {
                                    string dataArrivoTemp = string.Empty;

                                    foreach (Documento tempDoc in schedaDocumento.documenti)
                                    {
                                        if (!string.IsNullOrEmpty(tempDoc.dataArrivo))
                                        {
                                            dataArrivoTemp = tempDoc.dataArrivo;
                                            break;
                                        }
                                    }


                                    if (!string.IsNullOrEmpty(dataArrivoTemp))
                                    {
                                        //this.txt_DataArrivo_P.Text = Utils.dateLength(((DocsPAWA.DocsPaWR.Documento)schedaDocumento.documenti[0]).dataArrivo);
                                        this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text = Utils.dateLength(dataArrivoTemp);
                                        string oraPerv = Utils.getTime(dataArrivoTemp);
                                        if (oraPerv == "0.00.00")
                                            this.txt_OraPervenuto_P.Text = "";
                                        else
                                            this.txt_OraPervenuto_P.Text = oraPerv;
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region commentato per rilascio
                    //#region POPOLAMENTO CAMPO RISPOSTA AL PROTOCOLLO

                    //if (schedaDocumento.protocollo != null && schedaDocumento.rispostaDocumento != null && schedaDocumento.rispostaDocumento.segnatura!=null)
                    //{
                    //   if(schedaDocumento.tipoProto.Equals("A"))
                    //    this.txt_RispProtSegn_P.Text = schedaDocumento.rispostaDocumento.segnatura;
                    //   else
                    //    this.txt_RispIngressoProtSegn_P.Text = schedaDocumento.rispostaDocumento.segnatura;
                    //}
                    //else
                    //{
                    //    this.txt_RispProtSegn_P.Text = "";
                    //    this.txt_RispIngressoProtSegn_P.Text = "";
                    //}
                    //#endregion

                    #region POPOLAMENTO CAMPO RISPOSTA AL PROTOCOLLO
                    //ELISA
                    //if (schedaDocumento.protocollo != null)
                    //{
                    //   //risposta al protocollo 
                    //   if (schedaDocumento.rispostaDocumento != null)
                    //   {
                    //      this.txt_RispProtSegn_P.Text = schedaDocumento.rispostaDocumento.segnatura;
                    //   }
                    //   else
                    //   {
                    //      this.txt_RispProtSegn_P.Text = "";
                    //   }
                    //}
                    #endregion

                    #endregion

                    #region GESTIONE VISIBILITA' BOTTONI, CAMPI E PANNELLI PER UFFICIO REFERENTE
                    // UFFICIO REFERENTE
                    if (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                        && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"))
                    {
                        this.pnl_ufficioRef.Visible = true;

                    }

                    if (ufficioRef != null) 	//per l'ufficio referente
                    {
                        if (schedaDocumento.protocollo != null)
                        {
                            if (schedaDocumento.protocollo.segnatura != null && schedaDocumento.protocollo.segnatura != "")
                            {
                                if (!schedaDocumento.tipoProto.Equals("A"))
                                {
                                    this.txt_cod_uffRef.ReadOnly = true;
                                    this.txt_cod_uffRef.BackColor = Color.WhiteSmoke;
                                    this.txt_desc_uffRef.BackColor = Color.WhiteSmoke;
                                }
                            }
                            else
                            {
                                if (!schedaDocumento.tipoProto.Equals("A"))
                                {
                                    this.txt_cod_uffRef.ReadOnly = true;
                                    this.txt_cod_uffRef.BackColor = Color.WhiteSmoke;
                                    this.txt_desc_uffRef.BackColor = Color.WhiteSmoke;
                                    this.btn_Rubrica_ref.Enabled = false;
                                }
                                else
                                {

                                    this.txt_cod_uffRef.ReadOnly = false;
                                    this.txt_cod_uffRef.BackColor = Color.White;
                                    this.txt_desc_uffRef.BackColor = Color.White;
                                    this.btn_Rubrica_ref.Enabled = true;


                                    //this.btn_Rubrica_ref.Attributes.Add("onclick","ApriRubrica('proto','U');");
                                    if (use_new_rubrica != "1")
                                        btn_Rubrica_ref.Attributes.Add("onclick", "ApriRubrica('proto','U');");
                                    else
                                        btn_Rubrica_ref.Attributes.Add("onclick", "_ApriRubrica('uffref_proto');");

                                    if (schedaDocumento.systemId != null && (!schedaDocumento.predisponiProtocollazione) && schedaDocumento.protocollo.ModUffRef == false && schedaDocumento.protocollo.daProtocollare != null && schedaDocumento.protocollo.daProtocollare == "1")
                                    {

                                        this.txt_cod_uffRef.ReadOnly = true;
                                        this.txt_cod_uffRef.BackColor = Color.WhiteSmoke;
                                        this.txt_desc_uffRef.BackColor = Color.WhiteSmoke;
                                        this.btn_Rubrica_ref.Enabled = false;
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        if (schedaDocumento.protocollo != null)
                        {
                            if (!schedaDocumento.tipoProto.Equals("A"))
                            {
                                this.txt_cod_uffRef.ReadOnly = true;
                                this.txt_cod_uffRef.BackColor = Color.WhiteSmoke;
                                this.txt_desc_uffRef.BackColor = Color.WhiteSmoke;
                            }
                            else
                            {
                                this.txt_cod_uffRef.ReadOnly = false;
                                this.txt_cod_uffRef.BackColor = Color.White;
                                this.txt_desc_uffRef.BackColor = Color.White;
                                this.btn_Rubrica_ref.Enabled = true;
                                if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                                {
                                    schedaDocumento.protocollo.ModUffRef = true;
                                    if (schedaDocumento.protocollo != null && schedaDocumento.protocollo.segnatura != null && !schedaDocumento.protocollo.segnatura.Equals(""))
                                        schedaDocumento.protocollo.ModUffRef = false;
                                }
                                set_btn_Rubrica_ref_event();
                                if (schedaDocumento.systemId != null && (!schedaDocumento.predisponiProtocollazione))
                                {
                                    if (!this.txt_cod_uffRef.Text.Equals(""))
                                    {
                                        this.txt_cod_uffRef.ReadOnly = true;
                                        this.txt_cod_uffRef.BackColor = Color.WhiteSmoke;
                                        this.txt_desc_uffRef.BackColor = Color.WhiteSmoke;
                                        this.btn_Rubrica_ref.Enabled = false;
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region GESTIONE FLAG EVIDENZA E TIPOLOGIA ATTO
                    //evidenza
                    if (schedaDocumento.evidenza != null && schedaDocumento.evidenza.Equals("1"))
                    {
                        this.chkEvidenza.Checked = true;
                    }
                    else
                    {
                        this.chkEvidenza.Checked = false;
                    }

                    //tipo Atto
                    if (schedaDocumento.tipologiaAtto != null)
                    {
                        setTipoAtto(schedaDocumento.tipologiaAtto.systemId);
                    }

                    #endregion

                    #region GESTIONE PROTOCOLLO ANNULLATO
                    //annullamento
                    DocsPaWR.ProtocolloAnnullato protAnnull;
                    if (schedaDocumento.protocollo != null)
                    {
                        protAnnull = schedaDocumento.protocollo.protocolloAnnullato;

                        if (protAnnull != null)
                        {
                            this.panel_Annul.Visible = true;
                            this.txt_dataAnnul_P.Text = schedaDocumento.protocollo.protocolloAnnullato.dataAnnullamento;
                            this.txt_numAnnul_P.Text = schedaDocumento.protocollo.protocolloAnnullato.autorizzazione;
                            this.txt_numAnnul_P.ToolTip = schedaDocumento.protocollo.protocolloAnnullato.autorizzazione;
                            this.btn_modificaOgget_P.Enabled = false;
                            this.btn_aggiungi_P.Enabled = false;
                            this.ddl_tipoAtto.Enabled = false;
                            this.imgFasc.Enabled = false;
                            this.txt_DescFascicolo.Enabled = false;
                            this.txt_CodFascicolo.Enabled = false;
                            this.ddl_tmpl.Enabled = false;
                            btn_verificaPrec_P.Enabled = false;
                            this.btn_titolario.Enabled = false;
                        }
                    }
                    #endregion

                    #region  GESTIONE PROTOCOLLO DI EMERGENZA
                    if (schedaDocumento.protocollo != null)
                    {
                        //protocollo emergenza
                        if (schedaDocumento.datiEmergenza != null && schedaDocumento.datiEmergenza.dataProtocollazioneEmergenza != null && !schedaDocumento.datiEmergenza.dataProtocollazioneEmergenza.Equals(""))
                        {
                            this.txt_dta_protoEme.Text = schedaDocumento.datiEmergenza.dataProtocollazioneEmergenza;
                            this.txt_ProtoEme.Text = schedaDocumento.datiEmergenza.protocolloEmergenza;
                            this.pnl_protoEme.Visible = true;
                        }
                        else
                        {
                            this.pnl_protoEme.Visible = false;
                        }
                    }
                    #endregion

                    #region				GESTIONE REGISTRO CHIUSO

                    if (schedaDocumento != null && schedaDocumento.registro != null && schedaDocumento.registro.stato == "C")
                    {
                        this.btn_riproponiDati_P.Enabled = false;
                        this.btn_spedisci_P.Enabled = false;
                        this.btn_annulla_P.Visible = false;
                        this.btn_Risp.Enabled = false;
                        this.btn_modificaOgget_P.Enabled = false;
                        this.btn_aggiungi_P.Enabled = false;
                        this.ddl_tipoAtto.Enabled = false;
                        this.ddl_tmpl.Enabled = false;
                        this.imgFasc.Enabled = false;
                        this.txt_CodFascicolo.Enabled = false;
                        this.txt_DescFascicolo.Enabled = false;
                        this.btn_titolario.Enabled = false;
                        btn_verificaPrec_P.Enabled = false;
                        this.btn_risp_grigio.Enabled = false;
                        if (schedaDocumento.tipoProto != null && schedaDocumento.tipoProto.ToString().Equals("P"))
                        {

                            this.btn_spedisci_P.Enabled = false;

                        }
                    }
                    //Se protocollo vuoto non abilito i pulsanti
                    if (schedaDocumento == null || schedaDocumento.docNumber == null || schedaDocumento.docNumber == string.Empty)
                    {
                        this.btn_Risp.Enabled = false;
                        this.btn_risp_dx.Enabled = false;
                        this.btn_risp_grigio.Enabled = false;
                    }
                    #endregion

                    #region Abilitazione Stampa Segnatura PDF

                    if (schedaDocumento.protocollo != null)
                    {
                        if (schedaDocumento != null && schedaDocumento.systemId != null && schedaDocumento.protocollo.segnatura != null)
                        {
                            this.btn_StampaVoidLabel.Enabled = true;
                        }
                    }
                    #endregion

                    try
                    {
                        if (Request.QueryString["protocolla"] != null && Request.QueryString["protocolla"].Equals("1"))
                        {
                            string idRfSeg = Request.QueryString["idRFSeg"];
                            string codRFSeg = Request.QueryString["codRFSeg"];
                            string idRfRicevuta = Request.QueryString["idRFRicevuta"];
                            string codFasc = Request.QueryString["codFasc"];
                            if (!string.IsNullOrEmpty(idRfSeg) && !string.IsNullOrEmpty(codRFSeg))
                            {
                                schedaDocumento.id_rf_prot = idRfSeg;
                                schedaDocumento.cod_rf_prot = codRFSeg;
                                if (!string.IsNullOrEmpty(codFasc))
                                    setFascicolazioneRapida();

                                if (string.IsNullOrEmpty(idRfRicevuta))
                                {
                                    Registro reg = UserManager.getRegistroBySistemId(this, idRfSeg);
                                    if (reg.invioRicevutaManuale.ToUpper().Equals("1"))
                                    {
                                        //1) verifico che si tratti di un predisposto
                                        if (schedaDocumento.protocollo != null && string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                                        {
                                            //2) verifico cha siamo nel caso di interoperabilità
                                            if (schedaDocumento.descMezzoSpedizione != null && schedaDocumento.descMezzoSpedizione.ToUpper().Equals("INTEROPERABILITA"))
                                            {
                                                // 3) verifico che il registro abbia settato da amministrazione l'invio manuale o meno della ricevuta 
                                                if (schedaDocumento.registro.invioRicevutaManuale.ToUpper().Equals("1"))
                                                {
                                                    // 4) calcolo gli RF associati al registro
                                                    DocsPaWR.Registro[] listaRF = UserManager.getListaRegistriWithRF(this, "1", schedaDocumento.registro.systemId);
                                                    if (listaRF != null && listaRF.Length > 0)
                                                    {
                                                        // 5) nel caso di un solo RF associato al registro
                                                        if (listaRF.Length == 1 && !listaRF[0].invioRicevutaManuale.ToUpper().Equals("1"))
                                                            schedaDocumento.id_rf_invio_ricevuta = listaRF[0].systemId;
                                                        else
                                                        {
                                                            // 6) caso di più RF associati al registro e con invio automatico
                                                            if (listaRF.Length > 1)
                                                            {
                                                                bool daMostrarePopUp = false;
                                                                for (int i = 0; i < listaRF.Length; i++)
                                                                {
                                                                    if (!listaRF[i].invioRicevutaManuale.ToUpper().Equals("1"))
                                                                        daMostrarePopUp = true;
                                                                }
                                                                // apro la popup di selezione RF
                                                                if (daMostrarePopUp)
                                                                {
                                                                    RegisterStartupScript("apriRFRicevuta", "<script>apriRFRicevuta('ricevuta','" + idRfSeg + "','" + codRFSeg + "', " + this.ddl_tmpl.SelectedIndex + ", '" + this.txt_CodFascicolo.Text + "');</script>");
                                                                    return;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(idRfRicevuta))
                                            schedaDocumento.id_rf_invio_ricevuta = idRfSeg;
                                        else
                                            schedaDocumento.id_rf_invio_ricevuta = idRfRicevuta;
                                    }
                                }
                                else
                                {
                                    schedaDocumento.id_rf_invio_ricevuta = idRfRicevuta;
                                }
                            }
                            else
                                if (!string.IsNullOrEmpty(idRfRicevuta))
                                    schedaDocumento.id_rf_invio_ricevuta = idRfRicevuta;

                            protocolla();
                            bool isEnableUffRef = false;
                            if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                                isEnableUffRef = true;

                            completaProtocollazione(isEnableUffRef);
                        }

                        if (Request.QueryString["invioManuale"] != null && Request.QueryString["invioManuale"].Equals("1"))
                        {
                            Registro newReg = new Registro();
                            if (!string.IsNullOrEmpty(Request.QueryString["idRFRicevuta"]))
                                newReg = UserManager.getRegistroBySistemId(this, Request.QueryString["idRFRicevuta"]);
                            else
                            {
                                if (!string.IsNullOrEmpty(schedaDocumento.id_rf_prot))
                                    newReg = UserManager.getRegistroBySistemId(this, schedaDocumento.id_rf_prot);
                                else
                                   if(schedaDocumento.registro != null)
                                        newReg = schedaDocumento.registro;
                            }
                            DocumentManager.DocumentoInvioRicevuta(Page, schedaDocumento, newReg);
                        }
                        //else
                        //    if (regSelezionato.invioRicevutaManuale.Equals("0"))
                        //    {
                        //        if (Session["ricevutaMailOcc"]!= null 
                        //            && !string.IsNullOrEmpty(Session["ricevutaMailOcc"].ToString()))
                        //        {
                        //            Registro newReg = UserManager.getRegistroBySistemId(this, Session["ricevutaMailOcc"].ToString());
                        //            DocumentManager.DocumentoInvioRicevuta(Page, schedaDocumento, newReg);
                        //            Session.Add("ricevutaMailOcc",string.Empty);
                        //        }
                        //    }

                    }
                    catch (Exception)
                    {
                        // Do nothing
                    }

                    // abilita/disabilita bottoni e campi
                    enableFormProtocolloFields();
                    setFormProperties();

                    if (ctrl_oggetto.oggetto_text != "" && ctrl_oggetto.oggetto_text.Length < 500)
                        this.ctrl_oggetto.OggToolTip = this.ctrl_oggetto.oggetto_text;

                    if (UserManager.ruoloIsAutorized(this, "FASC_INS_DOC"))
                    {
                        setFascicolazioneRapida();
                        this.ClearResourcesRicercaFascicoliFascRapida();
                    }

                    //abilitazione delle funzioni in base al ruolo
                    UserManager.disabilitaFunzNonAutorizzate(this);
                    if (!UserManager.ruoloIsAutorized(this, this.btn_annulla_P.Tipologia))
                        // btn_annulla_P.Enabled = false;
                        this.btn_annulla_P.Visible = false;
                    // se esiste almeno una coppia registro/rf - casella sulla quale il ruolo ha visibilità per 
                    //la spedizione, allora abilito il pulsante spedisci/rispedisci
                    if ((Session["isDocModificato"] == null || Session["isDocModificato"] == "true") && 
                        schedaDocumento != null && schedaDocumento.protocollo != null &&
                        (schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloUscita)) &&
                        (!string.IsNullOrEmpty((schedaDocumento.protocollo as DocsPAWA.DocsPaWR.ProtocolloUscita).segnatura)) &&
                        // Ruolo autorizzato all'invio o esiste almeno un interoperante PITRE
                        (MultiCasellaManager.RoleIsAuthorizedSend(this.Page, "A") ||
                            (InteroperabilitaSemplificataManager.IsEnabledSimpInterop &&
                                //ABBATANGELI GIANLUIGI - aggiunto "d.canalePref!=null &&" nella condizione sottostante
                                (schedaDocumento.protocollo as DocsPAWA.DocsPaWR.ProtocolloUscita).destinatari != null && (schedaDocumento.protocollo as DocsPAWA.DocsPaWR.ProtocolloUscita).destinatari.Where(d => d.canalePref != null && d.canalePref.descrizione.Equals("Interoperabilità PITRE")).Count() > 0))
                        // Protocollo non annullato
                        && schedaDocumento.protocollo.protocolloAnnullato == null)
                        btn_spedisci_P.Enabled = true;
                    else
                        btn_spedisci_P.Enabled = false;
                    //ELISA
                    ////se il doc è protocollato e devo salvare la risp protocollo
                    //if (schedaDocumento.protocollo != null && schedaDocumento.protocollo.numero != null && schedaDocumento.protocollo.numero != String.Empty)
                    //{
                    //    if (schedaDocumento.modificaRispostaDocumento == true)
                    //        this.btn_salva_P.Enabled = true;

                    //    //this.imgDescOgg.Enabled = true;
                    //    //this.imgListaDest.Enabled = true;
                    //    //this.imgListaDestCC.Enabled = true;
                    //    //this.imgDescOgg.Attributes.Add("onclick", "ApriDescrizioneCampo('O');");

                    //}
                    this.imgDescOgg.Enabled = true;
                    this.imgListaDest.Enabled = true;
                    this.imgListaDestCC.Enabled = true;

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
                    //*************************************
                    //ELISA 
                    ////se il doc è presisposto non consento di usarlo come una risposta ad un altro protocollo
                    ////e disabilito l'icona
                    //if ((schedaDocumento.protocollo.daProtocollare != null &&
                    //    schedaDocumento.protocollo.daProtocollare.Equals("1"))
                    //    || (schedaDocumento.predisponiProtocollazione == true && schedaDocumento.systemId != null))
                    //{
                    //    this.btn_in_rispota_a.Enabled = false;
                    //    this.btn_in_rispostaIngresso_a.Enabled = false;
                    //}

                    DocumentManager.setDocumentoInLavorazione(schedaDocumento);
                    DocumentManager.setDocumentoSelezionato(schedaDocumento);

                    if ((schedaDocumento.systemId != null && schedaDocumento.documenti != null && schedaDocumento.documenti.Length > 0) ||
                        DocumentManager.onSessionRepositoryContext())
                    {
                        FileManager.setSelectedFile(this, schedaDocumento.documenti[0]);
                    }

                    #region proto_in_out
                    if (PIN && !POUT)
                    {
                        this.rbl_InOut_P.SelectedIndex = 0;
                        this.rbl_InOut_P.Enabled = false;
                    }
                    else if (POUT && !PIN)
                    {
                        this.rbl_InOut_P.SelectedIndex = 1;
                        this.rbl_InOut_P.Enabled = false;
                    }

                    // Se il documento è riproposto ed è fascicolato viene impostato a 0 il flag fascicolato in modo
                    // che non venga più visualizzato il messaggio "Descrizione non visualizzabile"
                    if (Session["docRiproposto"] != null && schedaDocumento != null && schedaDocumento.fascicolato == "1")
                        schedaDocumento.fascicolato = "0";

                    //se presente e abilitata chiave "FE_FASC_PRIMARIA" in dpa_chiavi_configurazione e 
                    //se il documento è fascicolato allora si rende visibile la label che indica 
                    //la descrizione del fascicolo principale
                    string valoreChiave;
                    valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_FASC_PRIMARIA");

                    if (!string.IsNullOrEmpty(schedaDocumento.fascicolato) && schedaDocumento.fascicolato == "1" && !string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1"))
                    {

                        string descFascPrimaria = DocumentManager.GetFascicolazionePrimaria(this, schedaDocumento.docNumber);
                        if (!string.IsNullOrEmpty(descFascPrimaria))
                        {
                            lbl_fasc_Primaria.Text = "Fasc. Princ. " + descFascPrimaria;
                            pnl_fasc_Primaria.Visible = true;
                        }
                    }
                    else
                        pnl_fasc_Primaria.Visible = false;
                }

                verificaHMdiritti();

                // setta il campo nascosto che contiene il tipo di protocollo (possibili valori: In, Out, Own)
                this.hd_tipoProtocollazione.Value = this.rbl_InOut_P.SelectedItem.Value;

                //Richiamo il metodo verificaCampiPersonalizzati per abilitare o meno
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null)
                {
                    if (this.dettaglioNota.NewNote)
                        if (Session["isDocModificato"] != null && Session["isDocModificato"].ToString().ToUpper() == "TRUE")
                            this.daAggiornareDx = false;

                    if (!this.daAggiornareDx)
                    {
                        if (Session["isDocModificato"] != null && Session["isDocModificato"].ToString().ToUpper() == "TRUE")
                            this.ddl_tipoAtto.Enabled = false;
                    }
                    else
                        ProfilazioneDocManager.verificaCampiPersonalizzati(this, schedaDocumento);
                }

                #region GESTIONE E ABILITAZIONE NUOVA/VECCHIA RUBRICA

                // Inizializzazione condizionale link rubrica
                use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
                HtmlImage btn_rubrica_p_sempl = (HtmlImage)FindControl("btn_rubrica_p_sempl");
                HtmlImage btn_rubrica_p = (HtmlImage)FindControl("btn_rubrica_p");
                HtmlImage btn_RubrMitInt_Sempl = (HtmlImage)FindControl("btn_RubrMitInt_Sempl");
                HtmlImage btn_RubrMitInt = (HtmlImage)FindControl("btn_RubrMitInt");
                HtmlImage btn_RubrDest_Sempl_P = (HtmlImage)FindControl("btn_RubrDest_Sempl_P");
                HtmlImage btn_RubrDest_P = (HtmlImage)FindControl("btn_RubrDest_P");
                HtmlImage btn_RubrMittMultiplo = (HtmlImage)FindControl("btn_RubrMittMultiplo");

                if (use_new_rubrica != "1")
                {
                    // Rubrica tradizionale					 
                    btn_rubrica_p_sempl.Attributes["onClick"] = String.Format("verificaApriRubricaSemplice('proto','mitt','{0}');", this.btn_rubrica_p_sempl_state.ToString());
                    btn_rubrica_p.Attributes["onClick"] = String.Format("verificaApriRubrica('" + this.val_proto_sele + "','mitt','{0}');", this.btn_rubrica_p_state.ToString());
                    btn_RubrMitInt_Sempl.Attributes["onClick"] = String.Format("verificaApriRubricaSemplice('proto','mittInt','{0}');", this.btn_RubrMitInt_Sempl_state.ToString());
                    btn_RubrMitInt.Attributes["onClick"] = String.Format("verificaApriRubrica('proto','mittInt','{0}');", this.btn_RubrMitInt_state.ToString());
                    btn_RubrDest_Sempl_P.Attributes["onClick"] = String.Format("verificaApriRubrica('proto','dest','{0}');", this.btn_RubrDest_Sempl_P_state.ToString());
                    btn_RubrDest_P.Attributes["onClick"] = String.Format("verificaApriRubrica('" + this.val_proto_sele + "','dest','{0}');", this.btn_RubrDest_P_state.ToString());
                    btn_RubrMittMultiplo.Attributes["onClick"] = String.Format("verificaApriRubrica('proto','mittMultiplo','{0}');", this.btn_RubrMittMultiplo_state.ToString());
                }
                else
                {
                    if (schedaDocumento != null && schedaDocumento.tipoProto != null && schedaDocumento.tipoProto != "")
                    {
                        // Nuova rubrica
                        switch (schedaDocumento.tipoProto)
                        {
                            case "A":
                                if (btn_rubrica_p_sempl_state)
                                    btn_rubrica_p_sempl.Attributes["onClick"] = String.Format("_ApriRubrica('{0}');", "mitt");
                                if (btn_rubrica_p_state)
                                    btn_rubrica_p.Attributes["onClick"] = String.Format("_ApriRubrica('{0}');", "mitt");
                                if (btn_RubrMittMultiplo_state)
                                    btn_RubrMittMultiplo.Attributes["onClick"] = String.Format("_ApriRubrica('{0}');", "mittMultiplo");
                                break;

                            case "P":
                                if (btn_rubrica_p_sempl_state)
                                    btn_rubrica_p_sempl.Attributes["onClick"] = String.Format("_ApriRubrica('{0}');", "mittOut");
                                if (btn_rubrica_p_state)
                                    btn_rubrica_p.Attributes["onClick"] = String.Format("_ApriRubrica('{0}');", "mittOut");
                                if (btn_RubrDest_Sempl_P_state)
                                    btn_RubrDest_Sempl_P.Attributes["onClick"] = String.Format("_ApriRubrica('{0}');", "dest");
                                if (btn_RubrDest_P_state)
                                    btn_RubrDest_P.Attributes["onClick"] = String.Format("_ApriRubrica('{0}');", "dest");
                                break;

                            case "I":
                                if (btn_rubrica_p_sempl_state)
                                    btn_rubrica_p_sempl.Attributes["onClick"] = String.Format("_ApriRubrica('{0}');", "mittInterno");
                                if (btn_rubrica_p_state)
                                    btn_rubrica_p.Attributes["onClick"] = String.Format("_ApriRubrica('{0}');", "mittInterno");
                                if (btn_RubrDest_Sempl_P_state)
                                    btn_RubrDest_Sempl_P.Attributes["onClick"] = String.Format("_ApriRubrica('{0}');", "destInterno");
                                if (btn_RubrDest_P_state)
                                    btn_RubrDest_P.Attributes["onClick"] = String.Format("_ApriRubrica('{0}');", "destInterno");
                                break;
                        }
                    }
                    if (btn_RubrMitInt_Sempl_state)
                        btn_RubrMitInt_Sempl.Attributes["onClick"] = String.Format("_ApriRubrica('{0}');", "mittInt");
                    if (btn_RubrMitInt_state)
                        btn_RubrMitInt.Attributes["onClick"] = String.Format("_ApriRubrica('{0}');", "mittInt");

                }

                // Cambio cursore - OnMouseOver
                btn_rubrica_p_sempl.Attributes["onMouseOver"] = String.Format("verificaChangeCursorT('hand','btn_rubrica_p_sempl','{0}')", btn_rubrica_p_sempl_state.ToString());
                btn_rubrica_p.Attributes["onMouseOver"] = String.Format("verificaChangeCursorT('hand','btn_rubrica_p','{0}')", btn_rubrica_p_state.ToString());
                btn_RubrMitInt_Sempl.Attributes["onMouseOver"] = String.Format("verificaChangeCursorT('hand','btn_RubrMitInt_Sempl','{0}')", btn_RubrMitInt_Sempl_state.ToString());
                btn_RubrMitInt.Attributes["onMouseOver"] = String.Format("verificaChangeCursorT('hand','btn_RubrMitInt','{0}')", btn_RubrMitInt_state.ToString());
                btn_RubrDest_Sempl_P.Attributes["onMouseOver"] = String.Format("verificaChangeCursorT('hand','btn_RubrDest_Sempl_P','{0}')", btn_RubrDest_Sempl_P_state.ToString());
                btn_RubrDest_P.Attributes["onMouseOver"] = String.Format("verificaChangeCursorT('hand','btn_RubrDest_P','{0}')", btn_RubrDest_P_state.ToString());
                btn_RubrMittMultiplo.Attributes["onMouseOver"] = String.Format("verificaChangeCursorT('hand','btn_RubrMittMultiplo','{0}')", btn_RubrMittMultiplo_state.ToString());

                #endregion
                    #endregion

                if (!this.IsPostBack)
                {
                    // Caricamento note documento
                    this.FetchNoteDocumento();
                }

                // Impostazione del documento corrente nello stato del contesto
                this.SetDocumentOnContext();

                //tipo atto se obligatorio
                string idAmmin = UserManager.getInfoUtente().idAmministrazione;
                if (DocumentManager.GetTipoDocObbl(idAmmin).Equals("1"))
                    //if (System.Configuration.ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"] != null
                    //    && System.Configuration.ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"] == "1")
                    this.lbl_atto.Text = "Tipologia documento *";
                else
                    this.lbl_atto.Text = "Tipologia documento";

                if (schedaDocumento != null && schedaDocumento.systemId != null && schedaDocumento.systemId != "")
                    this.btn_verificaPrec_P.Enabled = false;

                if (schedaDocumento != null && schedaDocumento.protocollo != null && schedaDocumento.protocollo.daProtocollare != null && schedaDocumento.protocollo.daProtocollare.Equals("1"))
                    if (!string.IsNullOrEmpty(schedaDocumento.systemId))
                    {
                        this.btn_annullaPred.Visible = true;
                    }

                if (isRiproposto != null && isRiproposto)
                {
                    DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                    if (template != null && template.DOC_NUMBER != string.Empty)
                    {
                        template.DOC_NUMBER = string.Empty;
                        Session.Add("template", template);
                    }

                    Session.Remove("docRiproposto");
                }

                if (this.Session["showConfirmSpedizioneAutomatica"] != null)
                {
                    this.msg_SpedizioneAutomatica.Confirm("E' stata richiesta la spedizione senza aver associato il documento principale.\\nSi vuole procedere comunque (SCELTA SCONSIGLIATA)?");
                    Session.Remove("showConfirmSpedizioneAutomatica");
                }
            }
            catch (System.Web.HttpException ex1)
            {
                ErrorManager.redirect(this, ex1, "protocollazione");
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
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

                    if (this.wws.ereditaVisibilita(appoIdAmm.Value, appoIdMod.Value))
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
            if (string.IsNullOrEmpty(CheckFields("P")))
            {
                fieldsOK.Value = "true";
            }
            else
            {
                fieldsOK.Value = "false";
            }

            if (Session["abilitaModificaSpedizione"] != null && (bool)Session["abilitaModificaSpedizione"])
            {
                this.btn_salva_P.Enabled = true;
                //Session.Remove("abilitaModificaSpedizione");
            }

            if (System.Configuration.ConfigurationManager.AppSettings["NON_VISUALIZZA_FRECCIA_RISPOSTA"] != null
                    && System.Configuration.ConfigurationManager.AppSettings["NON_VISUALIZZA_FRECCIA_RISPOSTA"] == "1")
            {
                if (schedaDocumento.systemId == null || GetCountDocInRisposta(schedaDocumento.systemId) == 0)
                {
                    this.btn_risp_dx.Visible = false;
                }
                else
                {
                    this.btn_risp_dx.Visible = true;
                }
            }

            if (Session["oggetto_popup"] != null)
            {
                this.ctrl_oggetto.oggetto_text = Session["oggetto_popup"].ToString();
                this.schedaDocumento.oggetto.descrizione = this.ctrl_oggetto.oggetto_text;
                this.schedaDocumento.oggetto.daAggiornare = true;
                if (this.ctrl_oggetto.cod_oggetto_isVisible)
                {
                    this.ctrl_oggetto.cod_oggetto_text = string.Empty;
                    this.schedaDocumento.oggetto.codOggetto = string.Empty;
                }
                Session.Remove("oggetto_popup");
            }

            /*
            if (this.Panel_ProfilazioneDinamica.Visible)
            {
                if (UserManager.isFiltroAooEnabled(this))
                {
                    DocsPaWR.Registro[] userRegistri = DocsPAWA.UserManager.getListaRegistri(this.Page);
                    this.ddl_tipoAtto.Enabled = UserManager.verifyRegNoAOO(schedaDocumento, userRegistri);
                }

            }
            */

            if (this.schedaDocumento != null)
            {
                int isInAdl = DocumentManager.isDocInADL(this.schedaDocumento.systemId, this);
                if (isInAdl == 1)
                {
                    this.btn_aggiungi_P.Enabled = true;
                    this.btn_aggiungi_P.ToolTip = "Rimuovi documento da area di lavoro";
                    this.btn_aggiungi_P.ImageUrl = "../images/proto/canc_area.gif";
                }
                else
                {
                    this.btn_aggiungi_P.Enabled = true;
                    this.btn_aggiungi_P.ToolTip = "Inserisci documento in area di lavoro";
                    this.btn_aggiungi_P.ImageUrl = "../images/proto/ins_area.gif";
                }
            }


            #region DISABILITAZIONE CONTROLLI SUL CONSOLIDAMENTO (ATTENZIONE: DEVE RIMANERE SEMPRE L'ULTIMA OPERAZIONE FATTA NEL PRERENDER)

            // Abilitazione / Disabilitazione controlli in caso di documento consolidato
            if (this.schedaDocumento != null)
            {
                if (this.schedaDocumento.ConsolidationState != null &&
                    this.schedaDocumento.ConsolidationState.State > DocumentConsolidationStateEnum.None)
                {
                    string message = string.Empty;

                    // Diabilitazione controlli su documento consolidato
                    if (this.schedaDocumento.ConsolidationState.State == DocumentConsolidationStateEnum.Step1)
                    {
                        message = "CONSOLIDATO CONTENUTO";
                    }
                    else if (this.schedaDocumento.ConsolidationState.State == DocumentConsolidationStateEnum.Step2)
                    {
                        message = "CONSOLIDATO CONTENUTO E METADATI";

                        System.Drawing.Color colorDisabled = System.Drawing.Color.FromArgb(225, 225, 225);

                        // Diabilitazione controlli su documento consolidato nei metadati
                        this.lbl_segnatura.BackColor = colorDisabled;

                        this.ctrl_oggetto.oggetto_isReadOnly = true;
                        this.ctrl_oggetto.BackColor = colorDisabled;

                        // Non è possibile annullare il protocollo una volta consolidato il documento
                        this.btn_annulla_P.Enabled = false;

                        if (this.schedaDocumento.protocollo.GetType() == typeof(DocsPaWR.ProtocolloEntrata))
                        {
                            this.txt_NumProtMit_P.ReadOnly = true;
                            this.txt_NumProtMit_P.BackColor = colorDisabled;
                            this.txt_DataProtMit_P.ReadOnly = true;
                            this.txt_DataProtMit_P.BackColor = colorDisabled;
                            this.btn_verificaPrec_P.Enabled = false;
                            this.txt_DataArrivo_P.ReadOnly = true;
                            this.txt_DataArrivo_P.BackColor = colorDisabled;
                            this.txt_OraPervenuto_P.Enabled = false;
                            this.txt_OraPervenuto_P.BackColor = colorDisabled;
                            if (wws.isEnableRiferimentiMittente())
                            {

                                txt_riferimentoMittente.ReadOnly = true;
                                txt_riferimentoMittente.BackColor = Color.Gainsboro;
                            }
                        }

                        // Disabilitazione controlli mittente
                        this.rubrica_veloce.ReadOnly = true;
                        this.rubrica_veloce.BackColor = colorDisabled;
                        this.rubrica_veloce_mitt_multi.ReadOnly = true;
                        this.rubrica_veloce_mitt_multi.BackColor = colorDisabled;

                        this.panel_DettaglioMittenti.Enabled = false;
                        this.panel_Mit.Enabled = false;
                        this.panel_mittInt.Enabled = false;

                        if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                        {
                            this.rubrica_veloce.Visible = false;
                            this.rubrica_veloce_destinatario.Visible = false;
                            this.rubrica_veloce_mitt_multi.Visible = false;
                        }

                        if (!string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                        {
                            this.pnl_mittente_veloce.Visible = false;
                            this.pnl_destinatario_veloce.Visible = false;
                        }

                        this.btn_downMittente.Enabled = false;
                        this.btn_upMittente.Enabled = false;
                        this.btn_RubrMittMultiplo.Style["disabled"] = Boolean.TrueString;
                        this.btn_CancMittMultiplo.Enabled = false;
                        this.btn_RubrMitInt_Sempl.Style["disabled"] = Boolean.TrueString;
                        this.btn_rubrica_p_sempl.Style["disabled"] = Boolean.TrueString;
                        this.btn_RubrMittMultiplo.Style["disabled"] = Boolean.TrueString;
                        this.btn_RubrMitInt.Style["disabled"] = Boolean.TrueString;

                        this.btn_rubrica_p.Disabled = true;
                        this.btn_RubrMittMultiplo.Disabled = true;
                        this.btn_RubrDest_P.Disabled = true;

                        this.txt_CodMit_P.ReadOnly = true;
                        this.txt_CodMit_P.BackColor = colorDisabled;
                        this.txt_DescMit_P.ReadOnly = true;
                        this.txt_DescMit_P.BackColor = colorDisabled;

                        this.txt_CodMitInt_P.ReadOnly = true;
                        this.txt_CodMitInt_P.BackColor = colorDisabled;
                        this.txt_DescMitInt_P.ReadOnly = true;
                        this.txt_DescMitInt_P.BackColor = colorDisabled;

                        this.lbx_mittMultiplo.Enabled = false;
                        this.lbx_mittMultiplo.BackColor = colorDisabled;

                        // Disabilitazione controlli destinatari 
                        this.rubrica_veloce_destinatario.ReadOnly = true;
                        this.rubrica_veloce_destinatario.BackColor = colorDisabled;

                        this.panel_Dest.Enabled = false;
                        this.pnl_rubr_dest_Semplice.Enabled = false;
                        this.btn_RubrDest_Sempl_P.Style["disabled"] = Boolean.TrueString;
                        this.pnl_rubr_des.Enabled = false;
                        this.btn_RubrDest_P.Style["disabled"] = Boolean.TrueString;
                        this.btn_aggiungiDest_P.Enabled = false;
                        this.btn_cancDestCC.Enabled = false;

                        this.txt_CodDest_P.ReadOnly = true;
                        this.txt_CodDest_P.BackColor = colorDisabled;
                        this.txt_DescDest_P.ReadOnly = true;
                        this.txt_DescDest_P.BackColor = colorDisabled;
                        this.lbx_dest.Enabled = false;
                        this.lbx_dest.BackColor = colorDisabled;
                        this.lbx_destCC.Enabled = false;
                        this.lbx_destCC.BackColor = colorDisabled;
                    }

                    this.lblStatoConsolidamento.Visible = (!string.IsNullOrEmpty(message));
                    this.lblStatoConsolidamento.Text = message;
                }
            }

            //Controllo i diritti sulla schedadocumento
            if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.accessRights) && schedaDocumento.accessRights.Equals("45"))
                this.ddl_tipoAtto.Enabled = false;

            #endregion

            //Verifica atipicita documento
            Utils.verificaAtipicita(schedaDocumento, DocsPaWR.TipoOggettoAtipico.DOCUMENTO, ref btn_visibilita);

            // Verifica del vero creatore del documento
            Utils.CheckCreatorRole(schedaDocumento, ref this.btn_log);


            logger.Info("END");
        }


        private void GestioneRispostaDocumenti()
        {
            if (schedaDocumento.rispostaDocumento != null && (!string.IsNullOrEmpty(schedaDocumento.rispostaDocumento.segnatura) || !string.IsNullOrEmpty(schedaDocumento.rispostaDocumento.docNumber)))
            {
                if (schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero) && schedaDocumento.modificaRispostaDocumento == false)
                {
                    this.btn_in_rispota_a.Enabled = false;
                }
                else
                {
                    this.btn_in_rispota_a.Enabled = true;
                }

                this.pnl_text_risposta.Visible = true;
                if (!string.IsNullOrEmpty(schedaDocumento.protocollo.segnatura))
                {
                    this.btn_risp_sx.Visible = true;
                }
            }
            else
            {
                this.btn_in_rispota_a.Enabled = true;
                this.btn_risp_sx.Visible = false;
                this.pnl_text_risposta.Visible = false;
            }


            //se il doc è protocollato e devo salvare la risp protocollo
            if (schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
            {
                if (schedaDocumento.modificaRispostaDocumento == true)
                    this.btn_salva_P.Enabled = true;
            }

            //se il doc è predisposto e devo salvare la risp al predisposto
            if (schedaDocumento.docNumber != null && schedaDocumento.modificaRispostaDocumento == true)
            {
                this.btn_salva_P.Enabled = true;
            }

            //if (schedaDocumento != null && schedaDocumento.protocollo != null
            //     && this.schedaDocumento.protocollo.segnatura != null
            //     && !this.schedaDocumento.protocollo.segnatura.Equals(""))
            if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.systemId))
            {

                DocsPAWA.DocsPaWR.InfoDocumento infoDocRisposta = GetDocInRisposta(schedaDocumento.systemId);
                
                if (infoDocRisposta != null)
                {

                    if (!string.IsNullOrEmpty(infoDocRisposta.segnatura)) //la risposta è un doc protocollato
                    {
                        this.btn_risp_dx.AlternateText = "visualizza protocollo in risposta";
                    }
                    else
                    {
                        this.btn_risp_dx.AlternateText = "visualizza documento in risposta";
                    }
                }
                else
                {
                    /* ABBATANGELI GIANLUIGI
                     * Verifico il caso in cui ci siano più di un 
                     * documento (protocollo,doc grigio o predisposto) in risposta al protocollo */
                    if (GetCountDocInRisposta(schedaDocumento.systemId) > 1)
                    {
                        this.btn_risp_dx.AlternateText = "visualizza elenco documenti in risposta al " + this.schedaDocumento.protocollo.segnatura;
                        this.btn_risp_dx.Attributes.Add("onclick", "ShowDialogRispostaProtocollo ('" + this.schedaDocumento.systemId + "','" + this.schedaDocumento.registro.systemId + "','" + this.Server.UrlEncode(this.schedaDocumento.protocollo.segnatura) + "','" + this.schedaDocumento.tipoProto + "'); return false");
                        this.btn_visibilita.Enabled = true;
                    }
                    else
                    {
                        //this.btn_risp_dx.Enabled = false;
                        //this.btn_risp_dx.Attributes.Add("onclick", "return false");
                        this.btn_risp_dx.Attributes.Add("onclick", "alert ('Non ci sono documento in risposta.');");
                    }
                }

                if (schedaDocumento.rispostaDocumento != null)
                {
                    if (!string.IsNullOrEmpty(schedaDocumento.rispostaDocumento.segnatura)) //la risposta è un doc protocollato
                    {
                        this.btn_risp_sx.AlternateText = "vai al protocollo " + this.schedaDocumento.rispostaDocumento.segnatura;
                    }
                    else
                    {
                        //ABBATANGELI GIANLUIGI - se non c'è segnatura è un doc grigio.
                        this.btn_risp_sx.AlternateText = "vai al documento " + this.schedaDocumento.rispostaDocumento.docNumber;
                    }
                }
            }

            if (schedaDocumento.protocollo != null)
            {
                //stiamo su un protocollo in ingresso e il pulsante delle catene viene attivato solamente se 
                //l'utente è abilitato ad effettuare un protocollo in uscita.
                if (this.rbl_InOut_P.Items[0].Selected)
                {
                    if (schedaDocumento.protocollo.segnatura != null && !(schedaDocumento.protocollo.segnatura.Equals("")))
                    {
                        if (this.IsRoleOutwardEnabled())
                        {
                            this.btn_Risp.Enabled = true;
                            //this.btn_risp_grigio.Enabled = true;
                        }
                        else
                        {
                            this.btn_Risp.Enabled = false;
                            //this.btn_risp_grigio.Enabled = false;
                        }
                        this.btn_risp_dx.Enabled = true;

                    }

                }
                //stiamo su un protocollo in uscita e il pulsante delle catene viene attivato solamente se 
                //l'utente è abilitato ad effettuare un protocollo in ingresso.
                if (this.rbl_InOut_P.Items[1].Selected)
                {
                    if (schedaDocumento.protocollo.segnatura != null && !(schedaDocumento.protocollo.segnatura.Equals("")))
                    {
                        if (this.IsRoleInwardEnabled())
                        {
                            this.btn_Risp.Enabled = true;
                            //this.btn_risp_grigio.Enabled = true;
                        }
                        else
                        {
                            this.btn_Risp.Enabled = false;
                            //this.btn_risp_grigio.Enabled = false;
                        }
                        this.btn_risp_dx.Enabled = true;

                    }

                }

                if (this.rbl_InOut_P.Items.Count > 2 && this.rbl_InOut_P.Items[2] != null && this.rbl_InOut_P.Items[2].Selected)
                {
                    if (schedaDocumento.protocollo.segnatura != null && !(schedaDocumento.protocollo.segnatura.Equals("")))
                    {
                        if (this.IsRoleInternalEnabled() || this.IsRoleInwardEnabled() || this.IsRoleOutwardEnabled())
                        {
                            this.btn_Risp.Enabled = true;
                            //this.btn_risp_grigio.Enabled = true;
                        }
                        else
                        {
                            this.btn_Risp.Enabled = false;
                            //this.btn_risp_grigio.Enabled = false;
                        }
                        this.btn_risp_dx.Enabled = true;

                    }
                }

                if (schedaDocumento.protocollo.segnatura != null && !(schedaDocumento.protocollo.segnatura.Equals("")))
                {
                    if (this.IsRoleGrigiEnabled())
                        this.btn_risp_grigio.Enabled = true;
                    else
                        this.btn_risp_grigio.Enabled = false;
                }

            }


            #region POPOLAMENTO CAMPO RISPOSTA AL PROTOCOLLO

            //if (schedaDocumento.protocollo != null )
            if (schedaDocumento != null)
            {
                //risposta al protocollo 
                if (schedaDocumento.rispostaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.rispostaDocumento.segnatura))
                {
                    this.txt_RispProtSegn_P.Text = schedaDocumento.rispostaDocumento.segnatura;
                }
                else
                {
                    if (schedaDocumento.rispostaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.rispostaDocumento.docNumber))
                    {
                        this.txt_RispProtSegn_P.Text = schedaDocumento.rispostaDocumento.docNumber;
                    }
                    else
                    {
                        this.txt_RispProtSegn_P.Text = "";
                    }
                }
            }
            if (schedaDocumento.protocollo != null && ((schedaDocumento.protocollo.daProtocollare != null && schedaDocumento.protocollo.daProtocollare.Equals("1"))
               || (schedaDocumento.predisponiProtocollazione == true && schedaDocumento.systemId != null)))
            {
                if (schedaDocumento.rispostaDocumento != null)
                {
                    this.btn_risp_sx.Visible = true;
                    this.btn_risp_sx.Enabled = true;
                    if (!string.IsNullOrEmpty(schedaDocumento.rispostaDocumento.segnatura) || !string.IsNullOrEmpty(schedaDocumento.rispostaDocumento.docNumber))
                    {
                        if (string.IsNullOrEmpty(schedaDocumento.rispostaDocumento.segnatura) && !string.IsNullOrEmpty(schedaDocumento.rispostaDocumento.docNumber))
                        {

                            this.btn_risp_sx.AlternateText = "vai al documento " + this.schedaDocumento.rispostaDocumento.docNumber;
                            this.txt_RispProtSegn_P.Text = schedaDocumento.rispostaDocumento.docNumber;
                        }
                        else
                        {
                            this.btn_risp_sx.AlternateText = "vai al documento " + this.schedaDocumento.rispostaDocumento.segnatura;
                            this.txt_RispProtSegn_P.Text = schedaDocumento.rispostaDocumento.segnatura;
                        }
                        pnl_text_risposta.Visible = true;
                    }
                    else
                    {
                        pnl_text_risposta.Visible = false;
                        this.txt_RispProtSegn_P.Text = "";
                        this.btn_risp_sx.Visible = false;
                        this.btn_risp_sx.Enabled = false;
                    }
                }
                else
                {
                    pnl_text_risposta.Visible = false;
                    this.txt_RispProtSegn_P.Text = "";
                }

            }
            #endregion

            if (this.txt_RispProtSegn_P != null && this.txt_RispProtSegn_P.Text != "")
            {
                this.btn_risp_sx.Enabled = true;
            }
            else
            {
                this.btn_risp_sx.Enabled = false;
            }

            //se il protocollo è annullato disabilito il pulsante relativo alle catene documentali
            if (schedaDocumento.protocollo != null)
            {
                if (schedaDocumento.protocollo.protocolloAnnullato != null)
                {
                    this.btn_annulla_P.Enabled = false;
                    this.btn_Risp.Enabled = false;
                    this.btn_in_rispota_a.Enabled = false;
                    this.btn_risp_grigio.Enabled = false;
                }
            }

        }

        public DocsPAWA.DocsPaWR.InfoDocumento GetDocInRisposta(string sys)
        {
            DocsPAWA.DocsPaWR.InfoDocumento infoDocRisp = null;
            if (GetFiltroDocInRisposta(schedaDocumento))
            {
                int numTotPage;
                int nRec;
                DocumentManager.setFiltroRicDoc(this, qV);
                DocsPaWR.InfoUtente infoUt = new DocsPAWA.DocsPaWR.InfoUtente();
                infoUt = UserManager.getInfoUtente(this);
                ListaFiltri = DocumentManager.getFiltroRicDoc(this);
                DocsPAWA.DocsPaWR.InfoDocumento[] infoDoc;
                SearchResultInfo[] idProfileList;
                if (UserManager.isFiltroAooEnabled(this))
                    infoDoc = DocumentManager.getQueryInfoDocumentoPaging(infoUt.idGruppo, infoUt.idPeople, this, this.ListaFiltri, 1, out numTotPage, out nRec, true, true, false, false, out idProfileList);
                else
                    infoDoc = DocumentManager.getQueryInfoDocumentoPaging(infoUt.idGruppo, infoUt.idPeople, this, this.ListaFiltri, 1, out numTotPage, out nRec, true, true, true, false, out idProfileList);

                if (infoDoc != null && infoDoc.Length == 1)
                {
                    infoDocRisp = infoDoc[0];
                }
            }
            return infoDocRisp;
        }

        public int GetCountDocInRisposta(string sys)
        {
            int numDocInRisposta = 0;
            DocsPAWA.DocsPaWR.InfoDocumento infoDocRisp = null;
            //ABBATANGELI GIANLUIGI
            if (!string.IsNullOrEmpty(sys))
            {
                if (GetFiltroDocInRisposta(schedaDocumento))
                {
                    int numTotPage;
                    int nRec;
                    DocumentManager.setFiltroRicDoc(this, qV);
                    DocsPaWR.InfoUtente infoUt = new DocsPAWA.DocsPaWR.InfoUtente();
                    infoUt = UserManager.getInfoUtente(this);
                    ListaFiltri = DocumentManager.getFiltroRicDoc(this);
                    DocsPAWA.DocsPaWR.InfoDocumento[] infoDoc;
                    if (UserManager.isFiltroAooEnabled(this))
                        numDocInRisposta = DocumentManager.getNumDocInRisposta(infoUt.idGruppo, infoUt.idPeople, this, this.ListaFiltri, false);
                    else
                        numDocInRisposta = DocumentManager.getNumDocInRisposta(infoUt.idGruppo, infoUt.idPeople, this, this.ListaFiltri, true);
                }
            }
            return numDocInRisposta;
        }


        public bool GetFiltroDocInRisposta(DocsPAWA.DocsPaWR.SchedaDocumento sd) //string docSys, string idRegistro, string tipoProto
        {
            try
            {
                if (sd == null)
                    return false;

                qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
                fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

                //Filtro per protocolli in PARTENZA
                //fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                ////se il protocollo corrente è in partenza allora devo cercare le sue risposte
                ////tra i protocolli in ingresso, viceversa per i protocolli in arrivo
                //if (tipoProto.Equals("P"))
                //{
                //    fV1.valore = "A";
                //}
                //else
                //{
                //    fV1.valore = "P";
                //}

                //fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                if (!UserManager.isFiltroAooEnabled(this))
                {
                    if (sd.tipoProto.Equals("A") || sd.tipoProto.Equals("P") || (sd.protocollo != null && !string.IsNullOrEmpty(sd.protocollo.daProtocollare) && sd.protocollo.daProtocollare.Equals("1") || sd.predisponiProtocollazione == true))
                    {
                        //Filtro per REGISTRI VISIBILI ALL'UTENTE
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRI_UTENTE_IN_CONDITION_CON_NULL.ToString();
                        fV1.valore = (String)Session["inRegCondition"];
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    else
                    {

                        //Filtro per REGISTRO DEL DOCUMENTO PROTOCOLLATO
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                        fV1.valore = sd.registro.systemId;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                //				//Filtro per REGISTRO
                //				fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //				fV1.argomento=DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                //				fV1.valore = idRegistro;
                //				fVList = Utils.addToArrayFiltroRicerca(fVList,fV1);

                //Filtro per ID_PARENT
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ID_PARENT.ToString();
                fV1.valore = sd.systemId;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                //if (tipoProto != "G")
                //{
                //    //Filtro per SOLI DOCUMENTI PROTOCOLLATI
                //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //    fV1.argomento = DocsPaWR.FiltriDocumento.DA_PROTOCOLLARE.ToString();
                //    fV1.valore = "0";  //corrisponde a 'false'
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //}
                qV[0] = fVList;
                return true;

            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
                return false;
            }
        }


        /// <summary>
        /// Caricamento note del documento
        /// </summary>
        protected void FetchNoteDocumento()
        {
            if (this.NotaDocumentoEnabled)
            {
                string clientId = string.Empty;

                if (UserManager.getStatoRegistro(schedaDocumento.registro) == "G")
                    clientId = this.btn_protocollaGiallo_P.ClientID;
                else
                    clientId = this.btn_protocolla_P.ClientID;

                //this.dettaglioNota.AttatchPulsanteConferma(clientId);
                this.dettaglioNota.Fetch();
            }
        }

        // private bool _onDettaglioNotaEnabledChanged = false;

        protected void DettaglioNota_OnEnabledChanged(object sender, EventArgs e)
        {
            // this._onDettaglioNotaEnabledChanged = true;
        }

        private void chkPrivato_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.chkPrivato.Checked)
                schedaDocumento.privato = "1";
            else
            {
                //schedaDocumento.daAggiornarePrivato = true;
                schedaDocumento.privato = "0";
            }
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


        private void verificaHMdiritti()
        {
            //disabilitazione dei bottoni in base all'autorizzazione di HM 
            //sul documento
            if (schedaDocumento != null && schedaDocumento.accessRights != null && schedaDocumento.accessRights != "")
            {
                if (UserManager.disabilitaButtHMDiritti(schedaDocumento.accessRights) || (schedaDocumento.inCestino != null && schedaDocumento.inCestino == "1") || (schedaDocumento.inArchivio != null && schedaDocumento.inArchivio == "1"))
                {
                    //bottoni che devono essere disabilitati in caso
                    //di diritti di sola lettura
                    this.btn_modificaOgget_P.Enabled = false;
                    this.btn_protocolla_P.Enabled = false;
                    this.btn_protocollaGiallo_P.Enabled = false;

                    if (this.NotaDocumentoEnabled && this.dettaglioNota.ReadOnly)
                        this.btn_salva_P.Enabled = this.dettaglioNota.Enabled;
                    else
                        this.btn_salva_P.Enabled = false;

                    this.btn_spedisci_P.Enabled = false;

                    this.btn_riproponiDati_P.Enabled = this.IsEnabledRiproponiConConoscenza;
                    //this.btn_annulla_P.Enabled = false;
                    this.btn_annulla_P.Visible = false;
                    this.btn_Risp.Enabled = false;
                    // PALUMBO: abilitata freccia destra per catene in caso di doc in stato finale
                    //this.btn_risp_dx.Enabled = false;
                    this.btn_risp_dx.Enabled = true;
                    this.btn_addTipoAtto.Enabled = false;
                    this.btn_inoltra.Visible = false;
                    this.btn_in_rispota_a.Enabled = false;
                    this.btn_trasmetti.Enabled = false;
                    this.btn_risp_grigio.Enabled = false;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void enableFormProtocolloFields()
        {
            // dettagli mittente
            if (schedaDocumento.tipoProto == "A")
            {
                this.btn_DetMit_P.Enabled = true;
            }

            // dettagli mittente intermedio
            this.btn_DetMitInt_P.Enabled = true;

            if (schedaDocumento.protocollo != null)
            {
                if ((schedaDocumento.protocollo.numero != null && !schedaDocumento.protocollo.numero.Equals("")))
                {
                    this.btn_protocolla_P.Enabled = false;

                    this.btn_protocollaGiallo_P.Enabled = false;

                    if (Convert.ToBoolean(this.ViewState["EditMode"]))
                    {
                        this.chkEvidenza.Enabled = true;
                    }
                    else
                    {
                        this.chkEvidenza.Enabled = false;
                    }
                }

                if (!(schedaDocumento.protocollo.numero != null && !schedaDocumento.protocollo.numero.Equals("")))
                {
                    //this.btn_annulla_P.Enabled = false;
                    this.btn_annulla_P.Visible = false;
                    this.btn_riproponiDati_P.Enabled = false;
                    this.btn_spedisci_P.Enabled = false;
                    this.btn_aggiungi_P.Enabled = false;

                    this.btn_inoltra.Visible = false;
                    this.btn_log.Enabled = false;
                    //AGGIUNTO PER PREDISPONI ALLA PROTOCOLLAZIONE
                    if (UserManager.ruoloIsAutorized(this, "DO_PROT_SALVA"))
                    {
                        if (schedaDocumento.predisponiProtocollazione == true)
                        {
                            this.btn_salva_P.Enabled = true;

                            if (schedaDocumento.systemId != null)
                            {
                                if (UserManager.ruoloIsAutorized(this, "DO_PRED_ANNULLA"))
                                    this.btn_annullaPred.Enabled = true;
                                else
                                    this.btn_annullaPred.Enabled = false;
                            }

                            //if (schedaDocumento.systemId == null)
                            //    this.btn_salva_P.Enabled = false;//vengo da riproni
                            //else
                            //{
                            //    this.btn_salva_P.Enabled = true;//vengo da predisponi

                            //    if (UserManager.ruoloIsAutorized(this, "DO_PRED_ANNULLA"))
                            //        this.btn_annullaPred.Enabled = true;
                            //    else
                            //        this.btn_annullaPred.Enabled = false;
                            //}
                        }
                        else
                        {
                            if (!IsPostBack && schedaDocumento.protocollo.ModUffRef == true && schedaDocumento.protocollo.daProtocollare != null && schedaDocumento.protocollo.daProtocollare == "1")
                            {
                                this.btn_salva_P.Enabled = true;
                            }
                        }

                        if (Session["catenaDoc"] != null)
                        {
                            if (schedaDocumento != null)
                            {
                                if (schedaDocumento.protocollo != null)
                                {
                                    if (schedaDocumento.protocollo.segnatura != null)
                                    {
                                        if (schedaDocumento.protocollo.segnatura.Equals("") && schedaDocumento.docNumber != null)
                                        {
                                            this.btn_salva_P.Enabled = true;
                                        }
                                    }
                                }

                            }
                        }
                    }

                    //se vengo da ricerca, schedaDocumento.predisponiProtocollazione è false, ma
                    //se sono in questo if allora
                    // se vero che (!(schedaDocumento.protocollo.numero != null && !schedaDocumento.protocollo.numero.Equals("")))
                    // ed è vero !string.IsNullOrEmpty(schedaDocumento.systemId)
                    // allora ho l'oggetto protocollo, ma senza numero, quindi è un predisposto.


                    //if (!string.IsNullOrEmpty(schedaDocumento.systemId))
                    //    this.btn_annullaPred.Visible = true;
                }
                else
                {
                    //this.btn_annulla_P.Enabled = true; 

                    //if ((IsRoleInwardEnabled() && schedaDocumento.tipoProto.Equals("A")) || (IsRoleOutwardEnabled() && schedaDocumento.tipoProto.Equals("P")) || (IsRoleInternalEnabled() && schedaDocumento.tipoProto.Equals("I"))) 
                    //this.btn_riproponiDati_P.Enabled = true;
                    if (!((IsRoleInwardEnabled() && schedaDocumento.tipoProto.Equals("A")) || (IsRoleOutwardEnabled() && schedaDocumento.tipoProto.Equals("P")) || (IsRoleInternalEnabled() && schedaDocumento.tipoProto.Equals("I"))))
                    {
                        this.btn_riproponiDati_P.Enabled = false;
                        this.btn_spedisci_P.Enabled = false;
                    }
                    this.btn_aggiungi_P.Enabled = true;

                    this.btn_visibilita.Enabled = true;
                    this.btn_inoltra.Enabled = true;
                    this.btn_log.Enabled = true;
                }
            }

            if ((schedaDocumento.systemId != null) && (!schedaDocumento.predisponiProtocollazione))
            {
                this.rbl_InOut_P.Enabled = false;
                //				this.txt_cod_uffRef.ReadOnly=true;
                //				this.txt_cod_uffRef.BackColor=Color.WhiteSmoke;
                //				this.txt_desc_uffRef.BackColor=Color.WhiteSmoke;
                //				this.btn_Rubrica_ref.Enabled = false;
                //this.rbl_InOut_P.Items[this.rbl_InOut_P.SelectedIndex].Attributes.Remove("style");
                //this.rbl_InOut_P.Items[this.rbl_InOut_P.SelectedIndex].Attributes["style"] = "BACKGROUND-COLOR: #810d06; font-weight:bold; font-size: 10px; color:#666666; font-family:Verdana"; 

                //this.rbl_InOut_P.Items[this.rbl_InOut_P.SelectedIndex].Attributes["style"] = "BACKGROUND-COLOR: #000000;"; 
            }
            else
                this.rbl_InOut_P.Enabled = true;

            //disabilito il campo descrizione di mittente intermedio
            this.txt_DescMitInt_P.Enabled = false;

            //abilito disabilito pulsante SPEDISCI se il doc è in partenza
            if (schedaDocumento.tipoProto != null && schedaDocumento.tipoProto.Equals("P"))
            {
                if (schedaDocumento.protocollo != null &&
                    schedaDocumento.protocollo.numero != null &&
                    !(schedaDocumento.protocollo.numero.Equals("")) &&
                    (schedaDocumento.protocollo.protocolloAnnullato == null ||
                        schedaDocumento.protocollo.protocolloAnnullato.dataAnnullamento == null ||
                        schedaDocumento.protocollo.protocolloAnnullato.dataAnnullamento.Equals("")
                     )
                   )
                {


                    this.btn_spedisci_P.Enabled = true;
                }
                else
                {
                    this.btn_spedisci_P.Enabled = false;
                }

                if (schedaDocumento.protocollo != null &&
                    schedaDocumento.protocollo.numero != null &&
                    !(schedaDocumento.protocollo.numero.Equals(""))
                   )
                {
                    //notifiche spedizioni
                    //this.btn_notifica.Visible = true;
                    this.btn_notifica_sped.Visible = true;
                    this.btn_notifica_sped_CC.Visible = true;
                }
                else
                {

                    //notifiche spedizioni
                    this.btn_notifica.Visible = false;
                    this.btn_notifica_sped.Visible = false;
                    this.btn_notifica_sped_CC.Visible = false;
                }
            }
            else
            {
                //notifiche spedizioni
                this.btn_notifica.Visible = false;
                this.btn_notifica_sped.Visible = false;
                this.btn_notifica_sped_CC.Visible = false;
            }


            //ultimo controllo sul documento annullato: disabilitare tutto
            //annullamento
            DocsPaWR.ProtocolloAnnullato protAnnull = null;
            if (schedaDocumento.protocollo != null)
                protAnnull = schedaDocumento.protocollo.protocolloAnnullato;

            if (protAnnull != null && protAnnull.dataAnnullamento != null && !protAnnull.Equals(""))
            {
                this.btn_protocolla_P.Enabled = false;
                this.btn_aggiungi_P.Enabled = false;
                this.btn_visibilita.Enabled = false;
                this.btn_inoltra.Enabled = false;
            }

            //gestione funzioni rapide rapida
            //Commentato per nuovo sviluppo estensione delle funzionalità di fascicolazione e trasmissione rapida
            //if ((schedaDocumento.systemId == null || schedaDocumento.systemId.Equals("")))
            //{

            if (UserManager.ruoloIsAutorized(this, "FASC_INS_DOC"))
            {
                this.pnl_fasc_rapida.Visible = true;
            }
            else
            {
                this.pnl_fasc_rapida.Visible = false;
            }

            if (UserManager.ruoloIsAutorized(this, "DO_TRA_NUOVA"))
            {
                this.pnl_trasm_rapida.Visible = true;
            }
            else
            {
                this.pnl_trasm_rapida.Visible = false;
            }
            //}
            //else
            //{
            //    this.pnl_fasc_rapida.Visible = false;
            //    this.pnl_trasm_rapida.Visible = false;
            //}

            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"))
            {

                if (schedaDocumento.tipoProto.Equals("A"))
                {
                    btn_Rubrica_ref.Visible = true;
                }
                else
                {
                    btn_Rubrica_ref.Visible = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void enableEditableFields()
        {

            string valoreChiaveNewRubricaVeloce = utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE");

            //abilito il pulsante modifica e rendo i campi read only
            if ((schedaDocumento.systemId != null) && (!schedaDocumento.predisponiProtocollazione))
            {
                this.btn_modificaOgget_P.Enabled = true;

                DocsPaWR.Registro[] userRegistri = DocsPAWA.UserManager.getListaRegistri(this.Page);

                //Contrllo se il documento arriva per trasmissione extra aoo e non si ha visibilità sul registro
                if (UserManager.isFiltroAooEnabled(this))
                {
                    this.btn_modificaOgget_P.Enabled = UserManager.verifyRegNoAOO(schedaDocumento, userRegistri);
                }

                if (UserManager.isFiltroAooEnabled(this))
                {
                    if (this.btn_annulla_P != null && this.btn_annulla_P.Visible)
                    {
                        this.btn_annulla_P.Visible = UserManager.verifyRegNoAOO(schedaDocumento, userRegistri);
                    }
                }
                else
                {
                    this.btn_annulla_P.Visible = true;
                }

                //this.btn_modificaOgget_P.Enabled = true;
                this.txt_CodMit_P.ReadOnly = true;
                this.txt_DescMit_P.ReadOnly = true;
                this.txt_CodMitInt_P.ReadOnly = true;
                //this.txt_codMittMultiplo.ReadOnly = true;
                //this.txt_descMittMultiplo.ReadOnly = true;

                //MittentiMultipli
                btn_downMittente.Enabled = false;
                btn_upMittente.Enabled = false;
                //btn_insMittMultiplo.Enabled = false;
                btn_CancMittMultiplo.Enabled = false;
                btn_RubrMittMultiplo.Disabled = true;
                //txt_codMittMultiplo.ReadOnly = true;
                //txt_descMittMultiplo.ReadOnly = true;


                if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                {
                    this.rubrica_veloce.Visible = false;
                    if (DocumentManager.isEnableMittentiMultipli())
                    {
                        this.rubrica_veloce_mitt_multi.Visible = false;
                    }
                }

                if (!string.IsNullOrEmpty(valoreChiaveNewRubricaVeloce) && valoreChiaveNewRubricaVeloce.Equals("1"))
                {
                    this.pnl_mittente_veloce.Visible = false;
                    this.pnl_destinatario_veloce.Visible = false;
                }

                if (Convert.ToBoolean(this.ViewState["EditMode"]) || (Session["isDocModificato"] != null && Session["isDocModificato"].ToString().ToUpper() == "TRUE"))
                {
                    //this.txt_oggetto_P.ReadOnly = false;

                    //Prova Andrea De Marco - Mev Gestione Eccezioni - Per ripristino, commentare il codice De Marco
                    if (!string.IsNullOrEmpty(schedaDocumento.interop) && schedaDocumento.interop.ToUpper().Equals("E") && !string.IsNullOrEmpty(schedaDocumento.descMezzoSpedizione) && schedaDocumento.descMezzoSpedizione.Equals("MAIL"))
                    {
                        this.txt_NumProtMit_P.ReadOnly = false;
                        this.txt_DataProtMit_P.ReadOnly = false;
                    }
                    //End De Marco

                    this.ddl_tmpl.Enabled = true;
                    this.imgFasc.Enabled = true;
                    this.txt_CodFascicolo.Enabled = true;
                    this.txt_DescFascicolo.Enabled = true;
                    this.ddl_tipoAtto.Enabled = true;
                    if (UserManager.ruoloIsAutorized(this, "FASC_NUOVO"))
                    {
                        this.imgNewFasc.Enabled = true;
                    }
                    else
                    {
                        this.imgNewFasc.Enabled = false;
                    }
                    //this.txt_DataArrivo_P.ReadOnly = false;
                    this.GetCalendarControl("txt_DataProtMit_P").txt_Data.ReadOnly = false;

                    if (System.Configuration.ConfigurationManager.AppSettings["PROT_DATA_ORA_MODIFICA"] != null
                        && System.Configuration.ConfigurationManager.AppSettings["PROT_DATA_ORA_MODIFICA"] == "1")
                    {
                        //Controllo se utente autorizzato abilito l'ora pervenuto
                        if (UserManager.ruoloIsAutorized(this, "DO_PROT_DATA_ORA_MODIFICA"))
                        {
                            this.txt_OraPervenuto_P.ReadOnly = false;
                            this.GetCalendarControl("txt_DataArrivo_P").txt_Data.ReadOnly = false;
                            this.GetCalendarControl("txt_DataArrivo_P").btn_Cal.Enabled = true;

                        }
                        else
                        {
                            this.txt_OraPervenuto_P.ReadOnly = true;
                            this.GetCalendarControl("txt_DataArrivo_P").txt_Data.ReadOnly = true;
                            //this.GetCalendarControl("txt_DataArrivo_P").btn_Cal.Enabled = false;
                        }
                    }
                    else
                    {
                        this.GetCalendarControl("txt_DataArrivo_P").txt_Data.ReadOnly = false;
                        this.GetCalendarControl("txt_DataArrivo_P").btn_Cal.Enabled = true;
                    }
                    this.btn_storiaData.Enabled = false;
                    this.txt_NumProtMit_P.ReadOnly = false;
                    this.btn_titolario.Enabled = true;
                    if (wws.isEnableRiferimentiMittente())
                    {
                        txt_riferimentoMittente.ReadOnly = false;
                        txt_riferimentoMittente.BackColor = Color.White;
                    }
                }
                else
                {
                    //this.txt_oggetto_P.ReadOnly = true;
                    this.ctrl_oggetto.oggetto_isReadOnly = true;
                    this.ddl_tmpl.Enabled = false;
                    this.imgFasc.Enabled = false;
                    this.txt_CodFascicolo.Enabled = false;
                    this.txt_DescFascicolo.Enabled = false;
                    this.ddl_tipoAtto.Enabled = false;
                    //this.txt_DataArrivo_P.ReadOnly = true;
                    this.GetCalendarControl("txt_DataArrivo_P").txt_Data.ReadOnly = true;
                    this.GetCalendarControl("txt_DataArrivo_P").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_DataProtMit_P").txt_Data.ReadOnly = true;
                    this.txt_OraPervenuto_P.ReadOnly = true;
                    //this.btn_storiaData.Enabled = false;
                    //Prova Andrea De Marco - Mev Gestione Eccezioni - Per ripristino, commentare il codice De Marco
                    //if (!string.IsNullOrEmpty(schedaDocumento.interop) && schedaDocumento.interop.ToUpper().Equals("E") && !string.IsNullOrEmpty(schedaDocumento.descMezzoSpedizione) && schedaDocumento.descMezzoSpedizione.Equals("MAIL"))
                    //{
                    //    this.txt_NumProtMit_P.ReadOnly = false;
                    //    this.txt_DataProtMit_P.ReadOnly = false;
                    //}
                    //else
                    //    this.txt_NumProtMit_P.ReadOnly = true;
                    //End De Marco
                    this.txt_NumProtMit_P.ReadOnly = true;
                    this.btn_titolario.Enabled = false;
                    if (wws.isEnableRiferimentiMittente())
                    {
                        txt_riferimentoMittente.ReadOnly = true;
                        txt_riferimentoMittente.BackColor = Color.Gainsboro;
                    }
                    if (this.NotaDocumentoEnabled) this.dettaglioNota.Enabled = false;
                    if (UserManager.ruoloIsAutorized(this, "DO_PROT_DATA_ORA_STORIA"))
                    {
                        this.btn_storiaData.Enabled = true;
                    }
                    else this.btn_storiaData.Enabled = false;
                }

                this.txt_CodDest_P.ReadOnly = true;
                this.txt_DescDest_P.ReadOnly = true;

                if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                {
                    this.rubrica_veloce_destinatario.Visible = false;
                }

                if (!string.IsNullOrEmpty(valoreChiaveNewRubricaVeloce) && valoreChiaveNewRubricaVeloce.Equals("1"))
                {
                    this.pnl_destinatario_veloce.Visible = false;
                }

                this.btn_cancDest.Enabled = false;
                this.btn_cancDestCC.Enabled = false;
                this.btn_insDest.Enabled = false;
                this.btn_insDestCC.Enabled = false;
                this.btn_StampaBuste_P.Enabled = true;

                btn_RubrOgget_P_state = false;
                btn_rubrica_p_sempl_state = false;
                btn_rubrica_p_state = false;
                btn_RubrMitInt_Sempl_state = false;
                btn_RubrMitInt_state = false;
                btn_RubrDest_Sempl_P_state = false;
                btn_RubrDest_P_state = false;

                addScriptChangeIcon(btn_modificaOgget_P);
                if (Session["isDocModificato"] != null && Session["isDocModificato"].ToString().ToUpper() == "TRUE")
                {
                    if (UserManager.ruoloIsAutorized(this, "DO_PROT_MIT_MODIFICA"))
                    {
                        this.txt_CodMit_P.ReadOnly = false;
                        //this.txt_codMittMultiplo.ReadOnly = false;
                        if (schedaDocumento.tipoProto.Equals("A"))
                        {
                            this.txt_DescMit_P.ReadOnly = false;
                            //this.txt_descMittMultiplo.ReadOnly = false;
                            btn_downMittente.Enabled = true;
                            btn_upMittente.Enabled = true;
                            //btn_insMittMultiplo.Enabled = true;
                            btn_CancMittMultiplo.Enabled = true;
                            btn_RubrMittMultiplo.Disabled = false;
                            if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                            {
                                if (DocumentManager.isEnableMittentiMultipli())
                                {
                                    this.rubrica_veloce_mitt_multi.Visible = true;
                                }
                            }
                        }
                        else
                        {
                            this.txt_DescMit_P.ReadOnly = true;
                            //this.txt_descMittMultiplo.ReadOnly = true;
                            btn_downMittente.Enabled = false;
                            btn_upMittente.Enabled = false;
                            //btn_insMittMultiplo.Enabled = false;
                            btn_CancMittMultiplo.Enabled = false;
                            btn_RubrMittMultiplo.Disabled = true;
                        }
                        this.btn_rubrica_p_state = true;
                        this.btn_rubrica_p_sempl_state = true;
                        this.m_modifica = true;
                        this.btn_salva_P.Enabled = true;
                        if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                        {
                            this.rubrica_veloce.Visible = true;
                        }

                        if (!string.IsNullOrEmpty(valoreChiaveNewRubricaVeloce) && valoreChiaveNewRubricaVeloce.Equals("1"))
                        {
                            this.pnl_mittente_veloce.Visible = true;
                        }
                    }

                    if (UserManager.ruoloIsAutorized(this, "DO_PROT_MIT_INT_MODIFICA"))
                    {
                        this.txt_CodMitInt_P.ReadOnly = false;
                        this.btn_RubrMitInt_state = true;
                        this.btn_RubrMitInt_Sempl_state = true;
                        this.m_modifica = true;
                        this.btn_salva_P.Enabled = true;
                        if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                        {
                            this.rubrica_veloce.Visible = true;
                        }

                        if (!string.IsNullOrEmpty(valoreChiaveNewRubricaVeloce) && valoreChiaveNewRubricaVeloce.Equals("1"))
                        {
                            this.pnl_mittente_veloce.Visible = true;
                        }
                    }

                    if (UserManager.ruoloIsAutorized(this, "DO_PROT_DEST_MODIFICA") && UserManager.ruoloIsAutorized(this, "DO_PROT_DESTCC_MODIFICA"))
                    {
                        this.btn_cancDest.Enabled = true;
                        this.btn_cancDestCC.Enabled = true;
                        this.btn_insDest.Enabled = true;
                        this.btn_insDestCC.Enabled = true;
                        this.txt_CodDest_P.ReadOnly = false;
                        this.txt_DescDest_P.ReadOnly = false;
                        this.btn_RubrDest_P_state = true;
                        this.btn_RubrDest_Sempl_P_state = true;
                        this.m_modifica = true;
                        if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                        {
                            this.rubrica_veloce_destinatario.Visible = true;
                        }
                        if (!string.IsNullOrEmpty(valoreChiaveNewRubricaVeloce) && valoreChiaveNewRubricaVeloce.Equals("1"))
                        {
                            this.pnl_destinatario_veloce.Visible = true;
                        }
                    }
                    /*if (UserManager.ruoloIsAutorized(this, "DO_PROT_DATA_ORA_MODIFICA"))
                    {
                        this.txt_OraPervenuto_P.ReadOnly = false;
                    } */

                }
            }
            else
            {
                this.btn_modificaOgget_P.Enabled = false;
                this.txt_CodMit_P.ReadOnly = false;
                //this.txt_codMittMultiplo.ReadOnly = false;

                if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                {
                    this.rubrica_veloce.Visible = true;
                }

                if (!string.IsNullOrEmpty(valoreChiaveNewRubricaVeloce) && valoreChiaveNewRubricaVeloce.Equals("1"))
                {
                    this.pnl_mittente_veloce.Visible = true;
                }

                if (schedaDocumento.tipoProto == "A")
                {
                    //Andrea De Marco - Mev Gestione Eccezioni PEC - per ripristino commentare De Marco
                    if (!string.IsNullOrEmpty(schedaDocumento.interop) && schedaDocumento.interop.ToUpper().Equals("E") && !string.IsNullOrEmpty(schedaDocumento.descMezzoSpedizione) && schedaDocumento.descMezzoSpedizione.Equals("MAIL"))
                    {
                        this.txt_NumProtMit_P.ReadOnly = false;
                        this.txt_DataProtMit_P.ReadOnly = false;
                    }
                    //End Andrea De Marco
                    this.txt_DescMit_P.ReadOnly = false;
                    //this.txt_descMittMultiplo.ReadOnly = false;
                    btn_downMittente.Enabled = true;
                    btn_upMittente.Enabled = true;
                    //btn_insMittMultiplo.Enabled = true;
                    btn_CancMittMultiplo.Enabled = true;
                    btn_RubrMittMultiplo.Disabled = false;
                    if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                    {
                        if (DocumentManager.isEnableMittentiMultipli())
                        {
                            this.rubrica_veloce_mitt_multi.Visible = true;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(valoreChiaveNewRubricaVeloce) && valoreChiaveNewRubricaVeloce.Equals("1"))
                    {
                        this.txt_DescMit_P.ReadOnly = false;
                    }
                    else
                    {
                        this.txt_DescMit_P.ReadOnly = true;
                    }
                    //this.txt_descMittMultiplo.ReadOnly = true;
                    btn_downMittente.Enabled = false;
                    btn_upMittente.Enabled = false;
                    //btn_insMittMultiplo.Enabled = false;
                    btn_CancMittMultiplo.Enabled = false;
                    btn_RubrMittMultiplo.Disabled = true;
                }
                if (schedaDocumento.tipoProto == "I")
                {
                    if (string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) || !(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                    {
                        this.txt_DescDest_P.ReadOnly = true;
                    }
                }


                this.txt_CodMitInt_P.ReadOnly = false;
                //this.txt_oggetto_P.ReadOnly = false;
                this.ctrl_oggetto.oggetto_isReadOnly = false;
                this.ddl_tmpl.Enabled = true;
                this.imgFasc.Enabled = true;
                this.txt_CodFascicolo.Enabled = true;
                this.txt_DescFascicolo.Enabled = true;
                this.ddl_tipoAtto.Enabled = true;
                this.txt_CodDest_P.ReadOnly = false;
                //this.txt_DescDest_P.ReadOnly = false;
                //this.txt_DataArrivo_P.ReadOnly = false;
                this.txt_OraPervenuto_P.ReadOnly = false;
                if (UserManager.ruoloIsAutorized(this, "FASC_NUOVO"))
                {
                    this.imgNewFasc.Enabled = true;
                }
                else
                {
                    this.imgNewFasc.Enabled = false;
                }
                this.GetCalendarControl("txt_DataArrivo_P").txt_Data.ReadOnly = false;
                if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                {
                    this.rubrica_veloce_destinatario.Visible = true;
                }
                if (!string.IsNullOrEmpty(valoreChiaveNewRubricaVeloce) && valoreChiaveNewRubricaVeloce.Equals("1"))
                {
                    this.pnl_destinatario_veloce.Visible = true;
                }

                /*if (UserManager.ruoloIsAutorized(this, "DO_PROT_DATA_ORA_MODIFICA"))
                {
                    this.txt_OraPervenuto_P.ReadOnly = false;
                    this.GetCalendarControl("txt_DataArrivo_P").txt_Data.ReadOnly = false;
                }
                else
                {
                    this.txt_OraPervenuto_P.ReadOnly = true;
                    this.GetCalendarControl("txt_DataArrivo_P").txt_Data.ReadOnly = true;
                }*/
                this.btn_storiaData.Enabled = false;
                this.btn_cancDest.Enabled = true;
                this.btn_cancDestCC.Enabled = true;
                this.btn_insDest.Enabled = true;
                this.btn_insDestCC.Enabled = true;
                this.btn_StampaBuste_P.Enabled = false;

                btn_RubrOgget_P_state = true;
                btn_rubrica_p_sempl_state = true;
                btn_rubrica_p_state = true;
                btn_RubrMitInt_Sempl_state = true;
                btn_RubrMitInt_state = true;
                btn_RubrDest_Sempl_P_state = true;
                btn_RubrDest_P_state = true;
                btn_titolario.Enabled = true;
            }

            if (schedaDocumento.systemId != null)
            {
                this.btn_storiaOgg_P.Enabled = true;
                this.btn_StoriaMitt.Enabled = true;
                this.btn_StoriaDest.Enabled = true;
                //this.btn_storiaData.Enabled = true;
            }
            else
            {
                this.btn_storiaOgg_P.Enabled = false;
                this.btn_StoriaMitt.Enabled = false;
                this.btn_StoriaDest.Enabled = false;
                //this.btn_storiaData.Enabled = false;
            }
            //Se si possiedono solo i diritti di lettura non si 
            //può modificare la tipologia
            if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.accessRights) && schedaDocumento.accessRights.Equals("45"))
                this.ddl_tipoAtto.Enabled = false;

            //Se ricevuto per interoperabilità con pec questi campi non sono modificabili se valorizzati
            if (this.FromInteropPecOrSimpInterop(schedaDocumento))
            {
                if (string.IsNullOrEmpty(schedaDocumento.protocollo.segnatura))
                {
                    txt_CodMit_P.ReadOnly = Convert.ToBoolean(ViewState["varFromInteropPecMit"]);
                    txt_DescMit_P.ReadOnly = Convert.ToBoolean(ViewState["varFromInteropPecMit"]);
                    this.btn_rubrica_p.Disabled = Convert.ToBoolean(ViewState["varFromInteropPecMit"]);
                }

                if (!string.IsNullOrEmpty(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).descrizioneProtocolloMittente))
                {
                    this.txt_NumProtMit_P.ReadOnly = Convert.ToBoolean(ViewState["varFromInteropPecProtMitt"]);
                }

                if (!string.IsNullOrEmpty(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).dataProtocolloMittente))
                {
                    bool valRes = Convert.ToBoolean(ViewState["varFromInteropPecDataMitt"]);
                    this.GetCalendarControl("txt_DataProtMit_P").txt_Data.ReadOnly = valRes;
                    this.GetCalendarControl("txt_DataProtMit_P").btn_Cal.Enabled = !valRes;
                }

                if (!string.IsNullOrEmpty(this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text))
                {
                    bool valRes = Convert.ToBoolean(ViewState["varFromInteropPecDataArrivo"]);
                    this.GetCalendarControl("txt_DataArrivo_P").txt_Data.ReadOnly = valRes;
                    this.GetCalendarControl("txt_DataArrivo_P").btn_Cal.Enabled = !valRes;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        private void addScriptChangeIcon(System.Web.UI.WebControls.ImageButton button)
        {
            string l_buttonId = button.ID.ToString();
            string l_script = getScriptChangeIcon(l_buttonId);

            button.Attributes.Add("onmouseover", l_script);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buttonId"></param>
        /// <returns></returns>
        private string getScriptChangeIcon(string buttonId)
        {
            string retValue = "ChangeCursorT('hand','" + buttonId + "')";

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void setFormProperties()
        {
            //per aprire il popup di area lavoro devo passare un parametro che è l'opposto della tipologia del documento
            // doc arrivo   tipoADL = P
            // doc partenza tipoADL = A
            //string tipoADL = "A";			
            //if(schedaDocumento.tipoProto.Equals("A")) tipoADL = "P";

            this.btn_RubrOgget_P.Attributes.Add("onclick", "ApriOggettario('proto');return false");

            //this.btn_adl_P.Attributes.Add       ("onclick", "ApriFinestraADL('../popup/areaDiLavoro.aspx?action=setDoc&tipoDoc=" + tipoADL + "');return true");
            this.btn_annulla_P.Attributes.Add("onclick", "ApriFinestraAnnullaProto();");
            this.btn_DetMit_P.Attributes.Add("onclick", "ApriFinestraCor('../popup/dettagliCorrispondenti.aspx?tipoCor=M',document.docProtocollo.txt_CodMit_P.value + document.docProtocollo.txt_DescMit_P.value);return false");
            this.btn_DetMitInt_P.Attributes.Add("onclick", "ApriFinestraCor('../popup/dettagliCorrispondenti.aspx?tipoCor=I',document.docProtocollo.txt_CodMitInt_P.value + document.docProtocollo.txt_DescMitInt_P.value);return false");
            this.btn_dettDest.Attributes.Add("onclick", "ApriFinestraCor('../popup/dettagliCorrispondenti.aspx?tipoCor=D',document.docProtocollo.lbx_dest.selectedIndex);return false");
            this.btn_dettDestCC.Attributes.Add("onclick", "ApriFinestraCor('../popup/dettagliCorrispondenti.aspx?tipoCor=C',document.docProtocollo.lbx_destCC.selectedIndex);return false");
            this.btn_storiaOgg_P.Attributes.Add("onclick", "ApriFinestraStoriaMod('oggetto');return false");
            ////this.btn_tipo_sped.Attributes.Add("onclick", "ApriFinestraSceltaTipoSped(document.docProtocollo.lbx_dest.selectedIndex,'D','" + this.ViewState["EditMode"].ToString().ToLower() + "');return false");
            //this.btn_tipo_sped.Attributes.Add("onclick", "ApriFinestraSceltaTipoSped(document.docProtocollo.lbx_dest.selectedIndex,'D','true');return false");
            ////this.btn_tipo_spedCC.Attributes.Add("onclick", "ApriFinestraSceltaTipoSped(document.docProtocollo.lbx_destCC.selectedIndex,'CC','" + this.ViewState["EditMode"].ToString().ToLower() + "');return false");
            //this.btn_tipo_spedCC.Attributes.Add("onclick", "ApriFinestraSceltaTipoSped(document.docProtocollo.lbx_destCC.selectedIndex,'CC','true');return false");
            this.btn_StoriaMitt.Attributes.Add("onclick", "ApriFinestraStoriaMod('mit');");
            //this.btn_storiaMitInt_P.Attributes.Add("onclick", "ApriFinestraStoriaMittentiIntermedi('mitInt');");
            this.btn_StoriaDest.Attributes.Add("onclick", "ApriFinestraStoriaMod('dest');");
            this.btn_storiaData.Attributes.Add("onclick", "ApriFinestraStoriaMod('data');");
            this.btn_visibilita.Attributes.Add("onclick", "ApriFinestraVisibilita();return false;");
            this.btn_notifica_sped.Attributes.Add("onclick", "ApriFinestraNotificaCor('../popup/notificheDocumento.aspx?tipoCor=D',document.docProtocollo.lbx_dest.selectedIndex);document.docProtocollo.lbx_dest.selectedIndex=-1;return false");
            this.btn_notifica_sped_CC.Attributes.Add("onclick", "ApriFinestraNotificaCor('../popup/notificheDocumento.aspx?tipoCor=C',document.docProtocollo.lbx_destCC.selectedIndex);document.docProtocollo.lbx_destCC.selectedIndex=-1;return false");
            this.btn_notifica.Attributes.Add("onclick", "ApriFinestraNotificaCor('../popup/notificheDocumento.aspx?tipoCor=T',null);return false");
            this.btn_el_spediz.Attributes.Add("onclick", "ApriFinestraSpedizioni('../popup/listaSpedizioni.aspx');return false");
            //this.btn_visibilita.Enabled = true;
            this.btn_StoriaMittMultipli.Attributes.Add("onclick", "ApriFinestraStoriaMod('mitMultipli');");
            this.btn_dettMittMultipli.Attributes.Add("onclick", "ApriFinestraDettMittMultipli('../popup/dettagliCorrispondenti.aspx?tipoCor=MD',document.docProtocollo.lbx_mittMultiplo.selectedIndex);return false");
            //if (schedaDocumento != null && schedaDocumento.protocollo != null
            //    && schedaDocumento.protocollo.daProtocollare != null && schedaDocumento.protocollo.daProtocollare.Equals("1"))
            //{
            //    this.btn_visibilita.Enabled = true;
            //}

            //visualizza l'appropriato bottone per la rubrica			
            if (UserManager.getStringaConfigurazione(this.Page, true).Equals("1"))
            {
                this.pnl_rubr_dest_Semplice.Visible = true;
                this.pnl_mit_int_semplice.Visible = true;
                this.pnl_mit_sempl.Visible = true;
                this.pnl_rubr_des.Visible = false;
                this.pnl_mit.Visible = false;
                this.pnl_mit_int.Visible = false;
            }

            if (schedaDocumento.protocollo != null)
            {
                if (!String.IsNullOrEmpty(schedaDocumento.protocollo.segnatura))
                    this.chkPrivato.Enabled = false;
                else
                    // Se il documento è predisposto alla protocollazione ma è stato
                    // ricevuto per interoperabilità semplificata ed è pendente, deve
                    // essere possibile marcarlo o meno come privato
                    if (schedaDocumento != null && !String.IsNullOrEmpty(schedaDocumento.systemId) &&
                        InteroperabilitaSemplificataManager.IsDocumentReceivedWithIS(schedaDocumento.systemId))
                    {
                        this.chkPrivato.Enabled = schedaDocumento.protocollo != null &&
                            schedaDocumento.protocollo is ProtocolloEntrata &&
                            String.IsNullOrEmpty(schedaDocumento.protocollo.numero) &&
                            Convert.ToBoolean(this.ViewState["EditMode"]) &&
                            InteroperabilitaSemplificataManager.IsDocumentReceivedWithIS(schedaDocumento.systemId) &&
                            InteroperabilitaSemplificataManager.EnablePrivateCheck(
                                schedaDocumento.protocollo,
                                schedaDocumento.typeId,
                                schedaDocumento.registro.systemId);
                    }
                    else
                        this.chkPrivato.Enabled = String.IsNullOrEmpty(schedaDocumento.systemId);

                        
            }

            //abilito o disabilito i bottoni
            if (schedaDocumento.inCestino == "1")
            {
                this.btn_visibilita.Enabled = false;
                this.btn_RubrOgget_P.Enabled = false;
                this.btn_modificaOgget_P.Enabled = false;
                this.btn_storiaOgg_P.Enabled = false;
            }
        }

        /// <summary>
        /// Il tipo protocollazione selezionato viene validato in base ai permessi del ruolo 
        /// relativo all'utente (l'utente puo' essere abilitato ad uno o piu' tipi di protocollazione).
        /// Se la scelta non e' valida viene notificato l'errore e selezionato il primo tipo di
        /// protocollazione valido per l'utente in questione.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbl_InOut_P_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                //Per caso riproponi con copia:
                Session.Add("scDocCopiato", schedaDocumento);
                if (Session["scDocCopiato"] != null)
                {
                    DocsPAWA.DocsPaWR.SchedaDocumento schDocCopiato = (DocsPAWA.DocsPaWR.SchedaDocumento)Session["scDocCopiato"];
                    if (schedaDocumento.docNumber != schDocCopiato.docNumber)
                    {
                        schedaDocumento = DocumentManager.annullaPredisposizione(UserManager.getInfoUtente(), schedaDocumento);
                        if (schedaDocumento != null)
                        {
                            schedaDocumento.predisponiProtocollazione = true;
                            DocumentManager.setDocumentoSelezionato(schedaDocumento);
                        }
                    }

                }


                FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                this.txt_CodFascicolo.Text = "";
                this.txt_DescFascicolo.Text = "";

                DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];


                DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                bool protoInterno = ws.IsInternalProtocolEnabled(cr.idAmministrazione);

                //per l'ufficio referente
                this.txt_cod_uffRef.ReadOnly = true;
                //				this.txt_desc_uffRef.ReadOnly = true;
                this.txt_cod_uffRef.BackColor = Color.WhiteSmoke;
                this.txt_desc_uffRef.BackColor = Color.WhiteSmoke;
                this.btn_Rubrica_ref.Enabled = false;

                switch (this.rbl_InOut_P.SelectedItem.Value)
                {
                    case "In":
                        if (this.IsRoleInwardEnabled() || this.IsRolePredisponiEnabled())
                        {
                            this.SetAsProtoIn();
                            this.txt_CodMit_P.Text = "";
                            this.txt_DescMit_P.Text = "";
                            this.hiddenIdCodMit_p.Value = "";
                            //this.GetCorrispondenteControl("CorrDaCodMit").clean();
                            //this.GetCorrispondenteControl("CorrDaCodMit").TIPO_PROTO = "A";
                            //this.GetCorrispondenteControl("CorrDaCodMitInt").clean();
                            //this.GetCorrispondenteControl("CorrDaCodMitInt").TIPO_PROTO = "A";
                            //per i campi relativi all'ufficio referente
                            this.txt_cod_uffRef.Text = "";
                            this.txt_desc_uffRef.Text = "";
                            this.val_proto_sele = "proto";
                        }
                        else
                        {
                            // Alert utente disabilitato alla protocollazione in ingresso
                            Response.Write("<script>alert('Utente non abilitato alla protocollazione in ingresso.');</script>");

                            if (this.IsRoleOutwardEnabled() || this.IsRolePredisponiEnabled())
                            {
                                this.SetAsProtoOut(); // Default to 'Uscita'
                                this.txt_CodMit_P.Text = "";
                                this.txt_DescMit_P.Text = "";
                                this.val_proto_sele = "proto";
                                this.hiddenIdCodMit_p.Value = "";
                                //this.GetCorrispondenteControl("CorrDaCodMit").clean();
                                //this.GetCorrispondenteControl("CorrDaCodMit").TIPO_PROTO = "A";
                                //this.GetCorrispondenteControl("CorrDaCodMitInt").clean();
                                //this.GetCorrispondenteControl("CorrDaCodMitInt").TIPO_PROTO = "A";
                                /* SOLA LETTURA per codice e descrizione ufficio referente */
                                if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                                {
                                    this.txt_cod_uffRef.ReadOnly = true;
                                    //									this.txt_desc_uffRef.ReadOnly = true;
                                    this.txt_cod_uffRef.BackColor = Color.WhiteSmoke;
                                    this.txt_desc_uffRef.BackColor = Color.WhiteSmoke;
                                    this.btn_Rubrica_ref.Enabled = false;
                                    //ufficio referente di default è la UO del ruolo che protocolla

                                    if (schedaDocumento.tipoProto.Equals("P"))
                                    {
                                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente = UserManager.getRuolo(this).uo;
                                    }
                                }
                                //per i campi relativi all'ufficio referente
                                //this.txt_cod_uffRef.Text = "";
                                //this.txt_desc_uffRef.Text = "";


                            }
                            else if ((this.IsRoleInternalEnabled() && protoInterno) || this.IsRolePredisponiEnabled())
                            {
                                this.SetAsProtoOwn(); // Default to 'Interno'
                                this.txt_CodMit_P.Text = "";
                                this.txt_DescDest_P.Text = "";
                                this.txt_DescMit_P.Text = "";
                                this.hiddenIdCodMit_p.Value = "";
                                //per i campi relativi all'ufficio referente
                                this.txt_cod_uffRef.Text = "";
                                this.txt_desc_uffRef.Text = "";
                                this.val_proto_sele = "protoInt";
                                //this.GetCorrispondenteControl("CorrDaCodMit").clean();
                                //this.GetCorrispondenteControl("CorrDaCodMit").TIPO_PROTO = "A";
                                //this.GetCorrispondenteControl("CorrDaCodMitInt").clean();
                                //this.GetCorrispondenteControl("CorrDaCodMitInt").TIPO_PROTO = "A";
                            }
                            else
                            {
                                /* Questa eventualita' puo' verificarsi solo a seguito di un eventuale bug
                                 * dell'applicazione che permettesse ad un utente non abilitato alla protocollazione
                                 * di visualizzare comunque questa form.
                                 */
                                throw new Exception("L'utente non e' abilitato alla protocollazione.");
                            }
                            mittType = "mitt";
                        }
                        if (ConfigSettings.getKey(ConfigSettings.KeysENUM.MEZZO_SPEDIZIONE) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.MEZZO_SPEDIZIONE).ToUpper().Equals("1"))
                        {
                            //this.pnl_mezzoSpedizione.Visible = true;
                            //this.ddl_spedizione.SelectedItem.Value = "";
                            //this.ddl_spedizione.SelectedItem.Text = "";
                            this.pnl_mezzoSpedizione.Visible = true;
                            CaricaComboMezzoSpedizione(this.ddl_spedizione);
                        }
                        else
                            this.pnl_mezzoSpedizione.Visible = false;


                        if (Session["docRiproposto"] != null)
                            Session.Remove("docRiproposto");

                        break;
                    case "Out":
                        if (this.IsRoleOutwardEnabled())
                        {
                            this.SetAsProtoOut();
                            this.txt_CodMit_P.Text = "";
                            this.txt_DescMit_P.Text = "";
                            this.hiddenIdCodMit_p.Value = "";
                            //this.GetCorrispondenteControl("CorrDaCodMit").clean();
                            //this.GetCorrispondenteControl("CorrDaCodMit").TIPO_PROTO = "P";
                            val_proto_sele = "proto";
                            SettaMittenteDefault("P");
                            /* SOLA LETTURA per codice e descrizione ufficio referente */
                            if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                            {
                                this.txt_cod_uffRef.ReadOnly = true;
                                //								this.txt_desc_uffRef.ReadOnly = true;
                                this.txt_cod_uffRef.BackColor = Color.WhiteSmoke;
                                this.txt_desc_uffRef.BackColor = Color.WhiteSmoke;
                                this.btn_Rubrica_ref.Enabled = false;
                                //ufficio referente di default è la UO del ruolo che protocolla
                                if (schedaDocumento.tipoProto.Equals("P"))
                                {
                                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente = UserManager.getRuolo(this).uo;
                                }
                            }
                            //per i campi relativi all'ufficio referente
                            //this.txt_cod_uffRef.Text = "";
                            //this.txt_desc_uffRef.Text = "";
                        }
                        else
                        {
                            // Alert utente disabilitato alla protocollazione in uscita
                            Response.Write("<script>alert('Utente non abilitato alla protocollazione in uscita.');</script>");

                            if (this.IsRoleInwardEnabled())
                            {
                                this.SetAsProtoIn(); // Default to 'Ingresso'
                                this.txt_CodMit_P.Text = "";
                                this.txt_DescMit_P.Text = "";
                                this.hiddenIdCodMit_p.Value = "";
                                val_proto_sele = "proto";
                                //per i campi relativi all'ufficio referente
                                this.txt_cod_uffRef.Text = "";
                                this.txt_desc_uffRef.Text = "";
                            }
                            else if (this.IsRoleInternalEnabled() && protoInterno)
                            {
                                this.SetAsProtoOwn(); // Default to 'Interno'
                                this.txt_CodMit_P.Text = "";
                                this.txt_DescDest_P.Text = "";
                                this.txt_DescMit_P.Text = "";
                                this.hiddenIdCodMit_p.Value = "";
                                //per i campi relativi all'ufficio referente
                                this.txt_cod_uffRef.Text = "";
                                this.txt_desc_uffRef.Text = "";
                                val_proto_sele = "protoInt";
                            }
                            else
                            {
                                /* Questa eventualita' puo' verificarsi solo a seguito di un eventuale bug
                                 * dell'applicazione che permettesse ad un utente non abilitato alla protocollazione
                                 * di visualizzare comunque questa form.
                                 */
                                throw new Exception("L'utente non e' abilitato alla protocollazione.");
                            }
                        }
                        mittType = "dest";
                        break;
                    case "Own":
                        if (this.IsRoleInternalEnabled() && protoInterno)
                        {
                            this.SetAsProtoOwn();
                            this.txt_CodMit_P.Text = "";
                            this.txt_DescDest_P.Text = "";
                            this.txt_DescMit_P.Text = "";
                            this.hiddenIdCodMit_p.Value = "";
                            //per i campi relativi all'ufficio referente
                            this.txt_cod_uffRef.Text = "";
                            this.txt_desc_uffRef.Text = "";
                            this.val_proto_sele = "protoInt";
                            SettaMittenteDefault("I");
                        }
                        else
                        {
                            // Alert utente disabilitato alla protocollazione interna
                            Response.Write("<script>alert('Utente non abilitato alla protocollazione interna.');</script>");

                            if (this.IsRoleInwardEnabled())
                            {
                                this.SetAsProtoIn(); // Default to 'Ingresso'
                                this.txt_CodMit_P.Text = "";
                                this.txt_DescMit_P.Text = "";
                                this.hiddenIdCodMit_p.Value = "";
                                //per i campi relativi all'ufficio referente
                                this.txt_cod_uffRef.Text = "";
                                this.txt_desc_uffRef.Text = "";
                                val_proto_sele = "proto";
                            }
                            else if (this.IsRoleOutwardEnabled())
                            {
                                this.SetAsProtoOut(); // Default to 'Uscita'
                                this.txt_CodMit_P.Text = "";
                                this.txt_DescMit_P.Text = "";
                                this.hiddenIdCodMit_p.Value = "";
                                val_proto_sele = "proto";
                                /* SOLA LETTURA per codice e descrizione ufficio referente */
                                if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                                {
                                    this.txt_cod_uffRef.ReadOnly = true;
                                    //									this.txt_desc_uffRef.ReadOnly = true;
                                    this.txt_cod_uffRef.BackColor = Color.WhiteSmoke;
                                    this.txt_desc_uffRef.BackColor = Color.WhiteSmoke;
                                    this.btn_Rubrica_ref.Enabled = false;
                                    //ufficio referente di default è la UO del ruolo che protocolla
                                    if (schedaDocumento.tipoProto.Equals("P"))
                                    {
                                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente = UserManager.getRuolo(this).uo;
                                    }
                                }
                                //per i campi relativi all'ufficio referente
                                //this.txt_cod_uffRef.Text = "";
                                //this.txt_desc_uffRef.Text = "";
                            }
                            else
                            {
                                /* Questa eventualita' puo' verificarsi solo a seguito di un eventuale bug
                                 * dell'applicazione che permettesse ad un utente non abilitato alla protocollazione
                                 * di visualizzare comunque questa form.
                                 */
                                throw new Exception("L'utente non e' abilitato alla protocollazione.");
                            }
                        }
                        mittType = "mittInterno";
                        break;
                }

                // l'istruzione che segue è stata spostata in "docProtocollo_PreRender"
                //this.hd_tipoProtocollazione.Value = this.rbl_InOut_P.SelectedItem.Value;			


                Session.Remove("scDocCopiato");
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }
        }

        //elisa
        private void SettaMittenteDefault(string tipoProto)
        {
            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.MITTENTE_DEFAULT) != null)
            {
                string mittenteDefault = ConfigSettings.getKey(ConfigSettings.KeysENUM.MITTENTE_DEFAULT).ToLower();
                // luluciani errore in ENAC se impostato mitt default su web.config, anche se lo cerco e cambio da rubrica o da codice quando 
                //protocollo e refresh ritorna il dafault.
                //OLD	if(mittenteDefault=="1" )
                DocsPaWR.Corrispondente corrMitt = null;

                if (schedaDocumento != null && schedaDocumento.protocollo != null)
                {
                    if (tipoProto.Equals("I"))
                        corrMitt = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente;
                    if (tipoProto.Equals("P"))
                        corrMitt = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente;
                }
                string descrizione = txt_DescMit_P.Text.Trim();


                if (mittenteDefault == "1" && !(corrMitt != null && corrMitt.descrizione != null && corrMitt.descrizione != ""))
                {
                    if (UserManager.getRuolo(this) != null)
                    {
                        DocsPaWR.Ruolo ruo = UserManager.getRuolo(this);
                        DocsPaWR.Corrispondente corr = ruo.uo;
                        if (corr != null)
                        {
                            if (tipoProto.Equals("I"))
                            {
                                ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente = corr;
                                if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                                {
                                    ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).ufficioReferente = corr;
                                }
                            }
                            else if (tipoProto.Equals("P"))
                            {
                                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente = corr;
                                if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                                {
                                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente = corr;
                                }
                            }

                            DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetAsProtoIn()
        {
            this.setProtoArrivo();
            this.txt_DescMit_P.ReadOnly = false;
            this.btn_aggiungi_P.Visible = true;
            this.btn_spedisci_P.Visible = false;
            this.val_proto_sele = "proto";
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetAsProtoOut()
        {
            this.setProtoPartenza();
            if (!string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
            {
                this.txt_DescMit_P.ReadOnly = false;
            }
            else
            {
                this.txt_DescMit_P.ReadOnly = true;
            }
            this.txt_DescDest_P.ReadOnly = false;
            this.btn_aggiungi_P.Visible = false;
            this.btn_spedisci_P.Visible = true;
            this.val_proto_sele = "proto";
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetAsProtoOwn()
        {
            this.setProtoInterno();
            if (!string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
            {
                this.txt_DescMit_P.ReadOnly = false;
                this.txt_DescDest_P.ReadOnly = false;
            }
            else
            {
                this.txt_DescMit_P.ReadOnly = true;
                this.txt_DescDest_P.ReadOnly = true;
            }
            this.btn_aggiungi_P.Visible = false;
            this.btn_spedisci_P.Visible = true;
            this.val_proto_sele = "protoInt";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsRoleInternalEnabled()
        {
            bool result = false;

            if (UserManager.getRuolo(this) != null)
            {
                DocsPaWR.Ruolo ruo = UserManager.getRuolo(this);

                for (int i = 0; i < ruo.funzioni.Length; i++)
                {
                    if (ruo.funzioni[i].descrizione == "PROTO_OWN")
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        private bool IsRolePredisponiEnabled()
        {
            bool result = false;

            return result = UserManager.ruoloIsAutorized(this, "DO_PRO_PREDISPONI") &&
                       UserManager.ruoloIsAutorized(this, "DO_PRO_SALVA");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsRoleOutwardEnabled()
        {
            bool result = false;

            if (UserManager.getRuolo(this) != null)
            {
                DocsPaWR.Ruolo ruo = UserManager.getRuolo(this);

                for (int i = 0; i < ruo.funzioni.Length; i++)
                {
                    if (ruo.funzioni[i].descrizione == "PROTO_OUT")
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsRoleInwardEnabled()
        {
            bool result = false;

            if (UserManager.getRuolo(this) != null)
            {
                DocsPaWR.Ruolo ruo = UserManager.getRuolo(this);

                for (int i = 0; i < ruo.funzioni.Length; i++)
                {
                    if (ruo.funzioni[i].descrizione == "PROTO_IN")
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsRoleGrigiEnabled()
        {
            bool result = false;

            if (UserManager.getRuolo(this) != null)
            {
                DocsPaWR.Ruolo ruo = UserManager.getRuolo(this);

                for (int i = 0; i < ruo.funzioni.Length; i++)
                {
                    if (ruo.funzioni[i].descrizione == "DO_NUOVODOC")
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private void setProtoArrivo()
        {
            schedaDocumento.protocollo = new DocsPAWA.DocsPaWR.ProtocolloEntrata();
            schedaDocumento.tipoProto = "A";
        }

        /// <summary>
        /// 
        /// </summary>
        private void setProtoPartenza()
        {
            schedaDocumento.protocollo = new DocsPAWA.DocsPaWR.ProtocolloUscita();
            schedaDocumento.tipoProto = "P";
        }

        /// <summary>
        /// 
        /// </summary>
        private void setProtoInterno()
        {
            schedaDocumento.protocollo = new DocsPAWA.DocsPaWR.ProtocolloInterno();
            schedaDocumento.tipoProto = "I";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txt_CodMit_P_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if(this.hiddenMitt.Value.Equals("0"))
                    this.txt_DescMit_P.Text = "";
                this.hiddenIdCodMit_p.Value = "";

                setDescCorrispondente(this.txt_CodMit_P.Text, "Mit", true);


                if (schedaDocumento.systemId != null)
                {
                    if (schedaDocumento.tipoProto == "A")
                    {
                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = true;
                    }
                    if (schedaDocumento.tipoProto == "P")
                    {
                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareMittente = true;
                    }
                    if (schedaDocumento.tipoProto == "I")
                    {
                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).daAggiornareMittente = true;
                    }
                }
                if (txt_CodMit_P.Text == "")
                {
                    if (schedaDocumento.tipoProto == "A")
                    {
                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente = null;
                        txt_DescMit_P.Text = "";
                        this.hiddenIdCodMit_p.Value = "";

                    }
                    if (schedaDocumento.tipoProto == "P")
                    {
                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente = null;
                        txt_DescMit_P.Text = "";
                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente = null;
                        txt_desc_uffRef.Text = "";
                        txt_cod_uffRef.Text = "";
                        this.hiddenIdCodMit_p.Value = "";
                    }
                    if (schedaDocumento.tipoProto == "I")
                    {
                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente = null;
                        txt_DescMit_P.Text = "";
                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).ufficioReferente = null;
                        txt_desc_uffRef.Text = "";
                        txt_cod_uffRef.Text = "";
                        this.hiddenIdCodMit_p.Value = "";
                    }
                }

                if (schedaDocumento.tipoProto == "A")
                {
                    //  string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_NumProtMit_P.ID + "').focus() </SCRIPT>";
                    //  RegisterStartupScript("focus", s);
                }
                else
                {
                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_CodDest_P.ID + "').focus() </SCRIPT>";
                    RegisterStartupScript("focus", s);
                }

                if (schedaDocumento.tipoProto == "A" && DocumentManager.isEnableMittentiMultipli())
                {
                    if (((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente != null && !checkDuplicatiMittMultipli(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente, lbx_mittMultiplo))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "MittPresenteInMittMultipli", "alert('Il mittente è già presente nella lista mittenti multipli.');", true);
                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = false;
                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente = null;
                        this.txt_CodMit_P.Text = string.Empty;
                        this.txt_DescMit_P.Text = string.Empty;
                        this.hiddenIdCodMit_p.Value = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void txt_oggetto_P_TextChanged(object sender, System.EventArgs e)
        private void ctrl_oggetto_OggChanged(object sender, Oggetto.OggTextEvent e)
        {
            try
            {
                // se proveniente dalla maschera oggettario
                // non crea un nuovo oggetto da descrizione textbox
                if (!_oggettoSelezionatoDaOggettario)
                {
                    DocsPaWR.Oggetto ProtoObj = new DocsPAWA.DocsPaWR.Oggetto();
                    ProtoObj.descrizione = e.Testo;//this.txt_oggetto_P.Text;
                    schedaDocumento.oggetto = ProtoObj;

                    if (schedaDocumento.systemId != null)
                    {
                        schedaDocumento.oggetto.daAggiornare = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_DescMit_P_TextChanged(object sender, System.EventArgs e)
        {
            try
            {

                DocsPaWR.Corrispondente corr = null;

                if (this.rbl_InOut_P.Items[0].Selected)
                {
                    corr = ((DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente;
                }

                if (this.rbl_InOut_P.Items[1].Selected)
                {
                    corr = ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente;
                }

                if (this.rbl_InOut_P.Items.Count > 2 && this.rbl_InOut_P.Items[2] != null && this.rbl_InOut_P.Items[2].Selected)
                {
                    corr = ((DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente;
                }
                string descrizione = this.txt_DescMit_P.Text;

                //if (corr == null || (descrizione.Trim() != "" && !descrizione.Trim().Equals(corr.descrizione) && !string.IsNullOrEmpty(corr.codiceRubrica)))
                if (corr == null || (descrizione.Trim() != "" && !descrizione.Trim().Equals(corr.descrizione) && !string.IsNullOrEmpty(corr.codiceRubrica) && (string.IsNullOrEmpty(corr.tipoIE) || corr.tipoIE != "E")))
                {
                    corr = new DocsPaWR.Corrispondente();

                    corr.descrizione = descrizione;

                    corr.tipoCorrispondente = "O";

                    //aggiunto per la modifica del popolamento del campo VAR_COD_RUBRICA dei corrisp occasionali

                    corr.idAmministrazione = (UserManager.getInfoUtente(this)).idAmministrazione;

                    if (this.rbl_InOut_P.Items[0].Selected)
                    {
                        /* vuol dire che sto creando un occasionale dalla popUp dettagliCorrisponenti.aspx */

                        if (Session["dettagliCorr.corrInfo"] != null)
                        {
                            corr.info = Session["dettagliCorr.corrInfo"] as DocsPaVO.addressbook.DettagliCorrispondente;
                            Session.Remove("dettagliCorr.corrInfo");
                        }

                        ((DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente = corr;

                        if (schedaDocumento.systemId != null)
                        {
                            ((DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = true;
                        }
                    }

                    if (this.rbl_InOut_P.Items[1].Selected)
                    {

                        ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente = corr;

                        if (schedaDocumento.systemId != null)
                        {

                            ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareMittente = true;
                        }
                    }

                    if (this.rbl_InOut_P.Items.Count > 2 && this.rbl_InOut_P.Items[2].Selected)
                    {

                        ((DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente = corr;

                        if (schedaDocumento.systemId != null)
                        {

                            ((DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).daAggiornareMittente = true;
                        }
                    }
                    DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                }

                //Correzione bug strano su Riproponi

                //if (corr != null && String.IsNullOrEmpty(corr.descrizione) && corr.tipoCorrispondente == "O")
                if (corr != null && corr.tipoCorrispondente == "O")
                {
                    corr = new DocsPaWR.Corrispondente();

                    corr.descrizione = descrizione;

                    corr.tipoCorrispondente = "O";

                    //aggiunto per la modifica del popolamento del campo VAR_COD_RUBRICA dei corrisp occasionali

                    corr.idAmministrazione = (UserManager.getInfoUtente(this)).idAmministrazione;
                    corr.descrizione = descrizione;
                    if (this.rbl_InOut_P.Items[0].Selected)
                    {
                        /* vuol dire che sto creando un occasionale dalla popUp dettagliCorrisponenti.aspx */

                        if (Session["dettagliCorr.corrInfo"] != null)
                        {
                            corr.info = Session["dettagliCorr.corrInfo"] as DocsPaVO.addressbook.DettagliCorrispondente;
                            Session.Remove("dettagliCorr.corrInfo");
                        }

                        ((DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente = corr;

                        if (schedaDocumento.systemId != null)
                        {
                            ((DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = true;
                        }
                    }

                    if (this.rbl_InOut_P.Items[1].Selected)
                    {

                        ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente = corr;

                        if (schedaDocumento.systemId != null)
                        {

                            ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareMittente = true;
                        }
                    }

                    if (this.rbl_InOut_P.Items.Count > 2 && this.rbl_InOut_P.Items[2].Selected)
                    {

                        ((DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente = corr;

                        if (schedaDocumento.systemId != null)
                        {

                            ((DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).daAggiornareMittente = true;
                        }
                    }
                    DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                }

                if (schedaDocumento.tipoProto == "A")
                {
                    // string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_NumProtMit_P.ID + "').focus() </SCRIPT>";
                    // RegisterStartupScript("focus", s);
                }
                else
                {
                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_CodDest_P.ID + "').focus() </SCRIPT>";
                    RegisterStartupScript("focus", s);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }
        }

        //private void creaCorrOccasionalePerProtocollo()
        //{

        //    try 
        //    {
        //        DocsPaWR.Corrispondente corr=null;
        //        if( this.rbl_InOut_P.Items[0].Selected)
        //        {
        //            corr=((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente;
        //        }
        //        if( this.rbl_InOut_P.Items[1].Selected)
        //        {
        //            corr=((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente;
        //        }
        //        if( this.rbl_InOut_P.Items.Count>2 && this.rbl_InOut_P.Items[2]!=null && this.rbl_InOut_P.Items[2].Selected)
        //        {
        //            corr=((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente;
        //        }
        //        string descrizione=this.txt_DescMit_P.Text;

        //        if(descrizione!="")
        //        {
        //            if (corr==null || !descrizione.Equals(corr.descrizione))
        //            {
        //                corr=new DocsPAWA.DocsPaWR.Corrispondente();
        //                corr.descrizione = descrizione;
        //                corr.tipoCorrispondente = "O";
        //                //aggiunto per la modifica del popolamento del campo VAR_COD_RUBRICA dei corrisp occasionali
        //                corr.idAmministrazione=(UserManager.getInfoUtente(this)).idAmministrazione;

        //                if( this.rbl_InOut_P.Items[0].Selected)
        //                {
        //                    /* vuol dire che sto creando un occasionale dalla popUp dettagliCorrisponenti.aspx */

        //                    if(Session["dettagliCorr.corrInfo"]!=null)
        //                    {
        //                        corr.info = Session["dettagliCorr.corrInfo"] as DocsPaVO.addressbook.DettagliCorrispondente;
        //                        Session.Remove("dettagliCorr.corrInfo");
        //                    }
        //                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente = corr;
        //                    if(schedaDocumento.systemId != null)
        //                    {
        //                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = true;
        //                    }

        //                }
        //                if( this.rbl_InOut_P.Items[1].Selected)
        //                {
        //                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente = corr;
        //                    if(schedaDocumento.systemId != null)
        //                    {
        //                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareMittente = true;
        //                    }
        //                }


        //                if(this.rbl_InOut_P.Items.Count>2 && this.rbl_InOut_P.Items[2].Selected)
        //                {
        //                    ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente = corr;
        //                    if(schedaDocumento.systemId != null)
        //                    {
        //                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).daAggiornareMittente = true;
        //                    }
        //                }

        //                DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
        //            }
        //        }
        //    } 
        //    catch (Exception ex) 
        //    {
        //        ErrorManager.redirect(this, ex,"protocollazione");
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipoAtto"></param>
        private void setTipoAtto(string tipoAtto)
        {
            if (tipoAtto != null)
            {
                for (int i = 0; i < this.ddl_tipoAtto.Items.Count; i++)
                {
                    if (this.ddl_tipoAtto.Items[i].Value == tipoAtto)
                    {
                        ddl_tipoAtto.SelectedItem.Selected = false;
                        this.ddl_tipoAtto.Items[i].Selected = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_DescMitInt_P_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                DocsPaWR.Corrispondente corr = new DocsPAWA.DocsPaWR.Corrispondente();
                corr.descrizione = this.txt_DescMitInt_P.Text;
                corr.tipoCorrispondente = "O";

                //aggiunto per la modifica del popolamento del campo VAR_COD_RUBRICA dei corrisp occasionale
                corr.idAmministrazione = (UserManager.getInfoUtente(this)).idAmministrazione;

                ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenteIntermedio = corr;

                if (schedaDocumento.systemId != null)
                {
                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittenteIntermedio = true;
                }

                DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_NumProtMit_P_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (this.rbl_InOut_P.Items[0].Selected)
                {
                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).descrizioneProtocolloMittente = this.txt_NumProtMit_P.Text;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_CodMitInt_P_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (this.rbl_InOut_P.Items[0].Selected)
                {
                    setDescCorrispondente(this.txt_CodMitInt_P.Text, "MitInt", true);
                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittenteIntermedio = true;

                    if (txt_CodMitInt_P.Text == "")
                    {
                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenteIntermedio = null;
                        txt_DescMitInt_P.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }
        }

        #region gestione Corrispondenti

        RubricaCallType get_calltype(string tipoCor)
        {
            RubricaCallType calltype;

            if (schedaDocumento.tipoProto != "A")
            {
                if (schedaDocumento.tipoProto == "I")
                {
                    if (tipoCor == "Mit")
                        calltype = RubricaCallType.CALLTYPE_PROTO_INT_MITT;
                    else
                        calltype = RubricaCallType.CALLTYPE_PROTO_INT_DEST;
                }
                else
                {
                    if (tipoCor == "Mit")
                        calltype = RubricaCallType.CALLTYPE_PROTO_OUT_MITT;
                    else
                        calltype = RubricaCallType.CALLTYPE_PROTO_OUT;
                }
            }
            else
            {
                calltype = RubricaCallType.CALLTYPE_PROTO_IN;
            }
            return calltype;
        }


        private DocsPAWA.DocsPaWR.SmistamentoRubrica creaSmistamentoRubrica(DocsPAWA.DocsPaWR.RubricaCallType calltype)
        {
            //popolamento oggetto per RUBRICA_USA_PROTO_SMISTAMENTO
            DocsPaWR.SmistamentoRubrica smistamentoRubrica = new SmistamentoRubrica();
            //smistamentoRubrica.smistamento: indica se è abilitata o meno lo smistamento
            smistamentoRubrica.smistamento = DocsPAWA.Utils.getAbilitazioneSmistamento();

            //callType
            smistamentoRubrica.calltype = (RubricaCallType)Convert.ToInt32(calltype);

            //InfoUtente
            DocsPaWR.InfoUtente infoUt = new DocsPAWA.DocsPaWR.InfoUtente();
            infoUt = UserManager.getInfoUtente(this);
            smistamentoRubrica.infoUt = infoUt;

            //Ruolo Protocollatore
            if (Session["userRuolo"] != null)
            {
                smistamentoRubrica.ruoloProt = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
            }

            if (UserManager.getRegistroSelezionato(this) != null)
            {
                //Registro corrente
                smistamentoRubrica.idRegistro = UserManager.getRegistroSelezionato(this).systemId;
            }
            return smistamentoRubrica;
        }


        private void setDescCorrispondente(string codiceRubrica, string tipoCor, bool fineValidita)
        {
            string msg = "Codice rubrica non esistente";
            DocsPaWR.Corrispondente corr = null;
            RubricaCallType calltype = get_calltype(tipoCor);
            ElementoRubrica er;
            ElementoRubrica[] erApp;
            btn_aggiungiDest_P.Enabled = true;
            ElementoRubrica[] listaCorr = null;
            //Session.Remove("multiCorr");

            if (codiceRubrica != "")
            {
                if (System.Configuration.ConfigurationManager.AppSettings["RUBRICA_V2"] != null &&
                    System.Configuration.ConfigurationManager.AppSettings["RUBRICA_V2"] == "1")
                {
                    //se lo smistamento è abilitato
                    if (Utils.getAbilitazioneSmistamento().Equals("1"))
                    {
                        if (calltype == RubricaCallType.CALLTYPE_PROTO_OUT ||
                            calltype == RubricaCallType.CALLTYPE_PROTO_INT_DEST)
                        {
                            //creazione oggetto smistamentoRubrica

                            DocsPaWR.SmistamentoRubrica smistaRubrica = creaSmistamentoRubrica(calltype);

                            //solamento per i destinatari di protocolli in uscita e interni
                            //devo valutare le condizioni per lo smistamento
                            //DocsPaWR.ParametriRicercaRubrica nuovo = new ParametriRicercaRubrica();
                            //nuovo.codice = codiceRubrica;
                            //nuovo.calltype = calltype;
                            //erApp = UserManager.getElementiRubrica(this, nuovo,smistaRubrica);
                            if ((er = UserManager.getElementoRubrica(this, codiceRubrica, smistaRubrica, "")) != null)
                            {
                                if (!er.isVisibile)
                                {
                                    msg = "Codice rubrica non utilizzabile in questo contesto";
                                    codice_non_trovato(msg, ref txt_CodMit_P, ref txt_DescMit_P);
                                    set_uffref_read_only();
                                    DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                                    btn_aggiungiDest_P.Enabled = false;
                                    return;
                                }
                            }
                        }
                    }

                    if (schedaDocumento.tipoProto != "A")
                    {
                        if (schedaDocumento.tipoProto == "I")
                        {
                            //Per le liste
                            ArrayList lsCorr = new ArrayList();
                            if (System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null && System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "1")
                            {
                                string idAmm = UserManager.getInfoUtente().idAmministrazione;
                                lsCorr = UserManager.getCorrispondentiByCodLista(this, codiceRubrica, idAmm);
                                if (lsCorr.Count != 0)
                                {
                                    if (tipoCor != "Mit")
                                    {
                                        corr = new DocsPAWA.DocsPaWR.Corrispondente();
                                        corr.codiceRubrica = codiceRubrica;
                                        corr.descrizione = UserManager.getNomeLista(this, codiceRubrica, idAmm);
                                        corr.tipoCorrispondente = "L";
                                    }
                                }
                                else
                                {
                                    // corr = UserManager.GetCorrispondenteInterno(this, codiceRubrica, fineValidita, true);
                                    listaCorr = UserManager.getElementiRubricaMultipli(this, codiceRubrica, calltype, true);
                                    if (listaCorr != null && listaCorr.Length > 0)
                                    {
                                        if (listaCorr.Length == 1)
                                        {
                                            if (!string.IsNullOrEmpty(listaCorr[0].systemId))
                                                corr = UserManager.getCorrispondenteBySystemID(this.Page, listaCorr[0].systemId);
                                            else
                                                corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, listaCorr[0].codice);
                                        }
                                        else
                                        {
                                            if (tipoCor == "Mit")
                                            {
                                                if (this.hiddenMitt.Value.Equals("0"))
                                                    ClientScript.RegisterStartupScript(this.GetType(), "multiCorr", "ApriFinestraMultiCorrispondentiMittenti();", true);
                                            }
                                            else
                                            {
                                                ClientScript.RegisterStartupScript(this.GetType(), "multiCorr", "ApriFinestraMultiCorrispondentiDestinatari();", true);
                                            }
                                            Session.Add("multiCorr", listaCorr);
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                corr = UserManager.GetCorrispondenteInterno(this, codiceRubrica, fineValidita, true);
                            }
                            if (tipoCor == "Mit")
                                calltype = RubricaCallType.CALLTYPE_PROTO_INT_MITT;
                            else
                                calltype = RubricaCallType.CALLTYPE_PROTO_INT_DEST;
                        }
                        else
                        {
                            if (tipoCor == "Mit")
                            {
                                // corr = UserManager.GetCorrispondenteInterno(this, codiceRubrica, fineValidita, true);

                                listaCorr = UserManager.getElementiRubricaMultipli(this, codiceRubrica, calltype, true);
                                if (listaCorr != null && listaCorr.Length > 0)
                                {
                                    if (listaCorr.Length == 1)
                                    {
                                        if (!string.IsNullOrEmpty(listaCorr[0].systemId))
                                            corr = UserManager.getCorrispondenteBySystemID(this.Page, listaCorr[0].systemId);
                                        else
                                            corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, listaCorr[0].codice);
                                    }
                                    else
                                    {
                                        if (this.hiddenMitt.Value.Equals("0"))
                                        {
                                            ClientScript.RegisterStartupScript(this.GetType(), "multiCorr", "ApriFinestraMultiCorrispondentiMittenti();", true);
                                            Session.Add("multiCorr", listaCorr);
                                            return;
                                        }
                                        else
                                        {
                                            if (this.hiddenMitt.Value.Equals("2"))
                                                corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, codiceRubrica);
                                            else
                                            {
                                                foreach (ElementoRubrica el in listaCorr)
                                                {
                                                    if (el.codiceRegistro != null && el.codiceRegistro.ToUpper().Equals(this.hiddenMitt.Value.ToUpper()))
                                                    {
                                                        if (!string.IsNullOrEmpty(el.systemId))
                                                            corr = UserManager.getCorrispondenteBySystemID(this.Page, el.systemId);
                                                    }
                                                }
                                            }
                                            this.hiddenMitt.Value = "0";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //Per le liste
                                ArrayList lsCorr = new ArrayList();
                                if (System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null && System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "1")
                                {
                                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                                    lsCorr = UserManager.getCorrispondentiByCodLista(this, codiceRubrica, idAmm);
                                    if (lsCorr != null && lsCorr.Count != 0)
                                    {
                                        corr = new DocsPAWA.DocsPaWR.Corrispondente();
                                        corr.codiceRubrica = codiceRubrica;
                                        corr.descrizione = UserManager.getNomeLista(this, codiceRubrica, idAmm);
                                        corr.tipoCorrispondente = "L";
                                    }
                                    else
                                    {
                                        lsCorr = UserManager.getCorrispondentiByCodRF(this, codiceRubrica);
                                        if (lsCorr != null && lsCorr.Count != 0)
                                        {
                                            corr = new DocsPAWA.DocsPaWR.Corrispondente();
                                            corr.codiceRubrica = codiceRubrica;
                                            corr.descrizione = UserManager.getNomeRF(this, codiceRubrica);
                                            corr.tipoCorrispondente = "F";
                                        }
                                        else
                                        {
                                            // corr = UserManager.getCorrispondenteRubrica(this, codiceRubrica, calltype);
                                            listaCorr = UserManager.getElementiRubricaMultipli(this, codiceRubrica, calltype, true);
                                            if (listaCorr != null && listaCorr.Length > 0)
                                            {
                                                if (listaCorr.Length == 1)
                                                {
                                                    if (!string.IsNullOrEmpty(listaCorr[0].systemId))
                                                        corr = UserManager.getCorrispondenteBySystemID(this.Page, listaCorr[0].systemId);
                                                    else
                                                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, listaCorr[0].codice);
                                                }
                                                else
                                                {
                                                    if (this.hiddenDest.Value.Equals("0"))
                                                    {
                                                        ClientScript.RegisterStartupScript(this.GetType(), "multiCorr", "ApriFinestraMultiCorrispondentiDestinatari();", true);
                                                        Session.Add("multiCorr", listaCorr);
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        if (this.hiddenDest.Value.Equals("2"))
                                                            corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, codiceRubrica);
                                                        else
                                                        {
                                                            foreach (ElementoRubrica el in listaCorr)
                                                            {
                                                                if (el.codiceRegistro != null && el.codiceRegistro.ToUpper().Equals(this.hiddenDest.Value.ToUpper()))
                                                                {
                                                                    if (!string.IsNullOrEmpty(el.systemId))
                                                                        corr = UserManager.getCorrispondenteBySystemID(this.Page, el.systemId);
                                                                }
                                                            }
                                                        }
                                                        //this.hiddenDest.Value = "0";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //corr = UserManager.getCorrispondente(this, codiceRubrica, fineValidita);
                                    corr = UserManager.getCorrispondenteRubrica(this, codiceRubrica, calltype);
                                }
                                calltype = RubricaCallType.CALLTYPE_PROTO_OUT;
                            }
                        }
                    }
                    else
                    {
                        //corr = UserManager.getCorrispondente(this, codiceRubrica, fineValidita);
                        calltype = DocsPaWR.RubricaCallType.CALLTYPE_PROTO_IN;
                        //corr = UserManager.getCorrispondenteRubrica(this, codiceRubrica, calltype);
                        listaCorr = UserManager.getElementiRubricaMultipli(this, codiceRubrica, calltype, true);
                        if (listaCorr != null && listaCorr.Length > 0)
                        {
                            if (listaCorr.Length == 1)
                            {
                                if (!string.IsNullOrEmpty(listaCorr[0].systemId))
                                    corr = UserManager.getCorrispondenteBySystemID(this.Page, listaCorr[0].systemId);
                                else
                                    corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, listaCorr[0].codice);
                            }
                            else
                            {
                                if (this.hiddenMitt.Value.Equals("0"))  // non seleziono da ajax
                                {
                                    ClientScript.RegisterStartupScript(this.GetType(), "multiCorr", "ApriFinestraMultiCorrispondentiMittenti();", true);
                                    Session.Add("multiCorr", listaCorr);
                                    return;
                                }
                                else
                                {
                                    if (this.hiddenMitt.Value.Equals("2"))  // ho selezionato da ajax elemento di RC
                                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, codiceRubrica);
                                    else
                                    {  // ho selezionato da ajax un elemento di rubrica non comune
                                        foreach (ElementoRubrica el in listaCorr)
                                        {
                                            if (el.codiceRegistro != null && el.codiceRegistro.ToUpper().Equals(this.hiddenMitt.Value.ToUpper()))
                                            {
                                                if (!string.IsNullOrEmpty(el.systemId))
                                                    corr = UserManager.getCorrispondenteBySystemID(this.Page, el.systemId);
                                            }
                                        }
                                    }
                                    this.hiddenMitt.Value = "0";
                                }
                            }
                        }
                    }

                    if (tipoCor == "Mit")
                    {
                        if (schedaDocumento.tipoProto == "A")
                        {
                            if (corr != null)
                            {
                                ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente = corr;
                            }
                            else
                            {
                                codice_non_trovato(msg, ref txt_CodMit_P, ref txt_DescMit_P);

                                //per l'ufficio referente
                                set_uffref_read_only();
                            }
                        }
                        if (schedaDocumento.tipoProto == "P")
                        {
                            if (corr != null)
                            {
                                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente = corr;
                            }
                            else
                            {
                                codice_non_trovato(msg, ref txt_CodMit_P, ref txt_DescMit_P);

                                //per l'ufficio referente
                                set_uffref_read_only();
                            }
                        }
                        if (schedaDocumento.tipoProto == "I")
                        {
                            if (corr != null)
                            {
                                ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente = corr;
                            }
                            else
                            {
                                codice_non_trovato(msg, ref txt_CodMit_P, ref txt_DescMit_P);

                                //per l'ufficio referente
                                set_uffref_read_only();
                            }
                        }
                    }

                    if (tipoCor == "MitInt")
                    {
                        if (corr != null)
                            ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenteIntermedio = corr;
                        else
                            codice_non_trovato(msg, ref txt_CodMitInt_P, ref txt_DescMitInt_P);
                    }

                    if (tipoCor == "Dest")
                    {
                        //aggiorna solo il campo descrizione ma non aggiunge tra i destinatari
                        if (corr != null)
                        {
                            if (corr is DocsPAWA.DocsPaWR.Ruolo)
                            {
                                DocsPaWR.Ruolo corrRuolo = (DocsPAWA.DocsPaWR.Ruolo)corr;

                                string desc = "";
                                desc = corrRuolo.descrizione;
                                this.txt_DescDest_P.Text = desc;
                            }
                            else
                            {
                                this.txt_DescDest_P.Text = corr.descrizione;
                                //il campo della descrizione del corrispondente deve essere in sola lettura 
                                //quando il tipo di protocollo è INTERNO
                                if (rbl_InOut_P.SelectedItem.Value == "Own")
                                {
                                    if (string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) || !(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                                    {
                                        this.txt_DescDest_P.ReadOnly = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            codice_non_trovato(msg, ref txt_CodDest_P, ref txt_DescDest_P);

                            // Il campo della descrizione del corrispondente deve essere in sola lettura 
                            // quando il tipo di protocollo è INTERNO
                            if (rbl_InOut_P.SelectedItem.Value == "Own")
                            {
                                if (string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) || !(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                                {
                                    this.txt_DescDest_P.ReadOnly = true;
                                }
                            }
                        }
                    }

                    //DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);

                    if (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                        && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1") && !schedaDocumento.tipoProto.Equals("A"))
                    {
                        if ((tipoCor == "MitInt" || tipoCor == "Mit"))
                        {
                            /* se il mittente del protocollo in uscita o interno è una UO 
                             * allora viene popolato il campo ufficio referente, altrimenti
                             * in caso sia una un ruolo o una persona viene mandato un avviso 
                             * all'utente */
                            if (corr != null)
                            {
                                if (corr.GetType() != typeof(DocsPAWA.DocsPaWR.Utente))
                                {
                                    setDescUfficioReferente(txt_CodMit_P.Text, "U", corr);
                                }
                                else
                                {
                                    ((DocsPAWA.DocsPaWR.Utente)corr).ruoli = UserManager.getRuoliFiltrati(((DocsPAWA.DocsPaWR.Utente)corr).ruoli);
                                    if (((DocsPAWA.DocsPaWR.Utente)corr).ruoli != null && ((DocsPAWA.DocsPaWR.Utente)corr).ruoli.Length > 0)
                                    {
                                        if (((DocsPAWA.DocsPaWR.Utente)corr).ruoli.Length > 1)
                                        {
                                            Response.Write("<script>alert('l\\'utente " + corr.descrizione + " appartiene a più UO\\n selezionare quella di interesse');</script>");

                                            //Response.Write("<script>var win = window.open(\"../popup/scegliUoUtente.aspx?win=protocollo&rubr="+corr.descrizione+"\",\"new\",\"width=550,height=280,toolbar=no,directories=no,menubar=no, scrollbars=no\"); var newLeft=(screen.availWidth-590); var newTop=(screen.availHeight-540); win.moveTo(newLeft,newTop);</script>");
                                            //Response.Write("<SCRIPT>alert('ATTENZIONE: Il mittente specificato è un utente');</SCRIPT>");
                                            //Response.Write("<script>rtnValue=window.showModalDialog(\"../popup/scegliUoUtente.aspx?win=protocollo&rubr="+corr.descrizione+"\",\"new\",'dialogWidth:615px;dialogHeight:380px;status:no;resizable:no;scroll:no;dialogLeft:100;dialogTop:100;center:no;help:no;');</script>");

                                            string scriptString = "<SCRIPT>ApriFinestraScegliUO('" + corr.codiceRubrica + "');</SCRIPT>";
                                            this.RegisterStartupScript("ApriFinestraScegliUO", scriptString);
                                        }
                                        else
                                        {
                                            if (schedaDocumento.tipoProto == "P")
                                            {
                                                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente = ((DocsPAWA.DocsPaWR.Utente)corr).ruoli[0].uo;
                                            }
                                            else
                                            {
                                                ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).ufficioReferente = ((DocsPAWA.DocsPaWR.Utente)corr).ruoli[0].uo;
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                    DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                }
            }
        }

        void codice_non_trovato(string msg, ref TextBox tbxCod, ref TextBox tbxDesc)
        {
            Response.Write("<script>alert(\"" + msg + "\");</script>");
            string s = "<SCRIPT language='javascript'>document.getElementById('" + tbxCod.ID + "').focus() </SCRIPT>";
            RegisterStartupScript("focus", s);

            tbxCod.Text = "";
            tbxDesc.Text = "";
        }

        void set_uffref_read_only()
        {
            txt_cod_uffRef.ReadOnly = true;
            txt_cod_uffRef.BackColor = Color.WhiteSmoke;
            txt_desc_uffRef.BackColor = Color.WhiteSmoke;
            btn_Rubrica_ref.Enabled = false;
        }

        private void setDescUfficioReferente(string codiceRubrica, string tipoCorr, DocsPAWA.DocsPaWR.Corrispondente corr)
        {
            DocsPaWR.Corrispondente corrRef = null;
            UserManager.removeCorrispondentiSelezionati(this);
            bool trovato = false;
            string msg = "";

            if (!codiceRubrica.Equals(""))
                corrRef = UserManager.getCorrispondenteReferente(this, codiceRubrica, false);

            if (corrRef != null)
            {
                if (corrRef.GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa)))
                {
                    if (tipoCorr.Equals("U"))
                        trovato = true;
                }
                if (corrRef.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Ruolo)))
                {
                    if (corr != null)
                    {
                        msg = "";
                        trovato = true;
                    }
                    else
                    {
                        msg = "ATTENZIONE: il codice rubrica specificato è associato a un RUOLO \\n\\t       non ad una UO!";
                    }
                }
                if (corrRef.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
                {
                    msg = "ATTENZIONE: il codice rubrica specificato è associato a un UTENTE \\n\\t       non ad una UO!";

                }
                if (trovato)
                {
                    this.txt_cod_uffRef.Text = corrRef.codiceRubrica;
                    this.txt_desc_uffRef.Text = UserManager.getDecrizioneCorrispondenteSemplice(corrRef);
                    if (schedaDocumento.tipoProto.Equals("A"))
                    {
                        if (corr == null)
                            ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).ufficioReferente = corrRef;

                    }
                    if (schedaDocumento.tipoProto.Equals("P"))
                    {
                        if (corr != null)
                        {
                            //VENGO DAL textchange DEL campo codice MITTENTE
                            if (corr.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
                            {
                                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente = ((DocsPAWA.DocsPaWR.Ruolo)corr).uo;
                            }
                            if (corr.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
                            {
                                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente = corr;
                            }
                        }
                        else
                        {
                            //VENGO DAL textchange DEL campo codice DELL'UFFICIO REFERENTE
                            ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente = corrRef;
                        }
                    }
                    if (schedaDocumento.tipoProto.Equals("I"))
                    {
                        if (corr != null)
                        {
                            //VENGO DAL textchange DEL campo codice MITTENTE
                            if (corr.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
                            {
                                ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).ufficioReferente = ((DocsPAWA.DocsPaWR.Ruolo)corr).uo;
                            }
                            if (corr.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
                            {
                                ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).ufficioReferente = corr;
                            }
                        }
                        else
                        {
                            ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).ufficioReferente = corrRef;
                        }
                    }
                    this.txt_cod_uffRef.ReadOnly = false;
                    //					this.txt_desc_uffRef.ReadOnly = false;
                    this.txt_cod_uffRef.BackColor = Color.White;
                    this.txt_desc_uffRef.BackColor = Color.White;
                    this.btn_Rubrica_ref.Enabled = true;
                    set_btn_Rubrica_ref_event();
                }
                else
                {
                    this.txt_cod_uffRef.Text = "";
                    this.txt_desc_uffRef.Text = "";
                }
            }
            else
            {
                msg = "Codice rubrica non esistente";
                this.txt_cod_uffRef.Text = "";
                this.txt_desc_uffRef.Text = "";
            }

            if (!msg.Equals(""))
            {
                if (!codiceRubrica.Equals(""))
                {
                    Response.Write("<SCRIPT>alert('" + msg + "');</SCRIPT>");
                }
                string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_cod_uffRef.ID + "').focus() </SCRIPT>";
                RegisterStartupScript("focus", s);
            }
            DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
        }

        //metodo per settare la descrizione dell'ufficio referente nella pagina di protocollo
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tipoDest"></param>
        private void addDestinatari(int index, string tipoDest)
        {
            //controlo se esiste già il corrispondente selezionato
            DocsPaWR.Corrispondente[] listaDest;
            DocsPaWR.Corrispondente[] listaDestCC;
            DocsPaWR.Corrispondente corr;

            listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari;
            listaDestCC = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza;


            //aggiungo il corrispondente
            if (tipoDest.Equals("P"))
            {
                corr = listaDestCC[index];
                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari = UserManager.addCorrispondente(listaDest, corr);
                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatari = true;
            }
            else
                if (tipoDest.Equals("C"))
                {
                    corr = listaDest[index];
                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza = UserManager.addCorrispondente(listaDestCC, corr);
                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatariConoscenza = true;
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tipoDest"></param>
        private void removeDestinatari(int index, string tipoDest)
        {
            DocsPaWR.Corrispondente[] listaDest;
            if (tipoDest.Equals("P"))
            {
                listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari;
                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari = UserManager.removeCorrispondente(listaDest, index);
                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatari = true;
            }
            else
                if (tipoDest.Equals("C"))
                {
                    listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza;
                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza = UserManager.removeCorrispondente(listaDest, index);
                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatariConoscenza = true;
                }

            setListBoxDestinatari();
        }

        private void setListBoxDestinatari()
        {
            logger.Info("BEGIN");
            //Valido per i documenti in Partenza
            DocsPaWR.Corrispondente destinatario;
            if ((((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari != null) && (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari.Length > 0))
            {
                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari.OrderBy(d => d.descrizione).ToArray();
                //elenco canali preferenziale /destinatario proto
                System.Collections.Generic.List<Corrispondente> listPrefChannelDest = new System.Collections.Generic.List<Corrispondente>();
                if (schedaDocumento.systemId != null)
                    listPrefChannelDest = UserManager.GetPrefChannelAllDest(schedaDocumento.systemId, "D");
                for (int i = 0; i < ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari.Length; i++)
                {
                    destinatario = (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[i]);
                    string editMode = Request.QueryString["editMode"];
                    string annullato = string.Empty;
                    bool rtn = CtrlIfDestWithRicevuta_NEW(destinatario, out annullato);
                    if (rtn)
                    {
                        string canaleRef = string.Empty;
                        if (destinatario != null && destinatario.canalePref != null)
                        {
                            Canale canaleOrig = destinatario.canalePref = (from c in listPrefChannelDest where c.systemId.Equals(destinatario.systemId) select c.canalePref).FirstOrDefault();
                            //non abbiamo ancora salvato quindi il canale preferenziale è uguale al mezzo di spedizione
                            if (canaleOrig == null)
                            {
                                canaleOrig = destinatario.canalePref;
                            }
                            if (canaleOrig != null && canaleOrig.typeId != null)
                            {
                                if (canaleOrig.typeId.ToUpper().Equals("MAIL") ||
                                    canaleOrig.typeId.ToUpper().Equals("INTEROPERABILITA") ||
                                    canaleOrig.typeId.ToUpper().Equals("INTEROPERABILITAPITRE"))
                                {
                                    canaleRef = "(" + canaleOrig.typeId + ")  ";
                                }
                                else
                                    if (canaleOrig.typeId.ToUpper().Equals(InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId))
                                        canaleRef = String.Format("({0})", InteroperabilitaSemplificataManager.ChannelDescription);
                                    else
                                        canaleRef = "  ";
                            }
                            else
                            {
                                canaleRef = "  ";
                            }
                        }
                        if (annullato == string.Empty)
                            this.lbx_dest.Items.Add(new StringBuilder().AppendFormat("(*){0}{1}", canaleRef, UserManager.getDecrizioneCorrispondenteSemplice(destinatario)).ToString());
                        else if (annullato == "1")
                            this.lbx_dest.Items.Add(new StringBuilder().AppendFormat("(*)(A){0}{1}", canaleRef, UserManager.getDecrizioneCorrispondenteSemplice(destinatario)).ToString());
                        else if (annullato == "E")
                            this.lbx_dest.Items.Add(new StringBuilder().AppendFormat("(!){0}{1}", canaleRef, UserManager.getDecrizioneCorrispondenteSemplice(destinatario)).ToString());

                    }
                    else
                    {
                        StringBuilder formatDestinatario = new StringBuilder();
                        if (destinatario != null && destinatario.canalePref != null)
                        {
                            Canale canaleOrig = (from c in listPrefChannelDest where c.systemId.Equals(destinatario.systemId) select c.canalePref).FirstOrDefault();
                            //non abbiamo ancora salvato quindi il canale preferenziale è uguale al mezzo di spedizione
                            if (canaleOrig == null)
                            {
                                canaleOrig = destinatario.canalePref;
                            }
                            if (canaleOrig != null && canaleOrig.typeId != null)
                            {
                                if (canaleOrig.typeId.ToUpper().Equals("MAIL") ||
                                    canaleOrig.typeId.ToUpper().Equals("INTEROPERABILITA") ||
                                    canaleOrig.typeId.ToUpper().Equals("INTEROPERABILITAPITRE"))
                                {

                                    formatDestinatario.AppendFormat("({0})  {1}", canaleOrig.typeId, UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                                }
                                else
                                    if (canaleOrig.typeId.ToUpper().Equals(InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId))
                                        formatDestinatario.AppendFormat("({0})  {1}", InteroperabilitaSemplificataManager.ChannelDescription, UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                            }
                        }
                        if (string.IsNullOrEmpty(formatDestinatario.ToString()))
                            formatDestinatario.AppendFormat(UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                        this.lbx_dest.Items.Add(formatDestinatario.ToString());
                    }

                    this.lbx_dest.Items[i].Value = destinatario.codiceRubrica;
                }
            }

            if ((((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza != null) && (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza.Length > 0))
            {
                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza.OrderBy(d => d.descrizione).ToArray();
                //elenco canali preferenziale /destinatario proto
                System.Collections.Generic.List<Corrispondente> listPrefChannelDestCC = new System.Collections.Generic.List<Corrispondente>();
                if (schedaDocumento.systemId != null)
                    listPrefChannelDestCC = UserManager.GetPrefChannelAllDest(schedaDocumento.systemId, "C");
                for (int i = 0; i < ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza.Length; i++)
                {
                    destinatario = (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza[i]);
                    string annullato = string.Empty;
                    bool rtn = CtrlIfDestWithRicevuta_NEW(destinatario, out annullato);
                    if (rtn)
                    {
                        string canaleRef = string.Empty;
                        if (destinatario != null && destinatario.canalePref != null)
                        {
                            Canale canaleOrig = (from c in listPrefChannelDestCC where c.systemId.Equals(destinatario.systemId) select c.canalePref).FirstOrDefault();
                            //non abbiamo ancora salvato quindi il canale preferenziale è uguale al mezzo di spedizione
                            if (canaleOrig == null)
                            {
                                canaleOrig = destinatario.canalePref;
                            }
                            if (canaleOrig != null && canaleOrig.typeId != null)
                            {
                                if (canaleOrig.typeId.ToUpper().Equals("MAIL") ||
                                    canaleOrig.typeId.ToUpper().Equals("INTEROPERABILITA") ||
                                    canaleOrig.typeId.ToUpper().Equals("INTEROPERABILITAPITRE"))
                                {
                                    canaleRef = "(" + canaleOrig.typeId + ")  ";
                                }
                                else
                                    if (canaleOrig.typeId.ToUpper().Equals(InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId))
                                        canaleRef = String.Format("({0})", InteroperabilitaSemplificataManager.ChannelDescription);
                                    else
                                        canaleRef = "  ";
                            }
                            else
                            {
                                canaleRef = "  ";
                            }
                        }
                        else
                        {
                            canaleRef = "  ";
                        }
                        if (annullato == string.Empty)
                            this.lbx_destCC.Items.Add(new StringBuilder().AppendFormat("(*){0}{1}", canaleRef, UserManager.getDecrizioneCorrispondenteSemplice(destinatario)).ToString());
                        else if (annullato == "1")
                            this.lbx_destCC.Items.Add(new StringBuilder().AppendFormat("(*)(A){0}{1}", canaleRef, UserManager.getDecrizioneCorrispondenteSemplice(destinatario)).ToString());
                        else if (annullato == "E")
                            this.lbx_destCC.Items.Add(new StringBuilder().AppendFormat("(!){0}{1}", canaleRef, UserManager.getDecrizioneCorrispondenteSemplice(destinatario)).ToString());

                    }
                    else
                    {
                        StringBuilder formatDestinatario = new StringBuilder();
                        if (destinatario != null && destinatario.canalePref != null)
                        {
                            Canale canaleOrig = (from c in listPrefChannelDestCC where c.systemId.Equals(destinatario.systemId) select c.canalePref).FirstOrDefault();
                            //non abbiamo ancora salvato quindi il canale preferenziale è uguale al mezzo di spedizione
                            if (canaleOrig == null)
                            {
                                canaleOrig = destinatario.canalePref;
                            }
                            if (canaleOrig != null && canaleOrig.typeId != null)
                            {
                                if (canaleOrig.typeId.ToUpper().Equals("MAIL") ||
                                    canaleOrig.typeId.ToUpper().Equals("INTEROPERABILITA") ||
                                    canaleOrig.typeId.ToUpper().Equals("INTEROPERABILITAPITRE"))
                                {

                                    formatDestinatario.AppendFormat("({0})  {1}", canaleOrig.typeId, UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                                }
                                else
                                    if (canaleOrig.typeId.ToUpper().Equals(InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId))
                                        formatDestinatario.AppendFormat("({0})  {1}", InteroperabilitaSemplificataManager.ChannelDescription, UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                            }
                        }
                        if (string.IsNullOrEmpty(formatDestinatario.ToString()))
                            formatDestinatario.AppendFormat(UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                        this.lbx_destCC.Items.Add(formatDestinatario.ToString());
                    }
                    this.lbx_destCC.Items[i].Value = destinatario.codiceRubrica;
                }
            }
            logger.Info("END");
        }

        /// <summary>
        /// 
        /// </summary>
        private void setListBoxDestinatariAbilitati(string corrDisable)
        {
            //Valido per i documenti in Partenza
            DocsPaWR.Corrispondente destinatario;
            ArrayList destinatariAbilitati = new ArrayList();
            //string destDisabilitati ="";
            if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari != null)
            {
                for (int i = 0; i < ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari.Length; i++)
                {
                    destinatario = (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[i]);
                    if (destinatario.tipoIE != null && (destinatario.tipoIE == "I" || destinatario.tipoIE == "E"))
                    {
                        if (destinatario.dta_fine == null || (destinatario.dta_fine != null && destinatario.dta_fine == ""))//se il destinatario è abilitato(caso in cui sia interno)
                        {
                            destinatariAbilitati.Add(destinatario);
                            string annullato = string.Empty;
                            bool rtn = CtrlIfDestWithRicevuta(destinatario, out annullato);
                            if (rtn)
                            {
                                if (annullato == string.Empty)
                                    this.lbx_dest.Items.Add("(*) " + UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                                else if (annullato == "1")
                                    this.lbx_dest.Items.Add("(*)(A)  " + UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                                else if (annullato == "E")
                                    this.lbx_dest.Items.Add("(*)(!)  " + UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                            }
                            else
                                this.lbx_dest.Items.Add(UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                            this.lbx_dest.Items[(destinatariAbilitati.Count) - 1].Value = destinatario.codiceRubrica;
                        }
                        else
                        {
                            corrDisable += "\\n- " + destinatario.descrizione;
                        }
                    }
                    else
                    {
                        destinatario = (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[i]);
                        destinatariAbilitati.Add(destinatario);
                        string annullato = string.Empty;
                        bool rtn = CtrlIfDestWithRicevuta(destinatario, out annullato);
                        if (rtn)
                        {
                            if (annullato == string.Empty)
                                this.lbx_destCC.Items.Add("(*) " + UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                            else if (annullato == "1")
                                this.lbx_destCC.Items.Add("(*)(A)  " + UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                            else if (annullato == "E")
                                this.lbx_destCC.Items.Add("(*)(!)  " + UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                        }
                        else
                            this.lbx_dest.Items.Add(UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                        this.lbx_dest.Items[(destinatariAbilitati.Count) - 1].Value = destinatario.codiceRubrica;
                    }
                }

                DocsPaWR.Corrispondente[] corrispondenti = new DocsPAWA.DocsPaWR.Corrispondente[destinatariAbilitati.Count];
                destinatariAbilitati.CopyTo(corrispondenti);
                destinatariAbilitati = null;
                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari = corrispondenti;
                corrispondenti = null;
            }

            if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza != null)
            {
                destinatariAbilitati = new ArrayList();
                for (int i = 0; i < ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza.Length; i++)
                {
                    destinatario = (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza[i]);
                    if (destinatario.tipoIE != null && destinatario.tipoIE == "I")
                    {
                        if (destinatario.dta_fine == null || (destinatario.dta_fine != null && destinatario.dta_fine == ""))//se il destinatario è abilitato(caso in cui sia interno)
                        {
                            destinatariAbilitati.Add(destinatario);
                            string annullato = string.Empty;
                            bool rtn = CtrlIfDestWithRicevuta(destinatario, out annullato);
                            if (rtn)
                            {
                                if (annullato == string.Empty)
                                    this.lbx_destCC.Items.Add("(*) " + UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                                else if (annullato == "1")
                                    this.lbx_destCC.Items.Add("(*)(A)  " + UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                                else if (annullato == "E")
                                    this.lbx_destCC.Items.Add("(*)(!)  " + UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                            }
                            else
                                this.lbx_destCC.Items.Add(UserManager.getDecrizioneCorrispondenteSemplice(destinatario));

                            this.lbx_destCC.Items[(destinatariAbilitati.Count) - 1].Value = destinatario.codiceRubrica;
                        }
                        else
                        {
                            corrDisable += "\\n- " + destinatario.descrizione;
                        }
                    }
                    else
                    {

                        destinatario = (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza[i]);
                        destinatariAbilitati.Add(destinatario);
                        string annullato = string.Empty;
                        bool rtn = CtrlIfDestWithRicevuta(destinatario, out annullato);
                        if (rtn)
                        {
                            if (annullato == string.Empty)
                                this.lbx_destCC.Items.Add("(*) " + UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                            else if (annullato == "1")
                                this.lbx_destCC.Items.Add("(*)(A)  " + UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                            else if (annullato == "E")
                                this.lbx_destCC.Items.Add("(*)(!)  " + UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                        }
                        else
                            this.lbx_destCC.Items.Add(UserManager.getDecrizioneCorrispondenteSemplice(destinatario));
                        this.lbx_destCC.Items[(destinatariAbilitati.Count) - 1].Value = destinatario.codiceRubrica;
                    }
                }

                DocsPaWR.Corrispondente[] corrispondenti = new DocsPAWA.DocsPaWR.Corrispondente[destinatariAbilitati.Count];
                destinatariAbilitati.CopyTo(corrispondenti);
                destinatariAbilitati = null;
                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza = corrispondenti;
                corrispondenti = null;
            }
            Session["corr_disabled"] = corrDisable;
        }


        private DocsPAWA.DocsPaWR.Corrispondente setMittenteAbilitato(DocsPAWA.DocsPaWR.Corrispondente mitt)
        {
            //Valido per tutti i protocolli in A/P/I
            DocsPaWR.Corrispondente corrMitt = null;
            string mittDisabilitati = "";

            if (mitt.tipoIE != null && (mitt.tipoIE == "I" || mitt.tipoIE == "E"))// se il mitt è interno
            {
                if (mitt.dta_fine == null || (mitt.dta_fine != null && mitt.dta_fine == ""))//se il mittente è abilitato
                {
                    corrMitt = mitt;
                }
                else
                {
                    mittDisabilitati += "\\n- " + mitt.descrizione;
                }
            }
            else
            {
                // se il mitt è esterno non devo fare nessun controllo
                corrMitt = mitt;
            }

            Session["corr_disabled"] = mittDisabilitati;
            return corrMitt;
        }

        private DocsPAWA.DocsPaWR.Corrispondente setMittenteIntermedioAbilitato(DocsPAWA.DocsPaWR.Corrispondente mittInt, string corrDisable)
        {
            //Valido per tutti i documenti in ARRIVO
            DocsPaWR.Corrispondente corrMittInt = null;

            if (mittInt.tipoIE != null && mittInt.tipoIE == "I")// se il mitt è interno
            {
                if (mittInt.dta_fine == null || (mittInt.dta_fine != null && mittInt.dta_fine == ""))//se il mittente è abilitato
                {
                    corrMittInt = mittInt;
                }
                else
                {
                    corrDisable += "\\n- " + mittInt.descrizione;
                }
            }
            else
            {
                // se il mitt è esterno non devo fare nessun controllo
                corrMittInt = mittInt;
            }

            Session["corr_disabled"] = corrDisable;
            return corrMittInt;
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        /// <returns></returns>
        protected bool CtrlIfDestWithRicevuta(DocsPAWA.DocsPaWR.Corrispondente dest, out string annullato)
        {
            bool rtn = false;
            annullato = string.Empty;
            DocsPaWR.ProtocolloDestinatario[] protoDest;
            try
            {
                DocsPaWR.SchedaDocumento scheda = DocumentManager.getDocumentoSelezionato(this);
                if (scheda == null || string.IsNullOrEmpty(scheda.systemId) || dest == null || (dest != null && dest.systemId == null))
                {
                    return rtn;
                }
                DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(DocumentManager.getDocumentoSelezionato(this));
                protoDest = DocumentManager.getDestinatariInteropAggConferma(this, infoDoc.idProfile, dest);

                if (protoDest != null)
                {
                    for (int i = 0; i < protoDest.Length; i++)
                    {
                        if (protoDest[i].annullato != null && protoDest[i].annullato.Equals("E"))
                        {
                            rtn = true;
                            annullato = "E";
                        }

                        if (protoDest[i] != null && protoDest[i].dataProtocolloDestinatario != null && !protoDest[i].dataProtocolloDestinatario.Equals(""))
                        {
                            if (protoDest[i].descrizioneCorr == dest.descrizione)
                                rtn = true;
                            if (protoDest[i].annullato != null && protoDest[i].annullato.Equals("1"))
                                annullato = "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {	//return rtn;
                ErrorManager.redirect(this, ex, "protocollazione");
            }
            return rtn;
        }

        protected bool CtrlIfDestWithRicevuta_NEW(DocsPAWA.DocsPaWR.Corrispondente dest, out string annullato)
        {
            bool rtn = false;
            annullato = string.Empty;
            DocsPaWR.ProtocolloDestinatario protoDest;
            try
            {
                DocsPaWR.SchedaDocumento scheda = DocumentManager.getDocumentoSelezionato(this);
                if (scheda == null || scheda.systemId == null || dest == null || (dest != null && dest.systemId == null))
                {
                    return rtn;
                }
                //if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari != null)

                protoDest = (DocsPAWA.DocsPaWR.ProtocolloDestinatario)dest.protocolloDestinatario;
                if (protoDest != null)
                {
                    if (protoDest.annullato != null && protoDest.annullato.Equals("E"))
                    {
                        rtn = true;
                        annullato = "E";
                    }
                }

                if (protoDest != null && protoDest.dataProtocolloDestinatario != null && !protoDest.dataProtocolloDestinatario.Equals(""))
                {
                    if (protoDest.descrizioneCorr == dest.descrizione)
                        rtn = true;
                    if (protoDest.annullato != null && protoDest.annullato.Equals("1"))
                        annullato = "1";

                }
            }
            catch (Exception ex)
            {	//return rtn;
                ErrorManager.redirect(this, ex, "protocollazione");
            }
            return rtn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string CheckDate()
        {
            //Se il registro è giallo e il documento è nuovo
            //controllo sul range: la data del protocollo deve essere > data ultimo protocollo e < data odierna			
            string d_reg = "";

            //reperisco la data secondo il registro
            DocsPaWR.Registro reg = schedaDocumento.registro;
            if (reg != null)
                d_reg = Utils.getMaxDate(reg.dataApertura, reg.dataUltimoProtocollo);
            else
            {
                string msg = "Si è verificato un errore nel reperimento delle informazioni";
                return msg;
            }

            //reperisco la data odierna
            DateTime dt_cor = DateTime.Now;
            CultureInfo ci = new CultureInfo("it-IT");
            string[] formati = { "dd/MM/yyyy" };

            DateTime dt_reg = DateTime.ParseExact(d_reg, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

            DateTime dt_segn = DateTime.ParseExact(this.txt_dataSegn.Text, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

            if (dt_segn.CompareTo(dt_reg) < 0 || dt_segn.CompareTo(dt_cor) > 0)
            {
                return "Data segnatura non valida";
            }
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private string CheckFields(string action)
        {
            string msg = "";

            // action:  S = salva   -  P = protocolla

            //controllo la data solo se sto protocollando
            if (action.Equals("P"))
            {

                //tipo atto se obligatorio
                string idAmm = UserManager.getInfoUtente().idAmministrazione;
                if (DocumentManager.GetTipoDocObbl(idAmm).Equals("1"))
                    //if (System.Configuration.ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"] != null
                    //    && System.Configuration.ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"] == "1")
                    if (this.ddl_tipoAtto.Enabled && this.ddl_tipoAtto.SelectedIndex == 0)
                        msg = "Inserire un valore per il campo Tipologia documento.";


                //data segnatura obbligatoria 
                if (this.txt_dataSegn.Text.Equals(""))
                {
                    msg = "Data di segnatura non presente";
                    return msg;
                }

                //controllo sulla data
                if (this.txt_dataSegn.Text.Length > 0)
                {
                    if (!Utils.isDate(this.txt_dataSegn.Text))
                    {
                        msg = "Errore nel formato della data di Segnatura";
                        return msg;
                    }
                }


                //Controllo su ora pervenuto
                if (this.txt_OraPervenuto_P.Text.Length > 0)
                {
                    if (!Utils.isTime(this.txt_OraPervenuto_P.Text))
                    {
                        msg = "Orario Pervenuto non valido";
                        return msg;
                    }
                }



                //controllo sul range di date
                if (msg == "") // se c'è già un errore è inutile controllare anche le date
                    if (!this.txt_dataSegn.ReadOnly)
                    {
                        msg = this.CheckDate();
                        if (!msg.Equals(""))
                            return msg;
                    }

                //controllo sulla data di arrivo
                //if (this.txt_DataArrivo_P.Text.Length > 0)
                if (this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text.Length > 0)
                {
                    //if (!Utils.isDate(this.txt_DataArrivo_P.Text))
                    if (!Utils.isDate(this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text))
                    {
                        msg = "Errore nel formato della data di arrivo. \\nIl formato richiesto è gg/mm/aaaa";
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_DataArrivo_P.ID + "').focus() </SCRIPT>";
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_DataArrivo_P").txt_Data.ID + "').focus() </SCRIPT>";
                        //RegisterStartupScript("focus", s);
                        return msg;
                    }

                    //if (!this.txt_DataArrivo_P.Text.Equals(this.txt_dataSegn.Text))
                    if (!this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text.Equals(this.txt_dataSegn.Text))
                    {
                        //if (Utils.getMaxDate(this.txt_DataArrivo_P.Text, this.txt_dataSegn.Text).Equals(this.txt_DataArrivo_P.Text))
                        if (Utils.getMaxDate(this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text, this.txt_dataSegn.Text).Equals(this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text))
                        {
                            msg = "La data di arrivo deve essere minore della data di segnatura";
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_DataArrivo_P.ID + "').focus() </SCRIPT>";
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_DataArrivo_P").txt_Data.ID + "').focus() </SCRIPT>";
                            //RegisterStartupScript("focus", s);
                            return msg;
                        }
                    }
                }
            }

            if (this.ctrl_oggetto.oggetto_text != null)
            {
                if (this.ctrl_oggetto.oggetto_text.Trim().Length == 0)
                {
                    msg = "Inserire il valore: oggetto";
                    // ctrl_oggetto.oggetto_SetControlFocus();
                    return msg;
                }
            }

            // controllo sulla lunghezza max dell'oggetto (2000 car.)
            //if (this.ctrl_oggetto.oggetto_text.Length > 2000)
            //{
            //    msg = "Consentita lunghezza massima di 2000 caratteri per il campo oggetto";
            //    ctrl_oggetto.oggetto_SetControlFocus();
            //    return msg;
            //}
            //else
            //    if (dettaglioNota.Testo.Length > 2000)
            //    {
            //        msg = "Consentita lunghezza massima di 2000 caratteri per il campo note";
            //        return msg;
            //    }

            switch (this.rbl_InOut_P.SelectedItem.Value)
            {
                case "In":
                    //controllo sull'inserimento del mittente
                    if ((this.txt_CodMit_P.Text.Trim().Equals("") || this.txt_CodMit_P.Text == null) || (this.txt_DescMit_P.Text.Trim().Equals("") || this.txt_DescMit_P.Text == null))
                    {
                        if (this.txt_DescMit_P.Text.Trim().Equals("") || this.txt_DescMit_P.Text == null)
                        {
                            msg = "Inserire il valore: mittente";
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_CodMit_P.ID + "').focus() </SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return msg;
                        }
                    }

                    if (this.GetCalendarControl("txt_DataProtMit_P").txt_Data.Text.Length > 0)
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_DataProtMit_P").txt_Data.Text))
                        {
                            msg = "Errore nel formato della data del Prot Mittente";
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_DataProtMit_P").txt_Data.ID + "').focus() </SCRIPT>";
                            //RegisterStartupScript("focus", s);
                            return msg;
                        }
                    }

                    if (this.GetCalendarControl("txt_DataProtMit_P").txt_Data.Text != "")
                    {
                        if ((Convert.ToDateTime(this.GetCalendarControl("txt_DataProtMit_P").txt_Data.Text)) > Convert.ToDateTime(txt_dataSegn.Text))
                        {
                            msg = "La data del protocollo mittente deve essere minore della data protocollo";
                            // string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_DataArrivo_P.ID + "').focus() </SCRIPT>";
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_DataArrivo_P").txt_Data.ID + "').focus() </SCRIPT>";
                            //RegisterStartupScript("focus", s);
                            return msg;
                        }
                    }

                    //controllo sulla data di arrivo
                    //if (this.txt_DataArrivo_P.Text.Length > 0)
                    if (this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text.Length > 0)
                    {
                        //if (!Utils.isDate(this.txt_DataArrivo_P.Text))
                        if (!Utils.isDate(this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text))
                        {
                            msg = "Errore nel formato della data di arrivo. \\nIl formato richiesto è gg/mm/aaaa";
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_DataArrivo_P.ID + "').focus() </SCRIPT>";
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_DataArrivo_P").txt_Data.ID + "').focus() </SCRIPT>";
                            //RegisterStartupScript("focus", s);
                            return msg;
                        }
                    }

                    if (this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text.Length > 0 && this.GetCalendarControl("txt_DataProtMit_P").txt_Data.Text.Length > 0)
                    {
                        if (!Utils.verificaIntervalloDateSenzaOra(this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text, this.GetCalendarControl("txt_DataProtMit_P").txt_Data.Text))
                        {
                            msg = "La data di arrivo deve essere maggiore della data di protocollo mittente";
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_DataArrivo_P.ID + "').focus() </SCRIPT>";
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_DataArrivo_P").txt_Data.ID + "').focus() </SCRIPT>";
                            //RegisterStartupScript("focus", s);
                            return msg;
                        }
                    }

                    //Controllo su ora pervenuto
                    if (this.txt_OraPervenuto_P.Text.Length > 0)
                    {
                        if (!Utils.isTime(this.txt_OraPervenuto_P.Text))
                        {
                            msg = "Orario Pervenuto non valido";
                            return msg;
                        }
                        if (this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text.Length == 0)
                        {
                            msg = "Inserire anche la data arrivo";
                            return msg;
                        }
                    }

                    break;
                case "Out":
                    //protocollo in uscita - controllo sui destinatari
                    if (this.lbx_dest.Items.Count <= 0)
                    {
                        msg = "Inserire il valore: destinatario";
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_CodDest_P.ID + "').focus() </SCRIPT>";
                        RegisterStartupScript("focus", s);
                        return msg;
                    }

                    break;
                case "Own":
                    //protocollo interno - controllo sul mittente obbligatorio
                    if (this.txt_CodMit_P.Text.Equals("") || this.txt_CodMit_P.Text == null)
                    {
                        if (this.txt_DescMit_P.Text.Equals("") || this.txt_DescMit_P.Text == null)
                        {
                            msg = "Inserire il valore: mittente";
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_CodMit_P.ID + "').focus() </SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return msg;
                        }
                    }
                    //protocollo interno - controllo sui destinatari
                    if (this.lbx_dest.Items.Count <= 0)
                    {
                        msg = "Inserire il valore: destinatario";
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_CodDest_P.ID + "').focus() </SCRIPT>";
                        RegisterStartupScript("focus", s);
                        return msg;
                    }
                    break;
            }

            //controllo sull'inserimento del mittente
            if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
            {
                if ((this.txt_cod_uffRef.Text.Equals("") || this.txt_cod_uffRef.Text == null) || (this.txt_desc_uffRef.Text.Equals("") || this.txt_desc_uffRef.Text == null))
                {

                    msg = "Inserire il valore: ufficio referente";
                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_cod_uffRef.ID + "').focus() </SCRIPT>";
                    RegisterStartupScript("focus", s);
                    return msg;
                }
                else
                {
                    if (!CheckUoReferente(schedaDocumento.tipoProto))
                    {
                        if (schedaDocumento.tipoProto.Equals("A"))
                        {
                            this.txt_cod_uffRef.ReadOnly = false;
                            //							this.txt_desc_uffRef.ReadOnly = false;
                            this.txt_cod_uffRef.BackColor = Color.White;
                            this.txt_desc_uffRef.BackColor = Color.White;
                            this.btn_Rubrica_ref.Enabled = true;
                            this.btn_Rubrica_ref.Attributes.Add("onclick", "ApriRubrica('proto','U');");
                        }

                        msg = "L\\'Ufficio Referente non possiede ruoli di riferimento.";
                        if (action.Equals("S"))
                        {
                            msg = msg + "\\nIl salvataggio dei dati non verrà effettuato";
                        }
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_cod_uffRef.ID + "').focus() </SCRIPT>";
                        RegisterStartupScript("focus", s);

                        return msg;
                    }
                }
            }

            //Profilazione Dinamica
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
            {
                if (ProfilazioneDocManager.verificaCampiObbligatori((DocsPAWA.DocsPaWR.Templates)Session["template"]))
                {
                    msg = "Ci sono dei campi obbligatori relativi al tipo di documento selezionato !";
                    return msg;
                }

                string messag = ProfilazioneDocManager.verificaOkContatore((DocsPAWA.DocsPaWR.Templates)Session["template"]);
                if (messag != string.Empty)
                    return messag;
            }

            return msg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_add_ADL_Click(object sender, System.EventArgs e)
        {
            //aggiunde il documento all'area di lavoro
            DocumentManager.addAreaLavoro(this, schedaDocumento);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CheckDestinatari()
        {
            bool result = true;
            DocsPaWR.DocsPaWebService docsPaWS = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPaWR.ProtocolloInterno pi = (DocsPAWA.DocsPaWR.ProtocolloInterno)this.schedaDocumento.protocollo;

            // Controlla destinatari
            foreach (DocsPAWA.DocsPaWR.Corrispondente destinatario in pi.destinatari)
            {
                if (destinatario.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
                {
                    if (!docsPaWS.UOHasReferenceRole(destinatario.systemId))
                    {
                        //Prova Andrea
                        //result = false;
                        //End Andrea
                        break;
                    }
                }
            }

            // Controlla destinatari CC
            if (result)
            {
                if (pi.destinatariConoscenza != null)
                {
                    foreach (DocsPAWA.DocsPaWR.Corrispondente destinatario in pi.destinatariConoscenza)
                    {
                        if (destinatario.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
                        {
                            if (!docsPaWS.UOHasReferenceRole(destinatario.systemId))
                            {
                                result = false;
                                break;
                            }
                        }
                    }
                }
            }

            docsPaWS.Dispose();

            return result;
        }


        private bool CheckUoReferente(string tipoProto)
        {
            bool result = true;
            DocsPaWR.DocsPaWebService docsPaWS = new DocsPAWA.DocsPaWR.DocsPaWebService();
            if (tipoProto.Equals("A"))
            {
                DocsPaWR.ProtocolloEntrata pa = (DocsPAWA.DocsPaWR.ProtocolloEntrata)this.schedaDocumento.protocollo;
                // Controlla se l'ufficio referente possiede dei ruoli di riferimento
                if (pa.ufficioReferente.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
                {
                    if (!docsPaWS.UOHasReferenceRole(pa.ufficioReferente.systemId))
                    {
                        result = false;
                    }
                }
            }
            if (tipoProto.Equals("P"))
            {
                DocsPaWR.ProtocolloUscita pu = (DocsPAWA.DocsPaWR.ProtocolloUscita)this.schedaDocumento.protocollo;
                // Controlla se l'ufficio referente possiede dei ruoli di riferimento
                if (pu.ufficioReferente.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
                {
                    if (!docsPaWS.UOHasReferenceRole(pu.ufficioReferente.systemId))
                    {
                        result = false;
                    }
                }
            }
            if (tipoProto.Equals("I"))
            {
                DocsPaWR.ProtocolloInterno pi = (DocsPAWA.DocsPaWR.ProtocolloInterno)this.schedaDocumento.protocollo;

                // Controlla se l'ufficio referente possiede dei ruoli di riferimento
                if (pi.ufficioReferente.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
                {
                    if (!docsPaWS.UOHasReferenceRole(pi.ufficioReferente.systemId))
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private void protocollaDoc()
        {

            string checkInteroperante = string.Empty;
            string checkSameMail = string.Empty;
            logger.Info("BEGIN");
            if (Session["templateRiproposto"] != null)
                Session.Remove("templateRiproposto");

            if (Session["abilitaModificaSpedizione"] != null && (bool)Session["abilitaModificaSpedizione"])
                Session.Remove("abilitaModificaSpedizione");

            if (this.schedaDocumento.checkOutStatus != null)
            {
                Response.Write("<script>alert('Impossibile protocollare il documento in quanto risulta bloccato');</script>");
            }
            else
            {
                //PROFILAZIONE DINAMICA
                if (ProfilazioneDocManager.verificaCampiObbligatori((DocsPAWA.DocsPaWR.Templates)Session["template"]))
                {
                    RegisterStartupScript("alert", "<script>alert('Ci sono dei campi obbligatori relativi al tipo di documento selezionato !');</script>");
                    return;
                }

                string messag = ProfilazioneDocManager.verificaOkContatore((DocsPAWA.DocsPaWR.Templates)Session["template"]);
                if (messag != string.Empty)
                {
                    RegisterStartupScript("alertContatore", "<script>alert('" + messag + "');</script>");
                    return;
                }
                //FINE PROFILAZIONE DINAMICA  	


                //controllo sulla fascicolazione obbligatoria
                if (this.isFascRapidaRequired)
                {
                    if (this.schedaDocumento.systemId == null)
                    {
                        if (this.txt_CodFascicolo.Text.Equals("") && this.txt_DescFascicolo.Text.Equals(""))
                        {
                            RegisterStartupScript("alert", "<script>alert('La fascicolazione rapida è obbligatoria !');</script>");
                            return;
                        }
                    }
                    else
                    {
                        // controllo su obbligatorietà della fascicolazione e chiamata al web service
                        // che ci da informazione se il documento è già stato o meno fascicolato
                        if (!DocumentManager.getSeDocFascicolato(this, schedaDocumento) && string.IsNullOrEmpty(this.txt_CodFascicolo.Text))
                        {
                            RegisterStartupScript("alert", "<script>alert('La fascicolazione rapida è obbligatoria !');</script>");
                            return;
                        }
                    }
                }
                // fine controllo fascicolazione obbligatoria


                DocsPaWR.Registro[] listaRF = UserManager.getListaRegistriWithRF(this, "1", schedaDocumento.registro.systemId);
                bool okRF = true;

                // verifico se è stata selezionata una nota di RF e se si sia selezionato un RF corretto nel caso di utenti con 2 RF almeno
                if (this.NotaDocumentoEnabled && listaRF != null && listaRF.Length > 1 && (dettaglioNota.TipoVisibilita == TipiVisibilitaNotaEnum.RF))
                {
                    try
                    {
                        this.dettaglioNota.Save();
                    }
                    catch (Exception exc)
                    {
                        okRF = false;
                        RegisterStartupScript("alertRFNote", "<script>alert('" + exc.Message + "');</script>");
                    }
                }

                if (wws.isEnableRiferimentiMittente() && rbl_InOut_P.SelectedItem.Value == "In")
                {
                    schedaDocumento.riferimentoMittente = txt_riferimentoMittente.Text;
                }

                // verifico che sia richiesta la presenza del codice RF nella segnatura
                // se è presente e se il registro del documento ha 1 o più RF associati,
                // apro la popup di selezione RF (sceltaRFSegnatura.aspx)
                if (okRF && ConfigurationManager.AppSettings["ENABLE_CODBIS_SEGNATURA"] != null && ConfigurationManager.AppSettings["ENABLE_CODBIS_SEGNATURA"] == "1")
                {
                    // se nella segnatura è stato richiesto il codice RF
                    InfoAmministrazione infAmm = wws.AmmGetInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione);
                    if (infAmm.Segnatura.Contains("COD_RF_PROT"))
                    {
                        // se ci sono uno o più RF associati al registro

                        if (listaRF != null && listaRF.Length == 1) //se un solo RF non apro popup, ma selec direttamente.
                        {
                            schedaDocumento.id_rf_prot = listaRF[0].systemId;
                            //c'è un solo RF, quindi la ricevuta la posso inviare solo con questo.
                            schedaDocumento.id_rf_invio_ricevuta = listaRF[0].systemId;

                            schedaDocumento.cod_rf_prot = listaRF[0].codRegistro;
                            DocumentManager.setDocumentoSelezionato(schedaDocumento);
                            //RegisterStartupScript("apriRFSegnatura", "<script>apriRFSegnatura('segnatura', " + this.ddl_tmpl.SelectedIndex + ", '" + this.txt_CodFascicolo.Text + "');</script>");
                        }
                        else
                            if (listaRF != null && listaRF.Length > 1)
                            {
                                bool continua = true;
                                //devo verificare che non si tratti di un documento scaricato via pec
                                if(schedaDocumento.interop != null && (schedaDocumento.interop.Equals("S") || schedaDocumento.interop.Equals("P") || schedaDocumento.interop.Equals("E")))
                                {
                                    // vado a prendere il riferimento del RF in cui è stato scaricato
                                    continua = false;
                                    DocsPaWR.Registro registroPec = UserManager.getRegistroDaPec(schedaDocumento.systemId, this.Page);
                                    if (registroPec != null && !string.IsNullOrEmpty(registroPec.systemId) && !string.IsNullOrEmpty(registroPec.codRegistro))
                                    {
                                        schedaDocumento.id_rf_prot = registroPec.systemId;
                                        schedaDocumento.id_rf_invio_ricevuta = registroPec.systemId;
                                        schedaDocumento.cod_rf_prot = registroPec.codRegistro;
                                        DocumentManager.setDocumentoSelezionato(schedaDocumento);
                                    }
                                }

                                if (continua && dettaglioNota.TipoVisibilita == TipiVisibilitaNotaEnum.RF)
                                {
                                    if (Session["RFNote"] == null)
                                    {
                                        if (this.dettaglioNota != null && !this.dettaglioNota.Testo.Equals(""))
                                        {
                                            Session.Add("dettaglioNota", this.dettaglioNota);
                                        }

                                        if (ddl_tmpl != null)
                                            Session.Add("modelloTrasmCodBisSegn", ddl_tmpl.SelectedIndex);
                                        //Nel caso in cui il ruolo dell'utente appartiene a più rf
                                        //e dalle note non si è selezionato ancora un rf si apri le popup
                                        //per la selezione del rf
                                        DocumentManager.setDocumentoSelezionato(schedaDocumento);
                                        if (this.ddl_tmpl != null && this.ddl_tmpl.SelectedIndex > 0)
                                            RegisterStartupScript("apriRFSegnatura", "<script>apriRFSegnatura('segnatura','" + ddl_tmpl.SelectedIndex + "');</script>");
                                        else
                                            RegisterStartupScript("apriRFSegnatura", "<script>apriRFSegnatura('segnatura','0');</script>");
                                        return;
                                    }
                                    else
                                    {
                                        //Il ruolo appartiene a più rf e già dalle note si è selezionato
                                        //un rf
                                        string[] mySplitResult = Session["RFNote"].ToString().Split('^');
                                        if (mySplitResult[0] == "OK")
                                        {
                                            this.hdnCodRFSegnatura.Value = mySplitResult[2];
                                            this.hdnIdRFSegnatura.Value = mySplitResult[1];
                                            string[] codRF_ar = mySplitResult[2].Split('-');
                                            string codRF = "";
                                            codRF_ar[0] = codRF_ar[0].Trim();
                                            if (codRF_ar[0] != "")
                                            {
                                                codRF = codRF_ar[0];
                                            }
                                            else
                                            {
                                                codRF = mySplitResult[2];
                                            }
                                            schedaDocumento.id_rf_prot = mySplitResult[1];
                                            schedaDocumento.cod_rf_prot = codRF;
                                            DocumentManager.setDocumentoSelezionato(schedaDocumento);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                #region codice commentato
                                //    if (listaRF != null && listaRF.Length > 1)
                                //    {
                                //        bool contDoubleRf = true;
                                //        if (schedaDocumento.tipoProto == "P")
                                //        {
                                //            if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari == null || ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari.Length == 0)
                                //            {
                                //                contDoubleRf = false;
                                //            }
                                //        }

                                //        if (schedaDocumento.tipoProto == "I")
                                //        {
                                //            if (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatari == null || ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatari.Length == 0)
                                //            {
                                //                contDoubleRf = false;
                                //            }

                                //        }

                                //        if (contDoubleRf)
                                //        {
                                //            if (this.dettaglioNota != null && !this.dettaglioNota.Testo.Equals(""))
                                //            {
                                //                Session.Add("dettaglioNota", this.dettaglioNota);
                                //            }
                                //            if (ddl_tmpl != null)
                                //                Session.Add("modelloTrasmCodBisSegn", ddl_tmpl.SelectedIndex);
                                //            DocumentManager.setDocumentoSelezionato(schedaDocumento);
                                //            RegisterStartupScript("apriRFSegnatura", "<script>apriRFSegnatura('segnatura');</script>");
                                //        }
                                //        else
                                //        {
                                //            ClientScript.RegisterStartupScript(this.GetType(), "destinatario", "alert('Attenzione! Selezionare almeno un destinatario');", true);
                                //        }
                                //        return;
                                //    }
                                //}
                                # endregion
                            }
                    }
                }

                //1)verifico che si tratti di un predisposto
                if (okRF && schedaDocumento.protocollo != null && string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                {
                    //2) verifico cha siamo nel caso di interoperabilità
                    //OLD
                    //if (schedaDocumento.descMezzoSpedizione != null
                    //    && (schedaDocumento.descMezzoSpedizione.ToUpper().Equals("INTEROPERABILITA")
                    //    || schedaDocumento.descMezzoSpedizione.ToUpper().Equals("MAIL")))

                    if (
                        (!string.IsNullOrEmpty(schedaDocumento.typeId)
                            && (schedaDocumento.typeId.ToUpper().Equals("INTEROPERABILITA")
                       || schedaDocumento.typeId.ToUpper().Equals("MAIL"))) ||

                       (schedaDocumento.descMezzoSpedizione != null
                        && (schedaDocumento.descMezzoSpedizione.ToUpper().Equals("INTEROPERABILITA")
                        || schedaDocumento.descMezzoSpedizione.ToUpper().Equals("MAIL")))

                        )
                    {
                        // 3) verifico che il registro abbia settato da amministrazione l'invio manuale o meno della ricevuta 
                        if (schedaDocumento.registro.invioRicevutaManuale.ToUpper().Equals("1"))
                        {
                            // 4) calcolo gli RF associati al registro
                            //DocsPaWR.Registro[] listaRF = UserManager.getListaRegistriWithRF(this, "1", schedaDocumento.registro.systemId);
                            if (listaRF != null && listaRF.Length > 0)
                            {
                                // 5) nel caso di un solo RF associato al registro
                                if (listaRF.Length == 1 && !listaRF[0].invioRicevutaManuale.ToUpper().Equals("1"))
                                {
                                    schedaDocumento.id_rf_invio_ricevuta = listaRF[0].systemId;
                                }
                                else
                                {
                                    // 6) caso di più RF associati al registro e con invio automatico
                                    if (listaRF.Length > 1)
                                    {
                                        bool daMostrarePopUp = false;
                                        for (int i = 0; i < listaRF.Length; i++)
                                        {
                                            if (!listaRF[i].invioRicevutaManuale.ToUpper().Equals("1"))
                                                daMostrarePopUp = true;
                                        }
                                        //se il documento arriva da interop o da mail, allora è stato scaricato da un reg o rf, quindi lo devo protocollare su questo.
                                        //Andrea De Marco - Modifica per MEV Gestione Eccezioni PEC - Commentare De Marco per ripristino e decommentare il codice sottostante
                                        if (schedaDocumento.interop == "S" || schedaDocumento.interop == "P" || schedaDocumento.interop == "E")
                                        //End De Marco
                                        //if (schedaDocumento.interop == "S" || schedaDocumento.interop == "P")
                                        {
                                            daMostrarePopUp = false;

                                            if (schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloEntrata))
                                            {
                                                string idrf = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente.idRegistro;
                                                schedaDocumento.id_rf_invio_ricevuta = idrf;
                                            }
                                        }

                                        // apro la popup di selezione RF
                                        if (daMostrarePopUp)
                                        {
                                            RegisterStartupScript("apriRFRicevuta", "<script>apriRFRicevuta('ricevuta','','', " + this.ddl_tmpl.SelectedIndex + ",'" + this.txt_CodFascicolo.Text + "');</script>");
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #region CONTROLLI PER RISPOSTA AL DOCUMENTO
                string descrMsg = string.Empty;
                if (!VerificaRispostaDocumento(ref descrMsg))
                {
                    RegisterStartupScript("disabled", "<SCRIPT>alert('" + descrMsg + "');</SCRIPT>");
                    return;
                }

                #endregion

                string msg = CheckFields("P");

                msg += ProfilazioneDocManager.VerifyAndSetTipoDoc(UserManager.getInfoUtente(this), ref schedaDocumento, this);

                if (okRF && string.IsNullOrEmpty(msg))
                {
                    // verifico che sia richiesta la presenza del codice RF nella segnatura
                    // se è presente e se il registro del documento ha 1 o più RF associati,
                    // apro la popup di selezione RF (sceltaRFSegnatura.aspx)
                    if (ConfigurationManager.AppSettings["ENABLE_CODBIS_SEGNATURA"] != null && ConfigurationManager.AppSettings["ENABLE_CODBIS_SEGNATURA"] == "1")
                    {
                        // se nella segnatura è stato richiesto il codice RF
                        InfoAmministrazione infAmm = wws.AmmGetInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione);
                        if (infAmm.Segnatura.Contains("COD_RF_PROT"))
                        {
                            // se ci sono uno o più RF associati al registro
                            //DocsPaWR.Registro[] listaRF = UserManager.getListaRegistriWithRF(this, "1", schedaDocumento.registro.systemId);
                            if (listaRF != null && listaRF.Length == 1) //se un solo RF non apro popup, ma selec direttamente.
                            {
                                schedaDocumento.id_rf_prot = listaRF[0].systemId;
                                schedaDocumento.cod_rf_prot = listaRF[0].codRegistro;
                                DocumentManager.setDocumentoSelezionato(schedaDocumento);
                                //RegisterStartupScript("apriRFSegnatura", "<script>apriRFSegnatura('segnatura', " + this.ddl_tmpl.SelectedIndex + ", '" + this.txt_CodFascicolo.Text + "');</script>");

                            }
                            else
                            {
                                //Andrea De Marco - Modifica per mev Gestione Eccezioni PEC - Commentare De Marco per Ripristino e decommentare codice sottostante
                                if (listaRF != null && listaRF.Length > 1 && Session["RFNote"] == null
                                    && !(schedaDocumento.interop == "S" || schedaDocumento.interop == "P" || schedaDocumento.interop == "E"))
                                //End De Marco
                                //if (listaRF != null && listaRF.Length > 1 && Session["RFNote"] == null
                                //    && !(schedaDocumento.interop == "S" || schedaDocumento.interop == "P"))
                                {
                                    if (this.dettaglioNota != null && !this.dettaglioNota.Testo.Equals(""))
                                    {
                                        Session.Add("dettaglioNota", this.dettaglioNota);
                                    }
                                    if (ddl_tmpl != null)
                                        Session.Add("modelloTrasmCodBisSegn", ddl_tmpl.SelectedIndex);
                                    DocumentManager.setDocumentoSelezionato(schedaDocumento);
                                    RegisterStartupScript("apriRFSegnatura", "<script>apriRFSegnatura('segnatura', '" + this.ddl_tmpl.SelectedIndex + "');</script>");
                                    return;
                                }
                                else
                                {
                                    bool continua = true;
                                    //devo verificare che non si tratti di un documento scaricato via pec
                                    if (schedaDocumento.interop != null && (schedaDocumento.interop.Equals("S") || schedaDocumento.interop.Equals("P") || schedaDocumento.interop.Equals("E")))
                                    {
                                        // vado a prendere il riferimento del RF in cui è stato scaricato
                                        continua = false;
                                        DocsPaWR.Registro registroPec = UserManager.getRegistroDaPec(schedaDocumento.systemId, this.Page);
                                        if (registroPec != null && !string.IsNullOrEmpty(registroPec.systemId) && !string.IsNullOrEmpty(registroPec.codRegistro))
                                        {
                                            schedaDocumento.id_rf_prot = registroPec.systemId;
                                            schedaDocumento.id_rf_invio_ricevuta = registroPec.systemId;
                                            schedaDocumento.cod_rf_prot = registroPec.codRegistro;
                                            DocumentManager.setDocumentoSelezionato(schedaDocumento);
                                        }
                                    }

                                    //Andrea De Marco - Modifica per mev Gestione Eccezioni PEC - Commentare De Marco per Ripristino e decommentare codice sottostante
                                    if (continua && listaRF != null && listaRF.Length > 1 && Session["RFNote"] == null
                                    && (schedaDocumento.interop == "S" || schedaDocumento.interop == "P" || schedaDocumento.interop == "E"))
                                    //End De Marco
                                    // if (listaRF != null && listaRF.Length > 1 && Session["RFNote"] == null
                                    //&& (schedaDocumento.interop == "S" || schedaDocumento.interop == "P"))
                                    {
                                        //se il documento arriva da interop o da mail, allora è stato scaricato da un reg o rf, quindi lo devo protocollare su questo.
                                        //Andrea De Marco - Modifica per mev Gestione Eccezioni PEC - Commentare De Marco per Ripristino e decommentare codice sottostante
                                        if (schedaDocumento.interop == "S" || schedaDocumento.interop == "P" || schedaDocumento.interop == "E")
                                        //End Andrea De Marco
                                        //if (schedaDocumento.interop == "S" || schedaDocumento.interop == "P")
                                        {
                                            Registro r = new Registro();
                                            DataSet ds = DocsPAWA.utils.MultiCasellaManager.GetAssDocAddress(schedaDocumento.systemId);
                                            if (ds != null && ds.Tables["ass_doc_rf"].Rows.Count > 0)
                                            {
                                                foreach (DataRow rg in ds.Tables["ass_doc_rf"].Rows)
                                                {
                                                    r = (from c in UserManager.getListaRegistriWithRF(this, "1", schedaDocumento.registro.systemId)
                                                         where c.systemId.Equals(rg["registro"].ToString())
                                                         select c).FirstOrDefault();
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(r.systemId) && (!string.IsNullOrEmpty(r.codRegistro)))
                                            {
                                                schedaDocumento.id_rf_prot = r.systemId;
                                                schedaDocumento.cod_rf_prot = r.codRegistro;
                                            }
                                            //il mittente è su registro TUTTI.. faccio aprire il popup
                                            //RegisterStartupScript("apriRFSegnatura", "<script>apriRFSegnatura('segnatura', '" + this.ddl_tmpl.SelectedIndex + "');</script>");
                                            //return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                ////creazione mittente occasionale
                //creaCorrOccasionalePerProtocollo();
                //string message ="";
                if (msg == "" && this.schedaDocumento.tipoProto == "I")
                {
                    if (!CheckDestinatari()) msg = @"Una o piu\' unita\' organizzative non hanno ruoli di riferimento. Impossibile protocollare il documento.";
                }

                if (msg != "")
                {
                    Response.Write("<script>alert('" + msg + "');</script>");
                    if (msg.StartsWith("Errore nel formato della data di arrivo"))
                    {
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_DataArrivo_P.ID + "').focus() </SCRIPT>";
                        RegisterStartupScript("focus", s);
                    }
                    return;
                }

                DocsPAWA.DocsPaWR.Registro reg = wws.GetRegistroBySistemId(schedaDocumento.registro.systemId);
                if (reg.Sospeso)
                {
                    RegisterStartupScript("alert", "<script>alert('Il registro selezionato è sospeso!');</script>");
                    return;
                }

                if (!this.IsRoleInternalEnabled())
                {
                    if (this.rbl_InOut_P.SelectedItem.Value.Equals("Own"))
                    {
                        if (this.rbl_InOut_P.Items[2] != null && this.rbl_InOut_P.Items.Count > 2 && this.rbl_InOut_P.Items[2].Selected)
                        {
                            // Alert utente disabilitato alla protocollazione interna
                            Response.Write("<script>alert('Utente non abilitato alla protocollazione interna.');</script>");
                            return;
                        }
                    }
                }

                if (!this.IsRoleInwardEnabled())
                {
                    if (this.rbl_InOut_P.SelectedItem.Value.Equals("In"))
                    {
                        if (this.rbl_InOut_P.Items[0].Selected)
                        {
                            // Alert utente disabilitato alla protocollazione ingresso
                            Response.Write("<script>alert('Utente non abilitato alla protocollazione in ingresso.');</script>");
                            return;
                        }
                    }
                }

                if (!this.IsRoleOutwardEnabled())
                {
                    if (this.rbl_InOut_P.SelectedItem.Value.Equals("Out"))
                    {
                        if (rbl_InOut_P.Items[1].Selected)
                        {
                            // Alert utente disabilitato alla protocollazione in uscita
                            Response.Write("<script>alert('Utente non abilitato alla protocollazione in uscita.');</script>");
                            return;
                        }
                    }
                }

                bool protoOk = false;
                string messaggio = "";

                //Aggiunto controllo se il docoumento è privato e si è scelto di fare 
                //una trasmissione rapida o di aggiungere il documento in un fascicolo
                if (schedaDocumento.privato == "1")
                {
                    Fascicolo fasc = getFascicolo();
                    if ((ddl_tmpl.SelectedIndex != 0) && (txt_CodFascicolo.Text != ""))
                    {
                        messaggio = InitMessageXml.getInstance().getMessage("fasc_trasmDocPrivato");
                    }
                    //else if (ddl_tmpl.SelectedIndex != 0)
                    //    messaggio = InitMessageXml.getInstance().getMessage("trasmDocPrivato");
                    else if (txt_CodFascicolo.Text != "" && fasc != null && fasc.tipo != "G")
                        messaggio = InitMessageXml.getInstance().getMessage("insDocPrivato");
                    if (messaggio != "")
                    {
                        protoOk = true;
                        msg_TrasmettiRapida.Confirm(messaggio);
                    }
                }

                bool destTo = false;
                bool destCC = false;

                if ((schedaDocumento.privato == "1") && (schedaDocumento.tipoProto != "A"))
                {

                    //se il protocollo è in uscita verifico che ci siano destinatari interni
                    if (schedaDocumento.tipoProto.Equals("P"))
                    {
                        DocsPaWR.Corrispondente[] listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari;
                        if (listaDest != null && listaDest.Length > 0)
                        {
                            for (int i = 0; i < listaDest.Length; i++)
                            {
                                if (listaDest[i].tipoIE == "I")
                                    destTo = true;
                            }
                            if (!destTo)
                            {
                                DocsPaWR.Corrispondente[] listaDestCC = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza;

                                if (listaDestCC != null && listaDestCC.Length > 0)
                                {
                                    for (int i = 0; i < listaDestCC.Length; i++)
                                    {
                                        if (listaDestCC[i].tipoIE == "I")
                                            destCC = true;
                                    }
                                }
                            }
                        }
                    }
                    //se il protocollo è interno verifico che ci siano destinatari interni
                    if (schedaDocumento.tipoProto.Equals("I"))
                    {
                        DocsPaWR.Corrispondente[] listaDest = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatari;
                        if (listaDest != null && listaDest.Length > 0)
                        {
                            for (int i = 0; i < listaDest.Length; i++)
                            {
                                if (listaDest[i].tipoIE == "I")
                                    destTo = true;
                            }
                            if (!destTo)
                            {
                                DocsPaWR.Corrispondente[] listaDestCC = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatariConoscenza;
                                if (listaDestCC != null && listaDestCC.Length > 0)
                                {
                                    for (int i = 0; i < listaDestCC.Length; i++)
                                    {
                                        if (listaDestCC[i].tipoIE == "I")
                                            destCC = true;
                                    }
                                }
                            }
                        }
                    }
                }

                ////Se sono stati trovati destinatari interni allora verranno effettuate trasmissioni e 
                ////quindi si fa un alert
                //if (destCC || destTo)
                //{
                //    protoOk = true;
                //    messaggio = InitMessageXml.getInstance().getMessage("trasmDocPrivato");
                //    msg_TrasmettiProto.Confirm(messaggio);

                //}
                ///*else
                //{
                //    protocollaOk();
                //}*/

                if (!string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                {
                    if (this.schedaDocumento.tipoProto == "P")
                    {
                        if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente != null)
                        {
                            if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente.tipoCorrispondente == "O")
                            {
                                Response.Write("<script>alert('Non è possibile inserire il mittente come occasionale.');</script>");
                                return;
                            }
                        }
                    }

                    if (this.schedaDocumento.tipoProto == "I")
                    {
                        if (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente != null)
                        {
                            if (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente.tipoCorrispondente == "O")
                            {
                                Response.Write("<script>alert('Non è possibile inserire il mittente come occasionale.');</script>");
                                return;
                            }
                        }
                    }
                }

                if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.interop))
                {

                    #region Controllo Interoperabilità

                    string allRegs = "";
                    //Check chiave interop
                    checkInteroperante =
                        DocsPAWA.utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione,
                                                                      "CHECK_MITT_INTEROPERANTE");

                    // Se il documento è stato ricevuto per IS, K1 viene disablitato
                    if (this.FromInteropPecOrSimpInterop(schedaDocumento) &&
                        (((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente != null &&
                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente.inRubricaComune))
                        checkInteroperante = "0";

                    // Replicato il metodo FromInteropPecOrSimpInterop con aggiunta dei controlli interop=p || interop=e e typeId.equals("MAIL")
                    //In questi casi K1 spento.
                    if (this.FromInteropPecOrSimpInteropOrMail(schedaDocumento) &&
                        (((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente != null &&
                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente.inRubricaComune))
                        checkInteroperante = "0";

                    /* nel caso in cui le mail della casella istituzionale  vengano INOLTRATE da un utente interno, 
                    il mittente del predisposto può risultare interno, in questo caso non deve scattare k1 e k2 quindi
                    metto le chiavi a 0 prima che partano i controlli. */
                    Corrispondente mittIE = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente;
                    /* nel caso in cui le mail della casella istituzionale  vengano INOLTRATE da un utente interno, 
                    il mittente del predisposto può risultare interno, in questo caso non deve scattare k1 e k2 quindi
                    metto le chiavi a 0 prima che partano i controlli. */
                    if ((mittIE != null && (!string.IsNullOrEmpty(mittIE.tipoCorrispondente)) && mittIE.tipoCorrispondente.Equals("O")) ||
                        (mittIE != null && mittIE.tipoIE.Equals("I")) ||
                        (schedaDocumento.tipoProto.Equals("A") && ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente)
                        )
                    {
                        checkInteroperante = "0";
                        checkSameMail = "0";
                        mittIE = null;
                    }
                    if (mittIE != null && mittIE.tipoIE.Equals("I"))
                    {
                        checkInteroperante = "0";
                        checkSameMail = "0";
                        mittIE = null;
                    }

                    //Check chiave stessa mail
                    checkSameMail =
                        DocsPAWA.utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione,
                                                                      "CHECK_MAILBOX_INTEROPERANTE");

                    // Se il documento è stato ricevuto per IS, K2 deve essere spento
                    if (this.FromInteropPecOrSimpInterop(schedaDocumento) &&
                        (((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente != null &&
                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente.inRubricaComune))
                        checkSameMail = "0";

                    // Replicato il metodo FromInteropPecOrSimpInterop con aggiunta dei controlli interop=p || interop=e e typeId.equals("MAIL")
                    //In questi casi K2 spento.
                    if (this.FromInteropPecOrSimpInteropOrMail(schedaDocumento) &&
                        (((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente != null &&
                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente.inRubricaComune))
                        checkSameMail = "0";

                    if (!((string.IsNullOrEmpty(checkInteroperante) || checkInteroperante.Equals("0"))
                        && (string.IsNullOrEmpty(checkSameMail) || checkSameMail.Equals("0"))))
                    {
                        //Modifica PALUMBO per permettere la trasmissione rapida dopo l'apertura di K1-K2
                        if (this.ddl_tmpl.SelectedIndex > 0)
                            Session.Add("IndModelloTrasm", this.ddl_tmpl.SelectedIndex);

                        string check = string.Empty;
                        DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
                        Corrispondente mitt = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente;
                        string sysid = mitt.systemId;
                        //necessito della vecchia descrizione del corrispondente
                        mitt.oldDescrizione = ws.getOldDescByCorr(mitt.systemId);
                        //se descOld è empty la setto ugugale a var_desc_corr
                        if (string.IsNullOrEmpty(mitt.oldDescrizione))
                            mitt.oldDescrizione = mitt.descrizione;
                        string var_insert = ws.getCheckInteropFromSysIdCorrGlob(sysid);
                        Session.Add("IdRegistro", mitt.idRegistro);
                        string idAOOColl = schedaDocumento.registro.systemId;
                        Registro[] regs = UserManager.getListaRegistriWithRF(this, "", "");
                        foreach (Registro registro in regs)
                        {
                            allRegs += registro.systemId + "','";
                        }
                        if (!string.IsNullOrEmpty(allRegs) && allRegs.EndsWith("','"))
                            allRegs = allRegs.Substring(0, allRegs.Length - 3);
                        DataSet datas = new DataSet();
                        //Andrea De Marco - Modifica per Gestioni Eccezioni - interop = E viene trattato come MAIL
                        //Per ripristino commentare De Marco e ripristinare il codice commentato sottostante
                        if (schedaDocumento.interop == "P" || schedaDocumento.interop == "E")
                        //End De Marco
                        //if (schedaDocumento.interop == "P")
                        {
                            datas = ws.GetCorrByEmail(mitt.email, allRegs);
                        }
                        else if (schedaDocumento.interop == "S")
                        {
                            datas = ws.GetCorrByEmailAndDescr(mitt.email, mitt.oldDescrizione, allRegs);
                        }
                        Session.Add("Interop", schedaDocumento.interop);
                        Session.Add("DescrizioneMitt", mitt.descrizione);
                        if (!string.IsNullOrEmpty(mitt.oldDescrizione))
                            Session.Add("OldDescrizioneMitt", mitt.oldDescrizione);
                        else
                            Session.Add("OldDescrizioneMitt", mitt.descrizione);

                        Session.Add("Mail", mitt.email);

                        if (datas != null)
                        {
                            //if (var_insert.Equals("3") || var_insert.Equals("1"))
                            //{
                            check = (datas.Tables[0].Rows.Count).ToString();
                            //}

                        }

                        #region K1 ON , K2 OFF

                        if (checkInteroperante.Equals("1") && checkSameMail.Equals("0"))
                        {
                            //se apro la maschera K1/K2 allora salvo in sessione le note
                            if (Convert.ToInt32(check.ToString()) == 0 || Convert.ToInt32(check.ToString()) > 1)
                            {
                                if (this.dettaglioNota != null && !this.dettaglioNota.Testo.Equals(""))
                                    Session.Add("dettaglioNota", this.dettaglioNota);
                            }
                            if (check.Equals("0"))
                            {
                                /*Emanuela : Se il ruolo non è abilitato a modificare i corrispondenti presenti in questo registro
                                inibisco l'apertura di K1 */
                                Registro regM = ws.GetRegistroBySistemId(mitt.idRegistro);
                                if (regM != null && regM.chaRF != null &&
                                   ((regM.chaRF.Equals("0") && UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_REG")) ||
                                   (regM.chaRF.Equals("1") && UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_RF")))
                                   )
                                {
                                    string idDoc = schedaDocumento.systemId;
                                    string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                                   mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&avviso1";
                                    //TODO chiamare in modal la maschera
                                    RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                    return;
                                }
                                else if (regM == null && UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_TUTTI"))
                                {
                                    string idDoc = schedaDocumento.systemId;
                                    string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                                   mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&avviso1";
                                    //TODO chiamare in modal la maschera
                                    RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                    return;
                                }
                                else if (UserManager.ruoloIsAutorized(this, "DO_OCC_IN_K1") && (schedaDocumento.interop == "P" || schedaDocumento.interop == "E"))
                                {
                                    string idDoc = schedaDocumento.systemId;
                                    string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                                   mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&avviso1";
                                    //TODO chiamare in modal la maschera
                                    RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                    return;
                                }
                            }
                            else if (!check.Equals("0") && !check.Equals("1"))
                            {
                                //TODO MASCHERA 2
                                string idDoc = schedaDocumento.systemId;
                                string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                               mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&same_mail&" +
                                               idAOOColl + "&avviso2&btn_nuovo";
                                //TODO chiamare in modal la maschera
                                RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                return;
                            }

                        }

                        #endregion

                        #region K1 OFF , K2 ON

                        else if (checkInteroperante.Equals("0") && checkSameMail.Equals("1"))
                        {
                            if (check.Equals("1"))
                            {
                                /*Se il ruolo non è abilitato a modificare i corrispondenti presenti in questo registro
                                inibisco l'apertura di K2 */
                                Registro regM = ws.GetRegistroBySistemId(mitt.idRegistro);
                                if (regM != null && regM.chaRF != null &&
                                    ((regM.chaRF.Equals("0") && UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_REG")) ||
                                    (regM.chaRF.Equals("1") && UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_RF")))
                                    )
                                {
                                    if (this.dettaglioNota != null && !this.dettaglioNota.Testo.Equals(""))
                                        Session.Add("dettaglioNota", this.dettaglioNota);
                                    string idDoc = schedaDocumento.systemId;
                                    string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                                   mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&btn_nuovo" +
                                                           "&avviso2";
                                    //TODO chiamare in modal la maschera
                                    RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                    return;
                                }
                                else if (regM == null && UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_TUTTI"))
                                {
                                    if (this.dettaglioNota != null && !this.dettaglioNota.Testo.Equals(""))
                                        Session.Add("dettaglioNota", this.dettaglioNota);
                                    string idDoc = schedaDocumento.systemId;
                                    string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                                   mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&btn_nuovo" +
                                                           "&avviso2";
                                    //TODO chiamare in modal la maschera
                                    RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                    return;
                                }
                            }
                            else if (!check.Equals("1") && !check.Equals("0"))
                            {
                                //TODO MASCHERA 2
                                if (this.dettaglioNota != null && !this.dettaglioNota.Testo.Equals(""))
                                    Session.Add("dettaglioNota", this.dettaglioNota);
                                string idDoc = schedaDocumento.systemId;
                                string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                               mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&same_mail&" +
                                               idAOOColl + "&avviso2&btn_nuovo";
                                //TODO chiamare in modal la maschera
                                RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                return;

                            }
                            else if (check.Equals("0") && schedaDocumento.interop == "S")
                            {
                                //ws.ResetCorrVarInsertIterop(mitt.systemId, "0");
                                datas = ws.GetCorrByEmail(mitt.email, allRegs);

                                check = datas.Tables[0].Rows.Count.ToString();

                                #region K1 OFF , K2 ON

                                if (checkInteroperante.Equals("0") && checkSameMail.Equals("1"))
                                {
                                    if (check.Equals("1"))
                                    {
                                        /*Se il ruolo non è abilitato a modificare i corrispondenti presenti in questo registro
                                        inibisco l'apertura di K2 */
                                        Registro regM = ws.GetRegistroBySistemId(mitt.idRegistro);
                                        if (regM != null && regM.chaRF != null &&
                                            ((regM.chaRF.Equals("0") && UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_REG")) ||
                                            (regM.chaRF.Equals("1") && UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_RF")))
                                            )
                                        {
                                            if (this.dettaglioNota != null && !this.dettaglioNota.Testo.Equals(""))
                                                Session.Add("dettaglioNota", this.dettaglioNota);
                                            string idDoc = schedaDocumento.systemId;
                                            string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                                           mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&btn_nuovo" +
                                                                   "&avviso2";
                                            //TODO chiamare in modal la maschera
                                            RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                            return;
                                        }
                                        else if (regM == null && UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_TUTTI"))
                                        {
                                            if (this.dettaglioNota != null && !this.dettaglioNota.Testo.Equals(""))
                                                Session.Add("dettaglioNota", this.dettaglioNota);
                                            string idDoc = schedaDocumento.systemId;
                                            string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                                           mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&btn_nuovo" +
                                                                   "&avviso2";
                                            //TODO chiamare in modal la maschera
                                            RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                            return;
                                        }
                                    }
                                    else if (!check.Equals("1") && !check.Equals("0"))
                                    {
                                        //TODO MASCHERA 2
                                        if (this.dettaglioNota != null && !this.dettaglioNota.Testo.Equals(""))
                                            Session.Add("dettaglioNota", this.dettaglioNota);
                                        string idDoc = schedaDocumento.systemId;
                                        string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                                       mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&same_mail&" +
                                                       idAOOColl + "&avviso2&btn_nuovo";
                                        //TODO chiamare in modal la maschera
                                        RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                        return;

                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(mitt.email) || !mitt.inRubricaComune)
                                        {
                                            ws.ResetCodRubCorrIterop(mitt.systemId, mitt.email + "_2");
                                            Session.Add("NewIdCorrK2", mitt.systemId);
                                            //ws.ResetCorrVarInsertIterop(mitt.systemId, "NULL");
                                        }
                                    }

                                }
                                #endregion

                            }
                            //Andrea De Marco - Modifica per MEV Gestione Eccezioni PEC - schedaDocumento.interop = E viene equiparato a schedaDocumento.interop = P
                            //Per ripristino commentare De Marco e decommentare il codice sottostante
                            else if (check.Equals("0") && (schedaDocumento.interop == "P" || schedaDocumento.interop == "E"))
                            //End De Marco
                            //else if (check.Equals("0") && schedaDocumento.interop == "P")
                            {
                                Session.Add("NewIdCorrNoInteropK2", mitt.systemId);
                                string[] splitted = mitt.codiceRubrica.Split('@');
                                if (splitted.Length > 1)
                                {
                                    ws.ResetCodRubCorrIterop(mitt.systemId, splitted[0] + "_" + splitted[1]);
                                    //RegisterStartupScript("alertK2",
                                    //                      "<script>alert_k2();</script>");
                                    //return;

                                }
                            }

                        }
                        #endregion

                        #region K1 ON , K2 ON

                        else if (checkInteroperante.Equals("1") && checkSameMail.Equals("1"))
                        {
                            if (check.Equals("0"))
                            {
                                if (this.dettaglioNota != null && !this.dettaglioNota.Testo.Equals(""))
                                    Session.Add("dettaglioNota", this.dettaglioNota);
                                Registro regM = ws.GetRegistroBySistemId(mitt.idRegistro);
                                if (regM != null && regM.chaRF != null &&
                                    ((regM.chaRF.Equals("0") && UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_REG")) ||
                                    (regM.chaRF.Equals("1") && UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_RF")))
                                    )
                                {
                                    string idDoc = schedaDocumento.systemId;
                                    string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                                   mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&avviso1";
                                    //TODO chiamare in modal la maschera
                                    RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                    return;
                                }
                                else if (regM == null && UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_TUTTI"))
                                {
                                    string idDoc = schedaDocumento.systemId;
                                    string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                                   mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&avviso1";
                                    //TODO chiamare in modal la maschera
                                    RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                    return;
                                }
                                else if (UserManager.ruoloIsAutorized(this, "DO_OCC_IN_K1") && (schedaDocumento.interop == "P" || schedaDocumento.interop == "E"))
                                {
                                    string idDoc = schedaDocumento.systemId;
                                    string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                                   mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&avviso1";
                                    //TODO chiamare in modal la maschera
                                    RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                    return;
                                }
                            }
                            else if (check.Equals("1"))
                            {
                                /*Se il ruolo non è abilitato a modificare i corrispondenti presenti in questo registro
                                inibisco l'apertura di K2 */
                                Registro regM = ws.GetRegistroBySistemId(mitt.idRegistro);
                                if (regM != null && regM.chaRF != null &&
                                    ((regM.chaRF.Equals("0") && UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_REG")) ||
                                    (regM.chaRF.Equals("1") && UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_RF")))
                                    )
                                {
                                    if (this.dettaglioNota != null && !this.dettaglioNota.Testo.Equals(""))
                                        Session.Add("dettaglioNota", this.dettaglioNota);
                                    string idDoc = schedaDocumento.systemId;
                                    string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                                   mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&btn_nuovo" +
                                                   "&avviso2";
                                    //TODO chiamare in modal la maschera
                                    RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                    return;
                                }
                                else if (regM == null && UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_TUTTI"))
                                {
                                    if (this.dettaglioNota != null && !this.dettaglioNota.Testo.Equals(""))
                                        Session.Add("dettaglioNota", this.dettaglioNota);
                                    string idDoc = schedaDocumento.systemId;
                                    string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                                   mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&btn_nuovo" +
                                                   "&avviso2";
                                    //TODO chiamare in modal la maschera
                                    RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                    return;
                                }
                            }
                            else if (!check.Equals("0") && !check.Equals("1"))
                            {
                                //TODO MASCHERA 2
                                if (this.dettaglioNota != null && !this.dettaglioNota.Testo.Equals(""))
                                    Session.Add("dettaglioNota", this.dettaglioNota);
                                string idDoc = schedaDocumento.systemId;
                                string param = mitt.codiceRubrica + "&" + mitt.tipoIE + "&" +
                                               mitt.tipoCorrispondente + "&" + mitt.systemId + "&" + idDoc + "&same_mail&" +
                                               idAOOColl + "&avviso2&btn_nuovo";
                                //TODO chiamare in modal la maschera
                                RegisterStartupScript("dettaglioCorr", "<script>callDettCorr('" + param + "');</script>");
                                return;
                            }
                        }
                        #endregion

                    #endregion

                    }
                }

                if (okRF && !protoOk)
                    protocollaOk();

                
            }

            #region commento
            //			if(documentoProtocollato)
            //			{
            //				// Trasmissione documento interno e uscita
            //				if(this.schedaDocumento.tipoProto == "I" || this.schedaDocumento.tipoProto == "P" || (enableUfficioRef!=null && enableUfficioRef.Equals("1") && schedaDocumento.tipoProto.Equals("A")))
            //				{
            //					string serverName = Utils.getHttpFullPath(this);
            //					bool verificaRagioni;
            //				
            //					if(DocumentManager.TrasmettiProtocolloInterno(this, serverName, this.schedaDocumento, isEnableUffRef , out verificaRagioni, out message)) 
            //					{
            //						if(!verificaRagioni)
            //						{
            //							// Notifica utente che la trasmissione non e' stata effettuata
            //							string theAlert = "<script>alert('Attenzione! le trasmissioni non sono state effettuate poiché non sono presenti\\nle ragioni di trasmissione per: " + message + "');</script>";
            //							Response.Write(theAlert);
            //						}
            //					}
            //					else
            //					{
            //						Exception exception = new Exception("Errore durante la trasmissione del protocollo.");
            //						ErrorManager.redirect(this, exception);
            //					}								
            //				}
            //
            //			
            //				//Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=protocollo'; </script>");				
            //			
            //
            //				//PROFILAZIONE DINAMINCA
            //				if(System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null) 
            //				{
            //					if(msg.Equals(""))
            //					{
            //						//Salvataggio dei campi della profilazione dinamica
            //						if(Session["template"] != null)
            //						{
            //						}
            //					}			
            //				}
            //				Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=protocollo'; </script>");				
            //			
            //				//FINE PROFILAZIONE DINAMINCA	
            //			}
            #endregion
            logger.Info("END");
        }

        private void protocollaOk()
        {
            logger.Info("BEGIN");
            //prima di effettuare la protocollazione se il documento è personale deve essere
            //trasformato in privato
            if (schedaDocumento.personale == "1")
            {
                schedaDocumento.personale = "0";
                schedaDocumento.privato = "1";
                schedaDocumento.accessRights = "0";
                //visibilità al ruolo
                DocumentManager.cambiaDocumentoPersonalePrivato(this, schedaDocumento);
                DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
            }

            documentoProtocollato = false;
            isEnableUffRef = false;
            if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                isEnableUffRef = true;
            if (schedaDocumento.tipoProto.Equals("A"))
            {

                // DA DECOMMENTARE PER LA 2.17 e sostituire al posto del successivo blocco if - else

                //string valoreChiaveDupl = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "CERCA_DUPLICATI_PROTOCOLLO");
                //if ((valoreChiaveDupl != null) && (!valoreChiaveDupl.Equals("1")))
                //{
                //    protocolla();
                //    documentoProtocollato = true;
                //}
                //else
                //{
                //    string valoreChiaveDupl2 = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "CERCA_DUPLICATI_PROTOCOLLO_2");
                //    if (valoreChiaveDupl2 == null || !valoreChiaveDupl2.Equals("1"))
                //        valoreChiaveDupl2 = "0";
                //    cercaDuplicati(valoreChiaveDupl2);
                //}

                if (!ConfigSettings.getKey(ConfigSettings.KeysENUM.CERCA_DUPLICATI_PROTOCOLLO).Equals("1"))
                {
                    protocolla();
                    documentoProtocollato = true;
                }
                else
                {
                    string dupl = ConfigSettings.getKey(ConfigSettings.KeysENUM.CERCA_DUPLICATI_PROTOCOLLO_2);
                    if (dupl == null || !dupl.Equals("1"))
                        dupl = "0";
                    cercaDuplicati(dupl);
                }
            }
            else
            {
                protocolla();
                documentoProtocollato = true;
            }

            if (documentoProtocollato)
            {
                completaProtocollazione(isEnableUffRef);

                DocsPaWR.InfoDocumento infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
                DocsPaWR.Fascicolo[] listaFascicoli = DocumentManager.GetFascicoliDaDoc(this, infoDocumento.idProfile);
                if (listaFascicoli != null && listaFascicoli.Length > 0)
                    FascicoliManager.SetDoFascRapida(this, listaFascicoli[listaFascicoli.Length - 1]);
            }

            Session.Remove("RFNote");
            logger.Info("END");
        }

        private void msg_TrasmettiRapida_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                protocollaOk();
            }
        }

        private void msg_TrasmettiProto_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                protocollaOk();
            }
        }

        private void msg_copiaDoc_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                riproponiConCopia();
            }
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Cancel)
            {
                riproponi();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateSpedizioneAutomatica"></param>
        /// <param name="stateTrasmissioneAutomatica"></param>
        protected virtual void SpedisciDocumento(bool stateSpedizioneAutomatica, bool stateTrasmissioneAutomatica)
        {
            // Se il documento è in partenza, viene effettuata la spedizione automatica
            DocsPAWA.DocsPaWR.SpedizioneDocumento infoSpedizione = Spedizione.SpedizioneManager.SpedisciDocumento(this.schedaDocumento);

            if (!infoSpedizione.Spedito)
            {
                // Si sono verificati errori nella spedizione del documento ad almeno un destinatario;
                // viene visualizzata la maschera per la gestione della spedizione
                //this.ClientScript.RegisterClientScriptBlock(this.GetType(), "OnErrorSpedizioneAutomaticaDocumento", "OnErrorSpedizioneAutomaticaDocumento();", true);                                
                Response.Write("<script language='javascript'>alert('Attenzione: non è stato possibile spedire o trasmettere il documento ad uno o più destinatari');</script>");
            }
            else
            {
                Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=protocollo';</script>");
            }
        }

        /// <summary>
        /// Spedizione del documento corrente
        /// </summary>
        protected virtual void SpedisciDocumento()
        {
            // Se il documento è in partenza, viene effettuata la spedizione automatica
            DocsPAWA.DocsPaWR.SpedizioneDocumento infoSpedizione = Spedizione.SpedizioneManager.SpedisciDocumento(this.schedaDocumento);

            // New Code - Andrea De Marco - 07-07-2014
            // Vecchio Comportamento:
            // l'oggetto infoSpedizione ha il campo spedito che viene valorizzato a true 
            // se almeno una trasmissione/spedizione è andata a buon fine.
            // Nuovo comportamento:
            // Controllo che tutti i destinatari (Interni/Esterni) siano stati raggiunti dalla trasmissione/spedizione
            #region Controllo per Spedizioni e Trasmissioni Automatiche
            try
            {
                if (infoSpedizione.Spedito)
                {
                    // Analisi destinatari esterni
                    for (int i = 0; i < infoSpedizione.DestinatariEsterni.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(infoSpedizione.DestinatariEsterni[i].StatoSpedizione.Stato.ToString()) &&
                            !infoSpedizione.DestinatariEsterni[i].StatoSpedizione.Stato.ToString().Equals("Spedito"))
                        {
                            infoSpedizione.Spedito = false;
                            break;
                        }
                    }

                    // Analisi dsetinatari interni
                    for (int i = 0; i < infoSpedizione.DestinatariInterni.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(infoSpedizione.DestinatariInterni[i].StatoSpedizione.Stato.ToString()) &&
                            !infoSpedizione.DestinatariInterni[i].StatoSpedizione.Stato.ToString().Equals("Spedito"))
                        {
                            infoSpedizione.Spedito = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            #endregion

            if (!infoSpedizione.Spedito)
            {
                // Si sono verificati errori nella spedizione del documento ad almeno un destinatario;
                // viene visualizzata la maschera per la gestione della spedizione
                //this.ClientScript.RegisterClientScriptBlock(this.GetType(), "OnErrorSpedizioneAutomaticaDocumento", "OnErrorSpedizioneAutomaticaDocumento();", true);                                
                // Old Code
                //Response.Write("<script language='javascript'>alert('Attenzione: non è stato possibile spedire o trasmettere il documento ad uno o più destinatari');</script>");
                // New Code - Andrea De Marco - 07-07-2014
                Response.Write("<script language='javascript'>alert('Attenzione: almeno per un destinatario non è stata effettuata una spedizione o una trasmissione del documento.');</script>");
            }
            else
            {
                Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=protocollo';</script>");
            }
        }

        /// <summary>
        /// Handler per l'evento di risposta alla messagebox di spedizione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void msg_SpedizioneAutomatica_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                // Spedizione del documento se l'utente conferma
                this.SpedisciDocumento();
            }

            // Rimozione della chiave di sessione temporanea per confermare la spedizione del documento
            this.Session.Remove("showConfirmSpedizioneAutomatica");
        }

        /// <summary>
        ///Questo metodo trasmette ai destinatari o all'ufficio referente dopo 
        ///che la protocollazione è andata a buon fine ed effettua 
        ///la profilazione dinamica del documento
        /// </summary>
        /// <param name="isEnableUffRef">true se è abilitato l'ufficio referente</param>
        /// <returns></returns>

        private void completaProtocollazione(bool isEnableUffRef)
        {
            // Trasmissione documento interno e uscita o arrivo
            string message = "";

            //aggiunto perchè se la protocollazione fallisce comunque sia venivano create trasmissioni
            if (!string.IsNullOrEmpty(this.schedaDocumento.systemId))
            {
                if (schedaDocumento.tipoProto == "P")
                {
                    if (this.estendiVisibilita.Value == "false")
                        this.schedaDocumento.eredita = "0";

                    //Prova Andrea - metto in sessione la scheda documento con il flag eredita a 0
                    //DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                    //DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                    //End Andrea

                    DocsPaWR.ConfigSpedizioneDocumento config = Spedizione.SpedizioneManager.GetConfigSpedizioneDocumento();

                    //*******************************************************
                    // Giordano Iacozzilli 21/09/2012 
                    // Ripristino della sola trasmissione in automatico ai 
                    // destinatari interni nei protocolli in uscita
                    //*******************************************************
                    //OLD CODE:
                    //if (config.SpedizioneAutomaticaDocumento)
                    //{
                    //    // Verifica che la versione corrente del documento sia stata acquisita
                    //    bool fileAcquisito = (this.schedaDocumento.documenti != null && this.schedaDocumento.documenti[0].fileSize != "0");

                    //    if (!fileAcquisito && config.AvvisaSuSpedizioneDocumento)
                    //    {
                    //        // Creazione di una chiave di sessione temporanea
                    //        // per gestire la visualizzazione del messaggio
                    //        // di conferma di spedizione automatica
                    //        this.Session["showConfirmSpedizioneAutomatica"] = true;
                    //    }
                    //    else
                    //    {
                    //        // Spedizione del documento
                    //        this.SpedisciDocumento();
                    //    }
                    //}
                    //
                    //NEW CODE:
                    if (config.SpedizioneAutomaticaDocumento || config.TrasmissioneAutomaticaDocumento)
                    {
                        if (config.SpedizioneAutomaticaDocumento)
                        {

                            bool fileAcquisito = (this.schedaDocumento.documenti != null && this.schedaDocumento.documenti[0].fileSize != "0");

                            if (!fileAcquisito && config.AvvisaSuSpedizioneDocumento)
                            {
                                // Creazione di una chiave di sessione temporanea
                                // per gestire la visualizzazione del messaggio
                                // di conferma di spedizione automatica
                                this.Session["showConfirmSpedizioneAutomatica"] = true;
                            }
                            else
                            {
                                // Spedizione del documento
                                this.SpedisciDocumento();
                            }
                        }
                        else
                            this.SpedisciDocumento();
                    }
                 
                    //*******************************************************
                    //FINE
                    //*******************************************************
                }
                else if (this.schedaDocumento.tipoProto == "I"
                    // || (enableUfficioRef != null && enableUfficioRef.Equals("1") && schedaDocumento.tipoProto.Equals("A"))
                  )
                {
                    if (estendiVisibilita.Value == "false")
                    {
                        schedaDocumento.eredita = "0";
                    }
                    string serverName = Utils.getHttpFullPath(this);
                    bool verificaRagioni;

                    if (DocumentManager.TrasmettiProtocolloInterno(this, serverName, this.schedaDocumento, isEnableUffRef, out verificaRagioni, out message))
                    {
                        if (!string.IsNullOrEmpty(message))
                        {
                            Response.Write("<script>" + message + "</script>");
                            message = "";
                        }

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
                //metodo per la trasmissione rapida (templ & Modelli)
                execTrasmRapida();

                if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                {
                    //correzione SAB dà problemi con la trasmissione all'ufficio referente perchè non è impostato a true il bool isEnableUffRef
                    isEnableUffRef = true;
                    spedisciUfficioReferente(isEnableUffRef);
                    //spedisciUfficioReferente();
                }
            }

            //Verifico se il cha_interop del protocollo è = "P", questo caso se
            //esiste un mittente intermedio effettuo una trasmissione a quest'ultimo 
            //con la ragione predefinita dell'amministrazione

            //Andrea De Marco - Modifica per MEV Gestione Eccezioni PEC - equipariamo i protocolli con interop = E a quelli interop = P
            //Per ripristino commentare De Marco e decommentare il codice sottostante
            if ((schedaDocumento.interop == "P" && schedaDocumento.tipoProto == "A") || (schedaDocumento.interop == "E" && schedaDocumento.tipoProto == "A"))
            //End De Marco
            //if (schedaDocumento.interop == "P" && schedaDocumento.tipoProto == "A")
            {
                Corrispondente mittInt = ((ProtocolloEntrata)schedaDocumento.protocollo).mittenteIntermedio;
                if (mittInt != null)
                {
                    Trasmissione trasmissione = new Trasmissione();
                    trasmissione.noteGenerali = "Protocollato documento nr. " + schedaDocumento.docNumber;
                    trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
                    trasmissione.infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
                    trasmissione.utente = UserManager.getUtente(this);
                    trasmissione.ruolo = UserManager.getRuolo(this);

                    RagioneTrasmissione ragione = new RagioneTrasmissione();
                    RagioneTrasmissione[] listaRagioni = wws.SmistamentoGetRagioniTrasmissione(trasmissione.ruolo.idAmministrazione);
                    for (int i = 0; i < listaRagioni.Length; i++)
                    {
                        if (((RagioneTrasmissione)listaRagioni[i]).descrizione.ToUpper() == "COMPETENZA")
                        {
                            ragione = (RagioneTrasmissione)listaRagioni[i];
                            break;
                        }
                    }

                    if (ragione.systemId != null && ragione.systemId != "")
                    {
                        trasmissione = TrasmManager.addTrasmissioneSingola(trasmissione, mittInt, ragione, "Protocollato documento nr. " + schedaDocumento.docNumber, "T", 0, this);
                        trasmissione = this.impostaNotificheUtentiDaModello(trasmissione);
                        DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
                        if (infoUtente.delegato != null)
                            trasmissione.delegato = ((DocsPAWA.DocsPaWR.InfoUtente)(infoUtente.delegato)).idPeople;

                        //Nuovo metodo saveExecuteTrasm
                        trasmissione = TrasmManager.saveExecuteTrasm(this, trasmissione, infoUtente);
                        //trasmissione = TrasmManager.saveTrasm(this, trasmissione);
                        //TrasmManager.executeTrasm(this, trasmissione);
                    }
                }
            }

            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null)
            {
                if (schedaDocumento.template != null)
                {
                    impostaStatoIniziale(schedaDocumento.template);
                }
            }

            /* ABBATANGELI GIANLUIGI
             * Commentato il codice sottostante che scrive inutilmente dati in sessione 
             * 
            //PROFILAZIONE DINAMICA
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null)
            {
                //Salvataggio dei campi della profilazione dinamica
                //if (Session["template"] != null)
                if (schedaDocumento.template != null)
                {
                    //wws.salvaInserimentoUtenteProfDim(UserManager.getInfoUtente(this), (DocsPaWR.Templates)Session["template"], schedaDocumento.docNumber, "P");
                    //schedaDocumento.template = (DocsPaWR.Templates)Session["template"];
                    //impostaStatoIniziale((DocsPaWR.Templates)Session["template"]);

                    impostaStatoIniziale(schedaDocumento.template);
                    Session["template"] = ProfilazioneDocManager.getTemplateDettagli(schedaDocumento.docNumber, this);
                    //Verifico che è stato selezionato il calcolo di un contatore,
                    //in caso affermativo, riapro la popup di profilazione, per far verificare il numero generato
                    if (Session["contaDopoChecked"] != null)
                    {
                        Response.Write("<script>" +
                                        "var w = window.screen.width; " +
                                        "var h = window.screen.height; " +
                                        "var new_w = (w-100)/2; " +
                                        "var new_h = (h-400)/2; " +
                                        "window.showModalDialog('AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinamica.aspx','','dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w); " +
                                        "</script>");
                        Session.Remove("contaDopoChecked");
                    }
                }
            }
            //FINE PROFILAZIONE DINAMICA

            * */
            Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=protocollo'; </script>");
        }

        private void cercaDuplicati(string cercaDuplicati2)
        {
            DocsPaWR.InfoProtocolloDuplicato[] datiProtDupl = null;
            DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum esitoRicercaDuplicati = DocumentManager.cercaDuplicati(this, schedaDocumento, cercaDuplicati2, out datiProtDupl);

            if (this.dettaglioNota != null && !this.dettaglioNota.Testo.Equals(""))
            {
                Session.Add("dettaglioNota", this.dettaglioNota);
            }
            if (esitoRicercaDuplicati != DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum.NessunDuplicato)
            {
                #region old_code
                //string scriptString = "if(confirm('Dati di protocollazione già presenti:";

                //if(datiProtDupl!=null && datiProtDupl.Length>0)
                //{
                //    scriptString += "\\n\\nData: " + datiProtDupl[0].dataProtocollo;
                //    scriptString += "\\nSegnatura: " + datiProtDupl[0].segnaturaProtocollo;
                //    if (datiProtDupl[0].uoProtocollatore != null && datiProtDupl[0].uoProtocollatore != string.Empty)
                //        scriptString += "\\nUfficio: " + datiProtDupl[0].uoProtocollatore.Replace("'", "\\'"); ;
                //}
                //scriptString += "\\n\\nContinuare?')) ";
                //scriptString += "window.document.location = window.document.location.href + '&protocolla=1';";

                //if (!this.Page.IsStartupScriptRegistered("confirmJavaScript"))
                //{
                //    string scString = "<SCRIPT language='javascript'>" + scriptString.Replace("'", "\'") + "</SCRIPT>";
                //    this.Page.RegisterStartupScript("confirmJavaScript", scString);
                //}
                #endregion
                if (datiProtDupl != null && datiProtDupl.Length > 0)
                {
                    string querySt = this.Server.UrlEncode("dtaProto=" + datiProtDupl[0].dataProtocollo
                                  + "&Segn=" + datiProtDupl[0].segnaturaProtocollo + "&Uff=" + datiProtDupl[0].uoProtocollatore.Replace("'", "|@ap@|")
                                  + "&IdDoc=" + datiProtDupl[0].idProfile + "&NumProto=" + datiProtDupl[0].numProto
                                  + "&DoProto=SI&result=" + esitoRicercaDuplicati.ToString() + "&modelloTrasm=" + this.ddl_tmpl.SelectedIndex + "&docAcquisito=" + datiProtDupl[0].docAcquisito);

                    string script = "<script>var retValue=window.showModalDialog('../popup/avvisoProtocolloEsistente.aspx?" + querySt
                                    + "','','dialogWidth:520px;dialogHeight:240px;fullscreen:no;toolbar:no;status:no;resizable:no;scroll:auto;"
                                    + "center:yes;help:no;close:no'); if (retValue=='protocolla')window.document.location = window.document.location.href + '&protocolla=1';</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "popupDuplicati", script);
                }
                else
                {
                    switch (esitoRicercaDuplicati)
                    {
                        case EsitoRicercaDuplicatiEnum.NoMittente:
                            RegisterClientScript("alertNoMittente", "alert('Nessun mittente selezionato');");
                            break;

                        case EsitoRicercaDuplicatiEnum.ProtocolloNullo:
                            RegisterClientScript("alertProtocollo", "alert('Nessun documento protocollato selezionato');");
                            break;

                        case EsitoRicercaDuplicatiEnum.NoProtocolloIngresso:
                            RegisterClientScript("alertNoProtoIngresso", "alert('Nessun protocollo in ingresso selezionato');");
                            break;
                    }
                }
            }
            else
            {
                this.protocolla();
                this.documentoProtocollato = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void setMittentiDaAggiornare()
        {
            if (this.rbl_InOut_P.SelectedItem.Value.Equals("In"))
            {
                ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = true;
                if (this.txt_DescMitInt_P.Text != null && !this.txt_DescMitInt_P.Text.Equals(""))
                {
                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittenteIntermedio = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void protocolla()
        {
            logger.Info("BEGIN");
            try
            {
                // per cancellare dalla sessione il dato di riproponi
                if (Session["docRiproposto"] != null && (bool)Session["docRiproposto"])
                    Session.Remove("docRiproposto");

                string result = "Operazione avvenuta con successo.\\n\\n";
                DocsPaWR.ResultProtocollazione risultatoProtocollazione = DocsPAWA.DocsPaWR.ResultProtocollazione.OK;
                string systemID = schedaDocumento.systemId;
                string resFunz = "";
                if (systemID != null)
                {
                    setMittentiDaAggiornare();
                }

                //if (this.rbl_InOut_P.SelectedValue.Equals("In") && this.isMezzoSpedizioneRequired)
                //{
                //    if (!this.ddl_spedizione.SelectedItem.Value.Equals(""))
                //    {
                //        schedaDocumento.mezzoSpedizione = Convert.ToInt32(this.ddl_spedizione.SelectedItem.Value);
                //        schedaDocumento.descMezzoSpedizione = this.ddl_spedizione.SelectedItem.Text;
                //    }
                //}

                if (this.NotaDocumentoEnabled)
                {
                    // Save dati nota
                    this.dettaglioNota.Save();
                    this.dettaglioNota.Enabled = false;
                }

                /* ABBATANGELI GIANLUIGI - TEST
                 * Commentato il codice sottostante che scrive inutilmente dati in sessione 
                 * 
                //PROFILAZIONE DINAMICA
                string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
                if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
                {
                    //Salvataggio dei campi della profilazione dinamica
                    if (Session["template"] != null)
                    {
                        if (!ProfilazioneDocManager.verificaCampiObbligatori((DocsPAWA.DocsPaWR.Templates)Session["template"]))
                        {
                            //wws.salvaInserimentoUtenteProfDim(UserManager.getInfoUtente(this), (DocsPaWR.Templates)Session["template"], schedaDocumento.docNumber, "");
                            schedaDocumento.template = (DocsPaWR.Templates)Session["template"];
                            Session["template"] = ProfilazioneDocManager.getTemplateDettagli(schedaDocumento.docNumber, this);
                        }
                    }

                    if (ddl_tipoAtto.SelectedIndex == 0)
                    {
                        schedaDocumento.tipologiaAtto = null;
                    }
                }
                //FINE PROFILAZIONE DINAMICA
                * */

                //variabile di sessione bool usata per evitare i buchi di prot per doppio click
                // if True then bloccato
                // if False then libero
                if (!DocumentManager.getBlockQuickProt(this))
                {
                    DocumentManager.setBlockQuickProt(this, true);
                    //gestione fascicolazione veloce se il doc non è nuovo protocollo soltanto / altrimenti protocollo e classifico (se è stato selezionato un codice)
                    DocsPaWR.Fascicolo fasc = null;
                    //DocsPaWR.TemplateTrasmissione template;
                    //if (systemID != null && !systemID.Equals(""))
                    //{
                    //    fasc = null;
                    //    //template = null;
                    //}
                    //else
                    //{
                    //fasc = getFascicolo();
                    //template = getTemplate();
                    //}

                    fasc = FascicoliManager.getFascicoloSelezionatoFascRapida(this);

                    if (!string.IsNullOrEmpty(this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text))
                        schedaDocumento.documenti[0].dataArrivo = this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text + " " + this.txt_OraPervenuto_P.Text;


                    schedaDocumento = DocumentManager.protocolla(this, schedaDocumento, fasc, null, out risultatoProtocollazione, ref resFunz);
                    //Per sviluppo estensione delle funzionalità di fascicolazione e trasmissione rapida
                    FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                    FascicoliManager.removeCodiceFascRapida(this);
                    FascicoliManager.removeDescrizioneFascRapida(this);
                }
                //Dopo la protocollazione mettiamo le variabili per l'aggiornamento del mitt/dest a false

                if (schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloEntrata))
                {
                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = false;
                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittenteIntermedio = false;
                    schedaDocumento.modificaRispostaDocumento = false;
                }
                else if (schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloUscita))
                {
                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatari = false;
                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatariConoscenza = false;
                    schedaDocumento.modificaRispostaDocumento = false;
                }
                else if (schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloInterno))
                {
                    ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).daAggiornareDestinatari = false;
                    ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).daAggiornareDestinatariConoscenza = false;

                }
                if (isEnableUffRef)
                {
                    schedaDocumento.protocollo.ModUffRef = false;
                }

                // inserimento in tabella per il mezzo di spedizione (se presente)
                if (risultatoProtocollazione == DocsPAWA.DocsPaWR.ResultProtocollazione.OK && schedaDocumento.tipoProto.Equals("A") && this.isMezzoSpedizioneRequired)
                {
                    if (schedaDocumento.mezzoSpedizione != null && !schedaDocumento.mezzoSpedizione.Equals("0"))
                    {
                        InfoUtente info = UserManager.getInfoUtente(this);
                        if (!DocumentManager.inserisciMetodoSpedizione(info, schedaDocumento.mezzoSpedizione.ToString(), schedaDocumento.systemId))
                        {
                            string theAlert = "<script>alert('Attenzione! mancato collegamento tra il documento e il mezzo di spedizione');</script>";
                            Response.Write(theAlert);
                        }
                    }

                    //if (!this.ddl_spedizione.SelectedItem.Value.Equals(""))
                    //{
                    //    schedaDocumento.protocollo.descMezzoSpedizione = this.ddl_spedizione.SelectedItem.Text;
                    //    schedaDocumento.protocollo.mezzoSpedizione = Convert.ToInt32(this.ddl_spedizione.SelectedItem.Value);
                    //}
                }
                else
                    schedaDocumento.descMezzoSpedizione = "Errore";
                // fine inserimento in tabella per il mezzo di spedizione

                DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                /*riportato dalla 2.5 by massimo digregorio 
                * old				//alert per confermare l'avvenuta operazione
                                result = result + "Numero di protocollo:  [ " + schedaDocumento.protocollo.numero + " ]";
                                Response.Write("<script>alert('" + result + "');</script>");
                new: */
                //alert per confermare l'avvenuta operazione
                //se questo parametro nel web.config della wa non è presente o è a "0" allora niente alert!
                string alert_conferma_creazione = ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_ALERT_PROTOCOLLO_CREATO);
                if ((alert_conferma_creazione != null) && (alert_conferma_creazione.Equals("1")))
                {
                    if (schedaDocumento.protocollo != null && schedaDocumento.protocollo.numero != null)
                    {
                        if (risultatoProtocollazione == DocsPAWA.DocsPaWR.ResultProtocollazione.OK)
                        {
                            result = result + " num. di protocollo: [ " + schedaDocumento.protocollo.numero + " ]";
                            Response.Write("<script>alert('" + result + "');</script>");
                        }
                    }
                }

                if (!resFunz.Equals(""))
                    Response.Write("<script>alert('" + resFunz + "');</script>");
                //se il documento è nuovo ricarico il frame di destra
                //				if (systemID == null || systemID.Equals("")) 
                if (schedaDocumento.systemId == null || schedaDocumento.systemId.Equals(""))
                {
                    //ricarica il frame destro
                    string funct_dx = "top.principale.iFrame_dx.document.location='tabDoc.aspx';";
                    Response.Write("<script language='javascript'> " + funct_dx + "</script>");
                }

                /*
                 * Il reload del frame e' stato spostato all'esterno del metodo per permettere la notifica
                 * di eventuali segnalazioni di errore
                 */

                //Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=protocollo'; </script>");				
                documentoProtocollato = true;
                Session.Add("isDocProtocollato", true);
                if (Session["multiCorr"] != null)
                    Session.Remove("multiCorr");

                DocsPaWR.InfoDocumento infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
                DocsPaWR.Fascicolo[] listaFascicoli = DocumentManager.GetFascicoliDaDoc(this, infoDocumento.idProfile);
                if (listaFascicoli != null && listaFascicoli.Length > 0)
                    FascicoliManager.SetDoFascRapida(this, listaFascicoli[listaFascicoli.Length - 1]);

            }
            catch (Exception ex)
            {
                //ErrorManager.redirect(this, ex);
                //Response.Write("<script>alert('Si è verificato un errore durante la procedura di protocollo: \n" + ex.Message.ToString() + "');return;</script>");
                //federica 21/11/2005
                ErrorManager.OpenErrorPage(this, ex, "protocollazione");
            }

            //			finally
            //			{
            //				FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
            //				Session.Remove("validCodeFasc");
            //			}
            logger.Info("END");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_protocolla_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            logger.Info("BEGIN");

            // Se il documento è stato trasmesso per IS ed è stato inviato come privato, viene mostrato un
            // popup che informa l'utente e richiede all'utente se lasciarlo marcato privato
            if (!String.IsNullOrEmpty(schedaDocumento.systemId) &&
                schedaDocumento.privato == "1" &&
                InteroperabilitaSemplificataManager.IsDocumentReceivedPrivate(schedaDocumento.systemId))
                ClientScript.RegisterStartupScript(this.GetType(), "docPrivatoMitt", "protoPrivatoDaMittente();", true);
            else
            {
                if (!this.GetControlAclDocumento().AclRevocata)
                {
                    protocollaDoc();
                }
            }

            Session.Add("refreshDxPageProf", true);
            logger.Info("END");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_protocollaGiallo_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            logger.Info("BEGIN");
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                protocollaDoc();
            }

            Session.Add("refreshDxPageProf", true);
            logger.Info("END");
        }

        /// <summary>
        /// Aggiornamento in batch delle sole note
        /// </summary>
        protected virtual void UpdateNote()
        {
            AssociazioneNota oggettoAssociato = new AssociazioneNota();
            oggettoAssociato.TipoOggetto = OggettiAssociazioniNotaEnum.Documento;
            oggettoAssociato.Id = this.schedaDocumento.systemId;

            // Inserimento della nota creata
            this.dettaglioNota.Save();

            // Aggiornamento delle note sul backend
            this.schedaDocumento.noteDocumento = Note.NoteManager.Update(oggettoAssociato, this.schedaDocumento.noteDocumento);

            // Disabilitazione del dettaglio nota e del pulsante salva
            this.dettaglioNota.Enabled = false;
            this.btn_salva_P.Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_salva_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            logger.Info("BEGIN");
            // Se il documento è in ceckout, non si può effettuare il salvataggio
            if (this.IsDocumentInCheckOutState())
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "NonSalvabile",
                    "alert('Non è possibile effettuare il salvataggio in quanto il documento principale oppure almeno un suo allegato risulta bloccato.');",
                    true);
            else
                this.SaveDocument();

            Session.Add("refreshDxPageProf", true);
            logger.Info("END");
        }

        private void SaveDocument()
        {
            logger.Info("BEGIN");
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                if (schedaDocumento.tipoProto == "P")
                    if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza != null && ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza.Length > 0)
                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatariConoscenza = true;


                if (schedaDocumento.tipoProto == "I")
                    if (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatariConoscenza != null && ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatariConoscenza.Length > 0)
                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).daAggiornareDestinatariConoscenza = true;

                // Se il documento è in checkout non può in alcun modo essere salvato
                if (this.schedaDocumento.checkOutStatus != null)
                {
                    Response.Write("<script>alert('Impossibile salvare i dati del documento in quanto risulta bloccato');</script>");
                    return;
                }

                if (this.NotaDocumentoEnabled && this.dettaglioNota.ReadOnly && this.dettaglioNota.IsDirty)
                {
                    this.UpdateNote();
                }
                else
                {
                    if (Session["catenaDoc"] != null)
                    {
                        Session.Remove("catenaDoc");

                        if (string.IsNullOrEmpty(schedaDocumento.systemId))
                        {
                            schedaDocumento = DocumentManager.creaDocumentoGrigio(this, schedaDocumento);
                            schedaDocumento.predisponiProtocollazione = true;
                        }
                    }

                    //prima di salvare se il documento è personale deve essere
                    //trasformato in privato
                    if (schedaDocumento.personale == "1")
                    {
                        schedaDocumento.personale = "0";
                        schedaDocumento.privato = "1";
                        schedaDocumento.accessRights = "0";
                        //visibilità al ruolo
                        DocumentManager.cambiaDocumentoPersonalePrivato(this, schedaDocumento);
                        DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                    }

                    try
                    {
                        #region CONTROLLI PER RISPOSTA AL DOCUMENTO
                        string descrMsg = string.Empty;
                        if (!VerificaRispostaDocumento(ref descrMsg))
                        {
                            RegisterStartupScript("disabled", "<SCRIPT>alert('" + descrMsg + "');</SCRIPT>");
                            return;
                        }
                        #endregion

                        bool daInserireCollMSpedDoc = false;
                        bool daUpdatareCollMSpedDoc = false;
                        bool daCancellareCollMSpedDoc = false;
                        int oldMezzo = 0;
                        if (schedaDocumento != null && schedaDocumento.mezzoSpedizione == null)
                            schedaDocumento.mezzoSpedizione = "0";

                        if (schedaDocumento != null && this.isMezzoSpedizioneRequired)
                        {
                            if (schedaDocumento.mezzoSpedizione.Equals("0") && Convert.ToInt32(schedaDocumento.mezzoSpedizione) != this.ddl_spedizione.SelectedIndex)
                                daInserireCollMSpedDoc = true;
                            if (!schedaDocumento.mezzoSpedizione.Equals("0") && Convert.ToInt32(schedaDocumento.mezzoSpedizione) != this.ddl_spedizione.SelectedIndex && this.ddl_spedizione.SelectedIndex != 0)
                            {
                                daUpdatareCollMSpedDoc = true;
                                oldMezzo = Convert.ToInt32(schedaDocumento.mezzoSpedizione);
                            }
                            if (Session["isDocModificato"] != null && (bool)Session["isDocModificato"]
                                && schedaDocumento.mezzoSpedizione != "0") //vuol dire che non è da cancellare, perchè non l'avevo mai settato in precedenza
                            {
                                if (this.ddl_spedizione.SelectedIndex == 0)
                                    daCancellareCollMSpedDoc = true;
                            }
                        }

                        bool enableUffRef = false;
                        string message = "";
                        bool daAggiornareUffRef = false;
                        if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                        {
                            enableUffRef = true;
                        }

                        string msg = CheckFields("S");

                        msg += ProfilazioneDocManager.VerifyAndSetTipoDoc(UserManager.getInfoUtente(this), ref schedaDocumento, this);

                        ////creazione mittente occasionale
                        //creaCorrOccasionalePerProtocollo();

                        if (msg.Equals(""))
                        {
                            if (this.isMezzoSpedizioneRequired)
                            {
                                schedaDocumento.mezzoSpedizione = this.ddl_spedizione.SelectedIndex.ToString();
                                schedaDocumento.descMezzoSpedizione = this.ddl_spedizione.SelectedItem.Text;
                            }

                            // controllo su obbligatorietà della fascicolazione e chiamata al web service
                            // che ci da informazione se il documento è già stato o meno fascicolato
                            if (this.isFascRapidaRequired && schedaDocumento != null && schedaDocumento.systemId != null)
                            {
                                if (!DocumentManager.getSeDocFascicolato(this, schedaDocumento) && string.IsNullOrEmpty(this.txt_CodFascicolo.Text))
                                {
                                    RegisterStartupScript("alert", "<script>alert('La fascicolazione rapida è obbligatoria !');</script>");
                                    FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                                    FascicoliManager.removeCodiceFascRapida(this);
                                    FascicoliManager.removeDescrizioneFascRapida(this);
                                    return;
                                }
                            }

                            // Save dati nota
                            if (this.NotaDocumentoEnabled)
                                this.dettaglioNota.Save();

                            if (Session["isDocModificato"] != null && (bool)Session["isDocModificato"] == true)
                                Session.Remove("isDocModificato");

                            if (Session["abilitaModificaSpedizione"] != null && (bool)Session["abilitaModificaSpedizione"])
                                Session.Remove("abilitaModificaSpedizione");

                            /* ABBATANGELI GIANLUIGI - TEST
                             * Commentato il codice sottostante che scrive inutilmente dati in sessione 
                             * 
                             //PROFILAZIONE DINAMICA
                            string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
                            if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
                            {
                                //Salvataggio dei campi della profilazione dinamica
                                if (Session["template"] != null)
                                {
                                    if (!ProfilazioneDocManager.verificaCampiObbligatori((DocsPAWA.DocsPaWR.Templates)Session["template"]))
                                    {
                                        //wws.salvaInserimentoUtenteProfDim(UserManager.getInfoUtente(this), (DocsPaWR.Templates)Session["template"], schedaDocumento.docNumber, "");
                                        schedaDocumento.template = (DocsPaWR.Templates)Session["template"];
                                        Session["template"] = ProfilazioneDocManager.getTemplateDettagli(schedaDocumento.docNumber, this);
                                    }
                                }
                            }
                            //FINE PROFILAZIONE DINAMICA

                            * */
                            if (wws.isEnableRiferimentiMittente() && rbl_InOut_P.SelectedItem.Value == "In")
                            {
                                schedaDocumento.riferimentoMittente = txt_riferimentoMittente.Text;
                            }

                            DocsPaWR.Utente utente = UserManager.getUtente(this);
                            schedaDocumento.idPeople = utente.idPeople;

                            if (schedaDocumento != null && schedaDocumento.protocollo != null
                                && string.IsNullOrEmpty(schedaDocumento.protocollo.numero)
                                && string.IsNullOrEmpty(schedaDocumento.systemId)) //sto facendo salva di un predisposto, ma in Docspa non esiste questa funzione,
                            //quindi faccio prima adddocgrigia e poi salvo predisponendo il doc.
                            {
                                Protocollo proto = schedaDocumento.protocollo;
                                schedaDocumento.protocollo = null;
                                string tipoProto = schedaDocumento.tipoProto;
                                schedaDocumento.tipoProto = "G";
                                schedaDocumento.predisponiProtocollazione = false;

                                schedaDocumento = DocumentManager.creaDocumentoGrigio(this, schedaDocumento);
                                schedaDocumento.predisponiProtocollazione = true;
                                schedaDocumento.tipoProto = tipoProto;
                                schedaDocumento.protocollo = proto;
                                schedaDocumento = DocumentManager.salva(this, schedaDocumento, enableUffRef, out daAggiornareUffRef);
                            }
                            else
                                schedaDocumento = DocumentManager.salva(this, schedaDocumento, enableUffRef, out daAggiornareUffRef);

                            //Inizio sviluppo estensione delle funzionalità di fascicolazione e trasmissione rapida
                            DocsPaWR.Fascicolo fasc;
                            string segnatura = "";
                            string returnMsg = "";
                            fasc = getFascicolo();
                            if (fasc != null)
                            {
                                if (fasc.stato == "C")
                                {
                                    returnMsg += "Il fascicolo scelto è chiuso. Pertanto il documento non è stato fascicolato";
                                }
                                else
                                {
                                    if (schedaDocumento.protocollo != null)
                                        segnatura = schedaDocumento.protocollo.segnatura;

                                    // NEW
                                    /*
                                    string errorMessage;
                                    if (DocumentManager.fascicolaRapida(this, schedaDocumento.systemId, schedaDocumento.docNumber, segnatura, fasc, out errorMessage))
                                    {
                                        schedaDocumento.fascicolato = "1";
                                    }
                                    else
                                    {
                                        // Errore nella fascicolazione del doc, riporta il messaggio proveniente dal server
                                        schedaDocumento.fascicolato = "0";

                                        if (string.IsNullOrEmpty(msg))
                                            returnMsg += " Il documento non è stato fascicolato";
                                        else
                                            returnMsg += " " + errorMessage;
                                    }
                                    */

                                    // OLD
                                    int risultato = DocumentManager.fascicolaRapida(this, schedaDocumento.systemId, schedaDocumento.docNumber, segnatura, fasc, true);
                                    switch (risultato)
                                    {
                                        case 0:
                                            schedaDocumento.fascicolato = "1";
                                            break;
                                        case 1:
                                            returnMsg += " Il documento non è stato fascicolato";
                                            break;
                                        case 2:
                                            returnMsg += "Il documento risulta già fascicolato nella fascicolazione indicata: " + fasc.descrizione;
                                            break;
                                    }

                                }
                                if (!returnMsg.Equals(""))
                                    Response.Write("<script>alert('" + returnMsg + "');</script>");

                                FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                                FascicoliManager.removeCodiceFascRapida(this);
                                FascicoliManager.removeDescrizioneFascRapida(this);
                            }

                            if (ddl_tmpl.SelectedIndex > 0)
                            {
                                execTrasmRapida();
                                ddl_tmpl.SelectedIndex = 0;
                            }
                            //Fine sviluppo estensione delle funzionalità di fascicolazione e trasmissione rapida    

                            DocsPaWR.Ruolo ruolo = UserManager.getRuolo(this);

                            // inserimento/update/cancellazione in tabella per il mezzo di spedizione (se presente)
                            if (daInserireCollMSpedDoc && schedaDocumento.tipoProto.Equals("A") && this.isMezzoSpedizioneRequired)
                            {
                                if (ddl_spedizione.SelectedIndex > 0)
                                {
                                    InfoUtente info = UserManager.getInfoUtente(this);
                                    if (!DocumentManager.inserisciMetodoSpedizione(info, this.ddl_spedizione.SelectedValue, schedaDocumento.systemId))
                                    {
                                        string theAlert = "<script>alert('Attenzione! mancato collegamento tra il documento e il mezzo di spedizione');</script>";
                                        Response.Write(theAlert);
                                    }
                                }
                            }
                            if (daUpdatareCollMSpedDoc && schedaDocumento.tipoProto.Equals("A") && this.isMezzoSpedizioneRequired)
                            {
                                if (ddl_spedizione.SelectedIndex > 0)
                                {
                                    InfoUtente info = UserManager.getInfoUtente(this);
                                    if (!DocumentManager.updateMetodoSpedizione(info, oldMezzo.ToString(), this.ddl_spedizione.SelectedValue, schedaDocumento.systemId))
                                    {
                                        string theAlert = "<script>alert('Attenzione! errato aggiornamento del collegamento tra il documento e il mezzo di spedizione');</script>";
                                        Response.Write(theAlert);
                                    }
                                }
                            }
                            if (daCancellareCollMSpedDoc && schedaDocumento.tipoProto.Equals("A") && this.isMezzoSpedizioneRequired)
                            {
                                InfoUtente info = UserManager.getInfoUtente(this);
                                if (!DocumentManager.deleteMetodoSpedizione(info, schedaDocumento.systemId))
                                {
                                    string theAlert = "<script>alert('Attenzione! errato aggiornamento del collegamento tra il documento e il mezzo di spedizione');</script>";
                                    Response.Write(theAlert);
                                }
                            }

                            DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                            // fine inserimento in tabella per il mezzo di spedizione

                            //se il documento era privato e si è modificato in pubblico allora devo
                            //verificare se esistono trasmissioni effettuate e in tal caso estendo la visibilità ai superiori.
                            //codice commentato perchè al momento non è possibile trasformare un documento privato in pubblico
                            /*if (daAggiornarePrivato == true)
                            {}*/

                            DocsPaWR.ConfigSpedizioneDocumento config = Spedizione.SpedizioneManager.GetConfigSpedizioneDocumento();

                            if (config.SpedizioneAutomaticaDocumento)
                            {
                                if (!DocumentManager.DO_UpdateVisibilita(this, schedaDocumento, ruolo))
                                {
                                    string theAlert = "<script>alert('Attenzione! Problemi nelle trasmissioni ai nuovi destinatari');</script>";
                                    Response.Write(theAlert);
                                }
                            }
                            this.m_modifica = false;
                            //this.txt_oggetto_P.ReadOnly = true;
                            this.ctrl_oggetto.oggetto_isReadOnly = true;
                            this.ddl_tipoAtto.Enabled = false;
                            this.imgFasc.Enabled = false;
                            this.txt_CodFascicolo.Enabled = false;
                            this.txt_DescFascicolo.Enabled = false;
                            this.ddl_tmpl.Enabled = false;
                            this.btn_titolario.Enabled = false;

                            // Disabilitazione delle note
                            if (this.NotaDocumentoEnabled) this.dettaglioNota.Enabled = false;

                            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null)
                            {
                                if (schedaDocumento.template != null)
                                {
                                    impostaStatoIniziale(schedaDocumento.template);
                                }
                            }

                            /* ABBATANGELI GIANLUIGI - TEST
                             * Commentato il codice sottostante che scrive inutilmente dati in sessione 
                             * 
                            //PROFILAZIONE DINAMICA			
                            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null)
                            {
                                //if (verificaCampiObbligatori((DocsPAWA.DocsPaWR.Templates)Session["template"]))
                                //{
                                //    RegisterStartupScript("alert", "<script>alert('Ci sono dei campi obbligatori relativi al tipo di documento selezionato !');</script>");
                                //    return;
                                //}
                                //Salvataggio dei campi della profilazione dinamica
                                if (Session["template"] != null)
                                {
                                    //wws.salvaInserimentoUtenteProfDim(UserManager.getInfoUtente(this), (DocsPaWR.Templates)Session["template"], schedaDocumento.docNumber, "S");

                                    //schedaDocumento.template = (DocsPaWR.Templates)Session["template"];
                                    //impostaStatoIniziale((DocsPaWR.Templates)Session["template"]);
                                    //Session["template"] = wws.getTemplateDettagli(schedaDocumento.docNumber);

                                    //Verifico che è stato selezionato il calcolo di un contatore,
                                    //in caso affermativo, riapro la popup di profilazione, per far verificare il numero generato
                                    if (Session["contaDopoChecked"] != null)
                                    {
                                        Response.Write("<script>" +
                                                        "var w = window.screen.width; " +
                                                        "var h = window.screen.height; " +
                                                        "var new_w = (w-100)/2; " +
                                                        "var new_h = (h-400)/2; " +
                                                        "window.showModalDialog('AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinamica.aspx','','dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w); " +
                                                        "</script>");
                                        Session.Remove("contaDopoChecked");
                                    }
                                    //Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=protocollo'; </script>");
                                }
                            }
                            * */
                            //FINE PROFILAZIONE DINAMICA
                        }
                        else
                        {
                            Response.Write("<script>alert('" + msg + "');</script>");
                            return;
                        }

                        //GestioneSalvataggioRispostaAlProtocollo();

                        /*if (daAggiornarePrivato == true)
                        {*/
                        if (enableUffRef == true)
                        {

                            this.txt_cod_uffRef.ReadOnly = true;
                            //this.txt_desc_uffRef.ReadOnly = true;
                            this.txt_cod_uffRef.BackColor = Color.WhiteSmoke;
                            this.txt_desc_uffRef.BackColor = Color.WhiteSmoke;
                            this.btn_Rubrica_ref.Enabled = false;
                            schedaDocumento.protocollo.ModUffRef = false;
                            // Trasmissione documento in entrata dopo SALVA
                            if (daAggiornareUffRef == true)
                            {
                                if (this.schedaDocumento.tipoProto == "A")
                                {
                                    string serverName = Utils.getHttpFullPath(this);
                                    bool verificaRagioni;

                                    if (DocumentManager.TrasmettiProtocolloUfficioReferente(this, serverName, this.schedaDocumento, enableUffRef, out verificaRagioni, out message))
                                    {
                                        if (!verificaRagioni)
                                        {
                                            // Notifica utente che la trasmissione non e' stata effettuata
                                            string theAlert = "<script>alert('Attenzione! le trasmissioni non sono state effettuate \\npoiché NON sono presenti le ragioni di trasmissione per l'Ufficio Referente!');</script>";
                                            Response.Write(theAlert);
                                        }
                                    }
                                    else
                                    {
                                        Exception exception = new Exception("Errore durante la trasmissione al protocollo");
                                        ErrorManager.redirect(this, exception, "protocollazione");
                                    }
                                }
                            }
                            else
                            {
                                if (this.txt_cod_uffRef.Text.Equals(""))//se non aggiorno perchè il campo uff ref è vuoto
                                    this.btn_salva_P.Enabled = true;
                            }
                        }
                        Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=protocollo'; </script>");
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.redirect(this, ex, "protocollazione");
                    }
                }

                DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                this.btn_salva_P.Enabled = false;

                DocsPaWR.InfoDocumento infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
                DocsPaWR.Fascicolo[] listaFascicoli = DocumentManager.GetFascicoliDaDoc(this, infoDocumento.idProfile);
                if (listaFascicoli != null && listaFascicoli.Length > 0)
                    FascicoliManager.SetDoFascRapida(this, listaFascicoli[listaFascicoli.Length - 1]);

            }
            logger.Info("END");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_aggiungi_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                if (!this.GetControlAclDocumento().AclRevocata)
                {
                    int isInAdl = DocumentManager.isDocInADL(this.schedaDocumento.systemId, this);
                    string msg = string.Empty;
                    if (isInAdl == 1)
                    {
                        DocumentManager.eliminaDaAreaLavoro(this.Page, this.schedaDocumento.systemId, null);
                        msg = "Documento rimosso dall'Area di Lavoro";
                    }
                    else
                    {
                        DocumentManager.addAreaLavoro(this, schedaDocumento);
                        msg = "Documento aggiunto all'area di lavoro";
                    }
                    Response.Write("<script>alert(\"" + msg + "\");</script>");
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
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

        private void btn_inoltra_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            logger.Info("BEGIN");
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                bool eUffRef = false;
                if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                {
                    eUffRef = true;
                }

                if (schedaDocumento.protocollo != null &&
                    schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloUscita))
                {
                    //Prima di riproporre devo pulire l'oggetto ProtocolloDestinatario per ogni destinatario del protocollo
                    DocsPaWR.Corrispondente destinatario;
                    if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari != null)
                    {
                        for (int i = 0; i < ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari.Length; i++)
                        {
                            destinatario = (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[i]);
                            destinatario.protocolloDestinatario = null;
                        }
                    }
                }
                schedaDocumento = DocumentManager.InoltraDocumento(this, schedaDocumento, eUffRef);

                Session["type_inoltro"] = schedaDocumento.tipoProto;

                //Inizio sviluppo estensione delle funzionalità di fascicolazione e trasmissione rapida
                DocsPaWR.Fascicolo fasc;
                string segnatura = "";
                string returnMsg = "";
                fasc = getFascicolo();
                if (fasc != null)
                {
                    if (fasc.stato == "C")
                    {
                        returnMsg += "Il fascicolo scelto è chiuso. Pertanto il documento non è stato fascicolato";
                    }
                    else
                    {
                        if (schedaDocumento.protocollo != null)
                            segnatura = schedaDocumento.protocollo.segnatura;

                        string msg = string.Empty;
                        if (!DocumentManager.fascicolaRapida(this, schedaDocumento.systemId, schedaDocumento.docNumber, segnatura, fasc, out msg))
                        {
                            if (string.IsNullOrEmpty(msg))
                                returnMsg += " Il documento non è stato fascicolato";
                            else
                                returnMsg += " " + msg;
                        }
                        else
                        {
                            schedaDocumento.fascicolato = "1";
                        }
                    }
                    if (!returnMsg.Equals(""))
                        Response.Write("<script>alert('" + returnMsg + "');</script>");
                    FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                    FascicoliManager.removeCodiceFascRapida(this);
                    FascicoliManager.removeDescrizioneFascRapida(this);
                }


                if (ddl_tmpl.SelectedIndex > 0)
                {
                    execTrasmRapida();
                    ddl_tmpl.SelectedIndex = 0;
                }
                //Fine sviluppo estensione delle funzionalità di fascicolazione e trasmissione rapida    


                FileManager.removeSelectedFile(this);
                schedaDocumento.predisponiProtocollazione = true;

                DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                DocumentManager.setDocumentoSelezionato(schedaDocumento);
                if (this.rbl_InOut_P.SelectedItem.Value == "Own")
                {
                    this.txt_DescMit_P.ReadOnly = true;
                    this.txt_DescDest_P.ReadOnly = true;
                }
                if (this.rbl_InOut_P.SelectedItem.Value == "Out")
                {
                    this.txt_DescMit_P.ReadOnly = true;
                    this.txt_DescDest_P.ReadOnly = false;
                }
                this.rbl_InOut_P.Enabled = true;
                this.ddl_tmpl.SelectedIndex = -1;

                Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=protocollo&inoltra=1';</script>");
            }
            logger.Info("END");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_riproponiDati_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            logger.Info("BEGIN");
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                if ((this.schedaDocumento.documenti != null && (!string.IsNullOrEmpty(this.schedaDocumento.documenti[0].fileName)))
                    || (this.schedaDocumento.allegati != null && schedaDocumento.allegati.Length > 0))
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["RIPROPONI_AVANZATO_ENABLED"] != null && System.Configuration.ConfigurationManager.AppSettings["RIPROPONI_AVANZATO_ENABLED"].ToUpper() == "TRUE")
                    {
                        string messaggio = InitMessageXml.getInstance().getMessage("docRiproponiCopiaDocumenti");
                        msg_copiaDoc.Confirm(messaggio);
                    }
                    else riproponi();
                }
                else
                {
                    riproponi();
                }
            }
            logger.Info("END");
        }

        private void riproponi()
        {
            logger.Info("BEGIN");
            DocumentManager.removeMemoriaFiltriRicDoc(this);
            DocumentManager.RemoveMemoriaVisualizzaBack(this);

            //luluciani 08/02/2007
            TrasmManager.RemoveMemoriaVisualizzaBack(this);
            if (this.ddl_tmpl.SelectedIndex == 0 && Session["Modello"] != null)
                Session.Remove("Modello");
            Session.Add("docRiproposto", true);

            DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];

            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
            {
                string diritti = string.Empty;

                if (this.schedaDocumento == null ||
                    (this.schedaDocumento != null && string.IsNullOrEmpty(this.schedaDocumento.systemId)))
                    // In modalità di inserimento, il reperimento delle tipologie documenti è più restrittivo:
                    // sono reperiti solamente le tipologie per cui il ruolo corrente ha il diritto di creazione oltre che di lettura
                    diritti = "2";
                else
                    // Modalità di ricerca: sono reperite le tipologie 
                    // per cui il ruolo corrente dispone almeno del diritto di lettura
                    diritti = "1";

                listaTipologiaAtto = DocumentManager.getTipoAttoPDInsRic(this, UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, diritti);
            }
            else
                listaTipologiaAtto = DocumentManager.getListaTipologiaAtto(this);
            bool trovato = false;
            if (listaTipologiaAtto != null && listaTipologiaAtto.Length > 0)
            {
                for (int i = 0; i < listaTipologiaAtto.Length; i++)
                {
                    if (template != null)
                    {
                        if (Int32.Parse(listaTipologiaAtto[i].systemId) == template.SYSTEM_ID)
                        {
                            Session.Add("templateRiproposto", template);
                            if (this.ddl_tipoAtto != null && this.ddl_tipoAtto.SelectedItem != null)
                            {
                                Session.Add("tipoAttoRipropostoTesto", this.ddl_tipoAtto.SelectedItem.Text);
                                Session.Add("tipoAttoRipropostoId", this.ddl_tipoAtto.SelectedItem.Value);
                                trovato = true;
                                break;
                            }
                        }
                    }
                }
                if (!trovato)
                {
                    template = null; //rimuovo i dati perchè non vedo la tipologia riproposta.
                    if (this.ddl_tipoAtto != null && this.ddl_tipoAtto.SelectedItem != null)
                        this.ddl_tipoAtto.SelectedIndex = 0;
                    this.schedaDocumento.tipologiaAtto = null;
                    this.schedaDocumento.template = null;
                }
            }

            // Se è attiva la funzionalità di riproponi con conoscenza, deve essere riproposta anche la fascicolazione
            if (this.IsEnabledRiproponiConConoscenza)
            {
                InfoDocumento infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
                Fascicolo[] listaFascicoli = DocumentManager.GetFascicoliDaDoc(this, infoDocumento.idProfile);
                if (listaFascicoli != null && listaFascicoli.Length > 0)
                    FascicoliManager.SetDoFascRapida(this, listaFascicoli[listaFascicoli.Length - 1]);
            }

            //verifica se labelFascRapid tipologia riproposta fa parte di
            //  quelle in visibilità al Ruolo, se no Non la ripropongo.

            bool eUffRef = false;
            if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
            {
                eUffRef = true;
            }

            //int lungh = schedaDocumento.paroleChiave.Length;

            //DocsPaWR.DocumentoParolaChiave[] listaParole = (DocsPaWR.DocumentoParolaChiave[])Session["ricDoc.listaParoleChiave"];
            //if (listaParole != null)
            //{
            //    //this.setParoleChiavi(listaParole);
            //    schedaDocumento.paroleChiave = addParoleChiaveToDoc(schedaDocumento, listaParole);
            //    //schedaDocumento.daAggiornareParoleChiave = true;
            //    //DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
            //    Session.Remove("ricDoc.listaParoleChiave");
            //}

            //Prima di riproporre devo pulire l'oggetto ProtocolloDestinatario per ogni destinatario del protocollo
            DocsPaWR.Corrispondente destinatario;
            if (schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloUscita)))
            {
                if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari != null)
                {
                    for (int i = 0; i < ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari.Length; i++)
                    {
                        destinatario = (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[i]);
                        destinatario.protocolloDestinatario = null;
                    }
                }
                if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza != null)
                {
                    for (int i = 0; i < ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza.Length; i++)
                    {
                        destinatario = (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza[i]);
                        destinatario.protocolloDestinatario = null;
                    }
                }
            }
            if (schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloInterno)))
            {
                if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari != null)
                {
                    for (int i = 0; i < ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatari.Length; i++)
                    {
                        destinatario = (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatari[i]);
                        destinatario.protocolloDestinatario = null;
                    }
                }
                if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza != null)
                {
                    for (int i = 0; i < ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatariConoscenza.Length; i++)
                    {
                        destinatario = (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatariConoscenza[i]);
                        destinatario.protocolloDestinatario = null;
                    }
                }

            }

            schedaDocumento = DocumentManager.riproponiDati(this, schedaDocumento, eUffRef);
            schedaDocumento.isRiprodotto = true;
            //Folder[] ii = schedaDocumento.folder;
            //if (ii != null)
            //{
            //    foreach (Folder ff in ii)
            //    {
            //        this.txt_CodFascicolo.Text = ff.idFascicolo;
            //        this.txt_DescFascicolo.Text = ff.descrizione;
            //        break;
            //    }
            //}
            //else
            //{
            //    this.txt_CodFascicolo.Text = "";
            //    this.txt_DescFascicolo.Text = "";
            //}

            //predispongo il nuovo documento alla protocollazione
            //DocumentManager.setDocumentoSelezionato(this, schedaDocumento);

            FileManager.removeSelectedFile(this);
            //schedaDocumento.predisponiProtocollazione = true;

            DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
            DocumentManager.setDocumentoSelezionato(this, schedaDocumento);

            if (this.rbl_InOut_P.SelectedItem.Value == "Own")
            {
                this.txt_DescMit_P.ReadOnly = true;
                this.txt_DescDest_P.ReadOnly = true;
            }
            if (this.rbl_InOut_P.SelectedItem.Value == "Out")
            {
                this.txt_DescMit_P.ReadOnly = true;
                this.txt_DescDest_P.ReadOnly = false;
            }
            this.rbl_InOut_P.Enabled = true;
            this.ddl_tmpl.SelectedIndex = -1;

            if (Session["rubrica.campoCorrispondente"] != null)
                Session.Remove("rubrica.campoCorrispondente");
            if (Session["CorrSelezionatoDaMulti"] != null)
                Session.Remove("CorrSelezionatoDaMulti");

            Session["type_riproponi"] = schedaDocumento.tipoProto;
            Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=protocollo';</script>");
            logger.Info("END");
        }

        private void riproponiConCopia()
        {
            logger.Info("BEGIN");
            DocumentManager.removeMemoriaFiltriRicDoc(this);
            DocumentManager.RemoveMemoriaVisualizzaBack(this);
            //luluciani 08/02/2007
            TrasmManager.RemoveMemoriaVisualizzaBack(this);
            //Session.Remove("Modello");
            Session.Add("docRiproposto", true);
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
            Session.Add("templateRiproposto", template);
            Session.Add("tipoAttoRipropostoTesto", this.ddl_tipoAtto.SelectedItem.Text);
            Session.Add("tipoAttoRipropostoId", this.ddl_tipoAtto.SelectedItem.Value);

            // Se è attiva la funzionalità di riproponi con conoscenza, deve essere riproposta anche la fascicolazione
            if (this.IsEnabledRiproponiConConoscenza)
            {
                InfoDocumento infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
                Fascicolo[] listaFascicoli = DocumentManager.GetFascicoliDaDoc(this, infoDocumento.idProfile);
                if (listaFascicoli != null && listaFascicoli.Length > 0)
                    FascicoliManager.SetDoFascRapida(this, listaFascicoli[listaFascicoli.Length - 1]);
            }

            //TODO:
            bool eUffRef = false;
            if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
            {
                eUffRef = true;
            }
            Session.Add("scDocCopiato", schedaDocumento);

            if (schedaDocumento.protocollo != null &&
                schedaDocumento.protocollo.GetType() == typeof(DocsPaWR.ProtocolloUscita))
            {
                //Prima di riproporre devo pulire l'oggetto ProtocolloDestinatario per ogni destinatario del protocollo
                DocsPaWR.Corrispondente destinatario;
                if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari != null)
                {
                    for (int i = 0; i < ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari.Length; i++)
                    {
                        destinatario = (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[i]);
                        destinatario.protocolloDestinatario = null;
                    }
                }
            }

            schedaDocumento = DocumentManager.riproponiConCopiaDoc(this, schedaDocumento, eUffRef);
            //schedaDocumento.predisponiProtocollazione = true;
            FileManager.removeSelectedFile(this);
            DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
            DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
            if (this.rbl_InOut_P.SelectedItem.Value == "Own")
            {
                this.txt_DescMit_P.ReadOnly = true;
                this.txt_DescDest_P.ReadOnly = true;
            }
            if (this.rbl_InOut_P.SelectedItem.Value == "Out")
            {
                this.txt_DescMit_P.ReadOnly = true;
                this.txt_DescDest_P.ReadOnly = false;
            }
            this.rbl_InOut_P.Enabled = true;
            this.ddl_tmpl.SelectedIndex = -1;

            Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=protocollo';</script>");
            logger.Info("END");
        }


        //private DocsPAWA.DocsPaWR.DocumentoParolaChiave[] addParoleChiaveToDoc(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento, DocsPAWA.DocsPaWR.DocumentoParolaChiave[] listaDocParoleChiave)
        //{
        //    DocsPaWR.DocumentoParolaChiave[] listaPC;
        //    listaPC = schedaDocumento.paroleChiave;
        //    if (listaDocParoleChiave != null)
        //    {
        //        for (int i = 0; i < listaDocParoleChiave.Length; i++)
        //        {
        //            if (!listaContains(schedaDocumento.paroleChiave, listaDocParoleChiave[i]))
        //                listaPC = Utils.addToArrayParoleChiave(listaPC, listaDocParoleChiave[i]);
        //        }
        //    }
        //    return listaPC;

        //}

        //private bool listaContains(DocsPAWA.DocsPaWR.DocumentoParolaChiave[] lista, DocsPAWA.DocsPaWR.DocumentoParolaChiave el)
        //{
        //    bool trovato = false;
        //    if (lista != null)
        //    {
        //        for (int i = 0; i < lista.Length; i++)
        //        {
        //            if (lista[i].systemId.Equals(el.systemId))
        //            {
        //                trovato = true;
        //                break;
        //            }
        //        }
        //    }
        //    return trovato;
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_CodDest_P_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                setDescCorrispondente(this.txt_CodDest_P.Text, "Dest", true);
                if (txt_CodDest_P.Text == "")
                    txt_DescDest_P.Text = "";
                Session.Add("modificaDescrDest", true);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_DescDest_P_TextChanged(object sender, System.EventArgs e)
        {
            if (Session["modificaDescrDest"] != null)
                Session.Remove("modificaDescrDest");
            else
                this.txt_CodDest_P.Text = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_aggiungiDest_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //cerca il corrispondente e lo aggiunge tra i destinatari principali
            DocsPaWR.Corrispondente corr = null;
            RubricaCallType calltype = get_calltype("Dest");
            ArrayList lsCorr = new ArrayList();

            if ((this.txt_CodDest_P.Text != null && !this.txt_CodDest_P.Text.Trim().Equals(""))) //|| (!(this.GetCorrispondenteControl("CorrDaCodDest").CODICE_TEXT.Trim() != "") && !(this.GetCorrispondenteControl("CorrDaCodDest").DESCRIZIONE_TEXT.Trim()!="")))
            {
                //string cod = this.GetCorrispondenteControl("CorrDaCodDest").CODICE_TEXT;
                //string desc = this.GetCorrispondenteControl("CorrDaCodDest").DESCRIZIONE_TEXT;
                //Per le liste
                if (System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null && System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "1")
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    lsCorr = UserManager.getCorrispondentiByCodLista(this, txt_CodDest_P.Text, idAmm);
                    //lsCorr = UserManager.getCorrispondentiByCodLista(this, cod,idAmm);
                    //Giordano Iacozzilli: 23/09/2013
                    //Task allineamento versioni LV_220 TO Dep-Cons2
                    //ID Changeset:#37338
                    //if (lsCorr.Count != 0)
                    if (lsCorr != null && lsCorr.Count != 0)
                    {
                        corr = new DocsPAWA.DocsPaWR.Corrispondente();
                        corr.codiceRubrica = txt_CodDest_P.Text;
                        corr.descrizione = UserManager.getNomeLista(this, txt_CodDest_P.Text, idAmm);
                        corr.systemId = UserManager.getSystemIdLista(this, txt_CodDest_P.Text);
                        //corr.descrizione = UserManager.getNomeLista(this, cod,idAmm);
                        //corr.systemId = UserManager.getSystemIdLista(this, desc);
                        corr.tipoCorrispondente = "L";
                    }
                    else
                    {
                        // nel caso di RF
                        lsCorr = UserManager.getCorrispondentiByCodRF(this, txt_CodDest_P.Text);
                        //Giordano Iacozzilli: 23/09/2013
                        //Task allineamento versioni LV_220 TO Dep-Cons2
                        //ID Changeset:#37338
                        //if (lsCorr != null && lsCorr.Count != 0)
                        if (lsCorr != null && lsCorr.Count != 0)
                        {
                            corr = new DocsPAWA.DocsPaWR.Corrispondente();
                            corr.codiceRubrica = txt_CodDest_P.Text;
                            corr.descrizione = UserManager.getNomeRF(this, txt_CodDest_P.Text);
                            corr.systemId = UserManager.getSystemIdRF(this, txt_CodDest_P.Text);
                            corr.tipoCorrispondente = "F";
                        }
                        else
                        //corr = UserManager.getCorrispondente(this, this.txt_CodDest_P.Text,true);
                        {
                            //corr = UserManager.getCorrispondenteRubrica(this, this.txt_CodDest_P.Text, calltype);
                            ElementoRubrica[] listaCorr = UserManager.getElementiRubricaMultipli(this, this.txt_CodDest_P.Text, calltype, true);
                            if (listaCorr != null && listaCorr.Length > 0)
                            {
                                if (listaCorr.Length == 1)
                                {
                                    if (!string.IsNullOrEmpty(listaCorr[0].systemId))
                                        corr = UserManager.getCorrispondenteBySystemID(this.Page, listaCorr[0].systemId);
                                    else
                                        corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, listaCorr[0].codice);
                                }
                                else
                                {
                                    if (this.hiddenDest.Value.Equals("0"))
                                    {
                                        ClientScript.RegisterStartupScript(this.GetType(), "multiCorr", "ApriFinestraMultiCorrispondentiMittenti();", true);
                                        Session.Add("multiCorr", listaCorr);
                                        return;
                                    }
                                    else
                                    {
                                        if (this.hiddenDest.Value.Equals("2"))
                                            corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, this.txt_CodDest_P.Text);
                                        else
                                        {
                                            foreach (ElementoRubrica el in listaCorr)
                                            {
                                                if (el.codiceRegistro != null && el.codiceRegistro.ToUpper().Equals(this.hiddenDest.Value.ToUpper()))
                                                {
                                                    if (!string.IsNullOrEmpty(el.systemId))
                                                        corr = UserManager.getCorrispondenteBySystemID(this.Page, el.systemId);
                                                }
                                            }
                                        }
                                        this.hiddenDest.Value = "0";
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //corr = UserManager.getCorrispondente(this, this.txt_CodDest_P.Text,true);	
                    corr = UserManager.getCorrispondenteRubrica(this, this.txt_CodDest_P.Text, calltype);
                }
            }
            else
            {
                if (!this.txt_DescDest_P.Text.Trim().Equals(""))
                {
                    corr = new DocsPAWA.DocsPaWR.Corrispondente();
                    corr.descrizione = this.txt_DescDest_P.Text;
                    corr.tipoCorrispondente = "O";
                    corr.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
                }
            }

            if (corr != null)
            {
                DocsPaWR.Corrispondente[] listaDestCC = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza;
                if (!UserManager.esisteCorrispondente(listaDestCC, corr))
                {
                    DocsPaWR.Corrispondente[] listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari;
                    if (!UserManager.esisteCorrispondente(listaDest, corr))
                    {
                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari = UserManager.addCorrispondenteModificato(listaDest, listaDestCC, corr);

                        if (this.rbl_InOut_P.SelectedItem.Value == "Own")
                        {
                            ArrayList corrTotali = new ArrayList();
                            bool allInt = true;
                            for (int i = 0; i < ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari.Length; i++)
                            {
                                DocsPaWR.Corrispondente c = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[i];
                                //Controllo del corrispondente che si intende aggiungere ai destinatari partendo da un protocollo INTERNO
                                if (c.tipoIE == "I")
                                {
                                    if (c.tipoCorrispondente == "R")
                                    {
                                        //										for(int k=0; k<((DocsPAWA.DocsPaWR.Ruolo) c).registri.Length; k++)
                                        //										{
                                        //											if(((DocsPAWA.DocsPaWR.Ruolo) c).registri[k].systemId == schedaDocumento.registro.systemId)
                                        //												corrTotali.Add(c);
                                        //										}
                                        if (UserManager.VerificaAutorizzazioneRuoloSuRegistro(this, schedaDocumento.registro.systemId, c.systemId))
                                            corrTotali.Add(c);
                                    }
                                    if (c.tipoCorrispondente == "U")
                                    {
                                        string[] codiciAooAutorizzate = UserManager.GetUoInterneAoo(this);
                                        for (int j = 0; j < codiciAooAutorizzate.Length; j++)
                                        {
                                            if (c.codiceRubrica == codiciAooAutorizzate[j])
                                                corrTotali.Add(c);
                                        }
                                    }
                                    if (c.tipoCorrispondente == "P")
                                    {
                                        string[] systemIdRuoliAutorizzati = UserManager.getUtenteInternoAOO(((Utente)c).idPeople, schedaDocumento.registro.systemId, this);
                                        if (systemIdRuoliAutorizzati.Length != 0)
                                            corrTotali.Add(c);
                                    }
                                }
                                else
                                {
                                    allInt = false;
                                }
                            }

                            //if (corr.tipoCorrispondente == "L" && lsCorr.Count != corrTotali.Count)
                            if (!allInt)
                            {
                                int corrCancella = 0;
                                for (int i = 0; i < ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari.Length; i++)
                                {
                                    DocsPaWR.Corrispondente c = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[i];
                                    if (c.codiceCorrispondente == corr.codiceCorrispondente)
                                    {
                                        corrCancella = i;
                                        break;
                                    }
                                }
                                ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari = UserManager.removeCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari, corrCancella);
                                //((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari = UserManager.addCorrispondenteModificato(listaDest, listaDestCC, corr);
                                //RegisterStartupScript("script", "<script>alert('Non è possibile inserire destinatari occasionali');</script>");
                                if (corr.tipoCorrispondente == "O")
                                {
                                    ClientScript.RegisterStartupScript(this.GetType(), "script", "alert('Non è possibile inserire destinatari occasionali');", true);
                                }
                                else
                                {
                                    ClientScript.RegisterStartupScript(this.GetType(), "script", "alert('Non è possibile inserire destinatari esterni');", true);
                                }
                                //((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari = null;
                                return;
                            }
                        }

                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatari = true;
                        DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                    }
                }
            }

            //Modifica Destinatari
            if (!this.txt_DescDest_P.Text.Trim().Equals("")) // && schedaDocumento.protocollo.numero != null)
            {
                schedaDocumento = DocumentManager.DO_AddDestinatrioModificato(schedaDocumento, corr.systemId);
                DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
            }

            //il campo della descrizione del corrispondente deve essere in sola lettura 
            //quando il tipo di protocollo è INTERNO
            if (this.rbl_InOut_P.SelectedItem.Value == "Own")
            {
                this.txt_CodDest_P.Text = "";
                this.txt_DescDest_P.Text = "";
                if (string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) || !(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                {
                    this.txt_DescDest_P.ReadOnly = true;
                }
            }
            else
            {
                this.txt_CodDest_P.Text = "";
                this.txt_DescDest_P.Text = "";
            }

            string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_CodDest_P.ID + "').focus() </SCRIPT>";
            RegisterStartupScript("focus", s);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_insDestCC_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //inserisco il destinatario selezionato tra i destinatari per conoscenza e lo rimuovo dai destinatari
            try
            {
                //Modifica Destinatari
                //if (schedaDocumento.protocollo.numero != null)
                //{
                string system_id = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[this.lbx_dest.SelectedIndex].systemId;
                schedaDocumento = DocumentManager.DO_AddDestinatrioCCModificato(schedaDocumento, system_id);
                schedaDocumento = DocumentManager.DO_RemoveDestinatarioModificati(schedaDocumento, system_id);
                //}

                if (this.lbx_dest.SelectedIndex >= 0)
                {
                    //cerca il corrispondente e lo aggiunge tra i destinatari principali
                    addDestinatari(this.lbx_dest.SelectedIndex, "C");
                    removeDestinatari(this.lbx_dest.SelectedIndex, "P");
                    DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                    DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                }
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_insDest_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                //Modifica Destinatari
                //if (schedaDocumento.protocollo.numero != null)
                //{
                if (this.lbx_dest.SelectedIndex >= 0 && !string.IsNullOrEmpty(((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza[this.lbx_dest.SelectedIndex].systemId))
                {
                    string system_id = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza[this.lbx_dest.SelectedIndex].systemId;
                    schedaDocumento = DocumentManager.DO_AddDestinatrioModificato(schedaDocumento, system_id);
                    schedaDocumento = DocumentManager.DO_RemoveDestinatarioCCModificati(schedaDocumento, system_id);
                }

                //inserisco il destinatario per conoscenza selezionato tra i destinatari e lo rimuovo dai destinatari per conoscenza
                if (this.lbx_destCC.SelectedIndex >= 0)
                {
                    //cerca il corrispondente e lo aggiunge tra i destinatari principali
                    addDestinatari(this.lbx_destCC.SelectedIndex, "P");
                    removeDestinatari(this.lbx_destCC.SelectedIndex, "C");
                    DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                    DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                }
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_cancDest_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (this.lbx_dest.SelectedIndex >= 0)
            {
                schedaDocumento = DocumentManager.DO_RemoveDestinatarioModificati(schedaDocumento, ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[this.lbx_dest.SelectedIndex].systemId);
                removeDestinatari(this.lbx_dest.SelectedIndex, "P");
                DocumentManager.setDocumentoSelezionato(schedaDocumento);
                DocumentManager.setDocumentoInLavorazione(schedaDocumento);
            }
            else
            {
                string msg = "Destinatario non selezionato";
                Response.Write("<script>alert('" + msg + "')</script>");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_cancDestCC_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (this.lbx_destCC.SelectedIndex >= 0)
            {
                schedaDocumento = DocumentManager.DO_RemoveDestinatarioCCModificati(schedaDocumento, ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza[this.lbx_destCC.SelectedIndex].systemId);
                removeDestinatari(this.lbx_destCC.SelectedIndex, "C");
                DocumentManager.setDocumentoSelezionato(schedaDocumento);
                DocumentManager.setDocumentoInLavorazione(schedaDocumento);
            }
            else
            {
                string msg = "Destinatario non selezionato";
                Response.Write("<script>alert('" + msg + "')</script>");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void setDataProtocollo()
        {
            //Da richiamare solo se il documento non è protocollato
            if (schedaDocumento.protocollo != null)
            {
                if ((schedaDocumento.protocollo.segnatura != null && !schedaDocumento.protocollo.segnatura.Equals("")))
                {
                    this.txt_dataSegn.ReadOnly = true;
                    this.btn_RubrOgget_P.Enabled = false;

                    return;
                }
            }

            DocsPaWR.Registro reg = schedaDocumento.registro;
            if (reg != null)
            {
                string stato = UserManager.getStatoRegistro(reg);
                if (stato.Equals("V"))
                {
                    if (schedaDocumento.protocollo.dataProtocollazione == null || schedaDocumento.protocollo.dataProtocollazione.Equals(""))
                        schedaDocumento.protocollo.dataProtocollazione = reg.dataApertura;
                    this.txt_dataSegn.ReadOnly = true;
                }
                else
                    if (stato.Equals("G"))
                    {
                        if (schedaDocumento.protocollo.dataProtocollazione == null || schedaDocumento.protocollo.dataProtocollazione.Equals(""))
                            schedaDocumento.protocollo.dataProtocollazione = Utils.getMaxDate(reg.dataApertura, reg.dataUltimoProtocollo);
                        this.txt_dataSegn.ReadOnly = false;
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_dataSegn_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                schedaDocumento.protocollo.dataProtocollazione = this.txt_dataSegn.Text;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }
        }

        /// <summary>
        /// t
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_modificaOgget_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Se il documento è in checkout non può essere modificato 
            if (this.IsDocumentInCheckOutState())
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "avvisoDocumentoInCheckOut",
                    "alert('Non è possibile effettuare il salvataggio in quanto il documento oppure almeno un suo allegato risulta bloccato.');",
                    true);
            else
            {
                Session.Add("refreshDxPageProf", true);
                this.EnableModify();
            }

        }

        /// <summary>
        /// Funzione per la verifica dello stato di checkout del documento selezionato
        /// </summary>
        /// <returns>True se il documento è bloccato</returns>
        private bool IsDocumentInCheckOutState()
        {
            // La scheda del documento selezionato
            SchedaDocumento sd;

            // Reperimento della scheda del documento selezionato
            sd = DocumentManager.getDocumentoSelezionato();

            // Restituzione dello stato di checkout
            return CheckInOut.CheckInOutServices.IsCheckedOutDocument(
                sd.systemId,
                sd.docNumber,
                UserManager.getInfoUtente(this),
                true);
        }

        /// <summary>
        /// Funzione per l'abilitazione dei controlli in modifica
        /// </summary>
        private void EnableModify()
        {
            if (UserManager.ruoloIsAutorized(this, "DO_PROT_OG_MODIFICA"))
            {
                //this.txt_oggetto_P.ReadOnly = false;
                this.ctrl_oggetto.oggetto_isReadOnly = false;
            }
            else
            {
                //this.txt_oggetto_P.ReadOnly = true;
                this.ctrl_oggetto.oggetto_isReadOnly = true;
            }

            if (enableUfficioRef != null && enableUfficioRef.Equals("1") && UserManager.ruoloIsAutorized(this, "DO_PROT_MODIFICA_UFF_REF"))
            {
                if (schedaDocumento.tipoProto.Equals("A"))
                {
                    this.txt_cod_uffRef.ReadOnly = false;
                    //					this.txt_desc_uffRef.ReadOnly = false;
                    this.txt_cod_uffRef.BackColor = Color.White;
                    this.txt_desc_uffRef.BackColor = Color.White;
                    this.btn_Rubrica_ref.Enabled = true;
                    set_btn_Rubrica_ref_event();
                }
            }

            if (schedaDocumento.tipoProto.Equals("A") && this.isMezzoSpedizioneRequired)
            {
                this.lbl_mezzoSpedizione.Visible = false;
                this.pnl_mezzoSpedizione.Visible = true;
                this.ddl_spedizione.Visible = true;

                // Se il documento è stato ricevuto per interoperabilità semplificata, per mail o per interoperabilità
                // classica, viene congelato il menù a tendina dei mezzi di spedizione in quanto non deve essere 
                // possibile cambiare il mezzo di spedizione
                if (schedaDocumento.typeId == InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId || schedaDocumento.typeId == "INTEROPERABILITA")
                    this.ddl_spedizione.Enabled = false;

                //    for (int i = 0; i < this.ddl_spedizione.Items.Count; i++)
                //    {
                //        if (this.ddl_spedizione.Items[i].Value == schedaDocumento.mezzoSpedizione.ToString())
                //            this.ddl_spedizione.Items[i].Selected = true;
                //    }

                //    ////this.ddl_spedizione.SelectedIndex = Convert.ToInt32(schedaDocumento.protocollo.mezzoSpedizione);
                //    //this.ddl_spedizione.Visible = true;
                //    ////this.ddl_spedizione.SelectedItem.Text = schedaDocumento.descMezzoSpedizione;
                //    //this.ddl_spedizione.SelectedItem.Value = schedaDocumento.descMezzoSpedizione;
                //    ////this.ddl_spedizione.Items[schedaDocumento.mezzoSpedizione].Selected = true;
            }

            this.btn_RubrOgget_P_state = true;

            this.btn_RubrOgget_P.Enabled = true;

            this.m_modifica = true;
            this.btn_salva_P.Enabled = true;

            this.ViewState["EditMode"] = true;
            enableEditableFields();

            // disabilitazione campo note                    
            if (this.NotaDocumentoEnabled) this.dettaglioNota.Enabled = true;

            if (UserManager.ruoloIsAutorized(this, "DO_PROT_MIT_MODIFICA"))
            {
                this.txt_CodMit_P.ReadOnly = false;
                //this.txt_codMittMultiplo.ReadOnly = false;
                if (schedaDocumento.tipoProto.Equals("A"))
                {
                    this.txt_DescMit_P.ReadOnly = false;
                    //this.txt_descMittMultiplo.ReadOnly = false;
                    btn_downMittente.Enabled = true;
                    btn_upMittente.Enabled = true;
                    //btn_insMittMultiplo.Enabled = true;
                    btn_CancMittMultiplo.Enabled = true;
                    btn_RubrMittMultiplo.Disabled = false;
                    if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                    {
                        if (DocumentManager.isEnableMittentiMultipli())
                        {
                            this.rubrica_veloce_mitt_multi.Visible = true;
                        }
                    }
                }
                else
                {
                    this.txt_DescMit_P.ReadOnly = true;
                    //this.txt_descMittMultiplo.ReadOnly = true;
                    btn_downMittente.Enabled = false;
                    btn_upMittente.Enabled = false;
                    //btn_insMittMultiplo.Enabled = false;
                    btn_CancMittMultiplo.Enabled = false;
                    btn_RubrMittMultiplo.Disabled = true;
                }
                this.btn_rubrica_p_state = true;
                this.btn_rubrica_p_sempl_state = true;
                this.m_modifica = true;
                this.btn_salva_P.Enabled = true;
                if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                {
                    this.rubrica_veloce.Visible = true;
                }

                if (!string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                {
                    this.pnl_mittente_veloce.Visible = true;
                }
            }

            if (UserManager.ruoloIsAutorized(this, "DO_PROT_MIT_INT_MODIFICA"))
            {
                this.txt_CodMitInt_P.ReadOnly = false;
                this.btn_RubrMitInt_state = true;
                this.btn_RubrMitInt_Sempl_state = true;
                this.m_modifica = true;
                this.btn_salva_P.Enabled = true;
            }

            if (UserManager.ruoloIsAutorized(this, "DO_PROT_DEST_MODIFICA") && UserManager.ruoloIsAutorized(this, "DO_PROT_DESTCC_MODIFICA"))
            {
                this.btn_cancDest.Enabled = true;
                this.btn_cancDestCC.Enabled = true;
                this.btn_insDest.Enabled = true;
                this.btn_insDestCC.Enabled = true;
                this.txt_CodDest_P.ReadOnly = false;
                this.txt_DescDest_P.ReadOnly = false;
                this.btn_RubrDest_P_state = true;
                this.btn_RubrDest_Sempl_P_state = true;
                this.m_modifica = true;
                if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
                {
                    this.rubrica_veloce_destinatario.Visible = true;
                }
                if (!string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                {
                    this.pnl_destinatario_veloce.Visible = true;
                }
            }

            Session.Add("isDocModificato", true);
            Session.Add("abilitaModificaSpedizione", true);

            //if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
            //{
            //    if (Session["Template"] != null)
            //    {
            //        string pulsanti = btn_salva_P.Enabled + "-" + btn_protocolla_P.Enabled + "-" + btn_protocollaGiallo_P.Enabled;
            //        ClientScript.RegisterStartupScript(this.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1&pulsanti=" + pulsanti + "'", true);
            //    }
            //}

            this.btn_riproponiDati_P.Enabled = false;
            this.btn_spedisci_P.Enabled = false;

            //Se ricevuto per interoperabilità con pec questi campi non sono modificabili se valorizzati
            if (this.FromInteropPecOrSimpInterop(schedaDocumento))
            {
                if (string.IsNullOrEmpty(schedaDocumento.protocollo.segnatura))
                {
                    txt_CodMit_P.ReadOnly = Convert.ToBoolean(ViewState["varFromInteropPecMit"]);
                    txt_DescMit_P.ReadOnly = Convert.ToBoolean(ViewState["varFromInteropPecMit"]);
                    this.btn_rubrica_p.Disabled = Convert.ToBoolean(ViewState["varFromInteropPecMit"]);
                }

                if (!string.IsNullOrEmpty(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).descrizioneProtocolloMittente))
                {
                    this.txt_NumProtMit_P.ReadOnly = Convert.ToBoolean(ViewState["varFromInteropPecProtMitt"]);
                }

                if (!string.IsNullOrEmpty(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).dataProtocolloMittente))
                {
                    bool valRes = Convert.ToBoolean(ViewState["varFromInteropPecDataMitt"]);
                    this.GetCalendarControl("txt_DataProtMit_P").txt_Data.ReadOnly = valRes;
                    this.GetCalendarControl("txt_DataProtMit_P").btn_Cal.Enabled = !valRes;
                }

                if (!string.IsNullOrEmpty(this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text))
                {
                    bool valRes = Convert.ToBoolean(ViewState["varFromInteropPecDataArrivo"]);
                    this.GetCalendarControl("txt_DataArrivo_P").txt_Data.ReadOnly = valRes;
                    this.GetCalendarControl("txt_DataArrivo_P").btn_Cal.Enabled = !valRes;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ModMit_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.txt_CodMit_P.ReadOnly = false;
            this.txt_DescMit_P.ReadOnly = false;
            this.btn_rubrica_p_state = true;
            this.btn_rubrica_p_sempl_state = true;
            this.m_modifica = true;
            this.btn_salva_P.Enabled = true;
            if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
            {
                this.rubrica_veloce.Visible = true;
            }

            if (!string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
            {
                this.pnl_mittente_veloce.Visible = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ModMitInt_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.txt_CodMitInt_P.ReadOnly = false;
            this.btn_RubrMitInt_state = true;
            this.btn_RubrMitInt_Sempl_state = true;
            this.m_modifica = true;
            this.btn_salva_P.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ModDest_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.btn_cancDest.Enabled = true;
            this.btn_cancDestCC.Enabled = true;
            this.btn_insDest.Enabled = true;
            this.btn_insDestCC.Enabled = true;
            this.txt_CodDest_P.ReadOnly = false;
            this.txt_DescDest_P.ReadOnly = false;
            this.btn_RubrDest_P_state = true;
            this.btn_RubrDest_Sempl_P_state = true;
            this.m_modifica = true;
            if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
            {
                this.rubrica_veloce_destinatario.Visible = true;
            }
            if (!string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) && (utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
            {
                this.pnl_destinatario_veloce.Visible = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_verificaPrec_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

            string cercaDuplicati2 = ConfigSettings.getKey(ConfigSettings.KeysENUM.CERCA_DUPLICATI_PROTOCOLLO_2);
            if (cercaDuplicati2 == null || !cercaDuplicati2.Equals("1"))
                cercaDuplicati2 = "0";

            if (!this.GetCalendarControl("txt_DataProtMit_P").txt_Data.Text.Equals(""))
            {
                ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).dataProtocolloMittente = this.GetCalendarControl("txt_DataProtMit_P").txt_Data.Text;
            }
            if (!this.txt_NumProtMit_P.Text.Equals(""))
            {
                ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).descrizioneProtocolloMittente = this.txt_NumProtMit_P.Text;
            }
            if (this.txt_DescMit_P.Text.Equals(""))
            {
                Page.RegisterStartupScript("", "<script>alert('Inserire il mittente del protocollo')</script>");
                return;
            }
            //if (this.txt_NumProtMit_P.Text.Equals("") && cercaDuplicati2.Equals("0"))
            //{
            //    Page.RegisterStartupScript("", "<script>alert('Inserire il numero del protocollo mittente')</script>");
            //    return;
            //}
            //if (this.GetCalendarControl("txt_DataProtMit_P").txt_Data.Text.Equals("") && cercaDuplicati2.Equals("0"))
            //{
            //    Page.RegisterStartupScript("", "<script>alert('Inserire la data del protocollo mittente')</script>");
            //    return;
            //}
            if (this.GetControlOggetto().oggetto_text.Equals("") && cercaDuplicati2.Equals("1"))
            {
                Page.RegisterStartupScript("", "<script>alert('Inserire un oggetto per il protocollo')</script>");
                return;
            }

            string scriptString = string.Empty;
            DocsPaWR.InfoProtocolloDuplicato[] datiProtDupl = null;

            DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum esitoRicercaDuplicati = DocumentManager.cercaDuplicati(this, schedaDocumento, cercaDuplicati2, out datiProtDupl);

            if (esitoRicercaDuplicati != DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum.NessunDuplicato)
            {
                if (datiProtDupl != null && datiProtDupl.Length > 0)
                {
                    string querySt = this.Server.UrlEncode("dtaProto=" + datiProtDupl[0].dataProtocollo
                                  + "&Segn=" + datiProtDupl[0].segnaturaProtocollo + "&Uff=" + datiProtDupl[0].uoProtocollatore.Replace("'", "|@ap@|")
                                  + "&IdDoc=" + datiProtDupl[0].idProfile + "&NumProto=" + datiProtDupl[0].numProto
                                  + "&DoProto=NO&result=" + esitoRicercaDuplicati.ToString() + "&modelloTrasm=" + this.ddl_tmpl.SelectedIndex + "&docAcquisito=" + datiProtDupl[0].docAcquisito);
                    string script = "<script>var retValue=window.showModalDialog('../popup/avvisoProtocolloEsistente.aspx?" + querySt
                                    + "','','dialogWidth:520px;dialogHeight:240px;fullscreen:no;toolbar:no;status:no;resizable:no;scroll:auto;"
                                    + "center:yes;help:no;close:no'); if (retValue=='protocolla')window.document.location = window.document.location.href + '&protocolla=1';</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "popupDuplicati", script);
                }
                else  // verifica senza precedenti
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "popUpNoPrecedenti", "<SCRIPT>alert('Dati di protocollazione non presenti.');</SCRIPT>");
                }

            }
            #region  oldCode
            //{
            //    scriptString += "Dati di protocollazione già presenti:";
            //    if(datiProtDupl!=null && datiProtDupl.Length>0)
            //    {												


            //        scriptString += "\\n\\nData: " + datiProtDupl[0].dataProtocollo;
            //        scriptString += "\\nSegnatura: " + datiProtDupl[0].segnaturaProtocollo;
            //        if(datiProtDupl[0].uoProtocollatore!=null && datiProtDupl[0].uoProtocollatore!=string.Empty)
            //            scriptString += "\\nUfficio: " + datiProtDupl[0].uoProtocollatore.Replace("'","\\'");					
            //    }
            //}
            //else
            //{
            //    scriptString += "Dati di protocollazione non presenti.";					
            //}


            //if(!this.Page.IsStartupScriptRegistered("alertJavaScript"))
            //{					
            //    string scString = "<SCRIPT>alert('" + scriptString.Replace("'","\\'") + "');</SCRIPT>";	


            //    this.Page.RegisterStartupScript("alertJavaScript", scString);
            //}
            #endregion
        }

        private string GetDataDuplicati(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento)
        {
            string retValue = string.Empty;

            return retValue;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_DataArrivo_P_TextChanged(object sender, System.EventArgs e)
        {
            DocsPaWR.Documento doc = new DocsPAWA.DocsPaWR.Documento();
            try
            {
                if (this.schedaDocumento.documenti == null || this.schedaDocumento.documenti.Length == 0)
                {
                    if (!string.IsNullOrEmpty(this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text))
                    {
                        if (!string.IsNullOrEmpty(this.txt_OraPervenuto_P.Text))
                            doc.dataArrivo = this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text + " " + this.txt_OraPervenuto_P.Text;
                        else
                            doc.dataArrivo = this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text;
                        DocsPaWR.Documento[] docs = new DocsPAWA.DocsPaWR.Documento[1];
                        docs[0] = doc;
                        this.schedaDocumento.documenti = docs;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text))
                    {
                        if (!string.IsNullOrEmpty(this.txt_OraPervenuto_P.Text))
                        {
                            ((DocsPAWA.DocsPaWR.Documento)this.schedaDocumento.documenti[0]).dataArrivo = this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text + " " + this.txt_OraPervenuto_P.Text;
                            doc.dataArrivo = this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text + " " + this.txt_OraPervenuto_P.Text;
                        }
                        else
                        {
                            ((DocsPAWA.DocsPaWR.Documento)this.schedaDocumento.documenti[0]).dataArrivo = this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text;
                            doc.dataArrivo = this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text;
                        }

                        this.schedaDocumento.dtaArrivoDaStoricizzare = "1";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }
        }

        private void txt_OraPervenuto_P_TextChanged(object sender, System.EventArgs e)
        {
            /*try
            {
                if (this.schedaDocumento.documenti == null || this.schedaDocumento.documenti.Length == 0)
                {
                    DocsPaWR.Documento doc = new DocsPAWA.DocsPaWR.Documento();
                    doc.dataArrivo =  this.txt_OraPervenuto_P.Text;
                    DocsPaWR.Documento[] docs = new DocsPAWA.DocsPaWR.Documento[1];
                    docs[0] = doc;
                    this.schedaDocumento.documenti = docs;
                }
                else
                {
                    ((DocsPAWA.DocsPaWR.Documento)this.schedaDocumento.documenti[0]).OraPervenuta = this.txt_OraPervenuto_P.Text;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }*/
            if (!string.IsNullOrEmpty(this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text))
            {
                this.txt_DataArrivo_P_TextChanged(sender, e);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_StampaBuste_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                DocsPaWR.FileDocumento fileRep = DocumentManager.getBustaReport(this, schedaDocumento);
                FileManager.setSelectedFileReport(this, fileRep, "../popup");
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_DataProtMit_P_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (this.rbl_InOut_P.Items[0].Selected)
                {
                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).dataProtocolloMittente = this.GetCalendarControl("txt_DataProtMit_P").txt_Data.Text;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }
        }

        #region stampa etichette

        /// <summary>
        /// utilizzato da ActiveX di stampa. estrae la descrizione dell' Amministrazione Attuale.
        /// </summary>
        /// <param name="IdAmministrazione"></param>
        /// <returns>restituisce la descrizione dell'amministrazione passata con il parametro di input IdAmministrazione</returns>
        /// add by Massimo Digregorio
        private string getDescAmministrazione(string IdAmministrazione)
        {
            string descAmm = string.Empty;
            string returnMsg = string.Empty;

            DocsPaWR.Amministrazione[] amministrazioni = UserManager.getListaAmministrazioni(this, out returnMsg);

            if (amministrazioni.Length == 1)
            {
                descAmm = amministrazioni[0].descrizione;
            }
            else
            {
                bool found = false;
                int i = 0;

                while ((!found) && (i < amministrazioni.Length))
                {
                    if (amministrazioni[i].systemId == IdAmministrazione)
                    {
                        found = true;
                        descAmm = amministrazioni[i].descrizione;
                    }

                    i++;
                }
            }

            return descAmm;
        }

        /// <summary>
        /// utilizzato da ActiveX di stampa. Estrae il codice classifica a cui la scheda documento è associata.
        /// </summary>
        /// <returns>codice classifica</returns>
        /// add by Massimo Digregorio
        private string getClassificaPrimaria()
        {
            //RECUPERARE VALORE PER infoDocumento
            DocsPaWR.InfoDocumento infoDocumento = DocumentManager.getInfoDocumento(DocumentManager.getDocumentoInLavorazione(this));
            string codClassifica = "";
            codClassifica = DocumentManager.GetClassificaDoc(this, infoDocumento.idProfile);
            return codClassifica;
        }

        /// <summary>
        /// utilizzato da ActiveX di stampa. Estrae il codice fascicolo a cui il documento è associato
        /// </summary>
        /// <returns>codice Fascicolo</returns>
        /// add by Massimo Digregorio
        private string getCodiceFascicolo()
        {
            //RECUPERARE VALORE PER infoDocumento
            DocsPaWR.InfoDocumento infoDocumento = DocumentManager.getInfoDocumento(DocumentManager.getDocumentoInLavorazione(this));
            string codFascicolo = "";
            codFascicolo = DocumentManager.getFascicoloDoc(this, infoDocumento);

            return codFascicolo;
        }

        /// <summary>
        /// /// utilizzato da ActiveX di stampa. Estrae tutti i parametri possibili stampabili nell'etichetta.
        /// </summary>
        /// add by Massimo Digregorio
        private void caricaCampiEtichetta()
        {
            #region parametro Dispositivo Di Stampa
            //versione precedente
            //if (ConfigSettings.getKey(ConfigSettings.KeysENUM.DISPOSITIVO_STAMPA) != null)
            //{
            //    this.hd_dispositivo.Value = ConfigSettings.getKey(ConfigSettings.KeysENUM.DISPOSITIVO_STAMPA);
            //}
            //else
            //{
            //    this.hd_dispositivo.Value = "Penna";
            //}
            //this.hd_modello_dispositivo.Value = ConfigSettings.getKey(ConfigSettings.KeysENUM.MODELLO_DISPOSITIVO_STAMPA);
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            var dispositivoStampaUtente = ws.AmmGetDispositivoStampaUtente(UserManager.getInfoUtente().idPeople);
            if (dispositivoStampaUtente != null)
            {
                this.hd_dispositivo.Value = "Etichette";
                this.hd_modello_dispositivo.Value = dispositivoStampaUtente.ToString();
            }
            else
                this.hd_dispositivo.Value = "Penna";


            #endregion parametro Dispositivo Di Stampa

            #region parametro Descrizione Amministrazione
            string descAmm = getDescAmministrazione(UserManager.getUtente(this).idAmministrazione);
            #endregion parametro Descrizione Amministrazione

            #region parametro Classifica Primaria
            string classificaPrimaria = String.Empty;

            string classificazioneInEtichetta = System.Configuration.ConfigurationManager.AppSettings["StampaClassificazioneInEtichetta"];
            if (classificazioneInEtichetta != null)
            {
                switch (classificazioneInEtichetta)
                {
                    case "1": // stampa il codice classifica In Etichetta
                        classificaPrimaria = getClassificaPrimaria();
                        break;
                    default:
                        //massimo digregorio, non necessario se l'assegnazione avviene in dichiarazione. old: classificaPrimaria = String.Empty;
                        break;
                }
            }
            this.hd_classifica.Value = classificaPrimaria;
            #endregion parametro Classifica Primaria

            #region parametro Fascicolo primario
            string fascicoloInEtichetta = System.Configuration.ConfigurationManager.AppSettings["StampaFascicoloInEtichetta"];
            if (fascicoloInEtichetta != null)
            {
                switch (fascicoloInEtichetta)
                {
                    case "1": // stampa il codice fascicolo In Etichetta
                        this.hd_fascicolo.Value = getCodiceFascicolo();
                        break;
                    default:
                        this.hd_fascicolo.Value = String.Empty;
                        break;
                }
            }
            #endregion parametro Fascicolo primario

            #region patch per cuneo
            string descAmministrInEtichetta = System.Configuration.ConfigurationManager.AppSettings["StampaDescrizioneAmministrazioneInEtichetta"];
            if (descAmministrInEtichetta != null)
            {
                switch (descAmministrInEtichetta)
                {
                    case "1": // Stampa Descrizione Amministrazione In Etichetta
                        this.hd_amministrazioneEtichetta.Value = descAmm;
                        break;
                    default:
                        this.hd_amministrazioneEtichetta.Value = String.Empty;
                        break;
                }
            }

            //aggiuto tag Hidden "hd_desAmministrazione" per ActiveX di stampa
            /* se parametro esiste ed a 0, a hd_desAmministrazione viene assegnata la classifica
                     * se parametro non esiste o esiste <> 0, a hd_desAmministrazione viene assegnata la descrizione dell'amministrazione
                     */
            bool BarCodeConAmministrazione = true;
            DocsPaWR.Configurazione visualizzaClassificaSopraBarCode = UserManager.getParametroConfigurazione(this.Page);

            if (visualizzaClassificaSopraBarCode != null)
            {
                if (visualizzaClassificaSopraBarCode.valore.Equals("0")) BarCodeConAmministrazione = false;
            }

            if (BarCodeConAmministrazione)
            {
                this.hd_descrizioneAmministrazione.Value = descAmm;
            }
            else
            {
                this.hd_descrizioneAmministrazione.Value = classificaPrimaria;
            }

            #endregion patch per cuneo

            #region parametro URL File di configurazione Dispositivo di Stampa
            this.hd_UrlIniFileDispositivo.Value = ConfigSettings.getKey(ConfigSettings.KeysENUM.URL_INIFILE_DISPOSITIVO_STAMPA);
            #endregion parametro URL File di configurazione Dispositivo di Stampa

            #region stampa multipla etichetta
            string abilita_multi_stampa_etichetta = utils.InitConfigurationKeys.GetValue("0", "FE_MULTI_STAMPA_ETICHETTA");
            if (abilita_multi_stampa_etichetta.Equals("1"))
            {
                // recupero il numero di stampe del documento da effettuare
                if (this.txt_num_stampe.Text != null)
                    this.hd_num_stampe.Value = this.txt_num_stampe.Text;
                else
                    this.hd_num_stampe.Value = "1";
                // recupero il valore di stampa corrente da inserire nella  successiva etichetta da stampare
                int num_stampe_eff;
                if (!String.IsNullOrEmpty(this.schedaDocumento.protocollo.stampeEffettuate))
                {
                    num_stampe_eff = Convert.ToInt32(this.schedaDocumento.protocollo.stampeEffettuate) + 1;
                    this.hd_num_stampe_effettuate.Value = num_stampe_eff.ToString();
                }
                else
                    this.hd_num_stampe_effettuate.Value = "1";
            }
            else
            {
                this.hd_num_stampe_effettuate.Value = "1";
                this.hd_num_stampe.Value = "1";
            }

            #endregion

            #region parametri scheda Documento
            this.hd_num_doc.Value = schedaDocumento.docNumber;
            this.hd_dataCreazione.Value = this.schedaDocumento.dataCreazione;
            this.hd_codiceUoCreatore.Value = schedaDocumento.creatoreDocumento.uo_codiceCorrGlobali;
            if (!string.IsNullOrEmpty(this.schedaDocumento.oraCreazione))
            {
                this.hd_ora_creazione.Value = Utils.timeLength(this.schedaDocumento.oraCreazione);
                //correzione sabrina - se l'ora di creazione non c'è o è '00:00:00' il metodo timeLength restituisce stringa vuota e poi il Substring dà errore
                if (!string.IsNullOrEmpty(this.hd_ora_creazione.Value))
                    this.hd_ora_creazione.Value = this.hd_ora_creazione.Value.Substring(0, 5);
            }

            this.hd_tipo_proto.Value = getEtichetta(schedaDocumento.tipoProto);

            this.hd_coduo_proto.Value = String.Empty;//è gestito sul db e sull'oggetto ruolo utente attuale, ma non nell'oggetto schedaDocumento;

            if (schedaDocumento.registro != null)
            {
                this.hd_codreg_proto.Value = schedaDocumento.registro.codRegistro;
                this.hd_descreg_proto.Value = schedaDocumento.registro.descrizione;
            }

            if (schedaDocumento.protocollo != null && schedaDocumento.protocollo.numero != null)
            {
                //Celeste
                //this.hd_num_proto.Value = schedaDocumento.protocollo.numero;
                this.hd_num_proto.Value = Utils.formatProtocollo(schedaDocumento.protocollo.numero);
                //Fine Celeste
                this.hd_anno_proto.Value = schedaDocumento.protocollo.anno;
                this.hd_data_proto.Value = schedaDocumento.protocollo.dataProtocollazione;

                //massimo digregorio new:
                if (schedaDocumento.protocollatore != null)
                    this.hd_coduo_proto.Value = schedaDocumento.protocollatore.uo_codiceCorrGlobali;
            }
            #endregion parametri scheda Documento

            #region Parametri allegati

            // Impostazione del numero degli allegati
            this.hd_numero_allegati.Value = schedaDocumento.allegati.Length.ToString();

            #endregion

            #region parametro data arrivo
            if (schedaDocumento != null && schedaDocumento.documenti != null)
            {
                int firstDoc = (schedaDocumento.documenti.Length > 0) ? schedaDocumento.documenti.Length - 1 : 0;
                if (string.IsNullOrEmpty(schedaDocumento.documenti[firstDoc].dataArrivo))
                {
                    this.hd_dataArrivo.Value = string.Empty;
                    this.hd_dataArrivoEstesa.Value = string.Empty;
                }
                else
                {
                    DateTime dataArrivo;
                    DateTime.TryParse(schedaDocumento.documenti[firstDoc].dataArrivo, out dataArrivo);
                    this.hd_dataArrivo.Value = dataArrivo.ToString("d");
                    this.hd_dataArrivoEstesa.Value = schedaDocumento.documenti[firstDoc].dataArrivo;
                }
            }
            else
            {
                this.hd_dataArrivo.Value = string.Empty;
                this.hd_dataArrivoEstesa.Value = string.Empty;
            }
            #endregion
        }


        private void btn_stampaSegn_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                string[] campiVariabili = new string[] { "1", "2" };
                string l_segnaturaCampiVariabili = DocumentManager.getSegnaturaCampiVariabili(this, schedaDocumento, campiVariabili);
                schedaDocumento.protocollo.segnatura = l_segnaturaCampiVariabili;

                caricaCampiEtichetta(); //add massimo digregorio
            }
            catch (System.Exception ex) //modificato massimo digregorio
            {
                System.Diagnostics.Debug.WriteLine(ex.Message); //add massimo digregorio
                //funzione non implementata
            }

            srcStampaSegnatura = "./stampaSegnatura.htm";
        }

        #endregion stampa etichette

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkEvidenza_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.chkEvidenza.Checked)
                schedaDocumento.evidenza = "1";
            else
                schedaDocumento.evidenza = "0";
        }

        /// <summary>
        /// Metodo per la modifica della data, non più Usato
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_mod_Dta_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //this.txt_DataArrivo_P.ReadOnly = false;
            this.GetCalendarControl("txt_DataArrivo_P").txt_Data.ReadOnly = false;
            this.m_modifica = true;
            this.btn_salva_P.Enabled = true;
        }

        /// <summary>
        /// Metodo per il recupero del fascicolo da codice
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo getFascicolo()
        {
            DocsPaWR.Fascicolo fascicoloSelezionato = null;
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
        }

        /// <summary>
        /// Metodo per il recupero del sottofascicolo da codice fascicolo e descrizione sottofascicolo
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo getFolder(DocsPAWA.DocsPaWR.Registro registro, ref string codice, ref string descrizione)
        {
            DocsPaWR.Folder[] listaFolder = null;
            DocsPaWR.Fascicolo fasc = null;
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
            DocsPaWR.Fascicolo[] listaFasc = null;
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
            //

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
                DocsPaWR.Fascicolo SottoFascicolo = getFolder(UserManager.getRegistroSelezionato(this), ref codice, ref descrizione);
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
                DocsPaWR.Fascicolo[] listaFasc = getFascicoli(UserManager.getRegistroSelezionato(this));

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
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
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
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
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
                            //Provo il caso in cui il fascicolo è chiuso
                            Fascicolo chiusoFasc = FascicoliManager.getFascicoloDaCodice(this.Page, this.txt_CodFascicolo.Text);
                            if (chiusoFasc != null && !string.IsNullOrEmpty(chiusoFasc.stato) && chiusoFasc.stato.Equals("C"))
                            {
                                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "fasc_chiuso", "alert('Attenzione, il fascicolo scelto è chiuso. Pertanto il documento non può essere inserito nel fascicolo selezionato');", true);
                            }
                            else
                            {
                                Page.RegisterStartupScript("", "<script>alert('Attenzione, codice fascicolo non presente')</script>");
                            }
                            Session["validCodeFasc"] = "false";
                            this.txt_DescFascicolo.Text = "";
                            this.txt_CodFascicolo.Text = "";
                        }
                    }
                }
            }

            //Commentato perchè adesso si deve tener conto della presenza di piu' titolari
            // e quindi di piu' fascicoli con lo stesso codice
            //Adesso la gestione è stata fatta come per la doc profila con la differenza che in questo caso
            //il registro è selezionato
            #region Codice commentato
            /*
            DocsPaWR.Fascicolo fasc = getFascicolo();
            if (fasc != null)
            {
                txt_DescFascicolo.Text = fasc.descrizione;
                if (fasc.tipo.Equals("G"))
                {
                    codClassifica = fasc.codice;
                }
                else
                {
                    //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                    DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, fasc.idClassificazione, UserManager.getUtente(this).idAmministrazione);
                    string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                    codClassifica = codiceGerarchia;
                }
                FascicoliManager.setFascicoloSelezionatoFascRapida(this, fasc);
            }
            else
            {
                Session["validCodeFasc"] = "false";
                Page.RegisterStartupScript("", "<script>alert('Codice fascicolo non presente')</script>");
                this.txt_DescFascicolo.Text = "";
                this.txt_CodFascicolo.Text = "";
            }
            */
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        private void caricaComboTemplate()
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
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.TemplateTrasmissione getTemplate()
        {
            if (!(ddl_tmpl.SelectedIndex > 0))
                return null;
            //crea trasmissione da template
            DocsPaWR.TemplateTrasmissione[] listaTmp;
            DocsPaWR.TemplateTrasmissione template = null;
            listaTmp = (DocsPAWA.DocsPaWR.TemplateTrasmissione[])(Session["doc_protocollo.tx_tmpl"]);

            if (listaTmp != null && listaTmp.Length > 0)
                template = (DocsPAWA.DocsPaWR.TemplateTrasmissione)listaTmp[ddl_tmpl.SelectedIndex - 1];
            return template;
        }

        #region tipologia atto
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddl_tipoAtto_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Session["rubrica.campoCorrispondente"] != null)
                Session.Remove("rubrica.campoCorrispondente");
            if (Session["CorrSelezionatoDaMulti"] != null)
                Session.Remove("CorrSelezionatoDaMulti");
            if (Session["dictionaryCorrispondente"] != null)
                Session.Remove("dictionaryCorrispondente");

            DocsPaWR.TipologiaAtto tipologiaAtto = new DocsPAWA.DocsPaWR.TipologiaAtto();
            tipologiaAtto.systemId = this.ddl_tipoAtto.SelectedItem.Value;
            tipologiaAtto.descrizione = this.ddl_tipoAtto.SelectedItem.Text;

            //Spostato nella pagina anteprimaProfDinamica
            //DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
            //string count = ws.CountTipoFromTipologia("CORRISPONDENTE", tipologiaAtto.descrizione);

            //Session.Add("CountCorr", count);

            schedaDocumento.daAggiornareTipoAtto = true;
            schedaDocumento.tipologiaAtto = tipologiaAtto;
            if (schedaDocumento.systemId != null && !schedaDocumento.systemId.Equals(""))
                this.btn_salva_P.Enabled = true;

            //if (ddl_tipoAtto.SelectedIndex == 0)
            //    Session.Remove("template");

            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
            {
                //DocsPaWR.Templates template = wws.getTemplate((UserManager.getInfoUtente(this)).idAmministrazione, ddl_tipoAtto.SelectedItem.Text, schedaDocumento.docNumber);
                //if (template != null)
                //{

                string idTemplate = ddl_tipoAtto.SelectedValue;
                if (idTemplate != "")
                {
                    DocsPaWR.Templates template = ProfilazioneDocManager.getTemplateById(idTemplate, this);
                    if (template != null)
                    {
                        Session.Remove("template");
                        schedaDocumento.template = template;

                        Session.Add("refreshDxPageProf", true);

                        //string pulsanti = btn_salva_P.Enabled + "-" + btn_protocolla_P.Enabled + "-" + btn_protocollaGiallo_P.Enabled;
                        //ClientScript.RegisterStartupScript(this.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1&pulsanti=" + pulsanti + "'", true);
                        //ClientScript.RegisterStartupScript(this.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);
                        //Se la tipologia documento è privato allora il documento deve essere privato
                        if (template.PRIVATO != null && template.PRIVATO == "1")
                        {
                            chkPrivato.Checked = true;
                            //chkPrivato.Enabled = false;
                            schedaDocumento.privato = "1";
                        }
                        //else
                        //    {
                        //        chkPrivato.Checked = false;
                        //        //chkPrivato.Enabled = true;
                        //        schedaDocumento.privato = "0";
                        //    }
                    }
                    else
                    {
                        //ClientScript.RegisterStartupScript(this.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=0';", true);
                        chkPrivato.Checked = false;
                        //chkPrivato.Enabled = true;
                        schedaDocumento.privato = "0";
                    }
                }
            }

            if (ddl_tipoAtto.SelectedIndex == 0)
            {
                Session.Remove("template");
                //ClientScript.RegisterStartupScript(this.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=0';", true);
            }
            Session.Remove("CorrSelezionatoDaMulti");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_addTipoAtto_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string javascript = null;
            javascript = "win=window.open('../popup/insTipoAtto.aspx?wnd=docProtocollo','TipoAtto','width=420,height=150,toolbar=no,directories=no,menubar=no,resizable=yes,scrollbars=no');";
            javascript += "win.focus();";
            Response.Write("<script>" + javascript + "</script>");
        }

        private void CaricaComboMezzoSpedizione(DropDownList ddl)
        {
            ddl.Items.Clear();
            //aggiunge una riga vuota alla combo
            ddl.Items.Add("");
            ArrayList listaMezzoSpedizione = new ArrayList();
            DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
            string idAmm = UserManager.getInfoUtente().idAmministrazione;
            DocsPAWA.DocsPaWR.MezzoSpedizione[] m_sped = ws.AmmListaMezzoSpedizione(idAmm, false);
            foreach (DocsPAWA.DocsPaWR.MezzoSpedizione m_spediz in m_sped)
            {
                ListItem li = new ListItem();
                li.Value = m_spediz.IDSystem;
                li.Text = m_spediz.Descrizione;
                ddl.Items.Add(li);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ddl"></param>
        private void CaricaComboTipologiaAtto(DropDownList ddl)
        {
            logger.Info("BEGIN");
            DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
                listaTipologiaAtto = DocumentManager.getTipoAttoPDInsRic(this, UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "2");
            else
                listaTipologiaAtto = DocumentManager.getListaTipologiaAtto(this);

            ddl.Items.Clear();
            ddl.Items.Add("");
            if (listaTipologiaAtto != null)
            {
                for (int i = 0; i < listaTipologiaAtto.Length; i++)
                {
                    ddl.Items.Add(listaTipologiaAtto[i].descrizione);
                    ddl.Items[i + 1].Value = listaTipologiaAtto[i].systemId;
                }
            }
            logger.Info("END");
        }

        private void CaricaComboRispostaProtocollo(DropDownList ddl, DocsPAWA.DocsPaWR.InfoDocumento rispProto)
        {

            ddl.Items.Clear();
            //aggiunge una riga vuota alla combo
            ddl.Items.Add("");
            ddl.Items.Add(rispProto.segnatura);
            //ddl.Items[1].Value=rispProto.docNumber;
            ddl.SelectedIndex = 1;
            ddl.Enabled = false;
        }
        #endregion

        /// <summary>
        /// Torna alla visualizzazione documenti del folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_BackToFolder_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Write(FascicoliManager.FolderViewReloadScript());
        }

        private void btn_annulla_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                Session.Add("refreshDxPageProf", true);
                Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location=top.principale.iFrame_dx.document.location;</script>");
            }
        }

        private void btn_RubrOgget_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        private void btn_storiaOgg_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        private void btn_log_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string scriptString = "<SCRIPT>ApriFinestraLog('D');</SCRIPT>";
            this.RegisterStartupScript("apriModalDialogLog", scriptString);
        }


        private void btn_BackToQuery_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        private void btn_dettDest_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        private void btn_adl_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Salvo i campi non obbligatori che possono essere stati modificati
            DocsPaWR.SchedaDocumento SchedaDoc = DocumentManager.getDocumentoInLavorazione(this);
            //SchedaDoc.oggetto.descrizione = this.txt_oggetto_P.Text;
            SchedaDoc.oggetto.descrizione = this.ctrl_oggetto.oggetto_text;

            // protocollo in ingresso
            if (this.rbl_InOut_P.Items[0].Selected)
            {
                DocsPaWR.ProtocolloEntrata protIng = (DocsPAWA.DocsPaWR.ProtocolloEntrata)SchedaDoc.protocollo;
                //protIng.dataProtocolloMittente = this.txt_DataArrivo_P.Text;
                protIng.dataProtocolloMittente = this.GetCalendarControl("txt_DataArrivo_P").txt_Data.Text;
            }

            DocumentManager.setDocumentoInLavorazione(this, SchedaDoc);
        }


        #region CATENA USCITA
        //private void btn_Risp_ingresso_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        //{
        //   if (this.lbl_segnatura.Text != "" && this.lbl_segnatura != null)
        //   {
        //      string scriptString = "<SCRIPT>ApriFinestraScegliDestinatari();</SCRIPT>";
        //      this.RegisterStartupScript("apriModalDialogProtocolli", scriptString);
        //   }
        //}

        //private void btn_in_rispostaIngresso_a_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        //{

        //     string scriptString = "<SCRIPT>ApriFinestraProtocolliInIngresso();</SCRIPT>";
        //   this.RegisterStartupScript("apriModalDialogProtocolli", scriptString);



        //}

        //private void btn_risp_ingresso_sx_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        //{
        //   //Riporta al dettaglio del protocollo in ingresso o uscita  a seconda del caso
        //   try
        //   {
        //      if (this.txt_RispIngressoProtSegn_P != null && this.txt_RispIngressoProtSegn_P.Text != "")
        //      {
        //         string docNum = (DocumentManager.getDocumentoInLavorazione(this)).rispostaDocumento.docNumber.ToString();
        //         string tipoProto = (DocumentManager.getDocumentoInLavorazione(this)).rispostaDocumento.tipoProto.ToString();

        //         DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
        //         DocsPaWR.InfoDocumento infoDocumento = DocumentManager.GetCatenaDocumentoMittente(this, infoUtente.idGruppo, infoUtente.idPeople, docNum, tipoProto);

        //         if (infoDocumento == null)
        //         {
        //            Response.Write("<script>window.alert('Non si posseggono i diritti necessari alla visualizzazione del documento richiesto.')</script>");
        //         }
        //         else
        //         {
        //            DocumentManager.setRisultatoRicerca(this, infoDocumento);
        //            Response.Write("<script language='javascript'> top.principale.document.location = '../documento/gestionedoc.aspx?tab=protocollo';</script>");
        //         }
        //      }
        //   }
        //   catch (System.Exception ex)
        //   {
        //      ErrorManager.redirect(this, ex, "protocollazione");
        //   }
        //}

        #endregion


        private void btn_Risp_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //Nel caso di predisposto alla protocollazione 20/08/2010 Fabio altrimenti non crea documento in risposta
            // if (!string.IsNullOrEmpty(this.lbl_segnatura.Text) ||  (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null
            //        && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1"))
            if (!string.IsNullOrEmpty(this.schedaDocumento.docNumber) || (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null
                    && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1"))
            {
                Session["catenaDoc"] = true;
                #region CATENE USCITA
                if (this.rbl_InOut_P.SelectedItem.Value == "Out")
                {
                    string scriptString = "<SCRIPT>ApriFinestraScegliDestinatari();</SCRIPT>";
                    this.RegisterStartupScript("apriModalDialogProtocolli", scriptString);
                }
                #endregion
                #region CATENE INGRESSO
                if (this.rbl_InOut_P.SelectedItem.Value == "In")
                {

                    //il tasto back non deve essere visibile come quando si ripropone un
                    //documento
                    DocumentManager.removeMemoriaFiltriRicDoc(this);
                    DocumentManager.RemoveMemoriaVisualizzaBack(this);
                    //FascicoliManager.removeFascicoloSelezionatoFascRapida(this);

                    // schedaDocumento = DocumentManager.riproponiDatiRisp(this, schedaDocumento);


                    //Data una scheda documento (quella del protocollo in ingresso) ne viene creata una nuova 
                    //(per il protocollo in uscita) riproponendo l'oggetto e il mittente come destinatario del protocollo in uscita 
                    SchedaDocumento schedaNewDoc = DocumentManager.NewSchedaDocumento(this);
                    if (schedaDocumento.oggetto != null)
                    {
                        schedaNewDoc.oggetto = schedaDocumento.oggetto;
                    }
                    schedaNewDoc.idPeople = schedaDocumento.idPeople;
                    schedaNewDoc.userId = schedaDocumento.userId;
                    schedaNewDoc.typeId = schedaDocumento.typeId;
                    schedaNewDoc.appId = schedaDocumento.appId;
                    schedaNewDoc.privato = "0";
                    //pezza EM

                    FileManager.removeSelectedFile(this);
                    // schedaNewDoc = DocumentManager.creaDocumentoGrigio(this, schedaNewDoc);

                    schedaNewDoc.registro = schedaDocumento.registro;
                    schedaNewDoc.tipoProto = "P";

                    if (schedaDocumento.tipoProto.Equals("A"))
                    {
                        schedaNewDoc.protocollo = new DocsPAWA.DocsPaWR.ProtocolloUscita();
                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaNewDoc.protocollo).destinatari = new DocsPAWA.DocsPaWR.Corrispondente[1];
                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaNewDoc.protocollo).destinatari[0] = new DocsPAWA.DocsPaWR.Corrispondente();
                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaNewDoc.protocollo).destinatari[0] = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente;
                    }

                    bool enableUffRef = false;
                    string message = "";
                    bool daAggiornareUffRef = false;
                    if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                    {
                        enableUffRef = true;
                    }

                    //schedaNewDoc = DocumentManager.salva(this, schedaNewDoc, enableUffRef, out daAggiornareUffRef);

                    schedaNewDoc.predisponiProtocollazione = true;
                    //Viene popolato l'oggetto risposta al protocollo:
                    if (schedaDocumento.protocollo != null &&
                        ((schedaDocumento.protocollo.numero != null && !schedaDocumento.protocollo.numero.Equals("")) ||
                            (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1"))
                        )
                    {
                        DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaDocumento);
                        schedaNewDoc.rispostaDocumento = infoDoc;
                    }

                    if (!string.IsNullOrEmpty(schedaNewDoc.systemId))
                    {

                        //Inizio sviluppo estensione delle funzionalità di fascicolazione e trasmissione rapida
                        DocsPaWR.Fascicolo fasc;
                        string segnatura = "";
                        string returnMsg = "";
                        fasc = getFascicolo();
                        if (fasc != null)
                        {
                            if (fasc.stato == "C")
                            {
                                returnMsg += "Il fascicolo scelto è chiuso. Pertanto il documento non è stato fascicolato";
                            }
                            else
                            {
                                if (schedaNewDoc.protocollo != null)
                                    segnatura = schedaNewDoc.protocollo.segnatura;

                                string msg = string.Empty;
                                if (!DocumentManager.fascicolaRapida(this, schedaNewDoc.systemId, schedaNewDoc.docNumber, segnatura, fasc, out msg))
                                {
                                    if (string.IsNullOrEmpty(msg))
                                        returnMsg += " Il documento non è stato fascicolato";
                                    else
                                        returnMsg += " " + msg;
                                }
                                else
                                {
                                    schedaNewDoc.fascicolato = "1";
                                }
                            }
                            if (!returnMsg.Equals(""))
                                Response.Write("<script>alert('" + returnMsg + "');</script>");
                            FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                            FascicoliManager.removeCodiceFascRapida(this);
                            FascicoliManager.removeDescrizioneFascRapida(this);
                        }
                        if (ddl_tmpl.SelectedIndex > 0)
                        {
                            execTrasmRapida();
                            ddl_tmpl.SelectedIndex = 0;
                        }
                        //Fine sviluppo estensione delle funzionalità di fascicolazione e trasmissione rapida    
                    }
                    DocumentManager.setDocumentoSelezionato(this, schedaNewDoc);
                    DocumentManager.setDocumentoInLavorazione(this, schedaNewDoc);
                    this.rbl_InOut_P.Enabled = true;

                    Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=protocollo';</script>");
                }
                #endregion
                #region CATENE INTERNO
                if (this.rbl_InOut_P.SelectedItem.Value == "Own" && (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1"))
                {

                    //il tasto back non deve essere visibile come quando si ripropone un
                    //documento
                    DocumentManager.removeMemoriaFiltriRicDoc(this);
                    DocumentManager.RemoveMemoriaVisualizzaBack(this);
                    //Data una scheda documento (quella del protocollo interno) ne viene creata una nuova 
                    //(sempre interno) riproponendo l'oggetto e il mittente e il destinatario del protocollo interno 
                    SchedaDocumento schedaNewDoc = DocumentManager.NewSchedaDocumento(this);
                    if (schedaDocumento.oggetto != null)
                    {
                        schedaNewDoc.oggetto = schedaDocumento.oggetto;
                    }
                    schedaNewDoc.idPeople = schedaDocumento.idPeople;
                    schedaNewDoc.userId = schedaDocumento.userId;
                    schedaNewDoc.typeId = schedaDocumento.typeId;
                    schedaNewDoc.appId = schedaDocumento.appId;
                    schedaNewDoc.privato = "0";

                    FileManager.removeSelectedFile(this);

                    schedaNewDoc.registro = schedaDocumento.registro;
                    schedaNewDoc.tipoProto = "I";

                    //PRENDO IL MIO RUOLO COME MITTENTE
                    Corrispondente corrispondenteIoMitt = null;

                    if (UserManager.getRuolo(this) != null)
                    {
                        DocsPaWR.Ruolo ruo = UserManager.getRuolo(this);
                        DocsPaWR.Corrispondente corr = ruo.uo;
                        if (corr != null)
                        {
                            corrispondenteIoMitt = corr;
                        }
                    }

                    schedaNewDoc.protocollo = new DocsPAWA.DocsPaWR.ProtocolloInterno();

                    //PRENDO IL MITTENTE DEL PROTOCOLLO INTERNO DI PARTENZA
                    Corrispondente corrispondenteMitt = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente;

                    ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).destinatari = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatari;

                    if (!corrispondenteMitt.systemId.Equals(corrispondenteIoMitt.systemId))
                    {

                        if (!UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatariConoscenza, corrispondenteMitt))
                        {
                            ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).destinatari = UserManager.addCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).destinatari, corrispondenteMitt);
                        }

                    }

                    int cancellaDest = -1;

                    if (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).destinatari != null)
                    {
                        for (int i = 0; i < (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).destinatari).Length; i++)
                        {
                            Corrispondente temp = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).destinatari[i];
                            if (temp.systemId.Equals(corrispondenteIoMitt.systemId))
                            {
                                cancellaDest = i;
                                break;
                            }
                        }
                    }

                    if (cancellaDest != -1)
                    {
                        //SE IL MITTENTE è presente nei destinatari lo elimino
                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).destinatari = UserManager.removeCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).destinatari, cancellaDest);
                    }

                    if (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatariConoscenza != null && ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatariConoscenza.Length > 0)
                    {
                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).destinatariConoscenza = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).destinatariConoscenza;

                        cancellaDest = -1;

                        for (int i = 0; i < (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).destinatariConoscenza).Length; i++)
                        {
                            Corrispondente temp = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).destinatariConoscenza[i];
                            if (temp.systemId.Equals(corrispondenteIoMitt.systemId))
                            {
                                cancellaDest = i;
                                break;
                            }
                        }

                        if (cancellaDest != -1)
                        {
                            //SE IL MITTENTE è presente nei destinatari lo elimino
                            ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).destinatariConoscenza = UserManager.removeCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).destinatariConoscenza, cancellaDest);
                        }
                    }

                    ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).mittente = corrispondenteIoMitt;


                    bool enableUffRef = false;
                    string message = "";
                    bool daAggiornareUffRef = false;
                    if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                    {
                        enableUffRef = true;
                    }

                    schedaNewDoc.predisponiProtocollazione = true;
                    //Viene popolato l'oggetto risposta al protocollo:
                    if (schedaDocumento.protocollo != null &&
                        ((schedaDocumento.protocollo.numero != null && !schedaDocumento.protocollo.numero.Equals(""))
                          || (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1")))
                    {
                        DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaDocumento);
                        schedaNewDoc.rispostaDocumento = infoDoc;
                    }

                    //Inizio sviluppo estensione delle funzionalità di fascicolazione e trasmissione rapida
                    DocsPaWR.Fascicolo fasc;
                    string segnatura = "";
                    string returnMsg = "";
                    fasc = getFascicolo();
                    if (fasc != null)
                    {
                        if (fasc.stato == "C")
                        {
                            returnMsg += "Il fascicolo scelto è chiuso. Pertanto il documento non è stato fascicolato";
                        }
                        else
                        {
                            if (schedaNewDoc.protocollo != null)
                                segnatura = schedaNewDoc.protocollo.segnatura;

                            string msg = string.Empty;
                            if (!DocumentManager.fascicolaRapida(this, schedaNewDoc.systemId, schedaNewDoc.docNumber, segnatura, fasc, out msg))
                            {
                                if (string.IsNullOrEmpty(msg))
                                    returnMsg += " Il documento non è stato fascicolato";
                                else
                                    returnMsg += " " + msg;
                            }
                            else
                            {
                                schedaNewDoc.fascicolato = "1";
                            }
                        }
                        if (!returnMsg.Equals(""))
                            Response.Write("<script>alert('" + returnMsg + "');</script>");
                        FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                        FascicoliManager.removeCodiceFascRapida(this);
                        FascicoliManager.removeDescrizioneFascRapida(this);
                    }
                    if (ddl_tmpl.SelectedIndex > 0)
                    {
                        execTrasmRapida();
                        ddl_tmpl.SelectedIndex = 0;
                    }
                    //Fine sviluppo estensione delle funzionalità di fascicolazione e trasmissione rapida    

                    DocumentManager.setDocumentoSelezionato(this, schedaNewDoc);
                    DocumentManager.setDocumentoInLavorazione(this, schedaNewDoc);
                    this.rbl_InOut_P.Enabled = true;

                    Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=protocollo';</script>");
                }
                #endregion

            }
        }

        private void btn_risp_grigio_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.schedaDocumento.docNumber) || (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null
                    && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1"))
            {
                Session["catenaDoc"] = true;
                DocumentManager.removeMemoriaFiltriRicDoc(this);
                DocumentManager.RemoveMemoriaVisualizzaBack(this);

                SchedaDocumento schedaNewDoc = DocumentManager.NewSchedaDocumento(this);
                if (schedaDocumento.oggetto != null)
                {
                    schedaNewDoc.oggetto = schedaDocumento.oggetto;
                }
                schedaNewDoc.idPeople = schedaDocumento.idPeople;
                schedaNewDoc.userId = schedaDocumento.userId;
                schedaNewDoc.typeId = schedaDocumento.typeId;
                schedaNewDoc.appId = schedaDocumento.appId;
                schedaNewDoc.privato = "0";

                FileManager.removeSelectedFile(this);

                schedaNewDoc.registro = null;
                schedaNewDoc.tipoProto = "G";
                schedaNewDoc.protocollo = null;

                bool enableUffRef = false;
                string message = "";
                bool daAggiornareUffRef = false;
                if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                {
                    enableUffRef = true;
                }

                schedaNewDoc.predisponiProtocollazione = false;
                //Viene popolato l'oggetto risposta al protocollo:
                DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaDocumento);
                schedaNewDoc.rispostaDocumento = infoDoc;
                
                //ABBATANGELI GIANLUIGI - risoluzione seconda segnalazione aggiunta al bug 1132
                if (schedaNewDoc.repositoryContext != null)
                {
                    schedaNewDoc.repositoryContext.IsDocumentoGrigio = true;
                }

                DocumentManager.setDocumentoSelezionato(this, schedaNewDoc);
                DocumentManager.setDocumentoInLavorazione(this, schedaNewDoc);
                this.rbl_InOut_P.Enabled = true;

                ClientScript.RegisterStartupScript(this.GetType(), "Gestione_risposta_grigio", "top.principale.document.location = '../documento/gestionedoc.aspx?tab=profilo';", true);
            }
        }

        private void btn_in_rispota_a_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null
                    && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1")
            {
                string scriptString = "<SCRIPT>ApriFinestraDocumenti('" + this.rbl_InOut_P.SelectedItem.Value + "');</SCRIPT>";
                this.RegisterStartupScript("apriModalDialogProtocolli", scriptString);
            }
            else
            {
                if (this.rbl_InOut_P.SelectedItem.Value == "In")
                {
                    string scriptString = "<SCRIPT>ApriFinestraProtocolliInUscita();</SCRIPT>";
                    this.RegisterStartupScript("apriModalDialogProtocolli", scriptString);
                }
                if (this.rbl_InOut_P.SelectedItem.Value == "Out")
                {
                    string scriptString = "<SCRIPT>ApriFinestraProtocolliInIngresso();</SCRIPT>";
                    this.RegisterStartupScript("apriModalDialogProtocolli", scriptString);
                }
            }
        }

        private void btn_risp_sx_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //Riporta al dettaglio del protocollo in ingresso o uscita  a seconda del caso
            try
            {

                if (this.txt_RispProtSegn_P != null && this.txt_RispProtSegn_P.Text != "")
                {
                    string docNum = (DocumentManager.getDocumentoInLavorazione(this)).rispostaDocumento.docNumber.ToString();
                    string tipoProto = (DocumentManager.getDocumentoInLavorazione(this)).rispostaDocumento.tipoProto.ToString();

                    DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
                    DocsPaWR.InfoDocumento infoDocumento = DocumentManager.GetCatenaDocumentoMittente(this, infoUtente.idGruppo, infoUtente.idPeople, docNum, tipoProto);

                    if (infoDocumento == null)
                    {
                        Response.Write("<script>window.alert('Non si posseggono i diritti necessari alla visualizzazione del documento richiesto.')</script>");
                    }
                    else
                    {
                        if (Session["catenaDoc"] != null)
                        {
                            Session.Remove("catenaDoc");
                        }
                        FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                        FascicoliManager.removeCodiceFascRapida(this);
                        FascicoliManager.removeDescrizioneFascRapida(this);
                        DocumentManager.setRisultatoRicerca(this, infoDocumento);
                        DocumentManager.setDocumentoSelezionato(DocumentManager.getDettaglioDocumento(this, infoDocumento.idProfile, infoDocumento.docNumber));
                        Response.Write("<script language='javascript'> top.principale.document.location = '../documento/gestionedoc.aspx?tab=protocollo';</script>");
                    }
                }
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }
        }

        protected void btn_risp_dx_Click(object sender, ImageClickEventArgs e)
        {
            DocsPAWA.DocsPaWR.InfoDocumento infoDocRisposta = GetDocInRisposta(schedaDocumento.systemId);

            if (infoDocRisposta != null)
            {
                DocumentManager.setRisultatoRicerca(this, infoDocRisposta);
                FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                FascicoliManager.removeCodiceFascRapida(this);
                FascicoliManager.removeDescrizioneFascRapida(this);
                if (!string.IsNullOrEmpty(infoDocRisposta.segnatura)) //la risposta è un doc protocollato
                {

                    Response.Write("<script language='javascript'> top.principale.document.location = '../documento/gestionedoc.aspx?tab=protocollo';</script>");

                }
                else
                {
                    Response.Write("<script language='javascript'> top.principale.document.location = '../documento/gestionedoc.aspx?tab=profilo';</script>");

                }
            }

        }

        public bool RicercaDocInRisposta(string docNum, string tipoProto)
        {
            try
            {

                qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];

                fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

                //Filtro per docNumber
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
                fV1.valore = docNum;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                //Filtro per protocolli in Arrivo
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                fV1.valore = tipoProto;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                //Filtro per prendere solo i documenti protocollati
                #region
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DA_PROTOCOLLARE.ToString();
                fV1.valore = "0";  //corrisponde a 'false'
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion
                qV[0] = fVList;
                return true;

            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
                return false;
            }
        }

        private void txt_cod_uffRef_TextChanged(object sender, System.EventArgs e)
        {
            try
            {

                setDescUfficioReferente(this.txt_cod_uffRef.Text, "U", null);

                if (txt_cod_uffRef.Text == "")
                {
                    if (schedaDocumento.tipoProto == "A")
                    {
                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).ufficioReferente = null;
                        txt_desc_uffRef.Text = "";
                    }
                    if (schedaDocumento.tipoProto == "P")
                    {
                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).ufficioReferente = null;
                        txt_desc_uffRef.Text = "";
                    }
                    if (schedaDocumento.tipoProto == "I")
                    {
                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).ufficioReferente = null;
                        txt_desc_uffRef.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "protocollazione");
            }
        }

        private void scegliUOUtente_clear()
        {
            Session.Remove("isLoaded_ScegliUoUtente");
            Session.Remove("retValue_ScegliUoUtente");
            //Session.Remove("scegliUOUtente.ruoli");
        }

        /* Questo metodo viene utilizzato solamente nel caso in cui l'amministrazione
         * utilizzi il campo ufficio referente (ciò si specifica nel web.config della WA)*/
        private bool scegliUOUtente_load()
        {
            bool retValue_scegliUoUtente = false;
            bool isLoad_scegliUoUtente = false;
            bool returnValue = true;
            if (Session["isLoaded_ScegliUoUtente"] != null && Session["retValue_ScegliUoUtente"] != null)
            {
                isLoad_scegliUoUtente = (bool)Session["isLoaded_ScegliUoUtente"]; //true = la pagine ScegliUoUtente è stata caricata
                retValue_scegliUoUtente = (bool)Session["retValue_ScegliUoUtente"];
            }
            //se la pagina ScegliUoUtente è stata caricata
            if (isLoad_scegliUoUtente == true)
            {
                //devo vedere se vengo dalla pressione del pulsante ANNULLA o da X (chiusura della finestra)
                if (retValue_scegliUoUtente == false) // caso in cui vengo da X
                {
                    returnValue = retValue_scegliUoUtente;
                }
                if (retValue_scegliUoUtente == true) // caso in cui vengo da ANNULLA
                {
                    returnValue = retValue_scegliUoUtente;
                }
            }

            return returnValue;
        }


        #region ProfilazioneDinamica

        private void impostaStatoIniziale(DocsPAWA.DocsPaWR.Templates template)
        {

            //DIAGRAMMI DI STATO
            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
            {
                DocsPaWR.DocsPaWebService wws = new DocsPaWebService();

                DocsPaWR.Stato statoDocumento = DocsPAWA.DiagrammiManager.getStatoDoc(this.schedaDocumento.docNumber, this);
                if (statoDocumento != null)
                    return;

                int idDiagramma = DocsPAWA.DiagrammiManager.getDiagrammaAssociato(template.SYSTEM_ID.ToString(), this);
                if (idDiagramma != 0 && idDiagramma != null)
                {
                    DocsPaWR.DiagrammaStato diagramma = DocsPAWA.DiagrammiManager.getDiagrammaById(idDiagramma.ToString(), this);
                    if (diagramma != null)
                    {
                        int contatoreStatiIniziali = 0;
                        DocsPaWR.Stato statoIniziale = new DocsPaWR.Stato();
                        for (int i = 0; i < diagramma.STATI.Length; i++)
                        {
                            DocsPaWR.Stato stato = (DocsPaWR.Stato)diagramma.STATI[i];
                            if (stato.STATO_INIZIALE)
                            {
                                contatoreStatiIniziali++;
                                statoIniziale = stato;
                            }
                        }

                        if (contatoreStatiIniziali == 1)
                        {
                            DocsPAWA.DiagrammiManager.salvaModificaStato(this.schedaDocumento.docNumber, statoIniziale.SYSTEM_ID.ToString(), diagramma, UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), "", this);

                            // Aggiornamento stato di consolidamento del documento successivamente al passaggio ad uno stato successivo
                            this.schedaDocumento.ConsolidationState = this.wws.GetDocumentConsolidationState(UserManager.getInfoUtente(), this.schedaDocumento.systemId);

                            //Verifico se esistono delle trasmissioni da fare
                            string idTemplate = ProfilazioneDocManager.getIdTemplate(schedaDocumento.docNumber, this);

                            if (idTemplate != "")
                            {
                                ArrayList modelli = new ArrayList(DocsPAWA.DiagrammiManager.isStatoTrasmAuto(UserManager.getInfoUtente(this).idAmministrazione, statoIniziale.SYSTEM_ID.ToString(), idTemplate, this));
                                for (int i = 0; i < modelli.Count; i++)
                                {
                                    DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                                    if (mod.SINGLE == "1")
                                    {
                                        TrasmManager.effettuaTrasmissioneDocDaModello(mod, statoIniziale.SYSTEM_ID.ToString(), schedaDocumento, this);
                                    }
                                    else
                                    {
                                        for (int j = 0; j < mod.MITTENTE.Length; j++)
                                        {
                                            if (mod.MITTENTE[j].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                            {
                                                TrasmManager.effettuaTrasmissioneDocDaModello(mod, statoIniziale.SYSTEM_ID.ToString(), schedaDocumento, this);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //FINE DIAGRAMMI DI STATO

            //Data Scadenza
            if (template.SCADENZA != null && template.SCADENZA != "" && template.SCADENZA != "0")
            {
                try
                {
                    DateTime dataOdierna = System.DateTime.Now;
                    int scadenza = Convert.ToInt32(template.SCADENZA);
                    DateTime dataCalcolata = dataOdierna.AddDays(scadenza);
                    DocsPAWA.DiagrammiManager.salvaDataScadenzaDoc(schedaDocumento.docNumber, Utils.formatDataDocsPa(dataCalcolata), template.SYSTEM_ID.ToString(), this);
                }
                catch (Exception ex) { }
            }
        }

        #endregion ProfilazioneDinamica

        #region Utils per ModelliTrasmissione
        private void caricaModelliTrasm()
        {
            logger.Info("BEGIN");
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
                if (schedaDocumento.tipologiaAtto != null)
                {
                    //DocsPaWR.Templates template = (Templates)Session["template"];
                    //if (template == null)
                    //    template = wws.getTemplate(idAmm, schedaDocumento.tipologiaAtto.descrizione, schedaDocumento.docNumber);

                    //if (template != null)
                    //{
                    //    idTipoDoc = template.SYSTEM_ID.ToString();
                    if (schedaDocumento.template != null && schedaDocumento.template.SYSTEM_ID.ToString() != "" && !isRiproposto)
                    {
                        idTipoDoc = schedaDocumento.template.SYSTEM_ID.ToString();

                        if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                        {
                            DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDgByIdTipoDoc(schedaDocumento.tipologiaAtto.systemId, idAmm, this);
                            if (dg != null)
                            {
                                idDiagramma = dg.SYSTEM_ID.ToString();
                                DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber, this);
                                if (stato != null)
                                    idStato = stato.SYSTEM_ID.ToString();
                            }
                        }
                    }
                }
            }

            //ArrayList idModelli = new ArrayList(ModelliTrasmManager.getModelliPerTrasm(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, "D", this, this.schedaDocumento.systemId, idRuoloUtente, false, schedaDocumento.accessRights));
            ArrayList idModelli = new ArrayList(ModelliTrasmManager.getModelliPerTrasmLite(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, "D", this, this.schedaDocumento.systemId, idRuoloUtente, false, schedaDocumento.accessRights));

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

                li.Value = mod.SYSTEM_ID.ToString();
                li.Text = mod.NOME;
                if (System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_CODICE_MODELLI_TRASM"] != null && System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_CODICE_MODELLI_TRASM"] == "1")
                    li.Text += " (" + mod.CODICE + ")";
                ddl_tmpl.Items.Add(li);
            }

            if (idModelli.Count > 0)
                ddl_tmpl.Items.Add(separatore);

            if (isRiproposto)
                if (Session["Modello"] != null)
                {
                    DocsPaWR.ModelloTrasmissione modello = (DocsPaWR.ModelloTrasmissione)Session["Modello"];
                    this.ddl_tmpl.SelectedValue = modello.SYSTEM_ID.ToString();
                    this.txt_codModello.Text = modello.CODICE;
                }
            logger.Info("END");
        }
        #endregion


        private void set_btn_Rubrica_ref_event()
        {
            string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
            if (use_new_rubrica != "1")
                btn_Rubrica_ref.Attributes.Add("onclick", "ApriRubrica('proto','U');");
            else
                btn_Rubrica_ref.Attributes.Add("onclick", "_ApriRubrica('uffref_proto');");
        }



        public void tastoInvio()
        {
            if (UserManager.getStatoRegistro(schedaDocumento.registro) == "G")
            {
                //Utils.DefaultButton(this, ref txt_oggetto_P, ref btn_protocollaGiallo_P);
                ctrl_oggetto.DefButton_Ogg(ref btn_protocollaGiallo_P);
                /*Utils.DefaultButton (this, ref txt_CodMit_P, ref btn_protocollaGiallo_P);*/
                if (string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) || !(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                {
                    Utils.DefaultButton(this, ref txt_DescMit_P, ref btn_protocollaGiallo_P);
                }
                Utils.DefaultButton(this, ref txt_CodMitInt_P, ref btn_protocollaGiallo_P);
                Utils.DefaultButton(this, ref txt_DescMitInt_P, ref btn_protocollaGiallo_P);
                Utils.DefaultButton(this, ref txt_NumProtMit_P, ref btn_protocollaGiallo_P);
                //Utils.DefaultButton (this, ref txt_DataProtMit_P, ref btn_protocollaGiallo_P);
                //Utils.DefaultButton (this, ref txt_DataArrivo_P, ref btn_protocollaGiallo_P);
                /*Utils.DefaultButton (this, ref txt_CodDest_P, ref btn_protocollaGiallo_P);*/
                if (string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) || !(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                {
                    Utils.DefaultButton(this, ref txt_DescDest_P, ref btn_protocollaGiallo_P);
                }
                //Utils.DefaultButton (this, ref lbx_dest, ref btn_protocollaGiallo_P);
                //Utils.DefaultButton (this, ref lbx_destCC, ref btn_protocollaGiallo_P);
                //Utils.DefaultButton (this, ref ddl_tipoAtto, ref btn_protocollaGiallo_P);
                Utils.DefaultButton(this, ref txt_RispProtSegn_P, ref btn_protocollaGiallo_P);
                Utils.DefaultButton(this, ref txt_cod_uffRef, ref btn_protocollaGiallo_P);
                Utils.DefaultButton(this, ref txt_desc_uffRef, ref btn_protocollaGiallo_P);
                Utils.DefaultButton(this, ref txt_dataAnnul_P, ref btn_protocollaGiallo_P);
                Utils.DefaultButton(this, ref txt_numAnnul_P, ref btn_protocollaGiallo_P);
                Utils.DefaultButton(this, ref txt_CodFascicolo, ref btn_protocollaGiallo_P);
                Utils.DefaultButton(this, ref txt_DescFascicolo, ref btn_protocollaGiallo_P);
                //Utils.DefaultButton (this, ref ddl_tmpl, ref btn_protocollaGiallo_P);
                Utils.DefaultButton(this, ref txt_dta_protoEme, ref btn_protocollaGiallo_P);
                Utils.DefaultButton(this, ref txt_ProtoEme, ref btn_protocollaGiallo_P);
            }
            else
            {
                //Utils.DefaultButton(this, ref txt_oggetto_P, ref btn_protocolla_P);
                ctrl_oggetto.DefButton_Ogg(ref btn_protocolla_P);
                /*Utils.DefaultButton (this, ref txt_CodMit_P, ref btn_protocolla_P);*/
                if (string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) || !(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                {
                    Utils.DefaultButton(this, ref txt_DescMit_P, ref btn_protocolla_P);
                }
                Utils.DefaultButton(this, ref txt_CodMitInt_P, ref btn_protocolla_P);
                Utils.DefaultButton(this, ref txt_DescMitInt_P, ref btn_protocolla_P);
                Utils.DefaultButton(this, ref txt_NumProtMit_P, ref btn_protocolla_P);
                //Utils.DefaultButton (this, ref txt_DataProtMit_P, ref btn_protocollaGiallo_P);
                //Utils.DefaultButton (this, ref txt_DataArrivo_P, ref btn_protocollaGiallo_P);
                /*Utils.DefaultButton (this, ref txt_CodDest_P, ref btn_protocolla_P);*/
                if (string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")) || !(utils.InitConfigurationKeys.GetValue("0", "FE_NEW_RUBRICA_VELOCE")).Equals("1"))
                {
                    Utils.DefaultButton(this, ref txt_DescDest_P, ref btn_protocolla_P);
                }
                //Utils.DefaultButton (this, ref lbx_dest, ref btn_protocollaGiallo_P);
                //Utils.DefaultButton (this, ref lbx_destCC, ref btn_protocollaGiallo_P);
                //Utils.DefaultButton (this, ref ddl_tipoAtto, ref btn_protocollaGiallo_P);
                Utils.DefaultButton(this, ref txt_RispProtSegn_P, ref btn_protocolla_P);
                Utils.DefaultButton(this, ref txt_cod_uffRef, ref btn_protocolla_P);
                Utils.DefaultButton(this, ref txt_desc_uffRef, ref btn_protocolla_P);
                Utils.DefaultButton(this, ref txt_dataAnnul_P, ref btn_protocolla_P);
                Utils.DefaultButton(this, ref txt_numAnnul_P, ref btn_protocolla_P);
                Utils.DefaultButton(this, ref txt_CodFascicolo, ref btn_protocolla_P);
                Utils.DefaultButton(this, ref txt_DescFascicolo, ref btn_protocolla_P);
                //Utils.DefaultButton (this, ref ddl_tmpl, ref btn_protocollaGiallo_P);
                Utils.DefaultButton(this, ref txt_dta_protoEme, ref btn_protocolla_P);
                Utils.DefaultButton(this, ref txt_ProtoEme, ref btn_protocolla_P);
            }
        }

        private void btn_CampiPersonalizzati_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string pulsanti = btn_salva_P.Enabled + "-" + btn_protocolla_P.Enabled + "-" + btn_protocollaGiallo_P.Enabled;
            ClientScript.RegisterStartupScript(this.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1&pulsanti=" + pulsanti + "'", true);
        }

        private void btn_trasmetti_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=protocollo';</script>");

            return;
        }


        private void btn_spedisci_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=protocollo';</script>");

            return;

            //if (!this.GetControlAclDocumento().AclRevocata)
            //{
            //    this.hdnSpedisciConInterop.Value = "1";
            //    int docAcquisito = 0;

            //    if (spedisciConInterop_new())
            //    {
            //        if (schedaDocumento.documenti != null && schedaDocumento.documenti[0] != null)
            //        {
            //            if (schedaDocumento.documenti.Length > 0)
            //            {
            //                try
            //                {
            //                    int fileSize = Convert.ToInt32(schedaDocumento.documenti[0].fileSize);
            //                    if (fileSize > 0)
            //                        docAcquisito = 1;
            //                    else
            //                        docAcquisito = 0;
            //                }
            //                catch
            //                {
            //                    docAcquisito = 0;
            //                }
            //            }
            //            else
            //            {
            //                docAcquisito = 0;
            //            }
            //        }

            //        int res = DocumentManager.verificaSpedizioneDocumento(this, schedaDocumento.systemId);

            //        switch (res)
            //        {
            //            case 0: // il doc non è stato spedito

            //                switch (docAcquisito)
            //                {
            //                    case 0: // se il doc non è stato acquisito elettronicamente
            //                        {
            //                            if (!IsStartupScriptRegistered("confirmSpedisciAcquisito"))
            //                                Page.RegisterStartupScript("confirmSpedisci", "<script>ApriconfirmSpedizioneDocUnica('Non è stato acquisito un documento elettronico.');</script>");

            //                            break;
            //                        }
            //                    case 1: // se il doc è stato acquisito
            //                        {
            //                            try
            //                            {
            //                                Page.RegisterStartupScript("", "<script>document.getElementById('hdConfirmSpedisci').value = 'Yes';document.getElementById('hdnSpedisciConInterop').value = '1';document.docProtocollo.submit();doWait();</script>");
            //                                //spedisciDoc();

            //                            }
            //                            catch (Exception ex)
            //                            {
            //                                string script = "<script>if(window.parent.parent.document.getElementById ('please_wait')!=null)";
            //                                script += "{";
            //                                script += "		window.parent.parent.document.getElementById ('please_wait').style.display = 'none'";
            //                                script += "} </script>";
            //                                Response.Write(script);
            //                                ErrorManager.redirect(this, ex, "spedizione");
            //                            }
            //                            break;
            //                        }
            //                }

            //                break;

            //            case 1: // il doc è stato già spedito
            //                {
            //                    switch (docAcquisito)
            //                    {
            //                        case 0: // se il doc non è stato acquisito elettronicamente
            //                            {
            //                                if (!IsStartupScriptRegistered("confirmSpedisci"))
            //                                    Page.RegisterStartupScript("confirmSpedisci", "<script>ApriconfirmSpedizioneDocUnica('Il documento è già stato spedito.\\n\\nNon è stato acquisito un documento elettronico.');</script>");

            //                                break;
            //                            }
            //                        case 1: // se il doc è stato acquisito
            //                            {
            //                                if (!IsStartupScriptRegistered("confirmSpedisci"))
            //                                    Page.RegisterStartupScript("confirmSpedisci", "<script>ApriconfirmSpedizioneDocUnica('Il documento è già stato spedito.');</script>");

            //                                break;
            //                            }
            //                        case 2: // 
            //                            {
            //                                throw new Exception("Errore nella verifica della spedizione del documento");
            //                            }

            //                    }
            //                    break;
            //                }

            //        }

            //    }
            //}
        }

        private void ddl_tmpl_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (ddl_tmpl.SelectedValue != null && ddl_tmpl.SelectedValue.Trim() != string.Empty)
            {
                if (Session["rubrica.campoCorrispondente"] != null)
                    Session.Remove("rubrica.campoCorrispondente");
                if (Session["CorrSelezionatoDaMulti"] != null)
                    Session.Remove("CorrSelezionatoDaMulti");
                //if (Session["dictionaryCorrispondente"] != null)
                //    Session.Remove("dictionaryCorrispondente");

                if (Session["isDocModificato"] != null && Session["isDocModificato"].ToString().ToUpper() == "TRUE")
                    this.daAggiornareDx = false;
                //MODELLI TRASMISSIONE NUOVI
                DocsPaWR.ModelloTrasmissione modello = new DocsPAWA.DocsPaWR.ModelloTrasmissione();
                //int indexSel = ddl_tmpl.Items.IndexOf(ddl_tmpl.Items.FindByValue(separatore));
                int indexSel = ddl_tmpl.SelectedIndex;

                //se l'item selezionato è il separatore oppure è un template vecchio..esco dal metodo
                if (ddl_tmpl.SelectedItem.Text == separatore || indexSel <= 0)
                {
                    Session.Remove("Modello");
                    return;
                }
                modello = ModelliTrasmManager.getModelloByID(UserManager.getRegistroSelezionato(this).idAmministrazione, ddl_tmpl.SelectedValue, this);
                this.txt_codModello.Text = modello.CODICE;
                if (modello != null && modello.SYSTEM_ID != 0)
                {
                    Session.Add("Modello", modello);

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

                    if (this.wws.ereditaVisibilita(appoIdAmm.Value, appoIdMod.Value))
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

        #region Trasmissione rapida
        //MODELLI TRASMISSIONE NUOVI
        private void execTrasmRapida()
        {
            logger.Info("BEGIN");
            int indexSel = ddl_tmpl.Items.IndexOf(ddl_tmpl.Items.FindByValue(separatore));

            if (Request.QueryString["protocolla"] != null && Request.QueryString["protocolla"].Equals("1"))
                if (Session["modelloTrasmDuplicato"] != null && !Session["modelloTrasmDuplicato"].Equals(""))
                {
                    this.ddl_tmpl.SelectedIndex = Convert.ToInt16(Session["modelloTrasmDuplicato"]);
                    Session.Remove("modelloTrasmDuplicato");
                }

            if (Request.QueryString["protocolla"] != null && Request.QueryString["protocolla"].Equals("1"))
                if (Session["modelloTrasmCodBisSegn"] != null && !Session["modelloTrasmCodBisSegn"].Equals(""))
                {
                    this.ddl_tmpl.SelectedIndex = Convert.ToInt16(Session["modelloTrasmCodBisSegn"]);
                    Session.Remove("modelloTrasmCodBisSegn");
                }
            string ddlTempl = Request.QueryString["ddlTempl"];
            if (!string.IsNullOrEmpty(ddlTempl))
                this.ddl_tmpl.SelectedIndex = Convert.ToInt32(ddlTempl);

            //Modifica PALUMBO per permettere la trasmissione rapida dopo l'apertura di K1-K2
            if (Convert.ToInt32(Session["indModelloTrasm"]) > 0)
            {
                this.ddl_tmpl.SelectedIndex = Convert.ToInt32(Session["indModelloTrasm"]);
                Session.Remove("indModelloTrasm");
            }

            if (this.ddl_tmpl.SelectedIndex > 0 && ddl_tmpl.SelectedIndex < indexSel)
            {
                if (Session["Modello"] != null)
                {
                    DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
                    Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();

                    //Parametri della trasmissione
                    trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;
                    trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
                    trasmissione.infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
                    trasmissione.utente = UserManager.getUtente(this);
                    trasmissione.ruolo = UserManager.getRuolo(this);
                    if (modello != null)
                        trasmissione.NO_NOTIFY = modello.NO_NOTIFY;
                    //Parametri delle trasmissioni singole
                    for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                    {
                        DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                        ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                        for (int j = 0; j < destinatari.Count; j++)
                        {
                            DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                            DocsPaWR.Corrispondente corr = new Corrispondente();
                            //old: ritoranva anche i corr diasbilitati DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                            if (mittDest.CHA_TIPO_MITT_DEST == "D")
                            {
                                corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                            }
                            else
                            {
                                corr = TrasmManager.getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, schedaDocumento, null, this);
                            }
                            if (corr != null)
                            {   //il corr è null se non esiste o se è stato disabiltato.    
                                DocsPaWR.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                                
                                //Andrea - Try - catch
                                try
                                {
                                    trasmissione = TrasmManager.addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, mittDest.NASCONDI_VERSIONI_PRECEDENTI, this);
                                }
                                catch (ExceptionTrasmissioni e) 
                                {
                                    //Aggiungo l'errore alla lista
                                    listaExceptionTrasmissioni.Add(e.Messaggio);
                                    /*
                                    foreach (string s in listaExceptionTrasmissioni)
                                    {
                                        //messError = messError + s + "\r\n";
                                        messError = messError + s + "|";
                                    }

                                    if (messError != "")
                                    {
                                        Session.Add("MessError", messError);

                                        //Response.Write("<script language='javascript'>window.alert(" + messError + ");</script>");
                                    }*/
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

                    DocsPaWR.Trasmissione t_rs = null;
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

                        //Nuovo metodo saveExecuteTrasm

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

                        if (this.verificaNotificheUtentiDaModello(trasmissione))
                            t_rs = TrasmManager.saveExecuteTrasm(this, trasmissione, infoUtente);
                        else
                            Response.Write("<script>window.alert('Trasmissione non effettuata in quanto il modello di trasmissione utilizzato prevede dei ruoli per i quali non sono presenti utenti con notifica. \\n Ricontrollare il modello.');</script>");
                        //trasmissione = TrasmManager.saveTrasm(this, trasmissione);
                        //t_rs = TrasmManager.executeTrasm(this, trasmissione);
                    }
                    if (t_rs != null && t_rs.ErrorSendingEmails)
                        Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");

                    //Salvataggio del system_id della trasm per il cambio di stato automatico
                    if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                    {
                        DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber, this);
                        bool trasmWF = false;
                        if (trasmissione != null && trasmissione.trasmissioniSingole != null)
                            for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                            {
                                DocsPaWR.TrasmissioneSingola trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                                if (trasmSing.ragione.tipo == "W")
                                    trasmWF = true;
                            }
                        if (stato != null && trasmWF)
                            DocsPAWA.DiagrammiManager.salvaStoricoTrasmDiagrammi(trasmissione.systemId, schedaDocumento.docNumber, Convert.ToString(stato.SYSTEM_ID), this);
                    }
                    return;
                }
            }
            //FINE MODELLI TRASMISSIONE NUOVI
            DocsPaWR.Trasmissione trasmEff = null;
            try
            {

                if (this.ddl_tmpl.SelectedIndex > 0 && ddl_tmpl.SelectedIndex > indexSel)
                {
                    trasmEff = creaTrasmissione();

                    if (trasmEff == null)
                        Response.Write("<script language='javascript'>alert('Si è verificato un errore nella creazione della trasmissione da template');</script>");
                    else
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

                        DocsPaWR.Trasmissione t_rs = TrasmManager.executeTrasm(this, trasmEff);

                        if (t_rs != null && t_rs.ErrorSendingEmails)
                            Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");

                        //	Response.Write("<script>parent.IframeTabs.document.location='docTrasmissioni.aspx';</script>");	
                        //Response.Write("<script language='javascript'>parent.IframeTabs.document.location='docTrasmissioni.aspx';</script>");	
                        //resetto il template della trasmissione

                        this.ddl_tmpl.SelectedIndex = 0;

                        //Salvataggio del system_id della trasm per il cambio di stato automatico
                        if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                        {
                            DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber, this);
                            bool trasmWF = false;
                            for (int i = 0; i < trasmEff.trasmissioniSingole.Length; i++)
                            {
                                DocsPaWR.TrasmissioneSingola trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmEff.trasmissioniSingole[i];
                                if (trasmSing.ragione.tipo == "W")
                                    trasmWF = true;
                            }
                            if (stato != null && trasmWF)
                                DocsPAWA.DiagrammiManager.salvaStoricoTrasmDiagrammi(trasmEff.systemId, schedaDocumento.docNumber, Convert.ToString(stato.SYSTEM_ID), this);
                        }
                    }
                }
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(this, es);
            }
            logger.Info("END");
        }

        private bool verificaNotificheUtentiDaModello(DocsPaWR.Trasmissione objTrasm)
        {
            if (objTrasm.trasmissioniSingole != null && objTrasm.trasmissioniSingole.Length > 0)
            {
                for (int cts = 0; cts < objTrasm.trasmissioniSingole.Length; cts++)
                {
                    if (objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length > 0)
                    {
                        bool boolUtente = false;
                        for (int ctu = 0; ctu < objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length; ctu++)
                        {
                            if (this.daNotificareSuModello(objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].utente.idPeople, objTrasm.trasmissioniSingole[cts].corrispondenteInterno.systemId))
                            {
                                boolUtente = true;
                            }
                        }
                        if (!boolUtente)
                            return false;
                    }
                }
            }

            return true;
        }

        private DocsPAWA.DocsPaWR.Trasmissione creaTrasmissione()
        {
            //crea trasmissione da template
            DocsPaWR.TemplateTrasmissione[] listaTmp;
            DocsPaWR.Trasmissione trasmissione = null;
            try
            {
                DocsPaWR.TemplateTrasmissione template = null;
                DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);
                if (schedaDocumento == null)
                    schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
                DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaDocumento);
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

            return trasmissione;
        }
        #endregion


        #region fascicolazione rapida

        public void setFascicolazioneRapida()
        {
            DocsPaWR.Fascicolo fascRap = new Fascicolo();
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
                this.txt_CodFascicolo.Text = "";
                this.txt_DescFascicolo.Text = "";
                //this.riproponi();
            }

            //setto la tooltip del fascicolo
            this.txt_DescFascicolo.ToolTip = txt_DescFascicolo.Text;
        }


        /// <summary>
        /// Gestione deallocazione risorse utilizzata dalla dialog ricerca fascicoli
        /// </summary>
        private void ClearResourcesRicercaFascicoliFascRapida()
        {
            //Rimuove le variabili usate per la gestione della pagina ricercaFascicoli.aspx
            if (DocsPAWA.popup.ricercaFascicoli.RicercaFascicoliSessionMng.IsLoaded(this))
            {
                DocsPAWA.popup.ricercaFascicoli.RicercaFascicoliSessionMng.SetAsNotLoaded(this);
                DocsPAWA.popup.ricercaFascicoli.RicercaFascicoliSessionMng.ClearSessionData(this);
            }
        }

        protected void btn_titolario_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ////E' necessario che sia selezionato un titolario e non la voce "tutti i titolari"
            ArrayList listaTitolari = new ArrayList(wws.getTitolariUtilizzabili(UserManager.getUtente(this).idAmministrazione));
            string idTitolario = this.getIdTitolario("", listaTitolari);
            FascicoliManager.removeFascicoloSelezionatoFascRapida();
            if (!string.IsNullOrEmpty(idTitolario))
            {
                if (!this.IsStartupScriptRegistered("apriModalDialog"))
                {
                    //string scriptString = "<SCRIPT>ApriTitolario('codClass=" + txt_codClass.Text + "&idTit=" + ddl_titolari.SelectedValue + "','gestClass')</SCRIPT>";
                    string scriptString = "<SCRIPT>ApriTitolario('codClass=" + string.Empty + "&idTit=" + idTitolario + "','gestProt')</SCRIPT>";
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
            foreach (DocsPaWR.OrgTitolario titolario in titolari)
            {
                if (titolario.Stato == DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Attivo)
                {
                    result = titolario.ID;
                }
            }
            return result;
        }
        #endregion

        private void imgFasc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
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
                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, fasc.idClassificazione, UserManager.getUtente(this).idAmministrazione);
                        string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "')</script>");
                    }
                }
            }
            else
            {
                if (this.txt_CodFascicolo.Text != "")
                {
                    DocsPaWR.Fascicolo fascSel = getFascicolo();

                    if (fasc != null)
                    {
                        FascicoliManager.setFascicoloSelezionatoFascRapida(this, fascSel);

                        if (fascSel.tipo.Equals("G"))
                        {
                            RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + fascSel.codice + "')</script>");
                        }
                        else
                        {
                            ///se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                            DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, fascSel.idClassificazione, UserManager.getUtente(this).idAmministrazione);
                            string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                            RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "')</script>");
                        }
                    }
                }
                else
                {
                    if (!(Session["validCodeFasc"] != null && Session["validCodeFasc"].ToString() == "false"))
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + txt_CodFascicolo.Text + "')</script>");
                }
            }
        }

        //controllo per la creazione di un nuovo fascicolo procedimentale e la sua successiva selezione 
        //auomatica all'interno della form di fascicolazione rapida

        private void imgNewFasc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DocsPaWR.Registro reg = wws.GetRegistroBySistemId(schedaDocumento.registro.systemId);
            //Session["protocollo"] = schedaDocumento;
            ArrayList listaTitolari = new ArrayList(wws.getTitolariUtilizzabili(UserManager.getUtente(this).idAmministrazione));
            if (reg != null && reg.Sospeso)
            {
                RegisterClientScriptBlock("alertRegistroSospeso", "<SCRIPT language='javascript'>alert('Il registro selezionato è sospeso!');</SCRIPT>");
                return;
            }

            DocsPAWA.DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
            if (fasc != null && fasc.isFascConsentita != null && fasc.isFascConsentita == "0")
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_fasc", "alert('Non è possibile creare un fascicolo nel fascicolo selezionato.');", true);
                return;
            }

            string profilazione = System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"];
            if (this.txt_CodFascicolo.Text.Trim().Equals(""))
            {
                RegisterClientScriptBlock("alertCodiceInesistente", "<SCRIPT language='javascript'>alert('Inserire un nodo di titolario!');</SCRIPT>");
            }


            if (!this.txt_CodFascicolo.Text.Trim().Equals(""))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "apreGestNew", "ApriFinestraNewFascNewTit('" + Server.UrlEncode(this.txt_CodFascicolo.Text) + "','docProtocollo','fascNewFascicolo.aspx','" + profilazione + "','" + getIdTitolario(this.txt_CodFascicolo.Text, listaTitolari) + "');", true);
            }

        }


        private void btn_dettDestCC_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        private void btn_notifica_sped_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        private void btn_notifica_sped_CC_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        #region Gestione CallContext

        /// <summary>
        /// Impostazione dei dati identificativi del documento corrente nel contesto corrente.
        /// In base agli identificativi immessi, il documento verrà
        /// ripristinato in una fase di back.
        /// </summary>
        private void SetDocumentOnContext()
        {
            // Se il documento è presente su database
            if (!string.IsNullOrEmpty(this.schedaDocumento.systemId))
            {
                SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
                if (currentContext.ContextName == SiteNavigation.NavigationKeys.DOCUMENTO ||
                    currentContext.ContextName == SiteNavigation.NavigationKeys.ALLEGATO)
                {
                    currentContext.ContextState["idProfile"] = this.schedaDocumento.systemId;
                    currentContext.ContextState["docNumber"] = this.schedaDocumento.docNumber;
                }
            }
        }

        #endregion

        protected void btn_StampaVoidLabel_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void imgDescOgg_Click(object sender, ImageClickEventArgs e)
        {
            //Session.Add("SessionDescCampo", this.txt_oggetto_P.Text);
            Session.Add("SessionDescCampo", this.ctrl_oggetto.oggetto_text);
            bool valore_o = false;
            if (btn_salva_P.Enabled || btn_protocolla_P.Enabled)
            {
                valore_o = true;
            }
            Session.Add("Abilitato_modifica", valore_o);
            this.ClientScript.RegisterStartupScript(this.GetType(), "openDescOggetto", "ApriDescrizioneCampo('O');", true);
            this.ClientScript.RegisterStartupScript(this.GetType(), "apri_nota", "top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=protocollo';", true);
        }

        protected void imgListaDest_Click(object sender, ImageClickEventArgs e)
        {
            Session.Add("SessionDescCampo", getStringaListaDestinatari());
            RegisterStartupScript("openListaDest", "<SCRIPT>ApriDescrizioneCampo('D');</SCRIPT>");
        }

        protected void imgListaDestCC_Click(object sender, ImageClickEventArgs e)
        {
            Session.Add("SessionDescCampo", getStringaListaDestinatariCC());
            RegisterStartupScript("openListaDest", "<SCRIPT>ApriDescrizioneCampo('C');</SCRIPT>");
        }

        private string getStringaListaDestinatari()
        {
            string toolTip = "";
            foreach (ListItem e in lbx_dest.Items)
            {
                toolTip = toolTip + e.Text + "<br>";
            }
            return toolTip;
        }

        private string getStringaListaDestinatariCC()
        {
            string toolTip = "";
            foreach (ListItem e in lbx_destCC.Items)
            {
                toolTip = toolTip + e.Text + "<br>";
            }
            return toolTip;
        }

        protected void btn_Correttore_Click(object sender, ImageClickEventArgs e)
        {
            this.ctrl_oggetto.SpellCheck();
        }

        protected Oggetto GetControlOggetto()
        {
            return (Oggetto)this.FindControl("ctrl_oggetto");
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool NotaDocumentoEnabled
        {
            get
            {
                return this.FindControl("tbl_note").Visible;
            }
            set
            {
                this.FindControl("tbl_note").Visible = value;
            }
        }

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

        #region utils gestione risposta
        private bool VerificaRispostaDocumento(ref string msg)
        {
            bool rispostaOK = true;
            bool registroOK = true;
            int registroDocCorrente = 0;
            string msgR = string.Empty;
            if (schedaDocumento != null && schedaDocumento.registro != null && !string.IsNullOrEmpty(schedaDocumento.registro.systemId))
            {
                if (schedaDocumento.rispostaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.rispostaDocumento.codRegistro))
                {
                    if (!schedaDocumento.registro.codRegistro.Equals(schedaDocumento.rispostaDocumento.codRegistro) && !UserManager.isFiltroAooEnabled(this))
                    {
                        registroOK = false;
                        msgR = "Attenzione si stanno collegando documenti su registri diversi.";
                    }
                }
            }
            if (registroOK)
            {
                if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.docNumber)) //doc creato
                {
                    //controllo sui documenti precedenti     
                    string tipoProtoDocCorrente = string.Empty;
                    switch (this.rbl_InOut_P.SelectedItem.Value)
                    {
                        case "Out":
                            tipoProtoDocCorrente = "P";
                            break;
                        case "In":
                            tipoProtoDocCorrente = "A";
                            break;
                        default:
                            tipoProtoDocCorrente = "";
                            break;

                    }
                    if (!string.IsNullOrEmpty(tipoProtoDocCorrente))
                    {
                        String[] tipoProtoPrec = DocumentManager.GetTipoProtoRisposta(schedaDocumento.systemId);
                        for (int i = 0; i < tipoProtoPrec.Length; i++)
                        {
                            if (!tipoProtoPrec[i].Equals(tipoProtoDocCorrente))
                            {
                                rispostaOK = true;
                            }
                            else
                            {//dovrebbe essere false, ma il collegamento di doc dello stesso tipo non viene gestito

                                rispostaOK = true;
                                break;
                            }
                        }
                    }
                    //fine

                    //controllo sui doc successivi
                    if (rispostaOK)
                    {
                        if (schedaDocumento.rispostaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.rispostaDocumento.docNumber)) //esiste una risposta
                        {
                            DocsPAWA.DocsPaWR.InfoDocumento risposta = schedaDocumento.rispostaDocumento;
                            SchedaDocumento schedaDocRisposta = DocumentManager.getDettaglioDocumento(this, risposta.idProfile, risposta.docNumber);
                            if (risposta.tipoProto != "G")
                            {
                                if (!string.IsNullOrEmpty(risposta.tipoProto))
                                {
                                    if (!risposta.tipoProto.Equals(tipoProtoDocCorrente))
                                    {
                                        rispostaOK = true;
                                    }
                                    else
                                    {//dovrebbe essere false, ma il collegamento di doc dello stesso tipo non viene gestito

                                        rispostaOK = true;
                                    }
                                }
                            }
                            else
                                rispostaOK = true;
                        }
                    }
                }

            }
            if (!registroOK)
            {
                msg = msgR;
                return false;
            }
            if (!rispostaOK)
            {
                msg = "Attenzione, si stanno collegando documenti dello stesso tipo: \\nIngresso/Ingresso Uscita/Uscita";
                return false;
            }

            return true;
        }


        #endregion

        #region Area Conservazione

        private void btn_storiaCons_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string idProfile = schedaDocumento.systemId;
            RegisterStartupScript("openStoriaConsDoc", "<SCRIPT>ApriStoriaConservazione('" + idProfile + "');</SCRIPT>");
        }

        #endregion

        /// <summary>
        /// Reperimento id del documento
        /// </summary>
        /// <returns></returns>
        protected string GetIdDocumento()
        {
            if (this.schedaDocumento != null)
                return this.schedaDocumento.systemId;
            else
                return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetModelProcessorDefaultExtension()
        {
            return this.clientModelProcessor.DefaultExtension;
        }

        /// <summary>
        /// Verifica se ci sono corrispondenti interni che non siano utenti dunque soggetti ad ereditarietà
        /// </summary>
        /// <returns></returns>
        private bool CheckDestinatariInterni()
        {
            DocsPaWR.Corrispondente[] listaDest = null;
            DocsPaWR.Corrispondente[] listaDestCC = null;

            //se è abilitato l'ufficio referente invio trasmissioni con la ragione dell'amministrazione
            if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
            {
                return true;
            }

            if (regSelezionato == null)
            {
                regSelezionato = UserManager.getRegistroSelezionato(this);
            }

            if (this.schedaDocumento.protocollo != null)
            {
                if (this.schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloInterno))
                {
                    listaDest = ((DocsPAWA.DocsPaWR.ProtocolloInterno)this.schedaDocumento.protocollo).destinatari;
                    listaDestCC = ((DocsPAWA.DocsPaWR.ProtocolloInterno)this.schedaDocumento.protocollo).destinatariConoscenza;
                }

                if (this.schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloUscita))
                {
                    listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)this.schedaDocumento.protocollo).destinatari;
                    listaDestCC = ((DocsPAWA.DocsPaWR.ProtocolloUscita)this.schedaDocumento.protocollo).destinatariConoscenza;
                }
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
            return false;
        }

        protected void btn_stampa_ricevuta_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (this.ricevutaPdf)
                {
                }
            }
            catch (Exception ex)
            {

                ClientScript.RegisterClientScriptBlock(this.GetType(), "errorePdf", "javascript:alert('errore nella creazione della ricevuta in formato pdf : " + ex.Message + "');");
            }
        }

        private void impostaRiferimentoMittente()
        {
            if (wws.isEnableRiferimentiMittente() && rbl_InOut_P.SelectedItem.Value == "In")
            {
                pnl_riferimentoMittente.Visible = true;
                //Documento nuovo
                if (string.IsNullOrEmpty(schedaDocumento.systemId))
                {
                    schedaDocumento.riferimentoMittente = txt_riferimentoMittente.Text;
                }
                //Documento già salvato
                else
                {
                    if (!string.IsNullOrEmpty(schedaDocumento.riferimentoMittente))
                    {
                        txt_riferimentoMittente.Text = schedaDocumento.riferimentoMittente.Split('$')[0].ToString();
                        //txt_riferimentoMittente.ReadOnly = true;
                        //txt_riferimentoMittente.BackColor = Color.Gainsboro;
                    }
                    else
                    {
                        //txt_riferimentoMittente.Text = string.Empty;
                        //schedaDocumento.riferimentoMittente = string.Empty;
                        //pnl_riferimentoMittente.Visible = false;
                    }
                }
            }
            else
            {
                txt_riferimentoMittente.Text = string.Empty;
                schedaDocumento.riferimentoMittente = string.Empty;
                pnl_riferimentoMittente.Visible = false;
            }
        }

        private void btn_invio_mail_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

            #region oldCodice
            //schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
            //Registro reg = schedaDocumento.registro;
            //// 4) calcolo gli RF associati al registro
            //DocsPaWR.Registro[] listaRF = UserManager.getListaRegistriWithRF(this, "1", schedaDocumento.registro.systemId);
            //if (listaRF != null && listaRF.Length > 0)
            //{
            //    // 5) nel caso di un solo RF associato al registro
            //    if (listaRF.Length == 1)
            //    {
            //        schedaDocumento.id_rf_invio_ricevuta = listaRF[0].systemId;
            //    }
            //    else
            //    {
            //        // 6) caso di più RF associati al registro e con invio automatico
            //        if (listaRF.Length > 1)
            //        {
            //            RegisterStartupScript("apriRFInvioManuale", "<script>apriRFInvioManuale('ricev', " + ddl_tmpl.SelectedIndex + ", '" + this.txt_CodFascicolo.Text + "');</script>");
            //            return;
            //        }
            //    }
            //}

            //if (!string.IsNullOrEmpty(schedaDocumento.id_rf_invio_ricevuta))
            //    reg = UserManager.getRegistroBySistemId(this, schedaDocumento.id_rf_invio_ricevuta);


           

            //bool resInvioRicevuta = DocumentManager.DocumentoInvioRicevuta(Page, schedaDocumento, reg);
            //if (resInvioRicevuta)
            //    RegisterStartupScript("confermaInvio", "<script>alert('Ricevuta inviata correttamente')</script>");
            ////else
            ////   RegisterStartupScript("confermaInvio", "<script>alert('Ricevuta non inviata: ripetere la procedura')</script>");

            #endregion old_codice

            schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
            //Registro reg = schedaDocumento.registro;


            //ATTENZIONE Calcolo l'indirizzo email dal quale è stato effettuato lo scarico  -- CORREZIONE PER ENAC -- SAB 05/03/2014
            Registro reg = null;


            DataSet ds = DocsPAWA.utils.MultiCasellaManager.GetAssDocAddress(schedaDocumento.docNumber);
            if (ds != null && ds.Tables["ass_doc_rf"].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables["ass_doc_rf"].Rows)
                {
                    reg = UserManager.getRegistroBySistemId(this, row["registro"].ToString());
                }
            }

            if (reg == null)
                reg = schedaDocumento.registro;

            if (reg == null)
            {
                // 4) calcolo gli RF associati al registro
                DocsPaWR.Registro[] listaRF = UserManager.getListaRegistriWithRF(this, "1", schedaDocumento.registro.systemId);
                if (listaRF != null && listaRF.Length > 0)
                {
                    // 5) nel caso di un solo RF associato al registro
                    if (listaRF.Length == 1)
                    {
                        schedaDocumento.id_rf_invio_ricevuta = listaRF[0].systemId;
                    }
                    else
                    {
                        // 6) caso di più RF associati al registro e con invio automatico
                        if (listaRF.Length > 1)
                        {
                            RegisterStartupScript("apriRFInvioManuale", "<script>apriRFInvioManuale('ricev', " + ddl_tmpl.SelectedIndex + ", '" + this.txt_CodFascicolo.Text + "');</script>");
                            return;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(schedaDocumento.id_rf_invio_ricevuta))
                    reg = UserManager.getRegistroBySistemId(this, schedaDocumento.id_rf_invio_ricevuta);
            }




            bool resInvioRicevuta = DocumentManager.DocumentoInvioRicevuta(Page, schedaDocumento, reg);
            if (resInvioRicevuta)
                RegisterStartupScript("confermaInvio", "<script>alert('Ricevuta inviata correttamente')</script>");
  

        }

        //private DocsPAWA.UserControls.CorrispondenteDaCodice GetCorrispondenteControl(string controlId)
        //{
        //    return (DocsPAWA.UserControls.CorrispondenteDaCodice)this.FindControl(controlId);
        //}

        //INSERITA DA FABIO PRENDE LE ETICHETTE DEI PROTOCOLLI
        private void getLettereProtocolli()
        {
            DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
            string idAmm = cr.idAmministrazione;
            DocsPAWA.DocsPaWR.EtichettaInfo[] etichette;
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = new DocsPAWA.DocsPaWR.InfoUtente();
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            etichette = wws.getEtichetteDocumenti(infoUtente, idAmm);
            this.rbIn.Text = etichette[0].Etichetta; //Valore A
            this.eti_arrivo = etichette[0].Descrizione;
            this.rbOut.Text = etichette[1].Etichetta; //Valore P
            this.eti_partenza = etichette[1].Descrizione;
            this.rbOwn.Text = etichette[2].Etichetta;//Valore I
            this.eti_interno = etichette[2].Descrizione;
        }

        //CALCOLA ETICHETTA PROTOCOLLI
        private string getEtichetta(String etichetta)
        {
            if (etichetta.Equals("A"))
            {
                return this.eti_arrivo;
            }
            else
            {
                if (etichetta.Equals("P"))
                {
                    return this.eti_partenza;
                }
                else
                {
                    return this.eti_interno;
                }
            }
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
        private bool cessioneDirittiAbilitato(DocsPAWA.DocsPaWR.Trasmissione trasm, DocsPaWR.ModelloTrasmissione modello)
        {
            bool retValue = false;
            if (modello != null)
                trasm.NO_NOTIFY = modello.NO_NOTIFY;
            if (modello != null && modello.CEDE_DIRITTI.Equals("1"))
            {

                if (trasm.cessione == null)
                {
                    DocsPaWR.CessioneDocumento cessione = new DocsPAWA.DocsPaWR.CessioneDocumento();
                    cessione.docCeduto = true;
                    //*******************************************************************************************
                    // MODIFICA IACOZZILLI GIORDANO 18/07/2012
                    // Modifica inerente la cessione dei diritti di un doc da parte di un utente non proprietario ma 
                    // nel ruolo del proprietario, in quel caso non posso valorizzare l'IDPEOPLE  con il corrente perchè
                    // il proprietario può essere un altro utente del mio ruolo, quindi andrei a generare un errore nella security,
                    // devo quindi controllare che nell'idpeople venga inserito l'id corretto del proprietario.
                    string valoreChiaveCediDiritti = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_CEDI_DIRITTI_IN_RUOLO");
                    if (!string.IsNullOrEmpty(valoreChiaveCediDiritti) && valoreChiaveCediDiritti.Equals("1"))
                    {
                        //Devo istanziare una classe utente.
                        string idProprietario = string.Empty;
                        if (!string.IsNullOrEmpty(schedaDocumento.systemId) && !string.IsNullOrEmpty(schedaDocumento.docNumber))
                            idProprietario = GetAnagUtenteProprietario(schedaDocumento.docNumber);
                        else
                            idProprietario = UserManager.getInfoUtente(this).idPeople;
                        //idProprietario = GetAnagUtenteProprietario();
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
        private string GetAnagUtenteProprietario(string docnumber)
        {
            DocumentoDiritto[] listaVisibilita = null;
            //DocsPaWR.SchedaDocumento sd = DocumentManager.getDocumentoSelezionato(this);
            string idProprietario = string.Empty;
            //listaVisibilita = DocumentManager.getListaVisibilitaSemplificata(this, sd.docNumber, true);
            listaVisibilita = DocumentManager.getListaVisibilitaSemplificata(this, docnumber, true);
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
            utils.CessioneDiritti obj = new DocsPAWA.utils.CessioneDiritti();
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
            if (((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenti != null)
            {
                //if (lbx_mittMultiplo.Items.Count != ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenti.Length)
                //    ((ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittentiMultipli = true;
                //else
                //    ((ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittentiMultipli = false;        

                //Valido per i protocolli in Arrivo
                DocsPaWR.Corrispondente mittente;
                lbx_mittMultiplo.Items.Clear();

                for (int i = 0; i < ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenti.Length; i++)
                {
                    mittente = (((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenti[i]);
                    ListItem item = new ListItem(UserManager.getDecrizioneCorrispondenteSemplice(mittente), mittente.codiceRubrica);
                    lbx_mittMultiplo.Items.Add(item);
                }
            }

            if (((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenti == null)
                lbx_mittMultiplo.Items.Clear();
        }

        private void txt_codMittMultiplo_TextChanged(object sender, System.EventArgs e)
        {
            ////Valido per i protocolli in Arrivo
            //try
            //{
            //    if (this.rbl_InOut_P.Items[0].Selected)
            //    {
            //        if (!string.IsNullOrEmpty(this.txt_codMittMultiplo.Text))
            //        {
            //            Corrispondente corr = UserManager.getCorrispondenteByCodRubrica(this, this.txt_codMittMultiplo.Text);

            //            if (corr == null)
            //                codice_non_trovato("Nessun corrispondente trovato!", ref txt_codMittMultiplo, ref txt_descMittMultiplo);
            //            if(corr != null)
            //                txt_descMittMultiplo.Text = corr.descrizione;                        
            //        }

            //        if (string.IsNullOrEmpty(this.txt_codMittMultiplo.Text))
            //        {
            //            txt_codMittMultiplo.Text = string.Empty;
            //            txt_descMittMultiplo.Text = string.Empty;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ErrorManager.redirect(this, ex, "protocollazione");
            //}
        }

        private void btn_insMittMultiplo_Click(object sender, System.EventArgs e)
        {
            ////Valido per i protocolli in Arrivo
            //try
            //{
            //    if (this.rbl_InOut_P.Items[0].Selected)
            //    {
            //        if (!string.IsNullOrEmpty(this.txt_descMittMultiplo.Text))
            //        {
            //            Corrispondente corr = null;
            //            if(!string.IsNullOrEmpty(this.txt_codMittMultiplo.Text))
            //                corr = UserManager.getCorrispondenteByCodRubrica(this, this.txt_codMittMultiplo.Text);

            //            if (corr != null)
            //            {
            //                ((ProtocolloEntrata)schedaDocumento.protocollo).mittenti = UserManager.addCorrispondente(((ProtocolloEntrata)schedaDocumento.protocollo).mittenti, corr);
            //            }
            //            else
            //            {
            //                corr = new DocsPAWA.DocsPaWR.Corrispondente();
            //                corr.descrizione = this.txt_descMittMultiplo.Text;
            //                corr.tipoCorrispondente = "O";
            //                corr.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
            //                ((ProtocolloEntrata)schedaDocumento.protocollo).mittenti = UserManager.addCorrispondente(((ProtocolloEntrata)schedaDocumento.protocollo).mittenti, corr);
            //            }

            //            ((ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittentiMultipli = true;
            //            DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
            //            DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
            //            this.setListBoxMittentiMultipli();
            //            txt_codMittMultiplo.Text = string.Empty;
            //            txt_descMittMultiplo.Text = string.Empty;
            //        }                    
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ErrorManager.redirect(this, ex, "protocollazione");
            //}
        }

        private void btn_CancMittMultiplo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //Valido per i protocolli in Arrivo
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

        private void isEnableRubricaVeloce()
        {
            if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE)) && ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_VELOCE).Equals("1"))
            {
                this.rubrica_veloce.Visible = true;
                string type_rubrica = "";
                if (this.IsRoleInwardEnabled())
                {
                    type_rubrica = this.rbl_InOut_P.SelectedItem.Value;
                }
                else
                {
                    if (!IsRoleOutwardEnabled() && this.rbl_InOut_P.SelectedItem.Value.Equals("In"))
                    {
                        type_rubrica = "Out";
                    }
                    else
                    {
                        if (IsRoleOutwardEnabled() && this.rbl_InOut_P.SelectedItem.Value.Equals("In"))
                        {
                            type_rubrica = "Out";
                        }
                        else
                        {
                            type_rubrica = "Own";
                        }
                    }
                }
                if (Session["type_inoltro"] != null)
                {
                    type_rubrica = (string)Session["type_inoltro"];
                    Session.Remove("type_inoltro");
                }
                if (Session["type_riproponi"] != null)
                {
                    type_rubrica = (string)Session["type_riproponi"];
                    Session.Remove("type_riproponi");
                }
                if (schedaDocumento != null)
                {
                    if (schedaDocumento.predisponiProtocollazione == true)
                    {
                        type_rubrica = schedaDocumento.tipoProto;
                    }
                }
                if (type_rubrica.Equals("In") || type_rubrica.Equals("A"))
                {
                    this.rubrica_veloce.CALLTYPE_RUBRICA_VELOCE = "CALLTYPE_PROTO_IN";
                    if (DocumentManager.isEnableMittentiMultipli())
                    {
                        this.rubrica_veloce_mitt_multi.CALLTYPE_RUBRICA_VELOCE = "CALLTYPE_MITT_MULTIPLI";
                        this.rubrica_veloce_mitt_multi.Visible = true;
                    }
                }
                if (type_rubrica.Equals("Out") || type_rubrica.Equals("P"))
                {
                    this.rubrica_veloce.CALLTYPE_RUBRICA_VELOCE = "CALLTYPE_PROTO_OUT_MITT";
                    this.rubrica_veloce_destinatario.Visible = true;
                    this.rubrica_veloce_destinatario.CALLTYPE_RUBRICA_VELOCE = "CALLTYPE_PROTO_OUT";
                }
                if (type_rubrica.Equals("Own") || type_rubrica.Equals("I"))
                {
                    this.rubrica_veloce.CALLTYPE_RUBRICA_VELOCE = "CALLTYPE_PROTO_INT_MITT";
                    this.rubrica_veloce_destinatario.Visible = true;
                    this.rubrica_veloce_destinatario.CALLTYPE_RUBRICA_VELOCE = "CALLTYPE_PROTO_INT_DEST";
                }
            }
        }

        private void btn_upMittente_Click(object sender, System.EventArgs e)
        {
            if (lbx_mittMultiplo.SelectedIndex != -1 && ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenti != null)
            {
                txt_CodMit_P.Text = lbx_mittMultiplo.SelectedItem.Value;
                txt_DescMit_P.Text = lbx_mittMultiplo.SelectedItem.Text;

                Corrispondente corr = null;
                corr = UserManager.getCorrispondenteByCodRubrica(this, txt_CodMit_P.Text);
                //RubricaCallType calltype = DocsPaWR.RubricaCallType.CALLTYPE_PROTO_IN;
                //corr = UserManager.getCorrispondenteRubrica(this, this.txt_CodMit_P.Text, calltype);

                if (corr != null)
                {
                    this.hiddenIdCodMit_p.Value = corr.systemId;
                    setDescCorrispondente(this.txt_CodMit_P.Text, "Mit", true);
                }
                else
                {
                    txt_CodMit_P.Text = string.Empty;
                    corr = new DocsPAWA.DocsPaWR.Corrispondente();
                    corr.descrizione = this.txt_DescMit_P.Text;
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
            if (!string.IsNullOrEmpty(this.txt_DescMit_P.Text))
            {
                Corrispondente corr = null;
                if (!string.IsNullOrEmpty(this.hiddenIdCodMit_p.Value))
                {
                    corr = UserManager.getCorrispondenteBySystemIDDisabled(this.Page, this.hiddenIdCodMit_p.Value);
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.txt_CodMit_P.Text))
                    {
                        corr = UserManager.getCorrispondenteByCodRubrica(this, this.txt_CodMit_P.Text);
                    }
                }

                if (corr != null && !string.IsNullOrEmpty(corr.codiceRubrica) && !string.IsNullOrEmpty(corr.descrizione))
                {
                    if (checkDuplicatiMittMultipli(corr, lbx_mittMultiplo))
                        ((ProtocolloEntrata)schedaDocumento.protocollo).mittenti = UserManager.addCorrispondente(((ProtocolloEntrata)schedaDocumento.protocollo).mittenti, corr);
                }
                else
                {
                    corr = new DocsPAWA.DocsPaWR.Corrispondente();
                    corr.descrizione = this.txt_DescMit_P.Text;
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
                txt_CodMit_P.Text = string.Empty;
                txt_DescMit_P.Text = string.Empty;
                this.hiddenIdCodMit_p.Value = string.Empty;
            }
        }

        private void btn_nascosto_mitt_multipli_Click(object sender, System.EventArgs e)
        {
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

        /// <summary>
        /// Inizializzazione controllo consolidamento
        /// </summary>
        protected virtual void InitializeControlConsolidation()
        {
            this.documentConsolidationCtrl.Consolidated += new DocumentConsolidatedDelegate(documentConsolidationCtrl_Consolidated);
        }

        /// <summary>
        /// Listener evento documento consolidato
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void documentConsolidationCtrl_Consolidated(object sender, DocumentConsolidatedEventArgs e)
        {
            this.ddl_tmpl.Items.Clear();
            this.caricaModelliTrasm();
        }

        protected void newRubricaVeloce(string tipo)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["RUBRICAVELOCE_MINIMUMPREFIXLENGTH"] != null)
            {
                mittente_veloce.MinimumPrefixLength = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RUBRICAVELOCE_MINIMUMPREFIXLENGTH"].ToString());
                destinatario_veloce.MinimumPrefixLength = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RUBRICAVELOCE_MINIMUMPREFIXLENGTH"].ToString());
            }

            string dataUser = null;
            DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
            System.Web.HttpContext ctx = System.Web.HttpContext.Current;
            if (ctx.Session["userRuolo"] != null)
            {
                dataUser = ((DocsPAWA.DocsPaWR.Ruolo)ctx.Session["userRuolo"]).systemId;
            }

            if (ctx.Session["userRegistro"] != null)
            {
                dataUser = dataUser + "-" + ((Registro)ctx.Session["userRegistro"]).systemId;
            }

            string idAmm = cr.idAmministrazione;
            string callType = null;
            string javascript = null;

            if (tipo.Equals("A"))
            {
                // Mittente su protocollo in ingresso
                callType = "CALLTYPE_PROTO_IN";
                mittente_veloce.ContextKey = dataUser + "-" + idAmm + "-" + callType;
            }

            if (tipo.Equals("P"))
            {
                // Mittente su protocollo in uscita
                callType = "CALLTYPE_PROTO_OUT_MITT";
                mittente_veloce.ContextKey = dataUser + "-" + idAmm + "-" + callType;

                // Destinatario su protocollo in uscita
                callType = "CALLTYPE_PROTO_OUT";
                destinatario_veloce.ContextKey = dataUser + "-" + idAmm + "-" + callType;
            }

            if (tipo.Equals("I"))
            {
                //Mittente su protocollo interno
                callType = "CALLTYPE_PROTO_INT_MITT";
                mittente_veloce.ContextKey = dataUser + "-" + idAmm + "-" + callType;

                // Destinatario su protocollo interno
                callType = "CALLTYPE_PROTO_INT_DEST";
                destinatario_veloce.ContextKey = dataUser + "-" + idAmm + "-" + callType;
            }
        }

        /// <summary>
        /// True se è attivo il riproponi con conoscenza
        /// </summary>
        public bool IsEnabledRiproponiConConoscenza
        {
            get
            {
                String riproponiConConoscenza = InitConfigurationKeys.GetValue("0", "FE_RIPROPONI_CON_CONOSCENZA");
                return riproponiConConoscenza == "1";
            }
        }

        protected bool FromInteropPecOrSimpInterop(SchedaDocumento schedaDocInterop)
        {
            bool result = false;

            if (schedaDocInterop != null && schedaDocInterop.interop != null && schedaDocInterop.interop.Equals("S") && schedaDocInterop.protocollo != null && schedaDocInterop.typeId != null &&
                (schedaDocInterop.typeId.Equals("INTEROPERABILITA") || schedaDocInterop.typeId.Equals(InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId)) && schedaDocInterop.tipoProto != null && schedaDocInterop.tipoProto.Equals("A"))
            {
                result = true;
            }

            return result;
        }

        protected bool FromInteropPecOrSimpInteropOrMail(SchedaDocumento schedaDocInterop)
        {
            bool result = false;

            if (schedaDocInterop != null && schedaDocInterop.interop != null && (schedaDocInterop.interop.Equals("S") || schedaDocInterop.interop.Equals("P") || schedaDocInterop.interop.Equals("E")) && schedaDocInterop.protocollo != null && schedaDocInterop.typeId != null &&
                (schedaDocInterop.typeId.Equals("INTEROPERABILITA") || schedaDocInterop.typeId.Equals(InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId) || schedaDocInterop.typeId.Equals("MAIL")) && schedaDocInterop.tipoProto != null && schedaDocInterop.tipoProto.Equals("A"))
            {
                result = true;
            }

            return result;
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

        private void spedisciUfficioReferente(bool isEnableUffReferente)
        {
            string serverName = Utils.getHttpFullPath(this);
            bool verificaRagioni;
            string message;

            if (DocumentManager.TrasmettiProtocolloUfficioReferente(this, serverName, this.schedaDocumento, isEnableUffReferente, out verificaRagioni, out message))
            {
                if (!string.IsNullOrEmpty(message))
                {
                    Response.Write("<script>" + message + "</script>");
                    message = "";
                }

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

        private void msg_rimuoviTipologia_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                string msg = ProfilazioneDocManager.RemoveTipologyDoc(UserManager.getInfoUtente(), schedaDocumento, this);
                if (!String.IsNullOrEmpty(msg))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "removeTipology", "alert('" + msg + "');", true);
                }
                else
                {
                    schedaDocumento.template = null;
                    schedaDocumento.tipologiaAtto = null;
                    DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                    DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                    ddl_tipoAtto.SelectedIndex = 0;
                }
            }
        }

        private void btn_delTipologyDoc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            msg_rimuoviTipologia.Confirm("Si conferma al rimozione della tipologia per questo documento ?");
        }

        /// <summary>
        /// Metodo per la costruzione del titolo della finestra di avviso mostrata quando viene protocollato 
        /// un documento ricevuto per IS marcato come privato dall'amministrazione mittente
        /// </summary>
        /// <returns>Titolo della finestra</returns>
        public String GetWindowTitle()
        {
            return appTitleProvider.PageTitle;
        }

    }
}
