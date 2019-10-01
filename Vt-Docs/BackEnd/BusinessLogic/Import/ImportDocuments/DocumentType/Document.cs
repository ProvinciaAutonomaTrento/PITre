using System;
using System.Collections.Generic;
using System.Linq;
using DocsPaVO.PrjDocImport;
using System.Data.OleDb;
using DocsPaVO.utente;
using DocsPaVO.Note;
using DocsPaVO.documento;
using BusinessLogic.Documenti;
using System.Collections;
using System.Collections.Generic;
using BusinessLogic.Utenti;
using DocsPaVO.ProfilazioneDinamica;
using BusinessLogic.ProfilazioneDinamica;
using DocsPaVO.Modelli_Trasmissioni;
using BusinessLogic.Trasmissioni;
using System.IO;
using BusinessLogic.Fascicoli;
using DocsPaVO.fascicolazione;
using DocsPaVO.filtri;
using System.Text;
using DocsPaVO.rubrica;
using DocsPaVO.addressbook;
using DocsPaVO.ricerche;

namespace BusinessLogic.Import.ImportDocuments.DocumentType
{
    public delegate void AcquireFileDelegate(DocumentRowData rowData, InfoUtente userInfo, Ruolo role, bool isSmistamentoEnabled,SchedaDocumento schedaDocumento, string ftpAddress, String ftpUsername, String ftpPassword);

    abstract class Document
    {
        #region Funzioni pubbliche
        /// <summary>
        /// Funzione per l'importazione di documenti
        /// </summary>
        /// <param name="dataReader">L'oggetto da cui estrarre i dati sul documento da creare</param>
        /// <param name="userInfo">Le informazini sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo dell'utente che ha lanciato la procedura</param>
        /// <param name="serverPath">L'indirizzo della WA</param>
        /// <param name="isProfilationRequired">True se è richiesta la profilazione obbligatoria</param>
        /// <param name="isRapidClassificationRequired">True se è richiesta la classificazione rapida</param>
        /// <param name="sharedDirectoryPath">L'indirizzo della cartella scharata</param>
        /// <param name="isGray">True se il documento da creare è un grigio</param>
        /// <param name="isSmistamentoEnabled">True se è abilitato lo smistamento</param>
        /// <param name="protoType">Codice identificativo del tipo di protocollo (Entrata, Partenza, ...)</param>
        /// <returns>Lista dei risultati di importazione</returns>
        public List<ImportResult> ImportDocuments(
            OleDbDataReader dataReader,
            InfoUtente userInfo,
            Ruolo role,
            string serverPath,
            bool isProfilationRequired,
            bool isRapidClassificationRequired,
            string ftpAddress,
            bool isGray,
            bool isSmistamentoEnabled,
            string protoType,
            String ftpUsername,
            String ftpPassword,
            bool isEnabledPregressi)
        {
            return ImportDocuments(dataReader,
                userInfo,
                role,
                serverPath,
                isProfilationRequired,
                isRapidClassificationRequired,
                ftpAddress,
                isGray,
                isSmistamentoEnabled,
                protoType,
                ftpUsername,
                ftpPassword,
                isEnabledPregressi,
                AcquireFile);
        }
        
        public List<ImportResult> ImportDocuments(
            OleDbDataReader dataReader, 
            InfoUtente userInfo, 
            Ruolo role, 
            string serverPath, 
            bool isProfilationRequired, 
            bool isRapidClassificationRequired, 
            string ftpAddress,
            bool isGray, 
            bool isSmistamentoEnabled, 
            string protoType,
            String ftpUsername,
            String ftpPassword,
            bool isEnabledPregressi,
            AcquireFileDelegate acquireDelegate)
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

            // Un valore intero utilizzato per contare i documenti
            // importati
            int importedDocuments = 0;

            // Un valore intero utilizzato per contare i documenti
            // non importati
            int notImportedDocuments = 0;

            // Le informazioni aggiuntive da riportare nella riga di report
            // relativa ad un documento creato.
            // Per un protocollo conterrà segnatura e id documento
            // Per i documenti non protocollati e gli allegati conterrà l'id documento
            string identificationData;

            // Il numero univoco associato al documento (docNumber)
            string docNumber = String.Empty;

            // Il system id del documento creato
            string idProfile = String.Empty;

            #endregion

            // Se non ci sono documenti da importare, il report conterrà
            // una sola riga con un messaggio informativo per l'utente
            if (!dataReader.HasRows)
                toReturn.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.OK,
                        Message = "Non sono stati rilevati documenti da importare."
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

                // Azzeramento del docNumber
                docNumber = String.Empty;

                #endregion

                #region Prelevamento dei dati dalla riga attuale

                try
                {
                    // Lettura dei dati dalla riga attuale
                    rowData = this.ReadDataFromCurrentRow(dataReader, userInfo, role, isEnabledPregressi);

                    // Lettura dei dati relativi alla profilazione
                    this.ReadProfilationData(dataReader, rowData, userInfo, role);
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

                    // Un documento non è stato importato
                    notImportedDocuments++;

                }

                #endregion

                #region Controllo validità campi obbligatori

                validParameters = this.CheckDataValidity(rowData, isProfilationRequired, isRapidClassificationRequired, out creationProblems);

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
                        // Creazione del documento  
                        bool fileAcquired;
                        creationProblems = this.CreateDocument(rowData, userInfo, role, out identificationData, serverPath, isGray, isRapidClassificationRequired, ftpAddress, isSmistamentoEnabled, protoType, out docNumber, out idProfile, ftpUsername, ftpPassword, isEnabledPregressi,acquireDelegate,out fileAcquired);
                        // Aggiunta degli eventuali errori alla lista dei dettagli
                        // del risultato
                        temp.OtherInformation.AddRange(creationProblems);

                        // Se si sono verificati errori durante l'elaborazione,
                        // il risultato è un Warning
                        if (temp.OtherInformation.Count > 0)
                        {
                            // Impostazione del risultato a fallimento
                            if (fileAcquired)
                            {
                                temp.Outcome = ImportResult.OutcomeEnumeration.Warnings;
                                temp.Message = String.Format("Documento creato con successo. {0}.",identificationData);
                            }
                            else
                            {
                                temp.Outcome = ImportResult.OutcomeEnumeration.FileNotAcquired;
                                temp.Message = String.Format("Documento creato con successo, ma acquisizione file fallita. {0}.",
                                        identificationData);
                            }

                        }
                        else
                        {
                            // Altrimenti il risultato è positivo
                            temp.Outcome = ImportResult.OutcomeEnumeration.OK;


                            // Il messaggio è un messaggio di successo
                            temp.Message = String.Format("Documento creato con successo. {0}.",
                                    identificationData);

                        }

                        // Impostazione del docNumber
                        temp.DocNumber = docNumber;

                        // Impostazione dell'ordinale
                        temp.Ordinal = rowData.OrdinalNumber;

                        // Aggiunta del risultato alla lista dei risultati
                        toReturn.Add(temp);

                        // Un documento è stato importato
                        importedDocuments++;

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

