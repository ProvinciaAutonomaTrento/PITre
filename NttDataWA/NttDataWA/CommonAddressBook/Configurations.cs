using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.DocsPaWR;
using log4net;
using NttDataWA.Utils;

namespace NttDataWA.CommonAddressBook
{
    public class Configurations
    {
        private static ILog logger = LogManager.GetLogger(typeof(Configurations));
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static ConfigurazioniRubricaComune GetConfigurations(InfoUtente infoUser)
        {
            return docsPaWS.GetConfigurazioniRubricaComune(infoUser);
        }
    }
}