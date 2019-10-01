using System;
using System.Collections.Generic;
using DocsPAWA.SiteNavigation;
using System.Data;
using DocsPAWA.Import.ReportGenerator;
using DocsPAWA.DocsPaWR;
using System.Linq;

namespace DocsPAWA.utils
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
                return CallContextStack.CurrentContext.ContextState["massiveOperation"] as Dictionary<String, MassiveOperationTarget>;
            }
            set
            {
                CallContextStack.CurrentContext.ContextState["massiveOperation"] = value;
            }
        }

        public static MassiveOperationTarget getItem(string id)
        {
            MassiveOperationTarget res = ItemsStatus[id].Clone;
            return res;
        }

        /// <summary>
        /// Funzione per il reperimento della lista di system id relativi agli item
        /// selezionati
        /// </summary>
        public static List<MassiveOperationTarget> GetSelectedItems()
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
                    if (status[key].Checked)
                        toReturn.Add(status[key]);
            }

            // Restituzione della lista di system id
            return toReturn;
        }

        /// <summary>
        /// Funzione utilizzata per verificare se la collezione dello stato
        /// degli item è valorizzata e contiene valori
        /// </summary>
        /// <returns>True se la collezione degli elementi è valorizzata e contiene valori</returns>
        public static bool IsCollectionInitialized()
        {
            return ItemsStatus != null &&
                ItemsStatus.Values.Count > 0;
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
            // Creazione e inizializzazione del dataset.
            this.reportDataSet = this.Initialize();

            this.worked = 0;
            this.notWorked = 0;
            this.alreadyWorked = 0;

        }

        public int Worked
        {
            get
            {
                return worked;
            }
        }

        public int NotWorked
        {
            get
            {
                return notWorked;
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

        /// <summary>
        /// Funzione per l'aggiunta di una riga al report
        /// </summary>
        /// <param name="objId">Id dell'oggetto cui si riferisce la riga del report</param>
        /// <param name="result">Esito dell'operazione</param>
        /// <param name="details">Dettagli sull'operazione</param>
        public void AddReportRow(string objId, MassiveOperationResultEnum result, string details)
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

        /// <summary>
        /// Funzione per l'aggiunta di una riga di riepilogo al
        /// report. Questa riga conterrà il numero di documenti lavorati,
        /// il numero di documenti non lavorati ed il numero di documenti
        /// che erano già nello stato lavorato.
        /// </summary>
        /// <param name="objectName">Nome del tipo di oggetto per cui è utilizzato questo report</param>
        public void AddSummaryRow(string objectName)
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

        /// <summary>
        /// Funzione per l'aggiunta di una riga di riepilogo al
        /// report. Questa riga conterrà la stringa passata dall'utente come messaggio, con
        /// gli opportuni parametri
        /// </summary>
        /// <param name="message">Messaggio da inserire nella riga</param>
        /// <param name="pars">Parametri da inserire nel messaggio</param>

        public void AddSummaryRow(string message, string[] pars)
        {
            // La riga da aggiungere
            DataRow dataRow;

            // Inizializzaizone di una nuova riga
            dataRow = this.reportDataSet.Tables[0].NewRow();

            // Compilazione dei dati della riga
            dataRow["ObjId"] = "N.A.";
            dataRow["Result"] = "";
            dataRow["Details"] = String.Format(
                message,pars);
            // Inserimento della riga
            this.reportDataSet.Tables[0].Rows.Add(dataRow);
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
            return this.reportDataSet;
        }

        /// <summary>
        /// Funzione per la generazione del report per l'esportazione del report in formato PDF
        /// </summary>
        /// <param name="templateFilePath">Il path in cui risiede il template per la generazione del report</param>
        public void GenerateDataSetForExport(string templateFilePath,string titolo)
        {
            // Conversione del dataset
            DataSet convertedDataSet = this.ConvertDataset(this.reportDataSet);

            CallContextStack.CurrentContext.ContextState["reportImport"] =
                ReportGenerator.GetReport(
                    templateFilePath,
                    convertedDataSet,
                    titolo);

        }

        /// <summary>
        /// Funzione per la generazione del report
        /// </summary>
        /// <param name="dataSet">Il dataset da convertire</param>
        /// <returns>Il dataset da convertire</returns>
        private DataSet ConvertDataset(DataSet dataSet)
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
            this.transmissionCollection = new Dictionary<string, MassiveOperationTransmissionDetailsElement>();
            this.transmission = new Trasmissione();
            this.transmission.ruolo = UserManager.getRuolo();
            this.transmission.utente = UserManager.getUtente();
            this.transmission.tipoOggetto =
                objType.Equals("D") ? TrasmissioneTipoOggetto.DOCUMENTO : TrasmissioneTipoOggetto.FASCICOLO;
            this.NoNotify = false;
            this.FromModel = false;

        }

        public int GetSingleTransmissionNumber()
        {
            return this.transmissionCollection.Values.Count();
        }

        /// <summary>
        /// Funzione per la restituzione dei dati di dettaglio relativi
        /// ad una specifica operazione.
        /// </summary>
        /// <param name="key">La chiave della trasmissione da restituire</param>
        /// <returns>I dettagli della trasmissione richiesta</returns>
        public MassiveOperationTransmissionDetailsElement GetElement(string key)
        {
            return this.transmissionCollection[key.ToUpper()];

        }

        /// <summary>
        /// La lista dei dettagli sulle trasmissioni da effettuare
        /// </summary>
        /// <returns>Un array con i dettagli sulle trasmissioni da effettuare</returns>
        public MassiveOperationTransmissionDetailsElement[] GetTransmissionElementArray()
        {
            return this.transmissionCollection.Values.ToArray<MassiveOperationTransmissionDetailsElement>();

        }

        public TrasmissioneSingola GetSingleTransmissionForUser(string corrSystemId)
        {
            TrasmissioneSingola toReturn = null;

            foreach (MassiveOperationTransmissionDetailsElement element in
                this.transmissionCollection.Values)
                if (element.SingleTrasmission.corrispondenteInterno.systemId == corrSystemId)
                    toReturn = element.SingleTrasmission;

            return toReturn;
        }

        public int GetNumberOfTransmissionsWithCessionReason()
        {
            int toReturn = 0;

            foreach (MassiveOperationTransmissionDetailsElement element in this.transmissionCollection.Values)
                if (element.SingleTrasmission.ragione.cessioneImpostata)
                    toReturn++;

            return toReturn;

        }

        /// <summary>
        /// Funzione per l'aggiunta dei dettagli su una nuova trasmissione singola
        /// </summary>
        /// <param name="singleTransmission">La trasmissione singola da aggiungere</param>
        public void AddTransmissionDetails(TrasmissioneSingola singleTransmission)
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

        /// <summary>
        /// Rimozione di un elemento di dettaglio
        /// </summary>
        /// <param name="key">La chiave dell'elemento da eliminare</param>
        public void RemoveTransmissionDetails(string key)
        {
            this.transmissionCollection.Remove(key);
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
            // Lista delle trasmissioni singole
            List<TrasmissioneSingola> singleTransmissionList;

            // Impostazione delle note generali solo se non sono state già impostate
            if(String.IsNullOrEmpty(this.transmission.noteGenerali))
                this.transmission.noteGenerali = generalNotes;

            singleTransmissionList = GetSingleTransmissionList();

            // Se la trasmissione non prevede notifica, vengono impostati tutti i flag
            // daNotificare a true
            if (this.NoNotify)
                foreach (TrasmissioneSingola singleTransmission in singleTransmissionList)
                    foreach(TrasmissioneUtente user in singleTransmission.trasmissioneUtente)
                        user.daNotificare = true;

            // Salvataggio della lista di trasmissioni
            this.transmission.trasmissioniSingole = singleTransmissionList.ToArray();

            // Restituzione della trasmissione creata
            return this.transmission;

        }

        /// <summary>
        /// Funzione per il recupero della lista delle trasmissioni singole
        /// </summary>
        /// <returns>Una lista con le informazioni sulle trasmissioni singole da effettuare</returns>
        public List<TrasmissioneSingola> GetSingleTransmissionList()
        {
            List<TrasmissioneSingola> singleTransmissionList;
            // Creazione della lista delle trasmissioni singole
            singleTransmissionList = new List<TrasmissioneSingola>();

            foreach (MassiveOperationTransmissionDetailsElement details in
                this.transmissionCollection.Values)
                singleTransmissionList.Add(details.SingleTrasmission);
            return singleTransmissionList;
        }

        /// <summary>
        /// Questa funzione serve per verificare se la trasmissione in lavorazione
        /// contiene almeno una trasmissione singola
        /// </summary>
        /// <returns>True se la trasmissione in lavorazione ha almeno una trasmissione singola</returns>
        public bool HaveTransmissions()
        {
            return this.transmissionCollection != null &&
                this.transmissionCollection.Values.Count > 0;
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
                this.transmission.NO_NOTIFY = value ? "1" : "0";
            }

            get
            {
                return this.transmission.NO_NOTIFY == "1" ? true : false ;
            }
        }

        /// <summary>
        /// Trasmissione che cede i diritti
        /// </summary>
        public bool LeaseRigths
        {
            get
            {
                return this.transmission.salvataConCessione;
            }

            set
            {
                this.transmission.salvataConCessione = value;
            }
        }

        /// <summary>
        /// Informazioni sul documento di cui cedere i diritti. (Da modello)
        /// </summary>
        public CessioneDocumento DocumentLeasing {
            get
            {
                return this.transmission.cessione;
            }
            set
            {
                this.transmission.cessione = value;
            }
        }

        /// <summary>
        /// Note generali della trasmissione
        /// </summary>
        public string GeneralNotes
        {
            get
            {
                return this.transmission.noteGenerali;
            }
            set
            {
                this.transmission.noteGenerali = value;
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
                return this.type;
            }
            set
            { 
                this.type = value;
                this.ExtendedType = value.Equals("S") ? this.ExtendedType = "Uno" : this.ExtendedType = "Tutti";
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
                return HidePreviousVersions ? "SI" : "NO";
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
            this._id = id;
            this._codice = codice;
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }

        public string Codice
        {
            get
            {
                return _codice;
            }
        }

        public MassiveOperationTarget Clone
        {
            get
            {
                MassiveOperationTarget res = new MassiveOperationTarget(Id, Codice);
                res.Checked = Checked;
                return res;
            }
        }

        public bool Checked
        {
            get;
            set;
        }
    }
}