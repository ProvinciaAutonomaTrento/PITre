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
	/// Summary description for PermessiFunzioni.
	/// </summary>
	public class PermessiFunzioni : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lbl_testa;
		protected System.Web.UI.HtmlControls.HtmlTableCell textArea;
		string cod_ruolo;
		string desc_ruolo;
		protected System.Web.UI.WebControls.ImageButton btn_find;
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
					cod_ruolo = Request.QueryString["codruolo"];		
					VisualPermessi();
				}
			}
			catch
			{
				textArea.InnerHtml = "<font color='#ff0000'><b>Attenzione! si è verificato un errore nella lettura delle funzioni associate!</b></font>";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <returns></returns>
		private XmlNode LoadAmmXml(XmlDocument xmlDoc)
		{
			XmlNode nodoAmm = null;

			try
			{
				//cerca il nodo dell'amministrazione corrente nell'xml
				string pathCodice = "//DOCSPA/AMMINISTRAZIONE/DATI[CODICE='" + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"0") +"']";
				nodoAmm  = xmlDoc.SelectSingleNode(pathCodice);
				nodoAmm = nodoAmm.ParentNode;				
			}
			catch
			{
				textArea.InnerHtml = "<font color='#ff0000'><b>Attenzione! si è verificato un errore nella lettura delle funzioni associate!</b></font>";
			}

			return nodoAmm;
		}

		/// <summary>
		/// visualizza le funzioni associate al ruolo
		/// </summary>
		public void VisualPermessi()
		{
			try
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(xmldocpath);
				XmlNode nodoAmm = LoadAmmXml(xmlDoc);			

				string outputHtml = "";

				// punta all'organigramma
				XmlNode nodoUO = nodoAmm.SelectSingleNode("ORGANIGRAMMA");
				
				// prende il ruolo in uo passato
				XmlNode nodeRuoloInUO = nodoUO.SelectSingleNode(".//RUOLO/DATI[CODICE='" + cod_ruolo + "']");
				desc_ruolo = nodeRuoloInUO.SelectSingleNode("DESCRIZIONE").InnerText;
			
				outputHtml = "<table border='0' cellpadding='0' cellspacing='2' width='100%'>";
				outputHtml += "<tr><td colspan='3' class='titolo' align='center'>Funzioni associate a: " + cod_ruolo + " - " + desc_ruolo + "</td></tr>";				

				if (nodeRuoloInUO!=null)
				{									
					// prende i tipifunzione per questo ruolo in uo									
					XmlNode nodoTipiFunzione = nodeRuoloInUO.ParentNode.SelectSingleNode("TIPIFUNZIONE");
											
					if (nodoTipiFunzione.HasChildNodes)
					{
						//foreach(XmlNode nodoCodice in nodiTipiFunzione)
						foreach(XmlNode nodoCodice in nodoTipiFunzione.ChildNodes)
						{				
							// prende il tipofunzione sull'anagrafica
							XmlNode nodoFunzione = xmlDoc.SelectSingleNode(@"//TIPIFUNZIONE/TIPO/DATI[CODICE='" + nodoCodice.InnerText.ToUpper() + "']");
						
							outputHtml += "<tr><td colspan='3'>&nbsp;</td></tr>";					
							outputHtml += "<tr><td colspan='3' class='testo_grigio_scuro'><img src='../Images/funzioni.gif' border='0'> " + nodoFunzione.SelectSingleNode("DESCRIZIONE").InnerText + " - " + nodoCodice.InnerText.ToUpper() + "</td></tr>";
							outputHtml += "<tr bgcolor='#810d06'><td class='testo_bianco' align='center'>Descrizione</td><td class='testo_bianco' align='center'>Codice</td><td></td></tr>";

							nodoFunzione = nodoFunzione.ParentNode;
							XmlNodeList tipiFunz = nodoFunzione.SelectNodes("FUNZIONI/FUNZIONE");
							if (tipiFunz.Count > 0)
							{
								foreach(XmlNode nodoTF in tipiFunz)
								{
									// prende la funzione elementare
									XmlNode nodoFunzElem = xmlDoc.SelectSingleNode(@"//FUNZIONIELEMENTARI/FUNZIONEELEMENTARE[CODICE='" + nodoTF.SelectSingleNode("CODICE").InnerText + "']");
									if (nodoFunzElem!=null)
									{
										outputHtml += "<tr><td class='testo' bgcolor='#e0e0e0'>" + nodoFunzElem.SelectSingleNode("DESCRIZIONE").InnerText + "</td><td class='testo' bgcolor='#ffefd5'>" + nodoFunzElem.SelectSingleNode("CODICE").InnerText + "</td><td bgcolor='#e0e0e0'><a href='javascript:void(0)' onclick='javascript:trova(\"" + nodoFunzElem.SelectSingleNode("CODICE").InnerText + "\")' title='Trova lo stesso codice in altre funzioni...'><img src='../Images/lentina.gif' border='0'></a></td></tr>";
									}
								}
							}
						}
					}
					else
					{
						outputHtml += "<tr><td colspan='3' class=testo_grigio_scuro><br>Non sono stati ancora associati tipi funzione!</td></tr>";
						//btn_find.Visible = false;
					}										
				}
			
				outputHtml += "</table>";
				textArea.InnerHtml = outputHtml;
			}
			catch
			{
				textArea.InnerHtml = "<font color='#ff0000'><b>Attenzione! si è verificato un errore nella lettura delle funzioni associate!</b></font>";
				//btn_find.Visible = false;
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
