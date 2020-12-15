using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.Utils;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using System.Collections;
using System.Data;
using NttDatalLibrary;
using System.Web.UI.HtmlControls;
using NttDataWA.UserControls;

namespace NttDataWA.Search
{
    public partial class SearchTransmission : System.Web.UI.Page
    {

        private const string KEY_SCHEDA_RICERCA = "RicercaTrasmissioni";
        private const string UP_PANEL_BUTTONS = "upPnlButtons";
        private const string CLOSE_POPUP_SEARCH_PROJECT = "closePopupSearchProject";
        private const string CLOSE_POPUP_OPEN_TITOLARIO = "closePopupOpenTitolario";
        public SearchManager schedaRicerca = null;


        /// <summary>
        /// Salvataggio criteri di ricerca
        /// </summary>
        /// <param name="filters"></param>
        private void SaveSearchFilters(DocsPaWR.FiltroRicerca[][] filters)
        {
            SearchManager schedaRicerca = new SearchManager();
            schedaRicerca.FiltriRicerca = filters;
            Session[this.SchedaRicercaSessionKey] = schedaRicerca;
        }

        protected void SetAjaxAddressBook()
        {
            string dataUser = this.Role.systemId;
            if (this.Registry == null)
            {
                this.Registry = RegistryManager.GetRegistryInSession();
            }
            dataUser = dataUser + "-" + this.Registry.systemId;

            string callType = "CALLTYPE_RICERCA_TRASM_SOTTOPOSTO";
            this.RapidRole.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

            callType = "CALLTYPE_OWNER_AUTHOR";
            this.RapidCreatore.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;


        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                schedaRicerca = (SearchManager)Session[SearchManager.SESSION_KEY];
                if (schedaRicerca == null)
                {
                    //Inizializzazione della scheda di ricerca per la gestione delle ricerche salvate
                    schedaRicerca = new SearchManager(KEY_SCHEDA_RICERCA, UserManager.GetUserInSession(), RoleManager.GetRoleInSession(), this);
                    Session[SearchManager.SESSION_KEY] = schedaRicerca;
                }
                schedaRicerca.Pagina = this;

                if (!this.IsPostBack)
                {
                    this.InitializePage();

                    //Back
                    if (this.Request.QueryString["back"] != null && this.Request.QueryString["back"].Equals("1"))
                    {
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject obj = navigationList.Last();
                        if (!obj.CodePage.Equals(Navigation.NavigationUtils.NamePage.SEARCH_TRANSMISSIONS.ToString()) && !obj.CodePage.Equals(Navigation.NavigationUtils.NamePage.SEARCH_TRANSMISSIONS.ToString()))
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

                        this.BindFilterValues(schedaRicerca, false);
                        DocumentManager.setFiltroRicFasc(this, schedaRicerca.FiltriRicerca);

                        this.SearchTransmissionsAndDisplayResult(this.SelectedPage);

                        if (!string.IsNullOrEmpty(obj.OriginalObjectId))
                        {
                            foreach (GridViewRow grd in this.gridViewResult.Rows)
                            {
                                string idProject = string.Empty;
                                if ((Label)grd.FindControl("ID_OBJECT") != null)
                                {
                                    idProject = ((Label)grd.FindControl("ID_OBJECT")).Text;
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

                    if (UIManager.UserManager.IsAuthorizedFunctions("DO_SEARCH_TRASM_NO_LAVORATE"))
                    {
                        this.pnlNoLavorate.Visible = true;
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
                            this.SearchTransmissionsAndDisplayResult(this.SelectedPage);
                            this.BuildGridNavigator();
                            this.projectLblNumeroDocumenti.Text = this.GetLabel("projectLblNumeroTransm") + " (" + this.RecordCount.ToString() + ") " + this.GetLabel("projectLblNumeroTransmFound");
                            this.UpnlNumerodocumenti.Update();
                            this.UpnlGrid.Update();
                            this.upPnlGridIndexes.Update();
                        }
                            /*
                        else
                        {
                            this.ShowResult(this.Result);
                        }
                             * */
                     
                    }
                    this.ReadRetValueFromPopup();

                    this.ColorSelect();
                    this.UpPnlType.Update();
                }


                if (this.CustomDocuments)
                {
                    this.PnlTypeDocument.Controls.Clear();
                    if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                    {

                        if (this.Template == null || !this.Template.SYSTEM_ID.ToString().Equals(this.DocumentDdlTypeDocument.SelectedValue))
                        {
                            if (this.ddl_oggetto.SelectedIndex == 0)
                            {
                                this.Template = ProfilerDocManager.getTemplateById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                            }
                            else
                            {
                                this.Template = ProfilerProjectManager.getTemplateFascById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                            }
                        }
                        if (this.CustomDocuments)
                        {
                            this.PopulateProfiledDocument();
                        }
                    }
                }


                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ColorSelect()
        {
            if (!string.IsNullOrEmpty(this.TxtCodeRole.Text) && !string.IsNullOrEmpty(this.TxtDescriptionRole.Text) && select_sottoposto.Items.Count != 0)
            {
                //USATO PER COLORARE I CAMPI DELLA SELECT QUANDO RICARICA LA PAGINA
                if (this.Color != null && this.Color.Count > 0)
                {

                    foreach (int colorTemp in this.Color)
                    {
                        this.select_sottoposto.Items[colorTemp].Attributes.Add("style", "color:#990000");
                    }

                }
            }
        }

        protected void PopulateProfiledDocument()
        {
            this.PnlTypeDocument.Controls.Clear();
            this.inserisciComponenti(false);
        }

        private void inserisciComponenti(bool readOnly)
        {
            List<AssDocFascRuoli> dirittiCampiRuolo = new List<AssDocFascRuoli>();
            if (this.ddl_oggetto.SelectedIndex == 0)
            {
                dirittiCampiRuolo = ProfilerDocManager.getDirittiCampiTipologiaDoc(RoleManager.GetRoleInSession().idGruppo, this.Template.SYSTEM_ID.ToString());
            }
            else
            {
                ArrayList dirittiCampiRuoloTemp = ProfilerProjectManager.getDirittiCampiTipologiaFasc(UIManager.RoleManager.GetRoleInSession().idGruppo, this.Template.SYSTEM_ID.ToString());
                if (dirittiCampiRuoloTemp != null)
                {
                    dirittiCampiRuolo = new List<AssDocFascRuoli>(dirittiCampiRuoloTemp.ToArray(typeof(AssDocFascRuoli)) as AssDocFascRuoli[]);
                }
            }

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

            //UserControls.Calendar dataSottocontatoreDa = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            //dataSottocontatoreDa.fromUrl = "../UserControls/DialogCalendar.aspx";
            //dataSottocontatoreDa.CSS = "testo_grigio";
            //dataSottocontatoreDa.ID = "da_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            //dataSottocontatoreDa.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);

            //UserControls.Calendar dataSottocontatoreA = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            //dataSottocontatoreA.fromUrl = "../UserControls/DialogCalendar.aspx";
            //dataSottocontatoreA.CSS = "testo_grigio";
            //dataSottocontatoreA.ID = "a_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            //dataSottocontatoreA.VisibleTimeMode = ProfilazioneDocManager.getVisibleTimeMode(oggettoCustom);

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

            //if (!oggettoCustom.DATA_INSERIMENTO.Equals(""))
            //{
            //    if (oggettoCustom.DATA_INSERIMENTO.IndexOf("@") != -1)
            //    {
            //        string[] date = oggettoCustom.DATA_INSERIMENTO.Split('@');
            //        dataSottocontatoreDa.Text = date[0].ToString();
            //        dataSottocontatoreA.Text = date[1].ToString();
            //    }
            //    else
            //    {
            //        dataSottocontatoreDa.Text = oggettoCustom.DATA_INSERIMENTO.ToString();
            //        dataSottocontatoreA.Text = "";
            //    }
            //}

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

            //if (!oggettoCustom.DATA_INSERIMENTO.Equals(""))
            //{
            //    if (oggettoCustom.DATA_INSERIMENTO.IndexOf("@") != -1)
            //    {
            //        string[] date = oggettoCustom.DATA_INSERIMENTO.Split('@');
            //        dataSottocontatoreDa.Text = date[0].ToString();
            //        dataSottocontatoreA.Text = date[1].ToString();
            //    }
            //    else
            //    {
            //        dataSottocontatoreDa.Text = oggettoCustom.DATA_INSERIMENTO.ToString();
            //        dataSottocontatoreA.Text = "";
            //    }
            //}

            ////Imposto il contatore in funzione del formato
            //CustomTextArea contatore = new CustomTextArea();
            //CustomTextArea sottocontatore = new CustomTextArea();
            //CustomTextArea dataInserimentoSottocontatore = new CustomTextArea();
            //contatore.EnableViewState = true;
            //sottocontatore.EnableViewState = true;
            //dataInserimentoSottocontatore.EnableViewState = true;

            //contatore.ID = oggettoCustom.SYSTEM_ID.ToString();
            //sottocontatore.ID = oggettoCustom.SYSTEM_ID.ToString() + "_sottocontatore";
            //dataInserimentoSottocontatore.ID = oggettoCustom.SYSTEM_ID.ToString() + "_dataSottocontatore";
            //if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
            //{
            //    contatore.Text = oggettoCustom.FORMATO_CONTATORE;
            //    sottocontatore.Text = oggettoCustom.FORMATO_CONTATORE;

            //    if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE) && !string.IsNullOrEmpty(oggettoCustom.VALORE_SOTTOCONTATORE))
            //    {
            //        contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO);
            //        contatore.Text = contatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);

            //        sottocontatore.Text = sottocontatore.Text.Replace("ANNO", oggettoCustom.ANNO);
            //        sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_SOTTOCONTATORE);

            //        if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && oggettoCustom.ID_AOO_RF != "0")
            //        {
            //            Registro reg = UserManager.getRegistroBySistemId(this, oggettoCustom.ID_AOO_RF);
            //            if (reg != null)
            //            {
            //                contatore.Text = contatore.Text.Replace("RF", reg.codRegistro);
            //                contatore.Text = contatore.Text.Replace("AOO", reg.codRegistro);

            //                sottocontatore.Text = sottocontatore.Text.Replace("RF", reg.codRegistro);
            //                sottocontatore.Text = sottocontatore.Text.Replace("AOO", reg.codRegistro);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        contatore.Text = contatore.Text.Replace("ANNO", "");
            //        contatore.Text = contatore.Text.Replace("CONTATORE", "");
            //        contatore.Text = contatore.Text.Replace("RF", "");
            //        contatore.Text = contatore.Text.Replace("AOO", "");

            //        sottocontatore.Text = sottocontatore.Text.Replace("ANNO", "");
            //        sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", "");
            //        sottocontatore.Text = sottocontatore.Text.Replace("RF", "");
            //        sottocontatore.Text = sottocontatore.Text.Replace("AOO", "");
            //    }
            //    //}
            //}
            //else
            //{
            //    contatore.Text = oggettoCustom.VALORE_DATABASE;
            //    sottocontatore.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            //}

            //Panel divRowCounter = new Panel();
            //divRowCounter.CssClass = "row";
            //divRowCounter.EnableViewState = true;

            //Panel divColCountCounter = new Panel();
            //divColCountCounter.CssClass = "col_full";
            //divColCountCounter.EnableViewState = true;
            //divColCountCounter.Controls.Add(contatore);
            //divColCountCounter.Controls.Add(sottocontatore);
            //divRowCounter.Controls.Add(divColCountCounter);

            //if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
            //{
            //    dataInserimentoSottocontatore.Text = oggettoCustom.DATA_INSERIMENTO;

            //    Panel divColCountAbort = new Panel();
            //    divColCountAbort.CssClass = "col";
            //    divColCountAbort.EnableViewState = true;
            //    divColCountAbort.Controls.Add(dataInserimentoSottocontatore);
            //    divRowCounter.Controls.Add(divColCountAbort);
            //}

            //CheckBox cbContaDopo = new CheckBox();
            //cbContaDopo.EnableViewState = true;

            ////Verifico i diritti del ruolo sul campo
            //this.impostaDirittiRuoloContatoreSottocontatore(etichettaContatoreSottocontatore, contatore, sottocontatore, dataInserimentoSottocontatore, cbContaDopo, etichettaDDL, ddl, oggettoCustom, this.Template, dirittiCampiRuolo);

            //if (etichettaContatoreSottocontatore.Visible)
            //{
            //    this.PnlTypeDocument.Controls.Add(divRowDesc);
            //}

            //contatore.ReadOnly = true;
            //contatore.CssClass = "txt_input_half";
            //contatore.CssClassReadOnly = "txt_input_half_disabled";

            //sottocontatore.ReadOnly = true;
            //sottocontatore.CssClass = "txt_input_half";
            //sottocontatore.CssClassReadOnly = "txt_input_half_disabled";

            //dataInserimentoSottocontatore.ReadOnly = true;
            //dataInserimentoSottocontatore.CssClass = "txt_input_full";
            //dataInserimentoSottocontatore.CssClassReadOnly = "txt_input_full_disabled";
            //dataInserimentoSottocontatore.Visible = false;


            ////Inserisco il cb per il conta dopo
            //if (oggettoCustom.CONTA_DOPO == "1")
            //{
            //    cbContaDopo.ID = oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo";
            //    cbContaDopo.Checked = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
            //    cbContaDopo.ToolTip = "Attiva / Disattiva incremento del contatore al salvataggio dei dati.";

            //    Panel divColCountAfter = new Panel();
            //    divColCountAfter.CssClass = "col";
            //    divColCountAfter.EnableViewState = true;
            //    divColCountAfter.Controls.Add(cbContaDopo);
            //    divRowDll.Controls.Add(divColCountAfter);
            //}

            //if (paneldll)
            //{
            //    this.PnlTypeDocument.Controls.Add(divRowEtiDll);
            //    this.PnlTypeDocument.Controls.Add(divRowDll);
            //}

            //if (contatore.Visible || sottocontatore.Visible)
            //{
            //    this.PnlTypeDocument.Controls.Add(divRowCounter);
            //}
        }

        private void inserisciLink(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            UserControls.LinkDocFasc link = (UserControls.LinkDocFasc)this.LoadControl("../UserControls/LinkDocFasc.ascx");
            link.EnableViewState = true;
            link.ID = oggettoCustom.SYSTEM_ID.ToString();
            link.IsEsterno = (oggettoCustom.TIPO_LINK.Equals("ESTERNO"));
            link.IsFascicolo = ("FASCICOLO".Equals(oggettoCustom.TIPO_OBJ_LINK));

            link.TxtEtiLinkDocOrFasc = oggettoCustom.DESCRIZIONE;

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(link.TxtEtiLinkDocOrFasc, link, oggettoCustom, this.Template, dirittiCampiRuolo);

            link.Value = oggettoCustom.VALORE_DATABASE;

            if (link.Visible)
            {
                this.PnlTypeDocument.Controls.Add(link);
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
                    ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
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

        public void impostaDirittiRuoloContatore(Object etichettaContatore, Object contatoreDa, Object contatoreA, Object etichettaContatoreDa, Object etichettaContatoreA, Object etichettaDDL, Object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
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

        private int impostaSelezioneEsclusiva(string valore, RadioButtonList rbl)
        {
            for (int i = 0; i < rbl.Items.Count; i++)
            {
                if (rbl.Items[i].Text == valore)
                    return i;
            }
            return 0;
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
                        ((DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i]).VALORE_DATABASE = "-1";
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public void impostaDirittiRuoloSelezioneEsclusiva(Object etichetta, Object campo, Object button, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
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

        private int impostaSelezioneMenuATendina(string valore, DropDownList ddl)
        {
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Text == valore)
                    return i;
            }
            return 0;
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

        public void impostaDirittiRuoloSulCampo(Object etichetta, Object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
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

        public void PopulateDDLSavedSearches()
        {

            schedaRicerca.ElencoRicerche("T", true, this.DdlRapidSearch);

            if (Session["idRicercaSalvata"] != null)
            {
                Session["itemUsedSearch"] = this.DdlRapidSearch.Items.IndexOf(this.DdlRapidSearch.Items.FindByValue(Session["idRicercaSalvata"].ToString()));
                Session["idRicercaSalvata"] = null;
            }

            this.BindFilterValues(schedaRicerca, false);
        }

        protected void ddl_oggetto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();

                if (this.ddl_oggetto.SelectedIndex == 0)
                {
                    this.ddl_tipo_doc.Visible = true;
                    this.PnlTypeDocument.Controls.Clear();
                    this.LoadTypeDocuments();
                    this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("LitSearchTransTypeDoc", language);
                    this.PlcDocInWorking.Visible = true;
                    this.SearchTransmissionExportExcel.Visible = true;
                    this.UpnlAzioniMassive.Update();
                }
                else
                {
                    this.ddl_tipo_doc.Visible = false;
                    this.PlcDocInWorking.Visible = false;
                    this.PnlTypeDocument.Controls.Clear();
                    this.LoadTypeProjects();
                    this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("LitSearchTransTypePrj", language);
                    this.SearchTransmissionExportExcel.Visible = true;
                    this.UpnlAzioniMassive.Update();
                }

                this.UpTotalTypeDocument.Update();
                this.UpPnlTypeDocument.Update();
                this.UpPnlObject.Update();
                this.UpPnlDocInWorking.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DdlRapidSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.DdlRapidSearch.SelectedIndex == 0)
                {
                    this.SearchDocumentAdvancedRemove.Enabled = false;

                    if (GridManager.IsRoleEnabledToUseGrids())
                    {
                        GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Transmission);
                    }
                    return;
                }
                else
                {
                    //this.ResetField();
                }

                try
                {
                    string gridTempId = string.Empty;
                    
                    schedaRicerca.Seleziona(Int32.Parse(this.DdlRapidSearch.SelectedValue), out gridTempId);

                    try
                    {
                        if (this.DdlRapidSearch.SelectedIndex > 0)
                        {
                            Session.Add("itemUsedSearch", this.DdlRapidSearch.SelectedIndex.ToString());
                        }
                        DocsPaWR.FiltroRicerca typeTrasm = (from item in schedaRicerca.FiltriRicerca[0] where item.argomento.Equals(DocsPaWR.FiltriTrasmissioneNascosti.RICEVUTE_EFFETTUATE.ToString()) select item).FirstOrDefault();
                        if (typeTrasm != null && !string.IsNullOrEmpty(typeTrasm.valore))
                        {
                            if (typeTrasm.valore.Equals("E"))
                                this.TransmLinkEffettuated_Click(null, null);
                            else
                                this.TransmLinkReceived_Click(null, null);
                            this.UpPnlRapidSearch.Update();
                            this.UpSearchDocumentTabs.Update();
                            this.UpPnlType.Update();
                        }

                        this.BindFilterValues(schedaRicerca, true);
                        DocumentManager.setFiltroRicFasc(this, schedaRicerca.FiltriRicerca);

                        this.SearchDocumentAdvancedRemove.Enabled = true;
                        this.upPnlButtons.Update();

                        if (this.CustomDocuments)
                        {
                            this.PnlTypeDocument.Controls.Clear();
                            if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                            {
                                if (this.Template == null || !this.Template.SYSTEM_ID.ToString().Equals(this.DocumentDdlTypeDocument.SelectedValue))
                                {
                                    if (this.ddl_oggetto.SelectedIndex == 0)
                                    {
                                        this.Template = ProfilerDocManager.getTemplateById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                                    }
                                    else
                                    {
                                        this.Template = ProfilerProjectManager.getTemplateFascById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                                    }
                                }
                                if (this.CustomDocuments)
                                {
                                    this.PopulateProfiledDocument();
                                    this.UpPnlTypeDocument.Update();
                                }
                            }
                        }

                        this.SearchTransmissionSearch_Click(null, null);

                    }
                    catch (Exception ex_)
                    {
                        string msg = utils.FormatJs(ex_.Message);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectRemoveCriteria', 'error', '', '" + msg + "');", true);
                    }
                }
                catch (Exception ex)
                {
                    string msg = utils.FormatJs(ex.Message);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCustom', 'error', '', '" + msg + "');", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void SearchTransmissionAdvancedRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.DdlRapidSearch.SelectedIndex > 0)
                {
                    string id = this.DdlRapidSearch.SelectedValue;
                    DocsPaWR.SearchItem item = SearchManager.GetItemSearch(Int32.Parse(id));

                    DocsPaWR.Ruolo ruolo = null;
                    if (item.owner_idGruppo != 0)
                        ruolo = RoleManager.GetRoleInSession();

                    string msg = "Il criterio di ricerca con nome '" + this.DdlRapidSearch.SelectedItem.ToString() + "' verrà rimosso.<br />";
                    msg += (ruolo != null) ? "Attenzione! Il criterio di ricerca è condiviso con il ruolo '" + ruolo.descrizione + "'.<br />" : "";
                    msg += "Confermi l'operazione?";
                    msg = utils.FormatJs(msg);

                    if (this.Session["itemUsedSearch"] != null)
                        Session.Remove("itemUsedSearch");

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('ErrorCustom', 'HiddenRemoveUsedSearch', '', '" + msg + "');", true);
                }
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

                    if (this.Session["itemUsedSearch"] != null)
                    {
                        this.DdlRapidSearch.SelectedIndex = Convert.ToInt32(this.Session["itemUsedSearch"]);
                        this.UpPnlRapidSearch.Update();
                    }
                    else if (this.Session["idRicercaSalvata"] != null)
                    {
                        this.DdlRapidSearch.Items.FindByValue(this.Session["idRicercaSalvata"].ToString()).Selected = true;
                        this.UpPnlRapidSearch.Update();
                    }

                    try
                    {
                        bool cercaInferiori = true;

                        this.chk_me_stesso.Checked = false;
                        this.chk_mio_ruolo.Checked = false;

                        foreach (DocsPaWR.FiltroRicerca item in schedaRicerca.FiltriRicerca[0])
                        {
                            // Ripristino filtro su tipologia di oggetto trasmesso (doc o fascicolo)
                            this.RestoreFiltersTipoOggettoTrasmesso(item);

                            // Ripristino filtro mittente / destinatario
                            this.RestoreFiltersMittenteDestinatario(item);

                            // Ripristino filtro mittente / destinatario sottoposto
                            this.RestoreFiltersMittenteDestinatarioSottoposto(item);

                            // Ripristino filtri data trasmissione
                            this.RestoreFiltersDataTrasmissione(item);

                            // Ripristino filtro ragione
                            this.RestoreRagioneTrasmissione(item);

                            // Ripristino filtro su data accettazione / rifiuto
                            this.RestoreFiltersAccettateRifiutate(item);

                            this.RestoreFiltersNonLavorateDaUtenteNotificato(item);

                            // Ripristino filtro su pannello Completamento
                            this.RestoreFilterCompletamento(item);

                            // Ripristino filtri su checkbox
                            this.RestoreFilterMeStessoRuoli(item);

                            // Ripristino filtri custom
                            this.RestoreCustomFilters(item);

                            // Ripristino filtri diagramma di stato
                            this.RestoreDiagrammaDiStato(item);

                            // Ripristino filtri tipologia documento
                            this.RestoreTipologiaDocumento(item);

                            //Ripristino filtri tipologia fascicolo
                            this.RestoreTipologiaFascicolo(item);

                            //Ripristino filtro Diagramma Stato Fascicolo
                            this.RestoreDiagrammaDiStatoFasc(item);

                            // Ripristino su filtro "visualizzazione sottoposti"
                            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.NO_CERCA_INFERIORI.ToString())
                                if (item.valore.ToUpper().Equals("TRUE"))
                                    cercaInferiori = false;

                            if (item.argomento.Equals(DocsPaWR.FiltriTrasmissioneNascosti.RICEVUTE_EFFETTUATE.ToString()))
                            {
                                this.TypeSearch = item.valore;
                            }

                            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.FIRMATO.ToString())
                            {

                                if (item.valore.Equals("1"))
                                {
                                    this.chk_firmati.Checked = true;
                                }
                                else
                                {
                                    this.chk_non_firmati.Checked = true;
                                }

                                this.PlcWithImage.Visible = true;
                                this.PlcWithImage.Attributes.Remove("class");
                                this.UpPnlDocInWorking.Update();
                            }

                            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TIPO_FILE_ACQUISITO.ToString())
                            {
                                this.ddl_tipoFileAcquisiti.Items.Clear();
                                this.ddl_tipoFileAcquisiti.SelectedValue = item.valore;
                            }

                            // Ripristino flags ricerca estesa agli storicizzati per ruoli mittente e destinatario
                            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString())
                                this.chkCreatoreExtendHistoricized.Checked = Boolean.Parse(item.valore);
                            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.RUOLO_EXTEND_TO_HISTORICIZED.ToString())
                                this.chkHistoricizedRole.Checked = Boolean.Parse(item.valore);

                        }

                        if (this.chk_mio_ruolo.Checked)
                        {
                            this.select_sottoposto.Enabled = true;
                            this.DocumentImgRoleAddressBook.Visible = true;
                        }
                        else
                        {
                            this.TxtCodeRole.Text = string.Empty;
                            this.TxtDescriptionRole.Text = string.Empty;
                            this.select_sottoposto.Items.Clear();
                            this.select_sottoposto.Enabled = false;
                            this.DocumentImgRoleAddressBook.Visible = false;
                        }

                        if (this.chk_visSott.Visible)
                            this.chk_visSott.Checked = cercaInferiori;
                    }
                    catch (Exception)
                    {
                        throw new Exception("I criteri di ricerca non sono piu\' validi.");
                    }
                }
                catch (System.Exception ex)
                {
                    ErrorManager.redirect(this, ex);
                }
            }
        }

