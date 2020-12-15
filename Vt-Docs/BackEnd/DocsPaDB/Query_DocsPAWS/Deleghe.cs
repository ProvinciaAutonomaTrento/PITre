using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DocsPaUtils.Interfaces.DbManagement;
using DocsPaDbManagement.Functions;
using DocsPaVO.Deleghe;
using DocsPaVO.ricerche;
using DocsPaDB.Utils;
using DocsPaUtils;
using log4net;
using System.Globalization;

namespace DocsPaDB.Query_DocsPAWS
{
    public class Deleghe : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(Deleghe));
        public Deleghe()
        { }

        //Creazione di una nuova delega 
        public bool creaNuovaDelega(DocsPaVO.Deleghe.InfoDelega delega)
        {
            bool result = true;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                   
                    //inserimento della nuova delega nella tabelle DPA_DELEGHE
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_DELEGHE");
                    q.setParam("param1", delega.id_utente_delegante + "," +
                        delega.id_ruolo_delegante + "," +
                        delega.id_utente_delegato + "," +
                        delega.id_ruolo_delegato + "," +
                        delega.id_uo_delegato + "," +
                        "'" + delega.cod_utente_delegante + "'" + "," +
                        "'" + delega.cod_ruolo_delegante.Replace("'","''") + "'" + "," +
                        "'" + delega.cod_utente_delegato + "'" + "," +
                        "'" + delega.cod_ruolo_delegato.Replace("'","''") + "'" + "," +
                        DocsPaDbManagement.Functions.Functions.ToDate(delega.dataDecorrenza) + "," + DocsPaDbManagement.Functions.Functions.ToDate(delega.dataScadenza) + ", '0'");
                    q.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    q.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(""));

                    queryString = q.getSQL();
                    logger.Debug("Inserimento nuova delega: ");
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }

        //Modifica una delega esistente 
        public bool modificaDelega(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Deleghe.InfoDelega delega, string tipoDelega, string idRuoloOld, string idUtenteOld, string dataScadenzaOld, string dataDecorrenzaOld, string idRuoloDeleganteOld)
        {
            bool result = true;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    switch (tipoDelega)
                    {
                        //Modifica delega attiva
                        //Posso modifica l'utente delegato o il ruolo del delegante o la data di scadenza
                        //Non può essere modificata la data di decorrenza
                        case "A":
                            #region delega attiva
                            //verifico se la delega è in esercizio
                            //se in esercizio non si può effettuare la modifica
                            if (checkDelegaInEsercizio(delega))
                            {
                                //non si sta esercitando una delega
                                //caso in cui è stato modificato l'utente delegato o il ruolo dell'utente delegante
                                if (!string.IsNullOrEmpty(idRuoloOld) || !string.IsNullOrEmpty(idUtenteOld) || !string.IsNullOrEmpty(idRuoloDeleganteOld))
                                {
                                    //Verifico che non ci sia già un'altra delega per lo stesso utente nello stesso periodo
                                    if (verificaUnicaDelega(infoUtente, delega))
                                    {
                                        //Nel caso di delega attiva, non in esercizio e non sovrapposta, la modifica del delegato comporterà:
                                        //modifica della delega sulla data di scadenza che verrà impostata alla data/ora di esecuzione dell'operazione
                                        //creazione di una nuova delega che avrà come delegato il nuovo utente selezionato
                                        q = DocsPaUtils.InitQuery.getInstance().getQuery("U_REVOCA_DELEGA");
                                        q.setParam("id_delega", delega.id_delega);
                                        queryString = q.getSQL();
                                        logger.Debug(queryString);
                                        if (!dbProvider.ExecuteNonQuery(queryString))
                                        {
                                            result = false;
                                            dbProvider.RollbackTransaction();
                                            throw new Exception();
                                        }
                                        result = creaNuovaDelega(delega);
                                    }
                                    else
                                        //Esiste una delega nello stesso periodo per lo stesso utente, non può essere modificata la delega
                                        result = false;
                                }
                                else
                                {
                                    //caso in cui è stata modificata solo la data di scadenza (prolungamento della delega o delega permanente)
                                    if ((string.IsNullOrEmpty(delega.dataScadenza) || Convert.ToDateTime(delega.dataScadenza) >= DateTime.Now)
                                        && (Convert.ToDateTime(dataDecorrenzaOld).ToShortDateString() == Convert.ToDateTime(delega.dataDecorrenza).ToShortDateString()))
                                    {
                                        //Si modifica la data di scadenza
                                        q = DocsPaUtils.InitQuery.getInstance().getQuery("U_MODIFICA_DELEGA");
                                        q.setParam("id_delega", delega.id_delega);
                                        q.setParam("param1", "DATA_SCADENZA = " + DocsPaDbManagement.Functions.Functions.ToDate(delega.dataScadenza));
                                        queryString = q.getSQL();
                                        logger.Debug(queryString);
                                        if (!dbProvider.ExecuteNonQuery(queryString))
                                        {
                                            result = false;
                                            dbProvider.RollbackTransaction();
                                            throw new Exception();
                                        }
                                    }
                                    else
                                        result = false;
                                }
                            }
                            else
                                result = false;
                            break;
                            #endregion
                        //Modifica delega impostata
                        case "I":
                            #region delega impostata
                            if (verificaUnicaDelega(infoUtente, delega))
                                {
                                    q = DocsPaUtils.InitQuery.getInstance().getQuery("DELETE_DELEGA");
                                    q.setParam("id_delega", delega.id_delega);
                                    queryString = q.getSQL();
                                    logger.Debug(queryString);
                                    if (!dbProvider.ExecuteNonQuery(queryString))
                                    {
                                        result = false;
                                        dbProvider.RollbackTransaction();
                                        throw new Exception();
                                    }
                                    result = creaNuovaDelega(delega);
                                }
                                else
                                    result = false;
                            break;
                            #endregion
                        //Modifica delega scaduta
                        case "S":
                        #region delega scaduta
                            //se la modifica è un'estensione della data di scadenza allora si modifica
                            //la delega altrimenti se ne crea una nuova
                            if ((string.IsNullOrEmpty(idRuoloOld) && string.IsNullOrEmpty(idUtenteOld) && string.IsNullOrEmpty(idRuoloDeleganteOld))
                                && (Convert.ToDateTime(dataDecorrenzaOld).ToShortDateString() == Convert.ToDateTime(delega.dataDecorrenza).ToShortDateString())
                                && (Convert.ToDateTime(dataScadenzaOld) == DateTime.Now)
                                && ( string.IsNullOrEmpty(delega.dataScadenza) || Convert.ToDateTime(delega.dataScadenza) >= DateTime.Now ) 
                                )
                            {
                                 if (verificaUnicaDelega(infoUtente, delega))
                                 {
                                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_MODIFICA_DELEGA");
                                    q.setParam("id_delega", delega.id_delega);
                                    q.setParam("param1", "DATA_DECORRENZA = " + DocsPaDbManagement.Functions.Functions.ToDate(delega.dataDecorrenza) + ", DATA_SCADENZA = " + DocsPaDbManagement.Functions.Functions.ToDate(delega.dataScadenza));
                                    queryString = q.getSQL();
                                    logger.Debug(queryString);
                                    if (!dbProvider.ExecuteNonQuery(queryString))
                                    {
                                        result = false;
                                        dbProvider.RollbackTransaction();
                                        throw new Exception();
                                    }
                                }
                                else
                                    result = false;
                            }
                            else
                                if (verificaUnicaDelega(infoUtente, delega))
                                {
                                    result = creaNuovaDelega(delega);
                                }
                                else
                                    result = false;
                            break;
                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }

        public List<InfoDelega> searchDeleghe(DocsPaVO.utente.InfoUtente infoUtente, SearchDelegaInfo searchInfo)
        {
            List<InfoDelega> deleghe = new List<InfoDelega>();
            logger.Debug("searchDeleghe");
            Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_DELEGHE_ASSEGNATE");
            query.setParam("order", "order by data_decorrenza desc");
            query.setParam("param1", DelegheStrategy.getStrategy(searchInfo.TipoDelega).getWhereClause(infoUtente, searchInfo));
            string commandText = query.getSQL();
            logger.Debug(commandText);
            DataSet dataSet;
            if (this.ExecuteQuery(out dataSet, "DELEGHE", commandText))
            {
                foreach (DataRow dataRow in dataSet.Tables["DELEGHE"].Rows)
                {
                    InfoDelega infoDelega = buildDelega(dataRow);
                    deleghe.Add(infoDelega);
                }
                dataSet.Dispose();
            }
            else
            {
                logger.Debug("Errore nell'esecuzione della query in 'searchDeleghe'");
                throw new ApplicationException("Errore nell'esecuzione della query 'searchDeleghe'");
            }
            return deleghe;
        }

        public List<InfoDelega> searchDeleghe(DocsPaVO.utente.InfoUtente infoUtente, SearchDelegaInfo searchInfo, SearchPagingContext pagingContext)
        {
            List<InfoDelega> deleghe = new List<InfoDelega>();
            logger.Debug("searchDeleghe");
            string queryFrom = string.Empty;
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("param1", DelegheStrategy.getStrategy(searchInfo.TipoDelega).getWhereClause(infoUtente, searchInfo));
            PagingQuery pagingQuery = new PagingQuery("S_COUNT_DELEGHE", "S_DELEGHE_PAGING", pagingContext, paramList,"data_decorrenza",true);
            Query query = pagingQuery.Query;
            query.setParam("order", "data_decorrenza desc");
            string commandText = pagingQuery.Query.getSQL();
            logger.Debug(commandText);
            DataSet dataSet;
            if (this.ExecuteQuery(out dataSet, "DELEGHE", commandText))
            {
                foreach (DataRow dataRow in dataSet.Tables["DELEGHE"].Rows)
                {
                    InfoDelega infoDelega = buildDelega(dataRow);
                    deleghe.Add(infoDelega);
                }
                dataSet.Dispose();
            }
            else
            {
                logger.Debug("Errore nell'esecuzione della query in 'searchDeleghe'");
                throw new ApplicationException("Errore nell'esecuzione della query 'searchDeleghe'");
            }
            return deleghe;
        }

        public InfoDelega getDelegaById(string id)
        {
            logger.Debug("getDelegaById");
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DELEGHE_ASSEGNATE");
            queryDef.setParam("param1", "where system_id='"+id+"'");
            queryDef.setParam("order", "");
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);
            DataSet dataSet;
            if (this.ExecuteQuery(out dataSet, "DELEGHE", commandText)){
                if(dataSet.Tables["DELEGHE"].Rows.Count==0) return null;
                return buildDelega(dataSet.Tables["DELEGHE"].Rows[0]);
            }
            else
            {
                logger.Debug("Errore nell'esecuzione della query in 'getListaDeleghe'");
                throw new ApplicationException("Errore nell'esecuzione della query 'getListaDeleghe'");
            }
        }

        //Restituisce la lista delle deleghe, a seconda del parametro tipo, assegnate, attive, impostate 
        public ArrayList getListaDeleghe(DocsPaVO.utente.InfoUtente infoUtente, string tipoDelega, string statoDelega, string idAmm, out int numDeleghe)
        {
            ArrayList deleghe = new ArrayList();
            numDeleghe = 0;
            logger.Debug("getListaDeleghe");
            string queryFrom = string.Empty;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DELEGHE_ASSEGNATE");
            string param1 = DelegheStrategy.getStrategy(tipoDelega).getWhereClause(infoUtente, statoDelega);
            queryDef.setParam("param1", param1);
            string param2 = string.Empty;
            if (!string.IsNullOrEmpty(idAmm))
            {
                if (!string.IsNullOrEmpty(param1))
                    param2 = " AND ";
                else
                    param2 = " WHERE ";
                param2 += "id_uo_delegato in (select system_id from dpa_corr_globali where id_amm = " + idAmm + " and cha_tipo_urp = 'U') ";
            }
            queryDef.setParam("param2", param2);
            queryDef.setParam("order", " order by data_decorrenza desc");
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            DataSet dataSet;

            if (this.ExecuteQuery(out dataSet, "DELEGHE", commandText))
            {
                foreach (DataRow dataRow in dataSet.Tables["DELEGHE"].Rows)
                {
                    InfoDelega infoDelega = buildDelega(dataRow);
                    deleghe.Add(infoDelega);
                }
                numDeleghe = dataSet.Tables["DELEGHE"].Rows.Count;
                dataSet.Dispose();
            }
            else
            {
                logger.Debug("Errore nell'esecuzione della query in 'getListaDeleghe'");
                throw new ApplicationException("Errore nell'esecuzione della query 'getListaDeleghe'");
            }
           return deleghe;
        }

        private InfoDelega buildDelega(DataRow dataRow)
        {
            DocsPaVO.Deleghe.InfoDelega infoDelega = new DocsPaVO.Deleghe.InfoDelega();
            infoDelega.id_delega = dataRow["SYSTEM_ID"].ToString();
            infoDelega.id_utente_delegante = dataRow["ID_PEOPLE_DELEGANTE"].ToString();
            infoDelega.codiceDelegante = dataRow["COD_PEOPLE_DELEGANTE"].ToString();
            Utenti utente = new Utenti();
            DocsPaVO.utente.Utente ut;
            if (dataRow["ID_PEOPLE_DELEGATO"].ToString() != "0")
            {
                ut = utente.GetUtente(dataRow["ID_PEOPLE_DELEGATO"].ToString());
                if (ut == null)
                {
                    infoDelega.utDelegatoDismesso = "1";
                    ut = utente.GetUtenteNoFiltroDisabled(dataRow["ID_PEOPLE_DELEGATO"].ToString());
                }
                infoDelega.cod_utente_delegato = ut.descrizione;
            }
            if (dataRow["ID_PEOPLE_DELEGANTE"].ToString() != "0")
            {
                ut = utente.GetUtente(dataRow["ID_PEOPLE_DELEGANTE"].ToString());
                if (ut == null)
                {
                    infoDelega.utDeleganteDismesso = "1";
                    ut = utente.GetUtenteNoFiltroDisabled(dataRow["ID_PEOPLE_DELEGANTE"].ToString());
                }
                infoDelega.cod_utente_delegante = ut.descrizione;
            }
            infoDelega.id_ruolo_delegante = dataRow["ID_RUOLO_DELEGANTE"].ToString();
            infoDelega.cod_ruolo_delegante = dataRow["COD_RUOLO_DELEGANTE"].ToString();
            infoDelega.id_utente_delegato = dataRow["ID_PEOPLE_DELEGATO"].ToString();
            infoDelega.id_uo_delegato = dataRow["ID_UO_DELEGATO"].ToString();
            infoDelega.id_ruolo_delegato = dataRow["ID_RUOLO_DELEGATO"].ToString();
            infoDelega.cod_ruolo_delegato = dataRow["COD_RUOLO_DELEGATO"].ToString();
            infoDelega.dataDecorrenza = dataRow["DATA_DECORRENZA"].ToString();
            infoDelega.dataScadenza = dataRow["DATA_SCADENZA"].ToString();
            infoDelega.inEsercizio = dataRow["CHA_IN_ESERCIZIO"].ToString();
            infoDelega.id_people_corr_globali = utente.GetSystemIdCorrispondenteByIDPeople(infoDelega.id_utente_delegato);

            if (!string.IsNullOrEmpty(dataRow["DATA_SCADENZA"].ToString()))
                if (infoDelega.inEsercizio == "1" && Convert.ToDateTime(dataRow["DATA_SCADENZA"]) < Convert.ToDateTime(DateTime.Now))
                {
                    if (dismettiDelega(infoDelega))
                        infoDelega.inEsercizio = "0";
                }
            return infoDelega;
        }

        //Dismette l'esecizio di una data delega
        public bool dismettiDelega(DocsPaVO.Deleghe.InfoDelega delega)
        {
            bool result = true;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_MODIFICA_DELEGA");
                    q.setParam("param1", "cha_in_esercizio = '0'");
                    q.setParam("id_delega", delega.id_delega);
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                    else
                        dbProvider.CommitTransaction();
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }

        //Revoca delle delega ad un dato utente
        public bool revocaDelega(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Deleghe.InfoDelega[] listaDeleghe, out string msg)
        {
            msg = string.Empty;
            bool result = true;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    foreach (DocsPaVO.Deleghe.InfoDelega delega in listaDeleghe)
                    {
                        logger.Debug("Delega " + delega.id_delega + " dataInizio: " + delega.dataDecorrenza + " dataFine: " + delega.dataScadenza);
                        //Per tutte le deleghe attive il sistema imposta la data e l'ora di scadenza al momento della revoca
                        //data/ora di decorrenza minore della data/ora odierna
                        //data/ora di scadenza successiva alla dara/ora odierna
                        // Filippo: aggiunta condizione dataScadenza<dataDecorrenza per app mobile (che non gestisce dataScadenza=null)
                        if ((toDate(delega.dataDecorrenza) < DateTime.Now) &&
                                (string.IsNullOrEmpty(delega.dataScadenza) || toDate(delega.dataScadenza) > DateTime.Now || toDate(delega.dataScadenza) < toDate(delega.dataDecorrenza)))
                        {
                            //Se la delega è attiva ed in esercizio non può essere rimossa
                            //if (checkDelegaInEsercizio(delega))
                            //{
                                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_REVOCA_DELEGA");
                                q.setParam("id_delega", delega.id_delega);
                                queryString = q.getSQL();
                                logger.Debug(queryString);
                                if (!dbProvider.ExecuteNonQuery(queryString))
                                {
                                    result = false;
                                    dbProvider.RollbackTransaction();
                                    throw new Exception();
                                }
                            //}
                            //else
                            //{
                            //    result = false;
                            //    msg = msg + "Delega con utente delegato " + delega.cod_utente_delegato + " in esercizio. Impossibile revocarla.\\n";
                            //}

                        }
                        //Per tutte le deleghe impostate il sistema eliminerà completamente la delega come se questa non fosse mai esistita
                        if (toDate(delega.dataDecorrenza) > DateTime.Now)
                        {
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("DELETE_DELEGA");
                            q.setParam("id_delega", delega.id_delega);
                            queryString = q.getSQL();
                            logger.Debug(queryString);
                            if (!dbProvider.ExecuteNonQuery(queryString))
                            {
                                result = false;
                                dbProvider.RollbackTransaction();
                                throw new Exception();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }

        //Dismette l'esecizio di una data delega
        public bool dismettiDelega(DocsPaVO.utente.InfoUtente infoUtente, string userIdDelegante)
        {
            bool result = true;
            string queryString;
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DISMETTI_DELEGA");
                    //Error cancello l'esercizio della delega dando il delegato, ma in effetti passo il delegante --
                    //q.setParam("param1", "id_people_delegato = " + infoUtente.idPeople + " and cha_in_esercizio = '1'");

                    q.setParam("param1", String.Format("id_people_delegante = {0} and id_people_delegato = {1} and cha_in_esercizio = '1'",infoUtente.idPeople,infoUtente.delegato.idPeople));
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    if (dbProvider.ExecuteNonQuery(queryString))
                    {
                        //Aggiorna la dpa_login
                        this.deleteDelegante(userIdDelegante);
                    }
                    else
                        result = false;
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);

                throw e;
            }
            return result;
        }


        //Dismette l'esecizio di eventuali deleghe appese
        public bool DismettiDelegheInEsercizio(DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            string queryString;
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DISMETTI_DELEGA");

                    q.setParam("param1", String.Format("id_people_delegato = {0} and cha_in_esercizio = '1'", infoUtente.idPeople));
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                        throw new Exception();
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            return result;
        }

        //Elimina dalla dpa_login le credenziali del delegante
        public void deleteDelegante(string userIdDelegante)
        {
            //Elimina l'utente delegante dalla dpa_login per dismettere l'esercizio della delega
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_LOGIN_AMM");
            q.setParam("param1", userIdDelegante);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.ExecuteNonQuery(queryString);
        }

        //Verifica che nell'arco di tempo corrente non ci siano deleghe sovrapposte
        public bool verificaUnicaDelega(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Deleghe.InfoDelega delega)
        {
            bool result = false;
            try
            {
                logger.Debug("verificaUnicaDelega");
                string queryFrom = string.Empty;
                int delegheOK = 0;
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DELEGHE_ASSEGNATE");
                string param1 = "where id_people_delegante = " + infoUtente.idPeople;
                if (!string.IsNullOrEmpty(delega.id_delega))
                    param1 += " and system_id<>" + delega.id_delega;
                param1 += " and (data_scadenza is null or data_scadenza >= " + DocsPaDbManagement.Functions.Functions.GetDate() + ")";
                queryDef.setParam("param1", param1);
                queryDef.setParam("order", "");
                string commandText = queryDef.getSQL();
                logger.Debug(commandText);
                DataSet dataSet;
                using (DBProvider dbProvider = new DBProvider())
                {
                    if (dbProvider.ExecuteQuery(out dataSet, "DELEGHE", commandText))
                    {
                        if (dataSet.Tables["DELEGHE"].Rows.Count == 0)
                        {
                            result = true;
                        }
                        else
                        {
                            foreach (DataRow dataRow in dataSet.Tables["DELEGHE"].Rows)
                            {
                                if ((delega.id_ruolo_delegante == "0") || (dataRow["ID_RUOLO_DELEGANTE"].ToString()=="0") || (delega.id_ruolo_delegante == dataRow["ID_RUOLO_DELEGANTE"].ToString()))
                                {
                                    if (string.IsNullOrEmpty(dataRow["DATA_SCADENZA"].ToString()) && string.IsNullOrEmpty(delega.dataScadenza)) { }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(dataRow["DATA_SCADENZA"].ToString()) 
                                            && (Convert.ToDateTime(dataRow["DATA_SCADENZA"]) < DateTime.Now))
                                                delegheOK++;
                                        else
                                        {
                                            if ((Convert.ToDateTime(delega.dataDecorrenza) > Convert.ToDateTime(dataRow["DATA_DECORRENZA"]))
                                                && (!string.IsNullOrEmpty(dataRow["DATA_SCADENZA"].ToString())) 
                                                &&(Convert.ToDateTime(delega.dataDecorrenza) >= Convert.ToDateTime(dataRow["DATA_SCADENZA"])))
                                                    delegheOK++;
                                            
                                            if ((Convert.ToDateTime(delega.dataDecorrenza) < Convert.ToDateTime(dataRow["DATA_DECORRENZA"]))
                                                && (!string.IsNullOrEmpty(delega.dataScadenza))
                                                && (Convert.ToDateTime(delega.dataScadenza) < Convert.ToDateTime(dataRow["DATA_DECORRENZA"])))
                                                    delegheOK++;

                                            if (delega.id_delega == dataRow["SYSTEM_ID"].ToString())
                                                delegheOK++;
                                        }
                                    }
                                }
                                else
                                    delegheOK++;
                            }

                            if (delegheOK == dataSet.Tables["DELEGHE"].Rows.Count)
                                result = true;
                        }
                    }
                    else
                        result = false;
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }

        //Conta quante sono le deleghe attive 
        public int checkDelegaAttiva(DocsPaVO.utente.InfoUtente infoUtente)
        {
            int result = 0;
            try
            {
                logger.Debug("checkDelegaAttiva");
                string queryFrom = string.Empty;
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CONTA_DELEGHE_ATTIVE");
                queryDef.setParam("param1", "id_people_delegato = " + infoUtente.idPeople +
                                    " and data_decorrenza <= " + DocsPaDbManagement.Functions.Functions.GetDate() + " " +
                                     " and data_scadenza >= " + DocsPaDbManagement.Functions.Functions.GetDate() + " " +
                                    " and (data_scadenza >= " + DocsPaDbManagement.Functions.Functions.GetDate() + " or data_scadenza is null)"); // tolto caso data_scadenza is null per non mostrare deleghe permanenti in home alla nuova wa
                string commandText = queryDef.getSQL();
                logger.Debug(commandText);
                using (DBProvider dbProvider = new DBProvider())
                {
                    string outParam;
                    dbProvider.ExecuteScalar(out outParam, commandText);
                    result = Convert.ToInt32(outParam);
                }

            }
            catch (Exception e)
            {
                result = 0;
                logger.Debug(e.Message);
            }

            return result;
        }

        /// <summary>
        /// Metodo utilizzato per verificare se un utente ha delle deleghe attive e in esercizio o se 
        /// ha attivato delle deleghe con un dato ruolo e queste sono al momento esercitate
        /// </summary>
        /// <param name="idPeople">Id dell'utente di cui verificare le deleghe in esecizio</param>
        /// <param name="idRole">Id del ruolo</param>
        /// <returns>True se l'utente ha almeno una delega attiva e la sta esercitando o se con il ruolo indicato ha attivato delle deleghe che al momento sono esercitate</returns>
        public bool CheckIfUserHaveDelegheInEsercizio(String idPeople, String idRole)
        {
            bool result = false;
            try
            {
                logger.Debug("CheckIfUserHaveDelegheInEsercizio");
                
                using (DBProvider dbProvider = new DBProvider())
                {
                    Query queryDef = InitQuery.getInstance().getQuery("COUNT_DELEGHE_ATTIVE_E_IN_ESERCIZIO_UTENTE");
                    queryDef.setParam("idPeople", idPeople);
                    queryDef.setParam("idRole", idRole);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);
                
                    string outParam;
                    dbProvider.ExecuteScalar(out outParam, commandText);
                    
                    // Se il risultato della query è diverso da 0, ci sono deleghe esercitate
                    if (!outParam.Equals("0"))
                        result = true;
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }

            return result;
        }

        //Verifica se una data delega è o meno in esercizio
        //Restituisce true se la delega non è in esercizio, false altrimenti
        public bool checkDelegaInEsercizio(DocsPaVO.Deleghe.InfoDelega infoDelega)
        {
            bool result = false;
            try
            {
                logger.Debug("checkDelegaInEsercizio");
                string queryFrom = string.Empty;
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("DELEGHE_IN_ESERCIZIO");
                queryDef.setParam("param1", "system_id = " + infoDelega.id_delega);
                string commandText = queryDef.getSQL();
                logger.Debug(commandText);
                using (DBProvider dbProvider = new DBProvider())
                {
                    string outParam;
                    dbProvider.ExecuteScalar(out outParam, commandText);
                    //Delega non in esercizio
                    if (outParam.Equals("0"))
                        result = true;
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }

        #region deleghe in amministrazione
        public bool verificaUnicaDelegaAmm(DocsPaVO.Deleghe.InfoDelega delega)
        {
            bool result = false;
            try
            {
                logger.Debug("verificaUnicaDelegaAmm");
                string queryFrom = string.Empty;
                int delegheOK = 0;
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DELEGHE_ASSEGNATE");
                queryDef.setParam("param1", "where id_people_delegante = " + delega.id_utente_delegante +
                    " and data_decorrenza <= " + DocsPaDbManagement.Functions.Functions.GetDate() + " " + " and (data_scadenza is null or data_scadenza >= " + DocsPaDbManagement.Functions.Functions.GetDate() + ")");
                queryDef.setParam("order", "");
                string commandText = queryDef.getSQL();
                logger.Debug(commandText);
                DataSet dataSet;
                if (this.ExecuteQuery(out dataSet, "DELEGHE", commandText))
                {
                    if (dataSet.Tables["DELEGHE"].Rows.Count == 0)
                    {
                        result = true;
                    }
                    else
                    {
                        foreach (DataRow dataRow in dataSet.Tables["DELEGHE"].Rows)
                        {
                            if ((delega.id_ruolo_delegante == "0") || (delega.id_ruolo_delegante == dataRow["ID_RUOLO_DELEGANTE"].ToString()))
                            {
                                if ((toDate(delega.dataDecorrenza) > Convert.ToDateTime(dataRow["DATA_DECORRENZA"]))
                                    && (toDate(delega.dataDecorrenza) > Convert.ToDateTime(dataRow["DATA_SCADENZA"])))
                                    delegheOK++;

                                if ((toDate(delega.dataDecorrenza) < Convert.ToDateTime(dataRow["DATA_DECORRENZA"]))
                                    && (!string.IsNullOrEmpty(delega.dataScadenza) && (toDate(delega.dataScadenza) < Convert.ToDateTime(dataRow["DATA_DECORRENZA"])))
                                    )
                                    delegheOK++;
                            }
                            else
                                delegheOK++;
                        }

                        if (delegheOK == dataSet.Tables["DELEGHE"].Rows.Count)
                            result = true;

                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }

            return result;
        }

        public bool modificaDelegaAmm(DocsPaVO.Deleghe.InfoDelega delegaOld, DocsPaVO.Deleghe.InfoDelega delegaNew, string tipoDelega, string dataScadenzaOld, string dataDecorrenzaOld)
        {
            bool result = true;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //caso in cui è stato modificato il delegato o il delegante o il suo ruolo
                    if (!delegaNew.id_utente_delegato.Equals(delegaOld.id_utente_delegato) ||
                        !delegaNew.id_utente_delegante.Equals(delegaOld.id_utente_delegante) ||
                        !delegaNew.id_ruolo_delegante.Equals(delegaOld.id_ruolo_delegante))
                    {
                        //Nel caso di delega attiva e non in esercizio, la modifica del delegato comporterà la modifica della
                        //delega sulla data di scadenza che verrà impostata alla data/ora di esecuzione dell'operazione
                        //e la creazione di una nuova delega che avrà il nuovo delegato selezionato
                        if (tipoDelega.Equals("A"))
                        {
                            //verifico se la delega è in esercizio
                            //se in esercizio non si può effettuare la modifica
                            if (checkDelegaInEsercizio(delegaOld))
                            {
                                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_REVOCA_DELEGA");
                                q.setParam("id_delega", delegaOld.id_delega);
                                queryString = q.getSQL();
                                logger.Debug(queryString);
                                if (!dbProvider.ExecuteNonQuery(queryString))
                                {
                                    result = false;
                                    dbProvider.RollbackTransaction();
                                    throw new Exception();
                                }
                                result = creaNuovaDelega(delegaNew);
                            }
                            else
                                result = false;
                        }
                        else
                        {
                            //delega impostata... se si cambia l'utente elimino la vecchia e creo la nuova
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("DELETE_DELEGA");
                            q.setParam("id_delega", delegaOld.id_delega);
                            queryString = q.getSQL();
                            logger.Debug(queryString);
                            if (!dbProvider.ExecuteNonQuery(queryString))
                            {
                                result = false;
                                dbProvider.RollbackTransaction();
                                throw new Exception();
                            }
                            result = creaNuovaDelega(delegaNew);
                        }
                    }
                    else
                    {
                        //Modifica di delega con data decorrenza uguale ma con data di scadenza maggiore, ovvero allungato intervallo di tempo
                        if (((!string.IsNullOrEmpty(dataScadenzaOld) && Convert.ToDateTime(dataScadenzaOld) < DateTime.Now)
                            && (string.IsNullOrEmpty(delegaNew.dataScadenza) || toDate(delegaNew.dataScadenza) >= DateTime.Now)
                            && (Convert.ToDateTime(dataDecorrenzaOld).ToShortDateString() != toDate(delegaNew.dataDecorrenza).ToShortDateString())))
                        {
                            if (verificaUnicaDelegaAmm(delegaNew))
                                creaNuovaDelega(delegaNew);
                            else
                                result = false;
                        }
                        else
                        {
                            //caso in cui è stata modificata solo la data di decorrenza o la data di scadenza
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("U_MODIFICA_DELEGA");
                            q.setParam("id_delega", delegaOld.id_delega);
                            q.setParam("param1", "DATA_DECORRENZA = " + DocsPaDbManagement.Functions.Functions.ToDate(delegaNew.dataDecorrenza) + ", DATA_SCADENZA = " + DocsPaDbManagement.Functions.Functions.ToDate(delegaNew.dataScadenza));

                            queryString = q.getSQL();
                            logger.Debug(queryString);
                            if (!dbProvider.ExecuteNonQuery(queryString))
                            {
                                result = false;
                                dbProvider.RollbackTransaction();
                                throw new Exception();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }

            return result;
        }
        #endregion

        private DateTime toDate(string date)
        {
            logger.Debug("toDate: " + date);
            string[] formats = {"dd/MM/yyyy",
                                "dd/MM/yyyy HH:mm:ss",
								"dd/MM/yyyy h:mm:ss",
								"dd/MM/yyyy h.mm.ss",
								"dd/MM/yyyy HH.mm.ss"};
            return DateTime.ParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
        }
    }

    public abstract class DelegheStrategy
    {
        private static Dictionary<string, DelegheStrategy> _delegheStrategies;

        static DelegheStrategy()
        {
            _delegheStrategies = new Dictionary<string, DelegheStrategy>();
            _delegheStrategies.Add("assegnate", new AssegnateDelegheStrategy());
            _delegheStrategies.Add("ricevute", new RicevuteDelegheStrategy());
            _delegheStrategies.Add("esercizio", new EsercizioDelegheStrategy());
            _delegheStrategies.Add("tutte", new TutteDelegheStrategy());
        }

        public static DelegheStrategy getStrategy(string tipoDelega)
        {
            return _delegheStrategies[tipoDelega];
        }

        public string getWhereClause(DocsPaVO.utente.InfoUtente infoUtente, string statoDelega)
        {
            SearchDelegaInfo searchInfo = new SearchDelegaInfo();
            searchInfo.StatoDelega = statoDelega;
            return getWhereClause(infoUtente, searchInfo);
        }

        public abstract string getWhereClause(DocsPaVO.utente.InfoUtente infoUtente, SearchDelegaInfo searchInfo);

    }

    public class AssegnateDelegheStrategy : DelegheStrategy
    {
        private string getAssegnateClause(DocsPaVO.utente.InfoUtente infoUtente, SearchDelegaInfo searchInfo)
        {
            string res = "";
            if (!string.IsNullOrEmpty(searchInfo.IdRuoloDelegante))
            {
                res = res + " and id_ruolo_delegante = " + searchInfo.IdRuoloDelegante;
            }
            return res;
        }

        public override string getWhereClause(DocsPaVO.utente.InfoUtente infoUtente, SearchDelegaInfo searchInfo)
        {
            string statoDelega = searchInfo.StatoDelega;
            string res = "";
            if (statoDelega.Equals("T"))
                res="where id_people_delegante = " + infoUtente.idPeople;
            if (statoDelega.Equals("A"))
                res="where id_people_delegante = " + infoUtente.idPeople + " and data_decorrenza <= " + DocsPaDbManagement.Functions.Functions.GetDate() + " and (data_scadenza is null or data_scadenza > " + DocsPaDbManagement.Functions.Functions.GetDate() + ")";
            if (statoDelega.Equals("I"))
                res="where id_people_delegante = " + infoUtente.idPeople + " and data_decorrenza > " + DocsPaDbManagement.Functions.Functions.GetDate();
            if (statoDelega.Equals("S"))
                res="where id_people_delegante = " + infoUtente.idPeople + " and data_scadenza < " + DocsPaDbManagement.Functions.Functions.GetDate();
            res = res + getAssegnateClause(infoUtente, searchInfo);
            return res;
        }
    }

    public class RicevuteDelegheStrategy : DelegheStrategy
    {
        public override string getWhereClause(DocsPaVO.utente.InfoUtente infoUtente, SearchDelegaInfo searchInfo)
        {
            string statoDelega = searchInfo.StatoDelega;
            if (statoDelega.Equals("A"))
                return "where id_people_delegato = " + infoUtente.idPeople + " " +
                            " and data_decorrenza <= " + DocsPaDbManagement.Functions.Functions.GetDate() + " " +
                            " and (data_scadenza is null or data_scadenza > " + DocsPaDbManagement.Functions.Functions.GetDate() + ") ";
            if (statoDelega.Equals("I"))
                return "where id_people_delegato = " + infoUtente.idPeople +
                            " and data_decorrenza > " + DocsPaDbManagement.Functions.Functions.GetDate() + " " +
                            " and (data_scadenza is null or data_scadenza >= " + DocsPaDbManagement.Functions.Functions.GetDate() + ") ";
            if (statoDelega.Equals("S"))
                return "where id_people_delegato = " + infoUtente.idPeople +
                            " and data_scadenza < " + DocsPaDbManagement.Functions.Functions.GetDate();
            if (statoDelega.Equals("T"))
                return "where id_people_delegato = " + infoUtente.idPeople;
            return null;
        }

    }

    public class EsercizioDelegheStrategy : DelegheStrategy
    {
        public override string getWhereClause(DocsPaVO.utente.InfoUtente infoUtente, SearchDelegaInfo searchInfo)
        {
            return "where id_people_delegato = " + infoUtente.delegato.idPeople +
                                      " and data_decorrenza <= " + DocsPaDbManagement.Functions.Functions.GetDate() + " " +
                                      "and (data_scadenza is null or data_scadenza >= " + DocsPaDbManagement.Functions.Functions.GetDate() + ") " +
                                      "and cha_in_esercizio='1'";
        }
    }

    public class TutteDelegheStrategy : DelegheStrategy
    {
        public override string getWhereClause(DocsPaVO.utente.InfoUtente infoUtente, SearchDelegaInfo searchInfo)
        {
            string statoDelega = searchInfo.StatoDelega;
            if (statoDelega.Equals("T"))
                return "";
            if (statoDelega.Equals("A"))
                return "where data_decorrenza <= " + DocsPaDbManagement.Functions.Functions.GetDate() + " and (data_scadenza is null or data_scadenza > " + DocsPaDbManagement.Functions.Functions.GetDate() + ")";
            if (statoDelega.Equals("I"))
                return "where data_decorrenza > " + DocsPaDbManagement.Functions.Functions.GetDate();
            if (statoDelega.Equals("S"))
                return "where data_scadenza < " + DocsPaDbManagement.Functions.Functions.GetDate();
            return null;
        }

    }
}
