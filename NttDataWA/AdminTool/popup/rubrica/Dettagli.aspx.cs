using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using SAAdminTool.DocsPaWR;
using SAAdminTool.SiteNavigation;
using Image = System.Web.UI.WebControls.Image;
using SAAdminTool.utils;

namespace SAAdminTool.popup.RubricaDocsPA
{
    /// <summary>
    /// Summary description for Dettagli.
    /// </summary>
    public class Dettagli : SAAdminTool.CssPage
    {
        protected System.Web.UI.HtmlControls.HtmlTable tblDettagli;
        protected System.Web.UI.WebControls.Panel pnlStandard;
        protected System.Web.UI.WebControls.Panel PanelCorrispondente;
        protected System.Web.UI.WebControls.Panel pnlRuolo;
        protected System.Web.UI.WebControls.Label lbl_nomeLista;
        protected System.Web.UI.WebControls.DataGrid dg_listCorr;
        protected System.Web.UI.WebControls.Panel PanelListaCorrispondenti;
        protected System.Web.UI.HtmlControls.HtmlTable tblUtente;
        protected System.Web.UI.WebControls.Label Label1;
        protected System.Web.UI.HtmlControls.HtmlTable tbl_container;
        protected DocsPaWebCtrlLibrary.ImageButton btn_chiudi;
        protected DocsPaWebCtrlLibrary.ImageButton btn_modifica;
        protected DocsPaWebCtrlLibrary.ImageButton btn_elimina;
        protected System.Web.UI.WebControls.TextBox txt_CodAmm;
        protected System.Web.UI.WebControls.TextBox txt_CodAOO;
        protected System.Web.UI.WebControls.TextBox txt_EmailAOO;
        protected System.Web.UI.WebControls.TextBox txt_CodRubrica;
        protected System.Web.UI.WebControls.TextBox txt_Indirizzo;
        protected System.Web.UI.WebControls.TextBox txt_CAP;
        protected System.Web.UI.WebControls.TextBox txt_Citta;
        protected System.Web.UI.WebControls.TextBox txt_local;
        protected System.Web.UI.WebControls.TextBox txt_Prov;
        protected System.Web.UI.WebControls.TextBox txt_Nazione;
        protected System.Web.UI.WebControls.TextBox txt_Tel1;
        protected System.Web.UI.WebControls.TextBox txt_Tel2;
        protected System.Web.UI.WebControls.TextBox txt_Fax;
        protected System.Web.UI.WebControls.TextBox txt_CodFis;
        protected System.Web.UI.WebControls.TextBox txt_PI;
        protected System.Web.UI.WebControls.TextBox txt_EMail;

        protected DocsPaWebCtrlLibrary.ImageButton btn_mod_corr;
        protected System.Web.UI.WebControls.Image resetRadio;
        protected System.Web.UI.WebControls.Label lbl_Ruoli;
        protected System.Web.UI.WebControls.Label lblRuoli;
        protected System.Web.UI.WebControls.Label lblReg;
        protected System.Web.UI.WebControls.TextBox txt_nome;
        protected System.Web.UI.WebControls.TextBox txt_cognome;
        protected System.Web.UI.WebControls.Panel pnl_nome_cogn;
        protected System.Web.UI.WebControls.TextBox txt_desc_utente;
        protected System.Web.UI.WebControls.Panel pnl_desc_utente;
        protected System.Web.UI.WebControls.Panel pnl_descrizione;
        protected System.Web.UI.WebControls.Panel pnl_indirizzo;
        protected System.Web.UI.WebControls.TextBox txt_descrizione;
        protected System.Web.UI.WebControls.Panel pnl_bottoniera;
        protected System.Web.UI.WebControls.Panel pnl_bottonieraEsterni;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_systemId;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_confirmDelCorr;
        protected System.Web.UI.WebControls.Panel pnl_modifica;
        protected SAAdminTool.DocsPaWR.DatiModificaCorr datiModifica = new SAAdminTool.DocsPaWR.DatiModificaCorr();
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_tipo_URP;
        protected System.Web.UI.WebControls.TextBox txt_note;
        protected System.Web.UI.WebControls.TextBox txt_DescR;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idReg;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_tipo_corr;
        protected SAAdminTool.DocsPaWR.Registro r;

        //multicasella
        protected System.Web.UI.WebControls.GridView gvCaselle;
        protected System.Web.UI.UpdatePanel updPanelMail;
        protected System.Web.UI.UpdatePanel updPanel1;
        protected System.Web.UI.UpdatePanel updPanel2;
        protected System.Web.UI.WebControls.TextBox txtCasella;
        protected System.Web.UI.WebControls.TextBox txtNote;
        protected System.Web.UI.WebControls.ImageButton imgAggiungiCasella;
        protected System.Web.UI.WebControls.RadioButton rdbPrincipale;
        protected System.Web.UI.WebControls.TextBox txtEmailCorr;
        protected System.Web.UI.WebControls.TextBox txtNoteMailCorr;
        //vedo se le liste di distribuzione sono abilitate
        protected string listeDistr = System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"];
        string cod;
        string ie;
        string t;
        string sysid;
        string newCorr;
        string paginaChiamante;

        // di default si suppone che le liste sono disabilitate
        protected int flagListe = 0;

        //messaggio in out per esito operazione cancellazione / modifica
        protected string message = "";
        //newIdCorrispondente in out
        protected string newIdCorr = "";
        protected string idDoc = "";
        protected string sameMail = "";
        protected string idAOOCOLL = "";
        protected string btn_nuovo = "";
        protected string rbl_pref = "";
        protected string avviso = "";
        protected string eventArgument = "";
        protected string email = "";
        protected string btn_nuovo_bis = "";
        protected string tipoCorr = "";
        protected System.Web.UI.WebControls.TextBox txt_CodR;
        protected System.Web.UI.WebControls.DropDownList dd_canpref;
        protected System.Web.UI.WebControls.Panel pnl_canalePref;
        protected System.Web.UI.WebControls.Label starEmail;
        protected System.Web.UI.WebControls.Label starCodAmm;
        protected System.Web.UI.WebControls.Label starCodAOO;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_canalePref;
        protected System.Web.UI.HtmlControls.HtmlTableCell pnl_chiudi;
        protected System.Web.UI.WebControls.Label lblLista;
        protected System.Web.UI.WebControls.Label lblRegistro;
        protected System.Web.UI.WebControls.Panel pnlRegistriCorr;
        protected System.Web.UI.WebControls.Panel pnlJolly;
        protected System.Web.UI.WebControls.Panel pnlRuoliUtente;
        protected SAAdminTool.DocsPaWR.Corrispondente c;
        protected System.Web.UI.WebControls.TextBox txt_luogoNascita;
        protected System.Web.UI.WebControls.TextBox txt_dataNascita;
        protected System.Web.UI.WebControls.DropDownList dd_titolo;
        protected System.Web.UI.WebControls.Panel pnl_titolo;
        protected System.Web.UI.WebControls.Panel pnl_infonascita;
        //protected bool modificato = false;
        private List<SAAdminTool.DocsPaWR.Corrispondente> list = null;
        protected string messaggioErrore;

        protected DropDownList ddl_tipoCorr;
        protected System.Web.UI.HtmlControls.HtmlGenericControl divCorr;
        private void Page_Load(object sender, System.EventArgs e)
        {
            Page.Response.Expires = -1;
            cod = Request.QueryString["cod"];
            ie = Request.QueryString["ie"];
            t = Request.QueryString["t"];
            sysid = Request.QueryString["sysid"];
            paginaChiamante = Request.QueryString["pagChiam"];
            newCorr = Request.QueryString["newCorr"];
            idDoc = Request.QueryString["idDoc"];
            sameMail = Request.QueryString["sameMail"];
            idAOOCOLL = Request.QueryString["idAOOCOLL"];
            btn_nuovo = Request.QueryString["btn_nuovo"];
            messaggioErrore = string.Empty;

            btn_nuovo_bis = Request.QueryString["btn_nuovo_bis"];
            rbl_pref = Request.Form["rbl_pref"];
            avviso = Request.QueryString["avviso"];
            if (Session["Mail"] != null)
                email = Session["Mail"].ToString();

            if (!IsPostBack)
            {
                LoadCanali(true);
                LoadTitoli();
                apriDettaglio();
                ((HiddenField)FindControl("hd_field")).Value = "";
                ((HiddenField)FindControl("hd_field")).ValueChanged += new EventHandler(hd_field_OnValueChanged);
                ((Image)FindControl("resetRadio")).Attributes.Add("onClick", "resettaRadio()");
                ((DropDownList)FindControl("ddl_tipoCorr")).Visible = true;
                ((DropDownList)FindControl("ddl_tipoCorr")).Enabled = false;
            }
            else
            {
                if (((HiddenField)FindControl("hd_field")).Value != "")
                {
                    apriDettaglio(((HiddenField)FindControl("hd_field")).Value);
                }
            }

            if (newCorr != null)
            {
                //((DropDownList) FindControl("ddl_tipoCorr")).Visible = true;
                ((Table)FindControl("tbl_checkInterop")).Visible = true;
                btn_elimina.Visible = false;

                ((Button)FindControl("btn_protocolla")).Visible = true;
                ((Button)FindControl("btn_protocolla")).Click += new EventHandler(btn_protocolla_Click);
                btn_modifica.Visible = false;
                btn_chiudi.Visible = false;
                ((Button)FindControl("btn_close")).Visible = true;
                ((ImageButton)FindControl("btn_VisDoc")).Visible = true;
                ((ImageButton)FindControl("btn_VisDoc")).Click += new System.Web.UI.ImageClickEventHandler(this.btn_VisDoc_Click);
                this.ViewState["modificato"] = true;

                if (sameMail.Equals("same_mail") && !IsPostBack)
                {
                    ((Table)FindControl("tbl_MultiMitt")).Visible = true;
                    string allRegistri = "";
                    DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
                    Registro[] reg = UserManager.getListaRegistriWithRF(this, "", "");
                    foreach (Registro registro in reg)
                    {
                        allRegistri += registro.systemId + "','";
                    }
                    if (!string.IsNullOrEmpty(allRegistri))
                        allRegistri = allRegistri.Substring(0, allRegistri.Length - 3);
                    else
                    {
                        //ovvero la UserManager.getListaRegistriWithRF(this, "1", idAOOCOLL) 
                        //non ritorna alcun RF o reg...
                        allRegistri += idAOOCOLL;

                    }
                    DataSet ds = new DataSet();
                    if (Session["Interop"] != null)
                        if (Session["Interop"].ToString().Equals("P"))
                            ds = ws.GetCorrByEmail(email, allRegistri);
                        else
                            ds = ws.GetCorrByEmailAndDescr(email, Session["OldDescrizioneMitt"].ToString(), allRegistri);
                    List<Corrispondente> list = new List<Corrispondente>();
                    if (ds.Tables[0].Rows.Count > 0)
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            Corrispondente corr = new Corrispondente();
                            corr.systemId = row["SYSTEM_ID"].ToString();
                            corr.descrizione = row["VAR_DESC_CORR"].ToString();
                            corr.codiceRubrica = row["VAR_COD_RUBRICA"].ToString();
                            corr.email = cod;
                            if (Session["Interop"] != null && Session["Interop"].ToString().Equals("P"))
                                corr.idRegistro = row["ID_REGISTRO"] != DBNull.Value ? ws.GetRegistroBySistemId(row["ID_REGISTRO"].ToString()).codRegistro : String.Empty;
                            else
                                corr.idRegistro = row["VAR_COD_REGISTRO"].ToString();
                            list.Add(corr);
                        }

                    Session.Add("ListCorrByMail", list);
                    if (ds.Tables[0].Rows.Count > 0 && ((HiddenField)FindControl("hd_field")).Value == "")
                    {
                        //resettaCampi();
                        ((Button)FindControl("btn_protocolla")).Enabled = false;
                    }
                    else apriDettaglio(((HiddenField)FindControl("hd_field")).Value);
                    //ViewState.Add("ListCorrByMail", list);
                    this.GetInitData();
                }
                if (btn_nuovo.Equals("btn_nuovo") && avviso == "2")
                {
                    ((Table)FindControl("tbl_blank")).Visible = true;
                    btn_mod_corr.ToolTip = "Nuovo";
                }

                if (avviso.Equals("1"))
                {
                    DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
                    ((Label)FindControl("lbl_Avviso")).Text =
                        @"Prima di procedere con la protocollazione è necessario modificare i dati del mittente del documento inserito in rubrica di RF/AOO. Si suggerisce di visualizzare il documento.";
                    //verifico se ci sono corrispondenti nella stessa amministrazione del mittente del protocollo, ed eventualmente avviso l'utente
                    if (c != null && c.canalePref != null && c.canalePref.descrizione.Equals("INTEROPERABILITA") && ws.ExistCorrInAmm(c))
                    {
                        ((Label)FindControl("lbl_Avviso")).Text += "<br/><br/>E' presente uno o più corrispondenti con lo stesso codice Amministrazione(" + c.codiceAmm + ") del mittente.";
                    }
                    //Emanuela: se sono nella maschera di k1 e il mezzo di spedizione è di tipo Mail, mostro il bottone Occasionale e Protocolla
                    DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoSelezionato();
                    if (!IsPostBack && UserManager.ruoloIsAutorized(this, "DO_OCC_IN_K1") && schedaDoc.mezzoSpedizione.Equals("9"))
                    {
                        ((Button)FindControl("btn_crea_occasionale_protocolla")).Enabled = true;
                        ((Button)FindControl("btn_crea_occasionale_protocolla")).Visible = true;
                        ((Button)FindControl("btn_protocolla")).Enabled = false;
                    }
                    ((Button)FindControl("btn_crea_occasionale_protocolla")).Click += new EventHandler(btn_protocolla_Click);
                }
                else if (avviso.Equals("2"))
                {
                    ((Label)FindControl("lbl_Avviso")).Text =
                        @"In rubrica sono presenti altri corrispondenti con lo stesso indirizzo di posta elettronica del mittente del documento. Vuoi scegliere uno dei corrispondenti oppure creare un nuovo corrispondente?";
                }
            }
            else
            {
                ((DropDownList)FindControl("ddl_tipoCorr")).Visible = true;
                ((DropDownList)FindControl("ddl_tipoCorr")).Enabled = false;

            }

            if (c != null)
            {
                if (c.tipoCorrispondente == "U")
                    ((DropDownList)FindControl("ddl_tipoCorr")).SelectedIndex = 0;
                else if (c.tipoCorrispondente == "P")
                    ((DropDownList)FindControl("ddl_tipoCorr")).SelectedIndex = 2;
                else if (c.tipoCorrispondente == "F")
                {
                    ((DropDownList)FindControl("ddl_tipoCorr")).Items.Add(new ListItem("RAGGRUPPAMENTO FUNZIONALE", "F"));
                    ((DropDownList)FindControl("ddl_tipoCorr")).SelectedIndex = 3;
                }
                else
                    ((DropDownList)FindControl("ddl_tipoCorr")).SelectedIndex = 1;

            }
            if (!IsPostBack)
            {
                this.resetRadio.Attributes.Add("onmouseout", "this.src='" + "../../images/ricerca/remove_search_filter.gif'");
                this.resetRadio.Attributes.Add("onmouseover", "this.src='" + "../../images/ricerca/remove_search_filter_up.gif'");
            }
        }

        private void resettaCampi()
        {
            txt_nome.Text = "";
            txt_cognome.Text = "";
            txt_Indirizzo.Text = "";
            txt_Citta.Text = "";
            txt_CAP.Text = "";
            txt_Nazione.Text = "";
            txt_Prov.Text = "";
            txt_Tel1.Text = "";
            txt_Tel2.Text = "";
            txt_Fax.Text = "";
            txt_CodFis.Text = "";
            txt_PI.Text = "";
            txt_note.Text = "";
            txt_local.Text = "";
            txt_luogoNascita.Text = "";
            txt_dataNascita.Text = "";
            if (Session["Interop"] != null)
                if (Session["Interop"].ToString().Equals("S"))
                {
                    if (Session["DescrizioneMitt"] != null)
                        txt_descrizione.Text = Session["DescrizioneMitt"].ToString();
                }
                else txt_descrizione.Text = "";
            txt_CodRubrica.Text = "";

        }

