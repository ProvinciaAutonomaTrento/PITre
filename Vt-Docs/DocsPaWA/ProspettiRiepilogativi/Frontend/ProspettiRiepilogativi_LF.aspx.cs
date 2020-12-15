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
using ProspettiRiepilogativi.Frontend;
using DocsPAWA.DocsPaWR;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using DocsPAWA;
using DocsPAWA.utils;



namespace ProspettiRiepilogativi
{
    /// <summary>
    /// Summary description for ProspettiRiepilogativi_LF.
    /// </summary>
    public class ProspettiRiepilogativi_LF : DocsPAWA.CssPage
    {
        protected System.Web.UI.WebControls.ImageButton btnStampa;
        protected System.Web.UI.WebControls.Label lb_report;
        protected System.Web.UI.WebControls.DropDownList ddl_registro;
        public System.Web.UI.WebControls.DropDownList ddl_anno;
        protected System.Web.UI.WebControls.RadioButtonList rb_prospettiRiepilogativi;
        protected System.Web.UI.WebControls.DropDownList ddl_amm;
        protected System.Web.UI.WebControls.ImageButton btn_zoom;
        protected System.Web.UI.WebControls.DropDownList ddl_sede;
        protected System.Web.UI.WebControls.Panel panel_sede;
        protected System.Web.UI.WebControls.Panel pnl_modalita;
        protected System.Web.UI.WebControls.RadioButtonList rb_modalita;
        protected System.Web.UI.WebControls.DataGrid DataGrid1;
        protected System.Web.UI.WebControls.DropDownList ddl_Mese;
        protected System.Web.UI.WebControls.Panel pnl_mese;
        protected System.Web.UI.WebControls.Panel panel_reg;
        protected System.Web.UI.WebControls.Panel pnl_protArma;
        protected System.Web.UI.WebControls.Panel pnl_anno;
        protected System.Web.UI.WebControls.Panel pnl_amm;
        protected System.Web.UI.WebControls.DropDownList ddl_et_titolario;
        protected System.Web.UI.WebControls.Panel pnl_DettPratica;
        protected System.Web.UI.WebControls.TextBox txt_NumPratica;
        protected System.Web.UI.WebControls.DropDownList ddl_regCC;
        protected System.Web.UI.WebControls.Panel pnl_regCC;
        protected System.Web.UI.WebControls.Panel pnl_DocSpediti;
        protected DocsPAWA.UserControls.Calendar txt_dataSpedDa;
        protected DocsPAWA.UserControls.Calendar txt_dataSpedA;
        protected System.Web.UI.WebControls.RadioButtonList rb_confermaSpedizione;
        //Filtri ricerca CDC
        protected System.Web.UI.WebControls.Panel pnl_FiltriCDC;
        protected System.Web.UI.WebControls.Label lbl_Data;
        protected System.Web.UI.WebControls.Label lbl_Uffici;
        protected System.Web.UI.WebControls.Label lbl_Magistrato;
        protected System.Web.UI.WebControls.Label lbl_Revisore;
        protected DocsPAWA.UserControls.Corrispondente corr_magistrato;
        protected DocsPAWA.UserControls.Corrispondente corr_revisore;
        protected System.Web.UI.WebControls.DropDownList ddl_uffici;
        protected System.Web.UI.WebControls.Label lbl_DataDa;
        protected System.Web.UI.WebControls.Label lbl_DataA;
        protected DocsPAWA.UserControls.Calendar dataDa;
        protected DocsPAWA.UserControls.Calendar dataA;

        protected System.Web.UI.WebControls.Panel pnl_sottoposto;
        protected System.Web.UI.WebControls.RadioButtonList rb_scelta_sott;
        protected System.Web.UI.WebControls.Panel pnl_scelta_uo;
        protected System.Web.UI.WebControls.Panel pnl_scelta_rf;
        protected System.Web.UI.WebControls.CheckBox chk_sottoposti;
        protected System.Web.UI.WebControls.Image btn_img_sott_rubr;
        protected DocsPAWA.UserControls.Calendar cld_creazione_dal;
        protected DocsPAWA.UserControls.Calendar cld_creazione_al;
        protected DocsPAWA.UserControls.Calendar cld_chiusura_dal;
        protected DocsPAWA.UserControls.Calendar cld_chiusura_al;
        protected System.Web.UI.WebControls.DropDownList ddl_rf;
        protected System.Web.UI.WebControls.DropDownList ddl_titolari;
        protected System.Web.UI.WebControls.DropDownList ddl_tipo_data_creazione;
        protected System.Web.UI.WebControls.DropDownList ddl_tipo_data_chiusura;
        protected System.Web.UI.WebControls.Label lbl_al_apertura;
        protected System.Web.UI.WebControls.Label lbl_al_chiusura;
        protected System.Web.UI.WebControls.Label lbl_dal_apertura;
        protected System.Web.UI.WebControls.Label lbl_dal_chiusura;
        protected System.Web.UI.WebControls.TextBox txt1_corr_sott;
        protected System.Web.UI.WebControls.TextBox txt2_corr_sott;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_systemIdCorrSott;
        protected System.Web.UI.HtmlControls.HtmlControl iframeVisUnificata;
        protected System.Web.UI.WebControls.DropDownList ddl_titolari_report_annuale;
        protected System.Web.UI.WebControls.Panel panel_tit_ann_fasc;
        protected System.Web.UI.WebControls.Panel pnlContrData;

        protected Hashtable idUffici = null;

        protected DocsPaWebService wws = new DocsPaWebService();

        protected ArrayList filtri = new ArrayList();
        DocsPAWA.DocsPaWR.Utente _utente = new DocsPAWA.DocsPaWR.Utente();

        private void Page_Load(object sender, System.EventArgs e)
        {
            #region Caricamento dei dati iniziali
            if (!IsPostBack)
            {

                utility.RemoveFileReport(this);
                //Caricamento radioButtonList
                setRadioButtonList();

                //rb_prospettiRiepilogativi.Items.Clear();

                //ArrayList reports = Controller.DO_ReadXML();
                //foreach(DocsPAWA.DocsPaWR.Report r in reports)
                //{
                //    if (r.Valore == "stampaProtArma" || r.Valore == "stampaDettaglioPratica" || r.Valore == "stampaGiornaleRiscontri" || r.Valore == "documentiSpediti")
                //    {
                //        if (!string.IsNullOrEmpty(wws.isEnableProtocolloTitolario()))
                //            rb_prospettiRiepilogativi.Items.Add(new ListItem(r.Descrizione, r.Valore));
                //    }
                //    else
                //    {
                //        if ((r.Valore == "reportNumFasc" || r.Valore == "reportNumDocInFasc"))
                //        {
                //            if (DocsPAWA.UserManager.ruoloIsAutorized(this, "EXP_FASC_COUNT"))
                //            {
                //                rb_prospettiRiepilogativi.Items.Add(new ListItem(r.Descrizione, r.Valore));
                //            }
                //        }
                //        else
                //        {
                //            rb_prospettiRiepilogativi.Items.Add(new ListItem(r.Descrizione, r.Valore));
                //        }
                //    }
                //}
                //panel_sede.Visible = true;

                _utente = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
                if (_utente != null)
                {
                    this.DO_SetControlFromDocsPA(_utente);

                    //NUOVI REPORT FASCICOLI
                    if (DocsPAWA.UserManager.ruoloIsAutorized(this, "EXP_FASC_COUNT"))
                    {
                        caricaComboRf();
                        caricaComboTitolari();
                    }
                }
                else
                {
                    //Caricamento delle amministrazionei
                    setDdlAmministrazioni();
                }

                //Caricamento stampa protocollo arma
                setStampaProtocolloArma();

                //titolario per report annuale fascicolo per voce di titolario
                caricaComboTitolariReportTitolario();
            }

            //NUOVI REPORT FASCICOLI
            if (DocsPAWA.UserManager.ruoloIsAutorized(this, "EXP_FASC_COUNT"))
            {
                string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(DocsPAWA.ConfigSettings.KeysENUM.RUBRICA_V2);
                if (use_new_rubrica == "1")
                {
                    this.btn_img_sott_rubr.Attributes.Add("onclick", "_ApriRubricaRicercaRuoliSottoposti();");
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

            }
            //Controllo dei report per CDC
            setCampiCorrCDC();
        }
            #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void prospettiRiepilogativi_PreRender(object sender, EventArgs e)
        {
            if (DocsPAWA.UserManager.ruoloIsAutorized(this, "EXP_FASC_COUNT"))
            {
                //carico il ruolo sottoposto, se esiste
                DocsPAWA.DocsPaWR.Corrispondente corrSott = DocsPAWA.UserManager.getCorrispondenteSelezionatoRuoloSottoposto(this);
                if (corrSott != null)
                {
                    DocsPAWA.UserManager.removeCorrispondentiSelezionati(this);
                    if (!corrSott.codiceRubrica.Equals(""))
                    {
                        this.txt1_corr_sott.Text = corrSott.codiceRubrica;
                        this.txt2_corr_sott.Text = DocsPAWA.UserManager.getDecrizioneCorrispondenteSemplice(corrSott);
                        DocsPAWA.UserManager.setCorrispondenteSelezionatoRuoloSottoposto(this.Page, corrSott);
                    }
                }
            }
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rb_prospettiRiepilogativi.SelectedIndexChanged += new System.EventHandler(this.rb_prospettiRiepilogativi_SelectedIndexChanged);
            this.ddl_amm.SelectedIndexChanged += new System.EventHandler(this.ddl_amm_SelectedIndexChanghed);
            this.ddl_registro.SelectedIndexChanged += new System.EventHandler(this.ddl_registro_SelectedIndexChanged);
            this.ddl_sede.SelectedIndexChanged += new System.EventHandler(this.ddl_sede_SelectedIndexChanged);
            this.ddl_anno.SelectedIndexChanged += new System.EventHandler(this.ddl_anno_SelectedIndexChanged);
            this.ddl_Mese.SelectedIndexChanged += new System.EventHandler(this.ddl_Mese_SelectedIndexChanged);
            this.rb_modalita.SelectedIndexChanged += new System.EventHandler(this.rb_modalita_SelectedIndexChanged);
            this.ddl_et_titolario.SelectedIndexChanged += new System.EventHandler(this.ddl_et_titolario_SelectedIndexChanged);
            this.btnStampa.Click += new System.Web.UI.ImageClickEventHandler(this.btnStampa_Click);
            this.btn_zoom.Click += new System.Web.UI.ImageClickEventHandler(this.btn_zoom_Click);
            this.txt_NumPratica.TextChanged += new System.EventHandler(this.txt_NumPratica_TextChanged);
            this.ddl_regCC.SelectedIndexChanged += new System.EventHandler(this.ddl_regCC_SelectedIndexChanged);
            this.rb_scelta_sott.SelectedIndexChanged += new System.EventHandler(this.rb_scelta_sott_SelectedIndexChanged);
            this.ddl_rf.SelectedIndexChanged += new System.EventHandler(this.ddl_rf_SelectedIndexChanged);
            this.ddl_tipo_data_creazione.SelectedIndexChanged += new System.EventHandler(this.ddl_tipo_data_creazione_SelectedIndexChanged);
            this.ddl_tipo_data_chiusura.SelectedIndexChanged += new System.EventHandler(this.ddl_tipo_data_chiusura_SelectedIndexChanged);
            this.txt1_corr_sott.TextChanged += new System.EventHandler(this.txt1_corr_sott_TextChanged);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.prospettiRiepilogativi_PreRender);

        }
        #endregion

        #region btnStampa_Click
        private void btnStampa_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                //usato con id univoco per le tabelle dei prospetti
                DocsPAWA.DocsPaWR.Utente ut = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
                int idPeople = Convert.ToInt32(ut.idPeople);
                // timestamp per i prospetti
                string timeStamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").Replace(".", ":");

                DocsPAWA.DocsPaWR.InfoUtente infoUtente = DocsPAWA.UserManager.getInfoUtente(this);
                DocsPAWA.DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
                infoUtente.idAmministrazione = cr.idAmministrazione;

                DocsPAWA.DocsPaWR.FileDocumento fileRep = new DocsPAWA.DocsPaWR.FileDocumento();

                string searchType = "";

                //EXPORT FASCICOLI
                if (DocsPAWA.UserManager.ruoloIsAutorized(this, "EXP_FASC_COUNT") && (rb_prospettiRiepilogativi.SelectedValue.Equals("reportNumFasc") || rb_prospettiRiepilogativi.SelectedValue.Equals("reportNumDocInFasc")))
                {

                    if (controllaCampiObbligatoriExportFasc())
                    {
                        //Session.Remove("filtri");
                        ArrayList filtriTemp = new ArrayList();
                        filtriTemp = (ArrayList)Session["filtri"];

                        if (rb_prospettiRiepilogativi.SelectedValue.Equals("reportNumFasc"))
                        {
                            ProspettiRiepilogativi.Frontend.Parametro _tipo_ricerca = new ProspettiRiepilogativi.Frontend.Parametro("tipo_ricerca", "tipo_ricerca", "reportNumFasc");
                            utility.DO_UpdateParameters(filtri, _tipo_ricerca);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                            searchType = "reportNumFasc";
                        }
                        else
                        {
                            ProspettiRiepilogativi.Frontend.Parametro _tipo_ricerca = new ProspettiRiepilogativi.Frontend.Parametro("tipo_ricerca", "tipo_ricerca", "reportNumDocInFasc");
                            utility.DO_UpdateParameters(filtri, _tipo_ricerca);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                            searchType = "reportNumDocInFasc";
                        }

                        if (this.rb_scelta_sott.SelectedValue.Equals("UO"))
                        {
                            DocsPAWA.DocsPaWR.Corrispondente corrUo = DocsPAWA.UserManager.getCorrispondenteByCodRubrica(this, this.txt1_corr_sott.Text);
                            ProspettiRiepilogativi.Frontend.Parametro _uo = new ProspettiRiepilogativi.Frontend.Parametro("tipo_scelta", "uo", corrUo.systemId);
                            ProspettiRiepilogativi.Frontend.Parametro _sottoposti = new ProspettiRiepilogativi.Frontend.Parametro("sottoposti", "sottoposti", (this.chk_sottoposti.Checked).ToString());
                            ProspettiRiepilogativi.Frontend.Parametro _nome_scelta = new ProspettiRiepilogativi.Frontend.Parametro("nome_scelta", "nome_scelta", (this.txt2_corr_sott.Text));
                            utility.DO_UpdateParameters(filtri, _uo);
                            //  Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                            utility.DO_UpdateParameters(filtri, _sottoposti);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                            utility.DO_UpdateParameters(filtri, _nome_scelta);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];

                        }
                        else
                        {
                            ProspettiRiepilogativi.Frontend.Parametro _rf = new ProspettiRiepilogativi.Frontend.Parametro("tipo_scelta", "rf", this.ddl_rf.SelectedValue);
                            ProspettiRiepilogativi.Frontend.Parametro _nome_scelta = new ProspettiRiepilogativi.Frontend.Parametro("nome_scelta", "nome_scelta", (this.ddl_rf.SelectedItem.Text));
                            utility.DO_UpdateParameters(filtri, _rf);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                            utility.DO_UpdateParameters(filtri, _nome_scelta);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];

                        }

                        //Comuni per tutti e due
                        ProspettiRiepilogativi.Frontend.Parametro _titolario = new ProspettiRiepilogativi.Frontend.Parametro("titolario_fasc", "titolario_fasc", this.ddl_titolari.SelectedValue);
                        utility.DO_UpdateParameters(filtri, _titolario);
                        Session["filtri"] = filtri;
                        filtri = (ArrayList)Session["filtri"];

                        //DATA CREAZIONE
                        if (ddl_tipo_data_creazione.SelectedValue.Equals("0") || ddl_tipo_data_creazione.SelectedValue.Equals("2"))
                        {
                            if (ddl_tipo_data_creazione.SelectedValue.Equals("2"))
                            {
                                ProspettiRiepilogativi.Frontend.Parametro _data_creazioneDal = new ProspettiRiepilogativi.Frontend.Parametro("dataC", this.GetCalendarControl("cld_creazione_dal").txt_Data.Text, "APERTURA_TODAY");
                                utility.DO_UpdateParameters(filtri, _data_creazioneDal);
                            }

                            if (ddl_tipo_data_creazione.SelectedValue.Equals("4"))
                            {
                                ProspettiRiepilogativi.Frontend.Parametro _data_creazioneDal = new ProspettiRiepilogativi.Frontend.Parametro("dataC", this.GetCalendarControl("cld_creazione_dal").txt_Data.Text, "APERTURA_MC");
                                utility.DO_UpdateParameters(filtri, _data_creazioneDal);
                            }

                            if (ddl_tipo_data_creazione.SelectedValue.Equals("3"))
                            {
                                ProspettiRiepilogativi.Frontend.Parametro _data_creazioneDal = new ProspettiRiepilogativi.Frontend.Parametro("dataC", this.GetCalendarControl("cld_creazione_dal").txt_Data.Text, "APERTURA_SC");
                                utility.DO_UpdateParameters(filtri, _data_creazioneDal);
                            }

                            if (!ddl_tipo_data_creazione.SelectedValue.Equals("2") && !ddl_tipo_data_creazione.SelectedValue.Equals("3") && !ddl_tipo_data_creazione.SelectedValue.Equals("4"))
                            {
                                ProspettiRiepilogativi.Frontend.Parametro _data_creazioneDal = new ProspettiRiepilogativi.Frontend.Parametro("dataC", "dataC", this.GetCalendarControl("cld_creazione_dal").txt_Data.Text);
                                utility.DO_UpdateParameters(filtri, _data_creazioneDal);
                            }


                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                        }
                        if (ddl_tipo_data_creazione.SelectedValue.Equals("1") || ddl_tipo_data_creazione.SelectedValue.Equals("3") || ddl_tipo_data_creazione.SelectedValue.Equals("4"))
                        {
                            ProspettiRiepilogativi.Frontend.Parametro _data_creazioneDal = new ProspettiRiepilogativi.Frontend.Parametro("dataCdal", "dataCdal", this.GetCalendarControl("cld_creazione_dal").txt_Data.Text);
                            ProspettiRiepilogativi.Frontend.Parametro _data_creazioneAl = new ProspettiRiepilogativi.Frontend.Parametro("dataCal", "dataCal", this.GetCalendarControl("cld_creazione_al").txt_Data.Text);
                            utility.DO_UpdateParameters(filtri, _data_creazioneDal);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                            utility.DO_UpdateParameters(filtri, _data_creazioneAl);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                        }

                        //DATA CHIUSURA

                        if (ddl_tipo_data_chiusura.SelectedValue.Equals("0") || ddl_tipo_data_chiusura.SelectedValue.Equals("2"))
                        {
                            ProspettiRiepilogativi.Frontend.Parametro _data_chiusuraDal = new ProspettiRiepilogativi.Frontend.Parametro("dataCh", "dataCh", this.GetCalendarControl("cld_chiusura_dal").txt_Data.Text);
                            utility.DO_UpdateParameters(filtri, _data_chiusuraDal);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                        }
                        if (ddl_tipo_data_chiusura.SelectedValue.Equals("1") || ddl_tipo_data_chiusura.SelectedValue.Equals("3") || ddl_tipo_data_chiusura.SelectedValue.Equals("4"))
                        {
                            ProspettiRiepilogativi.Frontend.Parametro _data_chiusuraDal = new ProspettiRiepilogativi.Frontend.Parametro("dataChdal", "dataChdal", this.GetCalendarControl("cld_chiusura_dal").txt_Data.Text);
                            ProspettiRiepilogativi.Frontend.Parametro _data_chiusuraAl = new ProspettiRiepilogativi.Frontend.Parametro("dataChal", "dataChal", this.GetCalendarControl("cld_chiusura_al").txt_Data.Text);
                            utility.DO_UpdateParameters(filtri, _data_chiusuraDal);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                            utility.DO_UpdateParameters(filtri, _data_chiusuraAl);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                        }


                        fileRep = Controller.DO_StampaExcel(filtri, timeStamp, infoUtente, searchType);
                        
                        /* 
                        if (fileRep != null)
                        {

                            Session["Estrazione"] = fileRep;

                            this.iframeVisUnificata.Attributes["src"] = "ReportExternalFile.aspx";
                            utility.do_openinRightFrame(this, "whitepage.htm");

                            /*  Response.ContentType = fileRep.contentType;
                              Response.AddHeader("content-disposition", "attachment;filename=" + fileRep.name);
                              Response.AddHeader("content-length", fileRep.content.Length.ToString());
                              Response.BinaryWrite(fileRep.content);

                            this.btn_zoom.Enabled = false;
                            //utility.do_openinRightFrame(this, "whitepage.htm");
                        }
                        else
                        {
                            utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                            utility.do_openinRightFrame(this, "whitepage.htm");

                        } */

                        //PALUMBO: modifica per permettere l'apertura dell'export xls come per gli altri export
                        string tipologia = "XLS";

                        if (fileRep == null || fileRep.content == null || fileRep.content.Length == 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "noRisultatiRicerca", "alert('Nessun documento trovato.');", true);
                            ClientScript.RegisterStartupScript(this.GetType(), "openFile", "OpenFile('" + tipologia + "');", true);
                        }
                        else
                        {
                            DocsPAWA.exportDati.exportDatiSessionManager session = new DocsPAWA.exportDati.exportDatiSessionManager();
                            session.SetSessionExportFile(fileRep);
                            ClientScript.RegisterStartupScript(this.GetType(), "openFile", "OpenFile('" + tipologia + "');", true);
                        }

