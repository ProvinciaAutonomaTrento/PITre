using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NttDataWA.UIManager
{
    /// <summary>
    /// Gestione
    /// </summary>
    public sealed class SmistaDocSessionManager
    {
        private const string SESSION_KEY = "SMISTA_DOC_MANAGER";
        private const string SESSION_RAG_TRASM = "SMISTA_RAG_TRASM";


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SmistaDocManager GetSmistaDocManager()
        {
            return GetSmistaDocManager(string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        public static SmistaDocManager GetSmistaDocManager(string idDocumento)
        {
            if (System.Web.HttpContext.Current.Session[SESSION_KEY] == null)
            {
                SmistaDocManager retValue = new SmistaDocManager(RoleManager.GetRoleInSession(), UserManager.GetUserInSession(), UserManager.GetInfoUser(), idDocumento);
                System.Web.HttpContext.Current.Session.Add(SESSION_KEY, retValue);
            }

            return (SmistaDocManager)System.Web.HttpContext.Current.Session[SESSION_KEY];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docManager"></param>
        public static void SetSmistaDocManager(SmistaDocManager docManager)
        {
            if (System.Web.HttpContext.Current.Session["SMISTA_DOC_MANAGER"] == null)
            {
                System.Web.HttpContext.Current.Session.Add("SMISTA_DOC_MANAGER", docManager);
            }
            else
                System.Web.HttpContext.Current.Session["SMISTA_DOC_MANAGER"] = docManager;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SetSessionRagTrasm()
        {
            if (System.Web.HttpContext.Current.Session[SESSION_RAG_TRASM] == null)
            {
                System.Web.HttpContext.Current.Session.Add(SESSION_RAG_TRASM, "1");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uoApp"></param>
        public static void SetSessionUoApp(DocsPaWR.UOSmistamento uoApp)
        {
            if (System.Web.HttpContext.Current.Session["SMISTA_DOC_MANAGER"] != null)
            {
                SmistaDocManager docManager = (SmistaDocManager)System.Web.HttpContext.Current.Session["SMISTA_DOC_MANAGER"];
                docManager.SetUOAppartenenza(uoApp);
                System.Web.HttpContext.Current.Session["SMISTA_DOC_MANAGER"] = docManager;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uoInf"></param>
        public static void SetSessionUoInf(DocsPaWR.UOSmistamento[] uoInf)
        {
            if (System.Web.HttpContext.Current.Session["SMISTA_DOC_MANAGER"] != null)
            {
                SmistaDocManager docManager = (SmistaDocManager)System.Web.HttpContext.Current.Session["SMISTA_DOC_MANAGER"];
                docManager.SetUOInferiori(uoInf);
                System.Web.HttpContext.Current.Session["SMISTA_DOC_MANAGER"] = docManager;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool ExistSessionRagTrasm()
        {
            bool exist = false;
            if (System.Web.HttpContext.Current.Session[SESSION_RAG_TRASM] != null &&
                System.Web.HttpContext.Current.Session[SESSION_RAG_TRASM].ToString().Equals("1"))
            {
                exist = true;
            }
            return exist;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ReleaseSmistaDocManager()
        {
            System.Web.HttpContext.Current.Session.Remove(SESSION_KEY);
            System.Web.HttpContext.Current.Session.Remove(SESSION_RAG_TRASM);
        }
    }
}