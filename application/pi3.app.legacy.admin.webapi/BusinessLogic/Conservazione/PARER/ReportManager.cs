// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Conservazione.PARER.Report;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Serilog;

namespace BusinessLogic.Conservazione.PARER
{
    public class ReportManager
    {
        private static ILogger logger = Log.ForContext(typeof(ReportManager));

        public static DocsPaVO.Conservazione.PARER.Report.ReportMonitoraggioPolicyResponse ReportMonitoraggioPolicy(DocsPaVO.Conservazione.PARER.Report.ReportMonitoraggioPolicyRequest request, ISpreadsheetService spreadsheetService, IFileConverterFactory fileConverterPDF, IReportGeneratorService reportGeneratorService)
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
                if (request.TipoDataEsecuzione == "S" && !string.IsNullOrEmpty(request.DataEsecuzioneFrom))
                {
                    rangeEstrazione = "Policy eseguite il giorno " + request.DataEsecuzioneFrom;
                }
                else if (request.TipoDataEsecuzione == "R")
                {
                    if (!string.IsNullOrEmpty(request.DataEsecuzioneFrom))
                    {
                        rangeEstrazione = "Policy eseguite dal giorno " + request.DataEsecuzioneFrom;
                        if (!string.IsNullOrEmpty(request.DataEsecuzioneTo))
                        {
                            rangeEstrazione = rangeEstrazione + " al giorno " + request.DataEsecuzioneTo;
                        }
                    }
                }
                else if (request.TipoDataEsecuzione == "M")
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

                doc = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(printRequest, spreadsheetService, fileConverterPDF, reportGeneratorService).Document;
                logger.Debug("Generato documento - dimensioni " + doc.content.Length);

                response.Document = doc;

            }
            catch (Exception ex)
            {
                logger.Debug(ex, "errore in ReportMonitoraggioPolicy");
            }

            logger.Debug("END");
            return response;
        }
    }
}
