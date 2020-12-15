using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using BusinessLogic.Documenti;
using BusinessLogic.Fascicoli;
using BusinessLogic.Rubrica;
using BusinessLogic.Utenti;
using DocsPaVO.addressbook;
using DocsPaVO.amministrazione;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using DocsPaVO.Import;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.rubrica;
using DocsPaVO.utente;
using log4net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
namespace BusinessLogic.Import
{
    class ImportUtils
    {
        private static ILog logger = LogManager.GetLogger(typeof(ImportUtils));
        // Culture info per l'Italia
        static CultureInfo ci = new CultureInfo("it-IT");
        // Lista dei formati data accettati
        static String[] dateFormats = { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy", "HH:mm:ss" };

        #region Gestione file temporaneo

        /// <summary>
        /// Funzione per la creazione del file temporaneo
        /// </summary>
        /// <param name="content">Il contenuto del file Excel</param>
        /// <param name="modelPath">Il path in cui salvare il file temporaneo</param>
        /// <param name="fileName">Il nome da attribuire al file temporaneo</param>
        public static string CreateTemporaryFile(byte[] content, string modelPath, string fileName)
        {
            // Il path completo del file temporaneo creato sul server
            string completePath = String.Empty;

            // Il file stream utilizzato per scrivere il file
            FileStream fileStream = null;

            // 1. Controllo dell'esistenza della directory in cui posizionare il file
            // temporaneo. Se non esiste viene creato.
            try
            {
                if (!Directory.Exists(modelPath + @"\Modelli\Import\"))
                    Directory.CreateDirectory(modelPath + @"\Modelli\Import\");
            }
            catch (Exception e)
            {
                // Viene rilanciata l'eccezione ai livelli superiori
                throw new ImportException("Errore durante la creazione della directory in cui depositare il file temporaneo. Dettagli: " + e.Message);
            }

            // 2. Creazione del path completo per l'accesso al file
            completePath = String.Format(@"{0}\Modelli\Import\{1}",
                modelPath, fileName);

            // 3. Apertura dello stream in scrittura
            try
            {
                fileStream = new FileStream(completePath, FileMode.OpenOrCreate, FileAccess.Write);
            }
            catch (Exception e)
            {
                // Viene rilanciata l'eccezione ai livelli superiori
                throw new ImportException("Errore durante la creazione del file temporaneo. Dettagli: " + e.Message);
            }

            // 4. Scrittura dei dati nel file
            try
            {
                fileStream.Write(content, 0, content.Length);
            }
            catch (Exception e)
            {
                // Viene rilanciata l'eccezione al livello superiore
                throw new ImportException("Errore durante la scrittura dei dati nel file temporaneo. Dettagli: " + e.Message);
            }

            // 5. Chiusura dello stream
            fileStream.Close();

            // Restituzione del path completo
            return completePath;

        }

        /// <summary>
        /// Funzione per l'eliminazione del file temporaneo
        /// </summary>
        /// <param name="completePath">Il path completo da cui è possibile reperire il file</param>
        public static void DeleteTemporaryFile(string completePath)
        {
            try
            {
                File.Delete(completePath);
            }
            catch (Exception e)
            {
                // Viene lanciata un'eccezione al livello superiore
                logger.Debug("Errore durante l'eliminazione del file temporaneo.", e);
            }

        }

        #endregion

        #region Gestione connessione a file Excel

        /// <summary>
        /// Funzione per la connessione ad una sorgente dati
        /// </summary>
        /// <param name="provider">Classpath del provider</param>
        /// <param name="extendedProperty">Proprietà avanzate di connessione</param>
        /// <param name="completePath">Path del file cui connettersi</param>
        /// <returns>L'oggetto per la gestione della connessione</returns>
        public static OleDbConnection ConnectToFile(string provider, string extendedProperty, string completePath)
        {
            // Stringa di connessione da utilizzare per connettersi al foglio excel
            string connectionString;
            
            // L'oggetto da restituire
            OleDbConnection oleConnection;

            // Creazione della stringa di connessione
            connectionString = String.Format("Provider={0}Data Source={1};Extended Properties=\"{2}\"",
                provider,
                completePath,
                extendedProperty);
            logger.Debug(connectionString);
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
                logger.Error("Errore durante la connessione al file excel.");
                logger.Error(e.Message);
                logger.Error(e.StackTrace);
                throw new ImportException("Errore durante la connessione al file excel. Dettagli: " + e.Message);

            }

            // Restituzione della connessione aperta
            return oleConnection;

        }

        /// <summary>
        /// Funzione per la chiusura di una connessione
        /// </summary>
        /// <param name="connection">La connessione da chiudere</param>
        public static void CloseConnection(OleDbConnection connection)
        {
            try
            {
                connection.Close();
            }
            catch (Exception e)
            {
                // Viene rilanciata un'eccezione al livello superiore
                throw new ImportException("Errore durante la disconnessione dal file excel. Dettagli: " + e.Message);
            }
 
        }

        #endregion

        #region Compilazione campo profilato
        public static Corrispondente FindCorrispondente(string codice,OggettoCustom customObject,InfoUtente userInfo,Ruolo role,string RFCode,string registryCode,string administrationId,bool isEnabledSmistamento)
        {
            Registro registry;
            string rfSyd=string.Empty;
            string registrySyd=string.Empty;
            TipoUtente userType;
            if (!String.IsNullOrEmpty(RFCode))
            {
                try
                {
                    // Reperimento dei dati sull'RF
                    registry = RegistriManager.getRegistroByCodAOO(
                        RFCode.ToUpper(), administrationId);
                }
                catch (Exception e)
                {
                    throw new ImportException(
                        String.Format("Errore durante il reperimento delle informazioni sul Registro/RF {0}",
                            RFCode));
                }

                // Se il registro non è stato recuperato con successo, viene lanciata
                // un'eccezione
                if (registry == null)
                    throw new ImportException(String.Format("Registro con codice {0} non trovato.",
                        RFCode));

                // Se il registro trovato non è un RF -> errore, altrimenti
                // si aggiunge l'id del registro al filstro registri
                if (registry.chaRF.Trim() != "1")
                    throw new ImportException(String.Format("Il registro {0} specificato, non è un RF.",
                        RFCode));

                rfSyd = registry.systemId;

            }
            #endregion

            #region Registro

            try
            {
                // Reperimento dei dati sul registro
                registry = RegistriManager.getRegistroByCodAOO(
                    registryCode.ToUpper(), administrationId);
            }
            catch (Exception e)
            {
                throw new ImportException(
                    String.Format("Errore durante il reperimento delle informazioni sul Registro/RF {0}",
                        registryCode));
            }

            // Se il registro non è stato recuperato con successo -> eccezione,
            // altrimenti viene aggiunto l'id del registro al filtro registri
            if (registry == null)
                throw new ImportException(String.Format("Registro {0} specificato non trovato.",
                    registryCode));

            registrySyd = registry.systemId;

            #endregion

            // Impostazione del codice corrispondente
            // Se il codice non è impostato, eccezione, altrimenti impostazione
            // del codice
            if (String.IsNullOrEmpty(codice))
                throw new ImportException(String.Format("Codice corrispondente non valido."));

            // Individuazione del tipo di utente da ricercare
            switch (customObject.TIPO_RICERCA_CORR.ToUpper())
            {
                case "INTERNI":
                    userType = TipoUtente.INTERNO;

                    break;

                case "ESTERNI":
                    userType = TipoUtente.ESTERNO;

                    break;

                default:
                    userType = TipoUtente.GLOBALE;

                    break;
            }

            // Impostazione del corrispondente
            return GetCorrispondenteByCode(
                ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST,
                codice,
                role,
                userInfo,
                registrySyd,
                rfSyd,
                isEnabledSmistamento,
                userType);

        }

        public static List<String> CompileProfilationField(OggettoCustom customObject, AssDocFascRuoli rights, String[] fieldValues, Ruolo role, InfoUtente userInfo, string RFCode, string administrationId, string registryCode, bool isEnabledSmistamento)
        {
            #region Dichiarazione variabili

            // Il registro
            Registro registry;

            // Il system id dell'RF
            string rfSyd = String.Empty;

            // Il system id del registro
            string registrySyd = String.Empty;

            // Tipologia di utente da ricercare
            TipoUtente userType;

            // La lista dei problemi emersi durante la compilazione del campo
            List<String> toReturn;

            // Oggetto in cui depositare le informazioni sul campo da selezionare
            ValoreOggetto objectValue;

            #endregion

            // Creazione della lista da restituire
            toReturn = new List<string>();

            // ...a seconda del tipo di oggetto bisogna intraprendere
            // operazioni diverse per quanto riguarda l'assegnazione dei valori
            switch (customObject.TIPO.DESCRIZIONE_TIPO.ToUpper())
            {
                case "CASELLADISELEZIONE":
                    // Nel caso della casella di selezione è possibile che sia selezionato
                    // più di un valore
                    // Se il ruolo può modificare il campo...
                    if (rights.INS_MOD_OGG_CUSTOM == "1")
                        // ...vengono impostati i valori
                        // Per ogni stringa contenuta all'interno dei valori selezionati,
                        // bisogna ricercare l'oggetto con le informazioni sull'opzione da selezionare
                        // ricavandone la posizione ed inserendo tale descrizione nella stessa posizione
                        // ma nell'array VALORI_SELEZIONATI
                        foreach (string selectedValue in fieldValues)
                        {
                            objectValue = ((ValoreOggetto[])customObject.ELENCO_VALORI.ToArray(typeof(ValoreOggetto))).Where(
                                e => e.VALORE.ToUpper().Equals(selectedValue.ToUpper())).FirstOrDefault();

                            // Se il valore non è stato reperito correttamente, viene lanciata una eccezione
                            if(objectValue == null)
                                throw new Exception(String.Format("Valore '{0}' non valido", selectedValue));

                            customObject.VALORI_SELEZIONATI[customObject.ELENCO_VALORI.IndexOf(objectValue)] = selectedValue;
                        }
                    else
                        // ...altrimenti non si può impostare il valore. Si procede quindi
                        // all'aggiunta di un messaggio di avviso alla lista dei "warnings"
                        toReturn.Add(String.Format("Non è possible valorizzare il campo '{0}' in quanto il ruolo non possiede diritto di modifica su tale campo",
                            customObject.DESCRIZIONE));
                    break;

                case "CORRISPONDENTE":
                    // Se il ruolo possiede i diritti di modifica sul campo...
                    if (rights.INS_MOD_OGG_CUSTOM == "1")
                    {
                        Corrispondente corr = FindCorrispondente(fieldValues[0], customObject, userInfo, role, RFCode, registryCode, administrationId, isEnabledSmistamento);
                        customObject.VALORE_DATABASE = corr.systemId;
                    }
                    else
                        // Altrimenti si aggiunge un messaggio alla lista dei warnings
                        toReturn.Add(String.Format("Non è possible valorizzare il campo '{0}' in quanto il ruolo non possiede diritto di modifica su tale campo",
                            customObject.DESCRIZIONE));
                    break;

                case "CONTATORE":
                case "CONTATORESOTTOCONTATORE":
                    try
                    {
                        // Reperimento dei dati sul registro
                        registry = RegistriManager.getRegistroByCodAOO(
                            fieldValues[0].ToUpper(), administrationId);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            String.Format("Errore durante il reperimento delle informazioni sul Registro/RF {0}",
                                fieldValues[0]));
                    }

                    switch (customObject.TIPO_CONTATORE.ToUpper())
                    {
                        case "A":   // Contatore di AOO
                            customObject.ID_AOO_RF = registry.systemId;
                            break;
                        case "R":   // Contatore di RF
                            // Se il registro non è un registro di RF, eccezione
                            if (!(registry.chaRF == "1"))
                                throw new Exception(String.Format(
                                    "Il registro {0} non è un RF",
                                    fieldValues[0]));

                            // Impostazione dell'id registro
                            customObject.ID_AOO_RF = registry.systemId;

                            break;
                    }

                    // Se il contatore è abilitato allo scatto differito...
                    if (customObject.CONTA_DOPO == "0")
                        // Se il ruolo ha diritti di modifica sul contatore...
                        if (rights.INS_MOD_OGG_CUSTOM == "1")
                            // Il contatore deve scattare
                            customObject.CONTATORE_DA_FAR_SCATTARE = true;

                    break;
                case "LINK":
                    if(fieldValues.Length<2) throw new Exception("Per costruire un oggetto link sono necessari due valori");
                    if ("INTERNO".Equals(customObject.TIPO_LINK))
                    {
                        if ("DOCUMENTO".Equals(customObject.TIPO_OBJ_LINK))
                        {
                            InfoDocumento infoDoc = null;
                            try
                            {
                                infoDoc = DocManager.GetInfoDocumento(userInfo, fieldValues[1], null, true);
                            }
                            catch (Exception e)
                            {
                                throw new Exception(String.Format("Errore nel reperimento del documento con id {0}", fieldValues[1]));
                            }
                            if (infoDoc == null) throw new Exception(String.Format("Il documento con id {0} non è presente", fieldValues[1]));
                            string errorMessage="";
                            int result = DocManager.VerificaACL("D", infoDoc.idProfile,userInfo,out errorMessage);
                            if (result != 2)
                            {
                                throw new Exception(String.Format("Non si possiedono i diritti per reperire il documento con id {0}", fieldValues[1]));
                            }
                        }
                        else
                        {
                            Fascicolo fasc = null;
                            try
                            {
                                // fasc = FascicoloManager.getFascicoloById(fieldValues[1], userInfo);
                                fasc = FascicoloManager.getFascicoloDaCodice(userInfo, fieldValues[1], null, false, true);
                            }
                            catch (Exception e)
                            {
                                throw new Exception(String.Format("Errore nel reperimento del fascicolo con id {0}", fieldValues[1]));
                            }
                            if (fasc == null) throw new Exception(String.Format("Il fascicolo con id {0} non è presente", fieldValues[1]));
                            string errorMessage = "";
                            int result = DocManager.VerificaACL("F", fasc.systemID, userInfo, out errorMessage);
                            if (result != 2)
                            {
                                throw new Exception(String.Format("Non si possiedono i diritti per reperire il fascicolo con id {0}", fieldValues[1]));
                            }
                        }
                    }
                    customObject.VALORE_DATABASE = fieldValues[0] + "||||" + fieldValues[1];
                    break;
                case "OGGETTOESTERNO":
                    if (fieldValues.Length < 2) throw new Exception("Per costruire un oggetto esterno sono necessari due valori");
                    customObject.MANUAL_INSERT = true;
                    customObject.CODICE_DB = fieldValues[0];
                    customObject.VALORE_DATABASE = fieldValues[1];
                    break;
                default:
                    // In tutti gli altri casi il valore è uno solo
                    // Se il ruolo ha diritti di modofica, viene impostato
                    // il valore altrimenti viene inserito un messaggio nella
                    // lista dei warning
                    if (rights.INS_MOD_OGG_CUSTOM == "1")
                        customObject.VALORE_DATABASE = fieldValues[0];
                    else
                        // ...altrimenti non si può impostare il valore. Si procede quindi
                        // all'aggiunta di un messaggio di avviso alla lista dei "warnings"
                        toReturn.Add(String.Format("Non è possible valorizzare il campo '{0}' in quanto il ruolo non possiede diritto di modifica su tale campo",
                            customObject.DESCRIZIONE));
                    break;

            }

            // Restituzione della lista delle eventuali segnalazioni
            return toReturn;

        }



        /// <summary>
        /// Funzione per la ricerca di un corrispondente a partire dal suo codice.
        /// </summary>
        /// <param name="callType">Il calltype</param>
        /// <param name="corrCode">Il codice del corrispondente da ricercare</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="registrySyd">L'id del registro</param>
        /// <param name="rfSyd">L'id dell'RF</param>
        /// <param name="isEnabledSmistamento">True se è abilitato lo smistamento</param>
        /// <param name="userTypeForProject">La tipologia di utente da ricercare (Interna/Esterna/Globale). Questo parametro viene preso in considerazione solo nel caso di campi custom di tipo Corrispondente.</param>
        /// <returns>Il corrispondente individuato</returns>
        public static Corrispondente GetCorrispondenteByCode(
            ParametriRicercaRubrica.CallType callType,
            string corrCode,
            Ruolo role,
            InfoUtente userInfo,
            string registrySyd,
            string rfSyd,
            bool isEnabledSmistamento,
            TipoUtente userTypeForProject)
        {
            #region Dichiarazione Variabili

            // L'oggetto utilizzato per memorizzare i parametri di ricerca
            ParametriRicercaRubrica searchParameters;

            // L'oggetto per memorizzare le impostazioni sullo smistamento
            SmistamentoRubrica smistamentoRubrica;

            // L'oggetto per l'effettuazione delle ricerche nella rubrica
            DPA3_RubricaSearchAgent corrSearcher;

            // La lista degli elementi resttiuiti dalla ricerca
            ArrayList corrList;

            // Il corrispondente da restituire
            Corrispondente toReturn = null;

            #endregion

            #region Impostazione parametri di ricerca

            // Creazione oggetto per la memorizzazione dei parametri di ricerca
            searchParameters = new ParametriRicercaRubrica();

            // Impostazione del call type
            searchParameters.calltype = callType;

            // Impostazione del codice da ricercare
            searchParameters.codice = corrCode;

            // Impostazione del flag per la ricerca del codice esatta
            searchParameters.queryCodiceEsatta = true;

            // Creazione del caller
            searchParameters.caller = new ParametriRicercaRubrica.CallerIdentity();

            // Impostazione del calltype
            searchParameters.caller.IdRuolo = role.systemId;

            // Impostazione dell'id utente
            searchParameters.caller.IdUtente = userInfo.idPeople;

            // Impostazione dell'id registro
            searchParameters.caller.IdRegistro = registrySyd;

            // Impostazione del filtro registro per la ricerca
            searchParameters.caller.filtroRegistroPerRicerca = registrySyd;

            // La ricerca va effettuata su Uffici, Utenti, Ruoli, RF
            searchParameters.doUo = true;
            searchParameters.doUtenti = true;
            searchParameters.doRuoli = true;
            searchParameters.doRF = true;
            bool abilitazioneRubricaComune = BusinessLogic.RubricaComune.Configurazioni.GetConfigurazioni(userInfo).GestioneAbilitata;
            searchParameters.doRubricaComune = abilitazioneRubricaComune;
            #endregion

            #region Impostazione parametri per smistamento

            // Creazione oggetto per parametri smistamento
            smistamentoRubrica = new SmistamentoRubrica();

            // Abilitazione smistamento
            smistamentoRubrica.smistamento = isEnabledSmistamento ? "1" : "0";

            // Impostazione calltype
            smistamentoRubrica.calltype = callType;

            // Impostazione informazioni sull'utente
            smistamentoRubrica.infoUt = userInfo;

            // Impostazione ruolo
            smistamentoRubrica.ruoloProt = role;

            // Impostazione dell'id del registro
            smistamentoRubrica.idRegistro = registrySyd;

            #endregion

            #region Impostazione parametri dipendenti dal contesto

            // Impostazione parametri dipendenti dal contesto
            switch (callType)
            {
                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN:
                    if (!String.IsNullOrEmpty(rfSyd))
                        searchParameters.caller.filtroRegistroPerRicerca += ", " + rfSyd;
                    searchParameters.doRubricaComune = abilitazioneRubricaComune;
                    searchParameters.doRubricaComune = true;
                    smistamentoRubrica.daFiltrareSmistamento = "0";
                    searchParameters.tipoIE = TipoUtente.GLOBALE;

                    break;

                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_MITT:
                    smistamentoRubrica.daFiltrareSmistamento = "0";
                    searchParameters.tipoIE = TipoUtente.INTERNO;

                    break;

                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT:
                    searchParameters.doListe = true;
                    searchParameters.doRubricaComune = abilitazioneRubricaComune;
                    if (!String.IsNullOrEmpty(rfSyd))
                        searchParameters.caller.filtroRegistroPerRicerca += ", " + rfSyd;

                    smistamentoRubrica.daFiltrareSmistamento = "1";
                    searchParameters.tipoIE = TipoUtente.GLOBALE;

                    break;

                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_MITT:
                    smistamentoRubrica.daFiltrareSmistamento = "0";
                    searchParameters.tipoIE = TipoUtente.INTERNO;

                    break;

                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST:
                    searchParameters.doListe = true;
                    smistamentoRubrica.daFiltrareSmistamento = "1";
                    searchParameters.tipoIE = TipoUtente.INTERNO;

                    break;

                case ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST:
                    if (!String.IsNullOrEmpty(rfSyd))
                        searchParameters.caller.filtroRegistroPerRicerca += ", " + rfSyd;
                    searchParameters.doRubricaComune = true;
                    searchParameters.tipoIE = userTypeForProject;
                    smistamentoRubrica.daFiltrareSmistamento = "0";

                    break;

            }

            #endregion

            #region Esecuzione Ricerca

            // Creazione oggetto per la ricerca
            corrSearcher = new DPA3_RubricaSearchAgent(userInfo);

            // Esecuzione della ricerca
            corrList = corrSearcher.Search(searchParameters, smistamentoRubrica);

            #endregion

            #region Gestione risultato

            // Se non sono stati restituiti corrispondenti eccezione
            if (corrList == null || corrList.Count == 0)
                throw new Exception(String.Format(
                    "Nessun corrispondente rilevato con il codice {0}",
                    corrCode));

            // Se sono stati restituiti più corrispondenti, ambiguità
            if (corrList.Count > 1)
                throw new Exception(String.Format(
                    "La ricerca del corrispondente con codice {0} ha restituito {1} risultati. Provare a restringere il campo di ricerca specificando un RF.",
                    corrCode, corrList.Count));

            #endregion
        
            // Reperimento dei dati sul corrispondente
            try
            {
                ElementoRubrica temp=(ElementoRubrica) corrList[0];
                if (temp.isRubricaComune)
                {
                    Corrispondente tempCorr = BusinessLogic.RubricaComune.RubricaServices.UpdateCorrispondente(userInfo, temp.codice);
                    toReturn = tempCorr;
                }
                else
                {
                    toReturn = UserManager.getCorrispondenteBySystemID(temp.systemId);
                }
            }
            catch (Exception e)
            {
                throw new ImportException(
                    String.Format(
                        "Errore durante il reperimento dei dati sul corrispondente con codice '{0}'",
                        corrCode));
            }

            // Restituzione del corrispondente
            return toReturn;

        }

        /// <summary>
        /// Funzione per il calcolo del system id del registro
        /// </summary>
        /// <param name="registryCode">Il codice del registro</param>
        /// <param name="administrationCode">Il codice dell'amministrazione</param>
        /// <returns>Il system id del registro</returns>
        public static string GetRegistrySystemId(string registryCode, string administrationCode)
        {
            // Il system ID da restituire
            string toReturn;

            try
            {
                // Calcolo e restituzione dell'id del registro
                toReturn = RegistriManager.getIdRegistro(administrationCode, registryCode);
            }
            catch (Exception e)
            {
                throw new Exception(
                    String.Format(
                        "Errore durante il reperimento delle informazioni sul registro {0}",
                        registryCode));
            }

            // Se non è stato rilevato alcun registro, eccezione
            if(String.IsNullOrEmpty(toReturn))
                throw new Exception(
                String.Format(
                    "Errore durante il reperimento delle informazioni sul registro {0}",
                    registryCode));

            // Restituzione del risultato
            return toReturn;

        }

        /// <summary>
        /// Funzione per il recupero dell'id dell'amministrazione a partire dal suo codice
        /// </summary>
        /// <param name="administrationCode">Il codice dell'amministrazione di cui si desidera ricavare l'ID</param>
        /// <returns>L'id dell'amministrazione</returns>
        public static string GetAdministrationId(string administrationCode)
        {
            // L'id da restituire
            string toReturn;

            try
            {
                // Calcolo e restituzione dell'id dell'amministrazione
                toReturn = new DocsPaDB.Query_DocsPAWS.Amministrazione().GetIDAmm(administrationCode);
            }
            catch (Exception e)
            {
                throw new ImportException(
                    String.Format(
                    "Errore durante il reperimento delle informazioni sull'amministrazione {0}",
                    administrationCode));
            }

            // Se non è stato recuperata nessuna amministrazione, eccezione
            if(String.IsNullOrEmpty(toReturn))
                throw new ImportException(
                    String.Format(
                    "Errore durante il reperimento delle informazioni sull'amministrazione {0}",
                    administrationCode));

            // Restituzione dell'id
            return toReturn;

        }

        /// <summary>
        /// Funzione per il reperimento dell'id di un RF
        /// </summary>
        /// <param name="rfCode">Il codice dell'RF di cui ricavare l'id</param>
        /// <param name="administrationId">L'id dell'amministrazione</param>
        /// <returns>L'id dell'RF</returns>
        public static string GetRFId(string rfCode, string administrationId)
        {
            // Il registro RF
            Registro registry = null;

            try
            {
                // Reperimento dei dati sul registro
                registry = RegistriManager.getRegistroByCodAOO(
                    rfCode.ToUpper(), administrationId);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Impossibile recuperare informazioni sull'RF {0}",
                    rfCode));
            }

            // Se rf è null non è stato possibile reperire informazioni sull'RF
            if(registry == null)
                throw new Exception(String.Format("Impossibile recuperare informazioni sull'RF {0}",
                    rfCode));

            // Se il registro non è un RF, eccezione
            if (!(registry.chaRF == "1"))
                throw new Exception(String.Format(
                    "Il registro {0}, non è un RF",
                    rfCode));

            // Restituzione dell'id dell'rf
            return registry.systemId;

        }

