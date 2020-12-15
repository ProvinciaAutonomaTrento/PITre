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

namespace DocsPAWA.ricercaDoc
{
	/// <summary>
	/// Summary description for tabGestioneRicDoc.
	/// </summary>
	public class tabGestioneRicDoc : DocsPAWA.CssPage
	{
		protected DocsPAWA.DocsPaWR.Utente userHome;
		protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
		protected System.Web.UI.WebControls.Label lbl_ruolo;
		protected System.Web.UI.WebControls.ImageButton btn_veloce;
		protected System.Web.UI.WebControls.ImageButton btn_estesa;
		protected System.Web.UI.WebControls.ImageButton btn_completa;
		protected System.Web.UI.WebControls.ImageButton btn_completamento;
		protected DocsPaWebCtrlLibrary.IFrameWebControl IframeTabs;
        protected System.Web.UI.WebControls.ImageButton btn_Grigia;
        protected System.Web.UI.WebControls.ImageButton btn_StampaReg;
        protected System.Web.UI.WebControls.ImageButton btn_StampaRep;
		protected string nomeTab;
        protected string ricercaGrigi;
        protected string pageTheme = string.Empty;
	
		private void initResultSearch(string page_dx)
		{
			DocumentManager.removeFiltroRicDoc(this);
			DocumentManager.removeDatagridDocumento(this);
			DocumentManager.removeRisultatoRicerca(this);
			DocumentManager.removeListaNonDocProt(this);
            if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
            {
                Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location = '" + page_dx + "&ricADL=1';</script>");
            }
            else
            {
                Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location = '" + page_dx + "';</script>");
            }
		}

        private void initResultSearchWaiting(string page_dx)
        {
            DocumentManager.removeFiltroRicDoc(this);
            DocumentManager.removeDatagridDocumento(this);
            DocumentManager.removeRisultatoRicerca(this);
            DocumentManager.removeListaNonDocProt(this);

            Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location = '" + page_dx + "';</script>");
        }

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
                Session["Bookmark"] = "RicercaDoc";
                
                Utils.startUp(this);
				// Put user code to initialize the page here
				userHome=(DocsPAWA.DocsPaWR.Utente) Session["userData"];
				userRuolo = (DocsPAWA.DocsPaWR.Ruolo) Session["userRuolo"];

                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_RICERCHE_GRIGI"]))
                    this.ricercaGrigi = System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_RICERCHE_GRIGI"];
                else
                    this.ricercaGrigi = "0";
                
                //nuova gestione rimozione sessione scheda
				DocumentManager.removeDocumentoInLavorazione(this);
	
				//rimuovo oggetto delle ricerche salvate
				// ogni qualvolta cambio contesto
				if ((IsPostBack)&& Session["itemUsedSearch"]!=null)
				{
					Session.Remove("itemUsedSearch");
           		}
		
				string nomeTab = "estesa";

				if(Request.QueryString["tab"] != string.Empty && Request.QueryString["tab"] != null)
				{
					nomeTab = Request.QueryString["tab"].ToString();
				}

                CaricaTab(nomeTab);

                //nascondo il tab stampa registro
                if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                {
                    btn_StampaReg.Enabled = false;
                    btn_StampaReg.ImageUrl = "../App_Themes/ImgComuni/tab_disattivo.gif";
                    btn_StampaRep.Enabled = false;
                    btn_StampaRep.ImageUrl = "../App_Themes/ImgComuni/tab_disattivo.gif";
                }

                //Rimuovo l'eventuale fascicolo  selezionato per la fascicolazione rapida
                if(IsPostBack)
                    FascicoliManager.removeFascicoloSelezionatoFascRapida(this);

                if (!Utils.isEnableRepertori(userRuolo.idAmministrazione))
                {
                    btn_StampaRep.Enabled = false;
                    btn_StampaRep.ImageUrl = "../App_Themes/ImgComuni/tab_disattivo.gif";
                }

