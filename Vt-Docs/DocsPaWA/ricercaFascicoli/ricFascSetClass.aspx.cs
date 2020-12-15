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

namespace DocsPAWA.ricercaFascicoli
{
	/// <summary>
	/// Summary description for ricFascSetClass.
	/// </summary>
	public class ricFascSetClass : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			 if(this.Request.QueryString["classId"] != null && !this.Request.QueryString["classId"].Equals("")) 
				{

					Hashtable HashClass=FascicoliManager.getTheHash(this);

					if (HashClass!=null)
					{
						int index = int.Parse(this.Request.QueryString["classId"]);
						DocsPaWR.FascicolazioneClassificazione classificazione=(DocsPAWA.DocsPaWR.FascicolazioneClassificazione)HashClass[index];

						FascicoliManager.setClassificazioneSelezionata(this,classificazione); 	//luca

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
