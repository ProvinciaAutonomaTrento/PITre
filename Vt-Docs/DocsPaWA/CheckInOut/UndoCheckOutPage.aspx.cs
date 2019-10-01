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

namespace DocsPAWA.CheckInOut
{
	/// <summary>
	/// Summary description for UndoCheckOutPage.
	/// </summary>
	public class UndoCheckOutPage : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			this.UndoCheckOutDocument();
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
		/// Annullamento del blocco sul documento
		/// </summary>
		private void UndoCheckOutDocument()
		{
			ValidationResultInfo retValue=CheckInOutServices.UndoCheckOutDocument();

			if (!retValue.Value)
			{
				// Scrittura dei messaggi di errore nel checkout
				Response.Write(this.GetErrorMessage(retValue));
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
                if (message != string.Empty)
                    message += Environment.NewLine;

                message += brokenRule.Description;
            }
        
			return message;
		}
	}
}