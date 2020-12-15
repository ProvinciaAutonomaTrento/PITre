using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.utente;
using DocsPaVO.CheckInOut;
using DocsPaDocumentale_DOCUMENTUM.DctmServices;
using DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.DataModel.Core.Profiles;
using Emc.Documentum.FS.DataModel.Core.Query;
using Emc.Documentum.FS.DataModel.Core.Content;
using log4net;

namespace DocsPaDocumentale_DOCUMENTUM.Documentale
{
    /// <summary>
    /// Classe per la gestione, a livello amministrativo, del checkIn/checkOut dell'ultima versione di un documento.
    /// Consente di enumerare i documenti bloccati e forzare lo sblocco di un documento.
    /// </summary>
    public class CheckInOutAdminDocumentManager : ICheckInOutAdminDocumentManager
    {
        private ILog logger = LogManager.GetLogger(typeof(CheckInOutAdminDocumentManager));
        #region Ctros, variables, constants

        /// <summary>
        /// 
        /// </summary>
        private DocsPaVO.utente.InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public CheckInOutAdminDocumentManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Reperimento dei metadati relativi a tutti i documenti bloccati nell'ambito dell'amministrazione
        /// </summary>
        /// <param name="idAmministration"></param>
        /// <returns></returns>
        public CheckOutStatus[] GetCheckOutStatusDocuments(string idAmministration)
        {
            // Reperimento oggetto query
            PassthroughQuery query = this.GetQuery(idAmministration);

            return this.GetCheckOutStatusDocuments(query);
        }

        /// <summary>
        /// Reperimento dei metadati relativi a tutti i documenti bloccati nell'ambito dell'amministrazione
        /// con la possibilità di filtrare per utente
        /// </summary>
        /// <param name="idAmministration"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public CheckOutStatus[] GetCheckOutStatusDocuments(string idAmministration, string idUser)
        {
            // Reperimento oggetto query
            PassthroughQuery query = this.GetQuery(idAmministration, idUser);

            return this.GetCheckOutStatusDocuments(query);
        }

        /// <summary>
        /// Rimozione forzata del blocco sul documento
        /// </summary>
        /// <param name="checkOutAdminStatus"></param>
        /// <returns></returns>
        public bool ForceUndoCheckOut(CheckOutStatus checkOutAdminStatus)
        {
            CheckInOutDocumentManager checkInOutMng = new CheckInOutDocumentManager(this.InfoUtente);

            return checkInOutMng.UndoCheckOut(checkOutAdminStatus);
        }

