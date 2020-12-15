using System;
using System.Collections.Generic;
//using NttDataWA.SiteNavigation;
using System.Data;
//using NttDataWA.Import.ReportGenerator;
using NttDataWA.DocsPaWR;
using System.Linq;
using NttDataWA.UIManager;

namespace NttDataWA.Utils
{
    /// <summary>
    /// Questa classe si occupa di fornire supporto alle operazioni massive
    /// </summary>
    public class MassiveOperationUtils
    {
        /// <summary>
        /// Proprietà utilizzata per la gestione dei dati relativi allo stato di 
        /// selezione / deselezione degli item.
        /// Il dizionario contiene per chiave il system id dell'item e per valore un booleano
        /// che, se impostato a true indica che un item è selezionato altrimenti indica che un item non
        /// è selezionato
        /// </summary>
        public static Dictionary<String, MassiveOperationTarget> ItemsStatus
        {
            get
            {
                try
                {
                    return System.Web.HttpContext.Current.Session["massiveOperation"] as Dictionary<String, MassiveOperationTarget>;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    System.Web.HttpContext.Current.Session["massiveOperation"] = value;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

        public static MassiveOperationTarget getItem(string id)
        {
            try
            {
                MassiveOperationTarget res=null;

                string tempId;

                if (ItemsStatus.ContainsKey(id))
                    tempId = id;
                else if (ItemsStatus.ContainsKey("C" + id))
                    tempId = "C" + id;
                else if (ItemsStatus.ContainsKey("P" + id))
                    tempId = "P" + id;
                else
                    tempId = string.Empty;

                if(!string.IsNullOrEmpty(tempId))
                    res = ItemsStatus[tempId].Clone;
                
                return res;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per il reperimento della lista di system id relativi agli item
        /// selezionati
        /// </summary>
        public static List<MassiveOperationTarget> GetSelectedItems()
        {
            try
            {
                // Il dizionario di item
                Dictionary<String, MassiveOperationTarget> status;
                // La lista da restituire
                List<MassiveOperationTarget> toReturn;

                // Creazione della lista da restituire
                toReturn = new List<MassiveOperationTarget>();

                // Recupero del dizionario degli stati di selezione degli item
                status = ItemsStatus;

                if (status != null)
                {
                    // Recupero degli item selezionati
                    foreach (string key in status.Keys)
                        toReturn.Add(status[key]);
                }

                // Restituzione della lista di system id
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione utilizzata per verificare se la collezione dello stato
        /// degli item è valorizzata e contiene valori
        /// </summary>
        /// <returns>True se la collezione degli elementi è valorizzata e contiene valori</returns>
        public static bool IsCollectionInitialized()
        {
            try
            {
                return ItemsStatus != null &&
                    ItemsStatus.Values.Count > 0;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Tipo di oggetto cui appartengono i system id presenti nel dizionario
        /// </summary>
        public static TrasmissioneTipoOggetto ObjectType { get; set; }

    }

    /// <summary>
    /// Questa classe rappresenta un report per le operazioni massive.
    /// Consente di gestire in modo del tutto trasparente alle pagine, un
    /// report con informaizoni sull'esito di una operazione effettuata su
    /// un insieme di documenti / fascicoli
    /// </summary>
    public class MassiveOperationReport
    {       
        // Il dataset in cui memorizzare il report
        DataSet reportDataSet;

        // Numero delle operazioni andate a buon fine, di quelle fallite
        // e di quelle non eseguite in quanto l'oggetto era già nello
        // stato lavorato
        private int worked, notWorked, alreadyWorked;

        /// <summary>
        /// Costruttore per l'inizializzazione del report
        /// </summary>
        public MassiveOperationReport()
        {
            try
            {
                // Creazione e inizializzazione del dataset.
                this.reportDataSet = this.Initialize();

                this.worked = 0;
                this.notWorked = 0;
                this.alreadyWorked = 0;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public int Worked
        {
            get
            {
                try
                {
                    return worked;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return 0;
                }
            }
        }

        public int NotWorked
        {
            get
            {
                try
                {
                    return notWorked;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return 0;
                }
            }
        }

        /// <summary>
        /// Enumerazione dei possibili esiti dell'operazione.
        /// Sono previsti i seguenti esiti:
        /// - OK: documento / fascicolo elaborato correttamente
        /// - KO: documento / fascicolo non elaborato
        /// - AlreadyWorked: Già nello stato lavorato
        /// </summary>
        public enum MassiveOperationResultEnum
        {
            OK,
            KO,
            AlreadyWorked
        }

        /// <summary>
        /// Funzione per l'inizializzazione del dataset per il report
        /// dell'esito delle operazioni massive.
        /// </summary>
        /// <returns>Un dataset con tre colonne: IdProfile per </returns>
        private DataSet Initialize()
        {
            try
            {
                // Il dataset da restituire
                DataSet toReturn;

                // La tabella da inserire nel report
                DataTable table;

                // Creazione del dataset
                toReturn = new DataSet();

                // Creazione della tabella
                table = new DataTable();

                // Aggiunta della tabella al dataset
                toReturn.Tables.Add(table);

                // Creazione e aggiunta delle colonne
                table.Columns.Add("ObjId", typeof(string));
                table.Columns.Add("Result", typeof(string));
                table.Columns.Add("Details", typeof(string));

                // Restituzione del report inizializzato
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per l'aggiunta di una riga al report
        /// </summary>
        /// <param name="objId">Id dell'oggetto cui si riferisce la riga del report</param>
        /// <param name="result">Esito dell'operazione</param>
        /// <param name="details">Dettagli sull'operazione</param>
        public void AddReportRow(string objId, MassiveOperationResultEnum result, string details)
        {
            try
            {
                // La riga da aggiungere
                DataRow dataRow;

                // La tabella a cui aggiungere la riga
                DataTable dataTable;

                // L'esito dell'operazione decodificato
                string resultDecoded;

                // Prelevamento della tabella a cui bisogna aggiungere la riga
                dataTable = this.reportDataSet.Tables[0];

                switch (result)
                {
                    case MassiveOperationResultEnum.OK:
                        resultDecoded = result.ToString();
                        this.worked++;
                        break;

                    case MassiveOperationResultEnum.KO:
                        resultDecoded = result.ToString();
                        this.notWorked++;
                        break;

                    case MassiveOperationResultEnum.AlreadyWorked:
                        this.alreadyWorked++;
                        resultDecoded = "Già lavorato";
                        break;

                    default:
                        resultDecoded = result.ToString();
                        break;
                }

                // Creazione della riga
                dataRow = dataTable.NewRow();

                // Inizializzazione dei dati per la riga
                dataRow["ObjId"] = objId;
                dataRow["Result"] = resultDecoded;
                dataRow["Details"] = details;

                // Aggiunta della riga alla tabell
                dataTable.Rows.Add(dataRow);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Funzione per l'aggiunta di una riga di riepilogo al
        /// report. Questa riga conterrà il numero di documenti lavorati,
        /// il numero di documenti non lavorati ed il numero di documenti
        /// che erano già nello stato lavorato.
        /// </summary>
        /// <param name="objectName">Nome del tipo di oggetto per cui è utilizzato questo report</param>
        public void AddSummaryRow(string objectName)
        {
            try
            {
                // La riga da aggiungere
                DataRow dataRow;

                // Inizializzaizone di una nuova riga
                dataRow = this.reportDataSet.Tables[0].NewRow();

                // Compilazione dei dati della riga
                dataRow["ObjId"] = "N.A.";
                dataRow["Result"] = "";
                dataRow["Details"] = String.Format(
                    "{0} lavorati: {1} - {0} non lavorati: {2} - {0} già nello stato lavorato: {3}",
                    objectName,
                    this.worked,
                    this.notWorked,
                    this.alreadyWorked);

                // Inserimento della riga
                this.reportDataSet.Tables[0].Rows.Add(dataRow);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Funzione per l'aggiunta di una riga di riepilogo al
        /// report. Questa riga conterrà la stringa passata dall'utente come messaggio, con
        /// gli opportuni parametri
        /// </summary>
        /// <param name="message">Messaggio da inserire nella riga</param>
        /// <param name="pars">Parametri da inserire nel messaggio</param>

        public void AddSummaryRow(string message, string[] pars)
        {
            try
            {
                // La riga da aggiungere
                DataRow dataRow;

                // Inizializzaizone di una nuova riga
                dataRow = this.reportDataSet.Tables[0].NewRow();

                // Compilazione dei dati della riga
                dataRow["ObjId"] = "N.A.";
                dataRow["Result"] = "";
                dataRow["Details"] = String.Format(
                    message, pars);
                // Inserimento della riga
                this.reportDataSet.Tables[0].Rows.Add(dataRow);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Funzione per la restituzione del dataset.
        /// I dati contenuti nel data set sono identificati dai seguenti
        /// nomi:
        /// ObjId - Id dell'oggetto
        /// Result - Esito
        /// Details - Dettagli
        /// </summary>
        /// <returns>Il dataset del report</returns>
        public DataSet GetDataSet()
        {
            try
            {
                return this.reportDataSet;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per la generazione del report per l'esportazione del report in formato PDF
        /// </summary>
        /// <param name="templateFilePath">Il path in cui risiede il template per la generazione del report</param>
        public void GenerateDataSetForExport(string templateFilePath,string titolo)
        {
            try
            {
                // Conversione del dataset
                DataSet convertedDataSet = this.ConvertDataset(this.reportDataSet);
                System.Web.HttpContext.Current.Session["reportImport"] =
                    ReportGenerator.GetReport(
                        templateFilePath,
                        convertedDataSet,
                        titolo);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Funzione per la generazione del report
        /// </summary>
        /// <param name="dataSet">Il dataset da convertire</param>
        /// <returns>Il dataset da convertire</returns>
        private DataSet ConvertDataset(DataSet dataSet)
        {
            try
            {
                // Il dataset da restituire
                DataSet toReturn;
                // La tabella da inserire nel dataset
                DataTable dataTable;
                // La riga da inserire nel dataset
                DataRow dataRow;
                // L'ordinale
                int ordinalNumber = 1;

                // Inizializzazione del dataset
                toReturn = new DataSet();
                dataTable = new DataTable();
                dataTable.Columns.Add("Ordinale", typeof(string));
                dataTable.Columns.Add("Id", typeof(string));
                dataTable.Columns.Add("Risultato", typeof(string));
                dataTable.Columns.Add("Dettagli", typeof(string));
                toReturn.Tables.Add(dataTable);

                // Conversione del dataset
                foreach (DataRow row in this.reportDataSet.Tables[0].Rows)
                {
                    dataRow = dataTable.NewRow();
                    dataRow["Ordinale"] = ordinalNumber++;
                    dataRow["Id"] = row["ObjId"].ToString();
                    dataRow["Risultato"] = row["Result"].ToString();
                    dataRow["Dettagli"] = row["Details"].ToString();
                    dataTable.Rows.Add(dataRow);
                }

                // Restituzione del report convertito
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
    }

    /// <summary>
    /// Oggetto per la gestione della lista dei dettagli sulle singole trasmissione
    /// </summary>
    public class MassiveOperationTrasmissionDetailsCollection
    {
        // La collezione degli elementi con i dettagli delle singole trasmissioni
        private Dictionary<string, MassiveOperationTransmissionDetailsElement> transmissionCollection;

        // La trasmissione in lavorazione
        private Trasmissione transmission;

        /// <summary>
        /// Metodo costruttore per l'inizializzazione della lista dei dettagli delle
        /// trasmissioni da effettuare
        /// </summary>
        public MassiveOperationTrasmissionDetailsCollection(string objType)
        {
            try
            {
                this.transmissionCollection = new Dictionary<string, MassiveOperationTransmissionDetailsElement>();
                this.transmission = new Trasmissione();
                this.transmission.ruolo = UIManager.RoleManager.GetRoleInSession();
                this.transmission.utente = UserManager.GetUserInSession(); ;
                this.transmission.tipoOggetto =
                    objType.Equals("D") ? TrasmissioneTipoOggetto.DOCUMENTO : TrasmissioneTipoOggetto.FASCICOLO;
                this.NoNotify = false;
                this.FromModel = false;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public int GetSingleTransmissionNumber()
        {
            try
            {
                return this.transmissionCollection.Values.Count();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return -1;
            }
        }

        /// <summary>
        /// Funzione per la restituzione dei dati di dettaglio relativi
        /// ad una specifica operazione.
        /// </summary>
        /// <param name="key">La chiave della trasmissione da restituire</param>
        /// <returns>I dettagli della trasmissione richiesta</returns>
        public MassiveOperationTransmissionDetailsElement GetElement(string key)
        {
            try
            {
                return this.transmissionCollection[key.ToUpper()];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public void SetElement(string key, MassiveOperationTransmissionDetailsElement value)
        {
            try
            {
                this.transmissionCollection[key.ToUpper()] = value;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// La lista dei dettagli sulle trasmissioni da effettuare
        /// </summary>
        /// <returns>Un array con i dettagli sulle trasmissioni da effettuare</returns>
        public MassiveOperationTransmissionDetailsElement[] GetTransmissionElementArray()
        {
            try
            {
                return this.transmissionCollection.Values.ToArray<MassiveOperationTransmissionDetailsElement>();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public TrasmissioneSingola GetSingleTransmissionForUser(string corrSystemId)
        {
            try
            {
                TrasmissioneSingola toReturn = null;

                foreach (MassiveOperationTransmissionDetailsElement element in
                    this.transmissionCollection.Values)
                    if (element.SingleTrasmission.corrispondenteInterno.systemId == corrSystemId)
                        toReturn = element.SingleTrasmission;

                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public int GetNumberOfTransmissionsWithCessionReason()
        {
            try
            {
                int toReturn = 0;

                foreach (MassiveOperationTransmissionDetailsElement element in this.transmissionCollection.Values)
                    if (element.SingleTrasmission.ragione.cessioneImpostata)
                        toReturn++;

                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return -1;
            }
        }

        /// <summary>
        /// Funzione per l'aggiunta dei dettagli su una nuova trasmissione singola
        /// </summary>
        /// <param name="singleTransmission">La trasmissione singola da aggiungere</param>
        public void AddTransmissionDetails(TrasmissioneSingola singleTransmission)
        {
            try
            {
                // L'oggetto da aggiungere alla collezione
                MassiveOperationTransmissionDetailsElement element;

                // Creazione dell'elemento
                element = new MassiveOperationTransmissionDetailsElement();

                // Impostazione delle varie proprietà del nuovo elemento
                element.SingleTrasmission = singleTransmission;
                element.ExpirationDate = singleTransmission.dataScadenza;
                element.Reason = singleTransmission.ragione.descrizione;
                element.RecivierDescription = singleTransmission.corrispondenteInterno.descrizione;
                element.Type = singleTransmission.tipoTrasm;
                element.UserTransmission = singleTransmission.trasmissioneUtente;
                element.Id = Guid.NewGuid().ToString().ToUpper();
                element.Note = singleTransmission.noteSingole;
                element.HidePreviousVersions = singleTransmission.hideDocumentPreviousVersions;

                // Se singleTransmission contiene un'unica trasmissione utente, allora daNotificare
                // è true indipendentemente dalle impostazioni
                if (singleTransmission.trasmissioneUtente.Length == 1)
                    singleTransmission.trasmissioneUtente[0].daNotificare = true;

                // Aggiunta del nuovo elemento 
                this.transmissionCollection.Add(element.Id.ToUpper(), element);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Rimozione di un elemento di dettaglio
        /// </summary>
        /// <param name="key">La chiave dell'elemento da eliminare</param>
        public void RemoveTransmissionDetails(string key)
        {
            try
            {
                this.transmissionCollection.Remove(key);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Questa funzione crea la trasmissione inserendo le informazioni sulle
        /// trasmissioni singole.
        /// Una volta chiamata questa funzione, l'oggetto trasmissione risulterà
        /// predisposto all'invio
        /// </summary>
        /// <returns>L'oggetto trasmissione pronto per essere inviato</returns>
        public Trasmissione CompileTransmissionObject(string generalNotes)
        {
            try
            {
                // Lista delle trasmissioni singole
                List<TrasmissioneSingola> singleTransmissionList;

                // Impostazione delle note generali solo se non sono state già impostate
                if (String.IsNullOrEmpty(this.transmission.noteGenerali))
                    this.transmission.noteGenerali = generalNotes;

                singleTransmissionList = GetSingleTransmissionList();

                // Se la trasmissione non prevede notifica, vengono impostati tutti i flag
                // daNotificare a true
                if (this.NoNotify)
                    foreach (TrasmissioneSingola singleTransmission in singleTransmissionList)
                        foreach (TrasmissioneUtente user in singleTransmission.trasmissioneUtente)
                            user.daNotificare = true;

                // Salvataggio della lista di trasmissioni
                this.transmission.trasmissioniSingole = singleTransmissionList.ToArray();

                // Restituzione della trasmissione creata
                return this.transmission;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per il recupero della lista delle trasmissioni singole
        /// </summary>
        /// <returns>Una lista con le informazioni sulle trasmissioni singole da effettuare</returns>
        public List<TrasmissioneSingola> GetSingleTransmissionList()
        {
            try
            {
                List<TrasmissioneSingola> singleTransmissionList;
                // Creazione della lista delle trasmissioni singole
                singleTransmissionList = new List<TrasmissioneSingola>();

                foreach (MassiveOperationTransmissionDetailsElement details in
                    this.transmissionCollection.Values)
                    singleTransmissionList.Add(details.SingleTrasmission);
                return singleTransmissionList;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Questa funzione serve per verificare se la trasmissione in lavorazione
        /// contiene almeno una trasmissione singola
        /// </summary>
        /// <returns>True se la trasmissione in lavorazione ha almeno una trasmissione singola</returns>
        public bool HaveTransmissions()
        {
            try
            {
                return this.transmissionCollection != null &&
                    this.transmissionCollection.Values.Count > 0;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// True se la trasmissione viene creata da modello
        /// </summary>
        public bool FromModel { get; set; }

        /// <summary>
        /// Funzione per l'impostazione del valore di NO_NOTIFY nella
        /// trasmissione
        /// </summary>
        public bool NoNotify
        {
            set
            {
                try
                {
                    this.transmission.NO_NOTIFY = value ? "1" : "0";
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }

            get
            {
                try
                {
                    return this.transmission.NO_NOTIFY == "1" ? true : false;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// Trasmissione che cede i diritti
        /// </summary>
        public bool LeaseRigths
        {
            get
            {
                try
                {
                    return this.transmission.salvataConCessione;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return false;
                }
            }

            set
            {
                try
                {
                    this.transmission.salvataConCessione = value;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

        /// <summary>
        /// Informazioni sul documento di cui cedere i diritti. (Da modello)
        /// </summary>
        public CessioneDocumento DocumentLeasing {
            get
            {
                try
                {
                    return this.transmission.cessione;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    this.transmission.cessione = value;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

        /// <summary>
        /// Note generali della trasmissione
        /// </summary>
        public string GeneralNotes
        {
            get
            {
                try
                {
                    return this.transmission.noteGenerali;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    this.transmission.noteGenerali = value;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

    }

    /// <summary>
    /// Il dettaglio di una singola trasmissione
    /// </summary>
    public class MassiveOperationTransmissionDetailsElement
    {
        /// <summary>
        /// Identificativo univoco dell'oggetto
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// La trasmissione singola legata a questo dettaglio
        /// </summary>
        public TrasmissioneSingola SingleTrasmission { get; set; }

        /// <summary>
        /// La descrizione del destinario
        /// </summary>
        public string RecivierDescription { get; set; }

        /// <summary>
        /// Nota individuale
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Il tipo di trasmissione
        /// </summary>
        private string type;
        public string Type {
            get
            {
                try
                {
                    return this.type;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    this.type = value;
                    this.ExtendedType = value.Equals("S") ? this.ExtendedType = "Uno" : this.ExtendedType = "Tutti";
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

        /// <summary>
        /// Versione estesa di Type: se Type è S, questa proprietà varrà Uno, altrimenti varrà Tutti
        /// </summary>
        public string ExtendedType { get; set; }

        /// <summary>
        /// La ragione di trasmissione
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// La data di scadenza della trasmissione
        /// </summary>
        public string ExpirationDate { get; set; }

        public bool HidePreviousVersions { get; set; }

        public string HidePreviousVersionsString
        {
            get
            {
                try
                {
                    return HidePreviousVersions ? "SI" : "NO";
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
        }

        public TrasmissioneUtente[] UserTransmission { get; set; }

    }

    public class MassiveOperationTarget
    {
        private string _codice;
        private string _id;

        public MassiveOperationTarget(string id, string codice)
        {
            try
            {
                this._id = id;
                this._codice = codice;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public string Id
        {
            get
            {
                try
                {
                    return _id;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
        }

        public string Codice
        {
            get
            {
                try
                {
                    return _codice;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
        }

        public MassiveOperationTarget Clone
        {
            get
            {
                try
                {
                    MassiveOperationTarget res = new MassiveOperationTarget(Id, Codice);
                    res.Checked = Checked;
                    return res;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
        }

        public bool Checked
        {
            get;
            set;
        }
    }

    public class ReportGenerator
    {
        /// <summary>
        /// Funzione per la generazione del file documento con il report dell'esportazione
        /// </summary>
        /// <param name="templateFilePath">Il path in cui reperire il template per l'espoertazione</param>
        /// <param name="dataSet">Il data set con le informazioni sul report</param>
        /// <returns>L'oggetto file documento con il report</returns>
        public static FileDocumento GetReport(string templateFilePath, DataSet dataSet, string titolo)
        {
            // L'oggetto file documento con le informazioni sul report generato
            FileDocumento toReturn;

            //// Creazione oggetto file documento
            toReturn = ProspettiRiepilogativi.Frontend.PdfReport.do_MakePdfReport(
                ProspettiRiepilogativi.Frontend.ReportDisponibili.ReportLogMassiveImport,
                templateFilePath,
                titolo,
                dataSet,
                null);

            // Restituzione oggetto
            return toReturn;

        }
    }

}
