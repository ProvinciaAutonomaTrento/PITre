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
using System.Web.Services;
using log4net;
using DocsPAWA.utils;



namespace DocsPAWA
{
    public class testata320 : DocsPAWA.CssPage
    {
        private ILog logger = LogManager.GetLogger(typeof(testata320));
        #region WebControls e variabili
        protected DocsPaWebCtrlLibrary.ImageButton btn_doc;
        protected DocsPaWebCtrlLibrary.ImageButton btn_search;
        protected DocsPaWebCtrlLibrary.ImageButton btn_gest;
        protected DocsPaWebCtrlLibrary.ImageButton btn_config;
        protected DocsPaWebCtrlLibrary.ImageButton btn_logout;
        protected DocsPaWebCtrlLibrary.ImageButton btn_help;
        protected System.Web.UI.WebControls.Image logoEnte;
        protected DocsPaWebCtrlLibrary.ImageButton imgNotificationCenter;

        protected System.Web.UI.WebControls.ImageButton img_logo;	// LOGO DOCSPA

        protected docsPaMenu.DocsPaMenuWC menuDoc;					// menù	DOCUMENTI		
        protected docsPaMenu.DocsPaMenuWC menuRic;					// menù RICERCA
        protected docsPaMenu.DocsPaMenuWC menuGest;					// menù GESTIONE
        protected docsPaMenu.DocsPaMenuWC menuConf;					// menù OPZIONI
        protected docsPaMenu.DocsPaMenuWC menuHelp;                 // menu HELP

        protected System.Web.UI.HtmlControls.HtmlTableCell mnubar0;	// LOGO DOCSPA
        protected System.Web.UI.HtmlControls.HtmlTableCell mnubar1;	// DOCUMENTI	
        protected System.Web.UI.HtmlControls.HtmlTableCell mnubar2;	// RICERCA
        protected System.Web.UI.HtmlControls.HtmlTableCell mnubar3;	// GESTIONE
        protected System.Web.UI.HtmlControls.HtmlTableCell mnubar4;	// OPZIONI
        protected System.Web.UI.HtmlControls.HtmlTableCell mnubar5; // HELP

        protected System.Web.UI.HtmlControls.HtmlTableCell backgroundLogo;
        protected System.Web.UI.HtmlControls.HtmlTableCell backgroundLogoEnte;

        protected System.Web.UI.WebControls.Label lbl_info_utente;	// label dati utente
        protected System.Web.UI.WebControls.Label lbl_DEPOSITO;
        protected DocsPAWA.DocsPaWR.Utente userHome;
        protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
        protected DocsPAWA.DocsPaWR.Utente userHomeDelegato;
        protected DocsPAWA.DocsPaWR.Ruolo userRuoloDelegato;
        protected DocsPAWA.DocsPaWR.InfoUtente infoUt;
        protected DocsPAWA.DocsPaWR.Registro userReg;

        protected string from;
        protected string idAmm;
        protected System.Web.UI.WebControls.TableCell td_A;
        protected System.Web.UI.WebControls.TableCell td_B;

        protected string eti_prot_ingresso;
        protected string eti_prot_partenza;

        protected  HtmlInputHidden hdSlogga;
        protected DocsPAWA.UserControls.ScrollElementsList.ScrollElementsList ScrollElementsList;
        #endregion

        #region Gestione Pagina

        /// <summary>
        /// PAGE LOAD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            Utils.startUp(this);
            this.Response.Expires = -1;

            if (!this.IsPostBack)
            {
                // Impostazione contesto chiamante
                this.SetContext();
            }

            FascicoliManager.removeMemoriaFascicoloSelezionato(this);
            FascicoliManager.removeMemoriaFolderSelezionata(this);

            // menù	DOCUMENTI
            this.menuDoc.MenuPosLeft = 133;

            // menù RICERCA	
            this.menuRic.MenuPosLeft = 224;

            // menù GESTIONE
            this.menuGest.MenuPosLeft = 315;

            // menù OPZIONI
            this.menuConf.MenuPosLeft = 406;

            // menù HELP
            // this.menuHelp.MenuPosLeft = 497;

            // disabilitazione dei pulsanti
           