                        Session["filtri"] = filtriTemp;
                    }
                }
                //FINE EXPORT FASCIOLI
                else
                {
                    //ClientScript.RegisterStartupScript(this.GetType(), "wait", "top.principale.frames[1].location='waitingpage.htm';", true);
                    //verifica campi obbligatori (amministrazione,registro,anno)
                    if ((ddl_anno.SelectedValue.Equals("") || ddl_registro.SelectedValue.Equals("") || ddl_amm.SelectedValue.Equals("")) && (this.pnl_DettPratica.Visible == false && this.pnl_protArma.Visible == false && this.pnl_regCC.Visible == false))
                    {
                        utility.do_alert(this, "Attenzione inserire i campi obbligatori");

                        utility.do_openinRightFrame(this, "whitepage.htm");
                    }
                    else
                    {
                        if (this.pnl_DettPratica.Visible == true && string.IsNullOrEmpty(this.txt_NumPratica.Text))
                        {
                            utility.do_alert(this, "Attenzione inserire i campi obbligatori");
                            utility.do_openinRightFrame(this, "whitepage.htm");
                        }
                        else
                        {
                            if (Session["filtri"] != null)
                            {
                                filtri = (ArrayList)Session["filtri"];
                                ProspettiRiepilogativi.Frontend.Parametro _anno = new ProspettiRiepilogativi.Frontend.Parametro("anno", "anno", ddl_anno.SelectedValue.ToString());
                                ProspettiRiepilogativi.Frontend.Parametro _mod = new ProspettiRiepilogativi.Frontend.Parametro("mod", "mod", "Compatta");
                                utility.DO_UpdateParameters(filtri, _anno, false);
                                utility.DO_UpdateParameters(filtri, _mod, false);
                                if (pnl_mese.Visible == true)
                                {
                                    ProspettiRiepilogativi.Frontend.Parametro _mese = new ProspettiRiepilogativi.Frontend.Parametro();
                                    _mese.Nome = "mese";
                                    _mese.Valore = ddl_Mese.SelectedValue;
                                    utility.DO_UpdateParameters(filtri, _mese);
                                }
                                if (this.pnl_protArma.Visible == true)
                                {
                                    ProspettiRiepilogativi.Frontend.Parametro _titolario = new ProspettiRiepilogativi.Frontend.Parametro("titolario", ddl_et_titolario.SelectedItem.Text, ddl_et_titolario.SelectedValue);
                                    utility.DO_UpdateParameters(filtri, _titolario);
                                }
                                if (pnl_DettPratica.Visible == true)
                                {
                                    string idTitolario = ddl_et_titolario.SelectedValue;
                                    string numProtPtratica = this.txt_NumPratica.Text;
                                    string idReg = this.ddl_registro.SelectedValue;
                                    DocsPAWA.DocsPaWR.Registro reg = wws.GetRegistroBySistemId(idReg);
                                    ArrayList nodoTitolario = new ArrayList(wws.getNodiFromProtoTit(reg, ((DocsPAWA.DocsPaWR.Utente)Session["userData"]).idAmministrazione, numProtPtratica, idTitolario));
                                    ProspettiRiepilogativi.Frontend.Parametro _pratica = new ProspettiRiepilogativi.Frontend.Parametro();//("pratica", numProtPtratica);
                                    _pratica.Nome = "pratica";
                                    _pratica.Valore = numProtPtratica;
                                    ProspettiRiepilogativi.Frontend.Parametro _classifica = new ProspettiRiepilogativi.Frontend.Parametro();//("classifica", ((DocsPAWA.DocsPaWR.OrgNodoTitolario)nodoTitolario[0]).Codice);
                                    _classifica.Nome = "classifica";
                                    if (nodoTitolario.Count > 0)
                                    {
                                        _classifica.Valore = ((DocsPAWA.DocsPaWR.OrgNodoTitolario)nodoTitolario[0]).Codice;
                                    }
                                    else
                                    {
                                        _classifica.Valore = string.Empty;
                                    }
                                    utility.DO_UpdateParameters(filtri, _pratica);
                                    Session["filtri"] = filtri;
                                    filtri = (ArrayList)Session["filtri"];
                                    utility.DO_UpdateParameters(filtri, _classifica);
                                    Session["filtri"] = filtri;
                                }
                                if (pnl_regCC.Visible == true)
                                {
                                    filtri = (ArrayList)Session["filtri"];
                                    ProspettiRiepilogativi.Frontend.Parametro _registroCC = new ProspettiRiepilogativi.Frontend.Parametro();
                                    _registroCC.Nome = "registroCC";
                                    _registroCC.Valore = ddl_regCC.SelectedValue;
                                    _registroCC.Descrizione = ddl_regCC.SelectedItem.Text;
                                    utility.DO_UpdateParameters(filtri, _registroCC);
                                    DocsPAWA.DocsPaWR.Registro reg = wws.GetRegistroBySistemId(ddl_regCC.SelectedValue);
                                    ProspettiRiepilogativi.Frontend.Parametro _regCodice = new ProspettiRiepilogativi.Frontend.Parametro();
                                    _regCodice.Nome = "codReg";
                                    _regCodice.Descrizione = reg.codRegistro;
                                    utility.DO_UpdateParameters(filtri, _regCodice);
                                }
                                if (this.pnl_DocSpediti.Visible == true)
                                {
                                    this.setParamReportDocSpediti(ut);
                                }
                                if (panel_tit_ann_fasc.Visible == true)
                                {
                                    ProspettiRiepilogativi.Frontend.Parametro _titolario = new ProspettiRiepilogativi.Frontend.Parametro("titolario", ddl_titolari_report_annuale.Text, ddl_titolari_report_annuale.SelectedValue);
                                    utility.DO_UpdateParameters(filtri, _titolario);
                                }
                                if (rb_prospettiRiepilogativi.SelectedValue.Equals("reportAnnualeFasc"))
                                {
                                    ProspettiRiepilogativi.Frontend.Parametro _titolario = new ProspettiRiepilogativi.Frontend.Parametro("titolario", "Tutti i titolari", "0");
                                    utility.DO_UpdateParameters(filtri, _titolario);
                                }

                                string templateFilePath = "";

                                //elisa 02/02/2006
                                //se l'amministrazione non ha i protocolli interni,
                                //l'aaplicazione prende i template xml senza la colonna dei protocolli interni.
                                DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                                bool protoInterno = ws.IsInternalProtocolEnabled(cr.idAmministrazione);
                                if (protoInterno)
                                {
                                    templateFilePath = Server.MapPath("../Frontend/TemplateXML/");
                                }
                                else
                                {
                                    templateFilePath = Server.MapPath("../Frontend/TemplateXML_NO_Interni/");
                                }

                                switch (rb_prospettiRiepilogativi.SelectedValue)
                                {
                                    #region reportAnnualeDoc
                                    case "reportAnnualeDoc":
                                        try
                                        {

                                            if (ddl_Mese.SelectedValue.ToString() != "")
                                            {
                                                templateFilePath += "R_mensileByReg.xml";
                                            }
                                            else
                                            {
                                                templateFilePath += "R_annualeByReg.xml";
                                            }

                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.Annuale_By_Registro, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                                        }

                                        if (fileRep != null)
                                        {
                                            utility.setSelectedFileReport(this, fileRep, "");
                                        }
                                        else
                                        {
                                            utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            utility.do_openinRightFrame(this, "whitepage.htm");

                                        }
                                        break;
                                    #endregion

                                    #region reportDocClassificati
                                    case "reportDocClassificati":
                                        try
                                        {
                                            if (rb_modalita.SelectedValue == "Compatta")
                                            {
                                                templateFilePath += "R_DocClassCompact.xml";
                                                fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.Documenti_Classificati, filtri, idPeople, timeStamp);
                                            }
                                            else
                                            {
                                                templateFilePath += "R_DocClass.xml";
                                                fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.Documenti_Classificati, filtri, idPeople, timeStamp);
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                                        }

                                        if (fileRep != null)
                                        {
                                            utility.setSelectedFileReport(this, fileRep, "");
                                        }
                                        else
                                        {
                                            utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            utility.do_openinRightFrame(this, "whitepage.htm");
                                        }
                                        break;
                                    #endregion

                                    #region reportAnnualeDocTrasmAmm
                                    case "reportAnnualeDocTrasmAmm":
                                        //Documenti trasmessi ad altre amministrazioni
                                        try
                                        {
                                            templateFilePath += "R_DocTrasmToAOO.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.Documenti_Trasmessi_Altre_AOO, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                                        }

                                        if (fileRep != null)
                                        {
                                            utility.setSelectedFileReport(this, fileRep, "");
                                        }
                                        else
                                        {
                                            //utility.do_openinRightFrame(this,"whitepage.htm");
                                            utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            utility.do_openinRightFrame(this, "whitepage.htm");
                                        }
                                        break;
                                    #endregion

                                    #region reportAnnualeFasc
                                    case "reportAnnualeFasc":
                                        try
                                        {
                                            templateFilePath += "R_annualeByFasc.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.Annuale_By_Fascicolo, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                                        }

                                        if (fileRep != null)
                                        {
                                            utility.setSelectedFileReport(this, fileRep, "");

                                        }
                                        else
                                        {
                                            utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            utility.do_openinRightFrame(this, "whitepage.htm");
                                        }
                                        break;
                                    #endregion

                                    #region reportAnnualeFascXTit
                                    case "reportAnnualeFascXTit":
                                        try
                                        {
                                            if (rb_modalita.SelectedValue == "Compatta")
                                            {
                                                templateFilePath += "R_FascPerVTCompact.xml";
                                                fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.Fascicoli_Per_VT, filtri, idPeople, timeStamp);
                                            }
                                            else
                                            {
                                                templateFilePath += "R_FascPerVT.xml";
                                                fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.Fascicoli_Per_VT, filtri, idPeople, timeStamp);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                                        }

                                        if (fileRep != null)
                                        {
                                            utility.setSelectedFileReport(this, fileRep, "");
                                        }
                                        else
                                        {
                                            utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            utility.do_openinRightFrame(this, "whitepage.htm");
                                        }
                                        break;
                                    #endregion

                                    #region tempiMediLavFasc
                                    case "tempiMediLavFasc":
                                        try
                                        {
                                            if (rb_modalita.SelectedValue != "Compatta")
                                            {
                                                templateFilePath += "R_TempiMediLavFasc.xml";
                                                fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.TempiMediLavFascicoli, filtri, idPeople, timeStamp);
                                            }
                                            else
                                            {
                                                templateFilePath += "R_TempiMediLavFascCompact.xml";
                                                fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.TempiMediLavFascicoli, filtri, idPeople, timeStamp);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                                        }

                                        if (fileRep != null)
                                        {
                                            utility.setSelectedFileReport(this, fileRep, "");
                                        }
                                        else
                                        {
                                            utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            utility.do_openinRightFrame(this, "whitepage.htm");
                                        }
                                        break;
                                    #endregion

                                    #region ReportDocXSede
                                    case "ReportDocXSede":
                                        try
                                        {
                                            templateFilePath += "R_DocXSede.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportDocXSede, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                                        }

                                        if (fileRep != null)
                                        {
                                            utility.setSelectedFileReport(this, fileRep, "");
                                        }
                                        else
                                        {
                                            utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            utility.do_openinRightFrame(this, "whitepage.htm");
                                        }
                                        break;
                                    #endregion

                                    #region ReportDocXUo
                                    case "ReportDocXUo":
                                        try
                                        {
                                            templateFilePath += "R_DocXUo.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportDocXUo, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                                        }

                                        if (fileRep != null)
                                        {
                                            utility.setSelectedFileReport(this, fileRep, "");
                                        }
                                        else
                                        {
                                            utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            utility.do_openinRightFrame(this, "whitepage.htm");
                                        }
                                        break;

                                    #endregion

                                    #region ReportContatoriDoc
                                    case "reportContatoriDocumento":
                                        try
                                        {
                                            templateFilePath += "R_ContatoriDoc.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportContatoriDocumento, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                                        }
                                        if (fileRep != null)
                                        {
                                            utility.setSelectedFileReport(this, fileRep, "");
                                        }
                                        else
                                        {
                                            utility.do_alert(this, "Non ci sono dati per il Rapporto contatori documento");
                                            utility.do_openinRightFrame(this, "whitepage.htm");
                                        }
                                        break;
                                    #endregion

                                    #region ReportContatoriFasc
                                    case "reportContatoriFascicolo":
                                        try
                                        {
                                            templateFilePath += "R_ContatoriFasc.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportContatoriFascicolo, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                                        }
                                        if (fileRep != null)
                                        {
                                            utility.setSelectedFileReport(this, fileRep, "");
                                        }
                                        else
                                        {
                                            utility.do_alert(this, "Non ci sono dati per il Rapporto contatori fascicolo");
                                            utility.do_openinRightFrame(this, "whitepage.htm");
                                        }
                                        break;
                                    #endregion

                                    #region ReportStampaProtArma
                                    case "stampaProtArma":
                                        try
                                        {
                                            templateFilePath += "R_ProtocolloArma.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportProtocolloArma, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                                        }
                                        if (fileRep != null)
                                        {
                                            utility.setSelectedFileReport(this, fileRep, "");
                                        }
                                        else
                                        {
                                            utility.do_alert(this, "Non ci sono dati per la Stampa Protocollo Arma");
                                            utility.do_openinRightFrame(this, "whitepage.htm");
                                        }
                                        break;

                                    #endregion

                                    #region ReportDettaglioPratica
                                    case "stampaDettaglioPratica":
                                        try
                                        {
                                            templateFilePath += "R_DettaglioPratica.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportDettaglioPratica, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                                        }
                                        if (fileRep != null)
                                        {
                                            utility.setSelectedFileReport(this, fileRep, "");
                                        }
                                        else
                                        {
                                            utility.do_alert(this, "Non ci sono dati per la Stampa Dettaglio Protocollo");
                                            utility.do_openinRightFrame(this, "whitepage.htm");
                                        }
                                        break;
                                    #endregion

                                    #region ReportStampaGiornaleRiscontri
                                    case "stampaGiornaleRiscontri":
                                        try
                                        {
                                            templateFilePath += "R_GiornaleRiscontri.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportGiornaleRiscontri, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                                        }
                                        if (fileRep != null)
                                        {
                                            utility.setSelectedFileReport(this, fileRep, "");
                                        }
                                        else
                                        {
                                            utility.do_alert(this, "Non ci sono dati per la Stampa del Giornale Riscontri");
                                            utility.do_openinRightFrame(this, "whitepage.htm");
                                        }
                                        break;
                                    #endregion

                                    #region ReportDocumentiSpediti
                                    case "documentiSpediti":
                                        try
                                        {
                                            templateFilePath += "R_DocSpediti.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportDocSpeditiInterop, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                                        }
                                        if (fileRep != null)
                                        {
                                            utility.setSelectedFileReport(this, fileRep, "");
                                        }
                                        else
                                        {
                                            utility.do_alert(this, "Non ci sono dati per la Stampa dei documenti spediti per interoperabilità");
                                            utility.do_openinRightFrame(this, "whitepage.htm");
                                        }
                                        break;
                                    #endregion
                                }
                                btn_zoom.Visible = true;
                                this.btn_zoom.Enabled = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
            //Stampa report CDC
            stampaReportCDC();
        }


        /// <summary>
        /// Esegue JS
        /// </summary>
        /// <param name="key"></param>
        /*private void executeJS(string key)
        {
            if (!this.Page.IsStartupScriptRegistered("theJS"))
                this.Page.RegisterStartupScript("theJS", key);
        }*/
        #endregion

        #region ddl_amm_SelectedIndexChanghed
        /// <summary>
        /// ddl_amm_SelectedIndexChanghed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddl_amm_SelectedIndexChanghed(object sender, System.EventArgs e)
        {
            try
            {
                if (ddl_amm.SelectedIndex != 0)
                {
                    ProspettiRiepilogativi.Frontend.Parametro _amm = new ProspettiRiepilogativi.Frontend.Parametro("amm", ddl_amm.SelectedItem.Text, ddl_amm.SelectedItem.Value);

                    if (Session["filtri"] == null)
                    {
                        filtri.Add(_amm);
                        Session.Add("filtri", filtri);
                    }
                    else
                    {
                        filtri = (ArrayList)Session["filtri"];
                        utility.DO_UpdateParameters(filtri, _amm);
                        Session.Add("filtri", filtri);
                    }
                    string amm = ddl_amm.SelectedValue;
                    Controller.DO_GetRegistriByAmministrazione(ddl_registro, amm);
                    utility.SetFocus(this, ddl_registro);
                }
                else
                {
                    ddl_registro.Items.Clear();
                    ddl_anno.Items.Clear();
                    ddl_sede.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }
        #endregion

        #region ddl_registro_SelectedIndexChanged
        /// <summary>
        /// ddl_registro_SelectedIndexChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddl_registro_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (ddl_registro.SelectedIndex != 0)
                {
                    ProspettiRiepilogativi.Frontend.Parametro reg = new ProspettiRiepilogativi.Frontend.Parametro("reg", ddl_registro.SelectedItem.Text, ddl_registro.SelectedItem.Value);

                    if (Session["filtri"] != null)
                    {
                        filtri = (ArrayList)Session["filtri"];
                        utility.DO_UpdateParameters(filtri, reg);
                        Session["filtri"] = filtri;
                    }
                    int idReg = Convert.ToInt32(ddl_registro.SelectedValue);
                    Controller.DO_GetAnniProfilazione(ddl_anno, idReg);
                    // verifico presenza Sedi, se presenti abilito la combobox e aggiungo
                    //il filtro di ricerca.
                    bool popolata = false;
                    if (panel_sede.Visible == true)
                    {
                        Controller.DO_GetSedi(Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(ddl_amm.SelectedValue)), ddl_sede, out popolata);
                        utility.SetFocus(this, ddl_sede);
                    }
                    else
                    {
                        utility.SetFocus(this, ddl_anno);
                    }
                }
                else
                {
                    ddl_anno.Items.Clear();
                    ddl_sede.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }
        #endregion

        #region ddl_anno_SelectedIndexChanged
        /// <summary>
        /// ddl_anno_SelectedIndexChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddl_anno_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (ddl_anno.SelectedIndex != 0)
                {
                    Controller.DO_GETMesi(ddl_Mese);
                    ProspettiRiepilogativi.Frontend.Parametro _anno = new ProspettiRiepilogativi.Frontend.Parametro("anno", ddl_anno.SelectedItem.Text, ddl_anno.SelectedItem.Value);

                    if (Session["filtri"] != null)
                    {
                        filtri = (ArrayList)Session["filtri"];
                        utility.DO_UpdateParameters(filtri, _anno);
                        Session["filtri"] = filtri;
                        utility.SetFocus(this, btnStampa);
                    }
                }
                else
                {
                    ddl_sede.Items.Clear();
                }
                utility.SetFocus(this, btnStampa);
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }
        #endregion

        #region ddl_sede_SelectedIndexChanged


        private void ddl_sede_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                ProspettiRiepilogativi.Frontend.Parametro _sede = new ProspettiRiepilogativi.Frontend.Parametro("sede", ddl_sede.SelectedItem.Text, ddl_sede.SelectedItem.Value);

                if (Session["filtri"] != null)
                {
                    filtri = (ArrayList)Session["filtri"];
                    utility.DO_UpdateParameters(filtri, _sede);
                    Session["filtri"] = filtri;
                    utility.SetFocus(this, ddl_anno);
                }
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }


        #endregion

        #region btn_zoom_Click
        /// <summary>
        /// btn_zoom_Click: mostra il report a tutto schermo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_zoom_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                string url = "ProspettiRiepilogativi_RF.aspx";
                utility.do_windowOpen(this, url);
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }
        #endregion

        #region rb_prospettiRiepilogativi_SelectedIndexChanged
        /// <summary>
        /// rb_prospettiRiepilogativi_SelectedIndexChanged:
        /// abilita/disabilita il pannello contenente il 
        /// filtro della sede. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rb_prospettiRiepilogativi_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                this.btn_zoom.Visible = true;

                utility.do_openinRightFrame(this, "whitepage.htm");

                utility.RemoveFileReport(this);
                string _value = rb_prospettiRiepilogativi.SelectedValue;
                this.pnl_sottoposto.Visible = false;
                if ((_value != "reportAnnualeDoc") && (_value != "reportDocClassificati") && (_value != "tempiMediLavFasc"))
                {
                    panel_sede.Visible = false;
                }
                else
                {
                    panel_sede.Visible = true;
                }
                if ((_value == "reportAnnualeFascXTit") || (_value == "reportDocClassificati") || (_value == "tempiMediLavFasc"))
                {
                    pnl_modalita.Visible = true;
                }
                else
                {
                    pnl_modalita.Visible = false;
                }

                if ((_value == "reportAnnualeDoc") || (_value == "reportAnnualeFasc"))
                {
                    pnl_mese.Visible = true;
                }
                else
                {
                    pnl_mese.Visible = false;
                }
                if (_value == "reportContatoriDocumento" || _value == "reportContatoriFascicolo")
                {
                    panel_reg.Visible = false;
                }
                else
                {
                    panel_reg.Visible = true;
                }
                if (_value == "stampaProtArma" || _value == "stampaDettaglioPratica" || _value == "stampaGiornaleRiscontri" || _value == "documentiSpediti" || _value == "reportNumFasc" || _value == "reportNumDocInFasc")
                {
                    pnl_protArma.Visible = true;
                    pnl_regCC.Visible = true;
                    pnl_anno.Visible = false;
                    pnl_amm.Visible = false;
                    this.panel_reg.Visible = false;
                    this.pnl_DettPratica.Visible = false;
                    this.pnl_DocSpediti.Visible = false;
                    if (_value == "stampaDettaglioPratica")
                        this.pnl_DettPratica.Visible = true;
                    if (_value == "stampaGiornaleRiscontri")
                        pnl_protArma.Visible = false;
                    if (_value == "documentiSpediti")
                    {
                        pnl_protArma.Visible = false;
                        this.pnl_DocSpediti.Visible = true;
                    }
                    if (_value == "reportNumFasc" || _value == "reportNumDocInFasc")
                    {
                        pnl_protArma.Visible = false;
                        pnl_regCC.Visible = false;
                        pnl_anno.Visible = false;
                        pnl_amm.Visible = false;
                        this.panel_reg.Visible = false;
                        this.pnl_DettPratica.Visible = false;
                        this.pnl_DocSpediti.Visible = false;
                        this.pnl_sottoposto.Visible = true;
                        this.pnl_scelta_uo.Visible = true;
                        this.pnl_scelta_rf.Visible = false;
                        this.rb_scelta_sott.SelectedValue = "UO";
                        this.btn_img_sott_rubr.Visible = true;
                        //Inserisci uo 
                        DocsPAWA.DocsPaWR.InfoUtente infoUtente = DocsPAWA.UserManager.getInfoUtente(this);
                        Ruolo ruoloTipologia = DocsPAWA.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo, this);
                        if (ruoloTipologia != null && ruoloTipologia.uo != null)
                        {
                            this.txt2_corr_sott.Text = ruoloTipologia.uo.descrizione;
                            this.txt1_corr_sott.Text = ruoloTipologia.uo.codice;
                        }
                        this.btn_zoom.Visible = false;
                    }
                }
                else
                {
                    this.pnl_regCC.Visible = false;
                    this.panel_reg.Visible = true;
                    this.pnl_amm.Visible = true;
                    this.pnl_anno.Visible = true;
                    this.pnl_protArma.Visible = false;
                    this.pnl_DettPratica.Visible = false;
                    this.pnl_DocSpediti.Visible = false;
                }

                if (_value == "reportAnnualeFascXTit" || _value == "reportDocClassificati" || _value == "reportAnnualeDoc" || _value == "tempiMediLavFasc")
                {
                    panel_tit_ann_fasc.Visible = true;
                }
                else
                {
                    panel_tit_ann_fasc.Visible = false;
                }

                //Controllo dei report per CDC
                setPanelReportCDC();

                caricaComboTitolariReportTitolario();
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }
        #endregion

        #region DO_SetFormControl
        #region accessoSingolo
        /// <summary>
        /// DO_SetFormControl: setta il focus sul
        /// controllo opportuno dopo aver caricato
        /// tutti i dati possibili
        /// </summary>
        private void DO_SetFormControl()
        {
            ArrayList filtri = new ArrayList();
            try
            {
                rb_prospettiRiepilogativi.SelectedIndex = 0;
                //elisa 09-01-2006
                //if(ddl_amm.Items.Count == 2)
                if (ddl_amm.Items.Count >= 2)
                {
                    ddl_amm.SelectedIndex = 1;
                    string amm = ddl_amm.SelectedValue;

                    ProspettiRiepilogativi.Frontend.Parametro _amm = new ProspettiRiepilogativi.Frontend.Parametro("amm", ddl_amm.SelectedItem.Text, ddl_amm.SelectedItem.Value);
                    filtri.Add(_amm);

                    Controller.DO_GetRegistriByAmministrazione(ddl_registro, amm);
                    utility.SetFocus(this, ddl_registro);
                }
                //if(ddl_registro.Items.Count == 2)
                if (ddl_registro.Items.Count >= 2)
                {
                    ddl_registro.SelectedIndex = 1;
                    int idReg = Convert.ToInt32(ddl_registro.SelectedValue);

                    ProspettiRiepilogativi.Frontend.Parametro reg = new ProspettiRiepilogativi.Frontend.Parametro("reg", ddl_registro.SelectedItem.Text, ddl_registro.SelectedItem.Value);
                    filtri.Add(reg);

                    Controller.DO_GetAnniProfilazione(ddl_anno, idReg);
                    // verifico presenza Sedi, se presenti abilito la combobox e aggiungo
                    //il filtro di ricerca.
                    bool popolata = false;
                    Controller.DO_GetSedi(Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(ddl_amm.SelectedValue)), ddl_sede, out popolata);
                    if (popolata)
                    {
                        ProspettiRiepilogativi.Frontend.Parametro _sede = new ProspettiRiepilogativi.Frontend.Parametro("sede", ddl_sede.SelectedItem.Text, ddl_sede.SelectedItem.Value);
                        filtri.Add(_sede);

                        utility.SetFocus(this, ddl_sede);
                    }
                    else
                    {
                        utility.SetFocus(this, ddl_anno);
                    }
                }
                //if(ddl_anno.Items.Count == 2)
                if (ddl_anno.Items.Count >= 2)
                {
                    ddl_anno.SelectedIndex = 1;

                    ProspettiRiepilogativi.Frontend.Parametro _anno = new ProspettiRiepilogativi.Frontend.Parametro("anno", ddl_anno.SelectedItem.Text, ddl_anno.SelectedItem.Value);
                    filtri.Add(_anno);

                    utility.SetFocus(this, btnStampa);
                }
                Session.Add("filtri", filtri);
                Session.Add("SetFormControl", true);
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }
        #endregion

        #region accesso da DocsPA
        private void DO_SetControlFromDocsPA(DocsPAWA.DocsPaWR.Utente _utente)
        {
            try
            {
                //Selezioniamo il primo report
                rb_prospettiRiepilogativi.SelectedIndex = 0;

                DocsPAWA.DocsPaWR.PR_Amministrazione amm = Controller.Do_GetAmmByIdAmm(Convert.ToInt32(_utente.idAmministrazione));
                ddl_amm.Items.Clear();
                ddl_amm.Items.Add("");
                ddl_amm.Items.Add(new ListItem(amm.Descrizione, amm.Codice));

                ddl_amm.SelectedIndex = 1;
                ddl_amm.Enabled = false;

                ProspettiRiepilogativi.Frontend.Parametro _amm = new ProspettiRiepilogativi.Frontend.Parametro("amm", ddl_amm.SelectedItem.Text, ddl_amm.SelectedItem.Value);
                filtri.Add(_amm);

                /*
                * Dobbiamo usare il registro dell'utente 
                */
                ddl_registro.Items.Clear();
                ddl_registro.Items.Add("");
                for (int i = 0; i < _utente.ruoli.Length; i++)
                {
                    DocsPAWA.DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)_utente.ruoli[i];
                    for (int j = 0; j < ruolo.registri.Length; j++)
                    {
                        DocsPAWA.DocsPaWR.Registro reg = (DocsPAWA.DocsPaWR.Registro)ruolo.registri[j];
                        if (!DO_VerifyList(ddl_registro, reg.systemId))
                        {
                            ddl_registro.Items.Add(new ListItem(reg.descrizione, reg.systemId));
                        }
                        //ddl_registro.SelectedIndex = 1;
                    }
                }
                //elisa 04/01/2006
                //if(ddl_registro.Items.Count == 2)
                if (ddl_registro.Items.Count >= 2)
                {
                    ddl_registro.SelectedIndex = 1;
                    int idReg = Convert.ToInt32(ddl_registro.SelectedValue);

                    ProspettiRiepilogativi.Frontend.Parametro reg = new ProspettiRiepilogativi.Frontend.Parametro("reg", ddl_registro.SelectedItem.Text, ddl_registro.SelectedItem.Value);
                    filtri.Add(reg);

                    Controller.DO_GetAnniProfilazione(ddl_anno, idReg);
                    // verifico presenza Sedi, se presenti abilito la combobox e aggiungo
                    //il filtro di ricerca.
                    bool popolata = false;
                    Controller.DO_GetSedi(Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(ddl_amm.SelectedValue)), ddl_sede, out popolata);
                    if (popolata)
                    {
                        //						if(ddl_sede.SelectedIndex != 0)
                        //						{
                        ProspettiRiepilogativi.Frontend.Parametro _sede = new ProspettiRiepilogativi.Frontend.Parametro("sede", ddl_sede.SelectedItem.Text, ddl_sede.SelectedItem.Value);
                        filtri.Add(_sede);
                        //						}
                    }
                    else
                    {
                        utility.SetFocus(this, ddl_anno);
                    }
                }
                if (ddl_anno.Items.Count > 1)
                {
                    ddl_anno.SelectedIndex = ddl_anno.Items.Count - 1;

                    ProspettiRiepilogativi.Frontend.Parametro _anno = new ProspettiRiepilogativi.Frontend.Parametro("anno", ddl_anno.SelectedItem.Text, ddl_anno.SelectedItem.Value);
                    filtri.Add(_anno);

                    utility.SetFocus(this, btnStampa);
                }

                Controller.DO_GETMesi(ddl_Mese);
                Session.Add("filtri", filtri);
                Session.Add("SetFormControl", true);
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }
        #endregion

        private bool DO_VerifyList(DropDownList list, string system_id)
        {
            bool result = false;
            for (int i = 0; i < list.Items.Count; i++)
            {
                if (system_id == list.Items[i].Value)
                {
                    result = true;
                    return result;
                }
            }
            return result;
        }

        #endregion

        #region rb_modalita_SelectedIndexChanged
        /// <summary>
        /// rb_modalita_SelectedIndexChanged:
        /// selezione della modalità del rapporto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rb_modalita_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Session["filtri"] != null)
            {
                filtri = (ArrayList)Session["filtri"];
                ProspettiRiepilogativi.Frontend.Parametro _mod = new ProspettiRiepilogativi.Frontend.Parametro();
                _mod.Nome = "mod";
                _mod.Descrizione = "modalità";
                if (rb_modalita.SelectedValue == "Compatta")
                {
                    _mod.Valore = "Compatta";
                }
                else
                {
                    _mod.Valore = "Estesa";
                }
                utility.DO_UpdateParameters(filtri, _mod);
            }
        }
        #endregion

        #region ddl_Mese_SelectedIndexChanged
        /// <summary>
        /// ddl_Mese_SelectedIndexChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddl_Mese_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Session["filtri"] != null)
            {
                filtri = (ArrayList)Session["filtri"];
                ProspettiRiepilogativi.Frontend.Parametro _mese = new ProspettiRiepilogativi.Frontend.Parametro();
                _mese.Nome = "mese";
                _mese.Valore = ddl_Mese.SelectedValue;
                utility.DO_UpdateParameters(filtri, _mese);
            }
        }
        #endregion

        #region ddl_et_titolario_SelectedIndexChanged
        private void ddl_et_titolario_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Session["filtri"] != null)
            {
                filtri = (ArrayList)Session["filtri"];
                ProspettiRiepilogativi.Frontend.Parametro _titolario = new ProspettiRiepilogativi.Frontend.Parametro("titolario", ddl_et_titolario.SelectedItem.Text, ddl_et_titolario.SelectedValue);
                utility.DO_UpdateParameters(filtri, _titolario);
            }
        }
        #endregion

        protected void txt_NumPratica_TextChanged(object sender, System.EventArgs e)
        {
            string idTitolario = ddl_et_titolario.SelectedValue;
            string numProtPtratica = this.txt_NumPratica.Text;
            string idReg = this.ddl_registro.SelectedValue;
            DocsPAWA.DocsPaWR.Registro reg = wws.GetRegistroBySistemId(idReg);
            ArrayList nodoTitolario = new ArrayList(wws.getNodiFromProtoTit(reg, ((DocsPAWA.DocsPaWR.Utente)Session["userData"]).idAmministrazione, numProtPtratica, idTitolario));
            if (Session["filtri"] != null)
            {
                filtri = (ArrayList)Session["filtri"];
                ProspettiRiepilogativi.Frontend.Parametro _pratica = new ProspettiRiepilogativi.Frontend.Parametro();//("pratica", numProtPtratica);
                _pratica.Nome = "pratica";
                _pratica.Valore = numProtPtratica;
                ProspettiRiepilogativi.Frontend.Parametro _classifica = new ProspettiRiepilogativi.Frontend.Parametro();//("classifica", ((DocsPAWA.DocsPaWR.OrgNodoTitolario)nodoTitolario[0]).Codice);
                _classifica.Nome = "classifica";
                if (nodoTitolario.Count > 0)
                {
                    _classifica.Valore = ((DocsPAWA.DocsPaWR.OrgNodoTitolario)nodoTitolario[0]).Codice;
                }
                else
                {
                    _classifica.Valore = string.Empty;
                }
                utility.DO_UpdateParameters(filtri, _pratica);
                Session["filtri"] = filtri;
                filtri = (ArrayList)Session["filtri"];
                utility.DO_UpdateParameters(filtri, _classifica);
                Session["filtri"] = filtri;
            }
        }

        private void ddl_regCC_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Session["filtri"] != null)
            {
                filtri = (ArrayList)Session["filtri"];
                ProspettiRiepilogativi.Frontend.Parametro _registroCC = new ProspettiRiepilogativi.Frontend.Parametro();
                _registroCC.Nome = "registroCC";
                _registroCC.Valore = ddl_regCC.SelectedValue;
                _registroCC.Descrizione = ddl_regCC.SelectedItem.Text;
                utility.DO_UpdateParameters(filtri, _registroCC);
                DocsPAWA.DocsPaWR.Registro reg = wws.GetRegistroBySistemId(ddl_regCC.SelectedValue);
                ProspettiRiepilogativi.Frontend.Parametro _regCodice = new ProspettiRiepilogativi.Frontend.Parametro();
                _regCodice.Nome = "codReg";
                _regCodice.Descrizione = reg.codRegistro;
                utility.DO_UpdateParameters(filtri, _regCodice);
            }
        }

        private void caricaDdlRegCC(DropDownList ddlRegCC)
        {
            ddlRegCC.Items.Clear();
            //ddlRegCC.Items.Add(new ListItem("Tutti", ""));
            for (int i = 0; i < _utente.ruoli.Length; i++)
            {
                DocsPAWA.DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)_utente.ruoli[i];
                for (int j = 0; j < ruolo.registri.Length; j++)
                {
                    DocsPAWA.DocsPaWR.Registro reg = (DocsPAWA.DocsPaWR.Registro)ruolo.registri[j];
                    if (!DO_VerifyList(ddlRegCC, reg.systemId))
                    {
                        ddlRegCC.Items.Add(new ListItem(reg.descrizione, reg.systemId));
                    }
                    //ddl_registro.SelectedIndex = 1;
                }
            }
        }

        private void setParamReportDocSpediti(DocsPAWA.DocsPaWR.Utente ut)
        {
            filtri = (ArrayList)Session["filtri"];
            ProspettiRiepilogativi.Frontend.Parametro _confermaProt = new ProspettiRiepilogativi.Frontend.Parametro();
            _confermaProt.Nome = "confermaProt";
            _confermaProt.Valore = rb_confermaSpedizione.SelectedValue;
            utility.DO_UpdateParameters(filtri, _confermaProt);
            ProspettiRiepilogativi.Frontend.Parametro _dataSpedDa = new ProspettiRiepilogativi.Frontend.Parametro();
            _dataSpedDa.Nome = "dataSpedDa";
            _dataSpedDa.Valore = this.txt_dataSpedDa.Text;
            utility.DO_UpdateParameters(filtri, _dataSpedDa);
            ProspettiRiepilogativi.Frontend.Parametro _dataSpedA = new ProspettiRiepilogativi.Frontend.Parametro();
            _dataSpedA.Nome = "dataSpedA";
            if (this.txt_dataSpedA.Text != string.Empty)
            {
                _dataSpedA.Valore = this.txt_dataSpedA.Text;
            }
            else
            {
                System.DateTime now = System.DateTime.Now;
                CultureInfo ci = new CultureInfo("en-US");
                _dataSpedA.Valore = now.ToString("dd/MM/yyyy", ci);
                this.txt_dataSpedA.Text = _dataSpedA.Valore;
            }
            utility.DO_UpdateParameters(filtri, _dataSpedA);
            ProspettiRiepilogativi.Frontend.Parametro _confermeAttese = new ProspettiRiepilogativi.Frontend.Parametro();
            ProspettiRiepilogativi.Frontend.Parametro _totDocSpediti = new ProspettiRiepilogativi.Frontend.Parametro();
            ProspettiRiepilogativi.Frontend.Parametro _confermeMancanti = new ProspettiRiepilogativi.Frontend.Parametro();

            switch (_confermaProt.Valore)
            {
                case "1":
                    string confermeAttese = Controller.DO_GetNumDocSpediti(_dataSpedDa.Valore, _dataSpedA.Valore, ddl_regCC.SelectedValue, "1", Convert.ToInt32(ut.idAmministrazione));
                    //ProspettiRiepilogativi.Frontend.Parametro _confermeAttese = new ProspettiRiepilogativi.Frontend.Parametro();                    
                    _confermeAttese.Nome = "confermeAttese";
                    _confermeAttese.Valore = confermeAttese;
                    utility.DO_UpdateParameters(filtri, _confermeAttese);
                    // ProspettiRiepilogativi.Frontend.Parametro _totDocSpediti = new ProspettiRiepilogativi.Frontend.Parametro();
                    _totDocSpediti.Nome = "totDocSpediti";
                    _totDocSpediti.Valore = confermeAttese;
                    utility.DO_UpdateParameters(filtri, _totDocSpediti);
                    //ProspettiRiepilogativi.Frontend.Parametro _confermeMancanti = new ProspettiRiepilogativi.Frontend.Parametro();
                    _confermeMancanti.Nome = "confermeMancanti";
                    _confermeMancanti.Valore = "0";
                    utility.DO_UpdateParameters(filtri, _confermeMancanti);
                    break;
                case "0":
                    string confermaMancanti = Controller.DO_GetNumDocSpediti(_dataSpedDa.Valore, _dataSpedA.Valore, ddl_regCC.SelectedValue, "0", Convert.ToInt32(ut.idAmministrazione));
                    //ProspettiRiepilogativi.Frontend.Parametro _confermeMancanti = new ProspettiRiepilogativi.Frontend.Parametro();
                    _confermeMancanti.Nome = "confermeMancanti";
                    _confermeMancanti.Valore = confermaMancanti;
                    utility.DO_UpdateParameters(filtri, _confermeMancanti);
                    // ProspettiRiepilogativi.Frontend.Parametro _confermeAttese = new ProspettiRiepilogativi.Frontend.Parametro();
                    _confermeAttese.Nome = "confermeAttese";
                    _confermeAttese.Valore = "0";
                    utility.DO_UpdateParameters(filtri, _confermeAttese);
                    // ProspettiRiepilogativi.Frontend.Parametro _totDocSpediti = new ProspettiRiepilogativi.Frontend.Parametro();
                    _totDocSpediti.Nome = "totDocSpediti";
                    _totDocSpediti.Valore = confermaMancanti;
                    utility.DO_UpdateParameters(filtri, _totDocSpediti);
                    break;
                case "T":
                    string totDocumenti = Controller.DO_GetNumDocSpediti(_dataSpedDa.Valore, _dataSpedA.Valore, ddl_regCC.SelectedValue, "T", Convert.ToInt32(ut.idAmministrazione));
                    // ProspettiRiepilogativi.Frontend.Parametro _totDocSpediti = new ProspettiRiepilogativi.Frontend.Parametro();
                    _totDocSpediti.Nome = "totDocSpediti";
                    _totDocSpediti.Valore = totDocumenti;
                    utility.DO_UpdateParameters(filtri, _totDocSpediti);
                    string spedizioni = Controller.DO_GetNumDocSpediti(_dataSpedDa.Valore, _dataSpedA.Valore, ddl_regCC.SelectedValue, "1", Convert.ToInt32(ut.idAmministrazione));
                    // ProspettiRiepilogativi.Frontend.Parametro _confermeAttese = new ProspettiRiepilogativi.Frontend.Parametro();
                    _confermeAttese.Nome = "confermeAttese";
                    _confermeAttese.Valore = spedizioni;
                    utility.DO_UpdateParameters(filtri, _confermeAttese);
                    // ProspettiRiepilogativi.Frontend.Parametro _confermeMancanti = new ProspettiRiepilogativi.Frontend.Parametro();
                    _confermeMancanti.Nome = "confermeMancanti";
                    if (totDocumenti == "")
                        totDocumenti = "0";
                    if (spedizioni == "")
                        spedizioni = "0";
                    _confermeMancanti.Valore = Convert.ToString(Convert.ToInt32(totDocumenti) - Convert.ToInt32(spedizioni));
                    utility.DO_UpdateParameters(filtri, _confermeMancanti);
                    break;
            }

        }

        private void setRadioButtonList()
        {
            ArrayList reports = Controller.DO_ReadXML();

            rb_prospettiRiepilogativi.Items.Clear();
            foreach (DocsPAWA.DocsPaWR.Report r in reports)
            {
                if (r.Valore == "stampaProtArma" || r.Valore == "stampaDettaglioPratica" || r.Valore == "stampaGiornaleRiscontri" || r.Valore == "documentiSpediti")
                {
                    if (!string.IsNullOrEmpty(wws.isEnableProtocolloTitolario()))
                        rb_prospettiRiepilogativi.Items.Add(new ListItem(r.Descrizione, r.Valore));
                }
                else
                {
                    if ((r.Valore == "reportNumFasc" || r.Valore == "reportNumDocInFasc"))
                    {
                        if (DocsPAWA.UserManager.ruoloIsAutorized(this, "EXP_FASC_COUNT"))
                        {
                            rb_prospettiRiepilogativi.Items.Add(new ListItem(r.Descrizione, r.Valore));
                        }
                    }
                    else
                    {
                        rb_prospettiRiepilogativi.Items.Add(new ListItem(r.Descrizione, r.Valore));
                    }
                }
            }

            //Controllo dei report per CDC
            setRadioButtonListCDC();


            panel_sede.Visible = true;
        }

        private void setDdlAmministrazioni()
        {
            ddl_amm.Items.Add("");
            try
            {
                Frontend.Controller.DO_GetAmministrazioni(ddl_amm);
                utility.SetFocus(this, ddl_amm);
                DO_SetFormControl();
            }

            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }

            if (Session["filtri"] != null)
            {
                if (Session["SetFormControl"] == null)
                {
                    Session.Remove("filtri");
                }
            }
        }

        private void setStampaProtocolloArma()
        {
            if (!string.IsNullOrEmpty(wws.isEnableProtocolloTitolario()))
            {
                Frontend.Controller.DO_GetTitolario(ref this.ddl_et_titolario, ddl_amm.SelectedValue);
                if ((DocsPAWA.DocsPaWR.Utente)Session["userData"] == null)
                    Controller.DO_GetRegistriCCByAmministrazione(ddl_regCC, ddl_amm.SelectedValue);
                else
                    this.caricaDdlRegCC(ddl_regCC);
            }
        }

        #region metodi report CDC
        private void setRadioButtonListCDC()
        {
            Ruolo ruolo = DocsPAWA.UserManager.getRuolo(this);
            if (ruolo != null && ruolo.funzioni != null)
            {
                Funzione REPORT_CDC_GLOBALI = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_GLOBALI")).FirstOrDefault();
                Funzione REPORT_CDC_PER_UFFICIO = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_PER_UFFICIO")).FirstOrDefault();
                Funzione REPORT_CDC_DECRETI = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_DECRETI")).FirstOrDefault();

                //Funzioni per CDC SRC
                Funzione REPORT_CDC_GLOBALI_SRC = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_GLOBALI_SRC")).FirstOrDefault();
                Funzione REPORT_CDC_PER_UFFICIO_SRC = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_PER_UFFICIO_SRC")).FirstOrDefault();
                Funzione REPORT_CDC_DECRETI_SRC = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_DECRETI_SRC")).FirstOrDefault();


                if (REPORT_CDC_GLOBALI != null || REPORT_CDC_PER_UFFICIO != null)
                {
                    //Modifica Iacozzilli Giordano:
                    //Modifico il testo del rb per l'introduzione di nuovi report Sccla a controllo successivo:
                    //Old Code:
                    //rb_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. Centrali di Controllo (Controllo Preventivo)", "CDC_reportControlloPreventivo"));
                    //New Code:
                    rb_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. Centrali di Controllo", "CDC_reportControlloPreventivo"));
                    //FINE

                    // Questi report non sono più necessari
                    //rb_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. Centrali di Controllo (Pensioni Civili)", "CDC_reportPensioniCivili"));
                    //rb_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. Centrali di Controllo (Pensioni Militari)", "CDC_reportPensioniMilitari"));
                }

                if (REPORT_CDC_DECRETI != null)
                {
                    // Visualizzazione report "Decreti in esame" e "Decreti pervenuti in intervallo temporale"
                    rb_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. di Controllo (Scadenzario - Decreti in esame)", "CDC_reportDecretiInEsame"));
                    rb_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. di Controllo (Decreti pervenuti in intervallo temporale)", "CDC_reportDecretiPervenutiInIntTemporale"));

                    rb_prospettiRiepilogativi.Items.Add(new ListItem("Elenchi decreti registrati Uffici Sez. Centrale", "CDC_reportElencoDecretiSCCLA"));
                    rb_prospettiRiepilogativi.Items.Add(new ListItem("Elenchi decreti restituiti Uffici Sez. Centrale", "CDC_reportElencoDecretiRestituitiSCCLA"));
                    rb_prospettiRiepilogativi.Items.Add(new ListItem("Elenchi decreti restituiti con rilievo Uffici Sez. Centrale ", "CDC_reportElencoDecretiRestituitiConRilievoSCCLA"));
                }

                if (REPORT_CDC_GLOBALI_SRC != null || REPORT_CDC_PER_UFFICIO_SRC != null)
                {
                    rb_prospettiRiepilogativi.Items.Add(new ListItem("Report Sez. Regionali di Controllo (Controllo Preventivo)", "CDC_reportControlloPreventivoSRC"));
                    // Questi report non sono più necessari
                    //rb_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. Centrali di Controllo (Pensioni Civili)", "CDC_reportPensioniCivili"));
                    //rb_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. Centrali di Controllo (Pensioni Militari)", "CDC_reportPensioniMilitari"));
                }

                if (REPORT_CDC_DECRETI_SRC != null)
                {
                    // Visualizzazione report "Decreti in esame" e "Decreti pervenuti in intervallo temporale"
                    rb_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. di Controllo Reg. (Scadenzario - Decreti in esame)", "CDC_reportDecretiInEsameSRC"));
                    rb_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. di Controllo Reg. (Decreti pervenuti in intervallo temporale)", "CDC_reportDecretiPervenutiInIntTemporaleSRC"));

                    rb_prospettiRiepilogativi.Items.Add(new ListItem("Elenchi decreti registrati Sez. Regionale", "CDC_reportElencoDecretiSRC"));
                    rb_prospettiRiepilogativi.Items.Add(new ListItem("Elenchi decreti restituiti Sez. Regionale", "CDC_reportElencoDecretiRestituitiSRC"));
                    rb_prospettiRiepilogativi.Items.Add(new ListItem("Elenchi decreti restituiti con rilievo Sez. Regionale", "CDC_reportElencoDecretiRestituitiConRilievoSRC"));
                }
            }
        }

        private void setPanelReportCDC()
        {
            pnl_FiltriCDC.Visible = false;
            ((TextBox)FindControl("txt_Elenco")).Text = "";
            switch (rb_prospettiRiepilogativi.SelectedValue)
            {
                case "CDC_reportControlloPreventivo":
                case "CDC_reportControlloPreventivoSRC":
                case "CDC_reportPensioniCivili":
                case "CDC_reportPensioniMilitari":
                case "CDC_reportDecretiPervenutiInIntTemporale":
                    pnl_amm.Visible = false;
                    panel_reg.Visible = false;
                    panel_sede.Visible = false;
                    pnl_anno.Visible = false;
                    pnl_mese.Visible = false;
                    pnl_modalita.Visible = false;
                    pnl_regCC.Visible = false;
                    pnl_protArma.Visible = false;
                    pnl_DettPratica.Visible = false;
                    pnl_DocSpediti.Visible = false;
                    this.pnlContrData.Visible = true;
                    ((Panel)FindControl("pnl_FiltriCDCElencoDecreti")).Visible = false;
                    pnl_FiltriCDC.Visible = true;
                    showCorr();
                    resetElementsCDC();
                    setVisibilityElementsCDC();
                    setDdlReportCDC();
                    break;

                case "CDC_reportDecretiPervenutiInIntTemporaleSRC":
                    pnl_amm.Visible = false;
                    panel_reg.Visible = false;
                    panel_sede.Visible = false;
                    pnl_anno.Visible = false;
                    pnl_mese.Visible = false;
                    pnl_modalita.Visible = false;
                    pnl_regCC.Visible = false;
                    pnl_protArma.Visible = false;
                    pnl_DettPratica.Visible = false;
                    pnl_DocSpediti.Visible = false;
                    this.pnlContrData.Visible = true;

                    pnl_FiltriCDC.Visible = true;
                    ((Panel)FindControl("pnl_FiltriCDCElencoDecreti")).Visible = false;
                    showCorr();
                    resetElementsCDC();
                    setVisibilityElementsCDC();
                    setDdlReportCDC();
                    break;

                case "CDC_reportDecretiInEsame":
                    pnl_amm.Visible = false;
                    panel_reg.Visible = false;
                    panel_sede.Visible = false;
                    pnl_anno.Visible = false;
                    pnl_mese.Visible = false;
                    pnl_modalita.Visible = false;
                    pnl_regCC.Visible = false;
                    pnl_protArma.Visible = false;
                    pnl_DettPratica.Visible = false;
                    pnl_DocSpediti.Visible = false;
                    pnlContrData.Visible = false;
                    pnl_FiltriCDC.Visible = true;
                    ((Panel)FindControl("pnl_FiltriCDCElencoDecreti")).Visible = false;
                    showCorr();
                    resetElementsCDC();
                    setVisibilityElementsCDC();
                    setDdlReportCDC();

                    break;

                case "CDC_reportDecretiInEsameSRC":
                    pnl_amm.Visible = false;
                    panel_reg.Visible = false;
                    panel_sede.Visible = false;
                    pnl_anno.Visible = false;
                    pnl_mese.Visible = false;
                    pnl_modalita.Visible = false;
                    pnl_regCC.Visible = false;
                    pnl_protArma.Visible = false;
                    pnl_DettPratica.Visible = false;
                    pnl_DocSpediti.Visible = false;
                    pnlContrData.Visible = false;
                    pnl_FiltriCDC.Visible = true;
                    showCorr();
                    ((Panel)FindControl("pnl_FiltriCDCElencoDecreti")).Visible = false;

                    resetElementsCDC();
                    setVisibilityElementsCDC();
                    setDdlReportCDC();

                    break;

                case "CDC_reportElencoDecretiSRC":
                case "CDC_reportElencoDecretiRestituitiSRC":
                case "CDC_reportElencoDecretiRestituitiConRilievoSRC":
                case "CDC_reportElencoDecretiSCCLA":
                case "CDC_reportElencoDecretiRestituitiSCCLA":
                case "CDC_reportElencoDecretiRestituitiConRilievoSCCLA":
                    pnl_amm.Visible = false;
                    panel_reg.Visible = false;
                    panel_sede.Visible = false;
                    pnl_anno.Visible = false;
                    pnl_mese.Visible = false;
                    pnl_modalita.Visible = false;
                    pnl_regCC.Visible = false;
                    pnl_protArma.Visible = false;
                    pnl_DettPratica.Visible = false;
                    pnl_DocSpediti.Visible = false;

                    pnl_FiltriCDC.Visible = false;
                    this.pnlContrData.Visible = false;
                    ((Panel)FindControl("pnl_FiltriCDCElencoDecreti")).Visible = true;
                    hideCorr();
                    resetElementsCDC();
                    setVisibilityElementsCDC();
                    setDdlReportCDC();

                    break;
            }
            //***************************************************************
            //
            //Modifica CDC GIORDANO IACOZZILLI 15/06/2012
            //
            //Devo aggiungere altri 4 tipi di report per CDC
            //Per non modificare troppo il layout della pagina 
            //si è deciso di aggiungere un rblist nella parte dei parametri del report.
            //a seconda del Tipo di Controllo (successivo o preventivo) eseguo un diverso tipo di report.
            SetVisiblerblTipoControllo(rb_prospettiRiepilogativi.SelectedValue);
            //FINE Modifica CDC GIORDANO IACOZZILLI 15/06/2012
            //***************************************************************
        }

        /// <summary>
        /// Giordano Iacozzilli:
        /// Metodo che rende visibili o meno i panel contenenti il flag
        /// per la scelta del tipo si controllo (successivo o preventivo).
        /// </summary>
        /// <param name="p">rb_prospettiRiepilogativi.SelectedValue</param>
        private void SetVisiblerblTipoControllo(string p)
        {
            string[] reportPdf = new string[2] { "CDC_reportControlloPreventivo",
                                                 "CDC_reportDecretiPervenutiInIntTemporale" };

            string[] reporxls = new string[3] {"CDC_reportElencoDecretiSCCLA",
                                               "CDC_reportElencoDecretiRestituitiSCCLA",
                                               "CDC_reportElencoDecretiRestituitiConRilievoSCCLA"};
            if (reportPdf.Contains(p))
                ((Panel)FindControl("pnlTipoControlloContrData")).Visible = true;
            else
                ((Panel)FindControl("pnlTipoControlloContrData")).Visible = false;

            if (reporxls.Contains(p))
                ((Panel)FindControl("pnlControlloElencoDecreti")).Visible = true;
            else
                ((Panel)FindControl("pnlControlloElencoDecreti")).Visible = false;
        }

        private void hideCorr()
        {
            lbl_Revisore.Visible = false;
            lbl_Magistrato.Visible = false;
            corr_revisore.Visible = false;
            corr_magistrato.Visible = false;
        }

        private void showCorr()
        {
            lbl_Revisore.Visible = true;
            lbl_Magistrato.Visible = true;
            corr_revisore.Visible = true;
            corr_magistrato.Visible = true;
        }

        private void setVisibilityElementsCDC()
        {
            Ruolo ruolo = DocsPAWA.UserManager.getRuolo(this);
            if (ruolo != null && ruolo.funzioni != null)
            {
                Funzione REPORT_CDC_PER_UFFICIO = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_PER_UFFICIO")).FirstOrDefault();
                if (REPORT_CDC_PER_UFFICIO == null)
                {
                    lbl_Magistrato.Visible = false;
                    corr_magistrato.Visible = false;
                    lbl_Revisore.Visible = false;
                    corr_revisore.Visible = false;
                }
            }
        }

        private void setDdlReportCDC()
        {
            Ruolo ruolo = DocsPAWA.UserManager.getRuolo(this);
            idUffici = new Hashtable();
            DocsPaWebService ws = new DocsPaWebService();
            if (ruolo != null && ruolo.funzioni != null)
            {
                Funzione REPORT_CDC_GLOBALI = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_GLOBALI")).FirstOrDefault();
                Funzione REPORT_CDC_PER_UFFICIO = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_PER_UFFICIO")).FirstOrDefault();
                Funzione REPORT_CDC_DECRETI = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_DECRETI")).FirstOrDefault();

                //Funzioni per CDC SRC
                Funzione REPORT_CDC_GLOBALI_SRC = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_GLOBALI_SRC")).FirstOrDefault();
                Funzione REPORT_CDC_PER_UFFICIO_SRC = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_PER_UFFICIO_SRC")).FirstOrDefault();
                Funzione REPORT_CDC_DECRETI_SRC = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_DECRETI_SRC")).FirstOrDefault();

                //Report globali
                if (REPORT_CDC_GLOBALI != null || REPORT_CDC_GLOBALI_SRC != null)
                {
                    ddl_uffici.Items.Add(new ListItem("Tutti gli uffici", "TUTTI"));

                    Templates[] tipiAtto = (Templates[])DocsPAWA.ProfilazioneDocManager.getTemplates(ruolo.idAmministrazione, this).ToArray(typeof(Templates));
                    Templates tipoAtto = null;
                    ArrayList ruoliTipoAtto = null;

                    if (tipiAtto != null)
                    {
                        //***************************************************************
                        //
                        //Modifica CDC GIORDANO IACOZZILLI 15/06/2012
                        //
                        //Faccio una If per verificare quale report lanciare, se il successivo o il preventivo.
                        if (rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportControlloPreventivo"))
                        {
                            if (((RadioButtonList)FindControl("rblTipoControlloContrData")).SelectedItem.Value != "0")
                            {
                                //Successivo:
                                tipoAtto = tipiAtto.Where(template => template.DESCRIZIONE.ToUpper().Equals("CONTROLLO SUCCESSIVO SCCLA")).FirstOrDefault();
                            }
                            else
                                //Preventivo:
                                tipoAtto = tipiAtto.Where(template => template.DESCRIZIONE.ToUpper().Equals("CONTROLLO PREVENTIVO SCCLA")).FirstOrDefault();
                        }
                        //***************************************************************
                        //FINE
                        //***************************************************************
                        if (rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportPensioniCivili"))
                            tipoAtto = tipiAtto.Where(template => template.DESCRIZIONE.ToUpper().Equals("PENSIONI CIVILI SCCLA")).FirstOrDefault();

                        if (rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportPensioniMilitari"))
                            tipoAtto = tipiAtto.Where(template => template.DESCRIZIONE.ToUpper().Equals("PENSIONI MILITARI SCCLA")).FirstOrDefault();
                    }

                    if (tipoAtto != null)
                    {
                        ruoliTipoAtto = DocsPAWA.ProfilazioneDocManager.getRuoliTipoDoc(tipoAtto.SYSTEM_ID.ToString(), this);

                        AssDocFascRuoli[] ruoliAutorizzati = ((AssDocFascRuoli[])ruoliTipoAtto.ToArray(typeof(AssDocFascRuoli)));
                        ruoliAutorizzati = ruoliAutorizzati.Where(assRuolo => assRuolo.DIRITTI_TIPOLOGIA != null && assRuolo.DIRITTI_TIPOLOGIA != "0").ToArray<AssDocFascRuoli>();
                        ruoliTipoAtto = new ArrayList(ruoliAutorizzati);
                    }

                    if (ruoliTipoAtto != null)
                    {
                        if (rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportControlloPreventivo"))
                        {
                            //CONTROLLO PREVENTIVO SCCLA
                            foreach (AssDocFascRuoli assDocFascRuoli in ruoliTipoAtto)
                            {
                                if (!string.IsNullOrEmpty(assDocFascRuoli.DIRITTI_TIPOLOGIA) &&
                                    !assDocFascRuoli.DIRITTI_TIPOLOGIA.Equals("0"))
                                {
                                    Ruolo ruoloTipologia =
                                        DocsPAWA.UserManager.getRuoloByIdGruppo(assDocFascRuoli.ID_GRUPPO, this);
                                    if (ruoloTipologia != null && ruoloTipologia.uo != null)
                                    {
                                        string descrizioneUo = ruoloTipologia.uo.descrizione;
                                        string codiceUo = ruoloTipologia.uo.codice;

                                        if (!ddl_uffici.Items.Contains(new ListItem(descrizioneUo, codiceUo)))
                                            ddl_uffici.Items.Add(new ListItem(descrizioneUo, codiceUo));

                                        if (!idUffici.ContainsKey(codiceUo))
                                            idUffici.Add(codiceUo, ruoloTipologia.uo.systemId);

                                    }
                                }
                            }
                            Session.Add("hashTableUffici", idUffici);
                        }
                        //CONTROLLO PREVENTIVO SRC
                        else if (rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportControlloPreventivoSRC"))
                        {

                            //string idRegistro = DocsPAWA.UserManager.getRuolo(this).idRegistro;

                            DataSet ds_reg = ws.GetRegistriByTipologia("Controllo Preventivo SRC");
                            if (ds_reg != null)
                            {
                                foreach (DataRow row in ds_reg.Tables[0].Rows)
                                {
                                    if (!ddl_uffici.Items.Contains(new ListItem(row["VAR_DESC_REGISTRO"].ToString(), row["VAR_CODICE"].ToString())))
                                    {
                                        ddl_uffici.Items.Add(new ListItem(row["VAR_DESC_REGISTRO"].ToString(), row["VAR_CODICE"].ToString()));
                                    }

                                    if (!idUffici.ContainsKey(row["VAR_CODICE"].ToString()))
                                        idUffici.Add(row["VAR_CODICE"].ToString(), row["SYSTEM_ID"].ToString());
                                }
                                Session.Add("hashTableUffici", idUffici);
                            }

                        }

                    }
                }

                //Report per singolo ufficio
                if (REPORT_CDC_PER_UFFICIO != null && REPORT_CDC_GLOBALI == null)
                {
                    string descrizioneUo = DocsPAWA.UserManager.getRuolo(this).uo.descrizione;
                    string codiceUo = DocsPAWA.UserManager.getRuolo(this).uo.codice;

                    ddl_uffici.Items.Add(new ListItem(descrizioneUo, codiceUo));
                    ddl_uffici.Enabled = false;

                    if (!idUffici.ContainsKey(codiceUo))
                        idUffici.Add(codiceUo, DocsPAWA.UserManager.getRuolo(this).uo.systemId);

                    Session.Add("hashTableUffici", idUffici);
                }

                //Report per singolo ufficio SRC
                if (REPORT_CDC_PER_UFFICIO_SRC != null && REPORT_CDC_GLOBALI_SRC == null)
                {
                    //string descrizioneUo = DocsPAWA.UserManager.getRuolo(this).uo.descrizione;
                    //string codiceUo = DocsPAWA.UserManager.getRuolo(this).uo.codice;

                    Registro[] regs = DocsPAWA.UserManager.GetRegistriByRuolo(this,
                                                                              DocsPAWA.UserManager.getRuolo(this).
                                                                                  systemId);

                    string idRegistro = regs[0].systemId;

                    foreach (Registro reg in regs)
                    {
                        if (reg.systemId == idRegistro)
                        {
                            if (!ddl_uffici.Items.Contains(new ListItem(reg.descrizione, reg.codRegistro)))
                                ddl_uffici.Items.Add(new ListItem(reg.descrizione, reg.codRegistro));

                            if (!idUffici.ContainsKey(reg.codRegistro))
                                idUffici.Add(reg.codRegistro, reg.systemId);

                            break;
                        }
                    }

                    Session.Add("hashTableUffici", idUffici);
                }


                // Report per decreti ( in questo caso la combo deve contenere solo l'ufficio di appartenenza dell'utente)             
                if (REPORT_CDC_DECRETI != null && this.rb_prospettiRiepilogativi.SelectedValue == "CDC_reportDecretiInEsame" ||
                    this.rb_prospettiRiepilogativi.SelectedValue == "CDC_reportDecretiPervenutiInIntTemporale")
                {

                    string descrizioneUo = DocsPAWA.UserManager.getRuolo(this).uo.descrizione;
                    string codiceUo = DocsPAWA.UserManager.getRuolo(this).uo.codice;
                    this.ddl_uffici.Items.Clear();
                    ddl_uffici.Items.Add(new ListItem(descrizioneUo, codiceUo));
                    ddl_uffici.Enabled = false;
                }


                //************************************************************************************************************
                //MODIFICA IACOZZILLI GIORDANO 28/06/2012
                //
                //Sto creando nuovi report SCCLA Successivi e mi sono accorto che non è stato riportato un pezzo di 
                //codice dalla 316, il pezzo di codice metteva come default per il CDCUffici il codice dell'ufficio 
                //a cui appartiene il Ruolo che opera, mentre in caso contrario cè la dicitura TUTTI, solo che
                //laasciando la dicitura TUTTI non si entra in un blocco if (riga:2215) e il report va in errrore:
                //
                if (REPORT_CDC_DECRETI_SRC != null && this.rb_prospettiRiepilogativi.SelectedValue == "CDC_reportDecretiInEsameSRC" ||
                   this.rb_prospettiRiepilogativi.SelectedValue == "CDC_reportDecretiPervenutiInIntTemporaleSRC" ||
                   this.rb_prospettiRiepilogativi.SelectedValue == "CDC_reportElencoDecretiSRC" ||
                   this.rb_prospettiRiepilogativi.SelectedValue == "CDC_reportElencoDecretiRestituitiSRC" ||
                   this.rb_prospettiRiepilogativi.SelectedValue == "CDC_reportElencoDecretiRestituitiConRilievoSRC" ||
                   this.rb_prospettiRiepilogativi.SelectedValue == "CDC_reportElencoDecretiSCCLA" ||
                   this.rb_prospettiRiepilogativi.SelectedValue == "CDC_reportElencoDecretiRestituitiSCCLA" ||
                   this.rb_prospettiRiepilogativi.SelectedValue == "CDC_reportElencoDecretiRestituitiConRilievoSCCLA"
                   )
                {

                    string descrizioneUo = DocsPAWA.UserManager.getRuolo(this).uo.descrizione;
                    string codiceUo = DocsPAWA.UserManager.getRuolo(this).uo.codice;
                    this.ddl_uffici.Items.Clear();
                    ddl_uffici.Items.Add(new ListItem(descrizioneUo, codiceUo));
                    ddl_uffici.Enabled = false;
                }
                //****************************************************************************************************
                // FINE
                //****************************************************************************************************
            }
        }

        private void resetElementsCDC()
        {
            dataDa.Text = string.Empty;
            dataA.Text = string.Empty;
            ddl_uffici.Items.Clear();
            corr_magistrato.CODICE_TEXT = string.Empty;
            corr_magistrato.DESCRIZIONE_TEXT = string.Empty;
            corr_revisore.CODICE_TEXT = string.Empty;
            corr_revisore.DESCRIZIONE_TEXT = string.Empty;
        }

        private void stampaReportCDC()
        {
            //Modifica Giordano Iacozzilli:
            //15/06/2012:
            //Per evitare l'errore sulla stampa di report senza parametri:
            //Codice TFS ID:1371
            bool ErrInProc = false;

            //Controllo obbligatorietà campi
            if (rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportControlloPreventivo") ||
                rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportControlloPreventivoSRC") ||
                rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportPensioniCivili") ||
                rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportPensioniMilitari") ||
                rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportDecretiInEsame") ||
                rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportDecretiInEsameSRC") ||
                rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportDecretiPervenutiInIntTemporale") ||
                rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportDecretiPervenutiInIntTemporaleSRC") ||
                rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiSRC") ||
                rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiSRC") ||
                rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiConRilievoSRC") ||
                rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiSCCLA") ||
                rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiSCCLA") ||
                rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiConRilievoSCCLA")
                )
            {
                // Il controllo sulla data deve essere effettuato solo se non è stato richiesto il report sui documenti
                // in esame
                if ((!rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportDecretiInEsame") &&
                    !rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportDecretiInEsameSRC") &&
                    !rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiSRC") &&
                    !rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiSRC") &&
                    !rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiConRilievoSRC") &&
                    !rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiSCCLA") &&
                    !rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiSCCLA") &&
                    !rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiConRilievoSCCLA")
                    ) &&
                    (string.IsNullOrEmpty(dataDa.Text) || string.IsNullOrEmpty(dataA.Text)))
                {
                    utility.do_alert(this, "Attenzione inserire i campi obbligatori");
                    utility.do_openinRightFrame(this, "whitepage.htm");
                    ErrInProc = true;
                }
                if ((rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiSRC") ||
                    rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiSRC") ||
                    rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiConRilievoSRC") ||
                    rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiSCCLA") ||
                    rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiSCCLA") ||
                    rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiConRilievoSCCLA")) &&
                    ((TextBox)FindControl("txt_Elenco")).Text.Equals(""))
                {
                    utility.do_alert(this, "Attenzione inserire i campi obbligatori");
                    utility.do_openinRightFrame(this, "whitepage.htm");
                    ErrInProc = true;
                }
                else
                {
                    if (!ErrInProc && Session["filtri"] != null && (pnl_FiltriCDC.Visible || ((Panel)FindControl("pnl_FiltriCDCElencoDecreti")).Visible))
                    {
                        DocsPAWA.DocsPaWR.Utente ut = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
                        int idPeople = Convert.ToInt32(ut.idPeople);
                        string timeStamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").Replace(".", ":");
                        string templateFilePath = Server.MapPath("../Frontend/TemplateXML_NO_Interni/");
                        DocsPAWA.DocsPaWR.FileDocumento fileRep = new DocsPAWA.DocsPaWR.FileDocumento();

                        filtri = (ArrayList)Session["filtri"];
                        ProspettiRiepilogativi.Frontend.Parametro _dataDaCDC = new ProspettiRiepilogativi.Frontend.Parametro("CDCDataDa", "CDCDataDa", dataDa.Text);
                        ProspettiRiepilogativi.Frontend.Parametro _dataACDC = new ProspettiRiepilogativi.Frontend.Parametro("CDCDataA", "CDCDataA", dataA.Text);

                        ProspettiRiepilogativi.Frontend.Parametro _magistratoCDC = new ProspettiRiepilogativi.Frontend.Parametro("CDCMagistrato", "", "");
                        if (!string.IsNullOrEmpty(corr_magistrato.CODICE_TEXT))
                        {
                            Corrispondente corrMagistrato = DocsPAWA.UserManager.getCorrispondenteByCodRubrica(this, corr_magistrato.CODICE_TEXT);
                            if (corrMagistrato != null)
                                _magistratoCDC = new ProspettiRiepilogativi.Frontend.Parametro("CDCMagistrato", corr_magistrato.DESCRIZIONE_TEXT, corrMagistrato.systemId);
                            else
                                _magistratoCDC = new ProspettiRiepilogativi.Frontend.Parametro("CDCMagistrato", "", "");
                        }

                        ProspettiRiepilogativi.Frontend.Parametro _revisoreCDC = new ProspettiRiepilogativi.Frontend.Parametro("CDCRevisore", "", "");
                        if (!string.IsNullOrEmpty(corr_revisore.CODICE_TEXT))
                        {
                            Corrispondente corrRevisore = DocsPAWA.UserManager.getCorrispondenteByCodRubrica(this, corr_revisore.CODICE_TEXT);
                            if (corrRevisore != null)
                                _revisoreCDC = new ProspettiRiepilogativi.Frontend.Parametro("CDCRevisore", corr_revisore.DESCRIZIONE_TEXT, corrRevisore.systemId);
                            else
                                _revisoreCDC = new ProspettiRiepilogativi.Frontend.Parametro("CDCRevisore", "", "");
                        }

                        ProspettiRiepilogativi.Frontend.Parametro _ufficiCDC = new ProspettiRiepilogativi.Frontend.Parametro("CDCUffici", "", "");
                        if (!string.IsNullOrEmpty(ddl_uffici.SelectedValue) && !ddl_uffici.SelectedValue.ToUpper().Equals("TUTTI"))
                        {
                            Templates[] tipiAtto = (Templates[])DocsPAWA.ProfilazioneDocManager.getTemplates(DocsPAWA.UserManager.getRuolo(this).idAmministrazione, this).ToArray(typeof(Templates));
                            Templates tipoAtto = null;
                            ArrayList ruoliTipoAtto = null;

                            if (tipiAtto != null)
                            {
                                if (rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportControlloPreventivo"))
                                {
                                    //***************************************************************
                                    //
                                    //Modifica CDC GIORDANO IACOZZILLI 15/06/2012
                                    //
                                    //Faccio una If per verificare quale report lanciare, se il successivo o il preventivo.
                                    if (((RadioButtonList)FindControl("rblTipoControlloContrData")).SelectedItem.Value != "0")
                                    {
                                        //Successivo:
                                        tipoAtto =
                                            tipiAtto.Where(
                                                template =>
                                                template.DESCRIZIONE.ToUpper().Equals("CONTROLLO SUCCESSIVO SCCLA")).
                                                FirstOrDefault();
                                        Session.Add("tipoAtto", tipoAtto.SYSTEM_ID);
                                    }
                                    else
                                    {
                                        //Preventivo:
                                        tipoAtto =
                                            tipiAtto.Where(
                                                template =>
                                                template.DESCRIZIONE.ToUpper().Equals("CONTROLLO PREVENTIVO SCCLA")).
                                                FirstOrDefault();
                                        Session.Add("tipoAtto", tipoAtto.SYSTEM_ID);
                                    }
                                    //***************************************************************
                                    //FINE
                                    //***************************************************************
                                }

                                if (rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportControlloPreventivoSRC"))
                                {
                                    tipoAtto =
                                        tipiAtto.Where(
                                            template =>
                                            template.DESCRIZIONE.ToUpper().Equals("CONTROLLO PREVENTIVO SRC")).
                                            FirstOrDefault();
                                    Session.Add("tipoAttoSRC", tipoAtto.SYSTEM_ID);
                                }

                                if (rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportPensioniCivili"))
                                    tipoAtto = tipiAtto.Where(template => template.DESCRIZIONE.ToUpper().Equals("PENSIONI CIVILI SCCLA")).FirstOrDefault();

                                if (rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportPensioniMilitari"))
                                    tipoAtto = tipiAtto.Where(template => template.DESCRIZIONE.ToUpper().Equals("PENSIONI MILITARI SCCLA")).FirstOrDefault();
                            }


                            if (tipoAtto != null)
                            {
                                ruoliTipoAtto = DocsPAWA.ProfilazioneDocManager.getRuoliTipoDoc(tipoAtto.SYSTEM_ID.ToString(), this);

                                AssDocFascRuoli[] ruoliAutorizzati = ((AssDocFascRuoli[])ruoliTipoAtto.ToArray(typeof(AssDocFascRuoli)));
                                ruoliAutorizzati = ruoliAutorizzati.Where(assRuolo => assRuolo.DIRITTI_TIPOLOGIA != null && assRuolo.DIRITTI_TIPOLOGIA != "0").ToArray<AssDocFascRuoli>();
                                ruoliTipoAtto = new ArrayList(ruoliAutorizzati);
                            }

                            string CDCUffici = string.Empty;
                            DocsPaWebService ws = new DocsPaWebService();

                            if (ruoliTipoAtto != null)
                            {
                                if (rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportControlloPreventivo"))
                                {
                                    foreach (AssDocFascRuoli assDocFascRuoli in ruoliTipoAtto)
                                    {
                                        if (!string.IsNullOrEmpty(assDocFascRuoli.DIRITTI_TIPOLOGIA) &&
                                            !assDocFascRuoli.DIRITTI_TIPOLOGIA.Equals("0"))
                                        {
                                            Ruolo ruoloTipologia =
                                                DocsPAWA.UserManager.getRuoloByIdGruppo(assDocFascRuoli.ID_GRUPPO, this);
                                            if (ruoloTipologia != null && ruoloTipologia.uo != null &&
                                                ruoloTipologia.uo.codice.Equals(ddl_uffici.SelectedValue))
                                            {
                                                CDCUffici += ruoloTipologia.idGruppo + ",";
                                            }
                                        }
                                    }
                                }
                                else if (rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportControlloPreventivoSRC"))
                                {
                                    Hashtable hash = (Hashtable)Session["hashTableUffici"];
                                    string idRegistro = hash[ddl_uffici.SelectedValue].ToString();
                                    DataSet roles = ws.GetRuoliByRegistro(idRegistro);
                                    foreach (DataRow row in roles.Tables[0].Rows)
                                    {
                                        CDCUffici += row["ID_GRUPPO"].ToString() + ",";
                                    }
                                }
                            }

                            // Se il tipo di report da generare è relativo ai decreti in esame, viene prelevato l'id dell'ufficio
                            // in cui si trova l'utente
                            if (this.rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportDecretiInEsame") ||
                               this.rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportDecretiInEsameSRC") ||
                               this.rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportDecretiPervenutiInIntTemporale") ||
                               this.rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportDecretiPervenutiInIntTemporaleSRC") ||
                               this.rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiSRC") ||
                               this.rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiSRC") ||
                               this.rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiConRilievoSRC") ||
                               this.rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiSCCLA") ||
                               this.rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiSCCLA") ||
                               this.rb_prospettiRiepilogativi.SelectedValue.Equals("CDC_reportElencoDecretiRestituitiConRilievoSCCLA")
                               )
                                _ufficiCDC = new ProspettiRiepilogativi.Frontend.Parametro("CDCUffici", ddl_uffici.SelectedItem.Value, UserManager.getRuolo().uo.systemId);


                            if (!string.IsNullOrEmpty(CDCUffici))
                            {
                                string paramCDCUffici = CDCUffici.Substring(0, CDCUffici.Length - 1);
                                paramCDCUffici = "(" + paramCDCUffici + ")";
                                _ufficiCDC = new ProspettiRiepilogativi.Frontend.Parametro("CDCUffici", ddl_uffici.SelectedItem.Text, paramCDCUffici);
                            }
                        }

                        if (((Panel)FindControl("pnl_FiltriCDCElencoDecreti")).Visible)
                        {

                            ProspettiRiepilogativi.Frontend.Parametro _numElencoCDC = new ProspettiRiepilogativi.Frontend.Parametro("CDCNumElenco", "Numero Elenco", ((TextBox)FindControl("txt_Elenco")).Text);
                            utility.DO_UpdateParameters(filtri, _numElencoCDC, true);
                        }

                        utility.DO_UpdateParameters(filtri, _dataDaCDC, true);
                        utility.DO_UpdateParameters(filtri, _dataACDC, true);
                        utility.DO_UpdateParameters(filtri, _ufficiCDC, true);
                        utility.DO_UpdateParameters(filtri, _magistratoCDC, true);
                        utility.DO_UpdateParameters(filtri, _revisoreCDC, true);

                        FiltroRicerca[] filters;

                        try
                        {
                            int numRows = 0;
                            string mitt = String.Empty;
                            DocsPaWebService ws = new DocsPaWebService();

                            switch (rb_prospettiRiepilogativi.SelectedValue)
                            {
                                case "CDC_reportControlloPreventivo":
                                    //***************************************************************
                                    //
                                    //Modifica CDC GIORDANO IACOZZILLI 15/06/2012
                                    //
                                    //Faccio una If per verificare quale report lanciare, se il successivo o il preventivo.

                                    string idTipoAtto = string.Empty;
                                    if (((RadioButtonList)FindControl("rblTipoControlloContrData")).SelectedItem.Value != "0")
                                    {
                                        //Successivo:
                                        templateFilePath += "CDC_reportControlloSuccessivoSCCLA.xml";
                                        //Giordano Iacozzilli: Introduco questo controllo per evitare il catch sul null della session.
                                        if (Session["tipoAtto"] != null)
                                        {
                                            idTipoAtto = Session["tipoAtto"].ToString();
                                        }
                                        string ufficiPerCount = String.Empty;

                                        if (!String.IsNullOrEmpty(_ufficiCDC.Valore))
                                            ufficiPerCount = _ufficiCDC.Valore.Substring(1, _ufficiCDC.Valore.Length - 2);

                                        fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.CDC_reportSuccessivoSCCLA, filtri, idPeople, timeStamp);
                                    }
                                    else
                                    //Preventivo:
                                    {
                                        templateFilePath += "CDC_reportControlloPreventivo.xml";
                                        //Giordano Iacozzilli: Introduco questo controllo per evitare il catch sul null della session.
                                        if (Session["tipoAtto"] != null)
                                        {
                                            idTipoAtto = Session["tipoAtto"].ToString();
                                        }
                                        string ufficiPerCount = String.Empty;

                                        if (!String.IsNullOrEmpty(_ufficiCDC.Valore))
                                            ufficiPerCount = _ufficiCDC.Valore.Substring(1, _ufficiCDC.Valore.Length - 2);

                                        string countDeferimenti = ws.CountDeferimenti(_dataDaCDC.Valore, _dataACDC.Valore,
                                                                                      ufficiPerCount, idTipoAtto);

                                        string countDecretiEsaminati = ws.CountDecretiEsaminati(_dataDaCDC.Valore,
                                                                                                _dataACDC.Valore,
                                                                                                ufficiPerCount);
                                        //Session.Remove("tipoAtto");
                                        ProspettiRiepilogativi.Frontend.Parametro totDeferimenti = new ProspettiRiepilogativi.Frontend.Parametro("TOTDEFERIMENTI", "Totale Deferimenti", countDeferimenti);
                                        utility.DO_UpdateParameters(filtri, totDeferimenti, true);
                                        ProspettiRiepilogativi.Frontend.Parametro totDecretiEsaminati = new ProspettiRiepilogativi.Frontend.Parametro("TOTDECRETIESAMINATI", "Totale Decreti Esaminati", countDecretiEsaminati);
                                        utility.DO_UpdateParameters(filtri, totDecretiEsaminati, true);
                                        fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.CDC_reportControlloPreventivo, filtri, idPeople, timeStamp);
                                    }
                                    break;

                                case "CDC_reportControlloPreventivoSRC":
                                    templateFilePath += "CDC_reportControlloPreventivoSRC.xml";
                                    string ufficioSceltoSRC = ddl_uffici.SelectedItem.Text;
                                    //if (ufficioSceltoSRC != null)
                                    //    if (!ufficioSceltoSRC.Equals("TUTTI"))
                                    //    {
                                    //        Hashtable hash = (Hashtable)Session["hashTableUffici"];
                                    //        ufficioSceltoSRC = hash[ufficioSceltoSRC].ToString();
                                    //        DataSet roles = ws.GetRuoliByRegistro(ufficioSceltoSRC);

                                    //    }

                                    //    else
                                    //    {
                                    //        ufficioSceltoSRC = string.Empty;
                                    //    }

                                    //string idTipoAttoSRC = Session["tipoAttoSRC"].ToString();

                                    //string countDeferimentiSRC = ws.CountDeferimenti(_dataDaCDC.Valore, _dataACDC.Valore,
                                    //                                              ufficioSceltoSRC, idTipoAttoSRC);
                                    //Session.Remove("tipoAttoSRC");
                                    ProspettiRiepilogativi.Frontend.Parametro descRegistro = new ProspettiRiepilogativi.Frontend.Parametro("DESCREGISTRO", "Registro", ufficioSceltoSRC);
                                    utility.DO_UpdateParameters(filtri, descRegistro, true);
                                    fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.CDC_reportControlloPreventivoSRC, filtri, idPeople, timeStamp);
                                    break;

                                case "CDC_reportPensioniCivili":
                                    templateFilePath += "CDC_reportPensioniCivili.xml";
                                    fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.CDC_reportPensioniCivili, filtri, idPeople, timeStamp);
                                    break;

                                case "CDC_reportPensioniMilitari":
                                    templateFilePath += "CDC_reportPensioniMilitari.xml";
                                    fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.CDC_reportPensioniMilitari, filtri, idPeople, timeStamp);
                                    break;

                                case "CDC_reportDecretiInEsame":
                                    templateFilePath += "CDC_reportDecretiInEsame.xls";

                                    // Generazione filtri a partire dai valori inseriti nelle varie caselle
                                    filters = this.GenerateFilters();

                                    fileRep = this.GenerateReport(
                                        "ProspettiRiepilogativi",
                                        "DecretiInEsame",
                                        String.Format("CORTE DEI CONTI - {0}", ddl_uffici.SelectedItem.Text),
                                        String.Format("Provvedimenti in Esame al giorno {0}", DateTime.Now.AddDays(-1).ToString("dddd, dd MMMM yyyy")),
                                        ReportTypeEnum.Excel,
                                        filters);

                                    // Apertura come attachment
                                    Session["percorso"] = "Fuori";

                                    break;

                                case "CDC_reportDecretiInEsameSRC":
                                    templateFilePath += "CDC_reportDecretiInEsameSRC.xls";

                                    // Generazione filtri a partire dai valori inseriti nelle varie caselle
                                    filters = this.GenerateFilters();

                                    fileRep = this.GenerateReport(
                                        "ProspettiRiepilogativi",
                                        "DecretiInEsameSRC",
                                        String.Format("CORTE DEI CONTI - {0}", ddl_uffici.SelectedItem.Text),
                                        String.Format("Provvedimenti in Esame al giorno {0}", DateTime.Now.AddDays(-1).ToString("dddd, dd MMMM yyyy")),
                                        ReportTypeEnum.Excel,
                                        filters);

                                    // Apertura come attachment
                                    Session["percorso"] = "Fuori";

                                    break;

                                case "CDC_reportDecretiPervenutiInIntTemporale":
                                    //***************************************************************
                                    //
                                    //Modifica CDC GIORDANO IACOZZILLI 15/06/2012
                                    //
                                    //Faccio una If per verificare quale report lanciare, se il successivo o il preventivo.
                                    templateFilePath += "CDC_reportDecretiPervenutiInIntTemp.xls";
                                    if (((RadioButtonList)FindControl("rblTipoControlloContrData")).SelectedItem.Value != "0")
                                    {
                                        // Generazione filtri a partire dai valori inseriti nelle varie caselle
                                        filters = this.GenerateFilters();

                                        fileRep = this.GenerateReport(
                                            "ProspettiRiepilogativi",
                                            "DecretiPervenutiInIntTemporaleSuccSCCLA",
                                            String.Format("CORTE DEI CONTI - {0}", ddl_uffici.SelectedItem.Text),
                                            String.Format("Provvedimenti (Controllo Successivo) pervenuti da {0} a {1}",
                                                        DateTime.Parse(this.dataDa.Text).ToString("dddd, dd MMMM yyyy"),
                                                        DateTime.Parse(this.dataA.Text).ToString("dddd, dd MMMM yyyy")),
                                            ReportTypeEnum.Excel,
                                            filters);
                                    }
                                    else
                                    {
                                        // Generazione filtri a partire dai valori inseriti nelle varie caselle
                                        filters = this.GenerateFilters();

                                        fileRep = this.GenerateReport(
                                            "ProspettiRiepilogativi",
                                            "DecretiInEsamePervenutiInIntTemporale",
                                            String.Format("CORTE DEI CONTI - {0}", ddl_uffici.SelectedItem.Text),
                                            String.Format("Provvedimenti pervenuti da {0} a {1}",
                                                        DateTime.Parse(this.dataDa.Text).ToString("dddd, dd MMMM yyyy"),
                                                        DateTime.Parse(this.dataA.Text).ToString("dddd, dd MMMM yyyy")),
                                            ReportTypeEnum.Excel,
                                            filters);
                                    }
                                    // Apertura come attachment
                                    Session["percorso"] = "Fuori";

                                    //***************************************************************
                                    //FINE
                                    //***************************************************************
                                    break;
                                case "CDC_reportDecretiPervenutiInIntTemporaleSRC":
                                    templateFilePath += "CDC_reportDecretiPervenutiInIntTempSRC.xls";

                                    // Generazione filtri a partire dai valori inseriti nelle varie caselle
                                    filters = this.GenerateFilters();

                                    fileRep = this.GenerateReport(
                                        "ProspettiRiepilogativi",
                                        "DecretiInEsamePervenutiInIntTemporaleSRC",
                                        String.Format("CORTE DEI CONTI - {0}", ddl_uffici.SelectedItem.Text),
                                        String.Format("Provvedimenti pervenuti da {0} a {1}",
                                                    DateTime.Parse(this.dataDa.Text).ToString("dddd, dd MMMM yyyy"),
                                                    DateTime.Parse(this.dataA.Text).ToString("dddd, dd MMMM yyyy")),
                                        ReportTypeEnum.Excel,
                                        filters);

                                    // Apertura come attachment
                                    Session["percorso"] = "Fuori";
                                    break;

                                case "CDC_reportElencoDecretiSRC":
                                    templateFilePath += "CDC_reportElencoDecretiSRC.xls";

                                    // Generazione filtri a partire dai valori inseriti nelle varie caselle
                                    filters = this.GenerateFilters();

                                    //Recupero il corrispondente dalla query CDC_REPORT_ELENCO_DECRETI_SRC_2
                                    numRows = 0;
                                    mitt = ws.GetCorrFromElencoDecreti(UserManager.getInfoUtente(), filters, "CDC_REPORT_ELENCO_DECRETI_SRC_2", out numRows);

                                    if (string.IsNullOrEmpty(mitt))
                                        mitt = "--------";
                                    fileRep = this.GenerateReport(
                                        "ProspettiRiepilogativi",
                                        "ElencoDecretiSRC",
                                        String.Format("CORTE DEI CONTI-{0}", ddl_uffici.SelectedItem.Text),
                                        String.Format("Si trasmettono a " + mitt + " con elenco " + ((TextBox)FindControl("txt_Elenco")).Text + " n° " + numRows + " provvedimenti in data {0}", DateTime.Now.ToString("dd MMMM yyyy")),
                                        ReportTypeEnum.Excel,
                                        filters);

                                    // Apertura come attachment
                                    Session["percorso"] = "Fuori";
                                    break;

                                case "CDC_reportElencoDecretiSCCLA":
                                    templateFilePath += "CDC_reportElencoDecretiSCCLA.xls";

                                    // Generazione filtri a partire dai valori inseriti nelle varie caselle
                                    filters = this.GenerateFilters();

                                    //Recupero il corrispondente dalla query CDC_REPORT_ELENCO_DECRETI_SCCLA_2
                                    numRows = 0;
                                    //***************************************************************
                                    //
                                    //Modifica CDC GIORDANO IACOZZILLI 26/06/2012
                                    //
                                    //Faccio una If per verificare quale report lanciare, se il successivo o il preventivo.
                                    if (((RadioButtonList)FindControl("rblControlloElencoDecreti")).SelectedItem.Value != "0")
                                    {
                                        mitt = ws.GetCorrFromElencoDecreti(UserManager.getInfoUtente(), filters, "CDC_REPORT_ELENCO_DECRETI_SCCLA_2_SUCC", out numRows);

                                        if (string.IsNullOrEmpty(mitt))
                                            mitt = "--------";
                                        fileRep = this.GenerateReport(
                                            "ProspettiRiepilogativi",
                                            "ElencoDecretiSCCLASucc",
                                            String.Format("CORTE DEI CONTI-{0}", ddl_uffici.SelectedItem.Text),
                                            String.Format("(Controllo Successivo) Si trasmettono a " + mitt + " con elenco " + ((TextBox)FindControl("txt_Elenco")).Text + " n° " + numRows + " provvedimenti in data {0}", DateTime.Now.ToString("dd MMMM yyyy")),
                                            ReportTypeEnum.Excel,
                                            filters);

                                        // Apertura come attachment
                                        Session["percorso"] = "Fuori";
                                    }
                                    else
                                    {
                                        mitt = ws.GetCorrFromElencoDecreti(UserManager.getInfoUtente(), filters, "CDC_REPORT_ELENCO_DECRETI_SCCLA_2", out numRows);

                                        if (string.IsNullOrEmpty(mitt))
                                            mitt = "--------";
                                        fileRep = this.GenerateReport(
                                            "ProspettiRiepilogativi",
                                            "ElencoDecretiSCCLA",
                                            String.Format("CORTE DEI CONTI-{0}", ddl_uffici.SelectedItem.Text),
                                            String.Format("(Controllo Preventivo) Si trasmettono a " + mitt + " con elenco " + ((TextBox)FindControl("txt_Elenco")).Text + " n° " + numRows + " provvedimenti in data {0}", DateTime.Now.ToString("dd MMMM yyyy")),
                                            ReportTypeEnum.Excel,
                                            filters);

                                        // Apertura come attachment
                                        Session["percorso"] = "Fuori";
                                    }
                                    break;
                                case "CDC_reportElencoDecretiRestituitiSRC":
                                    templateFilePath += "CDC_reportElencoDecretiRestituitiSRC.xls";

                                    // Generazione filtri a partire dai valori inseriti nelle varie caselle
                                    filters = this.GenerateFilters();

                                    //Recupero il corrispondente dalla query CDC_REPORT_ELENCO_DECRETI_RESTITUITI_SRC_2
                                    numRows = 0;
                                    mitt = ws.GetCorrFromElencoDecreti(UserManager.getInfoUtente(), filters, "CDC_REPORT_ELENCO_DECRETI_RESTITUITI_SRC_2", out numRows);

                                    if (string.IsNullOrEmpty(mitt))
                                        mitt = "--------";
                                    fileRep = this.GenerateReport(
                                        "ProspettiRiepilogativi",
                                        "ElencoDecretiRestituitiSRC",
                                        String.Format("CORTE DEI CONTI-{0}", ddl_uffici.SelectedItem.Text),
                                        String.Format("Si trasmettono a " + mitt + " con elenco " + ((TextBox)FindControl("txt_Elenco")).Text + " n° " + numRows + " provvedimenti in data {0}", DateTime.Now.ToString("dd MMMM yyyy")),
                                        ReportTypeEnum.Excel,
                                        filters);

                                    // Apertura come attachment
                                    Session["percorso"] = "Fuori";
                                    break;
                                case "CDC_reportElencoDecretiRestituitiSCCLA":
                                    templateFilePath += "CDC_reportElencoDecretiRestituitiSCCLA.xls";

                                    // Generazione filtri a partire dai valori inseriti nelle varie caselle
                                    filters = this.GenerateFilters();

                                    //Recupero il corrispondente dalla query CDC_REPORT_ELENCO_DECRETI_RESTITUITI_SCCLA_2
                                    numRows = 0;

                                    //***************************************************************
                                    //
                                    //Modifica CDC GIORDANO IACOZZILLI 26/06/2012
                                    //
                                    //Faccio una If per verificare quale report lanciare, se il successivo o il preventivo.
                                    //Successivo
                                    if (((RadioButtonList)FindControl("rblControlloElencoDecreti")).SelectedItem.Value != "0")
                                    {
                                        mitt = ws.GetCorrFromElencoDecreti(UserManager.getInfoUtente(), filters, "CDC_REPORT_ELENCO_DECRETI_RESTITUITI_SCCLA_2_SUCC", out numRows);

                                        if (string.IsNullOrEmpty(mitt))
                                            mitt = "--------";
                                        fileRep = this.GenerateReport(
                                            "ProspettiRiepilogativi",
                                            "ElencoDecretiRestituitiSCCLASucc",
                                            String.Format("CORTE DEI CONTI-{0}", ddl_uffici.SelectedItem.Text),
                                            String.Format("(Controllo Successivo) Si trasmettono a " + mitt + " con elenco " + ((TextBox)FindControl("txt_Elenco")).Text + " n° " + numRows + " provvedimenti in data {0}", DateTime.Now.ToString("dd MMMM yyyy")),
                                            ReportTypeEnum.Excel,
                                            filters);

                                        // Apertura come attachment
                                        Session["percorso"] = "Fuori";
                                    }
                                    else
                                    //Preventivo
                                    {
                                        mitt = ws.GetCorrFromElencoDecreti(UserManager.getInfoUtente(), filters, "CDC_REPORT_ELENCO_DECRETI_RESTITUITI_SCCLA_2", out numRows);

                                        if (string.IsNullOrEmpty(mitt))
                                            mitt = "--------";
                                        fileRep = this.GenerateReport(
                                            "ProspettiRiepilogativi",
                                            "ElencoDecretiRestituitiSCCLA",
                                            String.Format("CORTE DEI CONTI-{0}", ddl_uffici.SelectedItem.Text),
                                            String.Format("(Controllo Preventivo) Si trasmettono a " + mitt + " con elenco " + ((TextBox)FindControl("txt_Elenco")).Text + " n° " + numRows + " provvedimenti in data {0}", DateTime.Now.ToString("dd MMMM yyyy")),
                                            ReportTypeEnum.Excel,
                                            filters);

                                        // Apertura come attachment
                                        Session["percorso"] = "Fuori";
                                    }
                                    break;
                                case "CDC_reportElencoDecretiRestituitiConRilievoSRC":
                                    templateFilePath += "CDC_reportElencoDecretiRestituitiConRilievoSRC.xls";

                                    // Generazione filtri a partire dai valori inseriti nelle varie caselle
                                    filters = this.GenerateFilters();

                                    //Recupero il corrispondente dalla query CDC_REPORT_ELENCO_DECRETI_RESTITUITI_CON_RILIEVO_SRC_2
                                    numRows = 0;
                                    mitt = ws.GetCorrFromElencoDecreti(UserManager.getInfoUtente(), filters, "CDC_REPORT_ELENCO_DECRETI_RESTITUITI_CON_RILIEVO_SRC_2", out numRows);

                                    if (string.IsNullOrEmpty(mitt))
                                        mitt = "--------";
                                    fileRep = this.GenerateReport(
                                        "ProspettiRiepilogativi",
                                        "ElencoDecretiRestituitiConRilievoSRC",
                                        String.Format("CORTE DEI CONTI-{0}", ddl_uffici.SelectedItem.Text),
                                        String.Format("Si trasmettono a " + mitt + " con elenco " + ((TextBox)FindControl("txt_Elenco")).Text + " n° " + numRows + " provvedimenti con rilievo in data {0}", DateTime.Now.ToString("dd MMMM yyyy")),
                                        ReportTypeEnum.Excel,
                                        filters);

                                    // Apertura come attachment
                                    Session["percorso"] = "Fuori";
                                    break;
                                case "CDC_reportElencoDecretiRestituitiConRilievoSCCLA":

                                    templateFilePath += "CDC_reportElencoDecretiRestituitiConRilievoSCCLA.xls";

                                    // Generazione filtri a partire dai valori inseriti nelle varie caselle
                                    filters = this.GenerateFilters();

                                    //Recupero il corrispondente dalla query CDC_REPORT_ELENCO_DECRETI_RESTITUITI_CON_RILIEVO_SCCLA_2
                                    numRows = 0;
                                    //***************************************************************
                                    //
                                    //Modifica CDC GIORDANO IACOZZILLI 02/07/2012
                                    //
                                    //Faccio una If per verificare quale report lanciare, se il successivo o il preventivo.
                                    //SUccessivo
                                    if (((RadioButtonList)FindControl("rblControlloElencoDecreti")).SelectedItem.Value != "0")
                                    {
                                        mitt = ws.GetCorrFromElencoDecreti(UserManager.getInfoUtente(), filters, "CDC_REPORT_ELENCO_DECRETI_RESTITUITI_CON_RILIEVO_SCCLA_2_SUCC", out numRows);

                                        if (string.IsNullOrEmpty(mitt))
                                            mitt = "--------";
                                        fileRep = this.GenerateReport(
                                            "ProspettiRiepilogativi",
                                            "ElencoDecretiRestituitiConRilievoSCCLASucc",
                                            String.Format("CORTE DEI CONTI-{0}", ddl_uffici.SelectedItem.Text),
                                            String.Format("(Controllo Successivo) Si trasmettono a " + mitt + " con elenco " + ((TextBox)FindControl("txt_Elenco")).Text + " n° " + numRows + " provvedimenti con rilievo in data {0}", DateTime.Now.ToString("dd MMMM yyyy")),
                                            ReportTypeEnum.Excel,
                                            filters);

                                        // Apertura come attachment
                                        Session["percorso"] = "Fuori";
                                    }
                                    else
                                    //preventivo
                                    {
                                        mitt = ws.GetCorrFromElencoDecreti(UserManager.getInfoUtente(), filters, "CDC_REPORT_ELENCO_DECRETI_RESTITUITI_CON_RILIEVO_SCCLA_2", out numRows);

                                        if (string.IsNullOrEmpty(mitt))
                                            mitt = "--------";
                                        fileRep = this.GenerateReport(
                                            "ProspettiRiepilogativi",
                                            "ElencoDecretiRestituitiConRilievoSCCLA",
                                            String.Format("CORTE DEI CONTI-{0}", ddl_uffici.SelectedItem.Text),
                                            String.Format("(Controllo Preventivo) Si trasmettono a " + mitt + " con elenco " + ((TextBox)FindControl("txt_Elenco")).Text + " n° " + numRows + " provvedimenti con rilievo in data {0}", DateTime.Now.ToString("dd MMMM yyyy")),
                                            ReportTypeEnum.Excel,
                                            filters);

                                        // Apertura come attachment
                                        Session["percorso"] = "Fuori";
                                    }

                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                        }


                        if (fileRep != null)
                        {
                            if (Session["percorso"] != null)
                                utility.SetSelectedFileReport(this, fileRep, true);
                            else
                                utility.setSelectedFileReport(this, fileRep, "");
                        }
                        else
                        {
                            utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                            utility.do_openinRightFrame(this, "whitepage.htm");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Metodo per la creazione dei filtri di ricerca
        /// </summary>
        /// <returns>Lista dei filtri di ricerca</returns>
        private FiltroRicerca[] GenerateFilters()
        {
            Frontend.Parametro[] converted = (Frontend.Parametro[])this.filtri.ToArray(typeof(Frontend.Parametro));

            IEnumerable<FiltroRicerca> filters =
                from p in converted
                select new FiltroRicerca()
                {
                    argomento = ((Frontend.Parametro)p).Nome,
                    valore = ((Frontend.Parametro)p).Valore
                };

            return filters.ToArray<FiltroRicerca>();

        }

        /// <summary>
        /// Metodo per la generazione di un report
        /// </summary>
        /// <param name="contextName">Nome del contesto di reportistica</param>
        /// <param name="reportKey">Chiave identificativa del report all'interno del contesto</param>
        /// <param name="title">Titolo da assegnare al report</param>
        /// <param name="subtitle">Sottotitolo da assegnare al report</param>
        /// <param name="reportType">Tipo di report da generare</param>
        /// <param name="filters">Filtri di ricerca</param>
        /// <returns>File documento con il report da mostrare all'utente</returns>
        private FileDocumento GenerateReport(String contextName, String reportKey, String title, String subtitle, ReportTypeEnum reportType, FiltroRicerca[] filters)
        {
            FileDocumento report = null;
            if (!reportKey.Equals("ElencoDecretiSRC") &&
                !reportKey.Equals("ElencoDecretiRestituitiSRC") &&
                !reportKey.Equals("ElencoDecretiRestituitiConRilievoSRC") &&
                !reportKey.Equals("ElencoDecretiSCCLA") &&
                !reportKey.Equals("ElencoDecretiRestituitiSCCLA") &&
                !reportKey.Equals("ElencoDecretiRestituitiConRilievoSCCLA")
                )
            {
                PrintReportRequest request = new PrintReportRequest()
                {
                    ContextName = contextName,
                    ReportKey = reportKey,
                    ReportType = reportType,
                    SubTitle = subtitle,
                    Title = title,
                    SearchFilters = filters,
                    UserInfo = UserManager.getInfoUtente()
                };
                // Generazione del report
                report = ReportingUtils.GenerateReport(request);


            }
            else
            {
                string[] splitted = title.Split('-');
                string addinfo = subtitle;
                title = splitted[0];
                if (splitted.Length > 1)
                    subtitle = splitted[1];
                PrintReportRequest request = new PrintReportRequest()
                {
                    ContextName = contextName,
                    ReportKey = reportKey,
                    ReportType = reportType,
                    SubTitle = subtitle,
                    Title = title,
                    SearchFilters = filters,
                    UserInfo = UserManager.getInfoUtente(),
                    AdditionalInformation = addinfo
                };
                // Generazione del report
                report = ReportingUtils.GenerateReport(request);
            }

            return report;
        }

        private void setCampiCorrCDC()
        {
            if (Session["rubrica.campoCorrispondente"] != null)
            {
                DocsPAWA.DocsPaWR.Corrispondente corr = (DocsPAWA.DocsPaWR.Corrispondente)Session["rubrica.campoCorrispondente"];
                if (corr != null)
                {
                    if (Session["rubrica.idCampoCorrispondente"] != null && Session["rubrica.idCampoCorrispondente"].ToString() == corr_magistrato.ID)
                    {
                        corr_magistrato.CODICE_TEXT = corr.codiceRubrica;
                        corr_magistrato.DESCRIZIONE_TEXT = corr.descrizione;
                        Session.Remove("rubrica.campoCorrispondente");
                        Session.Remove("rubrica.idCampoCorrispondente");
                    }
                    if (Session["rubrica.idCampoCorrispondente"] != null && Session["rubrica.idCampoCorrispondente"].ToString() == corr_revisore.ID)
                    {
                        corr_revisore.CODICE_TEXT = corr.codiceRubrica;
                        corr_revisore.DESCRIZIONE_TEXT = corr.descrizione;
                        Session.Remove("rubrica.campoCorrispondente");
                        Session.Remove("rubrica.idCampoCorrispondente");
                    }
                }
            }
        }
        #endregion metodi report CDC
        private void rb_scelta_sott_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string _value = rb_scelta_sott.SelectedValue;
            if (_value.Equals("UO"))
            {
                this.pnl_scelta_uo.Visible = true;
                this.pnl_scelta_rf.Visible = false;
                this.btn_img_sott_rubr.Visible = true;
            }
            else
            {
                this.pnl_scelta_uo.Visible = false;
                this.pnl_scelta_rf.Visible = true;
                this.btn_img_sott_rubr.Visible = false;
            }
        }

        private void ddl_rf_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //dafare (forse)
        }

        private void caricaComboRf()
        {
            this.ddl_rf.Items.Clear();

            DocsPAWA.DocsPaWR.OrgRegistro[] listaTotale = null;
            //voglio la lista dei soli RF, quindi al webMethod passero come chaRF il valore 1 (solo RF)

            DocsPAWA.DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];

            DocsPAWA.DocsPaWR.PR_Amministrazione amm = Controller.Do_GetAmmByIdAmm(Convert.ToInt32(cr.idAmministrazione));

            listaTotale = wws.AmmGetRegistri(amm.Codice, "1");

            if (listaTotale != null && listaTotale.Length > 0)
            {
                int y = 0;
                for (int i = 0; i < listaTotale.Length; i++)
                {
                    string testo = listaTotale[i].Codice;
                    this.ddl_rf.Items.Add(testo);
                    this.ddl_rf.Items[y].Value = listaTotale[i].IDRegistro;
                    y++;
                }

                DocsPAWA.DocsPaWR.Registro rf_reg = wws.GetRegistroBySistemId(ddl_rf.SelectedItem.Value);
            }

            DocsPAWA.DocsPaWR.Registro[] registriRf = null;
            Ruolo ruoloUtente = DocsPAWA.UserManager.getRuolo();
            registriRf = DocsPAWA.UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "1", "");

            if (registriRf != null && registriRf.Length > 0)
            {
                this.ddl_rf.SelectedValue = registriRf[0].systemId;
            }
        }

        private void caricaComboTitolari()
        {
            ddl_titolari.Items.Clear();
            ArrayList listaTitolari = new ArrayList(wws.getTitolariUtilizzabili(DocsPAWA.UserManager.getUtente(this).idAmministrazione));

            //Esistono dei titolari chiusi
            if (listaTitolari.Count > 1)
            {
                //Creo le voci della ddl dei titolari
                string valueTutti = string.Empty;
                foreach (DocsPAWA.DocsPaWR.OrgTitolario titolario in listaTitolari)
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

            }

            //Non esistono titolario chiusi
            if (listaTitolari.Count == 1)
            {
                DocsPAWA.DocsPaWR.OrgTitolario titolario = (DocsPAWA.DocsPaWR.OrgTitolario)listaTitolari[0];
                if (titolario.Stato != DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione)
                {
                    ListItem it = new ListItem(titolario.Descrizione, titolario.ID);
                    ddl_titolari.Items.Add(it);
                }
                ddl_titolari.Enabled = false;
            }
        }

        protected void ddl_tipo_data_creazione_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.cld_creazione_al.Text = "";
            this.cld_creazione_dal.Text = "";

            if (this.ddl_tipo_data_creazione.SelectedIndex == 0)
            {
                this.cld_creazione_al.Visible = false;
                this.lbl_al_apertura.Visible = false;
                this.lbl_dal_apertura.Visible = false;
                this.GetCalendarControl("cld_creazione_dal").txt_Data.Enabled = true;
            }

            if (this.ddl_tipo_data_creazione.SelectedIndex == 1)
            {
                this.cld_creazione_al.Visible = true;
                this.lbl_al_apertura.Visible = true;
                this.lbl_dal_apertura.Visible = true;
                this.GetCalendarControl("cld_creazione_dal").txt_Data.Enabled = true;
                this.GetCalendarControl("cld_creazione_al").txt_Data.Enabled = true;
            }

            if (this.ddl_tipo_data_creazione.SelectedIndex == 2)
            {
                this.cld_creazione_al.Visible = false;
                this.lbl_al_apertura.Visible = false;
                this.lbl_dal_apertura.Visible = false;
                this.GetCalendarControl("cld_creazione_dal").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                this.GetCalendarControl("cld_creazione_dal").txt_Data.Enabled = false;
            }

            if (this.ddl_tipo_data_creazione.SelectedIndex == 3)
            {
                this.cld_creazione_al.Visible = true;
                this.lbl_al_apertura.Visible = true;
                this.lbl_dal_apertura.Visible = true;
                this.GetCalendarControl("cld_creazione_dal").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                this.GetCalendarControl("cld_creazione_al").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                this.GetCalendarControl("cld_creazione_dal").txt_Data.Enabled = false;
                this.GetCalendarControl("cld_creazione_al").txt_Data.Enabled = false;
            }

            if (this.ddl_tipo_data_creazione.SelectedIndex == 4)
            {
                this.cld_creazione_al.Visible = true;
                this.lbl_al_apertura.Visible = true;
                this.lbl_dal_apertura.Visible = true;
                this.GetCalendarControl("cld_creazione_dal").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                this.GetCalendarControl("cld_creazione_al").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                this.GetCalendarControl("cld_creazione_dal").txt_Data.Enabled = false;
                this.GetCalendarControl("cld_creazione_al").txt_Data.Enabled = false;

            }
        }

        protected void ddl_tipo_data_chiusura_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.cld_chiusura_al.Text = "";
            this.cld_chiusura_dal.Text = "";

            if (this.ddl_tipo_data_chiusura.SelectedIndex == 0)
            {
                this.cld_chiusura_al.Visible = false;
                this.lbl_al_chiusura.Visible = false;
                this.lbl_dal_chiusura.Visible = false;
                this.GetCalendarControl("cld_chiusura_dal").txt_Data.Enabled = true;
            }

            if (this.ddl_tipo_data_chiusura.SelectedIndex == 1)
            {
                this.cld_chiusura_al.Visible = true;
                this.lbl_al_chiusura.Visible = true;
                this.lbl_dal_chiusura.Visible = true;
                this.GetCalendarControl("cld_chiusura_dal").txt_Data.Enabled = true;
                this.GetCalendarControl("cld_chiusura_al").txt_Data.Enabled = true;
            }

            if (this.ddl_tipo_data_chiusura.SelectedIndex == 2)
            {
                this.cld_chiusura_al.Visible = false;
                this.lbl_al_chiusura.Visible = false;
                this.lbl_dal_chiusura.Visible = false;
                this.GetCalendarControl("cld_chiusura_dal").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                this.GetCalendarControl("cld_chiusura_dal").txt_Data.Enabled = false;
            }

            if (this.ddl_tipo_data_chiusura.SelectedIndex == 3)
            {
                this.cld_chiusura_al.Visible = true;
                this.lbl_al_chiusura.Visible = true;
                this.lbl_dal_chiusura.Visible = true;
                this.GetCalendarControl("cld_chiusura_dal").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                this.GetCalendarControl("cld_chiusura_al").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                this.GetCalendarControl("cld_chiusura_dal").txt_Data.Enabled = false;
                this.GetCalendarControl("cld_chiusura_al").txt_Data.Enabled = false;
            }

            if (this.ddl_tipo_data_chiusura.SelectedIndex == 4)
            {
                this.cld_chiusura_al.Visible = true;
                this.lbl_al_chiusura.Visible = true;
                this.lbl_dal_chiusura.Visible = true;
                this.GetCalendarControl("cld_chiusura_dal").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                this.GetCalendarControl("cld_chiusura_al").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                this.GetCalendarControl("cld_chiusura_dal").txt_Data.Enabled = false;
                this.GetCalendarControl("cld_chiusura_al").txt_Data.Enabled = false;

            }
        }

        private void setDescCorrSott(string codiceRubrica)
        {
            DocsPAWA.DocsPaWR.Corrispondente corr = null;
            DocsPAWA.UserManager.removeCorrispondentiSelezionati(this);
            if (!codiceRubrica.Equals(""))
                corr = DocsPAWA.UserManager.getCorrispondente(this, codiceRubrica, false);

            if (corr == null)
            {
                corr_sott_non_trovato("1");
            }
            else
            {
                if (!corr.tipoCorrispondente.Equals("U"))
                {
                    corr_sott_non_trovato("2");
                }
                else
                {
                    this.txt1_corr_sott.Text = "";
                    this.txt2_corr_sott.Text = "";
                    this.hd_systemIdCorrSott.Value = "";

                    DocsPAWA.DocsPaWR.InfoUtente infoUtente = DocsPAWA.UserManager.getInfoUtente(this);
                    DocsPAWA.DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
                    infoUtente.idAmministrazione = cr.idAmministrazione;
                    DocsPAWA.DocsPaWR.Ruolo role = DocsPAWA.UserManager.getRuolo(this);
                    DocsPAWA.UserManager.setCorrispondenteSelezionatoRuoloSottopostoNoRubr(this.Page, corr);
                    this.txt1_corr_sott.Text = codiceRubrica;
                    this.txt2_corr_sott.Text = DocsPAWA.UserManager.getDecrizioneCorrispondenteSemplice(corr);
                    this.hd_systemIdCorrSott.Value = corr.systemId;


                }
            }
        }

        private void corr_sott_non_trovato(string caso)
        {
            string s = null;
            this.txt1_corr_sott.Text = "";
            this.txt2_corr_sott.Text = "";
            this.hd_systemIdCorrSott.Value = "";

            switch (caso)
            {
                case "1":
                    s = "Corrispondente non trovato";
                    break;

                case "2":
                    s = "Inserire soltanto UO come corrispondenti";
                    break;

                case "3":
                    s = "Inserire soltanto UO visibili dal ruolo corrente";
                    break;

            }
            if (!IsStartupScriptRegistered("corr_non_trovato"))
                RegisterStartupScript("corr_non_trovato",
                    "<script language=\"javascript\">" +
                    "alert (\"" + s + "\");</script>");
        }

        private void hd_systemIdCorrSott_ServerChange(object sender, System.EventArgs e)
        {

        }

        private void txt1_corr_sott_TextChanged(object sender, System.EventArgs e)
        {
            setDescCorrSott(this.txt1_corr_sott.Text);
        }

        private bool controllaCampiObbligatoriExportFasc()
        {
            bool result = true;
            if (rb_scelta_sott.SelectedValue.Equals("UO"))
            {
                if ((string.IsNullOrEmpty(txt1_corr_sott.Text)) || (string.IsNullOrEmpty(txt2_corr_sott.Text)))
                {
                    utility.do_alert(this, "Attenzione! Inserire una UO");

                    utility.do_openinRightFrame(this, "whitepage.htm");

                    result = false;
                }
            }

            if (rb_scelta_sott.SelectedValue.Equals("RF") && result)
            {
                if ((string.IsNullOrEmpty(ddl_rf.SelectedValue)))
                {
                    utility.do_alert(this, "Attenzione! Inserire un RF");

                    utility.do_openinRightFrame(this, "whitepage.htm");

                    result = false;
                }
            }

            if (string.IsNullOrEmpty(ddl_titolari.SelectedValue) && result)
            {
                utility.do_alert(this, "Attenzione! Scegliere un titolario");

                utility.do_openinRightFrame(this, "whitepage.htm");

                result = false;
            }

            if (!string.IsNullOrEmpty(cld_creazione_dal.Text) && result)
            {
                if (!DocsPAWA.Utils.isDate(this.GetCalendarControl("cld_creazione_dal").txt_Data.Text))
                {
                    utility.do_alert(this, "Attenzione! Formato data creazione errato");

                    utility.do_openinRightFrame(this, "whitepage.htm");

                    result = false;
                }
            }

            if (!string.IsNullOrEmpty(cld_chiusura_dal.Text) && result)
            {
                if (!DocsPAWA.Utils.isDate(this.GetCalendarControl("cld_chiusura_dal").txt_Data.Text))
                {
                    utility.do_alert(this, "Attenzione! Formato data chiusura errato");

                    utility.do_openinRightFrame(this, "whitepage.htm");

                    result = false;
                }
            }



            if (!string.IsNullOrEmpty(cld_creazione_al.Text) && result)
            {
                if (!DocsPAWA.Utils.isDate(this.GetCalendarControl("cld_creazione_al").txt_Data.Text))
                {
                    utility.do_alert(this, "Attenzione! Formato data creazione errato");

                    utility.do_openinRightFrame(this, "whitepage.htm");

                    result = false;
                }
            }


            if (!string.IsNullOrEmpty(cld_chiusura_al.Text) && result)
            {
                if (!DocsPAWA.Utils.isDate(this.GetCalendarControl("cld_chiusura_al").txt_Data.Text))
                {
                    utility.do_alert(this, "Attenzione! Formato data chiusura errato");

                    utility.do_openinRightFrame(this, "whitepage.htm");

                    result = false;
                }
            }

            if (result && !string.IsNullOrEmpty(cld_chiusura_dal.Text) && !string.IsNullOrEmpty(cld_chiusura_al.Text) && DocsPAWA.Utils.verificaIntervalloDate(this.GetCalendarControl("cld_chiusura_dal").txt_Data.Text, this.GetCalendarControl("cld_chiusura_al").txt_Data.Text))
            {
                utility.do_alert(this, "Attenzione! Intervallo data chiusura errato");

                utility.do_openinRightFrame(this, "whitepage.htm");

                result = false;
            }

            if (result && !string.IsNullOrEmpty(cld_creazione_dal.Text) && !string.IsNullOrEmpty(cld_creazione_al.Text) && DocsPAWA.Utils.verificaIntervalloDate(this.GetCalendarControl("cld_creazione_dal").txt_Data.Text, this.GetCalendarControl("cld_creazione_al").txt_Data.Text))
            {
                utility.do_alert(this, "Attenzione! Intervallo data creazione errato");

                utility.do_openinRightFrame(this, "whitepage.htm");

                result = false;
            }


            return result;
        }

        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
        }

        private void caricaComboTitolariReportTitolario()
        {
            ddl_titolari_report_annuale.Items.Clear();
            ArrayList listaTitolari = new ArrayList(wws.getTitolariUtilizzabili(DocsPAWA.UserManager.getUtente(this).idAmministrazione));

            //Esistono dei titolari chiusi
            if (listaTitolari.Count > 1)
            {
                //Creo le voci della ddl dei titolari
                string valueTutti = string.Empty;
                foreach (DocsPAWA.DocsPaWR.OrgTitolario titolario in listaTitolari)
                {
                    ListItem it = null;
                    switch (titolario.Stato)
                    {
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Attivo:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            ddl_titolari_report_annuale.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            ddl_titolari_report_annuale.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                    }
                }

                //Imposto la voce tutti i titolari             
                string _value = rb_prospettiRiepilogativi.SelectedValue;

                if ((_value != "reportAnnualeFascXTit"))
                {
                    valueTutti = valueTutti.Substring(0, valueTutti.Length - 1);
                    if (valueTutti != string.Empty)
                    {
                        if (valueTutti.IndexOf(',') == -1)
                            valueTutti = valueTutti + "," + valueTutti;

                        ListItem it = new ListItem("Tutti i titolari", "0");
                        ddl_titolari_report_annuale.Items.Insert(0, it);
                    }
                }
            }

            //Non esistono titolario chiusi
            if (listaTitolari.Count == 1)
            {
                DocsPAWA.DocsPaWR.OrgTitolario titolario = (DocsPAWA.DocsPaWR.OrgTitolario)listaTitolari[0];
                if (titolario.Stato != DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione)
                {
                    ListItem it = new ListItem(titolario.Descrizione, titolario.ID);
                    ddl_titolari_report_annuale.Items.Add(it);
                }
                ddl_titolari_report_annuale.Enabled = false;
            }
        }

    }
}
