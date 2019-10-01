using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.Web.UI.WebControls;
using ICSharpCode.SharpZipLib.GZip;
using SAAdminTool.DocsPaWR;
using SAAdminTool.popup.RubricaDocsPA.CustomControls;
using System.Collections.Generic;
using SAAdminTool.SiteNavigation;
using System.Linq;

namespace SAAdminTool.popup.RubricaDocsPA
{
    /// <exclude></exclude>
    public class RubricaDocsPA : SAAdminTool.CssPage
    {
        protected System.Web.UI.WebControls.TextBox TextBox2;
        protected System.Web.UI.WebControls.DropDownList DropDownList1;
        protected System.Web.UI.WebControls.Panel Panel1;
        protected System.Web.UI.WebControls.Panel Panel2;
        protected System.Web.UI.WebControls.Button Button3;
        protected System.Web.UI.WebControls.Button Button4;

        protected System.Web.UI.WebControls.PlaceHolder phCorr;
        protected System.Web.UI.WebControls.PlaceHolder phA;
        protected System.Web.UI.WebControls.PlaceHolder phCC;
        protected System.Web.UI.WebControls.HyperLink hlLink;

        protected CorrDataGrid dgCorr;
        protected CorrDataGrid dgA;
        protected CorrDataGrid dgCC;

        private string calltype;
        private string objtype;

        private SAAdminTool.DocsPaWR.ElementoRubrica[] _A_recipients;
        private SAAdminTool.DocsPaWR.ElementoRubrica[] _CC_recipients;

        private static bool VISUALIZZA_CORR_RUBRICA_CON_GERARCHIA;
        private static bool RUBRICA_PROTO_USA_SMISTAMENTO;

        protected System.Web.UI.WebControls.ImageButton ibtnMoveToA;
        protected System.Web.UI.WebControls.ImageButton ibtnMoveToCC;
        protected System.Web.UI.WebControls.TextBox tbxCodice;
        protected System.Web.UI.WebControls.CheckBox cbComandi;
        protected System.Web.UI.WebControls.TextBox tbxDescrizione;
        protected System.Web.UI.WebControls.TextBox tbxCitta;
        protected System.Web.UI.WebControls.TextBox tbxLocal;
        protected System.Web.UI.WebControls.TextBox txt_codice_fiscale;
        protected System.Web.UI.WebControls.TextBox txt_partita_iva;
        protected Microsoft.Web.UI.WebControls.TreeView TreeView1;
        protected System.Web.UI.WebControls.CheckBox cbUtenti;
        protected System.Web.UI.WebControls.CheckBox cbRuoli;
        protected DocsPaWebCtrlLibrary.ImageButton btnNuovo;
        protected System.Web.UI.WebControls.ImageButton btnEsporta;
        protected System.Web.UI.WebControls.ImageButton btnImporta;

        protected System.Web.UI.WebControls.Label labelCallType;

        protected System.Web.UI.WebControls.Panel pnlElenco;
        protected System.Web.UI.WebControls.Panel pnlOrganigramma;
        protected System.Web.UI.WebControls.Label lblA;
        protected System.Web.UI.WebControls.Label lblCC;
        protected System.Web.UI.WebControls.Label lbl_registro;
        protected System.Web.UI.WebControls.Label lblDisable;
        protected CorrTreeView tvOrganigramma;
        protected SimpleTabStrip tsMode;

        private const string _A_RECIPIENTS = "_A_RECIPIENTS_";
        private const string _CC_RECIPIENTS = "_CC_RECIPIENTS_";

        private const string HIERARCHY_TAG_RICERCA = "@_RICERCA_@";

        private const int M_ELENCO = 0;
        protected System.Web.UI.HtmlControls.HtmlTableCell td_lblA;
        protected System.Web.UI.WebControls.PlaceHolder phTabStrip;
        private const int M_ORGANIGRAMMA = 1;

        private Hashtable ht_checked;
        private Hashtable ht_not_checked_current_page;
        private bool cfg_simple;
        protected System.Web.UI.WebControls.CheckBox cbSelAll;
        protected System.Web.UI.WebControls.DropDownList ddlIE;
        protected System.Web.UI.WebControls.DropDownList ddl_rf;
        protected System.Web.UI.WebControls.ImageButton btnAnnulla;
        protected System.Web.UI.WebControls.ImageButton btnOk;
        protected System.Web.UI.WebControls.ImageButton btnCerca;
        protected System.Web.UI.WebControls.ImageButton btnReset;
        protected System.Web.UI.WebControls.CheckBox cbListeDist;
        private ScrollKeeper tvorg_sk = null;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtLoadCommand;
        protected System.Web.UI.WebControls.CheckBox chkRubricaComune;
        protected System.Web.UI.WebControls.Panel pnlRubricaComune;
        protected System.Web.UI.WebControls.CheckBox cb_rf;

        protected System.Web.UI.WebControls.TextBox txt_mail;

        private SelectorFilter sel_filter;

        internal AddressbookTipoUtente TipoIE_Corrente
        {
            get { return select_tipoIE(); }
        }

        static RubricaDocsPA()
        {
            string cfg_val = ConfigSettings.getKey("VISUALIZZA_CORR_RUBRICA_CON_GERARCHIA");
            VISUALIZZA_CORR_RUBRICA_CON_GERARCHIA = (cfg_val == "1");

            cfg_val = ConfigSettings.getKey("RUBRICA_PROTO_USA_SMISTAMENTO");
            RUBRICA_PROTO_USA_SMISTAMENTO = (cfg_val == "1");
        }

        void handle_proto_giallo()
        {
            if (Session["protoGiallo"] != null &&
                ((bool)Session["protoGiallo"]) == true)
            {
                Session.Add("protoGiallo", false);
            }
        }

        private void Page_Load(object sender, System.EventArgs e)
        {
            Page.GetPostBackClientEvent(btnCerca, "");

            // Se il query list contiene "control" viene aggiunto in sessione un valore che consente di gestire la presenza di
            // due o più controlli AuthirOwnerFilter
            if (!String.IsNullOrEmpty(Request["control"]))
                Session["ControlSelected"] = Request["control"];
            
            if(!IsPostBack)
                this.hlLink.NavigateUrl = "exportCorrispondenti.xls";

            //char[] separator ={ ';' };
            //String[] prefissi = null;
            //if (System.Configuration.ConfigurationManager.AppSettings["ABILITAZIONE_FLAG_RUBRICA"] != null)
            //{
            //    prefissi = System.Configuration.ConfigurationManager.AppSettings["ABILITAZIONE_FLAG_RUBRICA"].Split(separator);
            //}       

            cfg_simple = ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_SEMPLIFICATA) != null &&
                ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_SEMPLIFICATA) == "1";

            Response.Expires = -1;

            //tbxDescrizione.AutoPostBack = true;

            if (!Response.Filter.GetType().Equals(typeof(System.IO.Compression.GZipStream)))
            {
                Response.Filter = new Utils.GZipFilter(Response.Filter);
                Response.AppendHeader("Content-Encoding", "gzip");
            }

            //DocsPaWR.ElementoRubrica[] corrSearch = null;

            DefaultButton(this, ref tbxCitta, ref btnCerca);
            DefaultButton(this, ref tbxDescrizione, ref btnCerca);
            DefaultButton(this, ref tbxCodice, ref btnCerca);
            DefaultButton(this, ref tbxLocal, ref btnCerca);
            DefaultButton(this, ref txt_codice_fiscale, ref btnCerca);
            DefaultButton(this, ref txt_partita_iva, ref btnCerca);

            tsMode = new SimpleTabStrip();
            tsMode.ID = "tsMode";
            tsMode.BackColor = Color.FromArgb(234, 234, 234);
            tsMode.Tabs.Add(new SimpleTabStrip.Tab("../../images/rubrica/b_el_sel.gif", "../../images/rubrica/b_el_unsel.gif"));
            tsMode.Tabs.Add(new SimpleTabStrip.Tab("../../images/rubrica/b_org_sel.gif", "../../images/rubrica/b_org_unsel.gif"));
            tsMode.SelectedIndexChange += new EventHandler(tsMode_SelectedIndexChange);

            dgCorr = new CorrDataGrid();
            dgCorr.Name = "dgCorr";
            dgCorr.HierarchyCssClass = "dg_hierarchy_element";

            if (cfg_simple)
            {
                dgCorr.OpenSubItems += new OpenSubItemsHandler(dgCorr_OpenSubItems_Simple);
                dgCorr.HierarchyElementSelected += new HierarchyElementSelectedHandler(dgCorr_HierarchyElementSelected);
            }
            else
                dgCorr.OpenSubItems += new OpenSubItemsHandler(dgCorr_OpenSubItems);

            dgCorr.PageIndexChanged += new PageIndexChangedHandler(dgCorr_PageIndexChanged);
            dgCorr.DetailSelected += new DetailSelectedHandler(dgCorr_DetailSelected);
            dgCorr.VerifyItemSelection += new VerifyItemSelectionHandler(dgCorr_VerifyItemSelection);
            dgA = new CorrDataGrid();
            dgA.Name = "dgA";

            dgA.DisplayOnly = true;
            dgA.PageIndexChanged += new PageIndexChangedHandler(dgA_PageIndexChanged);
            dgA.RemoveItem += new RemoveItemHandler(dgA_RemoveItem);
            dgA.RemoveAll += new EventHandler(dgA_RemoveAll);
            dgA.ShowSelectors = false;
            dgA.ShowRemoveButtons = true;
            dgA.ShowItemHierarchyInList = VISUALIZZA_CORR_RUBRICA_CON_GERARCHIA;

            dgCC = new CorrDataGrid();
            dgCC.Name = "dgCC";
            dgCC.DisplayOnly = true;
            dgCC.PageIndexChanged += new PageIndexChangedHandler(dgCC_PageIndexChanged);
            dgCC.RemoveItem += new RemoveItemHandler(dgCC_RemoveItem);
            dgCC.RemoveAll += new EventHandler(dgCC_RemoveAll);
            dgCC.ShowSelectors = false;
            dgCC.ShowRemoveButtons = true;
            dgCC.ShowItemHierarchyInList = VISUALIZZA_CORR_RUBRICA_CON_GERARCHIA;

            dgCorr.AllowPaging = true;
            dgCorr.ShowRemoveAll = false;
            dgCorr.AllowCustomPaging = true;
            dgCorr.PageSize = 16;
            dgCorr.PagerStyle.Mode = PagerMode.NumericPages;
            dgCorr.FooterStyle.CssClass = "menu_1_bianco_dg";

            dgA.AllowPaging = true;
            dgA.AllowCustomPaging = true;
            dgA.PageSize = 7;
            dgA.PagerStyle.Mode = PagerMode.NumericPages;

            dgCC.AllowPaging = true;
            dgCC.AllowCustomPaging = true;
            dgCC.PageSize = 7;
            dgCC.PagerStyle.Mode = PagerMode.NumericPages;

            phCorr.Controls.Add(dgCorr);
            phA.Controls.Add(dgA);
            phCC.Controls.Add(dgCC);
            dgCorr.ID = "dgCorr";
            if (!cfg_simple)
                phTabStrip.Controls.Add(tsMode);

            tvOrganigramma = new CorrTreeView();
            if (this.ViewState["OrgSingolo"] != null && (bool)this.ViewState["OrgSingolo"])
            {
                tvOrganigramma.SelectedOrganigramma = (bool)this.ViewState["OrgSingolo"];
            }
            tvOrganigramma.Expand += new ClickEventHandler(tvOrganigramma_Expand);
            tvOrganigramma.Collapse += new ClickEventHandler(tvOrganigramma_Collapse);

            pnlOrganigramma.Controls.Add(tvOrganigramma);
            tvOrganigramma.CssClass = "testo_grigio";
            tvOrganigramma.SelectedCssClass = "testo_grigio_scuro";

            tvorg_sk = new ScrollKeeper();
            tvorg_sk.WebControl = tvOrganigramma.ClientID;
            pnlOrganigramma.Controls.Add(tvorg_sk);

            RegisterRequiresPostBack(dgCorr);
            RegisterRequiresPostBack(dgA);
            RegisterRequiresPostBack(dgCC);

            if (!cfg_simple)
                RegisterRequiresPostBack(tsMode);

            if (!IsPostBack)
                ddlIE.SelectedValue = "IE";

            _A_recipients = new SAAdminTool.DocsPaWR.ElementoRubrica[0];
            _CC_recipients = new SAAdminTool.DocsPaWR.ElementoRubrica[0];

            if (Request.Form[RubricaDocsPA._A_RECIPIENTS] != null && Request.Form[RubricaDocsPA._A_RECIPIENTS] != "")
            {
                _A_recipients = Utils.ER_Array_Deserialize(Request.Form[RubricaDocsPA._A_RECIPIENTS]);
            }

            if (Request.Form[RubricaDocsPA._CC_RECIPIENTS] != null && Request.Form[RubricaDocsPA._CC_RECIPIENTS] != "")
            {
                _CC_recipients = Utils.ER_Array_Deserialize(Request.Form[RubricaDocsPA._CC_RECIPIENTS]);
            }

            RegisterStartupScript("set_focus", "<script language=\"javascript\">document.getElementById(\"" + tbxDescrizione.ClientID + "\").focus();</script>");
            if (IsPostBack)
            {
                //				corrSearch = (SAAdminTool.DocsPaWR.ElementoRubrica[]) Session["current_search_resultset"];
                ht_checked = (Hashtable)this.ViewState["ht_checked"];
            }
            else
            {
                Session.Remove("current_search_resultset");


                //GESTIONE SELEZIONE DEI FLAG IN RUBRICA
                if (System.Configuration.ConfigurationManager.AppSettings["ABILITAZIONE_FLAG_RUBRICA"] != null
                    && (cbComandi.Visible && cbRuoli.Visible && cbUtenti.Visible))
                {
                    char[] separator ={ '|' };
                    String[] prefissi = System.Configuration.ConfigurationManager.AppSettings["ABILITAZIONE_FLAG_RUBRICA"].Split(separator);
                    for (int i = 0; i < prefissi.Length; i++)
                    {
                        if (cbComandi.Enabled)
                        {
                            cbComandi.Checked = (prefissi[0].ToUpper().Equals("Y"));
                        }
                        if (cbRuoli.Enabled)
                        {
                            cbRuoli.Checked = (prefissi[2].ToUpper().Equals("Y"));
                        }
                        if (cbUtenti.Enabled)
                        {
                            cbUtenti.Checked = (prefissi[1].ToUpper().Equals("Y"));
                        }
                        //caso per le liste: se sono abilitate in amministrazione bisogna consentirne o meno
                        //l'abilitazione del flag da web.config
                        if (cbListeDist.Enabled)
                        {
                            if (prefissi.Length > 3 && prefissi[3] != null)
                            {
                                cbListeDist.Checked = (prefissi[3].ToUpper().Equals("Y"));
                            }
                        }
                    }
                }
                else // come era prima
                {
                    cbComandi.Checked = true;
                    cbRuoli.Checked = true;
                    cbUtenti.Checked = true;
                }

                tsMode.SelectedIndex = M_ELENCO;
                set_mode(M_ELENCO);
                this.ViewState["calltype"] = Request.QueryString["calltype"];
                this.ViewState["objtype"] = Request.QueryString["objtype"];
                ht_checked = new Hashtable();
            }

            calltype = (string)this.ViewState["calltype"];
            objtype = (string)this.ViewState["objtype"];

            dgCorr.CallType = calltype;

            if (((RubricaCallType)Convert.ToInt32(calltype) != RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE) && ((RubricaCallType)Convert.ToInt32(calltype) != RubricaCallType.CALLTYPE_MITT_MODELLO_TRASM) && ((RubricaCallType)Convert.ToInt32(calltype) != RubricaCallType.CALLTYPE_REPLACE_ROLE) && ((RubricaCallType)Convert.ToInt32(calltype) != RubricaCallType.CALLTYPE_FIND_ROLE) /*&& ((RubricaCallType)Convert.ToInt32 (calltype) != RubricaCallType.CALLTYPE_DEST_MODELLO_TRASM)*/)
            {
                tvOrganigramma.CodUoAppartenenza = (UserManager.getRuolo(this)).uo.codiceRubrica;
                tvOrganigramma.InfoUtente = UserManager.getInfoUtente(this);
                dgCorr.CodUoAppartenenza = (UserManager.getRuolo(this)).uo.codiceRubrica;
                dgCorr.InfoUtente = UserManager.getInfoUtente(this);
            }

