using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaDB.Query_DocsPAWS
{
    public class Reminder : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(Reminder));

        #region Insert

        /// <summary>
        /// Metodo per la creazione di un reminder
        /// </summary>
        /// <param name="task"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public bool InsertReminder(DocsPaVO.Task.Task task,string subject, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            string query = string.Empty;
            string function = "DocsPaDb.Query_DocsPAWS.Reminder.InsertReminder";

            logger.DebugFormat("{0} - START ", function);
            if (task == null)
            {
                logger.DebugFormat("{0} - Parameter task is null", function);
                logger.DebugFormat("{0} - END ");
                return false;
            }

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_REMINDER");
                if (DBType.ToUpper().Equals("ORACLE"))
                    q.setParam("IdReminder", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_REMINDER"));

                q.setParam("IdTask", task.ID_TASK);
                q.setParam("IdUtenteMitt", task.UTENTE_MITTENTE.idPeople);
                q.setParam("IdUtenteDest", task.UTENTE_DESTINATARIO.idPeople);
                q.setParam("DtaScadenza", DocsPaDbManagement.Functions.Functions.ToDate(task.STATO_TASK.DATA_SCADENZA));
                q.setParam("NumInvii", "0");
                q.setParam("Descr", GetStringParameterValue(subject));

                query = q.getSQL();
                logger.DebugFormat("{0} - {1} ", function, query);
                if (!ExecuteNonQuery(query))
                    throw new Exception("Errore durante la creazione del reminder: " + query);

                result = true;
            }
            catch (Exception e)
            {
                logger.Error(function, e);
            }

            logger.DebugFormat("{0} - END",function);
            return result;
        }

        private string GetStringParameterValue(string value)
        {
            if (value == string.Empty)
                return "Null";
            else
                return "'" + DocsPaUtils.Functions.Functions.ReplaceApexes(value) + "'";
        }

        #endregion
    }
}
