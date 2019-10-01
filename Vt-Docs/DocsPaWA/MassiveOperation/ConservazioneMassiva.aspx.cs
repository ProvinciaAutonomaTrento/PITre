using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;
using System.Text;
using System.IO;

namespace DocsPAWA.MassiveOperation
{
    public partial class ConservazioneMassiva : MassivePage
    {

        protected override string PageName
        {
            get {
                return "Area di conservazione massiva";
            }
        }

        protected override bool IsFasc
        {
            get { return false; }
        }
        #region Event Handler

        protected override bool btnConferma_Click(object sender, EventArgs e)
        {
            // System id dei documenti selezionati
            List<String> objectsId;

            // Report da mostrare
            MassiveOperationReport report = new MassiveOperationReport();
            
            // Se la request contiene objType e tale parametro è valorizzato come D,
            // i system id gestiti da MassiveOperationUtils sono id di documenti
            // altrimenti sono system id di fascicoli ed in tal caso bisogna eseguire l'inserimento
            // dei documenti contenuti nei fascicoli
            if (!String.IsNullOrEmpty(Request["objType"]) &&
                Request["objType"] == "D")
                this.ExecuteInsertDocumentsInSA(
                    MassiveOperationUtils.GetSelectedItems(), report);
            else
                this.ExecuteInsertProjectsInSA(
                    MassiveOperationUtils.GetSelectedItems(), report);

            // Introduzione della riga di summary
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);
            this.generateReport(report,"Conservazione massiva");
            return true;
        }

        /// <summary>
        /// Questa funzione si occupa di avviare lo spostamento nell'area di conservazione
        /// </summary>
        /// <param name="documentsId">Lista dei system id dei documenti selezionati</param>
        /// <returns>Report dell'inserimento</returns>
        private void ExecuteInsertDocumentsInSA(List<MassiveOperationTarget> documentsId, MassiveOperationReport reportToCompile)
        {
            // Per ogni id di documento
            foreach (MassiveOperationTarget mot in documentsId)
            {
                try
                {
                    // Recupero della scheda documento
                    SchedaDocumento documentDetails = DocumentManager.getDettaglioDocumento(
                        this.Page,
                        mot.Id,
                        String.Empty);

                    // Se c'è la scheda documento, si procede con la verifica dei dati e con 
                    // l'eventuale inserimento nell'area di conservazione
                    if (documentDetails != null)
                        this.CheckDataValidityAndInsertInSA(documentDetails, reportToCompile);
                    else
                        // Altrimenti viene inserito un risultato negativo
                        reportToCompile.AddReportRow(
                            mot.Codice,
                            MassiveOperationReport.MassiveOperationResultEnum.KO,
                            "Non è stato possibile recuperare il dettaglio sul documento.");
                }
                catch (Exception e)
                {
                    reportToCompile.AddReportRow(
                        mot.Codice,
                        MassiveOperationReport.MassiveOperationResultEnum.KO,
                        "Errore durante il recupero dei dettagli sul documento.");
                }

            }

        }

        /// <summary>
        /// Funzione per l'inserimento di fascicoli nell'area di conservazione
        /// </summary>
        /// <param name="idProjects">ID dei fascicoli da inserire in area di conservazione</param>
        /// <param name="reportToCompile">Report da compilare</param>
        /// <returns>Report di inserimento</returns>
        public void ExecuteInsertProjectsInSA(List<MassiveOperationTarget> idProjects, MassiveOperationReport reportToCompile)
        {
            string idIstanza= DocumentManager.getPrimaIstanzaAreaCons(this,UserManager.getInfoUtente(Page).idPeople,
                UserManager.getInfoUtente(Page).idGruppo);
            ItemsConservazione[] itemsCons = null;
            if (!string.IsNullOrEmpty(idIstanza))
            {
                itemsCons = DocumentManager.getItemsConservazioneLite(idIstanza, this, UserManager.getInfoUtente());
            }
            // Per ogni id di fascicolo
            foreach (MassiveOperationTarget mot in idProjects)
            {
                // Recupero dei system id dei documenti presenti 
                // all'interno del fascicolo
                // SearchResultInfo[] idDocuments = FascicoliManager.getDocumentiFromFascicolo(mot.Id);
                // Lista dei system id dei documenti contenuti nel fasciolo
                string[] idDocuments = FascicoliManager.getIdDocumentiFromFascicolo(mot.Id);
                List<MassiveOperationTarget> temp = new List<MassiveOperationTarget>();
                for (int i = 0; i < idDocuments.Length; i++)
                {
                    temp.Add(new MassiveOperationTarget(idDocuments[i], idDocuments[i]));
                    // Inserimento dei documenti in area di conservazione
                    // Identificativo dell'istanza di certificazione
                    String conservationAreaId;
                    // Se il documento ha un file acquisito
                    SchedaDocumento documentDetails = DocumentManager.getDettaglioDocumento(this.Page, idDocuments[i], idDocuments[i]);
                    if (Convert.ToInt32(documentDetails.documenti[0].fileSize) > 0)
                    {
                        // Inserimento del documento in area di conservazione                        
                        //Inserimento del metodo di controllo della presenza di un documento fascicolato nell'istanza
                        if (this.CheckProjectAndInsertInSA(documentDetails.systemId, mot.Id, itemsCons))
                        {
                            this.InsertInSA2(documentDetails, reportToCompile, mot.Id);
                        }
                        else
                        {
                            string codice = documentDetails.systemId;
                            // Altrimenti viene aggiunto un risultato negativo
                            reportToCompile.AddReportRow(
                                codice,
                                MassiveOperationReport.MassiveOperationResultEnum.KO,
                                "Il documento interno al fascicolo è già presente in una nuova istanza in area conservazione, impossibile inserirlo nuovamente");
                        }
                    }

                    else
                    {
                        string codice = documentDetails.systemId;
                        // Altrimenti viene aggiunto un risultato negativo
                        reportToCompile.AddReportRow(
                            codice,
                            MassiveOperationReport.MassiveOperationResultEnum.KO,
                            "Il documento principale non ha alcun file associato, impossibile inserirlo in area conservazione");
                    }
                }


            }

        }

