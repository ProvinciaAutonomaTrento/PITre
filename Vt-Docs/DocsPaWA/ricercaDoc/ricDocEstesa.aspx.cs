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
using System.Text.RegularExpressions;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;
using System.Collections.Generic;
using System.Linq;


namespace DocsPAWA.ricercaDoc
{
    /// <summary>
    /// Summary description for ricDocEstesa.
    /// </summary>
    public class f_Ricerca_E : DocsPAWA.CssPage
    {

        #region variabili codice
        protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
        protected DocsPAWA.DocsPaWR.Utente userHome;
        protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
        protected System.Web.UI.HtmlControls.HtmlForm f_Ricerca;
        #endregion

        protected DocsPAWA.DocsPaWR.InfoUtente Safe;
        protected System.Web.UI.WebControls.Label lblSearch;
        protected System.Web.UI.WebControls.ImageButton enterKeySimulator;
        protected System.Web.UI.WebControls.ImageButton btn_Canc_Ric;
        //protected System.Web.UI.WebControls.RadioButtonList rb_archDoc_E;
        protected System.Web.UI.WebControls.ListBox lb_reg_E;
        protected System.Web.UI.WebControls.RadioButtonList rbl_Reg_E;
        protected System.Web.UI.WebControls.DropDownList ddl_numProt_E;
        protected System.Web.UI.WebControls.Label lblDAnumprot_E;
        protected System.Web.UI.WebControls.TextBox txt_initNumProt_E;
        protected System.Web.UI.WebControls.Label lblAnumprot_E;
        protected System.Web.UI.WebControls.TextBox txt_fineNumProt_E;
        protected System.Web.UI.WebControls.TextBox tbAnnoProtocollo;
        protected System.Web.UI.WebControls.DropDownList ddl_dataProt_E;
        protected System.Web.UI.WebControls.DropDownList ddl_dataScadenza_E;
        protected System.Web.UI.WebControls.Label lbl_initDataScadenza_E;
        protected System.Web.UI.WebControls.Label lbl_fineDataScadenza_E;
        // protected DocsPaWebCtrlLibrary.DateMask txt_initDataScadenza_E;
        protected DocsPAWA.UserControls.Calendar txt_initDataScadenza_E;
        // protected DocsPaWebCtrlLibrary.DateMask txt_fineDataScadenza_E;
        protected DocsPAWA.UserControls.Calendar txt_fineDataScadenza_E;
        protected System.Web.UI.WebControls.Label lbl_initdataProt_E;
        // protected DocsPaWebCtrlLibrary.DateMask txt_initDataProt_E;
        protected DocsPAWA.UserControls.Calendar txt_initDataProt_E;
        protected System.Web.UI.WebControls.Label lbl_finedataProt_E;

        // protected DocsPaWebCtrlLibrary.DateMask txt_fineDataProt_E;
        protected DocsPAWA.UserControls.Calendar txt_fineDataProt_E;
        protected System.Web.UI.WebControls.ImageButton btn_RubrOgget_E;
        protected System.Web.UI.WebControls.TextBox txt_oggetto;
        protected System.Web.UI.WebControls.TextBox txt_codMit_E;
        protected System.Web.UI.WebControls.TextBox txt_descrMit_E;
        protected System.Web.UI.WebControls.Panel pnl_fasc_rapida;
        protected System.Web.UI.WebControls.TextBox txt_CodFascicolo;
        protected System.Web.UI.WebControls.TextBox txt_DescFascicolo;
        protected System.Web.UI.WebControls.Panel panel_numOgg_commRef;
        protected System.Web.UI.WebControls.TextBox txt_numOggetto;
        protected System.Web.UI.WebControls.RadioButtonList rbl_Rif_E;
        protected System.Web.UI.HtmlControls.HtmlImage btn_Rubrica_E;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_systemIdMit_Est;
        protected System.Web.UI.WebControls.Button btn_salva;
        protected System.Web.UI.WebControls.Button btn_Ricerca;
        protected System.Web.UI.WebControls.DropDownList ddl_Ric_Salvate;
        private bool isSavedSearch = false;
        private bool isLimited = false;
        protected System.Web.UI.WebControls.Panel pnlBottom;
        protected System.Web.UI.WebControls.Panel pnlTop;
        protected System.Web.UI.WebControls.Panel pnlLimitata;
        protected System.Web.UI.WebControls.Panel pnlBlank;
        protected System.Web.UI.WebControls.Panel pnlRicSalvate;
        protected System.Web.UI.WebControls.Panel pnl_dataProt;
        protected System.Web.UI.WebControls.Panel pnl_dataScad;
        protected System.Web.UI.WebControls.Label star;
        protected System.Web.UI.WebControls.Label star1;
        protected System.Web.UI.WebControls.DropDownList ddl_registri;
        //protected System.Web.UI.WebControls.Label lblNumProto;
        protected System.Web.UI.HtmlControls.HtmlTableRow trNumProto;
        private const string KEY_SCHEDA_RICERCA = "RicercaDocEstesa";
        protected DocsPaWebCtrlLibrary.ImageButton imgFasc;
        protected string codClassifica = "";
        //protected UserControls.Creatore Creatore;
        protected System.Web.UI.WebControls.CheckBoxList cbl_archDoc_E;
        //protected DocsPAWA.UserControls.Calendar txt_initDataStampa;
        //protected System.Web.UI.WebControls.Label lbl_initDataStampa;
        //protected System.Web.UI.WebControls.Label lbl_fineDataStampa;
        //protected DocsPAWA.UserControls.Calendar txt_fineDataStampa;
        //protected System.Web.UI.WebControls.DropDownList ddl_dataStampa;
        protected System.Web.UI.WebControls.DropDownList ddl_idDocumento_C;
        protected System.Web.UI.WebControls.TextBox txt_initIdDoc_C;
        protected System.Web.UI.WebControls.TextBox txt_fineIdDoc_C;
        protected System.Web.UI.WebControls.Label lblAidDoc_C;
        protected System.Web.UI.WebControls.Label lblDAidDoc_C;
        protected DocsPAWA.UserControls.Calendar txt_initDataCreazione_E;
        protected DocsPAWA.UserControls.Calendar txt_finedataCreazione_E;
        protected System.Web.UI.WebControls.Label lbl_dataCreazioneDa;
        protected System.Web.UI.WebControls.Label lbl_dataCreazioneA;
        protected System.Web.UI.WebControls.DropDownList ddl_dataCreazione_E;
        protected System.Web.UI.WebControls.CheckBox chk_mitt_dest_storicizzati;
        protected DocsPAWA.UserControls.Calendar txt_initDataStampa_E;
        protected DocsPAWA.UserControls.Calendar txt_finedataStampa_E;
        protected System.Web.UI.WebControls.Label lbl_dataStampaDa;
        protected System.Web.UI.WebControls.Label lbl_dataStampaA;
        protected System.Web.UI.WebControls.DropDownList ddl_dataStampa_E;

        protected System.Web.UI.WebControls.ListItem opArr;
        protected System.Web.UI.WebControls.ListItem opPart;
        protected System.Web.UI.WebControls.ListItem opInt;
        protected System.Web.UI.WebControls.ListItem opGrigio;
        protected System.Web.UI.WebControls.ListItem opAll;
        protected DocsPAWA.DocsPaWR.EtichettaInfo[] etichette;

        #region per memorizzare i registri selezionati
        protected DocsPAWA.DocsPaWR.Registro[] listaReg;
        #endregion
        private DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
        public SchedaRicerca schedaRicerca = null;
        protected Utilities.MessageBox mb_ConfirmDelete;
        protected System.Web.UI.WebControls.TextBox txt_codDesc;
        protected System.Web.UI.WebControls.Label l_amm_interop;

        protected System.Web.UI.WebControls.Panel p_cod_amm;

        protected System.Web.UI.HtmlControls.HtmlInputControl clTesto;
        protected int caratteriDisponibili = 2000;

        protected System.Web.UI.WebControls.DropDownList ddlOrder, ddlOrderDirection;

        protected bool change_from_grid;

        protected System.Web.UI.WebControls.Button btn_modifica;

        protected string numResult;

        protected DocsPaWebCtrlLibrary.ImageButton btn_clear_fields;

        protected System.Web.UI.WebControls.Panel pnl_riferimento;

        protected System.Web.UI.WebControls.TextBox txt_rif_mittente;

        protected DocsPAWA.UserControls.AuthorOwnerFilter aofAuthor, aofOwner;
        protected System.Web.UI.WebControls.RadioButtonList rblFiltriAllegati;
        protected Dictionary<string, Corrispondente> dic_Corr;
        //private void Page_Unload(object sender, System.EventArgs e)
        //{
        //    FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
        //}

        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
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

                if (ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_OGGETTO_COMM_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_OGGETTO_COMM_REF).Equals("1"))
                {
                    this.panel_numOgg_commRef.Visible = true;
                }
                Utils.startUp(this);

