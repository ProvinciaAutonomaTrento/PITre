using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using DocsPaVO.utente;
using DocsPaVO.documento;
using DocsPaVO.PrjDocImport;
using BusinessLogic.Import.ImportDocuments.DocumentType;
using System.Text.RegularExpressions;
using System.IO;
using BusinessLogic.Documenti;

namespace BusinessLogic.Import.ImportDocuments.DocumentType
{
    /// <summary>
    /// Questa classe si occupa dell'aggiunta degli allegati.
    /// </summary>
    class Attachment
    {
        #region Funzioni pubbliche
        
        /// <summary>
        /// Funzione per l'importazione degli allegati
        /// </summary>
        /// <param name="dataReader">L'oggetto da cui leggere i dati</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="ftpAddress">L'indirizzo della cartella condivisa in cui reperire il file da associare all'allegato</param>
        /// <param name="resultContainer">L'oggetto con il report di importazione</param>
        /// <returns>Il report relativo all0'importazione degli allegati</returns>
        public List<ImportResult> ImportDocuments(
            OleDbDataReader dataReader, 
            InfoUtente userInfo, 
            Ruolo role,
            string ftpAddress, 
            ref ResultsContainer resultContainer,
            String ftpUsername,
            String ftpPassword)
        {
            #region Dichiarazione variabili

            // Il report da restituire
            List<ImportResult> toReturn = new List<ImportResult>();

            // L'oggetto contente i dati sui documenti letti dalla riga del foglio excel
            // in analisi
            DocumentRowData rowData = null;

            // Valore booleano utilizzato per indicare la
            // validità dei dati
            bool validParameters = false;

            // Valore booleano utilizzato per indicare se
            // ci sono stati problemi durante la lettura dei dati dalla
            // riga del foglio excel
            bool noErrorsReadingRowData = false;

            // La lista dei problemi rilevati durante le fasi di
            // creazione di un documento
            List<string> creationProblems = null;

            // Un oggetto utilizzato per contenere temporaneamente
            // la riga di report per la creazione di un documento
            ImportResult temp = null;

            // Un valore intero utilizzato per contare gli allegati
            // importati
            int importedAttachments = 0;

            // Un valore intero utilizzato per contare gli allegati
            // non importati
            int notImportedAttachments = 0;

            // Le informazioni aggiuntive da riportare nella riga di report
            // relativa ad un allegato creato.
            string identificationData;

            // Il report relativo al documento cui si vuole aggiungere l'allegato
            ImportResult documentReport;

            #endregion

            // Se non ci sono documenti da importare, il report conterrà
            // una sola riga con un messaggio informativo per l'utente
            if (!dataReader.HasRows)
                toReturn.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.OK,
                    Message = "Non sono stati rilevati allegati da importare."
                });

            // Finchè ci sono dati da leggere
            while (dataReader.Read())
            {
                #region Inizializzazione variabili

                // Reset del controllore di validità dati
                validParameters = true;

                // Reset del flag di errore in lettura dati
                noErrorsReadingRowData = true;

                // Creazione della lista dei problemi
                creationProblems = new List<string>();

                // Creazione del risultato dell'operazione di creazione
                temp = new ImportResult();

                // Inizializzazione dei dati identificativi del documento
                // creato
                identificationData = String.Empty;

                #endregion

                #region Prelevamento dei dati dalla riga attuale

                try
                {
                    // Lettura dei dati dalla riga attuale
                    rowData = this.ReadDataFromCurrentRow(dataReader, userInfo, role);

                }
                catch (Exception e)
                {
                    // Aggiunta di un oggetto result con i dettagli sull'eccezione
                    toReturn.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = e.Message,
                        Ordinal = rowData.OrdinalNumber
                    });

                    // Si sono verificati errori durante la lettura dei dati dalla riga attuale
                    noErrorsReadingRowData = false;

