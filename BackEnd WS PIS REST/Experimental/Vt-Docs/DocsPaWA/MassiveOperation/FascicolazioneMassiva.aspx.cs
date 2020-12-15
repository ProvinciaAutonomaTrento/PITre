using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using System.Linq;

namespace DocsPAWA.MassiveOperation
{
    public partial class FascicolazioneMassiva : MassivePage
    {

        #region Proprietà

        protected override string PageName
        {
            get {
                return "Fascicolazione Massiva";
            }
        }

        protected override bool IsFasc
        {
            get {
                return false; 
            }
        }
        /// <summary>
        /// Il path della finestra di di ricerca fascicoli
        /// </summary>
        protected string GetPath
        {
            get
            {
                return Utils.getHttpFullPath(this) + "/popup/ricercaFascicoli.aspx";
            }
        }

        #endregion

        #region Funzioni di utilità

        /// <summary>
        /// Funzione per la ricerca di fascicoli con un dato codice
        /// </summary>
        /// <param name="registry">Il registro in cui ricercare il fascicolo</param>
        /// <param name="projectCode">Il codice del fascicolo da ricercare</param>
        /// <returns>La lista dei fascicoli individuati</returns>
        private Fascicolo GetProject(Registro registry, string projectCode)
        {
            // La lista di fascicoli restituiti dalla ricerca
            Fascicolo[] projectList;

            // Il risultato da restituire
            Fascicolo toReturn = null;

            // La gerarchia di fascicolazione
            FascicolazioneClassifica[] classificationHie;

            // Codice con cui effettuare la ricerca una volta aperta la maschera
            string projectCodeToSearch;

            // Recupero della lista dei fascicoli
            projectList = FascicoliManager.getListaFascicoliDaCodice(
                this,
                projectCode,
                registry,
                "R");

            // Se la ricerca ha restituito null, viene segnalato un messaggio all'utente
            if (projectList == null)
                ScriptManager.RegisterStartupScript(this,
                    this.GetType(),
                    "ErrSearch",
                    "alert('Errore durante la ricerca del fascicolo con il codice segnalato.');",
                    true);
            else
            {
                // ...altrimenti viene intrapresa una azione diversa a seconda del
                // numero di elementi restituiti
                switch (projectList.Length)
                {
                    case 0:     // Nessun risultato
                        // Visualizzazione di un avviso per l'utente
                        ScriptManager.RegisterStartupScript(this,
                            this.GetType(),
                            "NoResult",
                            "alert('Nessun fascicolo individuato con il codice segnalato');",
                            true);
                        break;

                    case 1:   // Un fascicolo individuato
                        toReturn = projectList[0];
                        break;

                    default:    // Più fascicoli individuati
                        // Viene mostrata la finestra di ricerca fascicoli con il codice impostato
                        // Memorizzazione della lista di fascicoli nella sessione
                        Session.Add("listaFascFascRapida", projectList);

                        // Se il fascicolo è un procedimentale...
                        if (!projectList[0].tipo.Equals("G"))
                        {
                            // ...calcolo della gerarchia di classificazione
                            classificationHie = FascicoliManager.getGerarchia(
                                this,
                                projectList[0].idClassificazione,
                                UserManager.getUtente(this).idAmministrazione);

                            projectCodeToSearch = classificationHie[classificationHie.Length - 1].codice;
                        }
                        else
                            projectCodeToSearch = projectCode;

                        // Registrazione dello script per l'apertura della finestra
                        ScriptManager.RegisterStartupScript(this,
                            this.GetType(),
                            "OpenModale",
                            "ApriRicercaFascicoli('" + projectCodeToSearch + "', 'Y');",
                            true);

                        toReturn = null;
                        break;
                }

            }

            return toReturn;

        }

