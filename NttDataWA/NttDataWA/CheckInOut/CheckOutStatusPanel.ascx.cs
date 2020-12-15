namespace NttDataWA.CheckInOut
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using NttDataWA.DocsPaWR;

	/// <summary>
	///	UserControl contenente le informazioni di stato sul documento
	///	correntemente in CheckOut
	/// </summary>
	public class CheckOutStatusPanel : System.Web.UI.UserControl
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires=-1;

			if (!this.IsPostBack)
			{
				this.IDDocument=Request.QueryString["idDocument"];
				this.DocumentNumber=Request.QueryString["documentNumber"];

				this.Fetch();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// Creazione tabella informazioni di stato sul blocco del documento
		/// </summary>
		/// <param name="status"></param>
		private void DrawTable(CheckOutStatus status)
		{
			Table table=new Table();
		
			// Impostazione stili tabella
			table.Attributes.Add("align","center");
			table.Attributes.Add("border","0");
			table.CssClass="info";
			table.CellSpacing=0;
			table.CellPadding=5;
			table.Width=new Unit(400);

			table.Rows.Add(this.CreateHeaderTableRow());

			string userInfo=string.Empty;

            if (status.UserName != null && status.UserName != string.Empty)
            {
                var strUser = UIManager.AddressBookManager.getCorrispondenteByCodRubrica(status.UserName, true).descrizione;
                table.Rows.Add(CreateTableRow("Utente:", status.UserName + " [" + strUser + "]" ));
            }
			if (status.RoleName!=null && status.RoleName!=string.Empty)
				table.Rows.Add(CreateTableRow("Ruolo:",status.RoleName));

			table.Rows.Add(CreateTableRow("Data:",status.CheckOutDate.ToString()));
			
			if (status.DocumentLocation!=null && status.DocumentLocation!=string.Empty)
				table.Rows.Add(CreateTableRow("Percorso:",status.DocumentLocation));

			if (status.MachineName!=null && status.MachineName!=string.Empty)
				table.Rows.Add(CreateTableRow("Computer:",status.MachineName));

			this.Controls.Add(table);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private TableRow CreateHeaderTableRow()
		{
			TableRow row=new TableRow();
			
			TableCell cell=new TableCell();
			cell.CssClass="titolo_scheda";
			cell.ColumnSpan=2;
			cell.Attributes.Add("align","center");
			cell.Text="Informazioni sul documento bloccato";
			cell.ColumnSpan=2;
			row.Cells.Add(cell);

			return row;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="headerText"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		private TableRow CreateTableRow(string headerText,string text)
		{
			TableRow row=new TableRow();
			
			TableCell cellHeader=new TableCell();
			cellHeader.CssClass="titolo_scheda";
			cellHeader.Text=headerText;
			row.Cells.Add(cellHeader);

			TableCell cellText=new TableCell();
			cellText.CssClass="titolo_scheda";
			cellText.Text=text;
			row.Cells.Add(cellText);

			return row;
		}

		/// <summary>
		/// Caricamento dati relativamente allo stato del documento bloccato
		/// </summary>
		private void Fetch()
		{
            CheckOutStatus status = CheckInOutServices.GetCheckOutDocumentStatus();
            
			if (status!=null)
			{
				this.DrawTable(status);
			}
		}

		/// <summary>
		/// ID documento corrente
		/// </summary>
		private string IDDocument
		{
			get
			{
				if (this.ViewState["IDDocument"]!=null)
					return this.ViewState["IDDocument"].ToString();
				else
					return string.Empty;
			}
			set
			{
				this.ViewState["IDDocument"]=value;
			}
		}

		/// <summary>
		/// DocNumber documento corrente
		/// </summary>
		private string DocumentNumber
		{
			get
			{
				if (this.ViewState["DocumentNumber"]!=null)
					return this.ViewState["DocumentNumber"].ToString();
				else
					return string.Empty;
			}
			set
			{
				this.ViewState["DocumentNumber"]=value;
			}
		}
	}
}