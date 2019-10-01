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
using System.Web.Security;
using System.Xml;
using System.IO;

namespace Amministrazione.Gestione_Login
{
	/// <summary>
	/// Summary description for CheckXml.
	/// </summary>
	public class CheckXml : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lbl_msgAmm;
		protected System.Web.UI.WebControls.Button btn_si;
		protected System.Web.UI.WebControls.Label lbl_error;
		protected System.Web.UI.WebControls.Button btn_no;
		//----------------------------------------------------------------------------------------------------
		string xmldocAmmpath = System.Configuration.ConfigurationManager.AppSettings["FILEAMMINISTRAZIONI"];
		string xmldocTitpath = System.Configuration.ConfigurationManager.AppSettings["FILETITOLARIO"];	
		string xmldocSecpath = System.Configuration.ConfigurationManager.AppSettings["FILESECURITY"];

		/// <summary>
		/// Page_Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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

			btn_si.Attributes.Add("onclick","if (!window.confirm('Attenzione, i files presenti saranno sovrascritti.\\n\\nSei sicuro di voler procedere?')) {return false};");
			

			if(!IsPostBack)
			{
				// verifica sul file system la presenza di entrambi i file (devono esserci entrambi o NESSUNO dei due!)
//				if (((File.Exists(xmldocAmmpath)) && (!File.Exists(xmldocTitpath))) || ((!File.Exists(xmldocAmmpath)) && (File.Exists(xmldocTitpath))) )
//				{
//					if (File.Exists(xmldocAmmpath))
//					{
//						// rimuove il file Amministrazioni.xml dalla path
//						RimuoviXML(xmldocAmmpath);
//					}
//					if (File.Exists(xmldocTitpath))
//					{
//						// rimuove il file Titolario.xml dalla path
//						RimuoviXML(xmldocTitpath);
//					}
//				}
				
				if(!PresenzaXML())
				{
					SetFocus(btn_si);
					lbl_error.Text="";	
					lbl_msgAmm.Text = "Attenzione! esistono files XML non ancora inviati...";
				}
				else
				{
					// redirect su Home...
					// Response.Redirect("Home.aspx");
					Response.Write("<script>; var popup = window.open('../Gestione_Homepage/Home.aspx','Home',"+
						"'fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes,scrollbars=yes');"+
						"popup.moveTo(0,0); popup.resizeTo(screen.availWidth,screen.availHeight);"+
						" if(popup!=self) {window.opener=null;self.close();}"+
						"</script>");
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ctrl"></param>
		private void SetFocus(System.Web.UI.Control ctrl)
		{
			string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
			RegisterStartupScript("focus", s);
		}	

		/// <summary>
		/// PresenzaXML
		/// </summary>
		/// <returns>True, False</returns>
		public bool PresenzaXML()
		{
			bool result = false; 
			bool esiste = false;
			
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			XmlDocument doc;

			//gestione del file Amministrazioni
			if (!File.Exists(xmldocAmmpath))
			{				
				// download del file
				doc = ws.DownloadAmministrazione();

				if(doc!=null)
				{
					doc.Save(System.Configuration.ConfigurationManager.AppSettings["FILEAMMINISTRAZIONI"]);
					esiste = true;
				}
			} 			

			//gestione del file Titolario
//			if (!File.Exists(xmldocTitpath))
//			{
//				// download del file
//				doc=ws.DownloadTitolario();
//				if(doc!=null)
//				{
//					doc.Save(System.Configuration.ConfigurationManager.AppSettings["FILETITOLARIO"]);
//
//					// inserisce la userid e la pwd dell'utente nel file xml
//					foreach(XmlNode titolario in doc.SelectNodes("//TITOLARIO"))
//					{
//						//XmlNode nodoDati = doc.SelectSingleNode("//TITOLARIO/DATI");
//						XmlNode nodoDati = titolario.SelectSingleNode("DATI");
//						XmlElement nodoUser;
//						XmlElement nodoPwd;
//						nodoUser = doc.CreateElement("UTENTE");
//						nodoUser.InnerText = (string)Session["UserIdAdmin"];
//						nodoDati.AppendChild(nodoUser);
//						nodoPwd = doc.CreateElement("PASSWORD");
//						nodoPwd.InnerText = (string)Session["PwdAdmin"];
//						nodoDati.AppendChild(nodoPwd);
//					}
//					doc.Save(System.Configuration.ConfigurationManager.AppSettings["FILETITOLARIO"]);
//					esiste = true;
//				}
//			}
			
			#region GADAMO: ipotesi di gestione diversa del file XML del titolario
//			//gestione del file Security
//			if (!File.Exists(xmldocSecpath))
//			{
//				// download del file
//				doc = ws.DownloadSecurity();
//				if(doc!=null)
//				{
//					doc.Save(System.Configuration.ConfigurationManager.AppSettings["FILESECURITY"]);				
//					esiste = true;
//				}
//			}
			#endregion

			if (esiste)
			{
				result = true;						
			}

			return result;
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
			this.btn_si.Click += new System.EventHandler(this.btn_si_Click);
			this.btn_no.Click += new System.EventHandler(this.btn_no_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_si_Click(object sender, System.EventArgs e)
		{
			bool esito = false;
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			XmlDocument doc;
			
			// download del file amministrazioni
			doc = ws.DownloadAmministrazione();

			if(doc!=null)
			{
				doc.Save(System.Configuration.ConfigurationManager.AppSettings["FILEAMMINISTRAZIONI"]);
				esito = true;
			}
			 
			// download del file titolario
//			doc=ws.DownloadTitolario();
//			if(doc!=null)
//			{
//				doc.Save(System.Configuration.ConfigurationManager.AppSettings["FILETITOLARIO"]);
//
//				// inserisce la userid e la pwd dell'utente nel file xml
//				foreach(XmlNode titolario in doc.SelectNodes("//TITOLARIO"))
//				{
//					XmlNode nodoDati = titolario.SelectSingleNode("DATI");
//					//XmlNode nodoDati = doc.SelectSingleNode("//TITOLARIO/DATI");
//					XmlElement nodoUser;
//					XmlElement nodoPwd;
//					nodoUser = doc.CreateElement("UTENTE");
//					nodoUser.InnerText = (string)Session["UserIdAdmin"];;
//					nodoDati.AppendChild(nodoUser);
//					nodoPwd = doc.CreateElement("PASSWORD");
//					nodoPwd.InnerText = (string)Session["PwdAdmin"];;
//					nodoDati.AppendChild(nodoPwd);
//				}
//				doc.Save(System.Configuration.ConfigurationManager.AppSettings["FILETITOLARIO"]);
//				esito = true;
//			}

			if(esito)
			{
				// redirect su Home...
				// Response.Redirect("Home.aspx");
				Response.Write("<script>; var popup = window.open('../Gestione_Homepage/Home.aspx','Home',"+
					"'fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes,scrollbars=yes');"+
					"popup.moveTo(0,0); popup.resizeTo(screen.availWidth,screen.availHeight);"+
					" if(popup!=self) {window.opener=null;self.close();}"+
					"</script>");
			}
			else
			{
				// si è verificato un errore!!!!!!!
				btn_si.Visible=false;
				btn_no.Visible=false;
				lbl_msgAmm.Text="Attenzione! si è verificato un errore nel caricamento dei file!";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_no_Click(object sender, System.EventArgs e)
		{
			// redirect su Home...
			// Response.Redirect("Home.aspx");
			Response.Write("<script>; var popup = window.open('../Gestione_Homepage/Home.aspx','Home',"+
			"'fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes,scrollbars=yes');"+
			"popup.moveTo(0,0); popup.resizeTo(screen.availWidth,screen.availHeight);"+
			" if(popup!=self) {window.opener=null;self.close();}"+
			"</script>");
		}

		/// <summary>
		/// rimuove il file xml eventualmente rimasto sul file system (orfano)
		/// </summary>
		/// <param name="xmldocpath"></param>
		private void RimuoviXML(string xmldocpath)
		{			
			string newDirectory = Path.GetDirectoryName(xmldocpath) + "\\BACKUP_XML";
				
			Directory.CreateDirectory(newDirectory);

			string pathNewFile = newDirectory + "\\" + Path.GetFileName(xmldocpath) + "_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
			
			Directory.Move(xmldocpath,pathNewFile);
		}
	}
}