        private void apriDettaglio()
        {
            cod = Server.UrlDecode(Request.QueryString["cod"].Replace("|@ap@|", "'"));
            ie = Request.QueryString["ie"];
            t = Request.QueryString["t"];
            sysid = Request.QueryString["sysid"];

            String rc = Request["rc"];

            this.pnl_chiudi.Visible = true;
            pnlJolly.Visible = false;
            pnlRuoliUtente.Visible = true;

            // Se sono Liste o RF di rubrica locale
            if (t == "L" || (t.Equals("F") && String.IsNullOrEmpty(rc)))
            {
                PanelCorrispondente.Visible = false;
                this.divCorr.Visible = false;
                this.divCorr.Style.Value = "height:0px";
                pnl_canalePref.Visible = false;
                this.pnl_bottoniera.Visible = true;
                this.pnl_bottonieraEsterni.Visible = false;
                this.Label1.Visible = false;
                this.lblLista.Visible = true;
                ArrayList listaCorrispondenti = new ArrayList();
                if (t.Equals("F"))
                {
                    lblLista.Text = "Dettaglio RF";
                    lbl_nomeLista.Text = UserManager.getNomeRF(this, cod);
                    listaCorrispondenti = UserManager.getCorrispondentiByCodRF(this, cod);
                }
                else
                {
                    //lbl_nomeLista.Text = UserManager.getNomeLista(this, cod) + " (" + cod + ")";
                    //listaCorrispondenti = UserManager.getCorrispondentiByCodLista(this, cod);
                    lbl_nomeLista.Text = UserManager.getNomeLista(this, cod, UserManager.getInfoUtente().idAmministrazione) + " (" + cod + ")";
                    listaCorrispondenti = UserManager.getCorrispondentiByCodLista(this, cod, UserManager.getInfoUtente().idAmministrazione);

                }
                dg_listCorr.DataSource = creaDataTable(listaCorrispondenti);
                dg_listCorr.DataBind();
                PanelListaCorrispondenti.Visible = true;
            }
            else
            {
                this.Label1.Visible = true;
                this.lblLista.Visible = false;
                PanelListaCorrispondenti.Visible = false;
                DocsPaWR.AddressbookTipoUtente tipoIE;
                pnl_canalePref.Visible = false;
                lblRuoli.Visible = false;
                lbl_Ruoli.Visible = false;

                if (ie.ToUpper() == "I")
                    tipoIE = SAAdminTool.DocsPaWR.AddressbookTipoUtente.INTERNO;
                else
                {
                    if (ie.ToUpper() == "E")
                    {
                        tipoIE = SAAdminTool.DocsPaWR.AddressbookTipoUtente.ESTERNO;
                        pnl_canalePref.Visible = true;
                    }
                    else
                        tipoIE = SAAdminTool.DocsPaWR.AddressbookTipoUtente.GLOBALE;
                }

                //Il codice rubrica non è più univoco.Per la visualizzazione del dettaglio utilizziamo il SystemID
                //c = UserManager.getCorrispondenteByCodRubricaIE (this, cod.Replace("'","''"), tipoIE);
                if (sysid != null && sysid != "")
                    c = UserManager.getCorrispondenteBySystemID(this, sysid);
                else
                    c = UserManager.getCorrispondenteByCodRubricaRubricaComune(this, cod.Replace("'", "''"));
                //c = UserManager.getCorrispondenteByCodRubricaIE(this, cod.Replace("'", "''"), tipoIE);

                this.hd_idReg.Value = c.idRegistro;

                if (c.tipoIE.Equals("E"))
                {
                    //luluciani: se io non ho la funzione rendo inivisibili i campi cosi' da 
                    //poter vendere la funzionalità ai clienti senza fare vederi i tasti  

                    if (UserManager.ruoloIsAutorized(this, "GEST_RUBRICA"))
                    {
                        this.pnl_bottonieraEsterni.Visible = true;
                        this.pnl_modifica.Visible = true;

                    }
                    else
                    {
                        //this.pnl_bottonieraEsterni.Visible = false;
                        this.pnl_modifica.Visible = false;
                    }

                    if (c.canalePref != null)
                    {
                        this.dd_canpref.SelectedIndex = dd_canpref.Items.IndexOf(dd_canpref.Items.FindByValue(c.canalePref.systemId));
                        this.hd_canalePref.Value = c.canalePref.systemId;
                        setVisibilityPanelStar();
                    }



                    //string x = c.tipoCorrispondente;
                    pnlRegistriCorr.Visible = true;
                    //registro è popolato solo per i corrisp esterni
                    if (c.idRegistro == null || (c.idRegistro != null && c.idRegistro.Trim() == ""))
                        lblRegistro.Text = "TUTTI [RC]";
                    else
                    {
                        DocsPaWR.Registro regCorr = UserManager.getRegistroBySistemId(this, c.idRegistro);
                        if (regCorr != null)
                        {
                            lblRegistro.Text = regCorr.codRegistro;
                            if (regCorr.chaRF == "0")
                                lblReg.Text = "Registro";
                            else
                                lblReg.Text = "RF";
                        }
                    }
                }
                else
                {
                    this.pnl_bottonieraEsterni.Visible = false;
                    this.pnl_modifica.Visible = false;
                }
                this.hd_systemId.Value = c.systemId;
                DocsPaVO.addressbook.DettagliCorrispondente dett = null;
                dett = UserManager.getDettagliCorrispondente(this, c.systemId);
                //DocsPaVO.addressbook.DettagliCorrispondente dett = UserManager.getDettagliCorrispondente(this, c.systemId);
                if (c.inRubricaComune && !string.IsNullOrEmpty(c.errore))
                {
                    dett = new DocsPaVO.addressbook.DettagliCorrispondente();
                    bool res = DocsPaUtils.Data.TypedDataSetManager.MakeTyped(c.info, dett);
                    //dett = (DocsPaVO.addressbook.DettagliCorrispondente)c.info;
                }
                else
                {
                    dett = UserManager.getDettagliCorrispondente(this, c.systemId);
                }
                txt_CodRubrica.Text = c.codiceRubrica;
                Session.Add("codRubrica", c.codiceRubrica);

                if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE)) &&
    bool.Parse(ConfigSettings.getKey(ConfigSettings.KeysENUM.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE)) &&
     !string.IsNullOrEmpty(c.codDescAmministrizazione))
                    txt_descrizione.Text = c.codDescAmministrizazione + c.descrizione;
                else
                    txt_descrizione.Text = c.descrizione;

                this.txt_CodAmm.Text = c.codiceAmm;
                this.txt_CodAOO.Text = c.codiceAOO;
                /*if (c.tipoIE != null && c.tipoIE.Equals("E"))
                {
                    if (c.canalePref != null && (c.canalePref.descrizione.Equals("INTEROPERABILITA") || c.canalePref.typeId == InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId))
                    {
                        this.txt_CodAmm.Text = c.codiceAmm;
                        this.txt_CodAOO.Text = c.codiceAOO;
                    }
                    else
                    {
                        this.txt_CodAmm.Text = string.Empty;
                        this.txt_CodAOO.Text = string.Empty;
                    }
                }
                else
                {
                    this.txt_CodAmm.Text = c.codiceAmm;
                    this.txt_CodAOO.Text = c.codiceAOO;
                }*/
                if (ie.ToUpper() == "E")
                {
                    //this.updPanelMail.Visible = true;
                    this.txt_EmailAOO.Visible = false;
                    ViewState.Add("readOnlyMail", "1");
                    EnableInsertMail();
                    BindGridViewCaselle(c);
                }
                else
                {
                    this.txt_EmailAOO.Visible = true;
                    this.txtCasella.Visible = false;
                    (this.Page.FindControl("divNote") as HtmlGenericControl).Visible = false;
                    txtNote.Visible = false;
                    this.imgAggiungiCasella.Visible = false;
                    this.updPanelMail.Visible = false;
                }
                this.txt_EmailAOO.Text = c.email;
                //Andrea
                if (!string.IsNullOrEmpty(c.email) && c.Emails.Length == 0)
                {
                    this.txtCasella.Text = c.email;
                }
                //End Andrea
                setcampiReadOnly(c);

                this.pnl_nome_cogn.Visible = false;

                if (!(c is SAAdminTool.DocsPaWR.Ruolo))
                {
                    pnlStandard.Visible = true;
                    pnlRuolo.Visible = false;
                    pnl_indirizzo.Visible = true;
                    this.hd_tipo_URP.Value = "U";

                    if (dett != null)
                    {
                        txt_Indirizzo.Text = dett.Corrispondente[0].indirizzo;
                        txt_Citta.Text = dett.Corrispondente[0].citta;
                        txt_CAP.Text = dett.Corrispondente[0].cap;
                        txt_Nazione.Text = dett.Corrispondente[0].nazione;
                        txt_Prov.Text = dett.Corrispondente[0].provincia;
                        txt_Tel1.Text = dett.Corrispondente[0].telefono;
                        txt_Tel2.Text = dett.Corrispondente[0].telefono2;
                        txt_Fax.Text = dett.Corrispondente[0].fax;
                        txt_CodFis.Text = dett.Corrispondente[0].codiceFiscale;
                        txt_PI.Text = dett.Corrispondente[0].partitaIva;
                        txt_note.Text = dett.Corrispondente[0].note;
                        txt_local.Text = dett.Corrispondente[0].localita;
                        txt_luogoNascita.Text = dett.Corrispondente[0].luogoNascita;
                        txt_dataNascita.Text = dett.Corrispondente[0].dataNascita;
                        dd_titolo.SelectedIndex = dd_titolo.Items.IndexOf(dd_titolo.Items.FindByText(dett.Corrispondente[0].titolo));
                    }

                    if (c is SAAdminTool.DocsPaWR.Utente)
                    {

                        this.pnl_nome_cogn.Visible = true;
                        //campi visibili solo nel caso di utente esterno
                        if (c.tipoIE.Equals("E"))
                        {
                            this.pnl_titolo.Visible = true;
                            this.pnl_infonascita.Visible = true;
                        }
                        this.hd_tipo_URP.Value = "P";
                        DocsPaWR.Utente u = (SAAdminTool.DocsPaWR.Utente)c;
                        //dati utente
                        this.txt_cognome.Text = u.cognome;
                        this.txt_nome.Text = u.nome;

                        string id_amm = UserManager.getInfoUtente(this).idAmministrazione;
                        DocsPaWR.ElementoRubrica[] ers = UserManager.GetRuoliUtente(this, id_amm, u.codiceRubrica);
                        lblRuoli.Text = "";

                        if (ers.Length > 0)
                        {
                            foreach (SAAdminTool.DocsPaWR.ElementoRubrica er in ers)
                            {
                                lblRuoli.Text += (er.descrizione + "<br>");
                            }
                            lblRuoli.Visible = true;
                            lbl_Ruoli.Visible = true;
                        }

                        if (c.tipoIE.Equals("E"))
                            pnlRuoliUtente.Visible = false;

                        pnl_descrizione.Visible = false;
                        pnl_titolo.Visible = true;
                        pnl_nome_cogn.Visible = true;
                        pnl_infonascita.Visible = true;
                    }
                    else
                    {
                        // è una UO
                        if (!c.tipoIE.Equals("E"))
                            pnlJolly.Visible = true;

                        pnl_descrizione.Visible = true;
                        pnl_titolo.Visible = false;
                        pnl_nome_cogn.Visible = false;
                        pnl_infonascita.Visible = false;
                    }
                }
                else
                {
                    pnlStandard.Visible = false;
                    pnlRuolo.Visible = true;
                    pnl_descrizione.Visible = true;
                    pnl_titolo.Visible = false;
                    pnl_nome_cogn.Visible = false;
                    pnl_infonascita.Visible = false;
                    pnl_indirizzo.Visible = false;
                    DocsPaWR.Ruolo u = (SAAdminTool.DocsPaWR.Ruolo)c;

                    this.hd_tipo_URP.Value = "R";

                    txt_CodR.Text = u.codiceRubrica;
                    txt_DescR.Text = u.descrizione;
                    // è un ruolO interno
                    if (!c.tipoIE.Equals("E"))
                        pnlJolly.Visible = true;
                }
            }
        }

        private void apriDettaglio(string sysid)
        {
            cod = Server.UrlDecode(Request.QueryString["cod"].Replace("|@ap@|", "'"));
            ie = Request.QueryString["ie"];
            t = Request.QueryString["t"];
            //sysid = Request.QueryString["sysid"];

            this.pnl_chiudi.Visible = true;
            pnlJolly.Visible = false;
            pnlRuoliUtente.Visible = true;

            if (t == "L" || t.Equals("F"))
            {
                PanelCorrispondente.Visible = false;
                pnl_canalePref.Visible = false;
                this.pnl_bottoniera.Visible = true;
                this.pnl_bottonieraEsterni.Visible = false;
                this.Label1.Visible = false;
                this.lblLista.Visible = true;
                ArrayList listaCorrispondenti = new ArrayList();
                if (t.Equals("F"))
                {
                    lblLista.Text = "Dettaglio RF";
                    lbl_nomeLista.Text = UserManager.getNomeRF(this, cod);
                    listaCorrispondenti = UserManager.getCorrispondentiByCodRF(this, cod);
                }
                else
                {
                    //lbl_nomeLista.Text = UserManager.getNomeLista(this, cod) + " (" + cod + ")";
                    //listaCorrispondenti = UserManager.getCorrispondentiByCodLista(this, cod);
                    lbl_nomeLista.Text = UserManager.getNomeLista(this, cod, UserManager.getInfoUtente().idAmministrazione) + " (" + cod + ")";
                    listaCorrispondenti = UserManager.getCorrispondentiByCodLista(this, cod, UserManager.getInfoUtente().idAmministrazione);

                }
                dg_listCorr.DataSource = creaDataTable(listaCorrispondenti);
                dg_listCorr.DataBind();
                PanelListaCorrispondenti.Visible = true;
            }
            else
            {
                this.Label1.Visible = true;
                this.lblLista.Visible = false;
                PanelListaCorrispondenti.Visible = false;
                DocsPaWR.AddressbookTipoUtente tipoIE;
                pnl_canalePref.Visible = false;
                lblRuoli.Visible = false;
                lbl_Ruoli.Visible = false;

                if (ie.ToUpper() == "I")
                    tipoIE = SAAdminTool.DocsPaWR.AddressbookTipoUtente.INTERNO;
                else
                {
                    if (ie.ToUpper() == "E")
                    {
                        tipoIE = SAAdminTool.DocsPaWR.AddressbookTipoUtente.ESTERNO;
                        pnl_canalePref.Visible = true;
                    }
                    else
                        tipoIE = SAAdminTool.DocsPaWR.AddressbookTipoUtente.GLOBALE;
                }

                //Il codice rubrica non è più univoco.Per la visualizzazione del dettaglio utilizziamo il SystemID
                //c = UserManager.getCorrispondenteByCodRubricaIE (this, cod.Replace("'","''"), tipoIE);
                if (sysid != null && sysid != "")
                    c = UserManager.getCorrispondenteBySystemID(this, sysid);
                else
                    c = UserManager.getCorrispondenteByCodRubricaRubricaComune(this, cod.Replace("'", "''"));
                //c = UserManager.getCorrispondenteByCodRubricaIE(this, cod.Replace("'", "''"), tipoIE);

                this.hd_idReg.Value = c.idRegistro;

                if (c.tipoIE.Equals("E"))
                {
                    //luluciani: se io non ho la funzione rendo inivisibili i campi cosi' da 
                    //poter vendere la funzionalità ai clienti senza fare vederi i tasti  
                    if (UserManager.ruoloIsAutorized(this, "GEST_RUBRICA"))
                    {
                        this.pnl_bottonieraEsterni.Visible = true;
                        //if (eventArgument != "changeCorr")
                        this.pnl_modifica.Visible = true;
                        //else
                        //{
                        //    this.pnl_modifica.Visible = false;
                        //}

                    }
                    else
                    {
                        //this.pnl_bottonieraEsterni.Visible = false;
                        //this.pnl_modifica.Visible = false;
                        this.pnl_modifica.Visible = true;
                    }

                    if (c.canalePref != null)
                    {
                        this.dd_canpref.SelectedIndex = dd_canpref.Items.IndexOf(dd_canpref.Items.FindByValue(c.canalePref.systemId));
                        this.hd_canalePref.Value = c.canalePref.systemId;
                        setVisibilityPanelStar();
                    }



                    pnlRegistriCorr.Visible = true;
                    //registro è popolato solo per i corrisp esterni
                    if (c.idRegistro == null || (c.idRegistro != null && c.idRegistro.Trim() == ""))
                        lblRegistro.Text = @"TUTTI [RC]";
                    else
                    {
                        DocsPaWR.Registro regCorr = UserManager.getRegistroBySistemId(this, c.idRegistro);
                        if (regCorr != null)
                        {
                            lblRegistro.Text = regCorr.codRegistro;
                            if (regCorr.chaRF == "0")
                                lblReg.Text = "Registro";
                            else
                                lblReg.Text = "RF";
                        }
                    }
                }
                else
                {
                    this.pnl_bottonieraEsterni.Visible = false;
                    this.pnl_modifica.Visible = false;
                }
                this.hd_systemId.Value = c.systemId;
                DocsPaVO.addressbook.DettagliCorrispondente dett = null;
                dett = UserManager.getDettagliCorrispondente(this, c.systemId);
                //DocsPaVO.addressbook.DettagliCorrispondente dett = UserManager.getDettagliCorrispondente(this, c.systemId);
                if (c.inRubricaComune && !string.IsNullOrEmpty(c.errore))
                {
                    dett = new DocsPaVO.addressbook.DettagliCorrispondente();
                    bool res = DocsPaUtils.Data.TypedDataSetManager.MakeTyped(c.info, dett);
                    //dett = (DocsPaVO.addressbook.DettagliCorrispondente)c.info;
                }
                else
                {
                    dett = UserManager.getDettagliCorrispondente(this, c.systemId);
                }
                txt_CodRubrica.Text = c.codiceRubrica;
                if (Session["codRubrica"] != null)
                {
                    Session.Remove("codRubrica");
                    Session.Add("codRubrica", c.codiceRubrica);
                }
                txt_descrizione.Text = c.descrizione;

                if (c.tipoIE != null && c.tipoIE.Equals("E"))
                {
                    if (c.canalePref != null && c.canalePref.descrizione.Equals("INTEROPERABILITA"))
                    {
                        this.txt_CodAmm.Text = c.codiceAmm;
                        this.txt_CodAOO.Text = c.codiceAOO;
                    }
                    else
                    {
                        this.txt_CodAmm.Text = string.Empty;
                        this.txt_CodAOO.Text = string.Empty;
                    }
                }
                else
                {
                    this.txt_CodAmm.Text = c.codiceAmm;
                    this.txt_CodAOO.Text = c.codiceAOO;
                }

                #region Multi casella corrispondenti esterni
                if (c.tipoIE != null && c.tipoIE.Equals("E"))
                {
                    //Siamo in K1 - K2, quindi multi casella in readonly
                    ViewState["readOnlyMail"] = "1";
                    ViewState.Remove("gvCaselle");
                    BindGridViewCaselle(UserManager.getCorrispondenteBySystemID(this.Page, sysid));
                }
                #endregion

                this.txt_EmailAOO.Text = c.email;
                setcampiReadOnly(c);
                this.pnl_nome_cogn.Visible = false;

                if (!(c is SAAdminTool.DocsPaWR.Ruolo))
                {
                    pnlStandard.Visible = true;
                    pnlRuolo.Visible = false;
                    pnl_indirizzo.Visible = true;
                    this.hd_tipo_URP.Value = "U";

                    if (dett != null)
                    {
                        txt_Indirizzo.Text = dett.Corrispondente[0].indirizzo;
                        txt_Citta.Text = dett.Corrispondente[0].citta;
                        txt_CAP.Text = dett.Corrispondente[0].cap;
                        txt_Nazione.Text = dett.Corrispondente[0].nazione;
                        txt_Prov.Text = dett.Corrispondente[0].provincia;
                        txt_Tel1.Text = dett.Corrispondente[0].telefono;
                        txt_Tel2.Text = dett.Corrispondente[0].telefono2;
                        txt_Fax.Text = dett.Corrispondente[0].fax;
                        txt_CodFis.Text = dett.Corrispondente[0].codiceFiscale;
                        txt_PI.Text = dett.Corrispondente[0].partitaIva;
                        txt_note.Text = dett.Corrispondente[0].note;
                        txt_local.Text = dett.Corrispondente[0].localita;
                        txt_luogoNascita.Text = dett.Corrispondente[0].luogoNascita;
                        txt_dataNascita.Text = dett.Corrispondente[0].dataNascita;
                        txt_descrizione.Text = c.descrizione;
                        dd_titolo.SelectedIndex = dd_titolo.Items.IndexOf(dd_titolo.Items.FindByText(dett.Corrispondente[0].titolo));
                    }

                    if (c is SAAdminTool.DocsPaWR.Utente)
                    {

                        this.pnl_nome_cogn.Visible = true;
                        //campi visibili solo nel caso di utente esterno
                        if (c.tipoIE.Equals("E"))
                        {
                            this.pnl_titolo.Visible = true;
                            this.pnl_infonascita.Visible = true;
                        }
                        this.hd_tipo_URP.Value = "P";
                        DocsPaWR.Utente u = (SAAdminTool.DocsPaWR.Utente)c;
                        //dati utente
                        this.txt_cognome.Text = u.cognome;
                        this.txt_nome.Text = u.nome;

                        string id_amm = UserManager.getInfoUtente(this).idAmministrazione;
                        DocsPaWR.ElementoRubrica[] ers = UserManager.GetRuoliUtente(this, id_amm, u.codiceRubrica);
                        lblRuoli.Text = "";

                        if (ers.Length > 0)
                        {
                            foreach (SAAdminTool.DocsPaWR.ElementoRubrica er in ers)
                            {
                                lblRuoli.Text += (er.descrizione + "<br>");
                            }
                            lblRuoli.Visible = true;
                            lbl_Ruoli.Visible = true;
                        }

                        if (c.tipoIE.Equals("E"))
                            pnlRuoliUtente.Visible = false;

                        pnl_descrizione.Visible = false;
                        pnl_nome_cogn.Visible = true;
                        pnl_titolo.Visible = true;
                    }
                    else
                    {
                        // è una UO
                        if (!c.tipoIE.Equals("E"))
                            pnlJolly.Visible = true;

                        pnl_descrizione.Visible = true;
                        pnl_nome_cogn.Visible = false;
                        pnl_titolo.Visible = false;
                    }
                }
                else
                {
                    pnlStandard.Visible = false;
                    pnlRuolo.Visible = true;
                    pnl_descrizione.Visible = true;
                    pnl_nome_cogn.Visible = false;
                    pnl_titolo.Visible = false;
                    pnl_infonascita.Visible = false;
                    pnl_indirizzo.Visible = false;
                    DocsPaWR.Ruolo u = (SAAdminTool.DocsPaWR.Ruolo)c;

                    this.hd_tipo_URP.Value = "R";

                    txt_CodR.Text = u.codiceRubrica;
                    txt_DescR.Text = u.descrizione;
                    // è un ruolO interno
                    if (!c.tipoIE.Equals("E"))
                        pnlJolly.Visible = true;
                }
            }
        }

        private void setcampiReadOnly(SAAdminTool.DocsPaWR.Corrispondente corr)
        {
            //campi comuni a tutti i corrispondenti
            this.txt_CodAmm.ReadOnly = true;
            this.txt_CodAOO.ReadOnly = true;
            this.txt_EmailAOO.ReadOnly = true;
            this.txt_CodR.ReadOnly = true;
            this.txt_DescR.ReadOnly = true;
            this.txt_CodRubrica.ReadOnly = true;
            this.txt_descrizione.ReadOnly = true;
            this.txt_Indirizzo.ReadOnly = true;
            this.txt_CAP.ReadOnly = true;
            this.txt_Citta.ReadOnly = true;
            this.txt_local.ReadOnly = true;
            this.txt_Prov.ReadOnly = true;
            this.txt_Nazione.ReadOnly = true;
            this.txt_Tel1.ReadOnly = true;
            this.txt_Tel2.ReadOnly = true;
            this.txt_Fax.ReadOnly = true;
            this.txt_CodFis.ReadOnly = true;
            this.txt_PI.ReadOnly = true;
            this.txt_nome.ReadOnly = true;
            this.txt_cognome.ReadOnly = true;
            this.txt_note.ReadOnly = true;
            this.dd_canpref.Enabled = false;
            this.txt_luogoNascita.ReadOnly = true;
            this.txt_dataNascita.ReadOnly = true;
            this.dd_titolo.Enabled = false;

        }

        private void setcampiModify()
        {
            //campi comuni a tutti i corrispondenti
            this.txt_CodAmm.ReadOnly = false;
            this.txt_CodAOO.ReadOnly = false;

            #region multi casella
            if (string.IsNullOrEmpty(newCorr))
            {
                this.txt_EmailAOO.ReadOnly = false;
                //Allora siamo in presenza di un corrispondente esterno, quindi visualizzo il multi casella
                if (this.updPanelMail.Visible)
                {
                    //rendo il multicasella readonly
                    ViewState["readOnlyMail"] = "0";
                    EnableInsertMail();
                    BindGridViewCaselle(UserManager.getCorrispondenteBySystemID(this.Page, sysid));
                }
            }
            else
            {
                this.txt_EmailAOO.ReadOnly = true;
                //Allora siamo in presenza di un corrispondente esterno, quindi visualizzo il multi casella
                if (this.updPanelMail.Visible)
                {
                    //rendo il multicasella writable
                    ViewState["readOnlyMail"] = "1";
                    EnableInsertMail();
                    BindGridViewCaselle(UserManager.getCorrispondenteBySystemID(this.Page, sysid));
                }
            }
            #endregion

            this.txt_CodR.ReadOnly = true;
            this.txt_DescR.ReadOnly = false;

            if (string.IsNullOrEmpty(newCorr))
                this.txt_CodRubrica.ReadOnly = true;
            else
            {
                this.txt_CodRubrica.ReadOnly = false;
            }

            this.txt_descrizione.ReadOnly = false;
            this.txt_Indirizzo.ReadOnly = false;
            this.txt_CAP.ReadOnly = false;
            this.txt_Citta.ReadOnly = false;
            this.txt_local.ReadOnly = false;
            this.txt_Prov.ReadOnly = false;
            this.txt_Nazione.ReadOnly = false;
            this.txt_Tel1.ReadOnly = false;
            this.txt_Tel2.ReadOnly = false;
            this.txt_Fax.ReadOnly = false;
            this.txt_CodFis.ReadOnly = false;
            this.txt_PI.ReadOnly = false;
            this.txt_note.ReadOnly = false;
            if (string.IsNullOrEmpty(newCorr))
                this.dd_canpref.Enabled = true;
            else
            {
                this.dd_canpref.Enabled = false;
            }
            this.txt_luogoNascita.ReadOnly = false;
            this.txt_dataNascita.ReadOnly = false;
            this.dd_titolo.Enabled = true;

            if (pnl_nome_cogn.Visible)
            {
                //la desc per gli utenti è creata automaticamente da Docspa.
                //non si deve modificare
                if (string.IsNullOrEmpty(((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue))
                {
                    this.txt_descrizione.ReadOnly = true;
                    this.txt_cognome.ReadOnly = false;
                    this.txt_nome.ReadOnly = false;
                }
                else if (((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue.Equals("P"))
                {
                    this.txt_cognome.ReadOnly = false;
                    this.txt_nome.ReadOnly = false;
                }

            }

        }

        private string get_codice_amm(string id_amm)
        {
            string returnMsg = string.Empty;

            if (id_amm == null || id_amm == "")
                return "";

            DocsPaWR.Amministrazione[] amms = UserManager.getListaAmministrazioni(this, out returnMsg);
            foreach (SAAdminTool.DocsPaWR.Amministrazione amm in amms)
                if (amm.systemId == id_amm)
                    return amm.codice;

            return "";
        }

        private string get_codice_reg(string id_reg)
        {
            if (id_reg == null || id_reg == "")
                return "";

            DocsPaWR.Registro[] regs = UserManager.getListaRegistri(this);
            foreach (SAAdminTool.DocsPaWR.Registro reg in regs)
                if (reg.systemId == id_reg)
                    return reg.codice;

            return "";
        }

        private string get_email_reg(string id_reg)
        {
            if (id_reg == null || id_reg == "")
                return "";

            DocsPaWR.Registro[] regs = UserManager.getListaRegistri(this);
            foreach (SAAdminTool.DocsPaWR.Registro reg in regs)
                if (reg.systemId == id_reg)
                    return reg.email;

            return "";
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
            //string evento = Request.Params.Get("__EVENTARGUMENT");
            //if (evento == null || evento == "resettaCampi")
            this.btn_mod_corr.Click += new System.Web.UI.ImageClickEventHandler(this.btn_mod_corr_Click);
            this.txt_CodAmm.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_CodAOO.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_EmailAOO.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.dd_canpref.SelectedIndexChanged += new System.EventHandler(this.dd_canpref_SelectedIndexChanged);
            this.txt_descrizione.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_nome.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_cognome.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_Indirizzo.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_CAP.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_Citta.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_local.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_Prov.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_Nazione.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_Tel1.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_Tel2.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_Fax.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_CodFis.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_PI.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_note.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_DescR.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.btn_elimina.Click += new System.Web.UI.ImageClickEventHandler(this.btn_elimina_Click);
            this.btn_modifica.Click += new System.Web.UI.ImageClickEventHandler(this.btn_modifica_Click);
            this.btn_chiudi.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudi_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.dettagli_PreRender);
            this.txt_luogoNascita.TextChanged += new System.EventHandler(this.DataChangedHandler);
            this.txt_dataNascita.TextChanged += new System.EventHandler(this.txt_dataNascita_TextChanged);
            this.dd_titolo.SelectedIndexChanged += new System.EventHandler(this.dd_titolo_SelectedIndexChanged);
            this.txt_CodRubrica.TextChanged += new EventHandler(txt_CodRubrica_TextChanged);
            ((DropDownList)FindControl("ddl_tipoCorr")).SelectedIndexChanged += new System.EventHandler(this.ddl_tipoCorr_SelectedIndexChanged);
        }
        #endregion

        private void Button1_Click(object sender, System.EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Close", "self.close();", true);
            //Response.Write ("<script language=\"javascript\">self.close();</script>");
        }

        private DataTable creaDataTable(ArrayList listaCorrispondenti)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CODICE");
            dt.Columns.Add("DESCRIZIONE");

            for (int i = 0; i < listaCorrispondenti.Count; i++)
            {
                DocsPaWR.Corrispondente c = (SAAdminTool.DocsPaWR.Corrispondente)listaCorrispondenti[i];
                DataRow dr = dt.NewRow();
                if (c.disabledTrasm)
                {
                    dr[0] = "<span style=\"color:red\">" + c.codiceRubrica + "</span>";
                    dr[1] = "<span style=\"color:red\">" + c.descrizione + "</span>";
                }
                else
                {
                    dr[0] = c.codiceRubrica;
                    dr[1] = c.descrizione;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private void btn_chiudi_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Close", "self.close();", true);
            //Response.Write ("<script language=\"javascript\">self.close();</script>");
        }

        private bool codice_rubrica_valido(string cod)
        {
            if (cod == null || cod.Trim() == "")
                return false;

            Regex rx = new Regex(@"^[0-9A-Za-z_\ \.\-]+$");
            return rx.IsMatch(cod);
        }

        /// <summary>
        /// Verifica i campi della maschera.
        /// Modifica di Lembo 21/05/2013
        /// Ho messo in ingresso una stringa, che dovrebbe essere il testo del pulsante premuto.
        /// Nel caso di "Salva come occasionale e protocolla" deve saltare il check del Codice rubrica.
        /// 
        /// </summary>
        /// <param name="testoDelPulsante">Il testo del pulsante. A seconda dello stesso si fa il check del cod Rubrica</param>
        /// <returns></returns>
        private bool verificaSelezione(string testoDelPulsante)
        {
            //string corr_type = this.hd_tipo_URP.Value;
            string corr_type = ((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue;
            bool resultCheck = true;

            int indxMail = dd_canpref.Items.IndexOf(dd_canpref.Items.FindByText("MAIL"));

            int indxInterop = dd_canpref.Items.IndexOf(dd_canpref.Items.FindByText("INTEROPERABILITA"));

            if ((corr_type == "U" && this.txt_descrizione.Text.Trim() == "") ||
                (corr_type == "R" && this.txt_descrizione.Text.Trim() == "") ||
                (corr_type == "P" && (this.txt_cognome.Text.Trim() == "" || this.txt_nome.Text.Trim() == "")) ||
                (dd_canpref.SelectedIndex == indxInterop && (this.txt_CodAmm.Text.Equals(String.Empty) ||
                    this.txt_CodAOO.Text.Equals(String.Empty))))
            {
                messaggioErrore = "Attenzione: compilare tutti i campi obbligatori";
                resultCheck = false;
            }
            //controlli mail
            if (updPanelMail.Visible)
            {
                string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                //se attivo il multicasella
                if (gvCaselle.Rows.Count < 1 && (dd_canpref.SelectedIndex == indxMail || dd_canpref.SelectedIndex == indxInterop))
                {
                    //verifico che l'indirizzo non sia vuoto
                    if (string.IsNullOrEmpty(txtCasella.Text))
                    {
                        messaggioErrore = "Immettere una casella di posta elettronica valida!";
                        resultCheck = false;
                    }

                    //verifico il formato dell'indirizzo mail

                    if (!System.Text.RegularExpressions.Regex.Match(txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), pattern).Success)
                    {
                        messaggioErrore = "Formato casella di posta non valido!";
                        resultCheck = false;
                    }
                    if (resultCheck)
                    {
                        (ViewState["gvCaselle"] as List<SAAdminTool.DocsPaWR.MailCorrispondente>).Add(new SAAdminTool.DocsPaWR.MailCorrispondente()
                        {
                            Email = txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                            Note = txtNote.Text,
                            Principale = "1"
                        });
                        txt_EmailAOO.Text = txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    }
                }
                else
                {
                    foreach (GridViewRow row in gvCaselle.Rows)
                    {
                        //verifico che l'indirizzo non sia vuoto
                        if (string.IsNullOrEmpty((row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).Text))
                        {
                            messaggioErrore = "Immettere una casella di posta elettronica valida!";
                            resultCheck = false;
                            row.Cells[1].Focus();
                            break;
                        }
                        //verifico il formato dell'indirizzo mail
                        if (!System.Text.RegularExpressions.Regex.Match((row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())
                            , pattern).Success)
                        {
                            messaggioErrore = "Formato casella di posta non valido!";
                            resultCheck = false;
                            row.Cells[1].Focus();
                            break;
                        }
                    }
                    if (resultCheck)
                    {
                        bool princ = false;
                        //scrivo in txt_EmailAOO la casella di posta principale
                        foreach (SAAdminTool.DocsPaWR.MailCorrispondente c in (List<SAAdminTool.DocsPaWR.MailCorrispondente>)ViewState["gvCaselle"])
                        {
                            if (c.Principale.Equals("1"))
                            {
                                princ = true;
                                this.txt_EmailAOO.Text = c.Email.Trim();
                                break;
                            }
                        }
                        if (!princ) // Nel caso vengano eliminate tutte le caselle per un corrispondente con canale no interoperante/no mail, allora svuoto anche txt_EmailAOO
                            txt_EmailAOO.Text = string.Empty;
                    }

                }
            }
            else
            {
                // no multicasella(corrispondenti non esterni
                if ((dd_canpref.SelectedIndex == indxMail && this.txt_EmailAOO.Text.Equals(String.Empty))
                || (dd_canpref.SelectedIndex == indxInterop && this.txt_EmailAOO.Text.Equals(String.Empty)))
                {
                    messaggioErrore = "Immettere una casella di posta elettronica valida!";
                    resultCheck = false;
                }
            }

            if (pnlStandard.Visible)//caso utenti e Uo
            {
                // modifica Lembo 21/05/2013 - fix
                if (!testoDelPulsante.Equals("Salva come occasionale e protocolla"))
                {
                    if (!codice_rubrica_valido(this.txt_CodRubrica.Text))
                    {
                        messaggioErrore = "Attenzione, il campo CODICE RUBRICA contiene caratteri non validi";
                        if (this.txt_CodRubrica.Text.Contains("@"))
                            messaggioErrore = "Il codice rubrica non deve contenere il simbolo @";
                        resultCheck = false;
                    }
                }

                if ((this.txt_Tel1 == null || this.txt_Tel1.Text.Equals(""))
                    && !(this.txt_Tel2 == null || this.txt_Tel2.Text.Equals("")))
                {
                    messaggioErrore = "Attenzione, inserire il campo TELEFONO PRINC.";
                    resultCheck = false;
                }

                //verifica del corretto formato della data di nascita nel caso in cui non sia stata cancellata
                if (this.txt_dataNascita.Text != string.Empty && !SAAdminTool.Utils.isDate(this.txt_dataNascita.Text))
                {
                    messaggioErrore = "Attenzione, il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa";
                    resultCheck = false;
                }

                if (this.txt_CAP != null && !this.txt_CAP.Text.Equals("") && !SAAdminTool.Utils.isNumeric(this.txt_CAP.Text))
                {
                    messaggioErrore = "Attenzione, il campo CAP deve essere numerico!";
                    resultCheck = false;
                }

                if (this.txt_Prov != null && !this.txt_Prov.Text.Equals("") && !SAAdminTool.Utils.isCorrectProv(this.txt_Prov.Text))
                {
                    messaggioErrore = "Attenzione, il campo PROVINCIA contiene caratteri non validi!";
                    resultCheck = false;
                }

                if (corr_type.Equals("P"))
                {
                    if (this.txt_CodFis != null && !this.txt_CodFis.Text.Equals("") && (SAAdminTool.Utils.CheckTaxCode(this.txt_CodFis.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0))
                    {
                        messaggioErrore = "Attenzione, Codice Fiscale non corretto!";
                        resultCheck = false;
                    }
                }
                else
                    if (corr_type.Equals("U"))
                    {
                        if ((this.txt_CodFis != null && !this.txt_CodFis.Text.Trim().Equals("")) && ((this.txt_CodFis.Text.Trim().Length == 11 && SAAdminTool.Utils.CheckVatNumber(this.txt_CodFis.Text.Trim()) != 0) || (this.txt_CodFis.Text.Trim().Length == 16 && SAAdminTool.Utils.CheckTaxCode(this.txt_CodFis.Text.Trim()) != 0) || (this.txt_CodFis.Text.Trim().Length != 11 && this.txt_CodFis.Text.Trim().Length != 16)))
                        {
                            messaggioErrore = "Attenzione, Codice Fiscale non corretto!";
                            resultCheck = false;
                        }
                    }

                if (this.txt_PI != null && !this.txt_PI.Text.Equals("") && (SAAdminTool.Utils.CheckVatNumber(this.txt_PI.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0))
                {
                    messaggioErrore = "Attenzione, Partita Iva non corretta!";
                    resultCheck = false;
                }

                if (!updPanelMail.Visible) // per il multicasella non ripeto i controlli sulla mail
                {
                    if (dd_canpref.SelectedIndex == indxMail || dd_canpref.SelectedIndex == indxInterop)
                    {
                        if (string.IsNullOrEmpty(this.txt_EmailAOO.Text) || txt_EmailAOO.Text.Trim().Equals(string.Empty) || !AmmUtils.UtilsXml.IsValidEmail(this.txt_EmailAOO.Text.Trim()))
                        {
                            messaggioErrore = "Attenzione, inserire una EMAIL valida";
                            resultCheck = false;
                        }
                    }
                }
            }
            return resultCheck;
        }


        private void btn_modifica_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            if (!Convert.ToBoolean(this.ViewState["modificato"]) && !Convert.ToBoolean(this.ViewState["modificatoCanale"]) &&
                !Convert.ToBoolean(this.ViewState["modificatoCaselle"]) && string.IsNullOrEmpty(txtCasella.Text))
            {
                string msg = "Non è stata effettuata alcuna modifica";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + msg + "');", true);
                //Response.Write("<script language='javascript'>window.alert('" + msg +"');</script>");
                this.btn_modifica.Enabled = true;
                SetFocusOnPanel();
                return;
            }

            string app = hd_idReg.Value.ToString();
            if (app != "")
            {
                r = ws.GetRegistroBySistemId(app);
                if ((r.chaRF == "0") && !UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_REG"))
                {
                    string msg = "Attenzione, utente o il ruolo non abilitati a modificare i corrispondenti presenti in questo registro";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + msg + "');self.close();", true);
                    //Response.Write("<script language='javascript'>window.alert('" + msg + "');</script>");
                    //Response.Write("<script language=\"javascript\">self.close();</script>");
                    //this.btn_modifica.Enabled = true;
                    //SetFocusOnPanel();
                    return;
                }
                else if ((r.chaRF == "1") && !UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_RF"))
                {
                    string msg = "Attenzione, utente o ruolo non abilitati a modificare i corrispondenti presenti in questo registro";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + msg + "');self.close();", true);
                    //Response.Write("<script language='javascript'>window.alert('" + msg + "');</script>");
                    //Response.Write("<script language=\"javascript\">self.close();</script>");
                    //this.btn_modifica.Enabled = true;
                    //SetFocusOnPanel();
                    return;
                }
            }
            else if (!UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_TUTTI"))
            {
                string msg = "Attenzione, utente o ruolo non abilitati a modificare i corrispondenti presenti in questo registro";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + msg + "');self.close();", true);
                //Response.Write("<script language='javascript'>window.alert('" + msg + "');</script>");
                //Response.Write("<script language=\"javascript\">self.close();</script>");
                //this.btn_modifica.Enabled = true;
                //SetFocusOnPanel();
                return;
            }

            //if (checkFields())
            if (verificaSelezione("No"))
            {
                if (this.hd_tipo_URP != null && this.hd_tipo_URP.Value != "")
                {
                    switch (hd_tipo_URP.Value)
                    {
                        case "U":
                            modifyUO(ref datiModifica);
                            break;
                        case "R":
                            modifyRuolo(ref datiModifica);
                            break;
                        case "P":
                            modifyUtente(ref datiModifica);
                            break;
                    }
                    if (dd_canpref.SelectedItem.Value == "")
                    {
                        for (int i = 0; i > dd_canpref.Items.Count; i++)
                        {
                            if (dd_canpref.Items[i].Text.ToUpper().Equals("LETTERA"))
                                datiModifica.idCanalePref = dd_canpref.Items[i].Value;
                        }
                    }
                }

                //operazione andata a buon fine

                if (string.IsNullOrEmpty(newCorr))
                {
                    string idNewCorr = string.Empty;
                    if (UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "M", out idNewCorr, out message))
                    {
                        switch (message)
                        {
                            case "OK":
                                message = "Modifica del corrispondente avvenuta con successo";
                                //modifico eventualmente la lista delle caselle associate al corrispondente esterno
                                if (!string.IsNullOrEmpty(idNewCorr) && !idNewCorr.Equals("0"))
                                    InsertComboMailsCorr(idNewCorr);
                                else
                                    InsertComboMailsCorr(datiModifica.idCorrGlobali);
                                break;
                            default:
                                message =
                                    "Attenzione: si è verificato un errore nella procedura di modifica del corrispondente";
                                break;
                        }

                        if (message != null && message != "")
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + message + "');", true);
                        }

                        string updatedID = this.hd_systemId.Value;
                        this.btn_modifica.Enabled = false;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refresh", "window.returnValue='REFRESH?ID=" + updatedID + "';window.close();", true);
                    }
                    else
                    {
                        if (message != null && message != "")
                        {
                            if (message.Equals("KO"))
                                message = "Attenzione: si è verificato un errore durante la modifica del corrispondente";
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + message + "');", true);
                        }
                    }
                }
                else
                {

                    if (UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "M", out newIdCorr, out message))
                    {
                        switch (message)
                        {
                            case "OK":
                                message = "Modifica del corrispondente avvenuta con successo";
                                //modifico eventualmente la lista delle caselle associate al corrispondente esterno
                                InsertComboMailsCorr(newIdCorr);
                                break;
                            default:
                                message =
                                    "Attenzione: si è verificato un errore nella procedura di modifica del corrispondente";
                                break;
                        }

                        if (message != null && message != "")
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + message + "');", true);
                        }

                        this.btn_modifica.Enabled = false;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Return", "window.returnValue='newIdCorr=" + newIdCorr + "'; window.close();", true);
                    }
                    else
                    {
                        if (message != null && message != "")
                        {
                            if (message.Equals("KO"))
                                message = "Attenzione: si è verificato un errore durante la modifica del corrispondente";
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + message + "');", true);
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(messaggioErrore))
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + messaggioErrore + "');", true);
            }
        }

        private void btn_protocolla_Click(object sender, EventArgs e)
        {
            DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            string testoDelPulsante = ((Button)sender).Text;
            //Emanuela: gestione utente occasionale
            if (!((Button)sender).Text.Equals("Salva come occasionale e protocolla"))
            {
                if (string.IsNullOrEmpty(this.txt_CodRubrica.Text.Trim()))
                {
                    string msg = "Il Codice Rubrica deve essere valorizzato";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "NoMod", "alert('" + msg + "');", true);
                    this.btn_modifica.Enabled = true;
                    SetFocusOnPanel();
                    return;
                }
                if (((DropDownList)FindControl("ddl_tipoCorr")) != null && ((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue != null &&
                    ((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue != "P" && string.IsNullOrEmpty(this.txt_descrizione.Text.Trim()))
                {
                    string msg = "La Descrizione deve essere valorizzata";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "NoMod", "alert('" + msg + "');", true);
                    this.btn_modifica.Enabled = true;
                    SetFocusOnPanel();
                    return;
                }


                // Se si sta inserendo una persona, sono obbligatori anche nome e cognome
                if (this.ddl_tipoCorr.SelectedValue == "P" && (String.IsNullOrEmpty(this.txt_nome.Text.Trim()) || String.IsNullOrEmpty(this.txt_cognome.Text.Trim())))
                {
                    string msg = "Devono essere valorizzati nome e cognome";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "NoMod", "alert('" + msg + "');", true);
                    this.btn_modifica.Enabled = true;
                    SetFocusOnPanel();
                    return;
                }

                if (this.ddl_tipoCorr.SelectedValue == "P")
                {
                    if (this.txt_CodFis != null && !this.txt_CodFis.Text.Equals("") && (SAAdminTool.Utils.CheckTaxCode(this.txt_CodFis.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckCodiceFiscale", "alert('Attenzione, Codice Fiscale non corretto!');", true);
                        return;
                    }
                }
                else if (this.ddl_tipoCorr.SelectedValue == "U")
                {
                    if ((this.txt_CodFis != null && !this.txt_CodFis.Text.Trim().Equals("")) && ((this.txt_CodFis.Text.Trim().Length == 11 && SAAdminTool.Utils.CheckVatNumber(this.txt_CodFis.Text.Trim()) != 0) || (this.txt_CodFis.Text.Trim().Length == 16 && SAAdminTool.Utils.CheckTaxCode(this.txt_CodFis.Text.Trim()) != 0) || (this.txt_CodFis.Text.Trim().Length != 11 && this.txt_CodFis.Text.Trim().Length != 16)))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckCodiceFiscale", "alert('Attenzione, Codice Fiscale non corretto!');", true);
                        return;
                    }
                }

                if (this.txt_PI != null && !this.txt_PI.Text.Equals("") && (SAAdminTool.Utils.CheckVatNumber(this.txt_PI.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckPartitaIva", "alert('Attenzione, Partita Iva non corretta!');", true);
                    return;
                }


                if (!Convert.ToBoolean(this.ViewState["modificato"]) && !Convert.ToBoolean(this.ViewState["modificatoCanale"]))
                {
                    string msg = "Non è stata effettuata alcuna modifica";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "NoMod", "alert('" + msg + "');", true);
                    this.btn_modifica.Enabled = true;
                    SetFocusOnPanel();
                    return;
                }
            } //fine if Emanuela occasionale

            if (verificaSelezione(testoDelPulsante))
            {
                //Emanuela: gestione utente occasionale
                if (!((Button)sender).Text.Equals("Salva come occasionale e protocolla"))
                {
                    if (this.hd_tipo_corr != null && (!string.IsNullOrEmpty(this.hd_tipo_corr.Value)))
                    {
                        switch (hd_tipo_corr.Value)
                        {
                            case "U":
                                if (Session["Interop"] != null)
                                    if (Session["Interop"].ToString().Equals("S") || (((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue.Equals("U")))
                                        modifyUO(ref datiModifica);
                                    else if (Session["Interop"].ToString().Equals("P") && (((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue.Equals("P")))
                                    {
                                        modifyUtente(ref datiModifica);
                                    }
                                    else if (Session["Interop"].ToString().Equals("P") && (((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue.Equals("R")))
                                    {
                                        modifyRuolo(ref datiModifica);
                                    }
                                break;
                            case "R":
                                modifyRuolo(ref datiModifica);
                                break;
                            case "P":
                                modifyUtente(ref datiModifica);
                                break;
                        }
                    }
                    else if (this.hd_tipo_URP != null && this.hd_tipo_URP.Value != "")
                    {
                        switch (hd_tipo_URP.Value)
                        {
                            case "U":
                                if (Session["Interop"] != null)
                                    if (Session["Interop"].ToString().Equals("S") || (((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue.Equals("U")))
                                        modifyUO(ref datiModifica);
                                    else if (Session["Interop"].ToString().Equals("P") && (((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue.Equals("P")))
                                    {
                                        modifyUtente(ref datiModifica);
                                    }
                                    else if (Session["Interop"].ToString().Equals("P") && (((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue.Equals("R")))
                                    {
                                        modifyRuolo(ref datiModifica);
                                    }
                                break;
                            case "R":
                                modifyRuolo(ref datiModifica);
                                break;
                            case "P":
                                modifyUtente(ref datiModifica);
                                break;
                        }
                    }

                    //operazione andata a buon fine
                }// fine if Emanuela occasionale
                if (string.IsNullOrEmpty(newCorr))
                {
                    //Emanuela:gestione utente occasionale
                    if (!((Button)sender).Text.Equals("Salva come occasionale e protocolla"))
                    {
                        if (UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "M", out message))
                        {
                            switch (message)
                            {
                                case "OK":
                                    message = "Modifica del corrispondente avvenuta con successo";
                                    InsertComboMailsCorr(datiModifica.idCorrGlobali);
                                    break;
                                default:
                                    message =
                                        "Attenzione: si è verificato un errore nella procedura di modifica del corrispondente";
                                    break;
                            }

                            if (message != null && message != "")
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + message + "');", true);
                            }

                            string updatedID = this.hd_systemId.Value;
                            this.btn_modifica.Enabled = false;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Refresh", "window.returnValue='REFRESH?ID=" + updatedID + "'; window.close();", true);

                        }
                        else
                        {
                            if (message != null && message != "")
                            {
                                if (message.Equals("KO"))
                                    message = "Attenzione: si è verificato un errore durante la modifica del corrispondente";
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "KO", "alert('" + message + "');", true);
                            }
                        }
                    }//fine if utente occasionale
                }
                else
                {
                    string corr_type = ((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue;
                    string idCorr = "";
                    #region Ci sono + di un corrispondente
                    if (sameMail.Equals("same_mail"))
                    {
                        string codUpper = datiModifica.codRubrica.ToUpper();
                        #region Non scelgo niente
                        if (((HiddenField)FindControl("hd_field")).Value == "")
                        {

                            if (datiModifica.codRubrica == Session["codRubrica"] as string || codUpper.Contains("INTEROP"))
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckCodRubrica",
                                                                    "alert('Inserire un codice rubrica diverso')", true);
                                return;
                            }
                            if (codUpper.Contains("@"))
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckCodRubrica",
                                                                    "alert('Il codice rubrica non deve contenere il simbolo @')", true);
                                return;
                            }
                            #region Ho l'avviso 1
                            if (avviso.Equals("1"))
                            {
                                if (UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "M",
                                                                                  out newIdCorr, out message))
                                {
                                    switch (message)
                                    {
                                        case "OK":
                                            InsertComboMailsCorr(newIdCorr);
                                            message = "Modifica del corrispondente avvenuta con successo";

                                            break;
                                        default:
                                            message =
                                                "Attenzione: si è verificato un errore nella procedura di modifica del corrispondente";
                                            break;
                                    }

                                    if (message != null && message != "")
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert",
                                                                            "alert('" + message + "');", true);
                                    }

                                    ws.ResetCodRubCorrIterop(newIdCorr, datiModifica.codRubrica);
                                    ws.UpdateDocArrivoFromInterop(sysid, newIdCorr);
                                    this.btn_modifica.Enabled = false;
                                    if (newIdCorr != null)
                                    {
                                        Session.Remove("ListCorrByMail");
                                        Session.Remove("IdRegistro");
                                        Session.Remove("Mail");
                                        Session.Remove("codRubrica");
                                        Session.Remove("Interop");


                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Return",
                                                                            "window.returnValue='newIdCorr=" + newIdCorr +
                                                                            "'; window.close();", true);
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error",
                                                                            "alert('Attenzione! Corrispondente non inserito. Codice rubrica già presente.');", true);
                                    }
                                }
                                else
                                {
                                    if (message != null && message != "")
                                    {
                                        if (message.Equals("KO"))
                                            message =
                                                "Attenzione: si è verificato un errore durante la modifica del corrispondente";
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Alert",
                                                                            "alert('" + message + "');", true);
                                    }
                                }
                            }
                            #endregion
                            #region Ho l'avviso 2
                            else if (avviso.Equals("2"))
                            {
                                //creo l'oggetto canale
                                DocsPaWR.Canale canale = new SAAdminTool.DocsPaWR.Canale();
                                canale.systemId = this.dd_canpref.SelectedItem.Value;
                                DocsPaVO.addressbook.DettagliCorrispondente dettagli =
                                             new DocsPaVO.addressbook.DettagliCorrispondente();
                                if (Session["Interop"] != null)
                                    #region Sono Interop
                                    if (Session["Interop"].ToString().Equals("S"))
                                    {
                                        //Creo il nuovo corrispondente e lo associ al documento
                                        UnitaOrganizzativa uo = new UnitaOrganizzativa();

                                        uo.tipoIE = ie;
                                        //uo.tipoCorrispondente = t; // utilizzando questo prende S e genera eccezione dopo.
                                        uo.tipoCorrispondente = "U";
                                        uo.codiceAmm = datiModifica.codiceAmm;
                                        uo.codiceAOO = datiModifica.codiceAoo;
                                        uo.email = datiModifica.email;
                                        uo.codiceRubrica = datiModifica.codRubrica;
                                        uo.canalePref = canale;
                                        uo.descrizione = datiModifica.descCorr;
                                        uo.oldDescrizione = Session["OldDescrizioneMitt"].ToString();
                                        if (Session["IdRegistro"] != null)
                                            uo.idRegistro = Session["IdRegistro"].ToString();

                                        uo.idAmministrazione = UserManager.getInfoUtente().idAmministrazione;
                                        uo.info = new DocsPaVO.addressbook.DettagliCorrispondente();
                                        dettagli.Corrispondente.AddCorrispondenteRow(
                                            datiModifica.indirizzo.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                            datiModifica.citta.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                            datiModifica.cap.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                            datiModifica.provincia.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                            datiModifica.nazione.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                            datiModifica.telefono.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                            datiModifica.telefono2.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                            datiModifica.fax,
                                            datiModifica.codFiscale.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                            datiModifica.note.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                            datiModifica.localita.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                            string.Empty, string.Empty, string.Empty,
                                            datiModifica.partitaIva.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()));

                                        uo.info = dettagli;
                                        uo.dettagli = true;

                                        if (datiModifica.codRubrica == "")
                                        {
                                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CampiObbligatori",
                                                                                "alert('Codice Rubrica obbligatorio')", true);
                                            return;
                                        }
                                        if (datiModifica.descCorr == "")
                                        {
                                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CampiObbligatori",
                                                                                "alert('Descrizione obbligatoria')", true);
                                            return;
                                        }


                                        Corrispondente newCorrispondente = UserManager.addressbookInsertCorrispondente(this, uo,
                                                                                                                        null);
                                        idCorr = newCorrispondente.systemId;
                                        InsertComboMailsCorr(idCorr);
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Return",
                                                                        "window.returnValue='newIdCorr=" + idCorr +
                                                                        "'; window.close();", true);
                                    }
                                    #endregion
                                    #region Non sono Interop
                                    else if (Session["Interop"].ToString().Equals("P"))
                                    {
                                        switch (corr_type)
                                        {
                                            case "U":

                                                //Creo il nuovo corrispondente e lo associ al documento
                                                UnitaOrganizzativa uo = new UnitaOrganizzativa();

                                                uo.tipoIE = ie;
                                                //uo.tipoCorrispondente = t; // utilizzando questo prende S e genera eccezione dopo.
                                                uo.tipoCorrispondente = "U";
                                                uo.codiceAmm = datiModifica.codiceAmm;
                                                uo.codiceAOO = datiModifica.codiceAoo;
                                                uo.email = datiModifica.email;
                                                uo.codiceRubrica = datiModifica.codRubrica;
                                                uo.canalePref = canale;
                                                uo.descrizione = datiModifica.descCorr;

                                                if (Session["IdRegistro"] != null)
                                                    uo.idRegistro = Session["IdRegistro"].ToString();

                                                uo.idAmministrazione = UserManager.getInfoUtente().idAmministrazione;
                                                uo.info = new DocsPaVO.addressbook.DettagliCorrispondente();
                                                dettagli.Corrispondente.AddCorrispondenteRow(
                                                    datiModifica.indirizzo.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                                    datiModifica.citta.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                                    datiModifica.cap.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                                    datiModifica.provincia.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                                    datiModifica.nazione.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                                    datiModifica.telefono.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                                    datiModifica.telefono2.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                                    datiModifica.fax,
                                                    datiModifica.codFiscale.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                                    datiModifica.note.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                                    datiModifica.localita.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                                    string.Empty, string.Empty, string.Empty,
                                                     datiModifica.partitaIva.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()));

                                                uo.info = dettagli;
                                                uo.dettagli = true;

                                                if (datiModifica.codRubrica == "")
                                                {
                                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CampiObbligatori",
                                                                                        "alert('Codice Rubrica obbligatorio')", true);
                                                    return;
                                                }
                                                if (datiModifica.descCorr == "")
                                                {
                                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CampiObbligatori",
                                                                                        "alert('Descrizione obbligatoria')", true);
                                                    return;
                                                }

                                                codUpper = datiModifica.codRubrica.ToUpper();
                                                if (datiModifica.codRubrica == Session["codRubrica"] as string || codUpper.Contains("INTEROP"))
                                                {
                                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckCodRubrica",
                                                                                        "alert('Inserire un codice rubrica diverso')", true);
                                                    return;
                                                }
                                                if (codUpper.Contains("@"))
                                                {
                                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckCodRubrica",
                                                                                        "alert('Il codice rubrica non deve contenere il simbolo @')", true);
                                                    return;
                                                }


                                                Corrispondente newCorrispondente = UserManager.addressbookInsertCorrispondente(this, uo,
                                                                                                                                null);
                                                idCorr = newCorrispondente.systemId;
                                                InsertComboMailsCorr(idCorr);
                                                if (idCorr != null)
                                                {
                                                    Session.Remove("ListCorrByMail");
                                                    Session.Remove("IdRegistro");
                                                    Session.Remove("Mail");
                                                    Session.Remove("codRubrica");
                                                    Session.Remove("Interop");


                                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Return",
                                                                                        "window.returnValue='newIdCorr=" + idCorr +
                                                                                        "'; window.close();", true);
                                                }
                                                else
                                                {
                                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error",
                                                                                        "alert('Attenzione! Corrispondente non inserito. Codice rubrica già presente.');", true);
                                                }

                                                break;

                                            case "R":

                                                Corrispondente res = new SAAdminTool.DocsPaWR.Corrispondente();
                                                DocsPaWR.Ruolo ruolo = new SAAdminTool.DocsPaWR.Ruolo();
                                                ruolo.tipoCorrispondente = "R";
                                                ruolo.codiceCorrispondente = txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                                ruolo.codiceRubrica = txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                                ruolo.descrizione = txt_descrizione.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());

                                                ruolo.idRegistro = Session["IdRegistro"].ToString();
                                                ruolo.email = txt_EmailAOO.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                                ruolo.codiceAmm = txt_CodAmm.Text;
                                                ruolo.codiceAOO = txt_CodAOO.Text;
                                                ruolo.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
                                                DocsPaWR.UnitaOrganizzativa parent_uo = new UnitaOrganizzativa();
                                                parent_uo.descrizione = "";
                                                parent_uo.systemId = "0";

                                                ruolo.canalePref = canale;
                                                res = UserManager.addressbookInsertCorrispondente(this, ruolo, parent_uo);
                                                idCorr = res.systemId;
                                                InsertComboMailsCorr(idCorr);
                                                if (idCorr != null)
                                                {
                                                    Session.Remove("ListCorrByMail");
                                                    Session.Remove("IdRegistro");
                                                    Session.Remove("Mail");
                                                    Session.Remove("codRubrica");
                                                    Session.Remove("Interop");


                                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Return",
                                                                                        "window.returnValue='newIdCorr=" + idCorr +
                                                                                        "'; window.close();", true);
                                                }
                                                else
                                                {
                                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error",
                                                                                        "alert('Attenzione! Corrispondente non inserito. Codice rubrica già presente.');", true);
                                                }
                                                break;

                                            case "P":
                                                res = new SAAdminTool.DocsPaWR.Corrispondente();
                                                DocsPaWR.Utente utente = new SAAdminTool.DocsPaWR.Utente();
                                                utente.codiceCorrispondente = txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                                utente.codiceRubrica = txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                                utente.cognome = txt_cognome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                                utente.nome = txt_nome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                                utente.email = txt_EmailAOO.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                                // utente.idRegistro = reg_corr.systemId;
                                                utente.codiceAmm = txt_CodAmm.Text;
                                                utente.codiceAOO = txt_CodAOO.Text;
                                                utente.descrizione = this.dd_titolo.Text + txt_cognome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()) + txt_nome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                                utente.luogoDINascita = txt_luogoNascita.Text;
                                                utente.dataNascita = txt_dataNascita.Text;
                                                utente.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
                                                utente.titolo = this.dd_titolo.Text;

                                                utente.idRegistro = Session["IdRegistro"].ToString();

                                                utente.tipoCorrispondente = ((DropDownList)FindControl("ddl_tipoCorr")).SelectedItem.Value;
                                                utente.canalePref = canale;

                                                if ((txt_Indirizzo.Text != null && !txt_Indirizzo.Equals("")) ||
                                                    (txt_CAP.Text != null && !txt_CAP.Equals("")) ||
                                                    (txt_Citta.Text != null && !txt_Citta.Equals("")) ||
                                                    (txt_Prov.Text != null && !txt_Prov.Equals("")) ||
                                                    (txt_Nazione.Text != null && !txt_Nazione.Equals("")) ||
                                                    (txt_Tel1.Text != null && !txt_Tel1.Equals("")) ||
                                                    (txt_Tel2.Text != null && !txt_Tel2.Equals("")) ||
                                                    (txt_Fax.Text != null && !txt_Fax.Equals("")) ||
                                                    (txt_CodFis.Text != null && !txt_CodFis.Equals("")) ||
                                                    (txt_note.Text != null && !txt_note.Equals("")) ||
                                                    (txt_PI.Text != null && !txt_PI.Equals("")))
                                                {
                                                    dettagli.Corrispondente.AddCorrispondenteRow(
                                                        txt_Indirizzo.Text, txt_Citta.Text, txt_CAP.Text,
                                                        txt_Prov.Text, txt_Nazione.Text, txt_Tel1.Text,
                                                        txt_Tel2.Text, txt_Fax.Text, txt_CodFis.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                                        txt_note.Text, txt_local.Text, txt_luogoNascita.Text, txt_dataNascita.Text,
                                                        dd_titolo.Text, txt_PI.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()));

                                                    utente.info = dettagli;
                                                    utente.dettagli = true;
                                                }
                                                if (datiModifica.nome == "")
                                                {
                                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CampiObbligatori",
                                                                                       "alert('Nome obbligatorio')", true);
                                                    return;
                                                }
                                                if (datiModifica.cognome == "")
                                                {
                                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CampiObbligatori",
                                                                                      "alert('Cognome obbligatorio')", true);
                                                    return;
                                                }
                                                res = UserManager.addressbookInsertCorrispondente(this, utente, null);
                                                idCorr = res.systemId;
                                                InsertComboMailsCorr(idCorr);
                                                if (idCorr != null)
                                                {
                                                    Session.Remove("ListCorrByMail");
                                                    Session.Remove("IdRegistro");
                                                    Session.Remove("Mail");
                                                    Session.Remove("codRubrica");
                                                    Session.Remove("Interop");


                                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Return",
                                                                                        "window.returnValue='newIdCorr=" + idCorr +
                                                                                        "'; window.close();", true);
                                                }
                                                else
                                                {
                                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error",
                                                                                        "alert('Attenzione! Corrispondente non inserito. Codice rubrica già presente.');", true);
                                                }
                                                break;
                                        }
                                    }
                                    #endregion

                            }
                            #endregion
                        }
                        #endregion
                        #region Scelgo
                        else
                        {
                            //UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "D",
                            //                                              out message);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Return", "window.returnValue='newIdCorr=" + ((HiddenField)FindControl("hd_field")).Value + "'; window.close();", true);
                            Session.Remove("ListCorrByMail");
                            Session.Remove("IdRegistro");
                            Session.Remove("Mail");
                            Session.Remove("codRubrica");
                            Session.Remove("Interop");
                            //return;
                        }
                        #endregion
                    }
                    #endregion

                    #region C'è zero o un solo corrispondente
                    else
                    {
                        //Emanuela:gestione occasionale
                        if (!((Button)sender).Text.Equals("Salva come occasionale e protocolla"))
                        {
                            //creo l'oggetto canale
                            DocsPaWR.Canale canale = new SAAdminTool.DocsPaWR.Canale();
                            canale.systemId = this.dd_canpref.SelectedItem.Value;
                            DocsPaVO.addressbook.DettagliCorrispondente dettagli =
                                         new DocsPaVO.addressbook.DettagliCorrispondente();

                            switch (corr_type)
                            {
                                case "U":
                                    //Creo il nuovo corrispondente e lo associ al documento
                                    UnitaOrganizzativa uo = new UnitaOrganizzativa();

                                    uo.tipoIE = ie;
                                    //uo.tipoCorrispondente = t; // utilizzando questo prende S e genera eccezione dopo.
                                    uo.tipoCorrispondente = "U";
                                    uo.codiceAmm = datiModifica.codiceAmm;
                                    uo.codiceAOO = datiModifica.codiceAoo;
                                    uo.email = datiModifica.email;
                                    uo.codiceRubrica = datiModifica.codRubrica;
                                    uo.canalePref = canale;
                                    uo.descrizione = datiModifica.descCorr;
                                    uo.oldDescrizione = Session["OldDescrizioneMitt"].ToString();
                                    if (Session["IdRegistro"] != null)
                                        uo.idRegistro = Session["IdRegistro"].ToString();

                                    uo.idAmministrazione = UserManager.getInfoUtente().idAmministrazione;
                                    uo.info = new DocsPaVO.addressbook.DettagliCorrispondente();

                                    string indirizzo = string.Empty;
                                    if (!string.IsNullOrEmpty(datiModifica.indirizzo))
                                        indirizzo = datiModifica.indirizzo.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());

                                    string citta = string.Empty;
                                    if (!string.IsNullOrEmpty(datiModifica.citta))
                                        citta = datiModifica.citta.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());

                                    string cap = string.Empty;
                                    if (!string.IsNullOrEmpty(datiModifica.cap))
                                        cap = datiModifica.cap.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());

                                    string provincia = string.Empty;
                                    if (!string.IsNullOrEmpty(datiModifica.provincia))
                                        provincia = datiModifica.provincia.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());

                                    string nazione = string.Empty;
                                    if (!string.IsNullOrEmpty(datiModifica.nazione))
                                        nazione = datiModifica.nazione.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());

                                    string telefono = string.Empty;
                                    if (!string.IsNullOrEmpty(datiModifica.telefono))
                                        telefono = datiModifica.telefono.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());

                                    string telefono2 = string.Empty;
                                    if (!string.IsNullOrEmpty(datiModifica.telefono2))
                                        telefono2 = datiModifica.telefono2.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());

                                    string codFiscale = string.Empty;
                                    if (!string.IsNullOrEmpty(datiModifica.codFiscale))
                                        codFiscale = datiModifica.codFiscale.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());

                                    string partitaIva = string.Empty;
                                    if (!string.IsNullOrEmpty(datiModifica.partitaIva))
                                        partitaIva = datiModifica.partitaIva.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());

                                    string note = string.Empty;
                                    if (!string.IsNullOrEmpty(datiModifica.note))
                                        note = datiModifica.note.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());

                                    string localita = string.Empty;
                                    if (!string.IsNullOrEmpty(datiModifica.localita))
                                        localita = datiModifica.localita.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());

                                    dettagli.Corrispondente.AddCorrispondenteRow(
                                        indirizzo,
                                        citta,
                                        cap,
                                        provincia,
                                        nazione,
                                        telefono,
                                        telefono2,
                                        datiModifica.fax,
                                        codFiscale,
                                        note,
                                        localita,
                                        string.Empty, string.Empty, string.Empty, partitaIva);

                                    uo.info = dettagli;
                                    uo.dettagli = true;

                                    if (datiModifica.codRubrica == "")
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CampiObbligatori",
                                                                            "alert('Codice Rubrica obbligatorio')", true);
                                        return;
                                    }
                                    if (datiModifica.descCorr == "")
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CampiObbligatori",
                                                                            "alert('Descrizione obbligatoria')", true);
                                        return;
                                    }


                                    string codUpper = datiModifica.codRubrica.ToUpper();
                                    if (avviso.Equals("2"))
                                    {
                                        if (codUpper.Contains("INTEROP"))
                                        {
                                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckCodRubrica",
                                                                                "alert('Inserire un codice rubrica diverso')", true);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        if (datiModifica.codRubrica == Session["codRubrica"] as string || codUpper.Contains("INTEROP"))
                                        {
                                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckCodRubrica",
                                                                                "alert('Inserire un codice rubrica diverso')", true);
                                            return;
                                        }
                                    }

                                    /*if (datiModifica.codRubrica == Session["codRubrica"] as string || codUpper.Contains("INTEROP"))
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckCodRubrica",
                                                                            "alert('Inserire un codice rubrica diverso')", true);
                                        return;
                                    }*/
                                    if (codUpper.Contains("@"))
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckCodRubrica",
                                                                            "alert('Il codice rubrica non deve contenere il simbolo @')", true);
                                        return;
                                    }

                                    if (!avviso.Equals("2"))
                                    {
                                        // Se il codice è già presente in rubrica per lo specifico registro
                                        // non si deve procedere
                                        if (!ws.IsCodRubricaPresente(datiModifica.codRubrica, hd_tipo_corr.Value, UserManager.getInfoUtente().idAmministrazione, hd_idReg.Value, datiModifica.inRubricaComune))
                                        {
                                            UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "M",
                                                                                            out idCorr,
                                                                                            out message);
                                            switch (message)
                                            {
                                                case "OK":
                                                    InsertComboMailsCorr(idCorr);
                                                    message = "Modifica del corrispondente avvenuta con successo";

                                                    break;
                                                default:
                                                    message =
                                                        "Attenzione: si è verificato un errore nella procedura di modifica del corrispondente";
                                                    break;
                                            }

                                            if (message != null && message != "")
                                            {
                                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert",
                                                                                    "alert('" + message + "');", true);
                                                //Response.Write("<SCRIPT>alert('" + message + "');</SCRIPT>");
                                            }

                                            ws.ResetCodRubCorrIterop(idCorr, datiModifica.codRubrica);
                                            ws.UpdateDocArrivoFromInterop(sysid, idCorr);
                                        }
                                        else
                                            idCorr = null;

                                        if (idCorr != null)
                                        {
                                            Session.Remove("ListCorrByMail");
                                            Session.Remove("IdRegistro");
                                            Session.Remove("Mail");
                                            Session.Remove("codRubrica");
                                            Session.Remove("Interop");

                                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Return",
                                                                                "window.returnValue='newIdCorr=" + idCorr +
                                                                                "'; window.close();", true);
                                        }
                                        else
                                        {
                                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error",
                                                                                "alert('Attenzione! Corrispondente non inserito. Codice rubrica già presente.');", true);
                                        }
                                    }
                                    else
                                    {
                                        Corrispondente newCorrispondente;

                                        // Se ci si trova nel caso di inserimento di un nuovo corrispondente, si procede 
                                        // con l'inserimento altrimenti viene utilizzato il corrispondente proposto
                                        if (((Button)FindControl("btn_protocolla")).Text == "Crea e Protocolla")
                                            newCorrispondente = UserManager.addressbookInsertCorrispondente(this, uo, null);
                                        else
                                            newCorrispondente = UserManager.getCorrispondenteByCodRubrica(this.Page, uo.codiceRubrica);

                                        idCorr = newCorrispondente.systemId;
                                        InsertComboMailsCorr(idCorr);
                                        if (idCorr != null)
                                        {
                                            Session.Remove("ListCorrByMail");
                                            Session.Remove("IdRegistro");
                                            Session.Remove("Mail");
                                            Session.Remove("codRubrica");
                                            Session.Remove("Interop");

                                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Return",
                                                                                "window.returnValue='newIdCorr=" + idCorr +
                                                                                "'; window.close();", true);
                                        }
                                        else
                                        {
                                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error",
                                                                                  "alert('Attenzione! Corrispondente non inserito. Codice rubrica già presente.');", true);
                                            ////restituisco il corrispondente già proposto come mittente da docProtocollo
                                            //idCorr = UserManager.getCorrispondenteByCodRubrica(this.Page, uo.codiceRubrica).systemId;
                                            //if (idCorr != null)
                                            //{
                                            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Return",
                                            //                                        "window.returnValue='newIdCorr=" + idCorr +
                                            //                                        "';window.close();", true);
                                            //}
                                            //else
                                            //{
                                            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error",
                                            //        "alert('Attenzione! Corrispondente non inserito. Codice rubrica già presente.');", true);
                                            //}
                                        }

                                    }
                                    break;

                                case "R":
                                    Corrispondente res = new SAAdminTool.DocsPaWR.Corrispondente();
                                    DocsPaWR.Ruolo ruolo = new SAAdminTool.DocsPaWR.Ruolo();
                                    ruolo.tipoCorrispondente = "R";
                                    ruolo.codiceCorrispondente = txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                    ruolo.codiceRubrica = txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                    ruolo.descrizione = txt_descrizione.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());

                                    ruolo.idRegistro = Session["IdRegistro"].ToString();
                                    ruolo.email = txt_EmailAOO.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                    ruolo.codiceAmm = txt_CodAmm.Text;
                                    ruolo.codiceAOO = txt_CodAOO.Text;
                                    ruolo.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
                                    DocsPaWR.UnitaOrganizzativa parent_uo = new UnitaOrganizzativa();
                                    parent_uo.descrizione = "";
                                    parent_uo.systemId = "0";

                                    ruolo.canalePref = canale;
                                    //res = UserManager.addressbookInsertCorrispondente(this, ruolo, parent_uo);
                                    //idCorr = res.systemId;
                                    string codUpp = datiModifica.codRubrica.ToUpper();
                                    if (!avviso.Equals("2") && datiModifica.codRubrica == Session["codRubrica"] as string)
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckCodRubrica",
                                                                            "alert('Inserire un codice rubrica diverso')", true);
                                        return;
                                    }
                                    if (codUpp.Contains("INTEROP"))
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckCodRubrica",
                                                                            "alert('Inserire un codice rubrica diverso')", true);
                                        return;
                                    }
                                    if (codUpp.Contains("@"))
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckCodRubrica",
                                                                            "alert('Il codice rubrica non deve contenere il simbolo @')", true);
                                        return;
                                    }


                                    if (!avviso.Equals("2"))
                                    {
                                        UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "D",
                                                                                            out idCorr,
                                                                                            out message);
                                        switch (message)
                                        {
                                            case "OK":
                                                SAAdminTool.utils.MultiCasellaManager.DeleteMailCorrispondenteEsterno(idCorr);
                                                message = "Modifica del corrispondente avvenuta con successo";
                                                break;
                                            default:
                                                message =
                                                    "Attenzione: si è verificato un errore nella procedura di modifica del corrispondente";
                                                break;
                                        }

                                        if (message != null && message != "")
                                        {
                                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert",
                                                                                    "alert('" + message + "');", true);
                                        }
                                        //ws.ResetCodRubCorrIterop(idCorr, datiModifica.codRubrica);
                                        Corrispondente newCorrispondente = UserManager.addressbookInsertCorrispondente(this, ruolo, parent_uo);
                                        idCorr = newCorrispondente.systemId;
                                        InsertComboMailsCorr(idCorr);
                                        ws.UpdateDocArrivoFromInterop(sysid, idCorr);
                                    }
                                    else
                                    {
                                        Corrispondente newCorrispondente;

                                        // Se ci si trova nel caso di inserimento di un nuovo corrispondente, si procede 
                                        // con l'inserimento altrimenti viene utilizzato il corrispondente proposto
                                        if (((Button)FindControl("btn_protocolla")).Text == "Crea e Protocolla")
                                            newCorrispondente = UserManager.addressbookInsertCorrispondente(this, ruolo, parent_uo);
                                        else
                                            newCorrispondente = UserManager.getCorrispondenteBySystemID(this.Page, datiModifica.idCorrGlobali);

                                        idCorr = newCorrispondente.systemId;
                                        InsertComboMailsCorr(idCorr);
                                    }

                                    if (idCorr != null)
                                    {
                                        Session.Remove("ListCorrByMail");
                                        Session.Remove("IdRegistro");
                                        Session.Remove("Mail");
                                        Session.Remove("codRubrica");
                                        Session.Remove("Interop");

                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Return",
                                                                            "window.returnValue='newIdCorr=" + idCorr +
                                                                            "'; window.close();", true);
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error",
                                                                                  "alert('Attenzione! Corrispondente non inserito. Codice rubrica già presente.');", true);
                                        ////restituisco il corrispondente già proposto come mittente da docProtocollo
                                        //idCorr = UserManager.getCorrispondenteBySystemID(this.Page, datiModifica.idCorrGlobali).systemId;
                                        //if (idCorr != null)
                                        //{
                                        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Return",
                                        //                                        "window.returnValue='newIdCorr=" + idCorr +
                                        //                                        "';window.close();", true);
                                        //}
                                        //else
                                        //{
                                        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error",
                                        //        "alert('Attenzione! Corrispondente non inserito. Codice rubrica già presente.');", true);
                                        //}

                                    }
                                    break;

                                case "P":
                                    res = new SAAdminTool.DocsPaWR.Corrispondente();
                                    DocsPaWR.Utente utente = new SAAdminTool.DocsPaWR.Utente();
                                    utente.codiceCorrispondente = txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                    utente.codiceRubrica = txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                    utente.cognome = txt_cognome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                    utente.nome = txt_nome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                    utente.email = txt_EmailAOO.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                    // utente.idRegistro = reg_corr.systemId;
                                    utente.codiceAmm = txt_CodAmm.Text;
                                    utente.codiceAOO = txt_CodAOO.Text;
                                    utente.descrizione = this.dd_titolo.Text + txt_cognome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()) + txt_nome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                    utente.luogoDINascita = txt_luogoNascita.Text;
                                    utente.dataNascita = txt_dataNascita.Text;
                                    utente.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
                                    utente.titolo = this.dd_titolo.Text;

                                    utente.idRegistro = Session["IdRegistro"].ToString();

                                    utente.tipoCorrispondente = ((DropDownList)FindControl("ddl_tipoCorr")).SelectedItem.Value;
                                    utente.canalePref = canale;

                                    if ((txt_Indirizzo.Text != null && !txt_Indirizzo.Equals("")) ||
                                        (txt_CAP.Text != null && !txt_CAP.Equals("")) ||
                                        (txt_Citta.Text != null && !txt_Citta.Equals("")) ||
                                        (txt_Prov.Text != null && !txt_Prov.Equals("")) ||
                                        (txt_Nazione.Text != null && !txt_Nazione.Equals("")) ||
                                        (txt_Tel1.Text != null && !txt_Tel1.Equals("")) ||
                                        (txt_Tel2.Text != null && !txt_Tel2.Equals("")) ||
                                        (txt_Fax.Text != null && !txt_Fax.Equals("")) ||
                                        (txt_CodFis.Text != null && !txt_CodFis.Equals("")) ||
                                        (txt_note.Text != null && !txt_note.Equals("")) ||
                                        (txt_PI.Text != null && !txt_PI.Equals("")))
                                    {
                                        dettagli.Corrispondente.AddCorrispondenteRow(
                                            txt_Indirizzo.Text, txt_Citta.Text, txt_CAP.Text,
                                            txt_Prov.Text, txt_Nazione.Text, txt_Tel1.Text,
                                            txt_Tel2.Text, txt_Fax.Text, txt_CodFis.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                            txt_note.Text, txt_local.Text, txt_luogoNascita.Text, txt_dataNascita.Text,
                                            dd_titolo.Text, txt_PI.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()));

                                        utente.info = dettagli;
                                        utente.dettagli = true;
                                    }
                                    if (txt_nome.Text == "")
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CampiObbligatori",
                                                                           "alert('Nome obbligatorio')", true);
                                        return;
                                    }
                                    if (txt_cognome.Text == "")
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CampiObbligatori",
                                                                          "alert('Cognome obbligatorio')", true);
                                        return;
                                    }


                                    if (datiModifica.codRubrica.ToUpper().Contains("INTEROP"))
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckCodRubrica",
                                                                            "alert('Inserire un codice rubrica diverso')", true);
                                        return;
                                    }
                                    if (datiModifica.codRubrica.Contains("@"))
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CheckCodRubrica",
                                                                            "alert('Il codice rubrica non deve contenere il simbolo @')", true);
                                        return;
                                    }

                                    if (!avviso.Equals("2"))
                                    {
                                        UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "M",
                                                                                            out idCorr,
                                                                                            out message);
                                        switch (message)
                                        {
                                            case "OK":
                                                InsertComboMailsCorr(idCorr);
                                                message = "Modifica del corrispondente avvenuta con successo";
                                                break;
                                            default:
                                                message =
                                                    "Attenzione: si è verificato un errore nella procedura di modifica del corrispondente";
                                                break;
                                        }

                                        if (message != null && message != "")
                                        {
                                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert",
                                                                                    "alert('" + message + "');", true);
                                        }
                                        ws.ResetCodRubCorrIterop(idCorr, datiModifica.codRubrica);
                                        ws.UpdateDocArrivoFromInterop(sysid, idCorr);
                                    }
                                    else
                                    {
                                        Corrispondente newCorrispondente;

                                        // Se ci si trova nel caso di inserimento di un nuovo corrispondente, si procede 
                                        // con l'inserimento altrimenti viene utilizzato il corrispondente proposto
                                        if (((Button)FindControl("btn_protocolla")).Text == "Crea e Protocolla")
                                            newCorrispondente = UserManager.addressbookInsertCorrispondente(this, utente, null);
                                        else
                                            newCorrispondente = UserManager.getCorrispondenteBySystemID(this.Page, datiModifica.idCorrGlobali);

                                        idCorr = newCorrispondente.systemId;
                                        InsertComboMailsCorr(idCorr);
                                    }

                                    //res = UserManager.addressbookInsertCorrispondente(this, utente, null);
                                    //idCorr = res.systemId;

                                    if (idCorr != null)
                                    {
                                        Session.Remove("ListCorrByMail");
                                        Session.Remove("IdRegistro");
                                        Session.Remove("Mail");
                                        Session.Remove("codRubrica");
                                        Session.Remove("Interop");
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Return",
                                                                                "window.returnValue='newIdCorr=" + idCorr +
                                                                                "'; window.close();", true);
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error",
                                                                                  "alert('Attenzione! Corrispondente non inserito. Codice rubrica già presente.');", true);
                                        //restituisco il corrispondente già proposto come mittente da docProtocollo
                                        //idCorr = UserManager.getCorrispondenteBySystemID(this.Page, datiModifica.idCorrGlobali).systemId;
                                        //if (idCorr != null)
                                        //{
                                        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Return",
                                        //                                        "window.returnValue='newIdCorr=" + idCorr +
                                        //                                        "';window.close();", true);
                                        //}
                                        //else
                                        //{
                                        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error",
                                        //        "alert('Attenzione! Corrispondente non inserito. Codice rubrica già presente.');", true);
                                        //}
                                    }
                                    break;
                            }
                        }//fine if Emanuela occasionale
                        else //Se il corrispondente è di tipo occasionale
                        {
                            datiModifica.idCorrGlobali = this.sysid;
                            datiModifica.codRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                            datiModifica.codiceAmm = this.txt_CodAmm.Text;
                            datiModifica.descCorr = this.email.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                            datiModifica.tipoCorrispondente = "O";

                            if (UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "M", out idCorr, out message))
                            {
                                switch (message)
                                {
                                    case "OK":
                                        message = "Modifica del corrispondente avvenuta con successo";
                                        break;
                                    default:
                                        message =
                                            "Attenzione: si è verificato un errore nella procedura di modifica del corrispondente";
                                        break;
                                }

                                if (message != null && message != "")
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + message + "');", true);
                                }
                                DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoSelezionato();
                                string sysid_doc = schedaDoc.docNumber;
                                ws.UpdateDocArrivoFromInteropOccasionale(sysid_doc, idCorr);
                                if (idCorr != null)
                                {
                                    Session.Remove("ListCorrByMail");
                                    Session.Remove("IdRegistro");
                                    Session.Remove("Mail");
                                    Session.Remove("codRubrica");
                                    Session.Remove("Interop");

                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Return",
                                                                                "window.returnValue='newIdCorr=" + idCorr +
                                                                                "'; window.close();", true);
                                }
                                else
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error",
                                                                              "alert('Attenzione: si è verificato un errore nella procedura di modifica del corrispondente.');", true);
                                }
                            }
                        }
                    #endregion
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(messaggioErrore))
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + messaggioErrore + "');", true);
            }
        }

        //reperimento dei dati per la modifica di una uo
        private void modifyUO(ref SAAdminTool.DocsPaWR.DatiModificaCorr datiModifica)
        {
            datiModifica.idCorrGlobali = this.hd_systemId.Value;
            datiModifica.descCorr = this.txt_descrizione.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codiceAoo = this.txt_CodAOO.Text;
            datiModifica.codiceAmm = this.txt_CodAmm.Text;
            datiModifica.email = this.txt_EmailAOO.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codice = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.nome = "";
            datiModifica.cognome = "";
            datiModifica.indirizzo = this.txt_Indirizzo.Text;
            datiModifica.cap = this.txt_CAP.Text;
            datiModifica.provincia = this.txt_Prov.Text;
            datiModifica.nazione = this.txt_Nazione.Text;
            datiModifica.citta = this.txt_Citta.Text;
            datiModifica.codFiscale = this.txt_CodFis.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.partitaIva = this.txt_PI.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.telefono = this.txt_Tel1.Text;
            datiModifica.telefono2 = this.txt_Tel2.Text;
            datiModifica.note = this.txt_note.Text;
            datiModifica.fax = this.txt_Fax.Text;
            datiModifica.localita = this.txt_local.Text;
            datiModifica.idCanalePref = dd_canpref.SelectedItem.Value;
            datiModifica.luogoNascita = string.Empty;
            datiModifica.dataNascita = string.Empty;
            datiModifica.titolo = string.Empty;
        }

        //reperimento dei dati per la modifica di un ruolo
        private void modifyRuolo(ref SAAdminTool.DocsPaWR.DatiModificaCorr datiModifica)
        {
            datiModifica.idCorrGlobali = this.hd_systemId.Value;
            datiModifica.descCorr = this.txt_descrizione.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codiceAoo = this.txt_CodAOO.Text;
            datiModifica.codiceAmm = this.txt_CodAmm.Text;
            datiModifica.email = this.txt_EmailAOO.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codice = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.nome = String.Empty;
            datiModifica.cognome = String.Empty;
            datiModifica.indirizzo = String.Empty;
            datiModifica.cap = String.Empty;
            datiModifica.provincia = String.Empty;
            datiModifica.nazione = String.Empty;
            datiModifica.citta = String.Empty;
            datiModifica.codFiscale = String.Empty;
            datiModifica.partitaIva = String.Empty;
            datiModifica.telefono = String.Empty;
            datiModifica.telefono2 = String.Empty;
            datiModifica.note = String.Empty;
            datiModifica.fax = String.Empty;
            datiModifica.idCanalePref = dd_canpref.SelectedItem.Value;
            datiModifica.dataNascita = String.Empty;
            datiModifica.luogoNascita = String.Empty;
            datiModifica.titolo = String.Empty;
        }

        //reperimento dei dati per la modifica di un utente
        private void modifyUtente(ref SAAdminTool.DocsPaWR.DatiModificaCorr datiModifica)
        {
            datiModifica.idCorrGlobali = this.hd_systemId.Value;
            datiModifica.codiceAoo = this.txt_CodAOO.Text;
            datiModifica.codiceAmm = this.txt_CodAmm.Text;
            datiModifica.email = this.txt_EmailAOO.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codice = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.nome = this.txt_nome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.cognome = this.txt_cognome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.titolo = dd_titolo.SelectedItem.Value;
            if (!string.IsNullOrEmpty(datiModifica.titolo))
                datiModifica.descCorr = datiModifica.titolo + " " + datiModifica.cognome + " " + datiModifica.nome;
            else
                datiModifica.descCorr = datiModifica.cognome + " " + datiModifica.nome;
            datiModifica.indirizzo = this.txt_Indirizzo.Text;
            datiModifica.cap = this.txt_CAP.Text;
            datiModifica.provincia = this.txt_Prov.Text;
            datiModifica.nazione = this.txt_Nazione.Text;
            datiModifica.citta = this.txt_Citta.Text;
            datiModifica.codFiscale = this.txt_CodFis.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.partitaIva = this.txt_PI.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.telefono = this.txt_Tel1.Text;
            datiModifica.telefono2 = this.txt_Tel2.Text;
            datiModifica.note = this.txt_note.Text;
            datiModifica.fax = this.txt_Fax.Text;
            datiModifica.localita = this.txt_local.Text;
            datiModifica.idCanalePref = dd_canpref.SelectedItem.Value;
            datiModifica.luogoNascita = this.txt_luogoNascita.Text;
            datiModifica.dataNascita = this.txt_dataNascita.Text;
            datiModifica.tipoCorrispondente = "P";
        }

        private bool checkFields()
        {
            if ((this.txt_Tel1 == null || this.txt_Tel1.Text.Equals("")) && !(this.txt_Tel2 == null || this.txt_Tel2.Text.Equals("")))
            {
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alertTelefono", "window.alert(\"Attenzione, inserire il campo telefono princ.\");", true);
                Response.Write("<script language='javascript'>window.alert(\"Attenzione, inserire il campo telefono princ.\");</script>");
                return false;
            }
            else if (this.txt_CAP != null && !this.txt_CAP.Text.Equals("") && this.txt_CAP.Text.Length != 5)
            {
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alertTelefono", "window.alert(\"Attenzione, inserire il campo telefono princ.\");", true);
                Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo CAP deve essere di 5 cifre\");</script>");
                return false;
            }
            else if (this.txt_CAP != null && !this.txt_CAP.Text.Equals("") && !SAAdminTool.Utils.isNumeric(this.txt_CAP.Text))
            {
                Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo CAP deve un numero\");</script>");
                return false;
            }
            else if (this.txt_Prov != null && !this.txt_Prov.Text.Equals("") && this.txt_Prov.Text.Length != 2)
            {
                Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo provincia deve essere di 2 cifre\");</script>");
                return false;
            }
            else if (this.txt_Prov != null && !this.txt_Prov.Text.Equals("") && !SAAdminTool.Utils.isCorrectProv(this.txt_Prov.Text))
            {
                Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo provincia non è corretto\");</script>");
                return false;
            }
            else if (this.txt_CodFis != null && !this.txt_CodFis.Text.Equals("") && SAAdminTool.Utils.CheckTaxCode(this.txt_CodFis.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0)
            {
                Response.Write("<script language='javascript'>window.alert(\"Attenzione, Codice Fiscale non corretto!\");</script>");
                return false;
            }
            else if (this.txt_PI != null && !this.txt_PI.Text.Equals("") && SAAdminTool.Utils.CheckVatNumber(this.txt_PI.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0)
            {
                Response.Write("<script language='javascript'>window.alert(\"Attenzione, Partita Iva non corretta!\");</script>");
                return false;
            }

            return true;
        }

        #region eventi per la gestione della modifica del corrispondente


        private void DataChangedHandler(object sender, System.EventArgs e)
        {
            this.ViewState["modificato"] = true;
        }

        private void dd_canpref_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            string IdCanalePref = this.hd_canalePref.Value;

            setVisibilityPanelStar();

            //SE L'UTENTE STA MODIFICANDO IL CANALE PREFERENZIALE DEL CORRISPONDENTE
            if (IdCanalePref != dd_canpref.SelectedItem.Value)
            {
                this.ViewState["modificatoCanale"] = true;
            }
            else
            {
                this.ViewState["modificatoCanale"] = false;
            }
        }

        private void dd_titolo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.ViewState["modificato"] = true;

        }

        private void setVisibilityPanelStar()
        {
            this.starCodAOO.Visible = false;
            this.starEmail.Visible = false;
            this.starCodAmm.Visible = false;

            switch (dd_canpref.SelectedItem.Text)
            {
                case "MAIL":
                    this.starEmail.Visible = true;
                    break;

                case "INTEROPERABILITA":
                    this.starEmail.Visible = true;
                    this.starCodAOO.Visible = true;
                    this.starCodAmm.Visible = true;
                    break;
            }

        }

        #endregion

        private void btn_elimina_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

            DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            //((UpdatePanel) FindControl("upd_dett_corr")).Dispose();


            string app = hd_idReg.Value.ToString();
            if (app != "")
            {
                r = ws.GetRegistroBySistemId(app);
                if ((r.chaRF == "0") && !UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_REG"))
                {
                    string msg = "Attenzione, utente o ruolo non abilitati ad eliminare i corrispondenti presenti in questo registro";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + msg + "');self.close();", true);
                    //Response.Write("<script language='javascript'>window.alert('" + msg + "');</script>");
                    //Response.Write("<script language=\"javascript\">self.close();</script>");
                    //this.btn_modifica.Enabled = true;
                    //SetFocusOnPanel();
                    return;
                }
                else if ((r.chaRF == "1") && !UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_RF"))
                {
                    string msg = "Attenzione, utente o ruolo non abilitati ad eliminare i corrispondenti presenti in questo registro";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + msg + "');self.close();", true);
                    //Response.Write("<script language='javascript'>window.alert('" + msg + "');</script>");
                    //Response.Write("<script language=\"javascript\">self.close();</script>");
                    //this.btn_modifica.Enabled = true;
                    //SetFocusOnPanel();
                    return;
                }
            }
            else if (!UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_TUTTI"))
            {
                string msg = "Attenzione, utente o ruolo non abilitati ad eliminare i corrispondenti presenti in questo registro";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + msg + "');self.close();", true);
                //Response.Write("<script language='javascript'>window.alert('" + msg + "');</script>");
                //Response.Write("<script language=\"javascript\">self.close();</script>");
                //this.btn_modifica.Enabled = true;
                //SetFocusOnPanel();
                return;
            }

            try
            {
                string confirm = this.hd_confirmDelCorr.Value;


                if (confirm != null && !confirm.Equals("") && confirm.Equals("Yes"))
                {

                    //prendo la system_id del corrispondente da eliminare
                    string idCorrGlobali = this.hd_systemId.Value;

                    //popolo l'oggetto DatiModificaCorr necessario all'esecuzione della procedura 
                    datiModifica.idCorrGlobali = idCorrGlobali;

                    switch (listeDistr)
                    {
                        case "1": // Liste di distribuzione abilitate
                            flagListe = 1;
                            break;
                        case "0": // Liste di distribuzione disabilitate							
                            flagListe = 0;
                            break;
                    }

                    //operazione andata a buon fine
                    if (UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "D", out message))
                    {
                        switch (message)
                        {
                            case "OK":
                                message = "Cancellazione avvenuta con successo";
                                break;
                            default:
                                message = "Attenzione: si è verificato un errore nella procedura di cancellazione";
                                break;
                        }
                        if (message != null && message != "")
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + message + "');", true);
                            //Response.Write("<SCRIPT>alert('" + message + "');</SCRIPT>");
                        }
                        //	Response.Write ("<script language=\"javascript\">window.returnValue='REFRESH'; window.close();</script>");
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ReturnVal", "window.returnValue='DELETE'; window.close();", true);

                        //Response.Write ("<script language=\"javascript\">window.returnValue='DELETE'; window.close();</script>");
                        //RegisterStartupScript("ReturnVal", "<script language=\"javascript\">window.returnValue='REFRESH'; window.close();</script>");
                    }//operazione non è andata a buon fine
                    else
                    {
                        switch (message)
                        {
                            case "ERROR":
                                message = "Attenzione: si è verificato un errore nella procedura di cancellazione";
                                break;
                            case "NOTOK":
                                SAAdminTool.DocsPaWR.DocsPaWebService wws = new SAAdminTool.DocsPaWR.DocsPaWebService();
                                DataSet dsListe = wws.isCorrInListaDistr(idCorrGlobali);
                                message = "Attenzione: il corrispondente non può essere rimosso poiché utilizzato nelle liste di distribuzione";
                                if (dsListe != null)
                                {
                                    if (dsListe.Tables.Count > 0)
                                    {
                                        DataTable tab = dsListe.Tables[0];
                                        if (tab.Rows.Count > 0)
                                        {
                                            message = "Attenzione, utente presente nelle seguenti liste di distribuzione\\n";
                                            for (int i = 0; i < tab.Rows.Count; i++)
                                            {
                                                message += tab.Rows[i]["var_desc_corr"].ToString();
                                                if (!string.IsNullOrEmpty(tab.Rows[i]["prop"].ToString()))
                                                    message += " creata da " + tab.Rows[i]["prop"].ToString();
                                                else
                                                    if (!string.IsNullOrEmpty(tab.Rows[i]["ruolo"].ToString()))
                                                        message += " creata per il ruolo " + tab.Rows[i]["ruolo"].ToString();
                                                message += "\\n";
                                            }
                                        }
                                    }
                                }
                                break;
                            default:
                                message = "Attenzione: si è verificato un errore nella procedura di cancellazione";
                                break;
                        }
                        if (message != null && message != "")
                            //RegisterStartupScript("Message", "<SCRIPT>alert('" + message + "');</SCRIPT>");
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + message + "');", true);
                        //Response.Write("<SCRIPT>alert('" + message + "');</SCRIPT>");
                    }
                }
                else
                {
                    //disabilito il pulsante modifica
                    this.btn_modifica.Enabled = false;
                    apriDettaglio();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex, "eliminazione di un corrispondente");
            }
        }

        private void btn_mod_corr_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            /*se stiamo tentando di creare un nuovo corrispondente(maschera K2), devo verificare se il ruolo è abilitato a creare un corrispondente
            * con registro uguale a quello del corr selezionato nell'elenco
            * */

            /*Emanuela: commento l'if di seguito perchè anche in k1, nel caso in cui l'utente sia abilitato all'inserimento
            di utente occasionale la maschera si apre, devo verificare se il ruolo è abilitato a creare un corrispondente con registro uguale
             * a quello del corr selezionato nell'elenco
             */
            //if (btn_nuovo != null && btn_nuovo.Equals("btn_nuovo") && (!string.IsNullOrEmpty(avviso) && avviso.Equals("2")))
            //{

            //Emanuela: se sono nella maschera k1 o k2 applico il seguente codice
            if (!string.IsNullOrEmpty(avviso))
            {
                if (!string.IsNullOrEmpty(hd_idReg.Value.ToString()))
                {
                    DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
                    Registro reg = ws.GetRegistroBySistemId(hd_idReg.Value.ToString());
                    if ((reg.chaRF == "0") && !UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_REG"))
                    {
                        string msg = "Attenzione, utente o ruolo non abilitati a creare corrispondenti su questo registro";
                        //string msg = "Attenzione, utente o il ruolo non abilitati a modificare i corrispondenti presenti in questo registro";
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + msg + "');", true);
                        return;
                    }
                    else if ((reg.chaRF == "1") && !UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_RF"))
                    {
                        string msg = "Attenzione, utente o ruolo non abilitati a creare corrispondenti su questo RF";
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + msg + "');", true);
                        return;
                    }
                }
                else if (!UserManager.ruoloIsAutorized(this, "DO_MOD_CORR_TUTTI"))
                {
                    string msg = "Attenzione, utente o ruolo non abilitati a creare corrispondenti su questo registro";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + msg + "');", true);
                    return;
                }
            }
            //}
            String val = this.dd_canpref.SelectedValue;
            this.dd_canpref.Items.Clear();
            this.LoadCanali(false);
            this.dd_canpref.SelectedValue = val;


            //all'apertura della pagina di dettaglio, tutti i campi sono in sola lettura.
            //Solamente al click sull'icona matita i campi vengono resi editabili
            setcampiModify();
            ((Button)FindControl("btn_crea_occasionale_protocolla")).Enabled = false;
            ((Button)FindControl("btn_protocolla")).Enabled = true;
            this.btn_modifica.Enabled = true;
            if (((HiddenField)FindControl("hd_field")).Value == "" && !string.IsNullOrEmpty(btn_nuovo))
            {
                resettaCampi();
                ((Button)FindControl("btn_protocolla")).Text = @"Crea e Protocolla";
            }

            SetFocusOnPanel();
            //se siamo in K1 - K2
            if (Request.QueryString["k"] != null && Request.QueryString["k"].Equals("1"))
            {
                if (Session["Interop"] != null)
                {
                    if (!Session["Interop"].ToString().Equals("S"))
                    {
                        ((DropDownList)FindControl("ddl_tipoCorr")).Enabled = true;
                        ((DropDownList)FindControl("ddl_tipoCorr")).Visible = true;
                        pnlStandard.Visible = true;
                        pnlRuolo.Visible = false;
                        set_mode(((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue);
                    }
                    else
                    {
                        ((DropDownList)FindControl("ddl_tipoCorr")).Enabled = false;
                        ((DropDownList)FindControl("ddl_tipoCorr")).Visible = true;
                    }
                }
            }
            else // se la masachera non è invocata a seguito dei controlli K1 - K2
            {
                ((DropDownList)FindControl("ddl_tipoCorr")).Enabled = false;
                ((DropDownList)FindControl("ddl_tipoCorr")).Visible = true;
                pnlStandard.Visible = true;
                pnlRuolo.Visible = false;
                set_mode(((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue);
            }

            if (((DropDownList)FindControl("ddl_tipoCorr")) != null)
            {
                if (((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue.Equals("U"))
                {
                    hd_tipo_corr.Value = "U";
                }
                else if (((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue.Equals("R"))
                {
                    hd_tipo_corr.Value = "R";
                }
                else if (((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue.Equals("P"))
                {
                    hd_tipo_corr.Value = "P";
                }
            }
        }

        private void SetFocusOnPanel()
        {
            if (this.pnlRuolo.Visible)
            {
                SetFocus(txt_DescR);
            }
            else
            {
                if (this.pnlStandard.Visible)
                {
                    if (this.pnl_nome_cogn.Visible)
                    {
                        SetFocus(txt_cognome);
                    }
                    else
                    {
                        SetFocus(txt_descrizione);
                    }
                }
            }
        }

        private void SetFocus(System.Web.UI.Control ctrl)
        {
            string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
            RegisterStartupScript("focus", s);
        }

        private void dettagli_PreRender(object sender, EventArgs e)
        {
            UserManager.disabilitaFunzNonAutorizzate(this);

            if (((HiddenField)FindControl("hd_field")).Value.Equals(""))
            {
                this.btn_mod_corr.Enabled = !this.InRubricaComune;

                if (!UserManager.ruoloIsAutorized(this, "GEST_RUBRICA"))
                {
                    this.btn_mod_corr.Enabled = false;
                    btn_mod_corr.Visible = false;
                }

                //apriDettaglio(((HiddenField)FindControl("hd_field")).Value);
            }
            else
            {
                this.btn_mod_corr.Enabled = false;
                this.btn_mod_corr.Visible = false;
                this.pnl_modifica.Visible = false;
                this.pnl_modifica.Enabled = false;
            }

            this.btn_modifica.Enabled = !this.InRubricaComune;
            this.btn_elimina.Enabled = !this.InRubricaComune;

            if (this.btn_elimina.Enabled)
                this.btn_elimina.Attributes.Add("OnClick", "ApriconfirmDelCorr();");


            if (paginaChiamante == "ElencoCorrDaUC")
            {
                this.btn_mod_corr.Enabled = false;
                this.btn_modifica.Enabled = false;
                this.btn_elimina.Enabled = false;
            }

            string evento = Request.Params.Get("__EVENTARGUMENT");
            if (evento == "resettaCampi")
            {
                if (UserManager.ruoloIsAutorized(this, "GEST_RUBRICA"))
                {
                    this.btn_mod_corr.Enabled = true;
                    this.btn_mod_corr.Visible = true;
                }
                this.pnl_modifica.Visible = true;
                this.pnl_modifica.Enabled = true;
                ((Button)FindControl("btn_protocolla")).Enabled = false;
                if (Session["Interop"] != null)
                {
                    ((DropDownList)FindControl("ddl_tipoCorr")).Enabled = false;
                    ((DropDownList)FindControl("ddl_tipoCorr")).Visible = true;
                }
            }
        }

        /// <summary>
        /// Carica i canali preferenziali nella combo-box
        /// </summary>
        private void LoadCanali(bool showIs)
        {
            // combo-box dei canali preferenziali
            this.dd_canpref.Items.Add("");
            ArrayList listaMezzoSpedizione = new ArrayList();
            DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            string idAmm = UserManager.getInfoUtente().idAmministrazione;
            SAAdminTool.DocsPaWR.MezzoSpedizione[] m_sped = ws.AmmListaMezzoSpedizione(idAmm, false);
            if (m_sped != null && m_sped.Length > 0)
            {
                foreach (SAAdminTool.DocsPaWR.MezzoSpedizione m_spediz in m_sped)
                {
                    if (!showIs)
                    {
                        if (!m_spediz.chaTipoCanale.ToUpper().Equals("S"))
                        {
                            ListItem item = new ListItem(m_spediz.Descrizione, m_spediz.IDSystem);
                            this.dd_canpref.Items.Add(item);
                        }
                    }
                    else
                    {
                        ListItem item = new ListItem(m_spediz.Descrizione, m_spediz.IDSystem);
                        this.dd_canpref.Items.Add(item);
                    }

                }
            }
        }


        /// <summary>
        /// Carica i titoli nella combo-box
        /// </summary>
        private void LoadTitoli()
        {

            //aggiunge una riga vuota alla combo
            dd_titolo.Items.Add("");
            DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            string[] listaTitoli = ws.GetListaTitoli();
            foreach (string tit in listaTitoli)
            {
                dd_titolo.Items.Add(tit);
            }
        }

        private bool CheckGoodString(string cod)
        {
            if (cod == null || cod.Trim() == "")
                return false;

            Regex rx = new Regex(@"^[0-9A-Za-z_\ \.\-]+$");
            return rx.IsMatch(cod);
        }

        /// <summary>
        /// Se true, indica che l'elemento visualizzato proviene dalla rubrica comune
        /// </summary>
        protected bool InRubricaComune
        {
            get
            {
                int result;
                return Int32.TryParse(this.Request.QueryString["rc"], out result);
            }
        }

        /// <summary>
        /// Controlla e segnala eventuali modifiche al valore del campo data di nascita
        /// </summary>
        private void txt_dataNascita_TextChanged(object sender, System.EventArgs e)
        {
            this.ViewState["modificato"] = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        private SAAdminTool.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (SAAdminTool.UserControls.Calendar)this.FindControl(controlId);
        }

        //Controlla modifiche sul Codice Rubrica
        private void txt_CodRubrica_TextChanged(object sender, EventArgs e)
        {
            this.ViewState["modificato"] = true;
        }

        private void btn_VisDoc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //Verifico se ho visibilità sul documento
            //Se ho visibilità apro la pagina di visualizzazione del documento, altrimenti messaggio
            Session.Add("protocolloEsistente", true);

            try
            {
                // Viene impostato l'id documento e viene verificato se si possiedono i diritti
                // di visibilità sul documento
                string errorMessage = string.Empty;
                if (!string.IsNullOrEmpty(idDoc))
                {
                    int diritti = DocumentManager.verificaACL("D", idDoc,
                                                              UserManager.getInfoUtente(this), out errorMessage);

                    // Se si possiedono i diritti...
                    if (!(diritti == 0))
                    {
                        // ...viene caricata la scheda del documento e viene i 
                        // impostato come documento selezionato
                        SAAdminTool.DocsPaWR.SchedaDocumento schedaSel = DocumentManager.getDettaglioDocumentoNoDataVista(
                            this, null, idDoc);

                        if (schedaSel != null)
                        {
                            DocumentManager.setDocumentoSelezionato(this, schedaSel);
                            FileManager.setSelectedFile(this, schedaSel.documenti[0], false);
                            //ClientScript.RegisterStartupScript(this.GetType(), "lanciaVIs",
                            //                                   "loadVisualizzatoreDocModalFromInterop('" + Session.SessionID +
                            //                                   "','" + "" + "','" + idDoc + "');", true);
                            ScriptManager.RegisterStartupScript(Page, typeof(Page), "lanciaVIs",
                                                                "loadVisualizzatoreDocModalFromInterop('" +
                                                                Session.SessionID +
                                                                "','" + "" + "','" + idDoc + "');", true);
                        }
                    }
                    else
                        // ...altrimenti viene visualizzato un messaggio di avviso
                        //ClientScript.RegisterStartupScript(this.GetType(), "avviso",
                        //                                   "alert(\"Non si possiedono i diritti per la visualizzazione di questo documento\");",
                        //                                   true);
                        ScriptManager.RegisterStartupScript(Page, typeof(Page), "avviso",
                                                            "alert(\"Non si possiedono i diritti per la visualizzazione di questo documento\");",
                                                            true);
                }

            }
            catch (Exception ex)
            {
                Session["ErrorManager.error"] = ex.ToString() + "\n" + ex.StackTrace;
                ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        protected void BtnSaveCorr_Click(object sender, EventArgs e)
        {
            string idCorr = Request.Form["rbl_pref"];
            bool standard = false;
            DocsPaWR.Corrispondente[] listaDest;

            if (!string.IsNullOrEmpty(idCorr))
            {
                DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoSelezionato();
                DocsPaWR.Corrispondente tempCorr = UserManager.getCorrispondenteBySystemID(this.Page, idCorr);
                if (tempCorr == null)
                {
                    tempCorr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, idCorr);
                }

                if (schedaDoc != null && schedaDoc.protocollo != null && !string.IsNullOrEmpty(schedaDoc.tipoProto))
                {
                    if (schedaDoc.tipoProto.Equals("A"))
                    {
                        if (((SAAdminTool.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti != null &&
                            ((SAAdminTool.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti.Length > 0 &&
                            UserManager.esisteCorrispondente(
                                ((SAAdminTool.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti, tempCorr))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi",
                                                                "alert('Attenzione! Corrispondente già presente nei mittenti multipli');",
                                                                true);
                        }
                        else
                        {
                            ((SAAdminTool.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittente = tempCorr;
                            ((SAAdminTool.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).daAggiornareMittente = true;
                        }
                    }

                    if (schedaDoc.tipoProto.Equals("P"))
                    {
                        if (tipo.Equals("M"))
                        {
                            ((SAAdminTool.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).mittente = tempCorr;
                            ((SAAdminTool.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).daAggiornareMittente = true;
                        }
                        else
                        {
                            if (((SAAdminTool.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari != null &&
                                ((SAAdminTool.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari.Length > 0 &&
                                UserManager.esisteCorrispondente(
                                    ((SAAdminTool.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari, tempCorr))
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi",
                                                                    "alert('Attenzione! Corrispondente già presente nei destinatari');",
                                                                    true);
                            }
                            else
                            {
                                if (((SAAdminTool.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza !=
                                    null &&
                                    ((SAAdminTool.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.
                                        Length > 0 &&
                                    UserManager.esisteCorrispondente(
                                        ((SAAdminTool.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).
                                            destinatariConoscenza, tempCorr))
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi",
                                                                        "alert('Attenzione! Corrispondente già presente nei destinatari in conoscenza');",
                                                                        true);
                                }
                                else
                                {
                                    listaDest = ((SAAdminTool.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari;
                                    ((SAAdminTool.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari =
                                        UserManager.addCorrispondente(listaDest, tempCorr);
                                }
                            }
                        }
                    }

                    if (schedaDoc.tipoProto.Equals("I"))
                    {
                        if (tipo.Equals("M"))
                        {
                            ((SAAdminTool.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).mittente = tempCorr;
                            ((SAAdminTool.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).daAggiornareMittente = true;
                        }
                        else
                        {
                            if (((SAAdminTool.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari != null &&
                                ((SAAdminTool.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari.Length > 0 &&
                                UserManager.esisteCorrispondente(
                                    ((SAAdminTool.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari, tempCorr))
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi",
                                                                    "alert('Attenzione! Corrispondente già presente nei destinatari');",
                                                                    true);
                            }
                            else
                            {
                                if (
                                    ((SAAdminTool.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza !=
                                    null &&
                                    ((SAAdminTool.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza.
                                        Length > 0 &&
                                    UserManager.esisteCorrispondente(
                                        ((SAAdminTool.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).
                                            destinatariConoscenza, tempCorr))
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi",
                                                                        "alert('Attenzione! Corrispondente già presente nei destinatari in conoscenza');",
                                                                        true);
                                }
                                else
                                {
                                    listaDest = ((SAAdminTool.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari;
                                    ((SAAdminTool.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari =
                                        UserManager.addCorrispondente(listaDest, tempCorr);
                                }
                            }
                        }
                    }
                }


                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "window.close();", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert",
                                                    "alert('Attenzione! Selezionare un corrispondente');", true);
            }
        }

        //protected void GetTheme()
        //{
        //    string Tema = string.Empty;
        //    string idAmm = string.Empty;
        //    if ((string)Session["AMMDATASET"] != null)
        //        idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
        //    else
        //    {
        //        if (UserManager.getInfoUtente() != null)
        //            idAmm = UserManager.getInfoUtente().idAmministrazione;
        //    }

        //    UserManager userM = new UserManager();
        //    Tema = userM.getCssAmministrazione(idAmm);

        //    if (!string.IsNullOrEmpty(Tema))
        //    {
        //        string[] colorsplit = Tema.Split('^');
        //        System.Drawing.ColorConverter colConvert = new ColorConverter();
        //        this.titlePage.ForeColor = System.Drawing.Color.White;
        //        this.titlePage.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#" + colorsplit[2]);

        //    }
        //    else
        //    {
        //        System.Drawing.ColorConverter colConvert = new ColorConverter();
        //        this.titlePage.ForeColor = System.Drawing.Color.White;
        //        this.titlePage.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#810d06");
        //    }

        //}

        protected String GetCorrName(DocsPaWR.Corrispondente elem)
        {
            return elem.descrizione;
        }

        protected String GetCorrReg(DocsPaWR.Corrispondente elem)
        {
            return !string.IsNullOrEmpty(elem.idRegistro) ? elem.idRegistro : string.Empty;
        }

        protected String GetCorrCodice(DocsPaWR.Corrispondente elem)
        {
            return elem.codiceRubrica;
        }

        protected String GetCorrID(DocsPaWR.Corrispondente elem)
        {

            return elem.systemId;


        }

        protected String GetCorrmail(DocsPaWR.Corrispondente elem)
        {
            return elem.email;
        }


        private List<SAAdminTool.DocsPaWR.Corrispondente> ListCorr
        {
            get { return list; }
            set
            {
                list = value;
            }
        }

        protected void GetInitData()
        {
            if (Session["ListCorrByMail"] != null)
            {
                this.ListCorr = (List<SAAdminTool.DocsPaWR.Corrispondente>)Session["ListCorrByMail"];

                ((DataGrid)FindControl("grvListaCorr")).DataSource = this.ListCorr;
                ((DataGrid)FindControl("grvListaCorr")).DataBind();
            }

        }

        private string tipo
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["tipo"] as string;
            }
            set
            {
                CallContextStack.CurrentContext.ContextState["tipo"] = value;
            }
        }

        public void hd_field_OnValueChanged(object sender, EventArgs e)
        {
            string sysidChosen = ((HiddenField)FindControl("hd_field")).Value;
            if (!string.IsNullOrEmpty(sysidChosen))
            {
                apriDettaglio(sysidChosen);
                ((UpdatePanel)FindControl("upd_dett_corr")).Update();
                ((Button)FindControl("btn_protocolla")).Enabled = true;
            }

        }

        private void set_mode(string select)
        {
            switch (select)
            {
                case "P":
                    pnl_infonascita.Visible = true;
                    pnl_nome_cogn.Visible = true;
                    txt_nome.ReadOnly = false;
                    txt_cognome.ReadOnly = false;
                    pnl_descrizione.Visible = false;
                    pnlRuolo.Visible = false;
                    pnl_titolo.Visible = true;
                    pnl_indirizzo.Visible = true;
                    break;
                case "U":
                    pnl_infonascita.Visible = false;
                    pnl_nome_cogn.Visible = false;
                    pnl_descrizione.Visible = true;
                    pnlRuolo.Visible = false;
                    pnl_titolo.Visible = false;
                    pnl_indirizzo.Visible = true;
                    break;
                case "R":
                    pnl_infonascita.Visible = false;
                    pnl_nome_cogn.Visible = false;
                    pnl_descrizione.Visible = true;
                    pnl_titolo.Visible = false;
                    pnl_indirizzo.Visible = false;
                    //pnlStandard.Visible = false;
                    break;

            }
        }

        public void ddl_tipoCorr_SelectedIndexChanged(object sender, EventArgs e)
        {
            set_mode(((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue);
            hd_tipo_corr.Value = ((DropDownList)FindControl("ddl_tipoCorr")).SelectedValue.ToString();
            ((DropDownList)sender).Enabled = true;
        }

        private void UpdButtonAfterResetRadio()
        {
            if (UserManager.ruoloIsAutorized(this, "GEST_RUBRICA"))
            {
                btn_mod_corr.Visible = true;
                btn_mod_corr.Enabled = true;
            }
            ((Button)this.FindControl("btn_protocolla")).Enabled = false;
        }

        #region Multi Casella Corrispondenti esterni
        protected bool TypeMailCorrEsterno(string typeMail)
        {
            return (typeMail.Equals("1")) ? true : false;
        }

        protected void BindGridViewCaselle(Corrispondente corr)
        {
            if (ViewState["gvCaselle"] == null)
                ViewState.Add("gvCaselle", SAAdminTool.utils.MultiCasellaManager.GetMailCorrispondenteEsterno(corr.systemId));
            gvCaselle.DataSource = (List<SAAdminTool.DocsPaWR.MailCorrispondente>)ViewState["gvCaselle"];
            gvCaselle.DataBind();
            //se è disabilitato il multicasella, dopo l'immissione di una casella disabilito il pulsante aggiungi.
            if (((List<SAAdminTool.DocsPaWR.MailCorrispondente>)ViewState["gvCaselle"]).Count > 0 && !utils.MultiCasellaManager.IsEnabledMultiMail(UserManager.getRuolo().idAmministrazione))
            {
                txtCasella.Enabled = false;
                txtNote.Enabled = false;
                imgAggiungiCasella.Enabled = false;
            }
            //(gvCaselle.Parent as HtmlGenericControl).Style.Remove("overflow-y");
        }

        protected void gvCaselle_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && ViewState["readOnlyMail"].ToString().Equals("1"))
            {
                (e.Row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = true;
                (e.Row.FindControl("txtNoteMailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = true;
                (e.Row.FindControl("rdbPrincipale") as System.Web.UI.WebControls.RadioButton).Enabled = false;
                (e.Row.FindControl("imgEliminaCasella") as System.Web.UI.WebControls.ImageButton).Enabled = false;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow && (!ViewState["readOnlyMail"].ToString().Equals("1")))
            {
                (e.Row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = false;
                (e.Row.FindControl("txtNoteMailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = false;
                (e.Row.FindControl("rdbPrincipale") as System.Web.UI.WebControls.RadioButton).Enabled = true;
                (e.Row.FindControl("imgEliminaCasella") as System.Web.UI.WebControls.ImageButton).Enabled = true;
            }
        }

        protected void EnableInsertMail()
        {
            if (ViewState["readOnlyMail"].ToString().Equals("1"))
            {
                txtCasella.Enabled = false;
                txtNote.Enabled = false;
                imgAggiungiCasella.Enabled = false;
            }
            else
            {
                txtCasella.Enabled = true;
                txtNote.Enabled = true;
                imgAggiungiCasella.Enabled = true;
            }
        }

        protected void imgEliminaCasella_Click(object sender, ImageClickEventArgs e)
        {
            bool isComboMain = (((sender as System.Web.UI.WebControls.ImageButton).Parent.Parent as System.Web.UI.WebControls.GridViewRow).
                                    FindControl("rdbPrincipale") as RadioButton).Checked;
            //se presenti più caselle e si tenta di eliminare una casella settata come principale il sistema avvisa l'utente
            if (isComboMain && (ViewState["gvCaselle"] as List<SAAdminTool.DocsPaWR.MailCorrispondente>).Count > 1)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningPrincipale", "<script>alert('Prima di eliminare una casella " +
                    "definita come principale è necessario impostare una nuova casella principale !');</script>", false);
                return;
            }
            int indexRowDelete = ((sender as System.Web.UI.WebControls.ImageButton).Parent.Parent as System.Web.UI.WebControls.GridViewRow).RowIndex;
            (ViewState["gvCaselle"] as List<SAAdminTool.DocsPaWR.MailCorrispondente>).RemoveAt(indexRowDelete);
            gvCaselle.DataSource = (List<SAAdminTool.DocsPaWR.MailCorrispondente>)ViewState["gvCaselle"];
            gvCaselle.DataBind();
            if (((List<SAAdminTool.DocsPaWR.MailCorrispondente>)ViewState["gvCaselle"]).Count < 1 && !utils.MultiCasellaManager.IsEnabledMultiMail(UserManager.getRuolo().idAmministrazione))
            {
                txtCasella.Enabled = true;
                txtNote.Enabled = true;
                imgAggiungiCasella.Enabled = true;
            }
            //(gvCaselle.Parent as HtmlGenericControl).Style.Remove("overflow-y");
            ViewState["modificatoCaselle"] = true;
        }

        protected void imgAggiungiCasella_Click(object sender, ImageClickEventArgs e)
        {
            //verifico che l'indirizzo non sia vuoto
            if (string.IsNullOrEmpty(txtCasella.Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningInserisciIndirizzo", "<script>alert('Immettere una casella di posta elettronica valida!');</script>", false);
                return;
            }

            //verifico il formato dell'indirizzo mail
            string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
            if (!System.Text.RegularExpressions.Regex.Match(txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), pattern).Success)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningInserisciIndirizzo", "<script>alert('Formato casella di posta non valido!');</script>", false);
                return;
            }

            //verifico che la casella non sia già stata associata al corrispondente       
            foreach (SAAdminTool.DocsPaWR.MailCorrispondente c in (List<SAAdminTool.DocsPaWR.MailCorrispondente>)ViewState["gvCaselle"])
            {
                if (c.Email.Trim().Equals(txtCasella.Text.Trim()))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningIndirizzoPresente", "<script>alert('La casella che si sta tentando di inserire è già presente!');</script>", false);
                    return;
                }
            }
            (ViewState["gvCaselle"] as List<SAAdminTool.DocsPaWR.MailCorrispondente>).Add(new SAAdminTool.DocsPaWR.MailCorrispondente()
            {
                Email = txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                Note = txtNote.Text,
                Principale = gvCaselle.Rows.Count < 1 ? "1" : "0"
            });
            if ((ViewState["gvCaselle"] as List<SAAdminTool.DocsPaWR.MailCorrispondente>).Count > 0)
            {
                txtCasella.Enabled = false;
                txtNote.Enabled = false;
                imgAggiungiCasella.Enabled = false;
            }
            //(gvCaselle.Parent as HtmlGenericControl).Style.Remove("overflow-y");
            //(gvCaselle.Parent as HtmlGenericControl).Style.Add("overflow-y", "scroll");
            gvCaselle.DataSource = (List<SAAdminTool.DocsPaWR.MailCorrispondente>)ViewState["gvCaselle"];
            gvCaselle.DataBind();
            ViewState["modificatoCaselle"] = true;

            //PULISCO I CAMPI EMAIL/NOTE EMAIL
            this.txtCasella.Text = string.Empty;
            this.txtNote.Text = string.Empty;
        }

        protected void rdbPrincipale_ChecekdChanged(object sender, EventArgs e)
        {
            string mailSelect = (((sender as RadioButton).Parent.Parent as System.Web.UI.WebControls.GridViewRow).FindControl("txtEmailCorr") as TextBox).Text;
            List<SAAdminTool.DocsPaWR.MailCorrispondente> listCaselle = ViewState["gvCaselle"] as List<SAAdminTool.DocsPaWR.MailCorrispondente>;
            foreach (SAAdminTool.DocsPaWR.MailCorrispondente c in listCaselle)
            {
                if (c.Email.Trim().Equals(mailSelect.Trim()))
                    c.Principale = "1";
                else
                    c.Principale = "0";
            }
            ViewState["gvCaselle"] = listCaselle as List<SAAdminTool.DocsPaWR.MailCorrispondente>;
            gvCaselle.DataSource = ViewState["gvCaselle"];
            gvCaselle.DataBind();
            ViewState["modificatoCaselle"] = true;
        }

        protected void txtEmailCorr_TextChanged(object sender, EventArgs e)
        {
            ViewState["modificatoCaselle"] = true;
            string newMail = (sender as System.Web.UI.WebControls.TextBox).Text;
            int rowModify = ((sender as System.Web.UI.WebControls.TextBox).Parent.Parent as System.Web.UI.WebControls.GridViewRow).RowIndex;
            (ViewState["gvCaselle"] as List<SAAdminTool.DocsPaWR.MailCorrispondente>)[rowModify].Email = newMail;
        }

        protected void txtNoteMailCorr_TextChanged(object sender, EventArgs e)
        {
            ViewState["modificatoCaselle"] = true;
            string newNote = (sender as System.Web.UI.WebControls.TextBox).Text;
            int rowModify = ((sender as System.Web.UI.WebControls.TextBox).Parent.Parent as System.Web.UI.WebControls.GridViewRow).RowIndex;
            (ViewState["gvCaselle"] as List<SAAdminTool.DocsPaWR.MailCorrispondente>)[rowModify].Note = newNote;
        }

        protected bool InsertComboMailsCorr(string idCorrispondente)
        {
            bool res = true;
            if (!string.IsNullOrEmpty(idCorrispondente))
            {
                //modifico eventualmente la lista delle caselle associate al corrispondente esterno
                if ((ViewState["gvCaselle"] as List<SAAdminTool.DocsPaWR.MailCorrispondente>).Count > 0)
                {
                    res = SAAdminTool.utils.MultiCasellaManager.InsertMailCorrispondenteEsterno(
                        (ViewState["gvCaselle"] as List<SAAdminTool.DocsPaWR.MailCorrispondente>), idCorrispondente);
                    if (!res)
                    {
                        message = "Attenzione: si è verificato un errore durante l'aggiornamento delle caselle mail associate al corrispondente";
                    }
                }
                if ((ViewState["gvCaselle"] as List<SAAdminTool.DocsPaWR.MailCorrispondente>).Count == 0)
                {
                    res = SAAdminTool.utils.MultiCasellaManager.DeleteMailCorrispondenteEsterno(idCorrispondente);
                    if (!res)
                    {
                        message = "Attenzione: si è verificato un errore durante l'aggiornamento delle caselle mail associate al corrispondente";
                    }
                }
            }
            return res;
        }
        #endregion
    }
}
