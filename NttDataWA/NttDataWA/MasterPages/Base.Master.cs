using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using log4net;

namespace NttDataWA.MasterPages
{
    public partial class Base : System.Web.UI.MasterPage
    {
        private ILog logger = LogManager.GetLogger(typeof(Base));

        public string RequestVerifier
        { 
            get
            {
                return (Session["RequestVerifier"] == null ? "" : Convert.ToString(Session["RequestVerifier"]));
            }
            set
            {
                Session["RequestVerifier"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string callingPage = System.IO.Path.GetFileName(Request.FilePath);

                SecurityCheck(callingPage);

                if (!IsPostBack)
                {
                    InitializesPage();

                    if (!string.IsNullOrEmpty(callingPage))
                    {
                        PreparePages(callingPage.ToUpper());
                        SetNavigationMenu(callingPage);
                    }

                    VisibiltyRoleFunctions();
                }
                else
                {
                    ReadRetValueFromPopup();
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                Response.End();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private void SecurityCheck(string page)
        {
            if (!Page.IsPostBack && Request.HttpMethod == "GET")
            {
                hfRequestVerifier.Value = Guid.NewGuid().ToString("N");
                RequestVerifier = hfRequestVerifier.Value;

                logger.DebugFormat("Creazione GUID per {0} {1}", Request.HttpMethod, page);
                logger.DebugFormat("Hidden field : {0}", hfRequestVerifier.Value);
                logger.DebugFormat("Sessione     : {0}", RequestVerifier);
            }
            else
            {
                if (RequestVerifier != hfRequestVerifier.Value)
                {
                    logger.DebugFormat("Controllo fallito per {0} {1}", Request.HttpMethod, page);
                    logger.DebugFormat("Hidden field : {0}", hfRequestVerifier.Value);
                    logger.DebugFormat("Sessione     : {0}", RequestVerifier);
                    Response.Redirect("~/LogOut.aspx");
                }
                else logger.DebugFormat("Controllo OK per {0} {1}", Request.HttpMethod, page);
            }
        }

        private void PreparePages(string page)
        {
            switch (page)
            {
                case "DOCUMENT.ASPX":
                case "VISIBILITY.ASPX":
                case "ATTACHMENTS.ASPX":
                case "TRANSMISSIONS.ASPX":
                case "CLASSIFICATIONS.ASPX":
                case "EVENTS.ASPX":
                case "DATAENTRY_DOCUMENT.ASPX":
                case "IMPORTDOCUMENTS.ASPX":
                case "IMPORTPREVIOUS.ASPX":
                case "IMPORTRDE.ASPX":
                case "IMPORTPROJECTS.ASPX":
                case "IMPORTINVOICE.ASPX":
                    SetDocumentPage();
                    break;

                case "PROJECT.ASPX":
                case "VISIBILITYP.ASPX":
                case "TRANSMISSIONSP.ASPX":
                case "EVENTSP.ASPX":
                case "STRUCTURE.ASPX":
                case "DATAENTRY_PROJECT.ASPX":
                    SetProjectPage();
                    break;

                case "SEARCHDOCUMENT.ASPX":
                case "SEARCHPROJECT.ASPX":
                case "SEARCHDOCUMENTPRINTS.ASPX":
                case "SEARCHDOCUMENTADVANCED.ASPX":
                case "SEARCHARCHIVE.ASPX":
                case "SEARCHVISIBILITY.ASPX":
                case "SEARCHTRANSMISSION.ASPX":
                    SetSearchPage();
                    break;

                case "INDEX.ASPX":
                case "ADLPROJECT.ASPX":
                case "ADLDOCUMENT.ASPX":
                case "LIBROFIRMA.ASPX":
                case "TASK.ASPX":
                    SetHomePage();
                    break;

                case "REGISTERS.ASPX":
                case "REGISTERREPERTORIES.ASPX":
                case "MANDATE.ASPX":
                case "GESTIONEMODELLITRASM.ASPX":
                case "DISTRIBUTIONLIST.ASPX":
                case "SIGNATUREPROCESSES.ASPX":
                case "MONITORINGPROCESSES.ASPX":
                case "SUMMERIES.ASPX":
                case "PRINTS.ASPX":
                case "NOTES.ASPX":
                case "CONSERVATIONAREA.ASPX":
                case "DOCUMENTSREMOVED.ASPX":
                case "ORGANIZATIONCHART.ASPX":
                case "INSTANCEACCESS.ASPX":
                case "INSTANCEDETAILS.ASPX":
                case "INSTANCESTRUCTURE.ASPX":
                    SetManagementPage();
                    break;

                case "CHANGEPASSWORD.ASPX":
                    SetOptionsPage();
                    break;
            }
        }

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(SendingReport.ReturnValue))
            {
                //Response.Redirect(Page.ResolveUrl("~/Document/Document.aspx"));

                ScriptManager.RegisterStartupScript(Page, GetType(), "setReturnValue", "SetRetValue('SendingReport','');disallowOp('menuTop');document.location.href = '" + Page.ResolveUrl("~/Document/Document.aspx") + "';", true);
            }
            if (!string.IsNullOrEmpty(HiddenIdProfile.Value))
            {
                string idProfile = HiddenIdProfile.Value;               
                HiddenIdProfile.Value = string.Empty;
                ViewRecentDocumentSelected(idProfile);

            }
            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals("upPnlInfoUser"))
            {
                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals("closePopupAddressBook")))
                {
                    ScriptManager.RegisterStartupScript(Page, this.GetType(), "popupObject", "document.getElementById('ifrm_Formazione').contentWindow.closePopupAddressBook();", true);
                    return;
                }
            }
        }

        private void SetNavigationMenu(string page)
        {
            LinkUrlNavigation.Attributes.Add("onclick", "disallowOp('menuTop')");
            LinkUrlNavigation.ImageUrl = VirtualPathUtility.ToAbsolute("~/Images/Common/navigation_icon.jpg");
            LinkUrlNavigation.ImageUrlDisabled = VirtualPathUtility.ToAbsolute("~/Images/Common/navigation_icon.jpg");
            LinkUrlNavigation.OnMouseOverImage = VirtualPathUtility.ToAbsolute("~/Images/Common/navigation_icon_hover.jpg");
            LinkUrlNavigation.OnMouseOutImage = VirtualPathUtility.ToAbsolute("~/Images/Common/navigation_icon.jpg");

            LinkButtonPreMaster.Attributes.Add("onclick", "disallowOp('menuTop')");
            LinkButtonPreMaster.ImageUrl = VirtualPathUtility.ToAbsolute("~/Images/Common/back_left.jpg");
            LinkButtonPreMaster.ImageUrlDisabled = VirtualPathUtility.ToAbsolute("~/Images/Common/back_left_disabled.jpg");
            LinkButtonPreMaster.OnMouseOverImage = VirtualPathUtility.ToAbsolute("~/Images/Common/back_left_hover.jpg");
            LinkButtonPreMaster.OnMouseOutImage = VirtualPathUtility.ToAbsolute("~/Images/Common/back_left.jpg");


            LinkButtonNextMaster.Attributes.Add("onclick", "disallowOp('menuTop')");
            LinkButtonNextMaster.ImageUrl = VirtualPathUtility.ToAbsolute("~/Images/Common/back_right.jpg");
            LinkButtonNextMaster.ImageUrlDisabled = VirtualPathUtility.ToAbsolute("~/Images/Common/back_right_disabled.jpg");
            LinkButtonNextMaster.OnMouseOverImage = VirtualPathUtility.ToAbsolute("~/Images/Common/back_right_hover.jpg");
            LinkButtonNextMaster.OnMouseOutImage = VirtualPathUtility.ToAbsolute("~/Images/Common/back_right.jpg");

            Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
            Navigation.NavigationObject objectPage = new Navigation.NavigationObject();
            SchedaDocumento doc = null;
            Fascicolo prj = null;
            List<Navigation.NavigationObject> navigationList = new List<Navigation.NavigationObject>();
            bool backClassificataion = true;
            string language = UIManager.UserManager.GetUserLanguage();
            string backTo = Utils.Languages.GetLabelFromCode("LblPaginationBackTo", language);
            switch (page.ToUpper())
            {
                case "INDEX.ASPX":
                    Navigation.NavigationUtils.RemoveNavigationList();
                    IsBack = false;
                    actualPage = new Navigation.NavigationObject();
                    actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME.ToString(), string.Empty);
                    actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME.ToString(), true, Page);
                    actualPage.CodePage = Navigation.NavigationUtils.NamePage.HOME.ToString();
                    actualPage.Page = "INDEX.ASPX";
                    navigationList.Add(actualPage);
                    Navigation.NavigationUtils.SetNavigationList(navigationList);

