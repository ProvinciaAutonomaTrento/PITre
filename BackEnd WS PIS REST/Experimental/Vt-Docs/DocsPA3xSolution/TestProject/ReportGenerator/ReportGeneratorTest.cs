using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject.DocsPaWS;
using System.IO;

namespace TestProject.ReportGenerator
{
    [TestClass]
    public class ReportGeneratorTest
    {
        /// <summary>
        /// Salva un file Excel con tutti i modelli di trasmissione legati
        /// al registro PAT ed aventi un codice che contiene 384 in C:\Report.xls
        /// </summary>
        [TestMethod]
        public void GenerateExcelFile()
        {

            DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService();
            ws.Timeout = System.Threading.Timeout.Infinite;

            PrintReportRequest request = new PrintReportRequest()
            {
                ContextName = "RicercaModelliTrasmissione",
                ReportType = ReportTypeEnum.Excel,
                SearchFilters = new FiltroRicerca[] {
                    new FiltroRicerca()
                        {
                            argomento = FiltriModelliTrasmissione.ID_REGISTRO.ToString(),
                            valore = "86107"
                        },
                    new FiltroRicerca()
                        {
                            argomento = FiltriModelliTrasmissione.CODICE_MODELLO.ToString(),
                            valore = "384"
                        }
                },
                SubTitle = "Sotto titolo",
                Title = "Titolo",
                UserInfo = new InfoUtente()
                    {
                        idAmministrazione = "361"
                    }
            };


            //PrintReportResponse response = ws.GenerateReportModelliTrasmissione(request);
            PrintReportResponse response = new PrintReportResponse();

            File.WriteAllBytes(@"C:\Report.xml", response.Document.content);

            Assert.IsNotNull(response);

        }
    }
}