        /// <summary>
        /// Funzione per la ricerca di un folder con un dato codice
        /// </summary>
        private void SearchFolder()
        {
            // Codice del fascicolo e descrizione del folder
            StringBuilder projectCode, folderDescription;

            // Contenuto della casella di codice splittata
            // La prima cella conterrà il codice, mentre la seconda conterrà la 
            // descrizione del folder
            string[] splittedString = null;

            // Lista dei folder con un dato codice
            Folder[] folders;

            // Il fascicolo in cui fascicolare
            Fascicolo project;

            // Splittamento della stringa del codice
            splittedString = this.txtCodFascicolo.Text.Split(
                new String[] { "//"}, 
                StringSplitOptions.RemoveEmptyEntries);

            // Reperimento del codice del fascicolo e della descrizione del folder
            projectCode = new StringBuilder(splittedString[0]);
            folderDescription = new StringBuilder(splittedString[1]);

            // Ricerca del sottofascicolo
            folders = this.SearchFolders(projectCode.ToString(), folderDescription.ToString());

            // Se ci sono folder...
            if (folders != null &&
                folders.Length > 0)
            {
                // ...viene richiesto il fascicolo con id pari all'id del primo folder
                project = FascicoliManager.getFascicoloById(
                    this,
                    folders[0].idFascicolo);

                // ...il folder selezionato è l'ultimo
                project.folderSelezionato = folders[folders.Length - 1];

                // ...costruzione del codice e della descrizione completi
                projectCode = new StringBuilder(project.codice + "//");
                folderDescription = new StringBuilder(project.descrizione + "//");
                foreach (Folder folder in folders)
                {
                    projectCode.Append(folder.descrizione + "/");
                    folderDescription.Append(folder.descrizione + "/");
                }

                // Eliminazione dell'ultimo carattere se è uno /
                if (folderDescription.ToString().EndsWith("/"))
                {
                    projectCode = projectCode.Remove(projectCode.Length - 1, 1);
                    folderDescription = folderDescription.Remove(folderDescription.Length - 1, 1);
                }

                this.txtCodFascicolo.Text = projectCode.ToString();
                this.txtDescFascicolo.Text = folderDescription.ToString();

                // Salvataggio del fascicolo di fascicolazione rapida
                FascicoliManager.setFascicoloSelezionatoFascRapida(
                    this,
                    project);
            }
        }

        /// <summary>
        /// Funzione per la ricerca dei folder di un fascicolo
        /// </summary>
        /// <param name="projectCode">Codice de fascicolo</param>
        /// <param name="folderDescription">Descrizione del folder</param>
        /// <returns>Lista di folder trovati</returns>
        private Folder[] SearchFolders(string projectCode,string folderDescription)
        {
            // Lista da restituire
            Folder[] folders = null;

            // Lista dei registri associati al ruolo
            Registro[] registries;

            // Reperimento della lista di registri
            registries = UserManager.getRuolo(this).registri;

            // Ricerca del fascicolo in ogni registro
            foreach (Registro reg in registries)
                folders = FascicoliManager.getListaFolderDaCodiceFascicolo(
                    this,
                    projectCode,
                    folderDescription,
                    reg);

            // I FOLDER VANNO CERCATI IN TUTTI I REGISTRI E QUINDI SE CI SONO PIù REGISTRI CHE LI CONTENGONO,
            // DEVE ESSERE RESTUITO UN ERRORE
            
            // Restituzione della lista di folder
            return folders;
                
        }

        /// <summary>
        /// Funzione per la ricerca di un fascicolo con un dato codice
        /// </summary>
        private


