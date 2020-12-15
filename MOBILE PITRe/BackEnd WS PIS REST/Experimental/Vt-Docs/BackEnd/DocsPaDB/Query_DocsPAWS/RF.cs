using System;
using System.Collections;
using System.Data;
using System.Threading;
using log4net;
using DocsPaVO.utente;
using DocsPaVO.addressbook;
using DocsPaUtils;

namespace DocsPaDB.Query_DocsPAWS 
{
	public class RF : DBProvider
	{
        private ILog logger = LogManager.GetLogger(typeof(RF));
		private Mutex semaforo = new Mutex();

		public RF(){}

        //OK
        public string getNomeRF(string codiceRF)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_GET_NOME_LISTA");
                queryMng.setParam("param1", codiceRF.ToUpper());
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getNomeRF - RF.cs - QUERY : " + commandText);
                logger.Debug("SQL - getNomeRF - RF.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                return ds.Tables[0].Rows[0][0].ToString();
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
        public string getSystemIdRF(string codiceRF)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_GET_SYSTEM_ID_LISTA");
                queryMng.setParam("param1", codiceRF.ToUpper());
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getSystemIdRF - RF.cs - QUERY : " + commandText);
                logger.Debug("SQL - getSystemIdRF - RF.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                return ds.Tables[0].Rows[0][0].ToString();
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
		public ArrayList getCorrispondentiByCodRF (string codiceRF) 
		{
			ArrayList corr = new ArrayList();
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				//semaforo.WaitOne();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("GET_CORRISPONDENTE_BY_RF");	
				queryMng.setParam("param1",codiceRF.ToUpper().Replace("'", "''"));
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - getCorrispondentiByCodRF - RF.cs - QUERY : "+commandText);
                logger.Debug("SQL - getCorrispondentiByCodRF - RF.cs - QUERY : " + commandText);

				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);

				DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
				for(int i=0; i<ds.Tables[0].Rows.Count; i++)
				{
					DocsPaVO.utente.Corrispondente c = u.GetCorrispondenteBySystemID(ds.Tables[0].Rows[i][0].ToString());
                    string ii = c.codiceRubrica;
                   // corr.Add(c);
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
			catch 
			{
				return null;
			}
			finally
			{
				dbProvider.Dispose();
				//semaforo.ReleaseMutex();
			}					

			return corr;			
		}

        public ArrayList getCorrispondentiByDescRF(string descRF)
        {
            ArrayList corr = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("GET_CORRISPONDENTE_BY_DESC_RF");
                queryMng.setParam("param1", descRF.ToUpper());
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getCorrispondentiByDescRF - RF.cs - QUERY : " + commandText);
                logger.Debug("SQL - getCorrispondentiByDescRF - RF.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DocsPaVO.utente.Corrispondente c = u.GetCorrispondenteBySystemID(ds.Tables[0].Rows[i][0].ToString());
                    string ii = c.codiceRubrica;
                    // corr.Add(c);
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
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
                //semaforo.ReleaseMutex();
            }

            return corr;
        }

        public string getSystemIdRFDaDPA_EL_REGISTRI(string codiceRF)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_SYSTEM_ID_DPA_EL_REGISTRI");
                queryMng.setParam("var_codice", codiceRF.ToUpper());
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getSystemIdRFDaDPA_EL_REGISTRI - RF.cs - QUERY : " + commandText);
                logger.Debug("SQL - getSystemIdRFDaDPA_EL_REGISTRI - RF.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                return ds.Tables[0].Rows[0][0].ToString();
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

        #region Gestione RF trattato come Corrispondente

        /// <summary>
        /// Metodo per il salvataggio del dettaglio di un Raggruppamento Funzionale nella rubrica
        /// locale. 
        /// </summary>
        /// <param name="rf">Dettagli da salvare</param>
        /// <param name="idRf">Id dell'RF</param>
        /// <returns>Esito dell'operazione di salvataggio dei dettagli</returns>
        public bool SaveRaggruppamentoFunzionaleCorrGlobali(RaggruppamentoFunzionale rf, String idRf)
        {
            bool retVal = false;

            String idCorr = this.GetIdRfCorrGlobali(idRf);

            Query query = InitQuery.getInstance().getQuery("U_DPADettGlob2");
            query.setParam("param1", String.Format("'{0}',", rf.indirizzo));
            query.setParam("param2", String.Format("'{0}',", rf.citta));
            query.setParam("param3", String.Format("'{0}',", rf.cap));
            query.setParam("param4", String.Format("'{0}',",rf.prov));
            query.setParam("param5", String.Format("'{0}',", rf.nazionalita));
            query.setParam("param6", String.Format("'{0}',", rf.telefono1));
            query.setParam("param7", String.Format("'{0}',", String.Empty));
            query.setParam("param8", String.Format("'{0}',", rf.fax));
            query.setParam("var_note", String.Format("'{0}'", String.Empty));
            query.setParam("param9", idCorr);

            using (DBProvider dbProvider = new DBProvider())
            {
                retVal = dbProvider.ExecuteNonQuery(query.getSQL());
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per il reperimento dei dettaglio di un Raggruppamento Funzionale. 
        /// </summary>
        /// <param name="idRf">Id dell'RF da caricare</param>
        /// <returns>Dettaglio del raggruppamento funzionale</returns>
        public RaggruppamentoFunzionale GetRaggruppamentoFunzionaleRC(String idRf)
        {
            RaggruppamentoFunzionale rf = new RaggruppamentoFunzionale();

            // Reperimento delle informazioni sull'RF come corrispondente
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_RF_CODE_DESCRIPTION_AND_INFO");
                query.setParam("idRf", idRf);

                // Sostituzione del dbUser se il db è sql server
                if (base.DBType.ToUpper() == "SQL")
                    query.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession() + ".");
               
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    while (dataReader.Read())
                        rf = new RaggruppamentoFunzionale()
                        {
                            systemId = idRf,
                            Codice = dataReader["var_cod_rubrica"].ToString(),
                            descrizione = dataReader["var_desc_corr"].ToString(),
                            idAmministrazione = dataReader["id_amm"].ToString(),                            
                            indirizzo = dataReader["var_indirizzo"].ToString(),
                            citta = dataReader["var_citta"].ToString(),
                            cap = dataReader["var_cap"].ToString(),
                            nazionalita = dataReader["var_nazione"].ToString(),
                            prov = dataReader["var_provincia"].ToString(),
                            telefono1 = dataReader["var_telefono"].ToString(),
                            telefono2 = dataReader["var_telefono2"].ToString(),
                            fax = dataReader["var_fax"].ToString(),
                            note = dataReader["var_note"].ToString(),
                            codiceAOO = dataReader["Aoo"].ToString(), 
                            codfisc = dataReader["var_cod_fisc"].ToString(),
                            partitaiva = dataReader["var_cod_pi"].ToString()

                        };

                }

            }

            return rf;

        }

        /// <summary>
        /// Metodo per il recupero dell'id corr globali di un RF
        /// </summary>
        /// <param name="idRf">Id dell'RF di cui recuperare l'id corr globali</param>
        /// <returns>Id corr globali dell'RF</returns>
        private String GetIdRfCorrGlobali(String idRf)
        {
            String retVal = String.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_GET_RF_CORR_GLOB_ID_FROM_ID_RF");
                query.setParam("idRf", idRf);
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    while(dataReader.Read())
                        retVal = dataReader[0].ToString();
                }
            }

            return retVal;
 
        }

        #endregion

    }
}
