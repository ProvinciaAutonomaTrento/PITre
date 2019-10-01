using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using System.Web.UI.HtmlControls;

namespace NttDataWA.Popup
{
    public partial class Phases : System.Web.UI.Page
    {
        public string SelectedPhaseId
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["SelectedPhaseId"] != null)
                {
                    result = HttpContext.Current.Session["SelectedPhaseId"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SelectedPhaseId"] = value;
            }
        }
        public Stato SelectedState
        {
            get
            {
                Stato result = null;
                if (HttpContext.Current.Session["SelectedState"] != null)
                {
                    result = HttpContext.Current.Session["SelectedState"] as Stato;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SelectedState"] = value;
            }
        }

        private DiagrammaStato StateDiagram
        {
            get
            {
                DiagrammaStato result = null;
                if (HttpContext.Current.Session["stateDiagramProject"] != null)
                {
                    result = HttpContext.Current.Session["stateDiagramProject"] as DiagrammaStato;
                }
                return result;
            }
        }

        private ArrayList MissingRolesList
        {
            get
            {
                ArrayList result = null;
                if (HttpContext.Current.Session["changeStateMissingRoles"] != null)
                {
                    result = HttpContext.Current.Session["changeStateMissingRoles"] as ArrayList;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["changeStateMissingRoles"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.InitializeLanguage();
                this.InitializePage();
            }
            else
            {
                if (!string.IsNullOrEmpty(this.HiddenSelectedState.Value))
                {
                    this.SelectedState = DiagrammiManager.GetStatoById(this.HiddenSelectedState.Value);
                    this.HiddenSelectedState.Value = string.Empty;
                    string msgConfirm = "WarningPhaseChangeStateDiagramm";
                    if (this.controllaStatoFinale())
                    {
                        msgConfirm = "WarningPhaseChangeStateDiagramFinalState";
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenChangeStateDiagramm', '', '" + SelectedState.DESCRIZIONE + "');", true);
                    return;
                }
                if (!string.IsNullOrEmpty(this.HiddenChangeStateDiagramm.Value))
                {
                    this.HiddenChangeStateDiagramm.Value = string.Empty;
                    string retVal = this.SelectedState.SYSTEM_ID.ToString();
                    this.MissingRolesList = DiagrammiManager.ChangeStateGetMissingRoles(UIManager.ProjectManager.getProjectInSession().systemID, this.SelectedState.SYSTEM_ID.ToString());
                    if (this.MissingRolesList == null || this.MissingRolesList.Count == 0)
                    {
                        this.SalvaStato();
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('Phases', '" + retVal + "');} else {parent.closeAjaxModal('Phases', '" + retVal + "');};", true);
                    this.SelectedState = null;
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            }
        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.PhaseClose.Text = Utils.Languages.GetLabelFromCode("PhaseClose", language);
            this.lblSelectState.Text = Utils.Languages.GetLabelFromCode("PhaseLblSelectSate", language);
            this.ltlCurrentState.Text = Utils.Languages.GetLabelFromCode("PhaseLtlCurrentState", language);
            this.ltlSelectedPhase.Text = Utils.Languages.GetLabelFromCode("PhaseLtlSelectedPhase", language);
            this.ltlCorrespondentStates.Text = Utils.Languages.GetLabelFromCode("PhaseLtlCorrespondentStates", language);
        }

        private void InitializePage()
        {
            Fascicolo fasc = UIManager.ProjectManager.getProjectInSession();
            List<AssPhaseStatoDiagramma> phasesState = UIManager.DiagrammiManager.GetFaseDiagrammaByIdFase(this.StateDiagram.SYSTEM_ID.ToString(), this.SelectedPhaseId.ToString());
            DocsPaWR.Stato stato = DiagrammiManager.getStatoFasc(fasc.systemID);
            
            this.lblCurrentState.Text = stato.DESCRIZIONE;
            this.lblSelectedPhase.Text = (phasesState[0].PHASE as DocsPaWR.Phases).DESCRIZIONE;

            List<string> idStatiSuccessiviSelezionabili = new List<string>();

            for (int i = 0; i < this.StateDiagram.PASSI.Length; i++)
            {
                DocsPaWR.Passo step = (DocsPaWR.Passo)this.StateDiagram.PASSI[i];
                if (step.STATO_PADRE.SYSTEM_ID == stato.SYSTEM_ID)
                {
                    for (int j = 0; j < step.SUCCESSIVI.Length; j++)
                    {
                        DocsPaWR.Stato st = (DocsPaWR.Stato)step.SUCCESSIVI[j];
                        if (DiagrammiManager.IsRuoloAssociatoStatoDia(this.StateDiagram.SYSTEM_ID.ToString(), UIManager.RoleManager.GetRoleInSession().idGruppo, st.SYSTEM_ID.ToString()))
                        {
                            if (!st.STATO_SISTEMA)
                                idStatiSuccessiviSelezionabili.Add(st.SYSTEM_ID.ToString());
                        }
                    }
                }
            }

            foreach (AssPhaseStatoDiagramma a in phasesState)
            {
                HtmlGenericControl divLinkStato =  new HtmlGenericControl("DIV");

                if(!idStatiSuccessiviSelezionabili.Contains(a.STATO.SYSTEM_ID.ToString()))
                {
                    Label lbl = new Label();
                    lbl.Text = a.STATO.DESCRIZIONE;
                    lbl.ID = a.STATO.SYSTEM_ID.ToString();
                    lbl.Attributes.Add("class", "disabled");
                    divLinkStato.Controls.Add(lbl);
                }
                else
                {
                    LinkButton lnk = new LinkButton();
                    lnk.Attributes.Add("href", "#");
                    lnk.Text = a.STATO.DESCRIZIONE;
                    lnk.CssClass = "clickable";
                    lnk.ID = a.STATO.SYSTEM_ID.ToString();
                    lnk.Attributes.Add("onclick", "$('#HiddenSelectedState').val('" + a.STATO.SYSTEM_ID + "'); disallowOp('ContentPlaceHolderContent');__doPostBack('UpPnlHiddenField');return false;"); 
                    divLinkStato.Controls.Add(lnk);
                }
                pnlCorrespondentStates.Controls.Add(divLinkStato);
            }
        }

        protected void PhaseClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('Phases', '');} else {parent.closeAjaxModal('Phases', '');};", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void SalvaStato()
        {
            Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
            bool statoFinale = this.controllaStatoFinale();
            DiagrammiManager.salvaModificaStatoFasc(fascicolo.systemID, this.SelectedState.SYSTEM_ID.ToString(), this.StateDiagram, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), string.Empty);

            if (statoFinale)
            {
                this.chiudiFascicolo(fascicolo, UserManager.GetInfoUser(), RoleManager.GetRoleInSession());
            }

            //if (this.PnlStateDiagram.Visible && !string.IsNullOrEmpty(this.DocumentStateDiagramDataValue.Text) && fascicolo.template.SYSTEM_ID != null)
            //{
            //    DiagrammiManager.salvaDataScadenzaFasc(fascicolo.systemID, this.DocumentStateDiagramDataValue.Text, fascicolo.template.SYSTEM_ID.ToString());
            //    fascicolo.dtaScadenza = this.DocumentStateDiagramDataValue.Text;
            //    UIManager.ProjectManager.setProjectInSession(fascicolo);
            //}

            //Verifico se effettuare una tramsissione automatica assegnata allo stato
            if (fascicolo.template != null && fascicolo.template.SYSTEM_ID != 0)
            {
                ArrayList modelli = new ArrayList(DiagrammiManager.isStatoTrasmAutoFasc(UserManager.GetInfoUser().idAmministrazione, this.SelectedState.SYSTEM_ID.ToString(), fascicolo.template.SYSTEM_ID.ToString()));
                for (int i = 0; i < modelli.Count; i++)
                {
                    DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                    if (mod.SINGLE == "1")
                    {
                        TrasmManager.effettuaTrasmissioneFascDaModello(mod, this.SelectedState.SYSTEM_ID.ToString(), fascicolo, this);
                    }
                    else
                    {
                        for (int k = 0; k < mod.MITTENTE.Length; k++)
                        {
                            if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == RoleManager.GetRoleInSession().systemId)
                            {
                                TrasmManager.effettuaTrasmissioneFascDaModello(mod, this.SelectedState.SYSTEM_ID.ToString(), fascicolo, this);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private bool controllaStatoFinale()
        {
            if (this.StateDiagram != null)
            {
                for (int i = 0; i < this.StateDiagram.STATI.Length; i++)
                {
                    DocsPaWR.Stato st = (DocsPaWR.Stato)this.StateDiagram.STATI[i];
                    if (st.SYSTEM_ID.ToString() == this.SelectedState.SYSTEM_ID.ToString() && st.STATO_FINALE)
                        return true;
                }
            }
            return false;
        }

        private void chiudiFascicolo(Fascicolo fascicolo, InfoUtente infoutente, Ruolo ruolo)
        {
            string msg = string.Empty;
            fascicolo.chiusura = DateTime.Now.ToShortDateString();
            fascicolo.stato = "C";
            if (fascicolo.chiudeFascicolo == null)
            {
                fascicolo.chiudeFascicolo = new DocsPaWR.ChiudeFascicolo();
            }
            fascicolo.chiudeFascicolo.idPeople = infoutente.idPeople;
            fascicolo.chiudeFascicolo.idCorrGlob_Ruolo = ruolo.systemId;
            fascicolo.chiudeFascicolo.idCorrGlob_UO = ruolo.uo.systemId;
            fascicolo = UIManager.ProjectManager.setFascicolo(fascicolo, infoutente);
            if (fascicolo != null)
            {
                //msg = "InfoProjectChiusuraFascicolo";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'info');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'check'}", true);
                UIManager.ProjectManager.setProjectInSession(fascicolo);
            }
        }

    }
}