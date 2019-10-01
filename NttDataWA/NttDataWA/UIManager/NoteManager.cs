using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;

namespace NttDataWA.UIManager
{
    public class NoteManager
    {
        private static DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();

        /// <summary>
        /// Reperimento ultima nota inserita per un documento / fascicolo in ordine cronologico
        /// </summary>
        /// <returns></returns>
        public InfoNota GetUltimaNota()
        {
            try
            {
                InfoNota retValue = null;
                //foreach (InfoNota nota in this.Note)
                //{
                //    if (!nota.DaRimuovere)
                //    {
                //        retValue = nota;
                //        break;
                //    }
                //}
                return retValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static NotaElenco[] GetListaNote(string rf, string qry, out int numNote) {
            return ws.GetListaNote(UserManager.GetInfoUser(), rf, qry, out numNote);
        }

        public static bool DeleteNoteInElenco(NotaElenco[] lst)
        {
            return ws.DeleteNoteInElenco(UserManager.GetInfoUser(), lst);
        }

        public static bool InsertNotaInElenco(DocsPaWR.NotaElenco nota, out string message)
        {
            return ws.InsertNotaInElenco(UserManager.GetInfoUser(), nota, out message);
        }

        public static bool ModNotaInElenco(DocsPaWR.NotaElenco nota, out string message)
        {
            return ws.ModNotaInElenco(UserManager.GetInfoUser(), nota, out message);
        }

        public static ImportResult[] InsertNotaInElencoDaExcel(byte[] dati, string filename)
        {
            return ws.InsertNotaInElencoDaExcel(UserManager.GetInfoUser(), dati, filename);
        }
    }
}