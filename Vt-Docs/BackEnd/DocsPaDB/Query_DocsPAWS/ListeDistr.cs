using System;
using System.Collections;
using System.Data;
using System.Threading;
using log4net;
using DocsPaUtils;

namespace DocsPaDB.Query_DocsPAWS
{
    public class ListeDistr : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(ListeDistr));
        //private Mutex semaforo = new Mutex();

        public ListeDistr() { }

        public bool isUniqueCod(string codice, string idAmm)
        {
            try 
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //semaforo.WaitOne();
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_IS_UNIQUE_COD");
                    queryMng.setParam("param1", codice.ToUpper());
                    queryMng.setParam("param2", idAmm);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - isUniqueCod - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - isUniqueCod - ListeDistr.cs - QUERY : " + commandText);

                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);

                    if (ds.Tables[0].Rows.Count != 0)
                        return false;
                    else
                        return true;


                }
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - isUniqueCod - ListeDistr.cs - QUERY " + ex.Message);
                return false;
            }
        }

        //OK
        public bool isUniqueCodLista(string codLista, string idAmm)
        {
            //DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //semaforo.WaitOne();
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_IS_UNIQUE_COD_LISTA");
                    queryMng.setParam("param1", codLista.ToUpper());
                    queryMng.setParam("param2", idAmm);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - isUniqueCodLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - isUniqueCodLista - ListeDistr.cs - QUERY : " + commandText);

                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);

                    if (ds.Tables[0].Rows.Count != 0)
                        return false;
                    else
                        return true;


                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return false;
            }
            //catch 
            //{
            //    return false;
            //}
            //finally
            //{
            //    dbProvider.Dispose();
            //    //semaforo.ReleaseMutex();
            //}			
        }


        //OK
        public bool isUniqueNomeLista(string nomeLista, string idAmm)
        {
            //DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //semaforo.WaitOne();
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_IS_UNIQUE_NOME_LISTA");
                    queryMng.setParam("param1", nomeLista.ToUpper());
                    queryMng.setParam("param2", idAmm);

                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - isUniqueNomeLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - isUniqueNomeLista - ListeDistr.cs - QUERY : " + commandText);

                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);

                    if (ds.Tables[0].Rows.Count != 0)
                        return false;
                    else
                        return true;
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return false;
            }
        }


        //OK
        public string getCodiceLista(string idLista)
        {
            //DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_GET_CODICE_LISTA");
                    queryMng.setParam("param1", idLista);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getCodiceLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getCodiceLista - ListeDistr.cs - QUERY : " + commandText);

                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);

                    return ds.Tables[0].Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return null;
            }
            //catch 
            //{
            //    return null;
            //}
            //finally
            //{
            //    dbProvider.Dispose();
            //    //semaforo.ReleaseMutex();
            //}					
        }

        //OK
        public string getRuoloOrUserLista(string idLista)
        {
            // DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_GET_RUOLO_LISTA");
                    queryMng.setParam("param1", idLista);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getRuoloOrUserLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getRuoloOrUserLista - ListeDistr.cs - QUERY : " + commandText);

                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    if (ds.Tables[0].Rows[0][0].ToString() != "")
                    {
                        return ds.Tables[0].Rows[0][0].ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return null;
            }
            //catch
            //{
            //    return null;
            //}
            //finally
            //{
            //    dbProvider.Dispose();
            //    //semaforo.ReleaseMutex();
            //}
        }

        //OK
        public ArrayList getCorrispondentiByCodLista(string codiceLista, string idAmm)
        {
            ArrayList corr = new ArrayList();
            //DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_GET_CORRISPONDENTI_BY_COD_LISTA");
                    queryMng.setParam("param1", codiceLista.ToUpper().Replace("'", "''"));
                    if (idAmm != "")
                    {
                        queryMng.setParam("param2", "AND L.ID_AMM=" + idAmm);
                    }
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getCorrispondentiByCodLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getCorrispondentiByCodLista - ListeDistr.cs - QUERY : " + commandText);

                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);

                    DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.utente.Corrispondente c = u.GetCorrispondenteBySystemID(ds.Tables[0].Rows[i][0].ToString());
                        if (c != null)
                        {
                            corr.Add(c);
                        }
                        //if(c.tipoIE == "I")
                        //{
                        //    corr.Add(u.GetCorrispondenteByCodRubrica(ds.Tables[0].Rows[i][1].ToString(),c.codiceRubrica,DocsPaVO.addressbook.TipoUtente.INTERNO));
                        //}
                        //if(c.tipoIE == "E")
                        //{
                        //    corr.Add(u.GetCorrispondenteByCodRubrica(ds.Tables[0].Rows[i][1].ToString(),c.codiceRubrica,DocsPaVO.addressbook.TipoUtente.ESTERNO));						
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
            //catch 
            //{
            //    return null;
            //}
            //finally
            //{
            //    dbProvider.Dispose();
            //    //semaforo.ReleaseMutex();
            //}					

            return corr;
        }

        /// <summary>
        /// Restituisce i corrispondenti della lista. La lista deve essere visibile all'utente in input
        /// </summary>
        /// <param name="codiceLista"></param>
        /// <param name="idAmm"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public ArrayList getCorrispondentiByCodListaByUtente(string codiceLista, string idAmm, DocsPaVO.utente.InfoUtente infoUtente)
        {
            ArrayList corr = new ArrayList();

            try
            {
                //semaforo.WaitOne();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_GET_CORRISPONDENTI_BY_COD_LISTA_BY_UTENTE");
                    queryMng.setParam("param1", codiceLista.ToUpper().Replace("'", "''"));
                    queryMng.setParam("idPeople", infoUtente.idPeople);
                    queryMng.setParam("idGruppo", infoUtente.idGruppo);
                    if (idAmm != "")
                    {
                        queryMng.setParam("param2", "AND L.ID_AMM=" + idAmm);
                    }
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getCorrispondentiByCodLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getCorrispondentiByCodLista - ListeDistr.cs - QUERY : " + commandText);

                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);

                    DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.utente.Corrispondente c = u.GetCorrispondenteBySystemID(ds.Tables[0].Rows[i][0].ToString());
                        if (c != null)
                        {
                            corr.Add(c);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
            return corr;
        }

        public ArrayList getCorrispondentiByDescLista(string descLista)
        {
            ArrayList corr = new ArrayList();
            //  DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_GET_CORRISPONDENTI_BY_DESC_LISTA");
                    queryMng.setParam("param1", descLista.ToUpper());
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getCorrispondentiByDescLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getCorrispondentiByDescLista - ListeDistr.cs - QUERY : " + commandText);

                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);

                    DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.utente.Corrispondente c = u.GetCorrispondenteBySystemID(ds.Tables[0].Rows[i][0].ToString());
                        if (c.tipoIE == "I")
                        {
                            corr.Add(u.GetCorrispondenteByCodRubrica(ds.Tables[0].Rows[i][1].ToString(), c.codiceRubrica, DocsPaVO.addressbook.TipoUtente.INTERNO));
                        }
                        if (c.tipoIE == "E")
                        {
                            corr.Add(u.GetCorrispondenteByCodRubrica(ds.Tables[0].Rows[i][1].ToString(), c.codiceRubrica, DocsPaVO.addressbook.TipoUtente.ESTERNO));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return null;
            }
            //catch
            //{
            //    return null;
            //}
            //finally
            //{
            //    dbProvider.Dispose();
            //    //semaforo.ReleaseMutex();
            //}

            return corr;
        }

        //OK
        public string getNomeLista(string codiceLista, string idAmm)
        {
            //DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {

                    string CondAmm = "";
                    //semaforo.WaitOne();
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_GET_NOME_LISTA");
                    queryMng.setParam("param1", codiceLista.ToUpper().Replace("'", "''"));
                    if (idAmm != "")
                    {
                        CondAmm = " AND ID_AMM=" + idAmm;
                        queryMng.setParam("param2", CondAmm);
                    }
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getNomeLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getNomeLista - ListeDistr.cs - QUERY : " + commandText);

                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    return ds.Tables[0].Rows[0][0].ToString();
                }


            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return null;
            }
        }

        //OK
        public string getSystemIdLista(string codiceLista)
        {
            // DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //semaforo.WaitOne();
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_GET_SYSTEM_ID_LISTA");
                    queryMng.setParam("param1", codiceLista.ToUpper());
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getSystemIdLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getSystemIdLista - ListeDistr.cs - QUERY : " + commandText);

                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    return ds.Tables[0].Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return null;
            }
            //catch
            //{
            //    return null;
            //}
            //finally
            //{
            //    dbProvider.Dispose();
            //    //semaforo.ReleaseMutex();
            //}
        }

        //OK
        public DataSet getListePerModificaUt(string idUtente)
        {
            // DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_GET_LISTE_PER_MODIFICA_UT");
                    queryMng.setParam("param1", idUtente);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getListePerModificaUt - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getListePerModificaUt - ListeDistr.cs - QUERY : " + commandText);

                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return null;
            }
        }

        //OK
        public DataSet getListePerRuoloUt(string idUtente, string idRuolo)
        {
            //  DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_GET_LISTE_PER_RUOLO_UT");
                    queryMng.setParam("param1", idUtente);
                    queryMng.setParam("param2", idRuolo);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getListePerModificaUt - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getListePerModificaUt - ListeDistr.cs - QUERY : " + commandText);

                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return null;
            }
        }


        //OK
        public DataSet getListe(string idUtente, string idAmm)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_GET_LISTE_1");
                queryMng.setParam("param1", idUtente);
                queryMng.setParam("param2", idAmm);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getListe - ListeDistr.cs - QUERY : " + commandText);
                logger.Debug("SQL - getListe - ListeDistr.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                return ds;
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
                //semaforo.ReleaseMutex();
            }
        }


        public DataSet isCorrInListaDistr(string idCorr)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_IS_CORR_IN_LISTA_DISTR");
                queryMng.setParam("param1", idCorr);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getListe - isCorrInListeDistr.cs - QUERY : " + commandText);
                logger.Debug("SQL - getListe - isCorrInListeDistr.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                return ds;
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
                //semaforo.ReleaseMutex();
            }
        }

        //OK
        public DataSet getListe(string idUtente, string idAmm, string descrizione)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_GET_LISTE_3");
                if (idUtente == null)
                {
                    queryMng.setParam("param1", "(");
                }
                else
                {
                    string param1 = "((ID_PEOPLE =" + idUtente + "AND ID_AMM =" + idAmm + ") OR";
                    queryMng.setParam("param1", param1);

                }
                queryMng.setParam("param2", idAmm);
                descrizione = descrizione.Replace("'", "''");
                queryMng.setParam("param3", descrizione.ToUpper());
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getListe - ListeDistr.cs - QUERY : " + commandText);
                logger.Debug("SQL - getListe - ListeDistr.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                return ds;
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
                //semaforo.ReleaseMutex();
            }
        }


        //OK
        public DataSet getListe(string idAmm)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_GET_LISTE_2");
                queryMng.setParam("param1", idAmm);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getListe - ListeDistr.cs - QUERY : " + commandText);
                logger.Debug("SQL - getListe - ListeDistr.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                return ds;
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
                //semaforo.ReleaseMutex();
            }
        }


        //OK
        public void deleteLista(string codiceLista)
        {
            // DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {

                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_MODIFICA_LISTA_1");
                    queryMng.setParam("param1", codiceLista);
                    string commandText = queryMng.getSQL();
                    bool retValue = false;
                    int rowsAffected;
                    System.Diagnostics.Debug.WriteLine("SQL - deleteLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - deleteLista - ListeDistr.cs - QUERY : " + commandText);

                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue = (rowsAffected > 0);

                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_DELETE_LISTA");
                    queryMng.setParam("param1", codiceLista);
                    dbProvider.BeginTransaction();
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - deleteLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - deleteLista - ListeDistr.cs - QUERY : " + commandText);

                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue = (rowsAffected > 0);

                    if (retValue)
                        dbProvider.CommitTransaction();
                    else
                        dbProvider.RollbackTransaction();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }


        //OK
        public DataSet getCorrispondentiLista(string codiceLista)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_GET_CORRISPONDENTI_LISTA");
                queryMng.setParam("param1", codiceLista);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getCorrispondentiLista - ListeDistr.cs - QUERY : " + commandText);
                logger.Debug("SQL - getCorrispondentiLista - ListeDistr.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                return ds;
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
                //semaforo.ReleaseMutex();
            }
        }


        //OK
        public void deleteCorrLista(string codiceCorr)
        {
            //  DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                // semaforo.WaitOne();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_DELETE_CORR_LISTA");
                    queryMng.setParam("param1", codiceCorr);
                    bool retValue = false;
                    //  dbProvider.BeginTransaction();
                    int rowsAffected;
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - deleteCorrLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - deleteCorrLista - ListeDistr.cs - QUERY : " + commandText);

                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue = (rowsAffected > 0);

                    if (retValue)
                        dbProvider.CommitTransaction();
                    else
                        dbProvider.RollbackTransaction();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }


        //OK
        public DataSet ricercaCorrLista(string p_ricercaDescrizione)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_RICERCA_CORR_LISTA");
                queryMng.setParam("param1", p_ricercaDescrizione);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - ricercaCorrLista - ListeDistr.cs - QUERY : " + commandText);
                logger.Debug("SQL - ricercaCorrLista - ListeDistr.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                return ds;
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
                //semaforo.ReleaseMutex();
            }
        }


        //OK
        public int salvaListaGruppo(DataSet dsCorrLista, string nomeLista, string codiceLista, string idUtente, string idAmm, string gruppo)
        {
            // DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            int result = 0;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    // semaforo.WaitOne();
                    DocsPaUtils.Query queryMng;
                    if (idUtente == null)
                    {
                        if (gruppo.Equals("yes"))
                        {
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_SALVA_DPA_ELENCO_LISTE_GRUPPO_1");
                        }
                        else
                        {
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_SALVA_DPA_ELENCO_LISTE_1");
                        }
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(""));

                        queryMng.setParam("param1", nomeLista);
                        queryMng.setParam("param2", idAmm);
                        queryMng.setParam("param3", codiceLista);
                    }
                    else
                    {
                        if (gruppo.Equals("yes"))
                        {
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_SALVA_DPA_ELENCO_LISTE_GRUPPO_2");
                        }
                        else
                        {
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_SALVA_DPA_ELENCO_LISTE_2");
                        }
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(""));

                        queryMng.setParam("param1", nomeLista);
                        queryMng.setParam("param2", idUtente);
                        queryMng.setParam("param3", idAmm);
                        queryMng.setParam("param4", codiceLista);
                    }
                    string commandText = queryMng.getSQL();
                    string idLista = string.Empty;
                    System.Diagnostics.Debug.WriteLine("SQL - salvaLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaLista - ListeDistr.cs - QUERY : " + commandText);

                    bool retValue = false;
                    //  dbProvider.BeginTransaction();
                    int rowsAffected;
                    dbProvider.BeginTransaction();
                    if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    {
                        retValue = (rowsAffected > 0);

                        if (retValue)
                        {
                            // Reperimento systemid
                            commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_CORR_GLOBALI");
                            dbProvider.ExecuteScalar(out idLista, commandText);
                        }
                    }
                    for (int i = 0; i < dsCorrLista.Tables[0].Rows.Count; i++)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_SALVA_DPA_LISTE_DISTR");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_LISTE_DISTR"));
                        queryMng.setParam("param1", idLista);
                        queryMng.setParam("param2", dsCorrLista.Tables[0].Rows[i][0].ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaLista - ListeDistr.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaLista - ListeDistr.cs - QUERY : " + commandText);

                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                        retValue = (rowsAffected > 0);
                        result = Convert.ToInt32(idLista);
                    }
                    dbProvider.CommitTransaction();
                    //if (retValue)
                    //    dbProvider.CommitTransaction();
                    //else
                    //    dbProvider.RollbackTransaction();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
            return result;
        }

        //OK
        public void salvaLista(DataSet dsCorrLista, string nomeLista, string codiceLista, string idUtente, string idAmm)
        {
            //  DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                // semaforo.WaitOne();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {

                    DocsPaUtils.Query queryMng;
                    if (idUtente == null)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_SALVA_DPA_ELENCO_LISTE_1");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(""));

                        queryMng.setParam("param1", nomeLista);
                        queryMng.setParam("param2", idAmm);
                        queryMng.setParam("param3", codiceLista);
                    }
                    else
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_SALVA_DPA_ELENCO_LISTE_2");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(""));

                        queryMng.setParam("param1", nomeLista);
                        queryMng.setParam("param2", idUtente);
                        queryMng.setParam("param3", idAmm);
                        queryMng.setParam("param4", codiceLista);
                    }
                    string commandText = queryMng.getSQL();
                    string idLista = string.Empty;
                    System.Diagnostics.Debug.WriteLine("SQL - salvaLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaLista - ListeDistr.cs - QUERY : " + commandText);

                    bool retValue = false;
                    //dbProvider.BeginTransaction();
                    int rowsAffected;
                    if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    {
                        retValue = (rowsAffected > 0);

                        if (retValue)
                        {
                            // Reperimento systemid
                            commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("");
                            dbProvider.ExecuteScalar(out idLista, commandText);
                        }
                    }
                    for (int i = 0; i < dsCorrLista.Tables[0].Rows.Count; i++)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_SALVA_DPA_LISTE_DISTR");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_LISTE_DISTR"));
                        queryMng.setParam("param1", idLista);
                        queryMng.setParam("param2", dsCorrLista.Tables[0].Rows[i][0].ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaLista - ListeDistr.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaLista - ListeDistr.cs - QUERY : " + commandText);

                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                        retValue = (rowsAffected > 0);

                        if (retValue)
                            dbProvider.CommitTransaction();
                        else
                            dbProvider.RollbackTransaction();
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }

        //OK
        public void modificaLista(DataSet dsCorrLista, string idLista, string nomeLista, string codiceLista)
        {
            //DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //semaforo.WaitOne();
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_MODIFICA_LISTA_3");
                    queryMng.setParam("param1", codiceLista);
                    queryMng.setParam("param2", nomeLista);
                    queryMng.setParam("param3", idLista);

                    bool retValue = false;
                    //dbProvider.BeginTransaction();
                    int rowsAffected;
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);

                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue = (rowsAffected > 0);

                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_MODIFICA_LISTA_1");
                    queryMng.setParam("param1", idLista);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);

                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue = (rowsAffected > 0);

                    for (int i = 0; i < dsCorrLista.Tables[0].Rows.Count; i++)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_MODIFICA_LISTA_2");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_LISTE_DISTR"));
                        queryMng.setParam("param1", idLista);
                        queryMng.setParam("param2", dsCorrLista.Tables[0].Rows[i][0].ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);
                        logger.Debug("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);

                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                        retValue = (rowsAffected > 0);

                        if (retValue)
                            dbProvider.CommitTransaction();
                        else
                            dbProvider.RollbackTransaction();
                    }
                }


            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
            //finally
            //{
            //    dbProvider.Dispose();
            //    semaforo.ReleaseMutex();
            //}					
        }

        //OK
        public void modificaListaUser(DataSet dsCorrLista, string idLista, string nomeLista, string codiceLista, string idUtente)
        {
            //DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_MODIFICA_LISTA_5");
                    queryMng.setParam("param1", codiceLista);
                    queryMng.setParam("param2", nomeLista);
                    queryMng.setParam("param3", idLista);
                    queryMng.setParam("param4", idUtente);
                    //queryMng.setParam("param5", "");

                    bool retValue = false;
                    //dbProvider.BeginTransaction();
                    int rowsAffected;
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);

                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue = (rowsAffected > 0);

                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_MODIFICA_LISTA_1");
                    queryMng.setParam("param1", idLista);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);

                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue = (rowsAffected > 0);

                    for (int i = 0; i < dsCorrLista.Tables[0].Rows.Count; i++)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_MODIFICA_LISTA_2");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_LISTE_DISTR"));
                        queryMng.setParam("param1", idLista);
                        queryMng.setParam("param2", dsCorrLista.Tables[0].Rows[i][0].ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);
                        logger.Debug("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);

                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                        retValue = (rowsAffected > 0);

                        if (retValue)
                            dbProvider.CommitTransaction();
                        else
                            dbProvider.RollbackTransaction();
                    }
                }


            }
            //catch
            //{
            //    dbProvider.RollbackTransaction();
            //}
            //finally
            //{
            //    dbProvider.Dispose();
            //    semaforo.ReleaseMutex();
            //}
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }

        //OK
        public void modificaListaGruppo(DataSet dsCorrLista, string idLista, string nomeLista, string codiceLista, string idGruppo)
        {
            //DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_MODIFICA_LISTA_4");
                    queryMng.setParam("param1", codiceLista);
                    queryMng.setParam("param2", nomeLista);
                    queryMng.setParam("param3", idLista);
                    queryMng.setParam("param4", idGruppo);
                    //queryMng.setParam("param5", idGruppo);

                    bool retValue = false;
                    //dbProvider.BeginTransaction();
                    int rowsAffected;
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);

                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue = (rowsAffected > 0);

                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_MODIFICA_LISTA_1");
                    queryMng.setParam("param1", idLista);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);

                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue = (rowsAffected > 0);

                    for (int i = 0; i < dsCorrLista.Tables[0].Rows.Count; i++)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_MODIFICA_LISTA_2");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_LISTE_DISTR"));
                        queryMng.setParam("param1", idLista);
                        queryMng.setParam("param2", dsCorrLista.Tables[0].Rows[i][0].ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);
                        logger.Debug("SQL - modificaLista - ListeDistr.cs - QUERY : " + commandText);

                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                        retValue = (rowsAffected > 0);

                        if (retValue)
                            dbProvider.CommitTransaction();
                        else
                            dbProvider.RollbackTransaction();
                    }
                }


            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }


        //OK
        public void modificaListaCorr(DataSet dsCorrLista, string idLista)
        {
            // DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                // semaforo.WaitOne();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_MODIFICA_LISTA_1");
                    queryMng.setParam("param1", idLista);
                    bool retValue = false;
                    // dbProvider.BeginTransaction();
                    int rowsAffected;
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - modificaListaCorr - ListeDistr.cs - QUERY : " + commandText);
                    logger.Debug("SQL - modificaListaCorr - ListeDistr.cs - QUERY : " + commandText);

                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue = (rowsAffected > 0);

                    for (int i = 0; i < dsCorrLista.Tables[0].Rows.Count; i++)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_MODIFICA_LISTA_2");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_LISTE_DISTR"));
                        queryMng.setParam("param1", idLista);
                        queryMng.setParam("param2", dsCorrLista.Tables[0].Rows[i][0].ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - modificaListaCorr - ListeDistr.cs - QUERY : " + commandText);
                        logger.Debug("SQL - modificaListaCorr - ListeDistr.cs - QUERY : " + commandText);

                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                        retValue = (rowsAffected > 0);

                        if (retValue)
                            dbProvider.CommitTransaction();
                        else
                            dbProvider.RollbackTransaction();
                    }
                }


            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }

        /// <summary>
        /// Metodo per l'aggiornamento del ruolo proprietario relativamente
        /// alle liste di distribuzione
        /// </summary>
        /// <param name="newIdGroup">Id gruppo del ruolo da utilizzare per la sostizione</param>
        /// <param name="oldIdGroup">Id gruppo del ruolo da sostituire</param>
        /// <returns></returns>
        public bool UpdateListsOwner(String newIdGroup, String oldIdGroup)
        {
            bool retVal = false;
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("U_LISTS_OWNER");
                query.setParam("newIdGroup", newIdGroup);
                query.setParam("oldIdGroup", oldIdGroup);

                retVal = dbProvider.ExecuteNonQuery(query.getSQL());
            }
            return retVal;
        }


    }
}
