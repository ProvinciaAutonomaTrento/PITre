using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Subscriber.DataAccess
{
    /// <summary>
    /// Adapter class per la gestione dei dati relativi ai canali di pubblicazione
    /// </summary>
    public sealed class ChannelDataAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(ChannelDataAdapter));

        /// <summary>
        /// Reperimento della lista dei canali di pubblicazione
        /// </summary>
        /// <returns></returns>
        public static ChannelInfo[] GetList()
        {
            _logger.Info("BEGIN");

            List<ChannelInfo> list = new List<ChannelInfo>();
            
            try
            {
                Database db = DbHelper.CreateDatabase();

                using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("GetInstances")))
                {
                    using (IDataReader reader = db.ExecuteReader(cw))
                    {
                        while (reader.Read())
                            list.Add(CreateChannelInfo(reader));
                    }
                }
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

            return list.ToArray();
        }

        /// <summary>
        /// Reperimento di un canale di pubblicazione
        /// </summary>
        /// <param name="id">Identificativo univoco</param>
        /// <returns></returns>
        public static ChannelInfo Get(int id)
        {
            _logger.Info("BEGIN");

            ChannelInfo instance = null;

            try
            {
                Database db = DbHelper.CreateDatabase();

                using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("GetInstance")))
                {
                    cw.AddInParameter("pId", DbType.Int32, id);

                    using (IDataReader reader = db.ExecuteReader(cw))
                    {
                        if (reader.Read())
                            instance = CreateChannelInfo(reader);
                    }
                }
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

            return instance;
        }

        /// <summary>
        /// Aggiornamento dati di un canale di pubblicazione
        /// </summary>
        /// <param name="data">
        /// Metadati del canale 
        /// </param>
        /// <returns></returns>
        public static ChannelInfo Save(ChannelInfo data)
        {
            _logger.Info("BEGIN");

            try
            {
                bool insertMode = (data.Id == 0);

                Database db = DbHelper.CreateDatabase();

                if (insertMode)
                {
                    using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("InsertInstance")))
                    {
                        cw.AddInParameter("pName", DbType.String, data.Name);
                        cw.AddInParameter("pDescription", DbType.String, data.Description);
                        cw.AddInParameter("pSmtpHost", DbType.String, data.SmtpHost);
                        cw.AddInParameter("pSmtpPort", DbType.Int32, data.SmtpPort);
                        cw.AddInParameter("pSmtpSsl", DbType.StringFixedLength, (data.SmtpSsl ? "1" : "0"));
                        cw.AddInParameter("pSmtpUserName", DbType.String, data.SmtpUserName);
                        cw.AddInParameter("pSmtpPassword", DbType.String, data.SmtpPassword);
                        cw.AddInParameter("pSmtpMail", DbType.String, data.SmtpMail);
                        cw.AddOutParameter("pId", DbType.Int32, 0);

                        db.ExecuteNonQuery(cw);

                        if (cw.RowsAffected == 1)
                            data.Id = Convert.ToInt32(cw.GetParameterValue("pId"));
                        else
                            throw new SubscriberException(ErrorCodes.SP_EXECUTION_ERROR,
                                  string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "InsertInstance", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
                    }
                }
                else
                {
                    using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("UpdateInstance")))
                    {
                        cw.AddInParameter("pId", DbType.Int32, data.Id);
                        cw.AddInParameter("pName", DbType.String, data.Name);
                        cw.AddInParameter("pDescription", DbType.String, data.Description);
                        cw.AddInParameter("pSmtpHost", DbType.String, data.SmtpHost);
                        cw.AddInParameter("pSmtpPort", DbType.Int32, data.SmtpPort);
                        cw.AddInParameter("pSmtpSsl", DbType.StringFixedLength, (data.SmtpSsl ? "1" : "0"));
                        cw.AddInParameter("pSmtpUserName", DbType.String, data.SmtpUserName);
                        cw.AddInParameter("pSmtpPassword", DbType.String, data.SmtpPassword);
                        cw.AddInParameter("pSmtpMail", DbType.String, data.SmtpMail);

                        db.ExecuteNonQuery(cw);

                        if (cw.RowsAffected == 0)
                            throw new SubscriberException(ErrorCodes.SP_EXECUTION_ERROR,
                                  string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "UpdateInstance", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
                    }
                }
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

            return data;
        }

        /// <summary>
        /// Rimozione di un canale di pubblicazione
        /// </summary>
        /// <param name="id">
        /// Identificativo univoco del canale
        /// </param>
        public static void Delete(int id)
        {
            _logger.Info("BEGIN");

            try
            {
                Database db = DbHelper.CreateDatabase();

                using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("DeleteInstance")))
                {
                    cw.AddInParameter("pId", DbType.Int32, id);

                    db.ExecuteNonQuery(cw);

                    if (cw.RowsAffected == 0)
                        throw new SubscriberException(ErrorCodes.SP_EXECUTION_ERROR,
                              string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "DeleteInstance", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
                }
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
        private static ChannelInfo CreateChannelInfo(IDataReader reader)
        {
            _logger.Info("BEGIN");

            try
            {
                return new ChannelInfo
                {
                    Id = Convert.ToInt32(DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, "ID", false).ToString()),
                    Name = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "NAME", false),
                    Description = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "DESCRIPTION", true, string.Empty),
                    SmtpHost = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "SMTPHOST", true, string.Empty),
                    SmtpPort = Convert.ToInt32(DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, "SMTPPORT", true, "0").ToString()),
                    SmtpSsl = (DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "SMTPSSL", true, "0") == "1"),
                    SmtpUserName = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "SMTPUSERNAME", true, string.Empty),
                    SmtpPassword = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "SMTPPASSWORD", true, string.Empty),
                    SmtpMail = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "SMTPMAIL", true, string.Empty)
                };
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
    }
}
