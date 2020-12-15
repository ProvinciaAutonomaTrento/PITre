using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool.Gestione_Conservazione
{
    public partial class Conservazione : System.Web.UI.Page
    {
        private DocsPaWR.DocsPaWebService _wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GestioneGrafica();
                if (this.DisplayMenuPARER())
                {
                    this.pnlAttivaCons.Visible = true;
                    this.btn_monitoring.OnClientClick = "ReportMonitoraggioPolicy();";
                    this.FetchData();
                }
            }
        }

        protected void GestioneGrafica()
        {
            if (!IsPostBack)
            {
                lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
            }
        }

        protected void FetchData()
        {
            string idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

            this.FetchStatoConservazione(idAmministrazione);

            string idGruppo = this._wsInstance.GetIdRuoloRespConservazione(idAmministrazione.ToString());
            string idUtenteResp = this._wsInstance.GetIdUtenteRespConservazione(idAmministrazione.ToString());

            if (!string.IsNullOrEmpty(idGruppo) && !string.IsNullOrEmpty(idUtenteResp))
            {
                DocsPaWR.Ruolo r = UserManager.getRuoloByIdGruppo(idGruppo, this.Page);
                DocsPaWR.Utente u = UserManager.GetUtenteByIdPeople(idUtenteResp);

                if (r != null && u != null)
                {
                    this.lbl_resp_cons.Text = string.Format("{0} ({1})", u.descrizione, r.descrizione);
                }
                else
                {
                    this.lbl_resp_cons.Text = "non definito";
                    this.lbl_resp_cons.ForeColor = System.Drawing.Color.Red;
                    this.lbl_resp_cons.Font.Bold = true;
                }
            }
            else
            {
                this.lbl_resp_cons.Text = "non definito";
                this.lbl_resp_cons.ForeColor = System.Drawing.Color.Red;
                this.lbl_resp_cons.Font.Bold = true;
            }

            this.SetPolicyCounter(idAmministrazione);

        }

        protected void FetchStatoConservazione(string idAmministrazione)
        {
            bool isConservazioneAttiva = this._wsInstance.GetStatoAttivazione(idAmministrazione);

            if (isConservazioneAttiva)
            {
                this.btn_attiva.Text = "Disattiva";
                this.btn_attiva.ToolTip = "Disattiva l'invio in conservazione per l'amministrazione";
                this.lbl_status.Text = "ATTIVA";
                this.lbl_status.ForeColor = System.Drawing.Color.Green;
                this.lbl_status.Font.Bold = true;
            }
            else
            {
                this.btn_attiva.Text = "Attiva";
                this.btn_attiva.ToolTip = "Attiva l'invio in conservazione per l'amministrazione";
                this.lbl_status.Text = "NON ATTIVA";
                this.lbl_status.ForeColor = System.Drawing.Color.Red;
                this.lbl_status.Font.Bold = true;
            }

        }

        protected void SetPolicyCounter(string idAmm)
        {
            try
            {
                this.lbl_policyDoc.Text = this._wsInstance.GetListaPolicyPARER(idAmm, "D") != null ? this._wsInstance.GetListaPolicyPARER(idAmm, "D").Where(p => p.isAttiva == "1").Count().ToString() : string.Empty;
                this.lbl_policyStampe.Text = this._wsInstance.GetListaPolicyPARER(idAmm, "S") != null ? this._wsInstance.GetListaPolicyPARER(idAmm, "S").Where(p => p.isAttiva == "1").Count().ToString() : string.Empty;
            }
            catch (Exception)
            {

            }
        }

        protected void btn_attiva_Click(object sender, EventArgs e)
        {
            string stato;
            if(this.btn_attiva.Text == "Attiva")
            {
                stato = "1";
            }
            else
            {
                stato = "0";
            }

            string idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            bool result = this._wsInstance.SetStatoAttivazione(idAmministrazione, stato);

            if(result)
            {
                this.FetchStatoConservazione(idAmministrazione);
                if(stato == "1" && this.lbl_resp_cons.Text == "non definito")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "save_warning", "alert('Per il funzionamento del processo è necessario definire un responsabile della conservazione');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "save_error", "alert('Si è verificato un errore nel cambiamento dello stato');", true);
            }
        }

        protected bool DisplayMenuPARER()
        {
            bool result = false;

            string FE_WA_CONSERVAZIONE = string.Empty;
            FE_WA_CONSERVAZIONE = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_WA_CONSERVAZIONE");
            if (!string.IsNullOrEmpty(FE_WA_CONSERVAZIONE) && FE_WA_CONSERVAZIONE.Equals("1"))
                result = true;
            else
                result = false;

            return result;

        }
    }
}