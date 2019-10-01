using System;
using DocsPaVO.Logger;
using DocsPaUtils.Functions;
using log4net;
namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Descrizione di riepilogo per Log.
    /// </summary>
    public class Log : DBProvider
    {
        private static ILog logger = LogManager.GetLogger(typeof(Log));
        #region Property

        private int idPeopleDelegato;


        private string Blank = " ";

        private string dataAzione;



        private enum Condition
        {
            AND,
            OR
        }


        #endregion



        #region Metodi

        /// <summary>
        /// Aggiunge record sulla tabella dei log attivati
        /// </summary>
        /// <param name="codiceLog">codice del log</param>
        /// <param name="idAmm">id amm.ne</param>
        /// <returns>bool</returns>
        public bool AddLogAttivi(string codiceLog, string idAmm, string notify = "")
        {
            bool result = true;
            string Sql = null;
            DocsPaUtils.Query q;
            string IDcodiceLog = null;
            System.Data.DataSet rec;

            try
            {
                // prende la system_id del record nella dpa_anagrafica_log
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ID_LOG");
                q.setParam("param1", codiceLog);
                q.setParam("param2", idAmm);
                Sql = q.getSQL();
                logger.Debug("AddLogAttivi - sql #1 : " + Sql);
                if (!this.ExecuteQuery(out rec, Sql))
                {
                    logger.Debug("Errore nella select della system_id nella DPA_ANAGRAFICA_LOG");
                    return false;
                }
                else
                {
                    if (rec != null)
                    {
                        if (rec.Tables[0].Rows.Count > 0)
                        {
                            IDcodiceLog = rec.Tables[0].Rows[0]["system_id"].ToString();
                        }
                    }
                }

                // quindi esegue la insert sulla dpa_log_attivati
                q = DocsPaUtils.InitQuery.getInstance().getQuery("I_LOG_ATTIVATI");
                q.setParam("param1", IDcodiceLog);
                q.setParam("param2", idAmm);
                if (!string.IsNullOrEmpty(notify))
                {
                    q.setParam("param3", "'" + notify + "'");
                }
                else
                {
                    q.setParam("param3", "'NN'");
                }
                Sql = q.getSQL();
                logger.Debug("AddLogAttivi - sql #2 : " + Sql);
                if (!this.ExecuteNonQuery(Sql))
                {
                    logger.Debug("Errore nell'inserimento dei record nella DPA_LOG_ATTIVATI");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                return false;
            }

            return result;
        }

        /// <summary>
        /// elimina i record dei log attivati
        /// </summary>
        /// <param name="idAmm">id amm.ne</param>
        /// <returns>bool</returns>
        public bool DelLogAttivi(string idAmm)
        {
            bool result = true;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_LOG_ATTIVATI");
                q.setParam("param1", idAmm);
                string Sql = q.getSQL();
                logger.Debug("DelLogAttivi - sql: " + Sql);
                if (!this.ExecuteNonQuery(Sql))
                {
                    logger.Debug("Errore nella eliminazione dei record nella DPA_LOG_ATTIVATI");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                return false;
            }

            return result;
        }

        /// <summary>
        /// elimina i record dei log attivati di amministrazione
        /// </summary>
        /// <param name="idAmm">id amm.ne</param>
        /// <returns>bool</returns>
        public bool DelLogAttiviAmm(string idAmm)
        {
            bool result = true;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_LOG_ATTIVATI_AMM");
                q.setParam("param1", idAmm);
                string Sql = q.getSQL();
                logger.Debug("DelLogAttiviAmm - sql: " + Sql);
                if (!this.ExecuteNonQuery(Sql))
                {
                    logger.Debug("Errore nella eliminazione dei record di amministrazione nella DPA_LOG_ATTIVATI");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                return false;
            }

            return result;
        }

        /// <summary>
        /// prende i log dall'anagrafica in join con i log attivati
        /// </summary>
        /// <param name="idAmm">id amm.ne</param>
        /// <returns>dataset</returns>
        public System.Data.DataSet GetLogDisattivi(string idAmm)
        {
            System.Data.DataSet ds2;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("J_LOG_ATTIVI_DISATTIVI");
                q.setParam("param1", idAmm);
                string Sql = q.getSQL();
                logger.Debug("GetLogDisattivi con idAmm = " + idAmm + " - sql: " + Sql);

                if (!this.ExecuteQuery(out ds2, "AZIONI_DISATT", Sql))
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                ds2 = null;
            }

            return ds2;
        }

        /// <summary>
        /// prende i log dall'anagrafica in join con i log attivati
        /// </summary>
        /// <param name="idAmm">id amm.ne</param>
        /// <returns>dataset</returns>
        public System.Data.DataSet GetLogDisattiviAmm(string idAmm)
        {
            System.Data.DataSet ds;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("J_LOG_ATTIVI_DISATTIVI_AMM");
                q.setParam("param1", idAmm);
                string Sql = q.getSQL();
                logger.Debug("GetLogDisattiviAmm con idAmm = " + idAmm + " - sql: " + Sql);

                if (!this.ExecuteQuery(out ds, "AZIONI_DISATTIVE", Sql))
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                ds = null;
            }

            return ds;
        }

        /// <summary>
        /// prende i log dall'anagrafica
        /// </summary>
        /// <returns>dataset</returns>
        public System.Data.DataSet GetLogDisattivi()
        {
            System.Data.DataSet ds;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_LOG_DISATTIVI");
                string Sql = q.getSQL();
                logger.Debug("GetLogDisattivi - sql: " + Sql);

                if (!this.ExecuteQuery(out ds, "AZIONI", Sql))
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                ds = null;
            }

            return ds;
        }

        /// <summary>
        /// prende i tipi evento non attivi per l'amministrazione8inserito con il nuovo centro notifiche)
        /// </summary>
        /// <returns>dataset</returns>
        public System.Data.DataSet LogIsNotActive(string idAmm)
        {
            System.Data.DataSet ds;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ANAGRAFICA_LOG");
                q.setParam("param1", idAmm);
                string Sql = q.getSQL();
                logger.Debug("GetLogDisattivi - sql: " + Sql);

                if (!this.ExecuteQuery(out ds, "AZIONI", Sql))
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                ds = null;
            }

            return ds;
        }
        /// <summary>
        /// prende i log dall'anagrafica
        /// </summary>
        /// <returns>dataset</returns>
        public System.Data.DataSet GetLogDisattiviAmm()
        {
            System.Data.DataSet ds;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_LOG_DISATTIVI_AMM");
                string Sql = q.getSQL();
                logger.Debug("GetLogDisattiviAmm - sql: " + Sql);

                if (!this.ExecuteQuery(out ds, "AZIONIAMM", Sql))
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                ds = null;
            }

            return ds;
        }

        /// <summary>
        /// prende i log attivi di una amm.ne data
        /// </summary>
        /// <param name="idAmm">id amm.ne</param>
        /// <returns>dataset</returns>
        public System.Data.DataSet GetLogAttivi(string idAmm)
        {
            System.Data.DataSet ds;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_LOG_ATTIVI");
                q.setParam("param1", idAmm);
                string Sql = q.getSQL();
                logger.Debug("GetLogAttivi - sql: " + Sql);

                if (!this.ExecuteQuery(out ds, "AZIONI", Sql))
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                ds = null;
            }

            return ds;
        }


        /// <summary>
        /// prende i log attivi di una amm.ne data
        /// </summary>
        /// <param name="idAmm">id amm.ne</param>
        /// <returns>dataset</returns>
        public System.Data.DataSet GetLogAttiviByOggetto(string oggetto, string idAmm)
        {
            System.Data.DataSet ds;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_LOG_ATTIVI_BY_OGGETTO");
                q.setParam("param1", idAmm);
                q.setParam("oggetto", oggetto);
                string Sql = q.getSQL();
                logger.Debug("GetLogAttivi - sql: " + Sql);

                if (!this.ExecuteQuery(out ds, "AZIONI", Sql))
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                ds = null;
            }

            return ds;
        }

        /// <summary>
        /// prende i log attivi di una amm.ne data
        /// </summary>
        /// <param name="idAmm">id amm.ne</param>
        /// <returns>dataset</returns>
        public System.Data.DataSet GetLogAttiviAmm(string idAmm)
        {
            System.Data.DataSet ds;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_LOG_ATTIVI_AMM");
                //DocsPaUtils.Query quer = DocsPaUtils.InitQuery.getInstance().getQuery("S_LOG_ATTIVI_AMM");
                q.setParam("param1", idAmm);
                string Sql = q.getSQL();
                //string Sql = "SELECT a.VAR_CODICE as CODICE, a.VAR_DESCRIZIONE as DESCRIZIONE, a.VAR_OGGETTO as OGGETTO, VAR_METODO as METODO, 1 as ATTIVO ";
                //Sql += "FROM DPA_ANAGRAFICA_LOG a , DPA_LOG_ATTIVATI b ";
                //Sql += "WHERE a.system_id = b.system_id_anagrafica ";
                //Sql += "AND b.id_amm = " + Convert.ToInt16(idAmm) + " ";
                //Sql += "AND a.VAR_CODICE like 'AMM_%' ";
                //Sql += "ORDER BY a.VAR_OGGETTO";

                logger.Debug("GetLogAttiviAmm - sql: " + Sql);

                if (!this.ExecuteQuery(out ds, "AZIONIAMM", Sql))
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                ds = null;
            }

            return ds;
        }

        /// <summary>
        /// elimina i log di una amm.ne data
        /// </summary>
        /// <param name="codAmm">codice amm.ne</param>
        /// <returns>bool</returns>
        public bool DeleteLogFiltrato(string codAmm, string type)
        {
            bool result = true;
            try
            {
                AmministrazioneXml amm = new AmministrazioneXml();
                string idAmm = amm.GetAdminByName(codAmm);
                if (type.Equals("Amministrazione"))
                    idAmm += " AND VAR_COD_AZIONE LIKE 'AMM_%'";
                else
                    idAmm += " AND VAR_COD_AZIONE NOT LIKE 'AMM_%'";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DELETE_LOGS");
                q.setParam("param1", "ID_AMM = " + idAmm);

                string Sql = q.getSQL();
                logger.Debug("DeleteLogFiltrato - sql: " + Sql);

                if (!this.ExecuteNonQuery(Sql))
                {
                    logger.Debug("Errore nella eliminazione dei record dei log");
                    result = false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// metodo per eseguire query sui log, dati alcuni filtri. Restituisce un file xml
        /// </summary>
        /// <param name="dataDa">data da:</param>
        /// <param name="dataA">data a:</param>
        /// <param name="user">userid operatore</param>
        /// <param name="oggetto">oggetto del log</param>
        /// <param name="azione">azione del log</param>
        /// <param name="codAmm">codice amm.ne</param>
        /// <param name="esito">esito operazione</param>
        /// <returns>stream xml</returns>
        public System.Data.DataSet GetXmlLog(string dataDa, string dataA, string user, string oggetto, string azione, string codAmm, string esito, string type)
        {
            System.Data.DataSet ds;
            DocsPaUtils.Query q;
            string Sql = "";
            string Where;
            string idAmm;

            try
            {
                AmministrazioneXml amm = new AmministrazioneXml();
                idAmm = amm.GetAdminByName(codAmm);

                Where = setWhereCondition(dataDa, dataA, user, oggetto, azione, idAmm, esito);
                if (type.Equals("Amministrazione"))
                    Where += " AND VAR_COD_AZIONE LIKE 'AMM_%'";
                else
                    Where += " AND VAR_COD_AZIONE NOT LIKE 'AMM_%'";

                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_LOGS");
                q.setParam("param1", "SYSTEM_ID,USERID_OPERATORE,ID_PEOPLE_OPERATORE,ID_GRUPPO_OPERATORE,ID_AMM, DTA_AZIONE, VAR_OGGETTO, ID_OGGETTO,VAR_DESC_OGGETTO, VAR_COD_AZIONE,VAR_DESC_AZIONE, CHA_ESITO");
                q.setParam("param2", Where);

                Sql = q.getSQL();
                logger.Debug("GetXmlLog - sql: " + Sql);

                if (!this.ExecuteQuery(out ds, "QUERY_STORICO", Sql))
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                ds = null;
            }
            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataDa"></param>
        /// <param name="dataA"></param>
        /// <param name="user"></param>
        /// <param name="idOggetto"></param>
        /// <param name="oggetto"></param>
        /// <param name="azione"></param>
        /// <param name="codAmm"></param>
        /// <param name="esito"></param>
        /// <param name="type"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public System.Data.DataSet GetXmlLogFiltrato(string dataDa, string dataA, string user, string idOggetto, string oggetto, string azione, string codAmm, string esito, string type, int table)
        {
            System.Data.DataSet ds;
            DocsPaUtils.Query q;
            DocsPaUtils.Query q1;
            string Sql = "";
            string Where;
            string idAmm;

            try
            {
                AmministrazioneXml amm = new AmministrazioneXml();
                idAmm = amm.GetAdminByName(codAmm);

                Where = setWhereCondition(dataDa, dataA, user, idOggetto, oggetto, azione, idAmm, esito);
                if (type.Equals("Amministrazione"))





                    Where += " AND VAR_COD_AZIONE LIKE 'AMM_%'";
                else
                    Where += " AND VAR_COD_AZIONE NOT LIKE 'AMM_%'";

                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_LOGS");
                q1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_LOGS");
                switch (table)
                {
                    case 0:
                    case 1:
                        q.setParam("param1", "USERID_OPERATORE, DTA_AZIONE, VAR_OGGETTO, VAR_DESC_OGGETTO, VAR_DESC_AZIONE, CHA_ESITO, ID_PEOPLE_OPERATORE, ID_GRUPPO_OPERATORE, ID_AMM, ID_OGGETTO, VAR_COD_AZIONE");
                        q.setParam("param2", Where);
                        q.setParam("param3", "DPA_LOG");
                        break;

                    case 2:
                        q.setParam("param1", "USERID_OPERATORE, DTA_AZIONE, VAR_OGGETTO, VAR_DESC_OGGETTO, VAR_DESC_AZIONE, CHA_ESITO, ID_PEOPLE_OPERATORE, ID_GRUPPO_OPERATORE, ID_AMM, ID_OGGETTO, VAR_COD_AZIONE");
                        q.setParam("param2", Where);
                        q.setParam("param3", "DPA_LOG_STORICO");
                        break;

                    case 3:
                        q.setParam("param1", "USERID_OPERATORE, DTA_AZIONE, VAR_OGGETTO, VAR_DESC_OGGETTO, VAR_DESC_AZIONE, CHA_ESITO, ID_PEOPLE_OPERATORE, ID_GRUPPO_OPERATORE, ID_AMM, ID_OGGETTO, VAR_COD_AZIONE");
                        q.setParam("param2", Where);
                        q.setParam("param3", "DPA_LOG");
                        q1.setParam("param1", "USERID_OPERATORE, DTA_AZIONE, VAR_OGGETTO, VAR_DESC_OGGETTO, VAR_DESC_AZIONE, CHA_ESITO, ID_PEOPLE_OPERATORE, ID_GRUPPO_OPERATORE, ID_AMM, ID_OGGETTO, VAR_COD_AZIONE");
                        q1.setParam("param2", Where);
                        q1.setParam("param3", "DPA_LOG_STORICO");
                        break;

                    case 4:
                        q.setParam("param1", "USERID_OPERATORE, DTA_AZIONE, VAR_OGGETTO, VAR_DESC_OGGETTO, VAR_DESC_AZIONE, CHA_ESITO, ID_AMM, VAR_COD_AZIONE");
                        q.setParam("param2", Where);
                        q.setParam("param3", "PGU_LOG");
                        break;

                    //MEV CONS 1.3
                    //per gestione log in caso di amministazioni con log conservazione disabilitati
                    case 11:
                        q.setParam("param1", "USERID_OPERATORE, DTA_AZIONE, VAR_OGGETTO, VAR_DESC_OGGETTO, VAR_DESC_AZIONE, CHA_ESITO, ID_PEOPLE_OPERATORE, ID_GRUPPO_OPERATORE, ID_AMM, ID_OGGETTO, VAR_COD_AZIONE");
                        q.setParam("param2", Where + " AND VAR_OGGETTO NOT IN ('CONSERVAZIONE', 'AREA_CONSERVAZIONE')");
                        q.setParam("param3", "DPA_LOG");
                        break;

                    case 12:
                        q.setParam("param1", "USERID_OPERATORE, DTA_AZIONE, VAR_OGGETTO, VAR_DESC_OGGETTO, VAR_DESC_AZIONE, CHA_ESITO, ID_PEOPLE_OPERATORE, ID_GRUPPO_OPERATORE, ID_AMM, ID_OGGETTO, VAR_COD_AZIONE");
                        q.setParam("param2", Where + " AND VAR_OGGETTO NOT IN ('CONSERVAZIONE', 'AREA_CONSERVAZIONE')");
                        q.setParam("param3", "DPA_LOG_STORICO");
                        break;

                    case 13:
                        q.setParam("param1", "USERID_OPERATORE, DTA_AZIONE, VAR_OGGETTO, VAR_DESC_OGGETTO, VAR_DESC_AZIONE, CHA_ESITO, ID_PEOPLE_OPERATORE, ID_GRUPPO_OPERATORE, ID_AMM, ID_OGGETTO, VAR_COD_AZIONE");
                        q.setParam("param2", Where + " AND VAR_OGGETTO NOT IN ('CONSERVAZIONE', 'AREA_CONSERVAZIONE')");
                        q.setParam("param3", "DPA_LOG");
                        q1.setParam("param1", "USERID_OPERATORE, DTA_AZIONE, VAR_OGGETTO, VAR_DESC_OGGETTO, VAR_DESC_AZIONE, CHA_ESITO, ID_PEOPLE_OPERATORE, ID_GRUPPO_OPERATORE, ID_AMM, ID_OGGETTO, VAR_COD_AZIONE");
                        q1.setParam("param2", Where + " AND VAR_OGGETTO NOT IN ('CONSERVAZIONE', 'AREA_CONSERVAZIONE')");
                        q1.setParam("param3", "DPA_LOG_STORICO");
                        break;
                }

                Sql = q.getSQL();
                //Modifica per MEV CONS 1.3
                if (table == 3 || table == 13)
                    Sql = "SELECT * FROM (" + Sql + " UNION ALL " + q1.getSQL() + ") A ORDER BY A.DTA_AZIONE DESC";
                else
                    Sql += " ORDER BY DTA_AZIONE DESC";
                logger.Debug("GetXmlLogFiltrato - sql: " + Sql);

                if (!this.ExecuteQuery(out ds, "QUERY", Sql))
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                ds = null;
            }

            return ds;
        }

        /// <summary>
        /// metodo per eseguire query sui log, dati alcuni filtri. Restituisce un file xml
        /// </summary>
        /// <param name="dataDa">data da:</param>
        /// <param name="dataA">data a:</param>
        /// <param name="user">userid operatore</param>
        /// <param name="oggetto">oggetto del log</param>
        /// <param name="azione">azione del log</param>
        /// <param name="codAmm">codice amm.ne</param>
        /// <param name="esito">esito operazione</param>
        /// <returns>stream xml</returns>
        public System.Data.DataSet GetXmlLogFiltrato(string dataDa, string dataA, string user, string oggetto, string azione, string codAmm, string esito, string type, int table)
        {
            return this.GetXmlLogFiltrato(dataDa, dataA, user, string.Empty, oggetto, azione, codAmm, esito, type, table);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private string getUserIdByIdPeople(int Id)
        {
            string result = "";
            DocsPaUtils.Query q;
            string Sql = "";

            try
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PEOPLE");
                q.setParam("param1", "USER_ID");
                q.setParam("param2", "SYSTEM_ID = " + Id);
                Sql = q.getSQL();

                this.ExecuteScalar(out result, Sql);

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                result = "";
            }

            return result;
        }

        #endregion

        #region  Scrittura del Log nuova versione


        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserID_Operatore"></param>
        /// <param name="ID_People_Operatore"></param>
        /// <param name="ID_Gruppo_Operatore"></param>
        /// <param name="ID_Amministrazione"></param>
        /// <param name="Oggetto"></param>
        /// <param name="ID_Oggetto"></param>
        /// <param name="Var_desc_Oggetto"></param>
        /// <param name="Cod_Azione"></param>
        /// <param name="Desc_Azione"></param>
        /// <param name="Cha_Esito"></param>
        /// <returns></returns>
        public static bool InsertLog(string UserID_Operatore, string ID_People_Operatore, string ID_Gruppo_Operatore, string ID_Amministrazione,
                               string Oggetto, string ID_Oggetto, string Var_desc_Oggetto, string Cod_Azione, string Desc_Azione, CodAzione.Esito Cha_Esito, 
                                DocsPaVO.utente.InfoUtente delegato, string codWorkingApplication, string checkNotify = "", string idTrasm = "", string dataAzione = "")
        {
            DocsPaVO.utente.Utente user;
            string userId = string.Empty;
            int idPeople = -1;
            string idGruppo = string.Empty;
            int idAmministrazione;
            string VarOggetto = string.Empty;
            string IdOggetto = string.Empty;
            string VarDescOggetto = string.Empty;
            string VarCodAzione = string.Empty;
            string VarDescAzione = string.Empty;
            string Producer = string.Empty;
            int ChaEsito;
            string VarCodWorkingApplication;
            string CheckNotify = string.Empty;
            string IdTrasm = string.Empty;
            string DataAzione = string.Empty;
            string idPeopleDelegante = (delegato!=null?ID_People_Operatore:string.Empty);

            try
            {
                if (!string.IsNullOrEmpty(UserID_Operatore))
                {
                    if (UserID_Operatore.Length > 0)
                    {
                        userId = UserID_Operatore;
                        idPeople = Convert.ToInt32(ID_People_Operatore);
                        if (ID_Gruppo_Operatore == null)
                            idGruppo = "0";
                        else
                            idGruppo = ID_Gruppo_Operatore;
                    }
                }
                else
                {
                    UserID_Operatore = "";
                }


                if (delegato != null)
                {
                    userId = delegato.userId;
                    idPeople = Convert.ToInt32(delegato.idPeople);
                    //idGruppo = delegato.idGruppo;
                }

                if (!string.IsNullOrEmpty(ID_Amministrazione))
                    idAmministrazione = Convert.ToInt32(ID_Amministrazione);
                else
                    idAmministrazione = 0;


                VarOggetto = Oggetto;

                if (ID_Oggetto == null || ID_Oggetto == "")
                    IdOggetto = "0";
                else
                    IdOggetto = ID_Oggetto;

                if (Var_desc_Oggetto == null || Var_desc_Oggetto == "")
                    VarDescOggetto = " ";
                else
                    VarDescOggetto = Var_desc_Oggetto;

                VarCodAzione = Cod_Azione;

                if (delegato != null)
                {
                    Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();

                    //utente che delega
                    string descDelegante = string.Empty;
                    string nomeDelegante = string.Empty;
                    string ruoloDelegante = string.Empty;
                    user = utente.GetUtente(UserID_Operatore, ID_Amministrazione);
                    if (user != null && !string.IsNullOrEmpty(user.cognome) && !string.IsNullOrEmpty(user.nome))
                        nomeDelegante = user.cognome + " " + user.nome;
                    else
                        nomeDelegante = " ";
                    if (!string.IsNullOrEmpty(ID_Gruppo_Operatore) && !ID_Gruppo_Operatore.Equals("0"))
                    {
                        ruoloDelegante = utente.GetRoleDescriptionByIdGroup(ID_Gruppo_Operatore);
                    }


                    descDelegante = " SOSTITUTO DI " + nomeDelegante;

                    //utente delegato
                    string nomeDelegato = string.Empty;
                    string ruoloDelegato = string.Empty;
                    string descDelegato = string.Empty;
                    user = utente.GetUtente(delegato.userId, ID_Amministrazione);
                    nomeDelegato = user.cognome + " " + user.nome;
                    if (!string.IsNullOrEmpty(delegato.idGruppo) && !delegato.idGruppo.Equals("0"))
                    {
                        ruoloDelegato = utente.GetRoleDescriptionByIdGroup(delegato.idGruppo);
                    }
                    
                    descDelegato = nomeDelegato; 

                    //popolo il campo desc azione con il delegante
                    VarDescAzione = Desc_Azione + " (" + descDelegante + ")";
                    //popolo il campo desc producer
                    Producer = (descDelegato + descDelegante  +
                        (!string.IsNullOrEmpty(ruoloDelegante) ? " (nel ruolo " + ruoloDelegante + ")" : string.Empty)).Replace("'", "''");
                }
                else
                {
                    //produttore dell'evento
                    Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
                    string nomeProduttore = string.Empty;
                    string ruoloProduttore = string.Empty;
                    user = utente.GetUtente(UserID_Operatore, ID_Amministrazione);
                    if (user != null)
                        nomeProduttore = user.cognome + " " + user.nome;
                    else
                        nomeProduttore = UserID_Operatore;
                    if (!string.IsNullOrEmpty(ID_Gruppo_Operatore) && !ID_Gruppo_Operatore.Equals("0"))
                    {
                        ruoloProduttore = utente.GetRoleDescriptionByIdGroup(ID_Gruppo_Operatore);
                    }

                    //popolo il campo desc azione
                    VarDescAzione = Desc_Azione;

                    //popolo il campo desc producer con le informazioni sul produttore
                    Producer = nomeProduttore +
                        (!string.IsNullOrEmpty(ruoloProduttore) ? " (" + ruoloProduttore + ")" : string.Empty).Replace("'", "''");

                    //Se il log è tra i log definiti di tipo amministrazione scrivo come descrizione "Amministratore di sistema"
                    if(IsLogAmministrazione(Cod_Azione))
                    {
                        Producer = "Amministratore di sistema";
                    }
                }
                if (Cha_Esito != null)
                    ChaEsito = Convert.ToInt32(Cha_Esito);
                else
                    ChaEsito = 0;
                VarCodWorkingApplication = codWorkingApplication;

                if (!string.IsNullOrEmpty(checkNotify) && checkNotify.Equals("1"))
                    CheckNotify = checkNotify;

                if (!string.IsNullOrEmpty(idTrasm))
                    IdTrasm = idTrasm;

                if (!string.IsNullOrEmpty(dataAzione))
                    DataAzione = dataAzione;

            }
            catch (Exception ex)
            {
                logger.Error(" Errore in InsertLog ID_PROFILE: " + IdOggetto + " ID_RUOLO: " + idGruppo+ " " + ex.Message + " " + ex.StackTrace);
                return false;
            }
            return Insert(userId, idPeople, idGruppo, idAmministrazione, VarOggetto, IdOggetto, VarDescOggetto, VarCodAzione, VarDescAzione, Producer, ChaEsito, VarCodWorkingApplication, CheckNotify, IdTrasm, DataAzione, idPeopleDelegante);
        }

        /// <summary>
        /// Verifica se il log in input è un log di amministrazione
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public static bool IsLogAmministrazione(string log)
        {
            return Enum.IsDefined(typeof(DocsPaVO.documento.LogAmministrazione), log.ToUpper());
        }

        private static bool Insert(string userId, int idPeople, string idGruppo, int idAmministrazione, string VarOggetto, string IdOggetto, string VarDescOggetto, string VarCodAzione, string VarDescAzione, string Producer, int ChaEsito, string VarCodWorkingApplication, string CheckNotify, string IdTrasm, string DataAzione, string idPeopleDelegante)
        {
            bool result = true;
            DocsPaUtils.Query q;
            string Sql = "";
            string dtaAzione = string.Empty;

            try
            {

                if(string.IsNullOrEmpty(idGruppo))
                    idGruppo="0";

                if (!string.IsNullOrEmpty(DataAzione))
                {
                    dtaAzione = DocsPaDbManagement.Functions.Functions.ToDate(DataAzione); 
                }
                else
                {
                    dtaAzione = DocsPaDbManagement.Functions.Functions.GetDate();
                }

                using (DocsPaDB.DBProvider db = new DocsPaDB.DBProvider())
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPALOG");
                    if (!string.IsNullOrEmpty(CheckNotify) && !string.IsNullOrEmpty(IdTrasm))
                    {
                        q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName() + " USERID_OPERATORE, ID_PEOPLE_OPERATORE, ID_GRUPPO_OPERATORE,ID_AMM, DTA_AZIONE, VAR_OGGETTO, ID_OGGETTO, VAR_DESC_OGGETTO,VAR_COD_AZIONE, VAR_DESC_AZIONE, CHA_ESITO, VAR_COD_WORKING_APPLICATION,CHECK_NOTIFY,ID_TRASM_SINGOLA,DESC_PRODUCER, ID_PEOPLE_DELEGANTE");
                        q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_LOG") +
                        "'" + userId + "'," + idPeople + "," + idGruppo + "," + idAmministrazione + "," + dtaAzione + ",'" + VarOggetto.Replace("'", "''") + "'," +
                        IdOggetto.Replace("'", "''") + ",'" + VarDescOggetto.Replace("'", "''") + "','" + VarCodAzione.Replace("'", "''") + "','" + VarDescAzione.Replace("'", "''") + "','" + ChaEsito + "','" + VarCodWorkingApplication + "','" + CheckNotify + "', '" + IdTrasm + "', '" + Producer.Replace("'", "''") + "', " + (string.IsNullOrEmpty(idPeopleDelegante)?"null":"'" + idPeopleDelegante + "'"));
                    }
                    else if (!string.IsNullOrEmpty(CheckNotify) && string.IsNullOrEmpty(IdTrasm))
                    {
                        q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName() + " USERID_OPERATORE, ID_PEOPLE_OPERATORE, ID_GRUPPO_OPERATORE,ID_AMM, DTA_AZIONE, VAR_OGGETTO, ID_OGGETTO, VAR_DESC_OGGETTO,VAR_COD_AZIONE, VAR_DESC_AZIONE, CHA_ESITO, VAR_COD_WORKING_APPLICATION,CHECK_NOTIFY,DESC_PRODUCER, ID_PEOPLE_DELEGANTE");
                        q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_LOG") +
                        "'" + userId + "'," + idPeople + "," + idGruppo + "," + idAmministrazione + "," + dtaAzione + ",'" + VarOggetto.Replace("'", "''") + "'," +
                         IdOggetto.Replace("'", "''") + ",'" + VarDescOggetto.Replace("'", "''") + "','" + VarCodAzione.Replace("'", "''") + "','" + VarDescAzione.Replace("'", "''") + "','" + ChaEsito + "','" + VarCodWorkingApplication + "','" + CheckNotify + "', '" + Producer.Replace("'", "''") + "', " + (string.IsNullOrEmpty(idPeopleDelegante) ? "null" : "'" + idPeopleDelegante + "'"));
                    }
                    else
                    {
                        q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName() + " USERID_OPERATORE, ID_PEOPLE_OPERATORE, ID_GRUPPO_OPERATORE,ID_AMM, DTA_AZIONE, VAR_OGGETTO, ID_OGGETTO, VAR_DESC_OGGETTO,VAR_COD_AZIONE, VAR_DESC_AZIONE, CHA_ESITO, VAR_COD_WORKING_APPLICATION,DESC_PRODUCER, ID_PEOPLE_DELEGANTE");
                        q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_LOG") +
                        "'" + userId + "'," + idPeople + "," + idGruppo + "," + idAmministrazione + "," + dtaAzione + ",'" + VarOggetto.Replace("'", "''") + "'," +
                         IdOggetto.Replace("'", "''") + ",'" + VarDescOggetto.Replace("'", "''") + "','" + VarCodAzione.Replace("'", "''") + "','" + VarDescAzione.Replace("'", "''") + "','" + ChaEsito + "','" + VarCodWorkingApplication + "', '" + Producer.Replace("'", "''") + "', " + (string.IsNullOrEmpty(idPeopleDelegante) ? "null" : "'" + idPeopleDelegante + "'"));
                    }

                    Sql = q.getSQL();
                   if( !db.ExecuteNonQuery(Sql))
                       logger.Error("errore nell'insert nella DPA_LOG commando sql: ," + Sql);

                    logger.Debug("LOG " + Sql);
                }
                CheckNotify = string.Empty;
                IdTrasm = string.Empty;
                result = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message.ToString(), ex);
                result = false;
            }

            return result;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserID_Operatore"></param>
        /// <param name="ID_People_Operatore"></param>
        /// <param name="ID_Gruppo_Operatore"></param>
        /// <param name="ID_Amministrazione"></param>
        /// <param name="Oggetto"></param>
        /// <param name="ID_Oggetto"></param>
        /// <param name="Var_desc_Oggetto"></param>
        /// <param name="Cod_Azione"></param>
        /// <param name="Desc_Azione"></param>
        /// <param name="Cha_Esito"></param>
        /// <returns></returns>
        public bool InsertLogStorico(string UserID_Operatore, string ID_People_Operatore, string ID_Gruppo_Operatore, string ID_Amministrazione,
                               string Oggetto, string ID_Oggetto, string Var_desc_Oggetto, string Cod_Azione, string Desc_Azione, string Cha_Esito, string data_azione)
        {
            string userid = string.Empty;
            int idPeople = 0;
            string idGruppo = string.Empty;
            int idAmministrazione = 0;
            string VarOggetto = string.Empty;
            string IdOggetto = string.Empty;
            string VarDescOggetto = string.Empty;
            string VarCodAzione = string.Empty;
            string VarDescAzione = string.Empty;
            int ChaEsito = 0;

            if (UserID_Operatore.Length > 0)
                userid = UserID_Operatore;

            idPeople = Convert.ToInt32(ID_People_Operatore);

            if (ID_Gruppo_Operatore == null)
                idGruppo = "0";
            else
                idGruppo = ID_Gruppo_Operatore;

            if (ID_Amministrazione != null)
                idAmministrazione = Convert.ToInt32(ID_Amministrazione);
            else
                idAmministrazione = 0;


            VarOggetto = Oggetto;

            if (ID_Oggetto == null)
                IdOggetto = "0";
            else
                IdOggetto = ID_Oggetto;

            if (Var_desc_Oggetto == null)
                VarDescOggetto = " ";
            else
                VarDescOggetto = Var_desc_Oggetto;

            VarCodAzione = Cod_Azione;


            VarDescAzione = Desc_Azione;

            ChaEsito = Convert.ToInt16(Cha_Esito);

            this.dataAzione = data_azione;

            return InsertStorico(userid, idPeople, idGruppo, idAmministrazione, VarOggetto, IdOggetto, VarDescOggetto, VarCodAzione, VarDescAzione, ChaEsito);
        }


        private bool InsertStorico(string userid, int idPeople, string idGruppo, int idAmministrazione, string VarOggetto, string IdOggetto, string VarDescOggetto, string VarCodAzione, string VarDescAzione, int ChaEsito)
        {
            bool result = true;
            DocsPaUtils.Query q;
            string Sql = "";

            try
            {
                using (DocsPaDB.DBProvider db = new DocsPaDB.DBProvider())
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPALOG_STORICO");
                    if (!dbType.ToUpper().Equals("SQL"))
                    {
                        string nextVal = DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_LOG_STORICO");
                        //nextVal = nextVal.Substring(0, nextVal.Length - 2);
                        q.setParam("param1", "SYSTEM_ID, USERID_OPERATORE, ID_PEOPLE_OPERATORE, ID_GRUPPO_OPERATORE,ID_AMM, DTA_AZIONE, VAR_OGGETTO, ID_OGGETTO, VAR_DESC_OGGETTO,VAR_COD_AZIONE, VAR_DESC_AZIONE, CHA_ESITO");
                        q.setParam("param2", nextVal + "'" + userid + "'," + idPeople + "," + idGruppo + "," + idAmministrazione + "," + DocsPaDbManagement.Functions.Functions.ToDate(dataAzione) + ",'" + VarOggetto + "'," +
                            IdOggetto + ",'" + VarDescOggetto.Replace("'", "''") + "','" + VarCodAzione + "','" + VarDescAzione.Replace("'", "''") + "','" + ChaEsito + "'");
                    }
                    else
                    {
                        q.setParam("param1", " USERID_OPERATORE, ID_PEOPLE_OPERATORE, ID_GRUPPO_OPERATORE,ID_AMM, DTA_AZIONE, VAR_OGGETTO, ID_OGGETTO, VAR_DESC_OGGETTO,VAR_COD_AZIONE, VAR_DESC_AZIONE, CHA_ESITO");
                        q.setParam("param2", "'" + userid + "'," + idPeople + "," + idGruppo + "," + idAmministrazione + "," + DocsPaDbManagement.Functions.Functions.ToDate(dataAzione) + ",'" + VarOggetto + "'," +
                            IdOggetto + ",'" + VarDescOggetto.Replace("'", "''") + "','" + VarCodAzione + "','" + VarDescAzione.Replace("'", "''") + "','" + ChaEsito + "'");
                    }
                    Sql = q.getSQL();
                    //Sql = "INSERT INTO DPA_LOG_STORICO "; 
                    //Sql += "(USERID_OPERATORE,ID_PEOPLE_OPERATORE,ID_GRUPPO_OPERATORE,ID_AMM, DTA_AZIONE, VAR_OGGETTO, ID_OGGETTO,VAR_DESC_OGGETTO, VAR_COD_AZIONE,VAR_DESC_AZIONE, CHA_ESITO) ";
                    //Sql += "VALUES ";
                    //Sql += "('" + this.userId + "'," + this.idPeople + "," + this.idGruppo + "," + this.idAmministrazione + "," + DocsPaDbManagement.Functions.Functions.ToDate(dataAzione) + ",'" + this.VarOggetto + "',"
                    //    + this.IdOggetto + ",'" + this.VarDescOggetto + "','" + this.VarCodAzione + "','" + this.VarDescAzione.Replace("'", "''") + "','" + this.ChaEsito + "')";

                    logger.Debug("LOG_STORICO " + Sql);
                    if (!db.ExecuteNonQuery(Sql))
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message.ToString(), ex);
                result = false;
            }

            return result;

        }


        public static DocsPaVO.Logger.CodAzione.infoOggetto getInfoOggetto(string NomeWebMethod, string CodAdmin)
        {
            DocsPaVO.Logger.CodAzione.infoOggetto InfoOggetto = new DocsPaVO.Logger.CodAzione.infoOggetto();
            DocsPaUtils.Query q;
            string Sql = "";
            System.Data.DataSet ds;

            try
            {

                logger.Debug("getInfoOggetto -Log-");
                using (DocsPaDB.DBProvider db = new DocsPaDB.DBProvider())
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPAANAGRAFICA_LOG__DPA_LOG_ATTIVATI");
                    if (!string.IsNullOrEmpty(CodAdmin))
                        q.setParam("param1", "AND AT.ID_AMM = " + CodAdmin);
                    else
                        q.setParam("param1", "");
                    q.setParam("param2", string.Format("{0}{1}{0}", "'", NomeWebMethod));
                    Sql = q.getSQL();

                    logger.Debug("getInfoOggetto " + Sql);

                    if (db.ExecuteQuery(out ds, "LogAttivi", Sql))
                    {
                        if (ds.Tables["LogAttivi"].Rows.Count > 0)
                        {
                            InfoOggetto.Codice = ds.Tables["LogAttivi"].Rows[ds.Tables["LogAttivi"].Rows.Count - 1]["VAR_CODICE"].ToString();
                            InfoOggetto.Attivo = 1;
                            InfoOggetto.Descrizione = ds.Tables["LogAttivi"].Rows[ds.Tables["LogAttivi"].Rows.Count - 1]["VAR_DESCRIZIONE"].ToString();
                            InfoOggetto.Oggetto = ds.Tables["LogAttivi"].Rows[ds.Tables["LogAttivi"].Rows.Count - 1]["VAR_OGGETTO"].ToString();
                            InfoOggetto.Notify = ds.Tables["LogAttivi"].Rows[ds.Tables["LogAttivi"].Rows.Count - 1]["NOTIFY"].ToString();
                            ds.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("getInfoOggetto -Log-", ex);
            }

            return InfoOggetto;

        }
        #endregion

        #region Scrittura del Log vecchia versione

        public bool WriteLog(string idAmministrazione, string idGruppo, string idPeople, string userId,
                             CodAzione.VAR_COD_AZIONE VarCodAzione, string IdOggetto, string VarCodOggetto, CodAzione.Esito ChaEsito, string VarDescOggetto)
        {
            int IdAmministrazione = 0;
            string IdGruppo = string.Empty;
            int IdPeople = 0;
            string UserId = string.Empty;
            string varCodAzione = string.Empty;
            string idOggetto = string.Empty;
            int chaEsito = 0;
            string varCodOggetto = string.Empty;
            string varDescOggetto = string.Empty;

            try
            {
                if (idAmministrazione.Length == 0)
                {
                    IdAmministrazione = 0;
                }
                else
                {
                    IdAmministrazione = Convert.ToInt32(idAmministrazione);
                }

                if (idGruppo.Length == 0)
                {
                    IdGruppo = " ";
                }
                else
                {
                    IdGruppo = idGruppo;
                }

                if (idPeople.Length == 0)
                {
                    IdPeople = 0;
                }
                else
                {
                    IdPeople = Convert.ToInt32(idPeople);
                }

                if (userId.Length == 0)
                {
                    UserId = getUserIdByIdPeople(IdPeople);
                }
                else
                {
                    UserId = userId;
                }

                varCodAzione = Enum.GetName(typeof(CodAzione.VAR_COD_AZIONE), VarCodAzione).ToString().Replace("_", " ");

                if ((IdOggetto == null) || (IdOggetto.Length == 0))
                {
                    idOggetto = "0";
                }
                else
                {
                    idOggetto = IdOggetto;
                }

                chaEsito = Convert.ToInt32(ChaEsito);

                if (VarCodOggetto.Length == 0)
                {
                    varCodOggetto = Blank;
                }
                else
                {
                    varCodOggetto = VarCodOggetto;
                }


                varDescOggetto = VarDescOggetto;

            }
            catch (Exception ex)
            {
                logger.Error("errore in WriteLog: " + ex.Message + " " + ex.StackTrace);
                return false;
            }


            return writelog(IdAmministrazione, IdGruppo, IdPeople, UserId, varCodAzione, idOggetto, chaEsito, varCodOggetto, varDescOggetto);
        }

        private bool writelog(int IdAmministrazione, string IdGruppo, int IdPeople, string UserId, string varCodAzione, string idOggetto, int chaEsito, string varCodOggetto, string varDescOggetto)
        {
            bool result = true;
            DocsPaUtils.Query q;
            string Sql = "";

            try
            {
                using (DocsPaDB.DBProvider db = new DocsPaDB.DBProvider())
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPALOG");
                    q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName() + " USER_ID, ID_UTENTE, ID_RUOLO, VAR_COD_AZIONE, DTA_AZIONE, ID_OGGETTO, VAR_COD_OGGETTO, CHA_ESITO, ID_AMM, VAR_DESC_OGGETTO");
                    q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_LOG") +
                    "'" + UserId + "' , " + IdPeople.ToString() + ", " + IdGruppo + ", '" + varCodAzione + "', " +
                    DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")) + ", " + idOggetto + ", '" + varCodOggetto + "', '" + chaEsito + "' , " + IdAmministrazione + ", '" + varDescOggetto + "'");
                    Sql = q.getSQL();

                    logger.Debug("LOG " + Sql);
                    if (!this.ExecuteNonQuery(Sql))
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message.ToString(), ex);
                result = false;
            }

            return result;
        }

        #endregion

        #region Lettura del log

        #region codice commentato
        //		private string setWhereCondition(string disabled)
        //		{
        //			string result="";
        //
        //			switch (disabled)
        //			{
        //				case "A":
        //				result = string.Format("{0} {1}","DISABLED IS NOT NULL");
        //				break;
        //
        //				case "N":
        //				result = string.Format("{0} = {1}{2}{1}", "DISABLED","'",Enum.GetName(typeof(CodAzione),CodAzione.DISABLED_USERID.N.ToString().ToUpper()));
        //				break;
        //
        //				case "Y":
        //				result = string.Format("{0} = {1}{2}{1}", "DISABLED","'",Enum.GetName(typeof(CodAzione),CodAzione.DISABLED_USERID.Y.ToString().ToUpper()));
        //				break;
        //			}
        //			
        //			return result;
        //		}
        #endregion

        private string setWhereCondition(string dataDa, string dataA, string user, string idOggetto, string oggetto, string azione, string idAmm, string esito)
        {
            string result = "ID_AMM = " + idAmm;

            //
            // Controllo per Log PGU
            if (!string.IsNullOrEmpty(oggetto))
            {
                if (oggetto.Equals("PGU"))
                    result = "( " + result + " OR ID_AMM IS NULL )";
            }
            // 
            // End Controllo



            if (dataDa != null && dataDa != "")
                result += " AND DTA_AZIONE >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(dataDa, true);

            if (dataA != null && dataA != "")
                result += " AND DTA_AZIONE <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(dataA, false);

            if (user != null && user != "")
                result += " AND UPPER(USERID_OPERATORE) = UPPER('" + user + "')";

            if (oggetto != null && oggetto != "")
                result += " AND VAR_OGGETTO = '" + oggetto + "'";

            if (azione != null && azione != "")
                result += " AND VAR_COD_AZIONE = '" + azione + "'";

            if (esito != null && esito != "")
                result += " AND CHA_ESITO = '" + esito + "'";

            if (idOggetto != null && idOggetto != "")
                result += " AND ID_OGGETTO = '" + idOggetto + "'";

            return result;
        }

        private string setWhereCondition(string dataDa, string dataA, string user, string oggetto, string azione, string idAmm, string esito)
        {
            return this.setWhereCondition(dataDa, dataA, user, string.Empty, oggetto, azione, idAmm, esito);
        }

        public bool VerficiaTemporaleLogAggiungiDocumento(string intervallo)
        {
            bool result = false;
            DocsPaUtils.Query q;
            System.Data.DataSet ds;
            string Sql = string.Empty;

            try
            {
                logger.Debug("VerficiaTemporaleLogAggiungiDocumento -Log-");
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_VERIFICA_INTERVALLO_LOG_PROTOCOLLO");
                q.setParam("intervallo", intervallo);
                Sql = q.getSQL();

                logger.Debug("VerficiaTemporaleLogAggiungiDocumento " + Sql);

                if (this.ExecuteQuery(out ds, "LogAttivi", Sql))
                {
                    if (ds.Tables["LogAttivi"].Rows.Count > 0)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("VerficiaTemporaleLogAggiungiDocumento -Log-", ex);
                return false;
            }

            return result;

        }

        #region codice commentato
        //		private string setWhereCondition(string Action, string Oggetto, string UserId, string dateStart, string dateEnd,string esito, string disabled)
        //		{
        //			string result="";
        //			string where="";
        //
        //			if (Action.Length > 0)
        //			{
        //				result = string.Format("{0} = {1}{2}{1}","VAR_COD_AZIONE", "'",Action.Replace("_", " "));			
        //				result = result + string.Format("{0}{1}{0}", " ", Enum.GetName(typeof(Condition), Condition.AND).ToUpper());
        //			}
        //			
        //			
        //			if (Oggetto.Length > 0)
        //			{
        //				result = result  + string.Format("{0} = {1}{2}{1}", "VAR_COD_OGGETTO", "'",Oggetto);
        //				result = result + string.Format("{0}{1}{0}", " ", Enum.GetName(typeof(Condition), Condition.AND).ToUpper());
        //			}
        //
        //			if (UserId.Length > 0)
        //			{
        //				result = result  + string.Format("{0} LIKE {1}{2}{1}","USER_ID","'", UserId);
        //				result = result + string.Format("{0}{1}{0}", " ", Enum.GetName(typeof(Condition), Condition.AND).ToUpper());
        //			}
        //
        //			if (esito.Length > 0)
        //			{
        //				result = result + string.Format("{0} = {1}{2}{1}", "'","CHA_ESITO");
        //				result = result + string.Format("{0}{1}{0}", " ", Enum.GetName(typeof(Condition), Condition.AND).ToUpper());
        //			}
        //
        //
        //
        //			if ((dateStart.Length > 0) && (dateEnd.Length > 0))
        //			{	
        //				result = result  + string.Format("{0} {1} {2} {3} {4}", DocsPaDbManagement.Functions.Functions.ToDate(DocsPaDbManagement.Functions.Functions.ToChar("DTA_AZIONE",false ) ,false),"BETWEEN",DocsPaDbManagement.Functions.Functions.ToDate("'" + dateStart + "'",false),Enum.GetName(typeof(Condition), Condition.AND).ToUpper(), DocsPaDbManagement.Functions.Functions.ToDate("'" + dateEnd + "'",false));
        //			}
        //			else
        //			{
        //				if (dateStart.Length > 0)
        //					result = result  + string.Format("{0} = {1}",  DocsPaDbManagement.Functions.Functions.ToDate(DocsPaDbManagement.Functions.Functions.ToChar("DTA_AZIONE",false ) ,false) ,DocsPaDbManagement.Functions.Functions.ToDate( "'" + dateStart + "'",false));				
        //			}
        //
        //			
        //			if (result.Trim().Substring((result.Trim().Length -  Enum.GetName(typeof(Condition), Condition.AND).ToString().Length), Enum.GetName(typeof(Condition), Condition.AND).ToString().Length).Trim() == Enum.GetName(typeof(Condition), Condition.AND).ToString().ToUpper())
        //				where = result.Trim().Substring(0,(result.Trim().Length -  Enum.GetName(typeof(Condition), Condition.AND).ToString().Length));
        //			else
        //				where =result;
        //			
        //			return where;
        //		}



        //		public System.Data.DataSet getLog(string Action, string Oggetto, string UserId, string dateStart, string dateEnd, string esito,string disabled)
        //		{
        //			System.Data.DataSet ds = new System.Data.DataSet();
        //			DocsPaUtils.Query q;
        //			string Sql="";
        //			string Where;
        //
        //			try
        //			{			
        //				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPALOG");
        //				Where= setWhereCondition( Action,  Oggetto,  UserId,  dateStart,  dateEnd, esito,disabled);
        //				q.setParam("param1",Where);
        //				Sql = q.getSQL();	
        //
        //				logger.Debug("ReadLOG " + Sql);
        //				if (!this.ExecuteQuery(ds,Sql))
        //				{
        //					throw new Exception();
        //				}
        //			}
        //			catch (Exception ex)
        //			{
        //				logger.Debug(ex.Message.ToString(),ex);
        //				ds=null;	
        //			}
        //
        //			return ds;
        //		
        //		}

        #endregion

        #endregion

        #region getUser

        //		public System.Data.DataSet getUser(string disabled)
        //		{
        //			System.Data.DataSet ds = new System.Data.DataSet();
        //			DocsPaUtils.Query q;
        //			string Sql="";
        //			string Where;
        //
        //			try
        //			{
        //				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PEOPLE_ALL");
        //				Where = setWhereCondition(disabled);
        //				q.setParam("param1","User_Id, System_Id, Disabled");
        //				q.setParam("param2",Where);
        //				Sql = q.getSQL();	
        //
        //				logger.Debug("getUser " + Sql);
        //				if (!this.ExecuteQuery(ds,Sql))
        //				{
        //					throw new Exception();
        //				}
        //
        //			}
        //			catch (Exception ex)
        //			{
        //				logger.Debug(ex.Message.ToString(),ex);
        //				ds=null;
        //			}
        //
        //			return ds;
        //		}

        #endregion
    }
}

