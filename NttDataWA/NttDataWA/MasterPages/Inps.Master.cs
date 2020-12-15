using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using NttDataWA.Utils;

namespace NttDataWA.MasterPages
{
    public partial class Inps : System.Web.UI.MasterPage
    {
        private string PAGEHOME = "INDEX.ASPX";
        private string NOTPROTOCOL = "N";
        private string PAGEPROJECT = "PROJECT.ASPX";
        private string PAGEREGISTERS = "REGISTERS.ASPX";
        private string PAGEREGISTERREPERTORIES = "REGISTERREPERTORIES.ASPX";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializesPage();

                string callingPage = System.IO.Path.GetFileName(HttpContext.Current.Request.FilePath);

                if (!string.IsNullOrEmpty(callingPage))
                {
                    switch (callingPage.ToUpper().ToString())
                    {
                        case "DOCUMENT.ASPX":
                        case "VISIBILITY.ASPX":
                        case "ATTACHMENTS.ASPX":
                        case "TRANSMISSIONS.ASPX":
                        case "CLASSIFICATIONS.ASPX":
                        case "EVENTS.ASPX":
                        case "DATAENTRY_DOCUMENT.ASPX":
                            this.SetDocumentPage();
                            string typeDocument = Request.QueryString["t"];
                            if (!string.IsNullOrEmpty(typeDocument))
                            {
                                if (typeDocument.ToUpper().Equals("A") || typeDocument.ToUpper().Equals("P") || typeDocument.ToUpper().Equals("I"))
                                {
                                    this.SetProtocolPage();
                                }
                                else
                                {
                                    if (typeDocument.ToUpper().Equals(NOTPROTOCOL))
                                    {
                                        this.SetNotProtocolPage();
                                    }
                                }
                            }
                            break;
                        case "PROJECT.ASPX":
                        case "VISIBILITYP.ASPX":
                        case "TRANSMISSIONSP.ASPX":
                        case "EVENTSP.ASPX":
                            this.SetProjectPage();
                            break;

                        case "SEARCHDOCUMENT.ASPX":
                        case "SEARCHPROJECT.ASPX":
                        case "SEARCHDOCUMENTPRINTS.ASPX":
                        case "SEARCHDOCUMENTADVANCED.ASPX":
                        case "SEARCHARCHIVE.ASPX":
                            this.SetSearchPage();
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(callingPage) && (callingPage.ToUpper().Equals(PAGEHOME)))
                {
                    this.SetHomePage();
                }

                if (!string.IsNullOrEmpty(callingPage) && callingPage.ToUpper().Equals(PAGEPROJECT))
                {
                    this.SetProjectPage();
                }

                if (!string.IsNullOrEmpty(callingPage) && callingPage.ToUpper().Equals(PAGEREGISTERS))
                {
                    this.SetRegistersPage();
                }

                if (!string.IsNullOrEmpty(callingPage) && callingPage.ToUpper().Equals(PAGEREGISTERREPERTORIES))
                {
                    this.SetRegisterRepertoriesPage();
                }

                //if (Request.UrlReferrer != null)
                //{
                //    // instance of the class history and method call that saves the history
                //    HystoryBrowser.history hist = new HystoryBrowser.history();
                //    hist.saveHistoryBrowser(Request.UrlReferrer.ToString());
                //    // bind history list to object
                //    AddItemsBulletList((IDictionary)Session["history"]);              
                //}


                this.VisibiltyRoleFunctions();

            }
        }

