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

namespace NttDataWA.DigitalSignature
{
	/// <summary>
	/// Visualizzazione del documento firmato
	/// </summary>
	public class SignedRecordViewer : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
            Response.Expires = -1;

            if (requestHash)
            {
                DocsPaWR.MassSignature massSignature = UIManager.FileManager.getSelectedMassSignature();
                if (massSignature != null)
                    Response.Write(massSignature.base64Sha256);
                else
                    throw new Exception("UIManager.FileManager.getSelectedMassSignature() in errore.");
            }
            else {
                try {

                    DigitalSignature.DigitalSignManager firmaDigitaleMng = new DigitalSignature.DigitalSignManager();
            
                    NttDataWA.DocsPaWR.FileDocumento fileFirmato = firmaDigitaleMng.GetSignedDocument(this);

			        firmaDigitaleMng=null;

                    if (fileFirmato != null)
                    {
                        Response.BinaryWrite(fileFirmato.content);
                        Response.Flush();
                    }
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return;
                }
            }
		}

        /// <summary>
        /// Reperimento id del documento da query string
        /// </summary>
        protected bool requestHash
        {
            get
            {
                string valInReq = this.Request.QueryString["isHash"];
                if (string.IsNullOrEmpty(valInReq))
                {
                    return false;
                }
                else
                {
                    return bool.Parse(valInReq);
                }
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
