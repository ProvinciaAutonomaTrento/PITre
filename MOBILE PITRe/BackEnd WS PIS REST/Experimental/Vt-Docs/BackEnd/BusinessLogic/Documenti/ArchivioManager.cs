using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using DocsPaVO.ricerche;
using log4net;

namespace BusinessLogic.Documenti
{
    public class ArchivioManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(ArchivioManager));
        //OK VERO
        /// <summary>
        /// Aggiunge un singolo documento in deposito con valore:
        ///   -1 se il documento non viene inserito in deposito
        ///    1 se il documento viene inserito in deposito
        ///    2 se il documento viene inserito in deposito ma è classificato in + fascicoli o appartiene ad una serie
        /// </summary>
        /// <param name="idProfile"></param>
        /// </returns>
        public static string TrasfInDepositoDoc(string idProfile, string SerieOrFasc, string tipoOp)
        {
            string esito = "";
            using (DocsPaDB.TransactionContext transactionalContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                esito = doc.TrasfInDepDocumento(idProfile, SerieOrFasc, tipoOp);
                if (esito == "-1")
                {
                    logger.Debug("Errore nel trasferimento di un singolo documento in deposito (TrasfDocInDeposito)");
                    throw new Exception();
                }
                else
                {
                    transactionalContext.Complete();
                }
            }
            return esito;
        }

        //OK VERO
        /// <summary>
        /// Aggiunge un fascicolo in deposito con valore:
        ///    1 se il fascicolo contiene documenti tutti in deposito con valore 1
        ///    2 se il fascicolo contiene anche solo un documento in deposito con valore 2
        /// </summary>
        /// <param name="idProject"></param>
        /// <param name="infoUtente"></param>
        /// <param name="valore"></param>
        /// <returns></returns>
        private static bool TrasfInDepositoFascicolo(string idProject, string valore, string tipoOp)
        {
            bool result = false;
            DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            result = fasc.TrasfInDepFasc(idProject, valore, tipoOp);
            if (!result)
            {
                logger.Debug("Errore nel trasferimento di un fascicolo procedimentale in deposito (TrasfInDepositoFascProc)");
                throw new Exception();
            }
            return result;
        }

        //OK VERO
        /// <summary>
        /// Inserisce in deposito tutti i documenti in un dato fascicolo generale
        /// Inserisce in deposito il fascicolo generale: il valore di questo in archivio
        /// sarà sempre 2
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <param name="anno"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static int TrasfInDepositoAllDocsFascGen(DocsPaVO.fascicolazione.Fascicolo fascicolo, string anno, DocsPaVO.utente.InfoUtente infoUtente, string tipoOp)
        {
            int docInDep2 = 0;
            bool result = false;
            using (DocsPaDB.TransactionContext transactionalContext = new DocsPaDB.TransactionContext())
            {
                //Recupero la lista di tutti i documenti contenuti in un dato fascicolo generale
                DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                ArrayList listaDoc = fasc.GetDocumentiDaArchiviare(infoUtente.idGruppo, infoUtente.idPeople, fascicolo, anno);

                string resInsDoc = "";
                for (int i = 0; i < listaDoc.Count; i++)
                {
                    DocsPaVO.documento.InfoDocumento infoDoc = (DocsPaVO.documento.InfoDocumento)listaDoc[i];
                    resInsDoc = TrasfInDepositoDoc(infoDoc.idProfile, "0", tipoOp);
                    if (resInsDoc == "-1")
                        break;

                    if (resInsDoc == "2")
                        docInDep2++;
                }

                if (resInsDoc != "-1")
                {
                    //il fascicolo genereale viene inserito in deposito sempre con valore a 2
                    result = TrasfInDepositoFascicolo(fascicolo.systemID, "2", tipoOp);
                }
                if (result)
                    transactionalContext.Complete();
            }
            return docInDep2;
        }

        //OK
        /// <summary>
        /// Inserisce in deposito una lista di fascicoli procedimentali chiusi. Per ogni fascicolo
        /// procedimentale chiuso:
        ///         Inserisce in deposito tutti i documenti del fascicolo
        ///         Inserisce in deposito il fascicolo procedimentale: il valore di questo in archivio
        ///         sarà sempre 1
        /// </summary>
        /// <param name="filtriRic"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool TrasfInDepositoAllFascProc(DocsPaVO.filtri.FiltroRicerca[][] filtriRic, DocsPaVO.utente.InfoUtente infoUtente, string tipoOp)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            int res = 0;      
            bool result = true;
            ArrayList listaFasc = null;
            using (DocsPaDB.TransactionContext transactionalContext = new DocsPaDB.TransactionContext())
            {
                //Ricerca tutti i fascicoli procedimentali chiusi
                listaFasc = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoli(infoUtente, null, filtriRic[0], false, false, false, null, "");

                for (int i = 0; i < listaFasc.Count; i++)
                {
                    DocsPaVO.fascicolazione.Fascicolo fasc = (DocsPaVO.fascicolazione.Fascicolo)listaFasc[i];
                    if (!TrasfInDepositoFascicoloProc(fasc.systemID, tipoOp))
                        res++;
                    ////Per ogni fascicolo inserisco in archivio tutti i documenti contenuti
                    //ArrayList lista = new ArrayList();
                    //lista = fascicoli.getIdDocFasc(fasc.systemID);
                    //string resInsDoc = "";
                    //int docInDep2 = 0;
                    //string valore;
                    //for (int k = 0; k < lista.Count; k++)
                    //{
                    //    //Trasferimento in deposito dell'i-esimo documento del fascicolo
                    //    resInsDoc = TrasfInDepositoDoc(lista[k].ToString(), "0", tipoOp);

                    //    if (resInsDoc == "-1")
                    //        break;

                    //    if (resInsDoc == "2")
                    //        docInDep2++;
                    //}
                    //if (resInsDoc != "-1")
                    //{
                    //    //il fascicolo procedimentale chiuso viene inserito in deposito sempre con valore a 1
                    //    result = TrasfInDepositoFascicolo(fasc.systemID, "1", tipoOp);
                    //}
                }
                if (res > 0) 
                        result = false;
                if (result)
                    transactionalContext.Complete();
            }
            return result;
        }

        //OK
        /// <summary>
        /// Inserisce in deposito un singolo fascicolo procedimentale chiuso. 
        /// Inserisce in deposito tutti i documenti del fascicolo
        /// </summary>
        /// <param name="fascID"></param>
        /// <returns></returns>
        public static bool TrasfInDepositoFascicoloProc(string fascID, string tipoOp)
        {
            bool result = true;
            using (DocsPaDB.TransactionContext transactionalContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                //Recupero i documenti contenuti nel fascicolo per inserirli in deposito uno ad uno
                string resInsDoc = insDocInFascinArchivio(fascID, tipoOp, false);
                if (resInsDoc != "-1")
                {
                    //Inserisco in archivio anche tutti i sottofascicoli del fascicolo
                    ArrayList ListaSottoFasc = new ArrayList();
                    ListaSottoFasc = fascicoli.getSottoFascicoli(fascID);
                    if (ListaSottoFasc != null && ListaSottoFasc.Count>0)
                    {
                        for (int i = 0; i < ListaSottoFasc.Count; i++)
                        {
                            resInsDoc = insDocInFascinArchivio(ListaSottoFasc[i].ToString(), tipoOp, true);
                            if (resInsDoc == "-1")
                                break;
                            else
                            {
                                //Il sottofascicolo viene inserito in archivio con valore sempre uguale a 1
                                result = TrasfInDepositoFascicolo(ListaSottoFasc[i].ToString(), "1", tipoOp);
                            }

                        }
                    }
                    if (resInsDoc != "-1" && result)
                    {
                        //Il fascicolo procedimentale viene inserito in archivio con valore sempre uguale a 1
                        result = TrasfInDepositoFascicolo(fascID, "1", tipoOp);
                    }
                }
                if (result)
                    transactionalContext.Complete();
            }
            return result;
        }

        //Recupero i documenti contenuti nel fascicolo per inserirli in deposito uno ad uno
        private static string insDocInFascinArchivio(string fascId, string tipoOp, bool sottoFascicolo)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            ArrayList listaDoc = new ArrayList();
            if (!sottoFascicolo)
            {
                List<SearchResultInfo> tempList = fascicoli.getIdDocFasc(fascId);
                foreach (SearchResultInfo temp in tempList) listaDoc.Add(temp.Id);
            }
            else
            {
                listaDoc = fascicoli.getIdDocSottoFasc(fascId);
            } 
            string resInsDoc = "";
            if (listaDoc != null && listaDoc.Count > 0)
            {
                for (int i = 0; i < listaDoc.Count; i++)
                {
                    //Inserisco il documento in deposito
                    resInsDoc = TrasfInDepositoDoc(listaDoc[i].ToString(), "0", tipoOp);
                    if (resInsDoc == "-1")
                        break;
                }
            }
            return resInsDoc;
        }

        /// <summary>
        /// Inserisce in deposito una serie di documenti
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static int TrasfInDepositoSerie(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.ProfilazioneDinamica.Templates template,string anno, string valOggetto, string tipoOp, string rfAOO)
        {
            bool result = false;
            int docInDep2 = 0;
            using (DocsPaDB.TransactionContext transactionalContext = new DocsPaDB.TransactionContext())
            {
                ArrayList listaInfoDoc = new ArrayList();
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                //Recupero la serie di documenti
                listaInfoDoc = doc.ListaDocumentiInSerie(listaInfoDoc, template, anno, valOggetto, rfAOO);
                string resInsDoc = "";
                for (int i = 0; i < listaInfoDoc.Count; i++)
                {
                    DocsPaVO.documento.InfoDocumento infoDoc = (DocsPaVO.documento.InfoDocumento)listaInfoDoc[i];
                    //Trasferimento in deposito dell'i-esimo documento del fascicolo
                    resInsDoc = TrasfInDepositoDoc(infoDoc.idProfile,"1", tipoOp);
                    if (resInsDoc == "-1")
                        break;
                     if (resInsDoc == "2")
                        docInDep2++;
                }
                if (resInsDoc == "-1")
                    result = false;
                else result = true;
                if (result)
                    transactionalContext.Complete();
            }
            return docInDep2;
        }
    }
}
