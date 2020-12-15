using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using DocsPaVO.amministrazione;

namespace BusinessLogic.Amministrazione
{
    public class AssertionEventManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(AssertionEventManager));

        /// <summary>
        /// Returns a list of all the assertions defined for the administration passed as an input parameter
        /// </summary>
        /// <param name="idAmm"> id dell'amministrazione</param>
        /// <returns></returns>
        public static List<Assertion> GetListAssertionsEvent(string idAmm)
        {
            List<Assertion> listAssertion = new List<Assertion>();
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.AdministrationAssertionEvent assertionDb = new DocsPaDB.Query_DocsPAWS.AdministrationAssertionEvent();
                    listAssertion = assertionDb.GetListAssertionByAmm(idAmm);
                    transactionContext.Complete();
                    return listAssertion;
                }
                catch (Exception e)
                {

                    logger.Debug("Errore in Amministrazione.AssertionEventManager  - metodo: GetListAssertionsEvent", e);
                    return listAssertion;
                }
            }
        }

        /// <summary>
        /// returns the list of configurable event types.
        /// </summary>
        /// <param name="idAmm"></param>
        public static List<string> GetTypeConfigurableEvents(string idAmm)
        {
            List<string> listTypeEventConf = new List<string>();
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.AdministrationAssertionEvent assertionDb = new DocsPaDB.Query_DocsPAWS.AdministrationAssertionEvent();
                    listTypeEventConf = assertionDb.GetTypeConfigurableEvents(idAmm);
                    transactionContext.Complete();
                    return listTypeEventConf;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in Amministrazione.AssertionEventManager  - metodo: GetTypeConfigurableEvents", e);
                    return listTypeEventConf;
                }
            }
        }

        /// <summary>
        /// Insert a new assertion event.
        /// </summary>
        /// <param name="newAssertion"></param>
        /// <returns></returns>
        public static int InsertAssertionEvent(Assertion newAssertion)
        {
            int res;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.AdministrationAssertionEvent assertionDb = new DocsPaDB.Query_DocsPAWS.AdministrationAssertionEvent();
                    res = assertionDb.InsertAssertionEvent(newAssertion);
                    transactionContext.Complete();
                    
                }
                catch (Exception e)
                {

                    logger.Debug("Errore in Amministrazione.AssertionEventManager  - metodo: InsertAssertionEvent", e);
                    return -1;
                }
            }
            return res;
        }

        public static List<string> SearchAur(string typeAur, string code, string description, string idAmm)
        {
            List<string> listAur = new List<string>();
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.AdministrationAssertionEvent assertionDb = new DocsPaDB.Query_DocsPAWS.AdministrationAssertionEvent();
                    listAur = assertionDb.SearchAur(typeAur, code, description, idAmm);
                    transactionContext.Complete();
                    
                }
                catch (Exception e)
                {

                    logger.Debug("Errore in Amministrazione.AssertionEventManager  - metodo: SearchAur", e);
                    return listAur;
                }
            }
            return listAur; 
        }

        /// <summary>
        /// Insert a new assertion event.
        /// </summary>
        /// <param name="assertion"></param>
        /// <returns></returns>
        public static int UpdateAssertionEvent(Assertion assertion)
        {
            int res;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.AdministrationAssertionEvent assertionDb = new DocsPaDB.Query_DocsPAWS.AdministrationAssertionEvent();
                    res = assertionDb.UpdateAssertionEvent(assertion);
                    transactionContext.Complete();

                }
                catch (Exception e)
                {

                    logger.Debug("Errore in Amministrazione.AssertionEventManager  - metodo: UpdateAssertionEvent", e);
                    return -1;
                }
            }
            return res;
        }

        public static bool RemoveAssertions(List<Assertion> assertions)
        {
            bool result = false;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.AdministrationAssertionEvent assertionDb = new DocsPaDB.Query_DocsPAWS.AdministrationAssertionEvent();
                    result = assertionDb.RemoveAssertions(assertions);
                    transactionContext.Complete();

                }
                catch (Exception e)
                {

                    logger.Debug("Errore in Amministrazione.AssertionEventManager  - metodo: RemoveAssertions", e);
                    return result;
                }
            }
            return result;
        }
    }
}
