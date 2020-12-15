using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using SAAdminTool.DocsPaWR;
using System.Collections.Generic;
using System.Linq;
using SAAdminTool.utils;

namespace SAAdminTool.popup
{
    /// <summary>
    /// Summary description for Inserimento_Destinatari.
    /// </summary>
    public class Inserisci : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.Label Label1;
        protected SAAdminTool.DocsPaWR.Corrispondente newCorr;
        protected SAAdminTool.DocsPaWR.Corrispondente currCorr;
        protected System.Web.UI.WebControls.Button btn_Insert;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoCorr;
        protected System.Web.UI.WebControls.Label lbl_desc_nome;
        protected System.Web.UI.WebControls.Label lbl_email;
        protected System.Web.UI.WebControls.TextBox TextBox6;
        protected System.Web.UI.WebControls.Label lbl_indirizzo;
        protected System.Web.UI.WebControls.Label lbl_cap;
        protected System.Web.UI.WebControls.Label lbl_citta;
        protected System.Web.UI.WebControls.Label lbl_provincia;
        protected System.Web.UI.WebControls.Label lbl_localita;
        protected System.Web.UI.WebControls.Label lbl_nazione;
        protected System.Web.UI.WebControls.Label lbl_telefono2;
        protected System.Web.UI.WebControls.Label lbl_codfisc;
        protected System.Web.UI.WebControls.Label lbl_telefono1;
        protected System.Web.UI.WebControls.Label lbl_fax;
        protected System.Web.UI.WebControls.Label lbl_note;
        protected System.Web.UI.WebControls.Label lbl_amm;
        protected System.Web.UI.WebControls.Label lbl_aoo;
        protected System.Web.UI.WebControls.Panel pnlUO;
        protected SAAdminTool.DocsPaWR.Corrispondente parentCorr;
        protected System.Web.UI.WebControls.Label Label2;
        protected System.Web.UI.WebControls.Label Label3;
        protected System.Web.UI.WebControls.Label Label13;
        protected System.Web.UI.WebControls.Label Label14;
        protected System.Web.UI.WebControls.Label Label15;
        protected System.Web.UI.WebControls.Label Label16;
        protected System.Web.UI.WebControls.Label Label17;
        protected System.Web.UI.WebControls.Label Label18;
        protected System.Web.UI.WebControls.Label Label19;
        protected System.Web.UI.WebControls.Label Label20;
        protected System.Web.UI.WebControls.Label Label21;
        protected System.Web.UI.WebControls.Label Label22;
        protected System.Web.UI.WebControls.Label Label23;
        protected System.Web.UI.WebControls.Panel pnlRuolo;
        protected System.Web.UI.WebControls.Panel pnlUtente;
        protected System.Web.UI.WebControls.Label lbl_local;
        protected System.Web.UI.WebControls.Label Label4;
        protected System.Web.UI.WebControls.TextBox uo_descr;
        protected System.Web.UI.WebControls.TextBox uo_indirizzo;
        protected System.Web.UI.WebControls.TextBox uo_cap;
        protected System.Web.UI.WebControls.TextBox uo_citta;
        protected System.Web.UI.WebControls.TextBox uo_prov;
        protected System.Web.UI.WebControls.TextBox uo_local;
        protected System.Web.UI.WebControls.TextBox uo_nazione;
        protected System.Web.UI.WebControls.TextBox uo_telefono1;
        protected System.Web.UI.WebControls.TextBox uo_telefono2;
        protected System.Web.UI.WebControls.TextBox uo_fax;
        protected System.Web.UI.WebControls.TextBox uo_codfisc;
        protected System.Web.UI.WebControls.TextBox uo_partitaiva;
        protected System.Web.UI.WebControls.TextBox uo_note;
        protected System.Web.UI.WebControls.TextBox r_descr;
        protected System.Web.UI.WebControls.TextBox p_nome;
        protected System.Web.UI.WebControls.TextBox p_cognome;
        protected System.Web.UI.WebControls.TextBox p_indirizzo;
        protected System.Web.UI.WebControls.TextBox p_cap;
        protected System.Web.UI.WebControls.TextBox p_citta;
        protected System.Web.UI.WebControls.TextBox p_prov;
        protected System.Web.UI.WebControls.TextBox p_nazione;
        protected System.Web.UI.WebControls.TextBox p_local;
        protected System.Web.UI.WebControls.TextBox p_telefono1;
        protected System.Web.UI.WebControls.TextBox p_telefono2;
        protected System.Web.UI.WebControls.TextBox p_fax;
        protected System.Web.UI.WebControls.TextBox p_codfisc;
        protected System.Web.UI.WebControls.TextBox p_partitaiva;
        protected System.Web.UI.WebControls.TextBox p_note;
        protected System.Web.UI.WebControls.TextBox CodRubrica;
        protected System.Web.UI.WebControls.TextBox CodAmm;
        protected System.Web.UI.WebControls.TextBox CodAoo;
        //protected System.Web.UI.WebControls.TextBox EmailAoo;
        protected System.Web.UI.WebControls.TextBox p_luogoNascita;
        protected System.Web.UI.WebControls.TextBox p_dataNascita;

