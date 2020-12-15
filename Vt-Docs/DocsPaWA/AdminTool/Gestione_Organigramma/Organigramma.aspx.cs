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
using System.Linq;
using DocsPAWA.DocsPaWR;
using DocsPAWA.AdminTool.Manager;
using DocsPAWA.SiteNavigation;
using DocsPAWA.utils;
using Amministrazione.Manager;
using DocsPAWA.AdminTool.Gestione_Organigramma;
using DocsPAWA;
using System.Configuration;
using System.Collections.Generic;

namespace Amministrazione.Gestione_Organigramma
{
    /// <summary>
    /// Summary description for Organigramma.
    /// </summary>
    public class Organigramma : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.Button btnGenerateSummary;
        //private enum statoRuoloInLibroFirma {NON_IN_LIBRO_FIRMA, IN_LIBRO_FIRMA_DISATTIVO, IN_LIBRO_FIRMA_ATTIVO};

        #region class myTreeNode
        /// <summary>
        /// Summary description for myTreeNode
        /// </summary>
        public class myTreeNode : Microsoft.Web.UI.WebControls.TreeNode
        {
            // Tipo Nodo [Possibili Valori: U=(Unità organizz.), R=(Ruolo), U=(Utente) ]
            public string getTipoNodo()
            {
                return ViewState["TipoNodo"].ToString();
            }
            public void setTipoNodo(string id)
            {
                ViewState["TipoNodo"] = id;
            }

            // IDCorrGlobale
            public string getIDCorrGlobale()
            {
                return ViewState["IDCorrGlobale"].ToString();
            }
            public void setIDCorrGlobale(string id)
            {
                ViewState["IDCorrGlobale"] = id;
            }

            // Codice
            public string getCodice()
            {
                return ViewState["Codice"].ToString();
            }
            public void setCodice(string id)
            {
                ViewState["Codice"] = id;
            }

            // CodiceRubrica
            public string getCodiceRubrica()
            {
                return ViewState["CodiceRubrica"].ToString();
            }
            public void setCodiceRubrica(string id)
            {
                ViewState["CodiceRubrica"] = id;
            }

            // Descrizione
            public string getDescrizione()
            {
                return ViewState["Descrizione"].ToString();
            }
            public void setDescrizione(string id)
            {
                ViewState["Descrizione"] = id;
            }

            // Livello
            public string getLivello()
            {
                return ViewState["Livello"].ToString();
            }
            public void setLivello(string id)
            {
                ViewState["Livello"] = id;
            }

            // Amministrazione
            public string getIDAmministrazione()
            {
                return ViewState["IDAmministrazione"].ToString();
            }
            public void setIDAmministrazione(string id)
            {
                ViewState["IDAmministrazione"] = id;
            }

            // AOO interoperabilità
            public string getCodRegInterop()
            {
                return ViewState["CodRegInterop"].ToString();
            }
            public void setCodRegInterop(string id)
            {
                ViewState["CodRegInterop"] = id;
            }

            // Tipo ruolo
            public string getIDTipoRuolo()
            {
                return ViewState["IDTipoRuolo"].ToString();
            }
            public void setIDTipoRuolo(string id)
            {
                ViewState["IDTipoRuolo"] = id;
            }

            // ID Groups
            public string getIDGruppo()
            {
                return ViewState["IDGruppo"].ToString();
            }
            public void setIDGruppo(string id)
            {
                ViewState["IDGruppo"] = id;
            }

            // ID People
            public string getIDPeople()
            {
                return ViewState["IDPeople"].ToString();
            }
            public void setIDPeople(string id)
            {
                ViewState["IDPeople"] = id;
            }

            // Ruolo Di riferimento
            public string getDiRiferimento()
            {
                return ViewState["DiRiferimento"].ToString();
            }
            public void setDiRiferimento(string id)
            {
                ViewState["DiRiferimento"] = id;
            }

            // Percorso
            public string getPercorso()
            {
                return ViewState["Percorso"].ToString();
            }
            public void setPercorso(string id)
            {
                ViewState["Percorso"] = id;
            }

            //Ruolo Responsabile
            public string getResponsabile()
            {
                return ViewState["Responsabile"].ToString();
            }
            public void setResponsabile(string id)
            {
                ViewState["Responsabile"] = id;
            }

            public string getSegretario()
            {
                return ViewState["Segretario"].ToString();
            }
            public void setSegretario(string id)
            {
                ViewState["Segretario"] = id;
            }

            public string getDisabledTrasm()
            {
                return ViewState["DisabledTrasm"].ToString();
            }
            public void setDisabledTrasm(string id)
            {
                ViewState["DisabledTrasm"] = id;
            }

            /// <summary>
            /// Id del registro per l'interoperabilità semplificata
            /// </summary>
            public String RegistryInteropSemplificata
            {
                get
                {
                    if (ViewState["RegistryInteropSemplificata"] != null)
                        return ViewState["RegistryInteropSemplificata"].ToString();
                    else
                        return String.Empty;
                }
                set
                {
                    ViewState["RegistryInteropSemplificata"] = value;
                }

            }

            /// <summary>
            /// Id dell'RF per l'interoperabilità semplificata
            /// </summary>
            public String RfInteropSemplificata
            {
                get
                {
                    if (ViewState["RfInteropSemplificata"] != null)
                        return ViewState["RfInteropSemplificata"].ToString();
                    else
                        return String.Empty;
                }
                set
                {
                    ViewState["RfInteropSemplificata"] = value;
                }

            }
        }

        #endregion

        #region WebControls e variabili

        protected System.Web.UI.WebControls.Label lbl_avviso;
        protected System.Web.UI.WebControls.Panel pnl_tv;
        protected Microsoft.Web.UI.WebControls.TreeView treeViewUO;
        protected System.Web.UI.WebControls.DropDownList ddl_aoo;
        protected System.Web.UI.WebControls.Panel pnl_uo;
        protected System.Web.UI.WebControls.Panel pnl_ruoli;
        protected System.Web.UI.WebControls.Panel pnl_funzioni;
        protected System.Web.UI.WebControls.Panel pnl_registri;
        protected System.Web.UI.WebControls.CheckBox chk_rif;
        protected System.Web.UI.WebControls.CheckBox cb_disabled_trasm;
        protected System.Web.UI.WebControls.DataGrid dg_funzioni;
        protected System.Web.UI.WebControls.DataGrid dg_registri;
        protected System.Web.UI.WebControls.DataGrid dg_rf;
        protected System.Web.UI.WebControls.Button btn_elimina_uo;
        protected System.Web.UI.WebControls.Button btn_ins_sotto_uo;
        protected System.Web.UI.WebControls.TextBox txt_rubricaUO;
        protected System.Web.UI.WebControls.TextBox txt_descrizioneUO;
        protected System.Web.UI.WebControls.CheckBox chk_interopUO;
        protected System.Web.UI.WebControls.TextBox txt_descrizioneRuolo;
        protected System.Web.UI.WebControls.TextBox txt_rubricaRuolo;
        protected System.Web.UI.WebControls.Label lbl_position;
        protected System.Web.UI.WebControls.Button btn_elimina_ruolo;
        protected System.Web.UI.WebControls.Button btn_sposta_ruolo;
        protected System.Web.UI.WebControls.Button btn_utenti_ruolo;
        protected System.Web.UI.WebControls.Button btn_mod_ruolo;
        protected System.Web.UI.WebControls.Button btn_mod_funzioni;
        protected System.Web.UI.WebControls.Button btn_mod_registri;
        protected System.Web.UI.WebControls.Label lbl_percorso;
        protected System.Web.UI.WebControls.Panel pnl_utente;
        protected System.Web.UI.WebControls.Button btn_mod_utente;
        protected System.Web.UI.WebControls.TextBox txt_userIdUtente;
        protected System.Web.UI.WebControls.TextBox txt_cognomeUtente;
        protected System.Web.UI.WebControls.TextBox txt_nomeUtente;
        protected System.Web.UI.WebControls.TextBox txt_rubicaUtente;
        protected System.Web.UI.WebControls.TextBox txt_emailUtente;
        protected System.Web.UI.WebControls.TextBox txt_sedeUtente;
        protected System.Web.UI.WebControls.DropDownList ddl_notifica;
        protected System.Web.UI.WebControls.CheckBox cb_abilitato;
        protected System.Web.UI.WebControls.CheckBox cb_amm;
        protected System.Web.UI.WebControls.TextBox txt_dominioUtente;
        protected System.Web.UI.WebControls.Button btn_mod_uo;
        protected System.Web.UI.WebControls.Button btn_ins_ruolo_uo;
        protected System.Web.UI.WebControls.DropDownList ddl_tipo_ruolo;
        protected System.Web.UI.WebControls.DropDownList ddl_ricTipo;
        protected System.Web.UI.WebControls.TextBox txt_ricCod;
        protected System.Web.UI.WebControls.TextBox txt_ricDesc;
        protected System.Web.UI.WebControls.Button btn_find;
        protected System.Web.UI.HtmlControls.HtmlTableCell td_descRicerca;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModal;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_DisableRole;
        protected System.Web.UI.WebControls.Button btn_vis_reg;
        protected DataSet dsFunzioni;
        protected System.Web.UI.WebControls.TextBox txt_uo_cap;
        protected System.Web.UI.WebControls.TextBox txt_uo_citta;
        protected System.Web.UI.WebControls.TextBox txt_uo_prov;
        protected System.Web.UI.WebControls.TextBox txt_uo_telefono1;
        protected System.Web.UI.WebControls.TextBox txt_uo_telefono2;
        protected System.Web.UI.WebControls.TextBox txt_uo_fax;
        protected System.Web.UI.WebControls.TextBox txt_uo_nazione;
        protected System.Web.UI.WebControls.TextBox txt_uo_indirizzo;
        protected System.Web.UI.WebControls.TextBox txt_uo_note;
        protected System.Web.UI.WebControls.TextBox txt_uo_codice_fiscale;
        protected System.Web.UI.WebControls.TextBox txt_uo_partita_iva;

        protected System.Web.UI.WebControls.Button btn_sposta_uo;
        protected System.Web.UI.WebControls.Button btn_sposta_utente;
        protected System.Web.UI.WebControls.Button btn_gest_qual;
        protected System.Web.UI.WebControls.Button btn_gest_app;
        protected System.Web.UI.WebControls.Panel pnlRF;
        protected DataSet dsRegistri;
        protected DataSet dsRF;
        protected System.Web.UI.WebControls.CheckBox chk_resp;
        protected System.Web.UI.WebControls.Button btn_visib_titolario;

        protected System.Web.UI.WebControls.Button btn_clear_uo;
        protected System.Web.UI.WebControls.Button btn_annulla_ins_uo;
        protected System.Web.UI.WebControls.Button btn_ordinamento;
        protected System.Web.UI.WebControls.Panel pnl_ordinamento;
        protected System.Web.UI.WebControls.Panel pnlRubricaComune;
        protected System.Web.UI.WebControls.Panel pnlContenitore;
        protected System.Web.UI.WebControls.Panel pnlReg;
        protected System.Web.UI.WebControls.Label lblCodiceRC;
        protected System.Web.UI.WebControls.Label lblDescrizioneRC;
        protected System.Web.UI.WebControls.Label lblDataCreazioneRC;
        protected System.Web.UI.WebControls.Label lblDataUltimaModificaRC;
        protected System.Web.UI.WebControls.Button btn_invia_rc;
        protected System.Web.UI.WebControls.CheckBox cb_segretario;
        protected System.Web.UI.WebControls.Button btn_importaOrganigramma;
        protected bool noStoricizzUO = false;
        protected System.Web.UI.WebControls.Button btn_storicizza;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txt_confirmSto;

        protected System.Web.UI.HtmlControls.HtmlTableRow trCodUO, trStoricizzaAndUpdateModels;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hdReturnValueSelectedRicUO;
        protected System.Web.UI.WebControls.CheckBox chkStoricizza, chkUpdateModels;
        protected System.Web.UI.WebControls.TextBox txtCodUo, txtDescUo;
        protected System.Web.UI.WebControls.ImageButton btnOrg;
        protected System.Web.UI.WebControls.HiddenField hdIdCorrGlob;

        // Bottone per mostrare la storia del ruolo
        protected System.Web.UI.HtmlControls.HtmlButton btnShowRoleHistory;

        // Checkbox per ricercare ruoli storicizzati
        protected System.Web.UI.WebControls.CheckBox chkSearchHistoricized;

        protected System.Web.UI.WebControls.HiddenField hfRetValModSposta;

        protected System.Web.UI.WebControls.Button btn_copiaVisRuolo;

        protected System.Web.UI.HtmlControls.HtmlTableRow trExtendVisibility;
        protected System.Web.UI.WebControls.RadioButtonList rblExtendVisibility;

        protected System.Web.UI.HtmlControls.HtmlTableRow trCalculateAtipicita;
        protected DocsPAWA.AdminTool.UserControl.CalculateAtipicitaOptions calculateAtipicitaOptions;
        protected System.Web.UI.WebControls.CheckBox cbx_Sel;

        // Checkbox e Menù a tendina per la specifica dell'interoperabilità semplificata
        protected System.Web.UI.WebControls.CheckBox chkInteroperante;
        protected System.Web.UI.WebControls.DropDownList ddlRegistriInteropSemplificata, ddlRfInteropSemplificata;
        protected System.Web.UI.HtmlControls.HtmlTableRow trInteropSemplificata;
        protected System.Web.UI.WebControls.Label lblInteroperanteIS;
        protected System.Web.UI.WebControls.Button btn_processiFirmaUtente;
        protected System.Web.UI.WebControls.Button btn_processiFirmaRuolo;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModalLFRuolo;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModalLFUtente;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_TipoOperazione;

        private InvalidaPassiCorrelatiDelegate invalidaPassiCorrelati;
        private StoricizzaPassiCorrelatiDelegate storicizzaPassiCorrelati;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_ReturnValueProcessiFirmaRegistroRF;

        #endregion

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
            this.ddl_ricTipo.SelectedIndexChanged += new System.EventHandler(this.ddl_ricTipo_SelectedIndexChanged);
            this.treeViewUO.Expand += new Microsoft.Web.UI.WebControls.ClickEventHandler(this.treeViewUO_Expand);
            this.treeViewUO.SelectedIndexChange += new Microsoft.Web.UI.WebControls.SelectEventHandler(this.treeViewUO_SelectedIndexChange);
            this.treeViewUO.Collapse += new Microsoft.Web.UI.WebControls.ClickEventHandler(this.treeViewUO_Collapse);
            this.chk_interopUO.CheckedChanged += new System.EventHandler(this.chk_interopUO_CheckedChanged);
            this.btn_elimina_uo.Click += new System.EventHandler(this.btn_elimina_uo_Click);
            this.btn_sposta_uo.Click += new System.EventHandler(this.btn_sposta_uo_Click);
            this.btn_ins_ruolo_uo.Click += new System.EventHandler(this.btn_ins_ruolo_uo_Click);
            this.btn_ins_sotto_uo.Click += new System.EventHandler(this.btn_ins_sotto_uo_Click);
            this.btn_mod_uo.Click += new System.EventHandler(this.btn_mod_uo_Click);
            this.ddl_tipo_ruolo.SelectedIndexChanged += new System.EventHandler(this.ddl_tipo_ruolo_SelectedIndexChanged);
            this.btn_elimina_ruolo.Click += new System.EventHandler(this.btn_elimina_ruolo_Click);
            this.btn_sposta_ruolo.Click += new System.EventHandler(this.btn_sposta_ruolo_Click);
            this.btn_utenti_ruolo.Click += new System.EventHandler(this.btn_utenti_ruolo_Click);
            this.btn_mod_ruolo.Click += new System.EventHandler(this.btn_mod_ruolo_Click);
            this.btn_mod_registri.Click += new System.EventHandler(this.btn_mod_registri_Click);
            this.btn_mod_funzioni.Click += new System.EventHandler(this.btn_mod_funzioni_Click);
            this.btn_sposta_utente.Click += new System.EventHandler(this.btn_sposta_utente_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.dg_rf.PreRender += new System.EventHandler(this.dg_rf_PreRender);
            this.btn_clear_uo.Click += new System.EventHandler(this.btn_clear_uo_Click);
            this.btn_annulla_ins_uo.Click += new System.EventHandler(this.btn_annulla_ins_uo_Click);
            this.btn_ordinamento.Click += new System.EventHandler(this.btn_ordinamento_Click);
            this.dg_registri.PreRender += new EventHandler(dg_registri_PreRender);
            this.btn_importaOrganigramma.Click += new System.EventHandler(this.btn_ImportaOrganigramma_Click);
            this.ddl_aoo.SelectedIndexChanged += new EventHandler(ddl_aoo_SelectedIndexChanged);
            this.btn_storicizza.Click += new EventHandler(btn_storicizza_Click);
            this.txtCodUo.TextChanged += new EventHandler(txtCodUo_TextChanged);
            this.dg_rf.ItemDataBound += new DataGridItemEventHandler(dg_rf_ItemDataBound);
            this.dg_registri.ItemDataBound += new DataGridItemEventHandler(dg_registri_ItemDataBound);
            this.txt_rubricaUO.TextChanged += new EventHandler(txt_rubricaUO_TextChanged);
        }

        void txt_rubricaUO_TextChanged(object sender, EventArgs e)
        {
            this.btn_ordinamento.Enabled = false;
            this.btn_annulla_ins_uo.Enabled = false;
            this.btn_clear_uo.Enabled = false;
            this.btn_elimina_uo.Enabled = false;
            this.btn_importaOrganigramma.Enabled = false;
            this.btn_sposta_uo.Enabled = false;
            this.btn_ins_ruolo_uo.Enabled = false;
            this.btn_ins_sotto_uo.Enabled = false;
            this.btn_mod_uo.Enabled = true;
            this.btn_storicizza.Enabled = true;
            Session["cambiaCodUO"] = true;
        }

        void btn_storicizza_Click(object sender, EventArgs e)
        {
            if (((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_confirmSto")).Value == "si")
            {
                this.ModificaUO(true);
                if (Session["cambiaCodUO"] != null)
                {
                    Session.Remove("cambiaCodUO");
                    this.btn_ordinamento.Enabled = true;
                    this.btn_annulla_ins_uo.Enabled = true;
                    this.btn_clear_uo.Enabled = true;
                    this.btn_elimina_uo.Enabled = true;
                    this.btn_importaOrganigramma.Enabled = true;
                    this.btn_sposta_uo.Enabled = true;
                    this.btn_ins_ruolo_uo.Enabled = true;
                    this.btn_ins_sotto_uo.Enabled = true;
                }
            }
        }

        void ddl_aoo_SelectedIndexChanged(object sender, EventArgs e)
        {
            int oldDDLAOO = 0;
            if (Session["oldDDL_AOO"] != null)
                oldDDLAOO = (int)Session["oldDDL_AOO"];
            else
            {
                if (Session["rememberOldDDL_AOO"] != null)
                    oldDDLAOO = (int)Session["rememberOldDDL_AOO"];
            }
            
            if (this.ddl_aoo.Enabled && this.ddl_aoo.SelectedIndex != 0 && this.ddl_aoo.SelectedIndex != oldDDLAOO)
                Session.Add("interopUOChanged", true);
        }

        #endregion

        #region Gestione soluzione codice (copiato dalla pagina di spostamento ruolo)

        private void txtCodUo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                myTreeNode treeNode;
                treeNode = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
                treeNode.setTipoNodo("R");

                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();

                theManager.RicercaInOrg("U", this.txtCodUo.Text.Trim(), "", treeNode.getIDAmministrazione(), false, true);

                if (theManager.getRisultatoRicerca() != null && theManager.getRisultatoRicerca().Count.Equals(1))
                {
                    foreach (DocsPAWA.DocsPaWR.OrgRisultatoRicerca risultato in theManager.getRisultatoRicerca())
                    {
                        this.txtCodUo.Text = risultato.Codice;
                        this.txtDescUo.Text = risultato.Descrizione;
                        this.hdIdCorrGlob.Value = risultato.IDCorrGlob;
                    }

                    // Se la UO selezionata è quella attuale, viene disabilitato il pulsante Sposta
                    if (this.hdIdCorrGlob.Value == ((myTreeNode)treeNode.Parent).getIDCorrGlobale())
                    {
                        this.btn_sposta_ruolo.Enabled = false;
                        this.btn_mod_ruolo.Enabled = true;
                        this.trExtendVisibility.Visible = false;
                        this.rblExtendVisibility.SelectedIndex = 0;
                        this.trCalculateAtipicita.Visible = false;
                        this.EnableModifyControls(true, treeNode);
                    }
                    else
                    {
                        this.EnableModifyControls(false, treeNode);
                        this.trCalculateAtipicita.Visible = Utils.GetAbilitazioneAtipicita(); ;
                        this.btn_sposta_ruolo.Enabled = true;
                        this.btn_mod_ruolo.Enabled = false;
                        this.trExtendVisibility.Visible = Utils.GetAbilitazioneAtipicita();
                        this.rblExtendVisibility.SelectedIndex = 1;
                    }
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Nessuna UO trovata');", true);
                    this.txtCodUo.Text = String.Empty;
                }

            }
            catch
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Si è verificato un errore durante la ricerca');", true);
            }
        }

        #endregion

        #region Page Load
        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.cb_disabled_trasm.Attributes.Add("onClick", "disabledCb('" + cb_disabled_trasm.ClientID + "', '" + chk_rif.ClientID + "');");
                this.chk_rif.Attributes.Add("onClick", "disabledCb('" + chk_rif.ClientID + "', '" + cb_disabled_trasm.ClientID + "');");
                this.chkInteroperante.Attributes.Add("onClick", String.Format("manageDropDownListRegistries('{0}');", chkInteroperante.ClientID));
                DocsPAWA.AdminTool.Manager.AmministrazioneManager amm = new DocsPAWA.AdminTool.Manager.AmministrazioneManager();
                this.lblInteroperanteIS.Text = "Interoperante " + amm.GetLabelTipoDocumento(DocsPAWA.utils.InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId);
                if (Session["cambiaCodUO"] != null)
                {
                    Session.Remove("cambiaCodUO");
                    this.btn_ordinamento.Enabled = true;
                    this.btn_annulla_ins_uo.Enabled = true;
                    this.btn_clear_uo.Enabled = true;
                    this.btn_elimina_uo.Enabled = true;
                    this.btn_importaOrganigramma.Enabled = true;
                    this.btn_sposta_uo.Enabled = true;
                    this.btn_ins_ruolo_uo.Enabled = true;
                    this.btn_ins_sotto_uo.Enabled = true;
                }
                RemoveSessionInterrompiProcessi();
                RemoveSessionStoricizzaProcessi();

