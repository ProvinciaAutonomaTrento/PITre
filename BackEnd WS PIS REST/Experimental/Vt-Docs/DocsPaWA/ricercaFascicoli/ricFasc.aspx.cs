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
using Microsoft.Web.UI.WebControls;
using System.Globalization;
using System.Configuration;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;
using System.Collections.Generic;
using System.Linq;

namespace DocsPAWA.fascicoli
{
    public class browsingFasc : DocsPAWA.CssPage
    {
        #region Variabili
        protected System.Web.UI.WebControls.Panel pnl_profilazione;
        protected System.Web.UI.WebControls.Panel pnl_cons;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoFasc;
        protected System.Web.UI.WebControls.ImageButton img_dettagliProfilazione;
        protected DocsPaWR.DocsPaWebService wws = new DocsPaWR.DocsPaWebService();
        protected System.Web.UI.WebControls.Label lbl_ruolo;
        protected System.Web.UI.WebControls.DropDownList ddl_registri;
        protected System.Web.UI.WebControls.Image img_statoReg;
        protected System.Web.UI.WebControls.Button Button1;
        protected System.Web.UI.WebControls.TextBox TextBox1;
        protected System.Web.UI.WebControls.Label lbl_mostraTuttiFascicoli;
        protected System.Web.UI.WebControls.RadioButtonList rbl_MostraTutti;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_systemIdUffRef;
        private DocsPAWA.DocsPaWR.Registro[] userRegistri;
        private string flagCodice = "";
        private bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
            && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));
        //protected DocsPaWebCtrlLibrary.ImageButton btn_new_tit;
        protected System.Web.UI.WebControls.Button btn_new_tit;
        private const string KEY_SCHEDA_RICERCA = "RicercaFascicoli";
        private bool allClass;
        private DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
        private DocsPAWA.DocsPaWR.FiltroRicerca fV1;
        private DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
        private int indexH;
        private System.Collections.Hashtable TheHash;
        private const string navigatePageSearch = "NewTabSearchResult.aspx";
        private DocsPAWA.DocsPaWR.Ruolo userRuolo;
        private DocsPAWA.DocsPaWR.Registro userReg;
        private const int CONST_NUMBER_OF_DDL_CONTROLS = 6;
        private const string SESSION_HashTableListControls = "HashTableListControls";
        private DocsPAWA.DocsPaWR.FascicolazioneClassificazione m_classificazioneSelezionata;
        private const string CONST_BASE_NAME_CONTROL_OF_LIST = "ddl_livello";
        private const int CONST_BASE_INDEX_CONTROL_OF_LIST = 1;
        protected bool viewCodiceInCombo;
        private ArrayList storedScriptCall = new ArrayList();
        private string nameScript = "script";
        //protected DocsPaWebCtrlLibrary.ImageButton btn_new;
        protected System.Web.UI.WebControls.Button btn_new;
        protected DocsPaWebCtrlLibrary.ImageButton btn_del;
        //protected DocsPaWebCtrlLibrary.ImageButton btn_ricFascicoli;
        protected System.Web.UI.WebControls.Button btn_ricFascicoli;
        protected System.Web.UI.WebControls.Button btn_salva;
        protected System.Web.UI.WebControls.Image icoReg;
        protected System.Boolean l_createFascicolo;
        protected System.Web.UI.WebControls.Label lbl_esito;
        protected System.Web.UI.WebControls.DropDownList ddl_ragioni;
        protected System.Web.UI.WebControls.RadioButtonList rbl_esito;
        protected System.Web.UI.WebControls.Label lbl_initdataA;
        protected System.Web.UI.WebControls.Label lbl_finedataA;
        protected System.Web.UI.WebControls.Label lbl_initdataC;
        protected System.Web.UI.WebControls.Label lbl_finedataC;
        protected DocsPAWA.UserControls.Calendar txt_initDataA;
        protected DocsPAWA.UserControls.Calendar txt_fineDataA;
        protected DocsPAWA.UserControls.Calendar txt_initDataC;
        protected DocsPAWA.UserControls.Calendar txt_fineDataC;
        protected System.Web.UI.WebControls.DropDownList ddl_dataA;
        protected System.Web.UI.WebControls.DropDownList ddl_dataC;
        protected System.Web.UI.WebControls.Label lblAnnoFasc;
        protected System.Web.UI.WebControls.Label lbl_NumFasc;
        protected System.Web.UI.WebControls.TextBox txtNumFasc;
        protected System.Web.UI.WebControls.TextBox txtAnnoFasc;
        protected System.Web.UI.WebControls.Label lbl_Stato;
        protected System.Web.UI.WebControls.Label lbl_tipo;
        protected System.Web.UI.WebControls.DropDownList ddlStato;
        protected System.Web.UI.WebControls.DropDownList ddlTipo;
        protected System.Web.UI.WebControls.Label lbl_descr;
        protected System.Web.UI.WebControls.TextBox txtDescr;
        protected System.Web.UI.WebControls.Label ldl_apertA;
        protected System.Web.UI.WebControls.Label lbl_dtaC;
        protected System.Web.UI.WebControls.Label lbl_dtaCreaz;
        protected System.Web.UI.WebControls.DropDownList ddl_creaz;
        protected System.Web.UI.WebControls.Label lbl_initCreaz;
        protected System.Web.UI.WebControls.Label lbl_finCreaz;
        protected DocsPAWA.UserControls.Calendar txt_initDataCrea;
        protected DocsPAWA.UserControls.Calendar txt_fineDataCrea;
        protected System.Web.UI.WebControls.Label lbl_codClass;
        protected System.Web.UI.WebControls.TextBox txt_codClass;
        protected System.Web.UI.WebControls.Label lbl_livello1;
        protected System.Web.UI.WebControls.DropDownList ddl_livello1;
        protected System.Web.UI.WebControls.Label lbl_livello2;
        protected System.Web.UI.WebControls.DropDownList ddl_livello2;
        protected System.Web.UI.WebControls.Label lbl_livello3;
        protected System.Web.UI.WebControls.DropDownList ddl_livello3;
        protected System.Web.UI.WebControls.Label lbl_livello4;
        protected System.Web.UI.WebControls.DropDownList ddl_livello4;
        protected System.Web.UI.WebControls.Label lbl_livello5;
        protected System.Web.UI.WebControls.DropDownList ddl_livello5;
        protected System.Web.UI.WebControls.Label lbl_livello6;
        protected System.Web.UI.WebControls.DropDownList ddl_livello6;
        protected System.Web.UI.WebControls.Label Label1;
        protected System.Web.UI.WebControls.TextBox txt_RicTit;
        protected DocsPaWebCtrlLibrary.ImageButton btn_ricTit;
        protected Microsoft.Web.UI.WebControls.TreeView Titolario;
        protected System.Web.UI.WebControls.RadioButtonList rblst11;
        protected System.Web.UI.WebControls.RadioButtonList rbLst1;
        protected DocsPaWebCtrlLibrary.ImageButton btn_titolario;
        protected System.Web.UI.WebControls.RadioButtonList OptLst;
        //protected System.Web.UI.WebControls.Button ButtonRicercaClassDaProt;
        protected System.Web.UI.WebControls.Label Label2;
        protected System.Web.UI.WebControls.DropDownList ddl_data_LF;
        protected System.Web.UI.WebControls.DropDownList ddl_titolari;
        protected System.Web.UI.WebControls.Label lbl_dta_LF_DA;
        protected DocsPAWA.UserControls.Calendar txt_dta_LF_DA;
        protected System.Web.UI.WebControls.Label lbl_dta_LF_A;
        protected DocsPAWA.UserControls.Calendar txt_dta_LF_A;
        protected System.Web.UI.WebControls.Label Label3;
        protected System.Web.UI.WebControls.TextBox txt_descr_LF;
        protected System.Web.UI.WebControls.TextBox txt_varCodRubrica_LF;
        protected System.Web.UI.WebControls.Label lbl_uffRef;
        protected System.Web.UI.WebControls.TextBox txt_cod_UffRef;
        protected System.Web.UI.WebControls.TextBox txt_desc_uffRef;
        protected System.Web.UI.WebControls.Panel pnl_uffRef;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_systemIdLF;
        protected System.Web.UI.WebControls.Image btn_Rubrica;
        protected System.Web.UI.WebControls.Image btn_rubricaRef;
        protected System.Web.UI.WebControls.Label lblNote;
