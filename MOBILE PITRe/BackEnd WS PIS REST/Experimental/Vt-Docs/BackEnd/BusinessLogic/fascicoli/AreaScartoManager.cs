using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using log4net;

namespace BusinessLogic.fascicoli
{
    public class AreaScartoManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(AreaScartoManager));
        //Recupera la lista dei fascicoli procedimentali chiusi in deposito per un dato fascicolo generale
        //OK
        public static System.Collections.ArrayList getListaFascicoliInDeposito(DocsPaVO.utente.InfoUtente infoUtente,
                        DocsPaVO.fascicolazione.Fascicolo fascicolo,
                        int numPage, out int numTotPage,
                        out int nRec, string tipoRic)
        {
            numTotPage = 0;
            nRec = 0;
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            ArrayList result = null;
            if (tipoRic == "C")
                result = fascicoli.GetListaFascicoliInDeposito(infoUtente, fascicolo, numPage, out numTotPage, out nRec);
            else
                result = fascicoli.GetListaFascicoliRicInDeposito(infoUtente, tipoRic, numPage, out numTotPage, out nRec);
            if (result == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (getListaFascicoliInDeposito)");
                throw new Exception("F_System");
            }
            return result;
        }


        //Verifica che non ci sia già una nuova istanza di scarto per un dato utente e gruppo
        //OK
        public static int isPrimaIstanzaScarto(string idPeople, string idGruppo)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            int result = 0;
            result = fasc.isPrimaIstanzaScarto(idPeople, idGruppo);
            if (result == -1)
                throw new Exception();
            return result;
        }


        public static bool addAllFascInScarto(DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.utente.InfoUtente infoUtente, string tipoRic)
        {
            bool result = true;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                result = fasc.addAllFascAreaScarto(fascicolo, infoUtente, tipoRic);
                if (!result)
                {
                    logger.Debug("Errore nell' inserimento dei fascicoli in area di scarto (metodo: addAllFascInScarto)");
                    throw new Exception();
                }

                transactionContext.Complete();
                return result;
            }

        }


        //Aggiunge un singolo fascicolo all'area di scarto
        //OK
        public static string addDocumentiFascicoloInAreaScarto(string idProfile, string idProject, string docNumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string result = String.Empty;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                result = fasc.addDocumentiFascAreaScarto(idProfile, idProject, docNumber, infoUtente);
                if (result == "-1")
                {
                    logger.Debug("Errore nel'inserimento del fascicolo in area di scarto (metodo: addAreaLavoroMethod)");
                    throw new Exception();
                }

                transactionContext.Complete();
                return result;
            }

        }

        //Elimina i documenti di un fascicolo dall'istanza dell'area di scarto
        //OK
        public static void cancellaFascAreaScarto(DocsPaVO.fascicolazione.Fascicolo fasc, string idIstanza, bool deleteIstanza, string systemId)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicolo = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            bool result = true;
            if (fasc != null)
            {
                result = fascicolo.DeleteAreaScarto(fasc.systemID, idIstanza, deleteIstanza, systemId);
            }
            if (!result)
                throw new Exception();
        }


        public static ArrayList getListaScarto(DocsPaVO.utente.InfoUtente infoUtente, int numPage, out int numTotPage, out int nRec)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            ArrayList listaScarto = fascicoli.GetListaScarto(infoUtente, numPage, out numTotPage, out nRec);

            if (listaScarto == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (metodo: getListaScarto)");
                throw new Exception("F_System");
            }
            return listaScarto;
        }

        public static System.Collections.ArrayList getListaFascicoliInScarto(DocsPaVO.utente.InfoUtente infoUtente,
                       DocsPaVO.AreaScarto.InfoScarto infoScarto,
                       int numPage, out int numTotPage,
                       out int nRec)
        {
            numTotPage = 0;
            nRec = 0;
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            ArrayList result = fascicoli.GetListaFascicoliInScarto(infoUtente, infoScarto, numPage, out numTotPage, out nRec);

            if (result == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (getListaFascicoliInScarto)");
                throw new Exception("F_System");
            }
            return result;
        }

        public static System.Collections.ArrayList getListaFascicoliInScartoNoPaging(DocsPaVO.utente.InfoUtente infoUtente,
                      DocsPaVO.AreaScarto.InfoScarto infoScarto)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            ArrayList result = fascicoli.GetListaFascicoliInScartoNoPaging(infoUtente, infoScarto);

            if (result == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (getListaFascicoliInScarto)");
                throw new Exception("F_System");
            }
            return result;
        }

        public static bool cambiaStatoScarto(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.AreaScarto.InfoScarto infoScarto, string nuovoCampo)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            bool result = true;
            bool resultScarto = true;
            if (infoScarto.stato == "S")
            {
                //stato = S = scartato -> si deve eliminare fisicamente tutta l'istanza 
                //dell'area di scarto selezionata
                //per ogni fascicolo procedimentale chiuso che si vuole scartare
                //si devono eliminare fisicamenti tutti i documenti in esso contenuti che
                //hanno cha_in_archivio = 1, tutti i sottofascicoli ed il fascicolo stesso. 
                ArrayList ListaFasc = fascicoli.GetListaFascicoliInScartoNoPaging(infoUtente, infoScarto);
                foreach (DocsPaVO.fascicolazione.Fascicolo fasc in ListaFasc)
                {
                    //per ogni fascicolo verifico se è possibile scartare i documenti in esso classificati
                    ArrayList ListaDoc = fascicoli.GetDocumentiDaScartare(infoUtente.idGruppo, infoUtente.idPeople, "", fasc);
                    //Elimino i documenti se sono in archivio in stato 1
                    eliminaListaDoc(infoUtente, ListaDoc);
                    //Prelevo tutti i sottofascicoli del fascicolo:
                    ArrayList ListaSottoFasc = new ArrayList();
                    ListaSottoFasc = fascicoli.getSottoFascicoli(fasc.systemID);
                    bool resultSottoFasc = true;
                    if (ListaSottoFasc.Count > 0)
                    {
                        for (int i = 0; i < ListaSottoFasc.Count; i++)
                        {
                            ArrayList ListaDocSottoFasc = fascicoli.GetDocumentiDaScartare(infoUtente.idGruppo, infoUtente.idPeople, ListaSottoFasc[i].ToString(), null);
                            //Elimino i documenti del sottofascicolo se sono in archivio in stato 1
                            eliminaListaDoc(infoUtente, ListaDocSottoFasc);
                            //elimino il sottofascicolo
                            resultSottoFasc = fascicoli.EliminaFasc(infoUtente, ListaSottoFasc[i].ToString());
                        }
                    }
                    else
                    {
                        //elimino il fascicolo
                        if (resultSottoFasc)
                            resultScarto = fascicoli.EliminaFasc(infoUtente, fasc.systemID);
                        if (resultScarto)
                        {
                            infoScarto.stato = "S";
                            result = fascicoli.CambiaStatoScarto(infoUtente, infoScarto, nuovoCampo);
                        }
                    }
                }


            }
            if (infoScarto.stato != "S")
                result = fascicoli.CambiaStatoScarto(infoUtente, infoScarto, nuovoCampo);

            if (!result)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (CambiaStatoScarto)");
                throw new Exception("F_System");
            }
            return result;
        }

        private static bool eliminaListaDoc(DocsPaVO.utente.InfoUtente infoUtente, ArrayList ListaDoc)
        {
            bool resultDelete = false;
            foreach (DocsPaVO.documento.InfoDocumento infoDoc in ListaDoc)
            {
                resultDelete = BusinessLogic.Documenti.DocManager.EliminaDoc(infoUtente, infoDoc);
                if (!resultDelete)
                {
                    logger.Debug("Errore nell'eliminazione dei documenti. (eliminaListaDoc)");
                    throw new Exception("F_System");
                    resultDelete = false;
                }
            }
            return resultDelete;
        }

        public static bool updateScarto(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.AreaScarto.InfoScarto infoScarto)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            bool result = fascicoli.UpdateScarto(infoUtente, infoScarto);

            if (!result)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (UpdateScarto)");
                throw new Exception("F_System");
            }
            return result;
        }
    }
}

