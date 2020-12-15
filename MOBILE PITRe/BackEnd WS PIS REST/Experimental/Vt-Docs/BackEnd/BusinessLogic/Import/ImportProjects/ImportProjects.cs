using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using BusinessLogic.Documenti;
using BusinessLogic.Fascicoli;
using BusinessLogic.ProfilazioneDinamica;
using BusinessLogic.Trasmissioni;
using BusinessLogic.Utenti;
using DocsPaVO.amministrazione;
using DocsPaVO.fascicolazione;
using DocsPaVO.Import.Project;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.Note;
using DocsPaVO.PrjDocImport;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.utente;

namespace BusinessLogic.Import.ImportProjects
{
    /// <summary>
    /// Questa classe si occupa di effettura la creazione di fascicoli a partire da dati
    /// contenuti in un foglio excel il cui content viene passato per parametro
    /// </summary>
    public class ImportProjects
    {
        #region Funzioni pubbliche

        public List<ImportResult> ImportProject(byte[] content, string fileName, string serverPath,
            string modelPath, InfoUtente userInfo, Ruolo role, bool isEnabledSmistamento)
        {
            // Il path completo in cui è posizionato il file excel contente i dati
            // sui fascicoli da importare
            string completePath = String.Empty;

            // Il risultato dell'elaborazione
            List<ImportResult> result = new List<ImportResult>();

            // Per prima cosa si verifica se l'utente può creare fascicoli
            Funzione[] functions = (Funzione[])role.funzioni.ToArray(typeof(Funzione));

            Funzione canCreateFuntion = functions.Where(e => e.codice == "FASC_NUOVO").FirstOrDefault();

            // Se  non è abilitato, viene interrotta la procedura e restituito un unico
            // risultato negativo
            if (canCreateFuntion == null)
                result.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = "Utente non abilitato alla creazione di fascicoli."
                });
            else
            {
                try
                {
                    // 1. Creazione del file temporaneo in cui poggiare il foglio 
                    // excel contenente i dati sui fascicoli da importare
                    completePath = ImportUtils.CreateTemporaryFile(content, modelPath, fileName);

                    // 2. Creazione dei fascicoli
                    result = CreateProjects(completePath, userInfo, role, serverPath, isEnabledSmistamento);

                    // 3. Cancellazione file temporaneo
                    ImportUtils.DeleteTemporaryFile(completePath);
                }
                catch (Exception e)
                {
                    // Se il file è stato creato, cancellazione
                    if (!String.IsNullOrEmpty(completePath))
                        ImportUtils.DeleteTemporaryFile(completePath);

                    // Creazione di un nuovo risultato con i dettagli dell'eccezione
                    result.Add(new ImportResult()
                        {
                            Outcome = ImportResult.OutcomeEnumeration.KO,
                            Message = e.Message
                        });
                }
            }

