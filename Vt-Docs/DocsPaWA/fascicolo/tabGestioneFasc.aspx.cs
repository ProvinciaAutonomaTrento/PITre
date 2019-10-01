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
using System.Globalization;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA
{
    /// <summary>
    /// Summary description for tab_gestioneDoc.
    /// </summary>
    public class tabGestioneFasc : DocsPAWA.CssPage
    {
        protected System.Web.UI.WebControls.Label lbl_registri;
        protected System.Web.UI.WebControls.Image img_statoReg;
        protected System.Web.UI.HtmlControls.HtmlTableCell td_tab1;
        protected System.Web.UI.HtmlControls.HtmlTableCell td_tab2;
        protected System.Web.UI.HtmlControls.HtmlTableCell td_tab3;
        protected System.Web.UI.HtmlControls.HtmlTableCell td_tab4;
        protected System.Web.UI.HtmlControls.HtmlTableCell td_tab5;
        protected System.Web.UI.HtmlControls.HtmlTableCell td_tab6;
        protected string nomeTab;
        protected DocsPAWA.DocsPaWR.Utente userHome;
        protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
        protected DocsPaWebCtrlLibrary.ImageButton btn_documenti;
        protected DocsPaWebCtrlLibrary.ImageButton btn_trasmissioni;
        protected DocsPaWebCtrlLibrary.IFrameWebControl IframeTabs;
        protected System.Web.UI.WebControls.Image icoReg;
        protected System.Web.UI.WebControls.Panel pnl_regStato;
        protected DocsPAWA.DocsPaWR.Registro[] userRegistri;


        private void Page_Load(object sender, System.EventArgs e)
        {

            Utils.startUp(this);

            userHome = UserManager.getUtente(this);
            userRuolo = UserManager.getRuolo(this);


            if (!IsPostBack)
            {
                // Caricamento della griglia e suo salvataggio se non ce n'è una già salvata
             //   if (GridManager.SelectedGrid == null)
               //     GridManager.SelectedGrid = GridManager.GetStandardGridForUser(GridTypeEnumeration.DocumentInProject);

                this.btn_documenti.Attributes.Add("onClick", "ShowWaitingPage();");
                this.btn_trasmissioni.Attributes.Add("onClick", "ShowWaitingPage();");

                // Aggiornamento contesto corrente
                this.RefreshCallContext("Documenti");

                //CaricaComboRegistri(ddl_registri);
                //carica il ruolo scelto
                if (userRuolo != null)
                {
                    //this.lbl_ruolo.Text = userRuolo.descrizione; 

                    //this.lbl_registri.Text=UserManager.getRegistroSelezionato(this).descrizione;
                    DocsPaWR.Registro registroSelezionato = UserManager.getRegistroSelezionato(this);
                    if (registroSelezionato == null)
                    {
                        this.settaPrimoRegistroSelezionato();
                        registroSelezionato = UserManager.getRegistroSelezionato(this);
                    }
                    
                   
                        this.pnl_regStato.Visible = true;
                        //this.pnl_riga.Visible = true;
                        this.lbl_registri.Text = registroSelezionato.codRegistro;
                        this.setStatoReg(registroSelezionato);
                        string rigaDescrizione = "<tr><td align=\"center\" height=\"15\" class=\"titolo_bianco\" bgcolor=\"#810d06\">Registro</td></tr>";

                        Page.RegisterClientScriptBlock("CallDescReg", "<!--Desc Reg inzio--><DIV onmouseover=\"closeIt()\"><LAYER onmouseover=\"closeIt()\"></LAYER></DIV><DIV id=\"descreg\" style=\"visibility:hidden;LEFT: 200px; POSITION: absolute; TOP: 24px\"><div align=\"left\"><table cellSpacing=\"0\" border='1' bordercolordark='#ffffff' cellPadding=\"0\" bgColor=\"#d9d9d9\"  width='210px' height='60px'>" + rigaDescrizione + "<tr><td  bgColor=\"#d9d9d9\" class=\"testo_grigio_sp\">" + UserManager.getRegistroSelezionato(this).descrizione + "</td></tr></table></div></DIV><!--Fine desc reg-->");
                   
                    //Page.RegisterClientScriptBlock("CallDescReg","<!--Desc Reg inzio--><DIV onmouseover=\"closeIt()\"><LAYER onmouseover=\"closeIt()\"></LAYER></DIV><DIV id=\"descreg\" style=\"visibility:hidden;LEFT: 230px; POSITION: absolute; TOP: 60px\"><div align=\"left\"><table cellSpacing=\"0\" border='1' bordercolordark='#ffffff' cellPadding=\"0\" bgColor=\"#d9d9d9\"  width='100px' height='60px'><tr><td  bgColor=\"#d9d9d9\" class=\"testo_grigio_sp\">"+UserManager.getRegistroSelezionato(this).descrizione+"</td></tr></table></div></DIV><!--Fine desc reg-->");	

                }
                //////
                //////				if(Request.QueryString["back"] != null && !Request.QueryString["back"].Equals(""))
                //////				{
                //////					DocsPaWR.Fascicolo fascSel = FascicoliManager.getMemoriaFascicoloSelezionato(this);
                //////					DocsPaWR.Folder folderSel = FascicoliManager.getMemoriaFolderSelezionata(this);
                //////
                //////					FascicoliManager.setFascicoloSelezionato(this,fascSel);
                //////					FascicoliManager.setFolderSelezionato(this,folderSel);
                //////				}

            }

            if ((!Request.QueryString["tab"].Equals("")) && (!Request.QueryString["tab"].Equals(null)))
            {
                nomeTab = Request.QueryString["tab"].ToString();
                CaricaTab(nomeTab);
            }

            DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato(this);

            //si abilitano le trasmissioni solo per i fascicoli procedimentali
            if (!fasc.tipo.Equals("P"))
            {
                this.btn_trasmissioni.Enabled = false;
            }
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            //abilitazione delle funzioni in base al ruolo
            UserManager.disabilitaFunzNonAutorizzate(this);

            // Impostazione fascicolo corrente nel contesto corrente
            this.SetFascicoloOnContext();

            // Impostazione dell'immagineper i bottoni della pulsantiera
            this.RefreshTabButtonsImages();
        }

        /// <summary>
        /// Funzione per l'impostazione delle immagini da associare ai pulsanti della bottoniera
        /// </summary>
        private void RefreshTabButtonsImages()
        {
            // Inizializzazione a non selezionato di entrambi i pulsanti della bottoniera
            this.btn_documenti.ImageUrl = this.GetTabButtonImageUrl(this.btn_documenti, false);
            this.btn_trasmissioni.ImageUrl = this.GetTabButtonImageUrl(this.btn_trasmissioni, false);

            // Reperimento dell'id del bottone selezionato
            string selectedButtonID = this.ViewState["ID_Butt_precedente"] as string;

            // Se non è null o empty, si procede con la selezione dell'immagine per il selezionato
            // altrimenti viene selezionato come attivo il pulsante Documenti
            if (selectedButtonID != null)
            {
                ImageButton selectedButton = ((ImageButton)Page.FindControl(selectedButtonID));
                selectedButton.ImageUrl = this.GetTabButtonImageUrl(selectedButton, true);
            }
            else
                this.btn_documenti.ImageUrl = this.GetTabButtonImageUrl(this.btn_documenti, true);

        }

        /// <summary>
        /// Funzione per la costruzione del path dell'immagine da associare ad un determinato
        /// pulsante
        /// </summary>
        /// <param name="imageButton">L'image button a cui associare l'immagine</param>
        /// <param name="selectedImage">True se bisogna visualizzare l'immagine per il selezionato</param>
        /// <returns>L'url dell'immagine</returns>
        private string GetTabButtonImageUrl(ImageButton imageButton, bool selectedImage)
        {
            string retValue = string.Empty;

            string buttonID = imageButton.ID;
            buttonID = buttonID.Substring(buttonID.IndexOf("_") + 1);

            bool contain = false;

            if (buttonID == "trasmissioni")
                contain = this.ContainsTrasmissioni();
            else
                buttonID = "tab_" + buttonID;
            
            string enabledImageUrl = string.Empty;

            if (selectedImage)
                retValue = "_attivo";
            else
                retValue = "_nonattivo";

            if (contain)
                retValue = "_presente" + retValue;

            return "~/App_Themes/" + this.Page.Theme + "/" + buttonID + retValue + ".gif";
        }

        /// <summary>
        /// Verifica se il documento contiene trasmissioni
        /// </summary>
        /// <returns></returns>
        private bool ContainsTrasmissioni()
        {
            bool retValue = false;
            int idProject;
            DocsPAWA.DocsPaWR.Fascicolo project;

            project = FascicoliManager.getFascicoloSelezionato(this);

            if (project != null)
            {
                try
                {
                    idProject = Convert.ToInt32(project.systemID);

                    if (idProject != 0)
                    {
                        DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                        retValue = (ws.FascicoloGetCountTrasmissioniFascicolo(idProject) > 0);
                    }
                }
                catch { }
            }

            return retValue;
        }

        private void btn_Click(string strNumTab, ImageButton btn)
        {
            //solo se già un butt è stato cliccato
            //if (ViewState["ID_Butt_precedente"] != null)
            //{

                //img del butt precedentemente cliccato torna a nonattivo.	

            //    ImageButton btnPrec = ((ImageButton)Page.FindControl((string)ViewState["ID_Butt_precedente"]));
            //    btnPrec.ImageUrl = "../images/fasc/" + btnPrec.ID.Substring(btn.ID.IndexOf("_") + 1) + "_nonattivo.gif";

            //}
            //serve per segnare la prima volta che si click un bottone.
            if (Page.IsPostBack)
                ViewState.Add("ID_Butt_precedente", btn.ID);

            //cambio img del butt cliccato
            //btn.ImageUrl = "../images/fasc/" + strNumTab + "_attivo.gif";
        }

        private void btn_documenti_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            string strNumTab = btn.ID.Substring(btn.ID.IndexOf("_") + 1);
            btn_Click(strNumTab, btn);
            IframeTabs.NavigateTo = "Fasc" + strNumTab + ".aspx";

            // Aggiornamento contesto corrente
            this.RefreshCallContext(strNumTab);
        }


        private void btn_trasmissioni_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato(this);
            DocsPaWR.RagioneTrasmissione[] listaRagioni = TrasmManager.getListaRagioniFasc(this, fasc);
            if (listaRagioni.Length == 0 && !fasc.stato.Equals("A"))
            {
                RegisterStartupScript("alert", @"<script>alert('Fascicolo chiuso. Non sono configurate ragioni in sola lettura utilizzabili per poterlo trasmettere.\nContattare l\'amministratore .');</script>");
                return;
            }

            ImageButton btn = (ImageButton)sender;
            string strNumTab = btn.ID.Substring(btn.ID.IndexOf("_") + 1);
            btn_Click(strNumTab, btn);
            IframeTabs.NavigateTo = "Fasc" + strNumTab + ".aspx";

            // Aggiornamento contesto corrente
            this.RefreshCallContext(strNumTab);
        }

        private void btn_procedimentali_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            string strNumTab = btn.ID.Substring(btn.ID.IndexOf("_") + 1);
            btn_Click(strNumTab, btn);
            IframeTabs.NavigateTo = "Fasc" + strNumTab + ".aspx";
        }

        private void CaricaTab(string nomeTab)
        {
            string nomeButtTab = "btn_" + nomeTab;

            ImageButton ButtImg = (ImageButton)Page.FindControl(nomeButtTab);
            btn_Click(nomeTab, ButtImg);

            string queryString = string.Empty;
            if (this.OnBackContext && this.Request.QueryString["docIndex"] != null)
                queryString = "?back=true&docIndex=" + this.Request.QueryString["docIndex"];

            IframeTabs.NavigateTo = "Fasc" + nomeTab + ".aspx" + queryString;
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
            this.btn_documenti.Click += new System.Web.UI.ImageClickEventHandler(this.btn_documenti_Click);
            this.btn_trasmissioni.Click += new System.Web.UI.ImageClickEventHandler(this.btn_trasmissioni_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);

        }
        #endregion




        #region new_setStatoReg
        /*
		private void setStatoReg(DocsPAWA.DocsPaWR.Registro  reg)
		{
			// inserisco il registro selezionato in sessione			
			UserManager.setRegistroSelezionato(this, reg);

			string dataApertura=reg.dataApertura;
			string nomeImg;
			DateTime dt_cor = DateTime.Now;
			
			CultureInfo ci = new CultureInfo("it-IT");

			
			string[] formati={"dd/MM/yyyy HH.mm.ss","dd/MM/yyyy H.mm.ss"};

			DateTime d_ap = DateTime.ParseExact(dataApertura,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);
			//aggiungo un giorno per fare il confronto con now (che comprende anche minuti e secondi)
			d_ap = d_ap.AddDays(1);
		
			string mydate = dt_cor.ToString(ci);

			//DateTime dt = DateTime.ParseExact(mydate,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);

			string StatoAperto=ConfigurationManager.AppSettings["STATO.REG.APERTO"];
			if (reg.stato.Equals(StatoAperto)) 
			{
				if (dt_cor.CompareTo(d_ap)>0)
				{
					//data odierna maggiore della data di apertura del registro
					nomeImg = "stato_giallo.gif";
				} 
				else
					nomeImg = "stato_verde.gif";
			}
			else
				nomeImg = "stato_rosso.gif";	

			this.img_statoReg.ImageUrl = "../images/" + nomeImg;
		}
*/

        private void setStatoReg(DocsPAWA.DocsPaWR.Registro reg)
        {
            // inserisco il registro selezionato in sessione			
            UserManager.setRegistroSelezionato(this, reg);
            string nomeImg;

            if (UserManager.getStatoRegistro(reg).Equals("G"))
                nomeImg = "stato_giallo2.gif";
            else if (UserManager.getStatoRegistro(reg).Equals("V"))
                nomeImg = "stato_verde2.gif";
            else
                nomeImg = "stato_rosso2.gif";

            this.img_statoReg.ImageUrl = "../images/" + nomeImg;
        }

        private void settaPrimoRegistroSelezionato()
        {


            DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistri(this);
            DocsPaWR.Registro  userReg = userRegistri[0];
            UserManager.setRegistroSelezionato(this, userReg);
            setStatoReg(userReg);
            //attenzione! ripulire i campi relativi al mittente e all'oggetto che dipendono dal registro	

        }
       


        #endregion

        #region Gestione callcontext

        /// <summary>
        /// Verifica se si è in un contesto di back
        /// </summary>
        private bool OnBackContext
        {
            get
            {
                return (this.Request.QueryString["back"] != null &&
                        this.Request.QueryString["back"] != string.Empty);
            }
        }

        /// <summary>
        /// Ripristino dati contesto chiamante
        /// </summary>
        private void RestoreCallContext()
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext.ContextName == SiteNavigation.NavigationKeys.FASCICOLO && currentContext.IsBack)
            {
                if (currentContext.ContextState.ContainsKey("systemId"))
                {
                    // Reperimento fascicolo
                    DocsPaWR.Fascicolo fascicolo = FascicoliManager.getFascicolo(this, currentContext.ContextState["systemId"].ToString());

                    FascicoliManager.setFascicoloSelezionato(fascicolo);
                }
            }
        }

        /// <summary>
        /// Aggiornamento call context
        /// </summary>
        /// <param name="tabName"></param>
        private void RefreshCallContext(string tabName)
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext.ContextName == SiteNavigation.NavigationKeys.FASCICOLO)
                currentContext.QueryStringParameters["tab"] = tabName;
        }

        /// <summary>
        /// Impostazione fascicolo corrente nel contesto corrente
        /// </summary>
        private void SetFascicoloOnContext()
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext.ContextName == SiteNavigation.NavigationKeys.FASCICOLO)
            {
                DocsPaWR.Fascicolo fascicolo = FascicoliManager.getFascicoloSelezionato();
                if (fascicolo != null)
                    currentContext.ContextState["systemId"] = fascicolo.systemID;
            }

            // currentContext.SessionState["FascicoloSelezionato"]=FascicoliManager.getFascicoloSelezionato();
        }

        #endregion
    }
}