                //processiCoinvolti_R = (Session["processiCoinvolti_R"] != null?(List<ProcessoFirma>)Session["processiCoinvolti_R"]:new List<ProcessoFirma>());
                //istazaProcessiCoinvolti_R = (Session["istazaProcessiCoinvolti_R"] != null ? (List<IstanzaProcessoDiFirma>)Session["istazaProcessiCoinvolti_R"] : new List<IstanzaProcessoDiFirma>());
                //processiCoinvolti_U = (Session["processiCoinvolti_U"] != null ? (List<ProcessoFirma>)Session["processiCoinvolti_U"] : new List<ProcessoFirma>());
                //istazaProcessiCoinvolti_U = (Session["istazaProcessiCoinvolti_U"] != null ? (List<IstanzaProcessoDiFirma>)Session["istazaProcessiCoinvolti_U"] : new List<IstanzaProcessoDiFirma>());
                //Session["InfoUtenteAmministratore"] = getIfoUtenteAmministratore();
            }
            else
            {
                if (this.hd_returnValueModalLFRuolo.Value != null && this.hd_returnValueModalLFRuolo.Value != string.Empty && this.hd_returnValueModalLFRuolo.Value != "undefined")
                {
                    this.GestRitornoAvvisoLFRuolo(this.hd_returnValueModalLFRuolo.Value);
                    return;
                }
                if (this.hd_returnValueModalLFUtente.Value != null && this.hd_returnValueModalLFUtente.Value != string.Empty && this.hd_returnValueModalLFUtente.Value != "undefined")
                {
                    this.GestRitornoAvvisoLFUtente(this.hd_returnValueModalLFUtente.Value);
                    return;
                }
            }

            // Le opzioni del calcolo atipicità devono essere visualizzate solo se è attiva
            this.calculateAtipicitaOptions.Visible = Utils.GetAbilitazioneAtipicita();
            
            // Se il campo nascosto hfRetValModSposta è valorizzato, bisogna refreshare
            // il contenuto del tree view
            if (!String.IsNullOrEmpty(this.hfRetValModSposta.Value) && this.hfRetValModSposta.Value != "undefined")
            {
                try
                {
                    this.RicercaNodo(this.hfRetValModSposta.Value);
                }
                catch (Exception ex)
                {
                    this.Inizialize();
    
                }

                this.hfRetValModSposta.Value = String.Empty;
            }

            #region Elaborazione codice corrispondente UO (copiato ed incollato dalla maschera di spostamento Ruolo)

            if (!String.IsNullOrEmpty(this.hdReturnValueSelectedRicUO.Value)
                && this.hdReturnValueSelectedRicUO.Value != "undefined")
            {
                string[] appo = this.hdReturnValueSelectedRicUO.Value.Split('|');

                this.txtCodUo.Text = appo[0];
                this.txtDescUo.Text = appo[1];
                this.hdIdCorrGlob.Value = appo[2];

                this.hdReturnValueSelectedRicUO.Value = string.Empty;

                // Se la UO selezionata è quella attuale, viene disabilitato il pulsante Sposta
                myTreeNode treeNode;
                treeNode = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex).Parent;
                treeNode.setTipoNodo("R");

                if (this.hdIdCorrGlob.Value == (treeNode).getIDCorrGlobale())
                {
                    this.EnableModifyControls(true, (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex));
                    this.btn_sposta_ruolo.Enabled = false;
                    this.btn_mod_ruolo.Enabled = true;
                    this.trExtendVisibility.Visible = false;
                    this.rblExtendVisibility.SelectedIndex = 0;
                    this.trCalculateAtipicita.Visible = false;
                    
                }
                else
                {
                    this.EnableModifyControls(false, (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex));
                    this.btn_sposta_ruolo.Enabled = true;
                    this.btn_mod_ruolo.Enabled = false;
                    this.trExtendVisibility.Visible = Utils.GetAbilitazioneAtipicita();
                    this.rblExtendVisibility.SelectedIndex = 0;
                    this.trCalculateAtipicita.Visible = Utils.GetAbilitazioneAtipicita();
                }
            }

            if (!String.IsNullOrEmpty(this.hd_ReturnValueProcessiFirmaRegistroRF.Value)
                && this.hd_ReturnValueProcessiFirmaRegistroRF.Value != "undefined")
            {
                this.hd_ReturnValueProcessiFirmaRegistroRF.Value = string.Empty;
                this.InvalidaProcessiFirmaRegistriCoinvolti();
                this.InserimentoRegistri();
            }
                #endregion

                try
            {
                Session["AdminBookmark"] = "Organigramma";

                //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
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

                this.btn_storicizza.Attributes.Add("onclick", "confirmStoricizza();");

                if (!IsPostBack)
                {
                    try
                    {
                        lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
                        this.Inizialize();
                    }
                    catch(Exception ex)
                    {
                        this.lbl_avviso.Text = ex.StackTrace;
                        this.GUI("ResetAll");
                    }
                }
                else
                {
                    try
                    {
                        // gestione del valore di ritorno della modal Dialog (ricerca)
                        if (this.hd_returnValueModal.Value != null && this.hd_returnValueModal.Value != string.Empty && this.hd_returnValueModal.Value != "undefined")
                        {
                            this.RicercaNodo(this.hd_returnValueModal.Value);
                        }

                        if (Request.QueryString["indice"] != null)
                        {
                            this.treeViewUO.SelectedNodeIndex = Request.QueryString["indice"].ToString();
                            myTreeNode TreeNodo;
                            TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(this.treeViewUO.SelectedNodeIndex);
                            this.GestioneLoadDettagli(TreeNodo);
                        }
                    }
                    catch(Exception ex)
                    {
                        this.lbl_avviso.Text = ex.ToString();
                        this.GUI("ResetAll");
                    }
                }

                //Verifico se provengo da un import
                if (Session["importOrganigramma"] != null)
                {
                    this.Inizialize();
                    Session.Remove("importOrganigramma");
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        private InfoUtente getIfoUtenteAmministratore()
        {
            InfoUtente userInfo = null;

            SessionManager sm = new SessionManager();
            userInfo = sm.getUserAmmSession();
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            userInfo.idAmministrazione = new DocsPaWebService().getIdAmmByCod(codiceAmministrazione);

            return userInfo;
        }

        private void Inizialize()
        {
            try
            {
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();

                if (idAmm != null && idAmm != string.Empty)
                {
                    theManager.ListaUOLivelloZero(idAmm);

                    if (theManager.getListaUO() != null && theManager.getListaUO().Count > 0)
                    {
                        //this.btn_find.Attributes.Add("onclick", "ApriRisultRic('" + idAmm + "');");
                        this.btn_find.OnClientClick = "javascript:ApriRisultRic('" + idAmm + "');";
                        this.LoadTreeviewLivelloZero(theManager.getListaUO());
                        this.SetFocus(this.txt_ricDesc);
                    }
                    else
                    {
                        this.GUI("InsPrimaUO");
                        this.GraficaNuovaUO(false);
                    }
                }
                else
                {
                    this.lbl_avviso.Text = "Attenzione! l'amministrazione corrente non risulta essere presente nel database.<br><br>Effettuare il Chiudi e trasmetti.";
                    this.GUI("ResetAll");
                }


            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        #endregion

        #region Treeview

        /// <summary>
        /// Load Treeview Livello Zero
        /// </summary>
        /// <param name="listaUO"></param>
        private void LoadTreeviewLivelloZero(ArrayList listaUO)
        {
            try
            {
                this.PulisceTuttaTreeView();

                Microsoft.Web.UI.WebControls.TreeNode treenode = new Microsoft.Web.UI.WebControls.TreeNode();

                //treenode.Text = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"1");	
                treenode.Text = "Organigramma";

                treeViewUO.Nodes.Add(treenode);

                Microsoft.Web.UI.WebControls.TreeNode tNode = new Microsoft.Web.UI.WebControls.TreeNode();
                tNode = treeViewUO.Nodes[0];

                myTreeNode nodoT;
                myTreeNode nodoFiglio;

                foreach (DocsPAWA.DocsPaWR.OrgUO uo in listaUO)
                {
                    nodoT = new myTreeNode();

                    nodoT.ID = uo.IDCorrGlobale;
                    nodoT.Text = uo.CodiceRubrica + " - " + uo.Descrizione;
                    nodoT.ImageUrl = "../Images/uo.gif";

                    tNode.Nodes.Add(nodoT);

                    nodoT.setTipoNodo("U");
                    nodoT.setIDCorrGlobale(uo.IDCorrGlobale);
                    nodoT.setCodice(uo.Codice);
                    nodoT.setCodiceRubrica(uo.CodiceRubrica);
                    nodoT.setDescrizione(uo.Descrizione);
                    nodoT.setLivello(uo.Livello);
                    nodoT.setIDAmministrazione(uo.IDAmministrazione);
                    nodoT.setCodRegInterop(uo.CodiceRegistroInterop);
                    //nodoT.setPercorso("<a href=\"javascript:void(0)\" onclick=\"Ricarica('"+nodoT.GetNodeIndex()+"');\">" + uo.Descrizione + "</a> &gt; ");
                    nodoT.setPercorso(uo.Descrizione + " &gt; ");
                    nodoT.RegistryInteropSemplificata = uo.IdRegistroInteroperabilitaSemplificata;
                    nodoT.RfInteropSemplificata = uo.IdRfInteroperabilitaSemplificata;


                    if ((!uo.Ruoli.Equals("0")) || (!uo.SottoUo.Equals("0")))
                    {
                        nodoFiglio = new myTreeNode();
                        nodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
                        nodoT.Nodes.Add(nodoFiglio);
                    }
                    else
                    {
                        nodoT.Text = uo.CodiceRubrica + " - " + uo.Descrizione;
                    }
                }

                tNode.Expanded = true;
                this.SelezionaPrimo();
                this.LoadTreeViewLivelloFigli("0.0", "U");
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// Load TreeView Livello Figli
        /// </summary>
        /// <param name="indice"></param>
        /// <param name="tipoNodo"></param>
        private void LoadTreeViewLivelloFigli(string indice, string tipoNodo)
        {
            try
            {
                treeViewUO.SelectedNodeIndex = indice;

                myTreeNode TreeNodo;
                TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(indice);
                TreeNodo.Expanded = true;

                if (TreeNodo.Nodes.Count > 0)
                    TreeNodo.Nodes.RemoveAt(0);

                myTreeNode nodoRuoli;
                myTreeNode nodoUtenti;
                myTreeNode nodoUO;
                myTreeNode nodoFiglio;

                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.ListaRuoliUO(TreeNodo.getIDCorrGlobale());

                ArrayList lista = new ArrayList();
                lista = theManager.getListaRuoliUO();

                // ... ruoli
                if (lista != null && lista.Count > 0)
                {
                    foreach (DocsPAWA.DocsPaWR.OrgRuolo ruolo in lista)
                    {
                        nodoRuoli = new myTreeNode();

                        //Modifica per visualizzare il codice del tipo ruolo
                        theManager.ListaTipiRuolo(ruolo.IDAmministrazione);
                        DocsPAWA.DocsPaWR.OrgTipoRuolo[] elencoTipiRuolo = (DocsPAWA.DocsPaWR.OrgTipoRuolo[])theManager.getListaTipiRuolo().ToArray(typeof(DocsPAWA.DocsPaWR.OrgTipoRuolo));
                        DocsPAWA.DocsPaWR.OrgTipoRuolo tipoRuolo = elencoTipiRuolo.Where(oggetto => oggetto.IDTipoRuolo.ToUpper().Equals(ruolo.IDTipoRuolo.ToUpper())).FirstOrDefault();

                        nodoRuoli.Text = ruolo.CodiceRubrica + "  - [" + tipoRuolo.Livello + "] - " + ruolo.Descrizione;
                        //Fine modifica per visualizzare il codice del tipo ruolo

                        nodoRuoli.ID = ruolo.IDCorrGlobale;
                        nodoRuoli.ImageUrl = "../Images/ruolo.gif";

                        TreeNodo.Nodes.Add(nodoRuoli);

                        nodoRuoli.setTipoNodo("R");
                        nodoRuoli.setIDCorrGlobale(ruolo.IDCorrGlobale);
                        nodoRuoli.setIDTipoRuolo(ruolo.IDTipoRuolo);
                        nodoRuoli.setIDGruppo(ruolo.IDGruppo);
                        nodoRuoli.setCodice(ruolo.Codice);
                        nodoRuoli.setCodiceRubrica(ruolo.CodiceRubrica);
                        nodoRuoli.setDescrizione(ruolo.Descrizione);
                        nodoRuoli.setDiRiferimento(ruolo.DiRiferimento);
                        nodoRuoli.setIDAmministrazione(ruolo.IDAmministrazione);
                        //nodoRuoli.setPercorso(TreeNodo.getPercorso() + "<a href=\"javascript:void(0)\" onclick=\"Ricarica('"+nodoRuoli.GetNodeIndex()+"');\">" + ruolo.Descrizione + "</a> &gt; ");
                        nodoRuoli.setPercorso(TreeNodo.getPercorso() + ruolo.Descrizione + " &gt; ");
                        nodoRuoli.setResponsabile(ruolo.Responsabile);
                        nodoRuoli.setSegretario(ruolo.Segretario);
                        nodoRuoli.setDisabledTrasm(ruolo.DisabledTrasm);
                        // ... utenti
                        if (ruolo.Utenti.Length > 0)
                        {
                            foreach (DocsPAWA.DocsPaWR.OrgUtente utente in ruolo.Utenti)
                            {
                                nodoUtenti = new myTreeNode();

                                nodoUtenti.ID = utente.IDCorrGlobale;
                                nodoUtenti.Text = utente.CodiceRubrica + " - " + utente.Cognome + " " + utente.Nome;
                                nodoUtenti.ImageUrl = "../Images/utente.gif";

                                nodoRuoli.Nodes.Add(nodoUtenti);

                                nodoUtenti.setTipoNodo("P");
                                nodoUtenti.setIDCorrGlobale(utente.IDCorrGlobale);
                                nodoUtenti.setIDPeople(utente.IDPeople);
                                nodoUtenti.setCodice(utente.Codice);
                                nodoUtenti.setCodiceRubrica(utente.CodiceRubrica);
                                nodoUtenti.setIDAmministrazione(utente.IDAmministrazione);
                            }
                        } // fine inserimento utenti	
                        else
                        {
                            nodoRuoli.Text = ruolo.CodiceRubrica + "  - [" + tipoRuolo.Livello + "] - " + ruolo.Descrizione;
                        }
                    } // fine inserimento ruoli 						
                }

                // ... uo sottostanti				
                int livello = Convert.ToInt32(TreeNodo.getLivello()) + 1;

                theManager.ListaUO(TreeNodo.getIDCorrGlobale(), livello.ToString(), TreeNodo.getIDAmministrazione());
                lista = theManager.getListaUO();

                if (lista != null && lista.Count > 0)
                {
                    foreach (DocsPAWA.DocsPaWR.OrgUO sub_uo in lista)
                    {
                        nodoUO = new myTreeNode();

                        nodoUO.ID = sub_uo.IDCorrGlobale;
                        nodoUO.Text = sub_uo.CodiceRubrica + " - [" + sub_uo.Livello + "]" + " - " + sub_uo.Descrizione;
                        nodoUO.ImageUrl = "../Images/uo.gif";

                        TreeNodo.Nodes.Add(nodoUO);

                        nodoUO.setTipoNodo("U");
                        nodoUO.setIDCorrGlobale(sub_uo.IDCorrGlobale);
                        nodoUO.setCodice(sub_uo.Codice);
                        nodoUO.setCodiceRubrica(sub_uo.CodiceRubrica);
                        nodoUO.setDescrizione(sub_uo.Descrizione);
                        nodoUO.setLivello(sub_uo.Livello);
                        nodoUO.setIDAmministrazione(sub_uo.IDAmministrazione);
                        nodoUO.setCodRegInterop(sub_uo.CodiceRegistroInterop);
                        //nodoUO.setPercorso(TreeNodo.getPercorso() + "<a href=\"javascript:void(0)\" onclick=\"Ricarica('"+nodoUO.GetNodeIndex()+"');\">" + sub_uo.Descrizione + "</a> &gt; ");
                        nodoUO.setPercorso(TreeNodo.getPercorso() + " [" + sub_uo.Livello + "] " + sub_uo.Descrizione + " &gt; ");
                        nodoUO.RegistryInteropSemplificata = sub_uo.IdRegistroInteroperabilitaSemplificata;
                        nodoUO.RfInteropSemplificata = sub_uo.IdRfInteroperabilitaSemplificata;
                        if ((!sub_uo.Ruoli.Equals("0")) || (!sub_uo.SottoUo.Equals("0")))
                        {
                            nodoFiglio = new myTreeNode();
                            nodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
                            nodoUO.Nodes.Add(nodoFiglio);
                        }
                        else
                        {
                            nodoUO.Text = sub_uo.CodiceRubrica + " - [" + sub_uo.Livello + "]" + " - " + sub_uo.Descrizione;
                        }
                    } // fine inserimento uo sottostanti
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// treeViewUO Expand
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewUO_Expand(object sender, Microsoft.Web.UI.WebControls.TreeViewClickEventArgs e)
        {
            try
            {
                if (e.Node != "0")
                {
                    myTreeNode TreeNodo;
                    TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(e.Node);

                    if (TreeNodo.getTipoNodo().Equals("U"))
                    {
                        if (TreeNodo.Nodes.Count > 0)
                            TreeNodo.Nodes.Clear();

                        this.LoadTreeViewLivelloFigli(e.Node, TreeNodo.getTipoNodo());
                    }

                    this.GestioneLoadDettagli(TreeNodo);

                    treeViewUO.SelectedNodeIndex = e.Node;
                }
                else
                {
                    this.Inizialize();
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// treeViewUO Collapse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewUO_Collapse(object sender, Microsoft.Web.UI.WebControls.TreeViewClickEventArgs e)
        {
            try
            {
                if (e.Node != "0")
                {
                    myTreeNode TreeNodo;
                    TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(e.Node);

                    Microsoft.Web.UI.WebControls.TreeNode nodoFiglio;

                    if (TreeNodo.getTipoNodo().Equals("U"))
                    {
                        if (TreeNodo.Nodes.Count > 0)
                            TreeNodo.Nodes.Clear();

                        nodoFiglio = new Microsoft.Web.UI.WebControls.TreeNode();
                        nodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
                        TreeNodo.Nodes.Add(nodoFiglio);
                    }

                    treeViewUO.SelectedNodeIndex = e.Node;
                }
                else
                {
                    this.GUI("TreeviewLivelloZero");
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// Selezione Treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewUO_SelectedIndexChange(object sender, Microsoft.Web.UI.WebControls.TreeViewSelectEventArgs e)
        {
            try
            {
                if (e.NewNode.Equals("0"))
                {
                    this.lbl_percorso.Text = "";
                    this.GUI("TreeviewLivelloZero");
                }
                else
                {
                    myTreeNode TreeNodo;
                    TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(e.NewNode);
                    if (TreeNodo!=null && !TreeNodo.Expanded && TreeNodo.getTipoNodo() != null && TreeNodo.getTipoNodo().Equals("R"))
                    {
                        //VerificaProcessiCoinvolti(TreeNodo);
                    }
                    this.GestioneLoadDettagli(TreeNodo);
                    this.btn_storicizza.Visible = true;
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// Seleziona Primo nodo
        /// </summary>
        private void SelezionaPrimo()
        {
            try
            {
                treeViewUO.SelectedNodeIndex = "0.0";

                myTreeNode TreeNodo;
                TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex("0.0");

                this.GestioneLoadDettagli(TreeNodo);
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// Collassa Nodi
        /// </summary>
        /// <param name="indice">Indice nodo</param>
        private void CollassaNodi(string indice)
        {
            try
            {
                Microsoft.Web.UI.WebControls.TreeNode Treenodo;
                Treenodo = treeViewUO.GetNodeFromIndex(indice);

                foreach (Microsoft.Web.UI.WebControls.TreeNode nodo in Treenodo.Nodes)
                {
                    nodo.Expanded = false;
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// Pulisce Tutta la TreeView
        /// </summary>
        private void PulisceTuttaTreeView()
        {
            treeViewUO.Nodes.Clear();
        }

        /// <summary>
        /// Aggiorna la treeview con i nuovi dati (modificati)
        /// </summary>		
        private void AggiornaTreeviewDopoMod(string idCorrGlobale)
        {
            try
            {
                myTreeNode TreeNodo;
                TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);

                switch (TreeNodo.getTipoNodo())
                {
                    case "U": // UO
                        TreeNodo.Text = this.txt_rubricaUO.Text + " - " + this.txt_descrizioneUO.Text;
                        if(!string.IsNullOrEmpty(idCorrGlobale))
                            TreeNodo.setIDCorrGlobale(idCorrGlobale);
                        TreeNodo.setCodice(this.txt_rubricaUO.Text);
                        TreeNodo.setCodiceRubrica(this.txt_rubricaUO.Text);
                        TreeNodo.setDescrizione(this.txt_descrizioneUO.Text);
                        if (this.chk_interopUO.Checked)
                        {
                            if (!this.ddl_aoo.SelectedItem.Value.ToString().Equals("null"))
                                TreeNodo.setCodRegInterop(this.ddl_aoo.SelectedItem.Value);
                        }
                        else
                            TreeNodo.setCodRegInterop(string.Empty);

                        if (this.chkInteroperante.Checked && !String.IsNullOrEmpty(this.ddlRegistriInteropSemplificata.SelectedValue) && !String.IsNullOrEmpty(this.ddlRfInteropSemplificata.SelectedValue))
                        {
                            TreeNodo.RegistryInteropSemplificata = this.ddlRegistriInteropSemplificata.SelectedValue;
                            TreeNodo.RfInteropSemplificata = this.ddlRfInteropSemplificata.SelectedValue;
                        }
                        else
                        {
                            TreeNodo.RegistryInteropSemplificata = String.Empty;
                            TreeNodo.RfInteropSemplificata = String.Empty;
                        }

                        this.DettagliUO(TreeNodo);
                        break;

                    case "R": // Ruolo
                        //Modifica per visualizzare il codice del tipo ruolo
                        Manager.OrganigrammaManager theManager = new Manager.OrganigrammaManager();
                        theManager.ListaTipiRuolo(TreeNodo.getIDAmministrazione());
                        DocsPAWA.DocsPaWR.OrgTipoRuolo[] elencoTipiRuolo = (DocsPAWA.DocsPaWR.OrgTipoRuolo[])theManager.getListaTipiRuolo().ToArray(typeof(DocsPAWA.DocsPaWR.OrgTipoRuolo));
                        DocsPAWA.DocsPaWR.OrgTipoRuolo tipoRuolo = elencoTipiRuolo.Where(oggetto => oggetto.IDTipoRuolo.ToUpper().Equals(TreeNodo.getIDTipoRuolo().ToUpper())).FirstOrDefault();

                        TreeNodo.Text = this.txt_rubricaRuolo.Text + "  - [" + tipoRuolo.Livello + "] - " + this.txt_descrizioneRuolo.Text;
                        //nodoRuoli.Text = ruolo.CodiceRubrica + "  - [" + tipoRuolo.Livello + "] - " + ruolo.Descrizione;
                        //Fine modifica per visualizzare il codice del tipo ruolo

                        //TreeNodo.Text = this.txt_rubricaRuolo.Text + " - " + this.txt_descrizioneRuolo.Text;

                        TreeNodo.setCodice(this.txt_rubricaRuolo.Text);
                        TreeNodo.setCodiceRubrica(this.txt_rubricaRuolo.Text);
                        TreeNodo.setDescrizione(this.txt_descrizioneRuolo.Text);
                        if (this.chk_rif.Checked)
                            TreeNodo.setDiRiferimento("1");
                        else
                            TreeNodo.setDiRiferimento("");

                        if (this.chk_resp.Checked)
                            TreeNodo.setResponsabile("1");
                        else
                            TreeNodo.setResponsabile("");

                        if (this.cb_segretario.Checked)
                            TreeNodo.setSegretario("1");
                        else
                            TreeNodo.setSegretario("");

                        if (this.cb_disabled_trasm.Checked)
                            TreeNodo.setDisabledTrasm("1");
                        else
                            TreeNodo.setDisabledTrasm("");

                        if (this.btn_elimina_ruolo.Visible)
                        {
                            string js = "if(document.getElementById('hd_DisableRole').value!='no_disable' && document.getElementById('hd_DisableRole').value!='disable')\n{\n";
                            js += "return window.confirm('Attenzione!\\n\\nil ruolo [" + TreeNodo.getDescrizione().Replace("'", "\\'") + "] sarà eliminato.\\n\\nSei sicuro di voler procedere?');\\n}";
                            this.btn_elimina_ruolo.Attributes.Add("onclick", js);
                        }
                        this.DettagliRuolo(TreeNodo);
                        break;

                    case "P": // Utente
                        this.DettagliUtente(TreeNodo);
                        break;
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// Aggiorna la treeview con i nuovi dati (inseriti)
        /// </summary>		
        private void AggiornaTreeviewDopoIns()
        {
            try
            {
                Microsoft.Web.UI.WebControls.TreeViewClickEventArgs e = new TreeViewClickEventArgs(treeViewUO.SelectedNodeIndex);
                this.treeViewUO_Expand(null, e);
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// Aggiorna Treeview Dopo Cancellazione nodo
        /// </summary>
        private void AggiornaTreeviewDopoCanc(string tipo)
        {
            try
            {
                Microsoft.Web.UI.WebControls.TreeNode TreeNodoDaElim;
                TreeNodoDaElim = treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);

                myTreeNode TreeNodoPadre;

                string indiceNodo = treeViewUO.SelectedNodeIndex;
                int indicePunto = indiceNodo.LastIndexOf(".");
                indiceNodo = indiceNodo.Substring(0, indicePunto);

                this.treeViewUO.SelectedNodeIndex = indiceNodo;
                TreeNodoPadre = (myTreeNode)treeViewUO.GetNodeFromIndex(indiceNodo);

                this.GestioneLoadDettagli(TreeNodoPadre);

                TreeNodoPadre.Nodes.Remove(TreeNodoDaElim);

                if (TreeNodoPadre.Nodes.Count.Equals(0))
                {
                    switch (tipo)
                    {
                        case "U":
                            if (this.treeViewUO.SelectedNodeIndex.Equals("0") || this.treeViewUO.SelectedNodeIndex.Equals("0.0"))
                            {
                                this.btn_elimina_uo.Visible = false;
                                this.btn_sposta_uo.Visible = false;
                            }
                            else
                            {
                                this.btn_elimina_uo.Visible = true;
                                this.btn_sposta_uo.Visible = true;
                            }
                            break;
                        case "R":
                            //this.btn_elimina_ruolo.Visible = true;
                            //this.btn_sposta_ruolo.Visible = true;
                            if (isOrdinamentoAbilitato())
                                this.btn_ordinamento.Visible = false;
                            this.btn_elimina_uo.Visible = true;

                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// Seleziona un dato nodo della Treeview e visualizza i dettagli a sinistra
        /// </summary>
        /// <param name="testoDaRicercare">Testo da ricercare tra i nodi</param>
        private void SelezionaNodoByText(string testoDaRicercare)
        {
            try
            {
                Microsoft.Web.UI.WebControls.TreeNode TreeNodo;
                TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);

                foreach (myTreeNode nodo in TreeNodo.Nodes)
                {
                    if (nodo.Text == testoDaRicercare)
                    {
                        treeViewUO.SelectedNodeIndex = nodo.GetNodeIndex();
                        this.GestioneLoadDettagli(nodo);
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// Restituisce il nodo padre
        /// </summary>
        /// <param name="nodo">nodo (figlio)</param>
        /// <returns></returns>
        private Microsoft.Web.UI.WebControls.TreeNode GetNodoPadre(Microsoft.Web.UI.WebControls.TreeNode nodo)
        {
            Microsoft.Web.UI.WebControls.TreeNode nodoPadre;

            string indice = nodo.GetNodeIndex();
            int i = indice.LastIndexOf('.');
            if (i > 0)
            {
                string indicePadre = (indice.Remove(i, indice.Length - (indice.LastIndexOf('.'))));
                //Gestione inserimento ruoli sotto la UO principale
                //NEW
                if (indicePadre != "0")
                    nodoPadre = treeViewUO.GetNodeFromIndex(indicePadre);
                else
                    nodoPadre = nodo;
                //OLD
                //nodoPadre = treeViewUO.GetNodeFromIndex(indicePadre);

            }
            else
            {
                nodoPadre = nodo;
            }

            return nodoPadre;
        }

        #endregion

        #region Gestione dei dati sull'interfaccia grafica

        private void GestioneLoadDettagli(myTreeNode TreeNode)
        {
            this.EnableModifyControls(true, TreeNode);
            try
            {
                switch (TreeNode.getTipoNodo())
                {
                    case "U":
                        this.DettagliUO(TreeNode);
                        break;
                    case "R":
                        this.DettagliRuolo(TreeNode);
                        this.DettagliRegistri(TreeNode);
                        this.DettagliRF(TreeNode);
                        this.DettagliFunzioni(TreeNode);
                        break;
                    case "P":
                        this.DettagliUtente(TreeNode);
                        break;
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        private void DettagliUO(myTreeNode TreeNode)
        {
            try
            {
                this.GUI("DettagliUO");

                this.GestLabelPercorso(TreeNode);

                if (TreeNode.Nodes.Count > 0)
                {
                    this.btn_elimina_uo.Visible = false;
                }
                else
                {
                    if (!this.treeViewUO.SelectedNodeIndex.Equals("0") && !this.treeViewUO.SelectedNodeIndex.Equals("0.0"))
                    {
                        this.btn_elimina_uo.Visible = true;
                        this.btn_elimina_uo.Attributes.Add("onclick", "if (!window.confirm('Attenzione!\\n\\nla UO [" + TreeNode.getDescrizione() + "] sarà eliminata.\\n\\nSei sicuro di voler procedere?')) {return false};");
                    }
                    else
                    {
                        this.btn_elimina_uo.Visible = false;
                    }
                }

                if (this.treeViewUO.SelectedNodeIndex.Equals("0") || this.treeViewUO.SelectedNodeIndex.Equals("0.0"))
                {
                    this.btn_sposta_uo.Visible = false;
                    this.btn_importaOrganigramma.Visible = true;
                }
                else
                {
                    this.btn_sposta_uo.Visible = true;
                    this.btn_importaOrganigramma.Visible = false;

                    // modifica CH per consentire lo spostamento di UO in caso di presenza di apostrofi
                    string desc = Server.UrlEncode(TreeNode.getDescrizione());
                    desc = desc.Replace("'", "\\'");
                    //this.btn_sposta_uo.Attributes.Add("onclick", "ApriSpostaUO('" + TreeNode.getIDCorrGlobale() + "','" + TreeNode.getCodiceRubrica() + "','" + Server.UrlEncode(TreeNode.getDescrizione()) + "','" + TreeNode.getIDAmministrazione() + "','" + TreeNode.getLivello() + "');");
                    this.btn_sposta_uo.Attributes.Add("onclick", "ApriSpostaUO('" + TreeNode.getIDCorrGlobale() + "','" + TreeNode.getCodiceRubrica() + "','" + desc + "','" + TreeNode.getIDAmministrazione() + "','" + TreeNode.getLivello() + "');");
                }

                // Impostazione visibilità del pulsante invia dati a rubrica comune
                this.SetVisibilityRubricaComune(true);

                // Caricamento dati in rubrica comune
                this.FetchDatiRubricaComune(TreeNode.getIDCorrGlobale());

                this.btn_ins_ruolo_uo.Visible = true;
                this.btn_ins_sotto_uo.Visible = true;
                this.btn_mod_uo.Text = "Modifica";

                this.txt_rubricaUO.Text = TreeNode.getCodiceRubrica();
                //this.txt_rubricaUO.ReadOnly = true;
                this.txt_descrizioneUO.Text = TreeNode.getDescrizione();

                this.chk_interopUO.Visible = true;
                this.ddl_aoo.Visible = true;

                if (TreeNode.getCodRegInterop() != null && TreeNode.getCodRegInterop() != "" && TreeNode.getCodRegInterop() != string.Empty)
                {
                    chk_interopUO.Checked = true;
                    this.ddl_aoo.Enabled = true;
                }
                else
                {
                    chk_interopUO.Checked = false;
                    this.ddl_aoo.Enabled = false;
                }
                Session.Add("UOinterop", this.chk_interopUO.Checked);
                this.CaricaComboBox_AOO(TreeNode.getCodRegInterop(), TreeNode.getIDAmministrazione());

                // Caricamento impostazioni interoperabilità semplificata
                this.LoadSettingsIS(TreeNode.RegistryInteropSemplificata, TreeNode.RfInteropSemplificata, TreeNode.getIDAmministrazione());

                // Gestione della visualizzazione della sezione relativa all'interoperabilità semplificata
                // e visualizzazione dei valori
                this.trInteropSemplificata.Visible = InteroperabilitaSemplificataManager.IsEnabledSimpInterop;
                this.chkInteroperante.Checked = !String.IsNullOrEmpty(TreeNode.RegistryInteropSemplificata);
                this.ddlRegistriInteropSemplificata.Enabled = this.chkInteroperante.Checked;
                this.ddlRfInteropSemplificata.Enabled = this.chkInteroperante.Checked;
                if (!String.IsNullOrEmpty(TreeNode.RegistryInteropSemplificata))
                    this.ddlRegistriInteropSemplificata.SelectedValue = TreeNode.RegistryInteropSemplificata;
                else
                    this.ddlRegistriInteropSemplificata.SelectedIndex = 0;
                if (!String.IsNullOrEmpty(TreeNode.RfInteropSemplificata))
                    this.ddlRfInteropSemplificata.SelectedValue = TreeNode.RfInteropSemplificata;
                else
                    this.ddlRfInteropSemplificata.SelectedIndex = 0;


                this.DettagliUOStampaBuste(TreeNode.getIDCorrGlobale());
                this.btn_clear_uo.Visible = false;
                this.btn_annulla_ins_uo.Visible = false;

                if (isOrdinamentoAbilitato())
                    this.btn_ordinamento.Visible = (TreeNode.Nodes.Count > 1);
                else
                    this.btn_ordinamento.Visible = false;
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        private void DettagliUOStampaBuste(string idCorrGlob)
        {
            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            theManager.DettagliUOStampaBuste(idCorrGlob);
            if (theManager.getDatiUOStampaBuste() != null)
            {
                DocsPAWA.DocsPaWR.OrgDettagliGlobali dettagli = theManager.getDatiUOStampaBuste();
                this.txt_uo_indirizzo.Text = dettagli.Indirizzo;
                this.txt_uo_citta.Text = dettagli.Citta;
                this.txt_uo_cap.Text = dettagli.Cap;
                this.txt_uo_prov.Text = dettagli.Provincia;
                this.txt_uo_nazione.Text = dettagli.Nazione;
                this.txt_uo_telefono1.Text = dettagli.Telefono1;
                this.txt_uo_telefono2.Text = dettagli.Telefono2;
                this.txt_uo_fax.Text = dettagli.Fax;
                this.txt_uo_note.Text = dettagli.Note;
                this.txt_uo_codice_fiscale.Text = dettagli.CodiceFiscale;
                this.txt_uo_partita_iva.Text = dettagli.PartitaIva;

                Session.Add("codiceUO", this.txt_rubricaUO.Text);
                Session.Add("descUO", this.txt_descrizioneUO.Text);
                Session.Add("UOinSessione", dettagli);                
            }
        }

        private void DettagliRuolo(myTreeNode TreeNode)
        {
            try
            {
                this.GUI("DettagliRuolo");

                this.GestLabelPercorso(TreeNode);

                // dati sulla GUI
                this.CaricaComboBox_TipoRuolo(TreeNode.getIDTipoRuolo(), TreeNode.getIDAmministrazione());
                this.ddl_tipo_ruolo.Enabled = Utils.GetAbilitazioneGestioneRuoliAvanzata();

                this.txt_rubricaRuolo.Text = TreeNode.getCodiceRubrica();
                this.txt_rubricaRuolo.ReadOnly = !Utils.GetAbilitazioneGestioneRuoliAvanzata();

                this.txt_descrizioneRuolo.Text = TreeNode.getDescrizione();
                this.chk_rif.Checked = false;
                if (TreeNode.getDiRiferimento() != null && TreeNode.getDiRiferimento().Equals("1"))
                {
                    this.chk_rif.Checked = true;
                }
                this.chk_resp.Checked = false;
                if (TreeNode.getResponsabile() != null && TreeNode.getResponsabile().Equals("1"))
                {
                    this.chk_resp.Checked = true;
                }
                this.cb_segretario.Checked = false;
                if (TreeNode.getSegretario() != null && TreeNode.getSegretario().Equals("1"))
                {
                    this.cb_segretario.Checked = true;
                }
                this.cb_disabled_trasm.Checked = false;
                if (TreeNode.getDisabledTrasm() != null && TreeNode.getDisabledTrasm().Equals("1"))
                {
                    this.cb_disabled_trasm.Checked = true;
                }

                // pulsante "Gestione utenti"
                string nodoIdGruppo = string.Empty;
                if (TreeNode.getIDGruppo() != null && !TreeNode.getIDGruppo().Equals("0"))
                    nodoIdGruppo = TreeNode.getIDGruppo();
                string nodoIdCorrGlobale = string.Empty;
                if (TreeNode.getIDCorrGlobale() != null && !TreeNode.getIDCorrGlobale().Equals("0"))
                    nodoIdCorrGlobale = TreeNode.getIDCorrGlobale();
                this.btn_utenti_ruolo.Attributes.Add("onclick", "ApriGestioneUtenti('" + Server.UrlEncode(TreeNode.getPercorso().Replace("'", "|@ap@|")) + "','" + nodoIdGruppo + "','" + TreeNode.getIDAmministrazione() + "','" + nodoIdCorrGlobale + "');");
                this.btn_utenti_ruolo.Visible = true;

                // pulsante "Modifica"
                this.btn_mod_ruolo.Text = "Modifica";

                // pulsante "Elimina"
                if (TreeNode.Nodes.Count > 0)
                {
                    this.btn_elimina_ruolo.Visible = false;
                }
                else
                {
                    this.btn_elimina_ruolo.Visible = true;
                    string js = "if(document.getElementById('hd_DisableRole').value!='no_disable' && document.getElementById('hd_DisableRole').value!='disable')\n{\n";
                    js += "return window.confirm('Attenzione!\\n\\nil ruolo [" + TreeNode.getDescrizione().Replace("'", "\\'") + "] sarà eliminato.\\n\\nSei sicuro di voler procedere?');}";
                    this.btn_elimina_ruolo.Attributes.Add("onclick", js);
                }

                // pulsante sposta 
                // NON PIU' NECESSARIO IN QUANTO IL CODICE DELLA UO DI APPARTENENZA PUò ESSERE MODIFICATO DIRETTAMENTE
                //this.btn_sposta_ruolo.Attributes.Add("onclick", "ApriSpostaRuolo('" + nodoIdCorrGlobale + "','" + nodoIdGruppo + "','" + Server.UrlEncode(TreeNode.getDescrizione()) + "','" + TreeNode.getIDAmministrazione() + "','" + Server.UrlEncode(this.ddl_tipo_ruolo.SelectedItem.Text) + "');");
                this.btn_sposta_ruolo.Visible = true;
                this.btn_sposta_ruolo.Enabled = false;
                this.btn_mod_ruolo.Enabled = true;
                this.trExtendVisibility.Visible = false;
                this.rblExtendVisibility.SelectedIndex = 0;
                // Visualizzazione della riga della tabella con i dati sulla UO di appartenenza e dei due checkbox per
                // storicizzazione ruolo e aggiornamento modelli di trasmissione
                this.trCodUO.Visible = Utils.GetAbilitazioneGestioneRuoliAvanzata();
                this.btnOrg.OnClientClick = String.Format("ApriOrganigramma({0});", TreeNode.getIDAmministrazione());
                this.trStoricizzaAndUpdateModels.Visible = Utils.GetAbilitazioneGestioneRuoliAvanzata();
                this.chkStoricizza.Checked = false;
                this.chkUpdateModels.Checked = true;
                this.btnShowRoleHistory.Attributes["onclick"] = DocsPAWA.popup.RoleHistory.GetScriptToOpenWindow(TreeNode.getIDCorrGlobale(), String.Empty);
                this.btnShowRoleHistory.Visible = true;

                this.btn_processiFirmaRuolo.Attributes.Add("onclick", "ApriProcessiFirmaRuolo('" + nodoIdGruppo + "');");
                if (!string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_LIBRO_FIRMA")) && DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_LIBRO_FIRMA").Equals("1"))
                    this.btn_processiFirmaRuolo.Visible = true; 
                else
                    this.btn_processiFirmaRuolo.Visible = false;

                //pulsante visibiltà nodi di titolario
                this.btn_visib_titolario.Visible = false;
                if (this.isTitolarioRuoloAbilitato())
                {
                    this.btn_visib_titolario.Attributes.Add("onclick", "ApriVisibTitolario('" + nodoIdCorrGlobale + "','" + nodoIdGruppo + "','" + TreeNode.getIDAmministrazione() + "','" + this.txt_descrizioneRuolo.Text.Replace("'", "\\'") + "','" + this.txt_rubricaRuolo.Text.Replace("'", "\\'") + "');");
                    this.btn_visib_titolario.Visible = true;
                }

                // Compilazione codice, descrizione e campo nascosto relativi alla UO di appartenenza
                string livelloTree = TreeNode.GetNodeIndex(); // .getLivello();
                if (!livelloTree.Equals("0.0"))
                {
                    this.txtCodUo.Text = ((myTreeNode)TreeNode.Parent).getCodice();
                    this.txtDescUo.Text = ((myTreeNode)TreeNode.Parent).getDescrizione();
                    this.hdIdCorrGlob.Value = ((myTreeNode)TreeNode.Parent).getIDCorrGlobale();
                }

                //pulsante copia visibilità ruolo
                if (TreeNode.Nodes.Count > 0)
                {
                    myTreeNode TreeNodoPadre = (myTreeNode)this.GetNodoPadre(TreeNode);
                    string idCorrGlobUO = TreeNodoPadre.getIDCorrGlobale();
                    this.btn_copiaVisRuolo.Attributes.Add("onclick", "ApriCopiaVisibilitaRuolo('" + TreeNode.getIDAmministrazione() + "','" + idCorrGlobUO + "','" + nodoIdCorrGlobale + "','" + nodoIdGruppo + "','" + this.txt_descrizioneRuolo.Text.Replace("'", "\\'") + "','" + this.txt_rubricaRuolo.Text.Replace("'", "\\'") + "','" + Server.UrlEncode(this.ddl_tipo_ruolo.SelectedItem.Text) + "');");
                    this.btn_copiaVisRuolo.Visible = Utils.GetAbilitazioneCopiaVisibilita();
                }
                else
                {
                    this.btn_copiaVisRuolo.Visible = false;
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        private void DettagliFunzioni(myTreeNode TreeNode)
        {
            try
            {
                this.btn_mod_funzioni.Visible = true;

                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.ListaFunzioni(TreeNode.getIDAmministrazione(), TreeNode.getIDCorrGlobale());

                if (theManager.getListaFunzioni().Count > 0)
                {
                    InitializeDataSetFunzioni();
                    DataRow row;
                    foreach (DocsPAWA.DocsPaWR.OrgTipoFunzione funzione in theManager.getListaFunzioni())
                    {
                        row = dsFunzioni.Tables[0].NewRow();
                        row["IDTipoFunzione"] = funzione.IDTipoFunzione;
                        row["Codice"] = funzione.Codice;
                        row["Descrizione"] = funzione.Descrizione;
                        row["IDAmministrazione"] = TreeNode.getIDAmministrazione();
                        if (funzione.Associato != null && funzione.Associato != String.Empty)
                        {
                            row["Sel"] = "true";
                        }
                        else
                        {
                            row["Sel"] = "false";
                        }
                        dsFunzioni.Tables["FUNZIONI"].Rows.Add(row);
                    }

                    DataView dv = dsFunzioni.Tables["FUNZIONI"].DefaultView;
                    dv = OrdinaGrid(dv, "Descrizione");
                    dg_funzioni.DataSource = dv;
                    dg_funzioni.DataBind();
                }
                else
                {
                    this.btn_mod_funzioni.Visible = false;
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        private void DettagliRegistri(myTreeNode TreeNode)
        {
            try
            {
                DocsPaWebService ws = new DocsPaWebService();
                this.btn_mod_registri.Visible = true;
                this.btn_vis_reg.Visible = true;
                
                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                //voglio la lista dei soli registri, quindi al webMethod passero come chaRF il valore 0 (solo registri)
                theManager.ListaRegistriRF(TreeNode.getIDAmministrazione(), TreeNode.getIDCorrGlobale(), "0");

                // prende la system_id della UO padre
                myTreeNode TreeNodoPadre = (myTreeNode)this.GetNodoPadre(TreeNode);
                string idCorrGlobUO = TreeNodoPadre.getIDCorrGlobale();

                if (theManager.getListaRegistri().Count > 0)
                {
                    this.btn_vis_reg.Attributes.Add("onclick", "ApriGestVisibilita('" + idCorrGlobUO + "','" + TreeNode.getIDAmministrazione() + "','" + TreeNode.getIDCorrGlobale() + "','" + TreeNode.getIDGruppo() + "');");

                    InitializeDataSetRegistri();
                    DataRow row;
                    foreach (DocsPAWA.DocsPaWR.OrgRegistro registro in theManager.getListaRegistri())
                    {
                        row = dsRegistri.Tables[0].NewRow();
                        row["IDRegistro"] = registro.IDRegistro;
                        row["Codice"] = registro.Codice;
                        row["Descrizione"] = registro.Descrizione;
                        row["IDAmministrazione"] = registro.IDAmministrazione;
                        if (registro.Associato != null && registro.Associato != String.Empty)
                        {
                            row["Sel"] = "true";
                        }
                        else
                        {
                            row["Sel"] = "false";
                        }
                        row["Sospeso"] = registro.Sospeso;

                        //caso ruolo associato ad un Registro
                        if (row["Sel"].ToString().Equals("true"))
                        {
                            DataSet ds = MultiCasellaManager.GetRightMailRegistro(registro.IDRegistro, TreeNode.getIDCorrGlobale());
                            //se è un Registro con una o più caselle imposto i diritti del ruolo sulle singole caselle
                            if (ds.Tables.Count > 0 && ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                            {
                                if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                                {
                                    string casellaPrincipale = MultiCasellaManager.GetMailPrincipaleRegistro(registro.IDRegistro);
                                    foreach (DataRow r in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                                    {
                                        if (row.RowState.ToString().ToLower().Equals("added"))
                                        {
                                            row = dsRegistri.Tables[0].NewRow();
                                            row["IDRegistro"] = registro.IDRegistro;
                                            row["Codice"] = registro.Codice;
                                            row["Descrizione"] = registro.Descrizione;
                                            row["IDAmministrazione"] = registro.IDAmministrazione;
                                            if (registro.Associato != null && registro.Associato != String.Empty)
                                            {
                                                row["Sel"] = "true";
                                            }
                                            else
                                            {
                                                row["Sel"] = "false";
                                            }
                                            row["Sospeso"] = registro.Sospeso;
                                        }
                                        if (casellaPrincipale != null && casellaPrincipale.Equals(r["EMAIL_REGISTRO"].ToString()))
                                        {
                                            row["EmailRegistro"] = "* " + r["EMAIL_REGISTRO"].ToString();
                                        }
                                        else
                                        {
                                            row["EmailRegistro"] = r["EMAIL_REGISTRO"].ToString();
                                        }
                                        row["consulta"] = (r["CONSULTA"].ToString().Equals("1") ? "true" : "false");
                                        row["spedisci"] = (r["SPEDISCI"].ToString().Equals("1") ? "true" : "false");
                                        row["notifica"] = (r["NOTIFICA"].ToString().Equals("1") ? "true" : "false");
                                        dsRegistri.Tables["REGISTRI"].Rows.Add(row);
                                    }
                                }
                            }  
                            //se Registro senza caselle di posta
                            else
                            {
                                row["EmailRegistro"] = string.Empty;
                                row["consulta"] = "false";
                                row["spedisci"] = "false";
                                row["notifica"] = "false";
                                dsRegistri.Tables["REGISTRI"].Rows.Add(row);
                            }
                        }
                        //se il ruolo non è associato al Registro
                        else
                        {
                            CasellaRegistro[]  caselle = MultiCasellaManager.GetMailRegistro(registro.IDRegistro);
                            //se registro con una o più caselle imposto i diritti del ruolo sulle singole caselle
                            if (caselle != null && caselle.Length > 0)
                            {
                                string casellaPrincipale = MultiCasellaManager.GetMailPrincipaleRegistro(registro.IDRegistro);
                                foreach (CasellaRegistro c in caselle)
                                {
                                    if (row.RowState.ToString().ToLower().Equals("added"))
                                    {
                                        row = dsRegistri.Tables[0].NewRow();
                                        row["IDRegistro"] = registro.IDRegistro;
                                        row["Codice"] = registro.Codice;
                                        row["Descrizione"] = registro.Descrizione;
                                        row["IDAmministrazione"] = registro.IDAmministrazione;
                                        if (registro.Associato != null && registro.Associato != String.Empty)
                                        {
                                            row["Sel"] = "true";
                                        }
                                        else
                                        {
                                            row["Sel"] = "false";
                                        }
                                        row["Sospeso"] = registro.Sospeso;
                                    }
                                    if (casellaPrincipale != null && casellaPrincipale.Equals(c.EmailRegistro))
                                    {
                                        row["EmailRegistro"] = "* " + c.EmailRegistro;
                                    }
                                    else
                                    {
                                        row["EmailRegistro"] = c.EmailRegistro;
                                    }
                                    row["consulta"] = "false";
                                    row["spedisci"] = "false";
                                    row["notifica"] = "false";
                                    dsRegistri.Tables["REGISTRI"].Rows.Add(row);
                                }
                                if (ws.IsEnabledInteropInterna())
                                {
                                    if (row.RowState.ToString().ToLower().Equals("added"))
                                    {
                                        row = dsRegistri.Tables[0].NewRow();
                                        row["IDRegistro"] = registro.IDRegistro;
                                        row["Codice"] = registro.Codice;
                                        row["Descrizione"] = registro.Descrizione;
                                        row["IDAmministrazione"] = registro.IDAmministrazione;
                                        if (registro.Associato != null && registro.Associato != String.Empty)
                                        {
                                            row["Sel"] = "true";
                                        }
                                        else
                                        {
                                            row["Sel"] = "false";
                                        }
                                        row["Sospeso"] = registro.Sospeso;
                                    }
                                    row["EmailRegistro"] = string.Empty;
                                    row["consulta"] = "false";
                                    row["spedisci"] = "false";
                                    row["notifica"] = "false";
                                    dsRegistri.Tables["REGISTRI"].Rows.Add(row);
                                }
                            }

                            //se registro senza caselle di posta 
                            else
                            {
                                row["EmailRegistro"] = string.Empty;
                                row["consulta"] = "false";
                                row["spedisci"] = "false";
                                row["notifica"] = "false";
                                dsRegistri.Tables["REGISTRI"].Rows.Add(row);
                            }
                        }
                    }

                    DataView dv = dsRegistri.Tables["REGISTRI"].DefaultView;
                    dv = OrdinaGrid(dv, "Descrizione");
                    this.dg_registri.DataSource = dv;
                    this.dg_registri.DataBind();
                }
                else
                {
                    this.btn_mod_registri.Visible = false;
                    this.btn_vis_reg.Visible = false;
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }


        private void DettagliRF(myTreeNode TreeNode)
        {
            DocsPaWebService ws = new DocsPaWebService();
            try
            {
                this.pnlRF.Visible = false;

                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                //voglio la lista dei soli RF, quindi al webMethod passero come chaRF il valore 1 (solo RF)
                theManager.ListaRegistriRF(TreeNode.getIDAmministrazione(), TreeNode.getIDCorrGlobale(), "1");
                DataRow row;
                // prende la system_id della UO padre
                myTreeNode TreeNodoPadre = (myTreeNode)this.GetNodoPadre(TreeNode);
                string idCorrGlobUO = TreeNodoPadre.getIDCorrGlobale();

                
                if (theManager.getListaRegistri() != null && theManager.getListaRegistri().Count > 0)
                {
                    this.pnlRF.Visible = true;
                    InitializeDataSetRF();
                    foreach (DocsPAWA.DocsPaWR.OrgRegistro registro in theManager.getListaRegistri())
                    {
                        row = dsRF.Tables[0].NewRow();
                        row["IDRegistro"] = registro.IDRegistro;

                        row["Codice"] = registro.Codice;

                        row["Descrizione"] = registro.Descrizione;
                        row["IDAmministrazione"] = registro.IDAmministrazione;
                        if (registro.Associato != null && registro.Associato != String.Empty)
                        {
                            row["Sel"] = "true";
                        }
                        else
                        {
                            row["Sel"] = "false";
                        }

                        row["Disabled"] = registro.rfDisabled;
                        row["AooCollegata"] = registro.idAOOCollegata;

                        //caso ruolo associato ad un RF
                        if (row["Sel"].ToString().Equals("true"))
                        {                           
                            DataSet ds = MultiCasellaManager.GetRightMailRegistro(registro.IDRegistro, TreeNode.getIDCorrGlobale());
                            //se RF con una o più caselle imposto i diritti del ruolo sulle singole caselle
                            if (ds.Tables.Count > 0 && ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                            {
                                if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                                {
                                    string casellaPrincipale = MultiCasellaManager.GetMailPrincipaleRegistro(registro.IDRegistro);
                                    foreach (DataRow r in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                                    {
                                        if (row.RowState.ToString().ToLower().Equals("added"))
                                        {
                                            row = dsRF.Tables[0].NewRow();
                                            row["IDRegistro"] = registro.IDRegistro;
                                            row["Codice"] = registro.Codice;
                                            row["Descrizione"] = registro.Descrizione;
                                            row["IDAmministrazione"] = registro.IDAmministrazione;
                                            if (registro.Associato != null && registro.Associato != String.Empty)
                                            {
                                                row["Sel"] = "true";
                                            }
                                            else
                                            {
                                                row["Sel"] = "false";
                                            }
                                            row["Disabled"] = registro.rfDisabled;
                                            row["AooCollegata"] = registro.idAOOCollegata;
                                        }
                                        if (casellaPrincipale != null && casellaPrincipale.Equals(r["EMAIL_REGISTRO"].ToString()))
                                        {
                                            row["EmailRegistro"] = "* " + r["EMAIL_REGISTRO"].ToString();
                                        }
                                        else
                                        {
                                            row["EmailRegistro"] = r["EMAIL_REGISTRO"].ToString();
                                        }
                                        row["consulta"] = (r["CONSULTA"].ToString().Equals("1") ? "true" : "false");
                                        row["spedisci"] = (r["SPEDISCI"].ToString().Equals("1") ? "true" : "false");
                                        row["notifica"] = (r["NOTIFICA"].ToString().Equals("1") ? "true" : "false");
                                        dsRF.Tables["RF"].Rows.Add(row);
                                    }
                                }
                            }

                            //se rf senza caselle di posta
                            else
                            {
                                row["EmailRegistro"] = string.Empty;
                                row["consulta"] = "false";
                                row["spedisci"] = "false";
                                row["notifica"] = "false";
                                dsRF.Tables["RF"].Rows.Add(row);
                            }
                        }
                        //se il ruolo non è associato all'RF
                        else
                        {                            
                            CasellaRegistro [] caselle = MultiCasellaManager.GetMailRegistro(registro.IDRegistro);
                            //se rf con una o più caselle imposto i diritti del ruolo sulle singole caselle
                            if (caselle != null && caselle.Length > 0)
                            {
                                string casellaPrincipale = MultiCasellaManager.GetMailPrincipaleRegistro(registro.IDRegistro);
                                foreach (CasellaRegistro c in caselle)
                                {
                                    if (row.RowState.ToString().ToLower().Equals("added"))
                                    {
                                        row = dsRF.Tables[0].NewRow();
                                        row["IDRegistro"] = registro.IDRegistro;
                                        row["Codice"] = registro.Codice;
                                        row["Descrizione"] = registro.Descrizione;
                                        row["IDAmministrazione"] = registro.IDAmministrazione;
                                        row["Sel"] = "false";
                                        row["Disabled"] = registro.rfDisabled;
                                        row["AooCollegata"] = registro.idAOOCollegata;
                                    }
                                    if (casellaPrincipale != null && casellaPrincipale.Equals(c.EmailRegistro))
                                    {
                                        row["EmailRegistro"] = "* " + c.EmailRegistro;
                                    }
                                    else
                                    {
                                        row["EmailRegistro"] = c.EmailRegistro;
                                    }
                                    row["consulta"] = "false"; 
                                    row["spedisci"] = "false";
                                    row["notifica"] = "false";
                                    dsRF.Tables["RF"].Rows.Add(row);
                                }
                                if (ws.IsEnabledInteropInterna())
                                {
                                    if (row.RowState.ToString().ToLower().Equals("added"))
                                    {
                                        row = dsRF.Tables[0].NewRow();
                                        row["IDRegistro"] = registro.IDRegistro;
                                        row["Codice"] = registro.Codice;
                                        row["Descrizione"] = registro.Descrizione;
                                        row["IDAmministrazione"] = registro.IDAmministrazione;
                                        row["Sel"] = "false";
                                        row["Disabled"] = registro.rfDisabled;
                                        row["AooCollegata"] = registro.idAOOCollegata;
                                    }
                                    row["EmailRegistro"] = string.Empty;
                                    row["consulta"] = "false";
                                    row["spedisci"] = "false";
                                    row["notifica"] = "false";
                                    dsRF.Tables["RF"].Rows.Add(row);
                                }
                            }

                            //se rf senza caselle di posta 
                            else
                            {
                                row["EmailRegistro"] = string.Empty;
                                row["consulta"] = "false";
                                row["spedisci"] = "false";
                                row["notifica"] = "false";
                                dsRF.Tables["RF"].Rows.Add(row);
                            }
                        }
                    }

                    DataView dv = dsRF.Tables["RF"].DefaultView;
                    dv = OrdinaGrid(dv, "Descrizione");
                    this.dg_rf.DataSource = dv;
                    this.dg_rf.DataBind();
                }

            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        private void DettagliUtente(myTreeNode TreeNode)
        {
            try
            {
                this.GUI("DettagliUtente");

                myTreeNode TreeNodoPadre = (myTreeNode)this.GetNodoPadre(TreeNode);
                this.GestLabelPercorso(TreeNodoPadre);

                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.DatiUtente(TreeNode.getIDCorrGlobale());

                DocsPAWA.DocsPaWR.OrgUtente utente = theManager.getDatiUtente();

                this.txt_userIdUtente.Text = utente.UserId;
                //this.txt_pwdUtente.Text = utente.Password;
                this.txt_dominioUtente.Text = utente.Dominio;
                this.txt_nomeUtente.Text = utente.Nome;
                this.txt_cognomeUtente.Text = utente.Cognome;
                this.txt_rubicaUtente.Text = utente.CodiceRubrica;
                this.txt_emailUtente.Text = utente.Email;
                this.txt_sedeUtente.Text = utente.Sede;
                if (utente.NotificaTrasm != string.Empty && utente.NotificaTrasm != null)
                {
                    this.ddl_notifica.SelectedIndex = ddl_notifica.Items.IndexOf(ddl_notifica.Items.FindByValue(utente.NotificaTrasm));
                }
                else
                {
                    this.ddl_notifica.SelectedIndex = ddl_notifica.Items.IndexOf(ddl_notifica.Items.FindByValue("null"));
                }
                this.cb_abilitato.Checked = false;
                if (utente.Abilitato.Equals("N"))
                {
                    this.cb_abilitato.Checked = true;
                }
                this.cb_amm.Checked = false;
                if (utente.Amministratore.Equals("1"))
                {
                    this.cb_amm.Checked = true;
                }

                // disabilita tutto
                this.txt_userIdUtente.ReadOnly = true;
                //this.txt_pwdUtente.ReadOnly=true;
                this.txt_dominioUtente.ReadOnly = true;
                this.txt_nomeUtente.ReadOnly = true;
                this.txt_cognomeUtente.ReadOnly = true;
                this.txt_rubicaUtente.ReadOnly = true;
                this.txt_emailUtente.ReadOnly = true;
                this.txt_sedeUtente.ReadOnly = true;
                this.ddl_notifica.Enabled = false;
                this.cb_abilitato.Enabled = false;
                this.cb_amm.Enabled = false;

                //gestione pulsanti
                if (VerificaProcessiCoinvolti("", TreeNode))
                {
                    if (TreeNodoPadre.Nodes.Count > 1)
                        this.Gest_AperturaWnd_SpostaUtente(false);
                    else
                    {
                        this.btn_sposta_utente.Attributes.Add("onclick", "alert('Attenzione,\\npoichè il ruolo presenta solo questo utente, per effettuare lo spostamento\\nè consigliato procedere come segue:\\n\\nutilizzare il tasto \\'Gestione utenti\\' presente nei dettagli del ruolo\\nper eliminare o inserire utenti nei ruoli.'); return true;");
                    }
                }
                //gestione qualifiche
                string chiaveQualifiche = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "GESTIONE_QUALIFICHE");
                if (!string.IsNullOrEmpty(chiaveQualifiche) && chiaveQualifiche.Equals("1"))
                {
                    myTreeNode TreeNodoUO = (myTreeNode)this.GetNodoPadre(TreeNodoPadre);
                    this.btn_gest_qual.Attributes.Add("onclick", "ApriGestioneQualifiche('" + utente.IDAmministrazione + "','" + TreeNodoUO.getIDCorrGlobale() + "','" + TreeNodoPadre.getIDGruppo() + "','" + utente.IDPeople + "');");
                }
                else
                {
                    this.btn_gest_qual.Visible = false;
                }

                string chiaveApplicazioni = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "ENABLE_GEST_EXT_APPS");
                if (!string.IsNullOrEmpty(chiaveApplicazioni) && chiaveApplicazioni.Equals("1"))
                {
                    this.btn_gest_app.Attributes.Add("onclick", "ApriGestioneApplicazioni('" + utente.IDPeople + "','" + utente.UserId + "');");
                }
                else
                {
                    this.btn_gest_app.Visible = false;
                }
                this.btn_processiFirmaUtente.Attributes.Add("onclick", "ApriProcessiFirmaUtente('" + TreeNodoPadre.getIDGruppo() + "','" + utente.IDPeople + "');");
                if (!string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_LIBRO_FIRMA")) && DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_LIBRO_FIRMA").Equals("1"))
                    this.btn_processiFirmaUtente.Visible = true;
                else
                    this.btn_processiFirmaUtente.Visible = false;
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        private void Gest_AperturaWnd_SpostaUtente(bool openWnd)
        {
            myTreeNode TreeNodo;
            TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
            string idAmm = TreeNodo.getIDAmministrazione();
            string idCorrGlobUtente = TreeNodo.getIDCorrGlobale();

            if (TreeNodo.getTipoNodo() == "P")
            {
                myTreeNode TreeNodoPadre = (myTreeNode)this.GetNodoPadre(TreeNodo);
                int contaUtentiNelRuolo = TreeNodoPadre.Nodes.Count;
                string idCorrGlobGruppo = TreeNodoPadre.getIDCorrGlobale();
                string idGruppo = TreeNodoPadre.getIDGruppo();

                string nome = this.txt_nomeUtente.Text;
                if (nome.Contains("'"))
                    nome = nome.Replace("'", "");
                string cognome = this.txt_cognomeUtente.Text;
                if (cognome.Contains("'"))
                    cognome = cognome.Replace("'", " ");

                string userid = this.txt_userIdUtente.Text;

                if (openWnd)
                {
                    if (!ClientScript.IsStartupScriptRegistered("openWndSpostaUt"))
                    {

                        string scriptString = "<SCRIPT>ApriSpostaUtente('" + idCorrGlobUtente + "','" + userid + "','" + Server.UrlEncode(nome) + "','" + Server.UrlEncode(cognome) + "','" + idAmm + "','" + Convert.ToString(contaUtentiNelRuolo) + "','" + idCorrGlobGruppo + "','" + idGruppo + "');</SCRIPT>";
                        ClientScript.RegisterStartupScript(this.GetType(), "openWndSpostaUt", scriptString);
                    }
                }
                else
                    this.btn_sposta_utente.Attributes.Add("onclick", "ApriSpostaUtente('" + idCorrGlobUtente + "','" + userid + "','" + Server.UrlEncode(nome) + "','" + Server.UrlEncode(cognome) + "','" + idAmm + "','" + Convert.ToString(contaUtentiNelRuolo) + "','" + idCorrGlobGruppo + "','" + idGruppo + "');");
            }
        }

        private void InitializeDataSetRegistri()
        {
            dsRegistri = new DataSet();
            DataColumn[] keys = new DataColumn[2];
            DataColumn dc;
            dsRegistri.Tables.Add("REGISTRI");
            dc = new DataColumn("IDRegistro", System.Type.GetType("System.String"));
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            keys[0] = dc;
            dc = new DataColumn("Codice");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dc = new DataColumn("Descrizione");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dc = new DataColumn("EmailRegistro", System.Type.GetType("System.String"));
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            keys[1] = dc;
            dc = new DataColumn("IDAmministrazione");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dc = new DataColumn("Sel");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dc = new DataColumn("consulta");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dc = new DataColumn("notifica");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dc = new DataColumn("spedisci");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dc = new DataColumn("Sospeso");
            dsRegistri.Tables["REGISTRI"].Columns.Add(dc);
            dsRegistri.Tables["REGISTRI"].PrimaryKey = keys;
        }

        private void InitializeDataSetRF()
        {
            dsRF = new DataSet();
            DataColumn[] keys = new DataColumn[2];
            DataColumn dc;
            dsRF.Tables.Add("RF");

            dc = new DataColumn("IDRegistro", System.Type.GetType("System.String"));
            dsRF.Tables["RF"].Columns.Add(dc);
            keys[0] = dc;

            dc = new DataColumn("Codice");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("Descrizione");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("IDAmministrazione");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("Sel");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("Disabled");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("AooCollegata");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("EmailRegistro", System.Type.GetType("System.String"));
            
            dsRF.Tables["RF"].Columns.Add(dc);
            keys[1] = dc;

            dc = new DataColumn("consulta");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("notifica");
            dsRF.Tables["RF"].Columns.Add(dc);

            dc = new DataColumn("spedisci");
            dsRF.Tables["RF"].Columns.Add(dc);

            dsRF.Tables["RF"].PrimaryKey = keys;
        }

        private void InitializeDataSetFunzioni()
        {
            dsFunzioni = new DataSet();
            DataColumn dc;
            dsFunzioni.Tables.Add("FUNZIONI");
            dc = new DataColumn("IDTipoFunzione");
            dsFunzioni.Tables["FUNZIONI"].Columns.Add(dc);
            dc = new DataColumn("Codice");
            dsFunzioni.Tables["FUNZIONI"].Columns.Add(dc);
            dc = new DataColumn("Descrizione");
            dsFunzioni.Tables["FUNZIONI"].Columns.Add(dc);
            dc = new DataColumn("IDAmministrazione");
            dsFunzioni.Tables["FUNZIONI"].Columns.Add(dc);
            dc = new DataColumn("Sel");
            dsFunzioni.Tables["FUNZIONI"].Columns.Add(dc);
        }

        private DataView OrdinaGrid(DataView dv, string sortColumn)
        {
            string[] words = dv.Sort.Split(' ');
            string sortMode;
            if (words.Length < 2)
            {
                sortMode = " ASC";
            }
            else
            {
                if (words[1].Equals("ASC"))
                {
                    sortMode = " DESC";
                }
                else
                {
                    sortMode = " ASC";
                }
            }
            dv.Sort = sortColumn + sortMode;
            return dv;
        }

        /// <summary>
        /// Pulisce la ddl_aoo e quelle per l'interoperabilità semplificata delle voci oltre quelle di default
        /// </summary>
        private void PulisceComboBox_AOO()
        {
            int contaItems;
            contaItems = this.ddl_aoo.Items.Count;
            contaItems--;
            for (int n = contaItems; n > 1; n--)
            {
                this.ddl_aoo.Items.RemoveAt(n);
                this.ddlRegistriInteropSemplificata.Items.RemoveAt(n);
            }

            contaItems = this.ddlRfInteropSemplificata.Items.Count;
            contaItems--;
            for (int n = contaItems; n > 1; n--)
                this.ddlRfInteropSemplificata.Items.RemoveAt(n);
                
        }

        /// <summary>
        /// Pulisce la ddl_tipo_ruolo delle voci oltre quelle di default
        /// </summary>
        private void PulisceComboBox_TipoRuolo()
        {
            int contaItems;
            contaItems = this.ddl_tipo_ruolo.Items.Count;
            contaItems--;
            for (int n = contaItems; n > 1; n--)
            {
                this.ddl_tipo_ruolo.Items.RemoveAt(n);
            }
        }

        /// <summary>
        /// Carica Tipo Ruolo
        /// </summary>
        /// <param name="idAmm"></param>
        private void CaricaComboBox_TipoRuolo(string idTipoRuolo, string idAmm)
        {
            try
            {
                string FindByValue = "null";

                // pulisce la ddl_tipo_ruolo
                this.PulisceComboBox_TipoRuolo();

                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.ListaTipiRuolo(idAmm);

                if (theManager.getListaTipiRuolo().Count > 0)
                {
                    foreach (DocsPAWA.DocsPaWR.OrgTipoRuolo tipoRuolo in theManager.getListaTipiRuolo())
                    {
                        ListItem item = new ListItem(tipoRuolo.Codice + " - [" + tipoRuolo.Livello + "]" + " - " + tipoRuolo.Descrizione, tipoRuolo.IDTipoRuolo + "@" + tipoRuolo.Codice + "#" + tipoRuolo.Descrizione);
                        this.ddl_tipo_ruolo.Items.Add(item);

                        if (idTipoRuolo != null && idTipoRuolo.Equals(tipoRuolo.IDTipoRuolo))
                            FindByValue = tipoRuolo.IDTipoRuolo + "@" + tipoRuolo.Codice + "#" + tipoRuolo.Descrizione;
                    }

                    this.ddl_tipo_ruolo.SelectedIndex = ddl_tipo_ruolo.Items.IndexOf(ddl_tipo_ruolo.Items.FindByValue(FindByValue));
                }
                else
                {
                    if (!ClientScript.IsStartupScriptRegistered("alertJavaScript"))
                    {

                        string scriptString = "<SCRIPT>alert('Attenzione, non sono presenti TIPI RUOLO.');</SCRIPT>";
                        ClientScript.RegisterStartupScript(this.GetType(), "alertJavaScript", scriptString);
                    }
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// Carica la combo-box con i registri disponibili per AOO e seleziona il registro corrente (se passato come parametro)
        /// </summary>
        /// <param name="codRegistro"></param>
        private void CaricaComboBox_AOO(string codRegistro, string idAmm)
        {
            try
            {
                // pulisce la ddl_aoo
                this.PulisceComboBox_AOO();

                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.ListaRegistriRF(idAmm, null, "");

                if (theManager.getListaRegistri() != null && theManager.getListaRegistri().Count > 0)
                {
                    foreach (DocsPAWA.DocsPaWR.OrgRegistro registro in theManager.getListaRegistri())
                    {
                        ListItem item = new ListItem(registro.Descrizione, registro.Codice);
                        this.ddl_aoo.Items.Add(item);
                    }

                    this.ddl_aoo.SelectedIndex = ddl_aoo.Items.IndexOf(ddl_aoo.Items.FindByValue(codRegistro));
                    Session.Add("oldDDL_AOO", this.ddl_aoo.SelectedIndex);
                }
                else
                {
                    this.lbl_avviso.Text = "Attenzione! registri non presenti";
                    this.GUI("ResetAll");
                }

            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        private void chk_interopUO_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.chk_interopUO.Checked)
                this.ddl_aoo.Enabled = true;
            else
            {
                this.ddl_aoo.Enabled = false;
                this.ddl_aoo.SelectedIndex = ddl_aoo.Items.IndexOf(ddl_aoo.Items.FindByValue("null"));
            }
            bool oldValueCheck = (bool)Session["UOinterop"];
            if (oldValueCheck != this.chk_interopUO.Checked)
                Session.Add("interopUOChanged", true);
            else
                Session.Remove("interopUOChanged");
        }

        private void GraficaNuovaUO(bool sottoUO)
        {
            this.SetVisibilityRubricaComune(false);

            this.btn_elimina_uo.Visible = false;
            this.btn_ins_ruolo_uo.Visible = false;
            this.btn_ins_sotto_uo.Visible = false;
            this.btn_sposta_uo.Visible = false;
            this.btn_mod_uo.Text = "Inserisci";
            this.btn_ordinamento.Visible = false;

            if (sottoUO)
            {
                this.btn_clear_uo.Visible = true;
                this.btn_annulla_ins_uo.Visible = true;
            }

            this.txt_rubricaUO.Text = "";
            this.txt_rubricaUO.ReadOnly = false;
            this.txt_descrizioneUO.Text = "";
            this.chk_interopUO.Checked = false;
            this.chk_interopUO.Enabled = true;
            this.txt_uo_note.Text = "";
            this.ddl_aoo.Enabled = false;
            this.trInteropSemplificata.Visible = InteroperabilitaSemplificataManager.IsEnabledSimpInterop;
            this.txt_uo_citta.Text = "";
            this.txt_uo_indirizzo.Text = "";
            this.txt_uo_cap.Text = "";
            this.txt_uo_prov.Text = "";
            this.txt_uo_codice_fiscale.Text = "";
            this.txt_uo_partita_iva.Text = "";
            //Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            //theManager.CurrentIDAmm(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"0"));

            this.CaricaComboBox_AOO(null, AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3"));

            // Caricamento delle impostazioni relative all'interoperabilità semplificata
            this.LoadSettingsIS(null, null, AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3"));

            this.SetFocus(this.txt_rubricaUO);

            if (!sottoUO)
            {
                this.lbl_percorso.Text = "Inserimento nuova unità organizzativa...";
            }
        }

        private void GraficaNuovoRuolo()
        {
            this.GUI("NuovoRuolo");

            myTreeNode TreeNodo;
            TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);

            this.CaricaComboBox_TipoRuolo("null", TreeNodo.getIDAmministrazione());

            this.SetVisibilityRubricaComune(false);

            this.ddl_tipo_ruolo.Enabled = true;

            this.btn_elimina_ruolo.Visible = false;
            this.btn_sposta_ruolo.Visible = false;
            this.btn_utenti_ruolo.Visible = false;
            this.btn_copiaVisRuolo.Visible = false;
            this.btn_processiFirmaRuolo.Visible = false;
            this.btn_mod_ruolo.Text = "Inserisci";

            this.txt_rubricaRuolo.Text = "";
            this.txt_rubricaRuolo.ReadOnly = false;
            this.txt_descrizioneRuolo.Text = "";
            this.chk_rif.Checked = false;
            this.chk_resp.Checked = false;
            this.cb_segretario.Checked = false;
            this.cb_disabled_trasm.Checked = false;

            // In caso di inserimento i dati relativi alla UO di appartenza e i due checkbox storicizza ruolo, aggiorna
            // modelli di trasmissione ed il pulsante per mostrare la storia del ruolo devono essere nascosti
            this.trCodUO.Visible = false;
            this.trStoricizzaAndUpdateModels.Visible = false;
            this.btnShowRoleHistory.Visible = false;
            this.trExtendVisibility.Visible = false;
            
        }

        private void SetFocus(System.Web.UI.Control ctrl)
        {
            string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
            RegisterStartupScript("focus", s);
        }

        #endregion

        #region Visibilità dei pannelli sull'interfaccia grafica

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        private void GUI(string from)
        {
            // gestione della visibilità degli oggetti sulla GUI
            this.trCalculateAtipicita.Visible = false;
            switch (from)
            {
                case "ResetAll": //........................ annulla tutto!
                    this.pnl_tv.Visible = false;
                    this.pnl_uo.Visible = false;
                    this.pnl_ruoli.Visible = false;
                    this.pnl_funzioni.Visible = false;
                    this.pnl_registri.Visible = false;
                    this.pnl_utente.Visible = false;
                    this.lbl_percorso.Visible = false;
                    break;
                case "InsPrimaUO": //...................... non esistono UO e bisogna inserire la prima UO in assoluto
                    this.pnl_tv.Visible = false;
                    this.pnl_uo.Visible = true;
                    this.pnl_ruoli.Visible = false;
                    this.pnl_funzioni.Visible = false;
                    this.pnl_registri.Visible = false;
                    this.pnl_utente.Visible = false;
                    break;
                case "TreeviewLivelloZero": //............. visualizzazione delle UO a livello zero (page load)
                    this.pnl_tv.Visible = true;
                    this.pnl_uo.Visible = false;
                    this.pnl_ruoli.Visible = false;
                    this.pnl_funzioni.Visible = false;
                    this.pnl_registri.Visible = false;
                    this.pnl_utente.Visible = false;
                    break;
                case "DettagliUO": //...................... visualizzazione dati della UO selezionata
                    this.pnl_tv.Visible = true;
                    this.pnl_uo.Visible = true;
                    this.pnl_ruoli.Visible = false;
                    this.pnl_funzioni.Visible = false;
                    this.pnl_registri.Visible = false;
                    this.pnl_utente.Visible = false;
                    break;
                case "DettagliRuolo": //.................... visualizzazione dati del ruolo selezionato
                    this.pnl_tv.Visible = true;
                    this.pnl_uo.Visible = false;
                    this.pnl_ruoli.Visible = true;
                    this.pnl_funzioni.Visible = true;
                    this.pnl_registri.Visible = true;
                    this.pnl_utente.Visible = false;
                    break;
                case "DettagliUtente": //.................... visualizzazione dati dell'utente selezionato
                    this.pnl_tv.Visible = true;
                    this.pnl_uo.Visible = false;
                    this.pnl_ruoli.Visible = false;
                    this.pnl_funzioni.Visible = false;
                    this.pnl_registri.Visible = false;
                    this.pnl_utente.Visible = true;
                    break;
                case "NuovaUO": //........................... inserimento nuova UO
                    this.pnl_tv.Visible = true;
                    this.pnl_uo.Visible = true;
                    this.pnl_ruoli.Visible = false;
                    this.pnl_funzioni.Visible = false;
                    this.pnl_registri.Visible = false;
                    this.pnl_utente.Visible = false;
                    break;
                case "NuovoRuolo": //........................... inserimento nuovo Ruolo
                    this.pnl_tv.Visible = true;
                    this.pnl_uo.Visible = false;
                    this.pnl_ruoli.Visible = true;
                    this.pnl_funzioni.Visible = false;
                    this.pnl_registri.Visible = false;
                    this.pnl_utente.Visible = false;
                    break;
            }
        }

        #endregion

        #region Tasti

        #region UO
        /// <summary>
        /// Tasto Inserimento sotto-UO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ins_sotto_uo_Click(object sender, System.EventArgs e)
        {
            this.btn_storicizza.Visible = false;
            this.GraficaNuovaUO(true);
        }

        /// <summary>
        /// tasto Modifica /Salva UO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_mod_uo_Click(object sender, System.EventArgs e)
        {
            switch (this.btn_mod_uo.Text)
            {
                case "Modifica":
                    this.ModificaUO(false);
                    break;

                case "Inserisci":
                    if (Session["cambiaCodUO"] != null)
                    {
                        Session.Remove("cambioCodUO");
                        this.btn_ordinamento.Enabled = true;
                        this.btn_annulla_ins_uo.Enabled = true;
                        this.btn_clear_uo.Enabled = true;
                        this.btn_elimina_uo.Enabled = true;
                        this.btn_importaOrganigramma.Enabled = true;
                        this.btn_sposta_uo.Enabled = true;
                        this.btn_ins_ruolo_uo.Enabled = true;
                        this.btn_ins_sotto_uo.Enabled = true;
                    }
                    if (this.lbl_percorso.Text.Equals("Inserimento nuova unità organizzativa..."))
                    {
                        this.InserimentoUO();
                    }
                    else
                    {
                        this.InserimentoSottoUO();
                    }
                    break;
            }
        }

        /// <summary>
        /// tasto inserisci nuovo ruolo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ins_ruolo_uo_Click(object sender, System.EventArgs e)
        {
            this.GraficaNuovoRuolo();
        }

        /// <summary>
        /// tasto Elimina UO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_elimina_uo_Click(object sender, System.EventArgs e)
        {
            this.EliminaUO();
        }

        private void btn_sposta_uo_Click(object sender, System.EventArgs e)
        {
            this.Inizialize();
        }

        private void btn_clear_uo_Click(object sender, System.EventArgs e)
        {
            this.txt_uo_indirizzo.Text = "";
            this.txt_uo_citta.Text = "";
            this.txt_uo_cap.Text = "";
            this.txt_uo_prov.Text = "";
            this.txt_uo_nazione.Text = "";
            this.txt_uo_telefono1.Text = "";
            this.txt_uo_telefono2.Text = "";
            this.txt_uo_fax.Text = "";
            this.txt_uo_note.Text = "";
        }

        private void btn_annulla_ins_uo_Click(object sender, System.EventArgs e)
        {
            myTreeNode TreeNodo;
            TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);

            this.GestioneLoadDettagli(TreeNodo);
        }

        private void btn_ordinamento_Click(object sender, System.EventArgs e)
        {
            myTreeNode TreeNodo;
            TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);

            TreeNodo.Expanded = false;

            string idUo = TreeNodo.getIDCorrGlobale();
            int idLivello = Convert.ToInt32(TreeNodo.getLivello()) + 1;
            string idAmm = TreeNodo.getIDAmministrazione();
            string descUO = TreeNodo.getDescrizione();

            // open modalDialog: ApriOrdinamento(idUo,idLivello,idAmm,descUO)
            if (!ClientScript.IsStartupScriptRegistered("openOrdinamento"))
            {
                string scriptString = "<SCRIPT>ApriOrdinamento('" + idUo + "','" + idLivello + "','" + idAmm + "','" + descUO + "');</SCRIPT>";
                ClientScript.RegisterStartupScript(GetType(), "openOrdinamento", scriptString);
            }
        }

        #endregion

        #region ruolo

        private void ddl_tipo_ruolo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                this.trCalculateAtipicita.Visible = false;

                if (this.ddl_tipo_ruolo.SelectedValue != "null")
                {
                    myTreeNode TreeNodo;
                    TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
                    if (this.btn_mod_ruolo.Text == "Inserisci")
                    {
                        string tipo = this.ddl_tipo_ruolo.SelectedValue;

                        int lungID = tipo.LastIndexOf("@");
                        string codice = tipo.Substring((lungID + 1), (tipo.LastIndexOf("#") - (lungID + 1)));
                        string descrizione = tipo.Substring((tipo.LastIndexOf("#") + 1), (tipo.Length - (tipo.LastIndexOf("#") + 1)));

                        this.txt_rubricaRuolo.Text = "";
                        this.txt_descrizioneRuolo.Text = "";

                        if (txtDescUo.Visible && txtCodUo.Visible)
                        {
                            codice = codice + " " + txtCodUo.Text;
                            descrizione = descrizione + " " + txtDescUo.Text;
                        }
                        else
                        {
                            codice = codice + " " + TreeNodo.getCodiceRubrica();
                            descrizione = descrizione + " " + TreeNodo.getDescrizione();
                        }

                        this.txt_rubricaRuolo.Text = codice;
                        if (this.txt_rubricaRuolo.Text.Length > 20)
                            this.txt_rubricaRuolo.Text = codice.Substring(0, 20);

                        this.txt_descrizioneRuolo.Text = descrizione;
                        if (this.txt_descrizioneRuolo.Text.Length > 128)
                            this.txt_descrizioneRuolo.Text = descrizione.Substring(0, 128);
                    }
                    // Visualizzazione della option list per la gestione dell'estensione di visibilità
                    // ai superiori solo se non si è in inserimento
                    if (this.btn_mod_ruolo.Text != "Inserisci")
                    {
                        this.trExtendVisibility.Visible = Utils.GetAbilitazioneAtipicita();
                        this.rblExtendVisibility.SelectedIndex = 0;
                    }
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        private void btn_mod_ruolo_Click(object sender, System.EventArgs e)
        {
            RemoveSessionInterrompiProcessi();
            RemoveSessionStoricizzaProcessi();
            switch (this.btn_mod_ruolo.Text)
            {
                case "Inserisci":
                    this.InsModCanc_Ruolo("inserimento");
                    break;
                case "Modifica":
                    if(VerificaProcessiCoinvolti(this.btn_mod_ruolo.Text, (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex)))
                        this.InsModCanc_Ruolo("modifica");
                    break;
                case "Sposta":
                    if (VerificaProcessiCoinvolti(this.btn_mod_ruolo.Text, (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex)))
                        this.InsModCanc_Ruolo("Sposta");
                    break;
                default:
                    break;
            }
            
        }

        private void btn_elimina_ruolo_Click(object sender, System.EventArgs e)
        {
            if (VerificaProcessiCoinvolti("cancellazione", (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex)))
                this.InsModCanc_Ruolo("cancellazione");
        }

        private void btn_utenti_ruolo_Click(object sender, System.EventArgs e)
        {
            try
            {
                //if (VerificaProcessiCoinvolti("sposta_ruolo"))
                //{
                    myTreeNode TreeNodo;
                    TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);

                    if (TreeNodo.Nodes.Count > 0)
                    {
                        TreeNodo.Nodes.Clear();
                    }

                    Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();

                    theManager.ListaUtenti(TreeNodo.getIDGruppo());

                    if (theManager.getListaUtenti() != null && theManager.getListaUtenti().Count > 0)
                    {
                        myTreeNode nodoUtenti;

                        foreach (DocsPAWA.DocsPaWR.OrgUtente utente in theManager.getListaUtenti())
                        {
                            nodoUtenti = new myTreeNode();

                            nodoUtenti.ID = utente.IDCorrGlobale;
                            nodoUtenti.Text = utente.CodiceRubrica + " - " + utente.Cognome + " " + utente.Nome;
                            nodoUtenti.ImageUrl = "../Images/utente.gif";

                            TreeNodo.Nodes.Add(nodoUtenti);

                            nodoUtenti.setTipoNodo("P");
                            nodoUtenti.setIDCorrGlobale(utente.IDCorrGlobale);
                            nodoUtenti.setCodice(utente.Codice);
                            nodoUtenti.setCodiceRubrica(utente.CodiceRubrica);
                            nodoUtenti.setIDAmministrazione(utente.IDAmministrazione);
                        }
                    }
                    else
                    {
                        this.btn_elimina_ruolo.Visible = true;
                    }

                    //Serve per far ricaricare alla rubrica le modifiche apportate
                    DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    ws.resetHashTable();
                //}
            }
            catch (Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        private void btn_sposta_ruolo_Click(object sender, System.EventArgs e)
        {
            //this.Inizialize();

            /*
            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();

            theManager.RicercaInOrg("U", this.txt_ricCod.Text.Trim(), "", this.hd_idAmm.Value);

            if (theManager.getRisultatoRicerca() != null && theManager.getRisultatoRicerca().Count.Equals(1))
            {
                foreach (DocsPAWA.DocsPaWR.OrgRisultatoRicerca risultato in theManager.getRisultatoRicerca())
                {
                    this.txt_ricDesc.Text = risultato.Descrizione;
                    this.hd_idCorrGlobDest.Value = risultato.IDCorrGlob;

                    string[] appo2 = this.hd_tiporuolo.Value.Split('-');
                    this.txt_codNewRuolo.Text = appo2[0].Trim() + " " + this.txt_ricCod.Text;
                    this.txt_descNewRuolo.Text = appo2[1].Trim() + " " + this.txt_ricDesc.Text;

                    this.SetFocus(this.txt_codNewRuolo, false);

                    DocsPAWA.Utils.DefaultButton(this, ref txt_codNewRuolo, ref btn_sposta);
                    DocsPAWA.Utils.DefaultButton(this, ref txt_descNewRuolo, ref btn_sposta);
                }
            }*/

            // Creazione del dettaglio del ruolo da spostare
            RemoveSessionInterrompiProcessi();
            RemoveSessionStoricizzaProcessi();
            if (this.VerificaProcessiCoinvolti("sposta", (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex)))
                SpostaRuolo();
                 
        }

        private void SpostaRuolo()
        {
            // Creazione del dettaglio del ruolo da spostare
            myTreeNode treeNode;
            treeNode = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
            treeNode.setTipoNodo("R");

            OrgRuolo role = new OrgRuolo()
            {
                IDCorrGlobale = treeNode.getIDCorrGlobale(),
                IDUo = this.hdIdCorrGlob.Value,
                IDGruppo = treeNode.getIDGruppo(),
                IDTipoRuolo = this.ddl_tipo_ruolo.SelectedValue.Substring(0, this.ddl_tipo_ruolo.SelectedValue.LastIndexOf("@")),
                Codice = this.txt_rubricaRuolo.Text.Replace("'", "''").Trim(),
                CodiceRubrica = this.txt_rubricaRuolo.Text.Replace("'", "''").Trim(),
                Descrizione = this.txt_descrizioneRuolo.Text.Replace("'", "''").Trim(),
                DiRiferimento = this.chk_rif.Checked ? "1" : "0",
                Responsabile = this.chk_resp.Checked ? "1" : "0",
                Segretario = this.cb_segretario.Checked ? "1" : "0",
                DisabledTrasm = this.cb_disabled_trasm.Checked ? "1" : "0",
                IDAmministrazione = treeNode.getIDAmministrazione()
            };

            SaveChangesToRole.SaveChangesRequest = new SaveChangesToRoleRequest()
            {
                ModifiedRole = role,
                Historicize = this.chkStoricizza.Checked,
                UpdateTransModelsAssociation = this.chkUpdateModels.Checked,
                ExtendVisibility = (ExtendVisibilityOption)Enum.Parse(typeof(ExtendVisibilityOption), "N"),
                ComputeAtipicita = Utils.GetAbilitazioneAtipicita() && this.calculateAtipicitaOptions.CalculateAtipicita()

            };
            this.ClientScript.RegisterStartupScript(this.GetType(), "OpenMove", SaveChangesToRole.ScriptToOpen, true);
        }

        #endregion

        #region funzioni
        private void btn_mod_funzioni_Click(object sender, System.EventArgs e)
        {
            this.InserimentoTipoFunzioni();
        }
        #endregion

        #region registri
        private void btn_mod_registri_Click(object sender, System.EventArgs e)
        {
            if(!CheckPresenzaProcessiFirmaRegistri())
                this.InserimentoRegistri();
            else
            {
                string scriptString = "<SCRIPT>AvvisoPresenzaProcessiFirmaRegistroRf();</SCRIPT>";
                ClientScript.RegisterStartupScript(GetType(), "AvvisoPresenzaProcessiFirmaRegistroRf", scriptString);
            }
        }
        #endregion

        #region utenti

        private void btn_sposta_utente_Click(object sender, System.EventArgs e)
        {
            this.Gest_AperturaWnd_SpostaUtente(true);
            //this.Inizialize();
        }
     
        #endregion

        #endregion

        #region Inserimento / Modifica / Elimina dati

        #region UO

        /// <summary>
        /// Inserisce una nuova UO a livello 0
        /// </summary>
        private void InserimentoUO()
        {
            try
            {
                if (this.txt_rubricaUO.Text.Trim() != "" && this.txt_descrizioneUO.Text.Trim() != "")
                {
                    Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                    DocsPAWA.DocsPaWR.OrgUO newUO = new DocsPAWA.DocsPaWR.OrgUO();

                    string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                    //theManager.CurrentIDAmm(codAmm);

                    newUO.IDParent = "0";
                    newUO.Codice = this.txt_rubricaUO.Text.Replace("'", "''").Trim();
                    newUO.CodiceRubrica = this.txt_rubricaUO.Text.Replace("'", "''").Trim();
                    newUO.Descrizione = this.txt_descrizioneUO.Text.Replace("'", "''").Trim();
                    newUO.Livello = "0";
                    newUO.IDAmministrazione = idAmm;

                    if (this.chk_interopUO.Checked)
                    {
                        if (!this.ddl_aoo.SelectedItem.Value.ToString().Equals("null"))
                        {
                            newUO.CodiceRegistroInterop = this.ddl_aoo.SelectedItem.Value.ToString();
                        }
                    }

                    if (this.chkInteroperante.Checked && !String.IsNullOrEmpty(this.ddlRegistriInteropSemplificata.SelectedItem.Value) && !String.IsNullOrEmpty(this.ddlRfInteropSemplificata.SelectedItem.Value))
                    {
                        newUO.IdRegistroInteroperabilitaSemplificata = this.ddlRegistriInteropSemplificata.SelectedValue;
                        newUO.IdRfInteroperabilitaSemplificata = this.ddlRfInteropSemplificata.SelectedValue;
                    }
                    // dettagli UO
                    DocsPAWA.DocsPaWR.OrgDettagliGlobali dettagli = new DocsPAWA.DocsPaWR.OrgDettagliGlobali();
                    newUO.DettagliUo = dettagli;
                    newUO.DettagliUo.Indirizzo = this.txt_uo_indirizzo.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Citta = this.txt_uo_citta.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Cap = this.txt_uo_cap.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Provincia = this.txt_uo_prov.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Nazione = this.txt_uo_nazione.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Telefono1 = this.txt_uo_telefono1.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Telefono2 = this.txt_uo_telefono2.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Fax = this.txt_uo_fax.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Note = this.txt_uo_note.Text.Replace("'", "''").Trim();
                    //Controllo la correttezza del codice fiscale
                    if ((this.txt_uo_codice_fiscale != null && !this.txt_uo_codice_fiscale.Text.Trim().Equals("")) && ((this.txt_uo_codice_fiscale.Text.Trim().Length == 11 && Utils.CheckVatNumber(this.txt_uo_codice_fiscale.Text.Trim()) != 0) || (this.txt_uo_codice_fiscale.Text.Trim().Length == 16 && Utils.CheckTaxCode(this.txt_uo_codice_fiscale.Text.Trim()) != 0) || (this.txt_uo_codice_fiscale.Text.Trim().Length != 11 && this.txt_uo_codice_fiscale.Text.Trim().Length != 16)))
                    {
                        RegisterStartupScript("chk_fields",
                            "<script language=\"javascript\">" +
                            "alert (\"Attenzione, Codice Fiscale non corretto!\");" +
                            "</script>");
                        return;
                    }
                    else
                    newUO.DettagliUo.CodiceFiscale = this.txt_uo_codice_fiscale.Text.Replace("'", "''").Trim();
                    //Controllo la correttezza della partita iva
                    if (this.txt_uo_partita_iva != null && !this.txt_uo_partita_iva.Text.Equals("") && Utils.CheckVatNumber(this.txt_uo_partita_iva.Text) != 0)
                    {
                        RegisterStartupScript("chk_fields",
                            "<script language=\"javascript\">" +
                            "alert (\"Attenzione, Partita Iva non corretta\");" +
                            "</script>");
                        return;
                    }
                    else
                    newUO.DettagliUo.PartitaIva = this.txt_uo_partita_iva.Text.Replace("'", "''").Trim();

                    theManager.InsNuovaUO(newUO);

                    DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                    esito = theManager.getEsitoOperazione();

                    if (esito.Codice.Equals(0))
                    {
                        this.Inizialize();
                    }
                    else
                    {
                        if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                        {
                            string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                            this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                        }
                    }

                    esito = null;
                }
                else
                {
                    if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                    {
                        string scriptString = "<SCRIPT>alert('Attenzione, inserire tutti i campi obbligatori.');</SCRIPT>";
                        this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                    }

                    this.SetFocus(txt_rubricaUO);
                }

                //Serve per far ricaricare alla rubrica le modifiche apportate
                DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                ws.resetHashTable();
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// Inserisce una nuova sotto UO
        /// </summary>
        private void InserimentoSottoUO()
        {
            try
            {
                if (this.txt_rubricaUO.Text.Trim() != "" && this.txt_descrizioneUO.Text.Trim() != "")
                {
                    myTreeNode TreeNodo;
                    TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);

                    Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                    DocsPAWA.DocsPaWR.OrgUO newUO = new DocsPAWA.DocsPaWR.OrgUO();

                    string testoNodo = this.txt_rubricaUO.Text + " - " + this.txt_descrizioneUO.Text;

                    newUO.IDParent = TreeNodo.getIDCorrGlobale();
                    newUO.Codice = this.txt_rubricaUO.Text.Replace("'", "''").Trim();
                    newUO.CodiceRubrica = this.txt_rubricaUO.Text.Replace("'", "''").Trim();
                    newUO.Descrizione = this.txt_descrizioneUO.Text.Replace("'", "''").Trim();
                    newUO.Livello = (Convert.ToInt32(TreeNodo.getLivello()) + 1).ToString();
                    newUO.IDAmministrazione = TreeNodo.getIDAmministrazione();
                    if (this.chk_interopUO.Checked)
                    {
                        if (!this.ddl_aoo.SelectedItem.Value.ToString().Equals("null"))
                        {
                            newUO.CodiceRegistroInterop = this.ddl_aoo.SelectedItem.Value.ToString();
                        }
                    }

                    if (this.chkInteroperante.Checked)
                    {
                        newUO.IdRegistroInteroperabilitaSemplificata = this.ddlRegistriInteropSemplificata.SelectedValue;
                        newUO.IdRfInteroperabilitaSemplificata = this.ddlRfInteropSemplificata.SelectedValue;
                    }
                    // dettagli UO
                    DocsPAWA.DocsPaWR.OrgDettagliGlobali dettagli = new DocsPAWA.DocsPaWR.OrgDettagliGlobali();
                    newUO.DettagliUo = dettagli;
                    newUO.DettagliUo.Indirizzo = this.txt_uo_indirizzo.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Citta = this.txt_uo_citta.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Cap = this.txt_uo_cap.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Provincia = this.txt_uo_prov.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Nazione = this.txt_uo_nazione.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Telefono1 = this.txt_uo_telefono1.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Telefono2 = this.txt_uo_telefono2.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Fax = this.txt_uo_fax.Text.Replace("'", "''").Trim();
                    newUO.DettagliUo.Note = this.txt_uo_note.Text.Replace("'", "''").Trim();
                    if ((this.txt_uo_codice_fiscale != null && !this.txt_uo_codice_fiscale.Text.Trim().Equals("")) && ((this.txt_uo_codice_fiscale.Text.Trim().Length == 11 && Utils.CheckVatNumber(this.txt_uo_codice_fiscale.Text.Trim()) != 0) || (this.txt_uo_codice_fiscale.Text.Trim().Length == 16 && Utils.CheckTaxCode(this.txt_uo_codice_fiscale.Text.Trim()) != 0) || (this.txt_uo_codice_fiscale.Text.Trim().Length != 11 && this.txt_uo_codice_fiscale.Text.Trim().Length != 16)))
                    {
                        RegisterStartupScript("chk_fields",
                            "<script language=\"javascript\">" +
                            "alert (\"Attenzione, Codice Fiscale non corretto!\");" +
                            "</script>");
                        return;
                    }
                    else
                        newUO.DettagliUo.CodiceFiscale = this.txt_uo_codice_fiscale.Text.Replace("'", "''").Trim();
                    //Controllo la correttezza della partita iva
                    if (this.txt_uo_partita_iva != null && !this.txt_uo_partita_iva.Text.Equals("") && Utils.CheckVatNumber(this.txt_uo_partita_iva.Text) != 0)
                    {
                        RegisterStartupScript("chk_fields",
                            "<script language=\"javascript\">" +
                            "alert (\"Attenzione, Partita Iva non corretta!\");" +
                            "</script>");
                        return;
                    }
                    else
                        newUO.DettagliUo.PartitaIva = this.txt_uo_partita_iva.Text.Replace("'", "''").Trim();

                    theManager.InsNuovaUO(newUO);

                    DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                    esito = theManager.getEsitoOperazione();

                    if (esito.Codice.Equals(0))
                    {

                        //Emanuela: aggiungo il segunte codice per la gestione della fatturazione
                        //VERIFICO SE L'AMMINISTRAZIONE SIA ABILITATA ALLA FATTURAZIONE
                        //VADO A VERIFICARE SE L'UO AGGIORNATA è PRESENTE NELLA TABELLA TIBCO; IN CASO AGGIORNO LA TABELLA
                        if (true/* && chiava abilitata*/)
                        {
                            DocsPAWA.DocsPaWR.Amministrazione amm = new DocsPAWA.DocsPaWR.Amministrazione();
                            bool result = false;
                            string oldCodiceUO = string.Empty;
                            amm = theManager.ModificaUoTIBCO(oldCodiceUO, newUO, out result);
                            //Se risultato è true implica che la UO non è stata trovata nella tabella TIBCO e quindi occorre inviare l'email al referente
                            if (result && amm!=null)
                            {
                                //INVIO MAIL
                                string descrizioneAOO = this.ddl_aoo.SelectedItem.Text.ToString();
                                theManager.inviaNotificaMail(newUO, amm, descrizioneAOO, "I","");                            
                            }
                        }
                        // fine Emanuela

                        this.AggiornaTreeviewDopoIns();
                        TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
                        foreach (myTreeNode nodo in TreeNodo.Nodes)
                        {
                            if (nodo.Text == testoNodo)
                            {
                                treeViewUO.SelectedNodeIndex = nodo.GetNodeIndex();
                                this.GestioneLoadDettagli(nodo);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                        {
                            string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                            this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                        }
                    }

                    esito = null;
                }
                else
                {
                    if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                    {
                        string scriptString = "<SCRIPT>alert('Attenzione, inserire tutti i campi obbligatori.');</SCRIPT>";
                        this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                    }

                    this.SetFocus(this.txt_rubricaUO);
                }

                //Serve per far ricaricare alla rubrica le modifiche apportate
                DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                ws.resetHashTable();
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// Modifica dati UO
        /// </summary>
        private void ModificaUO(bool storicizza)
        {
            try
            {
                bool bloccaesecuzione = false;
                if (Session["interopUOChanged"] != null && (bool)Session["interopUOChanged"])
                {
                    if (Session["UOinSessione"] != null)
                    {
                        string descrUO = (string)Session["descUO"];
                        DocsPAWA.DocsPaWR.OrgDettagliGlobali dettagli = (DocsPAWA.DocsPaWR.OrgDettagliGlobali)Session["UOinSessione"];
                        if (
                            (this.txt_uo_indirizzo.Text != dettagli.Indirizzo) ||
                            (this.txt_uo_citta.Text != dettagli.Citta) ||
                            (this.txt_uo_cap.Text != dettagli.Cap) ||
                            (this.txt_uo_prov.Text != dettagli.Provincia) ||
                            (this.txt_uo_nazione.Text != dettagli.Nazione) ||
                            (this.txt_uo_telefono1.Text != dettagli.Telefono1) ||
                            (this.txt_uo_telefono2.Text != dettagli.Telefono2) ||
                            (this.txt_uo_fax.Text != dettagli.Fax) ||
                            (this.txt_descrizioneUO.Text != descrUO) ||
                            (this.txt_uo_note.Text != dettagli.Note) ||
                            (this.txt_uo_codice_fiscale.Text != dettagli.CodiceFiscale) ||
                            (this.txt_uo_partita_iva.Text != dettagli.PartitaIva)
                            )
                            bloccaesecuzione = false;
                        else
                        {
                            if (this.chk_interopUO.Checked)
                            {
                                if (this.ddl_aoo.SelectedIndex != ddl_aoo.Items.IndexOf(ddl_aoo.Items.FindByValue("null")))
                                    bloccaesecuzione = false;
                                else
                                    bloccaesecuzione = true;
                            }
                            else
                                bloccaesecuzione = false;
                        }

                        if (!bloccaesecuzione)
                        {
                            Session.Remove("UOinSessione");
                            Session.Remove("interopUOChanged");
                        }
                    }
                    if (!bloccaesecuzione)
                    {
                        Session["rememberOldDDL_AOO"] = Session["oldDDL_AOO"];
                        Session.Remove("oldDDL_AOO");
                        Session.Remove("UOinterop");
                    }
                }
                else
                {
                    if (Session["UOinSessione"] != null)                
                    {
                        string descrUO = (string)Session["descUO"];
                        DocsPAWA.DocsPaWR.OrgDettagliGlobali dettagli = (DocsPAWA.DocsPaWR.OrgDettagliGlobali)Session["UOinSessione"];
                        if (
                            (this.txt_uo_indirizzo.Text != dettagli.Indirizzo) ||
                            (this.txt_uo_citta.Text != dettagli.Citta) ||
                            (this.txt_uo_cap.Text != dettagli.Cap) ||
                            (this.txt_uo_prov.Text != dettagli.Provincia) ||
                            (this.txt_uo_nazione.Text != dettagli.Nazione) ||
                            (this.txt_uo_telefono1.Text != dettagli.Telefono1) ||
                            (this.txt_uo_telefono2.Text != dettagli.Telefono2) ||
                            (this.txt_uo_fax.Text != dettagli.Fax) ||
                            (this.txt_descrizioneUO.Text != descrUO) ||
                            (this.txt_uo_note.Text != dettagli.Note) ||
                            (this.txt_uo_codice_fiscale.Text != dettagli.CodiceFiscale) ||
                            (this.txt_uo_partita_iva.Text != dettagli.PartitaIva)
                            )
                            bloccaesecuzione = false;
                        else
                            bloccaesecuzione = true;

                        if (!bloccaesecuzione)
                            Session.Remove("UOinSessione");
                    }
                    if (!bloccaesecuzione)
                    {
                        Session["rememberOldDDL_AOO"] = Session["oldDDL_AOO"];
                        Session.Remove("oldDDL_AOO");
                        Session.Remove("UOinterop");
                    }
                }

                if (Session["cambiaCodUO"] != null)
                {
                    Session.Remove("cambioCodUO");
                    bloccaesecuzione = false;
                    this.btn_ordinamento.Enabled = true;
                    this.btn_annulla_ins_uo.Enabled = true;
                    this.btn_clear_uo.Enabled = true;
                    this.btn_elimina_uo.Enabled = true;
                    this.btn_importaOrganigramma.Enabled = true;
                    this.btn_sposta_uo.Enabled = true;
                    this.btn_ins_ruolo_uo.Enabled = true;
                    this.btn_ins_sotto_uo.Enabled = true;
                }


                # region codice commentato
                //if (storicizza)
                //{
                //    if (Session["interopUOChanged"] != null && (bool)Session["interopUOChanged"])
                //    {
                //        if (Session["UOinSessione"] != null)
                //        {
                //            string descrUO = (string)Session["descUO"];
                //            DocsPAWA.DocsPaWR.OrgDettagliGlobali dettagli = (DocsPAWA.DocsPaWR.OrgDettagliGlobali)Session["UOinSessione"];
                //            if (
                //                (this.txt_uo_indirizzo.Text != dettagli.Indirizzo) ||
                //                (this.txt_uo_citta.Text != dettagli.Citta) ||
                //                (this.txt_uo_cap.Text != dettagli.Cap) ||
                //                (this.txt_uo_prov.Text != dettagli.Provincia) ||
                //                (this.txt_uo_nazione.Text != dettagli.Nazione) ||
                //                (this.txt_uo_telefono1.Text != dettagli.Telefono1) ||
                //                (this.txt_uo_telefono2.Text != dettagli.Telefono2) ||
                //                (this.txt_uo_fax.Text != dettagli.Fax) ||
                //                (this.txt_descrizioneUO.Text != descrUO)
                //                )
                //                noStoricizzUO = false;
                //            else
                //                noStoricizzUO = true;
                //        }
                //        Session.Remove("UOinSessione");
                //        Session.Remove("interopUOChanged");
                //    }
                //    Session.Remove("oldDDL_AOO");
                //    Session.Remove("UOinterop");
                //}
                //else
                //{

                //}

                //if (Session["interopUOChanged"] != null && (bool)Session["interopUOChanged"])
                //{
                //    if (Session["UOinSessione"] != null)
                //    {
                //        string descrUO = (string)Session["descUO"];
                //        DocsPAWA.DocsPaWR.OrgDettagliGlobali dettagli = (DocsPAWA.DocsPaWR.OrgDettagliGlobali)Session["UOinSessione"];
                //        if (
                //            (this.txt_uo_indirizzo.Text != dettagli.Indirizzo) ||
                //            (this.txt_uo_citta.Text != dettagli.Citta) ||
                //            (this.txt_uo_cap.Text != dettagli.Cap) ||
                //            (this.txt_uo_prov.Text != dettagli.Provincia) ||
                //            (this.txt_uo_nazione.Text != dettagli.Nazione) ||
                //            (this.txt_uo_telefono1.Text != dettagli.Telefono1) ||
                //            (this.txt_uo_telefono2.Text != dettagli.Telefono2) ||
                //            (this.txt_uo_fax.Text != dettagli.Fax) ||
                //            (this.txt_descrizioneUO.Text != descrUO)
                //            )
                //            noStoricizzUO = false;
                //        else
                //            noStoricizzUO = true;
                //    }
                //    Session.Remove("UOinSessione");
                //    Session.Remove("interopUOChanged");
                //}
                //Session.Remove("oldDDL_AOO");
                //Session.Remove("UOinterop");
                # endregion

                try
                {
                    myTreeNode TreeNodo;
                    TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
                    bloccaesecuzione &= this.chkInteroperante.Checked && TreeNodo.RegistryInteropSemplificata == this.ddlRegistriInteropSemplificata.SelectedValue &&
                        TreeNodo.RfInteropSemplificata == this.ddlRfInteropSemplificata.SelectedValue;


                }
                catch {}

                if (!bloccaesecuzione)
                {
                    if (this.txt_rubricaUO.Text.Trim() != "" && this.txt_descrizioneUO.Text.Trim() != "")
                    {
                        myTreeNode TreeNodo;
                        TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
                        Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                        DocsPAWA.DocsPaWR.OrgUO theUO = new DocsPAWA.DocsPaWR.OrgUO();

                        string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                        theUO.IDCorrGlobale = TreeNodo.getIDCorrGlobale();
                        theUO.Codice = this.txt_rubricaUO.Text.Replace("'", "''").Trim();
                        theUO.CodiceRubrica = this.txt_rubricaUO.Text.Replace("'", "''").Trim();
                        theUO.Descrizione = this.txt_descrizioneUO.Text.Replace("'", "''").Trim();
                        theUO.IDAmministrazione = idAmm;
                        theUO.CodiceRegistroInterop = null;
                        if (this.chk_interopUO.Checked)
                        {
                            if (!this.ddl_aoo.SelectedItem.Value.ToString().Equals("null"))
                                theUO.CodiceRegistroInterop = this.ddl_aoo.SelectedItem.Value;
                        }

                        if (this.chkInteroperante.Checked && !this.ddlRegistriInteropSemplificata.SelectedItem.Value.ToString().Equals("null"))
                        {
                            theUO.IdRegistroInteroperabilitaSemplificata = this.ddlRegistriInteropSemplificata.SelectedValue;
                            theUO.IdRfInteroperabilitaSemplificata = this.ddlRfInteropSemplificata.SelectedValue;
                        }

                        // dettagli UO
                        DocsPAWA.DocsPaWR.OrgDettagliGlobali dettagli = new DocsPAWA.DocsPaWR.OrgDettagliGlobali();
                        theUO.DettagliUo = dettagli;
                        theUO.DettagliUo.Indirizzo = this.txt_uo_indirizzo.Text.Replace("'", "''").Trim();
                        theUO.DettagliUo.Citta = this.txt_uo_citta.Text.Replace("'", "''").Trim();
                        theUO.DettagliUo.Cap = this.txt_uo_cap.Text.Replace("'", "''").Trim();
                        theUO.DettagliUo.Provincia = this.txt_uo_prov.Text.Replace("'", "''").Trim();
                        theUO.DettagliUo.Nazione = this.txt_uo_nazione.Text.Replace("'", "''").Trim();
                        theUO.DettagliUo.Telefono1 = this.txt_uo_telefono1.Text.Replace("'", "''").Trim();
                        theUO.DettagliUo.Telefono2 = this.txt_uo_telefono2.Text.Replace("'", "''").Trim();
                        theUO.DettagliUo.Fax = this.txt_uo_fax.Text.Replace("'", "''").Trim();
                        theUO.DettagliUo.Note = this.txt_uo_note.Text.Replace("'", "''").Trim();
                        //Controllo se il codice fiscale è valido
                        if ((this.txt_uo_codice_fiscale != null && !this.txt_uo_codice_fiscale.Text.Trim().Equals("")) && ((this.txt_uo_codice_fiscale.Text.Trim().Length == 11 && Utils.CheckVatNumber(this.txt_uo_codice_fiscale.Text.Trim()) != 0) || (this.txt_uo_codice_fiscale.Text.Trim().Length == 16 && Utils.CheckTaxCode(this.txt_uo_codice_fiscale.Text.Trim()) != 0) || (this.txt_uo_codice_fiscale.Text.Trim().Length != 11 && this.txt_uo_codice_fiscale.Text.Trim().Length != 16)))
                        {
                            RegisterStartupScript("chk_fields",
                                "<script language=\"javascript\">" +
                                "alert (\"Attenzione, Codice Fiscale non corretto!\");" +
                                "</script>");
                            return;
                        }
                        else
                            theUO.DettagliUo.CodiceFiscale = this.txt_uo_codice_fiscale.Text.Replace("'", "''").Trim();
                        //Controllo se la partita iva iva è valida
                        if (this.txt_uo_partita_iva!=null && !this.txt_uo_partita_iva.Text.Equals("") && Utils.CheckVatNumber(this.txt_uo_partita_iva.Text) != 0)
                        {
                            RegisterStartupScript("chk_fields",
                                "<script language=\"javascript\">" +
                                "alert (\"Attenzione, Partita Iva non corretta!\");" +
                                "</script>");
                            return;
                        }
                        else
                            theUO.DettagliUo.PartitaIva = this.txt_uo_partita_iva.Text.Replace("'", "''").Trim();
                        

                        // Modifica Palumbo per controllare storicizzazione UO in presenza di codice UO già utilizzato
                        if (storicizza) 
                        {
                            if (theManager.CheckCodiceUODuplicato(theUO.IDCorrGlobale, theUO.Codice, theUO.IDAmministrazione))
                            {
                                if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                                {
                                    string scriptString = "<SCRIPT>alert('Attenzione, codice UO " + theUO.Codice + " già esistente.');</SCRIPT>";
                                    this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                                }
                                this.SetFocus(this.txt_rubricaUO);
                                return;
                            }
                        } 

                        theManager.ModUO(theUO, storicizza);

                        DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                        esito = theManager.getEsitoOperazione();

                        if (esito.Codice.Equals(0))
                        {   
                            //Emanuela: aggiungo il seguente codice per la gestione della fatturazione
                            //VERIFICO SE L'AMMINISTRAZIONE SIA ABILITATA ALLA FATTURAZIONE
                            //VADO A VERIFICARE SE L'UO AGGIORNATA è PRESENTE NELLA TABELLA TIBCO; IN CASO AGGIORNO LA TABELLA
                            string oldCodiceUO = TreeNodo.getCodiceRubrica();
                            string newCodiceUo = theUO.Codice;
                            if (!oldCodiceUO.Equals(newCodiceUo) && !storicizza/* && chiava abilitata*/)
                            {
                                DocsPAWA.DocsPaWR.Amministrazione amm = new DocsPAWA.DocsPaWR.Amministrazione(); 
                                bool result = false;
                                amm=theManager.ModificaUoTIBCO(oldCodiceUO, theUO, out result);

                                //Se result è true vuol dire che l'UO è presente nella tabella TIBCO ed è stata modificata
                                // devo quindi notificare via email
                                if (result && amm!=null)
                                {
                                    //INVIO MAIL
                                    string descrizioneAOO = this.ddl_aoo.SelectedItem.Text.ToString();
                                    theManager.inviaNotificaMail(theUO, amm, descrizioneAOO, "M", oldCodiceUO);
                                }
                            }
                            //Emanuela

                            this.AggiornaTreeviewDopoMod(theUO.IDCorrGlobale);
                            String selectedNode = this.treeViewUO.SelectedNodeIndex;
                            Inizialize();
                            RicercaNodo(TreeNodo.getIDCorrGlobale() + "_" + TreeNodo.Parent.ToString());
                            TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(selectedNode);
                            treeViewUO.SelectedNodeIndex = selectedNode;
                            this.GestioneLoadDettagli(TreeNodo);

                            //if(theUO.Livello != null)
                            //    if (theUO.Livello.Equals("1") || theUO.Livello.Equals("2"))
                            //    {
                            if (this.GestioneRubricaComuneAbilitata)
                            {
                                // verifico che la UO sia stata già precedentemente inviata in rubrica comune
                                DocsPAWA.AdminTool.Manager.SessionManager sessionMng = new DocsPAWA.AdminTool.Manager.SessionManager();
                                RubricaComune.Proxy.Elementi.ElementoRubrica elemento = DocsPAWA.RubricaComune.RubricaServices.GetElementoRubricaUO(sessionMng.getUserAmmSession(), theUO.IDCorrGlobale);
                                if (elemento != null)
                                {
                                    string scriptString = "<SCRIPT>alert('Attenzione, la UO è già stata pubblicata in rubrica comune; è necessario ripetere la pubblicazione!');</SCRIPT>";
                                    this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                                }
                            }
                        }
                        else
                        {
                            if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                            {
                                string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                                this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                            }
                        }

                        esito = null;

                        //Serve per far ricaricare alla rubrica le modifiche apportate
                        DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                        ws.resetHashTable();
                    }
                    else
                    {
                        if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                        {
                            string scriptString = "<SCRIPT>alert('Attenzione, inserire tutti i campi obbligatori.');</SCRIPT>";
                            this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                        }

                        this.SetFocus(this.txt_rubricaUO);
                    }
                }
                else
                {
                    if (storicizza)
                        RegisterClientScriptBlock("avvisoNoStoricizza", "<script>alert('Operazione di storicizzazione UO non terminata in quanto non risultano modifiche rilevanti');</script>");
                    else
                        RegisterClientScriptBlock("avvisoNoStoricizza", "<script>alert('Operazione di modifica UO non terminata in quanto non risultano modifiche rilevanti');</script>");
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// Elimina UO
        /// </summary>
        private void EliminaUO()
        {
            try
            {
                myTreeNode TreeNodo;
                TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);

                DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();

                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.EliminaUO(sessionManager.getUserAmmSession(), TreeNodo.getIDCorrGlobale());

                DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                esito = theManager.getEsitoOperazione();

                if (esito.Codice.Equals(0))
                {
                    DocsPAWA.DocsPaWR.OrgUO theUO = new DocsPAWA.DocsPaWR.OrgUO();
                    theUO.Codice = TreeNodo.getCodiceRubrica(); ;
                    theUO.Descrizione = (string)Session["descUO"];
                    if (!this.ddl_aoo.SelectedItem.Value.ToString().Equals("null"))
                        theUO.CodiceRegistroInterop = this.ddl_aoo.SelectedItem.Value;
                    theUO.IDAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                    //Comunico al referente IPA che la UO è stat rimossa
                    DocsPAWA.DocsPaWR.Amministrazione amm = new DocsPAWA.DocsPaWR.Amministrazione();

                    

                    //Emanuela:Eliminazione della UO nella tabella di supporto alla fatturazione
                    bool result = false;
                    amm = theManager.eliminaUoTIBCO(theUO, out result);

                    //se l'eliminazione nella tabella è stata effettuata notifico via mail
                    if(result)
                    {
                        string descrizioneAOO = this.ddl_aoo.SelectedItem.Text.ToString();
                        theManager.inviaNotificaMail(theUO, amm, descrizioneAOO ,"C","");
                    }
                    // Rimozione elemento in rubrica comune

                    // 

                    this.AggiornaTreeviewDopoCanc("U");
                    DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    DataSet dsListe = wws.isCorrInListaDistr(TreeNodo.getIDCorrGlobale());
                    string messaggio = string.Empty;
                    if (dsListe != null)
                    {
                        if (dsListe.Tables.Count > 0)
                        {
                            DataTable tab = dsListe.Tables[0];
                            if (tab.Rows.Count > 0)
                            {
                                messaggio += "Attenzione, UO presente nelle seguenti liste di distribuzione\\n";
                                for (int i = 0; i < tab.Rows.Count; i++)
                                {
                                    messaggio += tab.Rows[i]["var_desc_corr"].ToString();
                                    if (!string.IsNullOrEmpty(tab.Rows[i]["prop"].ToString()))
                                        messaggio += " creata da " + tab.Rows[i]["prop"].ToString();
                                    else
                                        if (!string.IsNullOrEmpty(tab.Rows[i]["ruolo"].ToString()))
                                            messaggio += " creata per il ruolo " + tab.Rows[i]["ruolo"].ToString();
                                    messaggio += "\\n";
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(messaggio))
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + messaggio + "');", true);
                    }
                }
                else
                {
                    if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                    {
                        string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                        this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                    }
                }

                esito = null;

                //Serve per far ricaricare alla rubrica le modifiche apportate
                DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                ws.resetHashTable();
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        private bool isOrdinamentoAbilitato()
        {
            return (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ORDINAMENTO_ORGANIGRAMMA"]) && System.Configuration.ConfigurationManager.AppSettings["ORDINAMENTO_ORGANIGRAMMA"].Equals("1"));
        }

        #endregion

        #region ruolo

        /// <summary>
        /// Inserimento Ruolo
        /// </summary>
        /// <param name="tipoAzione">valori: modifica / inserimento / cancellazione</param>
        private void InsModCanc_Ruolo(string tipoAzione)
        {
            bool showAlert = false;
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();

            try
            {
                this.ddl_tipo_ruolo.Enabled = true;
                myTreeNode TreeNodo;

                TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
                if (!TreeNodo.Expanded && TreeNodo.getTipoNodo() != null && TreeNodo.getTipoNodo().Equals("U"))
                {
                    Microsoft.Web.UI.WebControls.TreeViewClickEventArgs e = new TreeViewClickEventArgs(treeViewUO.SelectedNodeIndex);
                    this.treeViewUO_Expand(null, e);
                }
                TreeNodo.setTipoNodo("R");
                string idCorrGlobale = !string.IsNullOrEmpty(TreeNodo.getIDCorrGlobale()) ? TreeNodo.getIDCorrGlobale() : string.Empty;

                bool unicoRuoloRespUo = this.isUnicoRuoloRespUo(TreeNodo, tipoAzione);
                bool unicoRuoloSegretarioUo = this.isUnicoRuoloSegretarioUo(TreeNodo, tipoAzione);

                //List<string> idPassi;
                //statoRuoloInLibroFirma statoInLF = StatoRuoloInLibroFirma(TreeNodo, out idPassi);

                //if (statoInLF != statoRuoloInLibroFirma.IN_LIBRO_FIRMA_ATTIVO)
                //{
                    if (unicoRuoloRespUo && unicoRuoloSegretarioUo)
                    {
                        if (this.ddl_tipo_ruolo.SelectedValue != "null" && this.txt_rubricaRuolo.Text.Trim() != "" && this.txt_descrizioneRuolo.Text.Trim() != "")
                        {
                            //myTreeNode TreeNodo;
                            //TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);

                            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                            DocsPAWA.DocsPaWR.OrgRuolo ruolo = new DocsPAWA.DocsPaWR.OrgRuolo();

                            string testoNodo = this.txt_rubricaRuolo.Text + " - " + this.txt_descrizioneRuolo.Text;

                            if (!tipoAzione.Equals("inserimento"))// nel caso di inserimento risulterà NULL
                            {
                                ruolo.IDCorrGlobale = TreeNodo.getIDCorrGlobale();
                                ruolo.IDUo = this.hdIdCorrGlob.Value;
                            }
                            else
                                ruolo.IDUo = TreeNodo.getIDCorrGlobale(); // nel caso di inserimento rappresenta la UO selezionata nella treeview

                            if (!tipoAzione.Equals("inserimento"))// nel caso di inserimento risulterà NULL
                            {
                                ruolo.IDGruppo = TreeNodo.getIDGruppo();
                            }
                            ruolo.IDTipoRuolo = this.ddl_tipo_ruolo.SelectedValue.Substring(0, this.ddl_tipo_ruolo.SelectedValue.LastIndexOf("@"));
                            ruolo.Codice = this.txt_rubricaRuolo.Text.Replace("'", "''").Trim();
                            ruolo.CodiceRubrica = this.txt_rubricaRuolo.Text.Replace("'", "''").Trim();
                            ruolo.Descrizione = this.txt_descrizioneRuolo.Text.Replace("'", "''").Trim();

                            if (this.chk_rif.Checked)
                                ruolo.DiRiferimento = "1";
                            else
                                ruolo.DiRiferimento = "0";

                            if (this.chk_resp.Checked)
                                ruolo.Responsabile = "1";
                            else
                                ruolo.Responsabile = "0";

                            if (this.cb_segretario.Checked)
                                ruolo.Segretario = "1";
                            else
                                ruolo.Segretario = "0";

                            if (this.cb_disabled_trasm.Checked)
                                ruolo.DisabledTrasm = "1";
                            else
                                ruolo.DisabledTrasm = "0";

                            ruolo.IDAmministrazione = TreeNodo.getIDAmministrazione();

                            TreeNodo.setIDTipoRuolo(ruolo.IDTipoRuolo);
                            TreeNodo.setDiRiferimento(ruolo.DiRiferimento);
                            TreeNodo.setResponsabile(ruolo.Responsabile);
                            TreeNodo.setSegretario(ruolo.Segretario);
                            TreeNodo.setCodice(ruolo.Codice);
                            TreeNodo.setCodiceRubrica(ruolo.CodiceRubrica);
                            TreeNodo.setDescrizione(ruolo.Descrizione);
                            TreeNodo.setDisabledTrasm(ruolo.DisabledTrasm);

                            if (!string.IsNullOrEmpty(ruolo.IDGruppo))
                                TreeNodo.setIDGruppo(ruolo.IDGruppo);
                            else
                                TreeNodo.setIDGruppo("0");
                            if (!string.IsNullOrEmpty(ruolo.IDCorrGlobale))
                                TreeNodo.setIDCorrGlobale(ruolo.IDCorrGlobale);
                            else
                                TreeNodo.setIDCorrGlobale("0");

                            switch (tipoAzione)
                            {
                                case "inserimento":
                                    // Se è attiva la funzionalità di atipicità viene mostrata la finestra
                                    // per il calcolo della atipicità altrimenti tutto continua a funzionare come prima
                                    if (Utils.GetAbilitazioneAtipicita())
                                    {
                                        if (UserManager.getListaCorrispondenti(this, new AddressbookQueryCorrispondente() { codiceRubrica = ruolo.Codice, tipoUtente = AddressbookTipoUtente.INTERNO }).Length > 0)
                                        {
                                        }
                                        CalculateAtipicitaAfterInsertRole.Role = ruolo;
                                        this.ClientScript.RegisterStartupScript(
                                            this.GetType(), "OpenCalcolaAtipicita", CalculateAtipicitaAfterInsertRole.GetOpenScript(ruolo.IDUo),
                                            true);
                                    }
                                    else
                                        theManager.InsNuovoRuolo(ruolo, false);

                                    break;

                                case "modifica":
                                    //theManager.ModRuolo(ruolo);
                                    SaveChangesToRole.SaveChangesRequest = new SaveChangesToRoleRequest()
                                    {
                                        ModifiedRole = ruolo,
                                        Historicize = this.chkStoricizza.Checked,
                                        UpdateTransModelsAssociation = this.chkUpdateModels.Checked,
                                        ExtendVisibility = this.trExtendVisibility.Visible ? (ExtendVisibilityOption)Enum.Parse(
                                            typeof(ExtendVisibilityOption), this.rblExtendVisibility.SelectedValue) : ExtendVisibilityOption.N
                                    };

                                    this.ClientScript.RegisterStartupScript(this.GetType(), "OpenMove", SaveChangesToRole.ScriptToOpen, true);
                                return;
                                    break;


                                /* case "cancellazione":
                                     theManager.EliminaRuolo(ruolo);
                                     break;*/
                                case "cancellazione":
                                    if (this.hd_DisableRole.Value == null)
                                        this.hd_DisableRole.Value = string.Empty;
                                    if (this.hd_DisableRole.Value.Equals("disable"))
                                    {
                                        this.hd_DisableRole.Value = string.Empty;
                                        theManager.EliminaRuolo(ruolo);
                                    }
                                    else if (this.hd_DisableRole.Value.Equals("no_disable"))
                                    {
                                        this.hd_DisableRole.Value = string.Empty;
                                        /*if (!this.Page.IsStartupScriptRegistered("alertnonDisabilita"))
                                        {
                                            string message = "<script language=\"javascript\">window.alert('OK non ho disabilitato nulla');</script>";
                                            ClientScript.RegisterStartupScript(typeof(Page), "alertnonDisabilita", message);
                                        }*/
                                        return;
                                    }
                                    else
                                    {
                                        theManager.OnlyDisabledRole(ruolo);
                                        DocsPAWA.DocsPaWR.EsitoOperazione res = new DocsPAWA.DocsPaWR.EsitoOperazione();
                                        res = theManager.getEsitoOperazione();
                                        if (res.Codice.Equals(1) || res.Codice.Equals(2) || res.Codice.Equals(9))
                                        {
                                            if (!this.Page.IsStartupScriptRegistered("confirmDisable"))
                                            {
                                                string message = "<script language=\"javascript\">if(window.confirm('Attenzione, " + res.Descrizione.Replace("'", "''") + "'))";
                                                message += "\n{document.getElementById('hd_DisableRole').value=\"disable\";}";
                                                message += "\nelse{document.getElementById('hd_DisableRole').value=\"no_disable\";}";
                                                message += "\nvar button_del_role = document.getElementById('btn_elimina_ruolo');";
                                                message += "\nbutton_del_role.click();</script>";
                                                ClientScript.RegisterStartupScript(typeof(Page), "confirmDisable", message);
                                            }
                                            return;
                                        }
                                        else 
                                        {                                     
                                             theManager.EliminaRuolo(ruolo);
                                        }
                                    }
                                    break;
                            }

                            DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                            esito = theManager.getEsitoOperazione();
                            switch (tipoAzione)
                            {
                                case "inserimento":
                                    if (!Utils.GetAbilitazioneAtipicita())
                                    {
                                        if (esito.Codice.Equals(0))
                                        {
                                            this.AggiornaTreeviewDopoIns();
                                            this.SelezionaNodoByText(testoNodo);
                                            this.btn_utenti_ruolo.Visible = true;

                                            String selectedNode = this.treeViewUO.SelectedNodeIndex;
                                            Inizialize();
                                            RicercaNodo(TreeNodo.getIDCorrGlobale() + "_" + TreeNodo.Parent.ToString());
                                            TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(selectedNode);
                                            treeViewUO.SelectedNodeIndex = selectedNode;
                                            this.GestioneLoadDettagli(TreeNodo);
                                        }
                                        else
                                        {
                                            // codice errori : 1,2,3
                                            showAlert = true;
                                            if (esito.Codice.Equals(4))
                                            {
                                                TreeNodo.setDisabledTrasm("0");
                                                cb_disabled_trasm.Checked = false;
                                            }
                                            if (esito.Codice.Equals(1))
                                            {
                                                TreeNodo.setIDCorrGlobale(idCorrGlobale);
                                            }
                                        }
                                    }
                                    break;
                                case "modifica":
                                    if (esito.Codice.Equals(0))
                                    {
                                    this.AggiornaTreeviewDopoMod("");
                                    }
                                    else
                                    {
                                        // codice errori : 3
                                        showAlert = true;
                                        if (esito.Codice.Equals(4))
                                        {
                                            Ruolo r = DocsPAWA.UserManager.getRuoloById(ruolo.IDCorrGlobale, this);
                                            if (r.disabledTrasm)
                                            {
                                                TreeNodo.setDisabledTrasm("1");
                                                cb_disabled_trasm.Checked = true;
                                                TreeNodo.setDiRiferimento("0");
                                                chk_rif.Checked = false;
                                            }
                                            else
                                            {
                                                TreeNodo.setDisabledTrasm("0");
                                                cb_disabled_trasm.Checked = false;
                                                TreeNodo.setDiRiferimento("1");
                                                chk_rif.Checked = true;
                                            }
                                        }
                                    }
                                    break;
                                case "cancellazione":
                                    if (esito.Codice.Equals(0) || esito.Codice.Equals(1) || esito.Codice.Equals(2) || esito.Codice.Equals(9))
                                    {
                                        //wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                                        DataSet dsListe = wws.isCorrInListaDistr(TreeNodo.getIDCorrGlobale());
                                        string messaggio = string.Empty;
                                        if (dsListe != null)
                                        {
                                            if (dsListe.Tables.Count > 0)
                                            {
                                                DataTable tab = dsListe.Tables[0];
                                                if (tab.Rows.Count > 0)
                                                {
                                                    messaggio += "Attenzione, ruolo presente nelle seguenti liste di distribuzione\\n";
                                                    for (int i = 0; i < tab.Rows.Count; i++)
                                                    {
                                                        messaggio += tab.Rows[i]["var_desc_corr"].ToString();
                                                        if (!string.IsNullOrEmpty(tab.Rows[i]["prop"].ToString()))
                                                            messaggio += " creata da " + tab.Rows[i]["prop"].ToString();
                                                        else
                                                            if (!string.IsNullOrEmpty(tab.Rows[i]["ruolo"].ToString()))
                                                                messaggio += " creata per il ruolo " + tab.Rows[i]["ruolo"].ToString();
                                                        messaggio += "\\n";
                                                    }
                                                }
                                            }

                                            if (!string.IsNullOrEmpty(messaggio))
                                            {
                                                showAlert = true;
                                                esito.Descrizione += "\\n " + messaggio;
                                            }
                                        }
                                        this.AggiornaTreeviewDopoCanc("R");
                                        AsyncCallback callback = new AsyncCallback(CallBack);
                                        invalidaPassiCorrelati = new InvalidaPassiCorrelatiDelegate(InvalidaPassiCorrelati);
                                        DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                                        invalidaPassiCorrelati.BeginInvoke("R", ruolo.IDGruppo, string.Empty, sessionManager.getUserAmmSession(), callback, null);
                                }
                                    else
                                    {
                                        // codice errori : 3,4,5,6,7,8,10(errore nel metodo AmmOnlyDisabledRole)
                                        if (!esito.Codice.Equals(1) && (!esito.Codice.Equals(2)) && (!esito.Codice.Equals(9)))
                                            showAlert = true;
                                    }
                                    break;
                            }


                            if (showAlert)
                            {
                                if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                                {
                                    string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                                    this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                                }
                            }

                            esito = null;
                        }
                        else
                        {
                            if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                            {
                                string scriptString = "<SCRIPT>alert('Attenzione, inserire tutti i campi obbligatori.');</SCRIPT>";
                                this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                            }

                            this.SetFocus(this.txt_rubricaRuolo);
                        }

                        //Serve per far ricaricare alla rubrica le modifiche apportate
                        DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                        ws.resetHashTable();
                    }
                    else
                    {
                        if (!unicoRuoloSegretarioUo)
                        {
                            string scriptString = "<SCRIPT>alert('Attenzione, esiste già un ruolo segretario per questa UO');</SCRIPT>";
                            this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                        }
                        else
                        {
                            if (!unicoRuoloRespUo)
                            {
                                string scriptString = "<SCRIPT>alert('Attenzione, esiste già un ruolo responsabile per questa UO');</SCRIPT>";
                                this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                            }
                        }
                    }
                //}
                //else
                //{
                //    //Il ruolo è coinvolto in un processo attino non concluso.
                //    string scriptString = "<SCRIPT>alert('Impossibile apportare le modifiche, il ruolo è convolto in un processo di firma attivo.');</SCRIPT>";
                //    this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                //}
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        private void InvalidaPassiCorrelati(string tipoNodo, string idRuolo, string idPeople, InfoUtenteAmministratore infoAmm)
        {
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            wws.Timeout = System.Threading.Timeout.Infinite;
            wws.InvalidaPassiCorrelatiTitolare(idRuolo, idPeople, tipoNodo, infoAmm);
        }

        public delegate void InvalidaPassiCorrelatiDelegate(string tipoNodo, string idRuolo, string idPeople, InfoUtenteAmministratore infoAmm);

        private void CallBack(IAsyncResult result)
        {
            invalidaPassiCorrelati.EndInvoke(result);
        }

        private void StoricizzaPassiCorrelati(string idRuoloOld, string idRuoloNew)
        {
            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            theManager.StoricizzaRuoloPassiCorrelati(idRuoloOld, idRuoloNew);
        }

        public delegate void StoricizzaPassiCorrelatiDelegate(string idRuoloOld, string idRuoloNew);

        private void CallBackStoricizza(IAsyncResult result)
        {
            storicizzaPassiCorrelati.EndInvoke(result);
        }

        private bool VerificaProcessiCoinvolti(string tipoOperazione, myTreeNode TreeNodo)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            bool result = true;
            if (TreeNodo.getTipoNodo().Equals("R"))
            {
                if (!this.chkStoricizza.Checked && !this.cb_disabled_trasm.Checked && !tipoOperazione.Equals("sposta"))
                    return result;

                if (tipoOperazione.Equals("sposta"))
                {
                    tipoOperazione = Amministrazione.Manager.OrganigrammaManager.TipoOperazione.SPOSTA_RUOLO;
                }
                else if (this.cb_disabled_trasm.Checked)
                {
                    tipoOperazione = Amministrazione.Manager.OrganigrammaManager.TipoOperazione.MODIFICA_RUOLO_CON_DISABILITAZIONE_TRASM;
                }
                else
                {
                    tipoOperazione = Amministrazione.Manager.OrganigrammaManager.TipoOperazione.MODIFICA_RUOLO_CON_STORICIZZAZIONE;
                }

                //Verifico che non ci siano processi attivi
                string idRuolo = TreeNodo.getIDGruppo();
                int countProcessiCoinvolti_R = ws.GetCountProcessiDiFirmaByTitolare(idRuolo, string.Empty);
                int countIstanzaProcessiCoinvolti_R = ws.GetCountIstanzaProcessiDiFirmaByTitolare(idRuolo, string.Empty);
                if (countProcessiCoinvolti_R > 0 || countIstanzaProcessiCoinvolti_R > 0)
                {
                    string tipoTitolare = Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.RUOLO;
                    string scriptString = "<SCRIPT>AvvisoRuoloConLF('" + tipoTitolare + "','" + idRuolo + "','" + countProcessiCoinvolti_R + "','" + countIstanzaProcessiCoinvolti_R + "','" + tipoOperazione + "');</SCRIPT>";
                    this.Page.RegisterStartupScript("AvvisoRuoloConLF", scriptString);
                    result = false;
                }
            }
            //Caso di sposta utente
            if (TreeNodo.getTipoNodo().Equals("P"))
            {
                myTreeNode TreeNodoPadre = (myTreeNode)this.GetNodoPadre(TreeNodo);
                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.DatiUtente(TreeNodo.getIDCorrGlobale());
                DocsPAWA.DocsPaWR.OrgUtente utente = theManager.getDatiUtente();
                string idUtente = utente.IDPeople;
                string idRuolo = TreeNodoPadre.getIDGruppo();
                int countProcessiCoinvolti_U = ws.GetCountProcessiDiFirmaByTitolare(idRuolo, idUtente);
                int countIstanzaProcessiCoinvolti_U = ws.GetCountIstanzaProcessiDiFirmaByTitolare(idRuolo, idUtente);
                if (countProcessiCoinvolti_U > 0 || countIstanzaProcessiCoinvolti_U > 0)
                {
                    string msg = "Attenzione, l\\'utente in modifica è coinvolto in processi di firma o processi di firma avviati; procedendo con lo spostamento verranno invalidati/ interrotti tutti i processi. \\n\\nSe si vuole procedere senza interruzione/invalidazione dei processi è necessario effettuare la sostituzione dell\\'utente utilizzando il tasto \\'Gestione utenti\\' presente nei dettagli del ruolo.\\n\\nSicuro di voler procedere con l\\'operazione?";
                    this.btn_sposta_utente.Attributes.Add("onclick", "if (!window.confirm('" + msg + "')) {return false};");
                    result = false;
                }
                else if(TreeNodoPadre.Nodes.Count == 1) //l'utente non è coinvolto ma è l'ultimo di un ruolo coinvolto
                {
                    int countProcessiCoinvolti_R = ws.GetCountProcessiDiFirmaByTitolare(idRuolo, string.Empty);
                    int countIstanzaProcessiCoinvolti_R = ws.GetCountIstanzaProcessiDiFirmaByTitolare(idRuolo, string.Empty);
                    if (countProcessiCoinvolti_R > 0 || countIstanzaProcessiCoinvolti_R > 0)
                    {
                        string msg = "Attenzione, l\\'utente in modifica è l\\'ultimo utente del ruolo coinvolto in processi di firma o processi di firma avviati; procedendo con lo spostamento verranno invalidati/ interrotti tutti i processi. \\n\\nSe si vuole procedere senza interruzione/invalidazione dei processi è necessario effettuare la sostituzione dell\\'utente utilizzando il tasto \\'Gestione utenti\\' presente nei dettagli del ruolo.\\n\\nSicuro di voler procedere con l\\'operazione?";
                        this.btn_sposta_utente.Attributes.Add("onclick", "if (!window.confirm('" + msg + "')) {return false};");
                        result = false;
                    }
                }
            }
            return result;
        }

        #endregion

        #region registri

        private bool CheckPresenzaProcessiFirmaRegistri()
        {
            bool result = false;
            myTreeNode TreeNodo;
            TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
            string idRuoloInUo = TreeNodo.getIDCorrGlobale();
            string idGruppo = TreeNodo.getIDGruppo();
            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();

            //Verifico se le caselle sono coinvolte in processi di firma
            System.Collections.Generic.List<RightRuoloMailRegistro> rightRuoloMailReg = new System.Collections.Generic.List<RightRuoloMailRegistro>();
            //inserisco le coppie 1 a n (ruolo/casella di posta registro)
            if (this.dg_registri.Items.Count > 0)
            {
                foreach (DataGridItem item in dg_registri.Items)
                {
                    CheckBox cbx_Sel = item.Cells[5].FindControl("cbx_Sel") as CheckBox;
                    // se il registro/rf ha caselle di posta ed è associato al ruolo
                    if (cbx_Sel.Checked && (!item.Cells[3].Text.Equals("&nbsp;") && (!string.IsNullOrEmpty(item.Cells[3].Text))))
                    {
                        string mail = (item.Cells[3].Text.Split('*').Length > 1) ? item.Cells[3].Text.Split('*')[1] : item.Cells[3].Text;
                        string idRegistro = item.Cells[0].Text;
                        string spedisci = (item.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Checked.ToString();
                        rightRuoloMailReg.Add(new RightRuoloMailRegistro
                        {
                            IdRegistro = idRegistro,
                            EmailRegistro = mail,
                            cha_spedisci = spedisci
                        });
                    }
                }
            }
            //inserisco le coppie 1 a n (ruolo/casella di posta rf)
            if (this.dg_registri.Items.Count > 0)
            {
                foreach (DataGridItem item in dg_rf.Items)
                {
                    CheckBox cbx_Sel = item.Cells[5].FindControl("cbx_Sel") as CheckBox;
                    // se il registro/rf ha caselle di posta ed è associato al ruolo
                    if (cbx_Sel.Checked && (!item.Cells[3].Text.Equals("&nbsp;") && (!string.IsNullOrEmpty(item.Cells[3].Text))))
                    {
                        string mail = (item.Cells[3].Text.Split('*').Length > 1) ? item.Cells[3].Text.Split('*')[1] : item.Cells[3].Text;
                        string idRegistro = item.Cells[0].Text;
                        string spedisci = (item.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Checked.ToString();
                            
                            rightRuoloMailReg.Add(new RightRuoloMailRegistro
                            {
                                IdRegistro = idRegistro,
                                EmailRegistro = mail,
                                cha_spedisci = spedisci
                            });
                    }
                }
            }
            if(rightRuoloMailReg != null && rightRuoloMailReg.Count() > 0)
            {
                if (theManager.ExistsPassiFirmaByRuoloTitolareAndRegistro(rightRuoloMailReg.ToArray(), idRuoloInUo, idGruppo))
                {
                    return true;
                }
            }
            return result;
        }

        /// <summary>
        /// Inserimento registri
        /// </summary>
        private void InserimentoRegistri()
        {
            try
            {
                if (this.dg_registri.Items.Count > 0)
                {
                    myTreeNode TreeNodo;
                    TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);

                    CheckBox spunta;

                    DocsPAWA.DocsPaWR.OrgRegistro registro = null;

                    ArrayList listaRegistriSelezionati = new ArrayList();

                    for (int i = 0; i < this.dg_registri.Items.Count; i++)
                    {
                        spunta = (CheckBox)dg_registri.Items[i].Cells[5].FindControl("cbx_Sel");

                        if (spunta.Checked)
                        {
                            registro = new DocsPAWA.DocsPaWR.OrgRegistro();

                            registro.IDRegistro = dg_registri.Items[i].Cells[0].Text;
                            registro.Codice = dg_registri.Items[i].Cells[1].Text;
                            registro.Descrizione = dg_registri.Items[i].Cells[2].Text;
                            registro.IDAmministrazione = dg_registri.Items[i].Cells[4].Text;
                            registro.Associato = TreeNodo.getIDCorrGlobale();

                            listaRegistriSelezionati.Add(registro);

                            registro = null;
                        }
                    }

                    for (int i = 0; i < this.dg_rf.Items.Count; i++)
                    {
                        spunta = (CheckBox)dg_rf.Items[i].Cells[5].FindControl("cbx_Sel");

                        if (spunta.Checked)
                        {
                            registro = new DocsPAWA.DocsPaWR.OrgRegistro();

                            registro.IDRegistro = dg_rf.Items[i].Cells[0].Text;
                            registro.Codice = dg_rf.Items[i].Cells[1].Text;
                            registro.Descrizione = dg_rf.Items[i].Cells[2].Text;
                            registro.IDAmministrazione = dg_rf.Items[i].Cells[4].Text;
                            registro.Associato = TreeNodo.getIDCorrGlobale();
                            registro.rfDisabled = dg_rf.Items[i].Cells[9].Text;
                            registro.idAOOCollegata = dg_rf.Items[i].Cells[10].Text;

                            if (!(registro.rfDisabled == "1" && (!listaRegistriSelezionati.Contains(registro.idAOOCollegata))))
                            {
                                listaRegistriSelezionati.Add(registro);

                            }
                            else
                            {
                                spunta.Checked = false;
                            }
                            registro = null;
                        }
                    }

                    if (listaRegistriSelezionati != null && listaRegistriSelezionati.Count > 0)
                    {
                        Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                        DocsPAWA.DocsPaWR.OrgRegistro[] registri = new DocsPAWA.DocsPaWR.OrgRegistro[listaRegistriSelezionati.Count];
                        listaRegistriSelezionati.CopyTo(registri);
                        listaRegistriSelezionati = null;

                        // prende la system_id della UO padre
                        myTreeNode TreeNodoPadre = (myTreeNode)this.GetNodoPadre(TreeNodo);
                        string idUO = TreeNodoPadre.getIDCorrGlobale();

                        theManager.getListaRegistri();
                        theManager.InsRegistri(registri, idUO, TreeNodo.getIDCorrGlobale());

                        DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                        esito = theManager.getEsitoOperazione();
                        //aggiorna la visibilità del ruolo sulle caselle di posta
                        if (esito.Codice == 0)
                        {
                            DocsPAWA.DocsPaWR.ValidationResultInfo res = SetVisCaselleRegistri();
                            if (!res.Value)
                            {
                                string scriptString = "<SCRIPT>alert('Attenzione, errore aggiornamento visibilità caselle di posta');</SCRIPT>";
                                this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                            }
                        }
                        if (!esito.Codice.Equals(0))
                        {
                            if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                            {
                                string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                                this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                            }
                        }

                        esito = null;
                    }
                    else
                    {
                        if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                        {
                            string scriptString = "<SCRIPT>alert('Attenzione, nessun registro selezionato');</SCRIPT>";
                            this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                        }
                    }
                }

                //Serve per far ricaricare alla rubrica le modifiche apportate
                DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                ws.resetHashTable();
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        private bool InvalidaProcessiFirmaRegistriCoinvolti()
        {
            bool result = false;
            myTreeNode TreeNodo;
            try
            {
                TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
                string idRuoloInUo = TreeNodo.getIDCorrGlobale();
                string idGruppo = TreeNodo.getIDGruppo();
                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();

                //Verifico se le caselle sono coinvolte in processi di firma
                System.Collections.Generic.List<RightRuoloMailRegistro> rightRuoloMailReg = new System.Collections.Generic.List<RightRuoloMailRegistro>();
                //inserisco le coppie 1 a n (ruolo/casella di posta registro)
                if (this.dg_registri.Items.Count > 0)
                {
                    foreach (DataGridItem item in dg_registri.Items)
                    {
                        CheckBox cbx_Sel = item.Cells[5].FindControl("cbx_Sel") as CheckBox;
                        // se il registro/rf ha caselle di posta ed è associato al ruolo
                        if (cbx_Sel.Checked && (!item.Cells[3].Text.Equals("&nbsp;") && (!string.IsNullOrEmpty(item.Cells[3].Text))))
                        {
                            string mail = (item.Cells[3].Text.Split('*').Length > 1) ? item.Cells[3].Text.Split('*')[1] : item.Cells[3].Text;
                            string idRegistro = item.Cells[0].Text;
                            string spedisci = (item.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Checked.ToString();
                            rightRuoloMailReg.Add(new RightRuoloMailRegistro
                            {
                                IdRegistro = idRegistro,
                                EmailRegistro = mail,
                                cha_spedisci = spedisci
                            });
                        }
                    }
                }
                //inserisco le coppie 1 a n (ruolo/casella di posta rf)
                if (this.dg_registri.Items.Count > 0)
                {
                    foreach (DataGridItem item in dg_rf.Items)
                    {
                        CheckBox cbx_Sel = item.Cells[5].FindControl("cbx_Sel") as CheckBox;
                        // se il registro/rf ha caselle di posta ed è associato al ruolo
                        if (cbx_Sel.Checked && (!item.Cells[3].Text.Equals("&nbsp;") && (!string.IsNullOrEmpty(item.Cells[3].Text))))
                        {
                            string mail = (item.Cells[3].Text.Split('*').Length > 1) ? item.Cells[3].Text.Split('*')[1] : item.Cells[3].Text;
                            string idRegistro = item.Cells[0].Text;
                            string spedisci = (item.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Checked.ToString();

                            rightRuoloMailReg.Add(new RightRuoloMailRegistro
                            {
                                IdRegistro = idRegistro,
                                EmailRegistro = mail,
                                cha_spedisci = spedisci
                            });
                        }
                    }
                }
                if (rightRuoloMailReg != null && rightRuoloMailReg.Count() > 0)
                {
                    DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                    if (theManager.InvalidaProcessiRegistriCoinvolti(rightRuoloMailReg.ToArray(), idRuoloInUo, idGruppo, sessionManager.getUserAmmSession()))
                    {
                        return true;
                    }
                }
            }
            catch(Exception e)
            {
                result = false;
            }      
            return result;
        }
        #endregion

        #region funzioni
        /// <summary>
        /// Inserisce Funzioni
        /// </summary>
        private void InserimentoTipoFunzioni()
        {
            try
            {
                if (this.dg_funzioni.Items.Count > 0)
                {
                    myTreeNode TreeNodo;
                    TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);

                    CheckBox spunta;

                    DocsPAWA.DocsPaWR.OrgTipoFunzione funzione = null;

                    ArrayList listaFunzioniSelezionate = new ArrayList();

                    for (int i = 0; i < this.dg_funzioni.Items.Count; i++)
                    {
                        spunta = (CheckBox)dg_funzioni.Items[i].Cells[4].FindControl("Chk_funzioni");

                        if (spunta.Checked)
                        {
                            funzione = new DocsPAWA.DocsPaWR.OrgTipoFunzione();

                            funzione.IDTipoFunzione = dg_funzioni.Items[i].Cells[0].Text;
                            funzione.Codice = dg_funzioni.Items[i].Cells[1].Text;
                            funzione.Descrizione = dg_funzioni.Items[i].Cells[2].Text;
                            funzione.IDAmministrazione = dg_funzioni.Items[i].Cells[3].Text;
                            funzione.Associato = TreeNodo.getIDCorrGlobale();

                            listaFunzioniSelezionate.Add(funzione);

                            funzione = null;
                        }
                    }

                    if (listaFunzioniSelezionate != null && listaFunzioniSelezionate.Count > 0)
                    {
                        DocsPAWA.DocsPaWR.OrgTipoFunzione[] funzioni = new DocsPAWA.DocsPaWR.OrgTipoFunzione[listaFunzioniSelezionate.Count];
                        listaFunzioniSelezionate.CopyTo(funzioni);
                        listaFunzioniSelezionate = null;

                        Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                        theManager.InsTipoFunzioni(funzioni);

                        DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                        esito = theManager.getEsitoOperazione();

                        if (!esito.Codice.Equals(0))
                        {
                            if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                            {
                                string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                                this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                            }
                        }

                        esito = null;
                    }
                    else
                    {
                        if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                        {
                            string scriptString = "<SCRIPT>alert('Attenzione, nessuna funzione selezionata');</SCRIPT>";
                            this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }
        #endregion

        #endregion

        #region Ricerca

        private void ddl_ricTipo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.chkSearchHistoricized.Checked = false;
            this.chkSearchHistoricized.Enabled = false;

            switch (this.ddl_ricTipo.SelectedValue)
            {
                case "U":
                    this.td_descRicerca.InnerHtml = "Nome UO:";
                    break;
                case "R":
                    this.td_descRicerca.InnerHtml = "Nome ruolo:";
                    this.chkSearchHistoricized.Enabled = true;
                    break;
                case "PN":
                    this.td_descRicerca.InnerHtml = "Nome utente:";
                    break;
                case "PC":
                    this.td_descRicerca.InnerHtml = "Cognome utente:";
                    break;
            }

            this.SetFocus(this.txt_ricDesc);

        }

        private void RicercaNodo(string returnValue)
        {
            try
            {
                this.hd_returnValueModal.Value = "";

                string[] appo = returnValue.Split('_');

                string idCorrGlobale = appo[0];
                string idParent = appo[1];

                string tipo = this.ddl_ricTipo.SelectedValue;

                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();

                //theManager.CurrentIDAmm(codAmm);

                theManager.ListaUOLivelloZero(idAmm);

                this.LoadTreeviewLivelloZero(theManager.getListaUO());

                theManager.ListaIDParentRicerca(idParent, tipo);

                if (theManager.getListaIDParentRicerca() != null && theManager.getListaIDParentRicerca().Count > 0)
                {
                    ArrayList lista = new ArrayList();
                    theManager.getListaIDParentRicerca().Reverse();
                    lista = theManager.getListaIDParentRicerca();

                    lista.Add(Convert.ToInt32(idCorrGlobale));

                    for (int n = 1; n <= lista.Count - 1; n++)
                    {
                        myTreeNode TreeNodo;
                        TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);

                        foreach (myTreeNode nodo in TreeNodo.Nodes)
                        {
                            if (nodo.ID.Equals(lista[n].ToString()) && nodo.ID != idCorrGlobale)
                            {
                                if (nodo.getTipoNodo().Equals("U"))
                                {
                                    this.LoadTreeViewLivelloFigli(nodo.GetNodeIndex(), nodo.getTipoNodo());
                                }
                                else
                                {
                                    nodo.Expanded = true;
                                }
                                treeViewUO.SelectedNodeIndex = nodo.GetNodeIndex();

                                break;
                            }
                            if (nodo.ID.Equals(lista[n].ToString()) && nodo.ID.Equals(idCorrGlobale))
                            {
                                this.GestioneLoadDettagli(nodo);
                                treeViewUO.SelectedNodeIndex = nodo.GetNodeIndex();

                                //if (nodo.getTipoNodo().Equals("R"))
                                //    VerificaProcessiCoinvolti(nodo);

                                break;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                this.lbl_avviso.Text = ex.ToString();
                this.GUI("ResetAll");
            }
        }

        #endregion

        #region Gestione label di percorso

        private void GestLabelPercorso(myTreeNode nodo)
        {
            this.lbl_percorso.Text = nodo.getPercorso();
        }

        #endregion

        private void dg_rf_PreRender(object sender, System.EventArgs e)
        {
            DataGrid dg = ((DataGrid)sender);
            for (int i = 0; i < dg.Items.Count; i++)
            {
                string disabled = this.dg_rf.Items[i].Cells[5].Text;

                if (disabled != null)
                {
                    if (disabled != String.Empty && disabled == "1")//l'RF è disabilitato
                    {
                        dg.Items[i].ToolTip = "Questo RF è disabilitato, pertanto non è possibile associargli un ruolo";

                        CheckBox checkBox = dg.Items[i].Cells[4].Controls[0].FindControl("Chk_rf") as CheckBox;
                        checkBox.Enabled = false;
                        dg.Items[i].ForeColor = System.Drawing.Color.Red;
                    }
                }
            }

        }

        private void dg_registri_PreRender(object sender, System.EventArgs e)
        {
            // DataGrid dg_reg = ((DataGrid)sender);
            for (int i = 0; i < this.dg_registri.Items.Count; i++)
            {
                if (this.dg_registri.Items[i].Cells[5].Text.ToUpper().Equals("TRUE"))
                {
                    this.dg_registri.Items[i].ToolTip = "Questo registro è sospeso, pertanto non è possibile associargli un ruolo";

                    CheckBox checkBox = dg_registri.Items[i].Cells[4].Controls[0].FindControl("Chk_registri") as CheckBox;
                    checkBox.Enabled = false;
                    this.dg_registri.Items[i].ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        protected void OnGridRegistriItemBounded(Object sender, DataGridItemEventArgs e)
        {
            /*
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                CheckBox chkRegistri = e.Item.FindControl("Chk_registri") as CheckBox;

                chkRegistri.Attributes.Add("onclick", "gestioneClickReg('" + e.Item.ItemIndex + "', '" + e.Item.Cells[0].Text + "');");
            }
            */
        }


        #region Gestione Rubrica Comune

        /// <summary>
        /// Impostazione visibilità dei controlli relativi alla gestione della rubrica comune;
        /// </summary>
        /// <param name="visible"></param>
        private void SetVisibilityRubricaComune(bool visible)
        {
            this.pnlRubricaComune.Visible = (this.GestioneRubricaComuneAbilitata && visible);
        }

        /// <summary>
        /// Verifica se la rubrica comune è abilitata
        /// </summary>
        protected bool GestioneRubricaComuneAbilitata
        {
            get
            {
                DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                DocsPAWA.DocsPaWR.ConfigurazioniRubricaComune config = DocsPAWA.RubricaComune.Configurazioni.GetConfigurazioni(sessionManager.getUserAmmSession());
                return config.GestioneAbilitata;
            }
        }

        /// <summary>
        /// Handler pulsante di invio della UO in rubrica comune
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_invia_rc_OnClick(object sender, EventArgs e)
        {
            if (treeViewUO.SelectedNodeIndex != "-1")
            {
                myTreeNode selectedNode = treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex) as myTreeNode;

                if (selectedNode != null)
                {
                    // Successivamente l'invio, aggiornamento dati dell'uo inviata in rubrica comune
                    this.FetchDatiRubricaComune(selectedNode.getIDCorrGlobale());
                }
            }
        }

        /// <summary>
        /// Reperimento dell'ID dell'UO corrente
        /// </summary>
        /// <returns></returns>
        protected string GetIdUOSelezionata()
        {
            string id = string.Empty;

            if (treeViewUO.SelectedNodeIndex != "-1")
            {
                myTreeNode selectedNode = treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex) as myTreeNode;

                if (selectedNode != null)
                    id = selectedNode.getIDCorrGlobale();
            }

            return id;
        }
        
        /// <summary>
        /// Caricamento dati relativi all'uo corrente in rubrica comune
        /// </summary>
        /// <param name="idUO"></param>
        protected void FetchDatiRubricaComune(string idUO)
        {
            if (this.pnlRubricaComune.Visible)
            {
                this.ClearDatiRubricaComune();

                DocsPAWA.AdminTool.Manager.SessionManager sessionMng = new DocsPAWA.AdminTool.Manager.SessionManager();

                RubricaComune.Proxy.Elementi.ElementoRubrica elemento = DocsPAWA.RubricaComune.RubricaServices.GetElementoRubricaUO(sessionMng.getUserAmmSession(), idUO);

                if (elemento != null)
                {
                    const string format = "dd/MM/yyyy HH:mm:ss";

                    // L'uo è già presente in rubrica comune come elemento di interesse generale;
                    // viene mostrata la data di ultima modifica e la data di creazione
                    this.lblCodiceRC.Text = elemento.Codice;
                    this.lblDescrizioneRC.Text = elemento.Descrizione;
                    this.lblDataCreazioneRC.Text = elemento.DataCreazione.ToString(format);
                    this.lblDataUltimaModificaRC.Text = elemento.DataUltimaModifica.ToString(format);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ClearDatiRubricaComune()
        {
            this.lblCodiceRC.Text = string.Empty;
            this.lblDescrizioneRC.Text = string.Empty;
            this.lblDataCreazioneRC.Text = string.Empty;
            this.lblDataUltimaModificaRC.Text = string.Empty;
        }


        #endregion

        private bool isTitolarioRuoloAbilitato()
        {
            return (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["TITOLARIO_RUOLO_ORG"]) && System.Configuration.ConfigurationManager.AppSettings["TITOLARIO_RUOLO_ORG"].Equals("1"));
        }

        private bool isUnicoRuoloRespUo(myTreeNode treeNodo, string tipoAzione)
        {
            bool result = true;
            if (treeNodo.getTipoNodo() == "R")
            {
                //Gestione inserimento ruoli sotto la UO principale
                //NEW                            
                myTreeNode treeNodoParent = null;
                switch (tipoAzione)
                {
                    case "modifica":
                        try
                        {
                            treeNodoParent = (myTreeNode)treeNodo.Parent;
                        }
                        catch (Exception ex)
                        {
                            treeNodoParent = treeNodo;
                        }
                        break;
                    case "inserimento":
                    case "cancellazione":
                        treeNodoParent = treeNodo;
                        break;
                }

                //OLD
                //myTreeNode treeNodoParent = (myTreeNode)treeNodo.Parent;

                for (int i = 0; i < treeNodoParent.Nodes.Count; i++)
                {
                    myTreeNode nodoFiglio = ((myTreeNode)treeNodoParent.Nodes[i]);
                    //se ho già un ruolo segretario o responsabile nn posso selezionarne un altro                    
                    if (nodoFiglio.getTipoNodo() == "R")
                    {
                        if (!nodoFiglio.Equals(treeNodo))
                        {
                            if (this.chk_resp.Checked && nodoFiglio.getResponsabile() == "1")
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < treeNodo.Nodes.Count; i++)
                {
                    myTreeNode nodoFiglio = ((myTreeNode)treeNodo.Nodes[i]);
                    //se ho già un ruolo segretario o responsabile nn posso selezionarne un altro
                    if (nodoFiglio.getTipoNodo() == "R")
                    {
                        if (!nodoFiglio.Equals(treeNodo))
                        {
                            if (this.chk_resp.Checked && nodoFiglio.getResponsabile() == "1")
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
            return result;
        }
        private bool isUnicoRuoloSegretarioUo(myTreeNode treeNodo, string tipoAzione)
        {
            bool result = true;
            if (treeNodo.getTipoNodo() == "R")
            {
                //Gestione inserimento ruoli sotto la UO principale
                //NEW                            
                myTreeNode treeNodoParent = null;
                switch (tipoAzione)
                {
                    case "modifica":
                        try
                        {
                            treeNodoParent = (myTreeNode)treeNodo.Parent;
                        }
                        catch (Exception ex)
                        {
                            treeNodoParent = treeNodo;
                        }
                        break;
                    case "inserimento":
                    case "cancellazione":
                        treeNodoParent = treeNodo;
                        break;
                }

                //OLD
                //myTreeNode treeNodoParent = (myTreeNode)treeNodo.Parent;

                for (int i = 0; i < treeNodoParent.Nodes.Count; i++)
                {
                    myTreeNode nodoFiglio = ((myTreeNode)treeNodoParent.Nodes[i]);
                    //se ho già un ruolo segretario o responsabile nn posso selezionarne un altro
                    if (nodoFiglio.getTipoNodo() == "R")
                    {
                        if (!nodoFiglio.Equals(treeNodo))
                        {
                            if (this.cb_segretario.Checked && nodoFiglio.getSegretario() == "1")
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < treeNodo.Nodes.Count; i++)
                {
                    myTreeNode nodoFiglio = ((myTreeNode)treeNodo.Nodes[i]);
                    //se ho già un ruolo segretario o responsabile nn posso selezionarne un altro
                    if (nodoFiglio.getTipoNodo() == "R")
                    {
                        if (!nodoFiglio.Equals(treeNodo))
                        {
                            if (this.cb_segretario.Checked && nodoFiglio.getSegretario() == "1")
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
            return result;
        }


        protected void btn_ImportaOrganigramma_Click(object sender, EventArgs e)
        {
            //DocsPAWA.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();
            //DocsPAWA.DocsPaWR.OrgTitolario titolario = new DocsPAWA.DocsPaWR.OrgTitolario();
            //titolario.CodiceAmministrazione = nodo.CodiceAmministrazione;
            //titolario.ID = nodo.ID;
            //titolario.Stato = DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione;
            //Session.Add("titolarioSelezionato", titolario);
            ClientScript.RegisterStartupScript(this.GetType(), "importaOrganigramma", "ApriImportaOrganigramma();", true);
        }

        protected void btnGenerateSummary_Click(object sender, EventArgs e)
        {
            // Recupero dell informazioni sull'utente / amministratore loggato
            InfoUtente userInfo = null;
            
            SessionManager sm = new SessionManager();
            userInfo = sm.getUserAmmSession();
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            userInfo.idAmministrazione = new DocsPaWebService().getIdAmmByCod(codiceAmministrazione);

            // Inizializzazione del call context
            if (CallContextStack.CurrentContext == null)
                CallContextStack.CurrentContext = new CallContext("ReportingContext");

            // Salvataggio della request nel call context
            PrintReportRequest request =
                new PrintReportRequest()
                {
                    ContextName = "ConsistenzaOrganigramma",
                    SearchFilters = null,
                    UserInfo = userInfo
                };

            ReportingUtils.PrintRequest = request;

            Page.ClientScript.RegisterStartupScript(this.GetType(), "openReport", ReportingUtils.GetOpenReportPageScript(false), true);

        }

        /// <summary>
        /// Metodo per l'abilitazione / disabilitazione dei controlli relativi alla modifica / spostamento del ruolo.
        /// Viene utilizzato per rendere mutuamente esclusive le due operazioni
        /// </summary>
        /// <param name="enable">True se bisogna configurare la schermata in modo che consenta la modifica</param>
        public void EnableModifyControls(bool enable, myTreeNode node)
        {
            ddl_tipo_ruolo.Enabled = enable;
            txt_rubricaRuolo.Enabled = enable;
            txt_descrizioneRuolo.Enabled = enable;
            cb_disabled_trasm.Enabled = enable;
            chk_rif.Enabled = enable;
            chk_resp.Enabled = enable;
            cb_segretario.Enabled = enable;

            // Se si è chiesta la disabilitazione della modifica vengono visualizzati i dati originali ed
            // aggiornati i dati del tree node

            if (!enable)
            {
                // Recupero del dettaglio del ruolo in modifica
                OrganigrammaManager manager = new OrganigrammaManager();
                OrgRuolo role = manager.GetRole(node.getIDCorrGlobale());

                manager.ListaTipiRuolo(role.IDAmministrazione);
                OrgTipoRuolo tipo = ((OrgTipoRuolo[])manager.getListaTipiRuolo().ToArray(typeof(OrgTipoRuolo))).Where(e => e.IDTipoRuolo == role.IDTipoRuolo).FirstOrDefault();
                ddl_tipo_ruolo.SelectedValue = String.Format("{0}@{1}#{2}", tipo.IDTipoRuolo, tipo.Codice, tipo.Descrizione);
                node.setIDTipoRuolo(role.IDTipoRuolo);

                txt_rubricaRuolo.Text = role.Codice;
                node.setIDCorrGlobale(role.IDCorrGlobale);
                node.setIDGruppo(role.IDGruppo);

                txt_descrizioneRuolo.Text = role.Descrizione;
                node.setDescrizione(role.Descrizione);

                cb_disabled_trasm.Checked = role.DisabledTrasm == "1";
                node.setDisabledTrasm(role.DisabledTrasm);

                chk_rif.Checked = role.DiRiferimento == "1";
                node.setDiRiferimento(role.DiRiferimento);

                chk_resp.Checked = role.Responsabile == "1";
                node.setResponsabile(role.Responsabile);

                cb_segretario.Checked = role.Segretario == "1";
                node.setSegretario(role.Segretario);
                
            }

        }

        #region Multi casella
        protected void dg_registri_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            DocsPaWebService ws = new DocsPaWebService();
            if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
            {
                if (((!(e.Item.Cells[5].FindControl("cbx_Sel") as CheckBox).Checked) ||(string.IsNullOrEmpty(e.Item.Cells[3].Text) || e.Item.Cells[3].Text.Equals("&nbsp;"))) &&
                    (!(e.Item.Cells[5].FindControl("cbx_Sel") as CheckBox).Checked || !ws.IsEnabledInteropInterna()))
                {
                    (e.Item.Cells[6].FindControl("cbx_Consulta") as CheckBox).Enabled = false;
                    (e.Item.Cells[7].FindControl("cbx_Notifica") as CheckBox).Enabled = false;
                    (e.Item.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Enabled = false;
                }
                //aggancio evento checked changed alla checkbox Sel
                (e.Item.Cells[5].FindControl("cbx_Sel") as CheckBox).CheckedChanged += new EventHandler(cbx_Sel_CheckedChanged);
            }
        }
        protected void cbx_SelReg_CheckedChanged(object sender, System.EventArgs e)
        {
            DocsPaWebService ws = new DocsPaWebService();
            //recupero le check box
            CheckBox cbx_Sel = (sender as CheckBox);
            string idRegistro = ((cbx_Sel.Parent.Parent as DataGridItem).Cells)[0].Text;
            CheckBox cbx_Consulta = cbx_Sel.Parent.Parent.FindControl("cbx_Consulta") as CheckBox;
            CheckBox cbx_Spedisci = cbx_Sel.Parent.Parent.FindControl("cbx_Spedisci") as CheckBox;
            CheckBox cbx_Notifica = cbx_Sel.Parent.Parent.FindControl("cbx_Notifica") as CheckBox;
            //recupero l'indirizzo mail
            string mail = ((cbx_Sel.Parent.Parent as DataGridItem).Cells)[3].Text;
            if (cbx_Sel.Checked == false)
            {
                cbx_Consulta.Checked = false;
                cbx_Notifica.Checked = false;
                cbx_Spedisci.Checked = false;
                cbx_Consulta.Enabled = false;
                cbx_Notifica.Enabled = false;
                cbx_Spedisci.Enabled = false;

                //se ho effettuato un'azione su un Registro con più caselle di posta, allora intervengo anche sulle altre rows di Registro
                foreach (DataGridItem r in dg_registri.Items)
                {
                    if (r.Cells[0].Text.Equals(idRegistro))
                    {
                        (r.Cells[5].FindControl("cbx_Sel") as CheckBox).Checked = false;
                        (r.Cells[6].FindControl("cbx_Consulta") as CheckBox).Checked = false;
                        (r.Cells[7].FindControl("cbx_Notifica") as CheckBox).Checked = false;
                        (r.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Checked = false;
                        (r.Cells[6].FindControl("cbx_Consulta") as CheckBox).Enabled = false;
                        (r.Cells[7].FindControl("cbx_Notifica") as CheckBox).Enabled = false;
                        (r.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Enabled = false;
                    }
                }
            }
            else if (cbx_Sel.Checked)
            {
                if (!ws.IsEnabledInteropInterna() && (mail.Equals("&nbsp;") || string.IsNullOrEmpty(mail)))
                {
                    cbx_Consulta.Enabled = false;
                    cbx_Notifica.Enabled = false;
                    cbx_Spedisci.Enabled = false;
                }
                else
                {
                    cbx_Consulta.Enabled = true;
                    cbx_Notifica.Enabled = true;
                    cbx_Spedisci.Enabled = true;
                }
                //se ho effettuato un'azione su un Registro con più caselle di posta, allora intervengo anche sulle altre rows del Registro
                foreach (DataGridItem r in dg_registri.Items)
                {
                    if (r.Cells[0].Text.Equals(idRegistro))
                    {
                        (r.Cells[5].FindControl("cbx_Sel") as CheckBox).Checked = true;
                        if (!ws.IsEnabledInteropInterna() && (string.IsNullOrEmpty(r.Cells[3].Text) || r.Cells[3].Text.Equals("&nbsp;")))
                        {
                            (r.Cells[6].FindControl("cbx_Consulta") as CheckBox).Enabled = false;
                            (r.Cells[7].FindControl("cbx_Notifica") as CheckBox).Enabled = false;
                            (r.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Enabled = false;
                        }
                        else
                        {
                            (r.Cells[6].FindControl("cbx_Consulta") as CheckBox).Enabled = true;
                            (r.Cells[7].FindControl("cbx_Notifica") as CheckBox).Enabled = true;
                            (r.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Enabled = true;
                        }
                    }
                }
            }
        }

        protected void dg_rf_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            DocsPaWebService ws = new DocsPaWebService();
            if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
            {
                if (((!(e.Item.Cells[5].FindControl("cbx_Sel") as CheckBox).Checked) || (string.IsNullOrEmpty(e.Item.Cells[3].Text) || e.Item.Cells[3].Text.Equals("&nbsp;"))) &&
                    (!(e.Item.Cells[5].FindControl("cbx_Sel") as CheckBox).Checked || !ws.IsEnabledInteropInterna()))
                {
                    (e.Item.Cells[6].FindControl("cbx_Consulta") as CheckBox).Enabled = false;
                    (e.Item.Cells[7].FindControl("cbx_Notifica") as CheckBox).Enabled = false;
                    (e.Item.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Enabled = false;
                }
                //aggancio evento checked changed alla checkbox Sel
                (e.Item.Cells[5].FindControl("cbx_Sel") as CheckBox).CheckedChanged += new EventHandler(cbx_Sel_CheckedChanged);
            }        
        }
        protected void cbx_Sel_CheckedChanged(object sender, System.EventArgs e)
        {
            DocsPaWebService ws = new DocsPaWebService();
            //recupero le check box
            CheckBox cbx_Sel = (sender as CheckBox);
            string idRF = ((cbx_Sel.Parent.Parent as DataGridItem).Cells)[1].Text;
            CheckBox cbx_Consulta = cbx_Sel.Parent.Parent.FindControl("cbx_Consulta") as CheckBox;
            CheckBox cbx_Spedisci = cbx_Sel.Parent.Parent.FindControl("cbx_Spedisci") as CheckBox;
            CheckBox cbx_Notifica = cbx_Sel.Parent.Parent.FindControl("cbx_Notifica") as CheckBox;
            //recupero l'indirizzo mail
            string mail = ((cbx_Sel.Parent.Parent as DataGridItem).Cells)[3].Text;
            if (cbx_Sel.Checked == false)
            {
                cbx_Consulta.Checked = false;
                cbx_Notifica.Checked = false;
                cbx_Spedisci.Checked = false;
                cbx_Consulta.Enabled = false;
                cbx_Notifica.Enabled = false;
                cbx_Spedisci.Enabled = false;

                //se ho effettuato un'azione su un RF con più caselle di posta, allora intervengo anche sulle altre rows di RF
                foreach (DataGridItem r in dg_rf.Items)
                {
                    if (r.Cells[1].Text.Equals(idRF))
                    {

                        (r.Cells[5].FindControl("cbx_Sel") as CheckBox).Checked = false;
                        (r.Cells[6].FindControl("cbx_Consulta") as CheckBox).Checked = false;
                        (r.Cells[7].FindControl("cbx_Notifica") as CheckBox).Checked = false;
                        (r.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Checked = false;
                        (r.Cells[6].FindControl("cbx_Consulta") as CheckBox).Enabled = false;
                        (r.Cells[7].FindControl("cbx_Notifica") as CheckBox).Enabled = false;
                        (r.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Enabled = false;
                    }
                }
            }
            else if (cbx_Sel.Checked)
            {
                if (!ws.IsEnabledInteropInterna() && (mail.Equals("&nbsp;") || string.IsNullOrEmpty(mail)))
                {
                    cbx_Consulta.Enabled = false;
                    cbx_Notifica.Enabled = false;
                    cbx_Spedisci.Enabled = false;
                }
                else
                {
                    cbx_Consulta.Enabled = true;
                    cbx_Notifica.Enabled = true;
                    cbx_Spedisci.Enabled = true;
                }
                //se ho effettuato un'azione su un RF con più caselle di posta, allora intervengo anche sulle altre rows di RF
                foreach (DataGridItem r in dg_rf.Items)
                {
                    if (r.Cells[1].Text.Equals(idRF))
                    {
                        (r.Cells[5].FindControl("cbx_Sel") as CheckBox).Checked = true;
                        if (!ws.IsEnabledInteropInterna() && (string.IsNullOrEmpty(r.Cells[3].Text) || r.Cells[3].Text.Equals("&nbsp;")))
                        {
                            (r.Cells[6].FindControl("cbx_Consulta") as CheckBox).Enabled = false;
                            (r.Cells[7].FindControl("cbx_Notifica") as CheckBox).Enabled = false;
                            (r.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Enabled = false;
                        }
                        else
                        {
                            (r.Cells[6].FindControl("cbx_Consulta") as CheckBox).Enabled = true;
                            (r.Cells[7].FindControl("cbx_Notifica") as CheckBox).Enabled = true;
                            (r.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Enabled = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Aggiorna la visibilità sulle caselle di posta dei registri/rf associati al ruolo
        /// </summary>
        private DocsPAWA.DocsPaWR.ValidationResultInfo SetVisCaselleRegistri()
        {
            DocsPaWebService ws = new DocsPaWebService();
            myTreeNode treeNode;
            treeNode = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
            
            //elimino tutte le voci associate al ruolo in DPA_VIS_MAIL_REGISTRO
            DocsPAWA.DocsPaWR.ValidationResultInfo result = MultiCasellaManager.DeletelRightMailRegistro(string.Empty, treeNode.getIDCorrGlobale(), string.Empty);
            if (result.Value)
            {
                System.Collections.Generic.List<RightRuoloMailRegistro> rightRuoloMailReg = new System.Collections.Generic.List<RightRuoloMailRegistro>();
                //inserisco le coppie 1 a n (ruolo/casella di posta registro)
                if (this.dg_registri.Items.Count > 0)
                {
                    string idRuolo = treeNode.getIDCorrGlobale();
                    foreach (DataGridItem item in dg_registri.Items)
                    {
                        CheckBox cbx_Sel = item.Cells[5].FindControl("cbx_Sel") as CheckBox;
                        // se il registro/rf ha caselle di posta ed è associato al ruolo
                        if (cbx_Sel.Checked && (!item.Cells[3].Text.Equals("&nbsp;") && (!string.IsNullOrEmpty(item.Cells[3].Text))))
                        {
                            string mail = (item.Cells[3].Text.Split('*').Length > 1) ? item.Cells[3].Text.Split('*')[1] : item.Cells[3].Text;
                            string idRegistro = item.Cells[0].Text;
                            string consulta = (item.Cells[6].FindControl("cbx_Consulta") as CheckBox).Checked.ToString();
                            string notifica = (item.Cells[7].FindControl("cbx_Notifica") as CheckBox).Checked.ToString();
                            string spedisci = (item.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Checked.ToString();
                            rightRuoloMailReg.Add(new RightRuoloMailRegistro
                            {
                                IdRegistro = idRegistro,
                                idRuolo = idRuolo,
                                EmailRegistro = mail,
                                cha_consulta = consulta,
                                cha_spedisci = spedisci,
                                cha_notifica = notifica
                            });
                        }
                        if (cbx_Sel.Checked && (item.Cells[3].Text.Equals("&nbsp;") || string.IsNullOrEmpty(item.Cells[3].Text)) && ws.IsEnabledInteropInterna())
                        {
                            string idRegistro = item.Cells[0].Text;
                            string consulta = (item.Cells[6].FindControl("cbx_Consulta") as CheckBox).Checked.ToString();
                            string notifica = (item.Cells[7].FindControl("cbx_Notifica") as CheckBox).Checked.ToString();
                            string spedisci = (item.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Checked.ToString();
                            rightRuoloMailReg.Add(new RightRuoloMailRegistro
                            {
                                IdRegistro = idRegistro,
                                idRuolo = idRuolo,
                                EmailRegistro = "",
                                cha_consulta = consulta,
                                cha_spedisci = spedisci,
                                cha_notifica = notifica
                            });
                        }
                    }
                }
                //inserisco le coppie 1 a n (ruolo/casella di posta rf)
                if (this.dg_registri.Items.Count > 0)
                {
                    string idRuolo = treeNode.getIDCorrGlobale();
                    foreach (DataGridItem item in dg_rf.Items)
                    {
                        CheckBox cbx_Sel = item.Cells[5].FindControl("cbx_Sel") as CheckBox;
                        // se il registro/rf ha caselle di posta ed è associato al ruolo
                        if (cbx_Sel.Checked && (!item.Cells[3].Text.Equals("&nbsp;") && (!string.IsNullOrEmpty(item.Cells[3].Text))))
                        {
                            string mail = (item.Cells[3].Text.Split('*').Length > 1) ? item.Cells[3].Text.Split('*')[1] : item.Cells[3].Text;
                            string idRegistro = item.Cells[0].Text;
                            string consulta = (item.Cells[6].FindControl("cbx_Consulta") as CheckBox).Checked.ToString();
                            string notifica = (item.Cells[7].FindControl("cbx_Notifica") as CheckBox).Checked.ToString();
                            string spedisci = (item.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Checked.ToString();
                            rightRuoloMailReg.Add(new RightRuoloMailRegistro
                            {
                                IdRegistro = idRegistro,
                                idRuolo = idRuolo,
                                EmailRegistro = mail,
                                cha_consulta = consulta,
                                cha_spedisci = spedisci,
                                cha_notifica = notifica
                            });
                        }
                        if (cbx_Sel.Checked && (item.Cells[3].Text.Equals("&nbsp;") || string.IsNullOrEmpty(item.Cells[3].Text)) && ws.IsEnabledInteropInterna())
                        {
                            string idRegistro = item.Cells[0].Text;
                            string consulta = (item.Cells[6].FindControl("cbx_Consulta") as CheckBox).Checked.ToString();
                            string notifica = (item.Cells[7].FindControl("cbx_Notifica") as CheckBox).Checked.ToString();
                            string spedisci = (item.Cells[8].FindControl("cbx_Spedisci") as CheckBox).Checked.ToString();
                            rightRuoloMailReg.Add(new RightRuoloMailRegistro
                            {
                                IdRegistro = idRegistro,
                                idRuolo = idRuolo,
                                EmailRegistro = "",
                                cha_consulta = consulta,
                                cha_spedisci = spedisci,
                                cha_notifica = notifica
                            });
                        }
                    }
                }
                if(rightRuoloMailReg.Count > 0) 
                    result = MultiCasellaManager.InsertRightMailRegistro(rightRuoloMailReg);
            }
            return result;
        }
        #endregion

        #region Interoperabilità Semplificata

        public void LoadSettingsIS(String idRegistry, String idRf, String idAmm)
        {
            // Caricamento dei registri e degli RF per l'IS
            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            theManager.ListaRegistriRF(idAmm, null, "");

            if (theManager.getListaRegistri() != null && theManager.getListaRegistri().Count > 0)
            {
                foreach (DocsPAWA.DocsPaWR.OrgRegistro registro in theManager.getListaRegistri())
                {
                    ListItem item = new ListItem(String.Format("{0} - {1}", registro.Codice, registro.Descrizione), registro.IDRegistro);
                    this.ddlRegistriInteropSemplificata.Items.Add(item);
                }

                this.ddlRegistriInteropSemplificata.SelectedIndex = ddlRegistriInteropSemplificata.Items.IndexOf(ddlRegistriInteropSemplificata.Items.FindByValue(idRegistry));
            }

            // Caricamento RF dell'amministrazione
            DocsPaWebService ws = new DocsPaWebService();
            ws.Timeout = System.Threading.Timeout.Infinite;

            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];

            foreach (OrgRegistro registro in ws.AmmGetRegistri(codiceAmministrazione, "1"))
            {
                ListItem item = new ListItem(String.Format("{0} - {1}", registro.Codice, registro.Descrizione), registro.IDRegistro);
                this.ddlRfInteropSemplificata.Items.Add(item);
                
            }    
            this.ddlRfInteropSemplificata.SelectedIndex = ddlRfInteropSemplificata.Items.IndexOf(ddlRfInteropSemplificata.Items.FindByValue(idRf));
            

        }
        #endregion

        #region Libro Firma

        private void GestRitornoAvvisoLFRuolo(string valore)
        {
            try
            {
                string tipoOperazione = hd_TipoOperazione.Value;
                switch (valore)
                {
                    case "Y":
                        SetSessionInterrompiProcessi(true);
                        if (tipoOperazione.Equals(Amministrazione.Manager.OrganigrammaManager.TipoOperazione.SPOSTA_RUOLO))
                            SpostaRuolo();
                        else
                            this.InsModCanc_Ruolo("modifica");
                        break;
                    case "Y_SENZA_INTERRUZIONE":
                        if (tipoOperazione.Equals(Amministrazione.Manager.OrganigrammaManager.TipoOperazione.SPOSTA_RUOLO))
                            SpostaRuolo();
                        else
                        {
                            SetSessionStoricizzaProcessi(true);
                            this.InsModCanc_Ruolo("modifica");
                        }
                        break;
                    case "N":
                        break;
                }
                this.hd_returnValueModalLFRuolo.Value = "";
                this.hd_TipoOperazione.Value = "";
            }
            catch
            {
            }
        }

        private void GestRitornoAvvisoLFUtente(string valore)
        {
            try
            {
                string tipoOperazione = hd_TipoOperazione.Value;
                switch (valore)
                {
                    case "Y":
                        if (tipoOperazione.Equals("sposta_utente"))
                            this.Gest_AperturaWnd_SpostaUtente(true);
                        break;
                    case "N":
                        break;
                }
                this.hd_returnValueModalLFRuolo.Value = "";
                this.hd_TipoOperazione.Value = "";
            }
            catch
            {
            }
        }

        private void SetSessionInterrompiProcessi(bool interrompi)
        {
            Session["INTERROMPIPROCESSILF"] = interrompi;
        }

        private bool GetSessionInterrompiProcessi()
        {
            if (Session["INTERROMPIPROCESSILF"] == null)
                return false;
            else
                return (bool)Session["INTERROMPIPROCESSILF"];
        }

        private void RemoveSessionInterrompiProcessi()
        {
            Session.Remove("INTERROMPIPROCESSILF");
        }

        private void SetSessionStoricizzaProcessi(bool storicizza)
        {
            Session["STORICIZZAPROCESSILF"] = storicizza;
        }

        private bool GetSessionStoricizzaProcessi()
        {
            if (Session["STORICIZZAPROCESSILF"] == null)
                return false;
            else
                return (bool)Session["STORICIZZAPROCESSILF"];
        }

        private void RemoveSessionStoricizzaProcessi()
        {
            Session.Remove("STORICIZZAPROCESSILF");
        }
        #endregion
    }
}
