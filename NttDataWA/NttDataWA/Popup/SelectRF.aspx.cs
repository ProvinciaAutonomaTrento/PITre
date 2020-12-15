using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class selectRF : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                this.SelectRFBtnOk.Enabled = false;
                //ddl_regRF.Text= Utils.Languages.GetLabelFromCode("DocumentNoteSelectAnRF", UIManager.UserManager.GetUserLanguage());

                if (!IsPostBack)
                {

                    this.InitializeLanguage();
                               
                    this.lbl_doc_rf.Text = Utils.Languages.GetLabelFromCode("DocumentLblSelectRF", UIManager.UserManager.GetUserLanguage());

                    CaricaComboRegistri(this.ddl_regRF);
                }

                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
        }

        protected void Ddl_regRF_SelectIndexChange(object sender, EventArgs e)
        {
            try {
                if (this.ddl_regRF.SelectedValue.Length > 0)
                {
                    this.SelectRFBtnOk.Enabled = true;
                }
                else
                {
                    this.SelectRFBtnOk.Enabled = false;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.SelectRFBtnOk.Text = Utils.Languages.GetLabelFromCode("DocumentSelectRFBtnOk", language);
            this.SelectRFBtnChiudi.Text = Utils.Languages.GetLabelFromCode("DocumentSelectRFBtnChiudi", language);           
        }


        private void CaricaComboRegistri(DropDownList ddl)
        {
            DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getSelectedRecord();
            
            //Verifico se il registro ha RF associati
            DocsPaWR.Registro[] listaRF = UserManager.getListaRegistriWithRF(this, "1", schedaDocumento.registro.systemId);
            if (listaRF != null && listaRF.Length > 0)
            {
                if (listaRF.Length == 1)
                {
                    ListItem li = new ListItem();
                    li.Value = listaRF[0].systemId;
                    li.Text = listaRF[0].codRegistro + " - " + listaRF[0].descrizione;
                    this.ddl_regRF.Items.Add(li);
                    ListItem lit2 = new ListItem();
                    lit2.Value = schedaDocumento.registro.systemId;
                    lit2.Text = schedaDocumento.registro.codRegistro + " - " + schedaDocumento.registro.descrizione;
                    this.ddl_regRF.Items.Add(lit2);
                }
                else
                {
                    ListItem lit = new ListItem();
                    lit.Value = "";
                    lit.Text = Utils.Languages.GetLabelFromCode("DocumentNoteSelectAnRF", UIManager.UserManager.GetUserLanguage());
                    this.ddl_regRF.Items.Add(lit);
                    ListItem lit2 = new ListItem();
                    lit2.Value = schedaDocumento.registro.systemId;
                    lit2.Text = schedaDocumento.registro.codRegistro + " - " + schedaDocumento.registro.descrizione;
                    this.ddl_regRF.Items.Add(lit2);
                    
                }


                    foreach (DocsPaWR.Registro regis in listaRF)
                    {
                       
                            ListItem li = new ListItem();
                            li.Value = regis.systemId;
                            li.Text = regis.codRegistro + " - " + regis.descrizione;
                            this.ddl_regRF.Items.Add(li);                       
                    }

                
            }
        }

        protected void SelectRFBtnChiudi_Click(object sender, EventArgs e)
        {
            Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('SelectRF', '');</script></body></html>");
            Response.End();
        }

        protected void SelectRFBtnOk_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                if (this.ddl_regRF.SelectedValue.Length > 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('SelectRF','" + this.ddl_regRF.SelectedValue + "');", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
    }
}