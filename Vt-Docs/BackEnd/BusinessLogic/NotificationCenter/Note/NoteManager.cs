using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DocsPaVO.Note;
using DocsPaVO.utente;

namespace BusinessLogic.Note
{
    /// <summary>
    /// Classe per la gestione delle note associate a documenti o fascicoli
    /// </summary>
    public sealed class NoteManager
    {
        private NoteManager()
        { }

        #region Public methods

        /// <summary>
        /// Aggiornamento batch di una lista di note
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="oggettoAssociato"></param>
        /// <param name="note">
        /// Lista delle note da aggiornare
        /// </param>
        /// <returns>
        /// Lista delle note aggiornate
        /// </returns>
        public static InfoNota[] Update(InfoUtente infoUtente, AssociazioneNota oggettoAssociato, InfoNota[] note)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                note = GetInstanceDb(infoUtente).Update(oggettoAssociato, note);
                
                transactionContext.Complete();
            }

            return note;

        }

        /// <summary>
        /// Inserimento di una nuova nota da associare ad un documento / fascicolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocumento"></param>
        /// <param name="nota"></param>
        /// <returns></returns>
        public static InfoNota InsertNota(InfoUtente infoUtente, AssociazioneNota oggettoAssociato, InfoNota nota)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                nota = GetInstanceDb(infoUtente).InsertNota(oggettoAssociato, nota);

                transactionContext.Complete();
            }

            return nota;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="nota"></param>
        /// <returns></returns>
        public static InfoNota UpdateNota(InfoUtente infoUtente, InfoNota nota)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                nota = GetInstanceDb(infoUtente).UpdateNota(nota);
                
                transactionContext.Complete();
            }

            return nota;
        }

        /// <summary>
        /// Cancellazione di una nota esistente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idNota"></param>
        public static void DeleteNota(InfoUtente infoUtente, string idNota)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                GetInstanceDb(infoUtente).DeleteNota(idNota);

                transactionContext.Complete();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idNota"></param>
        /// <returns></returns>
        public static string GetNotaAsString(InfoUtente infoUtente, string idNota)
        {
            return GetInstanceDb(infoUtente).GetNotaAsString(idNota);
        }

        /// <summary>
        /// Reperimento di una nota esistente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idNota"></param>
        /// <returns></returns>
        public static InfoNota GetNota(InfoUtente infoUtente, string idNota)
        {
            return GetInstanceDb(infoUtente).GetNota(idNota);
        }

        /// <summary>
        /// Reperimento ultima nota inserita per un documento / fascicolo in ordine cronologico
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        public static string GetUltimaNotaAsString(InfoUtente infoUtente, AssociazioneNota oggettoAssociato)
        {
            return GetInstanceDb(infoUtente).GetUltimaNotaAsString(oggettoAssociato);
        }

        /// <summary>
        /// Reperimento ultima nota inserita per un documento / fascicolo in ordine cronologico
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        public static InfoNota GetUltimaNota(InfoUtente infoUtente, AssociazioneNota oggettoAssociato)
        {   
            return GetInstanceDb(infoUtente).GetUltimaNota(oggettoAssociato);
        }

        /// <summary>
        /// Reperimento della lista delle note associate ad un documento / fascicolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocumento"></param>
        /// <param name="filtroRicerca"></param>
        /// <returns></returns>
        public static InfoNota[] GetNote(InfoUtente infoUtente, AssociazioneNota oggettoAssociato, FiltroRicercaNote filtroRicerca)
        {
            return GetInstanceDb(infoUtente).GetNote(oggettoAssociato, filtroRicerca);
        }

        /// <summary>
        /// Determina se l'oggetto cui sono associate le note è in sola lettura per l'utente corrente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="oggettoAssociato"></param>
        /// <returns></returns>
        public static bool IsNoteSolaLettura(InfoUtente infoUtente, AssociazioneNota oggettoAssociato)
        {
            return GetInstanceDb(infoUtente).IsNoteSolaLettura(oggettoAssociato);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static DocsPaDB.Query_DocsPAWS.Note GetInstanceDb(InfoUtente infoUtente)
        {
            return new DocsPaDB.Query_DocsPAWS.Note(infoUtente);
        }

        #endregion

        #region NOTE SELEZIONABILI DA UN ELENCO NOTE PRECONFIGURATO
        //Restituisce la lista delle NOTE presenti nell'elenco preconfigurato (dpa_elenco_note)
        public static NotaElenco[] GetListaNote(InfoUtente infoUtente, string idRF, string descNota, out int numNote)
        {
            return GetInstanceDb(infoUtente).GetListaNote(idRF, descNota, out numNote);
        }

        //Inserimento di una nuova nota da inserire in un elenco preconfigurato di note
        public static bool InsertNotaInElenco(InfoUtente infoUtente, NotaElenco nota, out string message)
        {
            bool returnResult = false;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                returnResult = GetInstanceDb(infoUtente).InsertNotaInElenco(nota, out message);
                transactionContext.Complete();
            }
            return returnResult;
        }

        //Inserimento di un elenco di note da import file excel
        public static bool InsertNotaInElencoDaExcel(DocsPaVO.utente.InfoUtente infoUtente, byte[] dati, string nomeFile, string serverPath, out string message)
        {
            bool returnResult = false;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                returnResult = GetInstanceDb(infoUtente).InsertNotaInElencoDaExcel(dati, nomeFile, serverPath, out message);
                transactionContext.Complete();
            }
            return returnResult;
        }


        public static bool ModNotaInElenco(InfoUtente infoUtente, NotaElenco nota, out string message)
        {
            bool returnResult = false;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                returnResult = GetInstanceDb(infoUtente).ModNotaInElenco(nota, out message);
                transactionContext.Complete();
            }
            return returnResult;
        }

        public static bool DeleteNoteInElenco(InfoUtente infoUtente, DocsPaVO.Note.NotaElenco[] listaNote)
        {
            bool returnResult = false;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                returnResult = GetInstanceDb(infoUtente).DeleteNoteInElenco(listaNote);
                transactionContext.Complete();
            }
            return returnResult;
        }

        public static string[] GetElencoNote(string idRegistroRf, string prefixText)
        {
            string[] lista;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Note note = new DocsPaDB.Query_DocsPAWS.Note();
                lista = note.GetElencoNote(idRegistroRf, prefixText);
                transactionContext.Complete();
            }
            return lista;
        }
        
       
        #endregion
    }
}
