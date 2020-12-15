namespace DocsPAWA.SitoAccessibile
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///	Gestione grafica delle informazioni relativamente 
	///	al contesto dell'utente corrente
	/// </summary>
	public class UserContext : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label lblCurrentRole;
		protected System.Web.UI.WebControls.Label lblCurrentUser;
		protected System.Web.UI.HtmlControls.HtmlImage imgXhtml;
		protected System.Web.UI.HtmlControls.HtmlImage imgCss;

		

		private void Page_Load(object sender, System.EventArgs e)
		{
			string imgDirUrl = getHttpFullPath(this).ToString() + "/SitoAccessibile/Images/";
			this.imgXhtml.Src = imgDirUrl + "w3c-xhtml.png";
			this.imgCss.Src = imgDirUrl + "w3c-css.gif";
			this.Fetch();
			
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

		public void RefreshUserContext()
		{
			this.Fetch();
		}

		private void Fetch()
		{
			this.lblCurrentUser.Text=UserManager.getUtente().descrizione;
			this.lblCurrentRole.Text=UserManager.getRuolo().descrizione;
            
            string coloreAmministrazione = findFontColor(UserManager.getUtente().idAmministrazione);
            if (findFontColor(UserManager.getUtente().idAmministrazione) != "")
            {
                string[] colorSplit = coloreAmministrazione.Split('^');
                string red = colorSplit[0];
                string green = colorSplit[1];
                string blu = colorSplit[2];
                this.lblCurrentUser.ForeColor = System.Drawing.Color.FromArgb(Convert.ToInt16(red), Convert.ToInt16(green), Convert.ToInt16(blu));
                this.lblCurrentRole.ForeColor = System.Drawing.Color.FromArgb(Convert.ToInt16(red), Convert.ToInt16(green), Convert.ToInt16(blu));
            }
        }

        private string findFontColor(string idAmm)
        {
            return FileManager.findFontColor(idAmm);
        }

		private static string getHttpFullPath(System.Web.UI.UserControl page) 
		{
			string h="http://" + page.Request.Url.Host + page.Request.ApplicationPath;

			if(page!=null 
				&& page.Request!=null 
				&& page.Request.Url!=null &&
				page.Request.Url.Host!=null)
			{
				if(!page.Request.Url.Port.Equals(80))
				{
					h= page.Request.Url.Scheme+"://" + page.Request.Url.Host +":"+ page.Request.Url.Port + page.Request.ApplicationPath;
				}
				else
				{
					h=page.Request.Url.Scheme+"://" + page.Request.Url.Host + page.Request.ApplicationPath;
				}
			}

			return h;
		}
        
        public bool existsLogoAmm
        {
            get
            {
                return fileExist("logo.gif", "LoginFE");
            }
            // set { login.IdAmministrazione = value; }
        }

        private bool fileExist(string fileName, string type)
        {
            return FileManager.fileExist(fileName, type);
        }
    }
}