        void SearchProject()
        {
            // La lista dei registri su cui effettuare la ricerca
            Registro[] registers;

            // Lista dei fascicoli restituiti dalla ricerca
            List<Fascicolo> projectList;

            // Array temporaneo in cui depositare i fascicoli restiuiti dalla query effettuata
            // su un registro (serve perché se ho due fascicoli con lo stesso system id la add nella
            // lista lo aggiunge lo stesso in quanto non è definito l'equals)
            List<Fascicolo> tmp;

            // Un fascicolo con tipologia diversa da generale.
            Fascicolo project = null;

            // Gerarchia di classificazione
            FascicolazioneClassifica[] classificationHie;

            // Il codice di classifica con cui aprire l'eventuale finestra di ricerca
            String searchCode;

            // Recupero dei registri associati all'utente
            registers = UserManager.GetRegistriByRuolo(this, UserManager.getRuolo(this).systemId);

            // Inizializzazione della lista
            projectList = new List<Fascicolo>();

            // Ricerca del fascicolo su tutti i registri
            projectList.AddRange(
                FascicoliManager.getListaFascicoliDaCodice(
                    this, 
                    this.txtCodFascicolo.Text, 
                    null, 
                    "I"));

            // Ricerca del fascicolo in tutti i registri
            if (registers != null &&
                registers.Length > 0)
                foreach (Registro registry in registers)
                {
                    tmp = new List<Fascicolo>(FascicoliManager.getListaFascicoliDaCodice(
                            this,
                            this.txtCodFascicolo.Text,
                            registry,
                            "I"));

                    foreach (Fascicolo t in tmp)
                        if (projectList.Where(e => e.systemID == t.systemID).Count() == 0)
                            projectList.Add(t);
                }


            // A seconda del numero di risultati trovati viene eseguita un'operazione
            // diversa
            switch (projectList.Count)
            {
                case 0:     // Nessun fascicolo -> popup di avviso
                    ScriptManager.RegisterStartupScript(this,
                        this.GetType(),
                        "NoPrj",
                        String.Format(
                            "alert('Non è stato individuato nessun fascicolo con il codice {0}.');",
                            this.txtCodFascicolo.Text),
                        true);

                    break;

                case 1:     // Un solo fascicolo -> è il fascicolo in cui fascicolare
                    this.txtDescFascicolo.Text = projectList[0].descrizione;
                    FascicoliManager.setFascicoloSelezionatoFascRapida(this, projectList[0]);
                    break;

                default:    // Default, ci sono più fascicoli -> apertura popup di ricerca
                    Session.Add("listaFascFascRapida", projectList);

                    // Il codice di ricerca è inizialmente impostato al contenuto 
                    // della casella di testo del codice
                    searchCode = this.txtCodFascicolo.Text;

                    // Se è presente almeno un fascicolo restituito che non sia di tipo generale,
                    // viene ricavata la gerarchia relativa al codice
                    project = projectList.Where(e => e.tipo != "G").FirstOrDefault();
                    if (project != null)
                    {
                        classificationHie = FascicoliManager.getGerarchia(
                            this,
                            project.idClassificazione,
                            UserManager.getUtente(this).idAmministrazione);

                        // Il codice con cui aprire la pagina di ricerca, è quello dell'ultimo
                        // fascicolo della gerarchia
                        if(classificationHie != null &&
                            classificationHie.Length > 0)
                            searchCode = classificationHie[classificationHie.Length - 1].codice;

                    }

                    // Immersione dello script per l'apertura della finestra di ricerca del codice
                    // Registrazione dello script per l'apertura della finestra
                    ScriptManager.RegisterStartupScript(this,
                        this.GetType(),
                        "OpenModale",
                        "ApriRicercaFascicoli('" + searchCode + "', 'Y');",
                        true);

                    break;
            }

        }


