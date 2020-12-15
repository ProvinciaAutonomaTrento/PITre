using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using BusinessLogic.Import.ImportDocuments.DocumentType;
using DocsPaVO.PrjDocImport;
using DocsPaVO.utente;
using BusinessLogic.Import.ImportDocuments;
using System.Text;
using log4net;

namespace BusinessLogic.Import.RDE
{
    /// <summary>
    /// Classe per l'implementazione del motore di importazione per il Registro
    /// Documenti di Emergenza
    /// </summary>
    public class ImportRDE
    {
        private static ILog logger = LogManager.GetLogger(typeof(ImportRDE));

        #region Punto di Accesso

        /// <summary>
        /// Funzione per l'importazione dei documenti RDA
        /// </summary>
        /// <param name="content">Il contenuto del file Excel</param>
        /// <param name="fileName">Il nome da attribuire al file temporaneo</param>
        /// <param name="serverPath">L'indirizzo della WA</param>
        /// <param name="modelPath">Il path in cui sono memorizzati i modelli</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="isRapidClassificationRequired">True se è richiesta classificazione rapida obbligatoria</param>
        /// <param name="isSmistamentoEnabled">True se è abilitato lo smistamento</param>
        /// <param name="sharedDirectoryPath">Il path della cartella condivisa</param>
        /// <param name="provider">Il provider da utilizzare per la connessione</param>
        /// <param name="extendedProperty">Le proprietà estese da utilizzare per l'instaurazione della connessione con il provider</param>
        /// <param name="versionNumber">Il numero di versione dell'importer da utilizzare.
        ///     - 1 per la versione che non contempla i protocolli interni ed i corrispondenti identificati tramite codice
        ///     - 2 per la versione che contempla i protocolli interni ed i corrispondenti identificati tramite codice</param>
        /// <returns>Il report relativo all'importazione RDE</returns>
        public static ResultsContainer CreateRDA(
            byte[] content,
            string fileName,
            string serverPath,
            string modelPath,
            InfoUtente userInfo,
            Ruolo role,
            bool isRapidClassificationRequired,
            bool isSmistamentoEnabled,
            string ftpAddress,
            string provider,
            string extendedProperty,
            int versionNumber,
            String ftpUsername,
            String ftpPassword,
            bool isEnabledPregressi)
        {
            // Il path completo in cui è posizionato il file excel contenente i dati
            // sui documenti da importare
            string completePath = String.Empty;

            // L'oggetto con le informazioni sui documenti RDE da importare
            DocumentRowDataContainer container = null;

            // Il risultato dell'elaborazione
            ResultsContainer result = new ResultsContainer();

            try
            {
                // 1. Creazione del file temporaneo in cui poggiare il foglio 
                // excel contenente i dati sui documenti da importare
                completePath = ImportUtils.CreateTemporaryFile(content, modelPath, fileName);

                // 2. Caricamento dei dati contenuti all'interno della cartella Excel
                container = ReadDataFromExcel(provider, extendedProperty, completePath, versionNumber);

                // 3. Creazione dei documenti
                result = CreateDocuments(container, userInfo, role, serverPath, isRapidClassificationRequired, isSmistamentoEnabled, ftpAddress, ftpUsername, ftpPassword, isEnabledPregressi);               
                
                // 4. Cancellazione file temporaneo
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
        /// Funzione per l'importazione di un documento di emergenza a partire dai dati
        /// descrittivi contenuti all'interno di uno specifico oggetto.
        /// </summary>
        /// <param name="rowData">L'oggetto con le informazioni sul documento da importare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="serverPath">L'indirizzo della WA</param>
        /// <param name="isRapidClassificationRequired">True se è abilitata la classificazione rapida</param>
        /// <param name="isSmistamentoEnabled">True se è abilitato lo smistamento</param>
        /// <param name="sharedDirectoryPath">L'indirizzo della cartella scharata</param>
        /// <param name="protoType">Il tipo di documento da creare</param>
        /// <returns>Il risultato dell'importazione del documento</returns>
        public static ImportResult ImportRDEDocument(
            DocumentRowData rowData,
            InfoUtente userInfo,
            Ruolo role,
            string serverPath,
            bool isRapidClassificationRequired,
            bool isSmistamentoEnabled,
            string ftpAddress,
            bool isProfilationRequired,
            ImportDocumentsManager.ProtoType protoType,
            String ftpUsername,
            String ftpPassword,
            bool isEnabledPregressi)
        {
            #region Dichiarazione variabili

            // Possibilità da parte dell'utente di creare il documento della tipologia richiesta
            bool canCreateDocuments;

            // Il risultato dell'elaborazione
            ImportResult toReturn;

            // Il contenitore dei risultati
            ResultsContainer resultContainer = new ResultsContainer();

            #endregion

            // Verifica della possibilità di creare documenti del tipo specificato
            canCreateDocuments = CanCreateDocumentOfType(protoType, (Funzione[])role.funzioni.ToArray(typeof(Funzione)));

            // Se con il ruolo corrente è possibile creare il documento della tipologi richiesta,
            // si procede alla creazione
            if (canCreateDocuments)
            {

                toReturn = ImportDocumentsManager.ImportDocument(
                    rowData,
                    userInfo,
                    role,
                    serverPath,
                    isProfilationRequired,
                    isRapidClassificationRequired,
                    ftpAddress,
                    isSmistamentoEnabled,
                    protoType,
                    ref resultContainer,
                    ftpUsername,
                    ftpPassword,
                    isEnabledPregressi);

            }
            else
                // Altrimenti aggiunta di un result negativo
                toReturn = new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = "Utente non abilitato alla creazione di documenti di tipo " + protoType + "."
                };

            // Restituzione del risultato
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'estrazione dei dati sui documenti RDE
        /// da importare leggendo i dati da foglio Excel
        /// </summary>
        /// <param name="modelRootPath">Il path in cui andare a salvare il file temporaneo</param>
        /// <param name="provider">Il provider dati da utilizzare per la connessione al file Excel</param>
        /// <param name="extendedProperties">Le proprietà estese da utilizzare per la connessione al file Excel</param>
        /// <param name="fileName">Il nome da attribuire al file temporaneo</param>
        /// <param name="content">Il contenuto del file</param>
        /// <param name="versionNumber">Il numero di versione dell'importer da utilizzare:
        ///     - 1 per la versione che non contempla i protocolli interni ed i corrispondenti identificati tramite codice
        ///     - 2 per la versione che contempla i protocolli interni ed i corrispondenti identificati tramite codice</param>
        /// <returns>Il contenitore dei dati estratti dal foglio</returns>
        public static DocumentRowDataContainer ReadDataFromExcel(
            string modelRootPath,
            string provider,
            string extendedProperties,
            string fileName,
            byte[] content,
            int versionNumber)
        {
            // Il path in cui è stato creato il file temportaneo
            string completePath;

            // L'oggetto da restituire
            DocumentRowDataContainer toReturn;

            // 1. Creazione del file temporaneo in cui poggiare il foglio 
            // excel contenente i dati sui documenti da importare
            completePath = ImportUtils.CreateTemporaryFile(content, modelRootPath, fileName);

            // 2. Lettura metadati
            try
            {
                toReturn = ReadDataFromExcel(
                    provider,
                    extendedProperties,
                    completePath,
                    versionNumber);
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
        
        /// <summary>
        /// Funzione per la determinazione della possibilità da parte dell'utente di creare
        /// documenti di un particolare tipo
        /// </summary>
        /// <param name="protoType">Il tipo di documento che si desidera creare</param>
        /// <param name="functions">La lista di funzioni associate al ruolo</param>
        /// <returns>True se l'utente è abilitato alla creazione del documento del tipo richiesto</returns>
        private static bool CanCreateDocumentOfType(
            ImportDocumentsManager.ProtoType protoType,
            Funzione[] functions)
        {
            // La chiave di cui verificare la presenza nella lista di funzioni
            // associate al ruolo
            string strProtoType = String.Empty;

            switch(protoType)
            {
                case ImportDocumentsManager.ProtoType.A:
                    strProtoType = "PROTO_IN";
                    break;

                case ImportDocumentsManager.ProtoType.P:
                    strProtoType = "PROTO_OUT";
                    break;

                case ImportDocumentsManager.ProtoType.I:
                    strProtoType = "PROTO_OWN";
                    break;

            }

            if (String.IsNullOrEmpty(strProtoType))
                throw new Exception("Tipo documento non valido.");

            // Restituzione del risultato
            return functions.Where(e => e.codice.Equals(strProtoType)).FirstOrDefault() != null;
            
        }
        
        #region Funzioni per estrazione dati relativi a documenti in ingresso, in uscita e interni

        /// <summary>
        /// Funzione per la lettura dei dati dal foglio excel.
        /// </summary>
        /// <param name="completePath">Il path in cui è stato salvato il file temporaneo da cui estrarre i dati</param>
        /// <param name="provider">Il provider da utilizzare per la connessione al foglio Excel</param>
        /// <param name="extendedProperty">Le proprietà estese</param>
        /// <param name="versionNumber">Il numero di versione. 
        ///     - 1 per la versione che non contempla i protocolli interni ed i corrispondenti identificati tramite codice
        ///     - 2 per la versione che contempla i protocolli interni ed i corrispondenti identificati tramite codice</param>
        /// <returns>L'oggetto con i dati estratti dal foglio excel</returns>
        private static DocumentRowDataContainer ReadDataFromExcel(
            string provider,
            string extendedProperty,
            string completePath,
            int versionNumber)
        {
            #region Dichiarazione variabili

            // L'oggetto da restituire
            DocumentRowDataContainer toReturn;

            // L'oggetto per effettuare la connessione al foglio excel
            OleDbConnection oleConnection;

            // L'oggetto per effettuare le query al foglio excel
            OleDbCommand oleCommand;

            // L'oggetto utilizzato per leggere i dati estratti dal foglio
            OleDbDataReader dataReader;

            #endregion

            // Creazione dell'oggetto con i dati estratti dal foglio excel
            toReturn = new DocumentRowDataContainer();

            #region Connessione al foglio excel

            oleConnection = ImportUtils.ConnectToFile(provider, extendedProperty, completePath);

            #endregion

            try
            {
                #region Estrazione dati

                // Se nel foglio excel sono presenti delle righe con codice identificativo non riconosciuto,
                // viene sollevata un'eccezione
                String notIdentified = GetNotIdentifiedProtoType(oleConnection);
                if (!String.IsNullOrEmpty(notIdentified))
                    throw new Exception("Sono state individuate le seguenti tipologie di documento non valide o non utilizzabili in questo contesto: " + notIdentified);

                // Creazione della query per la selezione dei dati sui documenti in arrivo da importare
                oleCommand = new OleDbCommand("SELECT * FROM [RDE$] WHERE [Tipo Protocollo] = 'A'", oleConnection);

                // Esecuzione della query per il recupero dei dati
                dataReader = oleCommand.ExecuteReader();

                // Lettura dei dati relativi ai protocolli in ingresso
                toReturn.InDocument = ReadDocumentData(dataReader, "A", versionNumber);

                // Creazione della query per la selezione dei dati sui documenti in partenza da importare
                oleCommand = new OleDbCommand("SELECT * FROM [RDE$] WHERE [Tipo Protocollo] = 'P'", oleConnection);

                // Esecuzione della query per il recupero dei dati
                dataReader = oleCommand.ExecuteReader();

                // Lettura dei dati relativi ai protocolli in ingresso
                toReturn.OutDocument = ReadDocumentData(dataReader, "P", versionNumber);

                // Creazione della query per la selezione dei dati sui documenti interni da importare
                oleCommand = new OleDbCommand("SELECT * FROM [RDE$] WHERE [Tipo Protocollo] = 'I'", oleConnection);

                // Esecuzione della query per il recupero dei dati
                dataReader = oleCommand.ExecuteReader();

                // Lettura dei dati relativi ai protocolli in ingresso
                toReturn.OwnDocument = ReadDocumentData(dataReader, "I", versionNumber);

                #endregion

            }
            catch (Exception e)
            {

                // Lancio dell'eccezione
                throw new Exception(String.Format(
                    "Errore durante la lettura dei dati dal foglio excel. Dettagli: {0}",
                    e.Message));

            }
            finally
            {
                #region Chiusura connessione

                if (oleConnection.State != System.Data.ConnectionState.Closed)
                    // Chiusura della connessione
                    oleConnection.Close();

                #endregion
            }
            // Restituzione oggetto con i dati estratti dal foglio Excel
            return toReturn;

        }

        /// <summary>
        /// Questa funzione restituisce l'insieme delle stringhe di identificazione tipo protocollo
        /// non identificate
        /// </summary>
        /// <param name="oleConnection">Connessione da sfruttare per interrogare il foglio Excel</param>
        /// <returns>Sigle non riconosciute separate da ,</returns>
        private static string GetNotIdentifiedProtoType(OleDbConnection oleConnection)
        {
            // Valore da restituire
            String toReturn = String.Empty;

            // Inizializzazione della query da eseguire
            //select * from [RDE$] where (Tipo Protocollo = 'A' OR Tipo Protocollo = 'P' OR Tipo Protocollo = 'I')
            OleDbCommand oleCommand = new OleDbCommand(
                "select [Tipo Protocollo] from [RDE$] where  [Tipo Protocollo] NOT IN ( 'A','P','I') ",
                oleConnection);
            OleDbDataReader reader = null;
            try
            {
                // Esecuzione della query
                reader = oleCommand.ExecuteReader();

                // Costruzione della risposta
                StringBuilder sb = new StringBuilder();
                while (reader.Read())
                {
                    sb.Append(reader[0]);
                    sb.Append(", ");

                }

                toReturn = sb.ToString();
                if (toReturn.Length > 0)
                    toReturn = toReturn.Substring(0, toReturn.Length - 2);

            }
            catch (Exception e) { logger.Error(e.Message.ToString()); }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
            return toReturn;

        }
   

        /// <summary>
        /// Funzione per la lettura dei dati relativi ad un documenti di emergenza da importare
        /// </summary>
        /// <param name="dataReader">L'oggetto da cui leggere i dati</param>
        /// <param name="protoType">Il tipo di protocollo di emergenza da importare (I, P, A)</param>
        /// <param name="versionNumber">Numero di versione dell'importer da utilizzare. 
        ///     - La versione 1 non contempla i protocolli interni ed i codici per mittenti e destinatari
        ///     - La versione 2 contempla i protocolli interni ed i codici per mittenti e destinatari</param>
        /// <returns>La lista dei dati relativi ai documenti da creare</returns>
        private static List<DocumentRowData> ReadDocumentData(OleDbDataReader dataReader, string protoType, int versionNumber)
        {
            // L'oggetto da restituire
            List<DocumentRowData> toReturn;

            // Oggetto in cui contenere temporaneamente i dati
            RDEDocumentRowData temporaryData;

            // La data di protocollazione
            DateTime protoDate;

            // Creazione della lista da restituire
            toReturn = new List<DocumentRowData>();
            
            // Estrazione dei dati relativi ai protocollo di emergenza da importare
            while (dataReader.Read())
            {
                // Creazione oggetto temporaneo
                temporaryData = new RDEDocumentRowData();

                // Creazione array codici modelli trasmissione
                temporaryData.TransmissionModelCode = new string[0];

                // Lettura codice amministrazione
                temporaryData.AdminCode = dataReader["Codice Amministrazione"].ToString();

                // Parsing della data di protocollazione
                if (!String.IsNullOrEmpty(dataReader["Data protocollo emergenza"].ToString())){
                    protoDate = DateTime.Parse(dataReader["Data protocollo emergenza"].ToString());
                    temporaryData.EmergencyProtocolDate = protoDate.ToString("dd/MM/yyyy");
                }

                // Parsing dell'ora di protocollazione
                if (!String.IsNullOrEmpty(dataReader["Ora protocollo emergenza"].ToString()))
                {
                    protoDate = ImportUtils.ReadDate(dataReader["Ora protocollo emergenza"].ToString());
                    temporaryData.EmergencyProtocolTime = protoDate.ToString("HH.mm.ss");
                }

                // Lettura del numero del protocollo di emergenza
                temporaryData.OrdinalNumber = dataReader["Numero protocollo emergenza"].ToString();

                // Lettura della segnatura del protocollo di emergenza
                temporaryData.EmergencyProtocolSignature = dataReader["Stringa protocollo emergenza"].ToString();

                // Lettura dell'oggetto del protocollo
                temporaryData.Obj = dataReader["Oggetto"].ToString();

                // Lettura della data del protocollo mittente
                temporaryData.SenderProtocolDate = dataReader["Data protocollo mittente"].ToString();

                // Lettura del numero protocollo mittente
                temporaryData.SenderProtocolNumber = dataReader["Numero protocollo mittente"].ToString();

                // Lettura della data di arrivo se è valorizzato almeno il campo Data arrivo
                if (!String.IsNullOrEmpty(dataReader["Data arrivo"].ToString()))
                {
                    temporaryData.ArrivalDate = DateTime.Parse(dataReader["Data arrivo"].ToString()).ToString("dd/MM/yyyy");

                    // parserizzo l'orario prima di utilizzarlo per verificare la formattazione e sollevare un messaggio di cortesia
                    if (!String.IsNullOrEmpty(dataReader["Ora arrivo"].ToString()))
                    {

                        //DateTime time;
                        //DateTime.TryParse(dataReader["Ora arrivo"].ToString(), out time);

                        TimeSpan time;
                        TimeSpan.TryParse(dataReader["Ora arrivo"].ToString().Replace(".", ":"), out time);

                        if (time != null)
                            temporaryData.ArrivalTime = time.ToString();
                    }
                }

                // Lettura del codice di classifica
                if (!String.IsNullOrEmpty(dataReader["Codice classifica"].ToString()))
                    temporaryData.ProjectCodes = dataReader["Codice classifica"].ToString().Split(';');

                // Lettura del codice RF
                temporaryData.RFCode = dataReader["Codice RF"].ToString();

                // Lettura del codice del registro
                temporaryData.RegCode = dataReader["Codice Registro"].ToString();

                // A seconda del tipo di protocollo bisogna trattare in modo differente
                // i campi relativi ai corrispondenti
                switch (protoType.ToUpper())
                {
                    case "A":   // Arrivo
                        // In questo caso i campi da prendere in considerazione sono Mittente e
                        // Cod_Mittente
                        if (!String.IsNullOrEmpty(dataReader["Mittente"].ToString()))
                            temporaryData.CorrDesc = new List<string>(
                                dataReader["Mittente"].ToString().Trim().Split(';'));

                        if (versionNumber != 1 && 
                            !String.IsNullOrEmpty(dataReader["Cod_Mittente"].ToString()))
                            temporaryData.CorrCode = new List<string>(
                                dataReader["Cod_Mittente"].ToString().Trim().Split(';'));

                        break;

                    case "P":   // Partenza
                    case "I":   // Interno
                        // In questi casi bisogna considerare i campi Mittente, Cod_Mittente,
                        // Destinatari, Cod_Destinari, Destinatari_CC e Cod_Destinatari_CC
                        if (!String.IsNullOrEmpty(dataReader["Mittente"].ToString()) ||
                            !String.IsNullOrEmpty(dataReader["Destinatari"].ToString()) ||
                            !String.IsNullOrEmpty(dataReader["Destinatari_CC"].ToString()))
                            temporaryData.CorrDesc = new List<string>();

                        if (versionNumber != 1 && (
                            !String.IsNullOrEmpty(dataReader["Cod_Mittente"].ToString()) ||
                            !String.IsNullOrEmpty(dataReader["Cod_Destinatari"].ToString()) ||
                            !String.IsNullOrEmpty(dataReader["Cod_Destinatari_CC"].ToString())))
                            temporaryData.CorrCode = new List<string>();

                        if (!String.IsNullOrEmpty(dataReader["Mittente"].ToString()))
                            foreach (string mitt in dataReader["Mittente"].ToString().Trim().Split(';'))
                                temporaryData.CorrDesc.Add(mitt.Trim() + "#M#");

                        if (versionNumber != 1 &&
                            !String.IsNullOrEmpty(dataReader["Cod_Mittente"].ToString()))
                            foreach (string mitt in dataReader["Cod_Mittente"].ToString().Trim().Split(';'))
                                temporaryData.CorrCode.Add(mitt.Trim() + "#M#");

                        if (!String.IsNullOrEmpty(dataReader["Destinatari"].ToString()))
                            foreach (string mitt in dataReader["Destinatari"].ToString().Trim().Split(';'))
                                temporaryData.CorrDesc.Add(mitt.Trim() + "#D#");

                        if (versionNumber != 1 && 
                            !String.IsNullOrEmpty(dataReader["Cod_Destinatari"].ToString()))
                            foreach (string mitt in dataReader["Cod_Destinatari"].ToString().Trim().Split(';'))
                                temporaryData.CorrCode.Add(mitt.Trim() + "#D#");

                        if (!String.IsNullOrEmpty(dataReader["Destinatari_CC"].ToString()))
                            foreach (string mitt in dataReader["Destinatari_CC"].ToString().Trim().Split(';'))
                                temporaryData.CorrDesc.Add(mitt.Trim() + "#CC#");

                        if (versionNumber != 1 &&
                            !String.IsNullOrEmpty(dataReader["Cod_Destinatari_CC"].ToString()))
                            foreach (string mitt in dataReader["Cod_Destinatari_CC"].ToString().Trim().Split(';'))
                                temporaryData.CorrCode.Add(mitt.Trim() + "#CC#");

                        break;

                }

                // Aggiunta dell'oggetto alla lista degli oggetti da restituire
                toReturn.Add(temporaryData);

            }

            // Restituzione risultato dell'elaborazione
            return toReturn;

        }

        #endregion

        #region Import Documenti

        /// <summary>
        /// Funzione per l'importazione dei documenti
        /// </summary>
        /// <param name="completePath">Il path in cui è memorizzato il file temporaneo</param>
        /// <param name="userInfo">Le informazioni sull'utente cha ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="serverPath">L'indirizzo della WA</param>
        /// <param name="isRapidClassificationRequired">True se è richiesta classificazione rapida obbligatoria</param>
        /// <param name="isSmistamentoEnabled">True se è abilitato lo smistamento</param>
        /// <returns></returns>
        private static ResultsContainer CreateDocuments(DocumentRowDataContainer container, InfoUtente userInfo, Ruolo role, string serverPath, bool isRapidClassificationRequired, bool isSmistamentoEnabled, string ftpAddress, String ftpUsername, String ftpPassword, bool isEnabledPregressi)
        {
            #region Dichiarazione variabili

            // La stringa di connessione al foglio excel
            string connectionString = String.Empty;

            // Il risultato dell'elaborazione
            ResultsContainer result = null;

            // La lista di funzioni associate al ruolo
            Funzione[] functions = null;

            // Un booleano utilizzato per tesare se con il ruolo segnalato può creare
            // creato un documento di un certo tipo
            bool canCreateDocuments = false;

            #endregion

            // Creazione della lista dei risultati
            result = new ResultsContainer();

            // Recupero delle funzioni associate al ruolo
            functions = (Funzione[])role.funzioni.ToArray(typeof(Funzione));

            #region Creazione documenti di emergenza in Arrivo

            // Verifica della possibilità di creare documenti in arrivo con il ruolo
            // role
            canCreateDocuments = functions.Where(e => e.codice == "PROTO_IN").FirstOrDefault() != null;

            // Se con il ruolo corrente è possibile creare documenti in arrivo,
            // si procede alla creazione
            if (canCreateDocuments)
            {
                // Se la lista dei documenti in Arrivo in è valorizzata, si procede con l'importazione
                if (container.InDocument != null)
                    result.InDocument.AddRange(new InDocument().ImportDocuments(
                        container.InDocument,
                        userInfo,
                        role,
                        serverPath,
                        false,
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

            #region Creazione documenti di emergenza in Partenza

            // Verifica della possibilità di creare documenti in uscita con il ruolo
            // role
            canCreateDocuments = functions.Where(e => e.codice == "PROTO_OUT").FirstOrDefault() != null;

            // Se con il ruolo corrente è possibile creare documenti in partenza,
            // si procede alla creazione
            if (canCreateDocuments)
            {
                if (container.OutDocument != null)
                    result.OutDocument.AddRange(new OutDocument().ImportDocuments(
                        container.OutDocument,
                        userInfo,
                        role,
                        serverPath,
                        false,
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

            #region Creazione documenti di emergenza Interni

            // Verifica della possibilità di creare documenti interni con il ruolo
            // role
            canCreateDocuments = functions.Where(e => e.codice == "PROTO_OWN").FirstOrDefault() != null;

            // Se con il ruolo corrente è possibile creare documenti interni,
            // si procede alla creazione
            if (canCreateDocuments)
            {
                if (container.OwnDocument != null)
                    result.OwnDocument.AddRange(new OwnDocument().ImportDocuments(
                        container.OwnDocument,
                        userInfo,
                        role,
                        serverPath,
                        false,
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

            // Restituzione del risultato dell'importazione
            return result;

        }

        #endregion

    }
}