        private void RestoreDiagrammaDiStatoFasc(DocsPaWR.FiltroRicerca item)
        {
            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DIAGRAMMA_STATO_FASC.ToString())
            {
                this.ddlStateCondition.Visible = true;
                this.PnlStateDiagram.Visible = true;
                this.ddlStateCondition.SelectedValue = item.nomeCampo;
                this.DocumentDdlStateDiagram.SelectedValue = item.valore;
                this.UpPnlTypeDocument.Update();
            }
        }

        private void RestoreTipologiaDocumento(DocsPaWR.FiltroRicerca item)
        {
            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TIPO_ATTO.ToString())
            {
                this.DocumentDdlTypeDocument.SelectedValue = item.valore;
                //Verifico se esiste un diagramma di stato associato al tipo di documento
                //Modifica per la visualizzazione solo degli stati per cui esistono documenti in essi
                string idDiagramma = DiagrammiManager.getDiagrammaAssociato(this.DocumentDdlTypeDocument.SelectedValue).ToString();

                if (this.DocumentDdlTypeDocument.Items.FindByValue(item.valore) != null)
                {
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
                            ListItem itemList = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                            this.DocumentDdlStateDiagram.Items.Add(itemList);
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

                this.UpPnlTypeDocument.Update();
            }
        }

        private void RestoreTipologiaFascicolo(DocsPaWR.FiltroRicerca item)
        {
            // Ripristino filtro "Tipologia fascicoli"
            if (item.argomento == DocsPaWR.FiltriFascicolazione.TIPOLOGIA_FASCICOLO.ToString())
            {
                this.DocumentDdlTypeDocument.SelectedValue = item.valore;
                //Verifico se esiste un diagramma di stato associato al tipo di documento
                //Modifica per la visualizzazione solo degli stati per cui esistono documenti in essi
                string idDiagramma = DiagrammiManager.getDiagrammaAssociatoFasc(this.DocumentDdlTypeDocument.SelectedValue).ToString();

                if (this.DocumentDdlTypeDocument.Items.FindByValue(item.valore) != null)
                {
                    if (!string.IsNullOrEmpty(idDiagramma) && !idDiagramma.Equals("0"))
                    {
                        this.PnlStateDiagram.Visible = true;

                        //Inizializzazione comboBox
                        this.DocumentDdlStateDiagram.Items.Clear();
                        ListItem itemEmpty = new ListItem();
                        this.DocumentDdlStateDiagram.Items.Add(itemEmpty);

                        DocsPaWR.Stato[] statiDg = DiagrammiManager.getStatiPerRicerca(idDiagramma, "F");
                        foreach (Stato st in statiDg)
                        {
                            ListItem itemList = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                            this.DocumentDdlStateDiagram.Items.Add(itemList);
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

            // Ripristino filtro "Tipologia fascicoli"
            if (item.argomento == DocsPaWR.FiltriFascicolazione.PROFILAZIONE_DINAMICA.ToString())
            {
                this.Template = item.template;
                this.UpPnlTypeDocument.Update();
            }
        }

        /// <summary>
        /// Ripristino filtro diagramma di stato
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private void RestoreDiagrammaDiStato(DocsPaWR.FiltroRicerca item)
        {
            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DIAGRAMMA_STATO_DOC.ToString())
            {
                this.ddlStateCondition.Visible = true;
                this.PnlStateDiagram.Visible = true;
                this.ddlStateCondition.SelectedValue = item.nomeCampo;
                this.DocumentDdlStateDiagram.SelectedValue = item.valore;
                this.UpPnlTypeDocument.Update();
            }
        }

        /// <summary>
        /// Ripristino valori filtri custom
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dropDown"></param>
        /// <param name="textBox"></param>
        /// <returns></returns>

        private bool RestoreCustomFilters(DocsPaWR.FiltroRicerca item)
        {
            Boolean result = false;

            if (item.argomento.Equals("NOTE_GENERALI"))
            {
                this.txt_note_generali.Text = item.valore;
                result = true;
            }

            if (item.argomento.Equals("NOTE_INDIVIDUALI"))
            {
                this.txt_note_individuali.Text = item.valore;
                result = true;
            }


            if (item.argomento.Equals("SCADENZA_SUCCESSIVA_AL") || item.argomento.Equals("SCADENZA_IL"))
            {
                this.cld_scadenza_al.Text = item.valore;
                result = true;
            }

            if (item.argomento.Equals("SCADENZA_PRECEDENTE_IL"))
            {
                this.cld_scadenza_dal.Text = item.valore;
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Ripristino filtro su checkbox me stesso e ruolo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private void RestoreFilterMeStessoRuoli(DocsPaWR.FiltroRicerca item)
        {

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.A_ME_STESSO.ToString())
            {
                if (item.valore.Equals("true"))
                {
                    this.chk_me_stesso.Checked = true;
                }
            }

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.AL_MIO_RUOLO.ToString())
            {
                if (item.valore.Equals("true"))
                {
                    this.chk_mio_ruolo.Checked = true;
                }
            }
        }

        private void RestoreFilterCompletamento(DocsPaWR.FiltroRicerca item)
        {
            if (item != null)
            {
                if (item.argomento.Equals(DocsPaWR.FiltriTrasmissioneNascosti.DA_PROTOCOLLARE.ToString()))
                {
                    this.P_Prot.Checked = true;
                }
                else if (item.argomento.Equals(DocsPaWR.FiltriTrasmissioneNascosti.MANCANZA_FASCICOLAZIONE.ToString()))
                {
                    this.M_Fasc.Checked = true;
                }
                else if (item.argomento.Equals(DocsPaWR.FiltriTrasmissioneNascosti.MANCANZA_IMMAGINE.ToString()))
                {
                    this.M_Img.Checked = true;
                    this.PlcWithImage.Visible = false;
                    this.PlcWithImage.Attributes.Remove("class");
                    this.UpPnlDocInWorking.Update();

                }
                else if (item.argomento.Equals(DocsPaWR.FiltriTrasmissioneNascosti.CON_IMMAGINE.ToString()))
                {
                    this.M_si_img.Checked = true;
                    this.chk_firmati.Visible = true;
                    this.chk_non_firmati.Visible = true;
                    this.PlcWithImage.Visible = true;
                    this.PlcWithImage.Attributes.Remove("class");
                    this.UpPnlDocInWorking.Update();
                }

            }
        }

        /// <summary>
        /// Ripristino filtro su data accettazione / rifiuto
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersAccettateRifiutate(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;
            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.PENDENTI.ToString())
            {
                this.cbx_Pendenti.Checked = (item.valore.IndexOf("P") > -1);
                retValue = true;
            }

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.ACCETTATA_RIFIUTATA.ToString())
            {
                bool accettate = (item.valore.IndexOf("A") > -1);
                bool rifiutate = (item.valore.IndexOf("R") > -1);

                this.cbx_Acc.Checked = accettate;
                this.cbx_Rif.Checked = rifiutate;

                this.impostaAccettateRifiutate();

                retValue = true;
            }

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO.ToString())
            {
                this.dataUno_TAR.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_DA.ToString() ||
                     item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_DA.ToString() ||
                     item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_DA.ToString())
            {
                // Azione di selezione tipo filtro per data
                this.ddl_TAR.SelectedValue = "0";
                this.PerformActionSelectTAR();
                this.dataUno_TAR.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_A.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_A.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_A.ToString())
            {
                // Azione di selezione tipo filtro per data
                this.ddl_TAR.SelectedValue = "1";
                this.PerformActionSelectTAR();
                this.dataDue_TAR.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_TODAY.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_TODAY.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_TODAY.ToString())
            {
                this.ddl_TAR.SelectedValue = "2";
                this.PerformActionSelectTAR();
                this.dataUno_TAR.Visible = true;
                this.dataUno_TAR.Text = DocumentManager.toDay();
                this.dataUno_TAR.Visible = true;
                this.dataUno_TAR.Enabled = false;
                this.dataUno_TAR.Visible = true;
                this.dataUno_TAR.Enabled = false;
                this.dataDue_TAR.Visible = false;
                this.dataDue_TAR.Visible = false;
                this.dataDue_TAR.Visible = false;
                this.LiSearchData_1.Visible = false;
                this.LiSearchData_2.Visible = false;

                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_SC.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_SC.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_SC.ToString())
            {
                this.ddl_TAR.SelectedValue = "3";
                this.dataUno_TAR.Text = DocumentManager.getFirstDayOfWeek();
                this.dataUno_TAR.Enabled = false;
                this.dataUno_TAR.Enabled = false;
                this.dataDue_TAR.Visible = true;
                this.dataDue_TAR.Text = DocumentManager.getLastDayOfWeek();
                this.dataDue_TAR.Visible = true;
                this.dataDue_TAR.Visible = true;
                this.dataDue_TAR.Enabled = false;
                this.dataDue_TAR.Enabled = false;
                this.LiSearchData_1.Visible = true;
                this.LiSearchData_2.Visible = true;

                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_MC.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_MC.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_MC.ToString())
            {
                this.ddl_TAR.SelectedValue = "4";
                this.dataUno_TAR.Text = DocumentManager.getFirstDayOfMonth();
                this.dataUno_TAR.Enabled = false;
                this.dataUno_TAR.Enabled = false;
                this.dataDue_TAR.Visible = true;
                this.dataDue_TAR.Text = DocumentManager.getLastDayOfMonth();
                this.dataDue_TAR.Visible = true;
                this.dataDue_TAR.Visible = true;
                this.dataDue_TAR.Enabled = false;
                this.dataDue_TAR.Enabled = false;
                this.LiSearchData_1.Visible = true;
                this.LiSearchData_2.Visible = true;

                retValue = true;
            }

            return retValue;
        }

        protected void impostaAccettateRifiutate()
        {
            //Nessuna selezione
            if (!cbx_Acc.Checked && !cbx_Rif.Checked && !cbx_Pendenti.Checked)
            {
                ddl_TAR.SelectedIndex = 0;
                ddl_TAR.Enabled = false;
                LiSearchData_1.Visible = false;
                this.dataUno_TAR.Enabled = false;
                this.dataUno_TAR.Visible = false;
                this.dataUno_TAR.Text = "";
                LiSearchData_2.Visible = false;
                this.dataDue_TAR.Visible = false;
                this.dataDue_TAR.Visible = false;
                this.dataDue_TAR.Text = "";
            }

            //Accettate
            if (cbx_Acc.Checked)
            {
                ddl_TAR.Enabled = true;
                this.dataUno_TAR.Enabled = true;
                this.dataUno_TAR.Visible = true;
            }

            //Rifiutate
            if (cbx_Rif.Checked)
            {
                ddl_TAR.Enabled = true;
                this.dataUno_TAR.Enabled = true;
                this.dataUno_TAR.Visible = true;
            }

            //Accettate-Rifiutate
            if (cbx_Acc.Checked && cbx_Rif.Checked)
            {
                ddl_TAR.Enabled = true;
                this.dataUno_TAR.Enabled = true;
                this.dataUno_TAR.Visible = true;
            }

            //Pendenti
            if (cbx_Pendenti.Checked)
            {
                cbx_Acc.Checked = false;
                cbx_Rif.Checked = false;
                ddl_TAR.SelectedIndex = 0;
                ddl_TAR.Enabled = false;
                LiSearchData_1.Visible = false;
                this.dataUno_TAR.Enabled = false;
                this.dataUno_TAR.Visible = false;
                this.dataUno_TAR.Text = "";
                LiSearchData_2.Visible = false;
                this.dataDue_TAR.Visible = false;
                this.dataDue_TAR.Visible = false;
                this.dataDue_TAR.Text = "";
            }
        }


        /// <summary>
        /// Ripristino filtro su data accettazione / rifiuto
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersNonLavorateDaUtenteNotificato(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.NON_LAVORATE_DA_UTENTE_NOTIFICATO.ToString())
            {
                this.cbx_no_lavorate.Checked = true;

                retValue = true;
            }

