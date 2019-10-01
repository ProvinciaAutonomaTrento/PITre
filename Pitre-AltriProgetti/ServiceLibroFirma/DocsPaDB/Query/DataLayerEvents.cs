using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using DocsPaVO.LibroFirma;
//using System.IO;
using DocsPaUtils;

namespace DocsPaDB.Query 
{
    public class DataLayerEvents : DBProvider
    {
        
        /// <summary>
        /// Returns the list of events to be processed
        /// </summary>
        /// <returns></returns>
        public List<Evento> SelectLibroFirmaEventsToBeProcessed(string[] listOfEventType)
        {
            List<Evento> listEvent = new List<Evento>();
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_EVENTS_IN_MONITOR");

                string conditionToAdd = "";

                if (listOfEventType != null)
                {
                    conditionToAdd = "And Dpa_Anagrafica_Eventi.Cha_tipo_evento IN (";

                    int valCounter = 0;
                    foreach (string tipoEvento in listOfEventType)
                    {
                        if (valCounter > 0)
                            conditionToAdd = conditionToAdd + ",";

                        conditionToAdd = conditionToAdd + "'" + tipoEvento + "'";
                        valCounter++;
                    }

                    conditionToAdd = conditionToAdd + ")";
                }

                q.setParam("condition", conditionToAdd);
               
                string queryString = q.getSQL();

                logWriter.addLog(logWriter.DEBUG, queryString);

                OpenConnection();

                if (!this.ExecuteQuery(out ds, "EventsFound", queryString))
                {
                    return listEvent;
                }
                else
                {
                    this.CreateEventLibroFirmaObjects(ds, ref listEvent);
                }
            }
            catch (Exception exc)
            {
                logWriter.addLog(logWriter.ERRORE, exc.Message);
            }
            finally
            {
                if (listEvent.Count < 1)
                    logWriter.addLog(logWriter.INFO, "NESSUN EVENTO TROVATO");
                CloseConnection();
            }

            logWriter.addLog(logWriter.INFO, "Elementi da elaborare: " + (listEvent != null ? listEvent.Count.ToString() : "0"));

            return listEvent;
        }

        /// <summary>
        /// Builds the list of events from the dataset to be processed.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="listEvent"></param>
        private void CreateEventLibroFirmaObjects(DataSet ds, ref List<Evento> listEvent)
        {
            try
            {
                if (ds.Tables["EventsFound"] != null && ds.Tables["EventsFound"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["EventsFound"].Rows)
                    {
                        DataRow dr = row;
                        Evento e = new Evento()
                        {
                            idEvento = dr["Id_Log"].ToString(),
                            idDocumento = dr["Id_Documento"].ToString(),
                            idProcesso = dr["Id_Istanza"].ToString(),
                            idRuolo = dr["Id_Group"].ToString(),
                            idUtente = dr["Id_People"].ToString(),
                            codiceAzione = dr["Var_Cod_Azione"].ToString(),
                            tipoEvento = dr["Cha_tipo_evento"].ToString(),
                            DataInserimento = dr["Data_Inserimento"].ToString(),
                            Delegato = dr["Delegato"].ToString()
                        };

                        listEvent.Add(e);
                    }
                }
            }
            catch (Exception exc)
            {
                logWriter.addLog(logWriter.ERRORE, exc.Message);
               
                listEvent = new List<Evento>();
                throw exc;
            }
        }

