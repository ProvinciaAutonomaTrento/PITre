namespace SAAdminTool.CheckInOut
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Reflection;
	using System.IO;
	using SAAdminTool.DocsPaWR;

	/// <summary>
	///	UserControl che gestisce il flusso di lavoro della
	///	funzione di CheckIn - CheckOut di documenti.
	/// </summary>
	public class CheckInOutController : System.Web.UI.UserControl
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack)
			{
                // Impostazione path cartella checkinout
                this.SetCheckInOutFolderUrl();
			}
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
        /// Impostazione path cartella checkinout
        /// </summary>
        protected virtual void SetCheckInOutFolderUrl()
        {
            if (this.UseRelativeFolderPath)
            {
                // Se è attivata la gestione dei path statici (per junction),
                // impostazione nel viewstate del path impostato staticamente
                this.ViewState["CheckInOutFolderUrl"] = this.RelativeFolderPath;
            }
            else
            {
                this.ViewState["CheckInOutFolderUrl"] = SAAdminTool.Utils.getHttpFullPath(this.Page) + "/CheckInOut/";
            }
        }

        /// <summary>
        /// Impostazione e reperimento del path statico del folder checkinout
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public string RelativeFolderPath
        {
            get
            {
                if (this.ViewState["RelativeFolderPath"] != null)
                    return this.ViewState["RelativeFolderPath"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["RelativeFolderPath"] = value;
            }
        }

        /// <summary>
        /// Verifica se è attiva la gestion dei path statici (per junction)
        /// </summary>
        protected bool UseRelativeFolderPath
        {
            get
            {
                return (HttpContext.Current.Session["useStaticRootPath"] != null);
            }
        }

		/// <summary>
		/// Visualizzazione finestra di dialogo relativa alle informazioni di stato sul checkout
		/// </summary>
		public void ShowDialogCheckOutStatus(string idDocument,string documentNumber)
		{
			this.RegisterClientScript("ShowDialogCheckOutStatus","ShowDialogCheckOutStatus('" + idDocument + "', '" + documentNumber + "');");
		}

		/// <summary>
		/// Reperimento nome computer
		/// </summary>
		public static string MachineName
		{
			get
			{
				string machineName=string.Empty;

				string ipAddress=HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

				if (ipAddress!=null && ipAddress!=string.Empty)
				{
					try
					{
						System.Net.IPHostEntry entry=System.Net.Dns.GetHostByAddress(ipAddress);
						machineName=entry.HostName;
					}
					catch
					{
						machineName=ipAddress;
					}
				}

				return machineName;
			}
		}

		/// <summary>
		/// Verifica se l'utente corrente è abilitato all'utilizzo 
		/// della funzione di checkin-checkout
		/// </summary>
		public bool UserEnabled
		{
			get
			{
				return CheckInOutServices.UserEnabled;
			}
		}

        /// <summary>
        /// Verifica se l'utente corrente è abilitato all'utilizzo 
        /// della funzione di Download del documento
        /// </summary>
        public bool UserEnabledSimpleDownload
        {
            get
            {
                return SaveFileServices.UserEnabled;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected string CheckInOutFolderUrl
		{
			get
			{
                return this.ViewState["CheckInOutFolderUrl"].ToString();
			}
		}

		/// <summary>
		/// Url della pagina web di attesa per le operazioni di checkInOut
		/// </summary>
		protected string CheckOutWaitingPageUrl
		{
			get
			{
				return this.CheckInOutFolderUrl + "CheckOutWaitingPage.html";
			}
		}

		/// <summary>
		/// Url della pagina web di attesa per le operazioni di checkInOut
		/// </summary>
		protected string CheckInWaitingPageUrl
		{
			get
			{
				return this.CheckInOutFolderUrl + "CheckInWaitingPage.htm";
			}
		}

		/// <summary>
		/// Url della pagina web di attesa per le operazioni di checkInOut
		/// </summary>
		protected string OpenCheckOutWaitingPageUrl
		{
			get
			{
				return this.CheckInOutFolderUrl + "OpenCheckOutWaitingPage.htm";
			}
		}

		/// <summary>
		/// Url della pagina web di attesa per le operazioni di checkInOut
		/// </summary>
		protected string UndoCheckOutWaitingPageUrl
		{
			get
			{
				return this.CheckInOutFolderUrl + "UndoCheckOutWaitingPage.htm";
			}
		}

		/// <summary>
		/// Url della pagina web necessaria per effettuare il checkin di un documento
		/// </summary>
		protected string CheckInPageUrl
		{
			get
			{
				return this.CheckInOutFolderUrl + "CheckInPage.aspx";
			}
		}

		/// <summary>
		/// Url della pagina web necessaria per effettuare il checkout di un documento
		/// </summary>
		protected string CheckOutPageUrl
		{
			get
			{
				return this.CheckInOutFolderUrl + "CheckOutPage.aspx";
			}
		}

        /// <summary>
        /// Url della pagina web necessaria per effettuare il download semplice di un documento 
        /// </summary>
        protected string SaveFilePageUrl
        {
            get
            {
                return this.CheckInOutFolderUrl + "SaveFilePage.aspx";
            }
        }

        /// <summary>
        /// Url della pagina web necessaria per effettuare il download di un documento in checkout
        /// </summary>
        protected string DownloadCheckOutPageUrl
        {
            get
            {
                return this.CheckInOutFolderUrl + "DownloadCheckOutPage.aspx";
            }
        }

		/// <summary>
		/// Url della pagina web necessaria l'UndoCheckOut di un documento
		/// </summary>
		protected string UndoCheckOutPageUrl
		{
			get
			{
                String undoPageUrl = this.CheckInOutFolderUrl + "UndoCheckOutPage.aspx";

                // Se il documento ha associato un modello M/Text, viene cancellato il documento
                // dai server M/Text
                if (!String.IsNullOrEmpty(this.CheckOutFilePath) && this.CheckOutFilePath.Contains("mtext://"))
                    undoPageUrl = String.Format("{0}/Models/MText/Delete.aspx", Utils.getHttpFullPath());

				return undoPageUrl;
			}
		}

		/// <summary>
		/// Reperimento percorso del file per il documento in CheckOut
		/// </summary>
		protected string CheckOutFilePath
		{
			get
			{
				string retValue=string.Empty;
                
				if (CheckOutContext.Current!=null && CheckOutContext.Current.Status!=null)
					retValue=CheckOutContext.Current.Status.DocumentLocation;
                
				return retValue.Replace(@"\",@"\\");
			}
		}

		/// <summary>
		/// Registrazione script client
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if(!this.Page.ClientScript.IsStartupScriptRegistered(this.Page.GetType(), scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), scriptKey, scriptString);
			}
		}

        /// <summary>
        /// Url della pagina da richiamare per visualizzare un documento M/Text
        /// </summary>
        protected String MTextShowDocumentUrl
        {
            get
            {
                String retVal = String.Empty;
                if(CheckOutContext.Current != null && 
                    CheckOutContext.Current.Status != null &&
                    !String.IsNullOrEmpty(CheckOutContext.Current.Status.DocumentLocation) &&
                    CheckOutContext.Current.Status.DocumentLocation.StartsWith("mtext://"))
                    retVal = String.Format("{0}/Models/MText/Show.aspx?fqn={1}", 
                        Utils.getHttpFullPath(), 
                        HttpUtility.UrlEncode(CheckOutContext.Current.Status.DocumentLocation.Remove(0, 8))); 
 
                // Restituzione del valore
                return retVal;
            }
        }
	}
}
