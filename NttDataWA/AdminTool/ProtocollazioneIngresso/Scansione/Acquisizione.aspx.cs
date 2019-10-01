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
using SAAdminTool.utils;

namespace ProtocollazioneIngresso.Scansione
{
	/// <summary>
	/// Summary description for Acquisizione.
	/// </summary>
	public class Acquisizione : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
            this.Response.Expires = -1;
		}

		public string GetMaskTitle()
		{
			string maskTitle=string.Empty;
			string documentoPrincipale=Request.QueryString["DocumentoPrincipale"];

			if (documentoPrincipale!=null && documentoPrincipale!=string.Empty)
			{
				if (documentoPrincipale.ToUpper()=="TRUE")
					maskTitle="Acquisizione documento";
				else
					maskTitle="Acquisizione allegato";
			}

			return maskTitle;
		}

		/// <summary>
		/// Registrazione script client
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if(!this.Page.IsStartupScriptRegistered(scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
				this.Page.RegisterStartupScript(scriptKey, scriptString);
			}
		}

        /// <summary>
        /// Reperimento della segnatura di protocollo, necessaria per la stampa diretta su foglio tramite scanner REI
        /// </summary>
        protected string SegnaturaProtocollo
        {
            get
            {
                if (this.StampaSegnaturaAbilitata)
                {
                    ProtocollazioneIngresso.Protocollo.ProtocolloMng mng = new ProtocollazioneIngresso.Protocollo.ProtocolloMng(this);

                    SAAdminTool.DocsPaWR.SchedaDocumento schedaDocumento = mng.GetDocumentoCorrente();
                    
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
                ProtocollazioneIngresso.Protocollo.ProtocolloMng mng = new ProtocollazioneIngresso.Protocollo.ProtocolloMng(this);

                SAAdminTool.DocsPaWR.SchedaDocumento schedaDocumento = mng.GetDocumentoCorrente();

                if (schedaDocumento.protocollo == null)
                    return false;
                else if (schedaDocumento.documentoPrincipale != null)
                    // Acquisizione dal dettaglio dell'allegato
                    return false;
                else
                {
                    // Consente la stampa della segnatura solamente se il documento non è un allegato
        			string documentoPrincipale=Request.QueryString["DocumentoPrincipale"];
                    if (!string.IsNullOrEmpty(documentoPrincipale))
                        return (documentoPrincipale.ToUpper() == "TRUE");
                    else
                        return false;
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
