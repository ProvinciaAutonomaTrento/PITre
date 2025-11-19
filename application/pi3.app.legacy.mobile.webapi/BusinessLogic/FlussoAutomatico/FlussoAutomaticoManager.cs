// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;

namespace BusinessLogic.FlussoAutomatico
{
    public class FlussoAutomaticoManager
    {
        private static ILogger logger = Log.ForContext(typeof(FlussoAutomaticoManager));

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

    }
}