                    // Un allegato non è stato importato
                    notImportedAttachments++;

                }

                #endregion

                #region Controllo validità campi obbligatori

                validParameters = this.CheckDataValidity(rowData, out creationProblems);

                // Aggiunta della lista degli errori alla lista dei dettagli
                // del risultato attuale
                temp.OtherInformation.AddRange(creationProblems);

                #endregion

                // Se gli eventuali dati profilati sono stati letti con successo
                // e se la validazione è passata...
                if (noErrorsReadingRowData && validParameters)
                {
                    try
                    {
                        // Reperimento del report relativo al documento cui si desidera aggiungere
                        // l'allegato
                        documentReport = this.GetDocumentType(
                            rowData.MainOrdinal, 
                            resultContainer);

                        // Se il documentReport è null -> tipologia non riconosciuta
                        if (documentReport == null)
                        {
                            // Impostazione del risultato a fallimento
                            temp.Outcome = ImportResult.OutcomeEnumeration.KO;

                            // Impostazione del messaggio
                            temp.Message = String.Format("Tipologia {0} non riconosciuta",
                                rowData.MainOrdinal);

                            // Un allegato non è stato importato
                            notImportedAttachments++;

                        }
                        else
                        {
                            // Altrimenti si procede all'importazione dell'allegato
                            CreateDocument(
                                rowData,
                                userInfo,
                                role,
                                out identificationData,
                                ftpAddress,
                                documentReport,
                                ftpUsername,
                                ftpPassword);

                            // Aggiunta dell'esito positivo
                            temp.Outcome = ImportResult.OutcomeEnumeration.OK;

                            // Aggiunta del messaggio
                            temp.Message = String.Format("Allegato aggiunto con successo. {0}",
                                identificationData);
                            
                            // Un allegato è stato importato
                            importedAttachments++;

                        }

                        // Impostazione dell'ordinale
                        temp.Ordinal = rowData.OrdinalNumber;

                        // Aggiunta del risultato alla lista dei risultati
                        toReturn.Add(temp);

                        

                    }
                    catch (Exception e)
                    {
                        // Viene creato un risultato negativo
                        toReturn.Add(new ImportResult()
                        {
                            Outcome = ImportResult.OutcomeEnumeration.KO,
                            Message = e.Message,
                            Ordinal = rowData.OrdinalNumber
                        });

                        // Un allegato non è stato importato
                        notImportedAttachments++;

                    }

                }
                else
                    if (noErrorsReadingRowData)
                    {
                        // Altrimenti viene aggiunto alla lista dei risultati
                        // un nuovo risultato negativo e ne viene impostata la descrizione
                        toReturn.Add(new ImportResult()
                        {
                            Outcome = ImportResult.OutcomeEnumeration.KO,
                            Message = "Alcuni parametri obbligatori non sono validi.",
                            OtherInformation = creationProblems,
                            Ordinal = rowData.OrdinalNumber
                        });

                        // Un allegato non è stato importato
                        notImportedAttachments++;

                    }

            }

            // Aggiunta di un'ultima riga contenente il totale degli allegati
            // importati e non importati
            toReturn.Add(new ImportResult()
            {
                Outcome = ImportResult.OutcomeEnumeration.OK,
                Message = String.Format("Allegati importati: {0}; Allegati non importati: {1}",
                    importedAttachments, notImportedAttachments)
            });

            // Restituzione del report sui risultati
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'importazione di allegati i cui dati sono descritti all'intenro di 
        /// un apposito oggetto
        /// </summary>
        /// <param name="documentRowData"></param>
        /// <param name="userInfo"></param>
        /// <param name="role"></param>
        /// <param name="sharedDirectoryPath"></param>
        /// <param name="resultsContainer"></param>
        /// <returns></returns>
        public List<ImportResult> ImportDocuments(
            List<DocumentRowData> documentRowDataList, 
            InfoUtente userInfo,
            Ruolo role,
            string ftpAddress,
            ref ResultsContainer resultsContainer,
            String ftpUsername,
            String ftpPassword)
        {
            #region Dichiarazione variabili

            // L'oggetto da restituire
            List<ImportResult> toReturn;

            // Risultato dell'ultima importazione
            ImportResult importResult = null;

            // Il numero di documenti importati
            int importedDocuments;

            // Il numero di documenti non importati
            int notImportedDocuments;

            #endregion

            // Inizializzazione variabili
            toReturn = new List<ImportResult>();
            importedDocuments = 0;
            notImportedDocuments = 0;

            // Se non ci sono documenti da importare, il risultato sarà costituito da un solo risultato
            // positivo
            if (documentRowDataList.Count == 0)
                toReturn.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.OK,
                    Message = "Non sono stati rilevati allegati da importare."
                });

            // Importazione dei documenti
            foreach (DocumentRowData documentData in documentRowDataList)
            {
                try
                {
                    importResult = this.ImportDocument(
                        documentData,
                        userInfo,
                        role,
                        ftpAddress,
                        ref resultsContainer,
                        ftpUsername,
                        ftpPassword);
                }
                catch (Exception e)
                {
                    // Inclusione di un risultato negativo
                    toReturn.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = e.Message,
                        Ordinal = documentData.OrdinalNumber
                    });

                    // Un documento non è stato importato
                    notImportedDocuments++;

                }

                // Se il risultato restituito dalla funzione di creazione del documento
                // è positivo, aggiunta di un risultato positivo, altrimenti aggiunta di un
                // risultato negativo
                if (importResult.Outcome == ImportResult.OutcomeEnumeration.KO)
                    notImportedDocuments++;
                else
                    importedDocuments++;

                // Aggiunta dell'oggetto result alla lista dei risultati
                toReturn.Add(importResult);

            }

            // Aggiunta di un ultimo risultato con il numero di 
            // documenti importati e non importati
            // Aggiunta di un'ultima riga contenente il totale dei documenti
            // importati e non importati
            toReturn.Add(new ImportResult()
            {
                Outcome = ImportResult.OutcomeEnumeration.OK,
                Message = String.Format("Allegati importati: {0}; Allegati non importati: {1}",
                    importedDocuments, notImportedDocuments)
            });

            // Restituzione del risultato
            return toReturn;
 
        }

        /// <summary>
        /// Funzione per l'importazione di un allegato i cui dati sono riportati all'interno di 
        /// un apposito oggetto
        /// </summary>
        /// <param name="documentRowData">L'oggetto con i dati sull'allegato da importare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo dell'utente che ha lanciato la procedura</param>
        /// <param name="ftpAddress">L'indirizzo della cartella sharata in cui recuperare il file da allegare al documento</param>
        /// <returns>Il risultato dell'operaizone di importazione</returns>
        public ImportResult ImportDocument(
            DocumentRowData documentRowData, 
            InfoUtente userInfo, 
            Ruolo role,
            string ftpAddress,
            ref ResultsContainer resultContainer,
            String ftpUsername,
            String ftpPassword)
        {
            #region Dichiarazione variabili

            // Il risultato da restituire
            ImportResult toReturn = new ImportResult();

            // Valore booleano utilizzato per indicare la
            // validità dei dati
            bool validParameters = false;

            // Valore booleano utilizzato per indicare se
            // ci sono stati problemi durante la lettura dei dati dalla
            // riga del foglio excel
            bool noErrorsReadingRowData = false;

            // La lista dei problemi rilevati durante le fasi di
            // creazione di un documento
            List<string> creationProblems = null;

            // Le informazioni aggiuntive da riportare nella riga di report
            // relativa ad un allegato creato.
            string identificationData;

            // L'oggetto con le infomazioni sul documento a cui è necessario aggiungere
            // l'allegato
            ImportResult documentReport;

            #endregion

            // Se non è stato passato il documento da importare, viene restituito
            // un messaggio informativo per l'utente
            if (documentRowData==null)
                toReturn = new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.OK,
                    Message = "Non sono stati rilevati allegati da importare."
                };

            // Se è stato passato l'oggetto da importare
            if(documentRowData != null)
            {
                #region Inizializzazione variabili

                // Reset del controllore di validità dati
                validParameters = true;

                // Reset del flag di errore in lettura dati
                noErrorsReadingRowData = true;

                // Creazione della lista dei problemi
                creationProblems = new List<string>();

                // Inizializzazione dei dati identificativi del documento
                // creato
                identificationData = String.Empty;

                #endregion

                #region Controllo validità campi obbligatori

                validParameters = this.CheckDataValidity(documentRowData, out creationProblems);

                // Aggiunta della lista degli errori alla lista dei dettagli
                // del risultato
                toReturn.OtherInformation.AddRange(creationProblems);

                #endregion

                // Se gli eventuali dati profilati sono stati letti con successo
                // e se la validazione è passata...
                if (noErrorsReadingRowData && validParameters)
                {
                    try
                    {
                        // Reperimento del report relativo al documento cui si desidera aggiungere
                        // l'allegato
                        documentReport = this.GetDocumentType(
                            documentRowData.MainOrdinal,
                            resultContainer);

                        // Se il documentReport è null -> tipologia non riconosciuta
                        if (documentReport == null)
                        {
                            // Impostazione del risultato a fallimento
                            toReturn.Outcome = ImportResult.OutcomeEnumeration.KO;

                            // Impostazione del messaggio
                            toReturn.Message = String.Format("Tipologia {0} non riconosciuta",
                                documentRowData.MainOrdinal);

                        }
                        else
                        {
                            // Altrimenti si procede all'importazione dell'allegato
                            CreateDocument(
                                documentRowData,
                                userInfo,
                                role,
                                out identificationData,
                                ftpAddress,
                                documentReport,
                                ftpUsername,
                                ftpPassword);

                            // Aggiunta dell'esito positivo
                            toReturn.Outcome = ImportResult.OutcomeEnumeration.OK;

                            // Aggiunta del messaggio
                            toReturn.Message = String.Format("Allegato aggiunto con successo. {0}",
                                identificationData);

                        }

                        // Impostazione dell'ordinale
                        toReturn.Ordinal = documentRowData.OrdinalNumber;

                    }
                    catch (Exception e)
                    {
                        // Viene creato un risultato negativo
                        toReturn = new ImportResult()
                        {
                            Outcome = ImportResult.OutcomeEnumeration.KO,
                            Message = e.Message,
                            Ordinal = documentRowData.OrdinalNumber
                        };

                    }

                }
                else
                    if (noErrorsReadingRowData)
                    {
                        // Altrimenti viene restituito un messaggio di avviso
                        toReturn = new ImportResult()
                        {
                            Outcome = ImportResult.OutcomeEnumeration.KO,
                            Message = "Alcuni parametri obbligatori non sono validi.",
                            OtherInformation = creationProblems,
                            Ordinal = documentRowData.OrdinalNumber
                        };

                    }

            }

            // Restituzione del risultato
            return toReturn;
 
        }

        /// <summary>
        /// Funzione per l'estrazione dei dati dalla sorgente dati
        /// </summary>
        /// <param name="dataReader">La sorgente da cui estrarre i dati</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="isEnabledPregressi"> Abilita inseriemento dei documenti pregressi</param>
        /// <returns>
        /// La lista dei dati estratti dalla sorgente
        /// </returns>
        public List<DocumentRowData> ReadMetaData(
            OleDbDataReader dataReader, 
            InfoUtente userInfo, 
            Ruolo role,
            bool isEnabledPregressi)
        {
            #region Dichiarazione variabili
            // L'oggetto da restituire
            List<DocumentRowData> toReturn;

            // L'oggetto con le informazioni estratte da una riga della sorgente dati
            DocumentRowData documentRowData;

            #endregion

            // Creazione dell'oggetto da restituire
            toReturn = new List<DocumentRowData>();

            // Finchè ci sono righe da esaminare
            while (dataReader.Read())
            {
                try
                {
                    // Estrazione dei dati dalla riga attuale
                    documentRowData = this.ReadDataFromCurrentRow(dataReader, userInfo, role);

                }
                catch (Exception e)
                {
                    throw new Exception("Errore durante l'estrazione dei dati");
                }

                // Aggiunta dei dati alla lista dei risultati
                toReturn.Add(documentRowData);

            }

            // Restituzione della lista dei dati estratti
            return toReturn;

        }

        #endregion

        /// <summary>
        /// Funzione per il reperimento della lista dei report relativi alla tipologia di documento
        /// richiesto e del docNumber
        /// </summary>
        /// <param name="container">Il contenitore di tutti i report relativi ai documenti creati</param>
        /// <returns>Il report di creazione relativo al documento richiesto</returns>
        private ImportResult GetDocumentType(string mainOrdinal, ResultsContainer container)
        {
            // L'oggetto da restituire
            ImportResult toReturn = null;

            // Il report relativo alla tipologia di documenti richiesta
            List<ImportResult> documentTypeReport = null;
            
            // Espressione regolare di contollo numerico
            Regex isNumeric = new Regex(@"^\d+$");

            // L'indice da cui partire per cominciare a tagliare la stringa
            // codificata
            int startIndex = 1;
     

            // Se mainOrdinal contiene almeno due caratteri...
            if (mainOrdinal.Trim().Length > 1)
            {
                // Individuazione della tipologia di documento (fra A, P, I)
                switch (mainOrdinal[0].ToString().ToUpper())
                {
                    case "A":   // Arrivo
                        documentTypeReport = container.InDocument;
                        break;

                    case "P":   // Partenza
                        documentTypeReport = container.OutDocument;
                        break;

                    case "I":   // Interno
                        documentTypeReport = container.OwnDocument;
                        break;

                }

                // Se il documento non è stato identificato, si prova a vedere
                // se è un NP (Non protocollato)
                if (documentTypeReport == null && mainOrdinal.Substring(0, 2).ToUpper() == "NP")
                {
                    documentTypeReport = container.GrayDocument;
                    startIndex = 2;
                }

                // Se documentType è stato identificato, si procede con l'analisi
                // di validità dell'ordinale.
                /*
                 * Principio di validità.
                 * 
                 * La lunghezza dell'ordinale del principale deve contenere almeno
                 * un carattere in più rispetto alla stringa di specifica
                 * della tipologia di documento.
                 * L'ordinale, posizionato a partire dalla posizione Len(numCharID)
                 * fino a Len(mainOrdinal) deve essere un numero.
                 * 
                 */
                if (documentTypeReport != null &&
                    mainOrdinal.Trim().Length > startIndex &&
                    isNumeric.Match(mainOrdinal.Substring(startIndex)).Success)
                    toReturn = documentTypeReport.Where(
                        e => e.Ordinal == mainOrdinal.Substring(startIndex)).FirstOrDefault();

                    
            }

            // Restituzione del risultato
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'estrazione dei dati da una riga del foglio excel
        /// </summary>
        /// <param name="row">L'oggetto da cui estrarre i dati</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <returns>L'oggetto con i dati estratti</returns>
        private DocumentRowData ReadDataFromCurrentRow(OleDbDataReader row, InfoUtente userInfo, Ruolo role)
        {
            #region Dichiarazione Variabili

            // L'oggetto da restituire
            DocumentRowData toReturn;

            #endregion

            // Creazione dell'oggetto da restituire
            toReturn = new DocumentRowData();

            // Prelevamento dell'ordinale
            toReturn.OrdinalNumber = row["Ordinale"].ToString();

            // Prelevamento del codice amministrazione
            toReturn.AdminCode = row["Codice Amministrazione"].ToString();

            // Prelevamento dell'ordinale del documento principale
            toReturn.MainOrdinal = row["Ordinale principale"].ToString().Trim();

            // Prelevamento della descrizione dell'allegato
            toReturn.Obj = row["Descrizione"].ToString().Trim();
        
            // Prelevamento del pathname
            toReturn.Pathname = row["Pathname"].ToString().Trim();

            // Restituzione dell'oggetto creato
            return toReturn;

        }

        /// <summary>
        /// Funzione per la validazione dei dati
        /// </summary>
        /// <param name="rowData">L'oggetto con i dati da validare</param>
        /// <param name="validationProblems">La lista dei problemi riscontrati in fase di validazione</param>
        /// <returns>True se la validazione passa</returns>
        private bool CheckDataValidity(DocumentRowData rowData, out List<string> validationProblems)
        {
            #region Dichiarazione variabili

            // Il risultato della validazione
            bool toReturn = true;

            // La lista dei problemi rilevati in fase di validazione
            List<String> validationResult;

            #endregion

            // Creazione della lista dei problemi di validazione
            validationResult = new List<string>();

            // L'ordinale è obbligatorio
            if (String.IsNullOrEmpty(rowData.OrdinalNumber))
            {
                toReturn = false;
                validationResult.Add("Il campo 'Ordinale' è obbligatorio.");
               
            }

            // L'ordinale del documento principale è obbligatorio
            if (String.IsNullOrEmpty(rowData.MainOrdinal))
            {
                toReturn = false;
                validationResult.Add("Il campo 'Ordinale documento principale' è obbligatirio");
            
            }

            // Il codice amministrazione è obbligatorio
            if (String.IsNullOrEmpty(rowData.AdminCode))
            {
                toReturn = false;
                validationResult.Add("Il campo 'Codice Amministrazione' è obbligatirio");

            }

            // La descrizione è obbligatoria
            if (String.IsNullOrEmpty(rowData.Obj))
            {
                toReturn = false;
                validationResult.Add("Il campo 'Descrizione' è obbligatorio");
            }

            // Impostazione della lista dei problemi
            validationProblems = validationResult;

            // Restituzione del risultato di validazione
            return toReturn;
        
        }

        /// <summary>
        /// Funzione per la creazione dell'allegato
        /// </summary>
        /// <param name="rowData">L'oggetto con le informazioni sull'allegato da creare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="identificationData">I dati identificativi dell'allegato</param>
        /// <param name="sharedDirectoryPath">Il path i cui è posizionato il file da allegare al documento</param>
        /// <param name="importResults">L'oggetto con le informazioni sul documento cui si desidera aggiungere l'allegato</param>
        protected void CreateDocument(DocumentRowData rowData, InfoUtente userInfo, Ruolo role, out string identificationData, string ftpAddress, ImportResult importResults, String ftpUsername, String ftpPassword)
        {
            #region Dichiarazione variabili

            // L'allegato da agggiungere
            Allegato attachment = null;

            #endregion

            // Se il documento non è stato creato, eccezione
            if(String.IsNullOrEmpty(importResults.DocNumber))
                throw new Exception("Il documento cui si desidera aggiungere un allegato non è stato creato con successo.");

            // Creazione dell'oggetto Allegato
            attachment = this.CreateAttachmentObject(
                rowData.Obj,
                importResults.DocNumber);

            // Aggiunta dell'allegato al documento
            try
            {
                AllegatiManager.aggiungiAllegato(userInfo, attachment);
            }
            catch
            {
                throw new Exception("Errore durante l'aggiunta dell'allegato al documento specificato.");
            }

            // Acquisizione del file (se specificato il path)
            if (!String.IsNullOrEmpty(rowData.Pathname))
                this.AcquireFile(
                    rowData.Pathname,
                    rowData.AdminCode,
                    userInfo,
                    ftpAddress,
                    attachment,
                    ftpUsername,
                    ftpPassword, 
                    rowData);



            // Impostazione delle informazioni sull'allegato creato
            identificationData = String.Format(
                    "Id documento: {0}",
                    attachment.docNumber);

        }

        /// <summary>
        /// Funzione per la creazione dell'oggetto Allegato con le informazioni sull'allegato
        /// da creare
        /// </summary>
        /// <param name="description">La descrizione dell'allegato</param>
        /// <param name="docNumber">Il doc number del documento principale</param>
        /// <returns>L'oggetto con le informazioni sull'allegato creato</returns>
        private Allegato CreateAttachmentObject(string description, string docNumber)
        {
            // L'oggetto da restituire
            Allegato toReturn;

            // Creazione dell'allegato
            toReturn = new Allegato();

            // Impostazione della descrizione dell'allegato
            toReturn.descrizione = description;

            // Impostazione del docNumber
            toReturn.docNumber = docNumber;

            // Restituzione dell'oggetto creato
            return toReturn;
            
        }

        /// <summary>
        /// Funzione per l'acquisizione del file da associare ad un allegato
        /// </summary>
        /// <param name="path">Il path relativo del file da caricare</param>
        /// <param name="administrationCode">Il codice dell'amministrazione</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="ftpAddress">L'indirizzo a cui è possibile recuperare le informazioni sul file</param>
        /// <param name="attachment">L'allegato a cui associare il file</param>
        protected void AcquireFile(string path, string administrationCode, InfoUtente userInfo, string ftpAddress, Allegato attachment, String ftpUsername, String ftpPassword, DocumentRowData rowData)
        {
            #region Dichiarazione Variabili

            // Il contenuto del file
            byte[] fileContent;

            // L'oggetto fileRequest
            FileRequest fileRequest;

            // L'oggetto fileDocumento
            FileDocumento fileDocumento;

            #endregion

            #region Lettura file documento

            try
            {
                // Apertura, lettura e chiusura del file
                //fileContent = ImportUtils.DownloadFileFromFTP(
                //     ftpAddress,
                //     String.Format("{0}/{1}/{2}",
                //         administrationCode,
                //         userInfo.userId,
                //         path),
                //     ftpUsername,
                //     ftpPassword);

                // nuova versione, i file vengono presi in locale e caricati dagli utenti in locale nella procedura di import

                fileContent = ImportUtils.DounloadFileFromUserTempFolder(rowData, userInfo, true);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);

            }

            #endregion

            #region Creazione dell'oggetto fileDocumento

            // Creazione dell'oggetto fileDocumento
            fileDocumento = new FileDocumento();

            // Impostazione del nome del file
            fileDocumento.name = Path.GetFileName(path);

            // Impostazione del full name
            fileDocumento.fullName = path;

            // Impostazione del path
            fileDocumento.path = Path.GetPathRoot(path);

            // Impostazione della grandezza del file
            fileDocumento.length = fileContent.Length;

            // Impostazione del content del documento
            fileDocumento.content = fileContent;

            #endregion

            #region Creazione dell'oggetto fileRequest

            fileRequest = (FileRequest)attachment;

            #endregion

            #region Acquisizione del file

            try
            {
                FileManager.putFile(fileRequest, fileDocumento, userInfo);
            }
            catch (Exception e)
            {
                // Aggiunta del problema alla lista dei problemi
                throw new Exception("Errore durante l'upload del file.");
            }

            #endregion

        }

    }

}