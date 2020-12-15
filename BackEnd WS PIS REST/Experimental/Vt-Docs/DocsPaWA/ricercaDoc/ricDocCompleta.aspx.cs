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
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using System.Collections.Generic;
using System.Linq;

namespace DocsPAWA.ricercaDoc
{
    /// <summary>
    /// Summary description for ricDocCompleta.
    /// </summary>
    public class f_Ricerca_C : DocsPAWA.CssPage
    {
        #region variabili codice
        protected System.Web.UI.WebControls.RadioButtonList rbl_Reg_C;
        protected System.Web.UI.WebControls.DropDownList ddl_numProt_C;
        protected System.Web.UI.WebControls.Label lblDAnumprot_C;
        protected System.Web.UI.WebControls.TextBox txt_initNumProt_C;
        protected System.Web.UI.WebControls.TextBox txt_fineNumProt_C;
        protected System.Web.UI.WebControls.DropDownList ddl_dataProt_C;
        // protected DocsPaWebCtrlLibrary.DateMask txt_initDataProt_C;
        protected DocsPAWA.UserControls.Calendar txt_initDataProt_C;
        // protected DocsPaWebCtrlLibrary.DateMask txt_fineDataProt_C;
        protected DocsPAWA.UserControls.Calendar txt_fineDataProt_C;
        protected System.Web.UI.WebControls.Label lblAnumprot_C;
        protected System.Web.UI.WebControls.Label lblSearch;
        protected System.Web.UI.WebControls.Label lbl_initdataProt_C;
        protected System.Web.UI.WebControls.Label lbl_finedataProt_C;
        protected System.Web.UI.WebControls.TextBox txt_codMitt_C;
        protected System.Web.UI.WebControls.TextBox txt_descrMitt_C;
        protected System.Web.UI.WebControls.TextBox txt_codMittInter_C;
        protected System.Web.UI.WebControls.TextBox txt_descrMittInter_C;
        protected System.Web.UI.WebControls.TextBox txt_numProtMitt_C;
        protected System.Web.UI.WebControls.DropDownList ddl_dataProtMitt_C;
        protected System.Web.UI.WebControls.Label lbl_initdataProtMitt_C;
        // protected DocsPaWebCtrlLibrary.DateMask txt_initDataProtMitt_C;
        protected DocsPAWA.UserControls.Calendar txt_initDataProtMitt_C;
        protected System.Web.UI.WebControls.Label lbl_finedataProtMitt_C;
        //protected DocsPaWebCtrlLibrary.DateMask txt_fineDataProtMitt_C;
        protected DocsPAWA.UserControls.Calendar txt_fineDataProtMitt_C;
        protected System.Web.UI.WebControls.RadioButtonList rb_archDoc_C;
        protected System.Web.UI.WebControls.ListBox lb_reg_C;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_systemIdMit_Com;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_systemIdMitInt;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_systemIdUffRef;
        protected DocsPAWA.DocsPaWR.InfoUtente Safe;
        protected string enableUfficioRef;
        protected DocsPaWebService wws = new DocsPaWebService();
        private bool isSavedSearch = false;
        private const string KEY_SCHEDA_RICERCA = "RicercaDocCompleta";
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
        protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
        protected System.Web.UI.WebControls.TextBox txt_oggetto;
        protected System.Web.UI.WebControls.ImageButton btn_protocolla_P;
        protected System.Web.UI.WebControls.ImageButton btn_RubrDest_P;
        protected System.Web.UI.WebControls.ImageButton btn_selezionaParoleChiave;
        protected System.Web.UI.WebControls.Button btn_ricerca;
        protected System.Web.UI.WebControls.Label lbl_nomeF;
        protected System.Web.UI.WebControls.Label lbl_cognomeF;
        protected System.Web.UI.WebControls.TextBox txt_nomeFirma_C;
        protected System.Web.UI.WebControls.TextBox txt_cognomeFirma_C;
        protected System.Web.UI.WebControls.DropDownList ddl_dataArrivo_C;
        // protected DocsPaWebCtrlLibrary.DateMask txt_initDataArrivo_C;
        protected DocsPAWA.UserControls.Calendar txt_initDataArrivo_C;
        protected System.Web.UI.WebControls.Label lbl_fineDataArrivo_C;
        // protected DocsPaWebCtrlLibrary.DateMask txt_fineDataArrivo_C;
        protected DocsPAWA.UserControls.Calendar txt_fineDataArrivo_C;
        protected System.Web.UI.WebControls.Label lbl_initDataArrivo_C;
        protected System.Web.UI.WebControls.ListBox ListParoleChiave;
        protected System.Web.UI.WebControls.TextBox txt_numOggetto;
        protected System.Web.UI.WebControls.Panel panel_numOgg_commRef;
        protected System.Web.UI.WebControls.TextBox txt_commRef;
        //protected System.Web.UI.WebControls.TextBox txt_note;
        protected UserControls.RicercaNote rn_note;
        protected System.Web.UI.WebControls.RadioButtonList rb_annulla_C;
        protected System.Web.UI.WebControls.RadioButtonList rb_evidenza_C;
        protected System.Web.UI.WebControls.TextBox txt_protoEme;
        protected System.Web.UI.WebControls.TextBox txt_dtaProtoEme_Da;
        protected System.Web.UI.WebControls.Panel pnl_protoEme;
        // protected DocsPaWebCtrlLibrary.DateMask txt_dtaProtoEme;
        protected DocsPAWA.UserControls.Calendar txt_dataProtoEmeInizio;
        protected DocsPAWA.UserControls.Calendar txt_dataProtoEmeFine;
        protected System.Web.UI.WebControls.DropDownList ddl_dataProtoEme;
        protected System.Web.UI.WebControls.Label lbl_dataProtoEmeInizio;
        protected System.Web.UI.WebControls.Label lbl_dataProtoEmeFine;

        protected System.Web.UI.WebControls.ImageButton enterKeySimulator;
        protected System.Web.UI.WebControls.TextBox tbAnnoProt;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoDoc_C;
        protected System.Web.UI.WebControls.TextBox txt_ogg_C;
        protected System.Web.UI.WebControls.TextBox txt_CodFascicolo;
        protected System.Web.UI.WebControls.TextBox txt_DescFascicolo;
        protected System.Web.UI.WebControls.Panel pnl_fasc_rapida;
        protected System.Web.UI.WebControls.TextBox txt_segnatura;
        protected System.Web.UI.WebControls.ImageButton btn_CampiPersonalizzati;



        protected System.Web.UI.WebControls.TextBox txt_codUffRef;
        protected System.Web.UI.WebControls.TextBox txt_descUffRef;
        protected System.Web.UI.WebControls.Panel pnl_uffRef;
        protected System.Web.UI.WebControls.Panel pnl_cons;
        protected System.Web.UI.HtmlControls.HtmlImage btn_Rubrica_ref;
        protected System.Web.UI.HtmlControls.HtmlImage btn_Rubrica_C;
        protected System.Web.UI.WebControls.DropDownList ddl_statiDoc;
        protected System.Web.UI.WebControls.Panel Panel_StatiDocumento;
        protected System.Web.UI.WebControls.ImageButton btn_Canc_Ric;
        protected System.Web.UI.WebControls.DropDownList ddl_Ric_Salvate;
        protected System.Web.UI.WebControls.ImageButton btn_rubricaMitInt_C;
        protected Utilities.MessageBox mb_ConfirmDelete;
        protected System.Web.UI.WebControls.Button btn_salva;
        protected System.Web.UI.WebControls.Panel pnl;
        protected System.Web.UI.WebControls.Panel pnl_mezzoSpedizione;
        public SchedaRicerca schedaRicerca = null;
        protected DocsPaWebCtrlLibrary.ImageButton imgFasc;
        protected string codClassifica = "";
        protected string idRegNodoTit = "";
        //protected UserControls.Creatore Creatore;
        protected System.Web.UI.WebControls.CheckBoxList cbl_archDoc_C;
        protected System.Web.UI.WebControls.DropDownList ddl_spedizione;
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
        protected System.Web.UI.WebControls.DropDownList ddl_dataScadenza_C;
        protected System.Web.UI.WebControls.Label lbl_initDataScadenza_C;
        protected System.Web.UI.WebControls.Label lbl_fineDataScadenza_C;
        protected DocsPAWA.UserControls.Calendar txt_initDataScadenza_C;
        protected DocsPAWA.UserControls.Calendar txt_fineDataScadenza_C;
        private bool isMezzoSpedizioneRequired = false;
        protected System.Web.UI.WebControls.CheckBoxList cbl_docInCompl;
        protected System.Web.UI.WebControls.CheckBox cbx_Trasm;
        protected System.Web.UI.WebControls.CheckBox cbx_TrasmSenza;
        protected System.Web.UI.WebControls.DropDownList ddl_ragioneTrasm;
        protected Hashtable m_hashTableRagioneTrasmissione;
        protected DocsPAWA.DocsPaWR.RagioneTrasmissione[] listaRagioni;
        protected System.Web.UI.HtmlControls.HtmlGenericControl DivRicCompleta;
        protected System.Web.UI.WebControls.CheckBox cb_Conservato;
        protected System.Web.UI.WebControls.CheckBox cb_NonConservato;
        protected System.Web.UI.WebControls.CheckBox cb_mitt_dest_storicizzati;
        protected System.Web.UI.WebControls.Panel pnl_trasfDesp;
        protected System.Web.UI.WebControls.RadioButtonList rbl_TrasfDep;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoFileAcquisiti;
        protected System.Web.UI.WebControls.CheckBox chkFirmato;
        protected System.Web.UI.WebControls.CheckBox chkNonFirmato;
        protected System.Web.UI.WebControls.Panel pnl_ProtocolloTitolario;
        protected System.Web.UI.WebControls.Label lbl_ProtocolloTitolario;
        protected System.Web.UI.WebControls.DropDownList ddl_Titolario;
        protected System.Web.UI.WebControls.TextBox txt_ProtocolloTitolario;
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
        protected System.Web.UI.WebControls.TextBox txt_codDesc;
        protected System.Web.UI.WebControls.Label l_amm_interop;
        protected System.Web.UI.WebControls.Panel p_cod_amm;

        protected System.Web.UI.HtmlControls.HtmlInputControl clTesto;
        protected  int caratteriDisponibili=2000;

        protected System.Web.UI.WebControls.DropDownList ddlOrder, ddlOrderDirection;
        protected DropDownList ddl_op_versioni;
        protected TextBox txt_versioni;
        protected DropDownList ddl_op_allegati;
        protected TextBox txt_allegati;
        protected bool change_from_grid;
        protected bool no_custom_grid_cont;

        protected System.Web.UI.WebControls.Button btn_modifica;

        protected System.Web.UI.WebControls.DropDownList ddlStateCondition;
        protected System.Web.UI.WebControls.Panel pnl_visiblitaDoc;
        protected System.Web.UI.WebControls.RadioButtonList rbl_visibilita;

        protected string numResult;

        protected DocsPaWebCtrlLibrary.ImageButton btn_clear_fields;

        protected System.Web.UI.WebControls.Panel pnl_riferimento;

        protected System.Web.UI.WebControls.TextBox txt_rif_mittente;

        protected DocsPAWA.UserControls.AuthorOwnerFilter aofAuthor, aofOwner;
        protected System.Web.UI.WebControls.RadioButtonList rblFiltriAllegati;
        protected System.Web.UI.WebControls.RadioButtonList rblFiltriNumAllegati;
        protected Dictionary<string, Corrispondente> dic_Corr;

        #endregion

        private void ricDocCompleta_PreRender(object sender, System.EventArgs e)
        {
            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Document)
            {
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);
            }

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

