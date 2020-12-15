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

namespace Amministrazione.Gestione_Funzioni
{
	/// <summary>
	/// Summary description for CercaFunzElementari.
	/// </summary>
	public class CercaFunzElementari : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.DropDownList ddl_funz;
		string xmldocpath = System.Configuration.ConfigurationManager.AppSettings["FILEAMMINISTRAZIONI"];

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				//----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
				if(Session.IsNewSession)
				{
					Response.Redirect("../Exit.aspx?FROM=EXPIRED");
				}
			
				AmmUtils.WebServiceLink  ws=new AmmUtils.WebServiceLink();
				if(!ws.CheckSession(Session.SessionID))
				{
					Response.Redirect("../Exit.aspx?FROM=ABORT");
				}
				// ---------------------------------------------------------------
	
				if (!IsPostBack)
				{					
					LoadFunzElementari();
				}
			}
			catch
			{				
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void LoadFunzElementari()
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(xmldocpath);

			XmlNode nodoFunzElem = xmlDoc.SelectSingleNode(@"//FUNZIONIELEMENTARI");
			if (nodoFunzElem!=null)
			{
				foreach(XmlNode nodo in nodoFunzElem)
				{
					ListItem item = new ListItem(nodo.SelectSingleNode("CODICE").InnerText,nodo.SelectSingleNode("CODICE").InnerText);
					ddl_funz.Items.Add(item);
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
