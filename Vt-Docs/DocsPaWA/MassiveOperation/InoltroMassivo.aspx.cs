using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.MassiveOperation
{
    public partial class InoltroMassivo : MassivePage
    {
        protected override string PageName
        {
            get { return "Inoltra massivo"; }
        }

        protected override bool IsFasc
        {
            get { return false; }
        }

        #region Event handler

        /// <summary>
        /// Al click sul pulsante bisogna creare il documento per l'inoltro massivo
        /// </summary>
        protected override bool btnConferma_Click(object sender, EventArgs e)
        {
            #region Dichiarazione variabili

            // Lista di system id degli elementi selezionati
            List<MassiveOperationTarget> selectedItem;

            // Errore verificatosi in fase di creazione della scheda
            String error = String.Empty;

            // La scheda documento creata
            SchedaDocumento document;

            #endregion

            // Recupero della lista dei system id dei documenti selezionati
            selectedItem = MassiveOperationUtils.GetSelectedItems();

            // Generazione della scheda
            MassiveOperationReport report = new MassiveOperationReport();
            try
            {
                document = this.GenerateDocumentScheda(selectedItem, out error);
                if (document != null)
                {
                    report.AddReportRow("-", MassiveOperationReport.MassiveOperationResultEnum.OK, "Il documento è stato creato correttamente");
                }
                else
                {
                    report.AddReportRow("-", MassiveOperationReport.MassiveOperationResultEnum.KO, "Il documento non è stato creato");
                }
            }
            catch (Exception ex)
            {
                report.AddReportRow("-", MassiveOperationReport.MassiveOperationResultEnum.KO, "Errore nella creazione del documento");
            }
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);
            this.generateReport(report, "Inoltra massivo");
            this.addJSOnChiudiButton("self.returnValue=true;self.close();");
            return true;
        }

        #endregion

        #region Funzioni di utilità

        /// <summary>
        /// Funzione per a creazione della scheda
        /// </summary>
        /// <param name="selectedItem">Lista dei system id dei documenti selezionati</param>
        /// <param name="error"></param>
        /// <returns>La scheda documento creata</returns>
        private SchedaDocumento GenerateDocumentScheda(List<MassiveOperationTarget> selectedItem, out String error)
        {
            SchedaDocumento document = null;
            List<string> ids = new List<string>();
            foreach (MassiveOperationTarget temp in selectedItem) ids.Add(temp.Id);
            document = DocumentManager.GetSchedaDocumentoInoltroMassivo(
                ids,
                UserManager.getInfoUtente(this),
                UserManager.getRuolo(this),
                out error);

            DocumentManager.setDocumentoSelezionato(this, document);

            return document;


        }

        #endregion
    }
}