        private const string ModeUO = "U";
        private const string ModeRuolo = "R";
        protected System.Web.UI.WebControls.Button btn_chiudi;
        protected System.Web.UI.WebControls.Label lbl_canpref;
        protected System.Web.UI.WebControls.DropDownList dd_canpref;
        protected System.Web.UI.WebControls.Label star;
        protected System.Web.UI.WebControls.Label TipoCorr;
        protected System.Web.UI.WebControls.Button btn_Insert_disabled;
        protected System.Web.UI.WebControls.Label codRubr;
        protected System.Web.UI.WebControls.Label lbl_registro;
        protected System.Web.UI.WebControls.DropDownList ddl_registri;
        protected System.Web.UI.WebControls.Panel pnl_registri;
        private const string ModeUtente = "P";
        private string modify;
        protected DocsPaWR.Registro[] listaRegistri;
        protected System.Web.UI.WebControls.Label star_amm;
        protected System.Web.UI.WebControls.Label star_aoo;
        protected System.Web.UI.WebControls.DropDownList dd_titolo;
        protected System.Web.UI.WebControls.Panel pnl_titolo;
        //protected System.Web.UI.WebControls.Panel pnl_datinascita;

        //multicasella
        protected System.Web.UI.WebControls.GridView gvCaselle;
        protected System.Web.UI.UpdatePanel PanelMail;
        protected System.Web.UI.WebControls.TextBox txtCasella;
        protected System.Web.UI.WebControls.TextBox txtNote;
        protected System.Web.UI.WebControls.ImageButton imgAggiungiCasella;
        protected System.Web.UI.WebControls.RadioButton rdbPrincipale;
        protected System.Web.UI.WebControls.TextBox txtEmailCorr;
        protected System.Web.UI.WebControls.TextBox txtNoteMailCorr;

