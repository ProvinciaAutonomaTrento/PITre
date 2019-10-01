using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.SmartClient
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SmartClientManager
    {
        /// <summary>
        /// Verifica se l'utente risulta abilitato all'utilizzo dei componenti SmartClient
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.SmartClient.SmartClientConfigurations GetConfigurationsPerUser(DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.SmartClient smartClientDb = new DocsPaDB.Query_DocsPAWS.SmartClient(infoUtente);

            return smartClientDb.GetConfigurationsPerUser();
        }
    }
}
