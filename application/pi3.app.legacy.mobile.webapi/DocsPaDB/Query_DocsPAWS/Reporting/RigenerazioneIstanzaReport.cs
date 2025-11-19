// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaUtils;
using DocsPaVO.filtri;
using DocsPaVO.Report;
using DocsPaVO.utente;
using Serilog;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DocsPaDB.Query_DocsPAWS.Reporting;

[ReportDataExtractorClass()]
public class RigenerazioneIstanzaReport
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(RigenerazioneIstanzaReport));

    [ReportDataExtractorMethod(ContextName="RigenerazioneIstanza")]
    public DataSet GetInfoRigenerazioneIstanza(InfoUtente infoUtente, List<FiltroRicerca> filters)
    {
        DataSet dataSet = new DataSet();

        using (DBProvider dbProvider = new DBProvider())
        {
            Query query = InitQuery.getInstance().getQuery("S_NOTIFICHE_RIFIUTO_REPORT");
            
            string idIstanza = filters.Where(f => f.argomento == "idIstanza").FirstOrDefault().valore;
            query.setParam("idIst", idIstanza);

            string commandText = query.getSQL();

            logger.Debug("QUERY - SQL: " + commandText);
            dbProvider.ExecuteQuery(out dataSet, commandText);

        }

        return dataSet;
    }

}
