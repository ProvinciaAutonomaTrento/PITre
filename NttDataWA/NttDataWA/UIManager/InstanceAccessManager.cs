using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;

namespace NttDataWA.UIManager
{
    public class InstanceAccessManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        #region Services Backend

        /// <summary>
        /// Restituisce la lista delle istanze d'accesso il cui proprietario è l'utente loggato
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGroup"></param>
        /// <returns></returns>
        public static List<InstanceAccess> GetInstanceAccess(string idPeople, string idGroup)
        {
            try
            {
                return docsPaWS.GetInstanceAccess(idPeople, idGroup, UserManager.GetInfoUser()).ToList<InstanceAccess>();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.InstanceAccess getInstanceAccessInSession()
        {
            try
            {
                return (DocsPaWR.InstanceAccess)HttpContext.Current.Session["Instance"];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void setInstanceAccessInSession(DocsPaWR.InstanceAccess instanceAccess)
        {
            try
            {
                HttpContext.Current.Session["Instance"] = instanceAccess;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);

            }
        }

        /// <summary>
        /// Ritorna l'istanza di accesso avente l'id specificato
        /// </summary>
        /// <param name="idInstanceAccess"></param>
        /// <returns></returns>
        public static InstanceAccess GetInstanceAccessById(string idInstanceAccess)
        {
            try
            {
                return docsPaWS.GetInstanceAccessById(idInstanceAccess, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Inserisce una nuova istanza d'accesso
        /// </summary>
        /// <param name="instanceAccess"></param>
        /// <returns></returns>
        public static InstanceAccess InsertInstanceAccess(InstanceAccess instanceAccess)
        {
            InstanceAccess instance = null;
            try
            {
                instance = docsPaWS.InsertInstanceAccess(instanceAccess, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return instance;
        }

        /// <summary>
        /// Inserisce la lista di documenti relativi ad un'istanza d'accesso
        /// </summary>
        /// <param name="listInstanceAccessDocuments"></param>
        /// <returns></returns>
        public static bool InsertInstanceAccessDocuments(List<InstanceAccessDocument> listInstanceAccessDocuments)
        {
            bool result = false;
            try
            {
                result = docsPaWS.InsertInstanceAccessDocuments(listInstanceAccessDocuments.ToArray(), UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        /// <summary>
        /// Inserisce la lista degli allegati di un documento appartenente ad un'istanza d'accesso
        /// </summary>
        /// <param name="listInstanceAccessAttachments"></param>
        /// <returns></returns>
        public static bool InsertInstanceAccessAttachments(List<InstanceAccessAttachments> listInstanceAccessAttachments)
        {
            bool result = false;
            try
            {
                result = docsPaWS.InsertInstanceAccessAttachments(listInstanceAccessAttachments.ToArray(), UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        /// <summary>
        /// Rimuove la lista di documenti legati ad un'istanza di accesso
        /// </summary>
        /// <param name="listInstanceAccessDocuments"></param>
        /// <returns></returns>
        public static bool RemoveInstanceAccessDocuments(List<InstanceAccessDocument> listInstanceAccessDocuments)
        {
            bool result = false;
            try
            {
                result = docsPaWS.RemoveInstanceAccessDocuments(listInstanceAccessDocuments.ToArray(), UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        /// <summary>
        /// Rimuove la lista di allegati di un documento appartenente ad un'istanza di accesso
        /// </summary>
        /// <param name="listInstanceAccessAttachments"></param>
        /// <returns></returns>
        public static bool RemoveInstanceAccessAttachments(List<InstanceAccessAttachments> listInstanceAccessAttachments)
        {
            bool result = false;
            try
            {
                result = docsPaWS.RemoveInstanceAccessAttachments(listInstanceAccessAttachments.ToArray(), UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        /// <summary>
        /// Aggiorna l'istanza d'accesso
        /// </summary>
        /// <param name="instanceAccess"></param>
        /// <returns></returns>
        public static DocsPaWR.InstanceAccess UpdateInstanceAccess(InstanceAccess instanceAccess)
        {
            DocsPaWR.InstanceAccess instance = null;
            try
            {
                instance = docsPaWS.UpdateInstanceAccess(instanceAccess, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return instance;
        }

        public static bool UpdateInstanceAccessDocuments(List<InstanceAccessDocument> listInstanceAccessDocuments)
        {
            bool result = false;
            try
            {
                result = docsPaWS.UpdateInstanceAccessDocuments(listInstanceAccessDocuments.ToArray(), UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        public static void AsyncCreateDownload(InstanceAccess instanceAccess)
        {
            try
            {
                docsPaWS.AsyncCreateDownload(instanceAccess, UserManager.GetInfoUser(), RoleManager.GetRoleInSession());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static string GetStateDownloadInstanceAccess(string idInstanceAccess)
        {
            string result = string.Empty;
            try
            {
                result = docsPaWS.GetStateDownloadInstanceAccess(idInstanceAccess, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        public static void RemoveInstanceZip(InstanceAccess instanceAccess)
        {
            try
            {
                docsPaWS.RemoveInstanceZip(instanceAccess, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DocsPaWR.InstanceAccess CreateDeclarationDocument()
        {
            DocsPaWR.InstanceAccess instance = null;
            try
            {
                instance = docsPaWS.CreateDeclarationDocument(getInstanceAccessInSession(), UserManager.GetInfoUser(), RoleManager.GetRoleInSession());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return instance;
        }

        /// <summary>
        /// Aggiorna lo stato ENABLE dei documenti all'interno dell'istanza
        /// </summary>
        /// <param name="listInstanceAccessDocuments"></param>
        /// <returns></returns>
        public static bool UpdateInstanceAccessDocumentEnable(List<InstanceAccessDocument> listInstanceAccessDocuments)
        {
            bool result = false;
            try
            {
                result = docsPaWS.UpdateInstanceAccessDocumentEnable(listInstanceAccessDocuments.ToArray(), UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        /// <summary>
        /// Aggiorna lo stato ENABLE degli allegati all'interno dell'istanza
        /// </summary>
        /// <param name="listInstanceAccessAttachments"></param>
        /// <returns></returns>
        public static bool UpdateInstanceAccessAttachmentsEnable(List<InstanceAccessAttachments> listInstanceAccessAttachments)
        {
            bool result = false;
            try
            {
                result = docsPaWS.UpdateInstanceAccessAttachmentsEnable(listInstanceAccessAttachments.ToArray(), UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        /// <summary>
        /// Ritorna il template da associare alla dichiarazione di conformità
        /// </summary>
        /// <param name="listInstanceAccessAttachments"></param>
        /// <returns></returns>
        public static DocsPaWR.Templates GetTemplate()
        {
        
            DocsPaWR.Templates template = null;
            try
            {
                template = docsPaWS.GetTemplateInstanceAccess(UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return template;
        }

        public static SchedaDocumento ForwardsInstanceAccess(DocsPaWR.InstanceAccess instance, out int totalFileSizeInstance)
        {
            SchedaDocumento documento = null;
            totalFileSizeInstance = 0;
            try
            {
                documento = docsPaWS.ForwardsInstanceAccess(instance, UserManager.GetInfoUser(), RoleManager.GetRoleInSession(), out totalFileSizeInstance);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return documento;
        }

        #endregion

        #region structs

        public struct TipoRichiesta
        {
            public const string COPIA_CONFORME = "Copia conforme";
            public const string COPIA_SEMPLICE = "Copia semplice";
            public const string ESTRATTO = "Estratto";
            public const string DUPLCATO = "Duplicato";
        }
        #endregion
    }
}
