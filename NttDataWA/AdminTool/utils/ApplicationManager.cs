using System;
using System.Configuration;
using SAAdminTool.DocsPaWR;
using System.Web.UI;
using System.Web;
using System.Globalization;
using System.Xml;
using System.Collections;
using System.Data;
using log4net;
using System.Collections.Generic;
using  SAAdminTool.AdminTool.Manager;

namespace SAAdminTool
{
    /// <summary>
    /// Summary description for ApplicationManager.
    /// </summary>
    public class ApplicationManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(ApplicationManager));
        private static SAAdminTool.DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();

        public static void removeSelectedFile(Page page)
        {
            logger.Info("BEGIN");
            page.Session.Remove("docsalva.bool");
            logger.Info("END");
        }

        public static string getApplicationName(Page page)
        {
            string retValue = string.Empty;
            try
            {
                retValue = docsPaWS.getApplicationName();
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return retValue;
        }

        public static ExtApplication[] getListaApplicazioni(Page page)
        {
            ExtApplication[] result = null;
            try
            {
                result = docsPaWS.GetExtApplications();

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception exception)
            {
                ErrorManager.redirect(page, exception);
            }

            return result;
        }

      public static SAAdminTool.DocsPaWR.ValidationResultInfo ExtAppDeleteUte(String idApplicazione, String idUtente)
      {
          AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
          SAAdminTool.DocsPaWR.ValidationResultInfo retValue = docsPaWS.ExtAppDeleteUte(idApplicazione, idUtente);
          return retValue;
      }

      public static SAAdminTool.DocsPaWR.ValidationResultInfo ExtAppAddUte(String idApplicazione, String idUtente)
      {
          AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
          SAAdminTool.DocsPaWR.ValidationResultInfo retValue = docsPaWS.ExtAppAddUte(idApplicazione, idUtente);
          return retValue;
      }
    }
}