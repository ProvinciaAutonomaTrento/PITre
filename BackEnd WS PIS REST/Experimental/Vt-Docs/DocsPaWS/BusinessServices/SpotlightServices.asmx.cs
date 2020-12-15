using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using VtDocs.BusinessServices.Entities;

namespace DocsPaWS.BusinessServices
{
    /// <summary>
    /// 
    /// </summary>
    [WebService(Namespace = "http://www.valueteam.com/VtDocs/Business/SpotlightServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class SpotlightServices : BusinessServices, VtDocs.BusinessServices.Spotlight.ISpotlightService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Spotlight.SpotlightResponse SpotlightDocumenti(VtDocs.BusinessServices.Entities.Spotlight.SpotlightRequest request)
        {
            VtDocs.BusinessServices.Entities.Spotlight.SpotlightResponse response = new VtDocs.BusinessServices.Entities.Spotlight.SpotlightResponse();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    string filterString = string.Format("{0}*", request.Filter).Replace("'", "''");

                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("SPOTLIGHT_GET_COUNT_DOCUMENTI");
                    queryDef.setParam("filter", filterString);
                    queryDef.setParam("idPeople", request.InfoUtente.idPeople);
                    queryDef.setParam("idGruppo", request.InfoUtente.idGruppo);

                    string commandText = queryDef.getSQL();

                    string countField;
                    if (dbProvider.ExecuteScalar(out countField, commandText))
                    {
                        response.TotalRecordCount = Convert.ToInt32(countField);

                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("SPOTLIGHT_GET_DOCUMENTI");
                        queryDef.setParam("filter", filterString);
                        queryDef.setParam("idPeople", request.InfoUtente.idPeople);
                        queryDef.setParam("idGruppo", request.InfoUtente.idGruppo);
                        queryDef.setParam("start", request.Start.ToString());
                        queryDef.setParam("limit", request.Limit.ToString());

                        commandText = queryDef.getSQL();

                        List<VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultRecord> records = new List<VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultRecord>();

                        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                        {
                            while (reader.Read())
                            {
                                List<VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultCell> cells = new List<VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultCell>();

                                cells.Add
                                    (
                                        new VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultCell
                                        {
                                            FieldName = "IdProfile",
                                            FieldValue = reader.GetValue(reader.GetOrdinal("IdProfile")).ToString()
                                        }
                                    );

                                cells.Add
                                (
                                    new VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultCell
                                    {
                                        FieldName = "Oggetto",
                                        FieldValue = reader.GetString(reader.GetOrdinal("Oggetto"))
                                    }
                                );

                                cells.Add
                                (
                                    new VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultCell
                                    {
                                        FieldName = "DocName",
                                        FieldValue = reader.GetString(reader.GetOrdinal("DocName"))
                                    }
                                );

                                records.Add(
                                new VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultRecord
                                {
                                    Cells = cells.ToArray()
                                });
                            }
                        }

                        response.Records = records.ToArray();
                        response.Success = true;
                    }
                    else
                        throw new ApplicationException(dbProvider.LastExceptionMessage);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Ricerca spotlight per i fascicoli
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Spotlight.SpotlightResponse SpotlightFascicoli(VtDocs.BusinessServices.Entities.Spotlight.SpotlightRequest request)
        {
            VtDocs.BusinessServices.Entities.Spotlight.SpotlightResponse response = new VtDocs.BusinessServices.Entities.Spotlight.SpotlightResponse();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    string filterString = string.Format("{0}*", request.Filter).Replace("'", "''");

                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("SPOTLIGHT_GET_COUNT_FASCICOLI");
                    queryDef.setParam("filter", filterString);
                    queryDef.setParam("idPeople", request.InfoUtente.idPeople);
                    queryDef.setParam("idGruppo", request.InfoUtente.idGruppo);

                    string commandText = queryDef.getSQL();

                    string countField;
                    if (dbProvider.ExecuteScalar(out countField, commandText))
                    {
                        response.TotalRecordCount = Convert.ToInt32(countField);

                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("SPOTLIGHT_GET_FASCICOLI");
                        queryDef.setParam("filter", filterString);
                        queryDef.setParam("idPeople", request.InfoUtente.idPeople);
                        queryDef.setParam("idGruppo", request.InfoUtente.idGruppo);
                        queryDef.setParam("start", request.Start.ToString());
                        queryDef.setParam("limit", request.Limit.ToString());

                        commandText = queryDef.getSQL();

                        List<VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultRecord> records = new List<VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultRecord>();

                        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                        {
                            while (reader.Read())
                            {
                                List<VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultCell> cells = new List<VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultCell>();

                                cells.Add
                                    (
                                        new VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultCell
                                        {
                                            FieldName = "Id",
                                            FieldValue = reader.GetValue(reader.GetOrdinal("Id")).ToString()
                                        }
                                    );

                                cells.Add
                                (
                                    new VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultCell
                                    {
                                        FieldName = "Codice",
                                        FieldValue = reader.GetString(reader.GetOrdinal("Codice"))
                                    }
                                );

                                cells.Add
                                (
                                    new VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultCell
                                    {
                                        FieldName = "Descrizione",
                                        FieldValue = reader.GetString(reader.GetOrdinal("Descrizione"))
                                    }
                                );

                                cells.Add
                                (
                                    new VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultCell
                                    {
                                        FieldName = "Tipo",
                                        FieldValue = reader.GetString(reader.GetOrdinal("Tipo"))
                                    }
                                );

                                records.Add(
                                new VtDocs.BusinessServices.Entities.Spotlight.SpotlightResultRecord
                                {
                                    Cells = cells.ToArray()
                                });
                            }
                        }

                        response.Records = records.ToArray();
                        response.Success = true;
                    }
                    else
                        throw new ApplicationException(dbProvider.LastExceptionMessage);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }
    }
}