        /// <summary>
        /// Funzione per la fascicolazione dei documenti
        /// </summary>
        /// <param name="selectedItem">Lista dei system id dei documenti selezionati</param>
        /// <param name="project">Il fascicolo su cui fascicolare</param>
        /// <param name="reportTable">Tabella del report in cui inserire le informazioni sull'esito degli import</param>
        private void ProceedWithOperation(List<MassiveOperationTarget> selectedItem, Fascicolo project, MassiveOperationReport report)
        {
            // Un booleano utilizzato per indicare che un documento è già
            // fascicolato in un determinato fascicolo
            bool isAlreadyClassifficated;

            // Per indicare se il documento è bloccato
            bool isBlocked;

            // Il risultato della fascicolazione
            bool classificationResult;

            // Il messaggio da agigungere al report
            string message;

            bool isAnnullato;

            // Il risultato da aggiungere al report
            MassiveOperationReport.MassiveOperationResultEnum result;
            DocsPaWebService ws = new DocsPaWebService();
            // Per ogni documento da fascicolare...
            foreach (MassiveOperationTarget mot in selectedItem)
            {
                // Inizializzazione di messaggio e risultato
                message = String.Empty;
                result = MassiveOperationReport.MassiveOperationResultEnum.OK;

                InfoUtente infoUtente = UserManager.getInfoUtente(this);

                // ... si verifica se il documento è già fascicolato nel fascicolo
                // selezionato
                //isAlreadyClassifficated = DocumentManager.IsDocumentInProject(this, mot.Id, project.systemID);
                isAlreadyClassifficated = DocumentManager.IsDocumentInFolderOrSubFolder(this, infoUtente, mot.Id, project);

                //Verifico se il documento è in checkout
                isBlocked = DocsPAWA.CheckInOut.CheckInOutServices.IsCheckedOutDocument(mot.Id,mot.Id,infoUtente, true);

                isAnnullato = ws.IsDocAnnullatoByIdProfile(mot.Id);

                string accessRight = ws.getAccessRightDocBySystemID(mot.Id, infoUtente);
                // ...se il documento è già fascicolato, viene aggiunto un messaggio
                // adeguato al report
                if (isAlreadyClassifficated)
                {
                    message = "Il documento risulta già fascicolato nel fascicolo specificato o non è stato possibile recuperare informazioni sui fascicoli.";
                    result = MassiveOperationReport.MassiveOperationResultEnum.AlreadyWorked;
                }
                else if(isBlocked)
                {
                    message = "Il documento risulta bloccato.";
                    result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                }
                else if (accessRight.Equals("20"))
                {
                    result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                    message = String.Format("Il documento è in attesa di accettazione, quindi non può essere fascicolato");
                }
                else if (isAnnullato)
                {
                    result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                    message = String.Format("Il documento risulta annullato, quindi non può essere fascicolato");
                }
                else
                {
                    // ...altrimenti si procede con la classificazione
                    classificationResult = DocumentManager.FascicolaDocumentoAM(
                        this,
                        mot.Id,
                        project,
                        out message);

                    // Impostazione di un messaggio adeguato in base all'esito della
                    // fascicolazione
                    if (classificationResult)
                        message = "Documento fascicolato con successo.";
                    else
                    {
                        result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                        message = "Errore durante la fascicolazione del documento.";

                    }
                }

                // Aggiunta della riga al report
                report.AddReportRow(mot.Codice, result, message);

            }

        }

        #endregion

        #region Event handler

        protected void Page_Load(object sender, EventArgs e)
        {
            // Il fascicolo selezionato per selezione rapida
            Fascicolo project;

            // Se non si è in postback, viene cancellato il fascicolo
            // di selezione rapida
            if (!IsPostBack)
            {
                FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                FascicoliManager.removeCodiceFascRapida(this);
                FascicoliManager.removeDescrizioneFascRapida(this);
                FascicoliManager.removeFolderSelezionato(this);
            }
        }

        /// <summary>
        /// Al prerender viene verificato se c'è un fascicolo di selezione rapida
        /// ed in tal caso vengono compilati i campi codice e descrizione
        /// </summary>
        public void Page_Prerender(object sender, EventArgs e)
        {
            // Fascicolo di selezione rapida
            Fascicolo project = null;

            // Recupero del fascicolo di fascicolazione rapida
            project = FascicoliManager.getFascicoloSelezionatoFascRapida(this);

            // Se il fascicolo è stato individuato con successo e non è valorizzato folderSelezionato
            // vengono compilate le due caselle di testo del codice e della descrizione del fasciolo
            if (project != null && project.folderSelezionato == null)
            {
                this.txtCodFascicolo.Text = project.codice;
                this.txtDescFascicolo.Text = project.descrizione;
            }
            else
                if (!String.IsNullOrEmpty(FascicoliManager.getCodiceFascRapida(this)) &&
                    !String.IsNullOrEmpty(FascicoliManager.getDescrizioneFascRapida(this)))
                {
                    this.txtCodFascicolo.Text = FascicoliManager.getCodiceFascRapida(this);
                    this.txtDescFascicolo.Text = FascicoliManager.getDescrizioneFascRapida(this);
                }
        
        }


        /// <summary>
        /// Evento generato al cambio del testo nella casella del codice fascicolo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCodFascicolo_TextChanged(object sender, EventArgs e)
        {
            // Il fascicolo restituito dalla ricerca
            Fascicolo project;

            // Viene cancellato il fascicolo rapido
            FascicoliManager.setFascicoloSelezionatoFascRapida(this, null);

            // Se la casella di testo contiene del testo...
            if (!String.IsNullOrEmpty(this.txtCodFascicolo.Text))
                // Se c'è uno / significa che bisogna effettuare la ricerca nei folder
                if (this.txtCodFascicolo.Text.Contains("//"))
                    this.SearchFolder();
                else
                    this.SearchProject();

        }

        

        

