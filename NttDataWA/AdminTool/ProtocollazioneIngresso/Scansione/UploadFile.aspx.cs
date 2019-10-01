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
using SAAdminTool;
using ProtocollazioneIngresso.Log;

namespace ProtocollazioneIngresso.Scansione
{
	/// <summary>
	/// Summary description for upload.
	/// </summary>
	public class UploadFile : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e) 
		{	
			try 
			{	
                if (Request.QueryString.Count>1)
				{
					string filePath=Request.QueryString["filePath"];

                    string convertPdf = "false";
                        if (Request.QueryString["convertPdf"] != null)
                            convertPdf = Request.QueryString["convertPdf"];
                    
                    string recognizeText="false";
					if (Request.QueryString["recognizeText"]!=null)
						recognizeText=Request.QueryString["recognizeText"];

					string acrobatIntegration=this.IsEnabledPdfAcrobatIntegration().ToString().ToLower();

					string removeLocalFile="false";
					if (Request.QueryString["removeLocalFile"]!=null)
						removeLocalFile=Request.QueryString["removeLocalFile"];

					if (filePath!=null)
					{
						filePath=filePath.Replace(@"\",@"\\");

                        //Verifico se è abilitata la conversione pdf ed è stata rihiesta
                        if (Convert.ToBoolean(SAAdminTool.Utils.isEnableConversionePdfLatoServer()) && Convert.ToBoolean(convertPdf))
                        {
                            this.RegisterClientScript("UploadFile",
                                "UploadFileConvertPdfServer('" + filePath + "'," +
                                convertPdf + "," +
                                recognizeText + "," +
                                acrobatIntegration + ", " +
                                removeLocalFile + ");");
                        }
                        else
                        {
                            this.RegisterClientScript("UploadFile",
                                "UploadFile('" + filePath + "'," +
                                convertPdf + "," +
                                recognizeText + "," +
                                acrobatIntegration + ", " +
                                removeLocalFile + ");");
                        }
					}
				}
				else
				{
                    XmlDocument xmlDom = new XmlDocument();
                    xmlDom.Load(Request.InputStream);
                    bool convertPdfSever = false;
                    if (Request.QueryString["convertPdfServer"] != null)
                        convertPdfSever = Convert.ToBoolean(Request.QueryString["convertPdfServer"]);

                    string errorMessage;
                    bool retValue = this.AppendFile(xmlDom, "CurrentVersion", false, convertPdfSever, out errorMessage);

                    if (retValue)
                        retValue = this.AppendFile(xmlDom, "NewVersion", true, convertPdfSever, out errorMessage);
                
                    if (!retValue)
                    {
                        // In caso di errore nell'invio del documento,
                        // viene impostato a "1" lo statuscode http 
                        // dell'output restituito al client
                        Response.StatusCode = 1;
                        Response.StatusDescription = errorMessage;

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
        /// <param name="xmlDom"></param>
        /// <param name="nodeName"></param>
        /// <param name="addVersion"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
		private bool AppendFile(XmlDocument xmlDom,string nodeName,bool addVersion,bool convertPdfServer, out string errorMessage) 
		{
			bool retValue=true;
            errorMessage = string.Empty;

			XmlNode file = xmlDom.SelectSingleNode("root/" + nodeName);
			if (file != null) 
			{
				string fileBuf = file.FirstChild.Value;

                SAAdminTool.DocsPaWR.FileDocumento fileDoc = new SAAdminTool.DocsPaWR.FileDocumento();
				fileDoc.name = file.Attributes["FileName"].Value;
                fileDoc.cartaceo = true;
				fileDoc.fullName = fileDoc.name;
				fileDoc.content = Convert.FromBase64String(fileBuf);
				fileDoc.length = fileDoc.content.Length;
                
				DocumentUploader docUploader=new DocumentUploader(this);
                retValue = docUploader.Upload(fileDoc, convertPdfServer, out errorMessage);
			}

			return retValue;
		}
		
		private bool IsEnabledPdfAcrobatIntegration()
		{
			bool retValue=false;

			try
			{
				retValue=Convert.ToBoolean(SAAdminTool.ConfigSettings.getKey(SAAdminTool.ConfigSettings.KeysENUM.ADOBE_ACROBAT_INTEGRATION));
			}
			catch
			{
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
			this.ID = "frmUploadFile";
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}