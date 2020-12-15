using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Net;
using Debugger = ConservazioneWA.Utils.Debugger;

namespace ConservazioneWA.Utils
{
    public class ProxyManager
    {
        public WSConservazioneLocale.DocsPaConservazioneWS getProxy()
        {
            // WSConservazioneLocale.DocsPaConservazioneWS wss = new ConservazioneWA.WSConservazioneLocale.DocsPaConservazioneWS();
            WSConservazioneLocale.DocsPaConservazioneWS wss = new ConservazioneWA.WSConservazioneLocale.DocsPaConservazioneWS();
            wss.Timeout = 3600000;

            //#region Gestione del timeout di connessione
            //try
            //{
            //    int timeOut = Int32.Parse(ConfigurationManager.AppSettings["EXECUTIONTIMEOUT"].ToString());
            //    //int l_timeOutLimit = 3600 * 1000 * 24;

            //    if (timeOut > 0)
            //        wss.Timeout = timeOut;
            //}
            //catch (Exception e)
            //{
            //    Debugger.Write("Errore nella gestione del timeout del getProxy: " + e.Message);
            //}
            //#endregion

            #region Gestione del proxy
            try
            {
                string proxy = ConfigurationManager.AppSettings["PROXY"].ToString();
                if (string.IsNullOrEmpty(proxy))
                {
                    proxy = "0";
                }
                if (proxy.Equals("1"))
                {
                    string proxyUrl = ConfigurationManager.AppSettings["PROXYURL"].ToString();
                    if (!proxyUrl.ToLower().StartsWith("http://"))
                        proxyUrl = "http://" + proxyUrl;
                    IWebProxy proxyObject = new WebProxy(proxyUrl, true);
                    if (ConfigurationManager.AppSettings["PROXYCREDENTIALS"].ToString().Equals("1"))
                    {
                        string username = ConfigurationManager.AppSettings["PROXYUSERNAME"].ToString();
                        string password = ConfigurationManager.AppSettings["PROXYPASSWORD"].ToString();
                        string domain = ConfigurationManager.AppSettings["PROXYDOMAIN"].ToString();
                        NetworkCredential myCred = new NetworkCredential(username, password, domain);
                        proxyObject.Credentials = myCred;
                    }
                    wss.Proxy = proxyObject;
                }
            }
            catch (Exception e)
            {
                Debugger.Write("Errore nella gestione del proxy del getProxy: " + e.Message);
            }
            #endregion

            wss.Timeout = System.Threading.Timeout.Infinite;

            return wss;
        }


        public DocsPaWR.DocsPaWebService getProxyDocsPa()
        {
            DocsPaWR.DocsPaWebService wss = new ConservazioneWA.DocsPaWR.DocsPaWebService();
            wss.Timeout = 3600000;

            #region Gestione del timeout di connessione
            try
            {
                int timeOut = Int32.Parse(ConfigurationManager.AppSettings["EXECUTIONTIMEOUT"].ToString());
                //int l_timeOutLimit = 3600 * 1000 * 24;

                if (timeOut > 0)
                    wss.Timeout = timeOut;
            }
            catch (Exception e) 
            {
                Debugger.Write("Errore nella gestione del timeout del getProxyDocsPa: " + e.Message);
            }
            #endregion

            #region Gestione del proxy
            try
            {
                string proxy = ConfigurationManager.AppSettings["PROXY"].ToString();
                if (string.IsNullOrEmpty(proxy))
                {
                    proxy = "0";
                }
                if (proxy.Equals("1"))
                {
                    string proxyUrl = ConfigurationManager.AppSettings["PROXYURL"].ToString();
                    if (!proxyUrl.ToLower().StartsWith("http://"))
                        proxyUrl = "http://" + proxyUrl;
                    IWebProxy proxyObject = new WebProxy(proxyUrl, true);
                    if (ConfigurationManager.AppSettings["PROXYCREDENTIALS"].ToString().Equals("1"))
                    {
                        string username = ConfigurationManager.AppSettings["PROXYUSERNAME"].ToString();
                        string password = ConfigurationManager.AppSettings["PROXYPASSWORD"].ToString();
                        string domain = ConfigurationManager.AppSettings["PROXYDOMAIN"].ToString();
                        NetworkCredential myCred = new NetworkCredential(username, password, domain);
                        proxyObject.Credentials = myCred;
                    }
                    wss.Proxy = proxyObject;
                }
            }
            catch (Exception e)
            {
                Debugger.Write("Errore nella gestione del proxy del getProxyDocsPa: " + e.Message);
            }
            #endregion

            wss.Timeout = System.Threading.Timeout.Infinite;

            return wss;
        }

        //public WSConservazioneLocale.DocsPaConservazioneWSwse getWse()
        //{
        //    WSConservazioneLocale.DocsPaConservazioneWSwse docsPaWS = new ConservazioneWA.WSConservazioneLocale.DocsPaConservazioneWSwse();

        //    #region Gestione del timeout di connessione
        //    try
        //    {
        //        int timeOut = Int32.Parse(ConfigurationManager.AppSettings["EXECUTIONTIMEOUT"].ToString());
        //        //int l_timeOutLimit = 3600 * 1000 * 24;

        //        if (timeOut > docsPaWS.Timeout)
        //            docsPaWS.Timeout = timeOut;
        //    }
        //    catch (Exception) { }
        //    #endregion

        //    #region Gestione del proxy
        //    try
        //    {
        //        string proxy = ConfigurationManager.AppSettings["PROXY"].ToString();
        //        if (string.IsNullOrEmpty(proxy))
        //        {
        //            proxy = "0";
        //        }
        //        if (proxy.Equals("1"))
        //        {
        //            string proxyUrl = ConfigurationManager.AppSettings["PROXYURL"].ToString();
        //            if (!proxyUrl.ToLower().StartsWith("http://"))
        //                proxyUrl = "http://" + proxyUrl;
        //            IWebProxy proxyObject = new WebProxy(proxyUrl, true);
        //            if (ConfigurationManager.AppSettings["PROXYCREDENTIALS"].ToString().Equals("1"))
        //            {
        //                string username = ConfigurationManager.AppSettings["PROXYUSERNAME"].ToString();
        //                string password = ConfigurationManager.AppSettings["PROXYPASSWORD"].ToString();
        //                string domain = ConfigurationManager.AppSettings["PROXYDOMAIN"].ToString();
        //                NetworkCredential myCred = new NetworkCredential(username, password, domain);
        //                proxyObject.Credentials = myCred;
        //            }
        //            docsPaWS.Proxy = proxyObject;
        //        }
        //    }
        //    catch (Exception) { }
        //    #endregion

        //    return docsPaWS;
        //}
    }
}