                //visibilità filtro numero allegati --> sistemi esterni
                if (string.IsNullOrEmpty(filterExternalSystem) || (!filterExternalSystem.Equals("1")))
                {
                    foreach (ListItem f in rblFiltriNumAllegati.Items)
                    {
                        if (f.Value.Equals("esterni"))
                        {
                            rblFiltriNumAllegati.Items.Remove(f);
                            break;
                        }
                    }
                }
            }
            DocsPAWA.DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionatoFascRapida(this);

            if (fasc != null)
            {
                /*                this.idRegNodoTit = fasc.idRegistroNodoTit;
                                // this.idRegistro = (string)fasc.idRegistro;
                                if (!this.idRegNodoTit.Equals("") && this.idRegNodoTit != null)
                                {
                                    UserManager.setListaIdRegistri(this, idRegNodoTit);
                                    updateListaRegistri(idRegNodoTit);

                                    // rbl_Reg_C_SelectedIndexChanged(sender,e);
                                }
                                else
                                {
                                    updateListaRegistri("");
                                }
                */
                this.txt_CodFascicolo.Text = fasc.codice;
                this.txt_DescFascicolo.Text = fasc.descrizione;
            }

            DocsPAWA.DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteSelezionato(this);
            if (corr != null)
            {
                this.txt_codMitt_C.Text = corr.codiceRubrica;
                this.txt_descrMitt_C.Text = corr.descrizione;
            }

            if (Session["dictionaryCorrispondente"] != null)
            {
                dic_Corr = (Dictionary<string, Corrispondente>)Session["dictionaryCorrispondente"];

                if (dic_Corr != null && dic_Corr.ContainsKey("ricCompleta") && dic_Corr["ricCompleta"] != null)
                {
                    txt_codMitt_C.Text = dic_Corr["ricCompleta"].codiceRubrica;
                    this.txt_descrMitt_C.Text = dic_Corr["ricCompleta"].descrizione;
                    this.hd_systemIdMit_Com.Value = dic_Corr["ricCompleta"].systemId;
                }
                dic_Corr.Remove("ricCompleta");
                Session.Add("dictionaryCorrispondente", dic_Corr);
            }

            string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
            HtmlImage btn_Rubrica_C = (HtmlImage)FindControl("btn_Rubrica_C");
            if (!this.cb_mitt_dest_storicizzati.Checked)
                btn_Rubrica_C.Attributes["onclick"] = "_ApriRubrica('ric_stor');";
            else
            {
                btn_Rubrica_C.Attributes["onclick"] = "_ApriRubrica('ric_mittdest');";
                btn_Rubrica_ref.Attributes["onclick"] = "_ApriRubrica('ric_uffref');";
                btn_rubricaMitInt_C.Attributes["onclick"] = "_ApriRubrica('ric_mittintermedio');";
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

            string new_search = string.Empty;
            if (ViewState["new_search"] != null)
            {
                new_search = ViewState["new_search"] as string;
                ViewState["new_search"] = null;
            }

            if (change_from_grid && string.IsNullOrEmpty(new_search))
            {
                
                if (RicercaCompleta())
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
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=completa&tabRes=completa" + altro + "';</script>");
                        //       ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=estesa&tabRes=estesa" + altro + "';", true);
                    }
                    else
                    {
                        //     ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?from=estesa&tabRes=estesa" + altro + "';", true);
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?from=completa&tabRes=completa" + altro + "';</script>");
                    }
                }
            }

            //Visualizzazione pannello visbilità tipica / atipica
            if (Utils.GetAbilitazioneAtipicita())
                pnl_visiblitaDoc.Visible = true;
        }

        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Remove("rubrica.campoCorrispondente");
                Session.Remove("rubrica.idCampoCorrispondente");
                Session.Remove("CorrSelezionatoDaMulti");
                
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

            //	
            Utils.startUp(this);

            if (string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE)) ||
               !bool.Parse(ConfigSettings.getKey(ConfigSettings.KeysENUM.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE)))
            {
                p_cod_amm.Visible = false;
            }

                    this.Page.MaintainScrollPositionOnPostBack = true;
            enableUfficioRef = ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF);

            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_OGGETTO_COMM_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_OGGETTO_COMM_REF).Equals("1"))
            {
                this.panel_numOgg_commRef.Visible = true;
            }

            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.MEZZO_SPEDIZIONE) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.MEZZO_SPEDIZIONE).ToUpper().Equals("1"))
                this.isMezzoSpedizioneRequired = true;
            else
                this.isMezzoSpedizioneRequired = false;

            if (isMezzoSpedizioneRequired)
                this.pnl_mezzoSpedizione.Visible = true;
            else
                this.pnl_mezzoSpedizione.Visible = false;

            //if (ConfigSettings.getKey(ConfigSettings.KeysENUM.CONSERVAZIONE) != null
            //&& ConfigSettings.getKey(ConfigSettings.KeysENUM.CONSERVAZIONE).ToUpper().Equals("1"))
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

            ////Trasferimento deposito
            //if (UserManager.ruoloIsAutorized(this, "GEST_ARCHIVIA"))
            //    this.pnl_trasfDesp.Visible = true;
            //else
            //    this.pnl_trasfDesp.Visible = false;


            schedaRicerca = (SchedaRicerca)Session[SchedaRicerca.SESSION_KEY];
            if (schedaRicerca == null)
            {
                //Inizializzazione della scheda di ricerca per la gestione delle 
                //ricerche salvate
                DocsPaWR.Utente utente = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
                DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];

                schedaRicerca = new SchedaRicerca(KEY_SCHEDA_RICERCA, utente, ruolo, this);
                Session[SchedaRicerca.SESSION_KEY] = schedaRicerca;
            }
            schedaRicerca.Pagina = this;

            setFormProperties();

            if (!IsPostBack)
            {
                DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();
                info = UserManager.getInfoUtente(this.Page);


                string valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_OGGETTO");
                if (!string.IsNullOrEmpty(valoreChiave))
                    caratteriDisponibili = int.Parse(valoreChiave);

                txt_ogg_C.MaxLength = caratteriDisponibili;
                clTesto.Value = caratteriDisponibili.ToString();
                txt_ogg_C.Attributes.Add("onKeyUp", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','DESCRIZIONE'," + clTesto.ClientID + ")");
                txt_ogg_C.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','DESCRIZIONE'," + clTesto.ClientID + ")");

                CaricaComboMezzoSpedizione(this.ddl_spedizione);
                caricaRagioni(schedaRicerca.FiltriRicerca);
                caricaComboTipoFileAcquisiti();

                if (schedaRicerca.FiltriRicerca != null)
                {
                    //carico il creatore, se esiste
                    //DocsPaWR.Corrispondente creator = UserManager.getCreatoreSelezionato(this);
                    //if (creator != null)
                    //{
                    //    this.Creatore.RestoreCurrentFilters();
                    //}
                    //UserManager.removeCreatoreSelezionato(this);
                }

                if (wws.isEnableRiferimentiMittente())
                {
                    this.pnl_riferimento.Visible = true;
                }
            }

            if (!IsPostBack)
            {
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
                    //Scheda Ricerca
                    schedaRicerca.ElencoRicerche("D", ddl_Ric_Salvate);
                    isSavedSearch = PopulateField(schedaRicerca.FiltriRicerca, false);
                }
                // Visualizzazione pagina di ricerca nella selezione 
                // di un criterio di ricerca salvato
                this.ddl_Ric_Salvate.Attributes.Add("onChange", "OnChangeSavedFilter();");

                //mette il focus sul campo numero protocollo
                string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C.ID + "').focus();</SCRIPT>";
                RegisterStartupScript("focus", s);

                //set anno corrente al page load, ma non ap postback
                // attenzione: se vengo da un back di elemento di una ricerca salvata
                // devo comportarmi diversamente
                if (!isSavedSearch)
                {
                    // Se la ricerca è una ricerca in ADL, non viene valorizzato il campo Anno protocollo
                    if (Request["ricADL"] != "1")
                        this.tbAnnoProt.Text = System.DateTime.Now.Year.ToString();
                }

                #region ABILITAZIONE PROTOCOLLAZIONE INTERNA
                DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];

                DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                if (!ws.IsInternalProtocolEnabled(cr.idAmministrazione)) this.cbl_archDoc_C.Items.Remove(this.cbl_archDoc_C.Items[2]);
                #endregion

                #region Abilitazione ricerca allegati
                if (!this.IsEnabledProfilazioneAllegato)
                    this.cbl_archDoc_C.Items.Remove(this.cbl_archDoc_C.Items.FindByValue("ALL"));
                #endregion

                //Butt_ricerca.Attributes.Add("OnClick","ApriFrame('RicercaDoc_cn.aspx','centrale');");
                //Button1.Attributes.Add("OnClick","ApriFrame('RicercaDoc_cn.aspx','centrale');");

                // attenzione: se vengo da un back di elemento di una ricerca salvata
                // devo comportarmi diversamente
                if (!isSavedSearch)
                {
                    this.txt_fineNumProt_C.Visible = false;
                    this.lblDAnumprot_C.Visible = false;
                    this.lblAnumprot_C.Visible = false;
                    this.GetCalendarControl("txt_fineDataProt_C").Visible = false;
                    this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Visible = false;
                    this.GetCalendarControl("txt_fineDataProt_C").btn_Cal.Visible = false;
                    this.lbl_finedataProt_C.Visible = false;
                    this.lbl_initdataProt_C.Visible = false;
                    this.GetCalendarControl("txt_fineDataArrivo_C").Visible = false;
                    this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Visible = false;
                    this.GetCalendarControl("txt_fineDataArrivo_C").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").Visible = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = false;
                    this.lbl_finedataProtMitt_C.Visible = false;
                    this.lbl_initdataProtMitt_C.Visible = false;
                    this.GetCalendarControl("txt_finedataCreazione_E").Visible = false;
                    this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;


                    //	this.Desc_Ruolo.Text=userRuolo.descrizione;
                    //userReg=((DocsPaVO.utente.registro)userHome.registri);
                }
                if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                {
                    this.pnl_uffRef.Visible = true;
                }
                setListaRegistri();
                //CaricaComboTipologiaAtto(this.ddl_tipoDoc_C);
            }
            setListaParoleChiave();
            caricaTrasmissioni();

            //carico il mittente selezionato, se esiste
            DocsPaWR.Corrispondente cor = UserManager.getCorrispondenteSelezionato(this);

            if (cor != null)
            {
                this.txt_codMitt_C.Text = cor.codiceRubrica;
                this.txt_descrMitt_C.Text = UserManager.getDecrizioneCorrispondenteSemplice(cor);
                this.hd_systemIdMit_Com.Value = cor.systemId;
            }

            //carico il mittente intermedio selezionato, se esiste
            cor = UserManager.getCorrispondenteIntSelezionato(this);
            if (cor != null)
            {
                this.txt_codMittInter_C.Text = cor.codiceRubrica;
                this.txt_descrMittInter_C.Text = UserManager.getDecrizioneCorrispondenteSemplice(cor);
                this.hd_systemIdMitInt.Value = cor.systemId;
            }
            //carico l'ufficio referente selezionato, se esiste
            cor = UserManager.getCorrispondenteReferenteSelezionato(this);
            if (cor != null)
            {
                this.txt_codUffRef.Text = cor.codiceRubrica;
                this.txt_descUffRef.Text = UserManager.getDecrizioneCorrispondenteSemplice(cor);
                this.hd_systemIdUffRef.Value = cor.systemId;
            }

            UserManager.removeCorrispondentiSelezionati(this);


            //PROFILAZIONE DINAMICA
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
            {
                verificaCampiPersonalizzati();
            }
            else
            {
                btn_CampiPersonalizzati.Visible = false;
            }
            //FINE PROFILAZIONE DINAMICA

            tastoInvio();

            //new ADL
            if (!IsPostBack &&
                    Request.QueryString["ricADL"] != null &&
                    Request.QueryString["ricADL"] == "1" &&
                    !SiteNavigation.CallContextStack.CurrentContext.IsBack
                )
            {
                lblSearch.Text = "Ricerche Salvate Area di Lavoro";
                this.btn_ricerca_Click(null, null);
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
                    ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=completa&tabRes=completa" + altro + "';", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?from=completa&tabRes=completa" + altro + "';", true);
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
            if (this.IsEnabledProfilazioneAllegato && cbl_archDoc_C.Items.FindByValue("ALL").Selected)
                rblFiltriAllegati.Style.Add("display", "block");
            else if (this.IsEnabledProfilazioneAllegato && (!cbl_archDoc_C.Items.FindByValue("ALL").Selected))
                rblFiltriAllegati.Style.Add("display", "none");
            if (this.IsEnabledProfilazioneAllegato)
            {
                int countItem = -1;
                foreach (ListItem i in cbl_archDoc_C.Items)
                {
                    ++countItem;
                    if (i.Value.Equals("ALL"))
                    {
                        string scriptGestAll = "<script language='javascript'>" +
                            "function SetVisibilityFilterAlleg() { " +
                            "if (document.getElementById('cbl_archDoc_C_" + countItem + "') != null && " +
                            "document.getElementById('cbl_archDoc_C_" + countItem + "').checked) { " +
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
                            "var cbo_all = document.getElementById('cbl_archDoc_C_" + countItem + "'); " +
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

        private void caricaRagioni(DocsPAWA.DocsPaWR.FiltroRicerca[][] qV)
        {
            //listaRagioni = TrasmManager.getListaRagioni(this, null);
            //true : vengo da ricerca trasmissioni cerco tutte le ragioni
            //false : ricerca trasm con cha_vis='1'

            listaRagioni = TrasmManager.getListaRagioni(this, null, true);

            if (!Page.IsPostBack)
            {
                m_hashTableRagioneTrasmissione = new Hashtable();
                if (listaRagioni != null && listaRagioni.Length > 0)
                {
                    ddl_ragioneTrasm.Items.Add("Tutte");
                    for (int i = 0; i < listaRagioni.Length; i++)
                    {
                        m_hashTableRagioneTrasmissione.Add(i, listaRagioni[i]);

                        ListItem newItem = new ListItem(listaRagioni[i].descrizione, listaRagioni[i].systemId);
                        ddl_ragioneTrasm.Items.Add(newItem);
                    }
                    TrasmManager.setHashRagioneTrasmissione(this, m_hashTableRagioneTrasmissione);

                    this.ddl_ragioneTrasm.SelectedIndex = 0;
                }
            }
            else
            {
                m_hashTableRagioneTrasmissione = TrasmManager.getHashRagioneTrasmissione(this);
            }

            if (qV != null)
            {
                foreach (DocsPAWA.DocsPaWR.FiltroRicerca[] filterArray in qV)
                    foreach (DocsPAWA.DocsPaWR.FiltroRicerca filterItem in filterArray)
                    {
                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.TRASMESSI_CON.ToString())
                        {
                            this.ddl_ragioneTrasm.SelectedItem.Text = filterItem.valore;
                            this.cbx_Trasm.Checked = true;
                        }
                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.TRASMESSI_SENZA.ToString())
                        {
                            this.ddl_ragioneTrasm.SelectedItem.Text = filterItem.valore;
                            this.cbx_TrasmSenza.Checked = true;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.TRASMESSI_CON.ToString())
                        {
                            this.cbx_Trasm.Checked = true;
                            if (filterItem.valore.Equals("Tutte"))
                                this.ddl_ragioneTrasm.SelectedIndex = 0;
                            else
                                this.ddl_ragioneTrasm.SelectedValue = filterItem.valore;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.TRASMESSI_SENZA.ToString())
                        {
                            this.cbx_TrasmSenza.Checked = true;
                            if (filterItem.valore.Equals("Tutte"))
                                this.ddl_ragioneTrasm.SelectedIndex = 0;
                            else
                                this.ddl_ragioneTrasm.SelectedValue = filterItem.valore;
                        }
                    }
            }
        }

        private void setListaRegistri()
        {
            bool filtroAoo = false;
            DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistriNoFiltroAOO(this, out filtroAoo);

            //DocsPaWR.Registro[] registri = UserManager.getRuolo(this).registri;
            //string[] listaReg = new string[registri.Length];
            if (userRegistri != null && filtroAoo)
            {
                ListItem itemM = new ListItem("I miei", "M");
                rbl_Reg_C.Items.Add(itemM);
                itemM = new ListItem("Tutti", "T");
                rbl_Reg_C.Items.Add(itemM);
                itemM = new ListItem("Reset", "R");
                rbl_Reg_C.Items.Add(itemM);
                lb_reg_C.Rows = 5;
            }
            else
            {
                userRegistri = UserManager.getRuolo(this).registri;
                ListItem itemM = new ListItem("Tutti", "T");
                rbl_Reg_C.Items.Add(itemM);
                itemM = new ListItem("Reset", "R");
                rbl_Reg_C.Items.Add(itemM);
                //rbl_Reg_E.SelectedIndex = 1;
            }
            rbl_Reg_C.SelectedIndex = 0;
            string[] id = new string[userRegistri.Length];
            for (int i = 0; i < userRegistri.Length; i++)
            {
                lb_reg_C.Items.Add(userRegistri[i].codRegistro);
                lb_reg_C.Items[i].Value = userRegistri[i].systemId;
                string nomeRegCurrente = "UserReg" + i;
                // SELEZIONA TUTTI I REGISTRI PRESENTI per DEFAULT
                if (!filtroAoo)
                {
                    if (!userRegistri[i].flag_pregresso)
                        lb_reg_C.Items[i].Selected = true;
                }
                else
                    if (rbl_Reg_C.SelectedItem.Value == "M")
                        for (int j = 0; j < UserManager.getRuolo(this).registri.Length; j++)
                        {
                            if (UserManager.getRuolo(this).registri[j].codRegistro == lb_reg_C.Items[i].Text)
                            {
                                if (!userRegistri[i].flag_pregresso)
                                {
                                    lb_reg_C.Items[i].Selected = true;
                                    break;
                                }
                            }
                        }

                id[i] = (string)userRegistri[i].systemId;
            }

            //UserManager.setListaIdRegistri(this, listaReg);
            //rbl_Reg_C.Items[0].Selected = true;
        }

        private void setListaParoleChiave()
        {
            //DocsPaWR.DocumentoParolaChiave[] listaParoleChiave = DocumentManager.getListaParoleChiave(this);
            DocsPaWR.DocumentoParolaChiave[] listaParoleChiave = DocumentManager.getListaParoleChiaveSel(this);
            if (listaParoleChiave == null)
                return;

            this.ListParoleChiave.Items.Clear();

            if (listaParoleChiave.Length > 0)
            {
                for (int i = 0; i < listaParoleChiave.Length; i++)
                {
                    this.ListParoleChiave.Items.Add(((DocsPAWA.DocsPaWR.DocumentoParolaChiave)listaParoleChiave[i]).descrizione);
                    this.ListParoleChiave.Items[i].Value = ((DocsPAWA.DocsPaWR.DocumentoParolaChiave)listaParoleChiave[i]).systemId;
                }
            }

            DocumentManager.removeListaParoleChiaveSel(this);
        }

        private void setFormProperties()
        {
            this.btn_RubrDest_P.Attributes.Add("onclick", "ApriOggettario('ric_C');");
            //TODO Chiave di Configurazione per chiamare o no la vecchia gestione
            string keyAdvanced = string.Empty;
            keyAdvanced = DocsPAWA.utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione,
                                                                  "FE_PAROLE_CHIAVI_AVANZATE");
            if (keyAdvanced.Equals("1"))
                this.btn_selezionaParoleChiave.Attributes.Add("onclick", "ApriFinestraParoleChiaveAdvanced('RicC');");
            else
            {
                this.btn_selezionaParoleChiave.Attributes.Add("onclick", "ApriFinestraParoleChiave('RicC');");
            }


            //string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
            //HtmlImage btn_Rubrica_C = (HtmlImage)FindControl("btn_Rubrica_C");
            //if (this.cb_mitt_dest_storicizzati.Checked)
            //    btn_Rubrica_C.Attributes["onclick"] = "_ApriRubrica('ric_stor');";
            //else
            //{
            //    btn_Rubrica_C.Attributes["onclick"] = "_ApriRubrica('ric_mittdest');";
            //    btn_Rubrica_ref.Attributes["onclick"] = "_ApriRubrica('ric_uffref');";
            //    btn_rubricaMitInt_C.Attributes["onclick"] = "_ApriRubrica('ric_mittintermedio');";
            //}

            if (!Page.IsPostBack)
            {
                CaricaComboTipologiaAtto(ddl_tipoDoc_C);

                //Protocollo Titolario
                string contatoreTitolario = wws.isEnableContatoreTitolario();
                if (!string.IsNullOrEmpty(contatoreTitolario))
                {
                    this.txt_ProtocolloTitolario.Attributes.Add("onKeyPress", "ValidateNumericKey();");
                    this.pnl_ProtocolloTitolario.Visible = true;
                    this.lbl_ProtocolloTitolario.Text = "Titolario / " + contatoreTitolario;
                    caricaComboTitolari(ddl_Titolario);
                }
            }
        }

        private void CaricaComboMezzoSpedizione(DropDownList ddl)
        {
            ddl.Items.Clear();
            //aggiunge una riga vuota alla combo
            ddl.Items.Add("");
            ArrayList listaMezzoSpedizione = new ArrayList();
            DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
            string idAmm = UserManager.getInfoUtente().idAmministrazione;
            DocsPAWA.DocsPaWR.MezzoSpedizione[] m_sped = ws.AmmListaMezzoSpedizione(idAmm, true);
            foreach (DocsPAWA.DocsPaWR.MezzoSpedizione m_spediz in m_sped)
            {
                ListItem li = new ListItem();
                li.Value = m_spediz.IDSystem;
                li.Text = m_spediz.Descrizione;
                ddl.Items.Add(li);
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
            this.enterKeySimulator.Click += new System.Web.UI.ImageClickEventHandler(this.enterKeySimulator_Click);
            this.mb_ConfirmDelete.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.mb_ConfirmDelete_GetMessageBoxResponse);
            this.ddl_Ric_Salvate.SelectedIndexChanged += new System.EventHandler(this.ddl_Ric_Salvate_SelectedIndexChanged);
            this.btn_Canc_Ric.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Canc_Ric_Click);
            this.lb_reg_C.SelectedIndexChanged += new System.EventHandler(this.lb_reg_C_SelectedIndexChanged);
            this.rbl_Reg_C.SelectedIndexChanged += new System.EventHandler(this.rbl_Reg_C_SelectedIndexChanged);
            this.ddl_numProt_C.SelectedIndexChanged += new System.EventHandler(this.ddl_numProt_C_SelectedIndexChanged);
            this.ddl_dataProt_C.SelectedIndexChanged += new System.EventHandler(this.ddl_dataProt_C_SelectedIndexChanged);
            this.ddl_tipoDoc_C.SelectedIndexChanged += new System.EventHandler(this.ddl_tipoDoc_C_SelectedIndexChanged);
            //this.ddl_dataStampa.SelectedIndexChanged += new System.EventHandler(this.ddl_dataStampa_SelectedIndexChanged);
            this.btn_CampiPersonalizzati.Click += new System.Web.UI.ImageClickEventHandler(this.btn_CampiPersonalizzati_Click);



            this.ddl_statiDoc.SelectedIndexChanged += new System.EventHandler(this.ddl_statiDoc_SelectedIndexChanged);
            this.txt_codMitt_C.TextChanged += new System.EventHandler(this.txt_codMitt_C_TextChanged);
            this.txt_codUffRef.TextChanged += new System.EventHandler(this.txt_codUffRef_TextChanged);
            this.txt_codMittInter_C.TextChanged += new System.EventHandler(this.txt_codMittInter_C_TextChanged);
            this.txt_CodFascicolo.TextChanged += new System.EventHandler(this.txt_CodFascicolo_TextChanged);
            this.ddl_dataProtMitt_C.SelectedIndexChanged += new System.EventHandler(this.ddl_dataProtMitt_C_SelectedIndexChanged);
            this.ddl_dataArrivo_C.SelectedIndexChanged += new System.EventHandler(this.ddl_dataArrivo_C_SelectedIndexChanged);
            this.btn_ricerca.Click += new System.EventHandler(this.btn_ricerca_Click);
            this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
            this.imgFasc.Click += new System.Web.UI.ImageClickEventHandler(this.imgFasc_Click);
            this.ID = "f_Ricerca_C";
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.ricDocCompleta_PreRender);
            //this.Unload += new System.EventHandler(this.Page_Unload);
            this.ddl_dataCreazione_E.SelectedIndexChanged += new System.EventHandler(this.ddl_dataCreazione_E_SelectedIndexChanged);
            this.ddl_idDocumento_C.SelectedIndexChanged += new System.EventHandler(this.ddl_idDocumento_C_SelectedIndexChanged);
            this.ddl_dataScadenza_C.SelectedIndexChanged += new System.EventHandler(this.ddl_dataScadenza_SelectedIndexChanged);
            this.ddl_dataProtoEme.SelectedIndexChanged += new System.EventHandler(this.ddl_dataProtoEme_SelectedIndexChanged);
            //this.rb_conservazione.SelectedIndexChanged += new EventHandler(rb_conservazione_SelectedIndexChanged);
            this.cb_Conservato.CheckedChanged += new EventHandler(cb_Conservato_CheckedChanged);
            this.cb_NonConservato.CheckedChanged += new EventHandler(cb_NonConservato_CheckedChanged);
            this.ddl_dataStampa_E.SelectedIndexChanged += new System.EventHandler(this.ddl_dataStampa_E_SelectedIndexChanged);
            this.cb_mitt_dest_storicizzati.CheckedChanged += new System.EventHandler(this.cb_mitt_dest_storicizzati_CheckedChanged);
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

        #endregion

        private void caricaTrasmissioni()
        {
            if (this.Page.IsPostBack)
            {
                if (this.cbx_Trasm.Checked)
                    this.cbx_TrasmSenza.Checked = false;

                if (this.cbx_TrasmSenza.Checked)
                    this.cbx_Trasm.Checked = false;
            }
        }

        private void rbl_Reg_C_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.rbl_Reg_C.SelectedItem.Value.Equals("R"))
            {
                UserManager.removeListaIdRegistri(this);
                //for (int h = 0; h < this.lb_reg_C.Items.Count; h++)
                //    lb_reg_C.Items[h].Selected = false;
                lb_reg_C.ClearSelection();
            }
            if (this.rbl_Reg_C.SelectedItem.Value.Equals("T"))
            {
                UserManager.setListaIdRegistri(this, UserManager.getListaIdRegistriUtente(this));
                for (int h = 0; h < this.lb_reg_C.Items.Count; h++)
                    lb_reg_C.Items[h].Selected = true;
            }
            if (this.rbl_Reg_C.SelectedItem.Value.Equals("M"))
            {
                lb_reg_C.ClearSelection();
                ArrayList idList = new ArrayList();
                for (int h = 0; h < this.lb_reg_C.Items.Count; h++)
                {
                    for (int i = 0; i < UserManager.getRuolo(this).registri.Length; i++)
                    {
                        if (UserManager.getRuolo(this).registri[i].codRegistro == lb_reg_C.Items[h].Text)
                        {
                            if (UserManager.getRuolo(this).registri[i] != null && !UserManager.getRuolo(this).registri[i].flag_pregresso)
                            {
                                lb_reg_C.Items[h].Selected = true;
                                idList.Add(lb_reg_C.Items[h].Value);
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
        private void lb_reg_C_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.rbl_Reg_C.ClearSelection();

            ArrayList idList = new ArrayList();
            for (int h = 0; h < this.lb_reg_C.Items.Count; h++)
            {
                if (lb_reg_C.Items[h].Selected)
                    idList.Add(lb_reg_C.Items[h].Value);
            }
            string[] id = new string[idList.Count];
            for (int i = 0; i < idList.Count; i++)
                id[i] = (string)idList[i];
            UserManager.setListaIdRegistri(this, id);
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

        protected bool RicercaCompleta()
        {
            try
            {
                //array contenitore degli array filtro di ricerca
                qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
                fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

                string valore = "";
                if (this.cbl_archDoc_C.Items.FindByValue("A") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
                    fV1.valore = Convert.ToString(this.cbl_archDoc_C.Items.FindByValue("A").Selected);
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.cbl_archDoc_C.Items.FindByValue("P") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
                    fV1.valore = Convert.ToString(this.cbl_archDoc_C.Items.FindByValue("P").Selected);
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.cbl_archDoc_C.Items.FindByValue("I") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
                    fV1.valore = Convert.ToString(this.cbl_archDoc_C.Items.FindByValue("I").Selected);
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.cbl_archDoc_C.Items.FindByValue("G") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
                    fV1.valore = Convert.ToString(this.cbl_archDoc_C.Items.FindByValue("G").Selected);
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.cbl_archDoc_C.Items.FindByValue("Pr") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                    fV1.valore = Convert.ToString(this.cbl_archDoc_C.Items.FindByValue("Pr").Selected);
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                if (this.IsEnabledProfilazioneAllegato && this.cbl_archDoc_C.Items.FindByValue("ALL").Selected)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.ALLEGATO.ToString();
                    fV1.valore = this.cbl_archDoc_C.Items.FindByValue("ALL").Selected.ToString();
                    fV1.valore = this.rblFiltriAllegati.SelectedValue.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                //if (this.cbl_archDoc_C.Items.FindByValue("S") != null)
                //{
                //    if (this.cbl_archDoc_C.Items.FindByValue("S").Selected)
                //        valore += "1";
                //    else
                //        valore += "0";
                //}
                //else
                //    valore += "0";


                #region filtro data creazione
                if (this.ddl_dataCreazione_E.SelectedIndex == 2)
                {
                    // siamo nel caso di Settimana corrente
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

                #region filtro Archivio (Arrivo, Partenza, Tutti)
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                fV1.valore = "tipo";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion

                #region filtro registro
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                string registri = "";
                if (this.lb_reg_C.Items.Count > 0)
                {
                    for (int i = 0; i < this.lb_reg_C.Items.Count; i++)
                    {
                        if (this.lb_reg_C.Items[i].Selected)
                            registri += this.lb_reg_C.Items[i].Value + ",";

                    }
                }
                if (!registri.Equals(""))
                {
                    //Elimino la virgola alla fine della stringa se è stato selezionato
                    //almeno un registro
                    registri = registri.Substring(0, registri.Length - 1);
                    fV1.valore = registri;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                else
                {
                    Response.Write("<script>alert('Selezionare almeno un registro');top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                    return false;

                }
                #endregion

                #region filtro numero protocollo
                if (this.ddl_numProt_C.SelectedIndex == 0)
                {//valore singolo carico NUM_PROTOCOLLO

                    if (this.txt_initNumProt_C.Text != null && !this.txt_initNumProt_C.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();
                        fV1.valore = this.txt_initNumProt_C.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    //if (this.cbl_archDoc_C.Items.FindByValue("S").Selected)
                    //{
                    //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    //    fV1.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA.ToString();
                    //    fV1.valore=this.txt_initNumProt_C.Text;
                    //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    //}
                }
                else
                {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                    if (!this.txt_initNumProt_C.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                        fV1.valore = this.txt_initNumProt_C.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.txt_fineNumProt_C.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                        fV1.valore = this.txt_fineNumProt_C.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    //if (this.cbl_archDoc_C.Items.FindByValue("S").Selected)
                    //{
                    //    if (!this.txt_initNumProt_C.Text.Equals(""))
                    //    {
                    //        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    //        fV1.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA_DAL.ToString();
                    //        fV1.valore = this.txt_initNumProt_C.Text;
                    //        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    //    }
                    //    if (!this.txt_fineNumProt_C.Text.Equals(""))
                    //    {
                    //        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    //        fV1.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA_AL.ToString();
                    //        fV1.valore = this.txt_fineNumProt_C.Text;
                    //        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    //    }
                    //}
                }
                #endregion

                #region filtro Data Stampa Registro (Commentato)
                //if (this.cbl_archDoc_C.Items.FindByValue("S") != null && this.cbl_archDoc_C.Items.FindByValue("S").Selected)
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

                #region filtro data protocollo
                if (this.ddl_dataProt_C.SelectedIndex == 2)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProt_C.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProt_C.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProt_C.SelectedIndex == 0)
                {//valore singolo carico DATA_PROTOCOLLO
                    if (this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text != null && !this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProt_C").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataProt_C.SelectedIndex == 1)
                {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                    if (this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text != null && !this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProt_C").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataProt_C").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro oggetto
                if (this.txt_ogg_C.Text != null && !this.txt_ogg_C.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.OGGETTO.ToString();
                    fV1.valore = DocsPaUtils.Functions.Functions.ReplaceApexes(Utils.DO_AdattaString(this.txt_ogg_C.Text));
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro mitt/dest

                if (!string.IsNullOrEmpty(this.hd_systemIdMit_Com.Value))
                {
                    if (!this.txt_descrMitt_C.Text.Equals(""))
                    {
                        if (!string.IsNullOrEmpty(this.txt_codMitt_C.Text))
                        {
                            if (this.cb_mitt_dest_storicizzati.Checked)
                            {
                                // Ricerca i documenti per i mittenti / destinatari storicizzati
                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString();
                                fV1.valore = this.txt_codMitt_C.Text;
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString();
                                fV1.valore = this.cb_mitt_dest_storicizzati.Checked.ToString();
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                            else
                            {
                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
                                fV1.valore = this.hd_systemIdMit_Com.Value;
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                        }
                    }
                }
                else
                {
                    if (!this.txt_descrMitt_C.Text.Equals(""))
                    {
                        if (!string.IsNullOrEmpty(this.txt_codMitt_C.Text))
                        {
                            if (this.cb_mitt_dest_storicizzati.Checked)
                            {
                                // Ricerca i documenti per i mittenti / destinatari storicizzati
                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString();
                                fV1.valore = this.txt_codMitt_C.Text;
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString();
                                fV1.valore = this.cb_mitt_dest_storicizzati.Checked.ToString();
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                            else
                            {
                                // Ricerca dell'id del corrispondente a partire dal codice
                                DocsPaWR.Corrispondente corrByCode = UserManager.getCorrispondenteByCodRubrica(this, this.txt_codMitt_C.Text);
                                if (corrByCode != null)
                                {
                                    this.hd_systemIdMit_Com.Value = corrByCode.systemId;

                                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
                                    fV1.valore = this.hd_systemIdMit_Com.Value;
                                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                                }
                                else
                                {
                                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
                                    fV1.valore = this.txt_descrMitt_C.Text;
                                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                        }
                        else
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
                            fV1.valore = this.txt_descrMitt_C.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }
                }

                #endregion

                #region filtro mitt/intermedio
                if (this.txt_descrMittInter_C.Text != null && !this.txt_descrMittInter_C.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    if (this.hd_systemIdMitInt != null && !this.hd_systemIdMitInt.Value.Equals(""))
                    {
                        fV1.argomento = DocsPaWR.FiltriDocumento.ID_MITTENTE_INTERMEDIO.ToString();
                        fV1.valore = this.hd_systemIdMitInt.Value;
                    }
                    else
                    {
                        fV1.argomento = DocsPaWR.FiltriDocumento.MITTENTE_INTERMEDIO.ToString();
                        fV1.valore = DocsPaUtils.Functions.Functions.ReplaceApexes(this.txt_descrMittInter_C.Text);
                    }
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro numero protocollo mittente
                if (this.txt_numProtMitt_C.Text != null && !this.txt_numProtMitt_C.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROTOCOLLO_MITTENTE.ToString();
                    fV1.valore = DocsPaUtils.Functions.Functions.ReplaceApexes(this.txt_numProtMitt_C.Text);
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro Tipologia Documento
                if (this.ddl_tipoDoc_C.SelectedIndex > 0)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString();
                    fV1.valore = this.ddl_tipoDoc_C.SelectedItem.Value;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                #endregion

                #region filtro data protocollo mittente
                if (this.ddl_dataProtMitt_C.SelectedIndex == 2)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProtMitt_C.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProtMitt_C.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProtMitt_C.SelectedIndex == 0)
                {//valore singolo carico DATA_PROTOCOLLO_MIT
                    if (this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text != null && !this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataProtMitt_C.SelectedIndex == 1)
                {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                    if (!this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro data arrivo
                if (this.ddl_dataArrivo_C.SelectedIndex == 2)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_ARRIVO_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataArrivo_C.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_ARRIVO_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataArrivo_C.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_ARRIVO_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataArrivo_C.SelectedIndex == 0)
                {//valore singolo carico DATA_PROTOCOLLO_MIT
                    if (this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text != null && !this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_ARRIVO_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataArrivo_C.SelectedIndex == 1)
                {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                    if (!this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_ARRIVO_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_ARRIVO_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro parole chiave

                //creo tanti filtri quante sono le parole chiave (condizione di AND)
                for (int i = 0; i < this.ListParoleChiave.Items.Count; i++)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PAROLE_CHIAVE.ToString();
                    fV1.valore = this.ListParoleChiave.Items[i].Value;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                #endregion

                #region filtro firmatari
                if (this.txt_nomeFirma_C.Text != null && !this.txt_nomeFirma_C.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.FIRMATARIO_NOME.ToString();
                    fV1.valore = this.txt_nomeFirma_C.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.txt_cognomeFirma_C.Text != null && !this.txt_cognomeFirma_C.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.FIRMATARIO_COGNOME.ToString();
                    fV1.valore = this.txt_cognomeFirma_C.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro numero oggetto
                if (this.txt_numOggetto.Text != null && !this.txt_numOggetto.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_OGGETTO.ToString();
                    fV1.valore = this.txt_numOggetto.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro commissione referente
                if (this.txt_commRef.Text != null && !this.txt_commRef.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.COMMISSIONE_REF.ToString();
                    fV1.valore = this.txt_commRef.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro note
                //                if (this.txt_note.Text != null && !this.txt_note.Text.Equals(""))
                if (this.rn_note.Testo != null && !this.rn_note.Testo.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NOTE.ToString();
                    string[] rf;
                    string rfsel = "0";
                    if (Session["RFNote"] != null && !string.IsNullOrEmpty(Session["RFNote"].ToString()))
                    {
                        rf = Session["RFNote"].ToString().Split('^');
                        rfsel = rf[1];
                    }


                    fV1.valore = DocsPaUtils.Functions.Functions.ReplaceApexes(this.rn_note.Testo) + "@-@" + this.rn_note.TipoRicerca + "@-@" + rfsel;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
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

                #region filtro Stato Doc  (Annullato, Non annullato, Tutti)
                if (!this.rb_annulla_C.SelectedItem.Value.Equals("T"))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.ANNULLATO.ToString();
                    fV1.valore = this.rb_annulla_C.SelectedItem.Value;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro Evidenza  (Si, No, Tutti)
                if (!this.rb_evidenza_C.SelectedItem.Value.Equals("T"))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.EVIDENZA.ToString();
                    fV1.valore = this.rb_evidenza_C.SelectedItem.Value;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region visualizza storico  ?da aggiustare? il valore da passare può essere qualunque
                //				if (this.rbl_Rif_C.SelectedItem.Value.Equals("S"))
                //				{
                //					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
                //					fV1.argomento=DocsPaWR.FiltriDocumento.VIS_STORICO_MITT_DEST.ToString();
                //					fV1.valore=this.rbl_Rif_C.SelectedItem.Value;
                //					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
                //				}
                #endregion

                #region filtro Protocollo Emergenza

                //filtro DATA PROTO EMERGENZA
                if (this.txt_protoEme.Text != null && !this.txt_protoEme.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTO_EMERGENZA.ToString();
                    fV1.valore = this.txt_protoEme.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }


                if (this.ddl_dataProtoEme.SelectedIndex == 2)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProtoEme.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProtoEme.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProtoEme.SelectedIndex == 0)
                {//valore singolo carico DATA_PROTOCOLLO_EMERGENZA
                    if (this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text != null && !this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataProtoEme.SelectedIndex == 1)
                {//valore singolo carico DATA_PROTO_EMER_DAL - DATA_PROTO_EMER_AL
                    if (!this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                ////filtro DATA PROTO EMERGENZA
                //if (this.GetCalendarControl("txt_dtaProtoEme").txt_Data.Text!=null && !this.GetCalendarControl("txt_dtaProtoEme").txt_Data.Text.Equals(""))
                //{
                //    if(!Utils.isDate(this.GetCalendarControl("txt_dtaProtoEme").txt_Data.Text))
                //    {
                //        Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                //        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_dtaProtoEme").txt_Data.ID + "').focus();</SCRIPT>";
                //        RegisterStartupScript("focus", s);
                //        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");						
                //        return false;
                //    }
                //    fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
                //    fV1.argomento=DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_IL.ToString();
                //    fV1.valore=this.GetCalendarControl("txt_dtaProtoEme").txt_Data.Text;
                //    fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
                //}
                #endregion

                #region nuovo filtro per prendere solo i documenti protocollati
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DA_PROTOCOLLARE.ToString();
                fV1.valore = "0";  //corrisponde a 'false'
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion

                #region filtro PROFILAZIONE_DINAMICA
                //fV1 = (DocsPAWA.DocsPaWR.FiltroRicerca) Session["filtroProfilazioneDinamica"];
                fV1 = (DocsPAWA.DocsPaWR.FiltroRicerca)schedaRicerca.GetFiltro(DocsPAWA.DocsPaWR.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString());
                if (fV1 != null)
                {
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion filtro PROFILAZIONE_DINAMICA

                #region CONDIZIONE_STATO_DOCUMENTO
                fV1 = new FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.CONDIZIONE_STATO_DOCUMENTO.ToString();
                fV1.valore = this.ddlStateCondition.SelectedValue;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion

                //#region filtro DIAGRAMMI DI STATO
                //if (ddl_statiDoc.Visible && ddl_statiDoc.SelectedIndex != 0)
                //{
                //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //    fV1.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DIAGRAMMA_STATO_DOC.ToString();
                //    string cond = String.Format(
                //        " AND (DPA_DIAGRAMMI.DOC_NUMBER = A.DOCNUMBER AND DPA_DIAGRAMMI.ID_STATO {0} {1}) ",
                //        this.ddlStateCondition.SelectedValue == "Equals" ? "=" : "!=", 
                //        ddl_statiDoc.SelectedValue);
                //    fV1.valore = cond;
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //}
                //#endregion filtro DIAGRAMMI DI STATO
                #region filtro DIAGRAMMI DI STATO
                if (this.ddl_statiDoc.Visible && this.ddl_statiDoc.SelectedIndex != 0)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DIAGRAMMA_STATO_DOC.ToString();
                    fV1.nomeCampo = this.ddlStateCondition.SelectedValue;
                    fV1.valore = this.ddl_statiDoc.SelectedValue;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion filtro DIAGRAMMI DI STATO

                #region filtro Segnatura

                if (!this.txt_segnatura.Text.Equals(string.Empty))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.SEGNATURA.ToString();
                    fV1.valore = this.txt_segnatura.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                #endregion

                //#region filtro Proprietario (User Control)
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

                #region filtro ufficio referente
                if (enableUfficioRef != null && enableUfficioRef.Equals("1"))
                {
                    if (this.txt_codUffRef.Text != null && !this.txt_codUffRef.Text.Equals(""))
                    {
                        if (this.hd_systemIdUffRef != null && !this.hd_systemIdUffRef.Value.Equals(""))
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.ID_UO_REF.ToString();
                            fV1.valore = this.hd_systemIdUffRef.Value;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        //						else
                        //						{
                        //							fV1.argomento=DocsPaWR.FiltriDocumento.DESC_UO_REF.ToString();
                        //							fV1.valore = DocsPaUtils.Functions.Functions.ReplaceApexes(this.txt_descUffRef.Text);
                        //						}		
                    }
                }
                #endregion

                #region Anno protocollo
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                fV1.valore = this.tbAnnoProt.Text;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                //if (this.cbl_archDoc_C.Items.FindByValue("S").Selected)
                //{
                //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //    fV1.argomento = DocsPaWR.FiltriStampaRegistro.ANNO_PROTOCOLLO_STAMPA.ToString();
                //    fV1.valore = this.tbAnnoProt.Text;
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //}
                #endregion

                #region filtro RICERCA IN AREA LAVORO
                if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1"))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_IN_ADL.ToString();
                    fV1.valore = UserManager.getInfoUtente(this).idPeople.ToString() + "@" + UserManager.getRuolo(this).systemId.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region MEZZO SPEDIZIONE
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.MEZZO_SPEDIZIONE.ToString();
                fV1.valore = this.ddl_spedizione.SelectedValue;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion

                #region filtro Data Scadenza
                if (this.ddl_dataScadenza_C.SelectedIndex == 2)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCAD_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataScadenza_C.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCAD_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataScadenza_C.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCAD_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataScadenza_C.SelectedIndex == 0)
                {
                    if (!this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCADENZA_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataScadenza_C.SelectedIndex == 1)
                {
                    if (!this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCADENZA_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");

                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCADENZA_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro su mancanza/presenza Immagine e fascicolazione
                if ((this.cbl_docInCompl.Items.FindByValue("C_Img").Selected && !this.cbl_docInCompl.Items.FindByValue("S_Img").Selected)
                    ||
                  (!this.cbl_docInCompl.Items.FindByValue("C_Img").Selected && this.cbl_docInCompl.Items.FindByValue("S_Img").Selected))
                {
                    if (this.cbl_docInCompl.Items.FindByValue("C_Img").Selected)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.MANCANZA_IMMAGINE.ToString();
                        fV1.valore = "0";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (this.cbl_docInCompl.Items.FindByValue("S_Img").Selected)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.MANCANZA_IMMAGINE.ToString();
                        fV1.valore = "1";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                if ((this.cbl_docInCompl.Items.FindByValue("C_Fasc").Selected && !this.cbl_docInCompl.Items.FindByValue("S_Fasc").Selected)
                    ||
                  (!this.cbl_docInCompl.Items.FindByValue("C_Fasc").Selected && this.cbl_docInCompl.Items.FindByValue("S_Fasc").Selected))
                {
                    if (this.cbl_docInCompl.Items.FindByValue("C_Fasc").Selected)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.MANCANZA_FASCICOLAZIONE.ToString();
                        fV1.valore = "0";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (this.cbl_docInCompl.Items.FindByValue("S_Fasc").Selected)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.MANCANZA_FASCICOLAZIONE.ToString();
                        fV1.valore = "1";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                //}

                #endregion

                #region filtro TRASMESSI_CON
                if (this.cbx_Trasm.Checked)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TRASMESSI_CON.ToString();
                    if (this.ddl_ragioneTrasm.SelectedIndex == 0)
                        fV1.valore = "Tutte";
                    else
                        fV1.valore = this.ddl_ragioneTrasm.SelectedItem.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro TRASMESSI_SENZA
                if (this.cbx_TrasmSenza.Checked)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TRASMESSI_SENZA.ToString();
                    if (this.ddl_ragioneTrasm.SelectedIndex == 0)
                        fV1.valore = "Tutte";
                    else
                        fV1.valore = this.ddl_ragioneTrasm.SelectedItem.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro Conservazione
                if (this.cb_Conservato.Checked)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.CONSERVAZIONE.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                else
                {
                    if (this.cb_NonConservato.Checked)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.CONSERVAZIONE.ToString();
                        fV1.valore = "0";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro Deposito
                //if (!this.rbl_TrasfDep.SelectedItem.Value.Equals("T"))
                //{
                //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //    fV1.argomento = DocsPaWR.FiltriDocumento.DEPOSITO.ToString();
                //    fV1.valore = this.rbl_TrasfDep.SelectedItem.Value;
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //}
                #endregion

                #region filtro file firmati
                if (this.chkFirmato.Checked)
                {
                    //cerco documenti firmati
                    if (!chkNonFirmato.Checked)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                        fV1.valore = "1";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }//se sono entrambi selezionati cerco i documenti che abbiano un file acquisito, siano essi firmati o meno.
                    else
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                        fV1.valore = "2";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {
                    //cerco i documenti non firmati
                    if (chkNonFirmato.Checked)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                        fV1.valore = "0";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                #endregion

                #region filtro tipo file acquisito
                if (this.ddl_tipoFileAcquisiti.SelectedIndex > 0)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIPO_FILE_ACQUISITO.ToString();
                    fV1.valore = this.ddl_tipoFileAcquisiti.SelectedItem.Value;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro protocollo titolario
                if (!string.IsNullOrEmpty(wws.isEnableContatoreTitolario()))
                {
                    //Filtro sul titolario
                    if (!string.IsNullOrEmpty(ddl_Titolario.SelectedValue))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.ID_TITOLARIO.ToString();
                        fV1.valore = ddl_Titolario.SelectedValue;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    //Filtro sul contatore titolario
                    if (!string.IsNullOrEmpty(txt_ProtocolloTitolario.Text))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROT_TITOLARIO.ToString();
                        fV1.valore = txt_ProtocolloTitolario.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro per Stampe Registro
                if (this.cbl_archDoc_C.Items.FindByValue("R") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.STAMPA_REG.ToString();
                    if (this.cbl_archDoc_C.Items.FindByValue("R").Selected)
                        if (this.cbl_archDoc_C.Items.FindByValue("A").Selected ||
                            this.cbl_archDoc_C.Items.FindByValue("P").Selected ||
                            this.cbl_archDoc_C.Items.FindByValue("G").Selected ||
                            this.cbl_archDoc_C.Items.FindByValue("Pr").Selected)
                        {
                            fV1.valore = "U^true";
                        }
                        else
                            fV1.valore = "true";
                    else
                        fV1.valore = "false";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    if (this.ddl_dataStampa_E.SelectedIndex == 2)
                    {
                        // siamo nel caso di Settimana corrente
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
                    if (this.ddl_numProt_C.SelectedIndex == 0)
                    {//valore singolo carico NUM_PROTOCOLLO
                        if (this.txt_initNumProt_C.Text != null && !this.txt_initNumProt_C.Text.Equals(""))
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA.ToString();
                            fV1.valore = this.txt_initNumProt_C.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }
                    else
                    {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                        if (!this.txt_initNumProt_C.Text.Equals(""))
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA_DAL.ToString();
                            fV1.valore = this.txt_initNumProt_C.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        if (!this.txt_fineNumProt_C.Text.Equals(""))
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA_AL.ToString();
                            fV1.valore = this.txt_fineNumProt_C.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }

                    if (!this.tbAnnoProt.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriStampaRegistro.ANNO_PROTOCOLLO_STAMPA.ToString();
                        fV1.valore = this.tbAnnoProt.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
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

                //nuovo filtro per prendere solo i documenti protocollati

                #region filtro riferimento
                if (wws.isEnableRiferimentiMittente() && !string.IsNullOrEmpty(txt_rif_mittente.Text))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.RIFERIMENTO_MITTENTE.ToString();
                    fV1.valore = txt_rif_mittente.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                #endregion

                #region filtro Versioni
                if (!txt_versioni.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUMERO_VERSIONI.ToString();
                    fV1.valore = this.ddl_op_versioni.SelectedValue+this.txt_versioni.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro Allegati
                if (!string.IsNullOrEmpty(txt_allegati.Text))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUMERO_ALLEGATI.ToString();
                    fV1.valore = this.ddl_op_allegati.SelectedValue + this.txt_allegati.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = "NUMERO_ALLEGATI_TIPO";
                    fV1.valore = rblFiltriNumAllegati.SelectedValue;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region Ordinamento

                // Reperimento del filtro da utilizzare per la griglia
                List<FiltroRicerca> filterList = GridManager.GetOrderFilterForDocument(this.ddlOrder.SelectedValue, this.ddlOrderDirection.SelectedValue);

                // Se la lista è valorizzata vengono aggiunti i filtri
                if (filterList != null)
                    foreach (FiltroRicerca filter in filterList)
                        fVList = Utils.addToArrayFiltroRicerca(fVList, filter);

                #endregion

                #region Odinamento contatore no griglie custum
                //Se non ho le griglie custom
                if (!GridManager.IsRoleEnabledToUseGrids())
                {
                    if (ddl_tipoDoc_C.SelectedItem != null && !string.IsNullOrEmpty(ddl_tipoDoc_C.SelectedItem.Text))
                    {
                        DocsPAWA.DocsPaWR.Templates template = ProfilazioneDocManager.getTemplatePerRicercaById(ddl_tipoDoc_C.SelectedItem.Value, this.Page);
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
                                fV1.argomento =FiltriDocumento.PROFILATION_FIELD_FOR_ORDER.ToString();
                                fV1.valore = customObjectTemp.SYSTEM_ID.ToString();
                                fV1.nomeCampo = customObjectTemp.DESCRIZIONE;
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                        }

                        
                    }
                }

                #endregion

                #region Visibilità Tipica / Atipica
                if (pnl_visiblitaDoc.Visible)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.VISIBILITA_T_A.ToString();
                    fV1.valore = rbl_visibilita.SelectedValue;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion Visibilità Tipica / Atipica

                qV[0] = fVList;

                return true;
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
                return false;
            }
        }

        protected void cb_mitt_dest_storicizzati_CheckedChanged(object seder, EventArgs e)
        {
            try
            {
                setDescMittente(this.txt_codMitt_C.Text, "Mit");
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        /// <summary>
        /// apre pop up per la selezione dei fascicoli
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

        private void btn_ricerca_Click(object sender, EventArgs e)
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

              //  ddl_Ric_Salvate.SelectedIndex = 0;
                // controllo dei campi NUM. PROTOCOLLO numerici				
                if (txt_initNumProt_C.Text != "")
                {
                    if (IsNumber(txt_initNumProt_C.Text) == false)
                    {
                        Response.Write("<script>alert('Il numero di protocollo deve essere numerico!');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C.ID + "').focus();</SCRIPT>";
                        RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                if (txt_fineNumProt_C.Text != "")
                {
                    if (IsNumber(txt_fineNumProt_C.Text) == false)
                    {
                        Response.Write("<script>alert('Il numero di protocollo deve essere numerico!');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_fineNumProt_C.ID + "').focus();</SCRIPT>";
                        RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                // fine controllo
                //controllo validità anno inserito
                if (tbAnnoProt.Text != "")
                {
                    if (IsValidYear(tbAnnoProt.Text) == false)
                    {
                        Response.Write("<script>alert('Formato anno non corretto !');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + tbAnnoProt.ID + "').focus();</SCRIPT>";
                        RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                //fine controllo validità anno inserito --- 14/01/2005
                //Controllo intervallo date

                if (this.ddl_dataProt_C.SelectedIndex == 1)
                {
                    if (Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text, this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date di Data Protocollo!');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProt_C").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                if (this.ddl_dataProtMitt_C.SelectedIndex == 1)
                {
                    if (Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text, this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date di Data Protocollo Mittente!');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                if (this.ddl_dataArrivo_C.SelectedIndex == 1)
                {
                    if (Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text, this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date di Data Arrivo!');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                if (this.ddl_dataCreazione_E.SelectedIndex == 1)
                {
                    if (Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text, this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date di Data Creazione!');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                if (this.ddl_dataScadenza_C.SelectedIndex == 1)
                {
                    if (Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text, this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date di Data Scadenza!');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }

                if (this.ddl_dataProtoEme.SelectedIndex == 1)
                {
                    if (Utils.verificaIntervalloDate(this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text, this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date di Data Segnatura Emergenza!');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }

                //Fine controllo intervallo date

                // Controllo lunghezza oggetto inserito
                if (this.txt_ogg_C.Text.Trim() != string.Empty && !FullTextSearch.Configurations.CheckTextMinLenght(this.txt_ogg_C.Text))
                {
                    string message = string.Format("<script>alert('Per ricercare un oggetto è necessario immettere almeno {0} caratteri');</script>", FullTextSearch.Configurations.FullTextMinTextLenght.ToString());
                    Response.Write(message);
                    this.txt_ogg_C.Focus();
                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                    return;
                }

                // Controllo lunghezza descrizione mittente/destinatario inserito
                if (this.txt_descrMitt_C.Text.Trim() != string.Empty && !FullTextSearch.Configurations.CheckTextMinLenght(this.txt_descrMitt_C.Text))
                {
                    string message = string.Format("<script>alert('Per ricercare un mittente/destinatario è necessario immettere almeno {0} caratteri');</script>", FullTextSearch.Configurations.FullTextMinTextLenght.ToString());
                    Response.Write(message);
                    this.txt_descrMitt_C.Focus();
                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                    return;
                }

                //Controllo versioni
                if (this.txt_versioni.Text.Trim() != string.Empty)
                {
                    try
                    {
                        int temp = Int32.Parse(this.txt_versioni.Text);
                    }
                    catch (Exception ex)
                    {
                        string message = "<script>alert('Il numero versioni inserito non è valido');</script>";
                        Response.Write(message);
                        this.txt_versioni.Focus();
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }

                // Controllo sul tipo di ricerca richiesto (ex Tipo Protocollo)
                bool controllo = false;
                for (int i = 0; i < this.cbl_archDoc_C.Items.Count; i++)
                {
                    if (this.cbl_archDoc_C.Items[i].Selected)
                        controllo = true;
                }
                if (!controllo)
                {
                    Response.Write("<script>alert('Inserire almeno un tipo di ricerca');</script>");
                    string s = "<SCRIPT language='javascript'>document.getElementById('" + cbl_archDoc_C.ID + "').focus();</SCRIPT>";
                    RegisterStartupScript("focus", s);
                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                    return;
                }

                if (RicercaCompleta())
                {
                    int numCriteri = 0;
                    if (this.cbl_archDoc_C.Items.FindByValue("A").Selected && this.cbl_archDoc_C.Items.FindByValue("P").Selected && this.cbl_archDoc_C.Items.FindByValue("G").Selected && this.cbl_archDoc_C.Items.FindByValue("Pr").Selected)
                        numCriteri = 1;

                    if (qV[0] == null || qV[0].Length <= numCriteri)
                    {
                        Response.Write("<script>alert('Inserire almeno un criterio di ricerca');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C.ID + "').focus();</SCRIPT>";
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
                                //string stopWord = DocumentManager.verificaStopWord(this, qV[0][i].valore);
                                //if (!string.IsNullOrEmpty(stopWord))
                                //{
                                //    string messaggio = InitMessageXml.getInstance().getMessage("STOP_WORD");
                                //    Response.Write("<script>alert('" + String.Format(messaggio, stopWord) + "');</script>");
                                //    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C.ID + "').focus();</SCRIPT>";
                                //    RegisterStartupScript("focus", s);
                                //    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                //    return;
                                //}
                                if (qV[0][i].valore.StartsWith("%"))
                                {
                                    Response.Write("<script>alert('Il parametro di ricerca non può iniziare con il carattere %');</script>");
                                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C.ID + "').focus();</SCRIPT>";
                                    RegisterStartupScript("focus", s);
                                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                    return;
                                }
                                //if (qV[0][i].valore.Contains("%&&") || qV[0][i].valore.Contains("&&%"))
                                //{
                                //    Response.Write("<script>alert('La combinazione degli operatori && e % non è supportata');</script>");
                                //    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C.ID + "').focus();</SCRIPT>";
                                //    RegisterStartupScript("focus", s);
                                //    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                //    return;
                                //}
                            }
                            if (qV[0][i].argomento.Equals(DocsPaWR.FiltriDocumento.MITT_DEST.ToString()) && !string.IsNullOrEmpty(qV[0][i].valore))
                            {
                                //string stopWord = DocumentManager.verificaStopWord(this, qV[0][i].valore);
                                //if (!string.IsNullOrEmpty(stopWord))
                                //{
                                //    string messaggio = InitMessageXml.getInstance().getMessage("STOP_WORD");
                                //    Response.Write("<script>alert('" + String.Format(messaggio, stopWord) + "');</script>");
                                //    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C.ID + "').focus();</SCRIPT>";
                                //    RegisterStartupScript("focus", s);
                                //    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                //    return;
                                //}
                                if (qV[0][i].valore.StartsWith("%"))
                                {
                                    Response.Write("<script>alert('Il parametro di ricerca non può iniziare con il carattere %');</script>");
                                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C.ID + "').focus();</SCRIPT>";
                                    RegisterStartupScript("focus", s);
                                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                    return;
                                }
                                //if (qV[0][i].valore.Contains("%&&") || qV[0][i].valore.Contains("&&%"))
                                //{
                                //    Response.Write("<script>alert('La combinazione degli operatori && e % non è supportata');</script>");
                                //    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C.ID + "').focus();</SCRIPT>";
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
                    Session.Remove("listInArea");

                    //Reload del frame centrale 
                    //				Response.Write("<script>parent.parent.iFrame_dx.document.location.reload();</script>");

                    //Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDoc.aspx';</script>");	
                    if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=completa&tabRes=completa';</script>");
                    else
                        //Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDoc.aspx';</script>");
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?tabRes=completa';</script>");

                    ViewState["new_search"] = "true";
                }
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void CaricaComboTipologiaAtto(DropDownList ddl)
        {
            DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null)
            {
                //listaTipologiaAtto=DocumentManager.getListaTipologiaAtto(this,UserManager.getInfoUtente(this).idAmministrazione);
                listaTipologiaAtto = DocumentManager.getTipoAttoPDInsRic(this, UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "1");
            }
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
        }

        protected void ddl_numProt_C_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            txt_fineNumProt_C.Text = "";
            if (this.ddl_numProt_C.SelectedIndex == 0)
            {
                this.txt_fineNumProt_C.Visible = false;
                this.lblDAnumprot_C.Visible = false;
                this.lblAnumprot_C.Visible = false;
            }
            else
            {
                this.txt_fineNumProt_C.Visible = true;
                this.lblDAnumprot_C.Visible = true;
                this.lblAnumprot_C.Visible = true;
            }
        }

        protected void ddl_dataProt_C_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataProt_C.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataProt_C").Visible = true;
                    this.GetCalendarControl("txt_initDataProt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataProt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_C").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProt_C").Visible = false;
                    this.GetCalendarControl("txt_fineDataProt_C").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Visible = false;
                    this.lbl_finedataProt_C.Visible = false;
                    this.lbl_initdataProt_C.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataProt_C").Visible = true;
                    this.GetCalendarControl("txt_initDataProt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataProt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_C").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProt_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Enabled = true;
                    this.lbl_finedataProt_C.Visible = true;
                    this.lbl_initdataProt_C.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataProt_C").Visible = true;
                    this.GetCalendarControl("txt_initDataProt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataProt_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt_C").Visible = false;
                    this.GetCalendarControl("txt_fineDataProt_C").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Visible = false;
                    this.lbl_finedataProt_C.Visible = false;
                    this.lbl_initdataProt_C.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataProt_C").Visible = true;
                    this.GetCalendarControl("txt_initDataProt_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataProt_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Enabled = false;
                    this.lbl_finedataProt_C.Visible = true;
                    this.lbl_initdataProt_C.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataProt_C").Visible = true;
                    this.GetCalendarControl("txt_initDataProt_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataProt_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Enabled = false;
                    this.lbl_finedataProt_C.Visible = true;
                    this.lbl_initdataProt_C.Visible = true;
                    break;
            }
        }

        protected void ddl_dataProtMitt_C_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataProtMitt_C.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").Visible = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = false;
                    this.lbl_finedataProtMitt_C.Visible = false;
                    this.lbl_initdataProtMitt_C.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Enabled = true;
                    this.lbl_finedataProtMitt_C.Visible = true;
                    this.lbl_initdataProtMitt_C.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").Visible = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = false;
                    this.lbl_finedataProtMitt_C.Visible = false;
                    this.lbl_initdataProtMitt_C.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Enabled = false;
                    this.lbl_finedataProtMitt_C.Visible = true;
                    this.lbl_initdataProtMitt_C.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Enabled = false;
                    this.lbl_finedataProtMitt_C.Visible = true;
                    this.lbl_initdataProtMitt_C.Visible = true;
                    break;
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

        //Metodo per la costruzione ricorsiva della condizione IN
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
            bool codiceEsatto = (cb_mitt_dest_storicizzati.Checked) ? false : true;
            DocsPaWR.RubricaCallType calltype = DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTDEST;
            if (!codiceRubrica.Equals(""))
            {
                if (tipoMit == "Mit")
                {
                    calltype = DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTDEST;
                    listaCorr = UserManager.getElementiRubricaMultipli(this, codiceRubrica, calltype, codiceEsatto);
                }
                else
                {
                    if (tipoMit == "MitInt")
                    {
                        calltype = DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO;
                        corr = UserManager.getCorrispondenteRubrica(this, codiceRubrica, calltype);
                    }
                }
            }

            if (tipoMit == "Mit")
            {
                if (listaCorr != null && listaCorr.Length > 0)
                {
                    if (listaCorr.Length == 1)
                    {
                        if (!string.IsNullOrEmpty(listaCorr[0].systemId))
                        {
                            corr = UserManager.getCorrispondenteBySystemID(this.Page, listaCorr[0].systemId);
                        }
                        else
                        {
                            corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, listaCorr[0].codice);
                        }
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "multiCorr", "ApriFinestraMultiCorrispondenti();", true);
                        Session.Add("multiCorr", listaCorr);
                        return;
                    }

                }

                if (corr != null)
                {
                    if (!cb_mitt_dest_storicizzati.Checked && !string.IsNullOrEmpty(corr.dta_fine))
                    {
                        this.txt_codMitt_C.Text = "";
                        this.txt_descrMitt_C.Text = "";
                        this.hd_systemIdMit_Com.Value = "";
                    }
                    else
                    {
                        this.txt_codMitt_C.Text = corr.codiceRubrica;
                        this.txt_descrMitt_C.Text = UserManager.getDecrizioneCorrispondenteSemplice(corr);
                        this.hd_systemIdMit_Com.Value = corr.systemId;
                    }
                }

                else
                {
                        this.txt_codMitt_C.Text = "";
                        this.txt_descrMitt_C.Text = "";
                        this.hd_systemIdMit_Com.Value = "";
                }
            }

            else if (tipoMit == "MitInt")
            {
                if (corr != null)
                {
                    this.txt_descrMittInter_C.Text = UserManager.getDecrizioneCorrispondenteSemplice(corr);
                    this.hd_systemIdMitInt.Value = corr.systemId;
                }
                else
                {
                    this.txt_codMittInter_C.Text = "";
                    this.txt_descrMittInter_C.Text = "";
                    this.hd_systemIdMitInt.Value = "";
                }
            }
        }

        private void setDescUffRef(string codiceRubrica)
        {
            DocsPaWR.Corrispondente corr = null;
            string msg = "Codice rubrica non valido per l\\'Ufficio referente!";
            if (!codiceRubrica.Equals(""))
                corr = UserManager.getCorrispondenteReferente(this, codiceRubrica, false);
            if (corr != null && (corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))))
            {
                this.txt_descUffRef.Text = UserManager.getDecrizioneCorrispondenteSemplice(corr);
                this.hd_systemIdUffRef.Value = corr.systemId;
            }
            else
            {
                if (!codiceRubrica.Equals(""))
                {
                    RegisterStartupScript("alert", "<script language='javascript'>alert('" + msg + "');</script>");
                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_codUffRef.ID + "').focus() </SCRIPT>";
                    RegisterStartupScript("focus", s);
                }
                this.txt_codUffRef.Text = "";
                this.txt_descUffRef.Text = "";
                this.hd_systemIdUffRef.Value = "";
            }
        }

        private void txt_codMitt_C_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                setDescMittente(this.txt_codMitt_C.Text, "Mit");
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void txt_codMittInter_C_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                setDescMittente(this.txt_codMittInter_C.Text, "MitInt");
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void ddl_dataArrivo_C_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataArrivo_C.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataArrivo_C").Visible = true;
                    this.GetCalendarControl("txt_initDataArrivo_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataArrivo_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataArrivo_C").Visible = false;
                    this.GetCalendarControl("txt_fineDataArrivo_C").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Visible = false;
                    this.lbl_fineDataArrivo_C.Visible = false;
                    this.lbl_initDataArrivo_C.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataArrivo_C").Visible = true;
                    this.GetCalendarControl("txt_initDataArrivo_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataArrivo_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataArrivo_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataArrivo_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataArrivo_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Enabled = true;
                    this.lbl_fineDataArrivo_C.Visible = true;
                    this.lbl_initDataArrivo_C.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataArrivo_C").Visible = true;
                    this.GetCalendarControl("txt_initDataArrivo_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataArrivo_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataArrivo_C").Visible = false;
                    this.GetCalendarControl("txt_fineDataArrivo_C").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Visible = false;
                    this.lbl_fineDataArrivo_C.Visible = false;
                    this.lbl_initDataArrivo_C.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataArrivo_C").Visible = true;
                    this.GetCalendarControl("txt_initDataArrivo_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataArrivo_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataArrivo_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataArrivo_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Enabled = false;
                    this.lbl_fineDataArrivo_C.Visible = true;
                    this.lbl_initDataArrivo_C.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataArrivo_C").Visible = true;
                    this.GetCalendarControl("txt_initDataArrivo_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataArrivo_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataArrivo_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataArrivo_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Enabled = false;
                    this.lbl_fineDataArrivo_C.Visible = true;
                    this.lbl_initDataArrivo_C.Visible = true;
                    break;
            }
        }

        private void enterKeySimulator_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.btn_ricerca_Click(null, null);
        }

        public bool IsValidYear(string strYear)
        {
            Regex onlyNumberPattern = new Regex(@"\d{4}");
            return onlyNumberPattern.IsMatch(strYear);
        }

      /*  private void txt_CodFascicolo_TextChanged(object sender, System.EventArgs e)
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
                        //txt_CodFascicolo.Text = listaFasc[0].codice;
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

        #region ProfilazioneDinamica
        private void verificaCampiPersonalizzati()
        {
            DocsPaWR.Templates template = new DocsPAWA.DocsPaWR.Templates();
            if (!ddl_tipoDoc_C.SelectedValue.Equals(""))
            {
                template = (DocsPAWA.DocsPaWR.Templates)Session["templateRicerca"];
                if (Session["templateRicerca"] == null)
                {
                    template = ProfilazioneDocManager.getTemplatePerRicerca((UserManager.getInfoUtente(this)).idAmministrazione, ddl_tipoDoc_C.SelectedItem.Text, this);
                    Session.Add("templateRicerca", template);
                }
                if (template != null && !(ddl_tipoDoc_C.SelectedItem.Text.ToUpper()).Equals(template.DESCRIZIONE.ToUpper()))
                {
                    template = ProfilazioneDocManager.getTemplatePerRicerca((UserManager.getInfoUtente(this)).idAmministrazione, ddl_tipoDoc_C.SelectedItem.Text, this);
                    Session.Add("templateRicerca", template);
                }
            }
            if (template != null && template.SYSTEM_ID == 0)
            {
                btn_CampiPersonalizzati.Visible = false;
            }
            else
            {
                if (template != null && template.ELENCO_OGGETTI.Length != 0)
                {
                    btn_CampiPersonalizzati.Visible = true;
                }
                else
                {
                    btn_CampiPersonalizzati.Visible = false;
                }
            }
        }

        private void attivaProfilazioneDinamica()
        {
            //PROFILAZIONE DINAMICA
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null)
            {
                //DIAGRAMMI DI STATO
                if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                {
                    //Verifico se esiste un diagramma di stato associato al tipo di documento
                    //Modifica per la visualizzazione solo degli stati per cui esistono documenti in essi
                    string idDiagramma = DocsPAWA.DiagrammiManager.getDiagrammaAssociato(ddl_tipoDoc_C.SelectedValue, this).ToString();
                    if (ddl_tipoDoc_C.SelectedValue != "" && !string.IsNullOrEmpty(idDiagramma))
                    {
                        Panel_StatiDocumento.Visible = true;

                        //Inizializzazione comboBox
                        ddl_statiDoc.Items.Clear();
                        ListItem itemEmpty = new ListItem();
                        ddl_statiDoc.Items.Add(itemEmpty);

                        DocsPaWR.Stato[] statiDg = DocsPAWA.DiagrammiManager.getStatiPerRicerca(idDiagramma, "D", this);
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
                    //Verifico se esiste un diagramma di stato associato al tipo di documento
                    DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDgByIdTipoDoc(ddl_tipoDoc_C.SelectedValue, (UserManager.getInfoUtente(this)).idAmministrazione, this);
                    Session.Add("DiagrammaSelezionato", dg);
                    if (ddl_tipoDoc_C.SelectedValue != "" && dg != null)
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
                //FINE DIAGRAMMI STATO
            }
            //FINE PROFILAZIONE DINAMICA
        }


        private void ddl_tipoDoc_C_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Session.Remove("templateRicerca");
            Session.Remove("rubrica.campoCorrispondente");
            Session.Remove("dictionaryCorrispondente");
            Session.Remove("noRicercaCodice");
            Session.Remove("noRicercaDesc");
            schedaRicerca.RimuoviFiltro(DocsPAWA.DocsPaWR.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString());
            attivaProfilazioneDinamica();
            //Se non ho le griglie custom
            if (!GridManager.IsRoleEnabledToUseGrids())
            {
                if (ddl_tipoDoc_C.SelectedItem != null && !string.IsNullOrEmpty(ddl_tipoDoc_C.SelectedItem.Text))
                {
                    DocsPAWA.DocsPaWR.Templates template = ProfilazioneDocManager.getTemplatePerRicerca((UserManager.getInfoUtente(this)).idAmministrazione, ddl_tipoDoc_C.SelectedItem.Text, this);
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
                            GridManager.CompileDdlOrderAndSetOrderFilterDocuments(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                        }
                    }

                }
            }
        }
        #endregion

        private void txt_codUffRef_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                setDescUffRef(this.txt_codUffRef.Text);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void btn_CampiPersonalizzati_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            RicercaCompleta();
            schedaRicerca.FiltriRicerca = qV;
            Session.Remove("noRicercaCodice");
            Session.Remove("noRicercaDesc");
            RegisterStartupScript("Apri", "<script>apriPopupAnteprima();</script>");
        }

        private void tastoInvio()
        {
            Utils.DefaultButton(this, ref txt_initNumProt_C, ref btn_ricerca);
            Utils.DefaultButton(this, ref txt_fineNumProt_C, ref btn_ricerca);
            Utils.DefaultButton(this, ref tbAnnoProt, ref btn_ricerca);
            Utils.DefaultButton(this, ref this.GetCalendarControl("txt_initDataProt_C").txt_Data, ref btn_ricerca);
            Utils.DefaultButton(this, ref this.GetCalendarControl("txt_fineDataProt_C").txt_Data, ref btn_ricerca);
            Utils.DefaultButton(this, ref txt_segnatura, ref btn_ricerca);
            Utils.DefaultButton(this, ref txt_ogg_C, ref btn_ricerca);
            Utils.DefaultButton(this, ref txt_numProtMitt_C, ref btn_ricerca);
            Utils.DefaultButton(this, ref this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data, ref btn_ricerca);
            Utils.DefaultButton(this, ref this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data, ref btn_ricerca);
            Utils.DefaultButton(this, ref txt_nomeFirma_C, ref btn_ricerca);
            Utils.DefaultButton(this, ref txt_cognomeFirma_C, ref btn_ricerca);
            TextBox note = rn_note.getTextBox();
            Utils.DefaultButton(this, ref note, ref btn_ricerca);

        }

        private void mb_ConfirmDelete_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                try
                {
                    schedaRicerca.Cancella(Int32.Parse(ddl_Ric_Salvate.SelectedValue));
                    Response.Write("<script>alert(\"I criteri di ricerca sono stati rimossi\");window.location.href = window.location.href;</script>");
                }
                catch (Exception ex)
                {
                    string msg = "Impossibile rimuovere i criteri di ricerca. Errore: " + ex.Message;
                    msg = msg.Replace("\"", "\\\"");
                    Response.Write("<script>alert(\"" + msg + "\");window.location.href = window.location.href;</script>");
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
                            Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=completa&tabRes=completa';</script>");
                        else
                            Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?tabRes=completa';</script>");
                    }
                }
                catch (Exception ex_)
                {
                    string msg = ex_.Message;
                    msg = msg + " Rimuovere i criteri di ricerca selezionati.";
                    msg = msg.Replace("\"", "\\\"");
                    Response.Write("<script>alert(\"" + msg + "\");window.location.href = window.location.href;</script>");
                }
                this.btn_modifica.Enabled = true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                msg = msg.Replace("\"", "\\\"");
                Response.Write("<script>alert(\"" + msg + "\");window.location.href = window.location.href;</script>");
            }
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

        private void btn_salva_Click(object sender, EventArgs e)
        {
            if (RicercaCompleta())
            {
                // Impostazione del filtro utilizzato
                GridManager.SetSearchFilter(this.ddlOrder.SelectedItem.Text, this.ddlOrderDirection.SelectedValue);

                schedaRicerca.FiltriRicerca = qV;
                schedaRicerca.ProprietaNuovaRicerca = new DocsPAWA.ricercaDoc.SchedaRicerca.NuovaRicerca();
                if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1"))
                    RegisterStartupScript("SalvaRicerca", "<script>apriSalvaRicercaADL();</script>");
                else
                    RegisterStartupScript("SalvaRicerca", "<script>apriSalvaRicerca();</script>");
            }
        }

        protected void ddl_dataStampa_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
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
                    this.GetCalendarControl("txt_initDataStampa_E").btn_Cal.Enabled = true;
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

        protected void ddl_dataScadenza_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataScadenza_C.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataScadenza_C").Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataScadenza_C").Visible = false;
                    this.GetCalendarControl("txt_fineDataScadenza_C").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Visible = false;
                    this.lbl_fineDataScadenza_C.Visible = false;
                    this.lbl_initDataScadenza_C.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataScadenza_C").Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataScadenza_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Enabled = true;
                    this.lbl_fineDataScadenza_C.Visible = true;
                    this.lbl_initDataScadenza_C.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataScadenza_C").Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataScadenza_C").Visible = false;
                    this.GetCalendarControl("txt_fineDataScadenza_C").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Visible = false;
                    this.lbl_fineDataScadenza_C.Visible = false;
                    this.lbl_initDataScadenza_C.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataScadenza_C").Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataScadenza_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Enabled = false;
                    this.lbl_fineDataScadenza_C.Visible = true;
                    this.lbl_initDataScadenza_C.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataScadenza_C").Visible = true;
                    this.GetCalendarControl("txt_initDataScadenza_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataScadenza_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Enabled = false;
                    this.lbl_fineDataScadenza_C.Visible = true;
                    this.lbl_initDataScadenza_C.Visible = true;
                    break;
            }
        }

        protected void ddl_dataProtoEme_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataProtoEme.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_dataProtoEmeInizio").Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_dataProtoEmeFine").Visible = false;
                    this.GetCalendarControl("txt_dataProtoEmeFine").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Visible = false;
                    this.lbl_dataProtoEmeFine.Visible = false;
                    this.lbl_dataProtoEmeInizio.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_dataProtoEmeInizio").Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_dataProtoEmeFine").Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeFine").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeFine").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Enabled = true;
                    this.lbl_dataProtoEmeFine.Visible = true;
                    this.lbl_dataProtoEmeInizio.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_dataProtoEmeInizio").Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_dataProtoEmeFine").Visible = false;
                    this.GetCalendarControl("txt_dataProtoEmeFine").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Visible = false;
                    this.lbl_dataProtoEmeFine.Visible = false;
                    this.lbl_dataProtoEmeInizio.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_dataProtoEmeInizio").Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_dataProtoEmeFine").Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeFine").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeFine").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Enabled = false;
                    this.lbl_dataProtoEmeFine.Visible = true;
                    this.lbl_dataProtoEmeInizio.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_dataProtoEmeInizio").Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_dataProtoEmeFine").Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeFine").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeFine").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Enabled = false;
                    this.lbl_dataProtoEmeFine.Visible = true;
                    this.lbl_dataProtoEmeInizio.Visible = true;
                    break;
            }
        }

        protected void ddl_dataCreazione_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
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

        private bool PopulateField(DocsPAWA.DocsPaWR.FiltroRicerca[][] qV, bool grid)
        {
            try
            {
                if (qV == null || qV.Length == 0)
                    return false;

                #region pulizia campi
                #region Mancanza_immagine, Mancanza_Fascicolazione e Predisposto alla Protocollazione
                for (int i = 0; i < this.cbl_docInCompl.Items.Count; i++)
                {
                    this.cbl_docInCompl.Items[i].Selected = false;
                }
                #endregion
                #region TIPO
                //rb_archDoc_C.SelectedValue="T";
                //for (int i = 0; i < this.cbl_archDoc_C.Items.Count; i++)
                //{
                //    this.cbl_archDoc_C.Items[i].Selected = true;
                //}
                #endregion TIPO
                #region REGISTRO
                foreach (ListItem i in lb_reg_C.Items)
                    i.Selected = true;
                #endregion REGISTRO
                #region NUM_PROTOCOLLO
                ddl_numProt_C.SelectedIndex = 0;
                ddl_numProt_C_SelectedIndexChanged(null, new System.EventArgs());
                txt_initNumProt_C.Text = "";
                #endregion NUM_PROTOCOLLO
                #region ANNO_PROTOCOLLO
                tbAnnoProt.Text = DateTime.Now.Year.ToString();
                #endregion ANNO_PROTOCOLLO
                #region DATA_PROT
                ddl_dataProt_C.SelectedIndex = 0;
                ddl_dataProt_C_SelectedIndexChanged(null, new System.EventArgs());
                this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text = "";
                #endregion DATA_PROT
                #region ANNULLATO
                rb_annulla_C.SelectedValue = "T";
                #endregion ANNULLATO
                #region TIPO_ATTO
                ddl_tipoDoc_C.SelectedIndex = 0;
                #endregion TIPO_ATTO
                #region DATA_STAMPA_REGISTRO (Commentato)
                //ddl_dataStampa.SelectedIndex = 0;
                //ddl_dataStampa_SelectedIndexChanged(null, new System.EventArgs());
                //this.GetCalendarControl("txt_initDataStampa").txt_Data.Text = "";
                #endregion DATA_STAMPA_REGISTRO
                #region SEGNATURA
                txt_segnatura.Text = "";
                #endregion SEGNATURA
                #region OGGETTO
                txt_ogg_C.Text = "";
                #endregion OGGETTO
                #region MITT_DEST
                txt_codMitt_C.Text = "";
                txt_descrMitt_C.Text = "";
                #endregion MITT_DEST
                #region ID_UO_REF
                txt_codUffRef.Text = "";
                txt_descUffRef.Text = "";
                #endregion ID_UO_REF
                #region MITTENTE_INTERMEDIO
                txt_codMittInter_C.Text = "";
                txt_descrMittInter_C.Text = "";
                #endregion MITTENTE_INTERMEDIO
                #region PROTOCOLLO_MITTENTE
                txt_numProtMitt_C.Text = "";
                #endregion PROTOCOLLO_MITTENTE
                #region DATA_PROT_MITTENTE
                ddl_dataProtMitt_C.SelectedIndex = 0;
                ddl_dataProtMitt_C_SelectedIndexChanged(null, new System.EventArgs());
                this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = "";
                #endregion DATA_PROT_MITTENTE
                #region DATA_ARRIVO
                ddl_dataArrivo_C.SelectedIndex = 0;
                ddl_dataArrivo_C_SelectedIndexChanged(null, new System.EventArgs());
                this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text = "";
                #endregion DATA_ARRIVO
                #region PAROLE_CHIAVE
                ListParoleChiave.Items.Clear();
                #endregion PAROLE_CHIAVE
                #region COMMISSIONE_REF
                txt_commRef.Text = "";
                #endregion COMMISSIONE_REF
                #region NOTE
                rn_note.Testo = "";
                #endregion NOTE
                #region NUM_PROTO_EMERGENZA
                txt_protoEme.Text = "";
                #endregion NUM_PROTO_EMERGENZA
                #region DATA_PROTO_EMERGENZA
                this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text = "";
                this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Text = "";
                #endregion DATA_PROTO_EMERGENZA
                #region FIRMATARIO
                txt_nomeFirma_C.Text = "";
                txt_cognomeFirma_C.Text = "";
                #endregion FIRMATARIO
                #region NUM_OGGETTO
                txt_numOggetto.Text = "";
                #endregion NUM_OGGETTO
                #region DOCNUMBER
                ddl_idDocumento_C.SelectedIndex = 0;
                ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
                txt_initIdDoc_C.Text = "";
                #endregion DOCNUMBER
                #region DATA_CREAZIONE
                ddl_dataCreazione_E.SelectedIndex = 0;
                ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text = "";
                #endregion DATA_CREAZIONE
                #region EVIDENZA
                rb_evidenza_C.SelectedValue = "T";
                #endregion EVIDENZA
                #region Trasmissione Con/Senza
                this.cbx_Trasm.Checked = false;
                this.cbx_TrasmSenza.Checked = false;
                #endregion
                /*
				#region DIAGRAMMA_STATO_DOC
				ddl_statiDoc.SelectedIndex = 0;
				#endregion DIAGRAMMA_STATO_DOC
                */
                #region FASCICOLO
                txt_CodFascicolo.Text = "";
                txt_DescFascicolo.Text = "";
                FascicoliManager.setFascicoliSelezionati(this, null);
                #endregion FASCICOLO
                #region MEZZO SPEDIZIONE
                this.ddl_spedizione.SelectedIndex = 0;
                #endregion
                #region DATA SCADENZA
                ddl_dataScadenza_C.SelectedIndex = 0;
                ddl_dataScadenza_SelectedIndexChanged(null, new System.EventArgs());
                this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text = "";
                #endregion
                #region CONSERVAZIONE
                //this.rb_conservazione.Items[0].Selected = false;
                //this.rb_conservazione.Items[1].Selected = false;
                #endregion
                #region DEPOSITO
                //rbl_TrasfDep.SelectedValue = "T";
                #endregion DEPOSITO
                #region RIFERIMENTO MITTENTE
                if (wws.isEnableRiferimentiMittente())
                {
                    this.txt_rif_mittente.Text = string.Empty;
                }
                #endregion
                #endregion pulizia campi

                DocsPaWR.FiltroRicerca[] filters = qV[0];
                //array contenitore degli array filtro di ricerca
                foreach (DocsPAWA.DocsPaWR.FiltroRicerca aux in filters)
                {
                    try
                    {
                        #region TIPO
                        if (aux.argomento == DocsPaWR.FiltriDocumento.TIPO.ToString())
                        {
                            //string[] tipoDoc = aux.valore.Split('^');
                            //if (tipoDoc[0].Equals("1"))
                            //    this.cbl_archDoc_C.Items.FindByValue("A").Selected = true;
                            //else
                            //    this.cbl_archDoc_C.Items.FindByValue("A").Selected = false;
                            //if (tipoDoc[1].Equals("1"))
                            //    this.cbl_archDoc_C.Items.FindByValue("P").Selected = true;
                            //else
                            //    this.cbl_archDoc_C.Items.FindByValue("P").Selected = false;
                            //if (tipoDoc[2].Equals("1"))
                            //    if (this.cbl_archDoc_C.Items.FindByValue("I") != null)
                            //        this.cbl_archDoc_C.Items.FindByValue("I").Selected = true;
                            //    else
                            //        if (this.cbl_archDoc_C.Items.FindByValue("I") != null)
                            //            this.cbl_archDoc_C.Items.FindByValue("I").Selected = false;
                            //if (tipoDoc[3].Equals("1"))
                            //    this.cbl_archDoc_C.Items.FindByValue("G").Selected = true;
                            //else
                            //    this.cbl_archDoc_C.Items.FindByValue("G").Selected = false;
                            //if (tipoDoc[4].Equals("1"))
                            //    this.cbl_archDoc_C.Items.FindByValue("Pr").Selected = true;
                            //else
                            //    this.cbl_archDoc_C.Items.FindByValue("Pr").Selected = false;
                            //if (tipoDoc[5].Equals("1"))
                            //    this.cbl_archDoc_C.Items.FindByValue("S").Selected = true;
                            //else
                            //    this.cbl_archDoc_C.Items.FindByValue("S").Selected = false;
                            //rb_archDoc_E.SelectedValue = aux.valore;
                        }
                        #endregion TIPO
                        #region PROTOCOLLO_ARRIVO
                        if (aux.argomento == DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString())
                        {
                            this.cbl_archDoc_C.Items.FindByValue("A").Selected = Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region PROTOCOLLO_PARTENZA
                        if (aux.argomento == DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString())
                        {
                            this.cbl_archDoc_C.Items.FindByValue("P").Selected = Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region PROTOCOLLO_INTERNO
                        if (aux.argomento == DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString())
                        {
                            if (this.cbl_archDoc_C.Items.FindByValue("I") != null)
                                this.cbl_archDoc_C.Items.FindByValue("I").Selected = Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region GRIGI
                        if (aux.argomento == DocsPaWR.FiltriDocumento.GRIGIO.ToString())
                        {
                            this.cbl_archDoc_C.Items.FindByValue("G").Selected = Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region PREDISPOSTI
                        if (aux.argomento == DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString())
                        {
                            this.cbl_archDoc_C.Items.FindByValue("Pr").Selected = Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region TRASMISSIONE CON
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.TRASMESSI_CON.ToString())
                        {
                            this.cbx_Trasm.Checked = true;
                            if (aux.valore.Equals("Tutte"))
                                this.ddl_ragioneTrasm.SelectedIndex = 0;
                            else
                                this.ddl_ragioneTrasm.SelectedValue = aux.valore;
                        }
                        #endregion
                        #region TRASMISSIONE SENZA
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.TRASMESSI_SENZA.ToString())
                        {
                            this.cbx_TrasmSenza.Checked = true;
                            if (aux.valore.Equals("Tutte"))
                                this.ddl_ragioneTrasm.SelectedIndex = 0;
                            else
                                this.ddl_ragioneTrasm.SelectedValue = aux.valore;
                        }
                        #endregion
                        #region REGISTRO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.REGISTRO.ToString())
                        {
                            char[] sep = { ',' };
                            string[] regs = aux.valore.Split(sep);
                            foreach (ListItem li in lb_reg_C.Items)
                                li.Selected = false;
                            foreach (string reg in regs)
                            {
                                for (int i = 0; i < lb_reg_C.Items.Count; i++)
                                {
                                    if (lb_reg_C.Items[i].Value == reg)
                                        lb_reg_C.Items[i].Selected = true;
                                }
                            }
                        }
                        #endregion REGISTRO
                        #region NUM_PROTOCOLLO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString())
                        {
                            if (ddl_numProt_C.SelectedIndex != 0)
                                ddl_numProt_C.SelectedIndex = 0;
                            ddl_numProt_C_SelectedIndexChanged(null, new System.EventArgs());
                            txt_initNumProt_C.Text = aux.valore;
                        }
                        #endregion NUM_PROTOCOLLO
                        #region NUM_PROTOCOLLO_DAL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString())
                        {
                            if (ddl_numProt_C.SelectedIndex != 1)
                                ddl_numProt_C.SelectedIndex = 1;
                            ddl_numProt_C_SelectedIndexChanged(null, new System.EventArgs());
                            txt_initNumProt_C.Text = aux.valore;
                        }
                        #endregion NUM_PROTOCOLLO_DAL
                        #region NUM_PROTOCOLLO_AL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString())
                        {
                            if (ddl_numProt_C.SelectedIndex != 1)
                                ddl_numProt_C.SelectedIndex = 1;
                            ddl_numProt_C_SelectedIndexChanged(null, new System.EventArgs());
                            txt_fineNumProt_C.Text = aux.valore;
                        }
                        #endregion NUM_PROTOCOLLO_AL
                        #region ANNO_PROTOCOLLO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString())
                        {
                            tbAnnoProt.Text = aux.valore;
                        }
                        #endregion ANNO_PROTOCOLLO
                        #region DATA_PROT_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString())
                        {
                            if (ddl_dataProt_C.SelectedIndex != 0)
                                ddl_dataProt_C.SelectedIndex = 0;
                            ddl_dataProt_C_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_PROT_IL
                        #region DATA_PROT_SUCCESSIVA_AL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString())
                        {
                            if (ddl_dataProt_C.SelectedIndex != 1)
                                ddl_dataProt_C.SelectedIndex = 1;
                            ddl_dataProt_C_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_PROT_SUCCESSIVA_AL
                        #region DATA_PROT_PRECEDENTE_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString())
                        {
                            if (ddl_dataProt_C.SelectedIndex != 1)
                                ddl_dataProt_C.SelectedIndex = 1;
                            ddl_dataProt_C_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_PROT_PRECEDENTE_IL
                        #region DATA_PROT_SC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_SC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProt_C.SelectedIndex = 3;
                            this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                            this.GetCalendarControl("txt_initDataProt_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataProt_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataProt_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProt_C").btn_Cal.Enabled = false;
                            this.lbl_finedataProt_C.Visible = true;
                            this.lbl_initdataProt_C.Visible = true;
                        }
                        #endregion
                        #region DATA_PROT_MC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProt_C.SelectedIndex = 4;
                            this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                            this.GetCalendarControl("txt_initDataProt_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataProt_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataProt_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProt_C").btn_Cal.Enabled = false;
                            this.lbl_finedataProt_C.Visible = true;
                            this.lbl_initdataProt_C.Visible = true;
                        }
                        #endregion
                        #region DATA_PROT_TODAY
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_TODAY.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProt_C.SelectedIndex = 2;
                            this.GetCalendarControl("txt_initDataProt_C").Visible = true;
                            this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                            this.GetCalendarControl("txt_initDataProt_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataProt_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataProt_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataProt_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProt_C").Visible = false;
                            this.GetCalendarControl("txt_fineDataProt_C").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_fineDataProt_C").btn_Cal.Visible = false;
                            this.lbl_finedataProt_C.Visible = false;
                            this.lbl_initdataProt_C.Visible = false;
                        }
                        #endregion
                        #region DATA_STAMPA_IL
                        //else if (aux.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO.ToString())
                        //{
                        //    if (ddl_dataStampa.SelectedIndex != 0)
                        //        ddl_dataStampa.SelectedIndex = 0;
                        //    ddl_dataStampa_SelectedIndexChanged(null, new System.EventArgs());
                        //    this.GetCalendarControl("txt_initDataStampa").txt_Data.Text = aux.valore;
                        //}
                        #endregion DATA_STAMPA_IL
                        #region DATA_STAMPA_SUCCESSIVA_AL
                        //else if (aux.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_DAL.ToString())
                        //{
                        //    if (ddl_dataStampa.SelectedIndex != 1)
                        //        ddl_dataStampa.SelectedIndex = 1;
                        //    ddl_dataStampa_SelectedIndexChanged(null, new System.EventArgs());
                        //    this.GetCalendarControl("txt_initDataStampa").txt_Data.Text = aux.valore;
                        //}
                        #endregion DATA_STAMPA_SUCCESSIVA_AL
                        #region DATA_STAMPA_PRECEDENTE_IL
                        //else if (aux.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_AL.ToString())
                        //{
                        //    if (ddl_dataStampa.SelectedIndex != 1)
                        //        ddl_dataStampa.SelectedIndex = 1;
                        //    ddl_dataStampa_SelectedIndexChanged(null, new System.EventArgs());
                        //    //this.GetCalendarControl("txt_fineDataStampa").Visible = true;
                        //    //this.GetCalendarControl("txt_fineDataStampa").btn_Cal.Visible = true;
                        //    //this.GetCalendarControl("txt_fineDataStampa").txt_Data.Visible = true;
                        //    //this.lbl_initDataStampa.Visible = true;
                        //    //this.lbl_fineDataStampa.Visible = true;
                        //    this.GetCalendarControl("txt_fineDataStampa").txt_Data.Text = aux.valore;
                        //}
                        #endregion DATA_STAMPA_PRECEDENTE_IL
                        #region DATA_STAMPA_SC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_STAMPA_SC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataStampa_E.SelectedIndex = 3;
                            this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                            this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataStampa_E").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_finedataStampa_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_finedataStampa_E").btn_Cal.Enabled = false;
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
                            this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_finedataStampa_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_finedataStampa_E").btn_Cal.Enabled = false;
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
                            this.GetCalendarControl("txt_finedataStampa_E").Visible = false;
                            this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_finedataStampa_E").btn_Cal.Visible = false;
                            this.lbl_dataStampaA.Visible = false;
                            this.lbl_dataStampaDa.Visible = false;
                        }
                        #endregion
                        #region ANNULLATO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ANNULLATO.ToString())
                        {
                            rb_annulla_C.SelectedValue = aux.valore;
                        }
                        #endregion ANNULLATO
                        #region TIPO_ATTO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString())
                        {
                            ddl_tipoDoc_C.SelectedValue = aux.valore;
                            attivaProfilazioneDinamica();
                            //							ddl_tipoDoc_C_SelectedIndexChanged(null, new System.EventArgs());
                        }
                        #endregion TIPO_ATTO
                        #region SEGNATURA
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.SEGNATURA.ToString())
                        {
                            txt_segnatura.Text = aux.valore;
                        }
                        #endregion SEGNATURA
                        #region OGGETTO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.OGGETTO.ToString())
                        {
                            txt_ogg_C.Text = aux.valore;
                        }
                        #endregion OGGETTO
                        #region MITT_DEST
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.MITT_DEST.ToString())
                        {
                            txt_descrMitt_C.Text = aux.valore;
                        }
                        #endregion MITT_DEST
                        #region COD_MITT_DEST
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString())
                        {
                            DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubrica(this, aux.valore);
                            txt_codMitt_C.Text = corr.codiceRubrica;
                            txt_descrMitt_C.Text = corr.descrizione;
                        }
                        #endregion
                        #region MITT_DEST_STORICIZZATI
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString())
                        {
                            bool chkValue;
                            bool.TryParse(aux.valore, out chkValue);
                            this.cb_mitt_dest_storicizzati.Checked = chkValue;
                        }
                        #endregion
                        #region ID_MITT_DEST
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString())
                        {
                            DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteBySystemID(this, aux.valore);
                            txt_codMitt_C.Text = corr.codiceRubrica;
                            txt_descrMitt_C.Text = corr.descrizione;
                        }
                        #endregion ID_MITT_DEST
                        #region ID_UO_REF
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ID_UO_REF.ToString())
                        {
                            DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteBySystemID(this, aux.valore);
                            txt_codUffRef.Text = corr.codiceRubrica;
                            txt_descUffRef.Text = corr.descrizione;
                        }
                        #endregion ID_UO_REF
                        #region MITTENTE_INTERMEDIO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.MITTENTE_INTERMEDIO.ToString())
                        {
                            txt_descrMittInter_C.Text = aux.valore;
                        }
                        #endregion MITTENTE_INTERMEDIO
                        #region ID_MITTENTE_INTERMEDIO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ID_MITTENTE_INTERMEDIO.ToString())
                        {
                            DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteBySystemID(this, aux.valore);
                            txt_codMittInter_C.Text = corr.codiceRubrica;
                            txt_descrMittInter_C.Text = corr.descrizione;
                        }
                        #endregion ID_MITTENTE_INTERMEDIO
                        #region PROTOCOLLO_MITTENTE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.PROTOCOLLO_MITTENTE.ToString())
                        {
                            txt_numProtMitt_C.Text = aux.valore;
                        }
                        #endregion PROTOCOLLO_MITTENTE
                        #region DATA_PROT_MITTENTE_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_IL.ToString())
                        {
                            if (ddl_dataProtMitt_C.SelectedIndex != 0)
                                ddl_dataProtMitt_C.SelectedIndex = 0;
                            ddl_dataProtMitt_C_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_PROT_MITTENTE_IL
                        #region DATA_PROT_MITTENTE_SUCCESSIVA_AL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_SUCCESSIVA_AL.ToString())
                        {
                            if (ddl_dataProtMitt_C.SelectedIndex != 1)
                                ddl_dataProtMitt_C.SelectedIndex = 1;
                            ddl_dataProtMitt_C_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_PROT_MITTENTE_SUCCESSIVA_AL
                        #region DATA_PROT_MITTENTE_PRECEDENTE_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_PRECEDENTE_IL.ToString())
                        {
                            if (ddl_dataProtMitt_C.SelectedIndex != 1)
                                ddl_dataProtMitt_C.SelectedIndex = 1;
                            ddl_dataProtMitt_C_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_PROT_MITTENTE_PRECEDENTE_IL
                        #region DATA_PROT_MITTENTE_SC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_SC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProtMitt_C.SelectedIndex = 3;
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Enabled = false;
                            this.lbl_finedataProtMitt_C.Visible = true;
                            this.lbl_initdataProtMitt_C.Visible = true;
                        }
                        #endregion
                        #region DATA_PROT_MITTENTE_MC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_MC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProtMitt_C.SelectedIndex = 4;
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Enabled = false;
                            this.lbl_finedataProtMitt_C.Visible = true;
                            this.lbl_initdataProtMitt_C.Visible = true;
                        }
                        #endregion
                        #region DATA_PROT_MITTENTE_TODAY
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_TODAY.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProtMitt_C.SelectedIndex = 2;
                            this.GetCalendarControl("txt_initDataProtMitt_C").Visible = true;
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").Visible = false;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = false;
                            this.lbl_finedataProtMitt_C.Visible = false;
                            this.lbl_initdataProtMitt_C.Visible = false;
                        }
                        #endregion
                        #region DATA_ARRIVO_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_ARRIVO_IL.ToString())
                        {
                            if (ddl_dataArrivo_C.SelectedIndex != 0)
                                ddl_dataArrivo_C.SelectedIndex = 0;
                            ddl_dataArrivo_C_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_ARRIVO_IL
                        #region DATA_ARRIVO_SUCCESSIVA_AL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_ARRIVO_SUCCESSIVA_AL.ToString())
                        {
                            if (ddl_dataArrivo_C.SelectedIndex != 1)
                                ddl_dataArrivo_C.SelectedIndex = 1;
                            ddl_dataArrivo_C_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_ARRIVO_SUCCESSIVA_AL
                        #region DATA_ARRIVO_PRECEDENTE_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_ARRIVO_PRECEDENTE_IL.ToString())
                        {
                            if (ddl_dataArrivo_C.SelectedIndex != 1)
                                ddl_dataArrivo_C.SelectedIndex = 1;
                            ddl_dataArrivo_C_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_ARRIVO_PRECEDENTE_IL
                        #region DATA_ARRIVO_SC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_ARRIVO_SC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataArrivo_C.SelectedIndex = 3;
                            this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                            this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataArrivo_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataArrivo_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataArrivo_C").btn_Cal.Enabled = false;
                            this.lbl_fineDataArrivo_C.Visible = true;
                            this.lbl_initDataArrivo_C.Visible = true;
                        }
                        #endregion
                        #region DATA_ARRIVO_MC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_ARRIVO_MC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataArrivo_C.SelectedIndex = 4;
                            this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                            this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataArrivo_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataArrivo_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataArrivo_C").btn_Cal.Enabled = false;
                            this.lbl_fineDataArrivo_C.Visible = true;
                            this.lbl_initDataArrivo_C.Visible = true;
                        }
                        #endregion
                        #region DATA_ARRIVO_TODAY
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_ARRIVO_TODAY.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataArrivo_C.SelectedIndex = 2;
                            this.GetCalendarControl("txt_initDataArrivo_C").Visible = true;
                            this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                            this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataArrivo_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataArrivo_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataArrivo_C").Visible = false;
                            this.GetCalendarControl("txt_fineDataArrivo_C").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_fineDataArrivo_C").btn_Cal.Visible = false;
                            this.lbl_fineDataArrivo_C.Visible = false;
                            this.lbl_initDataArrivo_C.Visible = false;
                        }
                        #endregion
                        #region PAROLE_CHIAVE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.PAROLE_CHIAVE.ToString())
                        {
                            DocumentoParolaChiave[] pk = DocumentManager.getListaParoleChiave(this);
                            bool found = false;
                            for (int i = 0; !found && i < pk.Length; i++)
                            {
                                if (pk[i].systemId == aux.valore)
                                {
                                    ListItem li = new ListItem(pk[i].descrizione, pk[i].systemId);
                                    this.ListParoleChiave.Items.Add(li);
                                    found = true;
                                }
                            }
                        }
                        #endregion PAROLE_CHIAVE
                        #region COMMISSIONE_REF
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.COMMISSIONE_REF.ToString())
                        {
                            txt_commRef.Text = aux.valore;
                        }
                        #endregion COMMISSIONE_REF
                        #region NOTE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NOTE.ToString())
                        {
                            string[] infoRic = Utils.splittaStringaRicercaNote(aux.valore);
                            rn_note.Testo = infoRic[0];
                            rn_note.TipoRicerca = (infoRic[1])[0];
                        }
                        #endregion NOTE
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
                        #region NUM_PROTO_EMERGENZA
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NUM_PROTO_EMERGENZA.ToString())
                        {
                            txt_protoEme.Text = aux.valore;
                        }
                        #endregion NUM_PROTO_EMERGENZA
                        #region DATA_PROTO_EMERGENZA_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_IL.ToString())
                        {
                            this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_PROTO_EMERGENZA_IL
                        #region DATA_PROTO_EMERGENZA_SUCCESSIVA_AL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_SUCCESSIVA_AL.ToString())
                        {
                            if (ddl_dataProtoEme.SelectedIndex != 1)
                                ddl_dataProtoEme.SelectedIndex = 1;
                            ddl_dataProtoEme_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_PROTO_EMERGENZA_SUCCESSIVA_AL
                        #region DATA_PROTO_EMERGENZA_PRECEDENTE_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_PRECEDENTE_IL.ToString())
                        {
                            if (ddl_dataProtoEme.SelectedIndex != 1)
                                ddl_dataProtoEme.SelectedIndex = 1;
                            ddl_dataProtoEme_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_ARRIVO_PRECEDENTE_IL
                        #region DATA_PROTO_EMERGENZA_SC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_SC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProtoEme.SelectedIndex = 3;
                            this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                            this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_dataProtoEmeInizio").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_dataProtoEmeFine").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_dataProtoEmeFine").btn_Cal.Enabled = false;
                            this.lbl_dataProtoEmeFine.Visible = true;
                            this.lbl_dataProtoEmeInizio.Visible = true;
                        }
                        #endregion
                        #region DATA_PROTO_EMERGENZA_MC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_MC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProtoEme.SelectedIndex = 4;
                            this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                            this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_dataProtoEmeInizio").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_dataProtoEmeFine").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_dataProtoEmeFine").btn_Cal.Enabled = false;
                            this.lbl_dataProtoEmeFine.Visible = true;
                            this.lbl_dataProtoEmeInizio.Visible = true;
                        }
                        #endregion
                        #region DATA_PROTO_EMERGENZA_TODAY
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_TODAY.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProtoEme.SelectedIndex = 2;
                            this.GetCalendarControl("txt_dataProtoEmeInizio").Visible = true;
                            this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                            this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_dataProtoEmeInizio").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_dataProtoEmeInizio").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_dataProtoEmeFine").Visible = false;
                            this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_dataProtoEmeFine").btn_Cal.Visible = false;
                            this.lbl_dataProtoEmeFine.Visible = false;
                            this.lbl_dataProtoEmeInizio.Visible = false;
                        }
                        #endregion
                        #region FIRMATARIO_NOME
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.FIRMATARIO_NOME.ToString())
                        {
                            txt_nomeFirma_C.Text = aux.valore;
                        }
                        #endregion FIRMATARIO_NOME
                        #region FIRMATARIO_COGNOME
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.FIRMATARIO_COGNOME.ToString())
                        {
                            txt_cognomeFirma_C.Text = aux.valore;
                        }
                        #endregion FIRMATARIO_COGNOME
                        #region NUM_OGGETTO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NUM_OGGETTO.ToString())
                        {
                            txt_numOggetto.Text = aux.valore;
                        }
                        #endregion NUM_OGGETTO
                        #region EVIDENZA
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.EVIDENZA.ToString())
                        {
                            rb_evidenza_C.SelectedValue = aux.valore;
                        }
                        #endregion EVIDENZA
                        #region CONDIZIONE_STATO_DOCUMENTO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.CONDIZIONE_STATO_DOCUMENTO.ToString())
                            this.ddlStateCondition.SelectedValue = aux.valore.ToString();
                        #endregion
                        #region DIAGRAMMA_STATO_DOC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DIAGRAMMA_STATO_DOC.ToString())
                        {
                            ddl_statiDoc.Visible = true;
                            ddl_statiDoc.SelectedIndex = 0;
                            string val = null;
                            try
                            {
                                string s = "DPA_DIAGRAMMI.ID_STATO";
                                s = aux.valore.Substring(aux.valore.LastIndexOf(s)).Trim();
                                s = s.Substring(0, s.Length - 1);
                                s = s.Replace(" ", "");
                                char[] sep = { '=' };
                                string[] tks = s.Split(sep);
                                val = tks[1];
                            }
                            catch { }

                            if (val != null)
                            {
                                bool found = false;
                                for (int i = 0; !found && i < ddl_statiDoc.Items.Count; i++)
                                {
                                    if (ddl_statiDoc.Items[i].Value == val)
                                    {
                                        ddl_statiDoc.SelectedIndex = i;
                                        found = true;
                                    }
                                }
                            }
                        }
                        #endregion DIAGRAMMA_STATO_DOC
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
                                        // txt_DescFascicolo.Text = fasc.descrizione;
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
                        #region MEZZO SPEDIZIONE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.MEZZO_SPEDIZIONE.ToString())
                        {
                            this.ddl_spedizione.SelectedValue = aux.valore;
                        }
                        #endregion
                        #region DATA_CREAZIONE_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString())
                        {
                            if (ddl_dataCreazione_E.SelectedIndex != 0)
                                ddl_dataCreazione_E.SelectedIndex = 0;
                            ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_CREAZIONE_IL
                        #region DATA_CREAZIONE_SUCCESSIVA_AL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString())
                        {
                            if (ddl_dataCreazione_E.SelectedIndex != 1)
                                ddl_dataCreazione_E.SelectedIndex = 1;
                            ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_CREAZIONE_SUCCESSIVA_AL
                        #region DATA_CREAZIONE_PRECEDENTE_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString())
                        {
                            if (ddl_dataCreazione_E.SelectedIndex != 1)
                                ddl_dataCreazione_E.SelectedIndex = 1;
                            ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
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
                            this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Enabled = false;
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
                            this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Enabled = false;
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
                            this.GetCalendarControl("txt_finedataCreazione_E").Visible = false;
                            this.GetCalendarControl("txt_finedataCreazione_E").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_finedataCreazione_E").btn_Cal.Visible = false;
                            this.lbl_dataCreazioneA.Visible = false;
                            this.lbl_dataCreazioneDa.Visible = false;
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
                        #region DATA_SCADENZA_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCADENZA_IL.ToString())
                        {
                            if (ddl_dataScadenza_C.SelectedIndex != 0)
                                ddl_dataScadenza_C.SelectedIndex = 0;
                            ddl_dataScadenza_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_SCADENZA_IL
                        #region DATA_SCADENZA_SUCCESSIVA_AL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCADENZA_SUCCESSIVA_AL.ToString())
                        {
                            if (ddl_dataScadenza_C.SelectedIndex != 1)
                                ddl_dataScadenza_C.SelectedIndex = 1;
                            ddl_dataScadenza_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_SCADENZA_SUCCESSIVA_AL
                        #region DATA_SCADENZA_PRECEDENTE_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCADENZA_PRECEDENTE_IL.ToString())
                        {
                            if (ddl_dataScadenza_C.SelectedIndex != 1)
                                ddl_dataScadenza_C.SelectedIndex = 1;
                            ddl_dataScadenza_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_SCADENZA_PRECEDENTE_IL
                        #region DATA_SCAD_SC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCAD_SC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataScadenza_C.SelectedIndex = 3;
                            this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                            this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataScadenza_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataScadenza_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataScadenza_C").btn_Cal.Enabled = false;
                            this.lbl_fineDataScadenza_C.Visible = true;
                            this.lbl_initDataScadenza_C.Visible = true;
                        }
                        #endregion
                        #region DATA_SCAD_MC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCAD_MC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataScadenza_C.SelectedIndex = 4;
                            this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                            this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataScadenza_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataScadenza_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataScadenza_C").btn_Cal.Enabled = false;
                            this.lbl_fineDataScadenza_C.Visible = true;
                            this.lbl_initDataScadenza_C.Visible = true;
                        }
                        #endregion
                        #region DATA_SCAD_TODAY
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCAD_TODAY.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataScadenza_C.SelectedIndex = 2;
                            this.GetCalendarControl("txt_initDataScadenza_C").Visible = true;
                            this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                            this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataScadenza_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataScadenza_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataScadenza_C").Visible = false;
                            this.GetCalendarControl("txt_fineDataScadenza_C").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_fineDataScadenza_C").btn_Cal.Visible = false;
                            this.lbl_fineDataScadenza_C.Visible = false;
                            this.lbl_initDataScadenza_C.Visible = false;
                        }
                        #endregion
                        #region Mancanza Immagine
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.MANCANZA_IMMAGINE.ToString())
                        {
                            if (aux.valore.Equals("1"))
                                this.cbl_docInCompl.Items.FindByValue("S_Img").Selected = true;
                            if (aux.valore.Equals("0"))
                                this.cbl_docInCompl.Items.FindByValue("C_Img").Selected = true;
                        }
                        #endregion
                        #region Mancanza Fascicolazione

                        else if (aux.argomento == DocsPaWR.FiltriDocumento.MANCANZA_FASCICOLAZIONE.ToString())
                        {
                            if (aux.valore.Equals("1"))
                                this.cbl_docInCompl.Items.FindByValue("S_Fasc").Selected = true;
                            if (aux.valore.Equals("0"))
                                this.cbl_docInCompl.Items.FindByValue("C_Fasc").Selected = true;
                        }
                        #endregion
                        #region CONSERVAZIONE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.CONSERVAZIONE.ToString())
                        {
                            if (aux.valore.Equals("1"))
                            {
                                this.cb_Conservato.Checked = true;
                                this.cb_NonConservato.Checked = false;
                            }
                            if (aux.valore.Equals("0"))
                            {
                                this.cb_NonConservato.Checked = true;
                                this.cb_Conservato.Checked = false;
                            }
                        }
                        #endregion
                        #region DEPOSITO
                        //else if (aux.argomento == DocsPaWR.FiltriDocumento.DEPOSITO.ToString())
                        //{
                        //    rbl_TrasfDep.SelectedValue = aux.valore;
                        //}
                        #endregion DEPOSITO
                        #region PROTOCOLLO TITOLARIO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ID_TITOLARIO.ToString())
                        {
                            if (!string.IsNullOrEmpty(aux.valore))
                                ddl_Titolario.SelectedValue = aux.valore;
                        }
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NUM_PROT_TITOLARIO.ToString())
                        {
                            if (!string.IsNullOrEmpty(aux.valore))
                                txt_ProtocolloTitolario.Text = aux.valore;
                        }
                        #endregion
                        #region CODICE_DESCRIZIONE_AMMINISTRAZIONE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.CODICE_DESCRIZIONE_AMMINISTRAZIONE.ToString())
                        {
                            if (!string.IsNullOrEmpty(aux.valore))
                                txt_codDesc.Text = aux.valore;
                        }
                        #endregion
                        #region NUMERO_VERSIONI
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NUMERO_VERSIONI.ToString())
                        {
                            if (!string.IsNullOrEmpty(aux.valore))
                            {
                                ddl_op_versioni.SelectedValue = aux.valore.Substring(0, 1);
                                txt_versioni.Text = aux.valore.Substring(1);
                            }
                        }
                        #endregion
                        #region NUMERO_ALLEGATI
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NUMERO_ALLEGATI.ToString())
                        {
                            if (!string.IsNullOrEmpty(aux.valore))
                            {
                                ddl_op_allegati.SelectedValue = aux.valore.Substring(0, 1);
                                txt_allegati.Text = aux.valore.Substring(1);
                            }
                        }
                        #endregion
                        #region RIFERIMENTO MITTENTE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.RIFERIMENTO_MITTENTE.ToString())
                        {
                            if (wws.isEnableRiferimentiMittente())
                            {
                                txt_rif_mittente.Text = aux.valore;
                            }

                        }
                        #endregion
                        #region ORDINAMENTO ASC/DESC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ORDER_DIRECTION.ToString() && !change_from_grid && !grid)
                        {
                            if (!string.IsNullOrEmpty(aux.valore))
                            {
                                this.ddlOrderDirection.SelectedValue = aux.valore;
                            }

                        }
                        #endregion

                        #region ORDINAMENTO TIPO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ORACLE_FIELD_FOR_ORDER.ToString() && !change_from_grid && !grid)
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

                        #region ORDINAMENTO CONTATORE SOLO SE TIPOLOGIA HA CONTATORE E NON SI HANNO LE GRIGLIE CUSTUM
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.CONTATORE_GRIGLIE_NO_CUSTOM.ToString())
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
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.VISIBILITA_T_A.ToString())
                        {
                            rbl_visibilita.SelectedValue = aux.valore;                            
                        }
                        #endregion Visibilità Tipica / Atipica
                    }
                    catch (Exception)
                    {
                        throw new Exception("I criteri di ricerca non sono piu\' validi.");
                    }
                }

                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
                    verificaCampiPersonalizzati();

                if (this.Session["itemUsedSearch"] != null)
                {
                    ddl_Ric_Salvate.SelectedIndex = Convert.ToInt32(this.Session["itemUsedSearch"]);
                }

                return true;
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
                return false;
            }
        }

        private void ddl_statiDoc_SelectedIndexChanged(object sender, System.EventArgs e)
        {

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

        private void caricaComboTipoFileAcquisiti()
        {
            ArrayList tipoFile = new ArrayList();
            tipoFile = DocumentManager.getExtFileAcquisiti(this);
            bool firmati = false;
            for (int i = 0; i < tipoFile.Count; i++)
            {
                //if (tipoFile[i].ToString().Contains("P7M"))
                //{
                //    if (!firmati)
                //    {
                //        ListItem item = new ListItem(tipoFile[i].ToString().Substring(tipoFile[i].ToString().Length - 3));
                //        this.ddl_tipoFileAcquisiti.Items.Add(item);
                //        firmati = true;
                //    }
                //}
                //else
                //{
                if (!tipoFile[i].ToString().Contains("P7M"))
                {
                    ListItem item = new ListItem(tipoFile[i].ToString());
                    this.ddl_tipoFileAcquisiti.Items.Add(item);
                }
                //}
            }
        }
        private void caricaComboTitolari(DropDownList ddl)
        {
            ddl.Items.Clear();
            ArrayList listaTitolari = new ArrayList(wws.getTitolariUtilizzabili(UserManager.getUtente(this).idAmministrazione));

            //Esistono dei titolari chiusi
            if (listaTitolari.Count != 0)
            {
                ddl.Items.Insert(0, string.Empty);
                //Creo le voci della ddl dei titolari
                string valueTutti = string.Empty;
                foreach (DocsPaWR.OrgTitolario titolario in listaTitolari)
                {
                    ListItem it = null;
                    switch (titolario.Stato)
                    {
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Attivo:
                            it = new ListItem(titolario.DescrizioneLite, titolario.ID);
                            ddl.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                            it = new ListItem(titolario.DescrizioneLite, titolario.ID);
                            ddl.Items.Add(it);
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
                    ddl.Items.Insert(1, it);
                }
            }
        }

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

        protected void ModifyRapidSearch_Click(object sender, EventArgs e)
        {
            if (RicercaCompleta())
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
            DocsPaWR.Utente utente = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
            DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];

            schedaRicerca = new SchedaRicerca(KEY_SCHEDA_RICERCA, utente, ruolo, this);
            Session[SchedaRicerca.SESSION_KEY] = schedaRicerca;

            schedaRicerca.Pagina = this;

            for (int i = 0; i < this.cbl_docInCompl.Items.Count; i++)
            {
                this.cbl_docInCompl.Items[i].Selected = false;
            }

            foreach (ListItem i in lb_reg_C.Items)
                i.Selected = true;
 
            ddl_numProt_C.SelectedIndex = 0;
            ddl_numProt_C_SelectedIndexChanged(null, new System.EventArgs());
            txt_initNumProt_C.Text = "";

            tbAnnoProt.Text = DateTime.Now.Year.ToString();
 
            ddl_dataProt_C.SelectedIndex = 0;
            ddl_dataProt_C_SelectedIndexChanged(null, new System.EventArgs());
            this.GetCalendarControl("txt_initDataProt_C").txt_Data.Text = "";
        
            rb_annulla_C.SelectedValue = "T";
    
            ddl_tipoDoc_C.SelectedIndex = 0;

            txt_segnatura.Text = "";

            txt_ogg_C.Text = "";
     
            txt_codMitt_C.Text = "";
            txt_descrMitt_C.Text = "";
 
            txt_codUffRef.Text = "";
            txt_descUffRef.Text = "";

            txt_codMittInter_C.Text = "";
            txt_descrMittInter_C.Text = "";
     
            txt_numProtMitt_C.Text = "";
   
            ddl_dataProtMitt_C.SelectedIndex = 0;
            ddl_dataProtMitt_C_SelectedIndexChanged(null, new System.EventArgs());
            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = "";
      
            ddl_dataArrivo_C.SelectedIndex = 0;
            ddl_dataArrivo_C_SelectedIndexChanged(null, new System.EventArgs());
            this.GetCalendarControl("txt_initDataArrivo_C").txt_Data.Text = "";
    
            ListParoleChiave.Items.Clear();
   
            txt_commRef.Text = "";
     
            rn_note.Testo = "";
     
            txt_protoEme.Text = "";

            this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text = "";
            this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Text = "";
  
            txt_nomeFirma_C.Text = "";
            txt_cognomeFirma_C.Text = "";

            txt_numOggetto.Text = "";

            ddl_idDocumento_C.SelectedIndex = 0;
            ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
            txt_initIdDoc_C.Text = "";

            ddl_dataCreazione_E.SelectedIndex = 0;
            ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
            this.GetCalendarControl("txt_initDataCreazione_E").txt_Data.Text = "";

            rb_evidenza_C.SelectedValue = "T";

            this.cbx_Trasm.Checked = false;
            this.cbx_TrasmSenza.Checked = false;

            txt_CodFascicolo.Text = "";
            txt_DescFascicolo.Text = "";
            FascicoliManager.setFascicoliSelezionati(this, null);

            this.ddl_spedizione.SelectedIndex = 0;

            ddl_dataScadenza_C.SelectedIndex = 0;
            ddl_dataScadenza_SelectedIndexChanged(null, new System.EventArgs());
            this.GetCalendarControl("txt_initDataScadenza_C").txt_Data.Text = "";

            //DATA STAMPA
            ddl_dataStampa_E.SelectedIndex = 0;
            this.GetCalendarControl("txt_initDataStampa_E").txt_Data.Text = "";
            this.GetCalendarControl("txt_finedataStampa_E").txt_Data.Text = "";
            this.GetCalendarControl("txt_finedataStampa_E").Visible = false;

            if (this.cbl_archDoc_C.Items.FindByValue("A") != null)
            {
                this.cbl_archDoc_C.Items.FindByValue("A").Selected = true;
            }
            if (this.cbl_archDoc_C.Items.FindByValue("P") != null)
            {
                this.cbl_archDoc_C.Items.FindByValue("P").Selected = true;
            }
            if (this.cbl_archDoc_C.Items.FindByValue("I") != null)
            {
                this.cbl_archDoc_C.Items.FindByValue("I").Selected = true;
            }
            if (this.cbl_archDoc_C.Items.FindByValue("G") != null)
            {
                this.cbl_archDoc_C.Items.FindByValue("G").Selected = true;
            }
            if (this.cbl_archDoc_C.Items.FindByValue("Pr") != null)
            {
                this.cbl_archDoc_C.Items.FindByValue("Pr").Selected = false;
            }
            if (this.cbl_archDoc_C.Items.FindByValue("ALL") != null)
            {
                this.cbl_archDoc_C.Items.FindByValue("ALL").Selected = false;
            }
            if (this.cbl_archDoc_C.Items.FindByValue("R") != null)
            {
                this.cbl_archDoc_C.Items.FindByValue("R").Selected = false;
            }

            //UserManager.removeCreatoreSelezionato(this.Page);
            //this.Creatore.Clear();

            if (pnl_cons.Visible)
            {
                cb_Conservato.Checked = false;
                cb_NonConservato.Checked = false;
            }

            if (pnl_trasfDesp.Visible)
            {
                rbl_TrasfDep.SelectedValue = "T";
            }

            txt_CodFascicolo.Text = "";
            txt_DescFascicolo.Text = "";
            FascicoliManager.removeFascicoliSelezionati();
            FascicoliManager.removeFascicoloSelezionatoFascRapida();

            this.ddl_ragioneTrasm.SelectedIndex = 0;

            if (pnl_protoEme.Visible)
            {
                ddl_dataProtoEme.SelectedIndex = 0;
                 this.GetCalendarControl("txt_dataProtoEmeInizio").txt_Data.Text = "";
                 this.GetCalendarControl("txt_dataProtoEmeFine").txt_Data.Text = "";
                 this.GetCalendarControl("txt_dataProtoEmeFine").Visible = false;          
            }

            ddl_tipoFileAcquisiti.SelectedIndex = 0;
            chkFirmato.Checked = false;
            chkNonFirmato.Checked = false;
            ddl_op_versioni.SelectedIndex = 0;
            txt_versioni.Text = string.Empty;
            this.ddl_Ric_Salvate.SelectedIndex = 0;

            if (btn_CampiPersonalizzati.Visible)
            {
                Session.Remove("TemplateRicerca");
                this.btn_CampiPersonalizzati.Visible = false;
            }

            if (this.aofAuthor.Visible)
                this.aofAuthor.DeleteFilters();

            if (this.aofOwner.Visible)
                this.aofOwner.DeleteFilters();

            GridManager.CompileDdlOrderAndSetOrderFilterDocuments(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);

            if (p_cod_amm.Visible)
            {
                this.txt_codDesc.Text = string.Empty;
            }

            this.txt_allegati.Text = "";
            this.ddl_op_allegati.SelectedIndex = 0;
        }
    }
}