            if (calltype != null)
            {
                //				//Federica 12 gennaio 2007
                //				if(((RubricaCallType)Convert.ToInt32 (calltype) != RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE) /*&& ((RubricaCallType)Convert.ToInt32 (calltype) != RubricaCallType.CALLTYPE_MITT_MODELLO_TRASM) && ((RubricaCallType)Convert.ToInt32 (calltype) != RubricaCallType.CALLTYPE_DEST_MODELLO_TRASM)*/)
                //					sel_filter = new SelectorFilter (this, (RubricaCallType)Convert.ToInt32 (calltype));
                //				
                HtmlTableCell tc = (HtmlTableCell)FindControl("td_lblA");
                this.btnEsporta.Visible = false;
                this.btnImporta.Visible = false;

                this.debbug_callType((RubricaCallType)Convert.ToInt32(calltype));
               
                switch ((RubricaCallType)Convert.ToInt32(calltype))
                {

                    case RubricaCallType.CALLTYPE_PROTO_IN:
                    case RubricaCallType.CALLTYPE_PROTO_IN_INT:
                        ibtnMoveToCC.Visible = false;
                        dgCC.Visible = false;
                        lblCC.Visible = false;
                        lblA.Text = "Da";
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        ddlIE.Enabled = true;
                        if (!this.IsPostBack)
                            this.ddlIE.SelectedValue = "E";
                        cbSelAll.Visible = false;
                        handle_proto_giallo();
                        cbRuoli.Visible = true;
                        cbUtenti.Visible = true;
                        cbComandi.Visible = true;
                        cbListeDist.Checked = false;
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_PROTO_INT_MITT:
                        ibtnMoveToCC.Visible = false;
                        dgCC.Visible = false;
                        lblCC.Visible = false;
                        lblA.Text = "Da";
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        ddlIE.Enabled = false;
                        cbSelAll.Visible = false;
                        handle_proto_giallo();
                        cbRuoli.Visible = true;
                        cbUtenti.Visible = true;
                        cbComandi.Visible = true;
                        cbListeDist.Checked = false;
                        //disabilito il pulsante Nuovo Corrispondente per i mittenti del protocollo interno
                        this.btnNuovo.Enabled = false;
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_PROTO_OUT_MITT:
                    case RubricaCallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO:
                        ibtnMoveToCC.Visible = false;
                        dgCC.Visible = false;
                        lblCC.Visible = false;
                        lblA.Text = "Da";
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        ddlIE.Enabled = false;
                        cbSelAll.Visible = false;
                        handle_proto_giallo();
                        cbRuoli.Visible = true;
                        cbUtenti.Visible = true;
                        cbComandi.Visible = true;
                        cbListeDist.Checked = false;
                        //disabilito il pulsante Nuovo Corrispondente per i mittenti del protocollo in uscita
                        this.btnNuovo.Enabled = false;
                        this.lbl_registro.Visible = false;
                        this.ddl_rf.Visible = false;
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI:
                        ibtnMoveToCC.Visible = false;
                        dgCC.Visible = false;
                        lblCC.Visible = false;
                        lblA.Text = "A";
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        ddlIE.Enabled = false;
                        cbSelAll.Visible = false;
                        handle_proto_giallo();
                        if (!IsPostBack)
                        {
                            cbRuoli.Visible = true;
                            cbRuoli.Enabled = true;
                            cbUtenti.Visible = true;
                            cbUtenti.Enabled = true;
                            cbUtenti.Checked = false;
                            cbComandi.Visible = true;
                            cbComandi.Enabled = true;
                            cbComandi.Checked = false;
                        }
                        cbListeDist.Checked = false;
                        //disabilito il pulsante Nuovo Corrispondente per i mittenti del protocollo in uscita
                        this.btnNuovo.Enabled = false;
                        this.lbl_registro.Visible = false;
                        this.ddl_rf.Visible = false;
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_RICERCA_MITTDEST:
                    case RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO:
                    case RubricaCallType.CALLTYPE_RICERCA_ESTESA:
                    case RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO:
                    case RubricaCallType.CALLTYPE_RICERCA_DOCUMENTI:
                        ibtnMoveToCC.Visible = false;
                        dgCC.Visible = false;
                        lblCC.Visible = false;
                        lblA.Text = "Da";
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        ddlIE.Enabled = false;
                        cbSelAll.Visible = false;
                        handle_proto_giallo();
                        cbListeDist.Checked = false;
                        //disabilito il pulsante Nuovo Corrispondente nelle ricerche dei documenti
                        this.btnNuovo.Enabled = false;
                        this.ddlIE.SelectedValue = "IE";
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT:
                        ibtnMoveToCC.Visible = false;
                        dgCC.Visible = false;
                        lblCC.Visible = false;
                        lblA.Text = "Da";
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        ddlIE.Enabled = false;
                        cbSelAll.Visible = false;
                        handle_proto_giallo();
                        cbListeDist.Checked = false;
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_PROTO_OUT:
                    case RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO:
                        ibtnMoveToCC.Visible = true;
                        dgCC.Visible = true;
                        lblCC.Visible = true;
                        lblA.Text = "A";
                        lblCC.Text = "CC";
                        dgCorr.AllowMultipleSelection = true;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        ddlIE.Enabled = true;
                        cb_rf.Visible = true;
                        //cb_rf.Checked = true;
                        cbListeDist.Visible = true;
                        handle_proto_giallo();
                        break;

                    case RubricaCallType.CALLTYPE_PROTO_INT_DEST:
                        ibtnMoveToCC.Visible = true;
                        dgCC.Visible = true;
                        lblCC.Visible = true;
                        lblA.Text = "A";
                        lblCC.Text = "CC";
                        dgCorr.AllowMultipleSelection = true;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        ddlIE.Enabled = false;
                        //						lblIE.Visible = false;
                        cbListeDist.Visible = true;
                        //cbListeDist.Checked = true;
                        //disabilito nuovo corrisp per i destinatari interni
                        this.btnNuovo.Enabled = false;
                        handle_proto_giallo();
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_TRASM_INF:
                    case RubricaCallType.CALLTYPE_TRASM_SUP:
                    case RubricaCallType.CALLTYPE_TRASM_ALL:
                    case RubricaCallType.CALLTYPE_TRASM_PARILIVELLO:
                        ibtnMoveToCC.Visible = false;
                        dgCC.Visible = false;
                        lblCC.Visible = false;
                        lblA.Text = "A";
                        dgCorr.AllowMultipleSelection = true;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        ddlIE.Enabled = false;
                        //						lblIE.Visible = false;
                        //disabilito nuovo corrisp in Nuova Trasmissione 
                        this.btnNuovo.Enabled = false;
                        cbListeDist.Visible = true;
                        cb_rf.Visible = true;
                        break;

                    case RubricaCallType.CALLTYPE_MANAGE:
                        if (!UserManager.ruoloIsAutorized(this, "EXPORT_RUBRICA"))
                            this.btnEsporta.Visible = false;
                        else
                        this.btnEsporta.Visible = true;
                      
                        if (!UserManager.ruoloIsAutorized(this, "IMPORT_RUBRICA"))
                            this.btnImporta.Visible = false;
                        else
                            this.btnImporta.Visible = true;


                       
                    


                        //this.btnEsporta.Visible = true;
                        //this.btnImporta.Visible = true;
                        ibtnMoveToCC.Visible = false;
                        ibtnMoveToA.Visible = false;
                        dgCC.Visible = false;
                        dgA.Visible = false;
                        lblCC.Visible = false;
                        lblA.Visible = false;
                        dgCorr.ShowSelectors = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.None;
                        ddlIE.Enabled = false;
                        if (!this.IsPostBack)
                        {
                            this.ddlIE.SelectedValue = "E";
                            this.ddlIE.Items[ddlIE.SelectedIndex].Text = this.ddlIE.Items[ddlIE.SelectedIndex].Text + " Amm.ne";
                        }
                        cbSelAll.Visible = false;
                        cbListeDist.Checked = false;
                        tsMode.Tabs.RemoveAt(1);
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_UFFREF_PROTO:
                    case RubricaCallType.CALLTYPE_GESTFASC_LOCFISICA:
                    case RubricaCallType.CALLTYPE_GESTFASC_UFFREF:
                    case RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA:
                    case RubricaCallType.CALLTYPE_EDITFASC_UFFREF:
                    case RubricaCallType.CALLTYPE_NEWFASC_LOCFISICA:
                    case RubricaCallType.CALLTYPE_NEWFASC_UFFREF:
                    case RubricaCallType.CALLTYPE_RICERCA_UFFREF:
                    case RubricaCallType.CALLTYPE_FILTRIRICFASC_UFFREF:
                    case RubricaCallType.CALLTYPE_FILTRIRICFASC_LOCFIS:
                    case RubricaCallType.CALLTYPE_STAMPA_REG_UO:
                        ibtnMoveToCC.Visible = false;
                        dgCC.Visible = false;
                        lblCC.Visible = false;
                        lblA.Text = "Da";
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        ddlIE.Enabled = false;
                        cbSelAll.Visible = false;
                        cbRuoli.Visible = false;
                        cbUtenti.Visible = false;
                        cbComandi.Visible = false;
                        handle_proto_giallo();
                        cbListeDist.Checked = false;
                        //disabilito nuovo corrisp 
                        this.btnNuovo.Enabled = false;
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_PROTO_INGRESSO:
                        if (!this.IsPostBack)
                            this.ddlIE.SelectedValue = "E";
                        ibtnMoveToCC.Visible = false;
                        dgCC.Visible = false;
                        lblCC.Visible = false;
                        lblA.Text = "Da";
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        cbSelAll.Visible = false;
                        handle_proto_giallo();
                        cbListeDist.Checked = false;
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_RICERCA_TRASM:
                    case RubricaCallType.CALLTYPE_RICERCA_TRASM_TODOLIST:
                    case RubricaCallType.CALLTYPE_RICERCA_CREATOR:
                    case RubricaCallType.CALLTYPE_OWNER_AUTHOR:
                    case RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO:
                    case RubricaCallType.CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI:
                    case RubricaCallType.CALLTYPE_TUTTI_RUOLI:
                    case RubricaCallType.CALLTYPE_TUTTE_UO:
                        ibtnMoveToCC.Visible = false;
                        dgCC.Visible = false;
                        lblCC.Visible = false;
                        //disabilito nuovo corrisp in ricerca trasmissioni
                        this.btnNuovo.Enabled = false;
                        lblA.Text = "Da";
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        cbSelAll.Visible = false;
                        cbComandi.Visible = true;
                        cbRuoli.Visible = false;
                        cbUtenti.Visible = false;
                        cbComandi.Visible = false;
                        ddlIE.Enabled = false;
                        tsMode.Tabs.RemoveAt(1);
                        tsMode.ClearEvents();
                        cbListeDist.Checked = false;
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE:
                        cbListeDist.Checked = false;
                        cbListeDist.Visible = false;
                        lblCC.Visible = false;
                        ibtnMoveToCC.Visible = false;
                        phCC.Visible = false;
                        lblA.Text = "A";
                        btnNuovo.Enabled = false;
                        if (Session["AMMDATASET"] != null)
                        ddlIE.Enabled = false;
                        else
                        {
                            ddlIE.Enabled = true;
                            this.ddl_rf.Visible = false;
                            this.lbl_registro.Visible = false;
                        }
                        break;

                    case RubricaCallType.CALLTYPE_MITT_MODELLO_TRASM:
                    case RubricaCallType.CALLTYPE_FIND_ROLE:
                        cbComandi.Checked = false;
                        cbComandi.Enabled = false;
                        cbRuoli.Checked = true;
                        cbRuoli.Enabled = false;
                        cbUtenti.Checked = false;
                        cbUtenti.Enabled = false;
                        cbListeDist.Checked = false;
                        cbListeDist.Visible = false;
                        lblCC.Visible = false;
                        ibtnMoveToCC.Visible = false;
                        phCC.Visible = false;
                        ddlIE.Enabled = false;
                        btnNuovo.Enabled = false;
                        dgCorr.AllowMultipleSelection = true;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        //dgA.PageSize = 1;
                        tc.VAlign = "top";
                        cbSelAll.Visible = false;
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        lblA.Text = "DA";
                        break;

                    case RubricaCallType.CALLTYPE_REPLACE_ROLE:
                        cbComandi.Checked = false;
                        cbComandi.Enabled = false;
                        cbRuoli.Checked = true;
                        cbRuoli.Enabled = false;
                        cbUtenti.Checked = false;
                        cbUtenti.Enabled = false;
                        cbListeDist.Checked = false;
                        cbListeDist.Visible = false;
                        lblCC.Visible = false;
                        ibtnMoveToCC.Visible = false;
                        phCC.Visible = false;
                        ddlIE.Enabled = false;
                        btnNuovo.Enabled = false;
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        //dgA.PageSize = 1;
                        tc.VAlign = "top";
                        cbSelAll.Visible = false;
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        lblA.Text = "DA";
                        break;

                    case RubricaCallType.CALLTYPE_RUOLO_REG_NOMAIL:
                    case RubricaCallType.CALLTYPE_RUOLO_RESP_REG:
                    case RubricaCallType.CALLTYPE_RUOLO_RESP_REPERTORI:
                        cbComandi.Checked = false;
                        cbComandi.Enabled = false;
                        cbRuoli.Checked = true;
                        cbRuoli.Enabled = false;
                        cbUtenti.Checked = false;
                        cbUtenti.Enabled = false;
                        cbListeDist.Checked = false;
                        cbListeDist.Visible = false;
                        lblCC.Visible = false;
                        ibtnMoveToCC.Visible = false;
                        phCC.Visible = false;
                        ddlIE.Enabled = false;
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        cbSelAll.Visible = false;
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        lblA.Text = "DA";
                        break;

                    case RubricaCallType.CALLTYPE_UTENTE_REG_NOMAIL:
                        cbComandi.Checked = false;
                        cbComandi.Enabled = false;
                        cbRuoli.Checked = false;
                        cbRuoli.Enabled = false;
                        cbUtenti.Checked = true;
                        cbUtenti.Enabled = false;
                        cbListeDist.Checked = false;
                        cbListeDist.Visible = false;
                        lblCC.Visible = false;
                        ibtnMoveToCC.Visible = false;
                        phCC.Visible = false;
                        ddlIE.Enabled = false;
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.RadioButton;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        cbSelAll.Visible = false;
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        lblA.Text = "DA";
                        break;

                    case RubricaCallType.CALLTYPE_MODELLI_TRASM_INF:
                    case RubricaCallType.CALLTYPE_MODELLI_TRASM_SUP:
                    case RubricaCallType.CALLTYPE_MODELLI_TRASM_ALL:
                    case RubricaCallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO:
                        //						cbComandi.Checked = false;
                        //						cbComandi.Enabled = false;
                        //						cbRuoli.Checked = true;
                        //						cbRuoli.Enabled = true;
                        //						cbUtenti.Checked = true;
                        //						cbUtenti.Enabled = true;
                        cbListeDist.Visible = true;
                        lblCC.Visible = false;
                        ibtnMoveToCC.Visible = false;
                        phCC.Visible = false;
                        ddlIE.Enabled = false;
                        btnNuovo.Enabled = false;
                        dgCorr.AllowMultipleSelection = true;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        //						dgA.PageSize = 1;
                        tc.VAlign = "top";
                        cbSelAll.Visible = false;
                        lblA.Text = "A";
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_DEST_MODELLO_TRASM:
                        cbListeDist.Checked = true;
                        cbListeDist.Visible = true;
                        lblCC.Visible = false;
                        ibtnMoveToCC.Visible = false;
                        phCC.Visible = false;
                        ddlIE.Enabled = false;
                        btnNuovo.Enabled = false;
                        dgCorr.AllowMultipleSelection = true;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        tc.VAlign = "top";
                        cbSelAll.Visible = false;
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        lblA.Text = "A";
                        break;

                    case RubricaCallType.CALLTYPE_CORR_INT:
                        cbListeDist.Checked = false;
                        cbListeDist.Visible = false;
                        lblCC.Visible = false;
                        ibtnMoveToCC.Visible = false;
                        phCC.Visible = false;
                        ddlIE.Enabled = false;
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        cbSelAll.Visible = false;
                        lblA.Text = "A";
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_CORR_EST:
                        cbListeDist.Checked = false;
                        cbListeDist.Visible = false;
                        lblCC.Visible = false;
                        ibtnMoveToCC.Visible = false;
                        phCC.Visible = false;
                        ddlIE.Enabled = false;
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        cbSelAll.Visible = false;
                        lblA.Text = "A";
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_CORR_INT_EST:
                        cbListeDist.Checked = false;
                        cbListeDist.Visible = false;
                        lblCC.Visible = false;
                        ibtnMoveToCC.Visible = false;
                        phCC.Visible = false;
                        ddlIE.Enabled = false;
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        cbSelAll.Visible = false;
                        lblA.Text = "A";
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;

                    case RubricaCallType.CALLTYPE_CORR_NO_FILTRI:
                    case RubricaCallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO:
                        cbListeDist.Checked = false;
                        cbListeDist.Visible = false;
                        lblCC.Visible = false;
                        ibtnMoveToCC.Visible = false;
                        phCC.Visible = false;
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        cbSelAll.Visible = false;
                        lblA.Text = "A";
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;
                    case RubricaCallType.CALLTYPE_MITT_MULTIPLI:
                    case RubricaCallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO:
                        lblA.Text = "Da";
                        ibtnMoveToCC.Visible = false;
                        dgCC.Visible = false;
                        lblCC.Visible = false;
                        dgCorr.AllowMultipleSelection = true;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        ddlIE.Enabled = true;
                        cb_rf.Visible = false;
                        cb_rf.Checked = false;
                        //cbListeDist.Checked = false;
                        cbListeDist.Visible = true;
                        if (!this.IsPostBack)
                            this.ddlIE.SelectedValue = "E";
                        cbRuoli.Visible = true;
                        cbUtenti.Visible = true;
                        cbComandi.Visible = true;
                        cb_rf.Visible = false;
                        
                        //handle_proto_giallo();
                        break;

                    case RubricaCallType.CALLTYPE_CORR_INT_NO_UO:
                        cbListeDist.Checked = false;
                        cbListeDist.Visible = false;
                        lblCC.Visible = false;
                        ibtnMoveToCC.Visible = false;
                        phCC.Visible = false;
                        ddlIE.Enabled = false;
                        cbComandi.Visible = false;
                        dgCorr.AllowMultipleSelection = false;
                        tvOrganigramma.SelectorType = TreeViewSelectorType.CheckBox;
                        dgA.PageSize = 1;
                        tc.VAlign = "top";
                        cbSelAll.Visible = false;
                        lblA.Text = "A";
                        cb_rf.Visible = false;
                        //cb_rf.Checked = false;
                        break;
                }
                hlLink.Visible = (btnImporta.Visible || btnEsporta.Visible);
                select_tipoIE();
            }
            if ((System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null && System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "0") || System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == null)
            {
                cbListeDist.Visible = false;
                cbListeDist.Checked = false;
            }

