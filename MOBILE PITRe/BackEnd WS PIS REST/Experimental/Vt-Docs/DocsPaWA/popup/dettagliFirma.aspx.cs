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
	/// Summary description for dettagliFirma.
	/// </summary>
    public class dettagliFirma : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label Label1;
		protected DocsPAWA.dataSet.DataSetFirmatari dataSetFirmatari1;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
	
		DocsPaWR.SchedaDocumento schedaDocumento;
		protected System.Web.UI.WebControls.Label lb_dettagli;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected ArrayList Dt_elem;

		
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			DocsPaWR.Firmatario firmatario;
			schedaDocumento =(DocsPAWA.DocsPaWR.SchedaDocumento)DocumentManager.getDocumentoSelezionato(this) ;
			
			if (schedaDocumento == null)
			{
				this.lb_dettagli.Text = "Errore nel reperimento dei dati del documento";
				this.lb_dettagli.Visible = true;
				return;
			}
			if(!Page.IsPostBack)
			{
				Dt_elem=new ArrayList();
				string codInfo = Request.QueryString["codInfo"];
				if ((codInfo == null) || (codInfo == ""))
				{
					this.lb_dettagli.Text = "Errore nel reperimento dei dati del documento";
					this.lb_dettagli.Visible = true;
					return;
				}

				int int_codInfo = Int32.Parse(codInfo);
				if (schedaDocumento.documenti[int_codInfo].firmatari.Length > 0)
				{
					for (int i=0; i < schedaDocumento.documenti[int_codInfo].firmatari.Length; i++)
					{
						firmatario = schedaDocumento.documenti[int_codInfo].firmatari[i];
						this.dataSetFirmatari1.element1.Addelement1Row(firmatario.nome, firmatario.cognome, firmatario.codiceFiscale, firmatario.dataNascita);
					}	
					Session["Dg_firmatari"]=this.dataSetFirmatari1.Tables[0];
					this.DataGrid1.DataSource=this.dataSetFirmatari1.Tables[0];
					this.DataGrid1.DataBind();
				}
				else
				{
					this.lb_dettagli.Text = "Documento non firmato";
					this.lb_dettagli.Visible = true;
					return;
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
			this.dataSetFirmatari1 = new DocsPAWA.dataSet.DataSetFirmatari();
			((System.ComponentModel.ISupportInitialize)(this.dataSetFirmatari1)).BeginInit();
			// 
			// dataSetFirmatari1
			// 
			this.dataSetFirmatari1.DataSetName = "DataSetFirmatari";
			this.dataSetFirmatari1.Locale = new System.Globalization.CultureInfo("en-US");
			this.dataSetFirmatari1.Namespace = "http://tempuri.org/DataSetFirmatari.xsd";
			this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.Datagrid1_pager);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataSetFirmatari1)).EndInit();

		}
		#endregion


		#region DataGrid

		private void Datagrid1_pager(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			this.DataGrid1.CurrentPageIndex=e.NewPageIndex;
			DataTable TDNew=(DataTable) Session["Dg_firmatari"];			
			DataGrid1.DataSource=TDNew; 						
			DataGrid1.DataBind();
		}

		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			Session.Remove("Dg_firmatari");
			Response.Write("<script>window.close();</script>");	
		}


		public class Cols 
		{		
			private string nome;
			private string cognome;
			private string codiceFiscale;
			private string dataNascita;

		
			public Cols(string nome, string cognome, string codiceFiscale, string dataNascita)
			{
				this.nome=nome;
				this.cognome=cognome;
				this.codiceFiscale=codiceFiscale;
				this.dataNascita= dataNascita;
			}			
			public string Nome{get{return nome;}}
			public string Cognome{get{return cognome;}}
			public string CodiceFiscale{get{return codiceFiscale;}}
			public string DataNascita{get{return dataNascita;}}	
		}


		#endregion

	}
}
