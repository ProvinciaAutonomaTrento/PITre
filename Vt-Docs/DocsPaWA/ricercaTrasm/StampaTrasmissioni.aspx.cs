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

namespace DocsPAWA.ricercaTrasm
{
	/// <summary>
	/// Summary description for StampaTrasmissioni.
	/// </summary>
	public class StampaTrasmissioni : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lbStampa;
		protected System.Web.UI.WebControls.Label lbRuoloUtente;
		protected System.Web.UI.WebControls.DataGrid grigliaEffettuate;
		protected System.Web.UI.WebControls.DataGrid grigliaRicevute;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		protected System.Web.UI.WebControls.Label lbTipoOggetto;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if((string)Session["tiporic"]=="E")
			{
				#region commento
				/*
				DocsPaWR.Trasmissione[] tr = (DocsPAWA.DocsPaWR.Trasmissione[])Session["docTrasmissioni.trasmQueryEff"];
				foreach(DocsPAWA.DocsPaWR.Trasmissione trRow in tr)
				{
					DocsPaWR.TrasmissioneSingola[] ts = trRow.trasmissioniSingole;
					foreach(DocsPAWA.DocsPaWR.TrasmissioneSingola ts1 in ts)
					{
						
					}
					trRow.daAggiornare;
					trRow.dataInvio;
					trRow.infoDocumento;
					trRow.infoFascicolo;
					trRow.noteGenerali;
					trRow.ruolo.descrizione;
				}*/
				#endregion commento
				grigliaEffettuate.Visible = true;
				grigliaEffettuate.DataSource = TrasmManager.getDataTableEff(this) ;
				grigliaEffettuate.DataBind();
			}
			else
			{
				grigliaRicevute.Visible = true ;
				grigliaRicevute .DataSource = TrasmManager.getDataTableRic(this);
				grigliaRicevute .DataBind();
			
			}

			lbStampa.Text = "Stampa effettuata il giorno " + System.DateTime.Now.ToShortDateString();
			lbRuoloUtente.Text = UserManager.getRuolo(this).descrizione + "  " + UserManager.getUtente(this).descrizione;
			if((string)Session["Tipo_obj"]=="D")
			{
				lbTipoOggetto.Text = "Tipo oggetto trasmesso: Documento" ;
			}
			else
			{
				lbTipoOggetto.Text = "Tipo oggetto trasmesso: Fascicolo" ;
			}
			Response.Buffer = true;
			Response.ContentType = "text/html";//"application/vnd.ms-excel";
			//Response.Charset = "";
			//Response.End();
			
			/*repeater.DataSource = projectList;
			repeater.DataBind();*/
			

			#region codice per caricare da un template xls
			/*Response.AddHeader("content-disposition","attachment;filename=" + "tabella.xls" +"") ;
			DataSet dsXls = new DataSet();
			//strXmlFilePath=Server.MapPath(strXmlFilePath);
			dsXls.ReadXml(@"c:\tabella.xls");
			this.DataGrid1.DataSource=dsXls;
			this.DataGrid1.DataBind();*/
			#endregion

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
