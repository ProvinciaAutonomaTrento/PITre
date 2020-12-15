using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using log4net;

namespace BusinessLogic.ProfilazioneDinamica
{
    public class RicercaCampiComuni
    {
        private static ILog logger = LogManager.GetLogger(typeof(RicercaCampiComuni));

        public RicercaCampiComuni() { }

        public static ArrayList eseguiRicercaCampiComuni(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] listaFiltri, int numPage, int pageSize, out int nRec)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                ArrayList result = null;
                nRec = 0;
                
                try
                {
                    DocsPaDB.Query_DocsPAWS.RicProfCampiComuni ricProfCampiComuni = new DocsPaDB.Query_DocsPAWS.RicProfCampiComuni();
                    result = ricProfCampiComuni.eseguiRicercaCampiComuni(infoUtente, listaFiltri, numPage, pageSize, out nRec);
                    transactionContext.Complete();                    
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDinamica  - metodo: eseguiRicercaCampiComuni", e);
                    result = null;
                }

                return result;
            }
        }
    }
}
