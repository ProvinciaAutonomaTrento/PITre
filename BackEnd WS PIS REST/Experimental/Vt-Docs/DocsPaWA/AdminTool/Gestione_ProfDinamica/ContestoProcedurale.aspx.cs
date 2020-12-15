using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.AdminTool.Gestione_ProfDinamica
{
    public partial class ContestoProcedurale : System.Web.UI.Page
    {

        protected DocsPAWA.DocsPaWR.Templates modelloSelezionato;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadGridContestoProcedurale();
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            string systemIdContesto = string.Empty;

            foreach (var item in this.dg_ContestoProcedurale.Items)
                if (((RadioButton)(((DataGridItem)item).FindControl("rbSelezioneContesto"))).Checked)
                {
                    systemIdContesto = ((HiddenField)(((DataGridItem)item).FindControl("System_id"))).Value;
                    break;
                }
            if (!string.IsNullOrEmpty(systemIdContesto))
            {

                DocsPAWA.DocsPaWR.Templates template = Session["template"] as DocsPAWA.DocsPaWR.Templates;

                DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
                DocsPAWA.DocsPaWR.InfoUtente infoUtente = session.getUserAmmSession();
                if (ProfilazioneDocManager.UpdateAssociazioneTemplateContestoProcedurale(template.SYSTEM_ID.ToString(), systemIdContesto, infoUtente))
                {
                    template.ID_CONTESTO_PROCEDURALE = systemIdContesto;
                    System.Web.HttpContext.Current.Session["template"] = template;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Close", "window.close();", true);
                }
                else
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Si è verificato un errore durante il salvataggio dei dati');", true);
            }
        }

        protected void btnNuovoContesto_Click(object sender, EventArgs e)
        {
            this.pnlNuovoContesto.Visible = true;
            this.btnNuovoContesto.Enabled = false;

            this.upButtons.Update();
            this.UpNuovoContesto.Update();
        }

        protected void BtnChiudiNuovoContesto_Click(object sender, EventArgs e)
        {
            this.pnlNuovoContesto.Visible = false;
            this.btnNuovoContesto.Enabled = true;

            this.ClearField();

            this.upButtons.Update();
            this.UpNuovoContesto.Update();
        }

        protected void btn_AggiungiContesto_Click(object sender, EventArgs e)
        {
            //Verifico che tutti i campi siano stati inseriti
            if (string.IsNullOrEmpty(this.txt_tipoContesto.Text) || string.IsNullOrEmpty(this.txt_Nome.Text) ||
                string.IsNullOrEmpty(this.txt_famiglia.Text) || string.IsNullOrEmpty(this.txt_versione.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Tutti i campi sono obbligatori!');", true);
            }
            else
            {
                DocsPAWA.DocsPaWR.ContestoProcedurale contesto = new DocsPaWR.ContestoProcedurale();

                contesto.TIPO_CONTESTO_PROCEDURALE = this.txt_tipoContesto.Text;
                contesto.NOME = this.txt_Nome.Text;
                contesto.FAMIGLIA = this.txt_famiglia.Text;
                contesto.VERSIONE = this.txt_versione.Text;

                DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
                DocsPAWA.DocsPaWR.InfoUtente infoUtente = session.getUserAmmSession();

                if (ProfilazioneDocManager.InsertContestoProcedurale(contesto, infoUtente))
                {
                    LoadGridContestoProcedurale();
                    this.ClearField();
                    this.pnlNuovoContesto.Visible = false;
                    this.btnNuovoContesto.Enabled = true;

                    this.upButtons.Update();
                    this.UpNuovoContesto.Update();
                    this.UpDgContestoProcedurale.Update();
                }
                else
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Si è verificato un errore durante il salvataggio dei dati');", true);
            }
        }

        private void ClearField()
        {
            this.txt_tipoContesto.Text = string.Empty;
            this.txt_Nome.Text = string.Empty;
            this.txt_famiglia.Text = string.Empty;
            this.txt_versione.Text = string.Empty;
        }

        private void LoadGridContestoProcedurale()
        {
            DocsPAWA.DocsPaWR.Templates template = Session["template"] as DocsPAWA.DocsPaWR.Templates;

            DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = session.getUserAmmSession();

            List<DocsPAWA.DocsPaWR.ContestoProcedurale> listContestoProcedurale = ProfilazioneDocManager.GetListContestoProcedurale(infoUtente);

            if (listContestoProcedurale != null && listContestoProcedurale.Count > 0)
            {
                this.dg_ContestoProcedurale.DataSource = listContestoProcedurale;
                this.dg_ContestoProcedurale.DataBind();
            }

            if (!string.IsNullOrEmpty(template.ID_CONTESTO_PROCEDURALE))
            {
                foreach (var item in this.dg_ContestoProcedurale.Items)
                    if (((HiddenField)(((DataGridItem)item).FindControl("System_id"))).Value.Equals(template.ID_CONTESTO_PROCEDURALE))
                    {
                        ((RadioButton)(((DataGridItem)item).FindControl("rbSelezioneContesto"))).Checked = true;
                        break;
                    }
            }
        }
    }
}