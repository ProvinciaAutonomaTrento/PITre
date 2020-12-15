using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace NttDataWA.Navigation
{
    public class NavigationUtils
    {

        public static string GetLink(string page, bool back, Page pag)
        {
            string retVal = string.Empty;
            switch (page)
            {
                case "HOME":
                    retVal = pag.ResolveUrl("~/Index.aspx?back=1");
                    break;
                case "DOCUMENT":
                    if (back)
                    {
                        retVal = pag.ResolveUrl("~/Document/Document.aspx?back=1");
                    }
                    else
                    {
                        retVal = pag.ResolveUrl("~/Document/Document.aspx?next=1");
                    }
                    break;
                case "DOCUMENT_CLASSIFICATION":
                    retVal = pag.ResolveUrl("~/Document/Classifications.aspx?back=1");
                    break;
                case "DOCUMENT_ATTACHMENT":
                    retVal = pag.ResolveUrl("~/Document/Attachments.aspx?back=1");
                    break;
                case "DOCUMENT_TRANSMISSIONS":
                    retVal = pag.ResolveUrl("~/Document/Transmissions.aspx?back=1");
                    break;
                case "DOCUMENT_VISIBILITY":
                    retVal = pag.ResolveUrl("~/Document/Visibility.aspx?back=1");
                    break;
                case "DOCUMENT_EVENTS":
                    retVal = pag.ResolveUrl("~/Document/Events.aspx?back=1");
                    break;
                case "PROJECT":
                    if (back)
                    {
                        retVal = pag.ResolveUrl("~/Project/Project.aspx?back=1");
                    }
                    else
                    {
                        retVal = pag.ResolveUrl("~/Project/Project.aspx?next=1");
                    }
                    break;
                case "PROJECT_STRUCTURE":
                    retVal = pag.ResolveUrl("~/Project/Structure.aspx?back=1");
                    break;
                case "PROJECT_TRANSMISSIONS":
                    retVal = pag.ResolveUrl("~/Project/TransmissionsP.aspx?back=1");
                    break;
                case "PROJECT_VISIBILITY":
                    retVal = pag.ResolveUrl("~/Project/VisibilityP.aspx?back=1");
                    break;
                case "PROJECT_EVENTS":
                    retVal = pag.ResolveUrl("~/Project/EventsP.aspx?back=1");
                    break;
                case "SEARCH_DOCUMENTS_SIMPLE":
                    retVal = pag.ResolveUrl("~/Search/SearchDocument.aspx?back=1");
                    break;
                case "SEARCH_DOCUMENTS_ADVANCED":
                    retVal = pag.ResolveUrl("~/Search/SearchDocumentAdvanced.aspx?back=1");
                    break;
                case "SEARCH_DOCUMENTS_ADVANCED_ADL":
                    retVal = pag.ResolveUrl("~/Search/SearchDocumentAdvanced.aspx?back=1&IsAdl=true");
                    break;
                case "SEARCH_DOCUMENTS_PRINTS":
                    retVal = pag.ResolveUrl("~/Search/SearchDocumentPrints.aspx?back=1");
                    break;
                case "SEARCH_PROJECTS":
                    retVal = pag.ResolveUrl("~/Search/SearchProject.aspx?back=1");
                    break;
                case "SEARCH_PROJECTS_WA":
                    retVal = pag.ResolveUrl("~/Search/SearchProject.aspx?back=1&IsAdl=true");
                    break;
                case "SEARCH_ARCHIVE":
                    retVal = pag.ResolveUrl("~/Search/SearchArchive.aspx?back=1");
                    break;
                case "MANDATE":
                    retVal = pag.ResolveUrl("~/Mandate/Mandate.aspx?back=1");
                    break;
                case "MANAGEMENT_REPERTORIES":
                    retVal = pag.ResolveUrl("~/Management/RegisterRepertories.aspx?back=1");
                    break;
                case "MANAGEMENT_REGISTERS":
                    retVal = pag.ResolveUrl("~/Management/Registers.aspx?back=1");
                    break;
                case "MANAGEMENT_MODELS_TRANSMISSION":
                    retVal = pag.ResolveUrl("~/Management/GestioneModelliTrasm.aspx?back=1");
                    break;
                case "MANAGEMENT_DISTRIBUTION_LISTS":
                    retVal = pag.ResolveUrl("~/Management/DistributionList.aspx?back=1");
                    break;
                case "MANAGEMENT_SIGNATURE_PROCESSES":
                    retVal = pag.ResolveUrl("~/Management/SignatureProcesses.aspx?back=1");
                    break;
                case "MANAGEMENT_MONITORING_PROCESSES":
                    retVal = pag.ResolveUrl("~/Management/MonitoringProcesses.aspx?back=1");
                    break;
                case "SUMMERIES":
                    retVal = pag.ResolveUrl("~/Summaries/Summeries.aspx?back=1");
                    break;
                case "MANAGEMENT_PRINTS":
                    retVal = pag.ResolveUrl("~/Management/Prints.aspx?back=1");
                    break;
                case "SEARCH_TRANSMISSIONS":
                    retVal = pag.ResolveUrl("~/Search/SearchTransmission.aspx?back=1");
                    break;
                case "IMPORT_DOCUMENT":
                    retVal = pag.ResolveUrl("~/ImportDati/ImportDocuments.aspx?back=1");
                    break;
                case "IMPORT_DOCUMENT_PREVIOUS":
                    retVal = pag.ResolveUrl("~/ImportDati/ImportPrevious.aspx?back=1");
                    break;
                case "IMPORT_RDE":
                    retVal = pag.ResolveUrl("~/ImportDati/ImportRDE.aspx?back=1");
                    break;
                case "IMPORT_PROJECT":
                    retVal = pag.ResolveUrl("~/ImportDati/ImportProjects.aspx?back=1");
                    break;
                case "MANAGEMENT_NOTES":
                    retVal = pag.ResolveUrl("~/Management/Notes.aspx?back=1");
                    break;
                case "MANAGEMENT_CONSERVATION":
                    retVal = pag.ResolveUrl("~/Management/ConservationArea.aspx?back=1");
                    break;
                case "MANAGEMENT_DOCUMENTS_REMOVED":
                    retVal = pag.ResolveUrl("~/Management/DocumentsRemoved.aspx?back=1");
                    break;
                case "MANAGEMENT_ORGANIZATION":
                    retVal = pag.ResolveUrl("~/Management/OrganizationChart.aspx?back=1");
                    break;
                case "CHANGE_PASSWORD":
                    retVal = pag.ResolveUrl("~/Options/ChangePassword.aspx?back=1");
                    break;
                case "HOME_ADL_PROJECT":
                    retVal = pag.ResolveUrl("~/Home/AdlProject.aspx?back=1");
                    break;
                case "HOME_ADL_DOCUMENT":
                    retVal = pag.ResolveUrl("~/Home/AdlDocument.aspx?back=1");
                    break;
                case "HOME_LIBRO_FIRMA":
                    retVal = pag.ResolveUrl("~/Home/LibroFirma.aspx?back=1");
                    break;
                case "HOME_TASK":
                    retVal = pag.ResolveUrl("~/Home/Task.aspx?back=1");
                    break;
                case "MANAGEMENT_INSTANCE_ACCESS":
                    retVal = pag.ResolveUrl("~/Management/InstanceAccess.aspx?back=1");
                    break;
                     case "MANAGEMENT_INSTANCE_DETAILS":
                    retVal = pag.ResolveUrl("~/Management/InstanceDetails.aspx?back=1");
                    break;
                case "MANAGEMENT_INSTANCE_STRUCTURE":
                    retVal = pag.ResolveUrl("~/Management/InstanceStructure.aspx?back=1");
                    break;
                case "IMPORT_INVOICE":
                    retVal = pag.ResolveUrl("~/ImportDati/ImportInvoice.aspx?back=1");
                    break;
            
            }
            return retVal;
        }

        public static string GetNamePage(string page, string type)
        {
            string retVal = string.Empty;
            string language = UIManager.UserManager.GetUserLanguage();
            switch (page)
            {
                case "HOME":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationHome", language);
                    break;
                case "DOCUMENT":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationDocument", language);
                    break;
                case "DOCUMENT_CLASSIFICATION":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationClassification", language);
                    break;
                case "DOCUMENT_ATTACHMENT":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationAttachments", language);
                    break;
                case "DOCUMENT_TRANSMISSIONS":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationTransmissions", language);
                    break;
                case "DOCUMENT_VISIBILITY":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationVisibility", language);
                    break;
                case "DOCUMENT_EVENTS":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationEvents", language);
                    break;
                case "DOCUMENT_NEW_TRANSMISSION":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationNewDocumentTransmission", language);
                    break;
                case "PROJECT":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationProject", language);
                    break;
                case "PROJECT_STRUCTURE":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationProjectStructure", language);
                    break;
                case "PROJECT_TRANSMISSIONS":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationProjectTransmissions", language);
                    break;
                case "PROJECT_VISIBILITY":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationProjectVisibility", language);
                    break;
                case "PROJECT_EVENTS":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationProjectEvents", language);
                    break;
                case "PROJECT_NEW_TRANSMISSION":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationNewProjectTransmission", language);
                    break;
                case "SEARCH_DOCUMENTS_SIMPLE":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationSearchSimple", language);
                    break;
                case "SEARCH_DOCUMENTS_ADVANCED":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationSearchAdvanced", language);
                    break;
                case "SEARCH_DOCUMENTS_PRINTS":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationSearchPrints", language);
                    break;
                case "SEARCH_PROJECTS":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationSearchProjects", language);
                    break;
                case "SEARCH_ARCHIVE":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationSearchArchive", language);
                    break;
                case "MANDATE":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementMandate", language);
                    break;
                case "MANAGEMENT_REPERTORIES":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementRepertories", language);
                    break;
                case "MANAGEMENT_REGISTERS":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementRegisters", language);
                    break;
                case "SEARCH_VISIBILITY":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementSearchVisibility", language);
                    break;
                case "SEARCH_DOCUMENTS_ADVANCED_ADL":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementSearchDocumentAdvancedAdl", language);
                    break;
                case "SEARCH_PROJECTS_WA":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementSearchProjectAdvancedAdl", language);
                    break;
                case "SEARCH_TRANSMISSIONS":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementSearchTransmission", language);
                    break;
                case "MANAGEMENT_MODELS_TRANSMISSION":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementManagementTransmission", language);
                    break;
                case "MANAGEMENT_DISTRIBUTION_LISTS":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementManagementDistributionLists", language);
                    break;
                case "MANAGEMENT_SIGNATURE_PROCESSES":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementManagementSignatureProcesses", language);
                    break;
                case "MANAGEMENT_MONITORING_PROCESSES":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementManagementMonitoringProcesses", language);
                    break;
                case "SUMMERIES":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementManagementSummaries", language);
                    break;
                case "MANAGEMENT_PRINTS":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementManagementPrints", language);
                    break;
                case "IMPORT_DOCUMENT":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationImportDocument", language);
                    break;
                case "IMPORT_DOCUMENT_PREVIOUS":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationImportDocumentPrevious", language);
                    break;
                case "IMPORT_RDE":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationImportDocumentRDE", language);
                    break;
                case "IMPORT_PROJECT":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationImportProject", language);
                    break;
                case "MANAGEMENT_NOTES":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementNotes", language);
                    break;
                case "MANAGEMENT_CONSERVATION":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementConservation", language);
                    break;
                case "MANAGEMENT_DOCUMENTS_REMOVED":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementDocumentsRemoved", language);
                    break;
                case "MANAGEMENT_ORGANIZATION":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementOrganization", language);
                    break;
                case "CHANGE_PASSWORD":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationOptionsChangePassword", language);
                    break;
                case "HOME_ADL_PROJECT":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationHomeAdlProject", language);
                    break;
                case "HOME_ADL_DOCUMENT":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationHomeAdlDocument", language);
                    break;
                case "HOME_LIBRO_FIRMA":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationHomeLibroFirma", language);
                    break;
                case "HOME_TASK":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationHomeTask", language);
                    break;
                case "MANAGEMENT_INSTANCE_ACCESS":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementInstanceAccess", language);
                    break;
                case "MANAGEMENT_INSTANCE_DETAILS":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementInstanceDetails", language);
                    break;
                case "MANAGEMENT_INSTANCE_STRUCTURE":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationManagementInstanceStructure", language);
                    break;
                case "IMPORT_INVOICE":
                    retVal = Utils.Languages.GetLabelFromCode("NavigationImportInvoice", language);
                    break;

            }
            return retVal;
        }

        public static List<NavigationObject> GetNavigationList()
        {
            try
            {
                return (List<NavigationObject>)GetSessionValue("navigationList");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void SetNavigationList(List<NavigationObject> list)
        {
            SetSessionValue("navigationList", list);
        }

        public static void RemoveNavigationList()
        {
            RemoveSessionValue("navigationList");
        }

        public static Navigation.NavigationObject CloneObject(Navigation.NavigationObject obj)
        {
            Navigation.NavigationObject clone = obj;
            Navigation.NavigationObject retVal = new Navigation.NavigationObject();

            if (clone.Classification != null)
            {
                retVal.Classification = clone.Classification;
            }
            if (!string.IsNullOrEmpty(clone.DxPositionElement))
            {
                retVal.DxPositionElement = clone.DxPositionElement;
            }
            if (!string.IsNullOrEmpty(clone.DxTotalNumberElement))
            {
                retVal.DxTotalNumberElement = clone.DxTotalNumberElement;
            }
            if (!string.IsNullOrEmpty(clone.DxTotalPageNumber))
            {
                retVal.DxTotalPageNumber = clone.DxTotalPageNumber;
            }

            if (!string.IsNullOrEmpty(clone.TypeTransmissionSearch))
            {
                retVal.TypeTransmissionSearch = clone.TypeTransmissionSearch;
            }

            if (clone.SearchTransmission)
            {
                retVal.SearchTransmission = clone.SearchTransmission;
            }

            if (clone.SearchFilters != null)
            {
                retVal.SearchFilters = clone.SearchFilters;
            }
            if (!string.IsNullOrEmpty(clone.NumPage))
            {
                retVal.NumPage = clone.NumPage;
            }
            if (!string.IsNullOrEmpty(clone.PageSize))
            {
                retVal.PageSize = clone.PageSize;
            }
            if (!string.IsNullOrEmpty(clone.DxTotalPageNumber))
            {
                retVal.DxTotalPageNumber = clone.DxTotalPageNumber;
            }

            if (clone.SearchFiltersOrder != null)
            {
                retVal.SearchFiltersOrder = clone.SearchFiltersOrder;
            }

            if (clone.folder != null)
            {
                retVal.folder = clone.folder;
            }

            if (!string.IsNullOrEmpty(clone.idProject))
            {
                retVal.idProject = clone.idProject;
            }

            if (clone.FromNotifyCenter)
            {
                retVal.FromNotifyCenter = clone.FromNotifyCenter;
            }

            //if (!string.IsNullOrEmpty(clone.OriginalObjectId))
            //{
            //    retVal.OriginalObjectId = clone.OriginalObjectId;
            //}

            if (clone.ViewResult)
            {
                retVal.ViewResult = true;
            }

            if (!string.IsNullOrEmpty(clone.CodePage))
            {
                retVal.CodePage = clone.CodePage;
            }

            if (!string.IsNullOrEmpty(clone.NamePage))
            {
                retVal.NamePage = clone.NamePage;
            }

            if (!string.IsNullOrEmpty(clone.NumPage))
            {
                retVal.NumPage = clone.NumPage;
            }

            if (!string.IsNullOrEmpty(clone.OriginalObjectId))
            {
                retVal.OriginalObjectId = clone.OriginalObjectId;
            }


            return retVal;
        }

        public enum NamePage
        {
            HOME,
            DOCUMENT,
            DOCUMENT_CLASSIFICATION,
            DOCUMENT_ATTACHMENT,
            DOCUMENT_TRANSMISSIONS,
            DOCUMENT_VISIBILITY,
            DOCUMENT_EVENTS,
            DOCUMENT_NEW_TRANSMISSION,
            PROJECT,
            PROJECT_ARCHIVE,
            PROJECT_STRUCTURE,
            PROJECT_TRANSMISSIONS,
            PROJECT_VISIBILITY,
            PROJECT_EVENTS,
            PROJECT_NEW_TRANSMISSION,
            SEARCH_DOCUMENTS_SIMPLE,
            SEARCH_DOCUMENTS_ADVANCED,
            SEARCH_DOCUMENTS_PRINTS,
            SEARCH_PROJECTS,
            SEARCH_ARCHIVE,
            SEARCH_TRANSMISSIONS,
            SEARCH_VISIBILITY,
            SEARCH_DOCUMENTS_WA,
            SEARCH_PROJECTS_WA,
            SEARCH_COMMON_FIELDS,
            MANAGEMENT_REGISTERS,
            MANAGEMENT_RF,
            MANAGEMENT_PRINTS,
            MANAGEMENT_PROSPECT,
            MANAGEMENT_MANDATE,
            MANAGEMENT_REPERTORIES,
            MANDATE,
            SEARCH_DOCUMENTS_ADVANCED_ADL,
            MANAGEMENT_MODELS_TRANSMISSION,
            MANAGEMENT_DISTRIBUTION_LISTS,
            SUMMERIES,
            IMPORT_DOCUMENT,
            IMPORT_DOCUMENT_PREVIOUS,
            IMPORT_RDE,
            IMPORT_PROJECT,
            MANAGEMENT_NOTES,
            MANAGEMENT_CONSERVATION,
            MANAGEMENT_DOCUMENTS_REMOVED,
            MANAGEMENT_ORGANIZATION,
            CHANGE_PASSWORD,
            HOME_ADL_PROJECT,
            HOME_ADL_DOCUMENT,
            HOME_LIBRO_FIRMA,
            HOME_TASK,
            MANAGEMENT_INSTANCE_ACCESS,
            MANAGEMENT_INSTANCE_DETAILS,
            MANAGEMENT_INSTANCE_STRUCTURE,
            MANAGEMENT_SIGNATURE_PROCESSES,
            MANAGEMENT_MONITORING_PROCESSES,
            IMPORT_INVOICE
        }

        #region Session functions
        /// <summary>
        /// Reperimento valore da sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        private static System.Object GetSessionValue(string sessionKey)
        {
            try
            {
                return System.Web.HttpContext.Current.Session[sessionKey];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Impostazione valore in sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="sessionValue"></param>
        private static void SetSessionValue(string sessionKey, object sessionValue)
        {
            try
            {
                System.Web.HttpContext.Current.Session[sessionKey] = sessionValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Rimozione chiave di sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        private static void RemoveSessionValue(string sessionKey)
        {
            try
            {
                System.Web.HttpContext.Current.Session.Remove(sessionKey);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
        #endregion
    }
}

