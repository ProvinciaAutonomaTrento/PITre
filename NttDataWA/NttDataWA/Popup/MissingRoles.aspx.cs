using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class MissingRoles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializeLanguage();
                Session["FromMassive"] = true;
                this.ListaRagioni = TrasmManager.getListaRagioni(this, ProjectManager.getProjectInSession().accessRights, false);
            }

            //this.UpdMissingRolesGrid.Update();
            //this.UpPnlButtons.Update();
            this.InitializePage();
            this.UpdMissingRolesGrid.Update();
            this.UpPnlButtons.Update();
            this.RefreshScript();
        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.MissingRolesBtnTransmit.Text = Utils.Languages.GetLabelFromCode("MissingRolesTransmit", language);
            this.MissingRolesBtnCancel.Text = Utils.Languages.GetLabelFromCode("MissingRolesCancel", language);
            this.lblMissingRolesSelectRole.Text = Utils.Languages.GetLabelFromCode("MissingRolesSelectRole", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("BaseMasterAddressBook", language);
        }

        private void InitializePage()
        {
            this.CallType = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
            int index = 0;

            foreach (string mr in this.MissingRolesList)
            {
                string reason = mr.Split('(')[1].Trim();
                reason = reason.Substring(0, reason.Length - 1);
                bool selectReason = false;

                if (reason.ToUpper() == "AUTHORIZER" || reason.ToUpper() == "PROPOSER")
                {
                    selectReason = false;
                }
                else
                {
                    RagioneTrasmissione checkReason = this.ListaRagioni.Where(x => x.descrizione == reason).FirstOrDefault();
                    if (checkReason == null)
                    {
                        selectReason = true;
                    }
                }
                
                this.InsertRole(mr, index, selectReason);
                index++;
            }
            //this.UpdMissingRolesGrid.Update();

        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        protected void MissingRolesBtnCancel_Click(object sender, EventArgs e)
        {
            this.CloseMask();
        }

        protected void MissingRolesBtnTransmit_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            ArrayList roleReasonsList = new ArrayList();

            // Check data
            foreach (Control ctrl in this.PnlCorrespondent.Controls)
            {
                if (ctrl is UserControls.CorrespondentCustom)
                {
                    UserControls.CorrespondentCustom corr = (UserControls.CorrespondentCustom)ctrl;
                    if (!string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom) && !string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
                    {
                        DropDownList ddl = (DropDownList)PnlCorrespondent.FindControl("ddlReason_" + corr.ID);
                        string reason = corr.TxtEtiCustomCorrespondent.Split('(')[1];
                        if (ddl != null)
                        {
                            reason = ddl.SelectedItem.Text + "§" + reason;
                        }
                        roleReasonsList.Add(corr.TxtCodeCorrespondentCustom + " (" + reason);
                    }
                    else
                    {
                        string msg = "WarningMissingRolesRequired";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msg + "', 'warning', '');", true);
                        break;
                    }
                }
            }

            if (roleReasonsList.Count > 0)
            {
                // Creo le trasmissioni ai corrispondenti selezionati
                DiagrammiManager.CreateTransmissionsMissingRoles((string[])roleReasonsList.ToArray(typeof(string)), this.SelectedState.SYSTEM_ID.ToString());
                this.SalvaStato();
                this.SelectedState = null;

                this.CloseMask();
            }
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            try
            {
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> ccList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.Cc"];
                string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

                if (atList != null && atList.Count > 0)
                {
                    Corrispondente corr = null;
                    //Profiler document
                    UserControls.CorrespondentCustom userCorr = (UserControls.CorrespondentCustom)this.PnlCorrespondent.FindControl(this.IdCustomObjectCustomCorrespondent);

                    string idAmm = UIManager.UserManager.GetInfoUser().idAmministrazione;
                    NttDataWA.Popup.AddressBook.CorrespondentDetail addressBookCorrespondent = (NttDataWA.Popup.AddressBook.CorrespondentDetail)atList[0];

                    if (!addressBookCorrespondent.isRubricaComune)
                    {
                        corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(addressBookCorrespondent.SystemID);
                    }
                    else
                    {
                        corr = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(addressBookCorrespondent.CodiceRubrica);
                    }

                    userCorr.TxtCodeCorrespondentCustom = corr.codiceRubrica;
                    userCorr.TxtDescriptionCorrespondentCustom = corr.descrizione;
                    userCorr.IdCorrespondentCustom = corr.systemId;
                }

                HttpContext.Current.Session["AddressBook.At"] = null;
                HttpContext.Current.Session["AddressBook.Cc"] = null;
                this.UpdMissingRolesGrid.Update();
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
            
        }

        private void CloseMask()
        {
            Session["FromMassive"] = null;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('MissingRoles', '');} else {parent.closeAjaxModal('MissingRoles', '');};", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InsertRole(string role, int index, bool reason)
        {
            UserControls.CorrespondentCustom corrispondente = (UserControls.CorrespondentCustom)this.LoadControl("../UserControls/CorrespondentCustom.ascx");
            corrispondente.EnableViewState = true;
            corrispondente.PageCaller = "Popup";
            corrispondente.TxtEtiCustomCorrespondent = role;
            corrispondente.ID = index.ToString();
            corrispondente.TypeCorrespondentCustom = "MISSING_ROLES";
            PnlCorrespondent.Controls.Add(corrispondente);

            if (reason)
            {
                string language = UIManager.UserManager.GetUserLanguage();

                DropDownList ddlReason = new DropDownList();
                ddlReason.EnableViewState = true;
                ddlReason.ID = "ddlReason_" + index.ToString();

                for (int i = 0; i < this.ListaRagioni.Length; i++)
                {
                    ddlReason.Items.Add(new ListItem(this.ListaRagioni[i].descrizione, this.ListaRagioni[i].systemId));
                }
                ddlReason.Items.Insert(0, new ListItem());
                ddlReason.CssClass = "chzn-select-deselect";

                ddlReason.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("MissingRolesReason", language));
                PnlCorrespondent.Controls.Add(ddlReason);
            }
        }

        private void SalvaStato()
        {
            try
            {
                Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                DiagrammiManager.salvaModificaStatoFasc(fascicolo.systemID, this.SelectedState.SYSTEM_ID.ToString(), this.StateDiagram, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), string.Empty);

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
            catch (Exception ex)
            {

            }
        }

        #region Properties
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

        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_PROTO_INT_DEST;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }

        }

        public string IdCustomObjectCustomCorrespondent
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] != null)
                {
                    result = HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] as string;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] = value;
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

        public RagioneTrasmissione[] ListaRagioni
        {
            get
            {
                RagioneTrasmissione[] result = null;
                if (HttpContext.Current.Session["listaRagioniTrasm"] != null)
                {
                    result = HttpContext.Current.Session["listaRagioniTrasm"] as RagioneTrasmissione[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listaRagioniTrasm"] = value;
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
        #endregion
    }
}