        /// <summary>
        /// Returns the items passo di firma
        /// </summary>
        /// <returns></returns>
        public PassoFirma GetPassoDiFirma(Evento e, string eventWithoutActor)
        {
            PassoFirma passo = new PassoFirma();

            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PASSO");
                q.setParam("Var_Cod_Azione", e.codiceAzione.ToString());
                q.setParam("Id_Group", e.idRuolo.ToString());
                q.setParam("Id_Processo", e.idProcesso.ToString());
                q.setParam("Id_Documento", e.idDocumento.ToString());

                q.setParam("Id_People", e.idUtente);
                
                string queryString = q.getSQL();

                logWriter.addLog(logWriter.DEBUG, queryString);

                this.ExecuteQuery(out ds, "Items", q.getSQL());

                if (ds.Tables["Items"] != null && ds.Tables["Items"].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables["Items"].Rows[0];
                    passo.idPasso = dr["ID_ISTANZA_PASSO"].ToString();
                    passo.numeroSequenza = Utils.ConvertField(dr["numero_Sequenza"].ToString());
                    passo.tipoFirma = dr["tipo_Firma"].ToString();
                    passo.idDocumeto = e.idDocumento;
                    passo.VersionId = dr["version_id"].ToString();
                    passo.idProcesso = e.idProcesso;
                    passo.idRuoloCoinvolto = e.idRuolo;
                    passo.idUtenteCoinvolto = e.idUtente;
                    passo.tipoEvento = e.tipoEvento;
                    passo.docAll = dr["Doc_All"].ToString();
                    passo.isAutomatico = string.IsNullOrEmpty(dr["CHA_AUTOMATICO"].ToString()) || !dr["CHA_AUTOMATICO"].ToString().Equals("1") ? false : true;
                }
                else if (!string.IsNullOrEmpty(eventWithoutActor))
                {
                    string[] eventWithoutActorList = eventWithoutActor.Split('|');

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PASSO_NO_ACTOR");

                    int valCounter = 0;
                    string conditionToAdd = string.Empty;
                    foreach (string tipoEvento in eventWithoutActorList)
                    {
                        if (valCounter > 0)
                            conditionToAdd = conditionToAdd + ",";

                        conditionToAdd = conditionToAdd + "'" + tipoEvento + "'";
                        valCounter++;
                    }
                    q.setParam("Var_Cod_Azione", e.codiceAzione.ToString());
                    q.setParam("Cha_Tipo_Azione", conditionToAdd);

                    q.setParam("Id_Processo", e.idProcesso.ToString());
                    q.setParam("Id_Documento", e.idDocumento.ToString());
                    
                    queryString = q.getSQL();

                    logWriter.addLog(logWriter.DEBUG, queryString);

                    this.ExecuteQuery(out ds, "Items", q.getSQL());

                    if (ds.Tables["Items"] != null && ds.Tables["Items"].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables["Items"].Rows[0];
                        passo.idPasso = dr["ID_ISTANZA_PASSO"].ToString();
                        passo.numeroSequenza = Utils.ConvertField(dr["numero_Sequenza"].ToString());
                        passo.tipoFirma = dr["tipo_Firma"].ToString();
                        passo.idDocumeto = e.idDocumento;
                        passo.VersionId = dr["version_id"].ToString();
                        passo.idProcesso = e.idProcesso;
                        passo.idRuoloCoinvolto = e.idRuolo;
                        passo.idUtenteCoinvolto = e.idUtente;
                        passo.tipoEvento = e.tipoEvento;
                        passo.docAll = dr["Doc_All"].ToString();
                    }
                }
            }
            catch (Exception exc)
            {
                logWriter.addLog(logWriter.ERRORE, exc.Message);
            }

            logWriter.addLog(logWriter.INFO, (passo != null && !string.IsNullOrEmpty(passo.idPasso) ? "PASSO: " + passo.idPasso.ToString() : "NESSUN PASSO TROVATO."));

