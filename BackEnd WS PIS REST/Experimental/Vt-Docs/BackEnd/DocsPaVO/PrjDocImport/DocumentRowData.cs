using System;
using System.Collections.Generic;
using System.Linq;
using DocsPaVO.Note;

namespace DocsPaVO.PrjDocImport
{
    /// <summary>
    /// Classe per la rappresentazione di dati relativi ad un documento o ad un
    /// fascicolo.
    /// </summary>
    [Serializable()]
    public class DocumentRowData
    {
        /// <summary>
        /// Funzione per la creazione di un nuovo oggetto RowData
        /// </summary>
        public DocumentRowData()
        {
            this.DocumentProfilationData = new List<ProfilationFieldInformation>();
            this.ProjectProfilationData = new List<ProfilationFieldInformation>();
        }

        /// <summary>
        /// L'ordinale
        /// </summary>
        public string OrdinalNumber { get; set; }

        /// <summary>
        /// L'ordinale del documento principale (utilizzato dal foglio excel Allegati)
        /// </summary>
        public string MainOrdinal { get; set; }

        /// <summary>
        /// Il codice dell'amministrazione
        /// </summary>
        public string AdminCode { get; set; }

        /// <summary>
        /// Numero da assegnare al protocollo. Sviluppo per abilitare Documenti pregressi.
        /// </summary>
        public string ProtocolNumber { get; set; }

        /// <summary>
        /// Data di creazione del protocollo. Sviluppo per abilitare Documenti pregressi.
        /// </summary>
        public DateTime ProtocolDate { get; set; }

        /// <summary>
        /// Codice utente creatore del protocollo. Sviluppo per abilitare Documenti pregressi.
        /// </summary>
        public String CodiceUtenteCreatore { get; set; }

        /// <summary>
        /// Codice Ruolo creatore del protocollo. Sviluppo per abilitare Documenti pregressi.
        /// </summary>
        public String CodiceRuoloCreatore { get; set; }

        /// <summary>
        /// Il codice del registro
        /// </summary>
        public string RegCode { get; set; }

        /// <summary>
        /// Il codice dell'RF
        /// </summary>
        public string RFCode { get; set; }

        /// <summary>
        /// Il codice dell'oggetto del documento
        /// </summary>
        public string ObjCode { get; set; }

        /// <summary>
        /// L'oggetto
        /// </summary>
        public string Obj { get; set; }

        /// <summary>
        /// Il codice del corrispondente
        /// </summary>
        public List<String> CorrCode { get; set; }

        /// <summary>
        /// La descrizione del corrispondente occasionale
        /// </summary>
        public List<String> CorrDesc { get; set; }

        /// <summary>
        /// Il pathname completo del file da associare al documento
        /// </summary>
        public string Pathname { get; set; }

        /// <summary>
        /// Booleano che indica se il documento deve essere messo in area di lavoro
        /// </summary>
        public bool InWorkingArea { get; set; }

        /// <summary>
        /// La nota da aggiungere al documento
        /// </summary>
        public InfoNota Note { get; set; }

        /// <summary>
        /// I codici dei modelli di trasmissione da utilizzare per inviare
        /// il documento
        /// </summary>
        public string[] TransmissionModelCode { get; set; }

        /// <summary>
        /// La tipologia di profilazione associata al documento
        /// </summary>
        public string DocumentTipology { get; set; }

        /// <summary>
        /// L'oggetto per memorizzare i dati relativi alla profilazione dei documenti.
        /// </summary>
        public List<ProfilationFieldInformation> DocumentProfilationData { get; set; }

        /// <summary>
        /// Funzione per l'aggiunta di un campo profilato alla lista dei campi profilati
        /// </summary>
        /// <param name="fieldName">Il nome del campo</param>
        /// <param name="values">Il valore o i valori da associare al campo</param>
        public void AddDocumentProfilationField(string fieldName, string[] values)
        {
            this.DocumentProfilationData.Add(new ProfilationFieldInformation()
            {
                Label = fieldName.ToUpper().Trim(),
                Values = values
            });
        }

        /// <summary>
        /// Funzione per la ricerca di un campo profilato all'interno della lista
        /// dei campi profilati legati al documento
        /// </summary>
        /// <param name="fieldLabel">La label del campo di cui si desidera ricevere i dati</param>
        /// <returns>L'insieme dei valori assegnati al campo</returns>
        public string[] GetDocumentProfilationField(string fieldLabel)
        {
            // Le informazioni sul campo richiesto
            ProfilationFieldInformation fieldInformation = null;

            // L'oggetto da restituire
            string[] toReturn = null; 

            // Selezione dell'elemento
            fieldInformation = this.DocumentProfilationData.Where(
                e => e.Label == fieldLabel.ToUpper()).FirstOrDefault();

            if (fieldInformation != null)
                toReturn = fieldInformation.Values;

            // Restituzione valore
            return toReturn;

        }