        /// <summary>
        /// Metodo per il controllo della presenza di un documento fascicolato.
        /// </summary>
        /// <param name="documentDetails"></param>
        /// <param name="report"></param>
        /// <param name="idProject"></param>
        /// <param name="itemsCons"></param>
        private bool CheckProjectAndInsertInSA(string idDocument, string idProject, ItemsConservazione[] itemsCons)
        {
            bool retval = true;
            if (itemsCons != null)
            {                
                foreach (ItemsConservazione item in itemsCons)
                {
                    if (idDocument == item.DocNumber && idProject == item.ID_Project)
                    {
                        retval = false ;
                    }
                    
                }
                               
            }
            return retval;

        }


        /// <summary>
        /// Funzione per la validazione dei dati sul documento e l'avvio dell'inserimento
        /// nell'area di conservazione
        /// </summary>
        /// <param name="documentDetails">Dettagli del documento da inserire nell'area di conservazione</param>
        /// <param name="report">Report dello spostamento</param>
        private void CheckDataValidityAndInsertInSA(SchedaDocumento documentDetails, MassiveOperationReport report)
        {
            // Identificativo dell'istanza di certificazione
            String conservationAreaId;
            // Se il documento ha un file acquisito
            if (Convert.ToInt32(documentDetails.documenti[0].fileSize) > 0 && (string.IsNullOrEmpty(documentDetails.inConservazione) || !(documentDetails.inConservazione).Equals("1")))
                // Inserimento del documento in area di conservazione
                this.InsertInSA(documentDetails, report);

            else
            {
                string codice = MassiveOperationUtils.getItem(documentDetails.systemId).Codice;
                if (!string.IsNullOrEmpty(documentDetails.inConservazione) && (documentDetails.inConservazione).Equals("1"))
                {
                    report.AddReportRow(
                       codice,
                       MassiveOperationReport.MassiveOperationResultEnum.KO,
                       "Il documento risulta già inserito in una nuova istanza di conservazione, impossibile inserirlo nuovamente");
                }
                else
                {
                    report.AddReportRow(
                        codice,
                        MassiveOperationReport.MassiveOperationResultEnum.KO,
                        "Il documento principale non ha alcun file associato, impossibile inserirlo in area conservazione");
                }
            }
        }

