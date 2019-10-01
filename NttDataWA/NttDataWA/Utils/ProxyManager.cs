using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using NttDataWA;

namespace NttDataWA.Utils
{
    public class ProxyManager
    {
        /// <summary>
        /// Gestione della connessione al Web Service
        /// </summary>
        /// <returns></returns>
        public static DocsPaWR.DocsPaWebService GetWS()
        {
            DocsPaWR.DocsPaWebService docsPaWS = new DocsPaWR.DocsPaWebService();

            try
            {
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.EXECUTIONTIMEOUT.ToString()]))
                {
                    int timeOut = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.EXECUTIONTIMEOUT.ToString()]);
                    int l_timeOutLimit = 3600 * 1000 * 24;
                    if (timeOut > l_timeOutLimit)
                    {
                        timeOut = l_timeOutLimit;
                    }

                    if (timeOut > docsPaWS.Timeout)
                        docsPaWS.Timeout = timeOut;
                }
                else
                {
                    docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

            try
            {
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.PROXY.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.PROXY.ToString()].ToString().Equals("1"))
                {
                    string proxy = System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.PROXY.ToString()].ToString();
                    if (proxy.Equals("1"))
                    {
                        string proxyUrl = string.Empty;
                        if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.PROXYURL.ToString()]))
                        {
                            proxyUrl = System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.PROXYURL.ToString()].ToString();
                            if (!string.IsNullOrEmpty(proxyUrl) && !proxyUrl.ToLower().StartsWith("http://"))
                                proxyUrl = "http://" + proxyUrl;
                        }
                        IWebProxy proxyObject = new WebProxy(proxyUrl, true);
                        if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.PROXYCREDENTIALS.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.PROXYCREDENTIALS.ToString()].ToString().Equals("1"))
                        {
                            string username = System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.PROXYUSERNAME.ToString()];
                            string password =System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.PROXYPASSWORD.ToString()];
                            string domain =System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.PROXYDOMAIN.ToString()];
                            NetworkCredential myCred = new NetworkCredential(username, password, domain);
                            proxyObject.Credentials = myCred;
                        }
                        docsPaWS.Proxy = proxyObject;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

            return docsPaWS;
        }

        public static DocsPaWR.DocsPaWebService getWSForCache(string url)
        {
            DocsPaWR.DocsPaWebService docsPaWS = new DocsPaWR.DocsPaWebService();

            #region Gestione del timeout di connessione
            try
            {
                int timeOut = Int32.Parse(ConfigSettings.getKey(ConfigSettings.KeysENUM.EXECUTIONTIMEOUT));
                int l_timeOutLimit = 3600 * 1000 * 24;
                if (timeOut > l_timeOutLimit)
                {
                    ConfigSettings.setKey(ConfigSettings.KeysENUM.EXECUTIONTIMEOUT, l_timeOutLimit.ToString());
                }

                if (timeOut > docsPaWS.Timeout)
                    docsPaWS.Timeout = timeOut;
                docsPaWS.Timeout = System.Threading.Timeout.Infinite;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            #endregion

            #region Gestione del proxy
            try
            {
                string proxy = ConfigSettings.getKey(ConfigSettings.KeysENUM.PROXY, "0");
                if (proxy.Equals("1"))
                {
                    string proxyUrl = ConfigSettings.getKey(url);
                    if (!proxyUrl.ToLower().StartsWith("http://"))
                        proxyUrl = "http://" + proxyUrl;
                    IWebProxy proxyObject = new WebProxy(proxyUrl, true);
                    if (ConfigSettings.getKey(ConfigSettings.KeysENUM.PROXYCREDENTIALS).Equals("1"))
                    {
                        string username = ConfigSettings.getKey(ConfigSettings.KeysENUM.PROXYUSERNAME);
                        string password = ConfigSettings.getKey(ConfigSettings.KeysENUM.PROXYPASSWORD);
                        string domain = ConfigSettings.getKey(ConfigSettings.KeysENUM.PROXYDOMAIN);
                        NetworkCredential myCred = new NetworkCredential(username, password, domain);
                        proxyObject.Credentials = myCred;
                    }
                    docsPaWS.Proxy = proxyObject;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            #endregion

            return docsPaWS;
        }
    }
}