                        // Un documento non è stato importato
                        notImportedDocuments++;

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
                                Message = "Alcuni parametri non sono validi.",
                                OtherInformation = creationProblems,
                                Ordinal = rowData.OrdinalNumber
                            });

                        // Un documento non è stato importato
                        notImportedDocuments++;

                    }

            }

            // Aggiunta di un'ultima riga contenente il totale dei documenti
            // importati e non importati
            toReturn.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.OK,
                        Message = String.Format("Documenti importati: {0}; Documenti non importati: {1}",
                            importedDocuments, notImportedDocuments)
                    });

            // Restituzione del report sui risultati
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'importazione di un insieme di documenti di una determinata tipologia
        /// </summary>
        /// <param name="documentRowDataList"></param>
        /// <param name="userInfo"></param>
        /// <param name="role"></param>
        /// <param name="serverPath"></param>
        /// <param name="isProfilationRequired"></param>
        /// <param name="isRapidClassificationRequired"></param>
        /// <param name="sharedDirectoryPath"></param>
        /// <param name="isGray"></param>
        /// <param name="isSmistamentoEnabled"></param>
        /// <param name="protoType"></param>
        /// <returns></returns>
        public List<ImportResult> ImportDocuments(
            List<DocumentRowData> documentRowDataList, 
            InfoUtente userInfo, 
            Ruolo role, 
            string serverPath, 
            bool isProfilationRequired, 
            bool isRapidClassificationRequired,
            string ftpAddress, 
            bool isGray, 
            bool isSmistamentoEnabled, 
            string protoType,
            string ftpUsername,
            String ftpPassword,
            bool isEnabledPregressi)
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
                    Message = "Non sono stati rilevati documenti da importare."
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
                        serverPath,
                        isProfilationRequired, isRapidClassificationRequired,
                        ftpAddress,
                        isGray,
                        isSmistamentoEnabled,
                        protoType,
                        ftpUsername,
                        ftpPassword,
                        isEnabledPregressi);
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
                Message = String.Format("Documenti importati: {0}; Documenti non importati: {1}",
                    importedDocuments, notImportedDocuments)
            });

            // Restituzione del risultato
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'importazione di un documento descritto all'interno di un apposito oggetto
        /// </summary>
        /// <param name="documentRowData">Oggetto con le informazioni sul documento da creare</param>
        /// <param name="userInfo">Le informazini sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo dell'utente che ha lanciato la procedura</param>
        /// <param name="serverPath">L'indirizzo della WA</param>
        /// <param name="isProfilationRequired">True se è richiesta la profilazione obbligatoria</param>
        /// <param name="isRapidClassificationRequired">True se è richiesta la classificazione rapida</param>
        /// <param name="ftpAddress">L'indirizzo della cartella FTP</param>
        /// <param name="isGray">True se il documento da creare è un grigio</param>
        /// <param name="isSmistamentoEnabled">True se è abilitato lo smistamento</param>
        /// <param name="protoType">Codice identificativo del tipo di protocollo (Entrata, Partenza, ...)</param>        
        /// <returns>Il risultato dell'importazione</returns>
        public ImportResult ImportDocument(
            DocumentRowData documentRowData,
            InfoUtente userInfo,
            Ruolo role,
            string serverPath,
            bool isProfilationRequired,
            bool isRapidClassificationRequired,
            string ftpAddress,
            bool isGray,
            bool isSmistamentoEnabled,
            string protoType,
            String ftpUsername,
            String ftpPassword,
            bool isEnabledPregressi)
        {
            return ImportDocument(documentRowData,
                userInfo,
                role,
                serverPath,
                isProfilationRequired,
                isRapidClassificationRequired,
                ftpAddress,
                isGray,
                isSmistamentoEnabled,
                protoType,
                ftpUsername,
                ftpPassword,
                isEnabledPregressi,
            AcquireFile);
        }
        
        public ImportResult ImportDocument(
            DocumentRowData documentRowData, 
            InfoUtente userInfo, 
            Ruolo role, 
            string serverPath, 
            bool isProfilationRequired, 
            bool isRapidClassificationRequired,
            string ftpAddress,
            bool isGray, 
            bool isSmistamentoEnabled, 
            string protoType,
            String ftpUsername,
            String ftpPassword,
            bool isEnabledPregressi,
            AcquireFileDelegate acquireDelegate,
            bool isStampaUnione = false)
        {
            #region Dichiarazione variabili

            // Il report da restituire
            ImportResult toReturn;

            // Valore booleano utilizzato per indicare la
            // validità dei dati
            bool validParameters = false;

            bool canCreateDocument = true;
            string msgCanCreateDocument = string.Empty;
            // La lista di funzioni associate al ruolo
            Funzione[] functions = null;

            // La lista dei problemi rilevati durante le fasi di
            // creazione di un documento
            List<string> creationProblems = null;

            // Le informazioni aggiuntive da riportare nella riga di report
            // relativa ad un documento creato.
            // Per un protocollo conterrà segnatura e id documento
            // Per i documenti non protocollati e gli allegati conterrà l'id documento
            string identificationData;

            // Il numero univoco associato al documento (docNumber)
            string docNumber = String.Empty;

            // Il system id del documento creato
            string idProfile = String.Empty;

            #endregion

            // Se non è stato specificato il documento da importare, il report conterrà
            // un messaggio informativo per l'utente
            if (documentRowData == null)
                toReturn = new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.OK,
                    Message = "Non sono stati rilevati documenti da importare."
                };
            else
            {
                // Esecuzione dell'importazione del documento
                #region Inizializzazione variabili

                // Reset del controllore di validità dati
                validParameters = true;

                // Creazione della lista dei problemi
                creationProblems = new List<string>();

                // Creazione del risultato dell'operazione di creazione
                toReturn = new ImportResult();

                // Inizializzazione dei dati identificativi del documento
                // creato
                identificationData = String.Empty;

                // Azzeramento del docNumber
                docNumber = String.Empty;

                #endregion

                #region Controllo abilitazione ruolo
                functions = (Funzione[])role.funzioni.ToArray(typeof(Funzione));
                switch (protoType)
                {
                    case "A":
                        canCreateDocument = functions.Where(e => e.codice == "DO_NUOVOPROT").FirstOrDefault() != null && functions.Where(e => e.codice == "PROTO_IN").FirstOrDefault() != null;
                        msgCanCreateDocument = "Ruolo non abilitato alla creazione di documenti in arrivo.";
                        break;
                    case "P":
                        canCreateDocument = functions.Where(e => e.codice == "DO_NUOVOPROT").FirstOrDefault() != null && functions.Where(e => e.codice == "PROTO_OUT").FirstOrDefault() != null;
                        msgCanCreateDocument = "Ruolo non abilitato alla creazione di documenti in partenza.";
                        break;
                    case "I":
                        canCreateDocument = functions.Where(e => e.codice == "DO_NUOVOPROT").FirstOrDefault() != null && functions.Where(e => e.codice == "PROTO_OWN").FirstOrDefault() != null;
                        msgCanCreateDocument = "Ruolo non abilitato alla creazione di documenti interni.";
                        break;
                }
                if (!canCreateDocument)
                {
                    toReturn = new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = msgCanCreateDocument,
                        OtherInformation = creationProblems,
                        Ordinal = documentRowData.OrdinalNumber
                    };
                    return toReturn;
                }
                #endregion

                #region Controllo validità campi obbligatori

                validParameters = this.CheckDataValidity(documentRowData, isProfilationRequired, isRapidClassificationRequired, out creationProblems);

                // Aggiunta della lista degli errori alla lista dei dettagli
                // del risultato
                toReturn.OtherInformation.AddRange(creationProblems);

                #endregion

                // Se la validazione è passata...
                if (validParameters)
                {
                    try
                    {
                        // Creazione del documento
                        bool fileAcquired;
                        creationProblems = this.CreateDocument(documentRowData, userInfo, role, out identificationData, serverPath, isGray, isRapidClassificationRequired, ftpAddress, isSmistamentoEnabled, protoType, out docNumber, out idProfile, ftpUsername, ftpPassword, isEnabledPregressi,acquireDelegate,out fileAcquired, isStampaUnione);
                        // Aggiunta degli eventuali errori alla lista dei dettagli
                        // del risultato
                        toReturn.OtherInformation.AddRange(creationProblems);

                        // Se si sono verificati errori durante l'elaborazione,
                        // il risultato è un Warning
                        if (toReturn.OtherInformation.Count > 0)
                        {
                            // Impostazione del risultato a fallimento
                            if (fileAcquired)
                            {
                                toReturn.Outcome = ImportResult.OutcomeEnumeration.Warnings;
                                toReturn.Message = String.Format("Documento creato con successo. {0}.",identificationData);
                            }
                            else
                            {
                                // Alessandro Aiello
                                // *****
                                // throw new Exception("File non acquisito, creazione documento annullata");
                                // ***** FINE
                                toReturn.Outcome = ImportResult.OutcomeEnumeration.FileNotAcquired;
                                toReturn.Message = String.Format("Documento creato con successo, ma acquisizione file fallita. {0}.",identificationData);
                            }


                        }
                        else
                        {
                            // Altrimenti il risultato è positivo
                            toReturn.Outcome = ImportResult.OutcomeEnumeration.OK;

                            // Il messaggio è un messaggio di successo
                            toReturn.Message = String.Format("Documento creato con successo. {0}.",
                                    identificationData);

                        }

                        // Impostazione del docNumber
                        toReturn.DocNumber = docNumber;

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
                {
                    // Altrimenti viene creato un risultato negativo e 
                    // ne viene impostata la descrizione
                    toReturn = new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = "Alcuni parametri non sono validi.",
                        OtherInformation = creationProblems,
                        Ordinal = documentRowData.OrdinalNumber
                    };

                }

            }

            // Restituzione del report sui risultati
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'estrazione dei dati dalla sorgente dati
        /// </summary>
        /// <param name="dataReader">La sorgente da cui estrarre i dati</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <returns>La lista dei dati estratti dalla sorgente</returns>
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
                    documentRowData = this.ReadDataFromCurrentRow(dataReader, userInfo, role, isEnabledPregressi);

                    // Estrazione dei dati profilati
                    this.ReadProfilationData(dataReader, documentRowData, userInfo, role);
                }
                catch (Exception e)
                {
                    throw new Exception(
                        String.Format(
                            "Si è verificato un errore durante l'estrazione dei dati dal foglio Excel. Dettagli: {0}", e.Message));
                }

                // Aggiunta dei dati alla lista dei risultati
                if(!documentRowData.IsEmpty()) toReturn.Add(documentRowData);

            }

            // Restituzione della lista dei dati estratti
            return toReturn;

        }

        #endregion

        #region Metodi accessori
        
        /// <summary>
        /// Funzione per la lettura dei dati profilati relativi alla tipologia documenti ed
        /// alla tipologia fascicolo da una riga di foglio excel
        /// </summary>
        /// <param name="row">L'oggetto da cui estrarre i dati</param>
        /// <param name="rowData">L'oggetto con i dati estratti</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        protected void ReadProfilationData(OleDbDataReader row, DocumentRowData rowData, InfoUtente userInfo, Ruolo role)
        {
            // Lettura dei valori assegnati ai campi profilati del documento
            // Per ogni colonna presente nel data reader
            for (int i = 0; i < row.FieldCount; i++)
                // ...se il nome della colonna comincia per DCampo...
                if (row.GetName(i).Trim().ToUpper().StartsWith("DCAMPO"))
                {
                    if (!String.IsNullOrEmpty(row[i].ToString()))
                    {
                        // ...si spezza la stringa nelle sue due componenti
                        string[] fieldInformation = row[i].ToString().Split('=');

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
                                String.Format("Errore durante l'estrazione del contenuto della cella '{0}' relativa al documento con ordinale {1}. Dettaglio: {2}",
                                    row.GetName(i),
                                    rowData.OrdinalNumber,
                                    fieldInformation[0]));

                            throw new Exception(
                                String.Format("Il campo '{0}', relativo al documento con ordinale {1}, non è stato valorizzato correttamente. Questi campi devono contenere coppie Chiave=Valore o Chiave=Valore1;Valore2...",
                                    row.GetName(i),
                                    rowData.OrdinalNumber));
                        }
                         
                        // ...si aggiunge al dizionario dei campi profilati la chiave
                        // ed i valori
                        rowData.AddDocumentProfilationField(
                            fieldInformation[0],
                            fieldInformation[1].Split(';'));

                    }

                }

            // Lettura dei valori assegnati ai campi profilati del fascicolo
            // Per ogni colonna presente nel data reader
            for (int i = 0; i < row.FieldCount; i++)
                // ...se il nome della colonna comincia per FCampo...
                if (row.GetName(i).Trim().ToUpper().StartsWith("FCAMPO"))
                {
                    if (!String.IsNullOrEmpty(row[i].ToString()))
                    {
                        // ...si spezza la stringa nelle sue due componenti
                        string[] fieldInformation = row[i].ToString().Split('=');

                        // ...se fieldInformation non contiene due valori, eccezione
                        if (fieldInformation.Length != 2)
                            throw new Exception(
                                String.Format("Il campo '{0}' non è stato valorizzato correttamente. Questi campi devono contenere coppie Chiave=Valore o Chiave=Valore1;Valore2...",
                                    row.GetName(i)));

                        // ...si aggiunge al dizionario dei campi profilati la chiave
                        // ed i valori
                        rowData.AddProjectProfilationField(
                            fieldInformation[0],
                            fieldInformation[1].Split(';'));

                    }

                }
        }

        /// <summary>
        /// Funzione per la creazine del documento : viene passato un delegate che si occupa dell'acquisizione
        /// </summary>
        /// <param name="rowData">L'oggetto con le informazioni sul documento da creare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo dell'utente che ha lanciato la procedura</param>
        /// <param name="identificationData">Questa variabile verrà impostata in modo da contenere i dati identificativi del documento creato (id documento e segnatura protocollo)</param>
        /// <param name="serverPath">L'indirizzo web dell'applicazione di frontend</param>
        /// <param name="isGray">True se il documento da creare è un grigio</param>
        /// <param name="isRapidClassificationRequired">True se, da configurazione, è obbligatoria la classificazione rapida del documento</param>
        /// <param name="sharedDirectoryPath">L'indirizzo web della cartella condivisa in cui reperire i file da associare al documento</param>
        /// <param name="isSmistamentoEnabled">True se è abilitato lo smistamento</param>
        /// <param name="protoType">Lettera identificativa della tipologia del documento</param>
        /// <param name="docNumber">Il numero numero di documento</param>
        /// <param name="idProfile">Il system id del documento creato</param>
        /// <param name="acquireDelegate">Il delegate che si occupa dell'acquisizione</param>
        /// <returns>La lista dei problemi verificatisi in fase di creazione del documento</returns>
        protected List<string> CreateDocument(DocumentRowData rowData, InfoUtente userInfo, Ruolo role, out string identificationData, string serverPath, bool isGray, bool isRapidClassificationRequired, string ftpAddress, bool isSmistamentoEnabled, string protoType, out string docNumber, out string idProfile, String ftpUsername, String ftpPassword, bool isEnabledPregressi, AcquireFileDelegate acquireDelegate, out bool fileAcquired, bool isStampaUnione = false)
        {
            #region Dichiarazione variabili

            // La lista dei problemi da restituire
            List<String> toReturn = new List<string>();

            // La scheda documento
            SchedaDocumento schedaDocumento = null;

            // L'id dell'amministrazione
            string administrationSyd = String.Empty;

            // L'id del regstro
            string registrySyd = String.Empty;

            // L'id dell'RF
            string rfSyd = String.Empty;

            // L'id del titolario
            string titolarioSyd = String.Empty;


            // Alessandro Aiello 22/03/2019
            // ***** I protocolli vengono prima predisposti, in seguito all'acquisizione del file (come previsto dall normativa, verranno protocollati)
            // ***** INIZIO
            switch (protoType.ToUpper())
            {
                case "A":
                case "P":
                case "I":
                    isGray = false;
                    if (!isStampaUnione)
                    {
                        rowData.Predisposto = true;
                    }
                    break;
            }

            // ***** FINE

            bool isPredisposto = rowData.Predisposto;

            // Lista temporanea in cui verranno memorizzati i
            // problemi avvenuti in una delle fasi di creazione del documento
            List<string> tempProblems = new List<string>();

            // Valore booleano utilizzato per indicare che è stato trovato
            // un protocollo uguale a quello in fase di creazione
            bool existProtocol = false;

            // La lista dei fascicoli in cui inserire il documento
            List<string> projectIds = new List<string>();

            #endregion

            // 0. Fase di inizializzazione
            // Vengono ricavati gli id dell'amministrazione...
            administrationSyd = ImportUtils.GetAdministrationId(rowData.AdminCode);

            // del registro...
            registrySyd = ImportUtils.GetRegistrySystemId(rowData.RegCode, rowData.AdminCode);

            // dell'RF, se specificato...
            if (!String.IsNullOrEmpty(rowData.RFCode))
                rfSyd = ImportUtils.GetRFId(rowData.RFCode, administrationSyd);

            // e del titolatio attivo o di quello specificato
            titolarioSyd = ImportUtils.GetTitolarioId(rowData.Titolario, administrationSyd);

            // Creazione della lista temporanea dei problemi
            tempProblems = new List<string>();

            // 1. Creazione della scheda documento per il documento da creare
            schedaDocumento = this.GetDocumentScheda(rowData, userInfo, role, administrationSyd, registrySyd, rfSyd, protoType, isSmistamentoEnabled, out tempProblems, isEnabledPregressi);

            // Aggiunta dei problemi verificati
            toReturn.AddRange(tempProblems);

            // 2. Verifica eventuali duplicati
            if (!isGray)
                existProtocol = ProtoManager.cercaDuplicati(schedaDocumento);

            // Se il protocollo esiste già, eccezione
            if (existProtocol)
                throw new Exception(
                    String.Format("Esiste già un protocollo uguale. Impossibile creare il protocollo."));

            // 3. Reperimento informazioni sul fascicolo
            // Creazione della lista temporanea
            tempProblems = new List<string>();

            // Reperimento degli identificativi dei fascicoli se almeno uno fra i seguenti campi risulta valorizzato
            // ProjectCodes, ProjectDescription, FolderDescription, NodeCode, ProjectTipology
            if ((rowData.ProjectCodes != null && rowData.ProjectCodes.Length > 0) ||
                !String.IsNullOrEmpty(rowData.ProjectDescription) ||
                !String.IsNullOrEmpty(rowData.FolderDescrition) ||
                !String.IsNullOrEmpty(rowData.NodeCode) ||
                !String.IsNullOrEmpty(rowData.ProjectTipology))
                projectIds = this.GetProjectsForClassification(rowData, userInfo, role, administrationSyd, registrySyd, rfSyd, titolarioSyd, out tempProblems, isSmistamentoEnabled);

            // Aggiunta dei problemi alla lista dei problemi
            toReturn.AddRange(tempProblems);

            // Se è richiesta fascicolazione rapida ma non sono stati restituiti fascicoli, non
            // si può procedere alla protocollazione
            if (projectIds.Count == 0 && isRapidClassificationRequired)
                throw new Exception("Non è stato rilevato alcun fascicolo in cui classificare il documento. Dato che la classificazione è obbligatoria, non si può procedere alla creazione del documento.");

            // 4. Creazione documento, allegato o protocollo
            if (!isGray && !isPredisposto)
                this.ProtocolDocument(schedaDocumento, userInfo, role);
            else
            {
                this.CreateGrayDocument(schedaDocumento, userInfo, role);
                if (isPredisposto)
                {
                    this.PredisponiDocumentoAllaProtocollazione(schedaDocumento, userInfo);
                    schedaDocumento.tipoProto = protoType;
                }
                   
            }

            // 5. Acquisizione del file associato al documento, se valorizzato
            fileAcquired = true;
            try
            {
                acquireDelegate.Invoke(rowData, userInfo,role,isSmistamentoEnabled, schedaDocumento, ftpAddress, ftpUsername, ftpPassword);
            }
            catch (Exception e)
            {
                // Aggiunta del problema alla lista dei problemi
                fileAcquired = false;
                toReturn.Add(e.Message);
                // rimuovere il documento ed impedire l'acquisizione degli allegati associati
            }

            // 6. Fascicolazione
            tempProblems = this.AddDocToProjects(userInfo, schedaDocumento.systemId, projectIds, isRapidClassificationRequired);

            // Aggiunta dei problemi di fascicolazione alla lista dei problemi
            toReturn.AddRange(tempProblems);



            // Alessandro Aiello 
            // ***** 25/03/2019
            // Protocollo i documenti
            // ***** INIZIO
            if(!isStampaUnione)
            {
                switch (protoType.ToUpper())
                {
                    case "A":
                    case "P":
                    case "I":
                        DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(userInfo, "0");
                        schedaDocumento = BusinessLogic.Documenti.ProtoManager.getDataProtocollo(schedaDocumento);
                        DocsPaVO.documento.ResultProtocollazione result;
                        if (!fileAcquired)
                        {
                            toReturn.Add("Documento non protocollato perché il file non è stato acquisito");
                        }
                        else if (!documentManager.ProtocollaDocumentoPredisposto(schedaDocumento, role, out result))
                        {
                            toReturn.Add("Errore nella Protocollazione del documento");
                        }

                        break;
                }
            }
            
            // ***** FINE






            // 7. Trasmissione documento
            tempProblems = this.TransmitDocument(schedaDocumento, rowData, userInfo, role, serverPath);

            // Aggiunta dei problemi di trasmissione alla lista dei problemi
            toReturn.AddRange(tempProblems);

            // 8. Salvataggio del documento nell'area di lavoro se richiesto
            if (rowData.InWorkingArea)
            {
                tempProblems = this.SaveDocumentInWorkingArea(schedaDocumento, userInfo, role);

                // Aggiunta dei problemi in fase di spostamento del documento
                // nell'area di lavoro, alla lista dei problemi da restituire
                toReturn.AddRange(tempProblems);

            }

            

            // Impostazione delle informazioni sul documento
            bool grigio = isGray || isPredisposto;
            identificationData = this.GetIdentificationData(schedaDocumento, grigio);

            // Impostazione del docNumber
            docNumber = schedaDocumento.docNumber;

            // Impostazione del system id
            idProfile = schedaDocumento.systemId;

            // Restituzione della lista dei risultati
            return toReturn;

        }

        /// <summary>
        /// Funzione per la creazione di un documento grigio o di un allegato
        /// </summary>
        /// <param name="schedaDocumento">La scheda documento con le informazioni sul documento o allegato da creare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo dell'utente che ha lanciato la procedura</param>
        private void PredisponiDocumentoAllaProtocollazione(SchedaDocumento schedaDocumento, InfoUtente userInfo)
        {

            try
            {
                // Creazione del documento grigio
                schedaDocumento = DocSave.predisponiAllaProtocollazione(
                    userInfo,
                    schedaDocumento);
            }
            catch (Exception e)
            {
                throw new Exception("Errore durante la fase di creazione del documento grigio.");
            }

        }

        private void CreateGrayDocument(SchedaDocumento schedaDocumento, InfoUtente userInfo, Ruolo role)
        {

            try
            {
                // Creazione del documento grigio
                schedaDocumento = DocSave.addDocGrigia(
                    schedaDocumento,
                    userInfo,
                    role);
            }
            catch (Exception e)
            {
                throw new Exception("Errore durante la fase di creazione del documento grigio.");
            }

        }

        protected SchedaDocumento GetDocumentScheda(DocumentRowData rowData, InfoUtente userInfo, Ruolo role, string administrationSyd, string registrySyd, string rfSyd, string protoType, bool isSmistamentoEnabled, out List<string> problems, bool isEnabledPregressi)
        {
            #region Dichiarazione variabili

            // La scheda da restituire
            SchedaDocumento toReturn = null;

            // La lista degli eventuali problemi
            List<string> creationProblems;

            #endregion

            // Creazione nuova scheda documento
            toReturn = DocManager.NewSchedaDocumento(userInfo);

            // Impostazione dell'oggetto
            toReturn.oggetto = this.LoadDocumentObject(
                userInfo,
                rowData.ObjCode,
                rowData.Obj,
                administrationSyd,
                registrySyd);

            // Aggiunta della nota al documento
            if (rowData.Note != null)
            {
                // Creazione dell'array delle note
                toReturn.noteDocumento = new List<InfoNota>();

                // Aggiunta della nota
                toReturn.noteDocumento.Add(rowData.Note);
            }

            // Impostazione delle informazioni sull'RF
            this.SetRFInformation(toReturn, rowData.RFCode, rfSyd);

            try
            {
                // Impostazione del registro
                toReturn.registro = RegistriManager.getRegistro(registrySyd);
            }
            catch (Exception e)
            {
                // Errore nel reperimento del registro
                throw new Exception(
                    String.Format("Errore durante il reperimento delle informazioni sul registro {0}",
                    rowData.RegCode));
            }

            // Se il registro è nullo, eccezione
            if (toReturn.registro == null)
                throw new Exception(String.Format(
                    "Errore durante il reperimento delle informazioni sul registro {0}",
                    rowData.RegCode));

            // Impostazione del tipo atto
            toReturn.tipoProto = protoType;

            // Creazione delle informazioni sul protocollo
            toReturn.protocollo = this.CreateProtocolObject(rowData, registrySyd, rfSyd, administrationSyd, isSmistamentoEnabled, userInfo, role);

            // Se si è in corte dei conti, viene impostato anche il numero di protocollo e la data
            if (isEnabledPregressi && toReturn.protocollo != null && rowData.ProtocolNumber!=null)
            {

                //Devono essere obbligatori
                toReturn.protocollo.numero = rowData.ProtocolNumber;
                toReturn.protocollo.dataProtocollazione = rowData.ProtocolDate.ToString("dd/MM/yyyy");
            }

            //Utente protocollatore e ruolo protollatore
            if (isEnabledPregressi)
            {
                string codiceRuolo =null;
                string codiceUtente=null;
                if (!String.IsNullOrEmpty (rowData.CodiceRuoloCreatore))
                    codiceRuolo = rowData.CodiceRuoloCreatore.ToUpper () ;

                if(!String.IsNullOrEmpty(rowData.CodiceUtenteCreatore))
                    codiceUtente = rowData.CodiceUtenteCreatore.ToUpper();


                // non valorizzazione dei campi sopra..
                // Valorizzazione corretta
                // Valorizza non corretta


                //SANDALI
                if ((codiceRuolo != null) && (codiceUtente != null))
                {
                    DocsPaVO.utente.Ruolo ruolo = (DocsPaVO.utente.Ruolo)Utenti.UserManager.getCorrispondenteByCodRubrica(codiceRuolo, DocsPaVO.addressbook.TipoUtente.INTERNO, administrationSyd);
                    DocsPaVO.utente.Utente utente = (DocsPaVO.utente.Utente)Utenti.UserManager.getCorrispondenteByCodRubrica(codiceUtente, DocsPaVO.addressbook.TipoUtente.INTERNO, administrationSyd);

                    bool exists=false; //MALEDETTE ARRAYLIST! :(
                    if (ruolo != null)
                    {
                        foreach (Ruolo r in Utenti.UserManager.getRuoliUtente(utente.idPeople))
                        {
                            if (r.codiceRubrica == ruolo.codiceRubrica)
                            {
                                exists = true;
                                break;
                            }
                        }
                    }

                    if (exists)
                    {
                        toReturn.creatoreDocumento = new CreatoreDocumento(Utenti.UserManager.GetInfoUtente(utente, ruolo), ruolo);
                    }
                    else
                    {
                        throw new Exception(String.Format(
                    "I'utente  {0}, non fa parte del ruolo {1}",
                    utente.codiceCorrispondente , ruolo.codiceCorrispondente));
                    }
                }
                else
                {

                    toReturn.creatoreDocumento = new CreatoreDocumento(userInfo, role);
                }
                
                //toReturn.creatoreDocumento.idPeople = utente.idPeople;
                //toReturn.creatoreDocumento.idCorrGlob_Ruolo = ruolo.systemId;
                //toReturn.creatoreDocumento.idCorrGlob_UO = ruolo.uo.systemId;
                //toReturn.creatoreDocumento.uo_codiceCorrGlobali = ruolo.uo.codice;
                
            }
            // Profilazione dinamica
            // Creazione della lista dei problemi
            creationProblems = new List<string>();

            // Se è specificato un template, si procede con la profialzione
            if (!String.IsNullOrEmpty(rowData.DocumentTipology))
                creationProblems = this.CompileDocumentProfilationFields(
                    rowData,
                    toReturn,
                    role,
                    userInfo,
                    administrationSyd,
                    isSmistamentoEnabled);

            #region Gestione Protocollo Emergenza

            // Se l'oggetto rowData è un oggetto RDEDocumentRowData,
            // bisogna inserire nella scheda documento i dati relativi 
            // al protocollo di emergenza
            if (rowData is RDEDocumentRowData)
            {
                // Casting ad oggetto RDEDocumentRowData
                RDEDocumentRowData emergencyData = (RDEDocumentRowData)rowData;

                toReturn.datiEmergenza = new DatiEmergenza();
                toReturn.datiEmergenza.dataProtocollazioneEmergenza = emergencyData.EmergencyProtocolDate + " " + emergencyData.EmergencyProtocolTime;
                toReturn.datiEmergenza.protocolloEmergenza = emergencyData.EmergencyProtocolSignature;

                // Se il protocollo è in Arrivo, vengono impostati i dati sul protocollo mittente
                if (protoType.ToUpper() == "A")
                {
                    ProtocolloEntrata inProto = (ProtocolloEntrata)toReturn.protocollo;

                    inProto.dataProtocolloMittente = emergencyData.SenderProtocolDate;
                    inProto.descrizioneProtocolloMittente = emergencyData.SenderProtocolNumber;
                    ((Documento)toReturn.documenti[0]).dataArrivo = emergencyData.ArrivalDate + " " + emergencyData.ArrivalTime;

                }
                
            }

            #endregion

            // Impostazione della lista dei problemi
            problems = creationProblems;

            // Restituzione della scheda documento
            return toReturn;

        }

        protected Oggetto LoadDocumentObject(InfoUtente userInfo, string objCode, string obj, string administrationSyd, string registrySyd)
        {
            #region Dichiarazione variabili

            // L'oggetto da restituire
            Oggetto toReturn = null;

            // L'oggetto query per il recupero delle informazioni su uno specifico oggetto
            QueryOggetto objectQuery = null;

            #endregion

            // Se è valorizzato il codice oggetto si procede al suo caricamento
            if (!String.IsNullOrEmpty(objCode))
            {
                // Creazione dell'oggetto query per il reperimento dell'oggetto
                objectQuery = new QueryOggetto();

                // Impostazione dell'id amministrazione
                objectQuery.idAmministrazione = administrationSyd;

                // Impostazione dell'id registro
                objectQuery.idRegistri = new System.Collections.ArrayList()
                    {
                        registrySyd
                    };

                // Impostazione del codice dell'oggetto
                objectQuery.queryCodice = objCode;

                // Reperimento dell'oggetto
                ArrayList oggs = ProtoManager.getListaOggetti(objectQuery);

                // Se sono stati rilevati più oggetti, eccezione altrimenti viene
                // impostato l'oggetto
                if (oggs == null || oggs.Count != 1)
                    throw new Exception(String.Format("Errore durante la ricerca dell'oggetto con codice {0}",
                        objCode));

                // Impostazione della nota
                toReturn = oggs[0] as Oggetto;

            }
            else
            {
                // Altrimenti viene creato un oggetto occasionale
                toReturn = new Oggetto();

                // Impostazione dell'id del registro
                toReturn.idRegistro = registrySyd;

                // Impostazione della descrizione dell'oggetto
                toReturn.descrizione = obj;

            }

            // Se non è stato impostato alcun oggetto, eccezione
            if (toReturn == null)
                throw new Exception("Impossibile recuperare informazioni sull'oggetto del documento");

            // Restituzione dell'oggetto
            return toReturn;

        }

        protected void SetRFInformation(SchedaDocumento documentScheda, string rfCode, string rfSyd)
        {
            // Impostazione del codice amministrazione nella scheda documento
            documentScheda.cod_rf_prot = rfCode.ToUpper();

            // Impostazione dell'id dell'RF
            documentScheda.id_rf_prot = rfSyd;

        }

        private List<string> CompileDocumentProfilationFields(DocumentRowData rowData, SchedaDocumento documentScheda, Ruolo role, InfoUtente userInfo, string administrationId, bool isSmistamentoEnabled)
        {
            #region Dichiarazione variabili

            // La lista dei template
            Templates[] templates = null;

            // Il template 
            Templates template = null;

            // I diritti di visibilità sui campi della tipologia
            ArrayList visibilityRights = null;

            // La lista degli errori emersi durante la compilazione dei
            // dati profilati
            List<string> toReturn = new List<string>();

            #endregion

            // Prelevamento della lista dei template creati per l'amministrazione
            templates = (Templates[])ProfilazioneDocumenti.getTemplates(administrationId).ToArray(typeof(Templates));

            // Ricerca del template con nome uguale a quello richiesto
            try
            {
                template = ProfilazioneDocumenti.getTemplateById(templates.Where(e => e.DESCRIZIONE.ToUpper() == rowData.DocumentTipology.ToUpper()).
                    FirstOrDefault().SYSTEM_ID.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(String.Format(
                    "Non è stato possibile recuperare le informazioni sul template {0}",
                    rowData.DocumentTipology));
            }

            // Se il template non è stato recuperato con successo, 
            // eccezione
            if (template == null)
                throw new Exception(String.Format(
                    "Non è stato possibile recuperare le informazioni sul template {0}",
                    rowData.DocumentTipology));

            // Altrimenti si procede con la compilazione dei campi profilati
            // Prelevamento dei diritti di visibilità sui campi della tipologia
            visibilityRights = ProfilazioneDocumenti.getDirittiCampiTipologiaDoc(
                role.idGruppo,
                template.SYSTEM_ID.ToString());

            // Se tutto è andato bene, si può procedere alla compilazione dei campi
            // profilati
            documentScheda.template = this.CompileProfilationObjects(
                    rowData,
                    role,
                    userInfo,
                    template,
                    (AssDocFascRuoli[])visibilityRights.ToArray(typeof(AssDocFascRuoli)),
                    administrationId,
                    isSmistamentoEnabled,
                    out toReturn);

            // Se il template è stato correttamente costruito, 
            // si procede con l'impostazione della descrizione e del system id
            // del template e si imposta a true il flag di aggiornamento
            // tipologia atto
            if (documentScheda.template != null)
            {
                documentScheda.tipologiaAtto = new TipologiaAtto();
                documentScheda.tipologiaAtto.descrizione = documentScheda.template.DESCRIZIONE;
                documentScheda.tipologiaAtto.systemId = documentScheda.template.SYSTEM_ID.ToString();
                documentScheda.daAggiornareTipoAtto = true;
            }

            // Restituzione degli eventuali problemi
            return toReturn;

        }

        private Templates CompileProfilationObjects(
            DocumentRowData rowData,
            Ruolo role,
            InfoUtente userInfo,
            Templates template,
            AssDocFascRuoli[] visibilityRights,
            string adminID,
            bool isSmistamentoEnabled,
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
                fieldValues = rowData.GetDocumentProfilationField(obj.DESCRIZIONE);

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
                        rowData.RegCode != null ? rowData.RegCode : String.Empty,
                        isSmistamentoEnabled));

                }
                else
                    // Altrimenti se l'utente ha diritti di modifica sul campo e il campo è
                    // obbligatorio, viene lanciata un'eccezione
                    if (rights != null && rights.INS_MOD_OGG_CUSTOM == "1" &&
                        obj.CAMPO_OBBLIGATORIO == "SI")
                        throw new Exception(String.Format(
                            "Il campo '{0}' è obbligatorio ma non risulta valorizzato nel foglio",
                            obj.DESCRIZIONE));

            }

            // Impostazione della lista dei problemi
            problems = toReturn;

            // Restituzione del template compilato
            return template;

        }

        protected void AcquireFile(DocumentRowData rowData, InfoUtente userInfo, Ruolo role, bool isSmistamentoEnabled, SchedaDocumento schedaDocumento, string ftpAddress, String ftpUsername, String ftpPassword)
        {
            if (String.IsNullOrEmpty(rowData.Pathname)) return;
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
                //    ftpAddress,
                //    String.Format("{0}/{1}/{2}",
                //        rowData.AdminCode,
                //        userInfo.userId,
                //        rowData.Pathname),
                //    ftpUsername,
                //    ftpPassword);

                // nuova versione, i file vengono presi in locale e caricati dagli utenti in locale nella procedura di import

                if(schedaDocumento.documentoPrincipale == null)
                {
                    fileContent = ImportUtils.DounloadFileFromUserTempFolder(rowData, userInfo);
                }
                else
                {
                    fileContent = ImportUtils.DounloadFileFromUserTempFolder(rowData, userInfo, true);
                }

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
            fileDocumento.name = Path.GetFileName(rowData.Pathname);

            // Impostazione del full name
            fileDocumento.fullName = rowData.Pathname;

            // Impostazione del path
            fileDocumento.path = Path.GetPathRoot(rowData.Pathname);

            // Impostazione della grandezza del file
            fileDocumento.length = fileContent.Length;

            // Impostazione del content del documento
            fileDocumento.content = fileContent;

            #endregion

            #region Creazione dell'oggetto fileRequest

            fileRequest = (FileRequest)schedaDocumento.documenti[0];

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

        protected List<string> AddDocToProjects(InfoUtente userInfo, string idProfile, List<string> projectIds, bool isRapidClassificationRequired)
        {
            #region Dichiarazione variabili

            // La lista dei problemi riscontrati in fascicolazione
            List<string> toReturn;

            // Valore booleano utilizzato per valutare il risultato della fascicolazione
            bool prjResult = false;

            // L'eventuale messagio restituito dalla funzione di inserimento documento in fascicolo
            string message = String.Empty;

            // Booleano utilizzato per indicare se l'id in analisi è il primo della lista
            // In questo caso se è richiesta fascicolazione rapida, il primo fascicolo
            // viene considerato come fascicolazione rapida
            bool firstProject = true;

            #endregion

            // Creazione della lista dei problemi da restituire
            toReturn = new List<string>();

            // Per ogni id fascicolo contenuto della lista degli id dei fascicoli
            foreach (string projectId in projectIds)
            {
                try
                {
                    // Azzeramento del booleano
                    prjResult = false;

                    // Si procede con la fascicolazione
                    prjResult = FascicoloManager.addDocFascicolo(
                        userInfo,
                        idProfile,
                        projectId,
                        firstProject ? true : false,
                        out message);

                    // Il prossimo elemento sicuramente non è il primo
                    firstProject = false;

                }
                catch (Exception e)
                {
                    // Aggiunta di un problema alla lista dei problemi
                    toReturn.Add("Si è verificato un errore durante la fascicolazione.");
                }

                // Se il risultato di fascicolazione è negativo,
                // viene aggiunto un messaggio di avviso alla lista dei problemi
                if (!prjResult)
                    toReturn.Add("Si è verificato un errore durante la fascicolazione.");

            }

            // Restituzione della lista dei problemi
            return toReturn;

        }

        protected List<string> TransmitDocument(SchedaDocumento schedaDocumento, DocumentRowData rowData, InfoUtente userInfo, Ruolo role, string serverPath)
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

            #endregion
            List<string> tempList = rowData.TransmissionModelCode.Where(e => !string.IsNullOrEmpty(e)).ToList();
            // Per ogni codice di modello di trasmissione...
            foreach (string modelCode in tempList)
            {
                // Azzeramento del flag eccezione
                haveException = false;

                // Azzeramento del flag trasmRes
                trasmRes = false;

                try
                {
                    // Si prova ad effettuare la trasmissione
                    trasmRes = TrasmManager.TransmissionExecuteDocTransmFromModelCode(
                        userInfo,
                        serverPath,
                        schedaDocumento,
                        modelCode,
                        role,
                        out transmModel);

                }
                catch (Exception e)
                {
                    haveException = true;

                }

                // Se si è verificata un'eccezione o se la trasmissione non è partita,
                // significa che si è verificato qualche problema
                if (!trasmRes || haveException)
                {
                    // Se trasmModel è valorizzato e non si riferisce a documenti
                    if (transmModel != null && transmModel.CHA_TIPO_OGGETTO != "D")
                        // Si segnala il problema all'utente
                        problems.Add(
                        String.Format(
                            "Il modello '{0}' non può essere utilizzato per trasmettere documenti",
                            modelCode.Trim()));
                    else
                        // altrimenti si segnala un errore generico
                        if (haveException)
                            problems.Add(
                                String.Format(
                                    "Errore durante l'invio delle trasmissioni del documento '{0}' utilizzando il modello {1}",
                                    schedaDocumento.systemId, modelCode.Trim()));
                        else
                            // altrimenti il modello non è stato reprito correttamente
                            problems.Add(
                                String.Format(
                                    "Impossibile recuperare le informazioni sul modello di trasmissione '{0}'",
                                    modelCode));
                }

            }

            // Restituzione dell'insieme dei problemi
            return problems;

        }

        protected List<string> SaveDocumentInWorkingArea(SchedaDocumento schedaDocumento, InfoUtente userInfo, Ruolo role)
        {
            // L'eventuale messaggio di errore da restituire
            List<string> toReturn = new List<string>();

            // Se il ruolo attuale non è abilitato allo spostamento di documenti nell'area di lavoro
            // non si può procedere
            Funzione[] functions = (Funzione[])role.funzioni.ToArray(typeof(Funzione));

            Funzione canAddInADL = functions.Where(e => e.codice == "DO_ADD_ADL").FirstOrDefault();

            if (canAddInADL == null)
                toReturn.Add("Con il ruolo corrente non è possibile aggiungere documenti nell'area di lavoro.");
            else
            {
                try
                {
                    areaLavoroManager.execAddLavoroMethod(schedaDocumento.systemId,
                        schedaDocumento.tipoProto, schedaDocumento.registro.systemId, userInfo, null);
                }
                catch (Exception e)
                {
                    toReturn.Add("Il documento è stato creato correttamente ma non è stato possibile aggiungerlo all'area di lavoro");
                }

            }

            // Restituzione dell'eventuale errore
            return toReturn;

        }

        /// <summary>
        /// Funzione per il reperimento degli identificativi dei fascicoli in cui classificare il documento
        /// </summary>
        /// <param name="rowData">L'oggetto con le informazioni sul documento da fascicolare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo dell'utente che ha lanciato la procedura</param>
        /// <param name="administrationSyd">Il system id dell'amministrazione in cui creare il documento</param>
        /// <param name="registrySyd">Il system id del registro in cui protocollare</param>
        /// <param name="rfSyd">Il system id dell'RF in cui protocollare</param>
        /// <param name="idTitolario">L'id del titolario in cui ricercare i fascicoli</param>
        /// <param name="problems">La lista dei problemi verificatisi in fase di ricerca degli identificati dei fascicoli</param>
        /// <param name="isEnabledSmistamento">True se è abilitato lo smistamento</param>
        /// <returns>La lista degli identificativi dei fascicoli in cui classificare il documento</returns>
        protected List<string> GetProjectsForClassification(DocumentRowData rowData, InfoUtente userInfo, Ruolo role, string administrationSyd, string registrySyd, string rfSyd, string idTitolario, out List<String> problems, bool isEnabledSmistamento)
        {
            #region Dichiarazione variabili

            // La lista con gli id dei fascicoli
            List<string> toReturn = new List<string>();

            // La lista degli eventuali problemi emersi durante la ricerca
            // dei fascioli
            List<string> projectProblems = new List<string>();

            // La lista di fascicoli restituira da sistema di ricerca
            Fascicolo[] projects = null;

            // Il fascicolo trovato
            Fascicolo project = null;

            // Il registro
            Registro registry = null;

            // I filtri per la ricerca di un fascicolo
            FiltroRicerca[] filters;

            // Numero massimo di elementi da restituire e
            // numero di elementi
            int totRec, nRec;

            // La stringa con i codici dei fascicoli trovati
            StringBuilder prjCodes;

            // Lista dei system id dei fascicoli restituiti dalla ricerca
            // Non viene utilizzata ma è richiesta dalla funzione
            List<SearchResultInfo> idProjectList;

            #endregion

            try
            {
                // Caricamento delle informazioni sul registro
                registry = RegistriManager.getRegistro(registrySyd);
            }
            catch (Exception e)
            {
                projectProblems.Add(String.Format(
                    "Impossibile recuperare le informazioni sul registro {0}",
                    rowData.RegCode));

            }

            // Se esistono dei codici fascicoli e registry è valorizzato...
            if (rowData.ProjectCodes != null && registry != null)
            {
                // Per ogni codice, si effettua la ricerca del fascicolo
                foreach (string cod in rowData.ProjectCodes)
                    try
                    {
                        project = FascicoloManager.getFascicoloDaCodice2(
                            administrationSyd,
                            role.idGruppo,
                            userInfo.idPeople,
                            cod,
                            registry,
                            true,
                            true,
                            idTitolario);

                        // Se il fascicolo è stato recuperato con successo,
                        // si inserisce il suo system id nella lista dei fascicoli
                        toReturn.Add(project.systemID);

                    }
                    catch (Exception e)
                    {
                        // Viene aggiunta una riga alla lista dei problemi
                        projectProblems.Add(String.Format(
                            "Impossibile recuperare le informazioni sul fascicolo {0}",
                            cod));
                    }

            }
            else
            {
                // Altrimenti si procede con la ricerca del fascicolo i cui
                // dati sono riportati nei campi dedicati al fascicolo
                // 1. Calcolo dei filtri di ricerca
                filters = this.GetFilters(
                    rowData,
                    registrySyd,
                    rfSyd,
                    idTitolario,
                    administrationSyd,
                    role,
                    userInfo,
                    isEnabledSmistamento);

                try
                {
                    // 2. Ricerca dei fascicoli
                    projects = (Fascicolo[])FascicoloManager.getListaFascicoliPaging(
                        userInfo,
                        null,
                        registry,
                        filters,
                        false,
                        true,
                        false,
                        out totRec,
                        out nRec,
                        1,
                        2,
                        false,
                        out idProjectList, null, string.Empty).ToArray(typeof(Fascicolo));

                    // 3. Se sono stati restituiti più fascicoli -> ambiguità -> errore
                    if (projects.Length > 1)
                    {
                        // Creazione della stringa contente i codici dei fascicoli trovati
                        prjCodes = new StringBuilder();

                        // Aggiunta dei codici dei fascicoli
                        foreach (Fascicolo prj in projects)
                            prjCodes.AppendFormat("{0}, ", prj.codice);

                        // Rimozione dell'ultima virgola
                        prjCodes.Remove(prjCodes.Length - 2, 2);

                        // Viene aggiunta una riga alla lista dei problemi
                        projectProblems.Add(String.Format(
                            "I parametri di ricerca specificati nei campi dedicati alla tipologia fascicolo hanno restituito i seguenti {0} fascicoli: {1}. Provare a restringere il campo di ricerca specificando più parametri o parametri più specifici.",
                                projects.Length, prjCodes));
                    }
                    else
                        // Altrimenti viene aggiunto il system id del fascicolo trovato
                        // alla lista dei system id fascicoli da restituire
                        toReturn.Add(projects[0].systemID);

                }
                catch (Exception e)
                {
                    // Viene aggiunta una riga alla lista dei problemi
                    projectProblems.Add(
                        "Errore durante il reperimento dei dati sul fascicolo specificato.");

                }

            }

            // Impostazione della lista dei problemi
            problems = projectProblems;

            // Restituzione della lista con i system id dei fascicoli in
            // cui fascicolare il documento
            return toReturn;

        }

        /// <summary>
        /// Funzione per la creazione dei filtri di ricerca fascicolo
        /// </summary>
        /// <param name="rowData">L'oggetto con le informazioni sul documento da classificare</param>
        /// <param name="registrySyd">Il system id del registro in cui protocollare</param>
        /// <param name="rfSyd">Il system id dell'RF in cui protocollare</param>
        /// <param name="idTitolario">L'id del titolario</param>
        /// <param name="administrationSyd">Il system id dell'amministrazione</param>
        /// <param name="role">Il ruolo dell'utente che ha lanciato l'operazione</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="isSmistamentoEnabled">True se è abilitato lo smistamento</param>
        /// <returns>La lista dei filtri di ricerca</returns>
        private FiltroRicerca[] GetFilters(DocumentRowData rowData, string registrySyd, string rfSyd, string idTitolario, string administrationSyd, Ruolo role, InfoUtente userInfo, bool isSmistamentoEnabled)
        {
            #region Dichiarazione variabili

            // La lista dei filtri da restituire
            List<FiltroRicerca> toReturn;

            // Filtro temporaneo
            FiltroRicerca tempFilter;

            // La lista dei template
            Templates[] templates = null;

            // Il template da utilizzare per la profilazione
            Templates template = null;

            // I campi
            string[] values;

            // Il tipo di utente da ricercare
            TipoUtente userType;

            #endregion

            // Creazione della lista dei filtri
            toReturn = new List<FiltroRicerca>();

            #region Filtro Codice Fascicolo

            // Se è valorizzato il codice fascicolo, si procede alla creazione
            // di un filtro per esso
            if (rowData.ProjectCodes != null && !String.IsNullOrEmpty(rowData.ProjectCodes[0]))
            {
                // Creazione di un filtro temporaneo
                tempFilter = new FiltroRicerca();

                // Impostazione del nome del campo
                tempFilter.argomento = "NUMERO_FASCICOLO";

                // Impostazione del valore
                tempFilter.valore = rowData.ProjectCodes[0];

                // Aggiunta del filtro alla lista dei filtri
                toReturn.Add(tempFilter);

            }

            #endregion

            #region Filtro Descrizione Fascicolo

            // Se è valorizzata la descrizione del fascicolo,
            // si procede alla creazione di un filtro per essa
            if (!String.IsNullOrEmpty(rowData.ProjectDescription))
            {
                // Creazione di un filtro temporaneo
                tempFilter = new FiltroRicerca();

                // Impostazione del nome del campo
                tempFilter.argomento = "TITOLO";

                // Impostazione del valore
                tempFilter.valore = rowData.ProjectDescription;

                // Aggiunta del filtro alla lista dei filtri
                toReturn.Add(tempFilter);
            }

            #endregion

            #region Filtro Descrizione Sottofascicolo

            // Se è valorizzata la descrizione del sottofascicolo,
            // si procede alla creazione di un filtro per essa
            if (!String.IsNullOrEmpty(rowData.FolderDescrition))
            {
                // Creazione di un filtro temporaneo
                tempFilter = new FiltroRicerca();

                // Impostazione del nome del campo
                tempFilter.argomento = "SOTTOFASCICOLO";

                // Impostazione del valore
                tempFilter.valore = rowData.FolderDescrition;

                // Aggiunta del filtro alla lista dei filtri
                toReturn.Add(tempFilter);
            }

            #endregion

            #region Filtro Titolario

            // Se il parametro idTitolario è valorizzato,
            // viene creato un filtro per esso
            if (!String.IsNullOrEmpty(idTitolario))
            {
                // Creazione di un filtro temporaneo
                tempFilter = new FiltroRicerca();

                // Impostazione del nome del campo
                tempFilter.argomento = "ID_TITOLARIO";

                // Impostazione del valore
                tempFilter.valore = idTitolario;

                // Aggiunta del filtro alla lista dei filtri
                toReturn.Add(tempFilter);
            }

            #endregion

            #region Filtro Codice Nodo

            // Se è valorizzato il codice nodo,
            // si procede alla creazione di un filtro per esso
            if (!String.IsNullOrEmpty(rowData.NodeCode))
            {
                // Creazione di un filtro temporaneo
                tempFilter = new FiltroRicerca();

                // Impostazione del nome del campo
                tempFilter.argomento = "CODICE_CLASSIFICA";

                // Impostazione del valore
                tempFilter.valore = rowData.NodeCode;

                // Aggiunta del filtro alla lista dei filtri
                toReturn.Add(tempFilter);
            }

            #endregion

            #region Filtro Tipologia Fascicolo e Campi Profilati

            // Se è valorizzato il campo tipologia, vengono creati,
            // i filtri di ricerca per la profilazione
            if (!String.IsNullOrEmpty(rowData.ProjectTipology))
            {

                try
                {
                    // Prelevamento della lista dei template creati per l'amministrazione
                    templates = (Templates[])ProfilazioneFascicoli.getTipoFascFromRuolo(
                        administrationSyd,
                        role.idGruppo,
                        "1").ToArray(typeof(Templates));

                    // Prelevamento del template di interesse
                    template = ProfilazioneFascicoli.getTemplateFascById(templates.Where(
                        e => e.DESCRIZIONE.ToUpper().TrimEnd().TrimStart() == rowData.ProjectTipology.TrimEnd().TrimStart()).
                        FirstOrDefault().SYSTEM_ID.ToString());
                }
                catch (Exception e)
                {
                }

                // Se è stato rilevato un template, si procede con la
                // compilazione dei campi relativi alla profilazione
                if (template != null)
                {
                    // Il primo campo riporta l'id del template
                    // Creazione di un filtro temporaneo
                    tempFilter = new FiltroRicerca();

                    // Impostazione della tipologia fascicolo
                    tempFilter.argomento = "TIPOLOGIA_FASCICOLO";

                    // Impostazione del valore
                    tempFilter.valore = template.SYSTEM_ID.ToString();

                    // Aggiunta del filtro alla lista dei filtri
                    toReturn.Add(tempFilter);

                    // Aggiunta delle informazioni sul template
                    tempFilter = new FiltroRicerca();

                    // Impostazione dell'argomento
                    tempFilter.argomento = "PROFILAZIONE_DINAMICA";

                    // Impostazione del valore
                    tempFilter.valore = " Profilazione Dinamica";

                    // Impostazione del template
                    tempFilter.template = template;

                    // Per ogni oggetto custom...
                    foreach (OggettoCustom obj in template.ELENCO_OGGETTI)
                    {
                        // ...switch sul tipo di oggetto
                        switch (obj.TIPO.DESCRIZIONE_TIPO.ToUpper())
                        {
                            case "CONTATORE":
                                try
                                {
                                    // Prelevamento dei valori assegnati al campo
                                    values = rowData.GetProjectProfilationField(obj.DESCRIZIONE);

                                    // Nel caso di contatore bisogna impostare per prima cosa
                                    // l'id del registro
                                    obj.ID_AOO_RF = ImportUtils.GetRegistrySystemId(
                                        values[0],
                                        rowData.AdminCode);

                                    // Se l'array dei valori contiene il secondo elemento, ...
                                    if (values.Length > 1)
                                        // ...viene impostato il valore minimo
                                        obj.VALORE_DATABASE = values[1] + "@";

                                    // Se l'array contiene il terzo campo...
                                    if (values.Length > 2)
                                        // ...viene impostato il valore massimo
                                        obj.VALORE_DATABASE += values[2];

                                }
                                catch (Exception e)
                                { }

                                break;

                            case "DATA":
                                // Prelevamento dei valori associati al campo
                                values = rowData.GetProjectProfilationField(obj.DESCRIZIONE);

                                // Se l'array è valorizzato...
                                if (values != null)
                                {
                                    // ...se contiene il primo elemento...
                                    if (values.Length > 0)
                                        // ...il primo campo è il valore data minimo
                                        obj.VALORE_DATABASE = values[0] + "@";

                                    // ...se è presente anche il secondo elemento...
                                    if (values.Length > 1)
                                        // ...il secondo è il valore di data massima
                                        obj.VALORE_DATABASE += values[1];

                                }

                                break;

                            case "CORRISPONDENTE":
                                try
                                {
                                    switch (obj.TIPO_RICERCA_CORR.ToUpper())
                                    {
                                        case "INTERNO":
                                            userType = TipoUtente.INTERNO;
                                            break;

                                        case "ESTERNO":
                                            userType = TipoUtente.ESTERNO;
                                            break;

                                        default:
                                            userType = TipoUtente.GLOBALE;
                                            break;

                                    }

                                    // Prelevamento del system id del corrispondente
                                    obj.VALORE_DATABASE = ImportUtils.GetCorrispondenteByCode(
                                        ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST,
                                        rowData.GetProjectProfilationField(obj.DESCRIZIONE)[0],
                                        role,
                                        userInfo,
                                        registrySyd,
                                        rfSyd,
                                        isSmistamentoEnabled,
                                        userType).systemId;

                                }
                                catch (Exception e) { }

                                break;

                            case "CASELLADISELEZIONE":
                                try
                                {
                                    // In questo caso è possibile che siano selezionati
                                    // più valori
                                    obj.VALORI_SELEZIONATI = new ArrayList(
                                        rowData.GetProjectProfilationField(obj.DESCRIZIONE));
                                }
                                catch (Exception e) { }

                                break;

                            default:
                                try
                                {
                                    // In tutti gli alti casi è ammesso un solo valore
                                    obj.VALORE_DATABASE = rowData.GetProjectProfilationField(obj.DESCRIZIONE)[0];
                                }
                                catch (Exception e)
                                {
                                }

                                break;

                        }

                    }

                    // Aggiunta del filtro alla lista dei filtri
                    toReturn.Add(tempFilter);

                }

            }

            #endregion

            // Restituzione della lista dei filtri
            return toReturn.ToArray<FiltroRicerca>();

        }

        /// <summary>
        /// Funzione per la protocollazione del documento
        /// </summary>
        /// <param name="schedaDocumento">La scheda documento con le informazioni sul documento da protocollare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        protected void ProtocolDocument(SchedaDocumento schedaDocumento, InfoUtente userInfo, Ruolo role)
        {
            // Il risultato della protocollazione
            ResultProtocollazione result = ResultProtocollazione.OK;

            try
            {
                // Protocollazione del documento
                schedaDocumento = ProtoManager.protocolla(
                    schedaDocumento,
                    role,
                    userInfo,
                    out result);
            }
            catch (Exception e)
            {
                // Un'eccezione corrisponde ad un risultato. Restituzione di un
                // messaggio appropriato in base al valore del risultato
                this.GenerateProtoException(result);

            }

            // Se result è diverso da ok viene restituito un errore
            if (result != ResultProtocollazione.OK)
                throw new Exception(
                    String.Format("Errore non identificato durante la protocollazione del documento ({0})",
                    result));

        }

        /// <summary>
        /// Funzione per la costruzione della stringa identificativa del protocollo creato
        /// </summary>
        /// <param name="schedaDocumento">La scheda del documento protocollato</param>
        /// <param name="isGray">True se il documento è un grigio</param>
        /// <returns>La string aidentificativa del documento</returns>
        protected string GetIdentificationData(SchedaDocumento schedaDocumento, bool isGray)
        {
            // La stringa da restituire
            string toReturn = String.Empty;

            // Se docType è un documento grigio
            if (isGray)
                // L'identificativo è l'id del documento
                toReturn = String.Format(
                    "Id documento: {0}",
                    schedaDocumento.systemId);
            else
                // Altrimenti l'identificativo è costituito dalla segnatura e dall'id del documento
                toReturn = String.Format(
                    "Id documento: {0} - Segnatura: {1}",
                    schedaDocumento.systemId,
                    schedaDocumento.protocollo.segnatura);

            // Restituzione della descrizione
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'analisi del risultato di protocollazione e generazione
        /// di un'eccezione appropriata
        /// </summary>
        /// <param name="result">Il risultato da analizzare</param>
        private void GenerateProtoException(ResultProtocollazione result)
        {
            // Il messaggio da impostare nell'eccezione
            StringBuilder exceptionMessage = new StringBuilder("Errore in fase di protocollazione: ");

            switch (result)
            {
                case ResultProtocollazione.AMMINISTRAZIONE_MANCANTE:
                    exceptionMessage.Append("Amministrazione mancante");
                    break;
                case ResultProtocollazione.APPLICATION_ERROR:
                    exceptionMessage.Append("Errore applicativo");
                    break;
                case ResultProtocollazione.DATA_ERRATA:
                    exceptionMessage.Append("Data non valida");
                    break;
                case ResultProtocollazione.DATA_SUCCESSIVA_ATTUALE:
                    exceptionMessage.Append("Data non valida");
                    break;
                case ResultProtocollazione.DESTINATARIO_MANCANTE:
                    exceptionMessage.Append("Destinatario mancante");
                    break;
                case ResultProtocollazione.DOCUMENTO_GIA_PROTOCOLLATO:
                    exceptionMessage.Append("Documento già protocollato");
                    break;
                case ResultProtocollazione.ERRORE_DURANTE_LA_FASCICOLAZIONE:
                    exceptionMessage.Append("Errore durante la fascicolazione");
                    break;
                case ResultProtocollazione.FASCICOLO_NON_TROVATO:
                    exceptionMessage.Append("Fascicolo non trovato");
                    break;
                case ResultProtocollazione.FORMATO_SEGNATURA_MANCANTE:
                    exceptionMessage.Append("Formato segnatura mancante");
                    break;
                case ResultProtocollazione.MITTENTE_MANCANTE:
                    exceptionMessage.Append("Mittente mancante");
                    break;
                case ResultProtocollazione.OGGETTO_MANCANTE:
                    exceptionMessage.Append("Oggetto mancante");
                    break;
                case ResultProtocollazione.REGISTRO_CHIUSO:
                    exceptionMessage.Append("Registro chiuso");
                    break;
                case ResultProtocollazione.REGISTRO_MANCANTE:
                    exceptionMessage.Append("Registro specificato non valido");
                    break;
                case ResultProtocollazione.STATO_REGISTRO_ERRATO:
                    exceptionMessage.Append("Stato del registro specificato non valido");
                    break;

            }

            // Generazione eccezione
            throw new Exception(exceptionMessage.ToString());

        }

        #endregion

        #region Metodi astratti

        protected abstract DocumentRowData ReadDataFromCurrentRow(OleDbDataReader row, InfoUtente userInfo, Ruolo role, bool isEnabledPregressi);

        protected abstract bool CheckDataValidity(DocumentRowData rowData, bool isProfilationRequired, bool isRapidClassificationRequired, out List<string> validationProblems);

        protected abstract Protocollo CreateProtocolObject(DocumentRowData rowData, string registrySyd, string rfSyd, string administrationSyd, bool isSmistamentoEnabled, InfoUtente userInfo, Ruolo role);

        #endregion

    }

}
