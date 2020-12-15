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
using System.Text;
using System.Collections.Generic;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;

namespace DocsPAWA.ricercaTrasm
{
    /// <summary>
    /// Summary description for ricTrasmCompleta.
    /// </summary>
    public class ricTrasmCompleta : DocsPAWA.CssPage
    {
        protected System.Web.UI.WebControls.Panel pnl_filtro;
        protected bool IfTODOLIST = false;
        protected System.Web.UI.WebControls.ImageButton btn_Canc_Ric;
        protected Utilities.MessageBox mb_ConfirmDelete;
        protected System.Web.UI.WebControls.DropDownList ddlOrder, ddlOrderDirection;

        #region variabili codice
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
        protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
        protected string queryStringPar_Verso = "";
        protected string queryStringPar_Tab = "";
        protected System.Web.UI.WebControls.Button btn_ricercaTrasm;
        protected System.Web.UI.WebControls.Button btn_salva;
        protected System.Web.UI.WebControls.CheckBox chk_DaCompletare;
        protected System.Web.UI.WebControls.DropDownList ddl_tipo_doc;
        protected System.Web.UI.WebControls.TextBox txt_codMitt_C;
        protected System.Web.UI.WebControls.TextBox txt_descrMitt_C;
        protected System.Web.UI.WebControls.TextBox txt_codMitt_U;
        protected System.Web.UI.WebControls.TextBox txt_descrMitt_U;
        protected System.Web.UI.WebControls.DropDownList ddl_dataTrasm;
        protected System.Web.UI.WebControls.Label lbl_initdataTrasm;
        protected DocsPAWA.UserControls.Calendar txt_initDataTrasm;
        protected System.Web.UI.WebControls.Label lbl_finedataTrasm;
        protected DocsPAWA.UserControls.Calendar txt_fineDataTrasm;
        protected System.Web.UI.WebControls.DropDownList ddl_ragioni;
        protected System.Web.UI.HtmlControls.HtmlTableCell Td2;
        protected DocsPAWA.DocsPaWR.RagioneTrasmissione[] listaRagioni;
        protected System.Web.UI.WebControls.CheckBox chk_visSott;
        protected System.Web.UI.WebControls.RadioButtonList rbl_tipo_corr;
        protected System.Web.UI.WebControls.TextBox txt_codCorr;
        protected System.Web.UI.WebControls.TextBox txt_descrCorr;
        protected System.Web.UI.WebControls.Label lbl_corr;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_systemIdCorr;
        protected System.Web.UI.WebControls.DropDownList ddl_oggetto;
        protected System.Web.UI.WebControls.Image btn_Rubrica_C;
        protected Hashtable m_hashTableRagioneTrasmissione;

        protected System.Web.UI.WebControls.CheckBox cbx_Acc;
        protected System.Web.UI.WebControls.CheckBox cbx_Rif;
        protected System.Web.UI.WebControls.CheckBox cbx_Pendenti;
        protected System.Web.UI.WebControls.DropDownList ddl_TAR;
        protected System.Web.UI.WebControls.Label lbl1_TAR;
        protected System.Web.UI.WebControls.Label lbl2_TAR;
        protected DocsPAWA.UserControls.Calendar dataUno_TAR;
        protected DocsPAWA.UserControls.Calendar dataDue_TAR;
        protected System.Web.UI.WebControls.Panel AccettateRifiutate;
        protected System.Web.UI.WebControls.Panel completamento;
        protected System.Web.UI.WebControls.Panel tipo_doc;
        //protected System.Web.UI.WebControls.RadioButtonList rbl_documentiInCompletamento;
        public DocsPAWA.ricercaDoc.SchedaRicerca schedaRicerca = null;
        protected System.Web.UI.WebControls.DropDownList ddl_Ric_Salvate;
        private bool isSavedSearch = false;
        private const string KEY_SCHEDA_RICERCA = "RicercaTrasmissioni";
        protected System.Web.UI.WebControls.CheckBox chk_mio_ruolo;
        protected System.Web.UI.WebControls.CheckBox chk_me_stesso;
        //protected System.Web.UI.WebControls.CheckBox chk_tutti_ruolo;
        protected System.Web.UI.WebControls.TextBox txt1_corr_sott;
        protected System.Web.UI.WebControls.TextBox txt2_corr_sott;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_systemIdCorrSott;
        protected System.Web.UI.WebControls.DropDownList select_sottoposto;
        protected System.Web.UI.WebControls.Image btn_img_sott_rubr;
      //  protected DocsPAWA.UserControls.Calendar cld_risposta_dal;
      //  protected DocsPAWA.UserControls.Calendar cld_risposta_al;
        protected DocsPAWA.UserControls.Calendar cld_scadenza_dal;
        protected DocsPAWA.UserControls.Calendar cld_scadenza_al;
        protected System.Web.UI.WebControls.TextBox txt_note_generali;
        protected System.Web.UI.WebControls.TextBox txt_note_individuali;
        protected System.Web.UI.WebControls.ListItem opArr;
        protected System.Web.UI.WebControls.ListItem opPart;
        protected System.Web.UI.WebControls.ListItem opInt;
        protected DocsPAWA.DocsPaWR.EtichettaInfo[] etichette;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoDoc_C;
        protected System.Web.UI.WebControls.Panel pnl_ric_fasc_prof;
        protected System.Web.UI.WebControls.ImageButton btn_CampiPersonalizzati;
        protected System.Web.UI.WebControls.Panel Panel_StatiDocumento;
        protected System.Web.UI.WebControls.DropDownList ddl_statiDoc;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoFasc;
        protected System.Web.UI.WebControls.Panel Panel_StatiFascicolo;
        protected System.Web.UI.WebControls.DropDownList ddl_statiFasc;
        protected System.Web.UI.WebControls.ImageButton img_dettagliProfilazione;
        protected System.Web.UI.WebControls.Label lbl_nome_utente;
        protected System.Web.UI.WebControls.Label lbl_tipo_ricerca;
        protected System.Web.UI.WebControls.Panel pnl_tipo_ricerca;
        protected System.Web.UI.WebControls.Label lbl_notifica;
        protected System.Web.UI.WebControls.CheckBox chk_firmati;
        protected System.Web.UI.WebControls.CheckBox chk_non_firmati;

        protected System.Web.UI.WebControls.CheckBox P_Prot;
        protected System.Web.UI.WebControls.CheckBox M_Fasc;
        protected System.Web.UI.WebControls.CheckBox M_si_img;
        protected System.Web.UI.WebControls.CheckBox M_Img;

        protected System.Web.UI.WebControls.Panel lbl_panel_con_imm;

        protected System.Web.UI.WebControls.DropDownList ddl_tipoFileAcquisiti;

        protected bool ruoloCheck;

        protected System.Web.UI.WebControls.CheckBox chkHistoricized, chkHistoricizedRole;
        protected Dictionary<string, Corrispondente> dic_Corr;

        #endregion

        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                // Put user code to initialize the page here
                Utils.startUp(this);
                //controlli aggiunti per il funzionamento dei radiobutton 

                if (Session["FromRicRap"] == null ||
                    !Session["FromRicRap"].ToString().Equals("1"))
                    // Caricamento della griglia standard per la ricerca documenti
                    GridManager.SelectedGrid = GridManager.GetStandardGridForUser(GridTypeEnumeration.Transmission);

                Session.Remove("FromRicRap");
                   
                if (!IsPostBack)
                {
                    // Compilazione delle drop down list con le informazioni sull'ordinamento
                    GridManager.SelectedGrid = GridManager.GetStandardGridForUser(GridTypeEnumeration.Transmission);
                    GridManager.CompileDdlOrderAndSetOrderFilterTransmission(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                }

                schedaRicerca = (DocsPAWA.ricercaDoc.SchedaRicerca)Session[DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY];
                if (schedaRicerca == null)
                {
                    //Inizializzazione della scheda di ricerca per la gestione delle 
                    //ricerche salvate
                    DocsPaWR.Utente utente = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
                    DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];

                    schedaRicerca = new DocsPAWA.ricercaDoc.SchedaRicerca(KEY_SCHEDA_RICERCA, utente, ruolo, this);
                    Session[DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY] = schedaRicerca;
                }
                schedaRicerca.Pagina = this;

                //this.chk_tutti_ruolo.Checked = chk_tutti_ruolo.Checked;

                if (!this.Page.IsPostBack)
                {
                    CaricaComboTipologiaAtto(ddl_tipoDoc_C);
                    CaricaComboTipologiaFasc();
                    caricaComboTipoFileAcquisiti();

                    ddl_tipo_doc.SelectedIndex = 1;

                    if (this.ddl_oggetto.SelectedValue.Equals("D"))
                    {
                        this.ddl_oggetto.SelectedValue = "D";
                        this.completamento.Visible = true;
                        this.tipo_doc.Visible = true;
                        this.ddl_tipoDoc_C.Visible = true;
                        this.ddl_tipoFasc.Visible = false;
                    }
                    else
                    {
                        this.pnl_ric_fasc_prof.Visible = true;
                        this.completamento.Visible = false;
                        this.tipo_doc.Visible = false;
                        this.ddl_tipoFasc.Visible = true;
                        this.ddl_tipoDoc_C.Visible = false;

                    }

                    this.chk_mio_ruolo.Checked = true;
                    this.chk_me_stesso.Checked = true;

                   // caricaComboFiltri();
                    caricaRagioni();
                    // carico la drop down list delle ricerche
                    schedaRicerca.ElencoRicerche("T", ddl_Ric_Salvate);
                    //if (Request.QueryString["oneTime"] == null)
                    //{

                    DocsPaWR.Utente utente = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
                    this.lbl_nome_utente.Text = "(" + utente.descrizione + ")";
                    DocsPaWR.Ruolo mioRuolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
                    setCorrispondenteMioRuolo(mioRuolo);

                    this.select_sottoposto.SelectedValue = utente.idPeople;

                    BindFilterValues(schedaRicerca);
                    //}
                    UserManager.removeCorrispondentiSelezionati(this);
                    string s = "<SCRIPT language='javascript'>document.getElementById('" + this.txt_codCorr.ID + "').focus()</SCRIPT>";
                    if (!this.IsStartupScriptRegistered("focus"))
                    {
                        if (Request.QueryString["tab"] == "toDoListAndAssPend" && Request.QueryString["verso"] == "R")
                        {
                            //Serve a non eseguire lo script lato client nel caso delle trasmissioni RICEVUTE - COSE DA FARE
                            RegisterStartupScript("focus", null);
                        }
                        else
                        {
                            //this.RegisterStartupScript("focus", s);
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);
                        }
                    }

                    //RUBRICA SOTTOPOSTO
                    string f = "<SCRIPT language='javascript'>document.getElementById('" + this.txt1_corr_sott.ID + "').focus()</SCRIPT>";
                    if (!this.IsStartupScriptRegistered("focus"))
                    {
                        if (Request.QueryString["tab"] == "toDoListAndAssPend" && Request.QueryString["verso"] == "R")
                        {
                            //Serve a non eseguire lo script lato client nel caso delle trasmissioni RICEVUTE - COSE DA FARE
                            RegisterStartupScript("focus", null);
                        }
                        else
                        {
                            //this.RegisterStartupScript("focus", s);
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", f);
                        }
                    }



                    //

               /*     if (Session["Tipo_obj"] == null)
                    {
                        if (this.OnSearchTrasmissioniFascioli())
                        {
                            Session["Tipo_obj"] = "F";
                            this.ddl_oggetto.SelectedValue = "F";
                            this.completamento.Visible = false;
                            this.tipo_doc.Visible = false;
                            //PROFILAZIONE DINAMICA FASCICOLI
                            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                            {
                                CaricaComboStatiFasc();
                                this.pnl_ric_fasc_prof.Visible = true;
                            }
                        }
                        else
                        {
                            Session["Tipo_obj"] = "D";
                            //this.ddl_fasc.SelectedIndex=-1;
                            this.ddl_oggetto.SelectedValue = "D";
                            this.ddl_tipo_doc.SelectedValue = "P";
                            this.completamento.Visible = true;
                            this.tipo_doc.Visible = true;
                            this.pnl_ric_fasc_prof.Visible = false;
                        }
                    }*/

                    // verifica esistenza di una ricerca già effettuata per il 
                    // popolamento del campo Documento in Completamento nel caso di
                    // back alla pagina di ricerca trasmissioni

                    abilitaRicercaProtInterno();

                }

                if (this.IsPostBack)
                {
                   if (this.ddl_oggetto.SelectedValue.Equals("F"))
                    {
                        this.completamento.Visible = false;
                        this.tipo_doc.Visible = false;
                        //PROFILAZIONE DINAMICA FASCICOLI
                        if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                        {
                            if (this.ddl_statiFasc.Visible == false || this.ddl_statiFasc.SelectedIndex == 0)
                            {
                                CaricaComboStatiFasc();
                            }
                            this.pnl_ric_fasc_prof.Visible = true;
                        }
                    }
                    else
                    {
                        this.pnl_ric_fasc_prof.Visible = false;
                        this.completamento.Visible = true;
                        this.tipo_doc.Visible = true;
                        this.ddl_tipo_doc.SelectedValue.Equals("P");
                        if (this.ddl_statiDoc.Visible == false || this.ddl_statiDoc.SelectedIndex == 0)
                        {
                            attivaProfilazioneDinamica();
                        }
                        //CaricaComboTipologiaAtto(this.ddl_tipoDoc_C);
                    }
                    
                    if (this.txt1_corr_sott != null && !this.txt1_corr_sott.Equals("") && this.txt2_corr_sott != null && !this.txt2_corr_sott.Equals("")  && select_sottoposto.Items.Count != 0)
                    {
                        //USATO PER COLORARE I CAMPI DELLA SELECT QUANDO RICARICA LA PAGINA
                        if (ViewState["colorItems"] != null)
                        {
                            List<int> color = ViewState["colorItems"] as List<int>;

                            foreach (int colorTemp in color)
                            {
                                this.select_sottoposto.Items[colorTemp].Attributes.Add("style", "color:#990000");
                            }

                        }
                    }

                }
                aggiornaPagina();
                string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
                if (use_new_rubrica != "1")
                {
                    this.btn_Rubrica_C.Attributes.Add("onclick", "ApriRubrica('ric_Trasm','" + this.rbl_tipo_corr.SelectedValue + "');");
                   
                    //MODIFICARE FUNZIONE JAVASCRIPT
                    this.btn_img_sott_rubr.Attributes.Add("onclick", "ApriRubrica('ric_Trasm','" + this.rbl_tipo_corr.SelectedValue + "');");
                }
                else
                {
                    btn_Rubrica_C.Attributes.Add("onclick", "_ApriRubricaRicercaTrasm('" + this.rbl_tipo_corr.SelectedValue + "');");
                    
                    //MODIFICARE FUNZIONE JAVASCRIPT
                    DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
                    this.btn_img_sott_rubr.Attributes.Add("onclick", "_ApriRubricaRicercaTrasmSottoposti();");
                }

                tastoInvio();
                impostaAccettateRifiutate();
                getLettereProtocolli();

                //PROFILAZIONE DINAMICA DOCUMENTI
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
                {
                    verificaCampiPersonalizzati();
                }
                else
                {
                    btn_CampiPersonalizzati.Visible = false;
                }
                //FINE PROFILAZIONE DINAMICA

                //PROFILAZIONE DINAMICA FASCICOLI
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                {
                    verificaCampiPersonalizzatiFasc();
                }
                else
                {
                    img_dettagliProfilazione.Visible = false;
                }
                //FINE PROFILAZIONE DINAMICA FASCICOLI
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

