namespace DocsPAWA.SitoAccessibile.Paging
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;

	/// <summary>
	///		Summary description for ListPagingNavigationControls.
	/// </summary>
	public class ListPagingNavigationControls : System.Web.UI.UserControl
	{
		public class PageChangedEventArgs : EventArgs
		{
			PagingContext _pagingContext=null;

			public PageChangedEventArgs(PagingContext pagingContext)
			{
				this._pagingContext=pagingContext;
			}

			public PagingContext PagingContext
			{
				get
				{
					return this._pagingContext;
				}
			}
		}

		public delegate void PageChangedDelegate(object sender,PageChangedEventArgs e);

		public event PageChangedDelegate OnPageChanged=null;

		protected System.Web.UI.WebControls.Button btnMovePrevious;
		protected System.Web.UI.WebControls.Button btnMoveNext;
		protected System.Web.UI.WebControls.Button btnPreviousGroup;
		protected System.Web.UI.WebControls.Button btn_1;
		protected System.Web.UI.WebControls.Button btn_2;
		protected System.Web.UI.WebControls.Button btn_3;
		protected System.Web.UI.WebControls.Button btn_4;
		protected System.Web.UI.WebControls.Button btn_5;
		protected System.Web.UI.WebControls.Button btn_6;
		protected System.Web.UI.WebControls.Button btn_7;
		protected System.Web.UI.WebControls.Button btn_8;
		protected System.Web.UI.WebControls.Button btn_9;
		protected System.Web.UI.WebControls.Button btn_10;
		protected System.Web.UI.WebControls.Button btnNextGroup;
		protected System.Web.UI.WebControls.Label lblPagingDescription;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack && this.Visible)
				// Aggiornamento dei controlli di paginazione
				this.RefreshControls();
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
			this.btnMovePrevious.Click += new System.EventHandler(this.btnMovePrevious_Click);
			this.btnMoveNext.Click += new System.EventHandler(this.btnMoveNext_Click);
			this.btnPreviousGroup.Click += new System.EventHandler(this.OnPreviousGroupButtonClick);
			this.btn_1.Click += new System.EventHandler(this.OnPageButtonClick);
			this.btn_2.Click += new System.EventHandler(this.OnPageButtonClick);
			this.btn_3.Click += new System.EventHandler(this.OnPageButtonClick);
			this.btn_4.Click += new System.EventHandler(this.OnPageButtonClick);
			this.btn_5.Click += new System.EventHandler(this.OnPageButtonClick);
			this.btn_6.Click += new System.EventHandler(this.OnPageButtonClick);
			this.btn_7.Click += new System.EventHandler(this.OnPageButtonClick);
			this.btn_8.Click += new System.EventHandler(this.OnPageButtonClick);
			this.btn_9.Click += new System.EventHandler(this.OnPageButtonClick);
			this.btn_10.Click += new System.EventHandler(this.OnPageButtonClick);
			this.btnNextGroup.Click += new System.EventHandler(this.OnNextGroupButtonClick);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region Gestione paginazione

		/// <summary>
		/// Constante che identifica il range di pagine visualizzate
		/// </summary>
		private const int PAGE_RANGE_NUMBER=10;

		/// <summary>
		/// Modalità di paginazione corrente
		/// </summary>
		public PagingModeEnum PagingMode
		{
			get
			{
				if (this.ViewState["PagingMode"]!=null)
					return (PagingModeEnum) this.ViewState["PagingMode"];
				else
					return PagingModeEnum.Advanced;
			}
			set
			{
				this.ViewState["PagingMode"]=value;
			}
		}

		#region Gestione azioni e handler di eventi dei pulsanti di paginazione

		/// <summary>
		/// Handler evento pulsante pagina precedente
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnMovePrevious_Click(object sender, System.EventArgs e)
		{
			this.PerformPageActionButtonClick(this.GetPagingContext().PageNumber - 1);
		}

		/// <summary>
		/// Handler evento pulsante pagina successiva
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnMoveNext_Click(object sender, System.EventArgs e)
		{
			this.PerformPageActionButtonClick(this.GetPagingContext().PageNumber + 1);
		}

		/// <summary>
		/// Handler evento pulsante di paginazione
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPageButtonClick(object sender,EventArgs e)
		{
			string buttonText=((Button) sender).Text;
				
			// Reperimetno numero pagina
			int pageNumber=Convert.ToInt32(buttonText);

			this.PerformPageActionButtonClick(pageNumber);
		}

		/// <summary>
		/// Handler evento pulsante di paginazione gruppo precedente
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPreviousGroupButtonClick(object sender,EventArgs e)
		{
			if (this.btn_1.Visible && this.btn_1.Text!=string.Empty)
			{
				// Reperimento numero prima pagina visualizzabile
				int pageNumber=Convert.ToInt32(this.btn_1.Text);

				this.PerformPageActionButtonClick(pageNumber - 1);
			}
		}

		/// <summary>
		/// Handler evento pulsante di paginazione gruppo successivo
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnNextGroupButtonClick(object sender,EventArgs e)
		{
			if (this.btn_10.Visible && this.btn_10.Text!=string.Empty)
			{
				// Reperimento numero ultima pagina visualizzabile
				int pageNumber=Convert.ToInt32(this.btn_10.Text);

				this.PerformPageActionButtonClick(pageNumber + 1);
			}
		}

		/// <summary>
		/// Azione di spostamento pagina
		/// </summary>
		/// <param name="pageNumber"></param>
		private void PerformPageActionButtonClick(int pageNumber)
		{
			PagingContext pagingContext=this.GetPagingContext();
			pagingContext.PageNumber=pageNumber;
			
			if (this.OnPageChanged!=null)
			{
				PageChangedEventArgs args=new PageChangedEventArgs(pagingContext);
				
				// Invio evento di paginazione
				this.OnPageChanged(this,args);

				// aggiornato paging context dagli eventargs
				pagingContext=args.PagingContext;
			}

			// Impostazione oggetto paginazione
			this.SetPagingContext(pagingContext);

			// Aggiornamento dei controlli di paginazione
			this.RefreshControls(pagingContext);
		}

		#endregion

		#region Gestione visibilità controlli di paginazione

		/// <summary>
		/// Aggiornamento dei controlli di paginazione
		/// </summary>
		/// <param name="pagingContext"></param>
		private void RefreshControls(PagingContext pagingContext)
		{
			// Aggiornamento visibilità pulsanti di navigazione semplice, sempre presenti
			this.RefreshVisibililitySimpleButtons(pagingContext);

			if (this.PagingMode.Equals(PagingModeEnum.Simple))
			{
				// In modalità semplice, i controlli di paginazione avanzati vengono nascosti
				this.HideControlsAdvancedMode();

				this.ShowPagingDescription(true);

				// Aggiornamento descrizione di pagina corrente / pagine totali
				this.RefreshPagingDescription(pagingContext);
			}
			else
			{
				this.ShowPagingDescription(false);

				this.RefreshControlsAdvancedMode(pagingContext);
			}
		}

		/// <summary>
		/// Aggiornamento dei controlli di paginazione
		/// </summary>
		private void RefreshControls()
		{
			PagingContext pagingContext=this.GetPagingContext();

			this.RefreshControls(pagingContext);
		}

		/// <summary>
		/// Vengono nascosti i controlli di paginazione di modalità avanzata
		/// </summary>
		private void HideControlsAdvancedMode()
		{
			this.SetVisibilityPagingButton(this.btnPreviousGroup,false);
			this.SetVisibilityPagingButton(this.btnNextGroup,false);

			for (int i=0;i<10;i++)
			{
				Button btnPageButton=(Button) this.FindControl("btn_" + (i + 1).ToString());
				if (btnPageButton!=null)
					this.SetVisibilityPagingButton(btnPageButton,false);
			}
		}

		/// <summary>
		/// Aggiornamento visibilità di un pulsante di navigazione
		/// </summary>
		/// <param name="pagingButton"></param>
		/// <param name="isVisible"></param>
		private void SetVisibilityPagingButton(Button pagingButton,bool isVisible)
		{
			// Impostazione visibilità controllo parent con tag "li"
			HtmlGenericControl parent=pagingButton.Parent as HtmlGenericControl;
			if (parent!=null && parent.TagName.Equals("li"))
				parent.Visible=isVisible;

			pagingButton.Visible=isVisible;
		}

		/// <summary>
		/// Aggiornamento visibilità pulsanti di navigazione semplice
		/// </summary>
		/// <param name="pagingContext"></param>
		private void RefreshVisibililitySimpleButtons(PagingContext pagingContext)
		{
			this.SetVisibilityPagingButton(this.btnMovePrevious,(pagingContext.PageNumber>1));
			this.SetVisibilityPagingButton(this.btnMoveNext,(pagingContext.PageNumber<pagingContext.PageCount));
		}

		/// <summary>
		/// Impostazione visibilità descrizione di paginazione
		/// </summary>
		/// <param name="isVisible"></param>
		private void ShowPagingDescription(bool isVisible)
		{
			//this.lblPagingDescription.Visible=isVisible;
			if (isVisible)
			{
				this.lblPagingDescription.CssClass = "hiddenDescription"; 
			} 
		}

		/// <summary>
		/// Aggiornamento descrizione pagina corrente / pagina totale
		/// </summary>
		/// <param name="pagingContext"></param>
		private void RefreshPagingDescription(PagingContext pagingContext)
		{
			string pagingDescription="pagina " + pagingContext.PageNumber.ToString() + " di " + pagingContext.PageCount.ToString();

			this.lblPagingDescription.Text=pagingDescription;
		}

		/// <summary>
		/// Aggiornamento controlli di paginazione in modalità avanzata
		/// </summary>
		/// <param name="pagingContext"></param>
		private void RefreshControlsAdvancedMode(PagingContext pagingContext)
		{
			int restOf=(pagingContext.PageNumber % PAGE_RANGE_NUMBER);

			int startPage=0;

			if (restOf.Equals(0))
			{
				restOf=pagingContext.PageNumber;
				if (pagingContext.PageNumber != 10)
					restOf -= (restOf - PAGE_RANGE_NUMBER);
			}
			
			startPage=(pagingContext.PageNumber - restOf) + 1;

			// Visualizzazione pulsante gruppo precedente
			this.SetVisibilityPagingButton(this.btnPreviousGroup,(startPage > PAGE_RANGE_NUMBER));
			if (this.btnPreviousGroup.Visible)
				this.btnPreviousGroup.Attributes.Add("title","Vai a pagina " + (startPage - 1).ToString() + " e precedenti");

			int lastPage=startPage;

			for (int i=0;i<10;i++)
			{
				// Pagina corrente
				int currentPage=(startPage + i);

				Button btnPageButton=(Button) this.FindControl("btn_" + (i + 1).ToString());

				// Visualizzazione pulsante corrente
				this.SetVisibilityPagingButton(btnPageButton,(currentPage<=pagingContext.PageCount));
				
				if (btnPageButton.Visible)
				{
					// Impostazione numero pagina, solamente se visibile
					btnPageButton.Text=currentPage.ToString();				

					// Impostazione stili pulsante
					if (!currentPage.Equals(pagingContext.PageNumber))
					{
						btnPageButton.Attributes.Add("title","Vai a pagina " + currentPage.ToString());
						btnPageButton.Attributes.Remove("class");
					}
					else
					{
						//btnPageButton.Attributes.Remove("title");
						btnPageButton.Attributes.Add("title", "Pagina corrente");
						btnPageButton.Attributes.Add("class","currentPage");
					}
				}
				else
				{
					btnPageButton.Text=string.Empty;
				}

				lastPage=currentPage;
			}

			// Visualizzazione pulsante gruppo successivo
			this.SetVisibilityPagingButton(this.btnNextGroup,(lastPage<pagingContext.PageCount));
			if (this.btnNextGroup.Visible)
				this.btnNextGroup.Attributes.Add("title","Vai a pagina " + (lastPage + 1).ToString() + " e successive");
		}

		#endregion

		#region Gestione oggetto "PagingContext" corrente

		/// <summary>
		/// Aggiornamento paginazione
		/// </summary>
		/// <param name="parentPageLink"></param>
		/// <param name="additionalAttributes"></param>
		/// <param name="pagingContext"></param>
		public void RefreshPaging(PagingContext pagingContext)
		{
			this.SetPagingContext(pagingContext);

			this.RefreshControls();
		}

		/// <summary>
		/// Reperimento contesto di paginazione corrente
		/// </summary>
		/// <returns></returns>
		public PagingContext GetPagingContext()
		{
			PagingContext pagingContext=this.ViewState["PagingContext"] as PagingContext;

			if (pagingContext==null)
			{
				pagingContext=new PagingContext(1);
				pagingContext.PageCount=1;
			}

			return pagingContext;
		}

		/// <summary>
		/// Impostazione contesto di paginazione corrente
		/// </summary>
		/// <param name="pagingContext"></param>
		private void SetPagingContext(PagingContext pagingContext)
		{
			this.ViewState["PagingContext"]=pagingContext;
		}

		#endregion
		
		#endregion
	}

	/// <summary>
	/// Enumeration che indica le possibili modalità di paginazione
	/// Simple: 
	///		Vengono mostrati solamente i pulsanti precedente e successivo
	/// Advanced: 
	///		Vengono mostrati una serie di pulsanti per navigare
	///		direttamente all'interno di una pagina
	/// </summary>
	public enum PagingModeEnum
	{
		Simple,
		Advanced
	}
}