                userHome = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
                userRuolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];

                if (string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE)) ||
                 !bool.Parse(ConfigSettings.getKey(ConfigSettings.KeysENUM.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE)))
                {
                    p_cod_amm.Visible = false;
                    //txt_codDesc.Visible = false;
                    //l_amm_interop.Visible = false;
                }

                if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Document)
                {
                    GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);
                }

                //Gestione ricerche limitate
                if (UserManager.ruoloIsAutorized(this, "DO_RICERCHE_LIMITATE"))
                {
                    this.pnlTop.Visible = false;
                    this.pnlLimitata.Visible = true;
                    this.pnlBottom.Visible = false;
                    this.pnlBlank.Visible = true;
                    this.pnlRicSalvate.Visible = false;
                    this.btn_salva.Visible = false;
                    //this.lblNumProto.Visible = true;
                    this.ddl_numProt_E.Visible = false;
                    this.trNumProto.Visible = true;
                    this.star.Visible = true;
                    this.star1.Visible = true;
                    isLimited = true;
                    this.pnl_dataProt.Visible = false;
                    this.pnl_dataScad.Visible = false;
                }
                else
                {
                    this.pnlTop.Visible = true;
                    this.pnlLimitata.Visible = false;
                    this.pnlBottom.Visible = true;
                    this.pnlBlank.Visible = false;
                    this.pnlRicSalvate.Visible = true;
                    this.btn_salva.Visible = true;
                    //this.lblNumProto.Visible = false;
                    this.ddl_numProt_E.Visible = true;
                    this.trNumProto.Visible = true;
                    this.star.Visible = false;
                    this.star1.Visible = false;
                    isLimited = false;
                    this.pnl_dataProt.Visible = true;
                    this.pnl_dataScad.Visible = true;
                }

                if (isLimited)
                {
                    this.ddl_numProt_E.SelectedIndex = 0;
                    this.ddl_numProt_E.Enabled = false;
                }
                schedaRicerca = (SchedaRicerca)Session[SchedaRicerca.SESSION_KEY];
                if (schedaRicerca == null)
                {
                    //Inizializzazione della scheda di ricerca per la gestione delle 
                    //ricerche salvate
                    schedaRicerca = new SchedaRicerca(KEY_SCHEDA_RICERCA, userHome, userRuolo, this);
                    Session[SchedaRicerca.SESSION_KEY] = schedaRicerca;
                }
                schedaRicerca.Pagina = this;
                if (!IsPostBack)
                {

                    DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();
                    info = UserManager.getInfoUtente(this.Page);


                    string valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_OGGETTO");
                    if (!string.IsNullOrEmpty(valoreChiave))
                        caratteriDisponibili = int.Parse(valoreChiave);


                    txt_oggetto.MaxLength = caratteriDisponibili;
                    clTesto.Value = caratteriDisponibili.ToString();
                    txt_oggetto.Attributes.Add("onKeyUp", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','DESCRIZIONE'," + clTesto.ClientID + ")");
                    txt_oggetto.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','DESCRIZIONE'," + clTesto.ClientID + ")");
                    // se ritorno alla pagina di ricerca dopo aver settato una fascicolazione
                    // rapida da un protocollo dopo il riproponi, devo annullare la variabile di 
                    // sessione relativa al template altrimenti il campo codice fascicolo viene
                    // automaticamente valorizzato
                    if (FascicoliManager.getFascicoloSelezionatoFascRapida(this) != null)
                        FascicoliManager.removeFascicoloSelezionatoFascRapida();

                    //verifica se nuova ADL
                    if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1") && (!IsPostBack))
                    {
                        schedaRicerca.ElencoRicercheADL("D", false, ddl_Ric_Salvate, null);
                        isSavedSearch = PopulateField(schedaRicerca.FiltriRicerca, false);
                    }
                    else
                    {
                        schedaRicerca.ElencoRicerche("D", ddl_Ric_Salvate);
                        isSavedSearch = PopulateField(schedaRicerca.FiltriRicerca, false);
                    }
                }
                if (!IsPostBack)
                {
                    //Il comando seguente serve a definire quale combo contiene la lista delle ricerche salvate

                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_E.ID + "').focus();</SCRIPT>";
                    RegisterStartupScript("focus", s);

                    // Visualizzazione pagina di ricerca nella selezione 
                    // di un criterio di ricerca salvato
                    this.ddl_Ric_Salvate.Attributes.Add("onChange", "OnChangeSavedFilter();");

                    //set anno corrente al page load, ma non ap postback

                    // attenzione: se vengo da un back di elemento di una ricerca salvata
                    // devo comportarmi diversamente
                    if (!isSavedSearch)
                    {
                        // Se la ricerca è una ricerca in ADL, non viene valorizzato il campo Anno protocollo
                        if (Request["ricADL"] != "1")
                            this.tbAnnoProtocollo.Text = System.DateTime.Now.Year.ToString();
                    }

                    DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];

                    #region ABILITAZIONE PROTOCOLLAZIONE INTERNA
                    DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    if (!ws.IsInternalProtocolEnabled(cr.idAmministrazione))
                        this.cbl_archDoc_E.Items.Remove(this.cbl_archDoc_E.Items[2]);
                    #endregion

                    #region Abilitazione ricerca allegati
                    if (!this.IsEnabledProfilazioneAllegato)
                        this.cbl_archDoc_E.Items.Remove(this.cbl_archDoc_E.Items.FindByValue("ALL"));
                    #endregion

                    //Butt_ricerca.Attributes.Add("OnClick","ApriFrame('RicercaDoc_cn.aspx','centrale');");
                    //Button1.Attributes.Add("OnClick","ApriFrame('RicercaDoc_cn.aspx','centrale');");

                    // attenzione: se vengo da un back di elemento di una ricerca salvata
                    // devo comportarmi diversamente
                    if (!isSavedSearch)
                    {
                        this.txt_fineNumProt_E.Visible = false;
                        this.lblDAnumprot_E.Visible = false;
                        this.lblAnumprot_E.Visible = false;
                        this.GetCalendarControl("txt_fineDataProt_E").Visible = false;
                        this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = false;
                        this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = false;
                        this.lbl_finedataProt_E.Visible = false;
                        this.lbl_initdataProt_E.Visible = false;
                        this.GetCalendarControl("txt_fineDataScadenza_E").Visible = false;
                        this.GetCalendarControl("txt_fineDataScadenza_E").btn_Cal.Visible = false;
                        this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Visible = false;
                        this.lbl_fineDataScadenza_E.Visible = false;
                        this.lbl_initDataScadenza_E.Visible = false;
                        this.GetCalendarControl("txt_finedataCreazione_E").Visible = false;
                        this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Visible = false;
                        this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Visible = false;
                        this.lbl_dataCreazioneDa.Visible = false;
                        this.lbl_dataCreazioneA.Visible = false;
                        this.GetCalendarControl("txt_finedataStampa_E").Visible = false;
                        this.GetCalendarControl("txt_finedataStampa_E").btn_Cal.Visible = false;
                        this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Visible = false;
                        this.lbl_dataStampaDa.Visible = false;
                        this.lbl_dataStampaA.Visible = false;
                    }
                    setListaRegistri();
                    setFormProperties();

                    //metto in session tutti i registri che l'utente può vedere dato il suo ruolo
                    Session["AvailableRegistries"] = userRuolo.registri;

                    //uso una variabile di sessione per memorizzare i registri selezionati dall'utente
                    //listaReg = new System.Collections.ArrayList();
                    Session["elencoReg"] = listaReg;

                    DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    if (wws.isEnableRiferimentiMittente())
                    {
                        this.pnl_riferimento.Visible = true;
                    }
                }


                //carico il mittente selezionato, se esiste
                DocsPaWR.Corrispondente cor = UserManager.getCorrispondenteSelezionato(this);

                if (cor != null)
                {
                    this.txt_codMit_E.Text = cor.codiceRubrica;
                    this.txt_descrMit_E.Text = UserManager.getDecrizioneCorrispondenteSemplice(cor);
                    this.hd_systemIdMit_Est.Value = cor.systemId;
                }

                UserManager.removeCorrispondentiSelezionati(this);

                //Fine Celeste

                tastoInvio();
                //new ADL
                if (!IsPostBack &&
                    Request.QueryString["ricADL"] != null &&
                    Request.QueryString["ricADL"] == "1" &&
                    !SiteNavigation.CallContextStack.CurrentContext.IsBack)
                {
                    lblSearch.Text = "Ricerche Salvate Area di Lavoro";
                    this.btn_Ricerca_Click(null, null);
                }

                getLettereProtocolli();

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
                        ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=estesa&tabRes=estesa" + altro + "';", true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?from=estesa&tabRes=estesa" + altro + "';", true);
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

                if (!IsPostBack)
                {
                    this.btn_clear_fields.Attributes.Add("onmouseout", "this.src='" + "../images/ricerca/remove_search_filter.gif'");
                    this.btn_clear_fields.Attributes.Add("onmouseover", "this.src='" + "../images/ricerca/remove_search_filter_up.gif'");
                }

                #region Gestione filtro Allegati
                if (this.IsEnabledProfilazioneAllegato && cbl_archDoc_E.Items.FindByValue("ALL").Selected)
                    rblFiltriAllegati.Style.Add("display", "block");
                else if (this.IsEnabledProfilazioneAllegato && (!cbl_archDoc_E.Items.FindByValue("ALL").Selected))
                    rblFiltriAllegati.Style.Add("display", "none");
                if (this.IsEnabledProfilazioneAllegato)
                {
                    int countItem = -1;
                    foreach(ListItem i in cbl_archDoc_E.Items)
                    {
                        ++countItem;
                        if (i.Value.Equals("ALL"))
                        {
                                string scriptGestAll = "<script language='javascript'>" +
                                    "function SetVisibilityFilterAlleg() { " +
                                    "if (document.getElementById('cbl_archDoc_E_" + countItem + "') != null && " +
                                    "document.getElementById('cbl_archDoc_E_" + countItem + "').checked) { " +
                                    "document.getElementById('rblFiltriAllegati').style.display = 'block'; " +
                                    "} " +
                                    "else " +
                                    "{ " +
                                    " document.getElementById('rblFiltriAllegati').style.display = 'none'; " +
                                    "} " +
                                "} " +
                            "</script>";
                            ClientScript.RegisterStartupScript(this.GetType(), "enable_disable_all", scriptGestAll);
                            string scriptClickAll = "<script language='javascript'> " + 
                                "var cbo_all = document.getElementById('cbl_archDoc_E_" + countItem + "'); " +
                                "if(cbo_all.addEventListener){ " +
                                "cbo_all.addEventListener('clik',SetVisibilityFilterAlleg); " +
                                "} else { " +
                                "cbo_all.attachEvent('onclick', SetVisibilityFilterAlleg); " +
                                "} " +
                                "</script>";
                            ClientScript.RegisterStartupScript(this.GetType(), "event_click_all", scriptClickAll);
                            break;
                        }
                    }
                }
                #endregion
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void setListaRegistri()
        {
            bool filtroAoo = false;
            DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistriNoFiltroAOO(this, out filtroAoo);
            //Non è abilitata nel backend la chiave no_filtro_aoo
            if (userRegistri != null && filtroAoo)
            {
                ListItem itemM = new ListItem("I miei", "M");
                rbl_Reg_E.Items.Add(itemM);
                itemM = new ListItem("Tutti", "T");
                rbl_Reg_E.Items.Add(itemM);
                itemM = new ListItem("Reset", "R");
                rbl_Reg_E.Items.Add(itemM);
                lb_reg_E.Rows = 5;
            }
            else
            {
                userRegistri = userRuolo.registri;
                ListItem itemM = new ListItem("Tutti", "T");
                rbl_Reg_E.Items.Add(itemM);
                itemM = new ListItem("Reset", "R");
                rbl_Reg_E.Items.Add(itemM);
                //rbl_Reg_E.SelectedIndex = 1;
            }
            rbl_Reg_E.SelectedIndex = 0;
            string[] id = new string[userRegistri.Length];
            for (int i = 0; i < userRegistri.Length; i++)
            {
                if (!isLimited)
                {
                    //lb_reg_E.Items.Add(userRuolo.registri[i].descrizione);
                    lb_reg_E.Items.Add(userRegistri[i].codRegistro);
                    lb_reg_E.Items[i].Value = userRegistri[i].systemId;
                    string nomeRegCurrente = "UserReg" + i;
                    // SELEZIONA TUTTI I REGISTRI PRESENTI per DEFAULT
                    if (!filtroAoo)
                    {
                        if (!userRegistri[i].flag_pregresso)
                            lb_reg_E.Items[i].Selected = true;
                    }
                    else
                        if (rbl_Reg_E.SelectedItem.Value == "M")
                            for (int j = 0; j < userRuolo.registri.Length; j++)
                            {
                                if (userRuolo.registri[j].codRegistro == lb_reg_E.Items[i].Text)
                                {
                                    if (!userRegistri[i].flag_pregresso)
                                    {
                                        lb_reg_E.Items[i].Selected = true;
                                        break;
                                    }
                                }
                            }
                }
                else
                {
                    ddl_registri.Items.Add(userRegistri[i].codRegistro);
                    ddl_registri.Items[i].Value = userRegistri[i].systemId;
                }
                id[i] = (string)userRegistri[i].systemId;
            }
            if (!isLimited)
                rbl_Reg_E.Items[0].Selected = true;
            else
                ddl_registri.SelectedIndex = 0;
            UserManager.setListaIdRegistri(this, id);
        }

        private void setFormProperties()
        {
            this.btn_RubrOgget_E.Attributes.Add("onclick", "ApriOggettario('ric_E');");
            //commentata per problema minimizzazione rubrica
            //this.btn_Rubrica_E.Attributes.Add("onclick","ApriRubrica('ric_E','');");
        }

        protected void rbl_Reg_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.rbl_Reg_E.SelectedItem.Value.Equals("R"))
            {
                UserManager.removeListaIdRegistri(this);
                //for (int h = 0; h < this.lb_reg_E.Items.Count; h++)
                //    lb_reg_E.Items[h].Selected = false;
                lb_reg_E.ClearSelection();
            }
            if (this.rbl_Reg_E.SelectedItem.Value.Equals("T"))
            {
                UserManager.setListaIdRegistri(this, UserManager.getListaIdRegistriUtente(this));
                for (int h = 0; h < this.lb_reg_E.Items.Count; h++)
                    lb_reg_E.Items[h].Selected = true;
            }
            if (this.rbl_Reg_E.SelectedItem.Value.Equals("M"))
            {
                lb_reg_E.ClearSelection();
                ArrayList idList = new ArrayList();
                for (int h = 0; h < this.lb_reg_E.Items.Count; h++)
                {
                    for (int i = 0; i < userRuolo.registri.Length; i++)
                    {
                        if (userRuolo.registri[i].codRegistro == lb_reg_E.Items[h].Text)
                        {
                            if (userRuolo.registri[i] != null && !userRuolo.registri[i].flag_pregresso)
                            {
                                lb_reg_E.Items[h].Selected = true;
                                idList.Add(lb_reg_E.Items[h].Value);
                                break;
                            }
                        }
                    }

                }

                string[] id = new string[idList.Count];
                for (int i = 0; i < idList.Count; i++)
                    id[i] = (string)idList[i];
                UserManager.setListaIdRegistri(this, id);
                //UserManager.setListaIdRegistri(this, UserManager.getListaIdRegistriUtente(this));

            }
        }

        private void lb_reg_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.rbl_Reg_E.ClearSelection();

            ArrayList idList = new ArrayList();
            for (int h = 0; h < this.lb_reg_E.Items.Count; h++)
            {
                if (lb_reg_E.Items[h].Selected)
                    idList.Add(lb_reg_E.Items[h].Value);
            }
            string[] id = new string[idList.Count];
            for (int i = 0; i < idList.Count; i++)
                id[i] = (string)idList[i];
            UserManager.setListaIdRegistri(this, id);
        }

        protected bool RicercaEstesa()
        {
            try
            {
                //array contenitore degli array filtro di ricerca
                qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
                fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

                string valore = "";
                if (this.cbl_archDoc_E.Items.FindByValue("A") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
                    if (this.cbl_archDoc_E.Items.FindByValue("A").Selected)
                        fV1.valore = "true";
                    else
                        fV1.valore = "false";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                if (this.cbl_archDoc_E.Items.FindByValue("P") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
                    if (this.cbl_archDoc_E.Items.FindByValue("P").Selected)
                        //valore += "1^";
                        fV1.valore = "true";
                    else
                        //valore += "0^";
                        fV1.valore = "false";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                if (this.cbl_archDoc_E.Items.FindByValue("I") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
                    if (this.cbl_archDoc_E.Items.FindByValue("I").Selected)
                        //valore += "1^";
                        fV1.valore = "true";
                    else
                        //valore += "0^";
                        fV1.valore = "false";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                #region filtro per Stampe Registro
                if (this.cbl_archDoc_E.Items.FindByValue("R") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.STAMPA_REG.ToString();
                    if (this.cbl_archDoc_E.Items.FindByValue("R").Selected)
                        if (this.cbl_archDoc_E.Items.FindByValue("A").Selected ||
                            this.cbl_archDoc_E.Items.FindByValue("P").Selected ||
                            this.cbl_archDoc_E.Items.FindByValue("G").Selected ||
                            this.cbl_archDoc_E.Items.FindByValue("Pr").Selected ||
                            this.cbl_archDoc_E.Items.FindByValue("ALL").Selected)
                        {
                            fV1.valore = "U^true";
                        }
                        else
                            fV1.valore = "true";
                    else
                        fV1.valore = "false";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    if (this.ddl_dataStampa_E.SelectedIndex == 0)
                    {//valore singolo carico DATA_STAMPA
                        if (!this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Text.Equals(""))
                        {
                            if (!Utils.isDate(this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Text))
                            {
                                Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                                string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataStampa_E").txt_Data.ID + "').focus();</SCRIPT>";
                                RegisterStartupScript("focus", s);
                                Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                return false;
                            }
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO.ToString();
                            fV1.valore = this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }
                    if (this.ddl_dataStampa_E.SelectedIndex == 1)
                    {//valore singolo carico DATA_STAMPA_DAL - DATA_STAMPA_AL
                        if (!this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Text.Equals(""))
                        {
                            if (!Utils.isDate(this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Text))
                            {
                                Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                                string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataStampa_E").txt_Data.ID + "').focus();</SCRIPT>";
                                RegisterStartupScript("focus", s);
                                Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                return false;
                            }
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_DAL.ToString();
                            fV1.valore = this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        if (!this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Text.Equals(""))
                        {
                            if (!Utils.isDate(this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Text))
                            {
                                Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                                string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_finedataStampa_E").txt_Data.ID + "').focus();</SCRIPT>";
                                RegisterStartupScript("focus", s);
                                Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                return false;
                            }
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_AL.ToString();
                            fV1.valore = this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }

                    // numero protocollo stampe registro
                    if (this.ddl_numProt_E.SelectedIndex == 0)
                    {//valore singolo carico NUM_PROTOCOLLO
                        if (this.txt_initNumProt_E.Text != null && !this.txt_initNumProt_E.Text.Equals(""))
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA.ToString();
                            fV1.valore = this.txt_initNumProt_E.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }
                    else
                    {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                        if (!this.txt_initNumProt_E.Text.Equals(""))
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA_DAL.ToString();
                            fV1.valore = this.txt_initNumProt_E.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        if (!this.txt_fineNumProt_E.Text.Equals(""))
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA_AL.ToString();
                            fV1.valore = this.txt_fineNumProt_E.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }

                    if (!this.tbAnnoProtocollo.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriStampaRegistro.ANNO_PROTOCOLLO_STAMPA.ToString();
                        fV1.valore = this.tbAnnoProtocollo.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
                fV1.valore = this.cbl_archDoc_E.Items.FindByValue("G").Selected.ToString();
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                if (this.IsEnabledProfilazioneAllegato && this.cbl_archDoc_E.Items.FindByValue("ALL").Selected)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.ALLEGATO.ToString();
                    fV1.valore = this.rblFiltriAllegati.SelectedValue.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                if (this.cbl_archDoc_E.Items.FindByValue("Pr") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                    if (this.cbl_archDoc_E.Items.FindByValue("Pr").Selected)
                        //valore += "1";
                        fV1.valore = "true";
                    else
                        //valore += "0";
                        fV1.valore = "false";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                #region filtro Archivio (Arrivo, Partenza, Interni, Grigi, Predisposti, STampe)
                //if (!this.rb_archDoc_E.SelectedItem.Value.Equals("T"))
                //{
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                fV1.valore = "tipo";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //}
                #endregion
                #region filtro docNumber
                if (this.ddl_idDocumento_C.SelectedIndex == 0)
                {
                    if (this.txt_initIdDoc_C.Text != null && !this.txt_initIdDoc_C.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
                        fV1.valore = this.txt_initIdDoc_C.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {
                    if (this.txt_initIdDoc_C.Text != null && !this.txt_initIdDoc_C.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
                        fV1.valore = this.txt_initIdDoc_C.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (this.txt_fineIdDoc_C.Text != null && !this.txt_fineIdDoc_C.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
                        fV1.valore = this.txt_fineIdDoc_C.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion
                #region filtro registro
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                if (!isLimited) // se la ricerca non è limitata
                {
                    string registri = "";
                    if (this.lb_reg_E.Items.Count > 0)
                    {
                        for (int i = 0; i < this.lb_reg_E.Items.Count; i++)
                        {
                            if (this.lb_reg_E.Items[i].Selected)
                                registri += this.lb_reg_E.Items[i].Value + ",";
                        }
                    }
                    if (!registri.Equals(""))
                    {
                        registri = registri.Substring(0, registri.Length - 1);
                        fV1.valore = registri;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    else
                    {
                        Response.Write("<script>alert('Selezionare almeno un registro');top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return false;
                    }
                }
                else
                {
                    fV1.valore = ddl_registri.SelectedValue;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                #endregion
                #region filtro numero protocollo
                if (this.ddl_numProt_E.SelectedIndex == 0)
                {//valore singolo carico NUM_PROTOCOLLO

                    if (this.txt_initNumProt_E.Text != null && !this.txt_initNumProt_E.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();
                        fV1.valore = this.txt_initNumProt_E.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                    if (!this.txt_initNumProt_E.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                        fV1.valore = this.txt_initNumProt_E.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.txt_fineNumProt_E.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                        fV1.valore = this.txt_fineNumProt_E.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion
                #region filtro Data Scadenza
                if (this.ddl_dataScadenza_E.SelectedIndex == 2)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCAD_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataScadenza_E.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCAD_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataScadenza_E.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCAD_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataScadenza_E.SelectedIndex == 0)
                {
                    if (!this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCADENZA_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataScadenza_E.SelectedIndex == 1)
                {
                    if (!this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCADENZA_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");

                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCADENZA_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion
                #region filtro data protocollo
                if (this.ddl_dataProt_E.SelectedIndex == 2)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProt_E.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProt_E.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProt_E.SelectedIndex == 0)
                {//valore singolo carico DATA_PROTOCOLLO
                    if (!this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProt_E").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataProt_E.SelectedIndex == 1)
                {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                    if (!this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProt_E").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataProt_E").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");

                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion
                #region filtro data creazione
                if (this.ddl_dataCreazione_E.SelectedIndex == 2)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataCreazione_E.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataCreazione_E.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataCreazione_E.SelectedIndex == 0)
                {//valore singolo carico DATA_CREAZIONE
                    if (!this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataCreazione_E.SelectedIndex == 1)
                {//valore singolo carico DATA_CREAZIONE_DAL - DATA_CREAZIONE_AL
                    if (!this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");

                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion
                #region filtro Data Stampa Registro
                if (this.ddl_dataStampa_E.SelectedIndex == 2)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_STAMPA_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataStampa_E.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_STAMPA_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataStampa_E.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_STAMPA_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                //if (this.cbl_archDoc_E.Items.FindByValue("S") != null && this.cbl_archDoc_E.Items.FindByValue("S").Selected)
                //{
                //    if (this.ddl_dataStampa.SelectedIndex == 0)
                //    {
                //        if (!this.GetCalendarControl("txt_initDataStampa").txt_Data.Text.Equals(""))
                //        {
                //            if (!Utils.isDate(this.GetCalendarControl("txt_initDataStampa").txt_Data.Text))
                //            {
                //                Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                //                string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataStampa").txt_Data.ID + "').focus();</SCRIPT>";
                //                RegisterStartupScript("focus", s);
                //                Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                //                return false;
                //            }
                //            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //            fV1.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO.ToString(); ;
                //            fV1.valore = this.GetCalendarControl("txt_initDataStampa").txt_Data.Text;
                //            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //        }
                //    }
                //    else
                //    {
                //        if (!this.GetCalendarControl("txt_initDataStampa").txt_Data.Text.Equals(""))
                //        {
                //            if (!Utils.isDate(this.GetCalendarControl("txt_initDataStampa").txt_Data.Text))
                //            {
                //                Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                //                string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataStampa").txt_Data.ID + "').focus();</SCRIPT>";
                //                RegisterStartupScript("focus", s);
                //                Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                //                return false;
                //            }
                //            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //            fV1.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_DAL.ToString();
                //            fV1.valore = this.GetCalendarControl("txt_initDataStampa").txt_Data.Text;
                //            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //        }
                //        if (!this.GetCalendarControl("txt_fineDataStampa").txt_Data.Text.Equals(""))
                //        {
                //            if (!Utils.isDate(this.GetCalendarControl("txt_fineDataStampa").txt_Data.Text))
                //            {
                //                Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                //                string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataStampa").txt_Data.ID + "').focus();</SCRIPT>";
                //                RegisterStartupScript("focus", s);
                //                Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");

                //                return false;
                //            }
                //            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //            fV1.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_AL.ToString();
                //            fV1.valore = this.GetCalendarControl("txt_fineDataStampa").txt_Data.Text;
                //            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //        }
                //    }
                //}
                #endregion
                #region filtro oggetto
                if (!this.txt_oggetto.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.OGGETTO.ToString();
                    fV1.valore = Utils.DO_AdattaString(this.txt_oggetto.Text);
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                #region filtro mitt/dest
                if (!string.IsNullOrEmpty(this.hd_systemIdMit_Est.Value))
                {
                    if (!this.txt_descrMit_E.Text.Equals(""))
                    {
                        if (!string.IsNullOrEmpty(this.txt_codMit_E.Text))
                        {
                            if (this.chk_mitt_dest_storicizzati.Checked)
                            {
                                // Ricerca i documenti per i mittenti / destinatari storicizzati
                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString();
                                fV1.valore = this.txt_codMit_E.Text;
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString();
                                fV1.valore = this.chk_mitt_dest_storicizzati.Checked.ToString();
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                            else
                            {
                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
                                fV1.valore = this.hd_systemIdMit_Est.Value;
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                        }
                    }
                }
                else
                {
                    if (!this.txt_descrMit_E.Text.Equals(""))
                    {
                        if (!string.IsNullOrEmpty(this.txt_codMit_E.Text))
                        {
                            if (this.chk_mitt_dest_storicizzati.Checked)
                            {
                                // Ricerca i documenti per i mittenti / destinatari storicizzati
                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString();
                                fV1.valore = this.txt_codMit_E.Text;
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString();
                                fV1.valore = this.chk_mitt_dest_storicizzati.Checked.ToString();
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                            else
                            {
                                // Ricerca dell'id del corrispondente a partire dal codice
                                DocsPaWR.Corrispondente corrByCode = UserManager.getCorrispondenteByCodRubrica(this, this.txt_codMit_E.Text);
                                if (corrByCode != null)
                                {
                                    this.hd_systemIdMit_Est.Value = corrByCode.systemId;

                                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
                                    fV1.valore = this.hd_systemIdMit_Est.Value;
                                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                                }
                                else
                                {
                                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
                                    fV1.valore = this.txt_descrMit_E.Text;
                                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                        }
                        else
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
                            fV1.valore = this.txt_descrMit_E.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }
                }

                #endregion
                #region filtro numero oggetto
                if (!this.txt_numOggetto.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_OGGETTO.ToString();
                    fV1.valore = this.txt_numOggetto.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
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

                #region filtro CODICE FASCICOLO
                if (!this.txt_CodFascicolo.Text.Equals(""))
                {
                    #region costruzione condizione IN per valorizzare il filtro di ricerca IN_CHILD_RIC_ESTESA
                    //Viene ricavato il root folder(con tutti i sottofascicoli) da ogni fascicolo trovato
                    // ArrayList listaFascicoli = FascicoliManager.getFascicoliSelezionati(this);
                    //ArrayList listaFascicoli = getFascicoli();

                    ArrayList listaFascicoli = null;
                    if (FascicoliManager.getFascicoloSelezionatoFascRapida(this) != null)
                    {
                        listaFascicoli = new ArrayList();
                        listaFascicoli.Add(FascicoliManager.getFascicoloSelezionatoFascRapida(this));
                    }
                    else //da Cambiare perchè cerca in tutti i fascicoli indipentemente da quello selezionato !!!
                        listaFascicoli = new ArrayList(FascicoliManager.getListaFascicoliDaCodice(this, this.txt_CodFascicolo.Text, UserManager.getRegistroSelezionato(this), "R"));

                    string inSubFolder = "IN (";
                    for (int k = 0; k < listaFascicoli.Count; k++)
                    {
                        DocsPaWR.Folder folder = FascicoliManager.getFolder(this, (DocsPAWA.DocsPaWR.Fascicolo)listaFascicoli[k]);
                        inSubFolder += folder.systemID;
                        if (folder.childs != null && folder.childs.Length > 0)
                        {
                            for (int i = 0; i < folder.childs.Length; i++)
                            {
                                inSubFolder += ", " + folder.childs[i].systemID;
                                inSubFolder = getInStringChild(folder.childs[i], inSubFolder);
                            }
                        }
                        inSubFolder += ",";
                    }
                    inSubFolder = inSubFolder.Substring(0, inSubFolder.Length - 1) + ")";

                    #endregion

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.IN_CHILD_RIC_ESTESA.ToString();
                    fV1.valore = inSubFolder;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                #region filtro RICERCA IN AREA LAVORO
                if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1"))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_IN_ADL.ToString();
                    fV1.valore = this.userHome.idPeople.ToString() + "@" + this.userRuolo.systemId.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                #region visualizza storico  ?da aggiustare? il valore da passare pu essere qualunque
                //				if (this.rbl_Rif_E.SelectedItem.Value.Equals("S"))
                //				{
                //					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
                //					fV1.argomento=DocsPaWR.FiltriDocumento.VIS_STORICO_MITT_DEST.ToString();
                //					fV1.valore=this.rbl_Rif_E.SelectedItem.Value;
                //					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
                //				}
                #endregion
                #region Anno Protocollo
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                fV1.valore = this.tbAnnoProtocollo.Text;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion

                #region Ordinamento

                // Reperimento del filtro da utilizzare per la griglia
                List<FiltroRicerca> filterList = GridManager.GetOrderFilterForDocument(this.ddlOrder.SelectedValue, this.ddlOrderDirection.SelectedValue);

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

                //nuovo filtro per prendere solo i documenti protocollati

                #region filtro riferimento
                DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                if (wws.isEnableRiferimentiMittente() && !string.IsNullOrEmpty(txt_rif_mittente.Text))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.RIFERIMENTO_MITTENTE.ToString();
                    fV1.valore = txt_rif_mittente.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                #endregion

                //ABBATANGELI GIANLUIGI - Filtro per nascondere doc di altre applicazioni
                if (System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"] != null && !System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"].Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.COD_EXT_APP.ToString();
                    fV1.valore = (System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"]);
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                #region
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DA_PROTOCOLLARE.ToString();
                fV1.valore = "0";  //corrisponde a 'false'
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion
                #region codice nome amministrazione
                if (!this.txt_codDesc.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.CODICE_DESCRIZIONE_AMMINISTRAZIONE.ToString();
                    fV1.valore = this.txt_codDesc.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                qV[0] = fVList;
                return true;
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
                return false;
            }
        }

        private void ddl_idDocumento_C_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            txt_fineIdDoc_C.Text = "";
            if (this.ddl_idDocumento_C.SelectedIndex == 0)
            {
                this.txt_fineIdDoc_C.Visible = false;
                this.lblAidDoc_C.Visible = false;
                this.lblDAidDoc_C.Visible = false;
            }
            else
            {
                this.txt_fineIdDoc_C.Visible = true;
                this.lblAidDoc_C.Visible = true;
                this.lblDAidDoc_C.Visible = true;
            }
        }

        protected bool PopulateField(DocsPAWA.DocsPaWR.FiltroRicerca[][] qV, bool grid)
        {
            try
            {
                if (qV == null || qV.Length == 0)
                    return false;

                #region pulizia campi
                #region DOCNUMBER
                ddl_idDocumento_C.SelectedIndex = 0;
                ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
                txt_initIdDoc_C.Text = "";
                #endregion DOCNUMBER
                #region TIPO
                //rb_archDoc_E.SelectedValue = "T";
                //for (int i = 0; i < this.cbl_archDoc_E.Items.Count; i++)
                //{
                //    this.cbl_archDoc_E.Items[i].Selected = true;
                //}
                #endregion TIPO
                #region REGISTRO
                foreach (ListItem i in lb_reg_E.Items)
                    i.Selected = true;
                #endregion REGISTRO
                #region NUM_PROTOCOLLO
                ddl_numProt_E.SelectedIndex = 0;
                ddl_numProt_E_SelectedIndexChanged(null, new System.EventArgs());
                txt_initNumProt_E.Text = "";
                #endregion NUM_PROTOCOLLO
                #region ANNO_PROTOCOLLO
                tbAnnoProtocollo.Text = DateTime.Now.Year.ToString();
                #endregion ANNO_PROTOCOLLO
                #region DATA_PROT
                ddl_dataProt_E.SelectedIndex = 0;
                ddl_dataProt_E_SelectedIndexChanged(null, new System.EventArgs());
                this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text = "";
                #endregion DATA_PROT
                #region DATA_SCADENZA
                ddl_dataScadenza_E.SelectedIndex = 0;
                ddl_dataScadenza_SelectedIndexChanged(null, new System.EventArgs());
                this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text = "";
                #endregion DATA_SCADENZA
                #region DATA_CREAZIONE
                ddl_dataCreazione_E.SelectedIndex = 0;
                ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text = "";
                #endregion DATA_CREAZIONE
                //#region DATA_STAMPA_REGISTRO
                //ddl_dataStampa.SelectedIndex = 0;
                //ddl_dataStampa_SelectedIndexChanged(null, new System.EventArgs());
                //this.GetCalendarControl("txt_initDataStampa").txt_Data.Text = "";
                //#endregion DATA_STAMPA_REGISTRO
                #region OGGETTO
                txt_oggetto.Text = "";
                #endregion OGGETTO
                #region MITT_DEST
                txt_codMit_E.Text = "";
                txt_descrMit_E.Text = "";
                #endregion MITT_DEST
                #region NUM_OGGETTO
                txt_numOggetto.Text = "";
                #endregion NUM_OGGETTO
                #region FASCICOLO
                txt_CodFascicolo.Text = "";
                txt_DescFascicolo.Text = "";
                FascicoliManager.setFascicoliSelezionati(this, null);
                #endregion FASCICOLO
                #region RIFERIMENTO MITTENTE
                DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                if (wws.isEnableRiferimentiMittente())
                {
                    this.txt_rif_mittente.Text = string.Empty;
                }
                #endregion
                #endregion pulizia campi

                DocsPaWR.FiltroRicerca[] filters = qV[0];
                //array contenitore degli array filtro di ricerca

                if (this.Session["itemUsedSearch"] != null)
                    ddl_Ric_Salvate.SelectedIndex = Convert.ToInt32(this.Session["itemUsedSearch"]);

                foreach (DocsPAWA.DocsPaWR.FiltroRicerca aux in filters)
                {
                    try
                    {
                        #region TIPO
                        if (aux.argomento == DocsPaWR.FiltriDocumento.TIPO.ToString())
                        {
                            //string[] tipoDoc = aux.valore.Split('^');
                            //if (tipoDoc[0].Equals("1"))
                            //    this.cbl_archDoc_E.Items.FindByValue("A").Selected = true;
                            //else
                            //    this.cbl_archDoc_E.Items.FindByValue("A").Selected = false;
                            //if (tipoDoc[1].Equals("1"))
                            //    this.cbl_archDoc_E.Items.FindByValue("P").Selected = true;
                            //else
                            //    this.cbl_archDoc_E.Items.FindByValue("P").Selected = false;
                            //if (tipoDoc[2].Equals("1"))
                            //    if(this.cbl_archDoc_E.Items.FindByValue("I") != null)
                            //        this.cbl_archDoc_E.Items.FindByValue("I").Selected = true;
                            //else
                            //    if (this.cbl_archDoc_E.Items.FindByValue("I") != null)
                            //        this.cbl_archDoc_E.Items.FindByValue("I").Selected = false;
                            //if (tipoDoc[3].Equals("1"))
                            //    this.cbl_archDoc_E.Items.FindByValue("G").Selected = true;
                            //else
                            //    this.cbl_archDoc_E.Items.FindByValue("G").Selected = false;
                            //if (tipoDoc[4].Equals("1"))
                            //    this.cbl_archDoc_E.Items.FindByValue("Pr").Selected = true;
                            //else
                            //    this.cbl_archDoc_E.Items.FindByValue("Pr").Selected = false;
                            //if (tipoDoc[5].Equals("1"))
                            //    this.cbl_archDoc_E.Items.FindByValue("S").Selected = true;
                            //else
                            //    this.cbl_archDoc_E.Items.FindByValue("S").Selected = false;
                            //rb_archDoc_E.SelectedValue = aux.valore;
                        }
                        #endregion TIPO
                        #region PROTOCOLLO_ARRIVO
                        if (aux.argomento == DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString())
                        {
                            this.cbl_archDoc_E.Items.FindByValue("A").Selected = Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region PROTOCOLLO_PARTENZA
                        if (aux.argomento == DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString())
                        {
                            this.cbl_archDoc_E.Items.FindByValue("P").Selected = Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region PROTOCOLLO_INTERNO
                        if (aux.argomento == DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString())
                        {
                            if (this.cbl_archDoc_E.Items.FindByValue("I") != null)
                                this.cbl_archDoc_E.Items.FindByValue("I").Selected = Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region GRIGI
                        if (aux.argomento == DocsPaWR.FiltriDocumento.GRIGIO.ToString())
                        {
                            this.cbl_archDoc_E.Items.FindByValue("G").Selected = Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region PREDISPOSTI
                        if (aux.argomento == DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString())
                        {
                            this.cbl_archDoc_E.Items.FindByValue("Pr").Selected = Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region ALLEGATI
                        if (aux.argomento == DocsPaWR.FiltriDocumento.ALLEGATO.ToString())
                        {
                            this.cbl_archDoc_E.Items.FindByValue("ALL").Selected = true; //Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region REGISTRO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.REGISTRO.ToString())
                        {
                            char[] sep = { ',' };
                            string[] regs = aux.valore.Split(sep);
                            foreach (ListItem li in lb_reg_E.Items)
                                li.Selected = false;
                            foreach (string reg in regs)
                            {
                                for (int i = 0; i < lb_reg_E.Items.Count; i++)
                                {
                                    if (lb_reg_E.Items[i].Value == reg)
                                        lb_reg_E.Items[i].Selected = true;
                                }
                            }
                        }
                        #endregion REGISTRO
                        #region NUM_PROTOCOLLO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString())
                        {
                            if (ddl_numProt_E.SelectedIndex != 0)
                                ddl_numProt_E.SelectedIndex = 0;
                            ddl_numProt_E_SelectedIndexChanged(null, new System.EventArgs());
                            txt_initNumProt_E.Text = aux.valore;
                        }
                        #endregion NUM_PROTOCOLLO
                        #region NUM_PROTOCOLLO_DAL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString())
                        {
                            if (ddl_numProt_E.SelectedIndex != 1)
                                ddl_numProt_E.SelectedIndex = 1;
                            ddl_numProt_E_SelectedIndexChanged(null, new System.EventArgs());
                            txt_initNumProt_E.Text = aux.valore;
                        }
                        #endregion NUM_PROTOCOLLO_DAL
                        #region NUM_PROTOCOLLO_AL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString())
                        {
                            if (ddl_numProt_E.SelectedIndex != 1)
                                ddl_numProt_E.SelectedIndex = 1;
                            ddl_numProt_E_SelectedIndexChanged(null, new System.EventArgs());
                            txt_fineNumProt_E.Text = aux.valore;
                        }
                        #endregion NUM_PROTOCOLLO_AL
                        #region ANNO_PROTOCOLLO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString())
                        {
                            tbAnnoProtocollo.Text = aux.valore;
                        }
                        #endregion ANNO_PROTOCOLLO
                        #region DATA_SCADENZA_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCADENZA_IL.ToString())
                        {
                            if (ddl_dataScadenza_E.SelectedIndex != 0)
                                ddl_dataScadenza_E.SelectedIndex = 0;
                            ddl_dataScadenza_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataScadenza_E").Visible = true;
                            this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataScadenza_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_SCADENZA_IL
                        #region DATA_SCADENZA_SUCCESSIVA_AL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCADENZA_SUCCESSIVA_AL.ToString())
                        {
                            if (ddl_dataScadenza_E.SelectedIndex != 1)
                                ddl_dataScadenza_E.SelectedIndex = 1;
                            ddl_dataScadenza_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataScadenza_E").Visible = true;
                            this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataScadenza_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_SCADENZA_SUCCESSIVA_AL
                        #region DATA_SCADENZA_PRECEDENTE_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCADENZA_PRECEDENTE_IL.ToString())
                        {
                            if (ddl_dataScadenza_E.SelectedIndex != 1)
                                ddl_dataScadenza_E.SelectedIndex = 1;
                            ddl_dataScadenza_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_fineDataScadenza_E").Visible = true;
                            this.GetCalendarControl("txt_fineDataScadenza_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Visible = true;
                            this.lbl_fineDataScadenza_E.Visible = true;
                            this.lbl_initDataScadenza_E.Visible = true;
                            this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_SCADENZA_PRECEDENTE_IL
                        #region DATA_SCAD_SC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCAD_SC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataScadenza_E.SelectedIndex = 3;
                            this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                            this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataScadenza_E").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataScadenza_E").Visible = true;
                            this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataScadenza_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataScadenza_E").btn_Cal.Enabled = false;
                            this.lbl_fineDataScadenza_E.Visible = true;
                            this.lbl_initDataScadenza_E.Visible = true;
                        }
                        #endregion
                        #region DATA_SCAD_MC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCAD_MC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataScadenza_E.SelectedIndex = 4;
                            this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                            this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataScadenza_E").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataScadenza_E").Visible = true;
                            this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataScadenza_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataScadenza_E").btn_Cal.Enabled = false;
                            this.lbl_fineDataScadenza_E.Visible = true;
                            this.lbl_initDataScadenza_E.Visible = true;
                        }
                        #endregion
                        #region DATA_SCAD_TODAY
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCAD_TODAY.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataScadenza_E.SelectedIndex = 2;
                            this.GetCalendarControl("txt_initDataScadenza_E").Visible = true;
                            this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                            this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataScadenza_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataScadenza_E").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataScadenza_E").Visible = false;
                            this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_fineDataScadenza_E").btn_Cal.Visible = false;
                            this.lbl_fineDataScadenza_E.Visible = false;
                            this.lbl_initDataScadenza_E.Visible = false;
                        }
                        #endregion
                        #region DATA_CREAZIONE_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString())
                        {
                            if (ddl_dataCreazione_E.SelectedIndex != 0)
                                ddl_dataCreazione_E.SelectedIndex = 0;
                            ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataCreazione_E").Visible = true;
                            this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataCreazione_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_CREAZIONE_IL
                        #region DATA_CREAZIONE_SUCCESSIVA_AL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString())
                        {
                            if (ddl_dataCreazione_E.SelectedIndex != 1)
                                ddl_dataCreazione_E.SelectedIndex = 1;
                            ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataCreazione_E").Visible = true;
                            this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataCreazione_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_CREAZIONE_SUCCESSIVA_AL
                        #region DATA_CREAZIONE_PRECEDENTE_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString())
                        {
                            if (ddl_dataCreazione_E.SelectedIndex != 1)
                                ddl_dataCreazione_E.SelectedIndex = 1;
                            ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_finedataCreazione_E").Visible = true;
                            this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_CREAZIONE_PRECEDENTE_IL
                        #region DATA_CREAZ_SC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_SC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataCreazione_E.SelectedIndex = 3;
                            this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                            this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataCreazione_E").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataCreazione_E").Visible = true;
                            this.GetCalendarControl("txt_fineDataCreazione_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_fineDataCreazione_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataCreazione_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataCreazione_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataCreazione_E").btn_Cal.Enabled = false;
                            this.lbl_dataCreazioneA.Visible = true;
                            this.lbl_dataCreazioneDa.Visible = true;
                        }
                        #endregion
                        #region DATA_CREAZ_MC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_MC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataCreazione_E.SelectedIndex = 4;
                            this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                            this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataCreazione_E").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataCreazione_E").Visible = true;
                            this.GetCalendarControl("txt_fineDataCreazione_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_fineDataCreazione_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataCreazione_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataCreazione_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataCreazione_E").btn_Cal.Enabled = false;
                            this.lbl_dataCreazioneA.Visible = true;
                            this.lbl_dataCreazioneDa.Visible = true;
                        }
                        #endregion
                        #region DATA_CREAZ_TODAY
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_TODAY.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataCreazione_E.SelectedIndex = 2;
                            this.GetCalendarControl("txt_initDataCreazione_E").Visible = true;
                            this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                            this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataCreazione_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataCreazione_E").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataCreazione_E").Visible = false;
                            this.GetCalendarControl("txt_fineDataCreazione_E").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_fineDataCreazione_E").btn_Cal.Visible = false;
                            this.lbl_dataCreazioneA.Visible = false;
                            this.lbl_dataCreazioneDa.Visible = false;
                        }
                        #endregion
                        #region DATA_PROT_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString())
                        {
                            if (ddl_dataProt_E.SelectedIndex != 0)
                                ddl_dataProt_E.SelectedIndex = 0;
                            ddl_dataProt_E_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataProt_E").Visible = true;
                            this.GetCalendarControl("txt_initDataProt_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_PROT_IL
                        #region DATA_PROT_SUCCESSIVA_AL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString())
                        {
                            if (ddl_dataProt_E.SelectedIndex != 1)
                                ddl_dataProt_E.SelectedIndex = 1;
                            ddl_dataProt_E_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataProt_E").Visible = true;
                            this.GetCalendarControl("txt_initDataProt_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_PROT_SUCCESSIVA_AL
                        #region DATA_PROT_PRECEDENTE_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString())
                        {
                            if (ddl_dataProt_E.SelectedIndex != 1)
                                ddl_dataProt_E.SelectedIndex = 1;
                            ddl_dataProt_E_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_fineDataProt_E").Visible = true;
                            this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = true;
                            this.lbl_finedataProt_E.Visible = true;
                            this.lbl_initdataProt_E.Visible = true;
                            this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_PROT_PRECEDENTE_IL
                        #region DATA_PROT_SC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_SC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProt_E.SelectedIndex = 3;
                            this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                            this.GetCalendarControl("txt_initDataProt_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProt_E").Visible = true;
                            this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Enabled = false;
                            this.lbl_finedataProt_E.Visible = true;
                            this.lbl_initdataProt_E.Visible = true;
                        }
                        #endregion
                        #region DATA_PROT_MC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProt_E.SelectedIndex = 4;
                            this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                            this.GetCalendarControl("txt_initDataProt_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProt_E").Visible = true;
                            this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Enabled = false;
                            this.lbl_finedataProt_E.Visible = true;
                            this.lbl_initdataProt_E.Visible = true;
                        }
                        #endregion
                        #region DATA_PROT_TODAY
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_TODAY.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProt_E.SelectedIndex = 2;
                            this.GetCalendarControl("txt_initDataProt_E").Visible = true;
                            this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                            this.GetCalendarControl("txt_initDataProt_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataProt_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProt_E").Visible = false;
                            this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = false;
                            this.lbl_finedataProt_E.Visible = false;
                            this.lbl_initdataProt_E.Visible = false;
                        }
                        #endregion
                        #region DOCNUMBER
                        if (aux.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER.ToString())
                        {
                            if (ddl_idDocumento_C.SelectedIndex != 0)
                                ddl_idDocumento_C.SelectedIndex = 0;
                            ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
                            txt_initIdDoc_C.Text = aux.valore;
                        }
                        #endregion DOCNUMBER
                        #region DOCNUMBER_DAL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString())
                        {
                            if (ddl_idDocumento_C.SelectedIndex != 1)
                                ddl_idDocumento_C.SelectedIndex = 1;
                            ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
                            txt_initIdDoc_C.Text = aux.valore;
                        }
                        #endregion DOCNUMBER_DAL
                        #region DOCNUMBER_AL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString())
                        {
                            if (ddl_idDocumento_C.SelectedIndex != 1)
                                ddl_idDocumento_C.SelectedIndex = 1;
                            ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
                            txt_fineIdDoc_C.Text = aux.valore;
                        }
                        #endregion DOCNUMBER_AL
                        #region DATA_STAMPA_IL
                        else if (aux.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO.ToString())
                        {
                            if (ddl_dataStampa_E.SelectedIndex != 0)
                                ddl_dataStampa_E.SelectedIndex = 0;
                            ddl_dataStampa_E_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataStampa").Visible = true;
                            this.GetCalendarControl("txt_initDataStampa").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataStampa").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataStampa").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_STAMPA_IL
                        #region DATA_STAMPA_SUCCESSIVA_AL
                        else if (aux.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_DAL.ToString())
                        {
                            if (ddl_dataStampa_E.SelectedIndex != 1)
                                ddl_dataStampa_E.SelectedIndex = 1;
                            ddl_dataStampa_E_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataStampa").Visible = true;
                            this.GetCalendarControl("txt_initDataStampa").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataStampa").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataStampa").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_STAMPA_SUCCESSIVA_AL
                        #region DATA_STAMPA_PRECEDENTE_IL
                        else if (aux.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_AL.ToString())
                        {
                            if (ddl_dataStampa_E.SelectedIndex != 1)
                                ddl_dataStampa_E.SelectedIndex = 1;
                            ddl_dataStampa_E_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_fineDataStampa").Visible = true;
                            this.GetCalendarControl("txt_fineDataStampa").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataStampa").txt_Data.Visible = true;
                            this.lbl_dataStampaDa.Visible = true;
                            this.lbl_dataStampaA.Visible = true;
                            this.GetCalendarControl("txt_fineDataStampa").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_STAMPA_PRECEDENTE_IL
                        #region DATA_STAMPA_SC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_STAMPA_SC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataStampa_E.SelectedIndex = 3;
                            this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                            this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataStampa_E").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataStampa_E").Visible = true;
                            this.GetCalendarControl("txt_fineDataStampa_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_fineDataStampa_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataStampa_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataStampa_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataStampa_E").btn_Cal.Enabled = false;
                            this.lbl_dataStampaA.Visible = true;
                            this.lbl_dataStampaDa.Visible = true;
                        }
                        #endregion
                        #region DATA_STAMPA_MC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_STAMPA_MC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataStampa_E.SelectedIndex = 4;
                            this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                            this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataStampa_E").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataStampa_E").Visible = true;
                            this.GetCalendarControl("txt_fineDataStampa_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_fineDataStampa_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataStampa_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataStampa_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataStampa_E").btn_Cal.Enabled = false;
                            this.lbl_dataStampaA.Visible = true;
                            this.lbl_dataStampaDa.Visible = true;
                        }
                        #endregion
                        #region DATA_STAMPA_TODAY
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_STAMPA_TODAY.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataStampa_E.SelectedIndex = 2;
                            this.GetCalendarControl("txt_initDataStampa_E").Visible = true;
                            this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                            this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataStampa_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataStampa_E").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataStampa_E").Visible = false;
                            this.GetCalendarControl("txt_fineDataStampa_E").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_fineDataStampa_E").btn_Cal.Visible = false;
                            this.lbl_dataStampaA.Visible = false;
                            this.lbl_dataStampaDa.Visible = false;
                        }
                        #endregion
                        #region OGGETTO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.OGGETTO.ToString())
                        {
                            txt_oggetto.Text = aux.valore;
                        }
                        #endregion OGGETTO
                        #region MITT_DEST
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.MITT_DEST.ToString())
                        {
                            txt_descrMit_E.Text = aux.valore;
                        }
                        #endregion MITT_DEST
                        #region COD_MITT_DEST
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString())
                        {
                            DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubrica(this, aux.valore);
                            txt_codMit_E.Text = corr.codiceRubrica;
                            txt_descrMit_E.Text = corr.descrizione;
                        }
                        #endregion
                        #region MITT_DEST_STORICIZZATI
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString())
                        {
                            bool chkValue;
                            bool.TryParse(aux.valore, out chkValue);
                            this.chk_mitt_dest_storicizzati.Checked = chkValue;
                        }
                        #endregion
                        #region ID_MITT_DEST
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString())
                        {
                            DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteBySystemID(this, aux.valore);
                            txt_codMit_E.Text = corr.codiceRubrica;
                            txt_descrMit_E.Text = corr.descrizione;
                        }
                        #endregion ID_MITT_DEST
                        #region NUM_OGGETTO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NUM_OGGETTO.ToString())
                        {
                            txt_numOggetto.Text = aux.valore;
                        }
                        #endregion NUM_OGGETTO
                        //#region Creatore (User Control)
                        //else if (aux.argomento == DocsPaWR.FiltriDocumento.ID_PEOPLE_CREATORE.ToString()
                        //    || aux.argomento == DocsPaWR.FiltriDocumento.ID_UO_CREATORE.ToString()
                        //    || aux.argomento == DocsPaWR.FiltriDocumento.ID_RUOLO_CREATORE.ToString()
                        //    || aux.argomento == DocsPaWR.FiltriDocumento.DESC_PEOPLE_CREATORE.ToString()
                        //    || aux.argomento == DocsPaWR.FiltriDocumento.DESC_RUOLO_CREATORE.ToString()
                        //    || aux.argomento == DocsPaWR.FiltriDocumento.DESC_UO_CREATORE.ToString()
                        //    )
                        //{
                        //    this.Creatore.RestoreCurrentFilters();
                        //}

                        //#endregion
                        #region FASCICOLO
                        else if (aux.argomento == DocsPaWR.FiltriFascicolazione.IN_CHILD_RIC_ESTESA.ToString())
                        {
                            string val = aux.valore.Trim();
                            val = val.Substring("IN".Length).Trim();
                            val = val.Substring("(".Length).Trim();
                            val = val.Substring(0, val.LastIndexOf(")")).Trim();
                            char[] sep = { ',' };
                            string[] ids = val.Split(sep);
                            if (ids != null && ids.Length > 0)
                            {
                                DocsPaWR.Folder folder = FascicoliManager.getFolder(this, ids[0].Trim());
                                DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicolo(this, folder.idFascicolo);
                                if (fasc != null)
                                {
                                    //ArrayList listaFascicoli = FascicoliManager.getFascicoloDaCodice3(this, fasc.codice);
                                    ArrayList listaFascicoli = new ArrayList(FascicoliManager.getListaFascicoliDaCodice(this, fasc.codice, UserManager.getRegistroSelezionato(this), "R"));

                                    if (listaFascicoli != null)
                                    {
                                        FascicoliManager.setFascicoliSelezionati(this, listaFascicoli);
                                        txt_CodFascicolo.Text = fasc.codice;
                                        if (listaFascicoli.Count == 1)
                                        {
                                            txt_DescFascicolo.Text = fasc.descrizione;
                                        }
                                        else
                                        {
                                            if (((DocsPAWA.DocsPaWR.Fascicolo)listaFascicoli[0]).descrizione == ((DocsPAWA.DocsPaWR.Fascicolo)listaFascicoli[1]).descrizione)
                                                txt_DescFascicolo.Text = ((DocsPAWA.DocsPaWR.Fascicolo)listaFascicoli[0]).descrizione;
                                        }
                                    }
                                    /*                                    if (listaFascicoli != null && listaFascicoli.Count == 2)
                                                                        {
                                                                            if (((DocsPAWA.DocsPaWR.Fascicolo)listaFascicoli[0]).descrizione == ((DocsPAWA.DocsPaWR.Fascicolo)listaFascicoli[1]).descrizione)
                                                                                txt_DescFascicolo.Text = ((DocsPAWA.DocsPaWR.Fascicolo)listaFascicoli[0]).descrizione;
                                                                        }
                                                                        else
                                                                        {
                                                                            txt_DescFascicolo.Text = "";
                                                                        }
                                     */
                                }
                            }
                        }
                        #endregion FASCICOLO

                        #region RIFERIMENTO MITTENTE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.RIFERIMENTO_MITTENTE.ToString())
                        {
                            if (wws.isEnableRiferimentiMittente())
                            {
                                txt_rif_mittente.Text = aux.valore;
                            }

                        }
                        #endregion
                        #region CODICE_DESCRIZIONE_AMMINISTRAZIONE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.CODICE_DESCRIZIONE_AMMINISTRAZIONE.ToString())
                        {
                            if (!string.IsNullOrEmpty(aux.valore))
                                txt_codDesc.Text = aux.valore;
                        }
                        #endregion
                        #region ORDINAMENTO ASC/DESC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ORDER_DIRECTION.ToString()  && !grid)
                        {
                            if (!string.IsNullOrEmpty(aux.valore))
                            {
                                this.ddlOrderDirection.SelectedValue = aux.valore;
                            }

                        }
                        #endregion

                        #region ORDINAMENTO TIPO
                         else if (aux.argomento == DocsPaWR.FiltriDocumento.ORDER_DIRECTION.ToString() && !grid)
                        {
                            if (!string.IsNullOrEmpty(aux.nomeCampo))
                            {
                                if (this.ddlOrder.SelectedValue.Contains(aux.nomeCampo))
                                {
                                    this.ddlOrder.SelectedValue = aux.nomeCampo;
                                }
                            }
                        }
                        #endregion

                    }
                    catch (Exception)
                    {
                        throw new Exception("I criteri di ricerca non sono piu\' validi.");
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
                return false;
            }
        }

        protected void chk_mitt_dest_storicizzati_CheckedChanged(object sender, System.EventArgs e)
        { 
        try
            {
                setDescMittente(this.txt_codMit_E.Text, "Mit");
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
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
        private void InitializeComponent()
        {
            this.ddl_Ric_Salvate.SelectedIndexChanged += new System.EventHandler(this.ddl_Ric_Salvate_SelectedIndexChanged);
            this.btn_Canc_Ric.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Canc_Ric_Click);
            this.lb_reg_E.SelectedIndexChanged += new System.EventHandler(this.lb_reg_E_SelectedIndexChanged);
            //this.rbl_Reg_E.SelectedIndexChanged += new System.EventHandler(this.rbl_Reg_E_SelectedIndexChanged);
            this.ddl_numProt_E.SelectedIndexChanged += new System.EventHandler(this.ddl_numProt_E_SelectedIndexChanged);
            this.ddl_dataProt_E.SelectedIndexChanged += new System.EventHandler(this.ddl_dataProt_E_SelectedIndexChanged);
            this.ddl_dataScadenza_E.SelectedIndexChanged += new System.EventHandler(this.ddl_dataScadenza_SelectedIndexChanged);
            this.ddl_dataCreazione_E.SelectedIndexChanged += new System.EventHandler(this.ddl_dataCreazione_E_SelectedIndexChanged);
            this.ddl_dataStampa_E.SelectedIndexChanged += new System.EventHandler(this.ddl_dataStampa_E_SelectedIndexChanged);
            this.ddl_idDocumento_C.SelectedIndexChanged += new System.EventHandler(this.ddl_idDocumento_C_SelectedIndexChanged);
            this.txt_codMit_E.TextChanged += new System.EventHandler(this.txt_codMit_E_TextChanged);
            this.txt_CodFascicolo.TextChanged += new System.EventHandler(this.txt_CodFascicolo_TextChanged);
            //this.btn_Ricerca.Click += new System.EventHandler(this.btn_Ricerca_Click);
            this.btn_Ricerca.Click += new EventHandler(btn_Ricerca_Click);
            //   this.btn_salva.Click += new EventHandler(this.btn_salva_Click);
            this.mb_ConfirmDelete.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.mb_ConfirmDelete_GetMessageBoxResponse);
            this.ID = "f_Ricerca_E";
            //this.ddl_statiDoc.SelectedIndexChanged += new System.EventHandler(this.ddl_statiDoc_SelectedIndexChanged);
            this.imgFasc.Click += new System.Web.UI.ImageClickEventHandler(this.imgFasc_Click);
            this.enterKeySimulator.Click += new System.Web.UI.ImageClickEventHandler(this.enterKeySimulator_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.f_Ricerca_E_PreRender);
            //this.Unload += new System.EventHandler(this.Page_Unload);
            this.chk_mitt_dest_storicizzati.CheckedChanged += new System.EventHandler(this.chk_mitt_dest_storicizzati_CheckedChanged);
            this.btn_clear_fields.Click += new ImageClickEventHandler(this.CleanCorrFilters);

            // Se è attiva l'interoperabilità semplificata, viene aggiunta una voce che consente di
            // filtrare per ricevute Interoperabilità semplificata
            if (InteroperabilitaSemplificataManager.IsEnabledSimpInterop)
                this.rblFiltriAllegati.Items.Add(
                    new ListItem(
                        InteroperabilitaSemplificataManager.SearchItemDescriprion,
                        InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId));
        }

        protected void CleanCorrFilters(object sender, EventArgs e)
        {
            this.aofOwner.ClearFilters();
            this.aofAuthor.ClearFilters();
        }

        #endregion

        protected void ddl_dataScadenza_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Text = "";
            switch (this.ddl_dataScadenza_E.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataScadenza_E").Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataScadenza_E").Visible = false;
                    this.GetCalendarControl("txt_fineDataScadenza_E").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Visible = false;
                    this.lbl_fineDataScadenza_E.Visible = false;
                    this.lbl_initDataScadenza_E.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataScadenza_E").Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataScadenza_E").Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Enabled = true;
                    this.lbl_fineDataScadenza_E.Visible = true;
                    this.lbl_initDataScadenza_E.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataScadenza_E").Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataScadenza_E").Visible = false;
                    this.GetCalendarControl("txt_fineDataScadenza_E").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Visible = false;
                    this.lbl_fineDataScadenza_E.Visible = false;
                    this.lbl_initDataScadenza_E.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataScadenza_E").Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataScadenza_E").Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Enabled = false;
                    this.lbl_fineDataScadenza_E.Visible = true;
                    this.lbl_initDataScadenza_E.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataScadenza_E").Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataScadenza_E").Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Enabled = false;
                    this.lbl_fineDataScadenza_E.Visible = true;
                    this.lbl_initDataScadenza_E.Visible = true;
                    break;
            }
        }

        protected void ddl_dataCreazione_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Text = "";
            switch (this.ddl_dataCreazione_E.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataCreazione_E").Visible = true;
                    this.GetCalendarControl("txt_initDataCreazione_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataCreazione_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_finedataCreazione_E").Visible = false;
                    this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataCreazione_E").Visible = true;
                    this.GetCalendarControl("txt_initDataCreazione_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataCreazione_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_finedataCreazione_E").Visible = true;
                    this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Enabled = true;
                    this.lbl_dataCreazioneA.Visible = true;
                    this.lbl_dataCreazioneDa.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataCreazione_E").Visible = true;
                    this.GetCalendarControl("txt_initDataCreazione_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataCreazione_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_finedataCreazione_E").Visible = false;
                    this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataCreazione_E").Visible = true;
                    this.GetCalendarControl("txt_initDataCreazione_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_finedataCreazione_E").Visible = true;
                    this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Enabled = false;
                    this.lbl_dataCreazioneA.Visible = true;
                    this.lbl_dataCreazioneDa.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataCreazione_E").Visible = true;
                    this.GetCalendarControl("txt_initDataCreazione_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_finedataCreazione_E").Visible = true;
                    this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Enabled = false;
                    this.lbl_dataCreazioneA.Visible = true;
                    this.lbl_dataCreazioneDa.Visible = true;
                    break;
            }
        }

        protected void ddl_dataStampa_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Text = "";
            switch (this.ddl_dataStampa_E.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataStampa_E").Visible = true;
                    this.GetCalendarControl("txt_initDataStampa_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataStampa_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_finedataStampa_E").Visible = false;
                    this.GetCalendarControl("txt_finedataStampa_E").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Visible = false;
                    this.lbl_dataStampaA.Visible = false;
                    this.lbl_dataStampaDa.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataStampa_E").Visible = true;
                    this.GetCalendarControl("txt_initDataStampa_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataStampa_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_finedataStampa_E").Visible = true;
                    this.GetCalendarControl("txt_finedataStampa_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_finedataStampa_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Enabled = true;
                    this.lbl_dataStampaA.Visible = true;
                    this.lbl_dataStampaDa.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataStampa_E").Visible = true;
                    this.GetCalendarControl("txt_initDataStampa_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataStampa_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_finedataStampa_E").Visible = false;
                    this.GetCalendarControl("txt_finedataStampa_E").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Visible = false;
                    this.lbl_dataStampaA.Visible = false;
                    this.lbl_dataStampaDa.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataStampa_E").Visible = true;
                    this.GetCalendarControl("txt_initDataStampa_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_finedataStampa_E").Visible = true;
                    this.GetCalendarControl("txt_finedataStampa_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_finedataStampa_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Enabled = false;
                    this.lbl_dataStampaA.Visible = true;
                    this.lbl_dataStampaDa.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataStampa_E").Visible = true;
                    this.GetCalendarControl("txt_initDataStampa_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_finedataStampa_E").Visible = true;
                    this.GetCalendarControl("txt_finedataStampa_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_finedataStampa_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Enabled = false;
                    this.lbl_dataStampaA.Visible = true;
                    this.lbl_dataStampaDa.Visible = true;
                    break;
            }
        }

        protected void ddl_dataProt_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text = "";
            switch (this.ddl_dataProt_E.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProt_E").Visible = false;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = false;
                    this.lbl_finedataProt_E.Visible = false;
                    this.lbl_initdataProt_E.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Enabled = true;
                    this.lbl_finedataProt_E.Visible = true;
                    this.lbl_initdataProt_E.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt_E").Visible = false;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = false;
                    this.lbl_finedataProt_E.Visible = false;
                    this.lbl_initdataProt_E.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Enabled = false;
                    this.lbl_finedataProt_E.Visible = true;
                    this.lbl_initdataProt_E.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Enabled = false;
                    this.lbl_finedataProt_E.Visible = true;
                    this.lbl_initdataProt_E.Visible = true;
                    break;
            }
        }

        protected void ddl_numProt_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            txt_fineNumProt_E.Text = "";

            if (this.ddl_numProt_E.SelectedIndex == 0)
            {
                this.txt_fineNumProt_E.Visible = false;
                this.lblDAnumprot_E.Visible = false;
                this.lblAnumprot_E.Visible = false;
            }
            else
            {
                this.txt_fineNumProt_E.Visible = true;
                this.lblDAnumprot_E.Visible = true;
                this.lblAnumprot_E.Visible = true;
            }
        }

        /// <summary>
        /// pop up per la selezione dei fascicoli
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgFasc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
            if (fasc != null)
            {
                if (this.txt_CodFascicolo.Text != "" && this.txt_DescFascicolo.Text != "")
                {
                    if (fasc.tipo.Equals("G"))
                    {
                        Session.Add("FascSelezFascRap", fasc);
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + fasc.codice + "', 'N')</script>");
                    }
                    else
                    {
                        ///se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, fasc.idClassificazione, UserManager.getUtente(this).idAmministrazione);
                        string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "', 'N')</script>");
                    }
                }
            }
            else
            {
                if (this.txt_CodFascicolo.Text != "")
                {
                    DocsPaWR.Fascicolo[] listaFasc = getFascicolo(null);
                    if (listaFasc != null)
                    {
                        Session.Add("listaFascFascRapida", listaFasc);

                        switch (listaFasc.Length)//il codice corrisponde a un solo fascicolo
                        {
                            case 0:
                                {
                                    RegisterStartupScript("AlertNoFasc", "<script>alert('Attenzione, codice fascicolo non presente');</script>");
                                    this.txt_DescFascicolo.Text = "";
                                    this.txt_CodFascicolo.Text = "";
                                }
                                break;
                            case 1:
                                {
                                    if (listaFasc[0].tipo.Equals("G"))
                                    {
                                        Session.Add("FascSelezFascRap", listaFasc[0]);
                                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + listaFasc[0].codice + "', 'N')</script>");
                                    }
                                    else
                                    {
                                        ///se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
                                        string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "', 'N')</script>");
                                    }
                                }
                                break;
                            default:
                                {
                                    if (listaFasc[0].tipo.Equals("G"))
                                    {
                                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + listaFasc[0].codice + "', 'Y')</script>");
                                    }
                                    else
                                    {
                                        ///se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
                                        string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "', 'Y')</script>");
                                    }
                                }
                                break;
                        }
                    }
                }
                else
                {
                    if (!(Session["validCodeFasc"] != null && Session["validCodeFasc"].ToString() == "false"))
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + txt_CodFascicolo.Text + "', 'N')</script>");
                }
                //RegisterStartupScript("openModale","<script>ApriRicercaFascicoli('"+txt_CodFascicolo.Text+"', 'N')</script>");
            }
        }

        /// <summary>
        /// Metodo per il recupero del fascicolo da codice
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo[] getFascicolo(DocsPAWA.DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                string codiceFascicolo = txt_CodFascicolo.Text;
                listaFasc = FascicoliManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "R");
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
        /// Verifica se il dato passato è numerico
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns></returns>
        public bool IsNumber(string strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
                !objTwoDotPattern.IsMatch(strNumber) &&
                !objTwoMinusPattern.IsMatch(strNumber) &&
                objNumberPattern.IsMatch(strNumber);
        }

        public bool IsValidYear(string strYear)
        {
            Regex onlyNumberPattern = new Regex(@"\d{4}");
            return onlyNumberPattern.IsMatch(strYear);
        }

        private void btn_Ricerca_Click(object sender, EventArgs e)
        {
            try
            {
                this.SetPageOnCurrentContext();

                // Salvataggio dei filtri per la ricerca proprietario e creatore
                this.aofOwner.SaveFilters();
                this.aofAuthor.SaveFilters();
                
                if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null && !Page.IsPostBack)
                {
                    GridManager.CompileDdlOrderAndSetOrderFilterDocuments(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                }

               // ddl_Ric_Salvate.SelectedIndex = 0;

                //Nelle ricerche limitate il numero protocollo è obbligatorio
                if (isLimited)
                {
                    if (txt_initNumProt_E.Text.Trim() == String.Empty)
                    {
                        string s = "<script>alert('Il numero di protocollo è obbligatorio!');</script>";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", s);
                        string s1 = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_E.ID + "').focus();</SCRIPT>";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s1);
                        string s2 = "<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "loadRigth", s2);
                        return;
                    }

                    // fine controllo
                    //controllo validità anno inserito
                    if (tbAnnoProtocollo.Text.Trim() == String.Empty)
                    {
                        if (IsValidYear(tbAnnoProtocollo.Text.Trim()) == false)
                        {
                            string s = "<script>alert('Attenzione, l\\'anno è obbligatorio!');</script>";
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", s); ;
                            string s1 = "<SCRIPT language='javascript'>document.getElementById('" + tbAnnoProtocollo.ID + "').focus();</SCRIPT>";
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s1);
                            string s2 = "<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>";
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "loadRigth", s2);

                            return;
                        }
                    }
                }

                // controllo dei campi NUM. PROTOCOLLO numerici				
                if (txt_initNumProt_E.Text != "")
                {
                    if (IsNumber(txt_initNumProt_E.Text) == false)
                    {
                        Response.Write("<script>alert('Il numero di protocollo deve essere numerico!');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_E.ID + "').focus();</SCRIPT>";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }

                if (txt_fineNumProt_E.Text != "")
                {
                    if (IsNumber(txt_fineNumProt_E.Text) == false)
                    {
                        Response.Write("<script>alert('Il numero di protocollo deve essere numerico!');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_fineNumProt_E.ID + "').focus();</SCRIPT>";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                // fine controllo
                //controllo validità anno inserito
                if (tbAnnoProtocollo.Text != "")
                {
                    if (IsValidYear(tbAnnoProtocollo.Text) == false)
                    {
                        string s = "<script>alert('Formato anno non corretto !');</script>";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", s); ;
                        string s1 = "<SCRIPT language='javascript'>document.getElementById('" + tbAnnoProtocollo.ID + "').focus();</SCRIPT>";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s1);
                        string s2 = "<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "loadRigth", s2);
                        return;
                    }
                }
                //fine controllo validità anno inserito --- 14/01/2005

                //Controllo intervallo date
                if (this.ddl_dataProt_E.SelectedIndex == 1)
                {
                    if (Utils.isDate(this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text, this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date Data Protocollo!');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProt_E").txt_Data.ID + "').focus();</SCRIPT>";
                        //Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                if (this.ddl_dataScadenza_E.SelectedIndex == 1)
                {
                    if (Utils.isDate(this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text, this.GetCalendarControl("txt_fineDataScadenza_E").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date Data Scadenza!');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.ID + "').focus();</SCRIPT>";
                        //Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                if (this.ddl_dataCreazione_E.SelectedIndex == 1)
                {
                    if (Utils.isDate(this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text, this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date Data Creazione!');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.ID + "').focus();</SCRIPT>";
                        //Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                //Fine controllo intervallo date

                // Controllo lunghezza oggetto inserito
                if (this.txt_oggetto.Text.Trim() != string.Empty && !FullTextSearch.Configurations.CheckTextMinLenght(this.txt_oggetto.Text))
                {
                    string message = string.Format("<script>alert('Per ricercare un oggetto è necessario immettere almeno {0} caratteri');</script>", FullTextSearch.Configurations.FullTextMinTextLenght.ToString());
                    Response.Write(message);

                    this.txt_oggetto.Focus();
                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                    return;
                }
                // Controllo lunghezza descrizione mittente/destinatario inserito
                if (this.txt_descrMit_E.Text.Trim() != string.Empty && !FullTextSearch.Configurations.CheckTextMinLenght(this.txt_descrMit_E.Text))
                {
                    string message = string.Format("<script>alert('Per ricercare un mittente/destinatario è necessario immettere almeno {0} caratteri');</script>", FullTextSearch.Configurations.FullTextMinTextLenght.ToString());
                    Response.Write(message);

                    this.txt_descrMit_E.Focus();
                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                    return;
                }
                // Controllo sul tipo di ricerca richiesto (ex Tipo Protocollo)
                bool controllo = false;
                for (int i = 0; i < this.cbl_archDoc_E.Items.Count; i++)
                {
                    if (this.cbl_archDoc_E.Items[i].Selected)
                        controllo = true;
                }
                if (!controllo)
                {
                    Response.Write("<script>alert('Inserire almeno un tipo di ricerca');</script>");
                    string s = "<SCRIPT language='javascript'>document.getElementById('" + cbl_archDoc_E.ID + "').focus();</SCRIPT>";
                    RegisterStartupScript("focus", s);
                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                    return;
                }

                if (RicercaEstesa())
                {
                    //controllo se è stato scelto il tipo documento (Tutti non è un criterio di ricerca)
                    int numCriteri = 0;
                    //if (this.rb_archDoc_E.SelectedItem.Value == "T")
                    if (this.cbl_archDoc_E.Items[0].Selected && this.cbl_archDoc_E.Items[1].Selected && this.cbl_archDoc_E.Items[2].Selected && this.cbl_archDoc_E.Items[3].Selected)
                        numCriteri = 1;

                    if (qV[0] == null || qV[0].Length <= numCriteri)
                    {
                        Response.Write("<script>alert('Inserire almeno un criterio di ricerca');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_E.ID + "').focus();</SCRIPT>";
                        RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }

                    // inserisco un controllo sulle stopword nel caso in cui sia abilitata la chiave use_text_index
                    string valoreChiave = utils.InitConfigurationKeys.GetValue("0", "USE_TEXT_INDEX");
                    if (!string.IsNullOrEmpty(valoreChiave) && (valoreChiave.Equals("1") || (valoreChiave.Equals("2"))))
                    {
                        for (int i = 0; i < qV[0].Length; i++)
                        {
                            if (qV[0][i].argomento.Equals(DocsPaWR.FiltriDocumento.OGGETTO.ToString()) && !string.IsNullOrEmpty(qV[0][i].valore))
                            {
                                if (qV[0][i].valore.StartsWith("%"))
                                {
                                    Response.Write("<script>alert('Il parametro di ricerca non può iniziare con il carattere %');</script>");
                                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_E.ID + "').focus();</SCRIPT>";
                                    RegisterStartupScript("focus", s);
                                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                    return;
                                }
                                //if (qV[0][i].valore.Contains("%&&") || qV[0][i].valore.Contains("&&%"))
                                //{
                                //    Response.Write("<script>alert('La combinazione degli operatori && e % non è supportata');</script>");
                                //    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_E.ID + "').focus();</SCRIPT>";
                                //    RegisterStartupScript("focus", s);
                                //    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                //    return;
                                //}
                                //string stopWord = DocumentManager.verificaStopWord(this, qV[0][i].valore);
                                //if (!string.IsNullOrEmpty(stopWord))
                                //{
                                //    string messaggio = InitMessageXml.getInstance().getMessage("STOP_WORD");
                                //    Response.Write("<script>alert('" + String.Format(messaggio,stopWord) + "');</script>");
                                //    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_E.ID + "').focus();</SCRIPT>";
                                //    RegisterStartupScript("focus", s);
                                //    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                //    return;
                                //}
                            }
                            if (qV[0][i].argomento.Equals(DocsPaWR.FiltriDocumento.MITT_DEST.ToString()) && !string.IsNullOrEmpty(qV[0][i].valore))
                            {
                                if (qV[0][i].valore.StartsWith("%"))
                                {
                                    Response.Write("<script>alert('Il parametro di ricerca non può iniziare con il carattere %.');</script>");
                                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_E.ID + "').focus();</SCRIPT>";
                                    RegisterStartupScript("focus", s);
                                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                    return;
                                }
                                //if (qV[0][i].valore.Contains("%&&") || qV[0][i].valore.Contains("&&%"))
                                //{
                                //    Response.Write("<script>alert('La combinazione degli operatori && e % non è supportata');</script>");
                                //    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_E.ID + "').focus();</SCRIPT>";
                                //    RegisterStartupScript("focus", s);
                                //    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                //    return;
                                //}
                                //string stopWord = DocumentManager.verificaStopWord(this, qV[0][i].valore);
                                //if (!string.IsNullOrEmpty(stopWord))
                                //{
                                //    string messaggio = InitMessageXml.getInstance().getMessage("STOP_WORD");
                                //    Response.Write("<script>alert('" + String.Format(messaggio, stopWord) + "');</script>");
                                //    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_E.ID + "').focus();</SCRIPT>";
                                //    RegisterStartupScript("focus", s);
                                //    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                //    return;
                                //}
                            }
                        }
                    }

                    schedaRicerca.FiltriRicerca = qV;
                    DocumentManager.setFiltroRicDoc(this, qV);
                    DocumentManager.removeDatagridDocumento(this);
                    DocumentManager.removeListaDocProt(this);
                    Session.Remove("dictionaryCorrispondente");

                    if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=estesa&tabRes=estesa';</script>");
                    else
                        //Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDoc.aspx';</script>");
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?tabRes=estesa';</script>");
                    //    Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDoc.aspx';</script>");
                }

                ViewState["new_search"] = "true";
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
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
        /// Metodo per recuperare i fascicoli dal codice digitato dall'utente
        /// </summary>
        /// <returns></returns>
        private ArrayList getFascicoli()
        {
            ArrayList listaFascicoli = new ArrayList();

            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                string codiceFascicolo = txt_CodFascicolo.Text;
                listaFascicoli = FascicoliManager.getFascicoloDaCodice3(this, codiceFascicolo);
                FascicoliManager.setFascicoliSelezionati(this, listaFascicoli);
            }
            if (listaFascicoli.Count != 0)
            {
                //txt_DescFascicolo.Text = "";
                return listaFascicoli;
            }
            else
            {
                txt_DescFascicolo.Text = "";
                return null;
            }
        }

        private string getInStringChild(DocsPAWA.DocsPaWR.Folder folder, string inSubFolder)
        {
            if (folder.childs != null && folder.childs.Length > 0)
            {
                for (int i = 0; i < folder.childs.Length; i++)
                {
                    inSubFolder += ", " + folder.childs[i].systemID;
                    inSubFolder = getInStringChild(folder.childs[i], inSubFolder);
                }
            }
            return inSubFolder;
        }
        
        private void setDescMittente(string codiceRubrica, string tipoMit)
        {
            Session.Remove("multiCorr");
            DocsPaWR.ElementoRubrica[] listaCorr = null;
            DocsPaWR.Corrispondente corr = null;
            DocsPaWR.RubricaCallType calltype;
            calltype = DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_ESTESA;
            bool codiceEsatto = (chk_mitt_dest_storicizzati.Checked) ? false : true;
            if (!codiceRubrica.Equals(""))
                listaCorr = UserManager.getElementiRubricaMultipli(this, codiceRubrica, calltype, codiceEsatto);
            if (tipoMit == "Mit")
            {
                if (listaCorr != null && listaCorr.Length > 0)
                {
                    if (listaCorr.Length == 1)
                    {
                        if (!string.IsNullOrEmpty(listaCorr[0].systemId))
                        {
                            //corr = UserManager.getCorrispondenteRubrica(this.Page, listaCorr[0].codice, DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_ESTESA);
                            corr = UserManager.getCorrispondenteBySystemID(this.Page, listaCorr[0].systemId);
                        }
                        else
                        {
                            corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, listaCorr[0].codice);
                        }

                    }
                    else
                    {
                        if (tipoMit == "Mit")
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "multiCorr", "ApriFinestraMultiCorrispondenti();", true);
                        }
                        Session.Add("multiCorr", listaCorr);
                        return;
                    }
                }


                if (corr != null)
                {
                    if (!chk_mitt_dest_storicizzati.Checked && !string.IsNullOrEmpty(corr.dta_fine))
                    {
                        this.txt_codMit_E.Text = "";
                        this.txt_descrMit_E.Text = "";
                        this.hd_systemIdMit_Est.Value = "";
                    }
                    else
                    {
                        txt_codMit_E.Text = corr.codiceRubrica;
                        this.txt_descrMit_E.Text = UserManager.getDecrizioneCorrispondenteSemplice(corr);
                        this.hd_systemIdMit_Est.Value = corr.systemId;
                    }
                }

                else
                {
                    this.txt_codMit_E.Text = "";
                    this.txt_descrMit_E.Text = "";
                    this.hd_systemIdMit_Est.Value = "";
                }
            }
        }

        private void txt_codMit_E_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                setDescMittente(this.txt_codMit_E.Text, "Mit");
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void enterKeySimulator_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.btn_Ricerca_Click(null, null);
        }

        /*		private void txt_CodFascicolo_TextChanged(object sender, System.EventArgs e)
                {
                    if (this.txt_CodFascicolo.Text.Equals(""))
                    {
                        txt_DescFascicolo.Text="";
                        return;
                    }

                    ArrayList fascicoli = getFascicoli();

                    if (fascicoli == null || fascicoli.Count == 0)
                    {
                        txt_CodFascicolo.Text = "";
                        Page.RegisterStartupScript("", "<script>alert('Codice Fascicolo non Presente')</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_CodFascicolo.ID + "').focus();</SCRIPT>";
                        Page.RegisterStartupScript("focus", s);
                        txt_DescFascicolo.Text = "";
                        return;
                    }

                    if (fascicoli != null && fascicoli.Count == 2)
                    {
                        if (((DocsPAWA.DocsPaWR.Fascicolo)fascicoli[0]).descrizione == ((DocsPAWA.DocsPaWR.Fascicolo)fascicoli[1]).descrizione)
                            txt_DescFascicolo.Text = ((DocsPAWA.DocsPaWR.Fascicolo)fascicoli[0]).descrizione;
                    }
                    else
                    {
                        txt_DescFascicolo.Text = "";
                    }
                }
        */
        /*   private void txt_CodFascicolo_TextChanged(object sender, System.EventArgs e)
           {
               Session["validCodeFasc"] = "true";
               //inizialmente svuoto il campo e pulisco la sessione
               FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
               this.txt_DescFascicolo.Text = "";
               //
               if (this.txt_CodFascicolo.Text.Equals(""))
               {
                   txt_DescFascicolo.Text = "";
                   return;
               }

               DocsPaWR.Fascicolo[] listaFasc = getFascicolo(null);

               if (listaFasc != null)
               {
                   if (listaFasc.Length > 0)
                   {
                       //caso 1: al codice digitato corrisponde un solo fascicolo
                       if (listaFasc.Length == 1)
                       {
                           txt_DescFascicolo.Text = listaFasc[0].descrizione;
                           txt_CodFascicolo.Text = listaFasc[0].codice;
                           //metto il fascicolo in sessione
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
                           //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                           //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'N');");
                           //RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codClassifica + "', 'N')</script>");
                       }
                       else
                       {
                           //Hashtable hashRegistriNodi = getRegistriNodi(listaFasc);
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
                           //Page.RegisterStartupScript("openListaFasc","<SCRIPT>ApriSceltaFascicolo();</SCRIPT>");
                           //Session.Add("hasRegistriNodi",hasRegistriNodi);

                           //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                           //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'Y');");
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
                       //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                       //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', '');");
                   }
               }
           }*/

        private void txt_CodFascicolo_TextChanged(object sender, System.EventArgs e)
        {
            Session["validCodeFasc"] = "true";
            //inizialmente svuoto il campo e pulisco la sessione
            FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
            this.txt_DescFascicolo.Text = "";
            //
            if (this.txt_CodFascicolo.Text.Equals(""))
            {
                txt_DescFascicolo.Text = "";
                return;
            }
            //su DocProfilo devo cercare senza condizione sul registro.
            //Basta che il fascicolo sia visibile al ruolo loggato


            if (this.txt_CodFascicolo.Text.IndexOf("//") > -1)
            {

                string codice = string.Empty;
                string descrizione = string.Empty;

                DocsPaWR.Fascicolo SottoFascicolo = getFolder(null, ref codice, ref descrizione);
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
            }
            else
            {
                DocsPAWA.DocsPaWR.Registro reg = null;

                //Se il ruolo ha un solo registro non serve aprire la popup (Modifica 28/07/2010) bug inps 3471
                DocsPAWA.DocsPaWR.Registro[] register = UserManager.GetRegistriByRuolo(this, (UserManager.getRuolo(this)).systemId);
                if (register != null)
                {
                    if (register.Length == 1)
                    {
                        reg = register[0];
                    }
                }

                DocsPaWR.Fascicolo[] listaFasc = getFascicolo(reg);

                if (listaFasc != null)
                {
                    if (listaFasc.Length > 0)
                    {
                        //caso 1: al codice digitato corrisponde un solo fascicolo
                        if (listaFasc.Length == 1)
                        {
                            txt_DescFascicolo.Text = listaFasc[0].descrizione;
                            //metto il fascicolo in sessione
                            //FascicoliManager.setFascicoloSelezionato(this,fasc);
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
                            //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                            //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'N');");
                        }
                        else
                        {
                            //Hashtable hashRegistriNodi = getRegistriNodi(listaFasc);
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
                            //Page.RegisterStartupScript("openListaFasc","<SCRIPT>ApriSceltaFascicolo();</SCRIPT>");
                            //Session.Add("hasRegistriNodi",hasRegistriNodi);

                            //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                            //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'Y');");
                            RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli2('" + codClassifica + "', 'Y')</script>");

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
                        //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                        //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', '');");
                    }
                }
            }
        }

        /// <summary>
        /// Metodo per il recupero del sottofascicolo da codice fascicolo e descrizione sottofascicolo
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        /// 
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

        private void txt_DescFascicolo_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void f_Ricerca_E_PreRender(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                // Caricamento delle combo con le informazioni sull'ordinamento
                GridManager.CompileDdlOrderAndSetOrderFilterDocuments(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                //visibilità filtro Allegati --> sistemi esterni
                string filterExternalSystem = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_FILTRO_ALLEGATI_ESTERNI");
                if (string.IsNullOrEmpty(filterExternalSystem) || (!filterExternalSystem.Equals("1")))
                {
                    foreach (ListItem f in rblFiltriAllegati.Items)
                    {
                        if (f.Value.Equals("esterni"))
                        {
                            rblFiltriAllegati.Items.Remove(f);
                            break;
                        }
                    }
                }
            }

            DocsPAWA.DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
            if (fasc != null)
            {
                this.txt_CodFascicolo.Text = fasc.codice;
                this.txt_DescFascicolo.Text = fasc.descrizione;
            }

            DocsPAWA.DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteSelezionato(this);
            if (corr != null)
            {
                this.txt_codMit_E.Text = corr.codiceRubrica;
                this.txt_descrMit_E.Text = corr.descrizione;
            }

            if (Session["dictionaryCorrispondente"] != null)
            {
                dic_Corr = (Dictionary<string, Corrispondente>)Session["dictionaryCorrispondente"];
                if (dic_Corr != null && dic_Corr.ContainsKey("ricEst") && dic_Corr["ricEst"] != null)
                {
                    txt_codMit_E.Text = dic_Corr["ricEst"].codiceRubrica;
                    this.txt_descrMit_E.Text = dic_Corr["ricEst"].descrizione;
                    this.hd_systemIdMit_Est.Value = dic_Corr["ricEst"].systemId;
                }
                dic_Corr.Remove("ricEst");
                Session.Add("dictionaryCorrispondente", dic_Corr);
            }

            if (DocsPAWA.popup.ricercaFascicoli.RicercaFascicoliSessionMng.IsLoaded(this))
            {
                // DocsPAWA.popup.ricercaFascicoli.RicercaFascicoliSessionMng.SetAsNotLoaded(this);
                DocsPAWA.popup.ricercaFascicoli.RicercaFascicoliSessionMng.ClearSessionData(this);
            }

            if (UserManager.ruoloIsAutorized(this, "FASC_INS_DOC"))
            {
                setFascicolazioneRapida();
                this.ClearResourcesRicercaFascicoliFascRapida();
            }


            this.GetCalendarControl("txt_initDataProt_E").txt_Data.Attributes.Add("onKeyUp", "scriviData('txt_initDataProt_E','/');");
            HtmlImage btn_rubrica_E = (HtmlImage)FindControl("btn_rubrica_E");
            string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
            if (!this.chk_mitt_dest_storicizzati.Checked)
                btn_rubrica_E.Attributes["onClick"] = "_ApriRubrica('ric_stor');";
            else
            {
                if (use_new_rubrica != "1")
                    btn_rubrica_E.Attributes["onClick"] = "ApriRubrica('ric_E','');";
                else
                    btn_rubrica_E.Attributes["onClick"] = "_ApriRubrica('ric_estesa');";
            }
            if (Session["PredispostiInToDoList"] != null)
            {
                Session.Remove("PredispostiInToDoList");
                if (RicercaEstesa())
                {
                    //controllo se è stato scelto il tipo documento (Tutti non è un criterio di ricerca)
                    int numCriteri = 0;
                    //if (this.rb_archDoc_E.SelectedItem.Value == "T")
                    if (this.cbl_archDoc_E.Items[0].Selected && this.cbl_archDoc_E.Items[1].Selected && this.cbl_archDoc_E.Items[2].Selected && this.cbl_archDoc_E.Items[3].Selected)
                        numCriteri = 1;

                    if (qV[0] == null || qV[0].Length <= numCriteri)
                    {
                        Response.Write("<script>alert('Inserire almeno un criterio di ricerca');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_E.ID + "').focus();</SCRIPT>";
                        RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }

                    schedaRicerca.FiltriRicerca = qV;
                    DocumentManager.setFiltroRicDoc(this, qV);
                    DocumentManager.removeDatagridDocumento(this);
                    DocumentManager.removeListaDocProt(this);

                    if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=estesa&tabRes=estesa';</script>");
                    else
                        //Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDoc.aspx';</script>");
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?tabRes=estesa';</script>");
                }
            }

            string new_search = string.Empty;
            if (ViewState["new_search"] != null)
            {
                new_search = ViewState["new_search"] as string;
                ViewState["new_search"] = null;
            }

            if (change_from_grid && string.IsNullOrEmpty(new_search))
            {

                if (RicercaEstesa())
                {
                    schedaRicerca.FiltriRicerca = qV;
                    DocumentManager.setFiltroRicDoc(this, qV);
                    DocumentManager.removeDatagridDocumento(this);
                    DocumentManager.removeListaDocProt(this);
                    change_from_grid = false;

                    string altro = string.Empty;

                    if (!string.IsNullOrEmpty(this.numResult) && this.numResult.Equals("0"))
                    {
                        altro = "&noRic=1";
                    }

                    if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                    {
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=estesa&tabRes=estesa" + altro + "';</script>");
                        //       ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=estesa&tabRes=estesa" + altro + "';", true);
                    }
                    else
                    {
                        //     ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?from=estesa&tabRes=estesa" + altro + "';", true);
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?from=estesa&tabRes=estesa" + altro + "';</script>");
                    }
                }
            }

        }

        public void tastoInvio()
        {
            Utils.DefaultButton(this, ref txt_initNumProt_E, ref btn_Ricerca);
            Utils.DefaultButton(this, ref txt_fineNumProt_E, ref btn_Ricerca);
            Utils.DefaultButton(this, ref tbAnnoProtocollo, ref btn_Ricerca);
            Utils.DefaultButton(this, ref txt_oggetto, ref btn_Ricerca);
            Utils.DefaultButton(this, ref txt_numOggetto, ref btn_Ricerca);
            Utils.DefaultButton(this, ref this.GetCalendarControl("txt_initDataProt_E").txt_Data, ref btn_Ricerca);
            Utils.DefaultButton(this, ref this.GetCalendarControl("txt_fineDataProt_E").txt_Data, ref btn_Ricerca);
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


        protected void btn_salva_Click(object sender, EventArgs e)
        {
            if (RicercaEstesa())
            {
                // Impostazione del filtro utilizzato
                GridManager.SetSearchFilter(this.ddlOrder.SelectedItem.Text, this.ddlOrderDirection.SelectedValue);

                schedaRicerca.FiltriRicerca = qV;
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

        private void ddl_Ric_Salvate_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (ddl_Ric_Salvate.SelectedIndex == 0)
            {
                if (GridManager.IsRoleEnabledToUseGrids())
                {
                    GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);
                    GridManager.CompileDdlOrderAndSetOrderFilterDocuments(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
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
                    Grid tempGrid = GridManager.GetGridFromSearchId(schedaRicerca.gridId, GridTypeEnumeration.Document);
                    if (tempGrid != null)
                    {
                        GridManager.SelectedGrid = tempGrid;
                        GridManager.CompileDdlOrderAndSetOrderFilterDocuments(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                        // Reperimento del filtro da utilizzare per la griglia
                        List<FiltroRicerca> filterList = GridManager.GetOrderFilterForDocument(this.ddlOrder.SelectedValue, this.ddlOrderDirection.SelectedValue);

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

                    if (PopulateField(qV, true))
                    {
                        DocumentManager.setFiltroRicDoc(this, qV);
                        DocumentManager.removeDatagridDocumento(this);
                        DocumentManager.removeListaDocProt(this);

                        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                            Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=estesa&tabRes=estesa';</script>");
                        else
                            //Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDoc.aspx';</script>");
                            Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?tabRes=estesa';</script>");
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

        private void mb_ConfirmDelete_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                try
                {

                    schedaRicerca.Cancella(Int32.Parse(ddl_Ric_Salvate.SelectedValue));
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

        #region Gestione CallContext

        /// <summary>
        /// Impostazione numero pagina corrente del contesto di ricerca
        /// </summary>
        private void SetPageOnCurrentContext()
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            currentContext.PageNumber = 1;
        }

        #endregion

        #region Gestione ricerca Grigio / Allegato

        /// <summary>
        /// Indica se la profilazione dell'allegato è abilitata o meno
        /// </summary>
        protected bool IsEnabledProfilazioneAllegato
        {
            get
            {
                const string VIEW_STATE_KEY = "IsEnabledProfilazioneAllegato";

                bool isEnabled = false;

                if (this.ViewState[VIEW_STATE_KEY] != null)
                {
                    isEnabled = Convert.ToBoolean(this.ViewState[VIEW_STATE_KEY]);
                }
                else
                {
                    DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
                    isEnabled = ws.IsEnabledProfilazioneAllegati();
                    this.ViewState.Add(VIEW_STATE_KEY, isEnabled);
                }

                return isEnabled;
            }
        }

        #endregion

        /// <summary>
        /// Prende i dati esistenti per le etichette dei protocolli (Inserita da Fabio)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void getLettereProtocolli()
        {

            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
            string idAmm = cr.idAmministrazione;
            this.etichette = wws.getEtichetteDocumenti(Safe, idAmm);
            if ((etichette[0].Etichetta).Length > 3)
            {
                this.opArr.Text = ((etichette[0].Etichetta).Substring(0, 3)) + "."; //Valore A
            }
            else
            {
                this.opArr.Text = etichette[0].Etichetta;
            }
            if ((etichette[1].Etichetta).Length > 3)
            {
                //CASO PER INFORMATICA TRENTINA PER LASCIARE 4 CARATTERI (Part.)
                if (etichette[1].Etichetta.Equals("Partenza"))
                {
                    this.opPart.Text = "Part.";
                }
                else
                {
                    this.opPart.Text = ((etichette[1].Etichetta).Substring(0, 3)) + "."; //Valore P
                }
            }
            else
            {
                this.opPart.Text = etichette[1].Etichetta;
            }
            if ((etichette[2].Etichetta).Length > 3)
            {
                this.opInt.Text = ((etichette[2].Etichetta).Substring(0, 3)) + ".";//Valore I
            }
            else
            {
                this.opInt.Text = etichette[2].Etichetta;
            }
            if ((etichette[3].Etichetta).Length > 3)
            {
                this.opGrigio.Text = ((etichette[3].Etichetta).Substring(0, 3)) + ".";//Valore G
            }
            else
            {
                this.opGrigio.Text = etichette[3].Etichetta;
            }
            if ((etichette[4].Etichetta).Length > 3)
            {
                this.opAll.Text = ((etichette[4].Etichetta).Substring(0, 3)) + ".";//Valore ALL
            }
            else
            {
                this.opAll.Text = etichette[4].Etichetta;
            }

        }

        public void setFascicolazioneRapida()
        {
            DocsPaWR.Fascicolo fascRap = new Fascicolo();
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
                    this.txt_DescFascicolo.Text = descrFascRapida;
                }
                else
                {
                    this.txt_CodFascicolo.Text = fascRap.codice;
                    this.txt_DescFascicolo.Text = fascRap.descrizione;
                }
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
            //Rimuove le variabili usate per la gestione della pagina scegliFascicoloFascRapida.aspx
            if (DocsPAWA.popup.scegliFascicoloFascRapida.ListaFascicoliSessionMng.IsLoaded(this))
            {
                DocsPAWA.popup.scegliFascicoloFascRapida.ListaFascicoliSessionMng.SetAsNotLoaded(this);
                DocsPAWA.popup.scegliFascicoloFascRapida.ListaFascicoliSessionMng.ClearSessionData(this);
            }
        }

        protected void ModifyRapidSearch_Click(object sender, EventArgs e)
        {
            if (RicercaEstesa())
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
            schedaRicerca = new SchedaRicerca(KEY_SCHEDA_RICERCA, userHome, userRuolo, this);
            Session[SchedaRicerca.SESSION_KEY] = schedaRicerca;

            schedaRicerca.Pagina = this;

            ddl_idDocumento_C.SelectedIndex = 0;
            ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
            txt_initIdDoc_C.Text = "";

            //rb_archDoc_E.SelectedValue = "T";
            //for (int i = 0; i < this.cbl_archDoc_E.Items.Count; i++)
            //{
            //    this.cbl_archDoc_E.Items[i].Selected = true;
            //}

            foreach (ListItem i in lb_reg_E.Items)
                i.Selected = true;

            ddl_numProt_E.SelectedIndex = 0;
            ddl_numProt_E_SelectedIndexChanged(null, new System.EventArgs());
            txt_initNumProt_E.Text = "";

            tbAnnoProtocollo.Text = DateTime.Now.Year.ToString();

            ddl_dataProt_E.SelectedIndex = 0;
            ddl_dataProt_E_SelectedIndexChanged(null, new System.EventArgs());
            this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text = "";

            ddl_dataScadenza_E.SelectedIndex = 0;
            ddl_dataScadenza_SelectedIndexChanged(null, new System.EventArgs());
            this.GetCalendarControl("txt_initDataScadenza_E").txt_Data.Text = "";

            ddl_dataCreazione_E.SelectedIndex = 0;
            ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
            this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text = "";

            txt_oggetto.Text = "";

            txt_codMit_E.Text = "";
            txt_descrMit_E.Text = "";

            txt_numOggetto.Text = "";

            txt_CodFascicolo.Text = "";
            txt_DescFascicolo.Text = "";
            FascicoliManager.removeFascicoliSelezionati();
            FascicoliManager.removeFascicoloSelezionatoFascRapida();

            //DATA STAMPA
            ddl_dataStampa_E.SelectedIndex = 0;
            this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Text = "";
            this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Text = "";
            this.GetCalendarControl("txt_finedataStampa_E").Visible = false;

            //UserManager.removeCreatoreSelezionato(this.Page);
            //this.Creatore.Clear();
            if (this.cbl_archDoc_E.Items.FindByValue("A") != null)
            {
                this.cbl_archDoc_E.Items.FindByValue("A").Selected = true;
            }
            if (this.cbl_archDoc_E.Items.FindByValue("P") != null)
            {
                this.cbl_archDoc_E.Items.FindByValue("P").Selected = true;
            }
            if(this.cbl_archDoc_E.Items.FindByValue("I")!=null)
            {
                 this.cbl_archDoc_E.Items.FindByValue("I").Selected = true;
            }
            if (this.cbl_archDoc_E.Items.FindByValue("G") != null)
            {
                this.cbl_archDoc_E.Items.FindByValue("G").Selected = true;
            }
            if (this.cbl_archDoc_E.Items.FindByValue("Pr") != null)
            {
                this.cbl_archDoc_E.Items.FindByValue("Pr").Selected = false;
            }
            if (this.cbl_archDoc_E.Items.FindByValue("ALL") != null)
            {
                this.cbl_archDoc_E.Items.FindByValue("ALL").Selected = false;
            }
            if (this.cbl_archDoc_E.Items.FindByValue("R") != null)
            {
                this.cbl_archDoc_E.Items.FindByValue("R").Selected = false;
            }

            this.ddl_Ric_Salvate.SelectedIndex = 0;

            if (this.aofAuthor.Visible)
                this.aofAuthor.DeleteFilters();

            if (this.aofOwner.Visible)
                this.aofOwner.DeleteFilters();

            GridManager.CompileDdlOrderAndSetOrderFilterDocuments(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);

            if (p_cod_amm.Visible)
            {
                this.txt_codDesc.Text = string.Empty;
            }
        }
    }
}
