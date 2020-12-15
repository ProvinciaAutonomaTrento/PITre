using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using DocsPAWA.SiteNavigation;
using System.Drawing;

namespace DocsPAWA.Grids
{
    public partial class GridPreferred : CssPage
    {
        protected InfoUtente infoUtente;

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);

            this.infoUtente = UserManager.getInfoUtente(this);

            if (!IsPostBack)
            {
                this.ListGrid = null;
              //  SetFocus();
                GetTheme();
                GetInitData();
                string result = string.Empty;
                if (Request.QueryString["tabRes"] != string.Empty && Request.QueryString["tabRes"] != null)
                {
                    result = Request.QueryString["tabRes"].ToString();
                }
                this.hid_tab_est.Value = result;
            }
        }

        protected void GetTheme()
        {
            string Tema = string.Empty;
            string idAmm = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
                idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            else
            {
                if (UserManager.getInfoUtente() != null)
                    idAmm = UserManager.getInfoUtente().idAmministrazione;
            }

            UserManager userM = new UserManager();
            Tema = userM.getCssAmministrazione(idAmm);

            if (!string.IsNullOrEmpty(Tema))
            {
                string[] colorsplit = Tema.Split('^');
                System.Drawing.ColorConverter colConvert = new ColorConverter();
                this.titlePage.ForeColor = System.Drawing.Color.White;
                this.titlePage.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#" + colorsplit[2]);

            }
            else
            {
                System.Drawing.ColorConverter colConvert = new ColorConverter();
                this.titlePage.ForeColor = System.Drawing.Color.White;
                this.titlePage.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#810d06");
            }




        }

        /// Al clic viene salvata la griglia preferita
        /// </summary>
        protected void BtnSavePrefGrid_Click(object sender, EventArgs e)
        {
            InfoUtente infoUtente = UserManager.getInfoUtente(this);
            string idGrid = Request.Form["rbl_pref"];
            bool standard = false;
            if (!string.IsNullOrEmpty(idGrid))
            {
                if (idGrid.Equals("-1"))
                {
                    //Ho impostato la griglia standard, quindi mi limito a cancellare la vecchia griglia preferita per questa tipologia
                    GridManager.RemovePreferredTypeGrid(infoUtente, GridManager.SelectedGrid.GridType);
                    GridManager.SelectedGrid = GridManager.getUserGrid(GridManager.SelectedGrid.GridType);
                }
                else
                {
                  List<GridBaseInfo> tempList = new List<GridBaseInfo>(this.ListGrid);
                  GridBaseInfo tempGrid = (GridBaseInfo)tempList.Where(g => g.GridId.Equals(idGrid)).FirstOrDefault();
                  GridManager.AddPreferredGrid(tempGrid.GridId, infoUtente, GridManager.SelectedGrid.GridType);
                  GridManager.SelectedGrid = GridManager.getUserGrid(GridManager.SelectedGrid.GridType);
                }

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save();", true);
            }

        }

        protected void DataGrid_ItemCreated(object sender, DataGridItemEventArgs e)
        {

            ImageButton imgDelete = (ImageButton)e.Item.FindControl("btn_Rimuovi");
            if (imgDelete != null)
            {
                imgDelete.Attributes.Add("onmouseover", "this.src='../images/ricerca/cancella_griglia_hover.gif'");
                imgDelete.Attributes.Add("onmouseout", "this.src='../images/ricerca/cancella_griglia.gif'");
            }

            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        protected void GetInitData()
        {
            if (GridManager.SelectedGrid != null)
            {
                this.ListGrid = GridManager.GetGridsBaseInfo(infoUtente, GridManager.SelectedGrid.GridType, true);
            }
            else
            {
                this.ListGrid = GridManager.GetGridsBaseInfo(infoUtente, GridTypeEnumeration.Document, true);
            }
            this.grvPreferred.DataSource = this.ListGrid;
            this.grvPreferred.DataBind();
            
        }

        protected String GetGridID(GridBaseInfo temp)
        {
            return temp.GridId;
        }

        protected String GetGridPreferred(GridBaseInfo temp)
        {
            string result = string.Empty;
            if (temp.IsPreferred)
            {
                result = "checked";
            }
            return result;
        }

        protected bool GetRoleGridID(GridBaseInfo temp)
        {
            bool result = false;
            if (temp.RoleGrid)
            {
                result = true;
            }
            return result;
        }

        protected String GetGridName(GridBaseInfo temp)
        {
            return temp.GridName;
        }

        protected String GetImageUserGridID(GridBaseInfo temp)
        {
            if (temp.UserGrid)
            {
                return "../images/ricerca/check_grid.gif";
            }
            else
            {
                return "../images/ricerca/no_check_grid.gif";
            }
        }

        protected String GetImageRoleGridID(GridBaseInfo temp)
        {
            if (temp.RoleGrid)
            {
                return "../images/ricerca/check_grid.gif";
            }
            else
            {
                return "../images/ricerca/no_check_grid.gif";
            }
        }

        protected bool GetDeleteGridID(GridBaseInfo temp)
        {
            bool result = true;
            if (temp.GridId.Equals("-1"))
            {
                result = false;
            }
            return result;
        }

     /*   public void SetFocus()
        {
            ClientScript.RegisterStartupScript(this.GetType(), "focus", "setFocus();", true);
        }*/

        protected void DeleteGrid(object sender, EventArgs e)
        {
            ImageButton image = (ImageButton)sender;
            TableCell cell = (TableCell)image.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            Label a = (Label)dgItem.FindControl("SYSTEM_ID");
            GridBaseInfo tempBase = new GridBaseInfo();
            foreach (GridBaseInfo gri in this.ListGrid)
            {
                if (gri.GridId.Equals(a.Text))
                {
                    tempBase = gri;
                    break;
                }
            }

            GridManager.RemoveGrid(tempBase);

            if (tempBase.IsPreferred)
            {
                GridManager.SelectedGrid = GridManager.getUserGrid(GridManager.SelectedGrid.GridType);
            }

            this.ListGrid = GridManager.GetGridsBaseInfo(this.infoUtente, GridManager.SelectedGrid.GridType, true);

            this.grvPreferred.DataSource = this.ListGrid;
            this.grvPreferred.CurrentPageIndex = 0;
            this.grvPreferred.DataBind();
            this.box_preferred_grids.Update();
        }

        protected void SelectedIndexChanged(object sender, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            this.grvPreferred.CurrentPageIndex = e.NewPageIndex;
            this.grvPreferred.DataSource = this.ListGrid;
            this.grvPreferred.DataBind();
            this.box_preferred_grids.Update();
        }

        private GridBaseInfo[] ListGrid
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["listGrid"] as GridBaseInfo[];
            }
            set
            {
                CallContextStack.CurrentContext.ContextState["listGrid"] = value;
            }
        }
    }
}