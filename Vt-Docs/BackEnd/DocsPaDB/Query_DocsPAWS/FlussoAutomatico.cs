using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    public class FlussoAutomatico : DBProvider
    {
        #region Const

        private ILog logger = LogManager.GetLogger(typeof(LibroFirma));

        #endregion

        #region Insert

        public bool InsertFlussoProcedurale(DocsPaVO.FlussoAutomatico.Flusso flusso)
        {
            bool result = false;
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_FLUSSO_PROCEDURALE");
                if (DBType.ToUpper().Equals("ORACLE"))
                    q.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_FLUSSO_PROCEDURALE"));
                q.setParam("idProcesso", flusso.ID_PROCESSO);
                q.setParam("idMessaggio", flusso.MESSAGGIO.ID);
                q.setParam("dataArrivo", DocsPaDbManagement.Functions.Functions.GetDate());
                q.setParam("idProfile", flusso.INFO_DOCUMENTO.ID_PROFILE);
                q.setParam("nomeRegistroIn", string.IsNullOrEmpty(flusso.INFO_DOCUMENTO.NOME_REGISTRO_IN) ? string.Empty : flusso.INFO_DOCUMENTO.NOME_REGISTRO_IN);
                q.setParam("numeroRegistroIn", string.IsNullOrEmpty(flusso.INFO_DOCUMENTO.NUMERO_REGISTRO_IN) ? "null" : flusso.INFO_DOCUMENTO.NUMERO_REGISTRO_IN);
                q.setParam("dataRegistroIn", string.IsNullOrEmpty(flusso.INFO_DOCUMENTO.DATA_REGISTRO_IN) ? "null" : DocsPaDbManagement.Functions.Functions.ToDate(flusso.INFO_DOCUMENTO.DATA_REGISTRO_IN));

                 query = q.getSQL();
                 logger.Debug("InsertFlussoProcedurale: " + query);
                if (ExecuteNonQuery(query))
                {
                    result = true;
                }
            }
            catch(Exception e)
            {
                logger.Error("Errore in InsertFlussoProcedurale: " + e.Message);
            }

            return result;
        }

        /// <summary>
        /// Inserimento di un nuovo contesto procedurale
        /// </summary>
        /// <param name="contesto"></param>
        /// <returns></returns>
        public bool InsertContestoProcedurale(DocsPaVO.FlussoAutomatico.ContestoProcedurale contesto)
        {
            bool result = false;

            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_CONTESTO_PROCEDURALE");
                if (DBType.ToUpper().Equals("ORACLE"))
                    q.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONTESTO_PROCEDURALE"));
                q.setParam("tipoContesto", contesto.TIPO_CONTESTO_PROCEDURALE);
                q.setParam("nome", contesto.NOME);
                q.setParam("famiglia", contesto.FAMIGLIA);
                q.setParam("versione", contesto.VERSIONE);

                query = q.getSQL();
                logger.Debug("InsertContestoProcedurale: " + query);
                if (ExecuteNonQuery(query))
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in InsertContestoProcedurale: " + e.Message);
            }

            return result;
        }

        public bool InsertCorrispondenteRGS(string idCorr, bool interoperanteRGS)
        {
            bool result = false;

            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_CORR_INTEROP");
                q.setParam("idCorr", idCorr);
                q.setParam("interoperanteRGS", interoperanteRGS ? "1" : "0");

                query = q.getSQL();
                logger.Debug("InsertCorrispondenteRGS: " + query);
                if (ExecuteNonQuery(query))
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in InsertCorrispondenteRGS: " + e.Message);
            }

            return result;
        }

        public bool DeleteCorrispondenteRGS(string idCorr)
        {
            bool result = false;

            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_CORR_INTEROP");
                q.setParam("idCorr", idCorr);

                query = q.getSQL();
                logger.Debug("DeleteCorrispondenteRGS: " + query);
                if (ExecuteNonQuery(query))
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in DeleteCorrispondenteRGS: " + e.Message);
            }

            return result;
        }


        public bool UpdateAssociazioneTemplateContestoProcedurale(string idTipoAtto, string idContestoProcedurale)
        {
            bool result = false;

            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TIPO_ATTO_CONTESTO");
                q.setParam("idTipoAtto", idTipoAtto);
                q.setParam("idContestoProcedurale", idContestoProcedurale);

                query = q.getSQL();
                logger.Debug("UpdateAssociazioneTemplateContestoProcedurale: " + query);
                if (ExecuteNonQuery(query))
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in UpdateAssociazioneTemplateContestoProcedurale: " + e.Message);
            }

            return result;
        }

        #endregion

        #region Select

        public bool CheckIsInteroperanteRGS(string idCorr)
        {
            bool result = false;
            string query;
            DataSet ds = new DataSet();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_CORR_INTEROP_BY_ID_CORR");
                q.setParam("idCorr", idCorr);

                query = q.getSQL();
                logger.Debug("CheckIsInteroperanteRGS: " + query);
                if (this.ExecuteQuery(out ds, "CheckIsInteroperanteRGS", query))
                {
                    if (ds.Tables["CheckIsInteroperanteRGS"] != null && ds.Tables["CheckIsInteroperanteRGS"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["CheckIsInteroperanteRGS"].Rows[0];
                        if (!string.IsNullOrEmpty(row["CHA_INTEROPERANTE_RGS"].ToString()) && row["CHA_INTEROPERANTE_RGS"].ToString().Equals("1"))
                            result = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in CheckIsInteroperanteRGS: " + e.Message);
            }

            return result;
        }

        /// <summary>
        /// Estrae la richiesta iniziale per il flusso RGS
        /// </summary>
        /// <returns></returns>
        public DocsPaVO.FlussoAutomatico.Messaggio GetMessaggioInizialeFlussoProcedurale()
        {
            DocsPaVO.FlussoAutomatico.Messaggio messaggioIniziale = new DocsPaVO.FlussoAutomatico.Messaggio();
            string query;
            DataSet ds = new DataSet();

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TIPO_MESSAGGI_FLUSSO_INIZIALE");
                query = q.getSQL();
                logger.Debug("GetMessaggioInizialeFlussoProcedurale: " + query);
                if (this.ExecuteQuery(out ds, "GetMessaggioInizialeFlussoProcedurale", query))
                {
                    if (ds.Tables["GetMessaggioInizialeFlussoProcedurale"] != null && ds.Tables["GetMessaggioInizialeFlussoProcedurale"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["GetMessaggioInizialeFlussoProcedurale"].Rows[0];
                        messaggioIniziale = this.BuildMessaggio(row);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetMessaggioInizialeFlussoProcedurale: " + e.Message);
                return null;
            }
            return messaggioIniziale;
        }

        /// <summary>
        /// Estrae il messaggio con cui è stato spedito il protocollo
        /// </summary>
        /// <param name="idProfile"></param>
        /// <returns></returns>
        public DocsPaVO.FlussoAutomatico.Messaggio GetMessaggioFlussoProceduraleByIdDocumento(string idProfile)
        {
            DocsPaVO.FlussoAutomatico.Messaggio messaggio = new DocsPaVO.FlussoAutomatico.Messaggio();
            string query;
            DataSet ds = new DataSet();

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TIPO_MESSAGGI_FLUSSO_BY_ID_PROFILE");
                q.setParam("idProfile", idProfile);
                query = q.getSQL();
                logger.Debug("GetMessaggioFlussoProceduraleByIdDocumento: " + query);
                if (this.ExecuteQuery(out ds, "MESSAGGIO_ID_PROFILE", query))
                {
                    if (ds.Tables["MESSAGGIO_ID_PROFILE"] != null && ds.Tables["MESSAGGIO_ID_PROFILE"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["MESSAGGIO_ID_PROFILE"].Rows[0];
                        messaggio = this.BuildMessaggio(row);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetMessaggioFlussoProceduraleByIdDocumento: " + e.Message);
                return null;
            }
            return messaggio;
        }


        public List<DocsPaVO.FlussoAutomatico.Messaggio> GetMessaggiSuccessiviFlusso(string idMessaggio)
        {
            List<DocsPaVO.FlussoAutomatico.Messaggio> messaggiSuccessivi = new List<DocsPaVO.FlussoAutomatico.Messaggio>();
            string query;
            DataSet ds = new DataSet();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_FLUSSO_MESSAGGI_BY_ID_MESSAGGIO");
                q.setParam("idMessaggio", idMessaggio);
                query = q.getSQL();
                logger.Debug("GetMessaggioFlussoProceduraleByIdDocumento: " + query);
                if (this.ExecuteQuery(out ds, "MESSAGGIO", query))
                {
                    if (ds.Tables["MESSAGGIO"] != null && ds.Tables["MESSAGGIO"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["MESSAGGIO"].Rows)
                        {
                            messaggiSuccessivi.Add(this.BuildMessaggio(row));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetMessaggiFlussoSuccessivi: " + e.Message);
                return null;
            }
            return messaggiSuccessivi;
        }

        public string GetIdProcessoFlussoProcedurale(string idProfile)
        {
            string idProcesso = string.Empty;
            string query;
            DataSet ds = new DataSet();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_FLUSSO_PROCEDURALE_ID_PROCESSO");
                q.setParam("idProfile", idProfile);
                query = q.getSQL();
                logger.Debug("GetIdProcessoFlussoProcedurale: " + query);
                if (this.ExecuteQuery(out ds, "ID_PROCESSO", query))
                {
                    if (ds.Tables["ID_PROCESSO"] != null && ds.Tables["ID_PROCESSO"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["ID_PROCESSO"].Rows[0];
                        idProcesso = row["ID_PROCESSO"].ToString(); ;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetIdProcessoFlussoProcedurale: " + e.Message);
                return null;
            }
            return idProcesso;
        }

        private DocsPaVO.FlussoAutomatico.Messaggio BuildMessaggio(DataRow row)
        {
            DocsPaVO.FlussoAutomatico.Messaggio messaggio = new DocsPaVO.FlussoAutomatico.Messaggio();

            messaggio.ID = row["ID_MESSAGGIO"].ToString();
            messaggio.DESCRIZIONE = row["DESCRIZIONE"].ToString();
            messaggio.INIZIALE = row["CHA_MESSAGGIO_INIZIALE"].ToString().Equals("1") ? true : false;
            messaggio.FINALE = row["CHA_MESSAGGIO_FINALE"].ToString().Equals("1") ? true : false;

            return messaggio;
        }

        public List<DocsPaVO.FlussoAutomatico.Flusso> GetListFlussoByIdProcesso(string idProcesso)
        {
            List<DocsPaVO.FlussoAutomatico.Flusso> listFlusso = new List<DocsPaVO.FlussoAutomatico.Flusso>();
            string query;
            DataSet ds = new DataSet();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_FLUSSO_PROCEDURALE_BY_ID_PROCESSO");
                q.setParam("idProcesso", idProcesso);
                query = q.getSQL();

                logger.Debug("GetListFlussoByIdProcesso: " + query);
                if (this.ExecuteQuery(out ds, "FLUSSO", query))
                {
                    if (ds.Tables["FLUSSO"] != null && ds.Tables["FLUSSO"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["FLUSSO"].Rows)
                        {
                            listFlusso.Add(this.BuildFlusso(row));
                        }
                    }
                }
            }
            catch(Exception e)
            {
                logger.Error("Errore in GetListFlussoByIdProcesso: " + e.Message);
                return null;
            }
            return listFlusso;
        }

        public DocsPaVO.FlussoAutomatico.Flusso GetFlussoInizioRichiesta(string idProcesso)
        {
            DocsPaVO.FlussoAutomatico.Flusso flusso = new DocsPaVO.FlussoAutomatico.Flusso();
            string query;
            DataSet ds = new DataSet();
            try
            {
                string idMessaggio = GetMessaggioInizialeFlussoProcedurale().ID; 
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_FLUSSO_PROCEDURALE_BY_ID_PROCESSO_INIZIALE");
                q.setParam("idProcesso", idProcesso);
                q.setParam("idMessaggio", idMessaggio);
                query = q.getSQL();

                logger.Debug("GetIdDocumentoInizioRichiesta: " + query);
                if (this.ExecuteQuery(out ds, "FLUSSO", query))
                {
                    if (ds.Tables["FLUSSO"] != null && ds.Tables["FLUSSO"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["FLUSSO"].Rows[0];
                        flusso.INFO_DOCUMENTO = new DocsPaVO.FlussoAutomatico.InfoDocumentoFlusso()
                        {
                            ID_PROFILE = row["ID_PROFILE"].ToString()
                        };
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetFlussoInizioRichiesta: " + e.Message);
                return null;
            }
            return flusso;
        }

        public List<string> GetTipiContestoProcedurale()
        {
            List<string> tipiContestoProcedurale = new List<string>();

            string query;
            DataSet ds = new DataSet();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_CONTESTO_PROCEDURALE");
                query = q.getSQL();

                logger.Debug("GetTipiContestoProcedurale: " + query);
                if (this.ExecuteQuery(out ds, "CONTESTO_PROCEDURALE", query))
                {
                    if (ds.Tables["CONTESTO_PROCEDURALE"] != null && ds.Tables["CONTESTO_PROCEDURALE"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["CONTESTO_PROCEDURALE"].Rows)
                        {
                            tipiContestoProcedurale.Add(row["TIPO_CONTESTO_PROCEDURALE"].ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetTipiContestoProcedurale: " + e.Message);
                return null;
            }

            return tipiContestoProcedurale;
        }


        public List<DocsPaVO.FlussoAutomatico.ContestoProcedurale> GetListContestoProcedurale()
        {
            List<DocsPaVO.FlussoAutomatico.ContestoProcedurale> tipiContestoProcedurale = new List<DocsPaVO.FlussoAutomatico.ContestoProcedurale>();

            string query;
            DataSet ds = new DataSet();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_CONTESTO_PROCEDURALE");
                query = q.getSQL();

                logger.Debug("GetListContestoProcedurale: " + query);
                if (this.ExecuteQuery(out ds, "CONTESTO_PROCEDURALE", query))
                {
                    if (ds.Tables["CONTESTO_PROCEDURALE"] != null && ds.Tables["CONTESTO_PROCEDURALE"].Rows.Count > 0)
                    {
                        DocsPaVO.FlussoAutomatico.ContestoProcedurale contesto;
                        foreach (DataRow row in ds.Tables["CONTESTO_PROCEDURALE"].Rows)
                        {
                            contesto = new DocsPaVO.FlussoAutomatico.ContestoProcedurale()
                            {
                                SYSTEM_ID = row["SYSTEM_ID"].ToString(),
                                TIPO_CONTESTO_PROCEDURALE = row["TIPO_CONTESTO_PROCEDURALE"].ToString(),
                                NOME = row["NOME"].ToString(),
                                FAMIGLIA = row["FAMIGLIA"].ToString(),
                                VERSIONE = row["VERSIONE"].ToString()
                            };
                            tipiContestoProcedurale.Add(contesto);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetListContestoProcedurale: " + e.Message);
                return null;
            }
            return tipiContestoProcedurale;
        }

        private DocsPaVO.FlussoAutomatico.Flusso BuildFlusso(DataRow row)
        {
            DocsPaVO.FlussoAutomatico.Flusso flusso = new DocsPaVO.FlussoAutomatico.Flusso();

            flusso.ID_PROCESSO = row["ID_PROCESSO"].ToString();
            flusso.DATA_ARRIVO = row["DATA_ARRIVO"].ToString();
            flusso.SYSTEM_ID = row["SYSTEM_ID"].ToString();

            flusso.MESSAGGIO = new DocsPaVO.FlussoAutomatico.Messaggio()
            {
                ID = row["ID_MESSAGGIO"].ToString(),
                DESCRIZIONE = row["DESCRIZIONE"].ToString(),
                INIZIALE = row["CHA_MESSAGGIO_INIZIALE"].ToString().Equals("1") ? true : false,
                FINALE = row["CHA_MESSAGGIO_FINALE"].ToString().Equals("1") ? true : false
            };

            flusso.INFO_DOCUMENTO = new DocsPaVO.FlussoAutomatico.InfoDocumentoFlusso()
            {
                ID_PROFILE = row["ID_PROFILE"].ToString(),
                NUM_PROTO = row["NUM_PROTO"].ToString(),
                OGGETTO = row["VAR_PROF_OGGETTO"].ToString(),
                NOME_REGISTRO_IN = row["NOME_REGISTRO"].ToString(),
                NUMERO_REGISTRO_IN = row["NUMERO_REGISTRO"].ToString(),
                DATA_REGISTRO_IN = row["DTA_REGISTRO"].ToString()
            };

            return flusso;
        }

        /// <summary>
        /// Restituisce l'id del primo fascicolo in cui è stato inserito il doucumento padre
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <param name="sicurezza"></param>
        /// <returns>ArrayList o 'null' se si è verificato un errore</returns>
        public DocsPaVO.fascicolazione.Fascicolo GetFasicoloByIdProfile(DocsPaVO.utente.InfoUtente infoUtente, string idProfile)
        {
            logger.Debug("GetFasicoloByIdProfile");
            DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROJECT_NO_SECURITY2_BY_ID_PROFILE");

                q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));
                q.setParam("param3", idProfile);
                q.setParam("param4", infoUtente.idPeople);
                q.setParam("param5", infoUtente.idGruppo);

                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                q.setParam("idRuoloPubblico", !string.IsNullOrEmpty(idRuoloPubblico) ? idRuoloPubblico : "0");

                string queryString = q.getSQL();

                logger.Debug(queryString);

                System.Data.DataSet dataSet;
                this.ExecuteQuery(out dataSet, "PROJECT", queryString);

                if (dataSet.Tables["PROJECT"].Rows != null && dataSet.Tables["PROJECT"].Rows.Count > 0)
                {
                    fascicolo = GetFascicolo(infoUtente, dataSet, dataSet.Tables["PROJECT"].Rows[0]);
                }
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetFasicoloByIdProfile " + e.Message);

                fascicolo = null;
            }

            return fascicolo;
        }

        private static DocsPaVO.fascicolazione.Fascicolo GetFascicolo(DocsPaVO.utente.InfoUtente infoUtente, System.Data.DataSet dataSet, System.Data.DataRow dataRow)
        {
            DocsPaVO.fascicolazione.Fascicolo objFascicolo = new DocsPaVO.fascicolazione.Fascicolo();
            string codiceRegistro = "";

            objFascicolo.systemID = dataRow["SYSTEM_ID"].ToString();
            objFascicolo.apertura = dataRow["DTA_APERTURA"].ToString().Trim();
            objFascicolo.chiusura = dataRow["DTA_CHIUSURA"].ToString().Trim();
            objFascicolo.codice = dataRow["VAR_CODICE"].ToString();
            objFascicolo.descrizione = dataRow["DESCRIPTION"].ToString();
            objFascicolo.stato = dataRow["CHA_STATO"].ToString();
            objFascicolo.tipo = dataRow["CHA_TIPO_FASCICOLO"].ToString();
            objFascicolo.idClassificazione = dataRow["ID_PARENT"].ToString();
            objFascicolo.codUltimo = dataRow["VAR_COD_ULTIMO"].ToString();
            objFascicolo.idRegistroNodoTit = dataRow["ID_REGISTRO"].ToString();
            objFascicolo.idTitolario = dataRow["ID_TITOLARIO"].ToString();

            if (dataRow.Table.Columns.Contains("NUM_MESI_CONSERVAZIONE"))
            {
                objFascicolo.numMesiConservazione = dataRow["NUM_MESI_CONSERVAZIONE"].ToString();
            }

            if (dataRow.Table.Columns.Contains("IN_CONSERVAZIONE"))
            {
                if (dataRow["IN_CONSERVAZIONE"] != DBNull.Value)
                {
                    objFascicolo.inConservazione = dataRow["IN_CONSERVAZIONE"].ToString();
                }
            }
            if (dataRow.Table.Columns.Contains("ACCESSRIGHTS"))
            {
                objFascicolo.accessRights = dataRow["ACCESSRIGHTS"].ToString();
            }

            //			//nuovo per popolare il campo descrizione del registro a cui il fascicolo è associato
            if (objFascicolo.idRegistroNodoTit != null && objFascicolo.idRegistroNodoTit != String.Empty)
            {
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                codiceRegistro = utenti.GetCodiceRegistroBySystemId(objFascicolo.idRegistroNodoTit);
                objFascicolo.codiceRegistroNodoTit = codiceRegistro;

            }

            //Se presenti inseriamo i valori di locazione fisica
            if (dataRow.Table.Columns.Contains("DTA_UO_LF") && dataRow.Table.Columns.Contains("ID_UO_LF"))
            {
                if (dataRow["DTA_UO_LF"] != null && dataRow["ID_UO_LF"] != null)
                {
                    objFascicolo.dtaLF = dataRow["DTA_UO_LF"].ToString();
                    objFascicolo.idUoLF = dataRow["ID_UO_LF"].ToString();
                }
            }

            //objFascicolo.dirittoUtente=dataRow["CHA_TIPO_DIRITTO"].ToString();

            // Gestione fascicolo cartaceo
            if (dataRow["CARTACEO"] != DBNull.Value)
            {
                int cartaceo;
                if (Int32.TryParse(dataRow["CARTACEO"].ToString(), out cartaceo))
                    objFascicolo.cartaceo = (cartaceo > 0);
            }

            // Gestione fascicolo privato
            if (dataRow.Table.Columns.Contains("CHA_PRIVATO"))
            {
                if (dataRow["CHA_PRIVATO"] != DBNull.Value)
                {
                    objFascicolo.privato = dataRow["CHA_PRIVATO"].ToString();
                }
                else
                {
                    objFascicolo.privato = null;
                }
            }

            //Gestione fascicolo pubblico
            if (dataRow.Table.Columns.Contains("CHA_PUBBLICO"))
            {
                if (dataRow["CHA_PUBBLICO"] != DBNull.Value)
                {
                    objFascicolo.pubblico = dataRow["CHA_PUBBLICO"].ToString().Equals("1") ? true : false;
                }
                else
                {
                    objFascicolo.pubblico = false;
                }
            }

            // Gestione fascicolo controllato
            objFascicolo.controllato = "0";
            if (dataRow.Table.Columns.Contains("CHA_CONTROLLATO"))
            {
                if (dataRow["CHA_CONTROLLATO"] != DBNull.Value)
                {
                    objFascicolo.controllato = dataRow["CHA_CONTROLLATO"].ToString();
                }

            }

            // Gestione fascicolazione consentita
            if (dataRow.Table.Columns.Contains("CHA_CONSENTI_CLASS"))
            {
                if (dataRow["CHA_CONSENTI_CLASS"] != DBNull.Value)
                {
                    objFascicolo.isFascConsentita = dataRow["CHA_CONSENTI_CLASS"].ToString();
                }
                else
                {
                    objFascicolo.isFascConsentita = null;
                }
            }

            // Gestione Classificazione consentita
            if (dataRow.Table.Columns.Contains("CHA_CONSENTI_FASC"))
            {
                if (dataRow["CHA_CONSENTI_FASC"] != DBNull.Value)
                {
                    objFascicolo.isFascicolazioneConsentita = dataRow["CHA_CONSENTI_FASC"].ToString().Equals("1");
                }
                else
                {
                    objFascicolo.isFascicolazioneConsentita = true;
                }
            }
            
            //Data Scadenza
            if (dataRow.Table.Columns.Contains("DTA_SCADENZA"))
                objFascicolo.dtaScadenza = dataRow["DTA_SCADENZA"].ToString();

            //Num Fascicolo
            if (dataRow.Table.Columns.Contains("NUM_FASCICOLO"))
                objFascicolo.numFascicolo = dataRow["NUM_FASCICOLO"].ToString();

            //Aggiunta campo visilbità dell'utente. Se 1 l'utente può vedere il fascicolo
            if (dataRow.Table.Columns.Contains("SICUREZZA"))
                objFascicolo.sicurezzaUtente = dataRow["SICUREZZA"].ToString();

            if (dataRow.Table.Columns.Contains("CHA_FASC_PRIMARIA"))
            {
                if (dataRow["CHA_FASC_PRIMARIA"] != DBNull.Value)
                {
                    objFascicolo.isFascPrimaria = dataRow["CHA_FASC_PRIMARIA"].ToString();
                }
            }

            if (dataRow.Table.Columns.Contains("DTA_CREAZIONE"))
            {
                objFascicolo.dataCreazione = dataRow["DTA_CREAZIONE"].ToString();
            }
            
            //***************************************************************
            //GIORDANO IACOZZILLI
            //17/07/2013
            //Gestione dell'icona della copia del docuemnto/fascicolo in deposito.
            //+FIX per le 100000 di query che ghettano il fascicolo.
            //***************************************************************
            if (dataRow.Table.Columns.Contains("CHA_IN_ARCHIVIO"))
            {
                objFascicolo.inArchivio = dataRow["CHA_IN_ARCHIVIO"] != null ? dataRow["CHA_IN_ARCHIVIO"].ToString() : "0";
            }
            //***************************************************************
            //FINE
            //***************************************************************
            return objFascicolo;
        }
        #endregion
    }
}
