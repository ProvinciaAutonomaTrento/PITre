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
public class ExportTrasmissioniPendentiReport
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(ExportTrasmissioniPendentiReport));

    [ReportDataExtractorMethod(ContextName = "ExportTrasmissioniPendentiDoc")]
    public DataSet GetTrasmissioniPendentiConWorkflowUtenteDoc(InfoUtente infoUt, List<FiltroRicerca> filters)
    {
        string idPeople = filters.Where(f => f.argomento == "idPeople").FirstOrDefault().valore;
        string idCorrGlobali = filters.Where(f => f.argomento == "idCorrGlobali").FirstOrDefault().valore;
        DataSet dataSet = new DataSet();
        try
        {
            Query query = InitQuery.getInstance().getQuery("S_DPA_TRASM_PENDENTI_PEOPLE_DOC");
            query.setParam("idCorrGlobali", idCorrGlobali);
            query.setParam("idPeople", idPeople);

            string commandText = query.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteQuery(out dataSet, commandText);
            }
        }
        catch (Exception e)
        {
            logger.Error("Errore in GetTrasmissioniPendentiConWorkflowUtente: " + e);
        }

        return dataSet;
    }

    [ReportDataExtractorMethod(ContextName = "ExportTrasmissioniPendentiFasc")]
    public DataSet GetTrasmissioniPendentiConWorkflowUtenteFasc(InfoUtente infoUt, List<FiltroRicerca> filters)
    {
        string idPeople = filters.Where(f => f.argomento == "idPeople").FirstOrDefault().valore;
        string idCorrGlobali = filters.Where(f => f.argomento == "idCorrGlobali").FirstOrDefault().valore;
        DataSet dataSet = new DataSet();
        try
        {
            Query query = InitQuery.getInstance().getQuery("S_DPA_TRASM_PENDENTI_PEOPLE_FASC");
            query.setParam("idCorrGlobali", idCorrGlobali);
            query.setParam("idPeople", idPeople);

            string commandText = query.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteQuery(out dataSet, commandText);
            }
        }
        catch (Exception e)
        {
            logger.Error("Errore in GetTrasmissioniPendentiConWorkflowUtente: " + e);
        }

        return dataSet;
    }

}
