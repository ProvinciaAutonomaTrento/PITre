// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaUtils;
using DocsPaVO.filtri;
using DocsPaVO.Report;
using DocsPaVO.utente;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DocsPaDB.Query_DocsPAWS.Reporting;

[ReportDataExtractorClass()]
public class ReportVersamentiReport
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(ReportVersamentiReport));


    [ReportDataExtractorMethod(ContextName="ReportVersamentiPARER")]
    public DataSet GetDataSet(InfoUtente infoUt, List<FiltroRicerca> filters)
    {
        DataSet dataSet = new DataSet();

        using (DBProvider dbProvider = new DBProvider())
        {
            try
            {
                Query query = InitQuery.getInstance().getQuery("S_GET_REPORT_POLICY_PARER");
                query.setParam("stato", filters.Where(f => f.argomento.Equals("stato")).FirstOrDefault().valore);
                query.setParam("idAmm", filters.Where(f => f.argomento.Equals("idAmm")).FirstOrDefault().valore);

                if(filters.Where(f=> f.argomento.Equals("idRegistro")).FirstOrDefault() != null)
                {
                    string idRegistro = filters.Where(f => f.argomento.Equals("idRegistro")).FirstOrDefault().valore;
                    if(!string.IsNullOrEmpty(idRegistro))
                    {
                        query.setParam("registro", " AND P.ID_REGISTRO=" + idRegistro);
                    }
                    else
                    {
                        query.setParam("registro", string.Empty);
                    }
                }
                else
                {
                    query.setParam("registro", string.Empty);
                }

                string command = query.getSQL();
                logger.Debug("QUERY - " + command);
                dbProvider.ExecuteQuery(out dataSet, command);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }

        return dataSet;
    }
}
