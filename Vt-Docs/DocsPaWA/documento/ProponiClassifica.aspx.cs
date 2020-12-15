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
namespace DocsPAWA.documento
{
	/// <summary>
	/// Summary description for ProponiClassifica.
	/// </summary>
	public class ProponiClassifica : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		protected System.Web.UI.WebControls.Label lb;
		protected System.Web.UI.WebControls.Button btn_chiudi;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			lb.Visible=false; 
			
			if(!IsPostBack)
			{
				DataSet dsXml = new DataSet();
				System.IO.StringReader  stream;

				if((string )Session["classificaXML"] != null )
				{
					stream = new System.IO.StringReader((string )Session["classificaXML"]);
					dsXml.ReadXml(stream);
					try
					{
						DataGrid1.DataSource = dsXml.Tables[1] ;
						DataGrid1.DataBind();
					}
					catch(Exception ex)
					{
						
						string eccezione = ex.Message;
						DataGrid1.Visible = false ;
						lb.Visible = true ;
						lb.Text = "Nessun fascicolo trovato";
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
			this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			
			Response.Write("<script>window.close();</script>");	
		}

		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			Response.Write("<script>window.close();</script>");	
		}

		private void DataGrid1_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			try
			{
				for(int i=0;i<this.DataGrid1.Items.Count;i++)
				{
					if(this.DataGrid1.Items[i].ItemIndex>=0)
					{	
						switch(this.DataGrid1.Items[i].ItemType.ToString().Trim())
						{
							case "Item":
							{
								this.DataGrid1.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
								this.DataGrid1.Items[i].Attributes.Add("onmouseout","this.className='bg_grigioN'");
								break;
							}
							case "AlternatingItem":
					
							{
								this.DataGrid1.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
								this.DataGrid1.Items[i].Attributes.Add("onmouseout","this.className='bg_grigioA'");
								break;
							}
				
						}
					}
				}
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
			}
		}

		private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string classificazione = ((Label) this.DataGrid1.SelectedItem.Cells[0].Controls[1]).Text;

			DocsPaWR.DocsPaWebService DocsWS = new DocsPAWA.DocsPaWR.DocsPaWebService();
			//Session["classificazione"] = DocsWS.GetIdDK(classificazione).ToString();
//			String strScript ="" ;
//			strScript = "<script>window.opener.document.forms(0).txt_codClass.value = '" ;
//			strScript += classificazione ; 
//			strScript += "';window.opener.document.docClassifica.submit();self.close()" ;
//			strScript += "</" + "script>";
//			RegisterClientScriptBlock("anything", strScript);
			string response = "<script language='javascript'>top.principale.iFrame_sx.document.location='tabgestionedoc.aspx?tab=classifica'";
			response+="</script>";
			//?Cod_classifica=classificazione+
			Session["classificazione"] = classificazione ;
			Response.Write(response);
			
			
//			<script language='javascript'>top.principale.iFrame_sx.document.location='../documento/tabgestionedoc.aspx?tab=classifica';
//			</script>

			
			
		}


		private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			
		}



	}
}
