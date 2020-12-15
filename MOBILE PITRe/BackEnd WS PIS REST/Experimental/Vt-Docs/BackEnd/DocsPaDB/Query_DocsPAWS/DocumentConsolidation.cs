using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Classe per la gestione delle azioni di consolidamento su database
    /// </summary>
    public class DocumentConsolidation
    {
        private ILog logger = LogManager.GetLogger(typeof(DocumentConsolidation));
        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        public DocsPaVO.utente.InfoUtente UserInfo { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public DocumentConsolidation(DocsPaVO.utente.InfoUtente userInfo)
        {
            this.UserInfo = userInfo;
        }

        /// <summary>
        /// Reperimento delle informazioni di stato consolidamento di un documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public DocsPaVO.documento.DocumentConsolidationStateInfo GetState(string idDocument)
        {
            logger.Info("BEGIN");
            try
            {
                DocsPaVO.documento.DocumentConsolidationStateInfo retValue = null;

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_DOCUMENT_CONSOLIDATION_STATE");
                    queryDef.setParam("idDocument", idDocument);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                        {
                            retValue = new DocsPaVO.documento.DocumentConsolidationStateInfo
                            {
                                State = (DocsPaVO.documento.DocumentConsolidationStateEnum) 
                                    Enum.Parse(typeof(DocsPaVO.documento.DocumentConsolidationStateEnum), 
                                                DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "CONSOLIDATION_STATE", true, "0"), 
                                                true),
                                Author = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "CONSOLIDATION_AUTHOR", true, string.Empty).ToString(),
                                Role = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "CONSOLIDATION_ROLE", true, string.Empty).ToString(),
                                Date = DocsPaUtils.Data.DataReaderHelper.GetValue<DateTime>(reader, "CONSOLIDATION_DATE", true).ToString()
                            };
                        }
                        else
                        {
                            throw new Exception(string.Format("Documento con id {0} non trovato: impossibile determinare lo stato di consolidamento", idDocument));
                        }
                    }
                }
                logger.Info("END");
                return retValue;
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nel reperimento dello stato di consolidamento del documento con id {0}", idDocument);

                logger.Error(errorMessage, ex);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        /// <summary>
        /// Modifica dello stato di consolidamento di un documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="state"></param>
        public DocsPaVO.documento.DocumentConsolidationStateInfo SetState(string idDocument, DocsPaVO.documento.DocumentConsolidationStateEnum toState)
        {
            DocsPaVO.documento.DocumentConsolidationStateInfo retValue = null;

            // Reperimento stato attuale
            DocsPaVO.documento.DocumentConsolidationStateInfo actualState = GetState(idDocument);

            if (actualState.State >= toState)
                throw new ApplicationException(string.Format("Stato di consolidamento non valido per il documento con id {0}", idDocument));

            if (actualState.State == DocsPaVO.documento.DocumentConsolidationStateEnum.None && 
                toState == DocsPaVO.documento.DocumentConsolidationStateEnum.Step2)
            {
                // Richiesta due passaggi di consolidamento in un'unica operazione
                this.InternalSetState(idDocument, DocsPaVO.documento.DocumentConsolidationStateEnum.Step1);
                retValue = this.InternalSetState(idDocument, DocsPaVO.documento.DocumentConsolidationStateEnum.Step2);
            }
            else 
                retValue = this.InternalSetState(idDocument, toState);

            return retValue;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="toState"></param>
        /// <returns></returns>
        protected virtual DocsPaVO.documento.DocumentConsolidationStateInfo InternalSetState(string idDocument, DocsPaVO.documento.DocumentConsolidationStateEnum toState)
        {
            try
            {
                DocsPaVO.documento.DocumentConsolidationStateInfo retValue = null;

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_SET_DOCUMENT_CONSOLIDATION_STATE");

                    queryDef.setParam("state", ((int)toState).ToString());
                    queryDef.setParam("author", this.UserInfo.idPeople);
                    queryDef.setParam("role", this.UserInfo.idGruppo);
                    queryDef.setParam("date", DocsPaDbManagement.Functions.Functions.GetDate());
                    queryDef.setParam("idDocument", idDocument);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    int rowsAffected;
                    if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    {
                        // Reperimento stato di consolidamento aggiornato
                        retValue = this.GetState(idDocument);

                        if (toState == DocsPaVO.documento.DocumentConsolidationStateEnum.Step2)
                        {
                            // Inserimento dei dati fondamentali del documento consolidato nella tabella dello storico
                            queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_INSERT_CONSOLIDATED_DOCUMENT");

                            queryDef.setParam("idDocument", idDocument);
                            queryDef.setParam("userdb", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                            
                            

                            commandText = queryDef.getSQL();
                            logger.Debug(commandText);

                            if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                            {
                                if (rowsAffected == 0)
                                    throw new ApplicationException("Errore nella storicizzazione dei dati del documento consolidato");
                            }
                            else
                                throw new ApplicationException("Errore nell'esecuzione della query I_INSERT_CONSOLIDATED_DOCUMENT");
                        }
                    }
                    else
                    {
                        throw new ApplicationException("Errore nell'esecuzione della query U_SET_DOCUMENT_CONSOLIDATION_STATE");
                    }

                    return retValue;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'impostazione dello stato di consolidamento del documento con id {0}", idDocument);

                logger.Debug(errorMessage, ex);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        #endregion
    }
}
