using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using log4net;

namespace NttDataWA.Utils
{
    public class Common
    {
        //private static ILog logger = LogManager.GetLogger(typeof(Common));

        /// <summary>
        /// Get Application path
        /// </summary>
        /// <returns>string</returns>
        public static string GetHttpFullPath()
        {
            try
            {
                string httpRootPath = GetHttpRootPath();
                httpRootPath += HttpContext.Current.Request.ApplicationPath;
                return httpRootPath;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Get application's root path
        /// </summary>
        /// <returns>string</returns>
        private static string GetHttpRootPath()
        {
            try
            {
                string httpRootPath = string.Empty;
                if (HttpContext.Current.Session["useStaticRootPath"] != null)
                {
                    bool useStaticRoot = false;
                    if (bool.TryParse(HttpContext.Current.Session["useStaticRootPath"].ToString(), out useStaticRoot))
                    {
                        if (useStaticRoot)
                        {
                            httpRootPath = System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.STATIC_ROOT_PATH.ToString()];
                            //logger.Debug("GetHttpRootPath - useStaticRootPath");
                        }
                    }
                }
                if (string.IsNullOrEmpty(httpRootPath))
                {
                    HttpRequest request = HttpContext.Current.Request;
                    httpRootPath = request.Url.Scheme + "://" + request.Url.Host;
                    if (!request.Url.Port.Equals(80))
                        httpRootPath += ":" + request.Url.Port;
                }
                return httpRootPath;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Object[] AddToArray(Object[] array, Object nuovoElemento)
        {
            try
            {
                Object[] nuovaLista;
                if (array != null)
                {
                    int len = array.Length;
                    nuovaLista = new Object[len + 1];
                    array.CopyTo(nuovaLista, 0);
                    nuovaLista[len] = nuovoElemento;
                    return nuovaLista;
                }
                else
                {
                    nuovaLista = new Object[1];
                    nuovaLista[0] = nuovoElemento;
                    return nuovaLista;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
    }
}