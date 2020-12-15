using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class SelectAttForPublication : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString != null && Request.QueryString["from"] != null && Request.QueryString["from"] == "attach")
                {
                    //List<string> statiSelAll = DiagrammiManager.Albo_GetStatiSelezioneFiles(this.StateDiagram.SYSTEM_ID.ToString()).ToList<string>();
                    var statiSelAll1 = DiagrammiManager.Albo_GetStatiSelezioneFiles(this.StateDiagram.SYSTEM_ID.ToString());
                    List<string> statiSelAll = statiSelAll1 == null ? null : statiSelAll1.ToList<string>();
                    if (statiSelAll != null && statiSelAll.Count > 0)
                    {
                        DocsPaWR.Stato stato = DiagrammiManager.GetStateDocument(DocumentManager.getSelectedRecord().systemId);

                        if (stato != null && statiSelAll.Contains(stato.SYSTEM_ID.ToString()))
                        {
                            modifica = true;
                        }
                        else
                            modifica = false;

                    }
                }
                else modifica = true;
                InitializeLanguage(modifica);
                AllDaNonPubblicare = DiagrammiManager.Albo_GetFileDaPubblicare(Doc.systemId, "N");
                this.SelectedPage = 0;
                grdAllegatiDataBind();
            }
                    
        }

        protected void InitializeLanguage( bool modificaX)
        {
            if (modificaX)
            {
                this.SelectAttForPubOK.Text = Utils.Languages.GetLabelFromCode("GenericBtnOk", UserManager.GetUserLanguage());
                this.SelectAttForPubAnnulla.Text = Utils.Languages.GetLabelFromCode("GenericBtnCancel", UserManager.GetUserLanguage());
            }else{
                this.SelectAttForPubOK.Text = Utils.Languages.GetLabelFromCode("GenericBtnOk", UserManager.GetUserLanguage());
                this.SelectAttForPubOK.Visible = false;
                this.SelectAttForPubAnnulla.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", UserManager.GetUserLanguage());
            }
        }

        private SchedaDocumento Doc
        {
            get
            {
                return DocumentManager.getSelectedRecord();
            }

            set
            {
                DocumentManager.setSelectedRecord(value);
            }
        }

        private string[] AllDaPubblicare;
        private string[] AllDaNonPubblicare;
        private bool modifica;

        private int SelectedPage
        {
            get
            {
                int toReturn = 0;
                if (HttpContext.Current.Session["SelectedPage"] != null) Int32.TryParse(HttpContext.Current.Session["SelectedPage"].ToString(), out toReturn);
                if (toReturn < 0) toReturn = 0;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["SelectedPage"] = value;
            }

        }

        private void grdAllegatiDataBind()
        {
            
            Allegato[] at = null;
            at = (from att in Doc.allegati where att.TypeAttachment == 1 select att).ToArray();
            this.grdAllegati.DataSource = at;
            this.grdAllegati.PageIndex = this.SelectedPage;
            this.grdAllegati.DataBind();
        }

        protected void grdAllegati_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.SelectedPage = e.NewPageIndex;
                this.grid_rowindex.Value = "0";
                grdAllegatiDataBind();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void grdAllegati_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.FindControl("chkPubblicazione") != null)
                {
                    if (AllDaNonPubblicare != null && AllDaNonPubblicare.Length > 0)
                    {
                        List<string> AllDaNonPubb = AllDaNonPubblicare.ToList<string>();
                        if (AllDaNonPubb.Contains(((HiddenField)e.Row.FindControl("attDocNum")).Value))
                            ((CheckBox)e.Row.FindControl("chkPubblicazione")).Checked = false;
                        else
                            ((CheckBox)e.Row.FindControl("chkPubblicazione")).Checked = true;

                    }
                    else
                    {
                        ((CheckBox)e.Row.FindControl("chkPubblicazione")).Checked = true;
                    }
                    if(!modifica)
                        ((CheckBox)e.Row.FindControl("chkPubblicazione")).Enabled = false;
                }
                
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SelectAttForPubOK_Click(object sender, EventArgs e)
        {
            if (grdAllegati != null && grdAllegati.Rows != null && grdAllegati.Rows.Count > 0)
            {
                foreach (GridViewRow r in grdAllegati.Rows)
                {
                    if (r.FindControl("chkPubblicazione") != null)
                    {
                        if (((CheckBox)r.FindControl("chkPubblicazione")).Checked)
                            DiagrammiManager.Albo_InsFileDaPubblicare(Doc.systemId, ((HiddenField)r.FindControl("attDocNum")).Value, "S");
                        else
                            DiagrammiManager.Albo_InsFileDaPubblicare(Doc.systemId, ((HiddenField)r.FindControl("attDocNum")).Value, "N");
                    }
                }
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SelectAttForPublication", "parent.closeAjaxModal('SelectAttForPublication', 'up', parent);", true);
        }

        protected void SelectAttForPubAnnulla_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SelectAttForPublication", "parent.closeAjaxModal('SelectAttForPublication', 'up', parent);", true);
        
        }

        private DiagrammaStato StateDiagram
        {
            get
            {
                DiagrammaStato result = null;
                if (HttpContext.Current.Session["stateDiagram"] != null)
                {
                    result = HttpContext.Current.Session["stateDiagram"] as DiagrammaStato;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["stateDiagram"] = value;
            }
        }
    }
}