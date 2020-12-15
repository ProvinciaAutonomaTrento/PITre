using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AmmUtils;
using DocsPAWA.AdminTool.Manager;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.AdminTool.Gestione_StrutturaSottofascicoli
{
    public partial class StrutturaSottofascicoli : Page
    {
        private readonly StrutturaSottofascicoliManager _manager = new StrutturaSottofascicoliManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.IsNewSession)
                Response.Redirect("../Exit.aspx?FROM=EXPIRED", true);

            var ws = new WebServiceLink();
            if (!ws.CheckSession(Session.SessionID))
                Response.Redirect("../Exit.aspx?FROM=ABORT", true);

            if (Session["AMMDATASET"] == null)
            {
                ClientScript.RegisterStartupScript(GetType(), "NoProfilazione",
                    "<script>alert('Attenzione selezionare un\\'amministrazione !'); document.location = '../Gestione_Homepage/Home.aspx';</script>");
                return;
            }

            if (IsPostBack)
                return;

            lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " +
                                UtilsXml.GetAmmDataSession((string) Session["AMMDATASET"], "1");

            LoadGrid();
        }

        private void LoadGrid()
        {
            var data = _manager.GetListStrutture(IDAmministrazione);

            if (data.Rows.Count > 0)
            {
                dgStrutture.Visible = true;
                dgStrutture.DataSource = data;
                dgStrutture.DataBind();
            }
            else
            {
                dgStrutture.Visible = false;
            }
        }

        #region Properties

        public InfoUtente InformazioniUtente
        {
            get
            {
                InfoUtente info = new SessionManager().getUserAmmSession();
                info.idAmministrazione = IDAmministrazione;
                return info;
            }
        }

        public string IDAmministrazione
        {
            get
            {
                var amministrazione = ((string) Session["AMMDATASET"]).Split('@');
                return Utils.getIdAmmByCod(amministrazione[0], this);
            }
        }

        public DataTable ProjectTreeStructure
        {
            get { return ViewState["ProjectTreeStructure"] as DataTable; }
            set { ViewState["ProjectTreeStructure"] = value; }
        }

        #endregion

        #region DataGrid Events

        protected void dgStrutture_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Pager)
                return;

            if (e.Item.Cells.Count > 0)
                e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
        }

        protected void dgStrutture_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            var input = Page.FindControl("txt_confirmDel") as HtmlInputHidden;
            if (input != null && input.Value != "si")
                return;

            var itemid = Convert.ToInt32(dgStrutture.Items[e.Item.ItemIndex].Cells[0].Text);
            var result = _manager.DeleteStrutture(IDAmministrazione, itemid);
            pNuovaStruttura.Visible = false;

            if (result.Codice == 0)
                LoadGrid();
            else
                MessageBox(result.Descrizione);
        }

        protected void dgStrutture_SelectedIndexChanged(object sender, EventArgs e)
        {
            hfStrutturaID.Value = dgStrutture.SelectedItem.Cells[0].Text;
            var itemid = Convert.ToInt32(dgStrutture.SelectedItem.Cells[0].Text);
            tbNomeStruttura.Text = dgStrutture.SelectedItem.Cells[1].Text;
            pNuovaStruttura.Visible = true;
            tbDesElemento.Text = string.Empty;
            
            ProjectTreeStructure = _manager.GetTreeStrutture(IDAmministrazione, itemid);
            tbNumNodiStruttura.Text = GetNodesNumber();
            lvStruttura_ItemCanceling(null, null);
        }

        #endregion

        #region ListView Strutture Events

        protected void lvStruttura_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            var eItem = e.Item as ListViewDataItem;
            if (eItem != null)
                InitialiseRow(eItem);
        }

        private void InitialiseRow(ListViewDataItem item)
        {
            var indentationLevel = Convert.ToInt32(DataBinder.Eval(item.DataItem, "IndentationLevel"));
            var last = Convert.ToInt32(DataBinder.Eval(item.DataItem, "Last"));
            var indent = item.FindControl("litIndent") as ITextControl;

            if (indentationLevel > 0 && indent != null)
                indent.Text = string.Format("{0}--",
                    string.Join("", Enumerable.Repeat("<span style=\"padding-left:1em\"></span>", indentationLevel).ToArray()));

            // Set the location name text
            var ctrl = (WebControl) item.FindControl("tbNodeName");
            if (ctrl != null)
            {
                ctrl.Style.Add("padding-left", "1em");
                (ctrl as ITextControl).Text = DataBinder.Eval(item.DataItem, "Name").ToString();
            }
        }

        protected void btNuovaStruttura_Click(object sender, EventArgs e)
        {
            pNuovaStruttura.Visible = true;
            tbNomeStruttura.Text = string.Empty;
            tbNomeStruttura.Focus();
            hfStrutturaID.Value = string.Empty;
            tbDesElemento.Text = string.Empty;

            ProjectTreeStructure = _manager.GetTreeStrutture();
            SetListViewData(InsertItemPosition.None, -1);
        }

        private void SetListViewData(InsertItemPosition itemposition, int edititemindex)
        {
            lvStruttura.InsertItemPosition = itemposition;
            lvStruttura.EditIndex = edititemindex;
            ProjectTreeStructure.DefaultView.Sort = "Index ASC";
            lvStruttura.DataSource = ProjectTreeStructure.DefaultView;
            lvStruttura.DataBind();
        }

        protected void btSalvaStruttura_Click(object sender, EventArgs e)
        {
            var id = int.MinValue;

            if (!IsValid)
                return;

            if (!string.IsNullOrEmpty(hfStrutturaID.Value))
                id = Convert.ToInt32(hfStrutturaID.Value);

            if(string.IsNullOrEmpty(IDAmministrazione))
            {
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "AMMNULL", "javascript:alert('L\\'ID amministrazione è null');", true);
                return;
            }

            var result = _manager.SaveStruttura(id, ProjectTreeStructure, tbNomeStruttura.Text, InformazioniUtente, UserManager.getRuolo(this), IDAmministrazione);
            if (result.Codice == 0)
            {
                LoadGrid();
                pNuovaStruttura.Visible = false;
                hfStrutturaID.Value = string.Empty;
            }
            else
            {
                MessageBox(result.Descrizione);
            }
        }

        protected void btAnnulla_Click(object sender, EventArgs e)
        {
            pNuovaStruttura.Visible = false;
            hfStrutturaID.Value = string.Empty;
        }

        protected void lvStruttura_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "InsertNode":

                    hfParentID.Value = Convert.ToString(e.CommandArgument);
                    SetListViewData(InsertItemPosition.LastItem, -1);

                    var tbInsertNodo = lvStruttura.InsertItem.FindControl("tbNodeName");
                    if (tbInsertNodo != null)
                        tbInsertNodo.Focus();

                    break;

                case "UpdateNode":

                    var value = (e.Item.FindControl("tbNodeName") as ITextControl).Text;
                    var id = Convert.ToInt32(e.CommandArgument);

                    var tbUpdateNodo = e.Item.FindControl("tbNodeName");
                    if (tbUpdateNodo != null)
                        tbUpdateNodo.Focus();

                    try
                    {
                        ProjectTreeStructure = _manager.UpdateNode(ProjectTreeStructure, id, value, hfStrutturaID.Value);
                        tbNumNodiStruttura.Text = GetNodesNumber();
                    }
                    catch (Exception ex)
                    {
                        MessageBox(ex.Message);
                    }

                    lvStruttura_ItemCanceling(null, null);
                    break;
            }
        }

        protected void lvStruttura_ItemInserting(object sender, ListViewInsertEventArgs e)
        {
            var nodename = (e.Item.FindControl("tbNodeName") as TextBox).Text;
            int? parentid = null;

            if (!string.IsNullOrEmpty(hfParentID.Value))
                parentid = Convert.ToInt32(hfParentID.Value);

            try
            {
                var ruolo = UserManager.getRuolo(this);
                ProjectTreeStructure = _manager.AddNode(
                    ProjectTreeStructure, parentid, nodename, hfStrutturaID.Value, InformazioniUtente, ruolo);

                tbNumNodiStruttura.Text = GetNodesNumber();
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                MessageBox(ex.Message);
            }

            if (!e.Cancel)
                lvStruttura_ItemCanceling(null, null);
        }

        protected void lvStruttura_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            SetListViewData(InsertItemPosition.None, e.NewEditIndex);
        }

        protected void lvStruttura_ItemCanceling(object sender, ListViewCancelEventArgs e)
        {
            SetListViewData(InsertItemPosition.None, -1);
        }

        protected void lvStruttura_ItemDeleting(object sender, ListViewDeleteEventArgs e)
        {
            try
            {
                var node = ProjectTreeStructure.Select(string.Format("INDEX = {0}", e.ItemIndex));
                var childs = ProjectTreeStructure.Select(string.Format("ID_PARENT = {0}", node[0]["SYSTEM_ID"]));
                if (childs.Any())
                    throw new Exception("Non è possibile cancellare un nodo con figli");

                if (string.IsNullOrEmpty(IDAmministrazione))
                    throw new Exception("Attenzione! L\\'utente non ha un amministrazione associata");

                var numfascicoli = -1;
                _manager.IsNodoStrutturaInFascicoliEmpty(ProjectTreeStructure, e.ItemIndex, hfStrutturaID.Value, out numfascicoli, IDAmministrazione);
                
                //ABBATANGELI - Controllo eliminato sotto richiesta di Mai Cristina
                //if (numfascicoli > 0)
                //    throw new Exception(
                //        string.Format(
                //            "Attenzione! Non è possibile cancellare l\\'elemento \r\n perchè ci sono {0} sottofascicoli che contengono dati", 
                //            numfascicoli));

                ProjectTreeStructure = _manager.RemoveNode(ProjectTreeStructure, node[0], hfStrutturaID.Value, InformazioniUtente);
                lvStruttura_ItemCanceling(null, null);
                tbNumNodiStruttura.Text = GetNodesNumber();
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                MessageBox(ex.Message);
            }
        }

        private void MessageBox(string message)
        {
            var script = string.Format("alert('{0}')", message.Replace("'","\\'"));
            ClientScript.RegisterStartupScript(GetType(), "message", script, true);
        }

        protected string GetNodesNumber()
        {
            return ProjectTreeStructure.Rows.Count > 0 ? "OK" : "";
        }

        #endregion

        protected void OnAddRootElement(object sender, ImageClickEventArgs e)
        {
            try
            {
                ProjectTreeStructure = _manager.AddNode(
                    ProjectTreeStructure, null, tbDesElemento.Text, string.Empty, null, null);

                lvStruttura_ItemCanceling(null, null);
                tbNumNodiStruttura.Text = GetNodesNumber();
                tbDesElemento.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox(ex.Message);
            }
        }
    }
}