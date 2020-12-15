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
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.SitoAccessibile.Documenti
{
	public class VisualizzaDocumento : SessionWebPage
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				string idProfile;
				string docNumber;
				string versionId;
				bool isAllegato;

				this.ParsePageParameters(out idProfile,out docNumber,out versionId,out isAllegato);

				DocumentoHandler handler=new DocumentoHandler();
				SchedaDocumento schedaDocumento=handler.GetDocumento(idProfile,docNumber);

				this.ShowFileDocument(schedaDocumento,versionId,isAllegato);
			}
			catch(Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="idProfile"></param>
		/// <param name="docNumber"></param>
		/// <param name="versionId"></param>
		/// <param name="isAllegato"></param>
		private void ParsePageParameters(out string idProfile,out string docNumber,out string versionId,out bool isAllegato)
		{
			idProfile=this.GetQueryStringParameter("idProfile");
			docNumber=this.GetQueryStringParameter("docNumber");
			versionId=this.GetQueryStringParameter("versionId");
			
			try
			{
				isAllegato=Convert.ToBoolean(this.GetQueryStringParameter("isAllegato"));
			}
			catch
			{
				isAllegato=false;
			}
		}

		/// <summary>
		/// Visualizzazione del documento
		/// </summary>
		private void ShowFileDocument(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento,string versionId,bool isAllegato)
		{
			DocsPaWR.FileRequest fileRequest=this.GetFileRequest(schedaDocumento,versionId,isAllegato);

			DocsPaWR.FileDocumento fileDocument=this.GetFileDocument(fileRequest);

			if (fileDocument!=null)
			{
				this.RenderDocumentContent(fileDocument);
			}
			else
			{
				// Documento non trovato
			}
		}

		/// <summary>
		/// Rendering del contenuto del documento
		/// </summary>
		/// <param name="fileDocument"></param>
		private void RenderDocumentContent(DocsPAWA.DocsPaWR.FileDocumento fileDocument)
		{
			Response.ContentType=fileDocument.contentType;
			Response.AddHeader("content-disposition", "inline;filename=" + fileDocument.name);
			Response.AddHeader("content-lenght", fileDocument.content.Length.ToString());				
			Response.BinaryWrite(fileDocument.content);
		}

		/// <summary>
		/// Reperimento oggetto "DocsPaWR.FileRequest" richiesto per la visualizzazione
		/// </summary>
		/// <param name="schedaDocumento"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.FileRequest GetFileRequest(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento,string versionId,bool isAllegato)
		{
			DocsPaWR.FileRequest retValue=null;

			DocsPaWR.FileRequest[] listDocuments=schedaDocumento.documenti;
			if (isAllegato)
				listDocuments=schedaDocumento.allegati;

			foreach (DocsPAWA.DocsPaWR.FileRequest fileRequest in listDocuments)
			{
				if (fileRequest.versionId.Equals(versionId))
				{
					retValue=fileRequest;
					break;
				}
			}

			return retValue;
		}

		/// <summary>
		/// Reperimento oggetto "DocsPaWR.FileDocumento"
		/// </summary>
		/// <param name="fileRequest"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.FileDocumento GetFileDocument(DocsPAWA.DocsPaWR.FileRequest fileRequest)
		{
			DocsPaWR.DocsPaWebService ws=new DocsPaWebService();
			return ws.DocumentoGetFile(fileRequest,UserManager.getInfoUtente());
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}