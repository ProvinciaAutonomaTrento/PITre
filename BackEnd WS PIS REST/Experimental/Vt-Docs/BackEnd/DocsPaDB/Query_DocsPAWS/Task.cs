using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DocsPaVO.Task;
using System.Data;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    public class Task : DBProvider
    {

        #region Const

        private ILog logger = LogManager.GetLogger(typeof(Task));

        public enum TaskStatus
        {
            IN_PROGRESS,
            CLOSED
        }

        #endregion

        #region Select

        public DocsPaVO.Task.Task GetTaskByTrasmSingola(string idTrasmSingola, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo GetTaskByTrasmSingola in DocsPaDb.Query_DocsPAWS.Task");
            DocsPaVO.Task.Task task = new DocsPaVO.Task.Task();
            string query = string.Empty;
            DocsPaUtils.Query q;
            DataSet ds = new DataSet();
            try
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TASK_BY_ID_TRASM_SINGOLA");
                q.setParam("idTrasmSingola", idTrasmSingola);
                query = q.getSQL();
                logger.Debug("GetTaskByTrasmSingola: " + query);
                if (this.ExecuteQuery(out ds, "task", query))
                {
                    if (ds.Tables["task"] != null && ds.Tables["task"].Rows.Count > 0)
                    {
                        task = BuildTask(ds.Tables["task"].Rows[0]);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetTaskByTrasmSingola " + e.Message);
            }

            return task;
        }

        private DocsPaVO.Task.Task BuildTask(DataRow row)
        {
            DocsPaVO.Task.Task task = new DocsPaVO.Task.Task()
            {
                ID_TASK = row["ID_TASK"].ToString(),
                ID_PROFILE = !string.IsNullOrEmpty(row["ID_PROFILE"].ToString()) ? row["ID_PROFILE"].ToString() : string.Empty,
                ID_PROJECT = !string.IsNullOrEmpty(row["ID_PROJECT"].ToString()) ? row["ID_PROJECT"].ToString() : string.Empty,
                COD_PROJECT = !string.IsNullOrEmpty(row["COD_PROJECT"].ToString()) ? row["COD_PROJECT"].ToString() : string.Empty,
                DESCRIPTION_OBJECT = row["DESCRIPTION_OBJECT"].ToString(),
                ID_TRASMISSIONE = row["ID_TRASMISSIONE"].ToString(),
                ID_TRASM_SINGOLA = row["ID_TRASM_SINGOLA"].ToString(),
                ID_RAGIONE_TRASM = row["ID_RAGIONE_TRASM"].ToString(),
                ID_PROFILE_REVIEW = row["ID_PROFILE_REVIEW"].ToString(),
                ID_TIPO_ATTO = row["ID_TIPO_ATTO"].ToString(),
                CONTRIBUTO_OBBLIGATORIO = (!string.IsNullOrEmpty(row["CHA_CONTRIBUTO"].ToString()) && row["CHA_CONTRIBUTO"].ToString().Equals("1")) ? "1" : "0"
            };
            task.STATO_TASK = new StatoTask()
            {
                ID_STATO_TASK = row["ID_STATO_TASK"].ToString(),
                DATA_APERTURA = row["DTA_APERTURA"].ToString(),
                DATA_SCADENZA = !string.IsNullOrEmpty(row["DTA_SCADENZA"].ToString()) ? row["DTA_SCADENZA"].ToString() : string.Empty,
                DATA_ANNULLAMENTO = !string.IsNullOrEmpty(row["DTA_ANNULLAMENTO"].ToString()) ? row["DTA_ANNULLAMENTO"].ToString() : string.Empty,
                DATA_LAVORAZIONE = !string.IsNullOrEmpty(row["DTA_LAVORAZIONE"].ToString()) ? row["DTA_LAVORAZIONE"].ToString() : string.Empty,
                DATA_CHIUSURA = row["DTA_CHIUSURA"].ToString(),
                NOTE_RIAPERTURA = !string.IsNullOrEmpty(row["NOTE_RIAPERTURA"].ToString()) ? row["NOTE_RIAPERTURA"].ToString() : string.Empty,
                NOTE_LAVORAZIONE = !string.IsNullOrEmpty(row["NOTE_LAVORAZIONE"].ToString()) ? row["NOTE_LAVORAZIONE"].ToString() : string.Empty,
            };
            string stato = row["CHA_STATO"].ToString();
            switch (stato)
            {
                case "A":
                    task.STATO_TASK.STATO = StatoAvanzamento.Aperto;
                    break;
                case "L":
                    task.STATO_TASK.STATO = StatoAvanzamento.Lavorato;
                    break;
                case "C":
                    task.STATO_TASK.STATO = StatoAvanzamento.Chiuso;
                    break;
                case "R":
                    task.STATO_TASK.STATO = StatoAvanzamento.Riaperto;
                    break;
            }

            task.RUOLO_DESTINATARIO = new DocsPaVO.utente.Ruolo()
            {
                idGruppo = !string.IsNullOrEmpty(row["ID_GRUPPO_DEST"].ToString()) ? row["ID_GRUPPO_DEST"].ToString() : string.Empty,
                codiceRubrica = !string.IsNullOrEmpty(row["GROUP_ID_DEST"].ToString()) ? row["GROUP_ID_DEST"].ToString() : string.Empty,
                descrizione = !string.IsNullOrEmpty(row["GROUP_NAME_DEST"].ToString()) ? row["GROUP_NAME_DEST"].ToString() : string.Empty
            };
            task.UTENTE_DESTINATARIO = new DocsPaVO.utente.Utente()
            {
                idPeople = row["ID_UTENTE_DEST"].ToString(),
                descrizione = row["USER_DESCRIPTION_DEST"].ToString(),
                userId = row["USER_CODE_DEST"].ToString()
            };

            task.RUOLO_MITTENTE = new DocsPaVO.utente.Ruolo()
            {
                idGruppo = !string.IsNullOrEmpty(row["ID_GRUPPO_MITT"].ToString()) ? row["ID_GRUPPO_MITT"].ToString() : string.Empty,
                codiceRubrica = !string.IsNullOrEmpty(row["GROUP_ID_MITT"].ToString()) ? row["GROUP_ID_MITT"].ToString() : string.Empty,
                descrizione = !string.IsNullOrEmpty(row["GROUP_NAME_MITT"].ToString()) ? row["GROUP_NAME_MITT"].ToString() : string.Empty
            };
            task.UTENTE_MITTENTE = new DocsPaVO.utente.Utente()
            {
                idPeople = row["ID_UTENTE_MITT"].ToString(),
                descrizione = row["USER_DESCRIPTION_MITT"].ToString(),
                userId = row["USER_CODE_MITT"].ToString()
            };

            return task;
        }

        public List<DocsPaVO.Task.Task> GetListaTask(bool incluteCompletedTasks, DocsPaVO.Task.TipoTask tipo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo GetListaTask in DocsPaDb.Query_DocsPAWS.Task");
            List<DocsPaVO.Task.Task> listaTask = new List<DocsPaVO.Task.Task>();
            string query = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                DocsPaUtils.Query q;
                string condDest = string.Empty;
                string condMitt = string.Empty;
                if (tipo.Equals(DocsPaVO.Task.TipoTask.ASSEGNATI))
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TASK_ASSEGNATI");
                }
                else
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TASK_RICEVUTI");
                }
                q.setParam("idRuolo", infoUtente.idGruppo);
                q.setParam("idUtente", infoUtente.idPeople);
                q.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                if (!incluteCompletedTasks)
                    q.setParam("filterLavorazione", "and CHA_STATO IN ('A', 'R')");
                else
                    q.setParam("filterLavorazione", "and CHA_STATO IN ('A', 'L', 'R')");
                query = q.getSQL();
                logger.Debug("GetListaTask: " + query);

                if (this.ExecuteQuery(out ds, "listaTask", query))
                {
                    if (ds.Tables["listaTask"] != null && ds.Tables["listaTask"].Rows.Count > 0)
                    {
                        DocsPaVO.Task.Task task;
                        foreach (DataRow row in ds.Tables["listaTask"].Rows)
                        {
                            task = new DocsPaVO.Task.Task()
                            {
                                ID_TASK = row["ID_TASK"].ToString(),
                                ID_PROFILE = !string.IsNullOrEmpty(row["ID_PROFILE"].ToString()) ? row["ID_PROFILE"].ToString() : string.Empty,
                                ID_PROJECT = !string.IsNullOrEmpty(row["ID_PROJECT"].ToString()) ? row["ID_PROJECT"].ToString() : string.Empty,
                                COD_PROJECT = !string.IsNullOrEmpty(row["COD_PROJECT"].ToString()) ? row["COD_PROJECT"].ToString() : string.Empty,
                                DESCRIPTION_OBJECT = row["DESCRIPTION_OBJECT"].ToString(),
                                ID_TRASMISSIONE = row["ID_TRASMISSIONE"].ToString(),
                                ID_TRASM_SINGOLA = row["ID_TRASM_SINGOLA"].ToString(),
                                ID_RAGIONE_TRASM = row["ID_RAGIONE_TRASM"].ToString(),
                                DESC_RAGIONE_TRASM = row["VAR_DESC_RAGIONE"].ToString(),
                                ID_PROFILE_REVIEW = row["ID_PROFILE_REVIEW"].ToString(),
                                ID_TIPO_ATTO = row["ID_TIPO_ATTO"].ToString(),
                                CONTRIBUTO_OBBLIGATORIO = (!string.IsNullOrEmpty(row["CHA_CONTRIBUTO"].ToString()) && row["CHA_CONTRIBUTO"].ToString().Equals("1")) ? "1" : "0"
                            };
                            task.STATO_TASK = new StatoTask()
                            {
                                ID_STATO_TASK = row["ID_STATO_TASK"].ToString(),
                                DATA_APERTURA = row["DTA_APERTURA"].ToString(),
                                DATA_SCADENZA = !string.IsNullOrEmpty(row["DTA_SCADENZA"].ToString()) ? row["DTA_SCADENZA"].ToString() : string.Empty,
                                DATA_ANNULLAMENTO = !string.IsNullOrEmpty(row["DTA_ANNULLAMENTO"].ToString()) ? row["DTA_ANNULLAMENTO"].ToString() : string.Empty,
                                DATA_LAVORAZIONE = !string.IsNullOrEmpty(row["DTA_LAVORAZIONE"].ToString()) ? row["DTA_LAVORAZIONE"].ToString() : string.Empty,
                                DATA_CHIUSURA = row["DTA_CHIUSURA"].ToString(),
                                NOTE_RIAPERTURA = !string.IsNullOrEmpty(row["NOTE_RIAPERTURA"].ToString()) ? row["NOTE_RIAPERTURA"].ToString() : string.Empty,
                                NOTE_LAVORAZIONE = !string.IsNullOrEmpty(row["NOTE_LAVORAZIONE"].ToString()) ? row["NOTE_LAVORAZIONE"].ToString() : string.Empty,
                            };
                            string stato = row["CHA_STATO"].ToString();
                            switch (stato)
                            { 
                                case "A":
                                    task.STATO_TASK.STATO = StatoAvanzamento.Aperto;
                                    break;
                                case "L":
                                    task.STATO_TASK.STATO = StatoAvanzamento.Lavorato;
                                    break;
                                case "C":
                                    task.STATO_TASK.STATO = StatoAvanzamento.Chiuso;
                                    break;
                                case "R":
                                    task.STATO_TASK.STATO = StatoAvanzamento.Riaperto;
                                    break;
                            }

                            if (tipo.Equals(TipoTask.ASSEGNATI))
                            {
                                task.RUOLO_DESTINATARIO = new DocsPaVO.utente.Ruolo()
                                {
                                    idGruppo = !string.IsNullOrEmpty(row["ID_GRUPPO"].ToString()) ? row["ID_GRUPPO"].ToString() : string.Empty,
                                    codiceRubrica = !string.IsNullOrEmpty(row["GROUP_ID"].ToString()) ? row["GROUP_ID"].ToString() : string.Empty,
                                    descrizione = !string.IsNullOrEmpty(row["GROUP_NAME"].ToString()) ? row["GROUP_NAME"].ToString() : string.Empty
                                };
                                task.UTENTE_DESTINATARIO = new DocsPaVO.utente.Utente()
                                {
                                    idPeople = row["ID_UTENTE"].ToString(),
                                    descrizione = row["USER_DESCRIPTION"].ToString(),
                                    userId = row["USER_CODE"].ToString()
                                };
                            }
                            else
                            {
                                task.RUOLO_MITTENTE = new DocsPaVO.utente.Ruolo()
                                {
                                    idGruppo = !string.IsNullOrEmpty(row["ID_GRUPPO"].ToString()) ? row["ID_GRUPPO"].ToString() : string.Empty,
                                    codiceRubrica = !string.IsNullOrEmpty(row["GROUP_ID"].ToString()) ? row["GROUP_ID"].ToString() : string.Empty,
                                    descrizione = !string.IsNullOrEmpty(row["GROUP_NAME"].ToString()) ? row["GROUP_NAME"].ToString() : string.Empty
                                };
                                task.UTENTE_MITTENTE = new DocsPaVO.utente.Utente()
                                {
                                    idPeople = row["ID_UTENTE"].ToString(),
                                    descrizione = row["USER_DESCRIPTION"].ToString(),
                                    userId = row["USER_CODE"].ToString()
                                };
                            }
                            listaTask.Add(task);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.Task - Metodo GetListaTask", e);
                listaTask = null;
            }
            logger.Debug("Fine Metodo GetListaTask in DocsPaDb.Query_DocsPAWS.Task");
            return listaTask;
        }

        /// <summary>
        /// Verifica se per il tipo ragione è previsto attivazione di un task
        /// </summary>
        /// <param name="idRagione"></param>
        /// <returns></returns>
        public bool IsRagioneDiTipoTask(string idRagione)
        {

            logger.Debug("Inizio Metodo CheckToStartTask in DocsPaDb.Query_DocsPAWS.Task");
            bool result = false;

            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TIPO_RAGIONE");
                q.setParam("idRagione", idRagione);

                string query = q.getSQL();
                logger.Debug("CheckToStartTask: " + query);

                if (this.ExecuteQuery(out ds, "tipoTask", query))
                {
                    if (ds.Tables["tipoTask"] != null && ds.Tables["tipoTask"].Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(ds.Tables["tipoTask"].Rows[0]["CHA_TIPO_TASK"].ToString())
                            && ds.Tables["tipoTask"].Rows[0]["CHA_TIPO_TASK"].ToString().Equals("1"))
                            result = true;
                    }
                }
                
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo CheckToStartTask in DocsPaDb.Query_DocsPAWS.Task: " + e.Message);
                result = false;
            }

            return result;
        
        }

        /// <summary>
        /// Query per il metodo "getRagioneTrasm"
        /// </summary>
        /// <param name="ds"></param>
        public void getRagTrasm(out DataSet ds, string idAmm, String tipoRagione)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARagTrasm2");
            q.setParam("param1", idAmm);
            q.setParam("param2", tipoRagione);
            string queryString = q.getSQL();
            this.ExecuteQuery(out ds, "RAGIONE", queryString);
        }
        #endregion

        #region Insert

        /// <summary>
        /// Metodo per la creazione del TASK
        /// </summary>
        /// <param name="task"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public bool InsertTask(DocsPaVO.Task.Task task, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            logger.Debug("Inizio Metodo InsertTask in DocsPaDb.Query_DocsPAWS.Task");

            if (task != null)
            {
                try
                {
                    BeginTransaction();
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_TASK");
                    if (DBType.ToUpper().Equals("ORACLE"))
                        q.setParam("idTask", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_TASK"));

                    q.setParam("idRuoloMtt", task.RUOLO_MITTENTE.idGruppo);
                    q.setParam("idUtenteMitt", task.UTENTE_MITTENTE.idPeople);
                    q.setParam("idRuoloDest", task.RUOLO_DESTINATARIO.idGruppo);
                    q.setParam("idUtenteDest", task.UTENTE_DESTINATARIO.idPeople);

                    if (!string.IsNullOrEmpty(task.ID_PROJECT))
                    {
                        q.setParam("idProject", task.ID_PROJECT);
                        q.setParam("idProfile", "null");
                    }
                    else
                    {
                        q.setParam("idProfile", task.ID_PROFILE);
                        q.setParam("idProject", "null");
                    }
                    q.setParam("idTrasmissione", task.ID_TRASMISSIONE);
                    q.setParam("idTrasmSingola", task.ID_TRASM_SINGOLA);
                    q.setParam("idRagioneTrasm", task.ID_RAGIONE_TRASM);

                    string query = q.getSQL();
                    logger.Debug("InsertTask: " + query);
                    if (ExecuteNonQuery(query))
                    {
                        string idTask = string.Empty;
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_TASK");
                        this.ExecuteScalar(out idTask, sql);
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_STATO_TASK");
                        if (DBType.ToUpper().Equals("ORACLE"))
                            q.setParam("idStatoTask", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_STATO_TASK"));
                        q.setParam("idTask", idTask);
                        q.setParam("dataApertura", DocsPaDbManagement.Functions.Functions.GetDate());
                        q.setParam("dataScadenza", DocsPaDbManagement.Functions.Functions.ToDate(task.STATO_TASK.DATA_SCADENZA));
                        q.setParam("noteRiapertura", string.IsNullOrEmpty(task.STATO_TASK.NOTE_RIAPERTURA) ? string.Empty : task.STATO_TASK.NOTE_RIAPERTURA.Replace("'", "''"));
                        q.setParam("stato", "A");

                        query = q.getSQL();
                        logger.Debug("InsertStatoTask: " + query);
                        if (ExecuteNonQuery(query))
                        {
                            result = true;
                        }
                    }

                    if (result)
                    {
                        CommitTransaction();
                    }
                    else
                    {
                        throw new Exception("Errore durante la creazione del task: " + query);
                    }
                }
                catch (Exception e)
                {
                    RollbackTransaction();
                    logger.Error("Errore in DocsPaDb.Query_DocsPAWS.Task - Metodo InsertTask", e);
                    result = false;
                }
            }
            logger.Debug("Fine Metodo InserTask in DocsPaDb.Query_DocsPAWS.Task");
            return result;
        }

        /// <summary>
        /// Riapre la lavorazione del task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool RiapriLavorazioneTask(DocsPaVO.Task.Task task)
        { 
            //La riapertura del task consiste nell'inserire un nuovo record nella dpa_stato_task
            bool result = false;
            logger.Debug("Inizio Metodo RiapriLavorazioneTask in DocsPaDb.Query_DocsPAWS.Task");
            try
            {
                if (task != null)
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_STATO_TASK");
                    if (DBType.ToUpper().Equals("ORACLE"))
                        q.setParam("idStatoTask", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_STATO_TASK"));
                    q.setParam("idTask", task.ID_TASK);
                    q.setParam("dataApertura", DocsPaDbManagement.Functions.Functions.GetDate());
                    q.setParam("dataScadenza", DocsPaDbManagement.Functions.Functions.ToDate(task.STATO_TASK.DATA_SCADENZA));
                    q.setParam("stato", "R");
                    q.setParam("noteRiapertura", string.IsNullOrEmpty(task.STATO_TASK.NOTE_RIAPERTURA) ? string.Empty : task.STATO_TASK.NOTE_RIAPERTURA.Replace("'", "''"));

                    string query = q.getSQL();
                    logger.Debug("InsertStatoTask: " + query);
                    if (ExecuteNonQuery(query))
                    {
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TASK_ID_ID_PROFILE_REVIEW");
                        q.setParam("idTask", task.ID_TASK);
                        query = q.getSQL();
                        logger.Debug("UpdateCotributo: " + query);
                        if (ExecuteNonQuery(query))
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.Task - Metodo RiapriLavorazioneTask", e);
            }
            return result;
        }

        #endregion

        #region Update

        /// <summary>
        /// Metodo per la chiusura della lavorazione del task
        /// </summary>
        /// <param name="idTask"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public bool ChiudiLavorazioneTask(string idStatoTask, string noteLavorazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo ChiudiLavorazioneTask in DocsPaDb.Query_DocsPAWS.Task");
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_STATO_TASK_CHIUDI_LAVORAZIONE");
                q.setParam("idStatoTask", idStatoTask);
                q.setParam("dataLavorazione", DocsPaDbManagement.Functions.Functions.GetDate());
                q.setParam("noteLavorazione", string.IsNullOrEmpty(noteLavorazione) ? string.Empty : noteLavorazione.Replace("'", "''"));

                string query = q.getSQL();
                logger.Debug("ChiudiLavorazioneTask: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante la chiusura della lavorazione del task: " + query);
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo ChiudiLavorazioneTask in DocsPaDb.Query_DocsPAWS.Task : " + e.Message);
                result = false;
            }
            logger.Debug("FINE Metodo ChiudiLavorazioneTask in DocsPaDb.Query_DocsPAWS.Task");
            return result;
        }

        /// <summary>
        /// Chiude il Task
        /// </summary>
        /// <param name="idStatoTask"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public bool ChiudiTask(string idStatoTask, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo ChiudiTask in DocsPaDb.Query_DocsPAWS.Task");
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_STATO_TASK_DTA_CHIUSURA");
                q.setParam("idStatoTask", idStatoTask);
                q.setParam("dataChiusura", DocsPaDbManagement.Functions.Functions.GetDate());

                string query = q.getSQL();
                logger.Debug("ChiudiTask: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante la chiusura del task: " + query);
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo ChiudiTask in DocsPaDb.Query_DocsPAWS.Task : " + e.Message);
                result = false;
            }
            logger.Debug("FINE Metodo ChiudiTask in DocsPaDb.Query_DocsPAWS.Task");
            return result;
        }


        public bool AnnullaTask(string idStatoTask, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo AnnullaTask in DocsPaDb.Query_DocsPAWS.Task");
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_STATO_TASK_DTA_ANNULLAMENTO");
                q.setParam("idStatoTask", idStatoTask);
                q.setParam("dataAnnullamento", DocsPaDbManagement.Functions.Functions.GetDate());

                string query = q.getSQL();
                logger.Debug("AnnullaTask: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'annullamento del task: " + query);
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo AnnullaTask in DocsPaDb.Query_DocsPAWS.Task : " + e.Message);
                result = false;
            }
            logger.Debug("FINE Metodo AnnullaTask in DocsPaDb.Query_DocsPAWS.Task");
            return result;
        }

        /// <summary>
        /// Associa il nuovo documento creato al task di origine
        /// </summary>
        /// <param name="idTask"></param>
        /// <param name="docnumber"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public bool AssociaContributoAlTask(string idTask, string docnumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo AssociaContributoAlTask in DocsPaDb.Query_DocsPAWS.Task");
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TASK_ID_PROFILE_REVIEW");
                q.setParam("idTask", idTask);
                q.setParam("docnumber", docnumber);

                string query = q.getSQL();
                logger.Debug("AssociaContributoAlTask: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'associazione del documento al task: " + query);
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo AssociaContributoAlTask in DocsPaDb.Query_DocsPAWS.Task : " + e.Message);
                result = false;
            }
            logger.Debug("FINE Metodo AssociaContributoAlTask in DocsPaDb.Query_DocsPAWS.Task");
            return result;
        }

        /// <summary>
        /// Aggiornamento delle note di trasmissione utente  in seguito al completamento del task
        /// </summary>
        /// <param name="idTrasmSingola"></param>
        /// <param name="note"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public bool UpdateNoteTrasmUtente(string idTrasmSingola, string note, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo UpdateNoteTrasmUtente in DocsPaDb.Query_DocsPAWS.Task");
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_NOTE");
                q.setParam("idTrasmSingola", idTrasmSingola);
                q.setParam("idPeople", infoUtente.idPeople);
                q.setParam("note", note.Replace("'", "''"));

                string query = q.getSQL();
                logger.Debug("UpdateNoteTrasmUtente: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento delle note di trasmissione utente: " + query);
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo UpdateNoteTrasmUtente in DocsPaDb.Query_DocsPAWS.Task : " + e.Message);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Aggiornamento dello stato del task associato al contributo nella profile
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateTaskStatusInProfile(string idProfile, TaskStatus status)
        {
            logger.Debug("Inizio Metodo UpdateTaskStatusInProfile in DocsPaDb.Query_DocsPAWS.Task");

            bool result = false;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROFILE_TASK_STATUS");
                q.setParam("idProfile", idProfile);
                q.setParam("status", status.ToString());

                string query = q.getSQL();
                logger.Debug("UpdateTaskStatusInProfile: " + query);

                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento dello stato del task associato al documento: " + query);
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo UpdateTaskStatusInProfile in DocsPaDb.Query_DocsPAWS.Task : " + ex.Message);
                result = false;
            }

            logger.Debug("FINE Metodo UpdateTaskStatusInProfile in DocsPaDb.Query_DocsPAWS.Task");
            return result;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Metodo per la rimozione del task
        /// </summary>
        /// <param name="idTask"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public bool RimuoviTask(string idTask, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo RimuoviTask in DocsPaDb.Query_DocsPAWS.Task");
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_TASK");
                q.setParam("idTask", idTask);

                string query = q.getSQL();
                logger.Debug("RimuoviTask: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante la rimozione del task: " + query);
                }
                else
                {
                    result = true;
                }
            }
            catch(Exception e)
            {
                logger.Error("Errore nel Metodo RimuoviTask in DocsPaDb.Query_DocsPAWS.Task : " + e.Message);
                result = false;
            }
            logger.Debug("FINE Metodo RimuoviTask in DocsPaDb.Query_DocsPAWS.Task");
            return result;
        }

        #endregion
    }
}
