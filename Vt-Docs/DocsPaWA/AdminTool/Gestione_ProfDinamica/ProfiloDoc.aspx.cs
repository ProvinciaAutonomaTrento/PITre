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
using System.IO;
using System.Linq;
using DocsPaWA.UserControls;


namespace DocsPAWA.AdminTool.Gestione_ProfDinamica
{
    public class ProfiloDoc : System.Web.UI.Page
    {
        #region Dichiarazioni-PageLoad
        protected System.Web.UI.WebControls.ImageButton Corrispondente;
        protected System.Web.UI.WebControls.Panel Panel_PersCorrispondente;
        protected System.Web.UI.WebControls.TextBox txt_etichettaCorr;
        protected System.Web.UI.WebControls.DropDownList ddl_ruoloPredefinito;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoRicerca;
        protected System.Web.UI.WebControls.CheckBox cb_Obbligatorio_Corr;
        protected System.Web.UI.WebControls.CheckBox cb_Ricerca_Corr;
        protected System.Web.UI.WebControls.Button btn_ConfermaPersCorrispondente;
        protected System.Web.UI.WebControls.ImageButton btn_up_7;


        protected System.Web.UI.WebControls.ImageButton btn_down_7;
        protected System.Web.UI.WebControls.Label lbl_position;
        protected System.Web.UI.WebControls.Label lbl_titolo;
        protected System.Web.UI.WebControls.Label lbl_avviso;
        protected System.Web.UI.WebControls.Label lblDataInizio;
        protected System.Web.UI.WebControls.Label lblDataFine;
        protected System.Web.UI.WebControls.TextBox txt_formatoContatore;
        protected System.Web.UI.WebControls.Panel Panel_Personalizzazione;
        protected System.Web.UI.WebControls.Panel Panel_PersContatore;
        protected System.Web.UI.WebControls.Button btn_ConfermaPersContatore;
        protected System.Web.UI.WebControls.TextBox txt_etichettaContatore;
        protected System.Web.UI.WebControls.Panel Panel_ListaComponenti;
        protected System.Web.UI.WebControls.DataGrid dg_listaComponenti;
        protected System.Web.UI.WebControls.ImageButton Contatore;
        protected System.Web.UI.WebControls.ImageButton CampoDiTesto;
        protected System.Web.UI.WebControls.ImageButton CasellaDiSelezione;
        protected System.Web.UI.WebControls.ImageButton MenuATendina;
        protected System.Web.UI.WebControls.ImageButton SelezioneEsclusiva;
        protected System.Web.UI.WebControls.ImageButton img_btnAddCampoContatore;
        protected System.Web.UI.WebControls.ImageButton img_btnDelCampoContatore;
        protected System.Web.UI.WebControls.Button btn_nuovoModello;
        protected System.Web.UI.WebControls.Panel Panel_ListaModelli;
        protected System.Web.UI.WebControls.Panel Panel_NuovoModello;
        protected System.Web.UI.WebControls.Panel Panel_PersCampoDiTesto;
        protected System.Web.UI.WebControls.Panel Panel_PersCasellaDiSelezione;
        protected System.Web.UI.WebControls.Panel Panel_PersMenuATendina;
        protected System.Web.UI.WebControls.Panel Panel_PersSelezioneEsclusiva;
        protected System.Web.UI.WebControls.Button btn_ConfermaPersCampoDiTesto;
        protected System.Web.UI.WebControls.Button btn_ConfermaPersCasellaDiSelezione;
        protected System.Web.UI.WebControls.TextBox txt_etichettaCampoDiTesto;
        protected System.Web.UI.WebControls.TextBox txt_TestoDefaultCampoTesto;
        protected System.Web.UI.WebControls.TextBox txt_etichettaCasellaDiSelezione;
        protected System.Web.UI.WebControls.TextBox DataInizio;
        protected System.Web.UI.WebControls.TextBox DataFine;
        protected System.Web.UI.WebControls.Button btn_modelli;
        protected System.Web.UI.WebControls.Button btn_anteprima;
        protected System.Web.UI.WebControls.Button btn_salvaTemplate;
        protected System.Web.UI.WebControls.Button btn_CampiComuni;
        protected System.Web.UI.WebControls.Button btn_ContestoProcedurale;
        protected System.Web.UI.WebControls.TextBox txt_etichettaSelezioneEsclusiva;
        protected System.Web.UI.WebControls.DropDownList ddl_valoriSelezioneEsclusiva;
        protected System.Web.UI.WebControls.DropDownList ddl_separatore;
        protected System.Web.UI.WebControls.DropDownList ddl_campiContatore;
        protected System.Web.UI.WebControls.TextBox txt_valoreSelezioneEsclusiva;
        protected System.Web.UI.WebControls.Button btn_ConfermaPersSelezioneEsclusiva;
        protected System.Web.UI.WebControls.Button btn_aggiungiValoreSelezioneEsclusiva;
        protected System.Web.UI.WebControls.Button btn_elimnaValoreSelezioneEsclusiva;
        protected System.Web.UI.WebControls.TextBox txt_etichettaMenuATendina;
        protected System.Web.UI.WebControls.TextBox txt_valoreMenuATendina;
        protected System.Web.UI.WebControls.DropDownList ddl_valoriMenuATendina;
        protected System.Web.UI.WebControls.Button btn_aggiungiValoreMenuATendina;
        protected System.Web.UI.WebControls.Button btn_eliminaValoreMenuATendina;
        protected System.Web.UI.WebControls.Button btn_ConfermaPersMenuATendina;
        protected System.Web.UI.WebControls.Button btn_isInstance;
        protected System.Web.UI.WebControls.TextBox txt_TipoDocumento;
        protected System.Web.UI.WebControls.CheckBox cb_Obbligatorio_CampoDiTesto;
        protected System.Web.UI.WebControls.CheckBox cb_Ricerca_CampoDiTesto;
        protected System.Web.UI.WebControls.CheckBox cb_Multilinea_CampoDiTesto;
        protected System.Web.UI.WebControls.CheckBox cb_Azzera_Anno;
        protected System.Web.UI.WebControls.TextBox txt_NumeroCaratteri_CampoDiTesto;
        protected System.Web.UI.WebControls.TextBox txt_NumeroLinee_CampoDiTesto;
        protected System.Web.UI.WebControls.CheckBox cb_Obbligatorio_CasellaDiSelezione;
        protected System.Web.UI.WebControls.CheckBox cb_Ricerca_CasellaDiSelezione;
        protected System.Web.UI.WebControls.CheckBox cb_Ricerca_Contatore;
        protected System.Web.UI.WebControls.CheckBox cb_Obbligatorio_MenuATendina;
        protected System.Web.UI.WebControls.CheckBox cb_Ricerca_MenuATendina;
        protected System.Web.UI.WebControls.CheckBox cb_Default_MenuATendina;
        protected System.Web.UI.WebControls.DropDownList DdlColoriMenu;
        protected System.Web.UI.WebControls.DropDownList DdlColoriSelezioneEsclusiva;
        protected System.Web.UI.WebControls.CheckBox cb_Default_SelezioneEsclusiva;
        protected System.Web.UI.WebControls.CheckBox cb_Obbligatorio_SelezioneEsclusiva;
        protected System.Web.UI.WebControls.CheckBox cb_Ricerca_SelezioneEsclusiva;
        protected System.Web.UI.WebControls.RadioButtonList rd_VerOri_SelezioneEsclusiva;
        protected System.Web.UI.WebControls.ImageButton Data;
        protected System.Web.UI.WebControls.Panel Panel_PersData;
        protected System.Web.UI.WebControls.TextBox txt_etichettaData;
        protected System.Web.UI.WebControls.CheckBox cb_Obbligatorio_Data;
        protected System.Web.UI.WebControls.CheckBox cb_Ricerca_Data;
        protected System.Web.UI.WebControls.Button btn_ConfermaPersData;
        protected System.Web.UI.WebControls.DataGrid dg_listaTemplates;
        protected static DataTable dt_listaComponenti = new DataTable();
        protected static DataTable dt_listaTemplates = new DataTable();
        protected System.Web.UI.WebControls.TextBox txt_MesiConservazione;
        protected System.Web.UI.WebControls.CheckBox cb_InvioConservazione;
        //private ArrayList tipiDocumento;
        protected System.Web.UI.WebControls.ImageButton pulsante_chiudi_pers;
        protected System.Web.UI.WebControls.Panel Panel_Dg_ListaComponenti;
        protected System.Web.UI.HtmlControls.HtmlGenericControl DivDGListaTemplates;
        protected System.Web.UI.HtmlControls.HtmlGenericControl DivDGListaComponenti;
        protected System.Web.UI.WebControls.Button btn_inEsercizio;
        private ArrayList listaTemplates;
        private UserControl.ScrollKeeper scrollKeeperListaComponenti = new UserControl.ScrollKeeper();
        protected System.Web.UI.WebControls.ImageButton btn_up_1;
        protected System.Web.UI.WebControls.ImageButton btn_down_1;
        protected System.Web.UI.WebControls.ImageButton btn_up_2;
        protected System.Web.UI.WebControls.ImageButton btn_down_2;
        protected System.Web.UI.WebControls.ImageButton btn_up_3;
        protected System.Web.UI.WebControls.ImageButton btn_down_3;
        protected System.Web.UI.WebControls.ImageButton btn_up_4;
        protected System.Web.UI.WebControls.ImageButton btn_down_4;
        protected System.Web.UI.WebControls.ImageButton btn_up_5;
        protected System.Web.UI.WebControls.ImageButton btn_down_5;
        protected System.Web.UI.WebControls.ImageButton btn_up_6;
        protected System.Web.UI.WebControls.ImageButton btn_down_6;
        protected System.Web.UI.WebControls.DropDownList ddl_valoriCasellaSelezione;
        protected System.Web.UI.WebControls.CheckBox cb_Default_CasellaSelezione;
        protected System.Web.UI.WebControls.RadioButtonList rd_VerOri_CasellaSelezione;
        protected System.Web.UI.WebControls.RadioButtonList RadioButtonContatore;
        protected System.Web.UI.WebControls.Button btn_aggiungiValoreCasellaSelezione;
        protected System.Web.UI.WebControls.Button btn_eliminaValoreCasellaSelezione;
        protected System.Web.UI.WebControls.TextBox txt_valoreCasellaSelezione;
        protected System.Web.UI.WebControls.Panel Panel_Diagrammi_Trasmissioni;
        protected System.Web.UI.WebControls.ImageButton pulsante_chiudi_diagr_trasm;
        protected System.Web.UI.WebControls.Label lbl_tipoDoc;
        protected System.Web.UI.WebControls.Button btn_confermaDiagrTrasm;
        protected System.Web.UI.WebControls.DropDownList ddl_Diagrammi;
        private UserControl.ScrollKeeper scrollKeeperListaTemplate = new UserControl.ScrollKeeper();
        protected System.Web.UI.WebControls.Button btn_modelliTrasm;
        protected System.Web.UI.WebControls.DataGrid dg_Stati;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txt_confirmDel;
        protected System.Web.UI.WebControls.Button btn_cambiaDiag;
        protected System.Web.UI.WebControls.Label lbl_nameTypeDoc;
        private string idAmministrazione;
        protected System.Web.UI.WebControls.TextBox txt_scadenza;
        protected System.Web.UI.WebControls.TextBox txt_preScadenza;
        protected System.Web.UI.WebControls.RadioButtonList rbl_tipoContatore;
        protected System.Web.UI.WebControls.CheckBox cb_ContaDopo;
        protected System.Web.UI.WebControls.CheckBox cb_Repertorio;
        //Veronica per gestione tipologia documento privato
        protected System.Web.UI.WebControls.Panel Panel_Privato;
        protected System.Web.UI.WebControls.Button btn_confermaPrivato;
        protected System.Web.UI.WebControls.CheckBox cb_Privato;
        protected System.Web.UI.WebControls.CheckBox cb_ModPrivato;
        protected System.Web.UI.WebControls.ImageButton pulsante_chiudi_privato;
        protected System.Web.UI.WebControls.Label lbl_TipoDocPr;
        protected System.Web.UI.WebControls.CheckBox cb_Disabilitato_MenuATendina;
        protected System.Web.UI.WebControls.CheckBox cb_Disabilitato_CasellaSelezione;
        protected System.Web.UI.WebControls.CheckBox cb_Disabilitato_SelezioneEsclusiva;
        protected Utilities.MessageBox msg_Privato;
        protected DocsPAWA.DocsPaWR.InfoUtenteAmministratore datiAmministratore;
        //protected System.Web.UI.WebControls.ImageButton btn_associaRuolo;
        protected System.Web.UI.WebControls.HiddenField hiddenIdOggetto;
        protected bool daVerificare = true;
        protected System.Web.UI.WebControls.CheckBox cb_Contatore_visibile;
        protected System.Web.UI.WebControls.DropDownList ddl_formatoOra;
        //Gestione mesi conservazione
        protected System.Web.UI.WebControls.Label LblTipoDocMesiCons;
        protected System.Web.UI.WebControls.Panel Panel_MesiCons;
        protected System.Web.UI.WebControls.Button btn_confermaMesiCons;
        protected System.Web.UI.WebControls.ImageButton pulsante_chiudi_MesiCons;
        protected System.Web.UI.WebControls.TextBox txt_ModMesiCons;
        //Gestione invio in conservazione
        protected System.Web.UI.WebControls.Label LblTipoDocInvioCons;
        protected System.Web.UI.WebControls.Panel Panel_InvioCons;
        protected System.Web.UI.WebControls.Button btn_confermaInvioCons;
        protected System.Web.UI.WebControls.ImageButton pulsante_chiudi_InvioCons;
        protected System.Web.UI.WebControls.CheckBox cb_ModInvioCons;
        //Gestione consolidamento/conservazione
        protected System.Web.UI.WebControls.Panel Panel_ConsolidaOgg;
        protected System.Web.UI.WebControls.Label LblOggConsolidamento;
        protected System.Web.UI.WebControls.ImageButton pulsante_chiudi_OggConsol;
        protected System.Web.UI.WebControls.CheckBox cb_ModConsolidaOgg;
        protected System.Web.UI.WebControls.Button btn_confermaConsolidaOgg;
        protected System.Web.UI.WebControls.CheckBox cb_consRep;
        protected System.Web.UI.WebControls.Label lbl_consRep;

        protected System.Web.UI.WebControls.Panel Panel_ConservaOgg;
        protected System.Web.UI.WebControls.Label LblOggConservazione;
        protected System.Web.UI.WebControls.ImageButton pulsante_chiudi_OggConserv;
        protected System.Web.UI.WebControls.CheckBox cb_ModConservaOgg;
        protected System.Web.UI.WebControls.Button btn_confermaConservaOgg;

        //protected Utilities.MessageBox msg_MesiCons;
        //gestione Link
        protected System.Web.UI.WebControls.ImageButton Link;
        protected System.Web.UI.WebControls.Panel Panel_PersLink;
        protected System.Web.UI.WebControls.ImageButton btn_up_8;
        protected System.Web.UI.WebControls.ImageButton btn_down_8;
        protected System.Web.UI.WebControls.Button btn_ConfermaPersLink;
        protected System.Web.UI.WebControls.TextBox txt_etichettaLink;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoLink;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoObjLink;
        protected System.Web.UI.WebControls.CheckBox cb_obbligatorioLink;
        //ContatoreSottocontatore
        protected System.Web.UI.WebControls.ImageButton eliminaDataInizio;
        protected System.Web.UI.WebControls.ImageButton eliminaDataFine;
        protected System.Web.UI.WebControls.ImageButton ContatoreSottocontatore;
        protected System.Web.UI.WebControls.Panel Panel_PersContSottocontatore;
        protected System.Web.UI.WebControls.TextBox txt_etichettaContSottocontatore;
        protected System.Web.UI.WebControls.DropDownList ddl_separatoreSottocontatore;
        protected System.Web.UI.WebControls.CheckBox cb_Ricerca_ContSottocontatore;
        protected System.Web.UI.WebControls.DropDownList ddl_campiContSottocontatore;
        protected System.Web.UI.WebControls.ImageButton img_btnAddCampoContSottocontatore;
        protected System.Web.UI.WebControls.ImageButton img_btnDelCampoContSottocontatore;
        protected System.Web.UI.WebControls.CheckBox cb_Azzera_AnnoContSottocontatore;
        protected System.Web.UI.WebControls.Label lbl_formatoContSottocontatore;
        protected System.Web.UI.WebControls.RadioButtonList rbl_tipoContSottocontatore;
        protected System.Web.UI.WebControls.CheckBox cb_ContaDopoContSottocontatore;
        protected System.Web.UI.WebControls.Button btn_ConfermaPersContSottocontatore;
        protected System.Web.UI.WebControls.ImageButton btn_up_9;
        protected System.Web.UI.WebControls.ImageButton btn_down_9;
        protected System.Web.UI.WebControls.TextBox txt_moduloSottoconotatore;
        //Separatore
        protected System.Web.UI.WebControls.ImageButton Separatore;
        protected System.Web.UI.WebControls.Panel Panel_PersSeparatore;
        protected System.Web.UI.WebControls.TextBox txt_etichettaSeparatore;

        protected System.Web.UI.WebControls.Button btn_ConfermaPersSeparatore;
        protected System.Web.UI.WebControls.ImageButton btn_up_10;
        protected System.Web.UI.WebControls.ImageButton btn_down_10;

        //gestione Oggetto Esterno
        protected System.Web.UI.WebControls.ImageButton OggettoEsterno;
        protected System.Web.UI.WebControls.Panel Panel_PersOggEsterno;
        protected System.Web.UI.WebControls.TextBox txt_etichettaOggEsterno;
        protected CheckBox cb_obbligatorioOggEsterno;
        protected CheckBox cb_ricercaOggEsterno;
        protected IntegrationAdapter intAdapter_OggEsterno;
        protected Button btn_ConfermaPersOggEsterno;
        protected ImageButton btn_up_OggEsterno;
        protected ImageButton btn_down_OggEsterno;

        protected System.Web.UI.WebControls.Button btnStorico;

        protected System.Web.UI.WebControls.Button btn_toExtSys;
        protected System.Web.UI.HtmlControls.HtmlTableRow tr_backToExtSys;

        protected Button btn_GestioneContatoreSottocontatore;
        protected Label lbl_repertorio;
        protected Label LblColoreSelezioneEsclusiva;
        protected Label LblColoreMenu;

        private void Page_Load(object sender, System.EventArgs e)
        {
            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
            Session["AdminBookmark"] = "GestioneTipiDoc";
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

            btn_anteprima.Attributes.Add("onclick", "javascript: apriPopupAnteprima();");

            if (Session["AMMDATASET"] == null)
            {
                RegisterStartupScript("NoProfilazione", "<script>alert('Attenzione selezionare un\\'amministrazione !'); document.location = '../Gestione_Homepage/Home.aspx';</script>");
                return;
            }
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            idAmministrazione = Utils.getIdAmmByCod(codiceAmministrazione, this);

            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "0")
                dg_listaTemplates.Columns[1].Visible = false;

            if (System.Configuration.ConfigurationManager.AppSettings["MODELLO_DOCUMENTO"] != null && System.Configuration.ConfigurationManager.AppSettings["MODELLO_DOCUMENTO"] == "0")
                dg_listaTemplates.Columns[3].Visible = false;

            if (!IsPostBack)
            {
                popolaTemplateDG();
                lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
                //Aggiunta Stefano Limiti
                //per avviare lo script che non permette l'inserimento dei caratteri alfabetici nella 
                //textbox dei mesi dei conservazione del template
                this.AddControlsClientAttribute();
                if (string.IsNullOrEmpty(Request.QueryString["extsysconf"]))
                    this.tr_backToExtSys.Visible = false;
            }


            impostaImmaginiModelli();

            //Verifico se provengo da una selezione di campi comuni
            if (Session["selezioneCampiComuni"] != null)
            {
                Session.Remove("selezioneCampiComuni");
                Panel_NuovoModello.Visible = false;
                Panel_Personalizzazione.Visible = false;
                Panel_Privato.Visible = false;
                Panel_Diagrammi_Trasmissioni.Visible = false;
                Panel_MesiCons.Visible = false;
                aggiungiComponenteDG();
            }

            UserControl.ScrollKeeper skDgTemplate = new UserControl.ScrollKeeper();
            skDgTemplate.WebControl = "DivDGListaTemplates";
            this.Form.Controls.Add(skDgTemplate);

            UserControl.ScrollKeeper skDgOggTemplate = new UserControl.ScrollKeeper();
            skDgOggTemplate.WebControl = "DivDGListaComponenti";
            this.Form.Controls.Add(skDgOggTemplate);

