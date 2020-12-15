using System;
using System.Configuration;
using System.Net;

namespace SAAdminTool {
	/// <summary>
	/// Summary description for ProxyManager.
	/// </summary>
	public class ProxyManager 
	{
		public static SAAdminTool.DocsPaWR.DocsPaWebService getWS() 
		{	
			DocsPaWR.DocsPaWebService docsPaWS = new SAAdminTool.DocsPaWR.DocsPaWebService();
			
			
			#region Gestione del timeout di connessione
			try {
                int timeOut = Int32.Parse(ConfigSettings.getKey(ConfigSettings.KeysENUM.EXECUTIONTIMEOUT));
                int l_timeOutLimit = 3600 * 1000 * 24;
                if (timeOut > l_timeOutLimit)
                {
                    ConfigSettings.setKey(ConfigSettings.KeysENUM.EXECUTIONTIMEOUT, l_timeOutLimit.ToString());
                }

                if (timeOut > docsPaWS.Timeout)
                    docsPaWS.Timeout = timeOut;	
                docsPaWS.Timeout = System.Threading.Timeout.Infinite;
			} catch (Exception) {}
			#endregion

			#region Gestione del proxy
			try {
				string proxy = ConfigSettings.getKey(ConfigSettings.KeysENUM.PROXY,"0");
				if (proxy.Equals("1")) {
					string proxyUrl = ConfigSettings.getKey(ConfigSettings.KeysENUM.PROXYURL);
					if(!proxyUrl.ToLower().StartsWith("http://"))
						proxyUrl = "http://" + proxyUrl;
					IWebProxy proxyObject = new WebProxy(proxyUrl, true);
					if(ConfigSettings.getKey(ConfigSettings.KeysENUM.PROXYCREDENTIALS).Equals("1")) {
						string username = ConfigSettings.getKey(ConfigSettings.KeysENUM.PROXYUSERNAME);
						string password = ConfigSettings.getKey(ConfigSettings.KeysENUM.PROXYPASSWORD);
						string domain = ConfigSettings.getKey(ConfigSettings.KeysENUM.PROXYDOMAIN);
						NetworkCredential myCred = new NetworkCredential(username,password,domain);
						proxyObject.Credentials = myCred;
					}
					docsPaWS.Proxy = proxyObject;																 
				}					
			} catch (Exception) {}
			#endregion

			return docsPaWS;
		}

        public static SAAdminTool.DocsPaWR.DocsPaWebService getWSForCache(string url)
        {
            DocsPaWR.DocsPaWebService docsPaWS = new SAAdminTool.DocsPaWR.DocsPaWebService();


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
            catch (Exception) { }
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
            catch (Exception) { }
            #endregion

            return docsPaWS;
        }
	}
}
