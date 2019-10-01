using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DocsPaVO.utente;
using DocsPaVO.filtri;
using DocsPaUtils;
using DocsPaVO.Report;
using log4net;

namespace DocsPaDB.Query_DocsPAWS.Reporting
{

    /// <summary>
    /// Questa classe viene utilizzata per l'estrazione dei dati relativi al report delle istanze di conservazione
    /// 
    /// </summary>
    [ReportDataExtractorClass()]
    class ReportIstanzeDiConservazione
    {
        private static ILog logger = LogManager.GetLogger(typeof(ReportIstanzeDiConservazione));

        [ReportDataExtractorMethod(ContextName = "IstanzeConservazione")]
        public DataSet GetReportConservazione(InfoUtente userInfo, List<FiltroRicerca> searchFilters)
        { 
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Reperimento query da eseguire ed esecuzione
                Query query = InitQuery.getInstance().getQuery("S_REPORT_ISTANZE_CONSERVAZIONE");
                

                // Impostazione filtri
                query.setParam("filterParam", this.GetConditionFilter(searchFilters));
                query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                logger.Debug(query.getSQL());
                dbProvider.ExecuteQuery(out dataSet, query.getSQL());

            }

            return dataSet;
        }

        private String GetConditionFilter(List<FiltroRicerca> searchFilters)
        {
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
            

            //dichiarazioni variabili per il settaggio dei filtri-parametro nella query
            string andStr = string.Empty;
            int numAndStr = 0;
            // Analisi dei filtri
            foreach (FiltroRicerca f in searchFilters)
            {
                switch (f.argomento)
                {
                    case "DATA_INVIO_IL":

                        andStr += " AND ";

                        numAndStr += 1;

                        andStr += "AC.DATA_INVIO >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND AC.DATA_INVIO <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);

                        break;

                    case "DATA_INVIO_AL":

                        andStr += " AND ";

                        numAndStr += 1;

                        andStr += "AC.DATA_INVIO <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);

                        break;

                    case "DATA_INVIO_DAL":

                        andStr += " AND ";

                        numAndStr += 1;

                        andStr += "AC.DATA_INVIO >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);

                        break;

                    case "DATA_INVIO_SC":

                        // data protocollo nella settimana corrente

                        andStr += " AND ";

                        numAndStr += 1;

                        if (!dbType.ToUpper().Equals("SQL"))

                            andStr += "AC.DATA_INVIO >=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND AC.DATA_INVIO <(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";

                        else

                            andStr += "AC.DATA_INVIO >=(select CAST(DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE()) as date)) AND AC.DATA_INVIO <=(select CAST(DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE()) as date)) ";

                        break;

                    case "DATA_INVIO_MC":

                        // data protocollo nel mese corrente

                        andStr += " AND ";

                        numAndStr += 1;

                        if (!dbType.ToUpper().Equals("SQL"))

                            andStr += "AC.DATA_INVIO >= Trunc(Sysdate,'MM')  AND AC.DATA_INVIO <(Sysdate+1 ) ";

                        else

                            andStr += "AC.DATA_INVIO >=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND AC.DATA_INVIO <=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";

                        break;

                    case "DATA_INVIO_TODAY":

                        andStr += " AND ";

                        numAndStr += 1;

                        if (!dbType.ToUpper().Equals("SQL"))

                            andStr += "AC.DATA_INVIO between trunc(sysdate ,'DD') and sysdate";

                        else

                            andStr += "DATEDIFF(DD, AC.DATA_INVIO, GETDATE()) = 0 ";

                        break;

                    case "DATA_CHIUSURA_IL":

                        andStr += " AND ";

                        numAndStr += 1;

                        andStr += "AC.DATA_CONSERVAZIONE >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND AC.DATA_CONSERVAZIONE <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);

                        break;

                    case "DATA_CHIUSURA_AL":

                        andStr += " AND ";

                        numAndStr += 1;

                        andStr += "AC.DATA_CONSERVAZIONE <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);

                        break;

                    case "DATA_CHIUSURA_DAL":

                        andStr += " AND ";

                        numAndStr += 1;

                        andStr += "AC.DATA_CONSERVAZIONE >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);

                        break;

                    case "DATA_CHIUSURA_SC":

                        // data protocollo nella settimana corrente

                        andStr += " AND ";

                        numAndStr += 1;

                        if (!dbType.ToUpper().Equals("SQL"))

                            andStr += "AC.DATA_CONSERVAZIONE >=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND AC.DATA_CONSERVAZIONE <(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";

                        else

                            andStr += "AC.DATA_CONSERVAZIONE >=(select CAST(DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE()) as date)) AND AC.DATA_CONSERVAZIONE <=(select CAST(DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE()) as date)) ";

                        break;

                    case "DATA_CHIUSURA_MC":

                        // data protocollo nel mese corrente

                        andStr += " AND ";

                        numAndStr += 1;

                        if (!dbType.ToUpper().Equals("SQL"))

                            andStr += "AC.DATA_CONSERVAZIONE >= Trunc(Sysdate,'MM')  AND AC.DATA_CONSERVAZIONE <(Sysdate+1 ) ";

                        else

                            andStr += "AC.DATA_CONSERVAZIONE >=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND AC.DATA_CONSERVAZIONE <=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";

                        break;

                    case "DATA_CHIUSURA_TODAY":

                        andStr += " AND ";

                        numAndStr += 1;

                        if (!dbType.ToUpper().Equals("SQL"))

                            andStr += "AC.DATA_CONSERVAZIONE between trunc(sysdate ,'DD') and sysdate";

                        else

                            andStr += "DATEDIFF(DD, AC.DATA_CONSERVAZIONE, GETDATE()) = 0 ";

                        break;

                    case "DATA_RIFIUTO_IL":

                        andStr += " AND ";

                        numAndStr += 1;

                        andStr += "AC.DATA_RIFIUTO >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND AC.DATA_RIFIUTO <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);

                        break;

                    case "DATA_RIFIUTO_AL":

                        andStr += " AND ";

                        numAndStr += 1;

                        andStr += "AC.DATA_RIFIUTO <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);

                        break;

                    case "DATA_RIFIUTO_DAL":

                        andStr += " AND ";

                        numAndStr += 1;

                        andStr += "AC.DATA_RIFIUTO >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);

                        break;

                    case "DATA_RIFIUTO_SC":

                        // data protocollo nella settimana corrente

                        andStr += " AND ";

                        numAndStr += 1;

                        if (!dbType.ToUpper().Equals("SQL"))

                            andStr += "AC.DATA_RIFIUTO >=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND AC.DATA_RIFIUTO <(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";

                        else

                            andStr += "AC.DATA_RIFIUTO >=(select CAST(DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE()) as date)) AND AC.DATA_RIFIUTO <=(select CAST(DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE()) as date)) ";

                        break;

                    case "DATA_RIFIUTO_MC":

                        // data protocollo nel mese corrente

                        andStr += " AND ";

                        numAndStr += 1;

                        if (!dbType.ToUpper().Equals("SQL"))

                            andStr += "AC.DATA_RIFIUTO >= Trunc(Sysdate,'MM')  AND AC.DATA_RIFIUTO <(Sysdate+1 ) ";

                        else

                            andStr += "AC.DATA_RIFIUTO >=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND AC.DATA_RIFIUTO <=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";

                        break;

                    case "DATA_RIFIUTO_TODAY":

                        andStr += " AND ";

                        numAndStr += 1;

                        if (!dbType.ToUpper().Equals("SQL"))

                            andStr += "AC.DATA_RIFIUTO between trunc(sysdate ,'DD') and sysdate";

                        else

                            andStr += "DATEDIFF(DD, AC.DATA_INVIO, GETDATE()) = 0 ";

                        break;


                }
            }

            return andStr;
        }

    }
}