        /// <summary>
        /// Funzione per l'inserimento di un documento nell'area di conservazione
        /// </summary>
        /// <param name="documentDetails">Dettagli del documento da inserire</param>
        /// <param name="report">Report di inserimento</param>
        private void InsertInSA(SchedaDocumento documentDetails, MassiveOperationReport report)
        {
            StringBuilder message = new StringBuilder();
            String conservationAreaId;
            string id = MassiveOperationUtils.getItem(documentDetails.systemId).Codice;
            // Se si sta creando la prima istanza di conservazione...
            if (DocumentManager.isPrimaConservazione(
                this,
                UserManager.getInfoUtente(Page).idPeople,
                UserManager.getInfoUtente(Page).idGruppo) == 1)
                message.Append("Si sta creando la prima istanza di conservazione. ");

            // Recupero dell'eventiale fascicolo selezionato
            Fascicolo selectedProject = FascicoliManager.getFascicoloSelezionato();

            // Inserimento del documento nella conservazione e recupero dell'id dell'istanza
            if (selectedProject != null)
                // Da ricerca documenti in fascicolo
                conservationAreaId = DocumentManager.addAreaConservazioneAM(
                    this.Page,
                    documentDetails.systemId,
                    selectedProject.systemID,
                    documentDetails.docNumber,
                    UserManager.getInfoUtente(Page),
                    "F");
            else
                conservationAreaId = DocumentManager.addAreaConservazioneAM(
                    this.Page,
                    documentDetails.systemId,
                    "",
                    documentDetails.docNumber,
                    UserManager.getInfoUtente(Page),
                    "D");

            try
            {
                int size_xml = DocumentManager.getItemSize(
                    this.Page,
                    documentDetails,
                    conservationAreaId);

                int doc_size = Convert.ToInt32(documentDetails.documenti[0].fileSize);

                int numeroAllegati = documentDetails.allegati.Length;
                string fileName = documentDetails.documenti[0].fileName;
                string tipoFile = Path.GetExtension(fileName);
                int size_allegati = 0;
                for (int i = 0; i < documentDetails.allegati.Length; i++)
                {
                    size_allegati = size_allegati + Convert.ToInt32(documentDetails.allegati[i].fileSize);
                }
                int total_size = size_allegati + doc_size + size_xml;

                DocumentManager.insertSizeInItemCons(Page, conservationAreaId, total_size);

                DocumentManager.updateItemsConservazione(
                    this.Page,
                    tipoFile,
                    Convert.ToString(numeroAllegati),
                    conservationAreaId);
            }
            catch (Exception e)
            {
                report.AddReportRow(
                    id,
                    MassiveOperationReport.MassiveOperationResultEnum.KO,
                    "Si è verificato un errore durante l'inserimento del documento nell'area di conservazione");
            }

            report.AddReportRow(
                id,
                MassiveOperationReport.MassiveOperationResultEnum.OK,
                "Documento inserito correttamente in area di conservazione.");

        }

        /// <summary>
        /// Funzione per l'inserimento di un documento nell'area di conservazione
        /// </summary>
        /// <param name="documentDetails">Dettagli del documento da inserire</param>
        /// <param name="report">Report di inserimento</param>
        private void InsertInSA2(SchedaDocumento documentDetails, MassiveOperationReport report, string idProject)
        {
            StringBuilder message = new StringBuilder();
            String conservationAreaId;
            //string id = MassiveOperationUtils.getItem(documentDetails.systemId).Codice;
            // Se si sta creando la prima istanza di conservazione...
            if (DocumentManager.isPrimaConservazione(
                this,
                UserManager.getInfoUtente(Page).idPeople,
                UserManager.getInfoUtente(Page).idGruppo) == 1)
                message.Append("Si sta creando la prima istanza di conservazione. ");

            // Recupero dell'eventiale fascicolo selezionato
            Fascicolo selectedProject = FascicoliManager.getFascicoloSelezionato();

            // Inserimento del documento nella conservazione e recupero dell'id dell'istanza
            if (!string.IsNullOrEmpty(idProject))
                // Da ricerca documenti in fascicolo
                conservationAreaId = DocumentManager.addAreaConservazioneAM(
                    this.Page,
                    documentDetails.systemId,
                    idProject,
                    documentDetails.docNumber,
                    UserManager.getInfoUtente(Page),
                    "F");
            else
                conservationAreaId = DocumentManager.addAreaConservazioneAM(
                    this.Page,
                    documentDetails.systemId,
                    "",
                    documentDetails.docNumber,
                    UserManager.getInfoUtente(Page),
                    "D");

            try
            {
                int size_xml = DocumentManager.getItemSize(
                    this.Page,
                    documentDetails,
                    conservationAreaId);

                int doc_size = Convert.ToInt32(documentDetails.documenti[0].fileSize);

                int numeroAllegati = documentDetails.allegati.Length;
                string fileName = documentDetails.documenti[0].fileName;
                string tipoFile = Path.GetExtension(fileName);
                int size_allegati = 0;
                for (int i = 0; i < documentDetails.allegati.Length; i++)
                {
                    size_allegati = size_allegati + Convert.ToInt32(documentDetails.allegati[i].fileSize);
                }
                int total_size = size_allegati + doc_size + size_xml;

                DocumentManager.insertSizeInItemCons(Page, conservationAreaId, total_size);

                DocumentManager.updateItemsConservazione(
                    this.Page,
                    tipoFile,
                    Convert.ToString(numeroAllegati),
                    conservationAreaId);
            }
            catch (Exception e)
            {
                report.AddReportRow(
                    documentDetails.systemId,
                    MassiveOperationReport.MassiveOperationResultEnum.KO,
                    "Si è verificato un errore durante l'inserimento del documento nell'area di conservazione");
            }

            report.AddReportRow(
                documentDetails.systemId,
                MassiveOperationReport.MassiveOperationResultEnum.OK,
                "Documento inserito correttamente in area di conservazione.");

        }

        #endregion
    }
}
