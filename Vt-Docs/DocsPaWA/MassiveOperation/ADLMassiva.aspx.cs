using System;
using System.Collections.Generic;
using System.Linq;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;

namespace DocsPAWA.MassiveOperation
{
    public partial class ADLMassiva : MassivePage
    {
        protected override string PageName
        {
            get
            {
                return "Sposta in area di lavoro massivo";
            }
        }

        protected override bool IsFasc
        {
            get { return !Request["objType"].Equals("D"); }
        }

        #region Event Handler

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
                this.MoveDocumentsInADL(MassiveOperationUtils.GetSelectedItems(), report);
            else
                this.MoveProjectsInADL(MassiveOperationUtils.GetSelectedItems(), report);

            // Introduzione della riga di summary
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            if (Request["objType"].Equals("D"))
                report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);
            else
                report.AddSummaryRow("Fascicoli lavorati: {0} - Fascicoli non lavorati: {1}", pars);

            this.generateReport(report, "Spostamento in ADL massivo");
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
        private void MoveDocumentsInADL(List<MassiveOperationTarget> documentSystemId, MassiveOperationReport report)
        {
            // Lista delle informazioni sui documenti
            List<InfoDocumento> documentsInfo = null;

            // Recupero delle informazioni sui documenti da aggiungere in ADL
            documentsInfo = this.LoadDocumentInformation(documentSystemId, report);

            // Salvataggio dei documenti nell'area di lavoro
            this.MoveDocumentsInWorkingArea(documentsInfo, report);

        }

        /// <summary>
        /// Funzione per il caricamento delle informaioni sui documenti
        /// </summary>
        /// <param name="documentSystemId">Lista dei system id dei documenti di cui recuperare le informazioni</param>
        /// <param name="report">Report dell'esecuzione</param>
        /// <returns>Lista delle informazioni sui documenti</returns>
        private List<InfoDocumento> LoadDocumentInformation(List<MassiveOperationTarget> documentSystemId, MassiveOperationReport report)
        {
            // L'oggetto da restituire
            List<InfoDocumento> toReturn = new List<InfoDocumento>();
            // Oggetto con le informazioni di base sul documento
            BaseInfoDoc baseInfoDoc;

            // Recupero delle informazioni sui documenti
            foreach (MassiveOperationTarget mot in documentSystemId)
            {
                try
                {
                    // Recupero delle informazioni di base sul documento
                    baseInfoDoc = DocumentManager.GetBaseInfoForDocument(
                        mot.Id,
                        String.Empty,
                        String.Empty).Where(e => e.IdProfile.Equals(mot.Id)).FirstOrDefault();

                    // Recuper dell'oggetto info documento
                    toReturn.Add(DocumentManager.GetInfoDocumento(
                        baseInfoDoc.IdProfile,
                        baseInfoDoc.DocNumber,
                        this));

                }
                catch (Exception e)
                {
                    report.AddReportRow(
                        mot.Codice,
                        MassiveOperationReport.MassiveOperationResultEnum.KO,
                        "Errore durante il recupero delle informazioni sul documento.");
                }

            }

            // Restiutuzione della lista delle informazioni sui documento da spostare nell'ADL
            return toReturn;

        }

        /// <summary>
        /// Funzione per lo spostamento dei documenti nell'area di lavoro
        /// </summary>
        /// <param name="documentsInfo">L'elenco dei documenti da spostare</param>
        /// <param name="report">Report del''esecuzione</param>
        private void MoveDocumentsInWorkingArea(List<InfoDocumento> documentsInfo, MassiveOperationReport report)
        {
            int isInADL;
            // Per ogni documento...
           foreach (InfoDocumento docInfo in documentsInfo)
            {
                string id = MassiveOperationUtils.getItem(docInfo.idProfile).Codice;
                // ...spostamento del documento nell'area di lavoro
                isInADL = DocumentManager.isDocInADL(docInfo.idProfile, this.Page);
                try
                {
                    if (isInADL == 0)
                    {
                        DocumentManager.addAreaLavoro(this, docInfo);
                        report.AddReportRow(
                            id,
                            MassiveOperationReport.MassiveOperationResultEnum.OK,
                            "Documento inserito correttamente nell'area di lavoro.");
                    }
                    else
                    {
                        report.AddReportRow(
                            id,
                            MassiveOperationReport.MassiveOperationResultEnum.KO,
                            "Documento già inserito nell'area di lavoro.");
                    }
                }
                catch (Exception e)
                {
                    report.AddReportRow(
                        id,
                        MassiveOperationReport.MassiveOperationResultEnum.KO,
                        "Errore durante lo spostamento del documento nell'area di lavoro. Dettagli: " + e.Message);
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
        private void MoveProjectsInADL(List<MassiveOperationTarget> projectsId, MassiveOperationReport report)
        {
            // Lista delle informazioni sui documenti
            List<Fascicolo> projectsInfo = null;

            // Recupero delle informazioni sui fascicoli da aggiungere in ADL
            projectsInfo = this.LoadProjectInformation(projectsId, report);

            // Salvataggio dei fascicoli nell'area di lavoro
            this.MoveDocumentsInWorkingArea(projectsInfo, report);

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
        /// Funzione per lo spostamento di fascicoli nell'area di lavoro
        /// </summary>
        /// <param name="projectsInformation">Lista dei fascicoli da spostare</param>
        /// <param name="report">Report dell'esecuzione</param>
        private void MoveDocumentsInWorkingArea(List<Fascicolo> projectsInformation, MassiveOperationReport report)
        {
            // Per ogni fascicolo...
            foreach(Fascicolo prj in projectsInformation)
                try
                {
                    if (!prj.InAreaLavoro.Equals("1"))
                    {
                        // ...spostamento del fascicolo nell'area di lavoro
                        FascicoliManager.addFascicoloInAreaDiLavoro(
                            this,
                            prj);

                        // ...aggiunta di un risultato positivo
                        report.AddReportRow(
                            prj.codice,
                            MassiveOperationReport.MassiveOperationResultEnum.OK,
                            "Fascicolo inserito correttamento nell'area di lavoro.");
                    }
                    else
                    {
                        report.AddReportRow(
                            prj.codice,
                            MassiveOperationReport.MassiveOperationResultEnum.KO,
                            "Fascicolo già inserito nell'area di lavoro.");
                    }
                }
                catch (Exception e)
                {
                    report.AddReportRow(
                        prj.codice,
                        MassiveOperationReport.MassiveOperationResultEnum.KO,
                        "Errore durante lo spostamento del fascicolo nell'area di lavoro.");
 
                }

        }

        #endregion

        #endregion

    }

}
