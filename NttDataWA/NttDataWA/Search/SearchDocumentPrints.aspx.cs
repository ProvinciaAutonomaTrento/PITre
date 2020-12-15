using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDatalLibrary;
using System.Text;
using NttDataWA.Utils;
using System.Collections;

namespace NttDataWA.Search
{
    public partial class SearchDocumentPrints : System.Web.UI.Page
    {
        private const string UP_DOCUMENT_BUTTONS = "upPnlButtons";
        private const string CLOSE_POPUP_ZOOM = "closeZoom";
        public SearchManager schedaRicerca = null;
        private const string KEY_SCHEDA_RICERCA = "RicercaStampa";
        private const string RANGE_FILTER_TYPE_INTERVAL = "1";
        private const string RANGE_FILTER_TYPE_SINGLE = "0";

        public static string componentType = Constans.TYPE_SMARTCLIENT;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    this.InitializePage();

                    //Back
                    if (this.Request.QueryString["back"] != null && this.Request.QueryString["back"].Equals("1"))
                    {
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject obj = navigationList.Last();
                        if (!obj.CodePage.Equals(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_PRINTS.ToString()))
                        {
                            obj = new Navigation.NavigationObject();
                            obj = navigationList.ElementAt(navigationList.Count - 2);
                        }
                        schedaRicerca.FiltriRicerca = obj.SearchFilters;
                        this.SearchFilters = obj.SearchFilters;
                        if (!string.IsNullOrEmpty(obj.NumPage))
                        {
                            this.SelectedPage = Int32.Parse(obj.NumPage);
                        }

                        this.BindFilterValues(schedaRicerca, this.ShowGridPersonalization);
                        DocumentManager.setFiltroRicFasc(this, schedaRicerca.FiltriRicerca);

                        this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());

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
                    }
                }

                else
                {
                    if (this.Result != null && this.Result.Length > 0)
                    {
                        // Visualizzazione dei risultati
                        this.SetCheckBox();

                        // Lista dei documenti risultato della ricerca
                        if (!this.SelectedPage.ToString().Equals(this.grid_pageindex.Value))
                        {
                            this.Result = null;
                            this.SelectedRow = string.Empty;
                            if (!string.IsNullOrEmpty(this.grid_pageindex.Value))
                            {
                                this.SelectedPage = int.Parse(this.grid_pageindex.Value);
                            }
                            this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                            this.BuildGridNavigator();
                            this.UpnlNumerodocumenti.Update();
                            this.UpnlGrid.Update();
                            this.upPnlGridIndexes.Update();
                        }
                        else
                        {
                            this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                        }
                    }
                    this.ReadRetValueFromPopup();
                }


                //rimuovo IsZoom alla chiusura della popup di zoom(da x o pulsante chiudi) quando richiamata da profilo, allegato, classifica
                if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_DOCUMENT_BUTTONS))
                {
                    if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_ZOOM)))
                    {
                        RemoveIsZoom();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DocumentViewer','');", true);
                        return;
                    }
                }

                if (this.ShowGridPersonalization)
                {
                    this.EnableDisableSave();
                }

                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void RblTypePrints_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
               

                if (!string.IsNullOrEmpty(this.RblTypePrints.SelectedValue))
                {
                    if (this.RblTypePrints.SelectedValue.Equals("0"))
                    {
                        this.PnlPrintRegistry.Visible = true;
                        this.PnlPrintRepertories.Visible = false;
                    }
                    else
                    {
                        this.PnlPrintRegistry.Visible = false;
                        this.PnlPrintRepertories.Visible = true;
                        this.setListaRepertori();
                    }

                    this.ClearSessionProperties();
                    this.SearchDocumentRemoveFilters_Click(null, null);
                    this.ListCheck = new Dictionary<string, string>();
                    this.ShowGrid(GridManager.SelectedGrid, null, 0, 0, this.Labels.ToArray<EtichettaInfo>());

                    this.UpPnlPrintRegistry.Update();

                }


                this.UpPnlSelectPrintType.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.GrigliaPersonalizzata.ReturnValue))
            {
                this.UpContainerProjectTab.Update();
                this.UpnlTabHeader.Update();
                // Se ci sono risultati, vengono visualizzati
                if (this.Result != null && this.Result.Length > 0)
                {
                    this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                    this.SearchDocumentDdlMassiveOperation.Enabled = true;
                }
                else
                {
                    this.ShowGrid(GridManager.SelectedGrid, null, 0, 0, this.Labels.ToArray<EtichettaInfo>());
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('GrigliaPersonalizzata','');", true);
            }

            if (!string.IsNullOrEmpty(this.GrigliaPersonalizzataSave.ReturnValue))
            {
                this.UpContainerProjectTab.Update();
                this.UpnlGrid.Update();
                this.UpnlTabHeader.Update();
                // Se ci sono risultati, vengono visualizzati
                if (this.Result != null && this.Result.Length > 0)
                {
                    this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                    this.SearchDocumentDdlMassiveOperation.Enabled = true;
                }
                else
                {
                    this.ShowGrid(GridManager.SelectedGrid, null, 0, 0, this.Labels.ToArray<EtichettaInfo>());
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('GrigliaPersonalizzataSave','');", true);

                string msg = "InfoSaveGrid";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info');} else {parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info');}  ", true);
            }

            if (!string.IsNullOrEmpty(this.GridPersonalizationPreferred.ReturnValue))
            {
                this.UpContainerProjectTab.Update();
                this.UpnlGrid.Update();
                this.UpnlTabHeader.Update();
                // Se ci sono risultati, vengono visualizzati
                if (this.Result != null && this.Result.Length > 0)
                {
                    this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                    this.SearchDocumentDdlMassiveOperation.Enabled = true;
                }
                else
                {
                    this.ShowGrid(GridManager.SelectedGrid, null, 0, 0, this.Labels.ToArray<EtichettaInfo>());
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('GridPersonalizationPreferred','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveAddAdlUser.ReturnValue))
            {
                if (this.MassiveAddAdlUser.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
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
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
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
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
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
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
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
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
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
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
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
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
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
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
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
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
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
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
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
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
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
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
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
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
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
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                    MassiveOperationUtils.ItemsStatus = null;
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveDigitalSignature','');", true);
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

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OnlyNumbers", "OnlyNumbers();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CombineRowsHover", "CombineRowsHover();", true);
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.LitSearchProject.Text = Utils.Languages.GetLabelFromCode("SearchDocumentPrintsTitle", language);
            this.SearchDocumentPrintsSearch.Text = Utils.Languages.GetLabelFromCode("SearchLabelButton", language);
            this.SearchDocumentRemoveFilters.Text = Utils.Languages.GetLabelFromCode("SearchLabelRemoveFiltersButton", language);
            this.SearchDocumentDdlMassiveOperation.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("projectDdlAzioniMassive", language));
            this.projectImgEditGrid.ToolTip = Utils.Languages.GetLabelFromCode("projectImgEditGrid", language);
            this.projectImgPreferredGrids.ToolTip = Utils.Languages.GetLabelFromCode("projectImgPreferredGrids", language);
            this.projectImgSaveGrid.ToolTip = Utils.Languages.GetLabelFromCode("projectImgSaveGrid", language);
            this.projectImgEditGrid.AlternateText = Utils.Languages.GetLabelFromCode("projectImgEditGrid", language);
            this.projectImgPreferredGrids.AlternateText = Utils.Languages.GetLabelFromCode("projectImgPreferredGrids", language);
            this.projectImgSaveGrid.AlternateText = Utils.Languages.GetLabelFromCode("projectImgSaveGrid", language);
            this.SearchDocumentYear.Text = Utils.Languages.GetLabelFromCode("SearchDocumentYear", language);
            this.LtlNumProto.Text = Utils.Languages.GetLabelFromCode("LtlNumProto", language);
            this.cboFilterTypeNumProtocollo.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E0", language);
            this.cboFilterTypeNumProtocollo.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E1", language);
            this.LtlDaNumProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
            this.LtlANumProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlRegistro.Text = Utils.Languages.GetLabelFromCode("LtlRegistro", language);
            this.LtlDataArrivo.Text = Utils.Languages.GetLabelFromCode("LtlDataStampa", language);
            this.cboFilterTypeDataStampa.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.cboFilterTypeDataStampa.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.LtlDaDataArrivo.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.projectLitNomeGriglia.Text = Utils.Languages.GetLabelFromCode("projectLitNomeGriglia", language);
            this.GridPersonalizationPreferred.Title = Utils.Languages.GetLabelFromCode("projectImgPreferredGrids", language);
            this.GrigliaPersonalizzata.Title = Utils.Languages.GetLabelFromCode("projectImgEditGrid", language);
            this.GrigliaPersonalizzataSave.Title = Utils.Languages.GetLabelFromCode("projectImgSaveGrid", language);
            this.RblTypePrints.Items[0].Text = Utils.Languages.GetLabelFromCode("RblTypePrintsRegistry", language);
            this.RblTypePrints.Items[1].Text = Utils.Languages.GetLabelFromCode("RblTypePrintsRepertory", language);
            this.LitSearchRepertoriesAoo.Text = Utils.Languages.GetLabelFromCode("LitSearchRepertoriesAoo", language);
            this.LitSearchRepertories.Text = Utils.Languages.GetLabelFromCode("LitSearchRepertories", language);
            this.ddl_repertori.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SearchDocumentPrintsDDL_Repertory", language));
            this.ddl_aoo_rf.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SearchDocumentPrintsDDL_AOO", language));
            this.SearchDocumentPrintsLitNum.Text = Utils.Languages.GetLabelFromCode("SearchDocumentPrintsLitNum", language);
            this.LtlDaNumRep.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
            this.LtlANumRep.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.Ddl_num_rep.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E0", language);
            this.Ddl_num_rep.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E1", language);
            this.SearchDocumentPrintsLitYearRep.Text = Utils.Languages.GetLabelFromCode("SearchDocumentYear", language);
            this.SearchDocumentPrintsLitTypePrints.Text = Utils.Languages.GetLabelFromCode("SearchDocumentPrintsLitTypePrints", language);
            this.rbl_TipiStampe.Items[0].Text = Utils.Languages.GetLabelFromCode("rbl_TipiStampe0", language);
            this.rbl_TipiStampe.Items[1].Text = Utils.Languages.GetLabelFromCode("rbl_TipiStampe1", language);
            this.rbl_TipiStampe.Items[2].Text = Utils.Languages.GetLabelFromCode("rbl_TipiStampe2", language);
            this.SearchDocumentPrintsLitIdDoc.Text = Utils.Languages.GetLabelFromCode("LtlIdDoc", language);
            this.ddl_idDocumento_C.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_idDocumento_C0", language);
            this.ddl_idDocumento_C.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_idDocumento_C1", language);
            this.LtlDaIdDoc.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
            this.LtlAIdDoc.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);

            this.LtlStatoConservazione.Text = Utils.Languages.GetLabelFromCode("LtlStatoCons", language);
            this.LtlDataVers.Text = Utils.Languages.GetLabelFromCode("LtlDataVers", language);
            this.LtlDaDataVers.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.LtlPolicy.Text = Utils.Languages.GetLabelFromCode("LtlPolicy", language);
            this.LtlCodPolicy.Text = Utils.Languages.GetLabelFromCode("LtlCodPolicy", language);
            this.LtlCounterPolicy.Text = Utils.Languages.GetLabelFromCode("LtlCounterPolicy", language);
            this.ltlDatePolicy.Text = Utils.Languages.GetLabelFromCode("LtlDatePolicy", language);
            this.LtlDaDatePolicy.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);

            this.optConsNC.Text = Utils.Languages.GetLabelFromCode("optConsNC", language);
            this.optConsAtt.Text = Utils.Languages.GetLabelFromCode("optConsAtt", language);
            this.optConsVer.Text = Utils.Languages.GetLabelFromCode("optConsVer", language);
            this.optConsPre.Text = Utils.Languages.GetLabelFromCode("optConsPre", language);
            this.optConsRif.Text = Utils.Languages.GetLabelFromCode("optConsRif", language);
            this.optConsErr.Text = Utils.Languages.GetLabelFromCode("optConsErr", language);
            this.optConsTim.Text = Utils.Languages.GetLabelFromCode("optConsTim", language);
            this.optConsFld.Text = Utils.Languages.GetLabelFromCode("optConsFld", language);
            this.ddl_DataVers.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_DataVers.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_DataVers.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_DataVers.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_DataVers.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.ddl_datePolicy.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_datePolicy.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_datePolicy.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_datePolicy.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_datePolicy.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);

            this.MassiveReport.Title = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.MassiveAddAdlUser.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveAddAdlUserTitle", language);
            this.MassiveRemoveAdlUser.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveAdlUserTitle", language);
            this.MassiveAddAdlRole.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveAddAdlRoleTitle", language);
            this.MassiveRemoveAdlRole.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveAdlRoleTitle", language);
            this.MassiveConservation.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConservationTitle", language);
            this.MassiveTransmission.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveTransmissionTitle", language);
            this.MassiveConversion.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConversionTitle", language);
            this.MassiveTimestamp.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveTimestampTitle", language);
            this.MassiveConsolidation.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConsolidationTitle", language);
            this.MassiveConsolidationMetadati.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConsolidationMetadatiTitle", language);
            this.MassiveForward.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveForwardTitle", language);
            this.MassiveCollate.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveCollateTitle", language);
            this.MassiveRemoveVersions.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveVersionsTitle", language);
            this.MassiveDigitalSignature.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveDigitalSignatureTitle", language);
            this.MassiveDigitalSignatureApplet.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveDigitalSignatureTitle", language);
            this.MassiveDigitalSignatureSocket.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveDigitalSignatureTitle", language);
            this.MassiveVersPARER.Title = Utils.Languages.GetLabelFromCode("MassiveVersTitle", language);
            this.ExportDati.Title = Utils.Languages.GetLabelFromCode("SearchDocumentExportDatiTitle", language);
            this.OpenTitolarioMassive.Title = Utils.Languages.GetLabelFromCode("TitleClassificationScheme", language);
            this.DigitalSignDetails.Title = Utils.Languages.GetLabelFromCode("DigitalSignDetailsTitle", language);
            this.SearchProjectMassive.Title = Utils.Languages.GetLabelFromCode("SearchProjectTitle", language);
            this.DocumentViewer.Title = Utils.Languages.GetLabelFromCode("TitleDocumentViewerPopup", language);
        
        }

        protected void SearchDocumentRemoveFilters_Click(object sender, EventArgs e)
        {
            try
            {
                this.TxtYear.Text = string.Empty;
                this.TxtYearRep.Text = string.Empty;
                this.cboFilterTypeNumProtocollo.SelectedIndex = 0;
                this.txtInitNumProtocollo.Text = string.Empty;
                this.txtEndNumProtocollo.Text = string.Empty;
                this.txtInitNumRep.Text = string.Empty;
                this.txtEndNumRep.Text = string.Empty;
                this.lb_reg_C.SelectedIndex = 0;
                this.rbl_Reg_C.SelectedIndex = 0;
                this.cboFilterTypeDataStampa.SelectedIndex = 0;
                this.txtInitDataStampa.Text = string.Empty;
                this.txtEndDataStampa.Text = string.Empty;
                this.ddl_DataVers.SelectedIndex = 0;
                this.cbl_Conservazione.Items[0].Selected = true;
                this.cbl_Conservazione.Items[1].Selected = true;
                this.cbl_Conservazione.Items[2].Selected = true;
                this.cbl_Conservazione.Items[3].Selected = true;
                this.cbl_Conservazione.Items[4].Selected = true;
                this.cbl_Conservazione.Items[5].Selected = true;
                this.cbl_Conservazione.Items[6].Selected = true;
                this.cbl_Conservazione.Items[7].Selected = true;
                this.txt_initDataVers.Text = string.Empty;
                this.txt_initDataVers.Text = string.Empty;
                this.ddl_datePolicy.SelectedIndex = 0;
                this.txtCodPolicy.Text = string.Empty;
                this.txtCounterPolicy.Text = string.Empty;
                this.txt_initDatePolicy.Text = string.Empty;
                this.txt_fineDatePolicy.Text = string.Empty;
                this.ddl_idDocumento_C.SelectedIndex = 0;
                this.txt_initIdDoc_C.Text = string.Empty;
                this.txt_fineIdDoc_C.Text = string.Empty;

                this.ddl_repertori.SelectedValue = string.Empty;
                this.ddl_aoo_rf.Items.Clear();

                this.ddl_numProt_E_SelectedIndexChanged(null, null);
                this.Ddl_num_rep_SelectedIndexChanged(null, null);
                this.ddl_dataArrivo_C_SelectedIndexChanged(null, null);
                this.ddl_dataVers_SelectedIndexChanged(null, null);
                this.ddl_idDocumento_C_SelectedIndexChanged(null, null);               

                if (this.rbl_TipiStampe.Visible)
                {
                    this.rbl_TipiStampe.SelectedValue = "TUTTE";
                    this.UpPnlTypePrints.Update();
                }
                this.UpPnlYear.Update();
                this.UpProtocollo.Update();
                this.UpRegistro.Update();
                this.UpDataArrivo.Update();
                this.UpPnlDocNumber.Update();
                this.UpPnlRegistry.Update();

                

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchDocumentAdvancedSearch_Click(object o, EventArgs e)
        {
            try
            {
                ArrayList validationItems = null;
                string firstInvalidControlID = string.Empty;

                bool result = this.SearchDocumentFilters();
                if (result)
                {
                    if (this.UpPnlSelectPrintType.Visible && !string.IsNullOrEmpty(this.RblTypePrints.SelectedValue) && this.RblTypePrints.SelectedValue.Equals("1") && !this.IsValidData(out validationItems, out firstInvalidControlID))
                    {

                        string validationMessage = string.Empty;

                        foreach (string item in validationItems)
                        {
                            if (validationMessage != string.Empty)
                                validationMessage += @"\n";

                            validationMessage += " - " + item;
                        }

                        if (validationMessage != string.Empty)
                            validationMessage = "Sono state rilevate le seguenti incongruenze: " +
                                @"<br/>" + @"<br/>" + validationMessage;

                        string msgDesc = "WarningDocumentCustom";
                        string errFormt = Server.UrlEncode(validationMessage);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');}; ", true);
                        return;
                    }
                    else
                    {
                        this.ListCheck = new Dictionary<string, string>();
                        this.SelectedPage = 1;
                        this.SelectedRow = string.Empty;
                        this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                        this.CheckAll = false;
                    }
                }
                this.UpnlAzioniMassive.Update();
                this.UpnlNumerodocumenti.Update();
                this.UpnlGrid.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private bool IsValidData(out ArrayList validationItems, out string firstInvalidControlID)
        {
            validationItems = new ArrayList();
            firstInvalidControlID = string.Empty;

            // Validazione selezione repertorio
            if (String.IsNullOrEmpty(ddl_repertori.SelectedValue))
                validationItems.Add("Selezionare un repertorio");

            // Validazione numero repertorio
            this.ValidateNumericRange("Numero repertorio", this.txtInitNumRep, this.txtEndNumRep, validationItems, ref firstInvalidControlID);

            // Validazione anno immesso
            if (this.TxtYearRep.Text != string.Empty && !this.IsValidNumber(this.TxtYearRep))
                validationItems.Add("Anno repertorio non valido");

            // Validazione data stampa
            this.ValidateDateRange("Data stampa", this.txtInitDataStampa, this.txtEndDataStampa, validationItems, ref firstInvalidControlID);

            return (validationItems.Count == 0);
        }

        private void ValidateDateRange(string fieldName, CustomTextArea initDate, CustomTextArea endDate, ArrayList validationItems, ref string firstInvalidControlID)
        {
            bool isValidInitDate = false;
            bool isValidEndDate = false;

            if (initDate.Text.Length > 0)
            {
                // Validazione data iniziale
                isValidInitDate = this.IsValidDate(initDate);

                if (!isValidInitDate)
                {
                    validationItems.Add(fieldName + " iniziale non valida");

                    if (firstInvalidControlID == string.Empty)
                        firstInvalidControlID = initDate.ID;
                }
            }

            if (endDate.Visible && endDate.Text.Length > 0)
            {
                // Validazione data finale
                isValidEndDate = this.IsValidDate(endDate);

                if (!isValidEndDate)
                {
                    validationItems.Add(fieldName + " finale non valida");

                    if (firstInvalidControlID == string.Empty)
                        firstInvalidControlID = endDate.ID;
                }
            }

            if (initDate.Text.Length > 0 || (endDate.Visible && endDate.Text.Length > 0))
            {
                if (initDate.Text.Equals(string.Empty))
                    validationItems.Add(fieldName + " iniziale mancante");

                else if (endDate.Visible && endDate.Text.Equals(string.Empty))
                    validationItems.Add(fieldName + " finale mancante");
            }

            // Validazione range di dati
            if (isValidInitDate && isValidEndDate && DateTime.Parse(initDate.Text) > DateTime.Parse(endDate.Text))
            {
                validationItems.Add(fieldName + " iniziale maggiore di quella finale");

                if (firstInvalidControlID == string.Empty)
                    firstInvalidControlID = endDate.ID;
            }
        }

        private bool IsValidDate(CustomTextArea dateMask)
        {
            bool retValue = false;

            if (dateMask.Text.Length > 0)
                retValue = utils.isDate(dateMask.Text);

            return retValue;
        }

        private void ValidateNumericRange(string fieldName, CustomTextArea initText, CustomTextArea endText, ArrayList validationItems, ref string firstInvalidControlID)
        {
            bool isValidInitNumber = false;
            bool isValidEndNumber = false;

            if (initText.Text.Length > 0)
            {
                isValidInitNumber = this.IsValidNumber(initText);

                if (!isValidInitNumber)
                {
                    validationItems.Add(fieldName + " non valido");

                    if (firstInvalidControlID == string.Empty)
                        firstInvalidControlID = initText.ID;
                }
            }

            if (endText.Visible && endText.Text.Length > 0)
            {
                if (endText.Visible)
                {
                    isValidEndNumber = this.IsValidNumber(endText);

                    if (!isValidEndNumber)
                    {
                        validationItems.Add(fieldName + " non valido");

                        if (firstInvalidControlID == string.Empty)
                            firstInvalidControlID = endText.ID;
                    }
                }
            }

            if (initText.Text.Length > 0 || (endText.Visible && endText.Text.Length > 0))
            {
                if (initText.Text.Equals(string.Empty))
                    validationItems.Add(fieldName + " iniziale mancante");

                else if (endText.Visible && endText.Text.Equals(string.Empty))
                    validationItems.Add(fieldName + " finale mancante");
            }

            // Validazione range di dati
            if (isValidInitNumber && isValidEndNumber &&
                int.Parse(initText.Text) > int.Parse(endText.Text))
            {
                validationItems.Add(fieldName + " iniziale maggiore di quello finale");

                if (firstInvalidControlID == string.Empty)
                    firstInvalidControlID = endText.ID;
            }

        }

        private bool IsValidNumber(CustomTextArea numberText)
        {
            bool retValue = true;

            try
            {
                int.Parse(numberText.Text);
            }
            catch
            {
                retValue = false;
            }

            return retValue;
        }

        protected bool GetGridPersonalization()
        {
            return this.ShowGridPersonalization;
        }

        protected void gridViewResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    string idProfile = GrigliaResult.Rows[e.Row.DataItemIndex]["IdProfile"].ToString();

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
                        ((CustomImageButton)e.Row.FindControl("conservazione")).ImageUrlDisabled = "../Images/Icons/padd_preservation_grid_disabled.png";

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
                            labelAdlRole = "ProjectIconTemplateAdlRole";
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

                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).Enabled = true;
                    ((CustomImageButton)e.Row.FindControl("conservazione")).Visible = this.AllowConservazione;
                    ((CustomImageButton)e.Row.FindControl("adl")).Visible = this.AllowADL;
                    ((CustomImageButton)e.Row.FindControl("adlrole")).Visible = this.AllowADLRole;
                    ((CustomImageButton)e.Row.FindControl("firmato")).Visible = bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsSigned"].ToString());

                    //evento click
                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("conservazione")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("adl")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("adlrole")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("firmato")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("estensionedoc")).Click += new ImageClickEventHandler(ImageButton_Click);
                    //tooltip
                    ((CustomImageButton)e.Row.FindControl("estensionedoc")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateEstensioneDoc", UIManager.UserManager.GetUserLanguage()) + " " + estensione;
                    ((CustomImageButton)e.Row.FindControl("conservazione")).ToolTip = Utils.Languages.GetLabelFromCode(labelConservazione, UIManager.UserManager.GetUserLanguage());
                    ((CustomImageButton)e.Row.FindControl("adl")).ToolTip = Utils.Languages.GetLabelFromCode(labelAdl, UIManager.UserManager.GetUserLanguage());
                    ((CustomImageButton)e.Row.FindControl("adlrole")).ToolTip = Utils.Languages.GetLabelFromCode(labelAdlRole, UIManager.UserManager.GetUserLanguage());
                    ((CustomImageButton)e.Row.FindControl("firmato")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateFirmato", UIManager.UserManager.GetUserLanguage());
                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateVisualizzaDocumento", UIManager.UserManager.GetUserLanguage());
                    ((CustomImageButton)e.Row.FindControl("eliminadocumento")).Visible = false;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
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
            InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
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

                            if (bool.Parse(this.GrigliaResult.Rows[rowIndex]["IsInStorageArea"].ToString()))
                            {
                                ProjectManager.RemoveDocumentFromStorageArea(schedaDocumento, infoUtente);
                                GrigliaResult.Rows[rowIndex]["IsInStorageArea"] = false;
                                Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "0";
                                btnIm.ImageUrl = "../Images/Icons/add_preservation_grid.png";
                                btnIm.OnMouseOutImage = "../Images/Icons/add_preservation_grid.png";
                                btnIm.OnMouseOverImage = "../Images/Icons/add_preservation_grid_hover.png";
                                btnIm.ImageUrlDisabled = "../Images/Icons/add_preservation_grid_disabled.png";
                                btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateConservazione", language);
                                btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateConservazione", language);
                            }
                            else
                            {
                                // MEV CS 1.5 - F03_01
                                #region OldCode
                                //ProjectManager.InsertDocumentInStorageArea(idProfile, schedaDocumento, infoUtente);
                                #endregion

                                #region NewCode
                                ProjectManager.InsertDocumentInStorageArea_WithConstraint(idProfile, schedaDocumento, infoUtente);
                                #endregion
                                // End MEV

                                GrigliaResult.Rows[rowIndex]["IsInStorageArea"] = true;
                                Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "1";
                                btnIm.ImageUrl = "../Images/Icons/remove_preservation_grid.png";
                                btnIm.OnMouseOutImage = "../Images/Icons/remove_preservation_grid.png";
                                btnIm.OnMouseOverImage = "../Images/Icons/remove_preservation_grid_hover.png";
                                btnIm.ImageUrlDisabled = "../Images/Icons/remove_preservation_grid_disabled.png";
                                btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveConservazione", language);
                                btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveConservazione", language);

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
                        this.SelectedRow = string.Empty;
                        this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
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
                        Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                        actualPage.IdObject = schedaDocumento.systemId;
                        actualPage.OriginalObjectId = schedaDocumento.systemId;
                        actualPage.NumPage = this.SelectedPage.ToString();
                        actualPage.SearchFilters = this.SearchFilters;
                        actualPage.PageSize = this.PageSize.ToString();
                        actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_PRINTS.ToString(), string.Empty);
                        actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_PRINTS.ToString(), true,this.Page);
                        actualPage.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_PRINTS.ToString();
                        actualPage.Page = "SEARCHDOCUMENTPRINTS.ASPX";
                        actualPage.DxTotalNumberElement = this.RecordCount.ToString();
                        actualPage.ViewResult = true;

                        if (this.PageCount == 0)
                        {
                            actualPage.DxTotalPageNumber = "1";
                        }
                        else
                        {
                            actualPage.DxTotalPageNumber = this.PageCount.ToString();
                        }

                        int indexElement = ((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1);
                        actualPage.DxPositionElement = indexElement.ToString();

                        navigationList.Add(actualPage);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);
                        UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                        this.ListObjectNavigation = this.Result;
                        Response.Redirect("~/Document/Document.aspx");
                        break;
                    }
                case "estensionedoc":
                    {
                        this.IsZoom = true;
                        UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                        FileManager.setSelectedFile(schedaDocumento.documenti[0]);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDocumentViewer", "ajaxModalPopupDocumentViewer();", true);
                        NttDataWA.Popup.DocumentViewer.OpenDocumentViewer = true;
                        break;
                    }
            }

        }

        protected void ddl_dataArrivo_C_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.txtInitDataStampa.Text = string.Empty;
                this.txtEndDataStampa.Text = string.Empty;

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.cboFilterTypeDataStampa.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txtInitDataStampa.ReadOnly = false;
                        this.txtEndDataStampa.Visible = false;
                        this.LtlADataArrivo.Visible = false;
                        this.LtlDaDataArrivo.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txtEndDataStampa.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txtInitDataStampa.ReadOnly = false;
                        this.txtEndDataStampa.ReadOnly = false;
                        this.LtlADataArrivo.Visible = true;
                        this.LtlDaDataArrivo.Visible = true;
                        this.txtEndDataStampa.Visible = true;
                        this.LtlDaDataArrivo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataArrivo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataArrivo.Visible = false;
                        this.txtEndDataStampa.Visible = false;
                        this.txtInitDataStampa.ReadOnly = true;
                        this.txtInitDataStampa.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataArrivo.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txtEndDataStampa.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataArrivo.Visible = true;
                        this.txtEndDataStampa.Visible = true;
                        this.txtEndDataStampa.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txtEndDataStampa.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txtEndDataStampa.ReadOnly = true;
                        this.txtInitDataStampa.ReadOnly = true;
                        this.LtlDaDataArrivo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataArrivo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataArrivo.Visible = true;
                        this.txtEndDataStampa.Visible = true;
                        this.txtInitDataStampa.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txtEndDataStampa.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txtEndDataStampa.ReadOnly = true;
                        this.txtInitDataStampa.ReadOnly = true;
                        this.LtlDaDataArrivo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataArrivo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                this.UpDataArrivo.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dataVers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_DataVers.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initDataVers.ReadOnly = false;
                        this.txt_fineDataVers.Visible = false;
                        this.LtlADataVers.Visible = false;
                        this.LtlDaDataVers.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDataVers.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initDataVers.ReadOnly = false;
                        this.txt_fineDataVers.ReadOnly = false;
                        this.LtlADataVers.Visible = true;
                        this.LtlDaDataVers.Visible = true;
                        this.txt_fineDataVers.Visible = true;
                        this.LtlDaDataVers.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataVers.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataVers.Visible = false;
                        this.txt_fineDataVers.Visible = false;
                        this.txt_initDataVers.ReadOnly = true;
                        this.txt_initDataVers.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataVers.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDataVers.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataVers.Visible = true;
                        this.txt_fineDataVers.Visible = true;
                        this.txt_initDataVers.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_fineDataVers.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_initDataVers.ReadOnly = true;
                        this.txt_fineDataVers.ReadOnly = true;
                        this.LtlDaDataVers.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataVers.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataVers.Visible = true;
                        this.txt_fineDataVers.Visible = true;
                        this.txt_initDataVers.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_fineDataVers.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_initDataVers.ReadOnly = true;
                        this.txt_fineDataVers.ReadOnly = true;
                        this.LtlDaDataVers.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataVers.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                this.UpStatoConservazione.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_datePolicy_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_datePolicy.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initDatePolicy.ReadOnly = false;
                        this.txt_fineDatePolicy.Visible = false;
                        this.LtlADatePolicy.Visible = false;
                        this.LtlDaDatePolicy.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDatePolicy.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initDatePolicy.ReadOnly = false;
                        this.txt_fineDatePolicy.ReadOnly = false;
                        this.LtlADatePolicy.Visible = true;
                        this.LtlDaDatePolicy.Visible = true;
                        this.txt_fineDatePolicy.Visible = true;
                        this.LtlDaDatePolicy.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADatePolicy.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADatePolicy.Visible = false;
                        this.txt_fineDatePolicy.Visible = false;
                        this.txt_initDatePolicy.ReadOnly = true;
                        this.txt_initDatePolicy.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDatePolicy.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDatePolicy.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADatePolicy.Visible = true;
                        this.txt_fineDatePolicy.Visible = true;
                        this.txt_initDatePolicy.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_fineDatePolicy.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_initDatePolicy.ReadOnly = true;
                        this.txt_fineDatePolicy.ReadOnly = true;
                        this.LtlDaDatePolicy.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADatePolicy.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADatePolicy.Visible = true;
                        this.txt_fineDatePolicy.Visible = true;
                        this.txt_initDatePolicy.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_fineDatePolicy.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_initDatePolicy.ReadOnly = true;
                        this.txt_fineDatePolicy.ReadOnly = true;
                        this.LtlDaDatePolicy.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADatePolicy.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                this.UpStatoConservazione.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_idDocumento_C_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            switch (this.ddl_idDocumento_C.SelectedIndex)
            {
                case 0:
                    this.txt_initIdDoc_C.ReadOnly = false;
                    this.txt_fineIdDoc_C.Visible = false;
                    this.LtlAIdDoc.Visible = false;
                    this.LtlDaIdDoc.Visible = false;
                    this.txt_fineIdDoc_C.Text = string.Empty;
                    break;

                case 1:
                    this.txt_initIdDoc_C.ReadOnly = false;
                    this.txt_fineIdDoc_C.ReadOnly = false;
                    this.LtlAIdDoc.Visible = true;
                    this.LtlDaIdDoc.Visible = true;
                    this.txt_fineIdDoc_C.Visible = true;
                    break;
            }
            
        }

        private void InitializePage()
        {

            this.LtlADataVers.Visible = false;
            this.txt_fineDataVers.Visible = false;
            this.LtlADatePolicy.Visible = false;
            this.txt_fineDatePolicy.Visible = false;

            this.ClearSessionProperties();
            this.ShowGridPersonalization = UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION");
            if (UIManager.RegistryManager.isEnableRepertori(UIManager.UserManager.GetUserInSession().idAmministrazione))
            {
                this.PlcRepertories.Visible = true;
            }
            this.InitializeLanguage();
            this.LoadMassiveOperation();
            this.ListaRegistri();
            this.VisibiltyRoleFunctions();
            

            this.ListCheck = new Dictionary<string, string>();
        }

        private void ListaRegistri()
        {
            bool filtroAoo = false;
            DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistriNoFiltroAOO(out filtroAoo);

            //DocsPaWR.Registro[] registri = UserManager.getRuolo(this).registri;
            //string[] listaReg = new string[registri.Length];
            if (userRegistri != null && filtroAoo)
            {
                ListItem itemM = new ListItem(this.GetLabel("LbRegistroMine"), "M");
                rbl_Reg_C.Items.Add(itemM);
                itemM = new ListItem(this.GetLabel("LbRegistroAll"), "T");
                rbl_Reg_C.Items.Add(itemM);
                itemM = new ListItem(this.GetLabel("LbRegistroReset"), "R");
                rbl_Reg_C.Items.Add(itemM);
                lb_reg_C.Rows = 5;
            }
            else
            {
                userRegistri = RoleManager.GetRoleInSession().registri;
                ListItem itemM = new ListItem(this.GetLabel("LbRegistroAll"), "T");
                rbl_Reg_C.Items.Add(itemM);
                itemM = new ListItem(this.GetLabel("LbRegistroReset"), "R");
                rbl_Reg_C.Items.Add(itemM);
                //rbl_Reg_E.SelectedIndex = 1;
            }
            rbl_Reg_C.SelectedIndex = 0;
            string[] id = new string[userRegistri.Length];
            for (int i = 0; i < userRegistri.Length; i++)
            {
                lb_reg_C.Items.Add(userRegistri[i].codRegistro);
                lb_reg_C.Items[i].Value = userRegistri[i].systemId;
                string nomeRegCurrente = "UserReg" + i;
                // SELEZIONA TUTTI I REGISTRI PRESENTI per DEFAULT
                if (!filtroAoo)
                {
                    if (!userRegistri[i].flag_pregresso)
                        lb_reg_C.Items[i].Selected = true;
                }
                else
                    if (rbl_Reg_C.SelectedItem.Value == "M")
                        for (int j = 0; j < RoleManager.GetRoleInSession().registri.Length; j++)
                        {
                            if (RoleManager.GetRoleInSession().registri[j].codRegistro == lb_reg_C.Items[i].Text)
                            {
                                if (!userRegistri[i].flag_pregresso)
                                {
                                    lb_reg_C.Items[i].Selected = true;
                                    break;
                                }
                            }
                        }

                id[i] = (string)userRegistri[i].systemId;
            }

            if (this.lb_reg_C.Items.Count == 1)
            {
                this.plcRegistro.Visible = false;
                this.UpRegistro.Update();
            }

        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        private void LoadMassiveOperation()
        {
            this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem("", ""));
            string title = string.Empty;
            string language = UIManager.UserManager.GetUserLanguage();

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_WA_CONSERVAZIONE.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_WA_CONSERVAZIONE.ToString()).Equals("1"))
            {
                this.IsConservazioneSACER = true;
            }

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
            //if (UIManager.UserManager.IsAuthorizedFunctions("DO_TRA_TRASMETTI") && UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_TRANSMISSION"))
            //{
            //    title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveTransmissionTitle", language);
            //    this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_TRANSMISSION"));

            //}
            if (UIManager.UserManager.IsAuthorizedFunctions("DO_TIMESTAMP") && UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_TIMESTAMP"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveTimestampTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_TIMESTAMP"));

            }
            if (UIManager.UserManager.IsAuthorizedFunctions("DO_SACER_VERSAMENTO") && this.IsConservazioneSACER)
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConservationPARERTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "DO_VERSAMENTO_PARER"));
            }

            //if (UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_CONVERSION"))
            //{
            //    title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConversionTitle", language);
            //    this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_CONVERSION"));

            //}
            //if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADD_ADL"))
            //{
            //    title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveAddAdlUserTitle", language);
            //    this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_ADL"));

            //    title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveAdlUserTitle", language);
            //    this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "REMOVE_MASSIVE_ADL"));
            //}

            //if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
            //{
            //    title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveAddAdlRoleTitle", language);
            //    this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_ADLR_DOC"));

            //    title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveAdlRoleTitle", language);
            //    this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "REMOVE_MASSIVE_ADLR_DOC"));
            //}

            //if (UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_INOLTRA"))
            //{
            //    title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveForwardTitle", language);
            //    this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_INOLTRA"));

            //}


            title = Utils.Languages.GetLabelFromCode("SearchDocumentExportDatiTitle", language);
            this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVEXPORTDOC"));

            //if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONS"))
            //{
            //    title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConservationTitle", language);
            //    this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "DO_MASSIVE_CONS"));

            //}

            //if (UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_REMOVE_VERSIONS"))
            //{
            //    title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveVersionsTitle", language);
            //    this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_REMOVE_VERSIONS"));
            //}

            //if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONSOLIDAMENTO"))
            //{
            //    title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConsolidationTitle", language);
            //    this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "DO_CONSOLIDAMENTO"));
            //}

            //if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONSOLIDAMENTO_METADATI"))
            //{
            //    title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConsolidationMetadatiTitle", language);
            //    this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "DO_CONSOLIDAMENTO_METADATI"));
            //}

            // riordina alfabeticamente
            List<ListItem> listCopy = new List<ListItem>();
            foreach (ListItem item in this.SearchDocumentDdlMassiveOperation.Items)
                listCopy.Add(item);
            this.SearchDocumentDdlMassiveOperation.Items.Clear();
            foreach (ListItem item in listCopy.OrderBy(item => item.Text))
                this.SearchDocumentDdlMassiveOperation.Items.Add(item);
        }

        private void VisibiltyRoleFunctions()
        {
            // INTEGRAZIONE PITRE-PARER
            // Microfunzione per recupero informazioni stato conservazione
            if (this.IsConservazioneSACER)
            {
                if (!(UIManager.UserManager.IsAuthorizedFunctions("DO_SACER_RECUPERO") || UIManager.UserManager.IsAuthorizedFunctions("DO_SACER_VERSAMENTO")))
                {
                    this.cbl_Conservazione.Visible = false;
                    this.UpStatoConservazione.Visible = false;
                }
            }
            else
            {
                this.cbl_Conservazione.Visible = false;
                this.UpStatoConservazione.Visible = false;
            }
                        
        }

        private void ClearSessionProperties()
        {
            Session.Remove("itemUsedSearch");
            Session.Remove("idRicercaSalvata");

            this.Result = null;
            this.SelectedRow = string.Empty;
            this.SearchFilters = null;
            this.RecordCount = 0;
            this.PageCount = 0;
            this.SelectedPage = 1;
            this.CheckAll = false;
            this.ListCheck = null;
            Session["templateRicerca"] = null;

            this.Labels = DocumentManager.GetLettereProtocolli();
            this.CellPosition = new Dictionary<string, int>();
            UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);

            // Caricamento della griglia se non ce n'è una già selezionata
            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Document)
            {
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);
            }

            schedaRicerca = new SearchManager(KEY_SCHEDA_RICERCA, UserManager.GetUserInSession(), RoleManager.GetRoleInSession(), this);
            Session[SearchManager.SESSION_KEY] = schedaRicerca;

            this.SearchDocumentDdlMassiveOperation.Enabled = false;
            this.ShowGrid(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
            {
                this.AllowADLRole = true;
            }

            //questa property rimane in sessione quando dal tab allegati faccio back e torno nella ricerca; va rimossa
            HttpContext.Current.Session.Remove("selectedAttachmentId");
        }

        protected void ddl_numProt_E_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.txtInitNumProtocollo.Text = string.Empty;
                this.txtEndNumProtocollo.Text = string.Empty;

                switch (this.cboFilterTypeNumProtocollo.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txtInitNumProtocollo.ReadOnly = false;
                        this.txtEndNumProtocollo.Visible = false;
                        this.LtlANumProto.Visible = false;
                        this.LtlDaNumProto.Visible = false;
                        this.txtEndNumProtocollo.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txtInitNumProtocollo.ReadOnly = false;
                        this.txtEndNumProtocollo.ReadOnly = false;
                        this.LtlANumProto.Visible = true;
                        this.LtlDaNumProto.Visible = true;
                        this.txtEndNumProtocollo.Visible = true;
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void Ddl_num_rep_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.txtInitNumRep.Text = string.Empty;
                this.txtEndNumRep.Text = string.Empty;

                switch (this.Ddl_num_rep.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txtInitNumRep.ReadOnly = false;
                        this.txtEndNumRep.Visible = false;
                        this.LtlANumRep.Visible = false;
                        this.LtlDaNumRep.Visible = false;
                        this.txtEndNumRep.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txtInitNumRep.ReadOnly = false;
                        this.txtEndNumRep.ReadOnly = false;
                        this.LtlANumRep.Visible = true;
                        this.LtlDaNumRep.Visible = true;
                        this.txtEndNumRep.Visible = true;
                        break;
                }
                this.UpPnlYearNumberRepertory.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void gridViewResult_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                string sortExpression = e.SortExpression.ToString();

                Field d = (Field)GridManager.SelectedGrid.Fields.Where(f => f.Visible && f.FieldId.Equals(sortExpression)).FirstOrDefault();
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

                    if (this.Result != null && this.Result.Length > 0)
                    {
                        this.SearchDocumentFilters();
                        this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    }
                    else
                    {
                        this.ShowGrid(GridManager.SelectedGrid, null, 0, 0, this.Labels.ToArray<EtichettaInfo>());
                    }

                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();

                }
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
        private void SearchDocumentsAndDisplayResult(FiltroRicerca[][] searchFilters, int selectedPage, Grid selectedGrid, EtichettaInfo[] labels)
        {
            // Numero di record restituiti dalla pagina
            int recordNumber = 0;

            // Risultati restituiti dalla ricerca
            SearchObject[] result;

            /* ABBATANGELI GIANLUIGI
             * il nuovo parametro outOfMaxRowSearchable è true se raggiunto il numero 
             * massimo di riche accettate in risposta ad una ricerca */
            bool outOfMaxRowSearchable;
            // Ricerca dei documenti
            result = this.SearchDocument(searchFilters, selectedPage, out recordNumber, out outOfMaxRowSearchable);

            if (outOfMaxRowSearchable)
            {
                string valoreChiaveDB = Utils.InitConfigurationKeys.GetValue("0", NttDataWA.Utils.DBKeys.MAX_ROW_SEARCHABLE.ToString());
                if (valoreChiaveDB.Length == 0)
                {
                    valoreChiaveDB = Utils.InitConfigurationKeys.GetValue("0", DBKeys.MAX_ROW_SEARCHABLE.ToString());
                }
                string msgDesc = "WarningSearchRecordNumber";
                string msgCenter = Utils.Languages.GetMessageFromCode("WarningSearchRecordNumber2", UIManager.UserManager.GetUserLanguage());
                string customError = recordNumber + " " + msgCenter + " " + valoreChiaveDB;
                string errFormt = Server.UrlEncode(customError);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(customError) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(customError) + "');}; ", true);
                recordNumber = 0;
                return;
            }

            // Se ci sono risultati, vengono visualizzati
            if (this.Result != null && this.Result.Length > 0)
            {
                this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                this.SearchDocumentDdlMassiveOperation.Enabled = true;
            }
            else
            {
                this.ShowGrid(selectedGrid, null, 0, 0, labels);
                this.BuildGridNavigator();
                this.SearchDocumentDdlMassiveOperation.Enabled = false;
            }

        }

        /// <summary>
        /// Funzione per la ricerca dei documenti
        /// </summary>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private SearchObject[] SearchDocument(FiltroRicerca[][] searchFilters, int selectedPage, out int recordNumber, out bool outOfMaxRowSearchable)
        {
            // Documenti individuati dalla ricerca
            SearchObject[] documents;

            // Informazioni sull'utente
            InfoUtente userInfo;

            // Numero totale di pagine
            int pageNumbers;

            // Lista dei system id dei documenti restituiti dalla ricerca
            SearchResultInfo[] idProfiles = null;

            // Prelevamento delle informazioni sull'utente
            userInfo = UserManager.GetInfoUser();

            // Recupero dei campi della griglia impostati come visibili
            Field[] visibleArray = null;
            List<Field> visibleFieldsTemplate;

            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Document)
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);

            visibleFieldsTemplate = GridManager.SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field)) && e.CustomObjectId != 0).ToList();

            if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
            {
                visibleArray = visibleFieldsTemplate.ToArray();
            }

            documents = DocumentManager.getQueryInfoDocumentoPagingCustom(userInfo, this, searchFilters, selectedPage, out pageNumbers, out recordNumber, true, true, this.ShowGridPersonalization, this.PageSize, false, visibleArray, null, out idProfiles);

            /* ABBATANGELI GIANLUIGI
             * outOfMaxRowSearchable viene impostato a true se getQueryInfoDocumentoPagingCustom
             * restituisce pageNumbers = -2 (raggiunto il numero massimo di righe possibili come risultato di ricerca)*/
            outOfMaxRowSearchable = (pageNumbers == -2);

            this.RecordCount = recordNumber;
            //this.PageCount = pageNumbers;
            this.PageCount = (int)Math.Round(((double)recordNumber / (double)this.PageSize) + 0.49);
            this.Result = documents;

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


            return documents;
        }

        /// <summary>
        /// Funzione per la visualizzazione dei risutati della ricerca
        /// </summary>
        /// <param name="result">I risultati della ricerca</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private void ShowGrid(Grid selectedGrid, SearchObject[] result, int recordNumber, int selectedPage, EtichettaInfo[] labels)
        {
            bool visibile = false;
            if (!this.IsPostBack)
            {
                Session.Remove("templateRicerca");
            }
            Templates templates = Session["templateRicerca"] as Templates;

            this.CellPosition.Clear();

            gridViewResult = this.HeaderGridView(selectedGrid,
              templates,
              this.ShowGridPersonalization, gridViewResult);

            DataTable dt = UIManager.GridManager.InitializeDataSet(selectedGrid,
                         Session["templateRicerca"] as Templates,
                         this.ShowGridPersonalization);


            if (this.Result != null && this.Result.Length > 0)
            {
                dt = this.FillDataSet(dt, this.Result, selectedGrid, labels, templates, this.ShowGridPersonalization);
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

            this.GrigliaResult = dt;
            this.gridViewResult.DataSource = dt;
            this.gridViewResult.DataBind();
            if (this.gridViewResult.Rows.Count > 0) this.gridViewResult.Rows[0].Visible = visibile;

            string gridType = this.GetLabel("projectLitGrigliaStandard");
            this.projectImgSaveGrid.Enabled = false;
            if (this.ShowGridPersonalization)
            {
                if (selectedGrid != null && string.IsNullOrEmpty(selectedGrid.GridId))
                {
                    gridType = "<span class=\"red\">" + this.GetLabel("projectLitGrigliaTemp") + "</span>";
                    if (this.gridViewResult.Rows.Count > 0) this.projectImgSaveGrid.Enabled = true;
                }
                else
                {
                    if (!(selectedGrid.GridId).Equals("-1"))
                    {
                        gridType = selectedGrid.GridName;
                        if (this.gridViewResult.Rows.Count > 0) projectImgSaveGrid.Enabled = true;
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
                StringBuilder temp;
                foreach (SearchObject doc in result)
                {
                    // ...viene inizializzata una nuova riga
                    dataRow = dataTable.NewRow();

                    foreach (Field field in visibleFields)
                    {
                        string numeroDocumento = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;

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
                                string numeroProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
                                string dataProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
                                string dataApertura = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
                                string protTit = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("PROT_TIT")).FirstOrDefault().SearchObjectFieldValue;

                                temp = new StringBuilder("<span style=\"color:");
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
                                valoreChiave = Utils.InitConfigurationKeys.GetValue("0", "FE_IS_PRESENT_NOTE");

                                if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1"))
                                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ESISTE_NOTA")).FirstOrDefault().SearchObjectFieldValue;
                                else
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
                                        // value = DocumentManager.getSegnaturaRepertorio(this.Page, dNumber, UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Codice);
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
                            //DATE INSERT IN ADL
                            case "DTA_ADL":
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
                                    case "F":
                                        value = this.GetLabel("optConsFld");
                                        break;
                                }
                                break;
                            //OGGETTI CUSTOM
                            default:
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                if (!string.IsNullOrEmpty(value))
                                {
                                    // se l'ggetto custom è un contatore di repertorio visualizzo la segnatura di repertorio
                                    if (value.ToUpper().Equals("#CONTATORE_DI_REPERTORIO#"))
                                    {
                                        //reperisco la segnatura di repertorio
                                        string dNumber = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                                        value = DocumentManager.getSegnaturaRepertorio(dNumber, AdministrationManager.AmmGetInfoAmmCorrente(UserManager.GetInfoUser().idAmministrazione).Codice);
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
                        if (!String.IsNullOrEmpty(dataAnnullamento))
                            dataRow[field.FieldId] = String.Format("<span id=\"doc" + numeroDocumento + "\"  style=\"text-decoration: line-through; color: Red;\">{0}</span>", value);
                        else
                            dataRow[field.FieldId] = String.Format("<span id=\"doc" + numeroDocumento + "\">{0}</span>", value);
                        value = string.Empty;
                    }

                    string immagineAcquisita = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D23")).FirstOrDefault().SearchObjectFieldValue;
                    string inConservazione = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_CONSERVAZIONE")).FirstOrDefault().SearchObjectFieldValue;
                    string inAdl = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_ADL")).FirstOrDefault().SearchObjectFieldValue;
                    string inAdlRole = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_ADLROLE")).FirstOrDefault().SearchObjectFieldValue;
                    string isFirmato = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("CHA_FIRMATO")).FirstOrDefault().SearchObjectFieldValue;

                    dataRow["ProtoType"] = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D3")).FirstOrDefault().SearchObjectFieldValue;
                    dataRow["IdProfile"] = doc.SearchObjectID;
                    dataRow["FileExtension"] = !String.IsNullOrEmpty(immagineAcquisita) && immagineAcquisita != "0" ? immagineAcquisita : String.Empty;
                    dataRow["IsInStorageArea"] = !String.IsNullOrEmpty(inConservazione) && inConservazione != "0" ? true : false;
                    dataRow["IsInWorkingArea"] = !String.IsNullOrEmpty(inAdl) && inAdl != "0" ? true : false;
                    dataRow["IsInWorkingAreaRole"] = !String.IsNullOrEmpty(inAdlRole) && inAdlRole != "0" ? true : false;
                    dataRow["IsSigned"] = !String.IsNullOrEmpty(isFirmato) && isFirmato != "0" ? true : false;

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
                    {
                        columnHL = GridManager.GetLinkColumn(field.Label,
                            field.FieldId,
                            field.Width);
                        columnHL.SortExpression = field.FieldId;
                    }
                    else
                    {

                        if (field is SpecialField)
                        {
                            switch (((SpecialField)field).FieldType)
                            {
                                case SpecialFieldsEnum.Icons:
                                    columnCKB = GridManager.GetBoundColumnIcon(field.Label, field.Width, field.FieldId);
                                    columnCKB.SortExpression = field.FieldId;
                                    break;
                                case SpecialFieldsEnum.CheckBox:
                                    {
                                        columnCKB = GridManager.GetBoundColumnCheckBox(field.Label, field.Width, field.FieldId);
                                        columnCKB.SortExpression = field.FieldId;
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
                                        column.SortExpression = field.FieldId;
                                        break;
                                    }

                                default:
                                    {
                                        column = GridManager.GetBoundColumn(
                                         field.Label,
                                         field.OriginalLabel,
                                         field.Width,
                                         field.FieldId);
                                        column.SortExpression = field.FieldId;
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
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsInWorkingAreaRole", "IsInWorkingAreaRole"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("ProtoType", "ProtoType"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsSigned", "IsSigned"));



                return grid;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
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

        /// <summary>
        /// Funzione per la visualizzazione dei risutati della ricerca
        /// </summary>
        /// <param name="result">I risultati della ricerca</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private void ShowResult(Grid selectedGrid, SearchObject[] result, int recordNumber, int selectedPage, EtichettaInfo[] labels)
        {
            this.ShowGrid(selectedGrid, this.Result, this.RecordCount, selectedPage, labels);
            this.grid_pageindex.Value = (this.PageCount - 1).ToString();
            this.grid_pageindex.Value = this.SelectedPage.ToString();
            this.gridViewResult.PageIndex = this.PageCount;
            this.gridViewResult.SelectedIndex = this.SelectedPage;
            this.BuildGridNavigator();

        }

        private bool SearchDocumentFilters()
        {
            try
            {
                DocsPaWR.FiltroRicerca[][] qV;
                DocsPaWR.FiltroRicerca[] fVList;
                DocsPaWR.FiltroRicerca fV1;
                //array contenitore degli array filtro di ricerca
                qV = new DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPaWR.FiltroRicerca[1];
                fVList = new DocsPaWR.FiltroRicerca[0];


                if (!this.UpPnlSelectPrintType.Visible || !string.IsNullOrEmpty(this.RblTypePrints.SelectedValue) && this.RblTypePrints.SelectedValue.Equals("0"))
                {
                    #region Stampa registro
                    #region filtro registro
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                    string registri = "";
                    if (this.lb_reg_C.Items.Count > 0)
                    {
                        for (int i = 0; i < this.lb_reg_C.Items.Count; i++)
                        {
                            if (this.lb_reg_C.Items[i].Selected)
                            {
                                if (!string.IsNullOrEmpty(registri)) registri += ",";
                                registri += this.lb_reg_C.Items[i].Value;
                            }

                        }
                    }
                    if (!registri.Equals(""))
                    {
                        fV1.valore = registri;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    #endregion

                    #region Filtro per numero e anno protocollo

                    this.AddFilterNumProtocollo(ref fVList);

                    #endregion

                    #region Filtro per data stampa registro

                    if (!string.IsNullOrEmpty(txtInitDataStampa.Text) &&
                        !string.IsNullOrEmpty(txtEndDataStampa.Text) &&
                        utils.verificaIntervalloDate(txtInitDataStampa.Text, txtEndDataStampa.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataStampaInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataStampaInterval', 'warning', '');};", true);
                        return false;
                    }

                    this.AddFilterDataStampa(ref fVList);

                    #endregion

                    #region filtro Archivio  (cerco tutti i doc del tipo R)

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                    fV1.valore = "R";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    #endregion
                    #endregion
                }
                else
                {
                    #region Stampa repertori
                    //ID_REPERTORIO
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriStampaRegistro.ID_REPERTORIO.ToString();
                    fV1.valore = ddl_repertori.SelectedValue;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    //REPERTORIO
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.REP_RF_AOO.ToString();

                    if (!String.IsNullOrEmpty(ddl_aoo_rf.SelectedValue))
                    {
                        if (ddl_aoo_rf.SelectedValue.ToUpper().Equals("TUTTI"))
                        {
                            string valore = string.Empty;
                            foreach (ListItem item in ddl_aoo_rf.Items)
                            {
                                if (!item.Value.ToUpper().Equals("TUTTI"))
                                    valore += item.Value + ",";
                            }
                            if (valore.EndsWith(","))
                                valore = valore.Substring(0, valore.Length - 1);
                            fV1.valore = valore;
                        }
                        else
                        {
                            fV1.valore = ddl_aoo_rf.SelectedValue;
                        }
                    }
                    else
                    {
                        fV1.valore = "0";
                    }

                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    //TIPO DOCUMENTO
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                    fV1.valore = "C";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    //NUMERO E ANNO REPERTORIO
                    this.AddFilterNumRepertorio(ref fVList);

                    //DATA STAMPA REPERTORIO
                    this.AddFilterDataStampa(ref fVList);

                    //FIRMATO
                    if (rbl_TipiStampe.SelectedValue == "FIRMATE")
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = FiltriStampaRegistro.REP_FIRMATO.ToString();
                        fV1.valore = "1";
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (rbl_TipiStampe.SelectedValue == "NON_FIRMATE")
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = FiltriStampaRegistro.REP_FIRMATO.ToString();
                        fV1.valore = "0";
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    #endregion
                }

                // ID DOCUMENTO
                if (this.ddl_idDocumento_C.SelectedIndex.Equals(0))
                {
                    if (!string.IsNullOrEmpty(this.txt_initIdDoc_C.Text))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = FiltriDocumento.DOCNUMBER.ToString();
                        fV1.valore = this.txt_initIdDoc_C.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.txt_initIdDoc_C.Text))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = FiltriDocumento.DOCNUMBER_DAL.ToString();
                        fV1.valore = this.txt_initIdDoc_C.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!string.IsNullOrEmpty(this.txt_fineIdDoc_C.Text))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = FiltriDocumento.DOCNUMBER_AL.ToString();
                        fV1.valore = this.txt_fineIdDoc_C.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                // INTEGRAZIONE PITRE-PARER
                // filtri di ricerca
                #region filtro stato conservazione

                DocsPaWR.FiltroRicerca fVStatoCons = null;
                foreach (ListItem item in cbl_Conservazione.Items)
                {
                    if (item.Selected)
                    {
                        if (fVStatoCons == null)
                            fVStatoCons = new DocsPaWR.FiltroRicerca() { argomento = DocsPaWR.FiltriDocumento.STATO_CONSERVAZIONE.ToString() };

                        fVStatoCons.valore += item.Value;
                    }
                }
                if (fVStatoCons != null)
                    fVList = utils.addToArrayFiltroRicerca(fVList, fVStatoCons);

                #endregion

                #region filtro data versamento

                if (this.cbl_Conservazione.Visible)
                {
                    switch (ddl_DataVers.SelectedValue)
                    {
                        // Valore singolo o oggi
                        // inserisco un solo valore nel filtro
                        case "0":
                        case "2":
                            if (!string.IsNullOrEmpty(this.txt_initDataVers.Text))
                            {
                                fV1 = new DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_VERSAMENTO_IL.ToString();
                                fV1.valore = this.txt_initDataVers.Text;
                                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                            break;
                        // Intervallo, settimana corrente, mese corrente
                        // inserisco una coppia di valori nei filtri
                        case "1":
                        case "3":
                        case "4":
                            if (!string.IsNullOrEmpty(this.txt_initDataVers.Text) && !string.IsNullOrEmpty(this.txt_fineDataVers.Text) &&
                                utils.verificaIntervalloDate(this.txt_initDataVers.Text, this.txt_fineDataVers.Text))
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataVersamento', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataVersamento', 'warning', '');};", true);
                                return false;
                            }
                            if (!string.IsNullOrEmpty(this.txt_initDataVers.Text))
                            {
                                fV1 = new DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_VERSAMENTO_DA.ToString();
                                fV1.valore = this.txt_initDataVers.Text;
                                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                            if (!string.IsNullOrEmpty(this.txt_fineDataVers.Text))
                            {
                                fV1 = new DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_VERSAMENTO_A.ToString();
                                fV1.valore = this.txt_fineDataVers.Text;
                                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                            break;
                    }
                }

                #endregion

                // Policy
                #region filtro codice
                if (!string.IsNullOrEmpty(this.txtCodPolicy.Text))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.POLICY_CODICE.ToString();
                    fV1.valore = this.txtCodPolicy.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro numero esecuzione
                if (!string.IsNullOrEmpty(this.txtCounterPolicy.Text))
                {
                    // verifico che sia valorizzato il filtro sul codice policy
                    if (string.IsNullOrEmpty(this.txtCodPolicy.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterCounterPolicy', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterCounterPolicy', 'warning', '');};", true);
                        return false;
                    }
                    else
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.POLICY_NUM_ESECUZIONE.ToString();
                        fV1.valore = this.txtCounterPolicy.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro data esecuzione
                switch (this.ddl_datePolicy.SelectedValue)
                {
                    // Valore Singolo, Oggi
                    case "0":
                    case "2":
                        if (!string.IsNullOrEmpty(this.txt_initDatePolicy.Text))
                        {
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.DATA_EXEC_POLICY_IL.ToString();
                            fV1.valore = this.txt_initDatePolicy.Text;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        break;
                    // Intervallo, Settimana Corrente, Mese Corrente
                    case "1":
                    case "3":
                    case "4":
                        if (!string.IsNullOrEmpty(this.txt_initDatePolicy.Text) && !string.IsNullOrEmpty(this.txt_fineDatePolicy.Text) &&
                                utils.verificaIntervalloDate(this.txt_initDatePolicy.Text, this.txt_fineDatePolicy.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataPolicy', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataPolicy', 'warning', '');};", true);
                            return false;
                        }
                        if (!string.IsNullOrEmpty(this.txt_initDatePolicy.Text))
                        {
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.DATA_EXEC_POLICY_DA.ToString();
                            fV1.valore = this.txt_initDatePolicy.Text;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        if (!string.IsNullOrEmpty(this.txt_fineDatePolicy.Text))
                        {
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.DATA_EXEC_POLICY_A.ToString();
                            fV1.valore = this.txt_fineDatePolicy.Text;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        break;
                }
                #endregion

                //ABBATANGELI GIANLUIGI - Filtro per nascondere doc di altre applicazioni
                if (System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.FILTRO_APPLICAZIONE.ToString()] != null && !System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.FILTRO_APPLICAZIONE.ToString()].Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.COD_EXT_APP.ToString();
                    fV1.valore = (System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.FILTRO_APPLICAZIONE.ToString()]);
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                #region Ordinamento

                // Reperimento del filtro da utilizzare per la griglia
                List<FiltroRicerca> filterList;

                if (GridManager.SelectedGrid.FieldForOrder != null)
                {
                    Field field = GridManager.SelectedGrid.FieldForOrder;
                    filterList = GridManager.GetOrderFilterForDocument(
                        field.FieldId,
                        GridManager.SelectedGrid.OrderDirection.ToString());
                }
                else
                    filterList = GridManager.GetOrderFilterForDocument(String.Empty, "DESC");

                // Se la lista è valorizzata vengono aggiunti i filtri
                if (filterList != null)
                    foreach (FiltroRicerca filter in filterList)
                        fVList = utils.addToArrayFiltroRicerca(fVList, filter);

                #endregion


                qV[0] = fVList;
                this.SearchFilters = qV;

                return true;
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
                return false;
            }

        }

        private void AddFilterNumRepertorio(ref DocsPaWR.FiltroRicerca[] filterItems)
        {
            bool rangeFilterInterval = (this.Ddl_num_rep.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);

            DocsPaWR.FiltroRicerca filterItem = null;

            if (this.txtInitNumRep.Text.Length > 0)
            {
                filterItem = new DocsPaWR.FiltroRicerca();

                if (rangeFilterInterval)
                    filterItem.argomento = DocsPaWR.FiltriStampaRegistro.NUM_REP_STAMPA_DAL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriStampaRegistro.NUM_REP_STAMPA.ToString();

                filterItem.valore = this.txtInitNumRep.Text;
                filterItems = utils.addToArrayFiltroRicerca(filterItems, filterItem);
                filterItem = null;
            }

            if (rangeFilterInterval && this.txtEndNumRep.Text.Length > 0)
            {
                filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriStampaRegistro.NUM_REP_STAMPA_AL.ToString();
                filterItem.valore = this.txtEndNumRep.Text;
                filterItems = utils.addToArrayFiltroRicerca(filterItems, filterItem);
                filterItem = null;
            }

            if (this.TxtYearRep.Text.Length > 0)
            {
                filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriStampaRegistro.ANNO_REP_STAMPA.ToString();
                filterItem.valore = this.TxtYearRep.Text;
                filterItems = utils.addToArrayFiltroRicerca(filterItems, filterItem);
                filterItem = null;
            }
        }


        /// <summary>
        /// Creazione oggetti di filtro per data stampa
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterDataStampa(ref DocsPaWR.FiltroRicerca[] filterItems)
        {
            bool rangeFilterInterval = (this.cboFilterTypeDataStampa.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);

            DocsPaWR.FiltroRicerca filterItem = null;

            if (this.txtInitDataStampa.Text.Length > 0)
            {
                filterItem = new DocsPaWR.FiltroRicerca();

                if (rangeFilterInterval)
                    filterItem.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_DAL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO.ToString();

                filterItem.valore = this.txtInitDataStampa.Text;
                filterItems = utils.addToArrayFiltroRicerca(filterItems, filterItem);
                filterItem = null;
            }

            if (rangeFilterInterval && this.txtEndDataStampa.Text.Length > 0)
            {
                filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_AL.ToString();
                filterItem.valore = this.txtEndDataStampa.Text;
                filterItems = utils.addToArrayFiltroRicerca(filterItems, filterItem);
                filterItem = null;
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per numero protocollo
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterNumProtocollo(ref DocsPaWR.FiltroRicerca[] filterItems)
        {
            bool rangeFilterInterval = (this.cboFilterTypeNumProtocollo.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);

            DocsPaWR.FiltroRicerca filterItem = null;

            if (this.txtInitNumProtocollo.Text.Length > 0)
            {
                filterItem = new DocsPaWR.FiltroRicerca();

                if (rangeFilterInterval)
                    filterItem.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA_DAL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA.ToString();

                filterItem.valore = this.txtInitNumProtocollo.Text;
                filterItems = utils.addToArrayFiltroRicerca(filterItems, filterItem);
                filterItem = null;
            }

            if (rangeFilterInterval && this.txtEndNumProtocollo.Text.Length > 0)
            {
                filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA_AL.ToString();
                filterItem.valore = this.txtEndNumProtocollo.Text;
                filterItems = utils.addToArrayFiltroRicerca(filterItems, filterItem);
                filterItem = null;
            }

            if (this.TxtYear.Text.Length > 0)
            {
                filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriStampaRegistro.ANNO_PROTOCOLLO_STAMPA.ToString();
                filterItem.valore = this.TxtYear.Text;
                filterItems = utils.addToArrayFiltroRicerca(filterItems, filterItem);
                filterItem = null;
            }
        }

        protected void BuildGridNavigator()
        {
            try
            {
                this.plcNavigator.Controls.Clear();

                int countPage = this.PageCount;

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

        protected void gridViewResult_PreRender(object sender, EventArgs e)
        {
            try
            {
                CheckBox chkBxHeader = (CheckBox)this.gridViewResult.HeaderRow.FindControl("cb_selectall");
                if (chkBxHeader != null)
                {
                    chkBxHeader.Checked = this.CheckAll;
                }


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

                if (!string.IsNullOrEmpty(this.SelectedRow))
                {
                    for (int i = 0; i < gridViewResult.Rows.Count; i++)
                    {
                        if (this.gridViewResult.Rows[i].RowIndex == Int32.Parse(this.SelectedRow))
                        {
                            this.gridViewResult.Rows[i].Attributes.Remove("class");
                            this.gridViewResult.Rows[i].CssClass = "selectedrow";
                            this.gridViewResult.Rows[i - 1].Attributes.Remove("class");
                            this.gridViewResult.Rows[i - 1].CssClass = "selectedrow";
                        }
                    }

                }


                // grid width
                int fullWidth = 0;
                foreach (Field field in GridManager.SelectedGrid.Fields.Where(u => u.Visible).OrderBy(f => f.Position).ToList())
                    fullWidth += field.Width;
                this.gridViewResult.Attributes["style"] = "width: " + fullWidth + "px;";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void gridViewResult_ItemCreated(Object sender, GridViewRowEventArgs e)
        {
            try
            {
                //if (this.ShowGridPersonalization)
                //{
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
                //}

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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
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
        /// Ripristino valori filtri di ricerca nei campi della UI
        /// </summary>
        /// <param name="schedaRicerca"></param>
        private void BindFilterValues(SearchManager schedaRicerca, bool grid)
        {
            if (schedaRicerca.FiltriRicerca != null && schedaRicerca.FiltriRicerca.Length > 0)
            {
                try
                {
                    // INC000000711566
                    // Ricarico la ddl dei repertori (nel caso in cui tra i filtri sia presente il tipo registro repertorio)
                    if (schedaRicerca.FiltriRicerca[0].Where(f => f.argomento.Equals(DocsPaWR.FiltriDocumento.TIPO.ToString())).FirstOrDefault().valore.Equals("C"))
                    {
                        this.setListaRepertori();
                    }

                    foreach (DocsPaWR.FiltroRicerca item in schedaRicerca.FiltriRicerca[0])
                    {
                        if (item.argomento == DocsPaWR.FiltriDocumento.REGISTRO.ToString())
                        {
                            char[] sep = { ',' };
                            string[] regs = item.valore.Split(sep);
                            foreach (ListItem li in this.lb_reg_C.Items)
                                li.Selected = false;
                            foreach (string reg in regs)
                            {
                                for (int i = 0; i < this.lb_reg_C.Items.Count; i++)
                                {
                                    if (this.lb_reg_C.Items[i].Value == reg)
                                        this.lb_reg_C.Items[i].Selected = true;
                                }
                            }
                        }

                        else if (item.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO.ToString())
                        {
                            if (this.cboFilterTypeDataStampa.SelectedIndex != 0)
                                this.cboFilterTypeDataStampa.SelectedIndex = 0;
                            this.ddl_dataArrivo_C_SelectedIndexChanged(null, new System.EventArgs());
                            this.txtInitDataStampa.Visible = true;
                            this.txtInitDataStampa.Text = item.valore;
                        }

                        else if (item.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_DAL.ToString())
                        {
                            if (this.cboFilterTypeDataStampa.SelectedIndex != 1)
                                this.cboFilterTypeDataStampa.SelectedIndex = 1;
                            this.ddl_dataArrivo_C_SelectedIndexChanged(null, new System.EventArgs());
                            this.txtInitDataStampa.Visible = true;
                            this.txtInitDataStampa.Text = item.valore;
                        }

                        else if (item.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_AL.ToString())
                        {
                            if (this.cboFilterTypeDataStampa.SelectedIndex != 1)
                                this.cboFilterTypeDataStampa.SelectedIndex = 1;
                            this.ddl_dataArrivo_C_SelectedIndexChanged(null, new System.EventArgs());
                            this.txtEndDataStampa.Visible = true;
                            this.LtlDaDataArrivo.Visible = true;
                            this.LtlADataArrivo.Visible = true;
                            this.txtEndDataStampa.Text = item.valore;
                        }

                        else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString())
                        {
                            if (this.cboFilterTypeNumProtocollo.SelectedIndex != 0)
                                this.cboFilterTypeNumProtocollo.SelectedIndex = 0;
                            this.ddl_numProt_E_SelectedIndexChanged(null, new System.EventArgs());
                            this.txtInitNumProtocollo.Text = item.valore;
                        }

                        else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString())
                        {
                            if (this.cboFilterTypeNumProtocollo.SelectedIndex != 1)
                                this.cboFilterTypeNumProtocollo.SelectedIndex = 1;
                            this.ddl_numProt_E_SelectedIndexChanged(null, new System.EventArgs());
                            this.txtInitNumProtocollo.Text = item.valore;
                        }

                        else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString())
                        {
                            if (this.cboFilterTypeNumProtocollo.SelectedIndex != 1)
                                this.cboFilterTypeNumProtocollo.SelectedIndex = 1;
                            this.ddl_numProt_E_SelectedIndexChanged(null, new System.EventArgs());
                            this.txtEndNumProtocollo.Text = item.valore;
                        }

                        else if (item.argomento == DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString())
                        {
                            this.TxtYear.Text = item.valore;
                        }


                        else if (item.argomento == DocsPaWR.FiltriStampaRegistro.ID_REPERTORIO.ToString())
                        {
                            this.ddl_repertori.SelectedValue = item.valore;
                            this.pupoluateDdlRfAoo();
                        }

                        else if (item.argomento == DocsPaWR.FiltriDocumento.REP_RF_AOO.ToString() && item.valore != "0")
                        {
                            this.ddl_aoo_rf.SelectedValue = item.valore;
                        }

                        else if (item.argomento == DocsPaWR.FiltriStampaRegistro.ANNO_REP_STAMPA.ToString())
                        {
                            this.TxtYearRep.Text = item.valore;
                        }

                        else if (item.argomento == DocsPaWR.FiltriStampaRegistro.NUM_REP_STAMPA_DAL.ToString())
                        {
                            this.Ddl_num_rep.SelectedValue = RANGE_FILTER_TYPE_INTERVAL;
                            this.txtInitNumRep.Text = item.valore;
                        }

                        else if (item.argomento == DocsPaWR.FiltriStampaRegistro.NUM_REP_STAMPA.ToString())
                        {
                            this.txtInitNumRep.Text = item.valore;
                        }

                        else if (item.argomento == DocsPaWR.FiltriStampaRegistro.NUM_REP_STAMPA_AL.ToString())
                        {
                            this.Ddl_num_rep.SelectedValue = RANGE_FILTER_TYPE_INTERVAL;
                            this.txtEndNumRep.Text = item.valore;
                            this.txtEndNumRep.Visible = true;
                        }
                        else if (item.argomento == DocsPaWR.FiltriStampaRegistro.REP_FIRMATO.ToString())
                        {
                            if (item.valore.Equals("1"))
                                this.rbl_TipiStampe.SelectedValue = "FIRMATE";
                            if (item.valore.Equals("0"))
                                this.rbl_TipiStampe.SelectedValue = "NON_FIRMATE";
                        }
                        else if (item.argomento == DocsPaWR.FiltriDocumento.TIPO.ToString())
                        {
                            if (!string.IsNullOrEmpty(item.valore) && item.valore.Equals("C"))
                            {
                                this.RblTypePrints.SelectedValue = "1";
                                this.PnlPrintRegistry.Visible = false;
                                this.PnlPrintRepertories.Visible = true;
                            }

                            if (!string.IsNullOrEmpty(item.valore) && item.valore.Equals("R"))
                            {
                                this.RblTypePrints.SelectedValue = "0";
                                this.PnlPrintRegistry.Visible = true;
                                this.PnlPrintRepertories.Visible = false;
                            }
                        }

                        else if (item.argomento == DocsPaWR.FiltriDocumento.STATO_CONSERVAZIONE.ToString())
                        {
                            if (!string.IsNullOrEmpty(item.valore))
                            {
                                this.cbl_Conservazione.Items[0].Selected = item.valore.Contains("N");
                                this.cbl_Conservazione.Items[1].Selected = item.valore.Contains("V");
                                this.cbl_Conservazione.Items[2].Selected = item.valore.Contains("W");
                                this.cbl_Conservazione.Items[3].Selected = item.valore.Contains("C");
                                this.cbl_Conservazione.Items[4].Selected = item.valore.Contains("R");
                                this.cbl_Conservazione.Items[5].Selected = item.valore.Contains("E");
                                this.cbl_Conservazione.Items[6].Selected = item.valore.Contains("T");
                                this.cbl_Conservazione.Items[7].Selected = item.valore.Contains("F");
                            }
                        }

                        else if (item.argomento == DocsPaWR.FiltriDocumento.POLICY_CODICE.ToString())
                        {
                            if (!string.IsNullOrEmpty(item.valore))
                            {
                                this.txtCodPolicy.Text = item.valore;
                            }
                        }
                        else if (item.argomento == DocsPaWR.FiltriDocumento.POLICY_NUM_ESECUZIONE.ToString())
                        {
                            if (!string.IsNullOrEmpty(item.valore))
                            {
                                this.txtCounterPolicy.Text = item.valore;
                            }
                        }
                        else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_EXEC_POLICY_IL.ToString())
                        {
                            if (!string.IsNullOrEmpty(item.valore))
                            {
                                this.ddl_datePolicy.SelectedIndex = 1;
                                this.ddl_datePolicy_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initDatePolicy.Text = item.valore;
                            }
                        }
                        else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_EXEC_POLICY_DA.ToString())
                        {
                            if (!string.IsNullOrEmpty(item.valore))
                            {
                                this.ddl_datePolicy.SelectedIndex = 1;
                                this.ddl_datePolicy_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initDatePolicy.Text = item.valore;
                            }
                        }
                        else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_EXEC_POLICY_A.ToString())
                        {
                            if (!string.IsNullOrEmpty(item.valore))
                            {
                                if (!this.ddl_datePolicy.SelectedIndex.Equals(1))
                                {
                                    this.ddl_datePolicy.SelectedIndex = 1;
                                }
                                this.ddl_datePolicy_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_fineDatePolicy.Text = item.valore;
                            }
                        }


                    }

                }
                catch (System.Exception ex)
                {
                    ErrorManager.redirect(this, ex);
                }
            }
        }

        private void setListaRepertori()
        {

            this.Repertori = UIManager.RegisterRepertories.GetRegistriesWithAooOrRfSup(RoleManager.GetRoleInSession().idGruppo, RoleManager.GetRoleInSession().idGruppo);

            if (this.Repertori != null)
            {
                ddl_repertori.Items.Clear();
                ListItem item = new ListItem();
                ddl_repertori.Items.Add(item);

                // INTEGRAZIONE PITRE-PARER
                // MEV Ricerca e gestione stampe repertorio
                // Aggiunta voce "TUTTI"
                item = new ListItem("Tutti", "ALL");
                ddl_repertori.Items.Add(item);

                foreach (RegistroRepertorio rep in this.Repertori)
                {
                    item = new ListItem(rep.TipologyDescription, rep.CounterId);
                    ddl_repertori.Items.Add(item);
                }
            }
        }

        protected void ddl_repertori_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.pupoluateDdlRfAoo();
            this.UpPnlRegistry.Update();
        }

        protected void SearchDocumentDdlMassiveOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
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

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "DO_VERSAMENTO_PARER")
                {
                    
                    // MEV Policy e responsabile conservazione
                    // 1) Se non è stato configurato un responsabile della conservazione si visualizza un messaggio bloccante
                    if (string.IsNullOrEmpty(UserManager.GetIdRuoloRespConservazione()))
                    {
                        string msg = "msgRespConsNotDefined";
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning');}", true);
                    }
                    else
                    {
                        // 2) Se è definita la chiave FE_MAX_DOC_VERSAMENTO devo controllare che il numero di documenti selezionati sia inferiore al limite
                        if (this.checkLimitDocVersamento())
                        {
                            string msg = "msgLimitVersReached";
                            string limit = Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_DOC_VERSAMENTO.ToString());
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '', '" + limit + "');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '', '" + limit + "');}", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveVersPARER", "ajaxModalPopupMassiveVersPARER();", true);
                        }
                    }
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVEXPORTDOC")
                {
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
                        case (Constans.TYPE_ACTIVEX):
                        case (Constans.TYPE_SMARTCLIENT):
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveDigitalSignature", "ajaxModalPopupMassiveDigitalSignature();", true);
                            break;
                        case (Constans.TYPE_APPLET):
                            HttpContext.Current.Session["CommandType"] = null;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveDigitalSignatureApplet", "ajaxModalPopupMassiveDigitalSignatureApplet();", true);
                            break;
                        default:
                            HttpContext.Current.Session["CommandType"] = null;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveDigitalSignatureSockett", "ajaxModalPopupMassiveDigitalSignatureSocket();", true);
                            break;
                    }
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorMassiveOperationNoItemSelected', 'warning', '');", true);
            }

            this.SearchDocumentDdlMassiveOperation.SelectedIndex = -1;
            this.UpnlAzioniMassive.Update();
        }

        protected void GridView_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "viewDetails")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                string idProfile = GrigliaResult.Rows[rowIndex]["IdProfile"].ToString();
                SchedaDocumento schedaDocumento = UIManager.DocumentManager.getDocumentDetails(this, idProfile, idProfile);
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
                Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                actualPage.IdObject = schedaDocumento.systemId;
                actualPage.OriginalObjectId = schedaDocumento.systemId;
                actualPage.NumPage = this.SelectedPage.ToString();
                actualPage.SearchFilters = this.SearchFilters;
                actualPage.PageSize = this.PageSize.ToString();
                actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_PRINTS.ToString(), string.Empty);
                actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_PRINTS.ToString(), true,this.Page);
                actualPage.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_PRINTS.ToString();
                actualPage.Page = "SEARCHDOCUMENTPRINTS.ASPX";
                actualPage.DxTotalNumberElement = this.RecordCount.ToString();
                actualPage.ViewResult = true;

                if (this.PageCount == 0)
                {
                    actualPage.DxTotalPageNumber = "1";
                }
                else
                {
                    actualPage.DxTotalPageNumber = this.PageCount.ToString();
                }

                int indexElement = (((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1))+1;
                actualPage.DxPositionElement = indexElement.ToString();

                navigationList.Add(actualPage);
                Navigation.NavigationUtils.SetNavigationList(navigationList);
                UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                this.ListObjectNavigation = this.Result;
                Response.Redirect("~/Document/Document.aspx");


            }
        }


        protected void pupoluateDdlRfAoo()
        {
            if (this.Repertori != null && this.Repertori.Length != 0 && !String.IsNullOrEmpty(ddl_repertori.SelectedValue) && this.ddl_repertori.SelectedValue != "ALL")
            {
                RegistroRepertorio repertorio = this.Repertori.Where(rep => rep.CounterId.Equals(ddl_repertori.SelectedValue)).FirstOrDefault();
                if (repertorio != null && repertorio.SingleSettings.Length != 0)
                {
                    ddl_aoo_rf.Items.Clear();
                    ddl_aoo_rf.Items.Add(new ListItem("Tutti", "TUTTI"));
                    foreach (RegistroRepertorioSingleSettings r in repertorio.SingleSettings)
                    {
                        if (!String.IsNullOrEmpty(r.RegistryId))
                        {
                            ListItem item = new ListItem(r.RegistryOrRfDescription, r.RegistryId);
                            ddl_aoo_rf.Items.Add(item);
                        }
                        if (!String.IsNullOrEmpty(r.RFId))
                        {
                            ListItem item = new ListItem(r.RegistryOrRfDescription, r.RFId);
                            ddl_aoo_rf.Items.Add(item);
                        }
                    }
                }
            }
            else
            {
                ddl_aoo_rf.Items.Clear();
            }
        }

        private void RemoveIsZoom()
        {
            HttpContext.Current.Session.Remove("isZoom");
        }

        private string ReturnValue
        {
            get
            {
                //Laura 19 Marzo
                if ((HttpContext.Current.Session["ReturnValuePopup"]) != null)
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
            }
        }

        protected int MaxLenghtObject
        {
            get
            {
                int result = 2000;
                if (HttpContext.Current.Session["maxLenghtObject"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["maxLenghtObject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["maxLenghtObject"] = value;
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

        private bool IsEnabledProfilazioneAllegato
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["isEnabledProfilazioneAllegato"] != null)
                {
                    return (bool)HttpContext.Current.Session["isEnabledProfilazioneAllegato"];
                }
                return result;

            }
            set
            {
                HttpContext.Current.Session["isEnabledProfilazioneAllegato"] = value;
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
        /// Risultati restituiti dalla ricerca.
        /// </summary>
        public SearchObject[] Result
        {
            get
            {
                return HttpContext.Current.Session["result"] as SearchObject[];
            }
            set
            {
                HttpContext.Current.Session["result"] = value;
            }
        }

        private FiltroRicerca[][] SearchFilters
        {
            get
            {
                return (FiltroRicerca[][])HttpContext.Current.Session["filtroRicerca"];
            }
            set
            {
                HttpContext.Current.Session["filtroRicerca"] = value;
            }
        }

        private int RecordCount
        {
            get
            {
                int toReturn = 0;
                if (HttpContext.Current.Session["recordCount"] != null) Int32.TryParse(HttpContext.Current.Session["recordCount"].ToString(), out toReturn);
                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["recordCount"] = value;
            }
        }

        private int SelectedPage
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["selectedPage"] != null) Int32.TryParse(HttpContext.Current.Session["selectedPage"].ToString(), out toReturn);
                if (toReturn < 1) toReturn = 1;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["selectedPage"] = value;
            }
        }

        /// <summary>
        /// Numero di pagine restituiti dalla ricerca
        /// </summary>
        public int PageCount
        {
            get
            {
                int toReturn = 1;

                if (HttpContext.Current.Session["PageCount"] != null)
                {
                    Int32.TryParse(
                        HttpContext.Current.Session["PageCount"].ToString(),
                        out toReturn);
                }

                return toReturn;
            }

            set
            {
                HttpContext.Current.Session["PageCount"] = value;
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

        /// <summary>
        /// Posizione celle per ordinamento
        /// </summary>
        public Dictionary<string, int> CellPosition
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

        private RegistroRepertorio[] Repertori
        {
            get
            {
                return (RegistroRepertorio[])HttpContext.Current.Session["repertori"];
            }
            set
            {
                HttpContext.Current.Session["repertori"] = value;
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

        private bool checkLimitDocVersamento()
        {
            bool result = false;

            // Controllo se la chiave è definita
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_DOC_VERSAMENTO.ToString())))
            {
                int limit;
                // Se non riesco ad estrarre il valore (es. chiave definita con valori non numerici) restituisco comunque false
                if (Int32.TryParse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_DOC_VERSAMENTO.ToString()), out limit))
                {
                    if (this.ListCheck.Count > limit)
                        result = true;
                    else
                        result = false;
                }
                else
                    result = false;
            }
            else
            {
                result = false;
            }

            return result;
        }

        private SearchObject[] ListObjectNavigation
        {
            set
            {
                HttpContext.Current.Session["ListObjectNavigation"] = value;
            }
        }

    }
}