        public static string GetTitolarioId(string titolarioName, string administrationSid)
        {
            return GetTitolarioObj(titolarioName, administrationSid).ID;

        }

        public static OrgTitolario GetTitolarioObj(string titolarioName, string administrationSyd)
        {
            #region

            // L'oggetto amministrazione a cui richiedere le informazioni sul titolario
            DocsPaDB.Query_DocsPAWS.Amministrazione administration;

            // L'oggetto temporaneo in cui memorizzare le informazioni sul
            // titolario individuato
            OrgTitolario titolarioObject = null;

            #endregion

            // Creazione dell'oggetto amministrazione
            administration = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            // Se titolatio non è valorizzato viene restituito l'id del titolario
            // attivo nell'amministrazione
            if (String.IsNullOrEmpty(titolarioName))
            {
                try
                {
                    titolarioObject = administration.getTitolarioAttivo(administrationSyd);
                }
                catch (Exception e)
                {
                    // Se non si riscono a recuperare le informazioni sul titolatio attivo,
                    // non viene intrapresa alcuna operazione
                }

            }
            else
            {
                // Altrimenti si procede al caricamento del titolario specificato
                try
                {
                    OrgTitolario[] tits = (OrgTitolario[])administration.getTitolariUtilizzabili(administrationSyd).ToArray(typeof(OrgTitolario));

                    titolarioObject = tits.Where(e => ((DateTime.Parse(e.DataAttivazione).Year.ToString() == titolarioName) && (e.Stato == OrgStatiTitolarioEnum.Chiuso))).FirstOrDefault();

                }
                catch (Exception e)
                {
                    // Se non si riscono a recuperare le informazioni sul titolatio,
                    // non viene intrapresa alcuna operazione
                }

            }

            // Se l'id del titolario è stato specificato, ne viene
            // restituito l'id altrimenti viene lanciata un'eccezione
            if (titolarioObject == null)
                throw new ImportException("Non è stato possibile ricavare informazioni sul titolario.");

            return titolarioObject;
 
        }

