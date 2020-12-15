using System;
using System.Collections.Generic;
using System.Linq;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;

namespace DocsPAWA.MassiveOperation
{
    public partial class RemoveADLMassiva : MassivePage
    {
        #region Event Handler

        protected override string PageName
        {
            get {

                return "Rimuovi dall'area di lavoro massivo";
            }
        }

        protected override bool IsFasc
        {
            get {
                return !"D".Equals(Request["objType"]);
            }
        }

        /// <summary>
        /// Al click sul pulsante di conferma, viene avviata la procedura per lo spostamento massivo
        /// di documenti nell'Area di Lavoro
        /// </summary>
        protected override bool btnConferma_Click(object sender, EventArgs e)
        {
            // Il report da visualizzare
            MassiveOperationReport report;

            // Inizializzazione del report
            report = new MassiveOperationReport();

            // Selezione della procedura da seguire in base al
            // tipo di oggetto
            if (Request["objType"].Equals("D"))
                this.RemoveDocumentsFromADL(MassiveOperationUtils.GetSelectedItems(), report);
            else
                this.RemoveProjectsFromADL(MassiveOperationUtils.GetSelectedItems(), report);

            // Introduzione della riga di summary
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            if (Request["objType"].Equals("D"))
                report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);
            else
                report.AddSummaryRow("Fascicoli lavorati: {0} - Fascicoli non lavorati: {1}", pars);

            // Generazione del report
            this.generateReport(report,"Rimozione da ADL massiva");
            return true;
        }

        #endregion

        #region Funzioni di utilità

        #region Documenti

        /// <summary>
        /// Funzione per lo spostamento dei documenti selezionati nell'area di lavoro
        /// </summary>
        /// <param name="documentSystemId">Lista dei system id dei documenti da spostare</param>
        /// <param name="report">Il report dell'esecuzione</param>
        private void RemoveDocumentsFromADL(List<MassiveOperationTarget> documentSystemId, MassiveOperationReport report)
        {
            // Salvataggio dei documenti nell'area di lavoro
            this.RemoveDocumentsFromWorkingArea(documentSystemId, report);
        }


        /// <summary>
        /// Funzione per lo spostamento dei documenti nell'area di lavoro
        /// </summary>
        /// <param name="documentsInfo">L'elenco dei documenti da spostare</param>
        /// <param name="report">Report del''esecuzione</param>
        private void RemoveDocumentsFromWorkingArea(List<MassiveOperationTarget> documentsIds, MassiveOperationReport report)
        {
            int isInADL;
            // Per ogni documento...
            foreach (MassiveOperationTarget mot in documentsIds)
            {
                isInADL = DocumentManager.isDocInADL(mot.Id, this.Page);
                // ...spostamento del documento nell'area di lavoro
                try
                {
                    if (isInADL == 1)
                    {
                        DocumentManager.eliminaDaAreaLavoro(this, mot.Id, null);
                        report.AddReportRow(
                            mot.Codice,
                            MassiveOperationReport.MassiveOperationResultEnum.OK,
                            "Documento rimosso correttamente dall'area di lavoro.");
                    }
                    else
                    {
                        report.AddReportRow(
                           mot.Id,
                           MassiveOperationReport.MassiveOperationResultEnum.KO,
                           "Documento già rimosso dall'area di lavoro.");
                    }
                }
                catch (Exception e)
                {
                    report.AddReportRow(
                        mot.Codice,
                        MassiveOperationReport.MassiveOperationResultEnum.KO,
                        "Errore durante la rimozione del documento dall'area di lavoro. Dettagli: " + e.Message);
                }
            }

        }

        #endregion

        #region Fascicolo

        /// <summary>
        /// Funzione per lo spostamento di fascicoli nell'area di lavoro
        /// </summary>
        /// <param name="projectsId">Lista dei system id dei fascicoli da spostare nell'area di lavoro</param>
        /// <param name="report">Report dell'elaborazione</param>
        private void RemoveProjectsFromADL(List<MassiveOperationTarget> projectsId, MassiveOperationReport report)
        {
            // Lista delle informazioni sui documenti
            List<Fascicolo> projectsInfo = null;

            // Recupero delle informazioni sui fascicoli da aggiungere in ADL
            projectsInfo = this.LoadProjectInformation(projectsId, report);

            // Salvataggio dei fascicoli nell'area di lavoro
            this.RemoveProjectsFromWorkingArea(projectsInfo, report);

        }

        /// <summary>
        /// Funzione per il recupero delle informazioni sui fascicoli da spostare in ADL
        /// </summary>
        /// <param name="projectsId">Lista degli identificativi dei fascicoli</param>
        /// <param name="report">Report dell'esecuzione</param>
        /// <returns>Lista delle informazioni sui fascicoli</returns>
        private List<Fascicolo> LoadProjectInformation(List<MassiveOperationTarget> projectsId, MassiveOperationReport report)
        {
            // Lista da restituire
            List<Fascicolo> toReturn = new List<Fascicolo>();

            // Per ogni fascicolo...
            foreach (MassiveOperationTarget mot in projectsId)
                try
                {
                    // ...aggiunta del fascicolo alla lista dei fascicoli
                    toReturn.Add(FascicoliManager.getFascicolo(
                        this,
                        mot.Id));
                }
                catch (Exception e)
                {
                    report.AddReportRow(
                        mot.Codice,
                        MassiveOperationReport.MassiveOperationResultEnum.KO,
                        "Errore durante il reperimento delle informazioni sul fascicolo.");
                }

            // Restituzione della lista con le informaizoni sui fascicoli
            return toReturn;
            
        }

        /// <summary>
        /// Funzione per la rimozione dei fascicoli dall'area di lavoro
        /// </summary>
        /// <param name="projectsInformation">Lista dei fascicoli da rimuovere</param>
        /// <param name="report">Report dell'esecuzione</param>
        private void RemoveProjectsFromWorkingArea(List<Fascicolo> projectsInformation, MassiveOperationReport report)
        {
            // Per ogni fascicolo...
            foreach (Fascicolo prj in projectsInformation)
            {
                try
                {
                    if (!prj.InAreaLavoro.Equals("0"))
                    {
                        // ...rimozione del fascicolo nell'area di lavoro
                        FascicoliManager.eliminaFascicoloDaAreaDiLavoro(
                            this,
                            prj);

                        // ...aggiunta di un risultato positivo
                        report.AddReportRow(
                            prj.codice,
                            MassiveOperationReport.MassiveOperationResultEnum.OK,
                            "Fascicolo rimosso correttamento dall'area di lavoro.");
                    }
                    else
                    {
                        report.AddReportRow(
                            prj.codice,
                            MassiveOperationReport.MassiveOperationResultEnum.KO,
                            "Fascicolo già rimosso dall'area di lavoro.");
                    }
                }
                catch (Exception e)
                {
                    report.AddReportRow(
                        prj.codice,
                        MassiveOperationReport.MassiveOperationResultEnum.KO,
                        "Errore durante la rimozione del fascicolo dall'area di lavoro. Dettagli: " + e.Message);

                }
            }

        }

        #endregion

        #endregion

    }

}