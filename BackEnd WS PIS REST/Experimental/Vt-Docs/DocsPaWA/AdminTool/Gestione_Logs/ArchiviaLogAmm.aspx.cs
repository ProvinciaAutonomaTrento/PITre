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
using System.IO;
using System.Xml;
using Microsoft.Web.UI.WebControls;

namespace Amministrazione.Gestione_Logs
{
	/// <summary>
	/// Summary description for ArchiviaLog.
	/// </summary>
	public class ArchiviaLogAmm : System.Web.UI.Page
	{
		#region WebControls e variabili
		protected System.Web.UI.WebControls.Label lbl_tit;
		protected System.Web.UI.WebControls.Button btn_cerca;
        protected System.Web.UI.WebControls.Button modificaNum;
        protected System.Web.UI.WebControls.Label lbl_position;
		protected System.Web.UI.WebControls.Button btn_archivia;
		protected System.Web.UI.WebControls.Label lbl_archivio;
		protected System.Web.UI.WebControls.Label lbl_avviso;
		protected System.Web.UI.WebControls.Label lbl_lista_log;
        protected System.Web.UI.WebControls.HiddenField permesso;

        protected System.Web.UI.WebControls.CheckBox chk_avviso;
        protected System.Web.UI.WebControls.TextBox txt_num_rec;
        protected string idAmm;
        //-------------------------------------------------------------------------
		public string PathLog = string.Empty;
		protected DataSet dataSet;
		#endregion

		#region Page_Load
		private void Page_Load(object sender, System.EventArgs e)
		{
			//----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
			if(Session.IsNewSession)
			{
				Response.Redirect("../Exit.aspx?FROM=EXPIRED");
			}
            this.idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
			
			AmmUtils.WebServiceLink  ws=new AmmUtils.WebServiceLink();
			if(!ws.CheckSession(Session.SessionID))
			{
				Response.Redirect("../Exit.aspx?FROM=ABORT");
			}
			// ---------------------------------------------------------------

			if (!IsPostBack)
			{				
				this.lbl_position.Text	= "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"1");				
				
                //if(this.VerificaArchivioLogPathAmm())
                //{
					//this.lbl_tit.Text = "Archivia in: " + this.PathLog;
					this.ContaArchivio();					
                //}
                //else
                //{
                //    //this.lbl_tit.Text = "";
                //    this.lbl_archivio.Text = "ATTENZIONE!<br>non esiste una directory dove archiviare i file .pdf<br><br>Verificare la chiave ARCHIVIO_LOG_PATH_AMM sul web.config del WS.<br><br>";
                //    this.lbl_avviso.Text = "";
                //    this.btn_archivia.Visible=false;
                //}
			}
        }

        private void ArchiviaLogAmm_PreRender(object sender, System.EventArgs e)
        {
            this.findRecords(this.idAmm);
        }

        #endregion

        //private bool VerificaArchivioLogPathAmm()
        //{
        //    bool retValue = false;
        //    AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
        //    string result = ws.VerificaArchivioLogPathAmm();

        //    if(result!=null && result!=string.Empty)
        //    {
        //        this.PathLog=result;
        //        retValue = true;
        //    }

        //    return retValue;
        //}

        private void findRecords(string idAmm)
        {
            string verificarecord = DocsPAWA.FileManager.findRecords(idAmm);
            if (verificarecord != null && verificarecord != "")
            {
                string[] a = verificarecord.Split('^');
                if (!a[0].Equals('0'))
                {
                    this.chk_avviso.Checked = true;
                    this.txt_num_rec.Text = a[1];
                }
                else
                {
                    this.chk_avviso.Checked = false;
                    this.txt_num_rec.Text = "";
                }
            }
        }
        
        #region conta archivio
		/// <summary>
		/// conta quanti record ci sono sulla tabella dei log
		/// </summary>
		public void ContaArchivio()
		{
			string result = "";

			// verifica il numero di record di log esistono sul db
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			result = ws.ContaArchivio(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"0"), "Amministrazione");
			if(result.Length != 0)
			{
				int valore = Int32.Parse(result);
			
				this.lbl_archivio.Text	= "Numero record attuali da archiviare:&nbsp;&nbsp;&nbsp;<font color='#ff0000'>" + valore + "</font>";			
				this.lbl_avviso.Text		= "L'operazione potrebbe richiedere qualche minuto";

				//FilesList();  //NON UTILIZZATO AL MOMENTO!
			}
			else
			{
				this.lbl_archivio.Text		= "<font color='#ff0000'>Nessun record dei log da archiviare!</font>";
				this.btn_archivia.Visible	= false;
				this.lbl_avviso.Visible		= false;
                //this.ddl_exportType.Visible = false;
			}
		}
		#endregion

        private void modificaNum_Click(object sender, System.EventArgs e)
        {
            if (this.permesso.Value == "ok")
            {
                if (this.chk_avviso.Checked)
                {
                    if (Convert.ToInt16(this.txt_num_rec.Text) > 0)
                    {
                        if (!DocsPAWA.FileManager.modifyRecordNumber(idAmm, '1', Convert.ToInt16(this.txt_num_rec.Text)))
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "modificaErrata", "alert('Modifica non effettuata correttamente.');", true);
                        }
                    }
                }
                else
                {
                    if (!DocsPAWA.FileManager.modifyRecordNumber(idAmm, '0', 0))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "modificaErrata", "alert('Modifica non effettuata correttamente.');", true);
                    }
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
			this.btn_archivia.Click += new System.EventHandler(this.btn_archivia_Click);
            this.modificaNum.Click += new EventHandler(modificaNum_Click);
            this.PreRender += new EventHandler(ArchiviaLogAmm_PreRender);
            this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region tasto archivia
		/// <summary>
		/// tasto archivia log
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_archivia_Click(object sender, System.EventArgs e)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			bool esito = ws.stampaLog(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"0"), "Amministrazione");
			if(esito)
			{			
				this.lbl_archivio.Text		= "Archiviazione effettuata con successo!";
				this.btn_archivia.Visible	= false;
				this.lbl_avviso.Visible		= false;
                //this.ddl_exportType.Visible = false;
                //FilesList();
			}
			else
			{
				this.lbl_archivio.Text		= "<font color='#ff0000'>ATTENZIONE! si è verificato un errore durante l'archiviazione dei log!</font>";
				this.btn_archivia.Visible	= false;
				this.lbl_avviso.Visible		= false;
                //this.ddl_exportType.Visible = false;
            }
		}
		#endregion

		#region elenco dei file di log su file system (NON UTILIZZATO AL MOMENTO!)
	
		/// <summary>
		/// visualizza l'elenco dei file di log su file system
		/// </summary>
		public void FilesList()
		{
			string[] files;			

			lbl_lista_log.Text = "<br><b>Lista files di log archiviati:</b><br><br>";

			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			files = ws.ListaFilesLog(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"0"));
			if(files != null || files.Length > 0)
			{
				foreach(string f in files)
				{
					int indF = f.IndexOf(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"0"));
					string f2 = f.Remove(0,indF);					

					lbl_lista_log.Text += "<img src='../Images/pdf.jpg' border='0'>&nbsp;" + f2 + "<br>";
				}
			}
		}

		#endregion
	}
}
