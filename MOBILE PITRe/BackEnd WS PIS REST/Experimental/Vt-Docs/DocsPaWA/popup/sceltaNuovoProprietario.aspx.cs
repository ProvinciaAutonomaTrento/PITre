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
using System.Configuration;
using DocsPAWA.utils;
using DocsPAWA.SiteNavigation;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.popup
{
    public partial class sceltaNuovoProprietario : DocsPAWA.CssPage
    {
        #region dichiarazioni
        protected DocsPAWA.DocsPaWR.Trasmissione trasmissione;
        protected ArrayList arrayIndTab;
        protected int index;
        protected System.Web.UI.WebControls.Label titolo;
        protected System.Web.UI.WebControls.Table tbl_Lista;
        protected System.Web.UI.WebControls.Button btn_ok;
        protected System.Web.UI.WebControls.Button btn_annulla;
        protected System.Web.UI.WebControls.Label lbl_avviso;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idRuoloNewPropr;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idPeopleNewPropr;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_tipo;

        protected System.Web.UI.WebControls.Panel pnl_dett_modello;
        protected System.Web.UI.WebControls.RadioButtonList rbl_share;              
        protected System.Web.UI.WebControls.TextBox txt_nomeModello;
        protected DocsPAWA.DocsPaWR.ModelloTrasmissione modello;

        protected string tipo = string.Empty;        
        #endregion

        #region Page Load
        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);
            try
            {
                trasmissione = TrasmManager.getGestioneTrasmissione(this);
                this.FillTableTrasmissioni();

                if (!IsPostBack)
                {
                    this.tipo = Request.QueryString["tipo"];
                    this.hd_tipo.Value = this.tipo;

                    switch (this.tipo)
                    {
                        case "ST":
                            this.gestioneProprietario();
                            break;

                        case "STempl":
                            this.gestioneModelli();
                            break;
                    }
                }
            }
            catch
            {
                string jscript = "<script language=javascript>alert('Errore nel reperimento dei dati!'); window.close();</script>";
                if (!ClientScript.IsStartupScriptRegistered("avvisaEchiude"))
                    ClientScript.RegisterStartupScript(this.GetType(), "avvisaEchiude", jscript);
            }
        }
        #endregion

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
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            this.btn_annulla.Click += new System.EventHandler(this.btn_annulla_Click);
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion

        #region metodi private
        private void gestioneProprietario()
        {
            this.pnl_dett_modello.Visible = false;                        
        }

        private void gestioneModelli()
        {
            this.gestioneProprietario();

            this.pnl_dett_modello.Visible = true;            
            //SetFocus(this.txt_nomeModello);

            this.rbl_share.Items[0].Text = rbl_share.Items[0].Text.Replace("@usr@", UserManager.getUtente(this).descrizione);
            this.rbl_share.Items[1].Text = rbl_share.Items[1].Text.Replace("@grp@", UserManager.getRuolo(this).descrizione);
        }

        /// <summary>
        /// gestione delle eventi sul tasto OK
        /// </summary>
        private void actionPost()
        {
            
            switch (this.hd_tipo.Value)
            {
                case "ST":
                    this.postGestioneProprietario();
                    break;

                case "STempl":
                    this.postGestioneModelli();
                    break;
            }
        }

        /// <summary>
        /// Post Gestione Proprietario : hd_tipo.Value = "ST"
        /// </summary>
        private void postGestioneProprietario()
        {
            string jscript = string.Empty;

            //Controllo che i campi obbligatori siano stati compilati
            if (this.hd_idPeopleNewPropr.Value.Equals(string.Empty) || this.hd_idRuoloNewPropr.Value.Equals(string.Empty))
            {
                jscript = "<script language='javascript'>alert('Selezionare un utente!');</script>";
                if (!ClientScript.IsStartupScriptRegistered("avvisaUt"))
                    ClientScript.RegisterStartupScript(this.GetType(), "avvisaUt", jscript);
                return;
            }

            trasmissione = TrasmManager.saveTrasm(this, trasmissione);
            trasmissione.daAggiornare = false;
            TrasmManager.setGestioneTrasmissione(this, trasmissione);
            TrasmManager.setDocTrasmSel(this, trasmissione);
            TrasmManager.setGestioneTrasmissione(this, trasmissione);

            //Session.Remove("doTrasm");
            Session.Add("doTrasm", trasmissione);

            if (this.hd_tipo.Value != "STempl")
            {

                if (trasmissione.tipoOggetto == DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
                    jscript = "<script language='javascript'>window.open('../documento/gestionedoc.aspx?tab=trasmissioni','principale'); window.close();</script>";
                else if (trasmissione.tipoOggetto == DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO)
                    jscript = "<script language='javascript'>window.open('../fascicolo/gestioneFasc.aspx?tab=trasmissioni','principale'); window.close();</script>";

                if (!ClientScript.IsStartupScriptRegistered("rinviaEchiude"))
                    ClientScript.RegisterStartupScript(this.GetType(), "rinviaEchiude", jscript);
                
            }

        }
            
        /// <summary>
        /// Post Gestione Modelli : hd_tipo.Value = "STempl"
        /// </summary>
        private void postGestioneModelli()
        {
            string jscript = string.Empty;
           
            //Controllo che i campi obbligatori siano stati compilati
            if (this.hd_idPeopleNewPropr.Value.Equals(string.Empty) || this.hd_idRuoloNewPropr.Value.Equals(string.Empty))
            {
                jscript = "<script language='javascript'>alert('Selezionare un utente!');</script>";
                if (!ClientScript.IsStartupScriptRegistered("avvisaUt"))
                    ClientScript.RegisterStartupScript(this.GetType(), "avvisaUt", jscript);
                return;
            }

            if (this.txt_nomeModello.Text.Trim() == "")
            {
                jscript = "<script language='javascript'>alert('Inserire il campo obbligatorio Nome Modello!');</script>";
                if (!ClientScript.IsStartupScriptRegistered("avvisaNM"))
                    ClientScript.RegisterStartupScript(this.GetType(), "avvisaNM", jscript);
                return;
            }

            // ------------------   TRASMISSIONE --------------------
            trasmissione.daAggiornare = true;
            TrasmManager.setGestioneTrasmissione(this, trasmissione);
            TrasmManager.setDocTrasmSel(this, trasmissione);
            TrasmManager.setGestioneTrasmissione(this, trasmissione);
           
            // ------------------   MODELLO TRASM --------------------
            modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            modello.NOME = this.txt_nomeModello.Text;
            if (this.rbl_share.Items[0].Selected)
            {
                for (int k = 0; k < modello.MITTENTE.Length; k++)
                {
                    modello.MITTENTE[k].ID_CORR_GLOBALI = 0;
                }
            }
            else
                modello.ID_PEOPLE = "";

            if (this.hd_tipo.Value.Equals("STempl"))
            {
                modello.CEDE_DIRITTI = "1";
                modello.ID_PEOPLE_NEW_OWNER = this.hd_idPeopleNewPropr.Value;
                modello.ID_GROUP_NEW_OWNER = this.hd_idRuoloNewPropr.Value;
            }

            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);
            ws.salvaModello(modello, infoUtente);

            Session.Remove("Modello");
           
            if (trasmissione != null && trasmissione.tipoOggetto == DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
                jscript = "<script>window.open('../trasmissione/trasmDatiTrasm_dx.aspx','iFrame_dx'); window.close();</script>";
            else
                jscript = "<script>window.open('../trasmissione/trasmFascDatiTras_dx.aspx','iFrame_dx'); window.close();</script>";

            if (!ClientScript.IsStartupScriptRegistered("chiude"))
                ClientScript.RegisterStartupScript(this.GetType(), "chiude", jscript);        
        }

        /// <summary>
        /// Imposta il Focus sull'oggetto web
        /// </summary>
        /// <param name="ctrl"></param>
        private void SetFocus(System.Web.UI.Control ctrl)
        {
            string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
            RegisterStartupScript("focus", s);
        }
        #endregion

        #region gestione cessione proprietà
        /// <summary>
        /// 
        /// </summary>
        private void FillTableTrasmissioni()
        {
            try
            {
                this.arrayIndTab = new ArrayList();
                if (trasmissione != null)
                {
                    lbl_avviso.Text = "<br>Per questa trasmissione è attiva l'opzione \"Cedi i diritti\".<br>";
                    if (trasmissione.trasmissioniSingole != null)
                    {
                        lbl_avviso.Text += "<br>Poichè il mittente è il proprietario del documento, selezionare<br>dalla lista seguente l'utente che erediterà la proprietà del documento.";
                        lbl_avviso.Text += "<br><br>La proprietà sarà estesa anche al ruolo dell'utente selezionato.<br><br>";
                        drawBorderRow();
                        drawHeader();

                        foreach (DocsPaWR.TrasmissioneSingola trs in trasmissione.trasmissioniSingole)
                            drawTable(trs);

                        drawBorderRow();
                    }
                }
            }
            catch
            {
                string jscript = "<script language=javascript>alert('Errore nel reperimento dei dati!'); window.close();</script>";
                if (!ClientScript.IsStartupScriptRegistered("avvisaEchiude"))
                    ClientScript.RegisterStartupScript(this.GetType(), "avvisaEchiude", jscript);
            }
        }

        private void drawBorderRow()
        {
            //riga separatrice(color rosa)
            TableRow tr = new TableRow();
            TableCell tc = new TableCell();
            tr.CssClass = "bg_grigioS";
            tc.ColumnSpan = 3;

            tc.Width = Unit.Percentage(100);
            tc.Height = Unit.Pixel(10);
            tr.Cells.Add(tc);

            this.tbl_Lista.Rows.Add(tr);
        }

        private void drawHeader()
        {
            TableRow tr;
            TableCell tc;

            //riga titoli
            tr = new TableRow();            
            tr.Height = Unit.Pixel(15);
            tr.CssClass = "menu_1_bianco";
            tr.BackColor = Color.FromArgb(149, 149, 149);
            tr.BorderColor = Color.DarkGray;
            //destinatatio
            tc = new TableCell();
            tc.Width = Unit.Percentage(60);
            tc.Text = "Destinatario";
            tr.Cells.Add(tc);
            //ragione
            tc = new TableCell();
            tc.Width = Unit.Percentage(34);
            tc.Text = "Ragione";
            tr.Cells.Add(tc);
            //check
            tc = new TableCell();
            tc.Width = Unit.Percentage(6);          
            tc.Text = "Sel";
            tr.Cells.Add(tc);

            //Aggiungo Row alla table1
            this.tbl_Lista.Rows.Add(tr);
        }
        
        protected void drawTable(DocsPAWA.DocsPaWR.TrasmissioneSingola trasmS)
        {
            try
            {
                string idGruppo = string.Empty;

                if (trasmS.corrispondenteInterno != null)
                {
                    Type tipoCI = trasmS.corrispondenteInterno.GetType();

                    if (tipoCI == typeof(DocsPAWA.DocsPaWR.Utente))
                        idGruppo = ((DocsPAWA.DocsPaWR.Utente)(trasmS.corrispondenteInterno)).idPeople;
                    if (tipoCI == typeof(DocsPAWA.DocsPaWR.Ruolo))
                        idGruppo = ((DocsPAWA.DocsPaWR.Ruolo)(trasmS.corrispondenteInterno)).idGruppo;

                    //if (trasmS.corrispondenteInterno.tipoCorrispondente.Equals("P"))
                    //    idGruppo = ((DocsPAWA.DocsPaWR.Utente)(trasmS.corrispondenteInterno)).idPeople;
                    //if (trasmS.corrispondenteInterno.tipoCorrispondente.Equals("R"))
                    //    idGruppo = ((DocsPAWA.DocsPaWR.Ruolo)(trasmS.corrispondenteInterno)).idGruppo;

                    TableRow tr = new TableRow();
                    tr.ID = "tbl_corr_" + index.ToString();
                    tr.CssClass = "testo_grigio";
                    tr.BackColor = Color.FromArgb(242, 242, 242);
                    tr.Height = Unit.Pixel(20);

                    // prima cella
                    TableCell tc = new TableCell();
                    tc.Text = "<b>" + trasmS.corrispondenteInterno.descrizione + "</b>";
                    tr.Cells.Add(tc);

                    // seconda cella
                    tc = new TableCell();
                    tc.Text = trasmS.ragione.descrizione;
                    tr.Cells.Add(tc);

                    // terza cella
                    tc = new TableCell();
                    tc.Text = " ";
                    tr.Cells.Add(tc);

                    //Aggiungo Row alla table1
                    this.tbl_Lista.Rows.Add(tr);

                    arrayIndTab.Add(index);
                    index++;

                    //ciclo per utenti
                    foreach (DocsPaWR.TrasmissioneUtente trasmUt in trasmS.trasmissioneUtente)
                    {
                        TableRow tru = new TableRow();
                        TableCell tcu = new TableCell();

                        tru.ID = "tbl_ut_" + index.ToString();
                        tcu.BackColor = Color.FromArgb(242, 242, 242);
                        tcu.CssClass = "testo_grigio";
                        tcu.HorizontalAlign = HorizontalAlign.Left;
                        tcu.BorderColor = Color.FromArgb(217, 217, 217);
                        tcu.Text = "&nbsp;&nbsp;" + trasmUt.utente.descrizione;

                        tru.Cells.Add(tcu);

                        tcu = new TableCell();
                        tcu.BackColor = Color.FromArgb(242, 242, 242);
                        tcu.Text = " ";
                        tru.Cells.Add(tcu);

                        TableCell tcuP = new TableCell();
                        //CheckBox chkPropr = new CheckBox();
                        RadioButton chkPropr = new RadioButton();
                        chkPropr.ID = "chkNotifica_" + trasmUt.utente.idPeople + "_" + idGruppo; // ID univoco nel formato: chkNotifica_<idPeople>_<idGroup>
                        chkPropr.Attributes.Add("onclick", "SingleSelect('chk',this);");
                        chkPropr.CheckedChanged += new EventHandler(this.chkPropr_CheckedChanged);
                        chkPropr.GroupName = "radioProp";


                        tcuP.HorizontalAlign = HorizontalAlign.Center;
                        tcuP.BorderColor = Color.FromArgb(217, 217, 217);
                        tcuP.BackColor = Color.FromArgb(242, 242, 242);
                        tcuP.CssClass = "testo_grigio";
                        tcuP.Controls.Add(chkPropr);

                        tru.Cells.Add(tcuP);

                        this.tbl_Lista.Rows.Add(tru);

                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        /// <summary>
        /// Selezione dei check box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkPropr_CheckedChanged(object sender, System.EventArgs e)
        {
            DocsPaWR.Trasmissione trasmissione = TrasmManager.getGestioneTrasmissione(this);

            //string IdCheckPropr = ((CheckBox)sender).ID;
            string IdCheckPropr = ((RadioButton)sender).ID;
           
            string[] valori = IdCheckPropr.Split('_');

            this.hd_idPeopleNewPropr.Value = valori[1];
            this.hd_idRuoloNewPropr.Value = valori[2];

            // Se si proviene dalla trasmissione massiva, vengono impostati gli id di utente e ruolo
            if (Session["fromMassiveAct"] != null && CallContextStack.CurrentContext.ContextState["TransmissionCollection"] != null)
            {
                MassiveOperationTrasmissionDetailsCollection obj = (MassiveOperationTrasmissionDetailsCollection)CallContextStack.CurrentContext.ContextState["TransmissionCollection"];

                obj.DocumentLeasing = new CessioneDocumento()
                {
                    idPeopleNewPropr = valori[1],
                    idRuoloNewPropr = valori[2]
                };

            }

            trasmissione.cessione.idPeopleNewPropr = this.hd_idPeopleNewPropr.Value;
            trasmissione.cessione.idRuoloNewPropr = this.hd_idRuoloNewPropr.Value;

            trasmissione = impostaUtenteConNotifica(trasmissione);

            TrasmManager.setGestioneTrasmissione(this, trasmissione);
        }

        private DocsPaWR.Trasmissione impostaUtenteConNotifica(DocsPaWR.Trasmissione trasm)
        {
            foreach (DocsPaWR.TrasmissioneSingola trasmS in trasm.trasmissioniSingole)
            {
                foreach (DocsPaWR.TrasmissioneUtente trasmU in trasmS.trasmissioneUtente)
                {
                    if (trasmU.utente.idPeople == this.hd_idPeopleNewPropr.Value)
                        trasmU.daNotificare = true;
                    else
                        trasmU.daNotificare = false;
                }
            }

            return trasm;
        }

        #endregion        

        #region Tasti
        /// <summary>
        /// Pulsante OK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ok_Click(object sender, System.EventArgs e)
        {
            try
            {
                // Se si proviene dalla trasmissione massiva, non bisogna chiudere la finestra e refreshare, ma bisogna solo chiudere
                // la finestra
                if (Session["fromMassiveAct"] != null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "close", "window.close();", true);
                    Session.Remove("fromMassiveAct");
                    return;
                }

                this.actionPost();
            }
            catch
            {
                string jscript = "<script language=javascript>alert('Errore nella memorizzazione dei dati!');</script>";
                if (!ClientScript.IsStartupScriptRegistered("avvisa"))
                    ClientScript.RegisterStartupScript(this.GetType(), "avvisa", jscript);
            }
        }

        /// <summary>
        /// Pulsante annulla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_annulla_Click(object sender, System.EventArgs e)
        {
            Session.Remove("Modello");

            trasmissione.cessione = null;
            TrasmManager.setGestioneTrasmissione(this, trasmissione);
            TrasmManager.setDocTrasmSel(this, trasmissione);
            TrasmManager.setGestioneTrasmissione(this, trasmissione);

            string jscript = "<script language=javascript>window.close();</script>";
            if (!ClientScript.IsStartupScriptRegistered("chiude"))
                ClientScript.RegisterStartupScript(this.GetType(), "chiude", jscript);
        }
        #endregion        
    }
}
