using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace BusinessLogic.FlussoAutomatico
{
    public class FlussoAutomaticoManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(FlussoAutomaticoManager));

        /// <summary>
        /// Restituisce il messaggio iniziale di invio richiesta per il flusso RGS
        /// </summary>
        /// <returns></returns>
        public static DocsPaVO.FlussoAutomatico.Messaggio GetMessaggioInizialeFlussoProcedurale()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();
                return flusso.GetMessaggioInizialeFlussoProcedurale();
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetMessaggioInizialeFlussoProcedurale " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Verifica se il corrispondente in input è interoperante RGS
        /// </summary>
        /// <param name="idCorr"></param>
        /// <returns></returns>
        public static bool CheckIsInteroperanteRGS(string idCorr)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();
                return flusso.CheckIsInteroperanteRGS(idCorr);
            }
            catch (Exception e)
            {
                logger.Error("Errore in CheckIsInteroperanteRGS " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Restituisce la lista dei possibili messaggi con cui inviare la richiesta RGS
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public static List<DocsPaVO.FlussoAutomatico.Messaggio> GetMessaggiSuccessiviFlussoProcedurale(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            List<DocsPaVO.FlussoAutomatico.Messaggio> messaggiSuccessivi = new List<DocsPaVO.FlussoAutomatico.Messaggio>();
            DocsPaVO.FlussoAutomatico.Messaggio messaggioPrecedente = new DocsPaVO.FlussoAutomatico.Messaggio();
            try
            {
                DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();
                if (schedaDocumento.rispostaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.rispostaDocumento.docNumber))
                {
                    messaggioPrecedente = flusso.GetMessaggioFlussoProceduraleByIdDocumento(schedaDocumento.rispostaDocumento.docNumber);

                    if (messaggioPrecedente != null && !string.IsNullOrEmpty(messaggioPrecedente.ID))
                        messaggiSuccessivi = flusso.GetMessaggiSuccessiviFlusso(messaggioPrecedente.ID);
                    else
                        messaggiSuccessivi = flusso.GetMessaggiSuccessiviFlusso(messaggioPrecedente.ID);
                }
                else
                {
                    messaggiSuccessivi.Add(GetMessaggioInizialeFlussoProcedurale());
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetMessaggiSuccessiviFlussoProcedurale " + e.Message);
                return null;
            }
            return messaggiSuccessivi;
        }

        /// <summary>
        /// Ritorna il tipo contesto procedurale riguardanti il flusso RGS
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTipiContestoProcedurale()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();
                return flusso.GetTipiContestoProcedurale();
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetTipiContestoProcedurale " + e.Message);
                return null;
            }
        }


        public static List<DocsPaVO.FlussoAutomatico.ContestoProcedurale> GetListContestoProcedurale()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();
                return flusso.GetListContestoProcedurale();
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetListContestoProcedurale " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Inserisce il flusso procedurale
        /// </summary>
        /// <param name="flussoProcedurale"></param>
        /// <returns></returns>
        public static bool InsertFlussoProcedurale(DocsPaVO.FlussoAutomatico.Flusso flussoProcedurale)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();
                return flusso.InsertFlussoProcedurale(flussoProcedurale);
            }
            catch (Exception e)
            {
                logger.Error("Errore in InsertFlussoProcedurale " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Inserisce il contesto procedurale
        /// </summary>
        /// <param name="flussoProcedurale"></param>
        /// <returns></returns>
        public static bool InsertContestoProcedurale(DocsPaVO.FlussoAutomatico.ContestoProcedurale contesto)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();
                return flusso.InsertContestoProcedurale(contesto);
            }
            catch (Exception e)
            {
                logger.Error("Errore in InsertContestoProcedurale " + e.Message);
                return false;
            }
        }

        public static bool InsertCorrispondenteRGS(string idCorr, bool interoperanteRGS)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();
                return flusso.InsertCorrispondenteRGS(idCorr, interoperanteRGS);
            }
            catch (Exception e)
            {
                logger.Error("Errore in InsertCorrispondenteRGS " + e.Message);
                return false;
            }
        }

        public static bool DeleteCorrispondenteRGS(string idCorr)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();
                return flusso.DeleteCorrispondenteRGS(idCorr);
            }
            catch (Exception e)
            {
                logger.Error("Errore in InsertCorrispondenteRGS " + e.Message);
                return false;
            }
        }

        public static bool UpdateAssociazioneTemplateContestoProcedurale(string idTipoAtto, string idContestoProcedurale)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();
                return flusso.UpdateAssociazioneTemplateContestoProcedurale(idTipoAtto, idContestoProcedurale);
            }
            catch (Exception e)
            {
                logger.Error("Errore in UpdateAssociazioneTemplateContestoProcedurale " + e.Message);
                return false;
            }
        }
        

        /// <summary>
        /// Restituisce l'id del processo; se è l'invio di una nuova richiesta calcola l'id altrimenti va a prelevare l'id del processo tramite il protocollo inviato precedentemente.
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="messaggio"></param>
        /// <returns></returns>
        public static string GetIdProcessoFlusso(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.FlussoAutomatico.Messaggio messaggio)
        {
            string idProcesso = string.Empty;

            if (messaggio.INIZIALE)
            {
                idProcesso = schedaDoc.registro.codRegistro.Trim() + "_" + schedaDoc.protocollo.anno.Trim() + "_" + schedaDoc.protocollo.numero.Trim();
            }
            else
            { 
                DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();
                idProcesso = flusso.GetIdProcessoFlussoProcedurale(schedaDoc.rispostaDocumento.docNumber);
            }

            return idProcesso;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="messaggio"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Fascicolo GetFasicoloByIdProfile(DocsPaVO.utente.InfoUtente infoUtente, string idProfile)
        {
            DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
            try
            {
                DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();
                fascicolo = flusso.GetFasicoloByIdProfile(infoUtente, idProfile);

                if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID))
                    fascicolo.folderSelezionato = BusinessLogic.Fascicoli.FolderManager.GetFoldersDocument(idProfile, fascicolo.systemID).Cast<DocsPaVO.fascicolazione.Folder>().FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetFasicoloByIdProfile " + e.Message);
                return null;
            }
            return fascicolo;
        }

        /// <summary>
        /// Restituisce l'intero flusso di protocollazione automatica in cui il documento in input è coinvolto.
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <returns></returns>
        public static List<DocsPaVO.FlussoAutomatico.Flusso> GetListaFlussoDocumento(DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            List<DocsPaVO.FlussoAutomatico.Flusso> listaFlusso = new List<DocsPaVO.FlussoAutomatico.Flusso>();
            try
            {
                DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();
                string idProcesso = string.Empty;
                if (schedaDoc.rispostaDocumento != null && !string.IsNullOrEmpty(schedaDoc.rispostaDocumento.docNumber))
                {
                    idProcesso = flusso.GetIdProcessoFlussoProcedurale(schedaDoc.rispostaDocumento.docNumber);
                }
                else
                {
                    idProcesso = flusso.GetIdProcessoFlussoProcedurale(schedaDoc.systemId);
                }
                if (!string.IsNullOrEmpty(idProcesso))
                    listaFlusso = flusso.GetListFlussoByIdProcesso(idProcesso);
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetMessaggiSuccessiviFlussoProcedurale " + e.Message);
                return null;
            }
            return listaFlusso;
        }

        /// <summary>
        /// Restituisce il flusso di inizio richiesta
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <returns></returns>
        public static DocsPaVO.FlussoAutomatico.Flusso GetFlussoInizioRichiesta(string idProcesso)
        {
            try
            {
            DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();
            return flusso.GetFlussoInizioRichiesta(idProcesso);
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetFlussoInizioRichiesta " + e.Message);
                return null;
            }
        }


        /*
        /// <summary>
        /// Seleziona i possibili messaggi per la spedizione per il flusso RGS a partire richiesta precedente. Seleziona il messaggio di default
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public static DocsPaVO.FlussoAutomatico.Messaggio GetMessaggioFlussoProcedurale(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            DocsPaVO.FlussoAutomatico.Messaggio messaggio = new DocsPaVO.FlussoAutomatico.Messaggio();
            DocsPaVO.FlussoAutomatico.Messaggio messaggioPrecedente = new DocsPaVO.FlussoAutomatico.Messaggio();
            DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();

            if (schedaDocumento.template != null && !string.IsNullOrEmpty(schedaDocumento.template.ID_CONTESTO_PROCEDURALE))
            {
                if (schedaDocumento.rispostaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.rispostaDocumento.docNumber))
                {
                    messaggioPrecedente = flusso.GetMessaggioFlussoProceduraleByIdDocumento(schedaDocumento.rispostaDocumento.docNumber);
                    if(messaggioPrecedente != null && !string.IsNullOrEmpty(messaggioPrecedente.ID))
                        messaggio = flusso.GetMessaggoSuccessivoDiDefault(messaggioPrecedente.ID);
                    else
                        messaggio = GetMessaggioInizialeFlussoProcedurale();
                }
                else
                {
                    messaggio = GetMessaggioInizialeFlussoProcedurale();
                }
            }
            return messaggio;
        }
         
         
        /// <summary>
        /// Restituisce le possibili richieste a partire dalla richieste precedenti
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public static List<DocsPaVO.FlussoAutomatico.Messaggio> GetMessaggiSuccessiviFlussoProcedurale(string idProfile)
        {
            List<DocsPaVO.FlussoAutomatico.Messaggio> messaggiSuccessivi = new List<DocsPaVO.FlussoAutomatico.Messaggio>();
            DocsPaVO.FlussoAutomatico.Messaggio messaggioPrecedente = new DocsPaVO.FlussoAutomatico.Messaggio();
            try
            {
                DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new DocsPaDB.Query_DocsPAWS.FlussoAutomatico();

                messaggioPrecedente = flusso.GetMessaggioFlussoProceduraleByIdDocumento(idProfile);
                if (messaggioPrecedente != null && !string.IsNullOrEmpty(messaggioPrecedente.ID))
                    messaggiSuccessivi = flusso.GetMessaggiSuccessiviFlusso(messaggioPrecedente.ID);
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetMessaggiSuccessiviFlussoProcedurale " + e.Message);
                return null;
            }
            return messaggiSuccessivi;
        }
        */
    }
}
