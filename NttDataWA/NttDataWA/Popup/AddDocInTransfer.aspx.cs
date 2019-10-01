using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class AddDocInTransfer : System.Web.UI.Page
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
            this.btnAddDocInTransfer.Text = Utils.Languages.GetLabelFromCode("btnAddDocInTransfer", language);
            this.AddDocInProjectClose.Text = Utils.Languages.GetLabelFromCode("AddDocInProjectBtnClose", language);
            //            this.LblAddDocNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumProtocol", language);
            this.LblAddDocDa.Text = Utils.Languages.GetLabelFromCode("LblAddDocDa", language) + " ";
            this.LblAddDocA.Text = Utils.Languages.GetLabelFromCode("LblAddDocA", language) + " ";
            this.LblAddDocAnno.Text = Utils.Languages.GetLabelFromCode("LblAddDocAnno", language) + " ";
            //          this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocData", language);
            this.DocProt.Text = Utils.Languages.GetLabelFromCode("DocProt", language);
            this.DocNonProt.Text = Utils.Languages.GetLabelFromCode("DocNonProt", language);
            //this.Pred.Text = Utils.Languages.GetLabelFromCode("Pred", language);
            this.ADL.Text = Utils.Languages.GetLabelFromCode("ADL", language);
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
        }

        private void closePage(string _ParametroDiRitorno)
        {
            ScriptManager.RegisterClientScriptBlock(this.upPnlButtons, this.upPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('OpenAddDoc','" + _ParametroDiRitorno + "');", true);
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
                        {
                            this.LblAddDocNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumDoc", UIManager.UserManager.GetUserLanguage());
                            this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocDtaDoc", UIManager.UserManager.GetUserLanguage());
                            this.resetField();
                            this.plh_filtri.Visible = false;
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

                FiltroRicerca[][] filtriRicerca = UIManager.AddDocInProjectManager.RicercaDocDaFascicolare(this.rbl_TipoDoc.SelectedValue, idReg, this.txtAddDocDa.Text, this.txtAddDocA.Text, this.txtAddDocDataDA.Text, this.txtAddDocDataA.Text, this.txtAddDocAnno.Text, this.TxtObject.Text);
                this.filtroRicercaSession = filtriRicerca;
                this.SelectedPage = 1;
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
            if (string.IsNullOrEmpty(dataDa) && !string.IsNullOrEmpty(dataA))
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
                InfoDocumento[] infoDoc = UIManager.AddDocInProjectManager.getQueryInfoDocumentoPaging(infoutente, filtroRicerca, numeroPagina, out numTotPage, out nRec, true, false, true, true, out idProfileList);

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
                    this.btnAddDocInTransfer.Enabled = true;
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
                    this.btnAddDocInTransfer.Enabled = false;
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

        protected void AddDocInTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                this.InsertDocInSESSION();
                this.InserimentoDocumentiPrivatiPersonali();
                this.closePage("");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        /// <summary>
        /// Questo metodo inserisce il documento selezionato nella classifica richiesta 
        /// </summary>
        private void InsertDocInSESSION()
        {
            try
            {
                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                string msg = string.Empty;
                this.SetCheckBox();
                List<DocsPaWR.ARCHIVE_AUTH_grid_document> _lstDocs = new List<ARCHIVE_AUTH_grid_document>();
                if (Session["DocInSESSION"] != null)
                {
                    _lstDocs = (List<DocsPaWR.ARCHIVE_AUTH_grid_document>)Session["DocInSESSION"];
                }
                if (this.ListCheck != null && this.ListCheck.Count > 0)
                {

                    foreach (string idProfile in ListCheck)
                    {
                        string idNumRecord = string.Empty;
                        SchedaDocumento doc = DocumentManager.getDocumentDetails(this.Page, idProfile, idProfile);
                        if (doc != null)
                        {
                            DocsPaWR.ARCHIVE_AUTH_grid_document _tmpDoc = new ARCHIVE_AUTH_grid_document();
                            _tmpDoc.System_ID = Convert.ToInt32(doc.systemId);
                            if (doc.protocollo != null && !string.IsNullOrEmpty(doc.protocollo.numero))
                            {
                                idNumRecord += doc.protocollo.numero + " - " + doc.protocollo.dataProtocollazione + "<br/>";
                            }
                            else
                            {
                                idNumRecord += doc.docNumber + " - " + doc.dataCreazione + "<br/>";
                            }
                            _tmpDoc.ID_Protocollo = idNumRecord;
                            _tmpDoc.Registro = doc.registro != null ? doc.registro.codRegistro : "";
                            _tmpDoc.Tipo = doc.tipoProto;
                            _tmpDoc.Oggetto = doc.oggetto.descrizione;
                            _tmpDoc.Data = Convert.ToDateTime(doc.dataCreazione);
                            if (doc.tipoProto.Equals("P"))
                            {
                                _tmpDoc.Mittente_Destinatario = ((DocsPaWR.ProtocolloUscita)doc.protocollo).mittente.descrizione + " / " +
                                                                  ((DocsPaWR.ProtocolloUscita)doc.protocollo).destinatari[0].descrizione;
                            }
                            if (doc.tipoProto.Equals("I"))
                            {
                                _tmpDoc.Mittente_Destinatario = ((DocsPaWR.ProtocolloInterno)doc.protocollo).mittente.descrizione + " / " +
                                                                        ((DocsPaWR.ProtocolloUscita)doc.protocollo).destinatari[0].descrizione;
                            }
                            if (doc.tipoProto.Equals("A"))
                            {
                                _tmpDoc.Mittente_Destinatario = ((DocsPaWR.ProtocolloEntrata)doc.protocollo).mittente.descrizione;
                            }
                            if (doc.tipoProto.Equals("G"))
                            {
                                _tmpDoc.Mittente_Destinatario = "";
                            }
                            _lstDocs.Add(_tmpDoc);
                        }
                    }
                }
                Session["DocInSESSION"] = _lstDocs;
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
                List<DocsPaWR.ARCHIVE_AUTH_grid_document> _lstPrivateDoc = new List<ARCHIVE_AUTH_grid_document>();
                //Getto dalla session i dati appena inseriti per accodarli.
                if (Session["DocInSESSION"] != null)
                {
                    _lstPrivateDoc = (List<DocsPaWR.ARCHIVE_AUTH_grid_document>)Session["DocInSESSION"];
                }

                if (this.ListaDocPrivati != null && this.ListaDocPrivati.Count > 0)
                {
                    foreach (SchedaDocumento doc in this.ListaDocPrivati)
                    {
                        string idNumRecord = string.Empty;
                        DocsPaWR.ARCHIVE_AUTH_grid_document _tmpDoc = new ARCHIVE_AUTH_grid_document();
                        _tmpDoc.System_ID = Convert.ToInt32(doc.systemId);
                        if (doc.protocollo != null && !string.IsNullOrEmpty(doc.protocollo.numero))
                        {
                            idNumRecord += doc.protocollo.numero + " - " + doc.protocollo.dataProtocollazione + "<br/>";
                        }
                        else
                        {
                            idNumRecord += doc.docNumber + " - " + doc.dataCreazione + "<br/>";
                        }
                        _tmpDoc.ID_Protocollo = idNumRecord;
                        _tmpDoc.Registro = doc.registro.codRegistro;
                        _tmpDoc.Tipo = doc.tipoProto;
                        _tmpDoc.Oggetto = doc.oggetto.descrizione;
                        _tmpDoc.Data = Convert.ToDateTime(doc.dataCreazione);
                        if (doc.tipoProto.Equals("P"))
                        {
                            _tmpDoc.Mittente_Destinatario = ((DocsPaWR.ProtocolloUscita)doc.protocollo).mittente.descrizione + " / " +
                                                              ((DocsPaWR.ProtocolloUscita)doc.protocollo).destinatari[0].descrizione;
                        }
                        if (doc.tipoProto.Equals("I"))
                        {
                            _tmpDoc.Mittente_Destinatario = ((DocsPaWR.ProtocolloInterno)doc.protocollo).mittente.descrizione + " / " +
                                                                    ((DocsPaWR.ProtocolloUscita)doc.protocollo).destinatari[0].descrizione;
                        }
                        if (doc.tipoProto.Equals("A"))
                        {
                            _tmpDoc.Mittente_Destinatario = ((DocsPaWR.ProtocolloEntrata)doc.protocollo).mittente.descrizione;
                        }
                        if (doc.tipoProto.Equals("G"))
                        {
                            _tmpDoc.Mittente_Destinatario = "";
                        }
                        _lstPrivateDoc.Add(_tmpDoc);
                    }
                    Session["DocInSESSION"] = _lstPrivateDoc;
                }

                List<DocsPaWR.ARCHIVE_AUTH_grid_document> _lstUtenteDoc = new List<ARCHIVE_AUTH_grid_document>();
                //Getto dalla session i dati appena inseriti per accodarli.
                if (Session["DocInSESSION"] != null)
                {
                    _lstUtenteDoc = (List<DocsPaWR.ARCHIVE_AUTH_grid_document>)Session["DocInSESSION"];
                }

                if (this.ListaDocUtente != null && this.ListaDocUtente.Count > 0)
                {

                    foreach (SchedaDocumento doc in this.ListaDocUtente)
                    {
                        string idNumRecord = string.Empty;
                        DocsPaWR.ARCHIVE_AUTH_grid_document _tmpDoc = new ARCHIVE_AUTH_grid_document();
                        _tmpDoc.System_ID = Convert.ToInt32(doc.systemId);

                        if (doc.protocollo != null && !string.IsNullOrEmpty(doc.protocollo.numero))
                        {
                            idNumRecord += doc.protocollo.numero + " - " + doc.protocollo.dataProtocollazione + "<br/>";
                        }
                        else
                        {
                            idNumRecord += doc.docNumber + " - " + doc.dataCreazione + "<br/>";
                        }
                        _tmpDoc.ID_Protocollo = idNumRecord;
                        _tmpDoc.Registro = doc.registro.codRegistro;
                        _tmpDoc.Tipo = doc.tipoProto;
                        _tmpDoc.Oggetto = doc.oggetto.descrizione;
                        _tmpDoc.Data = Convert.ToDateTime(doc.dataCreazione);
                        if (doc.tipoProto.Equals("P"))
                        {
                            _tmpDoc.Mittente_Destinatario = ((DocsPaWR.ProtocolloUscita)doc.protocollo).mittente.descrizione + " / " +
                                                              ((DocsPaWR.ProtocolloUscita)doc.protocollo).destinatari[0].descrizione;
                        }
                        if (doc.tipoProto.Equals("I"))
                        {
                            _tmpDoc.Mittente_Destinatario = ((DocsPaWR.ProtocolloInterno)doc.protocollo).mittente.descrizione + " / " +
                                                                    ((DocsPaWR.ProtocolloUscita)doc.protocollo).destinatari[0].descrizione;
                        }
                        if (doc.tipoProto.Equals("A"))
                        {
                            _tmpDoc.Mittente_Destinatario = ((DocsPaWR.ProtocolloEntrata)doc.protocollo).mittente.descrizione;
                        }
                        if (doc.tipoProto.Equals("G"))
                        {
                            _tmpDoc.Mittente_Destinatario = "";
                        }
                        _lstUtenteDoc.Add(_tmpDoc);
                    }
                    Session["DocInSESSION"] = _lstUtenteDoc;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #region varibaili di sessione
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

        //private List<InfoDocumento> ListaDocNonInseriti
        //{
        //    get
        //    {
        //        return (List<InfoDocumento>)HttpContext.Current.Session["ListaDocNonInseriti"];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["ListaDocNonInseriti"] = value;
        //    }
        //}

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

        protected List<string> ListCheck
        {
            get
            {
                List<string> result = null;
                if (HttpContext.Current.Session["listCheck"] != null)
                {
                    result = HttpContext.Current.Session["listCheck"] as List<string>;
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

        #endregion
    }


}
