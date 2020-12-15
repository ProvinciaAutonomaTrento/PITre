using DocsPaVO.Plugin;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace BusinessLogic.Plugin
{
    public class PluginHashBL
    {
        private static ILog logger = LogManager.GetLogger(typeof(PluginHashBL));

        public static DpaPluginHash GetHashMail(string hashFile)
        {
            DpaPluginHash dpaPluginHash = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.PluginHashDB pluginHashDB = new DocsPaDB.Query_DocsPAWS.PluginHashDB();
                dpaPluginHash = pluginHashDB.GetHashMail(hashFile);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.Plugin.PluginHashBL  - metodo: GetHashMail ", e);
            }
            return dpaPluginHash;
        }

        public static bool NewHashMail(string idProfile, string idPeople, string hashFile)
        {
            DocsPaDB.Query_DocsPAWS.PluginHashDB pluginHashDB = new DocsPaDB.Query_DocsPAWS.PluginHashDB();
            return pluginHashDB.NewHashMail(idProfile, idPeople, hashFile);
        }
    }
}
