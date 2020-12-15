using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace Publisher.DataAccess
{
    /// <summary>
    /// Accesso ai dati per il canale di pubblicazione
    /// </summary>
    public sealed class PublisherDataAdapter
    {
        #region Public Members

        /// <summary>
        /// Avvio del servizio del canale di pubblicazione
        /// </summary>
        /// <param name="channel"></param>
        public static ChannelRefInfo StartService(ChannelRefInfo channel)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                {
                    DateTime startDate = DateTime.Now;
                    string machineName = ApplicationContext.GetMachineName();
                    string publisherServiceUrl = ApplicationContext.GetPublishServiceUrl();

                    ArrayList parameters = new ArrayList();

                    parameters.Add(new DocsPaUtils.Data.ParameterSP("p_Id", channel.Id, 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("p_StartDate", startDate, 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.DateTime));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("p_MachineName", machineName, 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("p_PublisherServiceUrl", publisherServiceUrl, 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));

                    int affectedRows = provider.ExecuteStoredProcedure("PUBLISHER.StartService", parameters, null);

                    if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                    {
                        channel.State = ChannelStateEnum.Stopped;

                        throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                            string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, provider.LastExceptionMessage));
                    }
                    else if (affectedRows == 1)
                    {
                        channel.State = ChannelStateEnum.Started;
                        channel.StartExecutionDate = startDate;
                        channel.EndExecutionDate = DateTime.MinValue;
                        channel.MachineName = machineName;
                        // Impostazione dell'url del servizio di pubblicazione da cui risulterà avviato il canale
                        channel.PublisherServiceUrl = publisherServiceUrl;

                        transactionContext.Complete();
                    }
                    else
                    {
                        channel.State = ChannelStateEnum.Stopped;

                        throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                            string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "StartService", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
                    }
                }
            }

            return channel;
        }

        /// <summary>
        /// Stop del servizio del canale di pubblicazione
        /// </summary>
        /// <param name="channel"></param>
        public static ChannelRefInfo StopService(ChannelRefInfo channel)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                {
                    DateTime endDate = DateTime.Now;

                    ArrayList parameters = new ArrayList();

                    parameters.Add(new DocsPaUtils.Data.ParameterSP("p_Id", channel.Id, 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("p_EndDate", endDate, 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.DateTime));

                    int affectedRows = provider.ExecuteStoredProcedure("PUBLISHER.StopService", parameters, null);

                    if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                    {
                        throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                            string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, provider.LastExceptionMessage));
                    }
                    else if (affectedRows == 1)
                    {
                        channel.State = ChannelStateEnum.Stopped;
                        channel.StartExecutionDate = DateTime.MinValue;
                        channel.EndExecutionDate = endDate;
                        channel.MachineName = string.Empty;
                        channel.PublisherServiceUrl = string.Empty;
                    }
                    else
                    {
                        throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                            string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "StopService", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
                    }

                    transactionContext.Complete();
                }
            }

            return channel;
        }

        /// <summary>
        /// Inserimento dati dell'errore verificatosi
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static ErrorInfo SaveError(ErrorInfo error)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                {
                    ArrayList parameters = new ArrayList();

                    parameters.Add(new DocsPaUtils.Data.ParameterSP("p_InstanceId", error.IdInstance, 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("p_ErrorCode", error.ErrorCode, 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("p_ErrorDescription", error.ErrorDescription, 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("p_ErrorStack", error.ErrorStack, 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("p_ErrorDate", error.ErrorDate, 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.DateTime));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("p_Id", 0, 4, DocsPaUtils.Data.DirectionParameter.ParamOutput, DbType.Int32));

                    int affectedRows = provider.ExecuteStoredProcedure("PUBLISHER.InsertInstanceError", parameters, null);

                    if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                    {
                        throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                            string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, provider.LastExceptionMessage));
                    }
                    else if (affectedRows == 1)
                    {
                        DocsPaUtils.Data.ParameterSP[] pTyped = (DocsPaUtils.Data.ParameterSP[])parameters.ToArray(typeof(DocsPaUtils.Data.ParameterSP));
                        object outParam = pTyped.Where(e => e.Nome == "p_Id").First().Valore;
                        if (outParam != null)
                            error.Id = Convert.ToInt32(outParam.ToString());
                    }
                    else
                    {
                        throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                            string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "InsertInstanceError", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
                    }

                    transactionContext.Complete();
                }
            }

            return error;
        }

        /// <summary>
        /// Reperimento degli errori nell'istanza di pubblicazione
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static ErrorInfo[] GetErrors(ChannelRefInfo instance)
        {
            List<ErrorInfo> errors = new List<ErrorInfo>();

            try
            {
                using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                {
                    ArrayList parameters = new ArrayList();

                    parameters.Add(new DocsPaUtils.Data.ParameterSP("p_InstanceID", instance.Id, 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));

                    DataSet ds = new DataSet();

                    provider.ExecuteStoredProcedure("PUBLISHER.GetInstanceErrors", parameters, ds);

                    if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                    {
                        throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                            string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, provider.LastExceptionMessage));
                    }

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            errors.Add(CreateErrorInfo(row));
                        }
                    }
                }

            }
            catch (PublisherException pubEx)
            {
                throw pubEx;
            }
            catch (Exception ex)
            {
                throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message));
            }

            return errors.ToArray();
        }

        /// <summary>
        /// Rimozione di un'istanza di pubblicazione
        /// </summary>
        /// <param name="channelRef"></param>
        public static ChannelRefInfo RemoveChannel(ChannelRefInfo channelRef)
        {
            if (PublisherServiceControl.GetChannelState(channelRef) == ChannelStateEnum.Started)
            {
                throw new PublisherException(ErrorCodes.OPERATION_NOT_ALLOWED_PER_SERVICE_STARTED, ErrorDescriptions.OPERATION_NOT_ALLOWED_PER_SERVICE_STARTED);
            }
            else
            {
                try
                {
                    using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                    {
                        using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                        {
                            ArrayList parameters = new ArrayList();

                            parameters.Add(new DocsPaUtils.Data.ParameterSP("p_Id", channelRef.Id, 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));

                            int affectedRows = provider.ExecuteStoredProcedure("PUBLISHER.RemovePublishInstanceEvents", parameters, null);

                            if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                            {
                                throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                    string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, provider.LastExceptionMessage));
                            }
                            else if (affectedRows == 0)
                            {
                                throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                    string.Format(ErrorDescriptions.SP_EXECUTION_ERROR,
                                    "RemovePublishInstanceEvents", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
                            }

                            channelRef.Id = 0;
                            foreach (EventInfo ev in channelRef.Events)
                                ev.Id = 0;

                            transactionContext.Complete();
                        }
                    }
                }
                catch (PublisherException pubEx)
                {
                    throw pubEx;
                }
                catch (Exception ex)
                {
                    throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message));
                }
            }

            return channelRef;
        }

        /// <summary>
        /// Aggiornamento dati istanza di pubblicazione
        /// </summary>
        /// <param name="channelRef"></param>
        /// <returns></returns>
        public static ChannelRefInfo SaveChannel(ChannelRefInfo channelRef)
        {
            if (PublisherServiceControl.GetChannelState(channelRef) == ChannelStateEnum.Started)
            {
                throw new PublisherException(ErrorCodes.OPERATION_NOT_ALLOWED_PER_SERVICE_STARTED, ErrorDescriptions.OPERATION_NOT_ALLOWED_PER_SERVICE_STARTED);
            }
            else
            {
                try
                {
                    bool insertMode = (channelRef.Id == 0);

                    using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                    {
                        using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                        {
                            if (channelRef.StartLogDate == DateTime.MinValue || channelRef.StartLogDate == DateTime.MaxValue)
                            {
                                // Se non è stata impostata la data di avvio del log,
                                // viene impostata la data attuale
                                channelRef.StartLogDate = DateTime.Now;
                            }

                            if (insertMode)
                            {
                                ArrayList parameters = new ArrayList();

                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_Name", GetObjectParamValue(channelRef.ChannelName), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_IdAdmin", GetObjectParamValue(channelRef.Admin.Id), 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_SubscriberServiceUrl", GetObjectParamValue(channelRef.SubscriberServiceUrl), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_ExecutionType", GetObjectParamValue(channelRef.ExecutionConfiguration.IntervalType.ToString()), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_ExecutionTicks", GetObjectParamValue(channelRef.ExecutionConfiguration.ExecutionTicks), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_LastExecutionDate", GetObjectParamValue(channelRef.LastExecutionDate), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.DateTime));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_ExecutionCount", GetObjectParamValue(channelRef.ExecutionCount), 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_PublishedObjects", GetObjectParamValue(channelRef.PublishedObjects), 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_TotalExecutionCount", GetObjectParamValue(channelRef.TotalExecutionCount), 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_TotalPublishedObjects", GetObjectParamValue(channelRef.TotalPublishedObjects), 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_StartLogDate", GetObjectParamValue(channelRef.StartLogDate), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.DateTime));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_Id", 0, 4, DocsPaUtils.Data.DirectionParameter.ParamOutput, DbType.Int32));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_LastLogId", 0, 4, DocsPaUtils.Data.DirectionParameter.ParamOutput, DbType.Int32));

                                int affectedRows = provider.ExecuteStoredProcedure("PUBLISHER.InsertPublishInstance", parameters, null);

                                if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                                {
                                    throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                        string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, provider.LastExceptionMessage));
                                }
                                else if (affectedRows == 1)
                                {
                                    DocsPaUtils.Data.ParameterSP[] pTyped = (DocsPaUtils.Data.ParameterSP[])parameters.ToArray(typeof(DocsPaUtils.Data.ParameterSP));
                                    object outParam = pTyped.Where(e => e.Nome == "p_Id").First().Valore;
                                    if (outParam != null)
                                        channelRef.Id = Convert.ToInt32(outParam.ToString());

                                    outParam = pTyped.Where(e => e.Nome == "p_LastLogId").First().Valore;
                                    if (outParam != null)
                                        channelRef.LastLogId = Convert.ToInt32(outParam.ToString());

                                    if (channelRef.UpdateEventsAction)
                                    {
                                        // Aggiornamento eventi
                                        SaveEvents(channelRef);
                                    }
                                }
                                else
                                {
                                    throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                        string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "InsertPublishInstance", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
                                }
                            }
                            else
                            {
                                ArrayList parameters = new ArrayList();

                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_Id", GetObjectParamValue(channelRef.Id), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_Name", GetObjectParamValue(channelRef.ChannelName), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_SubscriberServiceUrl", GetObjectParamValue(channelRef.SubscriberServiceUrl), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_ExecutionType", GetObjectParamValue(channelRef.ExecutionConfiguration.IntervalType.ToString()), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_ExecutionTicks", GetObjectParamValue(channelRef.ExecutionConfiguration.ExecutionTicks), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_StartLogDate", GetObjectParamValue(channelRef.StartLogDate), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.DateTime));
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("p_LastLogId", 0, 4, DocsPaUtils.Data.DirectionParameter.ParamOutput, DbType.Int32));

                                int affectedRows = provider.ExecuteStoredProcedure("PUBLISHER.UpdatePublishInstance", parameters, null);

                                if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                                {
                                    throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                        string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, provider.LastExceptionMessage));
                                }
                                else if (affectedRows == 0)
                                {
                                    throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                        string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "UpdatePublishInstance", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
                                }
                                else
                                {
                                    DocsPaUtils.Data.ParameterSP[] pTyped = (DocsPaUtils.Data.ParameterSP[])parameters.ToArray(typeof(DocsPaUtils.Data.ParameterSP));
                                    object outParam = pTyped.Where(e => e.Nome == "p_LastLogId").First().Valore;
                                    if (outParam != null)
                                        channelRef.LastLogId = Convert.ToInt32(outParam.ToString());
                                }

                                if (channelRef.UpdateEventsAction)
                                {
                                    // Aggiornamento eventi
                                    SaveEvents(channelRef);
                                }
                            }

                            transactionContext.Complete();
                        }
                    }
                }
                catch (PublisherException pubEx)
                {
                    throw pubEx;
                }
                catch (Exception ex)
                {
                    throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message));
                }
            }

            return channelRef;
        }

        /// <summary>
        /// Aggiornamento di un evento di un'istanza di pubblicazione
        /// </summary>
        /// <param name="info"></param>
        /// <remarks></remarks>
        public static EventInfo SaveEvent(EventInfo info)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                if (PublisherServiceControl.GetChannelState(GetChannel(info.IdChannel)) == ChannelStateEnum.Started)
                    throw new PublisherException(ErrorCodes.OPERATION_NOT_ALLOWED_PER_SERVICE_STARTED, ErrorDescriptions.OPERATION_NOT_ALLOWED_PER_SERVICE_STARTED);

                using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                {
                    if (info.Id == 0)
                    {
                        // Inserimento evento
                        ArrayList parameters = new ArrayList();

                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_InstanceId", GetObjectParamValue(info.IdChannel), 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_EventName", GetObjectParamValue(info.EventName), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_ObjectType", GetObjectParamValue(info.ObjectType), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_ObjectTemplateName", GetObjectParamValue(info.ObjectTemplateName), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_DataMapperFullClass", GetObjectParamValue(info.DataMapperFullClass), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_LoadFileIfDocType", (info.LoadFileIfDocumentType ? "1" : "0"), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_Id", 0, 4, DocsPaUtils.Data.DirectionParameter.ParamOutput, DbType.Int32));

                        int affectedRows = provider.ExecuteStoredProcedure("PUBLISHER.InsertInstanceEvent", parameters, null);

                        if (affectedRows == 0)
                        {
                            throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "InsertInstanceEvent", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
                        }
                        else if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                        {
                            throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "InsertInstanceEvent", provider.LastExceptionMessage));
                        }
                        else
                        {
                            DocsPaUtils.Data.ParameterSP[] pTyped = (DocsPaUtils.Data.ParameterSP[])parameters.ToArray(typeof(DocsPaUtils.Data.ParameterSP));
                            object outParam = pTyped.Where(e => e.Nome == "p_Id").First().Valore;
                            if (outParam != null)
                                info.Id = Convert.ToInt32(outParam.ToString());
                        }
                    }
                    else
                    {
                        ArrayList parameters = new ArrayList();

                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_Id", GetObjectParamValue(info.Id), 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_EventName", GetObjectParamValue(info.EventName), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_ObjectType", GetObjectParamValue(info.ObjectType), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_ObjectTemplateName", GetObjectParamValue(info.ObjectTemplateName), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_DataMapperFullClass", GetObjectParamValue(info.DataMapperFullClass), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_LoadFileIfDocType", (info.LoadFileIfDocumentType ? "1" : "0"), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));

                        int affectedRows = provider.ExecuteStoredProcedure("PUBLISHER.UpdateInstanceEvent", parameters, null);

                        if (affectedRows == 0)
                        {
                            throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "UpdateInstanceEvent", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
                        }
                        else if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                        {
                            throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "UpdateInstanceEvent", provider.LastExceptionMessage));
                        }
                    }

                    transactionContext.Complete();
                }
            }

            return info;
        }

        /// <summary>
        /// Rimozione di un evento
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static EventInfo RemoveEvent(EventInfo info)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                if (PublisherServiceControl.GetChannelState(GetChannel(info.IdChannel)) == ChannelStateEnum.Started)
                    throw new PublisherException(ErrorCodes.OPERATION_NOT_ALLOWED_PER_SERVICE_STARTED, ErrorDescriptions.OPERATION_NOT_ALLOWED_PER_SERVICE_STARTED);

                using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                {
                    if (info.Id != 0)
                    {
                        // Inserimento evento
                        ArrayList parameters = new ArrayList();

                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_Id", GetObjectParamValue(info.Id), 4, DocsPaUtils.Data.DirectionParameter.ParamInput));

                        int affectedRows = provider.ExecuteStoredProcedure("PUBLISHER.RemoveInstanceEvent", parameters, null);

                        if (affectedRows == 0)
                        {
                            throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "RemoveInstanceEvent", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
                        }
                        else if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                        {
                            throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "RemoveInstanceEvent", provider.LastExceptionMessage));
                        }

                        info.Id = 0;

                        transactionContext.Complete();
                    }
                }
            }

            return info;
        }

        /// <summary>
        /// Aggiornamento dati successivo all'esecuzione di un'istanza
        /// </summary>
        /// <param name="channelRef"></param>
        /// <returns></returns>
        public static ChannelRefInfo UpdateExecutionState(ChannelRefInfo channelRef)
        {
            try
            {
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                    {
                        ArrayList parameters = new ArrayList();

                        parameters.Add(new DocsPaUtils.Data.ParameterSP("IdInstance", GetObjectParamValue(channelRef.Id), 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("ExecutionCount", GetObjectParamValue(channelRef.ExecutionCount), 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("PublishedObjects", GetObjectParamValue(channelRef.PublishedObjects), 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("TotalExecutionCount", GetObjectParamValue(channelRef.TotalExecutionCount), 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("TotalPublishedObjects", GetObjectParamValue(channelRef.TotalPublishedObjects), 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("LastExecutionDate", GetObjectParamValue(channelRef.LastExecutionDate), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.DateTime));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("StartLogDate", GetObjectParamValue(channelRef.StartLogDate), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.DateTime));
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("LastLogId", GetObjectParamValue(channelRef.LastLogId), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));

                        int affectedRows = provider.ExecuteStoredProcedure("PUBLISHER.UpdateExecutionState", parameters, null);

                        if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                        {
                            throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, provider.LastExceptionMessage));
                        }
                        else if (affectedRows == 1)
                            transactionContext.Complete();
                        else
                            throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "UpdateExecutionState", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
                    }
                }
            }
            catch (PublisherException pubEx)
            {
                throw pubEx;
            }
            catch (Exception ex)
            {
                throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message));
            }

            return channelRef;
        }

        /// <summary>
        /// Reperimento delle istanze di pubblicazione registrate nel sistema
        /// </summary>
        /// <returns></returns>
        public static ChannelRefInfo[] GetChannelList()
        {
            try
            {
                List<ChannelRefInfo> channels = new List<ChannelRefInfo>();

                using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                {
                    using (DataSet ds = new DataSet())
                    {
                        ArrayList parameters = new ArrayList();

                        provider.ExecuteStoredProcedure("PUBLISHER.GetPublishInstances", parameters, ds);

                        if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                        {
                            throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, provider.LastExceptionMessage));
                        }

                        if (ds != null && ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                ChannelRefInfo channel = CreateChannelRefInfo(row);
                                channel.Events = GetEventList(channel.Id);
                                channels.Add(channel);
                            }
                        }
                    }
                }

                return channels.ToArray();
            }
            catch (Exception ex)
            {
                throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message));
            }
        }

        /// <summary>
        /// Reperimento delle istanze di pubblicazione registrate per un'amministrazione
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ChannelRefInfo[] GetAdminChannelList(int id)
        {
            try
            {
                List<ChannelRefInfo> channels = new List<ChannelRefInfo>();

                using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                {
                    using (DataSet ds = new DataSet())
                    {
                        ArrayList parameters = new ArrayList();

                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_IdAdmin", id, 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));

                        provider.ExecuteStoredProcedure("PUBLISHER.GetAdminPublishInstances", parameters, ds);

                        if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                        {
                            throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, provider.LastExceptionMessage));
                        }

                        if (ds != null && ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                ChannelRefInfo channel = CreateChannelRefInfo(row);
                                channel.Events = GetEventList(channel.Id);
                                channels.Add(channel);
                            }
                        }
                    }
                }

                return channels.ToArray();
            }
            catch (Exception ex)
            {
                throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ChannelRefInfo GetChannel(int id)
        {
            try
            {
                ChannelRefInfo channel = null;

                using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                {
                    using (DataSet ds = new DataSet())
                    {
                        ArrayList parameters = new ArrayList();

                        parameters.Add(new DocsPaUtils.Data.ParameterSP("Id", id, 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));

                        provider.ExecuteStoredProcedure("PUBLISHER.GetPublishInstance", parameters, ds);

                        if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                        {
                            throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, provider.LastExceptionMessage));
                        }

                        if (ds != null && ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                channel = CreateChannelRefInfo(row);
                                channel.Events = GetEventList(channel.Id);
                            }
                        }
                    }
                }

                return channel;
            }
            catch (Exception ex)
            {
                throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message));
            }
        }

        /// <summary>
        /// Reperimento degli eventi monitorati dall'istanza
        /// </summary>
        /// <param name="idChannel"></param>
        /// <returns></returns>
        public static EventInfo[] GetEventList(int idChannel)
        {
            try
            {
                List<EventInfo> events = new List<EventInfo>();

                using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                {
                    using (DataSet ds = new DataSet())
                    {
                        ArrayList parameters = new ArrayList();

                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_IdInstance", idChannel, 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));

                        provider.ExecuteStoredProcedure("PUBLISHER.GetPublishInstanceEvents", parameters, ds);

                        if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                        {
                            throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, provider.LastExceptionMessage));
                        }

                        if (ds != null && ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                events.Add(CreateEventInfo(row));
                            }
                        }
                    }
                }

                return events.ToArray();
            }
            catch (Exception ex)
            {
                throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message));
            }
        }

        /// <summary>
        /// Reperimento di un evento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EventInfo GetEvent(int id)
        {
            try
            {
                EventInfo info = null;

                using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                {
                    using (DataSet ds = new DataSet())
                    {
                        ArrayList parameters = new ArrayList();

                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_Id", id, 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));

                        provider.ExecuteStoredProcedure("PUBLISHER.GetPublishInstanceEvent", parameters, ds);

                        if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                        {
                            throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, provider.LastExceptionMessage));
                        }

                        if (ds != null && ds.Tables.Count > 0)
                        {
                            info = CreateEventInfo(ds.Tables[0].Rows[0]);
                        }
                    }
                }

                return info;
            }
            catch (Exception ex)
            {
                throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message));
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Aggiornamento delle istanze di pubblicazione
        /// </summary>
        /// <param name="info"></param>
        private static void SaveEvents(ChannelRefInfo info)
        {
            try
            {
                if (info.Events != null && info.Events.Length > 0)
                {
                    using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                    {
                        // Preventiva rimozione degli eventi presenti
                        ArrayList parameters = new ArrayList();
                        parameters.Add(new DocsPaUtils.Data.ParameterSP("p_Id", info.Id, 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                        provider.ExecuteStoredProcedure("PUBLISHER.ClearInstanceEvents", parameters, null);

                        if (!string.IsNullOrEmpty(provider.LastExceptionMessage))
                        {
                            throw new PublisherException(ErrorCodes.SP_EXECUTION_ERROR,
                                string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "ClearInstanceEvents", provider.LastExceptionMessage));
                        }

                        List<EventInfo> newEvents = new List<EventInfo>();

                        foreach (EventInfo ev in info.Events)
                        {
                            ev.Id = 0;
                            newEvents.Add(SaveEvent(ev));
                        }

                        info.Events = newEvents.ToArray();
                    }
                }
            }
            catch (PublisherException pubEx)
            {
                throw pubEx;
            }
            catch (Exception ex)
            {
                throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static EventInfo CreateEventInfo(DataRow row)
        {
            return new EventInfo
            {
                Id = GetNumericFieldValue(row, "ID"),
                IdChannel = GetNumericFieldValue(row, "PUBLISHINSTANCEID"),
                EventName = GetFieldValue<string>(row, "EVENTNAME"),
                ObjectType = GetFieldValue<string>(row, "OBJECTTYPE"),
                ObjectTemplateName = GetFieldValue<string>(row, "OBJECTTEMPLATENAME"),
                DataMapperFullClass = GetFieldValue<string>(row, "DATAMAPPERFULLCLASS"),
                LoadFileIfDocumentType = (GetFieldValue<string>(row, "LOADFILEIFDOCTYPE") == "1")
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static ErrorInfo CreateErrorInfo(DataRow row)
        {
            return new ErrorInfo
            {
                Id = GetNumericFieldValue(row, "ID"),
                IdInstance = GetNumericFieldValue(row, "PUBLISHINSTANCEID"),
                ErrorCode = GetFieldValue<string>(row, "ERRORCODE"),
                ErrorDescription = GetFieldValue<string>(row, "ERRORDESCRIPTION"),
                ErrorStack = GetFieldValue<string>(row, "ERRORSTACK"),
                ErrorDate = GetFieldValue<DateTime>(row, "ERRORDATE")
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static ChannelRefInfo CreateChannelRefInfo(DataRow row)
        {
            ChannelRefInfo channel = new ChannelRefInfo
            {
                Id = GetNumericFieldValue(row, "ID"),
                ChannelName = GetFieldValue<string>(row, "INSTANCENAME"),
                Admin = new AdminInfo
                {
                    Id = GetNumericFieldValue(row, "IDADMIN")
                },
                SubscriberServiceUrl = GetFieldValue<string>(row, "SUBSCRIBERSERVICEURL"),
                ExecutionConfiguration = new JobExecutionConfigurations
                {
                    IntervalType = (JobExecutionConfigurations.IntervalTypesEnum)Enum.Parse(typeof(JobExecutionConfigurations.IntervalTypesEnum), GetFieldValue<string>(row, "EXECUTIONTYPE"), true),
                    ExecutionTicks = GetFieldValue<string>(row, "EXECUTIONTICKS")
                },
                LastExecutionDate = GetFieldValue<DateTime>(row, "LASTEXECUTIONDATE"),
                StartLogDate = GetFieldValue<DateTime>(row, "STARTLOGDATE"),
                LastLogId = GetNumericFieldValue(row, "LASTLOGID"),
                ExecutionCount = GetNumericFieldValue(row, "EXECUTIONCOUNT"),
                PublishedObjects = GetNumericFieldValue(row, "PUBLISHEDOBJECTS"),
                TotalExecutionCount = GetNumericFieldValue(row, "TOTALEXECUTIONCOUNT"),
                TotalPublishedObjects = GetNumericFieldValue(row, "TOTALPUBLISHEDOBJECTS"),
                StartExecutionDate = GetFieldValue<DateTime>(row, "STARTEXECUTIONDATE"),
                EndExecutionDate = GetFieldValue<DateTime>(row, "ENDEXECUTIONDATE"),
                MachineName = GetFieldValue<string>(row, "MACHINENAME"),
                PublisherServiceUrl = GetFieldValue<string>(row, "PUBLISHERSERVICEURL")
            };

            if (channel.StartExecutionDate != DateTime.MinValue)
                channel.State = ChannelStateEnum.Started;
            else
                channel.State = ChannelStateEnum.Stopped;

            return channel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object GetObjectParamValue(object value)
        {
            if (value == null)
                return DBNull.Value;
            else
                return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static int GetNumericFieldValue(DataRow row, string fieldName)
        {
            if (row[fieldName] == DBNull.Value)
                return 0;
            else
                return Convert.ToInt32(row[fieldName]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static T GetFieldValue<T>(DataRow row, string fieldName)
        {
            if (row[fieldName] == DBNull.Value)
                return default(T);
            else
                return (T)row[fieldName];
        }

        #endregion
    }
}
