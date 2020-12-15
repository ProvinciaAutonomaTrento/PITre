using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;

namespace NttDataWA.UIManager
{
    public class TaskManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        /// <summary>
        /// Estrazione della lista dei TASK ricevuti
        /// </summary>
        /// <returns></returns>
        public static List<Task> GetListaTaskRicevuti(bool incluteCompletedTasks)
        {
            try
            {
                return docsPaWS.GetListaTaskRicevuti(incluteCompletedTasks, UserManager.GetInfoUser()).ToList<Task>();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Task GetTaskByTrasmSingola(string idTrasmSingola)
        {
            try
            {
                return docsPaWS.GetTaskByTrasmSingola(idTrasmSingola, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Estrazione della lista dei TASK assegnati
        /// </summary>
        /// <returns></returns>
        public static List<Task> GetListaTaskAssegnati()
        {
            try
            {
                return docsPaWS.GetListaTaskAssegnati(UserManager.GetInfoUser()).ToList<Task>();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Rimuove definitamente il task
        /// </summary>
        /// <param name="idTask"></param>
        /// <returns></returns>
        public static bool RimuoviTask(string idTask)
        {
            try
            {
                return docsPaWS.RimuoviTask(idTask, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Riapre la lavorazione del task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static bool RiapriLavorazione(DocsPaWR.Task task)
        {
            try
            {
                return docsPaWS.RiapriLavorazione(task, UserManager.GetInfoUser(), RoleManager.GetRoleInSession());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Ciude il task impostando la data di chiusura
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static bool ChiudiTask(DocsPaWR.Task task)
        {
            try
            {
                return docsPaWS.ChiudiTask(task, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Rimuove il task non ancora chiuso
        /// </summary>
        /// <param name="idTask"></param>
        /// <returns></returns>
        public static bool AnnullaTask(DocsPaWR.Task task)
        {
            try
            {
                return docsPaWS.AnnullaTask(task, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Chiude la lavorazione del task
        /// </summary>
        /// <param name="idTask"></param>
        /// <returns></returns>
        public static bool ChiudiLavorazioneTask(DocsPaWR.Task task, string note)
        {
            try
            {
                return docsPaWS.ChiudiLavorazioneTask(task, note, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Associa il nuovo documento creato al task di origine
        /// </summary>
        /// <param name="idTask"></param>
        /// <param name="docnumber"></param>
        /// <returns></returns>
        public static bool AssociaContributoAlTask(DocsPaWR.Task task, SchedaDocumento schedaDoc)
        {
            try
            {
                return docsPaWS.AssociaContributoAlTask(task, schedaDoc, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public struct TagIdContributo
        {
            public const string LABEL_ATTIVITA_CONCLUSA = "<lblConclusaIl>";
            public const string LABEL_ATTIVITA_CONCLUSA_C = "</lblConclusaIl>";
            public const string LABEL_ID_CONTRIBUTO = "<lblIdContributo>";
            public const string LABEL_ID_CONTRIBUTO_C = "</lblIdContributo>";
            public const string LABEL_NOTE = "<lblNote>";
            public const string LABEL_TEXT_WRAP = "<lblTextwrap>";
        }
    }
}