        public static Canale LoadChannelInformation(string channelName)
        {
            // L'oggetto da restituire
            Canale toReturn = null;

            // L'array dei canali di trasmissione
            Canale[] channels;

            try
            {
                // Reperimento dei canali di trasmissione salvati sul DB
                channels = (Canale[])addressBookManager.getCanaliMethod().ToArray(typeof(Canale));

                // Recupero del canale di trasmissione richiesto
                toReturn = channels.Where(e => e.descrizione.ToUpper().Equals(channelName.ToUpper())).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new ImportException("Errore durante il reperimento dei dati sui canali di trasmissione.");
            }

            // Restituzione del canale (null nel caso in cui non sia stato possibile recuperare le informazioni
            // sul canale desiderato)
            return toReturn;

        }


        public static Registro GetRegistryFromCode(string administrationId, string registryCode)
        {
            // Il codice dell'amministrazione
            string administrationCode = String.Empty;

            // L'id del registro
            string registryId;

            // Il registro da restituire
            Registro toReturn;

            try
            {
                // Reperimento del codice dell'amministrazione
                administrationCode = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(administrationId).Codice;
            }
            catch(Exception e) {}

            // Reperimento del registro
            registryId = GetRegistrySystemId(registryCode, administrationCode);

            try
            {
                // Reperimento delle informazioni sul registro
                toReturn = RegistriManager.getRegistro(registryId);
            }
            catch (Exception e)
            {
                throw new ImportException(String.Format(
                    "Errore durante il reperimento delle informazioni sul registro {0}",
                    registryCode));
            }
            
            // Restituzione del registro
            return toReturn;
        
        }

