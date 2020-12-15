using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class HistoryProject : System.Web.UI.Page
    {
        protected DocsPaWR.DocumentoLogDocumento[] ListaLogFascicolo;
        protected DocsPaWR.StoricoProfilati[] ListaTipologia;

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializeLanguage();
                    this.InitializePage();
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
            this.HistoryBtnChiudi.Text = Utils.Languages.GetLabelFromCode("HistoryBtnChiudi", language);
            this.HistoryProjectLblObject.Text = Utils.Languages.GetLabelFromCode("HistoryProjectLblObject", language);
        }
        protected void InitializePage()
        {
            if (this.TypeHistory != null && !string.IsNullOrEmpty(this.TypeHistory))
            {
                switch (this.TypeHistory.ToLower())
                {
                    case "projectimghistorystate":
                        this.HistoryProjectLblState.Text = Utils.Languages.GetLabelFromCode("HistoryLblState", UIManager.UserManager.GetUserLanguage());
                        CreateDataGridHistoryStato();
                        break;
                    case "projectimghistorytipology":
                        this.HistoryProjectLblObject.Text = Utils.Languages.GetLabelFromCode("HistoryLblTipology", UIManager.UserManager.GetUserLanguage());
                        CreateDataGridHistoryTipology();
                        break;
                    default:
                        CreateDataGridHistoryProject();
                        break;
                }
            }
            else
            {
                CreateDataGridHistoryProject();
            }
        }

        protected string GetDate(string valDate)
        {
            string result = valDate;
            return result;
        }

        protected string GetOperatore(string valOperatore)
        {
            string result = valOperatore;
            return result;
        }

        protected string GetGruppo(string valGruppo)
        {
            string result = "(" + valGruppo + ")";
            return result;
        }

        protected string GetAzione(string valAzione)
        {
            string result = valAzione;
            return result;
        }

        protected string GetRule(string valRule)
        {
            string result = valRule;
            return result;
        }

        protected string GetUser(string valUser)
        {
            string result = valUser;
            return result;
        }

        protected string GetModify(string valModify)
        {
            string result = valModify;
            return result;
        }

        protected void CreateDataGridHistoryTipology()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            Fascicolo Project = UIManager.ProjectManager.getProjectInSession();
            if (Project != null && !string.IsNullOrEmpty(Project.systemID))
            {
                ListaTipologia = ProjectManager.getStoriaProfilatiFasc(Project.template, Project.systemID.ToString());

                if (ListaTipologia != null)
                {
                    if (ListaTipologia.Length > 0)
                    {
                        var filtered_data = (from f in ListaTipologia
                                             select new
                                             {
                                                 Data = GetDate(f.dta_modifica),
                                                 Operatore = "<strong>" + GetUser(f.utente.descrizione) + "</strong><br />" + GetRule(f.ruolo.descrizione),
                                                 //Modifica = GetModify(f.var_desc_modifica),
                                                 Azione = GetModify(f.oggetto.DESCRIZIONE + ": " + f.var_desc_modifica),
                                                 Tooltip0 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell0", language),
                                                 Tooltip1 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell1", language),
                                                 Tooltip2 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell3", language)
                                             }).ToArray();

                        this.GridViewHistoryProject.DataSource = filtered_data;
                        this.GridViewHistoryProject.DataBind();
                        Session["filtered_data"] = filtered_data;

                        this.GridViewHistoryProject.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("HistoryProjectHeaderGrid0", language);
                        this.GridViewHistoryProject.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("HistoryProjectHeaderGrid1", language);
                        this.GridViewHistoryProject.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("HistoryProjectHeaderGrid2", language);
                    }
                    else
                    {
                        this.lblDettagli.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                        this.lblDettagli.Visible = true;
                    }
                }
            }
            else
            {
                this.lblDettagli.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                this.lblDettagli.Visible = true;
            }
        }

        protected void CreateDataGridHistoryStato()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            Fascicolo Project = UIManager.ProjectManager.getProjectInSession();
            if (Project != null && !string.IsNullOrEmpty(Project.systemID))
            {

                System.Data.DataSet ds = DiagrammiManager.getDiagrammaStoricoFasc(Project.systemID);

                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    this.GridViewStoricoStati.DataSource = ds;
                    this.DatagridStoricoStati.Visible = true;
                    this.GridViewDocHistoryPnl.Visible = false;
                    //this.lblDettagliStato.Visible = false;
                    this.lblDettagli.Visible = false;
                    this.GridViewStoricoStati.DataBind();

                    this.GridViewStoricoStati.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid1", language);
                    this.GridViewStoricoStati.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid2", language);
                    this.GridViewStoricoStati.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid0", language);
                    this.GridViewStoricoStati.HeaderRow.Cells[3].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid3", language);
                }
                else
                {
                    //this.lblDettagliStato.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                    //this.lblDettagliStato.Visible = true;
                    this.lblDettagli.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                    this.lblDettagli.Visible = true;
                    return;
                }
            }
            else
            {
                //this.lblDettagliStato.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                //this.lblDettagliStato.Visible = true;
                this.lblDettagli.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                this.lblDettagli.Visible = true;
                return;
            }

        }

        protected void CreateDataGridHistoryProject()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            Fascicolo Project = UIManager.ProjectManager.getProjectInSession();

            if (Project != null && !string.IsNullOrEmpty(Project.systemID))
            {
                ListaLogFascicolo = ProjectManager.getStoriaLog(Project.systemID, "FASCICOLO");


                if (ListaLogFascicolo != null)
                {
                    if (ListaLogFascicolo.Length > 0)
                    {
                        var filtered_data = (from f in ListaLogFascicolo
                                             select new
                                             {
                                                 Data = GetDate(f.dataAzione),
                                                 Operatore = "<strong>"+GetOperatore(f.userIdOperatore) + "</strong><br />" + GetGruppo(f.idGruppoOperatore),
                                                 Azione = GetAzione(f.descrOggetto),
                                                 Tooltip0 = Utils.Languages.GetLabelFromCode("HistoryProjectTooltipCell0", language),
                                                 Tooltip1 = Utils.Languages.GetLabelFromCode("HistoryProjectTooltipCell1", language),
                                                 Tooltip2 = Utils.Languages.GetLabelFromCode("HistoryProjectTooltipCell2", language)
                                             }).ToArray();

                        this.GridViewHistoryProject.DataSource = filtered_data;
                        Session["filtered_data"] = filtered_data;
                        this.GridViewHistoryProject.DataBind();

                        this.GridViewHistoryProject.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("HistoryProjectHeaderGrid0", language);
                        this.GridViewHistoryProject.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("HistoryProjectHeaderGrid1", language);
                        this.GridViewHistoryProject.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("HistoryProjectHeaderGrid2", language);
                    }                   
                } 
               
            } else
                    {
                        this.lblDettagli.Text = Utils.Languages.GetLabelFromCode("HistoryProjectLblDettagliNoMod", language);
                        this.lblDettagli.Visible = true;
                    }
        }          



             //Fasc = FascicoliManager.getFascicoloSelezionato(this);
             //       Folder = FascicoliManager.getFolder(this, Fasc);
             //       if (Fasc == null)
             //       {
             //           this.lb_dettagli.Text = "Errore nel reperimento dei dati del fascicolo";
             //           this.lb_dettagli.Visible = true;
             //           return;
             //       }

             //       // Se l'id del ruolo creatore del documento risulta storicizzato, viene mostrato un pulsante che mostra
             //       // la storia delle modifiche del ruolo
             //       if (Utils.CheckIfCreatorRoleIsDisabled(Fasc))
             //       {
             //           this.ibRoleHistory.Visible = true;
             //           this.ibRoleHistory.OnClientClick = popup.RoleHistory.GetScriptToOpenWindow(Fasc.creatoreFascicolo.idCorrGlob_Ruolo, Fasc.creatoreFascicolo.idCorrGlob_Ruolo);
             //       }
             //       else
             //           this.ibRoleHistory.Visible = false;

             //       this.Label2.Text = "Storia del fascicolo " + Fasc.codice;
             //       ListaLogFascicolo = FascicoliManager.getStoriaLog(this, Fasc.systemID, "FASCICOLO");
             //       ListaLogFolder = FascicoliManager.getStoriaLog(this, Folder.systemID, "FOLDER");

             //       if ((ListaLogFascicolo == null || (ListaLogFascicolo != null && ListaLogFascicolo.Length <= 0)) &&
             //           (ListaLogFolder == null || (ListaLogFolder != null && ListaLogFolder.Length <= 0)))
             //       {
             //           DataGrid1.Visible = false;
             //           this.lb_dettagli.Text = "Non ci sono attività che riguardano il fascicolo in esame"; ;
             //           this.lb_dettagli.Visible = true;
             //       }
             //       else
             //       {
             //           string data;
             //           string utente;
             //           string idPeopleOPeratore;
             //           string idGruppoOperatore;
             //           string idAmm;
             //           string modificaObj;
             //           string codAzione;
             //           string ruolo;
             //           string chaEsito;
             //           for (int i = 0; i < ListaLogFascicolo.Length; i++)
             //           {
             //               data = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).dataAzione;
             //               data = data.Replace(".", ":");
             //               utente = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).userIdOperatore;
             //               idPeopleOPeratore = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).idPeopleOPeratore;
             //               ruolo = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).idGruppoOperatore;
             //               idAmm = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).idAmm;
             //               modificaObj = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).descrOggetto;
             //               codAzione = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).codAzione;
             //               chaEsito = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFascicolo[i]).chaEsito;
             //               utente += " (" + ruolo + ")";
             //               //if (ruolo.Equals("1"))
             //               //    ruolo = "OK";
             //               //else
             //               //    ruolo = "KO";
             //               if(chaEsito.Equals("1"))
             //                   this.dataSetStoriaObj1.element1.Addelement1Row(data, ruolo, utente, modificaObj);
             //           }

             //           for (int i = 0; i < ListaLogFolder.Length; i++)
             //           {
             //               data = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).dataAzione;
             //               data = data.Replace(".", ":");
             //               utente = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).userIdOperatore;
             //               idPeopleOPeratore = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).idPeopleOPeratore;
             //               ruolo = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).idGruppoOperatore;
             //               idAmm = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).idAmm;
             //               modificaObj = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).descrOggetto;
             //               codAzione = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).codAzione;
             //               chaEsito = ((DocsPAWA.DocsPaWR.DocumentoLogDocumento)ListaLogFolder[i]).chaEsito;
             //               utente += " (" + ruolo + ")";
             //               //if (ruolo.Equals("1"))
             //               //    ruolo = "OK";
             //               //else
             //               //    ruolo = "KO";
             //               if(chaEsito.Equals("1"))
             //                   this.dataSetStoriaObj1.element1.Addelement1Row(data, ruolo, utente, modificaObj);
             //           }

             //           Session["Dg_Log"] = this.dataSetStoriaObj1.Tables[0];
             //           this.DataGrid1.DataSource = this.dataSetStoriaObj1.Tables[0];
             //           this.DataGrid1.DataBind();
             //       }
             //   }

        public string TypeHistory
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["typeHistory"] != null)
                {
                    result = HttpContext.Current.Session["typeHistory"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["typeHistory"] = value;
            }
        }

        protected void HistoryProjectBtnChiudi_Click(object sender, EventArgs e)
        {       
            try 
            {
                this.TypeHistory = string.Empty;
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeHistoryProject", "parent.closeAjaxModal('HistoryProject','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
        protected void GridViewHistoryProject__PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                GridViewHistoryProject.PageIndex = e.NewPageIndex;
                this.GridViewHistoryProject.DataSource = Session["filtered_data"];
                this.GridViewHistoryProject.DataBind();
                string language = UIManager.UserManager.GetUserLanguage();
                if (this.TypeHistory != null && !string.IsNullOrEmpty(this.TypeHistory))
                {
                    switch (this.TypeHistory.ToLower())
                    {
                        case "projectimghistorystate":
                            break;
                        case "projectimghistorytipology":
                            this.GridViewHistoryProject.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("HistoryProjectHeaderGrid0", language);
                            this.GridViewHistoryProject.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("HistoryProjectHeaderGrid1", language);
                            this.GridViewHistoryProject.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("HistoryProjectHeaderGrid2", language);
                            break;
                        default:
                            break;
                    }
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