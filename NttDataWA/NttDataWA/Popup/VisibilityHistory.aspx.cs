using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
	public partial class VisibilityHistory : System.Web.UI.Page
	{
		protected StoriaDirittoDocumento[] ListHistory;
		protected Fascicolo Fasc;
		
		protected void Page_Load(object sender, EventArgs e)
		{
            try {
			    if (!IsPostBack)
			    {
                    this.InitializeLanguage();
				    this.GridHistory.Visible = false;
				    this.BindGrid();
			    }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
		}

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.LblHistory.Text = Utils.Languages.GetMessageFromCode("LblHistory", language);
            this.GridHistory.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityHistoryUser", language);
            this.GridHistory.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityHistoryRole", language);
            this.GridHistory.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityHistoryDate", language);
            this.GridHistory.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityHistoryCode", language);
            this.GridHistory.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityHistoryDescription", language);
            this.HistoryBtnClose.Text = Utils.Languages.GetLabelFromCode("HistoryBtnChiudi", language);
        }

		public void BindGrid()
		{
            if (Request.QueryString["tipoObj"].Equals("D"))
            {
                SchedaDocumento doc = UIManager.DocumentManager.getSelectedRecord();
                if (doc == null)
                {
                    //return;
                }

                this.HistoryList = DocumentManager.GetVisibilityHistory(doc.systemId, Request.QueryString["tipoObj"], UserManager.GetInfoUser());
            }

            if (Request.QueryString["tipoObj"].Equals("F"))
            {
                Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();

                if (fascicolo == null)
                {
                    //return;
                }

                this.HistoryList = DocumentManager.GetVisibilityHistory(fascicolo.systemID, Request.QueryString["tipoObj"], UserManager.GetInfoUser());
            }

			if (this.HistoryList == null || this.HistoryList.Length == 0)
			{
				this.LblHistory.Visible = true;
				this.GridHistory.Visible = false;
				//return;
			}
			else
			{
				this.GridHistory.Visible = true;
				//Bind list to grid
				List<StoriaDirittoDocumento> list = new List<StoriaDirittoDocumento>();
				list.AddRange(this.HistoryList);
				this.GridHistory.Visible = true;
				this.GridHistory.DataSource = list;
				this.GridHistory.DataBind();

				//Dt_elem = new ArrayList();
				//for (int i = 0; i < ListHistory.Length; i++)
				//    Dt_elem.Add(new Cols(ListHistory[i].utente, ListHistory[i].ruolo, ListHistory[i].data.Substring(0, 10), ListHistory[i].codOperazione, ListHistory[i].descrizione));

				//if (ListHistory.Length > 0)
				//{
				//    //DocumentManager.setDataGridAllegati(this,Dt_elem);					
				//    this.DGStoria.DataSource = Dt_elem;
				//    this.DGStoria.DataBind();
				//    this.LblDettagli.Visible = false;
				//}
				//else
				//{
				//    this.LblDettagli.Visible = true;
				//}
				//this.DGStoria.Visible = true;
			}
		}

		#region Session

		protected StoriaDirittoDocumento[] HistoryList
		{
			get
			{
				StoriaDirittoDocumento[] result = null;
				if (HttpContext.Current.Session["historylist"] != null)
				{
					result = HttpContext.Current.Session["historylist"] as StoriaDirittoDocumento[];
				}
				return result;
			}
			set
			{
				HttpContext.Current.Session["historylist"] = value;
			}

		}
		#endregion

		protected void HistoryBtnClose_Click(object sender, EventArgs e)
		{
			Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('VisibilityHistory', '');</script></body></html>");
			Response.End();
		}

		protected void GridHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
            try {
			    this.GridHistory.PageIndex = e.NewPageIndex;
			    this.GridHistory.DataSource = this.HistoryList;
			    this.GridHistory.DataBind();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
		}
		
	}
}