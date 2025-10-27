// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Serilog;

namespace BusinessLogic.Task
{
    public class TaskManager
    {
        private static ILogger logger = Log.ForContext(typeof(TaskManager));

        /// <summary>
        /// Metodo per la creazione del task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool CreateTask(DocsPaVO.Task.Task task, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Creazione del TASK in BusinessLogic.Task.TaskManager  - metodo: CreateTask ");
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Task t = new DocsPaDB.Query_DocsPAWS.Task();
                result = t.InsertTask(task, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.Task.TaskManager  - metodo: CreateTask ", e);
                result = false;
            }

            return result;
        }

    }
}