        /// <summary>
        /// Se tru, il blocco del documento può essere forzato
        /// </summary>
        /// <returns></returns>
        public bool CanForceUndoCheckOut()
        {
            return true;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        protected InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        /// <summary>
        /// Reperimento istanza servizio
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asSuperUser">
        /// Se true, si collega con le credenziali da superamministratore
        /// </param>
        /// <returns></returns>
        protected virtual T GetServiceInstance<T>(bool asSuperUser)
        {
            string dst = string.Empty;

            if (asSuperUser)
                dst = UserManager.ImpersonateSuperUser();
            else
                dst = this.InfoUtente.dst;

            return DctmServiceFactory.GetServiceInstance<T>(dst);
        }

        /// <summary>
        /// Creazione oggetto query per il reperimento di tutti i documenti bloccati
        /// </summary>
        /// <param name="idAdministration"></param>
        /// <returns></returns>
        protected virtual PassthroughQuery GetQuery(string idAdministration)
        {
            // Reperimento codice amministrazione
            string codAmm = DocsPaServices.DocsPaQueryHelper.getCodiceAmministrazione(idAdministration);

            string queryText = string.Format("SELECT DISTINCT {0} FROM {1} WHERE {2} ORDER BY r_lock_date DESC", this.GetCheckOutFieldsString(), ObjectTypes.DOCUMENTO, string.Format("i_cabinet_id = (SELECT r_object_id FROM dm_cabinet WHERE object_name = '{0}') AND r_lock_owner IS NOT NULLSTRING", codAmm));

            return this.CreatePassthroughQuery(queryText);
        }

        /// <summary>
        /// Creazione oggetto query per il reperimento di tutti i documenti bloccati da un utente particolare
        /// </summary>
        /// <param name="idAdministration"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        protected virtual PassthroughQuery GetQuery(string idAdministration, string idUser)
        {
            // Reperimento codice amministrazione
            string codAmm = DocsPaServices.DocsPaQueryHelper.getCodiceAmministrazione(idAdministration);

            // Reperimento oggetto utente
            string codiceUtente = DocsPaServices.DocsPaQueryHelper.getCodiceUtente(idUser);

            string queryText = string.Format("SELECT DISTINCT {0} FROM {1} WHERE {2} ORDER BY r_lock_date DESC",
                    this.GetCheckOutFieldsString(),
                    ObjectTypes.DOCUMENTO,
                            string.Format("i_cabinet_id = (SELECT r_object_id FROM dm_cabinet WHERE object_name = '{0}') AND lower(r_lock_owner) = lower('{1}')", codAmm, codiceUtente.ToLower()));

            return this.CreatePassthroughQuery(queryText);
        }

        /// <summary>
        /// Creazione oggetto PassthroughQuery
        /// </summary>
        /// <param name="queryText"></param>
        /// <returns></returns>
        protected virtual PassthroughQuery CreatePassthroughQuery(string queryText)
        {
            PassthroughQuery query = new PassthroughQuery();
            query.QueryString = queryText;
            query.Repositories.Add(DctmConfigurations.GetRepositoryName());
            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual CheckOutStatus[] GetCheckOutStatusDocuments(PassthroughQuery query)
        {
            List<CheckOutStatus> list = new List<CheckOutStatus>();

            try
            {
                QueryExecution queryExecution = new QueryExecution();
                queryExecution.CacheStrategyType = CacheStrategyType.DEFAULT_CACHE_STRATEGY;

                // Reperimento istanza queryservice
                IQueryService service = this.GetServiceInstance<IQueryService>(true);

                QueryResult result = service.Execute(query, queryExecution, null);

                if (result.DataPackage != null)
                {
                    foreach (DataObject dataObject in result.DataPackage.DataObjects)
                    {
                        // Creazione oggetto "CheckOutStatus"
                        list.Add(this.CreateCheckOutStatus(dataObject));
                    }
                }
            }
            catch (Exception ex)
            {
                list = null;

                logger.Debug("Errore in Documentum. GetCheckOutStatusDocuments: " + ex.ToString());
            }

            if (list != null)
                return list.ToArray();
            else
                return null;
        }

        /// <summary>
        /// Creazione oggetto CheckOutStatus a partire dai dati contenuti in un DataObject
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        protected virtual CheckOutStatus CreateCheckOutStatus(DataObject dataObject)
        {
            CheckOutStatus retValue = new CheckOutStatus();
            retValue.ID = ((ObjectId)dataObject.Identity.Value).Id;
            retValue.UserName = dataObject.Properties.Get("r_lock_owner").GetValueAsString();
            retValue.DocumentNumber = dataObject.Properties.Get(TypeDocumento.DOC_NUMBER).GetValueAsString();
            retValue.IDDocument = retValue.DocumentNumber;

            // Reperimento segnatura, solo se il documento è un protocollo
            string tipoProto = dataObject.Properties.Get(TypeDocumento.TIPO_PROTOCOLLO).GetValueAsString();
            if (tipoProto.ToUpper() != "G")
                retValue.Segnature = dataObject.Properties.Get("object_name").GetValueAsString();

            DateProperty checkOutDate = (DateProperty)dataObject.Properties.Get("r_lock_date");
            retValue.CheckOutDate = checkOutDate.Value.Value ;

            // Attributi custom che è stato necessario aggiungere in documentum
            retValue.DocumentLocation = dataObject.Properties.Get(TypeDocumento.CHECKOUT_LOCAL_FILE_PATH).GetValueAsString();
            retValue.MachineName = dataObject.Properties.Get(TypeDocumento.CHECKOUT_MACHINE_NAME).GetValueAsString();


            return retValue;
        }

        /// <summary>
        /// Reperimento lista dei campi utilizzati per il checkout di un documento
        /// </summary>
        /// <returns></returns>
        protected virtual string GetCheckOutFieldsString()
        {
            return string.Concat("r_object_id, object_name, r_lock_owner, r_lock_date, " +
                TypeDocumento.DOC_NUMBER, ", ", TypeDocumento.TIPO_PROTOCOLLO, ", ", TypeDocumento.CHECKOUT_LOCAL_FILE_PATH, ", ", TypeDocumento.CHECKOUT_MACHINE_NAME);
        }

        #endregion
    }
}