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
using System.Xml;
using System.IO;

namespace DocsPAWA.SmartClient
{
	/// <summary>
	/// Summary description for upload.
	/// </summary>
	public class UploadPageHandler : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e) 
		{	
			try
			{
				Stream stream=Request.InputStream;
				
				XmlDocument document=new XmlDocument();
				document.Load(stream);

				this.UploadFile(document);
			}
			catch (Exception ex)
			{
				ErrorManager.setError(this, ex);
			}
		}
		
		private void UploadFile(XmlDocument document)
		{
			DocsPaWR.FileDocumento fileDoc=new DocsPAWA.DocsPaWR.FileDocumento();

			XmlNode rootNode=document.SelectSingleNode("File");

			if (rootNode!=null)
			{
				fileDoc.name=rootNode.Attributes.GetNamedItem("name").Value;
				fileDoc.fullName=rootNode.Attributes.GetNamedItem("fullPath").Value;
                fileDoc.cartaceo = this.Cartaceo;
				string content=rootNode.Attributes.GetNamedItem("content").Value;
				fileDoc.content=Convert.FromBase64String(content);

				fileDoc.length=fileDoc.content.Length;

				FileManager.getInstance(Session.SessionID).uploadFile(this,fileDoc,false);
			}
		}

        /// <summary>
        /// Se true, al documento corrisponde un originale cartaceo
        /// </summary>
        private bool Cartaceo
        {
            get
            {
                bool retValue;
                bool.TryParse(this.Request.QueryString["cartaceo"], out retValue);
                return retValue;
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}