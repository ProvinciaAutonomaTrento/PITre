using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;
using BusinessLogic.Utenti;
using DocsPaVO.addressbook;
using DocsPaVO.Import.AddressBook;
using DocsPaVO.utente;
using DocsPaVO.rubrica;
using DocsPaDB;

namespace BusinessLogic.Import.ImportAddressBook
{
    /// <summary>
    /// Questa classe si occupa di effettuare l'importazione di corrispondenti
    /// definiti in un apposito foglio Excel nella rubrica
    /// </summary>
    public class ImportAddressBook
    {
        /// <summary>
        /// Le possibili operazioni che si possono compiere sul corrispondente
        /// </summary>
        public enum OperationEnum
        {
            I,     /// Inserimento
            M,     /// Modifica
            C      /// Cancellazione
        }

        #region Funzioni pubbliche

        /*
        /// <summary>
        /// Funzione per l'importazione dei corrispondenti definiti all'interno di un foglio excel
        /// </summary>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata avviata la procedura</param>
        /// <param name="data">Contenuto del foglio excel</param>
        /// <param name="listsFlag"></param>
        /// <param name="fileName">Nome da attribuire al file temporaneo</param>
        /// <param name="modelPath">Path in cui sono salvati i modelli</param>
        /// <param name="provider">Il provider Excel da utilizzare per la connessione al foglio Excel</param>
        /// <param name="extendedProperty">Proprietà avanzate da utilizzare per la connessione al foglio Excel</param>
        /// <returns>Report di inserimento</returns>
        public AddressBookImportResultContainer Import(
            InfoUtente userInfo,
            Ruolo role,
            byte[] data,
            int listsFlag,
            string fileName,
            string modelPath,
            string provider,
            string extendedProperty)
        {
            #region Dichiarazione variabili

            // Il path completo a cui è possibile reperire il file temporaneo
            string completePath;

            // La connessione al foglio Excel
            OleDbConnection oleConnection;

            // Il contenitore dei risultati
            AddressBookImportResultContainer toReturn = null;

            // Lista delle funzioni associate al ruolo
            Funzione[] functions;

            // Valori booleani utilizzati per indicare se sono abilitati inserimento/modifica/cancellazione
            // su tutti, rf e registro
            bool canInsAll, canInsRF, canInsReg, canModAll, canModRF, canModReg, canDelAll, canDelReg, canDelRF;

            #endregion

            // 0. Reperimento delle funzioni associate al ruolo
            functions = (Funzione[])(role.funzioni.ToArray(typeof(Funzione)));

            // 1. Creazione di un file temporaneo in cui depositare il contenuto del file Excel
            completePath = ImportUtils.CreateTemporaryFile(data, modelPath, fileName);

            // 2. Connessione al foglio Excel
            oleConnection = ImportUtils.ConnectToFile(provider, extendedProperty, completePath);

            // 3. Reperimento delle autorizzazioni degli utenti
            this.GetPermission(functions,
                out canInsAll,
                out canInsRF,
                out canInsReg,
                out canModAll,
                out canModRF,
                out canModReg,
                out canDelAll,
                out canDelReg,
                out canDelRF);

            // 4. Esecuzione dell'importazione
            try
            {
                toReturn = this.ExecuteImport(
                    oleConnection,
                    userInfo,
                    role,
                    listsFlag,
                    canInsAll,
                    canInsReg,
                    canInsRF,
                    canModAll,
                    canModReg,
                    canModRF,
                    canDelAll,
                    canDelReg,
                    canDelRF);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                try
                {
                    // 5. Chiusura della connessione
                    ImportUtils.CloseConnection(oleConnection);
                }
                catch (Exception e)
                {
                    toReturn.General.Add(new AddressBookImportResult()
                    {
                        Message = e.Message
                    });
                }

                try
                {
                    // 6. Cancellazione del file temporaneo
                    ImportUtils.DeleteTemporaryFile(completePath);
                }
                catch (Exception e)
                {
                    toReturn.General.Add(new AddressBookImportResult()
                    {
                        Message = e.Message
                    });
                }

            }

            // 7. Restituzione del risultato
            return toReturn;
        }
        */