        /// <summary>
        /// Questa funzione restituisce il contenuto di un file salvato in una cartella FTP
        /// </summary>
        /// <param name="FTPAddress">Indirizzo del server FTP</param>
        /// <param name="filePath">Path del file da aprire</param>
        /// <param name="username">Username per accedere al server FTP</param>
        /// <param name="password">Password per accedere al server FTP</param>
        /// <returns>Contenuto del file</returns>
        public static byte[] DownloadFileFromFTP(string FTPAddress, string filePath, string username, string password)
        {
            // Richiesta FTP
            FtpWebRequest request;

            // File stream
            Stream stream;

            // Array da restituire
            byte[] toReturn;

            logger.Debug(String.Format("Creazione request FTP per lettura file {0}", filePath));

            // Creazione della request FTP
            request = (FtpWebRequest)FtpWebRequest.Create(FTPAddress + "/" + filePath);

            //
            //SSL Region
            bool ftpSSL = false;
            //Chiave per abilitare l'utilizzo del ftp in modalità SSL
            ftpSSL = ((!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FTPSSL"]) && System.Configuration.ConfigurationManager.AppSettings["FTPSSL"].Equals("0")) || string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FTPSSL"]) ? false : true);

            if (ftpSSL)
            {
                ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertificatePolicy;
                //ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertificatePolicy);

                request.EnableSsl = true;
                //request.AuthenticationLevel = AuthenticationLevel.MutualAuthRequested;
            }
            //End SSL Region
            //

