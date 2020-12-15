using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using System.Data;
using NttDatalLibrary;
using System.Web.UI.HtmlControls;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDataWA.UserControls;
using System.Collections;
using System.Text.RegularExpressions;

namespace NttDataWA.Project
{
    public partial class Project : System.Web.UI.Page
    {

        #region constanti

        private const string DIRITTI = "2";
        private const string ATTIVO = "1";
        private const string PROCEDURALE = "P";
        private const string GENERALE = "G";
        private const string UP_DOCUMENT_BUTTONS = "upPnlButtons";
        private const string CLOSE_POPUP_ZOOM = "closeZoom";
        private const string CLOSE_POPUP_IMPORT_DOCUMENT = "closeImportDocument";
        private const string CLOSE_POPUP_OBJECT = "closePopupObject";
        private const string CLOSE_POPUP_CREATE_NEW_DOCUMENT = "closePopupCreateNewDocument";
        private const string CLOSE_DRAG_AND_DROP = "closeDragAndDrop";
        private const string PROTOCOLLO_IN_ARRIVO = "A";
        private const string POPUP_DRAG_AND_DROP = "POPUP_DRAG_AND_DROP";

        public static string componentType = Constans.TYPE_SMARTCLIENT;


        #endregion

        #region Remove property

        private SearchObject[] ListObjectNavigation
        {
            set
            {
                HttpContext.Current.Session["ListObjectNavigation"] = value;
            }
        }

        private void RemoveIsZoom()
        {
            HttpContext.Current.Session.Remove("isZoom");
        }

        protected Dictionary<String, FileToSign> ListToSign
        {
            get
            {
                Dictionary<String, FileToSign> result = null;
                if (HttpContext.Current.Session["listToSign"] != null)
                {
                    result = HttpContext.Current.Session["listToSign"] as Dictionary<String, FileToSign>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listToSign"] = value;
            }
        }

        private DescrizioneFascicolo DescrizioneFascicoloSelezionata
        {
            get
            {
                return HttpContext.Current.Session["DescrizioneFascicoloSelezionata"] as DescrizioneFascicolo;
            }
            set
            {
                HttpContext.Current.Session["DescrizioneFascicoloSelezionata"] = value;
            }
        }
        #endregion

        #region caricamento della pagina


        protected void Page_Load(object sender, EventArgs e)
        {
            //try {
            if (!IsPostBack)
            {
                this.TemplateProject = null;
                if (Request.QueryString.Count > 0 && (Request.QueryString["t"] != null && (Request.QueryString["t"].Equals("p") || Request.QueryString["t"].Equals("n") || Request.QueryString["t"].Equals("c"))))
                {
                    UIManager.ProjectManager.removeProjectInSession();
                }
                this.ClearSessionProperties();
                this.InitializePage();
                this.VisibiltyRoleFunctions();

                //***************************************************************
                //GIORDANO IACOZZILLI
                //17/07/2013
                //Gestione dell'icona della copia del docuemnto/fascicolo in deposito.
                //***************************************************************
                if (UIManager.ProjectManager.getProjectInSession() != null && UIManager.ProjectManager.getProjectInSession().inArchivio == "1")
                {
                    HeaderProject.FlagCopyInArchive = "1";
                }
                //***************************************************************
                //FINE
                //***************************************************************

                //ABBATANGELI - Fascicolo cancellato da utente
                if (UIManager.ProjectManager.getProjectInSession() != null && UIManager.ProjectManager.getProjectInSession().stato == "DELETED")
                {
                    UIManager.ProjectManager.setProjectInSession(null);
                    this.Request.QueryString["back"] = "1";
                }

                //Back
                if (this.Request.QueryString["back"] != null && this.Request.QueryString["back"].Equals("1"))
                {
                    List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                    Navigation.NavigationObject obj = navigationList.Last();
                    if (!obj.CodePage.Equals(Navigation.NavigationUtils.NamePage.PROJECT.ToString()))
                    {
                        obj = new Navigation.NavigationObject();
                        obj = navigationList.ElementAt(navigationList.Count - 2);
                    }
                    this.SearchFilters = obj.SearchFilters;
                    if (!string.IsNullOrEmpty(obj.NumPage))
                    {
                        this.SelectedPage = Int32.Parse(obj.NumPage);
                    }

                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, Label, obj.folder, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);

                    if (!string.IsNullOrEmpty(obj.OriginalObjectId))
                    {
                        string idProject = string.Empty;
                        foreach (GridViewRow grd in this.gridViewResult.Rows)
                        {
                            idProject = string.Empty;

                            if (GrigliaResult.Rows[grd.RowIndex]["IdProfile"] != null)
                            {
                                idProject = GrigliaResult.Rows[grd.RowIndex]["IdProfile"].ToString();
                            }

                            if (idProject.Equals(obj.OriginalObjectId))
                            {
                                this.gridViewResult.SelectRow(grd.RowIndex);
                                this.SelectedRow = grd.RowIndex.ToString();
                            }
                        }
                    }

                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();

                    // sottofascicolo
                    Fascicolo project = UIManager.ProjectManager.getProjectInSession();
                    if (project.systemID == project.folderSelezionato.systemID)
                        this.treenode_sel.Value = "root_" + project.folderSelezionato.systemID;
                    else
                    {
                        this.treenode_sel.Value = "node_" + project.folderSelezionato.systemID;
                        this.SelectedFolder();
                    }

                }

                if(Request.QueryString.Count > 0 && Request.QueryString["t"] != null && Request.QueryString["t"].Equals("c"))
                {
                    this.projectBtnSave.Visible = false;
                    this.projectBtnSaveAndAcceptDocument.Visible = true;
                }
            }
            else
            {
                if (((ScriptManager)Master.FindControl("ScriptManager1")).IsInAsyncPostBack)
                {
                    // detect action from async postback
                    switch (((ScriptManager)Master.FindControl("ScriptManager1")).AsyncPostBackSourceElementID)
                    {
                        case "upPnlGridList":
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                            break;
                    }
                }

                if (this.CustomProject)
                {
                    this.PnlTypeDocument.Controls.Clear();
                    if (!string.IsNullOrEmpty(this.projectDdlTipologiafascicolo.SelectedValue))
                    {
                        this.ProjectImgHistoryTipology.Visible = true;
                        if (this.TemplateProject == null || !this.TemplateProject.SYSTEM_ID.Equals(this.projectDdlTipologiafascicolo.SelectedValue))
                        {
                            if (UIManager.ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(UIManager.ProjectManager.getProjectInSession().systemID) && UIManager.ProjectManager.getProjectInSession().template != null && UIManager.ProjectManager.getProjectInSession().template.SYSTEM_ID != null && UIManager.ProjectManager.getProjectInSession().template.SYSTEM_ID != 0)
                            {
                                this.TemplateProject = UIManager.ProjectManager.getProjectInSession().template;
                            }
                            else
                            {
                                this.TemplateProject = ProfilerProjectManager.getTemplateFascById(this.projectDdlTipologiafascicolo.SelectedItem.Value);
                            }
                        }
                        if (this.CustomProject)
                        {
                            this.PopulateProfiledDocument();
                            this.LoadDiagramAndState();
                        }
                    }
                }

                this.setValuePopUp();
                this.UpnlAzioniMassive.Update();

                //ABBATANGELI - Ricarico la lista delle tipologie per evitare che aggiunto un nodo rimangano visibili tipologie con struttura.
                Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                if (fascicolo != null && fascicolo.folderSelezionato.childs != null)
                {
                    if (fascicolo.template != null && fascicolo.template.SYSTEM_ID == 0)
                    {
                        if (fascicolo.folderSelezionato.livello == "0")
                        {
                            fascicolo.folderSelezionato = ProjectManager.getFolder(this, fascicolo);
                        }
                        if (fascicolo.folderSelezionato.childs.Length > 0)
                        {
                            projectDdlTipologiafascicolo.Items.Clear();
                            this.loadTipologiaFascicolo(true);
                        }
                    }
                }

                if (this.Result != null && this.Result.Length > 0)
                {
                    // Visualizzazione dei risultati
                    this.SetCheckBox();

                    // Lista dei documenti risultato della ricerca
                    if (!this.SelectedPage.ToString().Equals(this.grid_pageindex.Value))
                    {
                        this.Result = null;
                        if (!string.IsNullOrEmpty(this.grid_pageindex.Value))
                        {
                            this.SelectedPage = int.Parse(this.grid_pageindex.Value);
                        }
                        if (!this.FromClassification)
                        {
                            this.SearchDocumentsAndDisplayResult(this.SearchFilters, SelectedPage, GridManager.SelectedGrid, Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                            this.BuildGridNavigator();
                            this.UpnlNumerodocumenti.Update();
                            this.UpnlGrid.Update();
                            this.upPnlGridIndexes.Update();
                        }
                        else
                        {
                            this.ShowInitDocument();
                        }

                    }
                    else
                    {
                        this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                    }
                }//Caso in cui Result è null(quindi parto da griglia vuota) però ho fatto l'import di documenti               
                else  if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_DOCUMENT_BUTTONS))
                {
                    if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_IMPORT_DOCUMENT)))
                    {
                        this.SearchDocumentsAndDisplayResult(this.SearchFilters, SelectedPage, GridManager.SelectedGrid, Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                        this.BuildGridNavigator();
                        this.UpnlNumerodocumenti.Update();
                        this.UpnlGrid.Update();
                        this.upPnlGridIndexes.Update();
                    }
                    if ((this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_CREATE_NEW_DOCUMENT))))
                    {
                        this.SearchDocumentsAndDisplayResult(this.SearchFilters, SelectedPage, GridManager.SelectedGrid, Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                        this.BuildGridNavigator();
                        this.UpnlNumerodocumenti.Update();
                        this.UpnlGrid.Update();
                        this.upPnlGridIndexes.Update();
                    }
                }

                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_OBJECT)))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_CreateNewDocument').contentWindow.closeObjectPopup();", true);
                }

                if (!string.IsNullOrEmpty(this.MassiveTransmissionAccept.ReturnValue))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveTransmissionAccept','');", true);
                    string accessRights = ProjectManager.GetAccessRightFascBySystemID(fascicolo.systemID);
                    fascicolo.accessRights = accessRights;
                    ProjectManager.setProjectInSession(fascicolo);
                    this.ProjectBtnAccept.Visible = ProjectManager.ExistsTrasmPendenteConWorkflowFascicolo(fascicolo.systemID, UIManager.RoleManager.GetRoleInSession().systemId, UserManager.GetUserInSession().idPeople);
                    switch ((HMdiritti)Enum.Parse(typeof(HMdiritti), fascicolo.accessRights))
                    {
                        case HMdiritti.HDdiritti_Waiting:
                            this.LblTipoDiritto.Text = Utils.Languages.GetLabelFromCode("VisibilityLabelWaiting", UserManager.GetUserLanguage());
                            break;
                        case HMdiritti.HMdiritti_Read:
                            this.LblTipoDiritto.Text = Utils.Languages.GetLabelFromCode("VisibilityLabelReadOnly", UserManager.GetUserLanguage());
                            break;
                        case HMdiritti.HMdiritti_Proprietario:
                        case HMdiritti.HMdiritti_Write:
                            this.LblTipoDiritto.Text = Utils.Languages.GetLabelFromCode("VisibilityLabelRW", UserManager.GetUserLanguage());
                            break;
                    }
                    if (!string.IsNullOrEmpty(fascicolo.accessRights) && Convert.ToInt32(fascicolo.accessRights) > Convert.ToInt32(HMdiritti.HMdiritti_Read))
                    {
                        this.projectBntChiudiFascicolo.Enabled = true;
                        if(fascicolo.stato.Equals("C"))
                            this.abilitazioneElementi(false, true);
                        else
                            this.abilitazioneElementi(true, false);
                        EnableEditMode();
                    }
                    this.UpContainer.Update();
                    this.UpDirittiFascicolo.Update();
                    this.upPnlButtons.Update();
                }
            }

            if (this.ShowGridPersonalization)
            {
                this.EnableDisableSave();
            }


            this.RefreshScripts();

            //rimuovo IsZoom alla chiusura della popup di zoom(da x o pulsante chiudi) quando richiamata da profilo, allegato, classifica
            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_DOCUMENT_BUTTONS))
            {
                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_ZOOM)))
                {
                    RemoveIsZoom();
                    UpnlAzioniMassive.Update();
                    UpnlTabHeader.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DocumentViewer','');", true);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(this.OpenTitolarioMassive.ReturnValue))
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "txt_CodFascicolo", "$('#MassiveCollate_panel>#ifrm_MassiveCollate').contents().find('.txt_addressBookLeft').val('" + utils.FormatJs(this.ReturnValue.Split('#').First()) + "');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "txt_DescFascicolo", "$('#MassiveCollate_panel>#ifrm_MassiveCollate').contents().find('#txt_DescFascicolo').val('" + utils.FormatJs(this.ReturnValue.Split('#').Last()) + "');", true);
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenTitolario','');", true);
            }
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_DOCUMENT_BUTTONS))
            {
                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(POPUP_DRAG_AND_DROP)))
                {
                    DragAndDropManager.ClearReport();

                    return;
                }
            }
            if(!string.IsNullOrEmpty(this.DescriptionProjectList.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DescriptionProjectList','');", true);
                if (this.DescrizioneFascicoloSelezionata != null)
                {
                    this.projectTxtDescrizione.Text = this.DescrizioneFascicoloSelezionata.Descrizione;
                    this.DescrizioneFascicoloSelezionata = null;
                    this.upnBtnTitolario.Update();
                }
                return;
            }
        }

        protected void EnableDisableSave()
        {
            if (GridManager.SelectedGrid != null && string.IsNullOrEmpty(GridManager.SelectedGrid.GridId))
            {
                this.projectImgSaveGrid.Enabled = true;
            }
            else
            {
                this.projectImgSaveGrid.Enabled = false;
            }
        }

        private void VisibiltyRoleFunctions()
        {

            if (!UIManager.UserManager.IsAuthorizedFunctions("IMP_DOC_MASSIVA"))
            {
                this.projectImgImportDoc.Visible = false;
                this.projectImgImportDocApplet.Visible = false;
                this.projectImgImportDocSocket.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("EXP_DOC_MASSIVA"))
            {
                this.projectImgExportDoc.Visible = false;
                this.projectImgExportDocApplet.Visible = false;
                this.projectImgExportDocSocket.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION"))
            {
                this.projectImgSaveGrid.Visible = false;
                this.projectImgEditGrid.Visible = false;
                this.projectImgPreferredGrids.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_APRICHIUDI"))
            {
                this.projectBntChiudiFascicolo.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("ABILITA_CANCELLAZIONE_FASCICOLO"))
            {
                this.projectBntCancellaFascicolo.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_ADD_ADL"))
            {
                this.projectBtnAdL.Visible = false;
                this.projectBtnAdlRole.Visible = false;
            }

            //Controllo se il ruolo utente è autorizzato a creare documenti privati
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_FASC_PRIVATO"))
            {
                this.ProjectCheckPrivate.Visible = false;
                this.UpProjectPrivate.Update();
            }

            //Controllo se il ruolo utente è autorizzato a creare documenti privati
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_FASC_PUBBLICO"))
            {
                this.ProjectCheckPublic.Enabled = false;
                this.UpProjectPublic.Update();
            }


            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_NEW_FOLDER"))
            {
                this.ImgFolderAdd.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_MOD_FOLDER"))
            {
                this.ImgFolderModify.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_DEL_FOLDER"))
            {
                this.ImgFolderRemove.Visible = false;
            }

            if (!UIManager.AdministrationManager.IsEnableRF(UserManager.GetInfoUser().idAmministrazione) || !UIManager.UserManager.IsAuthorizedFunctions("INSERIMENTO_NOTERF"))
            {
                if (this.RblTypeNote.Items.Count > 3)
                {
                    this.RblTypeNote.Items.RemoveAt(2);
                }
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
            {
                this.AllowADLRole = false;
                this.projectBtnAdlRole.Visible = false;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_CREA_GRIGIO_DA_FASCICOLO.ToString()))
                && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_CREA_GRIGIO_DA_FASCICOLO.ToString()).Equals("1")
                && UIManager.UserManager.IsAuthorizedFunctions("DO_NUOVODOC"))
            {
                this.projectImgNewDocument.Visible = true;
            }
            else
            {
                this.projectImgNewDocument.Visible = false;
            }
        }

        private void LoadDiagramAndState()
        {
            //try
            //{
            //    Fascicolo fasc = UIManager.ProjectManager.getProjectInSession();
            //    if (this.EnableStateDiagram && fasc != null && fasc.template != null && fasc.template.SYSTEM_ID != null && fasc.template.SYSTEM_ID != 0)
            //    {
            //        this.StateDiagram = DiagrammiManager.getDgByIdTipoFasc(fasc.template.SYSTEM_ID.ToString(), UIManager.UserManager.GetUserInSession().idAmministrazione);

            //        if (this.StateDiagram != null)
            //        {
            //            this.PnlStateDiagram.Visible = true;
            //            DocsPaWR.Stato stato = DiagrammiManager.getStatoFasc(fasc.systemID);
            //            if (stato != null)
            //            {
            //                //se il ruolo corrente ha visibilità sullo stato corrente allora carico gli stati successivi
            //                if (DiagrammiManager.IsRuoloAssociatoStatoDia(this.StateDiagram.SYSTEM_ID.ToString(), UIManager.RoleManager.GetRoleInSession().idGruppo, stato.SYSTEM_ID.ToString()))
            //                {
            //                    this.LitActualStateDiagram.Text = stato.DESCRIZIONE;
            //                    if (stato.STATO_FINALE)
            //                    {
            //                        this.DocumentDdlStateDiagram.Enabled = false;
            //                    }
            //                    else
            //                    {
            //                        this.popolaComboBoxStatiSuccessivi(stato, this.StateDiagram);
            //                    }

            //                }
            //            }
            //            else
            //            {
            //                //string st = DiagrammiManager.getStatoDocStorico(this.DocumentInWorking.docNumber, this);
            //                //this.LitActualStateDiagram.Text = st;
            //                //ddl_statiSuccessivi.Enabled = false;
            //                this.popolaComboBoxStatiSuccessivi(null, this.StateDiagram);
            //            }

            //            //Quando un documento è in sola lettura, non deve essere possibile cambiare lo stato
            //            if (fasc.accessRights == "45")
            //                this.DocumentDdlStateDiagram.Enabled = false;

            //            //if (this.DocumentInWorking != null && !string.IsNullOrEmpty(this.DocumentInWorking.systemId) && !string.IsNullOrEmpty(this.DocumentInWorking.dataScadenza) && this.DocumentInWorking.dataScadenza != "0" && this.DocumentInWorking.dataScadenza.IndexOf("1900") == -1)
            //            //{
            //            //    this.PnlDocumentStateDiagramDate.Visible = true;
            //            //    this.PnlScadenza.Visible = true;
            //            //    this.DocumentStateDiagramDataValue.Text = this.DocumentInWorking.dataScadenza;
            //            //    this.DocumentStateDiagramDataValue.Enabled = false;
            //            //}
            //            if (this.TemplateProject != null && !string.IsNullOrEmpty(this.TemplateProject.SCADENZA) && this.TemplateProject.SCADENZA != "0")
            //            {
            //                this.PnlDocumentStateDiagramDate.Visible = true;
            //                this.PnlScadenza.Visible = true;
            //                this.DocumentStateDiagramDataValue.Text = fasc.dtaScadenza;

            //                DateTime dataOdierna = System.DateTime.Now;
            //                int scadenza = Convert.ToInt32(fasc.template.SCADENZA);
            //                DateTime dataCalcolata = dataOdierna.AddDays(scadenza);
            //                this.PnlScadenza.Visible = true;
            //                if (string.IsNullOrEmpty(fasc.dtaScadenza))
            //                {
            //                    this.DocumentStateDiagramDataValue.Text = utils.formatDataDocsPa(dataCalcolata);

            //                    //this.DocumentInWorking.dataScadenza = Utils.formatDataDocsPa(dataCalcolata);
            //                }
            //                else
            //                {
            //                    this.DocumentStateDiagramDataValue.Text = fasc.dtaScadenza.Substring(0, 10);
            //                    this.DocumentStateDiagramDataValue.ReadOnly = true;
            //                }
            //                this.UpPnlScadenza.Update();
            //            }
            //            //Quando un documento è di una tipologia non in esercizio, non deve essere possibile cambiare lo stato
            //            if (fasc.template.IN_ESERCIZIO.ToUpper().Equals("NO"))
            //            {
            //                this.DocumentDdlStateDiagram.SelectedIndex = 0;
            //                this.DocumentDdlStateDiagram.Enabled = false;
            //            }

            //        }

            //    }
            //    else if (this.TemplateProject != null)
            //    {
            //        //Imposto la data di scadenza
            //        if (this.TemplateProject.SCADENZA != null && this.TemplateProject.SCADENZA != "" && this.TemplateProject.SCADENZA != "0")
            //        {
            //            try
            //            {
            //                DateTime dataOdierna = System.DateTime.Now;
            //                int scadenza = Convert.ToInt32(this.TemplateProject.SCADENZA);
            //                DateTime dataCalcolata = dataOdierna.AddDays(scadenza);
            //                if (string.IsNullOrEmpty(this.DocumentStateDiagramDataValue.Text))
            //                    this.DocumentStateDiagramDataValue.Text = utils.formatDataDocsPa(dataCalcolata);
            //            }
            //            catch (Exception ex) { }
            //        }
            //        else
            //        {
            //            this.DocumentStateDiagramDataValue.Text = "";
            //        }

            //        this.PnlDocumentStateDiagramDate.Visible = true;
            //        this.PnlScadenza.Visible = true;
            //        //this.DocumentStateDiagramDataValue.Enabled = false;
            //        this.UpPnlScadenza.Update();
            //    }
            //}

            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //}

            Fascicolo fasc = UIManager.ProjectManager.getProjectInSession();

            if (this.EnableStateDiagram && fasc != null && fasc.template != null && fasc.template.SYSTEM_ID != null && fasc.template.SYSTEM_ID != 0)
            {
                this.StateDiagram = DiagrammiManager.getDgByIdTipoFasc(fasc.template.SYSTEM_ID.ToString(), UIManager.UserManager.GetUserInSession().idAmministrazione);

                if (this.StateDiagram != null)
                {
                    this.PnlStateDiagram.Visible = true;
                    DocsPaWR.Stato stato = DiagrammiManager.getStatoFasc(fasc.systemID);
                    if (stato != null)
                    {
                        //Emanuela:Commentato il seguente if perchè se non ho visibilità sullo stato corrente non mi fa vedere nemmeno quelli successivi:sbagliato, quelli 
                        //successivi potrebbero essere visibili
                        //se il ruolo corrente ha visibilità sullo stato corrente allora carico gli stati successivi
                        //if (DiagrammiManager.IsRuoloAssociatoStatoDia(this.StateDiagram.SYSTEM_ID.ToString(), UIManager.RoleManager.GetRoleInSession().idGruppo, stato.SYSTEM_ID.ToString()))
                        //{
                            this.LitActualStateDiagram.Text = stato.DESCRIZIONE;

                            //se il documento è in stato finale ma non è stato rimosso il blocco 
                            //su quest'ultimo allora non limito i diritti sul doc a solo lettura
                            if (stato.STATO_FINALE)
                            {
                                fasc.accessRights = "45";
                                this.EnableEditMode();
                                this.UpContainer.Update();
                                this.upPnlButtons.Update();

                                //Popolo le fasi del diagramma
                                List<AssPhaseStatoDiagramma> phasesState = UIManager.DiagrammiManager.GetFasiStatiDiagramma(this.StateDiagram.SYSTEM_ID.ToString());
                                if (phasesState != null && phasesState.Count > 0)
                                {
                                    if (containerProject.Style["top"] == null || !containerProject.Style["top"].Equals("235px"))
                                    {
                                        containerProject.Attributes["style"] = "top: 235px";
                                        this.UpContainer.Update();
                                    }
                                    this.HeaderProject.BuildFasiDiagramma(phasesState, stato.SYSTEM_ID.ToString(), null);
                                }
                            }
                            else
                            {
                                this.popolaComboBoxStatiSuccessivi(stato, this.StateDiagram);
                            }
                        //}
                    }
                    else
                    {
                        Stato state = DiagrammiManager.getStatoFasc(fasc.systemID);
                        if (state != null)
                        {
                            string st = state.DESCRIZIONE.ToString();
                            this.LitActualStateDiagram.Text = st;
                        }
                        //ddl_statiSuccessivi.Enabled = false;
                        this.popolaComboBoxStatiSuccessivi(null, this.StateDiagram);
                    }

                    //Quando un documento è in sola lettura, non deve essere possibile cambiare lo stato
                    if (fasc.accessRights == "45")
                        this.DocumentDdlStateDiagram.Enabled = false;

                    //if (this.DocumentInWorking != null && !string.IsNullOrEmpty(this.DocumentInWorking.systemId) && !string.IsNullOrEmpty(this.DocumentInWorking.dataScadenza) && this.DocumentInWorking.dataScadenza != "0" && this.DocumentInWorking.dataScadenza.IndexOf("1900") == -1)
                    //{
                    //    this.PnlDocumentStateDiagramDate.Visible = true;
                    //    this.PnlScadenza.Visible = true;
                    //    this.DocumentStateDiagramDataValue.Text = this.DocumentInWorking.dataScadenza;
                    //    this.DocumentStateDiagramDataValue.Enabled = false;
                    //}
                    if (this.TemplateProject != null && !string.IsNullOrEmpty(this.TemplateProject.SCADENZA) && this.TemplateProject.SCADENZA != "0")
                    {
                        this.PnlDocumentStateDiagramDate.Visible = true;
                        this.PnlScadenza.Visible = true;
                        this.DocumentStateDiagramDataValue.Text = fasc.dtaScadenza;

                        DateTime dataOdierna = System.DateTime.Now;
                        int scadenza = Convert.ToInt32(this.TemplateProject.SCADENZA);
                        DateTime dataCalcolata = dataOdierna.AddDays(scadenza);
                        this.PnlScadenza.Visible = true;
                        if (string.IsNullOrEmpty(fasc.dtaScadenza))
                        {
                            this.DocumentStateDiagramDataValue.Text = utils.formatDataDocsPa(dataCalcolata);

                            //this.DocumentInWorking.dataScadenza = Utils.formatDataDocsPa(dataCalcolata);
                        }
                        else
                        {
                            this.DocumentStateDiagramDataValue.Text = fasc.dtaScadenza.Substring(0, 10);
                            this.DocumentStateDiagramDataValue.ReadOnly = true;
                        }
                        this.UpPnlScadenza.Update();
                    }
                    //Quando un documento è di una tipologia non in esercizio, non deve essere possibile cambiare lo stato
                    if (this.TemplateProject.IN_ESERCIZIO.ToUpper().Equals("NO"))
                    {
                        this.DocumentDdlStateDiagram.SelectedIndex = 0;
                        this.DocumentDdlStateDiagram.Enabled = false;
                    }


                }

            }
        }

        protected void InitializePageSize()
        {
            string keyValue = Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_PAGING_ROW_DOCINFASC.ToString());
            int tempValue = 0;
            if (!string.IsNullOrEmpty(keyValue))
            {
                tempValue = Convert.ToInt32(keyValue);
                if (tempValue >= 10 || tempValue <= 50)
                {
                    this.PageSize = tempValue;
                }
            }
        }

        private void ClearSessionProperties()
        {
            this.Result = null;
            this.SelectedRow = string.Empty;
            this.TemplateProject = null;
            this.RecordCount = 0;
            this.PagesCount = 0;
            this.SelectedPage = 1;
            this.SearchFilters = null;
            this.FromClassification = false;
            this.Labels = DocumentManager.GetLettereProtocolli();
            this.CellPosition = new Dictionary<string, int>();
            this.CheckAll = false;
            this.ListCheck = null;
            this.ListToSign = null;
            HttpContext.Current.Session["LinkCustom.type"] = null;
            // Caricamento della griglia se non ce n'è una già selezionata
            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.DocumentInProject)
            {
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.DocumentInProject);
            }

            this.SearchDocumentDdlMassiveOperation.Enabled = false;

            this.ShowGrid(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());

            //Giordano: accodo la mia pulizia:
            HeaderProject.FlagCopyInArchive = "0";
            //Fine

            //questa property rimane in sessione quando dal tab allegati faccio back e torno nella ricerca; va rimossa
            HttpContext.Current.Session.Remove("selectedAttachmentId");
        }

        protected void SearchDocumentDdlMassiveOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            string componentCall = string.Empty;
            if (this.ListCheck != null && this.ListCheck.Count > 0)
            {
                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_ADL")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveAddAdlUser", "ajaxModalPopupMassiveAddAdlUser();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "REMOVE_MASSIVE_ADL")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveRemoveAdlUser", "ajaxModalPopupMassiveRemoveAdlUser();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_ADLR_DOC")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveAddAdlRole", "ajaxModalPopupMassiveAddAdlRole();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "REMOVE_MASSIVE_ADLR_DOC")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveRemoveAdlRole", "ajaxModalPopupMassiveRemoveAdlRole();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "DO_MASSIVE_CONS")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveConservation", "ajaxModalPopupMassiveConservation();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVEXPORTDOC")
                {
                    //Svuoto eventuali sessioni impostate in ricerche precedenti.
                    Session["ricDoc.listaFiltri"] = null;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ExportDati", "ajaxModalPopupExportDati();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_TRANSMISSION")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveTransmission", "ajaxModalPopupMassiveTransmission();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_CONVERSION")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveConversion", "ajaxModalPopupMassiveConversion();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_TIMESTAMP")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveTimestamp", "ajaxModalPopupMassiveTimestamp();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "DO_CONSOLIDAMENTO")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveConsolidation", "ajaxModalPopupMassiveConsolidation();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "DO_CONSOLIDAMENTO_METADATI")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveConsolidationMetadati", "ajaxModalPopupMassiveConsolidationMetadati();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_INOLTRA")
                {
                    DocumentManager.setSelectedRecord(null);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveForward", "ajaxModalPopupMassiveForward();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_CLASSIFICATION")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveCollate", "ajaxModalPopupMassiveCollate();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_REMOVE_VERSIONS")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveRemoveVersions", "ajaxModalPopupMassiveRemoveVersions();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_SIGN")
                {
                    componentType = UserManager.getComponentType(Request.UserAgent);
                    switch (componentType)
                    {
                        case Constans.TYPE_APPLET:
                            componentCall = "ajaxModalPopupMassiveDigitalSignatureApplet();";
                            HttpContext.Current.Session["CommandType"] = null;
                            break;
                        case Constans.TYPE_ACTIVEX:
                        case Constans.TYPE_SMARTCLIENT:
                            componentCall = "ajaxModalPopupMassiveDigitalSignature();";
                            break;
                        case Constans.TYPE_SOCKET:
                            HttpContext.Current.Session["CommandType"] = null;
                            componentCall = "ajaxModalPopupMassiveDigitalSignatureSocket();";
                            break;
                        default:
                            componentCall = "ajaxModalPopupMassiveDigitalSignature();";
                            break; //ABBATANGELI - Modificare in openActiveX() subito dopo la demo del 12-12-2012
                    }
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveDigitalSignatureSocket", componentCall, true);

                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "DO_VERSAMENTO_PARER")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveVersPARER", "ajaxModalPopupMassiveVersPARER();", true);
                }
                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "DO_START_SIGNATURE_PROCESS")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveStartSignatureProcess", "ajaxModalPopupStartProcessSignature();", true);
                }
                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_SIGN_HSM")
                {
                    HttpContext.Current.Session["CommandType"] = null;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveSignatureHSM", "ajaxModalPopupMassiveSignatureHSM();", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorMassiveOperationNoItemSelected', 'warning', '');", true);
            }

            this.SearchDocumentDdlMassiveOperation.SelectedIndex = -1;
            this.UpnlAzioniMassive.Update();
        }     

        /// <summary>
        /// refresh dei componenti ajax
        /// </summary>
        private void RefreshScripts()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "JsTree", "JsTree();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshprojectTxtDescrizione", "charsLeft('projectTxtDescrizione', " + this.MaxLenghtProject + ", '" + this.projectLtrDescrizione.Text.Replace("'", "\'") + "');", true);
            this.projectTxtDescrizione_chars.Attributes["rel"] = "projectTxtDescrizione_" + this.MaxLenghtProject + "_" + this.projectLtrDescrizione.Text;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshNoteChars", "charsLeft('TxtNote', " + this.MaxLenghtNote + ", '');", true);
            this.TxtNote_chars.Attributes["rel"] = "TxtNote_" + this.MaxLenghtNote + "_" + this.projectLitVisibleNotes.Text;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "reallowOp", "reallowOp();", true);
        }

        private void InitializePage()
        {
            this.ListCheck = new Dictionary<string, string>();
            this.ListToSign = new Dictionary<string, FileToSign>();
            Session["templateRicerca"] = null;
            this.InitializeLabel();
            this.InitializePageSize();
            this.InitializeKey();
            this.Label = UIManager.DocumentManager.GetLettereProtocolli().ToArray();

            this.loadRegistri();

            this.InitInitiallyOpenTree();

            if (!UIManager.AdministrationManager.IsEnableRF(UserManager.GetInfoUser().idAmministrazione))
            {
                if (this.RblTypeNote.Items.Count > 3)
                {
                    this.RblTypeNote.Items.RemoveAt(2);
                }
            }
            this.LoadTransmissionModels();
            
            //caricamento delle funzioni massive
            this.LoadMassiveOperation();

            Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();

            if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID))
            {
                if (fascicolo.folderSelezionato == null)
                {
                    fascicolo.folderSelezionato = ProjectManager.getFolder(this, fascicolo);
                    UIManager.ProjectManager.setProjectInSession(fascicolo);
                }

                this.projectImgAddDoc.Enabled = true;
                this.projectImgNewDocument.Enabled = true;
                this.projectImgAddFilter.Enabled = true;
                this.projectImgExportDoc.Enabled = true;
                this.projectImgImportDoc.Enabled = true;
                this.projectImgExportDocApplet.Enabled = true;
                this.projectImgImportDocApplet.Enabled = true;
                this.projectImgExportDocSocket.Enabled = true;
                this.projectImgImportDocSocket.Enabled = true;
                this.EnableEditMode();
                this.PopulateProject();

            }
            else
            {
                DocsPaWR.UnitaOrganizzativa _uo = UIManager.RoleManager.GetRoleInSession().uo;
                this.projectTxtCodiceCollocazione.Text = _uo.codiceRubrica;
                this.idProjectCollocation.Value = _uo.systemId;
                this.projectTxtDescrizioneCollocazione.Text = _uo.descrizione;
                this.projectTxtdata.Text = (string)DateTime.Now.ToShortDateString();
                this.EnableEdit = true;
                (this.HeaderProject.FindControl("projectlblStato") as Label).Visible = false;
                this.projectlblDataChiusura.Visible = false;
                this.HiddenStateOfFolder.Value = "A";
                this.projectLblDataApertura.Visible = false;
                this.projectBtnAdL.Enabled = false;
                this.projectBtnAdlRole.Enabled = false;
                this.projectBntChiudiFascicolo.Enabled = false;
                this.projectBntCancellaFascicolo.Enabled = false;
                this.ShowGrid(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
            }

            bool fascicoloConSottocartelle = false;
            if (fascicolo != null && fascicolo.folderSelezionato.childs != null)
            {
                if (fascicolo.folderSelezionato.childs.Length > 0)
                    fascicoloConSottocartelle = true;
            }
            //caricamento della tipologia dei fascicoli
            this.loadTipologiaFascicolo(fascicoloConSottocartelle);

            this.InitializeAjaxAddressBook();


            // load grid with document in session
            if (Request.QueryString.Count > 0 && Request.QueryString["t"] != null && (Request.QueryString["t"].Equals("n") || Request.QueryString["t"].Equals("c")))
                this.ShowInitDocument();

            // load classification if in session
            FascicolazioneClassificazione classification = HttpContext.Current.Session["classification"] as FascicolazioneClassificazione;
            if (classification != null)
            {
                this.IdProject.Value = classification.systemID;
                this.TxtDescriptionProject.Text = classification.descrizione;
                this.TxtCodeProject.Text = classification.codice;
                HttpContext.Current.Session["classification"] = null;
            }

            switchAppletOrActiveX();
            InitDragAndDropReport();
        }

        private void switchAppletOrActiveX()
        {
            componentType = UserManager.getComponentType(Request.UserAgent);
            this.projectImgImportDoc.Visible = false;
            this.projectImgExportDoc.Visible = false;
            this.projectImgImportDocApplet.Visible = false;
            this.projectImgExportDocApplet.Visible = false;
            this.projectImgImportDocSocket.Visible = false;
            this.projectImgExportDocSocket.Visible = false;

            switch (componentType)
            {
                case Constans.TYPE_ACTIVEX:
                case Constans.TYPE_SMARTCLIENT:
                    this.projectImgImportDoc.Visible = true;
                    this.projectImgExportDoc.Visible = true;
                    break;
                case Constans.TYPE_SOCKET:
                    this.projectImgImportDocSocket.Visible = true;
                    this.projectImgExportDocSocket.Visible = true;
                    break;
                case Constans.TYPE_APPLET:
                    this.projectImgImportDocApplet.Visible = true;
                    this.projectImgExportDocApplet.Visible = true;
                    break;
                default:
                    break;
            }
        }

        private void InitDragAndDropReport()
        {
            this.UploadLiveuploads.Visible = false;
            string closeDragAndDrop = Request.QueryString[CLOSE_DRAG_AND_DROP];
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_ENABLE_DRAG_AND_DROP.ToString())) && Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_ENABLE_DRAG_AND_DROP.ToString()).Equals("1"))
            {
                this.UploadLiveuploads.Visible = true;
                if (DragAndDropManager.Report != null)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "MassiveReportDragAndDrop", " ajaxModalPopupMassiveReportDragAndDrop();", true);

                if (!String.IsNullOrEmpty(closeDragAndDrop) && Boolean.Parse(closeDragAndDrop))
                {
                        this.SearchDocumentsAndDisplayResult(this.SearchFilters, SelectedPage, GridManager.SelectedGrid, Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                        this.BuildGridNavigator();
                        this.UpnlNumerodocumenti.Update();
                        this.UpnlGrid.Update();
                        this.upPnlGridIndexes.Update();
                }
            }
        }

        private void InitInitiallyOpenTree()
        {
            Fascicolo Fasc = ProjectManager.getProjectInSession();
            if (Fasc != null)
            {
                DocsPaWR.Folder folder = ProjectManager.getFolder(this, Fasc);
                if (Fasc.folderSelezionato != null)
                {
                    if (folder.systemID == Fasc.folderSelezionato.systemID)
                        this.treenode_sel.Value = "root_" + Fasc.folderSelezionato.systemID;
                    else
                    {
                        this.treenode_sel.Value = "node_" + Fasc.folderSelezionato.systemID;
                        this.BtnChangeSelectedFolder_Click(null, null);
                    }
                }
                else
                    this.treenode_sel.Value = "root_" + folder.systemID;
            }
        }

        private void ShowInitDocument()
        {
            try
            {
                FiltroRicerca[][] qV = new FiltroRicerca[1][];
                qV[0] = new FiltroRicerca[1];
                FiltroRicerca[] fVList = new FiltroRicerca[0];
                FiltroRicerca fV1 = null;

                fV1 = new FiltroRicerca();
                fV1.argomento = "PROT_ARRIVO";
                fV1.valore = "true";
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                fV1 = new FiltroRicerca();
                fV1.argomento = "PROT_PARTENZA";
                fV1.valore = "true";
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                fV1 = new FiltroRicerca();
                fV1.argomento = "PROT_INTERNO";
                fV1.valore = "true";
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                fV1 = new FiltroRicerca();
                fV1.argomento = "GRIGIO";
                fV1.valore = "true";
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                fV1 = new FiltroRicerca();
                fV1.argomento = "PREDISPOSTO";
                fV1.valore = "true";
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                fV1 = new FiltroRicerca();
                fV1.argomento = "DOCNUMBER";
                fV1.valore = DocumentManager.getSelectedRecord().docNumber;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                qV[0] = fVList;
                this.SearchFilters = qV;
                SelectedPage = 1;

                int numPage = 0;
                int nRec = 0;
                SearchResultInfo[] idProfilesList;

                this.Result = DocumentManager.getQueryInfoDocumentoPagingCustom(UserManager.GetInfoUser(), this, this.SearchFilters, 1, out numPage, out nRec, true, true, this.ShowGridPersonalization, 1, true, null, new string[1] { DocumentManager.getSelectedRecord().docNumber }, out idProfilesList);

                // Se ci sono risultati, vengono visualizzati
                if (this.Result != null && this.Result.Length > 0)
                {
                    this.FromClassification = true;
                    gridViewResult.SelectedIndex = SelectedPage;
                    // Visualizzazione dei risultati
                    this.ShowGrid(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                    this.gridViewResult.SelectedIndex = SelectedPage;

                    string gridType = this.GetLabel("projectLitGrigliaStandard");
                    //projectImgSaveGrid.Enabled = false;
                    if (this.ShowGridPersonalization)
                    {
                        if (GridManager.SelectedGrid != null && string.IsNullOrEmpty(GridManager.SelectedGrid.GridId))
                        {
                            gridType = "<span class=\"red\">" + this.GetLabel("projectLitGrigliaTemp") + "</span>";
                            //if (gridViewResult.Rows.Count > 0) projectImgSaveGrid.Enabled = true;
                        }
                        else
                        {
                            if (!(GridManager.SelectedGrid.GridId).Equals("-1"))
                            {
                                gridType = GridManager.SelectedGrid.GridName;
                                //if (gridViewResult.Rows.Count > 0) projectImgSaveGrid.Enabled = true;
                            }
                        }
                    }

                    this.projectLblNumeroDocumenti.Text = this.GetLabel("projectLblNumeroDocumenti");
                    this.projectLblNumeroDocumenti.Text = this.projectLblNumeroDocumenti.Text.Replace("{0}", gridType);
                    this.projectLblNumeroDocumenti.Text = this.projectLblNumeroDocumenti.Text.Replace("{1}", "1");
                }
                else
                {
                    this.ShowGrid(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                }

                string jsCode = "$(function () {\n"
                              + "     $('.UpnlGrid').append('<div id=grid_disallower style=\\'position: relative; top: -' + ($('.gridViewResult').height()+20) + 'px; left: 1px; opacity: 0.5; filter: alpha(opacity=50); width: ' + ($('.gridViewResult').width()+5) + 'px; height: ' + ($('.gridViewResult').height()+5) + 'px; background: #fff; z-index: 110;\\'>&nbsp;</div>');\n"
                              + "});";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "grid_disallower", jsCode, true);
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private void EnableEditMode()
        {

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().accessRights) && Convert.ToInt32(ProjectManager.getProjectInSession().accessRights) < Convert.ToInt32(HMdiritti.HMdiritti_Write))
            {
                this.EnableEdit = false;
                this.projectBntChiudiFascicolo.Enabled = false;

                this.projectImgAddDoc.Enabled = false;
                this.projectImgNewDocument.Enabled = false;
                this.abilitazioneElementi(false, false);
            }
            else
            {
                if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_MOD_FASCICOLO"))
                {
                    this.EnableEdit = false;
                    this.projectBntChiudiFascicolo.Enabled = false;
                    this.abilitazioneElementi(false, false);
                }
                else
                {
                    this.EnableEdit = true;
                }
            }
        }

        private void InitializeAjaxAddressBook()
        {
            string dataUser = UIManager.RoleManager.GetRoleInSession().idGruppo;
            dataUser = dataUser + "-" + this.projectDdlRegistro.SelectedValue;
            if (UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione) != null)
            {
                RapidSenderDescriptionProject.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione).ID + "-" + UIManager.UserManager.GetUserInSession().idPeople + "-" + UIManager.UserManager.GetUserInSession().systemId;
            }


            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]))
                this.RapidCollocazione.MinimumPrefixLength = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]);

            dataUser = UIManager.RoleManager.GetRoleInSession().systemId + "-" + this.projectDdlRegistro.SelectedValue;
            string callType = "CALLTYPE_GESTFASC_LOCFISICA";
            this.RapidCollocazione.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
        }

        private void InitializeKey()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString())))
            {
                this.RapidClassificationRequired = bool.Parse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString()));
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_FASC_REQUIRED_TIPI_DOC.ToString())))
            {
                this.RapidClassificationRequiredByTypeDoc = Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_FASC_REQUIRED_TIPI_DOC.ToString()).Equals("1");
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()].Equals("1"))
            {
                this.EnableStateDiagram = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()].Equals("1"))
            {
                this.CustomProject = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString())))
            {
                this.RapidClassificationRequired = bool.Parse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString()));
            }


            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.FE_MAX_LENGTH_NOTE.ToString()]))
            {
                this.MaxLenghtNote = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.FE_MAX_LENGTH_NOTE.ToString()]);
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_DESC_FASC.ToString())))
            {
                this.MaxLenghtProject = int.Parse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_DESC_FASC.ToString()));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_DEL_DOC_FASC"))
            {
                this.AllowDeleteDocFromProject = true;
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONS") && !this.IsConservazioneSACER)
            {
                this.AllowConservazione = true;
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADD_ADL"))
            {
                this.AllowADL = true;
            }

            this.ShowGridPersonalization = UIManager.GridManager.EnableCustomGrid(UIManager.RoleManager.GetRoleInSession());

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
            {
                this.projectImgImportDoc.Visible = false;
                this.projectImgExportDoc.Visible = false;
                this.projectImgImportDocApplet.Visible = false;
                this.projectImgExportDocApplet.Visible = false;
                this.projectImgAddFilter.Visible = false;
                this.projectImgRemoveFilter.Visible = false;
                this.SearchDocumentDdlMassiveOperation.Visible = false;
            }
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString()).Equals("1"))
            {
                this.EnabledLibroFirma = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_CREA_RISPOSTA_DA_FASCICOLO.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_CREA_RISPOSTA_DA_FASCICOLO.ToString()).Equals("1"))
            {
                this.EnabledCreateDocumentAnswer = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, DBKeys.ENABLE_FASCICOLO_PUBBLICO.ToString())) && !Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, DBKeys.ENABLE_FASCICOLO_PUBBLICO.ToString()).Equals("0"))
            {
                this.ProjectCheckPublic.Visible = true;
            }
            //if (UIManager.UserManager.IsAuthorizedFunctions("DO_DESCRIZIONI_FASC"))
            //{
            //    this.ProjectImgDescrizioni.Visible = true;
            //}
            if (UserManager.IsAuthorizedFunctions("DO_STATE_SIGNATURE_PROCESS"))
            {
                this.EnableViewInfoProcessesStarted = true;
            }
            else
            {
                this.EnableViewInfoProcessesStarted = false;
            }
        }

        private void LoadMassiveOperation()
        {
            this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem("", ""));
            string title = string.Empty;
            string language = UIManager.UserManager.GetUserLanguage();

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_DOC_FIRMA") && UIManager.UserManager.IsAuthorizedFunctions("DO_DOC_FIRMA"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveDigitalSignatureTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_SIGN"));

            }
            if (UIManager.UserManager.IsAuthorizedFunctions("FASC_INS_DOC") && UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_CLASSIFICATION"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveCollateTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_CLASSIFICATION"));

            }
            if (UIManager.UserManager.IsAuthorizedFunctions("DO_TRA_TRASMETTI") && UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_TRANSMISSION"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveTransmissionTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_TRANSMISSION"));

            }
            if (UIManager.UserManager.IsAuthorizedFunctions("DO_TIMESTAMP") && UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_TIMESTAMP"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveTimestampTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_TIMESTAMP"));

            }

            if (UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_CONVERSION"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConversionTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_CONVERSION"));

            }
            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADD_ADL"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveAddAdlUserTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_ADL"));

                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveAdlUserTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "REMOVE_MASSIVE_ADL"));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveAddAdlRoleTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_ADLR_DOC"));

                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveAdlRoleTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "REMOVE_MASSIVE_ADLR_DOC"));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_INOLTRA"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveForwardTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_INOLTRA"));

            }


            title = Utils.Languages.GetLabelFromCode("SearchDocumentExportDatiTitle", language);
            this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVEXPORTDOC"));

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONS") && !this.IsConservazioneSACER)
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConservationTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "DO_MASSIVE_CONS"));

            }

            // INTEGRAZIONE PITRE-PARER
            if (UIManager.UserManager.IsAuthorizedFunctions("DO_SACER_VERSAMENTO") && this.IsConservazioneSACER)
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConservationPARERTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "DO_VERSAMENTO_PARER"));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_REMOVE_VERSIONS"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveVersionsTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_REMOVE_VERSIONS"));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONSOLIDAMENTO"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConsolidationTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "DO_CONSOLIDAMENTO"));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONSOLIDAMENTO_METADATI"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConsolidationMetadatiTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "DO_CONSOLIDAMENTO_METADATI"));
            }

            if (this.EnabledLibroFirma && UIManager.UserManager.IsAuthorizedFunctions("DO_START_SIGNATURE_PROCESS"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveStartSignatureProcess", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "DO_START_SIGNATURE_PROCESS"));
            }

            if (UserManager.IsAuthorizedFunctions("FIRMA_HSM") && UIManager.UserManager.IsAuthorizedFunctions("FIRMA_HSM_MASSIVA"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveSignatureHSMTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_SIGN_HSM"));
            }
            // riordina alfabeticamente
            List<ListItem> listCopy = new List<ListItem>();
            foreach (ListItem item in this.SearchDocumentDdlMassiveOperation.Items)
                listCopy.Add(item);
            this.SearchDocumentDdlMassiveOperation.Items.Clear();
            foreach (ListItem item in listCopy.OrderBy(item => item.Text))
                this.SearchDocumentDdlMassiveOperation.Items.Add(item);
        }

        protected void ProjectImgHistory_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // In base all'immagine cliccata si apre lo storico relativo
                CustomImageButton caller = sender as CustomImageButton;

                if (caller.ID != null && !string.IsNullOrEmpty(caller.ID))
                {
                    this.TypeHistory = caller.ID;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "HistoryProject", "ajaxModalPopupHistoryProject();", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitializeLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.MassiveSignatureHSM.Title = Utils.Languages.GetLabelFromCode("MassiveSignatureHSMTitle", language);
            this.StartProcessSignature.Title = Utils.Languages.GetLabelFromCode("StartProcessSignature", language);
            this.HistoryProject.Title = Utils.Languages.GetLabelFromCode("TitleHistoryProjectPopup", language);
            this.ProjectImgObjectHistory.ToolTip = Utils.Languages.GetLabelFromCode("HistoryProjectLblObject", language);
            this.projectLblDescrizione.Text = Utils.Languages.GetLabelFromCode("projectLblDescrizione", language);
            this.projectLblRegistro.Text = Utils.Languages.GetLabelFromCode("projectLblRegistro", language);
            this.projectLblTipoFascicolo.Text = Utils.Languages.GetLabelFromCode("projectLblTipoFascicolo", language);
            this.projectlblCodiceClassificazione.Text = Utils.Languages.GetLabelFromCode("projectlblCodiceClassificazione", language);
            this.projectLblDataCollocazioneFisica.Text = Utils.Languages.GetLabelFromCode("projectLblDataCollocazioneFisica", language);
            this.projectLblCollocazioneFisica.Text = Utils.Languages.GetLabelFromCode("projectLblCollocazioneFisica", language);
            this.projectLblCartaceo.Text = Utils.Languages.GetLabelFromCode("projectLblCartaceo", language);
            this.projectTxtdata.Text = System.DateTime.Now.ToShortDateString();
            this.projectImgNotedetails.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgNotedetails", language);
            this.projectImgNotedetails.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgNotedetails", language);
            this.projectItemNotePersonal.Text = Utils.Languages.GetLabelFromCode("DocumentItemNotePersonal", language);
            this.projectItemNoteRole.Text = Utils.Languages.GetLabelFromCode("DocumentItemNoteRole", language);
            this.projectItemNoteRF.Text = Utils.Languages.GetLabelFromCode("DocumentItemNoteRF", language);
            this.projectItemNoteAll.Text = Utils.Languages.GetLabelFromCode("DocumentItemNoteAll", language);
            this.projectlblDataChiusura.Text = Utils.Languages.GetLabelFromCode("projectlblDataChiusura", language);
            this.projectLblDataApertura.Text = Utils.Languages.GetLabelFromCode("projectLblDataApertura", language);
            this.projectLblNumeroDocumenti.Text = Utils.Languages.GetLabelFromCode("projectLblNumeroDocumenti", language);
            this.projectDdlTipologiafascicolo.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("projectDdlTipologiaFascciolo", language));
            this.SearchDocumentDdlMassiveOperation.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("projectDdlAzioniMassive", language));
            this.projectLitVisibleNotesChars.Text = Utils.Languages.GetLabelFromCode("DocumentLitVisibleNotesChars", language);
            this.projectLtrDescrizione.Text = Utils.Languages.GetLabelFromCode("DocumentLitVisibleNotesChars", language) + " ";
            this.projectBtnSave.Text = Utils.Languages.GetLabelFromCode("projectBtnSave", language);
            this.projectBtnSaveAndAcceptDocument.Text = Utils.Languages.GetLabelFromCode("projectBtnSave", language);
            this.projectBtnSaveAndAcceptDocument.ToolTip = Utils.Languages.GetLabelFromCode("projectBtnSaveAndAcceptDocumentTooltip", language);
            this.projectBntChiudiFascicolo.Text = Utils.Languages.GetLabelFromCode("projectBntChiudiApriFascicolo", language);
            this.projectBntCancellaFascicolo.Text = Utils.Languages.GetLabelFromCode("projectBntCancellaFascicolo", language);
            this.projectBtnAdL.Text = Utils.Languages.GetLabelFromCode("DocumentBtnAdL", language);
            this.projectBtnAdlRole.Text = Utils.Languages.GetLabelFromCode("DocumentBtnAdLRole", language);
            this.projectImgRemoveFilter.ToolTip = Utils.Languages.GetLabelFromCode("projectImgRemoveFilter", language);
            this.projectImgAddDoc.ToolTip = Utils.Languages.GetLabelFromCode("projectImgAddDoc", language);
            this.projectImgAddDoc.AlternateText = Utils.Languages.GetLabelFromCode("projectImgAddDoc", language);
            this.OpenAddDoc.Title = Utils.Languages.GetLabelFromCode("projectImgAddDoc", language);
            this.AddFilterProject.Title = Utils.Languages.GetLabelFromCode("AddFilterProjectTitle", language);
            this.projectImgAddFilter.ToolTip = Utils.Languages.GetLabelFromCode("projectImgAddFilter", language);

            this.projectImgExportDoc.ToolTip = Utils.Languages.GetLabelFromCode("projectImgExportDoc", language);
            this.projectImgExportDocApplet.ToolTip = Utils.Languages.GetLabelFromCode("projectImgExportDoc", language);
            this.projectImgExportDocSocket.ToolTip = Utils.Languages.GetLabelFromCode("projectImgExportDoc", language);
            this.ExportDocument.Title = Utils.Languages.GetLabelFromCode("projectImgExportDoc", language);
            this.ExportDocumentApplet.Title = Utils.Languages.GetLabelFromCode("projectImgExportDoc", language);
            this.ExportDocumentSocket.Title = Utils.Languages.GetLabelFromCode("projectImgExportDoc", language);
            this.projectImgImportDoc.ToolTip = Utils.Languages.GetLabelFromCode("projectImgImportDoc", language);
            this.projectImgImportDocApplet.ToolTip = Utils.Languages.GetLabelFromCode("projectImgImportDoc", language);
            this.projectImgImportDocSocket.ToolTip = Utils.Languages.GetLabelFromCode("projectImgImportDoc", language);
            this.ImportDocument.Title = Utils.Languages.GetLabelFromCode("projectImgImportDoc", language);
            this.ImportDocumentApplet.Title = Utils.Languages.GetLabelFromCode("projectImgImportDoc", language);
            this.ImportDocumentSocket.Title = Utils.Languages.GetLabelFromCode("projectImgImportDoc", language);

            this.projectImgEditGrid.ToolTip = Utils.Languages.GetLabelFromCode("projectImgEditGrid", language);
            this.projectImgPreferredGrids.ToolTip = Utils.Languages.GetLabelFromCode("projectImgPreferredGrids", language);
            this.projectImgSaveGrid.ToolTip = Utils.Languages.GetLabelFromCode("projectImgSaveGrid", language);
            this.GridPersonalizationPreferred.Title = Utils.Languages.GetLabelFromCode("projectImgPreferredGrids", language);
            this.GrigliaPersonalizzata.Title = Utils.Languages.GetLabelFromCode("projectImgEditGrid", language);
            this.GrigliaPersonalizzataSave.Title = Utils.Languages.GetLabelFromCode("projectImgSaveGrid", language);
            this.projectLitNomeGriglia.Text = Utils.Languages.GetLabelFromCode("projectLitNomeGriglia", language);
            this.litTreeExpandAll.Text = Utils.Languages.GetLabelFromCode("projectTreeExpandAll", language);
            this.litTreeCollapseAll.Text = Utils.Languages.GetLabelFromCode("projectTreeCollapseAll", language);
            this.ImgFolderSearch.ToolTip = Utils.Languages.GetLabelFromCode("projectImgFolderSearch", language);
            this.ImgFolderSearch.AlternateText = Utils.Languages.GetLabelFromCode("projectImgFolderSearch", language);
            this.ImgFolderAdd.ToolTip = Utils.Languages.GetLabelFromCode("projectImgFolderAdd", language);
            this.ImgFolderAdd.AlternateText = Utils.Languages.GetLabelFromCode("projectImgFolderAdd", language);
            this.ImgFolderModify.ToolTip = Utils.Languages.GetLabelFromCode("projectImgFolderModify", language);
            this.ImgFolderModify.AlternateText = Utils.Languages.GetLabelFromCode("projectImgFolderModify", language);
            this.ImgFolderRemove.ToolTip = Utils.Languages.GetLabelFromCode("projectImgFolderRemove", language);
            this.ImgFolderRemove.AlternateText = Utils.Languages.GetLabelFromCode("projectImgFolderRemove", language);
            this.CreateNode.Title = Utils.Languages.GetLabelFromCode("ProjectDataentryFolderTitle", language);
            this.ModifyNode.Title = Utils.Languages.GetLabelFromCode("ProjectModifyFolderTitle", language);
            this.SearchSubset.Title = Utils.Languages.GetLabelFromCode("ProjectSearchSubsetTitle", language);
            this.btnclassificationschema.AlternateText = Utils.Languages.GetLabelFromCode("btnclassificationschema", language);
            this.btnclassificationschema.ToolTip = Utils.Languages.GetLabelFromCode("btnclassificationschema", language);
            this.OpenTitolario.Title = Utils.Languages.GetLabelFromCode("btnclassificationschema", language);
            this.projectLitVisibleNotes.Text = Utils.Languages.GetLabelFromCode("DocumentNoteNoneVisible", language);
            this.ProjectCheckPrivate.Text = Utils.Languages.GetLabelFromCode("ProjectCheckPrivate", language);
            this.ProjectCheckPrivate.ToolTip = Utils.Languages.GetLabelFromCode("ProjectCheckPrivateTooltip", language);
            this.ProjectCheckPublic.Text = Utils.Languages.GetLabelFromCode("ProjectCheckPublic", language);
            this.ProjectCheckPublic.ToolTip = Utils.Languages.GetLabelFromCode("ProjectCheckPublicTooltip", language);
            this.litStruttura.Text = Utils.Languages.GetLabelFromCode("projectTreeTitle", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
            this.Note.Title = Utils.Languages.GetLabelFromCode("BaseMasterNotes", language);
            this.DocumentViewer.Title = Utils.Languages.GetLabelFromCode("TitleDocumentViewerPopup", language);
            this.litConfirmMoveFolder.Text = Utils.Languages.GetLabelFromCode("projectConfirmMoveFolder", language);
            this.litConfirmMoveDocuments.Text = Utils.Languages.GetLabelFromCode("projectConfirmMoveDocuments", language);
            this.ProjectImgHistoryState.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgHistoryState", language);
            this.DocumentLitStateDiagram.Text = Utils.Languages.GetLabelFromCode("DocumentLitStateDiagram", language);
            this.DocumentDdlStateDiagram.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlStateDiagram", language));
            this.ProjectDdlTransmissionsModel.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("TransmissionDdlTransmissionsModel", language));
            this.DocumentDateStateDiagram.Text = Utils.Languages.GetLabelFromCode("DocumentDateStateDiagram", language);
            this.litTreeAlertOperationNotAllowed.Text = Utils.Languages.GetLabelFromCode("ProjectTreeAlertOperationNotAllowed", language);
            //this.litAlertMoveFolder.Text = this.litAlertMoveDocuments.Text = Utils.Languages.GetLabelFromCode("ProjectLitAlertMoveDocuments", language);      
            
            this.DocumentImgCollocationAddressBook.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgCollocationAddressBook", language);
            this.DocumentImgCollocationAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgCollocationAddressBook", language);

            this.MassiveReport.Title = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.MassiveAddAdlUser.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveAddAdlUserTitle", language));
            this.MassiveRemoveAdlUser.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveAdlUserTitle", language));
            this.MassiveAddAdlRole.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveAddAdlRoleTitle", language));
            this.MassiveRemoveAdlRole.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveAdlRoleTitle", language));
            this.MassiveConservation.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConservationTitle", language));
            this.MassiveTransmission.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveTransmissionTitle", language));
            this.MassiveConversion.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConversionTitle", language));
            this.MassiveTimestamp.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveTimestampTitle", language));
            this.MassiveConsolidation.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConsolidationTitle", language));
            this.MassiveConsolidationMetadati.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConsolidationMetadatiTitle", language));
            this.MassiveForward.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveForwardTitle", language));
            this.MassiveCollate.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveCollateTitle", language));
            this.MassiveRemoveVersions.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveVersionsTitle", language));
            this.MassiveDigitalSignature.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveDigitalSignatureTitle", language));
            this.MassiveDigitalSignatureApplet.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveDigitalSignatureTitle", language));
            this.MassiveDigitalSignatureSocket.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentMassiveDigitalSignatureTitle", language));
            this.ExportDati.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentExportDatiTitle", language));
            this.OpenTitolarioMassive.Title = Utils.Languages.GetLabelFromCode("TitleClassificationScheme", language);
            this.DigitalSignDetails.Title = Utils.Languages.GetLabelFromCode("DigitalSignDetailsTitle", language);
            this.OpenAddDocCustom.Title = Utils.Languages.GetLabelFromCode("OpenAddDocTitleCustom", language);
            this.SearchProjectCustom.Title = Utils.Languages.GetLabelFromCode("SearchProjectTitleCustom", language);
            this.SearchProjectMassive.Title = Utils.Languages.GetLabelFromCode("SearchProjectMassive", language);
            this.MassiveVersPARER.Title = Utils.Languages.GetLabelFromCode("MassiveVersTitle", language);
            this.InfoSignatureProcessesStarted.Title = Utils.Languages.GetLabelFromCode("InfoSignatureProcessesStarted", language);
            this.projectImgNewDocument.ToolTip = Utils.Languages.GetLabelFromCode("ProjectNewDocument", language);
            this.CreateNewDocument.Title = Utils.Languages.GetLabelFromCode("ProjectCreateNewDocument", language);
            this.Object.Title = Utils.Languages.GetLabelFromCode("AddFilterObjectTitle", language);
            this.DetailsLFAutomaticMode.Title = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeTitle", language);
            this.MassiveReportDragAndDrop.Title = Utils.Languages.GetLabelFromCode("MassiveReportDragAndDropTitle", language);
            this.ProjectLitTransmRapid.Text = Utils.Languages.GetLabelFromCode("TransmissionLitRapidTransmission", language);
            //this.ProjectImgDescrizioni.ToolTip = Utils.Languages.GetLabelFromCode("ProjectImgDescrizioni", language);
            this.DescriptionProjectList.Title = Utils.Languages.GetLabelFromCode("DescriptionProjectListTitle", language);
            this.ProjectImgHistoryTipology.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgHistoryTipology", language);
            this.ProjectBtnAccept.Text = Utils.Languages.GetLabelFromCode("ProjectBtnAccept", language);
            this.ProjectBtnView.Text = Utils.Languages.GetLabelFromCode("ProjectBtnView", language);
            this.LblDiritti.Text = Utils.Languages.GetLabelFromCode("ProjectLblDiritti", language);
            this.MassiveTransmissionAccept.Title = Utils.Languages.GetLabelFromCode("MassiveTransmissionAcceptTitle", language);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        #endregion

        #region gestione degli eventi
        protected void TxtCodeProject_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.TxtCodeProject.Text))
                {
                    this.SearchProject();
                }
                else
                {
                    this.IdProject.Value = string.Empty;
                    this.TxtCodeProject.Text = string.Empty;
                    this.TxtDescriptionProject.Text = string.Empty;
                }

                upnBtnTitolario.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void SearchProject()
        {
            try
            {
                string idAmministrazione = UIManager.UserManager.GetUserInSession().idAmministrazione;
                string idTitolario = UIManager.ClassificationSchemeManager.getTitolarioAttivo(idAmministrazione).ID;
                Registro registro = UIManager.RegistryManager.getRegistroBySistemId(projectDdlRegistro.SelectedValue);
                //Fascicolo projectList = UIManager.ProjectManager.GetProjectByCode(registro, TxtCodeProject.Text);
                Fascicolo[] projectLists = ProjectManager.getListaFascicoliDaCodice(this, TxtCodeProject.Text, registro, "I");
                Fascicolo projectList = null;
                if (projectLists != null && projectLists.Length > 0)
                {
                    projectList = projectLists[0];
                }
                if (projectList == null || projectList.tipo.Equals("P"))
                {
                    this.IdProject.Value = string.Empty;
                    this.TxtCodeProject.Text = string.Empty;
                    this.TxtDescriptionProject.Text = string.Empty;
                    string msg = "WarningProjectNotFound";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                }
                else
                {

                    this.IdProject.Value = projectList.systemID;
                    this.TxtCodeProject.Text = projectList.codice;
                    this.TxtDescriptionProject.Text = projectList.descrizione;

                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private void loadRegistri()
        {
            DocsPaWR.Registro[] reg = RoleManager.GetRoleInSession().registri;
            Registro registro = UIManager.RegistryManager.GetRegistryInSession();
            foreach (DocsPaWR.Registro r in reg)
            {
                ListItem i = new ListItem();
                i.Text = r.codRegistro;
                i.Value = r.systemId;
                if (registro != null &&
                    registro.systemId == r.systemId)
                {
                    UIManager.RegistryManager.SetRegistryInSession(r);
                    i.Selected = true;
                }
                if (!r.flag_pregresso)
                {
                    this.projectDdlRegistro.Items.Add(i);
                }
            }

            if (registro == null)
            {
                registro = UIManager.RegistryManager.getRegistroBySistemId(this.projectDdlRegistro.SelectedValue);
                UIManager.RegistryManager.SetRegistryInSession(registro);
            }
        }

        private void setValuePopUp()
        {
            if (!string.IsNullOrEmpty(this.OpenTitolario.ReturnValue))
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.TxtCodeProject.Text = this.ReturnValue.Split('#').First();
                    this.TxtDescriptionProject.Text = this.ReturnValue.Split('#').Last();
                    this.upnBtnTitolario.Update();
                    this.TxtCodeProject_OnTextChanged(new object(), new EventArgs());
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenTitolario','');", true);
            }

            if (!string.IsNullOrEmpty(this.Note.ReturnValue))
            {
                this.FetchNote();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('Note','');", true);
            }

            if (!string.IsNullOrEmpty(this.OpenAddDoc.ReturnValue))
            {
                this.SelectedPage = 1;
                this.UpContainerProjectTab.Update();
                this.UpnlTabHeader.Update();
                this.SelectedRow = string.Empty;
                this.SearchFilters = null;
                SearchDocumentsAndDisplayResult(this.SearchFilters, SelectedPage, GridManager.SelectedGrid, Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenAddDoc','')", true);
            }
            if (!string.IsNullOrEmpty(this.CreateNewDocument.ReturnValue))
            {
                this.SelectedPage = 1;
                this.UpContainerProjectTab.Update();
                this.UpnlTabHeader.Update();
                this.SelectedRow = string.Empty;
                this.SearchFilters = null;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('CreateNewDocument','')", true);
            }

            if (!string.IsNullOrEmpty(this.ImportDocumentApplet.ReturnValue))
            {
                this.SelectedPage = 1;
                this.UpContainerProjectTab.Update();
                this.UpnlTabHeader.Update();
                this.SelectedRow = string.Empty;
                this.SearchFilters = null;
                SearchDocumentsAndDisplayResult(this.SearchFilters, SelectedPage, GridManager.SelectedGrid, Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ImportDocumentApplet','')", true);
            
            }

            if (!string.IsNullOrEmpty(this.ImportDocumentSocket.ReturnValue))
            {
                this.SelectedPage = 1;
                this.UpContainerProjectTab.Update();
                this.UpnlTabHeader.Update();
                this.SelectedRow = string.Empty;
                this.SearchFilters = null;
                SearchDocumentsAndDisplayResult(this.SearchFilters, SelectedPage, GridManager.SelectedGrid, Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ImportDocumentApplet','')", true);

            }

            /*
             * COMMENTO PERCHè VIENE GESTITA NEL DOPOSTBACK
            if (!string.IsNullOrEmpty(this.DocumentViewer.ReturnValue))
            {
                UpnlAzioniMassive.Update();
                UpnlTabHeader.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DocumentViewer','');", true);
            }
            */
            if (!string.IsNullOrEmpty(this.GrigliaPersonalizzata.ReturnValue))
            {
                UpContainerProjectTab.Update();
                UpnlTabHeader.Update();
                // Se ci sono risultati, vengono visualizzati
                if (this.Result != null && this.Result.Length > 0)
                {
                    this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());

                }
                else
                {
                    this.ShowGrid(GridManager.SelectedGrid, null, 0, 0, this.Label);
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('GrigliaPersonalizzata','')", true);
            }

            if (!string.IsNullOrEmpty(this.GrigliaPersonalizzataSave.ReturnValue))
            {
                UpContainerProjectTab.Update();
                UpnlGrid.Update();
                UpnlTabHeader.Update();
                // Se ci sono risultati, vengono visualizzati
                if (this.Result != null && this.Result.Length > 0)
                {
                    this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());

                }
                else
                {
                    this.ShowGrid(GridManager.SelectedGrid, null, 0, 0, this.Label);
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('GrigliaPersonalizzataSave','')", true);

                string msg = "InfoSaveGrid";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info');} else {parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info');}  ", true);
            }

            if (!string.IsNullOrEmpty(this.GridPersonalizationPreferred.ReturnValue))
            {
                UpContainerProjectTab.Update();
                UpnlGrid.Update();
                UpnlTabHeader.Update();
                // Se ci sono risultati, vengono visualizzati
                if (this.Result != null && this.Result.Length > 0)
                {
                    this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());

                }
                else
                {
                    this.ShowGrid(GridManager.SelectedGrid, null, 0, 0, this.Label);
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('GridPersonalizationPreferred','')", true);
            }

            if (!string.IsNullOrEmpty(this.AddFilterProject.ReturnValue))
            {
                this.projectImgRemoveFilter.Enabled = true;
                this.SelectedRow = string.Empty;
                this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AddFilterProject','')", true);
                this.UpContainerProjectTab.Update();
            }

            if (!string.IsNullOrEmpty(this.HiddenRemoveNode.Value))
            {
                this.DeleteSubset();
                this.HiddenRemoveNode.Value = string.Empty;
                this.upPnlButtons.Update();
            }

            if (!string.IsNullOrEmpty(this.HiddenAutomaticState.Value))
            {
                if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
                {
                    Fascicolo prj = ProjectManager.getProjectInSession();
                    DiagrammiManager.salvaModificaStatoFasc(ProjectManager.getProjectInSession().systemID, this.DocumentDdlStateDiagram.SelectedValue, this.StateDiagram, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), string.Empty);
                    DocsPaWR.Stato stato = ProjectManager.getStatoFasc(prj);
                    string idTipoFasc = string.Empty;
                    if (prj.template != null)
                    {
                        idTipoFasc = prj.template.SYSTEM_ID.ToString();
                    }
                    if (idTipoFasc != "")
                    {
                        ArrayList modelli = new ArrayList(DiagrammiManager.isStatoTrasmAutoFasc(UserManager.GetInfoUser().idAmministrazione, Convert.ToString(stato.SYSTEM_ID), idTipoFasc));
                        for (int i = 0; i < modelli.Count; i++)
                        {
                            DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];

                            if (mod.SINGLE == "1")
                            {
                                TrasmManager.effettuaTrasmissioneFascDaModello(mod, Convert.ToString(stato.SYSTEM_ID), prj, this);
                                Response.Redirect("../Project/Project.aspx", false);
                            }
                            else
                            {
                                for (int k = 0; k < mod.MITTENTE.Length; k++)
                                {
                                    if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.GetSelectedRole().systemId)
                                    {
                                        TrasmManager.effettuaTrasmissioneFascDaModello(mod, Convert.ToString(stato.SYSTEM_ID), prj, this);
                                        Response.Redirect("../Project/Project.aspx", false);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    this.HiddenAutomaticState.Value = string.Empty;
                    this.DocumentDdlStateDiagram.SelectedIndex = -1;
                    this.LoadDiagramAndState();
                    this.UpPnlTypeDocument.Update();
                }
            }

            if (!string.IsNullOrEmpty(this.HiddenFinalState.Value))
            {
                if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
                {
                    DiagrammiManager.salvaModificaStatoFasc(ProjectManager.getProjectInSession().systemID, this.DocumentDdlStateDiagram.SelectedValue, this.StateDiagram, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), string.Empty);
                    this.chiudiFascicolo(ProjectManager.getProjectInSession(), UIManager.UserManager.GetInfoUser(), UIManager.RoleManager.GetRoleInSession());
                    this.HiddenFinalState.Value = string.Empty;
                    this.DocumentDdlStateDiagram.SelectedIndex = -1;
                    this.LoadDiagramAndState();
                    this.UpPnlTypeDocument.Update();
                }
            }

            if (!string.IsNullOrEmpty(this.MassiveAddAdlUser.ReturnValue))
            {
                if (this.MassiveAddAdlUser.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveAddAdlUser','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveRemoveAdlUser.ReturnValue))
            {
                if (this.MassiveRemoveAdlUser.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveRemoveAdlUser','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveAddAdlRole.ReturnValue))
            {
                if (this.MassiveAddAdlRole.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveAddAdlRole','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveRemoveAdlRole.ReturnValue))
            {
                if (this.MassiveRemoveAdlRole.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveRemoveAdlRole','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveConservation.ReturnValue))
            {
                if (this.MassiveConservation.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveConservation','');", true);
            }

            if (!string.IsNullOrEmpty(this.ExportDati.ReturnValue))
            {
                if (this.ExportDati.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ExportDati','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveTransmission.ReturnValue))
            {
                if (this.MassiveTransmission.ReturnValue == "true")
                {
                    this.ListCheck = new Dictionary<string, string>();
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveTransmission','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveTimestamp.ReturnValue))
            {
                if (this.MassiveTimestamp.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveTimestamp','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveConversion.ReturnValue))
            {
                if (this.MassiveConversion.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveConversion','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveConsolidation.ReturnValue))
            {
                if (this.MassiveConsolidation.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveConsolidation','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveConsolidationMetadati.ReturnValue))
            {
                if (this.MassiveConsolidationMetadati.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveConsolidationMetadati','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveForward.ReturnValue))
            {
                if (this.MassiveForward.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                if (DocumentManager.getSelectedRecord() != null)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "redirDocument", "disallowOp(''); $(location).attr('href','../Document/Document.aspx');", true);
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveForward','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveCollate.ReturnValue))
            {
                if (this.MassiveCollate.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveCollate','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveDigitalSignature.ReturnValue))
            {
                if (this.MassiveDigitalSignature.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                    MassiveOperationUtils.ItemsStatus = null;
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveDigitalSignature','');", true);
            }

            if (!string.IsNullOrEmpty(this.SearchProjectMassive.ReturnValue))
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "txt_CodFascicolo", "$('#MassiveCollate_panel>#ifrm_MassiveCollate').contents().find('.txt_addressBookLeft').val('" + utils.FormatJs(this.ReturnValue.Split('#').First()) + "');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "txt_DescFascicolo", "$('#MassiveCollate_panel>#ifrm_MassiveCollate').contents().find('#txt_DescFascicolo').val('" + utils.FormatJs(this.ReturnValue.Split('#').Last()) + "');", true);
                }
                else
                    if (this.ReturnValue.Contains("//"))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "txt_CodFascicolo", "$('#MassiveCollate_panel>#ifrm_MassiveCollate').contents().find('.txt_addressBookLeft').val('" + utils.FormatJs(this.ReturnValue) + "');", true);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "txt_DescFascicolo", "$('#MassiveCollate_panel>#ifrm_MassiveCollate').contents().find('#txt_DescFascicolo').val('');", true);
                    }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchProjectMassive','');", true);
            }
        }

        private bool CheckIfRootFolder(DocsPaWR.Folder folder)
        {
            Fascicolo Fasc = ProjectManager.getProjectInSession();
            DocsPaWR.Folder rootFolder = ProjectManager.getFolder(this, Fasc);
            if (folder.systemID == rootFolder.systemID) return true;
            return false;
        }

        private void DeleteSubset()
        {
            if (!ProjectManager.CheckRevocationAcl())
            {
                try
                {
                    string nFasc = "";
                    Folder folder = Session["remove_node_folder"] as Folder;

                    if (this.CheckIfRootFolder(folder))
                    {
                        if (UIManager.ProjectManager.getProjectInSession().tipo.Equals("P"))
                        {
                            string msg = "WarningProjectImpossibleDeleteProcedural";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '', '" + utils.FormatJs(UIManager.ProjectManager.getProjectInSession().codice) + "');", true);
                        }
                        if (UIManager.ProjectManager.getProjectInSession().tipo.Equals("G"))
                        {
                            string msg = "WarningProjectImpossibleDeleteGeneral";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '', '" + utils.FormatJs(UIManager.ProjectManager.getProjectInSession().codice) + "');", true);
                        } return;
                    }
                    if (folder != null)
                    {
                        /* Se il folder selezionato ha figli (doc o sottocartelle) su cui HO visibilità 
                         * non deve essere rimosso. Dopo l'avviso all'utente, la procedura termina */
                        if (folder.childs.Length > 0)
                        {
                            string msg = "WarningProjectImpossibleDeleteHasChildren";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        }
                        else
                        {
                            /* Se il folder selezionato ha figli (doc o sottocartelle) su cui NON HO 
                             * la visibilità non deve essere rimosso */
                            //CanRemoveFascicolo ritornerà un bool: true = posso rimuovere il folder, false altrimenti
                            if (!ProjectManager.CanRemoveFascicolo(this, folder.systemID, out nFasc))
                            {
                                if (nFasc.Equals("0") || nFasc.Equals(""))
                                {
                                    string msg = "WarningProjectImpossibleDeleteContainsDocuments";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                                }
                                else
                                {
                                    string msg = "WarningProjectImpossibleDeleteHasChildren";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                                }
                            }
                            else
                            {
                                ProjectManager.delFolder(this, folder);
                            }
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

        protected void projectBtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();

                if (Request.QueryString.Count > 0 && Request.QueryString["t"] != null && Request.QueryString["t"].Equals("n") && string.IsNullOrEmpty(HiddenControlPrivate.Value) && !this.ProjectCheckPrivate.Checked && DocumentManager.getSelectedRecord() != null && ((!string.IsNullOrEmpty(DocumentManager.getSelectedRecord().privato) && DocumentManager.getSelectedRecord().privato == "1") || (!string.IsNullOrEmpty(DocumentManager.getSelectedRecord().personale) && DocumentManager.getSelectedRecord().personale == "1")))
                {
                    string msg = "WarningDocumentPrivateClassification";
                    if (!string.IsNullOrEmpty(DocumentManager.getSelectedRecord().privato) && DocumentManager.getSelectedRecord().privato == "1")
                    {
                        msg = "WarningDocumentPrivateClassification";
                    }
                    else
                    {
                        msg = "WarningDocumentUserClassification";
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'HiddenControlPrivate', '');} else {parent.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'HiddenControlPrivate', '');}", true);
                    return;
                }
                else
                {
                    if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID))
                    {
                        this.modificaFascicolo(fascicolo.systemID);
                    }
                    else
                    {
                        this.salvaNuovoFascicolo();
                    }
                    this.HiddenControlPrivate.Value = string.Empty;
                }
                this.upPnlButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void projectBtnSaveAndAcceptDocument_Click(object sender, EventArgs e)
        {
            try
            {
                if (Request.QueryString.Count > 0 && Request.QueryString["t"] != null && Request.QueryString["t"].Equals("c") && string.IsNullOrEmpty(HiddenControlPrivate.Value) && !this.ProjectCheckPrivate.Checked && DocumentManager.getSelectedRecord() != null && ((!string.IsNullOrEmpty(DocumentManager.getSelectedRecord().privato) && DocumentManager.getSelectedRecord().privato == "1") || (!string.IsNullOrEmpty(DocumentManager.getSelectedRecord().personale) && DocumentManager.getSelectedRecord().personale == "1")))
                {
                    string msg = "WarningDocumentPrivateClassification";
                    if (!string.IsNullOrEmpty(DocumentManager.getSelectedRecord().privato) && DocumentManager.getSelectedRecord().privato == "1")
                    {
                        msg = "WarningDocumentPrivateClassification";
                    }
                    else
                    {
                        msg = "WarningDocumentUserClassification";
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'HiddenControlPrivate', '');} else {parent.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'HiddenControlPrivate', '');}", true);
                    return;
                }
                else
                {
                    this.salvaNuovoFascicolo(true);
                    this.HiddenControlPrivate.Value = string.Empty;
                }
                this.upPnlButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private bool AcceptDocumentAndAddInFolder()
        {
            bool result = false;
            Fascicolo prj = ProjectManager.getProjectInSession();
            DocsPaWR.Trasmissione trasmissione = TrasmManager.getSelectedTransmission();
            DocsPaWR.TrasmissioneSingola trasmSing = null;
            DocsPaWR.TrasmissioneUtente trasmUtente = null;
            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

            List<DocsPaWR.TrasmissioneSingola> list = new List<TrasmissioneSingola>(trasmissione.trasmissioniSingole);

            //Emanuela: modifica per accettazione di trasmissioni a ruolo ed utente, seleziono per defaul prima quella di tipo ruolo per far si
            //che funzioni la gestione di accettazioni di entrambi le trasm
            List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.RUOLO).ToList();
            trasmSing = TrasmManager.RoleTransmissionWithHistoricized(trasmSingoleUtente, infoUtente.idCorrGlobali);
            bool trasmRoleWorked = true;

            if (trasmSing != null)
            {
                trasmUtente = (DocsPaWR.TrasmissioneUtente)trasmSing.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();
                if (trasmUtente != null && trasmSing.ragione.tipo == "W" && string.IsNullOrEmpty(trasmUtente.dataRifiutata) && string.IsNullOrEmpty(trasmUtente.dataAccettata))
                {
                    trasmRoleWorked = !TrasmManager.checkTrasm_UNO_TUTTI_AccettataRifiutata(trasmSing);
                }
                if (trasmUtente != null && trasmSing.ragione.tipo != "W" && TrasmManager.getIfDocOrFascIsInToDoList(infoUtente, trasmUtente.systemId))
                {
                    trasmRoleWorked = false;
                }
            }

            if (trasmSing == null || trasmRoleWorked)
            {
                // Se non è stata trovata la trasmissione come destinatario a ruolo, 
                // cerca quella con destinatario l'utente corrente

                trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.UTENTE).ToList();
                if (trasmSingoleUtente != null)
                {
                    DocsPaWR.Utente utenteCorrente = (DocsPaWR.Utente)UserManager.getCorrispondenteByIdPeople(this, infoUtente.idPeople, AddressbookTipoUtente.INTERNO);
                    TrasmissioneSingola s = trasmSingoleUtente.Where(e => ((DocsPaWR.Utente)e.corrispondenteInterno).idPeople == infoUtente.idPeople).FirstOrDefault();
                    if (s != null)
                        trasmSing = s;
                }
            }

            if (trasmSing != null)
            {
                DocsPaWR.TrasmissioneSingola[] listaTrasmSing;
                listaTrasmSing = trasmissione.trasmissioniSingole;

                trasmUtente = trasmSing.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();

                trasmUtente.dataAccettata = dateformat.getDataOdiernaDocspa(); //getDataCorrente();

                //tipoRisposta
                trasmUtente.tipoRisposta = DocsPaWR.TrasmissioneTipoRisposta.ACCETTAZIONE;
                string errore = string.Empty;
                result = TrasmManager.executeAccRif(this, trasmUtente, trasmissione.systemId, prj, out errore);
            }
            return result;
        }

        private bool controllaStatoFinale()
        {
            if (this.StateDiagram != null)
            {
                for (int i = 0; i < this.StateDiagram.STATI.Length; i++)
                {
                    DocsPaWR.Stato st = (DocsPaWR.Stato)this.StateDiagram.STATI[i];
                    if (st.SYSTEM_ID.ToString() == this.DocumentDdlStateDiagram.SelectedValue && st.STATO_FINALE)
                        return true;
                }
            }
            return false;
        }

        private void modificaFascicolo(string idfascicolo)
        {
            try
            {                
                string msg = null;
                bool statoFinale = false;
                Fascicolo fascicolo = UIManager.ProjectManager.getFascicoloById(idfascicolo, UIManager.UserManager.GetInfoUser());

                fascicolo = this.setFascicolo(fascicolo, fascicolo.idTitolario, fascicolo.idRegistroNodoTit, fascicolo.dataCreazione, null, true);
                if(UIManager.ProjectManager.getProjectInSession() != null && UIManager.ProjectManager.getProjectInSession().noteFascicolo != null)
                    fascicolo.noteFascicolo = UIManager.ProjectManager.getProjectInSession().noteFascicolo;

                #region Popolamento oggetto TypeOperation
                    //Popolamento oggetto TypeOperation per il successivo aggiornamento delle notifiche: verifica se è stata modificata
                    //la descrizione o la tipologia del fascicolo; in tal caso vengono riportate sulle notifiche legate al fascicolo stesso,
                    //qualora esse esistano
                List<TypeOperation> typeOperation = new List<TypeOperation>();
                if ((UIManager.ProjectManager.getProjectInSession().template == null ||
                    string.IsNullOrEmpty(UIManager.ProjectManager.getProjectInSession().template.ID_TIPO_FASC) &&
                    (fascicolo.template != null && !string.IsNullOrEmpty(fascicolo.template.ID_TIPO_FASC))))
                {
                    typeOperation.Add(TypeOperation.CHANGE_TYPE_PROJ);
                }
                if (!UIManager.ProjectManager.getProjectInSession().descrizione.Equals(fascicolo.descrizione))
                {
                    typeOperation.Add(TypeOperation.CHANGE_OBJECT);
                }
                #endregion

                #region profilazione dinamica
                if (this.CustomProject)
                {
                    //PROFILAZIONE DINAMICA
                    if (!string.IsNullOrEmpty(this.projectDdlTipologiafascicolo.SelectedValue))
                    {
                        fascicolo.template = this.PopulateTemplate();

                        if (ProfilerDocManager.verificaCampiObbligatori(this.TemplateProject))
                        {
                            string msgDesc = "WarningProjectRequestfields";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return;
                        }

                        string customMessaget = string.Empty;
                        string messag = ProfilerDocManager.verificaOkContatore(this.TemplateProject, out customMessaget);
                        if (messag != string.Empty)
                        {
                            if (!string.IsNullOrEmpty(customMessaget) && messag.Equals("CUSTOMERROR"))
                            {
                                string msgDesc = "WarningDocumentCustom";
                                string errFormt = Server.UrlEncode(customMessaget);
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');}; ", true);
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + messag.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + messag.Replace("'", @"\'") + "', 'warning', '" + messag.Replace("'", @"\'") + "');}", true);
                            }
                            return;
                        }
                    }

                    if (this.PnlStateDiagram.Visible && !string.IsNullOrEmpty(this.DocumentStateDiagramDataValue.Text) && fascicolo.template.SYSTEM_ID != null)
                    {
                        DiagrammiManager.salvaDataScadenzaFasc(fascicolo.systemID, this.DocumentStateDiagramDataValue.Text, fascicolo.template.SYSTEM_ID.ToString());
                        fascicolo.dtaScadenza = this.DocumentStateDiagramDataValue.Text;
                        UIManager.ProjectManager.setProjectInSession(fascicolo);
                    }
                }
                #endregion

                #region aggiornamento delle note
                if (fascicolo.tipo.Equals(GENERALE) ||
                        (fascicolo.tipo.Equals(PROCEDURALE)))
                {
                    this.UpdateNote(fascicolo);

                }

                #endregion

                #region Diagrammi di stato
                //    //Diagrammi di stato
                if (this.EnableStateDiagram)
                {
                    DocsPaWR.Stato statoAttuale = DiagrammiManager.getStatoFasc(idfascicolo);
                    DocsPaWR.DiagrammaStato dg = this.StateDiagram;
                    //Stato iniziale
                    if (statoAttuale == null && dg != null)
                    {
                        DiagrammiManager.salvaModificaStatoFasc(fascicolo.systemID, this.DocumentDdlStateDiagram.SelectedValue, dg, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), string.Empty);
                    }

                    //Stato qualsiasi
                    if (statoAttuale != null && dg != null)
                    {
                        if (!string.IsNullOrEmpty(this.DocumentDdlStateDiagram.SelectedValue) && dg != null)
                        {
                            if (this.controllaStatoFinale())
                            {
                                statoFinale = true;
                            }

                            ////
                            //Controllo se lo stato selezionato è uno stato automatico
                            if (DiagrammiManager.isStatoAuto(this.DocumentDdlStateDiagram.SelectedItem.Value, Convert.ToString(this.StateDiagram.SYSTEM_ID)))
                            {
                                string msgConfirm = "WarningDocumentConfirmAutomaticState";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenAutomaticState', '');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenAutomaticState', '');}", true);
                                this.UpConfirmStateDiagram.Update();
                                return;
                            }

                            if (statoFinale)
                            {
                                statoFinale = false;
                                string msgConfirm = "WarningProjectConfirmFinalState";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenFinalState', '');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenFinalState', '');}", true);
                                this.UpConfirmStateDiagram.Update();
                                return;
                            }
                            else
                            {
                                DiagrammiManager.salvaModificaStatoFasc(fascicolo.systemID, this.DocumentDdlStateDiagram.SelectedValue, dg, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), string.Empty);
                                //////
                            }

                            //Verifico se effettuare una tramsissione automatica assegnata allo stato
                            if (fascicolo.template != null && fascicolo.template.SYSTEM_ID != 0 && this.PnlStateDiagram.Visible)
                            {
                                ArrayList modelli = new ArrayList(DiagrammiManager.isStatoTrasmAutoFasc(UserManager.GetInfoUser().idAmministrazione, this.DocumentDdlStateDiagram.SelectedItem.Value, fascicolo.template.SYSTEM_ID.ToString()));
                                for (int i = 0; i < modelli.Count; i++)
                                {
                                    DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                                    if (mod.SINGLE == "1")
                                    {
                                        TrasmManager.effettuaTrasmissioneFascDaModello(mod, this.DocumentDdlStateDiagram.SelectedItem.Value, fascicolo, this);
                                    }
                                    else
                                    {
                                        for (int j = 0; j < mod.MITTENTE.Length; j++)
                                        {
                                            if (mod.MITTENTE[j].ID_CORR_GLOBALI.ToString() == RoleManager.GetRoleInSession().systemId)
                                            {
                                                TrasmManager.effettuaTrasmissioneFascDaModello(mod, this.DocumentDdlStateDiagram.SelectedItem.Value, fascicolo, this);
                                                break;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
                #endregion



                //salvatggio delle modifiche al fascicolo
                fascicolo = UIManager.ProjectManager.setFascicolo(fascicolo, UIManager.UserManager.GetInfoUser());

                if (fascicolo == null)
                {
                    msg = "ErrorModificaProject";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error');}  ", true);
                    return;
                }
                else
                {
                    //fascicolo = UIManager.ProjectManager.getFascicoloById(idfascicolo, UIManager.UserManager.GetInfoUser());
                    UIManager.ProjectManager.setProjectInSession(fascicolo);
                }

                msg = string.Empty;
                this.SaveNote(out msg);
                if (!string.IsNullOrEmpty(msg))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');}", true);
                    return;
                }

                if (fascicolo.template != null && fascicolo.template.SYSTEM_ID != 0)
                {
                    fascicolo.template = ProfilerProjectManager.getTemplateFascDettagli(fascicolo.systemID);
                    this.TemplateProject = fascicolo.template;
                    this.projectDdlTipologiafascicolo.Enabled = false;
                    if (this.EnableStateDiagram && this.StateDiagram != null)
                    {
                        this.LoadDiagramAndState();
                        this.DocumentDdlStateDiagram.SelectedIndex = -1;
                    }
                    this.PopulateProfiledDocument();
                    this.UpPnlTypeDocument.Update();
                }

                this.HeaderProject.RefreshHeader();
                this.ProjectTabs.RefreshProjectabs();

                #region Aggiornamento delle notifiche legate al fascicolo

                if (typeOperation != null && typeOperation.Count > 0 )
                {
                    NotificationManager.ModifyNotificationDelegate modifyNotification = new NotificationManager.ModifyNotificationDelegate(NotificationManager.ModifyNotification);
                    modifyNotification.BeginInvoke(UserManager.GetInfoUser(), typeOperation.ToArray(), fascicolo.systemID, NotificationManager.ListDomainObject.FOLDER, null, null);
                }

                #endregion

                //Invio della trasmissione rapida
                if (this.ProjectDdlTransmissionsModel.SelectedValue != "")
                {
                    this.TransmitModel();
                    this.ProjectTabs.RefreshProjectabs();
                }

                ImgFolderAdd.Enabled = !fascicolo.HasStrutturaTemplate;
                ImgFolderModify.Enabled = !fascicolo.HasStrutturaTemplate;
                ImgFolderRemove.Enabled = !fascicolo.HasStrutturaTemplate;
                upnlStruttura.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }


        /// <summary>
        /// carica la tipologia dei fascicoli in nella drop down
        /// </summary>
        private void loadTipologiaFascicolo(bool saltaStrutturati = false)
        {
            try
            {
                string _idAmministrazione = UIManager.UserManager.GetUserInSession().idAmministrazione;
                string _idGruppo = UIManager.RoleManager.GetRoleInSession().idGruppo;
                Templates[] template = UIManager.ProjectManager.getTipologiaFascicoloByRuolo(_idAmministrazione, _idGruppo, DIRITTI);

                projectDdlTipologiafascicolo.Items.Add(new ListItem(string.Empty, string.Empty));
                if (template.Length > 0)
                {
                    foreach (Templates t in template)
                    {
                        if (!t.IPER_FASC_DOC.Equals(ATTIVO))
                        {
                            if (saltaStrutturati && !string.IsNullOrEmpty(t.ID_TEMPLATE_STRUTTURA))
                            {
                                //tolgo dalla combo questa tipologia perchè associata ad una struttura
                            }
                            else
                            {
                                projectDdlTipologiafascicolo.Items.Add(new ListItem(t.DESCRIZIONE, t.SYSTEM_ID.ToString()));
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// salvatggio di un nuovo fascicolo
        /// </summary>
        private void salvaNuovoFascicolo(bool accettaDoc = false)
        {
            try
            {
                string msg = string.Empty;
                if (!string.IsNullOrEmpty(projectTxtDescrizione.Text) &&
                    !string.IsNullOrEmpty(TxtDescriptionProject.Text) &&
                    !string.IsNullOrEmpty(TxtCodeProject.Text) &&
                    !string.IsNullOrWhiteSpace(projectTxtDescrizione.Text))
                {
                    Registro registro = UIManager.RegistryManager.getRegistroBySistemId(projectDdlRegistro.SelectedValue);
                    Utente user = UIManager.UserManager.GetUserInSession();
                    Ruolo role = UIManager.RoleManager.GetRoleInSession();
                    InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                    OrgTitolario titolario = UIManager.ClassificationSchemeManager.getTitolarioAttivo(user.idAmministrazione);
                    FascicolazioneClassificazione[] fascicolazioneclassificazione = UIManager.ProjectManager.getFascicolazioneClassificazione(user.idAmministrazione, role.idGruppo, infoUtente.idPeople, registro, TxtCodeProject.Text, false, titolario.ID);

                    string valoreChiaveConsentiClass = string.Empty;
                    valoreChiaveConsentiClass = Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_BLOCCA_CLASS.ToString());

                    DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchiaDaCodice2(this, TxtCodeProject.Text, infoUtente.idAmministrazione, titolario.ID, registro);

                    DocsPaWR.FascicolazioneClassifica classifica = null;

                    if (gerClassifica != null)
                    {
                        for (int i = 0; i < gerClassifica.Length; i++)
                        {
                            classifica = (DocsPaWR.FascicolazioneClassifica)gerClassifica[i];
                        }
                    }
                    if (classifica.cha_ReadOnly)
                    {
                        this.IdProject.Value = string.Empty;
                        this.TxtCodeProject.Text = string.Empty;
                        this.TxtDescriptionProject.Text = string.Empty;
                        msg = "ErrorCreateProjectNode";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                        return;
                    }

                    Fascicolo fascicolo = setFascicolo(new Fascicolo(), titolario.ID, fascicolazioneclassificazione[0].idRegistroNodoTit, System.DateTime.Now.ToShortDateString(), fascicolazioneclassificazione[0].systemID, false);

                    fascicolo.idUoLF = this.idProjectCollocation.Value;
                    fascicolo.descrizioneUOLF = this.projectTxtDescrizioneCollocazione.Text;
                    fascicolo.varCodiceRubricaLF = this.projectTxtCodiceCollocazione.Text;

                    if (this.ProjectCheckPrivate.Checked)
                    {
                        fascicolo.privato = "1";
                    }
                    else
                    {
                        fascicolo.privato = "0";
                    }

                    fascicolo.pubblico = this.ProjectCheckPublic.Checked;

                    if (!string.IsNullOrEmpty(this.projectTxtdata.Text))
                    {
                        fascicolo.dtaLF = this.projectTxtdata.Text;
                    }
                    else
                    {
                        fascicolo.dtaLF = string.Empty;
                    }

                    //PROFILAZIONE DINAMICA
                    if (!string.IsNullOrEmpty(this.projectDdlTipologiafascicolo.SelectedValue))
                    {
                        fascicolo.template = this.PopulateTemplate();

                        if (ProfilerDocManager.verificaCampiObbligatori(this.TemplateProject))
                        {
                            string msgDesc = "WarningProjectRequestfields";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return;
                        }

                        string customMessaget = string.Empty;
                        string messag = ProfilerDocManager.verificaOkContatore(this.TemplateProject, out customMessaget);
                        if (messag != string.Empty)
                        {
                            if (!string.IsNullOrEmpty(customMessaget) && messag.Equals("CUSTOMERROR"))
                            {
                                string msgDesc = "WarningDocumentCustom";
                                string errFormt = Server.UrlEncode(customMessaget);
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');}; ", true);
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + messag.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + messag.Replace("'", @"\'") + "', 'warning', '" + messag.Replace("'", @"\'") + "');}", true);
                            }
                            return;
                        }
                    }

                    Fascicolo prj = ProjectManager.getProjectInSession();
                    if (prj != null && prj.noteFascicolo != null && prj.noteFascicolo.Length > 0)
                    {
                        fascicolo.noteFascicolo = prj.noteFascicolo;
                    }


                    UIManager.ProjectManager.setProjectInSession(fascicolo);
   
                    msg = string.Empty;
                    this.SaveNote(out msg);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');}", true);
                        return;
                    }
                    
                    //creazione del fascicolo
                    fascicolo = UIManager.ProjectManager.newFascicolo(fascicolazioneclassificazione[0], fascicolo, infoUtente, role);
                    string errore = UIManager.ProjectManager.getEsitoCreazioneFascicolo();

                    switch (errore)
                    {
                        case "OK": msg = string.Empty;
                            {
                                fascicolo = UIManager.ProjectManager.getFascicoloById(fascicolo.systemID, UIManager.UserManager.GetInfoUser());
                                fascicolo.template = ProfilerProjectManager.getTemplateFascDettagli(fascicolo.systemID);
                                UIManager.ProjectManager.setProjectInSession(fascicolo);
                                this.UpdateProjectCreation();
                                this.aggiornaPanel();

                                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                                Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                                actualPage.IdObject = fascicolo.systemID;
                                actualPage.SearchFilters = null;
                                actualPage.Type = string.Empty;
                                actualPage.Page = "PROJECT.ASPX";
                                actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), true,this.Page);
                                actualPage.CodePage = Navigation.NavigationUtils.NamePage.PROJECT.ToString();
                                actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), string.Empty);
                                navigationList.Add(actualPage);
                                Navigation.NavigationUtils.SetNavigationList(navigationList);
                                this.FromClassification = false;
                                break;
                            }
                        case "GENERIC_ERROR": msg = "ErrorProjectGenericError"; break;
                        case "FASCICOLO_GIA_PRESENTE": msg = "ErrorProjectFascicoloGiaPresente"; break;
                        case "FORMATO_FASCICOLATURA_NON_PRESENTE": msg = "ErrorProjectFormatoFascicolaturaNonPresente"; break;
                        default: msg = "ErrorProjectDefaul"; break;
                    }

                    if (!string.IsNullOrEmpty(msg))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'info', null,null,null,null,'');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'info', null,null,null,null,'');}  ", true);
                        return;
                    }
                    else
                    {
                        UIManager.ProjectManager.setProjectInSession(fascicolo);

                        // salvataggio documento di provenienza se presente
                        SchedaDocumento doc = DocumentManager.getSelectedRecord();
                        if (Request.QueryString.Count > 0 && Request.QueryString["t"] != null && (Request.QueryString["t"].Equals("n") || Request.QueryString["t"].Equals("c")) && doc != null)
                        {
                            //Se indicato, accetto e fascicolo il documento altrimenti procedo con la sola fascicolazione
                            if (accettaDoc)
                            {
                                if (!AcceptDocumentAndAddInFolder())
                                {
                                    msg = "ErrorojectSaveAndAcceptDocument";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '', '" + msg.Replace("'", @"\'") + "');", true);
                                    return;
                                }
                                this.projectBtnSave.Visible = true;
                                this.projectBtnSaveAndAcceptDocument.Visible = false;
                            }
                            else
                            {
                                UIManager.AddDocInProjectManager.addDocumentoInFolder(doc.systemId, fascicolo.folderSelezionato.systemID, infoUtente);
                            }
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "remove_grid_disallower", "$('#grid_disallower').remove();", true);
                            this.SelectedPage = 1;
                            this.BtnRebindGrid_Click(null, null);
                            this.SelectedRow = string.Empty;
                            this.SearchDocumentsAndDisplayResult(this.SearchFilters, SelectedPage, GridManager.SelectedGrid, Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                            this.UpnlGrid.Update();
                        }

                        this.InitInitiallyOpenTree();
                    }

                    //DIAGRAMMI DI STATO
                    //Essendo andata a buon fine la creazione del fascicolo verifico se è necessario salvarne anche lo stato
                    if (this.CustomProject && this.EnableStateDiagram)
                    {
                        //Salvo lo stato del fascicolo
                        if (this.PnlStateDiagram.Visible)
                            DiagrammiManager.salvaModificaStatoFasc(fascicolo.systemID, this.DocumentDdlStateDiagram.SelectedItem.Value, this.StateDiagram, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), this.DocumentStateDiagramDataValue.Text);

                        if (this.PnlStateDiagram.Visible && !string.IsNullOrEmpty(this.DocumentStateDiagramDataValue.Text) && fascicolo.template.SYSTEM_ID != null)
                        {
                            DiagrammiManager.salvaDataScadenzaFasc(fascicolo.systemID, this.DocumentStateDiagramDataValue.Text, fascicolo.template.SYSTEM_ID.ToString());
                            fascicolo.dtaScadenza = this.DocumentStateDiagramDataValue.Text;
                            UIManager.ProjectManager.setProjectInSession(fascicolo);
                        }

                        //Verifico se effettuare una tramsissione automatica assegnata allo stato
                        if (fascicolo.template != null && fascicolo.template.SYSTEM_ID != 0 && this.PnlStateDiagram.Visible)
                        {
                            ArrayList modelli = new ArrayList(DiagrammiManager.isStatoTrasmAutoFasc(UserManager.GetInfoUser().idAmministrazione, this.DocumentDdlStateDiagram.SelectedItem.Value, fascicolo.template.SYSTEM_ID.ToString()));
                            for (int i = 0; i < modelli.Count; i++)
                            {
                                DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                                if (mod.SINGLE == "1")
                                {
                                    TrasmManager.effettuaTrasmissioneFascDaModello(mod, this.DocumentDdlStateDiagram.SelectedItem.Value, fascicolo, this);
                                }
                                else
                                {
                                    for (int k = 0; k < mod.MITTENTE.Length; k++)
                                    {
                                        if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == RoleManager.GetRoleInSession().systemId)
                                        {
                                            TrasmManager.effettuaTrasmissioneFascDaModello(mod, this.DocumentDdlStateDiagram.SelectedItem.Value, fascicolo, this);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                    }
                    //FINE DIAGRAMMI DI STATO 

                    if (fascicolo.template != null && fascicolo.template.SYSTEM_ID != 0)
                    {
                        this.TemplateProject = fascicolo.template;
                        this.projectDdlTipologiafascicolo.Enabled = false;
                        if (this.EnableStateDiagram && this.StateDiagram != null)
                        {
                            this.LoadDiagramAndState();
                            this.DocumentDdlStateDiagram.SelectedIndex = -1;
                        }
                        this.PopulateProfiledDocument();
                        this.UpPnlTypeDocument.Update();
                    }

                    this.HeaderProject.RefreshHeader();
                    this.ProjectTabs.RefreshProjectabs();

                    //Invio della trasmissione rapida
                    if (this.ProjectDdlTransmissionsModel.SelectedValue != "")
                    {
                        this.TransmitModel();
                        this.ProjectTabs.RefreshProjectabs();
                    }

                    ImgFolderAdd.Enabled = !fascicolo.HasStrutturaTemplate;
                    ImgFolderModify.Enabled = !fascicolo.HasStrutturaTemplate;
                    ImgFolderRemove.Enabled = !fascicolo.HasStrutturaTemplate;
                    upnlStruttura.Update();

                    if (fascicolo != null && fascicolo.folderSelezionato.childs != null)
                    {
                        if (fascicolo.template.SYSTEM_ID == 0)
                        {
                            if (fascicolo.folderSelezionato.childs.Length > 0)
                            {
                                projectDdlTipologiafascicolo.Items.Clear();
                                this.loadTipologiaFascicolo(true);
                            }
                        }
                    }
                }
                else
                {
                    msg = "ErrorProjectFieldRequired";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning');}", true);
                }
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> ccList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.Cc"];
                string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

                switch (addressBookCallFrom)
                {
                    case "CUSTOM":
                        if (atList != null && atList.Count > 0)
                        {
                            Corrispondente corr = null;
                            //Profiler document
                            UserControls.CorrespondentCustom userCorr = (UserControls.CorrespondentCustom)this.PnlTypeDocument.FindControl(this.IdCustomObjectCustomCorrespondent);

                            string idAmm = UIManager.UserManager.GetInfoUser().idAmministrazione;
                            foreach (NttDataWA.Popup.AddressBook.CorrespondentDetail addressBookCorrespondent in atList)
                            {

                                if (!addressBookCorrespondent.isRubricaComune)
                                {
                                    corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(addressBookCorrespondent.SystemID);
                                }
                                else
                                {
                                    corr = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(addressBookCorrespondent.CodiceRubrica);
                                }

                            }
                            userCorr.TxtCodeCorrespondentCustom = corr.codiceRubrica;
                            userCorr.TxtDescriptionCorrespondentCustom = corr.descrizione;
                            userCorr.IdCorrespondentCustom = corr.systemId;
                            this.UpPnlTypeDocument.Update();
                        }
                        break;
                    case "COLLOCATION":
                        if (atList != null && atList.Count > 0)
                        {
                            Corrispondente corr = null;
                            string idAmm = UIManager.UserManager.GetInfoUser().idAmministrazione;
                            foreach (NttDataWA.Popup.AddressBook.CorrespondentDetail addressBookCorrespondent in atList)
                            {

                                if (!addressBookCorrespondent.isRubricaComune)
                                {
                                    corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(addressBookCorrespondent.SystemID);
                                }
                                else
                                {
                                    corr = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(addressBookCorrespondent.CodiceRubrica);
                                }

                            }
                            this.idProjectCollocation.Value = corr.systemId;
                            this.projectTxtCodiceCollocazione.Text = corr.codiceRubrica;
                            this.projectTxtDescrizioneCollocazione.Text = corr.descrizione;
                            this.UpProjectPhisycCollocation.Update();
                        }
                        break;
                }
                HttpContext.Current.Session["AddressBook.At"] = null;
                HttpContext.Current.Session["AddressBook.Cc"] = null;
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private void aggiornaPanel()
        {
            this.upPnlButtons.Update();
            this.UpPnlNote.Update();
            this.UpPnlTypeDocument.Update();
            this.UpContainerProjectTab.Update();
            this.UpPnlDateProject.Update();
            this.UpHeaderProject.Update();
            this.upnlStruttura.Update();
            this.upnBtnTitolario.Update();
            this.UpProjectPhisycCollocation.Update();
            this.UpnlAzioniMassive.Update();
            this.UpnlTabHeader.Update();
            this.HeaderProject.RefreshHeader();
        }

        private Fascicolo setFascicolo(Fascicolo newFascicolo, string _idTitolario, string _idRegistroNodoTitolario, string _dataCreazione, string _idNodoSelezionato, bool edit)
        {
            newFascicolo.descrizione = projectTxtDescrizione.Text.Replace(Environment.NewLine, " ");
            newFascicolo.idRegistro = _idRegistroNodoTitolario;
            newFascicolo.idTitolario = _idTitolario;
            newFascicolo.idRegistroNodoTit = _idRegistroNodoTitolario;
            if (!edit)
            {
                newFascicolo.apertura = System.DateTime.Now.ToString();
            }
            //if (!string.IsNullOrEmpty(_idNodoSelezionato))
            //    newFascicolo.codUltimo = UIManager.ProjectManager.getNumeroFascicolo(_idNodoSelezionato, newFascicolo.idRegistro);
            newFascicolo.codUltimo = string.Empty;
            newFascicolo.cartaceo = projectCkCartaceo.Checked;
            newFascicolo.isFascConsentita = "1";
            newFascicolo.controllato = "0";
            newFascicolo.idUoLF = this.idProjectCollocation.Value;
            newFascicolo.descrizioneUOLF = this.projectTxtDescrizioneCollocazione.Text;
            newFascicolo.varCodiceRubricaLF = this.projectTxtCodiceCollocazione.Text;
            newFascicolo.dtaLF = projectTxtdata.Text;
            newFascicolo.InAreaLavoro = "0";
            newFascicolo.template = PopulateTemplate();

            return newFascicolo;
        }

        /// <summary>
        /// rende nascoste/visibili le label dopo il salvatagio del fascicolo
        /// </summary>
        /// <param name="visibile"></param>
        private void ViewLabel()
        {
            Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();

            if (this.projectlblDataChiusuraGenerata.Text.Length > 0)
            {
                this.projectlblDataChiusura.Visible = true;
                this.HiddenStateOfFolder.Value = "C";
            }
            else
            {
                this.projectlblDataChiusura.Visible = false;
                this.HiddenStateOfFolder.Value = "A";
            }

            this.projectLblDataApertura.Visible = true;
        }

        /// <summary>
        /// rende nascoste/visibili le label dopo il salvatagio del fascicolo
        /// </summary>
        /// <param name="visibile"></param>
        private void PopulateProject()
        {

            Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();

            if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID))
            {
                if (fascicolo.folderSelezionato == null)
                {
                    fascicolo.folderSelezionato = UIManager.GridManager.GetFolderByIdFasc(UIManager.UserManager.GetInfoUser(), fascicolo);
                }

                UIManager.ProjectManager.setProjectInSession(fascicolo);
                this.projectTxtDescrizione.Text = fascicolo.descrizione;
                //(this.HeaderProject.FindControl("projectlblStato") as Label).Visible = true;

                if (this.projectlblDataChiusuraGenerata.Text.Length > 0)
                {
                    this.projectlblDataChiusura.Visible = true;
                    this.HiddenStateOfFolder.Value = "C";
                }
                else
                {
                    this.projectlblDataChiusura.Visible = false;
                    this.HiddenStateOfFolder.Value = "A";
                }

                this.projectLblDataApertura.Visible = true;
                this.projectCkCartaceo.Checked = fascicolo.cartaceo;
                this.projectLblDataAperturaGenerata.Text = fascicolo.apertura;
                this.projectlblDataChiusuraGenerata.Text = fascicolo.chiusura;

                if (projectlblDataChiusuraGenerata.Text.Length > 0)
                {
                    this.projectlblDataChiusura.Visible = true;
                    this.HiddenStateOfFolder.Value = "C";
                }
                else
                {
                    this.projectlblDataChiusura.Visible = false;
                    this.HiddenStateOfFolder.Value = "A";
                }

                string language = UIManager.UserManager.GetUserLanguage();

                //stato del fascicolo
                this.projectLblDescrizione.Text = Utils.Languages.GetLabelFromCode("projectLblDescrizione", language);
                switch (fascicolo.stato)
                {
                    case "A":
                        {
                            this.projectBntChiudiFascicolo.Text = Utils.Languages.GetLabelFromCode("projectBntChiudiChiudiFascicolo", language);
                            //this.projectImgAddDoc.Enabled = true;
                            this.projectBntCancellaFascicolo.Enabled = false;
                            break;
                        }
                    case "C":
                        {
                            this.projectBntChiudiFascicolo.Text = Utils.Languages.GetLabelFromCode("projectBntChiudiApriFascicolo", language);
                            this.projectBntCancellaFascicolo.Enabled = true;
                            this.abilitazioneElementi(false, true);
                            break;
                        }
                    default:
                        {
                            break;
                        }

                }

                this.SearchDocumentDdlMassiveOperation.Enabled = true;


                this.projectImgAddFilter.Enabled = true;
                this.projectImgExportDoc.Enabled = true;
                //this.projectImgImportDoc.Enabled = true;
                this.projectImgExportDocApplet.Enabled = true;
                this.projectImgExportDocSocket.Enabled = true;
                //this.projectImgImportDocApplet.Enabled = true;
                //this.projectImgRemoveFilter.Enabled = true;
                this.projectImgPreferredGrids.Enabled = true;
                this.projectImgEditGrid.Enabled = true;

                this.PlcStructur.Visible = true;
                this.PhlTitolario.Visible = false;

                this.projectLblRegistro.Visible = false;
                this.projectDdlRegistro.Visible = false;

                if (fascicolo.template != null && fascicolo.template.SYSTEM_ID != 0)
                {
                    this.TemplateProject = fascicolo.template;
                    this.PopulateProfiledDocument();
                    //this.LoadDiagramAndState();
                }

                if (!string.IsNullOrEmpty(fascicolo.InAreaLavoro) &&
                    fascicolo.InAreaLavoro.Equals("0"))
                {
                    this.projectBtnAdL.Text = Utils.Languages.GetLabelFromCode("projectBtnAdL", UIManager.UserManager.GetUserLanguage());
                }
                else
                {
                    this.projectBtnAdL.Text = Utils.Languages.GetLabelFromCode("projectBtnRimuoviAdL", UIManager.UserManager.GetUserLanguage());
                }

                if (ProjectManager.isFascInADLRole(fascicolo.systemID, this) == 0)
                {
                    this.projectBtnAdlRole.Text = Utils.Languages.GetLabelFromCode("DocumentBtnAdLRole", UIManager.UserManager.GetUserLanguage());
                }
                else
                {
                    this.projectBtnAdL.Enabled = false;
                    this.projectBtnAdlRole.Text = Utils.Languages.GetLabelFromCode("DocumentBtnAdLRoleRemove", UIManager.UserManager.GetUserLanguage());
                }

                if (fascicolo.tipo.Equals("G"))
                {
                    this.PnlPhisyCollocation.Visible = false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(fascicolo.idUoLF))
                    {
                        DocsPaWR.Corrispondente corr = AddressBookManager.GetCorrespondentBySystemId(fascicolo.idUoLF);
                        this.idProjectCollocation.Value = corr.systemId;
                        this.projectTxtCodiceCollocazione.Text = corr.codiceRubrica;
                        this.projectTxtDescrizioneCollocazione.Text = corr.descrizione;
                    }

                    if (!string.IsNullOrEmpty(fascicolo.dtaLF))
                    {
                        this.projectTxtdata.Text = fascicolo.dtaLF.Substring(0, 10);
                    }
                }

                this.FetchNote();

                if (fascicolo.template != null && fascicolo.template.SYSTEM_ID != 0)
                {
                    this.ProjectImgHistoryTipology.Visible = true;
                    this.projectDdlTipologiafascicolo.Enabled = false;
                    ListItem item = new ListItem();
                    item.Value = fascicolo.template.SYSTEM_ID.ToString();
                    item.Text = fascicolo.template.DESCRIZIONE;
                    if (this.projectDdlTipologiafascicolo.Items.Contains(item))
                    {
                        this.projectDdlTipologiafascicolo.SelectedValue = fascicolo.template.SYSTEM_ID.ToString();
                    }
                    else
                    {
                        this.projectDdlTipologiafascicolo.Items.Add(item);
                        this.projectDdlTipologiafascicolo.SelectedValue = item.Value;
                    }
                    if (EnableStateDiagram)
                    {
                        this.LoadDiagramAndState();
                    }
                }

                if (!string.IsNullOrEmpty(fascicolo.privato) && fascicolo.privato.Equals("1"))
                {
                    this.ProjectCheckPrivate.Checked = true;
                }

                this.ProjectCheckPrivate.Enabled = false;

                this.ProjectCheckPublic.Checked = fascicolo.pubblico;
                this.ProjectCheckPublic.Enabled = false;

                Fascicolo forDebugProject = UIManager.ProjectManager.getProjectInSession();
                this.SearchDocumentsAndDisplayResult(this.SearchFilters, SelectedPage, GridManager.SelectedGrid, Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                this.BuildGridNavigator();
                this.UpnlNumerodocumenti.Update();
                this.UpnlGrid.Update();
                this.upPnlGridIndexes.Update();

                ImgFolderAdd.Enabled = !fascicolo.HasStrutturaTemplate;
                ImgFolderModify.Enabled = !fascicolo.HasStrutturaTemplate;
                ImgFolderRemove.Enabled = !fascicolo.HasStrutturaTemplate;

                this.ProjectBtnAccept.Visible = ProjectManager.ExistsTrasmPendenteConWorkflowFascicolo(fascicolo.systemID, UIManager.RoleManager.GetRoleInSession().systemId, UserManager.GetUserInSession().idPeople);
                this.ProjectBtnView.Visible = ProjectManager.ExistsTrasmPendenteSenzaWorkflowFascicolo(fascicolo.systemID, UIManager.RoleManager.GetRoleInSession().systemId, UserManager.GetUserInSession().idPeople);
                this.PnlDirittiFascicolo.Visible = true;
                switch ((HMdiritti)Enum.Parse(typeof(HMdiritti), fascicolo.accessRights))
                {
                    case HMdiritti.HDdiritti_Waiting:
                        this.LblTipoDiritto.Text = Utils.Languages.GetLabelFromCode("VisibilityLabelWaiting", language);
                        break;
                    case HMdiritti.HMdiritti_Read:
                        this.LblTipoDiritto.Text = Utils.Languages.GetLabelFromCode("VisibilityLabelReadOnly", language);
                        break;
                    case HMdiritti.HMdiritti_Proprietario:
                    case HMdiritti.HMdiritti_Write:
                        this.LblTipoDiritto.Text = Utils.Languages.GetLabelFromCode("VisibilityLabelRW", language);
                        break;
                }
                this.UpDirittiFascicolo.Update();

            }
        }

        /// <summary>
        /// valorizza i campi dopo la  creazione/modifica del fascicolo
        /// </summary>
        private void UpdateProjectCreation()
        {
            Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();

            if (fascicolo != null)
            {
                this.ViewLabel();
                this.projectCkCartaceo.Checked = fascicolo.cartaceo;
                this.projectLblDataAperturaGenerata.Text = fascicolo.apertura;
                this.projectlblDataChiusuraGenerata.Text = fascicolo.chiusura;

                if (projectlblDataChiusuraGenerata.Text.Length > 0)
                {
                    this.projectlblDataChiusura.Visible = true;
                    this.HiddenStateOfFolder.Value = "C";
                }
                else
                {
                    this.projectlblDataChiusura.Visible = false;
                    this.HiddenStateOfFolder.Value = "A";
                }

                this.UpPnlDateProject.Update();
                this.UpHeaderProject.Update();
                this.ProjectCheckPrivate.Enabled = false;
                this.ProjectCheckPublic.Enabled = false;
                this.ProjectCheckPrivate.Attributes.Remove("class");
                this.ProjectCheckPrivate.Attributes.Add("class", "check_disabled");

                string language = UIManager.UserManager.GetUserLanguage();

                //stato del fascicolo
                this.projectLblDescrizione.Text = Utils.Languages.GetLabelFromCode("projectLblDescrizione", language);
                //switch (fascicolo.stato)
                //{
                //    case "A":
                //        {
                //            (this.HeaderProject.FindControl("projectLblStatoGenerato") as Label).Text = Utils.Languages.GetLabelFromCode("prjectStatoRegistroAperto", language);
                //            (this.HeaderProject.FindControl("projectLblStatoGenerato") as Label).CssClass = "open";
                //            break;
                //        }
                //    case "C":
                //        {
                //            (this.HeaderProject.FindControl("projectLblStatoGenerato") as Label).Text = Utils.Languages.GetLabelFromCode("prjectStatoRegistroChiuso", language);
                //            (this.HeaderProject.FindControl("projectLblStatoGenerato") as Label).CssClass = "close";
                //            break;
                //        }
                //    default:
                //        {
                //            (this.HeaderProject.FindControl("projectLblStatoGenerato") as Label).Text = Utils.Languages.GetLabelFromCode("prjectStatoRegistroGiallo", language);
                //            (this.HeaderProject.FindControl("projectLblStatoGenerato") as Label).CssClass = "giallo";
                //            break;
                //        }

                //}

                //abilitazione/disabilitazione componenti
                this.projectBntChiudiFascicolo.Enabled = true;
                this.projectBntCancellaFascicolo.Enabled = true;
                this.projectBtnAdL.Enabled = true;
                this.projectBtnAdlRole.Enabled = true;
                this.SearchDocumentDdlMassiveOperation.Enabled = true;

                this.projectImgAddDoc.Enabled = true;
                this.projectImgNewDocument.Enabled = true;
                this.projectImgAddFilter.Enabled = true;
                this.projectImgExportDoc.Enabled = true;
                this.projectImgImportDoc.Enabled = true;
                this.projectImgExportDocApplet.Enabled = true;
                this.projectImgImportDocApplet.Enabled = true;
                this.projectImgExportDocSocket.Enabled = true;
                this.projectImgImportDocSocket.Enabled = true;
                //this.projectImgRemoveFilter.Enabled = true;

                this.projectImgPreferredGrids.Enabled = true;
                this.projectImgEditGrid.Enabled = true;

                this.PlcStructur.Visible = true;
                this.PhlTitolario.Visible = false;

                //stato del fascicolo
                switch (fascicolo.stato)
                {
                    case "A": this.projectBntChiudiFascicolo.Text = Utils.Languages.GetLabelFromCode("projectBntChiudiChiudiFascicolo", language);
                        this.projectBntCancellaFascicolo.Enabled = false;
                        break;
                    case "C": this.projectBntChiudiFascicolo.Text = Utils.Languages.GetLabelFromCode("projectBntChiudiApriFascicolo", language);
                        this.projectBntCancellaFascicolo.Enabled = true;
                        this.projectBtnSave.Enabled = false;
                        this.SearchDocumentDdlMassiveOperation.Enabled = false;
                        this.projectBtnAdL.Enabled = false;
                        this.projectBtnAdlRole.Enabled = false;
                        break;
                }

                this.projectLblRegistro.Visible = false;
                this.projectDdlRegistro.Visible = false;
                //gestione note
                //if (GetLastNote() != null)
                //{
                //    TxtNote.Text = info.Testo;
                //    SetVisibleNote(fascicolo.noteFascicolo.Length);
                //}

                //if (fascicolo.folderSelezionato == null)
                //    fascicolo.folderSelezionato = UIManager.GridManager.GetFolderByIdFasc(UIManager.UserManager.GetInfoUser(), fascicolo);

                this.docsInFolderCount.Text = "0";

                ImgFolderAdd.Enabled = !fascicolo.HasStrutturaTemplate;
                ImgFolderModify.Enabled = !fascicolo.HasStrutturaTemplate;
                ImgFolderRemove.Enabled = !fascicolo.HasStrutturaTemplate;
                
                this.PnlDirittiFascicolo.Visible = true;
                switch ((HMdiritti)Enum.Parse(typeof(HMdiritti), fascicolo.accessRights))
                {
                    case HMdiritti.HDdiritti_Waiting:
                        this.LblTipoDiritto.Text = Utils.Languages.GetLabelFromCode("VisibilityLabelWaiting", language);
                        break;
                    case HMdiritti.HMdiritti_Read:
                        this.LblTipoDiritto.Text = Utils.Languages.GetLabelFromCode("VisibilityLabelReadOnly", language);
                        break;
                    case HMdiritti.HMdiritti_Proprietario:
                    case HMdiritti.HMdiritti_Write:
                        this.LblTipoDiritto.Text = Utils.Languages.GetLabelFromCode("VisibilityLabelRW", language);
                        break;
                }
                this.UpDirittiFascicolo.Update();
            }

        }



        /// <summary>
        /// riduce la lunghezza di una stringa aggiungendo dei ...
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lenghtMax"></param>
        /// <returns></returns>
        //private string ellipsis(string text, int lenghtMax)
        //{
        //    if (text.Length > lenghtMax)
        //        return text.Remove(lenghtMax) + "...";

        //    return text;
        //}

        protected void projectBntCancellaFascicolo_Click(object sender, EventArgs e)
        {
            try
            {
                string nDoc = "";
                Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                if (fascicolo.stato.Equals("C"))
                {
                    if (!ProjectManager.CanRemoveFascicoloPrincipale(this, fascicolo.systemID,out nDoc))
                    {
                        string msg = "WarningProjectImpossibleDelete";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '', '" + utils.FormatJs(nDoc) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '', '" + utils.FormatJs(nDoc) + "');}", true);
                    }
                    else
                    {
                        this.cancellaFascicolo(fascicolo, UIManager.UserManager.GetInfoUser(), UIManager.RoleManager.GetRoleInSession());
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void projectBntChiudiFascicolo_Click(object sender, EventArgs e)
        {
            try
            {
                Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();

                if (fascicolo.stato.Equals("A"))
                {
                    this.chiudiFascicolo(fascicolo, UIManager.UserManager.GetInfoUser(), UIManager.RoleManager.GetRoleInSession());
                }
                else
                {
                    this.apriFascicolo(fascicolo, UIManager.UserManager.GetInfoUser());
                }

                bool visible = fascicolo.stato.Equals("A");
                foreach (GridViewRow grd in this.gridViewResult.Rows)
                {
                    ((CustomImageButton)grd.FindControl("eliminadocumento")).Enabled = visible;
                    ((CustomImageButton)grd.FindControl("newDocumentAnswer")).Enabled = visible;
                }
                this.upPnlButtons.Update();

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// consente la chiusura di un fascicolo
        /// </summary>
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
                this.abilitazioneElementi(false, true);
                //msg = "InfoProjectChiusuraFascicolo";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'info');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'check'}", true);
                UIManager.ProjectManager.setProjectInSession(fascicolo);
                this.projectBntChiudiFascicolo.Text = Utils.Languages.GetLabelFromCode("projectBntChiudiApriFascicolo", UIManager.UserManager.GetUserLanguage());
                this.projectBntCancellaFascicolo.Enabled = true;
                this.UpdateOpenCloseProject();
            }
            else
            {
                msg = "ErroreProjectChiusuraFascicolo";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error');}", true);
            }
        }

        private void cancellaFascicolo(Fascicolo fascicolo, InfoUtente infoutente, Ruolo ruolo)
        {
            string msg = string.Empty;

            if (!string.IsNullOrEmpty(ProjectManager.getProjectInSession().accessRights)
                    && (Convert.ToInt32(ProjectManager.getProjectInSession().accessRights) == Convert.ToInt32(HMdiritti.HMdiritti_Write)
                    || Convert.ToInt32(ProjectManager.getProjectInSession().accessRights) == Convert.ToInt32(HMdiritti.HMdiritti_Proprietario))
                )
            {
                if (ProjectManager.deleteFascicoloAndFolder(fascicolo, infoutente))
                {
                    fascicolo.stato = "DELETED";
                    //fascicolo.systemID = null;
                    UIManager.ProjectManager.setProjectInSession(fascicolo);
                    msg = "ResultDeleteProject";
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info', '');", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info');} else {parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info');}", true);
                }
            }
            else 
            {
                msg = "WarningProjectDirittoMancantePerCancellazione";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning');} else {parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning');}", true);
            }

        }

        /// <summary>
        /// consente l'apertura di un fascicolo
        /// </summary>
        private void apriFascicolo(Fascicolo fascicolo, InfoUtente infoutente)
        {
            string msg = string.Empty;
            fascicolo.apertura = DateTime.Now.ToShortDateString();
            fascicolo.chiusura = string.Empty;
            fascicolo.stato = "A";
            fascicolo = UIManager.ProjectManager.setFascicolo(fascicolo, infoutente);
            if (fascicolo == null)
            {
                msg = "ErroreProjectAperturaFascicolo";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error');}", true);
            }
            else
            {
                abilitazioneElementi(true, true);
                UIManager.ProjectManager.setProjectInSession(fascicolo);
                projectBntChiudiFascicolo.Text = Utils.Languages.GetLabelFromCode("projectBntChiudiChiudiFascicolo", UIManager.UserManager.GetUserLanguage());
                this.projectBntCancellaFascicolo.Enabled = false;
                UpdateOpenCloseProject();
            }

            UIManager.ProjectManager.setProjectInSession(fascicolo);
        }

        protected void UpdateOpenCloseProject()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
            switch (fascicolo.stato)
            {
                case "A":
                    {
                        (this.HeaderProject.FindControl("projectLblStatoGenerato") as Label).Text = Utils.Languages.GetLabelFromCode("prjectStatoRegistroAperto", language);
                        (this.HeaderProject.FindControl("projectLblStatoGenerato") as Label).CssClass = "open";
                        this.projectBntChiudiFascicolo.Text = Utils.Languages.GetLabelFromCode("projectBntChiudiChiudiFascicolo", language);
                        this.projectlblDataChiusura.Visible = false;
                        this.HiddenStateOfFolder.Value = "A";
                        this.projectlblDataChiusuraGenerata.Visible = false;
                        this.projectlblDataChiusuraGenerata.Text = string.Empty;
                        break;
                    }

                case "C":
                    {
                        (this.HeaderProject.FindControl("projectLblStatoGenerato") as Label).Text = Utils.Languages.GetLabelFromCode("prjectStatoRegistroChiuso", language);
                        (this.HeaderProject.FindControl("projectLblStatoGenerato") as Label).CssClass = "close";
                        this.projectBntChiudiFascicolo.Text = Utils.Languages.GetLabelFromCode("projectBntChiudiApriFascicolo", language);
                        this.projectlblDataChiusura.Visible = true;
                        this.HiddenStateOfFolder.Value = "C";
                        this.projectlblDataChiusuraGenerata.Visible = true;
                        this.projectlblDataChiusuraGenerata.Text = fascicolo.chiusura;
                        break;
                    }
            }

            ImgFolderAdd.Enabled = !fascicolo.HasStrutturaTemplate;
            ImgFolderModify.Enabled = !fascicolo.HasStrutturaTemplate;
            ImgFolderRemove.Enabled = !fascicolo.HasStrutturaTemplate;

            if (fascicolo.template != null && fascicolo.template.SYSTEM_ID != 0)
            {
                this.PnlTypeDocument.Controls.Clear();
                this.inserisciComponenti(false);
                this.UpPnlTypeDocument.Update();
            }
            this.UpPnlDateProject.Update();
            this.UpHeaderProject.Update();
            this.upPnlButtons.Update();
            this.upnBtnTitolario.Update();
            this.UpPnlNote.Update();
            this.UpProjectPhisycCollocation.Update();
            this.UpnlAzioniMassive.Update();
            this.upnlStruttura.Update();
        }

        protected void projectBtnAdL_Click(object sender, EventArgs e)
        {
            try
            {
                Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                //UIManager.ProjectManager.getFascicoloById(projectLblIdGenerato.Text, UIManager.UserManager.GetInfoUser());
                if (!string.IsNullOrEmpty(fascicolo.InAreaLavoro) &&
                    fascicolo.InAreaLavoro.Equals("0"))
                {
                    this.inserisciInADL(fascicolo, UIManager.UserManager.GetInfoUser());
                }
                else
                {
                    this.rimuoviDaADL(fascicolo, UIManager.UserManager.GetInfoUser());
                }
                //SearchDocumentsAndDisplayResult(FiltroRicerca, gridViewResult.SelectedIndex, Griglia, Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(Griglia));
                upPnlButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void projectBtnAdLRole_Click(object sender, EventArgs e)
        {
            try
            {
                Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                if (ProjectManager.isFascInADLRole(fascicolo.systemID, this) == 0)
                {
                    this.inserisciInADLRole(fascicolo, UIManager.UserManager.GetInfoUser());
                }
                else
                {
                    this.rimuoviDaADLRole(fascicolo, UIManager.UserManager.GetInfoUser());
                }
                upPnlButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// inserisce un fascicolo in adl
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <param name="infoutente"></param>
        private void inserisciInADL(Fascicolo fascicolo, InfoUtente infoutente)
        {
            try
            {
                string msg = string.Empty;
                if (UIManager.ProjectManager.addFascicoloInAreaDiLavoro(fascicolo, infoutente))
                {

                    fascicolo.InAreaLavoro = "1";
                    //if (UIManager.ProjectManager.setFascicolo(fascicolo, infoutente) != null)
                    //{
                        this.projectBtnAdL.Text = Utils.Languages.GetLabelFromCode("projectBtnRimuoviAdL", UIManager.UserManager.GetUserLanguage());
                        //msg = "InfoInserisciADLProject";
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'info');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'info');}", true);
                        UIManager.ProjectManager.setProjectInSession(fascicolo);
                        this.RefreshADL();
                    //}
                }
                else
                {
                    msg = "ErrorInserisciADLProject";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error');}", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// inserisce un fascicolo in adl ruolo
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <param name="infoutente"></param>
        private void inserisciInADLRole(Fascicolo fascicolo, InfoUtente infoutente)
        {
            try
            {
                string msg = string.Empty;
                fascicolo.InAreaLavoro = "0";
                if (UIManager.ProjectManager.addFascicoloInAreaDiLavoroRole(fascicolo, infoutente))
                {
                    this.projectBtnAdL.Enabled = false;
                    this.projectBtnAdL.Text = Utils.Languages.GetLabelFromCode("projectBtnAdL", UIManager.UserManager.GetUserLanguage());
                    this.projectBtnAdlRole.Text = Utils.Languages.GetLabelFromCode("projectBtnRimuoviAdLR", UIManager.UserManager.GetUserLanguage());
                    UIManager.ProjectManager.setProjectInSession(fascicolo);
                    this.RefreshADL();
                }
                else
                {
                    msg = "ErrorInserisciADLProject";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error');}", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void RefreshADL()
        {
            this.upPnlButtons.Update();
        }
        /// <summary>
        /// rimuove un fascicolo in adl
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <param name="infoutente"></param>
        private void rimuoviDaADL(Fascicolo fascicolo, InfoUtente infoutente)
        {
            string msg = string.Empty;

            if (UIManager.ProjectManager.eliminaFascicoloDaAreaDiLavoro(fascicolo, infoutente))
            {
                fascicolo.InAreaLavoro = "0";
                //if (UIManager.ProjectManager.setFascicolo(fascicolo, infoutente) != null)
                //{
                    this.projectBtnAdL.Text = Utils.Languages.GetLabelFromCode("projectBtnAdL", UIManager.UserManager.GetUserLanguage());
                    //msg = "InfoRimuoviADLProject";
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'info', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'check', '');}", true);
                    UIManager.ProjectManager.setProjectInSession(fascicolo);
                    this.RefreshADL();
                //}
            }
            else
            {
                msg = "ErrorRimuoviADLProject";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
            }

        }

        /// <summary>
        /// rimuove un fascicolo in adl role
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <param name="infoutente"></param>
        private void rimuoviDaADLRole(Fascicolo fascicolo, InfoUtente infoutente)
        {
            string msg = string.Empty;

            if (UIManager.ProjectManager.eliminaFascicoloDaAreaDiLavoroRole(fascicolo, infoutente))
            {
                this.projectBtnAdL.Enabled = true;
                this.projectBtnAdlRole.Text = Utils.Languages.GetLabelFromCode("DocumentBtnAdLRole", UIManager.UserManager.GetUserLanguage());
                this.RefreshADL();
            }
            else
            {
                msg = "ErrorRimuoviADLProject";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'info', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'check', '');}", true);
            }

        }

        private void abilitazioneElementi(bool attiva, bool close)
        {
            this.projectTxtDescrizione.ReadOnly = !attiva;
            //this.ProjectImgDescrizioni.Enabled = attiva;

            this.projectDdlTipologiafascicolo.Enabled = attiva;
            this.projectTxtCodiceCollocazione.ReadOnly = !attiva;
            this.projectTxtDescrizioneCollocazione.ReadOnly = !attiva;
            this.projectTxtdata.ReadOnly = !attiva;

            this.projectCkCartaceo.Enabled = attiva;
            this.DocumentImgCollocationAddressBook.Enabled = attiva;
            this.RefreshTabs(attiva);

            //Only read
            if (!close && ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().accessRights) && Convert.ToInt32(ProjectManager.getProjectInSession().accessRights) == Convert.ToInt32(HMdiritti.HMdiritti_Read))
            {
                this.projectBtnSave.Enabled = true;
                this.RblTypeNote.Enabled = true;
                this.ddlNoteRF.Enabled = true;
                this.txtNoteAutoComplete.Enabled = true;
                this.TxtNote.ReadOnly = false;
            }
            else
            {
                this.projectBtnSave.Enabled = attiva;
                this.RblTypeNote.Enabled = attiva;
                this.ddlNoteRF.Enabled = attiva;
                this.txtNoteAutoComplete.Enabled = attiva;
                this.TxtNote.ReadOnly = !attiva;
            }
        }

        public void RefreshTabs(bool attiva)
        {
            this.projectImgAddDoc.Enabled = attiva;
            this.projectImgImportDoc.Enabled = attiva;
            this.projectImgImportDocApplet.Enabled = attiva;
            this.projectImgNewDocument.Enabled = attiva;
            this.UpContainerProjectTab.Update();
        }

        protected void projectDdlRegistro_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.InitializeAjaxAddressBook();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region gestione note
        protected void RblTypeNote_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.ddlNoteRF.Visible = false;
                this.txtNoteAutoComplete.Visible = false;

                ListItem item = this.RblTypeNote.Items.FindByValue("RF");
                //Se è presente il bottone di selezione esclusiva "RF" si verifica quanti sono gli
                //RF associati al ruolo dell'utente
                if (item != null && RblTypeNote.SelectedIndex == 2)
                {
                    //DocsPaWR.Registro[] registriRf = UserManager.getListaRegistriWithRF(RoleManager.GetRoleInSession().systemId, "1", "");
                    DocsPaWR.Registro[] registriRf = RegistryManager.GetRFListInSession();
                    //Se un ruolo appartiene a più di un RF, allora selezionando dal menù il valore RF
                    //l'utente deve selezionare su quale degli RF creare la nota
                    if (registriRf != null && registriRf.Length > 0)
                    {
                        //Se l'inserimento della nota avviene durante la protocollazione 
                        //ed è impostato nella segnatura il codice del RF, la selezione del RF dal quale
                        //prendere il codice sarà mantenuta valida anche per l'eventuale inserimento delle note
                        //in questo caso non si deve presentare la popup di selezione del RF
                        if (this.ddlNoteRF != null)
                            this.LoadNoteRF(registriRf);

                        this.txtNoteAutoComplete.Visible = false;

                        if (UserManager.IsAuthorizedFunctions("RICERCA_NOTE_ELENCO"))
                        {
                            this.txtNoteAutoComplete.Enabled = false;
                            this.txtNoteAutoComplete.Visible = true;
                            this.ddlNoteRF_SelectedIndexChanged(null, null);
                        }
                    }
                }
                this.UpPnlNote.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadNoteRF(DocsPaWR.Registro[] listaRF)
        {
            try
            {
                this.txtNoteAutoComplete.Text = "";
                this.ddlNoteRF.Items.Clear();
                if (listaRF != null && listaRF.Length > 0)
                {
                    this.ddlNoteRF.Visible = true;
                    this.txtNoteAutoComplete.Visible = true;

                    if (listaRF.Length == 1)
                    {
                        ListItem item = new ListItem();
                        item.Value = listaRF[0].systemId;
                        item.Text = listaRF[0].codRegistro;
                        this.ddlNoteRF.Items.Add(item);
                        this.EnableNoteAutoComplete();
                    }
                    else
                    {
                        ListItem itemVuoto = new ListItem();
                        itemVuoto.Value = "";
                        itemVuoto.Text = Utils.Languages.GetLabelFromCode("DocumentNoteSelectAnRF", UIManager.UserManager.GetUserLanguage());
                        this.ddlNoteRF.Items.Add(itemVuoto);
                        foreach (DocsPaWR.Registro regis in listaRF)
                        {
                            ListItem item = new ListItem();
                            item.Value = regis.systemId;
                            item.Text = regis.codRegistro;
                            this.ddlNoteRF.Items.Add(item);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private void EnableNoteAutoComplete()
        {
            Session.Add("RFNote", "OK^" + this.ddlNoteRF.SelectedItem.Value + "^" + this.ddlNoteRF.SelectedItem.Text);
            this.txtNoteAutoComplete.Enabled = true;
            this.autoComplete1.ContextKey = ddlNoteRF.SelectedItem.Value;
            this.autoComplete1.MinimumPrefixLength = this.AutocompleteMinimumPrefixLength;
            this.txtNoteAutoComplete.Text = "";
        }

        protected void ddlNoteRF_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if ((this.ddlNoteRF.Items.Count > 1 && this.ddlNoteRF.SelectedIndex != 0) || (this.ddlNoteRF.Items.Count == 1))
                {
                    this.EnableNoteAutoComplete();
                    //this.TxtNote.Text = "";
                }
                else
                {
                    this.txtNoteAutoComplete.Text = "";
                    this.txtNoteAutoComplete.Enabled = false;
                    Session.Add("RFNote", "");
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        public virtual void SaveNote(out string msg)
        {
            if (this.TxtNote.Text.Length > this.MaxLenghtNote)
            {
                msg = "ErrorDocumentNoteMaxLength";
                return;
            }
            else
            {
                // Se la nota non contiene testo, viene ripristinato il vecchio valore
                // e viene mostrato un messaggio di errore
                if (String.IsNullOrEmpty(this.TxtNote.Text.Trim()) && this.GetLastNote() != null)
                {
                    this.TxtNote.Text = this.GetLastNote().Testo;
                    msg = "ErrorDocumentNoteEmpty";
                    return;
                }

                //Ruolo ruoloUtente = RoleManager.GetRoleInSession();
                //DocsPaWR.Registro[] registriRf = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "1", "");
                DocsPaWR.Registro[] registriRf = RegistryManager.GetRFListInSession();
                // verifico se è stata selezionata una nota di RF e se si sia selezionato un RF corretto nel caso di utenti con 2 RF almeno
                if (registriRf != null && registriRf.Length > 1 && this.RblTypeNote.SelectedValue.Equals("RF") && string.IsNullOrEmpty(this.ddlNoteRF.SelectedValue))
                {
                    msg = "ErrorDocumentNoneRF";
                    return;
                }

                // Se i dati risultano modificati, viene creata una nuova nota
                this.InsertNote();
                this.FetchNote();
            }

            msg = string.Empty;
        }

        /// <summary>
        /// Reperimento ultima nota inserita per un documento in ordine cronologico
        /// </summary>
        /// <returns></returns>
        public InfoNota GetLastNote()
        {
            try
            {
                InfoNota retValue = null;
                Fascicolo prg = UIManager.ProjectManager.getProjectInSession();
                if (prg != null && prg.noteFascicolo != null)
                {
                    foreach (InfoNota nota in prg.noteFascicolo)
                    {
                        if (!nota.DaRimuovere)
                        {
                            retValue = nota;
                            break;
                        }
                    }
                }

                return retValue;
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Creazione nuova nota a seguito di una modifica dei dati
        /// </summary>
        protected void InsertNote()
        {
            try
            {
                InfoNota nota = new InfoNota();

                if (this.RblTypeNote.SelectedItem != null)
                    nota.TipoVisibilita = (TipiVisibilitaNotaEnum)Enum.Parse(typeof(TipiVisibilitaNotaEnum), this.RblTypeNote.SelectedItem.Value, true);
                else
                    nota.TipoVisibilita = TipiVisibilitaNotaEnum.Tutti;

                nota.Testo = this.TxtNote.Text;

                if (nota.TipoVisibilita == TipiVisibilitaNotaEnum.RF)
                {
                    nota.IdRfAssociato = this.ddlNoteRF.SelectedValue;
                }

                // Se la nota contiene del testo (vengono eliminati anche i ritorni a capo ai lati della stringa)
                if (!String.IsNullOrEmpty(this.TxtNote.Text.Trim()) && (this.GetLastNote() == null || (this.GetLastNote() != null && this.TxtNote.Text.Trim() != this.GetLastNote().Testo)))
                    nota = this.InsertNote(nota);
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Inserimento di una nuova nota da associare ad un documento / fascicolo
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        public InfoNota InsertNote(InfoNota nota)
        {
            try
            {
                nota.DaInserire = true;
                nota.Id = Guid.NewGuid().ToString();
                nota.DataCreazione = DateTime.Now;
                nota.UtenteCreatore = new InfoUtenteCreatoreNota();

                InfoUtente utente = UserManager.GetInfoUser();
                nota.UtenteCreatore.IdUtente = utente.idPeople;
                nota.UtenteCreatore.DescrizioneUtente = utente.userId;
                if (utente.delegato != null)
                {
                    nota.IdPeopleDelegato = utente.delegato.idPeople;
                    nota.DescrPeopleDelegato = utente.delegato.userId;
                }
                Ruolo ruolo = RoleManager.GetRoleInSession();
                nota.UtenteCreatore.IdRuolo = ruolo.idGruppo;
                nota.UtenteCreatore.DescrizioneRuolo = ruolo.descrizione;

                Fascicolo prj = UIManager.ProjectManager.getProjectInSession();

                // Inserimento della nota nella scheda documento (come primo elemento della lista, 
                // solo se il testo della nota da inserire ed il tipo di visibilità sono differenti
                // da quelli dell'ultima nota inserita)
                if (!String.IsNullOrEmpty(nota.Testo.Trim()) && prj != null &&
                    (prj.noteFascicolo == null || prj.noteFascicolo.Length == 0 ||
                    !prj.noteFascicolo[0].Testo.Trim().Equals(nota.Testo.Trim())
                    || !prj.noteFascicolo[0].TipoVisibilita.Equals(nota.TipoVisibilita)))
                {
                    List<InfoNota> note = null;
                    if (prj.noteFascicolo == null)
                    {
                        note = new List<InfoNota>();
                    }
                    else
                    {
                        note = new List<InfoNota>(prj.noteFascicolo);
                    }

                    note.Insert(0, nota);
                    prj.noteFascicolo = note.ToArray();
                    UIManager.ProjectManager.setProjectInSession(prj);
                }

                return nota;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Caricamento dati
        /// </summary>
        public virtual void FetchNote()
        {
            // Reperimento ultima nota
            InfoNota nota = this.GetLastNote();

            if (nota != null)
            {
                // Impostazione dell'autore dell'ultima nota
                if (string.IsNullOrEmpty(this.GetAuthorLastNote(nota)))
                    this.projectLitNoteAuthor.Text = string.Empty;
                else
                    this.projectLitNoteAuthor.Text = "<br />" + Utils.Languages.GetLabelFromCode("DocumentNoteAuthor", UIManager.UserManager.GetUserLanguage()) + " " + this.GetAuthorLastNote(nota);

                this.RblTypeNote.SelectedValue = nota.TipoVisibilita.ToString();
                this.RblTypeNote_SelectedIndexChanged(null, null);
                if (nota.TipoVisibilita.ToString() == "RF") this.ddlNoteRF.SelectedValue = nota.IdRfAssociato;
                this.TxtNote.Text = nota.Testo;

                // Impostazione numero note visibili dall'utente corrente
                this.SetVisibleNote(this.CountNote());
            }
            else
            {
                this.ClearNote();
                this.SetVisibleNote(0);
            }

            this.UpPnlNote.Update();
        }

        /// <summary>
        /// Reperimento dell'autore dell'ultima nota
        /// </summary>
        /// <param name="ultimaNota"></param>
        /// <returns></returns>
        protected string GetAuthorLastNote(InfoNota ultimaNota)
        {
            string autore = string.Empty;

            if (ultimaNota.UtenteCreatore != null)
            {

                if (!string.IsNullOrEmpty(ultimaNota.UtenteCreatore.DescrizioneUtente))
                    autore = ultimaNota.UtenteCreatore.DescrizioneUtente;

                if (autore != string.Empty && !string.IsNullOrEmpty(ultimaNota.UtenteCreatore.DescrizioneRuolo))
                    autore = string.Concat(autore, " (", ultimaNota.UtenteCreatore.DescrizioneRuolo, ")");

                if (!string.IsNullOrEmpty(ultimaNota.DescrPeopleDelegato))
                {
                    string temp = ultimaNota.DescrPeopleDelegato + "<br />" + Utils.Languages.GetLabelFromCode("DocumentNoteAuthorDelegatedBy", UIManager.UserManager.GetUserLanguage()) + " " + autore;
                    autore = temp;
                }
            }

            return autore;
        }

        public virtual void ClearNote()
        {
            this.SetVisibleNote(0);
            this.projectLitVisibleNotes.Text = string.Empty;
            this.projectLitNoteAuthor.Text = string.Empty;
            //this.RblTypeNote.SelectedValue = TipiVisibilitaNotaEnum.Tutti.ToString();
            this.TxtNote.Text = string.Empty;
            this.ddlNoteRF.Visible = false;
            this.txtNoteAutoComplete.Visible = false;
        }

        /// <summary>
        /// Impostazione messaggio di visibilità delle note
        /// </summary>
        /// <param name="countNote"></param>
        protected virtual void SetVisibleNote(int countNote)
        {
            string language = UIManager.UserManager.GetUserLanguage();

            if (countNote == 0)
                this.projectLitVisibleNotes.Text = Utils.Languages.GetLabelFromCode("DocumentNoteNoneVisible", language);
            else
            {
                string format = string.Empty;

                if (countNote == 1)
                    format = "{0} " + Utils.Languages.GetLabelFromCode("DocumentNoteVisibleOne", language);
                else
                    format = "{0} " + Utils.Languages.GetLabelFromCode("DocumentNoteVisibleMore", language);

                this.projectLitVisibleNotes.Text = string.Format(format, countNote.ToString());
            }
        }

        public int CountNote()
        {
            int count = 0;
            Fascicolo fasc = UIManager.ProjectManager.getProjectInSession();
            if (fasc != null)
            {
                foreach (InfoNota item in fasc.noteFascicolo)
                    if (!item.DaRimuovere)
                        count++;
            }
            return count;
        }

        /// <summary>
        /// Aggiornamento in batch delle sole note
        /// </summary>
        protected virtual void UpdateNote(Fascicolo fasc)
        {
            AssociazioneNota oggettoAssociato = new AssociazioneNota();
            oggettoAssociato.TipoOggetto = OggettiAssociazioniNotaEnum.Fascicolo;
            oggettoAssociato.Id = fasc.systemID;

            // Inserimento della nota creata
            string msg = string.Empty;
            this.SaveNote(out msg);
            if (!string.IsNullOrEmpty(msg))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');}", true);
                return;
            }

            // Aggiornamento delle note sul backend
            fasc.noteFascicolo = UIManager.ProjectManager.getProjectInSession().noteFascicolo;
            fasc.noteFascicolo = DocumentManager.UpdateNote(oggettoAssociato, fasc.noteFascicolo);
        }

        #endregion
        #region griglia

        private void ShowGrid(Grid selectedGrid, SearchObject[] result, int recordNumber, int selectedPage, EtichettaInfo[] labels)
        {
            bool visibile = false;
            Templates templates = Session["templateRicerca"] as Templates;
            gridViewResult = this.HeaderGridView(selectedGrid,
              templates,
              this.ShowGridPersonalization, gridViewResult);

            DataTable dt = UIManager.GridManager.InitializeDataSet(selectedGrid,
                         Session["templateRicerca"] as Templates,
                         this.ShowGridPersonalization);


            if (result != null && result.Length > 0)
            {
                dt = this.FillDataSet(dt, result, selectedGrid, labels, templates, this.ShowGridPersonalization);
                visibile = true;
            }

            // adding blank row eachone
            if (dt.Rows.Count == 1 && string.IsNullOrEmpty(dt.Rows[0]["idProfile"].ToString())) dt.Rows.RemoveAt(0);

            DataTable dt2 = dt;
            int dtRowsCount = dt.Rows.Count;
            int index = 1;
            if (dtRowsCount > 0)
            {
                for (int i = 0; i < dtRowsCount; i++)
                {
                    DataRow dr = dt2.NewRow();
                    dr.ItemArray = dt2.Rows[index - 1].ItemArray;
                    dt.Rows.InsertAt(dr, index);
                    index += 2;
                }
            }

            GrigliaResult = dt;
            gridViewResult.DataSource = dt;
            gridViewResult.DataBind();
            if (gridViewResult.Rows.Count > 0) gridViewResult.Rows[0].Visible = visibile;

            string gridType = this.GetLabel("projectLitGrigliaStandard");
            //projectImgSaveGrid.Enabled = false;
            if (this.ShowGridPersonalization)
            {
                if (selectedGrid != null && string.IsNullOrEmpty(selectedGrid.GridId))
                {
                    gridType = "<span class=\"red\">" + this.GetLabel("projectLitGrigliaTemp") + "</span>";
                }
                else
                {
                    if (!(selectedGrid.GridId).Equals("-1"))
                    {
                        gridType = selectedGrid.GridName;
                    }
                }

                this.EnableDisableSave();
            }

            this.projectLblNumeroDocumenti.Text = this.GetLabel("projectLblNumeroDocumenti");
            this.projectLblNumeroDocumenti.Text = this.projectLblNumeroDocumenti.Text.Replace("{0}", gridType);
            this.projectLblNumeroDocumenti.Text = this.projectLblNumeroDocumenti.Text.Replace("{1}", recordNumber.ToString());

            this.UpnlNumerodocumenti.Update();
            this.UpnlGrid.Update();
        }

        /// <summary>
        /// Funzione per la compilazione del datagrid da associare al datagrid
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="result"></param>
        public DataTable FillDataSet(DataTable dataTable,
            SearchObject[] result, Grid selectedGrid,
            EtichettaInfo[] labels, Templates templates, bool showGridPersonalization)
        {
            try
            {
                List<Field> visibleFields = selectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field))).ToList();
                Field specialField = selectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(SpecialField)) && ((SpecialField)e).FieldType.Equals(SpecialFieldsEnum.Icons)).FirstOrDefault<Field>();

                Templates templateTemp = templates;

                OggettoCustom customObjectTemp = new OggettoCustom();

                if (templates != null && !showGridPersonalization)
                {
                    customObjectTemp = templateTemp.ELENCO_OGGETTI.Where(
                         e => e.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && e.DA_VISUALIZZARE_RICERCA == "1").FirstOrDefault();

                    Field d = new Field();

                    if (customObjectTemp != null)
                    {
                        d.AssociatedTemplateName = templateTemp.DESCRIZIONE;
                        d.CustomObjectId = customObjectTemp.SYSTEM_ID;
                        d.FieldId = "CONTATORE";
                        d.IsNumber = true;
                        d.Label = customObjectTemp.DESCRIZIONE;
                        d.OriginalLabel = customObjectTemp.DESCRIZIONE;
                        d.OracleDbColumnName = "to_number(getcontatoredocordinamento (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                        d.SqlServerDbColumnName = "@dbUser@.getContatoreDocOrdinamento(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
                        visibleFields.Insert(2, d);
                    }
                    else
                    {
                        visibleFields.Remove(d);
                    }
                }

                string documentDescriptionColor = string.Empty;
                // Individuazione del colore da assegnare alla descrizione del documento
                switch (new DocsPaWebService().getSegnAmm(UIManager.UserManager.GetInfoUser().idAmministrazione))
                {
                    case "0":
                        documentDescriptionColor = "Black";
                        break;
                    case "1":
                        documentDescriptionColor = "Blue";
                        break;
                    default:
                        documentDescriptionColor = "Red";
                        break;
                }

                dataTable.Rows.Remove(dataTable.Rows[0]);
                // Valore da assegnare ad un campo
                string value = string.Empty;
                // Per ogni risultato...
                // La riga da aggiungere al dataset

                DataRow dataRow = null;
                System.Text.StringBuilder temp;
                foreach (SearchObject doc in result)
                {
                    // ...viene inizializzata una nuova riga
                    dataRow = dataTable.NewRow();

                    foreach (Field field in visibleFields)
                    {
                        string numeroDocumento = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                        string numeroProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
                        string dataProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;

                        switch (field.FieldId)
                        {
                            //SEGNATURA
                            case "D8":
                                value = "<span style=\"color:Red; font-weight:bold;\">" + doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue + "</span>";
                                break;
                            //REGISTRO
                            case "D2":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //TIPO
                            case "D3":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_DOCUMENTO_PRINCIPALE")).FirstOrDefault().SearchObjectFieldValue;
                                string tempVal = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                if (!string.IsNullOrEmpty(value))
                                    value = labels.Where(e => e.Codice == "ALL").FirstOrDefault().Descrizione;
                                else
                                    value = labels.Where(e => e.Codice == tempVal).FirstOrDefault().Descrizione;
                                break;
                            //OGGETTO
                            case "D4":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //MITTENTE / DESTINATARIO
                            case "D5":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //MITTENTE
                            case "D6":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //DESTINATARI
                            case "D7":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //DATA
                            case "D9":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            // ESITO PUBBLICAZIONE
                            case "D10":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //DATA ANNULLAMENTO
                            case "D11":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //DOCUMENTO
                            case "D1":
                                // Inizializzazione dello stringbuilder con l'apertura del tag Span in
                                // cui inserire l'identiifcativo del documento
                                string dataApertura = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
                                string protTit = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("PROT_TIT")).FirstOrDefault().SearchObjectFieldValue;

                                temp = new System.Text.StringBuilder("<span style=\"color:");
                                // Se il documento è un protocollo viene colorato in rosso altrimenti
                                // viene colorato in nero
                                temp.Append(String.IsNullOrEmpty(numeroProtocollo) ? "Black" : documentDescriptionColor);
                                // Il testo deve essere grassetto
                                temp.Append("; font-weight:bold;\">");

                                // Creazione dell'informazione sul documento
                                if (!String.IsNullOrEmpty(numeroProtocollo))
                                    temp.Append(numeroProtocollo + "<br />" + dataProtocollo);
                                else
                                    temp.Append(numeroDocumento + "<br />" + dataApertura);

                                if (!String.IsNullOrEmpty(protTit))
                                    temp.Append("<br />" + protTit);

                                // Chiusura del tag span
                                temp.Append("</span>");

                                value = temp.ToString();
                                break;
                            //NUMERO PROTOCOLLO
                            case "D12":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //AUTORE
                            case "D13":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //DATA ARCHIVIAZIONE
                            case "D14":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //PERSONALE
                            case "D15":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                                    value = "Si";
                                else
                                    value = "No";
                                break;
                            //PRIVATO
                            case "D16":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                                    value = "Si";
                                else
                                    value = "No";
                                break;
                            //TIPOLOGIA
                            case "U1":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //NOTE
                            case "D17":
                                string valoreChiave = string.Empty;
                                //valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_IS_PRESENT_NOTE");

                                if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1"))
                                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ESISTE_NOTA")).FirstOrDefault().SearchObjectFieldValue;
                                else
                                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //DATE INSERT IN ADL
                            case "DTA_ADL":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //CONTATORE
                            case "CONTATORE":
                                value = string.Empty;
                                try
                                {
                                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                    //verifico se si tratta di un contatore di reertorio
                                    if (value.ToUpper().Equals("#CONTATORE_DI_REPERTORIO#"))
                                    {
                                        //reperisco la segnatura di repertorio
                                        string dNumber = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                                        value = DocumentManager.getSegnaturaRepertorio(dNumber, AdministrationManager.AmmGetInfoAmmCorrente(UserManager.GetInfoUser().idAmministrazione).Codice);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    UIManager.AdministrationManager.DiagnosticError(ex);
                                    return null;
                                }
                                break;
                            //COD. FASCICOLI
                            case "D18":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //Nome e cognome autore
                            case "D19":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //Ruolo autore
                            case "D20":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //Data arrivo
                            case "D21":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //Stato del documento
                            case "D22":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            case "IMPRONTA":
                                // IMPRONTA FILE
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            // PEC 4 Requisito 3: ricerca documenti spediti
                            // Esito della spedizione
                            case "esito_spedizione":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            // PEC 4 Requisito 3: ricerca documenti spediti
                            // Conto delle ricevute
                            case "count_ric_interop":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            // INTEGRAZIONE PITRE-PARER
                            case "stato_conservazione":
                                string codStato = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                switch (codStato)
                                {
                                    case "N":
                                        value = "Non conservato";
                                        break;
                                    case "V":
                                        value = "In attesa di versamento";
                                        break;
                                    case "W":
                                        value = "Versamento in corso";
                                        break;
                                    case "C":
                                        value = "Preso in carico";
                                        break;
                                    case "R":
                                        value = "Rifiutato";
                                        break;
                                    case "E":
                                        value = "Errore nell'invio";
                                        break;
                                    case "T":
                                        value = "Timeout nell'operazione";
                                        break;
                                }
                                break;
                            // STATO TASK
                            case "CHA_TASK_STATUS":
                                string status = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                switch (status)
                                {
                                    case "IN_PROGRESS":
                                        value = this.GetLabel("TaskStatusInProgress");
                                        break;
                                    case "CLOSED":
                                        value = this.GetLabel("TaskStatusClosed");
                                        break;
                                    default:
                                        value = this.GetLabel("TaskStatusNA");
                                        break;
                                }
                                break;
                            //OGGETTI CUSTOM
                            default:
                                SearchObjectField tempField = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault();
                                if (tempField != null)
                                {
                                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        // se l'ggetto custom è un contatore di repertorio visualizzo la segnatura di repertorio
                                        if (value.ToUpper().Equals("#CONTATORE_DI_REPERTORIO#"))
                                        {
                                            string idDoc = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                                            value = DocumentManager.getSegnaturaRepertorio(idDoc, UserManager.getInfoAmmCorrente(UserManager.GetInfoUser().idAmministrazione).Codice);
                                        }
                                    }
                                }
                                else
                                {
                                    value = string.Empty;
                                }
                                break;
                        }

                        // Valorizzazione del campo fieldName
                        // Se il documento è annullato, viene mostrato un testo barrato, altrimenti
                        // viene mostrato così com'è
                        string dataAnnullamento = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D11")).FirstOrDefault().SearchObjectFieldValue;

                        string itemTitle = numeroDocumento + " / " + dataProtocollo;
                        if (!String.IsNullOrEmpty(numeroProtocollo))
                            itemTitle = numeroProtocollo + " / " + dataProtocollo;

                        if (!String.IsNullOrEmpty(dataAnnullamento))
                            dataRow[field.FieldId] = String.Format("<span id=\"doc" + numeroDocumento + "\" class=\"jstree-draggable2\" title=\"" + itemTitle.Replace("\"", "&quot;") + "\" style=\"text-decoration: line-through; color: Red;\">{0}</span>", value);
                        else
                            dataRow[field.FieldId] = String.Format("<span id=\"doc" + numeroDocumento + "\" class=\"jstree-draggable2\" title=\"" + itemTitle.Replace("\"", "&quot;") + "\">{0}</span>", value);
                        value = string.Empty;
                    }

                    string immagineAcquisita = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D23")).FirstOrDefault().SearchObjectFieldValue;
                    string inConservazione = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_CONSERVAZIONE")).FirstOrDefault().SearchObjectFieldValue;
                    string inAdl = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_ADL")).FirstOrDefault().SearchObjectFieldValue;
                    string inAdlRole = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_ADLROLE")).FirstOrDefault().SearchObjectFieldValue;
                    string isFirmato = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("CHA_FIRMATO")).FirstOrDefault().SearchObjectFieldValue;
                    string inLibroFirma = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_LIBROFIRMA")).FirstOrDefault().SearchObjectFieldValue;
                    string signType = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("CHA_TIPO_FIRMA")).FirstOrDefault().SearchObjectFieldValue;

                    dataRow["ProtoType"] = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D3")).FirstOrDefault().SearchObjectFieldValue;
                    dataRow["IdProfile"] = doc.SearchObjectID;
                    dataRow["FileExtension"] = !String.IsNullOrEmpty(immagineAcquisita) && immagineAcquisita != "0" ? immagineAcquisita : String.Empty;
                    dataRow["IsInStorageArea"] = !String.IsNullOrEmpty(inConservazione) && inConservazione != "0" ? true : false;
                    dataRow["IsInWorkingArea"] = !String.IsNullOrEmpty(inAdl) && inAdl != "0" ? true : false;
                    dataRow["IsInWorkingAreaRole"] = !String.IsNullOrEmpty(inAdlRole) && inAdlRole != "0" ? true : false;
                    dataRow["IsSigned"] = !String.IsNullOrEmpty(isFirmato) && isFirmato != "0" ? true : false;
                    dataRow["InLibroFirma"] = !String.IsNullOrEmpty(inLibroFirma) && inLibroFirma != "0" ? true : false;

                    if (this.ListToSign != null && this.ListCheck != null && this.ListCheck.ContainsKey(doc.SearchObjectID))
                    {
                        FileToSign file = null;
                        string fileExstention = !String.IsNullOrEmpty(immagineAcquisita) && immagineAcquisita != "0" ? immagineAcquisita : String.Empty;
                        if (!this.ListToSign.ContainsKey(doc.SearchObjectID))
                        {
                            file = new FileToSign(fileExstention, isFirmato, signType);
                            this.ListToSign.Add(doc.SearchObjectID, file);
                        }
                        else
                        {
                            file = this.ListToSign[doc.SearchObjectID];
                            file.signed = isFirmato;
                            file.fileExtension = fileExstention;
                            file.signType = signType;
                        }
                    }

                    // ...aggiunta della riga alla collezione delle righe
                    dataTable.Rows.Add(dataRow);
                }
                return dataTable;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per l'inizializzazione del data set in base ai campi definiti nella 
        /// griglia
        /// </summary>
        /// <param name="selectedGrid">La griglia su cui basare la creazione del dataset</param>
        /// <returns></returns>
        public GridView HeaderGridView(Grid selectedGrid, Templates templateTemp, bool showGridPersonalization, GridView grid)
        {
            try
            {
                int position = 0;
                List<Field> fields = selectedGrid.Fields.Where(e => e.Visible).OrderBy(f => f.Position).ToList();
                OggettoCustom customObjectTemp = new OggettoCustom();

                if (templateTemp != null && !showGridPersonalization)
                {
                    customObjectTemp = templateTemp.ELENCO_OGGETTI.Where(
                         e => e.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && e.DA_VISUALIZZARE_RICERCA == "1").FirstOrDefault();

                    Field d = new Field();

                    if (customObjectTemp != null)
                    {
                        d.AssociatedTemplateName = templateTemp.DESCRIZIONE;
                        d.CustomObjectId = customObjectTemp.SYSTEM_ID;
                        d.FieldId = "CONTATORE";
                        d.IsNumber = true;
                        d.Label = customObjectTemp.DESCRIZIONE;
                        d.OriginalLabel = customObjectTemp.DESCRIZIONE;
                        d.OracleDbColumnName = "to_number(getcontatoredocordinamento (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                        d.SqlServerDbColumnName = "@dbUser@.getContatoreDocOrdinamento(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
                        fields.Insert(2, d);
                    }
                    else
                        fields.Remove(d);
                }

                grid.Columns.Clear();

                // Creazione delle colonne
                foreach (Field field in fields)
                {
                    BoundField column = null;
                    ButtonField columnHL = null;
                    TemplateField columnCKB = null;
                    if (field.OriginalLabel.ToUpper().Equals("DOCUMENTO"))
                        columnHL = GridManager.GetLinkColumn(field.Label,
                            field.FieldId,
                            field.Width);
                    else
                    {

                        if (field is SpecialField)
                        {
                            switch (((SpecialField)field).FieldType)
                            {
                                case SpecialFieldsEnum.Icons:
                                    columnCKB = GridManager.GetBoundColumnIcon(field.Label, field.Width, field.FieldId);
                                    break;
                                case SpecialFieldsEnum.CheckBox:
                                    {
                                        columnCKB = GridManager.GetBoundColumnCheckBox(field.Label, field.Width, field.FieldId);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (field.FieldId)
                            {
                                case "CONTATORE":
                                    {
                                        column = GridManager.GetBoundColumn(
                                            field.Label,
                                            field.OriginalLabel,
                                            100,
                                            field.FieldId);
                                        break;
                                    }

                                default:
                                    {
                                        column = GridManager.GetBoundColumn(
                                         field.Label,
                                         field.OriginalLabel,
                                         field.Width,
                                         field.FieldId);
                                        break;
                                    }
                            }
                        }
                    }

                    if (columnCKB != null)
                        grid.Columns.Add(columnCKB);
                    else
                        if (column != null)
                            grid.Columns.Add(column);
                        else
                            grid.Columns.Add(columnHL);


                    if (!this.CellPosition.ContainsKey(field.FieldId))
                    {
                        CellPosition.Add(field.FieldId, position);
                    }
                    // Aggiornamento della posizione
                    position += 1;
                }
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IdProfile", "IdProfile"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("FileExtension", "FileExtension"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsInStorageArea", "IsInStorageArea"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsInWorkingArea", "IsInWorkingArea"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("ProtoType", "ProtoType"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsSigned", "IsSigned"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("InLibroFirma", "InLibroFirma"));
                return grid;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        protected void gridViewResult_ItemCreated(Object sender, GridViewRowEventArgs e)
        {
            if (this.ShowGridPersonalization)
            {
                //Posizione della freccetta nell'header
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    System.Web.UI.WebControls.Image arrow = new System.Web.UI.WebControls.Image();

                    arrow.BorderStyle = BorderStyle.None;

                    if (GridManager.SelectedGrid.OrderDirection == OrderDirectionEnum.Asc)
                    {
                        arrow.ImageUrl = "../Images/Icons/arrow_up.gif";
                    }
                    else
                    {
                        arrow.ImageUrl = "../Images/Icons/arrow_down.gif";
                    }

                    if (GridManager.SelectedGrid.FieldForOrder != null)
                    {
                        Field d = (Field)GridManager.SelectedGrid.Fields.Where(f => f.Visible && f.FieldId.Equals(GridManager.SelectedGrid.FieldForOrder.FieldId)).FirstOrDefault();
                        if (d != null)
                        {
                            int cell = this.CellPosition[d.FieldId];
                            e.Row.Cells[cell].Controls.Add(arrow);
                        }
                    }
                }
            }

            if (e.Row.RowType.Equals(DataControlRowType.DataRow))
            {
                string idProfile = this.GrigliaResult.Rows[e.Row.DataItemIndex]["idProfile"].ToString();
                string codeProject = idProfile;
                try { codeProject = this.Result.Where(y => y.SearchObjectID.Equals(idProfile)).FirstOrDefault().SearchObjectField.Where(x => x.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue; }
                catch { }
                if (string.IsNullOrEmpty(codeProject))
                    codeProject = idProfile;

                CheckBox checkBox = e.Row.FindControl("checkDocumento") as CheckBox;
                if (checkBox != null)
                {
                    checkBox.CssClass = "pr" + idProfile;
                    checkBox.Attributes["onclick"] = "SetItemCheck(this, '" + idProfile + "_" + codeProject + "')";
                    if (this.ListCheck.ContainsKey(idProfile))
                        checkBox.Checked = true;
                    else
                        checkBox.Checked = false;
                }
            }
        }

        protected void gridViewResult_PreRender(object sender, EventArgs e)
        {
            CheckBox chkBxHeader = (CheckBox)this.gridViewResult.HeaderRow.FindControl("cb_selectall");
            if (chkBxHeader != null)
            {
                chkBxHeader.Checked = this.CheckAll;
            }

            bool isUnamovable = !(
                UserManager.IsAuthorizedFunctions("FASC_INS_DOC")
                && UserManager.IsAuthorizedFunctions("DO_DEL_DOC_FASC")
                && UserManager.IsAuthorizedFunctions("FASC_NEW_FOLDER")
                && UserManager.IsAuthorizedFunctions("FASC_MOD_FOLDER")
                && UserManager.IsAuthorizedFunctions("FASC_DEL_FOLDER")
                && ProjectManager.getProjectInSession() != null
                && Convert.ToInt32(ProjectManager.getProjectInSession().accessRights) >= Convert.ToInt32(HMdiritti.HMdiritti_Write)
                && string.IsNullOrEmpty(ProjectManager.getProjectInSession().chiusura)
                ) ? true : false;

            int cellsCount = 0;
            if (gridViewResult.Columns.Count > 0)
                foreach (DataControlField td in gridViewResult.Columns)
                    if (td.Visible) cellsCount++;

            bool alternateRow = false;
            int indexCellIcons = -1;
            if (cellsCount > 0)
            {
                for (int i = 1; i < gridViewResult.Rows.Count; i = i + 2)
                {
                    gridViewResult.Rows[i].CssClass = "NormalRow";
                    if (alternateRow) gridViewResult.Rows[i].CssClass = "AltRow";
                    alternateRow = !alternateRow;

                    for (int j = 0; j < gridViewResult.Rows[i].Cells.Count; j++)
                    {
                        bool found = false;
                        foreach (Control c in gridViewResult.Rows[i].Cells[j].Controls)
                        {
                            if (c.ID == "visualizzadocumento")
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                            gridViewResult.Rows[i].Cells[j].Visible = false;
                        else
                        {
                            gridViewResult.Rows[i].Cells[j].ColumnSpan = cellsCount - 1;
                            gridViewResult.Rows[i].Cells[j].Attributes["style"] = "text-align: right;";
                            indexCellIcons = j;
                        }
                    }
                }

                alternateRow = false;
                for (int i = 0; i < gridViewResult.Rows.Count; i = i + 2)
                {
                    int index = i / 2;
                    string numeroDocumento = this.Result[index].SearchObjectField.Where(e2 => e2.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                    string numeroProtocollo = this.Result[index].SearchObjectField.Where(e2 => e2.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
                    string dataProtocollo = this.Result[index].SearchObjectField.Where(e2 => e2.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;

                    string itemTitle = numeroDocumento + " / " + dataProtocollo;
                    if (!String.IsNullOrEmpty(numeroProtocollo))
                        itemTitle = numeroProtocollo + " / " + dataProtocollo;

                    gridViewResult.Rows[i].Attributes["id"] = "doc" + numeroDocumento;
                    gridViewResult.Rows[i].Attributes["title"] = utils.FormatHtml(itemTitle);
                    if (isUnamovable)
                        gridViewResult.Rows[i].CssClass = "NormalRow";
                    else
                        gridViewResult.Rows[i].CssClass = "NormalRow jstree-draggable";
                    if (alternateRow)
                        if (isUnamovable)
                            gridViewResult.Rows[i].CssClass = "AltRow";
                        else
                            gridViewResult.Rows[i].CssClass = "AltRow jstree-draggable";
                    alternateRow = !alternateRow;

                    for (int j = 0; j < gridViewResult.Rows[i].Cells.Count; j++)
                    {
                        bool found = false;
                        foreach (Control c in gridViewResult.Rows[i].Cells[j].Controls)
                        {
                            if (c.ID == "visualizzadocumento")
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found)
                            gridViewResult.Rows[i].Cells[j].Visible = false;
                        else
                            gridViewResult.Rows[i].Cells[j].Attributes["style"] = "text-align: center;";
                    }
                }
                if (indexCellIcons > -1)
                    gridViewResult.HeaderRow.Cells[indexCellIcons].Visible = false;
                for (int j = 0; j < gridViewResult.HeaderRow.Cells.Count; j++)
                    gridViewResult.HeaderRow.Cells[j].Attributes["style"] = "text-align: center;";
            }

            if (!string.IsNullOrEmpty(this.SelectedRow) && !isUnamovable)
            {
                for (int i = 0; i < gridViewResult.Rows.Count; i++)
                {
                    if (this.gridViewResult.Rows[i].RowIndex == Int32.Parse(this.SelectedRow))
                    {
                        this.gridViewResult.Rows[i].Attributes.Remove("class");
                        this.gridViewResult.Rows[i].CssClass = "jstree-draggable-selected";
                        this.gridViewResult.Rows[i - 1].Attributes.Remove("class");
                        this.gridViewResult.Rows[i - 1].CssClass = "jstree-draggable jstree-draggable-selected";
                    }
                }

            }

            //MEV EMEA: per tipologie con multivalore con colore, vado a colorare la cella con il rispettivo colore
            for (int i = 0; i < gridViewResult.Columns.Count; i++)
            {
                string textColumn = gridViewResult.Columns[i].HeaderText;
                if (!string.IsNullOrEmpty(textColumn))
                {
                    Field field = (from f in GridManager.SelectedGrid.Fields
                                   where f.Label.Equals(textColumn) && !string.IsNullOrEmpty(f.AssociatedTemplateId) && f.Values != null && f.Values.Count() > 0
                                   select f).FirstOrDefault();
                    if (field != null)
                    {

                        string valueDoc = string.Empty;
                        for (int j = 0; j < gridViewResult.Rows.Count; j++)
                        {
                            valueDoc = this.gridViewResult.Rows[j].Cells[i].Text;
                            string htmlString = @valueDoc;
                            htmlString = Regex.Replace(htmlString, @"<(.|\n)*?>", "");
                            string color = (from value in field.Values
                                            where htmlString.Equals(value.Value)
                                            select value.ColorBG).FirstOrDefault();

                            if (!string.IsNullOrEmpty(color))
                            {
                                string[] colorSplit = color.Split('^');
                                this.gridViewResult.Rows[j].Cells[i].BackColor = System.Drawing.Color.FromArgb(Convert.ToInt16(colorSplit[0]), Convert.ToInt16(colorSplit[1]), Convert.ToInt16(colorSplit[2]));
                            }
                        }
                    }
                }
            }
            //FINE MEV EMEA

            // grid width
            int fullWidth = 0;
            foreach (Field field in GridManager.SelectedGrid.Fields.Where(u => u.Visible).OrderBy(f => f.Position).ToList())
                fullWidth += field.Width;
            this.gridViewResult.Attributes["style"] = "width: " + fullWidth + "px;";
        }

        protected void addAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.gridViewResult.HeaderRow.FindControl("cb_selectall") != null)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                    if (this.IdProfileList != null)
                    {
                        bool value = ((CheckBox)this.gridViewResult.HeaderRow.FindControl("cb_selectall")).Checked;
                        for (int i = 0; i < this.IdProfileList.Length; i++)
                        {
                            if (value)
                            {

                                if (!this.ListCheck.ContainsKey(this.IdProfileList[i]))
                                {
                                    this.ListCheck.Add(this.IdProfileList[i], this.CodeProfileList[i]);
                                }
                            }
                            else
                            {
                                if (this.ListCheck.ContainsKey(this.IdProfileList[i]))
                                {
                                    this.ListCheck.Remove(this.IdProfileList[i]);
                                }
                            }
                        }

                        this.CheckAll = value;

                        foreach (GridViewRow dgItem in this.gridViewResult.Rows)
                        {
                            CheckBox checkBox = dgItem.FindControl("checkDocumento") as CheckBox;
                            checkBox.Checked = value;
                        }

                        if (this.CheckAll)
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "clearCheckboxes", "clearCheckboxes('true', '');", true);

                        if (this.CheckAll)
                            this.HiddenItemsAll.Value = "true";
                        else
                            this.HiddenItemsAll.Value = string.Empty;
                        this.upPnlButtons.Update();
                    }

                    this.UpnlGrid.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void SetCheckBox()
        {
            try
            {
                bool checkAll = this.CheckAll;

                if (!string.IsNullOrEmpty(this.HiddenItemsChecked.Value))
                {
                    //salvo i check spuntati alla pagina cliccata in precedenza
                    string[] items = new string[1] { this.HiddenItemsChecked.Value };
                    if (this.HiddenItemsChecked.Value.IndexOf(",") > 0)
                        items = this.HiddenItemsChecked.Value.Split(',');

                    foreach (string item in items)
                    {
                        string key = item.Split('_')[0];
                        string value = item.Split('_')[1].Replace("<span style=\"color:Black;\">", "").Replace("</span>", "");
                        if (!this.ListCheck.ContainsKey(key))
                            this.ListCheck.Add(key, value);
                    }
                }


                if (!string.IsNullOrEmpty(this.HiddenItemsUnchecked.Value))
                {
                    this.CheckAll = false;

                    // salvo i check non spuntati alla pagina cliccata in precedenza
                    string[] items = new string[1] { this.HiddenItemsUnchecked.Value };
                    if (this.HiddenItemsUnchecked.Value.IndexOf(",") > 0)
                        items = this.HiddenItemsUnchecked.Value.Split(',');

                    foreach (string item in items)
                    {
                        string key = item.Split('_')[0];
                        string value = item.Split('_')[1];
                        if (this.ListCheck.ContainsKey(key))
                            this.ListCheck.Remove(key);
                    }
                }


                if (string.IsNullOrEmpty(this.HiddenItemsAll.Value))
                {
                    string js = string.Empty;
                    foreach (KeyValuePair<string, string> d in this.ListCheck)
                    {
                        if (!string.IsNullOrEmpty(js)) js += ",";
                        js += d.Key;
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "clearCheckboxes", "clearCheckboxes('false', '" + js + "');", true);
                }

                this.HiddenItemsChecked.Value = string.Empty;
                this.HiddenItemsUnchecked.Value = string.Empty;
                this.upPnlButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Questa funzione si occupa di ricercare i documenti e di visualizzare 
        /// i dati
        /// </summary>
        private void SearchDocumentsAndDisplayResult(FiltroRicerca[][] searchFilters, int selectedPage, Grid selectedGrid, EtichettaInfo[] labels, Folder folder,
            FiltroRicerca[][] orderFilters, bool ricerca)
        {
            // Numero di record restituiti dalla pagina
            int recordNumber = 0;

            // Risultati restituiti dalla ricerca
            SearchObject[] result;

            // Ricerca dei documenti
            result = this.SearchDocument(searchFilters, this.SelectedPage, out recordNumber, folder, orderFilters, selectedGrid);

            // Se ci sono risultati, vengono visualizzati
            if (this.Result != null && this.Result.Length > 0)
            {
                this.ShowResult(selectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());

            }
            else
            {
                this.ShowGrid(selectedGrid, null, 0, 0, labels);
            }

            this.docsInFolderCount.Text = this.RecordCount.ToString();
            this.upPnlStruttura.Update();

        }

        /// <summary>
        /// Funzione per la ricerca dei documenti
        /// </summary>
        /// <param name="searchFilters">Filtri da utilizzare per la ricerca</param>
        /// <param name="selectedPage">Pagina da caricare</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        /// <param name="folder">Folder in cui ricercare</param>
        /// <returns>Lista delle informazioni sui documenti restituiti dalla ricerca</returns>
        private SearchObject[] SearchDocument(FiltroRicerca[][] searchFilters,
            int selectedPage, out  int recordNumber, Folder folder,
            FiltroRicerca[][] orderFilters, Grid SelectedGrid)
        {
            // Documenti individuati dalla ricerca
            SearchObject[] toReturn;

            // Recupero dei campi della griglia impostati come visibili
            Field[] visibleArray = null;
            List<Field> visibleFieldsTemplate;

            int pageNumbers = 0;

            // Lista dei system id dei documenti restituiti dalla ricerca
            SearchResultInfo[] idProfiles;

            List<Field> visibleFields = GridManager.SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field))).ToList();
            Field specialField = GridManager.SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(SpecialField)) && ((SpecialField)e).FieldType.Equals(SpecialFieldsEnum.Icons)).FirstOrDefault<Field>();

            Templates templateTemp = Session["templateRicerca"] as Templates;

            OggettoCustom customObjectTemp = new OggettoCustom();

            if (templateTemp != null && !this.ShowGridPersonalization)
            {
                customObjectTemp = templateTemp.ELENCO_OGGETTI.Where(
                     e => e.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && e.DA_VISUALIZZARE_RICERCA == "1").FirstOrDefault();

                Field d = new Field();

                if (customObjectTemp != null)
                {
                    d.AssociatedTemplateName = templateTemp.DESCRIZIONE;
                    d.CustomObjectId = customObjectTemp.SYSTEM_ID;
                    d.FieldId = "CONTATORE";
                    d.IsNumber = true;
                    d.Label = customObjectTemp.DESCRIZIONE;
                    d.OriginalLabel = customObjectTemp.DESCRIZIONE;
                    d.OracleDbColumnName = "to_number(getcontatoredocordinamento (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                    d.SqlServerDbColumnName = "@dbUser@.getContatoreDocOrdinamento(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
                    visibleFields.Insert(2, d);
                }
                else
                {
                    visibleFields.Remove(d);
                }
            }

            //visibleFields = GridManager.SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field)) && e.CustomObjectId != 0).ToList();

            if (visibleFields != null && visibleFields.Count > 0)
            {
                visibleArray = visibleFields.ToArray();
            }

            toReturn = UIManager.ProjectManager.getListaDocumentiPagingCustom(
                    folder,
                    searchFilters,
                    selectedPage,
                    out pageNumbers,
                    out recordNumber,
                    true,
                    this.ShowGridPersonalization,
                    false,
                    visibleArray,
                    null,
                    this.PageSize,
                    orderFilters,
                    out idProfiles);

            this.RecordCount = recordNumber;
            this.PagesCount = pageNumbers + 1;
            this.Result = toReturn;

            //luluciani 3.16.x 2.7.3
            //da ricerche salvate se trova zero elementi l'oggetto REsult è null, invece 
            //da ricerca nomrale anche se zero risultati torna un elemento vuoto ma non null
            if (this.Result == null || this.Result.Length == 0)
            {
                this.SearchDocumentDdlMassiveOperation.Enabled = false;
            }
            else
            {
                this.SearchDocumentDdlMassiveOperation.Enabled = true;
            }

            //appoggio il risultato in sessione.
            if (idProfiles != null && idProfiles.Length > 0)
            {
                this.IdProfileList = new string[idProfiles.Length];
                this.CodeProfileList = new string[idProfiles.Length];
                for (int i = 0; i < idProfiles.Length; i++)
                {
                    this.IdProfileList[i] = idProfiles[i].Id;
                    this.CodeProfileList[i] = idProfiles[i].Id;
                }
            }

            // Restituzione della lista di documenti da visualizzare
            return toReturn;

        }

        /// <summary>
        /// Funzione per la visualizzazione dei risutati della ricerca
        /// </summary>
        /// <param name="result">I risultati della ricerca</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private void ShowResult(Grid selectedGrid, SearchObject[] result, int recordNumber, int selectedPage, EtichettaInfo[] labels)
        {
            this.ShowGrid(selectedGrid, this.Result, this.RecordCount, selectedPage, labels);
            this.gridViewResult.SelectedIndex = this.SelectedPage;
            this.BuildGridNavigator();

        }

        protected void gridViewResult_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortExpression = e.SortExpression.ToString();

            Field d = new Field();

            Templates templateTemp = Session["templateRicerca"] as Templates;

            OggettoCustom customObjectTemp = new OggettoCustom();

            if (templateTemp != null && !this.ShowGridPersonalization && sortExpression.Equals("CONTATORE"))
            {
                customObjectTemp = templateTemp.ELENCO_OGGETTI.Where(
                     g => g.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && g.DA_VISUALIZZARE_RICERCA == "1").FirstOrDefault();

                if (customObjectTemp != null)
                {
                    d.AssociatedTemplateName = templateTemp.DESCRIZIONE;
                    d.CustomObjectId = customObjectTemp.SYSTEM_ID;
                    d.FieldId = customObjectTemp.SYSTEM_ID.ToString();
                    d.IsNumber = true;
                    d.Label = customObjectTemp.DESCRIZIONE;
                    d.OriginalLabel = customObjectTemp.DESCRIZIONE;
                    d.OracleDbColumnName = "to_number(getcontatoredocordinamento (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                    d.SqlServerDbColumnName = "@dbUser@.getContatoreDocOrdinamento(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
                }
            }
            else
            {
                d = (Field)GridManager.SelectedGrid.Fields.Where(f => f.Visible && f.FieldId.Equals(sortExpression)).FirstOrDefault();
            }

            if (d != null)
            {
                if (GridManager.SelectedGrid.FieldForOrder != null && (GridManager.SelectedGrid.FieldForOrder.FieldId).Equals(d.FieldId))
                {
                    if (GridManager.SelectedGrid.OrderDirection == OrderDirectionEnum.Asc)
                    {
                        GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Desc;
                    }
                    else
                    {
                        GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
                    }
                }
                else
                {
                    if (GridManager.SelectedGrid.FieldForOrder == null && d.FieldId.Equals("D9"))
                    {
                        GridManager.SelectedGrid.FieldForOrder = d;
                        if (GridManager.SelectedGrid.OrderDirection == OrderDirectionEnum.Asc)
                        {
                            GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Desc;
                        }
                        else
                        {
                            GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
                        }
                    }
                    else
                    {
                        GridManager.SelectedGrid.FieldForOrder = d;
                        GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
                    }
                }
                GridManager.SelectedGrid.GridId = string.Empty;


                this.SelectedPage = 1;
                Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();

                // Se ci sono risultati, vengono visualizzati
                //if (this.Result != null && this.Result.Length > 0)
                //{
                //    this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());

                //}
                //else
                //{
                //    this.ShowGrid(GridManager.SelectedGrid, null, 0, 0, this.Label);
                //}

                this.SelectedPage = 1;
                this.SearchDocumentsAndDisplayResult(this.SearchFilters, SelectedPage, GridManager.SelectedGrid, Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);

                this.BuildGridNavigator();
                this.UpnlNumerodocumenti.Update();
                this.UpnlGrid.Update();
                this.upPnlGridIndexes.Update();

            }
        }

        protected void gridViewResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType.Equals(DataControlRowType.DataRow))
            {
                string idProfile = GrigliaResult.Rows[e.Row.DataItemIndex]["IdProfile"].ToString();

                bool ok = false;
                string labelConservazione = "ProjectIconTemplateRemoveConservazione";
                string labelAdl = "ProjectIconTemplateRemoveAdl";
                string labelAdlRole = "ProjectIconTemplateRemoveAdlRole";
                //imagini delle icone
                if (bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsInStorageArea"].ToString()))
                {
                    ((CustomImageButton)e.Row.FindControl("conservazione")).ImageUrl = "../Images/Icons/remove_preservation_grid.png";
                    ((CustomImageButton)e.Row.FindControl("conservazione")).OnMouseOutImage = "../Images/Icons/remove_preservation_grid.png";
                    ((CustomImageButton)e.Row.FindControl("conservazione")).OnMouseOverImage = "../Images/Icons/remove_preservation_grid_hover.png";
                    ((CustomImageButton)e.Row.FindControl("conservazione")).ImageUrlDisabled = "../Images/Icons/remove_preservation_grid_disabled.png";
                }
                else
                {
                    labelConservazione = "ProjectIconTemplateConservazione";
                    ((CustomImageButton)e.Row.FindControl("conservazione")).ImageUrl = "../Images/Icons/add_preservation_grid.png";
                    ((CustomImageButton)e.Row.FindControl("conservazione")).OnMouseOutImage = "../Images/Icons/add_preservation_grid.png";
                    ((CustomImageButton)e.Row.FindControl("conservazione")).OnMouseOverImage = "../Images/Icons/add_preservation_grid_hover.png";
                    ((CustomImageButton)e.Row.FindControl("conservazione")).ImageUrlDisabled = "../Images/Icons/add_preservation_grid_disabled.png";
                }

                ((CustomImageButton)e.Row.FindControl("adlrole")).Visible = this.AllowADLRole;
                if (EnabledLibroFirma && EnableViewInfoProcessesStarted)
                {
                    if (bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["InLibroFirma"].ToString()))
                        ((CustomImageButton)e.Row.FindControl("visualizzaProcessiFirmaAvviati")).Visible = true;

                    ((CustomImageButton)e.Row.FindControl("visualizzaStatoProcessiFirmaAvviati")).Visible = true;
                }
                if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
                {
                    if (bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsInWorkingAreaRole"].ToString()))
                    {
                        ((CustomImageButton)e.Row.FindControl("adl")).Enabled = false;

                        ((CustomImageButton)e.Row.FindControl("adlrole")).ImageUrl = "../Images/Icons/adl1x.png";
                        ((CustomImageButton)e.Row.FindControl("adlrole")).OnMouseOutImage = "../Images/Icons/adl1x.png";
                        ((CustomImageButton)e.Row.FindControl("adlrole")).OnMouseOverImage = "../Images/Icons/adl1x_hover.png";
                        ((CustomImageButton)e.Row.FindControl("adlrole")).ImageUrlDisabled = "../Images/Icons/adl1x_disabled.png";
                    }
                    else
                    {
                        if (bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsInWorkingArea"].ToString()))
                            labelAdlRole = "ProjectIconTemplateAdlRole";
                        else
                            labelAdlRole = "ProjectIconTemplateAdlRoleInsert";
                        ((CustomImageButton)e.Row.FindControl("adl")).Enabled = true;
                        ((CustomImageButton)e.Row.FindControl("adlrole")).ImageUrl = "../Images/Icons/adl1.png";
                        ((CustomImageButton)e.Row.FindControl("adlrole")).OnMouseOutImage = "../Images/Icons/adl1.png";
                        ((CustomImageButton)e.Row.FindControl("adlrole")).OnMouseOverImage = "../Images/Icons/adl1_hover.png";
                        ((CustomImageButton)e.Row.FindControl("adlrole")).ImageUrlDisabled = "../Images/Icons/adl1_disabled.png";
                    }
                }

                if (bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsInWorkingArea"].ToString()))
                {
                    ((CustomImageButton)e.Row.FindControl("adl")).ImageUrl = "../Images/Icons/adl2x.png";
                    ((CustomImageButton)e.Row.FindControl("adl")).OnMouseOutImage = "../Images/Icons/adl2x.png";
                    ((CustomImageButton)e.Row.FindControl("adl")).OnMouseOverImage = "../Images/Icons/adl2x_hover.png";
                    ((CustomImageButton)e.Row.FindControl("adl")).ImageUrlDisabled = "../Images/Icons/adl2x_disabled.png";
                }
                else
                {
                    labelAdl = "ProjectIconTemplateAdl";
                    ((CustomImageButton)e.Row.FindControl("adl")).ImageUrl = "../Images/Icons/adl2.png";
                    ((CustomImageButton)e.Row.FindControl("adl")).OnMouseOutImage = "../Images/Icons/adl2.png";
                    ((CustomImageButton)e.Row.FindControl("adl")).OnMouseOverImage = "../Images/Icons/adl2_hover.png";
                    ((CustomImageButton)e.Row.FindControl("adl")).ImageUrlDisabled = "../Images/Icons/adl2_disabled.png";
                }

                string estensione = GrigliaResult.Rows[e.Row.RowIndex]["FileExtension"].ToString();
                if (!string.IsNullOrEmpty(estensione))
                {
                    string imgUrl = ResolveUrl(FileManager.getFileIcon(this, estensione));
                    ((CustomImageButton)e.Row.FindControl("estensionedoc")).Visible = true;
                    ((CustomImageButton)e.Row.FindControl("estensionedoc")).ImageUrl = imgUrl;
                    ((CustomImageButton)e.Row.FindControl("estensionedoc")).OnMouseOutImage = imgUrl;
                    ((CustomImageButton)e.Row.FindControl("estensionedoc")).OnMouseOverImage = imgUrl;
                }
                else
                    ((CustomImageButton)e.Row.FindControl("estensionedoc")).Visible = false;

                //visible icone
                bool delete = this.AllowDeleteDocFromProject;
                bool enableCreateDocumentAnswer = true;
                ((CustomImageButton)e.Row.FindControl("eliminadocumento")).Visible = delete;

                if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ((!string.IsNullOrEmpty(ProjectManager.getProjectInSession().accessRights) && Convert.ToInt32(ProjectManager.getProjectInSession().accessRights) < Convert.ToInt32(HMdiritti.HMdiritti_Write)) || ProjectManager.getProjectInSession().stato.Equals("C")))
                {
                    delete = false;
                    enableCreateDocumentAnswer = false;
                }

                if (EnabledCreateDocumentAnswer && IsRoleOutwardEnabled() && GrigliaResult.Rows[e.Row.RowIndex]["ProtoType"].ToString().Equals(PROTOCOLLO_IN_ARRIVO))
                {
                    ((CustomImageButton)e.Row.FindControl("newDocumentAnswer")).Visible = true;
                }
                ((CustomImageButton)e.Row.FindControl("eliminadocumento")).Enabled = delete;
                ((CustomImageButton)e.Row.FindControl("newDocumentAnswer")).Enabled = enableCreateDocumentAnswer;
                ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).Enabled = true;
                ((CustomImageButton)e.Row.FindControl("conservazione")).Visible = this.AllowConservazione && !this.IsConservazioneSACER;
                ((CustomImageButton)e.Row.FindControl("adl")).Visible = this.AllowADL;
                ((CustomImageButton)e.Row.FindControl("adlrole")).Visible = this.AllowADLRole;
                ((CustomImageButton)e.Row.FindControl("firmato")).Visible = bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsSigned"].ToString());

                //evento click
                ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).Click += new ImageClickEventHandler(ImageButton_Click);
                ((CustomImageButton)e.Row.FindControl("eliminadocumento")).Click += new ImageClickEventHandler(ImageButton_Click);
                ((CustomImageButton)e.Row.FindControl("conservazione")).Click += new ImageClickEventHandler(ImageButton_Click);
                ((CustomImageButton)e.Row.FindControl("adl")).Click += new ImageClickEventHandler(ImageButton_Click);
                ((CustomImageButton)e.Row.FindControl("adlrole")).Click += new ImageClickEventHandler(ImageButton_Click);
                ((CustomImageButton)e.Row.FindControl("firmato")).Click += new ImageClickEventHandler(ImageButton_Click);
                ((CustomImageButton)e.Row.FindControl("estensionedoc")).Click += new ImageClickEventHandler(ImageButton_Click);
                ((CustomImageButton)e.Row.FindControl("visualizzaProcessiFirmaAvviati")).Click += new ImageClickEventHandler(ImageButton_Click);
                ((CustomImageButton)e.Row.FindControl("visualizzaStatoProcessiFirmaAvviati")).Click += new ImageClickEventHandler(ImageButton_Click);
                ((CustomImageButton)e.Row.FindControl("newDocumentAnswer")).Click += new ImageClickEventHandler(ImageButton_Click);
                //((CustomImageButton)e.Row.FindControl("estensionedoc")).OnClientClick="return ajaxModalPopupDocumentViewer();";
                //tooltip
                ((CustomImageButton)e.Row.FindControl("estensionedoc")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateEstensioneDoc", UIManager.UserManager.GetUserLanguage()) + " " + estensione;
                ((CustomImageButton)e.Row.FindControl("conservazione")).ToolTip = Utils.Languages.GetLabelFromCode(labelConservazione, UIManager.UserManager.GetUserLanguage());
                ((CustomImageButton)e.Row.FindControl("adl")).ToolTip = Utils.Languages.GetLabelFromCode(labelAdl, UIManager.UserManager.GetUserLanguage());
                ((CustomImageButton)e.Row.FindControl("adlrole")).ToolTip = Utils.Languages.GetLabelFromCode(labelAdlRole, UIManager.UserManager.GetUserLanguage());
                ((CustomImageButton)e.Row.FindControl("firmato")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateFirmato", UIManager.UserManager.GetUserLanguage());
                ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateVisualizzaDocumento", UIManager.UserManager.GetUserLanguage());
                ((CustomImageButton)e.Row.FindControl("eliminadocumento")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateEliminaDocumento", UIManager.UserManager.GetUserLanguage());
                ((CustomImageButton)e.Row.FindControl("visualizzaProcessiFirmaAvviati")).ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgInfoProcessiAvviati", UIManager.UserManager.GetUserLanguage());
                ((CustomImageButton)e.Row.FindControl("visualizzaStatoProcessiFirmaAvviati")).ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgProcessStateTooltip", UIManager.UserManager.GetUserLanguage());
                ((CustomImageButton)e.Row.FindControl("newDocumentAnswer")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectAnswerWithProtocol", UIManager.UserManager.GetUserLanguage());
            }
        }



        //evento delle icone della griglia
        protected void ImageButton_Click(object sender, ImageClickEventArgs e)
        {
            CustomImageButton btnIm = (CustomImageButton)sender;
            GridViewRow row = (GridViewRow)btnIm.Parent.Parent;
            int rowIndex = row.RowIndex;
            string idProfile = GrigliaResult.Rows[rowIndex]["IdProfile"].ToString();
            SchedaDocumento schedaDocumento = UIManager.DocumentManager.getDocumentDetails(this, idProfile, idProfile);
            Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
            InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
            bool search = false;
            string language = UIManager.UserManager.GetUserLanguage();

            if (!string.IsNullOrEmpty(this.SelectedRow))
            {
                if (rowIndex != Int32.Parse(this.SelectedRow))
                {
                    this.SelectedRow = string.Empty;
                }
            }

            switch (btnIm.ID)
            {
                case "conservazione":
                    {
                        // Se il documento ha un file acquisito...
                        if (int.Parse(schedaDocumento.documenti[0].fileSize) > 0)
                        {
                            int resultIndex = Result.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.SearchObjectID.Equals(idProfile)).index;
                            int resultIndex2 = Result[resultIndex].SearchObjectField.Select((item2, j) => new { obj2 = item2, index2 = j }).First(item2 => item2.obj2.SearchObjectFieldID.Equals("IN_CONSERVAZIONE")).index2;

                            if (string.IsNullOrEmpty(schedaDocumento.inConservazione))
                            {
                                //MEV CS 1.5 - F03_01
                                #region oldCode
                                //ProjectManager.InsertDocumentInStorageArea(idProfile, schedaDocumento, infoUtente);
                                #endregion

                                #region NewCode
                                ProjectManager.InsertDocumentInStorageArea_WithConstraint(idProfile, schedaDocumento, infoUtente);
                                #endregion
                                // End MEV

                                GrigliaResult.Rows[rowIndex]["IsInStorageArea"] = true;
                                Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "0";

                                btnIm.ImageUrl = "../Images/Icons/remove_preservation_grid.png";
                                btnIm.OnMouseOutImage = "../Images/Icons/remove_preservation_grid.png";
                                btnIm.OnMouseOverImage = "../Images/Icons/remove_preservation_grid_hover.png";
                                btnIm.ImageUrlDisabled = "../Images/Icons/remove_preservation_grid_disabled.png";
                                btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveConservazione", language);
                                btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveConservazione", language);
                            }
                            else
                            {
                                ProjectManager.RemoveDocumentFromStorageArea(schedaDocumento, infoUtente);
                                GrigliaResult.Rows[rowIndex]["IsInStorageArea"] = false;
                                Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "1";
                                btnIm.ImageUrl = "../Images/Icons/add_preservation_grid.png";
                                btnIm.OnMouseOutImage = "../Images/Icons/add_preservation_grid.png";
                                btnIm.OnMouseOverImage = "../Images/Icons/add_preservation_grid_hover.png";
                                btnIm.ImageUrlDisabled = "../Images/Icons/add_preservation_grid_disabled.png";
                                btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateConservazione", language);
                                btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateConservazione", language);
                            }
                        }
                        else
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorProjectAddDocInConservazione', 'warning', '');} else {parent.ajaxDialogModal('ErrorProjectAddDocInConservazione', 'warning', '');}", true);
                        break;
                    }
                case "adl":
                    {
                        int resultIndex = Result.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.SearchObjectID.Equals(idProfile)).index;
                        int resultIndex2 = Result[resultIndex].SearchObjectField.Select((item2, j) => new { obj2 = item2, index2 = j }).First(item2 => item2.obj2.SearchObjectFieldID.Equals("IN_ADL")).index2;

                        if (bool.Parse(GrigliaResult.Rows[rowIndex]["IsInWorkingArea"].ToString()))
                        {
                            DocumentManager.eliminaDaAreaLavoro(this, idProfile, null);
                            GrigliaResult.Rows[rowIndex]["IsInWorkingArea"] = false;
                            Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "0";
                            btnIm.ImageUrl = "../Images/Icons/adl2.png";
                            btnIm.OnMouseOutImage = "../Images/Icons/adl2.png";
                            btnIm.OnMouseOverImage = "../Images/Icons/adl2_hover.png";
                            btnIm.ImageUrlDisabled = "../Images/Icons/adl2_disabled.png";
                            btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateAdl", language);
                            btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateAdl", language);
                        }
                        else
                        {
                            DocumentManager.addAreaLavoro(this, schedaDocumento);
                            GrigliaResult.Rows[rowIndex]["IsInWorkingArea"] = true;
                            Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "1";
                            btnIm.ImageUrl = "../Images/Icons/adl2x.png";
                            btnIm.OnMouseOutImage = "../Images/Icons/adl2x.png";
                            btnIm.OnMouseOverImage = "../Images/Icons/adl2x_hover.png";
                            btnIm.ImageUrlDisabled = "../Images/Icons/adl2x_disabled.png";
                            btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveAdl", language);
                            btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveAdl", language);
                        }
                        search = true;
                        break;
                    }
                case "adlrole":
                    {
                        int resultIndex = Result.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.SearchObjectID.Equals(idProfile)).index;
                        int resultIndex2 = Result[resultIndex].SearchObjectField.Select((item2, j) => new { obj2 = item2, index2 = j }).First(item2 => item2.obj2.SearchObjectFieldID.Equals("IN_ADLROLE")).index2;

                        if (bool.Parse(GrigliaResult.Rows[rowIndex]["IsInWorkingAreaRole"].ToString()))
                        {
                            DocumentManager.eliminaDaAreaLavoroRole(this, idProfile, null);
                            GrigliaResult.Rows[rowIndex]["IsInWorkingAreaRole"] = false;
                            Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "0";
                            btnIm.ImageUrl = "../Images/Icons/adl1.png";
                            btnIm.OnMouseOutImage = "../Images/Icons/adl1.png";
                            btnIm.OnMouseOverImage = "../Images/Icons/adl1_hover.png";
                            btnIm.ImageUrlDisabled = "../Images/Icons/adl1_disabled.png";
                            btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateAdl", language);
                            btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateAdl", language);
                            //if (this.IsAdl)
                            //{
                            //    this.SelectedRow = string.Empty;
                            //    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                            //}
                        }
                        else
                        {
                            DocumentManager.addAreaLavoroRole(this, schedaDocumento);
                            GrigliaResult.Rows[rowIndex]["IsInWorkingAreaRole"] = true;
                            Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "1";
                            btnIm.ImageUrl = "../Images/Icons/adl1x.png";
                            btnIm.OnMouseOutImage = "../Images/Icons/adl1x.png";
                            btnIm.OnMouseOverImage = "../Images/Icons/adl1x_hover.png";
                            btnIm.ImageUrlDisabled = "../Images/Icons/adl1x_disabled.png";
                            btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveAdl", language);
                            btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveAdl", language);
                        }
                        search = true;
                        break;
                    }
                case "firmato":
                    {
                        UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                        FileManager.setSelectedFile(schedaDocumento.documenti[0]);
                        DocumentManager.removeSelectedNumberVersion();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDigitalSignDetails", "ajaxModalPopupDigitalSignDetails();", true);
                        break;
                    }
                case "visualizzadocumento":
                    {
                        HttpContext.Current.Session["isZoom"] = null;
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject obj = navigationList.Last();
                        Navigation.NavigationObject newObj = Navigation.NavigationUtils.CloneObject(obj);

                        newObj.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), string.Empty);
                        newObj.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), true,this.Page);
                        newObj.CodePage = Navigation.NavigationUtils.NamePage.PROJECT.ToString();
                        newObj.Page = "PROJECT.ASPX";
                        newObj.IdObject = schedaDocumento.systemId;
                        newObj.OriginalObjectId = schedaDocumento.systemId;
                        newObj.NumPage = this.SelectedPage.ToString();
                        newObj.PageSize = this.PageSize.ToString();
                        newObj.DxTotalPageNumber = this.PagesCount.ToString();
                        newObj.DxTotalNumberElement = this.RecordCount.ToString();
                        newObj.SearchFilters = this.SearchFilters;
                        newObj.SearchFiltersOrder = UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid);
                        newObj.ViewResult = true;
                        newObj.folder = fascicolo.folderSelezionato;
                        newObj.idProject = fascicolo.systemID;
                        int indexElement = ((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1);
                        newObj.DxPositionElement = indexElement.ToString();

                        if (obj.NamePage.Equals(Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), string.Empty)) && !string.IsNullOrEmpty(obj.IdObject) && obj.IdObject.Equals(newObj.idProject))
                        {
                            navigationList.Remove(obj);
                        }
                        navigationList.Add(newObj);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);

                        UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                        this.ListObjectNavigation = this.Result;
                        Response.Redirect("~/Document/Document.aspx");
                        break;
                    }
                case "eliminadocumento":
                    {
                        string messaggio = string.Empty;
                        bool classificationRequired = this.RapidClassificationRequired;

                        if (this.RapidClassificationRequiredByTypeDoc)
                        {
                            classificationRequired = DocumentManager.IsClassificationRqueredByTypeDoc(schedaDocumento.tipoProto);
                        }

                        ValidationResultInfo risultato = UIManager.ProjectManager.deleteDocFromFolder(fascicolo.folderSelezionato, infoUtente, idProfile, classificationRequired.ToString(), out messaggio);
                        if (risultato != null && risultato.BrokenRules.Length > 0)
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('AlertProjectRemoveDoc', 'error', '');} else {parent.ajaxDialogModal('AlertProjectRemoveDoc', 'error', '');}", true);
                        else
                            if (!string.IsNullOrEmpty(messaggio))
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorProjectRemoveDoc', 'error', '');} else {parent.ajaxDialogModal('ErrorProjectRemoveDoc', 'error', '');}", true);
                            else
                            {
                                search = true;
                                if (this.ListCheck.ContainsKey(idProfile))
                                {
                                    this.ListCheck.Remove(idProfile);
                                }
                            }
                        break;
                    }
                case "estensionedoc":
                    {
                        UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                        FileManager.setSelectedFile(schedaDocumento.documenti[0]);
                        HttpContext.Current.Session["isZoom"] = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDocumentViewer", "ajaxModalPopupDocumentViewer();", true);
                        NttDataWA.Popup.DocumentViewer.OpenDocumentViewer = true;
                        break;
                    }
                case "visualizzaProcessiFirmaAvviati":
                    {
                        UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                        FileManager.setSelectedFile(schedaDocumento.documenti[0]);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupInfoSignatureProcessesStarted", "ajaxModalPopupInfoSignatureProcessesStarted();", true);
                        break;
                    }
                case "visualizzaStatoProcessiFirmaAvviati":
                    {
                        UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                        FileManager.setSelectedFile(schedaDocumento.documenti[0]);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDetailsLFAutomaticMode", "ajaxModalPopupDetailsLFAutomaticMode();", true);
                        break;
                    }
                case "newDocumentAnswer":
                    {
                        CreaPredispostoInRisposta(schedaDocumento);

                        HttpContext.Current.Session["isZoom"] = null;
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject obj = navigationList.Last();
                        Navigation.NavigationObject newObj = Navigation.NavigationUtils.CloneObject(obj);

                        newObj.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), string.Empty);
                        newObj.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), true, this.Page);
                        newObj.CodePage = Navigation.NavigationUtils.NamePage.PROJECT.ToString();
                        newObj.Page = "PROJECT.ASPX";
                        newObj.NumPage = this.SelectedPage.ToString();
                        newObj.PageSize = this.PageSize.ToString();
                        newObj.DxTotalPageNumber = this.PagesCount.ToString();
                        newObj.DxTotalNumberElement = this.RecordCount.ToString();
                        newObj.SearchFilters = this.SearchFilters;
                        newObj.SearchFiltersOrder = UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid);
                        newObj.ViewResult = true;
                        newObj.folder = fascicolo.folderSelezionato;
                        newObj.idProject = fascicolo.systemID;
                        int indexElement = ((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1);
                        newObj.DxPositionElement = indexElement.ToString();

                        if (obj.NamePage.Equals(Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), string.Empty)) && !string.IsNullOrEmpty(obj.IdObject) && obj.IdObject.Equals(newObj.idProject))
                        {
                            navigationList.Remove(obj);
                        }
                        navigationList.Add(newObj);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);
                        this.ListObjectNavigation = this.Result;
                        Response.Redirect("~/Document/Document.aspx");
                        break;
                    }
            }

            if (search)
            {
                this.SearchDocumentsAndDisplayResult(null, gridViewResult.SelectedIndex, GridManager.SelectedGrid, Label, fascicolo.folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), search);
                this.aggiornaPanel();
            }
            else
            {
                this.aggiornaPanel();
                this.UpnlGrid.Update();
            }
        }

        protected void BuildGridNavigator()
        {
            try
            {
                this.plcNavigator.Controls.Clear();

                int countPage = this.PagesCount;

                int val = this.RecordCount % this.PageSize;
                if (val == 0)
                {
                    countPage = countPage - 1;
                }

                if (countPage > 1)
                {
                    Panel panel = new Panel();
                    panel.EnableViewState = true;
                    panel.CssClass = "recordNavigator";

                    int startFrom = 1;
                    if (this.SelectedPage > 6) startFrom = this.SelectedPage - 5;

                    int endTo = 10;
                    if (this.SelectedPage > 6) endTo = this.SelectedPage + 5;
                    if (endTo > countPage) endTo = countPage;

                    if (startFrom > 1)
                    {
                        LinkButton btn = new LinkButton();
                        btn.EnableViewState = true;
                        btn.Text = "...";
                        btn.Attributes["onclick"] = " $('#grid_pageindex').val(" + (startFrom - 1) + "); __doPostBack('upPnlGridIndexes', ''); return false;";
                        panel.Controls.Add(btn);
                    }

                    for (int i = startFrom; i <= endTo; i++)
                    {
                        if (i == this.SelectedPage)
                        {
                            Literal lit = new Literal();
                            lit.Text = "<span>" + i.ToString() + "</span>";
                            panel.Controls.Add(lit);
                        }
                        else
                        {
                            LinkButton btn = new LinkButton();
                            btn.EnableViewState = true;
                            btn.Text = i.ToString();
                            btn.Attributes["onclick"] = " $('#grid_pageindex').val($(this).text()); __doPostBack('upPnlGridIndexes', ''); return false;";
                            panel.Controls.Add(btn);
                        }
                    }

                    if (endTo < countPage)
                    {
                        LinkButton btn = new LinkButton();
                        btn.EnableViewState = true;
                        btn.Text = "...";
                        btn.Attributes["onclick"] = " $('#grid_pageindex').val(" + endTo + "); __doPostBack('upPnlGridIndexes', ''); return false;";
                        panel.Controls.Add(btn);
                    }

                    this.plcNavigator.Controls.Add(panel);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        //private bool ContinuePage()
        //{
        //    bool result = true;
        //    int val = this.RecordCount % this.PageSize;
        //    if (this.RecordCount % this.PageSize)
        //    {

        //    }
        //    return result;
        //}


        #endregion

        #region tipologia fascicoli
        protected void ProjectDdlTypeDocument_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            //DocsPaWR.TipologiaAtto typeDocument = new DocsPaWR.TipologiaAtto();
            //typeDocument.systemId = this.DocumentDdlTypeDocument.SelectedItem.Value;
            //typeDocument.descrizione = this.DocumentDdlTypeDocument.SelectedItem.Text;

            //this.DocumentInWorking.daAggiornareTipoAtto = true;
            //this.DocumentInWorking.tipologiaAtto = typeDocument;

            if (!string.IsNullOrEmpty(this.projectDdlTipologiafascicolo.SelectedValue))
            {
                this.ProjectImgHistoryTipology.Visible = true;
                if (this.CustomProject)
                {

                    this.TemplateProject = ProfilerProjectManager.getTemplateFascById(this.projectDdlTipologiafascicolo.SelectedItem.Value);
                    if (this.TemplateProject != null)
                    {
                        if (this.TemplateProject.PRIVATO != null && this.TemplateProject.PRIVATO == "1")
                        {
                            this.ProjectCheckPrivate.Checked = true;
                            this.UpProjectPrivate.Update();
                        }

                        //if (this.EnableStateDiagram)
                        //{
                        //    this.DocumentDdlStateDiagram.ClearSelection();
                        //    //Verifico se esiste un diagramma di stato associato al tipo di documento
                        //    this.StateDiagram = DiagrammiManager.getDgByIdTipoFasc(this.projectDdlTipologiafascicolo.SelectedItem.Value, UserManager.GetInfoUser().idAmministrazione);
                        //    if (this.StateDiagram != null)
                        //    {
                        //        this.PnlStateDiagram.Visible = true;
                        //        this.popolaComboBoxStatiSuccessivi(null, this.StateDiagram);
                        //    }
                        //    else
                        //    {
                        //        this.PnlStateDiagram.Visible = false;
                        //    }
                        //}
                        
                        Fascicolo fasc = ProjectManager.getProjectInSession();
                        string dtaScadenza = string.Empty;
                        if (this.EnableStateDiagram)
                        {
                            this.DocumentDdlStateDiagram.ClearSelection();
                            //Verifico se esiste un diagramma di stato associato al tipo di documento
                            this.StateDiagram = DiagrammiManager.getDgByIdTipoFasc(this.projectDdlTipologiafascicolo.SelectedItem.Value, UIManager.UserManager.GetInfoUser().idAmministrazione);
                            if (this.StateDiagram != null)
                            {
                                this.PnlStateDiagram.Visible = true;
                                this.popolaComboBoxStatiSuccessivi(null, this.StateDiagram);

                                if (this.TemplateProject != null && !string.IsNullOrEmpty(this.TemplateProject.SCADENZA) && this.TemplateProject.SCADENZA != "0")
                                {
                                    if(fasc!=null)
                                    {
                                        dtaScadenza = fasc.dtaScadenza;
                                    }
                                    this.PnlDocumentStateDiagramDate.Visible = true;
                                    this.PnlScadenza.Visible = true;
                                    this.DocumentStateDiagramDataValue.Text = dtaScadenza;

                                    DateTime dataOdierna = System.DateTime.Now;
                                    int scadenza = Convert.ToInt32(this.TemplateProject.SCADENZA);
                                    DateTime dataCalcolata = dataOdierna.AddDays(scadenza);
                                    this.PnlScadenza.Visible = true;
                                    if (string.IsNullOrEmpty(dtaScadenza))
                                    {
                                        this.DocumentStateDiagramDataValue.Text = utils.formatDataDocsPa(dataCalcolata);

                                        //this.DocumentInWorking.dataScadenza = Utils.formatDataDocsPa(dataCalcolata);
                                    }
                                    else
                                    {
                                        this.DocumentStateDiagramDataValue.Text = dtaScadenza.Substring(0, 10);
                                        this.DocumentStateDiagramDataValue.ReadOnly = true;
                                    }

                                }
                                else
                                {
                                    this.PnlDocumentStateDiagramDate.Visible = false;
                                    this.PnlScadenza.Visible = false;
                                    this.DocumentStateDiagramDataValue.Text = string.Empty;
                                }
                            }
                            else
                            {
                                this.PnlStateDiagram.Visible = false;
                            }
                        }
                    }
                }
            }
            else
            {
                this.TemplateProject = null;
                this.ProjectImgHistoryTipology.Visible = false;
                if (this.EnableStateDiagram)
                {
                    this.DocumentDdlStateDiagram.ClearSelection();
                    this.PnlStateDiagram.Visible = false;
                }
            }

            this.UpPnlTypeDocument.Update();
        }

        private void popolaComboBoxStatiSuccessivi(DocsPaWR.Stato stato, DocsPaWR.DiagrammaStato diagramma)
        {
            List<string> idStatiSuccessivi = new List<string>();
            int selectedIndex = this.DocumentDdlStateDiagram.SelectedIndex;
            //Inizializzazione
            this.DocumentDdlStateDiagram.Items.Clear();
            ListItem itemEmpty = new ListItem();
            this.DocumentDdlStateDiagram.Items.Add(itemEmpty);

            //Popola la combo con gli stati iniziali del diagramma
            if (stato == null)
            {
                this.LitActualStateDiagram.Text = string.Empty;
                for (int i = 0; i < diagramma.STATI.Length; i++)
                {
                    DocsPaWR.Stato st = (DocsPaWR.Stato)diagramma.STATI[i];

                    if (st.STATO_INIZIALE && DiagrammiManager.IsRuoloAssociatoStatoDia(diagramma.SYSTEM_ID.ToString(), UIManager.RoleManager.GetRoleInSession().idGruppo, st.SYSTEM_ID.ToString()))
                    {
                        ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                        this.DocumentDdlStateDiagram.Items.Add(item);
                        if (st.STATO_SISTEMA)
                            item.Enabled = false;
                    }
                }
                if (this.DocumentDdlStateDiagram.Items.Count == 2)
                    selectedIndex = 1;
            }
            //Popola la combo con i possibili stati, successivi a quello passato
            else
            {
                for (int i = 0; i < diagramma.PASSI.Length; i++)
                {
                    DocsPaWR.Passo step = (DocsPaWR.Passo)diagramma.PASSI[i];
                    if (step.STATO_PADRE.SYSTEM_ID == stato.SYSTEM_ID)
                    {
                        for (int j = 0; j < step.SUCCESSIVI.Length; j++)
                        {
                            DocsPaWR.Stato st = (DocsPaWR.Stato)step.SUCCESSIVI[j];
                            if (DiagrammiManager.IsRuoloAssociatoStatoDia(diagramma.SYSTEM_ID.ToString(), UIManager.RoleManager.GetRoleInSession().idGruppo, st.SYSTEM_ID.ToString()))
                            {
                                ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                                if (st.STATO_SISTEMA)
                                    item.Enabled = false;
                                this.DocumentDdlStateDiagram.Items.Add(item);
                                idStatiSuccessivi.Add(st.SYSTEM_ID.ToString());
                            }
                        }
                    }
                }
            }

            if (selectedIndex < this.DocumentDdlStateDiagram.Items.Count)
                this.DocumentDdlStateDiagram.SelectedIndex = selectedIndex;

            //Popolo le fasi del diagramma
            if (stato != null)
            {
                List<AssPhaseStatoDiagramma> phasesState = UIManager.DiagrammiManager.GetFasiStatiDiagramma(diagramma.SYSTEM_ID.ToString());
                if (phasesState != null && phasesState.Count > 0)
                {
                    //HtmlGenericControl div = new HtmlGenericControl("containerProject");
                    if (containerProject.Style["top"] == null || !containerProject.Style["top"].Equals("235px"))
                    {
                        containerProject.Attributes["style"] = "top: 235px";
                        this.UpContainer.Update();
                    }
                    this.HeaderProject.BuildFasiDiagramma(phasesState, stato.SYSTEM_ID.ToString(), idStatiSuccessivi);
                }
            }
        }

        private void PopulateProfiledDocument()
        {
            this.PnlTypeDocument.Controls.Clear();
            this.inserisciComponenti(true);
        }
        private void inserisciComponenti(bool readOnly)
        {
            this.RemovePropertySearchCorrespondentIntExtWithDisabled();
            if (this.TemplateProject.OLD_OGG_CUSTOM.Length < 1)
            {
                this.TemplateProject.OLD_OGG_CUSTOM = new StoricoProfilatiOldValue[this.TemplateProject.ELENCO_OGGETTI.Length];
            }

            ArrayList dirittiCampiRuolo = ProfilerProjectManager.getDirittiCampiTipologiaFasc(UIManager.RoleManager.GetRoleInSession().idGruppo, this.TemplateProject.SYSTEM_ID.ToString());

            for (int i = 0, index = 0; i < this.TemplateProject.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)this.TemplateProject.ELENCO_OGGETTI[i];

                ProfilerProjectManager.addNoRightsCustomObject(dirittiCampiRuolo, oggettoCustom);

                switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                {
                    case "CampoDiTesto":
                        this.inserisciCampoDiTesto(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "CasellaDiSelezione":
                        this.inserisciCasellaDiSelezione(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "MenuATendina":
                        this.inserisciMenuATendina(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "SelezioneEsclusiva":
                        this.inserisciSelezioneEsclusiva(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Contatore":
                        this.inserisciContatore(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "Data":
                        this.inserisciData(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Corrispondente":
                        this.inserisciCorrispondente(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Link":
                        this.inserisciLink(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "ContatoreSottocontatore":
                        this.inserisciContatoreSottocontatore(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "OggettoEsterno":
                        this.inserisciOggettoEsterno(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "Separatore":
                        this.inserisciCampoSeparatore(oggettoCustom);
                        break;
                }

            }

            //controlla che vi sia almeno un campo visibile per attivare il pulsante per lo storico
            int btn_HistoryIsVisible = 0;
            foreach (AssDocFascRuoli diritti in dirittiCampiRuolo)
            {
                if (!diritti.VIS_OGG_CUSTOM.Equals("0"))
                    ++btn_HistoryIsVisible;
            }
        }

        public void inserisciOggettoEsterno(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            Label etichetta = new Label();
            etichetta.EnableViewState = true;

            if ("SI".Equals(oggettoCustom.CAMPO_OBBLIGATORIO))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }
            etichetta.CssClass = "weight";
            UserControls.IntegrationAdapter intAd = (UserControls.IntegrationAdapter)this.LoadControl("../UserControls/IntegrationAdapter.ascx");
            intAd.ID = oggettoCustom.SYSTEM_ID.ToString();
            intAd.View = UserControls.IntegrationAdapterView.INSERT_MODIFY;
            intAd.ManualInsertCssClass = "txt_textdata_counter_disabled_red";
            intAd.EnableViewState = true;
            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, intAd, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);
            //if ((this.DocumentInWorking != null && !string.IsNullOrEmpty(this.DocumentInWorking.systemId) && !string.IsNullOrEmpty(this.DocumentInWorking.accessRights)) && ((this.DocumentInWorking.accessRights == "45" || readOnly)))
            //    intAd.View = UserControls.IntegrationAdapterView.READ_ONLY;
            intAd.ConfigurationValue = oggettoCustom.CONFIG_OBJ_EST;
            IntegrationAdapterValue value = new IntegrationAdapterValue(oggettoCustom.CODICE_DB, oggettoCustom.VALORE_DATABASE, oggettoCustom.MANUAL_INSERT);
            intAd.Value = value;

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;

            if (etichetta.Visible)
            {
                HtmlGenericControl parDesc = new HtmlGenericControl("p");
                parDesc.Controls.Add(etichetta);
                parDesc.EnableViewState = true;
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (intAd.Visible)
            {
                divColValue.Controls.Add(intAd);
                divRowValue.Controls.Add(divColValue);
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }

        }

        private void inserisciCampoDiTesto(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            DocsPaWR.StoricoProfilatiOldValue oldObjText = new StoricoProfilatiOldValue();

            Label etichettaCampoDiTesto = new Label();
            etichettaCampoDiTesto.EnableViewState = true;

            CustomTextArea txt_CampoDiTesto = new CustomTextArea();
            txt_CampoDiTesto.EnableViewState = true;

            if (oggettoCustom.MULTILINEA.Equals("SI"))
            {
                if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE + " *";
                }
                else
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;
                }
                etichettaCampoDiTesto.CssClass = "weight";

                txt_CampoDiTesto.CssClass = "txt_textarea";
                txt_CampoDiTesto.CssClassReadOnly = "txt_textarea_disabled";

                if (string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_LINEE))
                {
                    txt_CampoDiTesto.Height = 55;
                }
                else
                {
                    txt_CampoDiTesto.Rows = Convert.ToInt32(oggettoCustom.NUMERO_DI_LINEE);
                }

                if (string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_CARATTERI))
                {
                    txt_CampoDiTesto.MaxLength = 150;
                }
                else
                {
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }

                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;
                txt_CampoDiTesto.TextMode = TextBoxMode.MultiLine;


            }
            else
            {
                if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE + " *";
                }
                else
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;
                }
                etichettaCampoDiTesto.CssClass = "weight";

                if (!string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_CARATTERI))
                {
                    //ATTENZIONE : La lunghezza della textBox non è speculare al numero massimo di
                    //caratteri che l'utente inserisce.
                    if (((Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6) <= 400))
                    {
                        txt_CampoDiTesto.Width = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6;
                    }
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }
                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;
                txt_CampoDiTesto.CssClass = "txt_input_full";
                txt_CampoDiTesto.CssClassReadOnly = "txt_input_full_disabled";
                txt_CampoDiTesto.TextMode = TextBoxMode.SingleLine;


            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;


            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaCampoDiTesto, txt_CampoDiTesto, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && (ProjectManager.getProjectInSession().accessRights == "45" || !this.EnableEdit || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C"))))
            {
                txt_CampoDiTesto.ReadOnly = true;
            }


            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
            {
                //txt_CampoDiTesto.Attributes.Add("onClick", "window.document.getElementById('" + txt_CampoDiTesto.ClientID + "').focus();");
                if (this.TemplateProject.OLD_OGG_CUSTOM[index] == null)//se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
                {
                    //blocco storico profilazione campi di testo 
                    //salvo il valore corrente del campo di testo in oldObjCustom.
                    oldObjText.IDTemplate = this.TemplateProject.ID_TIPO_FASC;
                    oldObjText.ID_Doc_Fasc = ProjectManager.getProjectInSession().systemID;
                    oldObjText.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                    oldObjText.Valore = oggettoCustom.VALORE_DATABASE;
                    oldObjText.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                    InfoUtente user = UserManager.GetInfoUser();
                    oldObjText.ID_People = user.idPeople;
                    oldObjText.ID_Ruolo_In_UO = user.idCorrGlobali;
                    this.TemplateProject.OLD_OGG_CUSTOM[index] = oldObjText;
                }
            }

            if (etichettaCampoDiTesto.Visible)
            {
                HtmlGenericControl parDesc = new HtmlGenericControl("p");
                parDesc.Controls.Add(etichettaCampoDiTesto);
                parDesc.EnableViewState = true;
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (txt_CampoDiTesto.Visible)
            {
                divColValue.Controls.Add(txt_CampoDiTesto);
                divRowValue.Controls.Add(divColValue);
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }


        }

        private void inserisciContatoreSottocontatore(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, ArrayList dirittiCampiRuolo)
        {
            bool paneldll = false;

            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                return;

            Label etichettaContatoreSottocontatore = new Label();
            etichettaContatoreSottocontatore.EnableViewState = true;
            etichettaContatoreSottocontatore.Text = oggettoCustom.DESCRIZIONE;
            etichettaContatoreSottocontatore.CssClass = "weight";

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaContatoreSottocontatore);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);



            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            Label etichettaDDL = new Label();
            etichettaDDL.EnableViewState = true;
            DropDownList ddl = new DropDownList();
            ddl.EnableViewState = true;

            string language = UIManager.UserManager.GetUserLanguage();
            ddl.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));

            Panel divRowDll = new Panel();
            divRowDll.CssClass = "row";
            divRowDll.EnableViewState = true;

            Panel divRowEtiDll = new Panel();
            divRowEtiDll.CssClass = "row";
            divRowEtiDll.EnableViewState = true;

            HtmlGenericControl parDll = new HtmlGenericControl("p");
            parDll.EnableViewState = true;

            if (string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                Ruolo ruoloUtente = RoleManager.GetRoleInSession();
                Registro[] registriRfVisibili = UIManager.RegistryManager.GetListRegistriesAndRF(ruoloUtente.systemId, string.Empty, string.Empty);

                Panel divColDllEti = new Panel();
                divColDllEti.CssClass = "col";
                divColDllEti.EnableViewState = true;

                Panel divColDll = new Panel();
                divColDll.CssClass = "col";
                divColDll.EnableViewState = true;

                switch (oggettoCustom.TIPO_CONTATORE)
                {
                    case "T":
                        break;
                    case "A":
                        paneldll = true;
                        etichettaDDL.Text = "&nbsp;AOO&nbsp;";
                        etichettaDDL.CssClass = "weight";
                        etichettaDDL.Width = 50;
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "chzn-select-deselect";
                        ddl.Width = 240;

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "0" && !((Registro)registriRfVisibili[i]).Sospeso)
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Text = string.Empty;
                            it.Value = string.Empty;
                            ddl.Items.Add(it);
                            ddl.SelectedValue = string.Empty;
                        }
                        else
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                        if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
                        {
                            ddl.Enabled = false;
                        }

                        parDll.Controls.Add(etichettaDDL);
                        divColDllEti.Controls.Add(parDll);
                        divRowEtiDll.Controls.Add(divColDllEti);

                        divColDll.Controls.Add(ddl);
                        divRowDll.Controls.Add(divColDll);
                        break;
                    case "R":
                        paneldll = true;
                        etichettaDDL.Text = "&nbsp;RF&nbsp;";
                        etichettaDDL.CssClass = "weight";
                        etichettaDDL.Width = 50;
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "chzn-select-deselect";
                        ddl.Width = 240;

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "1" && ((Registro)registriRfVisibili[i]).rfDisabled == "0")
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Value = string.Empty;
                            it.Text = string.Empty;
                            ddl.Items.Add(it);
                            ddl.SelectedValue = string.Empty;
                        }
                        else
                        {
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                        }
                        if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
                        {
                            ddl.Enabled = false;
                        }

                        parDll.Controls.Add(etichettaDDL);
                        divColDllEti.Controls.Add(parDll);
                        divRowEtiDll.Controls.Add(divColDllEti);

                        divColDll.Controls.Add(ddl);
                        divRowDll.Controls.Add(divColDll);
                        break;
                }
            }

            //Imposto il contatore in funzione del formato
            CustomTextArea contatore = new CustomTextArea();
            CustomTextArea sottocontatore = new CustomTextArea();
            CustomTextArea dataInserimentoSottocontatore = new CustomTextArea();
            contatore.EnableViewState = true;
            sottocontatore.EnableViewState = true;
            dataInserimentoSottocontatore.EnableViewState = true;
            //contatore.Attributes.Add("onClick", "window.document.getElementById('" + contatore.ClientID + "').focus();");
            //sottocontatore.Attributes.Add("onClick", "window.document.getElementById('" + sottocontatore.ClientID + "').focus();");
            //dataInserimentoSottocontatore.Attributes.Add("onClick", "window.document.getElementById('" + dataInserimentoSottocontatore.ClientID + "').focus();");
            contatore.ID = oggettoCustom.SYSTEM_ID.ToString();
            sottocontatore.ID = oggettoCustom.SYSTEM_ID.ToString() + "_sottocontatore";
            dataInserimentoSottocontatore.ID = oggettoCustom.SYSTEM_ID.ToString() + "_dataSottocontatore";
            if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
            {
                contatore.Text = oggettoCustom.FORMATO_CONTATORE;
                sottocontatore.Text = oggettoCustom.FORMATO_CONTATORE;
                //if (Session["templateRiproposto"] != null)
                //{
                //    contatore.Text = contatore.Text.Replace("ANNO", "");
                //    contatore.Text = contatore.Text.Replace("CONTATORE", "");
                //    contatore.Text = contatore.Text.Replace("RF", "");
                //    contatore.Text = contatore.Text.Replace("AOO", "");

                //    sottocontatore.Text = sottocontatore.Text.Replace("ANNO", "");
                //    sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", "");
                //    sottocontatore.Text = sottocontatore.Text.Replace("RF", "");
                //    sottocontatore.Text = sottocontatore.Text.Replace("AOO", "");
                //}
                //else
                //{
                if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE) && !string.IsNullOrEmpty(oggettoCustom.VALORE_SOTTOCONTATORE))
                {
                    contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO);
                    contatore.Text = contatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);

                    sottocontatore.Text = sottocontatore.Text.Replace("ANNO", oggettoCustom.ANNO);
                    sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_SOTTOCONTATORE);

                    if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && oggettoCustom.ID_AOO_RF != "0")
                    {
                        Registro reg = UserManager.getRegistroBySistemId(this, oggettoCustom.ID_AOO_RF);
                        if (reg != null)
                        {
                            contatore.Text = contatore.Text.Replace("RF", reg.codRegistro);
                            contatore.Text = contatore.Text.Replace("AOO", reg.codRegistro);

                            sottocontatore.Text = sottocontatore.Text.Replace("RF", reg.codRegistro);
                            sottocontatore.Text = sottocontatore.Text.Replace("AOO", reg.codRegistro);
                        }
                    }
                }
                else
                {
                    contatore.Text = contatore.Text.Replace("ANNO", "");
                    contatore.Text = contatore.Text.Replace("CONTATORE", "");
                    contatore.Text = contatore.Text.Replace("RF", "");
                    contatore.Text = contatore.Text.Replace("AOO", "");

                    sottocontatore.Text = sottocontatore.Text.Replace("ANNO", "");
                    sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", "");
                    sottocontatore.Text = sottocontatore.Text.Replace("RF", "");
                    sottocontatore.Text = sottocontatore.Text.Replace("AOO", "");
                }
                //}
            }
            else
            {
                contatore.Text = oggettoCustom.VALORE_DATABASE;
                sottocontatore.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            }

            Panel divRowCounter = new Panel();
            divRowCounter.CssClass = "row";
            divRowCounter.EnableViewState = true;

            Panel divColCountCounter = new Panel();
            divColCountCounter.CssClass = "col_full";
            divColCountCounter.EnableViewState = true;
            divColCountCounter.Controls.Add(contatore);
            divColCountCounter.Controls.Add(sottocontatore);
            divRowCounter.Controls.Add(divColCountCounter);

            if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
            {
                dataInserimentoSottocontatore.Text = oggettoCustom.DATA_INSERIMENTO;

                Panel divColCountAbort = new Panel();
                divColCountAbort.CssClass = "col";
                divColCountAbort.EnableViewState = true;
                divColCountAbort.Controls.Add(dataInserimentoSottocontatore);
                divRowCounter.Controls.Add(divColCountAbort);
            }

            CheckBox cbContaDopo = new CheckBox();
            cbContaDopo.EnableViewState = true;

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloContatoreSottocontatore(etichettaContatoreSottocontatore, contatore, sottocontatore, dataInserimentoSottocontatore, cbContaDopo, etichettaDDL, ddl, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);

            if (etichettaContatoreSottocontatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            contatore.ReadOnly = true;
            contatore.CssClass = "txt_input_half";
            contatore.CssClassReadOnly = "txt_input_half_disabled";

            sottocontatore.ReadOnly = true;
            sottocontatore.CssClass = "txt_input_half";
            sottocontatore.CssClassReadOnly = "txt_input_half_disabled";

            dataInserimentoSottocontatore.ReadOnly = true;
            dataInserimentoSottocontatore.CssClass = "txt_input_full";
            dataInserimentoSottocontatore.CssClassReadOnly = "txt_input_full_disabled";
            dataInserimentoSottocontatore.Visible = false;


            //Inserisco il cb per il conta dopo
            if (oggettoCustom.CONTA_DOPO == "1")
            {
                cbContaDopo.ID = oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo";
                cbContaDopo.Checked = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                cbContaDopo.ToolTip = "Attiva / Disattiva incremento del contatore al salvataggio dei dati.";


                if ((!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE)) || ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
                {
                    cbContaDopo.Checked = false;
                    cbContaDopo.Visible = false;
                    cbContaDopo.Enabled = false;
                }

                Panel divColCountAfter = new Panel();
                divColCountAfter.CssClass = "col";
                divColCountAfter.EnableViewState = true;
                divColCountAfter.Controls.Add(cbContaDopo);
                divRowDll.Controls.Add(divColCountAfter);
            }

            if (paneldll)
            {
                this.PnlTypeDocument.Controls.Add(divRowEtiDll);
                this.PnlTypeDocument.Controls.Add(divRowDll);
            }

            if (contatore.Visible || sottocontatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowCounter);
            }
        }

        private void inserisciCasellaDiSelezione(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            DocsPaWR.StoricoProfilatiOldValue casellaSelOldObj = new StoricoProfilatiOldValue();
            Label etichettaCasellaSelezione = new Label();
            etichettaCasellaSelezione.EnableViewState = true;

            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE;
            }

            etichettaCasellaSelezione.Width = Unit.Percentage(100);
            etichettaCasellaSelezione.CssClass = "weight";

            CheckBoxList casellaSelezione = new CheckBoxList();
            casellaSelezione.EnableViewState = true;
            casellaSelezione.ID = oggettoCustom.SYSTEM_ID.ToString();
            int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreElenco = ((ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                if (i < oggettoCustom.VALORI_SELEZIONATI.Length)
                {
                    string valoreSelezionato = (string)(oggettoCustom.VALORI_SELEZIONATI[i]);
                    if (valoreElenco.ABILITATO == 1 || (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato)))
                    {
                        //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                        if (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato))
                            valoreElenco.ABILITATO = 1;

                        casellaSelezione.Items.Add(new ListItem(valoreElenco.VALORE, valoreElenco.VALORE));
                        //Valore di default
                        if (valoreElenco.VALORE_DI_DEFAULT.Equals("SI"))
                        {
                            valoreDiDefault = i;
                        }
                    }
                }
            }

            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Vertical;
            }
            if (valoreDiDefault != -1)
            {
                casellaSelezione.SelectedIndex = valoreDiDefault;
            }

            if (oggettoCustom.VALORI_SELEZIONATI != null)
            {
                this.impostaSelezioneCaselleDiSelezione(oggettoCustom, casellaSelezione);
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaCasellaSelezione);
            parDesc.EnableViewState = true;



            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColDesc.EnableViewState = true;



            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaCasellaSelezione, casellaSelezione, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
            {
                casellaSelezione.Enabled = false;
            }

            if (etichettaCasellaSelezione.Visible)
            {
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (casellaSelezione.Visible)
            {

                divColValue.Controls.Add(casellaSelezione);
                divRowValue.Controls.Add(divColValue);

                this.PnlTypeDocument.Controls.Add(divRowValue);
            }

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
            {
                if (this.TemplateProject.OLD_OGG_CUSTOM[index] == null) //se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
                {
                    //blocco storico profilazione casella di selezione 
                    casellaSelOldObj.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                    casellaSelOldObj.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                    //per questo oggetto faccio un merge dei valori selezionati e salvo la stringa nel db
                    for (int x = 0; x < oggettoCustom.VALORI_SELEZIONATI.Length; x++)
                    {
                        if (!string.IsNullOrEmpty(oggettoCustom.VALORI_SELEZIONATI[x]))
                            casellaSelOldObj.Valore += string.IsNullOrEmpty(casellaSelOldObj.Valore) ?
                                oggettoCustom.VALORI_SELEZIONATI[x] : "*#?" + oggettoCustom.VALORI_SELEZIONATI[x];
                    }
                    InfoUtente user = UserManager.GetInfoUser();
                    casellaSelOldObj.IDTemplate = this.TemplateProject.ID_TIPO_FASC;
                    casellaSelOldObj.ID_Doc_Fasc = ProjectManager.getProjectInSession().systemID;
                    casellaSelOldObj.ID_People = user.idPeople;
                    casellaSelOldObj.ID_Ruolo_In_UO = user.idCorrGlobali;
                    this.TemplateProject.OLD_OGG_CUSTOM[index] = casellaSelOldObj;
                }
            }
        }

        private void inserisciMenuATendina(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            DocsPaWR.StoricoProfilatiOldValue menuOldObj = new StoricoProfilatiOldValue();
            Label etichettaMenuATendina = new Label();
            etichettaMenuATendina.EnableViewState = true;
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE;
            }
            etichettaMenuATendina.CssClass = "weight";

            int maxLenght = 0;
            DropDownList menuATendina = new DropDownList();
            menuATendina.EnableViewState = true;
            menuATendina.ID = oggettoCustom.SYSTEM_ID.ToString();
            int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                {
                    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                        valoreOggetto.ABILITATO = 1;

                    menuATendina.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                    //Valore di default
                    if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    {
                        valoreDiDefault = i;
                    }

                    if (maxLenght < valoreOggetto.VALORE.Length)
                    {
                        maxLenght = valoreOggetto.VALORE.Length;
                    }
                }
            }
            menuATendina.CssClass = "chzn-select-deselect";
            string language = UIManager.UserManager.GetUserLanguage();
            menuATendina.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));
            menuATendina.Width = maxLenght + 250;

            if (valoreDiDefault != -1)
            {
                menuATendina.SelectedIndex = valoreDiDefault;
            }
            if (!(valoreDiDefault != -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
            {
                menuATendina.Items.Insert(0, "");
            }
            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                menuATendina.SelectedIndex = this.impostaSelezioneMenuATendina(oggettoCustom.VALORE_DATABASE, menuATendina);
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaMenuATendina);
            parDesc.EnableViewState = true;



            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;




            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaMenuATendina, menuATendina, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);

            if (etichettaMenuATendina.Visible)
            {
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }


            if (menuATendina.Visible)
            {
                divColValue.Controls.Add(menuATendina);
                divRowValue.Controls.Add(divColValue);
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }




            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
            {
                menuATendina.Enabled = false;
            }

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
            {
                if (this.TemplateProject.OLD_OGG_CUSTOM[index] == null)//se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
                {
                    //blocco storico profilazione del campo menu di selezione 
                    //salvo il valore corrente del campo menu di selezione in oldObjCustom.
                    menuOldObj.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                    menuOldObj.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                    menuOldObj.Valore = oggettoCustom.VALORE_DATABASE;
                    InfoUtente user = UserManager.GetInfoUser();
                    menuOldObj.ID_People = user.idPeople;
                    menuOldObj.ID_Ruolo_In_UO = user.idCorrGlobali;
                    menuOldObj.IDTemplate = this.TemplateProject.ID_TIPO_FASC;
                    Fascicolo fasc = ProjectManager.getProjectInSession();
                    menuOldObj.ID_Doc_Fasc = fasc.systemID;
                    this.TemplateProject.OLD_OGG_CUSTOM[index] = menuOldObj;
                }
            }
        }

        private void inserisciSelezioneEsclusiva(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            DocsPaWR.StoricoProfilatiOldValue selezEsclOldObj = new StoricoProfilatiOldValue();
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            Label etichettaSelezioneEsclusiva = new Label();
            etichettaSelezioneEsclusiva.EnableViewState = true;
            CustomImageButton cancella_selezioneEsclusiva = new CustomImageButton();
            string language = UIManager.UserManager.GetUserLanguage();
            cancella_selezioneEsclusiva.AlternateText = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Reset", language);
            cancella_selezioneEsclusiva.ToolTip = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Reset", language);
            cancella_selezioneEsclusiva.EnableViewState = true;

            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE;
            }

            cancella_selezioneEsclusiva.ID = "_" + oggettoCustom.SYSTEM_ID.ToString();
            cancella_selezioneEsclusiva.ImageUrl = "../Images/Icons/clean_field_custom.png";
            cancella_selezioneEsclusiva.OnMouseOutImage = "../Images/Icons/clean_field_custom.png";
            cancella_selezioneEsclusiva.OnMouseOverImage = "../Images/Icons/clean_field_custom_hover.png";
            cancella_selezioneEsclusiva.ImageUrlDisabled = "../Images/Icons/clean_field_custom_disabled.png";
            cancella_selezioneEsclusiva.CssClass = "clickable";
            cancella_selezioneEsclusiva.Click += cancella_selezioneEsclusiva_Click;
            etichettaSelezioneEsclusiva.CssClass = "weight";

            RadioButtonList selezioneEsclusiva = new RadioButtonList();
            selezioneEsclusiva.EnableViewState = true;
            selezioneEsclusiva.ID = oggettoCustom.SYSTEM_ID.ToString();
            int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                {
                    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                        valoreOggetto.ABILITATO = 1;

                    selezioneEsclusiva.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                    //Valore di default
                    if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    {
                        valoreDiDefault = i;
                    }
                }
            }

            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Vertical;
            }
            if (valoreDiDefault != -1)
            {
                selezioneEsclusiva.SelectedIndex = valoreDiDefault;
            }
            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                selezioneEsclusiva.SelectedIndex = impostaSelezioneEsclusiva(oggettoCustom.VALORE_DATABASE, selezioneEsclusiva);
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            Panel divColImage = new Panel();
            divColImage.CssClass = "col-right-no-margin";
            divColImage.EnableViewState = true;

            divColImage.Controls.Add(cancella_selezioneEsclusiva);

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaSelezioneEsclusiva);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);

            divRowDesc.Controls.Add(divColDesc);
            divRowDesc.Controls.Add(divColImage);


            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;

            divColValue.Controls.Add(selezioneEsclusiva);
            divRowValue.Controls.Add(divColValue);



            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSelezioneEsclusiva(etichettaSelezioneEsclusiva, selezioneEsclusiva, cancella_selezioneEsclusiva, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);

            if (etichettaSelezioneEsclusiva.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (selezioneEsclusiva.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
            {
                selezioneEsclusiva.Enabled = false;
                cancella_selezioneEsclusiva.Enabled = false;
            }

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
            {
                if (this.TemplateProject.OLD_OGG_CUSTOM[index] == null) //se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
                {
                    //blocco storico profilazione del campo di selezione esclusiva 
                    //salvo il valore corrente del campo di selezione esclusiva in oldObjCustom.
                    selezEsclOldObj.IDTemplate = this.TemplateProject.ID_TIPO_FASC;
                    Fascicolo fasc = ProjectManager.getProjectInSession();
                    selezEsclOldObj.ID_Doc_Fasc = fasc.systemID;
                    selezEsclOldObj.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                    selezEsclOldObj.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                    InfoUtente user = UserManager.GetInfoUser();
                    selezEsclOldObj.ID_People = user.idPeople;
                    selezEsclOldObj.ID_Ruolo_In_UO = user.idCorrGlobali;
                    selezEsclOldObj.Valore = oggettoCustom.VALORE_DATABASE;
                    this.TemplateProject.OLD_OGG_CUSTOM[index] = selezEsclOldObj;
                }
            }
        }

        private void inserisciContatore(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, ArrayList dirittiCampiRuolo)
        {
            bool paneldll = false;
            bool contaDopo = false;
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                return;

            Panel divColCountAfter = new Panel();

            Label etichettaContatore = new Label();
            etichettaContatore.Text = oggettoCustom.DESCRIZIONE;
            etichettaContatore.CssClass = "weight";
            etichettaContatore.EnableViewState = true;

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaContatore);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);



            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            Label etichettaDDL = new Label();
            etichettaDDL.EnableViewState = true;
            etichettaDDL.Width = 50;
            DropDownList ddl = new DropDownList();
            ddl.EnableViewState = true;

            string language = UIManager.UserManager.GetUserLanguage();
            ddl.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));

            Panel divRowDll = new Panel();
            divRowDll.CssClass = "row";
            divRowDll.EnableViewState = true;

            Panel divRowEtiDll = new Panel();
            divRowEtiDll.CssClass = "row";
            divRowEtiDll.EnableViewState = true;

            HtmlGenericControl parDll = new HtmlGenericControl("p");
            parDll.EnableViewState = true;

            if (string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {

                Ruolo ruoloUtente = RoleManager.GetRoleInSession();
                Registro[] registriRfVisibili = UIManager.RegistryManager.GetListRegistriesAndRF(ruoloUtente.systemId, string.Empty, string.Empty);

                Panel divColDllEti = new Panel();
                divColDllEti.CssClass = "col";
                divColDllEti.EnableViewState = true;

                Panel divColDll = new Panel();
                divColDll.CssClass = "col";
                divColDll.EnableViewState = true;

                switch (oggettoCustom.TIPO_CONTATORE)
                {
                    case "T":
                        break;
                    case "A":
                        paneldll = true;
                        etichettaDDL.Text = "&nbsp;AOO&nbsp;";
                        etichettaDDL.CssClass = "weight";
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "chzn-select-deselect";
                        ddl.Width = 240;

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "0" && !((Registro)registriRfVisibili[i]).Sospeso)
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Text = string.Empty;
                            it.Value = string.Empty;
                            ddl.Items.Add(it);
                            ddl.SelectedValue = string.Empty;
                        }
                        else
                        {
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                        }
                        if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
                        {
                            ddl.Enabled = false;
                        }

                        parDll.Controls.Add(etichettaDDL);
                        divColDllEti.Controls.Add(parDll);
                        divRowEtiDll.Controls.Add(divColDllEti);

                        divColDll.Controls.Add(ddl);
                        divRowDll.Controls.Add(divColDll);

                        break;
                    case "R":
                        paneldll = true;
                        etichettaDDL.Text = "&nbsp;RF&nbsp;";
                        etichettaDDL.CssClass = "weight";
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "chzn-select-deselect";
                        ddl.Width = 240;

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "1" && ((Registro)registriRfVisibili[i]).rfDisabled == "0")
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Value = string.Empty;
                            it.Text = string.Empty;
                            ddl.Items.Add(it);
                            ddl.SelectedValue = string.Empty;
                        }
                        else
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                        if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
                        {
                            ddl.Enabled = false;
                        }

                        ddl.CssClass = "chzn-select-deselect";

                        parDll.Controls.Add(etichettaDDL);
                        divColDllEti.Controls.Add(parDll);
                        divRowEtiDll.Controls.Add(divColDllEti);

                        divColDll.Controls.Add(ddl);
                        divRowDll.Controls.Add(divColDll);
                        break;

                }
            }

            //Imposto il contatore in funzione del formato
            CustomTextArea contatore = new CustomTextArea();
            contatore.EnableViewState = true;
            contatore.ID = oggettoCustom.SYSTEM_ID.ToString();
            contatore.CssClass = "txt_textdata_counter";
            contatore.CssClassReadOnly = "txt_textdata_counter_disabled";
            if (oggettoCustom.FORMATO_CONTATORE != "")
            {
                contatore.Text = oggettoCustom.FORMATO_CONTATORE;
                if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                {
                    //controllo se il contatore è custom in tal caso visualizzo anno accademico e non il semplice anno solare
                    if (!string.IsNullOrEmpty(oggettoCustom.ANNO_ACC))
                    {
                        string IntervalloDate = oggettoCustom.DATA_INIZIO.Substring(6, 4) + oggettoCustom.DATA_FINE.Substring(5, 5);
                        contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO_ACC);
                    }
                    else
                    { contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO); }
                    contatore.Text = contatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);
                    string codiceAmministrazione = UserManager.getInfoAmmCorrente(UIManager.UserManager.GetInfoUser().idAmministrazione).Codice;
                    contatore.Text = contatore.Text.Replace("COD_AMM", codiceAmministrazione);
                    contatore.Text = contatore.Text.Replace("COD_UO", oggettoCustom.CODICE_DB);

                    if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
                    {
                        int fine = oggettoCustom.DATA_INSERIMENTO.LastIndexOf(".");
                        if (fine == -1)
                            fine = oggettoCustom.DATA_INSERIMENTO.LastIndexOf(":");
                        contatore.Text = contatore.Text.Replace("gg/mm/aaaa hh:mm", oggettoCustom.DATA_INSERIMENTO.Substring(0, fine));
                        contatore.Text = contatore.Text.Replace("gg/mm/aaaa", oggettoCustom.DATA_INSERIMENTO.Substring(0, 10));
                    }

                    if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && oggettoCustom.ID_AOO_RF != "0")
                    {
                        Registro reg = UserManager.getRegistroBySistemId(this, oggettoCustom.ID_AOO_RF);
                        if (reg != null)
                        {
                            contatore.Text = contatore.Text.Replace("RF", reg.codRegistro);
                            contatore.Text = contatore.Text.Replace("AOO", reg.codRegistro);
                        }
                    }
                }
                else
                {
                    contatore.Text = "";
                }
            }
            else
            {
                contatore.Text = oggettoCustom.VALORE_DATABASE;
            }

            Panel divRowCounter = new Panel();
            divRowCounter.CssClass = "row";
            divRowCounter.EnableViewState = true;

            Panel divColCountCounter = new Panel();
            divColCountCounter.CssClass = "col";
            divColCountCounter.EnableViewState = true;


            CheckBox cbContaDopo = new CheckBox();
            cbContaDopo.EnableViewState = true;
            //Pulsante annulla
            CustomButton btn_annulla = new CustomButton();
            btn_annulla.EnableViewState = true;
            btn_annulla.ID = "btn_a_" + oggettoCustom.SYSTEM_ID.ToString();
            btn_annulla.Text = Utils.Languages.GetLabelFromCode("BtnAbortProflier", language);
            btn_annulla.Visible = false;
            btn_annulla.CssClass = "buttonAbort";
            btn_annulla.OnMouseOver = "buttonAbortHover";
            btn_annulla.CssClassDisabled = "buttonAbortDisable";

            Panel divColCountAbort = new Panel();
            divColCountAbort.CssClass = "col-right-no-margin-no-top";
            divColCountAbort.EnableViewState = true;


            //if (!String.IsNullOrEmpty(this.DocumentInWorking.docNumber))
            //{
            //    btn_annulla.Click += this.OnBtnCounterAbort_Click;
            //}


            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloContatore(etichettaContatore, contatore, cbContaDopo, etichettaDDL, ddl, oggettoCustom, this.TemplateProject, btn_annulla, dirittiCampiRuolo);


            if (etichettaContatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            contatore.ReadOnly = true;
            //if (oggettoCustom != null && !String.IsNullOrEmpty(oggettoCustom.DATA_ANNULLAMENTO) && this.EnableRepertory)
            //{
            //    contatore.Text += " -- Annullato il :" + oggettoCustom.DATA_ANNULLAMENTO;
            //    contatore.CssClassReadOnly = "txt_textdata_counter_disabled_red";
            //    this.UpPnlTypeDocument.Update();
            //}
            //contatore.Enabled = false;
            //Inserisco il cb per il conta dopo
            if (oggettoCustom.CONTA_DOPO == "1")
            {
                cbContaDopo.ID = oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo";
                cbContaDopo.Checked = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                cbContaDopo.ToolTip = "Attiva / Disattiva incremento del contatore al salvataggio dei dati.";

                if ((!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE) && ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45"))
                {
                    cbContaDopo.Checked = false;
                    cbContaDopo.Visible = false;
                    cbContaDopo.Enabled = false;
                }
                else
                {
                    contaDopo = true;
                }

                divColCountAfter.CssClass = "col";
                divColCountAfter.EnableViewState = true;
                divColCountAfter.Controls.Add(cbContaDopo);
                //divRowDll.Controls.Add(divColCountAfter);
            }


            divColCountCounter.Controls.Add(contatore);
            divRowCounter.Controls.Add(divColCountCounter);



            divColCountAbort.Controls.Add(btn_annulla);
            divRowCounter.Controls.Add(divColCountAbort);



            if (paneldll)
            {
                this.PnlTypeDocument.Controls.Add(divRowEtiDll);
                this.PnlTypeDocument.Controls.Add(divRowDll);

            }

            if (contaDopo)
            {
                divRowCounter.Controls.Add(divColCountAfter);
            }

            if (contatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowCounter);
            }

        }

        //protected void OnBtnCounterAbort_Click(object sender, EventArgs e)
        //{
        //    string idOggetto = (((CustomButton)sender).ID);
        //    this.IdObjectCustom = idOggetto;
        //  ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpPnlTypeDocument", "ajaxModalPopupAbortCounter();", true);
        //}

        private void inserisciData(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            //Per il momento questo tipo di campo è stato implementato con tre semplici textBox
            //Sarebbe opportuno creare un oggetto personalizzato, che espone le stesse funzionalità
            //della textBox, ma che mi permette di gestire la data con i tre campi separati.
            DocsPaWR.StoricoProfilatiOldValue dataOldOb = new StoricoProfilatiOldValue();
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            Label etichettaData = new Label();
            etichettaData.EnableViewState = true;

            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaData.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaData.Text = oggettoCustom.DESCRIZIONE;
            }
            etichettaData.CssClass = "weight";

            UserControls.Calendar data = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            data.EnableViewState = true;
            data.ID = oggettoCustom.SYSTEM_ID.ToString();
            data.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);
            data.SetEnableTimeMode();

            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                data.Text = oggettoCustom.VALORE_DATABASE;
            }

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaData, data, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && (ProjectManager.getProjectInSession().accessRights == "45" || !this.EnableEdit || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C"))))
            {
                data.ReadOnly = true;
            }

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
            {
                if (this.TemplateProject.OLD_OGG_CUSTOM[index] == null) //se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
                {
                    //blocco storico profilazione campo data
                    dataOldOb.IDTemplate = this.TemplateProject.ID_TIPO_FASC;
                    Fascicolo fasc = ProjectManager.getProjectInSession();
                    dataOldOb.ID_Doc_Fasc = fasc.systemID;
                    dataOldOb.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                    dataOldOb.Valore = oggettoCustom.VALORE_DATABASE;
                    dataOldOb.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                    InfoUtente user = UserManager.GetInfoUser();
                    dataOldOb.ID_People = user.idPeople;
                    dataOldOb.ID_Ruolo_In_UO = user.idCorrGlobali;
                    this.TemplateProject.OLD_OGG_CUSTOM[index] = dataOldOb;
                }
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaData);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            if (etichettaData.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col";
            divColValue.EnableViewState = true;

            divColValue.Controls.Add(data);
            divRowValue.Controls.Add(divColValue);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void inserisciLink(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            UserControls.LinkDocFasc link = (UserControls.LinkDocFasc)this.LoadControl("../UserControls/LinkDocFasc.ascx");
            link.EnableViewState = true;
            link.From = "P";
            link.ID = oggettoCustom.SYSTEM_ID.ToString();
            link.IsEsterno = (oggettoCustom.TIPO_LINK.Equals("ESTERNO"));
            link.IsFascicolo = ("FASCICOLO".Equals(oggettoCustom.TIPO_OBJ_LINK));

            if ("SI".Equals(oggettoCustom.CAMPO_OBBLIGATORIO))
            {
                link.TxtEtiLinkDocOrFasc = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                link.TxtEtiLinkDocOrFasc = oggettoCustom.DESCRIZIONE;
            }
            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(link.TxtEtiLinkDocOrFasc, link, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);
            Fascicolo sd = ProjectManager.getProjectInSession();
            if (sd != null && !string.IsNullOrEmpty(sd.systemID) && (!string.IsNullOrEmpty(sd.accessRights) && (sd.accessRights == "45")))
            {
                link.IsInsertModify = false;
            }

            //bool greyWithDocNum = "G".Equals(sd.tipoProto) && !string.IsNullOrEmpty(sd.docNumber);
            //if (!string.IsNullOrEmpty(this.DocumentInWorking.systemId) && (!string.IsNullOrEmpty(sd.accessRights) && (accessRights == "45" || readOnly)))
            //{
            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                link.HideLink = true;
            }
            //}
            //else
            //{
            //    link.HideLink = false;
            //}

            link.Value = oggettoCustom.VALORE_DATABASE;

            if (link.Visible)
            {
                this.PnlTypeDocument.Controls.Add(link);
            }
        }

        private void inserisciCampoSeparatore(DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            Label etichettaCampoSeparatore = new Label();
            etichettaCampoSeparatore.CssClass = "weight";
            etichettaCampoSeparatore.EnableViewState = true;
            etichettaCampoSeparatore.Text = oggettoCustom.DESCRIZIONE.ToUpper();

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col_full_line";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaCampoSeparatore);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            if (etichettaCampoSeparatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }
        }

        private void inserisciCorrispondente(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            DocsPaWR.StoricoProfilatiOldValue corrOldOb = new StoricoProfilatiOldValue();

            UserControls.CorrespondentCustom corrispondente = (UserControls.CorrespondentCustom)this.LoadControl("../UserControls/CorrespondentCustom.ascx");
            corrispondente.EnableViewState = true;

            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                corrispondente.TxtEtiCustomCorrespondent = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                corrispondente.TxtEtiCustomCorrespondent = oggettoCustom.DESCRIZIONE;
            }


            corrispondente.TypeCorrespondentCustom = oggettoCustom.TIPO_RICERCA_CORR;
            corrispondente.ID = oggettoCustom.SYSTEM_ID.ToString();

            //Da amministrazione è stato impostato un ruolo di default per questo campo.
            if (!string.IsNullOrEmpty(oggettoCustom.ID_RUOLO_DEFAULT) && oggettoCustom.ID_RUOLO_DEFAULT != "0")
            {
                DocsPaWR.Ruolo ruolo = RoleManager.getRuoloById(oggettoCustom.ID_RUOLO_DEFAULT);
                if (ruolo != null)
                {
                    corrispondente.IdCorrespondentCustom = ruolo.systemId;
                    corrispondente.TxtCodeCorrespondentCustom = ruolo.codiceRubrica;
                    corrispondente.TxtDescriptionCorrespondentCustom = ruolo.descrizione;
                }
                oggettoCustom.ID_RUOLO_DEFAULT = "0";
            }

            //Il campo è valorizzato.
            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                DocsPaWR.Corrispondente corr_1 = AddressBookManager.getCorrispondenteBySystemIDDisabled(oggettoCustom.VALORE_DATABASE);
                if (corr_1 != null)
                {
                    corrispondente.IdCorrespondentCustom = corr_1.systemId;
                    corrispondente.TxtCodeCorrespondentCustom = corr_1.codiceRubrica.ToString();
                    corrispondente.TxtDescriptionCorrespondentCustom = corr_1.descrizione.ToString();
                    oggettoCustom.VALORE_DATABASE = corr_1.systemId;
                }
            }

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(corrispondente.TxtEtiCustomCorrespondent, corrispondente, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
            {
                corrispondente.DisabledCorrespondentCustom = true;
            }

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
            {
                if (this.TemplateProject.OLD_OGG_CUSTOM[index] == null) //se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
                {
                    //blocco storico profilazione campo corrispondente
                    corrOldOb.IDTemplate = this.TemplateProject.ID_TIPO_FASC;
                    Fascicolo fasc = ProjectManager.getProjectInSession();
                    corrOldOb.ID_Doc_Fasc = fasc.systemID;
                    corrOldOb.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                    corrOldOb.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                    if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
                        corrOldOb.Valore = corrispondente.TxtCodeCorrespondentCustom + "<br/>" + "----------" + "<br/>" + corrispondente.TxtDescriptionCorrespondentCustom;
                    else
                        corrOldOb.Valore = "";
                    InfoUtente user = UserManager.GetInfoUser();
                    corrOldOb.ID_People = user.idPeople;
                    corrOldOb.ID_Ruolo_In_UO = user.idCorrGlobali;
                    this.TemplateProject.OLD_OGG_CUSTOM[index] = corrOldOb;
                }
            }


            if (corrispondente.Visible)
            {
                this.PnlTypeDocument.Controls.Add(corrispondente);
            }

        }


        public void impostaDirittiRuoloSulCampo(Object etichetta, Object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, ArrayList dirittiCampiRuolo)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneDocManager.getDirittiCampoTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO") || !this.EnableEdit || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                            {
                                ((CustomTextArea)campo).ReadOnly = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((CustomTextArea)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "CasellaDiSelezione":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO") || !this.EnableEdit || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                            {
                                ((System.Web.UI.WebControls.CheckBoxList)campo).Enabled = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.CheckBoxList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "MenuATendina":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO") || !this.EnableEdit || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                            {
                                ((System.Web.UI.WebControls.DropDownList)campo).Enabled = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.DropDownList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "SelezioneEsclusiva":
                            //Per la selezione esclusiva è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Contatore":
                            //Per il contatore è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Data":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO") || !this.EnableEdit || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                            {
                                ((UserControls.Calendar)campo).ReadOnly = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.Calendar)campo).Visible = false;
                                ((UserControls.Calendar)campo).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Corrispondente":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO") || !this.EnableEdit || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                            {
                                ((UserControls.CorrespondentCustom)campo).CODICE_READ_ONLY = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            else
                            {
                                ((UserControls.CorrespondentCustom)campo).CODICE_READ_ONLY = false;
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((UserControls.CorrespondentCustom)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Link":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO") || !this.EnableEdit || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                            {
                                ((UserControls.LinkDocFasc)campo).IsInsertModify = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            else
                            {
                                ((UserControls.LinkDocFasc)campo).IsInsertModify = true;
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((UserControls.LinkDocFasc)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            else
                            {
                                ((UserControls.LinkDocFasc)campo).IsInsertModify = true;
                            }


                            break;
                        case "OggettoEsterno":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO") || !this.EnableEdit || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                            {
                                ((UserControls.IntegrationAdapter)campo).View = UserControls.IntegrationAdapterView.READ_ONLY;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            else
                            {
                                ((UserControls.IntegrationAdapter)campo).View = UserControls.IntegrationAdapterView.INSERT_MODIFY;
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.IntegrationAdapter)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                    }
                }
            }
        }

        public void impostaDirittiRuoloSelezioneEsclusiva(Object etichetta, Object campo, Object button, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, ArrayList dirittiCampiRuolo)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneDocManager.getDirittiCampoTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO") || !this.EnableEdit || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                    {
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Enabled = false;
                        ((CustomImageButton)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Visible = false;
                        ((CustomImageButton)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                }
            }
        }

        public void impostaDirittiRuoloContatoreSottocontatore(Object etichettaContatoreSottocontatore, Object contatore, Object sottocontatore, Object dataInserimentoSottocontatore, Object checkBox, Object etichettaDDL, Object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                    {
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Enabled = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Enabled = false;

                        //Se il contatore è solo visibile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichettaContatoreSottocontatore).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                        ((CustomTextArea)contatore).Visible = false;
                        ((CustomTextArea)sottocontatore).Visible = false;
                        ((CustomTextArea)dataInserimentoSottocontatore).Visible = false;
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Visible = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Visible = false;

                        //Se il contatore non è nè visibile nè modificabile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }
                }
            }
        }

        private int impostaSelezioneEsclusiva(string valore, RadioButtonList rbl)
        {
            for (int i = 0; i < rbl.Items.Count; i++)
            {
                if (rbl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        private void impostaSelezioneCaselleDiSelezione(DocsPaWR.OggettoCustom objCustom, CheckBoxList cbl)
        {
            for (int i = 0; i < objCustom.VALORI_SELEZIONATI.Length; i++)
            {
                for (int j = 0; j < cbl.Items.Count; j++)
                {
                    if ((string)objCustom.VALORI_SELEZIONATI[i] == cbl.Items[j].Text)
                    {
                        cbl.Items[j].Selected = true;
                    }
                }
            }
        }

        private int impostaSelezioneMenuATendina(string valore, DropDownList ddl)
        {
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        private void impostaDirittiRuoloContatore(Object etichettaContatore, Object campo, Object checkBox, Object etichettaDDL, Object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, Button btn_annulla, ArrayList dirittiCampiRuolo)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneDocManager.getDirittiCampoTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                    {
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Enabled = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Enabled = false;

                        //Se il contatore è solo visibile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichettaContatore).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                        ((CustomTextArea)campo).Visible = false;
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Visible = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Visible = false;

                        //Se il contatore non è nè visibile nè modificabile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }

                    btn_annulla.Visible = false;
                }
            }
        }

        public void impostaDirittiRuoloContatoreSottocontatore(Object etichettaContatoreSottocontatore, Object contatore, Object sottocontatore, Object dataInserimentoSottocontatore, Object checkBox, Object etichettaDDL, Object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, ArrayList dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                    {
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Enabled = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Enabled = false;

                        //Se il contatore è solo visibile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichettaContatoreSottocontatore).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                        ((CustomTextArea)contatore).Visible = false;
                        ((CustomTextArea)sottocontatore).Visible = false;
                        ((CustomTextArea)dataInserimentoSottocontatore).Visible = false;
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Visible = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Visible = false;

                        //Se il contatore non è nè visibile nè modificabile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }
                }
            }
        }

        protected void cancella_selezioneEsclusiva_Click(object sender, EventArgs e)
        {
            string idOggetto = (((CustomImageButton)sender).ID).Substring(1);
            ((RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto)).SelectedIndex = -1;
            ((RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto)).EnableViewState = true;
            for (int i = 0; i < this.TemplateProject.ELENCO_OGGETTI.Length; i++)
            {
                if (((DocsPaWR.OggettoCustom)this.TemplateProject.ELENCO_OGGETTI[i]).SYSTEM_ID.ToString().Equals(idOggetto))
                {
                    ((DocsPaWR.OggettoCustom)this.TemplateProject.ELENCO_OGGETTI[i]).VALORE_DATABASE = "-1";
                }
            }
            this.UpPnlTypeDocument.Update();
        }

        protected Templates PopulateTemplate()
        {

            Templates result = this.TemplateProject;
            if (result != null)
            {
                for (int i = 0; i < result.ELENCO_OGGETTI.Length; i++)
                {
                    DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)result.ELENCO_OGGETTI[i];
                    this.salvaValoreCampo(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString());
                }
                this.TemplateProject = result;
            }
            return result;
        }

        private void salvaValoreCampo(DocsPaWR.OggettoCustom oggettoCustom, string idOggetto)
        {
            //In questo metodo, oltre al controllo si salvano i valori dei campi inseriti 
            //dall'utente nel template in sessione. Solo successivamente, quanto verra' salvato 
            //il documento i suddetti valori verranno riportai nel Db vedi metodo "btn_salva_Click" della "docProfilo.aspx"

            //Label_Avviso.Visible = false;
            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "CampoDiTesto":
                    CustomTextArea textBox = (CustomTextArea)this.PnlTypeDocument.FindControl(idOggetto);
                    if (textBox != null)
                    {
                        oggettoCustom.VALORE_DATABASE = textBox.Text;
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = string.Empty;
                    }
                    break;
                case "CasellaDiSelezione":
                    CheckBoxList casellaSelezione = (CheckBoxList)this.PnlTypeDocument.FindControl(idOggetto);
                    //Nessuna selezione
                    if (casellaSelezione != null)
                    {
                        if (casellaSelezione.SelectedIndex == -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                        {
                            for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Length; i++)
                                oggettoCustom.VALORI_SELEZIONATI[i] = null;
                            return;
                        }

                        //Controllo eventuali selezioni
                        oggettoCustom.VALORI_SELEZIONATI = new string[oggettoCustom.ELENCO_VALORI.Length];
                        oggettoCustom.VALORE_DATABASE = string.Empty;

                        for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
                        {
                            DocsPaWR.ValoreOggetto valoreOggetto = (DocsPaWR.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i];
                            foreach (ListItem valoreSelezionato in casellaSelezione.Items)
                            {
                                if (valoreOggetto.VALORE == valoreSelezionato.Text && valoreSelezionato.Selected)
                                    oggettoCustom.VALORI_SELEZIONATI[i] = valoreSelezionato.Text;
                            }
                        }
                    }
                    else
                    {
                        //Controllo eventuali selezioni
                        oggettoCustom.VALORI_SELEZIONATI = new string[oggettoCustom.ELENCO_VALORI.Length];
                        oggettoCustom.VALORE_DATABASE = string.Empty;
                    }
                    break;
                case "MenuATendina":
                    DropDownList dropDwonList = (DropDownList)this.PnlTypeDocument.FindControl(idOggetto);
                    if (dropDwonList != null)
                    {
                        oggettoCustom.VALORE_DATABASE = dropDwonList.SelectedItem.Text;
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = string.Empty;
                    }
                    break;
                case "SelezioneEsclusiva":
                    RadioButtonList radioButtonList = (RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto);
                    if (radioButtonList != null)
                    {
                        if ((oggettoCustom.VALORE_DATABASE == "-1" && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                            return;
                        }
                        if (oggettoCustom.VALORE_DATABASE == "-1")
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                        }
                        else
                        {
                            if (radioButtonList.SelectedItem != null)
                                oggettoCustom.VALORE_DATABASE = radioButtonList.SelectedItem.Text;
                        }
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = string.Empty;
                    }
                    break;
                case "Data":
                    UserControls.Calendar data = (UserControls.Calendar)this.PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    if (data != null)
                    {
                        if (string.IsNullOrEmpty(data.Text) && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                            return;
                        }
                        if (string.IsNullOrEmpty(data.Text))
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                        else
                            oggettoCustom.VALORE_DATABASE = data.Text;
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = string.Empty;
                    }
                    break;
                case "Corrispondente":
                    UserControls.CorrespondentCustom corr = (UserControls.CorrespondentCustom)this.PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    if (corr != null)
                    {
                        oggettoCustom.VALORE_DATABASE = corr.IdCorrespondentCustom;
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = string.Empty;
                    }
                    break;
                case "Contatore":
                case "ContatoreSottocontatore":
                    if (string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
                    {
                        switch (oggettoCustom.TIPO_CONTATORE)
                        {
                            case "A":
                                DropDownList ddlAoo = (DropDownList)this.PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu"); if (ddlAoo != null)
                                {
                                    oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                                }
                                break;
                            case "R":
                                DropDownList ddlRf = (DropDownList)this.PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu"); if (ddlRf != null)
                                {
                                    oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
                                }
                                break;
                        }
                        if (oggettoCustom.CONTA_DOPO == "1")
                        {
                            CheckBox cbContaDopo = (CheckBox)this.PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo");
                            if (cbContaDopo != null)
                            {
                                oggettoCustom.CONTATORE_DA_FAR_SCATTARE = cbContaDopo.Checked;
                            }
                        }
                        else
                        {
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE) && oggettoCustom.FORMATO_CONTATORE.LastIndexOf("COD_UO") != -1)
                        {
                            if (UIManager.RoleManager.GetRoleInSession() != null && RoleManager.GetRoleInSession().uo != null)
                                oggettoCustom.CODICE_DB = RoleManager.GetRoleInSession().uo.codice;
                        }
                    }
                    break;
                case "Link":
                    LinkDocFasc link = (LinkDocFasc)this.PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    oggettoCustom.VALORE_DATABASE = link.Value;
                    break;
                case "OggettoEsterno":
                    IntegrationAdapter intAd = (IntegrationAdapter)this.PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    IntegrationAdapterValue value = intAd.Value;
                    if (value != null)
                    {
                        oggettoCustom.VALORE_DATABASE = value.Descrizione;
                        oggettoCustom.CODICE_DB = value.Codice;
                        oggettoCustom.MANUAL_INSERT = value.ManualInsert;
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                        oggettoCustom.CODICE_DB = "";
                        oggettoCustom.MANUAL_INSERT = false;
                    }
                    break;
            }
        }

        protected void GridView_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "viewDetails")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                string idProfile = GrigliaResult.Rows[rowIndex]["IdProfile"].ToString();
                SchedaDocumento schedaDocumento = UIManager.DocumentManager.getDocumentDetails(this, idProfile, idProfile);
                Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                string language = UIManager.UserManager.GetUserLanguage();

                if (!string.IsNullOrEmpty(this.SelectedRow))
                {
                    if (rowIndex != Int32.Parse(this.SelectedRow))
                    {
                        this.SelectedRow = string.Empty;
                    }
                }


                HttpContext.Current.Session["isZoom"] = null;
                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject obj = navigationList.Last();
                Navigation.NavigationObject newObj = Navigation.NavigationUtils.CloneObject(obj);

                newObj.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), string.Empty);
                newObj.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), true,this.Page);
                newObj.CodePage = Navigation.NavigationUtils.NamePage.PROJECT.ToString();
                newObj.Page = "PROJECT.ASPX";
                newObj.IdObject = schedaDocumento.systemId;
                newObj.OriginalObjectId = schedaDocumento.systemId;
                newObj.NumPage = this.SelectedPage.ToString();
                newObj.PageSize = this.PageSize.ToString();
                newObj.DxTotalPageNumber = this.PagesCount.ToString();
                newObj.DxTotalNumberElement = this.RecordCount.ToString();
                newObj.SearchFilters = this.SearchFilters;
                newObj.SearchFiltersOrder = UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid);
                newObj.ViewResult = true;
                newObj.folder = fascicolo.folderSelezionato;
                newObj.idProject = fascicolo.systemID;
                int indexElement = (((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1))+1;
                newObj.DxPositionElement = indexElement.ToString();

                if (obj.NamePage.Equals(Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), string.Empty)) && !string.IsNullOrEmpty(obj.IdObject) && obj.IdObject.Equals(newObj.idProject))
                {
                    navigationList.Remove(obj);
                }
                navigationList.Add(newObj);
                Navigation.NavigationUtils.SetNavigationList(navigationList);

                UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                this.ListObjectNavigation = this.Result;
                Response.Redirect("~/Document/Document.aspx");


            }
        }


        #endregion

        #region TRANSMISSION MODELS
        private void LoadTransmissionModels()
        {

            string idDiagram = string.Empty;
            string idState = string.Empty;
            string idFasc = string.Empty;
            string accessRights = string.Empty;
            string RegTit = string.Empty;
            string idTipoFasc = string.Empty;

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
            {
                accessRights = ProjectManager.getProjectInSession().accessRights;
                idFasc = ProjectManager.getProjectInSession().systemID;
            }
            InfoUtente infoUtente = UserManager.GetInfoUser();
            Utente utente = UserManager.GetUserInSession();
            Ruolo ruolo = RoleManager.GetRoleInSession();
            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().template != null && ProjectManager.getProjectInSession().template.SYSTEM_ID != 0)
            {
                if (this.EnableStateDiagram)
                {
                    DiagrammaStato dia = DiagrammiManager.getDgByIdTipoFasc(ProjectManager.getProjectInSession().template.SYSTEM_ID.ToString(), infoUtente.idAmministrazione);
                    if (dia != null)
                    {
                        idDiagram = dia.SYSTEM_ID.ToString();
                        idTipoFasc = ProjectManager.getProjectInSession().template.SYSTEM_ID.ToString();
                        DocsPaWR.Stato stato = DiagrammiManager.getStatoFasc(ProjectManager.getProjectInSession().systemID);
                        if (stato != null)
                            idState = stato.SYSTEM_ID.ToString();
                    }
                }
            }

            Registro[] listReg = null;
            if (listReg == null)
            {
                listReg = ruolo.registri;
            }

            ArrayList idModelli = new ArrayList(UIManager.TransmissionModelsManager.GetTransmissionModelsLiteFasc(utente.idAmministrazione, listReg, utente.idPeople, infoUtente.idCorrGlobali, idTipoFasc, idDiagram, idState, "F", idFasc, ruolo.idGruppo, true, accessRights));

            ModelloTrasmissione[] modTrasm = idModelli.Cast<ModelloTrasmissione>().ToArray();
            modTrasm = (from mod in modTrasm orderby mod.NOME ascending select mod).ToArray<ModelloTrasmissione>();
            idModelli = ArrayList.Adapter(modTrasm.ToList());

            System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem("", "");
            this.ProjectDdlTransmissionsModel.Items.Add(li);

            for (int i = 0; i < idModelli.Count; i++)
            {
                DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)idModelli[i];
                li = new System.Web.UI.WebControls.ListItem();
                li.Value = mod.SYSTEM_ID.ToString();
                li.Text = mod.NOME;
                if (this.ViewCodeTransmissionModels)
                {
                    li.Text += " (" + mod.CODICE + ")";
                }
                this.ProjectDdlTransmissionsModel.Items.Add(li);
            }

        }

        private void PerformActionTransmitModel()
        {
            Registro registro = UIManager.RegistryManager.getRegistroBySistemId(projectDdlRegistro.SelectedValue);
            Trasmissione trasmEff = this.BuildTransmissionFromModel();
            if (trasmEff != null && trasmEff.utente == null) trasmEff.utente = UserManager.GetUserInSession();
            if (trasmEff != null && trasmEff.ruolo == null) trasmEff.ruolo = UserManager.GetSelectedRole();

            DocsPaWR.ModelloTrasmissione template = TrasmManager.getModelloTrasmNuovo(trasmEff, registro.systemId);

            if (!this.notificheUtImpostate(template))
            {
                this.ProjectDdlTransmissionsModel.SelectedIndex = -1;
                this.UpPnlTransmissionsModel.Update();

                string msg = "ErrorTransmissionRecipientsNotify";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', 'Trasmissione modello');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', 'Trasmissione modello');}", true);

                return;
            }


            Trasmissione trasmissione = new DocsPaWR.Trasmissione();
            if (template != null)
                trasmissione.NO_NOTIFY = trasmEff.NO_NOTIFY;

            //Parametri della trasmissione
            trasmissione.noteGenerali = template.VAR_NOTE_GENERALI;
            trasmissione.tipoOggetto = DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
            trasmissione.infoFascicolo = ProjectManager.getInfoFascicoloDaFascicolo(UIManager.ProjectManager.getProjectInSession());
            trasmissione.utente = UserManager.GetUserInSession();
            trasmissione.ruolo = UserManager.GetSelectedRole();


            // gestione della cessione diritti
            if (template.CEDE_DIRITTI != null && template.CEDE_DIRITTI.Equals("1"))
            {
                DocsPaWR.CessioneDocumento objCessione = new DocsPaWR.CessioneDocumento();

                objCessione.docCeduto = true;

                //*******************************************************************************************
                // MODIFICA IACOZZILLI GIORDANO 30/07/2012
                // Modifica inerente la cessione dei diritti di un doc da parte di un utente non proprietario ma 
                // nel ruolo del proprietario, in quel caso non posso valorizzare l'IDPEOPLE  con il corrente perchè
                // il proprietario può essere un altro utente del mio ruolo, quindi andrei a generare un errore nella security,
                // devo quindi controllare che nell'idpeople venga inserito l'id corretto del proprietario.
                string valoreChiaveCediDiritti = InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_CEDI_DIRITTI_IN_RUOLO.ToString());
                if (!string.IsNullOrEmpty(valoreChiaveCediDiritti) && valoreChiaveCediDiritti.Equals("1"))
                {
                    //Devo istanziare una classe utente.
                    string idProprietario = string.Empty;
                    idProprietario = GetAnagUtenteProprietario();
                    Utente _utproprietario = UserManager.GetUtenteByIdPeople(idProprietario);

                    objCessione.idPeople = idProprietario;
                    objCessione.idRuolo = UserManager.GetInfoUser().idGruppo;
                    objCessione.userId = _utproprietario.cognome + " " + _utproprietario.nome;


                    if (objCessione.idPeople == null || objCessione.idPeople == "")
                        objCessione.idPeople = idProprietario;

                    if (objCessione.idRuolo == null || objCessione.idRuolo == "")
                        objCessione.idRuolo = UserManager.GetInfoUser().idGruppo;

                    if (objCessione.userId == null || objCessione.userId == "")
                        objCessione.userId = _utproprietario.cognome + " " + _utproprietario.nome;
                }
                else
                {
                    //OLD CODE:
                    objCessione.idPeople = UserManager.GetInfoUser().idPeople;
                    objCessione.idRuolo = UserManager.GetInfoUser().idGruppo;
                    objCessione.userId = UserManager.GetInfoUser().userId;
                }
                //*******************************************************************************************
                // FINE MODIFICA
                //********************************************************************************************
                if (template.ID_PEOPLE_NEW_OWNER != null && template.ID_PEOPLE_NEW_OWNER != "")
                    objCessione.idPeopleNewPropr = template.ID_PEOPLE_NEW_OWNER;
                if (template.ID_GROUP_NEW_OWNER != null && template.ID_GROUP_NEW_OWNER != "")
                    objCessione.idRuoloNewPropr = template.ID_GROUP_NEW_OWNER;

                trasmissione.cessione = objCessione;
                // Se per il modello creato è prevista l'opzione di mantenimento dei diritti di lettura
                if (trasmEff.cessione == null)
                    if (!string.IsNullOrEmpty(template.MANTIENI_SCRITTURA) && template.MANTIENI_SCRITTURA == "1")
                    {
                        trasmissione.mantieniScrittura = true;
                        trasmissione.mantieniLettura = true;
                    }
                    else
                    {
                        trasmissione.mantieniScrittura = false;
                        // Se per il modello creato è prevista l'opzione di mantenimento dei diritti di lettura
                        if (!string.IsNullOrEmpty(template.MANTIENI_LETTURA) && template.MANTIENI_LETTURA == "1")
                            trasmissione.mantieniLettura = true;
                        else
                            trasmissione.mantieniLettura = false;
                    }
                else
                {
                    trasmissione.cessione = trasmEff.cessione;
                    trasmissione.mantieniLettura = trasmEff.mantieniLettura;
                    trasmissione.mantieniScrittura = trasmEff.mantieniScrittura;
                }
                // End Mev
            }

            bool eredita = false;
            //Parametri delle trasmissioni singole

            for (int i = 0; i < template.RAGIONI_DESTINATARI.Length; i++)
            {
                DocsPaWR.RagioneDest ragDest = (DocsPaWR.RagioneDest)template.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaWR.MittDest mittDest = (DocsPaWR.MittDest)destinatari[j];
                    DocsPaWR.Corrispondente corr = new Corrispondente();
                    if (mittDest.CHA_TIPO_MITT_DEST == "D")
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    else
                    {
                        corr = TrasmManager.getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, null, ProjectManager.getProjectInSession(), this);
                    }

                    if (corr != null)
                    {
                        DocsPaWR.RagioneTrasmissione ragione = TrasmManager.getRagioneById(mittDest.ID_RAGIONE.ToString());

                        try
                        {
                            trasmissione = this.addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, mittDest.NASCONDI_VERSIONI_PRECEDENTI);
                        }
                        catch (ExceptionTrasmissioni e)
                        {
                        }

                        if (ragione.eredita == "1")
                            eredita = true;
                    }
                }
            }


            DocsPaWR.Trasmissione t_rs = null;
            if (trasmissione.trasmissioniSingole != null && trasmissione.trasmissioniSingole.Length > 0)
            {
                trasmissione = this.impostaNotificheUtentiDaModello(trasmissione, template);

                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                if (infoUtente.delegato != null)
                    trasmissione.delegato = ((DocsPaWR.InfoUtente)(infoUtente.delegato)).idPeople;

                t_rs = TrasmManager.saveExecuteTrasm(this, trasmissione, infoUtente);
            }


            if (t_rs != null && t_rs.ErrorSendingEmails)
            {
                string msg = "ErrorTransmissionSendingEmails";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
            }

            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
            {
                Fascicolo prj = UIManager.ProjectManager.getProjectInSession();

                DocsPaWR.Stato stato = ProjectManager.getStatoFasc(prj);
                bool trasmWF = false;
                if (trasmissione != null && trasmissione.trasmissioniSingole != null &&
                    trasmissione.trasmissioniSingole.Length > 0)
                {
                    for (int j = 0; j < trasmissione.trasmissioniSingole.Length; j++)
                    {
                        DocsPaWR.TrasmissioneSingola trasmSing = (DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[j];
                        if (trasmSing.ragione.tipo == "W")
                            trasmWF = true;
                    }
                }
                if (stato != null && trasmWF)
                    TrasmManager.salvaStoricoTrasmDiagrammi(trasmissione.systemId, prj.systemID, Convert.ToString(stato.SYSTEM_ID));
            }


            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (ProjectManager.CheckRevocationAcl())
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()');}", true);
                return;
            }
            else
            {
                if (trasmissione.cessione != null && trasmissione.cessione.docCeduto)
                    if (trasmissione.mantieniLettura)
                        ProjectManager.setProjectInSession(ProjectManager.GetProjectByCode(RegistryManager.getRegistroBySistemId(ProjectManager.getProjectInSession().idRegistro), ProjectManager.getProjectInSession().codice));
            }


            this.ProjectDdlTransmissionsModel.SelectedIndex = -1;
            this.UpPnlTransmissionsModel.Update();
        }

        private void TransmitModel()
        {
            Registro registro = UIManager.RegistryManager.getRegistroBySistemId(projectDdlRegistro.SelectedValue);
            ModelloTrasmissione modello = TrasmManager.GetTemplateById(registro.idAmministrazione, this.ProjectDdlTransmissionsModel.SelectedValue);

            if (modello != null && modello.SYSTEM_ID != 0)
            {
                this.PerformActionTransmitModel();
            }
        }

        private Trasmissione BuildTransmissionFromModel()
        {
            DocsPaWR.Trasmissione trasmissione = new DocsPaWR.Trasmissione();
            try
            {
                DocsPaWR.ModelloTrasmissione template = TransmissionModelsManager.GetTemplateById(UserManager.GetInfoUser().idAmministrazione, this.ProjectDdlTransmissionsModel.SelectedValue);
                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

                if (template != null)
                    trasmissione.NO_NOTIFY = template.NO_NOTIFY;

                // gestione della cessione diritti
                if (template.CEDE_DIRITTI != null && template.CEDE_DIRITTI.Equals("1"))
                {
                    DocsPaWR.CessioneDocumento objCessione = new DocsPaWR.CessioneDocumento();

                    objCessione.docCeduto = true;
                    objCessione.idPeople = UserManager.GetInfoUser().idPeople;
                    objCessione.idRuolo = UserManager.GetInfoUser().idGruppo;
                    objCessione.idPeopleNewPropr = template.ID_PEOPLE_NEW_OWNER;
                    objCessione.idRuoloNewPropr = template.ID_GROUP_NEW_OWNER;
                    objCessione.userId = UserManager.GetInfoUser().userId;

                    if (!string.IsNullOrEmpty(template.MANTIENI_SCRITTURA) && !string.IsNullOrEmpty(template.MANTIENI_LETTURA))
                        if (Convert.ToBoolean(int.Parse(template.MANTIENI_SCRITTURA)))
                        {
                            trasmissione.mantieniLettura = true;
                            trasmissione.mantieniScrittura = true;
                        }
                        else
                        {
                            trasmissione.mantieniScrittura = false;
                            if (Convert.ToBoolean(int.Parse(template.MANTIENI_LETTURA)))
                                trasmissione.mantieniLettura = true;
                            else
                                trasmissione.mantieniLettura = false;
                        }

                    trasmissione.cessione = objCessione;
                }

                //Parametri della trasmissione
                trasmissione.noteGenerali = template.VAR_NOTE_GENERALI;
                trasmissione.tipoOggetto = DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
                trasmissione.infoFascicolo = ProjectManager.getInfoFascicoloDaFascicolo(UIManager.ProjectManager.getProjectInSession());

                //Parametri delle trasmissioni singole
                for (int i = 0; i < template.RAGIONI_DESTINATARI.Length; i++)
                {
                    DocsPaWR.RagioneDest ragDest = (DocsPaWR.RagioneDest)template.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {

                        DocsPaWR.MittDest mittDest = (DocsPaWR.MittDest)destinatari[j];
                        DocsPaWR.RagioneTrasmissione ragione = TrasmManager.getReasonById(mittDest.ID_RAGIONE.ToString());
                        DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                        if (corr != null) //corr nullo se non esiste o se è stato disabilitato                           
                            //Andrea - try - catch
                            try
                            {
                                trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, mittDest);
                            }
                            catch (ExceptionTrasmissioni e)
                            {
                                //Aggiungo l'errore alla lista
                            }
                        //End Andrea
                    }

                }

                if (template.CEDE_DIRITTI != null && template.CEDE_DIRITTI.Equals("1"))
                    trasmissione.trasmissioniSingole[0].ragione.cessioneImpostata = true;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

            //logger.Info("END");

            return trasmissione;
        }

        private bool notificheUtImpostate(DocsPaWR.ModelloTrasmissione modello)
        {
            bool retValue = true;
            bool flag = false;

            foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
            {
                foreach (DocsPaWR.MittDest mittDest in ragDest.DESTINATARI)
                {
                    if (!mittDest.CHA_TIPO_URP.Equals("U"))
                    {
                        // ritorna FALSE se anche un solo destinatario del modello non ha UTENTI_NOTIFICA
                        if (mittDest.UTENTI_NOTIFICA == null)
                            return false;

                        if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 0)
                        {
                            flag = false;

                            foreach (DocsPaWR.UtentiConNotificaTrasm utNot in mittDest.UTENTI_NOTIFICA)
                            {
                                if (utNot.FLAG_NOTIFICA.Equals("1"))
                                    flag = true;
                            }

                            // ritorna FALSE se anche un solo destinatario ha tutti gli utenti con le notifiche non impostate
                            if (!flag)
                                return false;
                        }
                    }
                }
            }

            return retValue;
        }

        private DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPaWR.Trasmissione trasmissione, DocsPaWR.Corrispondente corr, DocsPaWR.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, DocsPaWR.MittDest mittDest)
        {
            try
            {
                if (trasmissione.trasmissioniSingole != null)
                {
                    // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                    for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                    {
                        DocsPaWR.TrasmissioneSingola ts = (DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];

                        if (corr != null)
                            if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                            {
                                if (ts.daEliminare)
                                {
                                    ((DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                                    return trasmissione;
                                }
                                else
                                    return trasmissione;
                            }
                    }
                }

                // Aggiungo la trasmissione singola
                DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPaWR.TrasmissioneSingola();
                trasmissioneSingola.tipoTrasm = tipoTrasm;
                trasmissioneSingola.corrispondenteInterno = corr;
                trasmissioneSingola.ragione = ragione;
                trasmissioneSingola.noteSingole = note;

                //Imposto la data di scadenza
                if (scadenza > 0)
                {
                    //string dataScadenza = "";
                    System.DateTime data = System.DateTime.Now.AddDays(scadenza);
                    trasmissioneSingola.dataScadenza = dateformat.formatDataDocsPa(data);
                }

                // Aggiungo la lista di trasmissioniUtente
                if (corr is DocsPaWR.Ruolo)
                {
                    trasmissioneSingola.tipoDest = DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
                    DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr);

                    //Andrea
                    if (listaUtenti.Length == 0)
                    {
                        trasmissioneSingola = null;
                        throw new ExceptionTrasmissioni("Non è presente alcun utente per la Trasmissione al ruolo: "
                                                        + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                        + ".");
                    }
                    else
                    {
                        //ciclo per utenti se dest è gruppo o ruolo
                        for (int i = 0; i < listaUtenti.Length; i++)
                        {
                            DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPaWR.TrasmissioneUtente();
                            trasmissioneUtente.utente = (DocsPaWR.Utente)listaUtenti[i];
                            trasmissioneUtente.daNotificare = this.selezionaNotificaDaModello(mittDest, trasmissioneUtente.utente); //TrasmManager.getTxRuoloUtentiChecked();
                            trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                        }
                    }
                }

                if (corr is DocsPaWR.Utente)
                {
                    trasmissioneSingola.tipoDest = DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
                    DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPaWR.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPaWR.Utente)corr;
                    trasmissioneUtente.daNotificare = this.selezionaNotificaDaModello(mittDest, trasmissioneUtente.utente);

                    //Andrea
                    if (trasmissioneUtente.utente == null)
                    {
                        throw new ExceptionTrasmissioni("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                        + " è inesistente.");
                    }
                    //End Andrea
                    else
                        trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                }

                if (corr is DocsPaWR.UnitaOrganizzativa)
                {
                    DocsPaWR.UnitaOrganizzativa theUo = (DocsPaWR.UnitaOrganizzativa)corr;
                    DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = new DocsPaWR.AddressbookQueryCorrispondenteAutorizzato();
                    qca.ragione = trasmissioneSingola.ragione;
                    qca.ruolo = UserManager.GetSelectedRole();

                    //DocsPaWR.Ruolo[] ruoli = UserManager.getListaRuoliInUO(this, theUo, UserManager.getInfoUtente());
                    DocsPaWR.Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati(this, qca, theUo);

                    //Andrea
                    if (ruoli.Length == 0)
                    {
                        throw new ExceptionTrasmissioni("Manca un ruolo di riferimento per la UO: "
                                                            + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                            + ".");
                    }
                    //End Andrea
                    else
                    {
                        foreach (DocsPaWR.Ruolo r in ruoli)
                            if (r != null && r.systemId != null)
                                trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm, scadenza, mittDest);
                    }
                    return trasmissione;
                }

                if (trasmissioneSingola != null)
                    trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);


                return trasmissione;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private bool selezionaNotificaDaModello(DocsPaWR.MittDest mittDest, DocsPaWR.Utente utente)
        {
            bool retValue = false;

            try
            {
                if (!mittDest.CHA_TIPO_URP.Equals("U"))
                {
                    if (mittDest.UTENTI_NOTIFICA != null)
                    {
                        foreach (DocsPaWR.UtentiConNotificaTrasm utNot in mittDest.UTENTI_NOTIFICA)
                        {
                            if (utente.idPeople.Equals(utNot.ID_PEOPLE))
                                return Convert.ToBoolean(utNot.FLAG_NOTIFICA.Replace("1", "true").Replace("0", "false"));
                        }
                    }
                    else
                    {
                        retValue = TrasmManager.getTxRuoloUtentiChecked();
                    }
                }
                else
                {
                    retValue = TrasmManager.getTxRuoloUtentiChecked();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }

            return retValue;
        }

        private DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPaWR.Trasmissione trasmissione, DocsPaWR.Corrispondente corr, DocsPaWR.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, bool nascondiVersioniPrecedenti)
        {
            try
            {
                if (trasmissione.trasmissioniSingole != null)
                {
                    // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                    for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                    {
                        DocsPaWR.TrasmissioneSingola ts = (DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                        if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                        {
                            if (ts.daEliminare)
                            {
                                ((DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                                return trasmissione;
                            }
                            else
                                return trasmissione;
                        }
                    }
                }

                // Aggiungo la trasmissione singola
                DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPaWR.TrasmissioneSingola();
                trasmissioneSingola.tipoTrasm = tipoTrasm;
                trasmissioneSingola.corrispondenteInterno = corr;
                trasmissioneSingola.ragione = ragione;
                trasmissioneSingola.noteSingole = note;

                if (trasmissione.tipoOggetto == TrasmissioneTipoOggetto.FASCICOLO)
                    trasmissioneSingola.hideDocumentPreviousVersions = nascondiVersioniPrecedenti;
                else
                    trasmissioneSingola.hideDocumentPreviousVersions = false;

                // Imposto la data di scadenza
                if (scadenza > 0)
                {
                    System.DateTime data = System.DateTime.Now.AddDays(scadenza);
                    trasmissioneSingola.dataScadenza = Utils.dateformat.formatDataDocsPa(data);
                }

                // Aggiungo la lista di trasmissioniUtente
                if (corr is DocsPaWR.Ruolo)
                {
                    trasmissioneSingola.tipoDest = DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
                    DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr);
                    /*
                     * Andrea
                     */
                    if (listaUtenti == null || listaUtenti.Length == 0)
                    {
                        trasmissioneSingola = null;
                        throw new ExceptionTrasmissioni("Non è presente alcun utente per la Trasmissione al ruolo: "
                                                        + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                        + ".");
                    }
                    // End Andrea
                    else
                    {
                        //ciclo per utenti se dest è gruppo o ruolo
                        for (int i = 0; i < listaUtenti.Length; i++)
                        {
                            DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPaWR.TrasmissioneUtente();
                            trasmissioneUtente.utente = (DocsPaWR.Utente)listaUtenti[i];
                            trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                        }
                    }
                }

                if (corr is DocsPaWR.Utente)
                {
                    trasmissioneSingola.tipoDest = DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
                    DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPaWR.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPaWR.Utente)corr;
                    /*
                     * Andrea
                     */
                    if (trasmissioneUtente.utente == null)
                    {
                        throw new ExceptionTrasmissioni("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ") è inesistente.");
                    }
                    //End Andrea
                    else
                        trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                }

                if (corr is DocsPaWR.UnitaOrganizzativa)
                {
                    DocsPaWR.UnitaOrganizzativa theUo = (DocsPaWR.UnitaOrganizzativa)corr;
                    DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = new AddressbookQueryCorrispondenteAutorizzato();
                    qca.ragione = trasmissioneSingola.ragione;
                    qca.ruolo = UserManager.GetSelectedRole();
                    qca.queryCorrispondente = new AddressbookQueryCorrispondente();
                    qca.queryCorrispondente.fineValidita = true;

                    DocsPaWR.Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati(this, qca, theUo);
                    /*
                     * Andrea
                     */
                    if (ruoli == null || ruoli.Length == 0)
                    {
                        throw new ExceptionTrasmissioni("Manca un ruolo di riferimento per la UO: "
                                                        + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                        + ".");
                    }
                    //End Andrea
                    else
                    {
                        foreach (DocsPaWR.Ruolo r in ruoli)
                            trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm, scadenza, nascondiVersioniPrecedenti);
                    }
                    return trasmissione;
                }

                if (trasmissioneSingola != null)
                    trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);

                return trasmissione;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private DocsPaWR.Corrispondente[] queryUtenti(DocsPaWR.Corrispondente corr)
        {

            //costruzione oggetto queryCorrispondente
            DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPaWR.AddressbookQueryCorrispondente();

            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;

            qco.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
            qco.fineValidita = true;

            //corrispondenti interni
            qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;

            return UserManager.getListaCorrispondenti(this, qco);
        }

        /// <summary>
        /// Get dell'idProprietario del fascicolo
        /// </summary>
        /// <returns></returns>
        private string GetAnagUtenteProprietario()
        {
            try
            {
                FascicoloDiritto[] listaVisibilita = null;
                Fascicolo prj = ProjectManager.getProjectInSession();
                string idProprietario = string.Empty;
                InfoFascicolo infoFasc = new InfoFascicolo();
                string rootFolder = ProjectManager.GetRootFolderFasc(prj);

                listaVisibilita = ProjectManager.getListaVisibilitaSemplificata(infoFasc, true, rootFolder);

                if (listaVisibilita != null && listaVisibilita.Length > 0)
                {
                    for (int i = 0; i < listaVisibilita.Length; i++)
                    {
                        if (listaVisibilita[i].accessRights == 0)
                        {
                            return idProprietario = listaVisibilita[i].personorgroup;
                        }
                    }
                }
                return "";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private DocsPaWR.Trasmissione impostaNotificheUtentiDaModello(DocsPaWR.Trasmissione objTrasm, DocsPaWR.ModelloTrasmissione template)
        {
            if (objTrasm.trasmissioniSingole != null && objTrasm.trasmissioniSingole.Length > 0)
            {
                for (int cts = 0; cts < objTrasm.trasmissioniSingole.Length; cts++)
                {
                    if (objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length > 0)
                    {
                        for (int ctu = 0; ctu < objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length; ctu++)
                        {
                            objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].daNotificare = this.daNotificareSuModello(objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].utente.idPeople, objTrasm.trasmissioniSingole[cts].corrispondenteInterno.systemId, template);
                        }
                    }
                }
            }

            return objTrasm;
        }

        private bool daNotificareSuModello(string currentIDPeople, string currentIDCorrGlobRuolo, DocsPaWR.ModelloTrasmissione template)
        {
            bool retValue = true;

            for (int i = 0; i < template.RAGIONI_DESTINATARI.Length; i++)
            {
                DocsPaWR.RagioneDest ragDest = (DocsPaWR.RagioneDest)template.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaWR.MittDest mittDest = (DocsPaWR.MittDest)destinatari[j];
                    if (mittDest.ID_CORR_GLOBALI.Equals(Convert.ToInt32(currentIDCorrGlobRuolo)))
                    {
                        if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 0)
                        {
                            for (int cut = 0; cut < mittDest.UTENTI_NOTIFICA.Length; cut++)
                            {
                                if (mittDest.UTENTI_NOTIFICA[cut].ID_PEOPLE.Equals(currentIDPeople))
                                {
                                    if (mittDest.UTENTI_NOTIFICA[cut].FLAG_NOTIFICA.Equals("1"))
                                        retValue = true;
                                    else
                                        retValue = false;

                                    return retValue;
                                }
                            }
                        }
                    }
                }
            }
            return retValue;
        }

        private bool ViewCodeTransmissionModels
        {
            get
            {
                bool result = false;
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VISUALIZZA_CODICE_MODELLI_TRASM.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VISUALIZZA_CODICE_MODELLI_TRASM.ToString()].Equals("1")) result = false;
                return result;
            }
        }

        #endregion
        protected void projectImgRemoveFilter_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                this.SearchFilters = null;
                this.SelectedPage = 1;
                this.projectImgRemoveFilter.Enabled = false;
                Session["templateRicerca"] = null;
                SearchDocumentsAndDisplayResult(this.SearchFilters, SelectedPage, GridManager.SelectedGrid, Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
                UpContainerProjectTab.Update();
                UpnlTabHeader.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCodeCollocation_OnTextChanged(object sender, EventArgs e)
        {

            try
            {
                if (!string.IsNullOrEmpty(this.projectTxtCodiceCollocazione.Text))
                {
                    this.setDescCorrispondente(this.projectTxtCodiceCollocazione.Text, true);
                }
                else
                {
                    this.projectTxtCodiceCollocazione.Text = string.Empty;
                    this.projectTxtDescrizioneCollocazione.Text = string.Empty;
                    this.idProjectCollocation.Value = string.Empty;
                }

                this.UpProjectPhisycCollocation.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DocumentImgCollocationAddressBook_Click(object sender, EventArgs e)
        {
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA;


                HttpContext.Current.Session["AddressBook.from"] = "COLLOCATION";

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpProjectPhisycCollocation", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_PROTO_IN;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }

        }

        private void setDescCorrispondente(string codiceRubrica, bool fineValidita)
        {
            try
            {
                DocsPaWR.Corrispondente corr = null;

                if (!string.IsNullOrEmpty(codiceRubrica))
                {
                    corr = AddressBookManager.GetCorrispondenteInterno(codiceRubrica, fineValidita);
                }

                if (corr != null && !string.IsNullOrEmpty(corr.descrizione) && corr.GetType().Equals(typeof(DocsPaWR.UnitaOrganizzativa)))
                {
                    this.projectTxtDescrizioneCollocazione.Text = corr.descrizione;
                    this.projectTxtCodiceCollocazione.Text = corr.codiceRubrica;
                    this.idProjectCollocation.Value = corr.systemId;
                }
                else
                {
                    this.projectTxtDescrizioneCollocazione.Text = string.Empty;
                    this.projectTxtCodiceCollocazione.Text = string.Empty;
                    this.idProjectCollocation.Value = string.Empty;
                    string msgDesc = "WarningDocumentCorrNotFound";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void BtnRebindGrid_Click(object sender, EventArgs e)
        {

            try
            {
                //this.SelectedPage = 1;
                this.plcNavigator.Controls.Clear();
                SearchDocumentsAndDisplayResult(this.SearchFilters, SelectedPage, GridManager.SelectedGrid, Label, UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.SelectedGrid), true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnChangeSelectedFolder_Click(object sender, EventArgs e)
        {
            try
            {
                this.SelectedPage = 1;
                string folderId = this.treenode_sel.Value.Replace("node_", "").Replace("root_", "");
                if (!string.IsNullOrEmpty(folderId) && folderId.IndexOf("doc") < 0)
                {
                    Folder folder = ProjectManager.getFolder(this, folderId);
                    folder = ProjectManager.getFolder(this, folder);

                    Fascicolo project = UIManager.ProjectManager.getProjectInSession();
                    project.folderSelezionato = folder;
                    UIManager.ProjectManager.setProjectInSession(project);

                    Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                    this.BtnRebindGrid_Click(null, null);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void SelectedFolder()
        {
            try
            {
                string folderId = this.treenode_sel.Value.Replace("node_", "").Replace("root_", "");
                if (!string.IsNullOrEmpty(folderId) && folderId.IndexOf("doc") < 0)
                {
                    Folder folder = ProjectManager.getFolder(this, folderId);
                    folder = ProjectManager.getFolder(this, folder);

                    Fascicolo project = UIManager.ProjectManager.getProjectInSession();
                    project.folderSelezionato = folder;
                    UIManager.ProjectManager.setProjectInSession(project);

                    Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                    this.BtnRebindGrid_Click(null, null);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ProjectImgObjectHistory_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // In base all'immagine cliccata si apre lo storico relativo
                CustomImageButton caller = sender as CustomImageButton;

                if (caller.ID != null && !string.IsNullOrEmpty(caller.ID))
                {
                    this.TypeHistory = caller.ID;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "HistoryProject", "ajaxModalPopupHistoryProject();", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected bool GetGridPersonalization()
        {
            return this.ShowGridPersonalization;
        }

        protected void btnLinkCustom_Click(object sender, EventArgs e)
        {
            UserControls.LinkDocFasc link = (LinkDocFasc)this.PnlTypeDocument.FindControl(this.OpenAddDocCustom.ReturnValue);
            if (link == null)
            {
                link = (LinkDocFasc)this.PnlTypeDocument.FindControl(this.SearchProjectCustom.ReturnValue);
            }
            if (link != null)
            {
                if (link.IsFascicolo)
                {

                    Fascicolo fasc = ProjectManager.getFascicoloById(HttpContext.Current.Session["LinkCustom.return"].ToString());
                    if (fasc != null)
                    {
                        link.hf_Id = fasc.systemID;
                        link.txt_NomeObj = fasc.descrizione;
                        link.txt_Maschera = fasc.codice + " " + CutValue(fasc.descrizione);
                    }
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchProjectCustom','');", true);
                }
                else
                {

                    InfoDocumento infoDoc = DocumentManager.GetInfoDocumento(HttpContext.Current.Session["LinkCustom.return"].ToString(), HttpContext.Current.Session["LinkCustom.return"].ToString(), this.Page);
                    if (infoDoc != null)
                    {

                        link.hf_Id = infoDoc.idProfile;
                        link.txt_NomeObj = infoDoc.oggetto;
                        //this.txt_NomeObj.Text = infoDoc.oggetto;

                        if (!string.IsNullOrEmpty(infoDoc.segnatura))
                        {
                            link.txt_Maschera = infoDoc.segnatura + " " + CutValue(infoDoc.oggetto);
                            //this.txt_Maschera.Text = infoDoc.segnatura + " " + CutValue(infoDoc.oggetto);
                        }
                        else
                        {
                            link.txt_Maschera = infoDoc.idProfile + " " + CutValue(infoDoc.oggetto);
                        }
                    }
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenAddDocCustom','');", true);
                }
            }
            //this.hf_SelectedObject.Value = "0";

            //this.hpl_Link.Text = this.LinkText;

            HttpContext.Current.Session["LinkCustom.return"] = null;

        }

        private string CutValue(string value)
        {
            if (value.Length < 20) return value;
            int firstSpacePos = value.IndexOf(' ', 20);
            if (firstSpacePos == -1) firstSpacePos = 20;
            return value.Substring(0, firstSpacePos) + "...";
        }

        protected void CreaPredispostoInRisposta(SchedaDocumento docSelected)
        {
            // logger.Info("BEGIN");
            SchedaDocumento document = UIManager.DocumentManager.NewSchedaDocumento();
            document.predisponiProtocollazione = true;
            if (docSelected.oggetto != null)
            {
                document.oggetto = docSelected.oggetto;
            }

            if (docSelected.tipoProto.Equals("A"))
            {
                document.tipoProto = "P";
                document.protocollo = new DocsPaWR.ProtocolloUscita();
                document.registro = RoleManager.GetRoleInSession().registri[0];
                ((DocsPaWR.ProtocolloUscita)document.protocollo).destinatari = new DocsPaWR.Corrispondente[1];
                ((DocsPaWR.ProtocolloUscita)document.protocollo).destinatari[0] = new DocsPaWR.Corrispondente();
                ((DocsPaWR.ProtocolloUscita)document.protocollo).destinatari[0] = ((DocsPaWR.ProtocolloEntrata)docSelected.protocollo).mittente;
                if (EnableSenderDefault())
                {
                    DocsPaWR.Corrispondente corr = RoleManager.GetRoleInSession().uo;
                    ((DocsPaWR.ProtocolloUscita)document.protocollo).mittente = corr;
                }
            }
           
            DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(docSelected);
            document.rispostaDocumento = infoDoc;

            HttpContext.Current.Session["IsForwarded"] = true;
            this.DocumentAnswerFromProject = true;
            UIManager.DocumentManager.setSelectedRecord(document);
        }

        private bool IsRoleOutwardEnabled()
        {
            bool result = false;
            result = UIManager.UserManager.IsAuthorizedFunctions("PROTO_OUT");
            return result;
        }

        private bool EnableSenderDefault()
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.MITTENTE_DEFAULT.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.MITTENTE_DEFAULT.ToString()].Equals("1"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region varibili di sessione

        private bool DocumentAnswerFromProject
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["DocumentAnswerFromProject"] != null) result = (bool)HttpContext.Current.Session["DocumentAnswerFromProject"];
                return result;

            }
            set
            {
                HttpContext.Current.Session["DocumentAnswerFromProject"] = value;
            }
        }

        private bool EnabledCreateDocumentAnswer
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["EnabledCreateDocumentAnswer"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["EnabledCreateDocumentAnswer"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["EnabledCreateDocumentAnswer"] = value;
            }
        }

        private bool EnabledLibroFirma
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["EnabledLibroFirma"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["EnabledLibroFirma"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["EnabledLibroFirma"] = value;
            }
        }
        private bool EnableEdit
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableEdit"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableEdit"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableEdit"] = value;
            }
        }

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


        /// <summary>
        /// Posizione celle per ordinamento
        /// </summary>
        private Dictionary<string, int> CellPosition
        {
            get
            {
                return HttpContext.Current.Session["cellPosition"] as Dictionary<string, int>;
            }
            set
            {
                HttpContext.Current.Session["cellPosition"] = value;
            }

        }


        /// <summary>
        /// valore di ritorno della popup del titolario
        /// </summary>
        private string ReturnValue
        {
            get
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["ReturnValuePopup"].ToString()))
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// numero di caratteri nella nota di testo
        /// </summary>
        private int MaxLenghtNote
        {
            get
            {
                int result = 2000;
                if (HttpContext.Current.Session["MaxLenghtNote"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["MaxLenghtNote"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["MaxLenghtNote"] = value;
            }
        }


        /// <summary>
        /// numero di caratteri nella nota di testo
        /// </summary>
        private int MaxLenghtProject
        {
            get
            {
                int result = 2000;
                if (HttpContext.Current.Session["MaxLenghtProject"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["MaxLenghtProject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["MaxLenghtNote"] = value;
            }
        }


        /// <summary>
        /// auotcomplete per la ricerca della nota
        /// </summary>
        protected int AutocompleteMinimumPrefixLength
        {
            get
            {
                int result = 3;
                if (HttpContext.Current.Session["AutocompleteMinimumPrefixLength"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["AutocompleteMinimumPrefixLength"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["AutocompleteMinimumPrefixLength"] = value;
            }
        }

        private EtichettaInfo[] Label
        {
            get
            {
                return (EtichettaInfo[])HttpContext.Current.Session["Label"];

            }
            set
            {
                HttpContext.Current.Session["Label"] = value;
            }
        }

        private DataTable GrigliaResult
        {
            get
            {
                return (DataTable)HttpContext.Current.Session["GrigliaResult"];

            }
            set
            {
                HttpContext.Current.Session["GrigliaResult"] = value;
            }
        }

        //profilazione dinamica
        private DocsPaWR.Templates TemplateProject
        {
            get
            {
                Templates result = null;
                if (HttpContext.Current.Session["templateProject2"] != null)
                {
                    result = HttpContext.Current.Session["templateProject2"] as Templates;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["templateProject2"] = value;
            }
        }

        private bool CustomProject
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["customProject"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["customProject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["customProject"] = value;
            }
        }

        private bool EnableStateDiagram
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableStateDiagramProject"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableStateDiagramProject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableStateDiagramProject"] = value;
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
            set
            {
                HttpContext.Current.Session["stateDiagramProject"] = value;
            }
        }

        private bool RapidClassificationRequired
        {
            get
            {
                bool ritorno = false;
                if (HttpContext.Current.Session["RapidClassificationRequired"] != null)
                {
                    ritorno = (bool)HttpContext.Current.Session["RapidClassificationRequired"];
                }

                return ritorno;
            }
            set
            {
                HttpContext.Current.Session["RapidClassificationRequired"] = value;
            }
        }

        private bool RapidClassificationRequiredByTypeDoc
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["RapidClassificationRequiredByTypeDoc"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["RapidClassificationRequiredByTypeDoc"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["RapidClassificationRequiredByTypeDoc"] = value;
            }

        }

        private string PageCaller
        {
            set
            {
                HttpContext.Current.Session["pageCaller"] = value;
            }
        }

        private int PagesCount
        {
            get
            {
                int toReturn = 0;
                if (HttpContext.Current.Session["PagesCount"] != null) Int32.TryParse(HttpContext.Current.Session["PagesCount"].ToString(), out toReturn);
                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["PagesCount"] = value;
            }
        }

        private int RecordCount
        {
            get
            {
                int toReturn = 0;
                if (HttpContext.Current.Session["RecordNumber"] != null) Int32.TryParse(HttpContext.Current.Session["RecordNumber"].ToString(), out toReturn);
                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["RecordNumber"] = value;
            }
        }

        private int SelectedPage
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["SelectedPage"] != null) Int32.TryParse(HttpContext.Current.Session["SelectedPage"].ToString(), out toReturn);
                if (toReturn < 1) toReturn = 1;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["SelectedPage"] = value;
            }
        }

        private FiltroRicerca[][] SearchFilters
        {
            get
            {
                return (FiltroRicerca[][])HttpContext.Current.Session["FiltroRicerca"];
            }
            set
            {
                HttpContext.Current.Session["FiltroRicerca"] = value;
            }
        }

        private SearchObject[] Result
        {
            get
            {
                return HttpContext.Current.Session["Result"] as SearchObject[];
            }
            set
            {
                HttpContext.Current.Session["Result"] = value;
            }
        }

        private bool AllowDeleteDocFromProject
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["AllowDeleteDocFromProject"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["AllowDeleteDocFromProject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["AllowDeleteDocFromProject"] = value;
            }
        }

        private bool AllowConservazione
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["AllowConservazione"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["AllowConservazione"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["AllowConservazione"] = value;
            }
        }

        // INTEGRAZIONE PITRE-PARER
        // Se true, è attivo l'invio in conservazione al sistema SACER
        // Se false, è attivo l'invio in conservazione al Centro Servizi
        private bool IsConservazioneSACER
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["isConservazioneSACER"] != null)
                {
                    return (bool)HttpContext.Current.Session["isConservazioneSACER"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["isConservazioneSACER"] = value;
            }
        }

        private bool AllowADL
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["AllowADL"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["AllowADL"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["AllowADL"] = value;
            }
        }

        private List<EtichettaInfo> Labels
        {
            get
            {
                return (List<EtichettaInfo>)HttpContext.Current.Session["Labels"];

            }
            set
            {
                HttpContext.Current.Session["Labels"] = value;
            }
        }

        private bool ShowGridPersonalization
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["showGridPersonalization"] != null)
                {
                    return (bool)HttpContext.Current.Session["showGridPersonalization"];
                }
                return result;

            }
            set
            {
                HttpContext.Current.Session["showGridPersonalization"] = value;
            }
        }

        /// <summary>
        /// Number of result in page
        /// </summary>
        public int PageSize
        {
            get
            {
                int result = 20;
                if (HttpContext.Current.Session["pageSizeDocument"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["pageSizeDocument"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["pageSizeDocument"] = value;
            }
        }


        /// <summary>
        /// Number of result in page
        /// </summary>
        public bool FromClassification
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["fromClassification"] != null)
                {
                    return (bool)HttpContext.Current.Session["fromClassification"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["fromClassification"] = value;
            }
        }

        private string SelectedRow
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["selectedRow"] != null)
                {
                    result = HttpContext.Current.Session["selectedRow"] as String;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["selectedRow"] = value;
            }
        }

        private bool AllowADLRole
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["AllowADLRole"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["AllowADLRole"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["AllowADLRole"] = value;
            }
        }

        private string[] IdProfileList
        {
            get
            {
                string[] result = null;
                if (HttpContext.Current.Session["idProfileList"] != null)
                {
                    result = HttpContext.Current.Session["idProfileList"] as string[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["idProfileList"] = value;
            }
        }

        private string[] CodeProfileList
        {
            get
            {
                string[] result = null;
                if (HttpContext.Current.Session["CodeProfileList"] != null)
                {
                    result = HttpContext.Current.Session["CodeProfileList"] as string[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["CodeProfileList"] = value;
            }
        }

        protected Dictionary<String, String> ListCheck
        {
            get
            {
                Dictionary<String, String> result = null;
                if (HttpContext.Current.Session["listCheck"] != null)
                {
                    result = HttpContext.Current.Session["listCheck"] as Dictionary<String, String>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listCheck"] = value;
            }
        }

        protected bool CheckAll
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["checkAll"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["checkAll"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["checkAll"] = value;
            }
        }

        private void RemovePropertySearchCorrespondentIntExtWithDisabled()
        {
            HttpContext.Current.Session.Remove("searchCorrespondentIntExtWithDisabled");
        }

        private bool EnableViewInfoProcessesStarted
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["EnableViewInfoProcessesStarted"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["EnableViewInfoProcessesStarted"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["EnableViewInfoProcessesStarted"] = value;
            }
        }

        //protected void RepFasiDiagramma_ItemCreated(object sender, RepeaterItemEventArgs e)
        //{
        //    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //    {
        //        AssPhaseStatoDiagramma phase = e.Item.DataItem as AssPhaseStatoDiagramma;
        //        (e.Item.FindControl("faseDiagramma") as HtmlGenericControl).Attributes["style"] = "background-image: url('../Images/Common/FR_Disable.png'); float:left;width:120px;height:35px;background-repeat: no-repeat;";
        //        (e.Item.FindControl("descriptionFase") as Label).Text = phase.PHASE.DESCRIZIONE;
                
        //    }
        //}

        private void BuildFasiDiagramma(string idDiagramma, string idStatoAttuale, List<string> idStatiSuccessivi)
        {
            List<AssPhaseStatoDiagramma> phasesState = UIManager.DiagrammiManager.GetFasiStatiDiagramma(idDiagramma);
            if (phasesState != null && phasesState.Count > 0)
            {
                this.pnlFasiDiagrammaStato.Controls.Clear();
                this.pnlFasiDiagrammaStato.Visible = true;
                List<string> idPhases = phasesState.Select(x => x.PHASE.SYSTEM_ID).Distinct().ToList();
                foreach (string idPhase in idPhases)
                {
                    Phases p = (from p2 in phasesState where p2.PHASE.SYSTEM_ID.Equals(idPhase) select p2.PHASE).FirstOrDefault();
                    bool faseIniziale = (from p1 in phasesState where p1.PHASE.SYSTEM_ID.Equals(p.SYSTEM_ID) && p1.STATO.STATO_INIZIALE select p1).FirstOrDefault() != null;
                    bool faseSelezionata = (from p1 in phasesState where p1.PHASE.SYSTEM_ID.Equals(p.SYSTEM_ID) && p1.STATO.SYSTEM_ID.ToString().Equals(idStatoAttuale) select p1).FirstOrDefault() != null;
                    bool faseSuccessiva = (from p1 in phasesState where p1.PHASE.SYSTEM_ID.Equals(p.SYSTEM_ID) && idStatiSuccessivi.Contains(p1.STATO.SYSTEM_ID.ToString()) select p1).FirstOrDefault() != null;

                    HtmlGenericControl divFase = new HtmlGenericControl();
                    if (faseIniziale)
                    {
                        if (faseSelezionata)
                            divFase.Attributes["style"] = "background-image: url('../Images/Common/FR_Start_Selected.png'); float:left;width:120px;height:35px;background-repeat: no-repeat;";
                        else
                            divFase.Attributes["style"] = "background-image: url('../Images/Common/FR_Start_Disable.png'); float:left;width:120px;height:35px;background-repeat: no-repeat;";
                    }
                    else
                    {
                        if (faseSelezionata)
                            divFase.Attributes["style"] = "background-image: url('../Images/Common/FR_Selected.png'); float:left;width:120px;height:35px;background-repeat: no-repeat; position:relative; margin-left:-20px";
                        else if(faseSuccessiva)
                                divFase.Attributes["style"] = "background-image: url('../Images/Common/FR_Selected_Next.png'); float:left;width:120px;height:35px;background-repeat: no-repeat; position:relative; margin-left:-20px";
                            else
                                divFase.Attributes["style"] = "background-image: url('../Images/Common/FR_Disable.png'); float:left;width:120px;height:35px;background-repeat: no-repeat; position:relative; margin-left:-20px";
                    }

                    Label descriptionFase = new System.Web.UI.WebControls.Label();
                    descriptionFase.ID = "descriptionFase_" + p.SYSTEM_ID;
                    descriptionFase.Text = p.DESCRIZIONE;

                    HtmlGenericControl divDescriptionFase = new HtmlGenericControl();
                    divDescriptionFase.Attributes["style"] = "color:#ffffff; font-family:font-family; font-size:10px; text-align:center; line-height:35px;";
                    divDescriptionFase.Controls.Add(descriptionFase);

                    divFase.Controls.Add(divDescriptionFase);
                    this.pnlFasiDiagrammaStato.Controls.Add(divFase);
                }

                this.UpnlFasiDiagrammaStato.Update();
            }
        }
        #endregion

        private bool IsRapidClassificationRequired()
        {
            bool classificationRequired = false;
            if (this.RapidClassificationRequired)
            {
                classificationRequired = true;
            }
            if (this.RapidClassificationRequiredByTypeDoc)
            {
                SchedaDocumento doc = DocumentManager.getSelectedRecord();
                if (doc != null && !string.IsNullOrEmpty(doc.tipoProto))
                    classificationRequired = DocumentManager.IsClassificationRqueredByTypeDoc(doc.tipoProto);
            }
            return classificationRequired;
        }

        protected void ProjectBtnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                bool result = TrasmManager.AcceptMassiveTrasmFasc(ProjectManager.getProjectInSession().systemID);
                if(result)
                {
                    Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                    string accessRights = ProjectManager.GetAccessRightFascBySystemID(fascicolo.systemID);
                    fascicolo.accessRights = accessRights;
                    ProjectManager.setProjectInSession(fascicolo);
                    this.ProjectBtnAccept.Visible = ProjectManager.ExistsTrasmPendenteConWorkflowFascicolo(fascicolo.systemID, UIManager.RoleManager.GetRoleInSession().systemId, UserManager.GetUserInSession().idPeople);
                    switch ((HMdiritti)Enum.Parse(typeof(HMdiritti), fascicolo.accessRights))
                    {
                        case HMdiritti.HDdiritti_Waiting:
                            this.LblTipoDiritto.Text = Utils.Languages.GetLabelFromCode("VisibilityLabelWaiting", UserManager.GetUserLanguage());
                            break;
                        case HMdiritti.HMdiritti_Read:
                            this.LblTipoDiritto.Text = Utils.Languages.GetLabelFromCode("VisibilityLabelReadOnly", UserManager.GetUserLanguage());
                            break;
                        case HMdiritti.HMdiritti_Proprietario:
                        case HMdiritti.HMdiritti_Write:
                            this.LblTipoDiritto.Text = Utils.Languages.GetLabelFromCode("VisibilityLabelRW", UserManager.GetUserLanguage());
                            break;
                    }
                    if (!string.IsNullOrEmpty(fascicolo.accessRights) && Convert.ToInt32(fascicolo.accessRights) > Convert.ToInt32(HMdiritti.HMdiritti_Read))
                    {
                        this.projectBntChiudiFascicolo.Enabled = true;
                        if (fascicolo.stato.Equals("C"))
                            this.abilitazioneElementi(false, true);
                        else
                            this.abilitazioneElementi(true, false);
                        EnableEditMode();
                    }
                    this.UpContainer.Update();
                    this.UpDirittiFascicolo.Update();
                    this.upPnlButtons.Update();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorDocumentAccept', 'warning', '');} else {parent.ajaxDialogModal('ErrorDocumentAccept', 'warning', '');}", true);

                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorDocumentAccept', 'error', '');} else {parent.ajaxDialogModal('ErrorDocumentAccept', 'error', '');}", true);
            }
        }

        protected void ProjectBtnView_Click(object sender, EventArgs e)
        {
            try
            {
                Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                bool result = TrasmManager.ViewMassiveTrasmFasc(fascicolo.systemID);
                if (result)
                {
                    this.ProjectBtnView.Visible = false;
                    this.upPnlButtons.Update();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorDocumentView', 'warning', '');} else {parent.ajaxDialogModal('ErrorDocumentView', 'warning', '');}", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorDocumentView', 'error', '');} else {parent.ajaxDialogModal('ErrorDocumentView', 'error', '');}", true);
            }
        }

    }
}