            return retValue;
        }

        /// <summary>
        /// Azione di selezione data accettazione / rifiuto
        /// </summary>
        private void PerformActionSelectTAR()
        {
            switch (ddl_TAR.SelectedItem.Text)
            {
                case "Valore Singolo":
                    this.dataUno_TAR.Visible = true;
                    this.dataUno_TAR.Visible = true;
                    this.dataUno_TAR.Enabled = true;
                    this.dataUno_TAR.Visible = true;
                    this.dataUno_TAR.Enabled = true;
                    this.dataDue_TAR.Visible = false;
                    this.dataDue_TAR.Visible = false;
                    this.dataDue_TAR.Visible = false;
                    this.LiSearchData_1.Visible = false;
                    this.LiSearchData_2.Visible = false;
                    break;
                case "Intervallo":
                    this.dataUno_TAR.Visible = true;
                    this.dataUno_TAR.Visible = true;
                    this.dataUno_TAR.Enabled = true;
                    this.dataUno_TAR.Visible = true;
                    this.dataUno_TAR.Enabled = true;
                    this.dataDue_TAR.Visible = true;
                    this.dataDue_TAR.Visible = true;
                    this.dataDue_TAR.Enabled = true;
                    this.dataDue_TAR.Visible = true;
                    this.dataDue_TAR.Enabled = true;
                    this.LiSearchData_1.Visible = true;
                    this.LiSearchData_2.Visible = true;
                    break;

                case "Oggi":
                    this.dataUno_TAR.Visible = true;
                    this.dataUno_TAR.Visible = true;
                    this.dataUno_TAR.Enabled = false;
                    this.dataUno_TAR.Visible = true;
                    this.dataUno_TAR.Text = DocumentManager.toDay();
                    this.dataUno_TAR.Enabled = false;
                    this.dataDue_TAR.Visible = false;
                    this.dataDue_TAR.Visible = false;
                    this.dataDue_TAR.Visible = false;
                    this.LiSearchData_1.Visible = false;
                    this.LiSearchData_2.Visible = false;
                    break;

                case "Settimana Corr.":
                    this.dataUno_TAR.Visible = true;
                    this.dataUno_TAR.Enabled = false;
                    this.dataUno_TAR.Text = DocumentManager.getFirstDayOfWeek();
                    this.dataUno_TAR.Enabled = false;
                    this.dataDue_TAR.Visible = true;
                    this.dataDue_TAR.Visible = true;
                    this.dataDue_TAR.Enabled = false;
                    this.dataDue_TAR.Text = DocumentManager.getLastDayOfWeek();
                    this.dataDue_TAR.Visible = true;
                    this.dataDue_TAR.Enabled = false;
                    this.LiSearchData_1.Visible = true;
                    this.LiSearchData_2.Visible = true;
                    break;

                case "Mese Corrente":
                    this.dataUno_TAR.Visible = true;
                    this.dataUno_TAR.Enabled = false;
                    this.dataUno_TAR.Text = DocumentManager.getFirstDayOfMonth();
                    this.dataUno_TAR.Enabled = false;
                    this.dataDue_TAR.Visible = true;
                    this.dataDue_TAR.Visible = true;
                    this.dataDue_TAR.Enabled = false;
                    this.dataDue_TAR.Text = DocumentManager.getLastDayOfMonth();
                    this.dataDue_TAR.Enabled = false;
                    this.LiSearchData_1.Visible = true;
                    this.LiSearchData_2.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterItem"></param>
        /// <returns></returns>
        private bool RestoreRagioneTrasmissione(DocsPaWR.FiltroRicerca filterItem)
        {
            if (filterItem.argomento == DocsPaWR.FiltriTrasmissioneNascosti.RAGIONE.ToString())
            {
                foreach (ListItem item in this.ddl_ragioni.Items)
                {
                    item.Selected = false;
                    if (item.Text == filterItem.valore)
                    {
                        item.Selected = true;
                        this.PerformActionSelectRagioniTrasmissione();
                        return true;
                    }
                }
            }

            this.UpPnlTransmReason.Update();
            return false;
        }

        /// <summary>
        /// Azione di selezione ragione trasmissione
        /// </summary>
        private void PerformActionSelectRagioniTrasmissione()
        {
            DocsPaWR.RagioneTrasmissione ragione;
            if (ddl_ragioni.SelectedIndex > 0)
            {
                ragione = this.ListaRagioni[ddl_ragioni.SelectedIndex - 1];

                if (ragione.tipo.Equals("W"))
                {

                }
                else
                {
                    this.cbx_Pendenti.Checked = false;
                    this.cbx_Acc.Checked = false;
                    this.cbx_Rif.Checked = false;
                    this.ddl_TAR.SelectedIndex = 0;
                    this.ddl_TAR.Enabled = false;
                    this.LiSearchData_1.Visible = false;
                    this.dataUno_TAR.Enabled = false;
                    this.dataUno_TAR.Visible = false;
                    this.dataUno_TAR.Text = string.Empty;
                    this.LiSearchData_2.Visible = false;
                    this.dataDue_TAR.Visible = false;
                    this.dataDue_TAR.Visible = false;
                    this.dataDue_TAR.Text = string.Empty;
                }

                this.UpPnlOtherFilters.Update();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="filterItem"></param>
        /// <param name="filterType"></param>
        private bool RestoreTextBoxValue(CustomTextArea textBox, DocsPaWR.FiltroRicerca filterItem, Enum filterType)
        {
            bool retValue = false;

            if (filterItem.argomento == filterType.ToString())
            {
                textBox.Text = filterItem.valore;
                retValue = true;
            }

            return retValue;
        }

        /// <summary>
        /// Ripristino filtri data trasmissione
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersDataTrasmissione(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_SUCCESSIVA_AL.ToString())
            {
                this.ddl_dataTrasm.SelectedValue = "1";
                this.PerformActionSelectDataTrasmissione();
                this.RestoreTextBoxValue(this.txt_initDataTrasm, item, DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_SUCCESSIVA_AL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_PRECEDENTE_IL.ToString())
            {
                this.RestoreTextBoxValue(this.txt_fineDataTrasm, item, DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_PRECEDENTE_IL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_IL.ToString())
            {
                this.ddl_dataTrasm.SelectedValue = "0";
                this.PerformActionSelectDataTrasmissione();

                this.RestoreTextBoxValue(this.txt_initDataTrasm, item, DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_IL);
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_TODAY.ToString())
            {
                this.ddl_dataTrasm.SelectedIndex = 2;
                this.txt_initDataTrasm.Visible = true;
                this.txt_initDataTrasm.Text = DocumentManager.toDay();
                this.txt_initDataTrasm.Visible = true;
                this.txt_initDataTrasm.Enabled = false;
                this.txt_initDataTrasm.Visible = true;
                this.txt_initDataTrasm.Enabled = false;
                this.txt_fineDataTrasm.Visible = false;
                this.txt_fineDataTrasm.Visible = false;
                this.txt_fineDataTrasm.Visible = false;
                this.lbl_initdataTrasm.Visible = false;
                this.lbl_finedataTrasm.Visible = false;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_SC.ToString())
            {
                this.ddl_dataTrasm.SelectedIndex = 3;
                this.txt_initDataTrasm.Text = DocumentManager.getFirstDayOfWeek();
                this.txt_initDataTrasm.Enabled = false;
                this.txt_initDataTrasm.Enabled = false;
                this.txt_fineDataTrasm.Visible = true;
                this.txt_fineDataTrasm.Text = DocumentManager.getLastDayOfWeek();
                this.txt_fineDataTrasm.Visible = true;
                this.txt_fineDataTrasm.Visible = true;
                this.txt_fineDataTrasm.Enabled = false;
                this.txt_fineDataTrasm.Enabled = false;
                this.lbl_initdataTrasm.Visible = true;
                this.lbl_finedataTrasm.Visible = true;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_MC.ToString())
            {
                this.ddl_dataTrasm.SelectedIndex = 4;
                this.txt_initDataTrasm.Text = DocumentManager.getFirstDayOfMonth();
                this.txt_initDataTrasm.Enabled = false;
                this.txt_initDataTrasm.Enabled = false;
                this.txt_fineDataTrasm.Visible = true;
                this.txt_fineDataTrasm.Text = DocumentManager.getLastDayOfMonth();
                this.txt_fineDataTrasm.Visible = true;
                this.txt_fineDataTrasm.Visible = true;
                this.txt_fineDataTrasm.Enabled = false;
                this.txt_fineDataTrasm.Enabled = false;
                this.lbl_initdataTrasm.Visible = true;
                this.lbl_finedataTrasm.Visible = true;
                retValue = true;
            }
            #region TRASMISSIONE_IERI
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_IERI.ToString() && item.valore == "1")
            {
                this.ddl_dataTrasm.SelectedIndex = 5;
                this.txt_initDataTrasm.Visible = true;
                this.txt_initDataTrasm.Text = NttDataWA.Utils.dateformat.GetYesterday();
                this.txt_initDataTrasm.Visible = true;
                this.txt_initDataTrasm.ReadOnly = true;
                this.txt_fineDataTrasm.Visible = false;
                this.txt_fineDataTrasm.Visible = false;
                this.lbl_initdataTrasm.Visible = false;
                this.lbl_finedataTrasm.Visible = false;
                retValue = true;
            }
            #endregion
            #region TRASMISSIONE_ULTIMI_SETTE_GIORNI
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_ULTIMI_SETTE_GIORNI.ToString() && item.valore == "1")
            {
                this.ddl_dataTrasm.SelectedIndex = 6;
                this.txt_initDataTrasm.Text = NttDataWA.Utils.dateformat.GetLastSevenDay();
                this.txt_initDataTrasm.ReadOnly = true;
                this.txt_fineDataTrasm.Visible = true;
                this.txt_fineDataTrasm.Text = DocumentManager.toDay();
                this.txt_fineDataTrasm.Visible = true;
                this.txt_fineDataTrasm.ReadOnly = true;
                this.lbl_initdataTrasm.Visible = true;
                this.lbl_finedataTrasm.Visible = true;
                retValue = true;
            }
            #endregion
            #region TRASMISSIONE_ULTMI_TRENTUNO_GIORNI
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_ULTMI_TRENTUNO_GIORNI.ToString() && item.valore == "1")
            {
                this.ddl_dataTrasm.SelectedIndex = 7;
                this.txt_initDataTrasm.Text = NttDataWA.Utils.dateformat.GetLastThirtyOneDay();
                this.txt_initDataTrasm.ReadOnly = true;
                this.txt_fineDataTrasm.Visible = true;
                this.txt_fineDataTrasm.Text = DocumentManager.toDay();
                this.txt_fineDataTrasm.Visible = true;
                this.txt_fineDataTrasm.ReadOnly = true;
                this.lbl_initdataTrasm.Visible = true;
                this.lbl_finedataTrasm.Visible = true;
                retValue = true;
            }
            #endregion

            this.UpPnlDateTransmission.Update();
            return retValue;
        }

        /// <summary>
        /// Azione selezione tipo filtro data trasmissione
        /// </summary>
        private void PerformActionSelectDataTrasmissione()
        {
            switch (this.ddl_dataTrasm.SelectedIndex)
            {
                case 0:
                    this.txt_initDataTrasm.Visible = true;
                    this.txt_initDataTrasm.Visible = true;
                    this.txt_initDataTrasm.Enabled = true;
                    this.txt_initDataTrasm.Visible = true;
                    this.txt_initDataTrasm.Enabled = true;
                    this.txt_fineDataTrasm.Visible = false;
                    this.txt_fineDataTrasm.Visible = false;
                    this.txt_fineDataTrasm.Visible = false;
                    this.lbl_finedataTrasm.Visible = false;
                    this.lbl_initdataTrasm.Visible = false;
                    break;

                case 1:
                    this.txt_initDataTrasm.Visible = true;
                    this.txt_initDataTrasm.Visible = true;
                    this.txt_initDataTrasm.Enabled = true;
                    this.txt_initDataTrasm.Visible = true;
                    this.txt_initDataTrasm.Enabled = true;
                    this.txt_fineDataTrasm.Visible = true;
                    this.txt_fineDataTrasm.Visible = true;
                    this.txt_fineDataTrasm.Enabled = true;
                    this.txt_fineDataTrasm.Visible = true;
                    this.txt_fineDataTrasm.Enabled = true;
                    this.lbl_finedataTrasm.Visible = true;
                    this.lbl_initdataTrasm.Visible = true;
                    break;

                case 2:
                    this.txt_initDataTrasm.Visible = true;
                    this.txt_initDataTrasm.Visible = true;
                    this.txt_initDataTrasm.Enabled = false;
                    this.txt_initDataTrasm.Visible = true;
                    this.txt_initDataTrasm.Text = DocumentManager.toDay();
                    this.txt_initDataTrasm.Enabled = false;
                    this.txt_fineDataTrasm.Visible = false;
                    this.txt_fineDataTrasm.Visible = false;
                    this.txt_fineDataTrasm.Visible = false;
                    this.lbl_finedataTrasm.Visible = false;
                    this.lbl_initdataTrasm.Visible = false;
                    break;

                case 3:
                    this.txt_initDataTrasm.Visible = true;
                    this.txt_initDataTrasm.Enabled = false;
                    this.txt_initDataTrasm.Text = DocumentManager.getFirstDayOfWeek();
                    this.txt_initDataTrasm.Enabled = false;
                    this.txt_fineDataTrasm.Visible = true;
                    this.txt_fineDataTrasm.Visible = true;
                    this.txt_fineDataTrasm.Enabled = false;
                    this.txt_fineDataTrasm.Text = DocumentManager.getLastDayOfWeek();
                    this.txt_fineDataTrasm.Visible = true;
                    this.txt_fineDataTrasm.Enabled = false;
                    this.lbl_finedataTrasm.Visible = true;
                    this.lbl_initdataTrasm.Visible = true;
                    break;

                case 4:
                    this.txt_initDataTrasm.Visible = true;
                    this.txt_initDataTrasm.Enabled = false;
                    this.txt_initDataTrasm.Text = DocumentManager.getFirstDayOfMonth();
                    this.txt_initDataTrasm.Enabled = false;
                    this.txt_fineDataTrasm.Visible = true;
                    this.txt_fineDataTrasm.Visible = true;
                    this.txt_fineDataTrasm.Enabled = false;
                    this.txt_fineDataTrasm.Text = DocumentManager.getLastDayOfMonth();
                    this.txt_fineDataTrasm.Visible = true;
                    this.txt_fineDataTrasm.Enabled = false;
                    this.lbl_finedataTrasm.Visible = true;
                    this.lbl_initdataTrasm.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// Ripristino filtro mittente destinatario sottoposto
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersMittenteDestinatarioSottoposto(DocsPaWR.FiltroRicerca item)
        {

            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.RUOLO_SOTTOPOSTO.ToString())
            {
                DocsPaWR.Corrispondente corrSott = null;
                if (!item.valore.Equals(""))
                {
                    corrSott = AddressBookManager.getCorrispondenteBySystemIDDisabled(item.valore);
                    string codiceRub = corrSott.codiceRubrica;
                    this.Ds = UIManager.RoleManager.getUtentiInRuoloSottoposto(corrSott.systemId);
                    this.TxtDescriptionRole.Text = AddressBookManager.getDecrizioneCorrispondenteSemplice(corrSott);
                    this.TxtCodeRole.Text = codiceRub;
                    this.IdRole.Value = corrSott.systemId;

                    this.select_sottoposto.Items.Clear();
                    this.select_sottoposto.SelectedValue = "tutti";

                    if (this.Ds == null || this.Ds.Tables.Count == 0)
                    {
                        this.TxtCodeRole.Text = string.Empty;
                        this.TxtDescriptionRole.Text = string.Empty;
                        this.IdRole.Value = string.Empty;
                        this.select_sottoposto.Items.Clear();
                        this.select_sottoposto.Enabled = false;
                        this.UpPnlType.Update();
                        string msg = "ErrorTransmissionCorrespondentNotFound";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                    else
                    {
                        if (this.Ds.Tables[0].Rows.Count == 0)
                        {
                            this.select_sottoposto.Enabled = false;

                        }
                        else
                        {
                            //POPOLA SELECT UTENTI
                            this.select_sottoposto.Enabled = true;

                            this.ResetSelectSottoposti();


                        }
                    }
                }

            }

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.PERSONA_SOTTOPOSTA.ToString())
            {
                if (!item.valore.Equals(""))
                {
                    this.select_sottoposto.SelectedValue = item.valore;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Ripristino filtro mittente destinatario
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersMittenteDestinatario(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            string id = string.Empty;
            string codice = string.Empty;
            string descrizione = string.Empty;

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.ID_UO_MITT.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.ID_UO_DEST.ToString())
            {
                DocsPaWR.Corrispondente corr = AddressBookManager.getCorrispondenteBySystemIDDisabled(item.valore);

                if (corr != null)
                {
                    id = corr.systemId;
                    codice = corr.codiceRubrica;
                    descrizione = corr.descrizione;

                    retValue = true;
                }
            }

            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_MITT_RUOLO.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_DEST_RUOLO.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_MITT_UTENTE.ToString() ||
                    item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_DEST_UTENTE.ToString())
            {
                codice = item.valore;

                retValue = true;
            }

            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_RUOLO.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DESTINATARIO_RUOLO.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_UTENTE.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DESTINATARIO_UTENTE.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_UO.ToString() ||
                item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DESTINATARIO_UO.ToString())
            {
                descrizione = item.valore;

                retValue = true;
            }

            if (retValue)
            {
                if (descrizione == string.Empty)
                {
                    // Reperimento del valore della descrizione, se non memorizzato
                    DocsPaWR.Corrispondente corr = AddressBookManager.getCorrispondenteBySystemIDDisabled(codice);

                    if (corr != null)
                    {
                        descrizione = corr.descrizione;
                        id = corr.systemId;
                    }
                }

                this.txtDescrizioneCreatore.Text = descrizione;
                this.idCreatore.Value = id;
                this.txtCodiceCreatore.Text = codice;

                // Determinazione del tipo corrispondente
                string tipoCorrispondente = string.Empty;

                if (item.argomento.IndexOf("UTENTE") > -1)
                    tipoCorrispondente = "P";
                else if (item.argomento.IndexOf("RUOLO") > -1)
                    tipoCorrispondente = "R";
                else if (item.argomento.IndexOf("UO") > -1)
                    tipoCorrispondente = "U";

                if (tipoCorrispondente != string.Empty)
                {
                    this.rblOwnerType.SelectedValue = tipoCorrispondente;
                }
            }

            this.upPnlCreatore.Update();

            return retValue;
        }

        /// <summary>
        /// Ripristino filtro sul tipo di oggetto trasmesso
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private void RestoreFiltersTipoOggettoTrasmesso(DocsPaWR.FiltroRicerca item)
        {
            string language = UIManager.UserManager.GetUserLanguage();

            if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString() && item.valore == "F")
            {
                this.ddl_oggetto.SelectedValue = item.valore;
                this.ddl_tipo_doc.Visible = false;
                this.PnlTypeDocument.Controls.Clear();
                this.DocumentDdlTypeDocument.Items.Clear();
                this.LoadTypeProjects();
                this.ddl_tipo_doc.Visible = false;
                this.PlcDocInWorking.Visible = false;
                this.LoadTypeProjects();
                this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("LitSearchTransTypePrj", language);
                this.UpPnlObject.Update();
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROTOCOLLATI.ToString())
            {
                this.ddl_tipo_doc.SelectedValue = "P";
                this.UpPnlObject.Update();
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROT_ARRIVO.ToString())
            {
                this.ddl_tipo_doc.SelectedValue = "PA";
                this.UpPnlObject.Update();
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROT_PARTENZA.ToString())
            {
                this.ddl_tipo_doc.SelectedValue = "PP";
                this.UpPnlObject.Update();
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_NON_PROTOCOLLATI.ToString())
            {
                this.ddl_tipo_doc.SelectedValue = "NP";
                this.UpPnlObject.Update();
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_TUTTI.ToString())
            {
                this.ddl_tipo_doc.SelectedValue = "Tutti";
                this.UpPnlObject.Update();
            }
            else if (item.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROT_INTERNO.ToString())
            {
                this.ddl_tipo_doc.SelectedValue = "PI";
                this.UpPnlObject.Update();
            }
        }



        protected void SearchTransmissionsViewDetails_Click(object sender, ImageClickEventArgs e)
        {
            CustomImageButton btnIm = (CustomImageButton)sender;
            GridViewRow row = (GridViewRow)btnIm.Parent.Parent;
            int rowIndex = row.RowIndex / 2;
            TrasmManager.setSelectedTransmission(this.Result[rowIndex]);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ViewDetailTransmission", "ajaxModalPopupViewDetailTransmission();", true);
        }


        protected void ViewDocument_Click(object sender, ImageClickEventArgs e)
        {
            CustomImageButton btnIm = (CustomImageButton)sender;
            GridViewRow row = (GridViewRow)btnIm.Parent.Parent;
            int rowIndex = row.RowIndex;
            if ((Label)row.FindControl("TYPE_OBJECT") != null)
            {
                if (((Label)row.FindControl("TYPE_OBJECT")).Text.Equals("D"))
                {
                    if ((Label)row.FindControl("ID_OBJECT") != null)
                    {
                        string idProfile = ((Label)row.FindControl("ID_OBJECT")).Text;
                        SchedaDocumento schedaDocumento = UIManager.DocumentManager.getDocumentDetails(this, idProfile, idProfile);

                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                        actualPage.IdObject = schedaDocumento.systemId;
                        actualPage.OriginalObjectId = schedaDocumento.systemId;
                        actualPage.NumPage = this.SelectedPage.ToString();
                        actualPage.SearchFilters = this.SearchFilters;
                        actualPage.PageSize = this.PageSize.ToString();

                        actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_TRANSMISSIONS.ToString(), string.Empty);
                        actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_TRANSMISSIONS.ToString(), true, this.Page);
                        actualPage.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_TRANSMISSIONS.ToString();
                        actualPage.SearchTransmission = true;
                        actualPage.Type = "D";
                        actualPage.TypeTransmissionSearch = this.TypeSearch;

                        actualPage.Page = "SEARCHTRANSMISSION.ASPX";
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
                        this.ListTransmissionNavigation = this.Result;
                        Response.Redirect("~/Document/Document.aspx");
                    }

                }
                else
                {
                    if ((Label)row.FindControl("ID_OBJECT") != null)
                    {
                        string idProject = ((Label)row.FindControl("ID_OBJECT")).Text;
                        Fascicolo fascicolo = UIManager.ProjectManager.getFascicoloById(idProject);
                        fascicolo.template = ProfilerProjectManager.getTemplateFascDettagli(fascicolo.systemID);

                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                        actualPage.IdObject = fascicolo.systemID;
                        actualPage.OriginalObjectId = fascicolo.systemID;
                        actualPage.NumPage = this.SelectedPage.ToString();
                        actualPage.SearchFilters = this.SearchFilters;
                        actualPage.PageSize = this.PageSize.ToString();

                        actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_TRANSMISSIONS.ToString(), string.Empty);
                        actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_TRANSMISSIONS.ToString(), true, this.Page);
                        actualPage.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_TRANSMISSIONS.ToString();
                        actualPage.SearchTransmission = true;
                        actualPage.Type = "F";
                        actualPage.TypeTransmissionSearch = this.TypeSearch;

                        actualPage.Page = "SEARCHTRANSMISSION.ASPX";
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


                        UIManager.ProjectManager.setProjectInSession(fascicolo);
                        this.ListTransmissionNavigation = this.Result;
                        Response.Redirect("~/Project/project.aspx");
                    }
                }
            }
        }


        protected void gridViewResult_PreRender(object sender, EventArgs e)
        {
            try
            {
                // GridManager.SelectedGrid
                if (gridViewResult != null && gridViewResult.Rows != null && gridViewResult.Rows.Count > 0)
                {
                    int cellsCount = 0;
                    if (gridViewResult.Columns.Count > 0)
                        foreach (DataControlField td in gridViewResult.Columns)
                        {
                            if (td.Visible) cellsCount++;
                            //se la segnatura di repertorio non è abilitata nascondo il campo
                            if (td.HeaderText.Equals(Utils.Languages.GetLabelFromCode("SearchTrasmLblRep", UIManager.UserManager.GetUserLanguage())) &&
                                !NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, "VIS_SEGNATURA_REPERTORI").Equals("1"))
                            {
                                td.Visible = false;
                            }
                        }

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
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.HiddenRemoveUsedSearch.Value))
            {
                try
                {
                    schedaRicerca.Cancella(Int32.Parse(this.DdlRapidSearch.SelectedValue));
                    Session.Remove("itemUsedSearch");
                    this.DdlRapidSearch.SelectedIndex = 0;
                    this.SearchDocumentAdvancedRemove.Enabled = false;
                    this.PopulateDDLSavedSearches();
                    this.UpPnlRapidSearch.Update();

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('InfoSearchProjectRemoveSearch', 'info', '');", true);
                }
                catch (Exception ex)
                {
                    string msg = utils.FormatJs("Impossibile rimuovere i criteri di ricerca. Errore: " + ex.Message);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCustom', 'error', '', '" + msg + "');", true);
                }

                this.HiddenRemoveUsedSearch.Value = string.Empty;
                this.upPnlButtons.Update();
            }

            if (!string.IsNullOrEmpty(this.SaveSearch.ReturnValue))
            {
                this.PopulateDDLSavedSearches();
                this.SearchDocumentAdvancedRemove.Enabled = true;
                this.upPnlButtons.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SaveSearch','');", true);
            }

            if (!string.IsNullOrEmpty(this.ModifySearch.ReturnValue))
            {
                this.PopulateDDLSavedSearches();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ModifySearch','');", true);
            }
            if (!string.IsNullOrEmpty(this.ViewDetailTransmission.ReturnValue) && this.ViewDetailTransmission.ReturnValue.Equals("OPEN_PROJECT"))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ViewDetailTransmission','');", true);
                Response.Redirect(this.ResolveUrl("~/Project/Project.aspx?t=c"), false);
                return;
            }

            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_PANEL_BUTTONS))
            {
                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_SEARCH_PROJECT)))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_ViewDetailTransmission').contentWindow.closeSearchProject();", true);
                }

                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_OPEN_TITOLARIO)))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_ViewDetailTransmission').contentWindow.closeOpenTitolario();", true);
                }
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




        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

                switch (addressBookCallFrom)
                {
                    case "V_R_R_S":
                        if (atList != null && atList.Count > 0)
                        {
                            NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                            Corrispondente tempCorrSingle;
                            if (!corrInSess.isRubricaComune)
                                tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                            else
                                tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                            this.TxtCodeRole.Text = tempCorrSingle.codiceRubrica;
                            this.TxtDescriptionRole.Text = tempCorrSingle.descrizione;
                            this.IdRole.Value = tempCorrSingle.systemId;
                            this.select_sottoposto.Enabled = true;
                            this.Ds = RoleManager.getUtentiInRuoloSottoposto(tempCorrSingle.systemId);
                            this.ResetSelectSottoposti();
                            this.UpPnlType.Update();
                        }
                        break;
                    case "F_X_X_S":
                        if (atList != null && atList.Count > 0)
                        {
                            NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                            Corrispondente tempCorrSingle;
                            if (!corrInSess.isRubricaComune)
                                tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                            else
                                tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                            this.txtCodiceCreatore.Text = tempCorrSingle.codiceRubrica;
                            this.txtDescrizioneCreatore.Text = tempCorrSingle.descrizione;
                            this.idCreatore.Value = tempCorrSingle.systemId;
                            this.upPnlCreatore.Update();
                        }
                        break;
                }

                HttpContext.Current.Session["AddressBook.At"] = null;
                HttpContext.Current.Session["AddressBook.Cc"] = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchTransmissionSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.SearchTransmissionFilters())
                {
                    // Impostazione del filtro utilizzato
                    schedaRicerca.FiltriRicerca = this.SearchFilters;
                    schedaRicerca.ProprietaNuovaRicerca = new SearchManager.NuovaRicerca();
                    Session["idRicercaSalvata"] = null;
                    Session["tipoRicercaSalvata"] = "T";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupSaveSearch();", "ajaxModalPopupSaveSearch();", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void SearchTransmissionSearch_Click(object o, EventArgs e)
        {
            try
            {
                if ((this.TypeSearch.Equals("R") && this.chk_me_stesso.Checked == false && this.chk_mio_ruolo.Checked == false && this.chk_visSott.Checked == false))
                {
                    string msg = "WarningSelectRecipitnTransmission";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
                else
                {
                    if (this.chk_mio_ruolo.Checked == true && (string.IsNullOrEmpty(this.TxtCodeRole.Text) || string.IsNullOrEmpty(this.TxtDescriptionRole.Text)))
                    {
                        string msg = "WarningSelectOnlyRoleSearchTransmission";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                    else
                    {
                        if (this.chk_mio_ruolo.Checked == true && string.IsNullOrEmpty(select_sottoposto.SelectedValue))
                        {
                            string msg = "WarningSelectRoleNoUsersSearchTransmission";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            if (this.TypeSearch.Equals("E") && this.chk_mio_ruolo.Checked == false && this.chk_visSott.Checked == false)
                            {

                                string msg = "WarningSelectSenderSearchTransmission";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            else
                            {
                                bool result = this.SearchTransmissionFilters();
                                if (result)
                                {
                                    this.SelectedRow = string.Empty;
                                    this.SelectedPage = 1;
                                    this.SearchTransmissionsAndDisplayResult(this.SelectedPage);
                                }

                                this.projectLblNumeroDocumenti.Text = this.GetLabel("projectLblNumeroTransm") + " (" + this.RecordCount.ToString() + ") " + this.GetLabel("projectLblNumeroTransmFound");
                                this.UpnlAzioniMassive.Update();
                                this.UpnlNumerodocumenti.Update();
                                this.UpnlGrid.Update();
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

        /// <summary>
        /// Questa funzione si occupa di ricercare i documenti e di visualizzare 
        /// i dati
        /// </summary>
        private void SearchTransmissionsAndDisplayResult(int selectedPage)
        {
            // Numero di record restituiti dalla pagina
            int recordNumber = 0;
            this.Color = new List<int>();
            /* ABBATANGELI GIANLUIGI
             * il nuovo parametro outOfMaxRowSearchable è true se raggiunto il numero 
             * massimo di riche accettate in risposta ad una ricerca */
            bool outOfMaxRowSearchable;
            // Ricerca dei documenti
            this.LoadTrasmissioni(out recordNumber, out outOfMaxRowSearchable);


            if (outOfMaxRowSearchable)
            {
                string valoreChiaveDB = Utils.InitConfigurationKeys.GetValue("0", DBKeys.MAX_ROW_SEARCHABLE.ToString());
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
            this.ShowResult(this.Result);
        }

        private void ShowResult(Trasmissione[] result)
        {
            Trasmissione[] res = result;

            if (result != null && result.Length > 0)
            {
                List<Trasmissione> dt = new List<Trasmissione>();
                dt = result.ToList<Trasmissione>();

                List<Trasmissione> dt2 = new List<Trasmissione>();

                dt2 = dt;

                int dtRowsCount = dt.Count;
                int index = 1;
                if (dtRowsCount > 0)
                {
                    for (int i = 0; i < dtRowsCount; i++)
                    {
                        Trasmissione dr = new Trasmissione();
                        dr = dt2[index - 1];
                        dt.Insert(index, dr);
                        index += 2;
                    }
                }
                res = dt.ToArray<Trasmissione>();
            }

            this.gridViewResult.DataSource = res;
            this.gridViewResult.DataBind();
            this.grid_pageindex.Value = (this.PageCount - 1).ToString();
            this.grid_pageindex.Value = this.SelectedPage.ToString();
            this.gridViewResult.PageIndex = this.PageCount;
            this.gridViewResult.SelectedIndex = this.SelectedPage;
            this.BuildGridNavigator();


            this.projectLblNumeroDocumenti.Text = this.GetLabel("projectLblNumeroTransm") + " (" + this.RecordCount.ToString() + ") " + this.GetLabel("projectLblNumeroTransmFound");
            this.UpnlNumerodocumenti.Update();
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        private void LoadTrasmissioni(out int totalRecordCount, out bool outOfMaxRowSearchable)
        {
            int totalPageCount;
            totalRecordCount = 0;
            outOfMaxRowSearchable = false;

            try
            {
                DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPaWR.TrasmissioneOggettoTrasm();

                DocsPaWR.Trasmissione[] listaTrasmissioni = null;

                bool trasmEffettuate = (this.TypeSearch == "E");

                if (trasmEffettuate)
                {
                    listaTrasmissioni = TrasmManager.getQueryEffettuatePagingLite(oggettoTrasm, this.SearchFilters[0], this.SelectedPage, false, this.PageSize, out totalPageCount, out totalRecordCount);
                }
                else
                {
                    listaTrasmissioni = TrasmManager.getQueryRicevuteLite(oggettoTrasm, this.SearchFilters[0], this.SelectedPage, false, this.PageSize, out totalPageCount, out totalRecordCount);
                }

                this.Result = listaTrasmissioni;
                this.RecordCount = totalRecordCount;
                //this.PageCount = totalPageCount;
                this.PageCount = (int)Math.Round(((double)this.RecordCount / (double)this.PageSize) + 0.49);

                /* ABBATANGELI GIANLUIGI
                 * se viene restituito totalPageCount = -2 vuol dire che ho raggiunto 
                 * il numero massimo di riche accettate in risposta ad una ricerca */
                outOfMaxRowSearchable = (totalPageCount == -2);
                if (!outOfMaxRowSearchable)
                {
                    if (listaTrasmissioni != null && listaTrasmissioni.Length > 0)
                    {
                        this.SearchTransmissionExportExcel.Enabled = true;
                        /*
                        for (int i = 0; i < listaTrasmissioni.Length; i++)
                        {
                            DocsPaWR.Trasmissione trasm = listaTrasmissioni[i];

                            if (!trasmEffettuate && (trasm.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO))
                            {
                                //this.dt_Ric.Columns[8].Visible = false;
                                //this.dt_Ric.Columns[9].Visible = true;

                            }
                            if (trasmEffettuate && (trasm.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO))
                            {
                                //this.dt_Eff.Columns[6].Visible = false;
                                //this.dt_Eff.Columns[7].Visible = true;

                            }
                            trasm = null;
                        }

                        if (trasmEffettuate)
                        {

                            if (((DocsPaWR.Trasmissione)listaTrasmissioni[0]).tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO)
                            {
                                //this.dt_Eff.Columns[6].HeaderText = "Cod.";
                                //this.dt_Eff.Columns[7].HeaderText = "Descrizione";
                            }
                            else
                            {
                                //this.dt_Eff.Columns[5].HeaderText = "Doc.";
                                //this.dt_Eff.Columns[6].HeaderText = "Oggetto<br>&#160-------<br>&#160;Mittente";
                            }
                        }
                        else
                        {
                            if (((DocsPaWR.Trasmissione)listaTrasmissioni[0]).tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO)
                            {
                                //this.dt_Ric.Columns[8].HeaderText = "Cod.";
                                //this.dt_Ric.Columns[9].HeaderText = "Descrizione";
                            }
                            else
                            {
                                //this.dt_Ric.Columns[7].HeaderText = "Doc.";
                                //this.dt_Ric.Columns[8].HeaderText = "Oggetto<br>&#160-------<br>&#160;Mittente";
                            }
                        }
                         * */
                    }
                    else
                    {
                        //if (trasmEffettuate)
                        //    this.pnl_sollecito.Visible = false;
                        this.SearchTransmissionExportExcel.Enabled = false;
                    }
                }
                else
                {
                    this.SearchTransmissionExportExcel.Enabled = false;

                    //if (trasmEffettuate)
                    //this.pnl_sollecito.Visible = false;
                }
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        protected string GetSystemID(DocsPaWR.Trasmissione item)
        {
            return item.systemId;
        }

        protected string GetimgObject(DocsPaWR.Trasmissione item)
        {
            string result = string.Empty;
            if (item.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
            {
                result = ResolveUrl("~/Images/Icons/view_doc_grid.png");
            }
            else
            {
                result = ResolveUrl("~/Images/Icons/ricerca-fasc-1.png");
            }
            return result;
        }

        protected string GetIdObject(DocsPaWR.Trasmissione item)
        {
            string result = string.Empty;
            if (item.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
            {
                if (item.infoDocumento != null)
                {
                    result = item.infoDocumento.idProfile;
                }
            }
            else
            {
                if (item.infoFascicolo != null)
                {
                    result = item.infoFascicolo.idFascicolo;
                }
            }

            return result;
        }

        protected string GetRepertorio(DocsPaWR.Trasmissione item)
        {
            try
            {
                if (this.ddl_oggetto.SelectedValue.Equals("D"))
                {
                    return "<p style=\"color:red;\">" + item.infoDocumento.contatore + "</p>";
                }
                else return string.Empty;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        protected string GetTypeObject(DocsPaWR.Trasmissione item)
        {
            string result = string.Empty;
            if (item.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
            {
                result = "D";
            }
            else
            {

                result = "F";

            }

            return result;
        }

        protected string GetimgObjectHover(DocsPaWR.Trasmissione item)
        {
            string result = string.Empty;
            if (item.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
            {
                result = ResolveUrl("~/Images/Icons/view_doc_grid_hover.png");
            }
            else
            {
                result = ResolveUrl("~/Images/Icons/ricerca-fasc-1_hover.png");
            }
            return result;
        }

        protected string GetimgObjectDisabled(DocsPaWR.Trasmissione item)
        {
            string result = string.Empty;
            if (item.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
            {
                result = ResolveUrl("~/Images/Icons/view_doc_grid_disabled.png");
            }
            else
            {
                result = ResolveUrl("~/Images/Icons/ricerca-fasc-1_disabled.png");
            }
            return result;
        }

        protected string GetDataTrasm(DocsPaWR.Trasmissione item)
        {
            return item.dataInvio;
        }

        protected string GetSenderUser(DocsPaWR.Trasmissione trasm)
        {
            string descrUtente = string.Empty;
            string language = UIManager.UserManager.GetUserLanguage();
            string del = Utils.Languages.GetLabelFromCode("TransmissionDelegatedBy", language);

            if (string.IsNullOrEmpty(trasm.delegato))
                descrUtente = trasm.utente.descrizione;
            else
                descrUtente = trasm.delegato + "<br>" + del + " " + trasm.utente.descrizione;
            return descrUtente;
        }

        protected string GetRoleUser(DocsPaWR.Trasmissione item)
        {
            return item.ruolo.descrizione;
        }

        protected string GetDescriptionImg(DocsPaWR.Trasmissione item)
        {
            string language = UIManager.UserManager.GetUserLanguage();

            if (item.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
            {
                return Utils.Languages.GetLabelFromCode("ProjectIconTemplateVisualizzaDocumento", language);
            }
            else
            {
                return Utils.Languages.GetLabelFromCode("ProjectIconTemplateVisualizzaFascicolo", language);
            }

        }

        protected string GetDescriptionImgTrans()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode("SearchTransmissionViewDetails", language);
        }

        protected string GetReasonTrans(DocsPaWR.Trasmissione item)
        {
            string result = string.Empty;
            if (item.trasmissioniSingole != null && item.trasmissioniSingole.Length > 0)
            {
                result = item.trasmissioniSingole[0].ragione.descrizione;
            }
            return result;
        }

        protected string GetRecipientsTrans(DocsPaWR.Trasmissione trasm)
        {
            string destinatariTrasm = string.Empty;
            int i = 0;
            foreach (DocsPaWR.TrasmissioneSingola trasmS in trasm.trasmissioniSingole)
            {

                destinatariTrasm += trasmS.corrispondenteInterno.descrizione;
                if (i < trasm.trasmissioniSingole.Length - 1)
                {
                    destinatariTrasm += " - ";
                }
                i++;
            }
            return destinatariTrasm;
        }

        protected string GetDataScadenzaTrans(DocsPaWR.Trasmissione item)
        {
            string result = string.Empty;
            if (item.trasmissioniSingole != null && item.trasmissioniSingole.Length > 0)
            {
                result = item.trasmissioniSingole[0].dataScadenza;
            }
            return result;
        }

        protected string GetIdDocument(DocsPaWR.Trasmissione item)
        {
            string id = string.Empty;
            if (item.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
            {
                if (item.infoDocumento != null)
                {
                    //è un documento
                    if (!string.IsNullOrEmpty(item.infoDocumento.numProt))
                    {
                        //è un protocollo
                        id = "<span class=\"redWeight\">" + item.infoDocumento.numProt + "<br/>" + item.infoDocumento.dataApertura + "</span>";
                    }
                    else
                    {
                        //è un documento grigio
                        id = item.infoDocumento.docNumber + "<br/>" + item.infoDocumento.dataApertura;
                    }
                }
            }

            if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(item.IdSegnaturaOCodFasc))
                id = item.IdSegnaturaOCodFasc + "<br/>" + item.DataDocFasc;

            return id;
        }

        protected string GetInfoTrasm(DocsPaWR.Trasmissione trasm)
        {
            string InfoOggTrasm = null;

            if (trasm.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO) //è UN DOCUMENTO
            {
                if (trasm.infoDocumento != null)
                {
                    if (trasm.infoDocumento.docNumber != null)
                    {
                        string msg;
                        int diritti = 0;
                        if (!ListSecurityObject.ContainsKey(trasm.infoDocumento.docNumber))
                        {
                            diritti = DocumentManager.verifyDocumentACL("D", trasm.infoDocumento.idProfile, UserManager.GetInfoUser(), out msg);
                            ListSecurityObject.Add(trasm.infoDocumento.docNumber, diritti);
                        }
                        else
                        {
                            diritti = ListSecurityObject[trasm.infoDocumento.docNumber];
                        }

                        if (diritti == 2)
                        {
                            InfoOggTrasm = trasm.infoDocumento.oggetto;
                        }

                        if (diritti == 0)
                        {
                            InfoOggTrasm = "<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul " + trasm.tipoOggetto.ToString().ToLower() + "</b>";
                        }
                    }
                }
            }

            if (trasm.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO)  //è UN FASCICOLO
                if (trasm.infoFascicolo != null)
                {
                    // Verifica diritti dell'utente sul fascicolo
                    string msg;
                    int diritti = DocumentManager.verifyDocumentACL("F", trasm.infoFascicolo.idFascicolo, UserManager.GetInfoUser(), out msg);

                    if (diritti == 2)
                    {
                        InfoOggTrasm = trasm.infoFascicolo.descrizione;
                    }

                    if (diritti == 0)
                    {
                        InfoOggTrasm = "<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul " + trasm.tipoOggetto.ToString().ToLower() + "</b>";
                    }
                }

            return InfoOggTrasm;
        }

        protected bool GetEnableImgViewDocument(DocsPaWR.Trasmissione trasm)
        {
            bool result = true;

            if (trasm.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO) //è UN DOCUMENTO
            {
                if (trasm.infoDocumento != null)
                {
                    if (trasm.infoDocumento.docNumber != null)
                    {
                        string msg;
                        int diritti = 0;
                        if (!ListSecurityObject.ContainsKey(trasm.infoDocumento.docNumber))
                        {
                            diritti = DocumentManager.verifyDocumentACL("D", trasm.infoDocumento.idProfile, UserManager.GetInfoUser(), out msg);
                            ListSecurityObject.Add(trasm.infoDocumento.docNumber, diritti);
                        }
                        else
                        {
                            diritti = ListSecurityObject[trasm.infoDocumento.docNumber];
                        }

                        if (diritti == 2)
                        {
                            result = true;
                        }

                        if (diritti == 0)
                        {
                            result = false;
                        }
                    }
                }
            }

            if (trasm.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO)  //è UN FASCICOLO
                if (trasm.infoFascicolo != null)
                {
                    // Verifica diritti dell'utente sul fascicolo
                    string msg;

                    int diritti = 0;
                    if (!ListSecurityObject.ContainsKey(trasm.infoFascicolo.idFascicolo))
                    {
                        diritti = diritti = DocumentManager.verifyDocumentACL("F", trasm.infoFascicolo.idFascicolo, UserManager.GetInfoUser(), out msg);
                        ListSecurityObject.Add(trasm.infoFascicolo.idFascicolo, diritti);
                    }
                    else
                    {
                        diritti = ListSecurityObject[trasm.infoFascicolo.idFascicolo];
                    }

                    if (diritti == 2)
                    {
                        result = true;
                    }

                    if (diritti == 0)
                    {
                        result = false;
                    }
                }

            return result;
        }

        /// <summary>
        /// Fornisce il separatore per il campo oggetto/mittente nel caso
        /// delle tramissioni ricevute (documenti)
        /// </summary>
        /// <param name="oggetto"></param>
        /// <param name="mittente"></param>
        /// <returns></returns>
        protected string ShowSeparator(DocsPaWR.Trasmissione trasm)
        {
            string mittDoc = String.Empty;
            string InfoOggTrasm = string.Empty;
            if (trasm.infoDocumento != null && trasm.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
            {
                if (trasm.infoDocumento.docNumber != null)
                {
                    string msg;
                    int diritti = 0;
                    if (!ListSecurityObject.ContainsKey(trasm.infoDocumento.docNumber))
                    {
                        diritti = DocumentManager.verifyDocumentACL("D", trasm.infoDocumento.idProfile, UserManager.GetInfoUser(), out msg);
                        ListSecurityObject.Add(trasm.infoDocumento.docNumber, diritti);
                    }
                    else
                    {
                        diritti = ListSecurityObject[trasm.infoDocumento.docNumber];
                    }

                    if (diritti == 2)
                    {
                        InfoOggTrasm = trasm.infoDocumento.oggetto;
                        mittDoc = trasm.infoDocumento.mittDoc;
                    }

                    if (diritti == 0)
                    {
                        InfoOggTrasm = "<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul " + trasm.tipoOggetto.ToString().ToLower() + "</b>";
                        mittDoc = string.Empty;
                    }
                }
            }

            if (!String.IsNullOrEmpty(mittDoc))
                return "<br/>-------<br/>";
            else
                return String.Empty;
        }


        /// <summary>
        /// Fornisce il separatore per il campo oggetto/mittente nel caso
        /// delle tramissioni ricevute (documenti)
        /// </summary>
        /// <param name="oggetto"></param>
        /// <param name="mittente"></param>
        /// <returns></returns>
        protected string GetSenderDoc(DocsPaWR.Trasmissione trasm)
        {
            string mittDoc = String.Empty;
            string InfoOggTrasm = string.Empty;
            if (trasm.infoDocumento != null && trasm.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
            {
                if (trasm.infoDocumento.docNumber != null)
                {
                    string msg;
                    int diritti = 0;
                    if (!ListSecurityObject.ContainsKey(trasm.infoDocumento.docNumber))
                    {
                        diritti = DocumentManager.verifyDocumentACL("D", trasm.infoDocumento.idProfile, UserManager.GetInfoUser(), out msg);
                        ListSecurityObject.Add(trasm.infoDocumento.docNumber, diritti);
                    }
                    else
                    {
                        diritti = ListSecurityObject[trasm.infoDocumento.docNumber];
                    }

                    if (diritti == 2)
                    {
                        InfoOggTrasm = trasm.infoDocumento.oggetto;
                        mittDoc = trasm.infoDocumento.mittDoc;
                    }

                    if (diritti == 0)
                    {
                        InfoOggTrasm = "<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul " + trasm.tipoOggetto.ToString().ToLower() + "</b>";
                        mittDoc = string.Empty;
                    }
                }
            }

            return mittDoc;
        }

        //protected string GetInfoObjTransmDoc(DocsPaWR.Trasmissione trasm)
        //{
        //    string InfoOggTrasm = null;

        //    if (trasm.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO) //è UN DOCUMENTO
        //        if (trasm.infoDocumento != null)
        //            if (trasm.infoDocumento.docNumber != null)
        //            {
        //                string msg;
        //                int diritti = 0;
        //                if (!ListSecurityObject.ContainsKey(trasm.infoDocumento.docNumber))
        //                {
        //                    diritti = DocumentManager.verifyDocumentACL("D", trasm.infoDocumento.idProfile, UserManager.GetInfoUser(), out msg);
        //                    ListSecurityObject.Add(trasm.infoDocumento.docNumber, diritti);
        //                }
        //                else
        //                {
        //                    diritti = ListSecurityObject[trasm.infoDocumento.docNumber];
        //                }

        //                if (diritti == 2)
        //                {
        //                    InfoOggTrasm = trasm.infoDocumento.oggetto;
        //                }

        //                if (diritti == 0)
        //                {
        //                    InfoOggTrasm = "<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul " + trasm.tipoOggetto.ToString().ToLower() + "</b>";
        //                }
        //            }


        //    if (trasm.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO)  //è UN FASCICOLO
        //        if (trasm.infoFascicolo != null)
        //        {
        //            // Verifica diritti dell'utente sul fascicolo
        //            string msg;
        //            int diritti = 0;
        //            if (!ListSecurityObject.ContainsKey(trasm.infoFascicolo.idFascicolo))
        //            {
        //                diritti = diritti = DocumentManager.verifyDocumentACL("F", trasm.infoFascicolo.idFascicolo, UserManager.GetInfoUser(), out msg);
        //                ListSecurityObject.Add(trasm.infoFascicolo.idFascicolo, diritti);
        //            }
        //            else
        //            {
        //                diritti = ListSecurityObject[trasm.infoFascicolo.idFascicolo];
        //            }

        //            if (diritti == 2)
        //            {
        //                InfoOggTrasm = trasm.infoFascicolo.descrizione;
        //            }

        //            if (diritti == 0)
        //            {
        //                InfoOggTrasm = "<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul " + trasm.tipoOggetto.ToString().ToLower() + "</b>";
        //            }
        //        }

        //    //  if (string.IsNullOrEmpty(InfoOggTrasm))
        //    //     InfoOggTrasm = "<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul " + trasm.tipoOggetto.ToString().ToLower() + "</b>";

        //    return InfoOggTrasm;
        //}



        private bool SearchTransmissionFilters()
        {
            bool result = true;
            if (!ricercaTrasmissioni())
            {
                string msg = "WarningAddressBookBirthdayInvalid";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                return false;
            }
            else
            {
                if (!this.txt_initDataTrasm.Text.Equals("") && !this.txt_fineDataTrasm.Text.Equals(""))
                {
                    if (utils.verificaIntervalloDate(this.txt_initDataTrasm.Text, this.txt_fineDataTrasm.Text))
                    {
                        string msg = "WarningMandateDateInterval";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return false;
                    }
                }
            }
            return result;
        }

        private bool ricercaTrasmissioni()
        {
            bool res = true;
            try
            {
                DocsPaWR.FiltroRicerca[][] qV;
                DocsPaWR.FiltroRicerca[] fVList;
                DocsPaWR.FiltroRicerca fV1;
                //array contenitore degli array filtro di ricerca
                qV = new DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPaWR.FiltroRicerca[1];
                fVList = new DocsPaWR.FiltroRicerca[0];

                #region //filtri su mancanza immagine, fascicolazione e da protocollare
                if (this.P_Prot.Checked || this.M_Fasc.Checked || this.M_si_img.Checked || this.M_Img.Checked)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    if (this.M_Img.Checked)
                    {
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.MANCANZA_IMMAGINE.ToString();
                        fV1.valore = "M_Img";
                    }
                    else
                    {
                        if (this.M_Fasc.Checked)
                        {
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.MANCANZA_FASCICOLAZIONE.ToString();
                            fV1.valore = "M_Fasc";
                        }
                        else
                        {
                            if (this.P_Prot.Checked)
                            {
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DA_PROTOCOLLARE.ToString();
                                fV1.valore = "P_Prot";
                            }
                            else
                            {
                                if (this.M_si_img.Checked)
                                {
                                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.CON_IMMAGINE.ToString();
                                    fV1.valore = "M_si_img";
                                }
                            }
                        }
                    }

                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    if (fV1.valore.Equals("M_si_img"))
                    {
                        if (this.chk_firmati.Checked)
                        {
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.FIRMATO.ToString();
                            fV1.valore = "1";
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        if (this.chk_non_firmati.Checked)
                        {
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.FIRMATO.ToString();
                            fV1.valore = "0";
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }

                        if (this.ddl_tipoFileAcquisiti.SelectedIndex > 0)
                        {
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TIPO_FILE_ACQUISITO.ToString();
                            fV1.valore = this.ddl_tipoFileAcquisiti.SelectedItem.Value;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }

                    }
                }
                #endregion

                #region  filtro sul mittente/destinatario
                if (!string.IsNullOrEmpty(this.txtCodiceCreatore.Text) || !string.IsNullOrEmpty(this.txtDescrizioneCreatore.Text))
                {
                    if (!this.txtCodiceCreatore.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = getArgomento(true, this.rblOwnerType.SelectedValue, this.TypeSearch);
                        if (!this.rblOwnerType.SelectedValue.Equals("U"))
                            fV1.valore = this.txtCodiceCreatore.Text;
                        else
                            fV1.valore = this.idCreatore.Value;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    else
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = getArgomento(false, this.rblOwnerType.SelectedValue, this.TypeSearch);
                        fV1.valore = this.txtDescrizioneCreatore.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    // Aggiunta filtro per estensione ricerca a storicizzati
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = FiltriTrasmissioneNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString();
                    fV1.valore = this.chkCreatoreExtendHistoricized.Checked.ToString();
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region  filtro sulla ragione
                if (ddl_ragioni.SelectedIndex > 0)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.RAGIONE.ToString();
                    fV1.valore = ddl_ragioni.SelectedItem.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro sulla data trasmissione
                if (this.ddl_dataTrasm.SelectedIndex == 2)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataTrasm.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_SC.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataTrasm.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_MC.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataTrasm.SelectedIndex == 5)
                {
                    // siamo nel caso di Ieri
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_IERI.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataTrasm.SelectedIndex == 6)
                {
                    // siamo nel caso di Ultimi 7 giorni
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_ULTIMI_SETTE_GIORNI.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataTrasm.SelectedIndex == 7)
                {
                    // siamo nel caso di Ultimi 31 giorni
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_ULTMI_TRENTUNO_GIORNI.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                if (this.ddl_dataTrasm.SelectedIndex == 0)
                {//valore singolo carico DATA_CREAZIONE
                    if (!string.IsNullOrEmpty(this.txt_initDataTrasm.Text))
                    {
                        if (!utils.isDate(this.txt_initDataTrasm.Text))
                        {
                            string msg = "WarningAddressBookBirthdayInvalid";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return false;
                        }
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_IL.ToString();
                        fV1.valore = this.txt_initDataTrasm.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                if (this.ddl_dataTrasm.SelectedIndex == 1)
                {//valore singolo carico DATA_CREAZIONE
                    if (!string.IsNullOrEmpty(this.txt_initDataTrasm.Text))
                    {
                        if (!utils.isDate(this.txt_initDataTrasm.Text))
                        {
                            string msg = "WarningAddressBookBirthdayInvalid";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return false;
                        }
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.txt_initDataTrasm.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!string.IsNullOrEmpty(this.txt_fineDataTrasm.Text))
                    {
                        if (!utils.isDate(this.txt_fineDataTrasm.Text))
                        {
                            string msg = "WarningAddressBookBirthdayInvalid";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return false;
                        }
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_PRECEDENTE_IL.ToString();
                        fV1.valore = this.txt_fineDataTrasm.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region FILTRO SU RICEVUTE/EFFETTUATE
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.RICEVUTE_EFFETTUATE.ToString();
                fV1.valore = this.TypeSearch;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion

                #region filtro Assegnazioni Pendenti
                //if (this.TypeSearch.Equals("E"))
                //{
                //    fV1 = new DocsPaWR.FiltroRicerca();
                //    if (this.chk_DaCompletare.Checked)
                //        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.ATTIVITA_NON_CONCLUSE.ToString();
                //    else
                //        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.ASSEGNAZIONI_PENDENTI.ToString();
                //    fV1.valore = "true";
                //    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                //}
                #endregion

                #region filtro sulla natura dei documenti
                if (this.ddl_tipo_doc.SelectedIndex > -1)
                {
                    if (!ddl_oggetto.SelectedValue.ToString().Equals("F"))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        switch (ddl_tipo_doc.SelectedValue.ToString())
                        {
                            case "Tutti":
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_TUTTI.ToString();
                                break;
                            case "P":
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROTOCOLLATI.ToString();
                                break;
                            case "PA":
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROT_ARRIVO.ToString();
                                break;
                            case "PP":
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROT_PARTENZA.ToString();
                                break;
                            case "NP":
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_NON_PROTOCOLLATI.ToString();
                                break;
                            case "PI":
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROT_INTERNO.ToString();
                                break;
                        }
                        fV1.valore = "true";
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro "visualizza sottoposti"
                if (this.chk_visSott.Visible)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.NO_CERCA_INFERIORI.ToString();
                    if (this.chk_visSott.Checked)
                        fV1.valore = "false";
                    else
                        fV1.valore = "true";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro "oggetto trasmesso"
                string tipo_oggetto = this.ddl_oggetto.SelectedValue;
                //if (this.ddl_fasc.SelectedIndex >= 0)
                //	tipo_oggetto = "F";
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString();
                fV1.valore = tipo_oggetto;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion

                //FILTRI MODIFICATI

                #region Note generali
                if (txt_note_generali.Text != "")
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = "NOTE_GENERALI";
                    fV1.valore = this.txt_note_generali.Text.ToString();
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region Note individuali
                if (txt_note_individuali.Text != "")
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = "NOTE_INDIVIDUALI";
                    fV1.valore = this.txt_note_individuali.Text.ToString();
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion


                #region Data scadenza precedente e successiva
                if (!string.IsNullOrEmpty(this.cld_scadenza_dal.Text) && !string.IsNullOrEmpty(cld_scadenza_al.Text))
                {
                    if (!utils.isDate(this.cld_scadenza_dal.Text) || !utils.isDate(this.cld_scadenza_al.Text))
                    {
                        string msg = "WarningAddressBookBirthdayInvalid";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = "SCADENZA_SUCCESSIVA_AL";
                    fV1.valore = this.cld_scadenza_dal.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = "SCADENZA_PRECEDENTE_IL";
                    fV1.valore = this.cld_scadenza_al.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region Data scadenza singola
                if (!string.IsNullOrEmpty(this.cld_scadenza_dal.Text) && string.IsNullOrEmpty(this.cld_scadenza_al.Text))
                {
                    if (!utils.isDate(this.cld_scadenza_dal.Text))
                    {
                        string msg = "WarningAddressBookBirthdayInvalid";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = "SCADENZA_IL";
                    fV1.valore = this.cld_scadenza_dal.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region "Filtro TAR" - Tutte Accettate Rifiutate Pendenti
                string tipoRicerca = string.Empty;
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.ACCETTATA_RIFIUTATA.ToString();

                if (cbx_Acc.Checked && !cbx_Rif.Checked)
                {
                    fV1.valore = "A";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    tipoRicerca = "ACC";
                }

                if (!cbx_Acc.Checked && cbx_Rif.Checked)
                {
                    fV1.valore = "R";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    tipoRicerca = "RIF";
                }

                if (cbx_Acc.Checked && cbx_Rif.Checked)
                {
                    fV1.valore = "A_R";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    tipoRicerca = "ACC_RIF";
                }

                if (cbx_Pendenti.Checked)
                {
                    fV1.valore = "P";
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.PENDENTI.ToString();
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                switch (ddl_TAR.SelectedItem.Text)
                {
                    case "Valore Singolo":
                        if (!string.IsNullOrEmpty(this.dataUno_TAR.Text))
                        {
                            fV1 = new DocsPaWR.FiltroRicerca();
                            if (tipoRicerca == "ACC")
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE.ToString();
                            if (tipoRicerca == "RIF")
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO.ToString();
                            if (tipoRicerca == "ACC_RIF")
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF.ToString();
                            fV1.valore = this.dataUno_TAR.Text.ToString();
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                            res = checkData(fV1.argomento, fV1.valore);
                        }
                        break;
                    case "Intervallo":
                        if (!string.IsNullOrEmpty(this.dataUno_TAR.Text) && !string.IsNullOrEmpty(this.dataDue_TAR.Text))
                        {
                            if (Convert.ToDateTime(this.dataUno_TAR.Text) >= Convert.ToDateTime(this.dataDue_TAR.Text))
                            {
                                res = false;
                                return res;
                            }

                            fV1 = new DocsPaWR.FiltroRicerca();
                            if (tipoRicerca == "ACC")
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_DA.ToString();
                            if (tipoRicerca == "RIF")
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_DA.ToString();
                            if (tipoRicerca == "ACC_RIF")
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_DA.ToString();
                            fV1.valore = this.dataUno_TAR.Text.ToString();
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                            res = checkData(fV1.argomento, fV1.valore);
                            if (!res)
                                return res;

                            fV1 = new DocsPaWR.FiltroRicerca();
                            if (tipoRicerca == "ACC")
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_A.ToString();
                            if (tipoRicerca == "RIF")
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_A.ToString();
                            if (tipoRicerca == "ACC_RIF")
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_A.ToString();
                            fV1.valore = this.dataDue_TAR.Text.ToString();
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                            res = checkData(fV1.argomento, fV1.valore);
                        }

                        if (!string.IsNullOrEmpty(this.dataUno_TAR.Text) && string.IsNullOrEmpty(this.dataDue_TAR.Text))
                        {
                            fV1 = new DocsPaWR.FiltroRicerca();
                            if (tipoRicerca == "ACC")
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_DA.ToString();
                            if (tipoRicerca == "RIF")
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_DA.ToString();
                            if (tipoRicerca == "ACC_RIF")
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_DA.ToString();
                            fV1.valore = this.dataUno_TAR.Text.ToString();
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                            res = checkData(fV1.argomento, fV1.valore);
                        }

                        if (this.dataUno_TAR.Text == "" && this.dataDue_TAR.Text != "")
                        {
                            fV1 = new DocsPaWR.FiltroRicerca();
                            if (tipoRicerca == "ACC")
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_A.ToString();
                            if (tipoRicerca == "RIF")
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_A.ToString();
                            if (tipoRicerca == "ACC_RIF")
                                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_A.ToString();
                            fV1.valore = this.dataDue_TAR.Text.ToString();
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                            res = checkData(fV1.argomento, fV1.valore);
                        }
                        break;

                    case "Oggi":
                        fV1 = new DocsPaWR.FiltroRicerca();
                        if (tipoRicerca == "ACC")
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_TODAY.ToString();
                        if (tipoRicerca == "RIF")
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_TODAY.ToString();
                        if (tipoRicerca == "ACC_RIF")
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_TODAY.ToString();
                        fV1.valore = "1";
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        res = true;
                        break;

                    case "Settimana Corr.":
                        fV1 = new DocsPaWR.FiltroRicerca();
                        if (tipoRicerca == "ACC")
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_SC.ToString();
                        if (tipoRicerca == "RIF")
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_SC.ToString();
                        if (tipoRicerca == "ACC_RIF")
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_SC.ToString();
                        fV1.valore = "1";
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        res = true;
                        break;

                    case "Mese Corrente":
                        fV1 = new DocsPaWR.FiltroRicerca();
                        if (tipoRicerca == "ACC")
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACCETTAZIONE_MC.ToString();
                        if (tipoRicerca == "RIF")
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_RIFIUTO_MC.ToString();
                        if (tipoRicerca == "ACC_RIF")
                            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DATA_ACC_RIF_MC.ToString();
                        fV1.valore = "1";
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        res = true;
                        break;
                }
                #endregion "Filtro TAR" - Tutte Accettate Rifiutate Pendenti

                #region Filtro non lavorate
                if (cbx_no_lavorate.Checked)
                {
                    //Affichè venga applicato il filtro non lavorate occorre selezionare un utente
                    if (this.select_sottoposto.SelectedValue.Equals("tutti") || this.select_sottoposto.SelectedValue.Equals("altri"))
                    {
                        string msg = "WarningNonLavorate";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return false;
                    }
                    else
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.NON_LAVORATE_DA_UTENTE_NOTIFICATO.ToString();
                        fV1.valore = fV1.valore = this.select_sottoposto.SelectedValue;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region FILTRO SU AL MIO RUOLO
                if (this.chk_mio_ruolo.Checked == true)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.AL_MIO_RUOLO.ToString();

                    fV1.valore = "true";

                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region FILTRO SU A ME STESSO
                if (this.chk_me_stesso.Checked == true)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.A_ME_STESSO.ToString();
                    fV1.valore = "true";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region FILTRO SOTTOPOSTO RUOLO RUBRICA
                if (!this.TxtCodeRole.Text.Equals("") && !this.TxtDescriptionRole.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.RUOLO_SOTTOPOSTO.ToString();
                    fV1.valore = this.IdRole.Value;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    // Aggiunta filtro per estensione ricerca a storicizzati
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = FiltriTrasmissioneNascosti.RUOLO_EXTEND_TO_HISTORICIZED.ToString();
                    fV1.valore = this.chkHistoricizedRole.Checked.ToString();
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                #endregion

                #region FILTRO SOTTOPOSTO PERSONA RUBRICA
                if (!this.select_sottoposto.SelectedValue.Equals(string.Empty))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.PERSONA_SOTTOPOSTA.ToString();
                    fV1.valore = this.select_sottoposto.SelectedValue;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region FILTRO TIPOLOGIA DOCUMENTO
                if (this.DocumentDdlTypeDocument.SelectedIndex > 0)
                {
                    if (this.ddl_oggetto.SelectedIndex == 0)
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TIPO_ATTO.ToString();
                        fV1.valore = this.DocumentDdlTypeDocument.SelectedItem.Value;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                        this.SaveTemplateDocument();
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.PROFILAZIONE_DINAMICA.ToString();
                        fV1.template = this.Template;
                        fV1.valore = "Profilazione Dinamica";
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                }
                #endregion

                #region FILTRO DIAGRAMMA A STATI
                if (this.ddlStateCondition.Visible && this.ddlStateCondition.SelectedIndex != 0)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DIAGRAMMA_STATO_DOC.ToString();
                    string cond = " AND (DPA_DIAGRAMMI.DOC_NUMBER = A.DOCNUMBER AND DPA_DIAGRAMMI.ID_STATO = " + this.ddlStateCondition.SelectedValue + ") ";
                    fV1.valore = cond;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro tipologia fascicolo e profilazione dinamica

                if (this.ddl_oggetto.SelectedValue.Equals("F"))
                {
                    if (this.DocumentDdlTypeDocument.SelectedIndex > 0)
                    {
                        //this.Template.SYSTEM_ID = Int32.Parse(this.DocumentDdlTypeDocument.SelectedItem.Value);
                        //this.Template.DESCRIZIONE = this.DocumentDdlTypeDocument.SelectedItem.Text;
                        //this.SaveTemplateDocument();
                        //fV1 = new DocsPaWR.FiltroRicerca();
                        //fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.PROFILAZIONE_DINAMICA.ToString();
                        //fV1.template = this.Template;
                        //fV1.valore = "Profilazione Dinamica";
                        //fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TIPOLOGIA_FASCICOLO.ToString();
                        fV1.valore = this.DocumentDdlTypeDocument.SelectedItem.Value;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);


                    }
                }

                #endregion

                #region filtro DIAGRAMMI DI STATO FASCICOLI
                if (this.ddl_oggetto.SelectedValue.Equals("F"))
                {
                    if (this.ddlStateCondition.Visible && this.ddlStateCondition.SelectedIndex != 0)
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DIAGRAMMA_STATO_FASC.ToString();
                        fV1.valore = this.ddlStateCondition.SelectedValue;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion filtro DIAGRAMMI DI STATO FASCICOLI

                #region Ordinamento

                // Reperimento del filtro da utilizzare per la griglia
                List<FiltroRicerca> filterList = GridManager.GetOrderFilterForTransmission(this.ddlOrder.SelectedValue, this.ddlOrderDirection.SelectedValue);

                // Se la lista è valorizzata vengono aggiunti i filtri
                if (filterList != null)
                    foreach (FiltroRicerca filter in filterList)
                        fVList = utils.addToArrayFiltroRicerca(fVList, filter);

                #endregion

                qV[0] = fVList;
                this.SearchFilters = qV;
                return res;

            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
                res = false;
                return res;
            }
        }


        private string getArgomento(bool codRubrica, string tipoCorr, string tipoTrasm)
        {
            string argomento = "";
            switch (tipoCorr)
            {
                case "R":
                    if (codRubrica)
                    {
                        if (tipoTrasm.Equals("R"))
                            argomento = DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_MITT_RUOLO.ToString();
                        else
                            argomento = DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_DEST_RUOLO.ToString();
                    }
                    else
                    {
                        if (tipoTrasm.Equals("R"))
                            argomento = DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_RUOLO.ToString();
                        else
                            argomento = DocsPaWR.FiltriTrasmissioneNascosti.DESTINATARIO_RUOLO.ToString();
                    }
                    break;
                case "P":
                    if (codRubrica)
                    {
                        if (tipoTrasm.Equals("R"))
                            argomento = DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_MITT_UTENTE.ToString();
                        else
                            argomento = DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_DEST_UTENTE.ToString();
                    }
                    else
                    {
                        if (tipoTrasm.Equals("R"))
                            argomento = DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_UTENTE.ToString();
                        else
                            argomento = DocsPaWR.FiltriTrasmissioneNascosti.DESTINATARIO_UTENTE.ToString();
                    }
                    break;
                case "U":
                    if (codRubrica)
                    {
                        if (tipoTrasm.Equals("R"))
                            argomento = DocsPaWR.FiltriTrasmissioneNascosti.ID_UO_MITT.ToString();
                        else
                            argomento = DocsPaWR.FiltriTrasmissioneNascosti.ID_UO_DEST.ToString();
                    }
                    else
                    {
                        if (tipoTrasm.Equals("R"))
                            argomento = DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_UO.ToString();
                        else
                            argomento = DocsPaWR.FiltriTrasmissioneNascosti.DESTINATARIO_UO.ToString();
                    }
                    break;
            }
            return argomento;
        }

        private bool checkData(string argomento, string valore)
        {
            if (argomento.Equals("TRASMISSIONE_IL") ||
                argomento.Equals("TRASMISSIONE_SUCCESSIVA_AL") ||
                argomento.Equals("TRASMISSIONE_PRECEDENTE_IL") ||
                //argomento.Equals("TRASMISSIONE_SC") ||
                //argomento.Equals("TRASMISSIONE_MC") ||
                //argomento.Equals("TRASMISSIONE_TODAY") ||
                argomento.Equals("ACCETTATA_RIFIUTATA_IL") ||
                argomento.Equals("ACCETTATA_RIFIUTATA_SUCCESSIVA_AL") ||
                argomento.Equals("ACCETTATA_RIFIUTATA_PRECEDENTE_IL") ||
                argomento.Equals("SCADENZA_IL") ||
                argomento.Equals("SCADENZA_SUCCESSIVA_AL") ||
                argomento.Equals("SCADENZA_PRECEDENTE_IL") ||
                argomento.Equals("RISPOSTA_IL") ||
                argomento.Equals("RISPOSTA_SUCCESSIVA_AL") ||
                argomento.Equals("RISPOSTA_PRECEDENTE_IL") ||
                argomento.Equals("DATA_ACCETTAZIONE") ||
                argomento.Equals("DATA_ACCETTAZIONE_DA") ||
                argomento.Equals("DATA_ACCETTAZIONE_A") ||
                //argomento.Equals("DATA_ACCETTAZIONE_TODAY") ||
                //argomento.Equals("DATA_ACCETTAZIONE_SC") ||
                //argomento.Equals("DATA_ACCETTAZIONE_MC") ||
                argomento.Equals("DATA_RIFIUTO") ||
                argomento.Equals("DATA_RIFIUTO_DA") ||
                argomento.Equals("DATA_RIFIUTO_A") ||
                //argomento.Equals("DATA_RIFIUTO_TODAY") ||
                //argomento.Equals("DATA_RIFIUTO_SC") ||
                //argomento.Equals("DATA_RIFIUTO_MC") ||
                argomento.Equals("DATA_ACC_RIF") ||
                argomento.Equals("DATA_ACC_RIF_DA") ||
                argomento.Equals("DATA_ACC_RIF_A")
                //argomento.Equals("DATA_ACC_RIF_TODAY") ||
                //argomento.Equals("DATA_ACC_RIF_SC") ||
                //argomento.Equals("DATA_ACC_RIF_MC")
                )
            {
                if (!utils.isDate(valore))
                    return false;
                if (Convert.ToDateTime(valore) < (new DateTime(1754, 01, 01)))
                    return false;
            }
            return true;
        }

        private void SaveTemplateDocument()
        {
            int result = 0;
            for (int i = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i];
                if (controllaCampi(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString()))
                {
                    result++;
                }
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

                    break;
                case "Contatore":
                    CustomTextArea contatoreDa = (CustomTextArea)PnlTypeDocument.FindControl("da_" + idOggetto);
                    CustomTextArea contatoreA = (CustomTextArea)PnlTypeDocument.FindControl("a_" + idOggetto);
                    //Controllo la valorizzazione di campi ed eventualmente notifico gli errori
                    CustomTextArea dataRepertorioDa = (CustomTextArea)this.PnlTypeDocument.FindControl("TxtDataRepertorioDa_" + idOggetto);
                    CustomTextArea dataRepertorioA = (CustomTextArea)this.PnlTypeDocument.FindControl("TxtDataRepertorioA_" + idOggetto);
                    if (dataRepertorioDa != null && dataRepertorioA != null)
                    {
                        if (dataRepertorioDa.Text != "" && dataRepertorioA.Text != "")
                            oggettoCustom.DATA_INSERIMENTO = dataRepertorioDa.Text + "@" + dataRepertorioA.Text;

                        if (dataRepertorioDa.Text != "" && dataRepertorioA.Text == "")
                            oggettoCustom.DATA_INSERIMENTO = dataRepertorioDa.Text;
                    }
                    switch (oggettoCustom.TIPO_CONTATORE)
                    {
                        case "T":
                            if (contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }
                            break;
                        case "A":
                            DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (contatoreDa.Text.Equals("") && ddlAoo.SelectedValue.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }

                            if (contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";
                            break;
                        case "R":
                            DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (contatoreDa.Text.Equals("") && ddlRf.SelectedValue.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }

                            if (contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";
                            break;
                    }

                    if (contatoreDa.Text.Equals("") && contatoreA.Text != "")
                    {
                        //SetFocus(contatoreDa);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }

                    try
                    {
                        if (contatoreDa.Text != null && contatoreDa.Text != "")
                            Convert.ToInt32(contatoreDa.Text);
                        if (contatoreA.Text != null && contatoreA.Text != "")
                            Convert.ToInt32(contatoreA.Text);
                    }
                    catch (Exception ex)
                    {
                        //SetFocus(contatoreDa);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }


                    //I campi sono valorizzati correttamente procedo
                    if (contatoreDa.Text != "" && contatoreA.Text != "")
                        oggettoCustom.VALORE_DATABASE = contatoreDa.Text + "@" + contatoreA.Text;

                    if (contatoreDa.Text != "" && contatoreA.Text == "")
                        oggettoCustom.VALORE_DATABASE = contatoreDa.Text;

                    switch (oggettoCustom.TIPO_CONTATORE)
                    {
                        case "A":
                            DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                            break;
                        case "R":
                            DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
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
                    //TextBox contatoreSDa = (TextBox)PnlTypeDocument.FindControl("da_" + idOggetto);
                    //TextBox contatoreSA = (TextBox)PnlTypeDocument.FindControl("a_" + idOggetto);
                    //TextBox sottocontatoreDa = (TextBox)PnlTypeDocument.FindControl("da_sottocontatore_" + idOggetto);
                    //TextBox sottocontatoreA = (TextBox)PnlTypeDocument.FindControl("a_sottocontatore_" + idOggetto);
                    //UserControls.Calendar dataSottocontatoreDa = (UserControls.Calendar)PnlTypeDocument.FindControl("da_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString());
                    //UserControls.Calendar dataSottocontatoreA = (UserControls.Calendar)PnlTypeDocument.FindControl("a_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString());

                    ////Controllo la valorizzazione di campi ed eventualmente notifico gli errori
                    //switch (oggettoCustom.TIPO_CONTATORE)
                    //{
                    //    case "T":
                    //        if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals("") &&
                    //            sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals("") &&
                    //            dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals("")
                    //            )
                    //        {
                    //            //SetFocus(contatoreDa);
                    //            oggettoCustom.VALORE_DATABASE = "";
                    //            oggettoCustom.VALORE_SOTTOCONTATORE = "";
                    //            oggettoCustom.DATA_INSERIMENTO = "";
                    //            return true;
                    //        }
                    //        break;
                    //    case "A":
                    //        DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                    //        if (contatoreSDa.Text.Equals("") && sottocontatoreDa.Text.Equals("") && ddlAoo.SelectedValue.Equals(""))
                    //        {
                    //            //SetFocus(contatoreDa);
                    //            oggettoCustom.VALORE_DATABASE = "";
                    //            oggettoCustom.VALORE_SOTTOCONTATORE = "";
                    //            return true;
                    //        }

                    //        if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals(""))
                    //            oggettoCustom.VALORE_DATABASE = "";

                    //        if (sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals(""))
                    //            oggettoCustom.VALORE_SOTTOCONTATORE = "";

                    //        if (dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals(""))
                    //            oggettoCustom.DATA_INSERIMENTO = "";
                    //        break;
                    //    case "R":
                    //        DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                    //        if (contatoreSDa.Text.Equals("") && sottocontatoreDa.Text.Equals("") && ddlRf.SelectedValue.Equals(""))
                    //        {
                    //            //SetFocus(contatoreDa);
                    //            oggettoCustom.VALORE_DATABASE = "";
                    //            oggettoCustom.VALORE_SOTTOCONTATORE = "";
                    //            return true;
                    //        }

                    //        if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals(""))
                    //            oggettoCustom.VALORE_DATABASE = "";

                    //        if (sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals(""))
                    //            oggettoCustom.VALORE_SOTTOCONTATORE = "";

                    //        if (dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals(""))
                    //            oggettoCustom.DATA_INSERIMENTO = "";
                    //        break;
                    //}

                    //if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals("") &&
                    //    sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals("") &&
                    //    dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals("")
                    //    )
                    //{
                    //    //SetFocus(contatoreDa);
                    //    oggettoCustom.VALORE_DATABASE = "";
                    //    oggettoCustom.VALORE_SOTTOCONTATORE = "";
                    //    oggettoCustom.DATA_INSERIMENTO = "";
                    //    return true;
                    //}

                    //if (contatoreSDa.Text != null && contatoreSDa.Text != "")
                    //    Convert.ToInt32(contatoreSDa.Text);
                    //if (contatoreSA.Text != null && contatoreSA.Text != "")
                    //    Convert.ToInt32(contatoreSA.Text);
                    //if (sottocontatoreDa.Text != null && sottocontatoreDa.Text != "")
                    //    Convert.ToInt32(sottocontatoreDa.Text);
                    //if (sottocontatoreA.Text != null && sottocontatoreA.Text != "")
                    //    Convert.ToInt32(sottocontatoreA.Text);


                    ////I campi sono valorizzati correttamente procedo
                    //if (contatoreSDa.Text != "" && contatoreSA.Text != "")
                    //    oggettoCustom.VALORE_DATABASE = contatoreSDa.Text + "@" + contatoreSA.Text;

                    //if (contatoreSDa.Text != "" && contatoreSA.Text == "")
                    //    oggettoCustom.VALORE_DATABASE = contatoreSDa.Text;

                    //if (sottocontatoreDa.Text != "" && sottocontatoreA.Text != "")
                    //    oggettoCustom.VALORE_SOTTOCONTATORE = sottocontatoreDa.Text + "@" + sottocontatoreA.Text;

                    //if (sottocontatoreDa.Text != "" && sottocontatoreA.Text == "")
                    //    oggettoCustom.VALORE_SOTTOCONTATORE = sottocontatoreDa.Text;

                    //if (dataSottocontatoreDa.Text != "" && dataSottocontatoreA.Text != "")
                    //    oggettoCustom.DATA_INSERIMENTO = dataSottocontatoreDa.Text + "@" + dataSottocontatoreA.Text;

                    //if (dataSottocontatoreDa.Text != "" && dataSottocontatoreA.Text == "")
                    //    oggettoCustom.DATA_INSERIMENTO = dataSottocontatoreDa.Text;

                    //switch (oggettoCustom.TIPO_CONTATORE)
                    //{
                    //    case "A":
                    //        DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                    //        oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                    //        break;
                    //    case "R":
                    //        DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                    //        oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
                    //        break;
                    //}
                    break;


            }
            return false;
        }


        protected void SearchTransmissionRemoveFilters_Click(object sender, EventArgs e)
        {
            try
            {
                this.ClearSessionProperties();
                this.LoadReasons();
                if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                {
                    this.Template = null;
                    this.PnlTypeDocument.Controls.Clear();
                    if (this.EnableStateDiagram)
                    {
                        this.DocumentDdlStateDiagram.ClearSelection();
                        this.PnlStateDiagram.Visible = false;
                        this.ddlStateCondition.Visible = false;
                    }
                    this.DocumentDdlTypeDocument.SelectedIndex = 0;
                    this.UpPnlTypeDocument.Update();
                }
                //this.SetTransmissionType("R");
                this.TransmLinkReceived_Click(null, null);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitializePage()
        {
            this.InitializeLanguage();
            this.LoadKeys();
            this.InitializePageSize();
            this.LoadMassiveOperation();
            Session.Remove("itemUsedSearch");
            Session.Remove("idRicercaSalvata");

            schedaRicerca = new SearchManager(KEY_SCHEDA_RICERCA, UserManager.GetUserInSession(), RoleManager.GetRoleInSession(), this);
            Session[SearchManager.SESSION_KEY] = schedaRicerca;

            this.PopulateDDLSavedSearches();
            this.Ds = RoleManager.getUtentiInRuoloSottoposto(UIManager.RoleManager.GetRoleInSession().systemId);
            this.ClearSessionProperties();
            schedaRicerca = new SearchManager(KEY_SCHEDA_RICERCA, UserManager.GetUserInSession(), RoleManager.GetRoleInSession(), this);
            Session[SearchManager.SESSION_KEY] = schedaRicerca;
            this.LoadTypeDocuments();
            this.ComboTipoFileAcquisiti();
            this.LoadReasons();
            this.SetTransmissionType("R");
            this.M_si_img.Attributes.Add("onclick", "enableField(this.id);");
            this.Registry = RoleManager.GetRoleInSession().registri[0];
            UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
            this.SetAjaxAddressBook();
            this.ListCheck = new Dictionary<string, string>();
        }

        private void LoadMassiveOperation()
        {
            //this.SearchTransmissionExportExcel.Items.Add(new ListItem("", ""));
            //string title = string.Empty;
            //string language = UIManager.UserManager.GetUserLanguage();

            //title = Utils.Languages.GetLabelFromCode("MASSIVEXPORTDOC", language);
            //this.SearchTransmissionExportExcel.Items.Add(new ListItem(title, "MASSIVEXPORTDOC"));
        }

        protected void InitializePageSize()
        {
            string keyValue = Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_PAGING_ROW_TRASM.ToString());
            int tempValue = 0;
            if (!string.IsNullOrEmpty(keyValue))
            {
                tempValue = Convert.ToInt32(keyValue);
                if (tempValue >= 20 || tempValue <= 50)
                {
                    this.PageSize = tempValue;
                }
            }
        }


        private void ComboTipoFileAcquisiti()
        {
            ArrayList tipoFile = new ArrayList();
            tipoFile = DocumentManager.getExtFileAcquisiti(this);
            //bool firmati = false;
            for (int i = 0; i < tipoFile.Count; i++)
            {
                if (!tipoFile[i].ToString().Contains("P7M"))
                {
                    ListItem item = new ListItem(tipoFile[i].ToString());
                    this.ddl_tipoFileAcquisiti.Items.Add(item);
                }
            }
        }

        private void LoadTypeProjects()
        {
            if (this.ListaTipiFasc == null || this.ListaTipiFasc.Count == 0)
            {
                this.ListaTipiFasc = new ArrayList(ProfilerProjectManager.getTipoFascFromRuolo(UserManager.GetInfoUser().idAmministrazione, RoleManager.GetRoleInSession().idGruppo, "1"));
            }

            this.DocumentDdlTypeDocument.Items.Clear();

            ListItem item = new ListItem(string.Empty, string.Empty);
            if (this.DocumentDdlTypeDocument.Items.Count == 0)
            {
                this.DocumentDdlTypeDocument.Items.Add(item);
            }
            for (int i = 0; i < this.ListaTipiFasc.Count; i++)
            {
                DocsPaWR.Templates templates = (DocsPaWR.Templates)this.ListaTipiFasc[i];
                ListItem item_1 = new ListItem();
                item_1.Value = templates.SYSTEM_ID.ToString();
                item_1.Text = templates.DESCRIZIONE;

                //Christian - Ticket OC0000001490459 - Ricerca fascicoli: ripristino tipologia successivo a ordinamento tramite griglia.
                if (this.DocumentDdlTypeDocument.Items.FindByValue(templates.SYSTEM_ID.ToString()) == null)
                {
                    if (templates.IPER_FASC_DOC == "1")
                        this.DocumentDdlTypeDocument.Items.Insert(1, item_1);
                    else
                        this.DocumentDdlTypeDocument.Items.Add(item_1);
                }

            }
        }

        private void LoadKeys()
        {

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()].Equals("1"))
            {
                this.CustomDocuments = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()].Equals("1"))
            {
                this.EnableStateDiagram = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VISUALIZZA_CODICE_MODELLI_TRASM.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VISUALIZZA_CODICE_MODELLI_TRASM.ToString()].Equals("1"))
            {
                this.ViewCodeTransmissionModels = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_TRANS_FORW.ToString())) && Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_TRANS_FORW.ToString()).Equals("1"))
            {
                this.PermitUrges = true;
            }


            if (!(!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString())) && Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString()).Equals("1")))
                this.ddl_tipo_doc.Items.Remove(this.ddl_tipo_doc.Items.FindByValue("PI"));
        }

        private void SetTransmissionType(string type)
        {
            string language = UIManager.UserManager.GetUserLanguage();

            if (type.Equals("R"))
            {
                this.PlcUserFilter.Visible = true;
                this.LitUserTransmission.Text = Utils.Languages.GetLabelFromCode("chk_me_stesso", language) + " (" + UserManager.GetUserInSession().descrizione + ")";
                this.chk_me_stesso.Checked = true;
                this.chk_mio_ruolo.Checked = true;
                this.LitSearchNotifyUser.Text = Utils.Languages.GetLabelFromCode("LitSearchNotifyUser", language);
                this.DocumentDdlTypeDocument.Attributes.Remove("class");
                this.LiSearchTransSender.Text = Utils.Languages.GetLabelFromCode("LiSearchTransSender", language);

                this.cbx_no_lavorate.Checked = false;
                this.cbx_no_lavorate.Visible = true;

                this.gridViewResult.Columns[3].Visible = true;
                this.gridViewResult.Columns[4].Visible = true;
                this.gridViewResult.Columns[1].Visible = false;
                this.gridViewResult.Columns[5].Visible = false;

                this.BtnUrgesAll.Visible = false;
                this.BtnUrgesSelected.Visible = false;
                this.upPnlButtons.Update();
            }
            else
            {
                this.PlcUserFilter.Visible = false;
                this.LitSearchNotifyUser.Text = Utils.Languages.GetLabelFromCode("LitSearchNotifyUserEff", language);
                this.DocumentDdlTypeDocument.Attributes.Remove("class");

                this.LiSearchTransSender.Text = Utils.Languages.GetLabelFromCode("LiSearchTransRecipient", language);

                this.gridViewResult.Columns[3].Visible = false;
                this.gridViewResult.Columns[4].Visible = false;
                this.gridViewResult.Columns[5].Visible = true;

                if (this.PermitUrges)
                {
                    this.gridViewResult.Columns[1].Visible = true;
                    this.BtnUrgesAll.Visible = true;
                    this.BtnUrgesSelected.Visible = true;
                    this.upPnlButtons.Update();
                }

                this.cbx_no_lavorate.Checked = false;
                this.cbx_no_lavorate.Visible = false;
            }

            this.ResetCheckDocInworking();
            this.ResetSelectSottoposti();
            this.chk_visSott.Checked = false;
            this.ddl_tipo_doc.Visible = true;
            this.ddl_oggetto.SelectedIndex = 0;
            this.ddl_tipo_doc.SelectedIndex = 1;
            this.txtCodiceCreatore.Text = string.Empty;
            this.txtDescrizioneCreatore.Text = string.Empty;
            this.idCreatore.Value = string.Empty;

            this.PlcDocInWorking.Visible = true;


            this.gridViewResult.DataSource = null;
            this.gridViewResult.DataBind();

            this.Result = null;
            this.SelectedRow = string.Empty;
            this.SearchFilters = null;
            this.RecordCount = 0;
            this.PageCount = 0;
            this.SelectedPage = 1;
            this.ListSecurityObject = new Dictionary<string, int>();

            this.grid_pageindex.Value = this.SelectedPage.ToString();
            this.gridViewResult.PageIndex = this.PageCount;
            this.gridViewResult.SelectedIndex = this.SelectedPage;
            this.rblOwnerType.SelectedValue = "R";
            this.idCreatore.Value = string.Empty;
            this.txtCodiceCreatore.Text = string.Empty;
            this.txtDescrizioneCreatore.Text = string.Empty;
            this.chkCreatoreExtendHistoricized.Checked = false;

            this.ddl_dataTrasm_SelectedIndexChanged(null, null);
            this.ddl_ragioni.SelectedIndex = 0;
            this.UpPnlTransmReason.Update();
            this.cbx_Acc.Checked = false;
            this.cbx_Rif.Checked = false;
            this.cbx_Pendenti.Checked = false;

            this.cbx_no_lavorate.Checked = false;

            this.PnlDateOthers.Visible = false;
            this.ddl_TAR_SelectedIndexChanged(null, null);
            this.txt_note_generali.Text = string.Empty;
            this.cld_scadenza_dal.Text = string.Empty;
            this.cld_scadenza_al.Text = string.Empty;

            GridManager.CompileDdlOrderAndSetOrderFilterTransmission(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);

            this.UpPnlOtherFilters.Update();

            this.UpPnlDateTransmission.Update();

            this.upPnlCreatore.Update();


            this.BuildGridNavigator();
            this.UpnlGrid.Update();

            this.projectLblNumeroDocumenti.Text = this.GetLabel("projectLblNumeroTransm") + " (" + this.RecordCount.ToString() + ") " + this.GetLabel("projectLblNumeroTransmFound");
            this.UpnlNumerodocumenti.Update();
            this.SearchDocumentAdvancedRemove.Enabled = false;
            this.UpPnlType.Update();
            this.UpPnlObject.Update();
            this.UpPnlDocInWorking.Update();
            this.upPnlButtons.Update();

            // reset check list for massive ops
            this.ListCheck = new Dictionary<string, string>();
            this.HiddenItemsChecked.Value = string.Empty;
            this.HiddenItemsUnchecked.Value = string.Empty;
            this.upPnlButtons.Update();
        }



        protected void ResetCheckDocInworking()
        {
            this.P_Prot.Checked = false;
            this.M_Fasc.Checked = false;
            this.M_si_img.Checked = false;
            this.M_Img.Checked = false;
            this.UpPnlDocInWorking.Update();
        }


        private void ClearSessionProperties()
        {
            this.Role = UIManager.RoleManager.GetRoleInSession();
            this.TxtCodeRole.Text = this.Role.codiceRubrica;
            this.TxtDescriptionRole.Text = this.Role.descrizione;
            this.IdRole.Value = this.Role.systemId;
            this.TypeSearch = "R";
            this.ResetSelectSottoposti();
            this.Color = new List<int>();
            this.Template = null;

            this.Result = null;
            this.SelectedRow = string.Empty;
            this.SearchFilters = null;
            this.RecordCount = 0;
            this.PageCount = 0;
            this.SelectedPage = 1;
            this.ListaRagioni = null;
            this.ListSecurityObject = new Dictionary<string, int>();

            this.Labels = DocumentManager.GetLettereProtocolli();

            UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);

            // Caricamento della griglia se non ce n'è una già selezionata
            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Transmission)
            {
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Transmission);
            }

            GridManager.CompileDdlOrderAndSetOrderFilterTransmission(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);



            this.SearchTransmissionExportExcel.Enabled = false;
            //this.ShowGrid(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());

            this.ListCheck = null;
        }

        private void ResetSelectSottoposti()
        {
            this.select_sottoposto.Items.Clear();
            this.Color = new List<int>();

            if (this.Ds != null && this.Ds.Tables.Count != 0 && this.Ds.Tables[0].Rows.Count != 0)
            {
                this.select_sottoposto.Items.Add(new ListItem("<<qualsiasi utente>>", "tutti"));

                int countCol = 1;
                //SOLO PER IL MIO RULO
                if (((this.Role.codiceRubrica).ToUpper()).Equals((this.TxtCodeRole.Text).ToUpper()))
                {
                    this.select_sottoposto.Items.Add(new ListItem("<<gli altri utenti>>", "altri"));
                    countCol = 2;
                }
                else
                {
                    this.chk_me_stesso.Checked = false;
                }

                this.select_sottoposto.DataSource = this.Ds;
                this.select_sottoposto.DataBind();
                if (this.select_sottoposto.Items.FindByValue(UIManager.UserManager.GetUserInSession().idPeople) != null)
                {
                    this.select_sottoposto.Items.FindByValue(UIManager.UserManager.GetUserInSession().idPeople).Selected = true;
                }
                //this.select_sottoposto.Items.FindByValue(UIManager.UserManager.GetUserInSession().systemId).Selected = true;

                for (int i = 0; i < this.select_sottoposto.Items.Count - countCol; i++)
                {
                    DataRow row = this.Ds.Tables[0].Rows[i];
                    string dataUtenteRuolo = row["DTA_FINE"].ToString();

                    if (dataUtenteRuolo == null || dataUtenteRuolo.Equals(string.Empty))
                    {
                        this.select_sottoposto.Items[i + countCol].Attributes.Add("style", "color:#333333");
                    }
                    else
                    {
                        this.select_sottoposto.Items[i + countCol].Attributes.Add("style", "color:#990000");
                        this.Color.Add(i + countCol);
                    }

                }
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            this.SearchTransmissionSearch.Text = Utils.Languages.GetLabelFromCode("SearchLabelButton", language);
            this.SearchTransmissionSave.Text = Utils.Languages.GetLabelFromCode("SearchLabelSearchButton", language);
            this.SearchTransmissionRemoveFilters.Text = Utils.Languages.GetLabelFromCode("SearchLabelRemoveFiltersButton", language);
            this.TransmLinkReceived.Text = Utils.Languages.GetLabelFromCode("TransmLinkReceived", language);
            this.TransmLinkEffettuated.Text = Utils.Languages.GetLabelFromCode("TransmLinkEffettuated", language);
            this.LitSearchTransmissions.Text = Utils.Languages.GetLabelFromCode("LitSearchTransmissions", language);
            this.DdlRapidSearch.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DdlRapidSearch", language));
            this.SearchDocumentLitRapidSearch.Text = Utils.Languages.GetLabelFromCode("SearchDocumentLitRapidSearch", language);
            this.SearchTransmissionRecipient.Text = Utils.Languages.GetLabelFromCode("SearchTransmissionRecipient", language);
            //this.chk_me_stesso.Text = Utils.Languages.GetLabelFromCode("chk_me_stesso", language);
            this.LitRoleTransmission.Text = Utils.Languages.GetLabelFromCode("chk_mio_ruolo", language);
            this.DocumentImgRoleAddressBook.AlternateText = Utils.Languages.GetLabelFromCode("VisibilityImgAddressBookRole", language);
            this.DocumentImgRoleAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("VisibilityImgAddressBookRole", language);
            this.LitSearchExtendStor.Text = Utils.Languages.GetLabelFromCode("LitSearchExtendStor", language);
            this.LitSearchNotifyUser.Text = Utils.Languages.GetLabelFromCode("LitSearchNotifyUser", language);
            this.LitSearchTransUnder.Text = Utils.Languages.GetLabelFromCode("LitSearchTransUnder", language);
            this.LitSearchTransObj.Text = Utils.Languages.GetLabelFromCode("LitSearchTransObj", language);
            this.ddl_oggetto.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_oggetto0", language);
            this.ddl_oggetto.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_oggetto1", language);
            this.ddl_tipo_doc.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_tipo_doc0", language);
            this.ddl_tipo_doc.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_tipo_doc1", language);
            this.ddl_tipo_doc.Items[2].Text = DocumentManager.GetDescriptionLabel("A");
            this.ddl_tipo_doc.Items[3].Text = DocumentManager.GetDescriptionLabel("P");
            this.ddl_tipo_doc.Items[4].Text = DocumentManager.GetDescriptionLabel("I");
            this.ddl_tipo_doc.Items[5].Text = Utils.Languages.GetLabelFromCode("ddl_tipo_doc5", language);
            this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("LitSearchTransTypeDoc", language);

            this.ddlStateCondition.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));
            this.DocumentDdlStateDiagram.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlStateDiagram", language));
            this.LiSearchTransDocInComp.Text = Utils.Languages.GetLabelFromCode("LiSearchTransDocInComp", language);
            this.P_Prot.Text = Utils.Languages.GetLabelFromCode("P_Prot", language);
            this.M_Fasc.Text = Utils.Languages.GetLabelFromCode("M_Fasc", language);
            this.M_si_img.Text = Utils.Languages.GetLabelFromCode("M_si_img", language);
            this.M_Img.Text = Utils.Languages.GetLabelFromCode("M_Img", language);
            this.ddl_tipoFileAcquisiti.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("ddl_tipoFileAcquisiti", language));
            this.chk_firmati.Text = Utils.Languages.GetLabelFromCode("chk_firmati", language);
            this.chk_non_firmati.Text = Utils.Languages.GetLabelFromCode("chk_non_firmati", language);
            this.LitSearchTransTypeFileAcq.Text = Utils.Languages.GetLabelFromCode("LitSearchTransTypeFileAcq", language);
            this.optUO.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUO", language);
            this.optUser.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUser", language);
            this.optRole.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptRole", language);
            this.ImgCreatoreAddressBook.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgCustomCorrespondentAddressBookCustom", language);
            this.ImgCreatoreAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgCustomCorrespondentAddressBookCustom", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
            this.chkCreatoreExtendHistoricized.Text = Utils.Languages.GetLabelFromCode("SearchProjectCreatoreExtendHistoricized", language);
            this.LiSearchTransSender.Text = Utils.Languages.GetLabelFromCode("LiSearchTransSender", language);
            this.LitSearchTransDate.Text = Utils.Languages.GetLabelFromCode("LitSearchTransDate", language);
            this.lbl_initdataTrasm.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.ddl_dataTrasm.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_dataTrasm.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_dataTrasm.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_dataTrasm.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_dataTrasm.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.ddl_dataTrasm.Items[5].Text = Utils.Languages.GetLabelFromCode("ddl_data5", language);
            this.ddl_dataTrasm.Items[6].Text = Utils.Languages.GetLabelFromCode("ddl_data6", language);
            this.ddl_dataTrasm.Items[7].Text = Utils.Languages.GetLabelFromCode("ddl_data7", language);

            this.ddl_TAR.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_TAR.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_TAR.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_TAR.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_TAR.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);

            this.LitSearchTransReason.Text = Utils.Languages.GetLabelFromCode("LitSearchTransReason", language);
            this.ddl_ragioni.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("ddl_ragioniSearchTrans", language));
            this.LitSearchTransOtherFilters.Text = Utils.Languages.GetLabelFromCode("LitSearchTransOtherFilters", language);
            this.cbx_Acc.Text = Utils.Languages.GetLabelFromCode("cbx_Acc", language);
            this.cbx_Rif.Text = Utils.Languages.GetLabelFromCode("cbx_Rif", language);
            this.cbx_no_lavorate.Text = Utils.Languages.GetLabelFromCode("cbx_no_lavorate", language);
            this.cbx_no_lavorate.ToolTip = Utils.Languages.GetLabelFromCode("cbx_no_lavorate_tooltip", language);
            this.cbx_Pendenti.Text = Utils.Languages.GetLabelFromCode("cbx_Pendenti", language);
            this.LiSearchData_1.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.LinSearchTransGeneralNote.Text = Utils.Languages.GetLabelFromCode("LinSearchTransGeneralNote", language);
            this.LinSearchTransIndividualNote.Text = Utils.Languages.GetLabelFromCode("LinSearchTransIndividualNote", language);
            this.LitSearchTransEnd.Text = Utils.Languages.GetLabelFromCode("LitSearchTransEnd", language);
            this.LitSearchEndFrom.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
            this.LitSearchEndTo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LitSearchOrderTrans.Text = Utils.Languages.GetLabelFromCode("LitSearchOrderTrans", language);
            //this.SearchTransmissionExportExcel.to("data-placeholder", Utils.Languages.GetLabelFromCode("projectDdlAzioniMassive", language));

            this.ViewDetailTransmission.Title = Utils.Languages.GetLabelFromCode("ViewDetailTransmission", language);

            this.gridViewResult.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("SearchTransmission_date", language);
            this.gridViewResult.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("SearchTransmission_Mitt", language);
            this.gridViewResult.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("SearchTransmission_Reason", language);
            this.gridViewResult.Columns[5].HeaderText = Utils.Languages.GetLabelFromCode("SearchTransmission_Recipient", language);
            this.gridViewResult.Columns[6].HeaderText = Utils.Languages.GetLabelFromCode("SearchTransmission_Scadenza", language);
            this.gridViewResult.Columns[7].HeaderText = Utils.Languages.GetLabelFromCode("SearchTransmission_Doc", language);
            this.gridViewResult.Columns[8].HeaderText = Utils.Languages.GetLabelFromCode("SearchTransmission_Object", language);

            this.SaveSearch.Title = Utils.Languages.GetLabelFromCode("SearchProjectSaveSearchTitle", language);
            this.ModifySearch.Title = Utils.Languages.GetLabelFromCode("SearchProjectModifySearchTitle", language);

            this.SearchDocumentAdvancedRemove.Text = Utils.Languages.GetLabelFromCode("SearchLabelRemoveButton", language);

            this.select_sottoposto.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("select_sottoposto", language));
            this.DocumentDdlTypeDocument.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlTypeDocumentAny", language));

            this.SearchTransmissionExportExcel.AlternateText = Utils.Languages.GetLabelFromCode("SearchTransmissionExportExcel", language);
            this.SearchTransmissionExportExcel.ToolTip = Utils.Languages.GetLabelFromCode("SearchTransmissionExportExcel", language);

            this.ExportDati.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentExportDatiTitle", language));

            this.BtnUrgesAll.Text = Utils.Languages.GetLabelFromCode("SearchTransmissionBtnUrgesAll", language);
            this.BtnUrgesSelected.Text = Utils.Languages.GetLabelFromCode("SearchTransmissionBtnUrgesSelected", language);
            this.SearchProject.Title = Utils.Languages.GetLabelFromCode("SearchProjectTitle", language);
            this.OpenTitolario.Title = Utils.Languages.GetLabelFromCode("TitleClassificationScheme", language);
        }

        protected void ddl_TAR_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.dataUno_TAR.Text = string.Empty;
                this.dataDue_TAR.Text = string.Empty;

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_TAR.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.dataUno_TAR.ReadOnly = false;
                        this.dataDue_TAR.Visible = false;
                        this.LiSearchData_2.Visible = false;
                        this.LiSearchData_1.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.dataUno_TAR.ReadOnly = false;
                        this.dataDue_TAR.ReadOnly = false;
                        this.LiSearchData_2.Visible = true;
                        this.LiSearchData_1.Visible = true;
                        this.dataDue_TAR.Visible = true;
                        this.LiSearchData_1.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LiSearchData_2.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LiSearchData_2.Visible = false;
                        this.dataDue_TAR.Visible = false;
                        this.dataUno_TAR.ReadOnly = true;
                        this.dataUno_TAR.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LiSearchData_1.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 3: //Settimana corrente
                        this.LiSearchData_2.Visible = true;
                        this.dataDue_TAR.Visible = true;
                        this.dataUno_TAR.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.dataDue_TAR.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.dataDue_TAR.ReadOnly = true;
                        this.dataUno_TAR.ReadOnly = true;
                        this.LiSearchData_1.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LiSearchData_2.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LiSearchData_2.Visible = true;
                        this.dataDue_TAR.Visible = true;
                        this.dataUno_TAR.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.dataDue_TAR.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.dataDue_TAR.ReadOnly = true;
                        this.dataUno_TAR.ReadOnly = true;
                        this.LiSearchData_1.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LiSearchData_2.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadReasons()
        {
            if (this.ListaRagioni == null || this.ListaRagioni.Length > 0)
            {
                this.ListaRagioni = TrasmManager.getListaRagioni(this, string.Empty, true);
            }


            if (this.ListaRagioni != null && this.ListaRagioni.Length > 0)
            {
                for (int i = 0; i < this.ListaRagioni.Length; i++)
                {
                    ListItem newItem = new ListItem(this.ListaRagioni[i].descrizione, this.ListaRagioni[i].systemId);
                    this.ddl_ragioni.Items.Add(newItem);
                }
            }

            this.ddl_ragioni.Items.Insert(0, new ListItem());
        }

        protected void ddl_dataTrasm_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.txt_initDataTrasm.Text = string.Empty;
                this.txt_fineDataTrasm.Text = string.Empty;

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dataTrasm.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initDataTrasm.ReadOnly = false;
                        this.txt_fineDataTrasm.Visible = false;
                        this.txt_fineDataTrasm.Visible = false;
                        this.lbl_initdataTrasm.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.txt_initDataTrasm.ReadOnly = false;
                        this.txt_fineDataTrasm.ReadOnly = false;
                        this.txt_fineDataTrasm.Visible = true;
                        this.lbl_initdataTrasm.Visible = true;
                        this.txt_fineDataTrasm.Visible = true;
                        this.lbl_initdataTrasm.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.lbl_finedataTrasm.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.txt_fineDataTrasm.Visible = false;
                        this.txt_fineDataTrasm.Visible = false;
                        this.txt_initDataTrasm.ReadOnly = true;
                        this.txt_initDataTrasm.Text = NttDataWA.Utils.dateformat.toDay();
                        this.lbl_initdataTrasm.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 3: //Settimana corrente
                        this.txt_fineDataTrasm.Visible = true;
                        this.txt_fineDataTrasm.Visible = true;
                        this.txt_initDataTrasm.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_fineDataTrasm.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_fineDataTrasm.ReadOnly = true;
                        this.txt_initDataTrasm.ReadOnly = true;
                        this.lbl_initdataTrasm.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.lbl_finedataTrasm.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.txt_fineDataTrasm.Visible = true;
                        this.txt_fineDataTrasm.Visible = true;
                        this.txt_initDataTrasm.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_fineDataTrasm.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_fineDataTrasm.ReadOnly = true;
                        this.txt_initDataTrasm.ReadOnly = true;
                        this.lbl_initdataTrasm.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.lbl_finedataTrasm.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 5: //Ieri
                        this.lbl_finedataTrasm.Visible = false;
                        this.txt_fineDataTrasm.Visible = false;
                        this.txt_initDataTrasm.ReadOnly = true;
                        this.txt_initDataTrasm.Text = NttDataWA.Utils.dateformat.GetYesterday();
                        this.lbl_initdataTrasm.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDataTrasm.Text = string.Empty;
                        break;
                    case 6: //Ultimi 7 giorni
                        this.lbl_finedataTrasm.Visible = true;
                        this.txt_fineDataTrasm.Visible = true;
                        this.txt_initDataTrasm.Text = NttDataWA.Utils.dateformat.GetLastSevenDay();
                        this.txt_fineDataTrasm.Text = NttDataWA.Utils.dateformat.toDay();
                        this.txt_fineDataTrasm.ReadOnly = true;
                        this.txt_initDataTrasm.ReadOnly = true;
                        this.lbl_initdataTrasm.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.lbl_finedataTrasm.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 7: //Ultimi 31 iorni
                        this.lbl_finedataTrasm.Visible = true;
                        this.txt_fineDataTrasm.Visible = true;
                        this.txt_initDataTrasm.Text = NttDataWA.Utils.dateformat.GetLastThirtyOneDay();
                        this.txt_fineDataTrasm.Text = NttDataWA.Utils.dateformat.toDay();
                        this.txt_fineDataTrasm.ReadOnly = true;
                        this.txt_initDataTrasm.ReadOnly = true;
                        this.lbl_initdataTrasm.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.lbl_finedataTrasm.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CombineRowsHover", "CombineRowsHover();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshExpand", "RefreshExpand();", true);
        }

        protected void ImgCreatoreAddressBook_Click(object sender, EventArgs e)
        {
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_OWNER_AUTHOR;
                HttpContext.Current.Session["AddressBook.from"] = "F_X_X_S";
                HttpContext.Current.Session["AddressBook.EnableOnly"] = this.rblOwnerType.SelectedValue;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AddressBook", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = string.Empty;
                if (caller.ID == "TxtCodeRole")
                {
                    codeAddressBook = this.TxtCodeRole.Text;
                }
                else
                {
                    if (caller.ID == "txtCodiceCreatore")
                    {
                        codeAddressBook = this.txtCodiceCreatore.Text;
                    }
                }

                if (!string.IsNullOrEmpty(codeAddressBook))
                {
                    this.SearchCorrespondent(codeAddressBook, caller.ID, true);
                }
                else
                {
                    if (caller.ID == "TxtCodeRole")
                    {
                        this.TxtCodeRole.Text = string.Empty;
                        this.TxtDescriptionRole.Text = string.Empty;
                        this.IdRole.Value = string.Empty;
                        this.select_sottoposto.Items.Clear();
                        this.select_sottoposto.Enabled = false;
                        this.UpPnlType.Update();
                    }
                    else
                    {
                        if (caller.ID == "txtCodiceCreatore")
                        {
                            this.txtCodiceCreatore.Text = string.Empty;
                            this.txtDescrizioneCreatore.Text = string.Empty;
                            this.idCreatore.Value = string.Empty;
                            this.upPnlCreatore.Update();
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

        protected void SearchCorrespondent(string addressCode, string idControl, bool endOfValidity)
        {

            DocsPaWR.Corrispondente corr = null;
            RubricaCallType calltype = this.GetCallType(idControl);
            corr = UIManager.AddressBookManager.getCorrispondenteByCodRubrica(addressCode, false);

            if (corr == null)
            {
                if (idControl == "TxtCodeRole")
                {
                    this.TxtCodeRole.Text = string.Empty;
                    this.TxtDescriptionRole.Text = string.Empty;
                    this.IdRole.Value = string.Empty;
                    this.select_sottoposto.Items.Clear();
                    this.select_sottoposto.Enabled = false;
                    this.UpPnlType.Update();
                }
                else
                {
                    this.txtCodiceCreatore.Text = string.Empty;
                    this.txtDescrizioneCreatore.Text = string.Empty;
                    this.idCreatore.Value = string.Empty;
                    this.upPnlCreatore.Update();
                }
                string msg = "ErrorTransmissionCorrespondentNotFound";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

            }
            else
            {
                if (idControl == "TxtCodeRole")
                {
                    if (!corr.tipoCorrispondente.Equals("R"))
                    {
                        this.TxtCodeRole.Text = string.Empty;
                        this.TxtDescriptionRole.Text = string.Empty;
                        this.IdRole.Value = string.Empty;
                        this.select_sottoposto.Items.Clear();
                        this.select_sottoposto.Enabled = false;
                        this.UpPnlType.Update();
                        string msg = "WarningCorrespondentAsRole";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                    }
                    else
                    {

                        this.Ds = UIManager.RoleManager.getUtentiInRuoloSottoposto(corr.systemId);
                        if (this.Ds == null || this.Ds.Tables.Count == 0)
                        {
                            this.TxtCodeRole.Text = string.Empty;
                            this.TxtDescriptionRole.Text = string.Empty;
                            this.IdRole.Value = string.Empty;
                            this.select_sottoposto.Items.Clear();
                            this.select_sottoposto.Enabled = false;
                            this.UpPnlType.Update();
                            string msg = "ErrorTransmissionCorrespondentNotFound";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            this.TxtCodeRole.Text = corr.codiceRubrica;
                            this.TxtDescriptionRole.Text = corr.descrizione;
                            this.IdRole.Value = corr.systemId;
                            this.select_sottoposto.Enabled = true;
                            this.Ds = RoleManager.getUtentiInRuoloSottoposto(corr.systemId);
                            this.ResetSelectSottoposti();
                            this.UpPnlType.Update();
                        }
                    }
                }
                else
                {
                    this.txtCodiceCreatore.Text = corr.codiceRubrica;
                    this.txtDescrizioneCreatore.Text = corr.descrizione;
                    this.idCreatore.Value = corr.systemId;
                    this.rblOwnerType.SelectedIndex = -1;
                    this.rblOwnerType.Items.FindByValue(corr.tipoCorrispondente).Selected = true;
                    this.upPnlCreatore.Update();
                }
            }

        }

        protected RubricaCallType GetCallType(string idControl)
        {
            RubricaCallType calltype;


            if (idControl == "TxtCodeRole")
            {
                calltype = RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO;
            }
            else
            {
                calltype = RubricaCallType.CALLTYPE_PROTO_INT_DEST;
            }

            return calltype;
        }

        protected void DocumentImgAddressBook_Click(object sender, EventArgs e)
        {
            try
            {
                HttpContext.Current.Session["AddressBook.from"] = "V_R_R_S";
                this.CallType = RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpPnlSender", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TransmLinkReceived_Click(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                this.SearchTransmissionExportExcel.Enabled = false;
                this.UpnlAzioniMassive.Update();
                this.SearchTransmissionRecipient.Text = Utils.Languages.GetLabelFromCode("SearchTransmissionRecipient", language);
                this.LiTransmLiReceived.Attributes.Remove("class");
                this.LiTransmLiReceived.Attributes.Add("class", "searchIAmSearch");
                this.LiTransmLiEffettuated.Attributes.Remove("class");
                this.LiTransmLiEffettuated.Attributes.Add("class", "searchOther");
                this.Role = UIManager.RoleManager.GetRoleInSession();
                this.TxtCodeRole.Text = this.Role.codiceRubrica;
                this.TxtDescriptionRole.Text = this.Role.descrizione;
                this.IdRole.Value = this.Role.systemId;
                this.Ds = UIManager.RoleManager.getUtentiInRuoloSottoposto(UIManager.RoleManager.GetRoleInSession().systemId);
                this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("LitSearchTransTypeDoc", language);
                this.SetTransmissionType("R");
                this.DdlRapidSearch.SelectedIndex = -1;
                this.UpPnlRapidSearch.Update();
                this.TypeSearch = "R";
                this.SearchTransmissionExportExcel.Visible = true;
                this.UpnlAzioniMassive.Update();
                this.LoadTypeDocuments();
                this.Template = null;
                this.PnlTypeDocument.Controls.Clear();
                this.UpTotalTypeDocument.Update();
                this.UpPnlTypeDocument.Update();
                this.UpSearchDocumentTabs.Update();
                this.UpPnlType.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ResetField()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.SearchTransmissionRecipient.Text = Utils.Languages.GetLabelFromCode("SearchTransmissionRecipient", language);
            this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("LitSearchTransTypeDoc", language);
            this.LiTransmLiReceived.Attributes.Remove("class");
            this.LiTransmLiReceived.Attributes.Add("class", "searchIAmSearch");
            this.LiTransmLiEffettuated.Attributes.Remove("class");
            this.LiTransmLiEffettuated.Attributes.Add("class", "searchOther");
            this.SetTransmissionType("R");
            this.UpPnlRapidSearch.Update();
            this.TypeSearch = "R";
            this.UpSearchDocumentTabs.Update();
            this.UpPnlType.Update();
        }

        protected void TransmLinkEffettuated_Click(object sender, EventArgs e)
        {
            try
            {
                this.SearchTransmissionExportExcel.Enabled = false;
                this.UpnlAzioniMassive.Update();
                string language = UIManager.UserManager.GetUserLanguage();
                this.SearchTransmissionRecipient.Text = Utils.Languages.GetLabelFromCode("SearchTransmissionSender", language);
                this.LiTransmLiReceived.Attributes.Remove("class");
                this.LiTransmLiReceived.Attributes.Add("class", "searchOther");
                this.LiTransmLiEffettuated.Attributes.Remove("class");
                this.LiTransmLiEffettuated.Attributes.Add("class", "searchIAmSearch");
                this.Role = UIManager.RoleManager.GetRoleInSession();
                this.TxtCodeRole.Text = this.Role.codiceRubrica;
                this.TxtDescriptionRole.Text = this.Role.descrizione;
                this.IdRole.Value = this.Role.systemId;
                this.Ds = UIManager.RoleManager.getUtentiInRuoloSottoposto(UIManager.RoleManager.GetRoleInSession().systemId);
                this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("LitSearchTransTypeDoc", language);
                this.SetTransmissionType("E");
                this.DdlRapidSearch.SelectedIndex = -1;
                this.UpPnlRapidSearch.Update();
                this.TypeSearch = "E";
                this.SearchTransmissionExportExcel.Visible = true;
                this.UpnlAzioniMassive.Update();
                this.LoadTypeDocuments();
                this.Template = null;
                this.PnlTypeDocument.Controls.Clear();
                this.UpTotalTypeDocument.Update();
                this.UpPnlTypeDocument.Update();
                this.UpSearchDocumentTabs.Update();
                this.UpPnlType.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadTypeDocuments()
        {
            if (this.ListaTipologiaAtto == null || this.ListaTipologiaAtto.Length == 0)
            {
                if (this.CustomDocuments)
                {
                    this.ListaTipologiaAtto = DocumentManager.getTipoAttoPDInsRic(this, UserManager.GetInfoUser().idAmministrazione, RoleManager.GetRoleInSession().idGruppo, "1");
                }
                else
                    this.ListaTipologiaAtto = DocumentManager.getListaTipologiaAtto(this);
            }

            this.DocumentDdlTypeDocument.Items.Clear();

            ListItem item = new ListItem(string.Empty, string.Empty);
            this.DocumentDdlTypeDocument.Items.Add(item);

            if (this.ListaTipologiaAtto != null)
            {
                for (int i = 0; i < this.ListaTipologiaAtto.Length; i++)
                {
                    item = new ListItem();
                    item.Text = this.ListaTipologiaAtto[i].descrizione;
                    item.Value = this.ListaTipologiaAtto[i].systemId;
                    this.DocumentDdlTypeDocument.Items.Add(item);
                }
            }
        }

        protected void DocumentDdlTypeDocument_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                {
                    if (this.CustomDocuments)
                    {
                        if (this.ddl_oggetto.SelectedIndex == 0)
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
                        else
                        {
                            this.Template = ProfilerProjectManager.getTemplateFascById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                            if (this.Template != null)
                            {
                                if (this.EnableStateDiagram)
                                {
                                    this.DocumentDdlStateDiagram.ClearSelection();

                                    //Verifico se esiste un diagramma di stato associato al tipo di documento
                                    //Modifica per la visualizzazione solo degli stati per cui esistono documenti in essi
                                    string idDiagramma = DiagrammiManager.getDiagrammaAssociatoFasc(this.DocumentDdlTypeDocument.SelectedValue).ToString();
                                    if (!string.IsNullOrEmpty(idDiagramma) && !idDiagramma.Equals("0"))
                                    {
                                        this.PnlStateDiagram.Visible = true;

                                        //Inizializzazione comboBox
                                        this.DocumentDdlStateDiagram.Items.Clear();
                                        ListItem itemEmpty = new ListItem();
                                        this.DocumentDdlStateDiagram.Items.Add(itemEmpty);

                                        DocsPaWR.Stato[] statiDg = DiagrammiManager.getStatiPerRicerca(idDiagramma, "F");
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
                }
                else
                {
                    this.Template = null;
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

        protected void swapOtherFiltersCheckboxs(Object sender, EventArgs e)
        {

            //Nessuna selezione
            if (!cbx_Acc.Checked && !cbx_Rif.Checked && !cbx_Pendenti.Checked)
            {
                this.PnlDateOthers.Visible = false;
                ddl_TAR.SelectedIndex = 0;
                ddl_TAR.Visible = false;
                this.dataUno_TAR.Visible = false;
                this.dataUno_TAR.Visible = false;
                this.dataUno_TAR.Text = "";
                this.dataDue_TAR.Visible = false;
                this.dataDue_TAR.Visible = false;
                this.dataDue_TAR.Text = "";
            }

            //Accettate
            if (cbx_Acc.Checked)
            {
                this.PnlDateOthers.Visible = true;
                ddl_TAR.Enabled = true;
                this.dataUno_TAR.Enabled = true;
                this.dataUno_TAR.Visible = true;
            }

            //Rifiutate
            if (cbx_Rif.Checked)
            {
                this.PnlDateOthers.Visible = true;
                ddl_TAR.Enabled = true;
                this.dataUno_TAR.Enabled = true;
                this.dataUno_TAR.Visible = true;
            }

            //Accettate-Rifiutate
            if (cbx_Acc.Checked && cbx_Rif.Checked)
            {
                this.PnlDateOthers.Visible = true;
                ddl_TAR.Enabled = true;
                this.dataUno_TAR.Enabled = true;
                this.dataUno_TAR.Visible = true;
            }

            //Pendenti
            if (cbx_Pendenti.Checked)
            {
                this.PnlDateOthers.Visible = false;
                cbx_Acc.Checked = false;
                cbx_Rif.Checked = false;
                ddl_TAR.SelectedIndex = 0;
                ddl_TAR.Visible = false;
                this.dataUno_TAR.Visible = false;
                this.dataUno_TAR.Visible = false;
                this.dataUno_TAR.Text = "";
                this.dataDue_TAR.Visible = false;
                this.dataDue_TAR.Visible = false;
                this.dataDue_TAR.Text = "";
            }

            this.UpPnlOtherFilters.Update();
        }

        protected void BtnUrgesAll_Click(object sender, EventArgs e)
        {
            this.sendSollecito(true);
        }

        protected void BtnUrgesSelected_Click(object sender, EventArgs e)
        {
            this.sendSollecito(false);
        }

        /// <summary>
        /// Invia un sollecito delle trasmissioni effettuate
        /// </summary>
        /// <param name="tutte">bool: se "true" = tutte le trasmissioni / se "false" = solo quelle selezionate</param>
        protected void sendSollecito(bool tutte)
        {
            bool result = true;

            try
            {
                if (tutte)
                {
                    int totalPageCount;
                    int totalRecordCount = 0;
                    DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPaWR.TrasmissioneOggettoTrasm();
                    Trasmissione[] trasms = TrasmManager.getQueryEffettuatePagingLiteWithTrasmUtente(oggettoTrasm, this.SearchFilters[0], this.SelectedPage, false, this.PageSize, out totalPageCount, out totalRecordCount);
                    for (int i = 0; i < trasms.Length; i++)
                    {

                        result = result && TrasmManager.sendSollecito(trasms[i]);
                    }
                }
                else
                {
                    if (this.ListCheck != null && this.ListCheck.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> pair in this.ListCheck)
                        {
                            string idTrasmissione = pair.Key;
                            DocsPaWR.Trasmissione trasm = TrasmManager.GetTransmission(this, idTrasmissione, this.ddl_oggetto.SelectedValue);
                            result = result && TrasmManager.sendSollecito(trasm);
                        }
                    }
                }

                if (result)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('InfoSearchTransmissionUrgesSuccess', 'check', '');", true);
                else
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchTransmissionUrgesSuccess', 'warning', '');", true);
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
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    string idProfile = this.Result[e.Row.DataItemIndex / 2].systemId;
                    string codeProject = idProfile;

                    CheckBox checkBox = e.Row.FindControl("cbSel") as CheckBox;
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

        private void SetCheckBox()
        {
            try
            {
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



        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }

        }

        private DocsPaWR.Registro Registry
        {
            get
            {
                DocsPaWR.Registro result = null;
                if (HttpContext.Current.Session["registry"] != null)
                {
                    result = HttpContext.Current.Session["registry"] as DocsPaWR.Registro;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["registry"] = value;
            }
        }

        private DocsPaWR.Ruolo Role
        {
            get
            {
                return UIManager.RoleManager.GetRoleInSession();
            }
            set
            {
                HttpContext.Current.Session["role"] = value;
            }
        }

        public FiltroRicerca[][] SearchFilters
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

        private string SchedaRicercaSessionKey
        {
            get
            {
                return string.Concat("RicercaTrasmissioni_", SearchManager.SESSION_KEY);
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
        /// Risultati restituiti dalla ricerca.
        /// </summary>
        public DocsPaWR.Trasmissione[] Result
        {
            get
            {
                return HttpContext.Current.Session["result"] as Trasmissione[];
            }
            set
            {
                HttpContext.Current.Session["result"] = value;
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

        private DocsPaWR.TipologiaAtto[] ListaTipologiaAtto
        {
            get
            {
                TipologiaAtto[] result = null;
                if (HttpContext.Current.Session["listaTipologiaAtto"] != null)
                {
                    result = HttpContext.Current.Session["listaTipologiaAtto"] as TipologiaAtto[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listaTipologiaAtto"] = value;
            }
        }

        private ArrayList ListaTipiFasc
        {
            get
            {
                ArrayList result = null;
                if (HttpContext.Current.Session["listaTipiFasc"] != null)
                {
                    result = HttpContext.Current.Session["listaTipiFasc"] as ArrayList;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listaTipiFasc"] = value;
            }
        }

        private bool ViewCodeTransmissionModels
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["viewCodeTransmissionModels"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["viewCodeTransmissionModels"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["viewCodeTransmissionModels"] = value;
            }
        }

        private string TypeSearch
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["typeSearch"] != null)
                {
                    result = HttpContext.Current.Session["typeSearch"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["typeSearch"] = value;
            }
        }

        protected int GetPageSize()
        {
            return this.PageSize;
        }

        private Dictionary<string, int> ListSecurityObject
        {
            get
            {
                Dictionary<string, int> result = new Dictionary<string, int>();
                if (HttpContext.Current.Session["listSecurityObject"] != null)
                {
                    result = HttpContext.Current.Session["listSecurityObject"] as Dictionary<string, int>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listSecurityObject"] = value;
            }
        }


        private RagioneTrasmissione[] ListaRagioni
        {
            get
            {
                RagioneTrasmissione[] result = null;
                if (HttpContext.Current.Session["listaRagioni"] != null)
                {
                    result = HttpContext.Current.Session["listaRagioni"] as RagioneTrasmissione[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listaRagioni"] = value;
            }
        }

        private List<int> Color
        {
            get
            {
                List<int> result = new List<int>();
                if (HttpContext.Current.Session["colorSearchTransmission"] != null)
                {
                    result = HttpContext.Current.Session["colorSearchTransmission"] as List<int>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["colorSearchTransmission"] = value;
            }
        }

        private DataSet Ds
        {
            get
            {
                DataSet result = new DataSet();
                if (HttpContext.Current.Session["ds"] != null)
                {
                    result = HttpContext.Current.Session["ds"] as DataSet;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ds"] = value;
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

        private bool PermitUrges
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["PermitUrges"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["PermitUrges"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["PermitUrges"] = value;
            }
        }

        private Trasmissione[] ListTransmissionNavigation
        {
            set
            {
                HttpContext.Current.Session["ListTransmissionNavigation"] = value;
            }
        }

    }
}
