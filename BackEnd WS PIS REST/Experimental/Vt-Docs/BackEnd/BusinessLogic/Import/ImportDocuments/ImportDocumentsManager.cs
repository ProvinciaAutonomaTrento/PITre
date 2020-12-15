using System;
using System.Collections.Generic;
using System.Linq;
using DocsPaVO.PrjDocImport;
using DocsPaVO.utente;
using System.Data.OleDb;
using BusinessLogic.Import.ImportDocuments.DocumentType;
using DocsPaVO.documento;
using BusinessLogic.Documenti;
using BusinessLogic.Import.Template;
using BusinessLogic.Import.Template.Builders;
using System.IO;
using log4net;
namespace BusinessLogic.Import.ImportDocuments
{
    /// <summary>
    /// Questa classe si occupa di effettura la creazione di documenti a partire da dati
    /// contenuti in un foglio excel il cui content viene passato per parametro
    /// </summary>
    public class ImportDocumentsManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(ImportDocumentsManager));
        public enum ProtoType
        {
            A,  // Arrivo
            P,  // Partenza
            I,  // Interno
            G,  // Grigio
            ATT // Allegato
        }

        #region Funzioni pubbliche

        /// <summary>
        /// Funzione per l'importazione di documenti definiti all'interno di un file Excel
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fileName"></param>
        /// <param name="serverPath"></param>
        /// <param name="modelPath"></param>
        /// <param name="userInfo"></param>
        /// <param name="role"></param>
        /// <param name="isProfilationRequired"></param>
        /// <param name="isRapidClassificationRequired"></param>
        /// <param name="sharedDirectoryPath"></param>
        /// <param name="isSmistamentoEnabled"></param>
        /// <returns></returns>
        public static ResultsContainer ImportDocuments(
            byte[] content,
            string fileName,
            string serverPath,
            string modelPath,
            InfoUtente userInfo,
            Ruolo role,
            bool isProfilationRequired,
            bool isRapidClassificationRequired,
            string ftpAddress,
            bool isSmistamentoEnabled,
            String ftpUsername,
            String ftpPassword,
            bool isEnabledPregressi)
        {
            // Il path completo in cui è posizionato il file excel contenente i dati
            // sui documenti da importare
            string completePath = String.Empty;

            // Il risultato dell'elaborazione
            ResultsContainer result = new ResultsContainer();

            try
            {
                // 1. Creazione del file temporaneo in cui poggiare il foglio 
                // excel contenente i dati sui documenti da importare
                completePath = ImportUtils.CreateTemporaryFile(content, modelPath, fileName);

                // 2. Creazione dei documenti
                result = CreateDocuments(completePath, userInfo, role, serverPath, isProfilationRequired, isRapidClassificationRequired, ftpAddress, isSmistamentoEnabled, ftpUsername, ftpPassword, isEnabledPregressi);
                // 3. Cancellazione file temporaneo
                ImportUtils.DeleteTemporaryFile(completePath);
            }
            catch (Exception e)
            {
                // Se il file è stato creato, cancellazione
                if (!String.IsNullOrEmpty(completePath))
                    ImportUtils.DeleteTemporaryFile(completePath);

                // Creazione di un nuovo risultato con i dettagli dell'eccezione
                result.General.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = e.Message
                    });
            }

            // 4. Restituzione del risultato
            return result;

        }

        /// <summary>
        /// Funzione per l'importazione di documenti definiti in una lista
        /// di oggetti di tipo DocumentRowData
        /// </summary>
        /// <param name="documentRowDataList">La lista degli oggetti con le informazioni sui documenti da importare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Le informazioni sul ruolo che ha lanciato la procedura</param>
        /// <param name="serverPath">L'indirizzo della WA</param>
        /// <param name="isProfilationRequired">True se è richiesta la profilazione obbligatoria</param>
        /// <param name="isRapidClassificationRequired">True se è richiesta classificazione rapida</param>
        /// <param name="sharedDirectoryPath">L'indirizzo della cartella sharata in cui sono reperibili i file da associare al documento</param>
        /// <param name="isSmistamentoEnabled">True se è abilitato lo smiatamento</param>
        /// <param name="protoType">Il tipo di documento da creare</param>
        /// <param name="resultsContainer"></param>
        /// <returns>La lista dei risultati di importazione</returns>
        public static List<ImportResult> ImportDocuments(
            List<DocumentRowData> documentRowDataList,
            InfoUtente userInfo,
            Ruolo role,
            string serverPath,
            bool isProfilationRequired,
            bool isRapidClassificationRequired,
            string ftpAddress,
            bool isSmistamentoEnabled,
            ProtoType protoType,
            ref ResultsContainer resultsContainer,
            String ftpUsername,
            String ftpPassword,
            bool isEnabledPregressi)
        {
            #region Dichiarazione variabili

            // L'oggetto da restituire
            List<ImportResult> toReturn = null;

            #endregion

            switch (protoType)
            {
                case ProtoType.ATT:
                    if (resultsContainer != null)
                        toReturn = new Attachment().ImportDocuments(
                            documentRowDataList,
                            userInfo,
                            role,
                            ftpAddress,
                            ref resultsContainer,
                            ftpUsername,
                            ftpPassword);
                    break;

                case ProtoType.A:
                case ProtoType.G:
                case ProtoType.I:
                case ProtoType.P:
                    // Si recupera l'oggetto responsabile dell'import
                    // e si esegue l'importazione
                    toReturn = GetDocumentManager(protoType).ImportDocuments(
                        documentRowDataList,
                        userInfo,
                        role,
                        serverPath,
                        isProfilationRequired,
                        isRapidClassificationRequired,
                        ftpAddress,
                        protoType == ProtoType.G,
                        isSmistamentoEnabled,
                        protoType.ToString(),
                        ftpUsername,
                        ftpPassword,
                        isEnabledPregressi);
                    break;
            }

            // Restituzione del risultato
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'importazione di un documento i cui dati sono descritti
        /// all'interno di un oggetto DocumentRowData
        /// </summary>
        /// <param name="documentRowData">L'oggetto con i dati sul documento da creare</param>
        /// <param name="userInfo">Le informaizoni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="serverPath">L'indirizzo della WA</param>
        /// <param name="isProfilationRequired">True se è richiesta la profilazione</param>
        /// <param name="isRapidClassificationRequired">True se è richiesta la classificazione rapida</param>
        /// <param name="ftpAddress">L'indirizzo della cartella sherata in cui è possibile reperire l'immagine da associare al documento</param>
        /// <param name="isSmistamentoEnabled">True se è abilitato lo smistamento</param>
        /// <param name="protoType">Il tipo di documento da creare</param>
        /// <param name="resultsContainer"></param>
        /// <returns>Il risultato dell'importazione</returns>
        public static ImportResult ImportDocument(
            DocumentRowData documentRowData,
            InfoUtente userInfo,
            Ruolo role,
            string serverPath,
            bool isProfilationRequired,
            bool isRapidClassificationRequired,
            string ftpAddress,
            bool isSmistamentoEnabled,
            ProtoType protoType,
            ref ResultsContainer resultsContainer,
            String ftpUsername,
            String ftpPassword,
            bool isEnabledPregressi)
        {
            #region Dichiarazione variabili

            // L'oggetto da restituire
            ImportResult toReturn = null;

            #endregion

            switch (protoType)
            {
                case ProtoType.ATT:
                    if (resultsContainer != null)
                        toReturn = new Attachment().ImportDocument(
                            documentRowData,
                            userInfo,
                            role,
                            ftpAddress,
                            ref resultsContainer,
                            ftpUsername,
                            ftpPassword
                            );
                    break;

                case ProtoType.A:
                case ProtoType.G:
                case ProtoType.I:
                case ProtoType.P:
                    // Si recupera l'oggetto responsabile dell'import
                    // e si esegue l'importazione
                    toReturn = GetDocumentManager(protoType).ImportDocument(
                        documentRowData,
                        userInfo,
                        role,
                        serverPath,
                        isProfilationRequired,
                        isRapidClassificationRequired,
                        ftpAddress,
                        protoType == ProtoType.G,
                        isSmistamentoEnabled,
                        protoType.ToString(),
                        ftpUsername,
                        ftpPassword,
                        isEnabledPregressi);
                    break;
            }

            // Restituzione del risultato
            return toReturn;
            
        }

        /// <summary>
        /// Funzione per l'importazione di un documento i cui dati sono descritti
        /// all'interno di un oggetto DocumentRowData
        /// </summary>
        /// <param name="documentRowData">L'oggetto con i dati sul documento da creare</param>
        /// <param name="userInfo">Le informaizoni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="serverPath">L'indirizzo della WA</param>
        /// <param name="isProfilationRequired">True se è richiesta la profilazione</param>
        /// <param name="isRapidClassificationRequired">True se è richiesta la classificazione rapida</param>
        /// <param name="sharedDirectoryPath">L'indirizzo della cartella sherata in cui è possibile reperire l'immagine da associare al documento</param>
        /// <param name="isSmistamentoEnabled">True se è abilitato lo smistamento</param>
        /// <param name="protoType">Il tipo di documento da creare</param>
        /// <param name="resultsContainer"></param>
        /// <param name="acquireDelegate">Il delegate per l'acquisizione</param>
        /// <returns>Il risultato dell'importazione</returns>
        public static ImportResult ImportAndAcquireDocument(
            DocumentRowData documentRowData,
            InfoUtente userInfo,
            Ruolo role,
            string serverPath,
            bool isProfilationRequired,
            bool isRapidClassificationRequired,
            string ftpAddress,
            bool isSmistamentoEnabled,
            ProtoType protoType,
            ref ResultsContainer resultsContainer,
            String ftpUsername,
            String ftpPassword,
            bool isEnabledPregressi)
        {
            #region Dichiarazione variabili

            // L'oggetto da restituire
            ImportResult toReturn = null;

            #endregion

            switch (protoType)
            {
                case ProtoType.ATT:
                    if (resultsContainer != null)
                        toReturn = new Attachment().ImportDocument(
                            documentRowData,
                            userInfo,
                            role,
                            ftpAddress,
                            ref resultsContainer,
                            ftpUsername,
                            ftpPassword);
                    break;

                case ProtoType.A:
                case ProtoType.G:
                case ProtoType.I:
                case ProtoType.P:
                    // Si recupera l'oggetto responsabile dell'import
                    // e si esegue l'importazione
                    toReturn = GetDocumentManager(protoType).ImportDocument(
                        documentRowData,
                        userInfo,
                        role,
                        serverPath,
                        isProfilationRequired,
                        isRapidClassificationRequired,
                        ftpAddress,
                        protoType == ProtoType.G,
                        isSmistamentoEnabled,
                        protoType.ToString(),
                        ftpUsername,
                        ftpPassword,
                        isEnabledPregressi,
                        AcquireFileFromModel);
                    break;
            }

            // Restituzione del risultato
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'estrazione di dati da una sorgente dati excel
        /// </summary>
        /// <param name="content">Il contenuto del foglio excel</param>
        /// <param name="modelPath">Il path in cui andare a salvare il file temporaneo</param>
        /// <param name="fileName">Il nome da associare al file temporaneo</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <returns>Un oggetto con le informazioni estratte dalla cartella excel relativi a tutte le tipologie di documenti</returns>
        public static DocumentRowDataContainer ReadDataFromExcelFile(
            byte[] content,
            string modelPath,
            string fileName,
            InfoUtente userInfo,
            Ruolo role,
            bool isEnabledPregressi,
            bool isStampaUnione,
            String provider,
            String extendedProperty)
        {
            // Il path in cui è stato creato il file temportaneo
            string completePath;

            // L'oggetto da restituire
            DocumentRowDataContainer toReturn;

            // 1. Creazione del file temporaneo in cui poggiare il foglio 
            // excel contenente i dati sui documenti da importare
            completePath = ImportUtils.CreateTemporaryFile(content, modelPath, fileName);

            // 2. Lettura metadati
            try
            {
                toReturn = ReadMetadata(completePath, userInfo, role, isEnabledPregressi,isStampaUnione,provider, extendedProperty);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

                // 3. Cancellazione file temporaneo
                ImportUtils.DeleteTemporaryFile(completePath);
            }

            // Restituzione del risultato
            return toReturn;

        }

        #endregion

        #region Lettura metadati

        /// <summary>
        /// Funzione per la lettura dei metadati da un foglio excel
        /// </summary>
        /// <param name="completePath">Il path in cui reprire il file excel</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="isEnabledPregressi">True se è abilitato l'import dei documenti pregressi</param>
        /// <returns>L'oggetto con i metadati estratti dal foglio excel</returns>
        private static DocumentRowDataContainer ReadMetadata(
            string completePath,
            InfoUtente userInfo,
            Ruolo role,
            bool isEnabledPregressi,
            bool isStampaUnione,
            String provider,
            String extendedProperty)
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

            // L'oggetto da restituire
            DocumentRowDataContainer toReturn;

            #endregion

            #region Connessione al foglio excel

            try
            {
                // Creazione dell'oggetto per la connessione al foglio excel
                oleConnection = ImportUtils.ConnectToFile(provider, extendedProperty, completePath);



            }
            catch (Exception e)
            {
                // Viene rilanciata un'eccezione al livello superiore
                throw new Exception("Errore durante la connessione al file excel. Dettagli: " + e.Message);

            }
           
            #endregion

            // Creazione dell'oggetto da restituire
            toReturn = new DocumentRowDataContainer();

        

            #endregion

            #region Selezione dei dati sui documenti in partenza da importare e importazione

            try
            {
                // Creazione della query per la selezione dei dati sui documenti in partenza da importare
                oleCommand = new OleDbCommand("SELECT * FROM [Partenza$]", oleConnection);

                // Esecuzione della query per il recupero dei dati
                dataReader = oleCommand.ExecuteReader();

            }
            catch (Exception e)
            {
                dataReader.Close();
                throw new Exception("Errore durante il recupero dei dati sui documenti in uscita da importare. Dettagli: " + e.Message);
            }

            // Se il recupero dei dati è andato a buon fine, si procede con la loro importazione
            try
            {
                if (dataReader != null)
                    toReturn.OutDocument = new OutDocument().ReadMetaData(dataReader, userInfo, role, isEnabledPregressi);
            }
            catch (Exception e)
            {
                dataReader.Close();
                throw e;
            }
            #endregion

            #region Selezione dei dati sui documenti interni da importare e importazione

            try
            {
                // Creazione della query per la selezione dei dati sui documenti interni da importare
                oleCommand = new OleDbCommand("SELECT * FROM [Interni$]", oleConnection);

                // Esecuzione della query per il recupero dei dati
                dataReader = oleCommand.ExecuteReader();

            }
            catch (Exception e)
            {
                dataReader.Close();
                throw new Exception("Errore durante il recupero dei dati sui documenti interni da importare. Dettagli: " + e.Message);
            }

            // Se il recupero dei dati è andato a buon fine, si procede con la loro importazione
            try
            {
                if (dataReader != null)
                    toReturn.OwnDocument = new OwnDocument().ReadMetaData(dataReader, userInfo, role, isEnabledPregressi);
            }
            catch (Exception e)
            {
                dataReader.Close();
                throw e;
            }

            #endregion

            #region Selezione dei dati sui documenti non protocollati da importare e importazione

            try
            {
                // Creazione della query per la selezione dei dati sui documenti non protocollati da importare
                oleCommand = new OleDbCommand("SELECT * FROM [Non protocollati$]", oleConnection);

                // Esecuzione della query per il recupero dei dati
                dataReader = oleCommand.ExecuteReader();

            }
            catch (Exception e)
            {
                dataReader.Close();
                throw new Exception("Errore durante il recupero dei dati sui documenti non protocollati da importare. Dettagli: " + e.Message);
            }

            // Se il recupero dei dati è andato a buon fine, si procede con la loro importazione
            try
            {
                if (dataReader != null)
                    toReturn.GrayDocument = new GrayDocument().ReadMetaData(dataReader, userInfo, role, isEnabledPregressi);
            }
            catch (Exception e)
            {
                dataReader.Close();
                throw e;
            }

            #endregion

            #region Selezione dei dati sui documenti allegati da importare e importazione
            if (isStampaUnione)
            {
                try
                {
                    // Creazione della query per la selezione dei dati sui documenti allegati da importare
                    oleCommand = new OleDbCommand("SELECT * FROM [Allegati$]", oleConnection);

                    // Esecuzione della query per il recupero dei dati
                    dataReader = oleCommand.ExecuteReader();
                }
                catch (Exception e)
                {
                    dataReader.Close();
                    throw new Exception("Errore durante il recupero dei dati sugli allegati da importare. Dettagli: " + e.Message);
                }

                // Se il recupero dei dati è andato a buon fine, si procede con la loro importazione
                try
                {
                    if (dataReader != null)
                        toReturn.AttachmentDocument = new Attachment().ReadMetaData(dataReader, userInfo, role, isEnabledPregressi);
                }
                catch (Exception e)
                {
                    dataReader.Close();
                    throw e;

                }

                #region Selezione dei dati sui documenti in arrivo da importare e importazione

                try
                {
                    // Creazione della query per la selezione dei dati sui documenti in arrivo da importare
                    oleCommand = new OleDbCommand("SELECT * FROM [Arrivo$]", oleConnection);

                    // Esecuzione della query per il recupero dei dati
                    dataReader = oleCommand.ExecuteReader();

                }
                catch (Exception e)
                {
                    dataReader.Close();

                    throw new Exception("Errore durante il recupero dei dati sui documenti in arrivo da importare. Dettagli: " + e.Message);
                }

                // Se il recupero dei dati è andato a buon fine, si procede con la loro importazione
                try
                {
                    if (dataReader != null)
                        toReturn.InDocument = new InDocument().ReadMetaData(dataReader, userInfo, role, isEnabledPregressi);
                }
                catch (Exception e)
                {
                    dataReader.Close();
                    throw e;
                }

            }
            else
            {
                toReturn.AttachmentDocument = new List<DocumentRowData>();
                toReturn.InDocument = new List<DocumentRowData>();
            }
            #endregion

            #region Chiusura connessione

            try
            {
                // Chiusura della connessione
                oleConnection.Close();
            }
            catch (Exception e)
            { }

            #endregion

            // Restituzione del risultato dell'importazione
            return toReturn;

        }

        #endregion

        #region Import Documenti

       
        private static ResultsContainer CreateDocuments(string completePath, InfoUtente userInfo, Ruolo role, string serverPath, bool isProfilationRequired, bool isRapidClassificationRequired, string ftpAddress, bool isSmistamentoEnabled, String ftpUsername, String ftpPassword, bool isEnabledPregressi)
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
            ResultsContainer result = null;

            // La lista di funzioni associate al ruolo
            Funzione[] functions = null;

            // Un booleano utilizzato per tesare se con il ruolo segnalato può creare
            // creato un documento di un certo tipo
            bool canCreateDocuments = false;

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

            // Creazione della lista dei risultati
            result = new ResultsContainer();

            // Recupero delle funzioni associate al ruolo
            functions = (Funzione[])role.funzioni.ToArray(typeof(Funzione));

            #region Selezione dei dati sui documenti in arrivo da importare e importazione

            // Verifica della possibilità di creare documenti in arrivo con il ruolo
            // role
            canCreateDocuments = functions.Where(e => e.codice == "PROTO_IN").FirstOrDefault() != null;

            // Se con il ruolo corrente è possibile creare documenti in arrivo,
            // si procede alla creazione
            if (canCreateDocuments)
            {
                try
                {
                    // Creazione della query per la selezione dei dati sui documenti in arrivo da importare
                    oleCommand = new OleDbCommand("SELECT * FROM [Arrivo$]", oleConnection);

                    // Esecuzione della query per il recupero dei dati
                    dataReader = oleCommand.ExecuteReader();

                }
                catch (Exception e)
                {

                    // Viene inserito un risultato negativo
                    result.InDocument.Add(new ImportResult()
                        {
                            Outcome = ImportResult.OutcomeEnumeration.KO,
                            Message = "Errore durante il recupero dei dati sui documenti in arrivo da importare. Dettagli: " + e.Message
                        });

                }

                // Se il recupero dei dati è andato a buon fine, si procede con la loro importazione
                if (dataReader != null)
                    result.InDocument.AddRange(new InDocument().ImportDocuments(
                        dataReader,
                        userInfo,
                        role,
                        serverPath,
                        isProfilationRequired,
                        isRapidClassificationRequired,
                        ftpAddress,
                        false,
                        isSmistamentoEnabled,
                        "A",
                        ftpUsername,
                        ftpPassword,
                        isEnabledPregressi));
            }
            else
                // Altrimenti aggiunta di un result negativo
                result.InDocument.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = "Ruolo non abilitato alla creazione di documenti in arrivo."
                    });

            #endregion

            #region Selezione dei dati sui documenti in partenza da importare e importazione

            // Verifica della possibilità di creare documenti in uscita con il ruolo
            // role
            canCreateDocuments = functions.Where(e => e.codice == "PROTO_OUT").FirstOrDefault() != null;

            // Se con il ruolo corrente è possibile creare documenti in partenza,
            // si procede alla creazione
            if (canCreateDocuments)
            {
                try
                {
                    // Creazione della query per la selezione dei dati sui documenti in partenza da importare
                    oleCommand = new OleDbCommand("SELECT * FROM [Partenza$]", oleConnection);

                    // Esecuzione della query per il recupero dei dati
                    dataReader = oleCommand.ExecuteReader();

                }
                catch (Exception e)
                {
                    // Viene inserito un risultato negativo
                    result.OutDocument.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = "Errore durante il recupero dei dati sui documenti in partenza da importare. Dettagli: " + e.Message
                    });

                }

                if (dataReader != null)
                    result.OutDocument.AddRange(new OutDocument().ImportDocuments(
                        dataReader,
                        userInfo,
                        role,
                        serverPath,
                        isProfilationRequired,
                        isRapidClassificationRequired,
                        ftpAddress,
                        false,
                        isSmistamentoEnabled,
                        "P",
                        ftpUsername,
                        ftpPassword,
                        isEnabledPregressi));
            }
            else
                // Altrimenti aggiunta di un result negativo
                result.OutDocument.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = "Utente non abilitato alla creazione di documenti in partenza."
                });

            #endregion

            #region Selezione dei dati sui documenti interni da importare e importazione

            // Verifica della possibilità di creare documenti interni con il ruolo
            // role
            canCreateDocuments = functions.Where(e => e.codice == "PROTO_OWN").FirstOrDefault() != null;

            // Se con il ruolo corrente è possibile creare documenti interni,
            // si procede alla creazione
            if (canCreateDocuments)
            {
                try
                {
                    // Creazione della query per la selezione dei dati sui documenti interni da importare
                    oleCommand = new OleDbCommand("SELECT * FROM [Interni$]", oleConnection);

                    // Esecuzione della query per il recupero dei dati
                    dataReader = oleCommand.ExecuteReader();

                }
                catch (Exception e)
                {
                    // Viene inserito un risultato negativo
                    result.OwnDocument.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = "Errore durante il recupero dei dati sui documenti interni da importare. Dettagli: " + e.Message
                    });

                }

                if (dataReader != null)
                    result.OwnDocument.AddRange(new OwnDocument().ImportDocuments(
                        dataReader,
                        userInfo,
                        role,
                        serverPath,
                        isProfilationRequired,
                        isRapidClassificationRequired,
                        ftpAddress,
                        false,
                        isSmistamentoEnabled,
                        "I",
                        ftpUsername,
                        ftpPassword,
                        isEnabledPregressi));

            }
            else
                // Altrimenti aggiunta di un result negativo
                result.OwnDocument.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = "Utente non abilitato alla creazione di documenti interni."
                    });

            #endregion

            #region Selezione dei dati sui documenti non protocollati da importare e importazione

            // Verifica della possibilità di creare documenti non protocollati con il ruolo
            // role
            canCreateDocuments = functions.Where(e => e.codice == "DO_NUOVODOC").FirstOrDefault() != null;

            // Se con il ruolo corrente è possibile creare documenti non protocollati,
            // si procede alla creazione
            if (canCreateDocuments)
            {
                try
                {
                    // Creazione della query per la selezione dei dati sui documenti non protocollati da importare
                    oleCommand = new OleDbCommand("SELECT * FROM [Non protocollati$]", oleConnection);

                    // Esecuzione della query per il recupero dei dati
                    dataReader = oleCommand.ExecuteReader();

                }
                catch (Exception e)
                {
                    // Viene inserito un risultato negativo
                    result.GrayDocument.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = "Errore durante il recupero dei dati sui documenti non protocollati da importare. Dettagli: " + e.Message
                    });

                }

                if (dataReader != null)
                    result.GrayDocument.AddRange(new GrayDocument().ImportDocuments(
                        dataReader,
                        userInfo,
                        role,
                        serverPath,
                        isProfilationRequired,
                        isRapidClassificationRequired,
                        ftpAddress,
                        true,
                        isSmistamentoEnabled,
                        "G",
                        ftpUsername,
                        ftpPassword,
                        false));

            }
            else
                // Altrimenti aggiunta di un result negativo
                result.GrayDocument.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = "Utente non abilitato alla creazione di documenti non protocollati."
                });

            #endregion

            #region Selezione dei dati sui documenti allegati da importare e importazione

            // Verifica della possibilità di creare allegati con il ruolo role
            canCreateDocuments = functions.Where(e => e.codice == "DO_ALL_AGGIUNGI").FirstOrDefault() != null;

            // Se con il ruolo corrente è possibile creare documenti allegati,
            // si procede alla creazione
            if (canCreateDocuments)
            {
                try
                {
                    // Creazione della query per la selezione dei dati sui documenti allegati da importare
                    oleCommand = new OleDbCommand("SELECT * FROM [Allegati$]", oleConnection);

                    // Esecuzione della query per il recupero dei dati
                    dataReader = oleCommand.ExecuteReader();

                }
                catch (Exception e)
                {
                    // Viene inserito un risultato negativo
                    result.Attachment.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = "Errore durante il recupero dei dati sui documenti allegati da importare. Dettagli: " + e.Message
                    });

                }

                if (dataReader != null)
                    result.Attachment.AddRange(new Attachment().ImportDocuments(
                        dataReader,
                        userInfo,
                        role,
                        ftpAddress,
                        ref result,
                        ftpUsername,
                        ftpPassword));

            }
            else
                // Altrimenti aggiunta di un result negativo
                result.Attachment.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = "Utente non abilitato alla creazione di allegati."
                });

            #endregion

            #region Chiusura connessione

            try
            {
                // Chiusura della connessione
                oleConnection.Close();
            }
            catch (Exception e)
            {
                // Viene inserito un risultato negativo
                result.General.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = "Errore durante la chiusura della connessione al file excel. Dettagli: " + e.Message
                    });
            }

            #endregion

            // Restituzione del risultato dell'importazione
            return result;

        }

        #endregion

        /// <summary>
        /// Funzione per la restituzione dell'oggetto preposto all'importazione dei
        /// documenti di una determinata tipologia
        /// </summary>
        /// <param name="protoType">Il tipo di protocollo da importare</param>
        /// <returns>L'oggetto adatto ad effettuare l'importazione dei documenti della tipologia specificata</returns>
        private static Document GetDocumentManager(ProtoType protoType)
        {
            // L'oggetto da restituire
            Document toReturn = null;

            switch (protoType)
            {
                case ProtoType.A:   // Arrivo
                    toReturn = new InDocument();
                    break;

                case ProtoType.P:   // Partenza
                    toReturn = new OutDocument();
                    break;

                case ProtoType.G:   // Grigio
                    toReturn = new GrayDocument();
                    break;

                case ProtoType.I:   // Interno
                    toReturn = new OwnDocument();
                    break;
                    
            }

            // Restituzione del risultato
            return toReturn;
           
        }

        private static void AcquireFileFromModel(DocumentRowData rowData, InfoUtente userInfo, Ruolo role, bool isSmistamentoEnabled,SchedaDocumento schedaDocumento, string ftpAddress, String ftpUsername, String ftpPassword)
        {
            // L'oggetto fileRequest
            FileRequest fileRequest;
            // L'oggetto fileDocumento
            FileDocumento fileDocumento;
            string tipologia = rowData.DocumentTipology;
            TemplateManager tempManager = new TemplateManager(userInfo,role,isSmistamentoEnabled, tipologia, TemplateType.RTF);
            byte[] content= tempManager.BuildDocumentFromTemplate(rowData,schedaDocumento);
            // Creazione dell'oggetto fileDocumento
            fileDocumento = new FileDocumento();
            // Impostazione del nome del file
            fileDocumento.name = Path.GetFileName(schedaDocumento.systemId+".rtf");
            // Impostazione del full name
            fileDocumento.fullName = schedaDocumento.systemId + ".rtf";
            fileDocumento.estensioneFile = "rtf";
            // Impostazione del path
            //fileDocumento.path = Path.GetPathRoot(rowData.Pathname);
            // Impostazione della grandezza del file
            fileDocumento.length = content.Length;
            // Impostazione del content del documento
            fileDocumento.content = content;
            fileRequest = (FileRequest)schedaDocumento.documenti[0];
            try
            {
                FileManager.putFile(fileRequest, fileDocumento, userInfo);
            }
            catch (Exception e)
            {
                // Aggiunta del problema alla lista dei problemi
                throw new Exception("Errore durante l'upload del file.");
            }

        }

        public static byte[] CreateZipFromReport(ResultsContainer report,InfoUtente infoUtente)
        {
            ZipBuilder builder=new ZipBuilder();
            List<ImportResult> grayList = report.GrayDocument.FindAll(e => !string.IsNullOrEmpty(e.DocNumber));
            addDocsInZip(grayList, builder,"Documenti grigi",infoUtente);
            List<ImportResult> inList = report.InDocument.FindAll(e => !string.IsNullOrEmpty(e.DocNumber));
            addDocsInZip(inList, builder, "Documenti in arrivo", infoUtente);
            List<ImportResult> outList = report.OutDocument.FindAll(e => !string.IsNullOrEmpty(e.DocNumber));
            addDocsInZip(outList, builder, "Documenti in partenza", infoUtente);
            List<ImportResult> ownList = report.OwnDocument.FindAll(e => !string.IsNullOrEmpty(e.DocNumber));
            addDocsInZip(ownList, builder, "Documenti interni", infoUtente);
            return builder.GetOutput();
        }

        private static void addDocsInZip(List<ImportResult> results, ZipBuilder builder,string folder,InfoUtente infoUtente)
        {
            foreach (ImportResult temp in results)
            {
                try
                {
                    List<BaseInfoDoc> infos = DocManager.GetBaseInfoForDocument(temp.DocNumber, null, null);
                    BaseInfoDoc doc = infos[0];
                    FileRequest req = new FileRequest();
                    if (doc != null && !string.IsNullOrEmpty(doc.VersionLabel))
                        logger.Debug("addDocsInZip GetBaseInfoForDocument VersionId " + doc.VersionLabel);
                    req.versionId = doc.VersionId;
                    req.docNumber = doc.IdProfile;
                    req.versionLabel = doc.VersionLabel;
                    req.path = doc.Path;
                    req.version = doc.VersionLabel;
                    req.fileName = doc.FileName;
                    FileDocumento fileDocumento = BusinessLogic.Documenti.FileManager.getFile(req, infoUtente);
                    string extension = fileDocumento.fullName.Substring(fileDocumento.fullName.LastIndexOf(".") + 1);
                    builder.AddEntry(temp.DocNumber+"."+extension, new string[] { folder }, fileDocumento.content);
                }
                catch (Exception e)
                {

                }
            }
        }

    }
}