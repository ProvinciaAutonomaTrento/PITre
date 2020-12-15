namespace SAAdminTool.SiteNavigation
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for NavigationContext.
	/// </summary>
	public class NavigationContext : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label lblCallContextList;
		protected System.Web.UI.WebControls.LinkButton btnLink_1;
		protected System.Web.UI.WebControls.LinkButton btnLink_2;
		protected System.Web.UI.WebControls.LinkButton btnLink_3;
		protected System.Web.UI.WebControls.LinkButton btnLink_4;
		protected DocsPaWebCtrlLibrary.ImageButton btnBack;
		protected System.Web.UI.WebControls.LinkButton btnLink_5;

		/// <summary>
		/// 
		/// </summary>
		private const string SESSION_PREFIX="NavigationContext_";

		private void Page_Load(object sender, System.EventArgs e)
		{
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			this.SetFieldsVisibility();

			this.RefreshNavigationContext();
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
			this.btnBack.Click += new System.Web.UI.ImageClickEventHandler(this.btnBack_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

		#region Public methods

		/// <summary>
		/// Numero massimo di elementi di contesto visualizzati
		/// </summary>
		public int MaxItemsViewed
		{
			get
			{
				try
				{
					return Convert.ToInt32(this.ViewState["MaxItemsViewed"]);
				}
				catch
				{
					return 0;
				}
			}
			set
			{
				this.ViewState["MaxItemsViewed"]=value;
			}
		}

		/// <summary>
		/// Frame contenente il controllo di navigazione
		/// </summary>
		public string OwnerFrame
		{
			get
			{
				return InternalOwnerFrame;
			}
			set
			{
				InternalOwnerFrame=value;
			}
		}

		/// <summary>
		/// Aggiornamento controlli navigazione
		/// </summary>
		public static void RefreshNavigation()
		{
			string frame=InternalOwnerFrame;

			if (!string.IsNullOrEmpty(frame))
			{
				string refreshScript="<script language='javascript'>" + InternalOwnerFrame + ".location=" + InternalOwnerFrame + ".location;</script>";
                
				HttpContext.Current.Response.Write(refreshScript);
			}
		}

		#endregion

		#region Private methods
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnBack_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			this.PerformActionBack();
		}

		/// <summary>
		/// 
		/// </summary>
		private bool HasContext
		{
			get
			{
				return (!SiteNavigation.CallContextStack.IsEmpty);
			}
		}

		/// <summary>
		/// Impostazione visibilità campi
		/// </summary>
		private void SetFieldsVisibility()
		{
			this.btnBack.Visible=this.HasContext;
			this.lblCallContextList.Visible=this.btnBack.Visible;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="callerContextDescription"></param>
		/// <returns></returns>
		private string GetNavigationContext(out string callerContextDescription)
		{
			string navigationContext=string.Empty;
			callerContextDescription=string.Empty;

			if (this.HasContext)
			{
				SiteNavigation.CallContext[] list=SiteNavigation.CallContextStack.GetContextList();

				int count=1;

				foreach (SiteNavigation.CallContext item in list)
				{
					if (count > this.MaxItemsViewed)
						break;

					if (navigationContext!=string.Empty)
						navigationContext = " >> " + navigationContext;

					navigationContext = item.ToString() + navigationContext;

					if (callerContextDescription==string.Empty)
						callerContextDescription=item.ToString();

					count++;
				}

				if (list.Length > this.MaxItemsViewed)
					navigationContext=" ... >> " + navigationContext;
			}

			return navigationContext;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private string GetNavigationContext()
		{
			string callerContextDescription;
			
			return this.GetNavigationContext(out callerContextDescription);
		}

		/// <summary>
		/// 
		/// </summary>
		private void RefreshNavigationContext()
		{
			string callerContextDescription;
			this.lblCallContextList.Text=this.GetNavigationContext(out callerContextDescription);
            
			this.btnBack.ToolTip="Torna a '" + callerContextDescription + "'";
		}

		/// <summary>
		/// Registrazione script client
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if (!this.Page.ClientScript.IsStartupScriptRegistered(this.Page.GetType(), scriptKey))
			{
				string scriptString = "<script language='javascript'>" + scriptValue + "</script>";
                
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, scriptString);
			}
		}

		/// <summary>
		/// Frame contenente il controllo di navigazione
		/// </summary>
		private static string InternalOwnerFrame
		{
			get
			{
				string ownerFrame=string.Empty;

				string sessionKey=SESSION_PREFIX + "ownerFrame";

				if (HttpContext.Current.Session[sessionKey]!=null)
					ownerFrame=HttpContext.Current.Session[sessionKey].ToString();

				return ownerFrame;
			}
			set
			{
				HttpContext.Current.Session[SESSION_PREFIX + "ownerFrame"]=value;
			}
		}

		/// <summary>
		/// Azione di ripristino contesto di chiamata
		/// </summary>
		private void PerformActionBack()
		{
            SiteNavigation.CallContext callerContext = SiteNavigation.CallContextStack.RestoreCaller();
            Session.Remove("dictionaryCorrispondente");

            if (callerContext != null)
            {
                if (callerContext.ContextFrameName != string.Empty)
                {
                    this.RegisterClientScript("BackFrame", callerContext.ContextFrameName + ".document.location='" + callerContext.Url + "';");
                }
                else
                {
                    this.Response.Redirect(callerContext.Url);
                }
            }
		}

		#endregion
	}
}