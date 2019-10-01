using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;

namespace NttDataWA.UIManager
{
    public class PreservetionManager
    {        
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static DocsPaWR.TipoIstanzaConservazione[] GetTipologieIstanzeConservazione()
        {
            try
            {
                return docsPaWS.GetTipologieIstanzeConservazione();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
    }
}