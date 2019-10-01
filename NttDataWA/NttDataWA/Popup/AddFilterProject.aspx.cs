using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;
using NttDatalLibrary;
using System.Collections;
using System.Web.UI.HtmlControls;
using NttDataWA.UserControls;

namespace NttDataWA.Popup
{
    public partial class AddFilterProject : System.Web.UI.Page
    {
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
                return (FiltroRicerca[][])HttpContext.Current.Session["filtroRicerca"];
            }
            set
            {
                HttpContext.Current.Session["filtroRicerca"] = value;
            }
        }

        private List<GridItemAddDocInProject> ListaDocPrivati
        {
            get
            {
                return (List<GridItemAddDocInProject>)HttpContext.Current.Session["ListaDocPrivati"];
            }
            set
            {
                HttpContext.Current.Session["ListaDocPrivati"] = value;
            }
        }

        private List<GridItemAddDocInProject> ListaDocUtente
        {
            get
            {
                return (List<GridItemAddDocInProject>)HttpContext.Current.Session["ListaDocUtente"];
            }
            set
            {
                HttpContext.Current.Session["ListaDocUtente"] = value;
            }
        }

        private List<GridItemAddDocInProject> ListaDocNonInseriti
        {
            get
            {
                return (List<GridItemAddDocInProject>)HttpContext.Current.Session["ListaDocNonInseriti"];
            }
            set
            {
                HttpContext.Current.Session["ListaDocNonInseriti"] = value;
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


        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
                try {
                if (!IsPostBack)
                {
                    this.resetField();
                    this.InitializePage();
                    if (HttpContext.Current.Session["FiltroRicerca"] != null)
                    {
                        FiltroRicerca[][] filters = HttpContext.Current.Session["FiltroRicerca"] as FiltroRicerca[][];
                        BindFilterValues(filters);
                    }
                }
                else
                {
                    this.setValueReturn();
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

                this.ReApplyScripts();
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
            try {
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public void inserisciOggettoEsterno(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try {
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void inserisciCampoSeparatore(DocsPaWR.OggettoCustom oggettoCustom)
        {
            try {
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void inserisciContatoreSottocontatore(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try {
                bool paneldll = false;

                if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                    return;

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

                if (etichettaContatore.Visible)
                {
                    this.PnlTypeDocument.Controls.Add(divRowDesc);
                }


                CustomTextArea contatoreDa = new CustomTextArea();
                contatoreDa.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
                contatoreDa.Text = oggettoCustom.VALORE_DATABASE;
                contatoreDa.CssClass = "txt_textdata_custom";
                contatoreDa.EnableViewState = true;

                CustomTextArea contatoreA = new CustomTextArea();
                contatoreA.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
                contatoreA.Text = oggettoCustom.VALORE_DATABASE;
                contatoreA.CssClass = "txt_textdata_custom";
                contatoreA.EnableViewState = true;

                CustomTextArea sottocontatoreDa = new CustomTextArea();
                sottocontatoreDa.ID = "da_sottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
                sottocontatoreDa.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
                sottocontatoreDa.CssClass = "txt_textdata_custom";
                sottocontatoreDa.EnableViewState = true;

                CustomTextArea sottocontatoreA = new CustomTextArea();
                sottocontatoreA.ID = "a_sottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
                sottocontatoreA.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
                sottocontatoreA.CssClass = "txt_textdata_custom";
                sottocontatoreA.EnableViewState = true;

                UserControls.Calendar dataSottocontatoreDa = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
                dataSottocontatoreDa.ID = "da_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
                dataSottocontatoreDa.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);

                UserControls.Calendar dataSottocontatoreA = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
                dataSottocontatoreA.ID = "a_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
                dataSottocontatoreA.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);

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

                if (etichettaDataFrom.Visible)
                {
                    this.PnlTypeDocument.Controls.Add(divRowValueFrom);
                }

                Panel divRowValue = new Panel();
                divRowValue.CssClass = "row";
                divRowValue.EnableViewState = true;

                Panel divColValue = new Panel();
                divColValue.CssClass = "col";
                divColValue.EnableViewState = true;

                divColValue.Controls.Add(contatoreDa);
                divRowValue.Controls.Add(divColValue);

                if (contatoreDa.Visible)
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

                if (etichettaDataTo.Visible)
                {
                    this.PnlTypeDocument.Controls.Add(divRowValueTo);
                }

                Panel divRowValue2 = new Panel();
                divRowValue2.CssClass = "row";
                divRowValue2.EnableViewState = true;


                Panel divColValue2 = new Panel();
                divColValue2.CssClass = "col";
                divColValue2.EnableViewState = true;

                divColValue2.Controls.Add(contatoreA);
                divRowValue2.Controls.Add(divColValue2);

                if (contatoreA.Visible)
                {
                    this.PnlTypeDocument.Controls.Add(divRowValue2);
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
                        contatoreA.Text = string.Empty;
                    }
                }

                if (!string.IsNullOrEmpty(oggettoCustom.VALORE_SOTTOCONTATORE))
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

                if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
                {
                    if (oggettoCustom.DATA_INSERIMENTO.IndexOf("@") != -1)
                    {
                        string[] date = oggettoCustom.DATA_INSERIMENTO.Split('@');
                        dataSottocontatoreDa.Text = date[0].ToString();
                        dataSottocontatoreA.Text = date[1].ToString();
                    }
                    else
                    {
                        dataSottocontatoreDa.Text = oggettoCustom.DATA_INSERIMENTO.ToString();
                        dataSottocontatoreA.Text = string.Empty;
                    }
                }

                //Label etichettaContatoreDa = new Label();
                //etichettaContatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
                //etichettaContatoreDa.Font.Size = FontUnit.Point(8);
                //etichettaContatoreDa.Font.Bold = true;
                //etichettaContatoreDa.Font.Name = "Verdana";
                //Label etichettaContatoreA = new Label();
                //etichettaContatoreA.Text = "&nbsp;a&nbsp;";
                //etichettaContatoreA.Font.Size = FontUnit.Point(8);
                //etichettaContatoreA.Font.Bold = true;
                //etichettaContatoreA.Font.Name = "Verdana";

                //Label etichettaSottocontatoreDa = new Label();
                //etichettaSottocontatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
                //etichettaSottocontatoreDa.Font.Size = FontUnit.Point(8);
                //etichettaSottocontatoreDa.Font.Bold = true;
                //etichettaSottocontatoreDa.Font.Name = "Verdana";
                //Label etichettaSottocontatoreA = new Label();
                //etichettaSottocontatoreA.Text = "&nbsp;a&nbsp;";
                //etichettaSottocontatoreA.Font.Size = FontUnit.Point(8);
                //etichettaSottocontatoreA.Font.Bold = true;
                //etichettaSottocontatoreA.Font.Name = "Verdana";

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
                        contatoreA.Text = string.Empty;
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

                if (!string.IsNullOrEmpty(oggettoCustom.VALORE_SOTTOCONTATORE))
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
                        sottocontatoreA.Text = string.Empty;
                    }
                }

                if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
                {
                    if (oggettoCustom.DATA_INSERIMENTO.IndexOf("@") != -1)
                    {
                        string[] date = oggettoCustom.DATA_INSERIMENTO.Split('@');
                        dataSottocontatoreDa.Text = date[0].ToString();
                        dataSottocontatoreA.Text = date[1].ToString();
                    }
                    else
                    {
                        dataSottocontatoreDa.Text = oggettoCustom.DATA_INSERIMENTO.ToString();
                        dataSottocontatoreA.Text = string.Empty;
                    }
                }

                //Verifico i diritti del ruolo sul campo
                foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
                {
                    if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                    {
                        if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                        {
                            //etichettaContatoreSottocontatore.Visible = false;
                            //etichettaContatoreDa.Visible = false;
                            contatoreDa.Visible = false;
                            //etichettaContatoreA.Visible = false;
                            contatoreA.Visible = false;
                            //etichettaSottocontatoreDa.Visible = false;
                            sottocontatoreDa.Visible = false;
                            //etichettaSottocontatoreA.Visible = false;
                            sottocontatoreA.Visible = false;
                            //etichettaDataSottocontatoreDa.Visible = false;
                            dataSottocontatoreDa.Visible = false;
                            //etichettaDataSottocontatoreA.Visible = false;
                            dataSottocontatoreA.Visible = false;
                            etichettaDDL.Visible = false;
                            ddl.Visible = false;
                        }
                    }
                }

                if (paneldll)
                {
                    this.PnlTypeDocument.Controls.Add(divRowEtiDll);
                    this.PnlTypeDocument.Controls.Add(divRowDll);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void inserisciLink(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try {
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void HandleInternalDoc(string idDoc)
        {
            //InfoDocumento infoDoc = DocumentManager.GetInfoDocumento(idDoc, null, this);
            //if (infoDoc == null)
            //{
            //    Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
            //    return;
            //}
            //string errorMessage = "";
            //int result = DocumentManager.verificaACL("D", infoDoc.idProfile, UserManager.getInfoUtente(), out errorMessage);
            //if (result != 2)
            //{
            //    Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
            //}
            //else
            //{
            //    DocumentManager.setRisultatoRicerca(this, infoDoc);
            //    Response.Write("<script language='javascript'> top.principale.document.location = '../documento/gestionedoc.aspx?tab=protocollo&forceNewContext=true';</script>");
            //}
        }

        private void HandleInternalFasc(string idFasc)
        {
            //Fascicolo fasc = FascicoliManager.getFascicoloById(this, idFasc);
            //if (fasc == null)
            //{
            //    Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
            //    return;
            //}
            //string errorMessage = "";
            //int result = DocumentManager.verificaACL("F", fasc.systemID, UserManager.getInfoUtente(), out errorMessage);
            //if (result != 2)
            //{
            //    Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
            //}
            //else
            //{
            //    FascicoliManager.setFascicoloSelezionato(this, fasc);
            //    string newUrl = "../fascicolo/gestioneFasc.aspx?tab=documenti&forceNewContext=true";
            //    Response.Write("<script language='javascript'>top.principale.document.location='" + newUrl + "';</script>");
            //}
        }

        private void inserisciCorrispondente(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try {
                if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                {
                    return;
                }
                DocsPaWR.StoricoProfilatiOldValue corrOldOb = new StoricoProfilatiOldValue();

                UserControls.CorrespondentCustom corrispondente = (UserControls.CorrespondentCustom)this.LoadControl("../UserControls/CorrespondentCustom.ascx");
                corrispondente.EnableViewState = true;
                corrispondente.PageCaller = "Popup";

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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void inserisciData(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try {
                //Per il momento questo tipo di campo è stato implementato con tre semplici textBox
                //Sarebbe opportuno creare un oggetto personalizzato, che espone le stesse funzionalità
                //della textBox, ma che mi permette di gestire la data con i tre campi separati.

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

                this.impostaDirittiRuoloData(etichettaData, etichettaDataFrom, etichettaDataTo, data, data2, oggettoCustom, this.Template, dirittiCampiRuolo);

                if (data.Visible)
                {
                    this.PnlTypeDocument.Controls.Add(divRowValue2);
                }

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public void impostaDirittiRuoloData(System.Object etichettaData, System.Object etichettaDataDa, System.Object etichettaDataA, System.Object dataDa, System.Object dataA, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try {
                foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
                {
                    if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                    {
                        if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                        {
                            ((System.Web.UI.WebControls.Label)etichettaData).Visible = false;
                            ((System.Web.UI.WebControls.Label)etichettaDataDa).Visible = false;
                            ((System.Web.UI.WebControls.Label)etichettaDataA).Visible = false;
                            ((UserControls.Calendar)dataDa).Visible = false;
                            ((UserControls.Calendar)dataDa).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                            ((UserControls.Calendar)dataA).Visible = false;
                            ((UserControls.Calendar)dataA).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
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

            Ruolo ruoloUtente = RoleManager.GetRoleInSession();
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
        }

        public void impostaDirittiRuoloContatore(System.Object etichettaContatore, System.Object contatoreDa, System.Object contatoreA, System.Object etichettaContatoreDa, System.Object etichettaContatoreA, System.Object etichettaDDL, System.Object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try {
                //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneDocManager.getDirittiCampoTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

                foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
                {
                    if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                    {
                        if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                        {
                            ((System.Web.UI.WebControls.Label)etichettaContatore).Visible = false;

                            ((System.Web.UI.WebControls.Label)etichettaContatoreDa).Visible = false;
                            ((System.Web.UI.WebControls.TextBox)contatoreDa).Visible = false;
                            ((System.Web.UI.WebControls.Label)etichettaContatoreA).Visible = false;
                            ((System.Web.UI.WebControls.TextBox)contatoreA).Visible = false;

                            ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                            ((System.Web.UI.WebControls.DropDownList)ddl).Visible = false;
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

        private void inserisciSelezioneEsclusiva(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try {
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public void impostaDirittiRuoloSelezioneEsclusiva(System.Object etichetta, System.Object campo, System.Object button, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try {
                foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
                {
                    if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                    {
                        if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                        {
                            ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                            ((System.Web.UI.WebControls.RadioButtonList)campo).Visible = false;
                            ((CustomImageButton)button).Visible = false;
                            oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cancella_selezioneEsclusiva_Click(object sender, EventArgs e)
        {
            try {
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

        private void inserisciMenuATendina(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try {
                if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                {
                    return;
                }

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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void inserisciCampoDiTesto(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try {
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void inserisciCasellaDiSelezione(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try {
                if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                {
                    return;
                }
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
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
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
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
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
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
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
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
                            //if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                            //{
                            //    ((UserControls.Calendar)campo).ReadOnly = true;
                            //    oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            //}
                            //if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            //{
                            //    ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                            //    ((UserControls.Calendar)campo).Visible = false;
                            //    ((UserControls.Calendar)campo).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                            //    oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            //}
                            break;
                        case "Corrispondente":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
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
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.CorrespondentCustom)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Link":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
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
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.LinkDocFasc)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "OggettoEsterno":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
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


        private void InitializePage()
        {
            this.InitializeLabel();
            this.InitializeObjectValue();
        }

        private void removeSession()
        {
            HttpContext.Current.Session.Remove("enableCodeObject");
            HttpContext.Current.Session.Remove("AddDocInProject");
            HttpContext.Current.Session.Remove("PagesCount");
            HttpContext.Current.Session.Remove("SelectedPage");
            //HttpContext.Current.Session.Remove("filtroRicerca");
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

            if (!(!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString())) && Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString()).Equals("1")))
            {
                this.rbl_TipoDoc.Items.Remove(this.rbl_TipoDoc.Items.FindByValue("I"));
            }
        }


        private void InitializeLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            this.AddFilterProjectBtnInserisci.Text = Utils.Languages.GetLabelFromCode("AddFilterProjectFilter", language);
            this.AddFilterProjectClose.Text = Utils.Languages.GetLabelFromCode("AddDocInProjectBtnClose", language);
            this.LblAddDocNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumProtocol", language);
            this.LblAddDocDa.Text = Utils.Languages.GetLabelFromCode("LblAddDocDa", language) + " ";
            this.LblAddDocA.Text = Utils.Languages.GetLabelFromCode("LblAddDocA", language) + " ";
            this.LblAddDocAnno.Text = Utils.Languages.GetLabelFromCode("LblAddDocAnno", language) + " ";
            this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocData", language) + " ";
            List<EtichettaInfo> etichette = UIManager.DocumentManager.GetLettereProtocolli();
            this.Arrivo.Text = (from e in etichette where e.Codice.ToLower().Equals("A".ToLower()) select e.Etichetta).FirstOrDefault<string>();
            this.Partenza.Text = (from e in etichette where e.Codice.ToLower().Equals("P".ToLower()) select e.Etichetta).FirstOrDefault<string>();
            this.Grigio.Text = (from e in etichette where e.Codice.ToLower().Equals("G".ToLower()) select e.Etichetta).FirstOrDefault<string>();
            this.Interno.Text = (from e in etichette where e.Codice.ToLower().Equals("I".ToLower()) select e.Etichetta).FirstOrDefault<string>();
            this.Tutti.Text = Utils.Languages.GetLabelFromCode("AddFilterInProjectAll", language);
            this.LblAddDocDataDa.Text = Utils.Languages.GetLabelFromCode("LblAddDocDa", language) + " ";
            this.LblAddDocDataA.Text = Utils.Languages.GetLabelFromCode("LblAddDocA", language) + " ";
            this.projectLitVisibleObjectChars.Text = Utils.Languages.GetLabelFromCode("DocumentLitObjectChAv", language);
            this.DocumentDdlTypeDocument.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DdlTipologiaFascciolo", language));
            this.ddl_tipoFileAcquisiti.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("ddl_tipoFileAcquisiti", language));
            this.AddFilerProjectLitType.Text = Utils.Languages.GetLabelFromCode("AddFilerProjectLitType", language);
            this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("DocumentLitTypeDocument", language);
            this.Object.Title = Utils.Languages.GetLabelFromCode("TitleObjectPopup", language);
            this.AddFilterProjectLitFile.Text = Utils.Languages.GetLabelFromCode("AddFilterProjectLitFile", language);
            this.DocumentImgObjectary.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgObjectary", language);
            this.DocumentImgObjectary.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgObjectary", language);
            this.cb_firmato.Text = Utils.Languages.GetLabelFromCode("AddFilterProjectSigned", language);
            this.cb_nonFirmato.Text = Utils.Languages.GetLabelFromCode("AddFilterProjectNotSigned", language);
            this.l_rubrica.Text = Utils.Languages.GetLabelFromCode("l_rubricaAddFilterProject", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
            this.AddFilte4rProjectImgAddressBook.AlternateText = Utils.Languages.GetLabelFromCode("AddFilte4rProjectImgAddressBook", language);
            this.AddFilte4rProjectImgAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("AddFilte4rProjectImgAddressBook", language);
            this.LblAddDocOgetto.Text = Utils.Languages.GetLabelFromCode("AddFilterProjectSubject", language);
        }

        private void InitializeObjectValue()
        {
            this.LoadKeys();
            string language = UIManager.UserManager.GetUserLanguage();
            this.ddl_dtaProto.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ddlAddDocSingolo", language), "S"));
            this.ddl_numProto.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ddlAddDocSingolo", language), "S"));
            this.ddl_dtaProto.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ddlAddDocIntervallo", language), "R"));
            this.ddl_numProto.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ddlAddDocIntervallo", language), "R"));
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_OGGETTO.ToString())))
            {
                this.MaxLenghtObject = int.Parse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_OGGETTO.ToString()));
            }
            this.TypeDocument = string.Empty;
            this.AddDoc = true;
            this.Template = null;
            this.EnableStateDiagram = false;
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.USE_CODICE_OGGETTO.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.USE_CODICE_OGGETTO.ToString()]))
            {
                this.EnableCodeObject = true;
            }
            this.LoadTypeDocuments();
            this.ddl_tipoFileAcquisiti = caricaComboTipoFileAcquisiti(ddl_tipoFileAcquisiti);
            this.upPnlButtons.Update();
            this.SelectedPage = 1;
            this.TxtObject.MaxLength = MaxLenghtObject;

            this.plh_rubrica.Visible = false;
            this.plh_documento.Visible = false;
            this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocDtaDoc", UIManager.UserManager.GetUserLanguage());

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

        private void closePage(string _ParametroDiRitorno)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('AddFilterProject', '" + _ParametroDiRitorno + "');} else {parent.closeAjaxModal('AddFilterProject', '" + _ParametroDiRitorno + "');};", true);
        }



        protected void ddl_numProto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                if (ddl_numProto.SelectedValue.Equals("S"))
                {
                    txtAddDocA.Visible = false;
                    LblAddDocA.Visible = false;
                }
                else
                {
                    txtAddDocA.Visible = true;
                    LblAddDocA.Visible = true;
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
            try {
                if (ddl_dtaProto.SelectedValue.Equals("S"))
                {
                    txtAddDocDataA.Visible = false;
                    LblAddDocDataA.Visible = false;
                }
                else
                {
                    txtAddDocDataA.Visible = true;
                    LblAddDocDataA.Visible = true;
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
            try {
                this.plh_documento.Visible = true;
                switch (rbl_TipoDoc.SelectedValue)
                {
                    case "A":
                        {
                            this.LblAddDocNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumProtocol", UIManager.UserManager.GetUserLanguage());
                            this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocData", UIManager.UserManager.GetUserLanguage());
                            this.plh_rubrica.Visible = true;
                            this.l_rubrica.Text = Utils.Languages.GetLabelFromCode("l_rubricaAddFilterProjectSender", UIManager.UserManager.GetUserLanguage());
                            this.resetField();
                            break;
                        }
                    case "P":
                        {
                            this.LblAddDocNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumProtocol", UIManager.UserManager.GetUserLanguage());
                            this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocData", UIManager.UserManager.GetUserLanguage());
                            this.plh_rubrica.Visible = true;
                            this.l_rubrica.Text = Utils.Languages.GetLabelFromCode("l_rubricaAddFilterProject", UIManager.UserManager.GetUserLanguage());
                            this.resetField();
                            break;
                        }
                    case "I":
                        {

                            this.LblAddDocNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumProtocol", UIManager.UserManager.GetUserLanguage());
                            this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocData", UIManager.UserManager.GetUserLanguage());
                            this.plh_rubrica.Visible = true;
                            this.l_rubrica.Text = Utils.Languages.GetLabelFromCode("l_rubricaAddFilterProject", UIManager.UserManager.GetUserLanguage());
                            this.resetField();
                            break;
                        }
                    case "G":
                        {
                            this.LblAddDocNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumDoc", UIManager.UserManager.GetUserLanguage());
                            this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocDtaDoc", UIManager.UserManager.GetUserLanguage());
                            this.plh_rubrica.Visible = false;
                            this.resetField();
                            break;
                        }
                    case "T":
                        {
                            this.plh_rubrica.Visible = false;
                            this.plh_documento.Visible = false;
                            this.LblAddDocData.Text = Utils.Languages.GetLabelFromCode("LblAddDocDtaDoc", UIManager.UserManager.GetUserLanguage());
                            this.resetField();
                            break;
                        }
                }
                this.upPnlButtons.Update();
                this.UplnRadioButton.Update();
                this.UplnFiltri.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void AddFilterProjectClose_Click(object sender, EventArgs e)
        {
            try {
                removeSession();
                closePage(string.Empty);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        private void ReApplyScripts()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen_deselect", "$('.chzn-select-deselect').chosen({ allow_single_deselect: true, no_results_text: 'Nessun risultato trovato' });", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen", "$('.chzn-select').chosen({ no_results_text: 'Nessun risultato trovato' });", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "datepicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "onlynumbers", "OnlyNumbers();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshNoteChars", "charsLeft('TxtObject', " + this.MaxLenghtObject + ", '" + this.projectLitVisibleObjectChars.Text.Replace("'", "\'") + "');", true);
            this.TxtObject_chars.Attributes["rel"] = "TxtObject_" + this.MaxLenghtObject + "_" + this.projectLitVisibleObjectChars.Text;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);

        }

        private void resetField()
        {
            this.txtAddDocDa.Text = string.Empty;
            this.txtAddDocDataDA.Text = string.Empty;
            this.txtAddDocAnno.Text = DateTime.Now.Year.ToString();
            this.txtAddDocA.Text = string.Empty;
            this.txtAddDocDataA.Text = string.Empty;

            this.txtAddDocA.Visible = false;
            this.LblAddDocA.Visible = false;
            this.txtAddDocDataA.Visible = false;
            this.LblAddDocDataA.Visible = false;
            this.ddl_dtaProto.SelectedIndex = 0;
            this.ddl_numProto.SelectedIndex = 0;
        }


        private void setValueReturn()
        {
            if (!string.IsNullOrEmpty(this.Object.ReturnValue))
            {
                this.TxtObject.Text = this.ReturnValue.Split('#').First();
                if (this.ReturnValue.Split('#').Length > 1)
                    this.TxtCodeObject.Text = this.ReturnValue.Split('#').Last();
                this.UpdPnlObject.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('Object','')", true);
            }
            if (!string.IsNullOrEmpty(AddressBook.ReturnValue))
            {

            }
        }




        protected void TxtCodeObject_Click(object sender, EventArgs e)
        {
            try {
                List<Registro> registries = new List<Registro>();
                registries = UIManager.RegistryManager.GetListRegistriesAndRF(UIManager.RoleManager.GetRoleInSession().systemId, "1", string.Empty).ToList<Registro>();
                registries.Add(UIManager.RegistryManager.GetRegistryInSession());

                List<string> aL = new List<string>();
                if (registries != null)
                {
                    for (int i = 0; i < registries.Count; i++)
                    {
                        aL.Add(registries[i].systemId);
                    }
                }

                Oggetto[] listaObj = null;

                // E' inutile finire nel backend se la casella di testo è vuota (a parte il fatto che 
                // la funzione, in questo caso, restituisce tutto l'oggettario)
                if (!string.IsNullOrEmpty(this.TxtCodeObject.Text.Trim()))
                {
                    //In questo momento tralascio la descrizione oggetto che metto come stringa vuota
                    listaObj = DocumentManager.getListaOggettiByCod(aL.ToArray<string>(), string.Empty, this.TxtCodeObject.Text);
                }
                else
                {
                    listaObj = new Oggetto[] { 
                            new Oggetto()
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
                if (int.Parse(numeroDa) < int.Parse(numeroA))
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


        protected void AddFilterProjectBtnOk_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                HttpContext.Current.Session["FiltroRicerca"] = GetFilters();
                this.closePage("up");

                this.upPnlButtons.Update();
                this.UplnRadioButton.Update();
                this.UplnFiltri.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TrasmissionsImgAddressBook_Click(object sender, EventArgs e)
        {
            try {
                // Gabriele Melini 05-03-2014
                // richiamo la rubrica con lo stesso calltype utilizzato per la ricerca
                // di mittente/destinatario nell'inserimento di protocolli in arrivo/partenza
                //this.CallType = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
                this.CallType = this.GetCallType(string.Empty);
                HttpContext.Current.Session["AddressBook.from"] = "T_S_R_S";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpPnlSender", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            try  {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = this.TxtRecipientCode.Text;

                if (!string.IsNullOrEmpty(codeAddressBook))
                {
                    this.SearchCorrespondent(codeAddressBook, caller.ID);
                }
                else
                {
                    this.TxtRecipientCode.Text = string.Empty;
                    this.TxtRecipientDescription.Text = string.Empty;
                    this.IdRecipient.Value = string.Empty;
                    this.UpPnlRecipient.Update();
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
            try {
                RubricaCallType calltype = GetCallType(idControl);
                Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteRubrica(addressCode, calltype);
                if (corr == null)
                {
                    this.TxtRecipientCode.Text = string.Empty;
                    this.TxtRecipientDescription.Text = string.Empty;
                    this.IdRecipient.Value = string.Empty;
                    this.RecipientTypeOfCorrespondent.Value = string.Empty;
                    this.RecipientTypeOfCorrespondent.Value = string.Empty;

                    string msg = "ErrorTransmissionCorrespondentNotFound";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
                else
                {
                    this.TxtRecipientCode.Text = corr.codiceRubrica;
                    this.TxtRecipientDescription.Text = corr.descrizione;

                    if (corr.GetType() == typeof(Utente))
                    {
                        //this.IdRecipient.Value = corr.codiceRubrica + "|" + corr.idAmministrazione;
                        this.IdRecipient.Value = corr.systemId;
                        this.RecipientTypeOfCorrespondent.Value = "U";
                    }
                    else if (corr.GetType() == typeof(Ruolo))
                    {
                        this.IdRecipient.Value = corr.systemId;
                        this.RecipientTypeOfCorrespondent.Value = "R";
                    }
                    else if (corr.GetType() == typeof(UnitaOrganizzativa))
                    {
                        this.TxtRecipientCode.Text = string.Empty;
                        this.TxtRecipientDescription.Text = string.Empty;
                        this.IdRecipient.Value = string.Empty;
                        this.RecipientTypeOfCorrespondent.Value = string.Empty;

                        string msg = "ErrorTransmissionCorrespondentNotFound";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                }

                this.UpPnlRecipient.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected RubricaCallType GetCallType(string idControl)
        {
            // Gabriele Melini 05-03-2014
            // richiamo la rubrica con lo stesso calltype utilizzato per la ricerca
            // di mittente/destinatario nell'inserimento di protocolli in arrivo/partenza
            //RubricaCallType calltype = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
            RubricaCallType calltype = new RubricaCallType();
            switch (rbl_TipoDoc.SelectedValue)
            {
                case "A":
                    calltype = RubricaCallType.CALLTYPE_PROTO_IN;
                    break;
                case "P":
                    calltype = RubricaCallType.CALLTYPE_PROTO_OUT;
                    break;
                case "I":
                    calltype = RubricaCallType.CALLTYPE_PROTO_INT_DEST;
                    break;
                default:
                    calltype = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
                    break;
            }
            return calltype;
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

        protected void DocumentDdlTypeDocument_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                {
                    if (this.CustomDocuments)
                    {
                        this.Template = ProfilerDocManager.getTemplateById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                        if (this.Template != null)
                        {
                            Session["templateRicerca"] = this.Template;
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

        private DropDownList caricaComboTipoFileAcquisiti(DropDownList ddl_tipoFileAcquisiti)
        {
            string[] tipoFile = DocumentManager.getExtFileAcquisiti(UserManager.GetInfoUser()).ToArray();

            foreach (string ext in tipoFile)
            {
                if (!ext.Contains("P7M"))
                    ddl_tipoFileAcquisiti.Items.Add(new ListItem(ext));
            }

            return ddl_tipoFileAcquisiti;
        }


        /// <summary>
        /// Creazione oggetti filtro
        /// </summary>
        /// <returns></returns>
        private FiltroRicerca[][] GetFilters()
        {
            ArrayList filterItems = new ArrayList();

            this.AddFilterTipoDocumento(filterItems);

            switch (this.rbl_TipoDoc.SelectedValue)
            {
                case "A":
                case "P":
                case "I":
                    this.AddFilterNumProtocollo(filterItems);
                    this.AddFilterDataProtocollo(filterItems);
                    this.AddFilterMittDestDocumento(filterItems);
                    break;

                case "G":
                    this.AddFilterIDDocumento(filterItems);
                    this.AddFilterDataCreazioneDocumento(filterItems);

                    break;

                case "T":
                    this.AddFilterDataCreazioneDocumento(filterItems);
                    break;
            }

            this.AddFilterOggettoDocumento(filterItems);
            this.AddFilterTipologiaDocumento(filterItems);
            this.AddProfilazioneDinamica(filterItems);
            this.AddFilterFileFirmato(filterItems);
            this.AddFilterTipoFileAcquisito(filterItems);

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

            FiltroRicerca[][] retValue = new FiltroRicerca[1][];
            retValue[0] = initArray;
            return retValue;
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

        /* ABBATANGELI GIANLUIGI */
        /// <summary>
        /// Creazione filtro per applicazione
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterApplicazioneDocumento(ArrayList filterItems)
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"]))
            {
                FiltroRicerca filterItem = new FiltroRicerca();
                filterItem.argomento = FiltriDocumento.COD_EXT_APP.ToString();
                filterItem.valore = (System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"]);//this.ddl_extApp.SelectedItem.Value;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per soggetto mittente / destinatario
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterMittDestDocumento(ArrayList filterItems)
        {
            FiltroRicerca filterItem = new FiltroRicerca();

            string filterArgID = string.Empty;
            string filterArgDescr = string.Empty;

            switch (this.rbl_TipoDoc.SelectedValue)
            {
                case "A":
                    filterArgID = FiltriDocumento.ID_MITT_DEST.ToString();
                    filterArgDescr = FiltriDocumento.MITT_DEST.ToString();
                    break;
                case "P":
                case "I":
                    filterArgID = FiltriDocumento.ID_DESTINATARIO.ToString();
                    filterArgDescr = FiltriDocumento.ID_DESCR_DESTINATARIO.ToString();
                    break;
            }

            if (!string.IsNullOrEmpty(this.TxtRecipientDescription.Text) &&
                !string.IsNullOrEmpty(this.IdRecipient.Value))
            {
                //string id = this.IdRecipient.Value.Split('|')[1];
                string id = this.IdRecipient.Value;
                filterItem.argomento = filterArgID;
                filterItem.valore = id;
                filterItems.Add(filterItem);
            }
            else if (!string.IsNullOrEmpty(this.TxtRecipientDescription.Text))
            {
                filterItem.argomento = filterArgDescr;
                filterItem.valore = this.TxtRecipientDescription.Text;
                filterItems.Add(filterItem);
            }

            filterItem = null;
        }

        private void AddFilterTipoFileAcquisito(ArrayList filterItems)
        {
            if (this.ddl_tipoFileAcquisiti.SelectedIndex > 0)
            {
                FiltroRicerca filterItem = new FiltroRicerca();
                filterItem.argomento = FiltriDocumento.TIPO_FILE_ACQUISITO.ToString();
                filterItem.valore = this.ddl_tipoFileAcquisiti.SelectedItem.Value;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        private void AddFilterFileFirmato(ArrayList filterItems)
        {
            if (this.cb_firmato.Checked)
            {
                //cerco documenti firmati
                if (!cb_nonFirmato.Checked)
                {
                    FiltroRicerca filterItem = new FiltroRicerca();
                    filterItem.argomento = FiltriDocumento.FIRMATO.ToString();
                    filterItem.valore = "1";
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
                else
                {//cerco documenti che abbiano un file acquisito, sia esso firmato o meno.
                    FiltroRicerca filterItem = new FiltroRicerca();
                    filterItem.argomento = FiltriDocumento.FIRMATO.ToString();
                    filterItem.valore = "2";
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
            }
            else
            {
                //cerco i documenti non firmati
                if (cb_nonFirmato.Checked)
                {
                    FiltroRicerca filterItem = new FiltroRicerca();
                    filterItem.argomento = FiltriDocumento.FIRMATO.ToString();
                    filterItem.valore = "0";
                    filterItems.Add(filterItem);
                    filterItem = null;
                }

            }

        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try {
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
                            //this.UpPnlTypeDocument.Update();
                            this.UpPnlRecipient.Update();
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

                            this.TxtRecipientCode.Text = tempCorrSingle.codiceRubrica;
                            this.TxtRecipientDescription.Text = tempCorrSingle.descrizione;
                            this.IdRecipient.Value = tempCorrSingle.systemId;
                            this.UpPnlRecipient.Update();
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

        private void BindFilterValues(FiltroRicerca[][] filters)
        {
            try
            {
                foreach (DocsPaWR.FiltroRicerca item in filters[0])
                {
                    #region TIPO
                    if (item.argomento == DocsPaWR.FiltriDocumento.TIPO.ToString())
                    {
                        this.rbl_TipoDoc.Items.FindByValue("T").Selected = false;
                        this.rbl_TipoDoc.Items.FindByValue(item.valore).Selected = true;
                        rbl_TipoDoc_SelectedIndexChanged(null, new System.EventArgs());
                    }
                    #endregion

                    #region DATA_CREAZIONE_IL DATA_PROTO_IL
                    else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString() || item.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString())
                    {
                        if (ddl_dtaProto.SelectedIndex != 0)
                            ddl_dtaProto.SelectedIndex = 0;
                        ddl_dtaProto_SelectedIndexChanged(null, new System.EventArgs());
                        this.txtAddDocDataDA.Visible = true;
                        this.txtAddDocDataDA.Visible = true;
                        this.txtAddDocDataDA.Text = item.valore;
                    }
                    #endregion DATA_CREAZIONE_IL
                    #region DATA_CREAZIONE_SUCCESSIVA_AL DATA_PROTO_SUCCESSIVA_AL
                    else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString() || item.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString())
                    {
                        if (ddl_dtaProto.SelectedIndex != 1)
                            ddl_dtaProto.SelectedIndex = 1;
                        ddl_dtaProto_SelectedIndexChanged(null, new System.EventArgs());
                        this.txtAddDocDataDA.Visible = true;
                        this.txtAddDocDataDA.Visible = true;
                        this.txtAddDocDataDA.Text = item.valore;
                    }
                    #endregion DATA_CREAZIONE_SUCCESSIVA_AL
                    #region DATA_CREAZIONE_PRECEDENTE_IL DATA_PROTO_PRECEDENTE_IL
                    else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString() || item.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString())
                    {
                        if (this.ddl_dtaProto.SelectedIndex != 1)
                            this.ddl_dtaProto.SelectedIndex = 1;
                        this.ddl_dtaProto_SelectedIndexChanged(null, new System.EventArgs());
                        this.txtAddDocDataA.Visible = true;
                        this.txtAddDocDataA.Visible = true;
                        this.txtAddDocDataA.Text = item.valore;
                    }
                    #endregion DATA_CREAZIONE_PRECEDENTE_IL
                    #region NUM_PROTOCOLLO-DOCNUMBER
                    else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString() || item.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER.ToString())
                    {
                        if (this.ddl_numProto.SelectedIndex != 0)
                            this.ddl_numProto.SelectedIndex = 0;
                        this.ddl_numProto_SelectedIndexChanged(null, new System.EventArgs());
                        this.txtAddDocDa.Text = item.valore;
                    }
                    #endregion NUM_PROTOCOLLO-DOCNUMBER
                    #region NUM_PROTOCOLLO_DAL-DOCNUMBER_DAL
                    else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString() || item.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString())
                    {
                        if (this.ddl_numProto.SelectedIndex != 1)
                            this.ddl_numProto.SelectedIndex = 1;
                        this.ddl_numProto_SelectedIndexChanged(null, new System.EventArgs());
                        this.txtAddDocDa.Text = item.valore;
                    }
                    #endregion NUM_PROTOCOLLO_DAL-DOCNUMBER_DAL
                    #region NUM_PROTOCOLLO_AL-DOCNUMBER_AL
                    else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString() || item.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString())
                    {
                        if (this.ddl_numProto.SelectedIndex != 1)
                            this.ddl_numProto.SelectedIndex = 1;
                        this.ddl_numProto_SelectedIndexChanged(null, new System.EventArgs());
                        this.txtAddDocA.Text = item.valore;
                    }
                    #endregion NUM_PROTOCOLLO_AL-DOCNUMBER_AL
                    #region ANNO_PROTOCOLLO
                    else if (item.argomento == DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString())
                    {
                        this.txtAddDocAnno.Text = item.valore;
                    }
                    #endregion ANNO_PROTOCOLLO
                    #region OGGETTO
                    else if (item.argomento == DocsPaWR.FiltriDocumento.OGGETTO.ToString())
                    {
                        this.TxtObject.Text = item.valore;
                    }
                    #endregion OGGETTO
                    #region NUM_OGGETTO
                    else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_OGGETTO.ToString())
                    {
                        this.TxtCodeObject.Text = item.valore;
                    }
                    #endregion NUM_OGGETTO
                    #region MITT_DEST
                    else if (item.argomento == DocsPaWR.FiltriDocumento.MITT_DEST.ToString() || item.argomento == DocsPaWR.FiltriDocumento.ID_DESCR_DESTINATARIO.ToString())
                    {
                        TxtRecipientDescription.Text = item.valore;
                    }
                    #endregion MITT_DEST
                    #region ID_MITT_DEST
                    else if (item.argomento == DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString() || item.argomento == DocsPaWR.FiltriDocumento.ID_DESTINATARIO.ToString())
                    {
                        DocsPaWR.Corrispondente corr = AddressBookManager.GetCorrespondentBySystemId(item.valore);
                        TxtRecipientCode.Text = corr.codiceRubrica;
                        TxtRecipientDescription.Text = corr.descrizione;
                    }
                    #endregion ID_MITT_DEST
                    #region PROFILED DOCUMENT
                    // Ripristino filtro "Tipologia fascicoli"
                    else if (item.argomento == DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString())
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

                        //this.PnlSearchDocTipology.Attributes.Remove("class");
                        //this.PnlSearchDocTipology.Attributes.Add("class", "collapse shown");
                        this.UpPnlTypeDocument.Update();
                    }

                    // Ripristino filtro "Tipologia fascicoli"
                    else if (item.argomento == DocsPaWR.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString())
                    {
                        this.Template = item.template;
                        Session["templateRicerca"] = this.Template;
                        this.UpPnlTypeDocument.Update();

                    }
                    #endregion filtro PROFILED DOCUMENT
                    #region FIRMATI
                    else if (item.argomento == DocsPaWR.FiltriDocumento.FIRMATO.ToString())
                    {
                        switch (item.valore)
                        {
                            case "0":
                                this.cb_firmato.Checked = false;
                                this.cb_nonFirmato.Checked = true;
                                break;
                            case "1":
                                this.cb_firmato.Checked = true;
                                this.cb_nonFirmato.Checked = false;
                                break;
                            case "2":
                                this.cb_firmato.Checked = true;
                                this.cb_nonFirmato.Checked = true;
                                break;
                        }
                    }
                    #endregion

                    #region TIPO FILE ACQUISITO
                    else if (item.argomento == DocsPaWR.FiltriDocumento.TIPO_FILE_ACQUISITO.ToString())
                    {
                        this.ddl_tipoFileAcquisiti.SelectedValue = item.valore;
                    }
                    #endregion

                }
            }
            catch (Exception)
            {
                throw new Exception("I criteri di ricerca non sono piu\' validi.");
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

        #region componenti
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
        #endregion
    }
}