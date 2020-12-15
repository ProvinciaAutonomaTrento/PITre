using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Subscriber.DataAccess
{
    /// <summary>
    /// Adapter class per la gestione dei dati relativi allo storico delle regole di pubblicazione
    /// </summary>
    public sealed class RuleHistoryDataAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(RuleHistoryDataAdapter));

        /// <summary>
        /// Storico delle regole eseguite
        /// </summary>
        /// <param name="requestInfo">
        /// Dati di filtro per la ricerca delle regole
        /// </param>
        /// <returns></returns>
        public static GetRuleHistoryListResponse GetRuleHistoryList(GetRuleHistoryListRequest requestInfo)
        {
            _logger.Info("BEGIN");

            try
            {
                GetRuleHistoryListResponse response = new GetRuleHistoryListResponse();

                List<RuleHistoryInfo> list = new List<RuleHistoryInfo>();

                Database db = DbHelper.CreateDatabase();

                //using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("GetRuleHistory")))
                using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("GetRuleHistoryPaging")))
                {
                    cw.AddInParameter("pIdRule", DbType.Int32, requestInfo.IdRule);

                    // Filtri personalizzati
                    if (requestInfo.CustomFilters != null)
                    {
                        cw.AddInParameter("pObjectDescription", DbType.String, GetStringParamValue(requestInfo.CustomFilters.ObjectDescription));
                        cw.AddInParameter("pAuthorName", DbType.String, GetStringParamValue(requestInfo.CustomFilters.AuthorName));
                        cw.AddInParameter("pRoleName", DbType.String, GetStringParamValue(requestInfo.CustomFilters.RoleName));
                    }
                    else
                    {
                        cw.AddInParameter("pObjectDescription", DbType.String, DBNull.Value);
                        cw.AddInParameter("pAuthorName", DbType.String, DBNull.Value);
                        cw.AddInParameter("pRoleName", DbType.String, DBNull.Value);
                    }

                    // Criteri di paginazione
                    if (requestInfo.PagingContext != null)
                    {
                        cw.AddInParameter("pPage", DbType.Int32, requestInfo.PagingContext.PageNumber);
                        cw.AddInParameter("pObjectsPerPage", DbType.Int32, requestInfo.PagingContext.ObjectsPerPage);
                        cw.AddOutParameter("pObjectsCount", DbType.Int32, 4);
                    }
                    else
                    {
                        cw.AddInParameter("pPage", DbType.Int32, 1);    
                        cw.AddInParameter("pObjectsPerPage", DbType.Int32, Int32.MaxValue);
                        cw.AddOutParameter("pObjectsCount", DbType.Int32, 4);
                    }

                    using (IDataReader reader = db.ExecuteReader(cw))
                    {
                        while (reader.Read())
                        {
                            list.Add(CreteRuleHistoryInfo(reader));
                        }

                        object objectsCountParam = cw.GetParameterValue("pObjectsCount");

                        if (objectsCountParam != null)
                        {
                            // Reperimento numero di oggetti totali
                            requestInfo.PagingContext.TotalObjects = Convert.ToInt32(objectsCountParam.ToString());
                        }
                    }
                }

                response.Rules = list.ToArray();
                response.PagingContext = requestInfo.PagingContext;

                return response;
            }
            catch (SubscriberException pubEx)
            {
                _logger.Error(pubEx.Message);

                throw pubEx;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);

                throw new SubscriberException(ErrorCodes.UNHANDLED_ERROR, string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message));
            }
            finally
            {
                _logger.Info("END");
            }
        }

        /// <summary>
        /// Reperimento dei dati dello storico relativo all'ultima pubblicazione effettuata per una regola e per un oggetto
        /// </summary>
        /// <param name="idRule">Id della regola</param>
        /// <param name="idObject">Id dell'oggetto</param>
        /// <returns></returns>
        public static RuleHistoryInfo GetLastRuleHistory(int idRule, string idObject)
        {
            _logger.Info("BEGIN");

            try
            {
                RuleHistoryInfo history = null;

                Database db = DbHelper.CreateDatabase();

                using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("GetLastHistory")))
                {
                    cw.AddInParameter("pIdRule", DbType.Int32, idRule);
                    cw.AddInParameter("pIdObject", DbType.String, idObject);

                    using (IDataReader reader = db.ExecuteReader(cw))
                    {
                        if (reader.Read())
                        {
                            history = CreteRuleHistoryInfo(reader);
                        }
                    }
                }

                return history;
            }
            catch (SubscriberException pubEx)
            {
                _logger.Error(pubEx.Message);

                throw pubEx;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);

                throw new SubscriberException(ErrorCodes.UNHANDLED_ERROR, 
                                string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message),
                                ex);
            }
            finally
            {
                _logger.Info("END");
            }           
        }

        /// <summary>
        /// Reperimento dei dati dello storico a partire da un indentificativo univoco
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static RuleHistoryInfo GetHistoryItem(int id)
        {
            _logger.Info("BEGIN");

            try
            {
                RuleHistoryInfo history = null;

                Database db = DbHelper.CreateDatabase();

                using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("GetHistory")))
                {
                    cw.AddInParameter("pId", DbType.Int32, id);

                    using (IDataReader reader = db.ExecuteReader(cw))
                    {
                        if (reader.Read())
                        {
                            history = CreteRuleHistoryInfo(reader);
                        }
                    }
                }

                return history;
            }
            catch (SubscriberException pubEx)
            {
                _logger.Error(pubEx.Message);

                throw pubEx;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);

                throw new SubscriberException(ErrorCodes.UNHANDLED_ERROR, 
                                string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message),
                                ex);
            }
            finally
            {
                _logger.Info("END");
            }       
        }

        /// <summary>
        /// Aggiornamento dati
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static RuleHistoryInfo SaveHistoryItem(RuleHistoryInfo data)
        {
            _logger.Info("BEGIN");

            try
            {
                Database db = DbHelper.CreateDatabase();

                using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("InsertHistory")))
                {
                    cw.AddInParameter("pRuleId", DbType.Int32, data.IdRule);
                    cw.AddInParameter("pIdObject", DbType.String, data.ObjectSnapshot.IdObject);
                    cw.AddInParameter("pObjectType", DbType.String, data.ObjectSnapshot.ObjectType);
                    cw.AddInParameter("pObjectTemplateName", DbType.String, data.ObjectSnapshot.TemplateName);
                    cw.AddInParameter("pObjectDescription", DbType.String, data.ObjectSnapshot.Description);
                    cw.AddInParameter("pAuthorName", DbType.String, data.Author.Name);
                    cw.AddInParameter("pAuthorId", DbType.String, data.Author.Id);
                    cw.AddInParameter("pRoleName", DbType.String, data.Author.RoleName);
                    cw.AddInParameter("pRoleId", DbType.String, data.Author.IdRole);

                    if (data.ObjectSnapshot != null)
                        cw.AddClobParameter("pObjectSnapshot", ObjectSerializerHelper.Serialize(data.ObjectSnapshot));
                    else
                        cw.AddClobParameter("pObjectSnapshot", DBNull.Value);

                    if (data.MailMessageSnapshot != null)
                        cw.AddClobParameter("pMailMessageSnapshot", ObjectSerializerHelper.Serialize(data.MailMessageSnapshot));
                    else
                        cw.AddClobParameter("pMailMessageSnapshot", DBNull.Value);

                    cw.AddInParameter("pComputed", DbType.StringFixedLength, (data.Published ? "1" : "0"));
                    cw.AddInParameter("pComputeDate", DbType.DateTime, data.PublishDate);

                    if (data.ErrorInfo != null)
                    {
                        cw.AddInParameter("pErrorId", DbType.String, data.ErrorInfo.Id);
                        cw.AddInParameter("pErrorDescription", DbType.String, data.ErrorInfo.Message);
                        cw.AddInParameter("pErrorStack", DbType.String, data.ErrorInfo.Stack);
                    }
                    else
                    {
                        cw.AddInParameter("pErrorId", DbType.String, DBNull.Value);
                        cw.AddInParameter("pErrorDescription", DbType.String, DBNull.Value);
                        cw.AddInParameter("pErrorStack", DbType.String, DBNull.Value);
                    }

                    cw.AddOutParameter("pId", DbType.Int32, 4);

                    db.ExecuteNonQuery(cw);

                    if (cw.RowsAffected == 1)
                    {
                        data.Id = Convert.ToInt32(cw.GetParameterValue("pId"));

                    }
                    else
                        throw new SubscriberException(ErrorCodes.SP_EXECUTION_ERROR,
                            string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "InsertHistory", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
                }

                return data;
            }
            catch (SubscriberException pubEx)
            {
                _logger.Error(pubEx.Message);

                throw pubEx;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);

                throw new SubscriberException(ErrorCodes.UNHANDLED_ERROR, 
                                string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message),
                                ex);
            }
            finally
            {
                _logger.Info("END");
            }   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static RuleHistoryInfo CreteRuleHistoryInfo(IDataReader reader)
        {
            try
            {
                string value = DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, "OBJECTSNAPSHOT", true, string.Empty).ToString();
                PublishedObject pubObj = null;
                if (!string.IsNullOrEmpty(value))
                    pubObj = (PublishedObject)ObjectSerializerHelper.Deserialize(value);

                value = DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, "MAILMESSAGESNAPSHOT", true, string.Empty).ToString();
                Subscriber.Dispatcher.CalendarMail.MailRequest mailRequest = null;
                if (!string.IsNullOrEmpty(value))
                    mailRequest = (Subscriber.Dispatcher.CalendarMail.MailRequest)ObjectSerializerHelper.Deserialize(value);

                return new RuleHistoryInfo
                {
                    Id = Convert.ToInt32(DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, "ID", false).ToString()),
                    IdRule = Convert.ToInt32(DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, "RULEID", false).ToString()),
                    Published = (DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "COMPUTED", false) == "1"),
                    PublishDate = DataAccess.Helper.DataReaderHelper.GetValue<DateTime>(reader, "COMPUTEDATE", true, DateTime.MinValue),
                    ErrorInfo = new ErrorInfo
                    {
                        Id = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "ERRORID", true, string.Empty),
                        Message = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "ERRORDESCRIPTION", true, string.Empty),
                        Stack = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "ERRORSTACK", true, string.Empty)
                    },
                    Author = new EventAuthorInfo
                    {
                        Id = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "AUTHORID", true, string.Empty),
                        Name = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "AUTHORNAME", true, string.Empty),
                        IdRole = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "ROLEID", true, string.Empty),
                        RoleName = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "ROLENAME", true, string.Empty)
                    },
                    ObjectSnapshot = pubObj,
                    MailMessageSnapshot = mailRequest
                };
            }
            catch (SubscriberException pubEx)
            {
                _logger.Error(pubEx.Message);

                throw pubEx;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);

                throw new SubscriberException(ErrorCodes.UNHANDLED_ERROR, 
                            string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message),
                            ex);
            }
            finally
            {
                _logger.Info("END");
            }   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        private static object GetStringParamValue(object paramValue)
        {
            if (paramValue == null)
                return DBNull.Value;

            if (string.IsNullOrEmpty(paramValue.ToString()))
                return DBNull.Value;

            return paramValue.ToString().Trim().Replace("'", "''");
        }
    }
}
