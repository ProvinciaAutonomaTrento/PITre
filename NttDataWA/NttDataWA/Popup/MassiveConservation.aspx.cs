using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;


namespace NttDataWA.Popup
{
    public partial class MassiveConservation : System.Web.UI.Page
    {

        #region Properties

        private bool IsFasc
        {
            get
            {
                return Request.QueryString["objType"].Equals("D") ? false : true;
            }
        }

        protected Dictionary<String, String> ListCheck
        {
            get
            {
                Dictionary<String, String> result = null;
                if (HttpContext.Current.Session["listCheck"] != null)
                {
                    result = HttpContext.Current.Session["listCheck"] as Dictionary<String, String>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listCheck"] = value;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
            }
            else
            {
                this.ReadRetValueFromPopup();
            }
            this.RefreshScript();
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.CloseMask(true);
        }

        protected void BtnConfirm_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            // Il report da visualizzare
            MassiveOperationReport report;

            // Inizializzazione del report
            report = new MassiveOperationReport();

            // Selezione della procedura da seguire in base al
            // tipo di oggetto
            if (!this.IsFasc)
            {
                this.ExecuteInsertDocumentsInSA(MassiveOperationUtils.GetSelectedItems(), report);
            }
            else
            {
                this.ExecuteInsertProjectsInSA(MassiveOperationUtils.GetSelectedItems(), report);
            }

            // Introduzione della riga di summary
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);

            this.generateReport(report, "Conservazione massiva");
        }

