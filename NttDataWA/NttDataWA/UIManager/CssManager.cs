using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;

namespace NttDataWA.UIManager
{
    public class CssManager
    {       
        //private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public enum TextSize
        {
            NORMAL,
            MEDIUM,
            HIGH
        }

        public static string GetSizeText()
        {
            try
            {
                return HttpContext.Current.Session["sizeText"] as string;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void SetSizeText(string size)
        {
            try
            {
                HttpContext.Current.Session["sizeText"] = size;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
    }
}