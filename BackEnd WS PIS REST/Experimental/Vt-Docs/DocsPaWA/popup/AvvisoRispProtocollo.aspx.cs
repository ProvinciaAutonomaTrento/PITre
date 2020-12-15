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

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for AvvisoRispProtocollo.
	/// </summary>
    public class AvvisoRispProtocollo : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Image img_alert;
//		protected System.Web.UI.WebControls.Button btn_si;
		protected System.Web.UI.HtmlControls.HtmlTable tbl_alert;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.WebControls.Panel pnl_mitt;
		protected System.Web.UI.WebControls.Panel pnl_ogg;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.RadioButtonList rbl_scelta;
		protected System.Web.UI.WebControls.RadioButtonList rbl_scelta2;
        protected System.Web.UI.WebControls.RadioButtonList rbl_scelta3;
		protected System.Web.UI.WebControls.Panel pnl_rispNoProto;
		protected System.Web.UI.WebControls.Panel pnl_rispProto;
		protected System.Web.UI.WebControls.Button btn_ok;
//		protected System.Web.UI.WebControls.Button btn_no;
        protected System.Web.UI.WebControls.Panel pnl_occ;
        protected System.Web.UI.WebControls.Panel pnl_sovrascriviCorr;
        protected System.Web.UI.WebControls.Panel pnl_occ_proto;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
            this.Response.Expires = -1;
			
			if(!IsPostBack)
			{
				//gestione visualizzazione label di errore
				string req_corrisp = Request.QueryString["MITT"].ToString();
				string req_oggetto = Request.QueryString["OGG"].ToString();
				string req_isProto = Request.QueryString["isProto"].ToString();
                string req_diventaOccasionale = Request.QueryString["isOcc"].ToString();

				if(req_corrisp!=null && req_corrisp.Equals("True"))
				{
					this.pnl_mitt.Visible=false;
				}
				if(req_oggetto!=null && req_oggetto.Equals("True"))
				{
					this.pnl_ogg.Visible=false;
				}
				if(req_isProto!=null && req_isProto.Equals("True"))
				{
					//se la risposta al protocollo è PROTOCOLLATA
					this.pnl_rispProto.Visible=true;
					this.pnl_rispNoProto.Visible=false;
					this.rbl_scelta2.Items[0].Selected = true;
				}
				else
				{
					//se la risposta al protocollo NON E' PROTOCOLLATA
					this.pnl_rispProto.Visible=false;
					this.pnl_rispNoProto.Visible=true;
					this.rbl_scelta.Items[0].Selected = true;
				}

                if (req_diventaOccasionale != null && req_diventaOccasionale.Equals("True"))
                {
                    this.pnl_occ.Visible = false;
                    this.pnl_occ_proto.Visible = false;
                }
                else
                {
                    this.pnl_rispProto.Visible = false;
                    this.pnl_rispNoProto.Visible = false;
                    this.pnl_sovrascriviCorr.Visible = true; 

                    if (req_isProto != null && req_isProto.Equals("True"))
                    {
                        this.pnl_occ_proto.Visible = true;
                        this.pnl_occ.Visible = false;
                    }
                    else
                    {
                        this.pnl_occ_proto.Visible = false;
                        this.pnl_occ.Visible = true;      
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
			this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			/*  ----------------------------------------------------
			      Possibili valori di ritorno dell'avviso modale
			    ----------------------------------------------------
			  
			 	- Pannello pnl_rispNoProto -
			    Questo pannello viene mostrato nella pagina di avviso quando si sta rispondendo a un
			    protocollo in uscita con un protocollo in ingresso che ancora non è stato protocollato.
			  
			    Y : viene ritornato quando l'utente clicca sull' optione 'Continua utilizzando i dati immessi';
			    N : viene ritornato quando l'utente clicca sull' optione 'Continua e sovrascrivi i dati'
			    C : viene ritornato quando l'utente clicca sull' optione 'Seleziona un altro documento' 
			  
			    - Pannello pnl_rispProto -
				Questo pannello viene mostrato nella pagina di avviso quando si sta rispondendo a un
			    protocollo in uscita con un protocollo in ingresso protocollato in precedenza.
			  
			 	V : viene ritornato quando l'utente clicca sull' optione 'continua'
			 	C : viene ritornato quando l'utente clicca sull' optione 'Seleziona un altro documento' 		
			 */
			if (rbl_scelta.SelectedItem!=null && rbl_scelta.SelectedItem.Value!=String.Empty)
			{
				switch(rbl_scelta.SelectedItem.Value)
				{
					case "Y":
						if(!this.Page.IsStartupScriptRegistered("returnY"))
						{
							Page.RegisterStartupScript("returnY","<script>window.returnValue = 'Y'; window.close();</script>");
						}
						break;
					case "N":
						if(!this.Page.IsStartupScriptRegistered("returnN"))
						{
							Page.RegisterStartupScript("returnN","<script>window.returnValue = 'N'; window.close();</script>");
						}
						break;
					case "C":
						if(!this.Page.IsStartupScriptRegistered("returnC"))
						{
							Page.RegisterStartupScript("returnC","<script>window.returnValue = 'C'; window.close();</script>");
						}
						break;
//					case "S":
//						if(!this.Page.IsStartupScriptRegistered("returnV"))
//						{
//							Page.RegisterStartupScript("returnV","<script>window.returnValue = 'V'; window.close();</script>");
//						}
//						break;
				}
			}
			
			if (rbl_scelta2.SelectedItem!=null && rbl_scelta2.SelectedItem.Value!=String.Empty)
			{
				switch(rbl_scelta2.SelectedItem.Value)
				{
					case "C":
						if(!this.Page.IsStartupScriptRegistered("returnC"))
						{
							Page.RegisterStartupScript("returnC","<script>window.returnValue = 'C'; window.close();</script>");
						}
						break;
					case "S":
						if(!this.Page.IsStartupScriptRegistered("returnS"))
						{
							Page.RegisterStartupScript("returnV","<script>window.returnValue = 'S'; window.close();</script>");
						}
						break;
				}
			}

            if (rbl_scelta3.SelectedItem != null && rbl_scelta3.SelectedItem.Value != String.Empty)
            {
                switch (rbl_scelta3.SelectedItem.Value)
                {
                    case "Y":
                        if (!this.Page.IsStartupScriptRegistered("returnY"))
                        {
                            Page.RegisterStartupScript("returnY", "<script>window.returnValue = 'Y'; window.close();</script>");
                        }
                        break;
                    case "C":
                        if (!this.Page.IsStartupScriptRegistered("returnC"))
                        {
                            Page.RegisterStartupScript("returnC", "<script>window.returnValue = 'C'; window.close();</script>");
                        }
                        break;
                }
            }
		}

		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			if(!this.Page.IsStartupScriptRegistered("returnC"))
			{
				Page.RegisterStartupScript("returnC","<script>window.returnValue = 'C'; window.close();</script>");
			}
		}

		
		
	}
}
