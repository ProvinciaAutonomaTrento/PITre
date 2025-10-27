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
class ExportDistributionListReport
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(ExportDistributionListReport));

    [ReportDataExtractorMethod(ContextName = "ExportDistributionList")]
    public DataSet GetDataDistributionListExport(InfoUtente infoUtente, List<FiltroRicerca> filters)
    {
        DataSet dataSet = new DataSet();

        using (DBProvider dbProvider = new DBProvider())
        {
            try
            {
                Query query = InitQuery.getInstance().getQuery("S_GET_DATI_LISTA_DISTR");
                string userId = infoUtente.userId;
                string role = infoUtente.idGruppo;
                string listCode = filters.Where(f => f.argomento == "codiceLista").FirstOrDefault().valore;
                //query.setParam("codLista", listCode);
                query.setParam("userId", userId);
                query.setParam("roleId", role);
                query.setParam("listId", listCode);
                query.setParam("ammId", infoUtente.idAmministrazione);

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