        private SAAdminTool.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (SAAdminTool.UserControls.Calendar)this.FindControl(controlId);
        }

        private void set_mode(string m)
        {
            switch (m)
            {
                case Inserisci.ModeUO:
                    pnlUO.Visible = true;
                    pnlRuolo.Visible = false;
                    pnlUtente.Visible = false;
                    break;

                case Inserisci.ModeRuolo:
                    pnlUO.Visible = false;
                    pnlRuolo.Visible = true;
                    pnlUtente.Visible = false;
                    break;

                case Inserisci.ModeUtente:
                    pnlUO.Visible = false;
                    pnlRuolo.Visible = false;
                    pnlUtente.Visible = true;
                    break;
            }
            LoadCanali(this.dd_canpref);
            LoadTitoli(this.dd_titolo);
        }

        /// <summary>
        /// Carica i canali preferenziali nella combo-box
        /// </summary>
        private void LoadCanali(DropDownList ddl)
        {
            ddl.Items.Clear();
            //aggiunge una riga vuota alla combo
            ddl.Items.Add("");
            ArrayList listaMezzoSpedizione = new ArrayList();
            DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
            string idAmm = UserManager.getInfoUtente().idAmministrazione;
            SAAdminTool.DocsPaWR.MezzoSpedizione[] m_sped = ws.AmmListaMezzoSpedizione(idAmm, false);

            // Non deve essere possibile creare un corrispondente con canale preferenziale impostato
            // a interoperabilità semplificata
            m_sped = m_sped.Where(ms => ms.chaTipoCanale != "S").ToArray();

            foreach (SAAdminTool.DocsPaWR.MezzoSpedizione m_spediz in m_sped)
            {
                ListItem li = new ListItem();
                li.Value = m_spediz.IDSystem;
                li.Text = m_spediz.Descrizione;
                ddl.Items.Add(li);
            }

        
            
            //// combo-box dei canali preferenziali
            //DocsPaWR.Canale[] canali = UserManager.getListaCanali(this);
            //if (canali != null && canali.Length > 0)
            //{
            //    dd_canpref.Items.Clear();
            //    for (int i = 0; i < canali.Length; i++)
            //    {
            //        ListItem item = new ListItem(canali[i].descrizione.ToUpper(), canali[i].systemId);
            //        this.dd_canpref.Items.Add(item);
            //    }
            //    dd_canpref.SelectedIndex = dd_canpref.Items.IndexOf(dd_canpref.Items.FindByText("LETTERA"));
            //    if (dd_canpref.SelectedItem.Text.Equals("MAIL"))
            //        this.star.Visible = true;
            //    else
            //        this.star.Visible = false;
            //}
        }

        /// <summary>
        /// Carica i Titoli nella combo-box
        /// </summary>
        private void LoadTitoli(DropDownList ddl)
        {
            ddl.Items.Clear();
            //aggiunge una riga vuota alla combo
            ddl.Items.Add("");
            DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
            string[] listaTitoli=ws.GetListaTitoli();
            foreach (string tit in listaTitoli)
            {
                ddl.Items.Add(tit);
            }
        }


        private void Page_Load(object sender, System.EventArgs e)
        {
            Response.Expires = -1;

            this.RegisterClientScript("nascondi", "nascondi();");
            
            if (!IsPostBack)//prim aci stava !
            {
                modify = this.Request.QueryString["modify"].ToString();
                CaricaComboRegistri(ddl_registri);
                set_mode(Inserisci.ModeUO);
                if (this.dd_canpref.SelectedIndex == 0)
                    this.star.Visible = false;
                BindGridViewCaselle();
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
            this.ddl_tipoCorr.SelectedIndexChanged += new System.EventHandler(this.ddl_tipoCorr_SelectedIndexChanged);
            this.dd_canpref.SelectedIndexChanged += new System.EventHandler(this.dd_canpref_SelectedIndexChanged);
            this.btn_Insert.Click += new System.EventHandler(this.btn_Insert_Click);
            this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
            this.Load += new System.EventHandler(this.Page_Load);          
        }

      
        #endregion

        private bool codice_rubrica_valido(string cod)
        {
            if (cod == null || cod.Trim() == "")
                return false;

            Regex rx = new Regex(@"^[0-9A-Za-z_\ \.\-]+$");
            return rx.IsMatch(cod);
        }

        private void SetFocus(System.Web.UI.Control ctrl)
        {
            string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus(); </SCRIPT>";
            RegisterStartupScript("focus", s);
        }

        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }
       
        private void btn_Insert_Click(object sender, System.EventArgs e)
        {
            string corr_type = ddl_tipoCorr.SelectedValue;

            int indxMail = dd_canpref.Items.IndexOf(dd_canpref.Items.FindByText("MAIL"));
            int indxInterop = dd_canpref.Items.IndexOf(dd_canpref.Items.FindByText("INTEROPERABILITA"));

            if (ddl_registri.SelectedValue.Equals("") && !UserManager.ruoloIsAutorized(this, "DO_INS_CORR_TUTTI"))
            {
                RegisterStartupScript("chk_fields",
                "<script language=\"javascript\">" +
                "alert (\"Attenzione, l'utente o il ruolo non sono abilitati ad inserire un nuovo corrispondente\");" +
                "</script>");
                return;
            }

            if (CodRubrica.Text == "" ||
                (corr_type == "U" && uo_descr.Text.Trim() == "") ||
                (corr_type == "R" && r_descr.Text.Trim() == "") ||
                (corr_type == "P" && (p_cognome.Text.Trim() == "" || p_nome.Text.Trim() == "")))
            {
                RegisterStartupScript("chk_fields",
                    "<script language=\"javascript\">" +
                    "alert (\"Attenzione, riempire i campi obbligatori prima di effettuare\\nl'inserimento di un nuovo corrispondente\");" +
                    "</script>");
                return;
            }
            else
            {
                if (ViewState["gvCaselle"] == null || (ViewState["gvCaselle"] as List<DocsPaWR.MailCorrispondente>).Count < 1)
                {
                    //verifico che l'indirizzo non sia vuoto
                    if (string.IsNullOrEmpty(txtCasella.Text))
                    {
                        if (dd_canpref.SelectedItem.Text.Equals("MAIL") || dd_canpref.SelectedItem.Text.Equals("INTEROPERABILITA"))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningInserisciIndirizzo", "<script>alert('Immettere una casella di posta elettronica valida!');</script>", false);
                            return;
                        }
                    }

                    //verifico il formato dell'indirizzo mail
                    string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                    if (!string.IsNullOrEmpty(txtCasella.Text) && !System.Text.RegularExpressions.Regex.Match(txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), pattern).Success)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningInserisciIndirizzo", "<script>alert('Formato casella di posta non valido!');</script>", false);
                        return;
                    }
                    if (!string.IsNullOrEmpty(txtCasella.Text))
                    {
                        (ViewState["gvCaselle"] as List<SAAdminTool.DocsPaWR.MailCorrispondente>).Add(new SAAdminTool.DocsPaWR.MailCorrispondente()
                        {
                            Email = txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                            Note = txtNote.Text,
                            Principale = "1"
                        });
                    }
                }

                if (!codice_rubrica_valido(CodRubrica.Text))
                {
                    RegisterStartupScript("chk_fields",
                        "<script language=\"javascript\">" +
                        "alert (\"Attenzione, il campo CODICE RUBRICA contiene caratteri non validi\");" +
                        "</script>");
                    return;
                }

                if (CodAmm.Text != "" && !codice_rubrica_valido(CodAmm.Text))
                {
                    RegisterStartupScript("chk_fields",
                        "<script language=\"javascript\">" +
                        "alert (\"Attenzione, il campo CODICE AMMINISTRAZIONE contiene\\ncaratteri non validi\");" +
                        "</script>");
                    return;
                }
                if (CodAoo.Text != "" && !codice_rubrica_valido(CodAoo.Text))
                {
                    RegisterStartupScript("chk_fields",
                        "<script language=\"javascript\">" +
                        "alert (\"Attenzione, il campo CODICE AOO contiene caratteri non validi\");" +
                        "</script>");
                    return;
                }

                //verifica del formato della data di nascita
                if (this.p_dataNascita.Text != string.Empty && !Utils.isDate(this.p_dataNascita.Text))
                {
                    Page.RegisterStartupScript("", "<script>alert('Attenzione, il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                    return;
                }

                if ((this.uo_cap != null && !this.uo_cap.Text.Equals("") && !Utils.isNumeric(uo_cap.Text))
                    || (this.p_cap != null && !this.p_cap.Text.Equals("") && !Utils.isNumeric(p_cap.Text)))
                {
                    RegisterStartupScript("chk_fields",
                        "<script language=\"javascript\">" +
                        "alert (\"Attenzione, il campo CAP deve essere numerico\");" +
                        "</script>");
                    return;
                }

                //Verifica della correttezza del codice fiscale
                if (corr_type.Equals("U"))
                {
                    if ((this.uo_codfisc != null && !this.uo_codfisc.Text.Trim().Equals("")) && ((this.uo_codfisc.Text.Trim().Length == 11 && Utils.CheckVatNumber(this.uo_codfisc.Text.Trim()) != 0) || (this.uo_codfisc.Text.Trim().Length == 16 && Utils.CheckTaxCode(this.uo_codfisc.Text.Trim()) != 0) || (this.uo_codfisc.Text.Trim().Length != 11 && this.uo_codfisc.Text.Trim().Length != 16)))
                    {
                        RegisterStartupScript("chk_fields",
                            "<script language=\"javascript\">" +
                            "alert (\"Attenzione, Codice Fiscale non corretto!\");" +
                            "</script>");
                        return;
                    }

                    //Verifica della correttezza della partita iva
                    if (this.uo_partitaiva != null && !this.uo_partitaiva.Text.Equals("") && Utils.CheckVatNumber(uo_partitaiva.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0)
                    {
                        RegisterStartupScript("chk_fields",
                            "<script language=\"javascript\">" +
                            "alert (\"Attenzione, Partita Iva non corretta!\");" +
                            "</script>");
                        return;
                    }
                }
                else
                {
                    if (this.p_codfisc != null && !this.p_codfisc.Text.Equals("") && Utils.CheckTaxCode(p_codfisc.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0)
                    {
                        RegisterStartupScript("chk_fields",
                            "<script language=\"javascript\">" +
                            "alert (\"Attenzione, Codice Fiscale non corretto!\");" +
                            "</script>");
                        return;
                    }


                    if (this.p_partitaiva != null && !this.p_partitaiva.Text.Equals("") && Utils.CheckVatNumber(p_partitaiva.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0)
                    {
                        RegisterStartupScript("chk_fields",
                            "<script language=\"javascript\">" +
                            "alert (\"Attenzione, Partita Iva non corretta!\");" +
                            "</script>");
                        return;
                    }
                }
                
                

                if ((this.uo_telefono1 == null || this.uo_telefono1.Text.Equals(""))
                    && !(this.uo_telefono2 == null || this.uo_telefono2.Text.Equals(""))
                    || (this.p_telefono1 == null || this.p_telefono1.Text.Equals(""))
                    && !(this.p_telefono2 == null || this.p_telefono2.Text.Equals("")))
                {
                    RegisterStartupScript("chk_fields",
                        "<script language=\"javascript\">" +
                        "alert (\"Attenzione, inserire il campo TELEFONO principale\");" +
                        "</script>");
                    return;
                }

                if (ddl_registri != null && ddl_registri.Items.Count >= 0)
                {
                    string message = string.Empty;
                    bool ok = true;
                    if (ddl_registri.SelectedValue.Equals(""))
                    {
                        //foreach (DocsPaWR.Registro reg in getListaRegistri())
                        //{
                        //    if (reg.Sospeso)
                        //    {
                        //        ok = false;
                        //        message += "Il registro " + reg.descrizione + " risulta essere sospeso\\n";
                        //    }
                        //}
                    }
                    else
                    {
                        string idRegistroCorrente = "";

                        if (ddl_registri.SelectedValue != string.Empty)
                        {
                            char[] sep = { '_' };
                            string[] datiSelezione = ddl_registri.SelectedValue.Split(sep);

                            idRegistroCorrente = datiSelezione[0];
                            DocsPaWR.Registro registro_corrente = UserManager.getRegistroBySistemId(this, idRegistroCorrente);
                            if (registro_corrente.Sospeso)
                            {
                                ok = false;
                                message += "Il registro " + registro_corrente.descrizione + " risulta essere sospeso";
                            }
                        }
                    }
                    if (!ok)
                    {
                        RegisterStartupScript("chk_fields", "<script language=\"javascript\">alert ('" + message + "');</script>");
                        return;
                    }
                }
            }

            DocsPaWR.InfoUtente iu = UserManager.getInfoUtente(this.Page);

            DocsPaWR.Registro reg_corr = null;

            //if ((reg_corr = UserManager.getRegistroSelezionato(this)) == null)
            //    reg_corr = UserManager.getRuolo(this).registri[0];
            string idRegistro = string.Empty;
            if (ddl_registri != null && ddl_registri.SelectedIndex >= 0)
            {
                

                if (ddl_registri.SelectedValue != string.Empty)
                {
                    char[] sep = { '_' };
                    string[] datiSelezione = ddl_registri.SelectedValue.Split(sep);

                    idRegistro = datiSelezione[0];
                    //reg_corr = UserManager.getRegistroBySistemId(this, idRegistro);
                }
            }

            DocsPaWR.Corrispondente corr = new SAAdminTool.DocsPaWR.Corrispondente();
            DocsPaWR.Corrispondente res = null;

            
            //creo l'oggetto canale
            DocsPaWR.Canale canale = new SAAdminTool.DocsPaWR.Canale();
            canale.systemId = this.dd_canpref.SelectedItem.Value;

            DocsPaVO.addressbook.DettagliCorrispondente dettagli = new DocsPaVO.addressbook.DettagliCorrispondente();
            switch (corr_type)
            {
                case "U":
                    DocsPaWR.UnitaOrganizzativa uo = new SAAdminTool.DocsPaWR.UnitaOrganizzativa();
                    uo.tipoCorrispondente = "U";
                    uo.info = new DocsPaVO.addressbook.DettagliCorrispondente();
                    uo.codiceCorrispondente = this.CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    uo.codiceRubrica = this.CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    uo.codiceAmm = this.CodAmm.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    uo.codiceAOO = this.CodAoo.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    uo.descrizione = this.uo_descr.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    uo.idAmministrazione = iu.idAmministrazione;
                    uo.localita = this.p_local.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    //new 27 marzo
                    uo.idRegistro = idRegistro; 
                    dettagli.Corrispondente.AddCorrispondenteRow(
                        uo_indirizzo.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), uo_citta.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), uo_cap.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                        uo_prov.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), uo_nazione.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), uo_telefono1.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                        uo_telefono2.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), uo_fax.Text, uo_codfisc.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                        uo_note.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), uo_local.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), string.Empty, string.Empty, string.Empty, uo_partitaiva.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()));

                    uo.info = dettagli;
                    uo.dettagli = true;
                    uo.canalePref = canale;
                    //mail
                    if (ViewState["gvCaselle"] != null && (ViewState["gvCaselle"] as List<MailCorrispondente>).Count > 0)
                    {
                        foreach (MailCorrispondente c in (ViewState["gvCaselle"] as List<MailCorrispondente>))
                        {
                            if (c.Principale.Equals("1"))
                            {
                                uo.email = c.Email.Trim();
                                break;
                            }
                        }
                    }
                    else
                    {
                        uo.email = string.Empty;
                    }
                    res = UserManager.addressbookInsertCorrispondente(this, uo, null);
                    if (res != null && (!string.IsNullOrEmpty(res.systemId)))
                    {
                        if (!SAAdminTool.utils.MultiCasellaManager.InsertMailCorrispondenteEsterno(
                                (ViewState["gvCaselle"] as List<MailCorrispondente>), res.systemId))
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "error", "<script language=\"javascript\">alert(\"Si è verificato un errore " +
                                    "durante il salvataggio delle caselle di posta associate al corrispondente!);</script>", false);
                    }
                    break;

                case "R":
                    res = new SAAdminTool.DocsPaWR.Corrispondente();
                    DocsPaWR.Ruolo ruolo = new SAAdminTool.DocsPaWR.Ruolo();
                    ruolo.tipoCorrispondente = "R";
                    ruolo.codiceCorrispondente = this.CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    ruolo.codiceRubrica = this.CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    ruolo.descrizione = this.r_descr.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());

                    //prendo il registro selezionato nella combo dei registri
                    //old
                    //ruolo.idRegistro = this.ddl_registri.SelectedItem.Value;
                    //					ruolo.idRegistro = reg_corr.systemId;

                    //prendo il registro selezionato nella combo dei registri
                   // ruolo.idRegistro = this.ddl_registri.SelectedItem.Value;

                    //new 27 marzo
                    ruolo.idRegistro = idRegistro;


                    //if (ruolo.idRegistro.Equals(""))
                    //{
                    //    ruolo.idRegistro = null;
                    //}
                    ruolo.codiceAmm = this.CodAmm.Text;
                    ruolo.codiceAOO = this.CodAoo.Text;
                    ruolo.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
                    DocsPaWR.UnitaOrganizzativa parent_uo = new UnitaOrganizzativa();
                    parent_uo.descrizione = "";
                    parent_uo.systemId = "0";
                    ruolo.canalePref = canale;
                    //mail
                    if (ViewState["gvCaselle"] != null && (ViewState["gvCaselle"] as List<MailCorrispondente>).Count > 0)
                    {
                        foreach (MailCorrispondente c in (ViewState["gvCaselle"] as List<MailCorrispondente>))
                        {
                            if (c.Principale.Equals("1"))
                            {
                                ruolo.email = c.Email.Trim();
                                break;
                            }
                        }
                    }
                    else
                    {
                        ruolo.email = string.Empty;
                    }
                    res = UserManager.addressbookInsertCorrispondente(this, ruolo, parent_uo);
                    if (res != null && (!string.IsNullOrEmpty(res.systemId)))
                    {
                        if (!SAAdminTool.utils.MultiCasellaManager.InsertMailCorrispondenteEsterno(
                                (ViewState["gvCaselle"] as List<MailCorrispondente>), res.systemId))
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "error", "<script language=\"javascript\">alert(\"Si è verificato un errore " +
                                    "durante il salvataggio delle caselle di posta associate al corrispondente!);</script>", false);
                    }
                    break;

                case "P":
                    res = new SAAdminTool.DocsPaWR.Corrispondente();
                    DocsPaWR.Utente utente = new SAAdminTool.DocsPaWR.Utente();
                    utente.codiceCorrispondente = this.CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    utente.codiceRubrica = this.CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    utente.cognome = this.p_cognome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    utente.nome = this.p_nome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    utente.codiceAmm = this.CodAmm.Text;
                    utente.codiceAOO = this.CodAoo.Text;
                    utente.descrizione = this.dd_titolo.Text + this.p_cognome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()) + this.p_nome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    utente.luogoDINascita = this.p_luogoNascita.Text;
                    utente.dataNascita = this.p_dataNascita.Text;
                    utente.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
                    utente.titolo=this.dd_titolo.Text;
                    //utente.idRegistro = UserManager.getRegistroSelezionato(this).systemId;//UserManager.getListaIdRegistriUtente(this)[0];
                    //old
                    //utente.idRegistro = this.ddl_registri.SelectedItem.Value;

                    //new 27 marzo
                    utente.idRegistro = idRegistro;


                    //if (utente.idRegistro.Equals(""))
                    //{
                    //    utente.idRegistro = null;
                    //}
                    utente.tipoCorrispondente = this.ddl_tipoCorr.SelectedItem.Value;
                    utente.canalePref = canale;
                    //mail
                    if (ViewState["gvCaselle"] != null && (ViewState["gvCaselle"] as List<MailCorrispondente>).Count > 0)
                    {
                        foreach (MailCorrispondente c in (ViewState["gvCaselle"] as List<MailCorrispondente>))
                        {
                            if (c.Principale.Equals("1"))
                            {
                                utente.email = c.Email.Trim();
                                break;
                            }
                        }
                    }
                    else
                    {
                        utente.email = string.Empty;
                    }
                    if ((p_indirizzo.Text != null && !p_indirizzo.Equals("")) ||
                            (p_cap.Text != null && !p_cap.Equals("")) ||
                            (p_citta.Text != null && !p_citta.Equals("")) ||
                            (p_prov.Text != null && !p_prov.Equals("")) ||
                            (p_nazione.Text != null && !p_nazione.Equals("")) ||
                            (p_telefono1.Text != null && !p_telefono1.Equals("")) ||
                            (p_telefono2 != null && !p_telefono2.Equals("")) ||
                            (p_fax.Text != null && !p_fax.Equals("")) ||
                            (p_codfisc.Text != null && !p_codfisc.Equals("")) ||
                            (p_note.Text != null && !p_note.Equals("")) ||
                             p_partitaiva.Text!=null && !p_partitaiva.Equals(""))
                    {
                        dettagli.Corrispondente.AddCorrispondenteRow(
                            p_indirizzo.Text, p_citta.Text, p_cap.Text,
                            p_prov.Text, p_nazione.Text, p_telefono1.Text,
                            p_telefono2.Text, p_fax.Text, p_codfisc.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                            p_note.Text, p_local.Text, p_luogoNascita.Text, p_dataNascita.Text, dd_titolo.Text, p_partitaiva.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()));

                        utente.info = dettagli;
                        utente.dettagli = true;
                    }
                    res = UserManager.addressbookInsertCorrispondente(this, utente, null);
                    if (res != null && (!string.IsNullOrEmpty(res.systemId)))
                    {
                        if (!SAAdminTool.utils.MultiCasellaManager.InsertMailCorrispondenteEsterno(
                                (ViewState["gvCaselle"] as List<MailCorrispondente>), res.systemId))
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "error", "<script language=\"javascript\">alert(\"Si è verificato un errore " +
                                    "durante il salvataggio delle caselle di posta associate al corrispondente!);</script>", false);
                    }
                    break;

            }

            if (res != null && res.errore == null)
            {
                string insertId = res.systemId;
                Response.Write("<script language=\"javascript\">window.returnValue='REFRESH?ID=" + insertId + "&INSERT'; window.close();</script>");

                RegisterStartupScript("close", "<script language=\"javascript\">self.close();</script>");
            }
            else
            {
                string strReturn = "Si è verificato un errore!";
                if (res != null && res.errore != null)
                    strReturn = String.Concat(strReturn, String.Format(" {0}", res.errore));
                RegisterStartupScript("error", "<script language=\"javascript\">alert(\"" + strReturn + "\");</script>");
            }
        }

        private DocsPaWR.Registro[] getListaRegistri()
        {
            DocsPaWR.Registro[] userRegistri = null;
            DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoInLavorazione(this);
            if (schedaDoc != null && (modify != null && modify != "" && modify.Equals("false")))// se creo un cprrispondente mentre protocollo
            {
                if (schedaDoc.registro != null)
                {
                    DocsPaWR.Registro[] protoReg = new SAAdminTool.DocsPaWR.Registro[1];
                    protoReg[0] = schedaDoc.registro;
                    //   userRegistri = protoReg;//prendo il registro della schedaDoc

                    DocsPaWR.Registro[] listaTotale = UserManager.getListaRegistriWithRF(this, "1", protoReg[0].systemId);
                    if (listaTotale != null && listaTotale.Length > 0)
                    {
                        userRegistri = new Registro[listaTotale.Length + 1];
                        userRegistri[0] = protoReg[0];//prendo il registro della schedaDoc
                        listaTotale.CopyTo(userRegistri, 1);
                        ddl_registri.Enabled = true;
                    }
                    else
                    {
                        userRegistri = new Registro[1];
                        userRegistri[0] = protoReg[0];//prendo il registro della schedaDoc
                        ddl_registri.Enabled = false;
                    }
                }
                else
                {
                    //prendo i registri visibili al ruolo dell'utente
                    ddl_registri.Enabled = true;
                    userRegistri = UserManager.getListaRegistriWithRF(this, "", "");
                }
            }
            else
            {
                //prendo i registri/RF visibili al ruolo dell'utente
                ddl_registri.Enabled = true;
                userRegistri = UserManager.getListaRegistriWithRF(this, "", "");
            }

            return userRegistri;
        }

        private void CaricaComboRegistri(DropDownList ddl)
        {
            DocsPaWR.Registro[] userRegistri = getListaRegistri();

            if (userRegistri != null && userRegistri.Length > 0)
            {
                //Spaziatura per RF
                string strText = "";
                int contatoreRegRF = 0;
                for (short iff = 0; iff < 3; iff++)
                {
                    strText += " ";
                }

                //Aggiunta registri o rf
                for (int i = 0; i < userRegistri.Length; i++)
                {
                    if (!userRegistri[i].Sospeso && !userRegistri[i].flag_pregresso)
                    {
                        //string testo = string.Empty;
                        
                        if (userRegistri[i].chaRF == "1")
                        {
                            //RF
                            if (UserManager.ruoloIsAutorized(this, "DO_INS_CORR_RF"))
                            {
                                string testo = strText + userRegistri[i].codRegistro;
                                ListItem item = new ListItem();
                                item.Text = testo;
                                item.Value = userRegistri[i].systemId + "_" + userRegistri[i].rfDisabled + "_" + userRegistri[i].idAOOCollegata;
                                ddl.Items.Add(item);
                            }
                        }
                        else
                        {
                            //Registro
                            if (UserManager.ruoloIsAutorized(this, "DO_INS_CORR_REG"))
                            {
                                string testo = userRegistri[i].codRegistro;
                                ListItem item = new ListItem();
                                item.Text = testo;
                                item.Value = userRegistri[i].systemId + "_" + userRegistri[i].rfDisabled + "_" + userRegistri[i].idAOOCollegata;
                                ddl.Items.Add(item);
                            }

                        }
                        #region
                        //ListItem item = new ListItem();
                        //item.Text = testo;
                        //item.Value = userRegistri[i].systemId + "_" + userRegistri[i].rfDisabled + "_" + userRegistri[i].idAOOCollegata;
                        //ddl.Items.Add(item);
                        #endregion

                        //ddl.Items.Add(testo);

                        //ddl.Items[contatoreRegRF].Value = userRegistri[i].systemId + "_" + userRegistri[i].rfDisabled + "_" + userRegistri[i].idAOOCollegata;
                        //contatoreRegRF++;
                    }
                }

                //Aggiunta voce su tutti i registri o rf
                if (UserManager.ruoloIsAutorized(this, "DO_INS_CORR_TUTTI"))
                {
                    ListItem item = new ListItem();
                    item.Text = "TUTTI";
                    item.Value = "";
                    ddl.Items.Add(item);
                    ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByText("TUTTI"));
                }
            }

            #region Commento
            //DocsPaWR.Registro[] userRegistri = getListaRegistri();
            
            //if (userRegistri!=null && userRegistri.Length > 0)
            //{
            //    string strText = "";
            //    int contatoreRegRF = 0;
            //    for (short iff = 0; iff < 3; iff++)
            //    {
            //        strText += " ";
            //    }
            //    for (int i = 0; i < userRegistri.Length; i++)
            //    {
            //        if (!userRegistri[i].Sospeso)
            //        {
            //            string testo = userRegistri[i].codRegistro;
            //            if (userRegistri[i].chaRF == "1")
            //            {
            //                testo = strText + testo;
            //            }
            //            ddl.Items.Add(testo);                        

            //            ddl.Items[contatoreRegRF].Value = userRegistri[i].systemId + "_" + userRegistri[i].rfDisabled + "_" + userRegistri[i].idAOOCollegata;
            //            contatoreRegRF++;
            //        }
            //    }

            //    if (modify != null && modify != "" && modify.Equals("true")) //se vengo da gestione rubrica
            //    {
            //        ListItem item = new ListItem("TUTTI", "");
            //        ddl.Items.Add(item);
            //        ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByText("TUTTI"));
            //    }
            //}
            #endregion Commento
        }

        private void ddl_tipoCorr_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            set_mode(ddl_tipoCorr.SelectedValue);
            if (this.dd_canpref.SelectedIndex == -1 || this.dd_canpref.SelectedIndex == 0)
            {
                this.star_aoo.Visible = false;
                this.star_amm.Visible = false;
            }
        }

        private void btn_chiudi_Click(object sender, System.EventArgs e)
        {
            Response.Write("<script language=\"javascript\">window.returnValue='CLOSE'; self.close();</script>");
        }

        private void dd_canpref_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            if (dd_canpref.SelectedItem.Text.ToUpper().Equals("MAIL") || dd_canpref.SelectedItem.Text.ToUpper().Equals("INTEROPERABILITA"))
                this.star.Visible = true;
            else
                this.star.Visible = false;

            if (dd_canpref.SelectedItem.Text.ToUpper().Equals("INTEROPERABILITA"))
            {
                this.star_amm.Visible = true;
                this.star_aoo.Visible = true;
            }
            else
            {
                this.star_amm.Visible = false;
                this.star_aoo.Visible = false;
            }
        }

        //		private void inserimento_PreRender(object sender, EventArgs e)
        //		{
        //			if(!IsPostBack)
        //			{
        //				SetFocus(CodRubrica);
        //			}
        //		}

        #region Multi Casella Corrispondenti esterni
        protected void BindGridViewCaselle()
        {
            if (ViewState["gvCaselle"] == null)
                ViewState["gvCaselle"] = new List<SAAdminTool.DocsPaWR.MailCorrispondente>();
            gvCaselle.DataSource = (List<SAAdminTool.DocsPaWR.MailCorrispondente>)ViewState["gvCaselle"];
            gvCaselle.DataBind();
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
            if (!utils.MultiCasellaManager.IsEnabledMultiMail(UserManager.getRuolo().idAmministrazione))
            {
                imgAggiungiCasella.Enabled = true;
                txtCasella.Enabled = true;
                txtNote.Enabled = true;
            }
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
            gvCaselle.DataSource = (List<SAAdminTool.DocsPaWR.MailCorrispondente>)ViewState["gvCaselle"];
            gvCaselle.DataBind();
            //se è disabilitato il multicasella, dopo l'immissione di una casella disabilito il pulsante aggiungi.
            if (!utils.MultiCasellaManager.IsEnabledMultiMail(UserManager.getRuolo().idAmministrazione))
            {
                txtCasella.Enabled = false;
                txtNote.Enabled = false;
                imgAggiungiCasella.Enabled = false;
            }
            //PULISCO I CAMPI EMAIL/NOTE EMAIL
            this.txtCasella.Text = string.Empty;
            this.txtNote.Text = string.Empty;
        }

        protected void rdbPrincipale_ChekedChanged(Object sender, EventArgs e)
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
        }

        protected void txtEmailCorr_TextChanged(object sender, EventArgs e)
        {
            string newMail = (sender as System.Web.UI.WebControls.TextBox).Text;
            int rowModify = ((sender as System.Web.UI.WebControls.TextBox).Parent.Parent as System.Web.UI.WebControls.GridViewRow).RowIndex;
            (ViewState["gvCaselle"] as List<SAAdminTool.DocsPaWR.MailCorrispondente>)[rowModify].Email = newMail;
        }

        protected void txtNoteMailCorr_TextChanged(object sender, EventArgs e)
        {
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
                }
            }
            return res; 
        }

        protected bool TypeMailCorrEsterno(string typeMail)
        {
            return (typeMail.Equals("1")) ? true : false;
        }
        #endregion
    }
}
