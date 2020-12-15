using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DocsPaVO.filtri;
using DocsPaVO.Report;
using DocsPaVO.utente;
using DocsPaUtils;
using log4net;

namespace DocsPaDB.Query_DocsPAWS.Reporting
{
    [ReportDataExtractorClass]
    public class AmmMonitoraggioPolicyReport
    {
        private ILog logger = LogManager.GetLogger(typeof(AmmMonitoraggioPolicyReport));

        [ReportDataExtractorMethod(ContextName ="AmmMonitoraggioPolicy")]
        public DataSet GetData(InfoUtente utente, List<FiltroRicerca> filters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    Query query = InitQuery.getInstance().getQuery("S_AMM_CONS_REPORT_POLICY");

                    string idAmm = filters.Where(f => f.argomento == "id_amm").FirstOrDefault().valore;
                    string codice = filters.Where(f => f.argomento == "codice_policy").FirstOrDefault().valore;
                    string descrizione = filters.Where(f => f.argomento == "descrizione_policy").FirstOrDefault().valore;
                    string dataEsecuzioneTipo = filters.Where(f => f.argomento == "data_esecuzione_tipo").FirstOrDefault().valore;
                    string dataEsecuzioneDa = filters.Where(f => f.argomento == "data_esecuzione_da").FirstOrDefault().valore;
                    string dataEsecuzioneA = filters.Where(f => f.argomento == "data_esecuzione_a").FirstOrDefault().valore;

                    string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

                    if (string.IsNullOrEmpty(idAmm) || idAmm == "0")
                    {
                        query.setParam("filtro_amm", string.Empty);
                    }
                    else
                    {
                        query.setParam("filtro_amm", " AND A.SYSTEM_ID = " + idAmm);
                    }
                    if(string.IsNullOrEmpty(codice))
                    {
                        query.setParam("filtro_codice", string.Empty);
                    }
                    else
                    {
                        query.setParam("filtro_codice", string.Format(" AND UPPER(P.VAR_CODICE) LIKE UPPER('%{0}%') ", codice));
                    }
                    if(string.IsNullOrEmpty(descrizione))
                    {
                        query.setParam("filtro_descrizione", string.Empty);
                    }
                    else
                    {
                        query.setParam("filtro_descrizione", string.Format(" AND UPPER(P.VAR_DESCRIZIONE) LIKE UPPER('%{0}%') ", descrizione));
                    }
                    if(dataEsecuzioneTipo == "S")
                    {
                        string cond = " AND DATA_ESECUZIONE_POLICY >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(dataEsecuzioneDa, true);
                        cond = cond + " AND DATA_ESECUZIONE_POLICY <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(dataEsecuzioneDa, false);
                        query.setParam("filtro_data", cond);
                    }
                    else if(dataEsecuzioneTipo == "R")
                    {
                        string cond = string.Empty;
                        if(!string.IsNullOrEmpty(dataEsecuzioneDa))
                        {
                            cond = cond + " AND DATA_ESECUZIONE_POLICY >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(dataEsecuzioneDa, true);
                        }
                        if(!string.IsNullOrEmpty(dataEsecuzioneA))
                        {
                            cond = cond + " AND DATA_ESECUZIONE_POLICY <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(dataEsecuzioneA, false);
                        }
                        query.setParam("filtro_data", cond);
                    }
                    else if(dataEsecuzioneTipo == "M")
                    {
                        string cond;
                        if(dbType.ToUpper() == "SQL")
                        {
                            cond = " AND DATA_ESECUZIONE_POLICY>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND DATA_ESECUZIONE_POLICY<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                        }
                        else
                        {
                            cond = " AND DATA_ESECUZIONE_POLICY>= Trunc(Sysdate,'MM')    AND DATA_ESECUZIONE_POLICY<(Sysdate+1 ) ";
                        }
                        query.setParam("filtro_data", cond);
                    }

                    string command = query.getSQL();
                    logger.Debug("QUERY: " + command);

                    if (!dbProvider.ExecuteQuery(out dataSet, command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                }
                catch (Exception ex)
                {
                    logger.Debug(ex);
                    dataSet = null;
                }
            }

            return dataSet;
        }
    }
}
