using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace BusinessLogic.FriendApplication
{
    public class FriendApplicationManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(FriendApplicationManager));

        public static DocsPaVO.FriendApplication.FriendApplication getFriendApplication(string friendApplication, string codiceRegistro)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.FriendApplication friendApplicationDB = new DocsPaDB.Query_DocsPAWS.FriendApplication();
                DocsPaVO.FriendApplication.FriendApplication friendApplicationResult = friendApplicationDB.getFriendApplication(friendApplication, codiceRegistro);
                return friendApplicationResult;
            }
            catch (Exception ex)
            {
                logger.Error("Errore in FriendApplicationManager  - metodo: getFriendApplication", ex);
                throw ex;
            }            
        }
    }    
}
