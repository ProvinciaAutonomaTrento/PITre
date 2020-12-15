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

namespace Amministrazione.Gestione_Logs
{
	/// <summary>
	/// Summary description for GestLogs.
	/// </summary>
	public class GestLogs : System.Web.UI.Page
	{
		#region WebControls e variabili
		protected System.Web.UI.WebControls.Label lbl_position;
		protected System.Web.UI.WebControls.Label lbl_archivio;
		protected System.Web.UI.WebControls.Panel pnl_archivio;
		protected System.Web.UI.WebControls.Label lbl_seleziona;
        protected string valoreMaxS;
        protected string idAmm;
		string xmldocpath = System.Configuration.ConfigurationManager.AppSettings["FILEAMMINISTRAZIONI"];	
		#endregion

		#region Page Load
		/// <summary>
		/// Page_Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
            Session["AdminBookmark"] = "GestioneLog";
            
            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
			if(Session.IsNewSession)
			{
				Response.Redirect("../Exit.aspx?FROM=EXPIRED");
			}
            
            this.idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            this.findMax(this.idAmm);


            AmmUtils.WebServiceLink  ws=new AmmUtils.WebServiceLink();
			if(!ws.CheckSession(Session.SessionID))
			{
				Response.Redirect("../Exit.aspx?FROM=ABORT");
			}
			// ---------------------------------------------------------------

			if (!IsPostBack)
			{
				lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"1");
				lbl_seleziona.Visible = true;
				ContaArchivio();				
			}
        }

		#endregion

		#region Conta Archivio
		/// <summary>
		/// conta quanti record ci sono sulla tabella dei log
		/// </summary>
		public void ContaArchivio()
		{
			string result = "";

			// verifica il numero di record di log esistono sul db
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			result = ws.ContaArchivio(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"0"), "");
			if(result.Length != 0)
			{
				int valore = Int32.Parse(result);
                int valoreMax = 0;
                if (valoreMaxS != null && !valoreMaxS.Equals(""))
                // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    valoreMax = Convert.ToInt32(this.valoreMaxS); // è POSSIBILE SETTARE QUI UN VALORE PER FAR VISUALIZZARE L'AVVISO
                // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


                if (valoreMax != 0)
                {
                    if (valore >= valoreMax)
                    {
                        pnl_archivio.Visible = true;
                        lbl_archivio.Text = "<font color='#ff0000'>AVVISO</font><br>Si consiglia di eseguire archiviazione dei log<br>tramite la voce di men&ugrave; <i>Archivia log</i>.<br><br>Numero record attuali: <font color='#ff0000'>" + valore + "</font><br><font color='#c0c0c0'>(limite di avviso: " + valoreMax + ")</font><br>";
                    }
                }
			}
		}
		#endregion		

        private void findMax(string idAmm)
        {
            string verificarecord = SAAdminTool.FileManager.findRecords(idAmm);
            if (verificarecord != null && verificarecord != "")
            {
                string[] a = verificarecord.Split('^');
                if (!a[0].Equals('0'))
                {
                    this.valoreMaxS = a[1];
                }
                else
                {
                    this.valoreMaxS = "0";
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