        /// <summary>
        /// Event Handler per il bottone di ricerca fascicolo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void imgFasc_Click(object sender, ImageClickEventArgs e)
        {
            // Il fascicolo restituito dalla ricerca
            Fascicolo project;

            // La gerarchia di classificazione
            FascicolazioneClassifica[] classificationHie;

            // Il codice con cui avviare la ricerca
            string projectCode = String.Empty;

            // Se la casella di testo del codice fascicolo non è valorizzata,
            // viene avviata una ricerca con codice fascicolo nullo
            if (String.IsNullOrEmpty(this.txtCodFascicolo.Text))
                ScriptManager.RegisterStartupScript(this,
                    this.GetType(),
                    "SearchPrj",
                    "ApriRicercaFascicoli('', 'N');",
                    true);
            else
            {
                // Altrimenti viene effettuata una ricerca con il codice fascicolo
                // segnalato ed in seguito viene aperta la maschera di ricerca
                // con il codice segnalato
                // Ricerca del fascicolo con il codice segnalato
                project = this.GetProject(
                    null,
                    this.txtCodFascicolo.Text);

                // Se è stato trovato un fascicolo...
                if (project != null)
                {
                    Session["FascSelezFascRap"] = project;

                    // ...se è un generale, viene aperta la ricerca con il codice del fascicolo,
                    // altrimenti viene calcolata la gerarchia di classificazione e viene aperta
                    // la maschera con il codice dell'ultimo fascicolo della gerarchia di classificazione
                    if (project.tipo.Equals("G"))
                        projectCode = project.codice;
                    else
                    {
                        classificationHie = FascicoliManager.getGerarchia(
                            this,
                            project.idClassificazione,
                            UserManager.getUtente(this).idAmministrazione);

                        projectCode = classificationHie[classificationHie.Length - 1].codice;

                    }

                    ScriptManager.RegisterStartupScript(this,
                         this.GetType(),
                         "OpenModal",
                         "ApriRicercaFascicoli('" + projectCode + "', 'N');",
                         true);
                }
            }

        }

        /// <summary>
        /// Event Handler per il click del pulsante conferma
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override bool btnConferma_Click(object sender, EventArgs e)
        {
            // Il report da mostrare all'utente
            MassiveOperationReport report;

            // Lista di system id degli elementi selezionati
            List<MassiveOperationTarget> selectedItem;

            // Il fascicolo selezionato per la fascicolazione rapida
            Fascicolo project;

            // Valore utilizzato per indicare che è possibile procedere con la fascicolazione
            bool canProceed = true;

            // Inizializzazione del report
            report = new MassiveOperationReport();

            // Il messaggio di errore
            StringBuilder errorMessage = new StringBuilder("Impossibile eseguire la fascicolazione:");

            // Recupero della lista dei system id dei documenti selezionati
            selectedItem = MassiveOperationUtils.GetSelectedItems();

            // Recupero del fascicolo selezionato per la fascicolazione rapida
            project = FascicoliManager.getFascicoloSelezionatoFascRapida(this);


            // Se il fascicolo non è valorizzato non si può procedere
            if (project == null)
            {
                canProceed = false;
                errorMessage.AppendLine(" Selezionare un fascicolo in cui fascicolare.");
            }

            // Se non sono stati selezionati documenti, non si può procedere con la fascicolazione
            if (selectedItem == null ||
                selectedItem.Count == 0)
            {
                canProceed = false;
                errorMessage.AppendLine("- Selezionare almeno un documento da fascicolare.");
            }

            // Se non è possibile continuare, viene salvata una nuova riga per il report
            // e ne viene impostato l'esito negativo
            if (!canProceed)
                report.AddReportRow(
                    "N.A.", 
                    MassiveOperationReport.MassiveOperationResultEnum.KO, 
                    errorMessage.ToString());
            else
                // Altrimenti si procede con la fascicolazione
                this.ProceedWithOperation(selectedItem, project, report);
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);
            this.generateReport(report, "Fascicolazione massiva");
            return true;
        }

        #endregion

    }
}