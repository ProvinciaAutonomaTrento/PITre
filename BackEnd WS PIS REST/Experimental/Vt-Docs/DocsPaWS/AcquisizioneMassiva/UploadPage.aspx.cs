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
using System.IO;

namespace DocsPAWS.AcquisizioneMassiva
{
	/// <summary>
	/// Pagina utilizzata per ricevere i file in upload dal client dell'acquisizione massiva.
	/// Il file viene copiato nel percorso fornito nel parametro in queryString "pathAcquisizioneBatch".
	/// 
	/// </summary>
	public class UploadPage : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			string responseMessage=string.Empty;			

			// Reperimento percorso in cui effettuare l'upload dei file acquisiti
			string pathAcquisizioneBatch=BusinessLogic.AcquisizioneMassiva.Configurations.GetPathAcquisizioneBatch();

			if (Request.Files.Count>0)
			{
				if (!Directory.Exists(pathAcquisizioneBatch))
					// Se la cartella di upload non esiste, viene creata
					Directory.CreateDirectory(pathAcquisizioneBatch);

				if (!pathAcquisizioneBatch.EndsWith(@"\"))
					pathAcquisizioneBatch += @"\";

				HttpPostedFile file=Request.Files[0];
				file.SaveAs(pathAcquisizioneBatch + file.FileName);			
			}
			else
			{
				responseMessage += "Nessun file trasmesso dal client";
			}
			
			if (responseMessage.Equals(string.Empty))
				responseMessage="OK";
			
			Response.Write(responseMessage + "EndUploadResponse");
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
