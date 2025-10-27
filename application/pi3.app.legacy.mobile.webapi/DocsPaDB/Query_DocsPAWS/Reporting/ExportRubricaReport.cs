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

[ReportDataExtractorClass()]
public class ExportRubricaReport
{

    private static ILogger logger = Serilog.Log.ForContext(typeof(ExportRubricaReport));

    [ReportDataExtractorMethod(ContextName="ExportRubrica")]
    public DataSet GetDataListaCorrExport(InfoUtente infoUtente, List<FiltroRicerca> filters)
    {
        DataSet dataSet = new DataSet();

        using (DBProvider dbProvider = new DBProvider())
        {
            try
            {
                Query query = InitQuery.getInstance().getQuery("S_RICERCA_ALL_CORRISPONDENTI");

                string registri = filters.Where(f => f.argomento == "registri").FirstOrDefault().valore;
                if (!string.IsNullOrEmpty(registri))
                    query.setParam("registri", " AND ( r.system_id in (" + registri + ") OR r.system_id IS NULL)");
                else
                    query.setParam("registri", string.Empty);
                if (infoUtente != null && !string.IsNullOrEmpty(infoUtente.idAmministrazione))
                    query.setParam("idamm", infoUtente.idAmministrazione);
                else
                    query.setParam("idamm", string.Empty);

                string commandText = query.getSQL();
                logger.Debug("QUERY - " + commandText);

                dbProvider.ExecuteQuery(out dataSet, commandText);
                
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }
        return dataSet;

    }

    
}
