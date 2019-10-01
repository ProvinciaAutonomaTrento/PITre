using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System.Xml.Linq;

using NttDatalLibrary;
using NttDataWA.Utils;
using log4net;

namespace NttDataWA.Home
{
    public partial class LibroFirma : System.Web.UI.Page
    {
        private ILog logger = LogManager.GetLogger(typeof(CheckPage));
        #region Properties
        private List<string> TipiFirma_People;
        private string cha_TipoFirma_People;

        private long totalFileSize
        {
            get
            {
                if (HttpContext.Current.Session["TotalFileSize"] != null)
                    return (long)HttpContext.Current.Session["TotalFileSize"];
                else
                    return 0;
            }
            set
            {
                HttpContext.Current.Session["TotalFileSize"] = value;
            }
        }

        private long maxFileSizeSelectable
        {
            get
            {
                if (HttpContext.Current.Session["MaxFileSizeSelectable"] != null)
                    return (long)HttpContext.Current.Session["MaxFileSizeSelectable"];
                else
                    return 0;
            }
            set
            {
                HttpContext.Current.Session["MaxFileSizeSelectable"] = value;
            }
        }

        private List<ElementoInLibroFirma> ListaElementiLibroFirma
        {
            get
            {
                if (HttpContext.Current.Session["ListaElementiLibroFirma"] != null)
                    return (List<ElementoInLibroFirma>)HttpContext.Current.Session["ListaElementiLibroFirma"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ListaElementiLibroFirma"] = value;
            }
        }

        private List<ElementoInLibroFirma> ListaElementiFiltrati
        {
            get
            {
                List<ElementoInLibroFirma> result = null;
                if (HttpContext.Current.Session["ListaElementiFiltrati"] != null)
                {
                    result = HttpContext.Current.Session["ListaElementiFiltrati"] as List<ElementoInLibroFirma>;
                }
                return result;
            }

            set
            {
                HttpContext.Current.Session["ListaElementiFiltrati"] = value;
            }
        }


        private List<ElementoInLibroFirma> ListaElementiSelezionati
        {
            get
            {
                List<ElementoInLibroFirma> result = null;
                if (HttpContext.Current.Session["ListaElementiSelezionati"] != null)
                {
                    result = HttpContext.Current.Session["ListaElementiSelezionati"] as List<ElementoInLibroFirma>;
                }
                return result;
            }

            set
            {
                HttpContext.Current.Session["ListaElementiSelezionati"] = value;
            }
        }

        private bool CheckAll
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["checkAllElements"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["checkAllElements"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["checkAllElements"] = value;
            }
        }

        private string SelectedRow
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["selectedRowElement"] != null)
                {
                    result = HttpContext.Current.Session["selectedRowElement"] as String;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["selectedRowElement"] = value;
            }
        }

        private List<FiltroElementiLibroFirma> FiltersElement
        {
            get
            {
                return (List<FiltroElementiLibroFirma>)HttpContext.Current.Session["FiltersElement"];
            }
            set
            {
                HttpContext.Current.Session["FiltersElement"] = value;
            }
        }

        private bool VisibleColumnRecipient
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["VisibleColumnRecipient"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["VisibleColumnRecipient"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["VisibleColumnRecipient"] = value;
            }
        }


        private bool VisibleColumnTypology
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["VisibleColumnTypology"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["VisibleColumnTypology"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["VisibleColumnTypology"] = value;
            }
        }

        private int MaxSelLibroFirma
        {
            get
            {
                int result = 100;
                if (HttpContext.Current.Session["MaxSelLibroFirma"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["MaxSelLibroFirma"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["MaxSelLibroFirma"] = value;
            }
        }

        private bool AddElementiInADL
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["AddElementiInADL"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["AddElementiInADL"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["AddElementiInADL"] = value;
            }
        }
        #endregion

        #region Const

        private const string CLOSE_ESAMINA_UNO_A_UNO = "CLOSE_ESAMINA_UNO_A_UNO";
        private const string CLOSE_POPUP_SIGNATURE_SELECTED_ITEMS = "closePopupSignatureSelectedItems";
        private const string UP_PNL_BUTTONS = "upPnlButtons";
        private const string CLOSE_POPUP_OBJECT = "closePopupObject";
        private const string CLOSE_POPUP_ADDRESS_BOOK = "closePopupAddressBook";
        private const string AUTOMATICA = "A";
        private const string MANUALE = "M";
        private const string ESAMINA_TUTTI = "t";
        private const string ESAMINA_SINGOLO = "s";

        #endregion

        #region Standard Method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.InitializeLanguage();
                this.LoadKeys();
                this.ClearSession();
                this.InitializePage();
            }
            else
            {
                ReadValueFromPopup();
            }
            RefreshScript();
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void ReadValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.HiddenSelectForSignature.Value))
            {
                this.SelezionaPerFirmaORespingi(TipoStatoElemento.DA_FIRMARE);
                this.HiddenSelectForSignature.Value = string.Empty;
                return;
            }
            if (!string.IsNullOrEmpty(this.HiddenSelectForReject.Value))
            {
                this.SelezionaPerFirmaORespingi(TipoStatoElemento.DA_RESPINGERE);
                this.HiddenSelectForReject.Value = string.Empty;
                return;
            }
            if (!string.IsNullOrEmpty(this.HiddenRemoveSelect.Value))
            {
                this.RimuoviSelezione();
                this.HiddenRemoveSelect.Value = string.Empty;
                return;
            }

            if (!string.IsNullOrEmpty(this.HiddenRespingiSelezionati.Value))
            {
                this.HiddenRespingiSelezionati.Value = string.Empty;
                this.SelectedRow = "-1";
                RespingiSelezionati();
                this.HomeTabs.RefreshLayoutTab();
                return;
            }

