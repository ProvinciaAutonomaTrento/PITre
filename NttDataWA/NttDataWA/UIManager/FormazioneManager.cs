using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;

namespace NttDataWA.UIManager
{
    public class FormazioneManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static bool PulisciUnitaOrganizzativa(string idUo)
        {
            bool result = true;
            try
            {
                result = docsPaWS.PulisciUnitaOrganizzativa(idUo, UIManager.UserManager.GetInfoUser());
            }
            catch(Exception e)
            {
                result = false;
            }
            return result;
        }

        public static bool PopolaUnitaOrganizzativa(string idUO)
        {
            bool result = true;
            try
            {
                result = docsPaWS.PopolaUnitaOrganizzativa(idUO, UIManager.UserManager.GetInfoUser());
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }
    }
}