//        protected System.Web.UI.WebControls.TextBox txtNote;
        protected UserControls.RicercaNote rn_note;
        private int indexScript = 0;

        protected System.Web.UI.WebControls.Panel pnl_Sottofascicoli;
        protected System.Web.UI.WebControls.TextBox txt_sottofascicolo;

        protected System.Web.UI.WebControls.DropDownList ddl_dataScadenza;
        protected System.Web.UI.WebControls.Label lbl_dataScadenza_DA;
        protected DocsPAWA.UserControls.Calendar txt_dataScadenza_DA;
        protected System.Web.UI.WebControls.Label lbl_dataScadenza_A;
        protected DocsPAWA.UserControls.Calendar txt_dataScadenza_A;
        protected System.Web.UI.WebControls.Panel Panel_StatiDocumento;
        protected System.Web.UI.WebControls.Label lbl_statiDoc;
        protected System.Web.UI.WebControls.DropDownList ddl_statiDoc;
        protected System.Web.UI.WebControls.CheckBox cb_Conservato;
        protected System.Web.UI.WebControls.CheckBox cb_NonConservato;
        private bool optListCreatorChanged = false;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtSystemIdUtenteCreatore;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtTipoCorrispondente;
        //protected System.Web.UI.WebControls.Panel pnl_trasfDesp;
        protected System.Web.UI.WebControls.RadioButtonList rbl_TrasfDep;
        protected System.Web.UI.HtmlControls.HtmlLink styleLink;
        protected System.Web.UI.WebControls.Label lbl_Titolari;
        protected System.Web.UI.WebControls.Label lbl_protoTitolario;
        protected System.Web.UI.WebControls.TextBox txt_protoPratica;
        protected System.Web.UI.WebControls.DropDownList ddl_Ric_Salvate;
        private bool isSavedSearch = false;
        public DocsPAWA.ricercaDoc.SchedaRicerca schedaRicerca = null;
        protected DocsPAWA.DocsPaWR.Utente userHome;
        protected System.Web.UI.WebControls.ImageButton btn_Canc_Ric;
        protected Utilities.MessageBox mb_ConfirmDelete;

        protected System.Web.UI.WebControls.CheckBox cbx1;
        protected System.Web.UI.WebControls.CheckBox cbx2;
        protected System.Web.UI.WebControls.CheckBox CheckBox1;
        protected System.Web.UI.WebControls.CheckBox CheckBox2;
        protected System.Web.UI.WebControls.CheckBox CheckBox3;
        protected System.Web.UI.WebControls.CheckBox CheckBox4;
        protected System.Web.UI.WebControls.CheckBox CheckBox5;
        protected System.Web.UI.WebControls.CheckBox CheckBox6;
        protected System.Web.UI.WebControls.CheckBox CheckBox7;
        protected System.Web.UI.WebControls.CheckBox CheckBox8;
        protected System.Web.UI.WebControls.DropDownList ddl;

        protected System.Web.UI.WebControls.DropDownList ddlOrder, ddlOrderDirection;

        protected bool change_from_grid;

        protected int numFasc;

        DocsPaWR.Fascicolo[] listaFasc;

        protected bool no_custom_grid_cont;
        protected System.Web.UI.WebControls.Panel pnl_visiblitaFasc;
        protected System.Web.UI.WebControls.RadioButtonList rbl_visibilita;

        protected System.Web.UI.WebControls.Button btn_modifica;

        protected string numResult;

        protected DocsPaWebCtrlLibrary.ImageButton btn_clear_fields;

        protected DocsPAWA.UserControls.AuthorOwnerFilter aofAuthor, aofOwner;

        #endregion Variabili

        #region Sezione gestione script JavaScript lato client
        //è necessario che alla fine delal sezione HTML della pagina
        //sia presente il seguente codice:
        //
        //				<script language="javascript">
        //					esecuzioneScriptUtente();
        //				</script>
        //
        //nella page_Load:		
        //				if (!IsPostBack)
        //				{
        //					generaFunctionChiamataScript();
        //				}
        //chiamare addScript() per aggiungere gli script 
        //da eseguire alla fine della routine che ne richiede
        //l'esecuzione. Infine, eseguire generaFunctionChiamataScript()

        private void addScript(string scriptBody)
        {
            indexScript++;
            string newScriptName = nameScript + indexScript.ToString();
            creaScript(newScriptName, scriptBody);
        }

        private void creaScript(string nameScript, string scriptBody)
        {
            try
            {
                //crea funxione script
                string script = "<script language=\"javascript\">" +
                    "function " + nameScript + "(){" + scriptBody + "}</script>";
                Response.Write(script);

                //crea chiamata alla funzione
                storedScriptCall.Add(nameScript);
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void generaFunctionChiamataScript()
        {
            try
            {
                Response.Write("<script language=\"javascript\">");
                Response.Write("function esecuzioneScriptUtente()");
                Response.Write("{");
                for (int i = 0; i < storedScriptCall.Count; i++)
                {
                    string call = (string)storedScriptCall[i];
                    Response.Write(call + "();");
                }
                Response.Write("}");
                Response.Write("</script>");
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private string getJSToOpenResultPage(int idFascicolo)
        {
            string newUrl;
            newUrl = navigatePageSearch + "?idClass=" + idFascicolo;
            return newUrl;
        }

        private void AddControlsClientAttribute()
        {
            //this.btnShowRubrica.Attributes.Add("onClick", "_ApriRubrica('gf_proprietario');");
            //this.btnShowRubrica.Attributes.Add("onClick", "ShowDialogRubrica('" + this.txtTipoCorrispondente.ClientID + "');");
            //this.txt_protoPratica.Attributes.Add("onKeyPress", "ValidateNumericKey();");
        }

        #endregion Sezione gestione script JavaScript lato client

        #region Parametri
        private void getParametri()
        {
            //recupera i parametri dalla session o vengono
            //costruiti appositamente in base ai parametri
            //ricevuti.
            try
            {
                if (!IsPostBack)
                {
                    buildParametriPagina();
                }
                else
                {
                    //post back:
                    //recupero eventuali dati già creati per la 
                    //pagina e memorizzati in session
                    getParametriPaginaInSession();
                }
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void getParametriPaginaInSession()
        {
            //recupero i parametri che devono essere
            //disponibili alla pagina tra le varie sequenze di postback
            try
            {
                m_classificazioneSelezionata = FascicoliManager.getClassificazioneSelezionata(this);
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void buildParametriPagina()
        {
            //creo i dati che dovrà gestire la pagina
            try
            {
                //eseguo l'inizializzazione della ddl dei titolari
                caricaComboTitolari();
                if (ddl_titolari.Items.Count != 0)
                {
                    //eseguo l'inizializzazione dei controlli sulla pagina
                    if (!this.IsPostBack && this.OnBack)
                        RestoreSearchFilters();
                    caricamentoClassificazioniPadri();
                    btn_ricFascicoli.Enabled = true;
                }
                else
                {
                    btn_ricFascicoli.Enabled = false;
                }
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void caricamentoClassificazioniPadri()
        {
            //Vengono estratti i nodi di livello 1 (nodi padri) queryList: S_J_PROJECT__SECURITY
            //In base al titolario selezionato
            //DocsPaWR.FascicolazioneClassifica[] classifiche = FascicoliManager.GetFigliClassifica(this, null, UserManager.getUtente(this).idAmministrazione);
            //DocsPaWR.FascicolazioneClassifica[] classifiche = FascicoliManager.GetFigliClassifica2(this, null, UserManager.getUtente(this).idAmministrazione, ddl_titolari.SelectedValue);
            DocsPaWR.FascicolazioneClassifica[] classifiche = FascicoliManager.GetFigliClassifica2(this, null, UserManager.getUtente(this).idAmministrazione, getIdTitolario(null));
            //Carica la dropDownList di livello 1
            ddl_livello1.Items.Clear();
            caricaCombo(0, classifiche);
        }

        private void gestioneParametri()
        {
            //gestisce i parametri per il rendering della 
            //pagina.
            try
            {
                if (IsPostBack)
                {
                    if (m_classificazioneSelezionata != null && Session["Titolario"] != null && Session["Titolario"].ToString() == "Y")
                    {
                        this.txt_codClass.Text = m_classificazioneSelezionata.codice;
                        //Session["classificazione"] = m_classificazioneSelezionata;
                        cercaClassificazioneDaCodice();
                        //m_classificazioneSelezionata = null;
                        FascicoliManager.setClassificazioneSelezionata(this, m_classificazioneSelezionata);
                        Session.Remove("Titolario");
                        impostaAbilitazioneNuovoFascNuovoTit();
                        //btn_new.Enabled = true;
                        //this.btn_new_tit.Enabled = true;
                    }
                }
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void getParameterUser()
        {
            userRuolo = UserManager.getRuolo(this);
            userReg = UserManager.getRegistroSelezionato(this);
        }
        #endregion Parametri

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
            this.ddl_registri.SelectedIndexChanged += new System.EventHandler(this.ddl_registri_SelectedIndexChanged);
            // this.ButtonRicercaClassDaProt.Click += new System.EventHandler(this.ButtonRicercaClassDaProt_Click);
            this.btn_titolario.Click += new System.Web.UI.ImageClickEventHandler(this.btn_titolario_Click);
            this.txt_codClass.TextChanged += new System.EventHandler(this.txt_codClass_TextChanged);
            this.ddl_livello1.SelectedIndexChanged += new System.EventHandler(this.ddl_livelloControl_SelectedIndexChanged);
            this.ddl_livello2.SelectedIndexChanged += new System.EventHandler(this.ddl_livelloControl_SelectedIndexChanged);
            this.ddl_livello3.SelectedIndexChanged += new System.EventHandler(this.ddl_livelloControl_SelectedIndexChanged);
            this.ddl_livello4.SelectedIndexChanged += new System.EventHandler(this.ddl_livelloControl_SelectedIndexChanged);
            this.ddl_livello5.SelectedIndexChanged += new System.EventHandler(this.ddl_livelloControl_SelectedIndexChanged);
            this.ddl_livello6.SelectedIndexChanged += new System.EventHandler(this.ddl_livelloControl_SelectedIndexChanged);
            this.ddl_dataA.SelectedIndexChanged += new System.EventHandler(this.ddl_data_SelectedIndexChanged);
            this.ddl_dataC.SelectedIndexChanged += new System.EventHandler(this.ddl_dataC_SelectedIndexChanged);
            this.ddl_creaz.SelectedIndexChanged += new System.EventHandler(this.ddl_creaz_SelectedIndexChanged);
            this.ddl_dataScadenza.SelectedIndexChanged += new System.EventHandler(this.ddl_dataScadenza_SelectedIndexChanged);
            this.ddlTipo.SelectedIndexChanged += new System.EventHandler(this.ddlTipo_SelectedIndexChanged);
            this.ddl_data_LF.SelectedIndexChanged += new System.EventHandler(this.ddl_data_LF_SelectedIndexChanged);
            this.txt_varCodRubrica_LF.TextChanged += new System.EventHandler(this.txt_varCodRubrica_LF_TextChanged);
            this.txt_cod_UffRef.TextChanged += new System.EventHandler(this.txt_cod_UffRef_TextChanged);
            this.rbl_MostraTutti.SelectedIndexChanged += new System.EventHandler(this.rbl_MostraTutti_SelectedIndexChanged);
            this.btn_ricFascicoli.Click += new System.EventHandler(this.btn_ricFascicoli_Click);
            this.btn_new.Click += new System.EventHandler(this.btn_new_Click);
            //this.btn_new_tit.Click += new System.Web.UI.ImageClickEventHandler(this.btn_new_tit_Click);
            this.btn_new_tit.Click += new System.EventHandler(this.btn_new_tit_Click);
            this.img_dettagliProfilazione.Click += new ImageClickEventHandler(img_dettagliProfilazione_Click);
            this.ddl_tipoFasc.SelectedIndexChanged += new EventHandler(ddl_tipoFasc_SelectedIndexChanged);
            this.ddl_titolari.SelectedIndexChanged += new EventHandler(ddl_titolari_SelectedIndexChanged);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);
            this.cb_Conservato.CheckedChanged += new EventHandler(cb_Conservato_CheckedChanged);
            this.cb_NonConservato.CheckedChanged += new EventHandler(cb_NonConservato_CheckedChanged);
            this.btn_salva.Click += new EventHandler(btn_salva_Click);
            this.ddl_Ric_Salvate.SelectedIndexChanged += new EventHandler(ddl_Ric_Salvate_SelectedIndexChanged);
            this.btn_Canc_Ric.Click += new ImageClickEventHandler(btn_Canc_Ric_Click);
            this.mb_ConfirmDelete.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.mb_ConfirmDelete_GetMessageBoxResponse);
            this.btn_clear_fields.Click += new ImageClickEventHandler(this.CleanCorrFilters);
        }

        protected void CleanCorrFilters(object sender, EventArgs e)
        {
            this.aofOwner.ClearFilters();
            this.aofAuthor.ClearFilters();
        }

        void btn_salva_Click(object sender, EventArgs e)
        {
            if (ricercaFascicoli())
            {
                // Salvataggio del filtro di ricerca
                GridManager.SetSearchFilter(this.ddlOrder.SelectedItem.Text, this.ddlOrderDirection.SelectedValue);

                schedaRicerca.FiltriRicerca = FascicoliManager.getFiltroRicFasc(this);
                schedaRicerca.ProprietaNuovaRicerca = new DocsPAWA.ricercaDoc.SchedaRicerca.NuovaRicerca();
                if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1"))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "salvaRicerca", "apriSalvaRicercaADL();", true);
                    //RegisterStartupScript("SalvaRicerca", "<script>apriSalvaRicercaADL();</script>");
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "salvaRicerca", "apriSalvaRicerca();", true);
                    // RegisterStartupScript("SalvaRicerca", "<script>apriSalvaRicerca();</script>");
                }
            }
        }

        void cb_NonConservato_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cb_NonConservato.Checked)
                this.cb_Conservato.Checked = false;
        }

        void cb_Conservato_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cb_Conservato.Checked)
                this.cb_NonConservato.Checked = false;
        }

        #endregion Web Form Designer generated code

        //#region Proprietario
        //protected void btnShowRubrica_Click(object sender, ImageClickEventArgs e)
        //{
        //    this.FillDatiCorrispondenteDaRubrica();
        //}

        ///// <summary>
        ///// Caricamento dati corrispondente selezionato dalla rubrica
        ///// </summary>
        //private void FillDatiCorrispondenteDaRubrica()
        //{
        //    DocsPaWR.Corrispondente selectedCorr = UserManager.getCreatoreSelezionato(this.Page);
        //    if (selectedCorr != null)
        //    {
        //        switch (selectedCorr.tipoCorrispondente)
        //        {
        //            case "P":
        //                DocsPaWR.Utente ut = (DocsPaWR.Utente)selectedCorr;
        //                this.txtSystemIdUtenteCreatore.Value = ut.idPeople;
        //                this.txtCodiceUtenteCreatore.Text = ut.codiceRubrica;
        //                this.txtDescrizioneUtenteCreatore.Text = ut.descrizione;
        //                this.optListTipiCreatore.SelectedValue = "P";
        //                this.cbx_UOsotto.Checked = false;
        //                this.cbx_UOsotto.Visible = false;
        //                break;

        //            case "R":
        //                DocsPaWR.Ruolo ruo = (DocsPaWR.Ruolo)selectedCorr;
        //                this.txtSystemIdUtenteCreatore.Value = ruo.idGruppo;
        //                this.txtCodiceUtenteCreatore.Text = ruo.codiceRubrica;
        //                this.txtDescrizioneUtenteCreatore.Text = ruo.descrizione;
        //                this.optListTipiCreatore.SelectedValue = "R";
        //                this.cbx_UOsotto.Checked = false;
        //                this.cbx_UOsotto.Visible = false;
        //                break;

        //            case "U":
        //                DocsPaWR.UnitaOrganizzativa uo = (DocsPaWR.UnitaOrganizzativa)selectedCorr;
        //                this.txtSystemIdUtenteCreatore.Value = uo.systemId;
        //                this.txtCodiceUtenteCreatore.Text = uo.codiceRubrica;
        //                this.txtDescrizioneUtenteCreatore.Text = uo.descrizione;
        //                this.optListTipiCreatore.SelectedValue = "U";
        //                //this.cbx_UOsotto.Checked = false;
        //                this.cbx_UOsotto.Visible = true;
        //                break;
        //        }
        //        selectedCorr = null;
        //    }
        //    else
        //    {
        //        this.txtSystemIdUtenteCreatore.Value = string.Empty;
        //        this.txtCodiceUtenteCreatore.Text = string.Empty;
        //        this.txtDescrizioneUtenteCreatore.Text = string.Empty;
        //    }
        //}

        //protected void optListTipiCreatore_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    //this.PerformActionSelectTipoCreatore();
        //    this.txtTipoCorrispondente.Value = this.optListTipiCreatore.SelectedItem.Value;
        //    this.optListCreatorChanged = true;
        //    this.txtCodiceUtenteCreatore.Text = "";
        //    this.txtDescrizioneUtenteCreatore.Text = "";
        //    if (this.txtTipoCorrispondente.Value.Equals("U"))
        //    {
        //        this.cbx_UOsotto.Visible = true;
        //    }
        //    else
        //    {
        //        this.cbx_UOsotto.Checked = false;
        //        this.cbx_UOsotto.Visible = false;
        //    }
        //}

        //protected void txtCodiceUtenteCreatore_TextChanged(object sender, EventArgs e)
        //{
        //    string codiceCreat = this.txtCodiceUtenteCreatore.Text;
        //    DocsPaWR.Corrispondente corrispondente = null;

        //    if (codiceCreat != string.Empty)
        //    {
        //        // Reperimento oggetto corrispondente dal codice immesso dall'utente
        //        corrispondente = this.GetCorrispondenteDaCodice(codiceCreat);

        //        if (corrispondente == null)
        //        {
        //            this.Page.RegisterClientScriptBlock("CodiceRubricaNonTrovato", "<script language=javascript>alert('Codice rubrica non trovato');</script>");
        //            UserManager.removeCreatoreSelezionato(this.Page);
        //        }
        //        else
        //        {
        //            UserManager.setCreatoreSelezionato(this.Page, corrispondente);

        //            // Impostazione del tipo corrispondente corretto
        //            if (corrispondente.GetType().Equals(typeof(DocsPaWR.Utente)))
        //            {
        //                this.cbx_UOsotto.Checked = false;
        //                this.cbx_UOsotto.Visible = false;
        //                this.optListTipiCreatore.SelectedValue = "P";
        //            }
        //            else if (corrispondente.GetType().Equals(typeof(DocsPaWR.Ruolo)))
        //            {
        //                this.cbx_UOsotto.Checked = false;
        //                this.cbx_UOsotto.Visible = false;
        //                this.optListTipiCreatore.SelectedValue = "R";
        //            }
        //            else if (corrispondente.GetType().Equals(typeof(DocsPaWR.UnitaOrganizzativa)))
        //            {
        //                this.cbx_UOsotto.Visible = true;
        //                this.optListTipiCreatore.SelectedValue = "U";
        //            }
        //        }
        //        this.txtCodiceUtenteCreatore.Focus();

        //        // Caricamento dati corrispondente
        //        this.FillDatiCorrispondenteDaRubrica();
        //    }
        //    else
        //    {
        //        this.txtCodiceUtenteCreatore.Text = "";
        //        this.txtDescrizioneUtenteCreatore.Text = "";
        //        UserManager.removeCreatoreSelezionato(this.Page);

        //    }
        //}

        ///// <summary>
        ///// Reperimento di un corrispondente in base ad un codice rubrica fornito in ingresso
        ///// </summary>
        ///// <param name="page"></param>
        ///// <param name="codCorrispondente"></param>
        ///// <returns></returns>
        //private DocsPaWR.Corrispondente GetCorrispondenteDaCodice(string codCorrispondente)
        //{
        //    DocsPaWR.Corrispondente retValue = null;

        //    if (codCorrispondente != null)
        //        retValue = UserManager.getCorrispondente(this.Page, codCorrispondente, true);

        //    return retValue;
        //}

        //#endregion

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

        private void mb_ConfirmDelete_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                try
                {

                    schedaRicerca.Cancella(Int32.Parse(ddl_Ric_Salvate.SelectedValue));
                    Session[DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY] = null;
                    this.ddl_Ric_Salvate.SelectedIndex = 0;
                    FascicoliManager.removeFiltroRicFasc(this);
                    //Response.Write("<script>top.principale.iFrame_dx.document.location = 'tabRisultatiRicFasc.aspx';</scirpt>");
                    Response.Write("<script>alert(\"Il criterio di ricerca è stato rimosso\");window.location.href = window.location.href;</script>");
                }
                catch (Exception ex)
                {
                    string msg = "Impossibile rimuovere i criteri di ricerca. Errore: " + ex.Message;
                    msg = msg.Replace("\"", "\\\"");
                    Response.Write("<script>alert(\"" + msg + "\");window.location.href = window.location.href;</script>");
                }
            }
        }

        #region Eventi pagina
        private void Page_PreRender(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
                // Caricamento delle combo con le informazioni sull'ordinamento
                GridManager.CompileDdlOrderAndSetOrderFilterProjects(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);

            string Tema = GetCssAmministrazione();

            if (Tema != null && !Tema.Equals(""))
            {
                string[] realTema = Tema.Split('^');
                this.styleLink.Href = "../App_Themes/" + realTema[0] + "/" + realTema[0] + ".css";
            }
            else
                this.styleLink.Href = "../App_Themes/TemaRosso/TemaRosso.css";


            if (IsAbilitataRicercaSottoFascicoli())
                pnl_Sottofascicoli.Visible = true;
            else
                pnl_Sottofascicoli.Visible = false;

            //if (Session["cha_ReadOnly"] != null)
            if(Session["classificaSelezionata"] != null)
            {
                DocsPaWR.FascicolazioneClassifica classifica = (DocsPaWR.FascicolazioneClassifica)Session["classificaSelezionata"];
                //if ((bool)Session["cha_ReadOnly"] == true)
                if (classifica.cha_ReadOnly)
                {
                    impostaAbilitazioneNuovoFascNuovoTit();
                    //this.btn_new.Enabled=false;
                    //this.btn_new_tit.Enabled = false;
                }
                else
                {
                    if (IsStartupScriptRegistered("NoCod"))//se sono apparsi dei messaggi di errore disabilito il pulsante nuovo
                    {
                        impostaAbilitazioneNuovoFascNuovoTit();
                        //this.btn_new.Enabled = false;
                        //this.btn_new_tit.Enabled = false;
                    }
                    else
                    {
                        //if (this.txt_codClass.Text != "" && ddl_titolari.SelectedItem.Text != "Tutti i titolari")
                        if (this.txt_codClass.Text != "" && checkRicercaFasc(this.txt_codClass.Text) == "SI_RICERCA")
                        {
                            impostaAbilitazioneNuovoFascNuovoTit();
                        }
                    }
                }
            }
            // Inizializzazione condizionale link rubrica
            string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
            if (use_new_rubrica != "1")
                btn_Rubrica.Attributes["onClick"] = "ApriRubricaDaRicFasc('fascLF','U');";
            else
                btn_Rubrica.Attributes["onClick"] = "_ApriRubrica('gf_locfisica');";

            btn_Rubrica.Attributes["onMouseOver"] = "javascript:ImpostaCursore (1,'" + btn_Rubrica.ClientID + "');";
            btn_Rubrica.Attributes["onMouseOut"] = "javascript:ImpostaCursore (0,'" + btn_Rubrica.ClientID + "');";

            if (enableUfficioRef)
            {
                btn_rubricaRef.Attributes["onMouseOver"] = "javascript:ImpostaCursore (1,'" + btn_rubricaRef.ClientID + "');";
                btn_rubricaRef.Attributes["onMouseOut"] = "javascript:ImpostaCursore (0,'" + btn_rubricaRef.ClientID + "');";
            }

            if (!this.Page.IsClientScriptBlockRegistered("imposta_cursore"))
            {
                this.Page.RegisterClientScriptBlock("imposta_cursore",
                    "<script language=\"javascript\">\n" +
                    "function ImpostaCursore (t, ctl)\n{\n" +
                    "document.getElementById(ctl).style.cursor = (t == 0) ? 'default' : 'hand';\n" +
                    "}\n</script>\n");
            }

            // Gestione "back": viene effettuata la ricerca
            // in base ai filtri correntemente attivi
            if (!this.IsPostBack && this.OnBack)
            {
                // Ripristino filtri di ricerca
                this.RestoreSearchFilters();

                caricaTitolariRegistro(false);

                // Ricostruzione del CallContext con i filtri di ricerca correntemente in sessione
                DocsPaWR.FiltroRicerca[][] filters = FascicoliManager.getFiltroRicFasc(this);
                if (filters != null)
                    this.SetFiltersOnCurrentContext(filters);

                string url = "NewTabSearchResult.aspx?back=true&tabRes=fascicoli";
                if (this.Request.QueryString["fascIndex"] != null)
                    url += "&fascIndex=" + this.Request.QueryString["fascIndex"];

                //if (!txt_codClass.Text.Equals("") && ddl_titolari.SelectedItem.Text != "Tutti i titolari")
                if (!this.txt_codClass.Text.Equals("") && checkRicercaFasc(this.txt_codClass.Text) == "SI_RICERCA")
                {
                    url += "&idClass=1";
                }

                Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='" + url + "';</script>");
            }
            // personalizzazzione label data collocazione fisica da web.config.
            //if (Utils.label_data_Loc_fisica.Trim() != "")
            //    this.Label2.Text = Utils.label_data_Loc_fisica;
            //else
            //    this.Label2.Text = "Data coll.";

            string new_search = string.Empty;
            if (ViewState["new_search"] != null)
            {
                new_search = ViewState["new_search"] as string;
                ViewState["new_search"] = null;
            }

            if (change_from_grid && string.IsNullOrEmpty(new_search))
            {
                GridManager.CompileDdlOrderAndSetOrderFilterProjects(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                if (ricercaFascicoli())
                {
                    schedaRicerca.FiltriRicerca = qV;
                    FascicoliManager.setFiltroRicFasc(this, qV);
                    FascicoliManager.removeDatagridFascicolo(this);
                    FascicoliManager.removeListaFascicoliInGriglia(this);
                    change_from_grid = false;

                    string altro = string.Empty;

                    if (!string.IsNullOrEmpty(this.numResult) && this.numResult.Equals("0"))
                    {
                        altro = "&noRic=1";
                    }

                    if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                    {
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli" + altro + "';</script>");
                        //       ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=estesa&tabRes=estesa" + altro + "';", true);
                    }
                    else
                    {
                        //     ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?from=estesa&tabRes=estesa" + altro + "';", true);
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?tabRes=fascicoli" + altro + "';</script>");
                    }
                }
            }

            //Visualizzazione pannello visbilità tipica / atipica
            if (Utils.GetAbilitazioneAtipicita())
                pnl_visiblitaFasc.Visible = true;
        }

        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["rubrica.campoCorrispondente"] != null)
                    Session.Remove("rubrica.campoCorrispondente");
                if (Session["dictionaryCorrispondente"] != null)
                    Session.Remove("dictionaryCorrispondente");
                
                if (Request.QueryString["gridper"] != string.Empty && Request.QueryString["gridper"] != null)
                {
                    change_from_grid = true;
                }
                else
                {
                    change_from_grid = false;
                }
            }

            if (Request.QueryString["numRes"] != string.Empty && Request.QueryString["numRes"] != null)
            {
                this.numResult = Request.QueryString["numRes"];
            }
            else
            {
                this.numResult = string.Empty;
            }

            this.Page.MaintainScrollPositionOnPostBack = true;

            Utils.startUp(this);
            //SetDefaultButton(this.Page,txt_codClass,ButtonRicercaClassDaProt);
            Utils.DefaultButton(this, ref txt_codClass, ref btn_ricFascicoli);
            //Utils.DefaultButton(this, ref txt_protoPratica, ref btn_ricFascicoli);
            getParameterUser();
            string tema = this.Page.Theme;

            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Project)
            {
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Project);
            }

            //Se la chiave sul web.config del Wa è settata a 1 allora le comboBox in DocClassifica
            //riporteranno oltra alla descrizione anche il codice del nodo di titolario
            viewCodiceInCombo = (System.Configuration.ConfigurationManager.AppSettings["VIEW_CODICE_IN_COMBO_CLASSIFICA"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["VIEW_CODICE_IN_COMBO_CLASSIFICA"].Equals("1"));

            impostaVisibiltaBtnFascTit();
            //if (ConfigSettings.getKey(ConfigSettings.KeysENUM.CONSERVAZIONE) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.CONSERVAZIONE).ToUpper().Equals("1"))
            //    this.pnl_cons.Visible = true;
            //else
            //    this.pnl_cons.Visible = false;

            if (UserManager.FunzioneEsistente(this, "DO_CONS"))
            {
                this.pnl_cons.Visible = true;
            }
            else
            {
                this.pnl_cons.Visible = false;
            }

            //Trasferimento deposito
            //if (UserManager.ruoloIsAutorized(this, "GEST_ARCHIVIA"))
            //    this.pnl_trasfDesp.Visible = true;
            //else
            //    this.pnl_trasfDesp.Visible = false;
            //this.pnl_trasfDesp.Visible = false;

            try
            {
                userHome = (DocsPAWA.DocsPaWR.Utente)Session["userData"];

                string Tema = GetCssAmministrazione();
                string rigaDescrizione = "<tr><td align=\"center\" height=\"15\" class=\"titolo_bianco\" bgcolor=\"#810d06\">Registro</td></tr>";

                if (Tema != null && !Tema.Equals(""))
                {
                    string[] temaSplit = Tema.Split('^');
                    rigaDescrizione = "<tr><td align=\"center\" height=\"15\" class=\"titolo_bianco\" bgcolor=\"" + temaSplit[2] + "\">Registro</td></tr>";
                }

                if (!Page.IsPostBack)
                {
                    //ricaricamento della pagina
                    Session.Remove("newClass");
                    l_createFascicolo = false;
                    impostazioniIniziali();
                    //string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_codClass.ID + "').focus() </SCRIPT>";
                    //RegisterStartupScript("focus", s);
                    Page.RegisterClientScriptBlock("CallDescReg", "<!--Desc Reg inzio--><DIV onmouseover=\"closeIt()\"><LAYER onmouseover=\"closeIt()\"></LAYER></DIV><DIV id=\"descreg\" style=\"visibility:hidden;LEFT: 200px; POSITION: absolute; TOP: 24px\"><div align=\"left\"><table cellSpacing=\"0\" border='1' bordercolordark='#ffffff' cellPadding=\"0\" bgColor=\"#d9d9d9\"  width='210px' height='60px'>" + rigaDescrizione + "<tr><td  bgColor=\"#d9d9d9\" class=\"testo_grigio_sp\">" + userRegistri[this.ddl_registri.SelectedIndex].descrizione + "</td></tr></table></div></DIV><!--Fine desc reg-->");
                }

                //Celeste
                if (OptLst.SelectedItem.Value.Equals("Cod"))
                {
                    //Disabilitare le liste.
                    EnabledisabledDDL(1, 6, false);
                    txt_codClass.Enabled = true;
                    txt_codClass.ReadOnly = false;
                    //ClearDDL(1,6);
                }
                else
                {
                    //Abilitare le liste.
                    EnabledisabledDDL(1, 6, true);
                    //txt_codClass.Text ="";
                    txt_codClass.Enabled = false;
                    txt_codClass.ReadOnly = true;
                }
                //Fine Celeste

                getParametri();
                gestioneParametri();
                generaFunctionChiamataScript();

                schedaRicerca = (DocsPAWA.ricercaDoc.SchedaRicerca)Session[DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY];
                if (schedaRicerca == null)
                {
                    //Inizializzazione della scheda di ricerca per la gestione delle ricerche salvate
                    schedaRicerca = new DocsPAWA.ricercaDoc.SchedaRicerca(KEY_SCHEDA_RICERCA, userHome, userRuolo, this);
                    Session[DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY] = schedaRicerca;
                }
                schedaRicerca.Pagina = this;
                if (!IsPostBack)
                {
                    ////verifica se nuova ADL
                    //if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1") && (!IsPostBack))
                    //{
                    //    schedaRicerca.ElencoRicercheADL("D", false, ddl_Ric_Salvate, null);
                    //    isSavedSearch = PopulateField(schedaRicerca.FiltriRicerca);
                    //}
                    //else
                    //{
                    //schedaRicerca.ElencoRicerche("F", ddl_Ric_Salvate);
                    //verifica se nuova ADL
                    if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1"))
                    {
                        schedaRicerca.ElencoRicercheADL("F", false, ddl_Ric_Salvate, null);
                    }
                    else
                    {
                        schedaRicerca.ElencoRicerche("F", ddl_Ric_Salvate);
                    }
                    this.BindFilterValues(schedaRicerca, false);
                    //isSavedSearch = PopulateField(schedaRicerca.FiltriRicerca);
                    //}
                }

                if (enableUfficioRef)
                {
                    this.pnl_uffRef.Visible = true;
                    // Inizializzazione condizionale link rubrica
                    string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
                    if (use_new_rubrica != "1")
                        this.btn_rubricaRef.Attributes["onclick"] = "ApriRubricaDaRicFasc('fascUffRef','U');";
                    else
                        this.btn_rubricaRef.Attributes["onclick"] = "_ApriRubrica('gf_uffref');";

                    if (FascicoliManager.DO_VerifyFlagUR())
                    {
                        FascicoliManager.DO_RemoveFlagUR();
                        //carico l'ufficio referente selezionato, se esiste
                        DocsPaWR.Corrispondente cor = FascicoliManager.getUoReferenteSelezionato(this);
                        if (cor != null)
                        {
                            this.txt_cod_UffRef.Text = cor.codiceRubrica;
                            this.txt_desc_uffRef.Text = UserManager.getDecrizioneCorrispondenteSemplice(cor);
                            this.hd_systemIdUffRef.Value = cor.systemId;
                        }

                        FascicoliManager.removeUoReferenteSelezionato(this);
                    }
                }


                if (FascicoliManager.DO_VerifyFlagLF())
                {
                    FascicoliManager.DO_RemoveFlagLF();

                    DocsPaVO.LocazioneFisica.LocazioneFisica LF = FascicoliManager.DO_GetLocazioneFisica();
                    if (LF != null)
                    {
                        this.txt_varCodRubrica_LF.Text = LF.CodiceRubrica;
                        this.txt_descr_LF.Text = LF.Descrizione;
                        this.hd_systemIdLF.Value = LF.UO_ID;
                    }
                    FascicoliManager.DO_RemoveLocazioneFisica();
                }

                //Profilazione dinamica fascicoli
                if (!IsPostBack)
                {
                    this.AddControlsClientAttribute();
                    //this.txtTipoCorrispondente.Value = this.optListTipiCreatore.SelectedItem.Value;

                    if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    {
                        pnl_profilazione.Visible = true;
                        CaricaComboTipologiaFasc();
                    }
                }

                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                {
                    verificaCampiPersonalizzati();
                }
                else
                {
                    img_dettagliProfilazione.Visible = false;
                }
                //Fine profilazione dinamica fascicoli

                //Verifico se provengo da una selezione a causa di :
                //1. risoluzione di un nodo su più di un titolario 
                //2. risoluzione di più nodi di titolario a partire da un protocollo titolario
                if (Session["idTitolarioSelezionato"] != null)
                {
                    if (Session["codiceNodoSelezionato"] != null)
                    {
                        txt_codClass.Text = Session["codiceNodoSelezionato"].ToString();
                        Session.Remove("codiceNodoSelezionato");
                    }

                    ddl_titolari.SelectedValue = Session["idTitolarioSelezionato"].ToString();
                    if (cercaClassificazioneDaCodice())
                    {
                        Session["newClass"] = "S";
                        FascicoliManager.removeClassificazioneSelezionata(this);
                    }
                    impostaAbilitazioneNuovoFascNuovoTit();
                    Session.Remove("idTitolarioSelezionato");
                }
                //FINE VERIFICA SELEZIONE
                
                /*
                //Verifico se provengo da una selezione a causa della risoluzione
                //di un fascicolo su più di un titolario
                if (Session["idTitolarioSelezionato"] != null)
                {
                    ddl_titolari.SelectedValue = Session["idTitolarioSelezionato"].ToString();
                    
                    
                    if (cercaClassificazioneDaCodice())
                    {
                        Session["newClass"] = "S";
                        FascicoliManager.removeClassificazioneSelezionata(this);
                    }
                    impostaAbilitazioneNuovoFascNuovoTit();
                    Session.Remove("idTitolarioSelezionato");
                }
                */

                //new ADl
                if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null && !Page.IsPostBack)
                {
                    btn_ricFascicoli_Click(null, null);
                }

                //Protocollo Titolario
                string contatoreTit = wws.isEnableContatoreTitolario();
                if (!string.IsNullOrEmpty(contatoreTit))
                {
                    lbl_protoTitolario.Visible = true;
                    lbl_protoTitolario.Text = "<B>" + contatoreTit + "</B>";
                    txt_protoPratica.Visible = true;
                    btn_new_tit.Text = contatoreTit;
                    btn_new_tit.ToolTip = "";
                }
                else
                {
                    txt_codClass.Width = Unit.Percentage(73);
                }

                if (Session["idRicSalvata"] != null && !string.IsNullOrEmpty(Session["idRicSalvata"].ToString()))
                {
                    ddl_Ric_Salvate.SelectedValue = Session["idRicSalvata"].ToString();
                    Session.Remove("idRicSalvata");
                    string altro = string.Empty;

                    if (!string.IsNullOrEmpty(this.numResult) && this.numResult.Equals("0"))
                    {
                        altro = "&noRic=1";
                    }

                    if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli" + altro + "';", true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?tabRes=fascicoli" + altro + "';", true);
                    }
                }

                if (this.ddl_Ric_Salvate.SelectedIndex == 0)
                {
                    this.btn_modifica.Enabled = false;
                }
                else
                {
                    this.btn_modifica.Enabled = true;
                }
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }

            this.txt_codClass.Focus();

            tastoInvio();

            if (!IsPostBack)
            {
                this.btn_clear_fields.Attributes.Add("onmouseout", "this.src='" + "../images/ricerca/remove_search_filter.gif'");
                this.btn_clear_fields.Attributes.Add("onmouseover", "this.src='" + "../images/ricerca/remove_search_filter_up.gif'");
            }
        }

        private void tastoInvio()
        {
            Utils.DefaultButton(this, ref txt_initDataA.txt_Data, ref btn_ricFascicoli);
            Utils.DefaultButton(this, ref txt_fineDataA.txt_Data, ref btn_ricFascicoli);
            Utils.DefaultButton(this, ref txt_initDataC.txt_Data, ref btn_ricFascicoli);
            Utils.DefaultButton(this, ref txt_fineDataC.txt_Data, ref btn_ricFascicoli);
            Utils.DefaultButton(this, ref txt_initDataCrea.txt_Data, ref btn_ricFascicoli);
            Utils.DefaultButton(this, ref txt_fineDataCrea.txt_Data, ref btn_ricFascicoli);
            Utils.DefaultButton(this, ref txt_dataScadenza_DA.txt_Data, ref btn_ricFascicoli);
            Utils.DefaultButton(this, ref txt_dataScadenza_A.txt_Data, ref btn_ricFascicoli);
            Utils.DefaultButton(this, ref txt_dta_LF_DA.txt_Data, ref btn_ricFascicoli);
            Utils.DefaultButton(this, ref txt_dta_LF_A.txt_Data, ref btn_ricFascicoli);
            TextBox note = rn_note.getTextBox();
            Utils.DefaultButton(this, ref note, ref btn_ricFascicoli);

        }

        private void btn_Canc_Ric_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (ddl_Ric_Salvate.SelectedIndex > 0)
            {
                //Chiedi conferma su popup
                string id = ddl_Ric_Salvate.SelectedValue;
                DocsPaWR.DocsPaWebService docspaws = ProxyManager.getWS();
                DocsPaWR.SearchItem item = docspaws.RecuperaRicerca(Int32.Parse(id));
                DocsPaWR.Ruolo ruolo = null;
                if (item.owner_idGruppo != 0)
                    ruolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
                string msg = "Il criterio di ricerca con nome '" + ddl_Ric_Salvate.SelectedItem.ToString() + "' verrà rimosso.\\n";
                msg += (ruolo != null) ? "Attenzione! Il criterio di ricerca è condiviso con il ruolo '" + ruolo.descrizione + "'.\\n" : "";
                msg += "Confermi l'operazione?";
                msg = msg.Replace("\"", "\\\"");
                if (this.Session["itemUsedSearch"] != null)
                {
                    Session.Remove("itemUsedSearch");
                }
                mb_ConfirmDelete.Confirm(msg);
            }
        }


        private void ddl_Ric_Salvate_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (ddl_Ric_Salvate.SelectedIndex == 0)
            {
                if (GridManager.IsRoleEnabledToUseGrids())
                {
                    GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Project);
                    GridManager.CompileDdlOrderAndSetOrderFilterProjects(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                }
                return;
            }
            try
            {
                string gridTempId = string.Empty;

                schedaRicerca.Seleziona(Int32.Parse(ddl_Ric_Salvate.SelectedValue), out gridTempId);

                if (!string.IsNullOrEmpty(gridTempId) && GridManager.IsRoleEnabledToUseGrids())
                {
                    schedaRicerca.gridId = gridTempId;
                    Grid tempGrid = GridManager.GetGridFromSearchId(schedaRicerca.gridId, GridTypeEnumeration.Project);
                    if (tempGrid != null)
                    {
                        GridManager.SelectedGrid = tempGrid;
                        GridManager.CompileDdlOrderAndSetOrderFilterProjects(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);

                        // Reperimento del filtro da utilizzare per la griglia
                        List<FiltroRicerca> filterList = GridManager.GetOrderFilterForProject(this.ddlOrder.SelectedValue, this.ddlOrderDirection.SelectedValue);

                        // Se la lista è valorizzata vengono aggiunti i filtri
                        if (filterList != null)
                        {
                            foreach (FiltroRicerca filter in filterList)
                            {
                                schedaRicerca.FiltriRicerca[0] = Utils.addToArrayFiltroRicerca(schedaRicerca.FiltriRicerca[0], filter);
                            }
                        }
                    }
                }

                qV = schedaRicerca.FiltriRicerca;

                try
                {
                    if (ddl_Ric_Salvate.SelectedIndex > 0)
                    {
                        Session.Add("itemUsedSearch", ddl_Ric_Salvate.SelectedIndex.ToString());
                    }

                    this.BindFilterValues(schedaRicerca, true);
                    FascicoliManager.setFiltroRicFasc(this, schedaRicerca.FiltriRicerca);
                    if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                    {
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?idClass=1&tabRes=fascicoli&ricADL=1';</script>");
                    }
                    else
                    {
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?idClass=1&tabRes=fascicoli';</script>");
                    }

                    this.btn_modifica.Enabled = true;
                }
                catch (Exception ex_)
                {
                    string msg = ex_.Message;
                    msg = msg + " Rimuovere i criteri di ricerca selezionati.";
                    msg = msg.Replace("\"", "\\\"");
                    Response.Write("<script>alert(\"" + msg + "\");window.location.href = window.location.href;</script>");
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                msg = msg.Replace("\"", "\\\"");
                Response.Write("<script>alert(\"" + msg + "\");window.location.href = window.location.href;</script>");
            }
        }

        private void caricaTitolariRegistro(bool getFigli)
        {
            string codClassifica;
            codClassifica = this.txt_codClass.Text;
            if (codClassifica.Equals(""))
                codClassifica = null;

            //Viene recuperato l'elenco delle voci del titolario associate al registro selezionato
            //DocsPaWR.FascicolazioneClassificazione[] FascClass = FascicoliManager.fascicolazioneGetTitolario(this, codClassifica, getFigli);
            //DocsPaWR.FascicolazioneClassificazione[] FascClass = FascicoliManager.fascicolazioneGetTitolario2(this, codClassifica, getFigli, ddl_titolari.SelectedValue);
            DocsPaWR.FascicolazioneClassificazione[] FascClass = FascicoliManager.fascicolazioneGetTitolario2(this, codClassifica, getFigli, getIdTitolario(codClassifica));
            //if (FascClass.Length == 0)
            //    flagCodice = "1";
            //else
            //{
            //    FascicoliManager.setClassificazioneSelezionata(this, FascClass[0]);
            //    FascicoliManager.setDescrizioneClassificazione(this, FascClass[0].descrizione);
            //}

            /* REM PER BUG COD_ULTIMO */
            //cosi' nuovo hash nella stessa locazione di mem.
            if (TheHash != null)
                TheHash.Clear();
            else
                TheHash = new Hashtable();

            indexH = 0;
            for (int k = 0; k < FascClass.Length; k++)
            {
                indexH = indexH + 1;
                TheHash.Add(indexH, FascClass[k]);
            }
            FascicoliManager.setTheHash(this, TheHash);


        }

        #endregion Eventi pagina

        #region Gestione callcontext

        /// <summary>
        /// Ripristino, nel contesto corrente,
        /// del numero pagina della griglia di ricerca 
        /// </summary>
        private void ResetPageNumberOnCurrentContext()
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_FASCICOLI)
                currentContext.PageNumber = 1;
            //nuova ADL
            else if (currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_FASCICOLI_ADL)
                currentContext.PageNumber = 1;
        }

        /// <summary>
        /// In caso di back, ripristino registro dal contesto corrente
        /// </summary>
        private void RestoreRegistroOnContext()
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_FASCICOLI || currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_FASCICOLI_ADL)
            {
                if (currentContext.ContextState.ContainsKey("idRegistro"))
                {
                    string idRegistro = currentContext.ContextState["idRegistro"].ToString();

                    DocsPaWR.Registro registro = UserManager.getRegistroBySistemId(this, idRegistro);

                    this.settaRegistroStartUp(registro);
                }
            }
        }

        /// <summary>
        /// Impostazione filtri correntemente selezionati
        /// nello stato del contesto di ricerca corrente
        /// </summary>
        /// <param name="filters"></param>
        private void SetFiltersOnCurrentContext(DocsPAWA.DocsPaWR.FiltroRicerca[][] filters)
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_FASCICOLI || currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_FASCICOLI_ADL)
            {
                // Impostazione filtri correnti
                currentContext.SessionState["ricFasc.listaFiltri"] = filters;

                // Impostazione scheda ricerca nel contesto corrente
                string schedaRicercaSessionKey = this.SchedaRicercaSessionKey;
                currentContext.SessionState[schedaRicercaSessionKey] = Session[schedaRicercaSessionKey];

                currentContext.ContextState["idRegistro"] = this.ddl_registri.SelectedItem.Value;

                // Impostazione, nel query string, del tipo ricerca classificazione (per codice o per livello)
                currentContext.QueryStringParameters["tipoClass"] = this.OptLst.SelectedItem.Value;
            }
        }

        /// <summary>
        /// Verifica se si è in un contesto di back
        /// </summary>
        private bool OnBack
        {
            get
            {
                return (this.Request.QueryString["back"] != null &&
                        this.Request.QueryString["back"].ToLower() == "true");
            }
        }

        #endregion

        #region Gestione salvataggio / ripristino filtri di ricerca

        /// <summary>
        /// Salvataggio criteri di ricerca
        /// </summary>
        /// <param name="filters"></param>
        private void SaveSearchFilters(DocsPaWR.FiltroRicerca[][] filters)
        {
            DocsPAWA.ricercaDoc.SchedaRicerca schedaRicerca = new DocsPAWA.ricercaDoc.SchedaRicerca(KEY_SCHEDA_RICERCA);
            schedaRicerca.FiltriRicerca = filters;
            Session[this.SchedaRicercaSessionKey] = schedaRicerca;
        }

        /// <summary>
        /// Ripristino filtri di ricerca, solamente se si è in contesto di back
        /// </summary>
        private void RestoreSearchFilters()
        {
            DocsPAWA.ricercaDoc.SchedaRicerca schedaRicerca = Session[this.SchedaRicercaSessionKey] as DocsPAWA.ricercaDoc.SchedaRicerca;

            if (schedaRicerca != null)
            {
                this.BindFilterValues(schedaRicerca, false);
            }
        }

        /// <summary>
        /// Ripristino valori filtri di ricerca nei campi della UI
        /// </summary>
        /// <param name="schedaRicerca"></param>
        private void BindFilterValues(DocsPAWA.ricercaDoc.SchedaRicerca schedaRicerca, bool grid)
        {
            if (schedaRicerca.FiltriRicerca != null && schedaRicerca.FiltriRicerca.Length > 0)
            {
                try
                {

                    if (this.Session["itemUsedSearch"] != null)
                        ddl_Ric_Salvate.SelectedIndex = Convert.ToInt32(this.Session["itemUsedSearch"]);


                    // Ripristino tipo ricerca classificazione
                    if (this.Request.QueryString["tipoClass"] != null)
                    {
                        this.OptLst.SelectedValue = this.Request.QueryString["tipoClass"];

                        /// Azione di selezione tipologia di ricerca
                        /// (per livello o codice)
                        this.PerformActionSelectSearchMode();
                    }

                    try
                    {
                        DocsPaWR.FiltroRicerca[] filterItems = schedaRicerca.FiltriRicerca[0];
                        foreach (DocsPaWR.FiltroRicerca item in filterItems)
                        {

                            //Ripristino filtro titolario
                            if (item.argomento == DocsPaWR.FiltriFascicolazione.ID_TITOLARIO.ToString())
                                this.RestoreFiltersIdTitolario(item);

                        }

                        foreach (DocsPaWR.FiltroRicerca item in filterItems)
                        {

                            // Ripristino filtri su classificazione
                            this.RestoreFiltersClassificazione(item);

                            // Filtri data apertura fascicolo
                            this.RestoreFiltersDataApertura(item);

                            // Filtri data chiusura fascicolo
                            this.RestoreFiltersDataChiusura(item);

                            // Filtri data creazione fascicolo
                            this.RestoreFiltersDataCreazione(item);

                            if (this.RestoreDropDownValue(this.ddlTipo, item, DocsPaWR.FiltriFascicolazione.TIPO_FASCICOLO))
                                // Azione di selezione tipologia fascicolo
                                this.PerformActionSelectTipoFascicolo();

                            this.RestoreDropDownValue(this.ddlStato, item, DocsPaWR.FiltriFascicolazione.STATO);
                            this.RestoreTextBoxValue(this.txtNumFasc, item, DocsPaWR.FiltriFascicolazione.NUMERO_FASCICOLO);
                            this.RestoreTextBoxValue(this.txtDescr, item, DocsPaWR.FiltriFascicolazione.TITOLO);
                            this.RestoreTextBoxValue(this.txtAnnoFasc, item, DocsPaWR.FiltriFascicolazione.ANNO_FASCICOLO);
                            //                            this.RestoreTextBoxValue(this.txtNote, item, DocsPaWR.FiltriFascicolazione.VAR_NOTE);

                            // Ripristino valori per il box ricerca note
                            // Se l'argomento del filtro è note...
                            if (item.argomento == DocsPaWR.FiltriFascicolazione.VAR_NOTE.ToString())
                            {
                                // ...splitta la stringa in argomento...
                                string[] info = Utils.splittaStringaRicercaNote(item.valore);

                                // ...la prima posizione dell'array contiene il testo da ricercare...
                                this.rn_note.Testo = info[0];

                                // ...la seconda contiene la tipologia di ricerca
                                this.rn_note.TipoRicerca = (info[1])[0];

                            }

                            //Filtri sottofascicolo
                            if (item.argomento == DocsPaWR.FiltriFascicolazione.SOTTOFASCICOLO.ToString())
                            {
                                txt_sottofascicolo.Text = item.valore;
                            }

                            //Filtri data scadenza
                            RestoreFiltersDataScadenza(item);

                            // Filtri data collocazione fascicolo
                            this.RestoreFiltersDataCollocazione(item);

                            // Ripristino filtri locazione fisica
                            this.RestoreFiltersLocazioneFisica(item);

                            // Ripristino filtri ufficio referente
                            this.RestoreFiltersUfficioReferente(item);

                            // Ripristino filtro "Mostra tutti i fascicoli"
                            if (item.argomento == DocsPaWR.FiltriFascicolazione.INCLUDI_FASCICOLI_FIGLI.ToString())
                                this.rbl_MostraTutti.SelectedValue = item.valore;

                            // Ripristino filtro "Tipologia fascicoli"
                            if (item.argomento == DocsPaWR.FiltriFascicolazione.TIPOLOGIA_FASCICOLO.ToString())
                            {
                                //CHRISTIAN - Ticket OC0000001490459 - Ricerca fascicoli: ripristino tipologia successivo a ordinamento tramite griglia.

                                CaricaComboTipologiaFasc();
                                //for (int i = 0; i < this.ddl_tipoFasc.Items.Count; i++)
                                //{
                                if (this.ddl_tipoFasc.Items.Count > 0)
                                {
                                    if (ddl_tipoFasc.Items.FindByValue(item.valore) != null)
                                    {
                                        this.ddl_tipoFasc.SelectedValue = item.valore;
                                    }
                                }
                                //    if (this.ddl_tipoFasc.Items[i].Value == item.valore.ToString())
                                //        this.ddl_tipoFasc.Items[i].Selected = true;
                                //}

                                //this.ddl_tipoFasc.SelectedValue = item.valore;
                                if (ddl_tipoFasc.SelectedIndex > 0)
                                    img_dettagliProfilazione.Visible = true;

                                CaricaComboStatiFasc();
                                // this.ddl_tipoFasc.SelectedValue = item.valore;
                            }

                            // Ripristino filtro "Diagramma Stato"
                            if (item.argomento == DocsPaWR.FiltriFascicolazione.DIAGRAMMA_STATO_FASC.ToString())
                            {
                                ddl_statiDoc.SelectedValue = item.valore;
                            }

                            if (item.argomento == DocsPaWR.FiltriFascicolazione.ID_PEOPLE_CREATORE.ToString()
                                || item.argomento == DocsPaWR.FiltriFascicolazione.ID_RUOLO_CREATORE.ToString()
                                || item.argomento == DocsPaWR.FiltriFascicolazione.ID_UO_CREATORE.ToString()
                                || item.argomento == DocsPaWR.FiltriFascicolazione.DESC_PEOPLE_CREATORE.ToString()
                                || item.argomento == DocsPaWR.FiltriFascicolazione.DESC_RUOLO_CREATORE.ToString()
                                || item.argomento == DocsPaWR.FiltriFascicolazione.DESC_UO_CREATORE.ToString())
                            {
                                //this.RestoreFiltersProprietario(item);
                                //this.FillDatiCorrispondenteDaRubrica();
                            }

                            if (item.argomento == DocsPaWR.FiltriFascicolazione.UO_SOTTOPOSTE.ToString())
                            {
                                //this.cbx_UOsotto.Visible = true;
                                //this.cbx_UOsotto.Checked = true;
                            }

                            // Ripristino filtro su Conservazione
                            if (item.argomento == DocsPaWR.FiltriFascicolazione.CONSERVAZIONE.ToString())
                            {
                                if (item.valore.Equals("1"))
                                {
                                    this.cb_Conservato.Checked = true;
                                    this.cb_NonConservato.Checked = false;
                                }
                                if (item.valore.Equals("0"))
                                {
                                    this.cb_Conservato.Checked = false;
                                    this.cb_NonConservato.Checked = true;
                                }
                            }

                            impostaAbilitazioneNuovoFascNuovoTit();

                            if (item.argomento == DocsPaWR.FiltriDocumento.ORDER_DIRECTION.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.ddlOrderDirection.SelectedValue = item.valore;
                                }

                            }


                            if (item.argomento == DocsPaWR.FiltriDocumento.ORACLE_FIELD_FOR_ORDER.ToString() && !grid)
                            {
                                if (!string.IsNullOrEmpty(item.nomeCampo))
                                {
                                    if (this.ddlOrder.SelectedValue.Contains(item.nomeCampo))
                                    {
                                        this.ddlOrder.SelectedValue = item.nomeCampo;
                                    }
                                }
                            }

                            #region ORDINAMENTO CONTATORE SOLO SE TIPOLOGIA HA CONTATORE E NON SI HANNO LE GRIGLIE CUSTUM
                            if (item.argomento == DocsPaWR.FiltriDocumento.CONTATORE_GRIGLIE_NO_CUSTOM.ToString() && !grid)
                            {
                                if (!GridManager.IsRoleEnabledToUseGrids())
                                {
                                    /* ListItem cont = new ListItem("Contatore", "-2");
                                     ddlOrder.Items.Add(cont);
                                     ddlOrder.SelectedValue = "-2";
                                     this.ddlOrder.SelectedValue = "-2";*/
                                    this.no_custom_grid_cont = true;
                                }
                            }
                            #endregion

                            #region Visibilità Tipica / Atipica
                            else if (item.argomento == DocsPaWR.FiltriDocumento.VISIBILITA_T_A.ToString())
                            {
                                rbl_visibilita.SelectedValue = item.valore;
                            }
                            #endregion Visibilità Tipica / Atipica
                        }

                    }
                    catch (Exception)
                    {
                        throw new Exception("I criteri di ricerca non sono piu\' validi.");
                    }
                }
                catch (System.Exception ex)
                {
                    ErrorManager.redirect(this, ex);
                }
            }
        }

        /// <summary>
        /// Ripristino filtri su classificazione
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersClassificazione(DocsPaWR.FiltroRicerca item)
        {
            if (item.argomento == DocsPaWR.FiltriFascicolazione.CODICE_CLASSIFICA.ToString())
            {
                DocsPaWR.FascicolazioneClassificazione classificazione = new DocsPaWR.FascicolazioneClassificazione();
                classificazione.codice = item.argomento;
                FascicoliManager.setMemoriaClassificaRicFasc(this, classificazione);
                // Ricerca codice classificazione
                return this.cercaClassificazioneDaCodice(item.valore);
            }

            return false;
        }

        /// <summary>
        /// Ripristino filtri locazione fisica
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersLocazioneFisica(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriFascicolazione.ID_UO_LF.ToString())
            {
                // Reperimento dati corrispondente da systemID
                DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteBySystemID(this, item.valore);
                if (corr != null)
                {
                    this.hd_systemIdLF.Value = item.valore;
                    this.txt_varCodRubrica_LF.Text = corr.codiceRubrica;
                    this.txt_descr_LF.Text = corr.descrizione;
                    retValue = true;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Ripristino filtri ufficio referente
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersUfficioReferente(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriFascicolazione.ID_UO_REF.ToString())
            {
                // Reperimento dati corrispondente da systemID
                DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteBySystemID(this, item.valore);
                if (corr != null)
                {
                    this.hd_systemIdUffRef.Value = item.valore;
                    this.txt_cod_UffRef.Text = corr.codiceRubrica;
                    this.txt_desc_uffRef.Text = corr.descrizione;
                    retValue = true;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Ripristino filtri data apertura
        /// </summary>
        /// <param name="filterItems"></param>
        /// <returns></returns>
        private bool RestoreFiltersDataApertura(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriFascicolazione.APERTURA_SUCCESSIVA_AL.ToString())
            {
                this.ddl_dataA.SelectedValue = "1";
                this.PerformActionSelectDropDownDataApertura();

                this.RestoreTextBoxValue(this.GetCalendarControl("txt_initDataA").txt_Data, item, DocsPaWR.FiltriFascicolazione.APERTURA_SUCCESSIVA_AL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.APERTURA_PRECEDENTE_IL.ToString())
            {
                this.RestoreTextBoxValue(this.GetCalendarControl("txt_fineDataA").txt_Data, item, DocsPaWR.FiltriFascicolazione.APERTURA_PRECEDENTE_IL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.APERTURA_IL.ToString())
            {
                this.ddl_dataA.SelectedValue = "0";
                this.PerformActionSelectDropDownDataApertura();

                this.RestoreTextBoxValue(this.GetCalendarControl("txt_initDataA").txt_Data, item, DocsPaWR.FiltriFascicolazione.APERTURA_IL);
                retValue = true;
            }
            #region APERTURA_SC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.APERTURA_SC.ToString() && item.valore == "1")
            {
                this.ddl_dataA.SelectedIndex = 3;
                this.GetCalendarControl("txt_initDataA").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                this.GetCalendarControl("txt_initDataA").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_initDataA").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_fineDataA").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = true;
                this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataA").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_fineDataA").btn_Cal.Enabled = false;
                this.lbl_initdataA.Visible = true;
                this.lbl_finedataA.Visible = true;
                retValue = true;
            }
            #endregion
            #region APERTURA_MC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.APERTURA_MC.ToString() && item.valore == "1")
            {
                this.ddl_dataA.SelectedIndex = 4;
                this.GetCalendarControl("txt_initDataA").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                this.GetCalendarControl("txt_initDataA").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_initDataA").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_fineDataA").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = true;
                this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataA").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_fineDataA").btn_Cal.Enabled = false;
                this.lbl_initdataA.Visible = true;
                this.lbl_finedataA.Visible = true;
                retValue = true;
            }
            #endregion
            #region APERTURA_TODAY
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.APERTURA_TODAY.ToString() && item.valore == "1")
            {
                this.ddl_dataA.SelectedIndex = 2;
                this.GetCalendarControl("txt_initDataA").Visible = true;
                this.GetCalendarControl("txt_initDataA").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                this.GetCalendarControl("txt_initDataA").txt_Data.Visible = true;
                this.GetCalendarControl("txt_initDataA").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_initDataA").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_initDataA").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_fineDataA").Visible = false;
                this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = false;
                this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = false;
                this.lbl_initdataA.Visible = false;
                this.lbl_finedataA.Visible = false;
            }
            #endregion
            return retValue;
        }

        /// <summary>
        /// Ripristino filtri data chiusura
        /// </summary>
        /// <param name="filterItems"></param>
        /// <returns></returns>
        private bool RestoreFiltersDataChiusura(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriFascicolazione.CHIUSURA_SUCCESSIVA_AL.ToString())
            {
                this.ddl_dataC.SelectedValue = "1";
                this.PerformActionSelectDropDownDataChiusura();

                this.RestoreTextBoxValue(this.GetCalendarControl("txt_initDataC").txt_Data, item, DocsPaWR.FiltriFascicolazione.CHIUSURA_SUCCESSIVA_AL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CHIUSURA_PRECEDENTE_IL.ToString())
            {
                this.RestoreTextBoxValue(this.GetCalendarControl("txt_fineDataC").txt_Data, item, DocsPaWR.FiltriFascicolazione.CHIUSURA_PRECEDENTE_IL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CHIUSURA_IL.ToString())
            {
                this.ddl_dataC.SelectedValue = "0";
                this.PerformActionSelectDropDownDataChiusura();

                this.RestoreTextBoxValue(this.GetCalendarControl("txt_initDataC").txt_Data, item, DocsPaWR.FiltriFascicolazione.APERTURA_IL);
                retValue = true;
            }
            #region CHIUSURA_SC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CHIUSURA_SC.ToString() && item.valore == "1")
            {
                this.ddl_dataC.SelectedIndex = 3;
                this.GetCalendarControl("txt_initDataC").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                this.GetCalendarControl("txt_initDataC").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_initDataC").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_fineDataC").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = true;
                this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataC").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_fineDataC").btn_Cal.Enabled = false;
                this.lbl_initdataC.Visible = true;
                this.lbl_finedataC.Visible = true;
                retValue = true;
            }
            #endregion
            #region CHIUSURA_MC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CHIUSURA_MC.ToString() && item.valore == "1")
            {
                this.ddl_dataC.SelectedIndex = 4;
                this.GetCalendarControl("txt_initDataC").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                this.GetCalendarControl("txt_initDataC").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_initDataC").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_fineDataC").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = true;
                this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataC").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_fineDataC").btn_Cal.Enabled = false;
                this.lbl_initdataC.Visible = true;
                this.lbl_finedataC.Visible = true;
                retValue = true;
            }
            #endregion
            #region CHIUSURA_TODAY
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CHIUSURA_TODAY.ToString() && item.valore == "1")
            {
                this.ddl_dataC.SelectedIndex = 2;
                this.GetCalendarControl("txt_initDataC").Visible = true;
                this.GetCalendarControl("txt_initDataC").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                this.GetCalendarControl("txt_initDataC").txt_Data.Visible = true;
                this.GetCalendarControl("txt_initDataC").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_initDataC").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_initDataC").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_fineDataC").Visible = false;
                this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = false;
                this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = false;
                this.lbl_initdataC.Visible = false;
                this.lbl_finedataC.Visible = false;
            }
            #endregion
            return retValue;
        }

        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
        }

        /// <summary>
        /// Ripristino filtri data creazione
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersDataCreazione(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriFascicolazione.CREAZIONE_SUCCESSIVA_AL.ToString())
            {
                this.ddl_creaz.SelectedValue = "1";
                this.PerformActionSelectDropDownDataCreazione();

                this.RestoreTextBoxValue(this.GetCalendarControl("txt_initDataCrea").txt_Data, item, DocsPaWR.FiltriFascicolazione.CREAZIONE_SUCCESSIVA_AL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CREAZIONE_PRECEDENTE_IL.ToString())
            {
                this.RestoreTextBoxValue(this.GetCalendarControl("txt_fineDataCrea").txt_Data, item, DocsPaWR.FiltriFascicolazione.CREAZIONE_PRECEDENTE_IL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CREAZIONE_IL.ToString())
            {
                this.ddl_creaz.SelectedValue = "0";
                this.PerformActionSelectDropDownDataCreazione();

                this.RestoreTextBoxValue(this.GetCalendarControl("txt_initDataCrea").txt_Data, item, DocsPaWR.FiltriFascicolazione.CREAZIONE_IL);
                retValue = true;
            }
            #region CREAZIONE_SC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CREAZIONE_SC.ToString() && item.valore == "1")
            {
                this.ddl_creaz.SelectedIndex = 3;
                this.GetCalendarControl("txt_initDataCrea").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                this.GetCalendarControl("txt_initDataCrea").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_initDataCrea").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = true;
                this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Enabled = false;
                this.lbl_initCreaz.Visible = true;
                this.lbl_finCreaz.Visible = true;
                retValue = true;
            }
            #endregion
            #region CREAZIONE_MC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CREAZIONE_MC.ToString() && item.valore == "1")
            {
                this.ddl_creaz.SelectedIndex = 4;
                this.GetCalendarControl("txt_initDataCrea").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                this.GetCalendarControl("txt_initDataCrea").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_initDataCrea").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = true;
                this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Enabled = false;
                this.lbl_initCreaz.Visible = true;
                this.lbl_finCreaz.Visible = true;
                retValue = true;
            }
            #endregion
            #region CREAZIONE_TODAY
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CREAZIONE_TODAY.ToString() && item.valore == "1")
            {
                this.ddl_creaz.SelectedIndex = 2;
                this.GetCalendarControl("txt_initDataCrea").Visible = true;
                this.GetCalendarControl("txt_initDataCrea").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                this.GetCalendarControl("txt_initDataCrea").txt_Data.Visible = true;
                this.GetCalendarControl("txt_initDataCrea").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_initDataCrea").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_initDataCrea").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_fineDataCrea").Visible = false;
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = false;
                this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = false;
                this.lbl_initCreaz.Visible = false;
                this.lbl_finCreaz.Visible = false;
            }
            #endregion
            return retValue;
        }

        /// <summary>
        /// Ripristino filtri data scadenza
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersDataScadenza(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriFascicolazione.SCADENZA_SUCCESSIVA_AL.ToString())
            {
                this.ddl_dataScadenza.SelectedValue = "1";
                this.PerformActionSelectDropDownDataScadenza();

                this.RestoreTextBoxValue(this.GetCalendarControl("txt_dataScadenza_DA").txt_Data, item, DocsPaWR.FiltriFascicolazione.SCADENZA_SUCCESSIVA_AL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.SCADENZA_PRECEDENTE_IL.ToString())
            {
                this.RestoreTextBoxValue(this.GetCalendarControl("txt_dataScadenza_A").txt_Data, item, DocsPaWR.FiltriFascicolazione.SCADENZA_PRECEDENTE_IL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.SCADENZA_IL.ToString())
            {
                this.ddl_dataScadenza.SelectedValue = "0";
                this.PerformActionSelectDropDownDataScadenza();

                this.RestoreTextBoxValue(this.GetCalendarControl("txt_dataScadenza_DA").txt_Data, item, DocsPaWR.FiltriFascicolazione.SCADENZA_IL);
                retValue = true;
            }
            #region SCADENZA_SC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.SCADENZA_SC.ToString() && item.valore == "1")
            {
                this.ddl_dataScadenza.SelectedIndex = 3;
                this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_dataScadenza_DA").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Visible = true;
                this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Enabled = false;
                this.lbl_dataScadenza_DA.Visible = true;
                this.lbl_dataScadenza_A.Visible = true;
                retValue = true;
            }
            #endregion
            #region SCADENZA_MC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.SCADENZA_MC.ToString() && item.valore == "1")
            {
                this.ddl_dataScadenza.SelectedIndex = 4;
                this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_dataScadenza_DA").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Visible = true;
                this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Enabled = false;
                this.lbl_dataScadenza_DA.Visible = true;
                this.lbl_dataScadenza_A.Visible = true;
                retValue = true;
            }
            #endregion
            #region SCADENZA_TODAY
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.SCADENZA_TODAY.ToString() && item.valore == "1")
            {
                this.ddl_dataScadenza.SelectedIndex = 2;
                this.GetCalendarControl("txt_dataScadenza_DA").Visible = true;
                this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Visible = true;
                this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_dataScadenza_DA").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_dataScadenza_DA").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_dataScadenza_A").Visible = false;
                this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Visible = false;
                this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Visible = false;
                this.lbl_dataScadenza_DA.Visible = false;
                this.lbl_dataScadenza_A.Visible = false;
            }
            #endregion
            return retValue;
        }

        /// <summary>
        /// Ripristino filtri data collocazione
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersDataCollocazione(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriFascicolazione.DATA_LF_SUCCESSIVA_AL.ToString())
            {
                this.ddl_data_LF.SelectedValue = "1";
                this.PerformActionSelectDataLF();

                this.RestoreTextBoxValue(this.GetCalendarControl("txt_dta_LF_DA").txt_Data, item, DocsPaWR.FiltriFascicolazione.DATA_LF_SUCCESSIVA_AL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.DATA_LF_PRECEDENTE_IL.ToString())
            {

                this.RestoreTextBoxValue(this.GetCalendarControl("txt_dta_LF_A").txt_Data, item, DocsPaWR.FiltriFascicolazione.DATA_LF_PRECEDENTE_IL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.DATA_LF_SUCCESSIVA_AL.ToString())
            {
                this.ddl_data_LF.SelectedValue = "0";
                this.PerformActionSelectDataLF();

                this.RestoreTextBoxValue(this.GetCalendarControl("txt_dta_LF_DA").txt_Data, item, DocsPaWR.FiltriFascicolazione.DATA_LF_SUCCESSIVA_AL);
                retValue = true;
            }
            #region DATA_LF_SC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.DATA_LF_SC.ToString() && item.valore == "1")
            {
                this.ddl_data_LF.SelectedIndex = 3;
                this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = true;
                this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_dta_LF_A").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Enabled = false;
                this.lbl_dta_LF_DA.Visible = true;
                this.lbl_dta_LF_A.Visible = true;
                retValue = true;
            }
            #endregion
            #region DATA_LF_MC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.DATA_LF_MC.ToString() && item.valore == "1")
            {
                this.ddl_data_LF.SelectedIndex = 4;
                this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = true;
                this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_dta_LF_A").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Enabled = false;
                this.lbl_dta_LF_DA.Visible = true;
                this.lbl_dta_LF_A.Visible = true;
                retValue = true;
            }
            #endregion
            #region DATA_LF_TODAY
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.DATA_LF_TODAY.ToString() && item.valore == "1")
            {
                this.ddl_data_LF.SelectedIndex = 2;
                this.GetCalendarControl("txt_dta_LF_DA").Visible = true;
                this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Visible = true;
                this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_dta_LF_A").Visible = false;
                this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = false;
                this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = false;
                this.lbl_dta_LF_DA.Visible = false;
                this.lbl_dta_LF_A.Visible = false;
            }
            #endregion
            return retValue;
        }

        /// <summary>
        /// Ripristino filtro Proprietario
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>        
        private bool RestoreFiltersProprietario(DocsPaWR.FiltroRicerca item)
        {
            //if (item.argomento == DocsPaWR.FiltriFascicolazione.ID_PEOPLE_CREATORE.ToString())
            //{
            //    this.optListTipiCreatore.SelectedItem.Value = "P";

            //    return true;
            //}

            //if (item.argomento == DocsPaWR.FiltriFascicolazione.ID_RUOLO_CREATORE.ToString())
            //{
            //    this.optListTipiCreatore.SelectedItem.Value = "R";

            //    return true;
            //}

            //if (item.argomento == DocsPaWR.FiltriFascicolazione.ID_UO_CREATORE.ToString())
            //{
            //    this.optListTipiCreatore.SelectedItem.Value = "U";

            //    return true;
            //}

            //if (item.argomento == DocsPaWR.FiltriFascicolazione.DESC_PEOPLE_CREATORE.ToString())
            //{
            //    this.optListTipiCreatore.SelectedItem.Value = "P";

            //    return true;
            //}

            //if (item.argomento == DocsPaWR.FiltriFascicolazione.DESC_RUOLO_CREATORE.ToString())
            //{
            //    this.optListTipiCreatore.SelectedItem.Value = "R";

            //    return true;
            //}

            //if (item.argomento == DocsPaWR.FiltriFascicolazione.DESC_UO_CREATORE.ToString())
            //{
            //    this.optListTipiCreatore.SelectedItem.Value = "U";

            //    return true;
            //}

            //if (item.argomento == DocsPaWR.FiltriFascicolazione.UO_SOTTOPOSTE.ToString())
            //{
                //this.cbx_UOsotto.Visible = true;
                //this.cbx_UOsotto.Checked = true;
            //    return true;
            //}
            return false;
        }

        /// <summary>
        /// Ripristino filtro idTitolario
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>        
        private bool RestoreFiltersIdTitolario(DocsPaWR.FiltroRicerca item)
        {
            if (item.argomento == DocsPaWR.FiltriFascicolazione.ID_TITOLARIO.ToString())
            {
                this.ddl_titolari.SelectedValue = item.valore;
                if (txt_codClass.Text == null || txt_codClass.Text == "")
                    caricamentoClassificazioniPadri();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Ripristina il filtro idTitolario a partire dalla scheda ricerca
        /// Serve per la gestione delle ricerche tornando alla pagina col tasto back
        /// </summary>
        private void RestoreFiltersIdTitolario()
        {
            DocsPAWA.ricercaDoc.SchedaRicerca schedaRicerca = Session[this.SchedaRicercaSessionKey] as DocsPAWA.ricercaDoc.SchedaRicerca;

            if (schedaRicerca != null && schedaRicerca.FiltriRicerca != null && schedaRicerca.FiltriRicerca.Length > 0)
            {
                try
                {
                    DocsPaWR.FiltroRicerca[] filterItems = schedaRicerca.FiltriRicerca[0];

                    foreach (DocsPaWR.FiltroRicerca item in filterItems)
                    {
                        //Ripristino filtro titolario
                        if (item.argomento == DocsPaWR.FiltriFascicolazione.ID_TITOLARIO.ToString())
                            this.RestoreFiltersIdTitolario(item);
                    }
                }
                catch (System.Exception ex)
                {
                    ErrorManager.redirect(this, ex);
                }
            }
        }

        /// <summary>
        /// Rimuove il filtro idTitolario a partire dalla scheda ricerca
        /// Serve per la gestione delle ricerche tornando alla pagina col tasto back
        /// </summary>
        private void RemoveIdTitolario()
        {
            DocsPAWA.ricercaDoc.SchedaRicerca schedaRicerca = Session[this.SchedaRicercaSessionKey] as DocsPAWA.ricercaDoc.SchedaRicerca;

            if (schedaRicerca != null && schedaRicerca.FiltriRicerca != null && schedaRicerca.FiltriRicerca.Length > 0)
            {
                try
                {
                    DocsPaWR.FiltroRicerca[] filterItems = schedaRicerca.FiltriRicerca[0];

                    foreach (DocsPaWR.FiltroRicerca item in filterItems)
                    {
                        //Ripristino filtro titolario
                        if (item.argomento == DocsPaWR.FiltriFascicolazione.ID_TITOLARIO.ToString())
                            item.argomento = null;
                    }

                    schedaRicerca.FiltriRicerca[0] = filterItems;
                    Session[this.SchedaRicercaSessionKey] = schedaRicerca;
                }
                catch (System.Exception ex)
                {
                    ErrorManager.redirect(this, ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="filterItem"></param>
        /// <param name="filterType"></param>
        private bool RestoreTextBoxValue(TextBox textBox, DocsPaWR.FiltroRicerca filterItem, DocsPaWR.FiltriFascicolazione filterType)
        {
            bool retValue = false;

            if (filterItem.argomento == filterType.ToString())
            {
                textBox.Text = filterItem.valore;
                retValue = true;
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dropDown"></param>
        /// <param name="filterItem"></param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        private bool RestoreDropDownValue(DropDownList dropDown, DocsPaWR.FiltroRicerca filterItem, DocsPaWR.FiltriFascicolazione filterType)
        {
            bool retValue = false;

            if (filterItem.argomento == filterType.ToString() &&
                dropDown.Items.Count > 0)
            {
                dropDown.SelectedValue = filterItem.valore;
                retValue = true;
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        private string SchedaRicercaSessionKey
        {
            get
            {
                return string.Concat("RicercaFascicoli_", DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY);
            }
        }

        #endregion

        #region Profilazione dinamica
        private void CaricaComboTipologiaFasc()
        {
            ArrayList listaTipiFasc = new ArrayList(ProfilazioneFascManager.getTipoFascFromRuolo(UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "1",this));
            ListItem item = new ListItem();
            item.Value = "";
            item.Text = "";
            if (ddl_tipoFasc.Items.Count==0)
            {
                ddl_tipoFasc.Items.Add(item);
            }
            for (int i = 0; i < listaTipiFasc.Count; i++)
            {
                DocsPaWR.Templates templates = (DocsPaWR.Templates)listaTipiFasc[i];
                ListItem item_1 = new ListItem();
                item_1.Value = templates.SYSTEM_ID.ToString();
                item_1.Text = templates.DESCRIZIONE;
                
                //Christian - Ticket OC0000001490459 - Ricerca fascicoli: ripristino tipologia successivo a ordinamento tramite griglia.
                if (ddl_tipoFasc.Items.FindByValue(templates.SYSTEM_ID.ToString())==null)
                {
                    if (templates.IPER_FASC_DOC == "1")
                        ddl_tipoFasc.Items.Insert(1, item_1);
                    else
                        ddl_tipoFasc.Items.Add(item_1);
                }
               
            }
        }

        private void CaricaComboStatiFasc()
        {
            //DIAGRAMMI DI STATO
            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
            {
                if (ddl_tipoFasc.SelectedValue != null && ddl_tipoFasc.SelectedValue != "")
                {
                    //Verifico se esiste un diagramma di stato associato al tipo di documento
                    //Modifica per la visualizzazione solo degli stati per cui esistono documenti in essi
                    string idDiagramma = DocsPAWA.DiagrammiManager.getDiagrammaAssociatoFasc(ddl_tipoFasc.SelectedValue, this).ToString();
                    if (!string.IsNullOrEmpty(idDiagramma))
                    {
                        Panel_StatiDocumento.Visible = true;
                        //Inizializzazione comboBox
                        ddl_statiDoc.Items.Clear();
                        ListItem itemEmpty = new ListItem();
                        ddl_statiDoc.Items.Add(itemEmpty);

                        DocsPaWR.Stato[] statiDg = DocsPAWA.DiagrammiManager.getStatiPerRicerca(idDiagramma, "F", this);
                        foreach (Stato st in statiDg)
                        {
                            ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                            ddl_statiDoc.Items.Add(item);
                        }
                    }
                    else
                    {
                        Panel_StatiDocumento.Visible = false;
                    }

                    /*
                    //Verifico se esiste un diagramma di stato associato al tipo di fascicolo
                    DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDgByIdTipoFasc(ddl_tipoFasc.SelectedValue, (UserManager.getInfoUtente(this)).idAmministrazione, this);
                    Session.Add("DiagrammaSelezionato", dg);
                    if (dg != null)
                    {
                        Panel_StatiDocumento.Visible = true;

                        //Inizializzazione comboBox
                        ddl_statiDoc.Items.Clear();
                        ListItem itemEmpty = new ListItem();
                        ddl_statiDoc.Items.Add(itemEmpty);
                        for (int i = 0; i < dg.STATI.Length; i++)
                        {
                            DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato)dg.STATI[i];
                            ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                            ddl_statiDoc.Items.Add(item);
                        }
                    }
                    else
                    {
                        Panel_StatiDocumento.Visible = false;
                    }
                    */
                }
            }
            //FINE DIAGRAMMI STATO
        }

        private void img_dettagliProfilazione_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "profilazioneDinamica", "apriPopupAnteprima();", true);
        }

        private void ddl_tipoFasc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Session.Remove("templateRicerca");
            Session.Remove("filtroProfDinamica");
            Session.Remove("dictionaryCorrispondente");
            schedaRicerca.RimuoviFiltro(DocsPAWA.DocsPaWR.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString());
            verificaCampiPersonalizzati();
            CaricaComboStatiFasc();
            //Se non ho le griglie custom
            if (!GridManager.IsRoleEnabledToUseGrids())
            {
                if (ddl_tipoFasc.SelectedItem != null && !string.IsNullOrEmpty(ddl_tipoFasc.SelectedItem.Text))
                {
                    DocsPAWA.DocsPaWR.Templates template = ProfilazioneFascManager.getTemplateFascById(ddl_tipoFasc.SelectedItem.Value, this.Page);
                    if (template != null)
                    {
                        OggettoCustom customObjectTemp = new OggettoCustom();
                        customObjectTemp = template.ELENCO_OGGETTI.Where(
                        r => r.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && r.DA_VISUALIZZARE_RICERCA == "1").
                        FirstOrDefault();

                        if (customObjectTemp != null)
                        {
                            ListItem cont = new ListItem("Contatore", "-2");
                            ddlOrder.Items.Add(cont);
                            ddlOrder.SelectedValue = "-2";
                        }
                        else
                        {
                            GridManager.CompileDdlOrderAndSetOrderFilterProjects(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                        }
                    }

                }
            }
        }

        private void verificaCampiPersonalizzati()
        {
            DocsPaWR.Templates template = new DocsPAWA.DocsPaWR.Templates();
            if (!ddl_tipoFasc.SelectedValue.Equals(""))
            {
                template = (DocsPAWA.DocsPaWR.Templates)Session["templateRicerca"];
                if (template == null)
                {
                    template = ProfilazioneFascManager.getTemplateFascById(ddl_tipoFasc.SelectedValue, this);
                    Session.Add("templateRicerca", template);
                }
                if (template != null && !(ddl_tipoFasc.SelectedItem.Text.ToUpper()).Equals(template.DESCRIZIONE.ToUpper()))
                {
                    template = ProfilazioneFascManager.getTemplateFascById(ddl_tipoFasc.SelectedValue, this);
                    Session.Add("templateRicerca", template);
                }
            }
            if (template != null && template.SYSTEM_ID == 0)
            {
                img_dettagliProfilazione.Visible = false;
            }
            else
            {
                if (template != null && template.ELENCO_OGGETTI.Length != 0)
                {
                    img_dettagliProfilazione.Visible = true;
                }
                else
                {
                    img_dettagliProfilazione.Visible = false;
                }
            }
        }

        #endregion

        #region Metodi //Celeste

        private void EnabledisabledDDL(int indexMin, int indexMax, bool enabled)
        {
            DropDownList ddl;
            DropDownList ddlprev;
            string DDLName = "ddl_livello";

            for (int index = indexMin; index <= indexMax; index++)
            {
                ddl = (DropDownList)this.FindControl(DDLName + index.ToString());

                if (enabled)
                {
                    if (ddl.ID.Substring(DDLName.Length).ToString().Equals(indexMin.ToString()))
                        ddl.Enabled = true;
                    else
                    {
                        ddlprev = (DropDownList)this.FindControl(DDLName + Convert.ToString(index - 1));
                        if (ddlprev.SelectedValue.Length > 0)
                            ddl.Enabled = true;
                        else
                            ddl.Enabled = false;
                    }

                }
                else
                    ddl.Enabled = false;
            }

            return;
        }

        private void ClearDDL(int indexMin, int indexMax)
        {
            DropDownList ddl;
            for (int index = indexMin; index <= indexMax; index++)
            {
                ddl = (DropDownList)this.FindControl("ddl_livello" + index.ToString());
                ddl.SelectedIndex = -1;
            }
            return;
        }

        #endregion

        #region SetStatoReg

        private void setStatoReg(DocsPAWA.DocsPaWR.Registro reg)
        {
            // inserisco il registro selezionato in sessione			
            UserManager.setRegistroSelezionato(this, reg);
            string nomeImg;

            if (UserManager.getStatoRegistro(reg).Equals("G"))
                nomeImg = "stato_giallo2.gif";
            else if (UserManager.getStatoRegistro(reg).Equals("V"))
                nomeImg = "stato_verde2.gif";
            else
                nomeImg = "stato_rosso2.gif";

            this.img_statoReg.ImageUrl = "../images/" + nomeImg;
        }

        #endregion SetStatoReg

        #region Pulsanti Nuovo fascicolo - Nuovo nodo di titolario
        //private void btn_new_tit_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        private void btn_new_tit_Click(object sender, System.EventArgs e)
        {
            string profilazione = System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"];
            if (!this.txt_codClass.Text.Trim().Equals(""))
            {
                //ClientScript.RegisterStartupScript(this.GetType(), "apreGestNew", "ApriFinestraNewFascNewTit('" + Server.UrlEncode(this.txt_codClass.Text) + "','ricercaFascicoli','insertNewNodoTitolario.aspx','" + profilazione + "','" + ddl_titolari.SelectedValue + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "apreGestNew", "ApriFinestraNewFascNewTit('" + Server.UrlEncode(this.txt_codClass.Text) + "','ricercaFascicoli','insertNewNodoTitolario.aspx','" + profilazione + "','" + getIdTitolario(this.txt_codClass.Text) + "');", true);
            }
        }

        private void btn_new_Click(object sender, System.EventArgs e)
        {
            DocsPaWR.Registro reg = UserManager.getRegistroBySistemId(this,ddl_registri.SelectedItem.Value);
            if (reg.Sospeso)
            {
                RegisterClientScriptBlock("alertRegistroSospeso", "<script language=javascript>alert('Il registro selezionato è sospeso!');</script>");
                return;
            }

            string profilazione = System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"];
            if (!this.txt_codClass.Text.Trim().Equals(""))
            {
                this.ricercaFascicoli();
                if (Session["rubrica.campoCorrispondente"] != null)
                    Session.Remove("rubrica.campoCorrispondente");
                if (UserManager.getCorrispondenteSelezionato(this) != null)
                    UserManager.setCorrispondenteSelezionato(this, null);
                if (Session["CorrSelezionatoDaMulti"] != null)
                    Session.Remove("CorrSelezionatoDaMulti");
                if (Session["dictionaryCorrispondente"] != null)
                    Session.Remove("dictionaryCorrispondente");

                //ClientScript.RegisterStartupScript(this.GetType(), "apreGestNew", "ApriFinestraNewFascNewTit('" + Server.UrlEncode(this.txt_codClass.Text) + "','ricercaFascicoli','fascNewFascicolo.aspx','" + profilazione + "','"+ddl_titolari.SelectedValue+"');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "apreGestNew", "ApriFinestraNewFascNewTit('" + Server.UrlEncode(this.txt_codClass.Text) + "','ricercaFascicoli','fascNewFascicolo.aspx','" + profilazione + "','" + getIdTitolario(this.txt_codClass.Text) + "');", true);
            }
        }

        private void impostaVisibiltaBtnFascTit()
        {
            if (UserManager.ruoloIsAutorized(this, "FASC_NUOVO"))
            {
                btn_new.Visible = true;
            }
            else
            {
                btn_new.Visible = false;
            }

            if (UserManager.ruoloIsAutorized(this, "DO_TITOLARIO"))
            {
                btn_new_tit.Visible = true;
            }
            else
            {
                btn_new_tit.Visible = false;
            }
        }
        #endregion Fine Pulsanti Nuovo fascicolo - Nuovo nodo di titolario

        #region Impostazioni iniziali
        private void impostazioniIniziali()
        {
            //FascicoliManager.removeAllClassValue(this);
            FascicoliManager.removeClassificazioneSelezionata(this);

            this.ddl_creaz.SelectedIndex = 0;
            this.lbl_finCreaz.Visible = false;
            this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = false;
            this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = false;
            this.ddl_data_LF.SelectedIndex = 0;
            this.lbl_dta_LF_A.Visible = false;
            this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = false;
            this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = false;
            this.ddl_dataA.SelectedIndex = 0;
            this.lbl_finedataA.Visible = false;
            this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = false;
            this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = false;
            this.ddl_dataC.SelectedIndex = 0;
            this.lbl_finedataC.Visible = false;
            this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = false;
            this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = false;
            this.ddl_dataScadenza.SelectedIndex = 0;
            this.lbl_dataScadenza_A.Visible = false;
            this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Visible = false;
            this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Visible = false;

            caricaRegistriDisponibili();

            if (this.OnBack)
            {
                // Ripristino registro impostato nella ricerca precedente
                this.RestoreRegistroOnContext();
            }
            else
            {
                settaRegistroSelezionato();
            }
        }

        private void caricaAttributiDisponibili()
        {
            for (int i = 1; i <= 5; i++)
            {
                DropDownList ddlControl = (DropDownList)this.FindControl("DDLFiltro" + i.ToString());
                string[] arrayFiltri = Enum.GetNames(typeof(DocsPAWA.DocsPaWR.FiltriFascicolazione));
                if (ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_ID_LEG) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_ID_LEG).Equals("1"))
                {
                    Utils.populateDdlWithEnumValuesANdKeys(ddlControl, arrayFiltri);
                }
                else
                {
                    System.Collections.ArrayList arrayFiltriCorr = new System.Collections.ArrayList();
                    for (int j = 0; j < arrayFiltri.Length; j++)
                    {
                        if (!arrayFiltri[j].Equals("CODICE_LEGISLATURA"))
                        {
                            arrayFiltriCorr.Add(arrayFiltri[j]);
                        }
                    }
                    string[] arrayFiltri1 = (string[])arrayFiltriCorr.ToArray(typeof(string));
                    Utils.populateDdlWithEnumValuesANdKeys(ddlControl, arrayFiltri1);
                }
            }
        }

        public void SetDefaultButton(Page page, TextBox textControl, Button defaultButton)
        {
            string theScript = @"

				<SCRIPT language=""javascript"">

				<!--
				function fnTrapKD(btn, event){
					if (document.all){
						if (event.keyCode == 9){
							event.returnValue=false;
							event.cancel = true;
							btn.click();
						}
					}
					else if (document.getElementById){
						if (event.which ==9){
							event.returnValue=false;
							event.cancel = true;
							btn.click();
						}
					}
					else if(document.layers){
						if(event.which == 9){
							event.returnValue=false;
							event.cancel = true;
							btn.click();
					}
				}
		}
		// -->
		</SCRIPT>";
            Page.RegisterStartupScript("ForceDefaultToScript", theScript);
            textControl.Attributes.Add("onkeydown", "fnTrapKD(" + defaultButton.ClientID + ",event)");
        }

        private void caricaRegistriDisponibili()
        {
            //this.lbl_ruolo.Text=userRuolo.descrizione;
            userRegistri = UserManager.getListaRegistri(this);
            if (userRegistri != null && userRegistri.Length > 0)
            {
                for (int i = 0; i < userRegistri.Length; i++)
                {
                    //this.ddl_registri.Items.Add((userRuolo.registri[i]).descrizione);
                    this.ddl_registri.Items.Add((userRegistri[i]).codRegistro);
                    this.ddl_registri.Items[i].Value = (userRegistri[i]).systemId;
                }

                //massimo digregorio: sta caricando la lista dei registri. non deve selezionare	nulla.
                //e poi non serve di default è ddl_registri.SelectedIndex = 0;
                //old:              this.ddl_registri.SelectedIndex=0;

            }
        }

        private void settaRegistroSelezionato()
        {
            if (ddl_registri.SelectedIndex != -1)
            {
                if (userRegistri == null)
                    userRegistri = UserManager.getListaRegistri(this);
                UserManager.setRegistroSelezionato(this, userRegistri[this.ddl_registri.SelectedIndex]);
                userReg = userRegistri[this.ddl_registri.SelectedIndex];
                setStatoReg(userRegistri[this.ddl_registri.SelectedIndex]);
                //attenzione! ripulire i campi relativi al mittente e all'oggetto che dipendono dal registro	
            }
        }

        private void settaRegistroStartUp(DocsPAWA.DocsPaWR.Registro reg)
        {
            if (reg != null)
            {
                int n = 0;
                bool goOn = true;
                while (goOn && n < this.ddl_registri.Items.Count)
                {
                    if (this.ddl_registri.Items[n].Value == reg.systemId)
                    {
                        this.ddl_registri.ClearSelection();
                        this.ddl_registri.Items[n].Selected = true;
                        goOn = false;
                        UserManager.setRegistroSelezionato(this, reg);
                        setStatoReg(reg);
                    }
                    n++;
                }
            }

        }
        #endregion Impostazioni iniziali

        #region Ricerca Fascicoli
        private void eseguiRicerca()
        {
            string newUrl;
            if (!ricercaFascicoli())
            {
                return;
            }

            // Salvataggio filtri di ricerca per il ripristino successivo
            this.SaveSearchFilters(this.qV);

            this.ResetPageNumberOnCurrentContext();

            // Impostazione filtri correnti nel contesto di sessione
            this.SetFiltersOnCurrentContext(this.qV);

            // Visualizzazione pagina esito ricerca fascicoli
            //if (!txt_codClass.Text.Equals("") && ddl_titolari.SelectedItem.Text != "Tutti i titolari")


            if (!this.txt_codClass.Text.Equals("") && checkRicercaFasc(this.txt_codClass.Text) == "SI_RICERCA")
            {
                newUrl = navigatePageSearch + "?idClass=1&tabRes=fascicoli";
            }
            else
            {
                if (this.txt_codClass.Text.Equals(""))
                {
                    newUrl = navigatePageSearch + "?idClass=0&tabRes=fascicoli";
                }
                else
                {
                    //if (!IsStartupScriptRegistered("selezionareUnTitolario"))
                    //    ClientScript.RegisterStartupScript(this.GetType(), "selezionareUnTitolario", "alert('Codice presente su più titolari. Selezionare un titolario.');", true);
                    if (!IsStartupScriptRegistered("apriSceltaTitolario"))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "apriSceltaTitolario", "ApriSceltaTitolario('" + this.txt_codClass.Text + "');", true);
                        FascicoliManager.removeMemoriaFiltriRicFasc(this);
                        FascicoliManager.removeFiltroRicFasc(this);
                    }
                    newUrl = navigatePageSearch + "?";
                    FascicoliManager.removeFiltroRicFasc(this);
                }
            }
            //new ADL
            if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
            {
                newUrl += "&ricADL=1";
            }

            Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='" + newUrl + "';</script>");
        }

        private bool ricercaFascicoli()
        {
            bool result = true;
            try
            {
                //array contenitore degli array filtro di ricerca
                qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
                fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

                #region Filtro su codice classificazione fascicolo

                if (this.txt_codClass.Text != string.Empty)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CODICE_CLASSIFICA.ToString();
                    fV1.valore = this.txt_codClass.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                #endregion
                #region  filtro sulla tipologia del fascicolo
                if (ddlTipo.SelectedIndex > 0)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.TIPO_FASCICOLO.ToString();
                    fV1.valore = ddlTipo.SelectedItem.Value;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                #region  filtro sullo stato del fascicolo
                if (ddlStato.SelectedIndex > 0)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.STATO.ToString();
                    fV1.valore = ddlStato.SelectedItem.Value;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                #region numero fascicolo
                if (!this.txtNumFasc.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.NUMERO_FASCICOLO.ToString();
                    fV1.valore = this.txtNumFasc.Text.ToString();
                    if (Utils.isNumeric(fV1.valore))
                    {
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    else
                    {
                        Page.RegisterStartupScript("", "<script>alert('Il numero del fascicolo non è numerico!');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txtNumFasc.ID + "').focus() </SCRIPT>";
                        RegisterStartupScript("focus", s);
                        return false;
                    }
                }
                #endregion
                #region anno fascicolo
                if (!this.txtAnnoFasc.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.ANNO_FASCICOLO.ToString();
                    fV1.valore = this.txtAnnoFasc.Text.ToString();
                    if (Utils.isNumeric(fV1.valore))
                    {
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    else
                    {
                        Page.RegisterStartupScript("", "<script>alert('L\\'anno digitato non è numerico!');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txtAnnoFasc.ID + "').focus() </SCRIPT>";
                        RegisterStartupScript("focus", s);
                        return false;
                    }
                }
                #endregion
                #region  filtro sulla data di apertura fascicolo
                if (this.ddl_dataA.SelectedIndex == 2)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    //fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_TODAY.ToString();
                    //fV1.valore = "1";
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_IL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_initDataA").txt_Data.Text;

                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataA.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataA.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataA.SelectedIndex == 0)
                {//valore singolo carico DATA_APERTURA
                    if (this.GetCalendarControl("txt_initDataA").txt_Data.Text != null && !this.GetCalendarControl("txt_initDataA").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataA").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }

                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataA").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataA.SelectedIndex == 1)
                {//valore singolo carico DATA_APERTURA_DAL - DATA_APERTURA_AL
                    if (!this.GetCalendarControl("txt_initDataA").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataA").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }

                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataA").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txt_fineDataA").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_fineDataA").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_fineDataA").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion
                #region  filtro sulla data chiusura di un fascicolo
                if (this.ddl_dataC.SelectedIndex == 2)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    //fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_TODAY.ToString();
                    //fV1.valore = "1";
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_IL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_initDataC").txt_Data.Text;

                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataC.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataC.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataC.SelectedIndex == 0)
                {//valore singolo carico DATA_CHIUSURA
                    if (this.GetCalendarControl("txt_initDataC").txt_Data.Text != null && !this.GetCalendarControl("txt_initDataC").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataC").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }

                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataC").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataC.SelectedIndex == 1)
                {//valore singolo carico DATA_CHIUSURA_DAL - DATA_CHIUSURA_AL
                    if (!this.GetCalendarControl("txt_initDataC").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataC").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }

                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataC").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txt_fineDataC").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_fineDataC").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_fineDataC").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                //if (!this.GetCalendarControl("txt_initDataC").txt_Data.Text.Equals(""))
                //{
                //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();

                //    //if (this.ddl_dataC.SelectedIndex.Equals(0))
                //    //    fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_IL.ToString();
                //    //else
                //    //    fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_SUCCESSIVA_AL.ToString();

                //    fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_SUCCESSIVA_AL.ToString();

                //    fV1.valore = this.GetCalendarControl("txt_initDataC").txt_Data.Text.ToString();
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //    result = checkData(fV1.argomento, fV1.valore);
                //    if (!result)
                //    {
                //        Page.RegisterStartupScript("", "<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                //        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataC").txt_Data.ID + "').focus() </SCRIPT>";
                //        RegisterStartupScript("focus", s);
                //        return false;
                //    }
                //}

                //if (!this.GetCalendarControl("txt_fineDataC").txt_Data.Text.Equals(""))
                //{
                //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //    fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_PRECEDENTE_IL.ToString();
                //    fV1.valore = this.GetCalendarControl("txt_fineDataC").txt_Data.Text.ToString();
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //    result = checkData(fV1.argomento, fV1.valore);
                //    if (!result)
                //    {
                //        Page.RegisterStartupScript("", "<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                //        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataC").txt_Data.ID + "').focus() </SCRIPT>";
                //        RegisterStartupScript("focus", s);
                //        return false;
                //    }
                //}
                #endregion
                #region  filtro sulla data creazione di un fascicolo
                if (this.ddl_creaz.SelectedIndex == 2)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    //fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_TODAY.ToString();
                    //fV1.valore = "1";
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_IL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_initDataCrea").txt_Data.Text;

                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_creaz.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_creaz.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_creaz.SelectedIndex == 0)
                {//valore singolo carico DATA_CREAZIONE
                    if (this.GetCalendarControl("txt_initDataCrea").txt_Data.Text != null && !this.GetCalendarControl("txt_initDataCrea").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataCrea").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }

                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataCrea").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_creaz.SelectedIndex == 1)
                {//valore singolo carico DATA_CREAZIONE_DAL - DATA_CREAZIONE_AL
                    if (!this.GetCalendarControl("txt_initDataCrea").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataCrea").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }

                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataCrea").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                //if (!this.GetCalendarControl("txt_initDataCrea").txt_Data.Text.Equals(""))
                //{
                //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();

                //    //if (this.ddl_creaz.SelectedIndex.Equals(0))
                //    //    fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_IL.ToString();
                //    //else
                //    //    fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_SUCCESSIVA_AL.ToString();

                //    fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_SUCCESSIVA_AL.ToString();

                //    fV1.valore = this.GetCalendarControl("txt_initDataCrea").txt_Data.Text.ToString();
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //    result = checkData(fV1.argomento, fV1.valore);
                //    if (!result)
                //    {
                //        Page.RegisterStartupScript("", "<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                //        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCrea").txt_Data.ID + "').focus() </SCRIPT>";
                //        RegisterStartupScript("focus", s);
                //        return false;
                //    }
                //}

                //if (!this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text.Equals(""))
                //{
                //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //    fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_PRECEDENTE_IL.ToString();
                //    fV1.valore = this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text.ToString();
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //    result = checkData(fV1.argomento, fV1.valore);
                //    if (!result)
                //    {
                //        Page.RegisterStartupScript("", "<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                //        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataCrea").txt_Data.ID + "').focus() </SCRIPT>";
                //        RegisterStartupScript("focus", s);
                //        return false;
                //    }
                //}
                #endregion
                #region  filtro sulla data scadenza di un fascicolo
                if (this.ddl_dataScadenza.SelectedIndex == 2)
                {
                    //fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    //fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_TODAY.ToString();
                    //fV1.valore = "1";
                    //fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_IL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                if (this.ddl_dataScadenza.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataScadenza.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataScadenza.SelectedIndex == 0)
                {//valore singolo carico DATA_SCADENZA
                    if (this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text != null && !this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }

                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataScadenza.SelectedIndex == 1)
                {//valore singolo carico DATA_SCADENZA_DAL - DATA_SCADENZA_AL
                    if (!this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }

                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                //if (!this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text.Equals(""))
                //{
                //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();

                //    //if (this.ddl_dataScadenza.SelectedIndex.Equals(0))
                //    //    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_IL.ToString();
                //    //else
                //    //    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_SUCCESSIVA_AL.ToString();

                //    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_SUCCESSIVA_AL.ToString();

                //    fV1.valore = this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text.ToString();
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //    result = checkData(fV1.argomento, fV1.valore);
                //    if (!result)
                //    {
                //        Page.RegisterStartupScript("", "<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                //        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.ID + "').focus() </SCRIPT>";
                //        RegisterStartupScript("focus", s);
                //        return false;
                //    }
                //}

                //if (!this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Text.Equals(""))
                //{
                //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_PRECEDENTE_IL.ToString();
                //    fV1.valore = this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Text.ToString();
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //    result = checkData(fV1.argomento, fV1.valore);
                //    if (!result)
                //    {
                //        Page.RegisterStartupScript("", "<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                //        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_dataScadenza_A").txt_Data.ID + "').focus() </SCRIPT>";
                //        RegisterStartupScript("focus", s);
                //        return false;
                //    }
                //}
                #endregion
                #region descrizione
                if (!this.txtDescr.Text.Equals(""))
                {

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.TITOLO.ToString();
                    fV1.valore = this.txtDescr.Text.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                #endregion
                #region data Locazione Fisica
                if (this.ddl_data_LF.SelectedIndex == 2)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    //fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_TODAY.ToString();
                    //fV1.valore = "1";
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_IL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text;

                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_data_LF.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_data_LF.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_data_LF.SelectedIndex == 0)
                {//valore singolo carico DATA_LF
                    if (this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text != null && !this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }

                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_data_LF.SelectedIndex == 1)
                {//valore singolo carico DATA_LF_DAL - DATA_LF_AL
                    if (!this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }

                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                //if (!this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text.Equals(""))
                //{
                //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();

                //    //if (this.ddl_dataScadenza.SelectedIndex.Equals(0))
                //    //    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_IL.ToString();
                //    //else
                //    //    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_SUCCESSIVA_AL.ToString();

                //    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_SUCCESSIVA_AL.ToString();

                //    fV1.valore = this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text.ToString();
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //    result = checkData(fV1.argomento, fV1.valore);
                //    if (!result)
                //    {
                //        Page.RegisterStartupScript("", "<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                //        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_dta_LF_DA").txt_Data.ID + "').focus() </SCRIPT>";
                //        RegisterStartupScript("focus", s);
                //        return false;
                //    }
                //}

                //if (!this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text.Equals(""))
                //{
                //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //    fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_PRECEDENTE_IL.ToString();
                //    fV1.valore = this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text.ToString();
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //    result = checkData(fV1.argomento, fV1.valore);
                //    if (!result)
                //    {
                //        Page.RegisterStartupScript("", "<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                //        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_dta_LF_A").txt_Data.ID + "').focus() </SCRIPT>";
                //        RegisterStartupScript("focus", s);
                //        return false;
                //    }
                //}
                #endregion
                #region descrizione Locazione Fisica
                if (this.hd_systemIdLF != null && this.hd_systemIdLF.Value != "")
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPAWA.DocsPaWR.FiltriFascicolazione.ID_UO_LF.ToString();
                    string IdUoLF = this.hd_systemIdLF.Value;
                    fV1.valore = IdUoLF;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                #region filtro ufficio referente fascicolo
                if (enableUfficioRef)
                {
                    //if (this.txt_desc_uffRef.Text!=null && !this.txt_desc_uffRef.Text.Equals(""))
                    if (this.txt_cod_UffRef.Text != null && !this.txt_cod_UffRef.Text.Equals(""))
                    {
                        if (this.hd_systemIdUffRef != null && !this.hd_systemIdUffRef.Value.Equals(""))
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_UO_REF.ToString();
                            fV1.valore = this.hd_systemIdUffRef.Value;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        else
                        {
                            if (!IsStartupScriptRegistered("alert"))
                            {
                                Page.RegisterStartupScript("", "<script>alert('Codice rubrica non valido per l\\'Ufficio referente!');</script>");
                                string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_cod_UffRef.ID + "').focus() </SCRIPT>";
                                RegisterStartupScript("focus", s);
                            }

                            return false;
                        }

                        //						else//old perchè uff ref è solo interno
                        //						{
                        //							fV1.argomento=DocsPaWR.FiltriFascicolazione.DESC_UO_REF.ToString();
                        //							fV1.valore = DocsPaUtils.Functions.Functions.ReplaceApexes(this.txt_desc_uffRef.Text);
                        //						}
                    }

                }
                #endregion
                #region Note Fascicolo
//                if (!this.txtNote.Text.Equals(""))
                if (!this.rn_note.Testo.Equals(""))
                {

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.VAR_NOTE.ToString();
                    string[] rf;
                    string rfsel = "0";
                    if (Session["RFNote"]!= null && !string.IsNullOrEmpty(Session["RFNote"].ToString()))
                    {
                        rf = Session["RFNote"].ToString().Split('^');
                        rfsel = rf[1];
                    }
//                    fV1.valore = this.txtNote.Text.ToString();
                    fV1.valore = this.rn_note.Testo.ToString() + "@-@" + rn_note.TipoRicerca + "@-@" + rfsel;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                #endregion
                #region Mostra tutti i fascicoli

                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.INCLUDI_FASCICOLI_FIGLI.ToString();
                fV1.valore = this.rbl_MostraTutti.SelectedValue;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                #endregion
                #region filtro tipologia fascicolo e profilazione dinamica

                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                {
                    if (Session["filtroProfDinamica"] != null)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1 = (DocsPaWR.FiltroRicerca)Session["filtroProfDinamica"];
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (this.ddl_tipoFasc.SelectedIndex > 0)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.TIPOLOGIA_FASCICOLO.ToString();
                        fV1.valore = this.ddl_tipoFasc.SelectedItem.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                #endregion
                #region filtro DIAGRAMMI DI STATO
                if (ddl_statiDoc.Visible && ddl_statiDoc.SelectedIndex != 0)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPAWA.DocsPaWR.FiltriFascicolazione.DIAGRAMMA_STATO_FASC.ToString();
                    //string cond = " AND (DPA_DIAGRAMMI.ID_PROJECT = A.SYSTEM_ID AND DPA_DIAGRAMMI.ID_STATO = " + ddl_statiDoc.SelectedValue + ") ";
                    //fV1.valore = cond;
                    fV1.valore = ddl_statiDoc.SelectedValue;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion filtro DIAGRAMMI DI STATO
                #region Filtro diTitolario

                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_TITOLARIO.ToString();
                fV1.valore = this.ddl_titolari.SelectedValue;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                #endregion
                #region filtro RICERCA IN AREA LAVORO
                if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1"))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DOC_IN_FASC_ADL.ToString();
                    fV1.valore = UserManager.getInfoUtente(this).idPeople.ToString() + "@" + UserManager.getRuolo(this).systemId.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                #region filtro CONSERVAZIONE
                if (this.cb_Conservato.Checked)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CONSERVAZIONE.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.cb_NonConservato.Checked)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CONSERVAZIONE.ToString();
                    fV1.valore = "0";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                #region FILTRO SUI SOTTOFASCICOLI

                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.SOTTOFASCICOLO.ToString();
                fV1.valore = this.txt_sottofascicolo.Text;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                #endregion

                //#region filtro Creatore (User Control)
                //fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //fV1 = this.Creatore.GetFilter();
                //if (fV1 != null)
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //#endregion
                #region Filtri Creatore e Proprietario

                foreach (var ownerFilter in this.aofOwner.GetFiltersList())
                    fVList = Utils.addToArrayFiltroRicerca(fVList, ownerFilter);


                foreach (var authorFilter in this.aofAuthor.GetFiltersList())
                    fVList = Utils.addToArrayFiltroRicerca(fVList, authorFilter);

                #endregion

                //if (this.optListTipiCreatore.SelectedValue == "U")
                //{
                //    if (!this.txtCodiceUtenteCreatore.Text.Equals(""))
                //    {
                //        #region filtro su ID_UO_CREATORE
                //        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //        fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_UO_CREATORE.ToString();
                //        fV1.valore = this.txtSystemIdUtenteCreatore.Value;
                //        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //        #endregion
                //    }
                //    else
                //    {
                //        if (!this.txtDescrizioneUtenteCreatore.Text.Equals(""))
                //        {
                //            #region filtro su DESC_UO_CREATORE
                //            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //            fV1.argomento = DocsPaWR.FiltriFascicolazione.DESC_UO_CREATORE.ToString();
                //            fV1.valore = this.txtDescrizioneUtenteCreatore.Text;
                //            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //            #endregion
                //        }
                //    }
                //    #region filtro su UO sottoposte
                //    //if (this.cbx_UOsotto.Checked)
                //    //{
                //    //    fV1 = new DocsPaWR.FiltroRicerca();
                //    //    fV1.argomento = DocsPaWR.FiltriFascicolazione.UO_SOTTOPOSTE.ToString();
                //    //    fV1.valore = "1";
                //    //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //    //}
                //    #endregion
                //}
                //else
                //{
                //    if (this.optListTipiCreatore.SelectedValue == "R")
                //    {
                //        if (!this.txtCodiceUtenteCreatore.Text.Equals(""))
                //        {
                //            #region filtro su ID_RUOLO_CREATORE
                //            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //            fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_RUOLO_CREATORE.ToString();
                //            fV1.valore = this.txtSystemIdUtenteCreatore.Value;
                //            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //            #endregion
                //        }
                //        else
                //        {
                //            if (!this.txtDescrizioneUtenteCreatore.Text.Equals(""))
                //            {
                //                #region filtro su DESC_RUOLO_CREATORE
                //                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //                fV1.argomento = DocsPaWR.FiltriFascicolazione.DESC_RUOLO_CREATORE.ToString();
                //                fV1.valore = this.txtDescrizioneUtenteCreatore.Text;
                //                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //                #endregion
                //            }
                //        }
                //    }
                //    else
                //    {
                //        if (this.optListTipiCreatore.SelectedValue == "P")
                //        {
                //            if (!this.txtCodiceUtenteCreatore.Text.Equals(""))
                //            {
                //                #region filtro su ID_PEOPLE_CREATORE
                //                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //                fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_PEOPLE_CREATORE.ToString();
                //                fV1.valore = this.txtSystemIdUtenteCreatore.Value;
                //                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //                #endregion
                //            }
                //            else
                //            {
                //                if (!this.txtDescrizioneUtenteCreatore.Text.Equals(""))
                //                {
                //                    #region filtro su DESC_PEOPLE_CREATORE
                //                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DESC_PEOPLE_CREATORE.ToString();
                //                    fV1.valore = this.txtDescrizioneUtenteCreatore.Text;
                //                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //                    #endregion
                //                }
                //            }
                //        }
                //    }
                //}

                #region Odinamento contatore no griglie custum
                //Se non ho le griglie custom
                if (!GridManager.IsRoleEnabledToUseGrids())
                {
                    if (ddl_tipoFasc.SelectedItem != null && !string.IsNullOrEmpty(ddl_tipoFasc.SelectedItem.Text))
                    {
                        DocsPAWA.DocsPaWR.Templates template = ProfilazioneFascManager.getTemplateFascById(ddl_tipoFasc.SelectedItem.Value, this);
                        if (template != null)
                        {
                            OggettoCustom customObjectTemp = new OggettoCustom();
                            customObjectTemp = template.ELENCO_OGGETTI.Where(
                            r => r.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && r.DA_VISUALIZZARE_RICERCA == "1").
                            FirstOrDefault();
                            if (customObjectTemp != null && ddlOrder != null && ddlOrder.SelectedValue != null && ddlOrder.SelectedValue.Equals("-2"))
                            {
                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.CONTATORE_GRIGLIE_NO_CUSTOM.ToString();
                                fV1.valore = customObjectTemp.TIPO_CONTATORE;
                                fV1.nomeCampo = template.SYSTEM_ID.ToString();
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                                // Creazione di un filtro per la profilazione
                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = FiltriDocumento.PROFILATION_FIELD_FOR_ORDER.ToString();
                                fV1.valore = customObjectTemp.SYSTEM_ID.ToString();
                                fV1.nomeCampo = customObjectTemp.DESCRIZIONE;
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                        }


                    }
                }

                #endregion
				

                #region filtro Deposito
                //if (!this.rbl_TrasfDep.SelectedItem.Value.Equals("T"))
                //{
                //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //    fV1.argomento = DocsPaWR.FiltriFascicolazione.DEPOSITO.ToString();
                //    fV1.valore = this.rbl_TrasfDep.SelectedItem.Value;
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //}
                #endregion

                #region Ordinamento

                // Reperimento del filtro da utilizzare per la griglia
                List<FiltroRicerca> filterList = GridManager.GetOrderFilterForProject(this.ddlOrder.SelectedValue, this.ddlOrderDirection.SelectedValue);

                // Se la lista è valorizzata vengono aggiunti i filtri
                if (filterList != null)
                {
                    foreach (FiltroRicerca filter in filterList)
                    {
                        fVList = Utils.addToArrayFiltroRicerca(fVList, filter);

                        if (filter.argomento.Equals("ORDER_DIRECTION") && GridManager.IsRoleEnabledToUseGrids())
                        {
                            OrderDirectionEnum tempEnum = OrderDirectionEnum.Desc;
                            if (filter.valore.ToUpper().Equals("ASC"))
                            {
                                tempEnum = OrderDirectionEnum.Asc;
                            }
                            else
                            {
                                tempEnum = OrderDirectionEnum.Desc;
                            }

                            if (GridManager.SelectedGrid.OrderDirection != tempEnum)
                            {
                                GridManager.SelectedGrid.OrderDirection = tempEnum;
                                GridManager.SelectedGrid.GridId = string.Empty;
                            }
                        }
                        if (filter.argomento.Equals("ORACLE_FIELD_FOR_ORDER") && GridManager.IsRoleEnabledToUseGrids())
                        {
                            Field d = (Field)GridManager.SelectedGrid.Fields.Where(e => e.FieldId.ToUpper().Equals((filter.nomeCampo).ToUpper())).FirstOrDefault();
                            if ((GridManager.SelectedGrid.FieldForOrder == null && d != null) || GridManager.SelectedGrid.FieldForOrder.FieldId != d.FieldId)
                            {
                                GridManager.SelectedGrid.FieldForOrder = d;
                                GridManager.SelectedGrid.GridId = string.Empty;
                            }
                        }
                    }
                }

                #endregion

                #region Visibilità Tipica / Atipica
                if (pnl_visiblitaFasc.Visible)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.VISIBILITA_T_A.ToString();
                    fV1.valore = rbl_visibilita.SelectedValue;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion Visibilità Tipica / Atipica

                /* ABBATANGELI GIANLUIGI */
                /// <summary>
                /// Creazione oggetti di filtro per oggetto documento
                /// </summary>
                /// <param name="filterItems"></param>

                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"]))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.COD_EXT_APP.ToString();
                    fV1.valore = (System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"]);//rbl_visibilita.SelectedValue;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                qV[0] = fVList;

                FascicoliManager.setFiltroRicFasc(this, qV);
                return result;
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
                result = false;
                return result;
            }
        }

        private bool getMostraTuttiFascicoliValue()
        {
            bool retFunction;

            if (this.rbl_MostraTutti.SelectedItem.Value.Equals("S"))
            {
                retFunction = true;
            }
            else
            {
                retFunction = false;
            }

            return retFunction;
        }

        private void btn_ricFascicoli_Click(object sender, System.EventArgs e)
        {
            try
            {
                string funct_dx1 = string.Empty;

                // Salvataggio dei filtri per la ricerca proprietario e creatore
                this.aofOwner.SaveFilters();
                this.aofAuthor.SaveFilters();

                if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null && !Page.IsPostBack)
                {
                    GridManager.CompileDdlOrderAndSetOrderFilterProjects(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                }

                if (string.IsNullOrEmpty(this.txt_codClass.Text))
                {
                    FascicoliManager.removeMemoriaClassificaRicFasc(this.Page);
                    Session["classificaSelezionata"] = null;
                }

                #region Controllo intervallo date apertura
                if (!this.GetCalendarControl("txt_initDataA").txt_Data.Text.Equals(""))
                {
                    if (!Utils.isDate(this.GetCalendarControl("txt_initDataA").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare valori inseriti per Data Apertura!');</script>");
                        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli';";
                        else
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?tabRes=fascicoli';";

                        Response.Write("<script language='javascript'> " + funct_dx1 + "</script>");
                        return;
                    }
                }
                if (!this.GetCalendarControl("txt_fineDataA").txt_Data.Text.Equals(""))
                {
                    if (!Utils.isDate(this.GetCalendarControl("txt_fineDataA").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare valori inseriti per Data Apertura!');</script>");
                        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli';";
                        else
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?tabRes=fascicoli';";

                        Response.Write("<script language='javascript'> " + funct_dx1 + "</script>");
                        return;
                    }
                }
                if (!this.GetCalendarControl("txt_initDataA").txt_Data.Text.Equals("") && !this.GetCalendarControl("txt_fineDataA").txt_Data.Text.Equals(""))
                {
                    if (Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataA").txt_Data.Text, this.GetCalendarControl("txt_fineDataA").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare valori inseriti per Data Apertura!');</script>");
                        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli';";
                        else
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?tabRes=fascicoli';";

                        Response.Write("<script language='javascript'> " + funct_dx1 + "</script>");
                        return;
                    }
                }
                #endregion

                #region Controllo intervallo date chiusura
                if (!this.GetCalendarControl("txt_initDataC").txt_Data.Text.Equals(""))
                {
                    if (!Utils.isDate(this.GetCalendarControl("txt_initDataC").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare valori inseriti per Data Chiusura!');</script>");
                        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli';";
                        else
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?tabRes=fascicoli';";

                        Response.Write("<script language='javascript'> " + funct_dx1 + "</script>");
                        return;
                    }
                }
                if (!this.GetCalendarControl("txt_fineDataC").txt_Data.Text.Equals(""))
                {
                    if (!Utils.isDate(this.GetCalendarControl("txt_fineDataC").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare valori inseriti per Data Chiusura!');</script>");
                        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli';";
                        else
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?tabRes=fascicoli';";

                        Response.Write("<script language='javascript'> " + funct_dx1 + "</script>");
                        return;
                    }
                }
                if (!this.GetCalendarControl("txt_initDataC").txt_Data.Text.Equals("") && !this.GetCalendarControl("txt_fineDataC").txt_Data.Text.Equals(""))
                {
                    if (Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataC").txt_Data.Text, this.GetCalendarControl("txt_fineDataC").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare valori inseriti per Data Chiusura!');</script>");
                        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli';";
                        else
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?tabRes=fascicoli';";

                        Response.Write("<script language='javascript'> " + funct_dx1 + "</script>");
                        return;
                    }
                }
                #endregion

                #region Controllo intervallo date creazione
                if (!this.GetCalendarControl("txt_initDataCrea").txt_Data.Text.Equals(""))
                {
                    if (!Utils.isDate(this.GetCalendarControl("txt_initDataCrea").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare valori inseriti per Data Creazione!');</script>");
                        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli';";
                        else
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?tabRes=fascicoli';";

                        Response.Write("<script language='javascript'> " + funct_dx1 + "</script>");
                        return;
                    }
                }
                if (!this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text.Equals(""))
                {
                    if (!Utils.isDate(this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare valori inseriti per Data Creazione!');</script>");
                        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli';";
                        else
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?tabRes=fascicoli';";

                        Response.Write("<script language='javascript'> " + funct_dx1 + "</script>");
                        return;
                    }
                }
                if (!this.GetCalendarControl("txt_initDataCrea").txt_Data.Text.Equals("") && !this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text.Equals(""))
                {
                    if (Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataCrea").txt_Data.Text, this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare valori inseriti per Data Creazione!');</script>");
                        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli';";
                        else
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?tabRes=fascicoli';";

                        Response.Write("<script language='javascript'> " + funct_dx1 + "</script>");
                        return;
                    }
                }
                #endregion

                #region Controllo intervallo date collocazione
                if (!this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text.Equals(""))
                {
                    if (!Utils.isDate(this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare valori inseriti per Data Collocazione!');</script>");
                        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli';";
                        else
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?tabRes=fascicoli';";

                        Response.Write("<script language='javascript'> " + funct_dx1 + "</script>");
                        return;
                    }
                }
                if (!this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text.Equals(""))
                {
                    if (!Utils.isDate(this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare valori inseriti per Data Collocazione!');</script>");
                        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli';";
                        else
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?tabRes=fascicoli';";

                        Response.Write("<script language='javascript'> " + funct_dx1 + "</script>");
                        return;
                    }
                }
                if (!this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text.Equals("") && !this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text.Equals(""))
                {
                    if (Utils.verificaIntervalloDate(this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text, this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare valori inseriti per Data Collocazione!');</script>");
                        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli';";
                        else
                            funct_dx1 = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?tabRes=fascicoli';";

                        Response.Write("<script language='javascript'> " + funct_dx1 + "</script>");
                        return;
                    }
                }
                #endregion

                //if (!this.txt_codClass.Text.Equals("") && ddl_titolari.SelectedItem.Text != "Tutti i titolari")
                if (!this.txt_codClass.Text.Equals("") && checkRicercaFasc(this.txt_codClass.Text) == "SI_RICERCA")
                {
                    bool res = false;
                    res = cercaClassificazioneDaCodice();
                    if (!res)
                    {
                        if (!this.txt_codClass.Text.Equals(""))
                        {
                            string s2 = "<script>alert('Attenzione: codice classifica non presente!');</script>";
                            if (!IsStartupScriptRegistered("NoCod"))
                                Page.RegisterStartupScript("NoCod", s2);
                            //Page.RegisterStartupScript("NoCod","<script>alert('Attenzione: codice classifica non presente!');top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_codClass.ID + "').focus() </SCRIPT>";
                            RegisterStartupScript("focus", s);
                            impostaAbilitazioneNuovoFascNuovoTit();
                            //btn_new.Enabled = false;
                            //this.btn_new_tit.Enabled = false;
                            FascicoliManager.removeFiltroRicFasc(this);
                            //ricercaFascicoli();
                            string funct_dx = string.Empty;
                            // nuova ADL
                            if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                                funct_dx = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli';";
                            else
                                funct_dx = "top.principale.iFrame_dx.document.location='NewTabSearchResult.aspx?tabRes=fascicoli';";

                            Response.Write("<script language='javascript'> " + funct_dx + "</script>");
                            ddl_titolari.SelectedIndex = 0;
                        }
                        return;
                    }
                    Session["newClass"] = "S";
                    FascicoliManager.removeClassificazioneSelezionata(this);
                    //abilito il PULSANTE NUOVO
                    impostaAbilitazioneNuovoFascNuovoTit();
                    //btn_new.Enabled = true;
                    //this.btn_new_tit.Enabled = true;
                }
                else
                {
                    ClearDDL(1, 6);
                    impostaAbilitazioneNuovoFascNuovoTit();
                    //btn_new.Enabled = false;
                    //this.btn_new_tit.Enabled = false;
                }
                //if (!this.txt_codClass.Text.Equals("") && ddl_titolari.SelectedItem.Text != "Tutti i titolari")
                if (!this.txt_codClass.Text.Equals("") && checkRicercaFasc(this.txt_codClass.Text) == "SI_RICERCA")
                {
                    if (this.ddl_registri.SelectedItem != null)
                    {
                        caricaTitolariRegistro(false);
                        if (this.flagCodice == "1")
                        {
                            return;
                        }
                    }

                    if (TheHash == null || TheHash.Count < 1)
                        return;
                }
                Session["newClass"] = "N";
                enableFascFields();
                eseguiRicerca();
                schedaRicerca.FiltriRicerca = qV;
                FascicoliManager.setFiltroRicFasc(this, qV);
                FascicoliManager.removeDatagridFascicolo(this);
                FascicoliManager.removeListaFascicoliInGriglia(this);
                if (Session["ricercaCorrispondenteStoricizzato"] != null)
                    Session.Remove("ricercaCorrispondenteStoricizzato");
                if (Session["CorrSelezionatoDaMulti"] != null)
                    Session.Remove("CorrSelezionatoDaMulti");

                
                ViewState["new_search"] = "true";
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }

        }

        private void enableFascFields()
        {
            //abilitazione campi per la ricerca dei fascicoli
            this.lbl_mostraTuttiFascicoli.Enabled = true;
            this.rbl_MostraTutti.Enabled = true;
            this.btn_ricFascicoli.Enabled = true;
            this.btn_del.Enabled = true;
            //this.btn_new.Enabled = true;
        }

        private bool checkData(string argomento, string valore)
        {
            if (argomento.Equals("APERTURA_IL") ||
                argomento.Equals("APERTURA_SUCCESSIVA_AL") ||
                argomento.Equals("APERTURA_PRECEDENTE_IL") ||
                argomento.Equals("CHIUSURA_IL") ||
                argomento.Equals("CHIUSURA_SUCCESSIVA_AL") ||
                argomento.Equals("CHIUSURA_PRECEDENTE_IL") ||
                argomento.Equals("CREAZIONE_IL") ||
                argomento.Equals("CREAZIONE_SUCCESSIVA_AL") ||
                argomento.Equals("CREAZIONE_PRECEDENTE_IL") ||
                argomento.Equals("DATA_LF_PRECEDENTE_IL") ||
                argomento.Equals("DATA_LF_SUCCESSIVA_AL"))
            {
                if (!Utils.isDate(valore))
                {
                    return false;
                }
            }
            return true;
        }

        private void rbl_MostraTutti_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            allClass = getMostraTuttiFascicoliValue();
            FascicoliManager.setAllClassValue(this, allClass);
        }

        private bool cercaClassificazioneDaCodice(string codClassificazione)
        {
            bool res = false;
            this.numFasc = 0;
            if (!string.IsNullOrEmpty(codClassificazione))
            {
                this.listaFasc = getFascicolo(UserManager.getRegistroSelezionato(this), codClassificazione);
                if (listaFasc != null && listaFasc.Length > 0)
                {
                    this.numFasc = listaFasc.Length;
                }
            }
            //Viene effettuato perchè la ricerca deve essere sempre fatto in base al titolario selezionato
            //Verifico se non provengo da una selezione di titolario
            //if (Session["idTitolarioSelezionato"] == null)
            //{
            //    DocsPaWR.FiltroRicerca item = new DocsPAWA.DocsPaWR.FiltroRicerca();
            //    item.argomento = DocsPaWR.FiltriFascicolazione.ID_TITOLARIO.ToString();
            //    item.valore = ddl_titolari.SelectedValue;
            //    RestoreFiltersIdTitolario(item);
            //}

            //if (ddl_titolari.SelectedItem.Text == "Tutti i titolari" && listaFasc.Length > 1)
            if (checkRicercaFasc(codClassificazione) == "NO_RICERCA" && codClassificazione != "")
            {
                //ClientScript.RegisterStartupScript(this.GetType(), "selezionareUnTitolario", "alert('Codice presente su più titolari. Selezionare un titolario.');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "apriSceltaTitolario", "ApriSceltaTitolario('" + codClassificazione + "');", true);
                FascicoliManager.removeMemoriaFiltriRicFasc(this);
                FascicoliManager.removeFiltroRicFasc(this);
                return true;
            }

            //if (ddl_titolari.SelectedItem.Text != "Tutti i titolari" || (ddl_titolari.SelectedItem.Text == "Tutti i titolari" && listaFasc.Length == 1))
            if (checkRicercaFasc(codClassificazione) == "SI_RICERCA")
            {
                DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchiaDaCodice2(this, codClassificazione, UserManager.getUtente(this).idAmministrazione, getIdTitolario(codClassificazione));

                //esegue routine di svuotamento di tutte le combobox
                initListaCombo(0);
                if (gerClassifica != null)
                {
                    //Recupero l'idTitolario di appartenenza presente solo sull'ultimo nodo figlio
                    string idTitolatio = gerClassifica[gerClassifica.Length - 1].idTitolario;
                    if (idTitolatio != null && idTitolatio != "")
                        ddl_titolari.SelectedValue = idTitolatio;

                    DocsPaWR.FascicolazioneClassifica prevClassifica = null;
                    int lastIndex = 0;
                    for (int i = 0; i < gerClassifica.Length; i++)
                    {

                        DocsPaWR.FascicolazioneClassifica classifica = (DocsPAWA.DocsPaWR.FascicolazioneClassifica)gerClassifica[i];
                        DocsPaWR.FascicolazioneClassifica classPadre = prevClassifica;
                        prevClassifica = classifica;
                        //Elisa 11/08/2005 gestione nodo titolario ReadOnly
                        //Session.Add("cha_ReadOnly", classifica.cha_ReadOnly);
                        Session.Add("classificaSelezionata", classifica);
                        

                        //recupera tutte le classificazioni figlie di primo livello della classifica selezionata
                        //DocsPaWR.FascicolazioneClassifica[] classifiche = FascicoliManager.GetFigliClassifica(this, classPadre, UserManager.getUtente(this).idAmministrazione);
                        DocsPaWR.FascicolazioneClassifica[] classifiche = FascicoliManager.GetFigliClassifica2(this, classPadre, UserManager.getUtente(this).idAmministrazione, getIdTitolario(codClassificazione));

                        //modifica per far si che se il nodo del titolario richiesto è presente
                        //nell'amministrazione di interesse ma non è visibili all'utente loggato
                        //a DocsPa venga comunque fornito l'alert a video.
                        for (int k = 0; k < classifiche.Length; k++)
                        {
                            if (codClassificazione.ToUpper() == (classifiche[k].codice.ToUpper()))
                            {
                                res = true;
                                k = classifiche.Length;
                            }
                        }


                        caricaCombo(i, classifiche);
                        selectValueInCombo(i, classifica.codice);
                        lastIndex = i;
                    }
                    if (res == false)
                    {
                        ClearDDL(1, 6);
                        return res;
                    }
                    //fine modifica
                    gestioneSelezioneElemento(elementByIndex(lastIndex));
                }
                else
                {
                    caricamentoClassificazioniPadri();
                }
            }

            return res;
        }

        private void selectValueInCombo(int indexCombo, string valore)
        {
            DropDownList ddlCombo = elementByIndex(indexCombo);
            if (ddlCombo != null)
            {
                ListItem itemToSelect = ddlCombo.Items.FindByValue(valore);
                if (itemToSelect != null)
                {
                    itemToSelect.Selected = true;
                }
            }
        }

        private bool cercaClassificazioneDaCodice()
        {
            string codClassificazione = this.txt_codClass.Text.ToString();

            if (string.IsNullOrEmpty(codClassificazione))
            {
                return false;
            }
            else
            {
                return this.cercaClassificazioneDaCodice(codClassificazione);
            }
        }

        private bool IsAbilitataRicercaSottoFascicoli()
        {
            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.CERCA_SOTTOFASCICOLI) == null || ConfigSettings.getKey(ConfigSettings.KeysENUM.CERCA_SOTTOFASCICOLI).Equals("0"))
                return false;
            else
                return true;
        }

        #endregion

        #region Gestione Livelli DDL

        private void impostaAbilitazioneNuovoFascNuovoTit()
        {
            //E' selezionata la voce tutti i titolari
            if (ddl_titolari.Enabled && ddl_titolari.SelectedIndex == 0)
            {
                OptLst.SelectedIndex = 0;
                OptLst.Enabled = false;
                EnabledisabledDDL(1, 6, false);
                btn_new.Enabled = false;
                btn_new_tit.Enabled = false;
                FascicoliManager.removeClassificazioneSelezionata(this);
            }
            else
            {
                //Verifico se il titolario selezionato è attivo o meno
                //DocsPAWA.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(ddl_titolari.SelectedValue);
                bool chiave = true;
                //NUOVA FUNZIONE POSSO CREARE NUOVI FASCICOLI IN TUTTI I TITOLARI ANCHE QUELLI CHIUSI
                DocsPAWA.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(getIdTitolario(null));
                if (utils.InitConfigurationKeys.GetValue("0", "BE_FASC_TUTTI_TIT").Equals("1"))
                {
                    OptLst.Enabled = true;
                    if (txt_codClass.Text != null && txt_codClass.Text != "")
                    {
                        if (Session["classificaSelezionata"] != null)
                        {
                            DocsPaWR.FascicolazioneClassifica classifica = (DocsPaWR.FascicolazioneClassifica)Session["classificaSelezionata"];
                            btn_new.Enabled = true;
                            btn_new_tit.Enabled = (classifica.bloccaNodiFigli == "SI" ? false : true);
                        }
                    }
                    else
                    {
                        btn_new.Enabled = false;
                        btn_new_tit.Enabled = false;
                    }
                }
                else
                {
                    switch (titolario.Stato)
                    {
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Attivo:
                            OptLst.Enabled = true;
                            if (txt_codClass.Text != null && txt_codClass.Text != "")
                            {
                                //btn_new.Enabled = !(Session["cha_ReadOnly"] != null ? ((bool)Session["cha_ReadOnly"]) : false);
                                //btn_new_tit.Enabled = !(Session["cha_ReadOnly"] != null ? ((bool)Session["cha_ReadOnly"]) : false);

                                if (Session["classificaSelezionata"] != null)
                                {
                                    DocsPaWR.FascicolazioneClassifica classifica = (DocsPaWR.FascicolazioneClassifica)Session["classificaSelezionata"];
                                    btn_new.Enabled = !classifica.cha_ReadOnly;
                                    btn_new_tit.Enabled = (classifica.bloccaNodiFigli == "SI" ? false : true);
                                }
                            }
                            else
                            {
                                btn_new.Enabled = false;
                                btn_new_tit.Enabled = false;

                            }
                            break;
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                            OptLst.Enabled = true;
                            btn_new.Enabled = false;
                            btn_new_tit.Enabled = false;
                            break;
                    }
                }
            }
        }


        private void ddl_titolari_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            RemoveIdTitolario();
            initListaCombo(0);
            txt_codClass.Text = string.Empty;
            txt_protoPratica.Text = string.Empty;
            txt_codClass.Enabled = true;
            txt_codClass.ReadOnly = false;

            impostaAbilitazioneNuovoFascNuovoTit();

            caricamentoClassificazioniPadri();
        }

        private void caricaComboTitolari()
        {
            ddl_titolari.Items.Clear();
            //ArrayList listaTitolari = new ArrayList(wws.getTitolari(UserManager.getUtente(this).idAmministrazione));
            ArrayList listaTitolari = new ArrayList(wws.getTitolariUtilizzabili(UserManager.getUtente(this).idAmministrazione));

            //Esistono dei titolari chiusi
            if (listaTitolari.Count > 1)
            {
                //Creo le voci della ddl dei titolari
                string valueTutti = string.Empty;
                foreach (DocsPaWR.OrgTitolario titolario in listaTitolari)
                {
                    ListItem it = null;
                    switch (titolario.Stato)
                    {
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Attivo:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            ddl_titolari.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            ddl_titolari.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                    }
                }
                //Imposto la voce tutti i titolari
                valueTutti = valueTutti.Substring(0, valueTutti.Length - 1);
                if (valueTutti != string.Empty)
                {
                    if (valueTutti.IndexOf(',') == -1)
                        valueTutti = valueTutti + "," + valueTutti;

                    ListItem it = new ListItem("Tutti i titolari", valueTutti);
                    ddl_titolari.Items.Insert(0, it);
                }

                OptLst.SelectedIndex = 0;
                OptLst.Enabled = false;
                txt_codClass.Enabled = true;

                //se la chiave di db è a 1, seleziono di default il titolario attivo
                if (utils.InitConfigurationKeys.GetValue("0", "FE_CHECK_TITOLARIO_ATTIVO").Equals("1"))
                {
                    int indexTitAtt = 0;
                    foreach (DocsPaWR.OrgTitolario titolario in listaTitolari)
                    {
                        if (titolario.Stato == DocsPaWR.OrgStatiTitolarioEnum.Attivo)
                        {
                            ddl_titolari.SelectedIndex = ++indexTitAtt;
                            break;
                        }
                        indexTitAtt++;
                    }
                }
            }

            //Non esistono titolario chiusi
            if (listaTitolari.Count == 1)
            {
                DocsPaWR.OrgTitolario titolario = (DocsPaWR.OrgTitolario)listaTitolari[0];
                if (titolario.Stato != DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione)
                {
                    ListItem it = new ListItem(titolario.Descrizione, titolario.ID);
                    ddl_titolari.Items.Add(it);
                }
                ddl_titolari.Enabled = false;
            }

            //Imposto le etichette per il titolario e i suoi livelli
            if (listaTitolari != null && listaTitolari.Count > 0)
            {
                DocsPaWR.OrgTitolario titolario = (DocsPaWR.OrgTitolario)listaTitolari[0];
                if (!string.IsNullOrEmpty(titolario.EtichettaTit))
                    lbl_Titolari.Text = titolario.EtichettaTit;
                if (!string.IsNullOrEmpty(titolario.EtichettaLiv1))
                    lbl_livello1.Text = titolario.EtichettaLiv1;
                if(!string.IsNullOrEmpty(titolario.EtichettaLiv2))
                    lbl_livello2.Text = titolario.EtichettaLiv2;
                if (!string.IsNullOrEmpty(titolario.EtichettaLiv3))
                    lbl_livello3.Text = titolario.EtichettaLiv3;
                if (!string.IsNullOrEmpty(titolario.EtichettaLiv4))
                    lbl_livello4.Text = titolario.EtichettaLiv4;
                if (!string.IsNullOrEmpty(titolario.EtichettaLiv5))
                    lbl_livello5.Text = titolario.EtichettaLiv5;
                if (!string.IsNullOrEmpty(titolario.EtichettaLiv6))
                    lbl_livello6.Text = titolario.EtichettaLiv6;
                //Abilito le ddl in funzione del livello di profondità del titolario
                try
                {
                    int maxLivTitolario = Convert.ToInt16(titolario.MaxLivTitolario);
                    for (int i = 6; i > maxLivTitolario; i--)
                    {
                        DropDownList ddlControl = (DropDownList)this.FindControl("ddl_livello" + i.ToString());
                        Label labelControl = (Label)this.FindControl("lbl_livello" + i.ToString());
                        labelControl.Visible = false;
                        ddlControl.Visible = false;
                    }
                }
                catch (Exception e) { }                
            }
        }

        private void ddl_livelloControl_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                DropDownList ddlControl = (DropDownList)sender;
                gestioneSelezioneElemento(ddlControl);
                Session["newClass"] = "S";
                FascicoliManager.removeClassificazioneSelezionata(this);
                impostaAbilitazioneNuovoFascNuovoTit();
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void gestioneSelezioneElemento(DropDownList ddlControl)
        {
            int indexCombo = indexByElement(ddlControl);
            if (indexCombo >= 0)
            {
                string codClassifica = "";
                if (ddlControl.SelectedIndex >= 0)
                {
                    codClassifica = ddlControl.Items[ddlControl.SelectedIndex].Value.ToString();
                }
                setCodiceClassificaInTextBox(indexCombo);

                DocsPaWR.FascicolazioneClassifica[] classificheFiglie;

                //11/08/2005 elisa
                if (this.txt_codClass.Text != "")
                    codClassifica = this.txt_codClass.Text;
                //
                if (codClassifica != "")
                {
                    //carico la gerarchia per la classifica selezionata
                    //DocsPaWR.FascicolazioneClassifica[] gerClassifiche = FascicoliManager.getGerarchiaDaCodice(this, codClassifica, UserManager.getUtente(this).idAmministrazione);
                    //DocsPaWR.FascicolazioneClassifica[] gerClassifiche = FascicoliManager.getGerarchiaDaCodice2(this, codClassifica, UserManager.getUtente(this).idAmministrazione, ddl_titolari.SelectedValue);
                    DocsPaWR.FascicolazioneClassifica[] gerClassifiche = FascicoliManager.getGerarchiaDaCodice2(this, codClassifica, UserManager.getUtente(this).idAmministrazione, getIdTitolario(codClassifica));

                    if (gerClassifiche != null)
                    {
                        //classifica selezionata
                        DocsPaWR.FascicolazioneClassifica classifica = gerClassifiche[gerClassifiche.Length - 1];
                        //Session.Add("cha_ReadOnly", classifica.cha_ReadOnly);
                        Session.Add("classificaSelezionata", classifica);
                        
                        //classificheFiglie = FascicoliManager.GetFigliClassifica(this, classifica, UserManager.getUtente(this).idAmministrazione);
                        //classificheFiglie = FascicoliManager.GetFigliClassifica2(this, classifica, UserManager.getUtente(this).idAmministrazione, ddl_titolari.SelectedValue);
                        classificheFiglie = FascicoliManager.GetFigliClassifica2(this, classifica, UserManager.getUtente(this).idAmministrazione, getIdTitolario(codClassifica));

                        txt_protoPratica.Text = classifica.numProtoTit;
                    }
                    else
                    {
                        classificheFiglie = null;
                    }
                }
                else
                {
                    classificheFiglie = null;
                }

                DropDownList ddlNext = elementByIndex(indexCombo + 1);
                if (ddlNext != null)
                {
                    initListaCombo(indexCombo + 1);
                    caricaComboClassifica(ddlNext, classificheFiglie);
                }
            }
        }

        private void setCodiceClassificaInTextBox(int indexSelectedCombo)
        {
            DropDownList ddlControl = elementByIndex(indexSelectedCombo);
            string codClassifica = "";

            if (ddlControl != null)
            {
                if (ddlControl.SelectedIndex >= 0)
                    codClassifica = ddlControl.SelectedItem.Value.ToString();
                if (codClassifica == "")
                {
                    if (indexSelectedCombo > 0)
                    {
                        DropDownList ddlPrevControl = elementByIndex(indexSelectedCombo - 1);
                        if (ddlPrevControl.SelectedIndex >= 0)
                            codClassifica = ddlPrevControl.SelectedItem.Value.ToString();
                    }
                    else
                    {
                        codClassifica = "";
                    }
                }

                this.txt_codClass.Text = codClassifica;
                Session["newClass"] = "S";
                //FascicoliManager.removeClassificazioneSelezionata(this);

            }
        }

        private int indexByElement(DropDownList ddlControl)
        {
            int retValue = -1;

            for (int i = 0; i < CONST_NUMBER_OF_DDL_CONTROLS; i++)
            {
                string ddlItemXName = "ddl_livello" + (i + 1).ToString();
                if (ddlControl.ID.ToString() == ddlItemXName.ToString())
                {
                    retValue = i;
                    break;
                }
            }
            return retValue;
        }

        private void initListaCombo(int indexToStart)
        {
            if (indexToStart >= 0 && indexToStart < CONST_NUMBER_OF_DDL_CONTROLS)
            {
                for (int i = indexToStart; i < CONST_NUMBER_OF_DDL_CONTROLS; i++)
                {
                    DropDownList ddlControl = (DropDownList)this.FindControl("ddl_livello" + (i + 1).ToString());
                    ddlControl.Items.Clear();
                }
            }

        }

        private void caricaComboClassifica(DropDownList ddlDaCaricare, DocsPaWR.FascicolazioneClassifica[] classifiche)
        {
            if (ddlDaCaricare != null && classifiche != null && classifiche.Length > 0)
            {
                string aaaa = ddlDaCaricare.ID;
                ddlDaCaricare.Items.Add("");
                ListItem newItem;
                for (int i = 0; i < classifiche.Length; i++)
                {
                    DocsPaWR.FascicolazioneClassifica classifica = classifiche[i];
                    if (viewCodiceInCombo)//se la chiave VIEW_CODICE_IN_COMBO_CLASSIFICA, sul web.config del WA , è settata a 1 
                    {
                        newItem = new ListItem(classifica.codice + " - " + classifica.descrizione, classifica.codice);
                    }
                    else
                    {
                        newItem = new ListItem(classifica.descrizione, classifica.codice);
                    }
                    ddlDaCaricare.Items.Add(newItem);
                }
            }
        }

        private DropDownList elementByIndex(int indexOfElement)
        {
            DropDownList retValue = null;

            {
                retValue = (DropDownList)this.FindControl("ddl_livello" + (indexOfElement + 1).ToString());
            }
            return retValue;
        }

        private void caricaCombo(int indexCombo, DocsPaWR.FascicolazioneClassifica[] classifiche)
        {
            DropDownList ddlCombo = elementByIndex(indexCombo);
            if (ddlCombo != null)
            {
                caricaComboClassifica(ddlCombo, classifiche);
            }
        }
        #endregion

        #region PerformAction
        /// <summary>
        /// Azione di selezione tipologia di ricerca
        /// (per livello o codice)
        /// </summary>
        private void PerformActionSelectSearchMode()
        {
            if (OptLst.SelectedItem.Value.Equals("Cod"))
            {
                EnabledisabledDDL(1, 6, false);
                txt_codClass.Enabled = true;
                txt_codClass.ReadOnly = false;
            }
            else
            {
                //Abilitare le liste.
                EnabledisabledDDL(1, 6, true);
                txt_codClass.Enabled = false;
                txt_codClass.ReadOnly = true;
            }
        }

        /// <summary>
        /// Azione di selezione elemento da dropdown data apertura
        /// </summary>
        private void PerformActionSelectDropDownDataApertura()
        {
            switch (this.ddl_dataA.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataA").Visible = true;
                    this.GetCalendarControl("txt_initDataA").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataA").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataA").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataA").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataA").Visible = false;
                    this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = false;
                    this.lbl_finedataA.Visible = false;
                    this.lbl_initdataA.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataA").Visible = true;
                    this.GetCalendarControl("txt_initDataA").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataA").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataA").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataA").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataA").Visible = true;
                    this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataA").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataA").txt_Data.Enabled = true;
                    this.lbl_finedataA.Visible = true;
                    this.lbl_initdataA.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataA").Visible = true;
                    this.GetCalendarControl("txt_initDataA").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataA").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataA").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataA").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataA").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataA").Visible = false;
                    this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = false;
                    this.lbl_finedataA.Visible = false;
                    this.lbl_initdataA.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataA").Visible = true;
                    this.GetCalendarControl("txt_initDataA").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataA").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataA").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataA").Visible = true;
                    this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataA").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataA").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataA").txt_Data.Enabled = false;
                    this.lbl_finedataA.Visible = true;
                    this.lbl_initdataA.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataA").Visible = true;
                    this.GetCalendarControl("txt_initDataA").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataA").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataA").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataA").Visible = true;
                    this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataA").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataA").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataA").txt_Data.Enabled = false;
                    this.lbl_finedataA.Visible = true;
                    this.lbl_initdataA.Visible = true;
                    break;
            }

            /* Commentato perchè selezionato di default l'intervallo
            this.GetCalendarControl("txt_fineDataA").txt_Data.Text = "";
            if (this.ddl_dataA.SelectedIndex == 0)
            {
                this.GetCalendarControl("txt_fineDataA").Visible = false;
                this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = false;
                this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = false;
                this.lbl_finedataA.Visible = false;
                this.lbl_initdataA.Visible = false;

            }
            else
            {
                this.GetCalendarControl("txt_fineDataA").Visible = true;
                this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = true;
                this.lbl_finedataA.Visible = true;
                this.lbl_initdataA.Visible = true;
            }
            this.GetCalendarControl("txt_fineDataA").Visible = true;
            this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = true;
            this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = true;
            this.lbl_finedataA.Visible = true;
            this.lbl_initdataA.Visible = true;
            */
        }

        private void ddl_data_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.PerformActionSelectDropDownDataApertura();
        }

        /// <summary>
        /// Azione di selezione elemento da dropdown data chiusura
        /// </summary>
        private void PerformActionSelectDropDownDataChiusura()
        {
            switch (this.ddl_dataC.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataC").Visible = true;
                    this.GetCalendarControl("txt_initDataC").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataC").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataC").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataC").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataC").Visible = false;
                    this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = false;
                    this.lbl_finedataC.Visible = false;
                    this.lbl_initdataC.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataC").Visible = true;
                    this.GetCalendarControl("txt_initDataC").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataC").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataC").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataC").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataC").Visible = true;
                    this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataC").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataC").txt_Data.Enabled = true;
                    this.lbl_finedataC.Visible = true;
                    this.lbl_initdataC.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataC").Visible = true;
                    this.GetCalendarControl("txt_initDataC").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataC").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataC").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataC").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataC").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataC").Visible = false;
                    this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = false;
                    this.lbl_finedataC.Visible = false;
                    this.lbl_initdataC.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataC").Visible = true;
                    this.GetCalendarControl("txt_initDataC").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataC").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataC").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataC").Visible = true;
                    this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataC").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataC").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataC").txt_Data.Enabled = false;
                    this.lbl_finedataC.Visible = true;
                    this.lbl_initdataC.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataC").Visible = true;
                    this.GetCalendarControl("txt_initDataC").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataC").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataC").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataC").Visible = true;
                    this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataC").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataC").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataC").txt_Data.Enabled = false;
                    this.lbl_finedataC.Visible = true;
                    this.lbl_initdataC.Visible = true;
                    break;
            }
            /* Commentato perchè selezionato di default l'intervallo
            this.GetCalendarControl("txt_fineDataC").txt_Data.Text = "";
            if (this.ddl_dataC.SelectedIndex == 0)
            {
                this.GetCalendarControl("txt_fineDataC").Visible = false;
                this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = false;
                this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = false;
                this.lbl_finedataC.Visible = false;
                this.lbl_initdataC.Visible = false;

            }
            else
            {
                this.GetCalendarControl("txt_fineDataC").Visible = true;
                this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = true;
                this.lbl_finedataC.Visible = true;
                this.lbl_initdataC.Visible = true;
            }
            this.GetCalendarControl("txt_fineDataC").Visible = true;
            this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = true;
            this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = true;
            this.lbl_finedataC.Visible = true;
            this.lbl_initdataC.Visible = true;
            */
        }

        private void ddl_dataC_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.PerformActionSelectDropDownDataChiusura();
        }

        /// <summary>
        /// Azione di selezione elemento da dropdown data creazione
        /// </summary>
        private void PerformActionSelectDropDownDataCreazione()
        {
            switch (this.ddl_creaz.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataCrea").Visible = true;
                    this.GetCalendarControl("txt_initDataCrea").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataCrea").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataCrea").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataCrea").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataCrea").Visible = false;
                    this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = false;
                    this.lbl_finCreaz.Visible = false;
                    this.lbl_initCreaz.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataCrea").Visible = true;
                    this.GetCalendarControl("txt_initDataCrea").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataCrea").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataCrea").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataCrea").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataCrea").Visible = true;
                    this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataCrea").txt_Data.Enabled = true;
                    this.lbl_finCreaz.Visible = true;
                    this.lbl_initCreaz.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataCrea").Visible = true;
                    this.GetCalendarControl("txt_initDataCrea").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataCrea").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataCrea").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataCrea").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataCrea").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCrea").Visible = false;
                    this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = false;
                    this.lbl_finCreaz.Visible = false;
                    this.lbl_initCreaz.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataCrea").Visible = true;
                    this.GetCalendarControl("txt_initDataCrea").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataCrea").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataCrea").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCrea").Visible = true;
                    this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataCrea").txt_Data.Enabled = false;
                    this.lbl_finCreaz.Visible = true;
                    this.lbl_initCreaz.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataCrea").Visible = true;
                    this.GetCalendarControl("txt_initDataCrea").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataCrea").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataCrea").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCrea").Visible = true;
                    this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataCrea").txt_Data.Enabled = false;
                    this.lbl_finCreaz.Visible = true;
                    this.lbl_initCreaz.Visible = true;
                    break;
            }
            /* Commentato perchè selezionato di default l'intervallo
            this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text = "";
            if (this.ddl_creaz.SelectedIndex == 0)
            {
                this.GetCalendarControl("txt_fineDataCrea").Visible = false;
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = false;
                this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = false;
                this.lbl_finCreaz.Visible = false;
                this.lbl_initCreaz.Visible = false;

            }
            else
            {
                this.GetCalendarControl("txt_fineDataCrea").Visible = true;
                this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = true;
                this.lbl_finCreaz.Visible = true;
                this.lbl_initCreaz.Visible = true;
            }
            this.GetCalendarControl("txt_fineDataCrea").Visible = true;
            this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = true;
            this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = true;
            this.lbl_finCreaz.Visible = true;
            this.lbl_initCreaz.Visible = true;
            */
        }

        private void ddl_creaz_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.PerformActionSelectDropDownDataCreazione();
        }

        /// <summary>
        /// Azione di selezione elemento da dropdown data scadenza
        /// </summary>
        private void PerformActionSelectDropDownDataScadenza()
        {
            switch (this.ddl_dataScadenza.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_dataScadenza_DA").Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_DA").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_DA").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_dataScadenza_A").Visible = false;
                    this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Visible = false;
                    this.lbl_dataScadenza_A.Visible = false;
                    this.lbl_dataScadenza_DA.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_dataScadenza_DA").Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_DA").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_DA").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_dataScadenza_A").Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Enabled = true;
                    this.lbl_dataScadenza_A.Visible = true;
                    this.lbl_dataScadenza_DA.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_dataScadenza_DA").Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_DA").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_DA").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_dataScadenza_A").Visible = false;
                    this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Visible = false;
                    this.lbl_dataScadenza_A.Visible = false;
                    this.lbl_dataScadenza_DA.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_dataScadenza_DA").Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_DA").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_dataScadenza_A").Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Enabled = false;
                    this.lbl_dataScadenza_A.Visible = true;
                    this.lbl_dataScadenza_DA.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_dataScadenza_DA").Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_DA").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_dataScadenza_A").Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Enabled = false;
                    this.lbl_dataScadenza_A.Visible = true;
                    this.lbl_dataScadenza_DA.Visible = true;
                    break;
            }
            /* Commentato perchè selezionato di default l'intervallo
         if (ddl_dataScadenza.SelectedIndex == 0)
         {
             this.GetCalendarControl("txt_dataScadenza_A").Visible = false;
             this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Visible = false;
             this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Visible = false;
             this.GetCalendarControl("txt_dataScadenza_DA").Visible = true;
             this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Visible = true;
             this.GetCalendarControl("txt_dataScadenza_DA").btn_Cal.Visible = true;
             this.lbl_dataScadenza_DA.Visible = false;
             this.lbl_dataScadenza_A.Visible = false;
         }
         else
         {
             this.GetCalendarControl("txt_dataScadenza_A").Visible = true;
             this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Visible = true;
             this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Visible = true;
             this.GetCalendarControl("txt_dataScadenza_DA").Visible = true;
             this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Visible = true;
             this.GetCalendarControl("txt_dataScadenza_DA").btn_Cal.Visible = true;
             this.lbl_dataScadenza_DA.Visible = true;
             this.lbl_dataScadenza_A.Visible = true;
         }
            this.GetCalendarControl("txt_dataScadenza_A").Visible = true;
            this.GetCalendarControl("txt_dataScadenza_A").btn_Cal.Visible = true;
            this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Visible = true;
            this.GetCalendarControl("txt_dataScadenza_DA").Visible = true;
            this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Visible = true;
            this.GetCalendarControl("txt_dataScadenza_DA").btn_Cal.Visible = true;
            this.lbl_dataScadenza_DA.Visible = true;
            this.lbl_dataScadenza_A.Visible = true;
         */
        }

        private void ddl_dataScadenza_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.PerformActionSelectDropDownDataScadenza();
        }

        /// <summary>
        /// Azione di selezione di una tipologia di fascicolo
        /// </summary>
        private void PerformActionSelectTipoFascicolo()
        {
            if (this.ddlTipo.SelectedIndex == 1)
            {
                this.ddlStato.SelectedIndex = 0;
                this.ddlStato.Enabled = false;
                this.txtNumFasc.Text = "";
                this.txtNumFasc.ReadOnly = true;
                this.txtNumFasc.BackColor = Color.WhiteSmoke;
                this.txtAnnoFasc.Text = "";
                this.txtAnnoFasc.ReadOnly = true;
                this.txtAnnoFasc.BackColor = Color.WhiteSmoke;
                //data creazione non abilitata
                this.ddl_creaz.Enabled = false;
                this.lbl_initCreaz.Visible = true;
                this.GetCalendarControl("txt_initDataCrea").txt_Data.BackColor = Color.WhiteSmoke;
                this.GetCalendarControl("txt_initDataCrea").txt_Data.Text = "";
                //
                this.lbl_finCreaz.Visible = true;
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text = "";
                this.GetCalendarControl("txt_fineDataCrea").Visible = true;
                this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = false;
                this.lbl_finCreaz.Visible = false;
                this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = false;
                this.ddl_creaz.SelectedIndex = 0;
                if (enableUfficioRef)//PER I FASCICOLI GENERALI LA RICERCA NN è ABILITATA
                {
                    this.txt_desc_uffRef.Text = "";
                    this.txt_cod_UffRef.Text = "";
                    this.txt_cod_UffRef.ReadOnly = true;
                    this.txt_cod_UffRef.BackColor = Color.WhiteSmoke;
                    this.txt_desc_uffRef.BackColor = Color.WhiteSmoke;
                    this.btn_rubricaRef.Enabled = false;
                }
            }
            else
            {

                this.ddlStato.Enabled = true;
                this.txtNumFasc.ReadOnly = false;
                this.txtNumFasc.BackColor = Color.White;
                this.txtAnnoFasc.ReadOnly = false;
                this.txtAnnoFasc.BackColor = Color.White;
                //data creazione abilitata
                this.ddl_creaz.Enabled = true;
                this.lbl_initCreaz.Visible = true;
                this.GetCalendarControl("txt_initDataCrea").txt_Data.BackColor = Color.White;
                if (enableUfficioRef)//PER I FASCICOLI procedimentali LA RICERCA è ABILITATA
                {
                    this.txt_desc_uffRef.Text = "";
                    this.txt_cod_UffRef.Text = "";
                    this.txt_cod_UffRef.ReadOnly = false;
                    this.txt_cod_UffRef.BackColor = Color.White;
                    this.txt_desc_uffRef.BackColor = Color.White;
                    this.btn_rubricaRef.Enabled = true;
                }
            }
        }

        private void ddlTipo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Session.Remove("rubrica.campoCorrispondente");
            Session.Remove("CorrSelezionatoDaMulti");

            // Selezione tipologia fascicolo
            this.PerformActionSelectTipoFascicolo();
            this.ddlStato.Enabled = true;
            if (UserManager.ruoloIsAutorized(this, "GEST_ARCHIVIA") && ddlTipo.SelectedIndex==2)
            {
                this.ddlStato.SelectedIndex = 2;
                this.ddlStato.Enabled = false;
            }
            
        }

        /// <summary>
        /// Azione di selezione data locazione fisica
        /// </summary>
        private void PerformActionSelectDataLF()
        {
            switch (this.ddl_data_LF.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_dta_LF_DA").Visible = true;
                    this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_dta_LF_A").Visible = false;
                    this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = false;
                    this.lbl_dta_LF_A.Visible = false;
                    this.lbl_dta_LF_DA.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_dta_LF_DA").Visible = true;
                    this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_dta_LF_A").Visible = true;
                    this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dta_LF_A").txt_Data.Enabled = true;
                    this.lbl_dta_LF_A.Visible = true;
                    this.lbl_dta_LF_DA.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_dta_LF_DA").Visible = true;
                    this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_dta_LF_A").Visible = false;
                    this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = false;
                    this.lbl_dta_LF_A.Visible = false;
                    this.lbl_dta_LF_DA.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_dta_LF_DA").Visible = true;
                    this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_dta_LF_A").Visible = true;
                    this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dta_LF_A").txt_Data.Enabled = false;
                    this.lbl_dta_LF_A.Visible = true;
                    this.lbl_dta_LF_DA.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_dta_LF_DA").Visible = true;
                    this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_dta_LF_A").Visible = true;
                    this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dta_LF_A").txt_Data.Enabled = false;
                    this.lbl_dta_LF_A.Visible = true;
                    this.lbl_dta_LF_DA.Visible = true;
                    break;
            }
            /* Commentato perchè selezionato di default l'intervallo
            if (ddl_data_LF.SelectedIndex == 0)
            {
                this.GetCalendarControl("txt_dta_LF_A").Visible = false;
                this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = false;
                this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = false;
                this.GetCalendarControl("txt_dta_LF_DA").Visible = true;
                this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Visible = true;
                this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Visible = true;
                this.lbl_dta_LF_DA.Visible = false;
                this.lbl_dta_LF_A.Visible = false;
            }
            else
            {
                this.GetCalendarControl("txt_dta_LF_A").Visible = true;
                this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = true;
                this.GetCalendarControl("txt_dta_LF_DA").Visible = true;
                this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Visible = true;
                this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Visible = true;
                this.lbl_dta_LF_DA.Visible = true;
                this.lbl_dta_LF_A.Visible = true;
            }
            this.GetCalendarControl("txt_dta_LF_A").Visible = true;
            this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = true;
            this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = true;
            this.GetCalendarControl("txt_dta_LF_DA").Visible = true;
            this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Visible = true;
            this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Visible = true;
            this.lbl_dta_LF_DA.Visible = true;
            this.lbl_dta_LF_A.Visible = true;
             */
        }

        private void ddl_data_LF_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.PerformActionSelectDataLF();
        }


        #endregion

        #region Set Corrispondente - Set Ufficio referente
        private void txt_varCodRubrica_LF_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                setDescCorrispondente(this.txt_varCodRubrica_LF.Text, false);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void setDescCorrispondente(string codiceRubrica, bool fineValidita)
        {
            string msg = "Codice rubrica non esistente";
            DocsPaWR.Corrispondente corr = null;
            try
            {
                if (!codiceRubrica.Equals(""))
                {
                    corr = UserManager.GetCorrispondenteInterno(this, codiceRubrica, fineValidita);
                }

                if ((corr != null && corr.descrizione != "") && corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa)))
                {
                    txt_descr_LF.Text = corr.descrizione;
                    this.hd_systemIdLF.Value = corr.systemId;

                }
                else
                {
                    if (!codiceRubrica.Equals(""))
                    {
                        RegisterStartupScript("alert", "<script language='javascript'>alert('" + msg + "');</script>");
                    }
                    this.hd_systemIdLF.Value = "";
                    txt_descr_LF.Text = "";
                }

            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void setDescUffRef(string codiceRubrica)
        {
            DocsPaWR.Corrispondente corr = null;
            string msg = "Codice rubrica non valido per l\\'Ufficio referente!";
            if (!codiceRubrica.Equals(""))
            {
                corr = UserManager.getCorrispondenteReferente(this, codiceRubrica, false);
            }
            if (corr != null && (corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))))
            {
                this.txt_desc_uffRef.Text = UserManager.getDecrizioneCorrispondenteSemplice(corr);
                this.hd_systemIdUffRef.Value = corr.systemId;
            }
            else
            {
                if (!codiceRubrica.Equals(""))
                {
                    RegisterStartupScript("alert", "<script language='javascript'>alert('" + msg + "');</script>");
                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_cod_UffRef.ID + "').focus() </SCRIPT>";
                    RegisterStartupScript("focus", s);
                }
                //this.txt_cod_UffRef.Text = "";
                this.txt_desc_uffRef.Text = "";
                this.hd_systemIdUffRef.Value = "";
            }
        }

        private void txt_cod_UffRef_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                setDescUffRef(this.txt_cod_UffRef.Text);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }
        #endregion

        #region Eventi Vari
        private void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            #region NEW

            // quando cambio registro devo:
            // - svuotare le combo;
            // - pulire il campo di testo relativo al codice selezionato per l'altro registro;
            // - rimuovere la classificazione selezionata in precedenza.
            // - disabilita il tasto Nuovo
            initListaCombo(0);
            this.txt_codClass.Text = "";
            FascicoliManager.removeClassificazioneSelezionata(this);
            impostaAbilitazioneNuovoFascNuovoTit();
            //this.btn_new.Enabled = false;
            //this.btn_new_tit.Enabled = false;

            // - mettere in sessione il registro selezionato
            settaRegistroSelezionato();

            // - caricare le combobox con il titolario associato alla nuova selezione
            buildParametriPagina();

            #endregion

            #region OLD

            //				settaRegistroSelezionato();
            //				caricaTitolariRegistro(true);

            #endregion
        }

        private void DDLFiltro1_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }

        private void Textbox1_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void txt_RicTit_TextChanged(object sender, System.EventArgs e)
        {
            FascicoliManager.removeClassificazioneSelezionata(this);
            Session["flagGetFigli"] = "N";
        }

        private void addChiamateJScriptAiComandi()
        {
            //per i controlli HTML lato client, aggiunge le chiamate ai gestori di evento javascript
            #region btn_titolario
            //String pageName=Request.Url.Segments[Request.Url.Segments.Length-1].ToString();	
            string queryString = "codClass='+document.fascicoli_sx.txt_codClass.value+'";
            //this.btn_titolario.Attributes.Add("onclick","return btn_titolario_onClick('"+pageName+"','"+queryString+"');");						
            this.btn_titolario.Attributes.Add("onclick", "return btn_titolario_onClick('" + queryString + "');");
            #endregion

            #region txt_codClass
            //this.txt_codClass.Attributes.Add("onblur","__doPostBack('txt_codClass','')");
            #endregion
        }

        private void txt_codClass_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (cercaClassificazioneDaCodice())
                {
                    Session["newClass"] = "S";
                    FascicoliManager.removeClassificazioneSelezionata(this);
                }
                else
                {
                    if (!this.txt_codClass.Text.Equals(""))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Attenzione: codice classifica non presente!');", true);
                        string s = "document.getElementById('" + txt_codClass.ID + "').focus();";
                        ClientScript.RegisterStartupScript(this.GetType(), "focus", s, true);
                        ddl_titolari.SelectedIndex = 0;
                    }
                    //this.btn_new.Enabled = false;
                    //this.btn_new_tit.Enabled = false;
                }
                impostaAbilitazioneNuovoFascNuovoTit();
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void btn_titolario_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //E' necessario che sia selezionato un titolario e non la voce "tutti i titolari"
            if (ddl_titolari.Enabled && ddl_titolari.SelectedIndex == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "selezionareUnTitolario", "alert('Selezionare un titolario.');", true);
                return;
            }

            Session["Titolario"] = "Y";
            if (!this.IsStartupScriptRegistered("apriModalDialog"))
            {
                //string scriptString = "<SCRIPT>ApriTitolario('codClass=" + txt_codClass.Text + "&idTit=" + ddl_titolari.SelectedValue + "','gestFasc')</SCRIPT>";
                string scriptString = "<SCRIPT>ApriTitolario('codClass=" + txt_codClass.Text + "&idTit=" + getIdTitolario(txt_codClass.Text) + "','gestFasc')</SCRIPT>";
                this.RegisterStartupScript("apriModalDialog", scriptString);
            }
        }

        private void ButtonRicercaClassDaProt_Click(object sender, System.EventArgs e)
        {
            try
            {
                bool res = false;
                res = cercaClassificazioneDaCodice();
                if (!res)
                {
                    if (!this.txt_codClass.Text.Equals(""))
                    {
                        string s2 = "<script>alert('Attenzione: codice classifica non presente!');</script>";
                        if (!IsStartupScriptRegistered("NoCod"))
                            Page.RegisterStartupScript("NoCod", s2);
                        //Page.RegisterStartupScript("","<script>alert('Attenzione: codice classifica non presente!');top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_codClass.ID + "').focus() </SCRIPT>";
                        RegisterStartupScript("focus", s);
                        impostaAbilitazioneNuovoFascNuovoTit();
                        ddl_titolari.SelectedIndex = 0;
                        //btn_new.Enabled = false;
                        //this.btn_new_tit.Enabled = false;
                    }
                    else
                    {
                        //se il campo codice è vuoto disabilito il PULSANTE NUOVO
                        impostaAbilitazioneNuovoFascNuovoTit();
                        //btn_new.Enabled = false;
                        //this.btn_new_tit.Enabled = false;
                    }
                    //return;
                }
                Session["newClass"] = "S";
                //btn_ricFascicoli.Enabled = true;
                if (res)
                {
                    impostaAbilitazioneNuovoFascNuovoTit();
                    //btn_new.Enabled = true;
                    //this.btn_new_tit.Enabled = true;
                }
                FascicoliManager.removeClassificazioneSelezionata(this);
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        /// <summary>
        /// Metodo per il recupero del fascicolo da codice
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo[] getFascicolo(DocsPAWA.DocsPaWR.Registro registro, string codClassificazione)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            listaFasc = FascicoliManager.getListaFascicoliDaCodice(this, codClassificazione, registro, "R");
            if (listaFasc != null)
                return listaFasc;
            else
                return null;
        }

        //Questo metodo serve a verificare la ricerca tramite codice classifica i casi sono i seguenti :
        //1. Si proviene proviene da un "back" (tasto Back)si effettua la ricerca
        //2. Si proviene o meno da una selezione di un titolario, si ripristina o no il filtro ricerca per idTitolario
        //3. Selezione "Tutti i titolari" il codice restituisce un solo nodo OK si effettua la ricerca
        //4. Selezione <<uno specifico titolario>> OK si effettua la ricerca
        //5. Selezione "Tutti i titolari" il codice restituisce piu' di un nodo NO la ricerca viene bloccata e si chiede la selezione di un titolario
        private string checkRicercaFasc(string codClassificazione)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(codClassificazione)){
                result = "SI_RICERCA";
                //DocsPaWR.Fascicolo[] listaFasc = getFascicolo(UserManager.getRegistroSelezionato(this), codClassificazione);

                if (!this.IsPostBack && this.OnBack)
                    return result;

                if (Session["idTitolarioSelezionato"] == null)
                {
                    DocsPaWR.FiltroRicerca item = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    item.argomento = DocsPaWR.FiltriFascicolazione.ID_TITOLARIO.ToString();
                    item.valore = ddl_titolari.SelectedValue;
                    RestoreFiltersIdTitolario(item);
                }

                if (ddl_titolari.SelectedItem.Text == "Tutti i titolari" && this.numFasc > 1)
                    result = "NO_RICERCA";

                if (ddl_titolari.SelectedItem.Text == "Tutti i titolari" && this.numFasc == 1)
                    result = "SI_RICERCA";
             }
            return result;
        }

        private string getIdTitolario(string codClassificazione)
        {
            if (codClassificazione != null && codClassificazione != "")
            {
                //DocsPaWR.Fascicolo[] listaFasc = getFascicolo(UserManager.getRegistroSelezionato(this), codClassificazione);

                //In questo caso il metodo "GetFigliClassifica2" funzionerebbe male
                //per questo viene restituti l'idTitolario dell'unico fascicolo risolto
                if (ddl_titolari.SelectedItem.Text == "Tutti i titolari" && this.listaFasc.Length == 1)
                {
                    DocsPaWR.Fascicolo fasc = (DocsPaWR.Fascicolo)this.listaFasc[0];
                    return fasc.idTitolario;
                }
            }

            //In tutti gli altri casi è sufficiente restituire il value degli item della
            //ddl_Titolario in quanto formati secondo le specifiche di uno o piu' titolari
            return ddl_titolari.SelectedValue;
        }

        #endregion

        protected void txt_protoPratica_TextChanged(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txt_protoPratica.Text))
            {
                ArrayList listaNodiTitolario = new ArrayList(wws.getNodiFromProtoTit(UserManager.getRegistroSelezionato(this), UserManager.getUtente(this).idAmministrazione, txt_protoPratica.Text, ddl_titolari.SelectedValue));

                //Nessun nodo trovato
                if (listaNodiTitolario.Count == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "nessunNodoTrovato", "alert('Nessun nodo di titolario corrispondente al numero inserito.');", true);
                }

                //Trovato un solo nodo si procede alla valorizzazione delle combo
                if (listaNodiTitolario.Count == 1)
                {
                    this.txt_codClass.Text = ((DocsPAWA.DocsPaWR.OrgNodoTitolario)listaNodiTitolario[0]).Codice;
                    this.ddl_titolari.SelectedValue = ((DocsPAWA.DocsPaWR.OrgNodoTitolario)listaNodiTitolario[0]).ID_Titolario;
                    if (cercaClassificazioneDaCodice())
                    {
                        Session["newClass"] = "S";
                        FascicoliManager.removeClassificazioneSelezionata(this);
                    }                       
                }

                //Trovato più di un nodo si procede alla richiesta di scelta
                if (listaNodiTitolario.Count > 1)
                {
                    string queryString = this.Server.UrlEncode("indice=" + txt_protoPratica.Text + "&idTitolario=" + ddl_titolari.SelectedValue + "&TipoChiamata=ProtocolloTitolario");
                    ClientScript.RegisterStartupScript(this.GetType(), "apriSceltaNodo", "ApriSceltaNodo('" + queryString + "');", true); 
                }
            }            
        }

        protected void ModifyRapidSearch_Click(object sender, EventArgs e)
        {
            if (ricercaFascicoli())
            {
                // Impostazione del filtro utilizzato
                GridManager.SetSearchFilter(this.ddlOrder.SelectedItem.Text, this.ddlOrderDirection.SelectedValue);

                schedaRicerca.FiltriRicerca = qV;
                schedaRicerca.ProprietaNuovaRicerca = new DocsPAWA.ricercaDoc.SchedaRicerca.NuovaRicerca();
                if (this.ddl_Ric_Salvate.SelectedIndex > 0)
                {
                    string idRicercaSalvata = this.ddl_Ric_Salvate.SelectedItem.Value.ToString();
                    ClientScript.RegisterStartupScript(this.GetType(), "modificaRicerca", "apriModificaRicerca(" + idRicercaSalvata + ");", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alertModifica", "alert(Selezionare una ricerca salvata da modificare);", true);
                }
            }
        }

        protected void btnCleanUpField_Click(object sender, EventArgs e)
        {
            schedaRicerca = null;
            schedaRicerca = new DocsPAWA.ricercaDoc.SchedaRicerca(KEY_SCHEDA_RICERCA, userHome, userRuolo, this);
            Session[DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY] = schedaRicerca;
            schedaRicerca.Pagina = this;

            this.ddl_dataA.SelectedIndex = 0;
            this.GetCalendarControl("txt_initDataA").txt_Data.Text = "";
            this.GetCalendarControl("txt_fineDataA").txt_Data.Text = "";
            this.GetCalendarControl("txt_fineDataA").Visible = false;
            this.lbl_finedataA.Visible = false;

            this.ddl_dataC.SelectedIndex = 0;
            this.GetCalendarControl("txt_initDataC").txt_Data.Text = "";
            this.GetCalendarControl("txt_fineDataC").txt_Data.Text = "";
            this.GetCalendarControl("txt_fineDataC").Visible = false;
            this.lbl_finedataC.Visible = false;

            this.ddl_creaz.SelectedIndex = 0;
            this.GetCalendarControl("txt_initDataCrea").txt_Data.Text = "";
            this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text = "";
            this.GetCalendarControl("txt_fineDataCrea").Visible = false;
            this.lbl_finCreaz.Visible = false;
            this.ddl_creaz.Enabled = true;
            this.GetCalendarControl("txt_initDataCrea").txt_Data.Enabled = true;

            this.txtNumFasc.Text = string.Empty;
            this.txtAnnoFasc.Text = string.Empty;
            this.txtNumFasc.Enabled = true;
            this.txtAnnoFasc.Enabled = true;

            this.ddlStato.SelectedIndex = 0;
            this.ddlTipo.SelectedIndex = 0;
            this.txtDescr.Text = string.Empty;
            this.rn_note.Testo = string.Empty;
            this.rn_note.TipoRicerca = 'Q';

            this.ddl_dataScadenza.SelectedIndex = 0;
            this.GetCalendarControl("txt_dataScadenza_DA").txt_Data.Text = "";
            this.GetCalendarControl("txt_dataScadenza_A").txt_Data.Text = "";
            this.GetCalendarControl("txt_dataScadenza_A").Visible = false;
            this.lbl_dataScadenza_A.Visible = false;

            this.OptLst.SelectedValue = "Cod";
            this.ddl_titolari.SelectedIndex = 0;
            this.ddl_livello1.ClearSelection();
            this.ddl_livello1.Enabled = false;
            this.ddl_livello2.ClearSelection();
            this.ddl_livello2.Enabled = false;
            this.ddl_livello3.ClearSelection();
            this.ddl_livello3.Enabled = false;
            this.ddl_livello4.ClearSelection();
            this.ddl_livello4.Enabled = false;
            this.ddl_livello5.ClearSelection();
            this.ddl_livello5.Enabled = false;
            this.ddl_livello6.ClearSelection();
            this.ddl_livello6.Enabled = false;

            this.txt_codClass.Text = string.Empty;

            this.ddl_data_LF.SelectedIndex = 0;
            this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text = "";
            this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text = "";
            this.GetCalendarControl("txt_dta_LF_A").Visible = false;
            this.lbl_dta_LF_A.Visible = false;

            this.txt_varCodRubrica_LF.Text = string.Empty;
            this.txt_descr_LF.Text = string.Empty;

            if (pnl_uffRef.Visible)
            {
                this.txt_cod_UffRef.Text = string.Empty;
                this.txt_desc_uffRef.Text = string.Empty;
            }

            if (pnl_profilazione.Visible)
            {
                this.ddl_tipoFasc.SelectedIndex = 0;
            }

            if (pnl_Sottofascicoli.Visible)
            {
                this.txt_sottofascicolo.Text = string.Empty;
            }

            //this.optListTipiCreatore.SelectedValue = "R";
            //this.cbx_UOsotto.Checked = false;
            //this.txtCodiceUtenteCreatore.Text = string.Empty;
            //txtDescrizioneUtenteCreatore.Text = string.Empty;

            this.cb_Conservato.Checked = false;
            this.cb_NonConservato.Checked = false;

            if (Panel_StatiDocumento.Visible)
            {
                this.ddl_statiDoc.SelectedIndex = 0;
            }

            this.rbl_MostraTutti.SelectedValue = "N";

            if (img_dettagliProfilazione.Visible)
            {
                img_dettagliProfilazione.Visible = false;
                Session.Remove("TemplateRicerca");
                Session.Remove("filtroProfDinamica");
            }

            GridManager.CompileDdlOrderAndSetOrderFilterProjects(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);


        }

    }
}