        /// <summary>
        /// Funzione per l'estrazione dei dati sui corrispondenti da foglio Excel
        /// </summary>
        /// <param name="modelPath">Il path in cui andare a salvare il file</param>
        /// <param name="provider">Il provider da utilizzare per la connessione</param>
        /// <param name="extendedProperty">Le proprietà da utilizzare per la connessione al foglio Excel</param>
        /// <param name="fileName">Il nome del file temporaneo</param>
        /// <param name="data">Il contenuto binario del file Excel</param>
        /// <returns>L'insieme dei dati sui corrispondenti</returns>
        public AddressBookRowDataContainer ReadDataFromExcelFile(
            string modelPath,
            string provider,
            string extendedProperty,
            string fileName,
            byte[] data)
        {
            #region Dichiarazione Variabili

            // Il path in cui sarà memorizzato il file temporaneo
            string completePath;

            // La connessione da utilizzare per la connessione al file Excel
            OleDbConnection oleConnection;

            // L'oggetto da restituire
            AddressBookRowDataContainer toReturn = null; ;

            #endregion

            // 1. Creazione di un file temporaneo in cui depositare il contenuto del file Excel
            completePath = ImportUtils.CreateTemporaryFile(data, modelPath, fileName);

            // 2. Connessione al foglio Excel
            oleConnection = ImportUtils.ConnectToFile(provider, extendedProperty, completePath);

            try
            {
                // 3. Esecuzione della lettura dei dati dal foglio Excel
                // Creazione dell'oggetto da restituire
                toReturn = new AddressBookRowDataContainer();

                // 3.1 Estrazione dati sui corrispondenti da inserire
                toReturn.ToInsert = this.ExtractData(
                    oleConnection,
                    OperationEnum.I);

                // 3.2 Estrazione dati sui corrispondenti da modificare
                toReturn.ToModify = this.ExtractData(
                    oleConnection,
                    OperationEnum.M);

                // 3.3 Estrazione dati sui corrispondenti da cancellare
                toReturn.ToDelete = this.ExtractData(
                    oleConnection,
                    OperationEnum.C);

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                // 4. Chiusura della connessione
                oleConnection.Close();

                // 5. Cancellazione del file temporaneo
                ImportUtils.DeleteTemporaryFile(completePath);
            }

            // 6. Restituzione del risultato
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'esecuzione dell'importazione di un corrispondente
        /// i cui dati decrittivi sono riportati all'interno di un particolare oggetto
        /// </summary>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="listsFlag"></param>
        /// <param name="rowData">L'oggetto con i dati sul corrispondente</param>
        /// <param name="operation">L'operazione da compiere sul corrispondente</param>
        /// <returns>Il risultato dell'importazione</returns>
        public AddressBookImportResult Import(
            InfoUtente userInfo,
            Ruolo role,
            int listsFlag,
            OperationEnum operation,
            AddressBookRowData rowData)
        {
            #region Dichiarazione variabili

            // Lista delle funzioni
            Funzione[] functions;

            // Valori booleani utilizzati per indicare se sono abilitati inserimento/modifica/cancellazione
            // su tutti, rf e registro
            bool canInsAll, canInsRF, canInsReg, canModAll, canModRF, canModReg, canDelAll, canDelReg, canDelRF;

            // L'oggetto da restituire
            AddressBookImportResult toReturn;

            #endregion

            // 0. Reperimento delle funzioni associate al ruolo
            functions = (Funzione[])(role.funzioni.ToArray(typeof(Funzione)));

            // 1. Reperimento delle autorizzazioni
            this.GetPermission(functions,
                out canInsAll, out canInsRF, out canInsReg,
                out canModAll, out canModRF, out canModReg,
                out canDelAll, out canDelReg, out canDelRF);

            // 2. Esecuzione operazione
            toReturn = this.ExecuteImport(
                userInfo,
                role,
                listsFlag,
                operation,
                canInsAll,
                canInsReg,
                canInsRF,
                canModAll,
                canModReg,
                canModRF,
                canDelAll,
                canDelReg,
                canDelRF,
                rowData);

            // 3. Restituzione risultato
            return toReturn;

        }

        #endregion

        #region Funzioni di importazione

        /*
        /// <summary>
        /// Funzione per l'esecuzione dell'importazione 
        /// </summary>
        /// <param name="oleConnection">L'oggetto utilizzato per comunicare con il foglio Excel</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="listsFlag"></param>
        /// <param name="canDelInAll">Possibilità da parte del ruolo cui appartiene l'utente di cancellare su Tutti</param>
        /// <param name="canDelInReg">Possibilità da parte del ruolo cui appartiene l'utente di cancellare su Registro</param>
        /// <param name="canDelInRF">Possibilità da parte del ruolo cui appartiene l'utente di cancellare su RF</param>
        /// <param name="canInsInAll">Possibilità da parte del ruolo cui appartiene l'utente di inserire su Tutti</param>
        /// <param name="canInsInReg">Possibilità da parte del ruolo cui appartiene l'utente di inserire su Registro</param>
        /// <param name="canInsInRF">Possibilità da parte del ruolo cui appartiene l'utente di inserire su RF</param>
        /// <param name="canModInAll">Possibilità da parte del ruolo cui appartiene l'utente di modificare in Tutti</param>
        /// <param name="canModInReg">Possibilità da parte del ruolo cui appartiene l'utente di modificare in Registri</param>
        /// <param name="canModInRF">Possibilità da parte del ruolo cui appartiene l'utente di modificare in RFR</param>
        /// <returns>Un report dell'esecuzione</returns>
        private AddressBookImportResultContainer ExecuteImport(
            OleDbConnection oleConnection,
            InfoUtente userInfo,
            Ruolo role,
            int listsFlag,
            bool canInsInAll,
            bool canInsInReg,
            bool canInsInRF,
            bool canModInAll,
            bool canModInReg,
            bool canModInRF,
            bool canDelInAll,
            bool canDelInReg,
            bool canDelInRF)
        {
            // L'oggetto da restituire
            AddressBookImportResultContainer toReturn = new AddressBookImportResultContainer();

            // 1. Inserimento dei corrispondenti e salvataggio del report
            toReturn.Inserted = this.ExecuteImport(
                oleConnection,
                userInfo,
                role,
                listsFlag,
                OperationEnum.I,
                canInsInAll,
                canInsInReg,
                canInsInRF,
                canModInAll,
                canModInReg,
                canModInRF,
                canDelInAll,
                canDelInReg,
                canDelInRF);

            // 2. Modifica dei corrispondenti e salvataggio del report
            toReturn.Modified = this.ExecuteImport(
                oleConnection,
                userInfo,
                role,
                listsFlag,
                OperationEnum.M,
                canInsInAll,
                canInsInReg,
                canInsInRF,
                canModInAll,
                canModInReg,
                canModInRF,
                canDelInAll,
                canDelInReg,
                canDelInRF);

            // 3. Cancellazione dei corrispondenti e salvataggio del report
            toReturn.Deleted = this.ExecuteImport(
                oleConnection,
                userInfo,
                role,
                listsFlag,
                OperationEnum.C,
                canInsInAll,
                canInsInReg,
                canInsInRF,
                canModInAll,
                canModInReg,
                canModInRF,
                canDelInAll,
                canDelInReg,
                canDelInRF);

            // Restituzione del report completo
            return toReturn;

        }*/

        /// <summary>
        /// Funzione per l'esecuzione dell'importazione per una determinata operazione.
        /// </summary>
        /// <param name="userInfo">Le informazioni sull'utente cha ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="listsFlag"></param>
        /// <param name="operation">L'operazione da compiere</param>
        /// <param name="canDelInAll">Possibilità da parte del ruolo cui appartiene l'utente di cancellare su Tutti</param>
        /// <param name="canDelInReg">Possibilità da parte del ruolo cui appartiene l'utente di cancellare su Registro</param>
        /// <param name="canDelInRF">Possibilità da parte del ruolo cui appartiene l'utente di cancellare su RF</param>
        /// <param name="canInsInAll">Possibilità da parte del ruolo cui appartiene l'utente di inserire su Tutti</param>
        /// <param name="canInsInReg">Possibilità da parte del ruolo cui appartiene l'utente di inserire su Registro</param>
        /// <param name="canInsInRF">Possibilità da parte del ruolo cui appartiene l'utente di inserire su RF</param>
        /// <param name="canModInAll">Possibilità da parte del ruolo cui appartiene l'utente di modificare in Tutti</param>
        /// <param name="canModInReg">Possibilità da parte del ruolo cui appartiene l'utente di modificare in Registri</param>
        /// <param name="canModInRF">Possibilità da parte del ruolo cui appartiene l'utente di modificare in RFR</param>
        /// <param name="rowData">L'oggetto con le informazioni sul corrispondente su cui bisogna compiere l'operazione</param>
        /// <returns>Il risultato di importazione</returns>
        private AddressBookImportResult ExecuteImport(
            InfoUtente userInfo,
            Ruolo role,
            int listsFlag,
            OperationEnum operation,
            bool canInsInAll,
            bool canInsInReg,
            bool canInsInRF,
            bool canModInAll,
            bool canModInReg,
            bool canModInRF,
            bool canDelInAll,
            bool canDelInReg,
            bool canDelInRF,
            AddressBookRowData rowData)
        {
            #region Dichiarazione Variabili

            // L'oggetto da restituire
            AddressBookImportResult toReturn;

            #endregion

            // Inizializzazione del risultato da restituire
            toReturn = new AddressBookImportResult();

            try
            {
                // A seconda del tipo di operazione richiesta, viene richiamato il
                // metodo appropriato
                switch (operation)
                {
                    case OperationEnum.I:   // Inserimento
                        toReturn = this.Insert(
                            rowData,
                            userInfo,
                            role,
                            canInsInAll,
                            canInsInReg,
                            canInsInRF);
                        break;

                    case OperationEnum.M:   // Modifica
                        // Se è stato specificato un FromRegistry, viene richiamata la
                        // funzione di spostamento del corrispondente altrimenti viene
                        // richiamata quella di modifica
                        if (String.IsNullOrEmpty(rowData.FromRegistry))
                            toReturn = this.Modify(
                                rowData,
                                userInfo,
                                role,
                                canModInAll,
                                canModInRF,
                                canModInReg);
                        else
                            toReturn = this.MoveEntityFromRegistryAToRegistryB(
                                rowData,
                                userInfo,
                                role,
                                listsFlag,
                                canDelInReg,
                                canDelInRF,
                                canDelInAll,
                                canInsInReg,
                                canInsInRF,
                                canInsInAll);

                        break;

                    case OperationEnum.C:   // Cancellazione
                        toReturn = this.Delete(
                            rowData,
                            userInfo,
                            role,
                            listsFlag,
                            canDelInAll,
                            canDelInReg,
                            canDelInRF);
                        break;
                }
            }
            catch (Exception e)
            {
                // In caso di eccezione viene restituito un problema
                toReturn.Result = AddressBookImportResult.ResultEnum.KO;
                toReturn.AddProblem(String.Format(
                    "Si è verificato un problema irreparabile in fase di esecuzione dell'operazione di tipo {0}. Dettagli: {0}",
                    operation.ToString(), e.Message));
            }

            // Restituzione del risultato
            return toReturn;
        }

        #region Inserimento nuovo corrispondente

        /// <summary>
        /// Funzione per l'inserimento di nuovi corrispondenti
        /// </summary>
        /// <param name="rowData">L'oggetto con le informazioni sul corrispondente</param>
        /// <param name="userInfo">Informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="canInsInAll">Possibilità da parte del ruolo cui appartiene l'utente di inserire in Tutti</param>
        /// <param name="canInsInReg">Possibilità da parte del ruolo cui appartiene l'utente di inserire in Registri</param>
        /// <param name="canInsInRF">Possibilità da parte del ruolo cui appartiene l'utente di inserire in RF</param>
        /// <returns>Il risultato di importazione</returns>
        private AddressBookImportResult Insert(
            AddressBookRowData rowData,
            InfoUtente userInfo,
            Ruolo role,
            bool canInsInAll,
            bool canInsInReg,
            bool canInsInRF)
        {
            #region Dichiarazione Variabili

            // L'oggetto da restituire
            AddressBookImportResult toReturn;

            // Risultato della validazione dati
            bool validationResult;

            // Lista in cui memorizzare temporaneamente i problemi
            List<String> tmpProblems;

            // Il corrispondente Persona o Ruolo da inserire
            Corrispondente corrToInsert;

            // Il corrispondente UO da inserire
            UnitaOrganizzativa uoToInsert;

            #endregion

            // Inizializzazione variabili
            validationResult = true;
            toReturn = new AddressBookImportResult();
            tmpProblems = new List<string>();
            corrToInsert = null;

            // 1. Validazione dati estratti
            validationResult = this.ValidateData(
                rowData,
                out tmpProblems,
                OperationEnum.I,
                userInfo,
                role,
                canInsInAll,
                canInsInRF,
                canInsInReg);

            toReturn.AddProblem(tmpProblems);

            // 2. Inserimento
            if (validationResult)
            {
                // A seconda del tipo di corrispondente bisogna procedere in modo diverso
                switch (rowData.CorrType)
                {
                    case AddressBookRowData.CorrEnum.U:                    // Unità Organizzativa

                        try
                        {
                            // Creazione dei dati sull'unità organizzativa
                            uoToInsert = this.CreateUoData(rowData, role, userInfo);

                            // Inserimento del corrispondente
                            corrToInsert = addressBookManager.insertCorrispondente(uoToInsert, null);

                        }
                        catch (Exception e)
                        {
                            // Il risultato è negativo
                            toReturn.Result = AddressBookImportResult.ResultEnum.KO;

                            // Aggiunta dell'eccezione
                            toReturn.Message = String.Format("Errore durante l'inserimento della UO con codice {0}, nel registro {1}",
                                rowData.AddressBookCode,
                                rowData.RegistryCode);

                            toReturn.AddProblem(e.Message);
                        }

                        break;

                    case AddressBookRowData.CorrEnum.R:                  // Ruolo

                        try
                        {
                            // Creazione del corrispondente
                            corrToInsert = this.CreateRoleData(rowData, userInfo, role);

                            // Creazione dell'uo parent
                            uoToInsert = new UnitaOrganizzativa();

                            // Impostazione della descrizione e del system id
                            uoToInsert.descrizione = String.Empty;
                            uoToInsert.systemId = "0";

                            // Inserimento del corrispondente
                            corrToInsert = addressBookManager.insertCorrispondente(corrToInsert, uoToInsert);
                        }
                        catch (Exception e)
                        {
                            // Il risultato è negativo
                            toReturn.Result = AddressBookImportResult.ResultEnum.KO;

                            // Aggiunta dell'eccezione
                            toReturn.Message = String.Format("Errore durante l'inserimento del ruolo con codice {0}, nel registro {1}.",
                                rowData.AddressBookCode,
                                rowData.RegistryCode);
                            toReturn.AddProblem(e.Message);
                        }
                        break;

                    case AddressBookRowData.CorrEnum.P:                // Persona

                        try
                        {
                            // Creazione del corrispondente
                            corrToInsert = this.CreatePersonData(rowData, userInfo, role);

                            // Inserimento del corrispondente
                            corrToInsert = addressBookManager.insertCorrispondente(corrToInsert, null);
                        }
                        catch (Exception e)
                        {
                            // Il risultato è negativo
                            toReturn.Result = AddressBookImportResult.ResultEnum.KO;

                            // Aggiunta dell'eccezione
                            toReturn.Message = String.Format("Errore durante l'inserimento della persona con codice {0}, nel registro {1}.",
                                rowData.AddressBookCode,
                                rowData.RegistryCode);
                            toReturn.AddProblem(e.Message);
                        }
                        break;

                }

                // Se il corrispondente restituito dalla funzione di inserimento non ha restituito errori,
                // il risultato è positivo altrimenti è negativo
                if (!String.IsNullOrEmpty(corrToInsert.errore))
                    toReturn = new AddressBookImportResult()
                    {
                        Result = AddressBookImportResult.ResultEnum.KO,
                        Message = String.Format(
                            "Si sono verificati dei problemi durante l'inserimento del corrispondente. Codice corrispondente: {0}, Registro: {2}, Dettagli: {1}",
                            rowData.AddressBookCode,
                            corrToInsert.errore,
                            rowData.RegistryCode)
                    };
                else
                    toReturn = new AddressBookImportResult()
                    {
                        Result = AddressBookImportResult.ResultEnum.OK,
                        Message = String.Format(
                            "Corrispondente inserito correttamente. Codice corrispondente: {0}, Codice registro: {1}",
                            corrToInsert.codiceRubrica,
                            rowData.RegistryCode)
                    };

            }
            else
            {
                // Altrimenti l'esito dell'importazione è un negativo
                toReturn.Result = AddressBookImportResult.ResultEnum.KO;

                toReturn.Message = String.Format(
                    "Alcuni parametri obbligatori non sono stati specificati o non sono validi. Codice corrispondente: {0}, Codice registro: {1}",
                    rowData.AddressBookCode,
                    rowData.RegistryCode);
            }

            // Restituzione del report
            return toReturn;

        }

        /// <summary>
        /// Funzione per la creazione dei dati sul corrispondente di tipo Persona
        /// </summary>
        /// <param name="rowData">L'oggetto in cui sono memorizzati i dati sul corrispondente da importare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <returns>L'oggetto con i dati sul corrispondente da inserire</returns>
        private Corrispondente CreatePersonData(AddressBookRowData rowData, InfoUtente userInfo, Ruolo role)
        {
            // L'oggetto da restituire
            Utente toReturn = new Utente();

            // Il dettaglio del corrispondente
            DettagliCorrispondente corrDetails;

            // L'id del registro
            string registryId = null;

            // Il canale prefereziale
            Canale channel;

            // Se il registro specificato non è TUTTI, ne viene ricavato l'id
            if (rowData.RegistryCode != "TUTTI")
                registryId = ImportUtils.GetRegistryFromCode(userInfo.idAmministrazione, rowData.RegistryCode).systemId;

            // Creazione del canale preferenziale
            channel = new Canale();

            // Se è specificato un canale, ne vengono prelevate le informazioni
            // altrimenti si imposta il canale di default
            if (!String.IsNullOrEmpty(rowData.Channel))
                channel = ImportUtils.LoadChannelInformation(rowData.Channel);
            else
                channel.systemId = new DocsPaDB.Query_DocsPAWS.Utenti().GetSystemIDCanale();

            // Compilazione dei dati su corrispondente
            toReturn.codiceCorrispondente = rowData.AddressBookCode;
            toReturn.codiceRubrica = rowData.AddressBookCode;
            toReturn.cognome = rowData.Surname;
            toReturn.nome = rowData.Name;
            toReturn.email = rowData.Email;
            toReturn.codiceAmm = rowData.AdministrationCode;
            toReturn.codiceAOO = rowData.AOOCode;
            toReturn.descrizione = rowData.Surname + " " + rowData.Name;
            toReturn.idAmministrazione = userInfo.idAmministrazione;
            toReturn.idRegistro = registryId;
            toReturn.canalePref = channel;
            toReturn.tipoCorrispondente = rowData.CorrType.ToString();

            // Creazione dei dettagli
            corrDetails = new DettagliCorrispondente();

            // Compilazione dei dettagli
            corrDetails.Corrispondente.AddCorrispondenteRow(
                rowData.Address,
                rowData.City,
                rowData.PostalCode,
                rowData.District,
                rowData.Nation,
                rowData.PhoneNumber1,
                rowData.PhoneNumber2,
                rowData.FaxNumber,
                rowData.FiscalCode,
                String.Empty,
                rowData.Localita,
                String.Empty,
                String.Empty,
                String.Empty,
                rowData.VatNumber);

            // Associazione dei dettagli
            toReturn.info = corrDetails;
            toReturn.dettagli = true;

            // Restituzione del risultato
            return toReturn;
        }

        /// <summary>
        /// Funzione per la creazione dei dati sul corrispondente di tipo Ruolo
        /// </summary>
        /// <param name="rowData">L'oggetto in cui sono memorizzati i dati sul corrispondente da importare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <returns>L'oggetto con i dati sul corrispondente da inserire</returns>
        private Corrispondente CreateRoleData(AddressBookRowData rowData, InfoUtente userInfo, Ruolo role)
        {
            // L'oggetto da restituire
            Ruolo toReturn = new Ruolo();

            // Il canale preferenziale
            Canale channel;

            // L'id registro
            string registryId = null;

            // Calcolo dell'id del registro se diverso da TUTTI
            if (rowData.RegistryCode != "TUTTI")
                registryId = ImportUtils.GetRegistryFromCode(userInfo.idAmministrazione, rowData.RegistryCode).systemId;

            // Creazione del canale preferenziale
            channel = new Canale();

            // Se specificato, vengono caricate le informazioni sul canale appropriato
            if (!String.IsNullOrEmpty(rowData.Channel))
                channel = ImportUtils.LoadChannelInformation(rowData.Channel);
            else
                channel.systemId = new DocsPaDB.Query_DocsPAWS.Utenti().GetSystemIDCanale();

            // Compilazione delle informazioni sul ruolo da inserire
            toReturn.tipoCorrispondente = rowData.CorrType.ToString();
            toReturn.codiceCorrispondente = rowData.AddressBookCode;
            toReturn.codiceRubrica = rowData.AddressBookCode;
            toReturn.descrizione = rowData.Description;
            toReturn.idRegistro = registryId;

            toReturn.email = rowData.Email;
            toReturn.codiceAmm = rowData.AdministrationCode;
            toReturn.codiceAOO = rowData.AOOCode;
            toReturn.idAmministrazione = userInfo.idAmministrazione;

            toReturn.canalePref = channel;

            // Restituzione dell'oggetto
            return toReturn;

        }

        /// <summary>
        /// Funzione per la creazione dei dati sul corrispondentedi tipo UO
        /// </summary>
        /// <param name="rowData">L'oggetto in cui sono memorizzati i dati sul corrispondente da importare</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <returns>L'oggetto con i dati sul corrispondente da inserire</returns>
        private UnitaOrganizzativa CreateUoData(AddressBookRowData rowData, Ruolo role, InfoUtente userInfo)
        {
            // L'oggetto da restituire
            UnitaOrganizzativa toReturn = new UnitaOrganizzativa();

            // I dettagli del corrispondenti
            DettagliCorrispondente details;

            // Il canale preferenziale
            Canale channel;

            // L'id del registro
            string registryId = null;

            // Se il registro specificato non è TUTTI, ne viene ricavato l'id
            if (rowData.RegistryCode != "TUTTI")
                registryId = ImportUtils.GetRegistryFromCode(userInfo.idAmministrazione, rowData.RegistryCode).systemId;

            // Creazione del canale preferenziale
            channel = new Canale();

            // Se specificato, vengono caricate le informazioni sul canale appropriato
            if (!String.IsNullOrEmpty(rowData.Channel))
                channel = ImportUtils.LoadChannelInformation(rowData.Channel);
            else
                channel.systemId = new DocsPaDB.Query_DocsPAWS.Utenti().GetSystemIDCanale();

            // Compilazione dei campi 
            toReturn.tipoCorrispondente = rowData.CorrType.ToString();
            toReturn.codiceCorrispondente = rowData.AddressBookCode;
            toReturn.codiceRubrica = rowData.AddressBookCode;
            toReturn.codiceAmm = rowData.AdministrationCode;
            toReturn.codiceAOO = rowData.AOOCode;
            toReturn.email = rowData.Email;
            toReturn.descrizione = rowData.Description;
            toReturn.idAmministrazione = userInfo.idAmministrazione;
            toReturn.idRegistro = registryId;

            // Creazione e compilazione dei dettagli sul corrispondente
            details = new DocsPaVO.addressbook.DettagliCorrispondente();

            details.Corrispondente.AddCorrispondenteRow(
                rowData.Address,
                rowData.City,
                rowData.PostalCode,
                rowData.District,
                rowData.Nation,
                rowData.PhoneNumber1,
                rowData.PhoneNumber2,
                rowData.FaxNumber,
                rowData.FiscalCode,
                String.Empty,
                rowData.Localita,
                String.Empty,
                String.Empty,
                String.Empty,
                rowData.VatNumber);

            toReturn.info = details;
            toReturn.dettagli = true;
            toReturn.canalePref = channel;

            // Restituzione delle informazioni sul corrispondente
            return toReturn;

        }

        #endregion

        #region Modifica corrispondente

        /// <summary>
        /// Funzione per il salvataggio delle modifiche su corrispondenti
        /// </summary>
        /// <param name="rowData">L'oggetto con le informazioni sul corrispondente da modificare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="canModInAll">Possibilità da parte del ruolo cui appartiene l'utente di compiere modifiche su Tutti</param>
        /// <param name="canModInReg">Possibilità da parte del ruolo cui appartiene l'utente di compiere modifiche su Registri</param>
        /// <param name="canModInRF">Possibilità da parte del ruolo cui appartiene l'utente di compiere modifiche su RF</param>
        /// <returns>Il report relativo alla modifica</returns>
        private AddressBookImportResult Modify(
            AddressBookRowData rowData,
            InfoUtente userInfo,
            Ruolo role,
            bool canModInAll,
            bool canModInRF,
            bool canModInReg)
        {
            #region Dichiarazione variabili

            // Il report da restituire
            AddressBookImportResult toReturn;

            // Risultato del processo di validazione
            bool validationResult;

            // Lista temporanea dei problemi
            List<String> tmpProblems;

            // Oggetto con i dati da modificare
            DatiModificaCorr corrToModify;

            // Il risultato della modifica
            bool modResult;

            // L'eventuale messaggio di errore restituito dalla procedura di modifica
            string message;

            #endregion

            // Inizializzazione variabili
            toReturn = new AddressBookImportResult();

            // Inizializzazione variabili
            validationResult = true;
            tmpProblems = new List<string>();

            // 1. Validazione dati estratti
            validationResult = this.ValidateData(
                rowData,
                out tmpProblems,
                OperationEnum.M,
                userInfo,
                role,
                canModInAll,
                canModInRF,
                canModInReg);

            toReturn.AddProblem(tmpProblems);

            // 2. Modifica
            if (validationResult)
            {
                try
                {
                    // Creazione dei dati sull'unità organizzativa
                    corrToModify = this.CreateModifyData(rowData, role, userInfo);

                    // Modifica del corrispondente
                    modResult = UserManager.ModifyCorrispondenteEsterno(corrToModify, userInfo, out message);

                    // Se modResult è false, il risultato sarà negativo ed in questo caso, se la stringa
                    // message è valorizzata viene aggiunta alla lista dei problemi
                    if (!modResult)
                    {
                        toReturn.Result = AddressBookImportResult.ResultEnum.KO;

                        if (!String.IsNullOrEmpty(message))
                        {
                            toReturn.AddProblem(message);
                            toReturn.Message = String.Format(
                                "Si è verificato un errore durante la modifica del corrispondente con codice {0} sul registro {1}.",
                                rowData.AddressBookCode,
                                String.IsNullOrEmpty(rowData.FromRegistry) ? rowData.RegistryCode : rowData.FromRegistry);
                        }
                    }
                    else
                    {
                        toReturn.Result = AddressBookImportResult.ResultEnum.OK;

                        toReturn.Message = String.Format(
                            "Corrispondente modificato con successo. Codice corrispondente: {0}, Registro: {1}",
                            rowData.AddressBookCode,
                            rowData.RegistryCode);
                    }

                }
                catch (Exception e)
                {
                    // Il risultato è negativo
                    toReturn.Result = AddressBookImportResult.ResultEnum.KO;

                    // Aggiunta dell'eccezione
                    toReturn.Message = String.Format(
                        "Errore durante la modifica del corrispondente con codice {0} sul registro {1}.",
                        rowData.AddressBookCode,
                        String.IsNullOrEmpty(rowData.FromRegistry) ? rowData.RegistryCode : rowData.FromRegistry);
                    toReturn.AddProblem(e.Message);
                }

            }
            else
            {
                // Altrimenti l'esito dell'importazione è un negativo
                toReturn.Result = AddressBookImportResult.ResultEnum.KO;

                toReturn.Message = String.Format(
                    "Alcuni parametri obbligatori non sono stati specificati o non sono validi. Codice corrispondente: {0}, Registro {1}.",
                    rowData.AddressBookCode,
                    String.IsNullOrEmpty(rowData.FromRegistry) ? rowData.RegistryCode : rowData.FromRegistry);
            }

            // Restituzione del report
            return toReturn;

        }

        /// <summary>
        /// Funzione per la creazione dell'oggetto per la modifica dei dati di un corrispondente
        /// </summary>
        /// <param name="rowData">L'oggetto con i dati sul corrispondente da modificare</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <returns>L'oggetto da salvare</returns>
        private DatiModificaCorr CreateModifyData(AddressBookRowData rowData, Ruolo role, InfoUtente userInfo)
        {
            // L'oggetto da restituire
            DatiModificaCorr toReturn = new DatiModificaCorr();

            // Il canale preferenziale
            Canale channel;

            // L'id del registro e del corrispondente da modificare
            string registryId = String.Empty, corrId;

            if (rowData.RegistryCode != "TUTTI")
                registryId = ImportUtils.GetRegistryFromCode(userInfo.idAmministrazione, rowData.RegistryCode).systemId;

            corrId = ImportUtils.GetCorrispondenteByCode(
                ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST,
                rowData.AddressBookCode,
                role,
                userInfo,
                registryId,
                String.Empty,
                true,
                TipoUtente.ESTERNO).systemId;

            if (String.IsNullOrEmpty(corrId))
                throw new Exception(String.Format(
                    "Corrispondente con codice {0} non trovato nel registro {1}.",
                    rowData.AddressBookCode,
                    String.IsNullOrEmpty(rowData.FromRegistry) ? rowData.RegistryCode : rowData.FromRegistry));

            // Creazione del canale preferenziale
            channel = new Canale();

            // Se specificato, vengono caricate le informazioni sul canale appropriato
            if (!String.IsNullOrEmpty(rowData.Channel))
                channel = ImportUtils.LoadChannelInformation(rowData.Channel);
            else
                channel.systemId = new DocsPaDB.Query_DocsPAWS.Utenti().GetSystemIDCanale();

            // Compilazione dei campi del corrispondente da modificare
            toReturn.idCorrGlobali = corrId;
            toReturn.codice = registryId;
            toReturn.codRubrica = rowData.AddressBookCode;
            toReturn.codiceAmm = rowData.AdministrationCode;
            toReturn.codiceAoo = rowData.AOOCode;
            toReturn.tipoCorrispondente = rowData.CorrType.ToString();
            toReturn.descCorr = rowData.Description;
            toReturn.cognome = rowData.Surname;
            toReturn.nome = rowData.Name;
            toReturn.indirizzo = rowData.Address;
            toReturn.cap = rowData.PostalCode;
            toReturn.citta = rowData.City;
            toReturn.provincia = rowData.District;
            toReturn.nazione = rowData.Nation;
            toReturn.codFiscale = rowData.FiscalCode;
            toReturn.telefono = rowData.PhoneNumber1;
            toReturn.telefono2 = rowData.PhoneNumber2;
            toReturn.fax = rowData.FaxNumber;
            toReturn.email = rowData.Email;
            toReturn.idCanalePref = channel.systemId;

            // Restituzione dell'oggetto creato
            return toReturn;
        }

        #endregion

        #region Cancellazione corrispondente

        /// <summary>
        /// Funzione per la cancellazione di corrispondenti
        /// </summary>
        /// <param name="rowData">L'oggetto con le informazioni sul corrispondente da cancellare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="listsFlag"></param>
        /// <param name="canDoDelInAll">Possibilità da parte del ruolo cui appartiene l'utente di compiere eliminazioni in Tutti</param>
        /// <param name="canDoDelInReg">Possibilità da parte del ruolo cui appartiene l'utente di compiere eliminazioni in Registri</param>
        /// <param name="canDoDelInRF">Possibilità da parte del ruolo cui appartiene l'utente di compiere eliminazioni in RF</param>
        /// <returns>Il report relativo alla cancellazione</returns>
        private AddressBookImportResult Delete(
            AddressBookRowData rowData,
            InfoUtente userInfo,
            Ruolo role,
            int listsFlag,
            bool canDoDelInAll,
            bool canDoDelInReg,
            bool canDoDelInRF)
        {
            #region Dichiarazione variabili

            // Id del corrispondente da cancellare
            string idCorrToDelete;

            // L'oggetto da restituire
            AddressBookImportResult toReturn;

            // Il risultato della validazione dei dati
            bool validationResult;

            // Lista temporanea dei problemi
            List<String> tmpProblems;

            // Risultato della cancellazione
            bool deleteResult;

            // Eventuale messaggio restituito dalla procedura di cancellazione
            string message;

            // L'id del registro / RF
            string registryOrRFId;

            #endregion

            // Inizializzazione variabili
            toReturn = new AddressBookImportResult();
            validationResult = true;
            tmpProblems = new List<string>();
            registryOrRFId = String.Empty;
            idCorrToDelete = String.Empty;
            deleteResult = false;

            // 1. Validazione dati estratti
            validationResult = this.ValidateData(
                rowData,
                out tmpProblems,
                OperationEnum.C,
                userInfo,
                role,
                canDoDelInAll,
                canDoDelInRF,
                canDoDelInReg);

            toReturn.AddProblem(tmpProblems);

            // 2. Cancellazione
            if (validationResult)
            {
                try
                {
                    // Reperimento dell'id del registro se specificato
                    if (rowData.RegistryCode != "TUTTI")
                        registryOrRFId = ImportUtils.GetRegistryFromCode(userInfo.idAmministrazione, rowData.RegistryCode).systemId;
                }
                catch (Exception e)
                {
                    toReturn.Result = AddressBookImportResult.ResultEnum.KO;
                    toReturn.Message = String.Format(
                        "Errore durante il reperimento delle informazioni sul registro {0}",
                        rowData.RegistryCode);
                }

                try
                {
                    if (toReturn.Result != AddressBookImportResult.ResultEnum.KO)
                        idCorrToDelete = ImportUtils.GetCorrispondenteByCode(
                            ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST,
                            rowData.AddressBookCode,
                            role,
                            userInfo,
                            registryOrRFId,
                            String.Empty,
                            true,
                            TipoUtente.ESTERNO).systemId;
                }
                catch (Exception e)
                {
                    toReturn.Result = AddressBookImportResult.ResultEnum.KO;
                    toReturn.Message = String.Format(
                        "Impossibile reperire le informazioni sul corrispondente da eliminare. Codice corrispondente: {0}, Codice registro: {1}",
                        rowData.AddressBookCode,
                        rowData.RegistryCode);
                }

                try
                {
                    if (toReturn.Result != AddressBookImportResult.ResultEnum.KO)
                    {
                        // Cancellazione del corrispondente
                        deleteResult = UserManager.DeleteCorrispondenteEsterno(idCorrToDelete, listsFlag, userInfo, out message);

                        // Se deleteResult è false, il risultato sarà negativo ed in questo caso, se la stringa
                        // message è valorizzata viene aggiunta alla lista dei problemi
                        if (!deleteResult)
                        {
                            toReturn.Result = AddressBookImportResult.ResultEnum.KO;

                            if (!String.IsNullOrEmpty(message))
                            {
                                toReturn.AddProblem(message);
                                toReturn.Message = String.Format(
                                    "Si è verificato un errore durante l'eliminazione del corrispondente. Codice corrispondente: {0}, Codice Registro: {1}",
                                    rowData.AddressBookCode,
                                    rowData.RegistryCode);
                            }
                        }
                        else
                        {
                            toReturn.Result = AddressBookImportResult.ResultEnum.OK;
                            toReturn.Message = String.Format(
                                "Corrispondente eliminato con successo. Codice corrispondente: {0}, Codice registro: {1}",
                                rowData.AddressBookCode,
                                rowData.RegistryCode);
                        }
                    }

                }
                catch (Exception e)
                {
                    // Il risultato è negativo
                    toReturn.Result = AddressBookImportResult.ResultEnum.KO;

                    // Aggiunta dell'eccezione
                    toReturn.Message = String.Format(
                        "Errore durante l'eliminazione del corrispondente con codice {0} dal registro {1}.",
                        rowData.AddressBookCode,
                        rowData.RegistryCode);
                    toReturn.AddProblem(e.Message);
                }

            }
            else
            {
                // Altrimenti l'esito dell'importazione è un negativo
                toReturn.Result = AddressBookImportResult.ResultEnum.KO;

                toReturn.Message = String.Format(
                    "Alcuni parametri obbligatori non sono stati specificati o non sono validi. Codice corrispondente {0}, Codice registro {1}",
                    rowData.AddressBookCode,
                    rowData.RegistryCode);
            }

            // Restituzione del report
            return toReturn;
        }

        #endregion

        #region Spostamento utente da un registro ad un altro

        /// <summary>
        /// Questa funzione di occupa di spostare un corrispondente da un registro ad un altro.
        /// Viene effettuata una rimozione del corrispondente sul vecchio regisistro ed un reinserimento
        /// dello stesso nel nuovo registro. Il tutto viene fatto all'interno di un contesto transazionale in modo 
        /// tale che se una delle due operazioni dovesse fallire, viene effettuato un rollback e segnalato l'evento
        /// all'utente attraverso un opportuno messaggio di errore
        /// </summary>
        /// <param name="rowData">L'oggetto con le informazioni sul corrispondente da spostare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="listFlag"></param>
        /// <param name="canDelFromRegistry">Possibilità da parte dell'utente di cancellare da un registro</param>
        /// <param name="canDelFromRF">Possbilità da parte dell'utente di cancellare da un RF</param>
        /// <param name="canDelFromAll">Possibilità da parte di dell'utente di cancellare da Tutti</param>
        /// <param name="canInsInRegistry">Possibilità da parte dell'utente di inserire in un registro</param>
        /// <param name="canInsInRF">Possibilità da parte di un utente di inserire in un RF</param>
        /// <param name="canInsInAll">Possibilità da parte di un utente di inserire in Tutti</param>
        /// <returns>Il risultato dell'operazione di spostamento</returns>
        private AddressBookImportResult MoveEntityFromRegistryAToRegistryB(
            AddressBookRowData rowData,
            InfoUtente userInfo,
            Ruolo role,
            int listFlag,
            bool canDelFromRegistry,
            bool canDelFromRF,
            bool canDelFromAll,
            bool canInsInRegistry,
            bool canInsInRF,
            bool canInsInAll)
        {
            // Il risultato dell'operazione di spostamento
            AddressBookImportResult toReturn = null;

            // Variabile di appoggio utilizzata per memorizzare temporaneamente uno
            // dei due registri
            string tmpRegistry;

            // Variabile per la memorizzazione temporanea della lista dei problemi
            List<String> tmpProblemsList;

            // Si scambiano di posto il FromRegistry ed il Registry in modo da poter sfruttare le 
            // funzioni di cancellazione ed inserimento già presenti
            tmpRegistry = rowData.RegistryCode;
            rowData.RegistryCode = rowData.FromRegistry;
            rowData.FromRegistry = tmpRegistry;

            // Inizializzazione del contesto transazionale
            using (TransactionContext transactionContext = new TransactionContext())
            {
                // Cancellazione del corrispondente
                try
                {
                    toReturn = this.Delete(
                        rowData,
                        userInfo,
                        role,
                        listFlag,
                        canDelFromAll,
                        canDelFromRegistry,
                        canDelFromRF);
                }
                catch (Exception e)
                {
                    toReturn = new AddressBookImportResult()
                    {
                        Result = AddressBookImportResult.ResultEnum.KO,
                        Message = String.Format(
                            "Si è verificato un errore durante l'eliminazione del corrispondente. L'operazione di spostamento del corrisponde è stata annullata. Codice corrispondente: {0}, Codice Registro: {1}",
                            rowData.AddressBookCode,
                            rowData.RegistryCode),
                        Problems = new List<string>(new string[] { e.Message })
                    };
                }

                // Se il risultato è positivo si procede con l'inserimento
                if (toReturn != null &&
                    toReturn.Result == AddressBookImportResult.ResultEnum.OK)
                {
                    // Si riscambiano i valori di FromRegistry e Registry
                    tmpRegistry = rowData.RegistryCode;
                    rowData.RegistryCode = rowData.FromRegistry;
                    rowData.FromRegistry = tmpRegistry;

                    try
                    {
                        // Inserimento
                        toReturn = this.Insert(
                            rowData,
                            userInfo,
                            role,
                            canInsInAll,
                            canInsInRegistry,
                            canInsInRF);

                    }
                    catch (Exception e)
                    {
                        toReturn = new AddressBookImportResult()
                        {
                            Result = AddressBookImportResult.ResultEnum.KO,
                            Message = String.Format(
                                "Si è verificato un errore durante l'inserimento del corrispondente. Lo spostamento del corrispondente è stato annullato. Codice corrispondente: {0}, Codice Registro: {1}",
                                rowData.AddressBookCode,
                                rowData.RegistryCode),
                            Problems = new List<string>(new string[] { e.Message })
                        };

                    }
                }

                // Se l'operazione è andata a buon fine, si effettua la chiusura del contesto
                if (toReturn != null)
                {
                    if (toReturn.Result == AddressBookImportResult.ResultEnum.OK)
                        transactionContext.Complete();
                }
                else
                    toReturn = new AddressBookImportResult()
                    {
                        Result = AddressBookImportResult.ResultEnum.KO
                    };

            }

            // Inizializzazione del risultato
            if (toReturn.Result == AddressBookImportResult.ResultEnum.OK)
                toReturn.Message = String.Format(
                    "Spostamento del corrispondente {0} dal registro {1} al registro {2} avvenuto con successo.",
                    rowData.AddressBookCode,
                    rowData.FromRegistry,
                    rowData.RegistryCode);
            else
            {
                // Se il messaggio del risultato parziale è valorizzato, viene aggiunto ai 
                // dettagli del risultato finale
                if (!String.IsNullOrEmpty(toReturn.Message))
                {
                    // Creazione id una lista speculare alla lista dei problemi
                    tmpProblemsList = new List<string>(toReturn.Problems);

                    // Reinizializzazione della lista dei problemi
                    toReturn.Problems = new List<string>();

                    // Aggiunta del messaggio alla lista dei problemi
                    toReturn.AddProblem(toReturn.Message);

                    // Aggiunta dei problemi alla lista dei problemi
                    toReturn.AddProblem(tmpProblemsList);
                }

                // Impostazione del messaggio del risultato finale
                toReturn.Message = String.Format(
                    "Spostamento del corrispondente {0} dal registro {1} al registro {2} non avvenuto.",
                    rowData.AddressBookCode,
                    rowData.FromRegistry,
                    rowData.RegistryCode);
            }

            // Restituzione del risultato
            return toReturn;

        }

        #endregion

        #endregion

        /// <summary>
        /// Funzione per il reperimento delle autorizzazioni dell'utente.
        /// </summary>
        /// <param name="functions">Lista delle funzioni associate al ruolo</param>
        /// <param name="canInsAll">Possibilità di inserire un corrispondente in tutti</param>
        /// <param name="canInsRF">Possiblità di inserire un corrispondente in un RF</param>
        /// <param name="canInsReg">Possibilità di inserire un corrispondente in un registro</param>
        /// <param name="canModAll">Possibilità di modicare un corrispondente in tutti</param>
        /// <param name="canModRF">Possibilità di modificare un corripsondente in un RF</param>
        /// <param name="canModReg">Possibilità di modificare un corrispondente in un Registro</param>
        /// <param name="canDelAll">Possibilità di eliminare un corrispondente in Tutti</param>
        /// <param name="canDelReg">Possibilità di eliminare un corrispondente in Registro</param>
        /// <param name="canDelRF">Possibilità di eliminare un corrispondente in RF</param>
        private void GetPermission(
            Funzione[] functions,
            out bool canInsAll,
            out bool canInsRF,
            out bool canInsReg,
            out bool canModAll,
            out bool canModRF,
            out bool canModReg,
            out bool canDelAll,
            out bool canDelReg,
            out bool canDelRF)
        {
            canInsAll = functions.Where(e => e.codice == "DO_INS_CORR_TUTTI").FirstOrDefault() != null;
            canInsRF = functions.Where(e => e.codice == "DO_INS_CORR_RF").FirstOrDefault() != null;
            canInsReg = functions.Where(e => e.codice == "DO_INS_CORR_REG").FirstOrDefault() != null;
            canModAll = functions.Where(e => e.codice == "DO_MOD_CORR_TUTTI").FirstOrDefault() != null;
            canModRF = functions.Where(e => e.codice == "DO_MOD_CORR_RF").FirstOrDefault() != null;
            canModReg = functions.Where(e => e.codice == "DO_MOD_CORR_REG").FirstOrDefault() != null;
            canDelAll = true;
            canDelReg = true;
            canDelRF = true;
        }

        #region Funzioni di estrazione dati

        /// <summary>
        /// Funzione per la selezione dei dati riguardanti una particolare operazione
        /// e creazione degli oggetti con i dati sui corrispondenti
        /// </summary>
        /// <param name="oleConnection">La connessione da sfruttare per l'estrazione dati</param>
        /// <param name="operation">L'operazione da sfruttare come filtro per l'estrazione dati</param>
        /// <returns>La lista di oggetti con le informazioni sui corrispondenti su cui si desidera effettuare l'operazione specificata</returns>
        private List<AddressBookRowData> ExtractData(
            OleDbConnection oleConnection,
            OperationEnum operation)
        {
            // L'oggetto da restituire
            List<AddressBookRowData> toReturn;

            // La query da eseguire
            OleDbCommand oleCommand;

            // Il data reader con cui leggere i dati estratti
            OleDbDataReader dataReader;

            // Creazione del comando per l'estrazione dati
            oleCommand = new OleDbCommand(
                String.Format("SELECT * FROM [RUBRICA$] WHERE Storicizza = '{0}'", operation.ToString()),
                oleConnection);

            try
            {
                // Esecuzione della query
                dataReader = oleCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                throw new ImportAddressBookException("Errore durante l'estrazione dei dati sui corrispondenti.");
            }

            // Creazione dell'oggetto da restituire
            toReturn = new List<AddressBookRowData>();

            // Finchè ci sono dati da leggere...
            while (dataReader.Read())
                // ...estrazione dei dati e aggiunta dell'oggetto estratto alla lista dei
                // dati da restituire
                toReturn.Add(this.ReadDataFromCurrentRow(dataReader));

            // Restituzione del risultato
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'estrazione dei dati da una riga del foglio Excel
        /// </summary>
        /// <param name="row">La riga da cui estrarre i dati</param>
        /// <returns>L'oggetto con i dati estratti dalla riga</returns>
        private AddressBookRowData ReadDataFromCurrentRow(OleDbDataReader row)
        {
            // L'oggetto da restituire
            AddressBookRowData toReturn = new AddressBookRowData();

            // Lettura del codice registro
            toReturn.RegistryCode = row["Codice registro"].ToString().Trim().ToUpper();

            // Lettura del codice rubrica
            toReturn.AddressBookCode = row["Codice rubrica"].ToString().Trim();

            // Lettura del codice dell'amministrazione
            toReturn.AdministrationCode = row["Codice amministrazione"].ToString().Trim().ToUpper();

            // Lettura del codice dell'AOO
            toReturn.AOOCode = row["Codice AOO"].ToString().Trim().ToUpper();

            // Lettura del tipo di corrispondente
            if (row["Tipo"] != null)
                switch (row["Tipo"].ToString().Trim().ToUpper())
                {
                    case "P":
                        toReturn.CorrType = AddressBookRowData.CorrEnum.P;
                        break;

                    case "R":
                        toReturn.CorrType = AddressBookRowData.CorrEnum.R;
                        break;

                    case "U":
                        toReturn.CorrType = AddressBookRowData.CorrEnum.U;
                        break;
                }

            // Lettura della descrizione
            toReturn.Description = row["Descrizione"].ToString().Trim();
            if (string.IsNullOrEmpty(toReturn.Description))
                toReturn.Description = row["Cognome"].ToString().Trim() + " " + row["Nome"].ToString().Trim();

            // Lettura del cognome
            toReturn.Surname = row["Cognome"].ToString().Trim();

            // Lettura del nome
            toReturn.Name = row["Nome"].ToString().Trim();

            // Lettura dell'indirizzo
            toReturn.Address = row["Indirizzo"].ToString().Trim();

            // Lettura del CAP
            toReturn.PostalCode = row["Cap"].ToString().Trim();

            // Lettura della città
            toReturn.City = row["Città"].ToString().Trim();

            // Lettura della provincia
            toReturn.District = row["Provincia"].ToString().Trim();

            // Lettura della nazione
            toReturn.Nation = row["Nazione"].ToString().Trim();

            // Lettura del codice fiscale/Partira iva
            toReturn.FiscalCode = row["Codice Fiscale/Partita Iva"].ToString().Trim();

            // Lettura del numero di telefono primario
            toReturn.PhoneNumber1 = row["Tel1"].ToString().Trim();

            // Lettura del numero di telefono secondario
            toReturn.PhoneNumber2 = row["Tel2"].ToString().Trim();

            // Lettura del numero di Fax
            toReturn.FaxNumber = row["Fax"].ToString().Trim();

            // Lettura della mail
            toReturn.Email = row["Email"].ToString().Trim();

            // Lettura del codice del registro da cui spostare il corrispondente
            toReturn.FromRegistry = row["Registro da cui spostare"].ToString().Trim().ToUpper();

            // Lettura del canale preferenziale
            toReturn.Channel = row["Canale Preferenziale"].ToString().ToUpper();

            // Lettura della località
            toReturn.Localita = row["Località"].ToString();

            // Restituzione del risultato
            return toReturn;
        }

        #endregion

        #region Validazione dati

        /// <summary>
        /// Funzione per la validazione dei dati estratti da una riga di foglio Excel.
        /// </summary>
        /// <param name="rowData">L'oggetto di cui validare i dati</param>
        /// <param name="problems">Lista dei problemi riscontrati in fase di validazione</param>
        /// <param name="operation">L'operazione da compiere sul corrispondente</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="canDoOpInAll">Possibilità di compiere l'operazione in Tutti</param>
        /// <param name="canDoOpInReg">Possibilità di compiere l'operazione in un Registro</param>
        /// <param name="canDoOpInRF">Possibilità di compiere l'operazione in un RF</param>
        /// <returns>True se i dati sono validi</returns>
        private bool ValidateData(
            AddressBookRowData rowData,
            out List<String> problems,
            OperationEnum operation,
            InfoUtente userInfo,
            Ruolo role,
            bool canDoOpInAll,
            bool canDoOpInRF,
            bool canDoOpInReg)
        {
            #region Dichiarazione Variabili

            // La lista dei problemi riscontrati in fase di validazione
            List<String> validationProblems;

            // Lista temporanea dei problemi riscontrati in una delle fasi di validazione
            List<String> temp;

            #endregion

            // Inizializzazione variabili
            validationProblems = new List<string>();

            // Validazione dei campi comuni
            this.ValidateCommonFields(
                rowData.AddressBookCode,
                rowData.RegistryCode,
                canDoOpInAll,
                canDoOpInReg,
                canDoOpInRF,
                userInfo,
                out temp);

            // Aggiunta dei problemi di validazione dei campi comuni alla lista dei problemi
            validationProblems.AddRange(temp);

            // Validazione dei campi specifici, se l'operazione non è cancellazione
            if (operation != OperationEnum.C)
            {
                this.ValidateSpecificFields(
                    userInfo,
                    rowData,
                    operation,
                    canDoOpInAll,
                    canDoOpInRF,
                    canDoOpInReg,
                    out temp);

                // Aggiunta dei problemi di validazione dei campi specififci alla lista dei problemi
                validationProblems.AddRange(temp);

                // Validazione lunghezza dei campi di dettaglio
                this.ValidateFieldsLength(
                    rowData,
                    out temp);

                // Aggiunta dei problemi di validazione della lunghezza dei campi alla lista dei problemi
                validationProblems.AddRange(temp);

            }

            // Impostazione della variabile di out
            problems = validationProblems;

            // Restituzione del risultato di validazione
            return validationProblems.Count == 0;

        }

        /// <summary>
        /// Funzione per la validazione dei campi comuni 
        /// </summary>
        /// <param name="addressBookCode">Il codice rubrica del corrispondente su cui operare</param>
        /// <param name="registryCode">Il codice del registro</param>
        /// <param name="canDoOpInAll">Possibilità da parte dell'utente di compiere l'operazione su tutti</param>
        /// <param name="canDoOpInReg">Possibilitò da parte dell'utente di compiere l'operazione su Registri</param>
        /// <param name="canDoOpInRF">Possibilità da parte dell'utente di compiere l'operazione su RF</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <returns>True se i campi comuni sono validi</returns>
        private bool ValidateCommonFields(
            string addressBookCode,
            string registryCode,
            bool canDoOpInAll,
            bool canDoOpInReg,
            bool canDoOpInRF,
            InfoUtente userInfo,
            out List<String> problems)
        {
            // La lista dei problemi riscontrati in fase di validazione de campi comuni
            List<String> validationProblems;

            // Espressione regolare per la validazione del codice rubrica
            Regex isValidAddressBookCode;

            // Inizializzazione delle variabili
            validationProblems = new List<string>();
            isValidAddressBookCode = new Regex(@"^[0-9A-Za-z_\ \.\-]+$");

            // Validazione del codice rubrica
            if (String.IsNullOrEmpty(addressBookCode) || !isValidAddressBookCode.Match(addressBookCode).Success)
                validationProblems.Add("Il codice del corrispondente non è stato specificato o non è valido.");

            // Validazione del registro
            validationProblems.AddRange(
                this.ValidateInformationForRegistryOrRF(
                    registryCode,
                    userInfo,
                    canDoOpInAll,
                    canDoOpInReg,
                    canDoOpInRF));

            // Salvataggio della lista dei problemi
            problems = validationProblems;

            // Restituzione del valore
            return problems.Count == 0;

        }

        /// <summary>
        /// Funzione per la validazione dei campi specifici.
        /// </summary>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="rowData">L'oggetto con i dati da validare</param>
        /// <param name="operation">L'operazione da compiere</param>
        /// <param name="canDoOpInAll">Possibilità da parte del ruolo a cui è associato l'utente di compiere l'operazione su Tutti</param>
        /// <param name="canDoOpInRF">Possibilità da parte del ruolo a cui è associato l'utente di compiere l'operazione su RF</param>
        /// <param name="canDoOpInReg">Possibilità da parte del ruolo a cui è associato l'utente di compiere l'operazione su Registri</param>
        /// <param name="problems">La lista dei problemi riscontrati in fase di validazione</param>
        /// <returns>True se la validazione va a buon fine</returns>
        private bool ValidateSpecificFields(
            InfoUtente userInfo,
            AddressBookRowData rowData,
            OperationEnum operation,
            bool canDoOpInAll,
            bool canDoOpInRF,
            bool canDoOpInReg,
            out List<String> problems)
        {
            #region Dichiarazione Variabili

            // La lista dei problemi riscontrati in fase di validazione
            List<String> validationProblems;

            // Espressione regolare per il controllo numerico 
            Regex isNumeric;

            // Espressione regolare per la validazione della provincia
            Regex isValidProvince;

            // Lista temporanea dei problemi verificati in fase di verifica su registro o RF
            List<string> tmpProblems;

            // Espressione regolare per la validazione dell'indirizzo email
            Regex isValidEmail;

            #endregion

            // Inizializzazione dei valori
            validationProblems = new List<string>();
            isNumeric = new Regex("[0-9]+");
            isValidProvince = new Regex("[A-Za-z]+");
            isValidEmail = new Regex(@"^[\w\-\.]*[\w\.]\@[\w\.]*[\w\-\.]+[\w\-]+[\w]\.+[\w]+[\w $]");

            // Se è stato specificato un indirizzo email, viene validato
            if (!String.IsNullOrEmpty(rowData.Email) &&
                !isValidEmail.Match(rowData.Email).Success)
                validationProblems.Add("Email specificata non valida.");

            // Se il canale di comunicazione è interoperabilità, codice AOO ed email devono
            // essere specificati
            if (!String.IsNullOrEmpty(rowData.Channel) &&
                rowData.Channel.ToUpper().Equals("INTEROPERABILITA") &&
                (String.IsNullOrEmpty(rowData.AOOCode) ||
                String.IsNullOrEmpty(rowData.Email) ||
                String.IsNullOrEmpty(rowData.AdministrationCode)))
                validationProblems.Add("Per poter utilizzare il canale Interoperabilità è necessario specificare il codice Amministrazione, il codice AOO ed una email valida.");

            // Se il canale preferenziale è mail, deve essere impostata la mail
            if (!String.IsNullOrEmpty(rowData.Channel) &&
                rowData.Channel.ToUpper().Equals("MAIL") &&
                String.IsNullOrEmpty(rowData.Email))
                validationProblems.Add("Per poter utilizzare il canale Mail è necessario specificare una mail valida.");

            // Se il tipo corrispondente è Persona devono essere validati i campi nome e cognome
            // altrimenti bisogna validare il campo descrizione
            if (rowData.CorrType == AddressBookRowData.CorrEnum.P)
            {
                if (String.IsNullOrEmpty(rowData.Name))
                    validationProblems.Add("Il nome è obbligatorio per corrispondenti di tipo Persona");

                if (String.IsNullOrEmpty(rowData.Surname))
                    validationProblems.Add("Il cognome è obbligatorio per corrispondenti di tipo Persona");
            }
            else
                if (String.IsNullOrEmpty(rowData.Description))
                    validationProblems.Add("La descrizione è obbligatoria per corrispondenti di tipo Ruolo e UO");

            // Se l'operazione da compiere è inserimento ed il tipo corrispondente è Persona...
            if (operation == OperationEnum.I && rowData.CorrType == AddressBookRowData.CorrEnum.P)
            {
                // ...devono essere validati i campi caratteristici della persona
                // Validazione dell'indirizzo
                if (String.IsNullOrEmpty(rowData.Address))
                    validationProblems.Add("L'indirizzo del corrispondente è obbligatorio in caso di corrispondente di tipo persona.");

                // Validazione CAP
                if (String.IsNullOrEmpty(rowData.PostalCode) || !isNumeric.Match(rowData.PostalCode).Success)
                    validationProblems.Add("Il codice postale, obbligatorio per corrispondenti di tipo Persona, non è stato specificato o non è un numero.");

                // Validazione Città
                if (String.IsNullOrEmpty(rowData.City))
                    validationProblems.Add("La città è obbligatoria per corrispondenti di tipo Persona.");

                // Validazione Provincia
                if (String.IsNullOrEmpty(rowData.District) || !isValidProvince.Match(rowData.District).Success)
                    validationProblems.Add("La provincia, obbligatoria per corrispondenti di tipo Persona, non è stata specificata o non è valida.");

                // Validazione Nazione
                if (String.IsNullOrEmpty(rowData.Nation))
                    validationProblems.Add("La nazione è obbligatoria per corrispondenti di tipo Persona");

            }

            // Se è stato specificato anche un registro di partenza,
            // e questo è diverso da quello riportato in RegistryCode, si procede
            // alla verifica del registro FromRegistry
            if (operation == OperationEnum.M &&
                !String.IsNullOrEmpty(rowData.FromRegistry))
            {
                tmpProblems = this.ValidateInformationForRegistryOrRF(
                    rowData.FromRegistry,
                    userInfo,
                    canDoOpInAll,
                    canDoOpInReg,
                    canDoOpInRF);

                // Aggiunta dei problemi temporanei alla lista dei problemi
                validationProblems.AddRange(tmpProblems);

            }

            // Impostazione della lista dei problemi
            problems = validationProblems;

            // Restituzione del risultato di validazione
            return problems.Count == 0;
        }

        /// <summary>
        /// Funzione per la validazione della lunghezza dei campi
        /// </summary>
        /// <param name="rowData">L'oggetto con i dati da validare</param>
        /// <param name="problems">I problemi riscontrati in fase di validazione</param>
        /// <returns>True se la validazione passa</returns>
        private bool ValidateFieldsLength(AddressBookRowData rowData, out List<string> problems)
        {
            // Lista dei problemi da restituire
            List<string> validationProblems;

            // Inizializzazione variabili
            validationProblems = new List<string>();

            // L'indirizzo non deve essere più lungo di 128 caratteri
            if (!String.IsNullOrEmpty(rowData.Address) && rowData.Address.Length > 128)
                validationProblems.Add("L'indirizzo non deve superare i 128 caratteri di lunghezza.");

            // Il cap non deve superare i 5 caratteri
            if (!String.IsNullOrEmpty(rowData.PostalCode) && rowData.PostalCode.Length > 5)
                validationProblems.Add("Il CAP non deve superare i 5 caratteri di lunghezza.");

            // La provincia non deve superare i 2 caratteri
            if (!String.IsNullOrEmpty(rowData.District) && rowData.District.Length > 2)
                validationProblems.Add("Il codice di provincia non deve superare i 2 caratteri di lunghezza.");

            // La nazione non deve superare i 32 caratteri
            if (!String.IsNullOrEmpty(rowData.Nation) && rowData.Nation.Length > 32)
                validationProblems.Add("La nazione non deve superare i 32 caratteri di lunghezza.");

            // Il codice ficale non deve superare i 16 caratteri
            if (!String.IsNullOrEmpty(rowData.FiscalCode) && rowData.FiscalCode.Length > 16)
                validationProblems.Add("Il codice fiscale non deve superare i 16 caratteri di lunghezza.");

            // I telefoni ed il fax non deve superare i 16 caratteri
            if (!String.IsNullOrEmpty(rowData.PhoneNumber1) && rowData.PhoneNumber1.Length > 16)
                validationProblems.Add("Il telefono primario non deve superare i 16 caratteri di lunghezza.");
            if (!String.IsNullOrEmpty(rowData.PhoneNumber2) && rowData.PhoneNumber2.Length > 16)
                validationProblems.Add("Il telefono secondario non deve superare i 16 caratteri di lunghezza.");
            if (!String.IsNullOrEmpty(rowData.FaxNumber) && rowData.FaxNumber.Length > 16)
                validationProblems.Add("Il numero di fax non deve superare i 16 caratteri di lunghezza.");

            // La città non deve superare i 64 caratteri
            if (!String.IsNullOrEmpty(rowData.City) && rowData.City.Length > 64)
                validationProblems.Add("Il numero di fax non deve superare i 16 caratteri di lunghezza.");

            // Impostazione della lista dei problemi riscontrati in fase di validazione
            problems = validationProblems;

            // Restituizione risultato
            return validationProblems.Count == 0;

        }

        /// <summary>
        /// Funzione per la validazione delle informazioni per registri ed RF
        /// </summary>
        /// <param name="registryOrRFCode">Il codice dal registro da validare</param>
        /// <param name="role">Il ruolo dell'utente che ha lanciato la procedura</param>
        /// <param name="canDoOpInAll">Possibilità da parte del ruolo cui appartiene l'utente di compiere l'operazione specificata su Tutti</param>
        /// <param name="canDoOpInReg">Possibilità da parte del ruolo cui appartiene l'utente di compiere l'operazione specificata su Registri</param>
        /// <param name="canDoOpInRF">Possibilità da parte del ruolo cui appartiene l'utente di compiere l'operazione specificata su RF</param>
        /// <returns>La lista degli eventuali problemi riscontrati in fase di verifica</returns>
        private List<string> ValidateInformationForRegistryOrRF(
            string registryOrRFCode,
            InfoUtente userInfo,
            bool canDoOpInAll,
            bool canDoOpInReg,
            bool canDoOpInRF)
        {
            // Il valore da restituire
            List<string> toReturn;

            // L'identificativo del registro
            string registryId;

            // Il registro da validare
            Registro registryToValidate;

            // Creazione della lista dei problemi
            toReturn = new List<string>();

            // Se il registro non è specificato, errore
            if (String.IsNullOrEmpty(registryOrRFCode))
                toReturn.Add("E' obbligatorio specificare un registro.");
            else
                if (registryOrRFCode == "TUTTI")
                {
                    if (!canDoOpInAll)
                        toReturn.Add("Utente non abilitato a compiere l'operazione su tutti");
                }
                else
                {
                    // Altrimenti bisogna verificare se il ruolo è abilitato a compiere
                    // l'operazione su registro / RF
                    try
                    {
                        // Prelevamento dell'id del registro
                        registryId = ImportUtils.GetRegistryFromCode(userInfo.idAmministrazione, registryOrRFCode).systemId;

                        // Recupero informazioni sul registro
                        registryToValidate = RegistriManager.getRegistro(registryId);

                        // Se registryToValidate è un registro, l'utente deve essere abilitato 
                        // all'inserimento / modifica su registro altrimenti deve essere abilitato
                        // a inserimento / modifica su RF
                        if (registryToValidate.chaRF == "1")
                        {
                            if (!canDoOpInRF)
                                toReturn.Add("Utente non abilitato a compiere l'operazione su RF");
                        }
                        else
                            if (!canDoOpInReg)
                                toReturn.Add("Utente non abilitato a compiere l'operazione su Registri");

                    }
                    catch (Exception e)
                    {
                        toReturn.Add(e.Message);
                    }
                }

            // Restituzione del risultato
            return toReturn;

        }

        #endregion

    }

}