            if (!IsPostBack)
            {
                this.btnEsporta.Attributes.Add("onclick", "StampaCorrispondenti();");
                this.btnImporta.Attributes.Add("onclick", "ApriImportaCorrispondenti();");
                if (ddlIE.Enabled)
                {
                    if((RubricaCallType)Convert.ToInt32(calltype) != RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE) 
                    caricaComboRf();
                }
                else
                {
                    if (!ddlIE.Enabled && ddlIE.SelectedValue == "E")
                    {
                        //caso di gestione rubrica..devo caricare la combo dei registri, con tutti i registri 
                        //visibili dall'utente
                    }
                }
                if ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_OUT || (RubricaCallType)Convert.ToInt32(calltype) ==
                     RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                {
                    cb_rf.Checked = true;
                }
                else
                {
                    cb_rf.Checked = false;
                }
            }
            else
            {
                if (ddlIE.Enabled && ddl_rf.Visible)
                {
                    //prendo il valore selezionato
                    char[] sep = { '_' };
                    bool rfPresente = false;

                    foreach (ListItem item in ddl_rf.Items)
                    {
                        string[] datiSelezione = item.Value.Split(sep);

                        if (datiSelezione[1] != null && datiSelezione[1] != "")//se è un RF
                        {
                            rfPresente = true;
                            break;
                        }
                    }
                    if (rfPresente)
                        this.lbl_registro.Text = "Registro/RF";
                    else
                        this.lbl_registro.Text = "Registro";
                }
            }
            DocsPaWebService ws = new DocsPaWebService();
            cb_rf.Visible =ws.IsEnabledRF(string.Empty);

