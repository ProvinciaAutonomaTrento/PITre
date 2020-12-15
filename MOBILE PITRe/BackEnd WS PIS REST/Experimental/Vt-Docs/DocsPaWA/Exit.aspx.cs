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

namespace DocsPAWA
{
	/// <summary>
	/// Summary description for Exit.
	/// </summary>
	public class Exit : System.Web.UI.Page
	{
		#region Web Form Designer generated code
		protected override void OnLoad(EventArgs e)
		{
			string appConfigValue=ConfigSettings.getKey(ConfigSettings.KeysENUM.DISABLE_LOGOUT_CLOSE_BUTTON);

			if (appConfigValue==null || (!Convert.ToBoolean(appConfigValue)))
				Session.Abandon();

			base.OnLoad (e);
		}


		override protected void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}
		
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			//
		}
		#endregion
	}
}