            this.btn_doc.Enabled = false;
            this.btn_search.Enabled = false;
            this.btn_gest.Enabled = false;
            this.btn_config.Enabled = false;
            this.btn_help.Enabled = false;
           
            // abilitazione dei pulsanti in base al ruolo ricoperto dall'utente
            userRuolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
            if (userRuolo != null)
                this.enableFunction();

            if (IsPostBack)
            {
                UserManager.disabilitaVociMenuNonAutorizzate(this);
                this.checkChiaviWebConfig();
            }
            else
            {
                // gestione label dati utente in alto a DX
                userHome = UserManager.getUtente(this);
                infoUt = UserManager.getInfoUtente();
                //Gestione in caso di deleghe
                if (infoUt.delegato != null)
                {
                    userHomeDelegato = UserManager.getUtenteDelegato();
                    if (userHomeDelegato != null)
                    {
                        if (userHomeDelegato.descrizione != "" || userHomeDelegato.descrizione != null)
                        {
                            this.lbl_info_utente.Text = userHomeDelegato.descrizione + "&nbsp;";
                        }
                    }
                    if (userHome != null)
                    {
                        this.lbl_info_utente.Text += "<br>Delegato da " + userHome.descrizione + "&nbsp;";
                    }
                    if (userRuolo != null)
                    {
                        this.lbl_info_utente.Text += "<br>" + userRuolo.descrizione + "&nbsp;";
                    }
                }
                else
                {

                    if (userHome != null)
                    {
                        if (userHome.descrizione != "" || userHome.descrizione != null)
                        {
                            this.lbl_info_utente.Text = userHome.descrizione + "&nbsp;";
                        }
                    }
                    if (userRuolo != null)
                    {
                        this.lbl_info_utente.Text += "<br>" + userRuolo.descrizione + "&nbsp;";
                    }
                }

                if (UserManager.ruoloIsAutorized(this, "GEST_ARCHIVIA"))
                {
                    this.lbl_info_utente.Text += "<br> AREA DEPOSITO&nbsp;";
                }
            }

            Page.DataBind();

            //Modifica frezza + fabio etichette protocolli 
            getLettereProtocolli();
            foreach (docsPaMenu.myLink itm in menuDoc.Links)
            {
                if (itm.Key == "PROTO_INGRESSO_SEMPLIFICATO")
                {
                    itm.Text = eti_prot_ingresso;
                }
                if (itm.Key == "PROTO_USCITA_SEMPLIFICATO")
                {
                    itm.Text = eti_prot_partenza;
                }
            }
        }

        
      
        /// <summary>
        /// PRERENDER
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void testata317_PreRender(object sender, System.EventArgs e)
        {
            // Controllo per disabilitare le funzionalità in base al ruolo scelto
            UserManager.disabilitaVociMenuNonAutorizzate(this);

            // Controllo per disabilitare le funzionalità in base alle chiavi del web.config
            this.checkChiaviWebConfig();

            //Controllo se abilitare o meno i pulsanti per lo scorrimento di elementi in lista
            this.ScrollElementsList.Visible = UserControls.ScrollElementsList.ScrollManager.enableScrollElementsList();

            // string idAmm = "";
        }

        #endregion

        private string findFontColor(string idAmm)
        {
            return FileManager.findFontColor(idAmm);
        }

        private string findPulsColor(string idAmm)
        {
            return FileManager.findPulsColor(idAmm);
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            if (!this.DesignMode)
            {
                if (Context != null && Context.Session != null && Session != null)
                {
                    InitializeComponent();
                    base.OnInit(e);
                }
            }
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.img_logo.Click += new System.Web.UI.ImageClickEventHandler(this.img_logo_Click);
            this.btn_logout.Click += new System.Web.UI.ImageClickEventHandler(this.btn_logout_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.testata317_PreRender);
            this.Init += new EventHandler(testata320_Init);
            this.btn_help.Click += new System.Web.UI.ImageClickEventHandler(this.btn_help_Click);

            // Visualizzazione del pulsante del centro notifiche
            bool isEnabledNC = NotificationCenterHelper.IsUserEnabledToViewNotificationsCenter(this);
            this.imgNotificationCenter.Visible = isEnabledNC;
            this.imgNotificationCenter.Enabled = isEnabledNC;
            if (isEnabledNC)
            {
                int itemCount = 0;

                try
                {
                    itemCount = NotificationCenterHelper.CountNotViewedItems(Int32.Parse(UserManager.getInfoUtente().idPeople), UserManager.getInfoAmmCorrente(UserManager.getInfoUtente().idAmministrazione).Codice);
                }
                catch (Exception e)
                { }

                this.imgNotificationCenter.AlternateText = String.Format("Centro notifiche ({0})", itemCount.ToString());
            }
        }

