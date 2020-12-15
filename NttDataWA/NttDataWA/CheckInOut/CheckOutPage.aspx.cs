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
using NttDataWA.DocsPaWR;

namespace NttDataWA.CheckInOut
{
	/// <summary>
	/// Pagina utilizzata per effettuare il checkout di un documento
	/// </summary>
	public class CheckOutPage : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Controllo parametri
			if (Request.QueryString["location"]!=null &&
				Request.QueryString["idDocument"]!=null &&
				Request.QueryString["documentNumber"]!=null &&
				Request.QueryString["machineName"]!=null && 
				Request.QueryString["downloadFile"]!=null)
			{
				// Blocco del documento
				this.CheckOutDocument(Request.QueryString["idDocument"],
										Request.QueryString["documentNumber"],
										Request.QueryString["location"],
										Request.QueryString["machineName"],
										Convert.ToBoolean(Request.QueryString["downloadFile"]));
			}
			else
			{
				throw new ApplicationException("Parametro non fornito");
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

		/// <summary>
		/// 
		/// </summary>
		public static byte[] SessionContent
		{
			get
			{
				byte[] content=null;

				if (HttpContext.Current.Session["CheckOutPage.Content"]!=null)
				{
					content=(byte[]) HttpContext.Current.Session["CheckOutPage.Content"];
				}

				return content;
			}
			set
			{
				if (value==null)
					HttpContext.Current.Session.Remove("CheckOutPage.Content");
				else
					HttpContext.Current.Session["CheckOutPage.Content"]=value;
			}
		}

		/// <summary>
		/// Checkout di un documento e download del contenuto del file
		/// </summary>
		/// <param name="idDocument"></param>
		/// <param name="documentNumber"></param>
		/// <param name="machineName"></param>
		/// <param name="documentLocation">Percorso di estrazione del file</param>
		/// <param name="downloadFile">Se true, viene effettuato il download del file contestualmente al checkout</param>
		private void CheckOutDocument(string idDocument,string documentNumber,string documentLocation,string machineName,bool downloadFile)
		{
			CheckOutStatus checkOutStatus;

			ValidationResultInfo result=null;

			if (downloadFile)
			{
				byte[] content;

				result=CheckInOutServices.CheckOutDocumentWithFile(documentLocation,machineName,out checkOutStatus,out content);
				
				if (result.Value)
					SessionContent=content;
			}
			else
			{
				result=CheckInOutServices.CheckOutDocument(documentLocation,machineName,out checkOutStatus);
			}

			if (!result.Value)
			{
				// Scrittura dei messaggi di errore nel checkout
				Response.Write(this.GetErrorMessage(result));
			}
            else
            {
                System.Web.HttpContext.Current.Session["isCheckinOrOut"] = result.Value;
            }
		}

		/// <summary>
		/// Reperimento dell'eventuale messaggio di errore proveniente dal checkout
		/// </summary>
		/// <param name="resultInfo"></param>
		/// <returns></returns>
		private string GetErrorMessage(ValidationResultInfo resultInfo)
		{	
			string message=string.Empty;

			// Restituzione dei messaggi di validazione
			foreach (BrokenRule brokenRule in resultInfo.BrokenRules)
			{
				if (message!=string.Empty)
					message+=Environment.NewLine;

				message+=brokenRule.Description;
			}

			return message;
		}
	}
}