                if (!IsPostBack)
                {
                    if(String.IsNullOrEmpty(Request["gridPer"]))
                        Session.Remove("itemUsedSearch");
                    DocumentManager.removeFiltroRicDoc(this);
                }
            }
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
		}


		private void btn_Click(string strNumTab,ImageButton btn)
		{
            string Tema = GetCssAmministrazione();
            //string pageTheme = string.Empty;
            if (Tema != null && !Tema.Equals(""))
            {
                string[] realTema = Tema.Split('^');
                pageTheme = realTema[0];
            }
            else
                pageTheme = "TemaRosso";
            
            //solo se già un butt è stato cliccato
			if(ViewState["ID_Butt_precedente"]!=null)
			{
				#region tasto back
				DocumentManager.removeMemoriaNumPag(this);
				DocumentManager.removeMemoriaTab(this);
				DocumentManager.removeMemoriaFiltriRicDoc(this);
				#endregion

				DocumentManager.removeRisultatoRicerca(this);
				
				//img del butt precedentemente cliccato torna a nonattivo.	
				ImageButton btnPrec=((ImageButton)Page.FindControl((string)ViewState["ID_Butt_precedente"]));			
				btnPrec.ImageUrl="../App_Themes/ImgComuni/"+btnPrec.ID.Substring(btn.ID.IndexOf("_")+1)+"_nonattivo.gif";				
			}
			//serve per segnare la prima volta che si click un bottone.
			if(Page.IsPostBack)
				ViewState.Add("ID_Butt_precedente",btn.ID);

            //cambio img del butt cliccato

            btn.ImageUrl = "../App_Themes/" + pageTheme + "/" + strNumTab + "_attivo.gif";
		
            if (btn != null && btn.ID == "btn_estesa")
            {
                //verifico se sono abilitate le ricerche limitate
                if (UserManager.ruoloIsAutorized(this, "DO_RICERCHE_LIMITATE"))
                {
                    btn.ImageUrl = "../App_Themes/" + pageTheme + "/ricerca_attivo.gif";
                    btn_completa.Enabled = false;
                    btn_completa.ImageUrl = "../App_Themes/ImgComuni/tab_disattivo.gif";
                    btn_completa.Enabled = false;
                    if (this.ricercaGrigi.Equals("1"))
                    {
                        btn_Grigia.ImageUrl = "../App_Themes/ImgComuni/tab_disattivo.gif";
                        btn_Grigia.Enabled = false;
                    }
                    btn_completamento.ImageUrl = "../App_Themes/ImgComuni/tab_disattivo.gif";
                    btn_completamento.Enabled = false;
                    btn_StampaReg.ImageUrl = "../App_Themes/ImgComuni/tab_disattivo.gif";
                    btn_StampaReg.Enabled = false;
                    btn_veloce.ImageUrl = "../App_Themes/ImgComuni/tab_disattivo.gif";
                    btn_veloce.Enabled = false;
                    btn_StampaRep.ImageUrl = "../App_Themes/ImgComuni/tab_disattivo.gif";
                    btn_StampaRep.Enabled = false;
                }
            }

			
			//rimuovo la variabile di sessione usata per i registri selezionati e con cui fare la ricerca
			UserManager.removeListaIdRegistri(this);
			DocumentManager.setMemoriaTab(this,strNumTab);
		}

        private string GetCssAmministrazione()
        {
            string Tema = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
            {
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                DocsPAWA.UserManager userM = new UserManager();
                Tema = userM.getCssAmministrazione(idAmm);
            }
            else
            {
                if (UserManager.getInfoUtente() != null)
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    DocsPAWA.UserManager userM = new UserManager();
                    Tema = userM.getCssAmministrazione(idAmm);
                }
            }
            return Tema;
        }

		private void btn_veloce_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Session.Remove(SchedaRicerca.SESSION_KEY);
            initResultSearch("NewTabSearchResult.aspx?tabRes=veloce");
			ImageButton btn=(ImageButton) sender;
			string strNumTab=btn.ID.Substring(btn.ID.IndexOf("_")+1);
			btn_Click(strNumTab,btn);
			IframeTabs.NavigateTo="RicDoc"+strNumTab+".aspx";
            
            if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
            {
                IframeTabs.NavigateTo += "?ricADL=" + Request.QueryString["ricADL"].ToString();
            }

			this.RefreshCallerContext(strNumTab);

			this.RefreshSearchPagingCallerContext();
            Session.Remove("templateRicerca");
		}

		private void btn_estesa_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Session.Remove(SchedaRicerca.SESSION_KEY);
			ImageButton btn=(ImageButton) sender;
			string strNumTab=btn.ID.Substring(btn.ID.IndexOf("_")+1);
			btn_Click(strNumTab,btn);
			IframeTabs.NavigateTo="RicDoc"+strNumTab+".aspx";

            if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
            {
                IframeTabs.NavigateTo += "?ricADL=" + Request.QueryString["ricADL"].ToString();
                initResultSearchWaiting("../waitingpage.htm");
            }
            else
            {
                initResultSearch("NewTabSearchResult.aspx?tabRes=estesa");
            }

			this.RefreshCallerContext(strNumTab);

			this.RefreshSearchPagingCallerContext();
            Session.Remove("templateRicerca");
		}

		private void btn_completa_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Session.Remove(SchedaRicerca.SESSION_KEY);
			ImageButton btn=(ImageButton) sender;
			string strNumTab=btn.ID.Substring(btn.ID.IndexOf("_")+1);
			btn_Click(strNumTab,btn);
			IframeTabs.NavigateTo="RicDoc"+strNumTab+".aspx";

            if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
            {
                IframeTabs.NavigateTo += "?ricADL=" + Request.QueryString["ricADL"].ToString();
                initResultSearchWaiting("../waitingpage.htm");
            }
            else
            {
                initResultSearch("NewTabSearchResult.aspx?tabRes=completa");
            }

			this.RefreshCallerContext(strNumTab);

			this.RefreshSearchPagingCallerContext();
            Session.Remove("templateRicerca");
		}

		/// <summary>
		/// Impostazione contesto chiamante
		/// </summary>
		/// <param name="tabName"></param>
		private void RefreshCallerContext(string tabName)
		{
			SiteNavigation.CallContext currentContext=SiteNavigation.CallContextStack.CurrentContext;
			if (currentContext != null)
				currentContext.QueryStringParameters["tab"]=tabName;
		}

		/// <summary>
		/// 
		/// </summary>
		private void RefreshSearchPagingCallerContext()
		{
			SiteNavigation.CallContext currentContext=SiteNavigation.CallContextStack.CurrentContext;
			if (currentContext != null)
				currentContext.PageNumber=1;
		}

		private void btn_completamento_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Session.Remove(SchedaRicerca.SESSION_KEY);
			ImageButton btn=(ImageButton) sender;
			string strNumTab=btn.ID.Substring(btn.ID.IndexOf("_")+1);
			btn_Click(strNumTab,btn);
			IframeTabs.NavigateTo="RicDoc"+strNumTab+".aspx";

            if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
            {
                IframeTabs.NavigateTo += "?ricADL=" + Request.QueryString["ricADL"].ToString();
                initResultSearchWaiting("../waitingpage.htm");
            }
            else
            {
                initResultSearch("NewTabSearchResult.aspx?tabRes=completamento");
            }

			this.RefreshCallerContext(strNumTab);

			this.RefreshSearchPagingCallerContext();
            Session.Remove("templateRicerca");
		}
       
        private void CaricaTab(string nomeTab)
        {
            string nomeButtTab = "btn_" + nomeTab;

            ImageButton ButtImg = (ImageButton)Page.FindControl(nomeButtTab);

            bool found = false;

            btn_Click(nomeTab, ButtImg);

            IframeTabs.NavigateTo = "RicDoc" + nomeTab + ".aspx";

            if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
            {
                IframeTabs.NavigateTo += "?ricADL=" + Request.QueryString["ricADL"].ToString();
                found = true;
            }

            if (Request.QueryString["gridper"] != string.Empty && Request.QueryString["gridper"] != null)
            {
                if (found)
                {
                    IframeTabs.NavigateTo += "&gridper=" + Request.QueryString["gridper"].ToString();
                }
                else
                {
                    IframeTabs.NavigateTo += "?gridper=" + Request.QueryString["gridper"].ToString();
                }
                found = true;

            }

            if (Request.QueryString["numRes"] != string.Empty && Request.QueryString["numRes"] != null)
            {
                if (found)
                {
                    IframeTabs.NavigateTo += "&numRes=" + Request.QueryString["numRes"].ToString();
                }
                else
                {
                    IframeTabs.NavigateTo += "?numRes=" + Request.QueryString["numRes"].ToString();
                }
            }

             SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if ((currentContext==null || !currentContext.IsBack) && !found)
            {
                if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                {
                    initResultSearchWaiting("../waitingpage.htm");
                }
                else
                {
                    initResultSearch("NewTabSearchResult.aspx?tabRes=estesa");
                }
            }

            this.RefreshCallerContext(nomeTab);
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
			this.btn_veloce.Click += new System.Web.UI.ImageClickEventHandler(this.btn_veloce_Click);
			this.btn_estesa.Click += new System.Web.UI.ImageClickEventHandler(this.btn_estesa_Click);
			this.btn_completa.Click += new System.Web.UI.ImageClickEventHandler(this.btn_completa_Click);
			this.btn_completamento.Click += new System.Web.UI.ImageClickEventHandler(this.btn_completamento_Click);
            this.btn_Grigia.Click += new System.Web.UI.ImageClickEventHandler(this.btn_docGrigio_Click);
            this.btn_StampaReg.Click += new System.Web.UI.ImageClickEventHandler(this.btn_StampeReg_Click);
			this.IframeTabs.Navigate += new System.EventHandler(this.IframeTabs_Navigate);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.tabGestioneRicDoc_PreRender);
            this.btn_StampaRep.Click += new System.Web.UI.ImageClickEventHandler(this.btn_StampeRep_Click);

		}
		#endregion

        private void btn_docGrigio_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Session.Remove(SchedaRicerca.SESSION_KEY);
            initResultSearch("tabRisultatiRicDocGrigia.aspx");
            ImageButton btn = (ImageButton)sender;
            string strNumTab = btn.ID.Substring(btn.ID.IndexOf("_") + 1);
            btn_Click(strNumTab, btn);

            IframeTabs.NavigateTo = "RicDoc" + strNumTab + ".aspx";


            if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
            {
                IframeTabs.NavigateTo += "?ricADL=" + Request.QueryString["ricADL"].ToString();
            }

			this.RefreshCallerContext(strNumTab);

            this.RefreshSearchPagingCallerContext();
        }

		private void IframeTabs_Navigate(object sender, System.EventArgs e)
		{
		
		}

        private void btn_StampeReg_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Session.Remove(SchedaRicerca.SESSION_KEY);
            initResultSearch("NewTabSearchResult.aspx?tabRes=StampaReg");
            ImageButton btn = (ImageButton)sender;
            string strNumTab = btn.ID.Substring(btn.ID.IndexOf("_") + 1);
            btn_Click(strNumTab, btn);
            Response.Write("<script>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?tabRes=StampaReg';</script>");
            IframeTabs.NavigateTo = "RicDoc" + strNumTab + ".aspx";

            this.RefreshCallerContext(strNumTab);

            this.RefreshSearchPagingCallerContext();
            Session.Remove("templateRicerca");
        }

		private void tabGestioneRicDoc_PreRender(object sender, System.EventArgs e)
		{
			this.PerformBackSearch();
		}

		/// <summary>
		/// Azione di ricerca da "back"
		/// </summary>
		private void PerformBackSearch()
		{
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext.IsBack)
			{
				string url=string.Empty;
                bool found = false;
                if (this.Request.QueryString["tab"].ToLower() == "grigia")
                    url = "tabRisultatiRicDocGrigia.aspx";
                else
                {
                    url = "NewTabSearchResult.aspx?tabRes=" + this.Request.QueryString["tab"].ToString();
                    found = true;
                }

				string docIndex=this.Request.QueryString["docIndex"];
                string ricAdl = this.Request.QueryString["ricADL"];

                if (!string.IsNullOrEmpty(docIndex))
                {
                    if (found)
                    {
                        url += "&docIndex=" + docIndex;
                    }
                    else
                    {
                        url += "?docIndex=" + docIndex;
                    }
                }

                if (!string.IsNullOrEmpty(ricAdl))
                {
                    if (url.IndexOf('?') != -1)
                        url += "&ricADL=" + ricAdl;
                    if(url.IndexOf('?') == -1)
                        url += "?ricADL=" + ricAdl;
                }

                Response.Write("<script>top.principale.iFrame_dx.document.location = '" + url + "';</script>");			    
			}
		}

        private void btn_StampeRep_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Session.Remove(SchedaRicerca.SESSION_KEY);
            initResultSearch("NewTabSearchResult.aspx?tabRes=StampaRep");
            ImageButton btn = (ImageButton)sender;
            string strNumTab = btn.ID.Substring(btn.ID.IndexOf("_") + 1);
            btn_Click(strNumTab, btn);
            Response.Write("<script>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?tabRes=StampaRep';</script>");
            IframeTabs.NavigateTo = "RicDoc" + strNumTab + ".aspx";

            this.RefreshCallerContext(strNumTab);

            this.RefreshSearchPagingCallerContext();
            Session.Remove("templateRicerca");
        }
	}
}
