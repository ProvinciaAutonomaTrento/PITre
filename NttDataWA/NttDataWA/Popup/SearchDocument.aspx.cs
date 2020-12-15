using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDatalLibrary;
using NttDataWA.Utils;
using System.Web.UI.HtmlControls;
using NttDataWA.UserControls;
using System.Collections;
using System.Data;
using System.Text;

namespace NttDataWA.Popup
{
    public partial class SearchDocument : System.Web.UI.Page
    {
        private const string TYPE_EXT = "esterni";
        private const string TYPE_PITRE = "SIMPLIFIEDINTEROPERABILITY";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //this.ListCheck = new List<string>();
                    this.LoadKeys();
                    this.InitializeLanguage();
                    this.InitializePage();
                    this.ShowGrid(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
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
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                            this.Result = null;
                            this.SelectedRow = string.Empty;
                            if (!string.IsNullOrEmpty(this.grid_pageindex.Value))
                            {
                                this.SelectedPage = int.Parse(this.grid_pageindex.Value);
                            }
                            this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                            this.BuildGridNavigator();
                            this.UpTypeResult.Update();
                            this.UpnlGrid.Update();
                            this.upPnlGridIndexes.Update();
                        }
                        else
                        {
                            this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                        }
                    }

                    this.ReadValueFromPopup();
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
                }

                //this.upPnlButtons.Update();
                //this.UplnRadioButton.Update();
                //this.UplnFiltri.Update();
                //this.UplnGrid.Update();
                this.ReApplyScripts();
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
            int valoreDiDefault = -1;
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
                    if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    {
                        valoreDiDefault = i;
                    }

                    if (maxLenght < valoreOggetto.VALORE.Length)
                    {
                        maxLenght = valoreOggetto.VALORE.Length;
                    }
             //   }
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
            corrispondente.PageCaller = "Popup";

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
                    if (ddl.Items.Count == 1)
                        ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                    else
                        ddl.Items.Insert(0, new ListItem(""));

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
                                ((UserControls.CorrespondentCustom)campo).CODICE_READ_ONLY = true;
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




        private void ReadValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.Object.ReturnValue))
            {
                this.TxtObject.Text = this.ReturnValue.Split('#').First();
                if (this.ReturnValue.Split('#').Length > 1)
                    this.TxtCodeObject.Text = this.ReturnValue.Split('#').Last();
                this.UpdPnlObject.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('Object','');", true);
            }

            if (!string.IsNullOrEmpty(this.OpenTitolario.ReturnValue))
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.txt_CodFascicolo.Text = this.ReturnValue.Split('#').First();
                    this.txt_DescFascicolo.Text = this.ReturnValue.Split('#').Last();
                    this.UpCodFasc.Update();
                    txt_CodFascicolo_OnTextChanged(new object(), new EventArgs());
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenTitolario','')", true);
            }

            if (!string.IsNullOrEmpty(this.SearchProject.ReturnValue))
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.txt_CodFascicolo.Text = this.ReturnValue.Split('#').First();
                    this.txt_DescFascicolo.Text = this.ReturnValue.Split('#').Last();
                    this.UpCodFasc.Update();
                    this.txt_CodFascicolo_OnTextChanged(new object(), new EventArgs());
                }
                else if (this.ReturnValue.Contains("//"))
                {
                    this.txt_CodFascicolo.Text = string.Empty;
                    this.txt_DescFascicolo.Text = string.Empty;
                    this.UpCodFasc.Update();
                    this.txt_CodFascicolo_OnTextChanged(new object(), new EventArgs());
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchProject','');", true);
            }
        }

        private void InitializePage()
        {
            this.InitializeObjectValue();
            this.CheckAll = false;
            this.Result = null;
            this.ListCheck = new Dictionary<string, string>();
            this.Registry = RoleManager.GetRoleInSession().registri[0];
            RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
            this.PopulateDDLRegistry();
            this.resetField();
            this.cbl_archDoc_E.Attributes.Add("onclick", "enableField();");
            this.txt_initNumProt_E.ReadOnly = false;
            this.txt_fineNumProt_E.Visible = false;
            this.LtlANumProto.Visible = false;
            this.LtlDaNumProto.Visible = false;
            this.txt_fineNumProt_E.Text = string.Empty;
            this.txt_initDataProt_E.ReadOnly = false;
            this.txt_fineDataProt_E.Visible = false;
            this.LtlADataProto.Visible = false;
            this.txt_fineDataProt_E.Text = string.Empty;
            this.txt_initIdDoc_C.ReadOnly = false;
            this.txt_fineIdDoc_C.Visible = false;
            this.LtlAIdDoc.Visible = false;
            this.LtlDaIdDoc.Visible = false;
            this.Labels = DocumentManager.GetLettereProtocolli();
            this.CellPosition = new Dictionary<string, int>();
            this.IdProfileList = null;
            this.CheckAll = false;
            this.txt_initDataCreazione_E.ReadOnly = false;
            this.txt_finedataCreazione_E.Visible = false;
            this.LtlADataCreazione.Visible = false;

            if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FILTRO_ALLEGATI_ESTERNI.ToString())) ||
               !Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FILTRO_ALLEGATI_ESTERNI.ToString()).Equals("1"))
            {
                ListItem itemExternal = (from item in rblFiltriAllegati.Items.Cast<ListItem>() where item.Value.Equals(TYPE_EXT) select item).FirstOrDefault();
                rblFiltriAllegati.Items.Remove(itemExternal);
            }
            if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.INTEROP_SERVICE_ACTIVE.ToString())) ||
                !Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.INTEROP_SERVICE_ACTIVE.ToString()).Equals("1"))
            {
                ListItem itemPitre = (from item in rblFiltriAllegati.Items.Cast<ListItem>() where item.Value.Equals(TYPE_PITRE) select item).FirstOrDefault();
                rblFiltriAllegati.Items.Remove(itemPitre);
            }

            this.Result = null;
            this.SelectedRow = string.Empty;
            this.SearchFilters = null;
            this.Template = null;
            this.RecordCount = 0;
            this.PageCount = 0;
            this.SelectedPage = 1;

            this.Labels = DocumentManager.GetLettereProtocolli();
            this.CellPosition = new Dictionary<string, int>();
            this.IdProfileList = null;
            this.CodeProfileList = null;
            this.CheckAll = false;

            this.upPnlButtons.Update();
            this.SelectedPage = 1;
            this.TxtObject.MaxLength = MaxLenghtObject;
            //this.PlcJavascriptOtherFilters.Visible = true;
            //this.OlcOtherFilters.Visible = true;

            if (this.EnableAjaxAddressBook)
            {
                this.SetAjaxAddressBook();
                this.SetAjaxDescriptionProject();
            }

            GridManager.SelectedGrid = GridManager.GetStandardGridForUser(GridTypeEnumeration.Document, UIManager.UserManager.GetInfoUser());

            this.LoadTypeDocuments();

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

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_NEW_RUBRICA_VELOCE.ToString())) && (Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_NEW_RUBRICA_VELOCE.ToString())).Equals("1"))
            {
                this.EnableAjaxAddressBook = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString())) && !Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString()).Equals("1"))
            {
                this.opInt.Attributes.CssStyle.Add("display", "none");
                this.opInt.Selected = false;
            }

            if (UIManager.DocumentManager.IsEnabledProfilazioneAllegati())
            {
                this.IsEnabledProfilazioneAllegato = true;
            }

        }

        protected void ddl_dataProt_E_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.txt_initDataProt_E.Text = string.Empty;
                this.txt_fineDataProt_E.Text = string.Empty;

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dataProt_E.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initDataProt_E.ReadOnly = false;
                        this.txt_fineDataProt_E.Visible = false;
                        this.LtlADataProto.Visible = false;
                        this.LtlDaDataProto.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.txt_initDataProt_E.ReadOnly = false;
                        this.txt_fineDataProt_E.ReadOnly = false;
                        this.LtlADataProto.Visible = true;
                        this.LtlDaDataProto.Visible = true;
                        this.txt_fineDataProt_E.Visible = true;
                        this.LtlDaDataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataProto.Visible = false;
                        this.txt_fineDataProt_E.Visible = false;
                        this.txt_initDataProt_E.ReadOnly = true;
                        this.txt_initDataProt_E.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataProto.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDataProt_E.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataProto.Visible = true;
                        this.txt_fineDataProt_E.Visible = true;
                        this.txt_initDataProt_E.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_fineDataProt_E.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_fineDataProt_E.ReadOnly = true;
                        this.txt_initDataProt_E.ReadOnly = true;
                        this.LtlDaDataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataProto.Visible = true;
                        this.txt_fineDataProt_E.Visible = true;
                        this.txt_initDataProt_E.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_fineDataProt_E.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_fineDataProt_E.ReadOnly = true;
                        this.txt_initDataProt_E.ReadOnly = true;
                        this.LtlDaDataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_numProt_E_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.txt_initNumProt_E.Text = string.Empty;
                this.txt_initNumProt_E.Text = string.Empty;

                switch (this.ddl_numProt_E.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initNumProt_E.ReadOnly = false;
                        this.txt_fineNumProt_E.Visible = false;
                        this.LtlANumProto.Visible = false;
                        this.LtlDaNumProto.Visible = false;
                        this.txt_fineNumProt_E.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initNumProt_E.ReadOnly = false;
                        this.txt_fineNumProt_E.ReadOnly = false;
                        this.LtlANumProto.Visible = true;
                        this.LtlDaNumProto.Visible = true;
                        this.txt_fineNumProt_E.Visible = true;
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DdlRegistries_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            ListItem itemSelect = (sender as DropDownList).SelectedItem;
            Registro RegSel = (from reg in RoleManager.GetRoleInSession().registri
                               where reg.systemId.Equals(itemSelect.Value) &&
                                   reg.codRegistro.Equals(itemSelect.Text.Trim())
                               select reg).FirstOrDefault();
            UIManager.RegistryManager.SetRegistryInSession(RegSel);
            this.Registry = RegSel;
            if (this.EnableAjaxAddressBook)
            {
                this.SetAjaxAddressBook();
                this.SetAjaxDescriptionProject();
            }
            this.upPnlCreatore.Update();
            this.upPnlProprietario.Update();
            this.UpPnlSenderRecipient.Update();
            this.UpCodFasc.Update();
        }

        private void SetAjaxAddressBook()
        {
            string dataUser = RoleManager.GetRoleInSession().systemId;
            dataUser = dataUser + "-" + this.Registry.systemId;

            string callType = "CALLTYPE_OWNER_AUTHOR";
            this.RapidCreatore.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
            this.RapidProprietario.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

            callType = "CALLTYPE_CORR_INT_EST_CON_DISABILITATI";
            this.RapidRecipient.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.SearchDocumentBtnSearch.Text = Utils.Languages.GetLabelFromCode("ClassificationSchemaSearch", language);
            this.SearchDocumentProjectBtnInsert.Text = Utils.Languages.GetLabelFromCode("AddDocInProjectBtnInserisci", language);
            this.SearchDocumentClose.Text = Utils.Languages.GetLabelFromCode("AddDocInProjectBtnClose", language);
            this.LitPopupSearchDocumentFound.Text = Utils.Languages.GetLabelFromCode("LitPopupSearchProjectFound", language);
            this.LitPopupSearchDocumentFound2.Text = Utils.Languages.GetLabelFromCode("LitPopupSearchProjectFound3", language);
            this.LtlDataProto.Text = Utils.Languages.GetLabelFromCode("LtlDataProto", language);
            this.projectLitVisibleObjectChars.Text = Utils.Languages.GetLabelFromCode("DocumentLitObjectChAv", language);
            this.DocumentImgObjectary.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgObjectary", language);
            this.DocumentImgObjectary.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgObjectary", language);
            this.Object.Title = Utils.Languages.GetLabelFromCode("TitleObjectPopup", language);
            this.SearchDocumentLit.Text = Utils.Languages.GetLabelFromCode("SearchDocumentLit", language);
            this.ddl_numProt_E.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E0", language);
            this.ddl_numProt_E.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E1", language);
            this.LtlIdDoc.Text = Utils.Languages.GetLabelFromCode("LtlIdDoc", language);
            this.ddl_dataProt_E.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_dataProt_E.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_dataProt_E.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_dataProt_E.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_dataProt_E.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.LtlNumProto.Text = Utils.Languages.GetLabelFromCode("LtlNumProto", language);
            this.LitPopupSearchProjectFilters.Text = Utils.Languages.GetLabelFromCode("LitPopupSearchProjectFilters", language);

            this.optUO.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUO", language);
            this.optPropUO.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUO", language);
            this.optRole.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptRole", language);
            this.optPropRole.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptRole", language);
            this.optUser.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUser", language);
            this.optPropUser.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUser", language);
            this.chkCreatoreExtendHistoricized.Text = Utils.Languages.GetLabelFromCode("SearchProjectCreatoreExtendHistoricized", language);
            this.SearchProtocolloLit.Text = Utils.Languages.GetLabelFromCode("SearchProtocolloLit", language);
            this.litCreator.Text = Utils.Languages.GetLabelFromCode("LtlCreatore", language);
            this.litOwner.Text = Utils.Languages.GetLabelFromCode("LtlProprietario", language);
            this.ImgProprietarioAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgRecipientAddressBookAuthor", language);
            this.ImgCreatoreAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgRecipientAddressBookOwner", language);
            this.DocumentImgRecipientAddressBookMittDest.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgRecipientAddressBookMittDest", language);
            this.LtlMitDest.Text = Utils.Languages.GetLabelFromCode("LtlMitDest", language);
            this.chk_mitt_dest_storicizzati.Text = Utils.Languages.GetLabelFromCode("SearchProjectCreatoreExtendHistoricized", language);
            this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("SearchDocumentLitTypology", language);
            this.DocumentDdlTypeDocument.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlTypeDocument", language));
            this.opt_StateConditionEquals.Text = Utils.Languages.GetLabelFromCode("SearchProjectStateConditionEquals", language);
            this.opt_StateConditionUnequals.Text = Utils.Languages.GetLabelFromCode("SearchProjectStateConditionUnequals", language);
            this.DocumentDdlStateDiagram.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlStateDiagram", language));


            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
            this.btnclassificationschema.AlternateText = Utils.Languages.GetLabelFromCode("btnclassificationschema", language);
            this.btnclassificationschema.ToolTip = Utils.Languages.GetLabelFromCode("btnclassificationschema", language);
            this.SearchProjectImg.ToolTip = Utils.Languages.GetLabelFromCode("SearchProjectImg", language);
            this.SearchProjectImg.AlternateText = Utils.Languages.GetLabelFromCode("SearchProjectImg", language);
            this.LtlCodFascGenProc.Text = Utils.Languages.GetLabelFromCode("LtlCodFascGenProc", language);
            this.cb_Conservato.Text = Utils.Languages.GetLabelFromCode("cb_Conservato", language);
            this.cb_NonConservato.Text = Utils.Languages.GetLabelFromCode("cb_NonConservato", language);
            this.SearchDocumentTypeDocument.Text = Utils.Languages.GetLabelFromCode("SearchDocumentTypeDocument", language);

            this.opPredisposed.Text = Utils.Languages.GetLabelFromCode("opOredisposed", language);
            this.opPrints.Text = Utils.Languages.GetLabelFromCode("opPrints", language);

            if ((DocumentManager.GetDescriptionLabel("A")).Length > 3)
            {
                this.opArr.Text = ((DocumentManager.GetDescriptionLabel("A")).Substring(0, 3)) + "."; //Valore A
            }
            else
            {
                this.opArr.Text = DocumentManager.GetDescriptionLabel("A");
            }

            if ((DocumentManager.GetDescriptionLabel("P")).Length > 3)
            {
                //CASO PER INFORMATICA TRENTINA PER LASCIARE 4 CARATTERI (Part.)
                if (DocumentManager.GetDescriptionLabel("P").Equals("Partenza"))
                {
                    this.opPart.Text = "Part.";
                }
                else
                {
                    this.opPart.Text = ((DocumentManager.GetDescriptionLabel("P")).Substring(0, 3)) + "."; //Valore P
                }
            }
            else
            {
                this.opPart.Text = DocumentManager.GetDescriptionLabel("P");
            }

            if (DocumentManager.GetDescriptionLabel("I").Length > 3)
            {
                this.opInt.Text = ((DocumentManager.GetDescriptionLabel("I")).Substring(0, 3)) + ".";//Valore I
            }
            else
            {
                this.opInt.Text = DocumentManager.GetDescriptionLabel("I");
            }
            if (DocumentManager.GetDescriptionLabel("G").Length > 3)
            {
                this.opGrigio.Text = (DocumentManager.GetDescriptionLabel("G").Substring(0, 3)) + ".";//Valore G
            }
            else
            {
                this.opGrigio.Text = DocumentManager.GetDescriptionLabel("G");
            }
            if (DocumentManager.GetDescriptionLabel("ALL").Length > 3)
            {
                this.opAll.Text = (DocumentManager.GetDescriptionLabel("ALL").Substring(0, 3)) + ".";//Valore ALL
            }
            else
            {
                this.opAll.Text = DocumentManager.GetDescriptionLabel("ALL");
            }

            this.LtlDaIdDoc.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
            this.LtlDaNumProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);

            this.LtlAIdDoc.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlANumProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlADataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlDataCreazione.Text = Utils.Languages.GetLabelFromCode("LtlDataCreazione", language);
            this.ddl_dataCreazione_E.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_dataCreazione_E.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_dataCreazione_E.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_dataCreazione_E.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_dataCreazione_E.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.ddl_idDocumento_C.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_idDocumento_C0", language);
            this.ddl_idDocumento_C.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_idDocumento_C1", language);
            this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.OpenTitolario.Title = Utils.Languages.GetLabelFromCode("TitleClassificationScheme", language);
            this.SearchProject.Title = Utils.Languages.GetLabelFromCode("SearchProjectTitleSearch", language);
            this.rbOpIS.Text = SimplifiedInteroperabilityManager.SearchItemDescriprion;
        }

        protected void ddl_idDocumento_C_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (this.ddl_idDocumento_C.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initIdDoc_C.ReadOnly = false;
                        this.txt_fineIdDoc_C.Visible = false;
                        this.LtlAIdDoc.Visible = false;
                        this.LtlDaIdDoc.Visible = false;
                        this.txt_fineIdDoc_C.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initIdDoc_C.ReadOnly = false;
                        this.txt_fineIdDoc_C.ReadOnly = false;
                        this.LtlAIdDoc.Visible = true;
                        this.LtlDaIdDoc.Visible = true;
                        this.txt_fineIdDoc_C.Visible = true;
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
                this.txt_initDataCreazione_E.Text = string.Empty;
                this.txt_finedataCreazione_E.Text = string.Empty;

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dataCreazione_E.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initDataCreazione_E.ReadOnly = false;
                        this.txt_finedataCreazione_E.Visible = false;
                        this.LtlADataCreazione.Visible = false;
                        this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.txt_initDataCreazione_E.ReadOnly = false;
                        this.txt_finedataCreazione_E.ReadOnly = false;
                        this.LtlADataCreazione.Visible = true;
                        this.LtlDaDataCreazione.Visible = true;
                        this.txt_finedataCreazione_E.Visible = true;
                        this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataCreazione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataCreazione.Visible = false;
                        this.txt_finedataCreazione_E.Visible = false;
                        this.txt_initDataCreazione_E.ReadOnly = true;
                        this.txt_initDataCreazione_E.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataCreazione.Visible = true;
                        this.txt_finedataCreazione_E.Visible = true;
                        this.txt_initDataCreazione_E.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_finedataCreazione_E.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_finedataCreazione_E.ReadOnly = true;
                        this.txt_initDataCreazione_E.ReadOnly = true;
                        this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataCreazione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataCreazione.Visible = true;
                        this.txt_finedataCreazione_E.Visible = true;
                        this.txt_initDataCreazione_E.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_finedataCreazione_E.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_finedataCreazione_E.ReadOnly = true;
                        this.txt_initDataCreazione_E.ReadOnly = true;
                        this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataCreazione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchDocumentProjectBtnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                if (this.ListCheck != null && this.ListCheck.Count > 0)
                {
                    this.ListDocs = new List<SchedaDocumento>();
                    SchedaDocumento doc = new SchedaDocumento();
                    foreach(string idProfile in this.ListCheck.Keys)
                    {
                        doc = DocumentManager.getDocumentListVersions(this.Page, idProfile, idProfile);
                        this.ListDocs.Add(doc);
                    }

                    ScriptManager.RegisterClientScriptBlock(this.upPnlButtons, this.upPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('SearchDocument','up');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningSelectDocumentForInstance', 'warning', '');", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            try
            {
                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = caller.Text;

                if (!string.IsNullOrEmpty(codeAddressBook))
                {
                    this.SearchCorrespondent(codeAddressBook, caller.ID);
                }
                else
                {
                    switch (caller.ID)
                    {
                        case "txtCodiceCreatore":
                            this.txtCodiceCreatore.Text = string.Empty;
                            this.txtDescrizioneCreatore.Text = string.Empty;
                            this.idCreatore.Value = string.Empty;
                            this.upPnlCreatore.Update();
                            break;
                        case "txtCodiceProprietario":
                            this.txtCodiceProprietario.Text = string.Empty;
                            this.txtDescrizioneProprietario.Text = string.Empty;
                            this.idProprietario.Value = string.Empty;
                            this.upPnlProprietario.Update();
                            break;
                        case "txt_codMit_E":
                            this.txt_codMit_E.Text = string.Empty;
                            this.txt_descrMit_E.Text = string.Empty;
                            this.IdRecipient.Value = string.Empty;
                            this.UpPnlSenderRecipient.Update();
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

        protected void SearchCorrespondent(string addressCode, string idControl)
        {
            RubricaCallType calltype = GetCallType(idControl);
            Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteRubrica(addressCode, calltype);

            if (corr == null)
            {
                switch (idControl)
                {
                    case "txtCodiceCreatore":
                        this.txtCodiceCreatore.Text = string.Empty;
                        this.txtDescrizioneCreatore.Text = string.Empty;
                        this.idCreatore.Value = string.Empty;
                        this.upPnlCreatore.Update();
                        break;
                    case "txtCodiceProprietario":
                        this.txtCodiceProprietario.Text = string.Empty;
                        this.txtDescrizioneProprietario.Text = string.Empty;
                        this.idProprietario.Value = string.Empty;
                        this.upPnlProprietario.Update();
                        break;
                    //case "txt_codMit_E":
                    //    this.txt_codMit_E.Text = string.Empty;
                    //    this.txt_descrMit_E.Text = string.Empty;
                    //    this.IdRecipient.Value = string.Empty;
                    //    this.RecipientTypeOfCorrespondent.Value = string.Empty;
                    //    this.UpProtocollo.Update();
                    //    break;
                }

                string msg = "ErrorTransmissionCorrespondentNotFound";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
            }
            else
            {
                switch (idControl)
                {
                    case "txtCodiceCreatore":
                        this.txtCodiceCreatore.Text = corr.codiceRubrica;
                        this.txtDescrizioneCreatore.Text = corr.descrizione;
                        this.idCreatore.Value = corr.systemId;
                        this.rblOwnerType.SelectedIndex = -1;
                        this.rblOwnerType.Items.FindByValue(corr.tipoCorrispondente).Selected = true;
                        this.upPnlCreatore.Update();
                        break;
                    case "txtCodiceProprietario":
                        this.txtCodiceProprietario.Text = corr.codiceRubrica;
                        this.txtDescrizioneProprietario.Text = corr.descrizione;
                        this.idProprietario.Value = corr.systemId;
                        this.rblProprietarioType.SelectedIndex = -1;
                        this.rblProprietarioType.Items.FindByValue(corr.tipoCorrispondente).Selected = true;
                        this.upPnlProprietario.Update();
                        break;
                    //case "txt_codMit_E":
                    //    this.txt_codMit_E.Text = corr.codiceRubrica;
                    //    this.txt_descrMit_E.Text = corr.descrizione;
                    //    this.IdRecipient.Value = corr.systemId;
                    //    this.UpPnlSenderRecipient.Update();
                    //    break;
                }
            }

        }

        protected RubricaCallType GetCallType(string idControl)
        {
            RubricaCallType calltype = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
            if (idControl == "txt_codMit_E")
            {
                if (this.chk_mitt_dest_storicizzati.Checked)
                {
                    calltype = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                }
                else
                {
                    calltype = RubricaCallType.CALLTYPE_CORR_INT_EST;
                }
            }

            if (idControl == "txtCodiceCreatore")
            {
                calltype = RubricaCallType.CALLTYPE_OWNER_AUTHOR;
            }

            return calltype;
        }

        protected void chk_mitt_dest_storicizzati_Clik(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            string dataUser = RoleManager.GetRoleInSession().systemId;
            dataUser = dataUser + "-" + RoleManager.GetRoleInSession().registri[0].systemId;
            string callType = string.Empty;

            if (this.chk_mitt_dest_storicizzati.Checked)
            {
                callType = "CALLTYPE_CORR_INT_EST_CON_DISABILITATI";
            }
            else
            {
                callType = "CALLTYPE_CORR_INT_EST";
            }
            this.RapidRecipient.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

            this.UpPnlSenderRecipient.Update();
        }

        protected void chkCreatoreExtendHistoricized_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
        }


        protected void DocumentImgRecipientAddressBookMittDest_Click(object sender, ImageClickEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            try
            {
                //this.CallType = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
                if (this.chk_mitt_dest_storicizzati.Checked)
                {
                    this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                }
                else
                {
                    this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST;
                }
                //this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST;
                HttpContext.Current.Session["AddressBook.from"] = "T_S_R_S";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpProtocollo", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void txt_codMit_E_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = this.txt_codMit_E.Text;

                if (!string.IsNullOrEmpty(codeAddressBook))
                {
                    this.SearchCorrespondent(codeAddressBook, caller.ID);
                }
                else
                {
                    this.txt_codMit_E.Text = string.Empty;
                    this.txt_descrMit_E.Text = string.Empty;
                    this.IdRecipient.Value = string.Empty;
                    this.UpPnlSenderRecipient.Update();
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
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            try
            {
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

        protected void ImgCreatoreAddressBook_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
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

        protected void ImgProprietarioAddressBook_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_OWNER_AUTHOR;
                HttpContext.Current.Session["AddressBook.from"] = "F_X_X_S_2";
                HttpContext.Current.Session["AddressBook.EnableOnly"] = this.rblProprietarioType.SelectedValue;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AddressBook", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchDocumentBtnSearch_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            // reset check list for massive ops
            this.ListCheck = new Dictionary<string, string>();
            this.SelectedPage = 1;
            this.HiddenItemsChecked.Value = string.Empty;
            this.HiddenItemsUnchecked.Value = string.Empty;
            this.HiddenItemsAll.Value = string.Empty;
            this.upPnlButtons.Update();

            try
            {
                bool result = this.SearchDocumentFilters();
                if (result)
                {
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                }
                this.UpTypeResult.Update();
                this.UpnlGrid.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

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
            result = this.SearchDocument2(searchFilters, selectedPage, out recordNumber, out outOfMaxRowSearchable);

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

            // Se ci sono risultati, vengono visualizzati
            if (this.Result != null && this.Result.Length > 0)
            {
                this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
            }
            else
            {
                this.ShowGrid(selectedGrid, null, 0, 0, labels);
                this.BuildGridNavigator();
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

        protected void gridViewResult_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                Field d = new Field();
                string sortExpression = e.SortExpression.ToString();

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
                    this.UpTypeResult.Update();
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

        protected void GridView_RowCommand(Object sender, GridViewCommandEventArgs e)
        {

            // If multiple ButtonField column fields are used, use the
            // CommandName property to determine which button was clicked.
            if (e.CommandName == "viewDetails")
            {


                //int rowIndex = Convert.ToInt32(e.CommandArgument);
                //string idProfile = GrigliaResult.Rows[rowIndex]["IdProfile"].ToString();
                //SchedaDocumento schedaDocumento = UIManager.DocumentManager.getDocumentDetails(this, idProfile, idProfile);
                //InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                //string language = UIManager.UserManager.GetUserLanguage();

                //if (!string.IsNullOrEmpty(this.SelectedRow))
                //{
                //    if (rowIndex != Int32.Parse(this.SelectedRow))
                //    {
                //        this.SelectedRow = string.Empty;
                //    }
                //}

                //HttpContext.Current.Session["isZoom"] = null;
                //List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                //Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                //actualPage.IdObject = schedaDocumento.systemId;
                //actualPage.OriginalObjectId = schedaDocumento.systemId;
                //actualPage.NumPage = this.SelectedPage.ToString();
                //actualPage.SearchFilters = this.SearchFilters;
                //actualPage.PageSize = this.PageSize.ToString();

                //actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED.ToString(), string.Empty);
                //actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED.ToString(), true);
                //actualPage.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED.ToString();

                //actualPage.Page = "SEARCHDOCUMENTADVANCED.ASPX";
                //actualPage.DxTotalNumberElement = this.RecordCount.ToString();
                //actualPage.ViewResult = true;

                //if (this.PageCount == 0)
                //{
                //    actualPage.DxTotalPageNumber = "1";
                //}
                //else
                //{
                //    actualPage.DxTotalPageNumber = this.PageCount.ToString();
                //}

                //int indexElement = ((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1);
                //actualPage.DxPositionElement = indexElement.ToString();

                //navigationList.Add(actualPage);
                //Navigation.NavigationUtils.SetNavigationList(navigationList);

                //UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                //Response.Redirect("~/Document/Document.aspx");



            }

        }

        /// <summary>
        /// Number of result in page
        /// </summary>
        public int PageSize
        {
            get
            {
                int result = 10;
                //if (HttpContext.Current.Session["pageSizeDocument"] != null)
                //{
                //    result = int.Parse(HttpContext.Current.Session["pageSizeDocument"].ToString());
                //}
                return result;
            }
            //set
            //{
            //    HttpContext.Current.Session["pageSizeDocument"] = value;
            //}
        }

        protected void gridViewResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                //if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                //{
                //    string idProfile = GrigliaResult.Rows[e.Row.DataItemIndex]["IdProfile"].ToString();

                //    string labelConservazione = "ProjectIconTemplateRemoveConservazione";
                //    string labelAdl = "ProjectIconTemplateRemoveAdl";
                //    string labelAdlRole = "ProjectIconTemplateRemoveAdlRole";
                //    //imagini delle icone
                //    if (bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsInStorageArea"].ToString()))
                //    {
                //        ((CustomImageButton)e.Row.FindControl("conservazione")).ImageUrl = "../Images/Icons/remove_preservation_grid.png";
                //        ((CustomImageButton)e.Row.FindControl("conservazione")).OnMouseOutImage = "../Images/Icons/remove_preservation_grid.png";
                //        ((CustomImageButton)e.Row.FindControl("conservazione")).OnMouseOverImage = "../Images/Icons/remove_preservation_grid_hover.png";
                //        ((CustomImageButton)e.Row.FindControl("conservazione")).ImageUrlDisabled = "../Images/Icons/remove_preservation_grid_disabled.png";
                //    }
                //    else
                //    {
                //        labelConservazione = "ProjectIconTemplateConservazione";
                //        ((CustomImageButton)e.Row.FindControl("conservazione")).ImageUrl = "../Images/Icons/add_preservation_grid.png";
                //        ((CustomImageButton)e.Row.FindControl("conservazione")).OnMouseOutImage = "../Images/Icons/add_preservation_grid.png";
                //        ((CustomImageButton)e.Row.FindControl("conservazione")).OnMouseOverImage = "../Images/Icons/add_preservation_grid_hover.png";
                //        ((CustomImageButton)e.Row.FindControl("conservazione")).ImageUrlDisabled = "../Images/Icons/padd_preservation_grid_disabled.png";

                //    }

                //    labelAdlRole = "ProjectIconTemplateRemoveAdlRole";
                //    if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
                //    {
                //        if (bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsInWorkingAreaRole"].ToString()))
                //        {
                //            ((CustomImageButton)e.Row.FindControl("adl")).Enabled = false;
                //            ((CustomImageButton)e.Row.FindControl("adlrole")).ImageUrl = "../Images/Icons/adl1x.png";
                //            ((CustomImageButton)e.Row.FindControl("adlrole")).OnMouseOutImage = "../Images/Icons/adl1x.png";
                //            ((CustomImageButton)e.Row.FindControl("adlrole")).OnMouseOverImage = "../Images/Icons/adl1x_hover.png";
                //            ((CustomImageButton)e.Row.FindControl("adlrole")).ImageUrlDisabled = "../Images/Icons/adl1x_disabled.png";
                //        }
                //        else
                //        {
                //            if (bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsInWorkingArea"].ToString()))
                //                labelAdlRole = "ProjectIconTemplateAdlRole";
                //            else
                //                labelAdlRole = "ProjectIconTemplateAdlRoleInsert";
                //            ((CustomImageButton)e.Row.FindControl("adl")).Enabled = true;
                //            ((CustomImageButton)e.Row.FindControl("adlrole")).ImageUrl = "../Images/Icons/adl1.png";
                //            ((CustomImageButton)e.Row.FindControl("adlrole")).OnMouseOutImage = "../Images/Icons/adl1.png";
                //            ((CustomImageButton)e.Row.FindControl("adlrole")).OnMouseOverImage = "../Images/Icons/adl1_hover.png";
                //            ((CustomImageButton)e.Row.FindControl("adlrole")).ImageUrlDisabled = "../Images/Icons/adl1_disabled.png";
                //        }
                //    }

                //    if (bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsInWorkingArea"].ToString()))
                //    {
                //        ((CustomImageButton)e.Row.FindControl("adl")).ImageUrl = "../Images/Icons/adl2x.png";
                //        ((CustomImageButton)e.Row.FindControl("adl")).OnMouseOutImage = "../Images/Icons/adl2x.png";
                //        ((CustomImageButton)e.Row.FindControl("adl")).OnMouseOverImage = "../Images/Icons/adl2x_hover.png";
                //        ((CustomImageButton)e.Row.FindControl("adl")).ImageUrlDisabled = "../Images/Icons/adl2x_disabled.png";
                //    }
                //    else
                //    {
                //        labelAdl = "ProjectIconTemplateAdl";
                //        ((CustomImageButton)e.Row.FindControl("adl")).ImageUrl = "../Images/Icons/adl2.png";
                //        ((CustomImageButton)e.Row.FindControl("adl")).OnMouseOutImage = "../Images/Icons/adl2.png";
                //        ((CustomImageButton)e.Row.FindControl("adl")).OnMouseOverImage = "../Images/Icons/adl2_hover.png";
                //        ((CustomImageButton)e.Row.FindControl("adl")).ImageUrlDisabled = "../Images/Icons/adl2_disabled.png";
                //    }

                //    string estensione = GrigliaResult.Rows[e.Row.RowIndex]["FileExtension"].ToString();
                //    if (!string.IsNullOrEmpty(estensione))
                //    {
                //        string imgUrl = ResolveUrl(FileManager.getFileIcon(this, estensione));
                //        ((CustomImageButton)e.Row.FindControl("estensionedoc")).Visible = true;
                //        ((CustomImageButton)e.Row.FindControl("estensionedoc")).ImageUrl = imgUrl;
                //        ((CustomImageButton)e.Row.FindControl("estensionedoc")).OnMouseOutImage = imgUrl;
                //        ((CustomImageButton)e.Row.FindControl("estensionedoc")).OnMouseOverImage = imgUrl;
                //    }
                //    else
                //        ((CustomImageButton)e.Row.FindControl("estensionedoc")).Visible = false;

                //    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).Enabled = true;
                //    ((CustomImageButton)e.Row.FindControl("conservazione")).Visible = this.AllowConservazione;
                //    ((CustomImageButton)e.Row.FindControl("adl")).Visible = this.AllowADL;
                //    ((CustomImageButton)e.Row.FindControl("adlrole")).Visible = this.AllowADLRole;
                //    ((CustomImageButton)e.Row.FindControl("firmato")).Visible = bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsSigned"].ToString());

                //    //evento click
                //    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).Click += new ImageClickEventHandler(ImageButton_Click);
                //    ((CustomImageButton)e.Row.FindControl("conservazione")).Click += new ImageClickEventHandler(ImageButton_Click);
                //    ((CustomImageButton)e.Row.FindControl("adl")).Click += new ImageClickEventHandler(ImageButton_Click);
                //    ((CustomImageButton)e.Row.FindControl("adlrole")).Click += new ImageClickEventHandler(ImageButton_Click);
                //    ((CustomImageButton)e.Row.FindControl("firmato")).Click += new ImageClickEventHandler(ImageButton_Click);
                //    ((CustomImageButton)e.Row.FindControl("estensionedoc")).Click += new ImageClickEventHandler(ImageButton_Click);
                //    //tooltip
                //    ((CustomImageButton)e.Row.FindControl("estensionedoc")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateEstensioneDoc", UIManager.UserManager.GetUserLanguage()) + " " + estensione;
                //    ((CustomImageButton)e.Row.FindControl("conservazione")).ToolTip = Utils.Languages.GetLabelFromCode(labelConservazione, UIManager.UserManager.GetUserLanguage());
                //    ((CustomImageButton)e.Row.FindControl("adl")).ToolTip = Utils.Languages.GetLabelFromCode(labelAdl, UIManager.UserManager.GetUserLanguage());
                //    ((CustomImageButton)e.Row.FindControl("adlrole")).ToolTip = Utils.Languages.GetLabelFromCode(labelAdlRole, UIManager.UserManager.GetUserLanguage());
                //    ((CustomImageButton)e.Row.FindControl("firmato")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateFirmato", UIManager.UserManager.GetUserLanguage());
                //    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateVisualizzaDocumento", UIManager.UserManager.GetUserLanguage());
                //    ((CustomImageButton)e.Row.FindControl("eliminadocumento")).Visible = false;
                // }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void gridViewResult_ItemCreated(System.Object sender, GridViewRowEventArgs e)
        {
            try
            {
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

        /// <summary>
        /// Funzione per la visualizzazione dei risutati della ricerca
        /// </summary>
        /// <param name="result">I risultati della ricerca</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private void ShowGrid(Grid selectedGrid, SearchObject[] result, int recordNumber, int selectedPage, EtichettaInfo[] labels)
        {
            bool visibile = false;
            Templates templates = Session["templateRicerca"] as Templates;

            this.CellPosition.Clear();

            gridViewResult = this.HeaderGridView(selectedGrid,
              templates,
              this.ShowGridPersonalization, gridViewResult);

            DataTable dt = this.InitializeDataSet(selectedGrid,
                         this.ShowGridPersonalization);


            if (this.Result != null && this.Result.Length > 0)
            {
                dt = this.FillDataSet(dt, this.Result, selectedGrid, labels, templates, this.ShowGridPersonalization);
                visibile = true;
            }

            //// adding blank row eachone
            //if (dt.Rows.Count == 1 && string.IsNullOrEmpty(dt.Rows[0]["idProfile"].ToString())) dt.Rows.RemoveAt(0);

            //DataTable dt2 = dt;
            //int dtRowsCount = dt.Rows.Count;
            //int index = 1;
            //if (dtRowsCount > 0)
            //{
            //    for (int i = 0; i < dtRowsCount; i++)
            //    {
            //        DataRow dr = dt2.NewRow();
            //        dr.ItemArray = dt2.Rows[index - 1].ItemArray;
            //        dt.Rows.InsertAt(dr, index);
            //        index += 2;
            //    }
            //}

            this.GrigliaResult = dt;
            this.gridViewResult.DataSource = dt;
            this.gridViewResult.DataBind();
            if (this.gridViewResult.Rows.Count > 0) this.gridViewResult.Rows[0].Visible = visibile;

            string gridType = this.GetLabel("projectLitGrigliaStandard");

            this.DocumentCount.Text = recordNumber.ToString();

            this.UpTypeResult.Update();
            this.UpnlGrid.Update();
        }

        /// <summary>
        /// Funzione per l'inizializzazione del data set in base ai campi definiti nella 
        /// griglia
        /// </summary>
        /// <param name="selectedGrid">La griglia su cui basare la creazione del dataset</param>
        /// <returns></returns>
        public DataTable InitializeDataSet(Grid selectedGrid, bool showGridPersonalization)
        {
            try
            {
                // Il dataset da restituire
                DataSet toReturn;

                // La tabella da aggiungere al dataset
                DataTable dataTable;

                // La colonna da aggiungere alla tabella
                DataColumn dataColumn;

                // Inizializzazione del dataset
                toReturn = new DataSet();
                dataTable = new DataTable();
                toReturn.Tables.Add(dataTable);

                List<Field> fields = selectedGrid.Fields.Where(e => e.Visible).OrderBy(f => f.Position).ToList();

                dataTable.Columns.Add("IdProfile", typeof(String));
                dataTable.Columns.Add("FileExtension", typeof(String));
                dataTable.Columns.Add("IsInStorageArea", typeof(Boolean));
                dataTable.Columns.Add("IsInWorkingArea", typeof(Boolean));
                dataTable.Columns.Add("IsInWorkingAreaRole", typeof(Boolean));
                dataTable.Columns.Add("IsSigned", typeof(Boolean));
                dataTable.Columns.Add("ProtoType", typeof(String));

                // Creazione delle colonne
                foreach (Field field in fields)
                {
                    if (!field.FieldId.Equals("C2"))
                    {
                        dataColumn = new DataColumn();
                        dataColumn.DataType = typeof(string);
                        dataColumn.ColumnName = field.FieldId;
                        dataTable.Columns.Add(dataColumn);
                    }
                }

                DataRow drow = dataTable.NewRow();
                dataTable.Rows.Add(drow);
                return dataTable;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
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
                                //valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_IS_PRESENT_NOTE");

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
                                    bool existsCounter = (doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault() != null ? true : false);
                                    if (existsCounter)
                                    {
                                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                    }
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
                            //Codice protocollatore
                            case "D26":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //Nome e cognome protocollatore
                            case "D27":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //Ruolo protocollatore
                            case "D28":
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
                            //OGGETTI CUSTOM
                            default:
                                try
                                {
                                    if (!string.IsNullOrEmpty(doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue))
                                    {
                                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                        if (value.ToUpper().Equals("#CONTATORE_DI_REPERTORIO#"))
                                        {
                                            //reperisco la segnatura di repertorio
                                            string dNumber = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                                            value = DocumentManager.getSegnaturaRepertorio(dNumber, AdministrationManager.AmmGetInfoAmmCorrente(UserManager.GetInfoUser().idAmministrazione).Codice);
                                        }
                                    }
                                    else
                                    {
                                        value = "";
                                    }
                                }
                                catch (Exception e)
                                {
                                    value = "";
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

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
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

                grid.Columns.Clear();

                // Creazione delle colonne
                foreach (Field field in fields)
                {
                    if (!field.FieldId.Equals("C2"))
                    {
                        BoundField column = null;
                        ButtonField columnHL = null;
                        TemplateField columnCKB = null;
                        //if (field.OriginalLabel.ToUpper().Equals("DOCUMENTO"))
                        //{
                        //    columnHL = GridManager.GetLinkColumn(field.Label,
                        //        field.FieldId,
                        //        field.Width);
                        //    columnHL.SortExpression = field.FieldId;
                        //}
                        //else
                        //{

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
                        //}



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
                }
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IdProfile", "IdProfile"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("FileExtension", "FileExtension"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsInStorageArea", "IsInStorageArea"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsInWorkingArea", "IsInWorkingArea"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsInWorkingAreaRole", "IsInWorkingAreaRole"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("ProtoType", "ProtoType"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsSigned", "IsSigned"));
                // Altrimenti si procede con la creazione di una colonna normale

                return grid;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per la ricerca dei documenti
        /// </summary>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private SearchObject[] SearchDocument2(FiltroRicerca[][] searchFilters, int selectedPage, out int recordNumber, out bool outOfMaxRowSearchable)
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

            List<Field> visibleFields = GridManager.SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field))).ToList();
            Field specialField = GridManager.SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(SpecialField)) && ((SpecialField)e).FieldType.Equals(SpecialFieldsEnum.Icons)).FirstOrDefault<Field>();

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

        private bool SearchDocumentFilters()
        {
            //try
            //{
            DocsPaWR.FiltroRicerca[][] qV;
            DocsPaWR.FiltroRicerca[] fVList;
            DocsPaWR.FiltroRicerca fV1;
            //array contenitore degli array filtro di ricerca
            qV = new DocsPaWR.FiltroRicerca[1][];
            qV[0] = new DocsPaWR.FiltroRicerca[1];
            fVList = new DocsPaWR.FiltroRicerca[0];

            string valore = string.Empty;

            #region filtro Archivio (Arrivo, Partenza, Tutti)
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
            fV1.valore = "tipo";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion
            #region filtro tipo
            if (this.cbl_archDoc_E.Items.FindByValue("A") != null)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("A").Selected)
                    fV1.valore = "true";
                else
                    fV1.valore = "false";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.cbl_archDoc_E.Items.FindByValue("P") != null)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("P").Selected)
                    fV1.valore = "true";
                else
                    //valore += "0^";
                    fV1.valore = "false";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.cbl_archDoc_E.Items.FindByValue("I") != null)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("I").Selected)
                    fV1.valore = "true";
                else
                    fV1.valore = "false";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
            fV1.valore = this.cbl_archDoc_E.Items.FindByValue("G").Selected.ToString();
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

            if (this.IsEnabledProfilazioneAllegato && this.cbl_archDoc_E.Items.FindByValue("ALL").Selected)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ALLEGATO.ToString();
                fV1.valore = this.rblFiltriAllegati.SelectedValue.ToString();
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.cbl_archDoc_E.Items.FindByValue("Pr") != null)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("Pr").Selected)
                    //valore += "1";
                    fV1.valore = "true";
                else
                    //valore += "0";
                    fV1.valore = "false";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }



            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
            fV1.valore = "tipo";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion

            #region filtro registro
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
            if (this.plcRegistry.Visible && !string.IsNullOrEmpty(this.DdlRegistries.SelectedValue))
            {
                fV1.valore = this.DdlRegistries.SelectedValue;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion

            #region filtro docNumber
            if (this.ddl_idDocumento_C.SelectedIndex == 0)
            {
                if (this.txt_initIdDoc_C.Text != null && !this.txt_initIdDoc_C.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
                    fV1.valore = this.txt_initIdDoc_C.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            else
            {
                if (this.txt_initIdDoc_C.Text != null && !this.txt_initIdDoc_C.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
                    fV1.valore = this.txt_initIdDoc_C.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.txt_fineIdDoc_C.Text != null && !this.txt_fineIdDoc_C.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
                    fV1.valore = this.txt_fineIdDoc_C.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }


            #endregion
            #region filtro numero protocollo
            if (this.ddl_numProt_E.SelectedIndex == 0)
            {//valore singolo carico NUM_PROTOCOLLO

                if (this.txt_initNumProt_E.Text != null && !this.txt_initNumProt_E.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();
                    fV1.valore = this.txt_initNumProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            else
            {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                if (!this.txt_initNumProt_E.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                    fV1.valore = this.txt_initNumProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.txt_fineNumProt_E.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                    fV1.valore = this.txt_fineNumProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion

            #region filtro data protocollo
            if (this.ddl_dataProt_E.SelectedIndex == 2)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_TODAY.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProt_E.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProt_E.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProt_E.SelectedIndex == 0)
            {//valore singolo carico DATA_PROTOCOLLO
                if (!this.txt_initDataProt_E.Text.Equals(""))
                {

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();
                    fV1.valore = this.txt_initDataProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            if (this.ddl_dataProt_E.SelectedIndex == 1)
            {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                if (string.IsNullOrEmpty(txt_initDataProt_E.Text) ||
                    string.IsNullOrEmpty(txt_fineDataProt_E.Text) ||
                    utils.verificaIntervalloDate(txt_initDataProt_E.Text, txt_fineDataProt_E.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDateProtInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDateProtInterval', 'warning', '');};", true);
                    return false;
                }
                if (!this.txt_initDataProt_E.Text.Equals(""))
                {

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.txt_initDataProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.txt_fineDataProt_E.Text.Equals(""))
                {

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
                    fV1.valore = this.txt_fineDataProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion

            #region filtro data creazione
            if (this.ddl_dataCreazione_E.SelectedIndex == 2)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_TODAY.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_SC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_MC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 0)

                if (this.ddl_dataCreazione_E.SelectedIndex == 0)
                { //valore singolo carico DATA_CREAZIONE
                    if (!this.txt_initDataCreazione_E.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
                        fV1.valore = this.txt_initDataCreazione_E.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

            if (this.ddl_dataCreazione_E.SelectedIndex == 1)
            {//valore singolo carico DATA_CREAZIONE_DAL - DATA_CREAZIONE_AL
                if (string.IsNullOrEmpty(txt_initDataCreazione_E.Text) ||
                    string.IsNullOrEmpty(txt_finedataCreazione_E.Text) ||
                   utils.verificaIntervalloDate(txt_initDataCreazione_E.Text, txt_finedataCreazione_E.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDateCreateInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDateCreateInterval', 'warning', '');};", true);
                    return false;
                }
                if (!this.txt_initDataCreazione_E.Text.Equals(""))
                {

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.txt_initDataCreazione_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.txt_finedataCreazione_E.Text.Equals(""))
                {

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                    fV1.valore = this.txt_finedataCreazione_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion

            #region filtro oggetto
            if (!string.IsNullOrEmpty(this.TxtObject.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.OGGETTO.ToString();
                fV1.valore = utils.DO_AdattaString(this.TxtObject.Text);
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion

            #region filtro mitt/dest
            if (!string.IsNullOrEmpty(this.IdRecipient.Value))
            {
                if (!this.txt_descrMit_E.Text.Equals(""))
                {
                    if (!string.IsNullOrEmpty(this.txt_codMit_E.Text))
                    {
                        if (this.chk_mitt_dest_storicizzati.Checked)
                        {
                            // Ricerca i documenti per i mittenti / destinatari storicizzati
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString();
                            fV1.valore = this.txt_codMit_E.Text;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString();
                            fV1.valore = this.chk_mitt_dest_storicizzati.Checked.ToString();
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        else
                        {
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
                            fV1.valore = this.IdRecipient.Value;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }
                }
            }
            else
            {
                if (!this.txt_descrMit_E.Text.Equals(""))
                {
                    if (!string.IsNullOrEmpty(this.txt_codMit_E.Text))
                    {
                        if (this.chk_mitt_dest_storicizzati.Checked)
                        {
                            // Ricerca i documenti per i mittenti / destinatari storicizzati
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString();
                            fV1.valore = this.txt_codMit_E.Text;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString();
                            fV1.valore = this.chk_mitt_dest_storicizzati.Checked.ToString();
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        else
                        {
                            // Ricerca dell'id del corrispondente a partire dal codice
                            DocsPaWR.Corrispondente corrByCode = AddressBookManager.getCorrispondenteByCodRubrica(this.txt_codMit_E.Text, false);
                            if (corrByCode != null)
                            {
                                this.IdRecipient.Value = corrByCode.systemId;

                                fV1 = new DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
                                fV1.valore = this.IdRecipient.Value;
                                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                            else
                            {
                                fV1 = new DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
                                fV1.valore = this.txt_descrMit_E.Text;
                                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                        }
                    }
                    else
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
                        fV1.valore = this.txt_descrMit_E.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
            }
            #endregion

            #region Filtri Creatore

            if (!string.IsNullOrEmpty(this.idCreatore.Value))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = "ID_AUTHOR";
                fV1.valore = this.idCreatore.Value;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = "CORR_TYPE_AUTHOR";
                fV1.valore = this.rblOwnerType.SelectedValue;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = "EXTEND_TO_HISTORICIZED_AUTHOR";
                fV1.valore = this.chkCreatoreExtendHistoricized.Checked.ToString();
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

            }

            #endregion
            #region Filtri Proprietario

            if (!string.IsNullOrEmpty(this.idProprietario.Value))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_OWNER.ToString();
                fV1.valore = this.idProprietario.Value;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.CORR_TYPE_OWNER.ToString();
                fV1.valore = this.rblProprietarioType.SelectedValue;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

            }

            #endregion

            #region filtro CODICE FASCICOLO
            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                #region costruzione condizione IN per valorizzare il filtro di ricerca IN_CHILD_RIC_ESTESA
                ArrayList listaFascicoli = null;
                if (ProjectManager.getFascicoloSelezionatoFascRapida(this) != null)
                {
                    listaFascicoli = new ArrayList();
                    listaFascicoli.Add(ProjectManager.getFascicoloSelezionatoFascRapida(this));
                }
                else //da Cambiare perchè cerca in tutti i fascicoli indipentemente da quello selezionato !!!
                    listaFascicoli = new ArrayList(ProjectManager.getListaFascicoliDaCodice(this, this.txt_CodFascicolo.Text, UserManager.getRegistroSelezionato(this), "R"));

                string inSubFolder = "IN (";
                for (int k = 0; k < listaFascicoli.Count; k++)
                {
                    DocsPaWR.Folder folder = ProjectManager.getFolder(this, (DocsPaWR.Fascicolo)listaFascicoli[k]);
                    inSubFolder += folder.systemID;
                    if (folder.childs != null && folder.childs.Length > 0)
                    {
                        for (int i = 0; i < folder.childs.Length; i++)
                        {
                            inSubFolder += ", " + folder.childs[i].systemID;
                            inSubFolder = this.getInStringChild(folder.childs[i], inSubFolder);
                        }
                    }
                    inSubFolder += ",";
                }
                inSubFolder = inSubFolder.Substring(0, inSubFolder.Length - 1) + ")";

                #endregion

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.IN_CHILD_RIC_ESTESA.ToString();
                fV1.valore = inSubFolder;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion

            #region filtro Conservazione
            if (this.cb_Conservato.Checked)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.CONSERVAZIONE.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            else if (this.cb_NonConservato.Checked)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.CONSERVAZIONE.ToString();
                fV1.valore = "0";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion


            //ABBATANGELI GIANLUIGI - Filtro per nascondere doc di altre applicazioni
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.FILTRO_APPLICAZIONE.ToString()]))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.COD_EXT_APP.ToString();
                fV1.valore = (System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.FILTRO_APPLICAZIONE.ToString()]);
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            #region Filtro campi profilati
            if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
            {
                this.SaveTemplateDocument();
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.PROFILAZIONE_DINAMICA.ToString();
                fV1.template = this.Template;
                fV1.valore = "Profilazione Dinamica";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString();
                fV1.valore = this.DocumentDdlTypeDocument.SelectedItem.Value;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion

            #region filtro DIAGRAMMI DI STATO
            if (this.DocumentDdlStateDiagram.Visible && this.DocumentDdlStateDiagram.SelectedIndex != 0)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DIAGRAMMA_STATO_DOC.ToString();
                fV1.nomeCampo = this.ddlStateCondition.SelectedValue;
                fV1.valore = this.DocumentDdlStateDiagram.SelectedValue;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion filtro DIAGRAMMI DI STATO

            #region Ordinamento
            List<FiltroRicerca> filterList = GridManager.GetOrderFilter();

            // Se la lista è valorizzata vengono aggiunti i filtri
            if (filterList != null)
            {
                foreach (FiltroRicerca filter in filterList)
                {
                    fVList = utils.addToArrayFiltroRicerca(fVList, filter);
                }
            }

            #endregion

            qV[0] = fVList;
            this.SearchFilters = qV;
            return true;
        }

        private string getInStringChild(DocsPaWR.Folder folder, string inSubFolder)
        {
            if (folder.childs != null && folder.childs.Length > 0)
            {
                for (int i = 0; i < folder.childs.Length; i++)
                {
                    inSubFolder += ", " + folder.childs[i].systemID;
                    inSubFolder = getInStringChild(folder.childs[i], inSubFolder);
                }
            }
            return inSubFolder;
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

        protected void SearchDocumentClose_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.upPnlButtons, this.upPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('SearchDocument','');", true);
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

                    case "T_S_R_S":
                        if (atList != null && atList.Count > 0)
                        {
                            NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                            Corrispondente tempCorrSingle;
                            if (!corrInSess.isRubricaComune)
                                tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                            else
                                tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                            this.txt_codMit_E.Text = tempCorrSingle.codiceRubrica;
                            this.txt_descrMit_E.Text = tempCorrSingle.descrizione;
                            this.IdRecipient.Value = tempCorrSingle.systemId;
                            this.UpPnlSenderRecipient.Update();
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
                    case "F_X_X_S_2":
                        if (atList != null && atList.Count > 0)
                        {
                            NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                            Corrispondente tempCorrSingle;
                            if (!corrInSess.isRubricaComune)
                                tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                            else
                                tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                            this.txtCodiceProprietario.Text = tempCorrSingle.codiceRubrica;
                            this.txtDescrizioneProprietario.Text = tempCorrSingle.descrizione;
                            this.idProprietario.Value = tempCorrSingle.systemId;
                            this.upPnlProprietario.Update();
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



        protected void BuildGridNavigator()
        {
            try
            {
                this.plcNavigator.Controls.Clear();

                if (this.PageCount > 1)
                {
                    Panel panel = new Panel();
                    panel.EnableViewState = true;
                    panel.CssClass = "recordNavigator";

                    int startFrom = 1;
                    if (this.SelectedPage > 6) startFrom = this.SelectedPage - 5;

                    int endTo = 10;
                    if (this.SelectedPage > 6) endTo = this.SelectedPage + 5;
                    if (endTo > this.PageCount) endTo = this.PageCount;

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

                    if (endTo < this.PageCount)
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


        protected void TxtCodeObject_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

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

        private void resetField()
        {
            HttpContext.Current.Session["typeDoc"] = "search";

            this.IdProfileList = null;



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

        private void InitializeObjectValue()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_OGGETTO.ToString())))
            {
                this.MaxLenghtObject = int.Parse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_OGGETTO.ToString()));
            }
        }

        protected void PopulateDDLRegistry()
        {
            foreach (DocsPaWR.Registro reg in RoleManager.GetRoleInSession().registri)
            {
                ListItem item = new ListItem();
                item.Text = reg.codRegistro;
                item.Value = reg.systemId;
                this.DdlRegistries.Items.Add(item);
            }

            if (this.DdlRegistries.Items.Count == 1)
            {
                this.plcRegistry.Visible = false;
                this.UpPnlRegistry.Update();
            }
        }

        protected void txt_CodFascicolo_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                if (!string.IsNullOrEmpty(this.txt_CodFascicolo.Text))
                {
                    this.SearchProjectRegistro();
                }
                else
                {
                    this.txt_CodFascicolo.Text = string.Empty;
                    this.txt_DescFascicolo.Text = string.Empty;
                    this.IdProject.Value = string.Empty;

                }

                this.UpCodFasc.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchProjectRegistro()
        {
            this.txt_DescFascicolo.Text = string.Empty;
            string codClassifica = string.Empty;

            if (string.IsNullOrEmpty(this.txt_CodFascicolo.Text))
            {
                this.txt_DescFascicolo.Text = string.Empty;
                return;
            }

            DocsPaWR.Fascicolo[] listaFasc = getFascicoli(this.Registry);

            if (listaFasc != null)
            {
                if (listaFasc.Length > 0)
                {
                    //caso 1: al codice digitato corrisponde un solo fascicolo
                    if (listaFasc.Length == 1)
                    {
                        this.IdProject.Value = listaFasc[0].systemID;
                        this.txt_DescFascicolo.Text = listaFasc[0].descrizione;
                        if (listaFasc[0].tipo.Equals("G"))
                        {
                            codClassifica = listaFasc[0].codice;
                        }
                        else
                        {
                            //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                            DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                            string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                            codClassifica = codiceGerarchia;
                        }
                    }
                    else
                    {
                        //caso 2: al codice digitato corrispondono piu fascicoli
                        codClassifica = this.txt_CodFascicolo.Text;
                        if (listaFasc[0].tipo.Equals("G"))
                        {
                            //codClassifica = codClassifica;
                        }
                        else
                        {
                            //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                            DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                            string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                            codClassifica = codiceGerarchia;
                        }

                        ////Da Fare
                        //RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codClassifica + "', 'Y')</script>");
                        return;
                    }
                }
                else
                {
                    //caso 0: al codice digitato non corrisponde alcun fascicolo
                    if (listaFasc.Length == 0)
                    {
                        //Provo il caso in cui il fascicolo è chiuso
                        Fascicolo chiusoFasc = ProjectManager.getFascicoloDaCodice(this.Page, this.txt_CodFascicolo.Text);
                        if (chiusoFasc != null && !string.IsNullOrEmpty(chiusoFasc.stato) && chiusoFasc.stato.Equals("C"))
                        {
                            //string msg = @"Attenzione, il fascicolo scelto è chiuso. Pertanto il documento non può essere inserito nel fascicolo selezionato.";
                            string msg = "WarningDocumentFileNoOpen";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            //string msg = @"Attenzione, codice fascicolo non presente.";
                            string msg = "WarningDocumentCodFileNoFound";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        this.txt_DescFascicolo.Text = string.Empty;
                        this.txt_CodFascicolo.Text = string.Empty;
                        this.IdProject.Value = string.Empty;
                    }
                }
            }
            //}
        }

        private DocsPaWR.Fascicolo[] getFascicoli(DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                string codiceFascicolo = this.txt_CodFascicolo.Text;
                listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");
            }
            if (listaFasc != null)
            {
                return listaFasc;
            }
            else
            {
                return null;
            }
        }

        protected void SetAjaxDescriptionProject()
        {
            string dataUser = RoleManager.GetRoleInSession().idGruppo;
            dataUser = dataUser + "-" + this.Registry.systemId;
            if (UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione) != null)
            {
                RapidSenderDescriptionProject.ContextKey = dataUser + "-" + UserManager.GetUserInSession().idAmministrazione + "-" + UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione).ID + "-" + UserManager.GetUserInSession().idPeople + "-" + UIManager.UserManager.GetUserInSession().systemId;
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
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "res", "resizeDiv();", true);
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

        private DocsPaWR.Templates Template
        {
            get
            {
                Templates result = null;
                if (HttpContext.Current.Session["templateRc"] != null)
                {
                    result = HttpContext.Current.Session["templateRc"] as Templates;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["templateRc"] = value;
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

        private bool EnableAjaxAddressBook
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableAjaxAddressBook"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableAjaxAddressBook"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableAjaxAddressBook"] = value;
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

        /// <summary>
        /// Risultati restituiti dalla ricerca.
        /// </summary>
        public List<SchedaDocumento> ListDocs
        {
            get
            {
                return HttpContext.Current.Session["listDocs"] as List<SchedaDocumento>;
            }
            set
            {
                HttpContext.Current.Session["listDocs"] = value;
            }
        }

    }
}