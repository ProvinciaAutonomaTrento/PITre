using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.AdminTool.Gestione_ProfDinamica
{
    public partial class GestioneStoricoProfDinamica : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Caricamento delle informazioni sullo stato di abilitazione dello
            // storico relativamente ai campi della tipologia attualemnte in editing
            if (!IsPostBack)
            {
                // La pagina deve essere considerata scaduta non appena viene caricata dal browser
                Response.Expires = -1;

                // Caricamento dello stato di abilitazione dello storico sui campi relativi al template
                DocsPAWA.DocsPaWR.Templates template = Session["template"] as DocsPAWA.DocsPaWR.Templates;
                SelectiveHistoryResponse response = null;
                if (Request["objType"] == "D")
                    response = ProfilazioneDocManager.GetCustomHistoryList(template.SYSTEM_ID.ToString());
                else
                    response = ProfilazioneFascManager.GetCustomHistoryList(template.SYSTEM_ID.ToString());

                this.dgFields.DataSource = response.Fields;
                this.dgFields.DataBind();

                // Se tutti i campi sono selezionati, viene flaggato "Seleziona tutti"
                this.chkSelectDeselectAll.Checked = response.Fields.Count(f => f.Enabled) == response.Fields.Length;

                // Se non ci sono campi, viene visualizzato un messaggio e viene chiusa la finestra
                if (response.Fields.Length == 0)
                {
                    AjaxMessageBox.ShowMessage("Non è stato rilevato alcun campo per cui è possibile abilitare lo storico");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Close", "window.close();", true);
                }
            }
        }

        /// <summary>
        /// Metodo per la generazione dello script per l'apertura della finestra
        /// </summary>
        /// <returns></returns>
        public static String GetOpenScript(ObjectType objType)
        {
            return String.Format(
                "window.showModalDialog('{0}/AdminTool/Gestione_ProfDinamica/GestioneStoricoProfDinamica.aspx?objType={1}', '', 'dialogWidth:588px;dialogHeight:350px; resizable: no;status:no;scroll:yes;help:no;close:no;center:yes;');",
                Utils.getHttpFullPath(), objType == ObjectType.Document ? "D" : "F");

        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            // Costruzione della lista di oggetti con le informazioni sui campi
            List<CustomObjHistoryState> customObjectsState = new List<CustomObjHistoryState>();

            foreach (var item in this.dgFields.Items)
                if(((CheckBox)(((DataGridItem)item).FindControl("chkEnabled"))).Checked)
                    customObjectsState.Add(new CustomObjHistoryState()
                        {
                            FieldId = Convert.ToInt32(((HiddenField)(((DataGridItem)item).FindControl("hfObjectId"))).Value),
                            Enabled = true
                        });

            SelectiveHistoryResponse response = null;
            // Salvataggio dei dati
            if (Request["objType"] == "D")
                response = ProfilazioneDocManager.ActiveSelectiveHistory(this.chkSelectDeselectAll.Checked, customObjectsState.ToArray(), ((DocsPAWA.DocsPaWR.Templates)Session["template"]).SYSTEM_ID.ToString());
            else
                response = ProfilazioneFascManager.ActiveSelectiveHistory(this.chkSelectDeselectAll.Checked, customObjectsState.ToArray(), ((DocsPAWA.DocsPaWR.Templates)Session["template"]).SYSTEM_ID.ToString());

            if (!response.Result)
                this.AjaxMessageBox.ShowMessage("Si è verificato un errore durante il salvataggio dei dati");
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Close", "window.close();", true);
        }
    }
}