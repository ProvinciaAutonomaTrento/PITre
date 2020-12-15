using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SiteNavigation;


namespace DocsPAWA.AdminTool.Gestione_Conservazione
{
    public partial class PolicyDocumentiPARER : System.Web.UI.Page
    {

        private DocsPaWR.DocsPaWebService _wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.btn_new_policy.OnClientClick = String.Format("NewPolicyDocumenti();");
                this.listaPolicy = null;
                this.itemsToExport = null;
                if (Session["refreshGrid"] != null)
                    Session.Remove("refreshGrid");

                FetchData();
            }
            else
            {
                if (Session["refreshGrid"] != null && Session["refreshGrid"].ToString().Equals("1"))
                {
                    this.FetchData();
                    this.box_policy.Update();
                    Session.Remove("refreshGrid");
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Refresh", "Refresh();", true);
                }
            }
            //FetchData();
            
        }

        protected void ViewDetails(object sender, EventArgs e)
        {
            ImageButton image = (ImageButton)sender;
            TableCell cell = (TableCell)image.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            Label a = (Label)dgItem.FindControl("lblPolicyID");

            //if (Session["refreshGrid"] != null)
            //    Session.Add("refreshGrid", "1");
            //else
            //    Session["RefreshGrid"] = "1";
            
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenPolicyDetails", "OpenPolicyDetails('" + a.Text + "');", true);
        }

        protected void btn_refresh_Click(object sender, EventArgs e)
        {
            this.FetchData();
        }

        protected void ImageCreatedRender(Object sender, DataGridItemEventArgs e)
        {

        }

        protected void DataGrid_ItemCreated(object sender, DataGridItemEventArgs e)
        {

        }

        protected void chkPolicy_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void DeletePolicy(object sender, EventArgs e)
        {
            ImageButton image = (ImageButton)sender;
            TableCell cell = (TableCell)image.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            Label a = (Label)dgItem.FindControl("lblPolicyID");

            DocsPAWA.DocsPaWR.EsecuzionePolicy infoPolicy = this._wsInstance.GetInfoEsecuzionePolicy(a.Text);
            if (infoPolicy != null)
            {
                if (infoPolicy.numeroEsecuzioni.Equals("0"))
                {
                    if (this._wsInstance.DeletePolicyPARER(a.Text, this.GetInfoUser()))
                    {
                        this.FetchData();
                        this.box_policy.Update();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Refresh", "Refresh();", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "del_error", "alert('Si è verificato un errore nell\\' eliminazione della policy.');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "del_forbidden", "alert('Non è possibile eliminare la policy in quanto è già stata eseguita.');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "del_error", "alert('Si è verificato un errore nell\\' eliminazione della policy.');", true);
            }
        }

        protected void BtnSavePolicy_Click(object sender, EventArgs e)
        {

            ArrayList policyToUpdate = new ArrayList();

            foreach (DataGridItem item in grvPolicy.Items)
            {
                string id = ((Label)item.FindControl("lblPolicyID")).Text;
                string attiva = ((CheckBox)item.FindControl("chk_attiva_policy")).Checked ? "1" : "0";
                foreach (PolicyPARER policy in this.listaPolicy)
                {
                    if (policy.id.Equals(id))
                    {
                        if (policy.isAttiva != attiva)
                        {
                            policy.isAttiva = attiva;
                            policy.tipo = "D";
                            policyToUpdate.Add(policy);
                        }
                    }
                }
            }

            if (policyToUpdate != null && policyToUpdate.Count > 0)
            {
                if (this._wsInstance.UpdateStatoPolicy(policyToUpdate.ToArray(), this.GetInfoUser()))
                {
                    // Aggiornamento dati
                    this.FetchData();
                    this.box_policy.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "policy_update_ok", "SavePolicyCompleted();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "policy_update_error", "alert('Errore nell\\'aggiornamento delle policy');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_policy_to_update", "alert('Non ci sono policy da aggiornare');", true);
            }
        }

        protected void BtnExportPolicy_Click(object sender, EventArgs e)
        {
            ArrayList items = new ArrayList();

            foreach (DataGridItem item in grvPolicy.Items)
            {
                if (((CheckBox)item.FindControl("chk_seleziona_policy")).Checked)
                {
                    string id = ((Label)item.FindControl("lblPolicyID")).Text;
                    items.Add(id);
                }
            }

            if (items != null)
            {
                if (items.Count > 0)
                {
                    this.itemsToExport = items;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Export", "Export();", true);
                }
                else
                {
                    this.itemsToExport = null;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Export_noDoc", "alert('Nessuna policy selezionata');", true);
                }
            }
            else
            {
                this.itemsToExport = null;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Export_err", "alert(''Attenzione, si è verificato un errore');", true);
            }
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            if (chk != null)
            {
                if (chk.Checked)
                {
                    foreach (DataGridItem item in grvPolicy.Items)
                    {
                        ((CheckBox)item.FindControl("chk_seleziona_policy")).Checked = true;
                    }
                    chk.ToolTip = "Deseleziona tutti";
                }
                else
                {
                    foreach (DataGridItem item in grvPolicy.Items)
                    {
                        ((CheckBox)item.FindControl("chk_seleziona_policy")).Checked = false;
                    }
                    chk.ToolTip = "Seleziona tutti";
                }
            }
        }

        #region Methods

        private void FetchData()
        {
            //PolicyPARER[] policyList = this._wsInstance.GetListaPolicyPARER(this.IdAmministrazione);
            this.listaPolicy = this._wsInstance.GetListaPolicyPARER(this.IdAmministrazione, "D");
            this.grvPolicy.DataSource = this.listaPolicy;
            this.grvPolicy.CurrentPageIndex = 0;
            this.grvPolicy.DataBind();
        }

        protected string GetIdPolicy(PolicyPARER policy)
        {
            return policy.id;
        }

        protected string GetCodPolicy(PolicyPARER policy)
        {
            return policy.codice;
        }

        protected string GetDescrPolicy(PolicyPARER policy)
        {
            return policy.descrizione;
        }

        protected bool GetStatoAttivazionePolicy(PolicyPARER policy)
        {
            return (policy.isAttiva.Equals("1"));
        }

        protected InfoUtente GetInfoUser()
        {
            DocsPAWA.AdminTool.Manager.SessionManager manager = new Manager.SessionManager();
            InfoUtente result = manager.getUserAmmSession();
            result.idAmministrazione = this.IdAmministrazione;
            return result;
        }

        #endregion

        #region Properties

        protected string UrlNewPolicy
        {
            get
            {
                return "newPolicyDocPARER.aspx?s=new";
            }
        }

        protected string UrlViewPolicy
        {
            get
            {
                return "newPolicyDocPARER.aspx";
            }
        }

        protected string UrlExport
        {
            get
            {
                return "exportPolicyPARER.aspx";
            }
        }

        /// <summary>
        /// Reperimento idamministrazione corrente
        /// </summary>
        protected string IdAmministrazione
        {
            get
            {
                return AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            }
        }

        protected PolicyPARER[] listaPolicy
        {
            get
            {
                return HttpContext.Current.Session["listaPolicy"] as PolicyPARER[];
            }
            set
            {
                HttpContext.Current.Session["listaPolicy"] = value;
            }
        }

        protected ArrayList itemsToExport
        {
            get
            {
                return HttpContext.Current.Session["itemsToExport"] as ArrayList;
            }
            set
            {
                HttpContext.Current.Session["itemsToExport"] = value;
            }
        }

        #endregion
    }
}