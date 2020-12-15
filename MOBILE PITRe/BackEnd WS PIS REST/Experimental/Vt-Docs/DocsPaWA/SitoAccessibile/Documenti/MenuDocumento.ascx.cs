namespace DocsPAWA.SitoAccessibile.Documenti
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using DocsPAWA.DocsPaWR;

	/// <summary>
	///		Summary description for MenuDocumento.
	/// </summary>
	public class MenuDocumento : System.Web.UI.UserControl
	{
		private ItemsMenuDocumentoEnum _currentMenu=ItemsMenuDocumentoEnum.Documento;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerDocumento;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerClassifica;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerAllegati;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerVersioni;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerTrasmissioni;
		private SchedaDocumento _schedaDocumento=null;

		/// <summary>
		/// Costanti che identificano i menu del documento
		/// </summary>
		private const string MENU_DOCUMENTO="containerDocumento";
		private const string MENU_CLASSIFICA="containerClassifica";
		private const string MENU_ALLEGATI="containerAllegati";
		private const string MENU_VERSIONI="containerVersioni";
		private const string MENU_TRASMISSIONI="containerTrasmissioni";

		private void Page_Load(object sender, System.EventArgs e)
		{
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// Inizializzazione usercontrol del menu del documento
		/// </summary>
		/// <param name="idProfile"></param>
		/// <param name="docNumber"></param>
		public void Initialize(ItemsMenuDocumentoEnum currentMenu,SchedaDocumento schedaDocumento)
		{
			this._currentMenu=currentMenu;
			this._schedaDocumento=schedaDocumento;

			// Creazione elementi di menu
			this.CreateMenuItems();
		}

		/// <summary>
		/// 
		/// </summary>
		public ItemsMenuDocumentoEnum CurrentMenu
		{
			get
			{
				return this._currentMenu;
			}			
		}

		public SchedaDocumento SchedaDocumento
		{
			get
			{
				return this._schedaDocumento;
			}
		}

		/// <summary>
		/// Creazione elemento di menu
		/// </summary>
		/// <param name="menuItem"></param>
		/// <returns></returns>
		private HtmlAnchor CreateMenuItem(ItemsMenuDocumentoEnum menuItem)
		{
			string menuText=menuItem.ToString();

			HtmlAnchor anchor=new HtmlAnchor();

			HtmlGenericControl span=new HtmlGenericControl("span");
			span.InnerText=this.GetMenuText(menuItem);
			anchor.Controls.Add(span);

			anchor.HRef=this.GetHRefMenu(menuItem);

			if (menuItem==this.CurrentMenu)
			{
				anchor.HRef+="#";

				// Impostazione stile selezionato al menu corrente
				this.SetCurrentMenuStyleSelected();
			}

			return anchor;
		}

		/// <summary>
		/// Reperimento menu
		/// </summary>
		/// <param name="containerID"></param>
		/// <returns></returns>
		private HtmlGenericControl GetContainerMenu(string containerID)
		{
			return this.FindControl(containerID) as HtmlGenericControl;
		}

		/// <summary>
		/// Creazione elementi di menu
		/// </summary>
		private void CreateMenuItems()
		{
			this.containerDocumento.Controls.Add(this.CreateMenuItem(ItemsMenuDocumentoEnum.Documento));

			this.containerClassifica.Controls.Add(this.CreateMenuItem(ItemsMenuDocumentoEnum.Classifica));

			this.containerAllegati.Controls.Add(this.CreateMenuItem(ItemsMenuDocumentoEnum.Allegati));
			
			this.containerVersioni.Controls.Add(this.CreateMenuItem(ItemsMenuDocumentoEnum.Versioni));

			this.containerTrasmissioni.Controls.Add(this.CreateMenuItem(ItemsMenuDocumentoEnum.Trasmissioni));
		}

		/// <summary>
		/// Reperimento testo del menu
		/// </summary>
		/// <param name="menuItem"></param>
		/// <returns></returns>
		private string GetMenuText(ItemsMenuDocumentoEnum menuItem)
		{
			string retValue=string.Empty;

			switch (menuItem)
			{
				case ItemsMenuDocumentoEnum.Documento:
					retValue=this.GetMenuTextDocumento();
					break;

				case ItemsMenuDocumentoEnum.Classifica:
					retValue=this.GetMenuTextClassifica();
					break;

				case ItemsMenuDocumentoEnum.Allegati:
					retValue=this.GetMenuTextAllegati();
					break;

				case ItemsMenuDocumentoEnum.Versioni:
					retValue=this.GetMenuTextVersioni();
					break;

				case ItemsMenuDocumentoEnum.Trasmissioni:
					retValue=this.GetMenuTextTrasmissioni();
					break;
			}

			return retValue;
		}

		/// <summary>
		/// Reperimento testo del menu documento
		/// </summary>
		/// <returns></returns>
		private string GetMenuTextDocumento()
		{
			return ItemsMenuDocumentoEnum.Documento.ToString();
		}

		/// <summary>
		/// Reperimento testo del menu classifica
		/// </summary>
		/// <returns></returns>
		private string GetMenuTextClassifica()
		{
			int countFascicolazioni=0;
			
			try
			{
				Classificazioni.ClassificaHandler handler=new Classificazioni.ClassificaHandler();
				countFascicolazioni=handler.GetCountClassificazioniDocumento(this.SchedaDocumento.systemId);
			}
			catch
			{
			}

			string retValue=ItemsMenuDocumentoEnum.Classifica.ToString();

			if (countFascicolazioni>0)
				retValue+=" (" + countFascicolazioni.ToString() + ")";

			return retValue;
		}

		/// <summary>
		/// Reperimento testo del menu allegati
		/// </summary>
		/// <returns></returns>
		private string GetMenuTextAllegati()
		{
			string retValue=ItemsMenuDocumentoEnum.Allegati.ToString();

			int countAllegati=this.SchedaDocumento.allegati.Length;
			
			if (countAllegati>0)
				retValue=ItemsMenuDocumentoEnum.Allegati.ToString() + " (" + countAllegati.ToString() + ")";
			
			return retValue;
		}

		/// <summary>
		/// Reperimento testo del menu versioni
		/// </summary>
		/// <returns></returns>
		private string GetMenuTextVersioni()
		{
			string retValue=ItemsMenuDocumentoEnum.Versioni.ToString();
			
			int countVersioni=this.SchedaDocumento.documenti.Length;

			if (countVersioni>1)
				retValue += " (" + countVersioni.ToString() + ")";
			
			return retValue;
		}

		/// <summary>
		/// Reperimento testo del menu trasmissioni
		/// </summary>
		/// <returns></returns>
		private string GetMenuTextTrasmissioni()
		{
			string retValue=ItemsMenuDocumentoEnum.Trasmissioni.ToString();

			Trasmissioni.TrasmissioniHandler trasmissioniHandler=new DocsPAWA.SitoAccessibile.Documenti.Trasmissioni.TrasmissioniHandler();
			int countTrasmissioni=trasmissioniHandler.GetCountTrasmissioni(this.SchedaDocumento);

			if (countTrasmissioni>0)
				retValue += " (" + countTrasmissioni.ToString() + ")";

			return retValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private string GetHRefMenu(ItemsMenuDocumentoEnum menuItem)
		{
			string retValue=string.Empty;

			switch (menuItem)
			{
				case ItemsMenuDocumentoEnum.Documento:
					retValue="DettagliDocumento.aspx";
					break;
				case ItemsMenuDocumentoEnum.Classifica:
					retValue="Classificazioni/Classifica.aspx";
					break;
				case ItemsMenuDocumentoEnum.Allegati:
					retValue="Allegati/DettaglioAllegati.aspx";
					break;
				case ItemsMenuDocumentoEnum.Versioni:
					retValue="Versioni/DettaglioVersioni.aspx";
					break;
				case ItemsMenuDocumentoEnum.Trasmissioni:
					retValue="Trasmissioni/DettaglioTrasmissioni.aspx";
					break;
			}

			string queryString=string.Empty;

			if (Request.Url.Query!=string.Empty)
			{
				//queryString=Request.Url.Query;
				// Modificata da Donato il 23 ottobre 2006
				queryString=Request.Url.Query.Replace("&", "&amp;");
			}
			else
			{
				queryString="?iddoc=" + this.SchedaDocumento.systemId + "&amp;docnum=" + this.SchedaDocumento.docNumber;
			}

			return retValue + queryString;
		}

		/// <summary>
		/// Impostazione dello stile al menu principale correntemente selezionato
		/// </summary>
		private void SetCurrentMenuStyleSelected()
		{
			// Deselezione di tutti i menu
			this.SetAllMenuStyleUnselected();

			// Reperimento menu corrente e impostazione dello stile selezionato
			HtmlGenericControl containerMenu=this.GetCurrentContainerMenu();
			if (containerMenu!=null)
				containerMenu.Attributes.Add("class","current");
		}

		/// <summary>
		/// Reperimento menu correntemente selezionato
		/// </summary>
		/// <returns></returns>
		private HtmlGenericControl GetCurrentContainerMenu()
		{
			string containerID=string.Empty;

			switch (this.CurrentMenu)
			{
				case ItemsMenuDocumentoEnum.Documento:
					containerID=MENU_DOCUMENTO;
					break;
				case ItemsMenuDocumentoEnum.Classifica:
					containerID=MENU_CLASSIFICA;
					break;
				case ItemsMenuDocumentoEnum.Allegati:
					containerID=MENU_ALLEGATI;
					break;
				case ItemsMenuDocumentoEnum.Versioni:
					containerID=MENU_VERSIONI;
					break;
				case ItemsMenuDocumentoEnum.Trasmissioni:
					containerID=MENU_TRASMISSIONI;
					break;
			}

			return this.GetContainerMenu(containerID) as HtmlGenericControl;
		}

		/// <summary>
		/// Impostazione stile di tutti i menu come non selezionato
		/// </summary>
		private void SetAllMenuStyleUnselected()
		{
			this.SetMenuStyleUnselected(this.GetContainerMenu(MENU_DOCUMENTO));
			this.SetMenuStyleUnselected(this.GetContainerMenu(MENU_CLASSIFICA));
			this.SetMenuStyleUnselected(this.GetContainerMenu(MENU_ALLEGATI));
			this.SetMenuStyleUnselected(this.GetContainerMenu(MENU_VERSIONI));
			this.SetMenuStyleUnselected(this.GetContainerMenu(MENU_TRASMISSIONI));
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

		/// <summary>
		/// Enumeration che indica gli elementi di menu disponibili per il documento
		/// </summary>
		public enum ItemsMenuDocumentoEnum
		{
			Documento,
			Classifica,
			Allegati,
			Versioni,
			Trasmissioni
		}
	}
}