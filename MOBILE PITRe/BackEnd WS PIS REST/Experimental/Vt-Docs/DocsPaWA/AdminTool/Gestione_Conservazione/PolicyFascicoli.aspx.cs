using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.AdminTool.Gestione_Conservazione
{
    public partial class PolicyFascicoli : System.Web.UI.Page
    {
        private DocsPaWR.DocsPaWebService _wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.btn_new_policy.OnClientClick = String.Format("NewPolicyFascicoli();");
            }
            FetchData();
        }

        protected void FetchData()
        {
            Policy[] policyList = this._wsInstance.GetListaPolicy(IdAmministrazione, "F");
            this.grvPolicy.DataSource = policyList;
            this.grvPolicy.CurrentPageIndex = 0;
            this.grvPolicy.DataBind();
        }

        protected String GetPolicyID(Policy temp)
        {
            return temp.system_id;
        }

        protected String GetPolicyName(Policy temp)
        {
            return temp.nome;
        }

        protected void DeletePolicy(object sender, EventArgs e)
        {
            ImageButton image = (ImageButton)sender;
            TableCell cell = (TableCell)image.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            Label a = (Label)dgItem.FindControl("SYSTEM_ID");

            this._wsInstance.DeletePolicy(a.Text);

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Refresh", "Refresh();", true);
        }

        protected void ViewDetails(object sender, EventArgs e)
        {
            ImageButton image = (ImageButton)sender;
            TableCell cell = (TableCell)image.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            Label a = (Label)dgItem.FindControl("SYSTEM_ID");

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenPolicyDetails", "OpenPolicyDetails('" + a.Text + "');", true);
        }

        protected void ImageCreatedRender(Object sender, DataGridItemEventArgs e)
        {
            ImageButton imgPeriodo = (ImageButton)e.Item.FindControl("btn_periodo");
            if (imgPeriodo != null)
            {
                imgPeriodo.Attributes.Add("onmouseover", "this.src='../../images/proto/dett_lente_doc_up.gif'");
                Label a = (Label)e.Item.FindControl("AUTOMATICO");
                if (a.Text.Equals("1"))
                {
                    imgPeriodo.Attributes.Add("onmouseout", "this.src='../../images/open_period.gif'");
                }
                else
                {
                    imgPeriodo.Attributes.Add("onmouseout", "this.src='../../images/open_period_no.gif'");
                }
            }
        }
        protected void DataGrid_ItemCreated(object sender, DataGridItemEventArgs e)
        {

            ImageButton imgDelete = (ImageButton)e.Item.FindControl("btn_Rimuovi");
            ImageButton imgDetails = (ImageButton)e.Item.FindControl("btn_dettagli");


            if (imgDelete != null)
            {
                imgDelete.Attributes.Add("onmouseover", "this.src='../../images/ricerca/cancella_griglia_hover.gif'");
                imgDelete.Attributes.Add("onmouseout", "this.src='../../images/ricerca/cancella_griglia.gif'");
            }

            if (imgDetails != null)
            {
                imgDetails.Attributes.Add("onmouseover", "this.src='../../images/proto/dett_lente_doc_up.gif'");
                imgDetails.Attributes.Add("onmouseout", "this.src='../../images/proto/dett_lente_doc.gif'");
            }



            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        protected void ViewPeriodDetails(object sender, EventArgs e)
        {
            ImageButton image = (ImageButton)sender;
            TableCell cell = (TableCell)image.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            Label a = (Label)dgItem.FindControl("SYSTEM_ID");

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenPeriodDetails", "OpenPeriodDetails('" + a.Text + "');", true);

        }

        protected String GetAutomaticPeriod(Policy temp)
        {
            if (temp.periodoAttivo)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        protected String GetImagePeriod(Policy temp)
        {
            if (temp.periodoAttivo)
            {
                return "../../images/open_period.gif";
            }
            else
            {
                return "../../images/open_period_no.gif";
            }
        }

        protected string UrlNewPolicy
        {
            get
            {
                return "newPolicyFascicolo.aspx?s=new";
            }
        }

        protected string UrlPeriodPolicy
        {
            get
            {
                return "PeriodDetails.aspx";
            }
        }

        protected string UrlViewPolicy
        {
            get
            {
                return "newPolicyFascicolo.aspx";
            }
        }

        /// <summary>
        /// Reperimento idamministrazione corrente
        /// </summary>
        protected int IdAmministrazione
        {
            get
            {
                string idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                return Convert.ToInt32(idAmministrazione);
            }
        }
    }
}