using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Conservazione.PARER.Report;
using log4net;

namespace BusinessLogic.Conservazione.PARER
{
    public class ReportManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(ReportManager));

        public static ReportSingolaAmmResponse GetDataReportSingolaAmm(ReportSingolaAmmRequest request)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();

            ReportSingolaAmmResponse response = new ReportSingolaAmmResponse();

            string idRespCons = cons.GetIdUtenteResponsabileConservazione(request.IdAmm);

            if (!string.IsNullOrEmpty(idRespCons))
            {
                response = cons.GetDataReportSingolaAmm(request);

                if (response != null)
                {
                    DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
                    response.MailResponsabile = u.GetEmailUtente(idRespCons);
                }
            }

            return response;
        }

        #region Report policy
        public static DocsPaVO.Conservazione.PARER.Report.ReportMonitoraggioPolicyResponse ReportMonitoraggioPolicy(DocsPaVO.Conservazione.PARER.Report.ReportMonitoraggioPolicyRequest request)
        {
            ReportMonitoraggioPolicyResponse response = new ReportMonitoraggioPolicyResponse();
            logger.Debug("BEGIN");

            try
            {
                DocsPaVO.documento.FileDocumento doc = new DocsPaVO.documento.FileDocumento();

                List<DocsPaVO.filtri.FiltroRicerca> filters = new List<DocsPaVO.filtri.FiltroRicerca>();
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "id_amm", valore = request.IdAmm });
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "codice_policy", valore = request.Codice });
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "descrizione_policy", valore = request.Descrizione });
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "data_esecuzione_tipo", valore = request.TipoDataEsecuzione });
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "data_esecuzione_da", valore = request.DataEsecuzioneFrom });
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "data_esecuzione_a", valore = request.DataEsecuzioneTo });

                string rangeEstrazione = string.Empty;
                if(request.TipoDataEsecuzione == "S" && !string.IsNullOrEmpty(request.DataEsecuzioneFrom))
                {
                    rangeEstrazione = "Policy eseguite il giorno " + request.DataEsecuzioneFrom;
                }
                else if(request.TipoDataEsecuzione == "R")
                {
                    if(!string.IsNullOrEmpty(request.DataEsecuzioneFrom))
                    {
                        rangeEstrazione = "Policy eseguite dal giorno " + request.DataEsecuzioneFrom;
                        if(!string.IsNullOrEmpty(request.DataEsecuzioneTo))
                        {
                            rangeEstrazione = rangeEstrazione + " al giorno " + request.DataEsecuzioneTo;
                        }
                    }
                }
                else if(request.TipoDataEsecuzione == "M")
                {
                    rangeEstrazione = "Policy eseguite nel mese corrente";
                }

                DocsPaVO.Report.PrintReportRequest printRequest = new DocsPaVO.Report.PrintReportRequest();
                printRequest.ContextName = "AmmMonitoraggioPolicy";
                printRequest.ReportKey = "AmmMonitoraggioPolicy";
                printRequest.Title = "Report monitoraggio policy";
                printRequest.SubTitle = rangeEstrazione;
                printRequest.SearchFilters = filters;
                printRequest.ReportType = DocsPaVO.Report.ReportTypeEnum.Excel;

                doc = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(printRequest).Document;
                logger.Debug("Generato documento - dimensioni " + doc.content.Length);

                response.Document = doc;

            }
            catch(Exception ex)
            {
                logger.Debug(ex);
            }

            logger.Debug("END");
            return response;
        }
        #endregion
    }
}
