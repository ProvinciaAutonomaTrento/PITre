// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.amministrazione;
using Serilog;

namespace BusinessLogic.Amministrazione
{
    public class AssertionEventManager
    {
        private static ILogger logger = Log.ForContext(typeof(AssertionEventManager));

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

    }
}