                    LinkUrlNavigation.Enabled = false;
                    LinkUrlNavigation.Attributes.Remove("onclick");

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME.ToString(), string.Empty);
                    break;

                case "ADLPROJECT.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    doc = UIManager.DocumentManager.getSelectedRecord();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_ADL_PROJECT.ToString(), string.Empty);
                    break;

                case "ADLDOCUMENT.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    doc = UIManager.DocumentManager.getSelectedRecord();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_ADL_DOCUMENT.ToString(), string.Empty);
                    break;
                case "LIBROFIRMA.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    doc = UIManager.DocumentManager.getSelectedRecord();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_LIBRO_FIRMA.ToString(), string.Empty);
                    break;
                case "TASK.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    doc = UIManager.DocumentManager.getSelectedRecord();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), string.Empty);
                    break;
                case "DOCUMENT.ASPX":
                case "CLASSIFICATIONS.ASPX":
                case "VISIBILITY.ASPX":
                case "ATTACHMENTS.ASPX":
                case "TRANSMISSIONS.ASPX":
                case "EVENTS.ASPX":
                case "DATAENTRY_DOCUMENT.ASPX":

                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    doc = UIManager.DocumentManager.getSelectedRecord();
                    if (navigationList != null && navigationList.Count > 0)
                    {
                        objectPage = navigationList.Last();
                        if (doc != null && !string.IsNullOrEmpty(doc.systemId))
                        {
                            actualPage = new Navigation.NavigationObject();
                            actualPage = Navigation.NavigationUtils.CloneObject(objectPage);

                            actualPage.IdObject = doc.systemId;
                            actualPage.Type = string.Empty;
                            actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), string.Empty);
                            actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), true, Page);
                            actualPage.CodePage = Navigation.NavigationUtils.NamePage.DOCUMENT.ToString();
                            actualPage.Page = "DOCUMENT.ASPX";
                            if (objectPage.CodePage != Navigation.NavigationUtils.NamePage.DOCUMENT.ToString())
                            {
                                if (!IsBack)
                                {
                                    if (!objectPage.ViewResult && objectPage.CodePage != Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_DETAILS.ToString()
                                        && objectPage.CodePage != Navigation.NavigationUtils.NamePage.HOME_TASK.ToString()
                                        && objectPage.CodePage != Navigation.NavigationUtils.NamePage.PROJECT_TRANSMISSIONS.ToString()
                                        && objectPage.CodePage != Navigation.NavigationUtils.NamePage.DOCUMENT_TRANSMISSIONS.ToString())
                                    {
                                        if (string.IsNullOrEmpty(actualPage.IdObject) || string.IsNullOrEmpty(objectPage.IdObject) || !objectPage.IdObject.Equals(actualPage.IdObject))
                                        {
                                            if (objectPage.CodePage != Navigation.NavigationUtils.NamePage.DOCUMENT_ATTACHMENT.ToString())
                                            {
                                                navigationList.Add(actualPage);
                                                Navigation.NavigationUtils.SetNavigationList(navigationList);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(actualPage.IdObject) || string.IsNullOrEmpty(objectPage.IdObject) || !objectPage.IdObject.Equals(actualPage.IdObject))
                                {
                                    if (objectPage.CodePage != Navigation.NavigationUtils.NamePage.DOCUMENT.ToString())
                                    {
                                        objectPage = navigationList.ElementAt(navigationList.Count - 2);
                                    }
                                    else
                                    {
                                        objectPage = navigationList.Last();
                                    }
                                }
                            }
                        }

                        LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                        LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                        //switch (page.ToUpper())
                        //{
                        //    case "CLASSIFICATIONS.ASPX":
                        //        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT_CLASSIFICATION.ToString(), string.Empty);
                        //        break;
                        //case "DOCUMENT.ASPX":
                        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), string.Empty);
                        //        break;
                        //    case "VISIBILITY.ASPX":
                        //        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT_VISIBILITY.ToString(), string.Empty);
                        //        break;
                        //    case "ATTACHMENTS.ASPX":
                        //        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT_ATTACHMENT.ToString(), string.Empty);
                        //        break;
                        //    case "TRANSMISSIONS.ASPX":
                        //        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT_TRANSMISSIONS.ToString(), string.Empty);
                        //        break;
                        //    case "EVENTS.ASPX":
                        //        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT_EVENTS.ToString(), string.Empty);
                        //        break;
                        //    case "DATAENTRY_DOCUMENT.ASPX":
                        //        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT_NEW_TRANSMISSION.ToString(), string.Empty);
                        //        break;
                        //}




                        if (actualPage.ViewResult && !string.IsNullOrEmpty(objectPage.DxPositionElement) && !string.IsNullOrEmpty(objectPage.DxTotalNumberElement) && !string.IsNullOrEmpty(objectPage.NumPage))
                        {
                            PlhMasterNavigationDx.Visible = true;

                            if (actualPage.FromNotifyCenter)
                            {
                                LblPaginationDx.Text = objectPage.DxPositionElement + "/" + objectPage.DxTotalNumberElement + " " + Utils.Languages.GetLabelFromCode("LblPaginationTypeNotificationCenter", language) + " " + Utils.Languages.GetLabelFromCode("LblPaginationDx", language) + " " + objectPage.NumPage + " " +
                                    Utils.Languages.GetLabelFromCode("LblPaginationOfDx", language) + " " + objectPage.DxTotalPageNumber;
                            }
                            else
                            {
                                LblPaginationDx.Text = objectPage.DxPositionElement + "/" + objectPage.DxTotalNumberElement + " " + Utils.Languages.GetLabelFromCode("LblPaginationTypeDoc", language) + " " + Utils.Languages.GetLabelFromCode("LblPaginationDx", language) + " " + objectPage.NumPage + " " +
                                    Utils.Languages.GetLabelFromCode("LblPaginationOfDx", language) + " " + objectPage.DxTotalPageNumber;
                            }

                            if (objectPage.DxPositionElement.Equals(objectPage.DxTotalNumberElement))
                            {
                                LinkButtonNextMaster.Enabled = false;
                            }

                            if (objectPage.DxPositionElement.Equals("1") && objectPage.NumPage.Equals("1"))
                            {
                                LinkButtonPreMaster.Enabled = false;
                            }
                        }
                    }
                    else
                    {
                        Navigation.NavigationUtils.RemoveNavigationList();
                        IsBack = false;
                        navigationList = new List<Navigation.NavigationObject>();
                        actualPage = new Navigation.NavigationObject();
                        actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME.ToString(), string.Empty);
                        actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME.ToString(), true,Page);
                        actualPage.CodePage = Navigation.NavigationUtils.NamePage.HOME.ToString();
                        actualPage.Page = "INDEX.ASPX";
                        navigationList.Add(actualPage);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);

                        objectPage = new Navigation.NavigationObject();
                        objectPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), string.Empty);
                        objectPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), true,Page);
                        objectPage.CodePage = Navigation.NavigationUtils.NamePage.DOCUMENT.ToString();
                        objectPage.Page = "DOCUMENT.ASPX";
                        navigationList.Add(objectPage);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);

                        LinkUrlNavigation.AlternateText = backTo + " " + actualPage.NamePage;
                        LinkUrlNavigation.ToolTip = backTo + " " + actualPage.NamePage;
                        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), string.Empty);

                    }
                    break;



                case "PROJECT.ASPX":
                case "TRANSMISSIONSP.ASPX":
                case "VISIBILITYP.ASPX":
                case "STRUCTURE.ASPX":
                case "EVENTSP.ASPX":
                case "DATAENTRY_PROJECT.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    prj = UIManager.ProjectManager.getProjectInSession();
                    if (navigationList != null && navigationList.Count > 0)
                    {
                        objectPage = navigationList.Last();
                        if (prj != null && !string.IsNullOrEmpty(prj.systemID))
                        {
                            actualPage = new Navigation.NavigationObject();
                            actualPage = Navigation.NavigationUtils.CloneObject(objectPage);
                            actualPage.IdObject = prj.systemID;
                            actualPage.Type = string.Empty;
                            actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), string.Empty);
                            actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), true,Page);
                            actualPage.CodePage = Navigation.NavigationUtils.NamePage.PROJECT.ToString();
                            actualPage.Page = "PROJECT.ASPX";
                            //if (objectPage.CodePage != Navigation.NavigationUtils.NamePage.PROJECT.ToString())
                            //{
                            //    if (!IsBack)
                            //    {
                            //        if (!objectPage.ViewResult)
                            //        {
                            //            if (string.IsNullOrEmpty(actualPage.IdObject) || string.IsNullOrEmpty(objectPage.IdObject) || !objectPage.IdObject.Equals(actualPage.IdObject))
                            //            {
                            //                navigationList.Add(actualPage);
                            //                Navigation.NavigationUtils.SetNavigationList(navigationList);
                            //            }
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    if (string.IsNullOrEmpty(actualPage.IdObject) || string.IsNullOrEmpty(objectPage.IdObject) || !objectPage.IdObject.Equals(actualPage.IdObject))
                            //    {
                            //        if (string.IsNullOrEmpty(objectPage.idProject))
                            //        {
                            //            objectPage = navigationList.ElementAt(navigationList.Count - 2);
                            //        }
                            //        else
                            //        {
                            //            if (navigationList.Count >= 2)
                            //            {
                            //                objectPage = navigationList.ElementAt(navigationList.Count - 2);
                            //                if (objectPage.CodePage.Equals(Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString()) || objectPage.CodePage.Equals(Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS_WA.ToString()))
                            //                {
                            //                    objectPage = navigationList.ElementAt(navigationList.Count - 2);
                            //                    backClassificataion = false;
                            //                }

                            //                //if (objectPage.CodePage.Equals(Navigation.NavigationUtils.NamePage.DOCUMENT_CLASSIFICATION.ToString()))
                            //                //{
                            //                //    backClassificataion = false;
                            //                //}
                            //            }
                            //        }
                            //    }
                            //}
                            if (objectPage.CodePage != Navigation.NavigationUtils.NamePage.PROJECT.ToString())
                            {
                                if (!IsBack)
                                {
                                    if (!objectPage.ViewResult)
                                    {
                                        if (string.IsNullOrEmpty(actualPage.IdObject) || string.IsNullOrEmpty(objectPage.IdObject) || !objectPage.IdObject.Equals(actualPage.IdObject))
                                        {
                                            if (objectPage.CodePage != Navigation.NavigationUtils.NamePage.DOCUMENT_CLASSIFICATION.ToString())
                                            {
                                                navigationList.Add(actualPage);
                                                Navigation.NavigationUtils.SetNavigationList(navigationList);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(actualPage.IdObject) || string.IsNullOrEmpty(objectPage.IdObject) || !objectPage.IdObject.Equals(actualPage.IdObject))
                                {
                                    if (objectPage.CodePage != Navigation.NavigationUtils.NamePage.PROJECT.ToString())
                                    {
                                        objectPage = navigationList.ElementAt(navigationList.Count - 2);
                                    }
                                    else
                                    {
                                        objectPage = navigationList.Last();
                                    }
                                }
                            }
                        }

                        //switch (page.ToUpper())
                        //{
                        //    case "PROJECT.ASPX":
                        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), string.Empty);
                        //        break;
                        //    case "TRANSMISSIONSP.ASPX":
                        //        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT_TRANSMISSIONS.ToString(), string.Empty);
                        //        break;
                        //    case "VISIBILITYP.ASPX":
                        //        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT_VISIBILITY.ToString(), string.Empty);
                        //        break;
                        //    case "STRUCTURE.ASPX":
                        //        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT_STRUCTURE.ToString(), string.Empty);
                        //        break;
                        //    case "EVENTSP.ASPX":
                        //        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT_EVENTS.ToString(), string.Empty);
                        //        break;
                        //    case "DATAENTRY_PROJECT.ASPX":
                        //        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT_NEW_TRANSMISSION.ToString(), string.Empty);
                        //        break;
                        //}
                        LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                        LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;



                        //bool visibleMenu = true;
                        //if (navigationList.Count > 2)
                        //{
                        //    Navigation.NavigationObject tempOb = navigationList.ElementAt(navigationList.Count - 2);
                        //    tempOb.CodePage.Equals(actualPage.CodePage.Equals(
                        //}

                        if (actualPage.ViewResult && !string.IsNullOrEmpty(objectPage.DxPositionElement) && !string.IsNullOrEmpty(objectPage.DxTotalNumberElement) && !string.IsNullOrEmpty(objectPage.NumPage))
                        {
                            PlhMasterNavigationDx.Visible = true;

                            if (actualPage.FromNotifyCenter)
                            {
                                LblPaginationDx.Text = objectPage.DxPositionElement + "/" + objectPage.DxTotalNumberElement + " " + Utils.Languages.GetLabelFromCode("LblPaginationTypeNotificationCenter", language) + " " + Utils.Languages.GetLabelFromCode("LblPaginationDx", language) + " " + objectPage.NumPage + " " +
                                    Utils.Languages.GetLabelFromCode("LblPaginationOfDx", language) + " " + objectPage.DxTotalPageNumber;
                            }
                            else
                            {
                                LblPaginationDx.Text = objectPage.DxPositionElement + "/" + objectPage.DxTotalNumberElement + " " + Utils.Languages.GetLabelFromCode("LblPaginationTypePrj", language) + " " + Utils.Languages.GetLabelFromCode("LblPaginationDx", language) + " " + objectPage.NumPage + " " +
                                    Utils.Languages.GetLabelFromCode("LblPaginationOfDx", language) + " " + objectPage.DxTotalPageNumber;
                            }

                            if (objectPage.DxPositionElement.Equals(objectPage.DxTotalNumberElement))
                            {
                                LinkButtonNextMaster.Enabled = false;
                            }

                            if (objectPage.DxPositionElement.Equals("1") && objectPage.NumPage.Equals("1"))
                            {
                                LinkButtonPreMaster.Enabled = false;
                            }
                        }
                    }
                    else
                    {
                        Navigation.NavigationUtils.RemoveNavigationList();
                        IsBack = false;
                        navigationList = new List<Navigation.NavigationObject>();
                        actualPage = new Navigation.NavigationObject();
                        actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME.ToString(), string.Empty);
                        actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME.ToString(), true,Page);
                        actualPage.CodePage = Navigation.NavigationUtils.NamePage.HOME.ToString();
                        actualPage.Page = "INDEX.ASPX";
                        navigationList.Add(actualPage);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);

                        objectPage = new Navigation.NavigationObject();
                        objectPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), string.Empty);
                        objectPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), true, Page);
                        objectPage.CodePage = Navigation.NavigationUtils.NamePage.PROJECT.ToString();
                        objectPage.Page = "PROJECT.ASPX";
                        navigationList.Add(objectPage);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);

                        LinkUrlNavigation.AlternateText = backTo + " " + actualPage.NamePage;
                        LinkUrlNavigation.ToolTip = backTo + " " + actualPage.NamePage;
                        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), string.Empty);
                    }
                    break;


                case "SEARCHDOCUMENTADVANCED.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    if (Request.QueryString["IsAdl"] != null && Request.QueryString["IsAdl"].ToString().Equals("true"))
                    {
                        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED_ADL.ToString(), string.Empty);
                    }
                    else
                    {
                        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED.ToString(), string.Empty);
                    }
                    break;


                case "SEARCHDOCUMENT.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_SIMPLE.ToString(), string.Empty);
                    break;

                case "SEARCHVISIBILITY.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_VISIBILITY.ToString(), string.Empty);
                    break;

                case "SEARCHTRANSMISSION.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_TRANSMISSIONS.ToString(), string.Empty);
                    break;


                case "SEARCHDOCUMENTPRINTS.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_PRINTS.ToString(), string.Empty);
                    break;

                case "SEARCHPROJECT.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    if (Request.QueryString["IsAdl"] != null && Request.QueryString["IsAdl"].ToString().Equals("true"))
                    {
                        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS_WA.ToString(), string.Empty);
                    }
                    else
                    {
                        NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString(), string.Empty);
                    }

                    break;

                case "SEARCHARCHIVE.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString(), string.Empty);

                    break;

                case "REGISTERS.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    prj = UIManager.ProjectManager.getProjectInSession();
                    objectPage = navigationList.Last();
                    if (prj != null && !string.IsNullOrEmpty(prj.systemID))
                    {
                        actualPage.IdObject = prj.systemID;
                    }

                    actualPage = new Navigation.NavigationObject();

                    actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_REGISTERS.ToString(), string.Empty);
                    actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.MANAGEMENT_REGISTERS.ToString(), true, Page);
                    actualPage.CodePage = Navigation.NavigationUtils.NamePage.MANAGEMENT_REGISTERS.ToString();
                    actualPage.Page = "REGISTERS.ASPX";
                    if (objectPage.CodePage != Navigation.NavigationUtils.NamePage.MANAGEMENT_REGISTERS.ToString())
                    {
                        if (!IsBack)
                        {
                            navigationList.Add(actualPage);
                            Navigation.NavigationUtils.SetNavigationList(navigationList);
                        }
                    }
                    else
                    {
                        objectPage = navigationList.ElementAt(navigationList.Count - 2);
                    }

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_REGISTERS.ToString(), string.Empty);
                    break;

                case "REGISTERREPERTORIES.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    prj = UIManager.ProjectManager.getProjectInSession();
                    objectPage = navigationList.Last();
                    if (prj != null && !string.IsNullOrEmpty(prj.systemID))
                    {
                        actualPage.IdObject = prj.systemID;
                    }

                    actualPage = new Navigation.NavigationObject();

                    actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_REPERTORIES.ToString(), string.Empty);
                    actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.MANAGEMENT_REPERTORIES.ToString(), true, Page);
                    actualPage.CodePage = Navigation.NavigationUtils.NamePage.MANAGEMENT_REPERTORIES.ToString();
                    actualPage.Page = "REGISTERREPERTORIES.ASPX";
                    if (objectPage.CodePage != Navigation.NavigationUtils.NamePage.MANAGEMENT_REPERTORIES.ToString())
                    {
                        if (!IsBack)
                        {
                            navigationList.Add(actualPage);
                            Navigation.NavigationUtils.SetNavigationList(navigationList);
                        }
                    }
                    else
                    {
                        objectPage = navigationList.ElementAt(navigationList.Count - 2);
                    }

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_REPERTORIES.ToString(), string.Empty);
                    break;
                case "MANDATE.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    prj = UIManager.ProjectManager.getProjectInSession();
                    objectPage = navigationList.Last();
                    if (prj != null && !string.IsNullOrEmpty(prj.systemID))
                    {
                        actualPage.IdObject = prj.systemID;
                    }

                    actualPage = new Navigation.NavigationObject();

                    actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANDATE.ToString(), string.Empty);
                    actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.MANDATE.ToString(), true, Page);
                    actualPage.CodePage = Navigation.NavigationUtils.NamePage.MANDATE.ToString();
                    actualPage.Page = "MANDATE.ASPX";
                    if (objectPage.CodePage != Navigation.NavigationUtils.NamePage.MANDATE.ToString())
                    {
                        if (!IsBack)
                        {
                            navigationList.Add(actualPage);
                            Navigation.NavigationUtils.SetNavigationList(navigationList);
                        }
                    }
                    else
                    {
                        objectPage = navigationList.ElementAt(navigationList.Count - 2);
                    }

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANDATE.ToString(), string.Empty);
                    break;
                case "GESTIONEMODELLITRASM.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_MODELS_TRANSMISSION.ToString(), string.Empty);
                    break;

                case "DISTRIBUTIONLIST.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_DISTRIBUTION_LISTS.ToString(), string.Empty);
                    break;
                case "SIGNATUREPROCESSES.ASPX":
                                        navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_SIGNATURE_PROCESSES.ToString(), string.Empty);
                    break;

                case "MONITORINGPROCESSES.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_MONITORING_PROCESSES.ToString(), string.Empty);
                    break;

                case "SUMMERIES.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SUMMERIES.ToString(), string.Empty);
                    break;

                case "PRINTS.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_PRINTS.ToString(), string.Empty);
                    break;

                case "IMPORTDOCUMENTS.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.IMPORT_DOCUMENT.ToString(), string.Empty);
                    break;

                case "IMPORTINVOICE.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.IMPORT_INVOICE.ToString(), string.Empty);
                    break;

                case "IMPORTPREVIOUS.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.IMPORT_DOCUMENT_PREVIOUS.ToString(), string.Empty);
                    break;

                case "IMPORTRDE.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.IMPORT_RDE.ToString(), string.Empty);
                    break;
                case "IMPORTPROJECTS.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.IMPORT_PROJECT.ToString(), string.Empty);
                    break;
                case "NOTES.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_NOTES.ToString(), string.Empty);
                    break;
                case "CONSERVATIONAREA.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_CONSERVATION.ToString(), string.Empty);
                    break;

                case "DOCUMENTSREMOVED.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_DOCUMENTS_REMOVED.ToString(), string.Empty);
                    break;

                case "ORGANIZATIONCHART.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_ORGANIZATION.ToString(), string.Empty);
                    break;

                case "CHANGEPASSWORD.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.CHANGE_PASSWORD.ToString(), string.Empty);
                    break;


                case "INSTANCEACCESS.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_ACCESS.ToString(), string.Empty);
                    break;

                case "INSTANCEDETAILS.ASPX":
                case "INSTANCESTRUCTURE.ASPX":
                    navigationList = Navigation.NavigationUtils.GetNavigationList();
                    objectPage = navigationList.Last();

                    LinkUrlNavigation.AlternateText = backTo + " " + objectPage.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + objectPage.NamePage;

                    NamePage.Text = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_DETAILS.ToString(), string.Empty);
                    break;

               
            }

            if (IsBack)
            {
                Navigation.NavigationObject obj2 = navigationList.ElementAt(navigationList.Count - 2);

                Navigation.NavigationObject obj = navigationList.Last();
                if (!obj.ViewResult || backClassificataion)
                {
                    navigationList.Remove(obj);
                    Navigation.NavigationUtils.SetNavigationList(navigationList);
                }
                else
                {
                    obj.ViewResult = false;
                    obj.folder = null;
                }


                if (obj2 != null)
                {
                    Navigation.NavigationUtils.SetNavigationList(navigationList);
                    LinkUrlNavigation.AlternateText = backTo + " " + obj2.NamePage;
                    LinkUrlNavigation.ToolTip = backTo + " " + obj2.NamePage;
                }

                if (!backClassificataion)
                {
                    navigationList.Remove(obj);
                    Navigation.NavigationUtils.SetNavigationList(navigationList);
                }

                IsBack = false;
            }

        }

        public bool InternalRecordEnable
        {
            get
            {

                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString())) && Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString()).Equals("1"))
                    return true;
                else
                    return false;
            }

        }

        // INTEGRAZIONE PITRE-PARER
        public bool IsConservazioneSACER
        {
            get
            {
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_WA_CONSERVAZIONE.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_WA_CONSERVAZIONE.ToString()).Equals("1"))
                    return true;
                else
                    return false;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            //Deposito 11042013
            //Inserisco la gestione del logo per PCM/Deposito
            //Old:
            //try
            //{
            //    string layout = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.LAYOUT.ToString()];
            //    if (layout != null && layout.ToUpper().Equals("GFD"))
            //        headerLogo.ImageUrl = "../Images/Common/logo_gfd.jpg";
            //    else
            //        headerLogo.ImageUrl = "../Images/Common/logo.jpg";
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
            //NewCode:
            try
            {
                string layout = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.LAYOUT.ToString()];

                if (layout != null)
                {
                    switch (layout.ToUpper())
                    {
                        case "GFD":
                            headerLogo.ImageUrl = "../Images/Common/logo_gfd.jpg";
                            break;
                        case "PCM":
                            headerLogo.ImageUrl = "../Images/Common/logo_pcm.jpg";
                            break;
                    }
                }
                else
                    headerLogo.ImageUrl = "../Images/Common/logo.jpg";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void VisibiltyRoleFunctions()
        {

            if (!InternalRecordEnable)
            {
                LiNewInternalRecord.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("MENU_DOCUMENTI"))
            {
                LiMenuDocument.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("MENU_OPZIONI"))
            {
                LiMenuManagement.Visible = false;
            }

            //if (!UIManager.UserManager.IsAuthorizedFunctions("MENU_HELP"))
            //{
            //    LiMenuHelp.Visible = false;
            //}

            if (!UIManager.UserManager.IsAuthorizedFunctions("MENU_RICERCA"))
            {
                LiMenuSearch.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_NUOVOPROT"))
            {
                LiNewInboundRecord.Visible = false;
                LiNewOutBoundRecord.Visible = false;
                if (InternalRecordEnable)
                    LiNewInternalRecord.Visible = false;
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_NUOVOPROT"))
            {
                if (!UIManager.UserManager.IsAuthorizedFunctions("PROTO_IN"))
                {
                    LiNewInboundRecord.Visible = false;
                }

                if (!UIManager.UserManager.IsAuthorizedFunctions("PROTO_OUT"))
                {
                    LiNewOutBoundRecord.Visible = false;
                }

                if (!UIManager.UserManager.IsAuthorizedFunctions("PROTO_OWN"))
                {
                    if (InternalRecordEnable)
                        LiNewInternalRecord.Visible = false;
                }
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_NUOVODOC"))
            {
                LiNewDocument.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_DOC_PREDISPONI"))
            {
                if (!UIManager.UserManager.IsAuthorizedFunctions("DO_PRO_PREDISPONI"))
                {
                    LiNewPrepared.Visible = false;
                }
                else
                {
                    if (!UIManager.UserManager.IsAuthorizedFunctions("DO_PROT_PROTOCOLLA"))
                    {
                        LiNewPrepared.Visible = false;
                    }
                }
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("IMP_DOCS"))
            {
                LiMenuImportDoc.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("IMP_DOC_PREG"))
            {
                LiMenuImportDocPre.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("IMP_RDE"))
            {
                LiMenuImportRDE.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("IMP_DOCS") && !UIManager.UserManager.IsAuthorizedFunctions("IMP_DOC_PREG") && !UIManager.UserManager.IsAuthorizedFunctions("IMP_RDE") && !UIManager.UserManager.IsAuthorizedFunctions("IMPORTA_FATTURE"))
            {
                LiMenuImport.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_SU"))
            {
                LiMenuImportMailMerge.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_CERCA"))
            {
                LiMenuSearchDoc.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_GESTIONE"))
            {
                LiMenuSearchProject.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("TRAS_CERCA"))
            {
                LiMenuSearchTransmissions.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_RIC_VISIBILITA"))
            {
                LiMenuSearchVisibility.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_RIC_CAMPI_COMUNI"))
            {
                LiMenuSearchCommonFields.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_AREA_LAV"))
            {
                LiMenuSearchAdlDoc.Visible = false;
                LiMenuSearchAdlProject.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("IMP_FASC"))
            {
                LiMenuImportProject.Visible = false;
            }


            if (!UIManager.UserManager.IsAuthorizedFunctions("IMP_FASC") && !UIManager.UserManager.IsAuthorizedFunctions("FASC_GESTIONE"))
            {
                lifascicolo.Visible = false;
            }


            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_REGISTRI"))
            {
                LiMenuRegistry.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_REGISTRO_REPERTORIO"))
            {
                LiMenuRepertory.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_STAMPE"))
            {
                LiMenuPrints.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_PROSPETTI"))
            {
                LiMenuProspects.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_RUBRICA"))
            {
                LiMenuAddressBook.Visible = false;
            }

            //if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_ORGANIGRAMMA"))
            //{
            //    LiMenuList.Visible = false;
            //}

            //if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_DELEGHE"))
            //{
            //    LiMenuDelegate.Visible = false;
            //}

            if (!UIManager.UserManager.IsAuthorizedFunctions("ELENCO_NOTE"))
            {
                LiMenuNotes.Visible = false;
            }

            // INTEGRAZIONE PITRE-PARER
            // Se nell'amministrazione è configurato l'invio in conservazione al sistema SACER
            // deve essere impedito l'accesso al Centro Servizi
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_CONS") || IsConservazioneSACER)
            {
                LiMenuConservationArea.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_DOC_CESTINO"))
            {
                LiMenuDocumentsRemoved.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_ORGANIGRAMMA"))
            {
                LiMenuOrganizationChart.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_REP_SPED"))
            {
                LiMenuSendingReport.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_REGISTRI") &&
                !UIManager.UserManager.IsAuthorizedFunctions("GEST_REGISTRO_REPERTORIO") &&
                !UIManager.UserManager.IsAuthorizedFunctions("GEST_STAMPE") &&
                !UIManager.UserManager.IsAuthorizedFunctions("GEST_PROSPETTI") &&
                !UIManager.UserManager.IsAuthorizedFunctions("GEST_RUBRICA") &&
                !UIManager.UserManager.IsAuthorizedFunctions("GEST_ORGANIGRAMMA") &&
                !UIManager.UserManager.IsAuthorizedFunctions("GEST_DELEGHE") &&
                !UIManager.UserManager.IsAuthorizedFunctions("GEST_ELENCO_NOTE") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_CONS") &&
                !UIManager.UserManager.IsAuthorizedFunctions("GEST_DOC_CESTINO") &&
                !UIManager.UserManager.IsAuthorizedFunctions("GEST_REP_SPED") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_GES_VER") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_VER_APP") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_VER_ESE") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_GES_AUT") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_NEW_AUT") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_MOD_AUT") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_OLD_AUT") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_GES_SCA") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_SCA_APP") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_SCA_ESE"))
            {
                LiMenuManagement.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("OPZIONI_CAMBIA_PWD"))
            {
                LiMenuOptions.Visible = false;
                LiMenuChangePassword.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_NUOVO"))
            {
                liNuovoFascicolo.Visible = false;
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("CERCA_DOC_ADV"))
            {
                LinkSearchDoc.Attributes.Add("href", ResolveUrl("~/Search/SearchDocumentAdvanced.aspx"));
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_TRASMISSIONI"))
            {
                LiMenuManagementTransmission.Visible = false;
            }

            //Deposito 11042013:
            //Microfunzioni Deposito:
            /*
            DO_DEP_RIC_VER	Abilita il sottomenu Versamenti del menu Ricerca
            DO_DEP_RIC_SCA	Abilita il sottomenu Scarti del menu Ricerca
            DO_DEP_GES_VER	Abilita il sottomenu Area Pre-Deposito del menu Gestione
            DO_DEP_VER_APP	Abilita il pulsante Approva nel sottomenu Area Pre-Deposito
            DO_DEP_VER_ESE	Abilita il pulsante Esegui nel sottomenu Area Pre-Deposito
            DO_DEP_GES_AUT	Abilita il sottomenu Autorizzazioni Consultazione del menu Gestione
            DO_DEP_NEW_AUT	Abilita la Gestione nuove autorizzazioni del sottomenu Autorizzazioni Consultazione
            DO_DEP_MOD_AUT	Abilita la modifica delle  autorizzazioni concesse del sottomenu Autorizzazioni Consultazione
            DO_DEP_OLD_AUT	Abilita la Ricerca autorizzazioni concesse nel sottomenu Autorizzazioni Consultazione
            DO_DEP_GES_SCA  Abilita il sottomenu Selezione e scarto del menu Gestione
            DO_DEP_SCA_APP	Abilita il pulsante Approva nel sottomenu Selezione e scarto
            DO_DEP_SCA_ESE	Abilita il pulsante Esegui nel sottomenu Selezione e scarto
             */
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_RIC_VER"))
            {
                //Abilita il sottomenu Versamenti del menu Ricerca
                LiMenuSearchVersamenti.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_RIC_SCA"))
            {
                //Abilita il sottomenu Scarti del menu Ricerca
                LiMenuSearchScarti.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_GES_VER") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_VER_APP") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_VER_ESE"))
            {
                //Abilita il sottomenu Area Pre-Deposito del menu Gestione DO_DEP_VER_APP DO_DEP_VER_ESE
                LiMenuPreDeposito.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_GES_AUT") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_NEW_AUT") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_MOD_AUT") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_OLD_AUT"))
            {
                //Abilita il sottomenu Autorizzazioni Consultazione del menu Gestione
                LiMenuAutorizzazioneConsultazione.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_GES_SCA") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_SCA_APP") &&
                !UIManager.UserManager.IsAuthorizedFunctions("DO_DEP_SCA_ESE"))
            {
                //Abilita il sottomenu Selezione e scarto del menu Gestione
                //Iacozzilli: in collaudo no-link-scarto.
                LiMenuScarto.Visible = false;
            }

            if (string.IsNullOrEmpty(UIManager.AdministrationManager.GetNews(UserManager.GetInfoUser().idAmministrazione)) ||
               (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_DISABLED_CODICE_IPA.ToString()))
                && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_DISABLED_CODICE_IPA.ToString()).Equals("1")))
            {
                LiMenuCodeIpa.Visible = false;
            }


            if (!UIManager.UserManager.IsAuthorizedFunctions("INST_ACC"))
            {
                LiMenuInstAcc.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("GEST_PROCESSI_DI_FIRMA"))
            {
                LiMenuSignatureProcesses.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_START_SIGNATURE_PROCESS"))
            {
                LiMenuMonitoringProcesses.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("IMPORTA_FATTURE"))
            {
                LiMenuImportInvoice.Visible = false;
            }
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_ENABLE_BIGFILE"))
            {
                LiMenuClientBigfile.Visible = false;
            }
            if(!UIManager.UserManager.IsAuthorizedFunctions("DO_REGISTRO_ACCESSI_EXPORT") && !UIManager.UserManager.IsAuthorizedFunctions("DO_REGISTRO_ACCESSI_PUBLISH"))
            {
                LiMenuReportAccessi.Visible = false;
            }

            //se è attiva la chiave litedocument non mostro i seguenti menu
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
            {
                LiMenuImportProject.Visible = false;
                LiMenuSearchVisibility.Visible = false;
                LiMenuSearchCommonFields.Visible = false;
            }
            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ENABLE_FORMAZIONE") &&
                !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ENABLE_FORMAZIONE.ToString()]) &&
                System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ENABLE_FORMAZIONE.ToString()].Equals("1"))
            {
                LiMenuFormazione.Visible = true;
            }
        }

        protected void InitializesPage()
        {
            IdMasterBody.Attributes.Add("onload", "initDate();");
            if (Request.QueryString["back"] != null && !string.IsNullOrEmpty(Request.QueryString["back"]) && Request.QueryString["back"].ToString().Equals("1"))
            {
                IsBack = true;
            }
            SetIpAddress();
            InitializesLabels();
            InitializeCssText();
            LoadKeys();
            InitializeGuide();
            InitializeLastDocumentsView();
        }

        public void InitializeLastDocumentsView()
        {
            if (LitLastDocumentsView.Visible)
            {
                List<DocumentoVisualizzato> lastDocumentsView = null;
                if ((System.IO.Path.GetFileName(HttpContext.Current.Request.FilePath).ToUpper().ToString().Equals("DOCUMENT.ASPX")
                    || LastDocumentsView == null || LastDocumentsView.Count == 0))
                {
                    lastDocumentsView = DocumentManager.GetLastDocumentsView();
                    LastDocumentsView = lastDocumentsView;
                }
                else
                {
                    lastDocumentsView = LastDocumentsView;
                }
                if (lastDocumentsView != null && lastDocumentsView.Count > 0)
                {
                    foreach (DocumentoVisualizzato doc in lastDocumentsView)
                    {
                        HtmlGenericControl anchor = new HtmlGenericControl("a");
                        anchor.Attributes.Add("href", "#");
                        anchor.InnerText = (string.IsNullOrEmpty(doc.Segnatura) ? doc.IdProfile : doc.Segnatura) + " - " + utils.FormatJs(utils.FormatHtml(doc.Oggetto));
                        anchor.Attributes.Add("onclick", "$('#HiddenIdProfile').val('" + doc.IdProfile + "');__doPostBack('upPnlInfoUser');return false;");

                        LinkButton lnk = new LinkButton();
                        lnk.Attributes.Add("href", "#");
                        lnk.Text = (string.IsNullOrEmpty(doc.Segnatura) ? doc.IdProfile : doc.Segnatura) + " - " + utils.FormatJs(utils.FormatHtml(doc.Oggetto));
                        lnk.ToolTip = doc.Oggetto;
                        //lnk.CssClass = "clickableRight";
                        lnk.Attributes.Add("onclick", "$('#HiddenIdProfile').val('" + doc.IdProfile + "');__doPostBack('upPnlInfoUser');return false;");

                        HtmlGenericControl li = new HtmlGenericControl("li");
                        li.ID = doc.IdProfile + doc.System_id;
                        li.Attributes.Add("class", "li_2_a");
                        li.Controls.Add(lnk);

                        this.lastDocumentsView.Controls.Add(li);
                    }
                    //UpnlLastDocumentView.Update();
                }
            }       
        }

        private void InitializeGuide()
        {
            string callingPage = System.IO.Path.GetFileName(HttpContext.Current.Request.FilePath);

            if (!string.IsNullOrEmpty(callingPage))
            {
                string linkGuide = ResolveUrl("~/Guide/Guide.aspx");
                switch (callingPage.ToUpper().ToString())
                {
                    case "INDEX.ASPX":
                        Guide.Url = linkGuide + "?from=Home";
                        break;
                    case "DOCUMENT.ASPX":
                        Guide.Url = linkGuide + "?from=NuovoDoc";
                        break;
                    case "IMPORTDOCUMENTS.ASPX":
                        if (Request.QueryString["MailMerge"] != null && Request.QueryString["MailMerge"] == "true")
                        {
                            Guide.Url = linkGuide + "?from=ImpStampaUnione";
                        }
                        else
                        {
                            Guide.Url = linkGuide + "?from=ImpDoc";
                        }
                        break;
                    case "IMPORTPREVIOUS.ASPX":
                        Guide.Url = linkGuide + "?from=ImpDocPregressi";
                        break;
                    case "IMPORTRDE.ASPX":
                        Guide.Url = linkGuide + "?from=ImpRDE";
                        break;
                    case "PROJECT.ASPX":
                        Guide.Url = linkGuide + "?from=NuovoFasc";
                        break;
                    case "IMPORTPROJECTS.ASPX":
                        Guide.Url = linkGuide + "?from=ImpFasc";
                        break;
                    case "SEARCHDOCUMENT.ASPX":
                    case "SEARCHDOCUMENTADVANCED.ASPX":
                        Guide.Url = linkGuide + "?from=RicercaDoc";
                        if (Request.QueryString["IsAdl"] != null)
                        {
                            bool isAdl = Request.QueryString["IsAdl"] != null ? true : false;
                            if (isAdl)
                            {
                                Guide.Url = linkGuide + "?from=RicercaADLDoc";
                            }
                        }
                        break;
                    case "SEARCHPROJECT.ASPX":
                        Guide.Url = linkGuide + "?from=RicercaFasc";
                        if (Request.QueryString["IsAdl"] != null)
                        {
                            bool isAdl = Request.QueryString["IsAdl"] != null ? true : false;
                            if (isAdl)
                            {
                                Guide.Url = linkGuide + "?from=RicercaADLFasc";
                            }
                        }
                        break;
                    case "SEARCHTRANSMISSION.ASPX":
                        Guide.Url = linkGuide + "?from=RicercaTrasm";
                        break;
                    case "SEARCHVISIBILITY.ASPX":
                        Guide.Url = linkGuide + "?from=RicercaVis";
                        break;
                    case "REGISTERS.ASPX":
                        Guide.Url = linkGuide + "?from=GestioneReg";
                        break;
                    case "REGISTERREPERTORIES.ASPX":
                        Guide.Url = linkGuide + "?from=GestioneRegRepertorio";
                        break;
                    case "PRINTS.ASPX":
                        Guide.Url = linkGuide + "?from=GestioneStampe";
                        break;
                    case "SUMMERIES.ASPX":
                        Guide.Url = linkGuide + "?from=GestioneProspetti";
                        break;
                    //case "SUMMERIES.ASPX":
                    //    Guide.Url = linkGuide + "?from=GestioneRubrica";
                    //    break;
                    case "DISTRIBUTIONLIST.ASPX":
                        Guide.Url = linkGuide + "?from=GestioneListe";
                        break;
                    case "MANDATE.ASPX":
                        Guide.Url = linkGuide + "?from=GestioneDeleghe";
                        break;
                    case "GESTIONEMODELLITRASM.ASPX":
                        Guide.Url = linkGuide + "?from=GestioneModelliTrasm";
                        break;
                    case "NOTES.ASPX":
                        Guide.Url = linkGuide + "?from=GestioneListaNote";
                        break;
                    //case "NOTES.ASPX":
                    //    Guide.Url = linkGuide + "?from=GestioneReportSpedizioni";
                    //    break;
                    case "CONSERVATIONAREA.ASPX":
                        Guide.Url = linkGuide + "?from=GestioneAreaConserv";
                        break;
                    case "DOCUMENTSREMOVED.ASPX":
                        Guide.Url = linkGuide + "?from=GestioneDocRimossi";
                        break;
                    case "ORGANIZATIONCHART.ASPX":
                        Guide.Url = linkGuide + "?from=GestioneOrganigramma";
                        break;
                    case "CHANGEPASSWORD.ASPX":
                        Guide.Url = linkGuide + "?from=Opzioni";
                        break;
                    case "INSTANCEACCESS.ASPX":
                        Guide.Url = linkGuide + "?from=GestioneIstanzeDiAccesso";
                        break;
                    case "SIGNATUREPROCESSES.ASPX":
                        Guide.Url = linkGuide + "?from=GestioneProcessiDiFirma";
                        break;
                    case "MONITORINGPROCESSES.ASPX":
                        Guide.Url = linkGuide + "?from=GestioneMonitoraggioProcessi";
                        break;
                    default:
                        Guide.Url = linkGuide;
                        break;

                }
            }

        }

        private void LoadKeys()
        {
            LinkSearchDoc.Attributes.Add("href", ResolveUrl("~/Search/SearchDocument.aspx"));
            LinkSearchDoc.Attributes.Add("onclick", "disallowOp('menuTop')");

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
            {
                LinkSearchDoc.Attributes.Add("href", ResolveUrl("~/Search/SearchDocumentAdvanced.aspx"));
            }


            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_STAMPA_UNIONE.ToString())))
            {
                LiMenuImportMailMerge.Visible = false;
            }
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.BE_NUM_ULTIMI_DOC_VISUALIZZATI.ToString()))
                && !Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.BE_NUM_ULTIMI_DOC_VISUALIZZATI.ToString()).Equals("0"))
            {
                LitLastDocumentsView.Visible = true;
            }
            if(!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.BE_ENABLE_PORTALE_PROCEDIMENTI.ToString()))
                && Utils.InitConfigurationKeys.GetValue("0", DBKeys.BE_ENABLE_PORTALE_PROCEDIMENTI.ToString()).Equals("1"))
            {
                LiMenuProceedings.Visible = true;
            }
            else
            {
                LiMenuProceedings.Visible = false;
            }
        }

        protected void InitializeCssText()
        {
            if (!string.IsNullOrEmpty(UIManager.CssManager.GetSizeText()))
            {
                LnkSize1.Attributes.Remove("class");
                LnkSize2.Attributes.Remove("class");
                LnkSize3.Attributes.Remove("class");
                IdMasterBody.Attributes.Remove("class");
                if (UIManager.CssManager.GetSizeText().Equals(UIManager.CssManager.TextSize.NORMAL.ToString()))
                {
                    LnkSize1.Attributes.Add("class", "size1Sel");
                    LnkSize2.Attributes.Add("class", "size2");
                    LnkSize3.Attributes.Add("class", "size3");
                    IdMasterBody.Attributes.Add("class", "body_normal");
                }
                else
                {
                    if (UIManager.CssManager.GetSizeText().Equals(UIManager.CssManager.TextSize.MEDIUM.ToString()))
                    {
                        LnkSize2.Attributes.Add("class", "size2Sel");
                        LnkSize1.Attributes.Add("class", "size1");
                        LnkSize3.Attributes.Add("class", "size3");
                        IdMasterBody.Attributes.Add("class", "body_medium");
                    }
                    else
                    {
                        LnkSize3.Attributes.Add("class", "size3Sel");
                        LnkSize1.Attributes.Add("class", "size1");
                        LnkSize2.Attributes.Add("class", "size2");
                        IdMasterBody.Attributes.Add("class", "body_high");
                    }
                }
            }
            else
            {
                LnkSize1.Attributes.Add("class", "size1Sel");
                LnkSize2.Attributes.Add("class", "size2");
                LnkSize3.Attributes.Add("class", "size3");
                IdMasterBody.Attributes.Add("class", "body_normal");
            }
        }

        protected void LinkUrlNavigation_Click(object sender, EventArgs e)
        {
            ListObjectNavigation = null;
            List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
            string callingPage = System.IO.Path.GetFileName(HttpContext.Current.Request.FilePath);
            string linkPage = string.Empty;
            Navigation.NavigationObject obj = navigationList.Last();
            if (!string.IsNullOrEmpty(callingPage))
            {
                if (callingPage.ToUpper().Equals(obj.Page))
                {
                    if (obj.CodePage == Navigation.NavigationUtils.NamePage.DOCUMENT.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.DOCUMENT_CLASSIFICATION.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.DOCUMENT_ATTACHMENT.ToString())
                    {
                        obj = navigationList.Last();
                    }
                    else
                    {
                        obj = navigationList.ElementAt(navigationList.Count - 2);
                    }
                }
                else
                {
                    obj = navigationList.Last();
                }
            }

            if (obj.CodePage == Navigation.NavigationUtils.NamePage.DOCUMENT.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.DOCUMENT_CLASSIFICATION.ToString()
                || obj.CodePage == Navigation.NavigationUtils.NamePage.DOCUMENT_ATTACHMENT.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.DOCUMENT_TRANSMISSIONS.ToString())
            {
                if (!string.IsNullOrEmpty(obj.IdObject))
                {
                    SchedaDocumento doc = UIManager.DocumentManager.getDocumentDetails(Page, obj.IdObject, obj.IdObject);
                    UIManager.DocumentManager.setSelectedRecord(doc);
                    if (doc != null && doc.documenti != null)
                    {
                        FileManager.setSelectedFile(doc.documenti[0]);
                    }
                }
            }

            if (obj.CodePage == Navigation.NavigationUtils.NamePage.PROJECT.ToString())
            {
                if (!string.IsNullOrEmpty(obj.idProject))
                {
                    Fascicolo prj = UIManager.ProjectManager.getFascicoloById(obj.idProject, UIManager.UserManager.GetInfoUser());
                    prj.template = UIManager.ProfilerProjectManager.getTemplateFascDettagli(obj.idProject);
                    if (obj.folder != null)
                        prj.folderSelezionato = obj.folder;
                    UIManager.ProjectManager.setProjectInSession(prj);
                }
                else
                {
                    if (!string.IsNullOrEmpty(obj.IdObject))
                    {
                        Fascicolo prj = UIManager.ProjectManager.getFascicoloById(obj.IdObject, UIManager.UserManager.GetInfoUser());
                        prj.template = UIManager.ProfilerProjectManager.getTemplateFascDettagli(obj.IdObject);
                        if (obj.folder != null)
                            prj.folderSelezionato = obj.folder;
                        UIManager.ProjectManager.setProjectInSession(prj);
                    }
                }
            }

            Response.Redirect(Navigation.NavigationUtils.GetLink(obj.CodePage, true, Page));
        }

        private void ViewRecentDocumentSelected(string idProfile)
        {
            if (!string.IsNullOrEmpty(idProfile))
            {
                SchedaDocumento schedaDoc = new SchedaDocumento();
                schedaDoc.systemId = idProfile;
                DocumentManager.setSelectedRecord(schedaDoc);
                if (DocumentManager.CheckRevocationAcl())
                {
                    DocumentManager.setSelectedRecord(null);
                    ScriptManager.RegisterStartupScript(Page, typeof(Page), "ajaxDialogModal", "ajaxDialogModal('RevocationAclIndex', 'warning');", true);
                    return;
                }
                else
                {
                    schedaDoc = UIManager.DocumentManager.getDocumentDetails(Page, idProfile, idProfile);
                    if (schedaDoc.inCestino.Equals("1"))
                    {
                        DocumentManager.setSelectedRecord(null);
                        ScriptManager.RegisterStartupScript(Page, typeof(Page), "ajaxDialogModal", "ajaxDialogModal('WarningBaseMasterRemovedDocument', 'warning');", true);
                        return;
                    }
                    DocumentManager.setSelectedRecord(schedaDoc);
                    //List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                    //Navigation.NavigationObject objectPage = navigationList.Last();
                    //objectPage.ViewResult = true;

                    //ScriptManager.RegisterStartupScript(Page, GetType(), "setReturnValue", "document.location.href = '" + Page.ResolveUrl("~/Document/Document.aspx") + "';", true);
                    Response.Redirect(ResolveUrl("~/Document/Document.aspx"));
                }
            }
        }

        protected void NextNavigation_Click(object sender, EventArgs e)
        {
            List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
            string callingPage = System.IO.Path.GetFileName(HttpContext.Current.Request.FilePath);
            string linkPage = string.Empty;
            Navigation.NavigationObject obj = navigationList.Last();
            Navigation.NavigationObject objNew = new Navigation.NavigationObject();
            SearchObject[] toReturn;
            SearchObject search;
            int pageNumbers;
            int recordNumber;
            string type = string.Empty;
            string link = string.Empty;
            DocsPaWR.Trasmissione[] listaTrasmissioni = null;
            DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPaWR.TrasmissioneOggettoTrasm();

            // Lista dei system id dei fascicoli restituiti dalla ricerca
            SearchResultInfo[] idProjects;
            #region DOCUMENTO
            // DOCUMENTO
            if (obj.CodePage == Navigation.NavigationUtils.NamePage.DOCUMENT.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_SIMPLE.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED_ADL.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_PRINTS.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.HOME_ADL_DOCUMENT.ToString())
            {
                int requestedPage = Int32.Parse(obj.NumPage);
                int indexElement = Int32.Parse(obj.DxPositionElement);
                int intObject = indexElement + 1;
                if (obj.DxPositionElement.Equals(obj.PageSize))
                {
                    requestedPage = requestedPage + 1;
                    indexElement = 0;
                }
                else
                {
                    indexElement = (indexElement - (Int32.Parse(obj.PageSize) * (requestedPage - 1)));
                    if (indexElement.ToString().Equals(obj.PageSize))
                    {
                        requestedPage = requestedPage + 1;
                        indexElement = 0;
                    }
                }

                if (obj.folder == null)
                {
                    //Search document
                    // Recupero dei campi della griglia impostati come visibili
                    Field[] visibleArray = null;
                    List<Field> visibleFieldsTemplate;

                    if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Document)
                        GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);

                    visibleFieldsTemplate = GridManager.SelectedGrid.Fields.Where(ev => ev.Visible && ev.GetType().Equals(typeof(Field)) && ev.CustomObjectId != 0).ToList();

                    if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
                    {
                        visibleArray = visibleFieldsTemplate.ToArray();
                    }
                    if (ListObjectNavigation == null || !obj.NumPage.Equals(requestedPage.ToString()))
                    {
                        toReturn = DocumentManager.getQueryInfoDocumentoPagingCustom(UIManager.UserManager.GetInfoUser(), Page, obj.SearchFilters, requestedPage, out pageNumbers, out recordNumber, true, false, UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION"), Int32.Parse(obj.PageSize), false, visibleArray, null, out idProjects);
                        if (toReturn == null || toReturn.Count() == 0)
                        {
                            requestedPage = requestedPage - 1;
                            toReturn = DocumentManager.getQueryInfoDocumentoPagingCustom(UIManager.UserManager.GetInfoUser(), Page, obj.SearchFilters, requestedPage, out pageNumbers, out recordNumber, true, false, UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION"), Int32.Parse(obj.PageSize), false, visibleArray, null, out idProjects);
                            //indexElement = (indexElement - ((Int32.Parse(obj.DxTotalNumberElement) - recordNumber))) -1;
                            intObject = indexElement + 1;
                        }
                        ListObjectNavigation = toReturn;
                    }
                    else
                    {
                        toReturn = ListObjectNavigation;
                        recordNumber = Convert.ToInt32(obj.DxTotalNumberElement);
                    }
                }
                else
                {
                    //Search document in project
                    if (ListObjectNavigation == null || !obj.NumPage.Equals(requestedPage.ToString()))
                    {
                        toReturn = UIManager.ProjectManager.getListaDocumentiPagingCustom(
                            obj.folder,
                            obj.SearchFilters,
                            requestedPage,
                            out pageNumbers,
                            out recordNumber,
                            false,
                            UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION"),
                            false,
                            null,
                            null,
                            Int32.Parse(obj.PageSize),
                            obj.SearchFiltersOrder,
                            out idProjects);
                        ListObjectNavigation = toReturn;
                    }
                    else
                    {
                        toReturn = ListObjectNavigation;
                        recordNumber = Convert.ToInt32(obj.DxTotalNumberElement);
                    }
                    //pageNumbers = pageNumbers + 1;
                }

                pageNumbers = (int)Math.Round(((double)recordNumber / double.Parse(obj.PageSize)) + 0.49);

                search = toReturn[indexElement];
                

                objNew = Navigation.NavigationUtils.CloneObject(obj);

                objNew.IdObject = search.SearchObjectID;
                objNew.NumPage = requestedPage.ToString();
                objNew.DxTotalPageNumber = pageNumbers.ToString();
                objNew.DxTotalNumberElement = recordNumber.ToString();
                objNew.DxPositionElement = (intObject).ToString();

                navigationList.Remove(obj);
                navigationList.Add(objNew);

                Navigation.NavigationUtils.SetNavigationList(navigationList);

                if (!string.IsNullOrEmpty(search.SearchObjectID))
                {
                    SchedaDocumento doc = DocumentManager.getDocumentDetails(Page, search.SearchObjectID, search.SearchObjectID);

                    UIManager.DocumentManager.setSelectedRecord(doc);
                }

                link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), false, Page);

            }
            #endregion

            #region FASCICOLO
            // FASCICOLO
            if (obj.CodePage == Navigation.NavigationUtils.NamePage.PROJECT.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS_WA.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.HOME_ADL_PROJECT.ToString())
            {
                int requestedPage = Int32.Parse(obj.NumPage);
                int indexElement = Int32.Parse(obj.DxPositionElement);
                int intObject = indexElement + 1;
                if (obj.DxPositionElement.Equals(obj.PageSize))
                {
                    requestedPage = requestedPage + 1;
                    indexElement = 0;
                }
                else
                {
                    indexElement = (indexElement - (Int32.Parse(obj.PageSize) * (requestedPage - 1)));
                    if (indexElement.ToString().Equals(obj.PageSize))
                    {
                        requestedPage = requestedPage + 1;
                        indexElement = 0;
                    }
                }

                if (obj.folder == null)
                {
                    //if (obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString())
                    //{
                    //Search project

                    if (obj.CodePage == Navigation.NavigationUtils.NamePage.PROJECT.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString())
                    {

                        navigationList.Remove(obj);
                        obj = navigationList.Last();
                        requestedPage = Int32.Parse(obj.NumPage);
                        indexElement = Int32.Parse(obj.DxPositionElement);
                        intObject = indexElement + 1;
                        if (obj.DxPositionElement.Equals(obj.PageSize))
                        {
                            requestedPage = requestedPage + 1;
                            indexElement = 0;
                        }
                        else
                        {
                            indexElement = (indexElement - (Int32.Parse(obj.PageSize) * (requestedPage - 1)));
                        }
                    }

                    if (ListObjectNavigation == null || !obj.NumPage.Equals(requestedPage.ToString()))
                    {
                        toReturn = ProjectManager.getListaFascicoliPagingCustom(obj.Classification, obj.RegistryFilter, obj.SearchFilters[0], false, requestedPage, out pageNumbers, out recordNumber, Int32.Parse(obj.PageSize), false, out idProjects, null, true, false, null, null, true);
                        ListObjectNavigation = toReturn;
                    }
                    else
                    {
                        toReturn = ListObjectNavigation;
                        recordNumber = Convert.ToInt32(obj.DxTotalNumberElement);
                    }
                    type = "F";
                    //}
                    //else
                    //{
                    //    //Search document in project
                    //    toReturn = UIManager.ProjectManager.getListaDocumentiPagingCustom(
                    //  obj.folder,
                    //  obj.SearchFilters,
                    //  requestedPage,
                    //  out pageNumbers,
                    //  out recordNumber,
                    //  false,
                    //  UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION"),
                    //  false,
                    //  null,
                    //  null,
                    //  Int32.Parse(obj.PageSize),
                    //  obj.SearchFiltersOrder,
                    //  out idProjects);
                    //    pageNumbers = pageNumbers + 1;
                    //    type = "D";
                    //}
                }
                else
                {
                    //if (obj.CodePage == Navigation.NavigationUtils.NamePage.PROJECT.ToString())
                    //{
                    //    //Search project
                    //    toReturn = ProjectManager.getListaFascicoliPagingCustom(obj.Classification, obj.RegistryFilter, obj.SearchFilters[0], false, requestedPage, out pageNumbers, out recordNumber, Int32.Parse(obj.PageSize), false, out idProjects, null, true, false, null, null, true);
                    //    type = "F";
                    //}
                    //else
                    //{
                    //Search document in project

                    // INC000000598211 - PITRE - errore in navigazione documenti fascicoli
                    // Recupero dei campi della griglia impostati come visibili (come in ricerca documenti)
                    Field[] visibleArray = null;
                    List<Field> visibleFieldsTemplate;

                    if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Document)
                        GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);

                    visibleFieldsTemplate = GridManager.SelectedGrid.Fields.Where(ev => ev.Visible && ev.GetType().Equals(typeof(Field)) && ev.CustomObjectId != 0).ToList();
                    if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
                    {
                        visibleArray = visibleFieldsTemplate.ToArray();
                    }

                    if (ListObjectNavigation == null || !obj.NumPage.Equals(requestedPage.ToString()))
                    {
                        toReturn = UIManager.ProjectManager.getListaDocumentiPagingCustom(
                            obj.folder,
                            obj.SearchFilters,
                            requestedPage,
                            out pageNumbers,
                            out recordNumber,
                            false,
                            UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION"),
                            false,
                            visibleArray,
                            null,
                            Int32.Parse(obj.PageSize),
                            obj.SearchFiltersOrder,
                            out idProjects);
                        ListObjectNavigation = toReturn;
                    }
                    else
                    {
                        toReturn = ListObjectNavigation;
                        recordNumber = Convert.ToInt32(obj.DxTotalNumberElement);
                    }
                    //pageNumbers = pageNumbers + 1;
                    type = "D";
                    //}
                }

                pageNumbers = (int)Math.Round(((double)recordNumber / double.Parse(obj.PageSize)) + 0.49);

                search = toReturn[indexElement];

                objNew = Navigation.NavigationUtils.CloneObject(obj);

                objNew.IdObject = search.SearchObjectID;
                objNew.NumPage = requestedPage.ToString();
                objNew.DxTotalPageNumber = pageNumbers.ToString();
                objNew.DxTotalNumberElement = recordNumber.ToString();
                objNew.DxPositionElement = (intObject).ToString();

                navigationList.Remove(obj);
                navigationList.Add(objNew);

                Navigation.NavigationUtils.SetNavigationList(navigationList);

                if (!string.IsNullOrEmpty(type) && type.Equals("F"))
                {
                    if (!string.IsNullOrEmpty(search.SearchObjectID))
                    {
                        Fascicolo prj = UIManager.ProjectManager.getFascicoloById(search.SearchObjectID, UIManager.UserManager.GetInfoUser());
                        prj.template = UIManager.ProfilerProjectManager.getTemplateFascDettagli(search.SearchObjectID);
                        UIManager.ProjectManager.setProjectInSession(prj);
                    }

                    link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), false, Page);
                }
                else
                {
                    if (!string.IsNullOrEmpty(search.SearchObjectID))
                    {
                        SchedaDocumento doc = DocumentManager.getDocumentDetails(Page, search.SearchObjectID, search.SearchObjectID);
                        DocumentManager.setSelectedRecord(doc);
                    }

                    link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), false, Page);
                }

            }

            #endregion

            #region TRASMISSIONI
            // TRASMISSIONI
            if (obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_TRANSMISSIONS.ToString())
            {
                Trasmissione transm = null;
                int requestedPage = Int32.Parse(obj.NumPage);
                int indexElement = Int32.Parse(obj.DxPositionElement);
                int intObject = indexElement + 1;
                if (obj.DxPositionElement.Equals(obj.PageSize))
                {
                    requestedPage = requestedPage + 1;
                    indexElement = 0;
                }
                else
                {
                    indexElement = (indexElement - (Int32.Parse(obj.PageSize) * (requestedPage - 1)));
                    if (indexElement.ToString().Equals(obj.PageSize))
                    {
                        requestedPage = requestedPage + 1;
                        indexElement = 0;
                    }
                }

                if (obj.TypeTransmissionSearch.Equals("E"))
                {
                    if (ListTransmissionNavigation == null || !obj.NumPage.Equals(requestedPage.ToString()))
                    {
                        listaTrasmissioni = TrasmManager.getQueryEffettuatePagingLite(oggettoTrasm, obj.SearchFilters[0], requestedPage, false, Int32.Parse(obj.PageSize), out pageNumbers, out recordNumber);
                        ListTransmissionNavigation = listaTrasmissioni;
                    }
                    else
                    {
                        listaTrasmissioni = ListTransmissionNavigation;
                        recordNumber = Convert.ToInt32(obj.DxTotalNumberElement);
                    }
                }
                else
                {
                    if (ListTransmissionNavigation == null || !obj.NumPage.Equals(requestedPage.ToString()))
                    {
                        listaTrasmissioni = TrasmManager.getQueryRicevuteLite(oggettoTrasm, obj.SearchFilters[0], requestedPage, false, Int32.Parse(obj.PageSize), out pageNumbers, out recordNumber);
                        ListTransmissionNavigation = listaTrasmissioni;
                    }
                    else
                    {
                        listaTrasmissioni = ListTransmissionNavigation;
                        recordNumber = Convert.ToInt32(obj.DxTotalNumberElement);
                    }
                }

                pageNumbers = (int)Math.Round(((double)recordNumber / double.Parse(obj.PageSize)) + 0.49);

                transm = listaTrasmissioni[indexElement];

                objNew = Navigation.NavigationUtils.CloneObject(obj);

                objNew.IdObject = transm.systemId;
                objNew.NumPage = requestedPage.ToString();
                objNew.DxTotalPageNumber = pageNumbers.ToString();
                objNew.DxTotalNumberElement = recordNumber.ToString();
                objNew.DxPositionElement = (intObject).ToString();

                navigationList.Remove(obj);
                navigationList.Add(objNew);

                Navigation.NavigationUtils.SetNavigationList(navigationList);

                if (!string.IsNullOrEmpty(transm.systemId))
                {
                    if (transm.tipoOggetto == TrasmissioneTipoOggetto.DOCUMENTO)
                    {
                        SchedaDocumento doc = DocumentManager.getDocumentDetails(Page, transm.infoDocumento.idProfile, transm.infoDocumento.idProfile);
                        UIManager.DocumentManager.setSelectedRecord(doc);
                        link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), false, Page);
                    }
                    else
                    {
                        Fascicolo prj = UIManager.ProjectManager.getFascicoloById(transm.infoFascicolo.idFascicolo, UIManager.UserManager.GetInfoUser());
                        prj.template = UIManager.ProfilerProjectManager.getTemplateFascDettagli(transm.infoFascicolo.idFascicolo);
                        UIManager.ProjectManager.setProjectInSession(prj);
                        link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), false, Page);
                    }
                }
            }

            #endregion

            #region HOME
            // HOME
            if (obj.CodePage == Navigation.NavigationUtils.NamePage.HOME.ToString())
            {
                Notification notify = null;
                int requestedPage = Int32.Parse(obj.NumPage);

                int indexElement = Int32.Parse(obj.DxPositionElement);
                // INC000000299692 - Volani: non spostare questa riga sotto il calcolo della pagina, altrimenti viene preso l'index errato.
                notify = NotificationManager.ListNotifyFiltered[indexElement];

                int intObject = indexElement + 1;
                if (obj.DxPositionElement.Equals(obj.PageSize))
                {
                    requestedPage = requestedPage + 1;
                }
                else
                {
                    indexElement = (indexElement - (Int32.Parse(obj.PageSize) * (requestedPage - 1)));
                    if (indexElement.ToString().Equals(obj.PageSize))
                    {
                        requestedPage = requestedPage + 1;
                    }
                }

                objNew = Navigation.NavigationUtils.CloneObject(obj);

                objNew.IdObject = notify.ID_OBJECT;
                objNew.NumPage = requestedPage.ToString();
                objNew.DxTotalPageNumber = obj.DxTotalPageNumber;
                objNew.DxTotalNumberElement = obj.DxTotalNumberElement;
                objNew.DxPositionElement = (intObject).ToString();

                navigationList.Remove(obj);
                navigationList.Add(objNew);

                Navigation.NavigationUtils.SetNavigationList(navigationList);

                if (!string.IsNullOrEmpty(notify.ID_NOTIFY))
                {
                    if (notify.DOMAINOBJECT.Equals(FiltersNotifications.DOCUMENT))
                    {
                        SchedaDocumento doc = DocumentManager.getDocumentDetails(Page, notify.ID_OBJECT, notify.ID_OBJECT);
                        UIManager.DocumentManager.setSelectedRecord(doc);
                        link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), false, Page);
                    }
                    else
                    {
                        Fascicolo prj = UIManager.ProjectManager.getFascicoloById(notify.ID_OBJECT, UIManager.UserManager.GetInfoUser());
                        prj.template = UIManager.ProfilerProjectManager.getTemplateFascDettagli(notify.ID_OBJECT);
                        UIManager.ProjectManager.setProjectInSession(prj);
                        link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), false, Page);
                    }
                }
            }

            #endregion

            Response.Redirect(link);
        }

        protected void BackNavigation_Click(object sender, EventArgs e)
        {

            List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
            string callingPage = System.IO.Path.GetFileName(HttpContext.Current.Request.FilePath);
            string linkPage = string.Empty;
            Navigation.NavigationObject obj = navigationList.Last();
            Navigation.NavigationObject objNew = new Navigation.NavigationObject();
            SearchObject[] toReturn;
            SearchObject search;
            int pageNumbers;
            int recordNumber;
            string link = string.Empty;
            string type = string.Empty;
            DocsPaWR.Trasmissione[] listaTrasmissioni = null;
            DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPaWR.TrasmissioneOggettoTrasm();

            // Lista dei system id dei fascicoli restituiti dalla ricerca
            SearchResultInfo[] idProjects;


            // DOCUMENTO
            if (obj.CodePage == Navigation.NavigationUtils.NamePage.DOCUMENT.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_SIMPLE.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED_ADL.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_PRINTS.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.HOME_ADL_DOCUMENT.ToString())
            {
                int requestedPage = Int32.Parse(obj.NumPage);
                int indexElement = Int32.Parse(obj.DxPositionElement) - 1;
                int intObject = indexElement;
                if (indexElement.ToString().Equals(obj.PageSize))
                {
                    requestedPage = requestedPage - 1;
                    indexElement = Convert.ToInt32(obj.PageSize) -1;
                }
                else
                {
                    indexElement = (indexElement - (Int32.Parse(obj.PageSize) * (requestedPage - 1))) - 1;
                    if (indexElement == -1)
                    {
                        requestedPage = requestedPage - 1;
                        indexElement = Convert.ToInt32(obj.PageSize) - 1;
                    }
                }

                if (obj.folder == null)
                {
                    //Search document
                    // Recupero dei campi della griglia impostati come visibili
                    Field[] visibleArray = null;
                    List<Field> visibleFieldsTemplate;

                    if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Document)
                        GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);

                    visibleFieldsTemplate = GridManager.SelectedGrid.Fields.Where(ev => ev.Visible && ev.GetType().Equals(typeof(Field)) && ev.CustomObjectId != 0).ToList();

                    if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
                    {
                        visibleArray = visibleFieldsTemplate.ToArray();
                    }
                    if (ListObjectNavigation == null || !obj.NumPage.Equals(requestedPage.ToString()))
                    {
                        toReturn = DocumentManager.getQueryInfoDocumentoPagingCustom(UIManager.UserManager.GetInfoUser(), Page, obj.SearchFilters, requestedPage, out pageNumbers, out recordNumber, true, false, UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION"), Int32.Parse(obj.PageSize), false, visibleArray, null, out idProjects);
                        ListObjectNavigation = toReturn;
                    }
                    else
                    {
                        toReturn = ListObjectNavigation;
                        recordNumber = Convert.ToInt32(obj.DxTotalNumberElement);
                    }
                }
                else
                {
                    //Search document in project
                    if (ListObjectNavigation == null || !obj.NumPage.Equals(requestedPage.ToString()))
                    {
                        toReturn = UIManager.ProjectManager.getListaDocumentiPagingCustom(
                        obj.folder,
                        obj.SearchFilters,
                        requestedPage,
                        out pageNumbers,
                        out recordNumber,
                        false,
                        UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION"),
                        false,
                        null,
                        null,
                        Int32.Parse(obj.PageSize),
                        obj.SearchFiltersOrder,
                        out idProjects);
                        ListObjectNavigation = toReturn;
                    }
                    else
                    {
                        toReturn = ListObjectNavigation;
                        recordNumber = Convert.ToInt32(obj.DxTotalNumberElement);
                    }
                    //pageNumbers = pageNumbers + 1;
                }

                pageNumbers = (int)Math.Round(((double)recordNumber / double.Parse(obj.PageSize)) + 0.49);

                search = toReturn[indexElement];

                objNew = Navigation.NavigationUtils.CloneObject(obj);

                objNew.IdObject = search.SearchObjectID;
                objNew.NumPage = requestedPage.ToString();
                objNew.DxTotalPageNumber = pageNumbers.ToString();
                objNew.DxTotalNumberElement = recordNumber.ToString();
                objNew.DxPositionElement = (intObject).ToString();

                navigationList.Remove(obj);
                navigationList.Add(objNew);

                Navigation.NavigationUtils.SetNavigationList(navigationList);

                if (!string.IsNullOrEmpty(search.SearchObjectID))
                {
                    SchedaDocumento doc = DocumentManager.getDocumentDetails(Page, search.SearchObjectID, search.SearchObjectID);

                    UIManager.DocumentManager.setSelectedRecord(doc);
                }

                link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), false, Page);

            }


            // FASCICOLO
            if (obj.CodePage == Navigation.NavigationUtils.NamePage.PROJECT.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS_WA.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.HOME_ADL_PROJECT.ToString())
            {
                int requestedPage = Int32.Parse(obj.NumPage);
                int indexElement = Int32.Parse(obj.DxPositionElement) - 1;
                int intObject = indexElement;
                if (indexElement.ToString().Equals(obj.PageSize))
                {
                    requestedPage = requestedPage - 1;
                    indexElement = 0;
                }
                else
                {
                    indexElement = (indexElement - (Int32.Parse(obj.PageSize) * (requestedPage - 1))) - 1;
                    if (indexElement == -1)
                    {
                        requestedPage = requestedPage - 1;
                        indexElement = 0;
                    }
                }

                if (obj.folder == null)
                {
                    if (obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS_WA.ToString() || obj.CodePage == Navigation.NavigationUtils.NamePage.HOME_ADL_PROJECT.ToString())
                    {
                        //Search project
                        if (ListObjectNavigation == null || !obj.NumPage.Equals(requestedPage.ToString()))
                        {
                            toReturn = ProjectManager.getListaFascicoliPagingCustom(obj.Classification, obj.RegistryFilter, obj.SearchFilters[0], false, requestedPage, out pageNumbers, out recordNumber, Int32.Parse(obj.PageSize), false, out idProjects, null, true, false, null, null, true);
                            ListObjectNavigation = toReturn;
                        }
                        else
                        {
                            toReturn = ListObjectNavigation;
                            recordNumber = Convert.ToInt32(obj.DxTotalNumberElement);
                        }
                        type = "F";
                    }
                    else
                    {
                        //Search document in project
                        if (ListObjectNavigation == null || !obj.NumPage.Equals(requestedPage.ToString()))
                        {
                            toReturn = UIManager.ProjectManager.getListaDocumentiPagingCustom(
                              obj.folder,
                              obj.SearchFilters,
                              requestedPage,
                              out pageNumbers,
                              out recordNumber,
                              false,
                              UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION"),
                              false,
                              null,
                              null,
                              Int32.Parse(obj.PageSize),
                              obj.SearchFiltersOrder,
                              out idProjects);
                            ListObjectNavigation = toReturn;
                        }
                        else
                        {
                            toReturn = ListObjectNavigation;
                            recordNumber = Convert.ToInt32(obj.DxTotalNumberElement);
                        }
                        //pageNumbers = pageNumbers + 1;
                        type = "D";
                    }
                }
                else
                {
                    //Search document in project
                    if (ListObjectNavigation == null || !obj.NumPage.Equals(requestedPage.ToString()))
                    {
                        toReturn = UIManager.ProjectManager.getListaDocumentiPagingCustom(
                            obj.folder,
                            obj.SearchFilters,
                            requestedPage,
                            out pageNumbers,
                            out recordNumber,
                            false,
                            UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION"),
                            false,
                            null,
                            null,
                            Int32.Parse(obj.PageSize),
                            obj.SearchFiltersOrder,
                            out idProjects);
                        ListObjectNavigation = toReturn;
                    }
                    else
                    {
                        toReturn = ListObjectNavigation;
                        recordNumber = Convert.ToInt32(obj.DxTotalNumberElement);
                    }
                    //pageNumbers = pageNumbers + 1;
                    type = "D";
                }

                pageNumbers = (int)Math.Round(((double)recordNumber / double.Parse(obj.PageSize)) + 0.49);

                search = toReturn[indexElement];

                objNew = Navigation.NavigationUtils.CloneObject(obj);

                objNew.IdObject = search.SearchObjectID;
                objNew.NumPage = requestedPage.ToString();
                objNew.DxTotalPageNumber = pageNumbers.ToString();
                objNew.DxTotalNumberElement = recordNumber.ToString();
                objNew.DxPositionElement = (intObject).ToString();

                navigationList.Remove(obj);
                navigationList.Add(objNew);

                Navigation.NavigationUtils.SetNavigationList(navigationList);

                if (type.Equals("F"))
                {
                    if (!string.IsNullOrEmpty(search.SearchObjectID))
                    {
                        Fascicolo prj = UIManager.ProjectManager.getFascicoloById(search.SearchObjectID, UIManager.UserManager.GetInfoUser());
                        prj.template = UIManager.ProfilerProjectManager.getTemplateFascDettagli(search.SearchObjectID);
                        UIManager.ProjectManager.setProjectInSession(prj);
                    }

                    link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), false,Page);
                }
                else
                {
                    if (!string.IsNullOrEmpty(search.SearchObjectID))
                    {
                        SchedaDocumento doc = DocumentManager.getDocumentDetails(Page, search.SearchObjectID, search.SearchObjectID);
                        DocumentManager.setSelectedRecord(doc);
                    }

                    link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), false, Page);
                }

            }


            // TRASMISSIONI
            if (obj.CodePage == Navigation.NavigationUtils.NamePage.SEARCH_TRANSMISSIONS.ToString())
            {
                Trasmissione transm = null;
                int requestedPage = Int32.Parse(obj.NumPage);
                int indexElement = Int32.Parse(obj.DxPositionElement) - 1;
                int intObject = indexElement;
                if (indexElement.ToString().Equals(obj.PageSize))
                {
                    requestedPage = requestedPage - 1;
                    indexElement = 0;
                }
                else
                {
                    indexElement = (indexElement - (Int32.Parse(obj.PageSize) * (requestedPage - 1))) - 1;
                    if (indexElement == -1)
                    {
                        requestedPage = requestedPage - 1;
                        indexElement = 0;
                    }
                }

                if (obj.TypeTransmissionSearch.Equals("E"))
                {
                    if (ListTransmissionNavigation == null || !obj.NumPage.Equals(requestedPage.ToString()))
                    {
                        listaTrasmissioni = TrasmManager.getQueryEffettuatePagingLite(oggettoTrasm, obj.SearchFilters[0], requestedPage, false, Int32.Parse(obj.PageSize), out pageNumbers, out recordNumber);
                        ListTransmissionNavigation = listaTrasmissioni;
                    }
                    else
                    {
                        listaTrasmissioni = ListTransmissionNavigation;
                        recordNumber = Convert.ToInt32(obj.DxTotalNumberElement);
                    }

                }
                else
                {
                    if (ListTransmissionNavigation == null || !obj.NumPage.Equals(requestedPage.ToString()))
                    {
                        listaTrasmissioni = TrasmManager.getQueryRicevuteLite(oggettoTrasm, obj.SearchFilters[0], requestedPage, false, Int32.Parse(obj.PageSize), out pageNumbers, out recordNumber);
                        ListTransmissionNavigation = listaTrasmissioni;
                    }
                    else
                    {
                        listaTrasmissioni = ListTransmissionNavigation;
                        recordNumber = Convert.ToInt32(obj.DxTotalNumberElement);
                    }
                }

                pageNumbers = (int)Math.Round(((double)recordNumber / double.Parse(obj.PageSize)) + 0.49);

                transm = listaTrasmissioni[indexElement];

                objNew = Navigation.NavigationUtils.CloneObject(obj);

                objNew.IdObject = transm.systemId;
                objNew.NumPage = requestedPage.ToString();
                objNew.DxTotalPageNumber = pageNumbers.ToString();
                objNew.DxTotalNumberElement = recordNumber.ToString();
                objNew.DxPositionElement = (intObject).ToString();

                navigationList.Remove(obj);
                navigationList.Add(objNew);

                Navigation.NavigationUtils.SetNavigationList(navigationList);

                if (!string.IsNullOrEmpty(transm.systemId))
                {
                    if (transm.tipoOggetto == TrasmissioneTipoOggetto.DOCUMENTO)
                    {
                        SchedaDocumento doc = DocumentManager.getDocumentDetails(Page, transm.infoDocumento.idProfile, transm.infoDocumento.idProfile);
                        UIManager.DocumentManager.setSelectedRecord(doc);
                        link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), false, Page);
                    }
                    else
                    {
                        Fascicolo prj = UIManager.ProjectManager.getFascicoloById(transm.infoFascicolo.idFascicolo, UIManager.UserManager.GetInfoUser());
                        prj.template = UIManager.ProfilerProjectManager.getTemplateFascDettagli(transm.infoFascicolo.idFascicolo);
                        UIManager.ProjectManager.setProjectInSession(prj);
                        link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), false, Page);
                    }
                }
            }


            // HOME
            if (obj.CodePage == Navigation.NavigationUtils.NamePage.HOME.ToString())
            {
                Notification notify = null;
                int requestedPage = Int32.Parse(obj.NumPage);
                int indexElement = Int32.Parse(obj.DxPositionElement) - 1;
                // INC000000299692 - Volani: non spostare questa riga sotto il calcolo della pagina, altrimenti viene preso l'index errato.
                notify = NotificationManager.ListNotifyFiltered[indexElement - 1];

                int intObject = indexElement;

                if (indexElement.ToString().Equals(obj.PageSize))
                {
                    requestedPage = requestedPage - 1;
                }
                else
                {
                    indexElement = (indexElement - (Int32.Parse(obj.PageSize) * (requestedPage - 1))) - 1;
                    if (indexElement == -1)
                    {
                        requestedPage = requestedPage - 1;
                    }
                }                

                objNew = Navigation.NavigationUtils.CloneObject(obj);

                objNew.IdObject = notify.ID_OBJECT;
                objNew.NumPage = requestedPage.ToString();
                objNew.DxTotalPageNumber = obj.DxTotalPageNumber;
                objNew.DxTotalNumberElement = obj.DxTotalNumberElement;
                objNew.DxPositionElement = (intObject).ToString();

                navigationList.Remove(obj);
                navigationList.Add(objNew);

                Navigation.NavigationUtils.SetNavigationList(navigationList);

                if (!string.IsNullOrEmpty(notify.ID_NOTIFY))
                {
                    if (notify.DOMAINOBJECT.Equals(FiltersNotifications.DOCUMENT))
                    {
                        SchedaDocumento doc = DocumentManager.getDocumentDetails(Page, notify.ID_OBJECT, notify.ID_OBJECT);
                        UIManager.DocumentManager.setSelectedRecord(doc);
                        link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), false, Page);
                    }
                    else
                    {
                        Fascicolo prj = UIManager.ProjectManager.getFascicoloById(notify.ID_OBJECT, UIManager.UserManager.GetInfoUser());
                        prj.template = UIManager.ProfilerProjectManager.getTemplateFascDettagli(notify.ID_OBJECT);
                        UIManager.ProjectManager.setProjectInSession(prj);
                        link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), false, Page);
                    }
                }
            }



            Response.Redirect(link);
        }

        protected void LnkSize1_Click(object sender, EventArgs e)
        {
            try
            {
                LnkSize1.Attributes.Remove("class");
                LnkSize2.Attributes.Remove("class");
                LnkSize3.Attributes.Remove("class");
                LnkSize1.Attributes.Add("class", "size1Sel");
                LnkSize2.Attributes.Add("class", "size2");
                LnkSize3.Attributes.Add("class", "size3");
                UIManager.CssManager.SetSizeText(UIManager.CssManager.TextSize.NORMAL.ToString());
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "updateCss", "$('body').attr('class', 'body_normal');", true);
                //IdMasterBody.Attributes.Add("class", "body_normal");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void LnkSize2_Click(object sender, EventArgs e)
        {
            try
            {
                LnkSize1.Attributes.Remove("class");
                LnkSize2.Attributes.Remove("class");
                LnkSize3.Attributes.Remove("class");
                LnkSize2.Attributes.Add("class", "size2Sel");
                LnkSize1.Attributes.Add("class", "size1");
                LnkSize3.Attributes.Add("class", "size3");
                UIManager.CssManager.SetSizeText(UIManager.CssManager.TextSize.MEDIUM.ToString());
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "updateCss", "$('body').attr('class', 'body_medium');", true);
                //IdMasterBody.Attributes.Add("class", "body_medium");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void LnkSize3_Click(object sender, EventArgs e)
        {
            try
            {
                LnkSize1.Attributes.Remove("class");
                LnkSize2.Attributes.Remove("class");
                LnkSize3.Attributes.Remove("class");
                LnkSize3.Attributes.Add("class", "size3Sel");
                LnkSize1.Attributes.Add("class", "size1");
                LnkSize2.Attributes.Add("class", "size2");
                UIManager.CssManager.SetSizeText(UIManager.CssManager.TextSize.HIGH.ToString());
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "updateCss", "$('body').attr('class', 'body_high');", true);
                //IdMasterBody.Attributes.Add("class", "body_high");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializesLabels()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            string languageDirection = Utils.Languages.GetLanguageDirection(language);
            string banner = string.Empty;
            Html.Attributes.Add("dir", languageDirection);
            SetCssClass(languageDirection);
            InitializeLabelMenu(language);

            LblTextSize.Text = Utils.Languages.GetLabelFromCode("LblTextSize", language);
            BaseMasterTxtSearch.Text = Utils.Languages.GetLabelFromCode("BaseMasterTxtSearch", language);
            Loading.Text = Utils.Languages.GetLabelFromCode("Loading", language);
            litDialogCheck.Text = Utils.Languages.GetLabelFromCode("DialogCheckTitle", language);
            litDialogError.Text = Utils.Languages.GetLabelFromCode("DialogErrorTitle", language);
            litDialogInfo.Text = Utils.Languages.GetLabelFromCode("DialogInfoTitle", language);
            litDialogQuestion.Text = Utils.Languages.GetLabelFromCode("DialogQuestionTitle", language);
            litDialogWarning.Text = Utils.Languages.GetLabelFromCode("DialogWarningTitle", language);
            litConfirm.Text = Utils.Languages.GetLabelFromCode("DialogQuestionTitle", language);
            LinkButtonNextMaster.ToolTip = Utils.Languages.GetLabelFromCode("LinkButtonNextMaster", language);
            LinkButtonNextMaster.AlternateText = Utils.Languages.GetLabelFromCode("LinkButtonNextMaster", language);
            LinkButtonPreMaster.ToolTip = Utils.Languages.GetLabelFromCode("LinkButtonPreMaster", language);
            LinkButtonPreMaster.AlternateText = Utils.Languages.GetLabelFromCode("LinkButtonPreMaster", language);
            BaseMasterManagementTransmission.Text = Utils.Languages.GetLabelFromCode("BaseMasterManagementTransmission", language);
            Guide.Title = Utils.Languages.GetLabelFromCode("GuideTitle", language);
            BaseMasterLblpaCode.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblpaCode", language);
            BaseMasterLblInstAcc.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblInstAcc", language);
            BaseMasterLblSignatureProcesses.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSignatureProcesses", language);
            BaseMasterLblMonitoringProcesses.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblMonitoringProcesses", language);
            BaseMasterLblLastDocumentsView.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblLastDocumentsView", language);
            BaseMasterLblProceedings.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblProceedings", language);
            BaseMasterLblReportAccessi.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblReportAccessi", language);
            banner = UIManager.AdministrationManager.GetBanner(UserManager.GetInfoUser().idAmministrazione);
            this.BaseMasterLblClientBigfile.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblClientBigfile", language);
            this.ClientBigfile.Title = Utils.Languages.GetLabelFromCode("BaseMasterLblClientBigfile", language);

            if (!string.IsNullOrEmpty(banner))
            {
                LblBanner.Text = banner;
            }
            

            //Gestione orario solo per italiano e inglese
            if (string.IsNullOrEmpty(language) || language.Equals("Italian"))
            {
                spanLanguage.Attributes.Add("class", "lg1");
            }
            else
            {
                if (language.Equals("English"))
                {
                    spanLanguage.Attributes.Add("class", "lg2");
                }
            }

            ManageAddressBook.Title = Utils.Languages.GetLabelFromCode("AddFilterAddressBookTitle", language);
            // PEC 4 requisito 4 - report spedizioni
            SendingReport.Title = Utils.Languages.GetLabelFromCode("SendingReportTitle", language);
            LblDescriptionOfOrganization.Text = UIManager.UserManager.getInfoAmmCorrente(UIManager.UserManager.GetInfoUser().idAmministrazione).Descrizione;

            pDescriptionOfOrganization.Style.Add(HtmlTextWriterStyle.FontSize, "12px");
            if (LblDescriptionOfOrganization.Text.ToString().Length > 90 && LblDescriptionOfOrganization.Text.ToString().Length < 100)
            {
                pDescriptionOfOrganization.Style.Add(HtmlTextWriterStyle.FontSize, "11px");
            }
            else
                if (LblDescriptionOfOrganization.Text.ToString().Length > 100 && LblDescriptionOfOrganization.Text.ToString().Length < 110)
                {
                    pDescriptionOfOrganization.Style.Add(HtmlTextWriterStyle.FontSize, "10px");
                }
                else
                    if (LblDescriptionOfOrganization.Text.ToString().Length > 110)
                    {
                        pDescriptionOfOrganization.Style.Add(HtmlTextWriterStyle.FontSize, "9px");
                    }
            //UpdateDescriptionOfOrganization.Update();

        }

        protected void InitializeLabelMenu(string language)
        {
            BaseMasterLblHome.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblHome", language);
            BaseMasterLblDocument.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblDocument", language);
            //BaseMasterLblNewRecord.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblNewRecord", language);
            BaseMasterLblNewDocument.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblNewDocument", language);
            BaseMasterLblNewPrepared.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblNewPrepared", language);
            //BaseMasterLblARecord.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblARecord", language);
            //BaseMasterLblPRecord.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblPRecord", language);
            BaseMasterLblImport.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblImport", language);
            BaseMasterLblImportDoc.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblImportDoc", language);
            BaseMasterLblImportDocPre.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblImportDocPre", language);
            BaseMasterLblImportRDE.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblImportRDE", language);
            BaseMasterLblImportInvoice.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblImportInvoice", language);
            BaseMasterLblImportMailMerge.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblImportMailMerge", language);
            BaseMasterLblImportProject.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblImportProject", language);
            BaseMasterLblSearch.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearch", language);
            BaseMasterLblSearchDoc.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchDoc", language);
            BaseMasterLblSearchProject.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchProject", language);
            BaseMasterLblSearchTransmissions.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchTransmissions", language);
            BaseMasterLblSearchVisibility.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchVisibility", language);
            BaseMasterLblSearchAdlDoc.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchAdlDoc", language);
            BaseMasterLblSearchAdlProject.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchAdlProject", language);
            BaseMasterLblSearchCommonFields.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchCommonFields", language);
            BaseMasterLblManagement.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblManagement", language);
            BaseMasterLblRegistry.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblRegistry", language);
            BaseMasterLblConservationArea.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblConservationArea", language);
            BaseMasterLblDocumentsRemoved.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblDocumentsRemoved", language);
            BaseMasterLblOrganizationChart.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblOrganizationChart", language);
            BaseMasterLblRepertoires.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblRepertoires", language);
            BaseMasterPrints.Text = Utils.Languages.GetLabelFromCode("BaseMasterPrints", language);
            BaseMasterProspects.Text = Utils.Languages.GetLabelFromCode("BaseMasterProspects", language);
            BaseMasterAddressBook.Text = Utils.Languages.GetLabelFromCode("BaseMasterAddressBook", language);
            BaseMasterList.Text = Utils.Languages.GetLabelFromCode("BaseMasterList", language);
            BaseMasterDelegate.Text = Utils.Languages.GetLabelFromCode("BaseMasterDelegate", language);
            BaseMasterNotes.Text = Utils.Languages.GetLabelFromCode("BaseMasterNotes", language);
            BaseMasterOptions.Text = Utils.Languages.GetLabelFromCode("BaseMasterOptions", language);
            BaseMasterChangePassword.Text = Utils.Languages.GetLabelFromCode("BaseMasterChangePassword", language);
            BaseMasterHelp.Text = Utils.Languages.GetLabelFromCode("BaseMasterHelp", language);
            BaseMasterHelp.ToolTip = Utils.Languages.GetLabelFromCode("BaseMasterHelpTooltip", language);
            LiMenuHelp.Attributes.Add("ToolTip", "Test");
            LiMenuHelp.Attributes.Add("AlternateText", "Test");
            BaseMasterLogOut.Text = Utils.Languages.GetLabelFromCode("BaseMasterLogOut", language);
            lblMenuFascicolo.Text = Utils.Languages.GetLabelFromCode("BaseFascicolo", language);
            lblNuovoFascicolo.Text = Utils.Languages.GetLabelFromCode("BaseNuovoFascicolo", language);
            BaseMasterLblNewRecordInBound.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblNewRecordInBound", language);
            BaseMasterLblNewRecordOutBound.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblNewRecordOutBound", language);
            // PEC 4 requisito 4 - report spedizioni
            BaseMasterSendingReport.Text = Utils.Languages.GetLabelFromCode("BaseMasterSendingReport", language);
            if (InternalRecordEnable)
                BaseMasterLblNewRecordInternal.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblNewRecordInternal", language);
            //*****************************************
            //Deposito (Voci relative al Deposito)
            //15042013
            //*****************************************
            BaseMasterDeposito.Text = Utils.Languages.GetLabelFromCode("BaseMasterDeposito", language);
            BaseMasterAutorizzazioneConsultazione.Text = Utils.Languages.GetLabelFromCode("BaseMasterAutorizzazioneConsultazione", language);
            //Iacozzilli: in collaudo - no- scarto. 
            BaseMasterScarto.Text = Utils.Languages.GetLabelFromCode("BaseMasterScarto", language);
            BaseMasterLblSearchVersamenti.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchVersamenti", language);
            BaseMasterLblSearchScarti.Text = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchScarti", language);
            //*****************************************
            //*****************************************
            //FORMAZIONE
            BaseMasterFormazione.Text = Utils.Languages.GetLabelFromCode("BaseMasterFormazione", language);
            Formazione.Title = Utils.Languages.GetLabelFromCode("BaseMasterFormazione", language);
            FormazioneAddressBook.Title = Utils.Languages.GetLabelFromCode("BaseMasterAddressBook", language);
           
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
            //CssLayout.Attributes.Add("href", link);
        }

        protected void SetProjectPage()
        {
            lifascicolo.Attributes.Remove("class");
            lifascicolo.Attributes.Add("class", "iAmMenuFascicolo");

            NamePage.Text = "Scheda fascicolo";


        }

        protected void SetSearchPage()
        {
            LiMenuSearch.Attributes.Remove("class");
            LiMenuSearch.Attributes.Add("class", "iAmMenuSearch");
        }

        protected void SetHomePage()
        {
            LiMenuHome.Attributes.Remove("class");
            LiMenuHome.Attributes.Add("class", "iAmMenuHome");

        }

        protected void SetDocumentPage()
        {
            LiMenuDocument.Attributes.Remove("class");
            LiMenuDocument.Attributes.Add("class", "iAmMenuDocument");
        }

        protected void SetManagementPage()
        {
            LiMenuManagement.Attributes.Remove("class");
            LiMenuManagement.Attributes.Add("class", "iAmMenuManagement");
        }

        protected void SetOptionsPage()
        {
            LiMenuOptions.Attributes.Remove("class");
            LiMenuOptions.Attributes.Add("class", "iAmMenuOptions");
        }

        protected string GetSurnameUser()
        {
            string result = string.Empty;
            DocsPaWR.Utente utente = UIManager.UserManager.GetUserInSession();
            if (utente != null)
            {
                result = utente.cognome;
            }
            return result;
        }

        protected string GetNameUser()
        {
            string result = string.Empty;
            DocsPaWR.Utente utente = UIManager.UserManager.GetUserInSession();
            if (utente != null)
            {
                result = utente.nome;
            }
            return result;
        }

        protected string GetFullName()
        {
            string result = string.Empty;
            DocsPaWR.Utente utente = UIManager.UserManager.GetUserInSession();
            DocsPaWR.Utente delegato = UIManager.UserManager.getUtenteDelegato();

            if (utente != null)
            {
                if (delegato != null)
                {
                    string language = UIManager.UserManager.GetUserLanguage();
                    string del = Utils.Languages.GetLabelFromCode("TransmissionDelegatedBy", language).ToUpper();
                    result = "<strong>" + delegato.cognome + "</strong> " + delegato.nome + " <span class=\"italic\">(" + del + " " + utente.cognome + " " + utente.nome + ")</span>";
                }
                else
                {
                    result = "<strong>" + utente.cognome + "</strong> " + utente.nome;
                }
            }

            return result;
        }

        protected string GetRoleUser()
        {
            string result = string.Empty;
            //DocsPaWR.Utente utente = UIManager.UserManager.GetUserInSession();
            //if (utente != null)
            //{
            //    result = utente.ruoli[0].descrizione;
            //}
            DocsPaWR.Ruolo ruolo = UIManager.RoleManager.GetRoleInSession();
            if (ruolo != null)
            {
                result = ruolo.codiceRubrica + " - " + ruolo.descrizione;
            }
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

            LblCopyright.Text = version.CopyrightName;

            LblVersion.Text = version.ApplicationNameVersion;

            ImgEuropeFlag.Src = ResolveUrl("~/Images/Common/europe_flag.png");
            ImgItalianLaw.Src = ResolveUrl("~/Images/Common/italian_law.png");

            ImgEuropeFlag.Alt = Server.MachineName + getIPAddress();
            ImgItalianLaw.Alt = Server.MachineName + getIPAddress();

            ImgEuropeFlag.Attributes.Add("title", Server.MachineName + getIPAddress());
            ImgItalianLaw.Attributes.Add("title", Server.MachineName + getIPAddress());

            LblIP.Text = "IP " + getIPAddress();
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

        private bool IsBack
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsBack"] != null) result = (bool)HttpContext.Current.Session["IsBack"];
                return result;

            }
            set
            {
                HttpContext.Current.Session["IsBack"] = value;
            }
        }

        private List<DocumentoVisualizzato> LastDocumentsView
        {
            get
            {
                if (HttpContext.Current.Session["LastDocumentsView"] != null)
                    return (List<DocumentoVisualizzato>)HttpContext.Current.Session["LastDocumentsView"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["LastDocumentsView"] = value;
            }
        }

        private SearchObject[] ListObjectNavigation
        {
            get
            {
                if (HttpContext.Current.Session["ListObjectNavigation"] != null)
                    return (SearchObject[])HttpContext.Current.Session["ListObjectNavigation"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ListObjectNavigation"] = value;
            }
        }

        private Trasmissione[] ListTransmissionNavigation
        {
            get
            {
                if (HttpContext.Current.Session["ListTransmissionNavigation"] != null)
                    return (Trasmissione[])HttpContext.Current.Session["ListTransmissionNavigation"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ListTransmissionNavigation"] = value;
            }
        }
    }
}
