// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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

[ReportDataExtractorClass]
public class AmmExportPolicyPARERReport
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(AmmExportPolicyPARERReport));

    [ReportDataExtractorMethod(ContextName="AmmExportPolicyPARER")]
    public DataSet GetData(InfoUtente utente, List<FiltroRicerca> filters)
    {
        DataSet dataSet = new DataSet();

        using (DBProvider dbProvider = new DBProvider())
        {
            try
            {
                Query query = InitQuery.getInstance().getQuery("S_GET_POLICY_FOR_EXPORT");
                query.setParam("idPolicy", filters.Where(f => f.argomento == "idPolicy").FirstOrDefault().valore);

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

    [ReportDataExtractorMethod(ContextName = "AmmExportPolicyFascPARER")]
    public DataSet GetDataPolicyFascicoli(InfoUtente utente, List<FiltroRicerca> filters)
    {
        var dataSet = new DataSet();

        using(var dbProvider = new DBProvider())
        {
            try
            {
                Query query = InitQuery.getInstance().getQuery("S_GET_POLICY_FASC_FOR_EXPORT");
                query.setParam("idPolicy", filters.Where(f => f.argomento == "idPolicy").FirstOrDefault().valore);

                string command = query.getSQL();
                logger.Debug("QUERY - " + command);

                dbProvider.ExecuteQuery(out dataSet, command);
            }
            catch(Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }

        return dataSet;
    }
}
