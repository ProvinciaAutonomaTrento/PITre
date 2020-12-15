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

namespace Amministrazione
{
	/// <summary>
	/// Summary description for Help.
	/// </summary>
	public class Help : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lbl_testa;
		protected System.Web.UI.HtmlControls.HtmlTableCell textArea;
		
		string xmldocpath = AppDomain.CurrentDomain.BaseDirectory + @"AdminTool\Xml\Help.xml";

		private void Page_Load(object sender, System.EventArgs e)
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
				if (File.Exists(xmldocpath))
				{				
					//carica il file XML
					LoadXmlDoc();	
				} 
				else 
				{
					this.lbl_testa.Text="HELP non disponibile!";				
				}
			} 
		}

		public void LoadXmlDoc()
		{
			try
			{
				XmlDocument xmlDoc = new XmlDocument();			
				xmlDoc.Load(xmldocpath);

				// punta all'help giusto
				string codice = Request.QueryString["from"];
                if (SAAdminTool.Utils.GetAbilitazioneAtipicita())
                    codice = "SUO_ATIPICITA";
				XmlNode nodeHelp = xmlDoc.SelectSingleNode(@"//VOCE[CODICE='" + codice + "']");	
			
				lbl_testa.Text = "HELP > " + nodeHelp.SelectSingleNode("NOME").InnerText;
				this.textArea.InnerHtml = nodeHelp.SelectSingleNode("TESTO").InnerText;
			}
			catch
			{
				this.lbl_testa.Text="HELP non disponibile!";
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