     /*   private void caricaComboFiltri()
        {
            string[] arrayFiltri = Enum.GetNames(typeof(DocsPAWA.DocsPaWR.FiltriTrasmissione));

            for (int i = 1; i <= 2; i++)
            {
                DropDownList DDLFiltro = (DropDownList)FindControl("DDLFiltro" + i.ToString());
                Utils.populateDdlWithEnumValuesANdKeys(DDLFiltro, arrayFiltri);
            }
        }*/

        private void caricaRagioni()
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
                    ddl_ragioni.Items.Add("");
                    for (int i = 0; i < listaRagioni.Length; i++)
                    {
                        m_hashTableRagioneTrasmissione.Add(i, listaRagioni[i]);

                        ListItem newItem = new ListItem(listaRagioni[i].descrizione, listaRagioni[i].systemId);
                        ddl_ragioni.Items.Add(newItem);
                    }
                    TrasmManager.setHashRagioneTrasmissione(this, m_hashTableRagioneTrasmissione);

                    this.ddl_ragioni.SelectedIndex = 0;
                }
            }
            else
            {
                m_hashTableRagioneTrasmissione = TrasmManager.getHashRagioneTrasmissione(this);
            }
        }


        private void aggiornaPagina()
        {
            if ((Request.QueryString["tab"] != null)
                && (!Request.QueryString["tab"].Equals("")))
            {
                queryStringPar_Tab = Request.QueryString["tab"].ToString();
            }

            if ((Request.QueryString["verso"] != null)
                && (!Request.QueryString["verso"].Equals("")))
            {
                queryStringPar_Verso = Request.QueryString["verso"].ToString();
            }

            visibleField(queryStringPar_Tab, queryStringPar_Verso);
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
            this.ddl_tipo_doc.SelectedIndexChanged += new System.EventHandler(this.ddl_tipo_doc_SelectedIndexChanged);
            //this.ddl_fasc.SelectedIndexChanged += new System.EventHandler(this.DDLOggettoTab1_SelectedIndexChanged);
            this.rbl_tipo_corr.SelectedIndexChanged += new System.EventHandler(this.rbl_tipo_corr_SelectedIndexChanged);
            this.txt_codCorr.TextChanged += new System.EventHandler(this.txt_codCorr_TextChanged);
            this.ddl_dataTrasm.SelectedIndexChanged += new System.EventHandler(this.ddl_dataTrasm_SelectedIndexChanged);
            this.ddl_ragioni.SelectedIndexChanged += new System.EventHandler(this.ddl_ragioni_SelectedIndexChanged);
          //  this.DDLFiltro1.SelectedIndexChanged += new System.EventHandler(this.DDLFiltro1_SelectedIndexChanged);
            this.btn_ricercaTrasm.Click += new System.EventHandler(this.btn_ricercaTrasm_Click);
            this.hd_systemIdCorr.ServerChange += new System.EventHandler(this.hd_systemIdCorr_ServerChange);
         //   this.rbl_documentiInCompletamento.SelectedIndexChanged += new System.EventHandler(this.rbl_documentiInCompletamento_SelectedIndexChanged);
            this.btn_salva.Click += new EventHandler(btn_salva_Click);
            this.ddl_TAR.SelectedIndexChanged += new System.EventHandler(this.ddl_TAR_SelectedIndexChanged);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.ricTrasmCompleta_PreRender);
            this.ddl_Ric_Salvate.SelectedIndexChanged += new EventHandler(ddl_Ric_Salvate_SelectedIndexChanged);
            this.btn_Canc_Ric.Click += new ImageClickEventHandler(btn_Canc_Ric_Click);
            this.mb_ConfirmDelete.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.mb_ConfirmDelete_GetMessageBoxResponse);
            this.txt1_corr_sott.TextChanged += new System.EventHandler(this.txt1_corr_sott_TextChanged);
            this.hd_systemIdCorrSott.ServerChange += new System.EventHandler(this.hd_systemIdCorrSott_ServerChange);
            this.select_sottoposto.SelectedIndexChanged += new System.EventHandler(this.select_sottoposto_ServerChange);
            this.btn_CampiPersonalizzati.Click += new System.Web.UI.ImageClickEventHandler(this.btn_CampiPersonalizzati_Click);
            this.ddl_statiDoc.SelectedIndexChanged += new System.EventHandler(this.ddl_statiDoc_SelectedIndexChanged);
            this.ddl_tipoDoc_C.SelectedIndexChanged += new System.EventHandler(this.ddl_tipoDoc_C_SelectedIndexChanged);
            this.ddl_oggetto.SelectedIndexChanged += new System.EventHandler(this.ddl_oggetto_SelectedIndexChanged);
            this.ddl_tipoFasc.SelectedIndexChanged += new EventHandler(this.ddl_tipoFasc_SelectedIndexChanged);
            this.img_dettagliProfilazione.Click += new ImageClickEventHandler(img_dettagliProfilazione_Click);
            this.ddl_statiFasc.SelectedIndexChanged += new System.EventHandler(this.ddl_statiFasc_SelectedIndexChanged);
            this.chk_mio_ruolo.CheckedChanged += new System.EventHandler(this.chk_mio_ruolo_SelectIndexChanged);
            this.chk_firmati.CheckedChanged += new System.EventHandler(this.chk_firmati_SelectIndexChanged);
            this.chk_non_firmati.CheckedChanged += new System.EventHandler(this.chk_non_firmati_SelectIndexChanged);
            this.P_Prot.CheckedChanged += new System.EventHandler(this.P_Prot_SelectIndexChanged);
            this.M_Fasc.CheckedChanged += new System.EventHandler(this.M_Fasc_SelectIndexChanged);
            this.M_si_img.CheckedChanged += new System.EventHandler(this.M_si_img_SelectIndexChanged);
            this.M_Img.CheckedChanged += new System.EventHandler(this.M_Img_SelectIndexChanged);
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

        private void mb_ConfirmDelete_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                try
                {
                    schedaRicerca.Cancella(Int32.Parse(ddl_Ric_Salvate.SelectedValue));
                    Session[DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY] = null;
                    this.ddl_Ric_Salvate.SelectedIndex = 0;
                    DocsPaWR.Utente utente = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
                    DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
                    schedaRicerca = new DocsPAWA.ricercaDoc.SchedaRicerca(KEY_SCHEDA_RICERCA, utente, ruolo, this);
                    Session[DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY] = schedaRicerca;

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

        private void ddl_Ric_Salvate_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (ddl_Ric_Salvate.SelectedIndex == 0)
                return;
            try
            {
                // Caricamento della griglia relativa alla ricerca salvata
               // GridManager.SelectedGrid = GridManager.GetGridFromSearchId(this.ddl_Ric_Salvate.SelectedValue, GridTypeEnumeration.Transmission);
                // Compilazione delle drop down list con le informazioni sull'ordinamento
                //GridManager.CompileDdlOrderAndSetOrderFilterTransmission(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                Session["FromRicRap"] = "1";
                string gridTempId = string.Empty;
                schedaRicerca.Seleziona(Int32.Parse(ddl_Ric_Salvate.SelectedValue), out gridTempId);
                Session["oneTimeRicTrasm"] = 1;
                try
                {
                    if (ddl_Ric_Salvate.SelectedIndex > 0)
                    {
                        Session.Add("itemUsedSearch", ddl_Ric_Salvate.SelectedIndex.ToString());
                    }
                    BindFilterValues(schedaRicerca);
                    string docIndexQueryString = string.Empty;
                    if (!this.IsPostBack &&
                        this.Request.QueryString["back"] != null &&
                        this.Request.QueryString["docIndex"] != null &&
                        this.Request.QueryString["docIndex"] != string.Empty)
                        docIndexQueryString = "&docIndex=" + this.Request.QueryString["docIndex"];

                    string url = "tabRisultatiRicTrasm.aspx?";
                    if (this.Request.QueryString["tab"] != null)
                        url += "tab=" + this.Request.QueryString["tab"] + "&";

                    if (queryStringPar_Verso.Equals("E") == true)
                        url += "tiporic=E";
                    else
                        url += "tiporic=R";

                    DocumentManager.setFiltroRicTrasm(this, schedaRicerca.FiltriRicerca[0]);

                    Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='" + url + "&home=N" + docIndexQueryString + "';</script>");
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

        void btn_salva_Click(object sender, EventArgs e)
        {
            if (ricercaTrasmissioni())
            {
                // Impostazione del filtro utilizzato
                GridManager.SetSearchFilter(this.ddlOrder.SelectedItem.Text, this.ddlOrderDirection.SelectedValue);

                if (this.chk_me_stesso.Checked == false && this.chk_mio_ruolo.Checked == false)
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_check", "alert('Selezionare almeno un destinatario della trasmissione tra utente, ruolo o sottoposti');", true);
                }
                else
                {
                    schedaRicerca.FiltriRicerca = qV;
                    schedaRicerca.ProprietaNuovaRicerca = new DocsPAWA.ricercaDoc.SchedaRicerca.NuovaRicerca();
                    RegisterStartupScript("SalvaRicerca", "<script>apriSalvaRicerca();</script>");
                }
            }
        }
        #endregion

     /*   private void rbl_documentiInCompletamento_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.rbl_documentiInCompletamento.SelectedValue.Equals("M_si_img"))
            {
                this.chk_firmati.Visible = true;
                this.chk_non_firmati.Visible = true;
            }
            else
            {
                this.chk_firmati.Visible = false;
                this.chk_non_firmati.Visible = false;
            }
        }
        */
        private void visibleField(string tipoTab, string versoTrasm)
        {
            ///<summary>
            ///questa funzione a seconda del tab selezionato e del tipo trasmissione ricercato
            ///rende visibili i campi dei tab
            ///<param name=tipoTab> tipo di tab selezionato
            ///</param>
            ///<param name=tipoTab> tipo di trasmissione ricercata
            ///</param>
            ///</summary>
            ///

            if (versoTrasm.Equals("E") == true)
            {
                //effettuate
                if (tipoTab.Equals("completa"))
                {	//EFF:Completa
                    this.pnl_filtro.Visible = true;
                    this.chk_visSott.Visible = true;  //nuovo
                    //this.chk_DaCompletare.Visible=false;
                }
                else
                {
                    //Eff:Assegna. 
                    this.pnl_filtro.Visible = true;
                    this.chk_visSott.Visible = true;  //nuovo
                    //this.chk_DaCompletare.Visible=mostraDaCompletare();
                }
                this.lbl_corr.Text = "Destinatario";
               // this.chk_tutti_ruolo.Visible = true;
                this.lbl_tipo_ricerca.Text = "Mittente trasmissione:";
                this.chk_me_stesso.Visible = false;
                this.pnl_tipo_ricerca.Visible = false;
                this.lbl_notifica.Text = "Utente mittente";
            }
            else
            {
                //Ricevute
                //this.chk_DaCompletare.Visible=false;

                Session.Add("ProvenienzaHome", "ricTrasm");

                if (tipoTab.Equals("completa"))
                {
                    //RICEVUTE:Completa
                    this.pnl_filtro.Visible = true;
                    this.chk_visSott.Visible = true;
                }
                else
                {
                    //Ricevute:TO DO LIST
                    IfTODOLIST = true;
                    //string provenienza = Request.QueryString["home"];
                    //if (provenienza.Equals("Y"))
                    //{
                    //	Session.Add("ProvenienzaHome","ricTrasm");
                    //}
                    this.pnl_filtro.Visible = false;
                    this.chk_visSott.Visible = false;
                    this.completamento.Visible = false;
                }
                this.lbl_corr.Text = "Mittente";
               // this.chk_mio_ruolo.Visible = true;
                this.chk_me_stesso.Visible = true;
                this.lbl_tipo_ricerca.Text = "Destinatario trasmissione:";
                this.pnl_tipo_ricerca.Visible = true;
                this.lbl_notifica.Text = "Notificata a";
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


        private string getArgomento(bool codRubrica, string tipoCorr, string tipoTrasm)
        {
            string argomento = "";
            switch (tipoCorr)
            {
                case "R":
                    if (codRubrica)
                    {
                        if (tipoTrasm.Equals("R"))
                            argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_MITT_RUOLO.ToString();
                        else
                            argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_DEST_RUOLO.ToString();
                    }
                    else
                    {
                        if (tipoTrasm.Equals("R"))
                            argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_RUOLO.ToString();
                        else
                            argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DESTINATARIO_RUOLO.ToString();
                    }
                    break;
                case "P":
                    if (codRubrica)
                    {
                        if (tipoTrasm.Equals("R"))
                            argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_MITT_UTENTE.ToString();
                        else
                            argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_DEST_UTENTE.ToString();
                    }
                    else
                    {
                        if (tipoTrasm.Equals("R"))
                            argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_UTENTE.ToString();
                        else
                            argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DESTINATARIO_UTENTE.ToString();
                    }
                    break;
                case "U":
                    if (codRubrica)
                    {
                        if (tipoTrasm.Equals("R"))
                            argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.ID_UO_MITT.ToString();
                        else
                            argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.ID_UO_DEST.ToString();
                    }
                    else
                    {
                        if (tipoTrasm.Equals("R"))
                            argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_UO.ToString();
                        else
                            argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DESTINATARIO_UO.ToString();
                    }
                    break;
            }
            return argomento;
        }


        private bool ricercaTrasmissioni()
        {
            bool res = true;
            try
            {
                //array contenitore degli array filtro di ricerca
                qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];

                fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

                #region //filtri su mancanza immagine, fascicolazione e da protocollare
                if (this.P_Prot.Checked || this.M_Fasc.Checked || this.M_si_img.Checked || this.M_Img.Checked)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    if (this.M_Img.Checked)
                    {
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.MANCANZA_IMMAGINE.ToString();
                        fV1.valore = "M_Img";
                    }
                    else
                    {
                        if (this.M_Fasc.Checked)
                        {
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.MANCANZA_FASCICOLAZIONE.ToString();
                            fV1.valore = "M_Fasc";
                        }
                        else
                        {
                            if (this.P_Prot.Checked)
                            {
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DA_PROTOCOLLARE.ToString();
                                fV1.valore = "P_Prot";
                            }
                            else
                            {
                                if (this.M_si_img.Checked)
                                {
                                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.CON_IMMAGINE.ToString();
                                    fV1.valore = "M_si_img";
                                }
                            }
                        }
                    }

                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    if (fV1.valore.Equals("M_si_img"))
                    {
                        if (this.chk_firmati.Checked)
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.FIRMATO.ToString();
                            fV1.valore = "1";
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        if (this.chk_non_firmati.Checked)
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.FIRMATO.ToString();
                            fV1.valore = "0";
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }

                        if (this.ddl_tipoFileAcquisiti.SelectedIndex > 0)
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TIPO_FILE_ACQUISITO.ToString();
                            fV1.valore = this.ddl_tipoFileAcquisiti.SelectedItem.Value;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }

                    }
                }
                /*if (this.rbl_documentiInCompletamento.SelectedIndex >= 0)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    if (this.rbl_documentiInCompletamento.SelectedItem.Value.Equals("M_Img"))
                    {
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.MANCANZA_IMMAGINE.ToString();
                        fV1.valore = "M_Img";
                    }
                    else if (this.rbl_documentiInCompletamento.SelectedItem.Value.Equals("M_Fasc"))
                    {
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.MANCANZA_FASCICOLAZIONE.ToString();
                        fV1.valore = "M_Fasc";
                    }
                    else if (this.rbl_documentiInCompletamento.SelectedItem.Value.Equals("P_Prot"))
                    {
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DA_PROTOCOLLARE.ToString();
                        fV1.valore = "P_Prot";

                    }
                    else if (this.rbl_documentiInCompletamento.SelectedItem.Value.Equals("M_si_img"))
                    {
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.CON_IMMAGINE.ToString();
                        fV1.valore = "M_si_img";

                    }
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    if (fV1.valore.Equals("M_si_img"))
                    {
                        if (this.chk_firmati.Checked)
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.FIRMATO.ToString();
                            fV1.valore = "1";
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        if (this.chk_non_firmati.Checked)
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.FIRMATO.ToString();
                            fV1.valore = "0";
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }

                    }
                }
                */
                #endregion

                #region filtro "TODOLIST"
                if (this.IfTODOLIST)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TODO_LIST.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region  filtro sul mittente/destinatario
                if (!this.txt_codCorr.Text.Equals("") || !this.txt_descrCorr.Text.Equals(""))
                {
                    if (!this.txt_codCorr.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = getArgomento(true, this.rbl_tipo_corr.SelectedValue, queryStringPar_Verso);
                        if (!this.rbl_tipo_corr.SelectedValue.Equals("U"))
                            fV1.valore = this.txt_codCorr.Text;
                        else
                            fV1.valore = this.hd_systemIdCorr.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    else
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = getArgomento(false, this.rbl_tipo_corr.SelectedValue, queryStringPar_Verso);
                        fV1.valore = this.txt_descrCorr.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    // Aggiunta filtro per estensione ricerca a storicizzati
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = FiltriTrasmissioneNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString();
                    fV1.valore = this.chkHistoricized.Checked.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region  filtro sulla ragione
                if (ddl_ragioni.SelectedIndex > 0)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.RAGIONE.ToString();
                    fV1.valore = ddl_ragioni.SelectedItem.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro sulla data trasmissione
                if (this.ddl_dataTrasm.SelectedIndex == 2)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataTrasm.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataTrasm.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataTrasm.SelectedIndex == 0)
                {//valore singolo carico DATA_CREAZIONE
                    if (!this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                if (this.ddl_dataTrasm.SelectedIndex == 1)
                {//valore singolo carico DATA_CREAZIONE
                    if (!this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region FILTRO SU RICEVUTE/EFFETTUATE
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.RICEVUTE_EFFETTUATE.ToString();
                fV1.valore = Request.QueryString["verso"];
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion

                #region filtro Assegnazioni Pendenti
                if (queryStringPar_Verso.Equals("E") && !queryStringPar_Tab.Equals("completa"))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    if (this.chk_DaCompletare.Checked)
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.ATTIVITA_NON_CONCLUSE.ToString();
                    else
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.ASSEGNAZIONI_PENDENTI.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro sulla natura dei documenti
                if (this.ddl_tipo_doc.SelectedIndex > -1)
                {
                    if (!ddl_oggetto.SelectedValue.ToString().Equals("F"))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        switch (ddl_tipo_doc.SelectedValue.ToString())
                        {
                            case "Tutti":
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_TUTTI.ToString();
                                break;
                            case "P":
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROTOCOLLATI.ToString();
                                break;
                            case "PA":
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROT_ARRIVO.ToString();
                                break;
                            case "PP":
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROT_PARTENZA.ToString();
                                break;
                            case "NP":
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_NON_PROTOCOLLATI.ToString();
                                break;
                            case "PI":
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROT_INTERNO.ToString();
                                break;
                        }
                        fV1.valore = "true";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro "visualizza sottoposti"
                if (this.chk_visSott.Visible)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.NO_CERCA_INFERIORI.ToString();
                    if (this.chk_visSott.Checked)
                        fV1.valore = "false";
                    else
                        fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro "oggetto trasmesso"
                string tipo_oggetto = this.ddl_oggetto.SelectedValue;
                //if (this.ddl_fasc.SelectedIndex >= 0)
                //	tipo_oggetto = "F";
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString();
                fV1.valore = tipo_oggetto;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion

                //FILTRI MODIFICATI

                #region Note generali
                if (txt_note_generali.Text != "")
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = "NOTE_GENERALI";
                    fV1.valore = this.txt_note_generali.Text.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion 

                #region Note individuali
                if (txt_note_individuali.Text != "")
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = "NOTE_INDIVIDUALI";
                    fV1.valore = this.txt_note_individuali.Text.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion 

             /*   #region Data risposta precedente e successiva
                if (!this.GetCalendarControl("cld_risposta_dal").txt_Data.Text.Equals("") && !this.GetCalendarControl("cld_risposta_al").txt_Data.Text.Equals(""))
                {
                    if (!Utils.isDate(this.GetCalendarControl("cld_risposta_dal").txt_Data.Text) || !Utils.isDate(this.GetCalendarControl("cld_risposta_al").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return false;
                    }
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = "RISPOSTA_SUCCESSIVA_AL";
                    fV1.valore = this.GetCalendarControl("cld_risposta_dal").txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = "RISPOSTA_PRECEDENTE_IL";
                    fV1.valore = this.GetCalendarControl("cld_risposta_al").txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                */
              /*  #region Data risposta singola
                if (!this.GetCalendarControl("cld_risposta_dal").txt_Data.Text.Equals("") && this.GetCalendarControl("cld_risposta_al").txt_Data.Text.Equals(""))
                {
                    if (!Utils.isDate(this.GetCalendarControl("cld_risposta_dal").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return false;
                    }
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = "RISPOSTA_IL";
                    fV1.valore = this.GetCalendarControl("cld_risposta_dal").txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                */
                #region Data scadenza precedente e successiva
                if (!this.GetCalendarControl("cld_scadenza_dal").txt_Data.Text.Equals("") && !this.GetCalendarControl("cld_scadenza_al").txt_Data.Text.Equals(""))
                {
                    if (!Utils.isDate(this.GetCalendarControl("cld_scadenza_dal").txt_Data.Text) || !Utils.isDate(this.GetCalendarControl("cld_scadenza_al").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return false;
                    }
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = "SCADENZA_SUCCESSIVA_AL";
                    fV1.valore = this.GetCalendarControl("cld_scadenza_dal").txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = "SCADENZA_PRECEDENTE_IL";
                    fV1.valore = this.GetCalendarControl("cld_scadenza_al").txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region Data scadenza singola
                if (!this.GetCalendarControl("cld_scadenza_dal").txt_Data.Text.Equals("") && this.GetCalendarControl("cld_scadenza_al").txt_Data.Text.Equals(""))
                {
                    if (!Utils.isDate(this.GetCalendarControl("cld_scadenza_dal").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return false;
                    }
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = "SCADENZA_IL";
                    fV1.valore = this.GetCalendarControl("cld_scadenza_dal").txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region "Filtro TAR" - Tutte Accettate Rifiutate Pendenti
                string tipoRicerca = string.Empty;
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.ACCETTATA_RIFIUTATA.ToString();

                if (cbx_Acc.Checked && !cbx_Rif.Checked)
                {
                    fV1.valore = "A";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    tipoRicerca = "ACC";
                }

                if (!cbx_Acc.Checked && cbx_Rif.Checked)
                {
                    fV1.valore = "R";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    tipoRicerca = "RIF";
                }

                if (cbx_Acc.Checked && cbx_Rif.Checked)
                {
                    fV1.valore = "A_R";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    tipoRicerca = "ACC_RIF";
                }

                if (cbx_Pendenti.Checked)
                {
                    fV1.valore = "P";
                    fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.PENDENTI.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                switch (ddl_TAR.SelectedItem.Text)
                {
                    case "Valore Singolo":
                        if (this.GetCalendarControl("dataUno_TAR").txt_Data.Text != "")
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            if (tipoRicerca == "ACC")
                                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE.ToString();
                            if (tipoRicerca == "RIF")
                                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO.ToString();
                            if (tipoRicerca == "ACC_RIF")
                                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF.ToString();
                            fV1.valore = this.GetCalendarControl("dataUno_TAR").txt_Data.Text.ToString();
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            res = checkData(fV1.argomento, fV1.valore);
                        }
                        break;
                    case "Intervallo":
                        if (this.GetCalendarControl("dataUno_TAR").txt_Data.Text != "" && this.GetCalendarControl("dataDue_TAR").txt_Data.Text != "")
                        {
                            if (Convert.ToDateTime(this.GetCalendarControl("dataUno_TAR").txt_Data.Text) >= Convert.ToDateTime(this.GetCalendarControl("dataDue_TAR").txt_Data.Text))
                            {
                                res = false;
                                return res;
                            }

                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            if (tipoRicerca == "ACC")
                                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_DA.ToString();
                            if (tipoRicerca == "RIF")
                                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_DA.ToString();
                            if (tipoRicerca == "ACC_RIF")
                                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_DA.ToString();
                            fV1.valore = this.GetCalendarControl("dataUno_TAR").txt_Data.Text.ToString();
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            res = checkData(fV1.argomento, fV1.valore);
                            if (!res)
                                return res;

                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            if (tipoRicerca == "ACC")
                                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_A.ToString();
                            if (tipoRicerca == "RIF")
                                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_A.ToString();
                            if (tipoRicerca == "ACC_RIF")
                                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_A.ToString();
                            fV1.valore = this.GetCalendarControl("dataDue_TAR").txt_Data.Text.ToString();
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            res = checkData(fV1.argomento, fV1.valore);
                        }

                        if (this.GetCalendarControl("dataUno_TAR").txt_Data.Text != "" && this.GetCalendarControl("dataDue_TAR").txt_Data.Text == "")
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            if (tipoRicerca == "ACC")
                                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_DA.ToString();
                            if (tipoRicerca == "RIF")
                                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_DA.ToString();
                            if (tipoRicerca == "ACC_RIF")
                                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_DA.ToString();
                            fV1.valore = this.GetCalendarControl("dataUno_TAR").txt_Data.Text.ToString();
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            res = checkData(fV1.argomento, fV1.valore);
                        }

                        if (this.GetCalendarControl("dataUno_TAR").txt_Data.Text == "" && this.GetCalendarControl("dataDue_TAR").txt_Data.Text != "")
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            if (tipoRicerca == "ACC")
                                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_A.ToString();
                            if (tipoRicerca == "RIF")
                                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_A.ToString();
                            if (tipoRicerca == "ACC_RIF")
                                fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_A.ToString();
                            fV1.valore = this.GetCalendarControl("dataDue_TAR").txt_Data.Text.ToString();
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            res = checkData(fV1.argomento, fV1.valore);
                        }
                        break;

                    case "Oggi":
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        if (tipoRicerca == "ACC")
                            fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_TODAY.ToString();
                        if (tipoRicerca == "RIF")
                            fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_TODAY.ToString();
                        if (tipoRicerca == "ACC_RIF")
                            fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_TODAY.ToString();
                        fV1.valore = "1";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        res = true;
                        break;

                    case "Settimana Corr.":
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        if (tipoRicerca == "ACC")
                            fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_SC.ToString();
                        if (tipoRicerca == "RIF")
                            fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_SC.ToString();
                        if (tipoRicerca == "ACC_RIF")
                            fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_SC.ToString();
                        fV1.valore = "1";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        res = true;
                        break;

                    case "Mese Corrente":
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        if (tipoRicerca == "ACC")
                            fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_MC.ToString();
                        if (tipoRicerca == "RIF")
                            fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_MC.ToString();
                        if (tipoRicerca == "ACC_RIF")
                            fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_MC.ToString();
                        fV1.valore = "1";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        res = true;
                        break;
                }
                #endregion "Filtro TAR" - Tutte Accettate Rifiutate Pendenti

                #region FILTRO SU AL MIO RUOLO
                if (this.chk_mio_ruolo.Checked == true)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.AL_MIO_RUOLO.ToString();

                    fV1.valore = "true";

                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region FILTRO SU A ME STESSO
                if (this.chk_me_stesso.Checked == true)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.A_ME_STESSO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

             /*   #region FILTRO SU EFFETTUATE DA TUTTI I RUOLI
                if (this.chk_tutti_ruolo.Checked == true)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DAL_MIO_RUOLO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                */
                #region FILTRO SOTTOPOSTO RUOLO RUBRICA
                if (!this.txt1_corr_sott.Text.Equals("") && !this.txt2_corr_sott.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.RUOLO_SOTTOPOSTO.ToString();
                    fV1.valore = this.hd_systemIdCorrSott.Value;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    // Aggiunta filtro per estensione ricerca a storicizzati
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = FiltriTrasmissioneNascosti.RUOLO_EXTEND_TO_HISTORICIZED.ToString();
                    fV1.valore = this.chkHistoricizedRole.Checked.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    
                }
                #endregion

                #region FILTRO SOTTOPOSTO PERSONA RUBRICA
                if (!this.select_sottoposto.SelectedValue.Equals(string.Empty))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.PERSONA_SOTTOPOSTA.ToString();
                    fV1.valore = this.select_sottoposto.SelectedValue;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region FILTRO TIPOLOGIA DOCUMENTO
                if (this.ddl_tipoDoc_C.SelectedIndex > 0)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TIPO_ATTO.ToString();
                    fV1.valore = this.ddl_tipoDoc_C.SelectedItem.Value;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                #endregion

                #region FILTRO DIAGRAMMA A STATI
                if (ddl_statiDoc.Visible && ddl_statiDoc.SelectedIndex != 0)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DIAGRAMMA_STATO_DOC.ToString();
                    string cond = " AND (DPA_DIAGRAMMI.DOC_NUMBER = A.DOCNUMBER AND DPA_DIAGRAMMI.ID_STATO = " + ddl_statiDoc.SelectedValue + ") ";
                    fV1.valore = cond;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region FILTRO PROFILAZIONE_DINAMICA
                fV1 = (DocsPAWA.DocsPaWR.FiltroRicerca)schedaRicerca.GetFiltro(DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.PROFILAZIONE_DINAMICA.ToString());
                if (fV1 != null)
                {
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion filtro PROFILAZIONE_DINAMICA

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
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TIPOLOGIA_FASCICOLO.ToString();
                        fV1.valore = this.ddl_tipoFasc.SelectedItem.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro DIAGRAMMI DI STATO FASCICOLI
                if (ddl_statiFasc.Visible && ddl_statiFasc.SelectedIndex != 0)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.DIAGRAMMA_STATO_FASC.ToString();
                    fV1.valore = ddl_statiFasc.SelectedValue;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion filtro DIAGRAMMI DI STATO FASCICOLI

                #region Ordinamento

                // Reperimento del filtro da utilizzare per la griglia
                List<FiltroRicerca> filterList = GridManager.GetOrderFilterForTransmission(this.ddlOrder.SelectedValue, this.ddlOrderDirection.SelectedValue);

                // Se la lista è valorizzata vengono aggiunti i filtri
                if (filterList != null)
                    foreach (FiltroRicerca filter in filterList)
                        fVList = Utils.addToArrayFiltroRicerca(fVList, filter);

                #endregion

                qV[0] = fVList;
                return res;

            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
                res = false;
                return res;
            }
        }

        private bool mostraDaCompletare()
        {
            DocsPaWR.RagioneTrasmissione[] ragioni = TrasmManager.getListaRagioni(this, null, false);
            for (int i = 0; i < ragioni.Length; i++)
                if (ragioni[i].tipoRisposta != null && ragioni[i].tipoRisposta.Equals("C"))
                    return true;
            return false;
        }

        private void PerformSearch()
        {
            this.PerformSearch(null);
        }

        private void PerformSearch(DocsPAWA.DocsPaWR.FiltroRicerca[] filters)
        {
            try
            {
                if (filters == null)
                {
                    if (!ricercaTrasmissioni())
                    {
                        Response.Write("<script>alert('Errore nel formato della data');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataTrasm").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.iFrame_dx.document.location='../blank_page.htm';</script>");
                        return;
                    }
                    else
                    {
                        if (!this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text.Equals("") && !this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text.Equals(""))
                        {
                            if (Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text, this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text))
                            {
                                Response.Write("<script>alert('Verificare valori inseriti per Data Trasmissione!');</script>");
                                Response.Write("<script>top.principale.iFrame_dx.document.location='tabRisultatiRicTrasm.aspx?tiporic=R&home=N';</script>");
                                return;
                            }
                        }
                        filters = qV[0];
                    }
                }

                if (this.qV == null)
                {
                    this.qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                    this.qV[0] = filters;
                }

                // Salvataggio filtri di ricerca trasmissioni
                this.SaveSearchFilters(this.qV);

                // Impostazione filtro nello stato del contesto di chiamata
                this.SetFiltersOnCurrentContext(filters);

                DocumentManager.setFiltroRicTrasm(this, filters);

                TrasmManager.removeDataTableEff(this);
                TrasmManager.removeDataTableRic(this);

                //pulire session ric/eff query precedente:
                TrasmManager.removeMemoriaFiltriRicTrasm(this);
                TrasmManager.removeDocTrasmQueryEff(this);
                TrasmManager.removeDocTrasmQueryRic(this);

                string docIndexQueryString = string.Empty;
                if (!this.IsPostBack &&
                    this.Request.QueryString["back"] != null &&
                    this.Request.QueryString["docIndex"] != null &&
                    this.Request.QueryString["docIndex"] != string.Empty)
                    docIndexQueryString = "&docIndex=" + this.Request.QueryString["docIndex"];

                string url = "tabRisultatiRicTrasm.aspx?";
                if (this.Request.QueryString["tab"] != null)
                    url += "tab=" + this.Request.QueryString["tab"] + "&";

                if (queryStringPar_Verso.Equals("E") == true)
                    url += "tiporic=E";
                else
                    url += "tiporic=R";

                Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='" + url + "&home=N" + docIndexQueryString + "';</script>");

                TrasmManager.setMemoriaVisualizzaBack(this);
                TrasmManager.setMemoriaTab(this, queryStringPar_Verso);
                TrasmManager.setMemoriaFiltriRicTrasm(this, filters);
            }
            catch (System.Exception es)
            {
                //System.Diagnostics.Debug.WriteLine("error Login"+es.Message.ToString());
                ErrorManager.redirect(this, es);
            }
        }

        /// <summary>
        /// Verifica se si è selezionata la ricerca
        /// delle trasmissioni dei fascicoli
        /// </summary>
        /// <returns></returns>
        private bool OnSearchTrasmissioniFascioli()
        {
            bool retValue = false;

            DocsPaWR.FiltroRicerca[] filters = TrasmManager.getMemoriaFiltriRicTrasm(this);

            if (filters != null)
            {
                foreach (DocsPAWA.DocsPaWR.FiltroRicerca item in filters)
                {
                    retValue = (item.argomento.Equals(DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString()) &&
                              item.valore.ToUpper().Equals("F"));
                    if (retValue)
                        break;
                }
            }
            else
            {
                retValue = this.ddl_oggetto.SelectedValue.Equals("F");
            }

            return retValue;
        }

        private void btn_ricercaTrasm_Click(object sender, System.EventArgs e)
        {
            if ((this.chk_me_stesso.Checked == false && this.chk_mio_ruolo.Checked == false && this.chk_visSott.Checked == false))
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_check", "alert('Selezionare almeno un destinatario della trasmissione tra utente, ruolo o sottoposti');", true);
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Ritorna bianca", "top.principale.iFrame_dx.document.location='tabRisultatiRicTrasm.aspx?tiporic=R&home=N';", true);
            }
            else
            {
                if (this.chk_mio_ruolo.Checked == true && (string.IsNullOrEmpty(this.txt1_corr_sott.Text) || string.IsNullOrEmpty(this.txt2_corr_sott.Text)))
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_check", "alert('Selezionare almeno un ruolo');", true);
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Ritorna bianca", "top.principale.iFrame_dx.document.location='tabRisultatiRicTrasm.aspx?tiporic=R&home=N';", true);
                }
                else
                {
                    if (this.chk_mio_ruolo.Checked == true && string.IsNullOrEmpty(select_sottoposto.SelectedValue))
                    {
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_check", "alert('Il ruolo selezionato non ha utenti');", true);
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Ritorna bianca", "top.principale.iFrame_dx.document.location='tabRisultatiRicTrasm.aspx?tiporic=R&home=N';", true);
                    }
                    else
                    {
                        if (queryStringPar_Verso.Equals("E") && this.chk_mio_ruolo.Checked == false && this.chk_visSott.Checked == false)
                        {
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_check", "alert('Selezionare almeno un mittente della trasmissione tra ruolo e sottoposti');", true);
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Ritorna bianca", "top.principale.iFrame_dx.document.location='tabRisultatiRicTrasm.aspx?tiporic=E&home=N';", true);
                        }
                        else
                        {
                            DocumentManager.removeFiltroRicTrasm(this);

                            this.SetPageOnCurrentContext();

                            this.PerformSearch();
                        }
                    }
                }
            }
        }

        private bool checkData(string argomento, string valore)
        {
            if (argomento.Equals("TRASMISSIONE_IL") ||
                argomento.Equals("TRASMISSIONE_SUCCESSIVA_AL") ||
                argomento.Equals("TRASMISSIONE_PRECEDENTE_IL") ||
                //argomento.Equals("TRASMISSIONE_SC") ||
                //argomento.Equals("TRASMISSIONE_MC") ||
                //argomento.Equals("TRASMISSIONE_TODAY") ||
                argomento.Equals("ACCETTATA_RIFIUTATA_IL") ||
                argomento.Equals("ACCETTATA_RIFIUTATA_SUCCESSIVA_AL") ||
                argomento.Equals("ACCETTATA_RIFIUTATA_PRECEDENTE_IL") ||
                argomento.Equals("SCADENZA_IL") ||
                argomento.Equals("SCADENZA_SUCCESSIVA_AL") ||
                argomento.Equals("SCADENZA_PRECEDENTE_IL") ||
                argomento.Equals("RISPOSTA_IL") ||
                argomento.Equals("RISPOSTA_SUCCESSIVA_AL") ||
                argomento.Equals("RISPOSTA_PRECEDENTE_IL") ||
                argomento.Equals("DATA_ACCETTAZIONE") ||
                argomento.Equals("DATA_ACCETTAZIONE_DA") ||
                argomento.Equals("DATA_ACCETTAZIONE_A") ||
                //argomento.Equals("DATA_ACCETTAZIONE_TODAY") ||
                //argomento.Equals("DATA_ACCETTAZIONE_SC") ||
                //argomento.Equals("DATA_ACCETTAZIONE_MC") ||
                argomento.Equals("DATA_RIFIUTO") ||
                argomento.Equals("DATA_RIFIUTO_DA") ||
                argomento.Equals("DATA_RIFIUTO_A") ||
                //argomento.Equals("DATA_RIFIUTO_TODAY") ||
                //argomento.Equals("DATA_RIFIUTO_SC") ||
                //argomento.Equals("DATA_RIFIUTO_MC") ||
                argomento.Equals("DATA_ACC_RIF") ||
                argomento.Equals("DATA_ACC_RIF_DA") ||
                argomento.Equals("DATA_ACC_RIF_A")
                //argomento.Equals("DATA_ACC_RIF_TODAY") ||
                //argomento.Equals("DATA_ACC_RIF_SC") ||
                //argomento.Equals("DATA_ACC_RIF_MC")
                )
            {
                if (!Utils.isDate(valore))
                    return false;
                if (Convert.ToDateTime(valore) < (new DateTime(1754, 01, 01)))
                    return false;
            }
            return true;
        }

        private void DDLOggettoTab1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //			if(this.DDLOggettoTab1.SelectedIndex!=-1 && this.DDLOggettoTab1.Items[this.DDLOggettoTab1.SelectedIndex].Value.Equals("D"))
            //				{
            //					Session["Tipo_obj"] = "D";
            //					this.ddl_tipo_doc.Visible= true;
            //					this.ddl_tipo_doc.SelectedIndex = 0;
            //				}
            //			else
            //				{
            //					Session["Tipo_obj"] = "F";
            //					this.ddl_tipo_doc.Visible= false;
            //					this.ddl_tipo_doc.SelectedIndex = 0;
            //				}
            if (this.ddl_oggetto.SelectedValue.Equals("F"))
            {
                this.tipo_doc.Visible = false;
                Session["Tipo_obj"] = "F";
            }

        }

        private void DDLFiltro1_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }

        private void ddl_tipo_doc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //if(this.ddl_tipo_doc.SelectedIndex>-1)
            //this.ddl_fasc.SelectedIndex=-1;
            Session["Tipo_obj"] = "D";
        }

        /// <summary>
        /// Azione selezione tipo filtro data trasmissione
        /// </summary>
        private void PerformActionSelectDataTrasmissione()
        {
            switch (this.ddl_dataTrasm.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataTrasm").Visible = false;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = false;
                    this.lbl_finedataTrasm.Visible = false;
                    this.lbl_initdataTrasm.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Enabled = true;
                    this.lbl_finedataTrasm.Visible = true;
                    this.lbl_initdataTrasm.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataTrasm").Visible = false;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = false;
                    this.lbl_finedataTrasm.Visible = false;
                    this.lbl_initdataTrasm.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Enabled = false;
                    this.lbl_finedataTrasm.Visible = true;
                    this.lbl_initdataTrasm.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Enabled = false;
                    this.lbl_finedataTrasm.Visible = true;
                    this.lbl_initdataTrasm.Visible = true;
                    break;
            }
        }

        private void ddl_dataTrasm_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.PerformActionSelectDataTrasmissione();
        }

        /// <summary>
        /// Azione di selezione corrispondente
        /// </summary>
        private void PerformActionSelectTipoCorrispondente()
        {
            string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
            if (use_new_rubrica != "1")
                this.btn_Rubrica_C.Attributes.Add("onclick", "ApriRubrica('ric_Trasm','" + this.rbl_tipo_corr.SelectedValue + "');");
            else
                btn_Rubrica_C.Attributes.Add("onclick", "_ApriRubricaRicercaTrasm('" + this.rbl_tipo_corr.SelectedValue + "');");

            //ripulisco i campi
            UserManager.removeCorrispondentiSelezionati(this);
            this.txt_codCorr.Text = "";
            this.txt_descrCorr.Text = "";
            this.hd_systemIdCorr.Value = "";

            // Se si sta visualizzando un ruolo viene visualizzato il checkbox "Estendi a storicizzati" 
            // checked di default
            if (this.rbl_tipo_corr.SelectedValue == "R")
            {
                this.chkHistoricized.Enabled = true;
                this.chkHistoricized.Checked = true;
            }
            else
            {
                this.chkHistoricized.Checked = false;
                this.chkHistoricized.Enabled = false;
            }

            this.EnableFieldDescrizioneCorrispondente();
        }

        /// <summary>
        /// Abilitazione / disabilitazione campo descrizione corrispondente
        /// in funzione del tipo corrispondente selezionato
        /// </summary>
        private void EnableFieldDescrizioneCorrispondente()
        {
            if (this.rbl_tipo_corr.SelectedValue.Equals("U"))
                this.txt_descrCorr.ReadOnly = true;
            else
                this.txt_descrCorr.ReadOnly = false;
        }

        private void rbl_tipo_corr_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.PerformActionSelectTipoCorrispondente();
        }

        private void setDescCorr(string codiceRubrica)
        {
            DocsPaWR.Corrispondente corr = null;
            UserManager.removeCorrispondentiSelezionati(this);
            if (Session["dictionaryCorrispondente"] != null)
                dic_Corr = (Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>)Session["dictionaryCorrispondente"];
            else
                dic_Corr = new Dictionary<string, Corrispondente>();

            if (!codiceRubrica.Equals(""))
            {
                codiceRubrica = "^^^" + this.rbl_tipo_corr.SelectedValue + codiceRubrica;
                //corr = UserManager.getCorrispondente(this, codiceRubrica, false);
                ElementoRubrica[] listaCorr = UserManager.getElementiRubricaMultipli(this.Page, codiceRubrica, RubricaCallType.CALLTYPE_RICERCA_CORRISPONDENTE, !this.chkHistoricized.Checked);
                if (listaCorr == null || (listaCorr != null && listaCorr.Length == 0))
                    corr_non_trovato();
                else
                {
                    if (listaCorr != null && listaCorr.Length > 1)
                    {
                        //dic_Corr[this.ID] = null;
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "multiCorr", "ApriFinestraMultiCorrispondenti();", true);
                        Session.Add("multiCorr", listaCorr);
                        //Session["noRicercaDesc"] = true;
                        //Session["idCorrMulti"] = Convert.ToInt32(this.ID);
                        return;
                    }
                    else
                    {
                        if (listaCorr != null && listaCorr.Length == 1)
                        {
                            DocsPAWA.DocsPaWR.ElementoRubrica er = listaCorr[0];
                            if (this.chkHistoricized.Enabled)
                            {
                                if (this.chkHistoricized.Checked)
                                    dic_Corr["ricTrasm"] = UserManager.getCorrispondenteBySystemIDDisabled(this.Page, er.systemId);
                                else
                                    dic_Corr["ricTrasm"] = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this.Page, er.codice, AddressbookTipoUtente.GLOBALE);
                            }
                            else
                                dic_Corr["ricTrasm"] = UserManager.getCorrispondenteBySystemIDDisabled(this.Page, er.systemId);

                            if (dic_Corr["ricTrasm"] != null)
                            {
                                this.hd_systemIdCorr.Value = er.systemId;
                                txt_descrCorr.Text = er.descrizione;
                                txt_codCorr.Text = er.codice;
                            }
                            else
                            {
                                this.hd_systemIdCorr.Value = string.Empty;
                                txt_descrCorr.Text = string.Empty;
                                txt_codCorr.Text = string.Empty;
                            }
                            Session.Add("dictionaryCorrispondente", dic_Corr);
                            //Session["noRicercaDesc"] = true;
                        }
                    }
                }
            }
            else
            {
                this.hd_systemIdCorr.Value = string.Empty;
                txt_descrCorr.Text = string.Empty;
                txt_codCorr.Text = string.Empty;
            }

            //if (corr != null)
            //{
            //    //bool trovato= false;
            //    //if (corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Ruolo)))
            //    //{
            //    //    if (tipoCorr.Equals("R"))
            //    //        trovato = true;
            //    //}
            //    //if (corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa)))
            //    //{
            //    //    if (tipoCorr.Equals("U"))
            //    //        trovato = true;
            //    //}
            //    //if (corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
            //    //{
            //    //    if (tipoCorr.Equals("P"))
            //    //        trovato = true;
            //    //}
            //    //if (trovato)
            //    //{
            //    this.txt_descrCorr.Text = UserManager.getDecrizioneCorrispondenteSemplice(corr);
            //    this.hd_systemIdCorr.Value = corr.systemId;
            //    UserManager.setCorrispondenteSelezionato(this.Page, corr);
            //    //}
            //    //else
            //    //{
            //    //    corr_non_trovato();
            //    //}
            //}
            //else
            //{
            //    corr_non_trovato();
            //}
        }

        private void setDescCorrSott(string codiceRubrica)
        {
            ViewState["colorItems"] = null;
            DocsPaWR.Corrispondente corr = null;
            UserManager.removeCorrispondentiSelezionati(this);
            if (!codiceRubrica.Equals(""))
                corr = UserManager.getCorrispondente(this, codiceRubrica, false);

            if (corr == null)
            {
                corr_sott_non_trovato("1");
            }
            else
            {
                if (!corr.tipoCorrispondente.Equals("R"))
                {
                    corr_sott_non_trovato("2");
                }
                else
                {
                    // Se il ruolo è un ruolo storicizzato, bisogna bloccare la drop down list degli utenti su
                    // "qualsiasi utente"
                    if (!String.IsNullOrEmpty(corr.dta_fine))
                    {
                        this.select_sottoposto.Items.Clear();
                        this.select_sottoposto.Items.Add(new ListItem("<<qualsiasi utente>>", "tutti"));
                        UserManager.setCorrispondenteSelezionatoSottopostoNoRubr(this.Page, corr);
                        this.txt1_corr_sott.Text = codiceRubrica;
                        this.txt2_corr_sott.Text = UserManager.getDecrizioneCorrispondenteSemplice(corr);
                        this.hd_systemIdCorrSott.Value = corr.systemId;
                    }
                    else
                    {
                        DataSet ds = new DataSet();
                        DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                        DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);
                        ds = wws.getUtentiInRuoloSottoposto(infoUtente, corr.systemId);

                        if (ds == null || ds.Tables.Count == 0)
                        {
                            corr_sott_non_trovato("3");
                        }
                        else
                        {
                            this.txt1_corr_sott.Text = "";
                            this.txt2_corr_sott.Text = "";
                            this.hd_systemIdCorrSott.Value = "";
                            this.select_sottoposto.Items.Clear();

                            UserManager.setCorrispondenteSelezionatoSottopostoNoRubr(this.Page, corr);
                            //this.chk_visSott.Checked = true;
                            this.txt1_corr_sott.Text = codiceRubrica;
                            this.txt2_corr_sott.Text = UserManager.getDecrizioneCorrispondenteSemplice(corr);
                            this.hd_systemIdCorrSott.Value = corr.systemId;

                            //NON CI SONO UTENTI NEL RUOLO
                            if (ds.Tables[0].Rows.Count == 0)
                            {
                                this.select_sottoposto.Items.Clear();
                                this.select_sottoposto.Enabled = false;

                            }
                            else
                            {
                                //POPOLA SELECT UTENTI

                                this.select_sottoposto.Enabled = true;

                                //this.select_sottoposto.Items.Add(new ListItem());
                                this.select_sottoposto.Items.Add(new ListItem("<<qualsiasi utente>>", "tutti"));
                                DocsPaWR.Ruolo mioRuolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];

                                int countCol = 1;
                                //SOLO PER IL MIO RULO
                                if (((mioRuolo.codiceRubrica).ToUpper()).Equals((this.txt1_corr_sott.Text).ToUpper()))
                                {
                                    this.select_sottoposto.Items.Add(new ListItem("<<gli altri utenti>>", "altri"));
                                    countCol = 2;
                                }
                                else
                                {
                                    this.chk_me_stesso.Checked = false;
                                }

                                this.select_sottoposto.DataSource = ds;
                                this.select_sottoposto.DataBind();

                                List<int> color = new List<int>();

                                for (int i = 0; i < this.select_sottoposto.Items.Count - countCol; i++)
                                {
                                    DataRow row = ds.Tables[0].Rows[i];
                                    string dataUtenteRuolo = row["DTA_FINE"].ToString();

                                    if (dataUtenteRuolo == null || dataUtenteRuolo.Equals(string.Empty))
                                    {
                                        this.select_sottoposto.Items[i + countCol].Attributes.Add("style", "color:#333333");
                                    }
                                    else
                                    {
                                        this.select_sottoposto.Items[i + countCol].Attributes.Add("style", "color:#990000");
                                        color.Add(i + countCol);
                                    }
                                }

                                ViewState["colorItems"] = color;
                            }
                        }
                    }
                }
            }
        }

        private void corr_non_trovato()
        {
            this.txt_codCorr.Text = "";
            this.txt_descrCorr.Text = "";
            this.hd_systemIdCorr.Value = "";
            if (!IsStartupScriptRegistered("corr_non_trovato"))
                RegisterStartupScript("corr_non_trovato",
                    "<script language=\"javascript\">" +
                    "alert (\"Codice corrispondente non trovato\");</script>");
        }

        private void corr_sott_non_trovato(string caso)
        {
            string s = null;
            this.txt1_corr_sott.Text = "";
            this.txt2_corr_sott.Text = "";
            this.hd_systemIdCorrSott.Value = "";
            this.select_sottoposto.Items.Clear();
            this.select_sottoposto.Enabled = false;
            this.chk_visSott.Checked = false;

            switch (caso)
            {
                case "1":
                    s = "Corrispondente non trovato";
                    break;

                case "2":
                    s = "Inserire un ruolo come corrispondente";
                    break;

                case "3":
                    s = "Inserire soltanto ruoli sottoposti";
                    break;
            }
            if (!IsStartupScriptRegistered("corr_non_trovato"))
                RegisterStartupScript("corr_non_trovato",
                    "<script language=\"javascript\">" +
                    "alert (\"" + s + "\");</script>");
        }

        private void txt_codCorr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                //setDescCorr(this.txt_codCorr.Text, this.rbl_tipo_corr.SelectedValue);	
                setDescCorr(this.txt_codCorr.Text);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void txt1_corr_sott_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                //setDescCorr(this.txt_codCorr.Text, this.rbl_tipo_corr.SelectedValue);	
                setDescCorrSott(this.txt1_corr_sott.Text);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }



        private void hd_systemIdCorr_ServerChange(object sender, System.EventArgs e)
        {

        }

        private void hd_systemIdCorrSott_ServerChange(object sender, System.EventArgs e)
        {
           
        }

        private void select_sottoposto_ServerChange(object sender, System.EventArgs e)
        {
            //USATO PER COLORARE I CAMPI DELLA SELECT QUANDO RICARICA LA PAGINA
            if (ViewState["colorItems"] != null)
            {
                List<int> color = ViewState["colorItems"] as List<int>;

                foreach (int colorTemp in color)
                {
                    this.select_sottoposto.Items[colorTemp].Attributes.Add("style", "color:#990000");
                }
                
            }   
        }

        /// <summary>
        /// Azione di selezione ragione trasmissione
        /// </summary>
        private void PerformActionSelectRagioniTrasmissione()
        {
            DocsPaWR.RagioneTrasmissione ragione;
            if (ddl_ragioni.SelectedIndex > 0)
            {
                if (m_hashTableRagioneTrasmissione == null)
                    m_hashTableRagioneTrasmissione = TrasmManager.getHashRagioneTrasmissione(this);
                if (m_hashTableRagioneTrasmissione == null)
                    return;
                ragione = (DocsPAWA.DocsPaWR.RagioneTrasmissione)m_hashTableRagioneTrasmissione[ddl_ragioni.SelectedIndex - 1];

                if (ragione.tipo.Equals("W"))
                {
                    this.AccettateRifiutate.Visible = true;

                }
                else
                {
                    this.AccettateRifiutate.Visible = false;
                    cbx_Pendenti.Checked = false;
                    cbx_Acc.Checked = false;
                    cbx_Rif.Checked = false;
                    ddl_TAR.SelectedIndex = 0;
                    ddl_TAR.Enabled = false;
                    lbl1_TAR.Visible = false;
                    this.GetCalendarControl("dataUno_TAR").txt_Data.Enabled = false;
                    this.GetCalendarControl("dataUno_TAR").btn_Cal.Visible = false;
                    this.GetCalendarControl("dataUno_TAR").txt_Data.Text = "";
                    lbl2_TAR.Visible = false;
                    this.GetCalendarControl("dataDue_TAR").txt_Data.Visible = false;
                    this.GetCalendarControl("dataDue_TAR").btn_Cal.Visible = false;
                    this.GetCalendarControl("dataDue_TAR").txt_Data.Text = "";
                }
            }
            else
            {
                this.AccettateRifiutate.Visible = true;
            }

            string s = "<SCRIPT language='javascript'>document.getElementById('" + ddl_ragioni.ID + "').focus() </SCRIPT>";
            RegisterStartupScript("focus", s);
        }

        private void ddl_ragioni_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.PerformActionSelectRagioniTrasmissione();
        }

        private void ricTrasmCompleta_PreRender(object sender, EventArgs e)
        {
            //if (schedaRicerca != null && this.ddl_Ric_Salvate.SelectedIndex > 0)
            //{
            //    schedaRicerca.Seleziona(Int32.Parse(ddl_Ric_Salvate.SelectedValue));
            //    if (ddl_Ric_Salvate.SelectedIndex > 0)
            //    {
            //        Session.Add("itemUsedSearch", ddl_Ric_Salvate.SelectedIndex.ToString());
            //    }
            //    BindFilterValues(schedaRicerca);
            //}
            // Ripristino contesto chiamante
            this.RestoreBackContext();

            //carico il mittente selezionato, se esiste
            DocsPaWR.Corrispondente cor = UserManager.getCorrispondenteSelezionato(this);
            if (cor != null)
            {
                this.txt_codCorr.Text = cor.codiceRubrica;
                this.txt_descrCorr.Text = UserManager.getDecrizioneCorrispondenteSemplice(cor);
                this.hd_systemIdCorr.Value = cor.systemId;
                this.rbl_tipo_corr.SelectedValue = cor.tipoCorrispondente;
            }

            if (Session["dictionaryCorrispondente"] != null)
            {
                dic_Corr = (Dictionary<string, Corrispondente>)Session["dictionaryCorrispondente"];
                if (dic_Corr != null && dic_Corr.ContainsKey("ricTrasm") && dic_Corr["ricTrasm"] != null)
                {
                    txt_codCorr.Text = dic_Corr["ricTrasm"].codiceRubrica;
                    this.txt_descrCorr.Text = dic_Corr["ricTrasm"].descrizione;
                    this.hd_systemIdCorr.Value = dic_Corr["ricTrasm"].systemId;
                    this.rbl_tipo_corr.SelectedValue = dic_Corr["ricTrasm"].tipoCorrispondente;
                }
                dic_Corr.Remove("ricTrasm");
                Session.Add("dictionaryCorrispondente", dic_Corr);
            }

            //carico il ruolo sottoposto, se esiste
            DocsPaWR.Corrispondente corrSott = UserManager.getCorrispondenteSelezionatoSottoposto(this);
            if (corrSott != null)
            {
                UserManager.removeCorrispondentiSelezionati(this);
                if (!corrSott.codiceRubrica.Equals(""))
                {
                    DataSet ds = new DataSet();
                    DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);
                    ds = wws.getUtentiInRuoloSottoposto(infoUtente, corrSott.systemId);
                    if (ds == null || ds.Tables.Count == 0)
                    {
                        corr_sott_non_trovato("3");
                    }
                    else
                    {
                        this.txt1_corr_sott.Text = corrSott.codiceRubrica;
                        this.txt2_corr_sott.Text = UserManager.getDecrizioneCorrispondenteSemplice(corrSott);
                        UserManager.setCorrispondenteSelezionatoSottoposto(this.Page, corrSott);
                        //this.chk_visSott.Checked = true;
                        
                        //NON CI SONO UTENTI NEL RUOLO
                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            this.select_sottoposto.Items.Clear();
                            this.select_sottoposto.Enabled = false;
                           
                        }
                        else
                        {
                            //POPOLA SELECT UTENTI
                            int countCol = 1;
                            DocsPaWR.Ruolo mioRuolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];

                            if (corrSott.systemId != this.hd_systemIdCorrSott.Value)
                            {
                                this.select_sottoposto.Items.Clear();
                                ViewState["colorItems"] = null;
                                this.select_sottoposto.Enabled = true;
                               // this.select_sottoposto.Items.Add(new ListItem());
                                this.select_sottoposto.Items.Add(new ListItem("<<qualsiasi utente>>", "tutti"));
                                
                                //SOLO PER IL MIO RULO
                                if (((mioRuolo.codiceRubrica).ToUpper()).Equals((this.txt1_corr_sott.Text).ToUpper()))
                                {
                                    this.select_sottoposto.Items.Add(new ListItem("<<gli altri utenti>>", "altri"));
                                    countCol = 2;
                                }
                                else
                                {
                                    this.chk_me_stesso.Checked = false;
                                }
                                this.select_sottoposto.DataSource = ds;
                                this.select_sottoposto.DataBind();
                            }
                            else
                            {
                                if (this.chk_mio_ruolo.Checked == true && !string.IsNullOrEmpty(this.txt1_corr_sott.Text))
                                {
                                    string selezionato = this.select_sottoposto.SelectedValue;
                                    this.select_sottoposto.Items.Clear();
                                    ViewState["colorItems"] = null;

                                    this.select_sottoposto.Enabled = true;
                                    // this.select_sottoposto.Items.Add(new ListItem());
                                    this.select_sottoposto.Items.Add(new ListItem("<<qualsiasi utente>>", "tutti"));
                                    //SOLO PER IL MIO RULO

                                    if (((mioRuolo.codiceRubrica).ToUpper()).Equals((this.txt1_corr_sott.Text).ToUpper()))
                                    {
                                        this.select_sottoposto.Items.Add(new ListItem("<<gli altri utenti>>", "altri"));
                                        countCol = 2;
                                    }

                                    this.select_sottoposto.DataSource = ds;
                                    this.select_sottoposto.DataBind();
                                    this.select_sottoposto.SelectedValue = selezionato;
                                }
                                else
                                {
                                    this.txt1_corr_sott.Text = string.Empty;
                                    this.txt2_corr_sott.Text = string.Empty;
                                    this.select_sottoposto.Enabled = false;
                                }
                               
                                

                            }

                            this.hd_systemIdCorrSott.Value = corrSott.systemId;

                            List<int> color = new List<int>();

                            for (int i = 0; i < this.select_sottoposto.Items.Count - countCol; i++)
                            {
                                DataRow row = ds.Tables[0].Rows[i];
                                string dataUtenteRuolo = row["DTA_FINE"].ToString();

                                if (dataUtenteRuolo == null || dataUtenteRuolo.Equals(string.Empty))
                                {
                                    this.select_sottoposto.Items[i + countCol].Attributes.Add("style", "color:#333333");
                                }
                                else
                                {
                                    this.select_sottoposto.Items[i + countCol].Attributes.Add("style", "color:#990000");
                                    color.Add(i + countCol);
                                }
                            }

                            ViewState["colorItems"] = color;
                        }
                    }
                }
            }


            btn_Rubrica_C.Attributes["onMouseOver"] = "javascript:ImpostaCursore (1,'" + btn_Rubrica_C.ClientID + "');";
            btn_Rubrica_C.Attributes["onMouseOut"] = "javascript:ImpostaCursore (0,'" + btn_Rubrica_C.ClientID + "');";

            btn_img_sott_rubr.Attributes["onMouseOver"] = "javascript:ImpostaCursore (1,'" + btn_img_sott_rubr.ClientID + "');";
            btn_img_sott_rubr.Attributes["onMouseOut"] = "javascript:ImpostaCursore (0,'" + btn_img_sott_rubr.ClientID + "');";

            if (!this.Page.IsClientScriptBlockRegistered("imposta_cursore"))
            {
                this.Page.RegisterClientScriptBlock("imposta_cursore",
                    "<script language=\"javascript\">\n" +
                    "function ImpostaCursore (t, ctl)\n{\n" +
                    "document.getElementById(ctl).style.cursor = (t == 0) ? 'default' : 'hand';\n" +
                    "}\n</script>\n");
            }
        }

        private void tastoInvio()
        {
            Utils.DefaultButton(this, ref this.GetCalendarControl("txt_initDataTrasm").txt_Data, ref btn_ricercaTrasm);
            Utils.DefaultButton(this, ref this.GetCalendarControl("txt_fineDataTrasm").txt_Data, ref btn_ricercaTrasm);
            Utils.DefaultButton(this, ref ddl_ragioni, ref btn_ricercaTrasm);
          //  Utils.DefaultButton(this, ref DDLFiltro1, ref btn_ricercaTrasm);
          //  Utils.DefaultButton(this, ref DDLFiltro2, ref btn_ricercaTrasm);
        }


        #region Gestione call context

        /// <summary>
        /// Impostazione filtri correntemente selezionati
        /// nello stato del contesto di ricerca corrente
        /// </summary>
        /// <param name="filters"></param>
        private void SetFiltersOnCurrentContext(DocsPAWA.DocsPaWR.FiltroRicerca[] filters)
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_TRASMISSIONI)
            {
                currentContext.SessionState["MemoriaFiltriRicTrasm"] = filters;

                // Impostazione scheda ricerca nel contesto corrente
                // in modo da ripristinare i valori dei filtri effettuati
                string keySchedaRicerca = this.SchedaRicercaSessionKey;
                currentContext.SessionState[keySchedaRicerca] = Session[keySchedaRicerca];
            }
        }

        /// <summary>
        /// Impostazione numero pagina corrente del contesto di ricerca
        /// </summary>
        private void SetPageOnCurrentContext()
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_TRASMISSIONI)
                currentContext.PageNumber = 1;
        }

        /// <summary>
        /// Ripristino contesto di ricerca trasmissioni
        /// </summary>
        private void RestoreBackContext()
        {
            // Ripristino contesto back
            if (!this.IsPostBack &&
                Request.QueryString["back"] != null &&
                Request.QueryString["back"].ToLower().Equals("true"))
            {
                // Ripristino valori filtri di ricerca
                this.RestoreSearchFilters();

                DocsPaWR.FiltroRicerca[] filters = TrasmManager.getMemoriaFiltriRicTrasm(this);

                if (filters != null)
                    this.PerformSearch(filters);
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
            DocsPAWA.ricercaDoc.SchedaRicerca schedaRicerca = new DocsPAWA.ricercaDoc.SchedaRicerca();
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
                this.BindFilterValues(schedaRicerca);
            }
        }

        /// <summary>
        /// Ripristino valori filtri di ricerca nei campi della UI
        /// </summary>
        /// <param name="schedaRicerca"></param>
        private void BindFilterValues(DocsPAWA.ricercaDoc.SchedaRicerca schedaRicerca)
        {
            // devo valorizzare verso con l'informazione prelevata dalla ricerca salvata
            string verso = Request.QueryString["verso"];

            if (schedaRicerca.FiltriRicerca != null && schedaRicerca.FiltriRicerca.Length > 0)
            {
                try
                {
                    try
                    {
                        //Session.Add("itemUsedSearch", ddl_Ric_Salvate.SelectedIndex.ToString());
                        if (Session["itemUsedSearch"] != null)
                            this.ddl_Ric_Salvate.SelectedIndex = Convert.ToInt32(Session["itemUsedSearch"].ToString());
                        DocsPaWR.FiltroRicerca[] filterItems = schedaRicerca.FiltriRicerca[0];

                        bool cercaInferiori = true;
                        bool ruoloCheck = false;

                        this.chk_me_stesso.Checked = false;
                        this.chk_mio_ruolo.Checked = false;
                        //this.chk_tutti_ruolo.Checked = false;
                        
                        foreach (DocsPaWR.FiltroRicerca item in filterItems)
                        {
                            // Ripristino filtro su tipologia di oggetto trasmesso (doc o fascicolo)
                            this.RestoreFiltersTipoOggettoTrasmesso(item);

                            // Ripristino filtro mittente / destinatario
                            this.RestoreFiltersMittenteDestinatario(item);

                            // Ripristino filtro mittente / destinatario sottoposto
                            this.RestoreFiltersMittenteDestinatarioSottoposto(item);

                            // Ripristino filtri data trasmissione
                            this.RestoreFiltersDataTrasmissione(item);

                            // Ripristino filtro ragione
                            this.RestoreRagioneTrasmissione(item);

                            // Ripristino filtro su data accettazione / rifiuto
                            this.RestoreFiltersAccettateRifiutate(item);

                            // Ripristino filtro su pannello Completamento
                            this.RestoreFilterCompletamento(item);

                            // Ripristino filtri su checkbox
                            this.RestoreFilterMeStessoRuoli(item);

                            // Ripristino filtri custom
                            this.RestoreCustomFilters(item);

                            // Ripristino filtri diagramma di stato
                            this.RestoreDiagrammaDiStato(item);

                            // Ripristino filtri tipologia documento
                            this.RestoreTipologiaDocumento(item);

                            //Ripristino filtri tipologia fascicolo
                            this.RestoreTipologiaFascicolo(item);

                            //Ripristino filtro Diagramma Stato Fascicolo
                            this.RestoreDiagrammaDiStatoFasc(item);
  
                            // Ripristino su filtro "visualizzazione sottoposti"
                            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.NO_CERCA_INFERIORI.ToString())
                                if (item.valore.ToUpper().Equals("TRUE"))
                                    cercaInferiori = false;

                            if (item.argomento.Equals(DocsPaWR.FiltriTrasmissioneNascosti.RICEVUTE_EFFETTUATE.ToString()))
                                verso = item.valore;

                            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.FIRMATO.ToString())
                            {
                                if (item.valore.Equals("1"))
                                {
                                    this.chk_firmati.Checked = true;
                                }
                                else
                                {
                                    this.chk_non_firmati.Checked = true;
                                }
                            }

                            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TIPO_FILE_ACQUISITO.ToString())
                            {
                                this.ddl_tipoFileAcquisiti.Items.Clear();
                                caricaComboTipoFileAcquisiti();
                                this.ddl_tipoFileAcquisiti.SelectedValue = item.valore;
                                
                            }

                            // Ripristino flags ricerca estesa agli storicizzati per ruoli mittente e destinatario
                            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString())
                                this.chkHistoricized.Checked = Boolean.Parse(item.valore);
                            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.RUOLO_EXTEND_TO_HISTORICIZED.ToString())
                                this.chkHistoricizedRole.Checked = Boolean.Parse(item.valore);

                        }

                        if (this.chk_visSott.Visible)
                            this.chk_visSott.Checked = cercaInferiori;


                        if (Session["oneTimeRicTrasm"] != null)
                            Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='gestioneRicTrasm.aspx?verso=" + verso + "&oneTime=1';</script>");

                        if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
                            verificaCampiPersonalizzati();

                        if (this.ruoloCheck == true)
                        {
                            this.select_sottoposto.Enabled = true;
                            this.btn_img_sott_rubr.Visible = true;
                        }
                        else
                        {
                            this.txt1_corr_sott.Text = string.Empty;
                            this.txt2_corr_sott.Text = string.Empty;
                            this.select_sottoposto.Items.Clear();
                            this.select_sottoposto.Enabled = false;
                            this.btn_img_sott_rubr.Visible = false;
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
        /// Ripristino filtro su data accettazione / rifiuto
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersAccettateRifiutate(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;
            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.PENDENTI.ToString())
            {
                this.cbx_Pendenti.Checked = (item.valore.IndexOf("P") > -1);
                retValue = true;
            }

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.ACCETTATA_RIFIUTATA.ToString())
            {
                bool accettate = (item.valore.IndexOf("A") > -1);
                bool rifiutate = (item.valore.IndexOf("R") > -1);

                this.cbx_Acc.Checked = accettate;
                this.cbx_Rif.Checked = rifiutate;

                this.impostaAccettateRifiutate();

                retValue = true;
            }

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO.ToString())
            {
                this.GetCalendarControl("dataUno_TAR").txt_Data.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_DA.ToString() ||
                     item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_DA.ToString() ||
                     item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_DA.ToString())
            {
                // Azione di selezione tipo filtro per data
                this.ddl_TAR.SelectedValue = "0";
                this.PerformActionSelectTAR();
                this.GetCalendarControl("dataUno_TAR").txt_Data.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_A.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_A.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_A.ToString())
            {
                // Azione di selezione tipo filtro per data
                this.ddl_TAR.SelectedValue = "1";
                this.PerformActionSelectTAR();
                this.GetCalendarControl("dataDue_TAR").txt_Data.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_TODAY.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_TODAY.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_TODAY.ToString())
            {
                this.ddl_TAR.SelectedValue = "2";
                this.PerformActionSelectTAR();
                this.GetCalendarControl("dataUno_TAR").Visible = true;
                this.GetCalendarControl("dataUno_TAR").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                this.GetCalendarControl("dataUno_TAR").txt_Data.Visible = true;
                this.GetCalendarControl("dataUno_TAR").txt_Data.Enabled = false;
                this.GetCalendarControl("dataUno_TAR").btn_Cal.Visible = true;
                this.GetCalendarControl("dataUno_TAR").btn_Cal.Enabled = false;
                this.GetCalendarControl("dataDue_TAR").Visible = false;
                this.GetCalendarControl("dataDue_TAR").txt_Data.Visible = false;
                this.GetCalendarControl("dataDue_TAR").btn_Cal.Visible = false;
                this.lbl1_TAR.Visible = false;
                this.lbl2_TAR.Visible = false;

                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_SC.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_SC.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_SC.ToString())
            {
                this.ddl_TAR.SelectedValue = "3";
                this.GetCalendarControl("dataUno_TAR").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                this.GetCalendarControl("dataUno_TAR").txt_Data.Enabled = false;
                this.GetCalendarControl("dataUno_TAR").btn_Cal.Enabled = false;
                this.GetCalendarControl("dataDue_TAR").Visible = true;
                this.GetCalendarControl("dataDue_TAR").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                this.GetCalendarControl("dataDue_TAR").txt_Data.Visible = true;
                this.GetCalendarControl("dataDue_TAR").btn_Cal.Visible = true;
                this.GetCalendarControl("dataDue_TAR").txt_Data.Enabled = false;
                this.GetCalendarControl("dataDue_TAR").btn_Cal.Enabled = false;
                this.lbl1_TAR.Visible = true;
                this.lbl2_TAR.Visible = true;

                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_MC.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_MC.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_MC.ToString())
            {
                this.ddl_TAR.SelectedValue = "4";
                this.GetCalendarControl("dataUno_TAR").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                this.GetCalendarControl("dataUno_TAR").txt_Data.Enabled = false;
                this.GetCalendarControl("dataUno_TAR").btn_Cal.Enabled = false;
                this.GetCalendarControl("dataDue_TAR").Visible = true;
                this.GetCalendarControl("dataDue_TAR").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                this.GetCalendarControl("dataDue_TAR").txt_Data.Visible = true;
                this.GetCalendarControl("dataDue_TAR").btn_Cal.Visible = true;
                this.GetCalendarControl("dataDue_TAR").txt_Data.Enabled = false;
                this.GetCalendarControl("dataDue_TAR").btn_Cal.Enabled = false;
                this.lbl1_TAR.Visible = true;
                this.lbl2_TAR.Visible = true;

                retValue = true;
            }

            return retValue;
        }

        private void RestoreFilterCompletamento(DocsPaWR.FiltroRicerca item)
        {
            if (item != null)
            {
                if (item.argomento.Equals(DocsPaWR.FiltriTrasmissioneNascosti.DA_PROTOCOLLARE.ToString()))
                {
                    this.P_Prot.Checked = true;
                }
                else if (item.argomento.Equals(DocsPaWR.FiltriTrasmissioneNascosti.MANCANZA_FASCICOLAZIONE.ToString()))
                {
                    this.M_Fasc.Checked = true;
                }
                else if (item.argomento.Equals(DocsPaWR.FiltriTrasmissioneNascosti.MANCANZA_IMMAGINE.ToString()))
                {
                    this.M_Img.Checked = true;
                }
                else if (item.argomento.Equals(DocsPaWR.FiltriTrasmissioneNascosti.CON_IMMAGINE.ToString()))
                {
                    this.M_si_img.Checked = true;
                    this.chk_firmati.Visible = true;
                    this.chk_non_firmati.Visible = true;
                    this.lbl_panel_con_imm.Visible = true;
                }
              
            }
        }

        /// <summary>
        /// Ripristino filtro sul tipo di oggetto trasmesso
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private void RestoreFiltersTipoOggettoTrasmesso(DocsPaWR.FiltroRicerca item)
        {
            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString() && item.valore == "F")
            {
                this.ddl_oggetto.SelectedValue = item.valore;
                this.ddl_tipo_doc.Visible = false;
                this.pnl_ric_fasc_prof.Visible = true;
                this.ddl_tipoFasc.Visible = true;
                this.ddl_tipoDoc_C.Visible = false;
                this.completamento.Visible = false;
                this.ddl_tipo_doc.Visible = false;
                this.ddl_tipoFasc.Items.Clear();
                CaricaComboTipologiaFasc();
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROTOCOLLATI.ToString())
                this.ddl_tipo_doc.SelectedValue = "P";
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROT_ARRIVO.ToString())
                this.ddl_tipo_doc.SelectedValue = "PA";
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROT_PARTENZA.ToString())
                this.ddl_tipo_doc.SelectedValue = "PP";
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_NON_PROTOCOLLATI.ToString())
                this.ddl_tipo_doc.SelectedValue = "NP";
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_TUTTI.ToString())
                this.ddl_tipo_doc.SelectedValue = "Tutti";
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROT_INTERNO.ToString())
                this.ddl_tipo_doc.SelectedValue = "PI";
        }

        /// <summary>
        /// Ripristino valori filtri custom
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dropDown"></param>
        /// <param name="textBox"></param>
        /// <returns></returns>

        private bool RestoreCustomFilters(DocsPaWR.FiltroRicerca item)
        {
            Boolean result = false;

            if (item.argomento.Equals("NOTE_GENERALI"))
            {
                this.txt_note_generali.Text = item.valore;
                result = true;
            }

            if (item.argomento.Equals("NOTE_INDIVIDUALI"))
            {
                this.txt_note_individuali.Text = item.valore;
                result = true;
            }

         /*   if (item.argomento.Equals("RISPOSTA_SUCCESSIVA_AL") || item.argomento.Equals("RISPOSTA_IL"))
            {
                this.cld_risposta_al.Text = item.valore;
                result = true;
            }
          * */

          /*  if (item.argomento.Equals("RISPOSTA_PRECEDENTE_IL"))
            {
                this.cld_risposta_dal.Text = item.valore;
                result = true;
            }
           */

            if (item.argomento.Equals("SCADENZA_SUCCESSIVA_AL") || item.argomento.Equals("SCADENZA_IL"))
            {
                this.cld_scadenza_al.Text = item.valore;
                result = true;
            }

            if (item.argomento.Equals("SCADENZA_PRECEDENTE_IL"))
            {
                this.cld_scadenza_dal.Text = item.valore;
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Ripristino filtro diagramma di stato
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private void RestoreDiagrammaDiStato(DocsPaWR.FiltroRicerca item)
        {
            string s = null;
            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DIAGRAMMA_STATO_DOC.ToString())
            {
                if (this.ddl_statiDoc.SelectedIndex > 0)
                {
                    this.ddl_statiDoc.SelectedIndex = 0;
                }
                this.ddl_statiDoc.Visible = true;
                this.Panel_StatiDocumento.Visible = true;
                attivaProfilazioneDinamica();
                string val = null;
                s = "DPA_DIAGRAMMI.ID_STATO";
                    s = item.valore.Substring(item.valore.LastIndexOf(s)).Trim();
                    s = s.Substring(0, s.Length - 1);
                    s = s.Replace(" ", "");
                    char[] sep = { '=' };
                    string[] tks = s.Split(sep);
                    val = tks[1];

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
        }

        /// <summary>
        /// Ripristino filtro tipologia documento
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private void RestoreTipologiaDocumento(DocsPaWR.FiltroRicerca item)
        {
            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TIPO_ATTO.ToString())
            {
                CaricaComboTipologiaAtto(ddl_tipoDoc_C);
                this.ddl_tipoDoc_C.SelectedValue = item.valore;
            }
        }

        private void RestoreTipologiaFascicolo(DocsPaWR.FiltroRicerca item)
        {
            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TIPOLOGIA_FASCICOLO.ToString())
            {
                this.pnl_ric_fasc_prof.Visible = true;
                this.ddl_tipoFasc.Visible = true;
                this.ddl_tipoDoc_C.Visible = false;
                this.completamento.Visible = false;
                this.ddl_tipo_doc.Visible = false;
                this.ddl_tipoFasc.SelectedValue = item.valore;
                verificaCampiPersonalizzatiFasc();
                this.ddl_tipoFasc.Items.Clear();
                CaricaComboTipologiaFasc();
                this.ddl_tipoFasc.SelectedValue = item.valore;
            }
        }

        private void RestoreDiagrammaDiStatoFasc(DocsPaWR.FiltroRicerca item)
        {
            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DIAGRAMMA_STATO_FASC.ToString())
            {
                if (this.ddl_statiFasc.SelectedIndex > 0)
                {
                    this.ddl_statiFasc.SelectedIndex = 0;
                }
                this.ddl_statiFasc.Visible = true;
                this.Panel_StatiFascicolo.Visible = true;
                CaricaComboStatiFasc();
                this.ddl_statiFasc.SelectedValue = item.valore;
            }
        }

        /// <summary>
        /// Ripristino filtro mittente destinatario
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersMittenteDestinatario(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            string id = string.Empty;
            string codice = string.Empty;
            string descrizione = string.Empty;

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.ID_UO_MITT.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.ID_UO_DEST.ToString())
            {
                DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteBySystemID(this, item.valore);

                if (corr != null)
                {
                    id = corr.systemId;
                    codice = corr.codiceRubrica;
                    descrizione = corr.descrizione;

                    retValue = true;
                }
            }

            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_MITT_RUOLO.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_DEST_RUOLO.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_MITT_UTENTE.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_DEST_UTENTE.ToString())
            {
                codice = item.valore;

                retValue = true;
            }

            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_RUOLO.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DESTINATARIO_RUOLO.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_UTENTE.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DESTINATARIO_UTENTE.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_UO.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DESTINATARIO_UO.ToString())
            {
                descrizione = item.valore;

                retValue = true;
            }

            if (retValue)
            {
                if (descrizione == string.Empty)
                {
                    // Reperimento del valore della descrizione, se non memorizzato
                    DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubrica(this, codice);

                    if (corr != null)
                    {
                        descrizione = corr.descrizione;
                        id = corr.systemId;
                    }
                }

                this.txt_descrCorr.Text = descrizione;
                this.hd_systemIdCorr.Value = id;
                this.txt_codCorr.Text = codice;

                // Determinazione del tipo corrispondente
                string tipoCorrispondente = string.Empty;

                if (item.argomento.IndexOf("UTENTE") > -1)
                    tipoCorrispondente = "P";
                else if (item.argomento.IndexOf("RUOLO") > -1)
                    tipoCorrispondente = "R";
                else if (item.argomento.IndexOf("UO") > -1)
                    tipoCorrispondente = "U";

                if (tipoCorrispondente != string.Empty)
                {
                    this.rbl_tipo_corr.SelectedValue = tipoCorrispondente;

                    // Azione di selezione tipo corrispondente
                    this.EnableFieldDescrizioneCorrispondente();
                }
            }

            return retValue;
        }

        /// <summary>
        /// Ripristino filtro su checkbox me stesso e ruolo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private void RestoreFilterMeStessoRuoli(DocsPaWR.FiltroRicerca item)
        {

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.A_ME_STESSO.ToString())
            {
                if (item.valore.Equals("true"))
                {
                    this.chk_me_stesso.Checked = true;
                }
            }

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.AL_MIO_RUOLO.ToString())
            {
                if (item.valore.Equals("true"))
                {
                    this.chk_mio_ruolo.Checked = true;
                    this.ruoloCheck = true;
                }
            }

        /*    if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DAL_MIO_RUOLO.ToString())
            {
                if (item.valore.Equals("true"))
                {
                    this.chk_tutti_ruolo.Checked = true;
                }
            }
        */
        }

        /// <summary>
        /// Ripristino filtro mittente destinatario sottoposto
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersMittenteDestinatarioSottoposto(DocsPaWR.FiltroRicerca item)
        {

            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.RUOLO_SOTTOPOSTO.ToString())
            {
                DocsPaWR.Corrispondente corrSott = null;
                if (!item.valore.Equals(""))
                {
                    corrSott = UserManager.getCorrispondenteBySystemID(this, item.valore);
                    string codiceRub = corrSott.codiceRubrica;
                    DataSet ds = new DataSet();
                    DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);
                    ds = wws.getUtentiInRuoloSottoposto(infoUtente, corrSott.systemId);
                    this.txt2_corr_sott.Text = UserManager.getDecrizioneCorrispondenteSemplice(corrSott);
                    this.txt1_corr_sott.Text = codiceRub;
                    this.hd_systemIdCorrSott.Value = corrSott.systemId;
                    UserManager.setCorrispondenteSelezionatoSottoposto(this.Page, corrSott);
                    //this.chk_visSott.Checked = true;
                    this.select_sottoposto.Items.Clear();
                    this.select_sottoposto.SelectedValue = "tutti";

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        this.select_sottoposto.Enabled = false;

                    }
                    else
                    {
                        //POPOLA SELECT UTENTI
                        this.select_sottoposto.Enabled = true;

                        //this.select_sottoposto.Items.Add(new ListItem());
                        this.select_sottoposto.Items.Add(new ListItem("<<qualsiasi utente>>", "tutti"));
                        DocsPaWR.Ruolo mioRuolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];

                        int countCol = 1;
                        //SOLO PER IL MIO RULO
                        if (((mioRuolo.codiceRubrica).ToUpper()).Equals((this.txt1_corr_sott.Text).ToUpper()))
                        {
                            this.select_sottoposto.Items.Add(new ListItem("<<gli altri utenti>>", "altri"));
                            countCol = 2;
                        }
                        else
                        {
                            this.chk_me_stesso.Checked = false;
                        }
                        this.select_sottoposto.DataSource = ds;
                        this.select_sottoposto.DataBind();

                        List<int> color = new List<int>();

                        for (int i = 0; i < this.select_sottoposto.Items.Count - countCol; i++)
                        {
                            DataRow row = ds.Tables[0].Rows[i];
                            string dataUtenteRuolo = row["DTA_FINE"].ToString();

                            if (dataUtenteRuolo == null || dataUtenteRuolo.Equals(string.Empty))
                            {
                                this.select_sottoposto.Items[i + countCol].Attributes.Add("style", "color:#333333");
                            }
                            else
                            {
                                this.select_sottoposto.Items[i + countCol].Attributes.Add("style", "color:#990000");
                                color.Add(i + countCol);
                            }
                        }

                        ViewState["colorItems"] = color;
                    }
                }
                
            }

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.PERSONA_SOTTOPOSTA.ToString())
            {
                if (!item.valore.Equals(""))
                {
                    this.select_sottoposto.SelectedValue = item.valore;
                }
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterItem"></param>
        /// <returns></returns>
        private bool RestoreRagioneTrasmissione(DocsPaWR.FiltroRicerca filterItem)
        {
            if (filterItem.argomento == DocsPaWR.FiltriTrasmissioneNascosti.RAGIONE.ToString())
            {
                foreach (ListItem item in this.ddl_ragioni.Items)
                {
                    item.Selected = false;
                    if (item.Text == filterItem.valore)
                    {
                        item.Selected = true;
                        this.PerformActionSelectRagioniTrasmissione();
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Ripristino filtri data trasmissione
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersDataTrasmissione(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_SUCCESSIVA_AL.ToString())
            {
                this.ddl_dataTrasm.SelectedValue = "1";
                this.PerformActionSelectDataTrasmissione();
                this.RestoreTextBoxValue(this.GetCalendarControl("txt_initDataTrasm").txt_Data, item, DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_SUCCESSIVA_AL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_PRECEDENTE_IL.ToString())
            {
                this.RestoreTextBoxValue(this.GetCalendarControl("txt_fineDataTrasm").txt_Data, item, DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_PRECEDENTE_IL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_IL.ToString())
            {
                this.ddl_dataTrasm.SelectedValue = "0";
                this.PerformActionSelectDataTrasmissione();

                this.RestoreTextBoxValue(this.GetCalendarControl("txt_initDataTrasm").txt_Data, item, DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_IL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_TODAY.ToString())
            {
                this.ddl_dataTrasm.SelectedIndex = 2;
                this.GetCalendarControl("txt_initDataTrasm").Visible = true;
                this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                this.GetCalendarControl("txt_initDataTrasm").txt_Data.Visible = true;
                this.GetCalendarControl("txt_initDataTrasm").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_fineDataTrasm").Visible = false;
                this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = false;
                this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = false;
                this.lbl_initdataTrasm.Visible = false;
                this.lbl_finedataTrasm.Visible = false;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_SC.ToString())
            {
                this.ddl_dataTrasm.SelectedIndex = 3;
                this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                this.GetCalendarControl("txt_initDataTrasm").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_fineDataTrasm").Visible = true;
                this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = true;
                this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Enabled = false;
                this.lbl_initdataTrasm.Visible = true;
                this.lbl_finedataTrasm.Visible = true;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_MC.ToString())
            {
                this.ddl_dataTrasm.SelectedIndex = 4;
                this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                this.GetCalendarControl("txt_initDataTrasm").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Enabled = false;
                this.GetCalendarControl("txt_fineDataTrasm").Visible = true;
                this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = true;
                this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Enabled = false;
                this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Enabled = false;
                this.lbl_initdataTrasm.Visible = true;
                this.lbl_finedataTrasm.Visible = true;
                retValue = true;
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="filterItem"></param>
        /// <param name="filterType"></param>
        private bool RestoreTextBoxValue(TextBox textBox, DocsPaWR.FiltroRicerca filterItem, Enum filterType)
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
        private bool RestoreDropDownValue(DropDownList dropDown, DocsPaWR.FiltroRicerca filterItem, Enum filterType)
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
                return string.Concat("RicercaTrasmissioni_", DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY);
            }
        }

        #endregion

        #region filtriRicerca_TAR - Tutte Accettate Rifiutate Pendenti
        protected void impostaAccettateRifiutate()
        {
            //Nessuna selezione
            if (!cbx_Acc.Checked && !cbx_Rif.Checked && !cbx_Pendenti.Checked)
            {
                ddl_TAR.SelectedIndex = 0;
                ddl_TAR.Enabled = false;
                lbl1_TAR.Visible = false;
                this.GetCalendarControl("dataUno_TAR").txt_Data.Enabled = false;
                this.GetCalendarControl("dataUno_TAR").btn_Cal.Visible = false;
                this.GetCalendarControl("dataUno_TAR").txt_Data.Text = "";
                lbl2_TAR.Visible = false;
                this.GetCalendarControl("dataDue_TAR").txt_Data.Visible = false;
                this.GetCalendarControl("dataDue_TAR").btn_Cal.Visible = false;
                this.GetCalendarControl("dataDue_TAR").txt_Data.Text = "";
            }

            //Accettate
            if (cbx_Acc.Checked)
            {
                ddl_TAR.Enabled = true;
                this.GetCalendarControl("dataUno_TAR").txt_Data.Enabled = true;
                this.GetCalendarControl("dataUno_TAR").btn_Cal.Visible = true;
            }

            //Rifiutate
            if (cbx_Rif.Checked)
            {
                ddl_TAR.Enabled = true;
                this.GetCalendarControl("dataUno_TAR").txt_Data.Enabled = true;
                this.GetCalendarControl("dataUno_TAR").btn_Cal.Visible = true;
            }

            //Accettate-Rifiutate
            if (cbx_Acc.Checked && cbx_Rif.Checked)
            {
                ddl_TAR.Enabled = true;
                this.GetCalendarControl("dataUno_TAR").txt_Data.Enabled = true;
                this.GetCalendarControl("dataUno_TAR").btn_Cal.Visible = true;
            }

            //Pendenti
            if (cbx_Pendenti.Checked)
            {
                cbx_Acc.Checked = false;
                cbx_Rif.Checked = false;
                ddl_TAR.SelectedIndex = 0;
                ddl_TAR.Enabled = false;
                lbl1_TAR.Visible = false;
                this.GetCalendarControl("dataUno_TAR").txt_Data.Enabled = false;
                this.GetCalendarControl("dataUno_TAR").btn_Cal.Visible = false;
                this.GetCalendarControl("dataUno_TAR").txt_Data.Text = "";
                lbl2_TAR.Visible = false;
                this.GetCalendarControl("dataDue_TAR").txt_Data.Visible = false;
                this.GetCalendarControl("dataDue_TAR").btn_Cal.Visible = false;
                this.GetCalendarControl("dataDue_TAR").txt_Data.Text = "";
            }
        }

        /// <summary>
        /// Azione di selezione data accettazione / rifiuto
        /// </summary>
        private void PerformActionSelectTAR()
        {
            switch (ddl_TAR.SelectedItem.Text)
            {
                case "Valore Singolo":
                    this.GetCalendarControl("dataUno_TAR").Visible = true;
                    this.GetCalendarControl("dataUno_TAR").btn_Cal.Visible = true;
                    this.GetCalendarControl("dataUno_TAR").btn_Cal.Enabled = true;
                    this.GetCalendarControl("dataUno_TAR").txt_Data.Visible = true;
                    this.GetCalendarControl("dataUno_TAR").txt_Data.Enabled = true;
                    this.GetCalendarControl("dataDue_TAR").Visible = false;
                    this.GetCalendarControl("dataDue_TAR").btn_Cal.Visible = false;
                    this.GetCalendarControl("dataDue_TAR").txt_Data.Visible = false;
                    this.lbl1_TAR.Visible = false;
                    this.lbl2_TAR.Visible = false;
                    break;
                case "Intervallo":
                    this.GetCalendarControl("dataUno_TAR").Visible = true;
                    this.GetCalendarControl("dataUno_TAR").btn_Cal.Visible = true;
                    this.GetCalendarControl("dataUno_TAR").btn_Cal.Enabled = true;
                    this.GetCalendarControl("dataUno_TAR").txt_Data.Visible = true;
                    this.GetCalendarControl("dataUno_TAR").txt_Data.Enabled = true;
                    this.GetCalendarControl("dataDue_TAR").Visible = true;
                    this.GetCalendarControl("dataDue_TAR").btn_Cal.Visible = true;
                    this.GetCalendarControl("dataDue_TAR").btn_Cal.Enabled = true;
                    this.GetCalendarControl("dataDue_TAR").txt_Data.Visible = true;
                    this.GetCalendarControl("dataDue_TAR").txt_Data.Enabled = true;
                    this.lbl1_TAR.Visible = true;
                    this.lbl2_TAR.Visible = true;
                    break;

                case "Oggi":
                    this.GetCalendarControl("dataUno_TAR").Visible = true;
                    this.GetCalendarControl("dataUno_TAR").btn_Cal.Visible = true;
                    this.GetCalendarControl("dataUno_TAR").btn_Cal.Enabled = false;
                    this.GetCalendarControl("dataUno_TAR").txt_Data.Visible = true;
                    this.GetCalendarControl("dataUno_TAR").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("dataUno_TAR").txt_Data.Enabled = false;
                    this.GetCalendarControl("dataDue_TAR").Visible = false;
                    this.GetCalendarControl("dataDue_TAR").btn_Cal.Visible = false;
                    this.GetCalendarControl("dataDue_TAR").txt_Data.Visible = false;
                    this.lbl1_TAR.Visible = false;
                    this.lbl2_TAR.Visible = false;
                    break;

                case "Settimana Corr.":
                    this.GetCalendarControl("dataUno_TAR").Visible = true;
                    this.GetCalendarControl("dataUno_TAR").btn_Cal.Enabled = false;
                    this.GetCalendarControl("dataUno_TAR").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("dataUno_TAR").txt_Data.Enabled = false;
                    this.GetCalendarControl("dataDue_TAR").Visible = true;
                    this.GetCalendarControl("dataDue_TAR").btn_Cal.Visible = true;
                    this.GetCalendarControl("dataDue_TAR").btn_Cal.Enabled = false;
                    this.GetCalendarControl("dataDue_TAR").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("dataDue_TAR").txt_Data.Visible = true;
                    this.GetCalendarControl("dataDue_TAR").txt_Data.Enabled = false;
                    this.lbl1_TAR.Visible = true;
                    this.lbl2_TAR.Visible = true;
                    break;

                case "Mese Corrente":
                    this.GetCalendarControl("dataUno_TAR").Visible = true;
                    this.GetCalendarControl("dataUno_TAR").btn_Cal.Enabled = false;
                    this.GetCalendarControl("dataUno_TAR").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("dataUno_TAR").txt_Data.Enabled = false;
                    this.GetCalendarControl("dataDue_TAR").Visible = true;
                    this.GetCalendarControl("dataDue_TAR").btn_Cal.Visible = true;
                    this.GetCalendarControl("dataDue_TAR").btn_Cal.Enabled = false;
                    this.GetCalendarControl("dataDue_TAR").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("dataDue_TAR").txt_Data.Visible = true;
                    this.GetCalendarControl("dataDue_TAR").txt_Data.Enabled = false;
                    this.lbl1_TAR.Visible = true;
                    this.lbl2_TAR.Visible = true;
                    break;
            }
        }

        protected void ddl_TAR_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PerformActionSelectTAR();
        }
        #endregion filtriRicerca_TAR - Tutte Accettate Rifiutate

        #region ABILITAZIONE RICERCA PROTOCOLLAZIONE INTERNA
        protected void abilitaRicercaProtInterno()
        {
            DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            if (!ws.IsInternalProtocolEnabled(cr.idAmministrazione))
            {
                this.ddl_tipo_doc.Items.Remove(this.ddl_tipo_doc.Items[4]);
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
            DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);
            this.etichette = wws.getEtichetteDocumenti(infoUtente, idAmm);
            this.opArr.Text = "Protocollato in " + etichette[0].Etichetta;
            this.opPart.Text = "Protocollato in " + etichette[1].Etichetta;
            this.opInt.Text = "Protocollato in " + etichette[2].Etichetta;
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

        private void btn_CampiPersonalizzati_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            RegisterStartupScript("Apri", "<script>apriPopupAnteprima();</script>");
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
                    DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDgByIdTipoDoc(ddl_tipoDoc_C.SelectedValue, (UserManager.getInfoUtente(this)).idAmministrazione, this);
                    Session.Add("DiagrammaSelezionato", dg);
                    if (ddl_tipoDoc_C.SelectedValue != "" && dg != null)
                    {
                        Panel_StatiDocumento.Visible = true;
                        this.ddl_statiDoc.Visible = true;
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
                }
                //FINE DIAGRAMMI STATO
            }
            //FINE PROFILAZIONE DINAMICA
        }

        private void ddl_statiDoc_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }

        private void ddl_statiFasc_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }

        private void ddl_tipoDoc_C_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Session.Remove("templateRicerca");
            schedaRicerca.RimuoviFiltro(DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.PROFILAZIONE_DINAMICA.ToString());
            attivaProfilazioneDinamica();
   
        }

        private void ddl_oggetto_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Session.Remove("templateRicerca");
            this.ddl_tipoDoc_C.SelectedIndex = 0;
            this.ddl_tipoFasc.SelectedIndex = 0;

            if (this.ddl_statiDoc.Visible == true)
            {
                this.ddl_statiDoc.SelectedIndex = 0;  
            }

            if (this.ddl_statiFasc.Visible == true)
            {
                this.ddl_statiFasc.SelectedIndex = 0;
            }

            if (this.ddl_statiDoc.SelectedIndex>0)
            {
                this.ddl_statiDoc.SelectedIndex = 0;
            }

            if (this.ddl_statiFasc.SelectedIndex > 0)
            {
                this.ddl_statiFasc.SelectedIndex = 0;
            }

            if (this.ddl_oggetto.SelectedValue.Equals("D"))
            {
                //PROFILAZIONE DINAMICA DOCUMENTI
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
                {
                    //verificaCampiPersonalizzati();
                    attivaProfilazioneDinamica();
                    this.ddl_tipoDoc_C.Visible = true;
                }
                else
                {
                    btn_CampiPersonalizzati.Visible = false;
                }
                //FINE PROFILAZIONE DINAMICA

                this.completamento.Visible = true;
                this.ddl_statiFasc.Visible = false;
                this.pnl_ric_fasc_prof.Visible = false;
                this.ddl_tipoFasc.Visible = false;
                this.Panel_StatiFascicolo.Visible = false;
                this.ddl_tipo_doc.Visible = true;
            }
            else
            {
                //PROFILAZIONE DINAMICA FASCICOLI
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                {
                    verificaCampiPersonalizzatiFasc();
                    this.ddl_tipoFasc.Visible = true;
                }
                else
                {
                    img_dettagliProfilazione.Visible = false;
                }
                //FINE PROFILAZIONE DINAMICA FASCICOLI

                this.completamento.Visible = false;
                this.ddl_statiDoc.Visible = false;
                this.Panel_StatiDocumento.Visible = false;
            }

            schedaRicerca.RimuoviFiltro(DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.PROFILAZIONE_DINAMICA.ToString());

        }

        private void ddl_tipoFasc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Session.Remove("templateRicerca");
            Session.Remove("filtroProfDinamica");
            verificaCampiPersonalizzatiFasc();
            CaricaComboStatiFasc();
            //CaricaComboTipologiaFasc();
        }

        private void verificaCampiPersonalizzatiFasc()
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

        private void CaricaComboTipologiaFasc()
        {
            ArrayList listaTipiFasc = new ArrayList(ProfilazioneFascManager.getTipoFascFromRuolo(UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "1", this));
            ListItem item = new ListItem();
            item.Value = "";
            item.Text = "";
            ddl_tipoFasc.Items.Add(item);
            for (int i = 0; i < listaTipiFasc.Count; i++)
            {
                DocsPaWR.Templates templates = (DocsPaWR.Templates)listaTipiFasc[i];
                ListItem item_1 = new ListItem();
                item_1.Value = templates.SYSTEM_ID.ToString();
                item_1.Text = templates.DESCRIZIONE;
                if (templates.IPER_FASC_DOC == "1")
                    ddl_tipoFasc.Items.Insert(1, item_1);
                else
                    ddl_tipoFasc.Items.Add(item_1);
            }
        }

        private void CaricaComboStatiFasc()
        {
            //DIAGRAMMI DI STATO
            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
            {
                if (ddl_tipoFasc.SelectedValue != null && ddl_tipoFasc.SelectedValue != "")
                {
                    //Verifico se esiste un diagramma di stato associato al tipo di fascicolo
                    DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDgByIdTipoFasc(ddl_tipoFasc.SelectedValue, (UserManager.getInfoUtente(this)).idAmministrazione, this);
                    Session.Add("DiagrammaSelezionato", dg);
                    if (dg != null)
                    {
                        Panel_StatiFascicolo.Visible = true;
                        this.ddl_statiFasc.Visible = true;
                        //Inizializzazione comboBox
                        ddl_statiFasc.Items.Clear();
                        ListItem itemEmpty = new ListItem();
                        ddl_statiFasc.Items.Add(itemEmpty);
                        for (int i = 0; i < dg.STATI.Length; i++)
                        {
                            DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato)dg.STATI[i];
                            ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                            ddl_statiFasc.Items.Add(item);
                        }
                    }
                    else
                    {
                        Panel_StatiFascicolo.Visible = false;
                    }
                }
            }
            //FINE DIAGRAMMI STATO
        }

        private void img_dettagliProfilazione_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "profilazioneDinamica", "apriPopupAnteprima();", true);
        }

        private void setCorrispondenteMioRuolo(DocsPAWA.DocsPaWR.Ruolo mioRuolo)
        {
            ViewState["colorItems"] = null;
            DocsPaWR.Corrispondente corr = null;
            UserManager.removeCorrispondentiSelezionati(this);
            corr = UserManager.getCorrispondente(this, mioRuolo.codiceRubrica, false);
            DataSet ds = new DataSet();
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);
            ds = wws.getUtentiInRuoloSottoposto(infoUtente, corr.systemId);
            this.txt1_corr_sott.Text = "";
            this.txt2_corr_sott.Text = "";
            this.hd_systemIdCorrSott.Value = "";
            this.select_sottoposto.Items.Clear();

            UserManager.setCorrispondenteSelezionatoSottopostoNoRubr(this.Page, corr);
            this.txt1_corr_sott.Text = mioRuolo.codiceRubrica;
            this.txt2_corr_sott.Text = UserManager.getDecrizioneCorrispondenteSemplice(corr);
            this.hd_systemIdCorrSott.Value = corr.systemId;

            //NON CI SONO UTENTI NEL RUOLO
            if (ds != null && ds.Tables[0].Rows.Count == 0)
            {
                this.select_sottoposto.Items.Clear();
                this.select_sottoposto.Enabled = false;
            }
            else
            {
                //POPOLA SELECT UTENTI

                this.select_sottoposto.Enabled = true;

               // this.select_sottoposto.Items.Add(new ListItem());
                this.select_sottoposto.Items.Add(new ListItem("<<qualsiasi utente>>", "tutti"));

                int countCol = 1;
                //SOLO PER IL MIO RULO
                if (((mioRuolo.codiceRubrica).ToUpper()).Equals((this.txt1_corr_sott.Text).ToUpper()))
                {
                    this.select_sottoposto.Items.Add(new ListItem("<<gli altri utenti>>", "altri"));
                    countCol = 2;
                }
                else
                {
                    this.chk_me_stesso.Checked = false;
                }

                this.select_sottoposto.DataSource = ds;
                this.select_sottoposto.DataBind();

                List<int> color = new List<int>();

                for (int i = 0; i < this.select_sottoposto.Items.Count - countCol; i++)
                {
                    DataRow row = ds.Tables[0].Rows[i];
                    string dataUtenteRuolo = row["DTA_FINE"].ToString();

                    if (dataUtenteRuolo == null || dataUtenteRuolo.Equals(string.Empty))
                    {
                        this.select_sottoposto.Items[i + countCol].Attributes.Add("style", "color:#333333");
                    }
                    else
                    {
                        this.select_sottoposto.Items[i + countCol].Attributes.Add("style", "color:#990000");
                        color.Add(i + countCol);
                    }
                }
               ViewState["colorItems"] = color;
            }
        }

        private void chk_mio_ruolo_SelectIndexChanged(object sender, System.EventArgs e)
        {
            if (this.chk_mio_ruolo.Checked == false)
            {
                this.select_sottoposto.Enabled = false;
                this.txt1_corr_sott.Text = string.Empty;
                this.txt2_corr_sott.Text = string.Empty;
                this.btn_img_sott_rubr.Visible = false;
                this.select_sottoposto.Items.Clear();
                UserManager.removeCorrispondentiSelezionati(this);
            }
            else
            {
                if (this.select_sottoposto.Items.Count > 0)
                {
                    this.select_sottoposto.Enabled = true;
                }
                this.btn_img_sott_rubr.Visible = true;
            }
        }


        private void chk_firmati_SelectIndexChanged(object sender, System.EventArgs e)
        {
            if (this.chk_firmati.Checked == true)
            {
                this.chk_non_firmati.Checked = false;
            }
        }

        private void chk_non_firmati_SelectIndexChanged(object sender, System.EventArgs e)
        {
            if (this.chk_non_firmati.Checked == true)
            {
                this.chk_firmati.Checked = false;
            }
        }

        private void P_Prot_SelectIndexChanged(object sender, System.EventArgs e)
        {
            if (this.P_Prot.Checked == true)
            {
                this.M_Fasc.Checked = false;
                this.M_si_img.Checked = false;
                this.M_Img.Checked = false;
                this.chk_firmati.Visible = false;
                this.chk_non_firmati.Visible = false;
                this.lbl_panel_con_imm.Visible = false;
            }
        }

        private void M_Fasc_SelectIndexChanged(object sender, System.EventArgs e)
        {
            if (this.M_Fasc.Checked == true)
            {
                this.P_Prot.Checked = false;
                this.M_si_img.Checked = false;
                this.M_Img.Checked = false;
                this.chk_firmati.Visible = false;
                this.chk_non_firmati.Visible = false;
                this.lbl_panel_con_imm.Visible = false;
            }
        }

        private void M_si_img_SelectIndexChanged(object sender, System.EventArgs e)
        {
            if (this.M_si_img.Checked == true)
            {
                this.M_Fasc.Checked = false;
                this.P_Prot.Checked = false;
                this.M_Img.Checked = false;
                this.chk_firmati.Visible = true;
                this.chk_non_firmati.Visible = true;
                this.lbl_panel_con_imm.Visible = true;
            }
            else
            {
                this.chk_firmati.Visible = false;
                this.chk_non_firmati.Visible = false;
                this.lbl_panel_con_imm.Visible = false;
            }
        }

        private void M_Img_SelectIndexChanged(object sender, System.EventArgs e)
        {
            if (this.M_Img.Checked == true)
            {
                this.M_Fasc.Checked = false;
                this.M_si_img.Checked = false;
                this.P_Prot.Checked = false;
                this.chk_firmati.Visible = false;
                this.chk_non_firmati.Visible = false;
                this.lbl_panel_con_imm.Visible = false;
            }
        }

        private void caricaComboTipoFileAcquisiti()
        {
            ArrayList tipoFile = new ArrayList();
            tipoFile = DocumentManager.getExtFileAcquisiti(this);
            bool firmati = false;
            for (int i = 0; i < tipoFile.Count; i++)
            {
                if (!tipoFile[i].ToString().Contains("P7M"))
                {
                    ListItem item = new ListItem(tipoFile[i].ToString());
                    this.ddl_tipoFileAcquisiti.Items.Add(item);
                }
            }
        }

    }
}