        public bool InternalRecordEnable{
            get{

                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString())) && Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString()).Equals("1"))
                     return true;
                else
                    return false;
            }
        
        }

       

         

        private void VisibiltyRoleFunctions()
        {
            if (!UIManager.UserManager.IsAuthorizedFunctions("MENU_DOCUMENTI"))
            {
                this.LiMenuDocument.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("MENU_OPZIONI"))
            {
                this.LiMenuManagement.Visible = false;
            }

            //if (!UIManager.UserManager.IsAuthorizedFunctions("MENU_HELP"))
            //{
            //    this.LiMenuHelp.Visible = false;
            //}

            if (!UIManager.UserManager.IsAuthorizedFunctions("MENU_RICERCA"))
            {
                this.LiMenuSearch.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_NUOVOPROT"))
            {
                this.LiNewInboundRecord.Visible = false;
                this.LiNewOutBoundRecord.Visible = false;
                //if(this.InternalRecordEnable)
                //    this.LiNewInternalRecord.Visible = false;
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_NUOVOPROT"))
            {
                if (!UIManager.UserManager.IsAuthorizedFunctions("PROTO_IN"))
                {
                    this.LiNewInboundRecord.Visible = false;
                }

                if (!UIManager.UserManager.IsAuthorizedFunctions("PROTO_OUT"))
                {
                    this.LiNewOutBoundRecord.Visible = false;
                }

                //if (!UIManager.UserManager.IsAuthorizedFunctions("PROTO_OWN"))
                //{
                //     if(this.InternalRecordEnable)
                //        this.LiNewInternalRecord.Visible = false;
                //}
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_NUOVODOC"))
            {
                this.LiNewDocument.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("IMP_DOCS"))
            {
                this.LiMenuImportDoc.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("IMP_DOC_PREG"))
            {
                this.LiMenuImportDocPre.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("IMP_RDE"))
            {
                this.LiMenuImportRDE.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("IMP_DOCS") && !UIManager.UserManager.IsAuthorizedFunctions("IMP_DOC_PREG") && !UIManager.UserManager.IsAuthorizedFunctions("IMP_RDE"))
            {
                this.LiMenuImport.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_CERCA"))
            {
                this.LiMenuSearchDoc.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_GESTIONE"))
            {
                this.LiMenuSearchProject.Visible = false;
                this.liNuovoFascicolo.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("TRAS_CERCA"))
            {
                this.LiMenuSearchTransmissions.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_RIC_VISIBILITA"))
            {
                this.LiMenuSearchVisibility.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_RIC_CAMPI_COMUNI"))
            {
                this.LiMenuSearchCommonFields.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_AREA_LAV"))
            {
                this.LiMenuSearchAdlDoc.Visible = false;
                this.LiMenuSearchAdlProject.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("IMP_FASC"))
            {
                this.LiMenuImportProject.Visible = false;
            }

            
            if (!UIManager.UserManager.IsAuthorizedFunctions("IMP_FASC") && !UIManager.UserManager.IsAuthorizedFunctions("FASC_GESTIONE"))
            {
                this.lifascicolo.Visible = false;
            }


            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_REGISTRI"))
            {
                this.LiMenuRegistry.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_REGISTRO_REPERTORIO"))
            {
                this.LiMenuRepertory.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_STAMPE"))
            {
                this.LiMenuPrints.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_PROSPETTI"))
            {
                this.LiMenuProspects.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_RUBRICA"))
            {
                this.LiMenuAddressBook.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_ORGANIGRAMMA"))
            {
                this.LiMenuList.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_DELEGHE"))
            {
                this.LiMenuDelegate.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_ELENCO_NOTE"))
            {
                this.LiMenuNotes.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_REGISTRI") && !UIManager.UserManager.IsAuthorizedFunctions("GEST_REGISTRO_REPERTORIO") && !UIManager.UserManager.IsAuthorizedFunctions("GEST_STAMPE") && !UIManager.UserManager.IsAuthorizedFunctions("GEST_PROSPETTI") && !UIManager.UserManager.IsAuthorizedFunctions("GEST_RUBRICA") && !UIManager.UserManager.IsAuthorizedFunctions("GEST_ORGANIGRAMMA") && !UIManager.UserManager.IsAuthorizedFunctions("GEST_DELEGHE") && !UIManager.UserManager.IsAuthorizedFunctions("GEST_ELENCO_NOTE"))
            {
                this.LiMenuManagement.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("OPZIONI_CAMBIA_PWD"))
            {
                this.LiMenuOptions.Visible = false;
                this.LiMenuChangePassword.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_NUOVO"))
            {
                this.liNuovoFascicolo.Visible = false;
            }


            //se è attiva la chiave litedocument non mostro i seguenti menu
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
            {
                LiMenuImportProject.Visible = false;
                LiMenuSearchVisibility.Visible = false;
                LiMenuSearchCommonFields.Visible = false;
            }



        }

        protected void InitializesPage()
        {
            this.IdMasterBody.Attributes.Add("onload", "initDate();");
            this.SetIpAddress();
            this.InitializesLabels();
            this.InitializeCssText();
            this.LoadKeys();
        }

        private void LoadKeys()
        {
            this.LinkSearchDoc.Attributes.Add("href", ResolveUrl("~/Search/SearchDocument.aspx")); 
            this.LinkSearchDoc.Attributes.Add("onclick","disallowOp('menuTop', '"+ResolveUrl("~/Index.aspx").Replace("Index.aspx", "") + "')");

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
            {
                LinkSearchDoc.Attributes.Add("href", ResolveUrl("~/Search/SearchDocumentAdvanced.aspx")); 
            }
        }

        protected void InitializeCssText()
        {
            if (!string.IsNullOrEmpty(UIManager.CssManager.GetSizeText()))
            {
                this.LnkSize1.Attributes.Remove("class");
                this.LnkSize2.Attributes.Remove("class");
                this.LnkSize3.Attributes.Remove("class");
                this.IdMasterBody.Attributes.Remove("class");
                if (UIManager.CssManager.GetSizeText().Equals(UIManager.CssManager.TextSize.NORMAL.ToString()))
                {
                    this.LnkSize1.Attributes.Add("class", "size1Sel");
                    this.LnkSize2.Attributes.Add("class", "size2");
                    this.LnkSize3.Attributes.Add("class", "size3");
                    this.IdMasterBody.Attributes.Add("class", "body_normal");
                }
                else
                {
                    if (UIManager.CssManager.GetSizeText().Equals(UIManager.CssManager.TextSize.MEDIUM.ToString()))
                    {
                        this.LnkSize2.Attributes.Add("class", "size2Sel");
                        this.LnkSize1.Attributes.Add("class", "size1");
                        this.LnkSize3.Attributes.Add("class", "size3");
                        this.IdMasterBody.Attributes.Add("class", "body_medium");
                    }
                    else
                    {
                        this.LnkSize3.Attributes.Add("class", "size3Sel");
                        this.LnkSize1.Attributes.Add("class", "size1");
                        this.LnkSize2.Attributes.Add("class", "size2");
                        this.IdMasterBody.Attributes.Add("class", "body_high");
                    }
                }
            }
            else
            {
                this.LnkSize1.Attributes.Add("class", "size1Sel");
                this.LnkSize2.Attributes.Add("class", "size2");
                this.LnkSize3.Attributes.Add("class", "size3");
                this.IdMasterBody.Attributes.Add("class", "body_normal");
            }
        }

        protected void LnkSize1_Click(object sender, EventArgs e)
        {
            this.LnkSize1.Attributes.Remove("class");
            this.LnkSize2.Attributes.Remove("class");
            this.LnkSize3.Attributes.Remove("class");
            this.LnkSize1.Attributes.Add("class", "size1Sel");
            this.LnkSize2.Attributes.Add("class", "size2");
            this.LnkSize3.Attributes.Add("class", "size3");
            UIManager.CssManager.SetSizeText(UIManager.CssManager.TextSize.NORMAL.ToString());
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "updateCss", "$('body').attr('class', 'body_normal');", true);
            //this.IdMasterBody.Attributes.Add("class", "body_normal");
        }

        protected void LnkSize2_Click(object sender, EventArgs e)
        {
            this.LnkSize1.Attributes.Remove("class");
            this.LnkSize2.Attributes.Remove("class");
            this.LnkSize3.Attributes.Remove("class");
            this.LnkSize2.Attributes.Add("class", "size2Sel");
            this.LnkSize1.Attributes.Add("class", "size1");
            this.LnkSize3.Attributes.Add("class", "size3");
            UIManager.CssManager.SetSizeText(UIManager.CssManager.TextSize.MEDIUM.ToString());
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "updateCss", "$('body').attr('class', 'body_medium');", true);
            //this.IdMasterBody.Attributes.Add("class", "body_medium");
        }

        protected void LnkSize3_Click(object sender, EventArgs e)
        {
            this.LnkSize1.Attributes.Remove("class");
            this.LnkSize2.Attributes.Remove("class");
            this.LnkSize3.Attributes.Remove("class");
            this.LnkSize3.Attributes.Add("class", "size3Sel");
            this.LnkSize1.Attributes.Add("class", "size1");
            this.LnkSize2.Attributes.Add("class", "size2");
            UIManager.CssManager.SetSizeText(UIManager.CssManager.TextSize.HIGH.ToString());
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "updateCss", "$('body').attr('class', 'body_high');", true);
            //this.IdMasterBody.Attributes.Add("class", "body_high");
        }

        protected void InitializesLabels()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            string languageDirection = Utils.Languages.GetLanguageDirection(language);
            this.Html.Attributes.Add("dir", languageDirection);
            this.SetCssClass(languageDirection);
            this.InitializeLabelMenu(language);

            this.LblTextSize.Text = Utils.Languages.GetLabelFromCode("LblTextSize", language);
            this.BaseMasterTxtSearch.Text = Utils.Languages.GetLabelFromCode("BaseMasterTxtSearch", language);
            this.Loading.Text = Utils.Languages.GetLabelFromCode("Loading", language);
            this.litDialogCheck.Text = Utils.Languages.GetLabelFromCode("DialogCheckTitle", language);
            this.litDialogError.Text = Utils.Languages.GetLabelFromCode("DialogErrorTitle", language);
            this.litDialogInfo.Text = Utils.Languages.GetLabelFromCode("DialogInfoTitle", language);
            this.litDialogQuestion.Text = Utils.Languages.GetLabelFromCode("DialogQuestionTitle", language);
            this.litDialogWarning.Text = Utils.Languages.GetLabelFromCode("DialogWarningTitle", language);
            this.litConfirm.Text = Utils.Languages.GetLabelFromCode("DialogQuestionTitle", language);

            //Gestione orario solo per italiano e inglese
            if (string.IsNullOrEmpty(language) || language.Equals("Italian"))
            {
                this.spanLanguage.Attributes.Add("class", "lg1");
            }
            else
            {
                if (language.Equals("English"))
                {
                    this.spanLanguage.Attributes.Add("class", "lg2");
                }
            }
        }

        protected void InitializeLabelMenu(string language)
        {
            this.BaseMasterLblHome.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblHome", language);
            this.BaseMasterLblDocument.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblDocument", language);
            //this.BaseMasterLblNewRecord.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblNewRecord", language);
            this.BaseMasterLblNewDocument.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblNewDocument", language);
            //this.BaseMasterLblARecord.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblARecord", language);
            //this.BaseMasterLblPRecord.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblPRecord", language);
            this.BaseMasterLblImport.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblImport", language);
            this.BaseMasterLblImportDoc.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblImportDoc", language);
            this.BaseMasterLblImportDocPre.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblImportDocPre", language);
            this.BaseMasterLblImportRDE.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblImportRDE", language);
            this.BaseMasterLblImportProject.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblImportProject", language);
            this.BaseMasterLblSearch.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearch", language);
            this.BaseMasterLblSearchDoc.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchDoc", language);
            this.BaseMasterLblSearchProject.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchProject", language);
            this.BaseMasterLblSearchTransmissions.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchTransmissions", language);
            this.BaseMasterLblSearchVisibility.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchVisibility", language);
            this.BaseMasterLblSearchAdlDoc.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchAdlDoc", language);
            this.BaseMasterLblSearchAdlProject.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchAdlProject", language);
            this.BaseMasterLblSearchCommonFields.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchCommonFields", language);
            this.BaseMasterLblManagement.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblManagement", language);
            this.BaseMasterLblRegistry.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblRegistry", language);
            this.BaseMasterLblRepertoires.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblRepertoires", language);
            this.BaseMasterPrints.Text = Utils.Languages.GetLabelFromCode("BaseMasterPrints", language);
            this.BaseMasterProspects.Text = Utils.Languages.GetLabelFromCode("BaseMasterProspects", language);
            this.BaseMasterAddressBook.Text = Utils.Languages.GetLabelFromCode("BaseMasterAddressBook", language);
            this.BaseMasterList.Text = Utils.Languages.GetLabelFromCode("BaseMasterList", language);
            this.BaseMasterDelegate.Text = Utils.Languages.GetLabelFromCode("BaseMasterDelegate", language);
            this.BaseMasterNotes.Text = Utils.Languages.GetLabelFromCode("BaseMasterNotes", language);
            this.BaseMasterOptions.Text = Utils.Languages.GetLabelFromCode("BaseMasterOptions", language);
            this.BaseMasterChangePassword.Text = Utils.Languages.GetLabelFromCode("BaseMasterChangePassword", language);
            this.BaseMasterHelp.Text = Utils.Languages.GetLabelFromCode("BaseMasterHelp", language);
            this.BaseMasterLogOut.Text = Utils.Languages.GetLabelFromCode("BaseMasterLogOut", language);
            this.lblMenuFascicolo.Text=Utils.Languages.GetLabelFromCode("BaseFascicolo", language);
            this.lblNuovoFascicolo.Text = Utils.Languages.GetLabelFromCode("BaseNuovoFascicolo", language);
            this.BaseMasterLblNewRecordInBound.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblNewRecordInBound", language);
            this.BaseMasterLblNewRecordOutBound.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblNewRecordOutBound", language);
             //if(this.InternalRecordEnable)
             //   this.BaseMasterLblNewRecordInternal.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblNewRecordInternal", language);
         }

        /// <summary>
        /// Set right or left Css
        /// </summary>
        /// <param name="languageDirection"></param>
        private void SetCssClass(string languageDirection)
        {
            string link = "~/Css/Left/Layout.css";
            if (!string.IsNullOrEmpty(languageDirection) && languageDirection.Equals("rtl"))
            {
                link = "~/Css/Right/Layout.css";
            }
            this.CssLayout.Attributes.Add("href", link);
        }

        protected void SetProjectPage()
        {
            this.lifascicolo.Attributes.Remove("class");
            this.lifascicolo.Attributes.Add("class", "iAmMenuFascicolo");

            this.NamePage.Text = "Scheda fascicolo";

            //xDemo
            ListItem link = new ListItem();
            link.Text = "Home";
            link.Value = "~/Index.aspx";
            this.BltListHistory.Items.Add(link);
            //Fine Demo
        }

        protected void SetSearchPage()
        {
            this.LiMenuSearch.Attributes.Remove("class");
            this.LiMenuSearch.Attributes.Add("class", "iAmMenuSearch");

            this.NamePage.Text = "Cerca";

            //xDemo
            ListItem link = new ListItem();
            link.Text = "Home";
            link.Value = "~/Index.aspx";
            this.BltListHistory.Items.Add(link);
            //Fine Demo
        }

        protected void SetHomePage()
        {
            this.LiMenuHome.Attributes.Remove("class");
            this.LiMenuHome.Attributes.Add("class", "iAmMenuHome");
            this.NamePage.Text = "Home";

        }

        protected void SetDocumentPage()
        {
            this.LiMenuDocument.Attributes.Remove("class");
            this.LiMenuDocument.Attributes.Add("class", "iAmMenuDocument");

            //xDemo
            ListItem link = new ListItem();
            link.Text = "Home";
            link.Value = "~/Index.aspx";
            this.BltListHistory.Items.Add(link);
            //Fine Demo
        }

        protected void SetProtocolPage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            //Gestione orario solo per italiano e inglese
            if (string.IsNullOrEmpty(language) || language.Equals("Italian"))
            {
                this.NamePage.Text = "Nuovo Protocollo";
            }
            else
            {
                if (language.Equals("English"))
                {
                    this.NamePage.Text = "New Record";
                }
            }
        }

        protected void SetNotProtocolPage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            //Gestione orario solo per italiano e inglese
            if (string.IsNullOrEmpty(language) || language.Equals("Italian"))
            {
                this.NamePage.Text = "Nuovo Documento non protocollato";
            }
            else
            {
                if (language.Equals("English"))
                {
                    this.NamePage.Text = "New Document";
                }
            }
        }

        protected void SetRegistersPage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            if (string.IsNullOrEmpty(language) || language.Equals("Italian"))
            {
                this.NamePage.Text = "Registri";
            }
            else
            {
                if (language.Equals("English"))
                {
                    this.NamePage.Text = "Registers";
                }
            }
        }

        protected void SetRegisterRepertoriesPage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            if (string.IsNullOrEmpty(language) || language.Equals("Italian"))
            {
                this.NamePage.Text = "Registri repertorio";
            }
            else
            {
                if (language.Equals("English"))
                {
                    this.NamePage.Text = "Registers repertories";
                }
            }
        }

        protected string GetSurnameUser()
        {
            string result = string.Empty;
            DocsPaWR.Utente utente = UIManager.UserManager.GetUserInSession();
            //XDEMO
            if (utente != null)
            {
                result = utente.cognome;
            }
            else
            {
                result = "Molinari";

            }
            //FINE DEMO
            return result;
        }

        protected string GetNameUser()
        {
            string result = string.Empty;
            DocsPaWR.Utente utente = UIManager.UserManager.GetUserInSession();
            //XDEMO
            if (utente != null)
            {
                result = utente.nome;
            }
            else
            {
                result = "Cinzia";

            }
            //FINE DEMO
            return result;
        }

        protected string GetRoleUser()
        {
            string result = string.Empty;
            DocsPaWR.Utente utente = UIManager.UserManager.GetUserInSession();
            //XDEMO
            if (utente != null)
            {
                result = utente.ruoli[0].descrizione;
            }
            else
            {
                result = "Segreteria Avvocatura della Provincia";

            }
            //Fine demo
            return result;
        }

        private void AddItemsBulletList(IDictionary history)
        {
            //bltListHistory.DataSource = history.Values;
            //bltListHistory.DataBind();
        }

        private void SetIpAddress()
        {
            Utils.Version version = new Utils.Version();

            this.LblCopyright.Text = version.CopyrightName;

            this.LblVersion.Text = version.ApplicationNameVersion;

            this.ImgEuropeFlag.Src = ResolveUrl("~/Images/Common/europe_flag.png");
            this.ImgItalianLaw.Src = ResolveUrl("~/Images/Common/italian_law.png");

            this.ImgEuropeFlag.Alt = this.Server.MachineName + this.getIPAddress();
            this.ImgItalianLaw.Alt = this.Server.MachineName + this.getIPAddress();

            this.ImgEuropeFlag.Attributes.Add("title", this.Server.MachineName + this.getIPAddress());
            this.ImgItalianLaw.Attributes.Add("title", this.Server.MachineName + this.getIPAddress());

            this.LblIP.Text = "IP " + this.getIPAddress();
        }

        private string getIPAddress()
        {
            string retValue = string.Empty;
            try
            {
                retValue = " / " + Request.ServerVariables["LOCAL_ADDR"];
            }
            catch
            {
                retValue = "";
            }
            return retValue;
        }

    }
}