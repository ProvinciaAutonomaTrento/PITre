using System;
using System.Collections;
using System.Data;
using log4net;

namespace BusinessLogic.fascicoli
{
    public class IndiceSistematico
    {
        private static ILog logger = LogManager.GetLogger(typeof(IndiceSistematico));
        public IndiceSistematico(){}

        public static void addNuovaVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.IndiceSistematico indx = new DocsPaDB.Query_DocsPAWS.IndiceSistematico();
                    indx.addNuovaVoceIndice(voceIndice);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in IndiceSistematico  - metodo: addNuovaVoceIndice", e);
                }
            }
        }

        public static void removeVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.IndiceSistematico indx = new DocsPaDB.Query_DocsPAWS.IndiceSistematico();
                    indx.removeVoceIndice(voceIndice);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in IndiceSistematico  - metodo: removeVoceIndice", e);
                }
            }
        }

        public static ArrayList getIndiceByIdAmm(string idAmm)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.IndiceSistematico indx = new DocsPaDB.Query_DocsPAWS.IndiceSistematico();
                    ArrayList indice = indx.getIndiceByIdAmm(idAmm);
                    return indice;
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in IndiceSistematico  - metodo: getIndiceByIdAmm", e);
                    return null;
                }
            }
        }

        public static void associaVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.IndiceSistematico indx = new DocsPaDB.Query_DocsPAWS.IndiceSistematico();
                    indx.associaVoceIndice(voceIndice);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in IndiceSistematico  - metodo: associaVoceIndice", e);
                }
            }
        }

        public static void dissociaVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.IndiceSistematico indx = new DocsPaDB.Query_DocsPAWS.IndiceSistematico();
                    indx.dissociaVoceIndice(voceIndice);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in IndiceSistematico  - metodo: dissociaVoceIndice", e);
                }
            }
        }

        public static ArrayList getIndiceByIdProject(string idProject)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.IndiceSistematico indx = new DocsPaDB.Query_DocsPAWS.IndiceSistematico();
                    ArrayList indice = indx.getIndiceByIdProject(idProject);
                    return indice;
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in IndiceSistematico  - metodo: getIndiceByIdProject", e);
                    return null;
                }
            }
        }

        public static ArrayList getCodNodiByIndice(string indice, string idTitolario)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.IndiceSistematico indx = new DocsPaDB.Query_DocsPAWS.IndiceSistematico();
                    ArrayList nodiDaIndice = indx.getCodNodiByIndice(indice, idTitolario);
                    return nodiDaIndice;
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in IndiceSistematico  - metodo: getCodNodoByIndice", e);
                    return null;
                }
            }
        }

        public static DocsPaVO.fascicolazione.VoceIndiceSistematico existVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.IndiceSistematico indx = new DocsPaDB.Query_DocsPAWS.IndiceSistematico();
                    return indx.existVoceIndice(voceIndice);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in IndiceSistematico  - metodo: existVoceIndice", e);
                    return null;
                }
            }
        }

        public static bool isAssociataVoce(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice, bool suSingoloNodo)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.IndiceSistematico indx = new DocsPaDB.Query_DocsPAWS.IndiceSistematico();
                    return indx.isAssociataVoce(voceIndice, suSingoloNodo);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in IndiceSistematico  - metodo: isAssociataVoce", e);
                    return false;
                }
            }
        }
    }
}
