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


namespace Amministrazione.Gestione_Utenti
{
	/// <summary>
	/// Summary description for StatoConnUtenti.
	/// </summary>
	public class StatoConnUtenti : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lbl_msg;
		protected System.Web.UI.HtmlControls.HtmlTableCell outputArea;
		protected System.Web.UI.WebControls.Button btn_chiudi;
	
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

			btn_chiudi.Attributes.Add("onclick","javascript: window.close();");	
			//Response.AddHeader("refresh", "15;URL=StatoConnUtenti.aspx");

			string codAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"0");

			try
			{
				if(Request.QueryString["loopOn"] == null || Request.QueryString["loopOn"] == Boolean.TrueString)
				{
					// Chiamata asincrona al WS
					
					string xmlStream = ws.ElencoUtentiConnessi(codAmm);
					
					if(xmlStream != null && xmlStream != "")
					{
                        string theHtml = "";

                        int numUtenti = ws.NumeroUtentiConnessi(codAmm);
                        if (numUtenti != 0)
                            theHtml += "Ci sono " + numUtenti + " Utenti connessi<br /><br />";

						theHtml += "<table 'width=100%' class='contenitore' cellSpacing='0' cellPadding='2' align='center' BorderColor='Gray' border='1'>";
						theHtml += "<tr bgcolor='#810D06'>";
						theHtml += "<td align='center' class='menu_1_bianco_dg'>Utente</td>";
						theHtml += "<td align='center' class='menu_1_bianco_dg'>Connesso dal</td>";
						theHtml += "<td align='center' class='menu_1_bianco_dg'>Sconnetti</td></tr>";

						XmlDocument doc = new XmlDocument();
						doc.LoadXml(xmlStream);

						/* struttura dell'XML:
							* 
							* <UTENTI>
							*		<UTENTE>
							*			<USER_ID />
							*			<DESCRIZIONE />
							*			<DATAORA />
							*		</UTENTE>
							*		<UTENTE />
							*		...
							*		...
							* </UTENTI>
						*/

						//pulisce la lista
						this.outputArea.InnerHtml = null;

						XmlNode utenti = doc.SelectSingleNode("UTENTI");
						if(utenti != null)
						{
							foreach(XmlNode utente in utenti)
							{
								theHtml += "<tr><td class='testo_grigio_scuro'>" + utente.ChildNodes[0].InnerText + " - "  + utente.ChildNodes[1].InnerText + "</td>";
								theHtml += "<td class='testo_grigio_scuro' align='center'>" + utente.ChildNodes[2].InnerText + "</td>";
								theHtml += "<td class='testo_grigio_scuro' align='center'><a href='StatoConnUtenti.aspx?loopOn=" + Boolean.FalseString + "&user_id=" + utente.ChildNodes[0].InnerText + "' alt='Sconnetti utente " + utente.ChildNodes[1].InnerText + "'><img src='../Images/chiudi.gif' border='0'></a></td></tr>";																																			
							}				
						}	
						theHtml += "</table>";												
						this.outputArea.InnerHtml = theHtml;
					}
				}
				else
				{										
					// chiamata sincrona

					string theUserId = Request.QueryString["user_id"];
					if(!ws.DisconnettiUtente(codAmm,theUserId))
					{
						lbl_msg.Text = "Errore nella disconnessione dell'utente.";
					}
					//this.Response.Redirect("StatoConnUtenti.aspx?loopOn=" + Boolean.TrueString);
					if(!this.Page.IsStartupScriptRegistered("redirectJS"))
					{					
						string scriptString = "<SCRIPT>window.location.href = 'StatoConnUtenti.aspx?loopOn=" + Boolean.TrueString + "';</SCRIPT>";				
						this.Page.RegisterStartupScript("redirectJS", scriptString);
					}	
				}									
			}
			catch(Exception exception)
			{
				System.Diagnostics.Debug.Write("Errore nella lettura stato utenti connessi.\n" + exception.ToString());
				lbl_msg.Text = "Errore nella lettura stato utenti connessi.";
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
