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

namespace DocsPAWA.AdminTool.Gestione_Logs
{			
	/// <summary>
	/// Gestione dell'export dei risultati delle ricerche per Documenti, Fascicoli, Trasmissioni.
	/// </summary>
	public class exportLog : System.Web.UI.Page
	{
		#region WebControls e variabili
		protected System.Web.UI.WebControls.RadioButton rd_formatoPDF;
		protected System.Web.UI.WebControls.Button btn_export;		
		protected System.Web.UI.WebControls.RadioButton rd_formatoHTML;
		protected System.Web.UI.WebControls.RadioButton rd_formatoXLS;
		protected System.Web.UI.WebControls.Button btn_annulla;
		protected System.Web.UI.WebControls.TextBox txt_titolo;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_export;
        protected string codAmm;
		
		#endregion

		#region Inizializzazione Pagina e variabili
		/// <summary>
		/// Page Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack)
			{
				this.btn_annulla.Attributes.Add("onclick","window.close();");
                this.hd_export.Value = Request.QueryString["export"];
            }
            
            this.codAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
       
        }
		#endregion
		
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
			this.btn_export.Click += new System.EventHandler(this.btn_export_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_export_Click(object sender, System.EventArgs e)
		{
			string soggetto = this.hd_export.Value;
			string tipologia = string.Empty;
            string titolo = string.Empty;
            if (!string.IsNullOrEmpty(this.txt_titolo.Text))
                titolo = this.txt_titolo.Text;
            else
                titolo = "exportLog";
            DocsPaWR.FileDocumento file = new DocsPAWA.DocsPaWR.FileDocumento();
            
            // Reperimento dalla sessione del contesto di ricerca fulltext
            //DocsPAWA.DocsPaWR.FullTextSearchContext context = Session["FULL_TEXT_CONTEXT"] as DocsPAWA.DocsPaWR.FullTextSearchContext;
 

            if(this.rd_formatoPDF.Checked)
				tipologia = "PDF";
			else if(this.rd_formatoXLS.Checked)
				tipologia = "XLS";

			try
			{
                string user = string.Empty;
                string data_da = null;
                string data_a = null;
                string oggetto = string.Empty;
                string azione = string.Empty;
                string esito = string.Empty;
                int tabelle = 0;

                Hashtable filtri = (Hashtable) Session["listaFiltriLog"];
                // Filtro user
                if (filtri.ContainsKey("DATA_DA"))
                    data_da = (string)filtri["DATA_DA"];
                if (filtri.ContainsKey("DATA_A"))
                    data_a = (string)filtri["DATA_A"];
                if (filtri.ContainsKey("USER"))
                    user = (string)filtri["USER"];
                if (filtri.ContainsKey("OGGETTO"))
                    oggetto = (string)filtri["OGGETTO"];
                if (filtri.ContainsKey("AZIONE"))
                    azione = (string)filtri["AZIONE"];
                if (filtri.ContainsKey("ESITO"))
                    esito = (string)filtri["ESITO"];
                if (filtri.ContainsKey("TABELLE"))
                    tabelle = (int)filtri["TABELLE"];

                exportLogManager manager = new exportLogManager(soggetto, tipologia, titolo, this.codAmm, user, data_a, data_da, oggetto, azione, esito, tabelle);
				file = manager.Export();

				if(file.content!=null && file.content.Length>0)
					this.executeJS("<SCRIPT>OpenFile('"+tipologia+"', '" + titolo + "." + tipologia + "');</SCRIPT>");
				else
					this.executeJS("<SCRIPT>alert('Impossibile generare il file " + tipologia + "');</SCRIPT>");
			}
			catch(Exception ex)
			{
				this.executeJS("<SCRIPT>alert('Errore di sistema: " + ex.Message.Replace("'","\\'") + "');</SCRIPT>");
			}
		}

		/// <summary>
		/// Esegue JS
		/// </summary>
		/// <param name="key"></param>
		private void executeJS(string key)
		{
			if(!this.Page.IsStartupScriptRegistered("theJS"))			
				this.Page.RegisterStartupScript("theJS", key);
		}
	}
}