            // Download
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            // Impostazione delle credenziali
            request.Credentials = new NetworkCredential(username, password);

            // Impostazione parametri di connessione
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;
            
            logger.DebugFormat ("Request FTP creata per {0}",request.RequestUri.ToString());

            try
            {
                //Load the file
                logger.Debug("Lettura del file");

                // Apertura stream
                using (stream = request.GetResponse().GetResponseStream())
                {
                    logger.Debug("collegato");
                    List<byte> list = new List<byte>();

                    while (true)
                    {
                        int b = stream.ReadByte();

                        if (b == -1)
                            break;

                        list.Add((byte)b);
                    }

                    // Inizializzazione array
                    toReturn = list.ToArray();
                }

                //// Lettura dei dati
                //stream.Write(toReturn, 0, toReturn.Length);

                //// Chiusura stream
                //stream.Close();

                //Lettura conmpletata con successo
                logger.Debug("Lettura del file completata con successo");
            }
            catch (EntryPointNotFoundException e)
            {
                logger.Error(e);

                throw new Exception("Impossibile leggere le informazioni sul file.");
            } catch (Exception ex)
            {

                logger.ErrorFormat("Errore {0} {1}", ex.Message, ex.StackTrace);
                throw new Exception("Impossibile leggere le informazioni sul file.");
            }