            if (!string.IsNullOrEmpty(this.AddFilterLibroFirma.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AddFilterLibroFirma','');", true);
                this.LibroFirmaImgRemoveFilter.ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaImgRemoveFilterTooltip", UserManager.GetUserLanguage()).Replace("@@", this.ListaElementiFiltrati.Count.ToString());
                this.LibroFirmaImgRemoveFilter.AlternateText = Utils.Languages.GetLabelFromCode("LibroFirmaImgRemoveFilterTooltip", UserManager.GetUserLanguage()).Replace("@@", this.ListaElementiFiltrati.Count.ToString());
                OrderListElement();
                this.LibroFirmaImgRemoveFilter.Enabled = true;
                this.SelectedRow = null;
                this.GridViewResult_Bind();
                UpdateLabelStateElements();
                this.UpnlGrid.Update();
                this.UpnlButtonTop.Update();
                return;
            }

            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_PNL_BUTTONS))
            {
                if (this.Request.Form["__EVENTARGUMENT"] != null && this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_ESAMINA_UNO_A_UNO))
                {
                    this.GridViewResult_Bind();
                    this.UpnlGrid.Update();
                    return;
                }
                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_OBJECT)))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_AddFilterLibroFirma').contentWindow.closeObjectPopup();", true);
                    return;
                }

                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_ADDRESS_BOOK)))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_AddFilterLibroFirma').contentWindow.closeAddressBookPopup();", true);
                    return;
                }
                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_SIGNATURE_SELECTED_ITEMS)))
                {
                    HttpContext.Current.Session.Remove("AddElementiInADL");
                    this.SelectedRow = "-1";
                    LoadElementiLbroFirma();
                    this.ApplyFilters();
                    this.GridViewResult_Bind();
                    this.HomeTabs.RefreshLayoutTab();
                    return;
                }
                else
                {
                    this.SelectedRow = this.grid_rowindex.Value;
                    //if(!string.IsNullOrEmpty(this.SelectedRow))
                    //    this.gridViewResult.SelectRow(Convert.ToInt32(this.SelectedRow));
                    this.HighlightSelectedRow();
                    this.UpnlGrid.Update();
                }
            }
        }

        protected void LoadKeys()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA_COLUMN_RECIPIENT.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA_COLUMN_RECIPIENT.ToString()).Equals("1"))
            {
                this.VisibleColumnRecipient = true;
            }
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA_COLUMN_TYPOLOGY.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA_COLUMN_TYPOLOGY.ToString()).Equals("1"))
            {
                this.VisibleColumnTypology = true;
            }
        }

        private void InitializePage()
        {
            LoadRolesUser();
            LoadElementiLbroFirma();
            string language = UIManager.UserManager.GetUserLanguage();
            string firmaLocale = Utils.Languages.GetLabelFromCode("LibroFirmaTipoFirmaLocale", language);
            string firmaHSM = Utils.Languages.GetLabelFromCode("LibroFirmaTipoFirmaHSM", language);

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_MAX_SEL_LF.ToString())))
                this.MaxSelLibroFirma = int.Parse(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_MAX_SEL_LF.ToString()));
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_MAX_SEL_SIZE_LF.ToString())))
                this.maxFileSizeSelectable = long.Parse(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_MAX_SEL_SIZE_LF.ToString()));

            if (UserManager.ruoloIsAutorized(this, "DO_DOC_FIRMA"))
            {
                TipiFirma_People = (UserManager.ruoloIsAutorized(this, "FIRMA_HSM") ? new List<string>() { firmaLocale, firmaHSM } : new List<string>() { firmaLocale });
                PopulateTipoFirmaDigitale_People();
                this.TipoFirmaDigitale_People.Visible = true;
            }
            else
            {
                UpdateTipoFirma(string.Empty);
                this.TipoFirmaDigitale_People.Visible = false;
            }

            InitTotalFileSize();

            if (this.Request.QueryString["back"] != null && this.Request.QueryString["back"].Equals("1"))
            {
                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject obj = navigationList.Last();

                if (!string.IsNullOrEmpty(obj.NumPage))
                {
                    this.gridViewResult.PageIndex = Int32.Parse(obj.NumPage);
                }


                this.GridViewResult_Bind();

                if (!string.IsNullOrEmpty(obj.OriginalObjectId))
                {
                    string idElement = string.Empty;
                    foreach (GridViewRow grd in this.gridViewResult.Rows)
                    {
                        idElement = string.Empty;

                        if (grd.FindControl("systemIdElemento") != null)
                        {
                            idElement = (grd.FindControl("systemIdElemento") as Label).Text.ToString();
                        }

                        if (idElement.Equals(obj.OriginalObjectId))
                        {
                            //this.gridViewResult.SelectRow(grd.RowIndex);
                            this.SelectedRow = grd.RowIndex.ToString();
                        }
                    }
                }
                this.HighlightSelectedRow();
            }
            else
            {
                GridViewResult_Bind();
            }
        }

        private void InitTotalFileSize()
        {
            List<ElementoInLibroFirma> elementsToSign = (from e in this.ListaElementiLibroFirma
                                                         where e.StatoFirma.Equals(TipoStatoElemento.DA_FIRMARE)
                                                         select e).ToList();

            if (elementsToSign.Count > 0)
            {
                totalFileSize = elementsToSign.Select(c => c.FileSize).Sum();
            }
            else
            {
                totalFileSize = 0;
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.MassiveReport.Title = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.headerHomeLblRole.Text = Utils.Languages.GetLabelFromCode("HeaderHomeLblRole", language);
            this.LibroFirmaFirmaSelezionati.Text = Utils.Languages.GetLabelFromCode("LibroFirmaFirmaSelezionati", language);
            this.LibroFirmaRespingiSelezionati.Text = Utils.Languages.GetLabelFromCode("LibroFirmaRespingiSelezionati", language);
            this.LibroFirmaEliminaSelezionati.Text = Utils.Languages.GetLabelFromCode("LibroFirmaEliminaSelezionati", language);
            this.LibroFirmaFirmaSelezionati.ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaFirmaSelezionatiToolTip", language);
            this.LibroFirmaRespingiSelezionati.ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaRespingiSelezionatiToolTip", language);
            this.LibroFirmaEliminaSelezionati.ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaEliminaSelezionatiToolTip", language);
            this.LibroFirmaImgAddFilter.ToolTip = Utils.Languages.GetLabelFromCode("IndexImgAddFilterTooltip", language);
            this.LibroFirmaImgAddFilter.AlternateText = Utils.Languages.GetLabelFromCode("IndexImgAddFilterTooltip", language);
            this.LibroFirmaImgEsamina.ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaImgExamines", language);
            this.LibroFirmaImgEsamina.ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaImgExamines", language);
            this.SelezionaPerFirma.ToolTip = Utils.Languages.GetLabelFromCode("SelezionaPerFirma", language);
            this.SelezionaPerRespingi.ToolTip = Utils.Languages.GetLabelFromCode("SelezionaPerRespingi", language);
            this.Deseleziona.ToolTip = Utils.Languages.GetLabelFromCode("RimuoviSelezione", language);
            this.AddFilterLibroFirma.Title = Utils.Languages.GetLabelFromCode("AddFilterLibroFirmaTitle", language);
            this.EsaminaLibroFirma.Title = Utils.Languages.GetLabelFromCode("EsaminaLibroFirma", language);
            this.EsaminaLibroFirmaSingolo.Title = Utils.Languages.GetLabelFromCode("EsaminaLibroFirmaSingolo", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddFilterAddressBookTitle", language);
            this.Object.Title = Utils.Languages.GetLabelFromCode("AddFilterObjectTitle", language);
            this.SignatureSelectedItems.Title = Utils.Languages.GetLabelFromCode("SignatureSelectedItemsTitle", language);
            this.DetailsLFAutomaticMode.Title = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeTitle", language);
            this.TipoFirmaDigitale_People.ToolTip = Utils.Languages.GetLabelFromCode("ToolTipPreferenzaFirma", language);
            this.LibroFirmaImgRefresh.ToolTip = Utils.Languages.GetLabelFromCode("IndexImgRefreshTooltip", language);
            this.LibroFirmaFirmaSelezionatiAdl.Text = Utils.Languages.GetLabelFromCode("LibroFirmaFirmaSelezionatiAdl", language);
            this.LibroFirmaFirmaSelezionatiAdl.ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaFirmaSelezionatiAdlTooltip", language);
        }

        private void ClearSession()
        {
            HttpContext.Current.Session.Remove("ListaElementiLibroFirma");
            HttpContext.Current.Session.Remove("ListSelectedElements");
            HttpContext.Current.Session.Remove("checkAllElements");
            HttpContext.Current.Session.Remove("selectedRowElement");
            HttpContext.Current.Session.Remove("selectedPageElement");
            HttpContext.Current.Session.Remove("ListaElementiFiltrati");
            HttpContext.Current.Session.Remove("ElementiSelezionati");
            HttpContext.Current.Session.Remove("FiltersElement");
            HttpContext.Current.Session.Remove("VisibleColumnTypology");
            HttpContext.Current.Session.Remove("TotalFileSize");

            this.cha_TipoFirma_People = null;
        }

        private void EnableButtonTop()
        {
            if (this.ListaElementiFiltrati != null && this.ListaElementiFiltrati.Count > 0)
            {
                this.SelezionaPerFirma.Enabled = true;
                this.SelezionaPerRespingi.Enabled = true;
                this.Deseleziona.Enabled = true;
                this.LibroFirmaImgEsamina.Enabled = true;
            }
            else
            {
                this.SelezionaPerFirma.Enabled = false;
                this.SelezionaPerRespingi.Enabled = false;
                this.Deseleziona.Enabled = false;
                this.LibroFirmaImgEsamina.Enabled = false;
            }
            this.UpnlButtonTop.Update();
        }
        #endregion

        #region GridView

        private void GridViewResult_Bind()
        {
            try
            {
                this.gridViewResult.DataSource = ListaElementiFiltrati;
                logger.Info("AGGIORNAMENTO STATO - AGGIORNAMENTO DELLA GRIGLIA COUNT " + ListaElementiFiltrati.Count());
                this.gridViewResult.DataBind();
                this.HighlightSelectedRow();
                this.UpdateLabelStateElements();
                this.EnableButtonTop();
                this.UpnlGrid.Update();
            }
            catch (Exception e)
            {
                logger.Error("Errore in fase di creazione della griglia: " + e.Message);
            }
        }

        protected void gridViewResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (gridViewResult.Columns.Count > 0)
                    foreach (DataControlField td in gridViewResult.Columns)
                    {
                        if (td.HeaderText.Equals(Utils.Languages.GetLabelFromCode("LibroFirmaTipologia", UIManager.UserManager.GetUserLanguage())))
                            td.Visible = this.VisibleColumnTypology;

                        if (td.HeaderText.Equals(Utils.Languages.GetLabelFromCode("LibroFirmaDestinatario", UIManager.UserManager.GetUserLanguage())))
                            td.Visible = this.VisibleColumnRecipient;

                        if (td.HeaderText.Equals(Utils.Languages.GetLabelFromCode("EsaminaUnoAUnoSingoloElemento", UIManager.UserManager.GetUserLanguage())))
                            td.ShowHeader = false;
                    }

                ElementoInLibroFirma elemento = e.Row.DataItem as ElementoInLibroFirma;

                e.Row.CssClass += " NormalRow";
                if (!string.IsNullOrEmpty(elemento.InfoDocumento.IdDocumentoPrincipale))
                {
                    ElementoInLibroFirma docPrincipale = (from el in this.ListaElementiFiltrati where el.InfoDocumento.Docnumber.Equals(elemento.InfoDocumento.IdDocumentoPrincipale) select el).FirstOrDefault();
                    if (docPrincipale != null)
                        e.Row.CssClass += " Borderrow";
                    (e.Row.FindControl("BtnDocument") as CustomImageButton).ImageUrl = "../Images/Icons/ico_user_attachment.png";
                    (e.Row.FindControl("BtnDocument") as CustomImageButton).ImageUrlDisabled = "../Images/Icons/ico_user_attachment_disabled.png";
                    (e.Row.FindControl("BtnDocument") as CustomImageButton).OnMouseOutImage = "../Images/Icons/ico_user_attachment.png";
                    (e.Row.FindControl("BtnDocument") as CustomImageButton).OnMouseOverImage = "../Images/Icons/ico_user_attachment_hover.png";
                    (e.Row.FindControl("BtnDocument") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaBtnDocumentAttTooltip", UserManager.GetUserLanguage()).Replace("@@", elemento.InfoDocumento.Docnumber);
                }
                else
                {
                    (e.Row.FindControl("BtnDocument") as CustomImageButton).ImageUrl = "../Images/Icons/ico_previous_details.png";
                    (e.Row.FindControl("BtnDocument") as CustomImageButton).ImageUrlDisabled = "../Images/Icons/ico_previous_details_disabled.png";
                    (e.Row.FindControl("BtnDocument") as CustomImageButton).OnMouseOutImage = "../Images/Icons/ico_previous_details.png";
                    (e.Row.FindControl("BtnDocument") as CustomImageButton).OnMouseOverImage = "../Images/Icons/ico_previous_details_hover.png";
                    (e.Row.FindControl("BtnDocument") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaBtnDocumentDocTooltip", UserManager.GetUserLanguage()).Replace("@@", elemento.InfoDocumento.Docnumber);
                }
                switch (elemento.StatoFirma)
                {
                    case TipoStatoElemento.PROPOSTO:
                        (e.Row.FindControl("BtnStato") as CustomImageButton).ImageUrl = "../Images/Icons/LibroFirma/Sel_None.png";
                        (e.Row.FindControl("BtnStato") as CustomImageButton).ImageUrlDisabled = "../Images/Icons/LibroFirma/Sel_None_disabled.png";
                        (e.Row.FindControl("BtnStato") as CustomImageButton).OnMouseOutImage = "../Images/Icons/LibroFirma/Sel_None.png";
                        (e.Row.FindControl("BtnStato") as CustomImageButton).OnMouseOverImage = "../Images/Icons/LibroFirma/Sel_None_hover.png";
                        (e.Row.FindControl("BtnStato") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaProposto", UserManager.GetUserLanguage());

                        break;
                    case TipoStatoElemento.DA_FIRMARE:
                        (e.Row.FindControl("BtnStato") as CustomImageButton).ImageUrl = "../Images/Icons/LibroFirma/Sel_Firma.png";
                        (e.Row.FindControl("BtnStato") as CustomImageButton).ImageUrlDisabled = "../Images/Icons/LibroFirma/Sel_Firma_disabled.png";
                        (e.Row.FindControl("BtnStato") as CustomImageButton).OnMouseOutImage = "../Images/Icons/LibroFirma/Sel_Firma.png";
                        (e.Row.FindControl("BtnStato") as CustomImageButton).OnMouseOverImage = "../Images/Icons/LibroFirma/Sel_Firma_hover.png";
                        (e.Row.FindControl("BtnStato") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaToSisgn", UserManager.GetUserLanguage());
                        break;
                    case TipoStatoElemento.DA_RESPINGERE:
                        (e.Row.FindControl("BtnStato") as CustomImageButton).ImageUrl = "../Images/Icons/LibroFirma/Sel_Rifiuta.png";
                        (e.Row.FindControl("BtnStato") as CustomImageButton).ImageUrlDisabled = "../Images/Icons/LibroFirma/Sel_Rifiuta_disabled.png";
                        (e.Row.FindControl("BtnStato") as CustomImageButton).OnMouseOutImage = "../Images/Icons/LibroFirma/Sel_Rifiuta.png";
                        (e.Row.FindControl("BtnStato") as CustomImageButton).OnMouseOverImage = "../Images/Icons/LibroFirma/Sel_Rifiuta_hover.png";
                        (e.Row.FindControl("BtnStato") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaToReject", UserManager.GetUserLanguage());
                        break;
                }
                for (int i = 2; i < e.Row.Cells.Count - 3; i++)
                {
                    e.Row.Cells[i].Attributes.Add("onclick", "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "');__doPostBack('upPnlButtons');return false;");
                }
                (e.Row.FindControl("iconTypeSignature") as Image).ImageUrl = LibroFirmaManager.GetIconTypeSignature(elemento);
                (e.Row.FindControl("iconTypeSignature") as Image).ToolTip = LibroFirmaManager.GetLabelTypeSignature(elemento.TipoFirma);
                if (elemento.Modalita.Equals(AUTOMATICA))
                {
                    (e.Row.FindControl("imgModalita") as CustomImageButton).ImageUrl = "../Images/Icons/LibroFirma/Start_Process.png";
                    (e.Row.FindControl("imgModalita") as CustomImageButton).ImageUrlDisabled = "../Images/Icons/LibroFirma/Start_Process_disabled.png";
                    (e.Row.FindControl("imgModalita") as CustomImageButton).OnMouseOutImage = "../Images/Icons/LibroFirma/Start_Process.png";
                    (e.Row.FindControl("imgModalita") as CustomImageButton).OnMouseOverImage = "../Images/Icons/LibroFirma/Start_Process_hover.png";
                    (e.Row.FindControl("imgModalita") as CustomImageButton).CommandName = "ViewDetailsAutomaticMode";
                    (e.Row.FindControl("imgModalita") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaImgAutomaticModeTooltip", UserManager.GetUserLanguage());
                }
                else if (elemento.Modalita.Equals(MANUALE))
                {
                    (e.Row.FindControl("imgModalita") as CustomImageButton).ImageUrl = "../Images/Icons/LibroFirma/Manual_Process.png";
                    (e.Row.FindControl("imgModalita") as CustomImageButton).ImageUrlDisabled = "../Images/Icons/LibroFirma/Manual_Process.png";
                    (e.Row.FindControl("imgModalita") as CustomImageButton).OnMouseOutImage = "../Images/Icons/LibroFirma/Manual_Process.png";
                    (e.Row.FindControl("imgModalita") as CustomImageButton).OnMouseOverImage = "../Images/Icons/LibroFirma/Manual_Process.png";
                    (e.Row.FindControl("imgModalita") as CustomImageButton).Attributes.Add("onclick", "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "');__doPostBack('upPnlButtons');return false;");
                    //(e.Row.FindControl("imgModalita") as CustomImageButton).Enabled = false;
                    (e.Row.FindControl("imgModalita") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaImgManualModeTooltip", UserManager.GetUserLanguage());
                }
                (e.Row.FindControl("EsaminaUnoAUnoSingoloElemento") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaEsaminaSingoloTooltip", UserManager.GetUserLanguage());
                if (string.IsNullOrEmpty(elemento.ErroreFirma))
                {
                    (e.Row.FindControl("errorSign") as Control).Visible = false;
                }
                else
                {
                    (e.Row.FindControl("errorSign") as Control).Visible = true;
                }
            }
        }

        protected void gridViewResult_PreRender(object sender, EventArgs e)
        {

        }

        protected void gridViewResult_ItemCreated(Object sender, GridViewRowEventArgs e)
        {

        }

        protected void gridViewResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.gridViewResult.PageIndex = e.NewPageIndex;
                this.SelectedRow = "-1";
                GridViewResult_Bind();
                this.UpnlGrid.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void gridViewResult_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "viewLinkObject")
            {
                //int rowIndex = Convert.ToInt32(e.CommandArgument);
                string idProfile = (((e.CommandSource as Control).Parent.Parent as GridViewRow).FindControl("idProfile") as Label).Text;
                string idElement = (((e.CommandSource as Control).Parent.Parent as GridViewRow).FindControl("systemIdElemento") as Label).Text;
                SchedaDocumento schedaDocumento = UIManager.DocumentManager.getDocumentDetails(this, idProfile, idProfile);
                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                string language = UIManager.UserManager.GetUserLanguage();

                #region navigation
                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                actualPage.IdObject = idProfile;
                actualPage.OriginalObjectId = idElement;
                actualPage.NumPage = this.gridViewResult.PageIndex.ToString();
                actualPage.PageSize = this.gridViewResult.PageSize.ToString();

                actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_LIBRO_FIRMA.ToString(), string.Empty);
                actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME_LIBRO_FIRMA.ToString(), true, this.Page);
                actualPage.CodePage = Navigation.NavigationUtils.NamePage.HOME_LIBRO_FIRMA.ToString();

                actualPage.Page = "LIBROFIRMA.ASPX";
                navigationList.Add(actualPage);
                Navigation.NavigationUtils.SetNavigationList(navigationList);
                #endregion

                UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                Response.Redirect("~/Document/Document.aspx");
            }

            if (e.CommandName == "ViewDetailsAutomaticMode")
            {
                GridViewRow row = ((e.CommandSource as Control).Parent.Parent as GridViewRow);
                string idItem = (row.FindControl("systemIdElemento") as Label).Text;
                ElementoInLibroFirma elemento = (from el in this.ListaElementiFiltrati where el.IdElemento.Equals(idItem) select el).FirstOrDefault();
                //this.gridViewResult.SelectRow(row.RowIndex);
                this.grid_rowindex.Value = row.RowIndex.ToString();
                this.SelectedRow = row.RowIndex.ToString();
                this.HighlightSelectedRow();
                this.ListaElementiSelezionati = new List<ElementoInLibroFirma>() { elemento };
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDetailsLFAutomaticMode", "ajaxModalPopupDetailsLFAutomaticMode();", true);
                this.UpnlGrid.Update();
                return;
            }

        }

        protected void HighlightSelectedRow()
        {

            if (this.gridViewResult.Rows.Count > 0 && !this.SelectedRow.Equals("-1") && !string.IsNullOrEmpty(this.SelectedRow))
            {
                this.gridViewResult.SelectRow(Convert.ToInt32(this.SelectedRow));
                GridViewRow gvRow = this.gridViewResult.SelectedRow;
                foreach (GridViewRow GVR in this.gridViewResult.Rows)
                {
                    if (GVR == gvRow)
                    {
                        GVR.CssClass += " selectedrow";
                    }
                    else
                    {
                        GVR.CssClass = GVR.CssClass.Replace(" selectedrow", "");
                    }
                }
            }
        }

        protected string GetProponente(ElementoInLibroFirma elemento)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            string del = Utils.Languages.GetLabelFromCode("TransmissionDelegatedBy", language).ToUpper();

            if (elemento.UtenteProponente != null && !string.IsNullOrEmpty(elemento.UtenteProponente.idPeople))
            {
                if (!string.IsNullOrEmpty(elemento.DescProponenteDelegato))
                {
                    return elemento.DescProponenteDelegato + " (" + elemento.RuoloProponente.descrizione + ") " + del + " " + elemento.UtenteProponente.descrizione;
                }
                else
                {
                    return elemento.UtenteProponente.descrizione + " (" + elemento.RuoloProponente.descrizione + ")";
                }
            }
            else
            {
                return elemento.UtenteProponente + " (" + elemento.RuoloProponente + ")";
            }
        }

        /// <summary>
        /// Estrae l'oggetto del documento. Se il documento è documento principale, restituisce l'oggetto dello stesso,
        /// altrimenti se di tipo allegato, se tra gli elementi è presente il doc principale restituisce oggDoc - oggAll, altrimenti oggAll
        /// </summary>
        /// <param name="elemento"></param>
        /// <returns></returns>
        protected string GetObject(ElementoInLibroFirma elemento)
        {
            string retValue = string.Empty;

            if (string.IsNullOrEmpty(elemento.InfoDocumento.IdDocumentoPrincipale))
            {
                retValue = elemento.InfoDocumento.Oggetto;
            }
            else
            {
                ElementoInLibroFirma elementMainDoc = (from e in this.ListaElementiFiltrati
                                                       where e.InfoDocumento.Docnumber.Equals(elemento.InfoDocumento.IdDocumentoPrincipale)
                                                       select e).FirstOrDefault();
                if (elementMainDoc == null)
                {
                    retValue = elemento.InfoDocumento.OggettoDocumentoPrincipale + " - " + elemento.InfoDocumento.Oggetto;
                }
                else
                {
                    retValue = elemento.InfoDocumento.Oggetto;
                }
            }
            return retValue;
        }

        protected string GetFile(ElementoInLibroFirma elemento)
        {
            if (!string.IsNullOrEmpty(elemento.IdElemento) && elemento.InfoDocumento != null)
            {
                DocsPaWR.InfoDocLibroFirma doc = elemento.InfoDocumento;
                string file = "V" + doc.NumVersione + " ";
                file += string.IsNullOrEmpty(doc.IdDocumentoPrincipale) ? "" : "A" + doc.NumAllegato;
                return file;
            }
            else
            {
                return string.Empty;
            }
        }

        protected string GetDataInserimento(ElementoInLibroFirma elemento)
        {
            return Utils.dateformat.ConvertToDate(elemento.DataInserimento).ToString("dd/MM/yyyy");
        }

        protected string GetErrorSign(ElementoInLibroFirma elemento)
        {
            return elemento.ErroreFirma.ToString();
        }

        protected string GetDocnumber(ElementoInLibroFirma elemento)
        {
            string docnumber = string.Empty;
            string documentDescriptionColor = string.Empty;
            //// Individuazione del colore da assegnare alla descrizione del documento
            //switch (new DocsPaWebService().getSegnAmm(UIManager.UserManager.GetInfoUser().idAmministrazione))
            //{
            //    case "0":
            //        documentDescriptionColor = "Black";
            //        break;
            //    case "1":
            //        documentDescriptionColor = "Blue";
            //        break;
            //    default:
            //        documentDescriptionColor = "Red";
            //        break;
            //}
            if (!string.IsNullOrEmpty(elemento.IdElemento) && elemento.InfoDocumento != null)
            {
                DocsPaWR.InfoDocLibroFirma doc = elemento.InfoDocumento;

                docnumber += "<span style=\"color:";
                // Se il documento è un protocollo viene colorato in rosso altrimenti
                // viene colorato in nero
                docnumber += (String.IsNullOrEmpty(doc.NumProto) ? "Black" : "Red");
                // Il testo deve essere grassetto
                docnumber += "; font-weight:bold;\">";

                // Creazione dell'informazione sul documento
                if (!String.IsNullOrEmpty(doc.NumProto))
                    docnumber += (doc.NumProto + "<br />" + Utils.dateformat.ConvertToDate(doc.DataProtocollo).ToString("dd/MM/yyyy"));
                else
                    docnumber += (doc.Docnumber + "<br />" + Utils.dateformat.ConvertToDate(doc.DataCreazione).ToString("dd/MM/yyyy"));

                // Chiusura del tag span
                docnumber += "</span>";
            }
            return docnumber;
        }
        #endregion

        #region Event button

        protected void LibroFirmaImgRefresh_OnClick(object sender, EventArgs e)
        {
            try
            {
                LoadElementiLbroFirma();
                LibroFirmaImgRemoveFilter_Click(null, null);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TipoFirmaDigitale_People_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Se l'utente esercita in delega non memorizzo il tipo di firma scelto
            InfoUtente infoUtente = UserManager.GetInfoUser();
            if (!string.IsNullOrEmpty(TipoFirmaDigitale_People.SelectedValue.ToString()))
            {
                UpdateTipoFirma(TipoFirmaDigitale_People.SelectedValue.ToString().ToUpper());
                if (infoUtente.delegato == null || string.IsNullOrEmpty(infoUtente.delegato.idPeople))
                    UIManager.UserManager.SetSignTypePreference(UIManager.UserManager.GetUserInSession().idPeople, cha_TipoFirma_People);
            }
        }

        protected void LibroFirmaRespingiSelezionati_Click(object sender, EventArgs e)
        {
            List<ElementoInLibroFirma> elementsToReject = (from i in this.ListaElementiLibroFirma
                                                           where i.StatoFirma.Equals(TipoStatoElemento.DA_RESPINGERE)
                                                           select i).ToList();
            if (elementsToReject != null && elementsToReject.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmLibroFirmaRespingiSelezionati', 'HiddenRespingiSelezionati', '','');", true);
                return;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningNoElementsToRejectSelected', 'warning', '', '');} else {parent.ajaxDialogModal('WarningNoElementsToRejectSelected', 'warning', '', '');}", true);
            }
        }

        protected void LibroFirmaFirmaSelezionatiAdl_Click(object sender, EventArgs e)
        {
            this.AddElementiInADL = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupSignatureSelectedItems", "ajaxModalPopupSignatureSelectedItems();", true);
        }

        private void RespingiSelezionati()
        {

            string msg = string.Empty;
            try
            {
                List<ElementoInLibroFirma> elementsToReject = (from i in this.ListaElementiLibroFirma
                                                               where i.StatoFirma.Equals(TipoStatoElemento.DA_RESPINGERE)
                                                               select i).ToList();
                if (elementsToReject != null && elementsToReject.Count > 0)
                {
                    LibroFirmaManager.RejectElementsSignatureProcess(elementsToReject);

                    this.ListaElementiLibroFirma = (from i in ListaElementiLibroFirma
                                                    where !i.StatoFirma.Equals(TipoStatoElemento.DA_RESPINGERE)
                                                    select i).ToList();
                    this.ListaElementiFiltrati = (from i in ListaElementiFiltrati
                                                  where !i.StatoFirma.Equals(TipoStatoElemento.DA_RESPINGERE)
                                                  select i).ToList();
                    this.GridViewResult_Bind();

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('SuccessLibroFirmaRespingiSelezionati', 'check', '');} else {parent.ajaxDialogModal('SuccessLibroFirmaRespingiSelezionati', 'check', '');}", true);
                    return;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningNoElementsToRejectSelected', 'warning', '', '');} else {parent.ajaxDialogModal('WarningNoElementsToRejectSelected', 'warning', '', '');}", true);
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void LibroFirmaImgRemoveFilter_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Session.Remove("FiltersElement");
            this.ListaElementiFiltrati.Clear();
            this.ListaElementiFiltrati.AddRange(this.ListaElementiLibroFirma);
            this.SelectedRow = null;
            GridViewResult_Bind();
            this.LibroFirmaImgRemoveFilter.Enabled = false;
            this.UpdateLabelStateElements();
            this.UpnlButtonTop.Update();
        }

        protected void BtnStato_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            string message = string.Empty;
            string msg = string.Empty;
            CustomImageButton btnIm = (CustomImageButton)sender;
            GridViewRow row = (GridViewRow)btnIm.Parent.Parent;
            string idElemento = (row.FindControl("systemIdElemento") as Label).Text;
            ElementoInLibroFirma elemento = (from el in this.ListaElementiFiltrati where el.IdElemento.Equals(idElemento) select el).FirstOrDefault();
            switch (elemento.StatoFirma)
            {
                case TipoStatoElemento.PROPOSTO:
                    if ((from i in this.ListaElementiLibroFirma where i.StatoFirma != TipoStatoElemento.PROPOSTO select i).ToList().Count < this.MaxSelLibroFirma)
                    {
                        long tempTotalFileSize = totalFileSize + elemento.FileSize;

                        if (tempTotalFileSize <= maxFileSizeSelectable)
                        {
                            totalFileSize = tempTotalFileSize;

                            elemento.MotivoRespingimento = string.Empty;
                            if (LibroFirmaManager.AggiornaStatoElementi(new List<ElementoInLibroFirma>() { elemento }, TipoStatoElemento.DA_FIRMARE.ToString(), out message))
                            {
                                if (string.IsNullOrEmpty(message))
                                {
                                    (row.FindControl("BtnStato") as CustomImageButton).ImageUrl = "../Images/Icons/LibroFirma/Sel_Firma.png";
                                    (row.FindControl("BtnStato") as CustomImageButton).ImageUrlDisabled = "../Images/Icons/LibroFirma/Sel_Firma_disabled.png";
                                    (row.FindControl("BtnStato") as CustomImageButton).OnMouseOutImage = "../Images/Icons/LibroFirma/Sel_Firma.png";
                                    (row.FindControl("BtnStato") as CustomImageButton).OnMouseOverImage = "../Images/Icons/LibroFirma/Sel_Firma_hover.png";
                                    (row.FindControl("BtnStato") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaToSisgn", UserManager.GetUserLanguage());

                                    elemento.StatoFirma = TipoStatoElemento.DA_FIRMARE;
                                    if (string.IsNullOrEmpty(elemento.DataAccettazione))
                                    {
                                        elemento.DataAccettazione = Utils.dateformat.getDataOdiernaDocspa();

                                        //Aggiorno la data di accettazione anche per tutti gli allegati/doc princpale legati alla stessa trasm.
                                        ListaElementiLibroFirma.Where(i => i.IdTrasmSingola.Equals(elemento.IdTrasmSingola)).ToList().ForEach(f => f.DataAccettazione = elemento.DataAccettazione);
                                        ListaElementiLibroFirma.Where(i => i.IdTrasmSingola.Equals(elemento.IdTrasmSingola)).ToList().ForEach(f => f.StatoFirma = elemento.StatoFirma);
                                        GridViewResult_Bind();
                                    }
                                    //this.gridViewResult.SelectRow(row.RowIndex);
                                    this.SelectedRow = row.RowIndex.ToString();
                                    this.HighlightSelectedRow();
                                }
                                else
                                {
                                    //messagge contiene l'nformazione che l'elemento in libro firma è stato già preso in carico da un altro utente del ruolo
                                    msg = RemoveElementsLockedAnotherUser(message);
                                    GridViewResult_Bind();
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RemoveElementsLockedAnotherUser', 'warning', '', '" + msg + "');} else {parent.ajaxDialogModal('RemoveElementsLockedAnotherUser', 'warning', '', '" + msg + "');}", true);
                                }
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningMaxFileSizeSelectedItemInLF', 'warning', '', '" + (this.maxFileSizeSelectable / 1000000) + "');} else {parent.ajaxDialogModal('WarningMaxFileSizeSelectedItemInLF', 'warning', '', '');}", true);
                            return;
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningMaxSelectedItemInLF', 'warning', '', '" + this.MaxSelLibroFirma + "');} else {parent.ajaxDialogModal('WarningMaxSelectedItemInLF', 'warning', '', '" + this.MaxSelLibroFirma + "');}", true);
                        return;
                    }
                    break;
                case TipoStatoElemento.DA_FIRMARE:
                    if (totalFileSize > 0)
                        totalFileSize = totalFileSize - elemento.FileSize;

                    if (LibroFirmaManager.AggiornaStatoElementi(new List<ElementoInLibroFirma>() { elemento }, TipoStatoElemento.DA_RESPINGERE.ToString(), out message))
                    {
                        if (string.IsNullOrEmpty(message))
                        {
                            (row.FindControl("BtnStato") as CustomImageButton).ImageUrl = "../Images/Icons/LibroFirma/Sel_Rifiuta.png";
                            (row.FindControl("BtnStato") as CustomImageButton).ImageUrlDisabled = "../Images/Icons/LibroFirma/Sel_Firma_Rifiuta.png";
                            (row.FindControl("BtnStato") as CustomImageButton).OnMouseOutImage = "../Images/Icons/LibroFirma/Sel_Rifiuta.png";
                            (row.FindControl("BtnStato") as CustomImageButton).OnMouseOverImage = "../Images/Icons/LibroFirma/Sel_Rifiuta_hover.png";
                            (row.FindControl("BtnStato") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaToReject", UserManager.GetUserLanguage());
                            elemento.StatoFirma = TipoStatoElemento.DA_RESPINGERE;
                            //this.gridViewResult.SelectRow(row.RowIndex);
                            this.SelectedRow = row.RowIndex.ToString();
                            this.HighlightSelectedRow();
                        }
                        else
                        {
                            msg = RemoveElementsLockedAnotherUser(message);
                            GridViewResult_Bind();
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RemoveElementsLockedAnotherUser', 'warning', '', '" + msg + "');} else {parent.ajaxDialogModal('RemoveElementsLockedAnotherUser', 'warning', '', '" + msg + "');}", true);
                        }
                    }
                    break;
                case TipoStatoElemento.DA_RESPINGERE:
                    elemento.MotivoRespingimento = string.Empty;
                    if (LibroFirmaManager.AggiornaStatoElementi(new List<ElementoInLibroFirma>() { elemento }, TipoStatoElemento.PROPOSTO.ToString(), out message))
                    {
                        if (string.IsNullOrEmpty(message))
                        {
                            (row.FindControl("BtnStato") as CustomImageButton).ImageUrl = "../Images/Icons/LibroFirma/Sel_None.png";
                            (row.FindControl("BtnStato") as CustomImageButton).ImageUrlDisabled = "../Images/Icons/LibroFirma/Sel_None_disabled.png";
                            (row.FindControl("BtnStato") as CustomImageButton).OnMouseOutImage = "../Images/Icons/LibroFirma/Sel_None.png";
                            (row.FindControl("BtnStato") as CustomImageButton).OnMouseOverImage = "../Images/Icons/LibroFirma/Sel_None_hover.png";
                            (row.FindControl("BtnStato") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaProposto", UserManager.GetUserLanguage());
                            elemento.StatoFirma = TipoStatoElemento.PROPOSTO;
                            //this.gridViewResult.SelectRow(row.RowIndex);
                            this.SelectedRow = row.RowIndex.ToString();
                            this.HighlightSelectedRow();
                        }
                        else
                        {
                            msg = RemoveElementsLockedAnotherUser(message);
                            GridViewResult_Bind();
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RemoveElementsLockedAnotherUser', 'warning', '', '" + msg + "');} else {parent.ajaxDialogModal('RemoveElementsLockedAnotherUser', 'warning', '', '" + msg + "');}", true);
                        }
                    }
                    break;
            }
            this.UpdateLabelStateElements();
            this.UpnlGrid.Update();

        }

        private string RemoveElementsLockedAnotherUser(string message)
        {
            string infoElement;
            string msg = string.Empty;
            string[] elements = message.Split('#');
            List<string> idElements = new List<string>();
            foreach (string element in elements)
            {
                if (!string.IsNullOrEmpty(element))
                {
                    string[] info = element.Split('@');
                    idElements.Add(info[0]);
                    infoElement = Utils.Languages.GetLabelFromCode("LibroFirmaInfoObjectDoc", UserManager.GetUserLanguage()) + " " + info[1] + " " +
                      Utils.Languages.GetLabelFromCode("LibroFirmaInfoUtenteLocker", UserManager.GetUserLanguage()) + " " + info[2] + "";
                    msg += infoElement + "<br /><br />";
                }
            }
            this.ListaElementiFiltrati = (from e in this.ListaElementiFiltrati where !idElements.Contains(e.IdElemento) select e).ToList();
            this.ListaElementiLibroFirma = (from e in this.ListaElementiLibroFirma where !idElements.Contains(e.IdElemento) select e).ToList();
            return msg;
        }

        protected void SelezionaPerFirma_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmSelectForSignature', 'HiddenSelectForSignature', '','');", true);
            return;
        }

        protected void SelezionaPerRespingi_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmSelectForReject', 'HiddenSelectForReject', '','');", true);
            return;
        }

        protected void RimuoviSelezione_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmRemoveSelect', 'HiddenRemoveSelect', '','');", true);
            return;
        }

        protected void EsaminaUnoAUno_Click(object sender, EventArgs e)
        {
            this.ListaElementiSelezionati = this.ListaElementiFiltrati;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupEsaminaLibroFirma", "ajaxModalPopupEsaminaLibroFirma();", true);
            return;
        }

        protected void EsaminaUnoAUnoSingoloElemento_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            CustomImageButton btnIm = (CustomImageButton)sender;
            GridViewRow row = (GridViewRow)btnIm.Parent.Parent;

            string idElemento = (row.FindControl("systemIdElemento") as Label).Text;
            ElementoInLibroFirma elemento = (from el in this.ListaElementiFiltrati where el.IdElemento.Equals(idElemento) select el).FirstOrDefault();
            //this.gridViewResult.SelectRow(row.RowIndex);
            this.SelectedRow = row.RowIndex.ToString();
            this.HighlightSelectedRow();
            this.ListaElementiSelezionati = new List<ElementoInLibroFirma>() { elemento };
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupEsaminaLibroFirma", "ajaxModalPopupEsaminaLibroFirmaSingolo();", true);
            this.UpnlGrid.Update();
            return;
        }

        private void SelezionaPerFirmaORespingi(TipoStatoElemento stato)
        {
            try
            {
                //Il numero di elementi selezionati non può essere superiore a MaxSelLibroFirma
                string msg = string.Empty;
                List<String> listIdSelectedItem = (from e in this.ListaElementiFiltrati select e.IdElemento).ToList();
                int countSelectedItems = (from a in ListaElementiLibroFirma
                                          where a.StatoFirma != TipoStatoElemento.PROPOSTO && !listIdSelectedItem.Contains(a.IdElemento)
                                          select a).ToList().Count();
                //int countSelectedItems = (from i in this.ListaElementiLibroFirma where i.StatoFirma != TipoStatoElemento.PROPOSTO select i).ToList().Count;
                if ((this.MaxSelLibroFirma - countSelectedItems) == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningMaxSelectedItemInLF', 'warning', '', '" + this.MaxSelLibroFirma + "');} else {parent.ajaxDialogModal('WarningMaxSelectedItemInLF', 'warning', '', '" + this.MaxSelLibroFirma + "');}", true);
                    return;
                }

                List<ElementoInLibroFirma> elements = this.ListaElementiFiltrati.Select(x => x).Take(this.MaxSelLibroFirma - countSelectedItems).ToList();


                string operation = string.Empty;
                if (stato.ToString() == TipoStatoElemento.DA_FIRMARE.ToString())
                    operation = "ADD";
                if (ListaElementiFiltrati.Count > elements.Count)
                {
                    string mess = Utils.Languages.GetMessageFromCode("WarningMaxSelectedItems", UserManager.GetUserLanguage()).Replace("@@", this.MaxSelLibroFirma.ToString());
                    int totalSelected = elements.Count + countSelectedItems;
                    int tolatElementsToSelected = ListaElementiFiltrati.Count + countSelectedItems;
                    //mess = mess.Replace("@@", totalSelected.ToString()).Replace("##", tolatElementsToSelected.ToString());
                    msg += string.IsNullOrEmpty(msg) ? mess : "<br /> <br />" + mess;
                }
                foreach (ElementoInLibroFirma elem in elements)
                {
                    long fileSize = elem.FileSize;

                    if (operation == "ADD")
                    {
                        long tempTotalFileSize = totalFileSize + fileSize;
                        if (tempTotalFileSize > maxFileSizeSelectable)
                        {
                            int total = (elements.Count() - elements.IndexOf(elem));
                            elements.RemoveRange(elements.IndexOf(elem), total);

                            string mess = Utils.Languages.GetMessageFromCode("WarningMaxFileSizeSelectedItems", UserManager.GetUserLanguage()).Replace("@@", (this.maxFileSizeSelectable / 1000000).ToString());
                            msg += string.IsNullOrEmpty(msg) ? mess : "<br /> <br />" + mess;

                            break;
                        }
                        else
                        {
                            totalFileSize = tempTotalFileSize;
                        }
                    }
                    else
                    {
                        if (totalFileSize <= 0)
                        {
                            totalFileSize = 0;
                            break;
                        }

                        totalFileSize = totalFileSize - fileSize;
                    }
                }

                //List<ElementoInLibroFirma> elements = (from e in this.ListaElementiFiltrati select e).ToList();
                string message = string.Empty;
                if (LibroFirmaManager.AggiornaStatoElementi(elements, stato.ToString(), out message))
                {
                    foreach (ElementoInLibroFirma elemento in elements)
                    {
                        elemento.StatoFirma = stato;
                        if (stato.Equals(TipoStatoElemento.DA_FIRMARE))
                            elemento.MotivoRespingimento = string.Empty;
                        if (string.IsNullOrEmpty(elemento.DataAccettazione))
                            elemento.DataAccettazione = Utils.dateformat.getDataOdiernaDocspa();
                    }
                    if (!string.IsNullOrEmpty(message))
                    {
                        string mess = Utils.Languages.GetMessageFromCode("RemoveElementsLockedAnotherUser", UserManager.GetUserLanguage()).Replace("@@", RemoveElementsLockedAnotherUser(message));
                        msg += string.IsNullOrEmpty(msg) ? mess : "<br /> <br />" + mess;
                    }
                    if (!string.IsNullOrEmpty(msg))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('MessaggeLibroFirma', 'warning', '', '" + msg + "');} else {parent.ajaxDialogModal('MessaggeLibroFirma', 'warning', '', '" + msg + "');}", true);
                    }
                }
                this.SelectedRow = "-1";
                GridViewResult_Bind();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }



        private void RimuoviSelezione()
        {
            List<ElementoInLibroFirma> elements = (from e in this.ListaElementiFiltrati select e).ToList();
            string message = string.Empty;
            string msg = string.Empty;
            if (LibroFirmaManager.AggiornaStatoElementi(elements, TipoStatoElemento.PROPOSTO.ToString(), out message))
            {
                foreach (ElementoInLibroFirma elemento in this.ListaElementiFiltrati)
                {
                    if (elemento.StatoFirma == TipoStatoElemento.DA_FIRMARE)
                        totalFileSize = totalFileSize - elemento.FileSize;

                    elemento.StatoFirma = TipoStatoElemento.PROPOSTO;
                    elemento.MotivoRespingimento = string.Empty;
                }
                if (!string.IsNullOrEmpty(message))
                {
                    msg = RemoveElementsLockedAnotherUser(message);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RemoveElementsLockedAnotherUser', 'warning', '', '" + msg + "');} else {parent.ajaxDialogModal('RemoveElementsLockedAnotherUser', 'warning', '', '" + msg + "');}", true);
                }
                this.SelectedRow = "-1";
                GridViewResult_Bind();
                this.UpnlGrid.Update();
            }
        }

        public void ddlRolesUser_SelectedIndexChange(object sender, EventArgs e)
        {
            Ruolo[] roles = UIManager.UserManager.GetUserInSession().ruoli;
            Ruolo role = (from r in roles where r.systemId.Equals(this.ddlRolesUser.SelectedValue) select r).FirstOrDefault();
            UIManager.RoleManager.SetRoleInSession(role);
            UIManager.RegistryManager.SetRegAndRFListInSession(UIManager.UserManager.getListaRegistriWithRF(role.systemId, "", ""));
            UIManager.RegistryManager.SetRFListInSession(UIManager.UserManager.getListaRegistriWithRF(role.systemId, "1", ""));
            Response.Redirect("~/Index.aspx");
            return;
        }

        #endregion

        #region  Utils

        private void LoadRolesUser()
        {
            try
            {
                this.ddlRolesUser.Items.Clear();
                ListItem item;
                Ruolo[] role = UIManager.UserManager.GetUserInSession().ruoli;
                foreach (Ruolo r in role)
                {
                    item = new ListItem();
                    item.Value = r.systemId;
                    item.Text = r.descrizione;
                    this.ddlRolesUser.Items.Add(item);
                }

                this.ddlRolesUser.SelectedValue = RoleManager.GetRoleInSession().systemId;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void PopulateTipoFirmaDigitale_People()
        {
            Utente ute = UIManager.UserManager.GetUserInSession();

            string idUtente = UIManager.UserManager.GetUserInSession().idPeople;
            TipoFirmaDigitale_People.Items.Clear();
            foreach (string tipoF in TipiFirma_People)
            {
                ListItem item = new ListItem();
                item.Text = tipoF;
                item.Value = tipoF.Substring(0, 1).ToUpper();
                this.TipoFirmaDigitale_People.Items.Add(item);
            }

            if (cha_TipoFirma_People != null)
            {
                this.TipoFirmaDigitale_People.SelectedValue = cha_TipoFirma_People;
            }
            else
            {
                string signTypePreference = UIManager.UserManager.GetSignTypePreference(idUtente);
                if (string.IsNullOrEmpty(signTypePreference))
                {
                    if (!string.IsNullOrEmpty(TipoFirmaDigitale_People.SelectedValue.ToString()))
                    {
                        UpdateTipoFirma(TipoFirmaDigitale_People.SelectedValue.ToString().ToUpper());
                        UIManager.UserManager.SetSignTypePreference(UIManager.UserManager.GetUserInSession().idPeople, cha_TipoFirma_People);
                    }
                }
                else
                {
                    if (TipoFirmaDigitale_People.Items.FindByValue(signTypePreference) != null)
                    {
                        this.UpdateTipoFirma(signTypePreference);
                        this.TipoFirmaDigitale_People.SelectedValue = cha_TipoFirma_People;
                    }
                    else
                    {
                        this.TipoFirmaDigitale_People.SelectedIndex = 0;
                        this.UpdateTipoFirma(this.TipoFirmaDigitale_People.SelectedValue);
                    }
                }
            }

        }

        private void UpdateTipoFirma(string tipoF)
        {
            cha_TipoFirma_People = tipoF;
            HttpContext.Current.Session["LibroFirma_TipoFirma"] = tipoF;
        }
        private void OrderListElement()
        {
            this.ListaElementiFiltrati = (from e in ListaElementiFiltrati orderby Utils.dateformat.ConvertToDate(e.DataInserimento) descending select e).ToList<ElementoInLibroFirma>();
            List<ElementoInLibroFirma> tmp = new List<ElementoInLibroFirma>();

            foreach (ElementoInLibroFirma e in this.ListaElementiFiltrati)
            {
                //Se è un documento, inserisco lo stesso ed i suoi eventuali allegati nella lista temporanea
                if (string.IsNullOrEmpty(e.InfoDocumento.IdDocumentoPrincipale))
                {
                    tmp.Add(e);
                    tmp.AddRange((from e1 in this.ListaElementiFiltrati where e1.InfoDocumento.IdDocumentoPrincipale.Equals(e.InfoDocumento.Docnumber) select e1).ToList());
                }//Se è un allegato controllo che non ci sia il suo doc principale; in tal caso lo inserisco immediatamente, altrimenti verrà inserito nella condizione di sopra
                else if ((from e1 in this.ListaElementiFiltrati where e1.InfoDocumento.Docnumber.Equals(e.InfoDocumento.IdDocumentoPrincipale) select e1).FirstOrDefault() == null)
                {
                    tmp.Add(e);
                }
            }

            this.ListaElementiFiltrati = tmp;
        }

        private void ApplyFilters()
        {
            if (FiltersElement != null && FiltersElement.Count > 0)
            {
                List<ElementoInLibroFirma> tmpListElemetsFilter = new List<ElementoInLibroFirma>();
                ListaElementiFiltrati.Clear();
                foreach (DocsPaWR.FiltroElementiLibroFirma item in FiltersElement)
                {
                    #region TIPO
                    #region PROTOCOLLO_ARRIVO
                    if (item.Argomento == DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString())
                    {
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma
                                                     where e.InfoDocumento.TipoProto.Equals("A") && !string.IsNullOrEmpty(e.InfoDocumento.NumProto)
                                                     select e).ToList());
                            this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                        }
                    }
                    #endregion
                    #region PROTOCOLLO_PARTENZA
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString())
                    {
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma
                                                     where e.InfoDocumento.TipoProto.Equals("P") && !string.IsNullOrEmpty(e.InfoDocumento.NumProto)
                                                     select e).ToList());
                            this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                        }
                    }
                    #endregion
                    #region PROTOCOLLO_INTERNO

                    else if (item.Argomento == DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString())
                    {
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma
                                                     where e.InfoDocumento.TipoProto.Equals("I") && !string.IsNullOrEmpty(e.InfoDocumento.NumProto)
                                                     select e).ToList());
                            this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                        }
                    }
                    #endregion
                    #region GRIGI
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.GRIGIO.ToString())
                    {
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma
                                                     where e.InfoDocumento.TipoProto.Equals("G") && string.IsNullOrEmpty(e.InfoDocumento.IdDocumentoPrincipale)
                                                     select e).ToList());
                            this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                        }
                    }
                    #endregion
                    #region PREDISPOSTI
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString())
                    {
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma
                                                     where !e.InfoDocumento.TipoProto.Equals("G") && string.IsNullOrEmpty(e.InfoDocumento.NumProto)
                                                     select e).ToList());
                            this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                        }
                    }
                    #endregion
                    #region ALLEGATI
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.ALLEGATO.ToString())
                    {
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma where !string.IsNullOrEmpty(e.InfoDocumento.IdDocumentoPrincipale) select e).ToList());
                            this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                        }
                    }
                    #endregion
                    #endregion
                    #region OGGETTO
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.OGGETTO.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati where e.InfoDocumento.Oggetto.ToUpper().Trim().Contains(item.Valore.ToUpper().Trim()) select e).ToList());
                        this.ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion OGGETTO
                    #region NUMERO_PROTOCOLLO

                    #region NUM_PROTOCOLLO
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where !string.IsNullOrEmpty(e.InfoDocumento.NumProto) && Convert.ToInt32(e.InfoDocumento.NumProto) == Convert.ToInt32(item.Valore)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion NUM_PROTOCOLLO
                    #region NUM_PROTOCOLLO_DAL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma
                                                 where !string.IsNullOrEmpty(e.InfoDocumento.NumProto) && Convert.ToInt32(e.InfoDocumento.NumProto) >= Convert.ToInt32(item.Valore)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion NUM_PROTOCOLLO_DAL
                    #region NUM_PROTOCOLLO_AL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma
                                                 where !string.IsNullOrEmpty(e.InfoDocumento.NumProto) && Convert.ToInt32(e.InfoDocumento.NumProto) <= Convert.ToInt32(item.Valore)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion NUM_PROTOCOLLO_AL

                    #endregion
                    #region DATA_PROTOCOLLO

                    #region DATA_PROT_IL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where !string.IsNullOrEmpty(e.InfoDocumento.DataProtocollo) && Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString().Equals(item.Valore)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion DATA_PROT_IL
                    #region DATA_PROT_SUCCESSIVA_AL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where !string.IsNullOrEmpty(e.InfoDocumento.DataProtocollo) && Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString(), item.Valore)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion DATA_PROT_SUCCESSIVA_AL
                    #region DATA_PROT_PRECEDENTE_IL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where !string.IsNullOrEmpty(e.InfoDocumento.DataProtocollo) && Utils.utils.verificaIntervalloDateSenzaOra(item.Valore, Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString())
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion DATA_PROT_PRECEDENTE_IL
                    #region DATA_PROT_SC
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_SC.ToString() && item.Valore == "1")
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where !string.IsNullOrEmpty(e.InfoDocumento.DataProtocollo)
                                                    && Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString(), NttDataWA.Utils.dateformat.getFirstDayOfWeek())
                                                    && Utils.utils.verificaIntervalloDateSenzaOra(NttDataWA.Utils.dateformat.getLastDayOfWeek(), Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString())
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion
                    #region DATA_PROT_MC
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MC.ToString() && item.Valore == "1")
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where !string.IsNullOrEmpty(e.InfoDocumento.DataProtocollo)
                                                    && Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString(), NttDataWA.Utils.dateformat.getFirstDayOfMonth())
                                                    && Utils.utils.verificaIntervalloDateSenzaOra(NttDataWA.Utils.dateformat.getLastDayOfMonth(), Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString())
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion
                    #region DATA_PROT_TODAY
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_TODAY.ToString() && item.Valore == "1")
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where !string.IsNullOrEmpty(e.InfoDocumento.DataProtocollo) && Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString().Equals(NttDataWA.Utils.dateformat.getDataOdiernaDocspa())
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion
                    #endregion
                    #region DESTINATARIO

                    else if (item.Argomento == DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString())
                    {
                        List<string> idElements = LibroFirmaManager.GetElementiInLibroFirmaByDestinatario(new Corrispondente() { codiceRubrica = item.Valore });
                        if (idElements != null)
                        {
                            tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                     where idElements.Contains(e.IdElemento)
                                                     select e).ToList());
                            ListaElementiFiltrati.Clear();
                            this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                        }
                    }
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString())
                    {
                        List<string> idElements = LibroFirmaManager.GetElementiInLibroFirmaByDestinatario(new Corrispondente() { systemId = item.Valore });
                        if (idElements != null)
                        {
                            tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                     where idElements.Contains(e.IdElemento)
                                                     select e).ToList());
                            ListaElementiFiltrati.Clear();
                            this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                        }
                    }
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.MITT_DEST.ToString())
                    {
                        List<string> idElements = LibroFirmaManager.GetElementiInLibroFirmaByDestinatario(new Corrispondente() { descrizione = item.Valore });
                        if (idElements != null)
                        {
                            tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                     where idElements.Contains(e.IdElemento)
                                                     select e).ToList());
                            ListaElementiFiltrati.Clear();
                            this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                        }
                    }
                    #endregion
                    #region PROPONENTE

                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.CODICE_PROPONENTE.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where e.UtenteProponente.userId.Equals(item.Valore) || e.RuoloProponente.codiceRubrica.Equals(item.Valore)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.ID_PROPONENTE.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where e.UtenteProponente.systemId.Equals(item.Valore) || e.RuoloProponente.systemId.Equals(item.Valore)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DESCRIZIONE_PROPONENTE.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where e.UtenteProponente.descrizione.ToUpper().Contains(item.Valore.ToUpper()) || e.RuoloProponente.descrizione.ToUpper().Contains(item.Valore.ToUpper())
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }

                    #endregion
                    #region DOCNUMBER

                    #region DOCNUMBER
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DOCNUMBER.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where e.InfoDocumento.Docnumber.Equals(item.Valore)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion DOCNUMBER
                    #region DOCNUMBER_DAL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where Convert.ToInt32(e.InfoDocumento.Docnumber) >= Convert.ToInt32(item.Valore)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion DOCNUMBER_DAL
                    #region DOCNUMBER_AL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where Convert.ToInt32(e.InfoDocumento.Docnumber) <= Convert.ToInt32(item.Valore)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion DOCNUMBER_AL

                    #endregion
                    #region DATA_CREAZIONE

                    #region DATA_CREAZIONE_IL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where !string.IsNullOrEmpty(e.InfoDocumento.DataCreazione) && Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString().Equals(item.Valore)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion DATA_CREAZIONE_IL
                    #region DATA_CREAZIONE_SUCCESSIVA_AL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where !string.IsNullOrEmpty(e.InfoDocumento.DataCreazione) && Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString(), item.Valore)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion DATA_CREAZIONE_SUCCESSIVA_AL
                    #region DATA_CREAZIONE_PRECEDENTE_IL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where !string.IsNullOrEmpty(e.InfoDocumento.DataCreazione) && Utils.utils.verificaIntervalloDateSenzaOra(item.Valore, Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString())
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion DATA_CREAZIONE_PRECEDENTE_IL
                    #region DATA_CREAZ_SC
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_SC.ToString() && item.Valore == "1")
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where !string.IsNullOrEmpty(e.InfoDocumento.DataCreazione)
                                                    && Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString(), NttDataWA.Utils.dateformat.getFirstDayOfWeek())
                                                    && Utils.utils.verificaIntervalloDateSenzaOra(NttDataWA.Utils.dateformat.getLastDayOfWeek(), Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString())
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion
                    #region DATA_CREAZ_MC
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_MC.ToString() && item.Valore == "1")
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where !string.IsNullOrEmpty(e.InfoDocumento.DataCreazione)
                                                    && Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString(), NttDataWA.Utils.dateformat.getFirstDayOfMonth())
                                                    && Utils.utils.verificaIntervalloDateSenzaOra(NttDataWA.Utils.dateformat.getLastDayOfMonth(), Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString())
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion
                    #region DATA_CREAZ_TODAY
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_TODAY.ToString() && item.Valore == "1")
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where !string.IsNullOrEmpty(e.InfoDocumento.DataCreazione) && Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString().Equals(NttDataWA.Utils.dateformat.getDataOdiernaDocspa())
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion
                    #endregion
                    #region TIPOLOGIA

                    else if (item.Argomento == DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where e.InfoDocumento.TipologiaDocumento.Equals(item.Valore)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion
                    #region TIPO FIRMA
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.FIRMA_DIGITALE_CADES.ToString())
                    {
                        tmpListElemetsFilter.Clear();
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                     where e.TipoFirma.Equals(LibroFirmaManager.TypeEvent.SIGN_CADES)
                                                     select e).ToList());
                        }
                    }
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.FIRMA_DIGITALE_PADES.ToString())
                    {
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                           where e.TipoFirma.Equals(LibroFirmaManager.TypeEvent.SIGN_PADES)
                                                           select e).ToList());
                        }
                    }

                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.FIRMA_ELETTRONICA_SOTTOSCRIZIONE.ToString())
                    {
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                           where e.TipoFirma.Equals(LibroFirmaManager.TypeEvent.VERIFIED)
                                                           select e).ToList());
                        }
                    }
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.FIRMA_ELETTRONICA_AVANZAMENTO.ToString())
                    {
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                           where e.TipoFirma.Equals(LibroFirmaManager.TypeEvent.ADVANCEMENT_PROCESS)
                                                           select e).ToList());
                        }
                        this.ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion
                    #region MODALITA

                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.MODALITA_AUTOMATICA.ToString())
                    {
                        tmpListElemetsFilter.Clear();
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                     where e.Modalita.Equals("A")
                                                     select e).ToList());
                        }
                    }
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.MODALITA_MANUALE.ToString())
                    {
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                           where e.Modalita.Equals("M")
                                                           select e).ToList());
                        }
                        this.ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }

                    #endregion
                    #region STATO
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.PROPOSTO.ToString())
                    {
                        tmpListElemetsFilter.Clear();
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                     where e.StatoFirma.Equals(TipoStatoElemento.PROPOSTO)
                                                     select e).ToList());
                        }
                    }
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DA_FIRMARE.ToString())
                    {
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                           where e.StatoFirma.Equals(TipoStatoElemento.DA_FIRMARE)
                                                           select e).ToList());
                        }
                    }

                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DA_RESPINGERE.ToString())
                    {
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                           where e.StatoFirma.Equals(TipoStatoElemento.DA_RESPINGERE)
                                                           select e).ToList());
                        }
                        this.ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion
                    #region STATO CON_ERRORI/SENZA ERRORI

                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.CON_ERRORI.ToString())
                    {
                        tmpListElemetsFilter.Clear();
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                           where !string.IsNullOrEmpty(e.ErroreFirma)
                                                           select e).ToList());
                        }
                    }
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.SENZA_ERRORI.ToString())
                    {
                        if (Convert.ToBoolean(item.Valore))
                        {
                            tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                           where string.IsNullOrEmpty(e.ErroreFirma)
                                                           select e).ToList());
                        }
                        this.ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }

                    #endregion
                    #region NOTE

                    else if (item.Argomento == DocsPaWR.FiltriDocumento.NOTE.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where e.Note.ToUpper().Trim().Contains(item.Valore.ToUpper().Trim())
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }

                    #endregion
                    #region DATA_INSERIMENTO

                    #region DATA_INSERIMENTO_IL
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_IL.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString().Equals(item.Valore)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion DATA_INSERIMENTO_IL
                    #region DATA_INSERIMENTO_SUCCESSIVA_AL
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_SUCCESSIVA_AL.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString(), item.Valore)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion DATA_INSERIMENTO_SUCCESSIVA_AL
                    #region DATA_INSERIMENTO_PRECEDENTE_IL
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_PRECEDENTE_IL.ToString())
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where Utils.utils.verificaIntervalloDateSenzaOra(item.Valore, Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString())
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion DATA_INSERIMENTO_PRECEDENTE_IL
                    #region DATA_INSERIMENTO_SC
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_SC.ToString() && item.Valore == "1")
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString(), NttDataWA.Utils.dateformat.getFirstDayOfWeek())
                                                    && Utils.utils.verificaIntervalloDateSenzaOra(NttDataWA.Utils.dateformat.getLastDayOfWeek(), Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString())
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion
                    #region DATA_INSERIMENTO_MC
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_MC.ToString() && item.Valore == "1")
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString(), NttDataWA.Utils.dateformat.getFirstDayOfMonth())
                                                    && Utils.utils.verificaIntervalloDateSenzaOra(NttDataWA.Utils.dateformat.getLastDayOfMonth(), Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString())
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion
                    #region DATA_INSERIMENTO_TODAY
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_TODAY.ToString() && item.Valore == "1")
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString().Equals(NttDataWA.Utils.dateformat.getDataOdiernaDocspa())
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                    #endregion
                    #endregion
                }
                this.OrderListElement();
            }
        }

        private void LoadElementiLbroFirma()
        {
            try
            {
                this.ListaElementiLibroFirma = UIManager.LibroFirmaManager.GetElementiLibroFirma();
                //this.ListaElementiLibroFirma = (from e in ListaElementiLibroFirma orderby e.DataInserimento descending select e).ToList<ElementoInLibroFirma>();
                List<ElementoInLibroFirma> tmp = new List<ElementoInLibroFirma>();

                foreach (ElementoInLibroFirma e in this.ListaElementiLibroFirma)
                {
                    //Se è un documento, inserisco lo stesso ed i suoi eventuali allegati nella lista temporanea
                    if (string.IsNullOrEmpty(e.InfoDocumento.IdDocumentoPrincipale))
                    {
                        tmp.Add(e);
                        tmp.AddRange((from e1 in this.ListaElementiLibroFirma where e1.InfoDocumento.IdDocumentoPrincipale.Equals(e.InfoDocumento.Docnumber) select e1).ToList());
                    }//Se è un allegato controllo che non ci sia il suo doc principale; in tal caso lo inserisco immediatamente, altrimenti verrà inserito nella condizione di sopra
                    else if ((from e1 in this.ListaElementiLibroFirma where e1.InfoDocumento.Docnumber.Equals(e.InfoDocumento.IdDocumentoPrincipale) select e1).FirstOrDefault() == null)
                    {
                        tmp.Add(e);
                    }
                }

                this.ListaElementiLibroFirma = tmp;
                this.ListaElementiFiltrati = new List<ElementoInLibroFirma>();
                this.ListaElementiFiltrati.AddRange(tmp);
            }
            catch (Exception ex)
            {
                string msg = "ErrorReadElementsSignBook";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        private void UpdateLabelStateElements()
        {
            string totalElements = this.ListaElementiLibroFirma.Count().ToString();
            string filteredElements = this.ListaElementiFiltrati.Count().ToString();
            string elementsToSign = (from e in this.ListaElementiLibroFirma
                                     where e.StatoFirma.Equals(TipoStatoElemento.DA_FIRMARE)
                                     select e).ToList().Count().ToString();
            string elementsToReject = (from e in this.ListaElementiLibroFirma
                                       where e.StatoFirma.Equals(TipoStatoElemento.DA_RESPINGERE)
                                       select e).ToList().Count().ToString();
            string elementsPending = (from e in this.ListaElementiLibroFirma
                                      where e.StatoFirma.Equals(TipoStatoElemento.PROPOSTO)
                                      select e).ToList().Count().ToString();

            this.lblNumElementi.Text = Utils.Languages.GetLabelFromCode("lblNumElementi", UserManager.GetUserLanguage()).Replace("@@", totalElements).Replace("#@", elementsPending).Replace("##", elementsToSign).Replace("@#", elementsToReject).Replace("**", dimensioniInStringa());
            this.LibroFirmaImgRemoveFilter.ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaImgRemoveFilterTooltip", UserManager.GetUserLanguage()).Replace("@@", this.ListaElementiFiltrati.Count.ToString());
            this.LibroFirmaImgRemoveFilter.AlternateText = Utils.Languages.GetLabelFromCode("LibroFirmaImgRemoveFilterTooltip", UserManager.GetUserLanguage()).Replace("@@", this.ListaElementiFiltrati.Count.ToString());
            if (this.LibroFirmaImgRemoveFilter.Enabled)
            {
                this.LibroFirmaImgAddFilter.ToolTip = Utils.Languages.GetLabelFromCode("LibroFirmaImgAddFilterTooltip", UserManager.GetUserLanguage()).Replace("@@", this.ListaElementiFiltrati.Count.ToString());
                this.LibroFirmaImgAddFilter.AlternateText = Utils.Languages.GetLabelFromCode("LibroFirmaImgAddFilterTooltip", UserManager.GetUserLanguage()).Replace("@@", this.ListaElementiFiltrati.Count.ToString());
            }
            else
            {
                this.LibroFirmaImgAddFilter.ToolTip = Utils.Languages.GetLabelFromCode("IndexImgAddFilterTooltip", UserManager.GetUserLanguage());
                this.LibroFirmaImgAddFilter.AlternateText = Utils.Languages.GetLabelFromCode("IndexImgAddFilterTooltip", UserManager.GetUserLanguage());
            }
            this.UpnlButtonTop.Update();
            this.upPnlNumElementi.Update();
            logger.Info("AGGIORNAMENTO STATO - AGGIORNAMENTO DELLA GRIGLIA LABEL STATE " + this.lblNumElementi.Text);
        }

        private string dimensioniInStringa()
        {
            string strTotal = "0";
            string strTempVal = string.Empty;

            if (totalFileSize > 0)
            {
                if (totalFileSize > 1000000)
                {
                    strTotal = ((double)totalFileSize / 1000000).ToString();
                    if (strTotal.Length > 5 && strTotal.Contains(','))
                        strTotal = strTotal.Substring(0, 4);
                    strTotal = strTotal + " Mb";
                }
                else
                {
                    strTotal = ((double)totalFileSize / 1000).ToString();
                    if (strTotal.Length > 5)
                    {
                        int indV = strTotal.IndexOf(',');

                        if (strTotal.Count() > (indV + 2))
                            strTotal = strTotal.Substring(0, indV);
                    }
                    strTotal = strTotal + " Kb";
                }
            }

            return strTotal;
        }

        #endregion
    }



}
