using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaDB;
using log4net;

namespace DepositoServiceConsumer
{
    public class DepositoServiceConsumer : DBProvider
    {
        private static ILog logger = LogManager.GetLogger(typeof(DepositoServiceConsumer));
        public DepositoServicesWR.DepService WR = new DepositoServicesWR.DepService();


        public DepositoServiceConsumer(String webReferenceUrl)
        {
            try
            {
                //La webReferenceURL viene pescata nel web.config del WS e la si valida all'atto dell'invocazione del costruttore per chiamare i suoi metodi
                new Uri(webReferenceUrl);
                WR = new DepositoServicesWR.DepService();
                WR.Url = webReferenceUrl;
            }
            catch (Exception e)
            {
                logger.DebugFormat("Error webReferenceURL: Message: {0}, StackTrace: {1}", e.Message, e.StackTrace);

                throw e;
            }
        }

        public int getIdProfileByData(string numProto, string AnnoProto, string idRegistro, string idGruppo, string idPeople)
        {
            try
            {
                DepositoServicesWR.DepService wr = new DepositoServicesWR.DepService();
                return wr.DO_getIdProfileByData(numProto, AnnoProto, idRegistro, idGruppo, idPeople);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

        }
    }
}
