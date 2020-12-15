using System;
using System.Collections;
using DocsPAWA.DocsPaWR;


namespace DocsPAWA.smistaDoc
{
	/// <summary>
	/// Gestione
	/// </summary>
	public sealed class SmistaDocSessionManager
	{
		private const string SESSION_KEY="SMISTA_DOC_MANAGER";
		private const string SESSION_RAG_TRASM="SMISTA_RAG_TRASM";


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DocsPAWA.smistaDoc.SmistaDocManager GetSmistaDocManager()
        {
            return GetSmistaDocManager(string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
		public static DocsPAWA.smistaDoc.SmistaDocManager GetSmistaDocManager(string idDocumento)
		{
			if (System.Web.HttpContext.Current.Session[SESSION_KEY]==null)
			{
				DocsPAWA.smistaDoc.SmistaDocManager retValue=new DocsPAWA.smistaDoc.SmistaDocManager(UserManager.getRuolo(), UserManager.getUtente(), UserManager.getInfoUtente(), idDocumento);
				System.Web.HttpContext.Current.Session.Add(SESSION_KEY,retValue);
			}

			return (DocsPAWA.smistaDoc.SmistaDocManager) System.Web.HttpContext.Current.Session[SESSION_KEY];            
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docManager"></param>
        public static void SetSmistaDocManager(DocsPAWA.smistaDoc.SmistaDocManager docManager)
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
			if (System.Web.HttpContext.Current.Session[SESSION_RAG_TRASM]==null)
			{			
				System.Web.HttpContext.Current.Session.Add(SESSION_RAG_TRASM,"1");
			}		
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uoApp"></param>
        public static void SetSessionUoApp(DocsPAWA.DocsPaWR.UOSmistamento uoApp)
        {
            if (System.Web.HttpContext.Current.Session["SMISTA_DOC_MANAGER"] != null)
            {
                DocsPAWA.smistaDoc.SmistaDocManager docManager = (DocsPAWA.smistaDoc.SmistaDocManager)System.Web.HttpContext.Current.Session["SMISTA_DOC_MANAGER"];
                docManager.SetUOAppartenenza(uoApp);
                System.Web.HttpContext.Current.Session["SMISTA_DOC_MANAGER"] = docManager;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uoInf"></param>
        public static void SetSessionUoInf(DocsPAWA.DocsPaWR.UOSmistamento[] uoInf)
        {
            if (System.Web.HttpContext.Current.Session["SMISTA_DOC_MANAGER"] != null)
            {
                DocsPAWA.smistaDoc.SmistaDocManager docManager = (DocsPAWA.smistaDoc.SmistaDocManager)System.Web.HttpContext.Current.Session["SMISTA_DOC_MANAGER"];
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
			if (System.Web.HttpContext.Current.Session[SESSION_RAG_TRASM]!=null &&
				System.Web.HttpContext.Current.Session[SESSION_RAG_TRASM].ToString().Equals("1"))
			{			
				exist=true;
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
