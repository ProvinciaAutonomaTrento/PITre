using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using System.Data;
using System.Text;
using NttDatalLibrary;
using System.Web.UI.HtmlControls;
using NttDataWA.UserControls;
using System.Collections;

namespace NttDataWA.Popup
{
    public partial class AddDocInProject : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    this.ListCheck = new List<string>();
                    this.InitializePage();
                }
                else
                {
                    if (!this.SelectedPage.ToString().Equals(this.grid_pageindex.Value))
                    {
                        this.SetCheckBox();
                        if (!string.IsNullOrEmpty(this.grid_pageindex.Value))
                        {
                            this.SelectedPage = int.Parse(this.grid_pageindex.Value);
                            this.LoadData(UIManager.UserManager.GetInfoUser(), filtroRicercaSession, SelectedPage);
                        }
                        this.buildGridNavigator();
                    }
                    this.setValueReturn();
                }

                if (((ScriptManager)Master.FindControl("ScriptManager1")).IsInAsyncPostBack)
                {
                    // detect action from async postback
                    switch (((ScriptManager)Master.FindControl("ScriptManager1")).AsyncPostBackSourceElementID)
                    {
                        case "upPnlGridIndexes":
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                            break;
                    }
                }

                if (this.CustomDocuments)
                {
                    this.PnlTypeDocument.Controls.Clear();
                    if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                    {
                        if (this.Template == null || !this.Template.SYSTEM_ID.ToString().Equals(this.DocumentDdlTypeDocument.SelectedValue))
                        {
                            this.Template = ProfilerDocManager.getTemplateById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                        }
                        if (this.CustomDocuments)
                        {
                            this.PopulateProfiledDocument();
                        }
                    }
                }
                if (!string.IsNullOrEmpty(this.HiddenPublicFolder.Value))
                {
                    this.HiddenPublicFolder.Value = string.Empty;

                    this.InsertDocInFolder();

                    if ((this.ListaDocUtente != null && this.ListaDocUtente.Count > 0) || (this.ListaDocPrivati != null && this.ListaDocPrivati.Count > 0))
                    {
                        this.InserimentoDocumentiPrivatiPersonali();
                        this.upPnlGridIndexes.Update();
                    }
                    else if (this.ListaDocNonInseriti != null && this.ListaDocNonInseriti.Count > 0)
                    {
                        string msg2 = "WarningAddNonWritableDocInPrj";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg2.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg2.Replace("'", "\\'") + "', 'warning');}", true);

                        this.ricarica = true;
                    }
                    else
                    {
                        this.ricarica = true;
                        this.closePage("up");
                    }
                }
                this.upPnlButtons.Update();
                this.UplnRadioButton.Update();
                this.UplnFiltri.Update();
                this.UplnGrid.Update();
                this.ReApplyScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        //protected void ReloadCheckBox()
        //{
        //    foreach (GridViewRow dgItem in AddDocInProjectGrid.Rows)
        //    {
        //        Label lbl_key = dgItem.FindControl("idProfile") as Label;
        //        CheckBox checkBox = dgItem.FindControl("checkDocumento") as CheckBox;
        //        if (this.ListCheck.Contains(lbl_key.Text))
        //        {
        //            checkBox.Checked = true;
        //        }
        //    }
        //}

        //protected void AddDocInProjectGrid_OnDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        Label lbl_key = e.Row.FindControl("idProfile") as Label;
        //        CheckBox checkBox = e.Row.FindControl("checkDocumento") as CheckBox;
        //        if (this.ListCheck.Contains(lbl_key.Text))
        //        {
        //            checkBox.Checked = true;
        //        }
        //    }
        //}



        private void InitializePage()
        {
            this.InitializeKeys();
            this.InitializeLabel();

            this.LoadTypeDocuments();
            this.InitializeObjectValue();
            this.CheckAll = false;
        }

        private void InitializeKeys()
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.USE_CODICE_OGGETTO.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.USE_CODICE_OGGETTO.ToString()]))
            {
                this.EnableCodeObject = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
            {
                this.rbl_TipoDoc.Items.RemoveAt(3);
                this.rbl_TipoDoc.Items.RemoveAt(2);
                this.rbl_TipoDoc.Items.RemoveAt(0);
                this.rbl_TipoDoc.SelectedValue = "G";
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
            {
                this.AllowADLRole = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()].Equals("1"))
            {
                this.CustomDocuments = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()].Equals("1"))
            {
                this.EnableStateDiagram = true;
            }
        }

        protected void TxtCodeObject_Click(object sender, EventArgs e)
        {
            try
            {
                List<DocsPaWR.Registro> registries = new List<Registro>();
                registries = UIManager.RegistryManager.GetListRegistriesAndRF(UIManager.RoleManager.GetRoleInSession().systemId, "1", string.Empty).ToList<DocsPaWR.Registro>();
                registries.Add(UIManager.RegistryManager.GetRegistryInSession());

                List<string> aL = new List<string>();
                if (registries != null)
                {
                    for (int i = 0; i < registries.Count; i++)
                    {
                        aL.Add(registries[i].systemId);
                    }
                }

                DocsPaWR.Oggetto[] listaObj = null;

                // E' inutile finire nel backend se la casella di testo è vuota (a parte il fatto che 
                // la funzione, in questo caso, restituisce tutto l'oggettario)
                if (!string.IsNullOrEmpty(this.TxtCodeObject.Text.Trim()))
                {
                    //In questo momento tralascio la descrizione oggetto che metto come stringa vuota
                    listaObj = DocumentManager.getListaOggettiByCod(aL.ToArray<string>(), string.Empty, this.TxtCodeObject.Text);
                }
                else
                {
                    listaObj = new DocsPaWR.Oggetto[] { 
                            new DocsPaWR.Oggetto()
                            {
                                descrizione = String.Empty,
                                codOggetto = String.Empty
                            }};
                }

                if (listaObj != null && listaObj.Length > 0)
                {
                    this.TxtObject.Text = listaObj[0].descrizione;
                    this.TxtCodeObject.Text = listaObj[0].codOggetto;
                }
                else
                {
                    this.TxtObject.Text = string.Empty;
                    this.TxtCodeObject.Text = string.Empty;
                }



                this.UpdPnlObject.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected string GetColorCss(string type)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(type))
            {
                if (this.rbl_TipoDoc.SelectedValue.Equals("P") && (type.Equals("A") || type.Equals("P") || type.Equals("I")))
                {
                    result = "redWeight";
                }
                else
                {
                    result = "weight";
                }
            }
            return result;
        }

        private void removeSession()
        {
            HttpContext.Current.Session.Remove("enableCodeObject");
            HttpContext.Current.Session.Remove("AddDocInProject");
            HttpContext.Current.Session.Remove("PagesCount");
            HttpContext.Current.Session.Remove("SelectedPage");
            HttpContext.Current.Session.Remove("filtroRicerca");
        }


        private void InitializeLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.AddDocInProjectBtnSearch.Text = Utils.Languages.GetLabelFromCode("ClassificationSchemaSearch", language);
            this.AddDocInProjectBtnInserisci.Text = Utils.Languages.GetLabelFromCode("AddDocInProjectBtnInserisci", language);
            this.AddDocInProjectClose.Text = Utils.Languages.GetLabelFromCode("AddDocInProjectBtnClose", language);
            //            this.LblAddDocNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumProtocol", language);
            this.LblAddDocDa.Text = Utils.Languages.GetLabelFromCode("LblAddDocDa", language) + " ";
            this.LblAddDocA.Text = Utils.Languages.GetLabelFromCode("LblAddDocA", language) + " ";
            this.LblAddDocAnno.Text = Utils.Languages.GetLabelFromCode("LblAddDocAnno", language) + "  ";
            //          this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocData", language);
            this.DocProt.Text = Utils.Languages.GetLabelFromCode("DocProt", language);
            this.DocNonProt.Text = Utils.Languages.GetLabelFromCode("DocNonProt", language);
            this.Pred.Text = Utils.Languages.GetLabelFromCode("Pred", language);
            this.ADL.Text = Utils.Languages.GetLabelFromCode("ADLU", language);
            this.ADL_ROLE.Text = Utils.Languages.GetLabelFromCode("ADLR", language);
            this.DocStampe.Text = Utils.Languages.GetLabelFromCode("opPrints", language);
            this.LblAddDocOgetto.Text = Utils.Languages.GetLabelFromCode("AddFilterProjectSubject", language);
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
            {
                this.LblAddDocNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumDoc", language) + " ";
                this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocDtaDoc", language) + " ";
            }
            else
            {
                this.LblAddDocNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumProtocol", language);
                this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocData", language) + " ";
            }
            this.projectLitVisibleObjectChars.Text = Utils.Languages.GetLabelFromCode("DocumentLitObjectChAv", language);
            this.DocumentImgObjectary.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgObjectary", language);
            this.DocumentImgObjectary.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgObjectary", language);
            this.Object.Title = Utils.Languages.GetLabelFromCode("TitleObjectPopup", language);

            this.LblAddDocDataA.Text = Utils.Languages.GetLabelFromCode("LblAddDocA", language) + " ";

            this.LblAddDocDataDa.Text = Utils.Languages.GetLabelFromCode("LblAddDocDa", language) + " ";

            if (this.AllowADLRole)
            {
                this.ADL_ROLE.Text = Utils.Languages.GetLabelFromCode("ADLR", language);
            }
            this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("SearchDocumentLitTypology", language);
            this.DocumentDdlTypeDocument.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlTypeDocument", language));
        }

        private void InitializeObjectValue()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.ddl_dtaProto.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ddlAddDocSingolo", language), "S"));
            this.ddl_numProto.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ddlAddDocSingolo", language), "S"));
            this.ddl_dtaProto.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ddlAddDocIntervallo", language), "R"));
            this.ddl_numProto.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ddlAddDocIntervallo", language), "R"));
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_OGGETTO.ToString())))
            {
                this.MaxLenghtObject = int.Parse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_OGGETTO.ToString()));
            }

            this.resetField();
            this.TypeDocument = string.Empty;
            this.AddDoc = true;

            this.resultsearch.Visible = false;
            this.upPnlButtons.Update();
            this.SelectedPage = 1;
            this.TxtObject.MaxLength = MaxLenghtObject;
            if (!this.AllowADLRole)
            {
                this.rbl_TipoDoc.Items.RemoveAt(4);
            }
        }

        private void closePage(string _ParametroDiRitorno)
        {
            if (Request.QueryString["popupid"] == "OpenAddDocCustom")
            {
                ScriptManager.RegisterClientScriptBlock(this.upPnlButtons, this.upPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('OpenAddDocCustom','" + _ParametroDiRitorno + "');", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this.upPnlButtons, this.upPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('OpenAddDoc','" + _ParametroDiRitorno + "');", true);
            }
        }



        protected void ddl_numProto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();

                if (this.ddl_numProto.SelectedValue.Equals("S"))
                {
                    this.txtAddDocA.Visible = false;
                    this.LblAddDocA.Visible = false;
                    this.txtAddDocA.Text = string.Empty;
                    this.LblAddDocDa.Visible = false;
                }
                else
                {
                    this.txtAddDocA.Visible = true;
                    this.LblAddDocA.Visible = true;
                    this.LblAddDocDa.Visible = true;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dtaProto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ddl_dtaProto.SelectedValue.Equals("S"))
                {
                    this.txtAddDocDataA.Visible = false;
                    this.LblAddDocDataA.Visible = false;
                    this.txtAddDocDataA.Text = string.Empty;
                    this.LblAddDocDataDa.Visible = false;
                }
                else
                {
                    this.txtAddDocDataA.Visible = true;
                    this.LblAddDocDataA.Visible = true;
                    this.LblAddDocDataDa.Visible = true;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void rbl_TipoDoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                plh_filtri.Visible = true;
                switch (rbl_TipoDoc.SelectedValue)
                {
                    case "P":
                        {
                            this.LblAddDocNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumProtocol", UIManager.UserManager.GetUserLanguage());
                            this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocData", UIManager.UserManager.GetUserLanguage()) + " ";
                            this.resetField();
                            break;
                        }
                    case "G":
                        {
                            this.LblAddDocNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumDoc", UIManager.UserManager.GetUserLanguage());
                            this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocDtaDoc", UIManager.UserManager.GetUserLanguage()) + " ";
                            this.resetField();
                            break;
                        }

                    case "PRED":
                        {
                            this.LblAddDocNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumDoc", UIManager.UserManager.GetUserLanguage());
                            this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocDtaDoc", UIManager.UserManager.GetUserLanguage());
                            this.resetField();
                            break;
                        }

                    case "ADL":
                    case "ADL_ROLE":
                        {
                            this.LblAddDocNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumDoc", UIManager.UserManager.GetUserLanguage());
                            this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocDtaDoc", UIManager.UserManager.GetUserLanguage());
                            this.resetField();
                            this.plh_filtri.Visible = false;
                            break;
                        }
                    case "STAMPE":
                        {
                            this.LblAddDocNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumDoc", UIManager.UserManager.GetUserLanguage());
                            this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocDtaDoc", UIManager.UserManager.GetUserLanguage()) + " ";
                            this.DocumentDdlTypeDocument.SelectedIndex = 0;
                            this.DocumentDdlTypeDocument_OnSelectedIndexChanged(null, null);
                            this.resetField();
                           
                            break;
                        }

                }
                this.upPnlButtons.Update();
                this.UplnRadioButton.Update();
                this.UplnFiltri.Update();
                if (plh_filtri.Visible)
                {
                    this.buildGridNavigator();
                    this.UplnGrid.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void AddDocInProjectClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.removeSession();
                if (ricarica)
                    this.closePage("up");
                else
                    this.closePage(string.Empty);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        private void ReApplyScripts()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "datepicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "onlynumbers", "OnlyNumbers();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshNoteChars", "charsLeft('TxtObject', " + this.MaxLenghtObject + ", '" + this.projectLitVisibleObjectChars.Text.Replace("'", "\'") + "');", true);
            this.TxtObject_chars.Attributes["rel"] = "TxtObject_" + this.MaxLenghtObject + "_" + this.projectLitVisibleObjectChars.Text;

        }

        private void resetField()
        {
            this.txtAddDocDa.Text = string.Empty;
            this.txtAddDocDataDA.Text = string.Empty;
            this.txtAddDocAnno.Text = DateTime.Now.Year.ToString();
            this.txtAddDocA.Text = string.Empty;
            this.txtAddDocDataA.Text = string.Empty;
            this.IdProfileList = null;

            this.txtAddDocA.Visible = false;
            this.LblAddDocA.Visible = false;
            this.txtAddDocDataA.Visible = false;
            this.LblAddDocDataA.Visible = false;
            this.ddl_dtaProto.SelectedIndex = 0;
            this.ddl_numProto.SelectedIndex = 0;

            if (this.EnableCodeObject)
            {
                this.PnlCodeObject.Visible = true;
                this.PnlCodeObject2.Attributes.Add("class", "colHalf2");
                this.PnlCodeObject3.Attributes.Add("class", "colHalf3");
                this.PnlCodeObject.Attributes.Add("class", "colHalf");
                this.TxtObject.Attributes.Remove("class");
                this.TxtObject.Attributes.Add("class", "txt_objectRight");
            }
        }


        private void setValueReturn()
        {
            if (!string.IsNullOrEmpty(this.Object.ReturnValue))
            {
                this.TxtObject.Text = this.ReturnValue.Split('#').First();
                if (this.ReturnValue.Split('#').Length > 1)
                    this.TxtCodeObject.Text = this.ReturnValue.Split('#').Last();
                this.UpdPnlObject.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('Object','');", true);
            }

            if (!string.IsNullOrEmpty(this.HiddenIsPersonal.Value) && this.HiddenIsPersonal.Value.Equals("true"))
            {
                this.DocInFolder();
                this.closePage("up");
            }
        }

        protected void AddDocInProjectBtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                string idReg = string.Empty;
                //impostazioni iniziali
                if (UIManager.RegistryManager.GetRegistryInSession() != null)
                    idReg = UIManager.RegistryManager.GetRegistryInSession().systemId;
                //VALIDAZIONE DEI DATI DI RICERCA
                if (this.plh_filtri.Visible)
                {
                    string msg = string.Empty;
                    msg = verificaAnno(txtAddDocAnno.Text);
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = verificaRangeData(this.txtAddDocDataDA.Text, this.txtAddDocDataA.Text);
                        if (string.IsNullOrEmpty(msg))
                            msg = verificaRangeNumeroDoc(this.txtAddDocDa.Text, this.txtAddDocA.Text);
                    }

                    if (!string.IsNullOrEmpty(msg))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                        return;
                    }
                }
                this.ListCheck = new List<string>();
                FiltroRicerca[][] filtriRicerca =UIManager.AddDocInProjectManager.RicercaDocDaFascicolare(this.rbl_TipoDoc.SelectedValue, idReg, this.txtAddDocDa.Text, this.txtAddDocA.Text, this.txtAddDocDataDA.Text, this.txtAddDocDataA.Text, this.txtAddDocAnno.Text, this.TxtObject.Text);
                //Aggiungo filtri sulla  tipologia  documentale
                filtriRicerca = GetFilters(filtriRicerca);
                this.filtroRicercaSession = filtriRicerca;
                this.SelectedPage = 1;
                this.CheckAll = false;
                this.LoadData(UIManager.UserManager.GetInfoUser(), filtriRicerca, SelectedPage);
                this.UplnGrid.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Creazione oggetti filtro
        /// </summary>
        /// <returns></returns>
        private FiltroRicerca[][] GetFilters(FiltroRicerca[][] filtriRicerca)
        {
            ArrayList filterItems = new ArrayList();

            this.AddFilterTipologiaDocumento(filterItems);
            this.AddProfilazioneDinamica(filterItems);

            if (!UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION"))
            {
                if (!string.IsNullOrEmpty(DocumentDdlTypeDocument.SelectedValue))
                {
                    Templates template = DocumentManager.getTemplateById(DocumentDdlTypeDocument.SelectedValue, UserManager.GetInfoUser());
                    if (template != null)
                    {
                        OggettoCustom customObjectTemp = new OggettoCustom();
                        customObjectTemp = template.ELENCO_OGGETTI.Where(
                        r => r.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && r.DA_VISUALIZZARE_RICERCA == "1").
                        FirstOrDefault();
                        FiltroRicerca fV1;
                        fV1 = new FiltroRicerca();
                        fV1.argomento = FiltriDocumento.CONTATORE_GRIGLIE_NO_CUSTOM.ToString();
                        fV1.valore = customObjectTemp.TIPO_CONTATORE;
                        fV1.nomeCampo = template.SYSTEM_ID.ToString();
                        filterItems.Add(fV1);

                        // Creazione di un filtro per la profilazione
                        fV1 = new FiltroRicerca();
                        fV1.argomento = FiltriDocumento.PROFILATION_FIELD_FOR_ORDER.ToString();
                        fV1.valore = customObjectTemp.SYSTEM_ID.ToString();
                        fV1.nomeCampo = customObjectTemp.DESCRIZIONE;
                        filterItems.Add(fV1);
                    }

                }
            }

            FiltroRicerca[] initArray = new FiltroRicerca[filterItems.Count];
            filterItems.CopyTo(initArray);
            filterItems = null;

            List<FiltroRicerca> tmp = filtriRicerca[0].ToList();
            tmp.AddRange(initArray.ToList());
            filtriRicerca[0] = tmp.ToArray();
            return filtriRicerca;
        }

        /// <summary>
        /// Creazione oggetti di filtro per oggetto documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterTipoDocumento(ArrayList filterItems)
        {
            FiltroRicerca filterItem = new FiltroRicerca();
            filterItem.argomento = FiltriDocumento.TIPO.ToString();
            filterItem.valore = this.rbl_TipoDoc.SelectedValue;
            filterItems.Add(filterItem);
            filterItem = null;
        }

        /// <summary>
        /// Creazione oggetti di filtro per numero protocollo
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterNumProtocollo(ArrayList filterItems)
        {
            if (!string.IsNullOrEmpty(this.txtAddDocDa.Text) || !string.IsNullOrEmpty(this.txtAddDocA.Text))
            {
                string argomento = string.Empty;
                if (ddl_numProto.SelectedValue.Equals("R"))
                    argomento = FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                else
                    argomento = FiltriDocumento.NUM_PROTOCOLLO.ToString();

                if (!string.IsNullOrEmpty(txtAddDocDa.Text))
                    filterItems.Add(new FiltroRicerca() { argomento = argomento, valore = txtAddDocDa.Text });

                if (!string.IsNullOrEmpty(txtAddDocA.Text))
                    filterItems.Add(new FiltroRicerca() { argomento = FiltriDocumento.NUM_PROTOCOLLO_AL.ToString(), valore = txtAddDocA.Text });
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per data protocollo
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterDataProtocollo(ArrayList filterItems)
        {
            if (!string.IsNullOrEmpty(this.txtAddDocDataDA.Text) || (!string.IsNullOrEmpty(this.txtAddDocDataA.Text)) || !string.IsNullOrEmpty(this.txtAddDocAnno.Text))
            {
                string argomento = string.Empty;
                if (ddl_dtaProto.SelectedValue.Equals("R"))
                    argomento = FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
                else
                    argomento = FiltriDocumento.DATA_PROT_IL.ToString();

                if (!string.IsNullOrEmpty(this.txtAddDocDataDA.Text))
                    filterItems.Add(new FiltroRicerca() { argomento = argomento, valore = txtAddDocDataDA.Text });

                if (!string.IsNullOrEmpty(this.txtAddDocDataA.Text))
                    filterItems.Add(new FiltroRicerca() { argomento = FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString(), valore = txtAddDocDataA.Text });

                if (!string.IsNullOrEmpty(this.txtAddDocAnno.Text))
                    filterItems.Add(new FiltroRicerca() { argomento = FiltriDocumento.ANNO_PROTOCOLLO.ToString(), valore = txtAddDocAnno.Text });
                else
                    filterItems.Add(new FiltroRicerca() { argomento = FiltriDocumento.ANNO_PROTOCOLLO.ToString(), valore = string.Empty });
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per id documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterIDDocumento(ArrayList filterItems)
        {
            if (!string.IsNullOrEmpty(txtAddDocDa.Text) || !string.IsNullOrEmpty(txtAddDocA.Text))
            {
                string argomento = string.Empty;
                if (ddl_numProto.SelectedValue.Equals("R"))
                    argomento = FiltriDocumento.DOCNUMBER_DAL.ToString();
                else
                    argomento = FiltriDocumento.DOCNUMBER.ToString();

                if (!string.IsNullOrEmpty(txtAddDocDa.Text))
                    filterItems.Add(new FiltroRicerca() { argomento = argomento, valore = txtAddDocDa.Text });

                if (!string.IsNullOrEmpty(txtAddDocA.Text))
                    filterItems.Add(new FiltroRicerca() { argomento = FiltriDocumento.DOCNUMBER_AL.ToString(), valore = txtAddDocA.Text });
            }

        }

        /// <summary>
        /// Creazione oggetti di filtro per data creazione documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterDataCreazioneDocumento(ArrayList filterItems)
        {
            if (!string.IsNullOrEmpty(this.txtAddDocDataDA.Text) || !string.IsNullOrEmpty(this.txtAddDocDataA.Text))
            {
                string argomento = string.Empty;
                if (ddl_dtaProto.SelectedValue.Equals("R"))
                    argomento = FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                else
                    argomento = FiltriDocumento.DATA_CREAZIONE_IL.ToString();

                if (!string.IsNullOrEmpty(this.txtAddDocDataDA.Text))
                    filterItems.Add(new FiltroRicerca() { argomento = argomento, valore = txtAddDocDataDA.Text });

                if (!string.IsNullOrEmpty(this.txtAddDocDataA.Text))
                    filterItems.Add(new FiltroRicerca() { argomento = FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString(), valore = txtAddDocDataA.Text });
            }

        }

        /// <summary>
        /// Creazione oggetti di filtro per tipologia documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterTipologiaDocumento(ArrayList filterItems)
        {
            if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
            {
                for (int i = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
                {
                    DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i];
                    controllaCampi(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString());
                }

                FiltroRicerca filterItem = new FiltroRicerca();
                filterItem.argomento = FiltriDocumento.TIPO_ATTO.ToString();
                filterItem.valore = this.DocumentDdlTypeDocument.SelectedValue;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        private bool controllaCampi(DocsPaWR.OggettoCustom oggettoCustom, string idOggetto)
        {
            //In questo metodo, oltre al controllo si salvano i valori dei campi inseriti 
            //dall'utente nel template in sessione. Solo successivamente, quanto verra' salvato 
            //il documento i suddetti valori verranno riportai nel Db vedi metodo "btn_salva_Click" della "docProfilo.aspx"

            //Label_Avviso.Visible = false;
            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "CampoDiTesto":
                    CustomTextArea textBox = (CustomTextArea)PnlTypeDocument.FindControl(idOggetto);
                    if (textBox != null)
                    {
                        if (string.IsNullOrEmpty(textBox.Text))
                        {
                            //SetFocus(textBox);
                            oggettoCustom.VALORE_DATABASE = textBox.Text;
                            return true;
                        }
                        oggettoCustom.VALORE_DATABASE = textBox.Text;
                    }
                    break;
                case "CasellaDiSelezione":
                    CheckBoxList checkBox = (CheckBoxList)PnlTypeDocument.FindControl(idOggetto);
                    if (checkBox != null)
                    {
                        if (checkBox.SelectedIndex == -1)
                        {
                            //SetFocus(checkBox);
                            for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Length; i++)
                                oggettoCustom.VALORI_SELEZIONATI[i] = null;

                            return true;
                        }

                        oggettoCustom.VALORI_SELEZIONATI = new string[checkBox.Items.Count];
                        oggettoCustom.VALORE_DATABASE = "";
                        for (int i = 0; i < checkBox.Items.Count; i++)
                        {
                            if (checkBox.Items[i].Selected)
                            {
                                oggettoCustom.VALORI_SELEZIONATI[i] = checkBox.Items[i].Text;
                            }
                        }
                    }
                    break;
                case "MenuATendina":
                    DropDownList dropDwonList = (DropDownList)PnlTypeDocument.FindControl(idOggetto);
                    if (dropDwonList != null)
                    {
                        if (dropDwonList.SelectedItem.Text.Equals(""))
                        {
                            //SetFocus(dropDwonList);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }
                        oggettoCustom.VALORE_DATABASE = dropDwonList.SelectedItem.Text;
                    }
                    break;
                case "SelezioneEsclusiva":
                    RadioButtonList radioButtonList = (RadioButtonList)PnlTypeDocument.FindControl(idOggetto);
                    if (radioButtonList != null)
                    {
                        if (oggettoCustom.VALORE_DATABASE == "-1" || radioButtonList.SelectedIndex == -1 || radioButtonList.SelectedValue == "-1")
                        {
                            //SetFocus(radioButtonList);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }
                        oggettoCustom.VALORE_DATABASE = radioButtonList.SelectedItem.Text;
                    }
                    break;
                case "Data":
                    UserControls.Calendar dataDa = (UserControls.Calendar)PnlTypeDocument.FindControl("da_" + oggettoCustom.SYSTEM_ID.ToString());
                    UserControls.Calendar dataA = (UserControls.Calendar)PnlTypeDocument.FindControl("a_" + oggettoCustom.SYSTEM_ID.ToString());

                    if (dataDa != null && dataA != null)
                    {
                        if (dataDa.Text.Equals("") && dataA.Text.Equals(""))
                        {
                            //SetFocus(dataDa.txt_Data);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }

                        if (dataDa.Text.Equals("") && dataA.Text != "")
                        {
                            //SetFocus(dataDa.txt_Data);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }

                        if (dataDa.Text != "" && dataA.Text != "")
                            //oggettoCustom.VALORE_DATABASE = dataDa.txt_Data.Text + "@" + dataA.txt_Data.Text;
                            oggettoCustom.VALORE_DATABASE = dataDa.Text + "@" + dataA.Text;

                        if (dataDa.Text != "" && dataA.Text == "")
                            //oggettoCustom.VALORE_DATABASE = dataDa.txt_Data.Text;
                            oggettoCustom.VALORE_DATABASE = dataDa.Text;
                    }

                    break;
                case "Contatore":
                    CustomTextArea contatoreDa = (CustomTextArea)PnlTypeDocument.FindControl("da_" + idOggetto);
                    CustomTextArea contatoreA = (CustomTextArea)PnlTypeDocument.FindControl("a_" + idOggetto);

                    CustomTextArea dataRepertorioDa = (CustomTextArea)this.PnlTypeDocument.FindControl("TxtDataRepertorioDa_" + idOggetto);
                    CustomTextArea dataRepertorioA = (CustomTextArea)this.PnlTypeDocument.FindControl("TxtDataRepertorioA_" + idOggetto);


                     if (dataRepertorioDa != null && dataRepertorioA != null)
                     {
                         if (dataRepertorioDa.Text != "" && dataRepertorioA.Text != "")
                             oggettoCustom.DATA_INSERIMENTO = dataRepertorioDa.Text + "@" + dataRepertorioA.Text;

                         if (dataRepertorioDa.Text != "" && dataRepertorioA.Text == "")
                             oggettoCustom.DATA_INSERIMENTO = dataRepertorioDa.Text;
                     }

                    //Controllo la valorizzazione di campi ed eventualmente notifico gli errori
                    switch (oggettoCustom.TIPO_CONTATORE)
                    {
                        case "T":
                            if (contatoreDa != null && contatoreA != null && contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }
                            break;
                        case "A":
                            DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (contatoreDa != null && ddlAoo != null && contatoreDa.Text.Equals("") && ddlAoo.SelectedValue.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }

                            if (contatoreDa != null && contatoreA != null && contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";
                            break;
                        case "R":
                            DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (contatoreDa != null && ddlRf != null && contatoreDa.Text.Equals("") && ddlRf.SelectedValue.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                oggettoCustom.ID_AOO_RF = "";
                                return true;
                            }

                            if (contatoreDa != null && contatoreA != null && contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";
                            break;
                    }

                    if (contatoreDa != null && contatoreA != null && contatoreDa.Text.Equals("") && contatoreA.Text != "")
                    {
                        //SetFocus(contatoreDa);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }

                    try
                    {
                        if (contatoreDa != null && contatoreDa.Text != null && contatoreDa.Text != "")
                            Convert.ToInt32(contatoreDa.Text);
                        if (contatoreA != null && contatoreA.Text != null && contatoreA.Text != "")
                            Convert.ToInt32(contatoreA.Text);
                    }
                    catch (Exception ex)
                    {
                        //SetFocus(contatoreDa);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }

                    //I campi sono valorizzati correttamente procedo
                    if (contatoreDa != null && contatoreA != null)
                    {
                        if (contatoreDa.Text != "" && contatoreA.Text != "")
                            oggettoCustom.VALORE_DATABASE = contatoreDa.Text + "@" + contatoreA.Text;

                        if (contatoreDa.Text != "" && contatoreA.Text == "")
                            oggettoCustom.VALORE_DATABASE = contatoreDa.Text;
                    }

                    switch (oggettoCustom.TIPO_CONTATORE)
                    {
                        case "A":
                            DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (ddlAoo != null)
                                oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                            break;
                        case "R":
                            DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (ddlRf != null)
                                oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
                            break;
                    }
                    break;
                case "Corrispondente":
                    UserControls.CorrespondentCustom corr = (UserControls.CorrespondentCustom)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    DocsPaWR.Corrispondente corrispondente = new DocsPaWR.Corrispondente();

                    if (corr != null)
                    {
                        // 1 - Ambedue i campi del corrispondente non sono valorizzati
                        if (string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom) && string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                            return true;
                        }
                        // 2 - E' stato valorizzato solo il campo descrizione del corrispondente
                        if (string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom) && !string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
                        {
                            oggettoCustom.VALORE_DATABASE = corr.TxtDescriptionCorrespondentCustom;
                        }
                        // 3 - E' valorizzato il campo codice del corrispondente
                        if (!string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom))
                        {
                            //Cerco il corrispondente
                            if (!string.IsNullOrEmpty(corr.IdCorrespondentCustom))
                                corrispondente = UIManager.AddressBookManager.getCorrispondenteBySystemIDDisabled(corr.IdCorrespondentCustom);
                            else
                                corrispondente = UIManager.AddressBookManager.getCorrispondenteByCodRubrica(corr.TxtCodeCorrespondentCustom, false);

                            //corrispondente = UserManager.getCorrispondenteByCodRubrica(this, corr.CODICE_TEXT);
                            // 3.1 - Corrispondente trovato per codice
                            if (corrispondente != null)
                            {
                                oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
                                oggettoCustom.ESTENDI_STORICIZZATI = corr.ChkStoryCustomCorrespondentCustom;
                            }
                            // 3.2 - Corrispondente non trovato per codice
                            else
                            {
                                // 3.2.1 - Campo descrizione non valorizzato
                                if (string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
                                {
                                    oggettoCustom.VALORE_DATABASE = string.Empty;
                                    return true;
                                }
                                // 3.2.2 - Campo descrizione valorizzato
                                else
                                    oggettoCustom.VALORE_DATABASE = corr.TxtDescriptionCorrespondentCustom;
                            }
                        }
                    }
                    break;
                case "ContatoreSottocontatore":
                    break;


            }
            return false;
        }

        private void AddProfilazioneDinamica(ArrayList filterItems)
        {
            if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
            {
                this.PopulateProfiledDocument();
                FiltroRicerca fV1;
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.PROFILAZIONE_DINAMICA.ToString();
                fV1.template = this.Template;
                fV1.valore = "Profilazione Dinamica";
                //if (this.Template != null && this.Template.ELENCO_OGGETTI != null && this.Template.ELENCO_OGGETTI.Length > 0)
                //{
                //    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                //}
                filterItems.Add(fV1);


                //Templates templates = DocumentManager.getTemplateById(this.DocumentDdlTypeDocument.SelectedValue, UserManager.GetInfoUser());
                //filterItems.Add(new FiltroRicerca() { argomento = FiltriDocumento.PROFILAZIONE_DINAMICA.ToString(), template = templates });
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

            if (!this.TxtCodeObject.Text.Equals(""))
            {
                FiltroRicerca filterItem = new FiltroRicerca();
                filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_OGGETTO.ToString();
                filterItem.valore = this.TxtCodeObject.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        /// <summary>
        /// verifica il se il range dei numero di documenti è corretto
        /// </summary>
        /// <param name="numeroDa"></param>
        /// <param name="numeroA"></param>
        /// <returns></returns>
        private string verificaRangeNumeroDoc(string numeroDa, string numeroA)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(numeroDa) && !string.IsNullOrEmpty(numeroA))
                msg = "ErroreAddDocInProjectRange";//errore numero vuoto 

            if (!string.IsNullOrEmpty(numeroDa) && !string.IsNullOrEmpty(numeroA))
                if (int.Parse(numeroDa) > int.Parse(numeroA))
                    msg = "ErroreAddDocInProjectRange";//errore del range

            return msg;
        }

        /// <summary>
        /// verifica se il range delle date è corretto
        /// </summary>
        /// <param name="dataDa"></param>
        /// <param name="dataA"></param>
        /// <returns></returns>
        private string verificaRangeData(string dataDa, string dataA)
        {

            string msg = string.Empty;
            if (!this.ddl_dtaProto.SelectedValue.Equals("S") && (string.IsNullOrEmpty(dataDa) || string.IsNullOrEmpty(dataA)))
                msg = "ErroreAddDocInProjectRange";//errore data vuota

            if (!string.IsNullOrEmpty(dataDa) && !string.IsNullOrEmpty(dataA))
            {
                if (DateTime.Parse(dataDa) >= DateTime.Parse(dataA))
                {
                    msg = "ErroreAddDocInProjectRange";//errore range data
                }
            }
            return msg;
        }

        /// <summary>
        /// veirifica se l'anno inserito è corretto
        /// </summary>
        /// <param name="anno"></param>
        /// <returns></returns>
        private string verificaAnno(string anno)
        {
            string msg = string.Empty;

            if (string.IsNullOrEmpty(anno))
                msg = "ErroreAddDocInProjectAnno";//errore anno 

            return msg;
        }


        private void LoadData(InfoUtente infoutente, FiltroRicerca[][] filtroRicerca, int numeroPagina)
        {
            try
            {
                SearchResultInfo[] idProfileList;
                int numTotPage = 0;
                int nRec;
                this.IdProfileList = null;

                //ricerca dei documenti grigi o protocollati
                bool grigi = false;
                //if (rbl_TipoDoc.Items.FindByValue("G").Selected) grigi = true;
                InfoDocumento[] infoDoc = UIManager.AddDocInProjectManager.getQueryInfoDocumentoPaging(infoutente, filtroRicerca, numeroPagina, out numTotPage, out nRec, true, grigi, true, true, out idProfileList);

                if (infoDoc != null)
                {

                    if (this.AddDocInProjectGrid.HeaderRow != null)
                    {
                        CheckBox chkBxHeader = (CheckBox)this.AddDocInProjectGrid.HeaderRow.FindControl("cb_selectall");
                        if (chkBxHeader != null)
                        {
                            chkBxHeader.Enabled = true;
                        }
                    }
                }

                this.PagesCount = numTotPage;

                if (infoDoc == null)
                {
                    infoDoc = new InfoDocumento[0];
                }

                if (infoDoc.Length > 0)
                {
                    this.AddDocInProjectBtnInserisci.Enabled = true;
                    this.AddDocInProjectGrid.PageSize = nRec;
                    this.AddDocInProjectGrid.PageIndex = numeroPagina - 1;
                    this.lbl_countRecord.Text = Utils.Languages.GetLabelFromCode("lblAddDocInProjectTotDoc", UIManager.UserManager.GetUserLanguage()) + nRec;

                    //appoggio il risultato in sessione.
                    if (idProfileList != null && idProfileList.Length > 0)
                    {
                        this.IdProfileList = new string[idProfileList.Length];
                        for (int i = 0; i < idProfileList.Length; i++)
                        {
                            this.IdProfileList[i] = idProfileList[i].Id;
                        }
                    }
                }
                else
                {
                    this.lbl_countRecord.Text = Utils.Languages.GetLabelFromCode("LblAddDocResultDoc", UIManager.UserManager.GetUserLanguage());
                    this.AddDocInProjectBtnInserisci.Enabled = false;
                }

                this.BindGrid(infoDoc);
                this.upPnlButtons.Update();
                this.buildGridNavigator();
                this.UplnGrid.Update();
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
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                string[] IdProfiles = this.IdProfileList;
                if (IdProfiles != null)
                {
                    bool value = ((CheckBox)sender).Checked;
                    foreach (string infoD in IdProfiles)
                    {
                        if (value)
                        {

                            if (!this.ListCheck.Contains(infoD))
                            {
                                this.ListCheck.Add(infoD);
                            }
                        }
                        else
                        {
                            if (this.ListCheck.Contains(infoD))
                            {
                                this.ListCheck.Remove(infoD);
                            }
                        }
                    }

                    this.CheckAll = value;

                    foreach (GridViewRow dgItem in AddDocInProjectGrid.Rows)
                    {
                        CheckBox checkBox = dgItem.FindControl("checkDocumento") as CheckBox;
                        checkBox.Checked = value;
                    }
                }

                this.buildGridNavigator();
                this.UplnGrid.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected bool GetEnableDisableCheck()
        {
            bool result = true;
            if (this.IdProfileList != null && (this.IdProfileList.Length == 0 || this.IdProfileList.Length == 1))
            {
                result = false;
            }

            return result;
        }

        public void BindGrid(DocsPaWR.InfoDocumento[] infodocumento)
        {
            try
            {
                //Costruisco il datagrid
                List<GridItemAddDocInProject> Dg_elem = new List<GridItemAddDocInProject>();
                foreach (InfoDocumento infodoc in infodocumento)
                {
                    GridItemAddDocInProject gi = new GridItemAddDocInProject();
                    gi.Data = string.Empty;
                    if (!string.IsNullOrEmpty(infodoc.dataApertura) && infodoc.dataApertura.Length > 0)
                        gi.Data = infodoc.dataApertura.Substring(0, 10);
                    if (!string.IsNullOrEmpty(infodoc.idProfile))
                        gi.idProfile = infodoc.idProfile;

                    if (!string.IsNullOrEmpty(infodoc.numProt))
                        gi.IdDocumento = infodoc.numProt;
                    else //se il doc è grigio
                        gi.IdDocumento = infodoc.docNumber;

                    gi.Fascicola = true;
                    if (!string.IsNullOrEmpty(infodoc.dataAnnullamento))
                        gi.Fascicola = false;
                    if (!string.IsNullOrEmpty(infodoc.tipoProto))
                        gi.TipoDocumento = UIManager.DocumentManager.GetCodeLabel(infodoc.tipoProto);
                    if (!string.IsNullOrEmpty(infodoc.codRegistro))
                        gi.Registro = infodoc.codRegistro;
                    if (!string.IsNullOrEmpty(infodoc.idRegistro))
                        gi.idRegistro = infodoc.idRegistro;
                    if (!string.IsNullOrEmpty(infodoc.oggetto))
                        gi.Oggetto = infodoc.oggetto;

                    gi.Personale = "0";
                    if (!string.IsNullOrEmpty(infodoc.personale))
                        gi.Personale = infodoc.personale;

                    gi.Privato = "0";
                    if (!string.IsNullOrEmpty(infodoc.privato))
                        gi.Privato = infodoc.privato;

                    Dg_elem.Add(gi);
                }
                if (Dg_elem.Count > 0)
                    resultsearch.Visible = true;
                else
                    resultsearch.Visible = false;
                this.AddDocInProjectGrid.DataSource = Dg_elem;
                this.AddDocInProjectGrid.DataBind();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void AddDocInProjectGrid_OnRowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                //if (e.Row.RowType == DataControlRowType.DataRow &&
                //   (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate))
                //{
                //    CheckBox chkBxSelect = (CheckBox)e.Row.Cells[0].FindControl("checkDocumento");
                //    Label lbl_key = ((Label)e.Row.Cells[6].FindControl("idProfile"));

                //    if (chkBxSelect.Checked)//se è spuntato lo inserisco
                //    {
                //        if (!this.ListCheck.Contains(lbl_key.Text))
                //        {
                //            this.ListCheck.Add(lbl_key.Text);
                //        }
                //    }
                //    else //se non è selezionato vedo se è in hashtable, in caso lo rimuovo
                //    {
                //        if (this.ListCheck.Contains(lbl_key.Text))
                //        {
                //            this.ListCheck.Remove(lbl_key.Text);
                //        }
                //    }
                //    CheckBox chkBxHeader = (CheckBox)this.AddDocInProjectGrid.HeaderRow.FindControl("cb_selectall");
                //    chkBxHeader.Checked = this.CheckAll;
                //}
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Label lbl_key = e.Row.FindControl("idProfile") as Label;
                    if (HttpContext.Current.Session["LinkCustom.type"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["LinkCustom.type"].ToString()))
                    {
                        CheckBox checkBox = e.Row.FindControl("checkDocumento") as CheckBox;
                        checkBox.Visible = false;

                        RadioButton radio = e.Row.FindControl("rbSel") as RadioButton;
                        radio.Visible = true;

                        CheckBox chkBxHeader = (CheckBox)this.AddDocInProjectGrid.HeaderRow.FindControl("cb_selectall");
                        if (chkBxHeader != null)
                        {
                            chkBxHeader.Visible = false;
                        }
                    }
                    else
                    {
                        CheckBox checkBox = e.Row.FindControl("checkDocumento") as CheckBox;
                        if (this.ListCheck.Contains(lbl_key.Text))
                        {
                            checkBox.Checked = true;
                        }

                        CheckBox chkBxHeader = (CheckBox)this.AddDocInProjectGrid.HeaderRow.FindControl("cb_selectall");
                        if (chkBxHeader != null)
                        {
                            chkBxHeader.Checked = this.CheckAll;
                        }
                    }
                   
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
                //salvo i check spuntati alla pagina cliccata in precedenza
                foreach (GridViewRow r in AddDocInProjectGrid.Rows)
                {
                    CheckBox checkBox = r.FindControl("checkDocumento") as CheckBox;
                    Label lbl_key = r.FindControl("idProfile") as Label;

                    if (lbl_key != null && checkBox != null)
                    {
                        if (checkBox.Checked)//se è spuntato lo inserisco
                        {
                            if (!this.ListCheck.Contains(lbl_key.Text))
                            {
                                this.ListCheck.Add(lbl_key.Text);
                            }
                        }
                        else //se non è selezionato vedo se è in hashtable, in caso lo rimuovo
                        {
                            if (this.ListCheck.Contains(lbl_key.Text))
                            {
                                this.ListCheck.Remove(lbl_key.Text);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void buildGridNavigator()
        {
            try
            {
                this.plcNavigator.Controls.Clear();

                if (this.PagesCount > 1)
                {
                    Panel panel = new Panel();
                    panel.EnableViewState = true;
                    panel.CssClass = "recordNavigator";

                    int startFrom = 1;
                    if (this.SelectedPage > 6) startFrom = this.SelectedPage - 5;

                    int endTo = 10;
                    if (this.SelectedPage > 6) endTo = this.SelectedPage + 5;
                    if (endTo > this.PagesCount) endTo = this.PagesCount;

                    if (startFrom > 1)
                    {
                        LinkButton btn = new LinkButton();
                        btn.EnableViewState = true;
                        btn.Text = "...";
                        btn.Attributes["onclick"] = "disallowOp('Content2'); $('#grid_pageindex').val(" + (startFrom - 1) + "); __doPostBack('upPnlGridIndexes', ''); return false;";
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
                            btn.Attributes["onclick"] = "disallowOp('Content2'); $('#grid_pageindex').val($(this).text()); __doPostBack('upPnlGridIndexes', ''); return false;";
                            panel.Controls.Add(btn);
                        }
                    }

                    if (endTo < this.PagesCount)
                    {
                        LinkButton btn = new LinkButton();
                        btn.EnableViewState = true;
                        btn.Text = "...";
                        btn.Attributes["onclick"] = "disallowOp('Content2'); $('#grid_pageindex').val(" + endTo + "); __doPostBack('upPnlGridIndexes', ''); return false;";
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

        protected void AddDocInProjectBtnOk_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                if (HttpContext.Current.Session["LinkCustom.type"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["LinkCustom.type"].ToString()))
                {
                    foreach (GridViewRow r in AddDocInProjectGrid.Rows)
                    {
                        RadioButton checkBox = r.FindControl("rbSel") as RadioButton;
                        Label lbl_key = r.FindControl("idProfile") as Label;

                        if (lbl_key != null && checkBox != null)
                        {
                            if (checkBox.Checked)//se è spuntato lo inserisco
                            {
                                HttpContext.Current.Session["LinkCustom.return"] = lbl_key.Text;
                                break;
                            }
                        }
                    }
                    ScriptManager.RegisterClientScriptBlock(this.upPnlButtons, this.upPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('OpenAddDocCustom','" + HttpContext.Current.Session["LinkCustom.type"].ToString() + "');", true);
                }
                else
                {
                    Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                    if (fascicolo.pubblico)
                    {
                        string msgConfirm = "WarningDocumentConfirmPublicFolder";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenPublicFolder', '');", true);
                        return;
                    }
                    this.InsertDocInFolder();

                    if ((this.ListaDocUtente != null && this.ListaDocUtente.Count > 0) || (this.ListaDocPrivati != null && this.ListaDocPrivati.Count > 0))
                    {
                        this.InserimentoDocumentiPrivatiPersonali();
                        this.upPnlGridIndexes.Update();
                    }
                    else if (this.ListaDocNonInseriti != null && this.ListaDocNonInseriti.Count > 0)
                    {
                        string msg2 = "WarningAddNonWritableDocInPrj";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg2.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg2.Replace("'", "\\'") + "', 'warning');}", true);
                        
                        this.ricarica = true;
                    }else 
                    {
                        this.ricarica = true;
                        this.closePage("up");
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void DocumentDdlTypeDocument_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                {
                    if (this.CustomDocuments)
                    {
                        this.Template = ProfilerDocManager.getTemplateById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                        if (this.Template != null)
                        {
                            
                            if (this.EnableStateDiagram)
                            {
                                this.DocumentDdlStateDiagram.ClearSelection();

                                //Verifico se esiste un diagramma di stato associato al tipo di documento
                                //Modifica per la visualizzazione solo degli stati per cui esistono documenti in essi
                                string idDiagramma = DiagrammiManager.getDiagrammaAssociato(this.DocumentDdlTypeDocument.SelectedValue).ToString();
                                if (!string.IsNullOrEmpty(idDiagramma) && !idDiagramma.Equals("0"))
                                {
                                    this.PnlStateDiagram.Visible = true;

                                    //Inizializzazione comboBox
                                    this.DocumentDdlStateDiagram.Items.Clear();
                                    ListItem itemEmpty = new ListItem();
                                    this.DocumentDdlStateDiagram.Items.Add(itemEmpty);

                                    DocsPaWR.Stato[] statiDg = DiagrammiManager.getStatiPerRicerca(idDiagramma, "D");
                                    foreach (Stato st in statiDg)
                                    {
                                        ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                                        this.DocumentDdlStateDiagram.Items.Add(item);
                                    }

                                    this.ddlStateCondition.Visible = true;
                                    this.PnlStateDiagram.Visible = true;
                                }
                                else
                                {
                                    this.ddlStateCondition.Visible = false;
                                    this.PnlStateDiagram.Visible = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.Template = null;
                    Session["templateRicerca"] = null;
                    this.PnlTypeDocument.Controls.Clear();
                    if (this.EnableStateDiagram)
                    {
                        this.DocumentDdlStateDiagram.ClearSelection();
                        this.PnlStateDiagram.Visible = false;
                        this.ddlStateCondition.Visible = false;
                    }
                }
                this.UpPnlTypeDocument.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadTypeDocuments()
        {
            DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
            if (this.CustomDocuments)
            {
                listaTipologiaAtto = DocumentManager.getTipoAttoPDInsRic(this, UserManager.GetInfoUser().idAmministrazione, RoleManager.GetRoleInSession().idGruppo, "1");
            }
            else
                listaTipologiaAtto = DocumentManager.getListaTipologiaAtto(this);

            this.DocumentDdlTypeDocument.Items.Clear();

            ListItem item = new ListItem(string.Empty, string.Empty);
            this.DocumentDdlTypeDocument.Items.Add(item);

            if (listaTipologiaAtto != null)
            {
                for (int i = 0; i < listaTipologiaAtto.Length; i++)
                {
                    item = new ListItem();
                    item.Text = listaTipologiaAtto[i].descrizione;
                    item.Value = listaTipologiaAtto[i].systemId;
                    this.DocumentDdlTypeDocument.Items.Add(item);
                }
            }
        }

        #region CAMPI PROFILO DEL DOCUMENTO
        protected void PopulateProfiledDocument()
        {
            this.PnlTypeDocument.Controls.Clear();
            this.inserisciComponenti(false);
        }

        private void inserisciComponenti(bool readOnly)
        {
            List<AssDocFascRuoli> dirittiCampiRuolo = ProfilerDocManager.getDirittiCampiTipologiaDoc(RoleManager.GetRoleInSession().idGruppo, this.Template.SYSTEM_ID.ToString());

            for (int i = 0, index = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i];

                ProfilerDocManager.addNoRightsCustomObject(dirittiCampiRuolo, oggettoCustom);

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
                        SearchCorrespondentIntExtWithDisabled = true;
                        this.inserisciCorrispondente(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Link":
                        //this.inserisciLink(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "ContatoreSottocontatore":
                        this.inserisciContatoreSottocontatore(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "Separatore":
                        this.inserisciCampoSeparatore(oggettoCustom);
                        break;
                    case "OggettoEsterno":
                        this.inserisciOggettoEsterno(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                }
            }
        }

        public void inserisciOggettoEsterno(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, List<AssDocFascRuoli> dirittiCampiRuolo)
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
            impostaDirittiRuoloSulCampo(etichetta, intAd, oggettoCustom, this.Template, dirittiCampiRuolo);

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

        private void inserisciContatoreSottocontatore(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, List<AssDocFascRuoli> dirittiCampiRuolo)
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

            //Ricerca contatore
            TextBox contatoreDa = new TextBox();
            contatoreDa.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreDa.Text = oggettoCustom.VALORE_DATABASE;
            contatoreDa.Width = 40;
            contatoreDa.CssClass = "comp_profilazione_anteprima";

            TextBox contatoreA = new TextBox();
            contatoreA.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreA.Text = oggettoCustom.VALORE_DATABASE;
            contatoreA.Width = 40;
            contatoreA.CssClass = "comp_profilazione_anteprima";

            TextBox sottocontatoreDa = new TextBox();
            sottocontatoreDa.ID = "da_sottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            sottocontatoreDa.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            sottocontatoreDa.Width = 40;
            sottocontatoreDa.CssClass = "comp_profilazione_anteprima";

            TextBox sottocontatoreA = new TextBox();
            sottocontatoreA.ID = "a_sottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            sottocontatoreA.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            sottocontatoreA.Width = 40;
            sottocontatoreA.CssClass = "comp_profilazione_anteprima";

           
            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }
            }

            if (!oggettoCustom.VALORE_SOTTOCONTATORE.Equals(""))
            {
                if (oggettoCustom.VALORE_SOTTOCONTATORE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_SOTTOCONTATORE.Split('@');
                    sottocontatoreDa.Text = contatori[0].ToString();
                    sottocontatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    sottocontatoreDa.Text = oggettoCustom.VALORE_SOTTOCONTATORE.ToString();
                    sottocontatoreA.Text = "";
                }
            }


            Label etichettaContatoreDa = new Label();
            etichettaContatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
            etichettaContatoreDa.Font.Size = FontUnit.Point(8);
            etichettaContatoreDa.Font.Bold = true;
            etichettaContatoreDa.Font.Name = "Verdana";
            Label etichettaContatoreA = new Label();
            etichettaContatoreA.Text = "&nbsp;a&nbsp;";
            etichettaContatoreA.Font.Size = FontUnit.Point(8);
            etichettaContatoreA.Font.Bold = true;
            etichettaContatoreA.Font.Name = "Verdana";

            Label etichettaSottocontatoreDa = new Label();
            etichettaSottocontatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
            etichettaSottocontatoreDa.Font.Size = FontUnit.Point(8);
            etichettaSottocontatoreDa.Font.Bold = true;
            etichettaSottocontatoreDa.Font.Name = "Verdana";
            Label etichettaSottocontatoreA = new Label();
            etichettaSottocontatoreA.Text = "&nbsp;a&nbsp;";
            etichettaSottocontatoreA.Font.Size = FontUnit.Point(8);
            etichettaSottocontatoreA.Font.Bold = true;
            etichettaSottocontatoreA.Font.Name = "Verdana";

            Label etichettaDataSottocontatoreDa = new Label();
            etichettaDataSottocontatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
            etichettaDataSottocontatoreDa.Font.Size = FontUnit.Point(8);
            etichettaDataSottocontatoreDa.Font.Bold = true;
            etichettaDataSottocontatoreDa.Font.Name = "Verdana";
            Label etichettaDataSottocontatoreA = new Label();
            etichettaDataSottocontatoreA.Text = "&nbsp;a&nbsp;";
            etichettaDataSottocontatoreA.Font.Size = FontUnit.Point(8);
            etichettaDataSottocontatoreA.Font.Bold = true;
            etichettaDataSottocontatoreA.Font.Name = "Verdana";

            //TableRow row = new TableRow();
            //TableCell cell_1 = new TableCell();
            //cell_1.Controls.Add(etichettaContatoreSottocontatore);
            //row.Cells.Add(cell_1);

            //TableCell cell_2 = new TableCell();
            //


            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            Label etichettaDDL = new Label();
            etichettaDDL.EnableViewState = true;
            DropDownList ddl = new DropDownList();
            ddl.EnableViewState = true;

            string language = UIManager.UserManager.GetUserLanguage();
            ddl.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));

            // aggiunto default vuoto
            ddl.Items.Add(new ListItem() { Text = "", Value = "" });

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
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }

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

                        parDll.Controls.Add(etichettaDDL);
                        divColDllEti.Controls.Add(parDll);
                        divRowEtiDll.Controls.Add(divColDllEti);

                        divColDll.Controls.Add(ddl);
                        divRowDll.Controls.Add(divColDll);
                        break;
                }
            }

            if (!oggettoCustom.VALORE_SOTTOCONTATORE.Equals(""))
            {
                if (oggettoCustom.VALORE_SOTTOCONTATORE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_SOTTOCONTATORE.Split('@');
                    sottocontatoreDa.Text = contatori[0].ToString();
                    sottocontatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    sottocontatoreDa.Text = oggettoCustom.VALORE_SOTTOCONTATORE.ToString();
                    sottocontatoreA.Text = "";
                }
            }
        }

        private void inserisciCorrispondente(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            DocsPaWR.StoricoProfilatiOldValue corrOldOb = new StoricoProfilatiOldValue();

            UserControls.CorrespondentCustom corrispondente = (UserControls.CorrespondentCustom)this.LoadControl("../UserControls/CorrespondentCustom.ascx");
            corrispondente.EnableViewState = true;

            corrispondente.TxtEtiCustomCorrespondent = oggettoCustom.DESCRIZIONE;

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
            this.impostaDirittiRuoloSulCampo(corrispondente.TxtEtiCustomCorrespondent, corrispondente, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (corrispondente.Visible)
            {
                this.PnlTypeDocument.Controls.Add(corrispondente);
            }

        }

        private void inserisciData(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
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


            etichettaData.Text = oggettoCustom.DESCRIZIONE;

            etichettaData.CssClass = "weight";

            UserControls.Calendar data = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            data.EnableViewState = true;
            data.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            data.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);
            data.SetEnableTimeMode();

            UserControls.Calendar data2 = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            data2.EnableViewState = true;
            data2.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
            data2.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);
            data2.SetEnableTimeMode();

            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] date = oggettoCustom.VALORE_DATABASE.Split('@');
                    //dataDa.txt_Data.Text = date[0].ToString();
                    //dataA.txt_Data.Text = date[1].ToString();
                    data.Text = date[0].ToString();
                    data2.Text = date[1].ToString();
                }
                else
                {
                    //dataDa.txt_Data.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    //data.txt_Data.Text = "";
                    data.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    data2.Text = "";
                }
            }

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaData, data, oggettoCustom, this.Template, dirittiCampiRuolo);

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

            Label etichettaDataFrom = new Label();
            etichettaDataFrom.EnableViewState = true;
            etichettaDataFrom.Text = "Da";

            HtmlGenericControl parDescFrom = new HtmlGenericControl("p");
            parDescFrom.Controls.Add(etichettaDataFrom);
            parDescFrom.EnableViewState = true;

            Panel divRowValueFrom = new Panel();
            divRowValueFrom.CssClass = "row";
            divRowValueFrom.EnableViewState = true;

            Panel divColValueFrom = new Panel();
            divColValueFrom.CssClass = "col";
            divColValueFrom.EnableViewState = true;

            divColValueFrom.Controls.Add(parDescFrom);
            divRowValueFrom.Controls.Add(divColValueFrom);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValueFrom);
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

            //////
            Label etichettaDataTo = new Label();
            etichettaDataTo.EnableViewState = true;
            etichettaDataTo.Text = "A";

            Panel divRowValueTo = new Panel();
            divRowValueTo.CssClass = "row";
            divRowValueTo.EnableViewState = true;

            Panel divColValueTo = new Panel();
            divColValueTo.CssClass = "col";
            divColValueTo.EnableViewState = true;

            HtmlGenericControl parDescTo = new HtmlGenericControl("p");
            parDescTo.Controls.Add(etichettaDataTo);
            parDescTo.EnableViewState = true;

            divColValueTo.Controls.Add(parDescTo);
            divRowValueTo.Controls.Add(divColValueTo);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValueTo);
            }

            Panel divRowValue2 = new Panel();
            divRowValue2.CssClass = "row";
            divRowValue2.EnableViewState = true;


            Panel divColValue2 = new Panel();
            divColValue2.CssClass = "col";
            divColValue2.EnableViewState = true;

            divColValue2.Controls.Add(data2);
            divRowValue2.Controls.Add(divColValue2);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValue2);
            }

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void inserisciContatore(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            bool paneldll = false;

            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            Label etichettaContatore = new Label();
            etichettaContatore.EnableViewState = true;


            etichettaContatore.Text = oggettoCustom.DESCRIZIONE;

            etichettaContatore.CssClass = "weight";

            CustomTextArea contatoreDa = new CustomTextArea();
            contatoreDa.EnableViewState = true;
            contatoreDa.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreDa.CssClass = "txt_textdata";

            CustomTextArea contatoreA = new CustomTextArea();
            contatoreA.EnableViewState = true;
            contatoreA.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreA.CssClass = "txt_textdata";

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

            //Ruolo ruoloUtente = RoleManager.GetRoleInSession();
            //Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, string.Empty, string.Empty);
            Registro[] registriRfVisibili = RegistryManager.GetRegAndRFListInSession();
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

            //Emanuela 19-05-2014: aggiunto default vuoto
            ddl.Items.Add(new ListItem() { Text = "", Value = "" });

            Panel divRowDll = new Panel();
            divRowDll.CssClass = "row";
            divRowDll.EnableViewState = true;

            Panel divRowEtiDll = new Panel();
            divRowEtiDll.CssClass = "row";
            divRowEtiDll.EnableViewState = true;

            HtmlGenericControl parDll = new HtmlGenericControl("p");
            parDll.EnableViewState = true;

            Panel divColDllEti = new Panel();
            divColDllEti.CssClass = "col";
            divColDllEti.EnableViewState = true;

            Panel divColDll = new Panel();
            divColDll.CssClass = "col";
            divColDll.EnableViewState = true;


            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }
            }

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
                        if (((Registro)registriRfVisibili[i]).chaRF == "0")
                        {
                            item.Value = ((Registro)registriRfVisibili[i]).systemId;
                            item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                            ddl.Items.Add(item);
                        }
                    }
                    //  ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
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
                    /*
                     * Emanuela 21-05-2014: commento per far si che come RF di default venga mostrato l'item vuoto
                    if (ddl.Items.Count == 1)
                        ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                    else
                        ddl.Items.Insert(0, new ListItem(""));
                    */

                    if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && !oggettoCustom.ID_AOO_RF.Equals("0"))
                        ddl.SelectedValue = oggettoCustom.ID_AOO_RF;

                    ddl.CssClass = "chzn-select-deselect";

                    parDll.Controls.Add(etichettaDDL);
                    divColDllEti.Controls.Add(parDll);
                    divRowEtiDll.Controls.Add(divColDllEti);

                    divColDll.Controls.Add(ddl);
                    divRowDll.Controls.Add(divColDll);

                    break;
            }

            if (etichettaContatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }
            }

            Label etichettaContatoreDa = new Label();
            etichettaContatoreDa.EnableViewState = true;
            etichettaContatoreDa.Text = "Da";

            //////
            Label etichettaContatoreA = new Label();
            etichettaContatoreA.EnableViewState = true;
            etichettaContatoreA.Text = "A";

            Panel divColValueTo = new Panel();
            divColValueTo.CssClass = "col";
            divColValueTo.EnableViewState = true;

            Panel divRowValueFrom = new Panel();
            divRowValueFrom.CssClass = "row";
            divRowValueFrom.EnableViewState = true;

            Panel divCol1 = new Panel();
            divCol1.CssClass = "col";
            divCol1.EnableViewState = true;

            Panel divCol2 = new Panel();
            divCol2.CssClass = "col";
            divCol2.EnableViewState = true;

            Panel divCol3 = new Panel();
            divCol3.CssClass = "col";
            divCol3.EnableViewState = true;

            Panel divCol4 = new Panel();
            divCol4.CssClass = "col";
            divCol4.EnableViewState = true;

            divCol1.Controls.Add(etichettaContatoreDa);
            divCol2.Controls.Add(contatoreDa);
            divCol3.Controls.Add(etichettaContatoreA);
            divCol4.Controls.Add(contatoreA);
            divRowValueFrom.Controls.Add(divCol1);
            divRowValueFrom.Controls.Add(divCol2);
            divRowValueFrom.Controls.Add(divCol3);
            divRowValueFrom.Controls.Add(divCol4);

            impostaDirittiRuoloContatore(etichettaContatore, contatoreDa, contatoreA, etichettaContatoreDa, etichettaContatoreA, etichettaDDL, ddl, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (paneldll)
            {
                this.PnlTypeDocument.Controls.Add(divRowEtiDll);
                this.PnlTypeDocument.Controls.Add(divRowDll);
            }

            if (contatoreDa.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValueFrom);
            }

            #region DATA REPERTORIAZIONE
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_ENABLE_DATA_REPERTORIO.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_ENABLE_DATA_REPERTORIO.ToString()).Equals("1"))
            {
                Label dataRepertorio = new Label();
                dataRepertorio.EnableViewState = true;
                if (oggettoCustom.REPERTORIO.Equals("1"))
                    dataRepertorio.Text = Utils.Languages.GetLabelFromCode("SearhDocumentDataRepertorio", language);
                else
                    dataRepertorio.Text = Utils.Languages.GetLabelFromCode("SearhDocumentDataInserimentoContatore", language);
                dataRepertorio.CssClass = "weight";
                Panel divEtichettaDataRepertorio = new Panel();
                divEtichettaDataRepertorio.CssClass = "row";
                divEtichettaDataRepertorio.EnableViewState = true;
                divEtichettaDataRepertorio.Controls.Add(dataRepertorio);

                Panel divFiltriDataRepertorio = new Panel();
                divFiltriDataRepertorio.CssClass = "row";
                divFiltriDataRepertorio.EnableViewState = true;

                Panel divFiltriDdlIntervalloDataRepertorio = new Panel();
                divFiltriDdlIntervalloDataRepertorio.CssClass = "col";
                divFiltriDdlIntervalloDataRepertorio.EnableViewState = true;
                DropDownList ddlIntervalloDataRepertorio = new DropDownList();
                ddlIntervalloDataRepertorio.EnableViewState = true;
                ddlIntervalloDataRepertorio.ID = "DdlIntervalloDataRepertorio_" + oggettoCustom.SYSTEM_ID.ToString();
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "0", Text = Utils.Languages.GetLabelFromCode("ddl_data0", language), Selected = true });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "1", Text = Utils.Languages.GetLabelFromCode("ddl_data1", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "2", Text = Utils.Languages.GetLabelFromCode("ddl_data2", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "3", Text = Utils.Languages.GetLabelFromCode("ddl_data3", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "4", Text = Utils.Languages.GetLabelFromCode("ddl_data4", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "5", Text = Utils.Languages.GetLabelFromCode("ddl_data5", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "6", Text = Utils.Languages.GetLabelFromCode("ddl_data6", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "7", Text = Utils.Languages.GetLabelFromCode("ddl_data7", language) });
                ddlIntervalloDataRepertorio.AutoPostBack = true;
                ddlIntervalloDataRepertorio.SelectedIndexChanged += DdlIntervalloDataRepertorio_SelectedIndexChanged;
                divFiltriDdlIntervalloDataRepertorio.Controls.Add(ddlIntervalloDataRepertorio);

                Panel divFiltriDataDa = new Panel();
                divFiltriDataDa.CssClass = "col";
                divFiltriDataDa.EnableViewState = true;
                Panel divFiltriLblDataDa = new Panel();
                divFiltriLblDataDa.CssClass = "col-no-margin-top";
                divFiltriLblDataDa.EnableViewState = true;
                Label lblDataDa = new Label();
                lblDataDa.ID = "LblDataRepertorioDa_" + oggettoCustom.SYSTEM_ID;
                lblDataDa.EnableViewState = true;
                lblDataDa.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                lblDataDa.CssClass = "weight";
                divFiltriLblDataDa.Controls.Add(lblDataDa);
                divFiltriDataDa.Controls.Add(divFiltriLblDataDa);
                CustomTextArea dataDa = new CustomTextArea();
                dataDa.EnableViewState = true;
                dataDa.ID = "TxtDataRepertorioDa_" + oggettoCustom.SYSTEM_ID.ToString();
                dataDa.CssClass = "txt_textdata datepicker";
                dataDa.CssClassReadOnly = "txt_textdata_disabled";
                dataDa.Style["width"] = "80px";
                divFiltriDataDa.Controls.Add(dataDa);

                Panel divFiltriDataA = new Panel();
                divFiltriDataA.CssClass = "col";
                divFiltriDataA.EnableViewState = true;
                Panel divFiltriLblDataA = new Panel();
                divFiltriLblDataA.CssClass = "col-no-margin-top";
                divFiltriLblDataA.EnableViewState = true;
                Label lblDataA = new Label();
                lblDataA.ID = "LblDataRepertorioA_" + oggettoCustom.SYSTEM_ID;
                lblDataA.EnableViewState = true;
                lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                lblDataA.CssClass = "weight";
                lblDataA.Visible = false;
                divFiltriLblDataA.Controls.Add(lblDataA);
                divFiltriDataA.Controls.Add(divFiltriLblDataA);
                CustomTextArea dataA = new CustomTextArea();
                dataA.EnableViewState = true;
                dataA.ID = "TxtDataRepertorioA_" + oggettoCustom.SYSTEM_ID.ToString();
                dataA.CssClass = "txt_textdata datepicker";
                dataA.CssClassReadOnly = "txt_textdata_disabled";
                dataA.Style["width"] = "80px";
                dataA.Visible = false;
                divFiltriDataA.Controls.Add(dataA);

                divFiltriDataRepertorio.Controls.Add(divFiltriDdlIntervalloDataRepertorio);
                divFiltriDataRepertorio.Controls.Add(divFiltriDataDa);
                divFiltriDataRepertorio.Controls.Add(divFiltriDataA);

                Panel divRowDataRepertorio = new Panel();
                divRowDataRepertorio.CssClass = "row";
                divRowDataRepertorio.EnableViewState = true;

                divRowDataRepertorio.Controls.Add(divEtichettaDataRepertorio);
                divRowDataRepertorio.Controls.Add(divFiltriDataRepertorio);

                if (contatoreDa.Visible)
                {
                    this.PnlTypeDocument.Controls.Add(divRowDataRepertorio);
                }

                #region BindFilterDataRepertorio

                if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
                {
                    if (oggettoCustom.DATA_INSERIMENTO.IndexOf("@") != -1)
                    {
                        ddlIntervalloDataRepertorio.SelectedIndex = 1;
                        dataDa.ReadOnly = false;
                        dataA.ReadOnly = false;
                        lblDataA.Visible = true;
                        lblDataDa.Visible = true;
                        dataA.Visible = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        string[] dataInserimento = oggettoCustom.DATA_INSERIMENTO.Split('@');
                        dataDa.Text = dataInserimento[0].ToString();
                        dataA.Text = dataInserimento[1].ToString();
                    }
                    else
                    {
                        dataDa.Text = oggettoCustom.DATA_INSERIMENTO.ToString();
                        dataA.Text = "";
                    }
                }

                #endregion
            }
            #endregion
        }

        protected void DdlIntervalloDataRepertorio_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string idOggetto = (((DropDownList)sender).ID).Replace("DdlIntervalloDataRepertorio_", "");
                DropDownList dlIntervalloDataRepertorio = (DropDownList)sender;
                CustomTextArea dataDa = (CustomTextArea)this.PnlTypeDocument.FindControl("TxtDataRepertorioDa_" + idOggetto);
                Label lblDataDa = (Label)this.PnlTypeDocument.FindControl("LblDataRepertorioDa_" + idOggetto);
                CustomTextArea dataA = (CustomTextArea)this.PnlTypeDocument.FindControl("TxtDataRepertorioA_" + idOggetto);
                Label lblDataA = (Label)this.PnlTypeDocument.FindControl("LblDataRepertorioA_" + idOggetto);
                string language = UIManager.UserManager.GetUserLanguage();
                switch (dlIntervalloDataRepertorio.SelectedIndex)
                {
                    case 0: //Valore singolo
                        dataDa.ReadOnly = false;
                        dataA.Visible = false;
                        dataA.Text = string.Empty;
                        lblDataA.Visible = false;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        dataDa.ReadOnly = false;
                        dataA.ReadOnly = false;
                        lblDataA.Visible = true;
                        lblDataDa.Visible = true;
                        dataA.Visible = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        lblDataA.Visible = false;
                        dataA.Visible = false;
                        dataA.Text = string.Empty;
                        dataDa.ReadOnly = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.toDay();
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 3: //Settimana corrente
                        lblDataA.Visible = true;
                        dataA.Visible = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        dataA.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        dataA.ReadOnly = true;
                        dataDa.ReadOnly = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        lblDataA.Visible = true;
                        dataA.Visible = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        dataA.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        dataA.ReadOnly = true;
                        dataDa.ReadOnly = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 5: //Ieri
                        lblDataA.Visible = false;
                        dataA.Visible = false;
                        dataA.Text = string.Empty;
                        dataDa.ReadOnly = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.GetYesterday();
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 6: //Ultimi 7 giorni
                        lblDataA.Visible = true;
                        dataA.Visible = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.GetLastSevenDay();
                        dataA.Text = NttDataWA.Utils.dateformat.toDay();
                        dataA.ReadOnly = true;
                        dataDa.ReadOnly = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 7: //Ultimi 31 giorni
                        lblDataA.Visible = true;
                        dataA.Visible = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.GetLastThirtyOneDay();
                        dataA.Text = NttDataWA.Utils.dateformat.toDay();
                        dataA.ReadOnly = true;
                        dataDa.ReadOnly = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                OggettoCustom oggetto = (from o in this.Template.ELENCO_OGGETTI where o.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") select o).FirstOrDefault();
                if (oggetto != null)
                    this.controllaCampi(oggetto, oggetto.SYSTEM_ID.ToString());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public void impostaDirittiRuoloContatore(System.Object etichettaContatore, System.Object contatoreDa, System.Object contatoreA, System.Object etichettaContatoreDa, System.Object etichettaContatoreA, System.Object etichettaDDL, System.Object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichettaContatore).Visible = false;

                        ((System.Web.UI.WebControls.Label)etichettaContatoreDa).Visible = false;
                        ((CustomTextArea)contatoreDa).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaContatoreA).Visible = false;
                        ((CustomTextArea)contatoreA).Visible = false;

                        ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Visible = false;
                    }
                }
            }
        }

        private void inserisciSelezioneEsclusiva(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
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


            etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE;


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
            //int valoreDiDefault = -1;
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
                    //if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    //{
                    //    valoreDiDefault = i;
                    //}
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
            //if (valoreDiDefault != -1)
            //{
            //    selezioneEsclusiva.SelectedIndex = valoreDiDefault;
            //}
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
            this.impostaDirittiRuoloSelezioneEsclusiva(etichettaSelezioneEsclusiva, selezioneEsclusiva, cancella_selezioneEsclusiva, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (etichettaSelezioneEsclusiva.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (selezioneEsclusiva.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }
        }

        protected void cancella_selezioneEsclusiva_Click(object sender, EventArgs e)
        {
            try
            {
                string idOggetto = (((CustomImageButton)sender).ID).Substring(1);
                ((RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto)).SelectedIndex = -1;
                ((RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto)).EnableViewState = true;
                for (int i = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
                {
                    if (((DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i]).SYSTEM_ID.ToString().Equals(idOggetto))
                    {
                        ((DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i]).VALORE_DATABASE = "";
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public void impostaDirittiRuoloSelezioneEsclusiva(System.Object etichetta, System.Object campo, System.Object button, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Visible = false;
                        //((System.Web.UI.HtmlControls.HtmlAnchor)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                }
            }
        }

        private void inserisciMenuATendina(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            DocsPaWR.StoricoProfilatiOldValue menuOldObj = new StoricoProfilatiOldValue();
            Label etichettaMenuATendina = new Label();
            etichettaMenuATendina.EnableViewState = true;
            etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE;

            etichettaMenuATendina.CssClass = "weight";

            int maxLenght = 0;
            DropDownList menuATendina = new DropDownList();
            menuATendina.EnableViewState = true;
            menuATendina.ID = oggettoCustom.SYSTEM_ID.ToString();
            //int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                //if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                //{
                //    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                //    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                //        valoreOggetto.ABILITATO = 1;

                menuATendina.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                //Valore di default
                //if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                //{
                //    valoreDiDefault = i;
                //}

                if (maxLenght < valoreOggetto.VALORE.Length)
                {
                    maxLenght = valoreOggetto.VALORE.Length;
                }
                //  }
            }
            menuATendina.CssClass = "chzn-select-deselect";
            string language = UIManager.UserManager.GetUserLanguage();
            menuATendina.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));
            menuATendina.Width = maxLenght + 250;

            //if (valoreDiDefault != -1)
            //{
            //    menuATendina.SelectedIndex = valoreDiDefault;
            //}
            //if (!(valoreDiDefault != -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
            //{
            menuATendina.Items.Insert(0, "");
            //}
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
            this.impostaDirittiRuoloSulCampo(etichettaMenuATendina, menuATendina, oggettoCustom, this.Template, dirittiCampiRuolo);

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
        }

        private void inserisciCampoDiTesto(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            Label etichettaCampoDiTesto = new Label();
            etichettaCampoDiTesto.EnableViewState = true;

            CustomTextArea txt_CampoDiTesto = new CustomTextArea();
            txt_CampoDiTesto.EnableViewState = true;

            if (oggettoCustom.MULTILINEA.Equals("SI"))
            {

                etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;

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

                etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;

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
            this.impostaDirittiRuoloSulCampo(etichettaCampoDiTesto, txt_CampoDiTesto, oggettoCustom, this.Template, dirittiCampiRuolo);

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

        private void inserisciCasellaDiSelezione(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
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
            //int valoreDiDefault = -1;
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
                        //if (valoreElenco.VALORE_DI_DEFAULT.Equals("SI"))
                        //{
                        //    valoreDiDefault = i;
                        //}
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
            //if (valoreDiDefault != -1)
            //{
            //    casellaSelezione.SelectedIndex = valoreDiDefault;
            //}

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
            this.impostaDirittiRuoloSulCampo(etichettaCasellaSelezione, casellaSelezione, oggettoCustom, this.Template, dirittiCampiRuolo);

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

        public void impostaDirittiRuoloSulCampo(System.Object etichetta, System.Object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((CustomTextArea)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "CasellaDiSelezione":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.CheckBoxList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "MenuATendina":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
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
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.Calendar)campo).Visible = false;
                                ((UserControls.Calendar)campo).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Corrispondente":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((UserControls.CorrespondentCustom)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Link":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.LinkDocFasc)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "OggettoEsterno":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
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
        #endregion
        /// <summary>
        /// Questo metodo inserisce il documento selezionato nella classifica richiesta 
        /// </summary>
        private void InsertDocInFolder()
        {
            try
            {
                string idFolder = UIManager.ProjectManager.getProjectInSession().folderSelezionato.systemID;
                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                string msg = string.Empty;
                this.ListaDocPrivati = new List<SchedaDocumento>();
                this.ListaDocUtente = new List<SchedaDocumento>();
                this.ListaDocNonInseriti = new List<SchedaDocumento>();
                this.ListaIdProfNonInseriti = new List<string>();
                this.SetCheckBox();

                if (this.ListCheck != null && this.ListCheck.Count > 0)
                {
                    foreach (string idProfile in this.ListCheck)
                    {
                        SchedaDocumento doc = DocumentManager.getDocumentDetails(this.Page, idProfile, idProfile);
                        if (!string.IsNullOrEmpty(doc.accessRights) && Convert.ToInt32(doc.accessRights) > Convert.ToInt32(HMdiritti.HDdiritti_Waiting))
                        {

                            if (doc.privato.Equals("0") && doc.personale.Equals("0"))
                            {
                                if (!UIManager.AddDocInProjectManager.isDocumentiInFolder(idProfile, idFolder))
                                {
                                    if (!UIManager.AddDocInProjectManager.addDocumentoInFolder(idProfile, idFolder, infoUtente))
                                    {
                                        //this.ListaDocNonInseriti.Add(doc);
                                    }
                                }
                            }
                            else if (string.IsNullOrWhiteSpace(doc.privato) && string.IsNullOrWhiteSpace(doc.personale))
                            {
                                // aggiunta stampe registro
                                if (!UIManager.AddDocInProjectManager.isDocumentiInFolder(idProfile, idFolder))
                                {
                                    if (!UIManager.AddDocInProjectManager.addDocumentoInFolder(idProfile, idFolder, infoUtente))
                                    {
                                        //this.ListaDocNonInseriti.Add(doc);
                                    }
                                }
                            }
                            else
                            {
                                if (doc.personale.Equals("1"))
                                {
                                    if (!this.ListaDocUtente.Contains(doc))
                                    {
                                        this.ListaDocUtente.Add(doc);
                                    }
                                }
                                else
                                {
                                    if (doc.privato.Equals("1"))
                                    {
                                        if (!this.ListaDocPrivati.Contains(doc))
                                        {
                                            this.ListaDocPrivati.Add(doc);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //string msg2 = "Il documento " + idProfile + " non è stato inserito nel fascicolo perché non si hanno diritti di scrittura sullo stesso";
                            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg2.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg2.Replace("'", "\\'") + "', 'warning');}", true);
                            if (!this.ListaDocNonInseriti.Contains(doc))
                            {
                                this.ListaDocNonInseriti.Add(doc);
                                this.ListaIdProfNonInseriti.Add(doc.systemId);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void DocInFolder()
        {
            try
            {
                string idFolder = UIManager.ProjectManager.getProjectInSession().folderSelezionato.systemID;
                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();

                string msg = string.Empty;
                string idNumRecord = string.Empty;

                if (this.ListaDocPrivati != null && this.ListaDocPrivati.Count > 0)
                {
                    foreach (SchedaDocumento doc in this.ListaDocPrivati)
                    {
                        if (!UIManager.AddDocInProjectManager.addDocumentoInFolder(doc.docNumber, idFolder, infoUtente))
                        {

                        }
                    }
                }

                if (this.ListaDocUtente != null && this.ListaDocUtente.Count > 0)
                {
                    foreach (SchedaDocumento doc in this.ListaDocUtente)
                    {
                        if (!UIManager.AddDocInProjectManager.addDocumentoInFolder(doc.docNumber, idFolder, infoUtente))
                        {

                        }
                    }
                }

                this.HiddenIsPersonal.Value = string.Empty;
                this.upPnlGridIndexes.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InserimentoDocumentiPrivatiPersonali()
        {
            try
            {
                string msg = string.Empty;
                string idNumRecord = string.Empty;

                if (this.ListaDocPrivati != null && this.ListaDocPrivati.Count > 0)
                {
                    foreach (SchedaDocumento doc in this.ListaDocPrivati)
                    {
                        if (doc.protocollo != null && !string.IsNullOrEmpty(doc.protocollo.numero))
                        {
                            idNumRecord += doc.protocollo.numero + " - " + doc.protocollo.dataProtocollazione + " ";
                        }
                        else
                        {
                            idNumRecord += doc.docNumber + " - " + doc.dataCreazione + " ";
                        }
                    }

                    msg = "ConfirmAddDocInProjectPrivato";
                    string language = UIManager.UserManager.GetUserLanguage();
                    string msgTitleCnfirm = Utils.Languages.GetLabelFromCode("msgTitleCnfirm", language);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'HiddenIsPersonal', '" + msgTitleCnfirm + "','" + idNumRecord + "');", true);
                }

                if (this.ListaDocUtente != null && this.ListaDocUtente.Count > 0)
                {
                    foreach (SchedaDocumento doc in this.ListaDocUtente)
                    {
                        if (doc.protocollo != null && !string.IsNullOrEmpty(doc.protocollo.numero))
                        {
                            idNumRecord +=  doc.protocollo.numero + " - " + doc.protocollo.dataProtocollazione + " ";
                        }
                        else
                        {
                            idNumRecord +=  doc.docNumber + " - " + doc.dataCreazione + " ";
                        }
                    }

                    msg = "ConfirmAddDocInProjectPersonale";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'HiddenIsPersonal', 'Conferma','" + idNumRecord + "');", true);
                }
                this.upPnlGridIndexes.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #region varibaili di sessione
        private DocsPaWR.Templates Template
        {
            get
            {
                Templates result = null;
                if (HttpContext.Current.Session["template"] != null)
                {
                    result = HttpContext.Current.Session["template"] as Templates;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["template"] = value;
            }
        }

        private bool EnableStateDiagram
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableStateDiagram"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableStateDiagram"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableStateDiagram"] = value;
            }
        }

        private bool CustomDocuments
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["customDocuments"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["customDocuments"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["customDocuments"] = value;
            }
        }

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

        private bool EnableCodeObject
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableCodeObject"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableCodeObject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableCodeObject"] = value;
            }
        }


        private string TypeDocument
        {
            set
            {
                HttpContext.Current.Session["typeDoc"] = value;
            }
        }

        private bool AddDoc
        {
            set
            {
                HttpContext.Current.Session["AddDocInProject"] = value;
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

        private FiltroRicerca[][] filtroRicercaSession
        {
            get
            {
                return (FiltroRicerca[][])HttpContext.Current.Session["searchFilterDocInProject"];
            }
            set
            {
                HttpContext.Current.Session["searchFilterDocInProject"] = value;
            }
        }

        private List<SchedaDocumento> ListaDocPrivati
        {
            get
            {
                return (List<SchedaDocumento>)HttpContext.Current.Session["ListaDocPrivati"];
            }
            set
            {
                HttpContext.Current.Session["ListaDocPrivati"] = value;
            }
        }

        private List<SchedaDocumento> ListaDocUtente
        {
            get
            {
                return (List<SchedaDocumento>)HttpContext.Current.Session["ListaDocUtente"];
            }
            set
            {
                HttpContext.Current.Session["ListaDocUtente"] = value;
            }
        }

        private List<SchedaDocumento> ListaDocNonInseriti
        {
            get
            {
                return (List<SchedaDocumento>)HttpContext.Current.Session["ListaDocNonInseriti"];
            }
            set
            {
                HttpContext.Current.Session["ListaDocNonInseriti"] = value;
            }
        }

        private List<string> ListaIdProfNonInseriti
        {
            get
            {
                return (List<String>)HttpContext.Current.Session["ListaIdProfNonInseriti_20140828"];
            }
            set
            {
                HttpContext.Current.Session["ListaIdProfNonInseriti_20140828"] = value;
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


        private bool ricarica
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["ricarica"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["ricarica"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ricarica"] = value;
            }
        }

        private string[] IdProfileList
        {
            get
            {
                string[] result = null;
                if (HttpContext.Current.Session["idProfileList.addDocInProject"] != null)
                {
                    result = HttpContext.Current.Session["idProfileList.addDocInProject"] as string[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["idProfileList.addDocInProject"] = value;
            }

        }

        protected List<string> ListCheck
        {
            get
            {
                List<string> result = null;
                if (HttpContext.Current.Session["listCheck.addDocInProject"] != null)
                {
                    result = HttpContext.Current.Session["listCheck.addDocInProject"] as List<string>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listCheck.addDocInProject"] = value;
            }
        }

        protected bool CheckAll
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["checkAll.addDocInProject"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["checkAll.addDocInProject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["checkAll.addDocInProject"] = value;
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

        private bool SearchCorrespondentIntExtWithDisabled
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["searchCorrespondentIntExtWithDisabled"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["searchCorrespondentIntExtWithDisabled"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["searchCorrespondentIntExtWithDisabled"] = value;
            }
        }

        #endregion
    }


}
