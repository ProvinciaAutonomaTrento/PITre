using System;
using System.Web.Services;
using log4net;
using System.Collections;
using System.Data;



namespace FatturazioneElettronicaService
{
    /// <summary>
    /// Summary description for FatturazioneElettronica
    /// </summary>
    [WebService(Namespace = "http://nttdata.com/2013/FatturazioneElettronica")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class FatturazioneElettronica : System.Web.Services.WebService
    {
        private static ILog logger = LogManager.GetLogger(typeof(FatturazioneElettronica));


        private DocsPaUtils.Data.ParameterSP CreateParameter(string name, object value)
        {
            return new DocsPaUtils.Data.ParameterSP(name, value);
        }

        /// <summary>
        /// Non Utilizzabile - Siamo nel servizio esterno che punta ad un altro DB.
        /// </summary>
        /// <param name="system_id"></param>
        /// <returns></returns>
        //private DocsPaVO.utente.Amministrazione _getInfoAmministrazione(string system_id)
        //{
        //    DocsPaVO.utente.Amministrazione ammin = new DocsPaVO.utente.Amministrazione();
        //    DocsPaDB.DBProvider dbp = new DocsPaDB.DBProvider();
        //    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_INFO_AMMINISTRAZIONE_BY_ID");
        //    q.setParam("param1", system_id);
        //    logger.Debug(q.getSQL());
        //    string commandText = q.getSQL();
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        if (dbp.ExecuteQuery(out ds, "mail", commandText))
        //        {
        //            if (ds.Tables["mail"].Rows.Count >= 1)
        //            {
        //                ammin.codice = ds.Tables[0].Rows[0]["VAR_CODICE_AMM"].ToString();
        //                ammin.descrizione = ds.Tables[0].Rows[0]["VAR_DESC_AMM"].ToString();
        //                ammin.email = ds.Tables[0].Rows[0]["VAR_EMAIL_RES_IPA"].ToString();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Debug("Errore nella ricerca delle informazioni dell'amministrazione ", e);
        //        return null;
        //    }
        //    return ammin;
        //}

        /// <summary>
        /// Metodo interno per l'eliminazione della entry nella DPA_DATI_FATT
        /// </summary>
        /// <param name="theUO"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private DocsPaVO.utente.Amministrazione _AmmEliminaUoSysExternal(DocsPaVO.amministrazione.OrgUO theUO, string codiceAmmIPA, string codiceAooIPA, DocsPaVO.utente.Amministrazione amm, out bool result)
        {
            int affectedRows = 0;
            DocsPaDB.DBProvider dbp = new DocsPaDB.DBProvider();
            DocsPaVO.utente.Amministrazione ammin = new DocsPaVO.utente.Amministrazione();
            
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DELETE_UO_SYSEXTERNAL");
            
            q.setParam("param1", "'" + theUO.Codice + "'");
            q.setParam("param2", "'" + codiceAmmIPA + "'");
            q.setParam("param3", "'" + codiceAooIPA + "'");

            logger.Debug(q.getSQL());
            string queryString = q.getSQL();
            try
            {
                dbp.ExecuteNonQuery(queryString, out affectedRows);
            }
            catch (Exception e)
            {
                logger.Debug("Errore nell'eliminazione della UO in DPA_DATI_FATTURAZIONE ", e);
                result = false;
                ammin = null;
                return ammin;
            }
            if (affectedRows > 0)
            {
                result = true;
                ammin = amm;
            }
            else
            {
                result = false;
                ammin = null;
            }

            return ammin;
        }

        /// <summary>
        /// Metodo interno per la Stored Procedure Modify
        /// </summary>
        /// <param name="oldCodiceUO"></param>
        /// <param name="theUO"></param>
        /// <param name="codiceAmmIPA"></param>
        /// <param name="codiceAooIPA"></param>
        /// <param name="isFatturazione"></param>
        /// <param name="codiceUAC"></param>
        /// <param name="codiceClassificazione"></param>
        /// <param name="amm"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Amministrazione _AmmModificaUoSysExternal(string oldCodiceUO, DocsPaVO.amministrazione.OrgUO theUO, string codiceAmmIPA, string codiceAooIPA, string isFatturazione, string codiceUAC, string codiceClassificazione, DocsPaVO.utente.Amministrazione amm, out bool result)
        {

            logger.Debug("modificaUoSysExternal");
            DocsPaDB.DBProvider dbp = new DocsPaDB.DBProvider();
            ArrayList parameters = new ArrayList();
            DocsPaVO.utente.Amministrazione ammin = new DocsPaVO.utente.Amministrazione();
            if (string.IsNullOrEmpty(oldCodiceUO))
                parameters.Add(this.CreateParameter("oldCodiceUO", "0"));
            else
                parameters.Add(this.CreateParameter("oldCodiceUO", "0"));
            logger.Debug("oldCodiceUO: " + oldCodiceUO);
            parameters.Add(this.CreateParameter("newCodiceUO", theUO.Codice));
            logger.Debug("newCodiceUO: " + theUO.Codice);
            parameters.Add(this.CreateParameter("idAmm", Convert.ToInt32(theUO.IDAmministrazione)));
            logger.Debug("idAmm: " + theUO.IDAmministrazione);
            parameters.Add(this.CreateParameter("codiceAoo", theUO.CodiceRegistroInterop));
            logger.Debug("codiceAoo: " + theUO.CodiceRegistroInterop);

            //Aggiunta degli altri parametri
            //codiceAmmIPA, codiceAooIPA, isFatturazione, codiceUAC, codiceClassificazione, codiceAmmPITRE
            parameters.Add(this.CreateParameter("codiceAmmIPA", codiceAmmIPA));
            logger.Debug("codiceAmmIPA: " + codiceAmmIPA);
            parameters.Add(this.CreateParameter("codiceAooIPA", codiceAooIPA));
            logger.Debug("codiceAooIPA: " + codiceAooIPA);
            if (string.IsNullOrEmpty(isFatturazione))
                parameters.Add(this.CreateParameter("isFatturazione", DBNull.Value));
            else
                parameters.Add(this.CreateParameter("isFatturazione", isFatturazione));
            logger.Debug("isFatturazione: " + isFatturazione);
            parameters.Add(this.CreateParameter("codiceUAC", codiceUAC));
            logger.Debug("codiceUAC: " + codiceUAC);
            parameters.Add(this.CreateParameter("codiceClassificazione", codiceClassificazione));
            logger.Debug("codiceClassificazione: " + codiceClassificazione);
            parameters.Add(this.CreateParameter("codiceAmmPITRE", amm.codice));
            logger.Debug("codiceAmmPITRE: " + amm.codice);
            //Unico valore da controllare è isFatturazione
             //End parametri aggiuntivi

            int rowsAffected = 0;
            string queryString = string.Empty;
            // provo a non usare la store ma a fare direttamente insert o update
            if (!string.IsNullOrEmpty(oldCodiceUO))
            {
                // caso update
                queryString = "UPDATE DPA_DATI_FATTURAZIONE SET codice_uo = '" + theUO.Codice + "'" +
		            " WHERE UPPER(Codice_Amm_Ipa) = UPPER('" + codiceAmmIPA + "') AND UPPER(Codice_Aoo_Ipa) = UPPER('" + codiceAooIPA + "') AND UPPER(codice_uo) = UPPER('" + oldCodiceUO + "')";
            }
            else
            {
                // caso insert
                //queryString = "INSERT INTO DPA_DATI_FATTURAZIONE " + 
                //    "(system_id, codice_amm, codice_aoo, codice_uo, codice_uac, codice_classificazione, var_utente_proprietario, var_tipologia_documento, var_ragione_trasmissione) " + 
                //    " VALUES(SEQ_DATI_FATTURAZIONE.Nextval, '" + codiceAmmIPA + "', '" + codiceAooIPA + "', '" + theUO.Codice + "', '" + codiceUAC + "', '" + codiceClassificazione + "', 'TIBCO', 'Fattura elettronica', 'Ricevimento fattura')";
                queryString = "Insert Into Dpa_Dati_Fatturazione " + 
                    "(System_Id, Codice_Amm_Ipa, Codice_Aoo_Ipa, Codice_Uo, Codice_Uac, Codice_Classificazione, Var_Utente_Proprietario, Id_Tipologia_Documento, Var_Ragione_Trasmissione " + 
                    ",Codice_Amm_Pitre,Codice_AOO_Pitre) " +
                    "Values(Seq_Dati_Fatturazione.Nextval, '" + codiceAmmIPA + "', '" + codiceAooIPA + "', '" + theUO.Codice + "', '" + codiceUAC + "', '" + codiceClassificazione + "', 'TIBCO', '" + isFatturazione + "', 'RICEVIMENTO_FATTURA' " +
                    ",'" + amm.codice + "','" + theUO.CodiceRegistroInterop + "')";
            }


            // Parametro di output relativo all'eventuale aggiornamento
            try
            {
                logger.Debug("DBPProvider: " + dbp.ToString());
                logger.Debug("queryString: " + queryString);
                //rowsAffected = dbp.ExecuteStoreProcedure("SP_MODIFY_UO_FATTURAZIONE", parameters);
                dbp.ExecuteNonQuery(queryString, out rowsAffected);
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella modifica della UO in DPA_DATI_FATTURAZIONE ", e);
                result = false;
                ammin = null;
                return ammin;
            }

            //logger.Debug("Chiamata SP 'SP_MODIFY_UO_FATTURAZIONE'. Esito: " + Convert.ToString(rowsAffected));

            if (rowsAffected == 1)
            {
                result = true;
                //Imposto la amministrazione passata come parametro
                ammin = amm;
                //Estraggo l'email da dpa_amministra per informare il referente che è stata effettuata una modifica o non inserimento del codice della UO
                //ammin = _getInfoAmministrazione(theUO.IDAmministrazione);
            }
            else
            {
                // ritorna false per indicare che non è stato effettuato nessun aggiornamento (e quindi non serve notificare)
                // oppure, in caso di inserimento della UO in PITRE, è stato trovato il record corrsipondente nella tabella SysExternal
                // e quindi non è necessario notificare
                result = false;
                ammin = null;
            }
            return ammin;
        }

        [WebMethod]
        public DocsPaVO.utente.Amministrazione AmmEliminaUoSysExternal(DocsPaVO.amministrazione.OrgUO theUO, string codiceAmmIPA, string codiceAooIPA, DocsPaVO.utente.Amministrazione amm, out bool result)
        {
            return _AmmEliminaUoSysExternal(theUO, codiceAmmIPA, codiceAooIPA, amm, out result);
        }

        [WebMethod]
        public DocsPaVO.utente.Amministrazione AmmModificaUoSysExternal(string oldCodiceUO, DocsPaVO.amministrazione.OrgUO theUO, string codiceAmmIPA, string codiceAooIPA, string isFatturazione, string codiceUAC, string codiceClassificazione, DocsPaVO.utente.Amministrazione amm, out bool result)
        {
            /*3 CASI:
             * INSERIMENTO NELLA TABELLA SysExternal:
             *  - accedo alla tabella SysExternal tramite codice_ipa_amm, codice_ipa_aoo e codice UO
             *  - se non lo trova invio email al referente dell'amministrazione e job
             *  - altrimenti modifica l'intero record
             * MODIFICA:
             *  - accedo alla tabella SysExternal tramite codice_ipa_amm, codice_ipa_aoo e codice UO
             *  - individuato il record aggiorno il codice UO
             */
            return _AmmModificaUoSysExternal(oldCodiceUO, theUO, codiceAmmIPA, codiceAooIPA, isFatturazione, codiceUAC, codiceClassificazione, amm, out result);
        }

        /// <summary>
        /// Metodo di prova per verifica connection con DB
        /// </summary>
        /// <returns></returns>
        public ArrayList _getTipiOggetto()
        {
            DataSet ds = new DataSet();
            ArrayList result = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TIPI_OGGETTO");
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTipiOggetto - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTipiOggetto - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteQuery(ds, commandText);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    result.Add((string)ds.Tables[0].Rows[i]["TIPO"]);
                }

            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return result;
        }

        [WebMethod]
        public virtual ArrayList getTipiOggetto()
        {
            //using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    logger.Debug("Start - metodo: getTipiOggetto");
                    ArrayList tipiOggetto = _getTipiOggetto();
                    //transactionContext.Complete();
                    logger.Debug("tipiOggetto.count: " + tipiOggetto.Count);
                    return tipiOggetto;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getTipiOggetto", e);
                    return null;
                }
            }
        }

        [WebMethod]
        public virtual bool TestConnection()
        {
            //using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    logger.Debug("Start - metodo: TestConnection");
                    Boolean result = _testConnection();
                    //transactionContext.Complete();
                    logger.Debug("TestConnection result: " + result);
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getTipiOggetto", e);
                    return false;
                }
            }
        }

        /// <summary>
        /// Metodo di prova per verifica connection con DB
        /// </summary>
        /// <returns></returns>
        public bool _testConnection()
        {
            DataSet ds = new DataSet();
            Boolean result = false;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                const string query = "select SYSTEM_ID from DPA_DATI_FATTURAZIONE";
                string commandText = query;
                System.Diagnostics.Debug.WriteLine("SQL - testConnectionFatturazione - QUERY : " + commandText);
                logger.Debug("SQL - testConnectionFatturazione - QUERY : " + commandText);
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count > 0) 
                {
                    result = true;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return result;
        }
    }
}