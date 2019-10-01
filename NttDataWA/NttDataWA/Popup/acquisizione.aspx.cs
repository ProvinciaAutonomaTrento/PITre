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
using NttDataWA.Utils;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
	/// <summary>
	/// Summary description for acquisizione.
	/// </summary>
	public class acquisizione : System.Web.UI.Page
	{
        public static string fullpath = "";
		protected System.Web.UI.WebControls.Image Image1;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
            try {
                fullpath = utils.getHttpFullPath();
                this.Response.Expires = -1;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
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

        /// <summary>
        /// Reperimento della segnatura di protocollo, necessaria per la stampa diretta su foglio tramite scanner REI
        /// </summary>
        protected string SegnaturaProtocollo
        {
            get
            {
                if (this.StampaSegnaturaAbilitata)
                {
                    DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getSelectedRecord();

                    if (schedaDocumento != null && schedaDocumento.protocollo != null)
                        return schedaDocumento.protocollo.segnatura;
                    else
                        return string.Empty;
                }
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Indica se la stampa della seguatura di protocollo su scanner REI è abilitata o meno
        /// </summary>
        protected bool StampaSegnaturaAbilitata
        {
            get
            {
                DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getSelectedRecord();

                if (schedaDocumento.protocollo == null)
                    return false;
                else if (schedaDocumento.documentoPrincipale != null)
                    // Acquisizione dal dettaglio dell'allegato
                    return false;
                else
                {
                    // Acquisizione dal tab "Allegati" del documento principale
                    DocsPaWR.FileRequest fileRequest = FileManager.getSelectedFile();

                    // Consente la stampa della segnatura solamente se il documento non è un allegato
                    return ((fileRequest as DocsPaWR.Allegato) == null);
                }
            }
        }

        protected int IsEnabledAutomaticScan
        {
            get
            {
                Int32 automaticScan = 0;
                Int32.TryParse(
                    InitConfigurationKeys.GetValue("0", "FE_AUTOMATIC_SCAN"),
                    out automaticScan);

                return automaticScan;
            }
        }

	
	}
}