        void testata320_Init(object sender, EventArgs e)
        {
            if (UserManager.getInfoUtente() != null)
                idAmm = UserManager.getInfoUtente().idAmministrazione;
            if (idAmm != null && idAmm != "")
            {
                if (fileExist("logo.gif", "LoginFE"))
                {
                    this.img_logo.ImageUrl = "images/loghiAmministrazioni/logo.gif";
                }

                DocsPAWA.utils.InitImagePath iip = utils.InitImagePath.getInstance(idAmm);
                this.logoEnte.ImageUrl = iip.getPath("LOGO");
                string pathLogo = utils.InitImagePath.getInstance(idAmm).getPath("BKG_TESTO");
                string pathLogoEnte = utils.InitImagePath.getInstance(idAmm).getPath("BKG_LOGO");
                this.backgroundLogo.Attributes.Add("background", pathLogo);
                this.backgroundLogoEnte.Attributes.Add("background", pathLogoEnte);
                this.backgroundLogoEnte.Attributes.Add("height", "100%");

                string coloreAmministrazione = findFontColor(idAmm);
                if (findFontColor(idAmm) != "")
                {
                    string[] colorSplit = coloreAmministrazione.Split('^');
                    string red = colorSplit[0];
                    string green = colorSplit[1];
                    string blu = colorSplit[2];
                    this.lbl_info_utente.ForeColor = System.Drawing.Color.FromArgb(Convert.ToInt16(red), Convert.ToInt16(green), Convert.ToInt16(blu));
                }
                else
                    this.lbl_info_utente.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);

                string Tema = GetCssAmministrazione();
                string veroTema = string.Empty;
                if (Tema != null && !Tema.Equals(""))
                {
                    string[] realTema = Tema.Split('^');
                    veroTema = realTema[0];
                }
                else
                    veroTema = "TemaRosso";

                this.btn_doc.DisabledUrl = "~/App_Themes/" + veroTema + "/" + this.btn_doc.Thema + this.btn_doc.SkinID + ".gif";
                this.btn_config.DisabledUrl = "~/App_Themes/" + veroTema + "/" + this.btn_config.Thema + this.btn_config.SkinID + ".gif";
                this.btn_gest.DisabledUrl = "~/App_Themes/" + veroTema + "/" + this.btn_gest.Thema + this.btn_gest.SkinID + ".gif";
                this.btn_help.DisabledUrl = "~/App_Themes/" + veroTema + "/" + this.btn_help.Thema + this.btn_help.SkinID + ".gif";
                this.btn_search.DisabledUrl = "~/App_Themes/" + veroTema + "/" + this.btn_search.Thema + this.btn_search.SkinID + ".gif";
            }
        }
        #endregion

        private string GetCssAmministrazione()
        {
            string Tema = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
            {
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                DocsPAWA.UserManager userM = new UserManager();
                Tema = utils.InitImagePath.getInstance(idAmm).getPath("CSS");


            }
            else
            {
                if (UserManager.getInfoUtente() != null && !string.IsNullOrEmpty(UserManager.getInfoUtente().idAmministrazione))
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    DocsPAWA.UserManager userM = new UserManager();
                    Tema = utils.InitImagePath.getInstance(idAmm).getPath("CSS");
                }
            }
            return Tema;
        }

        #region Gestione Funzioni
        /// <summary>
        /// ABILITA FUNZIONI
        /// </summary>
        public void enableFunction()
        {
            //this.img_logo.Attributes.Add("onclick", "ApriFrame('GestioneRuolo.aspx','principale');");
            this.img_logo.Attributes.Add("onclick", "top.principale.document.location='GestioneRuolo.aspx';");
            // string idAmm = "";
            if (UserManager.getInfoUtente() != null)
                idAmm = UserManager.getInfoUtente().idAmministrazione;

            DocsPaWR.Funzione userFunz;

            for (int j = 0; j < userRuolo.funzioni.Length; ++j)
            {
                userFunz = ((DocsPAWA.DocsPaWR.Funzione)(userRuolo.funzioni[j]));

                switch (userFunz.codice.Trim())
                {
                    case "MENU_DOCUMENTI":
                        this.btn_doc.Enabled = false;
                        // this.btn_doc.DisabledUrl = utils.InitImagePath.getInstance(idAmm).getPath("DOCUMENTI");
                        this.mnubar1.Attributes.Add("onClick", "openIt(0);");
                        break;
                    case "MENU_GESTIONE":
                        this.btn_gest.Enabled = false;
                        //this.btn_gest.DisabledUrl = utils.InitImagePath.getInstance(idAmm).getPath("GESTIONE");
                        this.mnubar3.Attributes.Add("onClick", "openIt(2);");
                        break;
                    case "MENU_OPZIONI":
                        this.btn_config.Enabled = false;
                        //this.btn_config.DisabledUrl = utils.InitImagePath.getInstance(idAmm).getPath("OPZIONI");
                        this.mnubar4.Attributes.Add("onClick", "openIt(3);");
                        break;
                }
            }

            //la ricerca è sempre abilitata!
            btn_search.Enabled = false;
            //this.btn_search.DisabledUrl = utils.InitImagePath.getInstance(idAmm).getPath("RICERCA");
            btn_help.Enabled = true;
            this.mnubar2.Attributes.Add("onClick", "openIt(1);");
            // this.mnubar5.Attributes.Add("onClick", "OpenHelp('" + this.from + "');");
        }

        /// <summary>
        /// Controllo di visualizzazione delle voci di menù rispetto alle chiavi del web.config
        /// </summary>
        protected void checkChiaviWebConfig()
        {
            // menù DOCUMENTI
            for (int i = 0; i < this.menuDoc.Links.Count; i++)
            {
                if (!this.menuDoc.Links[i].Visible)
                    this.menuDoc.WidthTable = this.menuDoc.WidthTable - this.menuDoc.Links[i].WidthCell;
            }

            // menù GESTIONE
            for (int i = 0; i < this.menuGest.Links.Count; i++)
            {
                switch (this.menuGest.Links[i].Text)
                {
                    case "News":
                        DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                        string result = wws.getNews(idAmm);
                        if (!string.IsNullOrEmpty(result))
                        {
                            string[] res = result.Split('^');
                            if (res[0].ToString().Equals("1"))
                            {
                                this.menuGest.Links[i].Visible = true;
                                this.menuGest.Links[i].ClientScriptAction = "OpenNews('" + res[1].ToString() + "');";
                            }
                            else
                                this.menuGest.Links[i].Visible = false;
                        }
                        break;


                    case "Codici IPA":
                        //Soluzione n.ro 1
                        DocsPaWR.DocsPaWebService wws1 = new DocsPAWA.DocsPaWR.DocsPaWebService();
                        string result1 = wws1.getNews(idAmm);
                        if (!string.IsNullOrEmpty(result1))
                        {
                            string[] res = result1.Split('^');
                            if (res[0].ToString().Equals("1"))
                            {
                                this.menuGest.Links[i].Visible = true;
                                //this.menuGest.Links[i].ClientScriptAction = String.Format("window.open('{0}');",res[1].ToString());

                                this.menuGest.Links[i].ClientScriptAction = "OpenCodici_IPA('" + Utils.getHttpFullPath() + res[1].ToString() + "');";
                            }
                            else
                                this.menuGest.Links[i].Visible = false;
                        }
                        
                        break;


                    case "Liste":
                        if (System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null && System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "1")
                            this.menuGest.Links[i].Visible = true;
                        else
                        {
                            this.menuGest.Links[i].Visible = false;
                            this.menuGest.WidthTable = this.menuGest.WidthTable - this.menuGest.Links[i].WidthCell;

                        }
                        break;
                    case "Piani Rientro":
                        if (System.Configuration.ConfigurationManager.AppSettings["PianiRientro"] != null && System.Configuration.ConfigurationManager.AppSettings["PianiRientro"] == "1")
                            this.menuGest.Links[i].Visible = true;
                        else
                        {
                            this.menuGest.Links[i].Visible = false;
                            this.menuGest.WidthTable = this.menuGest.WidthTable - this.menuGest.Links[i].WidthCell;

                        }
                        break;

                    case "ADL Doc.":
                        if (System.Configuration.ConfigurationManager.AppSettings["NUOVA_ADL"] != null && System.Configuration.ConfigurationManager.AppSettings["NUOVA_ADL"] == "1")
                        {
                            this.menuGest.Links[i].Visible = false;

                        }
                        break;
                    case "ADL Fasc.":
                        if (System.Configuration.ConfigurationManager.AppSettings["NUOVA_ADL"] != null && System.Configuration.ConfigurationManager.AppSettings["NUOVA_ADL"] == "1")
                        {
                            this.menuGest.Links[i].Visible = false;
                        }
                        break;
                    case "Deposito":
                        if (!UserManager.ruoloIsAutorized(this, "DO_ARCHIVIA") && !UserManager.ruoloIsAutorized(this, "GEST_ARCHIVIA"))
                        {
                            this.menuGest.Links[i].Visible = false;
                        }
                        else
                        {
                            this.menuGest.Links[i].Visible = true;
                            if (UserManager.ruoloIsAutorized(this, "DO_ARCHIVIA"))
                            {
                                this.menuGest.Links[i].Text = "Deposito";
                            }
                            else
                            {
                                this.menuGest.Links[i].Text = "Corrente";
                            }
                        }
                        break;
                    case "Scarto":
                        if (UserManager.ruoloIsAutorized(this, "GEST_ARCHIVIA"))
                        {
                            this.menuGest.Links[i].Visible = true;
                        }
                        else
                            this.menuGest.Links[i].Visible = false;
                        break;
                    case "Deleghe":
                        //Verifica se nel sistema esiste la microfunzione "GEST_DELEGHE"
                        bool gestDelega = false;
                        if (Session["GestioneDeleghe"] == null)
                        {
                            if (UserManager.FunzioneEsistente(this, "GEST_DELEGHE"))
                            {
                                Session.Add("GestioneDeleghe", true);
                                this.menuGest.Links[i].Visible = true;
                            }
                            else
                            {
                                Session.Add("GestioneDeleghe", false);
                                this.menuGest.Links[i].Visible = false;
                            }
                        }
                        else
                        {
                            if (Convert.ToBoolean(Session["GestioneDeleghe"]))
                                this.menuGest.Links[i].Visible = true;
                            else
                                this.menuGest.Links[i].Visible = false;
                        }
                        break;
                    case "Elenco note":
                        if (UserManager.ruoloIsAutorized(this, "ELENCO_NOTE"))
                            this.menuGest.Links[i].Visible = true;
                        else
                            this.menuGest.Links[i].Visible = false;
                        break;
                    case "Stampa unione":
                        string valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_STAMPA_UNIONE");
                        if("1".Equals(valoreChiave)){
                            this.menuGest.Links[i].Visible = true;
                        }else{
                            this.menuGest.Links[i].Visible = false;
                        }
                        break;
                }
            }
            //menu OPZIONI
            if (this.menuConf.Links[0].Text.Equals("Cambia Password"))
            {
                if ((Session["ESERCITADELEGA"] != null) && (Convert.ToBoolean(Session["EsercitaDelega"])))
                    this.menuConf.Links[0].Visible = false;
                else
                    this.menuConf.Links[0].Visible = true;
            }

            //menu RICERCA
            for (int i = 0; i < this.menuRic.Links.Count; i++)
            {
                switch (this.menuRic.Links[i].Text)
                {

                    case "ADL Doc.":
                        if (System.Configuration.ConfigurationManager.AppSettings["NUOVA_ADL"] != null && System.Configuration.ConfigurationManager.AppSettings["NUOVA_ADL"] == "1")
                        {
                            this.menuRic.Links[i].Visible = true;
                            this.menuRic.Links[i].Href = "RicercaDoc/gestioneRicDoc.aspx?tab=estesa&ricADL=1";
                            this.menuRic.Links[i].Target = "principale";
                        }
                        else this.menuRic.Links[i].Visible = false;
                        break;
                    case "ADL Fasc.":
                        if (System.Configuration.ConfigurationManager.AppSettings["NUOVA_ADL"] != null && System.Configuration.ConfigurationManager.AppSettings["NUOVA_ADL"] == "1")
                        {
                            this.menuRic.Links[i].Visible = true;
                            this.menuRic.Links[i].Href = "RicercaFascicoli/gestioneRicFasc.aspx?ricADL=1";
                            this.menuRic.Links[i].Target = "principale";
                        }
                        else this.menuRic.Links[i].Visible = false;
                        break;
                }
            }
        }

        #endregion

        private bool fileExist(string fileName, string type)
        {
            return FileManager.fileExist(fileName, type);
        }

        #region Gestione Tasti
        /// <summary>
        /// TASTO DOCUMENTI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_doc_Click(object sender, EventArgs e)
        {
            try
            {
                //rimuovo gli oggetti che sono in sessione relativi al documento			
                DocumentManager.removeRisultatoRicerca(this);

                DocumentManager.removeFiltroRicDoc(this);
                DocumentManager.removeDatagridDocumento(this);
                DocumentManager.removeClassificazioneSelezionata(this);
                Session.Remove("rubrica.campoCorrispondente");
                Session.Remove("dictionaryCorrispondente");
                Session.Remove("CorrSelezionatoDaMulti");

                //annullamento variabili di sessione impostate 
                //dalla gestione ricerca fascicoli
                FascicoliManager.SetFolderViewTracing(this, false);
                this.CleanSessionMemoria();
                //   this.btn_doc.DisabledUrl = utils.InitImagePath.getInstance(idAmm).getPath("DOCUMENTI_ATTIVO");

                // Impostazione contesto chiamante
                //this.SetCallerContext();

                //Annullamento variabile in sessione per lo scorrimento delle liste
                //UserControls.ScrollElementsList.ScrollManager.clearSessionObjScrollElementsList();
                UserControls.ScrollElementsList.ScrollManager.clearContextObjScrollElementsList();
            }
            catch (Exception ex)
            {
                string f = ex.Message.ToString();
                ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        protected void btn_help_Click(object sender, System.EventArgs e)
        {
            this.from = (string)Session["Bookmark"];
            if (this.from == "" || this.from == null)
                this.from = "Home";

            string function = "<script language='javascript'>";
            function += "window.showModalDialog('Help/Manuale.aspx?from=" + this.from + "','', 'dialogWidth:900px;dialogHeight:600px;status:no;resizable:no;scroll:yes;dialogLeft:112;dialogTop:84;center:yes;help:no');";
            //function += "window.showModalDialog('Help/manuale-pitre.chm','', 'dialogWidth:900px;dialogHeight:600px;status:no;resizable:no;scroll:yes;dialogLeft:112;dialogTop:84;center:yes;help:no');";
            //function += "window.open('Help/manuale-pitre.chm::/Registri.html','', 'dialogWidth:900px;dialogHeight:600px;status:no;resizable:no;scroll:yes;dialogLeft:112;dialogTop:84;center:yes;help:no');";
            function += "</script>";

            this.RegisterClientScriptBlock("openHelp", function);

            // this.RegisterStartupScript("openHelp", "<script type='javascript'>" + function + "</script>");
        }

        /// <summary>
        /// TASTO GESTIONE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_gest_Click(object sender, System.EventArgs e)
        {
            int msgRtn;
            System.Web.UI.WebControls.CommandEventArgs ev = (System.Web.UI.WebControls.CommandEventArgs)e;
            try
            {
                if (ev.CommandArgument.Equals("GEST_FAX"))
                {
                    DocsPaWR.DocsPaWebService WS = ProxyManager.getWS();
                    infoUt = UserManager.getInfoUtente(this);
                    userRuolo = UserManager.getRuolo(this);
                    userReg = userRuolo.registri[0];
                    msgRtn = WS.FaxProcessaCasella(Utils.getHttpFullPath(this), infoUt, userRuolo, userReg);
                    if (msgRtn < 0)
                    {
                        logger.Error("Errore nella testata (GEST_FAX)");
                        throw new Exception();
                    }
                    switch (msgRtn)
                    {
                        case 0:
                            Response.Write("<script>alert('Nelle caselle Fax controllate,\\nnon risultano nuovi Fax da Processare ')</script>");
                            break;
                        case 1:
                            Response.Write("<script>alert('Trovato " + msgRtn.ToString() + " Fax,\\nconsultare la lista COSE DA FARE per vedere la trasmissione ad esso relativa.')</script>");
                            break;
                        default:
                            Response.Write("<script >alert('Trovati " + msgRtn.ToString() + " Fax,\\nconsultare la lista COSE DA FARE per vedere le trasmissioni ad essi relativa.')</script>");
                            break;
                    }
                }

                if (ev.CommandArgument.Equals("GEST_REGISTRI"))
                    GestManager.removeRegistroSel(this);

                if (!ev.CommandArgument.Equals("GEST_PROSPETTI") &&
                    !ev.CommandArgument.Equals("GEST_RUBRICA") &&
                    !ev.CommandArgument.Equals("GEST_MODELLI") &&
                    !ev.CommandArgument.Equals("GEST_ORGANIGRAMMA") &&
                    !ev.CommandArgument.Equals("GEST_AREA_LAV")
                    )
                {
                    this.CleanSessionMemoria();
                }

                //this.btn_gest.DisabledUrl = utils.InitImagePath.getInstance(idAmm).getPath("GESTIONE_ATTIVO");

                if (!ev.CommandArgument.Equals("GEST_ARCHIVIO_CARTACEO"))
                {
                    // Rimozione risorse 
                    FascicolazioneCartacea.SessionManager.Clear();
                }

                //Annullamento variabile in sessione per lo scorrimento delle liste
                //UserControls.ScrollElementsList.ScrollManager.clearSessionObjScrollElementsList();
                UserControls.ScrollElementsList.ScrollManager.clearContextObjScrollElementsList();
            }
            catch (Exception ex)
            {
                string f = ex.Message.ToString();
                ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        /// <summary>
        /// TASTO RICERCA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_search_Click(object sender, EventArgs e)
        {
            try
            {
                System.Web.UI.WebControls.CommandEventArgs ev = (System.Web.UI.WebControls.CommandEventArgs)e;
                if (!ev.CommandArgument.Equals("DO_RIC_VISIBILITA"))
                {
                    //SALVA RICERCHE :
                    DocumentManager.removeDocumentoInLavorazione(this);
                    DocumentManager.removeFiltroRicDoc(this);
                    TrasmManager.removeDataTableEff(this);
                    TrasmManager.removeDataTableRic(this);
                    TrasmManager.removeDocTrasmQueryEff(this);
                    TrasmManager.removeDocTrasmQueryRic(this);
                    DocumentManager.removeDatagridDocumento(this);
                    DocumentManager.removeFiltroRicTrasm(this);
                    FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                    FascicoliManager.removeCodiceFascRapida(this);
                    FascicoliManager.removeDescrizioneFascRapida(this);
                    FascicoliManager.removeFiltroRicFasc(this);
                    FascicoliManager.removeMemoriaFiltriRicFasc(this);
                    UserManager.removeCorrispondentiSelezionati(this);
                    Session.Remove(DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY);
                    this.CleanSessionMemoria();
                    //this.btn_search.DisabledUrl = utils.InitImagePath.getInstance(idAmm).getPath("RICERCA_ATTIVO");
                }

                if (ev.CommandArgument.Equals("FASC_GESTIONE"))
                {
                    //se utilizzo pagina ricerca fascicoli da popup del pulsante fasc. rapida, e faccio una ricerca, poi non
                    //subito dopo faccio un  ricerca fascicoli, rimaneva la predende  ricerca in sessione perchè rimanevano i filtri della ricerca
                    FascicoliManager.removeFiltroRicFasc(this);
                }

                //Annullamento variabile in sessione per lo scorrimento delle liste
                //UserControls.ScrollElementsList.ScrollManager.clearSessionObjScrollElementsList();
                UserControls.ScrollElementsList.ScrollManager.clearContextObjScrollElementsList();

                //se  non si è in modifica griglia temporanea distruggo la griglia (forza il cambio contesto)
                if (utils.GridManager.SelectedGrid != null)
                {
                    utils.GridManager.SelectedGrid = null;
                }
                Session.Remove("templateRicerca");
            }
            catch (Exception ex)
            {
                string f = ex.Message.ToString();
                ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        /// <summary>
        /// TASTO LOGOUT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_logout_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.Page.IsStartupScriptRegistered("alertJS"))
            {

                string appConfigValue = ConfigSettings.getKey(ConfigSettings.KeysENUM.DISABLE_LOGOUT_CLOSE_BUTTON);

                if (appConfigValue == null || (!Convert.ToBoolean(appConfigValue)))
                Session.Abandon();

               
                string scriptString = "<SCRIPT>window.parent.close();</SCRIPT>";
                this.ClientScript.RegisterStartupScript(this.GetType(), "alertJS", scriptString);
            }

            UserManager.logoff(this);
        }

       
        //public static bool AbandonSession()
        //{
        //    HttpContext.Current.Session.Abandon();
        //    return true;
        //}
        #endregion

        #region Gestione Memoria
        /// <summary>
        /// Pulisce Sessione Risultati della Ricerca
        /// </summary>
        private void CleanSessionRisultatiRicerca()
        {
            TrasmManager.removeDocTrasmSel(this);
            TrasmManager.removeDocTrasmQueryEff(this);
            TrasmManager.removeDataTableRic(this);
            TrasmManager.removeDataTableEff(this);
            TrasmManager.removeDataTableRic(this);
        }

        /// <summary>
        /// Pulisce Sessione Memoria
        /// </summary>
        private void CleanSessionMemoria()
        {
            DocumentManager.removeMemoriaFiltriRicDoc(this);
            DocumentManager.removeMemoriaNumPag(this);
            DocumentManager.removeMemoriaTab(this);
            DocumentManager.RemoveMemoriaVisualizzaBack(this);
            DocumentManager.RemoveMemoriaSalvaDoc(this);

            TrasmManager.RemoveMemoriaVisualizzaBack(this);
            TrasmManager.removeHashTrasmOggTrasm(this);



            // commentato perchè altrimenti export da ric fasc ADL al primo click in assoluto non funziona
            //FascicoliManager.removeMemoriaRicFasc(this);

            FascicoliManager.RemoveMemoriaVisualizzaBack(this);
            FascicoliManager.SetFolderViewTracing(this, false);
            FascicoliManager.removeFascicoloSelezionatoFascRapida(this);

            DocumentManager.removeFiltroRicTrasm(this);
        }
        #endregion

        #region Gestione Contesti

        /// <summary>
        /// Impostazione contesto corrente
        /// </summary>
        private void SetContext()
        {
            if (SiteNavigation.CallContextStack.IsEmpty)
            {
                string url = DocsPAWA.Utils.getHttpFullPath() + "/GestioneRuolo.aspx";

                SiteNavigation.CallContext newContext = new SiteNavigation.CallContext(SiteNavigation.NavigationKeys.PAGINA_INIZIALE, url);
                newContext.ContextFrameName = "top.principale";
                SiteNavigation.CallContextStack.SetCurrentContext(newContext);
            }
        }

        /// <summary>
        /// tasto back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_logo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Session["Bookmark"] = "Home";
            SiteNavigation.CallContextStack.Clear();
        }

        #endregion

        private void getLettereProtocolli()
        {
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPAWA.DocsPaWR.InfoUtente infoUtente;
            infoUtente = UserManager.getInfoUtente(this);
            DocsPAWA.DocsPaWR.EtichettaInfo[] etichette = wws.getEtichetteDocumenti(infoUtente, infoUtente.idAmministrazione);
            if (etichette != null)
            {
                this.eti_prot_ingresso = "Prot. " + etichette[0].Etichetta; //Valore A
                this.eti_prot_partenza = "Prot. " + etichette[1].Etichetta; //Valore P
            }
        }
    }
}
