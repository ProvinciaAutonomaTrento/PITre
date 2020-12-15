using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using System.Drawing;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.Grids
{
    public partial class GridSave : CssPage
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {

            Utils.startUp(this);

            if (!IsPostBack)
            {
                this.listGrid = null;
                this.gridIdPreferred = string.Empty;
                SetTema();
                SetUserRole();
                InfoUtente infoUtente = UserManager.getInfoUtente(this);
                this.ddl_ric_griglie.Items.Clear();

                if (GridManager.SelectedGrid != null)
                {
                    this.listGrid = GridManager.GetGridsBaseInfo(infoUtente, GridManager.SelectedGrid.GridType, true);
                }
                else
                {
                    this.listGrid = GridManager.GetGridsBaseInfo(infoUtente, GridTypeEnumeration.Document, true);
                }

                if (this.listGrid != null && this.listGrid.Length > 0)
                {
                    int count = 0;
                    foreach (GridBaseInfo gb in this.listGrid)
                    {
                        if (!gb.IsSearchGrid && !gb.GridId.Equals("-1"))
                        {
                            ListItem it = new ListItem(gb.GridName, gb.GridId);
                            this.ddl_ric_griglie.Items.Add(it);
                            if (gb.IsPreferred)
                            {
                                this.gridIdPreferred = gb.GridId;
                            }
                            count++;
                        }
                    }
                    if (count > 0)
                    {
                        this.pnl_scelta.Visible = false;
                        this.nuova_g.Visible = true;
                    }
                    else
                    {
                        //Ho solo la griglia standard quindi non posso modificare nulla devo per forza crearne una nuova
                        this.pnl_scelta.Visible = false;
                        this.nuova_g.Visible = true;
                        this.rbl_save.Items[0].Selected = true;
                        this.rbl_save.Items[1].Enabled = false;
                    }
                }
                string result = string.Empty;
                if (Request.QueryString["tabRes"] != string.Empty && Request.QueryString["tabRes"] != null)
                {
                    result = Request.QueryString["tabRes"].ToString();
                }
                this.hid_tab_est.Value = result;
                this.bDeleteExtender.Enabled = false;
            //    SetFocus();
            }

        }

        protected void btn_salva_Click(object sender, EventArgs e)
        {
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = new DocsPAWA.DocsPaWR.InfoUtente();
            infoUtente = UserManager.getInfoUtente(this.Page);
            Grid TemporaryGrid = GridManager.CloneGrid(GridManager.SelectedGrid);

            string nuovoNome = string.Empty;
            string visibility = this.rblVisibilita.SelectedItem.Value;

            //Nuova griglia
            if (this.rbl_save.SelectedItem.Value.Equals("new"))
            {
                if (string.IsNullOrEmpty(txt_titolo.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Inserire un nome per la griglia');", true);
                }
                else
                {
                    TemporaryGrid.GridName = txt_titolo.Text;
                    nuovoNome = txt_titolo.Text;
                    GridManager.SaveNewGrid(TemporaryGrid, infoUtente, nuovoNome, visibility, this.chk_pref.Checked);

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Griglia salvata');", true);

                    GridManager.SelectedGrid = TemporaryGrid;

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save();", true);
                }
            }
            //Modifica griglia
            else
            {
                TemporaryGrid.GridId = this.ddl_ric_griglie.SelectedValue;
                TemporaryGrid.GridName = this.ddl_ric_griglie.SelectedItem.Text;

                if (string.IsNullOrEmpty(txt_name_mod.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Inserire un nome per la griglia');", true);
                }
                else
                {

                    TemporaryGrid.GridName = txt_name_mod.Text;
                    GridManager.ModifyGrid(TemporaryGrid, infoUtente, visibility, this.chk_pref.Checked);

                    if (this.chk_pref.Checked || (!string.IsNullOrEmpty(this.gridIdPreferred) && this.gridIdPreferred.Equals(TemporaryGrid.GridId)))
                    {
                        GridManager.SelectedGrid = TemporaryGrid;
                    }

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Griglia salvata');", true);
                    
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save();", true);
                }


            }

        }

        protected void SetTema()
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

        protected void ChangeMode(object sender, EventArgs e)
        {
            RadioButtonList obj = (RadioButtonList)sender;
            if (obj.SelectedItem.Value.Equals("mod"))
            {
                this.pnl_scelta.Visible = true;
                this.nuova_g.Visible = false;
                InfoUtente infoUtente = UserManager.getInfoUtente(this);
                this.ddl_ric_griglie.Items.Clear();
                GridBaseInfo tempGrid = null;

                if (this.listGrid != null && this.listGrid.Length > 0)
                {
                    foreach (GridBaseInfo gb in this.listGrid)
                    {
                        if (!gb.IsSearchGrid && !gb.GridId.Equals("-1"))
                        {
                            ListItem it = new ListItem(gb.GridName, gb.GridId);
                            this.ddl_ric_griglie.Items.Add(it);
                            if (gb.IsPreferred)
                            {
                                tempGrid = gb;
                                this.ddl_ric_griglie.SelectedValue = gb.GridId;
                            }
                        }
                    }
                }
                if (tempGrid != null)
                {
                    if (tempGrid.UserGrid)
                    {
                        this.rblVisibilita.Items[0].Selected = true;
                        this.bDeleteExtender.Enabled = false;
                        this.rblVisibilita.Items[1].Selected = false;
                    }
                    else
                    {
                        this.rblVisibilita.Items[1].Selected = true;
                        this.bDeleteExtender.Enabled = true;
                        this.rblVisibilita.Items[0].Selected = false;
                    }
                }
                else
                {
                    this.rblVisibilita.Items[0].Selected = true;
                    this.bDeleteExtender.Enabled = false;
                    this.rblVisibilita.Items[1].Selected = false;
                }
                this.modify_g.Visible = true;
                this.txt_name_mod.Text = this.ddl_ric_griglie.SelectedItem.Text;
                this.upChangeGridName.Update();
                this.pnl_visibility.Update();
            }
            else
            {
                this.pnl_scelta.Visible = false;
                this.nuova_g.Visible = true;
                this.rblVisibilita.Items[0].Selected = true;
                this.rblVisibilita.Items[1].Selected = false;
                this.modify_g.Visible = false;
                this.bDeleteExtender.Enabled = false;
                this.upChangeGridName.Update();
            }
            this.upChangeGrid.Update();
            this.UpdatePanelConfirm.Update();
        }

        //protected void SetFocus()
        //{
        //    ClientScript.RegisterStartupScript(this.GetType(), "focus", "setFocus();", true);
        //}

        protected void SetUserRole()
        {
            Utente user = new Utente();
            user = UserManager.getUtente();
            Ruolo role = new Ruolo();
            role = UserManager.getRuolo();
            rblVisibilita.Items[0].Text = rblVisibilita.Items[0].Text.Replace("@usr@", user.descrizione);
            rblVisibilita.Items[1].Text = rblVisibilita.Items[1].Text.Replace("@grp@", role.descrizione);
        }

        protected void ChangeSelectedGrid(object sender, EventArgs e)
        {
            List<GridBaseInfo> tempList = new List<GridBaseInfo>(this.listGrid);
            GridBaseInfo tempGrid = (GridBaseInfo)tempList.Where(g => g.GridId.Equals(this.ddl_ric_griglie.SelectedValue)).FirstOrDefault();
            if (tempGrid.UserGrid)
            {
                this.rblVisibilita.Items[0].Selected = true;
                this.rblVisibilita.Items[1].Selected = false;
                this.bDeleteExtender.Enabled = false;
                this.bDeleteExtender.ConfirmOnFormSubmit = false;
                this.bDeleteExtender.Enabled = false;
            }
            else
            {
                this.rblVisibilita.Items[0].Selected = false;
                this.rblVisibilita.Items[1].Selected = true;
                this.bDeleteExtender.Enabled = true;
            }
            this.modify_g.Visible = true;
            this.txt_name_mod.Text = tempGrid.GridName;
            this.upChangeGridName.Update();
            this.pnl_visibility.Update();
            this.UpdatePanelConfirm.Update();
        }

        private GridBaseInfo[] listGrid
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

        private string gridIdPreferred
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["gridIdPreferred"] as string;
            }
            set
            {
                CallContextStack.CurrentContext.ContextState["gridIdPreferred"] = value;
            }
        }

    }
}