        protected void BtnReport_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "reallowOp", "reallowOp();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "visualReport", "parent.ajaxModalPopupMassiveReport();", true);
        }

        #endregion

        #region Methods

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.MassiveReport.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveReport','');", true);
            }
        }

        protected void InitializePage()
        {
            this.InitializeLanguage();
            this.InitializeList();

            this.BtnReport.Visible = false;
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnConfirm.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnConfirm", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnClose", language);
            this.BtnReport.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.MassiveReport.Title = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.litMessage.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserAskConfirm", language);
            this.grdReport.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserGridResult", language);
            this.grdReport.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserGridDetails", language);
        }

        public void InitializeList()
        {
            Dictionary<String, MassiveOperationTarget> temp = new Dictionary<string, MassiveOperationTarget>();

            // Inizializzazione della mappa con i system id degli oggetti e lo stato
            // di checking (in fase di inizializzazione tutti gli item sono deselezionati)
            foreach (KeyValuePair<string, string> item in this.ListCheck)
                if (!temp.Keys.Contains(item.Key))
                    temp.Add(item.Key, new MassiveOperationTarget(item.Key, item.Value));

            // Salvataggio del dizionario
            MassiveOperationUtils.ItemsStatus = temp;
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        private void CloseMask(bool withReturnValue)
        {
            string retValue = withReturnValue ? "true" : "false";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('MassiveConservation', '" + retValue + "');", true);
        }

        /// <summary>
        /// Funzione per l'inserimento di fascicoli nell'area di conservazione
        /// </summary>
        /// <param name="idProjects">ID dei fascicoli da inserire in area di conservazione</param>
        /// <param name="reportToCompile">Report da compilare</param>
        /// <returns>Report di inserimento</returns>
        public void ExecuteInsertProjectsInSA(List<MassiveOperationTarget> idProjects, MassiveOperationReport reportToCompile)
        {
            string idIstanza = DocumentManager.getPrimaIstanzaAreaCons(this, UserManager.GetInfoUser().idPeople,
                UserManager.GetInfoUser().idGruppo);
            ItemsConservazione[] itemsCons = null;
            if (!string.IsNullOrEmpty(idIstanza))
            {
                itemsCons = DocumentManager.getItemsConservazioneLite(idIstanza, this, UserManager.GetInfoUser());
            }
            // Per ogni id di fascicolo
            foreach (MassiveOperationTarget mot in idProjects)
            {
                // Recupero dei system id dei documenti presenti 
                // all'interno del fascicolo
                // SearchResultInfo[] idDocuments = FascicoliManager.getDocumentiFromFascicolo(mot.Id);
                // Lista dei system id dei documenti contenuti nel fasciolo
                string[] idDocuments = ProjectManager.getIdDocumentiFromFascicolo(mot.Id);
                List<MassiveOperationTarget> temp = new List<MassiveOperationTarget>();
                #region MEV CS 1.5 - F03_01
                // Indice dell'elemento lavorato
                int indexOfCurrentDoc = 0;

                // Dimensione complessiva raggiunta
                int currentDimOfDocs = 0;

                // Get valori limiti per le istanze di conservazione
                int DimMaxInIstanza = 0;
                int numMaxDocInIstanza = 0;
                int TolleranzaPercentuale = 0;
                try
                {
                    InfoUtente infoUt = UserManager.GetInfoUser();
                    DimMaxInIstanza = DocumentManager.getDimensioneMassimaIstanze(infoUt.idAmministrazione);
                    numMaxDocInIstanza = DocumentManager.getNumeroDocMassimoIstanze(infoUt.idAmministrazione);
                    TolleranzaPercentuale = DocumentManager.getPercentualeTolleranzaDinesioneIstanze(infoUt.idAmministrazione);
                }
                catch (Exception ex)
                {
                }
                #endregion

                for (int i = 0; i < idDocuments.Length; i++)
                {
                    temp.Add(new MassiveOperationTarget(idDocuments[i], idDocuments[i]));
                    // Inserimento dei documenti in area di conservazione
                    // Identificativo dell'istanza di certificazione
                    String conservationAreaId;
                    // Se il documento ha un file acquisito
                    SchedaDocumento documentDetails = DocumentManager.getDocumentDetails(this.Page, idDocuments[i], idDocuments[i]);
                    if (Convert.ToInt32(documentDetails.documenti[0].fileSize) > 0)
                    {
                        // Inserimento del documento in area di conservazione                        
                        //Inserimento del metodo di controllo della presenza di un documento fascicolato nell'istanza
                        if (this.CheckProjectAndInsertInSA(documentDetails.systemId, mot.Id, itemsCons))
                        {
                            // MEV CS 1.5 - F03_01
                            #region oldCode
                            //this.InsertInSA2(documentDetails, reportToCompile, mot.Id);
                            #endregion

                            #region NewCode
                            this.InsertInSA2_WithConstraint(documentDetails, 
                                reportToCompile, 
                                mot.Id,
                                ref indexOfCurrentDoc,
                                ref currentDimOfDocs,
                                DimMaxInIstanza,
                                numMaxDocInIstanza,
                                TolleranzaPercentuale
                                );
                            #endregion
                            // End MEV
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
                        retval = false;
                    }

                }

            }
            return retval;

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
                UserManager.GetInfoUser().idPeople,
                UserManager.GetInfoUser().idGruppo) == 1)
                message.Append("Si sta creando la prima istanza di conservazione. ");

            // Recupero dell'eventiale fascicolo selezionato
            Fascicolo selectedProject = ProjectManager.getProjectInSession();

            // Inserimento del documento nella conservazione e recupero dell'id dell'istanza
            if (!string.IsNullOrEmpty(idProject))
                // Da ricerca documenti in fascicolo
                conservationAreaId = DocumentManager.addAreaConservazioneAM(
                    this.Page,
                    documentDetails.systemId,
                    idProject,
                    documentDetails.docNumber,
                    UserManager.GetInfoUser(),
                    "F");
            else
                conservationAreaId = DocumentManager.addAreaConservazioneAM(
                    this.Page,
                    documentDetails.systemId,
                    "",
                    documentDetails.docNumber,
                    UserManager.GetInfoUser(),
                    "D");

            try
            {
                int size_xml = DocumentManager.getItemSize(
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

                DocumentManager.insertSizeInItemCons(conservationAreaId, total_size);

                DocumentManager.updateItemsConservazione(
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

        protected void generateReport(MassiveOperationReport report, string titolo)
        {
            this.generateReport(report, titolo, IsFasc);
        }

        public void generateReport(MassiveOperationReport report, string titolo, bool isFasc)
        {
            this.grdReport.DataSource = report.GetDataSet();
            this.grdReport.DataBind();
            this.pnlReport.Visible = true;
            this.upReport.Update();

            string template = (isFasc) ? "../xml/massiveOp_formatPdfExport_fasc.xml" : "../xml/massiveOp_formatPdfExport.xml";
            report.GenerateDataSetForExport(Server.MapPath(template), titolo);

            this.plcMessage.Visible = false;
            this.UpPnlMessage.Update();

            this.BtnConfirm.Enabled = false;
            this.BtnReport.Visible = true;
            this.UpPnlButtons.Update();
        }

        /// <summary>
        /// Questa funzione si occupa di avviare lo spostamento nell'area di conservazione
        /// </summary>
        /// <param name="documentsId">Lista dei system id dei documenti selezionati</param>
        /// <returns>Report dell'inserimento</returns>
        private void ExecuteInsertDocumentsInSA(List<MassiveOperationTarget> documentsId, MassiveOperationReport reportToCompile)
        {
            #region MEV CS 1.5 - F03_01
            // Indice dell'elemento lavorato
            int indexOfCurrentDoc = 0;

            // Dimensione complessiva raggiunta
            int currentDimOfDocs = 0;

            // Get valori limiti per le istanze di conservazione
            int DimMaxInIstanza = 0;
            int numMaxDocInIstanza = 0;
            int TolleranzaPercentuale = 0;
            try
            {
                InfoUtente infoUt = UserManager.GetInfoUser();
                DimMaxInIstanza = DocumentManager.getDimensioneMassimaIstanze(infoUt.idAmministrazione);
                numMaxDocInIstanza = DocumentManager.getNumeroDocMassimoIstanze(infoUt.idAmministrazione);
                TolleranzaPercentuale = DocumentManager.getPercentualeTolleranzaDinesioneIstanze(infoUt.idAmministrazione);
            }
            catch (Exception ex)
            {
            }
            #endregion

            // Per ogni id di documento
            foreach (MassiveOperationTarget mot in documentsId)
            {
                try
                {
                    // Recupero della scheda documento
                    SchedaDocumento documentDetails = DocumentManager.getDocumentDetails(
                        this.Page,
                        mot.Id,
                        String.Empty);

                    // Se c'è la scheda documento, si procede con la verifica dei dati e con 
                    // l'eventuale inserimento nell'area di conservazione
                    if (documentDetails != null)
                    {
                        // MEV CS 1.5 - F03_01
                        #region OldCode
                        //this.CheckDataValidityAndInsertInSA(documentDetails, reportToCompile);
                        #endregion

                        #region NewCode
                        this.CheckDataValidityAndInsertInSA_WithConstraint(documentDetails,
                            reportToCompile,
                            ref indexOfCurrentDoc,
                            ref currentDimOfDocs,
                            DimMaxInIstanza,
                            numMaxDocInIstanza,
                            TolleranzaPercentuale
                            );
                        #endregion
                        // End MEV
                    }
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
                UserManager.GetInfoUser().idPeople,
                UserManager.GetInfoUser().idGruppo) == 1)
                message.Append("Si sta creando la prima istanza di conservazione. ");

            // Recupero dell'eventiale fascicolo selezionato
            Fascicolo selectedProject = ProjectManager.getProjectInSession();

            // Inserimento del documento nella conservazione e recupero dell'id dell'istanza
            if (selectedProject != null)
                // Da ricerca documenti in fascicolo
                conservationAreaId = DocumentManager.addAreaConservazioneAM(
                    this.Page,
                    documentDetails.systemId,
                    selectedProject.systemID,
                    documentDetails.docNumber,
                    UserManager.GetInfoUser(),
                    "F");
            else
                conservationAreaId = DocumentManager.addAreaConservazioneAM(
                    this.Page,
                    documentDetails.systemId,
                    "",
                    documentDetails.docNumber,
                    UserManager.GetInfoUser(),
                    "D");

            try
            {
                int size_xml = DocumentManager.getItemSize(
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

                DocumentManager.insertSizeInItemCons(conservationAreaId, total_size);

                DocumentManager.updateItemsConservazione(
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

        #region MEV CS 1.5 - F03_01
        /// <summary>
        /// Funzione per l'inserimento di un documento nell'area di conservazione
        /// </summary>
        /// <param name="documentDetails">Dettagli del documento da inserire</param>
        /// <param name="report">Report di inserimento</param>
        private void InsertInSA2_WithConstraint(SchedaDocumento documentDetails, 
            MassiveOperationReport report, 
            string idProject,
            ref int indexOfCurrentDoc,
            ref int currentDimOfDocs,
            int DimMaxInIstanza,
            int numMaxDocInIstanza,
            int percentualeTolleranza
            )
        {
            StringBuilder message = new StringBuilder();
            String conservationAreaId;
            //string id = MassiveOperationUtils.getItem(documentDetails.systemId).Codice;
            // Se si sta creando la prima istanza di conservazione...
            if (DocumentManager.isPrimaConservazione(
                UserManager.GetInfoUser().idPeople,
                UserManager.GetInfoUser().idGruppo) == 1)
                message.Append("Si sta creando la prima istanza di conservazione. ");

            // Recupero dell'eventiale fascicolo selezionato
            Fascicolo selectedProject = ProjectManager.getProjectInSession();

            // Controllo Rispetto dei Vincoli dell'istanza
            #region Vincoli Istanza di Conservazione
            // Variabili di controllo per violazione dei vincoli sulle istanze
            bool numDocIstanzaViolato = false;
            bool dimIstanzaViolato = false;
            int TotalSelectedDocumentSize = 0;

            TotalSelectedDocumentSize = DocumentManager.GetTotalDocumentSize(documentDetails);
            // Dimensione documenti raggiunta
            currentDimOfDocs = TotalSelectedDocumentSize + currentDimOfDocs;
            // Numero di documenti raggiunti
            indexOfCurrentDoc = indexOfCurrentDoc + 1;

            numDocIstanzaViolato = DocumentManager.isVincoloNumeroDocumentiIstanzaViolato(indexOfCurrentDoc, numMaxDocInIstanza);
            dimIstanzaViolato = DocumentManager.isVincoloDimensioneIstanzaViolato(currentDimOfDocs, DimMaxInIstanza, percentualeTolleranza);

            double DimensioneMassimaConsentitaPerIstanza = 0;
            DimensioneMassimaConsentitaPerIstanza = DimMaxInIstanza - ((DimMaxInIstanza * percentualeTolleranza) / 100);

            int DimMaxConsentita = 0;
            DimMaxConsentita = Convert.ToInt32(DimensioneMassimaConsentitaPerIstanza);

            if (numDocIstanzaViolato || dimIstanzaViolato)
            {
                // Azzero le due variabili
                currentDimOfDocs = 0;
                indexOfCurrentDoc = 0;
            }
            #endregion

            // Inserimento del documento nella conservazione e recupero dell'id dell'istanza
            if (!string.IsNullOrEmpty(idProject))
                // Da ricerca documenti in fascicolo
                conservationAreaId = DocumentManager.addAreaConservazioneAM_WithConstraint(
                    this.Page,
                    documentDetails.systemId,
                    idProject,
                    documentDetails.docNumber,
                    UserManager.GetInfoUser(),
                    "F",
                    numDocIstanzaViolato,
                    dimIstanzaViolato,
                    DimMaxConsentita,
                    numMaxDocInIstanza,
                    TotalSelectedDocumentSize
                    );
            else
                conservationAreaId = DocumentManager.addAreaConservazioneAM_WithConstraint(
                    this.Page,
                    documentDetails.systemId,
                    "",
                    documentDetails.docNumber,
                    UserManager.GetInfoUser(),
                    "D",
                    numDocIstanzaViolato,
                    dimIstanzaViolato,
                    DimMaxConsentita,
                    numMaxDocInIstanza,
                    TotalSelectedDocumentSize
                    );

            try
            {
                if (conservationAreaId.ToString() != "-1")
                {
                    int size_xml = DocumentManager.getItemSize(
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

                    DocumentManager.insertSizeInItemCons(conservationAreaId, total_size);

                    DocumentManager.updateItemsConservazione(
                        tipoFile,
                        Convert.ToString(numeroAllegati),
                        conservationAreaId);
                }
            }
            catch (Exception e)
            {
                report.AddReportRow(
                    documentDetails.systemId,
                    MassiveOperationReport.MassiveOperationResultEnum.KO,
                    "Si è verificato un errore durante l'inserimento del documento nell'area di conservazione");
            }

            #region oldCode
            //report.AddReportRow(
            //    documentDetails.systemId,
            //    MassiveOperationReport.MassiveOperationResultEnum.OK,
            //    "Documento inserito correttamente in area di conservazione.");
            #endregion

            #region MEV CS 1.5 - F03_01
            if (!string.IsNullOrEmpty(conservationAreaId.ToString()))
            {
            report.AddReportRow(
                documentDetails.systemId,
                MassiveOperationReport.MassiveOperationResultEnum.OK,
                "Documento inserito correttamente in area di conservazione.");
            }
            else
            {
                report.AddReportRow(
                documentDetails.systemId,
                MassiveOperationReport.MassiveOperationResultEnum.KO,
                "Documento non inserito in area di conservazione per violazione vincoli dimensione istanza.");
            }
            #endregion
        }

        /// <summary>
        /// Funzione per l'inserimento di un documento nell'area di conservazione
        /// </summary>
        /// <param name="documentDetails">Dettagli del documento da inserire</param>
        /// <param name="report">Report di inserimento</param>
        private void InsertInSA_WithConstraint(SchedaDocumento documentDetails, 
            MassiveOperationReport report,
            ref int indexOfCurrentDoc,
            ref int currentDimOfDocs,
            int DimMaxInIstanza,
            int numMaxDocInIstanza,
            int percentualeTolleranza
            )
        {
            StringBuilder message = new StringBuilder();
            String conservationAreaId;
            string id = MassiveOperationUtils.getItem(documentDetails.systemId).Codice;
            // Se si sta creando la prima istanza di conservazione...
            if (DocumentManager.isPrimaConservazione(
                UserManager.GetInfoUser().idPeople,
                UserManager.GetInfoUser().idGruppo) == 1)
                message.Append("Si sta creando la prima istanza di conservazione. ");

            // Recupero dell'eventiale fascicolo selezionato
            Fascicolo selectedProject = ProjectManager.getProjectInSession();

            // Controllo Rispetto dei Vincoli dell'istanza
            #region Vincoli Istanza di Conservazione
            // Variabili di controllo per violazione dei vincoli sulle istanze
            bool numDocIstanzaViolato = false;
            bool dimIstanzaViolato = false;
            int TotalSelectedDocumentSize = 0;

            TotalSelectedDocumentSize = DocumentManager.GetTotalDocumentSize(documentDetails);
            // Dimensione documenti raggiunta
            currentDimOfDocs = TotalSelectedDocumentSize + currentDimOfDocs;
            // Numero di documenti raggiunti
            indexOfCurrentDoc = indexOfCurrentDoc + 1;

            numDocIstanzaViolato = DocumentManager.isVincoloNumeroDocumentiIstanzaViolato(indexOfCurrentDoc, numMaxDocInIstanza);
            dimIstanzaViolato = DocumentManager.isVincoloDimensioneIstanzaViolato(currentDimOfDocs, DimMaxInIstanza, percentualeTolleranza);

            double DimensioneMassimaConsentitaPerIstanza = 0;
            DimensioneMassimaConsentitaPerIstanza = DimMaxInIstanza - ((DimMaxInIstanza * percentualeTolleranza) / 100);

            int DimMaxConsentita = 0;
            DimMaxConsentita = Convert.ToInt32(DimensioneMassimaConsentitaPerIstanza);

            if (numDocIstanzaViolato || dimIstanzaViolato)
            {
                // Azzero le due variabili
                currentDimOfDocs = 0;
                indexOfCurrentDoc = 0;
            }
            #endregion

            // Inserimento del documento nella conservazione e recupero dell'id dell'istanza
            if (selectedProject != null)
                // Da ricerca documenti in fascicolo
                conservationAreaId = DocumentManager.addAreaConservazioneAM_WithConstraint(
                    this.Page,
                    documentDetails.systemId,
                    selectedProject.systemID,
                    documentDetails.docNumber,
                    UserManager.GetInfoUser(),
                    "F",
                    numDocIstanzaViolato,
                    dimIstanzaViolato,
                    DimMaxConsentita,
                    numMaxDocInIstanza,
                    TotalSelectedDocumentSize
                    );
            else
                conservationAreaId = DocumentManager.addAreaConservazioneAM_WithConstraint(
                    this.Page,
                    documentDetails.systemId,
                    "",
                    documentDetails.docNumber,
                    UserManager.GetInfoUser(),
                    "D",
                    numDocIstanzaViolato,
                    dimIstanzaViolato,
                    DimMaxConsentita,
                    numMaxDocInIstanza,
                    TotalSelectedDocumentSize
                    );

            try
            {
                if (conservationAreaId.ToString() != "-1")
                {
                    int size_xml = DocumentManager.getItemSize(
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

                    DocumentManager.insertSizeInItemCons(conservationAreaId, total_size);

                    DocumentManager.updateItemsConservazione(
                        tipoFile,
                        Convert.ToString(numeroAllegati),
                        conservationAreaId);
                }
            }
            catch (Exception e)
            {
                report.AddReportRow(
                    id,
                    MassiveOperationReport.MassiveOperationResultEnum.KO,
                    "Si è verificato un errore durante l'inserimento del documento nell'area di conservazione");
            }

            #region old code
            //report.AddReportRow(
            //    id,
            //    MassiveOperationReport.MassiveOperationResultEnum.OK,
            //    "Documento inserito correttamente in area di conservazione.");
            #endregion

            #region MEV CS 1.5 - F03_01
            if (!string.IsNullOrEmpty(conservationAreaId.ToString()))
            {
                report.AddReportRow(
                    documentDetails.systemId,
                    MassiveOperationReport.MassiveOperationResultEnum.OK,
                    "Documento inserito correttamente in area di conservazione.");
            }
            else
            {
                report.AddReportRow(
                documentDetails.systemId,
                MassiveOperationReport.MassiveOperationResultEnum.KO,
                "Documento non inserito in area di conservazione per violazione vincoli dimensione istanza.");
            }
            #endregion
        }

        /// <summary>
        /// Funzione per la validazione dei dati sul documento e l'avvio dell'inserimento
        /// nell'area di conservazione
        /// </summary>
        /// <param name="documentDetails">Dettagli del documento da inserire nell'area di conservazione</param>
        /// <param name="report">Report dello spostamento</param>
        private void CheckDataValidityAndInsertInSA_WithConstraint(SchedaDocumento documentDetails, 
            MassiveOperationReport report,
            ref int indexOfCurrentDoc,
            ref int currentDimOfDocs,
            int DimMaxInIstanza,
            int numMaxDocInIstanza,
            int percentualeTolleranza
            )
        {
            // Identificativo dell'istanza di certificazione
            String conservationAreaId;
            // Se il documento ha un file acquisito
            if (Convert.ToInt32(documentDetails.documenti[0].fileSize) > 0 && (string.IsNullOrEmpty(documentDetails.inConservazione) || !(documentDetails.inConservazione).Equals("1")))
                // Inserimento del documento in area di conservazione
                this.InsertInSA_WithConstraint(documentDetails, 
                    report,
                    ref indexOfCurrentDoc,
                    ref currentDimOfDocs,
                    DimMaxInIstanza,
                    numMaxDocInIstanza,
                    percentualeTolleranza
                    );

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

        #endregion

        #endregion

    }
}