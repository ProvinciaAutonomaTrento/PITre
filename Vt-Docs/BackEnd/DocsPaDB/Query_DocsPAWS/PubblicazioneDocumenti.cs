using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DocsPaDbManagement.Functions;
using System.Data;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    /// 
    public class PubblicazioneDocumenti
    {
        private ILog logger = LogManager.GetLogger(typeof(PubblicazioneDocumenti));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public PubblicazioneDocumenti(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this.InfoUtente = infoUtente;
        }

        public PubblicazioneDocumenti()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected DocsPaVO.utente.InfoUtente InfoUtente
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public DocsPaVO.Pubblicazione.DocumentoDaPubblicare[] RicercaDocumentiDaPubblicare(DocsPaVO.Pubblicazione.FiltroDocumentiDaPubblicare filtro)
        {
            List<DocsPaVO.Pubblicazione.DocumentoDaPubblicare> list = new List<DocsPaVO.Pubblicazione.DocumentoDaPubblicare>();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_RICERCA_DOCUMENTI_PUBBLICAZIONE");

                queryDef.setParam("idTipoAtto", filtro.IdTipoDocumento);
                queryDef.setParam("idTipoOggettoDataPubblicazione", filtro.IdTipoOggettoDataPubblicazione);
                queryDef.setParam("dataPubblicazione", filtro.DataPubblicazione);
                queryDef.setParam("dataPubblicazioneFinale", filtro.DataPubblicazioneFinale);
                queryDef.setParam("oraPubblicazione", filtro.OraPubblicazione);
                queryDef.setParam("idTipoOggettoOraPubblicazione", filtro.IdTipoOggettoOraPubblicazione);
                queryDef.setParam("oraPubblicazione", filtro.OraPubblicazione);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        list.Add(
                            new DocsPaVO.Pubblicazione.DocumentoDaPubblicare
                        {
                            IdProfile = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "system_id", false).ToString(),
                            DocNumber = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "docnumber", false).ToString(),
                            UtenteCreatore = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "author", false).ToString(),
                            RuoloCreatore = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "id_ruolo_creatore", false).ToString()
                        });
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pubblicazione"></param>
        /// <returns></returns>
        public bool InserimentoPubblicazioneDocumento(DocsPaVO.Pubblicazione.PubblicazioneDocumenti pubblicazione)
        {
            bool retval = false;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_PUBBLICAZIONI_DOCUMENTI");

                q.setParam("var_systemid", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                q.setParam("systemid", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("SEQ_PUBBLICAZIONI_DOCUMENTI"));
                q.setParam("id_profile", pubblicazione.ID_PROFILE);
                q.setParam("id_tipo_documento", pubblicazione.ID_TIPO_DOCUMENTO);
                q.setParam("id_user", string.Empty);
                q.setParam("id_ruolo", string.Empty);
                q.setParam("data_doc_pubblicato",DocsPaDbManagement.Functions.Functions.ToDate(pubblicazione.DATA_DOC_PUBBLICATO));
                q.setParam("data_pubblicazione_documento",DocsPaDbManagement.Functions.Functions.ToDate(pubblicazione.DATA_PUBBLICAZIONE_DOCUMENTO));
                q.setParam("esito_pubblicazione", pubblicazione.ESITO_PUBBLICAZIONE);
                q.setParam("errore_pubblicazione", "'" + pubblicazione.ERRORE_PUBBLICAZIONE + "'");

                logger.Debug("inserimento in circolare_pubblicazione: " + q.getSQL());
                retval = dbProvider.ExecuteNonQuery(q.getSQL());
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pubblicazione"></param>
        /// <returns></returns>
        public bool UpdatePubblicazioneDocumento(DocsPaVO.Pubblicazione.PubblicazioneDocumenti pubblicazione)
        {
            bool retval = false;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PUBBLICAZIONI_DOCUMENTI");
                q.setParam("id_profile", pubblicazione.ID_PROFILE);
                q.setParam("id_tipo_documento", pubblicazione.ID_TIPO_DOCUMENTO);
                q.setParam("data_pubblicazione_documento",DocsPaDbManagement.Functions.Functions.ToDate(pubblicazione.DATA_PUBBLICAZIONE_DOCUMENTO));
                q.setParam("esito_pubblicazione", pubblicazione.ESITO_PUBBLICAZIONE);
                q.setParam("errore_pubblicazione", "'"+pubblicazione.ERRORE_PUBBLICAZIONE+"'");
    
                logger.Debug("update in circolare_pubblicazione: " + q.getSQL());
                retval = dbProvider.ExecuteNonQuery(q.getSQL());
            }
            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pubblicazione"></param>
        /// <returns></returns>
        public bool UpdatePubblicazioneDocumentoGenerale(DocsPaVO.Pubblicazione.PubblicazioneDocumenti pubblicazione)
        {
            bool retval = false;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PUBBLICAZIONI_DOCUMENTI_GENERALE");
                q.setParam("id_profile", pubblicazione.ID_PROFILE);
                q.setParam("id_tipo_documento", pubblicazione.ID_TIPO_DOCUMENTO);
                q.setParam("data_doc_pubblicato", DocsPaDbManagement.Functions.Functions.ToDate(pubblicazione.DATA_DOC_PUBBLICATO));
                q.setParam("data_pubblicazione_documento", DocsPaDbManagement.Functions.Functions.ToDate(pubblicazione.DATA_PUBBLICAZIONE_DOCUMENTO));
                q.setParam("esito_pubblicazione", pubblicazione.ESITO_PUBBLICAZIONE);
                q.setParam("errore_pubblicazione","'" +pubblicazione.ERRORE_PUBBLICAZIONE+"'");

                logger.Debug("update in circolare_pubblicazione: " + q.getSQL());
                retval = dbProvider.ExecuteNonQuery(q.getSQL());
            }
            return retval;
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public DocsPaVO.Pubblicazione.PubblicazioneDocumenti[] RicercaPubblicazioneDocumenti(DocsPaVO.Pubblicazione.FiltroPubblicazioneDocumenti filtro)
        {
            List<DocsPaVO.Pubblicazione.PubblicazioneDocumenti> lista = new List<DocsPaVO.Pubblicazione.PubblicazioneDocumenti>();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PUBBLICAZIONI_DOCUMENTI");
            q.setParam("id_tipo_documento", filtro.ID_TIPO_DOCUMENTO);
            q.setParam("esito_pubblicazione", (filtro.ESITO_PUBBLICAZIONE ? "1" : "0"));
            string commandText = q.getSQL();
            logger.Debug("ricercaCircolaripubblicazione: " + commandText);
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        DocsPaVO.Pubblicazione.PubblicazioneDocumenti pubblicazione = new DocsPaVO.Pubblicazione.PubblicazioneDocumenti();
                        pubblicazione.DATA_DOC_PUBBLICATO = reader["data_doc_pubblicato"].ToString();
                        pubblicazione.SYSTEM_ID = reader["system_id"].ToString();
                        pubblicazione.ID_PROFILE = reader["id_profile"].ToString();
                        pubblicazione.ID_TIPO_DOCUMENTO = reader["id_tipo_documento"].ToString();
                        pubblicazione.ID_USER = reader["id_user"].ToString();
                        pubblicazione.ID_RUOLO = reader["id_ruolo"].ToString();
                        pubblicazione.DATA_DOC_PUBBLICATO = reader["data_doc_pubblicato"].ToString();
                        pubblicazione.DATA_PUBBLICAZIONE_DOCUMENTO = reader["data_pubblicazione_documento"].ToString();
                        pubblicazione.ESITO_PUBBLICAZIONE = reader["esito_pubblicazione"].ToString();
                        pubblicazione.ERRORE_PUBBLICAZIONE = reader["errore_pubblicazione"].ToString();
                        lista.Add(pubblicazione);
                    }
                }
            }

            logger.Debug("update in circolare_pubblicazione: " + q.getSQL());
            return lista.ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public DocsPaVO.Pubblicazione.PubblicazioneDocumenti[] RicercaPubblicazioneDocumentiByIdProfile(DocsPaVO.Pubblicazione.PubblicazioneDocumenti pub)
        {
            List<DocsPaVO.Pubblicazione.PubblicazioneDocumenti> lista = new List<DocsPaVO.Pubblicazione.PubblicazioneDocumenti>();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PUBBLICAZIONI_DOCUMENTI_BY_ID_PROFILE");
            q.setParam("id_tipo_documento", pub.ID_TIPO_DOCUMENTO);
            q.setParam("id_profile", pub.ID_PROFILE);
            string commandText = q.getSQL();
            logger.Debug("ricercaCircolaripubblicazione: " + commandText);
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        DocsPaVO.Pubblicazione.PubblicazioneDocumenti pubblicazione = new DocsPaVO.Pubblicazione.PubblicazioneDocumenti();
                        pubblicazione.DATA_DOC_PUBBLICATO = reader["data_doc_pubblicato"].ToString();
                        pubblicazione.SYSTEM_ID = reader["system_id"].ToString();
                        pubblicazione.ID_PROFILE = reader["id_profile"].ToString();
                        pubblicazione.ID_TIPO_DOCUMENTO = reader["id_tipo_documento"].ToString();
                        pubblicazione.ID_USER = reader["id_user"].ToString();
                        pubblicazione.ID_RUOLO = reader["id_ruolo"].ToString();
                        pubblicazione.DATA_DOC_PUBBLICATO = reader["data_doc_pubblicato"].ToString();
                        pubblicazione.DATA_PUBBLICAZIONE_DOCUMENTO = reader["data_pubblicazione_documento"].ToString();
                        pubblicazione.ESITO_PUBBLICAZIONE = reader["esito_pubblicazione"].ToString();
                        pubblicazione.ERRORE_PUBBLICAZIONE = reader["errore_pubblicazione"].ToString();
                        lista.Add(pubblicazione);
                    }
                }
            }

            logger.Debug("update in circolare_pubblicazione: " + q.getSQL());
            return lista.ToArray();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public string MaxDataPubblicazioneDocumento(DocsPaVO.Pubblicazione.FiltroPubblicazioneDocumenti filtro)
        {
            string data = string.Empty;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PUBBLICAZIONI_DOCUMENTI_MAX");
            q.setParam("id_tipo_documento", filtro.ID_TIPO_DOCUMENTO);
            string commandText = q.getSQL();
                  
            logger.Debug("maxDataCircolaripubblicazione: " + commandText);
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader != null)
                        while (reader.Read())
                            data = reader[0].ToString();
                }
            }
            return data;
        }

        public bool exsistDocument(string id_tipo_documento)
        {
            bool esiste = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ESISTE_DOCUMENTO");
            q.setParam("id_tipo_documento", id_tipo_documento);
            string commandText = q.getSQL();

            logger.Debug("exsistDocument: " + commandText);
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader != null)
                        while (reader.Read())
                        {
                            if (reader[0].ToString().Equals(id_tipo_documento))
                            {
                                esiste = true;
                                break;
                            }
                        }
                }
            }
            return esiste;
        }

        public DocsPaVO.Pubblicazione.PubblicazioneDocumenti getPubblicazioneDocumentoByIdProfile(string idProfile)
        {
            DocsPaVO.Pubblicazione.PubblicazioneDocumenti pubblicazione = null;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PUBBLICAZIONE_DOCUMENTI_BY_ID_PROFILE");
            q.setParam("id_profile", idProfile);
            logger.Debug("getEsitoPubblicazioneDocumento: "+q.getSQL());
            string commandText = q.getSQL();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        pubblicazione = new DocsPaVO.Pubblicazione.PubblicazioneDocumenti();
                        pubblicazione.SYSTEM_ID = reader["SYSTEM_ID"].ToString();
                        pubblicazione.ID_USER = reader["ID_USER"].ToString();
                        pubblicazione.ID_TIPO_DOCUMENTO = reader["ID_TIPO_DOCUMENTO"].ToString();
                        pubblicazione.ID_RUOLO = reader["ID_RUOLO"].ToString();
                        pubblicazione.ID_PROFILE = reader["ID_PROFILE"].ToString();
                        pubblicazione.ESITO_PUBBLICAZIONE = reader["ESITO_PUBBLICAZIONE"].ToString();
                        pubblicazione.ERRORE_PUBBLICAZIONE = reader["ERRORE_PUBBLICAZIONE"].ToString();
                        pubblicazione.DATA_PUBBLICAZIONE_DOCUMENTO = reader["DATA_PUBBLICAZIONE_DOCUMENTO"].ToString();
                        pubblicazione.DATA_DOC_PUBBLICATO = reader["DATA_DOC_PUBBLICATO"].ToString();
                     }
                }
            }

            return pubblicazione;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codice"></param>
        /// <returns></returns>
        public bool codicePerlaPubblicazione(string codice)
        {
            bool result = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_VERIFICA_CODICE_CORRETTO");
            q.setParam("var_cod_rubrica", codice);
            logger.Debug("codicePerlaPubblicazione " + q.getSQL());
            string commandText = q.getSQL();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                        result = true;

                }
            }
            return result;
        }

        public string getCodice(string systemid)
        {
            string result = string.Empty;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CODICE_CORRETTO_BY_ID");
            q.setParam("system_id", systemid);
            logger.Debug("getCodice " + q.getSQL());
            string commandText = q.getSQL();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                        result = reader["var_cod_rubrica"].ToString();

                }
            }
            return result;
        }

        public string getCodiceCorretto(string codice)
        {
            string idPerLaPubblicazione=string.Empty;
            string result = codice;
            bool ok = true;
            string id_parent = "1";
            while (ok)
            {
                if (string.IsNullOrEmpty(id_parent) || id_parent == "0"
                      || string.IsNullOrEmpty(codice))
                    break;
                else if (codicePerlaPubblicazione(codice))
                {
                    result = codice;
                    ok = false;
                }
                else
                {
                    //codice = RicercaFinale(codice, ref id_parent);
                    //codice = getCodice(id_parent);
                    idPerLaPubblicazione = RicercaFinale(codice, ref id_parent).ToString(); // modificata per far ritornare il system_id della UO
                    codice = getCodice(idPerLaPubblicazione); // modificata queryper utilizzare la connect by prior
                    
                    result = codice;
                    ok=false;                  
                }

            }


            return result;
        }

        private string RicercaFinale(string codice, ref string id_parent)
        {
            id_parent = null;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CODICE_CORRETTO");
            q.setParam("var_cod_rubrica", codice);
            logger.Debug("getCodiceCorretto " + q.getSQL());
            string commandText = q.getSQL();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        id_parent = reader["id_parent"].ToString();
                        //return reader["var_cod_rubrica"].ToString();
                        return reader["system_id"].ToString();
                    }
                }
            }
            return string.Empty;
        }

    
    }
}
