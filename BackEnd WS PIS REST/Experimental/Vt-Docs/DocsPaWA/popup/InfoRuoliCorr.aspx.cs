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
	/// Summary description for InfoRuoliCorr.
	/// </summary>
    public class InfoRuoliCorr : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			try
			{
				string codCor = Request.QueryString["codCor"];
				if ((codCor == null) || (codCor == ""))
					return;		
				getRuoloCor(codCor);
			}
			catch (System.Exception ex)
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void getRuoloCor(string codiceRubrica)
		{
			try
			{
				DataSet ds = UserManager.GetRuoliUtenteInt(this,codiceRubrica);
				bindDatagrid(ds);
				foreach(DataGridItem _di in DataGrid1.Items)
				{
//					string cha_pref=((Label)this.DataGrid1.SelectedItem.Cells[3].Controls[1]).Text;
					string cha_pref	=_di.Cells[3].Text;
					if(cha_pref.Equals("1"))
					{
						_di.FindControl("ImageButton1").Visible=true;	
					
					}
					else
					{
						_di.FindControl("ImageButton1").Visible=false;	
					}
				}

			}
			catch(Exception ex)
			{
				ErrorManager.redirect(this, ex);
			}
		
		}

		private void bindDatagrid(DataSet ds)
		{

			this.DataGrid1.DataSource=ds;
			this.DataGrid1.DataBind();
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}
	}
}
