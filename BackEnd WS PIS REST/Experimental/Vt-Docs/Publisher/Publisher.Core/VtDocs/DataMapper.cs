using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Publisher.VtDocs
{
    /// <summary>
    /// Classe DataMapper per il mapping di oggetti del sistema documentale 
    /// in corrispondenti oggetti validi per la pubblicazione
    /// </summary>
    public class DataMapper : IDataMapper
    {
        /// <summary>
        /// 
        /// </summary>
        private const string DOCUMENT_OBJECT_TYPE = "DOCUMENTO";

        /// <summary>
        /// 
        /// </summary>
        private const string PROJECT_OBJECT_TYPE = "FASCICOLO";

        /// <summary>
        /// 
        /// </summary>
        private IDataMapper _documentDataMapper = null;

        /// <summary>
        /// 
        /// </summary>
        private IDataMapper _projectDataMapper = null;

        /// <summary>
        /// Mapping dell'oggetto
        /// </summary>
        /// <param name="logInfo">
        /// Dati del log nel sistema documentale
        /// </param>
        /// <param name="ev">
        /// Dati dell'evento generato
        /// </param>
        /// <returns>
        /// Oggetto dal pubblicare
        /// </returns>
        public virtual Subscriber.Proxy.PublishedObject Map(VtDocs.LogInfo logInfo, EventInfo ev)
        {
            IDataMapper realMapper = null;

            if (!string.IsNullOrEmpty(ev.DataMapperFullClass))
            {
                // DataMapper custom
                Type t = Type.GetType(ev.DataMapperFullClass, false);

                if (t == null)
                    throw new PublisherException(ErrorCodes.INVALID_MAPPER_OBJECT_TYPE, ErrorDescriptions.INVALID_MAPPER_OBJECT_TYPE);

                realMapper = (IDataMapper) Activator.CreateInstance(t);
            }
            else
            {
                // DataMapper predefiniti
                if (logInfo.ObjectType == DOCUMENT_OBJECT_TYPE)
                {
                    // Il log si riferisce ad un documento
                    if (this._documentDataMapper == null)
                        this._documentDataMapper = new DocumentDataMapper();

                    realMapper = this._documentDataMapper;
                }
                else if (logInfo.ObjectType == PROJECT_OBJECT_TYPE)
                {
                    // Il log si riferisce ad un fascicolo
                    if (this._projectDataMapper == null)
                        this._projectDataMapper = new ProjectDataMapper();

                    realMapper = this._projectDataMapper;
                }
                else
                {
                    throw new PublisherException(ErrorCodes.INVALID_OBJECT_TYPE, 
                                string.Format(ErrorDescriptions.INVALID_OBJECT_TYPE, logInfo.ObjectType));
                }
            }

            try
            {
                return realMapper.Map(logInfo, ev);
            }
            catch (Exception ex)
            {
                PublisherException pubEx = new PublisherException(ErrorCodes.MAP_OBJECT_ERROR,
                    string.Format(ErrorDescriptions.MAP_OBJECT_ERROR, logInfo.ObjectId, logInfo.ObjectType, ex.Message));

                this.SaveError(ev.IdChannel, pubEx);

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idInstance"></param>
        /// <param name="exception"></param>
        protected virtual void SaveError(int idInstance, PublisherException exception)
        {
            ErrorInfo error = new ErrorInfo
            {
                IdInstance = idInstance,
                ErrorCode = exception.ErrorCode,
                ErrorDescription = exception.Message,
                ErrorStack = exception.ToString(),
                ErrorDate = DateTime.Now
            };

            error = DataAccess.PublisherDataAdapter.SaveError(error);
        }

    }
}
