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
    [ReportDataExtractorClass()]
    public class ExportProcessiDiFirmaReport
    {
        private ILog logger = LogManager.GetLogger(typeof(AmmExportMacroFunzioniReport));

        [ReportDataExtractorMethod(ContextName = "ExportProcessiFirma")]
        public DataSet GetProcessiDiFirmaByTitolare(InfoUtente infoUt, List<FiltroRicerca> filters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    Query query = InitQuery.getInstance().getQuery("S_EXPORT_DPA_SCHEMA_PROCESSO_FIRMA_BY_TITOLARE");

                    string idRuoloTitolare = filters.Where(f => f.argomento == "idRuolo").FirstOrDefault().valore;
                    string idUtenteTitolare = filters.Where(f => f.argomento == "idUtente").FirstOrDefault().valore;

                    if (string.IsNullOrEmpty(idRuoloTitolare))
                    {
                        string idRuoli = string.Empty;
                        LibroFirma libroFirma = new LibroFirma();
                        List<string> listIdRuoli = libroFirma.GetIdRuoliProcessiUltimoUtente(idUtenteTitolare);
                        foreach (string id in listIdRuoli)
                            idRuoli += string.IsNullOrEmpty(idRuoli) ? id : ", " + id;

                        query.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ((ID_RUOLO_COINVOLTO IN (" + idRuoli + ") AND ID_UTENTE_COINVOLTO IS NULL) OR ID_UTENTE_COINVOLTO =" + idUtenteTitolare + ")");
                        query.setParam("idRuoloTitolare", string.Empty);
                    }
                    else
                    {
                        query.setParam("idRuoloTitolare", string.IsNullOrEmpty(idRuoloTitolare) ? string.Empty : " AND ID_RUOLO_COINVOLTO = " + idRuoloTitolare);
                        query.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ID_UTENTE_COINVOLTO = " + idUtenteTitolare);
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

        [ReportDataExtractorMethod(ContextName = "ExportIstanzeProcessiFirma")]
        public DataSet GetIstanzaProcessiDiFirmaByTitolare(InfoUtente infoUt, List<FiltroRicerca> filters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    Query query = InitQuery.getInstance().getQuery("S_EXPORT_DPA_ISTANZA_PROCESSO_FIRMA_BY_TITOLARE");

                    string idRuoloTitolare = filters.Where(f => f.argomento == "idRuolo").FirstOrDefault().valore;
                    string idUtenteTitolare = filters.Where(f => f.argomento == "idUtente").FirstOrDefault().valore;
                    if (string.IsNullOrEmpty(idRuoloTitolare))
                    {
                        string idRuoli = string.Empty;
                        LibroFirma libroFirma = new LibroFirma();
                        List<string> listIdRuoli = libroFirma.GetIdRuoliProcessiUltimoUtente(idUtenteTitolare);
                        foreach (string id in listIdRuoli)
                            idRuoli += string.IsNullOrEmpty(idRuoli) ? id : ", " + id;

                        query.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ((ID_RUOLO_COINVOLTO IN (" + idRuoli + ") AND ID_UTENTE_COINVOLTO IS NULL) OR ID_UTENTE_COINVOLTO =" + idUtenteTitolare + ")");
                        query.setParam("idRuoloTitolare", string.Empty);
                    }
                    else
                    {
                        query.setParam("idRuoloTitolare", string.IsNullOrEmpty(idRuoloTitolare) ? string.Empty : " AND ID_RUOLO_COINVOLTO = " + idRuoloTitolare);
                        query.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ID_UTENTE_COINVOLTO = " + idUtenteTitolare);
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
        [ReportDataExtractorMethod(ContextName = "ExportProcessiDiFirmaInvalidati")]
        public DataSet GetProcessiDiFirmaInvalidati(InfoUtente infoUt, List<FiltroRicerca> filters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    Query query = InitQuery.getInstance().getQuery("S_EXPORT_DPA_REPORT_PROCESSI_TICK");

                    string idReport = filters.Where(f => f.argomento == "idReport").FirstOrDefault().valore;
                    string idRuoloCreatore = filters.Where(f => f.argomento == "idRuoloCreatore").FirstOrDefault().valore;

                    query.setParam("idReport", idReport);
                    query.setParam("idRuoloCreatore", idRuoloCreatore);

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
}
