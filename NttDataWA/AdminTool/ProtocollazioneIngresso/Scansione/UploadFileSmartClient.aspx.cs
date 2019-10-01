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
using SAAdminTool;
using ProtocollazioneIngresso.Log;

namespace ProtocollazioneIngresso.Scansione
{
	/// <summary>
	/// Summary description for UploadFileSmartClient.
	/// </summary>
	public class UploadFileSmartClient : System.Web.UI.Page
	{
		public string path 
		{
			get 
			{
				return SAAdminTool.Utils.getHttpFullPath(this);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{	
			try
			{
				if (Request.QueryString.Count>0)
				{
					string filePath=Request.QueryString["filePath"];
					
					string convertPdf="false";
					if (Request.QueryString["convertPdf"]!=null)
						convertPdf=Request.QueryString["convertPdf"];

					string recognizeText="false";
					if (Request.QueryString["recognizeText"]!=null)
						recognizeText=Request.QueryString["recognizeText"];

					string removeLocalFile="false";
					if (Request.QueryString["removeLocalFile"]!=null)
						removeLocalFile=Request.QueryString["removeLocalFile"];

					if (filePath!=null)
					{
						filePath=filePath.Replace(@"\",@"\\");

						this.RegisterClientScript("UploadFile",
							"UploadFile('" + filePath + "'," + 
											convertPdf + "," +
											recognizeText + "," + 
											removeLocalFile + ");");
					}
				}
				else
				{
					Stream stream=Request.InputStream;
				
					XmlDocument document=new XmlDocument();
					document.Load(stream);

                    string errorMessage;
                    if (!this.UploadFile(document, out errorMessage))
					{
						// In caso di errore nell'invio del documento,
						// viene impostato a "1" lo statuscode http 
						// dell'output restituito al client
						Response.StatusCode=1;
					}
				}
			}
			catch (Exception ex)
			{
				ErrorManager.setError(this, ex);
			}
		}

		/// <summary>
		/// Registrazione script client
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if(!this.Page.IsStartupScriptRegistered(scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
				this.Page.RegisterStartupScript(scriptKey, scriptString);
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
		private bool UploadFile(XmlDocument document, out string errorMessage)
		{
			bool retValue=false;
            errorMessage = string.Empty;

            SAAdminTool.DocsPaWR.FileDocumento fileDoc = new SAAdminTool.DocsPaWR.FileDocumento();

			XmlNode rootNode=document.SelectSingleNode("File");

			if (rootNode!=null)
			{
				fileDoc.name=rootNode.Attributes.GetNamedItem("name").Value;
				fileDoc.fullName=rootNode.Attributes.GetNamedItem("fullPath").Value;
                fileDoc.cartaceo = true;

				string content=rootNode.Attributes.GetNamedItem("content").Value;
				fileDoc.content=Convert.FromBase64String(content);

				fileDoc.length=fileDoc.content.Length;

				DocumentUploader docUploader=new DocumentUploader(this);
                retValue = docUploader.Upload(fileDoc, false, out errorMessage);
			}

			return retValue;
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
