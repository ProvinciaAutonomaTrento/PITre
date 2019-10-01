using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDatalLibrary;

namespace NttDataWA.Management
{
    public partial class DocumentsRemoved : System.Web.UI.Page
    {
        #region Property

        private List<InfoDocumento> DocumentRemoved
        {
            get
            {
                if (HttpContext.Current.Session["DocumentsRemoved"] != null)
                    return (List<InfoDocumento>)HttpContext.Current.Session["DocumentsRemoved"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["DocumentsRemoved"] = value;
            }
        }

        private List<EtichettaInfo> Label
        {
            get
            {
                if (HttpContext.Current.Session["Label"] != null)
                    return (List<EtichettaInfo>)HttpContext.Current.Session["Label"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["Label"] = value;
            }
        }

        private string IdDocument
        {
            get
            {
                if (HttpContext.Current.Session["IdDocument"] != null)
                    return (string)HttpContext.Current.Session["IdDocument"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["IdDocument"] = value;
            }
        }

        private string Command
        {
            get
            {
                if (HttpContext.Current.Session["Command"] != null)
                    return (string)HttpContext.Current.Session["Command"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["Command"] = value;
            }
        }

        private bool IsZoom
        {
            get
            {
                if (HttpContext.Current.Session["isZoom"] != null)
                    return (bool)HttpContext.Current.Session["isZoom"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["isZoom"] = value;
            }
        }

        private FiltroRicerca[][] Filters
        {
            get
            {
                if (HttpContext.Current.Session["Filters"] != null)
                    return (FiltroRicerca[][])HttpContext.Current.Session["Filters"];
                else return null;
            }
            set
            {
                HttpContext.Current.Session["Filters"] = value;
            }
        }

        #endregion

        #region Remove property

        private void RemoveIdDocument()
        {
            HttpContext.Current.Session.Remove("IdDocument");
        }

        private void RemoveCommand()
        {
            HttpContext.Current.Session.Remove("Command");
        }

        private void RemovePropertyZoom()
        {
            HttpContext.Current.Session.Remove("isZoom");
            HttpContext.Current.Session.Remove("selectedRecord");
            FileManager.removeSelectedFile();
        }

        private void RemoveFilters()
        {
            HttpContext.Current.Session.Remove("Filters");
            HttpContext.Current.Session.Remove("selectedPageDocumentRemoved");
        }
        #endregion

        #region Const

        private const string P7M = "P7M";
        private const string REMOVE_DOCUMENT = "RemoveDocument";
        private const string RESTORE_DOCUMENT = "Restore";
        private const string VIEW_IMAGE_DOCUMENT = "ViewImageDocument";
        private const string VIEW_DETAILS_DOCUMENT = "ViewDetailsDocument";
        private const string REMOVE_ALL_DOUCUMENT = "removeAlDocument";
        private const string UP_PANEL_BUTTONS = "UpPnlButtons";
        private const string CLOSE_ZOOM = "closeZoom";
        private const string FILTER_TYPE_GRIGIO = "G";
        private const string RANGE_FILTER_TYPE_INTERVAL = "1";
        private const string RANGE_FILTER_TYPLE_SINGLE_VALUE = "0";

        #endregion

        #region Standard method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeLanguage();
                RemoveFilters();
                InitializeContent();
            }
            else
            {
                if (!string.IsNullOrEmpty(this.HiddenDocumentRemoved.Value))
                {
                    InfoDocumento infoDoc = (from info in DocumentRemoved
                                             where info.docNumber.Equals(IdDocument)
                                             select info).FirstOrDefault();
                    switch (Command)
                    {
                        case REMOVE_DOCUMENT:
                            if (DocumentManager.RemoveDocument(UserManager.GetInfoUser(), infoDoc))
                            {
                                LoadDocumentRemoved();
                                GrdDocumentsRemoved_Bind();
                                this.UpPanelGrdDocumentsRemoved.Update();
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorRemoveDocumentRemoved', 'error', '','');", true);
                            }
                            break;
                        case RESTORE_DOCUMENT:
                            if (DocumentManager.RestoreDocument(UserManager.GetInfoUser(), infoDoc))
                            {
                                LoadDocumentRemoved();
                                GrdDocumentsRemoved_Bind();
                                this.UpPanelGrdDocumentsRemoved.Update();
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorRemoveDocumentRemoved', 'error', '','');", true);
                            }
                            break;
                        case REMOVE_ALL_DOUCUMENT:
                              bool docInCestino = false;
                              if (DocumentManager.RemoveAllDocuments(out docInCestino, UserManager.GetInfoUser(), DocumentRemoved.ToArray()))
                              {
                                  if (docInCestino)
                                  {
                                      //Ci sono ancora documenti nel cestino
                                      this.BtnDocumentsRemovedSearch.Enabled = true;
                                      this.BtnDocumentsRemovedRemoveFilters.Enabled = false;
                                  }
                                  else
                                  {
                                      this.BtnDocumentsRemovedSearch.Enabled = false;
                                      this.BtnDocumentsRemovedRemoveFilters.Enabled = false;
                                      this.BtnRemoveAllDocuments.Visible = false;
                                      this.BtnExportDocumentsRemoved.Visible = false;
                                  }
                                  LoadDocumentRemoved();
                                  GrdDocumentsRemoved_Bind();
                                  this.UpPanelGrdDocumentsRemoved.Update();
                              }
                              else
                              {
                                  ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorRemoveDocumentRemoved', 'error', '','');", true);
                              }
                            break;
                    }
                    RemoveIdDocument();
                    RemoveCommand();
                    this.HiddenDocumentRemoved.Value = string.Empty;
                    return;
                }
            }
            RefreshScript();
        }

        private void InitializeContent()
        {
            LoadTypeFileAcquired();
            LoadLabel();
            if (this.Request.QueryString["back"] != null && this.Request.QueryString["back"].Equals("1"))
            {
                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject obj = navigationList.Last();
                if (!obj.CodePage.Equals(Navigation.NavigationUtils.NamePage.MANAGEMENT_DOCUMENTS_REMOVED.ToString()))
                {
                    obj = new Navigation.NavigationObject();
                    obj = navigationList.ElementAt(navigationList.Count - 2);
                }
                this.GrdDocumentsRemoved.PageIndex = Int32.Parse(obj.NumPage);
                Filters = obj.SearchFilters;
                PopolateFilterValues();
                GrdDocumentsRemoved.SelectedIndex =  Int32.Parse(obj.DxPositionElement);
            }
            LoadDocumentRemoved();
            GrdDocumentsRemoved_Bind();
            SelectedRow();
        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.BtnDocumentsRemovedRemoveFilters.Text = Utils.Languages.GetLabelFromCode("BtnDocumentsRemovedRemoveFilters", language);
            this.BtnDocumentsRemovedSearch.Text = Utils.Languages.GetLabelFromCode("BtnDocumentsRemovedSearch", language);
            this.DocumentsRemovesLbl.Text = Utils.Languages.GetLabelFromCode("DocumentsRemovesLbl", language);
            this.DocumentsRemovedLblIdDocument.Text = Utils.Languages.GetLabelFromCode("DocumentsRemovedLblIdDocument", language);
            this.ddlItemInterval.Text = Utils.Languages.GetLabelFromCode("DocumentsRemovedDdlItemInterval", language);
            this.ddlItemValueSingle.Text = Utils.Languages.GetLabelFromCode("DocumentsRemovedDdlItemValueSingle", language);
            this.DocumentsRemovedLtlDaIdDoc.Text = Utils.Languages.GetLabelFromCode("DocumentsRemovedLtlDaIdDoc", language);
            this.DocumentsRemovedLtlAIdDoc.Text = Utils.Languages.GetLabelFromCode("DocumentsRemovedLtlAIdDoc", language);
            this.ddlDateItemValueSingle.Text = Utils.Languages.GetLabelFromCode("DocumentsRemovedDdlItemValueSingle", language);
            this.ddlDateItemInterval.Text = Utils.Languages.GetLabelFromCode("DocumentsRemovedDdlItemInterval", language);
            this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("DocumentsRemovedLtlDaIdDoc", language);
            this.LtlADataCreazione.Text = Utils.Languages.GetLabelFromCode("DocumentsRemovedLtlAIdDoc", language);
            this.DocumentsRemovedLtlDataCreazione.Text = Utils.Languages.GetLabelFromCode("DocumentsRemovedLtlDataCreazione", language);
            this.DocumentsRemovedLitObject.Text = Utils.Languages.GetLabelFromCode("DocumentsRemovedLitObject", language);
            this.LtlTipoFileAcq.Text = Utils.Languages.GetLabelFromCode("DocumentsRemovedLtlTipoFileAcq", language);
            this.ddl_tipoFileAcquisiti.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language));
            this.chkFirmato.Text = Utils.Languages.GetLabelFromCode("DocumentsRemovedChkFirmato", language);
            this.chkNonFirmato.Text = Utils.Languages.GetLabelFromCode("DocumentsRemovedChkNonFirmato", language);
            this.DocumentViewer.Title = Utils.Languages.GetLabelFromCode("TitleDocumentViewerPopup", language);
            this.ExportDati.Title = Utils.Languages.GetLabelFromCode("SearchDocumentExportDatiTitle", language);
        }


        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OnlyNumbers", "OnlyNumbers();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void RetValueFromPopup()
        {
            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_PANEL_BUTTONS))
            {

                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_ZOOM)))
                {
                    RemovePropertyZoom();
                    return;
                }
            }
        }

        #endregion

        #region Event

        protected void BtnDocumentsRemovedSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Filters = GetFilters();
                LoadDocumentRemoved();
                GrdDocumentsRemoved_Bind();
                this.BtnDocumentsRemovedRemoveFilters.Enabled = true;
                this.UpPanelGrdDocumentsRemoved.Update();
                this.UpPnlButtons.Update();
                this.UpPnlFilter.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnDocumentsRemovedRemoveFilters_Click(object sender, EventArgs e)
        {
            try
            {
                RemoveFilters();
                CleanFieldsFilters();
                LoadDocumentRemoved();
                GrdDocumentsRemoved_Bind();
                this.BtnDocumentsRemovedSearch.Enabled = true;
                this.BtnDocumentsRemovedRemoveFilters.Enabled = false;
                this.UpPanelGrdDocumentsRemoved.Update();
                this.UpPnlFilter.Update();
                this.UpPnlButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnRemoveAllDocuments_Click(object sender, ImageClickEventArgs e)
        {
            if (!UserManager.ruoloIsAutorized(this, "SVUOTA_CESTINO"))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningRemoveDocumentRemoved', 'warning', '','');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('ConfirmRemoveAllDocumentRemoved', 'HiddenDocumentRemoved', '');", true);
                Command = REMOVE_ALL_DOUCUMENT;
            }
        }

        protected void ddl_idDocumento_C_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (this.ddl_idDocumento_C.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_fineIdDoc_C.Visible = false;
                        this.DocumentsRemovedLtlAIdDoc.Visible = false;
                        this.DocumentsRemovedLtlDaIdDoc.Visible = false;
                        this.txt_fineIdDoc_C.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_fineIdDoc_C.Visible = true;
                        this.DocumentsRemovedLtlAIdDoc.Visible = true;
                        this.DocumentsRemovedLtlDaIdDoc.Visible = true;
                        this.txt_fineIdDoc_C.Text = string.Empty;
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dataCreazione_E_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (this.ddl_dataCreazione_E.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_finedataCreazione_E.Visible = false;
                        this.LtlDaDataCreazione.Visible = false;
                        this.LtlADataCreazione.Visible = false;
                        this.txt_finedataCreazione_E.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_finedataCreazione_E.Visible = true;
                        this.LtlDaDataCreazione.Visible = true;
                        this.LtlADataCreazione.Visible = true;
                        this.txt_finedataCreazione_E.Text = string.Empty;
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
        #endregion

        #region Management Grid

        private void GrdDocumentsRemoved_Bind()
        {
            this.GrdDocumentsRemoved.DataSource = DocumentRemoved;
            this.GrdDocumentsRemoved.DataBind();
        }

        protected void GrdDocumentsRemoved_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // gestione della icona dei dettagli
                    InfoDocumento info = e.Row.DataItem as InfoDocumento;
                    CustomImageButton buttonTypeDoc = e.Row.FindControl("DocumentsRemovedViewDocument") as CustomImageButton;
                    if (info.acquisitaImmagine == "0")
                    {
                        buttonTypeDoc.ImageUrl = "../Images/Icons/ico_no_file.png";
                        buttonTypeDoc.OnMouseOverImage = "../Images/Icons/ico_no_file.png";
                        buttonTypeDoc.OnMouseOutImage = "../Images/Icons/ico_no_file.png";
                        buttonTypeDoc.ImageUrlDisabled = "../Images/Icons/ico_no_file.png";
                        buttonTypeDoc.CssClass = "";
                        buttonTypeDoc.Enabled = false;
                    }
                    else
                    {
                        string url = ResolveUrl(FileManager.getFileIcon(this.Page, info.acquisitaImmagine.Replace(".", "")));
                        buttonTypeDoc.ImageUrl = url;
                        buttonTypeDoc.OnMouseOverImage = url;
                        buttonTypeDoc.OnMouseOutImage = url;
                        buttonTypeDoc.ImageUrlDisabled = url;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GrdDocumentsRemoved_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.GrdDocumentsRemoved.PageIndex = e.NewPageIndex;
                this.grid_rowindex.Value = "0";
                GrdDocumentsRemoved_Bind();
                this.UpPanelGrdDocumentsRemoved.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GrdDocumentsRemoved_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            Command = e.CommandName;
            
            switch (e.CommandName)
            {
                case REMOVE_DOCUMENT:
                    IdDocument = (((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).FindControl("idDocument") as Label).Text;
                    if (!UserManager.ruoloIsAutorized(this, "SVUOTA_CESTINO"))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningRemoveDocumentRemoved', 'warning', '','');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('ConfirmRemoveDocumentRemoved', 'HiddenDocumentRemoved', '', '" + IdDocument + "');", true);
                    }
                    break;
                case VIEW_DETAILS_DOCUMENT:
                    string idDoc = (((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).FindControl("idDocument") as Label).Text;
                    SchedaDocumento schedaDoc = UIManager.DocumentManager.getDocumentDetails(this.Page, idDoc, idDoc);
                    if (schedaDoc == null)
                    {
                        string msgDesc = "CheckSmistaDocAcl";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                    else
                    {
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                        actualPage.IdObject = schedaDoc.systemId;
                        actualPage.DxPositionElement = ((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).RowIndex.ToString();
                        actualPage.SearchFilters = Filters;
                        actualPage.NumPage = this.GrdDocumentsRemoved.PageIndex.ToString();
                        actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_DOCUMENTS_REMOVED.ToString(), string.Empty);
                        actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.MANAGEMENT_DOCUMENTS_REMOVED.ToString(), true, this.Page);
                        actualPage.CodePage = Navigation.NavigationUtils.NamePage.MANAGEMENT_DOCUMENTS_REMOVED.ToString();
                        actualPage.Page = "DOCUMENTSREMOVED.ASPX";
                        actualPage.ViewResult = true;
                        navigationList.Add(actualPage);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);

                        DocumentManager.setSelectedRecord(schedaDoc);
                        Response.Redirect("../Document/Document.aspx");
                        Response.End();
                    }
                    break;
                case VIEW_IMAGE_DOCUMENT:
                    this.IsZoom = true;
                    string idDocument = (((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).FindControl("idDocument") as Label).Text;

                    SchedaDocumento schedaDoc2 = UIManager.DocumentManager.getDocumentDetails(this.Page, idDocument, idDocument);
                    if (schedaDoc2 == null)
                    {
                        string msgDesc = "CheckSmistaDocAcl";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                    else
                    {
                        UIManager.DocumentManager.setSelectedRecord(UIManager.DocumentManager.getDocumentDetails(this.Page, idDocument, idDocument));
                        FileManager.setSelectedFile(UIManager.DocumentManager.getSelectedRecord().documenti[0]);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDocumentViewer", "ajaxModalPopupDocumentViewer();", true);
                        NttDataWA.Popup.DocumentViewer.OpenDocumentViewer = true;
                    }
                    break;
                case RESTORE_DOCUMENT:
                    IdDocument = (((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).FindControl("idDocument") as Label).Text;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('ConfirmRestoreDocumentRemoved', 'HiddenDocumentRemoved', '');", true);
                    break;
            }
        }

        #endregion

        #region Management filters

        private void CleanFieldsFilters()
        {
            //Pulisco campi per filtro id documento
            this.ddl_idDocumento_C.SelectedValue = RANGE_FILTER_TYPLE_SINGLE_VALUE;
            this.txt_initIdDoc_C.Text = string.Empty;
            this.txt_fineIdDoc_C.Text = string.Empty;
            this.DocumentsRemovedLtlDaIdDoc.Visible = false;
            this.DocumentsRemovedLtlAIdDoc.Visible = false;
            this.txt_fineIdDoc_C.Visible = false;
            //Pulisco campi per filtro data creazione
            this.ddl_dataCreazione_E.SelectedValue = RANGE_FILTER_TYPLE_SINGLE_VALUE;
            this.txt_initDataCreazione_E.Text = string.Empty;
            this.txt_finedataCreazione_E.Text = string.Empty;
            this.LtlDaDataCreazione.Visible = false;
            this.LtlADataCreazione.Visible = false;
            this.txt_finedataCreazione_E.Visible = false;
            //pulisco campo oggetto
            this.TxtObject.Text = string.Empty;
            //Pulisco campo tipo file acquisito
            this.ddl_tipoFileAcquisiti.SelectedValue = string.Empty;
            this.chkFirmato.Checked = false;
            this.chkNonFirmato.Checked = false;

        }

        /// <summary>
        /// Creazione oggetti filtro
        /// </summary>
        /// <returns></returns>
        private FiltroRicerca[][] GetFilters()
        {
            ArrayList filterItems = new ArrayList();
            this.AddFilterTipoDocumento(filterItems);
            this.AddFilterIDDocumento(filterItems);
            this.AddFilterDataCreazioneDocumento(filterItems);
            this.AddFilterOggettoDocumento(filterItems);
            this.AddFilterFileFirmato(filterItems);
            this.AddFilterTipoFileAcquisito(filterItems);

            DocsPaWR.FiltroRicerca[] initArray = new FiltroRicerca[filterItems.Count];
            filterItems.CopyTo(initArray);
            filterItems = null;

            DocsPaWR.FiltroRicerca[][] retValue = new FiltroRicerca[1][];
            retValue[0] = initArray;
            return retValue;
        }

        /// <summary>
        /// Creazione oggetti di filtro per oggetto documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterTipoDocumento(ArrayList filterItems)
        {
            DocsPaWR.FiltroRicerca filterItem = new FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
            filterItem.valore = FILTER_TYPE_GRIGIO;
            filterItems.Add(filterItem);
            filterItem = null;
        }

        /// <summary>
        /// Creazione oggetti di filtro per data creazione documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterDataCreazioneDocumento(ArrayList filterItems)
        {
            bool rangeFilterInterval = (this.ddl_dataCreazione_E.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);

            DocsPaWR.FiltroRicerca filterItem = null;

            if (this.txt_initDataCreazione_E.Text.Length > 0)
            {
                filterItem = new FiltroRicerca();

                if (rangeFilterInterval)
                    filterItem.argomento = FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                else
                    filterItem.argomento = FiltriDocumento.DATA_CREAZIONE_IL.ToString();

                filterItem.valore = this.txt_initDataCreazione_E.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }

            if (rangeFilterInterval && this.txt_finedataCreazione_E.Text.Length > 0)
            {
                filterItem = new FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                filterItem.valore = this.txt_finedataCreazione_E.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }
            /// <summary>
        /// Creazione oggetti di filtro per id documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterIDDocumento(ArrayList filterItems)
        {
            bool rangeFilterInterval = (this.ddl_idDocumento_C.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);

            DocsPaWR.FiltroRicerca filterItem = null;

            if (this.txt_initIdDoc_C.Text.Length > 0)
            {
                filterItem = new FiltroRicerca();

                if (rangeFilterInterval)
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();

                filterItem.valore = this.txt_initIdDoc_C.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }

            if (rangeFilterInterval && this.txt_fineIdDoc_C.Text.Length > 0)
            {
                filterItem = new FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
                filterItem.valore = this.txt_fineIdDoc_C.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per oggetto documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterOggettoDocumento(ArrayList filterItems)
        {
            if (this.TxtObject.Text.Length > 0)
            {
                FiltroRicerca filterItem = new FiltroRicerca();
                filterItem.argomento = FiltriDocumento.OGGETTO.ToString();
                filterItem.valore = this.TxtObject.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        private void AddFilterFileFirmato(ArrayList filterItems)
        {
            if (this.chkFirmato.Checked)
            {
                //cerco documenti firmati
                if (!this.chkNonFirmato.Checked)
                {
                    DocsPaWR.FiltroRicerca filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                    filterItem.valore = "1";
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
                else
                {//cerco documenti che abbiano un file acquisito, sia esso firmato o meno.
                    DocsPaWR.FiltroRicerca filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                    filterItem.valore = "2";
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
            }
            else
            {
                //cerco i documenti non firmati
                if (this.chkNonFirmato.Checked)
                {
                    DocsPaWR.FiltroRicerca filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                    filterItem.valore = "0";
                    filterItems.Add(filterItem);
                    filterItem = null;
                }

            }

        }

        private void AddFilterTipoFileAcquisito(ArrayList filterItems)
        {
            if (this.ddl_tipoFileAcquisiti.SelectedIndex > 0)
            {
                DocsPaWR.FiltroRicerca filterItem = new FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO_FILE_ACQUISITO.ToString();
                filterItem.valore = this.ddl_tipoFileAcquisiti.SelectedItem.Value;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        #endregion

        #region Utils

        private void LoadTypeFileAcquired()
        {
            ArrayList tipoFile = new ArrayList();
            tipoFile = DocumentManager.getExtFileAcquisiti(this);
            for (int i = 0; i < tipoFile.Count; i++)
            {
                if (!tipoFile[i].ToString().Contains(P7M))
                {
                    ListItem item = new ListItem(tipoFile[i].ToString());
                    this.ddl_tipoFileAcquisiti.Items.Add(item);
                }
            }
        }

        private void LoadDocumentRemoved()
        {
            InfoDocumento[] docRemoved = DocumentManager.getDocInCestino(UserManager.GetInfoUser(), Filters);
            DocumentRemoved = (docRemoved != null && docRemoved.Count() > 0) ? docRemoved.ToList() : new List<InfoDocumento>();
            if (DocumentRemoved == null || DocumentRemoved.Count == 0)
            {
                this.BtnDocumentsRemovedRemoveFilters.Enabled = false;
                this.BtnRemoveAllDocuments.Visible = false;
                this.BtnExportDocumentsRemoved.Visible = false;
            }
            else
            {
                this.BtnRemoveAllDocuments.Visible = true;
                this.BtnExportDocumentsRemoved.Visible = true;
            }
            LoadNumberDocumentLabel();
            this.UpPanelNumberDocumentsRemoved.Update();
            this.UpPanelBtnTop.Update();
        }

        private void LoadLabel()
        {
            Label = DocumentManager.GetLettereProtocolli();
        }

        private void LoadNumberDocumentLabel()
        {
            if (this.DocumentRemoved.Count > 0)
            {
                this.lblNumberDocumentsRemoved.Text = Utils.Languages.GetLabelFromCode("lblNumberDocumentsRemoved", UserManager.GetUserLanguage()).Replace("@@", DocumentRemoved.Count.ToString());
            }
            else
            {
                this.lblNumberDocumentsRemoved.Text = Utils.Languages.GetLabelFromCode("lblNoDocumentsRemoved", UserManager.GetUserLanguage());
            }

        }

        private void PopolateFilterValues()
        {
            if(Filters != null && Filters.Count() > 0)
            {
                foreach(DocsPaWR.FiltroRicerca item in Filters[0])
                {
                    #region DOCNUMBER
                    if (item.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER.ToString())
                    {
                        if (this.ddl_idDocumento_C.SelectedIndex != 0)
                            this.ddl_idDocumento_C.SelectedIndex = 0;
                        this.ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initIdDoc_C.Text = item.valore;
                    }
                    #endregion DOCNUMBER
                    #region DOCNUMBER_DAL
                    else if (item.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString())
                    {
                        if (this.ddl_idDocumento_C.SelectedIndex != 1)
                            this.ddl_idDocumento_C.SelectedIndex = 1;
                        this.ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initIdDoc_C.Text = item.valore;
                    }
                    #endregion DOCNUMBER_DAL
                    #region DOCNUMBER_AL
                    else if (item.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString())
                    {
                        if (this.ddl_idDocumento_C.SelectedIndex != 1)
                            this.ddl_idDocumento_C.SelectedIndex = 1;
                        this.ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_fineIdDoc_C.Text = item.valore;
                    }
                    #endregion DOCNUMBER_AL
                    #region DATA_CREAZIONE_IL
                    else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString())
                    {
                        if (ddl_dataCreazione_E.SelectedIndex != 0)
                            ddl_dataCreazione_E.SelectedIndex = 0;
                        ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initDataCreazione_E.Visible = true;
                        this.txt_initDataCreazione_E.Visible = true;
                        this.txt_initDataCreazione_E.Text = item.valore;
                    }
                    #endregion DATA_CREAZIONE_IL
                    #region DATA_CREAZIONE_SUCCESSIVA_AL
                    else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString())
                    {
                        if (ddl_dataCreazione_E.SelectedIndex != 1)
                            ddl_dataCreazione_E.SelectedIndex = 1;
                        ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initDataCreazione_E.Visible = true;
                        this.txt_initDataCreazione_E.Visible = true;
                        this.txt_initDataCreazione_E.Text = item.valore;
                    }
                    #endregion DATA_CREAZIONE_SUCCESSIVA_AL
                    #region DATA_CREAZIONE_PRECEDENTE_IL
                    else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString())
                    {
                        if (this.ddl_dataCreazione_E.SelectedIndex != 1)
                            this.ddl_dataCreazione_E.SelectedIndex = 1;
                        this.ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_finedataCreazione_E.Visible = true;
                        this.txt_finedataCreazione_E.Visible = true;
                        this.txt_finedataCreazione_E.Text = item.valore;
                    }
                    #endregion DATA_CREAZIONE_PRECEDENTE_IL
                    #region OGGETTO
                    else if (item.argomento == DocsPaWR.FiltriDocumento.OGGETTO.ToString())
                    {
                        this.TxtObject.Text = item.valore;
                    }
                    #endregion OGGETTO
                    #region TIPO_FILE_ACQUISITO
                    else if (item.argomento == DocsPaWR.FiltriDocumento.TIPO_FILE_ACQUISITO.ToString())
                    {
                        this.ddl_tipoFileAcquisiti.SelectedValue = item.valore;
                    }
                    #endregion
                    #region FIRMATO
                    else if (item.argomento == DocsPaWR.FiltriDocumento.FIRMATO.ToString())
                    {
                        switch (item.valore)
                        { 
                            case "0" :
                                this.chkNonFirmato.Checked = true;
                                break;
                            case "1":
                                this.chkFirmato.Checked = true;
                                break;
                            case "2":
                                this.chkFirmato.Checked = true;
                                this.chkNonFirmato.Checked = true;
                                break;
                        }
                    }
                    #endregion
                }
                this.BtnDocumentsRemovedRemoveFilters.Enabled = true;
            }
        }

        private void SelectedRow()
        {
            foreach (GridViewRow GVR in this.GrdDocumentsRemoved.Rows)
            {
                if (GVR.RowIndex == this.GrdDocumentsRemoved.SelectedIndex)
                {
                    GVR.CssClass += " selectedrow";
                    break;
                }
            }
        }
        #endregion
    }
}