            return passo;
        }

        /// <summary>
        /// Delete the items in event monitor
        /// </summary>
        /// <returns></returns>
        public bool DeleteEvent(string idLog)
        {
            bool retVal = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_EVENT_IN_MONITOR");
            q.setParam("Id_Log", idLog);

            string queryString = q.getSQL();

            logWriter.addLog(logWriter.DEBUG, queryString);
            
            try
            {
                this.ExecuteNonQuery(queryString);
            }
            catch (Exception ex)
            {
                logWriter.addLog(logWriter.ERRORE, ex.Message);
                retVal = false;
            }

            logWriter.addLog(logWriter.INFO, (retVal? "Elemento conidLog = " + idLog + " eliminato correttamente da DPA_EVENT_MONITOR." : "Impossibile eliminare elemento con logId = " + idLog ));
            return retVal;
        }

        /// <summary>
        /// Delete the items in "elemento in libro firma" if closed
        /// </summary>
        /// <returns></returns>
        public bool DeleteMAnualProcess()
        {
            bool retVal = false;
            string parameter = string.Empty;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_MANUAL_PROCESS");
           
            string queryString = q.getSQL();

            logWriter.addLog(logWriter.DEBUG, queryString);

            try
            {
                retVal = this.ExecuteNonQuery(queryString);
            }
            catch (Exception ex)
            {
                logWriter.addLog(logWriter.ERRORE, ex.Message);
                retVal = false;
            }

            //logWriter.addLog(logWriter.INFO, (retVal ? "Processi manuali elaborati, eliminati correttamente." : "Impossibile eliminare i processi manuali elaborati."));

            return retVal;
        }

        /// <summary>
        /// Delete the items in event monitor where not in "elemento in libro firma"
        /// </summary>
        /// <returns></returns>
        public bool DeleteAllIgnoredEvent()
        {
            bool retVal = false;
            string parameter = string.Empty;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_ALL_IGNORED_EVENT");

            string queryString = q.getSQL();

            logWriter.addLog(logWriter.DEBUG, queryString);

            try
            {
                retVal = this.ExecuteNonQuery(queryString);
            }
            catch (Exception ex)
            {
                logWriter.addLog(logWriter.ERRORE, ex.Message);
                retVal = false;
            }

            logWriter.addLog(logWriter.INFO, (retVal ? "Elementi tutti gli elementi non significativi in DPA_EVENT_MONITOR." : "Impossibile eliminare gli elementi non significativi in DPA_EVENT_MONITOR."));
            return retVal;
        }

        /// <summary>
        /// Delete all the elaborate items in event monitor
        /// </summary>
        /// <returns></returns>
        public bool DeleteEvent(List<string> listIdLog)
        {
            bool retVal = false;
            string parameter = string.Empty;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_EVENTS_IN_MONITOR");

            int i = 1;
            foreach (string idLog in listIdLog)
            {
                if (i == listIdLog.Count)
                    parameter += idLog;
                else
                    parameter += idLog + ",";

                i++;
            }

            q.setParam("loglist", parameter);

            string queryString = q.getSQL();

            logWriter.addLog(logWriter.DEBUG, queryString);

            try
            {
                retVal = this.ExecuteNonQuery(queryString);
            }
            catch (Exception ex)
            {
                logWriter.addLog(logWriter.ERRORE, ex.Message);
                retVal = false;
            }

            logWriter.addLog(logWriter.INFO, (retVal ? listIdLog.Count.ToString() + " elementi elaborati, eliminati da DPA_EVENT_MONITOR." : "Impossibile eliminare elementi elaborati."));
            return retVal;
        }

        /// <summary>
        /// Set the items in state CLOSED
        /// </summary>
        /// <returns></returns>
        public bool SetPassoClosed(PassoFirma passo)
        {
            bool retVal = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PASSO");
            q.setParam("Id_Passo", passo.idPasso);

            string queryString = q.getSQL();

            logWriter.addLog(logWriter.DEBUG, queryString);

            try
            {
                this.ExecuteNonQuery(queryString);
            }
            catch (Exception ex)
            {
                logWriter.addLog(logWriter.ERRORE, ex.Message);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Set the items in state CLOSED
        /// </summary>
        /// <returns></returns>
        public bool SetLookNextPasso(PassoFirma passo)
        {
            bool retVal = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_NEXT_PASSO_AS_LOOK");
            q.setParam("Id_Processo", passo.idProcesso);
            q.setParam("Id_Documento", passo.idDocumeto);
            q.setParam("Id_Processo", passo.idProcesso);
            q.setParam("N_Sequenza", (passo.numeroSequenza + 1).ToString());

            string queryString = q.getSQL();

            logWriter.addLog(logWriter.DEBUG, queryString);

            try
            {
                this.ExecuteNonQuery(queryString);
            }
            catch (Exception ex)
            {
                logWriter.addLog(logWriter.ERRORE, ex.Message);
                retVal = false;
            }

            return retVal;
        }

        public DocsPaVO.utente.Utente GetLoggedUser(string idPeople, string idGroup)
        {
            DocsPaVO.utente.Utente retVal = new DocsPaVO.utente.Utente();

            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_UTENTE");
                q.setParam("idpeople", idPeople);
                q.setParam("idgroup", idGroup);

                string queryString = q.getSQL();

                logWriter.addLog(logWriter.DEBUG, queryString);

                this.ExecuteQuery(out ds, "Items", queryString);

                if (ds.Tables["Items"] != null && ds.Tables["Items"].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables["Items"].Rows[0];
                    retVal.IdPeople = idPeople;
                    retVal.Nome = dr["Var_Nome"].ToString(); ;
                    retVal.Cognome = dr["Var_Cognome"].ToString();
                    retVal.Userid = dr["User_Id"].ToString();
                    retVal.CodAmm = dr["Var_Codice_Amm"].ToString();
                    retVal.Idruolo = idGroup;
                    retVal.CodRuolo = dr["Group_Id"].ToString();
                }
            }
            catch (Exception exc)
            {
                logWriter.addLog(logWriter.ERRORE, exc.Message);
            }

            logWriter.addLog(logWriter.INFO, (!string.IsNullOrEmpty(retVal.IdPeople) ? "Caricato utente " + retVal.Userid : "Nessun utente trovato con idpeople " + idPeople + " e idGroup " + idGroup));

            return retVal;
        }

        public DocsPaVO.utente.Utente GetLoggedAutomaticUser(string idPeople, string idGroup)
        {
            DocsPaVO.utente.Utente retVal = new DocsPaVO.utente.Utente();

            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_UTENTE_AUTOMATICO");
                q.setParam("idpeople", idPeople);

                string queryString = q.getSQL();

                logWriter.addLog(logWriter.DEBUG, queryString);

                this.ExecuteQuery(out ds, "Items", queryString);

                if (ds.Tables["Items"] != null && ds.Tables["Items"].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables["Items"].Rows[0];
                    retVal.IdPeople = idPeople;
                    retVal.Nome = dr["Var_Nome"].ToString(); ;
                    retVal.Cognome = dr["Var_Cognome"].ToString();
                    retVal.Userid = dr["User_Id"].ToString();
                    retVal.CodAmm = dr["Var_Codice_Amm"].ToString();
                }
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RUOLO");
                q.setParam("idgroup", idGroup);

                queryString = q.getSQL();

                logWriter.addLog(logWriter.DEBUG, queryString);

                this.ExecuteQuery(out ds, "Items", queryString);

                if (ds.Tables["Items"] != null && ds.Tables["Items"].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables["Items"].Rows[0];
                    retVal.Idruolo = idGroup;
                    retVal.CodRuolo = dr["Group_Id"].ToString();
                }
            }
            catch (Exception exc)
            {
                logWriter.addLog(logWriter.ERRORE, exc.Message);
            }

            logWriter.addLog(logWriter.INFO, (!string.IsNullOrEmpty(retVal.IdPeople) ? "Caricato utente " + retVal.Userid : "Nessun utente trovato con idpeople " + idPeople + " e idGroup " + idGroup));

            return retVal;
        }
    }
}