            if ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO || (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI || (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_TUTTI_RUOLI || (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_TUTTE_UO)
            {
                cb_rf.Visible = false;
            }
        }

        private void caricaComboRf()
        {
            DocsPaWR.Registro[] userRegistri = null;
            HttpContext ctx = HttpContext.Current;
            DocsPaWR.Registro[] listaTotale = null;
            Registro registro = null;

            //Carico la combo degli RF
            if (ctx.Session["userRegistro"] != null)
            {
                listaTotale = UserManager.getListaRegistriWithRF(this, "1", ((Registro)ctx.Session["userRegistro"]).systemId);
                registro = (Registro)ctx.Session["userRegistro"];
            }
            else
            {
                listaTotale = UserManager.getListaRegistriWithRF(this, "1", UserManager.getRuolo().registri[0].systemId);
                registro = UserManager.getRuolo().registri[0];
            }

            if (listaTotale != null && listaTotale.Length > 0)
            {
                userRegistri = new Registro[listaTotale.Length + 1];
                userRegistri[0] = registro;
                listaTotale.CopyTo(userRegistri, 1);
                ddl_rf.Enabled = true;
            }
            else
            {
                userRegistri = new Registro[1];
                userRegistri[0] = registro;
                ddl_rf.Enabled = false;
            }

            string strText = "";
            for (short iff = 0; iff < 3; iff++)
            {
                strText += " ";
            }

            for (int i = 0; i < userRegistri.Length; i++)
            {
                string testo = userRegistri[i].codRegistro;
                if (userRegistri[i].chaRF == "1")
                {
                    testo = strText + testo;
                }

                ddl_rf.Items.Add(testo);

                ddl_rf.Items[i].Value = userRegistri[i].systemId + "_" + userRegistri[i].rfDisabled + "_" + userRegistri[i].idAOOCollegata;
                //if (userRegistri[i].chaRF=="1")
                //   this.ddl_rf.Items[i].Attributes.Add("style", " color:Gray");
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ddlIE.SelectedIndexChanged += new System.EventHandler(this.ddlIE_SelectedIndexChanged);
            this.btnOk.Click += new System.Web.UI.ImageClickEventHandler(this.btnOk_Click);
            this.btnCerca.Click += new System.Web.UI.ImageClickEventHandler(this.btnCerca_Click);
            this.btnReset.Click += new System.Web.UI.ImageClickEventHandler(this.btnReset_Click);
            this.cbSelAll.CheckedChanged += new System.EventHandler(this.cbSelAll_CheckedChanged);
            this.ibtnMoveToA.Click += new System.Web.UI.ImageClickEventHandler(this.ibtnMoveToA_Click);
            this.ibtnMoveToCC.Click += new System.Web.UI.ImageClickEventHandler(this.ibtnMoveToCC_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.RubricaDocsPA_PreRender);
        }
        #endregion


        private AddressbookTipoUtente select_tipoIE()
        {

            RubricaCallType ct = (RubricaCallType)Convert.ToInt32(calltype);

            if (!ddlIE.Enabled)
            {
                if (ct == RubricaCallType.CALLTYPE_TRASM_ALL ||
                    ct == RubricaCallType.CALLTYPE_TRASM_INF ||
                    ct == RubricaCallType.CALLTYPE_PROTO_OUT_MITT ||
                    ct == RubricaCallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO ||
                    ct == RubricaCallType.CALLTYPE_TRASM_SUP ||
                    ct == RubricaCallType.CALLTYPE_TRASM_PARILIVELLO ||
                    ct == RubricaCallType.CALLTYPE_UFFREF_PROTO ||
                    ct == RubricaCallType.CALLTYPE_STAMPA_REG_UO ||
                    ct == RubricaCallType.CALLTYPE_GESTFASC_LOCFISICA ||
                    ct == RubricaCallType.CALLTYPE_GESTFASC_UFFREF ||
                    ct == RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA ||
                    ct == RubricaCallType.CALLTYPE_EDITFASC_UFFREF ||
                    ct == RubricaCallType.CALLTYPE_NEWFASC_LOCFISICA ||
                    ct == RubricaCallType.CALLTYPE_NEWFASC_UFFREF ||
                    ct == RubricaCallType.CALLTYPE_RICERCA_UFFREF ||
                    ct == RubricaCallType.CALLTYPE_PROTO_INT_MITT ||
                    ct == RubricaCallType.CALLTYPE_PROTO_INT_DEST ||
                    ct == RubricaCallType.CALLTYPE_FILTRIRICFASC_UFFREF ||
                    ct == RubricaCallType.CALLTYPE_FILTRIRICFASC_LOCFIS ||
                    ct == RubricaCallType.CALLTYPE_RICERCA_TRASM ||
                    ct == RubricaCallType.CALLTYPE_RICERCA_TRASM_TODOLIST ||
                    ct == RubricaCallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT ||
                    ct == RubricaCallType.CALLTYPE_MITT_MODELLO_TRASM ||
                    ct == RubricaCallType.CALLTYPE_MODELLI_TRASM_ALL ||
                    ct == RubricaCallType.CALLTYPE_MODELLI_TRASM_INF ||
                    ct == RubricaCallType.CALLTYPE_MODELLI_TRASM_SUP ||
                    ct == RubricaCallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO ||
                    ct == RubricaCallType.CALLTYPE_DEST_MODELLO_TRASM ||
                    ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA ||
                    ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO ||
                    ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE ||
                    ct == RubricaCallType.CALLTYPE_RUOLO_REG_NOMAIL ||
                    ct == RubricaCallType.CALLTYPE_RUOLO_RESP_REG ||
                    ct == RubricaCallType.CALLTYPE_RUOLO_RESP_REPERTORI ||
                    ct == RubricaCallType.CALLTYPE_UTENTE_REG_NOMAIL ||
                    ct == RubricaCallType.CALLTYPE_RICERCA_CREATOR ||
                    ct == RubricaCallType.CALLTYPE_OWNER_AUTHOR ||
                    ct == RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
                    ct == RubricaCallType.CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI ||
                    ct == RubricaCallType.CALLTYPE_TUTTI_RUOLI ||
                    ct == RubricaCallType.CALLTYPE_TUTTE_UO ||
                    ct == RubricaCallType.CALLTYPE_REPLACE_ROLE ||
                    ct == RubricaCallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI ||
                    ct == RubricaCallType.CALLTYPE_FIND_ROLE
                    )
                {
                    if (ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA ||
                        ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO ||
                        ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO
                        || ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE)
                    {

                        return AddressbookTipoUtente.INTERNO;
                    }
                    else
                    {
                        ddlIE.SelectedValue = "I";
                        return AddressbookTipoUtente.INTERNO;
                    }
                }
                else
                {
                    if (ct == RubricaCallType.CALLTYPE_MANAGE)
                    {
                        ddlIE.SelectedValue = "E";
                        return AddressbookTipoUtente.ESTERNO;
                    }

                    if (ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO)
                    {
                        return AddressbookTipoUtente.INTERNO;
                    }
                }

                if (ct == RubricaCallType.CALLTYPE_CORR_INT || ct == RubricaCallType.CALLTYPE_CORR_INT_NO_UO)
                {
                    ddlIE.SelectedValue = "I";
                    return AddressbookTipoUtente.INTERNO;
                }

                if (ct == RubricaCallType.CALLTYPE_CORR_EST)
                {
                    ddlIE.SelectedValue = "E";
                    return AddressbookTipoUtente.ESTERNO;
                }
            }
            else
            {
                switch (ddlIE.SelectedValue)
                {
                    case "I":
                        return AddressbookTipoUtente.INTERNO;

                    case "E":
                        return AddressbookTipoUtente.ESTERNO;

                    case "IE":
                        {
                            if (ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA
                                || ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO
                                || ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO
                                || ct == RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE)
                            {
                                //se ho cliccato su organigramma, nel caso di interni/Esterni
                                //devo restituire tutto l'organigramma interno escludendo i
                                //corrispondenti esterni all'amministrazione.
                                return AddressbookTipoUtente.INTERNO;

                            }
                            else
                            {
                                return AddressbookTipoUtente.GLOBALE;
                            }
                        }
                }
            }

            ddlIE.SelectedValue = "IE";
            return AddressbookTipoUtente.GLOBALE;
        }

        private void select_item_types(ref SAAdminTool.DocsPaWR.ParametriRicercaRubrica qco)
        {
            switch ((RubricaCallType)Convert.ToInt32(calltype))
            {
                case RubricaCallType.CALLTYPE_UFFREF_PROTO:
                case RubricaCallType.CALLTYPE_GESTFASC_LOCFISICA:
                case RubricaCallType.CALLTYPE_GESTFASC_UFFREF:
                case RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA:
                case RubricaCallType.CALLTYPE_EDITFASC_UFFREF:
                case RubricaCallType.CALLTYPE_NEWFASC_LOCFISICA:
                case RubricaCallType.CALLTYPE_NEWFASC_UFFREF:
                case RubricaCallType.CALLTYPE_RICERCA_UFFREF:
                case RubricaCallType.CALLTYPE_FILTRIRICFASC_UFFREF:
                case RubricaCallType.CALLTYPE_FILTRIRICFASC_LOCFIS:
                case RubricaCallType.CALLTYPE_STAMPA_REG_UO:
                    qco.doUo = true;
                    qco.doRuoli = false;
                    qco.doUtenti = false;
                    break;

                case RubricaCallType.CALLTYPE_RICERCA_TRASM:
                case RubricaCallType.CALLTYPE_RICERCA_TRASM_TODOLIST:
                case RubricaCallType.CALLTYPE_RICERCA_CREATOR:
                case RubricaCallType.CALLTYPE_OWNER_AUTHOR:
                case RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO:
                case RubricaCallType.CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI:
                case RubricaCallType.CALLTYPE_TUTTI_RUOLI:
                case RubricaCallType.CALLTYPE_TUTTE_UO:
                    qco.doUo = (Request.QueryString["tipo_corr"] == "U");
                    qco.doRuoli = (Request.QueryString["tipo_corr"] == "R");
                    qco.doUtenti = (Request.QueryString["tipo_corr"] == "P");
                    break;

                case RubricaCallType.CALLTYPE_ORGANIGRAMMA:
                case RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO:
                case RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO:
                    qco.doUo = true;
                    qco.doRuoli = true;
                    qco.doUtenti = true;
                    qco.doListe = false;
                    break;

                case RubricaCallType.CALLTYPE_CORR_INT_NO_UO:
                    qco.doUo = false;
                    qco.doListe = false;
                    qco.doRuoli = true;
                    qco.doUtenti = true;
                    break;

                default:
                    qco.doUo = cbComandi.Checked;
                    qco.doRuoli = cbRuoli.Checked;
                    qco.doUtenti = cbUtenti.Checked;
                    qco.doListe = (cbListeDist.Enabled && cbListeDist.Checked);
                    qco.doRF = (cb_rf.Enabled && cb_rf.Checked);
                    break;
            }
        }

        private string getCondizioneRegistro(DropDownList ddl_rf)
        {
            //string retValue = " IN ( ";
            string retValue = "";
            foreach (ListItem item in ddl_rf.Items)
            {
                if (item.Value != "")
                {
                    char[] sep = { '_' };
                    string[] currentSelection = item.Value.Split(sep);

                    retValue += " " + currentSelection[0] + ",";
                }
            }
            if (retValue.EndsWith(","))
               retValue = retValue.Remove(retValue.LastIndexOf(","));
            //retValue += " )";
            return retValue;
        }

       private void Search()
        {
            try
            {
                Session.Remove("current_search_resultset");
                dgCorr.SelectedPage = 1;
                dgCorr.ClearHierarchy();
                dgCorr.Columns.Clear();
                tvOrganigramma.Nodes.Clear();
                cbSelAll.Checked = false;

                DocsPaWR.ParametriRicercaRubrica qco = new SAAdminTool.DocsPaWR.ParametriRicercaRubrica();

                /*new */
                
                qco.calltype = (SAAdminTool.DocsPaWR.RubricaCallType)Convert.ToInt32(calltype);

                //if (qco.calltype == RubricaCallType.CALLTYPE_MANAGE)
                //{
                //    if (ddlIE.SelectedValue == "E") // se è selezionato Esterni
                //    {
                //        qco.calltype = RubricaCallType.CALLTYPE_ESTERNI_AMM;
                //    }
                //}
                //else
                //{
                if (ddlIE.SelectedValue == "E") // se è selezionato Esterni
                {
                    if (qco.calltype == RubricaCallType.CALLTYPE_PROTO_OUT 
                        || qco.calltype == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                    {
                        qco.calltype = RubricaCallType.CALLTYPE_PROTO_OUT_ESTERNI;
                    }
                }
                //}
            
                //if (qco.calltype == RubricaCallType.CALLTYPE_PROTO_OUT
                //    || qco.calltype == RubricaCallType.CALLTYPE_PROTO_IN
                //    || qco.calltype == RubricaCallType.CALLTYPE_PROTO_IN_INT)
                //{
                //    if (ddlIE.SelectedValue == "E") // se è selezionato Esterni
                //    {
                //        qco.calltype = RubricaCallType.CALLTYPE_ESTERNI_AMM;
                //    }
                //}

                UserManager.setQueryRubricaCaller(ref qco);

                qco.caller.filtroRegistroPerRicerca = qco.caller.IdRegistro;

                if (qco.calltype == RubricaCallType.CALLTYPE_PROTO_OUT_ESTERNI 
                    || qco.calltype == RubricaCallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO
                    || ((qco.calltype == RubricaCallType.CALLTYPE_PROTO_IN 
                        || qco.calltype == RubricaCallType.CALLTYPE_PROTO_INGRESSO 
                        || qco.calltype == RubricaCallType.CALLTYPE_PROTO_IN_INT 
                        || qco.calltype == RubricaCallType.CALLTYPE_MITT_MULTIPLI 
                        || qco.calltype == RubricaCallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO) 
                        && (ddlIE.SelectedValue=="E" || ddlIE.SelectedValue=="IE")
                        || ((qco.calltype == RubricaCallType.CALLTYPE_PROTO_OUT 
                            || qco.calltype == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                            && ddlIE.SelectedValue == "IE")
                    ))
                {
                    char[] sep = { '_' };
                    string[] currentSelection = ddl_rf.SelectedItem.Value.Split(sep);
                    string condRegistro = "";

                    if (currentSelection != null)
                    {
                        if (currentSelection[1] != string.Empty)
                        {
                            // se è un RF
                           // condRegistro = " IN (" + currentSelection[0] + ")";
                            condRegistro = currentSelection[0];
                        }
                        else
                        {
                            // se è un registro, devo ricercare su tutti i corrispondenti esterni,
                            //appartenenti al registro e agli RF che il ruolo può vedere
                            condRegistro = getCondizioneRegistro(ddl_rf);
                        }
                    }
                    qco.caller.filtroRegistroPerRicerca = condRegistro;
                }

                if (qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_MANAGE
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_ESTESA
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTDEST
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_CORR_EST
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_CORR_INT
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_CORR_INT_EST
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_CORR_NO_FILTRI
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_CORR_INT_NO_UO
                || qco.calltype == RubricaCallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO
                    )
                {
                    //nuova gestione: devo cercare in tutti i registri e RF visibili al ruolo
                    if (qco.caller.IdRuolo != null)                  
                    {
                        DocsPaWR.Registro[] regRuolo = UserManager.getListaRegistriWithRF(qco.caller.IdRuolo, "", "");
                        regRuolo = UserManager.getListaRegistriWithRF(qco.caller.IdRuolo, "", "");
                        string filtroRegistro = "";
                        for (int i = 0; i < regRuolo.Length; i++)
                        {
                            filtroRegistro = filtroRegistro + regRuolo[i].systemId;
                            if (i < regRuolo.Length - 1)
                            {
                                filtroRegistro = filtroRegistro + " , ";
                            }
                        }
                        qco.caller.filtroRegistroPerRicerca = filtroRegistro;
                        if (qco.calltype == RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE && !string.IsNullOrEmpty(filtroRegistro))
                            qco.caller.IdRegistro = filtroRegistro;
                    }
                    
                    else
                    {   
                        string idAmm = UserManager.getInfoUtente().idAmministrazione;
                        string infoAmm = UserManager.getInfoAmmCorrente(idAmm).Codice;
                        DocsPaWR.OrgRegistro[] regRuoloOrg = UserManager.getRegistriByCodAmm(infoAmm, "0");
                        string filtroRegistro = "";
                        for (int i = 0; i < regRuoloOrg.Length; i++)
                        {
                            filtroRegistro = filtroRegistro + regRuoloOrg[i].IDRegistro;
                            if (i < regRuoloOrg.Length - 1)
                            {
                                filtroRegistro = filtroRegistro + " , ";
                            }
                        }
                        qco.caller.filtroRegistroPerRicerca = filtroRegistro;
                        if (qco.calltype == RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE && !string.IsNullOrEmpty(filtroRegistro))
                            qco.caller.IdRegistro = filtroRegistro;
                    }

                }

                qco.parent = "";
                qco.citta = tbxCitta.Text;
                qco.codice = tbxCodice.Text;
                qco.descrizione = tbxDescrizione.Text;
                qco.localita = tbxLocal.Text;
                qco.tipoIE = select_tipoIE();
                //old qco.calltype = (SAAdminTool.DocsPaWR.RubricaCallType) Convert.ToInt32 (calltype);
                qco.ObjectType = objtype;
                qco.doRubricaComune = this.DoRicercaRubricaComune;
                qco.email = this.txt_mail.Text;
                
                //Aggiungo i parametri di ricerca per codice fiscale e partita iva
                qco.codiceFiscale = this.txt_codice_fiscale.Text;
                qco.partitaIva = this.txt_partita_iva.Text;
                select_item_types(ref qco);

                //popolamento oggetto per RUBRICA_USA_PROTO_SMISTAMENTO
                DocsPaWR.SmistamentoRubrica smistamentoRubrica = new SmistamentoRubrica();
                //smistamentoRubrica.smistamento: indica se è abilitata o meno lo smistamento
                smistamentoRubrica.smistamento = SAAdminTool.Utils.getAbilitazioneSmistamento();
                smistamentoRubrica.daFiltrareSmistamento = "0";
                if ( qco.calltype ==RubricaCallType.CALLTYPE_PROTO_INT_DEST
                    || qco.calltype == RubricaCallType.CALLTYPE_PROTO_OUT
                    || qco.calltype == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                {
                    smistamentoRubrica.daFiltrareSmistamento = "1";
                }

                //callType
                smistamentoRubrica.calltype = qco.calltype;

                //InfoUtente
                DocsPaWR.InfoUtente infoUt = new SAAdminTool.DocsPaWR.InfoUtente();
                infoUt = UserManager.getInfoUtente(this);
                smistamentoRubrica.infoUt = infoUt;

                //Ruolo Protocollatore
                if (Session["userRuolo"] != null)
                {
                    smistamentoRubrica.ruoloProt = (SAAdminTool.DocsPaWR.Ruolo)Session["userRuolo"];
                }

                if (UserManager.getRegistroSelezionato(this) != null)
                {
                    //Registro corrente
                    smistamentoRubrica.idRegistro = UserManager.getRegistroSelezionato(this).systemId;
                }

                DocsPaWR.ElementoRubrica[] corrSearch = UserManager.getElementiRubrica(this.Page, qco, smistamentoRubrica);
                List<InfoFascicolo> fascExtraAoo = new List<InfoFascicolo>();

                if(CallContextStack.CurrentContext == null) CallContextStack.CurrentContext = new CallContext("Rubrica");
                fascExtraAoo = (List<InfoFascicolo>)CallContextStack.CurrentContext.ContextState["InfoProjectListExtraAOO"];
                // se è stato trovato un corrispondente, verifico se quest'ultimo ha visibilità solo sui fascicoli contenuti in nodi extra Aoo(Inps)
                DocsPaWebService ws = new DocsPaWebService();
                if (ws.isFiltroAooEnabled() && corrSearch.Length > 0 && fascExtraAoo != null)
                {
                    Registro reg = null;
                    if (corrSearch[0].tipo.Equals("P"))// se il corrispondente è di tipo utente
                    {
                        Ruolo[] ruolo = UserManager.GetRuoliUtenteByIdCorr(this.Page, corrSearch[0].systemId);
                        Registro[] registro = UserManager.GetRegistriByRuolo(this.Page, ruolo[0].systemId);
                        reg = registro[0];
                    }
                    if (corrSearch[0].tipo.Equals("R"))//se il corrispondente è un ruolo
                    {
                        Registro[] registro = UserManager.GetRegistriByRuolo(this.Page, corrSearch[0].systemId);
                        reg = registro[0];
                    }
                    if (corrSearch[0].tipo.Equals("U"))// se il corrispondente è una UO
                    {
                        string[] codiceUO = UserManager.GetUoInterneAoo(this.Page);
                        if (codiceUO.Contains(corrSearch[0].codice))
                            reg = UserManager.getRegistroSelezionato(this.Page);
                        else
                        {
                            reg = new Registro();
                            reg.systemId = "0";
                        }
                    }
                    //se il corrispondente non ha visibilità sul registro del mittente, e non sono stati selezionati dei fascicoli su nodi extra Aoo dalla finestra di trasm. massiva
                    //allora devo filtrare il corrispondente
                    if (reg != null && (!reg.systemId.Equals(qco.caller.IdRegistro)))
                    {
                        if (fascExtraAoo.Count < 1)
                            corrSearch = null;
                    }
                }

                if (corrSearch != null && corrSearch.Length != 0)
                    Session["current_search_resultset"] = corrSearch;
                else
                    RegisterStartupScript("noresults", "<script language=\"javascript\">var ddlIE = document.getElementById('ddlIE'); if(ddlIE!=null && (ddlIE.selectedIndex!=1 && ddlIE.selectedIndex!=2)) {var ddl = document.getElementById('ddl_rf'); if(ddl!=null){ddl.style.visibility = 'hidden'; var label = document.getElementById('lbl_registro'); if(label!=null){label.style.visibility = 'hidden';}}}; alert(\"I parametri di ricerca impostati non hanno prodotto alcun risultato\");</script>");

                dgCorr.DataSource = corrSearch;
                dgCorr.SelectedPage = 1;
                dgCorr.DataBind();

                tsMode.SelectedIndex = M_ELENCO;

                pnlElenco.Visible = true;
                pnlOrganigramma.Visible = false;

                if (cfg_simple)
                {
                    string qco_ser = my_serialize_qco(qco);
                    dgCorr.AddHierarchyElement(corrSearch, HIERARCHY_TAG_RICERCA,
                        "Ricerca", qco_ser, false);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void SearchOnlyDeleteModifyCorr(string systemId)
        {
            try
            {
                Session.Remove("current_search_resultset");
                dgCorr.SelectedPage = 1;
                dgCorr.ClearHierarchy();
                dgCorr.Columns.Clear();
                tvOrganigramma.Nodes.Clear();
                cbSelAll.Checked = false;

                #region commento 28 marzo 2008
                //DocsPaWR.ParametriRicercaRubrica qco = new SAAdminTool.DocsPaWR.ParametriRicercaRubrica();
                //qco.calltype = (SAAdminTool.DocsPaWR.RubricaCallType)Convert.ToInt32(calltype);

                //qco.tipoIE = AddressbookTipoUtente.ESTERNO;

                //UserManager.setQueryRubricaCaller(ref qco);

                //qco.tipoIE = select_tipoIE();
                //qco.systemId = systemId;
                //qco.calltype = (SAAdminTool.DocsPaWR.RubricaCallType)Convert.ToInt32(calltype);
                //qco.ObjectType = objtype;

                //select_item_types(ref qco);

                //modifica Federica 16 aprile 2007
                //la ricerca automatica di un corrispondente appena creato non avviene
                //se non è spuntata l'opzione relativa nella pagina della rubrica
                //qco.doRuoli = true;
                //qco.doUo = true;
                //qco.doUtenti = true;
                ////fine modifica Federica

                //qco.doListe = false;

                // DocsPaWR.ElementoRubrica[] corrSearch = UserManager.getElementiRubrica(this.Page, qco);
                #endregion

                DocsPaWR.ElementoRubrica[] corrSearch = new ElementoRubrica[1];

                //new: ricerco solo il corrispondente appena creato o quello che è selezionato per la modifica
                corrSearch[0] = UserManager.getElementoRubricaSimpleBySystemId(this, systemId);

                if (corrSearch != null && corrSearch.Length != 0)
                    Session["current_search_resultset"] = corrSearch;
                else
                    RegisterStartupScript("noresults", "<script language=\"javascript\">alert(\"I parametri di ricerca impostati non hanno prodotto alcun risultato\");</script>");

                dgCorr.DataSource = corrSearch;
                dgCorr.SelectedPage = 1;
                dgCorr.DataBind();

                tsMode.SelectedIndex = M_ELENCO;

                pnlElenco.Visible = true;
                pnlOrganigramma.Visible = false;

                //if (cfg_simple)
                //{
                //    string qco_ser = my_serialize_qco(qco);
                //    dgCorr.AddHierarchyElement(corrSearch, HIERARCHY_TAG_RICERCA,
                //        "Ricerca", qco_ser, false);
                //}

                this.SelectFirstCorrispondente(corrSearch, true, true);


            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void addNewCorrToA(string systemId)
        {
            try
            {
                //Session.Remove("current_search_resultset");

                Hashtable tmp = new Hashtable();
                // Gli elementi scelti (nella datagrid a destra)
                foreach (ElementoRubrica eer in _A_recipients)
                    tmp.Add(eer.codice, eer);
                
                DocsPaWR.ElementoRubrica[] corrSearch = new ElementoRubrica[1];

                //new: ricerco solo il corrispondente appena creato o quello che è selezionato per la modifica
                corrSearch[0] = UserManager.getElementoRubricaSimpleBySystemId(this, systemId);

                //if (corrSearch != null && corrSearch.Length != 0)
                //    Session["current_search_resultset"] = corrSearch;
                //else
                //    RegisterStartupScript("noresults", "<script language=\"javascript\">alert(\"I parametri di ricerca impostati non hanno prodotto alcun risultato\");</script>");

                // Quelli selezionati e visibili
                if (!er_exists(corrSearch[0].codice, tmp))
                    tmp.Add(corrSearch[0].codice, corrSearch[0]);

                // Controlliamo anche la selezione "nascosta"
                DocsPaWR.ElementoRubrica[] hidden_selection = new ElementoRubrica[0];
                if (ht_checked != null && ht_checked.Count > 0)
                {
                    string s_codici = "";
                    foreach (DictionaryEntry de in ht_checked)
                    {
                        if (((bool)ht_checked[de.Key]) && (!er_exists((string)de.Key, tmp)))
                            s_codici += (de.Key + "|");
                    }

                    if (s_codici.EndsWith("|"))
                        s_codici = s_codici.Substring(0, s_codici.Length - 1);

                    if (s_codici != "")
                    {
                        string[] codici = System.Text.RegularExpressions.Regex.Split(s_codici, @"\|");
                        hidden_selection = UserManager.getElementiRubricaRange(codici, select_tipoIE(), this);
                    }
                    reset_hidden_selection();
                }

                _A_recipients = new SAAdminTool.DocsPaWR.ElementoRubrica[tmp.Count + hidden_selection.Length];
                tmp.Values.CopyTo(_A_recipients, 0);
                hidden_selection.CopyTo(_A_recipients, tmp.Count);

                dgA.DataSource = _A_recipients;
                dgA.SelectedPage = 1;
                dgA.DataBind();

                tsMode.SelectedIndex = M_ELENCO;
                pnlElenco.Visible = true;
                pnlOrganigramma.Visible = false;

                DocsPaWR.ElementoRubrica[] searchCorr = (SAAdminTool.DocsPaWR.ElementoRubrica[])Session["current_search_resultset"];                
                if (searchCorr != null)
                {
                    dgCorr.DataSource = searchCorr;
                    dgCorr.DataBind();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        /// <summary>
        /// Selezione del primo corrispondente in lista
        /// </summary>
        /// <param name="items">Lista dei corrispondenti</param>
        /// <param name="selectOnlyIfOneElement">se "true", seleziona il corrispondente solo se è l'unico elemento in lista</param>
        /// <param name="moveOnToList">se "true", il corrispondente selezionato viene spostato della griglia "To"</param>
        private void SelectFirstCorrispondente(SAAdminTool.DocsPaWR.ElementoRubrica[] items, bool selectOnlyIfOneElement, bool moveOnToList)
        {
            bool mustSelect = (items != null);

            if (mustSelect)
                mustSelect = (mustSelect && selectOnlyIfOneElement && items.Length == 1);

            if (mustSelect)
            {
                //				HtmlInputRadioButton radio=dgCorr.Items[0].Cells[0].Controls[0] as HtmlInputRadioButton;
                //
                //				if (radio!=null)
                //				{
                //					radio.Checked=true;

                this._A_recipients = items;
                //				}
            }
        }

        private void btnCerca_Click(object sender, ImageClickEventArgs e)
        {
            this.Search();
        }

        private string my_serialize_qco(ParametriRicercaRubrica qco)
        {
            return String.Format(@"@[{0}#{1}#{2}#{3}#{4}#{5}#{6}#{7}#{8}#{9}#{10}]@",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(Convert.ToString(qco.parent))),
                Convert.ToBase64String(Encoding.ASCII.GetBytes(qco.codice)),
                Convert.ToBase64String(Encoding.ASCII.GetBytes(qco.descrizione)),
                Convert.ToBase64String(Encoding.ASCII.GetBytes(qco.citta)),
                qco.doUo.ToString(),
                qco.doListe.ToString(),
                qco.doRuoli.ToString(),
                qco.doUtenti.ToString(),
                "",
                qco.tipoIE.ToString("g"),
                qco.doRubricaComune.ToString());
        }

        private ParametriRicercaRubrica my_deserialize_qco(string s_qco)
        {
            ParametriRicercaRubrica qco = new ParametriRicercaRubrica();
            string[] flds = s_qco.Split(new char[] { '#' });
            if (flds[0] != null && flds[0].Length > 0)
                qco.parent = Encoding.ASCII.GetString(Convert.FromBase64String(flds[0]));

            if (flds[1] != null && flds[1].Length > 0)
                qco.codice = Encoding.ASCII.GetString(Convert.FromBase64String(flds[1]));

            if (flds[2] != null && flds[2].Length > 0)
                qco.descrizione = Encoding.ASCII.GetString(Convert.FromBase64String(flds[2]));

            if (flds[3] != null && flds[3].Length > 0)
                qco.citta = Encoding.ASCII.GetString(Convert.FromBase64String(flds[3]));

            qco.doUo = Convert.ToBoolean(flds[4]);
            qco.doListe = Convert.ToBoolean(flds[5]);
            qco.doRuoli = Convert.ToBoolean(flds[6]);
            qco.doUtenti = Convert.ToBoolean(flds[7]);

            if (flds[9] == AddressbookTipoUtente.INTERNO.ToString("g"))
                qco.tipoIE = AddressbookTipoUtente.INTERNO;
            else
                if (flds[9] == AddressbookTipoUtente.ESTERNO.ToString("g"))
                    qco.tipoIE = AddressbookTipoUtente.ESTERNO;
                else
                    qco.tipoIE = AddressbookTipoUtente.GLOBALE;

            qco.doRubricaComune = Convert.ToBoolean(flds[10]);

            return qco;
        }

        private void openList(string codice)
        {
            tvOrganigramma.Nodes.Add(new Microsoft.Web.UI.WebControls.TreeNode());
            tsMode.SelectedIndex = M_ORGANIGRAMMA;
            set_mode(M_ORGANIGRAMMA);
            tvOrganigramma.Nodes.Clear();

            Microsoft.Web.UI.WebControls.TreeNodeCollection cur_nodes = tvOrganigramma.Nodes;
            Microsoft.Web.UI.WebControls.TreeNode nd = new Microsoft.Web.UI.WebControls.TreeNode();
            //nd.Text = UserManager.getNomeLista(this,codice.Substring(1));
            nd.Text = " " + UserManager.getNomeLista(this, Server.UrlDecode(codice), UserManager.getInfoUtente().idAmministrazione);
            nd.NodeData = Server.UrlDecode(codice);
            nd.Expanded = true;
            nd.CheckBox = (tvOrganigramma.SelectorType != TreeViewSelectorType.None);
            nd.ImageUrl = String.Format("../../images/rubrica/l_exp_o.gif");

            //ArrayList corr = UserManager.getCorrispondentiByCodLista(this,codice.Substring(1));
            ArrayList corr = UserManager.getCorrispondentiByCodLista(this, Server.UrlDecode(codice), UserManager.getInfoUtente().idAmministrazione);
            for (int i = 0; i < corr.Count; i++)
            {
                DocsPaWR.Corrispondente c = (SAAdminTool.DocsPaWR.Corrispondente)corr[i];
                DocsPaWR.ElementoRubrica er = UserManager.getElementoRubrica(this, c.codiceRubrica);
                Microsoft.Web.UI.WebControls.TreeNode temp = new Microsoft.Web.UI.WebControls.TreeNode();
                temp.Text = er.descrizione;
                temp.NodeData = er.codice;
                temp.Expanded = false;
                if (er.tipo == "U")
                    temp.ImageUrl = String.Format("../../images/rubrica/u_noexp.gif");
                if (er.tipo == "R")
                    temp.ImageUrl = String.Format("../../images/rubrica/r_noexp.gif");
                if (er.tipo == "P")
                    temp.ImageUrl = String.Format("../../images/rubrica/p_noexp.gif");

                nd.Nodes.Add(temp);
            }
            cur_nodes.Add(nd);
        }

        private void dgCorr_OpenSubItems(object sender, OpenSubItemsArgs e)
        {
            //if(e.Codice.StartsWith("@"))
            if (e.Tipo == "L")
            {
                openList(e.Codice);
                return;
            }

            tvOrganigramma.ShowPlus = false;
            //TreeNode nd = new TreeNode();
            NodoRubrica nd = new NodoRubrica();
            ArrayList ElementiFiltrati = new ArrayList();

            //popolamento oggetto per RUBRICA_USA_PROTO_SMISTAMENTO
            DocsPaWR.SmistamentoRubrica smistamentoRubrica = new SmistamentoRubrica();
            //smistamentoRubrica.smistamento: indica se è abilitata o meno lo smistamento
            smistamentoRubrica.smistamento = SAAdminTool.Utils.getAbilitazioneSmistamento();

            //callType
            smistamentoRubrica.calltype = (RubricaCallType)Convert.ToInt32(calltype);

            //InfoUtente
            DocsPaWR.InfoUtente infoUt = new SAAdminTool.DocsPaWR.InfoUtente();
            infoUt = UserManager.getInfoUtente(this);
            smistamentoRubrica.infoUt = infoUt;

            //Ruolo Protocollatore
            if (Session["userRuolo"] != null)
            {
                smistamentoRubrica.ruoloProt = (SAAdminTool.DocsPaWR.Ruolo)Session["userRuolo"];
            }

            if (UserManager.getRegistroSelezionato(this) != null)
            {
                //Registro corrente
                smistamentoRubrica.idRegistro = UserManager.getRegistroSelezionato(this).systemId;
            }

            smistamentoRubrica.daFiltrareSmistamento = "0";

            if (ddlIE.SelectedValue.Equals("IE"))
            {
                if ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_OUT
                    ||((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO))
                {
                    smistamentoRubrica.daFiltrareSmistamento = "1";
                    smistamentoRubrica.calltype = SAAdminTool.DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO;
                }
                else
                {
                    if (((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_OUT_MITT)
                      || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_INT_MITT) 
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI))
                    {
                        smistamentoRubrica.calltype = SAAdminTool.DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO; //callType: CALLTYPE_ORGANIGRAMMA_INTERNO 
                    }
                    else
                    {
                        smistamentoRubrica.calltype = SAAdminTool.DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE; //callType: CALLTYPE_ORGANIGRAMMA_TOTALE
                    }
                    //				else
                    //				{
                    //					smistamentoRubrica.calltype = SAAdminTool.DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO;
                    //				}
                }
            }
            else
            {
                if (((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_INT_DEST)
                    || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_OUT)
                   || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO))
                {
                    smistamentoRubrica.calltype = SAAdminTool.DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA;
                    smistamentoRubrica.daFiltrareSmistamento = "1";

                }
                else
                {
                    if (((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_NEWFASC_LOCFISICA)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_NEWFASC_UFFREF)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_FILTRIRICFASC_UFFREF)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_FILTRIRICFASC_LOCFIS)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_EDITFASC_UFFREF)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_GESTFASC_LOCFISICA)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_GESTFASC_UFFREF)
                        || (((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_TRASM_ALL
                        || (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_TRASM_INF
                        || (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_TRASM_SUP
                        || (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_TRASM_PARILIVELLO)
                        && UserManager.getRegistroSelezionato(this) == null)
                        )
                    {
                        smistamentoRubrica.calltype = SAAdminTool.DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE;
                    }
                    else
                    {
                        smistamentoRubrica.calltype = SAAdminTool.DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO;
                    }
                }
            }
            smistamentoRubrica.daFiltrareSmistamento = "0";
            //
            if (((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_INT_DEST)
                || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_OUT)
                || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO))
            {
                smistamentoRubrica.daFiltrareSmistamento = "1";
            }


            //viene richiamato con 2 soli parametri perchè la chiamata successiva verifica se l'elemento
            //rubrica sta nella dpa_uo_smistamento(certo se la chive sta a 1 sul web.config). Farlo 2 volte rallenta
            SAAdminTool.DocsPaWR.ElementoRubrica er1 = UserManager.getElementoRubrica(this, (Server.UrlDecode(e.Codice).Replace("|@ap@|", "'")));
            string id_amm = UserManager.getInfoUtente(this).idAmministrazione;
            SAAdminTool.DocsPaWR.ElementoRubrica[] ers = UserManager.getGerarchiaElemento((er1.codice),
                er1.interno ? SAAdminTool.DocsPaWR.AddressbookTipoUtente.INTERNO : SAAdminTool.DocsPaWR.AddressbookTipoUtente.ESTERNO,
                this, smistamentoRubrica);

            //Ruolo Predefinito
            DocsPaWR.Utente userHome = UserManager.getUtente(this);
           // DocsPaWR.Ruolo ruoloPred = userHome.ruoli[0]; //Questo non trova niente
            DocsPaWR.Ruolo ruoloPred = new DocsPaWR.Ruolo();
            if (Session["userRuolo"] != null)
            {
                ruoloPred = (SAAdminTool.DocsPaWR.Ruolo)Session["userRuolo"];
            }

            #region commento
            //SelectorFilter sf ;
            //se il registro è null vuol dire che nn devo filtrare i corrispondenti
            //poichè sono nel caso di trasmissione di un fascicolo creato su un nodo visibile a tutti i registri
            //Non si deve filtrare su una deeterminata AOO.
            //			DocsPaWR.RubricaCallType caller = ((RubricaCallType)Convert.ToInt32 (calltype));
            //			TipoOggetto tipo_oggetto;
            //			tipo_oggetto = (objtype.StartsWith("F:")) ? TipoOggetto.FASCICOLO : TipoOggetto.DOCUMENTO;
            //			if(tipo_oggetto == TipoOggetto.FASCICOLO)

            //			if(UserManager.getRegistroSelezionato(this)!=null)
            //			{
            //				if(ddlIE.SelectedValue.Equals("IE"))
            //				{
            //					sf = new SelectorFilter(this,DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO);
            //				}
            //				else
            //				{
            //					sf = new SelectorFilter(this,DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA);
            //				}
            //		
            //				sf.filterCorrSelectedAllowed(ref ers, ddlIE.SelectedValue);
            //			}

            #endregion

            if (ers == null || ers.Length == 0 || ers[0] == null || ers[0].codice == null)
            {
                RegisterStartupScript("bad_org", "<script language=\"javascript\">alert(\"Impossibile effettuare la navigazione dell'organigramma usando questo corrispondente come riferimento.\\nControllare la presenza di eventuali errori nella struttura dell'organigramma.\");</script>");
                rebind_current_search();
                return;
            }

            ignore_bad_ie = true;
            tvOrganigramma.Nodes.Add(new NodoRubrica());
            tsMode.SelectedIndex = M_ORGANIGRAMMA;
            set_mode(M_ORGANIGRAMMA);
            if (e.Tipo == "P")
            {
                tvOrganigramma.SelectedOrganigramma = true;
                this.ViewState["OrgSingolo"] = true;
            }
            tvOrganigramma.Nodes.Clear();

            UserManager.check_children_existence(this, ref ers);

            Microsoft.Web.UI.WebControls.TreeNodeCollection cur_nodes = tvOrganigramma.Nodes;

            Microsoft.Web.UI.WebControls.TreeNode tmp = null;

            for (int n = 0; n < ers.Length; n++)
            {
                SAAdminTool.DocsPaWR.ElementoRubrica er = ers[n];
                nd = new NodoRubrica();
                nd.ID = er.systemId;
                DocsPaWR.Corrispondente corr= UserManager.getCorrispondenteBySystemID(this, er.systemId);

                
                //Il ruolo predefinito 
                if (corr.systemId == ruoloPred.systemId)
                {
                    nd.Text =er.descrizione + " (*)";
                }
                else nd.Text = er.descrizione;

                nd.NodeData = er.codice;
                if (er.codice == e.Codice)
                    tmp = nd;
                
                nd.Expanded = true;
                nd.CheckBox = (tvOrganigramma.SelectorType != TreeViewSelectorType.None);
                nd.ImageUrl = String.Format("../../images/rubrica/{0}", er.tipo.ToLower());
                nd.ImageUrl += (er.tipo != "P" && er.has_children) ? "_exp_o.gif" : "_noexp.gif";
                cur_nodes.Add(nd);
                SAAdminTool.DocsPaWR.ElementoRubrica next_child = (n < (ers.Length - 1)) ? ers[n + 1] : null;
                //add_children (nd, er, next_child);

                //16 ottobre 2006
                nd.SelectAllowed = er.isVisibile;
                
                cur_nodes = nd.Nodes;
            }

            add_children(nd, ers[ers.Length - 1], null, smistamentoRubrica.calltype, smistamentoRubrica);

            if (tmp != null)
            {
                tvOrganigramma.SelectedNodeIndex = tmp.GetNodeIndex();
                string ctl = "document.getElementById('" + tvorg_sk.WebControl + "')";
                string theScript = String.Format("<script language=\"javascript\">try {{ {0}.scrollTop={0}.scrollHeight; ", tvOrganigramma.ClientID);

                theScript += "} catch (e) {}</script>";

                Page.RegisterStartupScript("tv", theScript);

                if (tvorg_sk != null)
                {
                    pnlOrganigramma.Controls.Remove(tvorg_sk);
                    HtmlForm frm = (HtmlForm)FindControl("frmRubrica");
                    HtmlInputHidden hctl = new HtmlInputHidden();
                    hctl.ID = tvorg_sk.UniqueID;
                    hctl.Value = "";
                    frm.Controls.Add(hctl);

                    theScript = "try { document.getElementById('" + tvorg_sk.ClientID +
                        "').value = " + ctl + ".scrollLeft+'&'+ " + ctl + ".scrollTop; } catch(e) {}";
                    frm.Attributes.Add("onClick", theScript);
                }
            }
        }

        private void add_children(Microsoft.Web.UI.WebControls.TreeNode root, SAAdminTool.DocsPaWR.ElementoRubrica er, SAAdminTool.DocsPaWR.ElementoRubrica next_child, SAAdminTool.DocsPaWR.RubricaCallType calltype, SAAdminTool.DocsPaWR.SmistamentoRubrica smistamentoRubrica)
        {
            DocsPaWR.ParametriRicercaRubrica qco = new SAAdminTool.DocsPaWR.ParametriRicercaRubrica();
            qco.calltype = calltype;
            UserManager.setQueryRubricaCaller(ref qco);
            qco.parent = er.codice;
            qco.tipoIE = er.interno ? SAAdminTool.DocsPaWR.AddressbookTipoUtente.INTERNO : SAAdminTool.DocsPaWR.AddressbookTipoUtente.ESTERNO;
            select_item_types(ref qco);

            //qco.doListe = false;
            DocsPaWR.ElementoRubrica[] children = UserManager.getElementiRubrica(this.Page, qco, smistamentoRubrica);
            rearrange_children(er, ref children);
            if (children != null && children.Length > 0)
            {

                UserManager.check_children_existence(this, ref children, qco.doUo, qco.doRuoli, qco.doUtenti);
                foreach (SAAdminTool.DocsPaWR.ElementoRubrica e in children)
                {
                    if ((next_child == null) || (e.codice != next_child.codice))
                    {
                        //TreeNode nd = new TreeNode();
                        NodoRubrica nd = new NodoRubrica();
                        nd.ID = e.systemId;
                        nd.Text = e.descrizione;
                        nd.NodeData = e.codice;
                        nd.CheckBox = (tvOrganigramma.SelectorType != TreeViewSelectorType.None);
                        nd.ImageUrl = String.Format("../../images/rubrica/{0}", e.tipo.ToLower());
                        nd.ImageUrl += (e.tipo != "P" && e.has_children) ? "_exp.gif" : "_noexp.gif";
                        nd.Expandable = (e.tipo != "P" && e.has_children) ? ExpandableValue.Always : ExpandableValue.Auto;
                        nd.NavigateUrl = "javascript:follow_node(this);";
                        nd.Expanded = false;
                        nd.Target = "_self";
                        //16 ottobre 2006
                        //						if(!ddlIE.SelectedValue.Equals("IE"))
                        //						{
                        nd.SelectAllowed = e.isVisibile;
                        //						}
                        //						else
                        //						{
                        //							nd.SelectAllowed=true;//per la combo INTERNI/ESTERNI i check sono sempre visibili
                        //						}

                        if (e.tipo != "P")
                        {
                            Microsoft.Web.UI.WebControls.TreeNode dummy = new Microsoft.Web.UI.WebControls.TreeNode();
                            dummy.Text = "Caricamento in corso...";
                            dummy.NodeData = "__DUMMY_NODE__";
                            dummy.Expanded = false;
                            nd.Nodes.Add(dummy);
                        }
                        root.Nodes.Add(nd);
                    }
                }
            }


        }

        private bool check_status(string cod)
        {
            foreach (string key in Request.Form.AllKeys)
                if (key.IndexOf("_CBX_[" + cod + "]_CBX_") >= 0)
                    return true;

            return false;
        }

        private void dgCorr_PageIndexChanged(object sender, PageIndexChangedArgs e)
        {
            DocsPaWR.ElementoRubrica[] corrSearch = (SAAdminTool.DocsPaWR.ElementoRubrica[])Session["current_search_resultset"];
            if (corrSearch == null)
                throw new NullReferenceException("Dati in sessione non validi");

            // TEST : Salviamo lo stato delle checkbox
            int istart = (dgCorr.SelectedPage - 1) * dgCorr.PageSize;
            int iend = (istart + dgCorr.PageSize) - 1;
            if (iend >= corrSearch.Length)
                iend = corrSearch.Length - 1;

            for (int i = istart; i <= iend; i++)
            {
                bool status = false;
                if (corrSearch[i].systemId != null)
                {
                    status = check_status(corrSearch[i].systemId);
                    ht_checked[corrSearch[i].systemId] = status;
                }
                else
                {
                    status = check_status(corrSearch[i].codice);
                    ht_checked[corrSearch[i].codice] = status;
                }
            }

            // ***************************************

            dgCorr.SelectedPage = e.NewPage;
            dgCorr.DataSource = corrSearch;
            dgCorr.DataBind();
        }

        private void dgCorr_HierarchyElementSelected(object sender, HierarchyElementSelectedArgs e)
        {
            CorrDataGrid.HierarchyElement he = (CorrDataGrid.HierarchyElement)e.Element;


            if (he.Codice == HIERARCHY_TAG_RICERCA)
            {
                dgCorr.DeleteHierarchyElementChildren(he.Codice);
                ParametriRicercaRubrica qco = null;

                if (he.Tipo.StartsWith("@[") && he.Tipo.EndsWith("]@"))
                    qco = my_deserialize_qco(he.Tipo.Substring(2, he.Tipo.Length - 4));
                qco.calltype = (RubricaCallType)Convert.ToInt32(calltype);
                UserManager.setQueryRubricaCaller(ref qco);

                DocsPaWR.ElementoRubrica[] corrSearch = UserManager.getElementiRubrica(this.Page, qco);
                Session["current_search_resultset"] = corrSearch;

                dgCorr.DataSource = corrSearch;
                dgCorr.DataBind();
            }
            else
            {
                dgCorr.DeleteHierarchyElement(he.Codice);
                OpenSubItemsArgs o = new OpenSubItemsArgs(he.Codice, he.Descrizione, he.Tipo, he.Interno);
                this.dgCorr_OpenSubItems_Simple(dgCorr, o);
            }
        }

        private void RubricaDocsPA_PreRender(object sender, EventArgs e)
        {
            // Esecuzione di comandi
            this.ExecPageCommand();

            HtmlForm frm = (HtmlForm)FindControl("frmRubrica");
            if (frm != null)
            {
                if (_A_recipients.Length > 0)
                {
                    HtmlInputHidden ctl = new HtmlInputHidden();
                    ctl.Name = RubricaDocsPA._A_RECIPIENTS;
                    ctl.ID = RubricaDocsPA._A_RECIPIENTS;
                    ctl.Value = Utils.ER_Array_Serialize(_A_recipients);
                    frm.Controls.Add(ctl);
                    dgA.DataSource = _A_recipients;

                }

                if (_CC_recipients.Length > 0)
                {
                    HtmlInputHidden ctl = new HtmlInputHidden();
                    ctl.Name = RubricaDocsPA._CC_RECIPIENTS;
                    ctl.ID = RubricaDocsPA._CC_RECIPIENTS;
                    ctl.Value = Utils.ER_Array_Serialize(_CC_recipients);
                    frm.Controls.Add(ctl);
                    dgCC.DataSource = _CC_recipients;

                }
            }

            dgA.DataBind();
            dgCC.DataBind();

            RubricaCallType rct = (RubricaCallType)Convert.ToInt32(calltype);

            if (rct == RubricaCallType.CALLTYPE_MANAGE)
            {
                btnNuovo.Attributes["onClick"] = "javascript:doNuovo('true');";
            }
            else
            {
                btnNuovo.Attributes["onClick"] = "javascript:doNuovo('false');";
            }
            //btnNuovo.Attributes["onClick"] = "javascript:doNuovo();";
            this.ViewState["ht_checked"] = ht_checked;

            btnCerca.Attributes["onClick"] = "javascript:doWait();";
            btnOk.Attributes["onClick"] = "javascript:doWait();";
            cbSelAll.Attributes["onClick"] = "javascript:doWait();";
            ibtnMoveToA.Attributes["onClick"] = "javascript:doWait();";
            ibtnMoveToCC.Attributes["onClick"] = "javascript:doWait();";

            bool b_ok_works = (rct == RubricaCallType.CALLTYPE_MANAGE) ||
                ((_A_recipients.Length > 0) || (_CC_recipients.Length > 0));

            btnOk.Enabled = b_ok_works;
            //if (!b_ok_works)
            //    btnOk.ImageUrl = "../../App_Themes/ImgComuni/b_ok_off.gif";
            //else
            //    btnOk.ImageUrl = "../../images/rubrica/b_ok.gif";

            if (rct != RubricaCallType.CALLTYPE_RICERCA_TRASM || rct != RubricaCallType.CALLTYPE_RICERCA_TRASM_TODOLIST || rct != RubricaCallType.CALLTYPE_RICERCA_CREATOR || rct != RubricaCallType.CALLTYPE_OWNER_AUTHOR || rct != RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO || rct != RubricaCallType.CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI || rct != RubricaCallType.CALLTYPE_TUTTI_RUOLI || rct != RubricaCallType.CALLTYPE_TUTTE_UO)
                tsMode.Attributes.Add("onClick", "javascript:doWait();");

            //abilitazione delle funzioni in base al ruolo
            UserManager.disabilitaFunzNonAutorizzate(this);

            this.SetVisibilityCheckRubricaComune();

            if(!(UserManager.ruoloIsAutorized(this, "DO_INS_CORR_RF")) && !(UserManager.ruoloIsAutorized(this, "DO_INS_CORR_REG")) && !(UserManager.ruoloIsAutorized(this, "DO_INS_CORR_TUTTI"))){
                btnNuovo.Enabled=false;
            }

            SAAdminTool.AdminTool.Manager.AmministrazioneManager am = new SAAdminTool.AdminTool.Manager.AmministrazioneManager();
            if (!am.IsEnabledRF(null))
            {
                this.cb_rf.Visible = false;
                this.cb_rf.Checked = false;
            }
        }

        /// <summary>
        /// Esecuzione di comandi da eseguire in fase di caricamento della pagina.
        /// Le specifiche sul comando da eseguire sono gestite nel campo hidden "txtLoadCommand"
        /// </summary>
        private void ExecPageCommand()
        {
            string command = this.txtLoadCommand.Value;
            this.txtLoadCommand.Value = string.Empty;

            this.ExecPageCommand(command);
        }

        /// <summary>
        /// Esecuzione di comandi da eseguire nel caricamento della pagina
        /// </summary>
        /// <param name="commandName"></param>
        private void ExecPageCommand(string command)
        {
            if (command.StartsWith("REFRESH"))
            {
                int indexOfParams = command.IndexOf("?");
                if (indexOfParams > -1)
                {
                    string par = command.Substring(indexOfParams + 1);
                    string[] args = par.Split('=');
                    if (args.Length == 2)
                    {
                        string retValueInsert = args[1];
                        string[] param = null;
                        string id = string.Empty;
                        string ritorno = string.Empty;
                        if (retValueInsert.Contains("&"))
                        {
                            param = retValueInsert.Split('&');
                            id = param[0];
                            ritorno = param[1];
                        }
                        //string id = args[1];

                        // Da fare: ricerca del solo elemento modificato o cancellato
                        if (id != null && id != "")
                        {
                            //Page.RegisterStartupScript("", "<SCRIPT>doWait();</SCRIPT>");
                            if (!string.IsNullOrEmpty(ritorno))
                                this.addNewCorrToA(id);
                            else
                                this.SearchOnlyDeleteModifyCorr(id);
                           }
                    }
                }
                else
                {
                    this.Search();
                }
            }
        }

        private void dgCorr_DetailSelected(object sender, DetailSelectedArgs e)
        {
            DocsPaWR.AddressbookQueryCorrispondente qco = new SAAdminTool.DocsPaWR.AddressbookQueryCorrispondente();
            DocsPaWR.Corrispondente corr = UserManager.getCorrispondente(this, e.Codice, true);
        }

        private void dgA_PageIndexChanged(object sender, PageIndexChangedArgs e)
        {
            dgA.SelectedPage = e.NewPage;
            rebind_current_search();
        }

        private void dgCC_PageIndexChanged(object sender, PageIndexChangedArgs e)
        {
            dgCC.SelectedPage = e.NewPage;
            rebind_current_search();
        }

        private void muovi_elementi(ref SAAdminTool.DocsPaWR.ElementoRubrica[] _rcptlist)
        {
            DocsPaWR.ElementoRubrica[] selected = null;

            selected = (tsMode.SelectedIndex == M_ELENCO) ? dgCorr.GetSelected() : get_tv_selected_elements();

            // Per alcuni tipi di chiamata ci può essere uno
            // ed un solo risultato, per cui
            // svuotiamo la lista dei corrispondenti
            // attualmente selezionati
            DocsPaWR.ElementoRubrica[] src;

            if (!dgCC.Visible && dgA.PageSize == 1)
                src = new SAAdminTool.DocsPaWR.ElementoRubrica[0];
            else
                src = _rcptlist;

            Hashtable tmp = new Hashtable();
            // Gli elementi scelti (nella datagrid a destra)
            foreach (ElementoRubrica eer in src)
            {
                if (eer.systemId != null && eer.systemId != "")
                {
                    tmp.Add(eer.systemId, eer);
                }
                else
                    tmp.Add(eer.codice, eer);
            }

            // Quelli selezionati e visibili
            foreach (ElementoRubrica eer in selected)
            {
                if (eer.systemId != null)
                {
                    if (UserManager.iscorrispondenteValid(eer.systemId))
                    {
                        if (!er_exists(eer.systemId, tmp))
                            tmp.Add(eer.systemId, eer);
                    }
                    else
                    {
                        //La condizione sui ruoli disabilitati (DTA_FINE valorizzata) non viene considerata per calltype di ricerca
                        if ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_FIND_ROLE ||
                            (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
                            (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_RICERCA_CREATOR ||
                            (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_OWNER_AUTHOR ||
                            (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_RICERCA_TRASM_TODOLIST ||
                            (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT ||
                            (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_RICERCA_DOCUMENTI ||
                            (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_RICERCA_TRASM ||
                            (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO ||
                            (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_RICERCA_ESTESA ||
                            (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO ||
                            (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_RICERCA_MITTDEST ||
                            (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI ||
                            (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_CORR_NO_FILTRI
                            )
                        {
                            if (!er_exists(eer.systemId, tmp))
                                tmp.Add(eer.systemId, eer);
                        }
                        else
                        {
                            string scr = "<script>alert(\"ATTENZIONE:selezione non consentita.\\nIl codice corrispondente selezionato risulta storicizzato.\");</script>";
                            Page.RegisterStartupScript("RubCom", scr);
                        }
                    }
                }
                else
                {
                    SAAdminTool.DocsPaWR.Corrispondente corr = new Corrispondente();
                    corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, eer.codice);
                    if (string.IsNullOrEmpty(corr.errore))
                    {
                        if (!er_exists(eer.codice, tmp))
                            tmp.Add(eer.codice, eer);
                    }
                    else {
                        string scr = "<script>alert(\"ATTENZIONE:selezione non consentita.\\nTrovato codice corrispondente duplicato nella rubrica locale.\\nPer utilizzare quello di rubrica comune è necessario eliminare quello in rubrica locale.\");</script>";
                        Page.RegisterStartupScript("RubCom", scr);
                    }
                }
            }

            // Controlliamo anche la selezione "nascosta"
            DocsPaWR.ElementoRubrica[] corrSearch = (SAAdminTool.DocsPaWR.ElementoRubrica[])Session["current_search_resultset"];
            if (corrSearch != null)
            {
                int istart = (dgCorr.SelectedPage - 1) * dgCorr.PageSize;
                int iend = (istart + dgCorr.PageSize) - 1;
                if (iend >= corrSearch.Length)
                    iend = corrSearch.Length - 1;
                ht_not_checked_current_page = new Hashtable();
                for (int i = istart; i <= iend; i++)
                {
                    bool status = false;
                    if (corrSearch[i].systemId != null)
                    {
                        status = check_status(corrSearch[i].systemId);

                        ht_not_checked_current_page[corrSearch[i].systemId] = status;
                    }
                    else
                    {
                        status = check_status(corrSearch[i].codice);
                        ht_not_checked_current_page[corrSearch[i].codice] = status;
                    }
                }
            }

            DocsPaWR.ElementoRubrica[] hidden_selection = new ElementoRubrica[0];
            if (ht_checked != null && ht_checked.Count > 0)
            {
                string s_codici = "";
                foreach (DictionaryEntry de in ht_checked)
                {
                    if (ht_not_checked_current_page[de.Key] != null)
                    {
                        if ((bool)ht_not_checked_current_page[de.Key])
                            if (((bool)ht_checked[de.Key]) && (!er_exists((string)de.Key, tmp)))
                                s_codici += (de.Key + "|");
                    }
                    else
                    if (((bool)ht_checked[de.Key]) && (!er_exists((string)de.Key, tmp)))
                        s_codici += (de.Key + "|");
                }

                if (s_codici.EndsWith("|"))
                    s_codici = s_codici.Substring(0, s_codici.Length - 1);

                if (s_codici != "")
                {
                    string[] codici = System.Text.RegularExpressions.Regex.Split(s_codici, @"\|");
                    hidden_selection = UserManager.getElementiRubricaRangeSysID(codici, this);
                    //hidden_selection = UserManager.getElementiRubricaRange(codici, select_tipoIE(), this);
                }
                reset_hidden_selection();
            }

            _rcptlist = new SAAdminTool.DocsPaWR.ElementoRubrica[tmp.Count + hidden_selection.Length];
            tmp.Values.CopyTo(_rcptlist, 0);
            hidden_selection.CopyTo(_rcptlist, tmp.Count);

            //DocsPaWR.ElementoRubrica[] corrSearch = (SAAdminTool.DocsPaWR.ElementoRubrica[])Session["current_search_resultset"];
            if (corrSearch != null)
            {
                dgCorr.DataSource = corrSearch;
                dgCorr.DataBind();
            }
        }

        private void ibtnMoveToA_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            muovi_elementi(ref _A_recipients);
        }

        private void reset_hidden_selection()
        {
            ht_checked.Clear();
        }

        private bool er_exists(string cod, Hashtable ers)
        {
            return ers.ContainsKey(cod);
        }

        private void ibtnMoveToCC_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            muovi_elementi(ref _CC_recipients);
        }

        private void tvOrganigramma_Collapse(object sender, TreeViewClickEventArgs e)
        {
            Microsoft.Web.UI.WebControls.TreeNode parent = tvOrganigramma.GetNodeFromIndex(e.Node);
            parent.ImageUrl = parent.ImageUrl.Replace("_exp_o.gif", "_exp.gif");
            //tvOrganigramma.SelectedNodeIndex = null;
            tvOrganigramma.SelectedNodeIndex = parent.GetNodeIndex();
        }

        private void tvOrganigramma_Expand(object sender, TreeViewClickEventArgs e)
        {
            Microsoft.Web.UI.WebControls.TreeNode parent = tvOrganigramma.GetNodeFromIndex(e.Node);
            tvOrganigramma.SelectedNodeIndex = parent.GetNodeIndex();
            DocsPaWR.ParametriRicercaRubrica qco = new SAAdminTool.DocsPaWR.ParametriRicercaRubrica();
            if (ddlIE.SelectedValue.Equals("IE"))
            {
                if ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_OUT
                    ||(RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                {
                    qco.calltype = SAAdminTool.DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO;
                }
                else
                {

                    if (((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_OUT_MITT)
                                                  || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_INT_MITT)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI))
                    {
                        qco.calltype = SAAdminTool.DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO; //callType: CALLTYPE_ORGANIGRAMMA_INTERNO 
                    }
                    else
                    {
                        qco.calltype = SAAdminTool.DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE; //callType: CALLTYPE_ORGANIGRAMMA_TOTALE
                    }
                }
                //				else
                //				{
                //					qco.calltype = SAAdminTool.DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO;
                //				}
            }
            else
            {
                if (((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_INT_DEST)
                    || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_OUT
                    ||((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)))
                {
                    qco.calltype = SAAdminTool.DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA;

                }
                else
                {
                    if (((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_NEWFASC_LOCFISICA)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_NEWFASC_UFFREF)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_FILTRIRICFASC_UFFREF)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_FILTRIRICFASC_LOCFIS)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_EDITFASC_UFFREF)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_GESTFASC_LOCFISICA)
                        || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_GESTFASC_UFFREF)
                        || (((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_TRASM_ALL
                        || (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_TRASM_INF
                        || (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_TRASM_SUP
                        || (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_TRASM_PARILIVELLO)
                        && UserManager.getRegistroSelezionato(this) == null)
                        )
                    {

                        qco.calltype = SAAdminTool.DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE;
                    }
                    else
                    {
                        qco.calltype = SAAdminTool.DocsPaWR.RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO;
                    }
                }
            }
            UserManager.setQueryRubricaCaller(ref qco);
            parent.ImageUrl = parent.ImageUrl.Replace("_exp.gif", "_exp_o.gif");

            if (parent.Nodes.Count > 0 && (parent.Nodes[0].NodeData != "__DUMMY_NODE__"))
                return;

            if ((parent.Nodes.Count == 1) && (parent.Nodes[0].NodeData == "__DUMMY_NODE__"))
                parent.Nodes.Clear();

            qco.parent = parent.NodeData;
            select_item_types(ref qco);

            //popolamento oggetto per RUBRICA_USA_PROTO_SMISTAMENTO
            DocsPaWR.SmistamentoRubrica smistamentoRubrica = new SmistamentoRubrica();
            //smistamentoRubrica.smistamento: indica se è abilitata o meno lo smistamento
            smistamentoRubrica.smistamento = SAAdminTool.Utils.getAbilitazioneSmistamento();

            //callType
            smistamentoRubrica.calltype = qco.calltype;

            //InfoUtente
            DocsPaWR.InfoUtente infoUt = new SAAdminTool.DocsPaWR.InfoUtente();
            infoUt = UserManager.getInfoUtente(this);
            smistamentoRubrica.infoUt = infoUt;

            //Ruolo Protocollatore
            if (Session["userRuolo"] != null)
            {
                smistamentoRubrica.ruoloProt = (SAAdminTool.DocsPaWR.Ruolo)Session["userRuolo"];
            }

            if (UserManager.getRegistroSelezionato(this) != null)
            {
                //Registro corrente
                smistamentoRubrica.idRegistro = UserManager.getRegistroSelezionato(this).systemId;
            }

            DocsPaWR.ElementoRubrica[] children = UserManager.getElementiRubrica(this.Page, qco, smistamentoRubrica);

            if (children != null && children.Length > 0)
            {
                UserManager.check_children_existence(this, ref children, qco.doUo, qco.doRuoli, qco.doUtenti);

                foreach (SAAdminTool.DocsPaWR.ElementoRubrica er in children)
                {
                    NodoRubrica c = new NodoRubrica();
                    c.ID = er.systemId;
                    c.Text = er.descrizione;
                    c.NodeData = er.codice;
                    c.Expanded = false;
                    c.CheckBox = (tvOrganigramma.SelectorType != TreeViewSelectorType.None);
                    c.Expandable = (er.tipo != "P" && er.has_children) ? ExpandableValue.CheckOnce : ExpandableValue.Auto;
                    c.ImageUrl = String.Format("../../images/rubrica/{0}", er.tipo.ToLower());
                    c.ImageUrl += (er.tipo != "P" && er.has_children) ? "_exp.gif" : "_noexp.gif";

                    c.SelectAllowed = er.isVisibile;

                    if (er.tipo != "L")
                        parent.Nodes.Add(c);
                }
            }
            tvOrganigramma.SelectedNodeIndex = parent.GetNodeIndex();
        }

        private void get_tv_checked_nodes(ref ArrayList sn, Microsoft.Web.UI.WebControls.TreeNodeCollection nodes)
        {
            string ctl_grp_name = null;
            if (tvOrganigramma.SelectorType == TreeViewSelectorType.CheckBox)
                ctl_grp_name = Request.Form["__CBX_" + tvOrganigramma.ID];
            else
                ctl_grp_name = Request.Form["_CBX__CBX_"];

            if (ctl_grp_name == null || ctl_grp_name == "")
                return;

            string[] codes = System.Text.RegularExpressions.Regex.Split(ctl_grp_name, ",");
            foreach (string cod in codes)
            {
                if (cod.StartsWith("@"))
                {
                    DocsPaWR.ElementoRubrica er = new SAAdminTool.DocsPaWR.ElementoRubrica();
                    er.tipo = "L";
                    er.codice = cod;
                    er.descrizione = UserManager.getNomeLista(this, cod.Substring(1), UserManager.getInfoUtente().idAmministrazione);
                    er.has_children = false;
                    er.interno = false;
                    er.systemId = cod;
                    er.codice = UserManager.getCodiceLista(this, cod);
                    sn.Add(er);
                }
                else
                {
                    //sn.Add(UserManager.getElementoRubrica(this, cod));
                    sn.Add(UserManager.getElementoRubricaSimpleBySystemId(this, cod));
                }
            }
        }

        private SAAdminTool.DocsPaWR.ElementoRubrica[] get_tv_selected_elements()
        {
            ArrayList sn = new ArrayList();
            get_tv_checked_nodes(ref sn, tvOrganigramma.Nodes);
            DocsPaWR.ElementoRubrica[] ers = new SAAdminTool.DocsPaWR.ElementoRubrica[sn.Count];
            if (sn.Count > 0)
                sn.CopyTo(ers, 0);

            return ers;
        }

        private void remove_recipient(ref SAAdminTool.DocsPaWR.ElementoRubrica[] rcpts, string cod)
        {
            string app = "";
            for (int n = 0; n < rcpts.Length; n++)
            {
                if (rcpts[n].systemId != "")
                {
                    app = rcpts[n].systemId;
                }
                else
                    app = rcpts[n].codice;
                if (app == cod)
                {
                    DocsPaWR.ElementoRubrica[] ers = new SAAdminTool.DocsPaWR.ElementoRubrica[rcpts.Length - 1];
                    if (ers.Length > 0)
                    {
                        int idx = 0;
                        for (int x = 0; x < rcpts.Length; x++)
                        {
                            if (x != n)
                                ers[idx++] = rcpts[x];
                        }
                    }
                    rcpts = ers;
                    break;
                }
            }
            rebind_current_search();
        }

        private void dgA_RemoveItem(object sender, RemoveItemArgs e)
        {
            remove_recipient(ref _A_recipients, (Server.UrlDecode(e.DaElim).Replace("|@ap@|", "''")));
        }

        private void dgCC_RemoveItem(object sender, RemoveItemArgs e)
        {
            remove_recipient(ref _CC_recipients, (Server.UrlDecode(e.DaElim).Replace("|@ap@|", "''")));
        }

        private void btnReset_Click(object sender, ImageClickEventArgs e)
        {
            reset_search_fields();
            dgCorr.ClearHierarchy();
            _A_recipients = new SAAdminTool.DocsPaWR.ElementoRubrica[0];
            _CC_recipients = new SAAdminTool.DocsPaWR.ElementoRubrica[0];

            tsMode.SelectedIndex = M_ELENCO;
            set_mode(M_ELENCO);
            ht_checked.Clear();
            remove_current_search();
        }

        public void resetRicerca()
        {
            reset_search_fields();
            dgCorr.ClearHierarchy();
            _A_recipients = new SAAdminTool.DocsPaWR.ElementoRubrica[0];
            _CC_recipients = new SAAdminTool.DocsPaWR.ElementoRubrica[0];

            tsMode.SelectedIndex = M_ELENCO;
            set_mode(M_ELENCO);
            ht_checked.Clear();
            remove_current_search();

            //servono per simulare l'annulla della pagian ScegliUoUtente
            Session["isLoaded_ScegliUoUtente"] = true;
            Session["retValue_ScegliUoUtente"] = false;

        }

        private void reset_search_fields()
        {
            tbxCitta.Text = "";
            tbxCodice.Text = "";
            tbxDescrizione.Text = "";
            tbxLocal.Text = "";
            txt_codice_fiscale.Text = "";
            txt_partita_iva.Text = "";
            tvOrganigramma.Nodes.Clear();
            txt_mail.Text = string.Empty;
        }

        private void btnOk_Click(object sender, ImageClickEventArgs e)
        {
            IResultHandler res = new DPA3_ResultHandler(this);
            RubricaCallType rct = (SAAdminTool.DocsPaWR.RubricaCallType)Convert.ToInt32(calltype);
            res.execute(_A_recipients, _CC_recipients, rct);
        }

        public void closeRubrica()
        {
            Session.Remove("current_search_resultset");
            RegisterStartupScript("goodbye", "<script language=\"javascript\">window.close();</script>");
        }

        private void set_mode(int mode)
        {
            pnlElenco.Visible = (mode == M_ELENCO);
            if (pnlElenco.Visible)
            {
                this.ViewState["OrgSingolo"] = false;
            }
            pnlOrganigramma.Visible = !(mode == M_ELENCO);

            if (pnlElenco.Visible && Session["current_search_resultset"] != null)
            {
                dgCorr_PageIndexChanged(dgCorr, new PageIndexChangedArgs(dgCorr.SelectedPage));
                tvOrganigramma.Nodes.Clear();
            }
        }

        private bool ignore_bad_ie = false;

        private void tsMode_SelectedIndexChange(object sender, System.EventArgs e)
        {
                
            set_mode(tsMode.SelectedIndex != M_ELENCO ? M_ORGANIGRAMMA : M_ELENCO);
            if ((ddlIE.SelectedValue == "E" || ddlIE.SelectedValue == "EA") && tsMode.SelectedIndex == 1)
            {
                RegisterStartupScript("bad_ie", "<script language=\"javascript\">var ddlIE = document.getElementById('ddlIE'); if(ddlIE!=null && ddlIE.selectedIndex!=1 && ddlIE.selectedIndex!=2) {var ddl = document.getElementById('ddl_rf'); if(ddl!=null){ddl.style.display = 'none'; var label = document.getElementById('lbl_registro'); if(label!=null){label.style.display = 'none'}}};alert(\"L'organigramma può essere visualizzato solo per corrispondenti interni!\");</script>");
                tsMode.SelectedIndex = M_ELENCO;
                resetRicerca();
                return;
            }

            if (tvOrganigramma.Nodes.Count == 0 && tsMode.SelectedIndex == 1)
            {
                ParametriRicercaRubrica qr = new ParametriRicercaRubrica();


                if (tsMode.SelectedIndex == 1)
                {
                    int val = 0;
                    if (!ddlIE.SelectedValue.Equals("IE"))
                    {
                        // per gestione tab organigramma in rubrica -- per ANAS 13 ottobre 2006
                        //qr.calltype = RubricaCallType.CALLTYPE_ORGANIGRAMMA;

                        //IMPORTANTE: ho dovuto cablare il calltype "36" perche se usavo l'enum CALLTYPE_ORGANIGRAMMA
                        //al passaggio nel metodo selectTipoIE rimaneva cablato il calltype passato in querystring all'apertura
                        //della rubrica, e la modifica per l'organigramma non funzionava

                        if (((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_INT_DEST)
                            || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_OUT)
                            || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                            || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_ORGANIGRAMMA))
                        {
                            //	calltype = "36"; //callType: CALLTYPE_ORGANIGRAMMA
                            val = (int)RubricaCallType.CALLTYPE_ORGANIGRAMMA;
                        }
                        else
                        {
                            if (((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_NEWFASC_LOCFISICA)
                                || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_NEWFASC_UFFREF)
                                || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_FILTRIRICFASC_UFFREF)
                                || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_FILTRIRICFASC_LOCFIS)
                                || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA)
                                || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_EDITFASC_UFFREF)
                                || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_GESTFASC_LOCFISICA)
                                || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_GESTFASC_UFFREF)
                                || (((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_TRASM_ALL
                                || (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_TRASM_INF
                                || (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_TRASM_SUP
                                || (RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_TRASM_PARILIVELLO)
                                && UserManager.getRegistroSelezionato(this) == null)
                                )
                            {
                                //calltype = "39"; //callType: CALLTYPE_ORGANIGRAMMA_TOTALE
                                val = (int)RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE;
                            }
                            else
                            {
                                //calltype = "40"; //callType: CALLTYPE_ORGANIGRAMMA_INTERNO
                                val = (int)RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO;
                            }
                        }

                    }
                    else
                    {
                        // per gestione tab organigramma in rubrica nel caso INTERNI/ESTERNI -- per ANAS 13 ottobre 2006

                        if (((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_INT_DEST)
                            || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_OUT)
                            || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                            || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO))
                        {
                            //calltype = "37"; //callType: CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO
                            val = (int)RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO;
                        }
                        else
                        {

                            if (((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_OUT_MITT)
                                || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO)
                                || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_INT_MITT)
                                || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO)
                                || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI))
                            {
                                //	calltype = "40"; //callType: CALLTYPE_ORGANIGRAMMA_INTERNO 
                                val = (int)RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO;
                            }
                            else
                            {
                                //calltype = "39"; 	
                                val = (int)RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE;//callType: CALLTYPE_ORGANIGRAMMA_TOTALE
                            }
                        }

                    }
                    calltype = Convert.ToString(val);
                    qr.calltype = ((RubricaCallType)Convert.ToInt32(calltype));

                    //qr.calltype = ((RubricaCallType)Convert.ToInt32 (val));
                }

                if (((ddlIE.Enabled) && ddlIE.SelectedValue == "E" && (!ignore_bad_ie)) || (ddlIE.SelectedItem.Text.Equals("Esterni Amm.ne")))
                {
                    RegisterStartupScript("bad_ie", "<script language=\"javascript\">alert(\"L'organigramma può essere visualizzato solo per corrispondenti interni!\");</script>");
                    tsMode.SelectedIndex = M_ELENCO;
                    resetRicerca();
                    return;
                }


                //popolamento oggetto per RUBRICA_USA_PROTO_SMISTAMENTO
                DocsPaWR.SmistamentoRubrica smistamentoRubrica = new SmistamentoRubrica();
                //smistamentoRubrica.smistamento: indica se è abilitata o meno lo smistamento
                smistamentoRubrica.smistamento = SAAdminTool.Utils.getAbilitazioneSmistamento();

                //callType
                smistamentoRubrica.calltype = (RubricaCallType)Convert.ToInt32(calltype);
                //smistamentoRubrica.calltype = (RubricaCallType)Convert.ToInt32 (val);

                //InfoUtente
                DocsPaWR.InfoUtente infoUt = new SAAdminTool.DocsPaWR.InfoUtente();
                infoUt = UserManager.getInfoUtente(this);
                smistamentoRubrica.infoUt = infoUt;

                //Ruolo Protocollatore
                if (Session["userRuolo"] != null)
                {
                    smistamentoRubrica.ruoloProt = (SAAdminTool.DocsPaWR.Ruolo)Session["userRuolo"];
                }

                if (UserManager.getRegistroSelezionato(this) != null)
                {
                    //Registro corrente
                    smistamentoRubrica.idRegistro = UserManager.getRegistroSelezionato(this).systemId;
                }
                smistamentoRubrica.daFiltrareSmistamento = "0";
                if (((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_INT_DEST)
                    || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_OUT)
                    || ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO))
                {
                    smistamentoRubrica.daFiltrareSmistamento = "1";
                }

                ElementoRubrica[] ers = UserManager.getRootItems(select_tipoIE(), this, smistamentoRubrica);

                select_item_types(ref qr);
                UserManager.check_children_existence(this, ref ers, qr.doUo, qr.doRuoli, qr.doUtenti);

                Microsoft.Web.UI.WebControls.TreeNodeCollection cur_nodes = tvOrganigramma.Nodes;

                for (int n = 0; n < ers.Length; n++)
                {
                    DocsPaWR.ElementoRubrica er = ers[n];

                    NodoRubrica nd = new NodoRubrica();
                    nd.ID = er.systemId;
                    nd.Text = er.descrizione;
                    nd.NodeData = er.codice;
                    nd.Expanded = true;
                    nd.CheckBox = (tvOrganigramma.SelectorType != TreeViewSelectorType.None);
                    nd.ImageUrl = String.Format("../../images/rubrica/{0}", er.tipo.ToLower());
                    nd.ImageUrl += (er.tipo != "P" && er.has_children) ? "_exp_o.gif" : "_noexp.gif";

                    nd.SelectAllowed = er.isVisibile;

                    tvOrganigramma.Nodes.Add(nd);
                    DocsPaWR.ElementoRubrica next_child = (n < (ers.Length - 1)) ? ers[n + 1] : null;
                    add_children(nd, er, next_child, qr.calltype, smistamentoRubrica);
                }

            }

        }


        public static void DefaultButton(System.Web.UI.Page Page, ref TextBox objTextControl, ref ImageButton objDefaultButton)
        {
            try
            {
                System.Text.StringBuilder sScript = new System.Text.StringBuilder();

                sScript.Append(Environment.NewLine + "<SCRIPT language=\"javascript\">" + Environment.NewLine);
                sScript.Append("function fnTrapKD(btn) {" + Environment.NewLine);
                sScript.Append(" if (document.all){" + Environment.NewLine);
                sScript.Append("   if (event.keyCode == 13)" + Environment.NewLine);
                sScript.Append("   { " + Environment.NewLine);
                sScript.Append("     event.returnValue=false;" + Environment.NewLine);
                sScript.Append("     event.cancel = true;" + Environment.NewLine);
                sScript.Append("     btn.click();" + Environment.NewLine);
                sScript.Append("   } " + Environment.NewLine);
                sScript.Append(" } " + Environment.NewLine);
                sScript.Append("}" + Environment.NewLine);
                sScript.Append("</SCRIPT>" + Environment.NewLine);

                objTextControl.Attributes.Add("onkeydown", "fnTrapKD(document.all." + objDefaultButton.ClientID + ")");

                Page.RegisterStartupScript("ForceDefaultToScript", sScript.ToString());
            }
            catch
            {
            }
        }

        private void dgCorr_OpenSubItems_Simple(object sender, OpenSubItemsArgs e)
        {
            DocsPaWR.ParametriRicercaRubrica qco = new SAAdminTool.DocsPaWR.ParametriRicercaRubrica();
            UserManager.setQueryRubricaCaller(ref qco);
            qco.parent = e.Codice;
            qco.doUo = true;
            qco.doRuoli = true;
            qco.doUtenti = true;
            qco.tipoIE = e.Interno ? AddressbookTipoUtente.INTERNO : AddressbookTipoUtente.ESTERNO;
            DocsPaWR.ElementoRubrica[] children = UserManager.getElementiRubrica(this.Page, qco);

            dgCorr.AddHierarchyElement(children, e.Codice, e.Descrizione, e.Tipo, e.Interno);
            Session["current_search_resultset"] = children;
        }

        private bool dgCorr_VerifyItemSelection(object sender, string cod)
        {
            return (ht_checked != null &&
                ht_checked[cod] != null &&
                ((bool)ht_checked[cod]));
        }

        private void ibtnSelAll_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DocsPaWR.ElementoRubrica[] corrSearch = (SAAdminTool.DocsPaWR.ElementoRubrica[])Session["current_search_resultset"];
            sel_filter = new SelectorFilter(this, (RubricaCallType)Convert.ToInt32((string)this.ViewState["calltype"]));
            if (corrSearch != null)
            {
                foreach (ElementoRubrica er in corrSearch)
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["RUBRICA_PROTO_USA_SMISTAMENTO"] != null &&
                        System.Configuration.ConfigurationManager.AppSettings["RUBRICA_PROTO_USA_SMISTAMENTO"] == "0")
                    {
                        if (!string.IsNullOrEmpty(er.systemId))
                            ht_checked[er.systemId] = true;
                        else
                        {
                            if (er.isRubricaComune)
                                ht_checked[er.codice] = true;
                        }
                    }
                    else
                    {
                        if ((!er.interno) || er.tipo != "U")
                        {
                            if ((RubricaCallType)Convert.ToInt32((string)this.ViewState["calltype"]) == RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE)
                            {
                                if (!string.IsNullOrEmpty(er.systemId))
                                    ht_checked[er.systemId] = true;
                                else
                                {
                                    if (er.isRubricaComune)
                                        ht_checked[er.codice] = true;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(er.systemId))
                                    ht_checked[er.systemId] = sel_filter.execute(er);
                                else
                                {
                                    if (er.isRubricaComune)
                                        ht_checked[er.codice] = sel_filter.execute(er);
                                }
                            }
                            continue;
                        }
                        else
                        {
                            if ((RubricaCallType)Convert.ToInt32(calltype) == RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE)
                            {
                                if (!string.IsNullOrEmpty(er.systemId))
                                    ht_checked[er.systemId] = true;
                                else
                                {
                                    if (er.isRubricaComune)
                                        ht_checked[er.codice] = true;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(er.systemId))
                                    ht_checked[er.systemId] = sel_filter.execute(er);
                                else
                                {
                                    if (er.isRubricaComune)
                                        ht_checked[er.codice] = sel_filter.execute(er);
                                }
                            }
                        }
                    }
                }
                rebind_current_search();
            }
            else
                cbSelAll.Checked = false;

        }

        private void ibtnUnselAll_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (ht_checked != null)
                ht_checked.Clear();

            rebind_current_search();
        }

        private void rebind_current_search()
        {
            DocsPaWR.ElementoRubrica[] corrSearch = (SAAdminTool.DocsPaWR.ElementoRubrica[])Session["current_search_resultset"];
            if (corrSearch == null)
                //throw new NullReferenceException("Dati in sessione non validi");
                return;

            dgCorr.DataSource = corrSearch;
            dgCorr.DataBind();
        }

        private void remove_current_search()
        {
            Session["current_search_resultset"] = null;
            dgCorr.DataSource = null;
            dgCorr.DataBind();
        }

        private void dgA_RemoveAll(object sender, EventArgs e)
        {
            _A_recipients = new ElementoRubrica[0];
            rebind_current_search();
        }

        private void dgCC_RemoveAll(object sender, EventArgs e)
        {
            _CC_recipients = new ElementoRubrica[0];
            rebind_current_search();
        }

        bool selector_filter(SelectorFilterArgs e)
        {
            ElementoRubrica er = null;

            // Recuperiamo l'elemento rubrica
            if (tsMode.SelectedIndex == M_ELENCO)
            {
                DocsPaWR.ElementoRubrica[] ers = (SAAdminTool.DocsPaWR.ElementoRubrica[])dgCorr.DataSource;
                if (ers != null)
                {
                    foreach (ElementoRubrica _er in ers)
                    {
                        if (_er.codice == e.Codice)
                        {
                            er = _er;
                            break;
                        }
                    }
                }
            }
            else
            {
                // Se siamo in modalità organigramma
                // è sicuramente interno
                er = UserManager.getElementoRubrica(this, @"I\" + e.Codice);
            }

            if (er == null)
                return false;

            return sel_filter.execute(er);
        }

        private bool dgCorr_SelectorFilter(object sender, SelectorFilterArgs e)
        {
            return selector_filter(e);
        }

        private bool tvOrganigramma_SelectorFilter(object sender, SelectorFilterArgs e)
        {
            return selector_filter(e);
        }

        private void cbSelAll_CheckedChanged(object sender, System.EventArgs e)
        {
            if (cbSelAll.Checked)
                ibtnSelAll_Click(null, null);
            else
                ibtnUnselAll_Click(null, null);
        }

        private void rearrange_children(ElementoRubrica parent, ref ElementoRubrica[] children)
        {
            if (parent.tipo != "U")
                return;

            ArrayList nc_1 = new ArrayList();
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].tipo == "R" || children[i].tipo == "U")
                {
                    nc_1.Add(children[i]);
                }
            }

            ElementoRubrica[] nc = new ElementoRubrica[nc_1.Count];
            nc_1.CopyTo(nc);

            children = nc;
        }

        private void ddlIE_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            tvOrganigramma.Nodes.Clear();
        }

        #region Gestione rubrica comune

        /// <summary>
        /// Verifica se la gestione delle rubrica comune è abilitata o meno
        /// </summary>
        protected bool GestioneRubricaComuneAbilitata
        {
            get
            {
                if (this.ViewState["RubricaComuneAbilitata"] == null)
                    this.ViewState.Add("RubricaComuneAbilitata", RubricaComune.Configurazioni.GetConfigurazioni(UserManager.getInfoUtente()).GestioneAbilitata);
                return Convert.ToBoolean(this.ViewState["RubricaComuneAbilitata"]);
            }
        }

        /// <summary>
        /// Verifica se il filtro per rubrica comune è stato impostato
        /// </summary>
        private bool DoRicercaRubricaComune
        {
            get
            {
                return this.pnlRubricaComune.Visible && 
                        this.chkRubricaComune.Checked;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetVisibilityCheckRubricaComune()
        {
            if (this.GestioneRubricaComuneAbilitata &&
               (this.ddlIE.SelectedValue == "E" || this.ddlIE.SelectedValue == "IE"))
                this.pnlRubricaComune.Style["display"] = "";
            else
                this.pnlRubricaComune.Style["display"] = "none";
        }

        #endregion

#region debbug
        private void debbug_callType(RubricaCallType calty)
        {
           switch (calty) 
           {
               case RubricaCallType.CALLTYPE_PROTO_IN:
this.labelCallType.Text = "CALLTYPE_PROTO_IN"; break;
        case RubricaCallType.CALLTYPE_PROTO_IN_INT:
this.labelCallType.Text = "CALLTYPE_PROTO_IN_INT"; break;
        case RubricaCallType.CALLTYPE_PROTO_OUT:
this.labelCallType.Text = "CALLTYPE_PROTO_OUT"; break;
        case RubricaCallType.CALLTYPE_TRASM_INF:
this.labelCallType.Text = "CALLTYPE_TRASM_INF"; break;
        case RubricaCallType.CALLTYPE_TRASM_SUP:
this.labelCallType.Text = "CALLTYPE_TRASM_SUP"; break;
        case RubricaCallType.CALLTYPE_TRASM_ALL:
this.labelCallType.Text = "CALLTYPE_TRASM_ALL"; break;
        case RubricaCallType.CALLTYPE_MANAGE:
this.labelCallType.Text = "CALLTYPE_MANAGE"; break;
        case RubricaCallType.CALLTYPE_PROTO_OUT_MITT:
this.labelCallType.Text = "CALLTYPE_PROTO_OUT_MITT"; break;
        case RubricaCallType.CALLTYPE_PROTO_INT_MITT:
this.labelCallType.Text = "CALLTYPE_PROTO_INT_MITT"; break;
        case RubricaCallType.CALLTYPE_PROTO_INGRESSO:
this.labelCallType.Text = "CALLTYPE_PROTO_INGRESSO"; break;
        case RubricaCallType.CALLTYPE_UFFREF_PROTO:
this.labelCallType.Text = "CALLTYPE_UFFREF_PROTO"; break;
        case RubricaCallType.CALLTYPE_GESTFASC_LOCFISICA:
this.labelCallType.Text = "CALLTYPE_GESTFASC_LOCFISICA"; break;
        case RubricaCallType.CALLTYPE_GESTFASC_UFFREF:
this.labelCallType.Text = "CALLTYPE_GESTFASC_UFFREF"; break;
        case RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA:
this.labelCallType.Text = "CALLTYPE_EDITFASC_LOCFISICA"; break;
        case RubricaCallType.CALLTYPE_EDITFASC_UFFREF:
this.labelCallType.Text = "CALLTYPE_EDITFASC_UFFREF"; break;
        case RubricaCallType.CALLTYPE_NEWFASC_LOCFISICA:
this.labelCallType.Text = "CALLTYPE_NEWFASC_LOCFISICA"; break;
        case RubricaCallType.CALLTYPE_NEWFASC_UFFREF:
this.labelCallType.Text = "CALLTYPE_NEWFASC_UFFREF"; break;
        case RubricaCallType.CALLTYPE_RICERCA_MITTDEST:
this.labelCallType.Text = "CALLTYPE_RICERCA_MITTDEST"; break;
        case RubricaCallType.CALLTYPE_RICERCA_UFFREF:
this.labelCallType.Text = "CALLTYPE_RICERCA_UFFREF"; break;
        case RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO:
this.labelCallType.Text = "CALLTYPE_RICERCA_MITTINTERMEDIO"; break;
        case RubricaCallType.CALLTYPE_RICERCA_ESTESA:
this.labelCallType.Text = "CALLTYPE_RICERCA_ESTESA"; break;
        case RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO:
this.labelCallType.Text = "CALLTYPE_RICERCA_COMPLETAMENTO"; break;
        case RubricaCallType.CALLTYPE_RICERCA_TRASM:
this.labelCallType.Text = "CALLTYPE_RICERCA_TRASM"; break;
        case RubricaCallType.CALLTYPE_PROTO_INT_DEST:
this.labelCallType.Text = "CALLTYPE_PROTO_INT_DEST"; break;
        case RubricaCallType.CALLTYPE_FILTRIRICFASC_UFFREF:
this.labelCallType.Text = "CALLTYPE_FILTRIRICFASC_UFFREF"; break;
        case RubricaCallType.CALLTYPE_FILTRIRICFASC_LOCFIS:
this.labelCallType.Text = "CALLTYPE_FILTRIRICFASC_LOCFIS"; break;
        case RubricaCallType.CALLTYPE_RICERCA_DOCUMENTI:
this.labelCallType.Text = "CALLTYPE_RICERCA_DOCUMENTI"; break;
        case RubricaCallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT:
this.labelCallType.Text = "CALLTYPE_RICERCA_DOCUMENTI_CORR_INT"; break;
        case RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE:
this.labelCallType.Text = "CALLTYPE_LISTE_DISTRIBUZIONE"; break;
        case RubricaCallType.CALLTYPE_TRASM_PARILIVELLO:
this.labelCallType.Text = "CALLTYPE_TRASM_PARILIVELLO"; break;
        case RubricaCallType.CALLTYPE_MITT_MODELLO_TRASM:
this.labelCallType.Text = "CALLTYPE_MITT_MODELLO_TRASM"; break;
        case RubricaCallType.CALLTYPE_MODELLI_TRASM_ALL:
this.labelCallType.Text = "CALLTYPE_MODELLI_TRASM_ALL"; break;
        case RubricaCallType.CALLTYPE_MODELLI_TRASM_INF:
this.labelCallType.Text = "CALLTYPE_MODELLI_TRASM_INF"; break;
        case RubricaCallType.CALLTYPE_MODELLI_TRASM_SUP:
this.labelCallType.Text = "CALLTYPE_MODELLI_TRASM_SUP"; break;
        case RubricaCallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO:
this.labelCallType.Text = "CALLTYPE_MODELLI_TRASM_PARILIVELLO"; break;
        case RubricaCallType.CALLTYPE_DEST_MODELLO_TRASM:
this.labelCallType.Text = "CALLTYPE_DEST_MODELLO_TRASM"; break;
        case RubricaCallType.CALLTYPE_ORGANIGRAMMA:
this.labelCallType.Text = "CALLTYPE_ORGANIGRAMMA"; break;
        case RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO:
this.labelCallType.Text = "CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO"; break;
        case RubricaCallType.CALLTYPE_STAMPA_REG_UO:
this.labelCallType.Text = "CALLTYPE_STAMPA_REG_UO"; break;
        case RubricaCallType.CALLTYPE_ORGANIGRAMMA_TOTALE:
this.labelCallType.Text = "CALLTYPE_ORGANIGRAMMA_TOTALE"; break;
        case RubricaCallType.CALLTYPE_ORGANIGRAMMA_INTERNO:
this.labelCallType.Text = "CALLTYPE_ORGANIGRAMMA_INTERNO"; break;
        case RubricaCallType.CALLTYPE_RICERCA_TRASM_TODOLIST:
this.labelCallType.Text = "CALLTYPE_RICERCA_TRASM_TODOLIST"; break;
        case RubricaCallType.CALLTYPE_RUOLO_REG_NOMAIL:
this.labelCallType.Text = "CALLTYPE_RUOLO_REG_NOMAIL"; break;
        case RubricaCallType.CALLTYPE_UTENTE_REG_NOMAIL:
this.labelCallType.Text = "CALLTYPE_UTENTE_REG_NOMAIL"; break;
        case RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO:
this.labelCallType.Text = "CALLTYPE_PROTO_USCITA_SEMPLIFICATO"; break;
        case RubricaCallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO:
this.labelCallType.Text = "CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO"; break;
        case RubricaCallType.CALLTYPE_CORR_INT:
this.labelCallType.Text = "CALLTYPE_CORR_INT"; break;
        case RubricaCallType.CALLTYPE_CORR_EST:
this.labelCallType.Text = "CALLTYPE_CORR_EST"; break;
        case RubricaCallType.CALLTYPE_CORR_INT_EST:
this.labelCallType.Text = "CALLTYPE_CORR_INT_EST"; break;
        case RubricaCallType.CALLTYPE_CORR_NO_FILTRI:
this.labelCallType.Text = "CALLTYPE_CORR_NO_FILTRI"; break;
        case RubricaCallType.CALLTYPE_RICERCA_CREATOR:
this.labelCallType.Text = "CALLTYPE_RICERCA_CREATOR"; break;
        case RubricaCallType.CALLTYPE_ESTERNI_AMM:
this.labelCallType.Text = "CALLTYPE_ESTERNI_AMM"; break;
        case RubricaCallType.CALLTYPE_PROTO_OUT_ESTERNI:
this.labelCallType.Text = "CALLTYPE_PROTO_OUT_ESTERNI"; break;
        case RubricaCallType.CALLTYPE_PROTO_IN_ESTERNI:
this.labelCallType.Text = "CALLTYPE_PROTO_IN_ESTERNI"; break;
        case RubricaCallType.CALLTYPE_RUOLO_RESP_REG:
this.labelCallType.Text = "CALLTYPE_RUOLO_RESP_REG"; break;
        case RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO:
this.labelCallType.Text = "CALLTYPE_RICERCA_TRASM_SOTTOPOSTO"; break;
        case RubricaCallType.CALLTYPE_MITT_MULTIPLI:
this.labelCallType.Text = "CALLTYPE_MITT_MULTIPLI"; break;
        case RubricaCallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO:
this.labelCallType.Text = "CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO"; break;
        case RubricaCallType.CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI:
this.labelCallType.Text = "CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI"; break;
        case RubricaCallType.CALLTYPE_TUTTI_RUOLI:
this.labelCallType.Text = "CALLTYPE_TUTTI_RUOLI"; break;
        case RubricaCallType.CALLTYPE_TUTTE_UO:
this.labelCallType.Text = "CALLTYPE_TUTTE_UO"; break;
        case RubricaCallType.CALLTYPE_CORR_INT_NO_UO:
this.labelCallType.Text = "CALLTYPE_CORR_INT_NO_UO"; break;
        case RubricaCallType.CALLTYPE_REPLACE_ROLE:
this.labelCallType.Text = "CALLTYPE_REPLACE_ROLE"; break;
        case RubricaCallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI:
this.labelCallType.Text = "CALLTYPE_DEST_FOR_SEARCH_MODELLI"; break;
        case RubricaCallType.CALLTYPE_FIND_ROLE:
this.labelCallType.Text = "CALLTYPE_FIND_ROLE"; break;
        case RubricaCallType.CALLTYPE_OWNER_AUTHOR:
this.labelCallType.Text = "CALLTYPE_OWNER_AUTHOR"; break;
        case RubricaCallType.CALLTYPE_RUOLO_RESP_REPERTORI:
this.labelCallType.Text = "CALLTYPE_RUOLO_RESP_REPERTORI"; break;
        case RubricaCallType.CALLTYPE_RICERCA_CORRISPONDENTE:
this.labelCallType.Text = "CALLTYPE_RICERCA_CORRISPONDENTE"; break;
        case RubricaCallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO:
this.labelCallType.Text = "CALLTYPE_RICERCA_CORR_NON_STORICIZZATO"; break;
           }

        }
#endregion

        class UOSmistamentoByCodiceSorter : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                string cod_x = ((UOSmistamento)x).Codice;
                string cod_y = ((UOSmistamento)y).Codice;
                return String.Compare(cod_x, cod_y);
            }

            #endregion

        }

        class UOSmistamentoByCodiceFinder : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                if (x is UOSmistamento && y is string)
                {
                    string cod_x = ((UOSmistamento)x).Codice;
                    return String.Compare(cod_x, (string)y);
                }
                else
                {
                    string cod_y = ((UOSmistamento)y).Codice;
                    return String.Compare((string)x, cod_y);
                }
            }

            #endregion

        }
    }

}

