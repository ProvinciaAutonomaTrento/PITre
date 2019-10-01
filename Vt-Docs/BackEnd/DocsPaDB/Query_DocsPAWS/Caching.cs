using System;
using DocsPaDbManagement.Functions;
using System.Configuration;
using System.Threading;
using System.Data;
using System.Collections;
using DocsPaVO.Caching;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    public class Caching : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(Caching));

        public bool isActiveCache(string idAmministrazione)
        {

                bool result = false;
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONFIG_CACHING");
                queryDef.setParam("idAmministrazione", "'" + idAmministrazione + "'");
                logger.Debug("isActiveCache - query: " + queryDef.getSQL());
                string commandText = queryDef.getSQL();

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                if (reader.GetInt32(reader.GetOrdinal("CACHING")) == 1)
                                    result = true;
                            }
                        }
                    }
                }
                return result;
        }

        public string GET_ID_AMM_BY_DOC_NUMER(string docnumber)
        {
            string result = string.Empty;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("GET_ID_AMM_BY_DOC_NUMER");
            queryDef.setParam("docnumber", docnumber);
            

            string commandText = queryDef.getSQL();
            logger.Debug("GET_ID_AMM_BY_DOC_NUMER: " + commandText);
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            result = reader["idamm"].ToString();
                            break;
                        }
                    }
                }
            }
            return result;
        }


        

        public string GET_DOCNUMER_BY_SEGNATURA(string segnatura)
        {
            string result = string.Empty;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("GET_DOCNUMER_BY_SEGNATURA");
            queryDef.setParam("segnatura", "'"+segnatura+"'");
            

            string commandText = queryDef.getSQL();
            logger.Debug("GET_DOCNUMER_BY_SEGNATURA: " + commandText);
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            result = reader["docnumber"].ToString();
                            break;
                        }
                    }
                }
            }
            return result;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DocsPaVO.utente.InfoUtente ricercaInfoutente(string userId)
        {
            DocsPaVO.utente.InfoUtente utente = null;
            
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_INFOUTENTE");
                q.setParam("user_id", "'" + userId + "'");
                logger.Debug("ricercaInfoutente - query: " + q.getSQL());
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(q.getSQL()))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                utente = new DocsPaVO.utente.InfoUtente();
                                utente.idGruppo = reader.GetInt32(reader.GetOrdinal("GROUPS_SYSTEM_ID")).ToString();
                                utente.idPeople = reader.GetInt32(reader.GetOrdinal("system_id")).ToString();
                                utente.userId = reader.GetString(reader.GetOrdinal("user_id"));
                                utente.idAmministrazione = reader.GetInt32(reader.GetOrdinal("ID_AMM")).ToString();
                            }
                        }
                    }
                }
            
                return utente;
            
        }



        public bool inserisciConfigCache(CacheConfig info)
        {
            
            bool result = false;
            using(DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_CONFIG_CACHE");
                q.setParam("idAmministrazione", "'" + info.idAmministrazione + "'");
                if(info.caching)
                    q.setParam("caching", "1");
                else
                    q.setParam("caching", "0");
                q.setParam("massima_dimensione_caching", info.massima_dimensione_caching.ToString());
                q.setParam("massima_dimensione_file", info.massima_dimensione_file.ToString());
                q.setParam("doc_root_server", "'" + info.doc_root_server + "'");
                q.setParam("ora_inizio_cache", "'" + info.ora_inizio_cache + "'");
                q.setParam("ora_fine_cache", "'" + info.ora_fine_cache + "'");
                q.setParam("urlwscaching", "'" + info.urlwscaching + "'");
                q.setParam("urlwscachinglocale", "'" + info.url_ws_caching_locale + "'");
                q.setParam("doc_root_server_locale", "'" + info.doc_root_server_locale + "'");
                string sql = q.getSQL();
                logger.Debug("inserisciConfigCache - query: "+ sql);
                if (!dbProvider.ExecuteNonQuery(sql))
                {
                    logger.Debug("inserisciConfigCache -  errore nella query");
                    dbProvider.RollbackTransaction();
                    result = false;
               }
                else
                                    {
                    dbProvider.CommitTransaction();
                    result = true;
               }

            }
            return result;
            
        }


      


        public string recuperaPathCache(InfoFileCaching info)
        {
            string path = string.Empty;

                DocsPaUtils.Query sql = DocsPaUtils.InitQuery.getInstance().getQuery("S_VERIFICA_FILE_IN_CACHE");
                sql.setParam("docnumber", info.DocNumber.ToString());
                sql.setParam("idAmministrazione", "'" + info.idAmministrazione + "'");
                sql.setParam("version_id", info.Version_id.ToString());
                logger.Debug("recuperaPathCache - query: " + sql.getSQL());
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(sql.getSQL()))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                                path = reader.GetString(reader.GetOrdinal("PATHCACHE"));
                        }
                    }
                }
                return path;
        }

        public string recuperaPathComponents(string docnumber, string version_id)
        {
            string path = null;//string.Empty;
           
                DocsPaUtils.Query sql = DocsPaUtils.InitQuery.getInstance().getQuery("S_COMPONENTS_FILE");
                sql.setParam("docnumber", docnumber);
                sql.setParam("version_id", version_id);
                logger.Debug("recuperaPathComponents - query: " + sql.getSQL());
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(sql.getSQL()))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                                path = reader.GetString(reader.GetOrdinal("PATH")).ToString();
                        }  
                   }
                }

                if (path == null)
                    path = string.Empty;
                return path;
        }

        public void deleteCache(InfoFileCaching info)
        {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DELETE_CACHE");
                q.setParam("docnumber", info.DocNumber.ToString());
                q.setParam("idAmministrazione", "'" + info.idAmministrazione.Replace("'", "''") + "'");
                q.setParam("aggiornato", (info.Aggiornato.ToString()));
                q.setParam("version_id", (info.Version_id.ToString()));

                string sql = q.getSQL();
                logger.Debug("deleteCache - query: " + sql);

                int rowsAffected;
                bool result = this.ExecuteNonQuery(sql, out rowsAffected);

                if (!result)
                    throw new ApplicationException("Rimozione del file in caching non andata a buon fine");

                if (rowsAffected == 0)
                    throw new ApplicationException(string.Format("File in caching non trovato per il documento con docnumber '{0}'", info.DocNumber));
           
        }




        public DocsPaVO.Caching.InfoFileCaching massimaVersioneDelDocumento(string docnumber)
        {
            DocsPaVO.Caching.InfoFileCaching info = null;
           
                DocsPaUtils.Query sql = DocsPaUtils.InitQuery.getInstance().getQuery("S_CACHE_MAX_VESIONE");
                sql.setParam("docnumber", docnumber.ToString());

                logger.Debug("massimaVersioneDelDocumento - query: " + sql.getSQL());

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(sql.getSQL()))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                info = new InfoFileCaching();
                                info.CacheFilePath = reader.GetString(reader.GetOrdinal("pathcache"));
                                info.ext = reader.GetString(reader.GetOrdinal("ext"));
                                info.Version_id = reader.GetInt32(reader.GetOrdinal("version_id"));
                            }
                        }
                    }
                }
                return info;

           
        }

        public DocsPaVO.Caching.InfoFileCaching massimaVersioneDelDocumentoComponents(string docnumber)
        {
            DocsPaVO.Caching.InfoFileCaching info = null;
                DocsPaUtils.Query sql = DocsPaUtils.InitQuery.getInstance().getQuery("S_CACHE_MAX_VESIONE_COMPONENTS");
                sql.setParam("docnumber", docnumber.ToString());

                logger.Debug("massimaVersioneDelDocumentoComponents - query:" + sql.getSQL());

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(sql.getSQL()))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                info = new InfoFileCaching();
                                info.CacheFilePath = reader.GetString(reader.GetOrdinal("pathcache"));
                                info.ext = reader.GetString(reader.GetOrdinal("ext"));
                                info.Version_id = reader.GetInt32(reader.GetOrdinal("version_id"));
                            }
                        }
                    }
                }
                return info;

           
        }
        public bool verificaEsistenzaFileInCacheConVersion(string docnumber, string versionId)
        {
                bool retval = false;
                DocsPaUtils.Query sql = DocsPaUtils.InitQuery.getInstance().getQuery("VERIFICA_FILE_IN_CACHE_CON_VERISON_ID");
                sql.setParam("docnumber", docnumber);
                sql.setParam("versionId", versionId);

                logger.Debug("verificaEsistenzaFileInCacheConVersion - query:" + sql.getSQL());

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(sql.getSQL()))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                retval = true;
                            }
                        }
                    }
                }
                return retval;

           
        }

        public bool verificaEsistenzaFileInCache(string docnumber, string versionId, string idAmministrazione)
        {
                bool retval = false;
                DocsPaUtils.Query sql = DocsPaUtils.InitQuery.getInstance().getQuery("S_VERIFICA_FILE_IN_CACHE");
                sql.setParam("docnumber", docnumber);
                sql.setParam("idAmministrazione", "'" + idAmministrazione + "'");
                sql.setParam("version_id", versionId);
                logger.Debug("verificaEsistenzaFileInCache - query:" + sql.getSQL());

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(sql.getSQL()))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                retval = true;
                                break;
                            }
                        }
                    }
                }
                return retval;
        }

        
        public bool verificaEsistenzaFileInCache(string docnumber)
        {
                bool retval = false;
                DocsPaUtils.Query sql = DocsPaUtils.InitQuery.getInstance().getQuery("S_FILE_IN_CACHE");
                sql.setParam("docnumber", docnumber.ToString());
                logger.Debug("verificaEsistenzaFileInCache - query: " + sql.getSQL());

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(sql.getSQL()))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                retval = true;
                                break;
                            }
                        }
                    }
                }
                return retval;
        }


        public bool verificaEsistenzaFileInCache(InfoFileCaching info)
        {
                bool retval = false;
                DocsPaUtils.Query sql = DocsPaUtils.InitQuery.getInstance().getQuery("S_VERIFICA_FILE_IN_CACHE");
                sql.setParam("docnumber", info.DocNumber.ToString());
                sql.setParam("idAmministrazione", "'" + info.idAmministrazione + "'");
                sql.setParam("version_id", info.Version_id.ToString());
                logger.Debug("verificaEsistenzaFileInCache - query: " + sql.getSQL());

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(sql.getSQL()))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                retval = true;
                                break;
                            }
                        }
                    }
                }
                return retval;
        }


        public InfoFileCaching[] ricercaDocumemtoInCache(string aggiornato, string idAmministrazione)
        {
            System.Collections.Generic.List<InfoFileCaching> list = new System.Collections.Generic.List<InfoFileCaching>();
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_CACHE");
                queryDef.setParam("idAmministrazione", "'" + idAmministrazione + "'");
                queryDef.setParam("aggiornato", aggiornato);
                logger.Debug("ricercaDocumemtoInCache - query: " + queryDef.getSQL());
                string commandText = queryDef.getSQL();

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                InfoFileCaching info = new InfoFileCaching();

                                info.Aggiornato = reader.GetInt32(reader.GetOrdinal("AGGIORNATO"));
                                info.CacheFilePath = reader.GetString(reader.GetOrdinal("PATHCACHE"));
                                info.DocNumber = reader.GetInt32(reader.GetOrdinal("DOCNUMBER"));
                                info.file_size = reader.GetInt32(reader.GetOrdinal("FILE_SIZE"));
                                info.idAmministrazione = reader.GetString(reader.GetOrdinal("idAmministrazione"));

                                info.Version_id = reader.GetInt32(reader.GetOrdinal("VERSION_ID"));
                                list.Add(info);
                            }
                        }
                    }
                }
                return list.ToArray();
        }


        public InfoFileCaching[] ricercaDocumemtoInCacheDatrasferire(string aggiornato, string idAmministrazione)
        {
            System.Collections.Generic.List<InfoFileCaching> list = new System.Collections.Generic.List<InfoFileCaching>();
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_CACHE_DA_TRASFERIRE");
                queryDef.setParam("idAmministrazione", "'" + idAmministrazione + "'");
                queryDef.setParam("aggiornato", aggiornato);
                logger.Debug("ricercaDocumemtoInCacheDatrasferire - query:" +queryDef.getSQL());
                string commandText = queryDef.getSQL();

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                InfoFileCaching info = new InfoFileCaching();

                                info.Aggiornato = reader.GetInt32(reader.GetOrdinal("AGGIORNATO"));
                                info.CacheFilePath = reader.GetString(reader.GetOrdinal("PATHCACHE"));
                                info.DocNumber = reader.GetInt32(reader.GetOrdinal("DOCNUMBER"));
                                info.file_size = reader.GetInt32(reader.GetOrdinal("FILE_SIZE"));
                                info.idAmministrazione = reader.GetString(reader.GetOrdinal("idAmministrazione"));
                                info.Version_id = reader.GetInt32(reader.GetOrdinal("VERSION_ID"));
                                info.locked = int.Parse(reader.GetString(reader.GetOrdinal("LOCKED")));
                                info.comptype = reader.GetString(reader.GetOrdinal("COMPTYPE"));
                                info.alternate_path = reader.GetString(reader.GetOrdinal("ALTERNATE_PATH"));
                                info.var_impronta = reader.GetString(reader.GetOrdinal("VAR_IMPRONTA"));
                                info.ext = reader.GetString(reader.GetOrdinal("EXT"));
                                list.Add(info);
                            }
                        }
                    }
                }

                return list.ToArray();
        }


        public bool updateCache(DocsPaVO.Caching.InfoFileCaching info)
        {
            bool result = true;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_CACHE");
                q.setParam("docnumber", info.DocNumber.ToString());
                q.setParam("pathcache", "'" + info.CacheFilePath + "'");
                q.setParam("idAmministrazione", "'" + info.idAmministrazione + "'");
                q.setParam("aggiornato", info.Aggiornato.ToString());
                q.setParam("version_id", info.Version_id.ToString());
                q.setParam("loked", "'" + info.locked.ToString() + "'");
                q.setParam("comptype", "'" + info.comptype + "'");
                q.setParam("file_size", info.file_size.ToString());
                q.setParam("alternate_path", "'" + info.alternate_path + "'");
                q.setParam("var_impronta", "'" + info.var_impronta + "'");
                q.setParam("ext", "'" + info.ext.Trim() + "'");
                q.setParam("data", "'"+info.last_access+"'");
                string sql = q.getSQL();
                logger.Debug("updateCache - query:" + sql);
                if (!this.ExecuteNonQuery(sql))
                {
                    result = false;
                    throw new Exception();
               
                }

                return result;
            
        }



        public bool updateComponentCache(DocsPaVO.Caching.InfoFileCaching info)
        {
            bool result = true;

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_CACHE_COMPONENTS");
                q.setParam("DOCNUMBER", info.DocNumber.ToString());
                q.setParam("VERSION_ID", info.Version_id.ToString());
                q.setParam("LOCKED", "'" + info.locked.ToString() + "'");
                q.setParam("COMPTYPE", "'" + info.comptype + "'");
                q.setParam("FILE_SIZE", info.file_size.ToString());
                q.setParam("ALTERNATE_PATH", "'" + info.alternate_path + "'");
                q.setParam("VAR_IMPRONTA", "'" + info.var_impronta + "'");
                q.setParam("EXT", "'" + info.ext + "'");
                q.setParam("PATH", "'" + info.CacheFilePath + "'");

                string sql = q.getSQL();
                logger.Debug("updateComponentCache - query:" + sql);
                if (!this.ExecuteNonQuery(sql))
                {
                    result = false;
                    throw new Exception();
             
                }

                return result;
            
        }

        
        public bool inserimetoCache(InfoFileCaching info, byte[] stream)
        {
            bool result = false;
            using (DBProvider dbProvider = new DBProvider())
            {
                if (info != null)
                {
                    info.var_impronta = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(stream);
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_CACHE");
                    q.setParam("docnumber", info.DocNumber.ToString());
                    q.setParam("pathcache", "'" + info.CacheFilePath + "'");
                    q.setParam("idAmministrazione", "'" + info.idAmministrazione + "'");
                    q.setParam("aggiornato", info.Aggiornato.ToString());
                    q.setParam("version_id", info.Version_id.ToString());
                    q.setParam("loked", "'" + info.locked.ToString() + "'");
                    q.setParam("comptype", "'" + info.comptype + "'");
                    q.setParam("file_size", info.file_size.ToString());
                    q.setParam("alternate_path", "'" + info.alternate_path + "'");
                    q.setParam("var_impronta", "'" + info.var_impronta + "'");
                    q.setParam("ext", "'" + info.ext.ToUpper() + "'");
                    q.setParam("data", "'" + info.last_access + "'");
                    string sql = q.getSQL();
                    logger.Debug("inserimetoCache - query: " + sql);
                    if (!dbProvider.ExecuteNonQuery(sql))
                    {
                        logger.Debug("inserimentoCache: errore nella query");
                        dbProvider.RollbackTransaction();
                        result = false;
                    }
                    else
                    {
                        dbProvider.CommitTransaction();
                        result = true;
                    }
                }
            }
                return result;
            
            }


        public bool inserimetoCache(InfoFileCaching info)
        {
            bool result = false;
            using(DBProvider dbProvider = new DBProvider())
            {

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_CACHE");
                q.setParam("docnumber", info.DocNumber.ToString());
                q.setParam("pathcache", "'" + info.CacheFilePath + "'");
                q.setParam("idAmministrazione", "'" + info.idAmministrazione + "'");
                q.setParam("aggiornato", info.Aggiornato.ToString());
                q.setParam("version_id", info.Version_id.ToString());
                q.setParam("loked", "'" + info.locked.ToString() + "'");
                q.setParam("comptype", "'" + info.comptype + "'");
                q.setParam("file_size", info.file_size.ToString());
                q.setParam("alternate_path", "'" + info.alternate_path + "'");
                q.setParam("var_impronta", "'" + info.var_impronta + "'");
                q.setParam("ext", "'" + info.ext.ToUpper() + "'");
                q.setParam("data", "'"+info.last_access+"'");
                string sql = q.getSQL();
                logger.Debug(sql);
                if (!dbProvider.ExecuteNonQuery(sql))
                {
            
                    logger.Debug("inserimentoCache - errore query");
                    dbProvider.RollbackTransaction();
                    result = false;
                }
                else
                {
                    result = true;
                    dbProvider.CommitTransaction();
                }
            }
             
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public void deleteConfigCache(string idAmministrazione)
        {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_CONFIG_CACHE");
                q.setParam("idAmministrazione", "'" + idAmministrazione + "'");

                string sql = q.getSQL();
                logger.Debug("deleteConfigCache - query" + sql);

                int rowsAffected;
                bool result = this.ExecuteNonQuery(sql, out rowsAffected);

                if (!result)
                    throw new ApplicationException("Rimozione del file in caching non andata a buon fine");

                if (rowsAffected == 0)
                    throw new ApplicationException(string.Format("Non vi è nessun record da eliminare"));
        }


        public bool updateConfigCache(CacheConfig info)
        {
            bool result = true;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_CONFIG_CACHE");
                q.setParam("idAmministrazione", "'" + info.idAmministrazione + "'");
                if(info.caching)
                    q.setParam("caching", "1");
                else
                    q.setParam("caching", "0");
                q.setParam("massima_dimensione_caching", info.massima_dimensione_caching.ToString());
                q.setParam("massima_dimensione_file", info.massima_dimensione_file.ToString());
                q.setParam("doc_root_server", "'" + info.doc_root_server + "'");
                q.setParam("ora_inizio_cache", "'" + info.ora_inizio_cache + "'");
                q.setParam("ora_fine_cache", "'" + info.ora_fine_cache + "'");
                q.setParam("urlwscaching", "'" + info.urlwscaching + "'");
                q.setParam("urlwscachinglocale", "'" + info.url_ws_caching_locale + "'");
                q.setParam("doc_root_server_locale", "'" + info.doc_root_server_locale + "'");
                string sql = q.getSQL();
                logger.Debug("updateConfigCache - query: "+sql);
                if (!this.ExecuteNonQuery(sql))
                {
                    result = false;
                    throw new Exception();
              
                }
                return result;
        }


        public CacheConfig getConfigurazioneCache(string idAmministrazione)
        {
            CacheConfig info = null;
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONFIG_CACHE");
                queryDef.setParam("idAmministrazione", "'" + idAmministrazione + "'");
                logger.Debug("getConfigurazioneCache - query :" + queryDef.getSQL());
                string commandText = queryDef.getSQL();
               
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                info = new CacheConfig();
                                info.caching = reader.GetInt32(reader.GetOrdinal("CACHING")) == 1 ? true : false;
                                info.doc_root_server = reader.GetString(reader.GetOrdinal("DOC_ROOT_SERVER"));
                                info.massima_dimensione_caching = reader.GetDouble(reader.GetOrdinal("MASSIMA_DIMENSIONE_CACHING"));
                                info.massima_dimensione_file = reader.GetDouble(reader.GetOrdinal("MASSIMA_DIMENSIONE_FILE"));
                                info.idAmministrazione = reader.GetString(reader.GetOrdinal("idAmministrazione"));
                                info.ora_fine_cache = reader.GetString(reader.GetOrdinal("ORA_FINE_CACHE"));
                                info.ora_inizio_cache = reader.GetString(reader.GetOrdinal("ORA_INIZIO_CACHE"));
                                info.urlwscaching = reader.GetString(reader.GetOrdinal("urlwscaching"));
                                info.url_ws_caching_locale = reader.GetString(reader.GetOrdinal("url_ws_caching_locale"));
                                info.doc_root_server_locale = reader.GetString(reader.GetOrdinal("doc_root_server_locale"));
                            }
                        }
                    }
                }
                return info;
            
        }




        public InfoFileCaching getFileComponents(string docnumber, string version_id, string idAmministrazione)
        {
            InfoFileCaching info = null;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COMPONENTS_FILE");
                q.setParam("docnumber", docnumber);
                q.setParam("idAmministrazione", "'" + idAmministrazione + "'");
                q.setParam("version_id", version_id);
                string queryString = q.getSQL();
                logger.Debug("getFileComponents - query: " + queryString);
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(queryString))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                info = new InfoFileCaching();
                                info.alternate_path = reader.GetString(reader.GetOrdinal("ALTERNATE_PATH"));
                                info.CacheFilePath = reader.GetString(reader.GetOrdinal("PATH"));
                                info.comptype = reader.GetString(reader.GetOrdinal("COMPTYPE"));
                                info.DocNumber = reader.GetInt32(reader.GetOrdinal("DOCNUMBER"));
                                info.ext = reader.GetString(reader.GetOrdinal("EXT"));
                                info.var_impronta = reader.GetString(reader.GetOrdinal("VAR_IMPRONTA"));
                                info.file_size = reader.GetInt32(reader.GetOrdinal("FILE_SIZE"));
                                info.locked = int.Parse(reader.GetString(reader.GetOrdinal("LOCKED")));
                                info.Version_id = reader.GetInt32(reader.GetOrdinal("VERSION_ID"));
                            }
                        }
                    }
                }
                return info;
        }
        
        public InfoFileCaching getFileCache(string docnumber, string version_id, string idAmministrazione)
        {
            InfoFileCaching info = null;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CACHE_ALL");
                q.setParam("docnumber", docnumber);
                q.setParam("idAmministrazione", "'" + idAmministrazione + "'");
                q.setParam("version_id", version_id);
                string queryString = q.getSQL();
                logger.Debug("getFileCache - query: " + queryString);
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(queryString))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                info = new InfoFileCaching();
                                info.Aggiornato = reader.GetInt32(reader.GetOrdinal("AGGIORNATO"));
                                info.alternate_path = reader.GetString(reader.GetOrdinal("ALTERNATE_PATH"));
                                info.CacheFilePath = reader.GetString(reader.GetOrdinal("PATHCACHE"));
                                info.comptype = reader.GetString(reader.GetOrdinal("COMPTYPE"));
                                info.DocNumber = reader.GetInt32(reader.GetOrdinal("DOCNUMBER"));
                                info.ext = reader.GetString(reader.GetOrdinal("EXT"));
                                info.var_impronta = reader.GetString(reader.GetOrdinal("VAR_IMPRONTA"));
                                info.file_size = reader.GetInt32(reader.GetOrdinal("FILE_SIZE"));
                                info.idAmministrazione = reader.GetString(reader.GetOrdinal("idAmministrazione"));
                                info.locked = int.Parse(reader.GetString(reader.GetOrdinal("LOCKED")));
                                info.Version_id = reader.GetInt32(reader.GetOrdinal("VERSION_ID"));
                                info.last_access = reader.GetString(reader.GetOrdinal("LAST_ACCESS"));
                            }
                        }
                    }
                }
                return info;
        }


        public InfoFileCaching getFileCache(string docnumber)
        {
            InfoFileCaching info = null;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CACHE_BY_DOCNUMBER");
            q.setParam("docnumber", docnumber);
            string queryString = q.getSQL();
            logger.Debug("getFileCache - query: " + queryString);
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(queryString))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            info = new InfoFileCaching();
                            info.Aggiornato = reader.GetInt32(reader.GetOrdinal("AGGIORNATO"));
                            info.alternate_path = reader.GetString(reader.GetOrdinal("ALTERNATE_PATH"));
                            info.CacheFilePath = reader.GetString(reader.GetOrdinal("PATHCACHE"));
                            info.comptype = reader.GetString(reader.GetOrdinal("COMPTYPE"));
                            info.DocNumber = reader.GetInt32(reader.GetOrdinal("DOCNUMBER"));
                            info.ext = reader.GetString(reader.GetOrdinal("EXT"));
                            info.var_impronta = reader.GetString(reader.GetOrdinal("VAR_IMPRONTA"));
                            info.file_size = reader.GetInt32(reader.GetOrdinal("FILE_SIZE"));
                            info.idAmministrazione = reader.GetString(reader.GetOrdinal("idAmministrazione"));
                            info.locked = int.Parse(reader.GetString(reader.GetOrdinal("LOCKED")));
                            info.Version_id = reader.GetInt32(reader.GetOrdinal("VERSION_ID"));
                            info.last_access = reader.GetString(reader.GetOrdinal("LAST_ACCESS"));
                        }
                    }
                }
            }
            return info;
        }

        public void GetImpronta(out string impronta, string versionId, string docNumber, string idAmministrazione)
        {

            impronta = "";
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CACHE_ALL");
                q.setParam("docnumber", docNumber);
                q.setParam("idAmministrazione", "'" + idAmministrazione + "'");
                q.setParam("version_id", versionId);
                string queryString = q.getSQL();
                logger.Debug("GetImpronta - query :" +queryString);
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(queryString))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                impronta = reader.GetString(reader.GetOrdinal("VAR_IMPRONTA"));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("GetImpronta - errore: " + e.Message);
            }
            finally
            {
                this.CloseConnection();
            }
        }
    }
}
