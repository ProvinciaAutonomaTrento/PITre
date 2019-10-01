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
using NttDataWA.Utils;

namespace NttDataWA.SmartClient
{
	/// <summary>
	/// Summary description for acquisizionePDF.
	/// </summary>
	public class UploadDocument : System.Web.UI.Page 
	{

		private string fileName = "";
		private string convertiPDF = "false";
		private string recognizeText = "false";
        
		/// <summary>
		/// Cancellazione del file locale successivamente all'upload sul server
		/// </summary>
		private string removeLocalFile = "false";

        /// <summary>
        /// Flag, indica se per il documento è disponibile un originale cartaceo
        /// </summary>
        private string cartaceo = "false";

		public string path {get {return utils.getHttpFullPath();}}
		public string FileName {get {return fileName;}}
		public string ConvertiPDF {get {return convertiPDF;}}
		public string RecognizeText {get {return recognizeText;}}
		public string RemoveLocalFile {get {return removeLocalFile;}}
        public string Cartaceo { get { return cartaceo; } }

		private void Page_Load(object sender, System.EventArgs e) 
		{
			fileName =  Request["fileName"].Replace("\\", "\\\\");
			convertiPDF = Request["convertiPDF"];
			recognizeText = Request["recognizeText"];
			removeLocalFile = Request["removeLocalFile"];
            cartaceo = Request["cartaceo"];
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
			this.ID = "acquisizioneXML";
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
