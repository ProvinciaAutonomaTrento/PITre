using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VtDocsWS.WebServices;
using log4net;

namespace VtDocsWS.Manager
{
    public class CheckConnectionManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(CheckConnectionManager));

        public static Services.CheckWS.CheckConnection.CheckConnectionResponse CheckConnection(Services.CheckWS.CheckConnection.CheckConnectionRequest request) 
        {
            Services.CheckWS.CheckConnection.CheckConnectionResponse response = new Services.CheckWS.CheckConnection.CheckConnectionResponse();
            
            try
            {
                response.EsitoConnection = BusinessLogic.Amministrazione.UtenteManager.Checkconnection();
                response.Success = true;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error: Message {0}, StackTrace {1}", ex.Message, ex.StackTrace);

                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }
    }
}