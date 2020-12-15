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

namespace NttDataWA.SmartClient
{
	/// <summary>
	/// Visualizzazione del documento firmato
	/// </summary>
	public class SignedRecordViewer : System.Web.UI.Page
	{
        private string requestType = string.Empty;
        private string idDocument = string.Empty;

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = -1;

            string componentType = UIManager.UserManager.getComponentType(Request.UserAgent);

            if (Request.QueryString["idDocumento"] != null)
                idDocument = Request.QueryString["idDocumento"].ToString();

            if (requestHash)
            {

                bool wantcosign = requestCosign;
                string strResp="";

                DocsPaWR.MassSignature massSignature = UIManager.FileManager.getSelectedMassSignature(idDocument);

                if (massSignature != null)
                {
                    strResp = massSignature.base64Sha256;
                    if (!String.IsNullOrEmpty(massSignature.base64Signature))
                        strResp += string.Format("#{0}", massSignature.base64Signature);


                    if (componentType == Utils.Constans.TYPE_SMARTCLIENT || componentType == Utils.Constans.TYPE_ACTIVEX)
                    {
                        byte[] byteResp = System.Text.ASCIIEncoding.ASCII.GetBytes(strResp);
                        Response.BinaryWrite(byteResp);
                        Response.Flush();
                    }
                    else
                    {
                        Response.Write(strResp);
                    }
                }
                else
                {
                    Response.Write(null);
                }
            }
            else
            {
                requestType = Request.QueryString["type"];

                SmartClient.FirmaDigitaleMng firmaDigitaleMng = new SmartClient.FirmaDigitaleMng();

                NttDataWA.DocsPaWR.FileDocumento fileFirmato = null;

                if (string.IsNullOrEmpty(idDocument))
                    fileFirmato = firmaDigitaleMng.GetSignedDocument(this);
                else
                    fileFirmato = firmaDigitaleMng.GetSignedDocument(this, idDocument);

                firmaDigitaleMng = null;

                if (fileFirmato != null)
                {
                    if (requestType == "applet")
                    {
                        string base64String = System.Convert.ToBase64String(fileFirmato.content, 0, fileFirmato.content.Length);

                        Response.Write(base64String);
                    }
                    else
                    {
                        Response.BinaryWrite(fileFirmato.content);
                        Response.Flush();
                    }
                }
                else
                {
                    //Response.End();
                    //torno una response NULL, quindi l'fso non salvando nulla dovrebbe tornare null
                    Response.Write(null);
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


        /// <summary>
        /// Reperimento id del documento da query string
        /// </summary>
        protected bool requestCosign
        {
            get
            {
                string valInReq = this.Request.QueryString["isCosign"];
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
