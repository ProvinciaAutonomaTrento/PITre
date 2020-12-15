using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool.DocsPaWR;
using SAAdminTool.SiteNavigation;

namespace SAAdminTool.AdminTool.Gestione_Conservazione
{
    public partial class PolicyStampe : System.Web.UI.Page
    {
        private DocsPaWR.DocsPaWebService _wsInstance = new SAAdminTool.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.btn_new_policy.OnClientClick = String.Format("NewPolicyStampe();");
            }
            FetchData();
        }

        protected void FetchData()
        {
            this._wsInstance.Timeout = System.Threading.Timeout.Infinite;
            Policy[] policyList = this._wsInstance.GetListaPolicy(IdAmministrazione, "C");
            Policy[] policyList2 = this._wsInstance.GetListaPolicy(IdAmministrazione, "R");

            Policy[] policyListTotal = null;
            int newDimension = 0;
            int start = 0;

            if (policyList != null)
            {
                newDimension = newDimension + policyList.Length;
            }
            if (policyList2 != null)
            {
                newDimension = newDimension + policyList2.Length;
            }

            if (newDimension != 0)
            {
                policyListTotal = new Policy[newDimension];
            }

            if (policyList != null && policyList.Length > 0)
            {
                for (int i = 0; i < policyList.Length; i++)
                {
                    policyListTotal[i] = policyList[i];
                }
                start = policyList.Length;
            }

            if (policyList2 != null && policyList2.Length > 0)
            {
                for (int i = 0; i < policyList2.Length; i++)
                {
                    policyListTotal[start + i] = policyList2[i];
                }
            }

            this.grvPolicy.DataSource = policyListTotal;
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
                return "newPolicyStampe.aspx?s=new";
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
                return "newPolicyStampe.aspx";
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

        protected String GetPolicyType(Policy temp)
        {
            if (temp.tipo.Equals("C"))
            {
                return "Stampa Repertori";
            }
            else
            {
                return "Stampa Registri";
            }
        }

    }
}