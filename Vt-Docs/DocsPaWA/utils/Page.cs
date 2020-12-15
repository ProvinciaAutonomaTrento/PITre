using System;
using System.Configuration;

namespace DocsPaWA.Utils
{
	/// <summary>
	/// Summary description for Page.
	/// </summary>
	public class Page : DocsPAWA.CssPage
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			// Validate Login
//			DocsPaWR.ValidationResult result = DocsPAWA.UserManager.ValidateLogin(this, this.Session.SessionID);
//			
//			if(result != DocsPAWA.DocsPaWR.ValidationResult.OK)
//			{
//				//Response.Redirect(ConfigurationManager.AppSettings["appRoot"] + ConfigurationManager.AppSettings["invalidSessionUrl"] + "?result=" + result, true);				
//				Response.Redirect(DocsPAWA.Utils.getHttpFullPath(this.Page) + "/sessionaborted.aspx?result=" + result, true);
//			}

			base.OnInit(e);
		}
	}
}
