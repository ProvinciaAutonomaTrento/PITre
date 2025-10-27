// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Conservazione.PARER;
using DocsPaVO.documento;
using DocsPaVO.utente;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Conservazione.PARER
{
    public class PolicyPARERManager
    {
        public static ArrayList getListaPolicy(string idAmm, string tipo)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetListaPolicyPARER(idAmm, tipo);
        }

        public static bool InsertNewPolicy(DocsPaVO.Conservazione.PARER.PolicyPARER policy)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.InsertNewPolicyPARER(policy);
        }

        public static bool UpdatePolicy(DocsPaVO.Conservazione.PARER.PolicyPARER policy)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.UpdatePolicyPARER(policy);
        }

        public static string GetCountDocumentiFromPolicy(DocsPaVO.Conservazione.PARER.PolicyPARER policy, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetCountDocumentiFromPolicy(policy, idAmm);
        }

        public static DocsPaVO.Conservazione.PARER.PolicyPARER GetPolicyById(string idPolicy)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetPolicyPARERById(idPolicy);
        }

        public static DocsPaVO.Conservazione.PARER.EsecuzionePolicy GetInfoEsecuzionePolicy(string idPolicy, string tipo)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetInfoEsecuzionePolicy(idPolicy, tipo);
        }

        public static bool DeletePolicy(string idPolicy)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.DeletePolicyPARER(idPolicy);
        }

        public static bool UpdateStatoPolicy(ArrayList lista, InfoUtente utente)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.UpdateStatoPolicy(lista, utente);
        }

        public static DocsPaVO.documento.FileDocumento ExportPolicy(string[] id, string formato, string tipo, ISpreadsheetService spreadsheetService, IFileConverterFactory fileConverterPDF, IReportGeneratorService reportGeneratorService)
        {
            DocsPaVO.documento.FileDocumento doc = new FileDocumento();

            // costruisco la lista di ID da passare al generatore
            string listaID = string.Empty;
            for (int i = 0; i < id.Length; i++)
            {
                listaID = listaID + id[i];
                if (i < id.Length - 1)
                    listaID = listaID + ",";
            }

            // preparazione filtri request
            List<DocsPaVO.filtri.FiltroRicerca> filters = new List<DocsPaVO.filtri.FiltroRicerca>();
            filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idPolicy", valore = listaID });
            filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "type", valore = tipo });

            DocsPaVO.Report.PrintReportRequest request = new DocsPaVO.Report.PrintReportRequest();
            request.ContextName = "AmmExportPolicyPARER";
            request.ReportKey = "AmmExportPolicyPARER";
            request.Title = "Dettaglio Policy";
            request.SubTitle = string.Empty;
            request.AdditionalInformation = string.Empty;
            request.SearchFilters = filters;

            switch (formato)
            {
                case "XLS":
                    request.ReportType = DocsPaVO.Report.ReportTypeEnum.Excel;
                    break;

                case "ODS":
                    request.ReportType = DocsPaVO.Report.ReportTypeEnum.ODS;
                    break;
            }
            doc = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request, spreadsheetService, fileConverterPDF, reportGeneratorService).Document;

            if (doc != null)
            {
                if (tipo.Equals("DOC"))
                    doc.name = string.Format("Report_Policy_Documenti_{0}.xls", DateTime.Now.ToString("dd-MM-yyyy"));
                else if (tipo.Equals("FASC"))
                    doc.name = string.Format("Report_Policy_Fascicoli_{0}.xls", DateTime.Now.ToString("dd-MM-yyyy"));
                else
                    doc.name = string.Format("Report_Policy_Stampe_{0}.xls", DateTime.Now.ToString("dd-MM-yyyy"));
            }

            return doc;
        }
        public static DocsPaVO.Conservazione.PARER.PolicyFascicoliPARER GetPolicyFascicoliById(string idPolicy)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetPolicyFascicoliPARERById(idPolicy);
        }

        public static bool DeletePolicyFascicoli(PolicyFascicoliPARER policy)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.DeletePolicyFascicoliPARER(policy);
        }

        public static bool UpdateStatoPolicyFascicoli(List<DocsPaVO.Conservazione.PARER.PolicyFascicoliPARER> lista, InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.UpdateStatoPolicyFascicoli(lista, infoUtente);
        }

    }
}
