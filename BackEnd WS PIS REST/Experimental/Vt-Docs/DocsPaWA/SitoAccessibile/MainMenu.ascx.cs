namespace DocsPAWA.SitoAccessibile
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///	Summary description for MainMenu.
	/// </summary>
	public class MainMenu : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlInputButton btnNuovoProtocollo;
		protected System.Web.UI.HtmlControls.HtmlInputButton btnNuovoProfilo;
		protected System.Web.UI.HtmlControls.HtmlInputButton btnRicercaDocumenti;
		protected System.Web.UI.HtmlControls.HtmlInputButton btnRicercaFascicoli;
		protected System.Web.UI.HtmlControls.HtmlInputButton btnRicercaTrasmissioni;
		protected System.Web.UI.HtmlControls.HtmlInputButton btnEsci;
		protected System.Web.UI.HtmlControls.HtmlInputButton btnHomepage;
		protected System.Web.UI.HtmlControls.HtmlInputButton btnDocumenti;
		protected System.Web.UI.HtmlControls.HtmlInputButton btnRicerche;

		private void Page_Load(object sender, System.EventArgs e)
		{
			this.RegisterEventServerClick();

			// Inizializzazione dei menu
			this.InitializeMenu();
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			// Impostazione stile menu correntemente selezionato
			this.SetCurrentMenuStyleSelected();
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
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new EventHandler(this.Page_PreRender);

		}
		#endregion

		#region Gestione eventi pulsanti menu

		private void RegisterEventServerClick()
		{
			this.btnHomepage.ServerClick += new EventHandler(this.btnHomepage_ServerClick);
			
			this.btnDocumenti.ServerClick += new EventHandler(this.btnDocumenti_ServerClick);
			
			this.btnRicerche.ServerClick += new EventHandler(this.btnRicerche_ServerClick);
			this.btnRicercaDocumenti.ServerClick += new EventHandler(this.btnRicercaDocumenti_ServerClick);	
			this.btnRicercaFascicoli.ServerClick += new EventHandler(this.btnRicercaFascicoli_ServerClick);	
			this.btnRicercaTrasmissioni.ServerClick += new EventHandler(this.btnRicercaTrasmissioni_ServerClick);	

			this.btnEsci.ServerClick+=new EventHandler(this.btnEsci_ServerClick);
		}

		private void btnHomepage_ServerClick(Object sender,EventArgs e)
		{
			this.ReleaseResources();

			this.CurrentMainMenu=MainMenuItemsEnum.HomePage;

			Response.Redirect(EnvironmentContext.RootPath + "Home.aspx");
		}

		private void btnDocumenti_ServerClick(Object sender,EventArgs e)
		{
			this.CurrentMainMenu=MainMenuItemsEnum.Documenti;

			this.SetSubmenuDocumentiVisibility(false);
		}

		private void btnRicerche_ServerClick(Object sender,EventArgs e)
		{
			this.CurrentMainMenu=MainMenuItemsEnum.Ricerca;

			this.SetSubmenuRicercheVisibility(false);
		}

		private void btnRicercaDocumenti_ServerClick(Object sender,EventArgs e)
		{
			if (!SUBMENU_RICERCA_DOCUMENTI.Equals(this.CurrentSubmenuKey))
				this.ReleaseResources();

			this.CurrentSubmenuKey=SUBMENU_RICERCA_DOCUMENTI;

			Response.Redirect(EnvironmentContext.RootPath + "Ricerca/Documenti.aspx?new=true");
		}

		private void btnRicercaFascicoli_ServerClick(Object sender,EventArgs e)
		{
			if (!SUBMENU_RICERCA_FASCICOLI.Equals(this.CurrentSubmenuKey))
				this.ReleaseResources();

			this.CurrentSubmenuKey=SUBMENU_RICERCA_FASCICOLI;

			Response.Redirect(EnvironmentContext.RootPath + "Ricerca/Fascicoli.aspx");
		}

		private void btnRicercaTrasmissioni_ServerClick(Object sender,EventArgs e)
		{
			if (!SUBMENU_RICERCA_TRASMISSIONI.Equals(this.CurrentSubmenuKey))
				this.ReleaseResources();

			this.CurrentSubmenuKey=SUBMENU_RICERCA_TRASMISSIONI;

			Response.Redirect(EnvironmentContext.RootPath + "Ricerca/Trasmissioni.aspx");
		}

		private void btnEsci_ServerClick(Object sender,EventArgs e)
		{
			this.ReleaseResources();

			this.CurrentMainMenu=MainMenuItemsEnum.HomePage;
			this.CurrentSubmenuKey=string.Empty;

			Response.Redirect(EnvironmentContext.RootPath + "Logout.aspx");
		}

		#endregion

		#region Gestione menu principali, secondari e stili

		// Constanti che identificano i menu principali
		private const string MENU_HOME_PAGE="containerMenuHomePage";
		private const string MENU_DOCUMENTI="containerMenuDocumenti";
		private const string MENU_RICERCHE="containerMenuRicerche";
		private const string MENU_ESCI="containerMenuEsci";
		
		// Constanti che identificano i menu documenti
		private const string SUBMENU_DOCUMENTI="submenuDocumenti";
		private const string MENU_NUOVO_PROTOCOLLO="menuNuovoProtocollo";
		private const string MENU_NUOVO_PROFILO="menuNuovoProfilo";

		// Constanti che identificano i menu richerche
		private const string SUBMENU_RICERCHE="submenuRicerche";
		private const string MENU_RICERCA_DOCUMENTI="menuRicercaDocumenti";
		private const string MENU_RICERCA_FASCICOLI="menuRicercaFascicoli";
		private const string MENU_RICERCA_TRASMISSIONI="menuRicercaTrasmissioni";
		
		/// <summary>
		/// Inizializzazione dei menu
		/// </summary>
		private void InitializeMenu()
		{
			// Impostazione visibilità menu principali
			this.SetMainMenuVisibility();

			// Inizializzazione dei menu secondari come non visibili
			this.SetSubmenuDocumentiVisibility(true);
			this.SetSubmenuRicercheVisibility(true);
		}

		/// <summary>
		/// Menu principale correntemente selezionato
		/// </summary>
		private MainMenuItemsEnum CurrentMainMenu
		{
			get
			{
				if (Session["SitoAccessibile.CurrentMainMenu"]==null)
					return MainMenuItemsEnum.HomePage;
				else
					return (MainMenuItemsEnum) Session["SitoAccessibile.CurrentMainMenu"];
			}
			set
			{
				Session["SitoAccessibile.CurrentMainMenu"]=value;
			}
		}

		/// <summary>
		/// Chiave del menu secondario correntemente selezionato
		/// </summary>
		private string CurrentSubmenuKey
		{
			get
			{
				if (Session["SitoAccessibile.CurrentSubmenuKey"]!=null)
					return Session["SitoAccessibile.CurrentSubmenuKey"].ToString();
				else
					return string.Empty;
			}
			set
			{
				Session["SitoAccessibile.CurrentSubmenuKey"]=value;
			}
		}

		/// <summary>
		/// Impostazione visibilità voci di menu principali
		/// relativamente ai diritti dell'utente corrente
		/// </summary>
		private void SetMainMenuVisibility()
		{
//			bool isVisible=this.HasFunction("MENU_DOCUMENTI");
//			this.GetContainerMenu(MENU_DOCUMENTI).Visible=isVisible;

			bool isVisible=false;
			this.GetContainerMenu(MENU_DOCUMENTI).Visible=isVisible;

			isVisible=this.HasFunction("MENU_RICERCA");
			this.GetContainerMenu(MENU_RICERCHE).Visible=isVisible;
		}
		
		/// <summary>
		/// Impostazione visibilità voci di menu secondarie
		/// menu documenti
		/// </summary>
		/// <param name="hideAll"></param>
		private void SetSubmenuDocumentiVisibility(bool hideAll)
		{	
			bool isVisible=(!hideAll && this.HasFunction("DO_PROTOCOLLA"));
			this.GetContainerMenu(MENU_NUOVO_PROTOCOLLO).Visible=isVisible;

			isVisible=(!hideAll && this.HasFunction("DO_PROFILO"));
			this.GetContainerMenu(MENU_NUOVO_PROFILO).Visible=isVisible;

			// Visibiltà contenitore dei menu documenti
			this.GetContainerMenu(SUBMENU_DOCUMENTI).Visible=isVisible;
		}

		/// <summary>
		/// Impostazione visibilità voci di menu secondarie
		/// menu ricerche
		/// </summary>
		/// <param name="hideAll"></param>
		private void SetSubmenuRicercheVisibility(bool hideAll)
		{
			bool isVisible=(!hideAll && this.HasFunction("DO_CERCA"));
			this.GetContainerMenu(MENU_RICERCA_DOCUMENTI).Visible=isVisible;

			isVisible=(!hideAll && this.HasFunction("FASC_GESTIONE"));
			this.GetContainerMenu(MENU_RICERCA_FASCICOLI).Visible=isVisible;

			isVisible=(!hideAll && this.HasFunction("TRAS_CERCA"));
			this.GetContainerMenu(MENU_RICERCA_TRASMISSIONI).Visible=isVisible;

			// Visibiltà contenitore dei menu ricerche
			this.GetContainerMenu(SUBMENU_RICERCHE).Visible=isVisible;
		}

		/// <summary>
		/// Impostazione dello stile al menu principale correntemente selezionato
		/// </summary>
		private void SetCurrentMenuStyleSelected()
		{
			// Deselezione di tutti i menu
			this.SetAllMenuStyleUnselected();

			// Reperimento menu corrente e impostazione dello stile selezionato
			HtmlGenericControl containerMenu=this.GetCurrentMenuContainer();			
			if (containerMenu!=null)
				containerMenu.Attributes.Add("class","selected");
		}

		/// <summary>
		/// Reperimento menu container correntemente selezionato
		/// </summary>
		/// <returns></returns>
		private HtmlGenericControl GetCurrentMenuContainer()
		{
			HtmlGenericControl containerMenu=null;
			
			if (this.CurrentMainMenu.Equals(MainMenuItemsEnum.HomePage))
				containerMenu=this.GetContainerMenu(MENU_HOME_PAGE);
			else if (this.CurrentMainMenu.Equals(MainMenuItemsEnum.Documenti))
				containerMenu=this.GetContainerMenu(MENU_DOCUMENTI);
			else if (this.CurrentMainMenu.Equals(MainMenuItemsEnum.Ricerca))
				containerMenu=this.GetContainerMenu(MENU_RICERCHE);
			else if (this.CurrentMainMenu.Equals(MainMenuItemsEnum.Esci))
				containerMenu=this.GetContainerMenu(MENU_ESCI);

			return containerMenu;
		}

		/// <summary>
		/// Reperimento controllo menu
		/// </summary>
		/// <param name="containerID"></param>
		/// <returns></returns>
		private HtmlGenericControl GetContainerMenu(string containerID)
		{
			return this.FindControl(containerID) as HtmlGenericControl;
		}

		/// <summary>
		/// Impostazione stile di tutti i menu come non selezionato
		/// </summary>
		private void SetAllMenuStyleUnselected()
		{
			this.SetMenuStyleUnselected(this.GetContainerMenu(MENU_HOME_PAGE));
			this.SetMenuStyleUnselected(this.GetContainerMenu(MENU_DOCUMENTI));
			this.SetMenuStyleUnselected(this.GetContainerMenu(MENU_RICERCHE));
			this.SetMenuStyleUnselected(this.GetContainerMenu(MENU_ESCI));
		}

		/// <summary>
		/// Impostazione stile di un menu come non selezionato
		/// </summary>
		/// <param name="containerMenu"></param>
		private void SetMenuStyleUnselected(HtmlGenericControl containerMenu)
		{
			if (containerMenu!=null && containerMenu.Attributes["class"]!=null)
				containerMenu.Attributes.Remove("class");
		}

		#endregion


		/// <summary>
		/// Verifica se il ruolo corrente è autorizzato 
		/// ad utilizzare una funzione
		/// </summary>
		/// <param name="codFunzione"></param>
		/// <returns></returns>
		public bool HasFunction(string codFunzione)
		{
			bool found = false;

			DocsPaWR.Ruolo ruolo=UserManager.getRuolo();

			if (ruolo!=null)
			{
				DocsPaWR.Funzione[] funzioni = ruolo.funzioni;
				if (funzioni!=null)
				{
					for (int i=0; !found && i<funzioni.Length; i++)
					{
						if (funzioni[i].codice == codFunzione)
							found = true;
					}
				}
			}

			return found;
		}

		/// <summary>
		/// Rilascio delle risorse relative alla pagina correntemente visualizzata
		/// </summary>
		private void ReleaseResources()
		{
			ResourceReleaser.ReleaseAll();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="menuItem"></param>
		/// <returns></returns>
		private MainMenuItemsEnum GetMainMenu(string menuItem)
		{
			return (MainMenuItemsEnum) Enum.Parse(typeof(MainMenuItemsEnum),menuItem,true);
		}

		/// <summary>
		/// Enumeration che identifica i menu principali disponibili
		/// </summary>
		public enum MainMenuItemsEnum
		{
			HomePage,
			Documenti,
			Ricerca,
			Esci
		}

		/// <summary>
		/// Constanti che identificano le chiavi associate ai menu secondari
		/// </summary>
		private const string SUBMENU_RICERCA_DOCUMENTI="RicercaDocumenti";
		private const string SUBMENU_RICERCA_FASCICOLI="RicercaFascicoli";
		private const string SUBMENU_RICERCA_TRASMISSIONI="RicercaTrasmissioni";
	}
}