            this.btnStorico.Enabled = Session["template"] != null && ((DocsPAWA.DocsPaWR.Templates)Session["template"]).SYSTEM_ID > 0 && ((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI.Length > 0;

            // INTEGRAZIONE PITRE-PARER
            if (!this.IsConservazionePARER())
            {
                foreach (DataGridColumn col in dg_listaTemplates.Columns)
                {
                    if (col.HeaderText != null && col.HeaderText.Equals("Invio Cons."))
                    {
                        col.Visible = false;
                    }
                }
                foreach (DataGridColumn col in dg_listaComponenti.Columns)
                {
                    if (col.HeaderText != null && (col.HeaderText.Equals("Consolidamento") || col.HeaderText.Equals("Conservazione")))
                    {
                        col.Visible = false;
                    }
                }

            }


        }
        #endregion Dichiarazioni-PageLoad


        private void AddControlsClientAttribute()
        {
            this.txt_MesiConservazione.Attributes.Add("onKeyPress", "ValidateNumericKey();");
            this.txt_ModMesiCons.Attributes.Add("onKeyPress", "ValidateNumericKey();");
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.btn_inEsercizio.Click += new System.EventHandler(this.btn_inEsercizio_Click);
            this.btn_salvaTemplate.Click += new System.EventHandler(this.btn_salvaTemplate_Click);
            this.btn_modelli.Click += new System.EventHandler(this.btn_modelli_Click);
            this.btn_nuovoModello.Click += new System.EventHandler(this.btn_nuovoModello_Click);
            this.dg_listaTemplates.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_listaTemplates_ItemCommand);
            this.dg_listaTemplates.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_listaTemplates_DeleteCommand);
            this.pulsante_chiudi_diagr_trasm.Click += new System.Web.UI.ImageClickEventHandler(this.pulsante_chiudi_diagr_trasm_Click);
            this.ddl_Diagrammi.SelectedIndexChanged += new System.EventHandler(this.ddl_Diagrammi_SelectedIndexChanged);
            this.btn_cambiaDiag.Click += new System.EventHandler(this.btn_cambiaDiag_Click);
            this.btn_modelliTrasm.Click += new System.EventHandler(this.btn_modelliTrasm_Click);
            this.btn_confermaDiagrTrasm.Click += new System.EventHandler(this.btn_confermaDiagrTrasm_Click);
            this.dg_Stati.SelectedIndexChanged += new System.EventHandler(this.dg_Stati_SelectedIndexChanged);
            this.CampoDiTesto.Click += new System.Web.UI.ImageClickEventHandler(this.btn_AggiungiOggetto_Click);
            this.CasellaDiSelezione.Click += new System.Web.UI.ImageClickEventHandler(this.btn_AggiungiOggetto_Click);
            this.MenuATendina.Click += new System.Web.UI.ImageClickEventHandler(this.btn_AggiungiOggetto_Click);
            this.SelezioneEsclusiva.Click += new System.Web.UI.ImageClickEventHandler(this.btn_AggiungiOggetto_Click);
            this.Contatore.Click += new System.Web.UI.ImageClickEventHandler(this.btn_AggiungiOggetto_Click);
            this.Data.Click += new System.Web.UI.ImageClickEventHandler(this.btn_AggiungiOggetto_Click);
            this.Corrispondente.Click += new System.Web.UI.ImageClickEventHandler(this.btn_AggiungiOggetto_Click);
            this.dg_listaComponenti.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_listaComponenti_DeleteCommand);
            this.dg_listaComponenti.SelectedIndexChanged += new System.EventHandler(this.dg_listaComponenti_SelectedIndexChanged);
            this.dg_listaComponenti.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_listaComponenti_ItemCommand);
            this.pulsante_chiudi_pers.Click += new System.Web.UI.ImageClickEventHandler(this.pulsante_chiudi_pers_Click);
            this.btn_ConfermaPersCampoDiTesto.Click += new System.EventHandler(this.btn_ConfermaPers_Click);
            this.btn_up_1.Click += new System.Web.UI.ImageClickEventHandler(this.btn_up_1_Click);
            this.btn_down_1.Click += new System.Web.UI.ImageClickEventHandler(this.btn_down_1_Click);
            this.ddl_valoriCasellaSelezione.SelectedIndexChanged += new System.EventHandler(this.ddl_valoriCasellaSelezione_SelectedIndexChanged);
            this.btn_ConfermaPersCasellaDiSelezione.Click += new System.EventHandler(this.btn_ConfermaPers_Click);
            this.btn_aggiungiValoreCasellaSelezione.Click += new System.EventHandler(this.btn_aggiungiValoreCasellaSelezione_Click);
            this.btn_eliminaValoreCasellaSelezione.Click += new System.EventHandler(this.btn_eliminaValoreCasellaSelezione_Click);
            this.btn_up_2.Click += new System.Web.UI.ImageClickEventHandler(this.btn_up_1_Click);
            this.btn_down_2.Click += new System.Web.UI.ImageClickEventHandler(this.btn_down_1_Click);
            this.ddl_valoriSelezioneEsclusiva.SelectedIndexChanged += new System.EventHandler(this.ddl_valoriSelezioneEsclusiva_SelectedIndexChanged);
            this.btn_ConfermaPersSelezioneEsclusiva.Click += new System.EventHandler(this.btn_ConfermaPers_Click);
            this.btn_aggiungiValoreSelezioneEsclusiva.Click += new System.EventHandler(this.btn_aggiungiValoreSelezioneEsclusiva_Click);
            this.btn_elimnaValoreSelezioneEsclusiva.Click += new System.EventHandler(this.btn_elimnaValoreSelezioneEsclusiva_Click);
            this.btn_up_3.Click += new System.Web.UI.ImageClickEventHandler(this.btn_up_1_Click);
            this.btn_down_3.Click += new System.Web.UI.ImageClickEventHandler(this.btn_down_1_Click);
            this.btn_ConfermaPersContatore.Click += new System.EventHandler(this.btn_ConfermaPers_Click);
            this.btn_up_4.Click += new System.Web.UI.ImageClickEventHandler(this.btn_up_1_Click);
            this.btn_down_4.Click += new System.Web.UI.ImageClickEventHandler(this.btn_down_1_Click);
            this.ddl_valoriMenuATendina.SelectedIndexChanged += new System.EventHandler(this.ddl_valoriMenuATendina_SelectedIndexChanged);
            this.btn_ConfermaPersMenuATendina.Click += new System.EventHandler(this.btn_ConfermaPers_Click);
            this.btn_aggiungiValoreMenuATendina.Click += new System.EventHandler(this.btn_aggiungiValoreMenuATendina_Click);
            this.btn_eliminaValoreMenuATendina.Click += new System.EventHandler(this.btn_eliminaValoreMenuATendina_Click);
            this.btn_up_5.Click += new System.Web.UI.ImageClickEventHandler(this.btn_up_1_Click);
            this.btn_down_5.Click += new System.Web.UI.ImageClickEventHandler(this.btn_down_1_Click);
            this.btn_ConfermaPersData.Click += new System.EventHandler(this.btn_ConfermaPers_Click);
            this.btn_up_6.Click += new System.Web.UI.ImageClickEventHandler(this.btn_up_1_Click);
            this.btn_down_6.Click += new System.Web.UI.ImageClickEventHandler(this.btn_down_1_Click);
            this.btn_up_7.Click += new System.Web.UI.ImageClickEventHandler(this.btn_up_1_Click);
            this.btn_down_7.Click += new System.Web.UI.ImageClickEventHandler(this.btn_down_1_Click);
            this.btn_ConfermaPersCorrispondente.Click += new System.EventHandler(this.btn_ConfermaPers_Click);
            //Veronica per privato
            this.msg_Privato.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_Privato_GetMessageBoxResponse);
            this.pulsante_chiudi_privato.Click += new System.Web.UI.ImageClickEventHandler(this.pulsante_chiudi_privato_Click);
            this.btn_confermaPrivato.Click += new System.EventHandler(this.btn_confermaPrivato_Click);
            //eventi dei button per la gestione dei mesi conservazione
            this.pulsante_chiudi_MesiCons.Click += new System.Web.UI.ImageClickEventHandler(this.pulsante_chiudi_MesiCons_Click);
            this.btn_confermaMesiCons.Click += new System.EventHandler(this.btn_confermaMesiCons_Click);
            // eventi dei button per la gestione dell'invio in conservazione
            this.pulsante_chiudi_InvioCons.Click += new System.Web.UI.ImageClickEventHandler(this.pulsante_chiudi_InvioCons_Click);
            this.btn_confermaInvioCons.Click += new System.EventHandler(this.btn_confermaInvioCons_Click);
            // eventi per consolidamento/conservazione 
            this.pulsante_chiudi_OggConsol.Click += new System.Web.UI.ImageClickEventHandler(this.pulsante_chiudi_OggConsol_Click);
            this.btn_confermaConsolidaOgg.Click += new System.EventHandler(this.btn_confermaConsolidaOgg_Click);
            this.pulsante_chiudi_OggConserv.Click += new System.Web.UI.ImageClickEventHandler(this.pulsante_chiudi_OggConserv_Click);
            this.btn_confermaConservaOgg.Click += new System.EventHandler(this.btn_confermaConservaOgg_Click);

            //this.msg_MesiCons.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_MesiCons_GetMessageBoxResponse);
            //gestione Link
            this.Link.Click += new System.Web.UI.ImageClickEventHandler(this.btn_AggiungiOggetto_Click);
            this.btn_up_8.Click += new System.Web.UI.ImageClickEventHandler(this.btn_up_1_Click);
            this.btn_down_8.Click += new System.Web.UI.ImageClickEventHandler(this.btn_down_1_Click);
            this.btn_ConfermaPersLink.Click += new System.EventHandler(this.btn_ConfermaPers_Click);
            this.ddl_tipoLink.SelectedIndexChanged += new System.EventHandler(this.ddl_tipoLink_SelectedIndexChanged);
            //this.btn_associaRuolo.Click += new ImageClickEventHandler(btn_associaRuolo_Click);
            //this.Page.PreRender += new EventHandler(Page_PreRender);
            //this.cb_ContaDopo.CheckedChanged += new EventHandler(cb_ContaDopo_CheckedChanged);

            //ContatoreSottocontatore
            this.ContatoreSottocontatore.Click += new System.Web.UI.ImageClickEventHandler(this.btn_AggiungiOggetto_Click);
            this.btn_ConfermaPersContSottocontatore.Click += new System.EventHandler(this.btn_ConfermaPers_Click);
            this.btn_up_9.Click += new System.Web.UI.ImageClickEventHandler(this.btn_up_1_Click);
            this.btn_down_9.Click += new System.Web.UI.ImageClickEventHandler(this.btn_down_1_Click);

            //Separatore
            this.Separatore.Click += new System.Web.UI.ImageClickEventHandler(this.btn_AggiungiOggetto_Click);
            this.btn_ConfermaPersSeparatore.Click += new System.EventHandler(this.btn_ConfermaPers_Click);
            this.btn_up_10.Click += new System.Web.UI.ImageClickEventHandler(this.btn_up_1_Click);
            this.btn_down_10.Click += new System.Web.UI.ImageClickEventHandler(this.btn_down_1_Click);

            //gestione Oggetto Esterno
            this.OggettoEsterno.Click += new System.Web.UI.ImageClickEventHandler(this.btn_AggiungiOggetto_Click);
            this.btn_up_OggEsterno.Click += new System.Web.UI.ImageClickEventHandler(this.btn_up_1_Click);
            this.btn_down_OggEsterno.Click += new System.Web.UI.ImageClickEventHandler(this.btn_down_1_Click);
            this.btn_ConfermaPersOggEsterno.Click += new System.EventHandler(this.btn_ConfermaPers_Click);
            this.Load += new System.EventHandler(this.Page_Load);
        }

        /*
        void cb_ContaDopo_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cb_ContaDopo.Checked)
                this.btn_associaRuolo.Visible = true;
            else
                this.btn_associaRuolo.Visible = false;

            daVerificare = false;
        }

        void btn_associaRuolo_Click(object sender, ImageClickEventArgs e)
        {
            daVerificare = false;
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
            DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[this.dg_listaComponenti.SelectedIndex];
            if (template != null && template.SYSTEM_ID > 0 && oggettoCustom != null && oggettoCustom.SYSTEM_ID > 0)
                RegisterStartupScript("apriPopupVisibilita", "<script>apriPopupAssociaRuolo(" + this.hiddenIdOggetto.Value + ");</script>");
            else
                RegisterStartupScript("alertSalvataggio", "<script>alert('Prima di associare la visibilità al ruolo, è necessario salvare il template');</script>");
        }

        private void Page_PreRender(object sender, EventArgs e)
        {
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];

            if (this.dg_listaComponenti.SelectedIndex != -1 && daVerificare && template != null)
            {
                DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[this.dg_listaComponenti.SelectedIndex];

                if (oggettoCustom != null && oggettoCustom.SYSTEM_ID != 0)
                {
                    if (!ProfilazioneDocManager.verificaAssociazioneRuoloOggettoCustom(oggettoCustom.SYSTEM_ID.ToString(), "", "INSERIMENTO", this))
                    {
                        this.cb_ContaDopo.Checked = false;
                        this.btn_associaRuolo.Visible = false;
                    }
                    else
                    {
                        this.cb_ContaDopo.Checked = true;
                        this.btn_associaRuolo.Visible = true;
                    }
                }
            }


        }
        */

        #endregion

        #region DataGrid Lista Componenti
        private void aggiungiComponenteDG()
        {
            //Vengono aggiunte le righe, che rappresentano i componenti, al DataGrid che contiene, l'eleco
            //di questi ultimi. Per la definizione del DataSource del DataGrid, è stato creato un DataTable
            //contente una colonna Tipo di componente.
            dt_listaComponenti = new DataTable();
            dt_listaComponenti.Columns.Add("Ordinamento");
            dt_listaComponenti.Columns.Add("Tipo");
            dt_listaComponenti.Columns.Add("Etichetta");
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
            for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                DataRow dr = dt_listaComponenti.NewRow();
                dr["Ordinamento"] = oggettoCustom.POSIZIONE;
                dr["Tipo"] = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                dr["Etichetta"] = oggettoCustom.DESCRIZIONE;
                dt_listaComponenti.Rows.Add(dr);


            }
            dg_listaComponenti.DataSource = dt_listaComponenti;
            dg_listaComponenti.DataBind();
            dg_listaComponenti.SelectedIndex = dg_listaComponenti.Items.Count - 1;

            for (int i = 0; i < dg_listaComponenti.Items.Count; i++)
            {
                if (dg_listaComponenti.Items[i].Cells[0].Text == "&nbsp;")
                    dg_listaComponenti.Items[i].Visible = false;
            }

            // INTEGRAZIONE PITRE-PARER
            // campi CONSOLIDAMENTO e CONSERVAZIONE
            for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                
                Label lbl_consolida = ((Label)dg_listaComponenti.Items[i].Cells[3].Controls[3]);
                if (!string.IsNullOrEmpty(oggettoCustom.CONSOLIDAMENTO) && oggettoCustom.CONSOLIDAMENTO.Equals("1"))
                    lbl_consolida.Text = "SI";
                else
                    lbl_consolida.Text = "NO";

                Label lbl_conserva = ((Label)dg_listaComponenti.Items[i].Cells[4].Controls[3]);
                if (!string.IsNullOrEmpty(oggettoCustom.CONSERVAZIONE) && oggettoCustom.CONSERVAZIONE.Equals("1"))
                    lbl_conserva.Text = "SI";
                else
                    lbl_conserva.Text = "NO";
            }

            ////Imposto le differenze grafiche e funzionali per i campi comuni
            if (template.IPER_FASC_DOC != "1")
            {
                for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
                {
                    if (((DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i]).CAMPO_COMUNE == "1")
                    {
                        dg_listaComponenti.Items[i].Height = 24;
                        dg_listaComponenti.Items[i].Cells[5].Controls[0].Visible = false;
                        dg_listaComponenti.Items[i].Cells[6].Controls[0].Visible = false;
                        dg_listaComponenti.Items[i].BackColor = System.Drawing.Color.Gray;
                    }
                }
            }

            dg_listaComponenti.Visible = true;
            Panel_Dg_ListaComponenti.Visible = true;
            ridimensionaDiv(dg_listaComponenti.Items.Count, "DivDGListaComponenti");


        }

        private void dg_listaComponenti_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //Metodo invocato quando si cambia la selezione di una riga del DataGrid dei componenti.
            //Di conseguenza viene visualizzato il pannello di personalizzazione del componente
            //selezionato e popolati i suoi campi.
            int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
            if (oggettoSelezionato != -1)
            {
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[oggettoSelezionato];
                DocsPAWA.DocsPaWR.TipoOggetto tipoOggetto = (DocsPAWA.DocsPaWR.TipoOggetto)oggettoCustom.TIPO;
                if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "Contatore")
                {
                    if (!string.IsNullOrEmpty(oggettoCustom.DATA_INIZIO.ToString()) && !string.IsNullOrEmpty(oggettoCustom.DATA_FINE.ToString()))
                    {
                        RadioButtonContatore.Items[0].Enabled = false;
                        RadioButtonContatore.Items[1].Selected = true;
                        RadioButtonContatore.Items[1].Enabled = true;
                        RadioButtonContatore.Items[0].Selected = false;
                        DataInizio.Visible = true;
                        DataFine.Visible = true;
                        cb_Azzera_Anno.Checked = true;
                        cb_Azzera_Anno.Enabled = false;
                        lblDataInizio.Visible = true;
                        lblDataFine.Visible = true;
                        DataInizio.Text = (oggettoCustom.DATA_INIZIO.ToString()).Substring(0, 10);
                        DataFine.Text = (oggettoCustom.DATA_FINE.ToString()).Substring(0, 10);
                        cb_Repertorio.Enabled = false;
                        cb_consRep.Enabled = false;
                    }

                    else
                        if (string.IsNullOrEmpty(oggettoCustom.DATA_INIZIO.ToString()) && string.IsNullOrEmpty(oggettoCustom.DATA_FINE.ToString()))
                        {
                            RadioButtonContatore.Items[1].Enabled = false;
                            RadioButtonContatore.Items[1].Selected = false;
                            RadioButtonContatore.Items[0].Selected = true;
                            RadioButtonContatore.Items[0].Enabled = true;
                            lblDataInizio.Visible = false;
                            lblDataFine.Visible = false;
                            cb_Azzera_Anno.Enabled = true;
                            cb_Azzera_Anno.Checked = false;
                            DataFine.Text = string.Empty;
                            DataInizio.Text = string.Empty;
                            DataFine.Visible = false;
                            DataInizio.Visible = false;
                            cb_Repertorio.Enabled = true;
                            cb_consRep.Enabled = cb_Repertorio.Checked ? true : false;
                        }

                }
                this.hiddenIdOggetto.Value = oggettoCustom.SYSTEM_ID.ToString();

                visualizzaPannelloPersonalizzazzione(tipoOggetto.DESCRIZIONE_TIPO, oggettoCustom);
                // nascondo i panel relativi a consolidamento/conservazione
                Panel_ConsolidaOgg.Visible = false;
                Panel_ConservaOgg.Visible = false;
            }
            lbl_avviso.Visible = false;
        }

        private void dg_listaComponenti_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_confirmDel")).Value == "si")
            {
                ((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_confirmDel")).Value = "";
                //Metodo invocato quando si desidera rimuovere un componente dal DataGrid contenente la lista di quest'ultimi.
                //Di conseguenza la rimozione viene anche effettuata dal template in sessione.

                //Verifico che se si elimina un campo comune quest'ultimo non deve essere associato a nessuna tipologia
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                int oggettoSelezionato = e.Item.ItemIndex;
                DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[oggettoSelezionato];

                if (template.IPER_FASC_DOC == "1")
                {
                    if (ProfilazioneDocManager.isInUseCampoComuneDoc(template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "campoNonEliminabile", "alert('Campo non eliminabile. Risulta associato a una o più tipologie.');", true);
                        return;
                    }
                }

                if (oggettoSelezionato != -1)
                {
                    //DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                    //DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[oggettoSelezionato];
                    Session.Add("template", ProfilazioneDocManager.eliminaOggettoCustomTemplate(template, oggettoSelezionato, this));
                    btn_salvaTemplate.Visible = true;
                    aggiungiComponenteDG();
                }

                Panel_Personalizzazione.Visible = false;
                dg_listaComponenti.SelectedIndex = -1;
                if (dg_listaComponenti.Items.Count == 0)
                {
                    btn_anteprima.Visible = false;
                    btn_salvaTemplate.Visible = false;
                    dg_listaComponenti.Visible = false;
                    Panel_Dg_ListaComponenti.Visible = false;
                }
                lbl_avviso.Visible = false;
            }
        }

        private void dg_listaComponenti_ItemCommand(object sender, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            int elSelezionato = e.Item.ItemIndex;
            if (elSelezionato != -1)
            {
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[elSelezionato];

                switch (e.CommandName)
                {
                    case "ConsolidaCampo":
                        Session.Add("temp_c1", elSelezionato);
                        this.LblOggConsolidamento.Text = oggettoCustom.DESCRIZIONE;
                        if (!string.IsNullOrEmpty(oggettoCustom.CONSOLIDAMENTO) && oggettoCustom.CONSOLIDAMENTO.Equals("1"))
                            this.cb_ModConsolidaOgg.Checked = true;
                        else
                            this.cb_ModConsolidaOgg.Checked = false;
                        this.Panel_ConsolidaOgg.Visible = true;
                        this.Panel_ConservaOgg.Visible = false;
                        break;

                    case "ConservaCampo":
                        Session.Add("temp_c2", elSelezionato);
                        this.LblOggConservazione.Text = oggettoCustom.DESCRIZIONE;
                        if (!string.IsNullOrEmpty(oggettoCustom.CONSERVAZIONE) && oggettoCustom.CONSERVAZIONE.Equals("1"))
                            this.cb_ModConservaOgg.Checked = true;
                        else
                            this.cb_ModConservaOgg.Checked = false;
                        this.Panel_ConsolidaOgg.Visible = false;
                        this.Panel_ConservaOgg.Visible = true;
                        break;

                }

            }
        }

        #endregion DataGrid Lista Componenti

        #region VisualizzaPannelloPersonalizzazione
        public void visualizzaPannelloPersonalizzazzione(string tipoComponente, DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            //Abilita uno dei pannelli di personalizzazione a seconda del tipo di componenete e popola
            //i campi presenti nel pannello.
            switch (tipoComponente)
            {
                case "CampoDiTesto":
                    Panel_Personalizzazione.Visible = true;
                    Panel_PersCampoDiTesto.Visible = true;
                    Panel_PersCasellaDiSelezione.Visible = false;
                    Panel_PersSelezioneEsclusiva.Visible = false;
                    Panel_PersMenuATendina.Visible = false;
                    Panel_PersContatore.Visible = false;
                    Panel_PersData.Visible = false;
                    Panel_PersCorrispondente.Visible = false;
                    Panel_PersLink.Visible = false;
                    Panel_PersContSottocontatore.Visible = false;
                    Panel_PersSeparatore.Visible = false;
                    Panel_PersOggEsterno.Visible = false;

                    if (oggettoCustom != null && oggettoCustom.CAMPO_OBBLIGATORIO != null &&
                        oggettoCustom.CAMPO_DI_RICERCA != null && oggettoCustom.MULTILINEA != null)
                    {
                        compilaPersCampoDiTesto(oggettoCustom);
                    }
                    else
                    {
                        resettaPersCampoDiTesto();
                    }
                    SetFocus(txt_etichettaCampoDiTesto);
                    break;
                case "CasellaDiSelezione":
                    Panel_Personalizzazione.Visible = true;
                    Panel_PersCampoDiTesto.Visible = false;
                    Panel_PersCasellaDiSelezione.Visible = true;
                    Panel_PersSelezioneEsclusiva.Visible = false;
                    Panel_PersMenuATendina.Visible = false;
                    Panel_PersContatore.Visible = false;
                    Panel_PersData.Visible = false;
                    Panel_PersCorrispondente.Visible = false;
                    Panel_PersLink.Visible = false;
                    Panel_PersContSottocontatore.Visible = false;
                    Panel_PersSeparatore.Visible = false;
                    Panel_PersOggEsterno.Visible = false;

                    if (oggettoCustom != null && oggettoCustom.CAMPO_OBBLIGATORIO != null &&
                        oggettoCustom.CAMPO_DI_RICERCA != null)
                    {
                        compilaPersCasellaDiSelezione(oggettoCustom);
                    }
                    else
                    {
                        resettaPersCasellaDiSelezione();
                    }
                    SetFocus(txt_etichettaCasellaDiSelezione);
                    break;
                case "SelezioneEsclusiva":
                    Panel_Personalizzazione.Visible = true;
                    Panel_PersCampoDiTesto.Visible = false;
                    Panel_PersCasellaDiSelezione.Visible = false;
                    Panel_PersSelezioneEsclusiva.Visible = true;
                    Panel_PersMenuATendina.Visible = false;
                    Panel_PersContatore.Visible = false;
                    Panel_PersData.Visible = false;
                    Panel_PersCorrispondente.Visible = false;
                    Panel_PersLink.Visible = false;
                    Panel_PersContSottocontatore.Visible = false;
                    Panel_PersSeparatore.Visible = false;
                    Panel_PersOggEsterno.Visible = false;

                    if (oggettoCustom != null && (oggettoCustom.ELENCO_VALORI).Length != 0 &&
                        oggettoCustom.CAMPO_DI_RICERCA != null && oggettoCustom.CAMPO_OBBLIGATORIO != null)
                    {
                        compilaPersSelezioneEsclusiva(oggettoCustom);
                    }
                    else
                    {
                        resettaPersSelezioneEsclusiva();
                    }

                    if (utils.InitConfigurationKeys.GetValue("0", "ENABLE_COLOR_MULTIVALUE") == null || !utils.InitConfigurationKeys.GetValue("0", "ENABLE_COLOR_MULTIVALUE").Equals("1"))
                    {
                        this.LblColoreSelezioneEsclusiva.Visible = false;
                        this.DdlColoriSelezioneEsclusiva.Visible = false;
                    }
                    SetFocus(txt_etichettaSelezioneEsclusiva);
                    break;
                case "MenuATendina":
                    Panel_Personalizzazione.Visible = true;
                    Panel_PersCampoDiTesto.Visible = false;
                    Panel_PersCasellaDiSelezione.Visible = false;
                    Panel_PersSelezioneEsclusiva.Visible = false;
                    Panel_PersMenuATendina.Visible = true;
                    Panel_PersContatore.Visible = false;
                    Panel_PersData.Visible = false;
                    Panel_PersCorrispondente.Visible = false;
                    Panel_PersLink.Visible = false;
                    Panel_PersContSottocontatore.Visible = false;
                    Panel_PersSeparatore.Visible = false;
                    Panel_PersOggEsterno.Visible = false;

                    if (oggettoCustom != null && (oggettoCustom.ELENCO_VALORI).Length != 0 &&
                        oggettoCustom.CAMPO_DI_RICERCA != null && oggettoCustom.CAMPO_OBBLIGATORIO != null)
                    {
                        compilaPersMenuATendina(oggettoCustom);
                    }
                    else
                    {
                        resettaPersMenuATendina();
                    }
                    if (utils.InitConfigurationKeys.GetValue("0", "ENABLE_COLOR_MULTIVALUE") == null || !utils.InitConfigurationKeys.GetValue("0", "ENABLE_COLOR_MULTIVALUE").Equals("1"))
                    {
                        this.LblColoreMenu.Visible = false;
                        this.DdlColoriMenu.Visible = false;
                    }
                    SetFocus(txt_etichettaMenuATendina);
                    break;
                case "Contatore":
                    Panel_Personalizzazione.Visible = true;
                    Panel_PersCampoDiTesto.Visible = false;
                    Panel_PersCasellaDiSelezione.Visible = false;
                    Panel_PersSelezioneEsclusiva.Visible = false;
                    Panel_PersMenuATendina.Visible = false;
                    Panel_PersData.Visible = false;
                    Panel_PersContatore.Visible = true;
                    Panel_PersCorrispondente.Visible = false;
                    Panel_PersLink.Visible = false;
                    Panel_PersContSottocontatore.Visible = false;
                    Panel_PersSeparatore.Visible = false;
                    Panel_PersOggEsterno.Visible = false;
                    //DataFine.Visible = false;
                    //resetto il radiobutton contatore
                    //RadioButtonContatore.Items[0].Enabled = true;
                    //RadioButtonContatore.Items[0].Selected = false;
                    //RadioButtonContatore.Items[1].Enabled = true;
                    //RadioButtonContatore.Items[1].Selected = false;

                    if (oggettoCustom != null && oggettoCustom.CAMPO_DI_RICERCA != null)
                    {
                        compilaPersContatore(oggettoCustom);
                    }
                    else
                    {
                        resettaPersContatore();
                    }



                    cb_Contatore_visibile.Enabled = cb_Ricerca_Contatore.Checked;
                    cb_Contatore_visibile.Checked = false;
                    //modifica
                    if (oggettoCustom != null &&
                        cb_Contatore_visibile.Enabled)
                    {
                        if (oggettoCustom.DA_VISUALIZZARE_RICERCA.Equals("1"))
                            cb_Contatore_visibile.Checked = true;
                        else
                            cb_Contatore_visibile.Checked = false;

                    }
                    //fine modifica
                    SetFocus(txt_etichettaContatore);
                    controllaEsistenzaRF();
                    break;
                case "Data":
                    Panel_Personalizzazione.Visible = true;
                    Panel_PersCampoDiTesto.Visible = false;
                    Panel_PersCasellaDiSelezione.Visible = false;
                    Panel_PersSelezioneEsclusiva.Visible = false;
                    Panel_PersMenuATendina.Visible = false;
                    Panel_PersContatore.Visible = false;
                    Panel_PersData.Visible = true;
                    Panel_PersCorrispondente.Visible = false;
                    Panel_PersLink.Visible = false;
                    Panel_PersContSottocontatore.Visible = false;
                    Panel_PersSeparatore.Visible = false;
                    Panel_PersOggEsterno.Visible = false;

                    if (oggettoCustom != null && oggettoCustom.CAMPO_DI_RICERCA != null &&
                        oggettoCustom.CAMPO_OBBLIGATORIO != null)
                    {
                        compilaPersData(oggettoCustom);
                    }
                    else
                    {
                        resettaPersData();
                    }
                    SetFocus(txt_etichettaData);
                    break;
                case "Corrispondente":
                    Panel_Personalizzazione.Visible = true;
                    Panel_PersCampoDiTesto.Visible = false;
                    Panel_PersCasellaDiSelezione.Visible = false;
                    Panel_PersSelezioneEsclusiva.Visible = false;
                    Panel_PersMenuATendina.Visible = false;
                    Panel_PersContatore.Visible = false;
                    Panel_PersData.Visible = false;
                    Panel_PersCorrispondente.Visible = true;
                    Panel_PersLink.Visible = false;
                    Panel_PersContSottocontatore.Visible = false;
                    Panel_PersSeparatore.Visible = false;
                    Panel_PersOggEsterno.Visible = false;

                    if (oggettoCustom != null && oggettoCustom.CAMPO_DI_RICERCA != null &&
                        oggettoCustom.CAMPO_OBBLIGATORIO != null)
                    {
                        compilaPersCorrispondente(oggettoCustom);
                    }
                    else
                    {
                        resettaPersCorrispondente();
                    }
                    SetFocus(txt_etichettaCorr);
                    break;
                case "Link":
                    Panel_Personalizzazione.Visible = true;
                    Panel_PersCampoDiTesto.Visible = false;
                    Panel_PersCasellaDiSelezione.Visible = false;
                    Panel_PersSelezioneEsclusiva.Visible = false;
                    Panel_PersMenuATendina.Visible = false;
                    Panel_PersContatore.Visible = false;
                    Panel_PersData.Visible = false;
                    Panel_PersCorrispondente.Visible = false;
                    Panel_PersLink.Visible = true;
                    Panel_PersContSottocontatore.Visible = false;
                    Panel_PersSeparatore.Visible = false;
                    Panel_PersOggEsterno.Visible = false;

                    if (oggettoCustom != null && !string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE) && !string.IsNullOrEmpty(oggettoCustom.TIPO_LINK))
                    {
                        ddl_tipoLink.SelectedValue = oggettoCustom.TIPO_LINK;
                        if (oggettoCustom.TIPO_LINK.Equals("INTERNO"))
                        {
                            ddl_tipoObjLink.SelectedValue = oggettoCustom.TIPO_OBJ_LINK;
                            ddl_tipoObjLink.Enabled = true;
                        }
                        else
                        {
                            ddl_tipoObjLink.SelectedIndex = 0;
                            ddl_tipoObjLink.Enabled = false;
                        }
                        txt_etichettaLink.Text = oggettoCustom.DESCRIZIONE;
                        if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                        {
                            cb_obbligatorioLink.Checked = true;
                        }
                        else
                        {
                            cb_obbligatorioLink.Checked = false;
                        }
                    }
                    else
                    {
                        ddl_tipoLink.SelectedIndex = 0;
                        ddl_tipoObjLink.Enabled = true;
                        ddl_tipoObjLink.SelectedIndex = 0;
                        txt_etichettaLink.Text = "";
                        cb_obbligatorioLink.Checked = false;
                    }
                    SetFocus(txt_etichettaLink);
                    break;
                case "ContatoreSottocontatore":
                    Panel_Personalizzazione.Visible = true;
                    Panel_PersCampoDiTesto.Visible = false;
                    Panel_PersCasellaDiSelezione.Visible = false;
                    Panel_PersSelezioneEsclusiva.Visible = false;
                    Panel_PersMenuATendina.Visible = false;
                    Panel_PersData.Visible = false;
                    Panel_PersContatore.Visible = false;
                    Panel_PersCorrispondente.Visible = false;
                    Panel_PersLink.Visible = false;
                    Panel_PersSeparatore.Visible = false;
                    Panel_PersContSottocontatore.Visible = true;
                    Panel_PersOggEsterno.Visible = false;

                    if (oggettoCustom != null && oggettoCustom.CAMPO_DI_RICERCA != null)
                    {
                        compilaPersContSottocontatore(oggettoCustom);
                    }
                    else
                    {
                        resettaPersContSottocontatore();
                    }

                    //fine modifica
                    SetFocus(txt_etichettaContSottocontatore);
                    controllaEsistenzaRF();
                    break;
                case "Separatore":
                    Panel_Personalizzazione.Visible = true;
                    Panel_PersCampoDiTesto.Visible = false;
                    Panel_PersCasellaDiSelezione.Visible = false;
                    Panel_PersSelezioneEsclusiva.Visible = false;
                    Panel_PersMenuATendina.Visible = false;
                    Panel_PersData.Visible = false;
                    Panel_PersContatore.Visible = false;
                    Panel_PersCorrispondente.Visible = false;
                    Panel_PersLink.Visible = false;
                    Panel_PersContSottocontatore.Visible = false;
                    Panel_PersSeparatore.Visible = true;
                    Panel_PersOggEsterno.Visible = false;

                    if (oggettoCustom != null && oggettoCustom.CAMPO_DI_RICERCA != null)
                    {
                        compilaPersSeparatore(oggettoCustom);
                    }
                    else
                    {
                        resettaPersSeparatore();
                    }

                    //fine modifica
                    SetFocus(txt_etichettaSeparatore);
                    break;
                case "OggettoEsterno":
                    Panel_Personalizzazione.Visible = true;
                    Panel_PersCampoDiTesto.Visible = false;
                    Panel_PersCasellaDiSelezione.Visible = false;
                    Panel_PersSelezioneEsclusiva.Visible = false;
                    Panel_PersMenuATendina.Visible = false;
                    Panel_PersContatore.Visible = false;
                    Panel_PersData.Visible = false;
                    Panel_PersCorrispondente.Visible = false;
                    Panel_PersLink.Visible = false;
                    Panel_PersContSottocontatore.Visible = false;
                    Panel_PersSeparatore.Visible = false;
                    Panel_PersOggEsterno.Visible = true;

                    if (oggettoCustom != null)
                    {
                        this.txt_etichettaOggEsterno.Text = oggettoCustom.DESCRIZIONE;
                        if ("SI".Equals(oggettoCustom.CAMPO_OBBLIGATORIO))
                        {
                            this.cb_obbligatorioOggEsterno.Checked = true;
                        }
                        else
                        {
                            this.cb_obbligatorioOggEsterno.Checked = false;
                        }
                        if (!string.IsNullOrEmpty(oggettoCustom.CONFIG_OBJ_EST))
                        {
                            this.intAdapter_OggEsterno.ConfigurationValue = oggettoCustom.CONFIG_OBJ_EST;
                        }
                        if ("SI".Equals(oggettoCustom.CAMPO_DI_RICERCA))
                        {
                            cb_ricercaOggEsterno.Checked = true;
                        }
                        else
                        {
                            cb_ricercaOggEsterno.Checked = false;
                        }
                    }
                    else
                    {
                        this.txt_etichettaOggEsterno.Text = "";
                        this.cb_obbligatorioOggEsterno.Checked = false;
                        this.cb_ricercaOggEsterno.Checked = false;
                        this.intAdapter_OggEsterno.Clear();

                    }
                    break;
            }
        }
        #endregion VisualizzaPannelloPersonalizzazione

        #region btn_nuovoModello_Click - bn_AggiungiOggetto_Click - resetNuovoTemplate
        private void btn_nuovoModello_Click(object sender, System.EventArgs e)
        {
            //Disabilita il pannello che contiene la lista dei template e visualizza quello per
            //l'inserimento di un nuovo template.
            Panel_ListaModelli.Visible = false;
            Panel_NuovoModello.Visible = true;
            resetNuovoTemplate();
            //lblDataFine.Visible = false;
            //lblDataInizio.Visible = false;
            //DataFine.Visible = false;
            //DataInizio.Visible = false;
            //RadioButtonContatore.Items[1].Selected = false;
            //RadioButtonContatore.Items[0].Selected = false;
            //DataFine.Text = string.Empty;
            //DataInizio.Text = string.Empty;
            SetFocus(txt_TipoDocumento);
            lbl_titolo.Visible = false;
            Panel_Diagrammi_Trasmissioni.Visible = false;
            cb_Privato.Checked = false;
            Panel_Privato.Visible = false;
            Panel_MesiCons.Visible = false;
            //disabilito il pulsante 'Gestione Storico'
            this.btnStorico.Enabled = false;
            this.btn_ContestoProcedurale.Visible = false;
            //disabilità il pulsante campi comuni
            this.btn_CampiComuni.Visible = false;
        }

        private void btn_AggiungiOggetto_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            aggiornaTemplateInSessione();

            //Questa imageButton mi serve per conservare una delle cinque imageButton cliccabili e
            //recuperarne l'ID per capire quale è stata cliccata
            ImageButton imgBtn = (ImageButton)sender;

            //Controllo esistenza tipo di documento
            if (controllaTipoDiDocumento() && !dg_listaTemplates.Visible)
            {
                lbl_avviso.Text = "Tipo di documento già esistente !";
                lbl_avviso.Visible = true;
                SetFocus(txt_TipoDocumento);
                return;
            }

            //Controllo la presenza dei valori "< >"
            if (txt_TipoDocumento.Text.IndexOf("<") != -1 || txt_TipoDocumento.Text.IndexOf(">") != -1)
            {
                lbl_avviso.Text = "Caratteri non permessi !";
                lbl_avviso.Visible = true;
                SetFocus(txt_TipoDocumento);
                return;
            }

            //Controllo dei campi obbligatori
            if (txt_TipoDocumento.Text.Equals("") && !dg_listaTemplates.Visible)
            {
                lbl_avviso.Text = "Inserire i campi obbligatori !";
                lbl_avviso.Visible = true;
                SetFocus(txt_TipoDocumento);
            }
            else
            {
                //Vengono visualizzati il DataGrid dei componenti e il pannello per la personalizzazione
                //del componente che si è scelto di inserire. Inoltre vengono disabilitati i campo del
                //nome del modello e del tipo di documento.
                lbl_avviso.Visible = false;
                txt_TipoDocumento.Enabled = false;
                txt_TipoDocumento.BackColor = System.Drawing.Color.AntiqueWhite;
                ddl_valoriSelezioneEsclusiva.Items.Clear();
                txt_etichettaSelezioneEsclusiva.Text = "";

                if (Session["template"] == null)
                {
                    aggiungiTemplateInSessione(imgBtn.ID);
                }
                else
                {
                    aggiungiOggettoTemplate(imgBtn.ID);
                }

                Panel_ListaComponenti.Visible = true;
                Panel_Personalizzazione.Visible = true;

                aggiungiComponenteDG();
                scrollKeeperListaComponenti.VPos = (dg_listaComponenti.Items.Count * 25) + 12;

                //L'oggettoCustom che sarebbe il secondo parametro del seguente metodo è "null"
                //in quanto le componenti del pannello personalizzazione non devono essere valorizzate
                visualizzaPannelloPersonalizzazzione(imgBtn.ID, null);
            }
        }

        public void resetNuovoTemplate()
        {
            //Riporta la situazione allo stato iniziale, o meglio alla stato di inserimento di un nuovo
            //modello. Importante la rimozione dell'oggetto "template" dalla sessione.
            lbl_avviso.Visible = false;
            txt_TipoDocumento.Enabled = true;
            txt_TipoDocumento.Text = "";
            Panel_ListaComponenti.Visible = true;
            Panel_Personalizzazione.Visible = false;
            Panel_Dg_ListaComponenti.Visible = false;
            dt_listaComponenti.Rows.Clear();
            txt_MesiConservazione.Text = "0";
            cb_InvioConservazione.Checked = false;
            /*
            if(dt_listaComponenti.Columns.Count != 0)
            {
                //listaComponenti.Columns.Remove("Nome");
                dt_listaComponenti.Columns.Remove("Tipo");
            }
            */
            DataFine.Visible = false;
            DataInizio.Visible = false;
            DataFine.Text = string.Empty;
            DataInizio.Text = string.Empty;
            lblDataInizio.Visible = false;
            lblDataFine.Visible = false;
            RadioButtonContatore.Items[0].Enabled = true;
            RadioButtonContatore.Items[0].Selected = true;
            RadioButtonContatore.Items[1].Enabled = true;
            RadioButtonContatore.Items[1].Selected = false;
            cb_Azzera_Anno.Enabled = true;
            cb_Azzera_Anno.Checked = false;
            cb_Repertorio.Checked = false;
            cb_Repertorio.Visible=true;
            lbl_repertorio.Visible=true;


            txt_TipoDocumento.BackColor = System.Drawing.Color.White;
            btn_modelli.Visible = true;
            btn_anteprima.Visible = false;
            btn_salvaTemplate.Visible = true;

            ddl_valoriSelezioneEsclusiva.Items.Clear();
            txt_etichettaSelezioneEsclusiva.Text = "";
            ddl_valoriMenuATendina.Items.Clear();
            txt_etichettaMenuATendina.Text = "";
            dg_listaComponenti.Visible = false;



            Session.Remove("template");
            Session.Remove("defaultMenuATendina");
            Session.Remove("defaultSelezioneEsclusiva");
            //Session.Remove("templateSelezionato");
            Session.Remove("nomeCampo");

            SetFocus(txt_TipoDocumento);
        }
        #endregion btn_nuovoModello_Click - btn_AggiungiOggetto_Click - resetNuovoTemplate

        #region btn_ConfermaPers_Click - pulsante_chiudi_pers_Click
        private void btn_ConfermaPers_Click(object sender, System.EventArgs e)
        {
            //this.btnStorico.Enabled = true;
            //Questo Button mi serve per conservare una dei cinque Bottoni cliccabili dai pannelli 
            //di personalizzazione e recuperarne l'ID per capire quale è stato cliccato
            //Il seguente metodo viene chiamato quando si clicca uno dei pulsanti "Conferma" dei pannelli
            //di personalizzazione. A senconda della provenienza della chiamata vengono effettuate
            //le dovute operazioni.
            Button button = (Button)sender;
            // aggiungiValoreOggetto();//????
            switch (button.ID)
            {
                case "btn_ConfermaPersCampoDiTesto":

                    //Controllo la presenza dei valori "< >"
                    if (txt_etichettaCampoDiTesto.Text.IndexOf("<") != -1 || txt_etichettaCampoDiTesto.Text.IndexOf(">") != -1)
                    {
                        lbl_avviso.Text = "Caratteri non permessi !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaCampoDiTesto);
                        return;
                    }

                    if (txt_etichettaCampoDiTesto.Text.TrimStart(' ').Equals(""))
                    {
                        lbl_avviso.Text = "Inserire i campi obbligatori !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaCampoDiTesto);
                    }
                    else
                    {
                        if (controllaEtichetta(txt_etichettaCampoDiTesto.Text))
                        {
                            RegisterStartupScript("etichettaDuplicata", "<script>alert('Etichetta campo già esistente !');</script>");
                            return;
                        }

                        int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                        if (oggettoSelezionato != -1)
                        {
                            try
                            {
                                if (!txt_NumeroCaratteri_CampoDiTesto.Text.Equals(""))
                                {
                                    int nCaratteri = Convert.ToInt32(txt_NumeroCaratteri_CampoDiTesto.Text);
                                    if (nCaratteri > 255)
                                    {
                                        lbl_avviso.Text = "Il N. Caratteri non può superare i 255 !";
                                        lbl_avviso.Visible = true;
                                        txt_NumeroCaratteri_CampoDiTesto.Text = "255";
                                        SetFocus(txt_NumeroCaratteri_CampoDiTesto);
                                        return;
                                    }
                                    if (nCaratteri < 2)
                                    {
                                        lbl_avviso.Text = "N. Caratteri deve essere maggiore o uguale a due !";
                                        lbl_avviso.Visible = true;
                                        SetFocus(txt_NumeroCaratteri_CampoDiTesto);
                                        return;
                                    }
                                    if(!string.IsNullOrEmpty(txt_TestoDefaultCampoTesto.Text))
                                    {
                                        int nCaratteriDefault = Convert.ToInt32(txt_TestoDefaultCampoTesto.Text.Length);
                                        if(nCaratteriDefault > nCaratteri)
                                        {
                                            lbl_avviso.Text = "La lunghezza del testo di default è superiore al N. Caratteri specificato !";
                                            lbl_avviso.Visible = true;
                                            SetFocus(txt_TestoDefaultCampoTesto);
                                            return;
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(txt_TestoDefaultCampoTesto.Text))
                                {
                                    int nCaratteriDefault = Convert.ToInt32(txt_TestoDefaultCampoTesto.Text.Length);
                                    if (nCaratteriDefault > 255)
                                    {
                                        lbl_avviso.Text = "Il numero di caratteri del Testo di default è superiore a 255 !";
                                        lbl_avviso.Visible = true;
                                        SetFocus(txt_TestoDefaultCampoTesto);
                                        return;
                                    }
                                }
                                if (!txt_NumeroLinee_CampoDiTesto.Text.Equals(""))
                                {
                                    int nLinee = Convert.ToInt32(txt_NumeroLinee_CampoDiTesto.Text);
                                    if (nLinee < 2)
                                    {
                                        lbl_avviso.Text = "N. Linee deve essere maggiore o uguale a due !";
                                        lbl_avviso.Visible = true;
                                        SetFocus(txt_NumeroLinee_CampoDiTesto);
                                        return;
                                    }
                                }
                            }
                            catch
                            {
                                lbl_avviso.Text = "N. Caratteri e N. Linee devono essere dei numeri interi !";
                                lbl_avviso.Visible = true;
                                //SetFocus(txt_NumeroCaratteri_CampoDiTesto);
                                SetFocus(txt_etichettaCampoDiTesto);
                                return;
                            }
                            aggiungiValoreOggetto();
                        }
                        aggiungiComponenteDG();
                        dg_listaComponenti.SelectedIndex = -1;
                        resettaPersCampoDiTesto();
                        Panel_Personalizzazione.Visible = false;
                        lbl_avviso.Visible = false;
                        btn_anteprima.Visible = true;
                        btn_salvaTemplate.Visible = true;
                    }
                    break;
                case "btn_ConfermaPersCasellaDiSelezione":

                    //Controllo la presenza dei valori "< >"
                    if (txt_etichettaCasellaDiSelezione.Text.IndexOf("<") != -1 || txt_etichettaCasellaDiSelezione.Text.IndexOf(">") != -1)
                    {
                        lbl_avviso.Text = "Caratteri non permessi !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaCasellaDiSelezione);
                        return;
                    }

                    if (txt_etichettaCasellaDiSelezione.Text.TrimStart(' ').Equals(""))
                    {
                        lbl_avviso.Text = "Inserire i campi obbligatori !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaCasellaDiSelezione);
                    }
                    else
                    {
                        if (controllaEtichetta(txt_etichettaCasellaDiSelezione.Text))
                        {
                            RegisterStartupScript("etichettaDuplicata", "<script>alert('Etichetta campo già esistente !');</script>");
                            return;
                        }

                        int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                        if (oggettoSelezionato != -1 && ddl_valoriCasellaSelezione.Items.Count >= 1)
                        {
                            aggiungiValoreOggetto();
                        }
                        else
                        {
                            lbl_avviso.Text = "Per questo tipo di componente, deve essere inserito almeno un valore !";
                            lbl_avviso.Visible = true;
                            SetFocus(txt_valoreCasellaSelezione);
                            return;
                        }
                        aggiungiComponenteDG();
                        dg_listaComponenti.SelectedIndex = -1;
                        txt_etichettaCasellaDiSelezione.Text = "";
                        Panel_Personalizzazione.Visible = false;
                        lbl_avviso.Visible = false;
                        btn_anteprima.Visible = true;
                        btn_salvaTemplate.Visible = true;
                    }
                    break;
                case "btn_ConfermaPersContatore":
                    if (RadioButtonContatore.SelectedValue == "Custom")
                    {
                        if (string.IsNullOrEmpty(DataInizio.Text))
                        {
                            lbl_avviso.Text = "Inserire data di inizio conteggio";
                            lbl_avviso.Visible = true;
                            return;
                        }

                        if (string.IsNullOrEmpty(DataFine.Text))
                        {
                            lbl_avviso.Text = "Inserire data di fine conteggio";
                            lbl_avviso.Visible = true;
                            return;
                        }

                        if (string.IsNullOrEmpty(DataFine.Text) && string.IsNullOrEmpty(DataInizio.Text))
                        {
                            lbl_avviso.Text = "Inserire date di conteggio";
                            lbl_avviso.Visible = true;
                            return;
                        }

                        if ((!string.IsNullOrEmpty(DataFine.Text) && !string.IsNullOrEmpty(DataInizio.Text)) && (ControllaDate(DataInizio.Text, DataFine.Text) != true))
                        {
                            lbl_avviso.Text = "Inserire date di conteggio corrette";
                            lbl_avviso.Visible = true;
                            return;
                        }

                        if ((!string.IsNullOrEmpty(DataFine.Text) && !string.IsNullOrEmpty(DataInizio.Text)) && (ControllaDate2(DataInizio.Text, DataFine.Text) != true))
                        {
                            lbl_avviso.Text = "L’anno della data iniziale custom deve essere inferiore all’anno della data finale custom";
                            lbl_avviso.Visible = true;
                            return;
                        }

                        if ((!string.IsNullOrEmpty(DataFine.Text) && !string.IsNullOrEmpty(DataInizio.Text)) && (ControllaDate3(DataInizio.Text, DataFine.Text) != true))
                        {
                            lbl_avviso.Text = "Il contatore non può avere l’intervallo superiore ad un anno";
                            lbl_avviso.Visible = true;
                            return;
                        }


                    }
                    //Controllo la presenza dei valori "< >"
                    if (txt_etichettaContatore.Text.IndexOf("<") != -1 || txt_etichettaContatore.Text.IndexOf(">") != -1)
                    {
                        lbl_avviso.Text = "Caratteri non permessi !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaContatore);
                        return;
                    }

                    /*
                    int oggettoCustomSelezionato = dg_listaComponenti.SelectedIndex;
                    DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                    DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[oggettoCustomSelezionato];

                    if (this.cb_ContaDopo.Checked)
                    {
                        // controllo sull'associazione ruolo-contatore
                        if (!ProfilazioneDocManager.verificaAssociazioneRuoloOggettoCustom(oggettoCustom.SYSTEM_ID.ToString(), "", "INSERIMENTO",this))
                        {
                            lbl_avviso.Text = "Non esiste una associazione tra un ruolo e il contatore in oggetto !";
                            lbl_avviso.Visible = true;
                            return;
                        }
                    }
                    else
                    {
                        // metodo che cancella i record sulla DPA_ASS_RUOLO_OGG_CUSTOM
                        // per l'id oggetto in esame se il check ContaDopo era attivo ed è stato disattivato
                        if (ProfilazioneDocManager.verificaAssociazioneRuoloOggettoCustom(oggettoCustom.SYSTEM_ID.ToString(), "", "INSERIMENTO",this))
                            ProfilazioneDocManager.cancellaAssociazioneRuoloOggettoCustom(oggettoCustom.SYSTEM_ID.ToString(), "", "INSERIMENTO", this);
                    }
                    */

                    //Controllo che il formato del contatore contenga il valore CONTATORE che è obbligatorio                 
                    if (!txt_formatoContatore.Text.Contains("CONTATORE"))
                    {
                        lbl_avviso.Text = "Nel formato è obbligatorio il campo CONTATORE !";
                        lbl_avviso.Visible = true;
                        return;
                    }

                    if (txt_etichettaContatore.Text.TrimStart(' ').Equals(""))
                    {
                        lbl_avviso.Text = "Inserire i campi obbligatori !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaContatore);
                    }
                    else
                    {
                        if (controllaEtichetta(txt_etichettaContatore.Text))
                        {
                            RegisterStartupScript("etichettaDuplicata", "<script>alert('Etichetta campo già esistente !');</script>");
                            return;
                        }

                        int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                        if (oggettoSelezionato != -1)
                        {
                            aggiungiValoreOggetto();
                        }
                        aggiungiComponenteDG();
                        dg_listaComponenti.SelectedIndex = -1;
                        txt_etichettaContatore.Text = "";
                        Panel_Personalizzazione.Visible = false;
                        lbl_avviso.Visible = false;
                        btn_anteprima.Visible = true;
                        btn_salvaTemplate.Visible = true;

                    }
                    break;
                case "btn_ConfermaPersSelezioneEsclusiva":

                    //Controllo la presenza dei valori "< >"
                    if (txt_etichettaSelezioneEsclusiva.Text.IndexOf("<") != -1 || txt_etichettaSelezioneEsclusiva.Text.IndexOf(">") != -1)
                    {
                        lbl_avviso.Text = "Caratteri non permessi !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaSelezioneEsclusiva);
                        return;
                    }

                    if (txt_etichettaSelezioneEsclusiva.Text.TrimStart(' ').Equals(""))
                    {
                        lbl_avviso.Text = "Inserire i campi obbligatori !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaSelezioneEsclusiva);
                    }
                    else
                    {
                        if (controllaEtichetta(txt_etichettaSelezioneEsclusiva.Text))
                        {
                            RegisterStartupScript("etichettaDuplicata", "<script>alert('Etichetta campo già esistente !');</script>");
                            return;
                        }

                        int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                        if (oggettoSelezionato != -1 && ddl_valoriSelezioneEsclusiva.Items.Count >= 2)
                        {
                            aggiungiValoreOggetto();
                        }
                        else
                        {
                            lbl_avviso.Text = "Per questo tipo di componente, i valori devono essere almeno due !";
                            lbl_avviso.Visible = true;
                            SetFocus(txt_valoreSelezioneEsclusiva);
                            return;
                        }
                        aggiungiComponenteDG();
                        dg_listaComponenti.SelectedIndex = -1;
                        txt_etichettaSelezioneEsclusiva.Text = "";
                        Panel_Personalizzazione.Visible = false;
                        lbl_avviso.Visible = false;
                        btn_anteprima.Visible = true;
                        btn_salvaTemplate.Visible = true;
                    }
                    break;
                case "btn_ConfermaPersMenuATendina":

                    //Controllo la presenza dei valori "< >"
                    if (txt_etichettaMenuATendina.Text.IndexOf("<") != -1 || txt_etichettaMenuATendina.Text.IndexOf(">") != -1)
                    {
                        lbl_avviso.Text = "Caratteri non permessi !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaMenuATendina);
                        return;
                    }

                    if (txt_etichettaMenuATendina.Text.TrimStart(' ').Equals(""))
                    {
                        lbl_avviso.Text = "Inserire i campi obbligatori !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaMenuATendina);
                    }
                    else
                    {
                        if (controllaEtichetta(txt_etichettaMenuATendina.Text))
                        {
                            RegisterStartupScript("etichettaDuplicata", "<script>alert('Etichetta campo già esistente !');</script>");
                            return;
                        }

                        int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                        if (oggettoSelezionato != -1 && ddl_valoriMenuATendina.Items.Count >= 1)
                        {
                            aggiungiValoreOggetto();
                        }
                        else
                        {
                            lbl_avviso.Text = "Per questo tipo di componente, deve essere inserito almeno un valore !";
                            lbl_avviso.Visible = true;
                            SetFocus(txt_valoreMenuATendina);
                            return;
                        }
                        aggiungiComponenteDG();
                        dg_listaComponenti.SelectedIndex = -1;
                        txt_etichettaMenuATendina.Text = "";
                        Panel_Personalizzazione.Visible = false;
                        lbl_avviso.Visible = false;
                        btn_anteprima.Visible = true;
                        btn_salvaTemplate.Visible = true;
                    }
                    break;
                case "btn_ConfermaPersData":

                    //Controllo la presenza dei valori "< >"
                    if (txt_etichettaData.Text.IndexOf("<") != -1 || txt_etichettaData.Text.IndexOf(">") != -1)
                    {
                        lbl_avviso.Text = "Caratteri non permessi !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaData);
                        return;
                    }

                    if (txt_etichettaData.Text.TrimStart(' ').Equals(""))
                    {
                        lbl_avviso.Text = "Inserire i campi obbligatori !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaData);
                    }
                    else
                    {
                        if (controllaEtichetta(txt_etichettaData.Text))
                        {
                            RegisterStartupScript("etichettaDuplicata", "<script>alert('Etichetta campo già esistente !');</script>");
                            return;
                        }

                        int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                        if (oggettoSelezionato != -1)
                        {
                            aggiungiValoreOggetto();
                        }
                        aggiungiComponenteDG();
                        dg_listaComponenti.SelectedIndex = -1;
                        txt_etichettaData.Text = "";
                        Panel_Personalizzazione.Visible = false;
                        lbl_avviso.Visible = false;
                        btn_anteprima.Visible = true;
                        btn_salvaTemplate.Visible = true;
                    }
                    break;
                case "btn_ConfermaPersCorrispondente":
                    //Controllo la presenza dei valori "< >"
                    if (txt_etichettaCorr.Text.IndexOf("<") != -1 || txt_etichettaCorr.Text.IndexOf(">") != -1)
                    {
                        lbl_avviso.Text = "Caratteri non permessi !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaCorr);
                        return;
                    }

                    if (txt_etichettaCorr.Text.TrimStart(' ').Equals(""))
                    {
                        lbl_avviso.Text = "Inserire i campi obbligatori !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaCorr);
                    }
                    else
                    {
                        if (controllaEtichetta(txt_etichettaCorr.Text))
                        {
                            RegisterStartupScript("etichettaDuplicata", "<script>alert('Etichetta campo già esistente !');</script>");
                            return;
                        }

                        int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                        if (oggettoSelezionato != -1)
                        {
                            aggiungiValoreOggetto();
                        }
                        aggiungiComponenteDG();
                        dg_listaComponenti.SelectedIndex = -1;
                        txt_etichettaCorr.Text = "";
                        Panel_Personalizzazione.Visible = false;
                        lbl_avviso.Visible = false;
                        btn_anteprima.Visible = true;
                        btn_salvaTemplate.Visible = true;
                    }
                    break;
                case "btn_ConfermaPersLink":
                    //Controllo la presenza dei valori "< >"
                    if (txt_etichettaLink.Text.IndexOf("<") != -1 || txt_etichettaLink.Text.IndexOf(">") != -1)
                    {
                        lbl_avviso.Text = "Caratteri non permessi !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaLink);
                        return;
                    }

                    if (txt_etichettaLink.Text.TrimStart(' ').Equals(""))
                    {
                        lbl_avviso.Text = "Inserire i campi obbligatori !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaLink);
                    }
                    else
                    {
                        if (controllaEtichetta(txt_etichettaLink.Text))
                        {
                            RegisterStartupScript("etichettaDuplicata", "<script>alert('Etichetta campo già esistente !');</script>");
                            return;
                        }
                        int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                        if (oggettoSelezionato != -1)
                        {
                            aggiungiValoreOggetto();
                        }
                        aggiungiComponenteDG();
                        dg_listaComponenti.SelectedIndex = -1;
                        txt_etichettaLink.Text = "";
                        Panel_Personalizzazione.Visible = false;
                        lbl_avviso.Visible = false;
                        btn_anteprima.Visible = true;
                        btn_salvaTemplate.Visible = true;
                    }
                    break;
                case "btn_ConfermaPersContSottocontatore":

                    //Controllo la presenza dei valori "< >"
                    if (txt_etichettaContSottocontatore.Text.IndexOf("<") != -1 || txt_etichettaContSottocontatore.Text.IndexOf(">") != -1)
                    {
                        lbl_avviso.Text = "Caratteri non permessi !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaContSottocontatore);
                        return;
                    }

                    //Controllo che il formato del contatore contenga il valore CONTATORE che è obbligatorio
                    if (!lbl_formatoContSottocontatore.Text.Contains("CONTATORE"))
                    {
                        lbl_avviso.Text = "Nel formato è obbligatorio il campo CONTATORE !";
                        lbl_avviso.Visible = true;
                        return;
                    }

                    if (String.IsNullOrEmpty(txt_etichettaContSottocontatore.Text.TrimStart(' ')) || String.IsNullOrEmpty(txt_moduloSottoconotatore.Text.TrimStart(' ')))
                    {
                        lbl_avviso.Text = "Inserire i campi obbligatori !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaContSottocontatore);
                    }
                    else
                    {
                        if (controllaEtichetta(txt_etichettaContSottocontatore.Text))
                        {
                            RegisterStartupScript("etichettaDuplicata", "<script>alert('Etichetta campo già esistente !');</script>");
                            return;
                        }

                        int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                        if (oggettoSelezionato != -1)
                        {
                            aggiungiValoreOggetto();
                        }
                        aggiungiComponenteDG();
                        dg_listaComponenti.SelectedIndex = -1;
                        txt_etichettaContSottocontatore.Text = "";
                        Panel_Personalizzazione.Visible = false;
                        lbl_avviso.Visible = false;
                        btn_anteprima.Visible = true;
                        btn_salvaTemplate.Visible = true;
                    }
                    break;

                case "btn_ConfermaPersSeparatore":
                    //Controllo la presenza dei valori "< >"
                    if (txt_etichettaSeparatore.Text.IndexOf("<") != -1 || txt_etichettaSeparatore.Text.IndexOf(">") != -1)
                    {
                        lbl_avviso.Text = "Caratteri non permessi !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaSeparatore);
                        return;
                    }

                    if (String.IsNullOrEmpty(txt_etichettaSeparatore.Text.TrimStart(' ')))
                    {
                        lbl_avviso.Text = "Inserire i campi obbligatori !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaSeparatore);
                    }
                    else
                    {
                        if (controllaEtichetta(txt_etichettaSeparatore.Text))
                        {
                            RegisterStartupScript("etichettaDuplicata", "<script>alert('Etichetta campo già esistente !');</script>");
                            return;
                        }

                        int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                        if (oggettoSelezionato != -1)
                        {
                            aggiungiValoreOggetto();
                        }
                        aggiungiComponenteDG();
                        dg_listaComponenti.SelectedIndex = -1;
                        txt_etichettaSeparatore.Text = "";
                        Panel_Personalizzazione.Visible = false;
                        lbl_avviso.Visible = false;
                        btn_anteprima.Visible = true;
                        btn_salvaTemplate.Visible = true;
                    }
                    break;
                case "btn_ConfermaPersOggEsterno":
                    if (txt_etichettaOggEsterno.Text.IndexOf("<") != -1 || txt_etichettaOggEsterno.Text.IndexOf(">") != -1)
                    {
                        lbl_avviso.Text = "Caratteri non permessi !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaLink);
                        return;
                    }

                    if (txt_etichettaOggEsterno.Text.TrimStart(' ').Equals(""))
                    {
                        lbl_avviso.Text = "Inserire i campi obbligatori !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_etichettaLink);
                    }
                    else
                    {
                        ValidationResult valRes = intAdapter_OggEsterno.ValidateConfig();
                        if (!valRes.IsValid)
                        {
                            RegisterStartupScript("oggEsternoNonValido", "<script>alert(\"" + valRes.ErrorMessage + "\");</script>");
                            return;
                        }
                        int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                        if (oggettoSelezionato != -1)
                        {
                            aggiungiValoreOggetto();
                        }
                        aggiungiComponenteDG();
                        dg_listaComponenti.SelectedIndex = -1;
                        txt_etichettaLink.Text = "";
                        Panel_Personalizzazione.Visible = false;
                        lbl_avviso.Visible = false;
                        btn_anteprima.Visible = true;
                        btn_salvaTemplate.Visible = true;
                    }
                    break;
            }
        }

        private void pulsante_chiudi_pers_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Panel_Personalizzazione.Visible = false;
        }

        private bool controllaEtichetta(string etichetta)
        {
            if (Session["template"] != null)
            {
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
                {
                    if (dg_listaComponenti.SelectedIndex != -1 && i != dg_listaComponenti.SelectedIndex)
                    {
                        DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                        if (oggettoCustom.DESCRIZIONE == etichetta)
                            return true;
                    }
                }
            }
            return false;
        }

        #endregion btn_ConfermaPers_Click - pulsante_chiudi_pers_Click

        #region btn_ConfermaPrivato_Click - pulsante_chiudi_privato_Click
        private void btn_confermaPrivato_Click(object sender, System.EventArgs e)
        {
            //codice per salvare le info privato
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerPrivato"];
            string check = "0";
            if (cb_ModPrivato.Checked == true)
            {
                check = "1";
            }
            if (template.PRIVATO == null)
            {
                template.PRIVATO = "0";
            }
            if (!template.PRIVATO.Equals(check))
            {
                template.PRIVATO = check;
                //Controllo se esistono documenti con questa tipologia
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];
                int numDoc = ProfilazioneDocManager.countDocTipoDoc(template.ID_TIPO_ATTO, codiceAmministrazione, this);
                if (numDoc > 0)
                {
                    //se esistono documenti con tipologia documento selezionata si chiede 
                    //conferma all'utente 
                    string messaggio = DocsPAWA.InitMessageXml.getInstance().getMessage("PD_DocPrivato");
                    msg_Privato.Confirm(messaggio);
                }
                else
                {
                    ProfilazioneDocManager.updatePrivatoTipoDoc(template.SYSTEM_ID, check, this);
                    Panel_Privato.Visible = false;
                    popolaTemplateDG();
                }
            }
            else
            {
                Panel_Privato.Visible = false;
                dg_listaTemplates.SelectedIndex = -1;
            }
        }

        private void pulsante_chiudi_privato_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Panel_Privato.Visible = false;
            lbl_TipoDocPr.Text = "";
        }

        private void msg_Privato_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerPrivato"];
                ProfilazioneDocManager.updatePrivatoTipoDoc(template.SYSTEM_ID, template.PRIVATO, this);
                Panel_Privato.Visible = false;
                popolaTemplateDG();
            }
        }
        #endregion

        #region btn_confermaMesiCons_Click - pulsante_chiudi_MesiCons_Click
        private void btn_confermaMesiCons_Click(object sender, System.EventArgs e)
        {
            //codice per salvare le info riguardanti i mesi di conservazione
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerMesiCons"];
            string mesi = "0";
            if (!string.IsNullOrEmpty(this.txt_ModMesiCons.Text))
                mesi = this.txt_ModMesiCons.Text;


            if (!template.NUM_MESI_CONSERVAZIONE.Equals(mesi))
            {

                ProfilazioneDocManager.updateMesiConsTipoDoc(template.SYSTEM_ID, mesi, this);
                Panel_MesiCons.Visible = false;
                popolaTemplateDG();

            }
            else
            {
                Panel_MesiCons.Visible = false;
                dg_listaTemplates.SelectedIndex = -1;
            }
        }

        private void pulsante_chiudi_MesiCons_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Panel_MesiCons.Visible = false;
            txt_ModMesiCons.Text = "";
        }

        #endregion

        #region btn_confermaInvioCons_Click - pulsante_chiudi_InvioCons_Click

        private void btn_confermaInvioCons_Click(object sender, System.EventArgs e)
        {
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerInvioCons"];
            string invioCons = cb_ModInvioCons.Checked ? "1" : "0";

            if (string.IsNullOrEmpty(template.INVIO_CONSERVAZIONE))
                template.INVIO_CONSERVAZIONE = "0";

            if (!template.INVIO_CONSERVAZIONE.Equals(invioCons))
            {
                ProfilazioneDocManager.updateInvioConsTipoDoc(template.SYSTEM_ID, invioCons, this);
                Panel_InvioCons.Visible = false;
                popolaTemplateDG();
            }
            else
            {
                Panel_InvioCons.Visible = false;
                dg_listaTemplates.SelectedIndex = -1;
            }

        }

        private void pulsante_chiudi_InvioCons_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Panel_InvioCons.Visible = false;
            LblTipoDocInvioCons.Text = "";
        }

        #endregion

        #region btn consolidamento / conservazione

        private void btn_confermaConsolidaOgg_Click(object sender, System.EventArgs e)
        {
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
            DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[Convert.ToInt32(Session["temp_c1"])];

            string consolida = cb_ModConsolidaOgg.Checked ? "1" : "0";

            if (string.IsNullOrEmpty(oggettoCustom.CONSOLIDAMENTO))
                oggettoCustom.CONSOLIDAMENTO = "0";

            if (oggettoCustom.TIPO_OPERAZIONE.Equals("DaAggiungere"))
            {
                // se l'oggetto custom non è stato ancora aggiunto
                // modifico solamente il valore in sessione
                oggettoCustom.CONSOLIDAMENTO = consolida;

                // se consolidamento=0 devo imporre anche conservazione=0 e conservazione repertorio=0
                if (consolida.Equals("0"))
                {
                    oggettoCustom.CONSERVAZIONE = "0";
                    oggettoCustom.CONS_REPERTORIO = "0";
                    Label lbl_conserva = ((Label)dg_listaComponenti.Items[Convert.ToInt32(Session["temp_c1"])].Cells[4].Controls[3]);
                    lbl_conserva.Text = "NO";
                }

                Session["template"] = template;
                
                Label lbl_consolida = ((Label)dg_listaComponenti.Items[Convert.ToInt32(Session["temp_c1"])].Cells[3].Controls[3]);
                if (consolida.Equals("1"))
                    lbl_consolida.Text = "SI";
                else
                    lbl_consolida.Text = "NO";

                Panel_ConsolidaOgg.Visible = false;
                Session.Remove("temp_c1");
            }
            else
            {
                // se l'oggetto custom è già stato aggiunto
                // modifico il valore nel db (se è stato cambiato)
                if (!oggettoCustom.CONSOLIDAMENTO.Equals(consolida))
                {
                    oggettoCustom.CONSOLIDAMENTO = consolida;
                    
                    // se 0 devo impostare a 0 conservazione e cons repertorio
                    if (oggettoCustom.CONSOLIDAMENTO.Equals("0"))
                    {
                        ProfilazioneDocManager.updateConsolidamentoOggetto(oggettoCustom.SYSTEM_ID, consolida, string.Empty, this);
                        oggettoCustom.CONSERVAZIONE = "0";
                        oggettoCustom.CONS_REPERTORIO = "0";
                        Label lbl_conserva = ((Label)dg_listaComponenti.Items[Convert.ToInt32(Session["temp_c1"])].Cells[4].Controls[3]);
                        lbl_conserva.Text = "NO";
                    }
                    else
                    {
                        ProfilazioneDocManager.updateConsolidamentoOggetto(oggettoCustom.SYSTEM_ID, consolida, string.Empty, this);
                    }

                    Session["template"] = template;

                    Label lbl_consolida = ((Label)dg_listaComponenti.Items[Convert.ToInt32(Session["temp_c1"])].Cells[3].Controls[3]);
                    if (consolida.Equals("1"))
                        lbl_consolida.Text = "SI";
                    else
                        lbl_consolida.Text = "NO";

                    Panel_ConsolidaOgg.Visible = false;
                    Session.Remove("temp_c1");
                }
                else
                {
                    Panel_ConsolidaOgg.Visible = false;
                    Session.Remove("temp_c1");
                }
            }
        }

        private void pulsante_chiudi_OggConsol_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Panel_ConsolidaOgg.Visible = false;
            LblOggConsolidamento.Text = "";
            if (Session["temp_c1"] != null)
                Session.Remove("temp_c1");
        }

        private void btn_confermaConservaOgg_Click(object sender, System.EventArgs e)
        {

            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
            DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[Convert.ToInt32(Session["temp_c2"])];

            string conserva = cb_ModConservaOgg.Checked ? "1" : "0";

            if (string.IsNullOrEmpty(oggettoCustom.CONSERVAZIONE))
                oggettoCustom.CONSERVAZIONE = "0";

            if (oggettoCustom.TIPO_OPERAZIONE.Equals("DaAggiungere"))
            {
                // se l'oggetto custom non è stato ancora aggiunto
                // modifico solamente il valore in sessione
                oggettoCustom.CONSERVAZIONE = conserva;

                // se conservazione=1 devo imporre consolidamento=1
                if (conserva.Equals("1"))
                {
                    oggettoCustom.CONSOLIDAMENTO = "1";
                    Label lbl_consolida = ((Label)dg_listaComponenti.Items[Convert.ToInt32(Session["temp_c2"])].Cells[3].Controls[3]);
                    lbl_consolida.Text = "SI";
                    // verifico se è stato impostato invio_conservazione=1 per il template
                    if (template.INVIO_CONSERVAZIONE.Equals("0"))
                    {
                        template.INVIO_CONSERVAZIONE = "1";
                        Label lbl_invio = ((Label)dg_listaTemplates.SelectedItem.Cells[7].Controls[3]);
                        lbl_invio.Text = "SI";
                    }
                }
                else
                {
                    oggettoCustom.CONS_REPERTORIO = "0";
                }

                Session["template"] = template;
                Label lbl_conserva = ((Label)dg_listaComponenti.Items[Convert.ToInt32(Session["temp_c2"])].Cells[4].Controls[3]);
                if (conserva.Equals("1"))
                    lbl_conserva.Text = "SI";
                else
                    lbl_conserva.Text = "NO";
                Panel_ConservaOgg.Visible = false;
                Session.Remove("temp_c2");
            }
            else
            {
                // se l'oggetto custom è già stato aggiunto
                // modifico il valore nel db (se è stato cambiato)
                if (!oggettoCustom.CONSERVAZIONE.Equals(conserva))
                {
                    oggettoCustom.CONSERVAZIONE = conserva;
                    //ProfilazioneDocManager.updateConservazioneOggetto(oggettoCustom.SYSTEM_ID, conserva, this);
                    if (oggettoCustom.CONSERVAZIONE.Equals("1"))
                    {
                        if (template.INVIO_CONSERVAZIONE.Equals("1"))
                        {
                            ProfilazioneDocManager.updateConservazioneOggetto(oggettoCustom.SYSTEM_ID, conserva, string.Empty, this);
                        }
                        else
                        {
                            template.INVIO_CONSERVAZIONE = "1";
                            ProfilazioneDocManager.updateConservazioneOggetto(oggettoCustom.SYSTEM_ID, conserva, template.SYSTEM_ID.ToString(), this);
                            Label lbl_invio = ((Label)dg_listaTemplates.SelectedItem.Cells[7].Controls[3]);
                            lbl_invio.Text = "SI";
                        }
                        oggettoCustom.CONSOLIDAMENTO = "1";
                        Label lbl_consolida = ((Label)dg_listaComponenti.Items[Convert.ToInt32(Session["temp_c2"])].Cells[3].Controls[3]);
                        lbl_consolida.Text = "SI";
                    }
                    else
                    {
                        oggettoCustom.CONS_REPERTORIO = "0";
                    }

                    Session["template"] = template;

                    Label lbl_conserva = ((Label)dg_listaComponenti.Items[Convert.ToInt32(Session["temp_c2"])].Cells[4].Controls[3]);
                    if (conserva.Equals("1"))
                        lbl_conserva.Text = "SI";
                    else
                        lbl_conserva.Text = "NO";

                    Panel_ConservaOgg.Visible = false;
                    Session.Remove("temp_c2");
                }
                else
                {
                    Panel_ConservaOgg.Visible = false;
                    if (Session["temp_c2"] != null)
                        Session.Remove("temp_c2");
                }
            }
        }

        private void pulsante_chiudi_OggConserv_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Panel_ConservaOgg.Visible = false;
            LblOggConservazione.Text = "";
        }

        #endregion

        #region aggiungiTemplateInSessione - aggiornaTemplateInSessione - aggiungiOggettoTemplate - aggiungiValoreOggetto -setElencoValori
        public void aggiungiTemplateInSessione(string tipoOggettoParam)
        {
            //Questo metodo viene chiamato solo quando si aggiunge per la prima volta un componente
            //ad un modello. Infatti viene effettuata la creazione di un nuovo template e aggiunto
            //a quest'ultimo il componente scelto. Infine il template cosi' costituito viene messo in sessione.
            //Il nome dell'oggettoCustom è un numero progressivo conservato in sessione.
            DocsPAWA.DocsPaWR.Templates template = new DocsPAWA.DocsPaWR.Templates();
            template.DESCRIZIONE = txt_TipoDocumento.Text;
            if (cb_Privato.Checked)
            {
                template.PRIVATO = "1";
            }
            else
            {
                template.PRIVATO = "0";
            }

            //Session.Add("nomeCampo",0);
            DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = new DocsPAWA.DocsPaWR.OggettoCustom();
            DocsPAWA.DocsPaWR.TipoOggetto tipoOggetto = new DocsPAWA.DocsPaWR.TipoOggetto();
            tipoOggetto.DESCRIZIONE_TIPO = tipoOggettoParam;

            oggettoCustom.TIPO = tipoOggetto;
            if (dg_listaTemplates.Visible)
            {
                oggettoCustom.TIPO_OPERAZIONE = "DaAggiungere";
            }

            Session.Add("template", ProfilazioneDocManager.aggiungiOggettoCustomTemplate(oggettoCustom, template, this));
        }

        public void aggiornaTemplateInSessione()
        {
            //Questo metodo ripulisce il template da quegli oggetti che sono rimasti in lavorazione
            //e che l'utente non vuole salvare. 
            //Oggetti identificati dal fatto che vengono inseriti nel dataGrid "dg_listaComponenti" 
            //con la cella del campo Ordinamento vuota.
            for (int i = 0; i < dg_listaComponenti.Items.Count; i++)
            {
                if (dg_listaComponenti.Items[i].Cells[0].Text == "&nbsp;" && ((DocsPAWA.DocsPaWR.Templates)Session["template"]) != null)
                {
                    DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                    Session.Add("template", ProfilazioneDocManager.eliminaOggettoCustomTemplate(template, i, this));
                    aggiungiComponenteDG();
                }
            }
        }

        public void aggiungiOggettoTemplate(string tipoOggettoParam)
        {
            //Questo metodo viene chiamato ogni qualvolta si desidera aggiungere un componente ad un template.
            //Viene chiamato il metodo del backend che permette l'aggiunta di un oggetto allo specifico template 
            //e che restituisce a sua volta il template aggiornato che viene rimesso in sessione.
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
            DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = new DocsPAWA.DocsPaWR.OggettoCustom();
            DocsPAWA.DocsPaWR.TipoOggetto tipoOggetto = new DocsPAWA.DocsPaWR.TipoOggetto();
            tipoOggetto.DESCRIZIONE_TIPO = tipoOggettoParam;

            oggettoCustom.TIPO = tipoOggetto;
            if (dg_listaTemplates.Visible)
            {
                oggettoCustom.TIPO_OPERAZIONE = "DaAggiungere";
            }

            //Verifico se sto aggiungendo un oggettoCustom ad un iperfascicolo
            //in questo caso va settata la proprietà CAMPO_COMUNE ad "1"
            //altrimenti si tratta di un oggettoCustom di una tipologia normale
            //e non è necessario indicare nulla nella suddetta proprietà
            if (template.IPER_FASC_DOC != null && template.IPER_FASC_DOC == "1")
            {
                oggettoCustom.CAMPO_COMUNE = "1";
            }

            Session.Add("template", ProfilazioneDocManager.aggiungiOggettoCustomTemplate(oggettoCustom, template, this));
        }

        public void aggiungiValoreOggetto()
        {
            //Questo metodo viene chiamato ogni qualvolta si desidera aggiungere un valore ad un componente,
            //solo etichetta o etichetta e valori.
            //Viene chiamato il metodo del backend che permette l'aggiunta di un valore allo specifico oggetto 
            //e che restituisce a sua volta il template aggiornato che viene rimesso in sessione.
            //Le chiamate di aggiunta valori vengo fatte a seconda del tipo di oggetto e con il parametro di cancellazione
            //a "true" o "false", a seconda della necessità.
            int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
            string anno = System.DateTime.Now.Year.ToString();

            if (oggettoSelezionato != -1)
            {
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                //modifica
                foreach (DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom1 in template.ELENCO_OGGETTI)
                    oggettoCustom1.DA_VISUALIZZARE_RICERCA = "0";
                //

                DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[oggettoSelezionato];
                DocsPAWA.DocsPaWR.TipoOggetto tipoOggetto = (DocsPAWA.DocsPaWR.TipoOggetto)oggettoCustom.TIPO;
                DocsPAWA.DocsPaWR.ValoreOggetto valoreOggetto = new DocsPAWA.DocsPaWR.ValoreOggetto();
                switch (tipoOggetto.DESCRIZIONE_TIPO)
                {
                    case "CampoDiTesto":
                        oggettoCustom.ANNO = anno;
                        oggettoCustom.POSIZIONE = Convert.ToString(dg_listaComponenti.SelectedIndex + 1);
                        oggettoCustom.DESCRIZIONE = txt_etichettaCampoDiTesto.Text;
                        oggettoCustom.VALORE_DATABASE = txt_TestoDefaultCampoTesto.Text;
                        oggettoCustom.NUMERO_DI_CARATTERI = txt_NumeroCaratteri_CampoDiTesto.Text;
                        oggettoCustom.NUMERO_DI_LINEE = txt_NumeroLinee_CampoDiTesto.Text;
                        oggettoCustom.TIPO_RICERCA_CORR = "0";
                        oggettoCustom.ID_RUOLO_DEFAULT = "0";
                        //Di default i seguenti valori MULTILINEA, CAMPO DI RICERCA e CAMPO OBBLIGATORIO sono una stringa vuota
                        if (cb_Multilinea_CampoDiTesto.Checked)
                        {
                            oggettoCustom.MULTILINEA = "SI";
                        }
                        else
                        {
                            oggettoCustom.MULTILINEA = "NO";
                        }
                        if (cb_Ricerca_CampoDiTesto.Checked)
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "NO";
                        }
                        if (cb_Obbligatorio_CampoDiTesto.Checked)
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                        }
                        //Verifica se si sta modificando un template esistente, in caso affermativo
                        //aggiungo all'oggetto custom il tipo di operazione da effettuare quando
                        //verra' fatto l'update del template
                        if (dg_listaTemplates.Visible && oggettoCustom.TIPO_OPERAZIONE.Equals(""))
                        {
                            oggettoCustom.TIPO_OPERAZIONE = "DaAggiornare";
                        }
                        Session.Add("template", template);
                        break;
                    case "MenuATendina":
                        oggettoCustom.ANNO = anno;
                        oggettoCustom.POSIZIONE = Convert.ToString(dg_listaComponenti.SelectedIndex + 1);
                        oggettoCustom.DESCRIZIONE = txt_etichettaMenuATendina.Text;
                        oggettoCustom.TIPO_RICERCA_CORR = "0";
                        oggettoCustom.ID_RUOLO_DEFAULT = "0";
                        if (cb_Obbligatorio_MenuATendina.Checked)
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                        }
                        if (cb_Ricerca_MenuATendina.Checked)
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "NO";
                        }
                        //Verifica se si sta modificando un template esistente, in caso affermativo
                        //aggiungo all'oggetto custom il tipo di operazione da effettuare quando
                        //verra' fatto l'update del template
                        if (dg_listaTemplates.Visible && oggettoCustom.TIPO_OPERAZIONE.Equals(""))
                        {
                            oggettoCustom.TIPO_OPERAZIONE = "DaAggiornare";
                        }

                        //Impostazione elenco dei valori
                        ArrayList newArrayValoriMenuATendina = new ArrayList();
                        if (oggettoCustom.ELENCO_VALORI.Length == 0)
                        {
                            oggettoCustom.ELENCO_VALORI = new DocsPAWA.DocsPaWR.ValoreOggetto[1];
                            oggettoCustom.ELENCO_VALORI[0] = new DocsPAWA.DocsPaWR.ValoreOggetto();
                        }

                        //Inserisco il colore al multivalore


                        for (int i = 0; i < ddl_valoriMenuATendina.Items.Count; i++)
                        {
                            for (int j = 0; j < oggettoCustom.ELENCO_VALORI.Length; j++)
                            {
                                if (ddl_valoriMenuATendina.Items[i].Text == oggettoCustom.ELENCO_VALORI[j].VALORE)
                                {
                                    newArrayValoriMenuATendina.Add(oggettoCustom.ELENCO_VALORI[j]);
                                    break;
                                }
                                if (j + 1 == oggettoCustom.ELENCO_VALORI.Length)
                                {
                                    DocsPAWA.DocsPaWR.ValoreOggetto valoreOggettoNuovo = new DocsPAWA.DocsPaWR.ValoreOggetto();
                                    valoreOggettoNuovo.DESCRIZIONE_VALORE = "Valore" + (ddl_valoriMenuATendina.SelectedIndex + 1);
                                    valoreOggettoNuovo.VALORE = ddl_valoriMenuATendina.Items[i].Text;
                                    valoreOggettoNuovo.VALORE_DI_DEFAULT = "NO";
                                    valoreOggettoNuovo.COLOR_BG = this.DdlColoriMenu.SelectedValue;
                                    newArrayValoriMenuATendina.Add(valoreOggettoNuovo);
                                }
                            }
                        }
                        oggettoCustom.ELENCO_VALORI = new DocsPAWA.DocsPaWR.ValoreOggetto[newArrayValoriMenuATendina.Count];
                        newArrayValoriMenuATendina.CopyTo(oggettoCustom.ELENCO_VALORI);
                        break;
                    case "SelezioneEsclusiva":
                        oggettoCustom.ANNO = anno;
                        oggettoCustom.POSIZIONE = Convert.ToString(dg_listaComponenti.SelectedIndex + 1);
                        oggettoCustom.DESCRIZIONE = txt_etichettaSelezioneEsclusiva.Text;
                        oggettoCustom.ORIZZONTALE_VERTICALE = rd_VerOri_SelezioneEsclusiva.SelectedValue;
                        oggettoCustom.TIPO_RICERCA_CORR = "0";
                        oggettoCustom.ID_RUOLO_DEFAULT = "0";
                        if (cb_Obbligatorio_SelezioneEsclusiva.Checked)
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                        }
                        if (cb_Ricerca_SelezioneEsclusiva.Checked)
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "NO";
                        }
                        //Verifica se si sta modificando un template esistente, in caso affermativo
                        //aggiungo all'oggetto custom il tipo di operazione da effettuare quando
                        //verra' fatto l'update del template
                        if (dg_listaTemplates.Visible && oggettoCustom.TIPO_OPERAZIONE.Equals(""))
                        {
                            oggettoCustom.TIPO_OPERAZIONE = "DaAggiornare";
                        }

                        //Impostazione elenco dei valori
                        ArrayList newArrayValoriSelezioneEsclusiva = new ArrayList();
                        if (oggettoCustom.ELENCO_VALORI.Length == 0)
                        {
                            oggettoCustom.ELENCO_VALORI = new DocsPAWA.DocsPaWR.ValoreOggetto[1];
                            oggettoCustom.ELENCO_VALORI[0] = new DocsPAWA.DocsPaWR.ValoreOggetto();
                        }
                        for (int i = 0; i < ddl_valoriSelezioneEsclusiva.Items.Count; i++)
                        {
                            for (int j = 0; j < oggettoCustom.ELENCO_VALORI.Length; j++)
                            {
                                if (ddl_valoriSelezioneEsclusiva.Items[i].Text == oggettoCustom.ELENCO_VALORI[j].VALORE)
                                {
                                    newArrayValoriSelezioneEsclusiva.Add(oggettoCustom.ELENCO_VALORI[j]);
                                    break;
                                }
                                if (j + 1 == oggettoCustom.ELENCO_VALORI.Length)
                                {
                                    DocsPAWA.DocsPaWR.ValoreOggetto valoreOggettoNuovo = new DocsPAWA.DocsPaWR.ValoreOggetto();
                                    valoreOggettoNuovo.DESCRIZIONE_VALORE = "Valore" + (ddl_valoriSelezioneEsclusiva.SelectedIndex + 1);
                                    valoreOggettoNuovo.VALORE = ddl_valoriSelezioneEsclusiva.Items[i].Text;
                                    valoreOggettoNuovo.VALORE_DI_DEFAULT = "NO";
                                    valoreOggettoNuovo.COLOR_BG = this.DdlColoriSelezioneEsclusiva.SelectedValue;
                                    newArrayValoriSelezioneEsclusiva.Add(valoreOggettoNuovo);
                                }
                            }
                        }
                        oggettoCustom.ELENCO_VALORI = new DocsPAWA.DocsPaWR.ValoreOggetto[newArrayValoriSelezioneEsclusiva.Count];
                        newArrayValoriSelezioneEsclusiva.CopyTo(oggettoCustom.ELENCO_VALORI);
                        break;
                    case "Contatore":
                        oggettoCustom.ANNO = anno;
                        oggettoCustom.POSIZIONE = Convert.ToString(dg_listaComponenti.SelectedIndex + 1);
                        oggettoCustom.DESCRIZIONE = txt_etichettaContatore.Text;
                        oggettoCustom.TIPO_RICERCA_CORR = "0";
                        oggettoCustom.ID_RUOLO_DEFAULT = "0";
                        oggettoCustom.DATA_INIZIO = DataInizio.Text;
                        oggettoCustom.DATA_FINE = DataFine.Text;
                        if (cb_Ricerca_Contatore.Checked)
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "NO";
                        }

                        if (cb_Azzera_Anno.Checked)
                        {
                            oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO = "SI";
                        }
                        else
                        {
                            oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO = "NO";
                        }

                        if (txt_formatoContatore.Text != "")
                            oggettoCustom.FORMATO_CONTATORE = txt_formatoContatore.Text;

                        if (cb_ContaDopo.Checked)
                            oggettoCustom.CONTA_DOPO = "1";
                        else
                            oggettoCustom.CONTA_DOPO = "0";

                        if (cb_Repertorio.Checked)
                            oggettoCustom.REPERTORIO = "1";
                        else
                            oggettoCustom.REPERTORIO = "0";

                        if (cb_consRep.Checked)
                        {
                            oggettoCustom.CONS_REPERTORIO = "1";
                            // se il contatore è individuato come di repertorio
                            // l'oggetto deve essere soggetto a consolidamento e conservazione
                            // e la tipologia deve essere configurata come soggetta a conservazione
                            oggettoCustom.CONSOLIDAMENTO = "1";
                            oggettoCustom.CONSERVAZIONE = "1";
                            template.INVIO_CONSERVAZIONE = "1";
                            Label lbl_consolida = ((Label)dg_listaComponenti.Items[dg_listaComponenti.SelectedIndex].Cells[3].Controls[3]);
                            if(lbl_consolida != null)
                                lbl_consolida.Text = "SI";
                            Label lbl_conserva = ((Label)dg_listaComponenti.Items[dg_listaComponenti.SelectedIndex].Cells[4].Controls[3]);
                            if(lbl_conserva != null)
                                lbl_conserva.Text = "SI";
                            Label lbl_invio = ((Label)dg_listaTemplates.SelectedItem.Cells[7].Controls[3]);
                            lbl_invio.Text = "SI";
                        }
                        else
                            oggettoCustom.CONS_REPERTORIO = "0";


                        switch (rbl_tipoContatore.SelectedValue)
                        {
                            case "Tipologia":
                                oggettoCustom.TIPO_CONTATORE = "T";
                                break;
                            case "AOO":
                                oggettoCustom.TIPO_CONTATORE = "A";
                                break;
                            case "RF":
                                oggettoCustom.TIPO_CONTATORE = "R";
                                break;
                        }


                        //MODIFICA

                        if (cb_Contatore_visibile.Checked)
                            oggettoCustom.DA_VISUALIZZARE_RICERCA = "1";
                        else
                            oggettoCustom.DA_VISUALIZZARE_RICERCA = "0";
                        //fine modifica

                        //Verifica se si sta modificando un template esistente, in caso affermativo
                        //aggiungo all'oggetto custom il tipo di operazione da effettuare quando
                        //verra' fatto l'update del template
                        if (dg_listaTemplates.Visible && oggettoCustom.TIPO_OPERAZIONE.Equals(""))
                        {
                            oggettoCustom.TIPO_OPERAZIONE = "DaAggiornare";
                        }

                        //Controllo la presenza di un campo "Contatore" per tipologia
                        //se voglio aggiungere un altro contatore per tipologia
                        if (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.TIPO_OPERAZIONE != "DaAggiornare")
                        {
                            for (int i = 0; i < template.ELENCO_OGGETTI.Length - 1; i++)
                            {
                                DocsPAWA.DocsPaWR.OggettoCustom ogg = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                                if (ogg.TIPO.DESCRIZIONE_TIPO == "Contatore" && ogg.TIPO_CONTATORE == "T")
                                {
                                    RegisterStartupScript("contatoreEsistente", "<script>alert('Un campo di tipo \"Contatore\" per tipologia, già esiste !');</script>");
                                    Panel_Personalizzazione.Visible = false;
                                    aggiornaTemplateInSessione();
                                    return;
                                }
                            }
                        }

                        // Non possono esistere più contatori di repertorio
                        int numRep = template.ELENCO_OGGETTI.Count(o => o.REPERTORIO == "1");
                        if (numRep > 1)
                        {

                            oggettoCustom.REPERTORIO = "0";
                            ClientScript.RegisterStartupScript(this.GetType(), "contatoreDiRepEsistente", "alert('Un campo di tipo \"Contatore\" di repertorio, già esiste !');", true);
                            Panel_Personalizzazione.Visible = false;
                            aggiornaTemplateInSessione();
                            return;
                        }

                        ////MODIFICA
                        //if (cb_ContSottocontatore_visibile.Checked)
                        //    oggettoCustom.DA_VISUALIZZARE_RICERCA = "1";
                        //else
                        //    oggettoCustom.DA_VISUALIZZARE_RICERCA = "0";
                        ////fine modifica

                        //Verifica se si sta modificando un template esistente, in caso affermativo
                        //aggiungo all'oggetto custom il tipo di operazione da effettuare quando
                        //verra' fatto l'update del template
                        if (dg_listaTemplates.Visible && oggettoCustom.TIPO_OPERAZIONE.Equals(""))
                        {
                            oggettoCustom.TIPO_OPERAZIONE = "DaAggiornare";
                        }

                        Session.Add("template", template);
                        break;
                    case "CasellaDiSelezione":
                        oggettoCustom.ANNO = anno;
                        oggettoCustom.POSIZIONE = Convert.ToString(dg_listaComponenti.SelectedIndex + 1);
                        oggettoCustom.DESCRIZIONE = txt_etichettaCasellaDiSelezione.Text;
                        oggettoCustom.ORIZZONTALE_VERTICALE = rd_VerOri_CasellaSelezione.SelectedValue;
                        oggettoCustom.TIPO_RICERCA_CORR = "0";
                        oggettoCustom.ID_RUOLO_DEFAULT = "0";
                        if (cb_Obbligatorio_CasellaDiSelezione.Checked)
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                        }
                        if (cb_Ricerca_CasellaDiSelezione.Checked)
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "NO";
                        }
                        //Verifica se si sta modificando un template esistente, in caso affermativo
                        //aggiungo all'oggetto custom il tipo di operazione da effettuare quando
                        //verra' fatto l'update del template
                        if (dg_listaTemplates.Visible && oggettoCustom.TIPO_OPERAZIONE.Equals(""))
                        {
                            oggettoCustom.TIPO_OPERAZIONE = "DaAggiornare";
                        }

                        //Impostazione elenco dei valori
                        ArrayList newArrayValoriCasellaDiSelezione = new ArrayList();
                        if (oggettoCustom.ELENCO_VALORI.Length == 0)
                        {
                            oggettoCustom.ELENCO_VALORI = new DocsPAWA.DocsPaWR.ValoreOggetto[1];
                            oggettoCustom.ELENCO_VALORI[0] = new DocsPAWA.DocsPaWR.ValoreOggetto();
                        }
                        for (int i = 0; i < ddl_valoriCasellaSelezione.Items.Count; i++)
                        {
                            for (int j = 0; j < oggettoCustom.ELENCO_VALORI.Length; j++)
                            {
                                if (ddl_valoriCasellaSelezione.Items[i].Text == oggettoCustom.ELENCO_VALORI[j].VALORE)
                                {
                                    newArrayValoriCasellaDiSelezione.Add(oggettoCustom.ELENCO_VALORI[j]);
                                    break;
                                }
                                if (j + 1 == oggettoCustom.ELENCO_VALORI.Length)
                                {
                                    DocsPAWA.DocsPaWR.ValoreOggetto valoreOggettoNuovo = new DocsPAWA.DocsPaWR.ValoreOggetto();
                                    valoreOggettoNuovo.DESCRIZIONE_VALORE = "Valore" + (ddl_valoriCasellaSelezione.SelectedIndex + 1);
                                    valoreOggettoNuovo.VALORE = ddl_valoriCasellaSelezione.Items[i].Text;
                                    valoreOggettoNuovo.VALORE_DI_DEFAULT = "NO";
                                    newArrayValoriCasellaDiSelezione.Add(valoreOggettoNuovo);
                                }
                            }
                        }
                        oggettoCustom.ELENCO_VALORI = new DocsPAWA.DocsPaWR.ValoreOggetto[newArrayValoriCasellaDiSelezione.Count];
                        newArrayValoriCasellaDiSelezione.CopyTo(oggettoCustom.ELENCO_VALORI);
                        break;
                    case "Data":
                        oggettoCustom.ANNO = anno;
                        oggettoCustom.POSIZIONE = Convert.ToString(dg_listaComponenti.SelectedIndex + 1);
                        oggettoCustom.DESCRIZIONE = txt_etichettaData.Text;
                        oggettoCustom.TIPO_RICERCA_CORR = "0";
                        oggettoCustom.ID_RUOLO_DEFAULT = "0";
                        if (cb_Ricerca_Data.Checked)
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "NO";
                        }
                        if (cb_Obbligatorio_Data.Checked)
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                        }
                        //Verifica se si sta modificando un template esistente, in caso affermativo
                        //aggiungo all'oggetto custom il tipo di operazione da effettuare quando
                        //verra' fatto l'update del template
                        if (dg_listaTemplates.Visible && oggettoCustom.TIPO_OPERAZIONE.Equals(""))
                        {
                            oggettoCustom.TIPO_OPERAZIONE = "DaAggiornare";
                        }
                        oggettoCustom.FORMATO_ORA = ddl_formatoOra.SelectedValue;
                        Session.Add("template", template);
                        break;
                    case "Corrispondente":
                        oggettoCustom.ANNO = anno;
                        oggettoCustom.POSIZIONE = Convert.ToString(dg_listaComponenti.SelectedIndex + 1);
                        oggettoCustom.DESCRIZIONE = txt_etichettaCorr.Text;
                        if (cb_Ricerca_Corr.Checked)
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "NO";
                        }
                        if (cb_Obbligatorio_Corr.Checked)
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                        }
                        if (ddl_tipoRicerca.SelectedIndex != 0)
                        {
                            oggettoCustom.TIPO_RICERCA_CORR = ddl_tipoRicerca.SelectedItem.Text;
                        }
                        else
                        {
                            oggettoCustom.TIPO_RICERCA_CORR = "0";
                        }
                        if (ddl_ruoloPredefinito.SelectedIndex != 0 && ddl_ruoloPredefinito.SelectedIndex != -1)
                        {
                            oggettoCustom.ID_RUOLO_DEFAULT = ddl_ruoloPredefinito.SelectedValue;
                        }
                        else
                        {
                            oggettoCustom.ID_RUOLO_DEFAULT = "0";
                        }
                        //Verifica se si sta modificando un template esistente, in caso affermativo
                        //aggiungo all'oggetto custom il tipo di operazione da effettuare quando
                        //verra' fatto l'update del template
                        if (dg_listaTemplates.Visible && oggettoCustom.TIPO_OPERAZIONE.Equals(""))
                        {
                            oggettoCustom.TIPO_OPERAZIONE = "DaAggiornare";
                        }
                        Session.Add("template", template);
                        break;
                    case "Link":
                        oggettoCustom.ANNO = anno;
                        oggettoCustom.POSIZIONE = Convert.ToString(dg_listaComponenti.SelectedIndex + 1);
                        oggettoCustom.DESCRIZIONE = txt_etichettaLink.Text;
                        oggettoCustom.TIPO_LINK = ddl_tipoLink.SelectedValue;
                        oggettoCustom.ID_RUOLO_DEFAULT = "0";
                        if (oggettoCustom.TIPO_LINK.Equals("INTERNO"))
                        {
                            oggettoCustom.TIPO_OBJ_LINK = ddl_tipoObjLink.SelectedValue;
                        }
                        else
                        {
                            oggettoCustom.TIPO_OBJ_LINK = null;
                        }
                        if (cb_obbligatorioLink.Checked)
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                        }
                        if (dg_listaTemplates.Visible && oggettoCustom.TIPO_OPERAZIONE.Equals(""))
                        {
                            oggettoCustom.TIPO_OPERAZIONE = "DaAggiornare";
                        }
                        Session.Add("template", template);
                        break;
                    case "ContatoreSottocontatore":
                        oggettoCustom.ANNO = anno;
                        oggettoCustom.POSIZIONE = Convert.ToString(dg_listaComponenti.SelectedIndex + 1);
                        oggettoCustom.DESCRIZIONE = txt_etichettaContSottocontatore.Text;
                        oggettoCustom.TIPO_RICERCA_CORR = "0";
                        oggettoCustom.ID_RUOLO_DEFAULT = "0";
                        oggettoCustom.MODULO_SOTTOCONTATORE = txt_moduloSottoconotatore.Text;
                        if (cb_Ricerca_ContSottocontatore.Checked)
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "NO";
                        }

                        if (cb_Azzera_AnnoContSottocontatore.Checked)
                        {
                            oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO = "SI";
                        }
                        else
                        {
                            oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO = "NO";
                        }

                        if (lbl_formatoContSottocontatore.Text != "")
                            oggettoCustom.FORMATO_CONTATORE = lbl_formatoContSottocontatore.Text;

                        if (cb_ContaDopoContSottocontatore.Checked)
                            oggettoCustom.CONTA_DOPO = "1";
                        else
                            oggettoCustom.CONTA_DOPO = "0";

                        //if (cb_RepertorioContSottocontatore.Checked)
                        //    oggettoCustom.REPERTORIO = "1";
                        //else
                        //    oggettoCustom.REPERTORIO = "0";

                        switch (rbl_tipoContSottocontatore.SelectedValue)
                        {
                            case "Tipologia":
                                oggettoCustom.TIPO_CONTATORE = "T";
                                break;
                            case "AOO":
                                oggettoCustom.TIPO_CONTATORE = "A";
                                break;
                            case "RF":
                                oggettoCustom.TIPO_CONTATORE = "R";
                                break;
                        }

                        //Verifica se si sta modificando un template esistente, in caso affermativo
                        //aggiungo all'oggetto custom il tipo di operazione da effettuare quando
                        //verra' fatto l'update del template
                        if (dg_listaTemplates.Visible && oggettoCustom.TIPO_OPERAZIONE.Equals(""))
                        {
                            oggettoCustom.TIPO_OPERAZIONE = "DaAggiornare";
                        }
                        break;
                    case "OggettoEsterno":
                        oggettoCustom.ANNO = anno;
                        oggettoCustom.POSIZIONE = Convert.ToString(dg_listaComponenti.SelectedIndex + 1);
                        oggettoCustom.DESCRIZIONE = txt_etichettaOggEsterno.Text;
                        oggettoCustom.ID_RUOLO_DEFAULT = "0";
                        oggettoCustom.CONFIG_OBJ_EST = this.intAdapter_OggEsterno.ConfigurationValue;
                        if (cb_obbligatorioOggEsterno.Checked)
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                        }
                        if (cb_ricercaOggEsterno.Checked)
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "SI";
                        }
                        else
                        {
                            oggettoCustom.CAMPO_DI_RICERCA = "NO";
                        }
                        if (dg_listaTemplates.Visible && oggettoCustom.TIPO_OPERAZIONE.Equals(""))
                        {
                            oggettoCustom.TIPO_OPERAZIONE = "DaAggiornare";
                        }
                        Session.Add("template", template);
                        break;

                    case "Separatore":
                        oggettoCustom.ANNO = anno;
                        oggettoCustom.POSIZIONE = Convert.ToString(dg_listaComponenti.SelectedIndex + 1);
                        oggettoCustom.DESCRIZIONE = txt_etichettaSeparatore.Text;
                        oggettoCustom.TIPO_RICERCA_CORR = "0";
                        oggettoCustom.ID_RUOLO_DEFAULT = "0";

                        //Verifica se si sta modificando un template esistente, in caso affermativo
                        //aggiungo all'oggetto custom il tipo di operazione da effettuare quando
                        //verra' fatto l'update del template
                        if (dg_listaTemplates.Visible && oggettoCustom.TIPO_OPERAZIONE.Equals(""))
                        {
                            oggettoCustom.TIPO_OPERAZIONE = "DaAggiornare";
                        }
                        Session.Add("template", template);
                        break;
                }
            }
        }
        #endregion aggiungiTemplateInSessione - aggiornaTemplateInSessione - aggiungiOggettoTemplate - aggiungiValoreOggetto -setElencoValori

        #region aggiungi-elimina ValoriSelezioneEsclusiva
        private void btn_aggiungiValoreSelezioneEsclusiva_Click(object sender, System.EventArgs e)
        {
            if (controllaValoriDuplicati(ddl_valoriSelezioneEsclusiva, txt_valoreSelezioneEsclusiva.Text))
            {
                RegisterStartupScript("valoreNonConsentito", "<script>alert('Il valore già esiste !'); </script>");
                return;
            }

            if (txt_valoreSelezioneEsclusiva.Text.IndexOf("-") != -1)
            {
                RegisterStartupScript("carattereNonConsetito", "<script>alert('Il carattere \"-\" non è consentito !'); </script>");
                return;
            }

            //Metodo che aggiunge un nuovo valore, alla ddl dei valori del componente "SelezioneEsclusiva".
            if (txt_valoreSelezioneEsclusiva.Text.TrimStart(' ').Equals("") || txt_etichettaSelezioneEsclusiva.Text.TrimStart(' ').Equals(""))
            {
                lbl_avviso.Text = "Inserire i campi obbligatori !";
                lbl_avviso.Visible = true;
            }
            else
            {
                lbl_avviso.Visible = false;
                ddl_valoriSelezioneEsclusiva.Items.Add(new ListItem(txt_valoreSelezioneEsclusiva.Text, txt_valoreSelezioneEsclusiva.Text));
                ddl_valoriSelezioneEsclusiva.SelectedIndex = ddl_valoriSelezioneEsclusiva.Items.Count - 1;
                txt_valoreSelezioneEsclusiva.Text = "";
                cb_Default_SelezioneEsclusiva.Checked = false;
                cb_Disabilitato_SelezioneEsclusiva.Checked = false;
                this.DdlColoriSelezioneEsclusiva.SelectedIndex = 0;
                aggiungiValoreOggetto();
            }
            SetFocus(txt_valoreSelezioneEsclusiva);
        }

        private void btn_elimnaValoreSelezioneEsclusiva_Click(object sender, System.EventArgs e)
        {
            //Metodo che rimuove un valore, dalla ddl dei valori del componente "SelezioneEsclusiva".
            if (ddl_valoriSelezioneEsclusiva.Items.Count != 0)
            {
                //Controllo per verificare se è possibile la rimozione del valore
                //Basta che un documento abbia questo valore attribuito che la rimozione non è possibile
                int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                if (oggettoSelezionato != -1)
                {
                    DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                    DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[oggettoSelezionato];
                    if (ProfilazioneDocManager.isValueInUse(Convert.ToString(oggettoCustom.SYSTEM_ID), Convert.ToString(template.SYSTEM_ID), ddl_valoriSelezioneEsclusiva.SelectedItem.Text, this))
                    {
                        RegisterStartupScript("cancellazioneImpossibile", "<script>alert('Valore assegnato ad un documento ! \\nCancellazione impossible!');</script>");
                        return;
                    }
                }

                ddl_valoriSelezioneEsclusiva.Items.RemoveAt(ddl_valoriSelezioneEsclusiva.SelectedIndex);
                cb_Default_SelezioneEsclusiva.Checked = false;
                cb_Disabilitato_SelezioneEsclusiva.Checked = false;
                this.DdlColoriSelezioneEsclusiva.SelectedIndex = 0;
            }
        }
        #endregion aggiungi-elimina ValoriSelezioneEsclusiva

        #region aggiungi-elimnina ValoriCasellaSelezione
        private void btn_aggiungiValoreCasellaSelezione_Click(object sender, System.EventArgs e)
        {
            if (controllaValoriDuplicati(ddl_valoriCasellaSelezione, txt_valoreCasellaSelezione.Text))
            {
                RegisterStartupScript("valoreNonConsentito", "<script>alert('Il valore già esiste !'); </script>");
                return;
            }

            if (txt_valoreCasellaSelezione.Text.IndexOf("-") != -1)
            {
                RegisterStartupScript("carattereNonConsetito", "<script>alert('Il carattere \"-\" non è consentito !'); </script>");
                return;
            }

            if (txt_valoreCasellaSelezione.Text.TrimStart(' ').Equals("") || txt_etichettaCasellaDiSelezione.Text.TrimStart(' ').Equals(""))
            {
                lbl_avviso.Text = "Inserire i campi obbligatori !";
                lbl_avviso.Visible = true;
            }
            else
            {
                lbl_avviso.Visible = false;
                ddl_valoriCasellaSelezione.Items.Add(new ListItem(txt_valoreCasellaSelezione.Text, txt_valoreCasellaSelezione.Text));
                ddl_valoriCasellaSelezione.SelectedIndex = ddl_valoriCasellaSelezione.Items.Count - 1;
                txt_valoreCasellaSelezione.Text = "";
                cb_Default_CasellaSelezione.Checked = false;
                cb_Disabilitato_CasellaSelezione.Checked = false;
                aggiungiValoreOggetto();
            }
            SetFocus(txt_valoreCasellaSelezione);
        }

        private void btn_eliminaValoreCasellaSelezione_Click(object sender, System.EventArgs e)
        {
            if (ddl_valoriCasellaSelezione.Items.Count != 0)
            {
                //Controllo per verificare se è possibile la rimozione del valore
                //Basta che un documento abbia questo valore attribuito che la rimozione non è possibile
                int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                if (oggettoSelezionato != -1)
                {
                    DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                    DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[oggettoSelezionato];
                    if (ProfilazioneDocManager.isValueInUse(Convert.ToString(oggettoCustom.SYSTEM_ID), Convert.ToString(template.SYSTEM_ID), ddl_valoriCasellaSelezione.SelectedItem.Text, this))
                    {
                        RegisterStartupScript("cancellazioneImpossibile", "<script>alert('Valore assegnato ad un documento ! \\nCancellazione impossible!');</script>");
                        return;
                    }
                }

                ddl_valoriCasellaSelezione.Items.RemoveAt(ddl_valoriCasellaSelezione.SelectedIndex);
                cb_Default_CasellaSelezione.Checked = false;
                cb_Disabilitato_CasellaSelezione.Checked = false;
            }
        }
        #endregion aggiungi-elimnina ValoriCasellaSelezione

        #region aggiungi-elimina ValoriMenuATendina
        private void btn_eliminaValoreMenuATendina_Click(object sender, System.EventArgs e)
        {
            //Metodo che rimuove un valore, dalla ddl dei valori del componente "MenuATendina".
            if (ddl_valoriMenuATendina.Items.Count != 0)
            {
                //Controllo per verificare se è possibile la rimozione del valore
                //Basta che un documento abbia questo valore attribuito che la rimozione non è possibile
                int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                if (oggettoSelezionato != -1)
                {
                    DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                    DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[oggettoSelezionato];
                    if (ProfilazioneDocManager.isValueInUse(Convert.ToString(oggettoCustom.SYSTEM_ID), Convert.ToString(template.SYSTEM_ID), ddl_valoriMenuATendina.SelectedItem.Text, this))
                    {
                        RegisterStartupScript("cancellazioneImpossibile", "<script>alert('Valore assegnato ad un documento ! \\nCancellazione impossible!');</script>");
                        return;
                    }
                }

                ddl_valoriMenuATendina.Items.RemoveAt(ddl_valoriMenuATendina.SelectedIndex);
                cb_Default_MenuATendina.Checked = false;
                cb_Disabilitato_MenuATendina.Checked = false;
                this.DdlColoriMenu.SelectedIndex = 0;
            }
        }

        private void btn_aggiungiValoreMenuATendina_Click(object sender, System.EventArgs e)
        {
            if (controllaValoriDuplicati(ddl_valoriMenuATendina, txt_valoreMenuATendina.Text))
            {
                RegisterStartupScript("valoreNonConsentito", "<script>alert('Il valore già esiste !'); </script>");
                return;
            }

            if (txt_valoreMenuATendina.Text.IndexOf("-") != -1)
            {
                RegisterStartupScript("carattereNonConsetito", "<script>alert('Il carattere \"-\" non è consentito !'); </script>");
                return;
            }
            //Metodo che aggiunge un nuovo valore, alla ddl dei valori del componente "MenuATendina".
            if (txt_valoreMenuATendina.Text.TrimStart(' ').Equals("") || txt_etichettaMenuATendina.Text.TrimStart(' ').Equals(""))
            {
                lbl_avviso.Text = "Inserire i campi obbligatori !";
                lbl_avviso.Visible = true;
            }
            else
            {
                lbl_avviso.Visible = false;
                ddl_valoriMenuATendina.Items.Add(new ListItem(txt_valoreMenuATendina.Text, txt_valoreMenuATendina.Text));
                ddl_valoriMenuATendina.SelectedIndex = ddl_valoriMenuATendina.Items.Count - 1;
                txt_valoreMenuATendina.Text = "";
                cb_Default_MenuATendina.Checked = false;
                cb_Disabilitato_MenuATendina.Checked = false;
                this.DdlColoriMenu.SelectedIndex = 0;
                aggiungiValoreOggetto();
            }
            SetFocus(txt_valoreMenuATendina);
        }
        #endregion aggiungi-elimina ValoriMenuATendina

        #region resetta-compila CampoDiTesto
        public void resettaPersCampoDiTesto()
        {
            txt_etichettaCampoDiTesto.Text = "";
            txt_TestoDefaultCampoTesto.Text = string.Empty;
            cb_Multilinea_CampoDiTesto.Checked = false;
            cb_Obbligatorio_CampoDiTesto.Checked = false;
            cb_Ricerca_CampoDiTesto.Checked = false;
            txt_NumeroCaratteri_CampoDiTesto.Text = "";
            //txt_NumeroCaratteri_CampoDiTesto.Enabled = true;
            //txt_NumeroCaratteri_CampoDiTesto.BackColor = System.Drawing.Color.White;
            txt_NumeroLinee_CampoDiTesto.Text = "";
            //txt_NumeroLinee_CampoDiTesto.Enabled = false;
            //txt_NumeroLinee_CampoDiTesto.BackColor = System.Drawing.Color.AntiqueWhite;
        }

        public void compilaPersCampoDiTesto(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            txt_etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;
            txt_TestoDefaultCampoTesto.Text = oggettoCustom.VALORE_DATABASE;
            txt_NumeroCaratteri_CampoDiTesto.Text = oggettoCustom.NUMERO_DI_CARATTERI;
            txt_NumeroLinee_CampoDiTesto.Text = oggettoCustom.NUMERO_DI_LINEE;
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                cb_Obbligatorio_CampoDiTesto.Checked = true;
            }
            else
            {
                cb_Obbligatorio_CampoDiTesto.Checked = false;
            }
            if (oggettoCustom.CAMPO_DI_RICERCA.Equals("SI"))
            {
                cb_Ricerca_CampoDiTesto.Checked = true;
            }
            else
            {
                cb_Ricerca_CampoDiTesto.Checked = false;
            }
            if (oggettoCustom.MULTILINEA.Equals("SI"))
            {
                cb_Multilinea_CampoDiTesto.Checked = true;
                //txt_NumeroCaratteri_CampoDiTesto.Enabled = true;
                //txt_NumeroCaratteri_CampoDiTesto.Enabled = false;
                //txt_NumeroCaratteri_CampoDiTesto.BackColor = System.Drawing.Color.AntiqueWhite;
                //txt_NumeroLinee_CampoDiTesto.Enabled = true;
                //txt_NumeroLinee_CampoDiTesto.BackColor = System.Drawing.Color.White;
            }
            else
            {
                cb_Multilinea_CampoDiTesto.Checked = false;
                //txt_NumeroCaratteri_CampoDiTesto.Enabled = true;
                //txt_NumeroCaratteri_CampoDiTesto.BackColor = System.Drawing.Color.White;
                //txt_NumeroLinee_CampoDiTesto.Enabled = false;
                //txt_NumeroLinee_CampoDiTesto.BackColor = System.Drawing.Color.AntiqueWhite;						
            }
        }
        #endregion resetta-compila CampoDiTesto

        #region resetta-compila CasellaDiSelezione - ddl_valoriCasellaSelezione_SelectedIndexChanged
        public void resettaPersCasellaDiSelezione()
        {
            txt_etichettaCasellaDiSelezione.Text = "";
            txt_valoreCasellaSelezione.Text = "";
            cb_Obbligatorio_CasellaDiSelezione.Checked = false;
            cb_Ricerca_CasellaDiSelezione.Checked = false;
            cb_Default_CasellaSelezione.Checked = false;
            ddl_valoriCasellaSelezione.Items.Clear();
        }

        public void compilaPersCasellaDiSelezione(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            txt_valoreCasellaSelezione.Text = "";
            txt_etichettaCasellaDiSelezione.Text = oggettoCustom.DESCRIZIONE;
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                cb_Obbligatorio_CasellaDiSelezione.Checked = true;
            }
            else
            {
                cb_Obbligatorio_CasellaDiSelezione.Checked = false;
            }
            if (oggettoCustom.CAMPO_DI_RICERCA.Equals("SI"))
            {
                cb_Ricerca_CasellaDiSelezione.Checked = true;
            }
            else
            {
                cb_Ricerca_CasellaDiSelezione.Checked = false;
            }
            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                rd_VerOri_CasellaSelezione.SelectedIndex = 1;
            }
            else
            {
                rd_VerOri_CasellaSelezione.SelectedIndex = 0;
            }
            DocsPAWA.DocsPaWR.ValoreOggetto valoreOggetto = new DocsPAWA.DocsPaWR.ValoreOggetto();
            ddl_valoriCasellaSelezione.Items.Clear();
            cb_Default_CasellaSelezione.Checked = false;
            cb_Disabilitato_CasellaSelezione.Checked = false;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                valoreOggetto = (DocsPAWA.DocsPaWR.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i];
                ddl_valoriCasellaSelezione.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                if (valoreOggetto.VALORE == ddl_valoriCasellaSelezione.SelectedItem.Text && valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                {
                    cb_Default_CasellaSelezione.Checked = true;
                }
                if (valoreOggetto.VALORE == ddl_valoriCasellaSelezione.SelectedItem.Text && valoreOggetto.ABILITATO == 0)
                {
                    cb_Disabilitato_CasellaSelezione.Checked = true;
                }
            }
        }

        private void ddl_valoriCasellaSelezione_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
            if (oggettoSelezionato != -1)
            {
                if (((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato] != null)
                {
                    DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato];
                    cb_Default_CasellaSelezione.Checked = false;
                    cb_Disabilitato_CasellaSelezione.Checked = false;
                    foreach (DocsPAWA.DocsPaWR.ValoreOggetto valoreOggetto in oggettoCustom.ELENCO_VALORI)
                    {
                        if (valoreOggetto.VALORE == ddl_valoriCasellaSelezione.SelectedItem.Text && valoreOggetto.VALORE_DI_DEFAULT == "SI")
                            cb_Default_CasellaSelezione.Checked = true;
                        if (valoreOggetto.VALORE == ddl_valoriCasellaSelezione.SelectedItem.Text && valoreOggetto.ABILITATO == 0)
                            cb_Disabilitato_CasellaSelezione.Checked = true;
                    }
                }
            }
        }
        #endregion resetta-compila CasellaDiSelezione - ddl_valoriCasellaSelezione_SelectedIndexChanged

        #region resetta-compila MenuATendina - ddl_valoriMenuATendina
        public void resettaPersMenuATendina()
        {
            txt_etichettaMenuATendina.Text = "";
            txt_valoreMenuATendina.Text = "";
            cb_Obbligatorio_MenuATendina.Checked = false;
            cb_Ricerca_MenuATendina.Checked = false;
            cb_Default_MenuATendina.Checked = false;
            this.DdlColoriMenu.SelectedIndex = 0;
            ddl_valoriMenuATendina.Items.Clear();
        }

        public void compilaPersMenuATendina(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            txt_valoreMenuATendina.Text = "";
            txt_etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE;
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                cb_Obbligatorio_MenuATendina.Checked = true;
            }
            else
            {
                cb_Obbligatorio_MenuATendina.Checked = false;
            }
            if (oggettoCustom.CAMPO_DI_RICERCA.Equals("SI"))
            {
                cb_Ricerca_MenuATendina.Checked = true;
            }
            else
            {
                cb_Ricerca_MenuATendina.Checked = false;
            }

            DocsPAWA.DocsPaWR.ValoreOggetto valoreOggetto = new DocsPAWA.DocsPaWR.ValoreOggetto();
            ddl_valoriMenuATendina.Items.Clear();
            cb_Default_MenuATendina.Checked = false;
            cb_Disabilitato_MenuATendina.Checked = false;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                valoreOggetto = (DocsPAWA.DocsPaWR.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i];
                ddl_valoriMenuATendina.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                if (valoreOggetto.VALORE == ddl_valoriMenuATendina.SelectedItem.Text)
                    this.DdlColoriMenu.SelectedValue = valoreOggetto.COLOR_BG;
                if (valoreOggetto.VALORE == ddl_valoriMenuATendina.SelectedItem.Text && valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                {
                    cb_Default_MenuATendina.Checked = true;
                }
                if (valoreOggetto.VALORE == ddl_valoriMenuATendina.SelectedItem.Text && valoreOggetto.ABILITATO == 0)
                {
                    cb_Disabilitato_MenuATendina.Checked = true;
                }
            }
        }

        private void ddl_valoriMenuATendina_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
            if (oggettoSelezionato != -1)
            {
                if (((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato] != null)
                {
                    DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato];
                    cb_Default_MenuATendina.Checked = false;
                    cb_Disabilitato_MenuATendina.Checked = false;
                    foreach (DocsPAWA.DocsPaWR.ValoreOggetto valoreOggetto in oggettoCustom.ELENCO_VALORI)
                    {
                        if (valoreOggetto.VALORE == ddl_valoriMenuATendina.SelectedItem.Text && valoreOggetto.VALORE_DI_DEFAULT == "SI")
                            cb_Default_MenuATendina.Checked = true;
                        if (valoreOggetto.VALORE == ddl_valoriMenuATendina.SelectedItem.Text && valoreOggetto.ABILITATO == 0)
                            cb_Disabilitato_MenuATendina.Checked = true;
                        if (valoreOggetto.VALORE == ddl_valoriMenuATendina.SelectedItem.Text)
                            this.DdlColoriMenu.SelectedValue = valoreOggetto.COLOR_BG;
                    }
                }
            }
        }
        #endregion resetta-compila MenuATendina - ddl_valoriMenuATendina

        #region resetta-compila SelezioneEsclusiva - ddl_valoriSelezioneEsclusiva
        public void resettaPersSelezioneEsclusiva()
        {
            txt_etichettaSelezioneEsclusiva.Text = "";
            txt_valoreSelezioneEsclusiva.Text = "";
            cb_Obbligatorio_SelezioneEsclusiva.Checked = false;
            cb_Ricerca_SelezioneEsclusiva.Checked = false;
            cb_Default_SelezioneEsclusiva.Checked = false;
            this.DdlColoriSelezioneEsclusiva.SelectedIndex = 0;
            ddl_valoriSelezioneEsclusiva.Items.Clear();
        }

        public void compilaPersSelezioneEsclusiva(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            txt_valoreSelezioneEsclusiva.Text = "";
            txt_etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE;
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                cb_Obbligatorio_SelezioneEsclusiva.Checked = true;
            }
            else
            {
                cb_Obbligatorio_SelezioneEsclusiva.Checked = false;
            }
            if (oggettoCustom.CAMPO_DI_RICERCA.Equals("SI"))
            {
                cb_Ricerca_SelezioneEsclusiva.Checked = true;
            }
            else
            {
                cb_Ricerca_SelezioneEsclusiva.Checked = false;
            }
            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                rd_VerOri_SelezioneEsclusiva.SelectedIndex = 1;
            }
            else
            {
                rd_VerOri_SelezioneEsclusiva.SelectedIndex = 0;
            }
            DocsPAWA.DocsPaWR.ValoreOggetto valoreOggetto = new DocsPAWA.DocsPaWR.ValoreOggetto();
            ddl_valoriSelezioneEsclusiva.Items.Clear();
            cb_Default_SelezioneEsclusiva.Checked = false;
            cb_Disabilitato_SelezioneEsclusiva.Checked = false;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                valoreOggetto = (DocsPAWA.DocsPaWR.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i];
                ddl_valoriSelezioneEsclusiva.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                if (valoreOggetto.VALORE == ddl_valoriSelezioneEsclusiva.SelectedItem.Text && valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                {
                    cb_Default_SelezioneEsclusiva.Checked = true;
                }
                if (valoreOggetto.VALORE == ddl_valoriSelezioneEsclusiva.SelectedItem.Text && valoreOggetto.ABILITATO == 0)
                {
                    cb_Disabilitato_SelezioneEsclusiva.Checked = true;
                }
                if (valoreOggetto.VALORE == ddl_valoriSelezioneEsclusiva.SelectedItem.Text)
                {   
                    this.DdlColoriSelezioneEsclusiva.SelectedValue = valoreOggetto.COLOR_BG;
                }
            }
        }

        private void ddl_valoriSelezioneEsclusiva_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
            if (oggettoSelezionato != -1)
            {
                if (((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato] != null)
                {
                    DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato];
                    cb_Default_SelezioneEsclusiva.Checked = false;
                    cb_Disabilitato_SelezioneEsclusiva.Checked = false;
                    foreach (DocsPAWA.DocsPaWR.ValoreOggetto valoreOggetto in oggettoCustom.ELENCO_VALORI)
                    {
                        if (valoreOggetto.VALORE == ddl_valoriSelezioneEsclusiva.SelectedItem.Text && valoreOggetto.VALORE_DI_DEFAULT == "SI")
                            cb_Default_SelezioneEsclusiva.Checked = true;
                        if (valoreOggetto.VALORE == ddl_valoriSelezioneEsclusiva.SelectedItem.Text && valoreOggetto.ABILITATO == 0)
                            cb_Disabilitato_SelezioneEsclusiva.Checked = true;
                        if (valoreOggetto.VALORE == ddl_valoriSelezioneEsclusiva.SelectedItem.Text)
                            this.DdlColoriSelezioneEsclusiva.SelectedValue = valoreOggetto.COLOR_BG;
                    }
                }
            }
        }
        #endregion resetta-compila SelezioneEsclusiva - ddl_valoriSelezioneEsclusiva

        #region resetta-compila Corrispondente - caricaDllRuoliPredefiniti
        public void resettaPersCorrispondente()
        {
            txt_etichettaCorr.Text = "";
            cb_Ricerca_Corr.Checked = false;
            cb_Obbligatorio_Corr.Checked = false;
            cb_Obbligatorio_Corr.Enabled = true;
            ddl_tipoRicerca.SelectedIndex = 0;
            ddl_tipoRicerca.Enabled = true;
            if (ddl_ruoloPredefinito.SelectedIndex != -1)
                ddl_ruoloPredefinito.SelectedIndex = 0;

            caricaDllRuoliPredefiniti();
        }

        public void caricaDllRuoliPredefiniti()
        {
            ddl_ruoloPredefinito.Items.Clear();
            ListItem itemEmpty = new ListItem();
            itemEmpty.Value = "";
            itemEmpty.Text = "";
            ddl_ruoloPredefinito.Items.Add(itemEmpty);

            ArrayList listaRuoli = new ArrayList(ProfilazioneDocManager.getRuoliByAmm(idAmministrazione, "", "", this));
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
            if (template != null)
            {
                ArrayList listaRuoliSelezionati = new ArrayList(ProfilazioneDocManager.getRuoliTipoDoc(template.ID_TIPO_ATTO, this));
                for (int i = 0; i < listaRuoli.Count; i++)
                {
                    DocsPAWA.DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)listaRuoli[i];
                    for (int j = 0; j < listaRuoliSelezionati.Count; j++)
                    {
                        DocsPAWA.DocsPaWR.AssDocFascRuoli obj = (DocsPAWA.DocsPaWR.AssDocFascRuoli)listaRuoliSelezionati[j];
                        if (ruolo.idGruppo == obj.ID_GRUPPO)
                        {
                            ListItem item = new ListItem();
                            item.Value = ruolo.systemId;
                            item.Text = ruolo.descrizione;
                            ddl_ruoloPredefinito.Items.Add(item);
                        }
                    }
                }
            }
        }

        public void compilaPersCorrispondente(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            caricaDllRuoliPredefiniti();
            txt_etichettaCorr.Text = oggettoCustom.DESCRIZIONE;
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                cb_Obbligatorio_Corr.Checked = true;
            }
            else
            {
                cb_Obbligatorio_Corr.Checked = false;
            }
            if (oggettoCustom.CAMPO_DI_RICERCA.Equals("SI"))
            {
                cb_Ricerca_Corr.Checked = true;
            }
            else
            {
                cb_Ricerca_Corr.Checked = false;
            }
            if (oggettoCustom.ID_RUOLO_DEFAULT != null && oggettoCustom.ID_RUOLO_DEFAULT != "" && oggettoCustom.ID_RUOLO_DEFAULT != "0")
            {
                ddl_ruoloPredefinito.SelectedValue = oggettoCustom.ID_RUOLO_DEFAULT;
            }
            else
            {
                ddl_ruoloPredefinito.SelectedIndex = 0;
            }
            if (oggettoCustom.TIPO_RICERCA_CORR != null && oggettoCustom.TIPO_RICERCA_CORR != "" && oggettoCustom.TIPO_RICERCA_CORR != "0")
            {
                switch (oggettoCustom.TIPO_RICERCA_CORR)
                {
                    case "INTERNI/ESTERNI":
                        ddl_tipoRicerca.SelectedIndex = 1;
                        break;
                    case "INTERNI":
                        ddl_tipoRicerca.SelectedIndex = 2;
                        break;
                    case "ESTERNI":
                        ddl_tipoRicerca.SelectedIndex = 3;
                        break;
                }
            }
            else
            {
                ddl_tipoRicerca.SelectedIndex = 0;
            }
        }



        #endregion resetta-compila Corrispondente - caricaDllRuoliPredefiniti

        #region resetta-compila Contatore
        public void resettaPersContatore()
        {
            txt_etichettaContatore.Text = "";
            cb_Ricerca_Contatore.Checked = false;
            cb_Azzera_Anno.Checked = false;
            cb_Azzera_Anno.Enabled = true;
            ddl_separatore.SelectedIndex = 0;
            ddl_campiContatore.SelectedIndex = 0;
            ddl_campiContatore.Enabled = false;
            txt_formatoContatore.Text = "";
            rbl_tipoContatore.SelectedIndex = 0;
            rbl_tipoContatore.SelectedIndex = 1;
            cb_Repertorio.Checked = false;
            cb_ContaDopo.Checked = false;
            cb_Repertorio.Enabled = true;
            cb_consRep.Checked = false;
            cb_consRep.Enabled = false;
            //prova di reset campi contatore
            lblDataFine.Visible = false;
            lblDataInizio.Visible = false;

            DataInizio.Visible = false;
            DataFine.Visible = false;
            DataInizio.Text = string.Empty;
            DataFine.Text = string.Empty;
            RadioButtonContatore.Items[0].Enabled = true;
            RadioButtonContatore.Items[0].Selected = true;
            RadioButtonContatore.Items[1].Enabled = true;
            RadioButtonContatore.Items[1].Selected = false;

        }

        public void compilaPersContatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            txt_etichettaContatore.Text = oggettoCustom.DESCRIZIONE;
            if (oggettoCustom.CAMPO_DI_RICERCA.Equals("SI"))
            {
                cb_Ricerca_Contatore.Checked = true;
            }
            else
            {
                cb_Ricerca_Contatore.Checked = false;
            }
            if (oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI")
            {
                cb_Azzera_Anno.Checked = true;
            }
            else
            {
                cb_Azzera_Anno.Checked = false;
            }

            //pongo cheked e disabled la checkbox in caso di Contatore Custom
            if (!string.IsNullOrEmpty(oggettoCustom.DATA_INIZIO.ToString()) && !string.IsNullOrEmpty(oggettoCustom.DATA_FINE.ToString()))
            {
                RadioButtonContatore.Items[0].Enabled = false;
                RadioButtonContatore.Items[0].Selected = false;
                cb_Azzera_Anno.Checked = true;
                cb_Azzera_Anno.Enabled = false;

            }

            if (oggettoCustom.FORMATO_CONTATORE != "")
                txt_formatoContatore.Text = oggettoCustom.FORMATO_CONTATORE;

            if (oggettoCustom.TIPO_CONTATORE != null)
            {
                switch (oggettoCustom.TIPO_CONTATORE)
                {
                    case "T":
                        rbl_tipoContatore.SelectedIndex = 0;
                        break;
                    case "A":
                        rbl_tipoContatore.SelectedIndex = 1;
                        break;
                    case "R":
                        rbl_tipoContatore.SelectedIndex = 2;
                        break;
                }
            }

            if (oggettoCustom.CONTA_DOPO != null)
            {
                if (oggettoCustom.CONTA_DOPO == "1")
                    cb_ContaDopo.Checked = true;
                else
                    cb_ContaDopo.Checked = false;
            }

            if (Utils.isEnableRepertori(idAmministrazione))
            {
                cb_Repertorio.Visible = true;
                lbl_repertorio.Visible = true;
                cb_consRep.Visible = this.IsConservazionePARER();
                lbl_consRep.Visible = this.IsConservazionePARER();
                if (oggettoCustom.REPERTORIO != null)
                {
                    if (oggettoCustom.REPERTORIO == "1")
                    {
                        cb_Repertorio.Checked = true;
                        cb_consRep.Enabled = true;
                    }
                    else
                    {
                        cb_Repertorio.Checked = false;
                        cb_consRep.Enabled = false;
                    }
                    if (cb_consRep.Enabled && oggettoCustom.CONS_REPERTORIO != null && oggettoCustom.CONS_REPERTORIO.Equals("1"))
                        cb_consRep.Checked = true;
                    else
                        cb_consRep.Checked = false;
                }
            }
            else
            {
                cb_Repertorio.Visible = false;
                lbl_repertorio.Visible = false;
                cb_consRep.Visible = false;
                lbl_consRep.Visible = false;
            }

            ddl_separatore.SelectedIndex = 0;
            ddl_campiContatore.SelectedIndex = 0;
        }

        #endregion resetta-compila Contatore

        #region resetta - compila Data
        public void resettaPersData()
        {
            txt_etichettaData.Text = "";
            cb_Obbligatorio_Data.Checked = false;
            cb_Ricerca_Data.Checked = false;
            ddl_formatoOra.SelectedIndex = 0;
        }

        public void compilaPersData(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            txt_etichettaData.Text = oggettoCustom.DESCRIZIONE;
            if (oggettoCustom.CAMPO_DI_RICERCA.Equals("SI"))
            {
                cb_Ricerca_Data.Checked = true;
            }
            else
            {
                cb_Ricerca_Data.Checked = false;
            }
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                cb_Obbligatorio_Data.Checked = true;
            }
            else
            {
                cb_Obbligatorio_Data.Checked = false;
            }
            if (string.IsNullOrEmpty(oggettoCustom.FORMATO_ORA))
                ddl_formatoOra.SelectedIndex = 0;
            //if (oggettoCustom.FORMATO_ORA.ToUpper() == "HH")
            //    ddl_formatoOra.SelectedIndex = 1;
            if (oggettoCustom.FORMATO_ORA.ToUpper() == "HH:MM")
                ddl_formatoOra.SelectedIndex = 1;
            if (oggettoCustom.FORMATO_ORA.ToUpper() == "HH:MM:SS")
                ddl_formatoOra.SelectedIndex = 2;
        }
        #endregion

        #region resetta-compila ContSottocontatore
        public void resettaPersContSottocontatore()
        {
            txt_etichettaContSottocontatore.Text = "";
            cb_Ricerca_ContSottocontatore.Checked = false;
            cb_Azzera_AnnoContSottocontatore.Checked = false;
            //ddl_separatoreSottocontatore.SelectedIndex = 0;
            //ddl_campiContSottocontatore.SelectedIndex = 0;
            //ddl_campiContSottocontatore.Enabled = false;
            //lbl_formatoContSottocontatore.Text = "";
            rbl_tipoContSottocontatore.SelectedIndex = 0;
            //cb_RepertorioContSottocontatore.Checked = false;
            cb_ContaDopoContSottocontatore.Checked = false;
            txt_moduloSottoconotatore.Text = string.Empty;
            btn_GestioneContatoreSottocontatore.Enabled = false;
        }

        public void compilaPersContSottocontatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            txt_etichettaContSottocontatore.Text = oggettoCustom.DESCRIZIONE;
            if (oggettoCustom.CAMPO_DI_RICERCA.Equals("SI"))
            {
                cb_Ricerca_ContSottocontatore.Checked = true;
            }
            else
            {
                cb_Ricerca_ContSottocontatore.Checked = false;
            }
            if (oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI")
            {
                cb_Azzera_AnnoContSottocontatore.Checked = true;
            }
            else
            {
                cb_Azzera_AnnoContSottocontatore.Checked = false;
            }

            if (oggettoCustom.FORMATO_CONTATORE != "")
                lbl_formatoContSottocontatore.Text = oggettoCustom.FORMATO_CONTATORE;

            if (oggettoCustom.TIPO_CONTATORE != null)
            {
                switch (oggettoCustom.TIPO_CONTATORE)
                {
                    case "T":
                        rbl_tipoContSottocontatore.SelectedIndex = 0;
                        break;
                    case "A":
                        rbl_tipoContSottocontatore.SelectedIndex = 1;
                        break;
                    case "R":
                        rbl_tipoContSottocontatore.SelectedIndex = 2;
                        break;
                }
            }

            if (oggettoCustom.CONTA_DOPO != null)
            {
                if (oggettoCustom.CONTA_DOPO == "1")
                    cb_ContaDopoContSottocontatore.Checked = true;
                else
                    cb_ContaDopoContSottocontatore.Checked = false;
            }

            txt_moduloSottoconotatore.Text = oggettoCustom.MODULO_SOTTOCONTATORE;

            if (oggettoCustom.SYSTEM_ID != 0 &&
                Session["template"] != null &&
                !string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "GEST_CONTATORI")) &&
                utils.InitConfigurationKeys.GetValue("0", "GEST_CONTATORI") != "0"
                )
            {
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                btn_GestioneContatoreSottocontatore.OnClientClick = "apriPopupGestioneContatore(" + oggettoCustom.SYSTEM_ID + "," + template.SYSTEM_ID + ");";
                btn_GestioneContatoreSottocontatore.Visible = true;
                btn_GestioneContatoreSottocontatore.Enabled = true;
            }
            else
            {
                btn_GestioneContatoreSottocontatore.Visible = false;
                btn_GestioneContatoreSottocontatore.Enabled = false;
            }
        }

        #endregion resetta-compila ContSottocontatore

        #region resetta - compila Separatore
        public void resettaPersSeparatore()
        {
            txt_etichettaSeparatore.Text = string.Empty;
        }

        public void compilaPersSeparatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            txt_etichettaSeparatore.Text = oggettoCustom.DESCRIZIONE;
        }
        #endregion resetta - compila Separatore


        #region controllaTipoDiDocumento - cb_Multilinea_CampoDiTesto_CheckedChanged
        private bool controllaTipoDiDocumento()
        {
            ArrayList tipiAttoTotali = ProfilazioneDocManager.getTipologiaAttoProfDin(idAmministrazione, this);
            for (int i = 0; i < tipiAttoTotali.Count; i++)
            {
                if (txt_TipoDocumento.Text.Equals(((DocsPAWA.DocsPaWR.TipologiaAtto)tipiAttoTotali[i]).descrizione))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion controllaTipoDiDocumento - cb_Multilinea_CampoDiTesto_CheckedChanged

        #region SetFocus
        private void SetFocus(System.Web.UI.Control ctrl)
        {
            string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
            RegisterStartupScript("focus", s);
        }
        #endregion SetFocus

        #region btn_modelli_Click - btn_salvaTemplate_Click
        private void btn_modelli_Click(object sender, System.EventArgs e)
        {
            //Visualizza il DataGrid contenente la lista dei modelli salvati
            Panel_NuovoModello.Visible = false;
            Panel_ListaComponenti.Visible = false;
            Panel_Dg_ListaComponenti.Visible = false;
            Panel_Personalizzazione.Visible = false;
            btn_modelli.Visible = false;
            lbl_avviso.Visible = false;
            lbl_titolo.Visible = true;
            btn_salvaTemplate.Visible = false;
            btn_anteprima.Visible = false;
            if (dg_listaTemplates.Items.Count == 0)
            {
                Panel_ListaModelli.Visible = false;
            }
            else
            {
                Panel_ListaModelli.Visible = true;
            }
        }

        private void btn_salvaTemplate_Click(object sender, System.EventArgs e)
        {
            btnStorico.Enabled = true;
            btn_ContestoProcedurale.Enabled = true;
            //Ripulisco il template da eventuali Campi in lavorazione che non sono stati salvati
            aggiornaTemplateInSessione();
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];

            //Metodo che permette l'inserimento del template in sessione, nel database
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmministrazione = Utils.getIdAmmByCod(codiceAmministrazione, this);

            DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
            datiAmministratore = session.getUserAmmSession();

            //Se in sessione non c'è template vuol dire che si sta inserendo una tipologia di
            //documento senza campi profilati 
            if (template == null)
            {
                template = new DocsPAWA.DocsPaWR.Templates();
                if (txt_TipoDocumento.Text == "")
                {
                    lbl_avviso.Text = "Inserire i campi obbligatori !";
                    lbl_avviso.Visible = true;
                    SetFocus(txt_TipoDocumento);
                    return;
                }
                else
                {
                    if (!controllaTipoDiDocumento())
                    {
                        template.DESCRIZIONE = txt_TipoDocumento.Text;
                        if (cb_Privato.Checked)
                        {
                            template.PRIVATO = "1";
                        }
                        else
                        {
                            template.PRIVATO = "0";
                        }

                        if (string.IsNullOrEmpty(this.txt_MesiConservazione.Text))
                        {
                            template.NUM_MESI_CONSERVAZIONE = "0";
                        }
                        else
                        {
                            template.NUM_MESI_CONSERVAZIONE = this.txt_MesiConservazione.Text;
                        }
                        if (cb_InvioConservazione.Checked)
                        {
                            template.INVIO_CONSERVAZIONE = "1";
                        }
                        else
                        {
                            template.INVIO_CONSERVAZIONE = "0";
                        }

                        ProfilazioneDocManager.salvaTemplate(datiAmministratore, template, idAmministrazione, this);
                        Panel_NuovoModello.Visible = false;
                        Panel_ListaComponenti.Visible = false;
                        Panel_Personalizzazione.Visible = false;
                        Panel_ListaModelli.Visible = true;
                        btn_salvaTemplate.Visible = false;
                        btn_anteprima.Visible = false;
                        btn_modelli.Visible = false;
                        popolaTemplateDG();
                        return;
                    }
                    else
                    {
                        lbl_avviso.Text = "Tipo di documento già esistente !";
                        lbl_avviso.Visible = true;
                        SetFocus(txt_TipoDocumento);
                        return;
                    }
                }
            }

            if (dg_listaTemplates.Visible)
            {


                if (ProfilazioneDocManager.aggiornaTemplate(template, this))
                {
                    Panel_NuovoModello.Visible = false;
                    Panel_ListaComponenti.Visible = false;
                    Panel_Personalizzazione.Visible = false;
                    Panel_ListaModelli.Visible = true;
                    btn_salvaTemplate.Visible = false;
                    btn_anteprima.Visible = false;
                    btn_modelli.Visible = false;
                    popolaTemplateDG();
                    dg_listaTemplates.SelectedIndex = -1;
                }
            }
            else
            {
                if (controllaTipoDiDocumento())
                {
                    Panel_NuovoModello.Visible = false;
                    Panel_ListaComponenti.Visible = false;
                    Panel_Personalizzazione.Visible = false;
                    Panel_ListaModelli.Visible = true;
                    btn_salvaTemplate.Visible = false;
                    btn_anteprima.Visible = false;
                    btn_modelli.Visible = false;
                    popolaTemplateDG();
                    return;
                }
                if (ProfilazioneDocManager.salvaTemplate(datiAmministratore, template, idAmministrazione, this))
                {
                    Panel_NuovoModello.Visible = false;
                    Panel_ListaComponenti.Visible = false;
                    Panel_Personalizzazione.Visible = false;
                    Panel_ListaModelli.Visible = true;
                    btn_salvaTemplate.Visible = false;
                    btn_anteprima.Visible = false;
                    btn_modelli.Visible = false;
                    popolaTemplateDG();
                }
            }

            Panel_Dg_ListaComponenti.Visible = false;
        }
        #endregion btn_modelli_Click - btn_salvaTemplate_Click

        #region DataGrid Lista Templates - btn_InEsercizio
        private void popolaTemplateDG()
        {
            //Vengono aggiunte le righe, che rappresentano i tipi di documento, al DataGrid che contiene, l'elenco
            //di questi ultimi. Per la definizione del DataSource del DataGrid, è stato creato un DataTable
            //contente una colonna Tipo di Documento.
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmministrazione = Utils.getIdAmmByCod(codiceAmministrazione, this);

            listaTemplates = new ArrayList(ProfilazioneDocManager.getTemplates(idAmministrazione, this));
            dt_listaTemplates = new DataTable();
            dt_listaTemplates.Columns.Add("Tipo");
            dt_listaTemplates.Columns.Add("In Esercizio");
            for (int i = 0; i < listaTemplates.Count; i++)
            {
                DataRow dr = dt_listaTemplates.NewRow();
                dr["Tipo"] = ((DocsPAWA.DocsPaWR.Templates)listaTemplates[i]).DESCRIZIONE;
                dr["In Esercizio"] = ((DocsPAWA.DocsPaWR.Templates)listaTemplates[i]).IN_ESERCIZIO;
                dt_listaTemplates.Rows.Add(dr);
            }

            dg_listaTemplates.DataSource = dt_listaTemplates;
            dg_listaTemplates.DataBind();

            //Imposto la colonna privato numero mesi conservazione
            for (int i = 0; i < listaTemplates.Count; i++)
            {
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)listaTemplates[i];
                if (template.PRIVATO != null && template.PRIVATO == "1")
                {
                    Label lbl = ((Label)dg_listaTemplates.Items[i].Cells[5].Controls[3]);
                    lbl.Text = "SI";
                }
                else
                {
                    Label lbl = ((Label)dg_listaTemplates.Items[i].Cells[5].Controls[3]);
                    lbl.Text = "NO";
                }

                Label lbl_mesi = ((Label)dg_listaTemplates.Items[i].Cells[6].Controls[3]);
                if (!string.IsNullOrEmpty(template.NUM_MESI_CONSERVAZIONE))
                {
                    lbl_mesi.Text = template.NUM_MESI_CONSERVAZIONE;
                }
                else
                {
                    lbl_mesi.Text = "0";
                }
                Label lbl_invioCons = ((Label)dg_listaTemplates.Items[i].Cells[7].Controls[3]);
                if (!string.IsNullOrEmpty(template.INVIO_CONSERVAZIONE) && template.INVIO_CONSERVAZIONE == "1")
                    lbl_invioCons.Text = "SI";
                else
                    lbl_invioCons.Text = "NO";
            }

            //Imposto il campo Diagramma di Stato
            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
            {
                for (int i = 0; i < listaTemplates.Count; i++)
                {
                    DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)listaTemplates[i];
                    int idDiagrammaAssociato = DocsPAWA.DiagrammiManager.getDiagrammaAssociato(Convert.ToString(template.SYSTEM_ID), this);
                    if (idDiagrammaAssociato != null && idDiagrammaAssociato != 0)
                    {
                        DocsPAWA.DocsPaWR.DiagrammaStato diagrammaAssociato = DocsPAWA.DiagrammiManager.getDiagrammaById(idDiagrammaAssociato.ToString(), this);
                        if (diagrammaAssociato != null)
                        {
                            Label lbl = ((Label)dg_listaTemplates.Items[i].Cells[1].Controls[3]);
                            lbl.Text = diagrammaAssociato.DESCRIZIONE;
                        }
                        else
                        {
                            Label lbl = ((Label)dg_listaTemplates.Items[i].Cells[1].Controls[3]);
                            lbl.Text = "";
                        }
                    }
                    else
                    {
                        Label lbl = ((Label)dg_listaTemplates.Items[i].Cells[1].Controls[3]);
                        lbl.Text = "";
                    }
                }
            }

            if (dg_listaTemplates.Items.Count != 0)
            {
                dg_listaTemplates.Visible = true;
            }
            else
            {
                Panel_ListaModelli.Visible = false;
            }

            //Imposto il campo Modelli
            impostaImmaginiModelli();

            //Imposto le differenze grafiche e funzionali per l'iperfascicolo che sicuro è il primo della lista
            //avendo ordinato lato backend l'array nel metodo "getTemplatesFasc"
            if (listaTemplates != null && listaTemplates.Count != 0)
            {
                if (((DocsPAWA.DocsPaWR.Templates)listaTemplates[0]).IPER_FASC_DOC == "1")
                {
                    dg_listaTemplates.Items[0].Cells[0].ControlStyle.Font.Bold = true;
                    dg_listaTemplates.Items[0].Cells[1].Enabled = false;
                    ((Label)dg_listaTemplates.Items[0].Cells[1].Controls[3]).Visible = false;
                    ((ImageButton)dg_listaTemplates.Items[0].Cells[1].Controls[1]).Visible = false;
                    dg_listaTemplates.Items[0].Cells[2].Enabled = false;
                    dg_listaTemplates.Items[0].Cells[3].Enabled = false;
                    dg_listaTemplates.Items[0].Cells[4].Enabled = false;
                    ((ImageButton)dg_listaTemplates.Items[0].Cells[4].Controls[1]).Visible = false;
                    dg_listaTemplates.Items[0].Cells[5].Enabled = false;
                    //dg_listaTemplates.Items[0].Cells[7].Controls[0].Visible = false;
                    dg_listaTemplates.Items[0].Cells[8].Controls[0].Visible = false;
                    dg_listaTemplates.Items[0].BackColor = System.Drawing.Color.Gray;
                }
            }

            ridimensionaDiv(dg_listaTemplates.Items.Count, "DivDGListaTemplates");
        }

        private void impostaImmaginiModelli()
        {
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmministrazione = Utils.getIdAmmByCod(codiceAmministrazione, this);

            listaTemplates = new ArrayList(ProfilazioneDocManager.getTemplates(idAmministrazione, this));

            //Imposto il campo Modelli
            for (int i = 0; i < listaTemplates.Count; i++)
            {
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)listaTemplates[i];
                ((System.Web.UI.WebControls.Label)dg_listaTemplates.Items[i].Cells[3].Controls[3]).Text = "";

                if (template.PATH_MODELLO_1 != "")
                {
                    ((System.Web.UI.WebControls.Label)dg_listaTemplates.Items[i].Cells[3].Controls[3]).Text = "1-";
                    ((System.Web.UI.WebControls.Label)dg_listaTemplates.Items[i].Cells[3].Controls[3]).Visible = true;
                }
                if (template.PATH_MODELLO_2 != "")
                {
                    ((System.Web.UI.WebControls.Label)dg_listaTemplates.Items[i].Cells[3].Controls[3]).Text += "2-";
                    ((System.Web.UI.WebControls.Label)dg_listaTemplates.Items[i].Cells[3].Controls[3]).Visible = true;
                }
                if (template.PATH_ALLEGATO_1 != "")
                {
                    ((System.Web.UI.WebControls.Label)dg_listaTemplates.Items[i].Cells[3].Controls[3]).Text += " A-";
                    ((System.Web.UI.WebControls.Label)dg_listaTemplates.Items[i].Cells[3].Controls[3]).Visible = true;
                }
                if (((System.Web.UI.WebControls.Label)dg_listaTemplates.Items[i].Cells[3].Controls[3]).Text.Length >= 2)
                {
                    string textModelli = ((System.Web.UI.WebControls.Label)dg_listaTemplates.Items[i].Cells[3].Controls[3]).Text;
                    ((System.Web.UI.WebControls.Label)dg_listaTemplates.Items[i].Cells[3].Controls[3]).Text = textModelli.Substring(0, textModelli.Length - 1);
                }
                if (template.PATH_MODELLO_1 == "" && template.PATH_MODELLO_2 == "" && template.PATH_ALLEGATO_1 == "")
                {
                    ((System.Web.UI.WebControls.Label)dg_listaTemplates.Items[i].Cells[3].Controls[3]).Text += "";
                    ((System.Web.UI.WebControls.Label)dg_listaTemplates.Items[i].Cells[3].Controls[3]).Visible = false;
                }
            }
        }

        private void btn_inEsercizio_Click(object sender, System.EventArgs e)
        {
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmministrazione = Utils.getIdAmmByCod(codiceAmministrazione, this);

            ProfilazioneDocManager.messaInEserizioTemplate(template, idAmministrazione, this);

            //btn_inEsercizio.Visible = false;
            if (template.IN_ESERCIZIO.ToUpper().Equals("SI"))
            {
                btn_inEsercizio.Text = "In Esercizio";
                template.IN_ESERCIZIO = "NO";
            }
            else
            {
                btn_inEsercizio.Text = "Sospendi";
                template.IN_ESERCIZIO = "SI";
            }

            popolaTemplateDG();
        }

        protected void btn_isInstance_OnClick(object sender, EventArgs e)
        {
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
            template.IS_TYPE_INSTANCE = template.IS_TYPE_INSTANCE == '0' ? '1' : '0';
            if (ProfilazioneDocManager.UpdateIsTypeInstance(template, idAmministrazione, this.Page))
                this.btn_isInstance.Text = template.IS_TYPE_INSTANCE == '1' ? "Rimuovi istanza" : "Istanza accesso";
        }

        private void dg_listaTemplates_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_confirmDel")).Value == "si")
            {
                ((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_confirmDel")).Value = "";
                Panel_Diagrammi_Trasmissioni.Visible = false;

                //Metodo invocato quando si desidera rimuovere un componente dal DataGrid contenente la lista di quest'ultimi.
                //Di conseguenza la rimozione viene anche effettuata dal template in sessione.
                int elSelezionato = e.Item.ItemIndex;
                if (elSelezionato != -1)
                {
                    string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                    string codiceAmministrazione = amministrazione[0];
                    idAmministrazione = Utils.getIdAmmByCod(codiceAmministrazione, this);

                    DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)listaTemplates[elSelezionato];
                    ProfilazioneDocManager.disabilitaTemplate(template, idAmministrazione, codiceAmministrazione, this);
                    DocsPAWA.DiagrammiManager.disassociaTipoDocDiagramma(template.SYSTEM_ID.ToString(), this);

                    popolaTemplateDG();

                    if (dg_listaTemplates.Items.Count == 0)
                    {
                        dg_listaTemplates.Visible = false;
                        Panel_ListaModelli.Visible = false;
                    }
                    else
                    {
                        dg_listaTemplates.Visible = true;
                    }

                    Panel_Personalizzazione.Visible = false;
                    Panel_ListaComponenti.Visible = false;
                    dg_listaComponenti.Visible = false;
                    dg_listaTemplates.SelectedIndex = -1;
                    btn_anteprima.Visible = false;
                    btn_salvaTemplate.Visible = false;
                    btn_inEsercizio.Visible = false;
                    this.btn_isInstance.Visible = false;
                    lbl_titolo.Visible = true;
                    lbl_nameTypeDoc.Text = "";
                }
            }
        }

        private void dg_listaTemplates_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            int elSelezionato = e.Item.ItemIndex;
            listaTemplates = new ArrayList(ProfilazioneDocManager.getTemplates(idAmministrazione, this));

            switch (e.CommandName)
            {
                case "Select":
                    Panel_Diagrammi_Trasmissioni.Visible = false;
                    Panel_Privato.Visible = false;
                    Panel_ConsolidaOgg.Visible = false;
                    Panel_ConservaOgg.Visible = false;
                    if (dg_listaComponenti.Items.Count != 0)
                    {
                        dt_listaComponenti = new DataTable();
                        dg_listaComponenti.DataSource = dt_listaComponenti;
                        dg_listaComponenti.DataBind();
                    }
                    if (elSelezionato != -1)
                    {
                        //DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)listaTemplates[elSelezionato];
                        DocsPAWA.DocsPaWR.Templates template = ProfilazioneDocManager.getTemplateById(((DocsPAWA.DocsPaWR.Templates)listaTemplates[elSelezionato]).SYSTEM_ID.ToString(), this);
                        Session.Add("template", template);
                        if (template.ELENCO_OGGETTI.Length != 0)
                            aggiungiComponenteDG();

                        //int nomeCampo = 0;
                        //for(int i=0; i<template.ELENCO_OGGETTI.Length; i++)
                        //{
                        //    string tipoComponente = (((DocsPAWA.DocsPaWR.OggettoCustom) template.ELENCO_OGGETTI[i]).TIPO).DESCRIZIONE_TIPO;
                        //    aggiungiComponenteDG();
                        //    if( Convert.ToInt32(((DocsPAWA.DocsPaWR.OggettoCustom) template.ELENCO_OGGETTI[i]).NOME) > nomeCampo)
                        //        nomeCampo = Convert.ToInt32(((DocsPAWA.DocsPaWR.OggettoCustom) template.ELENCO_OGGETTI[i]).NOME);
                        //}
                        //Session.Add("nomeCampo", nomeCampo + 1);


                        lbl_nameTypeDoc.Text = template.DESCRIZIONE;
                        Panel_ListaComponenti.Visible = true;
                        Panel_Personalizzazione.Visible = false;
                        btn_anteprima.Visible = true;
                        btn_salvaTemplate.Visible = false;
                        lbl_avviso.Visible = false;
                        //Session.Add("templateSelezionato",elSelezionato);
                        if (template.IN_ESERCIZIO.Equals("NO"))
                        {
                            //btn_inEsercizio.Visible = true;
                            btn_inEsercizio.Visible = true;
                            btn_inEsercizio.Text = "In Esercizio";
                        }
                        else
                        {
                            //btn_inEsercizio.Visible = false;
                            btn_inEsercizio.Visible = true;
                            btn_inEsercizio.Text = "Sospendi";
                        }

                        if (template.IPER_FASC_DOC != "1")
                        {
                            if (verificaEsistenzaCampiComuni())
                                btn_CampiComuni.Visible = true;
                            else
                                btn_CampiComuni.Visible = false;
                        }
                        else
                        {
                            btn_CampiComuni.Visible = false;
                        }
                        string keyInstance = DocsPAWA.utils.InitConfigurationKeys.GetValue(idAmministrazione, "BE_INSTANCE_ACCESS_PATH");
                        if (!string.IsNullOrEmpty(keyInstance) && !keyInstance.Equals("0"))
                        {
                            this.btn_isInstance.Visible = true;
                            if (template.IS_TYPE_INSTANCE == '1')
                            {
                                
                                this.btn_isInstance.Text = "Rimuovi istanza";
                            }
                            else
                            {
                                this.btn_isInstance.Text = "Istanza accesso";
                            }
                        }

                        this.btnStorico.Visible = true;
                        this.btnStorico.Enabled = template.ELENCO_OGGETTI.Length > 0;
                        this.btnStorico.OnClientClick = GestioneStoricoProfDinamica.GetOpenScript(DocsPaWR.ObjectType.Document);

                        if (dg_listaComponenti.Items.Count == 0)
                        {
                            dg_listaComponenti.Visible = false;
                            lbl_nameTypeDoc.Text = "";
                        }
                        dg_listaComponenti.SelectedIndex = -1;

                        string keyContestoProcedurale = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_ENABLE_FLUSSO_AUTOMATICO");
                        if (!string.IsNullOrEmpty(keyContestoProcedurale) && keyContestoProcedurale.Equals("1"))
                        {
                            this.btn_ContestoProcedurale.Visible = true;
                        }
                    }
                    break;

                case "Diagrammi":
                    Panel_Privato.Visible = false;
                    Panel_ListaComponenti.Visible = false;
                    Panel_Dg_ListaComponenti.Visible = false;
                    Panel_NuovoModello.Visible = false;
                    Panel_Personalizzazione.Visible = false;
                    Panel_Diagrammi_Trasmissioni.Visible = true;
                    btn_anteprima.Visible = false;
                    btn_salvaTemplate.Visible = false;
                    btn_inEsercizio.Visible = false;
                    Panel_MesiCons.Visible = false;
                    Panel_InvioCons.Visible = false;
                    Panel_ConsolidaOgg.Visible = false;
                    Panel_ConservaOgg.Visible = false;
                    this.btn_isInstance.Visible = false;

                    //DocsPAWA.DocsPaWR.Templates temp = (DocsPAWA.DocsPaWR.Templates) listaTemplates[elSelezionato];
                    DocsPAWA.DocsPaWR.Templates temp = ProfilazioneDocManager.getTemplateById(((DocsPAWA.DocsPaWR.Templates)listaTemplates[elSelezionato]).SYSTEM_ID.ToString(), this);

                    Session.Add("templateSelPerDiagrTrasm", temp);
                    lbl_tipoDoc.Text = temp.DESCRIZIONE;
                    dg_listaTemplates.SelectedIndex = elSelezionato;
                    settaDllDiagrammi();

                    if (temp.SCADENZA != "0" && temp.SCADENZA != null)
                        txt_scadenza.Text = temp.SCADENZA;
                    else
                        txt_scadenza.Text = "";
                    if (temp.PRE_SCADENZA != "0" && temp.PRE_SCADENZA != null)
                        txt_preScadenza.Text = temp.PRE_SCADENZA;
                    else
                        txt_preScadenza.Text = "";
                    break;

                case "Modelli":
                    //DocsPAWA.DocsPaWR.Templates temp_1 = (DocsPAWA.DocsPaWR.Templates) listaTemplates[elSelezionato];
                    DocsPAWA.DocsPaWR.Templates temp_1 = ProfilazioneDocManager.getTemplateById(((DocsPAWA.DocsPaWR.Templates)listaTemplates[elSelezionato]).SYSTEM_ID.ToString(), this);

                    Session.Add("templateSelPerModelli", temp_1);
                    dg_listaTemplates.SelectedIndex = elSelezionato;

                    RegisterStartupScript("apriPopupModelli", "<script>apriPopupModelli();</script>");
                    Panel_Privato.Visible = false;
                    Panel_ListaComponenti.Visible = false;
                    Panel_Dg_ListaComponenti.Visible = false;
                    Panel_Diagrammi_Trasmissioni.Visible = false;
                    Panel_Personalizzazione.Visible = false;
                    Panel_MesiCons.Visible = false;
                    Panel_InvioCons.Visible = false;
                    Panel_ConsolidaOgg.Visible = false;
                    Panel_ConservaOgg.Visible = false;
                    break;
                case "Visibilita":
                    //DocsPAWA.DocsPaWR.Templates temp_1 = (DocsPAWA.DocsPaWR.Templates) listaTemplates[elSelezionato];
                    DocsPAWA.DocsPaWR.Templates temp_2 = ProfilazioneDocManager.getTemplateById(((DocsPAWA.DocsPaWR.Templates)listaTemplates[elSelezionato]).SYSTEM_ID.ToString(), this);

                    Session.Add("templateSelPerVisibilita", temp_2);

                    dg_listaTemplates.SelectedIndex = elSelezionato;

                    RegisterStartupScript("apriPopupVisibilita", "<script>apriPopupVisibilita();</script>");
                    Panel_Privato.Visible = false;
                    Panel_ListaComponenti.Visible = false;
                    Panel_Dg_ListaComponenti.Visible = false;
                    Panel_Diagrammi_Trasmissioni.Visible = false;
                    Panel_Personalizzazione.Visible = false;
                    break;
                case "Privato":
                    DocsPAWA.DocsPaWR.Templates temp_3 = ProfilazioneDocManager.getTemplateById(((DocsPAWA.DocsPaWR.Templates)listaTemplates[elSelezionato]).SYSTEM_ID.ToString(), this);
                    Session.Add("templateSelPerPrivato", temp_3);
                    if (temp_3.PRIVATO != null && temp_3.PRIVATO == "1")
                    {
                        cb_ModPrivato.Checked = true;
                    }
                    else
                    {
                        cb_ModPrivato.Checked = false;
                    }
                    dg_listaTemplates.SelectedIndex = elSelezionato;
                    lbl_TipoDocPr.Text = temp_3.DESCRIZIONE;
                    Panel_Privato.Visible = true;
                    Panel_ListaComponenti.Visible = false;
                    Panel_Dg_ListaComponenti.Visible = false;
                    Panel_Diagrammi_Trasmissioni.Visible = false;
                    Panel_Personalizzazione.Visible = false;
                    Panel_MesiCons.Visible = false;
                    Panel_InvioCons.Visible = false;
                    Panel_ConsolidaOgg.Visible = false;
                    Panel_ConservaOgg.Visible = false;
                    break;
                case "MesiCons":
                    DocsPAWA.DocsPaWR.Templates temp_4 = ProfilazioneDocManager.getTemplateById(((DocsPAWA.DocsPaWR.Templates)listaTemplates[elSelezionato]).SYSTEM_ID.ToString(), this);
                    Session.Add("templateSelPerMesiCons", temp_4);

                    dg_listaTemplates.SelectedIndex = elSelezionato;
                    LblTipoDocMesiCons.Text = temp_4.DESCRIZIONE;
                    txt_ModMesiCons.Text = temp_4.NUM_MESI_CONSERVAZIONE;
                    Panel_MesiCons.Visible = true;
                    Panel_Privato.Visible = false;
                    Panel_ListaComponenti.Visible = false;
                    Panel_Dg_ListaComponenti.Visible = false;
                    Panel_Diagrammi_Trasmissioni.Visible = false;
                    Panel_Personalizzazione.Visible = false;
                    Panel_InvioCons.Visible = false;
                    Panel_ConsolidaOgg.Visible = false;
                    Panel_ConservaOgg.Visible = false;
                    break;
                case "InvioCons":
                    DocsPAWA.DocsPaWR.Templates temp_5 = ProfilazioneDocManager.getTemplateById(((DocsPAWA.DocsPaWR.Templates)listaTemplates[elSelezionato]).SYSTEM_ID.ToString(), this);
                    Session.Add("templateSelPerInvioCons", temp_5);

                    dg_listaTemplates.SelectedIndex = elSelezionato;
                    LblTipoDocInvioCons.Text = temp_5.DESCRIZIONE;
                    if (!string.IsNullOrEmpty(temp_5.INVIO_CONSERVAZIONE) && temp_5.INVIO_CONSERVAZIONE == "1")
                        cb_ModInvioCons.Checked = true;
                    else
                        cb_ModInvioCons.Checked = false;
                    Panel_InvioCons.Visible = true;
                    Panel_MesiCons.Visible = false;
                    Panel_Privato.Visible = false;
                    Panel_ListaComponenti.Visible = false;
                    Panel_Dg_ListaComponenti.Visible = false;
                    Panel_Diagrammi_Trasmissioni.Visible = false;
                    Panel_Personalizzazione.Visible = false;
                    Panel_ConsolidaOgg.Visible = false;
                    Panel_ConservaOgg.Visible = false;
                    break;

            }
        }

        #endregion DataGrid Lista Templates - btn_InEsercizio

        #region ridimensiona Div
        public void ridimensionaDiv(int rows, string nomeControl)
        {
            int dim = (rows * 25) + 12;
            if (dim > 111)
            {
                if (nomeControl == "DivDGListaTemplates")
                    ((System.Web.UI.HtmlControls.HtmlControl)FindControl(nomeControl)).Attributes.Add("style", "OVERFLOW: auto; HEIGHT: 117px");
                else
                    ((System.Web.UI.HtmlControls.HtmlControl)FindControl(nomeControl)).Attributes.Add("style", "OVERFLOW: auto; HEIGHT: 109px");
            }
            else
            {
                if (nomeControl == "DivDGListaTemplates")
                    ((System.Web.UI.HtmlControls.HtmlControl)FindControl(nomeControl)).Attributes.Add("style", "OVERFLOW: auto; HEIGHT:" + (dim + 4) + "px");
                else
                    ((System.Web.UI.HtmlControls.HtmlControl)FindControl(nomeControl)).Attributes.Add("style", "OVERFLOW: auto; HEIGHT:" + dim + "px");
            }
        }
        #endregion ridimensiona Div

        #region btn_up_1_Click - btn_down_1_Click
        private void btn_up_1_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            for (int i = 0; i < dg_listaComponenti.Items.Count; i++)
            {
                if (dg_listaComponenti.Items[i].Cells[0].Text == "&nbsp;")
                    return;
            }

            if (dg_listaComponenti.SelectedIndex != 0)
            {
                int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                Session.Add("template", ProfilazioneDocManager.spostaOggetto(template, oggettoSelezionato, "UP", this));

                aggiungiComponenteDG();
                dg_listaComponenti.SelectedIndex = oggettoSelezionato - 1;
                if (scrollKeeperListaComponenti.VPos > 25)
                {
                    scrollKeeperListaComponenti.VPos -= 25;
                }
                else
                {
                    scrollKeeperListaComponenti.VPos = 0;
                }
            }
        }

        private void btn_down_1_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            for (int i = 0; i < dg_listaComponenti.Items.Count; i++)
            {
                if (dg_listaComponenti.Items[i].Cells[0].Text == "&nbsp;")
                    return;
            }

            if (dg_listaComponenti.SelectedIndex < dg_listaComponenti.Items.Count - 1)
            {
                int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                Session.Add("template", ProfilazioneDocManager.spostaOggetto(template, oggettoSelezionato, "DOWN", this));

                aggiungiComponenteDG();
                dg_listaComponenti.SelectedIndex = oggettoSelezionato + 1;
                int maxScroll = (dg_listaComponenti.Items.Count * 25) + 12;
                if (scrollKeeperListaComponenti.VPos < maxScroll)
                {
                    scrollKeeperListaComponenti.VPos += 25;
                }
                else
                {
                    scrollKeeperListaComponenti.VPos = maxScroll;
                }
            }
        }
        #endregion btn_up_1_Click - btn_down_1_Click

        #region Pannello Associazione Diagrammi di stato e Trasmissioni
        private void pulsante_chiudi_diagr_trasm_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Panel_Diagrammi_Trasmissioni.Visible = false;
        }


        private void settaDllDiagrammi()
        {
            ddl_Diagrammi.Items.Clear();
            ListItem itemNull = new ListItem();
            itemNull.Text = "<Nessun Diagramma>";
            itemNull.Value = "0";
            ddl_Diagrammi.Items.Add(itemNull);

            ArrayList dg = new ArrayList(DocsPAWA.DiagrammiManager.getDiagrammi(idAmministrazione, this));
            for (int i = 0; i < dg.Count; i++)
            {
                ListItem item = new ListItem();
                item.Text = ((DocsPAWA.DocsPaWR.DiagrammaStato)dg[i]).DESCRIZIONE;
                item.Value = ((DocsPAWA.DocsPaWR.DiagrammaStato)dg[i]).SYSTEM_ID.ToString();
                ddl_Diagrammi.Items.Add(item);
            }
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerDiagrTrasm"];
            string idTipoDoc = template.SYSTEM_ID.ToString();
            int idDiagrammaAssociato = DocsPAWA.DiagrammiManager.getDiagrammaAssociato(idTipoDoc, this);
            if (idDiagrammaAssociato != 0)
            {
                for (int i = 0; i < dg.Count; i++)
                {
                    if (idDiagrammaAssociato == ((DocsPAWA.DocsPaWR.DiagrammaStato)dg[i]).SYSTEM_ID)
                    {
                        ddl_Diagrammi.SelectedIndex = i + 1;
                        break;
                    }
                }
            }
            if (ddl_Diagrammi.SelectedIndex != 0)
            {
                ddl_Diagrammi.Enabled = false;
                caricaDgStati();
                btn_cambiaDiag.Visible = true;
            }
            else
            {
                ddl_Diagrammi.Enabled = true;
                dg_Stati.Visible = false;
                btn_cambiaDiag.Visible = false;
            }
        }


        private void btn_confermaDiagrTrasm_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txt_scadenza.Text == "" && txt_preScadenza.Text != "")
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "scadenze", "<script>alert('Inserire i giorni di scadenza !');</script>");
                    return;
                }

                if (txt_scadenza.Text != null && txt_scadenza.Text != "")
                {
                    int scadenza = Convert.ToInt32(txt_scadenza.Text);
                }
                if (txt_preScadenza.Text != null && txt_preScadenza.Text != "")
                {
                    int scadenza_preScadenza = Convert.ToInt32(txt_preScadenza.Text);
                }

                if (txt_preScadenza.Text != null && txt_preScadenza.Text != "" && txt_scadenza.Text != null && txt_scadenza.Text != "")
                {
                    int scadenza = Convert.ToInt32(txt_scadenza.Text);
                    int scadenza_preScadenza = Convert.ToInt32(txt_preScadenza.Text);
                    if (scadenza <= scadenza_preScadenza)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "scadenze", "<script>alert('I giorni di scadenza devono essere maggiori di quelli di notifica !');</script>");
                        return;
                    }
                }

                ProfilazioneDocManager.updateScadenzeTipoDoc(((DocsPAWA.DocsPaWR.Templates)Session["templateSelPerDiagrTrasm"]).SYSTEM_ID, txt_scadenza.Text, txt_preScadenza.Text, this);
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "scadenze", "<script>alert('I giorni di scadenza devono essere dei numeri interi !');</script>");
                return;
            }

            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerDiagrTrasm"];
            string idTipoDoc = template.SYSTEM_ID.ToString();

            if (ddl_Diagrammi.SelectedItem.Value != "0")
            {
                string idDiagramma = ddl_Diagrammi.SelectedItem.Value;
                DocsPAWA.DiagrammiManager.associaTipoDocDiagramma(idTipoDoc, idDiagramma, this);
            }
            else
            {
                settaDllDiagrammi();
            }
            Panel_Diagrammi_Trasmissioni.Visible = false;
            popolaTemplateDG();
        }


        private void caricaDgStati()
        {
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerDiagrTrasm"];
            string idTipoDoc = template.SYSTEM_ID.ToString();
            int idDiagrammaAssociato = DocsPAWA.DiagrammiManager.getDiagrammaAssociato(idTipoDoc, this);
            if (idDiagrammaAssociato != null && idDiagrammaAssociato != 0)
            {
                DocsPAWA.DocsPaWR.DiagrammaStato diagrammaAssociato = DocsPAWA.DiagrammiManager.getDiagrammaById(idDiagrammaAssociato.ToString(), this);

                DataTable dt = new DataTable();
                dt.Columns.Add("ID_STATO");
                dt.Columns.Add("DESCRIZIONE");
                for (int i = 0; i < diagrammaAssociato.STATI.Length; i++)
                {
                    DocsPAWA.DocsPaWR.Stato st = diagrammaAssociato.STATI[i];
                    DataRow rw = dt.NewRow();
                    rw[0] = st.SYSTEM_ID;
                    rw[1] = st.DESCRIZIONE;
                    dt.Rows.Add(rw);
                }
                dt.AcceptChanges();
                dg_Stati.DataSource = dt;
                dg_Stati.DataBind();
                dg_Stati.Visible = true;
            }
        }


        private void ddl_Diagrammi_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (ddl_Diagrammi.SelectedIndex != 0)
            {
                DocsPAWA.DocsPaWR.DiagrammaStato diagrammaAssociato = DocsPAWA.DiagrammiManager.getDiagrammaById(ddl_Diagrammi.SelectedValue, this);

                DataTable dt = new DataTable();
                dt.Columns.Add("ID_STATO");
                dt.Columns.Add("DESCRIZIONE");
                for (int i = 0; i < diagrammaAssociato.STATI.Length; i++)
                {
                    DocsPAWA.DocsPaWR.Stato st = diagrammaAssociato.STATI[i];
                    DataRow rw = dt.NewRow();
                    rw[0] = st.SYSTEM_ID;
                    rw[1] = st.DESCRIZIONE;
                    dt.Rows.Add(rw);
                }
                dt.AcceptChanges();
                dg_Stati.DataSource = dt;
                dg_Stati.DataBind();
                dg_Stati.Visible = true;
                //btn_modelliTrasm.Visible = true;
            }
            else
            {
                dg_Stati.Visible = false;
                //btn_modelliTrasm.Visible = false;
            }
        }


        private void btn_modelliTrasm_Click(object sender, System.EventArgs e)
        {
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerDiagrTrasm"];
            string idTipoDoc = template.SYSTEM_ID.ToString();
            int idDiagrammaAssociato = DocsPAWA.DiagrammiManager.getDiagrammaAssociato(idTipoDoc, this);
            if (idDiagrammaAssociato == 0 && ddl_Diagrammi.SelectedIndex != 0)
                idDiagrammaAssociato = Convert.ToInt32(ddl_Diagrammi.SelectedValue);

            if (idDiagrammaAssociato != null && idDiagrammaAssociato != 0)
            {
                DocsPAWA.DocsPaWR.DiagrammaStato diagrammaAssociato = DocsPAWA.DiagrammiManager.getDiagrammaById(idDiagrammaAssociato.ToString(), this);
                Session.Remove("idTipoDoc");
                Session.Remove("idDiagramma");
                Session.Remove("idStato");
                Session.Add("idTipoDoc", idTipoDoc);
                Session.Add("idDiagramma", Convert.ToString(idDiagrammaAssociato));

                RegisterStartupScript("AssociaModelliTrasm", "<script>apriModelliTrasm();</script>");
            }
        }


        private void dg_Stati_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerDiagrTrasm"];
            string idTipoDoc = template.SYSTEM_ID.ToString();
            int idDiagrammaAssociato = DocsPAWA.DiagrammiManager.getDiagrammaAssociato(idTipoDoc, this);
            if (idDiagrammaAssociato == 0 && ddl_Diagrammi.SelectedIndex != 0)
                idDiagrammaAssociato = Convert.ToInt32(ddl_Diagrammi.SelectedValue);

            if (idDiagrammaAssociato != null && idDiagrammaAssociato != 0)
            {
                DocsPAWA.DocsPaWR.DiagrammaStato diagrammaAssociato = DocsPAWA.DiagrammiManager.getDiagrammaById(idDiagrammaAssociato.ToString(), this);
                string idStato = dg_Stati.SelectedItem.Cells[0].Text;
                string descrizioneStato = dg_Stati.SelectedItem.Cells[1].Text;

                Session.Remove("idTipoDoc");
                Session.Remove("idDiagramma");
                Session.Remove("idStato");
                Session.Remove("descrStato");
                Session.Add("idTipoDoc", idTipoDoc);
                Session.Add("idDiagramma", Convert.ToString(idDiagrammaAssociato));
                Session.Add("idStato", idStato);
                Session.Add("descrStato", descrizioneStato);

                RegisterStartupScript("AssociaModelliTrasm", "<script>apriModelliTrasm();</script>");
            }
        }

        private void btn_cambiaDiag_Click(object sender, System.EventArgs e)
        {
            //Controllo se tutti i documenti con questa tipologia e associati a questo diagramma
            //si trovano in uno stato finale
            bool result = false;
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerDiagrTrasm"];
            string idTipoDoc = template.SYSTEM_ID.ToString();

            if (ddl_Diagrammi.SelectedIndex != 0)
            {
                DocsPAWA.DocsPaWR.DiagrammaStato diagrammaAssociato = DocsPAWA.DiagrammiManager.getDiagrammaById(ddl_Diagrammi.SelectedValue, this);
                if (DocsPAWA.DiagrammiManager.isDocumentiInStatoFinale(Convert.ToString(diagrammaAssociato.SYSTEM_ID), idTipoDoc, this))
                    result = true;

                //ArrayList statiFinali = new ArrayList();
                //DocsPAWA.DocsPaWR.Stato st = new DocsPAWA.DocsPaWR.Stato();
                //for(int i=0; i<diagrammaAssociato.STATI.Length; i++)
                //{
                //    st = (DocsPAWA.DocsPaWR.Stato) diagrammaAssociato.STATI[i];		
                //    if(st.STATO_FINALE)
                //        statiFinali.Add(st);
                //}
                //for(int j=0; j<statiFinali.Count; j++)
                //{
                //    if(DocsPAWA.DiagrammiManager.isDocumentiInStatoFinale(Convert.ToString(diagrammaAssociato.SYSTEM_ID),idTipoDoc,Convert.ToString( ((DocsPAWA.DocsPaWR.Stato) statiFinali[j]).SYSTEM_ID),this))
                //        result = true;
                //}
            }

            //Elimino l'associazione del diagramma perchè possibile
            if (result)
            {
                DocsPAWA.DiagrammiManager.disassociaTipoDocDiagramma(idTipoDoc, this);
                settaDllDiagrammi();
            }
            else
            {
                RegisterStartupScript("disassociazioneImpossibile", "<script>alert('Non è possibile cambiare diagramma ! \\nNon tutti i documenti sono in uno stato finale !');</script>");
            }
        }

        #endregion

        #region controlloValoriDuplicati
        public bool controllaValoriDuplicati(DropDownList ddl, string valore)
        {
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Text == valore)
                    return true;
            }
            return false;
        }
        #endregion

        #region Dg_ItemCreated
        protected void dg_listaTemplates_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }
        }

        protected void dg_Stati_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }
        }

        protected void dg_listaComponenti_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }
        }
        #endregion Dg_ItemCreated

        #region gestioneFormatoContatore
        protected void ddl_separatore_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddl_separatore.SelectedValue != "")
            {
                ddl_campiContatore.Enabled = true;
            }
            else
            {
                ddl_separatore.SelectedIndex = 0;
                ddl_campiContatore.Enabled = false;
            }
        }

        protected void img_btnAddCampoContatore_Click(object sender, ImageClickEventArgs e)
        {
            if (ddl_campiContatore.SelectedValue != "")
            {
                if (!txt_formatoContatore.Text.Contains(ddl_campiContatore.SelectedValue))
                {
                    if (txt_formatoContatore.Text == "")
                    {
                        txt_formatoContatore.Text = ddl_campiContatore.SelectedValue;
                    }
                    else
                    {
                        txt_formatoContatore.Text += ddl_separatore.SelectedValue + ddl_campiContatore.SelectedValue;
                    }
                }
            }
        }

        protected void img_btnDelCampoContatore_Click(object sender, ImageClickEventArgs e)
        {
            if (txt_formatoContatore.Text != "")
                txt_formatoContatore.Text = "";
        }

        protected void controllaEsistenzaRF()
        {
            if (!UserManager.existRf(idAmministrazione, this) && rbl_tipoContatore.Items.Count >= 3)
            {
                rbl_tipoContatore.Items.RemoveAt(2);
                ddl_campiContatore.Items.RemoveAt(1);
            }
        }
        #endregion gestionFormatoContatore

        #region Gestione Campi Comuni
        protected void btn_CampiComuni_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "popupCampiComuni", "apriPopupCampiComuni();", true);
        }

        private bool verificaEsistenzaCampiComuni()
        {
            for (int i = 0; i < listaTemplates.Count; i++)
            {
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)listaTemplates[i];
                if (template.IPER_FASC_DOC == "1")
                    return true;
            }
            return false;
        }
        #endregion Gestione Campi Comuni

        #region Gestione Link
        protected void ddl_tipoLink_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddl_tipoLink.SelectedValue.Equals("INTERNO"))
            {
                this.ddl_tipoObjLink.Enabled = true;
            }
            else
            {
                this.ddl_tipoObjLink.SelectedIndex = 0;
                this.ddl_tipoObjLink.Enabled = false;
            }
        }
        #endregion

        #region Contesto procedurale

        protected void btn_ContestoProcedurale_Click(object sender, EventArgs e)
        {

            string scriptString = "<SCRIPT>ApriContestoProcedurale();</SCRIPT>";

            ClientScript.RegisterStartupScript(GetType(), "OpenPopupContestoProcedurale", scriptString);

            string javascript = String.Format(
                "window.showModalDialog('{0}/AdminTool/Gestione_ProfDinamica/ContestoProcedurale.aspx', '', 'dialogWidth:588px;dialogHeight:350px; resizable: no;status:no;scroll:yes;help:no;close:no;center:yes;');",
                 Utils.getHttpFullPath());

            ClientScript.RegisterStartupScript(this.GetType(), "OpenPopupContestoProcedurale", javascript);
        }

        #endregion


        #region CheckedChanged(Default) MenuATendina - CasellaDiSelezione - SelezioneEsclusiva
        protected void cb_Default_MenuATendina_CheckedChanged(object sender, EventArgs e)
        {
            int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
            if (oggettoSelezionato != -1)
            {
                if (((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato] != null)
                {
                    DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato];
                    foreach (DocsPAWA.DocsPaWR.ValoreOggetto valoreOggetto in oggettoCustom.ELENCO_VALORI)
                    {
                        if (valoreOggetto.VALORE == ddl_valoriMenuATendina.SelectedItem.Text && cb_Default_MenuATendina.Checked)
                            valoreOggetto.VALORE_DI_DEFAULT = "SI";
                        else
                            valoreOggetto.VALORE_DI_DEFAULT = "NO";
                    }
                }
            }
        }

        protected void DdlColoriMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
            if (oggettoSelezionato != -1)
            {
                if (((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato] != null)
                {
                    DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato];
                    foreach (DocsPAWA.DocsPaWR.ValoreOggetto valoreOggetto in oggettoCustom.ELENCO_VALORI)
                    {
                        if (valoreOggetto.VALORE == ddl_valoriMenuATendina.SelectedItem.Text)
                            valoreOggetto.COLOR_BG = DdlColoriMenu.SelectedValue;
                    }
                }
            }
        }

        protected void DdlColoriSelezioneEsclusiva_SelectedIndexChanged(object sender, EventArgs e)
        {
            int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
            if (oggettoSelezionato != -1)
            {
                if (((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato] != null)
                {
                    DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato];
                    foreach (DocsPAWA.DocsPaWR.ValoreOggetto valoreOggetto in oggettoCustom.ELENCO_VALORI)
                    {
                        if (valoreOggetto.VALORE == ddl_valoriSelezioneEsclusiva.SelectedItem.Text)
                            valoreOggetto.COLOR_BG = DdlColoriSelezioneEsclusiva.SelectedValue;
                    }
                }
            }
        }

        protected void cb_Default_SelezioneEsclusiva_CheckedChanged(object sender, EventArgs e)
        {
            int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
            if (oggettoSelezionato != -1)
            {
                if (((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato] != null)
                {
                    DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato];
                    foreach (DocsPAWA.DocsPaWR.ValoreOggetto valoreOggetto in oggettoCustom.ELENCO_VALORI)
                    {
                        if (valoreOggetto.VALORE == ddl_valoriSelezioneEsclusiva.SelectedItem.Text && cb_Default_SelezioneEsclusiva.Checked)
                            valoreOggetto.VALORE_DI_DEFAULT = "SI";
                        else
                            valoreOggetto.VALORE_DI_DEFAULT = "NO";
                    }
                }
            }
        }

        protected void cb_Default_CasellaSelezione_CheckedChanged(object sender, EventArgs e)
        {
            int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
            if (oggettoSelezionato != -1)
            {
                if (((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato] != null)
                {
                    DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato];
                    foreach (DocsPAWA.DocsPaWR.ValoreOggetto valoreOggetto in oggettoCustom.ELENCO_VALORI)
                    {
                        if (valoreOggetto.VALORE == ddl_valoriCasellaSelezione.SelectedItem.Text && cb_Default_CasellaSelezione.Checked)
                            valoreOggetto.VALORE_DI_DEFAULT = "SI";
                        else
                            valoreOggetto.VALORE_DI_DEFAULT = "NO";
                    }
                }
            }
        }

        #endregion CheckedChanged(Default) MenuATendina - CasellaDiSelezione - SelezioneEsclusiva

        #region CheckedChanged(Disabilitato) MenuATendina - CasellaDiSelezione - SelezioneEsclusiva
        protected void cb_Disabilitato_CasellaSelezione_CheckedChanged(object sender, EventArgs e)
        {
            int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
            if (oggettoSelezionato != -1)
            {
                if (((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato] != null)
                {
                    DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato];
                    foreach (DocsPAWA.DocsPaWR.ValoreOggetto valoreOggetto in oggettoCustom.ELENCO_VALORI)
                    {
                        if (valoreOggetto.VALORE == ddl_valoriCasellaSelezione.SelectedItem.Text)
                        {
                            if (cb_Disabilitato_CasellaSelezione.Checked)
                                valoreOggetto.ABILITATO = 0;
                            else
                                valoreOggetto.ABILITATO = 1;
                        }
                    }
                }
            }
        }

        protected void cb_Disabilitato_SelezioneEsclusiva_CheckedChanged(object sender, EventArgs e)
        {
            int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
            if (oggettoSelezionato != -1)
            {
                if (((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato] != null)
                {
                    DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato];
                    foreach (DocsPAWA.DocsPaWR.ValoreOggetto valoreOggetto in oggettoCustom.ELENCO_VALORI)
                    {
                        if (valoreOggetto.VALORE == ddl_valoriSelezioneEsclusiva.SelectedItem.Text)
                        {
                            if (cb_Disabilitato_SelezioneEsclusiva.Checked)
                                valoreOggetto.ABILITATO = 0;
                            else
                                valoreOggetto.ABILITATO = 1;
                        }
                    }
                }
            }
        }

        protected void cb_Disabilitato_MenuATendina_CheckedChanged(object sender, EventArgs e)
        {
            int oggettoSelezionato = dg_listaComponenti.SelectedIndex;
            if (oggettoSelezionato != -1)
            {
                if (((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato] != null)
                {
                    DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)((DocsPAWA.DocsPaWR.Templates)Session["template"]).ELENCO_OGGETTI[oggettoSelezionato];
                    foreach (DocsPAWA.DocsPaWR.ValoreOggetto valoreOggetto in oggettoCustom.ELENCO_VALORI)
                    {
                        if (valoreOggetto.VALORE == ddl_valoriMenuATendina.SelectedItem.Text)
                        {
                            if (cb_Disabilitato_MenuATendina.Checked)
                                valoreOggetto.ABILITATO = 0;
                            else
                                valoreOggetto.ABILITATO = 1;
                        }
                    }
                }
            }
        }
        #endregion CheckedChanged(Disabilitato) MenuATendina - CasellaDiSelezione - SelezioneEsclusiva

        protected void cb_Ricerca_Contatore_CheckedChanged(object sender, EventArgs e)
        {
            cb_Contatore_visibile.Enabled = cb_Ricerca_Contatore.Checked;
        }

        protected void RadioButtonContatore_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadioButtonList rbl = (RadioButtonList)sender;

            ListItem li = rbl.SelectedItem;

            if (li.Value.ToUpper() == "CUSTOM")
            {
                DataInizio.Visible = true;
                DataFine.Visible = true;
                lblDataFine.Visible = true;
                lblDataInizio.Visible = true;
                cb_Repertorio.Enabled = false;
                cb_Azzera_Anno.Enabled = false;
                cb_Azzera_Anno.Checked = true;
                cb_Repertorio.Checked = false;
            }

            if (li.Value.ToUpper() == "CLASSICO")
            {
                DataInizio.Visible = false;
                DataFine.Visible = false;
                cb_Azzera_Anno.Enabled = true;
                cb_Azzera_Anno.Checked = false;
                lblDataFine.Visible = false;
                lblDataInizio.Visible = false;
                cb_Repertorio.Enabled = true;
            }

            //if (RadioButtonContatore.SelectedValue == "Custom")
            //{

            //    DataInizio.Visible = true;
            //    DataFine.Visible = true;
            //}
        }

        protected void cb_Repertorio_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_Repertorio.Checked)
                cb_consRep.Enabled = true;
            else
            {
                cb_consRep.Checked = false;
                cb_consRep.Enabled = false;
            }
        }   

        private bool ControllaDate(string Datainiziale, string Datafinale)
        {

            try
            {
                DateTime d1 = Convert.ToDateTime(Datainiziale);
                DateTime d2 = Convert.ToDateTime(Datafinale);
                if (d1 < d2)
                {
                    return true;
                }
                else

                    return false;
            }
            catch
            {

                return false;

            }

        }

        private bool ControllaDate2(string Datainiziale, string Datafinale)
        {

            try
            {

                int DataInizio = Convert.ToInt32(Datainiziale.Substring(6, 4));
                int DataFine = Convert.ToInt32(Datafinale.Substring(6, 4));
                if (DataInizio < DataFine)
                { return true; }
                else
                    return false;
            }
            catch
            {

                return false;
            }


        }

        private bool ControllaDate3(string Datainiziale, string Datafinale)
        {

            try
            {
                DateTime d1 = Convert.ToDateTime(Datainiziale);
                DateTime d2 = Convert.ToDateTime(Datafinale);
                d1 = d1.AddYears(1);
                
                
                if (d2<=d1 )
                { return true; }
                else
                    return false;
            }
            catch
            {

                return false;
            }


        }

        /// <summary>
        /// INTEGRAZIONE PITRE-PARER
        /// Determina se è attiva la conservazione PARER
        /// </summary>
        /// <returns></returns>
        protected bool IsConservazionePARER()
        {
            bool result = false;

            string IS_CONSERVAZIONE_PARER = string.Empty;
            IS_CONSERVAZIONE_PARER = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_WA_CONSERVAZIONE");
            result = ((string.IsNullOrEmpty(IS_CONSERVAZIONE_PARER) || IS_CONSERVAZIONE_PARER.Equals("0")) ? false : true);

            return result;
        }


        


    }
}
