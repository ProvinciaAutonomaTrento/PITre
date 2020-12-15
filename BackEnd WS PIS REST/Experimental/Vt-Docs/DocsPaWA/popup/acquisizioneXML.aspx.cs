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

namespace DocsPAWA.popup {
	/// <summary>
	/// Summary description for acquisizionePDF.
	/// </summary>
	public class acquisizioneXML : DocsPAWA.CssPage {

		private string fileName = "";
		private string keepOriginal = "false";
		private string convertiPDF = "false";
        private string convertiPDFServer = "false";
		/// <summary>
		/// Cancellazione del file locale successivamente all'upload sul server
		/// </summary>
		private string removeLocalFile = "false";

        /// <summary>
        /// Se true, il file da inviare ha un corrispondente originale cartaceo
        /// </summary>
        private string cartaceo = "false";

        private string pdfSincrono = "false";
        
		public string path {get {return Utils.getHttpFullPath(this);}}
		public string FileName {get {return fileName;}}
		public string KeepOriginal {get {return keepOriginal;}}
		public string ConvertiPDF {get {return convertiPDF;}}
        public string ConvertiPDFServer { get { return convertiPDFServer; } }
		public string RemoveLocalFile {get {return removeLocalFile;}}
        public string Cartaceo { get { return cartaceo; } }


        public string PdfSincrono { get { return pdfSincrono; } }

		private void Page_Load(object sender, System.EventArgs e) 
        {
			fileName =  Request["fileName"].Replace("\\", "\\\\");
			keepOriginal = Request["keepOriginal"];
			convertiPDF = Request["convertiPDF"];
            convertiPDFServer = Request["convertiPDFServer"];
			removeLocalFile = Request["removeLocalFile"];
            cartaceo = Request["cartaceo"];
            pdfSincrono = Request["pdfSincrono"];
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e) {
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
		private void InitializeComponent() {    
			this.ID = "acquisizioneXML";
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