            // 4. Restituzione del risultato
            return result;

        }

        /// <summary>
        /// Funzione per l'estrazione dei dati relativi a fascicoli da importare da
        /// un foglio Excel
        /// </summary>
        /// <param name="content">Il contenuto del file Excel da cui estrarre i dati</param>
        /// <param name="modelPath">Il percorso su filesystem in cui salvare temporaneamente il file Excel</param>
        /// <param name="fileName">Il nome da attribuire al file Excel temporaneo</param>
        /// <param name="provider">Il provider Excel da utilizzare per la connessione</param>
        /// <param name="extendedProperty">Le proprietà estese da utilizzare per la connessione al file Excel</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <returns>La lista dei dati estratti dal foglio Excel</returns>
        public List<ProjectRowData> ReadDataFromExcel(
            byte[] content,
            string modelPath,
            string fileName,
            string provider,
            string extendedProperty,
            InfoUtente userInfo,
            Ruolo role)
        {
            // Il path completo del file temporaneo
            string tmpFilePath;

            // L'oggetto da restituire
            List<ProjectRowData> toReturn;

            // 1. Creazione del file
            tmpFilePath = ImportUtils.CreateTemporaryFile(content, modelPath, fileName);

            // 2. Estrazione dei dati
            try
            {
                toReturn = this.ExtractDataFromExcelFile(tmpFilePath, provider, extendedProperty, userInfo, role);
            }
            catch (Exception e)
            {
                throw new ImportProjectException(e.Message);
            }
            finally
            {
                // 3. Cancellazione del file
                ImportUtils.DeleteTemporaryFile(tmpFilePath);
            }

            // Restituzione del risultato
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'importazione di un fascicolo i cui dati sono descritti all'interno di un
        /// opportuno oggetto
        /// </summary>
        /// <param name="rowData">L'oggetto con i dati sul fascicolo da inserire</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="isEnabledSmistamento">True se è abilitato lo smistamento</param>
        /// <param name="serverPath">L'indirizzo web della WA</param>
        /// <returns>Il risultato dell'importazione</returns>
        public ImportResult ImportProject(
            ProjectRowData rowData,
            InfoUtente userInfo,
            Ruolo role,
            bool isEnabledSmistamento,
            string serverPath)
        {
            #region Dichiarazione Variabili

            // Il risultato dell'elaborazione
            ImportResult toReturn;

            // L'array delle funzione associate al ruolo
            Funzione[] functions;

            // Un booleano che indica se l'utente è abilitato alla creazione di fascicoli
            bool canCreateFuntion;

            #endregion

            // Creazione dell'oggetto da restituire e reperimento delle funzioni associate
            // al ruolo
            toReturn = new ImportResult();
            functions = (Funzione[])role.funzioni.ToArray(typeof(Funzione));

            // Verifica della possibilità da parte dell'utente di creare fascicoli
            canCreateFuntion = functions.Where(e => e.codice == "FASC_NUOVO").FirstOrDefault() != null;

            // Se  non è abilitato, viene interrotta la procedura e restituito un risultato
            // negativo
            if (!canCreateFuntion)
                toReturn = new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = "Utente non abilitato alla creazione di fascicoli."
                };
            else
            {
                try
                {
                    // Esecuzione dell'importazione
                    toReturn = this.ExecuteImportProject(rowData, isEnabledSmistamento, serverPath, role, userInfo);

                }
                catch (Exception e)
                {
                    // Creazione di un nuovo risultato con i dettagli dell'eccezione
                    toReturn = new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = e.Message
                    };
                
                }
            
            }

            // Impostazione del numero ordinale
            toReturn.Ordinal = rowData.OrdinalNumber;

            // Restituzione del risultato
            return toReturn;

        }

        #endregion

        #region Funzioni private di esecuzione delle operazione richieste dalle funzioni pubbliche

        /// <summary>
        /// Funzione per l'estrazione dei dati dal foglio Excel reperibile
        /// sul path filePath
        /// </summary>
        /// <param name="filePath">Il path da cui è possibile reperire il file Excel da cui estrarre i dati</param>
        /// <param name="extendedProperty">Le proprietà estese da utilizzare con la connessione la foglio</param>
        /// <param name="provider">Il provider da utilizzare per la connessione</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <returns>La lista degli oggetti con i dati estratti dal foglio Excel</returns>
        private List<ProjectRowData> ExtractDataFromExcelFile(
            string filePath, 
            string provider, 
            string extendedProperty, 
            InfoUtente userInfo, 
            Ruolo  role)
        {
            #region Dichiarazione Variabili

            // L'oggetto da restituire
            List<ProjectRowData> toReturn;

            // L'oggetto per la gestione della connessione al foglio Excel
            OleDbConnection oleConnection;

            #endregion

            // Creazione dell'oggetto da restituire
            toReturn = new List<ProjectRowData>();

            // Connessione al foglio Excel
            oleConnection = ImportUtils.ConnectToFile(provider, extendedProperty, filePath);

            // Estrazione dati
            toReturn = this.GetListOfProjectsData(oleConnection, userInfo, role);

            // Disconnessione dal foglio Excel
            ImportUtils.CloseConnection(oleConnection);

            // Restituzione della lista dei dati estratti
            return toReturn;
            
        }

        /// <summary>
        /// Funzione per la lettura dei dati dal foglio Excel
        /// </summary>
        /// <param name="oleConnection">L'oggetto per la gestione della connessione al foglio Excel</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <returns>La lista dei dati estratti dal foglio Excel</returns>
        private List<ProjectRowData> GetListOfProjectsData(
            OleDbConnection oleConnection, 
            InfoUtente userInfo, 
            Ruolo role)
        {
            #region Dichiarazione Variabili

            // L'oggetto da restituire
            List<ProjectRowData> toReturn;

            // La query da eseguire
            OleDbCommand oleCommand;

            // L'oggetto con cui leggere i dati estratti dal foglio Excel
            OleDbDataReader dataReader;

            #endregion

            #region Selezione dei dati da estrarre

            try
            {
                // Creazione della query per la selezione dei dati sui fascicoli da importare
                oleCommand = new OleDbCommand("SELECT * FROM [Fascicoli$]", oleConnection);

                // Esecuzione della query per il recupero dei dati
                dataReader = oleCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                throw new ImportProjectException("Errore durante l'estrazione dei dati. Dettagli: " + e.Message);
            }

            #endregion

            // Creazione dell'oggetto da restituire
            toReturn = new List<ProjectRowData>();

            #region Estrazione dati

            // Finchè ci sono dati da estrarre...
            while(dataReader.Read())
            {
                // ...estrazione dei dati dalla riga attuale e sua aggiunta alla lista da restituire
                toReturn.Add(this.ReadDataFromCurrentRow(dataReader, userInfo, role));

            }

            #endregion

            // Restituzione dell'oggetto
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'importazione di un fascicolo
        /// </summary>
        /// <param name="rowData">L'oggetto con le informazioni sul fascicolo da importare</param>
        /// <param name="isSmistamentoEnabled"></param>
        /// <param name="serverPath"></param>
        /// <param name="isEnabledSmistamento">True se è abilitato lo smistamento</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <returns>Il risultato dell'importazione</returns>
        private ImportResult ExecuteImportProject(
            ProjectRowData rowData,
            bool isEnabledSmistamento,
            string serverPath,
            Ruolo role,
            InfoUtente userInfo)
        {
            #region Dichiarazione Variabili

            // L'oggetto da restituire
            ImportResult toReturn;

            // Lista temporanea dei problemi
            List<string> tempProblems;

            // Il risultato della validazione
            bool validationResult;

            // Il nodo titolario in cui creare il fascicolo
            OrgNodoTitolario titolarioNode;

            // Il registro
            Registro registry;

            // L'oggetto classificazione
            Classificazione classification;

            // L'oggetto con la descrizione del fascicolo
            Fascicolo project;

            // Identificativo del registro da utilizzare per la creazione del fascicolo
            String registryId;

            #endregion

            // Creazione dell'oggetto da restituire
            toReturn = new ImportResult();

            // 1. Validazione dei dati
            validationResult = this.CheckDataValidity(rowData, out tempProblems);

            // Aggiunta dei problemi alla lista dei problemi
            toReturn.OtherInformation.AddRange(tempProblems);

            // Se i dati non sono validi, il risultato è negativo
            if (!validationResult)
            {
                toReturn.Outcome = ImportResult.OutcomeEnumeration.KO;
                toReturn.Message = "Alcuni parametri specificati non sono validi. Controllare i dettagli per maggiori informazioni.";
            }
            else
            {
                // Altrimenti si procede con le operazioni di creazione
                // 2. Prelevamento del nodo titolario di interesse
                titolarioNode = this.GetTitolarioNodeObject(rowData);

                // Reperimento delle informazioni sul registro
                registryId = ImportUtils.GetRegistrySystemId(rowData.RegistryCode, rowData.AdminCode);

                // Creazione dell'oggetto classificazione
                classification = this.GetClassificationObject(rowData, titolarioNode);

                tempProblems = new List<string>();
                // Creazione dell'oggetto fascicolo
                project = this.CreateProjectObject(
                    classification,
                    rowData,
                    titolarioNode,
                    registryId,
                    role,
                    userInfo,
                    out tempProblems,
                    isEnabledSmistamento);

                // Aggiunta degli eventuali errori alla lista dei dettagli
                // del seguente risultato
                toReturn.OtherInformation.AddRange(tempProblems);

                // Creazione del fascicolo
                this.CreateProject(classification, project, userInfo, role);

                // Trasmissione del fascicolo
                if (rowData.TransmissionModelCode != null)
                {
                    tempProblems = this.TransmitProject(
                        project,
                        rowData,
                        userInfo,
                        role,
                        serverPath);

                    // Aggiunta degli eventuali errori alla lista dei dettagli
                    // del risultato
                    toReturn.OtherInformation.AddRange(tempProblems);
                }

                // Salvataggio del fascicolo nell'area di lavoro
                if (rowData.InWorkingArea)
                {
                    tempProblems = this.SaveProjectInWorkingArea(project, userInfo, role);

                    // Aggiunta degli eventuali problemi alla lista
                    // dei dettagli del risultato
                    toReturn.OtherInformation.AddRange(tempProblems);

                }

                // Se si sono verificati errori durante l'elaborazione,
                // il risultato è un Fallimento
                if (toReturn.OtherInformation.Count > 0)
                {
                    // Impostazione del risultato a fallimento
                    toReturn.Outcome = ImportResult.OutcomeEnumeration.Warnings;

                    // Impostazione del messaggio
                    toReturn.Message = String.Format("Fascicolo creato con successo. Numero assegnato: {0}.",
                            project.codice);

                }
                else
                {
                    // Altrimenti il risultato è positivo
                    toReturn.Outcome = ImportResult.OutcomeEnumeration.OK;

                    // Il messaggio è un messaggio di successo
                    toReturn.Message = String.Format("Fascicolo creato con successo. Numero assegnato: {0}.",
                            project.codice);

                }

                // Impostazione dell'ordinale
                toReturn.Ordinal = rowData.OrdinalNumber;

            }
                
            // Restituzione del risultato
            return toReturn;

        }

        #endregion

        #region Funzioni di supporto
        
        /// <summary>
        /// Funzione per l'estrazione dei dati dalla riga attuale
        /// </summary>
        /// <param name="dataReader">L'oggetto da cui estrarre i dati</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <returns>L'oggetto con i dati estratti dalla riga attuale</returns>
        private ProjectRowData ReadDataFromCurrentRow(OleDbDataReader dataReader, InfoUtente userInfo, Ruolo role)
        {
            // L'oggetto da restituire
            ProjectRowData rowData = new ProjectRowData();

            // Prelevamento dell'ordinale
            rowData.OrdinalNumber = dataReader["Ordinale"].ToString().Trim();

            // Prelevamento del codice amministrazione
            rowData.AdminCode = dataReader["Codice amministrazione"].ToString().Trim();

            // Prelevamento del codice registro
            rowData.RegistryCode = dataReader["Codice Registro"].ToString().Trim();

            // Prelevamento del codice RF
            rowData.RFCode = dataReader["Codice RF"].ToString().Trim();

            // Prelevamento della descrizione
            rowData.Description = dataReader["Descrizione"].ToString().Trim();

            // Prelevamento del nome titolario
            //rowData.Titolario = dataReader["Titolario"].ToString().Trim();

            // Prelevamento del codice nodo
            rowData.NodeCode = dataReader["Codice nodo"].ToString().Trim();

            // Prelevamento del numero fascicolo
            rowData.ProjectNumber = dataReader["Numero Fascicolo"].ToString().Trim();

            // Prelevamento della data di creazione
            //rowData.CreationDate = dataReader["Data creazione"].ToString().Trim();

            // Prelevamento del valore che indica se il fascicolo deve essere immesso in ADL
            rowData.InWorkingArea = !String.IsNullOrEmpty(dataReader["ADL"].ToString()) &&
                dataReader["ADL"].ToString().ToUpper().Trim() == "SI";

            // Lettura delle note
            if (!String.IsNullOrEmpty(dataReader["Note"].ToString()))
            {
                // Creazione della nota
                rowData.Note = new InfoNota(dataReader["Note"].ToString().Trim());

                //Impostazione della visibilità Tutti
                rowData.Note.TipoVisibilita = TipiVisibilitaNotaEnum.Tutti;

                // Creazione dell'oggetto utilizzato per memorizzare le informazioni 
                // sull'utente creatore
                rowData.Note.UtenteCreatore = new InfoUtenteCreatoreNota();

                // Impostazione della descrizione del ruolo
                rowData.Note.UtenteCreatore.DescrizioneRuolo = role.descrizione;

                // Impostazione della descrizione dell'utente
                rowData.Note.UtenteCreatore.DescrizioneUtente = userInfo.userId;

                // Impostazione dell'id del ruolo
                rowData.Note.UtenteCreatore.IdRuolo = role.systemId;

                // Impostazione dell'id dell'utente
                rowData.Note.UtenteCreatore.IdUtente = userInfo.userId;

                // La nota deve essere inserita
                rowData.Note.DaInserire = true;

                // Impostazione della data di creazione
                rowData.Note.DataCreazione = DateTime.Now;

            }

            // Lettura dei codici dei modelli di trasmissione
            if (!String.IsNullOrEmpty(dataReader["Codice Modello Trasmissione"].ToString()))
                rowData.TransmissionModelCode = dataReader["Codice Modello Trasmissione"].
                    ToString().Trim().Split(';');

            // Lettura del nome della tipologia di fascicolo
            rowData.ProjectTipology = dataReader["Tipologia Fascicolo"].ToString().Trim();

            // Lettura dei dati relativi alla profilazione
            ReadProfilationData(dataReader, rowData);

            // Restituzione dell'oggetto
            return rowData;

        }

        /// <summary>
        /// Funzione per l'estrazione dei dati relativi ai campi profilati
        /// </summary>
        /// <param name="dataReader">L'oggetto da cui estrarre i dati</param>
        /// <param name="rowData">L'oggetto in cui salvare i dati estratti</param>
        private void ReadProfilationData(OleDbDataReader dataReader, ProjectRowData rowData)
        {
            // Lettura dei valori assegnati ai campi profilati
            // Per ogni colonna presente nel data reader
            for (int i = 0; i < dataReader.FieldCount; i++)
                // ...se il nome della colonna comincia per Campo...
                if (dataReader.GetName(i).Trim().ToUpper().StartsWith("CAMPO"))
                {
                    if (!String.IsNullOrEmpty(dataReader[i].ToString()))
                    {
                        // ...si spezza la stringa nelle sue due componenti
                        string[] fieldInformation = dataReader[i].ToString().Split('=');

                        // ...se fieldInformation non contiene due valori, eccezione
                        if (fieldInformation.Length != 2)
                        {
                            // Nel caso in cui l'array degli argomenti contiene un solo elemento
                            // che comincia con #Error, molto probabilmente si tratta di un errore
                            // di lettura restituito dal driver Excel. In questo caso viene lanciata un'eccezione
                            // appositamente costruita (per sicurezza viene anche inviata la stringa completa
                            // in quanto potrebbe capitare che in certi casi sia riportato anche un numero identificativo
                            // dell'errore
                            if (fieldInformation.Length == 1 && fieldInformation[0].StartsWith("#Error"))
                                throw new Exception(
                                String.Format("Errore durante l'estrazione del contenuto della cella '{0}' relativa al fascicolo con ordinale {1}. Dettaglio: {2}",
                                    dataReader.GetName(i),
                                    rowData.OrdinalNumber,
                                    fieldInformation[0]));

                            throw new ImportProjectException(
                                String.Format(
                                    "Il campo '{0}' non è stato valorizzato correttamente. Questi campi devono contenere coppie Chiave=Valore o Chiave=Valore1;Valore2...",
                                    dataReader.GetName(i)));
                        }

                        // ...si aggiunge al dizionario dei campi profilati la chiave
                        // ed i valori
                        rowData.AddProfilationField(
                            fieldInformation[0],
                            fieldInformation[1].Split(';'));

                    }

                }

        }

        /// <summary>
        /// Funzione per la validazione dei dati letti da una riga del foglio excel
        /// </summary>
        /// <param name="rowData">L'oggetto con i dati letti da una riga del foglio excel</param>
        /// <param name="notValidData">Una lista in cui aggiungere l'insieme dei dati non validi</param>
        /// <returns>True se la validazione passa, false altrimenti</returns>
        private bool CheckDataValidity(ProjectRowData rowData, out List<String> notValidData)
        {
            // La lista con gli errori da restituire
            List<String> problems = new List<string>();

            // Il risultato del controllo di validità
            bool validationResult = true;

            // L'ordinale è obbligatorio
            if (String.IsNullOrEmpty(rowData.OrdinalNumber))
            {
                validationResult = false;
                problems.Add("Campo 'Ordinale' obbligatorio.");
            }

            // Il codice amministrazione è obbligatorio
            if (String.IsNullOrEmpty(rowData.AdminCode))
            {
                validationResult = false;
                problems.Add("Campo 'Codice amministrazione' obbligatorio.");
            }

            // Il codice registro è obbligatorio
            if (String.IsNullOrEmpty(rowData.RegistryCode))
            {
                validationResult = false;
                problems.Add("Campo 'Codice Registro' obbligatorio.");
            }

            // La descrizione è obbligatoria
            if (String.IsNullOrEmpty(rowData.Description))
            {
                validationResult = false;
                problems.Add("Campo 'Descrizione' obbligatorio.");
            }

            // Il nodo di titolario
            if (String.IsNullOrEmpty(rowData.NodeCode))
            {
                validationResult = false;
                problems.Add("Campo 'Codice nodo' obbligatorio.");
            }

            // Impostazione della lista di problemi di vlaidazione
            notValidData = problems;

            // Restituzione del risultato della validazione
            return validationResult;

        }

        /// <summary>
        /// Funzione utilizzata per reperire le informazioni sul titolario
        /// </summary>
        /// <param name="rowData">L'oggetto con le informazioni lette da una riga del foglio excel</param>
        /// <returns>L'oggetto con le informazioni sul nodo di titolario</returns>
        private OrgNodoTitolario GetTitolarioNodeObject(ProjectRowData rowData)
        {
            #region Variabili

            // L'oggetto da restituire
            OrgNodoTitolario titolarioNode = null;

            // L'oggetto a cui richiedere l'id dell'amministrazione
            DocsPaDB.Query_DocsPAWS.Amministrazione administration =
                new DocsPaDB.Query_DocsPAWS.Amministrazione();

            // L'id dell'amministrazione
            string administrationID = String.Empty;

            // Il titolario
            OrgTitolario titolario = null;

            // La stringa per recuperare le informazioni sul nodo di titolario
            StringBuilder queryReg = new StringBuilder(" AND (ID_REGISTRO ");

            #endregion

            // Prelevamento del system id dell'amministrazione a partire dal suo codice
            administrationID = ImportUtils.GetAdministrationId(rowData.AdminCode);

            // Prelevamento del titolario
            titolario = ImportUtils.GetTitolarioObj(null, administrationID);

            try
            {
                // Se è specificato un codice registro, si provvede al reperimento
                // del system id ad esso assiciato
                if (!String.IsNullOrEmpty(rowData.RegistryCode))
                    queryReg.AppendFormat(" = {0} OR ID_REGISTRO ",
                        Utenti.RegistriManager.getIdRegistro(rowData.AdminCode, rowData.RegistryCode));

                queryReg.Append("IS NULL)");
            }
            catch (Exception e)
            {
                throw new ImportProjectException("Errore durante il reperimento delle informazioni sul registro " +
                    rowData.RegistryCode);
            }

            try
            {
                // Recupero delle informazioni sul nodo titolario
                titolarioNode = Amministrazione.TitolarioManager.getNodoTitolario(rowData.NodeCode,
                    rowData.AdminCode, queryReg.ToString(), titolario.ID);
            }
            catch (Exception e)
            {
                throw new ImportProjectException("Errore durante il caricamento delle informazioni sul nodo titolario con codice "
                    + rowData.NodeCode);
            }

            // Se non è stato recuperato il nodo...
            if (titolarioNode == null)
                // ...si solleva un'eccezione
                throw new ImportProjectException("Impossibile recuperare le informazioni sul nodo di titolario con codice "
                    + rowData.NodeCode);

            return titolarioNode;

        }

        /// <summary>
        /// Funzione per il reperimento delle informazioni sul registro
        /// </summary>
        /// <param name="registryId">L'id del registro di cui caricare le informazioni</param>
        /// <returns>Le informazioni sul registro</returns>
        private Registro GetRegistryObject(string registryId)
        {
            // Il registro da restituire
            Registro registy;

            try
            {
                // Recupero delle informazioni sul registro specificato
                registy = RegistriManager.getRegistro(registryId);
            }
            catch (Exception e)
            {
                throw new ImportProjectException("Errore durante il recupero delle informazioni sul registro");
            }

            // Restituzione delle informazioni sul registro
            return registy;

        }

        /// <summary>
        /// Creazione dell'oggetto classificazione
        /// </summary>
        /// <param name="rowData">L'oggetto con le informazioni lette dalla riga del foglio excel</param>
        /// <param name="registro">Il registro</param>
        /// <param name="nodoTitolario">Il nodo di titolario</param>
        /// <returns>L'oggetto classificazione</returns>
        private Classificazione GetClassificationObject(ProjectRowData rowData, OrgNodoTitolario titolarioNode)
        {
            #region Variabili

            // L'oggetto da restituire
            Classificazione classification = null;

            #endregion

            // Creazione dell'oggetto classificazione da restituire
            classification = new Classificazione();

            // Impostazione del codice nodo
            classification.codice = rowData.NodeCode;

            // Impostazione del codice da assegnare al fascicolo
            if (String.IsNullOrEmpty(rowData.ProjectNumber))
                classification.codUltimo = FascicoloManager.getFascNumRif(
                    titolarioNode.ID, 
                    ImportUtils.GetRegistrySystemId(rowData.RegistryCode, rowData.AdminCode));
            else
                classification.codUltimo = rowData.ProjectNumber;

            // Impostazione descrizione
            classification.descrizione = rowData.Description;

            // Impostazione del system id
            classification.systemID = titolarioNode.ID;

            // Impostazione del codice di livello
            classification.varcodliv1 = titolarioNode.CodiceLivello;

            // Restituzine dell'oggetto classificazione
            return classification;

        }

        /// <summary>
        /// Questa funzione si occupa di  creare l'oggetto con le informazioni sul fascicolo da
        /// creare
        /// </summary>
        /// <param name="classification">L'oggetto classificazione</param>
        /// <param name="isEnabledSmistamento">True se è abilitato lo smistamento</param>
        /// <param name="profilationProblems">Problemi emersi durante la profilazione</param>
        /// <param name="registryId">Identificativo del registro da utilizzare per la creazione del fascicolo</param>
        /// <param name="role">Il ruolo con cui è stata avviata la procedura</param>
        /// <param name="rowData">L'oggetto con le informazioni sul fascicolo da creare</param>
        /// <param name="titolarioNode">Il nodo di titolario</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        private Fascicolo CreateProjectObject(Classificazione classification, ProjectRowData rowData, OrgNodoTitolario titolarioNode, String registryId, Ruolo role, InfoUtente userInfo, out List<string> profilationProblems, bool isEnabledSmistamento)
        {
            #region Variabili

            // La lista di stringhe con i problemi emersi durante
            // la compilazione dei dati relativi alla profilazione del 
            // documento
            List<String> profilationProblem = new List<string>();

            // L'oggetto da restituire
            Fascicolo project = null;

            #endregion

            // Creazione dell'oggetto fascicolo
            project = new Fascicolo();

            // Impostazione della descrizione
            project.descrizione = rowData.Description;

            // Impostazione del codice da attribuire al fascicolo
            project.codUltimo = classification.codUltimo;

            // Impostazione del flag cartaceo
            project.cartaceo = false;

            // Impostazione del valore privato
            project.privato = "0";

            // Impostazione delle note
            if (rowData.Note != null)
                project.noteFascicolo = new InfoNota[1] { rowData.Note };

            // Impostazione della data di apertura
            //if (String.IsNullOrEmpty(rowData.CreationDate))
                project.apertura = DateTime.Today.ToString("dd/MM/yyyy");
            //else
            //    project.apertura = rowData.CreationDate;

            // Impostazione dell'id del registro del nodo titolario
            project.idRegistroNodoTit = titolarioNode.IDRegistroAssociato;

            // Impostazione dell'id del registro
            project.idRegistro = registryId;

            // Compilazione dei dati relativi alla profilazione
            if (!String.IsNullOrEmpty(rowData.ProjectTipology))
                profilationProblem = CompileProfilationFields(
                    rowData,
                    project,
                    role,
                    userInfo,
                    isEnabledSmistamento);

            // Impostazione della lista con gli eventuali errori di profilazione
            profilationProblems = profilationProblem;

            // Restituzione dell'oggetto creato
            return project;

        }

        /// <summary>
        /// Funzione per la compilazione delle informazioni sui campi profilati
        /// </summary>
        /// <param name="rowData">L'oggetto con i dati sul fascicolo da creare</param>
        /// <param name="project">L'oggetto fascicolo con i dati identificativi del fascicolo da creare</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="isEnabledSmistamento">True se è abilitato lo smitamamento</param>
        /// <returns>Lista dei problemi emersi durante la compilazione dei campi profilati</returns>
        private List<string> CompileProfilationFields(ProjectRowData rowData, Fascicolo project, Ruolo role, InfoUtente userInfo, bool isEnabledSmistamento)
        {
            #region Dichiarazione variabili

            // La lista dei template
            ArrayList templates = null;

            // Il template 
            Templates template = null;

            // L'oggetto a cui richiedere l'id dell'amministrazione
            DocsPaDB.Query_DocsPAWS.Amministrazione administration =
                new DocsPaDB.Query_DocsPAWS.Amministrazione();

            // L'id dell'amministrazione
            string adminID = String.Empty;

            // I diritti di visibilità sui campi della tipologia
            ArrayList visibilityRights = null;

            // La lista degli errori emersi durante la compilazione dei
            // dati profilati
            List<string> toReturn = new List<string>();

            #endregion

            // 1. Calcolo dell'id dell'amministrazione
            adminID = administration.GetIDAmm(rowData.AdminCode);

            // 2. Prelevamento della lista dei template creati per l'amministrazione
            templates = ProfilazioneFascicoli.getTipoFascFromRuolo(adminID, role.idGruppo, "2");

            // 3. Ricerca del template con nome uguale a quello richiesto
            foreach (Templates t in templates)
                if (t.DESCRIZIONE.ToUpper().TrimStart().TrimEnd() == rowData.ProjectTipology.ToUpper().TrimStart().TrimEnd())
                    template = ProfilazioneFascicoli.getTemplateFascById(t.SYSTEM_ID.ToString());

            // Se il template non è stato recuperato con successo, 
            // eccezione
            if (template == null)
                throw new ImportProjectException(String.Format(
                    "Non è stato possibile recuperare le informazioni sul template {0}",
                    rowData.ProjectTipology));

            // Altrimenti si procede con la compilazione dei campi profilati
            // Prelevamento dei diritti di visibilità sui campi della tipologia
            visibilityRights = ProfilazioneFascicoli.getDirittiCampiTipologiaFasc(
                role.idGruppo,
                template.SYSTEM_ID.ToString());

            // Se tutto è andato bene, si può procedere alla compilazione dei campi
            // profilati
            project.template = CompileProfilationObjects(
                    rowData,
                    role,
                    userInfo,
                    template,
                    (AssDocFascRuoli[])visibilityRights.ToArray(typeof(AssDocFascRuoli)),
                    adminID,
                    isEnabledSmistamento,
                    out toReturn);

            // Restituzione degli eventuali problemi
            return toReturn;

        }

        /// <summary>
        /// Funzione per la compilazione dei dati relativi ad un oggetto dei campi profilati
        /// </summary>
        /// <param name="rowData">L'oggetto con i dati sul fascicolo da creare</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="template">Il template</param>
        /// <param name="visibilityRights">I diritti di visibilità</param>
        /// <param name="adminID">L'id dell'amministrazione</param>
        /// <param name="isEnabledSmistamento">True se è abilitato lo smistamento</param>
        /// <param name="problems">La lista dei problemi emersi in fase di compilazione</param>
        /// <returns>Il template compilato</returns>
        private Templates CompileProfilationObjects(
            ProjectRowData rowData,
            Ruolo role,
            InfoUtente userInfo,
            Templates template,
            AssDocFascRuoli[] visibilityRights,
            string adminID,
            bool isEnabledSmistamento,
            out List<string> problems)
        {
            #region Dichiarazione variabili

            // La lista degli eventuali errori
            List<string> toReturn = null;

            // La lista dei valori associati ad una determinata
            // etichetta
            string[] fieldValues = null;

            // I diritti associati ad un determinato campo
            AssDocFascRuoli rights = null;

            #endregion

            // Creazione della lista dei problemi
            toReturn = new List<string>();

            // Compilazione degli oggetti del template
            // Per ogni oggetto del template
            foreach (OggettoCustom obj in template.ELENCO_OGGETTI)
            {
                // Recupero delle informazioni sui diritti relativi
                // all'oggetto obj
                rights = visibilityRights.Where(
                    e => e.ID_OGGETTO_CUSTOM == obj.SYSTEM_ID.ToString()).FirstOrDefault();

                // Recupero dei dati per la compilazione dell'oggetto
                // dal dizionario dei dati di profilazione contenuti nella
                // riga del foglio excel in esame
                fieldValues = rowData.GetProfilationField(obj.DESCRIZIONE.ToUpper());

                // Se fieldValues è valorizzato...
                if (fieldValues != null)
                {
                    // ...compilazione del campo profilato
                    toReturn.AddRange(ImportUtils.CompileProfilationField(
                        obj,
                        rights,
                        fieldValues,
                        role,
                        userInfo,
                        rowData.RFCode != null ? rowData.RFCode : String.Empty,
                        adminID,
                        rowData.RegistryCode != null ? rowData.RegistryCode : String.Empty,
                        isEnabledSmistamento));

                }
                else
                    // Altrimenti se l'utente ha diritti di modifica sul campo e il campo è
                    // obbligatorio, viene lanciata un'eccezione
                    if (rights != null && rights.INS_MOD_OGG_CUSTOM == "1" &&
                        obj.CAMPO_OBBLIGATORIO == "SI")
                        throw new ImportProjectException(String.Format(
                            "Errore durante la compilazione dei campi profilati. Il campo di tipologia '{0}' è obbligatorio ma non risulta valorizzato nel foglio",
                            obj.DESCRIZIONE));

            }

            // Impostazione della lista dei problemi
            problems = toReturn;

            // Restituzione del template compilato
            return template;

        }

        /// <summary>
        /// Funzione per la creazione del fascicolo
        /// </summary>
        /// <param name="classification">L'oggetto classificazione</param>
        /// <param name="project"></param>
        /// <param name="userInfo"></param>
        /// <param name="role"></param>
        private void CreateProject(Classificazione classification, Fascicolo project, InfoUtente userInfo, Ruolo role)
        {
            #region Variabili

            // L'oggetto utilizzato per ottenere informazioni sul fascicolo
            ResultCreazioneFascicolo creationResult;

            #endregion

            // Creazione dell'oggetto utilizzato per verificare il risultato
            // della creazione
            creationResult = new ResultCreazioneFascicolo();

            // Creazione del fascicolo
            Fascicolo fasc = FascicoloManager.newFascicolo(classification,
                project,
                userInfo,
                role,
                false,
                out creationResult);

            if (fasc != null)
                BusinessLogic.UserLog.UserLog.WriteLog(userInfo, "FASCICOLAZIONENEWFASCICOLO", fasc.systemID, string.Format("{0} {1}", "Cod. Fascicolo:", fasc.codice), DocsPaVO.Logger.CodAzione.Esito.OK);

            // Lancio di un'eccezione appropriata nel caso di fascicolo già presente,
            // fascicicolatura non presente o errore generico
            switch (creationResult)
            {
                case ResultCreazioneFascicolo.FASCICOLO_GIA_PRESENTE:
                    throw new ImportProjectException(
                        String.Format("Fascicolo {0} già presente.",
                             project.codice));
                    break;
                case ResultCreazioneFascicolo.FORMATO_FASCICOLATURA_NON_PRESENTE:
                    throw new ImportProjectException(
                       String.Format("Fascicolatura per il fascicolo {0} non presente.",
                            project.codice));
                    break;
                case ResultCreazioneFascicolo.GENERIC_ERROR:
                    throw new ImportProjectException("Errore generico durante la creazione del fascicolo.");
                    break;

            }

        }

        /// <summary>
        /// Funzione per la trasmissione del fascicolo
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <param name="rowData"></param>
        /// <param name="registry"></param>
        /// <param name="userInfo"></param>
        /// <param name="role"></param>
        /// <param name="serverPath"></param>
        /// <returns></returns>
        private List<string> TransmitProject(Fascicolo fascicolo, ProjectRowData rowData, InfoUtente userInfo, Ruolo role, string serverPath)
        {
            #region Dichiarazione variabili

            // La lista dei problemi
            List<String> problems = new List<string>();

            // Il modello di trasmissione da utilizzare per inviare il fascicolo
            ModelloTrasmissione transmModel = null;

            // Il risultato dell'operazione di invio
            bool trasmRes;

            // Un valore utilizzato per tenere traccia del fatto che si è
            // verificata un'eccezione
            bool haveException = false;

            // L'indice dell'ultimo carattere '_'
            int lastUnderscore;

            #endregion

            // Per ogni codice di modello di trasmissione...
            foreach (string modelCode in rowData.TransmissionModelCode)
            {
                // Azzeramento del flag eccezione
                haveException = false;

                // Azzeramento del flag trasmRes
                trasmRes = false;

                try
                {
                    // Prelevamento indice dell'ultimo _
                    lastUnderscore = modelCode.LastIndexOf('_');

                    // ...si prova a reperire il modello di trasmissione
                    // tramite il suo id
                    transmModel = ModelliTrasmissioni.getModelloByID(
                        role.idAmministrazione,
                        modelCode.Substring(lastUnderscore + 1));

                    // ...se il modello è per fascicoli si procede all'invio
                    // altrimenti non si può utilizzare il modello
                    if (transmModel.CHA_TIPO_OGGETTO == "F")
                        trasmRes = TrasmManager.TrasmissioneExecuteTrasmFascDaModello(
                            serverPath,
                            fascicolo,
                            transmModel,
                            userInfo);

                }
                catch (Exception e)
                {
                    haveException = true;

                }

                // Se si è verificata un'eccezione o se la trasmissione non è partita,
                // significa che si è verificato qualche problema
                if (!trasmRes || haveException)
                {
                    // Se trasmModel è valorizzato e non si riferisce a fascicoli
                    if (transmModel != null && transmModel.CHA_TIPO_OGGETTO != "F")
                        // Si segnala il problema all'utente
                        problems.Add(
                        String.Format(
                            "Il modello '{0}' non può essere utilizzato per trasmettere fascicoli",
                            modelCode.Trim()));
                    else
                        // altrimenti si segnala un errore generico
                        if (haveException)
                            problems.Add(
                                String.Format(
                                    "Errore durante l'invio delle trasmissioni del fascicolo '{0}' utilizzando il modello {1}",
                                    fascicolo.descrizione, modelCode.Trim()));
                }
            }

            // Restituzione dell'insieme dei problemi
            return problems;

        }

        /// <summary>
        /// Funzione per l'aggiunta di un fascicolo all'area di lavoro
        /// </summary>
        /// <param name="project"></param>
        /// <param name="userInfo"></param>
        /// <param name="role"></param>
        private List<string> SaveProjectInWorkingArea(Fascicolo project, InfoUtente userInfo, Ruolo role)
        {
            // L'eventuale messaggio di errore da restituire
            List<string> toReturn = new List<string>();

            // Se il ruolo attuale non è abilitato alla creazione del fascicolo
            // non si può procedere
            Funzione[] functions = (Funzione[])role.funzioni.ToArray(typeof(Funzione));

            Funzione canAddInADL = functions.Where(e => e.codice == "FASC_ADD_ADL").FirstOrDefault();

            if (canAddInADL == null)
                toReturn.Add("Con il ruolo corrente non è possibile aggiungere fascicoli nell'area di lavoro");
            else
            {
                try
                {
                    areaLavoroManager.execAddLavoroMethod(null, null, project.idRegistro, userInfo, project);
                }
                catch (Exception e)
                {
                    toReturn.Add("Il fascicolo è stato creato correttamente ma non è stato possibile aggiungerlo all'area di lavoro");
                }

            }

            // Restituzione dell'eventuale errore
            return toReturn;

        }

        #endregion

        #region Importazione fascicoli vecchia

        /// <summary>
        /// Funzione per la creazione dei fascicoli a partire da un foglio excel il cui path
        /// è passato per parametro
        /// </summary>
        private List<ImportResult> CreateProjects(string completePath, InfoUtente userInfo, Ruolo role, string serverPath, bool isEnabledSmistamento)
        {
            #region Dichiarazione variabili

            // La stringa di connessione al foglio excel
            string connectionString = String.Empty;

            // L'oggetto utilizzato per connettersi al foglio excel
            OleDbConnection oleConnection = null;

            // L'oggetto utilizzato per eseguire un comando sul file excel
            OleDbCommand oleCommand = null;

            // L'oggetto utilizzato per leggere i dati dal file excel
            OleDbDataReader dataReader = null;

            // Il risultato dell'elaborazione
            List<ImportResult> result = null;

            #endregion

            #region Connessione al foglio excel

            // Creazione della stringa di connessione
            //connectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=YES;\"",
            //    completePath);
            connectionString = String.Format("Provider=" + System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"] + "Data Source={0};Extended Properties=\"" + System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"] + "\"",
                completePath);

            try
            {
                // Creazione dell'oggetto per la connessione al foglio excel
                oleConnection = new OleDbConnection(connectionString);

                // Apertura connessione
                oleConnection.Open();
            }
            catch (Exception e)
            {
                // Viene rilanciata un'eccezione al livello superiore
                throw new Exception("Errore durante la connessione al file excel. Dettagli: " + e.Message);

            }

            #endregion

            #region Selezione dei dati sui fascicoli da importare

            try
            {
                // Creazione della query per la selezione dei dati sui fascicoli da importare
                oleCommand = new OleDbCommand("SELECT * FROM [Fascicoli$]", oleConnection);

                // Esecuzione della query per il recupero dei dati
                dataReader = oleCommand.ExecuteReader();

            }
            catch (Exception e)
            {
                // Chiusura del datareader
                if (dataReader != null && !dataReader.IsClosed)
                    dataReader.Close();

                // Chiusura della connessione
                oleConnection.Close();

                // Viene rilanciata un'eccezione al livello superiore
                throw new Exception("Errore durante il recupero dei dati sui fascicoli da importare. Dettagli: " + e.Message);
            }

            #endregion

            #region Importazione fascicoli

            result = ExecuteImportProject(dataReader, userInfo, role, serverPath, isEnabledSmistamento);

            #endregion

            #region Chiusura del data reader e della connessione

            try
            {
                // Chiusura del datareader
                if (dataReader != null && !dataReader.IsClosed)
                    dataReader.Close();

                // Chiusura della connessione
                oleConnection.Close();
            }
            catch (Exception e)
            {
                // Viene inserito un risultato negativo
                result.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = "Errore durante la chiusura della connessione al file excel. Dettagli: " + e.Message
                });
            }

            #endregion

            // Restituzione del risultato dell'importazione
            return result;
        }

        /// <summary>
        /// Funzione per l'esecuzione dell'importazione dei fascicoli
        /// </summary>
        private List<ImportResult> ExecuteImportProject(OleDbDataReader dataReader, InfoUtente userInfo, Ruolo role, string serverPath, bool isEnabledSmistamento)
        {
            #region Dichiarazione variabili

            // Il risultato del controllo di validità dati per la riga in analisi
            bool validParameters = false;

            // Un valore utilizzato per determinare se la lettura dei 
            // dati profilati ha avuto esito positivo
            bool validProfilationData = true;

            // L'oggetto utilizzato per memorizzare temporaneamente
            // i dati presenti nella riga correntemente in analisi
            ProjectRowData rowData = null;

            // Nodo di titolraio in cui inserire il fascicolo
            OrgNodoTitolario nodoTitolario = null;
            // Il registro 
            Registro registro = null;
            // L'oggetto con le informazioni di classificazione
            Classificazione classificazione = null;
            // L'oggetto con le informazioni sul fascicolo
            Fascicolo fascicolo = null;
            // La stringa con infomazioni utili al reperimento delle
            // informazioni sul nodo di titolario
            string regQuery = String.Empty;

            // Il risultato dell'elaborazione
            List<ImportResult> importResult = new List<ImportResult>();

            // Il numero di fascicoli importati
            int importedProject = 0;

            // Il numero di fascicoli non importati
            int notImportedProject = 0;

            // Il risultato dell'ultima operazione di creazione
            ImportResult temp = null;

            // La lista degli errori avvenuti in una delle fasi di creazione dei fascicoli
            List<string> creationProblems = null;

            // Identificativo del registro da utilizzare per la creazione del fascicolo
            String registryId;

            #endregion

            // Se non ci sono fascicoli da importare, il report conterrà
            // una sola riga con un messaggio che informi l'utente
            if (!dataReader.HasRows)
                importResult.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.OK,
                        Message = "Non sono stati rilevati fascicoli da importare."
                    });

            // Finchè ci sono dati da leggere
            while (dataReader.Read())
            {
                #region Inizializzazione variabili

                // Reset del controllore di validità dati
                validParameters = false;

                // Reset del controllore di validità campi profilati
                validProfilationData = true;

                // Creazione di un nuovo oggetto per contenere i 
                // dati letti dalla riga
                rowData = new ProjectRowData();

                // Creazione della lista dei problemi
                creationProblems = new List<string>();

                // Creazione del risultato dell'operazione di creazione
                temp = new ImportResult();

                #endregion

                #region Prelevamento dei dati dalla riga attuale

                try
                {
                    rowData = this.ReadDataFromCurrentRow(dataReader, userInfo, role);
                }
                catch (Exception e)
                {
                    // Aggiunta di un oggetto result con i dettagli sull'eccezione
                    importResult.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = e.Message,
                        Ordinal = rowData.OrdinalNumber
                    });

                    // L'esito della lettura dei dati profilati è negativo
                    validProfilationData = false;

                    // Un fascicolo non è stato importato
                    notImportedProject++;

                }

                #endregion

                #region Controllo validità campi obbligatori

                validParameters = CheckDataValidity(rowData, out creationProblems);

                // Aggiunta della lista degli errori alla lista dei dettagli
                // del risultato attuale
                temp.OtherInformation.AddRange(creationProblems);

                #endregion

                // Se gli eventuali dati profilati sono stati letti con successo
                // e se la validazione è passata...
                if (validProfilationData && validParameters)
                {
                    try
                    {
                        // Prelevamento del nodo titolario di interesse
                        nodoTitolario = GetTitolarioNodeObject(rowData);

                        // Creazione dell'oggetto classificazione
                        classificazione = GetClassificationObject(rowData, nodoTitolario);

                        // Reperimento delle informazioni sul registro
                        registryId = ImportUtils.GetRegistrySystemId(rowData.RegistryCode, rowData.AdminCode);

                        // Creazione dell'oggetto fascicolo
                        fascicolo = CreateProjectObject(
                            classificazione, 
                            rowData, 
                            nodoTitolario, 
                            registryId, 
                            role, 
                            userInfo, 
                            out creationProblems, 
                            isEnabledSmistamento);


                        // Aggiunta degli eventuali errori alla lista dei dettagli
                        // del seguente risultato
                        temp.OtherInformation.AddRange(creationProblems);

                        
                        // Creazione del fascicolo
                        CreateProject(classificazione, fascicolo, userInfo, role);

                        // Trasmissione del fascicolo
                        if (rowData.TransmissionModelCode != null)
                        {
                            creationProblems = TransmitProject(
                                fascicolo,
                                rowData,
                                userInfo,
                                role,
                                serverPath);

                            // Aggiunta degli eventuali errori alla lista dei dettagli
                            // del risultato
                            temp.OtherInformation.AddRange(creationProblems);
                        }

                        // Salvataggio del fascicolo nell'area di lavoro
                        if (rowData.InWorkingArea)
                        {
                            creationProblems = SaveProjectInWorkingArea(fascicolo, userInfo, role);

                            // Aggiunta degli eventuali problemi alla lista
                            // dei dettagli del risultato
                            temp.OtherInformation.AddRange(creationProblems);

                        }

                        // Se si sono verificati errori durante l'elaborazione,
                        // il risultato è un Fallimento
                        if (temp.OtherInformation.Count > 0)
                        {
                            // Impostazione del risultato a fallimento
                            temp.Outcome = ImportResult.OutcomeEnumeration.Warnings;

                            // Impostazione del messaggio
                            temp.Message = String.Format("Fascicolo creato con successo. Numero assegnato: {0}.",
                                    fascicolo.codice);

                        }
                        else
                        {
                            // Altrimenti il risultato è positivo
                            temp.Outcome = ImportResult.OutcomeEnumeration.OK;

                            // Il messaggio è un messaggio di successo
                            temp.Message = String.Format("Fascicolo creato con successo. Numero assegnato: {0}.",
                                    fascicolo.codice);

                        }

                        // Impostazione dell'ordinale
                        temp.Ordinal = rowData.OrdinalNumber;

                        // Agginta del risultato alla lista dei risultati
                        importResult.Add(temp);

                        // Un fascicolo è stato importato
                        importedProject++;

                    }
                    catch (Exception e)
                    {
                        // Viene creato un risultato negativo
                        importResult.Add(new ImportResult()
                        {
                            Outcome = ImportResult.OutcomeEnumeration.KO,
                            Message = e.Message,
                            Ordinal = rowData.OrdinalNumber
                        });

                        // Un fascicolo non è stato importato
                        notImportedProject++;

                    }

                }
                else
                    if (validProfilationData)
                    {
                        // Altrimenti viene aggiunto alla lista dei risultati
                        // un nuovo risultato negativo e ne viene impostata la descrizione
                        importResult.Add(new ImportResult()
                            {
                                Outcome = ImportResult.OutcomeEnumeration.KO,
                                Message = "Alcuni parametri obbligatori non sono validi.",
                                OtherInformation = creationProblems,
                                Ordinal = rowData.OrdinalNumber
                            });

                        // Un fascicolo non è stato importato
                        notImportedProject++;

                    }

            }

            // Aggiunta di un'ultima riga contenente il totale dei fascicoli
            // importati e non importati
            importResult.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.OK,
                    Message = String.Format("Fascicoli importati: {0}; Fascicoli non importati: {1}",
                        importedProject, notImportedProject)
                });

            // Restituzione del report sui risultati
            return importResult;

        }

        
        #endregion

    }
}
