using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
using log4net;
using DocsPaUtils;
using DocsPaVO.documento;
using System.IO;
using DocsPaVO.areaConservazione;
using DocsPaVO.utente;
using DocsPaVO.FormatiDocumento;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// 
    /// </summary>
    public class Conservazione
    {
        /// <summary>
        /// 
        /// </summary>
        private ILog logger = LogManager.GetLogger(typeof(Conservazione));

        /// <summary>
        /// Creazione di una nuova istanza di conservazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="descrizione"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public string CreateAreaConservazione(DocsPaVO.utente.InfoUtente infoUtente, string descrizione, string note, string idPolicy, string stato, bool consolidamento, string tipoIstanza)
        {
            string retValue = string.Empty;
            //modifica sab per gestione valore di ritorno (scope_identity), è una schifezza, ma al momento non è gestito nulla in transazione;
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            dbProvider.BeginTransaction();
            //using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_INSERT_ISTANZA_CONSERVAZIONE");

                queryDef.setParam("colSystemId", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryDef.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_AREA_CONSERVAZIONE"));
                queryDef.setParam("descrizione", descrizione.Replace("'", "''"));
                queryDef.setParam("note", note.Replace("'", "''"));
                queryDef.setParam("idAmministrazione", infoUtente.idAmministrazione);
                queryDef.setParam("idPeople", infoUtente.idPeople);
                queryDef.setParam("idRuoloInUo", infoUtente.idCorrGlobali);
                queryDef.setParam("userId", infoUtente.userId);
                queryDef.setParam("idGruppo", infoUtente.idGruppo);
                queryDef.setParam("stato", stato);
                if (!string.IsNullOrEmpty(stato) && stato.Equals("I"))
                {
                    queryDef.setParam("dataInvio", DocsPaDbManagement.Functions.Functions.GetDate());
                }
                else
                {
                    queryDef.setParam("dataInvio", "''");
                }
                queryDef.setParam("tipoIstanza", tipoIstanza);
                if (!string.IsNullOrEmpty(idPolicy))
                {
                    queryDef.setParam("idPolicy", idPolicy);
                    queryDef.setParam("validaPolicy", idPolicy);
                }
                else
                {
                    queryDef.setParam("idPolicy", "''");
                    queryDef.setParam("validaPolicy", "''");
                }
                if (consolidamento)
                {
                    queryDef.setParam("consolida", "1");
                }
                else
                {
                    queryDef.setParam("consolida", null);
                }

                string commandText = queryDef.getSQL();
                logger.InfoFormat("I_INSERT_ISTANZA_CONSERVAZIONE: {0}", commandText);

                logger.Debug("Inserimento nuova azione in registro conservazione: ");
                logger.Debug(commandText);
                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_AREA_CONSERVAZIONE");

                string field;
                if (!dbProvider.ExecuteScalar(out field, commandText))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                dbProvider.CommitTransaction();
                retValue = field;
            }

            return retValue;
        }

        /// <summary>
        /// Rimozione di un'istanza di conservazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="id"></param>
        public void DeleteIstanzaConservazione(DocsPaVO.utente.InfoUtente infoUtente, string id)
        {
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("D_DELETE_ISTANZA_CONSERVAZIONE");
                queryDef.setParam("systemId", id);

                string commandText = queryDef.getSQL();
                logger.InfoFormat("I_INSERT_ISTANZA_CONSERVAZIONE: {0}", commandText);

                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);
            }
        }

        public string addAreaConservazione(string idConservazione,
                                            string idProfile,
                                            string idProject,
                                            string docNumber,
                                            DocsPaVO.utente.InfoUtente infoUtente,
                                            string tipoOggetto,
                                            string idPolicy)
        {
            string result = String.Empty;

            if (idProject != null && idProject != String.Empty)
            {
                DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                //DocsPaVO.fascicolazione.Fascicolo fasc = fascicoli.GetFascicoloById(idProject, infoUtente);
                DocsPaVO.fascicolazione.Fascicolo fasc = fascicoli.getFascicoloByIdNoSecurity(idProject);
                result = this.addDocAreaConservazione(idConservazione, idProfile, idProject, docNumber, infoUtente, fasc.codice, tipoOggetto, idPolicy);
            }
            else
            {
                result = this.addDocAreaConservazione(idConservazione, idProfile, idProject, docNumber, infoUtente, "", tipoOggetto, idPolicy);
            }

            return result;
        }


        public string addDocAreaConservazione(
                                            string idConservazione,
                                            string idProfile,
                                            string idProject,
                                            string docNumber,
                                            DocsPaVO.utente.InfoUtente infoUtente,
                                            string codFasc,
                                            string tipoOggetto,
                                            string idPolicy)
        {
            Documenti documentiDb = new Documenti();
            DocsPaVO.documento.SchedaDocumento schedaDoc = documentiDb.GetDettaglioNoSecurity(infoUtente, idProfile, docNumber);

            ArrayList parameters = new ArrayList();
            string result = String.Empty;
            //  string sysId = "";
            DBProvider dbProvider = new DBProvider();
            try
            {
                string idPeople = infoUtente.idPeople;
                string idGruppo = infoUtente.idGruppo;
                string idAmm = infoUtente.idAmministrazione;
                string tipoProto = schedaDoc.tipoProto;
                string oggetto = schedaDoc.oggetto.descrizione;
                string userId = infoUtente.userId;
                string tipologiaDoc = string.Empty;
                if (schedaDoc.tipologiaAtto != null)
                    tipologiaDoc = schedaDoc.tipologiaAtto.descrizione;

                DocsPaVO.documento.TipologiaAtto tipoAtto = schedaDoc.tipologiaAtto;
                if (tipoAtto != null && tipoAtto.systemId.ToString() != String.Empty)
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelDB.getTemplate(docNumber);
                }

                parameters.Add(this.CreateParameter("idAmm", Convert.ToInt32(idAmm)));

                // Se l'idConservazione è stato fornito, il documento viene inserito in un'istanza già esistente
                if (string.IsNullOrEmpty(idConservazione))
                    parameters.Add(this.CreateParameter("idConservazione", DBNull.Value));
                else
                    parameters.Add(this.CreateParameter("idConservazione", Convert.ToInt32(idConservazione)));


                parameters.Add(this.CreateParameter("idPeople", Convert.ToInt32(idPeople)));
                if (idProfile != null && idProfile != String.Empty)
                {
                    parameters.Add(this.CreateParameter("idProfile", Convert.ToInt32(idProfile)));
                }
                else
                {
                    parameters.Add(this.CreateParameter("idProfile", DBNull.Value));
                }
                if (idProject == null || idProject == String.Empty)
                {
                    parameters.Add(this.CreateParameter("idProject", DBNull.Value));
                    parameters.Add(this.CreateParameter("codFasc", DBNull.Value));
                }
                else
                {
                    parameters.Add(this.CreateParameter("idProject", Convert.ToInt32(idProject)));
                    parameters.Add(this.CreateParameter("codFasc", codFasc));
                }
                parameters.Add(this.CreateParameter("oggetto", oggetto));
                parameters.Add(this.CreateParameter("tipoDoc", tipoProto));
                // parameters.Add(this.CreateParameter("sizeItem", total_size));

                parameters.Add(this.CreateParameter("idGruppo", Convert.ToInt32(idGruppo)));
                if (schedaDoc.registro != null)
                {
                    if (schedaDoc.registro.systemId != String.Empty)
                    {
                        parameters.Add(this.CreateParameter("idRegistro", Convert.ToInt32(schedaDoc.registro.systemId)));
                    }
                }
                else
                {
                    parameters.Add(this.CreateParameter("idRegistro", DBNull.Value));
                }

                parameters.Add(this.CreateParameter("docNumber", Convert.ToInt32(schedaDoc.docNumber)));

                parameters.Add(this.CreateParameter("userId", userId));

                parameters.Add(this.CreateParameter("tipoOggetto", tipoOggetto));

                parameters.Add(this.CreateParameter("tipoAtto", tipologiaDoc));

                // Identificativo della policy con cui è stata creata l'istanza di conservazione
                if (string.IsNullOrEmpty(idPolicy))
                    parameters.Add(this.CreateParameter("idPolicy", DBNull.Value));
                else
                    parameters.Add(this.CreateParameter("idPolicy", idPolicy));


                DocsPaUtils.Data.ParameterSP versionIdParam = new DocsPaUtils.Data.ParameterSP("result", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                parameters.Add(versionIdParam);

                ///*
                foreach (DocsPaUtils.Data.ParameterSP al in parameters)
                {
                    logger.DebugFormat("{0} -> {1}", al.Nome, al.Valore);
                }
                //*/
                DataSet ds = null;
                logger.DebugFormat("Chiamo > SP_INSERT_AREA_CONS");
                if (dbProvider.ExecuteStoredProcedure("SP_INSERT_AREA_CONS", parameters, ds) != -1)
                {
                    result = Convert.ToString(versionIdParam.Valore);
                    // logger.DebugFormat("SP_INSERT_AREA_CONS-> Result {0}", result);
                    //  dbProvider.SetLargeText("DPA_ITEMS_CONSERVAZIONE", sysId, "VAR_XML_METADATI", xmlMetadati[1]);
                }
                else
                {
                    result = "-1";
                }
            }
            catch
            {
                result = "-1";
            }
            finally
            {
                dbProvider.Dispose();
            }
            return result;
        }

        public bool insertSizeInItemsCons(int size, string sysId)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_SizeItemCons");
            bool result = true;
            try
            {
                q.setParam("size", Convert.ToString(size));
                q.setParam("sysId", "'" + sysId + "'");
                string queryString = q.getSQL();
                logger.Debug(queryString);
                using (DBProvider dbProvider = new DBProvider())
                    dbProvider.ExecuteNonQuery(queryString);
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Conservazione (query- U_SizeItemCons)");
                result = false;
            }
            return result;
        }

        public bool updateItemsCons(string tipoFile, string numAllegati, string sysId)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ITEMS_CONS");
            bool result = true;
            try
            {
                string setParam = " VAR_TIPO_FILE='" + tipoFile + "'," +
                                  " NUMERO_ALLEGATI='" + numAllegati + "'";

                string whereParam = " SYSTEM_ID='" + sysId + "'";

                q.setParam("param1", setParam);
                q.setParam("param2", whereParam);
                string queryString = q.getSQL();
                logger.Debug("updateItemsCons " + queryString);

                using (DBProvider dbProvider = new DBProvider())
                    dbProvider.ExecuteNonQuery(queryString);
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Conservazione (query- U_DPA_ITEMS_CONS)");
                result = false;
            }
            return result;
        }


        public bool UpdateStatoAreaCons(string sysId, string tipo_cons, string note, string descr, string idTipoSupp, bool consolida)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_AREA_CONS");
            DocsPaUtils.Query qItems = DocsPaUtils.InitQuery.getInstance().getQuery("U_ItemsConservazione");
            bool result = true;
            System.DateTime now = System.DateTime.Now;
            CultureInfo ci = new CultureInfo("en-US");
            //string dateString = DocsPaDbManagement.Functions.Functions.ToDate(now.ToString("dd/MM/yyyy hh24:mm:ss tt", ci));
            string dateString = DocsPaDbManagement.Functions.Functions.GetDate(true).ToString();
            try
            {
                q.setParam("idTipoSupp", "'" + idTipoSupp + "'");
                q.setParam("tipoCons", "'" + tipo_cons + "'");
                q.setParam("note", "'" + note + "'");
                q.setParam("descrizione", "'" + descr + "'");
                q.setParam("data", dateString);
                q.setParam("sysId", "'" + sysId + "'");

                if (consolida)
                {
                    q.setParam("consolida", "'1'");
                }
                else
                {
                    q.setParam("consolida", "''");
                }

                using (DBProvider dbProvider = new DBProvider())
                {
                    string queryString = q.getSQL();
                    dbProvider.ExecuteNonQuery(queryString);

                    qItems.setParam("idCons", "'" + sysId + "'");
                    dbProvider.ExecuteNonQuery(qItems.getSQL());
                }
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Conservazione (query- UpdateStatoAreaCons)");
                result = false;
            }
            return result;
        }

        public bool DeleteAreaConservazione(string idProject, string idProfile, string idIstanza, bool deleteIstanza, string systemId)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPAItemsConservazione");
            bool result = true;
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    string query = "";
                    //elimino un intero fascicolo
                    if (idProject != null && idProject != String.Empty && (idProfile == null || idProfile == String.Empty))
                        query = " ID_PROJECT=" + idProject + " AND CHA_STATO='" + "N'";

                    //elimino un documento sciolto
                    if (idProfile != null && idProfile != String.Empty && (idProject == null || idProject == String.Empty))
                        query = " ID_PROFILE=" + idProfile + " AND ID_PROJECT is null AND CHA_STATO='" + "N'";

                    //elimino un documento di un fascicolo
                    if (idProject != null && idProject != String.Empty && idProfile != null && idProfile != String.Empty)
                        query = " ID_PROFILE=" + idProfile + " AND ID_PROJECT=" + idProject + " AND CHA_STATO='" + "N'";

                    if (idIstanza != null && idIstanza != String.Empty)
                        query = " ID_CONSERVAZIONE=" + idIstanza;

                    if (!string.IsNullOrEmpty(systemId))
                        query = " SYSTEM_ID=" + systemId;

                    q.setParam("param", query);
                    string queryString = q.getSQL();

                    dbProvider.ExecuteNonQuery(queryString);

                    if (deleteIstanza)
                    {
                        DocsPaUtils.Query qDelete = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPAAreaConservazione");
                        qDelete.setParam("idIstanza", "'" + idIstanza + "'");

                        dbProvider.ExecuteNonQuery(qDelete.getSQL());
                    }
                }
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Conservazione (query- DeleteAreaConservazione)");
                result = false;

            }
            return result;
        }

        public int CanDeleteFromConservazione(string idProfile, string idPeople, string idGruppo)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAItemsConservazione");
            int result = 0;
            try
            {
                string query = " ID_PROFILE =" + idProfile + " AND ID_PEOPLE=" + idPeople + " AND ID_RUOLO_IN_UO = (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO =" + idGruppo + ")";

                q.setParam("param", query);
                //q.setParam("idPeople", idPeople);
                //q.setParam("idGruppo", idGruppo);
                string commandText = q.getSQL();
                DataSet ds = new DataSet();

                using (DBProvider dbProvider = new DBProvider())
                    dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    result = 1;
                }
                else
                {
                    result = 0;
                }
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Conservazione (query- CanDeleteFromConservazione)");
                result = -1;
            }
            return result;
        }

        /// <summary>
        /// Metodo che serve per il controllo se un documento fascicolato è già presente in un'istanza di conservazione.
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public string getPrimaIstanzaConservazione(string idPeople, string idGruppo)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAreaConservazione");
            string result = string.Empty;
            try
            {
                q.setParam("idPeople", idPeople);
                q.setParam("idGruppo", idGruppo);
                string commandText = q.getSQL();
                DataSet ds = new DataSet();

                using (DBProvider dbProvider = new DBProvider())
                    dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    result = ds.Tables[0].Rows[0][0].ToString();
                }
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Conservazione (query- isPrimaIstanzaConservazione)");
                result = "Errore";
            }
            return result;
        }

        public int isPrimaIstanzaConservazione(string idPeople, string idGruppo)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAreaConservazione");
            int result = 0;
            try
            {
                q.setParam("idPeople", idPeople);
                q.setParam("idGruppo", idGruppo);
                string commandText = q.getSQL();
                DataSet ds = new DataSet();

                using (DBProvider dbProvider = new DBProvider())
                    dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    result = 0;
                }
                else
                {
                    result = 1;
                }
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Conservazione (query- isPrimaIstanzaConservazione)");
                result = -1;
            }
            return result;
        }

        /// <summary>
        /// Cancellazione di un documento da un'istanza di conservazione
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <param name="idProfile"></param>
        /// <returns></returns>
        public void DeleteDocumentoFromItemCons(string idIstanza, string idProfile)
        {
            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("DELETE_DPA_ITEMS_CONS");

                queryDef.setParam("param", string.Format(" ID_CONSERVAZIONE = {0} AND ID_PROFILE = {1}", idIstanza, idProfile));

                string commandText = queryDef.getSQL();

                using (DBProvider dbProvider = new DBProvider())
                {
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        throw new ApplicationException(dbProvider.LastExceptionMessage);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Errore nella cancellazione del documento dall'istanza di conservazione: {0}", ex.Message), ex);
            }
        }

        public bool DocumentoDeleteFromItemsCons(string idIstanza, string systemId, string idProject)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DELETE_DPA_ITEMS_CONS");
            bool result = false;
            try
            {
                string queryParam = "";
                if (!string.IsNullOrEmpty(idProject))
                {
                    queryParam = " ID_CONSERVAZIONE =" + idIstanza + " AND ID_PROJECT=" + idProject + " AND CHA_TIPO_OGGETTO='F'";
                }
                else
                {
                    queryParam = " SYSTEM_ID=" + systemId;
                }
                q.setParam("param", queryParam);
                string commandText = q.getSQL();

                using (DBProvider dbProvider = new DBProvider())
                    result = dbProvider.ExecuteNonQuery(commandText);
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Conservazione (query- DELETE_DPA_ITEMS_CONS)");
            }
            return result;
        }

        public bool updateEsitoItemCons(string systemId, string esito)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_ESITO_DPA_ITEMS_CONS");
            bool result = false;
            try
            {
                q.setParam("param1", esito);
                q.setParam("param2", systemId);
                string commandText = q.getSQL();
                using (DBProvider dbProvider = new DBProvider())
                    result = dbProvider.ExecuteNonQuery(commandText);
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Conservazione (query- UPDATE_ESITO_DPA_ITEMS_CONS)");
            }
            return result;
        }

        private DocsPaUtils.Data.ParameterSP CreateParameter(string name, object value)
        {
            return new DocsPaUtils.Data.ParameterSP(name, value);
        }

        public bool AddDocInAreaConservazioneWithPolicy(string idConservazione, string idProfile, string idProject, string docNumber, string tipoOggetto, DocsPaVO.utente.InfoUtente infoUtente, string stato, DocsPaVO.areaConservazione.ItemPolicyValidator itemPolicyValidator)
        {
            Documenti documentiDb = new Documenti();
            DocsPaVO.documento.SchedaDocumento schedaDoc = documentiDb.GetDettaglioNoSecurity(infoUtente, idProfile, docNumber);

            bool result = false;

            DBProvider dbProvider = new DBProvider();
            try
            {

                string idPeople = infoUtente.idPeople;
                string idGruppo = infoUtente.idGruppo;
                string idAmm = infoUtente.idAmministrazione;
                string tipoProto = schedaDoc.tipoProto;
                string oggetto = schedaDoc.oggetto.descrizione;
                string userId = infoUtente.userId;
                string tipologiaDoc = string.Empty;
                if (schedaDoc.tipologiaAtto != null)
                    tipologiaDoc = schedaDoc.tipologiaAtto.descrizione;

                DocsPaVO.documento.TipologiaAtto tipoAtto = schedaDoc.tipologiaAtto;
                if (tipoAtto != null && tipoAtto.systemId.ToString() != String.Empty)
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelDB.getTemplate(docNumber);
                }


                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_ITEMS_CONSERVAZIONE_WITH_POLICY");
                q.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                q.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ITEMS_CONSERVAZIONE"));
                q.setParam("idConservazione", idConservazione);

                q.setParam("idProfile", idProfile);

                if (!string.IsNullOrEmpty(idProject))
                {
                    q.setParam("idProject", idProject);
                }
                else
                {
                    q.setParam("idProject", "NULL");
                }

                q.setParam("tipoDoc", tipoProto);

                q.setParam("oggetto", oggetto);

                if (schedaDoc != null && schedaDoc.registro != null && !string.IsNullOrEmpty(schedaDoc.registro.systemId))
                {
                    q.setParam("idRegistro", schedaDoc.registro.systemId);
                }
                else
                {
                    q.setParam("idRegistro", "NULL");
                }

                System.DateTime now = System.DateTime.Now;
                CultureInfo ci = new CultureInfo("en-US");
                string dateString = DocsPaDbManagement.Functions.Functions.ToDate(now.ToString("dd/MM/yyyy hh:mm:ss tt", ci));

                q.setParam("dataInserimento", dateString);

                q.setParam("stato", stato);

                if (!string.IsNullOrEmpty(idProject))
                {
                    DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                    DocsPaVO.fascicolazione.Fascicolo fasc = fascicoli.getFascicoloByIdNoSecurity(idProject);
                    q.setParam("codFasc", fasc.codice);
                }
                else
                {
                    q.setParam("codFasc", null);
                }

                q.setParam("docNumber", schedaDoc.docNumber);

                q.setParam("tipoOggetto", tipoOggetto);

                if (stato.Equals("I"))
                {
                    q.setParam("chaEsito", "1");
                    //MEV CONS 3.1 - aggiunge info del mask della policy
                    if (itemPolicyValidator != null)
                        q.setParam("maskPolicyValidator", DocsPaVO.areaConservazione.ItemPolicyValidator.getMaskItemPolicyValidator(itemPolicyValidator));
                }

                else
                {
                    q.setParam("chaEsito", null);
                    // aggiunge valore null per il mask della policy
                    q.setParam("maskPolicyValidator", null);
                }

                if (schedaDoc.tipologiaAtto != null)
                {
                    q.setParam("tipoAtto", schedaDoc.tipologiaAtto.descrizione);
                }
                else
                {
                    q.setParam("tipoAtto", null);
                }

                string systemID = string.Empty;
                string commandText = q.getSQL();
                logger.Debug(commandText);

                dbProvider.ExecuteScalar(out systemID, commandText);
                dbProvider.CommitTransaction();

                string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_ITEMS_CONSERVAZIONE");
                dbProvider.ExecuteScalar(out systemID, sql);

                //int size_xml = 0;

                //DocsPaConservazione.InfoDocXml DocXml = new DocsPaConservazione.InfoDocXml();
                //size_xml = System.Convert.ToInt32(DocXml.serializeScheda(schedaDoc, systemID));

                DocsPaVO.documento.FileRequest arrayDocPrincipale = (DocsPaVO.documento.FileRequest)(schedaDoc.documenti[0]);

                int doc_size = Convert.ToInt32(arrayDocPrincipale.fileSize);

                int numeroAllegati = schedaDoc.allegati.Count;
                string fileName = arrayDocPrincipale.fileName;
                string tipoFile = Path.GetExtension(fileName);
                int size_allegati = 0;
                for (int i = 0; i < schedaDoc.allegati.Count; i++)
                {
                    size_allegati = size_allegati + Convert.ToInt32(((DocsPaVO.documento.FileRequest)schedaDoc.allegati[i]).fileSize);
                }
                int total_size = size_allegati + doc_size; // +size_xml;

                result = insertSizeInItemsCons(total_size, systemID);

                updateItemsCons(tipoFile, (numeroAllegati).ToString(), systemID);

                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                dbProvider.Dispose();
            }
            return result;
        }



        #region REGISTRO CONSERVAZIONE




        /// <summary>
        /// inserimento azione nel registro di conservazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="descrizione"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public string InsertInRegistroCons(DocsPaVO.Conservazione.RegistroCons registroCons)
        {
            string retValue = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_INSERT_REGISTRO_CONSERVAZIONE");

                queryDef.setParam("colSystemId", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryDef.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_REGISTRO_CONSERVAZIONE"));



                queryDef.setParam("codAzione", registroCons.codAzione);
                queryDef.setParam("descAzione", registroCons.descAzione.Replace("'", "''"));
                queryDef.setParam("idIstanza", registroCons.idIstanza);

                if (registroCons.idOggetto != null && !string.IsNullOrEmpty(registroCons.idOggetto))
                {
                    queryDef.setParam("idOggetto", registroCons.idOggetto);
                }
                else
                {
                    queryDef.setParam("idOggetto", "NULL");
                }

                queryDef.setParam("tipoOggetto", registroCons.tipoOggetto);
                queryDef.setParam("userId", registroCons.userId);
                queryDef.setParam("esito", registroCons.esito);
                queryDef.setParam("idAmministrazione", registroCons.idAmm);
                queryDef.setParam("tipoAzione", registroCons.tipoAzione);
                queryDef.setParam("printed", registroCons.printed);


                string commandText = queryDef.getSQL();
                logger.InfoFormat("I_INSERT_REGISTRO_CONSERVAZIONE: {0}", commandText);

                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_REGISTRO_CONSERVAZIONE");

                string field;
                if (!dbProvider.ExecuteScalar(out field, commandText))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                retValue = field;
            }

            return retValue;
        }



        /// <summary>
        /// Metodo che ritorna la segnatura o il docNumber di un documento.
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public string getSegnatura_Id(string idProfile)
        {
            string result = string.Empty;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {


                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SEGNATURA_ID_DOC");

                    q.setParam("idDoc", idProfile);

                    string commandText = q.getSQL();
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        result = ds.Tables[0].Rows[0][0].ToString();
                    }
                }

            }
            catch
            {
                logger.Debug("Errore nella ricerca della segnatura/id del documento - getSegnatura_Id)");
                result = "Errore";
            }

            return result;
        }

        #endregion

        //
        // Mev Cs 1.4 - Esibizione
        /// <summary>
        /// Metodo per l'inserimento di documenti / fascicoli in area esibizione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="idProfile"></param>
        /// <param name="idProject"></param>
        /// <param name="docNumber"></param>
        /// <param name="infoUtente"></param>
        /// <param name="tipoOggetto"></param>
        /// <returns></returns>
        public string addAreaEsibizione(string idEsibizione,
                                            string idProfile,
                                            string idProject,
                                            string docNumber,
                                            DocsPaVO.utente.InfoUtente infoUtente,
                                            string tipoOggetto,
                                            string idConservazione,
                                            out DocsPaVO.documento.SchedaDocumento sd
                                            )
        {
            string result = String.Empty;

            if (idProject != null && idProject != String.Empty)
            {
                DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                //DocsPaVO.fascicolazione.Fascicolo fasc = fascicoli.GetFascicoloById(idProject, infoUtente);
                DocsPaVO.fascicolazione.Fascicolo fasc = fascicoli.getFascicoloByIdNoSecurity(idProject);

                result = this.addDocAreaEsibizione(idEsibizione, idProfile, idProject, docNumber, infoUtente, fasc.codice, tipoOggetto, idConservazione, out sd);
            }
            else
            {
                result = this.addDocAreaEsibizione(idEsibizione, idProfile, idProject, docNumber, infoUtente, "", tipoOggetto, idConservazione, out sd);
            }

            return result;
        }

        /// <summary>
        /// Metodo per l'inserimento di documenti / fascicoli in area esibizione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="idProfile"></param>
        /// <param name="idProject"></param>
        /// <param name="docNumber"></param>
        /// <param name="infoUtente"></param>
        /// <param name="codFasc"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="idPolicy"></param>
        /// <returns></returns>
        public string addDocAreaEsibizione(
                                            string idEsibizione,
                                            string idProfile,
                                            string idProject,
                                            string docNumber,
                                            DocsPaVO.utente.InfoUtente infoUtente,
                                            string codFasc,
                                            string tipoOggetto,
                                            string idConservazione,
                                            out DocsPaVO.documento.SchedaDocumento sd
            )
        {
            Documenti documentiDb = new Documenti();
            DocsPaVO.documento.SchedaDocumento schedaDoc = documentiDb.GetDettaglioNoSecurity(infoUtente, idProfile, docNumber);

            sd = schedaDoc;

            ArrayList parameters = new ArrayList();
            string result = String.Empty;
            //  string sysId = "";
            DBProvider dbProvider = new DBProvider();
            try
            {
                // Inizio Transazione
                dbProvider.BeginTransaction();

                string idPeople = infoUtente.idPeople;
                string idGruppo = infoUtente.idGruppo;
                string idAmm = infoUtente.idAmministrazione;
                string tipoProto = schedaDoc.tipoProto;
                string oggetto = schedaDoc.oggetto.descrizione;
                string userId = infoUtente.userId;
                string tipologiaDoc = string.Empty;
                if (schedaDoc.tipologiaAtto != null)
                    tipologiaDoc = schedaDoc.tipologiaAtto.descrizione;

                DocsPaVO.documento.TipologiaAtto tipoAtto = schedaDoc.tipologiaAtto;
                if (tipoAtto != null && tipoAtto.systemId.ToString() != String.Empty)
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelDB.getTemplate(docNumber);
                }

                parameters.Add(this.CreateParameter("idAmm", Convert.ToInt32(idAmm)));

                // Se l'idConservazione è stato fornito, il documento viene inserito in un'istanza già esistente
                if (string.IsNullOrEmpty(idEsibizione))
                    //parameters.Add(this.CreateParameter("idConservazione", DBNull.Value));
                    parameters.Add(this.CreateParameter("idEsibizione", DBNull.Value));
                else
                    //parameters.Add(this.CreateParameter("idConservazione", Convert.ToInt32(idEsibizione)));
                    parameters.Add(this.CreateParameter("idEsibizione", Convert.ToInt32(idEsibizione)));


                parameters.Add(this.CreateParameter("idPeople", Convert.ToInt32(idPeople)));
                if (idProfile != null && idProfile != String.Empty)
                {
                    parameters.Add(this.CreateParameter("idProfile", Convert.ToInt32(idProfile)));
                }
                else
                {
                    parameters.Add(this.CreateParameter("idProfile", DBNull.Value));
                }
                if (idProject == null || idProject == String.Empty)
                {
                    parameters.Add(this.CreateParameter("idProject", DBNull.Value));
                    parameters.Add(this.CreateParameter("codFasc", DBNull.Value));
                }
                else
                {
                    parameters.Add(this.CreateParameter("idProject", Convert.ToInt32(idProject)));
                    parameters.Add(this.CreateParameter("codFasc", codFasc));
                }
                parameters.Add(this.CreateParameter("oggetto", oggetto));
                parameters.Add(this.CreateParameter("tipoDoc", tipoProto));
                // parameters.Add(this.CreateParameter("sizeItem", total_size));

                parameters.Add(this.CreateParameter("idGruppo", Convert.ToInt32(idGruppo)));
                if (schedaDoc.registro != null)
                {
                    if (schedaDoc.registro.systemId != String.Empty)
                    {
                        parameters.Add(this.CreateParameter("idRegistro", Convert.ToInt32(schedaDoc.registro.systemId)));
                    }
                }
                else
                {
                    parameters.Add(this.CreateParameter("idRegistro", DBNull.Value));
                }

                parameters.Add(this.CreateParameter("docNumber", Convert.ToInt32(schedaDoc.docNumber)));

                parameters.Add(this.CreateParameter("userId", userId));

                parameters.Add(this.CreateParameter("tipoOggetto", tipoOggetto));

                parameters.Add(this.CreateParameter("tipoAtto", tipologiaDoc));

                parameters.Add(this.CreateParameter("idconservazione", idConservazione));

                DocsPaUtils.Data.ParameterSP versionIdParam = new DocsPaUtils.Data.ParameterSP("result", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                parameters.Add(versionIdParam);

                /*
                foreach (DocsPaUtils.Data.ParameterSP al in parameters)
                {
                    logger.DebugFormat("{0} -> {1}", al.Nome, al.Valore);
                }
                */
                DataSet ds = null;
                //logger.DebugFormat("Chiamo > SP_INSERT_AREA_ESIB");
                if (dbProvider.ExecuteStoredProcedure("SP_INSERT_AREA_ESIB", parameters, ds) != -1)
                {
                    result = Convert.ToString(versionIdParam.Valore);
                    //Commit
                    dbProvider.CommitTransaction();
                    // logger.DebugFormat("SP_INSERT_AREA_ESIB-> Result {0}", result);
                    //  dbProvider.SetLargeText("DPA_ITEMS_CONSERVAZIONE", sysId, "VAR_XML_METADATI", xmlMetadati[1]);
                }
                else
                {
                    result = "-1";
                    dbProvider.RollbackTransaction();
                }
            }
            catch
            {
                result = "-1";
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
            return result;
        }

        // Aggiornamento dimensione item esibizione
        /// <summary>
        /// Aggiornamento dimensione item esibizione
        /// </summary>
        /// <param name="size"></param>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public bool insertSizeInItemsEsib(int size, string sysId)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_SizeItemEsib");
            bool result = true;
            try
            {
                q.setParam("size", Convert.ToString(size));
                q.setParam("sysId", "'" + sysId + "'");
                string queryString = q.getSQL();
                logger.Debug(queryString);
                using (DBProvider dbProvider = new DBProvider())
                    dbProvider.ExecuteNonQuery(queryString);
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Esibizione (query- U_SizeItemEsib)");
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Aggiornamento informazioni item di esibizione
        /// </summary>
        /// <param name="tipoFile"></param>
        /// <param name="numAllegati"></param>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public bool updateItemsEsib(string tipoFile, string numAllegati, string sysId)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ITEMS_ESIB");
            bool result = true;
            try
            {
                string setParam = " VAR_TIPO_FILE='" + tipoFile + "'," +
                                  " NUMERO_ALLEGATI='" + numAllegati + "'";

                string whereParam = " SYSTEM_ID='" + sysId + "'";

                q.setParam("param1", setParam);
                q.setParam("param2", whereParam);
                string queryString = q.getSQL();
                logger.Debug("updateItemsEsib " + queryString);

                using (DBProvider dbProvider = new DBProvider())
                    dbProvider.ExecuteNonQuery(queryString);
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Esibizione (query- U_DPA_ITEMS_ESIB)");
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Metodo per verificare la presenza di un item esibizione con idProfile in un'istanza di esibizione per lo stesso idPeople e idGruppo proveniente dalla stessa istanza di conservazione
        /// </summary>
        /// <param name="id_profile"></param>
        /// <param name="id_project"></param>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanzaConservazione"></param>
        /// <returns></returns>
        public bool checkItemEsibizionePresenteInIstanzaEsibizione(string id_profile, string id_project, string type, DocsPaVO.utente.InfoUtente infoUtente, string idIstanzaConservazione)
        {
            // Invocare la funzione DB getInEsibizione(idProfile, idproject, typeID, idPeople, idGruppo, idIstanzaConservazione)

            bool result = false;
            string inEsibizione = string.Empty;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_getInEsibizione");

            try
            {
                string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
                if (dbType.ToUpper() == "SQL")
                    q.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                //idProfile:
                q.setParam("idProfile", id_profile);

                //idProject
                if (string.IsNullOrEmpty(id_project))
                    q.setParam("idProject", "NULL");
                else
                    q.setParam("idProject", id_project);

                //Type: D o F
                q.setParam("typeId", type);

                //idPeople
                q.setParam("idPeople", infoUtente.idPeople);

                //idGruppo
                q.setParam("idGruppo", infoUtente.idGruppo);

                //idGruppo
                q.setParam("idIstanzaConservazione", idIstanzaConservazione);

                string queryString = q.getSQL();
                logger.Debug("S_getInEsibizione " + queryString);

                using (DBProvider dbProvider = new DBProvider())
                    dbProvider.ExecuteScalar(out inEsibizione, queryString);

                if (!string.IsNullOrEmpty(inEsibizione))
                    result = inEsibizione.Equals("1") ? true : false;
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Esibizione (query- S_getInEsibizione)");
                result = false;
            }
            return result;
        }
        // End Mev Cs 1.4 - Esibizione
        //


        // Start MEV 1.5 F02_01 - report conservazione
        #region conservazione MEV 1.5 - report conservazione
        //reperimento item conservazionegetItemsConservazioneById
        public List<ItemsConservazione> getItemsConservazioneByIdCons(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("getItemsConservazioneByIdCons start");
            //risultato
            List<ItemsConservazione> retValue = new List<ItemsConservazione>();

            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE1");
            string fields_itemsCons = "SYSTEM_ID AS ID," +
                                   "ID_CONSERVAZIONE AS CONSERVAZIONE," +
                                   "ID_PROFILE AS PROFILE," +
                                   "ID_PROJECT AS PROJECT," +
                                   "CHA_TIPO_DOC AS TIPO_DOC," +
                                   "VAR_OGGETTO AS OGGETTO," +
                                   "ID_REGISTRO AS REGISTRO," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_INS", true) + " AS INSERIMENTO," +
                                   "CHA_STATO AS STATO," +
                                   "SIZE_ITEM AS DIMENSIONE," +
                                   "COD_FASC AS CODFASC," +
                                   "DOCNUMBER AS DOCNUM," +
                                   "VAR_TIPO_FILE AS TIPO_FILE," +
                                   "NUMERO_ALLEGATI," +
                                   "CHA_TIPO_OGGETTO AS TIPO_OGGETTO," +
                                   "CHA_ESITO AS ESITO, " +
                                   "VAR_TIPO_ATTO as TIPO_ATTO, " +
                                   "VALIDAZIONE_FIRMA, POLICY_VALIDA, ";
            queryDef1.setParam("param1", fields_itemsCons);
            fields_itemsCons = "FROM DPA_ITEMS_CONSERVAZIONE";
            queryDef1.setParam("param2", fields_itemsCons);
            fields_itemsCons = "WHERE ID_CONSERVAZIONE = " + idConservazione + " ORDER BY CODFASC";
            queryDef1.setParam("param3", fields_itemsCons);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {

                            ItemsConservazione itemsCons = new ItemsConservazione();
                            itemsCons.SystemID = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                            itemsCons.ID_Conservazione = reader.GetValue(reader.GetOrdinal("CONSERVAZIONE")).ToString();
                            itemsCons.ID_Profile = reader.GetValue(reader.GetOrdinal("PROFILE")).ToString();
                            itemsCons.ID_Project = reader.GetValue(reader.GetOrdinal("PROJECT")).ToString();
                            itemsCons.TipoDoc = reader.GetValue(reader.GetOrdinal("TIPO_DOC")).ToString();
                            itemsCons.desc_oggetto = reader.GetValue(reader.GetOrdinal("OGGETTO")).ToString();
                            itemsCons.ID_Registro = reader.GetValue(reader.GetOrdinal("REGISTRO")).ToString();
                            itemsCons.Data_Ins = reader.GetValue(reader.GetOrdinal("INSERIMENTO")).ToString();
                            itemsCons.StatoConservazione = reader.GetValue(reader.GetOrdinal("STATO")).ToString();
                            itemsCons.SizeItem = reader.GetValue(reader.GetOrdinal("DIMENSIONE")).ToString();
                            itemsCons.CodFasc = reader.GetValue(reader.GetOrdinal("CODFASC")).ToString();
                            itemsCons.DocNumber = reader.GetValue(reader.GetOrdinal("DOCNUM")).ToString();
                            itemsCons.numProt_or_id = reader.GetValue(reader.GetOrdinal("SEGNATURA")).ToString();
                            itemsCons.data_prot_or_create = reader.GetValue(reader.GetOrdinal("DATA_PROT_OR_CREA")).ToString();
                            itemsCons.numProt = reader.GetValue(reader.GetOrdinal("NUM_PROT")).ToString();
                            itemsCons.tipoFile = reader.GetValue(reader.GetOrdinal("TIPO_FILE")).ToString();
                            itemsCons.numAllegati = reader.GetValue(reader.GetOrdinal("NUMERO_ALLEGATI")).ToString();
                            itemsCons.immagineAcquisita = reader.GetValue(reader.GetOrdinal("IMG_ACQUISITA")).ToString();
                            itemsCons.tipo_oggetto = reader.GetValue(reader.GetOrdinal("TIPO_OGGETTO")).ToString();
                            itemsCons.esitoLavorazione = reader.GetValue(reader.GetOrdinal("ESITO")).ToString();
                            itemsCons.tipo_atto = reader.GetValue(reader.GetOrdinal("TIPO_ATTO")).ToString();
                            itemsCons.policyValida = reader.GetValue(reader.GetOrdinal("POLICY_VALIDA")).ToString();
                            if (!string.IsNullOrEmpty(itemsCons.tipo_atto))
                            {
                                DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                                itemsCons.template = model.getTemplateDettagli(itemsCons.DocNumber);
                            }

                            retValue.Add(itemsCons);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                retValue = null;
            }
            return retValue;

        }


        /// <summary>
        /// insert di una riga del report di un'istanza di conservazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="descrizione"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public string InsertItemReportFormatiConservazione(String id_Istanza,
                                                    String id_Item,
                                                    String docNumber,
                                                    String id_Project,
                                                    String version_Id,
                                                    String tipoFile,
                                                    String esito,
                                                    String convertibile,
                                                    String modifica,
                                                    String ut_Prop,
                                                    String ruolo_Prop)
        {
            string retValue = string.Empty;
            //modifica sab per gestione valore di ritorno (scope_identity), è una schifezza, ma al momento non è gestito nulla in transazione;
            //DocsPaDB.DBProvider dbProvider = new DBProvider();
            //dbProvider.BeginTransaction();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_INSERT_VERIFICA_FORMATI_CONS");

                queryDef.setParam("SYSTEM_ID", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_VERIFICA_FORMATI_CONS"));
                queryDef.setParam("ID_ISTANZA", id_Istanza);

                queryDef.setParam("ID_ITEM", id_Item);
                queryDef.setParam("DOCNUMBER", docNumber);
                queryDef.setParam("ID_PROJECT", id_Project);
                queryDef.setParam("VERSION_ID", version_Id);
                queryDef.setParam("TIPO_FILE", tipoFile);
                queryDef.setParam("ESITO", esito);
                queryDef.setParam("CONVERTIBILE", convertibile);
                queryDef.setParam("MODIFICA", modifica);
                queryDef.setParam("UT_PROP", ut_Prop);
                queryDef.setParam("RUOLO_PROP", ruolo_Prop);

                string commandText = queryDef.getSQL();
                logger.InfoFormat("I_INSERT_VERIFICA_FORMATI_CONS: {0}", commandText);

                logger.Debug("Inserimento riga del report dell'instanza di conservazione: ");
                logger.Debug(commandText);
                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_VERIFICA_FORMATI_CONS");

                //dbProvider.CommitTransaction();
                retValue = commandText;
            }

            return retValue;
        }

        /// <summary>
        /// insert di una riga del report di un'istanza di conservazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="descrizione"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public bool InsertItemReportFormatiConservazione(ReportFormatiConservazione elemento)
        {
            bool retValue = false;
            //modifica sab per gestione valore di ritorno (scope_identity), è una schifezza, ma al momento non è gestito nulla in transazione;
            //DocsPaDB.DBProvider dbProvider = new DBProvider();
            //dbProvider.BeginTransaction();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_INSERT_VERIFICA_FORMATI_CONS");
                string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
                if (dbType.ToUpper() == "SQL")
                {
                    queryDef.setParam("ColSYSTEM_ID", "");
                    queryDef.setParam("SYSTEM_ID", "");
                }
                else
                {
                    queryDef.setParam("ColSYSTEM_ID", "SYSTEM_ID");
                    queryDef.setParam("SYSTEM_ID", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_VERIFICA_FORMATI_CONS"));
                }
                queryDef.setParam("ID_ISTANZA", elemento.ID_Istanza);
                queryDef.setParam("ID_ITEM", !string.IsNullOrEmpty(elemento.ID_Item) ? elemento.ID_Item : "NULL");
                queryDef.setParam("DOCNUMBER", elemento.DocNumber);
                queryDef.setParam("ID_PROJECT", !string.IsNullOrEmpty(elemento.ID_Project) ? elemento.ID_Project : "NULL");
                queryDef.setParam("ID_DOCPRINCIPALE", !string.IsNullOrEmpty(elemento.ID_DocPrincipale) ? elemento.ID_DocPrincipale : "NULL");
                queryDef.setParam("VERSION_ID", elemento.Version_ID);
                queryDef.setParam("TIPO_FILE", elemento.TipoFile);
                queryDef.setParam("ESTENSIONE", elemento.Estensione);
                queryDef.setParam("AMMESSO", !string.IsNullOrEmpty(elemento.Ammesso) ? elemento.Ammesso : "NULL");
                queryDef.setParam("VALIDO", !string.IsNullOrEmpty(elemento.Valido) ? elemento.Valido : "NULL");
                queryDef.setParam("CONSOLIDATO", !string.IsNullOrEmpty(elemento.Consolidato) ? elemento.Consolidato : "NULL");
                queryDef.setParam("ESITO", !string.IsNullOrEmpty(elemento.Esito) ? elemento.Esito : string.Empty);
                queryDef.setParam("CONVERTIBILE", !string.IsNullOrEmpty(elemento.Convertibile) ? elemento.Convertibile : "NULL");
                queryDef.setParam("MODIFICA", !string.IsNullOrEmpty(elemento.Modifica) ? elemento.Modifica : "NULL");
                queryDef.setParam("UT_PROP", !string.IsNullOrEmpty(elemento.UtProp) ? elemento.UtProp : "NULL");
                queryDef.setParam("RUOLO_PROP", !string.IsNullOrEmpty(elemento.RuoloProp) ? elemento.RuoloProp : "NULL");
                queryDef.setParam("FIRMATA", !string.IsNullOrEmpty(elemento.Firmata) ? elemento.Firmata : "NULL");
                queryDef.setParam("MARCATA", !string.IsNullOrEmpty(elemento.Marcata) ? elemento.Marcata : "NULL");
                queryDef.setParam("DACONVERTIRE", !string.IsNullOrEmpty(elemento.DaConverire) ? elemento.DaConverire : "NULL");
                queryDef.setParam("CONVERTITO", !string.IsNullOrEmpty(elemento.Convertito) ? elemento.Convertito : "NULL");
                queryDef.setParam("ERRORE", !string.IsNullOrEmpty(elemento.Errore) ? elemento.Errore : "NULL");
                queryDef.setParam("TIPOERRORE", !string.IsNullOrEmpty(elemento.TipoErrore) ? elemento.TipoErrore : "NULL");

                string commandText = queryDef.getSQL();
                logger.InfoFormat("I_INSERT_VERIFICA_FORMATI_CONS: {0}", commandText);

                logger.Debug("Inserimento riga del report dell'instanza di conservazione: ");
                logger.Debug(commandText);
                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_VERIFICA_FORMATI_CONS");

                //dbProvider.CommitTransaction();
                retValue = !string.IsNullOrEmpty(commandText);
            }

            return retValue;
        }


        /// <summary>
        /// update report
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="descrizione"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public bool UpdateItemReportFormatiConservazione(ReportFormatiConservazione elemento)
        {
            bool retValue = false;


            //DocsPaDB.DBProvider dbProvider = new DBProvider();
            //dbProvider.BeginTransaction();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_VERIFICA_FORMATI_CONS");

                queryDef.setParam("SYSTEM_ID", elemento.System_ID);
                queryDef.setParam("ID_ISTANZA", elemento.ID_Istanza);
                queryDef.setParam("ID_ITEM", !string.IsNullOrEmpty(elemento.ID_Item) ? elemento.ID_Item : "NULL");
                queryDef.setParam("DOCNUMBER", elemento.DocNumber);
                queryDef.setParam("ID_PROJECT", !string.IsNullOrEmpty(elemento.ID_Project) ? elemento.ID_Project : "NULL");
                queryDef.setParam("ID_DOCPRINCIPALE", !string.IsNullOrEmpty(elemento.ID_DocPrincipale) ? elemento.ID_DocPrincipale : "NULL");
                queryDef.setParam("VERSION_ID", elemento.Version_ID);
                queryDef.setParam("TIPO_FILE", elemento.TipoFile);
                queryDef.setParam("ESTENSIONE", elemento.Estensione);
                queryDef.setParam("AMMESSO", !string.IsNullOrEmpty(elemento.Ammesso) ? elemento.Ammesso : "NULL");
                queryDef.setParam("VALIDO", !string.IsNullOrEmpty(elemento.Valido) ? elemento.Valido : "NULL");
                queryDef.setParam("CONSOLIDATO", !string.IsNullOrEmpty(elemento.Consolidato) ? elemento.Consolidato : "NULL");
                queryDef.setParam("ESITO", !string.IsNullOrEmpty(elemento.Esito) ? elemento.Esito : string.Empty);
                queryDef.setParam("CONVERTIBILE", !string.IsNullOrEmpty(elemento.Convertibile) ? elemento.Convertibile : "NULL");
                queryDef.setParam("MODIFICA", !string.IsNullOrEmpty(elemento.Modifica) ? elemento.Modifica : "NULL");
                queryDef.setParam("UT_PROP", !string.IsNullOrEmpty(elemento.UtProp) ? elemento.UtProp : "NULL");
                queryDef.setParam("RUOLO_PROP", !string.IsNullOrEmpty(elemento.RuoloProp) ? elemento.RuoloProp : "NULL");
                queryDef.setParam("FIRMATA", !string.IsNullOrEmpty(elemento.Firmata) ? elemento.Firmata : "NULL");
                queryDef.setParam("MARCATA", !string.IsNullOrEmpty(elemento.Marcata) ? elemento.Marcata : "NULL");
                queryDef.setParam("DACONVERTIRE", !string.IsNullOrEmpty(elemento.DaConverire) ? elemento.DaConverire : "NULL");
                queryDef.setParam("CONVERTITO", !string.IsNullOrEmpty(elemento.Convertito) ? elemento.Convertito : "NULL");
                queryDef.setParam("ERRORE", !string.IsNullOrEmpty(elemento.Errore) ? elemento.Errore : "NULL");
                queryDef.setParam("TIPOERRORE", !string.IsNullOrEmpty(elemento.TipoErrore) ? elemento.TipoErrore : "NULL");

                string commandText = queryDef.getSQL();
                logger.InfoFormat("U_UPDATE_VERIFICA_FORMATI_CONS: {0}", commandText);

                logger.Debug("Aggiornamento della riga del report dell'instanza di conservazione: ");
                logger.Debug(commandText);
                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                //commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_VERIFICA_FORMATI_CONS");

                //dbProvider.CommitTransaction();
                retValue = rowsAffected > 0;
            }

            return retValue;
        }


        public bool DeleteItemReportFormatiConservazioneByIdIstCons(string idCons)
        {
            bool retValue = false;
            string query;
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("D_DELETE_VERIFICA_FORMATI_CONS_BY_ID_ISTANZA");

                //queryDef.setParam("@ID_ISTANZA@", idCons);
                queryDef.setParam("ID_ISTANZA", idCons);
                query = queryDef.getSQL();
                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(query, out rowsAffected))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                retValue = rowsAffected >= 0;
            }
            return retValue;
        }

        public List<ReportFormatiConservazione> getItemReportFormatiConservazioneByIdIstCons(string idCons)
        {
            List<ReportFormatiConservazione> retValue = new List<ReportFormatiConservazione>();
            string query;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_SELECT_VERIFICA_FORMATI_CONS_BY_ID_ISTANZA");

                    queryDef.setParam("ID_ISTANZA", idCons);
                    query = queryDef.getSQL();
                    logger.Debug(string.Format("metodo getItemReportFormatiConservazioneByIdIstCons- Query {0}", query));

                    using (System.Data.IDataReader dr = dbProvider.ExecuteReader(query))
                    {
                        int System_IDOrdinal = dr.GetOrdinal("System_ID");
                        int ID_IstanzaOrdinal = dr.GetOrdinal("ID_Istanza");
                        int ID_ItemOrdinal = dr.GetOrdinal("ID_Item");
                        int DocNumberOrdinal = dr.GetOrdinal("DocNumber");
                        int ID_ProjectOrdinal = dr.GetOrdinal("ID_Project");
                        int ID_DocPrincipaleOrdinal = dr.GetOrdinal("ID_DocPrincipale");
                        int Version_IDOrdinal = dr.GetOrdinal("Version_ID");
                        int TipoFileOrdinal = dr.GetOrdinal("Tipo_File");
                        int EstensioneOrdinal = dr.GetOrdinal("Estensione");
                        int ConsolidatoOrdinal = dr.GetOrdinal("Consolidato");
                        int EsitoOrdinal = dr.GetOrdinal("Esito");
                        int ConvertibileOrdinal = dr.GetOrdinal("Convertibile");
                        int ModificaOrdinal = dr.GetOrdinal("Modifica");
                        int UtPropOrdinal = dr.GetOrdinal("Ut_Prop");
                        int RuoloPropOrdinal = dr.GetOrdinal("Ruolo_Prop");
                        int ValidoOrdinal = dr.GetOrdinal("Valido");
                        int AmmessoOrdinal = dr.GetOrdinal("Ammesso");
                        int FirmataOrdinal = dr.GetOrdinal("Firmata");
                        int MarcataOrdinal = dr.GetOrdinal("Marcata");
                        int DaConverireOrdinal = dr.GetOrdinal("DaConvertire");
                        int ConvertitoOrdinal = dr.GetOrdinal("Convertito");
                        int ErroreOrdinal = dr.GetOrdinal("Errore");
                        int TipoErroreOrdinal = dr.GetOrdinal("TipoErrore");

                        while (dr.Read())
                        {
                            retValue.Add(new ReportFormatiConservazione(
                                        dr.GetInt32(System_IDOrdinal).ToString(),
                                        dr.GetInt32(ID_IstanzaOrdinal).ToString(),
                                        dr.IsDBNull(ID_ItemOrdinal) ? string.Empty : dr.GetInt32(ID_ItemOrdinal).ToString(),
                                        dr.GetInt32(DocNumberOrdinal).ToString(),
                                        dr.IsDBNull(ID_ProjectOrdinal) ? string.Empty : dr.GetInt32(ID_ProjectOrdinal).ToString(),
                                        dr.GetInt32(ID_DocPrincipaleOrdinal).ToString(),
                                        dr.GetInt32(Version_IDOrdinal).ToString(),
                                        dr.IsDBNull(TipoFileOrdinal) ? string.Empty : dr.GetString(TipoFileOrdinal),
                                        dr.IsDBNull(EstensioneOrdinal) ? string.Empty : dr.GetString(EstensioneOrdinal),
                                        dr.IsDBNull(AmmessoOrdinal) ? string.Empty : dr.GetInt32(AmmessoOrdinal).ToString(),
                                        dr.IsDBNull(ValidoOrdinal) ? string.Empty : dr.GetInt32(ValidoOrdinal).ToString(),
                                        dr.IsDBNull(ConsolidatoOrdinal) ? string.Empty : dr.GetInt32(ConsolidatoOrdinal).ToString(),
                                        dr.IsDBNull(ConvertibileOrdinal) ? string.Empty : dr.GetInt32(ConvertibileOrdinal).ToString(),
                                        dr.IsDBNull(FirmataOrdinal) ? string.Empty : dr.GetInt32(FirmataOrdinal).ToString(),
                                        dr.IsDBNull(MarcataOrdinal) ? string.Empty : dr.GetInt32(MarcataOrdinal).ToString(),
                                        dr.IsDBNull(ModificaOrdinal) ? string.Empty : dr.GetInt32(ModificaOrdinal).ToString(),
                                        dr.IsDBNull(UtPropOrdinal) ? string.Empty : dr.GetInt32(UtPropOrdinal).ToString(),
                                        dr.IsDBNull(RuoloPropOrdinal) ? string.Empty : dr.GetInt32(RuoloPropOrdinal).ToString(),
                                        dr.IsDBNull(EsitoOrdinal) ? string.Empty : dr.GetString(EsitoOrdinal).ToString(),
                                        dr.IsDBNull(DaConverireOrdinal) ? string.Empty : dr.GetInt32(DaConverireOrdinal).ToString(),
                                        dr.IsDBNull(ConvertitoOrdinal) ? string.Empty : dr.GetInt32(ConvertitoOrdinal).ToString(),
                                        dr.IsDBNull(ErroreOrdinal) ? string.Empty : dr.GetInt32(ErroreOrdinal).ToString(),
                                        dr.IsDBNull(TipoErroreOrdinal) ? string.Empty : dr.GetInt32(TipoErroreOrdinal).ToString()));

                        }

                        dr.Close();

                    }

                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore metodo getItemReportFormatiConservazioneByIdIstCons", e);
                retValue = null;
            }
            return retValue;
        }

        public bool updateStatoConservazione(string idConservazione, string stato)
        {
            bool retValue = false;
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                //DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_STATO_ISTANZA_CONSERVAZIONE");
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_STATO_ISTANZA_CONSERVAZIONE_INVIO");

                queryDef.setParam("stato", stato);
                queryDef.setParam("systemId", idConservazione);


                if (stato.Equals("C") || stato.Equals("V"))//== StatoIstanza.CHIUSA)
                    queryDef.setParam("dataConservazione", DocsPaDbManagement.Functions.Functions.GetDate());
                else
                    queryDef.setParam("dataConservazione", "NULL");

                // GM 10-12-2013
                // per bug mancata valorizzazione data invio rigenerazione istanze/invio tramite policy
                if (stato.Equals("I"))
                    queryDef.setParam("dataInvio", ", DATA_INVIO=" + DocsPaDbManagement.Functions.Functions.GetDate());
                else
                    queryDef.setParam("dataInvio", string.Empty);
                // fine modifica

                string commandText = queryDef.getSQL();
                logger.InfoFormat("U_UPDATE_STATO_ISTANZA_CONSERVAZIONE: {0}", commandText);

                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_STATO_ITEMS_CONSERVAZIONE");
                queryDef.setParam("stato", stato);
                queryDef.setParam("systemId", idConservazione);

                commandText = queryDef.getSQL();

                logger.InfoFormat("U_UPDATE_STATO_ITEMS_CONSERVAZIONE: {0}", commandText);
                dbProvider.ExecuteNonQuery(commandText, out rowsAffected);


                retValue = rowsAffected > 0;
            }
            return retValue;
        }

        public bool updateStatoConservazione(string idConservazione, string stato, string tipo_cons, string note, string descr, string idTipoSupp, bool consolida)
        {
            bool retValue = false;
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_STATO_ISTANZA_CONSERVAZIONE2");

                queryDef.setParam("stato", stato);
                queryDef.setParam("systemId", idConservazione);
                queryDef.setParam("VAR_TIPO_CONS", tipo_cons);
                queryDef.setParam("VAR_NOTE", note);
                queryDef.setParam("VAR_DESCRIZIONE", descr);
                queryDef.setParam("VAR_TIPO_SUPPORTO", idTipoSupp);
                if (consolida)
                {
                    queryDef.setParam("CONSOLIDA", "1");
                }
                else
                {
                    queryDef.setParam("CONSOLIDA", "''");
                }

                if (stato.Equals("C"))//== StatoIstanza.CHIUSA)
                    queryDef.setParam("dataConservazione", DocsPaDbManagement.Functions.Functions.GetDate());
                else
                    queryDef.setParam("dataConservazione", "NULL");

                // GM 10-12-2013
                // per bug mancata valorizzazione data invio rigenerazione istanze/invio tramite policy
                if (stato.Equals("I"))
                    queryDef.setParam("dataInvio", ", DATA_INVIO=" + DocsPaDbManagement.Functions.Functions.GetDate());
                else
                    queryDef.setParam("dataInvio", string.Empty);
                // fine modifica

                string commandText = queryDef.getSQL();
                logger.InfoFormat("U_UPDATE_STATO_ISTANZA_CONSERVAZIONE: {0}", commandText);

                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_STATO_ITEMS_CONSERVAZIONE");
                queryDef.setParam("stato", stato);
                queryDef.setParam("systemId", idConservazione);

                commandText = queryDef.getSQL();

                logger.InfoFormat("U_UPDATE_STATO_ITEMS_CONSERVAZIONE: {0}", commandText);
                dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                retValue = rowsAffected > 0;
            }
            return retValue;
        }

        public bool updateStatoConservazioneWhithEsitoVerifica(string idConservazione, string stato, int esitoVerifica)
        {
            bool retValue = false;
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_STATO_ISTANZA_CONSERVAZIONE_WITH_ESITO_VERIFICA");

                queryDef.setParam("stato", stato);
                queryDef.setParam("systemId", idConservazione);
                queryDef.setParam("esitoVerifica", esitoVerifica.ToString());

                if (stato.Equals("C"))//== StatoIstanza.CHIUSA)
                    queryDef.setParam("dataConservazione", DocsPaDbManagement.Functions.Functions.GetDate());
                else
                    queryDef.setParam("dataConservazione", "NULL");

                // GM 10-12-2013
                // per bug mancata valorizzazione data invio rigenerazione istanza/invio tramite policy
                if (stato.Equals("I"))
                    queryDef.setParam("dataInvio", DocsPaDbManagement.Functions.Functions.GetDate());
                else
                    queryDef.setParam("dataInvio", string.Empty);

                string commandText = queryDef.getSQL();
                logger.InfoFormat("U_UPDATE_STATO_ISTANZA_CONSERVAZIONE_WITH_ESITO_VERIFICA: {0}", commandText);

                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_STATO_ITEMS_CONSERVAZIONE");
                queryDef.setParam("stato", stato);
                queryDef.setParam("systemId", idConservazione);

                commandText = queryDef.getSQL();

                logger.InfoFormat("U_UPDATE_STATO_ITEMS_CONSERVAZIONE: {0}", commandText);
                dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                retValue = rowsAffected > 0;
            }
            return retValue;
        }

        public bool checkItemConsolidato(string docNumber)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CHECK_ITEM_CONSOLIDATO");
            bool result = false;
            try
            {

                q.setParam("DocNumber", docNumber);

                string commandText = q.getSQL();
                DataSet ds = new DataSet();

                using (DBProvider dbProvider = new DBProvider())
                    dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    result = true;
                }

            }
            catch
            {
                logger.Debug("Errore controllo item conservazione in DocsPaDB conservazione : metodo checkItemConsolidato");
                result = false;
            }
            return result;
        }

        public ReportFormatiConservazione getReportFormatiConservazioneByDocNumber(string docNumber)
        {
            ReportFormatiConservazione retValue = null;
            string query;
            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_SELECT_VERIFICA_FORMATI_CONS_BY_DOCNUMBER");

                queryDef.setParam("DOCNUMBER", docNumber);
                query = queryDef.getSQL();
                logger.Debug(string.Format("metodo getReportFormatiConservazioneByDocNumber- Query {0}", query));
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader dr = dbProvider.ExecuteReader(query))
                    {
                        int System_IDOrdinal = dr.GetOrdinal("System_ID");
                        int ID_IstanzaOrdinal = dr.GetOrdinal("ID_Istanza");
                        int ID_ItemOrdinal = dr.GetOrdinal("ID_Item");
                        int DocNumberOrdinal = dr.GetOrdinal("DocNumber");
                        int ID_ProjectOrdinal = dr.GetOrdinal("ID_Project");
                        int ID_DocPrincipaleOrdinal = dr.GetOrdinal("ID_DocPrincipale");
                        int Version_IDOrdinal = dr.GetOrdinal("Version_ID");
                        int TipoFileOrdinal = dr.GetOrdinal("Tipo_File");
                        int EstensioneOrdinal = dr.GetOrdinal("Estensione");
                        int ConsolidatoOrdinal = dr.GetOrdinal("Consolidato");
                        int EsitoOrdinal = dr.GetOrdinal("Esito");
                        int ConvertibileOrdinal = dr.GetOrdinal("Convertibile");
                        int ModificaOrdinal = dr.GetOrdinal("Modifica");
                        int UtPropOrdinal = dr.GetOrdinal("Ut_Prop");
                        int RuoloPropOrdinal = dr.GetOrdinal("Ruolo_Prop");
                        int ValidoOrdinal = dr.GetOrdinal("Valido");
                        int AmmessoOrdinal = dr.GetOrdinal("Ammesso");
                        int FirmataOrdinal = dr.GetOrdinal("Firmata");
                        int MarcataOrdinal = dr.GetOrdinal("Marcata");
                        int DaConverireOrdinal = dr.GetOrdinal("DaConvertire");
                        int ConvertitoOrdinal = dr.GetOrdinal("Convertito");
                        int ErroreOrdinal = dr.GetOrdinal("Errore");
                        int TipoErroreOrdinal = dr.GetOrdinal("TipoErrore");

                        if (dr.Read())
                        {

                            retValue = new ReportFormatiConservazione(
                                        dr.GetInt32(System_IDOrdinal).ToString(),
                                        dr.GetInt32(ID_IstanzaOrdinal).ToString(),
                                        dr.IsDBNull(ID_ItemOrdinal) ? string.Empty : dr.GetInt32(ID_ItemOrdinal).ToString(),
                                        dr.GetInt32(DocNumberOrdinal).ToString(),
                                        dr.IsDBNull(ID_ProjectOrdinal) ? string.Empty : dr.GetInt32(ID_ProjectOrdinal).ToString(),
                                        dr.GetInt32(ID_DocPrincipaleOrdinal).ToString(),
                                        dr.GetInt32(Version_IDOrdinal).ToString(),
                                        dr.IsDBNull(TipoFileOrdinal) ? string.Empty : dr.GetString(TipoFileOrdinal),
                                        dr.IsDBNull(EstensioneOrdinal) ? string.Empty : dr.GetString(EstensioneOrdinal),
                                        dr.IsDBNull(AmmessoOrdinal) ? string.Empty : dr.GetInt32(AmmessoOrdinal).ToString(),
                                        dr.IsDBNull(ValidoOrdinal) ? string.Empty : dr.GetInt32(ValidoOrdinal).ToString(),
                                        dr.IsDBNull(ConsolidatoOrdinal) ? string.Empty : dr.GetInt32(ConsolidatoOrdinal).ToString(),
                                        dr.IsDBNull(ConvertibileOrdinal) ? string.Empty : dr.GetInt32(ConvertibileOrdinal).ToString(),
                                        dr.IsDBNull(FirmataOrdinal) ? string.Empty : dr.GetInt32(FirmataOrdinal).ToString(),
                                        dr.IsDBNull(MarcataOrdinal) ? string.Empty : dr.GetInt32(MarcataOrdinal).ToString(),
                                        dr.IsDBNull(ModificaOrdinal) ? string.Empty : dr.GetInt32(ModificaOrdinal).ToString(),
                                        dr.IsDBNull(UtPropOrdinal) ? string.Empty : dr.GetInt32(UtPropOrdinal).ToString(),
                                        dr.IsDBNull(RuoloPropOrdinal) ? string.Empty : dr.GetInt32(RuoloPropOrdinal).ToString(),
                                        dr.IsDBNull(EsitoOrdinal) ? string.Empty : dr.GetString(EsitoOrdinal).ToString(),
                                        dr.IsDBNull(DaConverireOrdinal) ? string.Empty : dr.GetInt32(DaConverireOrdinal).ToString(),
                                        dr.IsDBNull(ConvertitoOrdinal) ? string.Empty : dr.GetInt32(ConvertitoOrdinal).ToString(),
                                        dr.IsDBNull(ErroreOrdinal) ? string.Empty : dr.GetInt32(ErroreOrdinal).ToString(),
                                        dr.IsDBNull(TipoErroreOrdinal) ? string.Empty : dr.GetInt32(TipoErroreOrdinal).ToString());
                        }
                        dr.Close();


                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore metodo getItemReportFormatiConservazioneByIdIstCons", e);
                retValue = null;
            }
            return retValue;
        }

        public bool checkReportFormatiConservazioneAndUpdateStatoIstanzaCons(ReportFormatiConservazione document, string newVersionId)
        {
            bool result = false;
            try
            {
                logger.Debug("[sp_CheckConsConvAndUpdate]");
                //DataSet ds = new DataSet();

                ArrayList sp_params = new ArrayList();
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("p_id_istanza", document.ID_Istanza, 999999999, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("p_doc_Number", document.DocNumber, 999999999, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("p_newVersionId", newVersionId, 999999999, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                //sp_params.Add(new DocsPaUtils.Data.ParameterSP("returnvalue", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);

                //sp_params.Add(new DocsPaUtils.Data.ParameterSP("p_id_istanza", document.ID_Istanza));
                //sp_params.Add(new DocsPaUtils.Data.ParameterSP("p_doc_Number", document.DocNumber));
                //sp_params.Add(new DocsPaUtils.Data.ParameterSP("p_newVersionId", newVersionId));
                logger.Debug("id_istanza= " + document.ID_Istanza.ToString() + " doc_Number= " + document.DocNumber + " version_ID= " + newVersionId);
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoreProcedure("sp_CheckConsConvAndUpdate", sp_params);


                    // ds.Dispose();

                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore metodo checkReportFormatiConservazioneAndUpdateStatoIstanzaCons", e);
                result = false;
            }

            return result;
        }

        public List<ReportFormatiConservazione> getDettaglioListReportFormatiConservazioneByIdCons(string idIstanzaCons)
        {

            List<ReportFormatiConservazione> retValue = new List<ReportFormatiConservazione>();
            string query;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_SELECT_DETTAGLIO_VERIFICA_FORMATI_CONS_BY_ID_ISTANZA");

                    queryDef.setParam("ID_ISTANZA", idIstanzaCons);
                    query = queryDef.getSQL();
                    logger.Debug(string.Format("metodo getDettaglioListReportFormatiConservazioneByIdCons- Query {0}", query));

                    using (System.Data.IDataReader dr = dbProvider.ExecuteReader(query))
                    {
                        int System_IDOrdinal = dr.GetOrdinal("System_ID");
                        int ID_IstanzaOrdinal = dr.GetOrdinal("ID_Istanza");
                        int ID_ItemOrdinal = dr.GetOrdinal("ID_Item");
                        int DocNumberOrdinal = dr.GetOrdinal("DocNumber");
                        int ID_ProjectOrdinal = dr.GetOrdinal("ID_Project");
                        int ID_DocPrincipaleOrdinal = dr.GetOrdinal("ID_DocPrincipale");
                        int Version_IDOrdinal = dr.GetOrdinal("Version_ID");
                        int TipoFileOrdinal = dr.GetOrdinal("Tipo_File");
                        int EstensioneOrdinal = dr.GetOrdinal("Estensione");
                        int ConsolidatoOrdinal = dr.GetOrdinal("Consolidato");
                        int EsitoOrdinal = dr.GetOrdinal("Esito");
                        int ConvertibileOrdinal = dr.GetOrdinal("Convertibile");
                        int ModificaOrdinal = dr.GetOrdinal("Modifica");
                        int UtPropOrdinal = dr.GetOrdinal("Ut_Prop");
                        int RuoloPropOrdinal = dr.GetOrdinal("Ruolo_Prop");
                        int ValidoOrdinal = dr.GetOrdinal("Valido");
                        int AmmessoOrdinal = dr.GetOrdinal("Ammesso");
                        int FirmataOrdinal = dr.GetOrdinal("Firmata");
                        int MarcataOrdinal = dr.GetOrdinal("Marcata");
                        int DaConverireOrdinal = dr.GetOrdinal("DaConvertire");
                        int ConvertitoOrdinal = dr.GetOrdinal("Convertito");
                        int ErroreOrdinal = dr.GetOrdinal("Errore");
                        int TipoErroreOrdinal = dr.GetOrdinal("TipoErrore");

                        int var_segnaturaOrdinal = dr.GetOrdinal("VAR_SEGNATURA");
                        int OGGETTOOrdinal = dr.GetOrdinal("OGGETTO");
                        int CREATION_DATEOrdinal = dr.GetOrdinal("CREATION_DATE");
                        int DTA_PROTOOrdinal = dr.GetOrdinal("DTA_PROTO");
                        int CHA_TIPO_PROTOOrdinal = dr.GetOrdinal("CHA_TIPO_PROTO");
                        int DESCR_UTOrdinal = dr.GetOrdinal("DESCR_UT");
                        int DESCR_RUOLOOrdinal = dr.GetOrdinal("DESCR_RUOLO");

                        while (dr.Read())
                        {
                            retValue.Add(new ReportFormatiConservazione(
                                                         dr.GetValue(System_IDOrdinal).ToString(),
                                                         dr.GetValue(ID_IstanzaOrdinal).ToString(),
                                                         dr.IsDBNull(ID_ItemOrdinal) ? string.Empty : dr.GetValue(ID_ItemOrdinal).ToString(),
                                                         dr.GetValue(DocNumberOrdinal).ToString(),
                                                         dr.IsDBNull(ID_ProjectOrdinal) ? string.Empty : dr.GetValue(ID_ProjectOrdinal).ToString(),
                                                         dr.GetValue(ID_DocPrincipaleOrdinal).ToString(),
                                                         dr.GetValue(Version_IDOrdinal).ToString(),
                                                         dr.IsDBNull(TipoFileOrdinal) ? string.Empty : dr.GetString(TipoFileOrdinal),
                                                         dr.IsDBNull(EstensioneOrdinal) ? string.Empty : dr.GetString(EstensioneOrdinal),
                                                         dr.IsDBNull(AmmessoOrdinal) ? string.Empty : dr.GetValue(AmmessoOrdinal).ToString(),
                                                         dr.IsDBNull(ValidoOrdinal) ? string.Empty : dr.GetValue(ValidoOrdinal).ToString(),
                                                         dr.IsDBNull(ConsolidatoOrdinal) ? string.Empty : dr.GetValue(ConsolidatoOrdinal).ToString(),
                                                         dr.IsDBNull(ConvertibileOrdinal) ? string.Empty : dr.GetValue(ConvertibileOrdinal).ToString(),
                                                         dr.IsDBNull(FirmataOrdinal) ? string.Empty : dr.GetValue(FirmataOrdinal).ToString(),
                                                         dr.IsDBNull(MarcataOrdinal) ? string.Empty : dr.GetValue(MarcataOrdinal).ToString(),
                                                         dr.IsDBNull(ModificaOrdinal) ? string.Empty : dr.GetValue(ModificaOrdinal).ToString(),
                                                         dr.IsDBNull(UtPropOrdinal) ? string.Empty : dr.GetValue(UtPropOrdinal).ToString(),
                                                         dr.IsDBNull(RuoloPropOrdinal) ? string.Empty : dr.GetValue(RuoloPropOrdinal).ToString(),
                                                         dr.IsDBNull(EsitoOrdinal) ? string.Empty : dr.GetString(EsitoOrdinal).ToString(),
                                                         dr.IsDBNull(DaConverireOrdinal) ? string.Empty : dr.GetValue(DaConverireOrdinal).ToString(),
                                                         dr.IsDBNull(ConvertitoOrdinal) ? string.Empty : dr.GetValue(ConvertitoOrdinal).ToString(),
                                                         dr.IsDBNull(ErroreOrdinal) ? string.Empty : dr.GetValue(ErroreOrdinal).ToString(),
                                                         dr.IsDBNull(TipoErroreOrdinal) ? string.Empty : dr.GetValue(TipoErroreOrdinal).ToString(),

                                                         dr.IsDBNull(var_segnaturaOrdinal) ? string.Empty : dr.GetString(var_segnaturaOrdinal),
                                                         dr.IsDBNull(OGGETTOOrdinal) ? string.Empty : dr.GetString(OGGETTOOrdinal),
                                                         dr.IsDBNull(CREATION_DATEOrdinal) ? string.Empty : dr.GetDateTime(CREATION_DATEOrdinal).ToString(),
                                                         dr.IsDBNull(DTA_PROTOOrdinal) ? string.Empty : dr.GetDateTime(DTA_PROTOOrdinal).ToString(),
                                                         dr.IsDBNull(CHA_TIPO_PROTOOrdinal) ? string.Empty : dr.GetString(CHA_TIPO_PROTOOrdinal),
                                                         dr.IsDBNull(DESCR_UTOrdinal) ? string.Empty : dr.GetString(DESCR_UTOrdinal),
                                                         dr.IsDBNull(DESCR_RUOLOOrdinal) ? string.Empty : dr.GetString(DESCR_RUOLOOrdinal)));

                        }

                        dr.Close();

                    }

                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore metodo getItemReportFormatiConservazioneByIdIstCons", e);
                retValue = null;
            }
            return retValue;
        }
        #endregion









        #region CS 1.5 - Requisito F03_01
        /// <summary>
        /// Metodo per l'inserimento di un documento in area conservazione nel rispetto dei vincoli dell'istanza
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="idProfile"></param>
        /// <param name="idProject"></param>
        /// <param name="docNumber"></param>
        /// <param name="infoUtente"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="idPolicy"></param>
        /// <param name="numDocIstanzaViolato"></param>
        /// <param name="dimIstanzaViolato"></param>
        /// <returns></returns>
        public string addAreaConservazione_WithConstraint(string idConservazione,
                                                    string idProfile,
                                                    string idProject,
                                                    string docNumber,
                                                    DocsPaVO.utente.InfoUtente infoUtente,
                                                    string tipoOggetto,
                                                    string idPolicy,
                                                    bool numDocIstanzaViolato,
                                                    bool dimIstanzaViolato,
                                                    int vincoloDimIstanza,
                                                    int vincoloNumDocIstanza,
                                                    int sizeItem
                                                    )
        {
            string result = String.Empty;

            if (idProject != null && idProject != String.Empty)
            {
                // Recupero fascicolo
                DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                //DocsPaVO.fascicolazione.Fascicolo fasc = fascicoli.GetFascicoloById(idProject, infoUtente);
                DocsPaVO.fascicolazione.Fascicolo fasc = fascicoli.getFascicoloByIdNoSecurity(idProject);

                // Inserimento
                result = this.addDocAreaConservazione_WithConstraint(idConservazione, idProfile, idProject, docNumber, infoUtente, fasc.codice, tipoOggetto, idPolicy, numDocIstanzaViolato, dimIstanzaViolato, vincoloDimIstanza, vincoloNumDocIstanza, sizeItem);
            }
            else
            {
                // Inserimento
                result = this.addDocAreaConservazione_WithConstraint(idConservazione, idProfile, idProject, docNumber, infoUtente, "", tipoOggetto, idPolicy, numDocIstanzaViolato, dimIstanzaViolato, vincoloDimIstanza, vincoloNumDocIstanza, sizeItem);
            }

            return result;
        }

        /// <summary>
        /// Metodo che effettua la chiamata alla stored procedure SP_INS_AREA_CONS_VINCOLI
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="idProfile"></param>
        /// <param name="idProject"></param>
        /// <param name="docNumber"></param>
        /// <param name="infoUtente"></param>
        /// <param name="codFasc"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="idPolicy"></param>
        /// <param name="numDocIstanzaViolato"></param>
        /// <param name="dimIstanzaViolato"></param>
        /// <returns></returns>
        public string addDocAreaConservazione_WithConstraint(
                                                string idConservazione,
                                                string idProfile,
                                                string idProject,
                                                string docNumber,
                                                DocsPaVO.utente.InfoUtente infoUtente,
                                                string codFasc,
                                                string tipoOggetto,
                                                string idPolicy,
                                                bool numDocIstanzaViolato,
                                                bool dimIstanzaViolato,
                                                int vincoloDimIstanza,
                                                int vincoloNumDocIstanza,
                                                int sizeItem
                                                )
        {
            Documenti documentiDb = new Documenti();
            DocsPaVO.documento.SchedaDocumento schedaDoc = documentiDb.GetDettaglioNoSecurity(infoUtente, idProfile, docNumber);

            // Lista di Parametri da passare alla stored procedure
            ArrayList parameters = new ArrayList();

            string result = String.Empty;
            //  string sysId = "";
            DBProvider dbProvider = new DBProvider();
            try
            {
                // Reperimento parametri
                string idPeople = infoUtente.idPeople;
                string idGruppo = infoUtente.idGruppo;
                string idAmm = infoUtente.idAmministrazione;
                string tipoProto = schedaDoc.tipoProto;
                string oggetto = schedaDoc.oggetto.descrizione;
                string userId = infoUtente.userId;
                string tipologiaDoc = string.Empty;
                if (schedaDoc.tipologiaAtto != null)
                    tipologiaDoc = schedaDoc.tipologiaAtto.descrizione;

                DocsPaVO.documento.TipologiaAtto tipoAtto = schedaDoc.tipologiaAtto;
                if (tipoAtto != null && tipoAtto.systemId.ToString() != String.Empty)
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelDB.getTemplate(docNumber);
                }

                // popolo la lista dei parametri:
                // idAmm
                parameters.Add(this.CreateParameter("idAmm", Convert.ToInt32(idAmm)));

                // Se l'idConservazione è stato fornito, il documento viene inserito in un'istanza già esistente
                if (string.IsNullOrEmpty(idConservazione))
                    parameters.Add(this.CreateParameter("idConservazione", DBNull.Value));
                else
                    parameters.Add(this.CreateParameter("idConservazione", Convert.ToInt32(idConservazione)));

                // IdPeople
                parameters.Add(this.CreateParameter("idPeople", Convert.ToInt32(idPeople)));

                // IdProfile
                if (idProfile != null && idProfile != String.Empty)
                {
                    parameters.Add(this.CreateParameter("idProfile", Convert.ToInt32(idProfile)));
                }
                else
                {
                    parameters.Add(this.CreateParameter("idProfile", DBNull.Value));
                }

                // IdProject e codFasc
                if (idProject == null || idProject == String.Empty)
                {
                    parameters.Add(this.CreateParameter("idProject", DBNull.Value));
                    parameters.Add(this.CreateParameter("codFasc", DBNull.Value));
                }
                else
                {
                    parameters.Add(this.CreateParameter("idProject", Convert.ToInt32(idProject)));
                    parameters.Add(this.CreateParameter("codFasc", codFasc));
                }

                // Oggetto
                parameters.Add(this.CreateParameter("oggetto", oggetto));

                // Tipo Doc
                parameters.Add(this.CreateParameter("tipoDoc", tipoProto));
                // parameters.Add(this.CreateParameter("sizeItem", total_size));

                // idGruppo
                parameters.Add(this.CreateParameter("idGruppo", Convert.ToInt32(idGruppo)));

                // idRegistro
                if (schedaDoc.registro != null)
                {
                    if (schedaDoc.registro.systemId != String.Empty)
                    {
                        parameters.Add(this.CreateParameter("idRegistro", Convert.ToInt32(schedaDoc.registro.systemId)));
                    }
                }
                else
                {
                    parameters.Add(this.CreateParameter("idRegistro", DBNull.Value));
                }

                // DocNumber
                parameters.Add(this.CreateParameter("docNumber", Convert.ToInt32(schedaDoc.docNumber)));

                //UserId
                parameters.Add(this.CreateParameter("userId", userId));

                // TipoOggetto
                parameters.Add(this.CreateParameter("tipoOggetto", tipoOggetto));

                // TipoAtto
                parameters.Add(this.CreateParameter("tipoAtto", tipologiaDoc));

                // Identificativo della policy con cui è stata creata l'istanza di conservazione
                if (string.IsNullOrEmpty(idPolicy))
                    parameters.Add(this.CreateParameter("idPolicy", DBNull.Value));
                else
                    parameters.Add(this.CreateParameter("idPolicy", idPolicy));

                // vincoloDimensioneItsanzaViolato
                string dimIstViol = string.Empty;
                if (dimIstanzaViolato)
                    dimIstViol = "1";
                else
                    dimIstViol = "0";
                //parameters.Add(this.CreateParameter("dimIstanzaViolato", dimIstanzaViolato));
                parameters.Add(this.CreateParameter("dimIstanzaViolato", dimIstViol));

                // vincoloNumeroDocumentiIstanzaViolato
                string numDocIstViol = string.Empty;
                if (numDocIstanzaViolato)
                    numDocIstViol = "1";
                else
                    numDocIstViol = "0";
                //parameters.Add(this.CreateParameter("numDocIstanzaViolato", numDocIstanzaViolato));
                parameters.Add(this.CreateParameter("numDocIstanzaViolato", numDocIstViol));

                // Vincolo Dimensione Istanza
                parameters.Add(this.CreateParameter("vincoloDim", vincoloDimIstanza.ToString()));

                // Vincolo Numero Documenti Istanza
                parameters.Add(this.CreateParameter("vincoloNumDoc", vincoloNumDocIstanza.ToString()));

                // dimesione item
                parameters.Add(this.CreateParameter("sizeItem", sizeItem.ToString()));

                DocsPaUtils.Data.ParameterSP versionIdParam = new DocsPaUtils.Data.ParameterSP("result", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                parameters.Add(versionIdParam);

                // Log - Decomentare per loggare i parametri
                
                foreach (DocsPaUtils.Data.ParameterSP al in parameters)
                {
                    logger.DebugFormat("{0} -> {1}", al.Nome, al.Valore);
                }
                

                DataSet ds = null;
                logger.DebugFormat("Chiamo --> SP_INS_AREA_CONS_VINCOLI");
                // ExecuteStoredProcedure aggiunge la p_ a tutti i parametri
                if (dbProvider.ExecuteStoredProcedure("SP_INS_AREA_CONS_VINCOLI", parameters, ds) != -1)
                {
                    result = Convert.ToString(versionIdParam.Valore);
                    logger.DebugFormat("SP_INS_AREA_CONS_VINCOLI --> Result {0}", result);
                    //  dbProvider.SetLargeText("DPA_ITEMS_CONSERVAZIONE", sysId, "VAR_XML_METADATI", xmlMetadati[1]);
                }
                else
                {
                    result = "-1";
                }
            }
            catch
            {
                result = "-1";
            }
            finally
            {
                dbProvider.Dispose();
            }
            return result;
        }

        /// <summary>
        /// ritorna la somma delle dimensioni degli items presenti nell'items_conservazione con quell'idconservazione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        public string getSumSizeItem_byIdConservazione(string idConservazione)
        {
            string result = string.Empty;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SUM_SIZE_ITEMS_DPA_ITEMS_CONSERVAZIONE");

                    q.setParam("idConservazione", idConservazione);

                    string commandText = q.getSQL();
                    logger.DebugFormat("Query - S_SUM_SIZE_ITEMS_DPA_ITEMS_CONSERVAZIONE: {0}", commandText);
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        result = ds.Tables[0].Rows[0][0].ToString();
                    }
                }

            }
            catch
            {
                logger.Debug("Errore nella somma della dimensione degli item in una istanza di conservazione - getSumSizeItem_byIdConservazione)");
            }

            return result;
        }

        /// <summary>
        /// Ritorna il numero di documenti contenuti in una istanza di conservazione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        public string getCountNumDocInIstanza_byIdConservazione(string idConservazione)
        {
            string result = string.Empty;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_DPA_ITEMS_CONSERVAZIONE");

                    q.setParam("idConservazione", idConservazione);

                    string commandText = q.getSQL();
                    logger.DebugFormat("Query - S_COUNT_DPA_ITEMS_CONSERVAZIONE: {0}", commandText);
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        result = ds.Tables[0].Rows[0][0].ToString();
                    }
                }

            }
            catch
            {
                logger.Debug("Errore nelcount degli item in una istanza di conservazione - getCountNumDocInIstanza_byIdConservazione)");
            }

            return result;
        }

        #endregion

        // MEV CS 1.5 - Alert Conservazione

        /// <summary>
        /// Metodo per l'incremento dei contatori degli alert della conservazione
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="userid"></param>
        /// <param name="idgruppo"></param>
        /// <param name="codice"></param>
        /// <returns>
        /// 1: il contatore ha superato la soglia
        /// 0: il contatore non ha superato la soglia
        /// -1: errore nell'esecuzione della SP
        /// </returns>
        public string IncrementaContatoriAlertConservazione(DocsPaVO.utente.InfoUtente infoUtente, string codice)
        {
            string result;
            logger.Debug("Metodo IncrementaContatoriAlertConservazione");

            ArrayList parameters = new ArrayList();
            DBProvider dbProvider = new DBProvider();

            try
            {
                string idPeople = infoUtente.idPeople;
                string idGruppo = infoUtente.idGruppo;
                string idAmm = infoUtente.idAmministrazione;

                //impostazione parametri                
                parameters.Add(this.CreateParameter("userid", Convert.ToInt32(idPeople)));
                parameters.Add(this.CreateParameter("groupid", Convert.ToInt32(idGruppo)));
                parameters.Add(this.CreateParameter("ammid", Convert.ToInt32(idAmm)));
                parameters.Add(this.CreateParameter("operaz", codice));

                DocsPaUtils.Data.ParameterSP versionIdParam = new DocsPaUtils.Data.ParameterSP("risultato", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                parameters.Add(versionIdParam);

                //esecuzione SP
                if (dbProvider.ExecuteStoredProcedure("SP_GETCONTATOREALERTCONS", parameters, null) != -1)
                {
                    result = versionIdParam.Valore.ToString();
                }
                else
                {
                    result = "-1";
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                result = "-1";
            }
            finally
            {
                dbProvider.Dispose();
            }

            return result;

        }

        public InfoConservazione getInfoConsercazioneByidCons(string idConservazione)
        {
            string err = string.Empty;
            InfoConservazione retValue = new InfoConservazione();
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

            string fields_infoCons = "SYSTEM_ID AS ID," +
                                   "ID_AMM AS AMM," +
                                   "ID_PEOPLE AS PEOPLE," +
                                   "ID_RUOLO_IN_UO AS RUOLO," +
                                   "CHA_STATO AS STATO," +
                                   "ESITO_VERIFICA," +
                                   "VAR_TIPO_SUPPORTO AS SUPPORTO," +
                                   "VAR_NOTE AS NOTE," +
                                   "VAR_DESCRIZIONE AS DESCRIZIONE," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_APERTURA", true) + " AS APERTURA," +
                //"DATA_APERTURA AS APERTURA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_INVIO", true) + " AS INVIO," +
                //"DATA_INVIO AS INVIO," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_CONSERVAZIONE", true) + " AS CONSERVAZIONE," +
                //"DATA_CONSERVAZIONE AS CONSERVAZIONE," +
                                   "VAR_MARCA_TEMPORALE AS MARCA," +
                                   "VAR_FIRMA_RESPONSABILE AS FIRMA," +
                                   "VAR_LOCAZIONE_FISICA AS LOCAZIONE," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_PROX_VERIFICA", true) + " AS PROX_VERIFICA," +
                //"DATA_PROX_VERIFICA AS PROX_VERIFICA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_ULTIMA_VERIFICA", true) + " AS ULTIMA_VERIFICA," +
                //"DATA_ULTIMA_VERIFICA AS ULTIMA_VERIFICA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_RIVERSAMENTO", true) + " AS RIVERSAMENTO," +
                //"DATA_RIVERSAMENTO AS RIVERSAMENTO," +
                                   "VAR_TIPO_CONS AS TIPOCONS," +
                                   "COPIE_SUPPORTI AS NUM_COPIE," +
                                   "VAR_NOTE_RIFIUTO AS NOTE_RIFIUTO," +
                                   "VAR_FORMATO_DOC AS FORMATO_DOC," +
                                   "ID_POLICY, CONSOLIDA, ID_POLICY_VALIDAZIONE, IS_PREFERRED, " +
                // "USER_ID AS USERID," +
                                   "ID_GRUPPO AS GRUPPO, " +
                                   "VALIDATION_MASK, ";
            //"(select sum(b.size_item) from dpa_items_conservazione b where b.id_conservazione = DPA_AREA_CONSERVAZIONE.system_id) AS TOTAL_SIZE, ";

            if (dbType.ToUpper() == "SQL")
            {
                fields_infoCons = fields_infoCons + "DOCSADM.Vardescribe(DPA_AREA_CONSERVAZIONE.ID_PEOPLE, 'PEOPLENAME') AS USERID";
            }
            else
            {
                fields_infoCons = fields_infoCons + "Vardescribe(DPA_AREA_CONSERVAZIONE.ID_PEOPLE, 'PEOPLENAME') AS USERID";
            }
            queryDef1.setParam("param1", fields_infoCons);
            fields_infoCons = "FROM DPA_AREA_CONSERVAZIONE";
            queryDef1.setParam("param2", fields_infoCons);
            //passo il filtro di ricerca comprensivo della clausola where...
            //queryDef1.setParam("param3", filtro + " ORDER BY DATA_APERTURA DESC");
            queryDef1.setParam("param3", string.Format(" WHERE system_id = {0}", idConservazione) + " ORDER BY DATA_APERTURA");
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        reader.Read();

                        InfoConservazione infoCons = new InfoConservazione();
                        infoCons.SystemID = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                        infoCons.IdAmm = reader.GetValue(reader.GetOrdinal("AMM")).ToString();
                        infoCons.IdPeople = reader.GetValue(reader.GetOrdinal("PEOPLE")).ToString();
                        infoCons.IdRuoloInUo = reader.GetValue(reader.GetOrdinal("RUOLO")).ToString();
                        infoCons.StatoConservazione = reader.GetValue(reader.GetOrdinal("STATO")).ToString();
                        infoCons.TipoSupporto = reader.GetValue(reader.GetOrdinal("SUPPORTO")).ToString();
                        infoCons.Note = reader.GetValue(reader.GetOrdinal("NOTE")).ToString();
                        infoCons.Descrizione = reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
                        infoCons.Data_Apertura = reader.GetValue(reader.GetOrdinal("APERTURA")).ToString();
                        infoCons.Data_Invio = reader.GetValue(reader.GetOrdinal("INVIO")).ToString();
                        infoCons.Data_Conservazione = reader.GetValue(reader.GetOrdinal("CONSERVAZIONE")).ToString();
                        infoCons.MarcaTemporale = reader.GetValue(reader.GetOrdinal("MARCA")).ToString();
                        infoCons.FirmaResponsabile = reader.GetValue(reader.GetOrdinal("FIRMA")).ToString();
                        infoCons.LocazioneFisica = reader.GetValue(reader.GetOrdinal("LOCAZIONE")).ToString();
                        infoCons.Data_Prox_Verifica = reader.GetValue(reader.GetOrdinal("PROX_VERIFICA")).ToString();
                        infoCons.Data_Ultima_Verifica = reader.GetValue(reader.GetOrdinal("ULTIMA_VERIFICA")).ToString();
                        infoCons.Data_Riversamento = reader.GetValue(reader.GetOrdinal("RIVERSAMENTO")).ToString();
                        infoCons.TipoConservazione = reader.GetValue(reader.GetOrdinal("TIPOCONS")).ToString();
                        infoCons.numCopie = reader.GetValue(reader.GetOrdinal("NUM_COPIE")).ToString();
                        infoCons.noteRifiuto = reader.GetValue(reader.GetOrdinal("NOTE_RIFIUTO")).ToString();
                        infoCons.formatoDoc = reader.GetValue(reader.GetOrdinal("FORMATO_DOC")).ToString();
                        infoCons.userID = reader.GetValue(reader.GetOrdinal("USERID")).ToString();
                        infoCons.IdGruppo = reader.GetValue(reader.GetOrdinal("GRUPPO")).ToString();
                        infoCons.validationMask = Int32.Parse(reader.GetValue(reader.GetOrdinal("VALIDATION_MASK")).ToString());
                        infoCons.decrSupporto = this.getTipoSupporto(reader.GetValue(reader.GetOrdinal("SUPPORTO")).ToString());
                        infoCons.esitoVerifica = reader.IsDBNull(reader.GetOrdinal("ESITO_VERIFICA")) ? 0 : Int32.Parse(reader.GetValue(reader.GetOrdinal("ESITO_VERIFICA")).ToString());
                        string policy = reader.GetValue(reader.GetOrdinal("ID_POLICY")).ToString();
                        if (!string.IsNullOrEmpty(policy))
                        {
                            infoCons.automatica = "A";
                        }
                        else
                        {
                            infoCons.automatica = "M";
                        }
                        string consolida = reader.GetValue(reader.GetOrdinal("CONSOLIDA")).ToString();
                        if (!string.IsNullOrEmpty(consolida) && consolida.Equals("1"))
                        {
                            infoCons.consolida = true;
                        }
                        else
                        {
                            infoCons.consolida = false;
                        }
                        infoCons.idPolicyValidata = reader.GetValue(reader.GetOrdinal("ID_POLICY_VALIDAZIONE")).ToString();

                        string preferita = reader.GetValue(reader.GetOrdinal("IS_PREFERRED")).ToString();
                        if ((infoCons.StatoConservazione).Equals("N") && !string.IsNullOrEmpty(preferita) && preferita.Equals("1"))
                        {
                            infoCons.predefinita = true;
                        }
                        else
                        {
                            infoCons.predefinita = false;
                        }

                        // Verifica se l'istanza è in fase di preparazione (da "Inviata" a "InLavorazione")
                        //infoCons.IstanzaInPreparazione = DocsPaVO.FileManager.IsInPreparazioneAsync(infoCons.SystemID);


                        //aggiungo l'istanza di info conservazione dentro la lista
                        retValue = infoCons;

                    }
                }
                this.setIdGruppo(ref retValue);
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }
            return retValue;
        }

        private void setIdGruppo(ref InfoConservazione infoCons)
        {

            string idGruppo = infoCons.IdGruppo;
            if (!string.IsNullOrEmpty(idGruppo))
            {
                try
                {
                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                    {
                        DocsPaUtils.Query queryDef2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_GROUPNAME_FROM_GROUPS");
                        queryDef2.setParam("idGruppo", idGruppo);
                        DataSet ds = new DataSet();
                        dbProvider.ExecuteQuery(ds, queryDef2.getSQL());
                        infoCons.IdGruppo = ds.Tables[0].Rows[0][0].ToString();
                    }
                }
                catch (Exception ex)
                {
                    logger.Debug(ex.Message);
                    infoCons.IdGruppo = "";
                }
            }
            else
            {
                infoCons.IdGruppo = "";
            }

        }

        public InfoUtente getInfoUtenteConservazioneByInfoCons(InfoConservazione infoCons)
        {
            string err = string.Empty;
            DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente();
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
            string fields_infoUtente = "ID_GRUPPO AS GRUPPO";
            queryDef1.setParam("param1", fields_infoUtente);
            fields_infoUtente = "FROM DPA_CORR_GLOBALI";
            queryDef1.setParam("param2", fields_infoUtente);
            fields_infoUtente = "WHERE SYSTEM_ID = " + infoCons.IdRuoloInUo;
            queryDef1.setParam("param3", fields_infoUtente);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            infoUtente.idGruppo = reader.GetValue(reader.GetOrdinal("GRUPPO")).ToString();
                        }
                    }
                }
                DocsPaUtils.Query queryDef2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
                fields_infoUtente = "USER_ID AS ID, VAR_SEDE AS SEDE";
                queryDef2.setParam("param1", fields_infoUtente);
                fields_infoUtente = "FROM PEOPLE";
                queryDef2.setParam("param2", fields_infoUtente);
                fields_infoUtente = "WHERE SYSTEM_ID = " + infoCons.IdPeople;
                queryDef2.setParam("param3", fields_infoUtente);
                commandText = queryDef2.getSQL();
                logger.Debug(commandText);
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            infoUtente.userId = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                            infoUtente.sede = reader.GetValue(reader.GetOrdinal("SEDE")).ToString();
                        }
                    }
                }
                //creo l'oggetto user manager dell'interfaccia documentale per recuperare il dst
                //DocsPaDocumentale.Documentale.UserManager um = new DocsPaDocumentale.Documentale.UserManager();
                //infoUtente.dst = um.GetSuperUserAuthenticationToken();
                //completo i dati dell'oggetto info utente con quelli di info conservazione
                infoUtente.idAmministrazione = infoCons.IdAmm;
                infoUtente.idCorrGlobali = infoCons.IdRuoloInUo;
                infoUtente.idPeople = infoCons.IdPeople;
            }
            catch (Exception exc)
            {
                err = exc.Message;
                infoUtente = null;
                logger.Debug(err);
            }
            return infoUtente;
        }

        /// <summary>
        /// Restituisce il nome del tipo di supporto associato all'id passato in input
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public string getTipoSupporto(string idTipo)
        {
            string err = string.Empty;
            string TipoSupporto = string.Empty;
            if (!string.IsNullOrEmpty(idTipo))
            {
                DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
                string fields_infoTipo = "VAR_TIPO AS TIPO_SUPPORTO";
                queryDef1.setParam("param1", fields_infoTipo);
                fields_infoTipo = "FROM DPA_TIPO_SUPPORTO";
                queryDef1.setParam("param2", fields_infoTipo);
                fields_infoTipo = "WHERE SYSTEM_ID = " + idTipo;
                queryDef1.setParam("param3", fields_infoTipo);
                string commandText = queryDef1.getSQL();
                logger.Debug(commandText);
                try
                {
                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                    {
                        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                        {
                            while (reader.Read())
                            {
                                TipoSupporto = reader.GetValue(reader.GetOrdinal("TIPO_SUPPORTO")).ToString();
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    err = exc.Message;
                    logger.Debug(err);
                }
            }
            return TipoSupporto;
        }

        public string GetRapportoVersamentoDocsPA(string idIstanza, InfoUtente utente)
        {
            string result = string.Empty;
            string fieldValue = string.Empty;

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    fieldValue = dbProvider.GetLargeText("DPA_AREA_CONSERVAZIONE", idIstanza, "RAPPORTO_VERSAMENTO");
                }

                if (!string.IsNullOrEmpty(fieldValue))
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        byte[] b = Convert.FromBase64String(fieldValue);

                        stream.Write(b, 0, b.Length);

                        stream.Position = 0;

                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        result = formatter.Deserialize(stream).ToString(); 
                    }
                }

            }
            catch (Exception ex)
            {
                logger.DebugFormat("{0}\r\n{1}", ex.Message, ex.StackTrace);
            }

            return result;
        }


        #region INTEGRAZIONE PITRE-PARER

        public string getStatoConservazione(string idDoc)
        {
            string retVal = string.Empty;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_STATO_CONS");
                    query.setParam("idDoc", idDoc);
                    query.setParam("fields", "CHA_STATO");
                    string commandText = query.getSQL();

                    if (!dbProvider.ExecuteScalar(out retVal, commandText))
                        throw new Exception();

                    // se non trovo risultati il documento non è conservato
                    if (string.IsNullOrEmpty(retVal))
                        retVal = "N";
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
            }

            return retVal;

        }


        /// <summary>
        /// Estrazione lista documenti da versare
        /// Stati V (in attesa di versamento), E (errore nell'invio), T (timeout)
        /// </summary>
        /// <param name="registri">Se true, estrae solo il tipo registro stampe; se false, estrae tutto meno le stampe</param>
        /// <returns></returns>
        public ArrayList getListaDocToSend(bool registri)
        {
            ArrayList result = new ArrayList();

            try
            {
                Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_VERSAMENTO_GET_DOC_TO_SEND");
                // MEV XXXXXXXX
                // Il tipo registro stampe deve essere estratto per primo nella coda
                // Eseguo due chiamate - con la prima prendo le eventuali stampe, con la seconda le altre tipologie
                if (registri)
                    query.setParam("filtroStampe", "AND P.cha_tipo_proto IN ('R', 'C') ORDER BY P.cha_tipo_proto DESC");
                else
                    query.setParam("filtroStampe", "AND P.cha_tipo_proto NOT IN ('R', 'C')");
                string commandText = query.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            ItemsVersamento item = new ItemsVersamento();
                            item.idProfile = reader.GetValue(reader.GetOrdinal("ID_PROFILE")).ToString();
                            item.idPeople = reader.GetValue(reader.GetOrdinal("ID_PEOPLE")).ToString();
                            item.idGruppo = reader.GetValue(reader.GetOrdinal("ID_RUOLO")).ToString();
                            item.idAmm = reader.GetValue(reader.GetOrdinal("ID_AMM")).ToString();
                            item.tentativiInvio = reader.GetValue(reader.GetOrdinal("NUM_TENTATIVI_INVIO")).ToString();
                            item.stato = reader.GetValue(reader.GetOrdinal("CHA_STATO")).ToString();

                            result.Add(item);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel reperimento della coda di versamento: ", ex);
                result = null;
            }

            return result;
        }


        public bool addDocToQueueCons(string idDoc, InfoUtente infoUtente)
        {
            bool result = false;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_INSERT_DOC_IN_QUEUE_CONS");

                    string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

                    if (dbType.ToUpper().Equals("ORACLE"))
                        q.setParam("sysId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_VERSAMENTO"));

                    q.setParam("idDoc", idDoc);
                    q.setParam("idPeople", infoUtente.idPeople);
                    q.setParam("idRuolo", infoUtente.idGruppo);
                    q.setParam("idAmm", infoUtente.idAmministrazione);

                    string commandText = q.getSQL();

                    dbProvider.BeginTransaction();

                    int rowsAffected;
                    if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                        throw new ApplicationException(dbProvider.LastExceptionMessage);

                    dbProvider.CommitTransaction();
                    result = true;

                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Debug(ex);
            }

            return result;

        }

        public bool updateQueueCons(string idDoc, InfoUtente infoUtente, string stato, bool dataInvio, string warning, string numTentativo)
        {
            bool result = false;
            logger.Debug("BEGIN");

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                try
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_QUEUE_VERS");

                    q.setParam("idDoc", "=" + idDoc);
                    q.setParam("stato", stato);
                    
                    if (dataInvio)
                        q.setParam("dataInvio", string.Format(", dta_invio={0}", DocsPaDbManagement.Functions.Functions.GetDate()));
                    else
                        q.setParam("dataInvio", string.Empty);
                    
                    q.setParam("fileChiusura", string.Empty);

                    if (infoUtente != null)
                    {
                        if (!string.IsNullOrEmpty(infoUtente.idPeople))
                            q.setParam("idPeople", string.Format(", id_people={0}", infoUtente.idPeople));
                        else
                            q.setParam("idPeople", string.Empty);
                        if (!string.IsNullOrEmpty(infoUtente.idGruppo))
                            q.setParam("idRuolo", string.Format(", id_ruolo={0}", infoUtente.idGruppo));
                        else
                            q.setParam("idRuolo", string.Empty);
                    }
                    else
                    {
                        q.setParam("idPeople", string.Empty);
                        q.setParam("idRuolo", string.Empty);
                    }
                    
                    if (!string.IsNullOrEmpty(warning))
                        q.setParam("warning", string.Format(", cha_warning={0}", warning));
                    q.setParam("warning", string.Empty);

                    if (!string.IsNullOrEmpty(numTentativo))
                        q.setParam("numTentativo", string.Format(", num_tentativi_invio={0}", numTentativo));
                    else
                        q.setParam("numTentativo", string.Empty);

                    string commandText = q.getSQL();

                    dbProvider.BeginTransaction();

                    int rowsAffected = 0;
                    if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                        throw new ApplicationException(dbProvider.LastExceptionMessage);

                    dbProvider.CommitTransaction();
                    result = true;
                }
                catch (Exception ex)
                {
                    dbProvider.RollbackTransaction();
                    result = false;
                    logger.Debug(ex);
                }
            }

            logger.Debug("END");

            return result;
        }

        public bool updateQueueConsList(ArrayList lista, string stato)
        {
            bool result = false;
            logger.Debug("BEGIN");

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    string whereCond = "IN ( ";
                    // gestione liste con più di 1000 elementi
                    int counter = 1;
                    foreach (ItemsVersamento id in lista)
                    {
                        if (counter > 998)
                        {
                            whereCond += id.idProfile + ") OR ID_PROFILE IN (";
                            counter = 0;
                        }
                        else
                        {
                            whereCond += id.idProfile + ",";
                            counter++;
                        }
 
                    }
                    whereCond = whereCond.TrimEnd();
                    if (whereCond.EndsWith(","))
                    {
                        whereCond = whereCond.Remove(whereCond.Length - 1);
                    }
                    whereCond += " )";

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_QUEUE_VERS");
                    
                    q.setParam("idDoc", whereCond);
                    q.setParam("stato", stato);

                    q.setParam("dataInvio", string.Empty);
                    q.setParam("fileChiusura", string.Empty);
                    q.setParam("warning", string.Empty);
                    q.setParam("numTentativo", string.Empty);
                    q.setParam("idPeople", string.Empty);
                    q.setParam("idRuolo", string.Empty);

                    string commandText = q.getSQL();
                    logger.Debug(commandText);

                    dbProvider.BeginTransaction();
                    int rowsAffected;

                    if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                        throw new ApplicationException(dbProvider.LastExceptionMessage);

                    logger.Debug(string.Format("{0} righe aggiornate", rowsAffected.ToString()));
                    dbProvider.CommitTransaction();

                    result = true;

                }
                catch (Exception ex)
                {
                    dbProvider.RollbackTransaction();
                    result = false;
                    logger.Debug(ex);
                }
            }

            logger.Debug("END");
            return result;
        }

        public string getTemplateXML(string idAmm, string tipo)
        {
            string retVal = string.Empty;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_VERSAMENTO_GET_TEMPLATE");
                    query.setParam("idAmm", idAmm);
                    query.setParam("tipo", tipo);

                    string commandText = query.getSQL();
                    if (!dbProvider.ExecuteScalar(out retVal, commandText))
                        throw new Exception(dbProvider.LastExceptionMessage);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
            }

            return retVal;
        }

        public string getFileXML(string idProfile, string colName)
        {
            string retVal = string.Empty;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_VERSAMENTO_GET_XML");
                    query.setParam("field", colName);
                    query.setParam("idProfile", idProfile);

                    string commandText = query.getSQL();
                    if (!dbProvider.ExecuteScalar(out retVal, commandText))
                        throw new Exception(dbProvider.LastExceptionMessage);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
            }

            return retVal;
        }

        public bool insertFileXML(string tableName, string id, string colName, string val)
        {

            logger.Debug("BEGIN");

            bool retVal = false;
            string sysId = string.Empty;

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_STATO_CONS");
                    query.setParam("idDoc", id);
                    query.setParam("fields", "SYSTEM_ID");

                    string commandText = query.getSQL();
                    if (!dbProvider.ExecuteScalar(out sysId, commandText))
                        throw new Exception(dbProvider.LastExceptionMessage);


                    dbProvider.BeginTransaction();

                    if (!string.IsNullOrEmpty(sysId))
                        retVal = dbProvider.SetLargeText(tableName, sysId, colName, val);
                    else
                        throw new Exception("Salvataggio del file dei metadati fallito.");

                    dbProvider.CommitTransaction();


                }
            }
            catch (Exception ex)
            {
                retVal = false;
                logger.Debug(ex);
            }

            logger.Debug("END");
            return retVal;

        }

        public string getFileXML(string tableName, string id, string colName)
        {
            string retVal = string.Empty;

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    retVal = dbProvider.GetLargeText(tableName, id, colName);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
            }

            return retVal;
        }

        public DocsPaVO.areaConservazione.StampaRegistro getInfoStampaRegistro(string idDoc)
        {
            DocsPaVO.areaConservazione.StampaRegistro result = new StampaRegistro();

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_INFO_STAMPA_REG");
                    query.setParam("idDoc", idDoc);
                    string commandText = query.getSQL();
                    logger.Debug(commandText);

                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            result.docNumber = idDoc;
                            result.anno = reader.GetValue(reader.GetOrdinal("NUM_ANNO")).ToString();
                            result.idRegistro = reader.GetValue(reader.GetOrdinal("ID_REGISTRO")).ToString();
                            result.numProtoStart = reader.GetValue(reader.GetOrdinal("NUM_PROTO_START")).ToString();
                            result.numProtoEnd = reader.GetValue(reader.GetOrdinal("NUM_PROTO_END")).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                result = null;
            }

            return result;
        }

        public DocsPaVO.areaConservazione.StampaRegistro getInfoStampaReperiorio(string idDoc)
        {
            DocsPaVO.areaConservazione.StampaRegistro result = new DocsPaVO.areaConservazione.StampaRegistro();

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_INFO_STAMPA_REP");
                    query.setParam("idDoc", idDoc);
                    string commandText = query.getSQL();
                    logger.Debug(commandText);
                    
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            result.docNumber = idDoc;
                            result.anno = reader.GetValue(reader.GetOrdinal("NUM_ANNO")).ToString();
                            result.idRepertorio = reader.GetValue(reader.GetOrdinal("ID_REPERTORIO")).ToString();
                            result.numProtoStart = reader.GetValue(reader.GetOrdinal("NUM_REP_START")).ToString();
                            result.numProtoEnd = reader.GetValue(reader.GetOrdinal("NUM_REP_END")).ToString();
                            result.idRegistro = reader.GetValue(reader.GetOrdinal("REGISTRYID")).ToString();
                            if(!dbType.ToUpper().Equals("SQL"))
                            {
                                result.dtaStampaTruncString = reader.GetValue(reader.GetOrdinal("DATASTAMPA")).ToString(); 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                result = null;
            }

            return result;
        }

        public string getDataRepertoriazione(string anno, string value, string idOgg)
        {

            string result = string.Empty;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_DATA_INSERIMENTO_REP");
                    query.setParam("anno", anno);
                    query.setParam("cont", value);
                    query.setParam("idOgg", idOgg);

                    string commandText = query.getSQL();
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteScalar(out result, commandText))
                        throw new Exception(dbProvider.LastExceptionMessage);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                result = string.Empty;
            }

            return result;
        }

        public string getVersioneDatiSpecifici(string idAmm, string tipo)
        {
            string retVal = string.Empty;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_VERSAMENTO_GET_VERS_DATI_SPEC");
                    query.setParam("idAmm", idAmm);
                    query.setParam("tipo", tipo);

                    string commandText = query.getSQL();
                    if (!dbProvider.ExecuteScalar(out retVal, commandText))
                        throw new Exception(dbProvider.LastExceptionMessage);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
            }

            return retVal;
        }

        public string getNomeTipologia(string idContatore)
        {
            string result = string.Empty;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_NOME_TIPOLOGIA");
                    query.setParam("idCounter", idContatore);

                    string commandText = query.getSQL();
                    logger.Debug(commandText);

                    if (!dbProvider.ExecuteScalar(out result, commandText))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    logger.Debug("Tipologia: " + result);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                result = string.Empty;
            }

            
            return result;
        }

        public string getNomeTipologiaDaDoc(string idDoc)
        {
            string result = string.Empty;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query query = InitQuery.getInstance().getQuery("S_GET_NOME_TIPOLOGIA_DA_DOC");
                    query.setParam("idDoc", idDoc);

                    string command = query.getSQL();
                    logger.Debug(command);

                    if (!dbProvider.ExecuteScalar(out result, command))
                        throw new Exception(dbProvider.LastExceptionMessage);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                result = string.Empty;
            }

            return result;
        }

        public string getNomeTipologiaDaStampa(string idDoc)
        {
            string result = string.Empty;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query query = InitQuery.getInstance().getQuery("S_GET_NOME_TIPOLOGIA_DA_STAMPA");
                    query.setParam("idDoc", idDoc);

                    string command = query.getSQL();
                    logger.Debug(command);

                    if (!dbProvider.ExecuteScalar(out result, command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                result = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// Restituisce l'id del primo fascicolo (in ordine cronologico) in cui il documento è stato inserito
        /// </summary>
        /// <param name="idDoc"></param>
        /// <returns></returns>
        public string GetIdFascPrimaFascicolazione(string idDoc)
        {
            string result = string.Empty;

            try
            {
                Query q = InitQuery.getInstance().getQuery("S_CONS_PRIMA_FASC");
                q.setParam("idDoc", idDoc);

                string command = q.getSQL();
                logger.Debug(command);

                using (DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(command))
                    {
                        if (reader.Read())
                        {
                            result = reader.GetValue(reader.GetOrdinal("ID_FASCICOLO")).ToString();
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// MEV INTEGRAZIONE PITRE-PARER
        /// Policy e responsabile conservazione
        /// REQ F03_01 - Responsabile della conservazione
        /// Recupera l'id del ruolo responsabile della conservazione per l'amministrazione selezionata
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public string GetIdRoleResponsabileConservazione(string idAmm)
        {
            string result = string.Empty;

            try
            {
                Query q = InitQuery.getInstance().getQuery("S_GET_ID_ROLE_RESP_CONS");
                q.setParam("idAmm", idAmm);

                string command = q.getSQL();

                using (DBProvider dbProvider = new DBProvider())
                {
                    if (!dbProvider.ExecuteScalar(out result, command))
                        throw new Exception(dbProvider.LastExceptionMessage);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return result;
        }

        public string GetIdUtenteResponsabileConservazione(string idAmm)
        {
            string result = string.Empty;

            try
            {
                Query q = InitQuery.getInstance().getQuery("S_GET_ID_USER_RESP_CONS");
                q.setParam("idAmm", idAmm);

                string command = q.getSQL();

                using (DBProvider dbProvider = new DBProvider())
                {
                    if (!dbProvider.ExecuteScalar(out result, command))
                        throw new Exception(dbProvider.LastExceptionMessage);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return result;
        }

        public bool insertInSecurity(string idDoc, string idGroup, string idVersatore)
        {
            bool result = false;
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    Query q = InitQuery.getInstance().getQuery("I_SECURITY_CONSERVAZIONE");
                    q.setParam("param1", idDoc);
                    q.setParam("param2", idGroup);
                    q.setParam("param3", idVersatore);

                    string command = q.getSQL();
                    logger.Debug(command);

                    if (!dbProvider.ExecuteNonQuery(command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    result = true;
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return result;
        }

        public bool UpdateSecurityConservazione(string idDoc, string idGroup)
        {
            bool result = false;
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    Query q = InitQuery.getInstance().getQuery("U_SECURITY_CONSERVAZIONE");
                    q.setParam("param1", idDoc);
                    q.setParam("param2", idGroup);

                    string command = q.getSQL();
                    logger.Debug(command);

                    if (!dbProvider.ExecuteNonQuery(command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    result = true;
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return result;
        }

        public string VerificaDirittiInSecurity(string idDoc, string idGroup)
        {

            string result = string.Empty;

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    Query q = InitQuery.getInstance().getQuery("S_VERIFICA_DIRITTI_SEC_CONS");
                    q.setParam("param1", idDoc);
                    q.setParam("param2", idGroup);

                    string command = q.getSQL();
                    logger.Debug(command);

                    string val = string.Empty;
                    if (!dbProvider.ExecuteScalar(out val, command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    if (!string.IsNullOrEmpty(val))
                    {
                        int rights = Convert.ToInt32(val);

                        // Se minore di 63 devo aggiornare la security per garantire diritti di lettura/scrittura
                        // Altrimenti non tocco nulla
                        if (rights < 63)
                            result = "UPDATE";
                        else
                            result = "OK";
                    }
                    else
                    {
                        // Record non presente
                        // Devo inserirlo nella security
                        result = "INSERT";
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return result;
        }

        public bool SaveErrorMessage(string idProfile, string error)
        {
            bool result = false;
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    Query q = InitQuery.getInstance().getQuery("U_ERROR_VERSAMENTO");
                    q.setParam("idPr", idProfile);
                    q.setParam("str", error);

                    string command = q.getSQL();
                    logger.Debug(command);

                    result = dbProvider.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public string GetCodiceAOO(string idProfile)
        {
            logger.Debug("BEGIN");
            string result = string.Empty;

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    Query q = InitQuery.getInstance().getQuery("S_CONS_GET_COD_AOO");
                    q.setParam("idProfile", idProfile);

                    string command = q.getSQL();
                    logger.Debug(command);

                    if (!dbProvider.ExecuteScalar(out result, command))
                        throw new Exception(dbProvider.LastExceptionMessage);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                result = string.Empty;
            }

            logger.Debug("END");
            return result;
        }

        #region POLICY PARER

        public ArrayList GetListaPolicyPARER(string idAmm, string tipo)
        {

            ArrayList lista = new ArrayList();

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    Query q = InitQuery.getInstance().getQuery("S_GET_LISTA_POLICY_PARER");
                    q.setParam("idAmm", idAmm);
                    q.setParam("tipo", tipo);

                    string command = q.getSQL();
                    logger.Debug(command);

                    using (IDataReader reader = dbProvider.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            DocsPaVO.Conservazione.PARER.PolicyPARER p = new DocsPaVO.Conservazione.PARER.PolicyPARER();

                            p.id = reader.GetValue(reader.GetOrdinal("SYSTEM_ID")).ToString();
                            p.codice = reader.GetValue(reader.GetOrdinal("VAR_CODICE")).ToString();
                            p.descrizione = reader.GetValue(reader.GetOrdinal("VAR_DESCRIZIONE")).ToString();
                            p.isAttiva = reader.GetValue(reader.GetOrdinal("CHA_ATTIVA")).ToString();

                            lista.Add(p);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                lista = null;
            }

            return lista;
        }

        public ArrayList GetListaPolicyDaEseguire(string idAmm)
        {
            ArrayList lista = new ArrayList();

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    Query q = InitQuery.getInstance().getQuery("S_GET_POLICY_TO_EXECUTE");
                    q.setParam("idAmm", idAmm);

                    string command = q.getSQL();
                    logger.Debug(command);

                    using (IDataReader reader = dbProvider.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            DocsPaVO.Conservazione.PARER.EsecuzionePolicy info = new DocsPaVO.Conservazione.PARER.EsecuzionePolicy();
                            info.idPolicy = reader.GetValue(reader.GetOrdinal("ID_POLICY")).ToString();
                            info.dataUltimaEsecuzione = reader.GetValue(reader.GetOrdinal("ULTIMA_ESECUZIONE")).ToString();
                            info.dataProssimaEsecuzione = reader.GetValue(reader.GetOrdinal("PROSSIMA_ESECUZIONE")).ToString();
                            info.numeroEsecuzioni = reader.GetValue(reader.GetOrdinal("NUM_ESECUZIONI")).ToString();
                            lista.Add(info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                lista = null;
            }

            return lista;
        }

        public DocsPaVO.Conservazione.PARER.PolicyPARER GetPolicyPARERById(string idPolicy)
        {

            logger.Debug("BEGIN");

            DocsPaVO.Conservazione.PARER.PolicyPARER policy = new DocsPaVO.Conservazione.PARER.PolicyPARER();

            try
            {
                Query q = InitQuery.getInstance().getQuery("S_GET_POLICY_PARER_BY_ID");
                q.setParam("idPolicy", idPolicy);

                string command = q.getSQL();
                logger.Debug(command);

                using (DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(command))
                    {

                        while (reader.Read())
                        {
                            policy.id = reader.GetValue(reader.GetOrdinal("SYSTEM_ID")).ToString();
                            policy.codice = reader.GetValue(reader.GetOrdinal("VAR_CODICE")).ToString();
                            policy.descrizione = reader.GetValue(reader.GetOrdinal("VAR_DESCRIZIONE")).ToString();
                            policy.idAmm = reader.GetValue(reader.GetOrdinal("ID_AMM")).ToString();
                            policy.tipo = reader.GetValue(reader.GetOrdinal("CHA_TIPO_POLICY")).ToString();
                            policy.isAttiva = reader.GetValue(reader.GetOrdinal("CHA_ATTIVA")).ToString();
                            policy.idGruppoRuoloResp = reader.GetValue(reader.GetOrdinal("ID_GROUP_RUOLO_RESP")).ToString();
                            policy.periodicita = reader.GetValue(reader.GetOrdinal("CHA_PERIODICITA")).ToString();
                            policy.giornoEsecuzione = reader.GetValue(reader.GetOrdinal("CHA_ESECUZIONE_GIORNO")).ToString();
                            policy.meseEsecuzione = reader.GetValue(reader.GetOrdinal("CHA_ESECUZIONE_MESE")).ToString();
                            policy.dataEsecuzione = reader.GetValue(reader.GetOrdinal("DTA_ESECUZIONE_POLICY")).Equals(DBNull.Value) ? 
                                string.Empty : reader.GetValue(reader.GetOrdinal("DTA_ESECUZIONE_POLICY")).ToString();
                            policy.statoVersamento = reader.GetValue(reader.GetOrdinal("CHA_STATO_VERSAMENTO")).ToString();
                            policy.arrivo = reader.GetValue(reader.GetOrdinal("CHA_TIPO_PROTO_A")).ToString().Equals("1");
                            policy.partenza = reader.GetValue(reader.GetOrdinal("CHA_TIPO_PROTO_P")).ToString().Equals("1");
                            policy.interno = reader.GetValue(reader.GetOrdinal("CHA_TIPO_PROTO_I")).ToString().Equals("1");
                            policy.grigio = reader.GetValue(reader.GetOrdinal("CHA_TIPO_PROTO_G")).ToString().Equals("1");
                            policy.idTemplate = reader.GetValue(reader.GetOrdinal("ID_TEMPLATE")).ToString();
                            policy.idStato = reader.GetValue(reader.GetOrdinal("ID_STATO")).ToString();
                            policy.operatoreStato = reader.GetValue(reader.GetOrdinal("CHA_STATO")).ToString();
                            policy.idRegistro = reader.GetValue(reader.GetOrdinal("ID_REGISTRO")).ToString();
                            policy.idRF = reader.GetValue(reader.GetOrdinal("ID_RF")).ToString();
                            policy.idUO = reader.GetValue(reader.GetOrdinal("ID_UO_CREATRICE")).ToString();
                            policy.UOsottoposte = reader.GetValue(reader.GetOrdinal("CHA_UO_SOTTOPOSTE")).ToString();
                            policy.idTitolario = reader.GetValue(reader.GetOrdinal("ID_TITOLARIO")).ToString();
                            policy.idFascicolo = reader.GetValue(reader.GetOrdinal("ID_FASCICOLO")).ToString();
                            policy.tipoClassificazione = reader.GetValue(reader.GetOrdinal("CHA_TIPO_CLASS")).ToString();
                            policy.digitali = reader.GetValue(reader.GetOrdinal("CHA_DOC_DIGITALI")).ToString();
                            policy.firmati = reader.GetValue(reader.GetOrdinal("CHA_FIRMATO")).ToString();
                            policy.marcati = reader.GetValue(reader.GetOrdinal("CHA_MARCATO")).ToString();
                            policy.scadenzaMarca = reader.GetValue(reader.GetOrdinal("CHA_SCADENZA_TIMESTAMP")).ToString();
                            policy.filtroDataCreazione = reader.GetValue(reader.GetOrdinal("CHA_DATA_CREAZIONE_TIPO")).ToString();
                            policy.dataCreazioneDa = reader.GetValue(reader.GetOrdinal("DATA_CREAZ_FROM")).ToString();
                            policy.dataCreazioneA = reader.GetValue(reader.GetOrdinal("DATA_CREAZ_TO")).ToString();
                            policy.filtrodataProtocollazione = reader.GetValue(reader.GetOrdinal("CHA_DATA_PROTO_TIPO")).ToString();
                            policy.dataProtocollazioneDa = reader.GetValue(reader.GetOrdinal("DATA_PROT_FROM")).ToString();
                            policy.dataProtocollazioneA = reader.GetValue(reader.GetOrdinal("DATA_PROT_TO")).ToString();
                            policy.tipoRegistroStampa = reader.GetValue(reader.GetOrdinal("CHA_TIPO_REGISTRO_STAMPA")).ToString();
                            policy.idRepertorio = reader.GetValue(reader.GetOrdinal("ID_REPERTORIO_STAMPA")).ToString();
                            policy.annoStampa = reader.GetValue(reader.GetOrdinal("NUM_ANNO_STAMPA")).ToString();
                            policy.filtroDataStampa = reader.GetValue(reader.GetOrdinal("CHA_DATA_STAMPA_TIPO")).ToString();
                            policy.dataStampaDa = reader.GetValue(reader.GetOrdinal("DATA_ST_FROM")).ToString();
                            policy.dataStampaA = reader.GetValue(reader.GetOrdinal("DATA_ST_TO")).ToString();
                            policy.numGiorniCreazione = reader.GetValue(reader.GetOrdinal("NUM_GIORNI_DATA_CREAZIONE")).ToString();
                            policy.numGiorniProtocollazione = reader.GetValue(reader.GetOrdinal("NUM_GIORNI_DATA_PROTO")).ToString();
                            policy.numGiorniStampa = reader.GetValue(reader.GetOrdinal("NUM_GIORNI_DATA_STAMPA")).ToString();
                            
                        }

                        reader.Close();
                    }

                    // Formati documento associati alla policy
                    List<SupportedFileType> listaFormati = this.GetFormatiDocumentoPolicy(idPolicy);
                    if (listaFormati != null && listaFormati.Count > 0)
                    {
                        policy.FormatiDocumento = new List<SupportedFileType>();
                        policy.FormatiDocumento.AddRange(listaFormati);
                    }

                    // Campi profilati associati alla policy
                    if (!string.IsNullOrEmpty(policy.idTemplate) && !policy.idTemplate.Equals("0"))
                    {
                        policy.template = this.GetTemplateDocumento(policy.id, policy.idTemplate);
                    }
                   
                }
            }
            catch (Exception ex)
            {
                policy = null;
                logger.Debug(ex.Message);
            }

            logger.Debug("END");

            return policy;
        }

        public List<SupportedFileType> GetFormatiDocumentoPolicy(string idPolicy)
        {
            logger.Debug("BEGIN");
            List<SupportedFileType> lista = new List<SupportedFileType>();

            try
            {
                Query qf = InitQuery.getInstance().getQuery("S_GET_FORMATI_POLICY_PARER");
                qf.setParam("policyId", idPolicy);
                string command = qf.getSQL();

                logger.Debug(command);
                using (DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            SupportedFileType fileType = new SupportedFileType();
                            fileType.SystemId = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID_TYPE")));
                            fileType.FileExtension = reader.GetValue(reader.GetOrdinal("FILE_EXTENSION")).ToString();
                            lista.Add(fileType);
                        }

                        reader.Close();
                    }

                   
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel reperimento dei formati documento associati alla policy - " + ex.Message);
                lista = null;
            }

            logger.Debug("END");

            return lista;
        }

        public DocsPaVO.ProfilazioneDinamica.Templates GetTemplateDocumento(string idPolicy, string idTemplate)
        {
            
            logger.Debug("BEGIN");
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();

            try
            {    
                Model model = new Model();
                template = model.getTemplateById(idTemplate);

                if (template != null)
                {
                    using (DBProvider dbProvier = new DBProvider())
                    {
                        Query q = InitQuery.getInstance().getQuery("S_TEMPLATES_POLICY_CONSERVAZIONE_BY_ID");
                        q.setParam("idPolicy", idPolicy);
                        string command = q.getSQL();
                        logger.Debug(command);

                        using (IDataReader reader = dbProvier.ExecuteReader(command))
                        {
                            while (reader.Read())
                            {
                                DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                                ogg.SYSTEM_ID = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID_OBJ_CUSTOM")));
                                ogg.VALORE_DATABASE = reader.GetValue(reader.GetOrdinal("VALORE")).ToString();

                                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom o in template.ELENCO_OGGETTI)
                                {
                                    if (o.SYSTEM_ID.Equals(ogg.SYSTEM_ID))
                                    {
                                        if (o.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione"))
                                        {
                                            for (int i = 0; i < o.ELENCO_VALORI.Count; i++)
                                            {
                                                if (((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)o.ELENCO_VALORI[i]).VALORE.Equals(ogg.VALORE_DATABASE))
                                                    o.VALORI_SELEZIONATI[i] = ogg.VALORE_DATABASE;
                                            }
                                        }
                                        else if (o.TIPO.DESCRIZIONE_TIPO.Equals("Contatore"))
                                        {
                                            if (ogg.VALORE_DATABASE.Contains("_"))
                                            {
                                                o.VALORE_DATABASE = ogg.VALORE_DATABASE.Split('_')[0];
                                                o.ID_AOO_RF = ogg.VALORE_DATABASE.Split('_')[1];
                                            }
                                            else
                                            {
                                                o.VALORE_DATABASE = ogg.VALORE_DATABASE;
                                            }
                                        }
                                        else
                                        {
                                            o.VALORE_DATABASE = ogg.VALORE_DATABASE;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel recupero dei campi profilati");
                throw new Exception(ex.Message);
            }

            logger.Debug("END");
            return template;
        }

        public bool InsertNewPolicyPARER(DocsPaVO.Conservazione.PARER.PolicyPARER policy)
        {
            logger.Debug("BEGIN");

            bool result = false;

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    dbProvider.BeginTransaction();

                    Query q = InitQuery.getInstance().getQuery("I_DPA_POLICY_PARER");
                    q.setParam("codPolicy", policy.codice.Replace("'", "''"));
                    q.setParam("descrPolicy", policy.descrizione.Replace("'", "''"));
                    q.setParam("tipoPolicy", policy.tipo);
                    q.setParam("idAmm", policy.idAmm);
                    q.setParam("ruoloResp", policy.idGruppoRuoloResp);
                    q.setParam("period", policy.periodicita);
                    q.setParam("giornoExec", policy.giornoEsecuzione);
                    q.setParam("meseExec", policy.meseEsecuzione);
                    if (!string.IsNullOrEmpty(policy.dataEsecuzione))
                        q.setParam("dataExec", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataEsecuzione));
                    else
                        q.setParam("dataExec", "''");
                    q.setParam("statoVersamento", policy.statoVersamento);
                    q.setParam("arrivo", policy.arrivo ? "1" : "0");
                    q.setParam("partenza", policy.partenza ? "1" : "0");
                    q.setParam("interno", policy.interno ? "1" : "0");
                    q.setParam("grigio", policy.grigio ? "1" : "0");
                    q.setParam("idTemplate", policy.idTemplate);
                    q.setParam("idReg", policy.idRegistro);
                    q.setParam("idRf", policy.idRF);
                    q.setParam("idUo", policy.idUO);
                    q.setParam("idTit", policy.idTitolario);
                    q.setParam("idFasc", policy.idFascicolo);
                    q.setParam("tipoClass", policy.tipoClassificazione);
                    q.setParam("uosp", policy.UOsottoposte);
                    q.setParam("isfirmato", policy.firmati);
                    q.setParam("ismarcato", policy.marcati);
                    q.setParam("scadMarca", policy.scadenzaMarca);
                    q.setParam("dataCr", policy.filtroDataCreazione);
                    if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                        q.setParam("dataCrDa", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa));
                    else
                        q.setParam("dataCrDa", "''");
                    if (!string.IsNullOrEmpty(policy.dataCreazioneA))
                        q.setParam("dataCrA", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneA));
                    else
                        q.setParam("dataCrA", "''");
                    q.setParam("dataProto", policy.filtrodataProtocollazione);
                    if (!string.IsNullOrEmpty(policy.dataProtocollazioneDa))
                        q.setParam("dataProtoDa", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneDa));
                    else
                        q.setParam("dataProtoDa", "''");
                    if (!string.IsNullOrEmpty(policy.dataProtocollazioneA))
                        q.setParam("dataProtoA", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneA));
                    else
                        q.setParam("dataProtoA", "''");
                    q.setParam("numGiorniDataCreazione", policy.numGiorniCreazione);
                    q.setParam("numGiorniDataProto", policy.numGiorniProtocollazione);

                    // CAMPI PER POLICY STAMPE
                    q.setParam("tipoReg", policy.tipoRegistroStampa);
                    q.setParam("idRep", policy.idRepertorio);
                    q.setParam("annoStampa", policy.annoStampa);
                    q.setParam("dataStampa", policy.filtroDataStampa);
                    if (!string.IsNullOrEmpty(policy.dataStampaDa))
                        q.setParam("dataStampaDa", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataStampaDa));
                    else
                        q.setParam("dataStampaDa", "''");
                    if (!string.IsNullOrEmpty(policy.dataStampaA))
                        q.setParam("dataStampaA", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataStampaA));
                    else
                        q.setParam("dataStampaA", "''");
                    q.setParam("numGiorniDataStampa", policy.numGiorniStampa);

                    string command = q.getSQL();
                    logger.Debug(command);
                    if (!dbProvider.ExecuteNonQuery(command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    // Reperimento system id nuova policy
                    //string idPolicy = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_POLICY_PARER");
                    string idPolicy = string.Empty;
                    if (!dbProvider.ExecuteScalar(out idPolicy, DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_POLICY_PARER")))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    policy.id = idPolicy;
                    logger.Debug("ID POLICY: " + idPolicy);

                    // Inserimento associazioni tipi file
                    if (policy.FormatiDocumento != null && policy.FormatiDocumento.Count > 0)
                    {
                        logger.Debug("Aggiornamento formati documento");
                        foreach (SupportedFileType file in policy.FormatiDocumento)
                        {
                            Query qf = InitQuery.getInstance().getQuery("I_DPA_ASS_POLICY_TYPE");
                            qf.setParam("id_policy", idPolicy);
                            qf.setParam("id_type", file.SystemId.ToString());
                            command = qf.getSQL();
                            logger.Debug(command);
                            if (!dbProvider.ExecuteNonQuery(command))
                                throw new Exception(dbProvider.LastExceptionMessage);
                        }
                    }

                    // Inserimento associazioni campi profilati
                    // Aggiornamento campi profilati
                    if (policy.template != null && policy.template.ELENCO_OGGETTI != null && policy.template.ELENCO_OGGETTI.Count > 0)
                    {
                        logger.Debug("Aggiornamento campi profilati");
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in policy.template.ELENCO_OGGETTI)
                        {
                            if (ogg.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione"))
                            {
                                // scorro tutti i valori selezionati
                                if (ogg.VALORI_SELEZIONATI != null && ogg.VALORI_SELEZIONATI.Count > 0)
                                {
                                    foreach (string valore in ogg.VALORI_SELEZIONATI)
                                    {
                                        if (!string.IsNullOrEmpty(valore))
                                        {
                                            Query qt = InitQuery.getInstance().getQuery("I_DPA_ASS_POLICY_PROFILAZIONE");
                                            qt.setParam("id_policy", policy.id);
                                            qt.setParam("id_template", policy.template.SYSTEM_ID.ToString());
                                            qt.setParam("obj_id", ogg.SYSTEM_ID.ToString());
                                            qt.setParam("obj_value", valore);
                                            command = qt.getSQL();
                                            logger.Debug(command);
                                            if (!dbProvider.ExecuteNonQuery(command))
                                                throw new Exception(dbProvider.LastExceptionMessage);
                                        }
                                    }
                                }
                            }
                            else if (ogg.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") && !string.IsNullOrEmpty(ogg.ID_AOO_RF))
                            {
                                if (!string.IsNullOrEmpty(ogg.VALORE_DATABASE))
                                {
                                    Query qt = InitQuery.getInstance().getQuery("I_DPA_ASS_POLICY_PROFILAZIONE");
                                    qt.setParam("id_policy", policy.id);
                                    qt.setParam("id_template", policy.template.SYSTEM_ID.ToString());
                                    qt.setParam("obj_id", ogg.SYSTEM_ID.ToString());
                                    qt.setParam("obj_value", ogg.VALORE_DATABASE + "_" + ogg.ID_AOO_RF);
                                    command = qt.getSQL();
                                    logger.Debug(command);
                                    if (!dbProvider.ExecuteNonQuery(command))
                                        throw new Exception(dbProvider.LastExceptionMessage);
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(ogg.VALORE_DATABASE))
                                {
                                    Query qt = InitQuery.getInstance().getQuery("I_DPA_ASS_POLICY_PROFILAZIONE");
                                    qt.setParam("id_policy", policy.id);
                                    qt.setParam("id_template", policy.template.SYSTEM_ID.ToString());
                                    qt.setParam("obj_id", ogg.SYSTEM_ID.ToString());
                                    qt.setParam("obj_value", ogg.VALORE_DATABASE);
                                    command = qt.getSQL();
                                    logger.Debug(command);
                                    if (!dbProvider.ExecuteNonQuery(command))
                                        throw new Exception(dbProvider.LastExceptionMessage);
                                }
                            }
                        }
                    }

                    // Inserimento informazioni di esecuzione
                    logger.Debug("Inserimento informazioni di esecuzione");
                    Query qExec = InitQuery.getInstance().getQuery("I_DPA_EXEC_POLICY_PARER");
                    qExec.setParam("id", policy.id);
                    qExec.setParam("execDate", DocsPaDbManagement.Functions.Functions.ToDate(this.GetDataProssimaEsecuzione(policy), false));

                    command = qExec.getSQL();
                    logger.Debug(command);

                    if (!dbProvider.ExecuteNonQuery(command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    dbProvider.CommitTransaction();
                    result = true;
                }
                catch (Exception ex)
                {
                    dbProvider.RollbackTransaction();
                    logger.Debug(ex.Message);
                    result = false;
                }
            }


            logger.Debug("END");

            return result;
        }

        public bool UpdatePolicyPARER(DocsPaVO.Conservazione.PARER.PolicyPARER policy)
        {
            logger.Debug("BEGIN");

            bool result = false;

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    dbProvider.BeginTransaction();

                    Query q = InitQuery.getInstance().getQuery("U_POLICY_PARER");
                    q.setParam("idPolicy", policy.id);
                    q.setParam("idAmm", policy.idAmm);
                    //q.setParam("cod", policy.codice);
                    q.setParam("descr", policy.descrizione.Replace("'", "''"));
                    q.setParam("idGroupRuoloResp", policy.idGruppoRuoloResp);

                    q.setParam("period", policy.periodicita);
                    q.setParam("execDay", policy.giornoEsecuzione);
                    q.setParam("execMonth", policy.meseEsecuzione);
                    if (!string.IsNullOrEmpty(policy.dataEsecuzione))
                        q.setParam("execDate", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataEsecuzione));
                    else
                        q.setParam("execDate", "NULL");
                    q.setParam("statoVersamento", policy.statoVersamento);

                    // campi da valorizzare per policy documenti
                    if (policy.tipo.Equals("D"))
                    {
                        q.setParam("tipoProtoA", policy.arrivo ? "1" : "0");
                        q.setParam("tipoProtoP", policy.partenza ? "1" : "0");
                        q.setParam("tipoProtoI", policy.interno ? "1" : "0");
                        q.setParam("tipoProtoG", policy.grigio ? "1" : "0");
                    }
                    else
                    {
                        q.setParam("tipoProtoA", string.Empty);
                        q.setParam("tipoProtoP", string.Empty);
                        q.setParam("tipoProtoI", string.Empty);
                        q.setParam("tipoProtoG", string.Empty);
                    }
                        
                    q.setParam("idTemplate", policy.idTemplate);
                    q.setParam("stato", policy.idStato);
                    q.setParam("opStato", policy.operatoreStato);
                    q.setParam("idReg", policy.idRegistro);
                    q.setParam("idRF", policy.idRF);
                    q.setParam("idUO", policy.idUO);
                    q.setParam("UOsp", policy.UOsottoposte);
                    q.setParam("idTit", policy.idTitolario);
                    q.setParam("idFasc", policy.idFascicolo);
                    q.setParam("tipoClass", policy.tipoClassificazione);
                    q.setParam("docDigitali", policy.digitali);
                    q.setParam("isFirmato", policy.firmati);
                    q.setParam("isMarcato", policy.marcati);
                    q.setParam("scadMarca", policy.scadenzaMarca);
                    q.setParam("dataCr", policy.filtroDataCreazione);
                    q.setParam("dataProto", policy.filtrodataProtocollazione);

                    if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                        q.setParam("dataCreazioneFrom", string.Format("DATA_CREAZIONE_FROM={0},", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa)));
                    else
                        q.setParam("dataCreazioneFrom", "DATA_CREAZIONE_FROM='',");
                    if (!string.IsNullOrEmpty(policy.dataCreazioneA))
                        q.setParam("dataCreazioneTo", string.Format("DATA_CREAZIONE_TO={0},", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneA)));
                    else
                        q.setParam("dataCreazioneTo", "DATA_CREAZIONE_TO='',");

                    if (!string.IsNullOrEmpty(policy.dataProtocollazioneDa))
                        q.setParam("dataProtoFrom", string.Format("DATA_PROTO_FROM={0},", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneDa)));
                    else
                        q.setParam("dataProtoFrom", "DATA_PROTO_FROM='',");
                    if (!string.IsNullOrEmpty(policy.dataProtocollazioneA))
                        q.setParam("dataProtoTo", string.Format("DATA_PROTO_TO={0},", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneA)));
                    else
                        q.setParam("dataProtoTo", "DATA_PROTO_TO='',");
                    q.setParam("numGiorniDataCreazione", policy.numGiorniCreazione);
                    q.setParam("numGiorniDataProto", policy.numGiorniProtocollazione);

                    if (policy.tipo.Equals("S"))
                    {
                        q.setParam("tipoRegStampa", policy.tipoRegistroStampa);
                        q.setParam("idRep", policy.idRepertorio);
                        q.setParam("annoStampa", policy.annoStampa);
                        q.setParam("dataStampa", policy.filtroDataStampa);
                        if (!string.IsNullOrEmpty(policy.dataStampaDa))
                            q.setParam("dataStampaFrom", string.Format("DATA_STAMPA_FROM={0},", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataStampaDa)));
                        else
                            q.setParam("dataStampaFrom", "DATA_STAMPA_FROM='',");
                        if (!string.IsNullOrEmpty(policy.dataStampaA))
                            q.setParam("dataStampaTo", string.Format("DATA_STAMPA_TO={0},", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataStampaA)));
                        else
                            q.setParam("dataStampaTo", "DATA_STAMPA_TO='',");
                        q.setParam("numGiorniDataStampa", policy.numGiorniStampa);
                    }
                    else
                    {
                        q.setParam("tipoRegStampa", string.Empty);
                        q.setParam("idRep", string.Empty);
                        q.setParam("annoStampa", string.Empty);
                        q.setParam("dataStampa", string.Empty);
                        q.setParam("dataStampaFrom", "DATA_STAMPA_FROM='',");
                        q.setParam("dataStampaTo", "DATA_STAMPA_TO='',");
                        q.setParam("numGiorniDataStampa", string.Empty);
                    }

                    string command = q.getSQL();
                    logger.Debug(command);

                    if (!dbProvider.ExecuteNonQuery(command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    if (policy.tipo.Equals("D"))
                    {
                        // Rimuovo le associazioni presenti
                        logger.Debug("Rimozione associazione formati documento");
                        Query qrf = InitQuery.getInstance().getQuery("D_DELETE_DPA_ASS_POLICY_TYPE");
                        qrf.setParam("idPolicy", policy.id);
                        dbProvider.ExecuteNonQuery(qrf.getSQL());

                        logger.Debug("Rimozione associazione campi profilati");
                        Query qrt = InitQuery.getInstance().getQuery("D_DELETE_DPA_ASS_POLICY_PROFILAZIONE");
                        qrt.setParam("idPolicy", policy.id);
                        dbProvider.ExecuteNonQuery(qrt.getSQL());

                        // Aggiornamento formati documento
                        if (policy.FormatiDocumento != null && policy.FormatiDocumento.Count > 0)
                        {
                            logger.Debug("Aggiornamento formati documento");
                            foreach (SupportedFileType file in policy.FormatiDocumento)
                            {
                                Query qf = InitQuery.getInstance().getQuery("I_DPA_ASS_POLICY_TYPE");
                                qf.setParam("id_policy", policy.id);
                                qf.setParam("id_type", file.SystemId.ToString());
                                command = qf.getSQL();
                                logger.Debug(command);
                                if (!dbProvider.ExecuteNonQuery(command))
                                    throw new Exception(dbProvider.LastExceptionMessage);
                            }
                        }

                        // Aggiornamento campi profilati
                        if (policy.template != null && policy.template.ELENCO_OGGETTI != null && policy.template.ELENCO_OGGETTI.Count > 0)
                        {
                            logger.Debug("Aggiornamento campi profilati");
                            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in policy.template.ELENCO_OGGETTI)
                            {
                                if (ogg.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione"))
                                {
                                    // scorro tutti i valori selezionati
                                    if (ogg.VALORI_SELEZIONATI != null && ogg.VALORI_SELEZIONATI.Count > 0)
                                    {
                                        foreach (string valore in ogg.VALORI_SELEZIONATI)
                                        {
                                            if (!string.IsNullOrEmpty(valore))
                                            {
                                                Query qt = InitQuery.getInstance().getQuery("I_DPA_ASS_POLICY_PROFILAZIONE");
                                                qt.setParam("id_policy", policy.id);
                                                qt.setParam("id_template", policy.template.SYSTEM_ID.ToString());
                                                qt.setParam("obj_id", ogg.SYSTEM_ID.ToString());
                                                qt.setParam("obj_value", valore);
                                                command = qt.getSQL();
                                                logger.Debug(command);
                                                if (!dbProvider.ExecuteNonQuery(command))
                                                    throw new Exception(dbProvider.LastExceptionMessage);
                                            }
                                        }
                                    }
                                }
                                else if (ogg.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") && !string.IsNullOrEmpty(ogg.ID_AOO_RF))
                                {
                                    if (!string.IsNullOrEmpty(ogg.VALORE_DATABASE))
                                    {
                                        Query qt = InitQuery.getInstance().getQuery("I_DPA_ASS_POLICY_PROFILAZIONE");
                                        qt.setParam("id_policy", policy.id);
                                        qt.setParam("id_template", policy.template.SYSTEM_ID.ToString());
                                        qt.setParam("obj_id", ogg.SYSTEM_ID.ToString());
                                        qt.setParam("obj_value", ogg.VALORE_DATABASE + "_" + ogg.ID_AOO_RF);
                                        command = qt.getSQL();
                                        logger.Debug(command);
                                        if (!dbProvider.ExecuteNonQuery(command))
                                            throw new Exception(dbProvider.LastExceptionMessage);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(ogg.VALORE_DATABASE))
                                    {
                                        Query qt = InitQuery.getInstance().getQuery("I_DPA_ASS_POLICY_PROFILAZIONE");
                                        qt.setParam("id_policy", policy.id);
                                        qt.setParam("id_template", policy.template.SYSTEM_ID.ToString());
                                        qt.setParam("obj_id", ogg.SYSTEM_ID.ToString());
                                        qt.setParam("obj_value", ogg.VALORE_DATABASE);
                                        command = qt.getSQL();
                                        logger.Debug(command);
                                        if (!dbProvider.ExecuteNonQuery(command))
                                            throw new Exception(dbProvider.LastExceptionMessage);
                                    }
                                }
                            }

                        }
                    }

                    // Aggiornamento informazioni di esecuzione
                    logger.Debug("Aggiornamento informazioni di esecuzione");
                    Query qExec = InitQuery.getInstance().getQuery("U_DPA_EXEC_POLICY_PARER");
                    qExec.setParam("id", policy.id);
                    qExec.setParam("par", string.Format("DATA_PROSSIMA_ESECUZIONE={0}", DocsPaDbManagement.Functions.Functions.ToDate(this.GetDataProssimaEsecuzione(policy), false)));

                    command = qExec.getSQL();
                    logger.Debug(command);

                    if (!dbProvider.ExecuteNonQuery(command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    result = true;
                    dbProvider.CommitTransaction();

                }
                catch (Exception ex)
                {
                    dbProvider.RollbackTransaction();
                    logger.Debug(ex.Message);
                    result = false;
                }
            }

            logger.Debug("END");

            return result;
        }

        public bool DeletePolicyPARER(string idPolicy)
        {
            logger.Debug("BEGIN");
            bool result = false;

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    dbProvider.BeginTransaction();

                    // Rimozione policy da DPA_POLICY_PARER
                    Query q = InitQuery.getInstance().getQuery("D_POLICY_PARER");
                    q.setParam("id", idPolicy);
                    string command = q.getSQL();
                    logger.Debug(command);

                    if (!dbProvider.ExecuteNonQuery(command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    // Rimozione associazioni file
                    logger.Debug("Rimozione associazioni file...");
                    Query qf = InitQuery.getInstance().getQuery("D_DELETE_DPA_ASS_POLICY_TYPE");
                    qf.setParam("idPolicy", idPolicy);
                    command = qf.getSQL();

                    if (!dbProvider.ExecuteNonQuery(command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    // Rimozione associazione campi profilati
                    logger.Debug("Rimozione associazioni campi profilati...");
                    Query qt = InitQuery.getInstance().getQuery("D_DELETE_DPA_ASS_POLICY_PROFILAZIONE");
                    qt.setParam("idPolicy", idPolicy);
                    command = qt.getSQL();

                    if (!dbProvider.ExecuteNonQuery(command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    dbProvider.CommitTransaction();
                    result = true;
                }
                catch (Exception ex)
                {
                    logger.Debug(ex.Message);
                    dbProvider.RollbackTransaction();
                    result = false;
                }
            }

            logger.Debug("END");
            return result;
        }

        public bool UpdateStatoPolicy(ArrayList lista, InfoUtente utente)
        {
            logger.Debug("BEGIN");
            bool result = false;

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    dbProvider.BeginTransaction();

                    foreach (DocsPaVO.Conservazione.PARER.PolicyPARER policy in lista)
                    {
                        Query q = InitQuery.getInstance().getQuery("U_STATO_POLICY_PARER");
                        q.setParam("id", policy.id);
                        q.setParam("attiva", policy.isAttiva);
                        string command = q.getSQL();
                        logger.Debug(command);

                        if (!dbProvider.ExecuteNonQuery(command))
                            throw new Exception(dbProvider.LastExceptionMessage);

                        string tipo = string.Empty;
                        if (policy.tipo.Equals("D"))
                            tipo = "Documenti";
                        if (policy.tipo.Equals("S"))
                            tipo = "Stampe";
                        
                        if (policy.isAttiva.Equals("1"))
                        {
                            DocsPaVO.Logger.CodAzione.infoOggetto ogg = DocsPaDB.Query_DocsPAWS.Log.getInfoOggetto("AMMPOLICYPARERON", policy.idAmm);

                            Log.InsertLog(utente.userId, utente.idPeople, utente.idGruppo, utente.idAmministrazione, ogg.Oggetto, policy.id, string.Format("Attivazione policy {2} {0} da parte dell'utente {1}", policy.codice, utente.userId, tipo),
                                ogg.Codice, ogg.Descrizione, DocsPaVO.Logger.CodAzione.Esito.OK, null, null);
                        }
                        else
                        {
                            DocsPaVO.Logger.CodAzione.infoOggetto ogg = DocsPaDB.Query_DocsPAWS.Log.getInfoOggetto("AMMPOLICYPAREROFF", policy.idAmm);

                            Log.InsertLog(utente.userId, utente.idPeople, utente.idGruppo, utente.idAmministrazione, ogg.Oggetto, policy.id, string.Format("Disattivazione policy {2} {0} da parte dell'utente {1}", policy.codice, utente.userId, tipo),
                                ogg.Codice, ogg.Descrizione, DocsPaVO.Logger.CodAzione.Esito.OK, null, null);
                        }

                    }

                    dbProvider.CommitTransaction();
                    result = true;
                    
                }
                catch (Exception ex)
                {
                    dbProvider.RollbackTransaction();
                    logger.Debug(ex.Message);
                    result = false;
                }
            }

            logger.Debug("END");
            return result;
        }

        public bool UpdateStatoSingolaPolicy(string idPolicy, string attiva)
        {
            logger.Debug("BEGIN");
            bool result = false;

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    dbProvider.BeginTransaction();

                    Query q = InitQuery.getInstance().getQuery("U_STATO_POLICY_PARER");
                    q.setParam("id", idPolicy);
                    q.setParam("attiva", attiva);
                    string command = q.getSQL();
                    logger.Debug(command);

                    if (!dbProvider.ExecuteNonQuery(command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    dbProvider.CommitTransaction();
                    result = true;
                }
                catch (Exception ex)
                {
                    dbProvider.RollbackTransaction();
                    logger.Debug(ex.Message);
                    result = false;
                }
            }

            logger.Debug("END");
            return result;
        }

        public DocsPaVO.Conservazione.PARER.EsecuzionePolicy GetInfoEsecuzionePolicy(string idPolicy)
        {
            logger.Debug("BEGIN");
            DocsPaVO.Conservazione.PARER.EsecuzionePolicy info = new DocsPaVO.Conservazione.PARER.EsecuzionePolicy();

            try
            {
                Query q = InitQuery.getInstance().getQuery("S_DPA_EXEC_POLICY_PARER");
                q.setParam("id", idPolicy);

                string command = q.getSQL();
                logger.Debug(command);

                using (DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            info.dataUltimaEsecuzione = reader.GetValue(reader.GetOrdinal("ULTIMA_ESECUZIONE")).ToString();
                            info.dataProssimaEsecuzione = reader.GetValue(reader.GetOrdinal("PROSSIMA_ESECUZIONE")).ToString();
                            info.numeroEsecuzioni = reader.GetValue(reader.GetOrdinal("NUM_ESECUZIONI")).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                info = null;
            }

            logger.Debug("END");
            return info;
        }

        public bool SetInfoEsecuzionePolicy(DocsPaVO.Conservazione.PARER.EsecuzionePolicy info)
        {
            logger.Debug("BEGIN");
            bool result = false;

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    dbProvider.BeginTransaction();

                    Query q = InitQuery.getInstance().getQuery("U_DPA_EXEC_POLICY_PARER");
                    q.setParam("id", info.idPolicy);
                    
                    string par = string.Empty;
                    if (!string.IsNullOrEmpty(info.numeroEsecuzioni))
                        par = par + "NUM_ESECUZIONI=" + info.numeroEsecuzioni + ",";
                    if (!string.IsNullOrEmpty(info.dataUltimaEsecuzione))
                        par = par + "DATA_ULTIMA_ESECUZIONE=" + DocsPaDbManagement.Functions.Functions.ToDate(info.dataUltimaEsecuzione, false) + ",";
                    if (!string.IsNullOrEmpty(info.dataProssimaEsecuzione))
                        par = par + "DATA_PROSSIMA_ESECUZIONE=" + DocsPaDbManagement.Functions.Functions.ToDate(info.dataProssimaEsecuzione, false);
                    else
                        par = par + "DATA_PROSSIMA_ESECUZIONE=''";

                    if (par.EndsWith(","))
                        par.Remove(par.Length - 1);
                    q.setParam("par", par);

                    string command = q.getSQL();
                    logger.Debug(command);

                    if (!dbProvider.ExecuteNonQuery(command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    dbProvider.CommitTransaction();
                    result = true;
                }
                catch (Exception ex)
                {
                    logger.Debug(ex.Message);
                    dbProvider.RollbackTransaction();
                    result = false;
                }
            }

            logger.Debug("END");
            return result;
        }

        public bool SetInfoVersamentoAutomatico(string idDoc, DocsPaVO.Conservazione.PARER.EsecuzionePolicy info)
        {
            logger.Debug("BEGIN");
            bool result = false;

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    dbProvider.BeginTransaction();
                    Query q = InitQuery.getInstance().getQuery("I_DPA_VERSAMENTO_POLICY_PARER");
                    q.setParam("idDoc", idDoc);
                    q.setParam("idPolicy", info.idPolicy);
                    q.setParam("numExec", info.numeroEsecuzioni);

                    string command = q.getSQL();
                    logger.Debug(command);

                    if (!dbProvider.ExecuteNonQuery(command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    dbProvider.CommitTransaction();
                    result = true;
                }
                catch (Exception ex)
                {
                    dbProvider.RollbackTransaction();
                    logger.Debug(ex.Message);
                    result = false;
                }
            }

            logger.Debug("END");
            return result;
        }

        public DocsPaVO.documento.InfoDocumento[] GetListaDocFromPolicy(DocsPaVO.Conservazione.PARER.PolicyPARER policy)
        {
            InfoDocumento[] result = null;
            string filtri = string.Empty;
            string queryFrom = string.Empty;

            if (policy.tipo.Equals("D"))
            {
                // Filtro su oggetto documento per escludere report di esecuzione policy e report versamenti
                string filtroOgg = " AND UPPER(A.VAR_PROF_OGGETTO) NOT LIKE '%REPORT ESECUZIONE POLICY%' AND UPPER(A.VAR_PROF_OGGETTO) NOT LIKE '%REPORT VERSAMENTI%' ";
                // Filtro per escludere repertori configurati per la conservazione per i quali non sia ancora scattato il contatore
                string filtroRep = " AND NOT EXISTS(SELECT 'X' FROM PROFILE PP, DPA_REGISTRI_REPERTORIO RR, DPA_OGGETTI_CUSTOM OC WHERE PP.ID_TIPO_ATTO=RR.TIPOLOGYID AND RR.COUNTERID=OC.SYSTEM_ID AND GETSEGNATURAREPERTORIO(PP.DOCNUMBER, C.ID_AMM) IS NULL AND OC.CHA_CONS_REPERTORIO='1' AND PP.SYSTEM_ID=A.SYSTEM_ID) ";

                //filtri = filtroOgg + " AND A.ID_UO_CREATORE=C.SYSTEM_ID AND C.ID_AMM=" + policy.idAmm + this.GetFiltriPolicy(policy);
                //queryFrom = ", DPA_CORR_GLOBALI C";
                filtri = filtroOgg + filtroRep + " AND C.ID_AMM=" + policy.idAmm + this.GetFiltriPolicy(policy);
                queryFrom = " LEFT JOIN DPA_CORR_GLOBALI C ON A.ID_UO_CREATORE=C.SYSTEM_ID LEFT JOIN DPA_VERSAMENTO V ON A.DOCNUMBER=V.ID_PROFILE ";
            }
            if (policy.tipo.Equals("S"))
            {
                if (policy.tipoRegistroStampa.Equals("R"))
                {
                    // stampe registro di protocollo
                    //queryFrom = ", DPA_EL_REGISTRI E, DPA_STAMPAREGISTRI R";
                    queryFrom = " LEFT JOIN DPA_STAMPAREGISTRI R ON A.DOCNUMBER=R.DOCNUMBER LEFT JOIN DPA_EL_REGISTRI E ON E.SYSTEM_ID=R.ID_REGISTRO ";
                    //filtri = " AND R.ID_REGISTRO=E.SYSTEM_ID AND E.ID_AMM=" + policy.idAmm;
                    filtri = " AND E.ID_AMM=" + policy.idAmm;
                }
                if (policy.tipoRegistroStampa.Equals("C"))
                {
                    // stampe registro di repertorio
                    //queryFrom = ", DPA_STAMPA_REPERTORI R";
                    queryFrom = " LEFT JOIN DPA_STAMPA_REPERTORI R ON A.DOCNUMBER=R.DOCNUMBER ";
                    filtri = string.Format(" AND R.ID_REPERTORIO IN (SELECT DISTINCT RREP.COUNTERID FROM DPA_REGISTRI_REPERTORIO RREP, DPA_TIPO_ATTO T, DPA_OGGETTI_CUSTOM O WHERE RREP.TIPOLOGYID=T.SYSTEM_ID AND O.SYSTEM_ID=RREP.COUNTERID AND O.CHA_CONS_REPERTORIO=1 AND T.ID_AMM={0})", policy.idAmm);
                }
                //filtri = filtri + " AND A.DOCNUMBER=R.DOCNUMBER " + this.GetFiltriPolicyStampe(policy);
                queryFrom = queryFrom + " LEFT JOIN DPA_VERSAMENTO V ON A.DOCNUMBER=V.ID_PROFILE ";
                filtri = filtri + this.GetFiltriPolicyStampe(policy);
            }

            DataSet ds = new DataSet();
            Query q = InitQuery.getInstance().getQuery("S_GET_DOC_FROM_POLICY_PARER");
            
            q.setParam("queryFrom", queryFrom);
            q.setParam("filtri", filtri);

            string command = q.getSQL();
            logger.Debug(command);

            using (DBProvider dbProvider = new DBProvider())
            {
                if (!dbProvider.ExecuteQuery(ds, command))
                    throw new Exception(dbProvider.LastExceptionMessage);           

                if (ds != null && ds.Tables != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    result = new InfoDocumento[ds.Tables[0].Rows.Count];
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.documento.InfoDocumento tempInfo = new DocsPaVO.documento.InfoDocumento();
                        tempInfo.idProfile = ds.Tables[0].Rows[i]["system_id"].ToString();
                        tempInfo.docNumber = ds.Tables[0].Rows[i]["docnumber"].ToString();
                        tempInfo.tipoProto = ds.Tables[0].Rows[i]["cha_tipo_proto"].ToString();
                        tempInfo.oggetto = ds.Tables[0].Rows[i]["var_prof_oggetto"].ToString();
                        if(tempInfo.tipoProto.Equals("A") || tempInfo.tipoProto.Equals("P") || tempInfo.tipoProto.Equals("I")) 
                        {
                            tempInfo.numProt = ds.Tables[0].Rows[i]["num_proto"].ToString() + " / " + ds.Tables[0].Rows[i]["num_anno_proto"].ToString();
                            tempInfo.dataApertura = ds.Tables[0].Rows[i]["data_proto"].ToString();
                        }
                        else
                        {
                            tempInfo.dataApertura = ds.Tables[0].Rows[i]["data_creazione"].ToString();
                        }

                        result[i] = tempInfo;
                    }
                }
            }


            return result;
        }

        public string GetCountDocumentiFromPolicy(DocsPaVO.Conservazione.PARER.PolicyPARER policy, string idAmm)
        {
            logger.Debug("BEGIN");

            string result = string.Empty;
            string filtri = string.Empty;
            string queryFrom = string.Empty;

            Query q = InitQuery.getInstance().getQuery("S_GET_COUNT_DOC_FROM_POLICY");
            //q.setParam("idAmm", idAmm);
            if (policy.tipo.Equals("D"))
            {
                // Filtro su oggetto documento per escludere report di esecuzione policy e report versamenti
                string filtroOgg = " AND UPPER(A.VAR_PROF_OGGETTO) NOT LIKE '%REPORT ESECUZIONE POLICY%' AND UPPER(A.VAR_PROF_OGGETTO) NOT LIKE '%REPORT VERSAMENTI%' ";
                // Filtro per escludere repertori configurati per la conservazione per i quali non sia ancora scattato il contatore
                string filtroRep = " AND NOT EXISTS(SELECT 'X' FROM PROFILE PP, DPA_REGISTRI_REPERTORIO RR, DPA_OGGETTI_CUSTOM OC WHERE PP.ID_TIPO_ATTO=RR.TIPOLOGYID AND RR.COUNTERID=OC.SYSTEM_ID AND GETSEGNATURAREPERTORIO(PP.DOCNUMBER, C.ID_AMM) IS NULL AND OC.CHA_CONS_REPERTORIO='1' AND PP.SYSTEM_ID=A.SYSTEM_ID) ";

                //filtri = filtroOgg + " AND A.ID_UO_CREATORE=C.SYSTEM_ID AND C.ID_AMM=" + idAmm + this.GetFiltriPolicy(policy);
                filtri = filtroOgg + filtroRep + " AND C.ID_AMM=" + idAmm + this.GetFiltriPolicy(policy);
                queryFrom = " LEFT JOIN DPA_CORR_GLOBALI C ON A.ID_UO_CREATORE=C.SYSTEM_ID LEFT JOIN DPA_VERSAMENTO V ON A.DOCNUMBER=V.ID_PROFILE ";
            }
            if (policy.tipo.Equals("S"))
            {
                if (policy.tipoRegistroStampa.Equals("R"))
                {
                    // stampe registro di protocollo
                    //queryFrom = ", DPA_EL_REGISTRI E, DPA_STAMPAREGISTRI R";
                    queryFrom = " LEFT JOIN DPA_STAMPAREGISTRI R ON A.DOCNUMBER=R.DOCNUMBER LEFT JOIN DPA_EL_REGISTRI E ON E.SYSTEM_ID=R.ID_REGISTRO ";
                    //filtri = " AND R.ID_REGISTRO=E.SYSTEM_ID AND E.ID_AMM=" + idAmm;
                    filtri = " AND E.ID_AMM=" + policy.idAmm;
                }
                if (policy.tipoRegistroStampa.Equals("C"))
                {
                    // stampe registro di repertorio
                    //queryFrom = ", DPA_STAMPA_REPERTORI R";
                    //filtri = string.Format(" AND R.ID_REPERTORIO IN (SELECT DISTINCT RREP.COUNTERID FROM DPA_REGISTRI_REPERTORIO RREP, DPA_TIPO_ATTO T, DPA_OGGETTI_CUSTOM O WHERE RREP.TIPOLOGYID=T.SYSTEM_ID AND O.SYSTEM_ID=RREP.COUNTERID AND O.CHA_CONS_REPERTORIO=1 AND T.ID_AMM={0})", idAmm);
                    queryFrom = " LEFT JOIN DPA_STAMPA_REPERTORI R ON A.DOCNUMBER=R.DOCNUMBER ";
                    filtri = string.Format(" AND R.ID_REPERTORIO IN (SELECT DISTINCT RREP.COUNTERID FROM DPA_REGISTRI_REPERTORIO RREP, DPA_TIPO_ATTO T, DPA_OGGETTI_CUSTOM O WHERE RREP.TIPOLOGYID=T.SYSTEM_ID AND O.SYSTEM_ID=RREP.COUNTERID AND O.CHA_CONS_REPERTORIO=1 AND T.ID_AMM={0})", policy.idAmm);
                }
                queryFrom = queryFrom + " LEFT JOIN DPA_VERSAMENTO V ON A.DOCNUMBER=V.ID_PROFILE ";
                filtri = filtri + " AND A.DOCNUMBER=R.DOCNUMBER " + this.GetFiltriPolicyStampe(policy);
            }

            q.setParam("filtri", filtri);
            q.setParam("queryFrom", queryFrom);

            string command = q.getSQL();
            logger.Debug(command);

            using (DBProvider dbProvider = new DBProvider())
            {
                if (!dbProvider.ExecuteScalar(out result, command))
                    throw new Exception(dbProvider.LastExceptionMessage);
            }

            logger.Debug("END");
            return result;
        }

        private string GetFiltriPolicy(DocsPaVO.Conservazione.PARER.PolicyPARER policy)
        {
            logger.Debug("BEGIN");
            string result = string.Empty;
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
            string userDb = string.Empty;
            string from = string.Empty;

            string anno = string.Empty;

            if (dbType.ToUpper().Equals("SQL"))
                userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession() + ".";

            #region Stato Conservazione
            if (!string.IsNullOrEmpty(policy.statoVersamento))
            {
                if (policy.statoVersamento.Equals("R"))
                    //result = result + " AND A.DOCNUMBER IN (SELECT ID_PROFILE FROM DPA_VERSAMENTO WHERE CHA_STATO='R') ";
                    result = result + " AND V.CHA_STATO='R' ";
                if (policy.statoVersamento.Equals("F"))
                    //result = result + " AND A.DOCNUMBER IN (SELECT ID_PROFILE FROM DPA_VERSAMENTO WHERE CHA_STATO='F') ";
                    result = result + " AND V.CHA_STATO='F' ";
            }
            else
            {
                //result = result + " AND A.DOCNUMBER NOT IN (SELECT ID_PROFILE FROM DPA_VERSAMENTO) ";
                result = result + " AND V.CHA_STATO IS NULL ";
            }
            #endregion

            #region Tipo documento
            if (policy.arrivo || policy.partenza || policy.interno || policy.grigio)
            {
                logger.Debug("filtro tipo documento");
                int tipi = 0;
                result = result + " AND (";
                if (policy.arrivo)
                {
                    if (tipi.Equals(0))
                    {
                        result = result + " (a.cha_tipo_proto IN ('A') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                        tipi++;
                    }
                    else
                    {
                        result = result + " OR (a.cha_tipo_proto IN ('A') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                    }
                }
                if (policy.partenza)
                {
                    if (tipi.Equals(0))
                    {
                        result = result + " (a.cha_tipo_proto IN ('P') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                        tipi++;
                    }
                    else
                    {
                        result = result + " OR (a.cha_tipo_proto IN ('P') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                    }
                }
                if (policy.interno)
                {
                    if (tipi.Equals(0))
                    {
                        result = result + " (a.cha_tipo_proto IN ('I') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                        tipi++;
                    }
                    else
                    {
                        result = result + " OR (a.cha_tipo_proto IN ('I') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                    }
                }
                if (policy.grigio)
                {
                    if (tipi.Equals(0))
                    {
                        result = result + " (a.cha_tipo_proto IN ('G') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                        tipi++;
                    }
                    else
                    {
                        result = result + " OR (a.cha_tipo_proto IN ('G') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                    }
                }
                result = result + " ) ";
            }

            #endregion

            #region Tipologie
            if (!string.IsNullOrEmpty(policy.idTemplate) && !policy.idTemplate.Equals("0"))
            {
                if (policy.idTemplate.Equals("-1"))
                {
                    // nessuna tipologia
                    result = result + " AND A.ID_TIPO_ATTO IS NULL ";
                }
                else
                {
                    result = result + " AND A.ID_TIPO_ATTO=" + policy.idTemplate + " ";
                    if (policy.template != null && policy.template.ELENCO_OGGETTI != null && policy.template.ELENCO_OGGETTI.Count > 0)
                    {
                        DocsPaDB.Query_DocsPAWS.Model model = new Model();
                        result = result + model.getSeriePerRicercaProfilazione(policy.template, string.Empty);
                    }
                }
            }
            #endregion

            #region Diagrammi di stato
            if (!string.IsNullOrEmpty(policy.idStato) && !policy.idStato.Equals("0"))
            {
                if (!string.IsNullOrEmpty(policy.operatoreStato))
                {
                    if (policy.operatoreStato.Equals("1"))
                    {
                        // uguale a
                        result = result + string.Format(" AND A.SYSTEM_ID IN (SELECT DOC_NUMBER FROM DPA_DIAGRAMMI WHERE DPA_DIAGRAMMI.ID_STATO={0}) ", policy.idStato);
                    }
                    if (policy.operatoreStato.Equals("0"))
                    {
                        // diverso da
                        result = result + string.Format(" AND A.SYSTEM_ID IN (SELECT DOC_NUMBER FROM DPA_DIAGRAMMI WHERE DPA_DIAGRAMMI.ID_STATO != {0}) ", policy.idStato);
                    }
                }
            }
            #endregion

            #region Registro/AOO
            if (!string.IsNullOrEmpty(policy.idRegistro) && !policy.idRegistro.Equals("0"))
            {
                result = result + " AND A.ID_REGISTRO = " + policy.idRegistro + " ";
            }
            #endregion

            #region RF
            if (!string.IsNullOrEmpty(policy.idRF) && !policy.idRF.Equals("0"))
            {
                result = result + " AND A.ID_RUOLO_CREATORE IN (select el.ID_RUOLO_IN_UO from dpa_l_ruolo_reg el where el.ID_REGISTRO = " + policy.idRF + ") ";
            }
            #endregion

            #region UO creatore
            if (!string.IsNullOrEmpty(policy.idUO) && !policy.idUO.Equals("0"))
            {
                if (!string.IsNullOrEmpty(policy.UOsottoposte) && policy.UOsottoposte.Equals("1"))
                {
                    if (dbType.ToUpper().Equals("SQL"))
                    {
                        result = result + " AND a.ID_UO_CREATORE IN (select system_id from " + userDb + "fn_CONS_getSottoalberoUO(" + policy.idUO + "," + policy.idAmm + ") )";
                    }
                    else
                    {
                        result = result + " AND a.ID_UO_CREATORE IN (select p.SYSTEM_ID from dpa_corr_globali p start with p.SYSTEM_ID = " + policy.idUO + " connect by prior p.SYSTEM_ID = p.ID_PARENT AND p.CHA_TIPO_URP = 'U' AND p.ID_AMM=" + policy.idAmm + ")";
                    }
                }
                else
                {
                    result = result + " AND A.ID_UO_CREATORE = " + policy.idUO + " ";
                }
            }
            #endregion

            #region Classificazione
            if (!string.IsNullOrEmpty(policy.idFascicolo) && !policy.idFascicolo.Equals("0"))
            {
                if (!string.IsNullOrEmpty(policy.tipoClassificazione))
                {
                    string queryGer = string.Empty;
                    if (policy.tipoClassificazione.Equals("C"))
                    {
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            queryGer = "SELECT SYSTEM_ID FROM " + userDb + "fn_CONS_getSottoFascFolderGen(" + policy.idFascicolo + ") ";
                            result = result + string.Format(" AND A.SYSTEM_ID IN (SELECT DISTINCT LINK FROM PROJECT_COMPONENTS WHERE PROJECT_ID IN ({0})) ", queryGer);
                        }
                        else
                        {
                            //result = result + string.Format(" AND A.SYSTEM_ID IN (SELECT DISTINCT LINK FROM PROJECT_COMPONENTS WHERE PROJECT_ID IN (SELECT SYSTEM_ID FROM PROJECT WHERE CHA_TIPO_PROJ != 'T' START WITH SYSTEM_ID={0} AND CHA_TIPO_FASCICOLO != 'P' CONNECT BY PRIOR SYSTEM_ID=ID_PARENT ) ) ", policy.idFascicolo);
                            queryGer = "SELECT DISTINCT LINK FROM PROJECT_COMPONENTS PC, PROJECT P WHERE PC.PROJECT_ID=P.SYSTEM_ID AND " +
                                       "P.ID_PARENT IN (SELECT SYSTEM_ID FROM PROJECT WHERE CHA_TIPO_PROJ='F' AND CHA_TIPO_FASCICOLO='G' " +
                                       "CONNECT BY PRIOR SYSTEM_ID=ID_PARENT START WITH SYSTEM_ID IN (SELECT ID_PARENT FROM PROJECT WHERE SYSTEM_ID=" + policy.idFascicolo +
                                       "))";
                            result = result + string.Format(" AND A.SYSTEM_ID IN ({0}) ", queryGer);
                        }
                    }
                    if (policy.tipoClassificazione.Equals("F"))
                    {
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            queryGer = "SELECT SYSTEM_ID FROM " + userDb + "fn_CONS_getSottoFascFolderProc(" + policy.idFascicolo + ") ";
                            result = result + string.Format(" AND A.SYSTEM_ID IN (SELECT DISTINCT LINK FROM PROJECT_COMPONENTS WHERE PROJECT_ID IN ({0})) ", queryGer);
                        }
                        else
                        {
                            //result = result + string.Format(" AND A.SYSTEM_ID IN (SELECT DISTINCT LINK FROM PROJECT_COMPONENTS WHERE PROJECT_ID IN (SELECT SYSTEM_ID FROM PROJECT WHERE CHA_TIPO_PROJ != 'T' START WITH SYSTEM_ID={0} AND CHA_TIPO_FASCICOLO = 'P' CONNECT BY PRIOR SYSTEM_ID=ID_PARENT ) ) ", policy.idFascicolo);
                            queryGer = "SELECT DISTINCT LINK FROM PROJECT_COMPONENTS PC, PROJECT P WHERE PC.PROJECT_ID=P.SYSTEM_ID AND " +
                                       "P.ID_PARENT IN (SELECT SYSTEM_ID FROM PROJECT WHERE CHA_TIPO_PROJ='F' AND CHA_TIPO_FASCICOLO='P' " +
                                       "CONNECT BY PRIOR SYSTEM_ID=ID_PARENT START WITH SYSTEM_ID IN (SELECT ID_PARENT FROM PROJECT WHERE SYSTEM_ID=" + policy.idFascicolo +
                                       "))";
                            result = result + string.Format(" AND A.SYSTEM_ID IN ({0}) ", queryGer);
                        }
                    }
                    if (policy.tipoClassificazione.Equals("CF"))
                    {
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            queryGer = "SELECT SYSTEM_ID FROM " + userDb + "fn_CONS_getSottoFascFolder(" + policy.idFascicolo + ") ";
                            result = result + string.Format(" AND A.SYSTEM_ID IN (SELECT DISTINCT LINK FROM PROJECT_COMPONENTS WHERE PROJECT_ID IN ({0})) ", queryGer);
                        }
                        else
                        {
                            //result = result + string.Format(" AND A.SYSTEM_ID IN (SELECT DISTINCT LINK FROM PROJECT_COMPONENTS WHERE PROJECT_ID IN (SELECT SYSTEM_ID FROM PROJECT WHERE CHA_TIPO_PROJ != 'T' START WITH SYSTEM_ID={0} CONNECT BY PRIOR SYSTEM_ID=ID_PARENT ) ) ", policy.idFascicolo);
                            queryGer = "SELECT DISTINCT LINK FROM PROJECT_COMPONENTS PC, PROJECT P WHERE PC.PROJECT_ID=P.SYSTEM_ID AND " +
                                       "P.ID_PARENT IN (SELECT SYSTEM_ID FROM PROJECT WHERE CHA_TIPO_PROJ='F' AND CHA_TIPO_FASCICOLO IN ('P', 'G') " +
                                       "CONNECT BY PRIOR SYSTEM_ID=ID_PARENT START WITH SYSTEM_ID IN (SELECT ID_PARENT FROM PROJECT WHERE SYSTEM_ID=" + policy.idFascicolo +
                                       "))";
                            result = result + string.Format(" AND A.SYSTEM_ID IN ({0}) ", queryGer);
                        }
                    }
                }
            }
            #endregion

            #region Documenti digitali
            if (!string.IsNullOrEmpty(policy.digitali) && policy.digitali.Equals("1"))
            {
                if (dbType.ToUpper().Equals("SQL"))
                {
                    result = result + " AND @dbuser@.AtLeastOneCartaceo(A.DOCNUMBER) != '1'";
                }
                else
                {
                    result = result + " AND AtLeastOneCartaceo(A.DOCNUMBER) != '1'";
                }
            }
            #endregion

            #region Formati documenti
            if (policy.FormatiDocumento != null && policy.FormatiDocumento.Count > 0)
            {
                string listaFormati = string.Empty;
                for (int i = 0; i < policy.FormatiDocumento.Count; i++)
                {
                    listaFormati = listaFormati + "'" + policy.FormatiDocumento[i].FileExtension.ToUpper() + "'";
                    if (i < policy.FormatiDocumento.Count - 1)
                        listaFormati = listaFormati + ",";
                }

                if (dbType.ToUpper().Equals("SQL"))
                {
                    result = result + string.Format(" AND UPPER(@dbuser@.getchaimg (a.docnumber)) IN ({0}) AND NOT EXISTS (SELECT 'x' FROM PROFILE WHERE ID_DOCUMENTO_PRINCIPALE=A.DOCNUMBER AND UPPER(@dbuser@.getchaimg(SYSTEM_ID)) NOT IN ({0}) ) ", listaFormati);
                }
                else
                {
                    result = result + string.Format(" AND UPPER(getchaimg (a.docnumber)) IN ({0}) AND NOT EXISTS (SELECT 'x' FROM PROFILE WHERE ID_DOCUMENTO_PRINCIPALE=A.DOCNUMBER AND UPPER(getchaimg(SYSTEM_ID)) NOT IN ({0}) ) ", listaFormati);
                }

                #region codice commentato
                //result = result + " AND (";               
                //for (int i = 0; i < policy.FormatiDocumento.Count; i++)
                //{
                //    if (dbType.ToUpper().Equals("SQL"))
                //    {
                //        result = result + " UPPER(@dbuser@.getchaimg (a.docnumber)) ='" + policy.FormatiDocumento[i].FileExtension.ToUpper() + "'";
                //    }
                //    else
                //    {
                //        result = result + " UPPER(getchaimg (a.docnumber)) ='" + policy.FormatiDocumento[i].FileExtension.ToUpper() + "'";
                //    }
                //    if (i < policy.FormatiDocumento.Count - 1)
                //    {
                //        result = result + " OR ";
                //    }
                //}
                //result = result + ") ";
                #endregion
            }
            #endregion

            #region Documenti firmati
            if (!string.IsNullOrEmpty(policy.firmati))
            {
                if (policy.firmati.Equals("1"))
                {
                    // solo firmati
                    if (dbType.ToUpper().Equals("SQL"))
                    {
                        result = result + "AND @dbuser@.AtLeastOneFirmato(A.DOCNUMBER) = '1'";
                    }
                    else
                    {
                        result = result + "AND AtLeastOneFirmato(A.DOCNUMBER) = '1'";
                    }
                }
                if (policy.firmati.Equals("0"))
                {
                    // solo non firmati
                    if (dbType.ToUpper().Equals("SQL"))
                    {
                        result = result + "AND @dbuser@.AtLeastOneFirmato(A.DOCNUMBER) = '0'";
                    }
                    else
                    {
                        result = result + "AND AtLeastOneFirmato(A.DOCNUMBER) = '0'";
                    }
                }
            }
            #endregion

            #region Documenti marcati
            if (!string.IsNullOrEmpty(policy.marcati))
            {
                if (policy.marcati.Equals("1"))
                {
                    // con timestamp
                    //result = result + "AND a.docnumber in (select distinct doc_number from dpa_timestamp_doc) ";
                    result = result + " AND AtLeastOneMarcato(A.DOCNUMBER) = '1' ";
                    if (!string.IsNullOrEmpty(policy.scadenzaMarca) && !policy.scadenzaMarca.Equals("0"))
                    {
                        //string expString = " AND a.docnumber in (select doc_number from (select distinct doc_number, max(version_id) over(partition by doc_number) as max_vers, max(dta_scadenza) over (partition by doc_number) as max_scad from dpa_timestamp_doc) where max_scad "
                        string expString = " AND (a.docnumber in {0} OR a.docnumber in (select id_documento_principale from profile where system_id in {0} ) )";
                        string cond = "(select doc_number from (select distinct doc_number, max(version_id) over(partition by doc_number) as max_vers, max(dta_scadenza) over (partition by doc_number) as max_scad from dpa_timestamp_doc) where max_scad ";
                        switch (policy.scadenzaMarca)
                        {
                            // scaduti
                            case "E":
                                //result = result + expString +  " <= " + DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.Date.ToString()) + ") ";
                                cond = cond + " <= " + DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.Date.ToString()) + ") ";
                                result = result + string.Format(expString, cond);
                                break;
                            // settimana corrente
                            case "W":
                                if (dbType.ToUpper().Equals("SQL"))
                                {
                                    //result = result + expString + ">=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND max_scad<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE()))) ";
                                    cond = cond + ">=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND max_scad<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE()))) ";
                                }
                                else
                                {
                                    //result = result + expString + ">=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND max_scad<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual)) ";
                                    cond = cond + ">=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND max_scad<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual)) ";
                                }
                                result = result + string.Format(expString, cond);
                                break;
                            // mese corrente
                            case "M":
                                if (dbType.ToUpper().Equals("SQL"))
                                {
                                    //result = result + expString + ">=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND max_scad <=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))))  ";
                                    cond = cond + ">=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND max_scad <=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))))  ";
                                }
                                else
                                {
                                    //result = result + expString + ">= Trunc(Sysdate,'MM') AND  max_scad < add_months(trunc(sysdate, 'MM'), 1))";
                                    cond = cond + ">= Trunc(Sysdate,'MM') AND  max_scad < add_months(trunc(sysdate, 'MM'), 1))";
                                }
                                result = result + string.Format(expString, cond);
                                break;
                            // anno corrente
                            case "Y":
                                anno = DateTime.Now.Year.ToString();
                                //result = result + expString + " BETWEEN " + DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("01/01/{0}", anno), true);
                                //result = result + " AND " + DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("31/12/{0}", anno), false) + ") ";
                                cond = cond + " BETWEEN " + DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("01/01/{0}", anno), true);
                                cond = cond + " AND " + DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("31/12/{0}", anno), false) + ") ";
                                result = result + string.Format(expString, cond);
                                break;
                        }
                    }
                }
                if (policy.marcati.Equals("0"))
                {
                    // senza timestamp
                    //result = result + "AND a.docnumber not in (select distinct doc_number from dpa_timestamp_doc) ";
                    result = result + " AND AtLeastOneMarcato(A.DOCNUMBER) = '0' ";
                }
            }
            #endregion

            #region Data creazione
            if (!string.IsNullOrEmpty(policy.filtroDataCreazione) && !policy.filtroDataCreazione.Equals("0"))
            {
                switch (policy.filtroDataCreazione)
                {
                    // Valore singolo
                    case "S":
                        if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                        {
                            result = result + " AND A.CREATION_TIME >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(policy.dataCreazioneDa, true);
                            result = result + " AND A.CREATION_TIME <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(policy.dataCreazioneDa, false);
                        }
                        break;
                    // Intervallo
                    case "R":
                        if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                        {
                            result = result + " AND A.CREATION_TIME >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(policy.dataCreazioneDa, true);
                        }
                        if (!string.IsNullOrEmpty(policy.dataCreazioneA))
                        {
                            result = result + " AND A.CREATION_TIME <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(policy.dataCreazioneA, false);
                        }
                        break;
                    // Oggi
                    case "T":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            result = result + " AND DATEDIFF(DD, A.CREATION_TIME, GETDATE()) = 0 ";
                        }
                        else
                        {
                            result = result + " AND A.CREATION_TIME between trunc(sysdate ,'DD') and sysdate ";
                        }
                        break;
                    // Settimana corrente
                    case "W":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            result = result + "AND A.CREATION_TIME>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND A.CREATION_TIME<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE()))  ";
                        }
                        else
                        {
                            result = result + " AND A.CREATION_TIME>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND A.CREATION_TIME<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                        }
                        break;
                    // Mese corrente
                    case "M":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            result = result + "AND A.CREATION_TIME>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND A.CREATION_TIME<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                        }
                        else
                        {
                            result = result + " AND A.CREATION_TIME>= Trunc(Sysdate,'MM')    AND A.CREATION_TIME<(Sysdate+1 ) ";
                        }
                        break;
                    // Anno corrente
                    case "Y":
                        anno = DateTime.Now.Year.ToString();
                        result = result + " AND A.CREATION_TIME >=" + DocsPaDbManagement.Functions.Functions.ToDate(string.Format("01/01/{0}", anno), true);
                        result = result + " AND A.CREATION_TIME <=" + DocsPaDbManagement.Functions.Functions.ToDate(string.Format("31/12/{0}", anno), true);
                        break;
                    // Ieri
                    case "B":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            // ??
                        }
                        else
                        {
                            result = result + " AND A.CREATION_TIME BETWEEN TRUNC(sysdate-1 ,'DD') and (TRUNC(sysdate,'DD')-1/86400) ";
                        }
                        break;
                    // Settimana precedente
                    case "V":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            // da correggere
                            result = result + "AND A.CREATION_TIME>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND A.CREATION_TIME<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE()))  ";
                        }
                        else
                        {
                            result = result + " AND A.CREATION_TIME>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D'))-7)) startdayofweek from dual) AND A.CREATION_TIME<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D'))-7)) enddayofweek from dual) ";
                        }
                        break;
                    // Mese precedente
                    case "N":
                        string mese = (DateTime.Now.AddMonths(-1)).Month.ToString();
                        string giorni = "31";
                        if (mese.Equals("2"))
                            giorni = DateTime.IsLeapYear(DateTime.Now.Year) ? "29" : "28";
                        if (mese.Equals("4") || mese.Equals("6") || mese.Equals("9") || mese.Equals("11"))
                            giorni = "30";
                        anno = (DateTime.Now.AddMonths(-1)).Year.ToString();
                        result = result + " AND A.CREATION_TIME >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("01/{0}/{1}", mese, anno), true);
                        result = result + " AND A.CREATION_TIME <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("{0}/{1}/{2}", giorni, mese, anno), false);
                        break;
                    // Anno precedente
                    case "X":
                        anno = (DateTime.Now.Year - 1).ToString();
                        result = result + " AND A.CREATION_TIME >=" + DocsPaDbManagement.Functions.Functions.ToDate(string.Format("01/01/{0}", anno), true);
                        result = result + " AND A.CREATION_TIME <=" + DocsPaDbManagement.Functions.Functions.ToDate(string.Format("31/12/{0}", anno), true);
                        break;
                    // Numero giorni prima
                    case "P":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            result = result + string.Format(" AND A.CREATION_TIME BETWEEN CONVERT(DATETIME, DATEDIFF(day, {0}, GETDATE())) AND CONVERT(DATETIME, DATEDIFF(day, {0}-1, GETDATE()))", policy.numGiorniCreazione);
                        }
                        else
                        {
                            result = result + string.Format(" AND A.CREATION_TIME BETWEEN TRUNC(SYSDATE-{0},'DD') AND (TRUNC(SYSDATE-{0}+1, 'DD')-1/86400) ", policy.numGiorniCreazione);
                        }
                        break;
                }
            }
            #endregion

            #region Data protocollazione
            if (!string.IsNullOrEmpty(policy.filtroDataCreazione) && !policy.filtroDataCreazione.Equals("0"))
            {
                switch (policy.filtrodataProtocollazione)
                {
                    // Valore singolo
                    case "S":
                        if (!string.IsNullOrEmpty(policy.dataProtocollazioneDa))
                        {
                            result = result + " AND A.DTA_PROTO >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(policy.dataProtocollazioneDa, true);
                            result = result + " AND A.DTA_PROTO <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(policy.dataProtocollazioneDa, false);
                        }
                        break;
                    // Intervallo
                    case "R":
                        if (!string.IsNullOrEmpty(policy.dataProtocollazioneDa))
                        {
                            result = result + " AND A.DTA_PROTO >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(policy.dataProtocollazioneDa, true);
                        }
                        if (!string.IsNullOrEmpty(policy.dataProtocollazioneA))
                        {
                            result = result + " AND A.DTA_PROTO <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(policy.dataProtocollazioneA, false);
                        }
                        break;
                    // Oggi
                    case "T":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            result = result + " AND DATEDIFF(DD, A.DTA_PROTO, GETDATE()) = 0 ";
                        }
                        else
                        {
                            result = result + " AND A.DTA_PROTO between trunc(sysdate ,'DD') and sysdate ";
                        }
                        break;
                    // Settimana corrente
                    case "W":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            result = result + "AND A.DTA_PROTO>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND A.DTA_PROTO<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE()))  ";
                        }
                        else
                        {
                            result = result + " AND A.DTA_PROTO>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND A.DTA_PROTO<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                        }
                        break;
                    // Mese corrente
                    case "M":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            result = result + "AND A.DTA_PROTO>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND A.DTA_PROTO<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                        }
                        else
                        {
                            result = result + " AND A.DTA_PROTO>= Trunc(Sysdate,'MM')    AND A.DTA_PROTO<(Sysdate+1 ) ";
                        }
                        break;
                    // Anno corrente
                    case "Y":
                        anno = DateTime.Now.Year.ToString();
                        result = result + " AND A.DTA_PROTO >=" + DocsPaDbManagement.Functions.Functions.ToDate(string.Format("01/01/{0}", anno), true);
                        result = result + " AND A.DTA_PROTO <=" + DocsPaDbManagement.Functions.Functions.ToDate(string.Format("31/12/{0}", anno), true);
                        break;
                    // Ieri
                    case "B":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            // ??
                        }
                        else
                        {
                            result = result + " AND A.DTA_PROTO BETWEEN TRUNC(sysdate-1 ,'DD') and (TRUNC(sysdate,'DD')-1/86400) ";
                        }
                        break;
                    // Settimana precedente
                    case "V":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            // da correggere
                            result = result + "AND A.DTA_PROTO>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND A.DTA_PROTO<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE()))  ";
                        }
                        else
                        {
                            result = result + " AND A.DTA_PROTO>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D'))-7)) startdayofweek from dual) AND A.DTA_PROTO<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D'))-7)) enddayofweek from dual) ";
                        }
                        break;
                    // Mese precedente
                    case "N":
                        string mese = (DateTime.Now.AddMonths(-1)).Month.ToString();
                        string giorni = "31";
                        if (mese.Equals("2"))
                            giorni = DateTime.IsLeapYear(DateTime.Now.Year) ? "29" : "28";
                        if (mese.Equals("4") || mese.Equals("6") || mese.Equals("9") || mese.Equals("11"))
                            giorni = "30";
                        anno = (DateTime.Now.AddMonths(-1)).Year.ToString();
                        result = result + " AND A.DTA_PROTO >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("01/{0}/{1}", mese, anno), true);
                        result = result + " AND A.DTA_PROTO <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("{0}/{1}/{2}", giorni, mese, anno), false);
                        break;
                    // Anno precedente
                    case "X":
                        anno = (DateTime.Now.Year - 1).ToString();
                        result = result + " AND A.DTA_PROTO >=" + DocsPaDbManagement.Functions.Functions.ToDate(string.Format("01/01/{0}", anno), true);
                        result = result + " AND A.DTA_PROTO <=" + DocsPaDbManagement.Functions.Functions.ToDate(string.Format("31/12/{0}", anno), true);
                        break;
                    // Numero giorni prima
                    case "P":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            // da verificare
                            result = result + string.Format(" AND A.DTA_PROTO BETWEEN CONVERT(DATETIME, DATEDIFF(day, {0}, GETDATE())) AND CONVERT(DATETIME, DATEDIFF(day, {0}-1, GETDATE()))", policy.numGiorniProtocollazione);
                        }
                        else
                        {
                            result = result + string.Format(" AND A.DTA_PROTO BETWEEN TRUNC(SYSDATE-{0},'DD') AND (TRUNC(SYSDATE-{0}+1,'DD')-1/86400) ", policy.numGiorniProtocollazione);
                        }
                        break;
                }
            }
            #endregion

            logger.Debug("END");
            return result;
        }

        private string GetFiltriPolicyStampe(DocsPaVO.Conservazione.PARER.PolicyPARER policy)
        {
            logger.Debug("BEGIN");
            string result = string.Empty;

            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
            string userDb = string.Empty;
            if (dbType.ToUpper().Equals("SQL"))
                userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession() + ".";

            #region Stato Conservazione
            if (!string.IsNullOrEmpty(policy.statoVersamento))
            {
                if (policy.statoVersamento.Equals("R"))
                    //result = result + " AND A.DOCNUMBER IN (SELECT ID_PROFILE FROM DPA_VERSAMENTO WHERE CHA_STATO='R') ";
                    result = result + " AND V.CHA_STATO='R' ";
                if (policy.statoVersamento.Equals("F"))
                    // result = result + " AND A.DOCNUMBER IN (SELECT ID_PROFILE FROM DPA_VERSAMENTO WHERE CHA_STATO='F') ";
                    result = result + " AND V.CHA_STATO='F' ";
            }
            else
            {
                //result = result + " AND A.DOCNUMBER NOT IN (SELECT ID_PROFILE FROM DPA_VERSAMENTO) ";
                result = result + " AND V.CHA_STATO IS NULL ";
            }
            #endregion

            #region Tipo Registro
            if (!string.IsNullOrEmpty(policy.tipoRegistroStampa))
            {
                if (policy.tipoRegistroStampa.Equals("R"))
                {
                    result = result + " AND A.CHA_TIPO_PROTO = 'R' ";
                }
                if (policy.tipoRegistroStampa.Equals("C"))
                {
                    result = result + " AND A.CHA_TIPO_PROTO = 'C' ";
                }
            }
            #endregion

            #region Repertorio
            if (!string.IsNullOrEmpty(policy.idRepertorio) && !policy.idRepertorio.Equals("0"))
            {
                result = result + " AND R.ID_REPERTORIO = " + policy.idRepertorio;
            }
            #endregion

            #region Registro/AOO
            if (!string.IsNullOrEmpty(policy.idRegistro) && !policy.idRegistro.Equals("0"))
            {
                result = result + " AND (A.ID_REGISTRO = " + policy.idRegistro + " OR A.ID_REGISTRO IS NULL) ";
            }
            #endregion

            #region RF
            if (!string.IsNullOrEmpty(policy.idRF) && !policy.idRF.Equals("0"))
            {
                result = result + " AND A.ID_REGISTRO = " + policy.idRF + " ";
            }
            #endregion

            #region Anno stampa
            if (!string.IsNullOrEmpty(policy.annoStampa))
            {
                int anno;
                if (Int32.TryParse(policy.annoStampa, out anno) && anno > 0)
                {
                    result = result + " AND R.NUM_ANNO = " + policy.annoStampa + " ";
                }
            }
            #endregion

            #region Data di stampa
            if (!string.IsNullOrEmpty(policy.filtroDataStampa) && !policy.filtroDataStampa.Equals("0"))
            {
                string anno = string.Empty;
                switch (policy.filtroDataStampa)
                {
                    // Valore singolo
                    case "S":
                        if (!string.IsNullOrEmpty(policy.dataStampaDa))
                        {
                            result = result + " AND R.DTA_STAMPA >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(policy.dataStampaDa, true);
                            result = result + " AND R.DTA_STAMPA <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(policy.dataStampaDa, false);
                        }
                        break;
                    // Intervallo
                    case "R":
                        if (!string.IsNullOrEmpty(policy.dataStampaDa))
                        {
                            result = result + " AND R.DTA_STAMPA >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(policy.dataStampaDa, true);
                        }
                        if (!string.IsNullOrEmpty(policy.dataStampaA))
                        {
                            result = result + " AND R.DTA_STAMPA <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(policy.dataStampaA, false);
                        }
                        break;
                    // Oggi
                    case "T":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            result = result + " AND DATEDIFF(DD, R.DTA_STAMPA, GETDATE()) = 0 ";
                        }
                        else
                        {
                            result = result + " AND R.DTA_STAMPA between trunc(sysdate ,'DD') and sysdate ";
                        }
                        break;
                    // Settimana corrente
                    case "W":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            result = result + "AND R.DTA_STAMPA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND R.DTA_STAMPA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE()))  ";
                        }
                        else
                        {
                            result = result + " AND R.DTA_STAMPA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND R.DTA_STAMPA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                        }
                        break;
                    // Mese corrente
                    case "M":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            result = result + "AND R.DTA_STAMPA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND R.DTA_STAMPA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                        }
                        else
                        {
                            result = result + " AND R.DTA_STAMPA>= Trunc(Sysdate,'MM')    AND R.DTA_STAMPA<(Sysdate+1 ) ";
                        }
                        break;
                    // Anno corrente
                    case "Y":
                        anno = DateTime.Now.Year.ToString();
                        result = result + " AND R.DTA_STAMPA >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("01/01/{0}", anno), true);
                        result = result + " AND R.DTA_STAMPA <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("31/12/{0}", anno), false);
                        break;
                    // Ieri
                    case "B":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            result = result + " AND DATEDIFF(DD, R.DTA_STAMPA-1, GETDATE()) = 0 ";
                        }
                        else
                        {
                            result = result + " AND R.DTA_STAMPA BETWEEN TRUNC(sysdate-1 ,'DD') and (TRUNC(sysdate, 'DD')-1/86400) ";
                        }
                        break;
                    // Settimana precedente
                    case "V":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            // da correggere
                            result = result + "AND R.DTA_STAMPA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND R.DTA_STAMPA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE()))  ";
                        }
                        else
                        {
                            result = result + " AND R.DTA_STAMPA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D'))-7)) startdayofweek from dual) AND R.DTA_STAMPA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D'))-7)) enddayofweek from dual) ";
                        }
                        break;
                    // Mese precedente
                    case "N":
                        string mese = (DateTime.Now.AddMonths(-1)).Month.ToString();
                        string giorni = "31";
                        if (mese.Equals("2"))
                            giorni = DateTime.IsLeapYear(DateTime.Now.Year) ? "29" : "28";
                        if (mese.Equals("4") || mese.Equals("6") || mese.Equals("9") || mese.Equals("11"))
                            giorni = "30";
                        anno = (DateTime.Now.AddMonths(-1)).Year.ToString();
                        result = result + " AND R.DTA_STAMPA >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("01/{0}/{1}", mese, anno), true);
                        result = result + " AND R.DTA_STAMPA <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("{0}/{1}/{2}", giorni, mese, anno), false);
                        break;
                    // Anno precedente
                    case "X":
                        anno = (DateTime.Now.Year - 1).ToString();
                        result = result + " R.DTA_STAMPA >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("01/01/{0}", anno), true);
                        result = result + " R.DTA_STAMPA <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("31/12/{0}", anno), false);
                        break;
                    // Numero giorni prima
                    case "P":
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            result = result + string.Format(" AND R.DTA_STAMPA BETWEEN CONVERT(DATETIME, DATEDIFF(day, {0}, GETDATE())) AND CONVERT(DATETIME, DATEDIFF(day, {0}-1, GETDATE()))", policy.numGiorniStampa);
                        }
                        else
                        {
                            result = result + string.Format(" AND R.DTA_STAMPA BETWEEN TRUNC(SYSDATE-{0},'DD') AND (TRUNC(SYSDATE-{0}+1, 'DD')-1/86400) ", policy.numGiorniStampa);
                        }
                        break;
                }
            }
            #endregion

            logger.Debug("END");
            return result;
        }

        public string GetDataProssimaEsecuzione(DocsPaVO.Conservazione.PARER.PolicyPARER policy)
        {
            string result = string.Empty;

            switch (policy.periodicita)
            {
                case "D":
                    result = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
                    break;
                case "W":
                    #region individuo il giorno della settimana
                    DayOfWeek w = DateTime.Today.DayOfWeek;
                    switch (policy.giornoEsecuzione)
                    {
                        case "1":
                            w = DayOfWeek.Monday;
                            break;
                        case "2":
                            w = DayOfWeek.Tuesday;
                            break;
                        case "3":
                            w = DayOfWeek.Wednesday;
                            break;
                        case "4":
                            w = DayOfWeek.Thursday;
                            break;
                        case "5":
                            w = DayOfWeek.Friday;
                            break;
                        case "6":
                            w = DayOfWeek.Saturday;
                            break;
                        case "7":
                            w = DayOfWeek.Sunday;
                            break;
                    }
#endregion
                    int dw = ((int)w - (int)DateTime.Today.DayOfWeek + 7) % 7;
                    if (w.Equals(DateTime.Today.DayOfWeek))
                        dw = 7;
                    result = DateTime.Today.AddDays(dw).ToString("dd/MM/yyyy");
                    break;
                case "M":
                    int m = DateTime.Now.Month;
                    int y = DateTime.Now.Year;
                    int d = Convert.ToInt32(policy.giornoEsecuzione);
                    if (d > 28 && m.Equals(2))
                    {
                        d = DateTime.DaysInMonth(y, m);
                    }
                    // Il valore 31 va inteso come ultimo giorno del mese
                    if (d.Equals(31))
                    {
                        d = DateTime.DaysInMonth(y, m);
                    }
                    DateTime dm = new DateTime(y, m, d);
                    if (DateTime.Compare(dm, DateTime.Now) > 0)
                        result = dm.ToString("dd/MM/yyyy");
                    else
                        result = dm.AddMonths(1).ToString("dd/MM/yyyy");
                    break;
                case "Y":
                    int anno = DateTime.Now.Year;
                    DateTime dy = new DateTime(anno, Convert.ToInt32(policy.meseEsecuzione), Convert.ToInt32(policy.giornoEsecuzione));
                    if (DateTime.Compare(dy, DateTime.Now) > 0)
                        result = dy.ToString("dd/MM/yyyy");
                    else
                        result = dy.AddYears(1).ToString("dd/MM/yyyy");
                    break;
                case "O":
                    result = policy.dataEsecuzione;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Restituisce il numero di documenti versati nel giorno precedente che si trovano nello stato richiesto
        /// </summary>
        /// <param name="stato"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public string GetCountVersamentiGiornoPrecedente(string stato, string idAmm)
        {
            string result = string.Empty;

            try
            {
                Query q = InitQuery.getInstance().getQuery("S_COUNT_VERSAMENTI_REPORT");
                q.setParam("stato", stato);
                q.setParam("idAmm", idAmm);

                string command = q.getSQL();

                using (DBProvider dbProvider = new DBProvider())
                {
                    if (!dbProvider.ExecuteScalar(out result, command))
                        throw new Exception(dbProvider.LastExceptionMessage);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Restituisce il numero di policy attive per l'amministrazione indicata
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public string GetCountPolicyAttiveAmm(string idAmm)
        {
            string result = string.Empty;

            try
            {
                Query q = InitQuery.getInstance().getQuery("S_GET_POLICY_ATTIVE_AMM");
                q.setParam("idAmm", idAmm);

                string command = q.getSQL();

                using (DBProvider dbProvider = new DBProvider())
                {
                    if (!dbProvider.ExecuteScalar(out result, command))
                        throw new Exception(dbProvider.LastExceptionMessage);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return result;
        }

        #endregion

        #endregion
        #region LAVORAZIONE ISTANZE CONSERVAZIONE
        /// <summary>
        /// Estrazione lista documenti da versare
        /// Stati V (in attesa di versamento), E (errore nell'invio), T (timeout)
        /// </summary>
        /// <param name="registri">Se true, estrae solo il tipo registro stampe; se false, estrae tutto meno le stampe</param>
        /// <returns></returns>
        public List<InfoConservazione> GetInfoConservazioneDaStato(String statoConservazione)
        {
            List<InfoConservazione> result = new List<InfoConservazione>();
           
            try
            {
                Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_SELECT_ISTANZA_STATO_DA_STATO");
                String parameter = "cha_stato";
                query.setParam(parameter, statoConservazione);
                string commandText = query.getSQL();
                InfoConservazione infoCons = null;
                logger.Debug(commandText);


                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            infoCons = new InfoConservazione();
                            infoCons.SystemID = reader.GetValue(reader.GetOrdinal("SYSTEM_ID")).ToString();
                            infoCons.IdAmm = reader.GetValue(reader.GetOrdinal("ID_AMM")).ToString();
                            infoCons.IdPeople = reader.GetValue(reader.GetOrdinal("ID_PEOPLE")).ToString();
                            infoCons.IdRuoloInUo = reader.GetValue(reader.GetOrdinal("ID_RUOLO_IN_UO")).ToString();
                            infoCons.StatoConservazione = reader.GetValue(reader.GetOrdinal("CHA_STATO")).ToString();
                            infoCons.TipoSupporto = reader.GetValue(reader.GetOrdinal("VAR_TIPO_SUPPORTO")).ToString();
                            infoCons.Note = reader.GetValue(reader.GetOrdinal("VAR_NOTE")).ToString();
                            infoCons.Descrizione = reader.GetValue(reader.GetOrdinal("VAR_DESCRIZIONE")).ToString();
                            infoCons.Data_Apertura = reader.GetValue(reader.GetOrdinal("DATA_APERTURA")).ToString();
                            infoCons.Data_Invio = reader.GetValue(reader.GetOrdinal("DATA_INVIO")).ToString();
                            infoCons.Data_Conservazione = reader.GetValue(reader.GetOrdinal("DATA_CONSERVAZIONE")).ToString();
                            infoCons.MarcaTemporale = reader.GetValue(reader.GetOrdinal("VAR_MARCA_TEMPORALE")).ToString();
                            infoCons.FirmaResponsabile = reader.GetValue(reader.GetOrdinal("VAR_FIRMA_RESPONSABILE")).ToString();
                            infoCons.LocazioneFisica = reader.GetValue(reader.GetOrdinal("VAR_LOCAZIONE_FISICA")).ToString();
                            infoCons.Data_Prox_Verifica = reader.GetValue(reader.GetOrdinal("DATA_PROX_VERIFICA")).ToString();
                            infoCons.Data_Ultima_Verifica = reader.GetValue(reader.GetOrdinal("DATA_ULTIMA_VERIFICA")).ToString();
                            infoCons.Data_Riversamento = reader.GetValue(reader.GetOrdinal("DATA_RIVERSAMENTO")).ToString();
                            infoCons.TipoConservazione = reader.GetValue(reader.GetOrdinal("VAR_TIPO_CONS")).ToString();
                            infoCons.numCopie = reader.GetValue(reader.GetOrdinal("COPIE_SUPPORTI")).ToString();
                            infoCons.noteRifiuto = reader.GetValue(reader.GetOrdinal("VAR_NOTE_RIFIUTO")).ToString();
                            infoCons.formatoDoc = reader.GetValue(reader.GetOrdinal("VAR_FORMATO_DOC")).ToString();
                            infoCons.userID = reader.GetValue(reader.GetOrdinal("USER_ID")).ToString();
                            infoCons.IdGruppo = reader.GetValue(reader.GetOrdinal("ID_GRUPPO")).ToString();
                            infoCons.validationMask = Int32.Parse(reader.GetValue(reader.GetOrdinal("VALIDATION_MASK")).ToString());
                            infoCons.decrSupporto = this.getTipoSupporto(reader.GetValue(reader.GetOrdinal("VAR_TIPO_SUPPORTO")).ToString());
                            infoCons.esitoVerifica = reader.IsDBNull(reader.GetOrdinal("ESITO_VERIFICA")) ? 0 : Int32.Parse(reader.GetValue(reader.GetOrdinal("ESITO_VERIFICA")).ToString());
                            string policy = reader.GetValue(reader.GetOrdinal("ID_POLICY")).ToString();
                            if (!string.IsNullOrEmpty(policy))
                            {
                                infoCons.automatica = "A";
                            }
                            else
                            {
                                infoCons.automatica = "M";
                            }
                            string consolida = reader.GetValue(reader.GetOrdinal("CONSOLIDA")).ToString();
                            if (!string.IsNullOrEmpty(consolida) && consolida.Equals("1"))
                            {
                                infoCons.consolida = true;
                            }
                            else
                            {
                                infoCons.consolida = false;
                            }
                            infoCons.idPolicyValidata = reader.GetValue(reader.GetOrdinal("ID_POLICY_VALIDAZIONE")).ToString();

                            string preferita = reader.GetValue(reader.GetOrdinal("IS_PREFERRED")).ToString();
                            if ((infoCons.StatoConservazione).Equals("N") && !string.IsNullOrEmpty(preferita) && preferita.Equals("1"))
                            {
                                infoCons.predefinita = true;
                            }
                            else
                            {
                                infoCons.predefinita = false;
                            }

                            //aggiungo l'istanza di info conservazione dentro la lista
                            result.Add(infoCons);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel reperimento delle istanze di conservazione: ", ex);
                result = null;
            }

            return result;
        }
        #endregion
    }
}
