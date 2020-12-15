using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using DocsPaVO.InstanceAccess;
using System.Data;
using log4net;
using System.IO;

namespace DocsPaDB.Query_DocsPAWS
{
    public class InstanceAccessDB : DBProvider
    {
        #region Const

        private ILog logger = LogManager.GetLogger(typeof(InstanceAccessDB));

        #endregion

        #region Select

        /// <summary>
        /// Restituisce la lista di tutte le istanze d'accesso
        /// </summary>
        /// <returns></returns>
        public List<InstanceAccess> GetInstanceAccess(string idPeople, string idGroups, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetInstanceAccess in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            List<InstanceAccess> listInstanceAccess = new List<InstanceAccess>();
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_INST_ACC");
                string condition = "id_people_proprietario=" + idPeople + " AND id_gruppo_proprietario=" + idGroups;
                q.setParam("condition", condition);
                query = q.getSQL();
                if (this.ExecuteQuery(out ds, "instanceAccess", query))
                {
                    if (ds.Tables["instanceAccess"] != null && ds.Tables["instanceAccess"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["instanceAccess"].Rows)
                        {
                            listInstanceAccess.Add(BuildInstanceAccessObject(row));
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo GetInstanceAccess", exc);
                return null;
            }
            logger.Info("Fine Metodo GetInstanceAccess in DocsPaDb.Query_DocsPAWS.InstanceAccess");

            return listInstanceAccess;
        }

        /// <summary>
        /// Ritorna l'istanza di accesso avente l'id specificato
        /// </summary>
        /// <param name="idInstanceAccess"></param>
        /// <returns></returns>
        public InstanceAccess GetInstanceAccessById(string idInstanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetInstanceAccessById in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            InstanceAccess instanceAccess = null;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_INST_ACC");
                string condition = "a.system_id=" + idInstanceAccess;
                q.setParam("condition", condition);
                query = q.getSQL();
                if (this.ExecuteQuery(out ds, "instanceAccess", query))
                {
                    if (ds.Tables["instanceAccess"] != null && ds.Tables["instanceAccess"].Rows.Count > 0)
                    {
                        instanceAccess = BuildInstanceAccessObject(ds.Tables["instanceAccess"].Rows[0]);
                    }
                    if (instanceAccess != null)
                    {
                        instanceAccess.DOCUMENTS = GetInstanceAccessDocuments(idInstanceAccess, infoUtente.idAmministrazione);
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo GetInstanceAccessById", exc);
                return null;
            }
            logger.Info("Fine Metodo InsertInstanceAccessById in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return instanceAccess;
        }

        /// <summary>
        /// Ritorna la lista dei documenti appartenenti all'istanza di accesso avente l'id specificato
        /// </summary>
        /// <param name="idInstanceAccess"></param>
        /// <returns></returns>
        private List<InstanceAccessDocument> GetInstanceAccessDocuments(string idInstanceAccess, string idAmm)
        {
            logger.Info("Inizio Metodo GetInstanceAccessDocuments in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            List<InstanceAccessDocument> listInstanceAccessDocuments = new List<InstanceAccessDocument>();
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_INST_ACC_DOC");
                q.setParam("idAmm", idAmm);
                q.setParam("idInstanceAccess", idInstanceAccess);
                query = q.getSQL();
                if (this.ExecuteQuery(out ds, "instanceAccessDocuments", query))
                {
                    listInstanceAccessDocuments = BuildInstanceAccessDocumentsObject(ds);
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo GetInstanceAccessDocuments", exc);
                return null;
            }
            logger.Info("Fine Metodo GetInstanceAccessDocuments in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return listInstanceAccessDocuments;
        }

        /// <summary>
        /// Ritorna la lista degli allegati del documento avente l'id specificato
        /// </summary>
        /// <param name="idInstanceAccessDocument"></param>
        /// <returns></returns>
        private List<InstanceAccessAttachments> GetInstanceAccessAttachments(string idInstanceAccessDocument)
        {
            logger.Info("Inizio Metodo GetInstanceAccessAttachments in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            List<InstanceAccessAttachments> listInstanceAccessAttachments = new List<InstanceAccessAttachments>();
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_INST_ACC_ATT");
                q.setParam("idInstanceAccessDocument", idInstanceAccessDocument);
                query = q.getSQL();
                if (this.ExecuteQuery(out ds, "instanceAccessAttachments", query))
                {
                    listInstanceAccessAttachments = BuildInstanceAccessAttachmentsObject(ds);
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo GetInstanceAccessAttachments", exc);
                return null;
            }
            logger.Info("Fine Metodo GetInstanceAccessAttachments in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return listInstanceAccessAttachments;
        }

        /// <summary>
        /// Restituisce lo stato del download dell'istanza:
        /// -- 0 non è in scarico
        /// -- 1 è in corso lo scarico
        /// -- 2 lo scarico è terminato
        /// </summary>
        /// <param name="idInstanceAccess"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public string GetStateDownloadInstanceAccess(string idInstanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetStateDownloadInstanceAccesss in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            string stateDownload = string.Empty; ;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_INST_ACC_STATE_DOWNLOAD");
                q.setParam("idInstanceAccess", idInstanceAccess);
                query = q.getSQL();
                if (this.ExecuteQuery(out ds, "idInstanceAccess", query))
                {
                    if (ds.Tables["idInstanceAccess"] != null && ds.Tables["idInstanceAccess"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["idInstanceAccess"].Rows)
                        {
                            stateDownload = !string.IsNullOrEmpty(row["STATE_DOWNLOAD_FORWARD"].ToString()) ? row["STATE_DOWNLOAD_FORWARD"].ToString() : string.Empty;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo GetStateDownloadInstanceAccess", exc);
                return stateDownload;
            }
            logger.Info("Fine Metodo GetStateDownloadInstanceAccess in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return stateDownload;
        }


        public string GetIdProfileDownload(string idInstanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetIdProfileDownload in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            string stateDownload = string.Empty;

            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_INST_ACC_ID_PROFILE_DOWNLOAD");
                q.setParam("idInstanceAccess", idInstanceAccess);
                query = q.getSQL();
                this.ExecuteScalar(out stateDownload, query);
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo GetIdProfileDownload", exc);
                return stateDownload;
            }
            logger.Info("Fine Metodo GetIdProfileDownload in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return stateDownload;
        }

        public DocsPaVO.ProfilazioneDinamica.Templates GetTemplate(DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetTemplate in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            DocsPaVO.ProfilazioneDinamica.Templates template = null;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_TEMPLATE_INSTANCE");
                q.setParam("idAmm", infoUtente.idAmministrazione);
                query = q.getSQL();
                if (this.ExecuteQuery(out ds, "template", query))
                {
                    template = BuildTemplateObject(ds);
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo GetTemplate", exc);
                return null;
            }
            logger.Info("Fine Metodo GetTemplate in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return template;
        }

        #endregion

        #region Insert

        /// <summary>
        /// Inserisce una nuova istanza d'accesso
        /// </summary>
        /// <param name="instanceAccess"></param>
        /// <returns></returns>
        public InstanceAccess InsertInstanceAccess(InstanceAccess instanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo InsertInstanceAccess in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            string idInstance = string.Empty;
            try
            {
                if (instanceAccess != null)
                {
                    if (instanceAccess.RICHIEDENTE != null && instanceAccess.RICHIEDENTE.tipoCorrispondente.Equals("O") && string.IsNullOrEmpty(instanceAccess.RICHIEDENTE.systemId))
                    {
                        string prefix = System.Configuration.ConfigurationManager.AppSettings["prefissoCorrOccasionale"];
                        ArrayList sp_params = new ArrayList();
                        DocsPaUtils.Data.ParameterSP res;

                        res = new DocsPaUtils.Data.ParameterSP("RESULT", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);

                        // ID_REG
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("ID_REG", 0));
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("IDAMM", Int32.Parse(infoUtente.idAmministrazione)));
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("Prefix_cod_rub", prefix));
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("DESC_CORR", instanceAccess.RICHIEDENTE.descrizione));
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("CHA_DETTAGLI", "0"));
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("ID_CORR_GLOBALI", 0));
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("EMAIL", DBNull.Value));
                        sp_params.Add(res);
                        //this.BeginTransaction();
                        // modifica per l'email dell corrispondente occasionale
                        int resultStore = this.ExecuteStoredProcedure("INS_OCC_2", sp_params, null);

                        if (res.Valore != null && res.Valore.ToString() != "" && resultStore != -1 && resultStore != 0)
                        {
                            instanceAccess.RICHIEDENTE.systemId = res.Valore.ToString();
                            Utenti ut = new Utenti();
                            DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();
                            DocsPaUtils.Data.TypedDataSetManager.MakeTyped(instanceAccess.RICHIEDENTE.info, dettagliCorrispondente.Corrispondente.DataSet);
                            if (!ut.CheckExistDettagliCorr(instanceAccess.RICHIEDENTE.systemId))
                                ut.InsertDettagli(instanceAccess.RICHIEDENTE.systemId, dettagliCorrispondente);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    BeginTransaction();
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_INST_ACC");
                    System.Text.StringBuilder strBuilder = new StringBuilder();
                    if(dbType.ToUpper().Equals("ORACLE"))
                        strBuilder.Append(DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("INST_ACC"));
                    strBuilder.Append("'" + instanceAccess.DESCRIPTION.Replace("'", "''") + "',");
                    //strBuilder.Append("to_date('" + instanceAccess.CREATION_DATE.ToShortDateString() + " " + instanceAccess.CREATION_DATE.ToLongTimeString() + "','DD/MM/YYYY  HH24:MI:SS'),");
                    strBuilder.Append(DocsPaDbManagement.Functions.Functions.ToDate(instanceAccess.CREATION_DATE.ToString()) + ",");
                    strBuilder.Append("null ,");
                    strBuilder.Append(instanceAccess.ID_PEOPLE_OWNER + ",");
                    strBuilder.Append(instanceAccess.ID_GROUPS_OWNER + ",");

                    if (instanceAccess.RICHIEDENTE == null)
                        strBuilder.Append("null ,");
                    else
                        strBuilder.Append(instanceAccess.RICHIEDENTE.systemId + ",");

                    if(instanceAccess.REQUEST_DATE.Equals(DateTime.MinValue))
                        strBuilder.Append("null ,");
                    else
                        strBuilder.Append(DocsPaDbManagement.Functions.Functions.ToDate(instanceAccess.REQUEST_DATE.ToString()) + ",");
                        //strBuilder.Append("to_date('" + instanceAccess.REQUEST_DATE.ToShortDateString() + " " + instanceAccess.REQUEST_DATE.ToLongTimeString() + "','DD/MM/YYYY  HH24:MI:SS'),");
                    
                    if (string.IsNullOrEmpty(instanceAccess.ID_DOCUMENT_REQUEST))
                        strBuilder.Append("null ,");
                    else
                        strBuilder.Append(instanceAccess.ID_DOCUMENT_REQUEST + ",");

                    strBuilder.Append("'" + instanceAccess.NOTE.Replace("'", "''") + "',");
                    strBuilder.Append("'0', ");
                    strBuilder.Append("0");
                    q.setParam("value", strBuilder.ToString());
                    string query = q.getSQL();
                    if (ExecuteNonQuery(query))
                    {
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("INST_ACC");
                        this.ExecuteScalar(out idInstance, sql);
                        if (!string.IsNullOrEmpty(idInstance))
                            instanceAccess.ID_INSTANCE_ACCESS = idInstance;
                        else
                            instanceAccess = null;
                    }
                    else
                    {
                        instanceAccess = null;
                    }    
                }
            }
            catch (Exception exc)
            {
                RollbackTransaction();
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo InsertInstanceAccess", exc);
            }
            CommitTransaction();
            logger.Info("Fine Metodo InsertInstanceAccess in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return instanceAccess;
        }

        /// <summary>
        /// Inserisce la lista di documenti relativi ad un'istanza d'accesso
        /// </summary>
        /// <param name="listInstanceAccessDocument"></param>
        /// <returns></returns>
        public bool InsertInstanceAccessDocuments(List<InstanceAccessDocument> listInstanceAccessDocuments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo InsertInstanceAccessDocuments in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            bool result = true;
            string idInstanceAccessDoc = string.Empty;
            try 
            {
                if (listInstanceAccessDocuments != null && listInstanceAccessDocuments.Count > 0)
                { 
                    BeginTransaction();
                    foreach (InstanceAccessDocument instanceAccessDocument in listInstanceAccessDocuments)
                    {
                        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_INST_ACC_DOC");
                        System.Text.StringBuilder strBuilder = new StringBuilder();
                        strBuilder.Append(DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("INST_ACC_DOC"));
                        strBuilder.Append(instanceAccessDocument.ID_INSTANCE_ACCESS + ",");
                        strBuilder.Append(instanceAccessDocument.INFO_DOCUMENT.DOCNUMBER + ",");
                        strBuilder.Append(((instanceAccessDocument.INFO_PROJECT == null || string.IsNullOrEmpty(instanceAccessDocument.INFO_PROJECT.ID_PROJECT)) ? "null" : instanceAccessDocument.INFO_PROJECT.ID_PROJECT) + " ,");
                        strBuilder.Append(((instanceAccessDocument.INFO_PROJECT == null || string.IsNullOrEmpty(instanceAccessDocument.INFO_PROJECT.ID_PARENT)) ? "null" : instanceAccessDocument.INFO_PROJECT.ID_PARENT) + " ,");
                        strBuilder.Append((string.IsNullOrEmpty(instanceAccessDocument.TYPE_REQUEST) ? "''" : "'" + instanceAccessDocument.TYPE_REQUEST + "'") + " ,");
                        strBuilder.Append(instanceAccessDocument.ENABLE ? '1' : '0');
                        q.setParam("value", strBuilder.ToString());
                        string query = q.getSQL();
                        if (!this.ExecuteNonQuery(query))
                        {
                            result = false;
                            break;
                        }
                        else
                        {
                            string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("INST_ACC_DOC");
                            this.ExecuteScalar(out idInstanceAccessDoc, sql);
                            if (!string.IsNullOrEmpty(idInstanceAccessDoc) && instanceAccessDocument.ATTACHMENTS != null)
                            {
                                foreach (InstanceAccessAttachments att in instanceAccessDocument.ATTACHMENTS)
                                {
                                    att.ID_INSTANCE_ACCESS_DOCUMENT = idInstanceAccessDoc;
                                }
                                InsertInstanceAccessAttachments(instanceAccessDocument.ATTACHMENTS, infoUtente);   
                            }
                        }
                    }
                }
                if (result)
                {
                    CommitTransaction();
                }
                else
                {
                    RollbackTransaction();
                }
            }
            catch (Exception exc)
            {
                RollbackTransaction();
                result = false;
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo InsertInstanceAccessDocuments", exc);
            }
            logger.Info("Fine Metodo InsertInstanceAccessDocuments in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return result;
        }

        /// <summary>
        /// Inserisce la lista degli allegati di un documento appartenente ad un'istanza d'accesso
        /// </summary>
        /// <param name="listInstanceAccessAttachments"></param>
        /// <returns></returns>
        public bool InsertInstanceAccessAttachments(List<InstanceAccessAttachments> listInstanceAccessAttachments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo InsertInstanceAccessAttachments in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            bool result = true;
            try
            {
                if (listInstanceAccessAttachments != null && listInstanceAccessAttachments.Count > 0)
                {
                    BeginTransaction();
                    foreach (InstanceAccessAttachments instanceAccessAttachments in listInstanceAccessAttachments)
                    {
                        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_INST_ACC_ATT");
                        System.Text.StringBuilder strBuilder = new StringBuilder();
                        strBuilder.Append(DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("INST_ACC_ATT"));
                        strBuilder.Append(instanceAccessAttachments.ID_INSTANCE_ACCESS_DOCUMENT + ",");
                        strBuilder.Append((instanceAccessAttachments.ID_ATTACH) + " ,");
                        strBuilder.Append(instanceAccessAttachments.ENABLE ? '1' : '0');
                        q.setParam("value", strBuilder.ToString());
                        string query = q.getSQL();
                        if (!this.ExecuteNonQuery(query))
                        {
                            result = false;
                            break;
                        }
                    }
                }
                if (result)
                {
                    CommitTransaction();
                }
                else
                {
                    RollbackTransaction();
                }
            }
            catch (Exception exc)
            {
                RollbackTransaction();
                result = false;
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo InsertInstanceAccessAttachments", exc);
            }
            logger.Info("Fine Metodo InsertInstanceAccessAttachments in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return result;
        }

        #endregion

        #region Update

        /// <summary>
        /// Aggiorna l'istanza d'accesso
        /// </summary>
        /// <param name="instanceAccess"></param>
        /// <returns></returns>
        public InstanceAccess UpdateInstanceAccess(InstanceAccess instanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo UpdateInstanceAccess in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            bool result = false;
            string query;
            int rowAffected;
            try
            {
                if (instanceAccess != null)
                {
                    if (instanceAccess.RICHIEDENTE != null && instanceAccess.RICHIEDENTE.tipoCorrispondente.Equals("O") && string.IsNullOrEmpty(instanceAccess.RICHIEDENTE.systemId))
                    {
                        string prefix = System.Configuration.ConfigurationManager.AppSettings["prefissoCorrOccasionale"];
                        ArrayList sp_params = new ArrayList();
                        DocsPaUtils.Data.ParameterSP res;

                        res = new DocsPaUtils.Data.ParameterSP("RESULT", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);

                        // ID_REG
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("ID_REG", 0));
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("IDAMM", Int32.Parse(infoUtente.idAmministrazione)));
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("Prefix_cod_rub", prefix));
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("DESC_CORR", instanceAccess.RICHIEDENTE.descrizione));
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("CHA_DETTAGLI", "0"));
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("ID_CORR_GLOBALI", 0));
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("EMAIL", DBNull.Value));
                        sp_params.Add(res);
                        //this.BeginTransaction();
                        // modifica per l'email dell corrispondente occasionale
                        int resultStore = this.ExecuteStoredProcedure("INS_OCC_2", sp_params, null);

                        if (res.Valore != null && res.Valore.ToString() != "" && resultStore != -1 && resultStore != 0)
                        {
                            instanceAccess.RICHIEDENTE.systemId = res.Valore.ToString();
                            Utenti ut = new Utenti();
                            DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();
                            DocsPaUtils.Data.TypedDataSetManager.MakeTyped(instanceAccess.RICHIEDENTE.info, dettagliCorrispondente.Corrispondente.DataSet);
                            if (!ut.CheckExistDettagliCorr(instanceAccess.RICHIEDENTE.systemId))
                                ut.InsertDettagli(instanceAccess.RICHIEDENTE.systemId, dettagliCorrispondente);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    BeginTransaction();
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_INST_ACC");
                    q.setParam("idInstanceAccess", instanceAccess.ID_INSTANCE_ACCESS);
                    q.setParam("description", instanceAccess.DESCRIPTION.Replace("'", "''"));
                    q.setParam("idPeopleRichiedente", instanceAccess.RICHIEDENTE == null ? "null" : instanceAccess.RICHIEDENTE.systemId);
                    q.setParam("requestDate", instanceAccess.REQUEST_DATE.Equals(DateTime.MinValue) ? "null" : DocsPaDbManagement.Functions.Functions.ToDate(instanceAccess.REQUEST_DATE.ToString()));
                    q.setParam("idDocumentRequest", string.IsNullOrEmpty(instanceAccess.ID_DOCUMENT_REQUEST) ? "null" : instanceAccess.ID_DOCUMENT_REQUEST);
                    q.setParam("note", instanceAccess.NOTE.Replace("'", "''"));
                    q.setParam("stateDownloadForward", instanceAccess.STATE_DOWNLOAD_FORWARD.ToString());
                    query = q.getSQL();
                    this.ExecuteNonQuery(query, out  rowAffected);
                    if (rowAffected > 0)
                    {
                        result = true;
                    }
                }
                if (result)
                {
                    CommitTransaction();
                }
                else
                {
                    RollbackTransaction();
                    instanceAccess = null;
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo UpdateInstanceAccess", exc);
                RollbackTransaction();
                instanceAccess = null;
            }
            logger.Info("Fine Metodo UpdateInstanceAccess in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return instanceAccess;
        }

        /// <summary>
        /// Aggiorna l'istanza d'accesso
        /// </summary>
        /// <param name="instanceAccess"></param>
        /// <returns></returns>
        public bool UpdateInstanceAccessDocuments(List<InstanceAccessDocument> listInstanceAccessDocument, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo UpdateInstanceAccessDocuments in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            bool result = true;
            string query;
            int rowAffected;
            try
            {
                if (listInstanceAccessDocument != null && listInstanceAccessDocument.Count > 0)
                {
                    DocsPaUtils.Query q;
                    foreach(InstanceAccessDocument doc in listInstanceAccessDocument)
                    {
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("U_INST_ACC_DOC");
                        q.setParam("idInstanceAccessDocument", doc.ID_INSTANCE_ACCESS_DOCUMENT);
                        q.setParam("typeRequest", doc.TYPE_REQUEST);
                        query = q.getSQL();
                        this.ExecuteNonQuery(query, out  rowAffected);
                        if (rowAffected == 0)
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo UpdateInstanceAccess", exc);
                return false;
            }
            logger.Info("Fine Metodo UpdateInstanceAccess in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return result;
        }

        /// <summary>
        /// Imposta la data di chiusura per l'istanza di accesso
        /// </summary>
        /// <param name="idInstanceAccess"></param>
        /// <returns></returns>
        private bool UpdateCloseDateInstanceAccess(string idInstanceAccess)
        { 
            logger.Info("Inizio Metodo UpdateRequestDateInstanceAccess in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            bool result = false;
            string query;
            int rowAffected;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_INST_ACC_REQUEST_DATE");
                q.setParam("idInstanceAccess", idInstanceAccess);
                q.setParam("closetDate", DocsPaDbManagement.Functions.Functions.GetDate());
                query = q.getSQL();
                this.ExecuteNonQuery(query, out  rowAffected);
                if (rowAffected > 0)
                {
                    result = true;
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo UpdateRequestDateInstanceAccess", exc);
            }
            logger.Info("Fine Metodo UpdateRequestDateInstanceAccess in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return result;
        }

        public bool UpdateInstanceAccessDocumentEnable(List<InstanceAccessDocument> listInstanceAccessDocument, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo UpdateInstanceAccessDocumentEnabled in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            bool result = true;
            string query;
            int rowAffected;
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    foreach (InstanceAccessDocument doc in listInstanceAccessDocument)
                    {
                        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_INST_ACC_DOC_ENABLE");
                        q.setParam("idInstanceAccessDocument", doc.ID_INSTANCE_ACCESS_DOCUMENT);
                        q.setParam("enable", doc.ENABLE ? "1" : "0");
                        query = q.getSQL();
                        this.ExecuteNonQuery(query, out  rowAffected);
                        if (rowAffected < 1)
                        {
                        //    if (!UpdateInstanceAccessAttachmentsEnable(doc.ATTACHMENTS, infoUtente))
                        //    {
                        //        result = false;
                        //        break;
                        //    }                            
                        //}
                        //else
                        //{
                        //    result = false;
                            break;
                        }
                    }
                }
               
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo UpdateInstanceAccessDocumentEnable", exc);
                return false;
            }
            logger.Info("Fine Metodo UpdateInstanceAccessDocumentEnable in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return result;
        }

        public bool UpdateInstanceAccessAttachmentsEnable(List<InstanceAccessAttachments> listInstanceAccessAttachments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo UpdateInstanceAccessDocumentEnabled in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            bool result = true;
            string query;
            int rowAffected;
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    foreach (InstanceAccessAttachments att in listInstanceAccessAttachments)
                    {
                        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_INST_ACC_ATT_ENABLE");
                        q.setParam("idInstanceAccessAttachments", att.SYSTEM_ID);
                        q.setParam("enable", att.ENABLE ? "1" : "0");
                        query = q.getSQL();
                        this.ExecuteNonQuery(query, out  rowAffected);
                        if (rowAffected < 1)
                        {
                            result = false;
                            break;
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo UpdateInstanceAccessDocumentEnable", exc);
                return false;
            }
            logger.Info("Fine Metodo UpdateInstanceAccessDocumentEnable in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return result;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Rimuove la lista di documenti legati ad un'istanza di accesso
        /// </summary>
        /// <param name="listInstanceAccessDocuments"></param>
        /// <returns></returns>
        public bool RemoveInstanceAccessDocuments(List<InstanceAccessDocument> listInstanceAccessDocuments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo InsertInstanceAccessDocuments in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            bool result = true;
            int rowAffected;
            try
            {
                if (listInstanceAccessDocuments != null && listInstanceAccessDocuments.Count > 0)
                {
                    BeginTransaction();
                    foreach (InstanceAccessDocument instanceAccessDocument in listInstanceAccessDocuments)
                    {
                        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_INST_ACC_DOC");
                        q.setParam("idInstanceAccessDocument", instanceAccessDocument.ID_INSTANCE_ACCESS_DOCUMENT);
                        string query = q.getSQL();
                        this.ExecuteNonQuery(query, out  rowAffected);
                        if (rowAffected > 0)
                        {
                            if (instanceAccessDocument.ATTACHMENTS != null && instanceAccessDocument.ATTACHMENTS.Count > 0)
                                if (!RemoveAllInstanceAccessAttachments(instanceAccessDocument.ID_INSTANCE_ACCESS_DOCUMENT))
                                {
                                    result = false;
                                    break;
                                }
                        }
                        else
                        {
                            result = false;
                            break;
                        }
                    }
                }
                if (result)
                {
                    CommitTransaction();
                }
                else
                {
                    RollbackTransaction();
                }
            }
            catch (Exception exc)
            {
                RollbackTransaction();
                result = false;
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo InsertInstanceAccessDocuments", exc);
            }
            logger.Info("Fine Metodo InsertInstanceAccessDocuments in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return result;
        }

        /// <summary>
        /// Rimuove tutti gli allegati di un documento appartenente ad un'istanza di accesso
        /// </summary>
        /// <param name="idInstanceAccessDocument"></param>
        /// <returns></returns>
        private bool RemoveAllInstanceAccessAttachments(string idInstanceAccessDocument)
        {
            logger.Info("Inizio Metodo RemoveAllInstanceAccessAttachments in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            bool result = true;
            int rowAffected;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_INST_ACC_ATT_BY_ID_INST_ACC_DOC");
                q.setParam("idInstanceAccessDocument", idInstanceAccessDocument);
                string query = q.getSQL();
                this.ExecuteNonQuery(query, out  rowAffected);
                if (rowAffected > 0)
                {
                    result = true;
                }
            }
            catch (Exception exc)
            {
                result = false;
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo RemoveAllInstanceAccessAttachments", exc);
                throw exc;
            }
            logger.Info("Fine Metodo RemoveAllInstanceAccessAttachments in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return result;
        }

        /// <summary>
        /// Rimuove la lista di allegati di un documento appartenente ad un'istanza di accesso
        /// </summary>
        /// <param name="listInstanceAccessAttachments"></param>
        /// <returns></returns>
        public bool RemoveInstanceAccessAttachments(List<InstanceAccessAttachments> listInstanceAccessAttachments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo RemoveInstanceAccessAttachments in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            bool result = true;
            int rowAffected;
            try
            {
                if (listInstanceAccessAttachments != null && listInstanceAccessAttachments.Count > 0)
                {
                    BeginTransaction();
                    foreach (InstanceAccessAttachments instanceAccessAttachments in listInstanceAccessAttachments)
                    {
                        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_INST_ACC_ATT");
                        q.setParam("idInstanceAccessDocument", instanceAccessAttachments.ID_INSTANCE_ACCESS_DOCUMENT);
                        q.setParam("idAttach", instanceAccessAttachments.ID_ATTACH);
                        string query = q.getSQL();
                        this.ExecuteNonQuery(query, out  rowAffected);
                        if (rowAffected <= 0)
                        {
                            result = false;
                            break;
                        }
                    }
                }
                if (result)
                {
                    CommitTransaction();
                }
                else
                {
                    RollbackTransaction();
                }
            }
            catch (Exception exc)
            {
                RollbackTransaction();
                result = false;
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo RemoveInstanceAccessAttachments", exc);
            }
            logger.Info("Fine Metodo RemoveInstanceAccessAttachments in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            return result;
        }

        #endregion

        #region Build object

        private InstanceAccess BuildInstanceAccessObject(DataRow row)
        {
            InstanceAccess instanceAccess = null;
            try
            {
                instanceAccess = new InstanceAccess()
                {
                    ID_INSTANCE_ACCESS = !string.IsNullOrEmpty(row["ID_INSTANCE_ACCESS"].ToString()) ? row["ID_INSTANCE_ACCESS"].ToString() : string.Empty,
                    DESCRIPTION = !string.IsNullOrEmpty(row["DESCRIPTION"].ToString()) ? row["DESCRIPTION"].ToString() : string.Empty,
                    CREATION_DATE = Convert.ToDateTime(row["CREATION_DATE"].ToString(), new System.Globalization.CultureInfo("it-IT")),
                    CLOSE_DATE = !string.IsNullOrEmpty(row["CLOSE_DATE"].ToString()) ? Convert.ToDateTime(row["CLOSE_DATE"].ToString(), new System.Globalization.CultureInfo("it-IT")) : DateTime.MinValue,
                    ID_PEOPLE_OWNER = !string.IsNullOrEmpty(row["ID_PEOPLE_OWNER"].ToString()) ? row["ID_PEOPLE_OWNER"].ToString() : string.Empty,
                    ID_GROUPS_OWNER = !string.IsNullOrEmpty(row["ID_GROUPS_OWNER"].ToString()) ? row["ID_GROUPS_OWNER"].ToString() : string.Empty,
                    RICHIEDENTE = !string.IsNullOrEmpty(row["ID_RICHIEDENTE"].ToString()) ? new DocsPaVO.utente.Corrispondente(){ systemId = row["ID_RICHIEDENTE"].ToString(),
                                                                                                                                  codiceRubrica = row["CODICE_RUBRICA"].ToString(),
                                                                                                                                  descrizione = !string.IsNullOrEmpty(row["DESCRIPTION_RICHIEDENTE"].ToString()) ? row["DESCRIPTION_RICHIEDENTE"].ToString() : string.Empty} : null,
                    REQUEST_DATE = !string.IsNullOrEmpty(row["REQUEST_DATE"].ToString()) ? Convert.ToDateTime(row["REQUEST_DATE"].ToString(), new System.Globalization.CultureInfo("it-IT")) : DateTime.MinValue,
                    ID_DOCUMENT_REQUEST = !string.IsNullOrEmpty(row["ID_DOCUMENT_REQUEST"].ToString()) ? row["ID_DOCUMENT_REQUEST"].ToString() : string.Empty,
                    NOTE = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                    STATE_DOWNLOAD_FORWARD = !string.IsNullOrEmpty(row["STATE_DOWNLOAD_FORWARD"].ToString()) ?
                        Convert.ToChar(row["STATE_DOWNLOAD_FORWARD"].ToString()) : '0'
                };
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return instanceAccess;
        }

       
        private List<InstanceAccessDocument> BuildInstanceAccessDocumentsObject(DataSet ds)
        {
            List<InstanceAccessDocument> listInstanceAccessDocuments = new List<InstanceAccessDocument>();
            try
            {
                if (ds.Tables["instanceAccessDocuments"] != null && ds.Tables["instanceAccessDocuments"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["instanceAccessDocuments"].Rows)
                    {
                        InstanceAccessDocument instanceAccessDocument = new InstanceAccessDocument()
                        {
                            ID_INSTANCE_ACCESS_DOCUMENT = !string.IsNullOrEmpty(row["ID_INSTANCE_ACCESS_DOCUMENT"].ToString()) ? row["ID_INSTANCE_ACCESS_DOCUMENT"].ToString() : string.Empty,
                            ID_INSTANCE_ACCESS = !string.IsNullOrEmpty(row["ID_INSTANCE_ACCESS"].ToString()) ? row["ID_INSTANCE_ACCESS"].ToString() : string.Empty,
                            DOCNUMBER = !string.IsNullOrEmpty(row["DOCNUMBER"].ToString()) ? row["DOCNUMBER"].ToString() : string.Empty,
                            TYPE_REQUEST = !string.IsNullOrEmpty(row["TYPE_REQUEST"].ToString()) ? row["TYPE_REQUEST"].ToString() : string.Empty,
                            INFO_DOCUMENT = BuildInfoDocument(row),
                            INFO_PROJECT = !string.IsNullOrEmpty(row["ID_PROJECT"].ToString()) ? BuildInfoProject(row) : null,
                            ENABLE = (!string.IsNullOrEmpty(row["ENABLE"].ToString()) && row["ENABLE"].ToString().Trim()=="1") ? true : false,
                            ATTACHMENTS = GetInstanceAccessAttachments(row["ID_INSTANCE_ACCESS_DOCUMENT"].ToString())
                            
                        };
                        listInstanceAccessDocuments.Add(instanceAccessDocument);
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return listInstanceAccessDocuments;
        }

        private List<InstanceAccessAttachments> BuildInstanceAccessAttachmentsObject(DataSet ds)
        {
            List<InstanceAccessAttachments> listInstanceAccessAttachments = new List<InstanceAccessAttachments>();
            try
            {
                if (ds.Tables["instanceAccessAttachments"] != null && ds.Tables["instanceAccessAttachments"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["instanceAccessAttachments"].Rows)
                    {
                        InstanceAccessAttachments instanceAccessAttachments = new InstanceAccessAttachments()
                        {
                            SYSTEM_ID = !string.IsNullOrEmpty(row["SYSTEM_ID"].ToString()) ? row["SYSTEM_ID"].ToString() : string.Empty,
                            ID_INSTANCE_ACCESS_DOCUMENT = !string.IsNullOrEmpty(row["ID_INSTANCE_ACCESS_DOCUMENT"].ToString()) ? row["ID_INSTANCE_ACCESS_DOCUMENT"].ToString() : string.Empty,
                            ID_ATTACH = !string.IsNullOrEmpty(row["ID_ATTACH"].ToString()) ? row["ID_ATTACH"].ToString() : string.Empty,
                            FILE_NAME = !string.IsNullOrEmpty(row["FILE_NAME"].ToString()) ? row["FILE_NAME"].ToString() : string.Empty,
                            EXTENSION = !string.IsNullOrEmpty(row["EXTENSION"].ToString()) ? row["EXTENSION"].ToString() : string.Empty,
                            ENABLE = !string.IsNullOrEmpty(row["ENABLE"].ToString()) && row["ENABLE"].ToString().Trim()=="1" ? true : false
                        };
                        listInstanceAccessAttachments.Add(instanceAccessAttachments);
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return listInstanceAccessAttachments;
        }

        private InfoDocument BuildInfoDocument(DataRow row)
        {
            InfoDocument infoDocument;
            infoDocument = new InfoDocument()
            {
                DOCNUMBER = !string.IsNullOrEmpty(row["DOCNUMBER"].ToString()) ? row["DOCNUMBER"].ToString() : string.Empty,
                OBJECT = !string.IsNullOrEmpty(row["OBJECT"].ToString()) ? row["OBJECT"].ToString() : string.Empty,
                HASH = !string.IsNullOrEmpty(row["HASH"].ToString()) ? row["HASH"].ToString() : string.Empty,
                FILE_NAME = !string.IsNullOrEmpty(row["FILE_NAME"].ToString()) ? row["FILE_NAME"].ToString() : string.Empty,
                TYPE_PROTO = !string.IsNullOrEmpty(row["TYPE_PROTO"].ToString()) ? row["TYPE_PROTO"].ToString() : string.Empty,
                NUMBER_PROTO = !string.IsNullOrEmpty(row["NUMBER_PROTO"].ToString()) ? row["NUMBER_PROTO"].ToString() : string.Empty,
                MITT_DEST = !string.IsNullOrEmpty(row["MITT_DEST"].ToString()) ? row["MITT_DEST"].ToString() : string.Empty,
                REGISTER = !string.IsNullOrEmpty(row["REGISTER"].ToString()) ? row["REGISTER"].ToString() : string.Empty,
                DESCRIPTION_TIPOLOGIA_ATTO = !string.IsNullOrEmpty(row["DESCRIPTION_TIPOLOGIA_ATTO"].ToString()) ? row["DESCRIPTION_TIPOLOGIA_ATTO"].ToString() : string.Empty,
                COUNTER_REPERTORY = !string.IsNullOrEmpty(row["COUNTER_REPERTORY"].ToString()) ? row["COUNTER_REPERTORY"].ToString().Substring(0, row["COUNTER_REPERTORY"].ToString().Length - 2) : string.Empty,
                ID_DOCUMENTO_PRINCIPALE = !string.IsNullOrEmpty(row["ID_DOCUMENTO_PRINCIPALE"].ToString()) ? row["ID_DOCUMENTO_PRINCIPALE"].ToString() : string.Empty,
                TYPE_ATTACH = !string.IsNullOrEmpty(row["ID_DOCUMENTO_PRINCIPALE"].ToString()) ? GetTypeAttach(row["DOCNUMBER"].ToString()) : 0,
                EXTENSION = !string.IsNullOrEmpty(row["EXTENSION"].ToString()) ? row["EXTENSION"].ToString() : string.Empty,
                IS_SIGNED = !string.IsNullOrEmpty(row["SIGNED"].ToString()) && row["SIGNED"].ToString().Equals("1") ? true : false,
                DATE_CREATION = Convert.ToDateTime(row["DATA"].ToString(), new System.Globalization.CultureInfo("it-IT"))
            };

            return infoDocument;
        }

        private InfoProject BuildInfoProject(DataRow row)
        {
            InfoProject infoDocument;
            infoDocument = new InfoProject()
            {
                ID_PROJECT = !string.IsNullOrEmpty(row["ID_PROJECT"].ToString()) ? row["ID_PROJECT"].ToString() : string.Empty,
                DESCRIPTION_PROJECT = !string.IsNullOrEmpty(row["DESCRIPTION_PROJECT"].ToString()) ? row["DESCRIPTION_PROJECT"].ToString() : string.Empty,
                ID_PARENT = !string.IsNullOrEmpty(row["ID_PARENT"].ToString()) ? row["ID_PARENT"].ToString() : string.Empty,
                ID_FASCICOLO = !string.IsNullOrEmpty(row["ID_FASCICOLO"].ToString()) ? row["ID_FASCICOLO"].ToString() : string.Empty,
                CODE_PROJECT = !string.IsNullOrEmpty(row["CODE_PROJECT"].ToString()) ? row["CODE_PROJECT"].ToString() : string.Empty,
                CODE_CLASSIFICATION = !string.IsNullOrEmpty(row["CODE_CLASSIFICATION"].ToString()) ? row["CODE_CLASSIFICATION"].ToString() : string.Empty
            };

            return infoDocument;
        }

        private DocsPaVO.ProfilazioneDinamica.Templates BuildTemplateObject(DataSet ds)
        {
            DocsPaVO.ProfilazioneDinamica.Templates template = null;
            try
            {
                if (ds.Tables["template"] != null && ds.Tables["template"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["template"].Rows)
                    {
                        template = new DocsPaVO.ProfilazioneDinamica.Templates()
                        {
                            SYSTEM_ID = Convert.ToInt32(row["SYSTEM_ID"].ToString()),
                            ID_TIPO_ATTO = row["SYSTEM_ID"].ToString(),
                            DESCRIZIONE = row["VAR_DESC_ATTO"].ToString(),
                            ABILITATO_SI_NO = row["ABILITATO_SI_NO"].ToString(),
                            IN_ESERCIZIO = row["IN_ESERCIZIO"].ToString(),
                            PATH_MODELLO_1 = row["PATH_MOD_1"].ToString(),
                            PATH_MODELLO_2 = row["PATH_MOD_2"].ToString(),
                            PATH_MODELLO_1_EXT = row["Ext_Mod_1"].ToString().Trim(),
                            PATH_MODELLO_2_EXT = row["Ext_Mod_2"].ToString().Trim(),
                            PATH_MODELLO_STAMPA_UNIONE = row["PATH_MOD_SU"].ToString(),
                            PATH_MODELLO_EXCEL = row["PATH_MOD_EXC"].ToString(),
                            PATH_XSD_ASSOCIATO = row["PATH_XSD_ASSOCIATO"].ToString(),
                            PATH_ALLEGATO_1 = row["PATH_ALL_1"].ToString(),
                            SCADENZA = row["GG_SCADENZA"].ToString(),
                            PRE_SCADENZA = row["GG_PRE_SCADENZA"].ToString(),
                            PRIVATO = !string.IsNullOrEmpty(row["CHA_PRIVATO"].ToString()) ? row["CHA_PRIVATO"].ToString() : "0",
                            ID_AMMINISTRAZIONE = row["ID_AMM"].ToString(),
                            CODICE_CLASSIFICA = row["COD_CLASS"].ToString(),
                            CODICE_MODELLO_TRASM = row["COD_MOD_TRASM"].ToString(),
                            IPER_FASC_DOC = !string.IsNullOrEmpty(row["IPERDOCUMENTO"].ToString()) && row["IPERDOCUMENTO"].ToString() == "1" ? "1" : "0"
                        };
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return template;
        }

        private int GetTypeAttach(string docnumber)
        {
            int type = 0;
            string versionId = string.Empty;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_VERSION_ID_ATTACH");
                q.setParam("docnumber", docnumber);
                string query = q.getSQL();
                ExecuteScalar(out versionId, query);
                if (!string.IsNullOrEmpty(versionId))
                {
                    DocsPaDB.Query_DocsPAWS.Documenti docDB = new Documenti();
                    // Nuovo metodo di associazione tipologia allegato
                    // Utilizzo CHA_ALLEGATI_ESTERNO in VERSIONS
                    switch (docDB.GetTipologiaAllegato(versionId))
                    {
                        case "P":
                            type = 2;
                            break;
                        case "I":
                            type = 3;
                            break;
                        case "1":
                            type = 4;
                            break;
                        default:
                            type = 1;
                            break;
                    }

                    //if (docDB.GeIsAllegatoPEC(versionId).Equals("1"))
                    //    type = 2;
                    //else if (docDB.GeIsAllegatoIS(versionId).Equals("1"))
                    //    type = 3;
                    //else if (docDB.GetIsAllegatoEsterno(versionId).Equals("1"))
                    //    type = 4;
                    //else
                    //    type = 1;
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo GetTypeAttach", exc);
                throw exc;
            }
            return type;

        }

        public void UpdateInstanceStartDownload(InstanceAccess instanceAccess, DocsPaVO.utente.InfoUtente infoUser,string type)
        {
            logger.Info("Inizio Metodo UpdatePrepareDownload in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            string query;
            int rowAffected;
            try
            {
                string rootPath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUser.idAmministrazione, "BE_INSTANCE_ACCESS_PATH");
                
                if (!string.IsNullOrEmpty(rootPath))
                {
                    using (DBProvider dbProvider = new DBProvider())
                    {
                        if (instanceAccess != null)
                        {
                            BeginTransaction();
                            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_INST_ACC_DOWNLOAD");
                            q.setParam("idInstanceAccess", instanceAccess.ID_INSTANCE_ACCESS);
                            q.setParam("downloadState", type);
                            query = q.getSQL();
                            dbProvider.ExecuteNonQuery(query, out  rowAffected);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo CreateDownloadInstanceAccess", exc);
            }
            logger.Info("Fine Metodo CreateDownloadInstanceAccess in DocsPaDb.Query_DocsPAWS.CreateDownloadInstanceAccess");
        }

        public void UpdateIdProfileDownload(string idProfile, string idInstanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo InsertIdProfileDownload in DocsPaDb.Query_DocsPAWS.InstanceAccess");
            string query;
            int rowAffected;
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_INST_ACC_ID_PROFILE_DOWNLOAD");
                    q.setParam("idInstanceAccess", idInstanceAccess);
                    q.setParam("idProfileDownload", idProfile);
                    query = q.getSQL();
                    dbProvider.ExecuteNonQuery(query, out  rowAffected);
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.InstanceAccess - Metodo InsertIdProfileDownload", exc);
            }
            logger.Info("Fine Metodo InsertIdProfileDownload in DocsPaDb.Query_DocsPAWS.InstanceAccess");
        }

        #endregion

    }
}