        /// <summary>
        /// I codici dei fascicoli in cui fascicolare il documento
        /// </summary>
        public String[] ProjectCodes { get; set; }

        /// <summary>
        /// La descrizione del fascicolo in cui fascicolare il documento
        /// </summary>
        public string ProjectDescription { get; set; }

        /// <summary>
        /// La descrizione del sottofascicolo
        /// </summary>
        public string FolderDescrition { get; set; }

        /// <summary>
        /// La descrizione del titolario in cui ricercare il documento
        /// </summary>
        public string Titolario { get; set; }

        /// <summary>
        /// Il codice del nodo
        /// </summary>
        public string NodeCode { get; set; }

        /// <summary>
        /// La tipologia di profilazione associata al fascicolo
        /// </summary>
        public string ProjectTipology { get; set; }

        /// <summary>
        /// L'oggetto per memorizzare i dati relativi alla profilazione dei fascicoli.
        /// </summary>
        public List<ProfilationFieldInformation> ProjectProfilationData { get; set; }

        /// <summary>
        /// Funzione per l'aggiunta di un campo profilato alla lista dei campi profilati
        /// relativi al fascicolo
        /// </summary>
        /// <param name="fieldName">Il nome del campo</param>
        /// <param name="values">Il valore o i valori da associare al campo</param>
        public void AddProjectProfilationField(string fieldName, string[] values)
        {
            this.ProjectProfilationData.Add(new ProfilationFieldInformation()
            {
                Label = fieldName.ToUpper().Trim(),
                Values = values
            });

        }

        /// <summary>
        /// Funzione per la ricerca di un campo profilato all'interno della lista
        /// dei campi profilati legati al fascicolo
        /// </summary>
        /// <param name="fieldLabel">L'etichetta associata al campo di cui si desidera conoscere le informazioni</param>
        /// <returns>I valori associati al campo</returns>
        public string[] GetProjectProfilationField(string fieldLabel)
        {
            // Le informazioni sul campo richiesto
            ProfilationFieldInformation fieldInformation = null;

            // L'oggetto da restituire
            string[] toReturn = null;

            // Selezione dell'elemento
            fieldInformation = this.ProjectProfilationData.Where(
                e => e.Label == fieldLabel.ToUpper()).FirstOrDefault();

            if (fieldInformation != null)
                toReturn = fieldInformation.Values;

            // Restituzione valore
            return toReturn;

        }

        /// <summary>
        /// Funzione per la verfica di presenza elementi nella collezione
        /// dei campi profilati relativi ai fascicoli
        /// </summary>
        /// <returns>True se esistono elementi, false altrimenti</returns>
        public bool ExistProjectProfilationFields()
        {
            return this.ProjectProfilationData.Count > 0;
        }

        public bool IsEmpty()
        {
            return VoidValue(this.AdminCode) &&
                VoidValue(this.CorrCode) &&
                VoidValue(this.CorrDesc) &&
                VoidValue(this.DocumentTipology) &&
                VoidValue(this.FolderDescrition) &&
                VoidValue(this.MainOrdinal) &&
                VoidValue(this.NodeCode) &&
                VoidValue(this.Obj) &&
                VoidValue(this.ObjCode) &&
                VoidValue(this.OrdinalNumber) &&
                VoidValue(this.Pathname) &&
                VoidValue(this.ProjectCodes) &&
                VoidValue(this.ProjectDescription) &&
                VoidValue(this.ProjectTipology) &&
                VoidValue(this.RegCode) &&
                VoidValue(this.RFCode) &&
                VoidValue(this.Titolario) &&
                VoidValue(this.TransmissionModelCode);
        }

        private bool VoidValue(string value)
        {
            return value == null || string.IsNullOrEmpty(value.Trim());
        }

        private bool VoidValue(List<string> value)
        {
            if (value == null || value.Count == 0) return true;
            return false;
        }

        private bool VoidValue(string[] value)
        {
            if (value == null || value.Length == 0) return true;
            foreach (string temp in value)
            {
                if (!string.IsNullOrEmpty(temp)) return false;
            }
            return true;
        }

    }
}