            return toReturn;

        }

        public static byte[] DownloadFileFromUserTempFolder(DocsPaVO.PrjDocImport.DocumentRowData rowData, InfoUtente userInfo, bool isAttachment = false)
        {
            byte[] fileContent = null;
            string docRootPath;
            string attachmentsFolder;
            string userPath;
            string userTempDedicatedFolder;
            string filePath;
            try
            {
                string _rootPath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");

                docRootPath = System.IO.Path.Combine(_rootPath, @"Import\Documents\");  //System.Configuration.ConfigurationManager.AppSettings["IMPORT_TEMP_FOLDER"];
                //docRootPath = System.Configuration.ConfigurationManager.AppSettings["IMPORT_TEMP_FOLDER"];

                // Cartella dedicata Utente
                userPath = System.IO.Path.Combine(docRootPath, userInfo.idPeople);
                if (!System.IO.Directory.Exists(userPath))
                {
                    throw new Exception("Cartella utente non accessibile");
                }

                // cartella temporanea con data odierna
                userTempDedicatedFolder = System.IO.Path.Combine(userPath, DateTime.Now.ToString("yyyyMMdd"));
                if (!System.IO.Directory.Exists(userTempDedicatedFolder))
                {
                    throw new Exception("Cartella temporanea utente non accesibile");
                }

                if (isAttachment)
                {
                    attachmentsFolder = "Attachments"; // System.Configuration.ConfigurationManager.AppSettings["IMPORT_TEMP_ATTACHMENT_FOLDER"];
                    userTempDedicatedFolder = System.IO.Path.Combine(userTempDedicatedFolder, attachmentsFolder);
                    if (!System.IO.Directory.Exists(userTempDedicatedFolder))
                    {
                        throw new Exception("Cartella temporanea utente non accesibile");
                    }
                }


                filePath = System.IO.Path.Combine(userTempDedicatedFolder, rowData.Pathname);
                if (!System.IO.File.Exists(filePath))
                {
                    throw new Exception("File non accessibile");
                }

                fileContent = System.IO.File.ReadAllBytes(filePath);

                System.IO.File.Delete(filePath);
                
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw ex;
            }

            try
            {
                // eliminazione dei file vecchi
                if (!isAttachment)
                {
                    var directories = System.IO.Directory.GetDirectories(userPath);
                    foreach (var folder in directories)
                    {
                        if (!folder.Equals(userTempDedicatedFolder))
                        {
                            System.IO.Directory.Delete(folder, true);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                // ignoro
            }



            return fileContent;
        }



        public static bool AcceptAllCertificatePolicy(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; } 


        /// <summary>
        /// Funzione utilizzata per verificare se una stringa è una data
        /// </summary>
        /// <param name="date">Stringa da verificare</param>
        /// <returns>True se date è una data, false altrimenti</returns>
        public static bool IsDate(string date)
        {

            try
            {
                // Trim della data
                date = date.Trim();
                // Lettura della data
                DateTime d_ap = DateTime.ParseExact(date, dateFormats, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return true;

            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Funzione per la trasformazione di una stringa in data
        /// </summary>
        /// <param name="date">Data da leggere</param>
        /// <returns>Data</returns>
        public static DateTime ReadDate(string date)
        {
            // Trim della data
            date = date.Trim();

            // Lettura e restituzione della data
            return DateTime.ParseExact(date, dateFormats, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

        }
        /// <summary>
        /// TO DO.
        /// A partire dal projectCodes || ProjectDescription restituisce l'ID del Titolario Storicizzato in cui è presente il fascicolo procedimentale aperto
        /// </summary>
        /// <param name="ProjectCodes"></param>
        /// <param name="ProjectDescription"></param>
        /// <returns></returns>
        public static List<string> getTitolarioByCodeFascicolo(string ProjectCodes, string ProjectDescription, string SysIDAmministrazione, string IDRegistro) 
        {
            List<string> listaIDTitolari = new List<string>();

            //TO DO:
            //Query che a partire dal projectCodes || ProjectDescription restituisce l'ID del Titolario in cui è presente il fascicolo
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            listaIDTitolari = amm.getTitolarioByCodeFascicolo(ProjectCodes, ProjectDescription, SysIDAmministrazione, IDRegistro);
            
            return listaIDTitolari;
        }

    }

}
