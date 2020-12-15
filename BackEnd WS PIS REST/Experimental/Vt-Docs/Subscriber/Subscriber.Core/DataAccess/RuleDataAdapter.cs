using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Subscriber.DataAccess
{
    /// <summary>
    /// Adapter class per la gestione dei dati relativi alle regole di pubblicazione
    /// </summary>
    public sealed class RuleDataAdapter 
    {
        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(RuleDataAdapter));

        /// <summary>
        /// Reperimento delle regole censite in un canale di pubblicazione
        /// </summary>
        /// <param name="idChannel">Id del canale di pubblicazione</param>
        /// <returns></returns>
        public static RuleInfo[] GetRules(int idChannel)
        {
            _logger.Info("BEGIN");

            List<RuleInfo> list = new List<RuleInfo>();

            try
            {
                Database db = DbHelper.CreateDatabase();

                using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("GetRules")))
                {
                    cw.AddInParameter("pIdInstance", DbType.Int32, idChannel);

                    using (IDataReader reader = db.ExecuteReader(cw))
                    {
                        while (reader.Read())
                        {
                            BaseRuleInfo baseRule = CreateRuleInfo(reader);

                            if (baseRule.GetType() == typeof(RuleInfo))
                            {
                                list.Add((RuleInfo)baseRule);
                            }
                            else if (baseRule.GetType() == typeof(SubRuleInfo))
                            {
                                SubRuleInfo subRule = (SubRuleInfo)baseRule;
                                RuleInfo parentRule = list.Where(e => e.Id == subRule.IdParentRule).First();
                                parentRule.SubRulesList.Add(subRule);
                            }
                        }
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
        /// Reperimento di una regola di pubblicazione
        /// </summary>
        /// <param name="id">Identificativo univoco della regola</param>
        /// <returns></returns>
        public static RuleInfo GetRule(int id)
        {
            _logger.Info("BEGIN");

            RuleInfo rule = null;

            try
            {
                Database db = DbHelper.CreateDatabase();

                using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("GetRule")))
                {
                    cw.AddInParameter("pId", DbType.Int32, id);

                    using (IDataReader reader = db.ExecuteReader(cw))
                    {
                        while (reader.Read())
                        {
                            BaseRuleInfo baseRule = CreateRuleInfo(reader);

                            if (baseRule.GetType() == typeof(RuleInfo))
                                rule = (RuleInfo)baseRule;
                            else if (baseRule.GetType() == typeof(SubRuleInfo) && rule != null)
                                rule.SubRulesList.Add((SubRuleInfo)baseRule);
                        }
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

            return rule;
        }

        /// <summary>
        /// Aggiornamento dati di una regola
        /// </summary>
        /// <param name="data">
        /// Metadati della regola da aggiornare
        /// </param>
        /// <returns></returns>
        public static RuleInfo SaveRule(RuleInfo data)
        {
            _logger.Info("BEGIN");

            try
            {
                bool insertMode = (data.Id == 0);

                if (insertMode)
                    data = InsertRule(data);
                else
                    data = (RuleInfo)UpdateRule(data);

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
        /// Cancellazione di una regola
        /// </summary>
        /// <param name="id">
        /// Identificativo della regola
        /// </param>
        public static void DeleteRule(int id)
        {
            _logger.Info("BEGIN");

            try
            {
                Database db = DbHelper.CreateDatabase();

                using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("DeleteRule")))
                {
                    cw.AddInParameter("pId", DbType.Int32, id);

                    db.ExecuteNonQuery(cw);

                    if (cw.RowsAffected == 0)
                        throw new SubscriberException(ErrorCodes.SP_EXECUTION_ERROR,
                            string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "DeleteRule", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
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
        /// Reperimento delle sottoregole di pubblicazione
        /// </summary>
        /// <param name="idRule">
        /// Id della regola padre
        /// </param>
        /// <returns></returns>
        public static SubRuleInfo[] GetSubRules(int idRule)
        {
            _logger.Info("BEGIN");

            try
            {
                List<SubRuleInfo> list = new List<SubRuleInfo>();

                Database db = DbHelper.CreateDatabase();

                using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("GetSubRules")))
                {
                    cw.AddInParameter("pIdRule", DbType.Int32, idRule);

                    using (IDataReader reader = db.ExecuteReader(cw))
                    {
                        while (reader.Read())
                        {
                            list.Add((SubRuleInfo)CreateRuleInfo(reader));
                        }
                    }
                }

                return list.ToArray();
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
        /// Reperimento di una sottoregola
        /// </summary>
        /// <param name="id">
        /// Identificativo univoco della sottoregola
        /// </param>
        /// <returns></returns>
        public static SubRuleInfo GetSubRule(int id)
        {
            _logger.Info("BEGIN");

            try
            {
                SubRuleInfo rule = null;

                Database db = DbHelper.CreateDatabase();

                using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("GetSubRule")))
                {
                    cw.AddInParameter("pId", DbType.Int32, id);

                    using (IDataReader reader = db.ExecuteReader(cw))
                    {
                        if (reader.Read())
                            rule = (SubRuleInfo)CreateRuleInfo(reader);
                    }
                }

                return rule;
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
        /// Aggiornamento dati di una sottoregola di pubblicazione
        /// </summary>
        /// <param name="subRule">
        /// Dati della sottoregola di pubblicazione
        /// </param>
        /// <returns></returns>
        public static SubRuleInfo SaveSubRule(SubRuleInfo subRule)
        {
            _logger.Info("BEGIN");

            try
            {
                bool insertMode = (subRule.Id == 0);

                if (insertMode)
                    subRule = InsertSubRule(subRule);
                else
                    subRule = (SubRuleInfo)UpdateRule(subRule);

                return subRule;
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
        /// Cancellazione di una sottoregola
        /// </summary>
        /// <param name="id">
        /// Identificativo della sottoregola
        /// </param>
        public static void DeleteSubRule(int id)
        {            
            DeleteRule(id);
        }

        /// <summary>
        /// Inserimento di una regola
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static RuleInfo InsertRule(RuleInfo data)
        {
            _logger.Info("BEGIN");

            try
            {
                Database db = DbHelper.CreateDatabase();

                using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("InsertRule")))
                {
                    cw.AddInParameter("pInstanceId", DbType.Int32, data.IdInstance);
                    cw.AddInParameter("pName", DbType.String, data.RuleName);
                    cw.AddInParameter("pDescription", DbType.String, data.RuleDescription);
                    cw.AddInParameter("pEnabled", DbType.StringFixedLength, (data.Enabled ? "1" : "0"));
                    cw.AddInParameter("pOptions", DbType.String, data.GetOptions());
                    cw.AddInParameter("pParentRuleId", DbType.Int32, DBNull.Value);
                    cw.AddInParameter("pSubName", DbType.String, DBNull.Value);
                    cw.AddInParameter("pClassId", DbType.String, data.RuleClassFullName);
                    cw.AddOutParameter("pOrdinal", DbType.Int32, 4);
                    cw.AddOutParameter("pId", DbType.Int32, 4);

                    db.ExecuteNonQuery(cw);

                    if (cw.RowsAffected == 1)
                    {
                        data.Id = Convert.ToInt32(cw.GetParameterValue("pId"));
                        data.Ordinal = Convert.ToInt32(cw.GetParameterValue("pOrdinal"));

                        List<SubRuleInfo> newSubRules = new List<SubRuleInfo>();

                        foreach (SubRuleInfo subRule in data.SubRules)
                            newSubRules.Add(InsertSubRule(subRule));

                        data.SubRulesList = newSubRules;

                        return data;
                    }
                    else
                        throw new SubscriberException(ErrorCodes.SP_EXECUTION_ERROR,
                            string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "InsertRule", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
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

                throw new SubscriberException(ErrorCodes.UNHANDLED_ERROR, string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message));
            }
            finally
            {
                _logger.Info("END");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static SubRuleInfo InsertSubRule(SubRuleInfo data)
        {
            _logger.Info("BEGIN");

            try
            {
                Database db = DbHelper.CreateDatabase();

                using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("InsertSubRule")))
                {
                    cw.AddInParameter("pInstanceId", DbType.Int32, data.IdInstance);
                    cw.AddInParameter("pDescription", DbType.String, data.RuleDescription);
                    cw.AddInParameter("pEnabled", DbType.StringFixedLength, (data.Enabled ? "1" : "0"));
                    cw.AddInParameter("pOptions", DbType.String, data.GetOptions());
                    cw.AddInParameter("pParentRuleId", DbType.Int32, data.IdParentRule);
                    cw.AddInParameter("pSubName", DbType.String, data.SubRuleName);
                    cw.AddOutParameter("pOrdinal", DbType.Int32, 4);
                    cw.AddOutParameter("pId", DbType.Int32, 4);

                    db.ExecuteNonQuery(cw);

                    if (cw.RowsAffected == 1)
                    {
                        data.Id = Convert.ToInt32(cw.GetParameterValue("pId"));
                        data.Ordinal = Convert.ToInt32(cw.GetParameterValue("pOrdinal"));

                        return data;
                    }
                    else
                        throw new SubscriberException(ErrorCodes.SP_EXECUTION_ERROR,
                            string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "InsertSubRule", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
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

                throw new SubscriberException(ErrorCodes.UNHANDLED_ERROR, string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message));
            }
            finally
            {
                _logger.Info("END");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static BaseRuleInfo UpdateRule(BaseRuleInfo data)
        {
            _logger.Info("BEGIN");

            try
            {
                Database db = DbHelper.CreateDatabase();

                using (DBCommandWrapper cw = db.GetStoredProcCommandWrapper(DbHelper.GetSpNameForPackage("UpdateRule")))
                {
                    cw.AddInParameter("pId", DbType.Int32, data.Id);
                    cw.AddInParameter("pDescription", DbType.String, data.RuleDescription);
                    cw.AddInParameter("pEnabled", DbType.StringFixedLength, (data.Enabled ? "1" : "0"));
                    cw.AddInParameter("pOptions", DbType.String, data.GetOptions());

                    if (data.GetType() == typeof(RuleInfo))
                        cw.AddInParameter("pClassId", DbType.String, ((RuleInfo)data).RuleClassFullName);
                    else
                        cw.AddInParameter("pClassId", DbType.String, DBNull.Value);

                    db.ExecuteNonQuery(cw);

                    if (cw.RowsAffected == 0)
                        throw new SubscriberException(ErrorCodes.SP_EXECUTION_ERROR,
                            string.Format(ErrorDescriptions.SP_EXECUTION_ERROR, "UpdateRule", ErrorDescriptions.SP_EXECUTE_NO_ROWS));
                    else
                    {
                        if (data.GetType() == typeof(RuleInfo))
                        {
                            RuleInfo ruleInfo = (RuleInfo)data;

                            List<SubRuleInfo> newSubRules = new List<SubRuleInfo>();

                            foreach (SubRuleInfo subRule in ruleInfo.SubRules)
                                newSubRules.Add((SubRuleInfo)UpdateRule(subRule));

                            ruleInfo.SubRulesList = newSubRules;
                        }
                    }
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

                throw new SubscriberException(ErrorCodes.UNHANDLED_ERROR, string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message));
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
        private static BaseRuleInfo CreateRuleInfo(IDataReader reader)
        {
            _logger.Info("BEGIN");

            try
            {
                bool isSubRule = (DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, "PARENTRULEID", true, "0").ToString() != "0");

                if (isSubRule)
                {
                    return new SubRuleInfo
                    {
                        Id = Convert.ToInt32(DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, "ID", false).ToString()),
                        IdInstance = Convert.ToInt32(DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, "INSTANCEID", false).ToString()),
                        RuleName = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "NAME", false),
                        RuleDescription = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "DESCRIPTION", true, string.Empty),
                        SubRuleName = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "SUBNAME", false),
                        Enabled = (DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "ENABLED", true, "0") == "1"),
                        Ordinal = Convert.ToInt32(DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, "ORDINAL", false).ToString()),
                        Options = GetOptions(DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "OPTIONS", true, string.Empty)),
                        IdParentRule = Convert.ToInt32(DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, "PARENTRULEID", false).ToString())
                    };
                }
                else
                {
                    return new RuleInfo
                    {
                        Id = Convert.ToInt32(DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, "ID", false).ToString()),
                        IdInstance = Convert.ToInt32(DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, "INSTANCEID", false).ToString()),
                        RuleName = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "NAME", false),
                        RuleDescription = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "DESCRIPTION", true, string.Empty),
                        Enabled = (DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "ENABLED", true, "0") == "1"),
                        Ordinal = Convert.ToInt32(DataAccess.Helper.DataReaderHelper.GetValue<object>(reader, "ORDINAL", false).ToString()),
                        Options = GetOptions(DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "OPTIONS", true, string.Empty)),
                        RuleClassFullName = DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "CLASS_ID", true, string.Empty)
                    };
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

                throw new SubscriberException(ErrorCodes.UNHANDLED_ERROR, string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message));
            }
            finally
            {
                _logger.Info("END");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static NameValuePair[] GetOptions(string value)
        {
            _logger.Info("BEGIN");

            try
            {
                List<NameValuePair> list = new List<NameValuePair>();

                foreach (string pair in value.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] items = pair.Split(new char[1] { ':' }, StringSplitOptions.None);

                    list.Add(
                            new NameValuePair
                            {
                                Name = items[0],
                                Value = items[1]
                            }
                        );
                }

                return list.ToArray();
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
    }
}
