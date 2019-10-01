using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using System.Collections;

namespace Publisher.VtDocs
{
    /// <summary>
    /// Classe per l'accesso ai log del sistema documentale
    /// </summary>
    public sealed class LogDataAdapter
    {
        #region Public Members

        /// <summary>
        /// Ricerca dei log nel sistema documentale
        /// </summary>
        /// <param name="criteria">
        /// Criteri di filtro
        /// </param>
        /// <returns></returns>
        public static LogInfo[] GetLogs(LogCriteria criteria)
        {
            try
            {
                List<LogInfo> logs = new List<LogInfo>();

                using (DocsPaDB.DBProvider provider = new DocsPaDB.DBProvider())
                {
                    ArrayList parameters = new ArrayList();

                    parameters.Add(new DocsPaUtils.Data.ParameterSP("IdAdmin", criteria.Admin.Id, 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("FromLogId", criteria.FromLogId, 4, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.Int32));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("ObjectType", (string.IsNullOrEmpty(criteria.ObjectType) ? string.Empty : criteria.ObjectType), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("ObjectTemplateName", (string.IsNullOrEmpty(criteria.ObjectTemplateName) ? string.Empty : criteria.ObjectTemplateName), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("EventName", (string.IsNullOrEmpty(criteria.EventName) ? string.Empty : criteria.EventName), 0, DocsPaUtils.Data.DirectionParameter.ParamInput, DbType.String));

                    using (DataSet ds = new DataSet())
                    {
                        provider.ExecuteStoredProcedure("PUBLISHER.GetLogs", parameters, ds);

                        if (ds != null && ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                logs.Add(new LogInfo
                                {
                                    Id = GetNumericFieldValue(row, "ID"),
                                    IdAdmin = GetNumericFieldValue(row, "ID_ADMIN"),
                                    IdUser = GetNumericFieldValue(row, "ID_USER"),
                                    UserName = GetFieldValue<string>(row, "USER_NAME"),
                                    IdRole = GetNumericFieldValue(row, "ID_ROLE"),
                                    RoleCode = GetFieldValue<string>(row, "ROLE_CODE"),
                                    RoleDescription = GetFieldValue<string>(row, "ROLE_DESCRIPTION"),
                                    ObjectId = GetNumericFieldValue(row, "ID_OBJECT"),
                                    ObjectType = GetFieldValue<string>(row, "OBJECT_TYPE"),
                                    EventCode = GetFieldValue<string>(row, "EVENT_CODE"),
                                    EventDescription = GetFieldValue<string>(row, "EVENT_DESCRIPTION"),
                                    Data = GetFieldValue<DateTime>(row, "EVENT_DATE"),
                                    ObjectDescription= GetFieldValue<string>(row,"OBJECT_DESCRIPTION")
                                });
                            }
                        }
                    }
                }

                return logs.ToArray();
            }
            catch (Exception ex)
            {
                throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, ex.Message);
            }
        }

        #endregion

        #region Private Members

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